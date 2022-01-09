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
            div_32x32_shift - perform a signed divide of
            two 32 bit values, shifting the first before
            division, and returning the 32 bit quotient
        -------------------------------------------------*/

        //#ifndef div_32x32_shift
        public static int32_t div_32x32_shift(int32_t a, int32_t b, uint8_t shift)
        {
            return (int32_t)(((int64_t)a << shift) / (int64_t)b);
        }


        /*-------------------------------------------------
            divu_64x32 - perform an unsigned 64 bit x 32 bit
            divide and return the 32 bit quotient
        -------------------------------------------------*/
        public static uint32_t divu_64x32(uint64_t a, uint32_t b)
        {
            return (uint32_t)(a / (uint64_t)b);
        }

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


        /***************************************************************************
            INLINE TIMING FUNCTIONS
        ***************************************************************************/

        /*-------------------------------------------------
            get_profile_ticks - return a tick counter
            from the processor that can be used for
            profiling. It does not need to run at any
            particular rate.
        -------------------------------------------------*/
        //#ifndef get_profile_ticks
        public static int64_t get_profile_ticks()
        {
            return (int64_t)m_osdcore.osd_ticks();
        }
        //#endif
    }
}
