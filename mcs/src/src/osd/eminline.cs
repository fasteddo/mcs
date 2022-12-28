// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int32_t = System.Int32;
using int64_t = System.Int64;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.eminline_global;
using static mame.osdcore_global;


namespace mame
{
    public static class eminline_global
    {
        /*-------------------------------------------------
            mul_16x16 - perform a signed 16 bit x 16 bit
            multiply and return the full 32 bit result
        -------------------------------------------------*/
        //constexpr int32_t mul_16x16(int16_t a, int16_t b)
        //{
        //    return int32_t(a) * int32_t(b);
        //}


        /*-------------------------------------------------
            mul_32x32 - perform a signed 32 bit x 32 bit
            multiply and return the full 64 bit result
        -------------------------------------------------*/
        public static int64_t mul_32x32(int32_t a, int32_t b)
        {
            return (int64_t)a * (int64_t)b;
        }


        /*-------------------------------------------------
            mulu_32x32 - perform an unsigned 32 bit x
            32 bit multiply and return the full 64 bit
            result
        -------------------------------------------------*/
        public static uint64_t mulu_32x32(uint32_t a, uint32_t b)
        {
            return (uint64_t)a * (uint64_t)b;
        }


        /*-------------------------------------------------
            mul_32x32_hi - perform a signed 32 bit x 32 bit
            multiply and return the upper 32 bits of the
            result
        -------------------------------------------------*/
        //#ifndef mul_32x32_hi
        public static int32_t mul_32x32_hi(int32_t a, int32_t b)
        {
            return (int32_t)((uint32_t)(((int64_t)a * (int64_t)b) >> 32));
        }


        /*-------------------------------------------------
            mulu_32x32_hi - perform an unsigned 32 bit x
            32 bit multiply and return the upper 32 bits
            of the result
        -------------------------------------------------*/
        //constexpr uint32_t mulu_32x32_hi(uint32_t a, uint32_t b)
        //{
        //    return uint32_t((uint64_t(a) * uint64_t(b)) >> 32);
        //}


        /*-------------------------------------------------
            mul_32x32_shift - perform a signed 32 bit x
            32 bit multiply and shift the result by the
            given number of bits before truncating the
            result to 32 bits
        -------------------------------------------------*/
        //constexpr int32_t mul_32x32_shift(int32_t a, int32_t b, uint8_t shift)
        //{
        //    return int32_t((int64_t(a) * int64_t(b)) >> shift);
        //}


        /*-------------------------------------------------
            mulu_32x32_shift - perform an unsigned 32 bit x
            32 bit multiply and shift the result by the
            given number of bits before truncating the
            result to 32 bits
        -------------------------------------------------*/
        //constexpr uint32_t mulu_32x32_shift(uint32_t a, uint32_t b, uint8_t shift)
        //{
        //    return uint32_t((uint64_t(a) * uint64_t(b)) >> shift);
        //}


        /*-------------------------------------------------
            div_64x32 - perform a signed 64 bit x 32 bit
            divide and return the 32 bit quotient
        -------------------------------------------------*/
        //constexpr int32_t div_64x32(int64_t a, int32_t b)
        //{
        //    return a / int64_t(b);
        //}


        /*-------------------------------------------------
            divu_64x32 - perform an unsigned 64 bit x 32 bit
            divide and return the 32 bit quotient
        -------------------------------------------------*/
        public static uint32_t divu_64x32(uint64_t a, uint32_t b)
        {
            return (uint32_t)(a / (uint64_t)b);
        }


        /*-------------------------------------------------
            div_64x32_rem - perform a signed 64 bit x 32
            bit divide and return the 32 bit quotient and
            32 bit remainder
        -------------------------------------------------*/
        //inline int32_t div_64x32_rem(int64_t a, int32_t b, int32_t &remainder)
        //{
        //    int32_t const res(div_64x32(a, b));
        //    remainder = a - (int64_t(b) * res);
        //    return res;
        //}


        /*-------------------------------------------------
            divu_64x32_rem - perform an unsigned 64 bit x
            32 bit divide and return the 32 bit quotient
            and 32 bit remainder
        -------------------------------------------------*/
        public static uint32_t divu_64x32_rem(uint64_t a, uint32_t b, out uint32_t remainder)
        {
            uint32_t res = divu_64x32(a, b);
            remainder = (uint32_t)(a - ((uint64_t)b * res));
            return res;
        }


        /*-------------------------------------------------
            div_32x32_shift - perform a signed divide of
            two 32 bit values, shifting the first before
            division, and returning the 32 bit quotient
        -------------------------------------------------*/
        public static int32_t div_32x32_shift(int32_t a, int32_t b, uint8_t shift)
        {
            return (int32_t)(((int64_t)a << shift) / (int64_t)b);
        }


        /*-------------------------------------------------
            divu_32x32_shift - perform an unsigned divide of
            two 32 bit values, shifting the first before
            division, and returning the 32 bit quotient
        -------------------------------------------------*/
        //constexpr uint32_t divu_32x32_shift(uint32_t a, uint32_t b, uint8_t shift)
        //{
        //    return (uint64_t(a) << shift) / uint64_t(b);
        //}


        /*-------------------------------------------------
            mod_64x32 - perform a signed 64 bit x 32 bit
            divide and return the 32 bit remainder
        -------------------------------------------------*/
        //constexpr int32_t mod_64x32(int64_t a, int32_t b)
        //{
        //    return a - (b * div_64x32(a, b));
        //}


        /*-------------------------------------------------
            modu_64x32 - perform an unsigned 64 bit x 32 bit
            divide and return the 32 bit remainder
        -------------------------------------------------*/
        //constexpr uint32_t modu_64x32(uint64_t a, uint32_t b)
        //{
        //    return a - (b * divu_64x32(a, b));
        //}


        /*-------------------------------------------------
            recip_approx - compute an approximate floating
            point reciprocal
        -------------------------------------------------*/
        //constexpr float recip_approx(float value)
        //{
        //    return 1.0f / value;
        //}


        /*-------------------------------------------------
            mul_64x64 - perform a signed 64 bit x 64 bit
            multiply and return the full 128 bit result
        -------------------------------------------------*/
        //inline int64_t mul_64x64(int64_t a, int64_t b, int64_t &hi)


        /*-------------------------------------------------
            mulu_64x64 - perform an unsigned 64 bit x 64
            bit multiply and return the full 128 bit result
        -------------------------------------------------*/
        //inline uint64_t mulu_64x64(uint64_t a, uint64_t b, uint64_t &hi)


        /*-------------------------------------------------
            addu_32x32_co - perform an unsigned 32 bit + 32
            bit addition and return the result with carry
            out
        -------------------------------------------------*/
        //inline bool addu_32x32_co(uint32_t a, uint32_t b, uint32_t &sum)
        //{
        //    sum = a + b;
        //    return (a > sum) || (b > sum);
        //}


        /*-------------------------------------------------
            addu_64x64_co - perform an unsigned 64 bit + 64
            bit addition and return the result with carry
            out
        -------------------------------------------------*/
        //inline bool addu_64x64_co(uint64_t a, uint64_t b, uint64_t &sum)
        //{
        //    sum = a + b;
        //    return (a > sum) || (b > sum);
        //}


        /***************************************************************************
            INLINE BIT MANIPULATION FUNCTIONS
        ***************************************************************************/

        /*-------------------------------------------------
            count_leading_zeros_32 - return the number of
            leading zero bits in a 32-bit value
        -------------------------------------------------*/
        //inline uint8_t count_leading_zeros_32(uint32_t val)
        //{
        //    if (!val) return 32U;
        //    uint8_t count;
        //    for (count = 0; int32_t(val) >= 0; count++) val <<= 1;
        //    return count;
        //}


        /*-------------------------------------------------
            count_leading_ones_32 - return the number of
            leading one bits in a 32-bit value
        -------------------------------------------------*/
        //inline uint8_t count_leading_ones_32(uint32_t val)
        //{
        //    uint8_t count;
        //    for (count = 0; int32_t(val) < 0; count++) val <<= 1;
        //    return count;
        //}


        /*-------------------------------------------------
            count_leading_zeros_64 - return the number of
            leading zero bits in a 64-bit value
        -------------------------------------------------*/
        //inline uint8_t count_leading_zeros_64(uint64_t val)
        //{
        //    if (!val) return 64U;
        //    uint8_t count;
        //    for (count = 0; int64_t(val) >= 0; count++) val <<= 1;
        //    return count;
        //}


        /*-------------------------------------------------
            count_leading_ones_64 - return the number of
            leading one bits in a 64-bit value
        -------------------------------------------------*/
        //inline uint8_t count_leading_ones_64(uint64_t val)
        //{
        //    uint8_t count;
        //    for (count = 0; int64_t(val) < 0; count++) val <<= 1;
        //    return count;
        //}


        /*-------------------------------------------------
            population_count_32 - return the number of
            one bits in a 32-bit value
        -------------------------------------------------*/
        public static unsigned population_count_32(uint32_t val)
        {
            // optimal Hamming weight assuming fast 32*32->32
            uint32_t m1 = 0x55555555;
            uint32_t m2 = 0x33333333;
            uint32_t m4 = 0x0f0f0f0f;
            uint32_t h01 = 0x01010101;
            val -= (val >> 1) & m1;
            val = (val & m2) + ((val >> 2) & m2);
            val = (val + (val >> 4)) & m4;
            return (unsigned)((val * h01) >> 24);
        }


        /*-------------------------------------------------
            population_count_64 - return the number of
            one bits in a 64-bit value
        -------------------------------------------------*/
        //inline unsigned population_count_64(uint64_t val)


        /*-------------------------------------------------
            rotl_32 - circularly shift a 32-bit value left
            by the specified number of bits (modulo 32)
        -------------------------------------------------*/
        public static uint32_t rotl_32(uint32_t val, int shift)
        {
            shift &= 31;
            if (shift != 0)
                return val << shift | val >> (32 - shift);
            else
                return val;
        }


        /*-------------------------------------------------
            rotr_32 - circularly shift a 32-bit value right
            by the specified number of bits (modulo 32)
        -------------------------------------------------*/
        //constexpr uint32_t rotr_32(uint32_t val, int shift)
        //{
        //    shift &= 31;
        //    if (shift)
        //        return val >> shift | val << (32 - shift);
        //    else
        //        return val;
        //}


        /*-------------------------------------------------
            rotl_64 - circularly shift a 64-bit value left
            by the specified number of bits (modulo 64)
        -------------------------------------------------*/
        //constexpr uint64_t rotl_64(uint64_t val, int shift)
        //{
        //    shift &= 63;
        //    if (shift)
        //        return val << shift | val >> (64 - shift);
        //    else
        //        return val;
        //}


        /*-------------------------------------------------
            rotr_64 - circularly shift a 64-bit value right
            by the specified number of bits (modulo 64)
        -------------------------------------------------*/
        //constexpr uint64_t rotr_64(uint64_t val, int shift)
        //{
        //    shift &= 63;
        //    if (shift)
        //        return val >> shift | val << (64 - shift);
        //    else
        //        return val;
        //}


        /***************************************************************************
            INLINE TIMING FUNCTIONS
        ***************************************************************************/

        /*-------------------------------------------------
            get_profile_ticks - return a tick counter
            from the processor that can be used for
            profiling. It does not need to run at any
            particular rate.
        -------------------------------------------------*/
        public static int64_t get_profile_ticks()
        {
            return (int64_t)m_osdcore.osd_ticks();
        }
    }
}
