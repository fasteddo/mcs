// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using attoseconds_t = System.Int64;
using s32 = System.Int32;
using s64 = System.Int64;
using seconds_t = System.Int32;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // core components of the attotime structure
    //typedef Int64 attoseconds_t;
    //typedef int seconds_t;

    public class attotime
    {
        // core definitions
        public const attoseconds_t ATTOSECONDS_PER_SECOND_SQRT = 1000000000;
        public const attoseconds_t ATTOSECONDS_PER_SECOND = ATTOSECONDS_PER_SECOND_SQRT * ATTOSECONDS_PER_SECOND_SQRT;
        public const attoseconds_t ATTOSECONDS_PER_MILLISECOND = ATTOSECONDS_PER_SECOND / 1000;
        public const attoseconds_t ATTOSECONDS_PER_MICROSECOND = ATTOSECONDS_PER_SECOND / 1000000;
        public const attoseconds_t ATTOSECONDS_PER_NANOSECOND = ATTOSECONDS_PER_SECOND / 1000000000;

        public const seconds_t ATTOTIME_MAX_SECONDS = 1000000000;

        // convert between a double and attoseconds
        public static double ATTOSECONDS_TO_DOUBLE(attoseconds_t x) { return (double)x * 1e-18; }
        //#define DOUBLE_TO_ATTOSECONDS(x)        ((attoseconds_t)((x) * 1e18))

        // convert between hertz (as a double) and attoseconds
        public static double ATTOSECONDS_TO_HZ(attoseconds_t x) { return (double)ATTOSECONDS_PER_SECOND / (double)x; }
        public static attoseconds_t HZ_TO_ATTOSECONDS(s32 x) { return ATTOSECONDS_PER_SECOND / (attoseconds_t)x; }
        public static attoseconds_t HZ_TO_ATTOSECONDS(u32 x) { return ATTOSECONDS_PER_SECOND / (attoseconds_t)x; }
        public static attoseconds_t HZ_TO_ATTOSECONDS(attoseconds_t x) { return ATTOSECONDS_PER_SECOND / (attoseconds_t)x; }
        public static attoseconds_t HZ_TO_ATTOSECONDS(u64 x) { return ATTOSECONDS_PER_SECOND / (attoseconds_t)x; }
        public static attoseconds_t HZ_TO_ATTOSECONDS(XTAL x) { return ATTOSECONDS_PER_SECOND / x; }

        // macros for converting other seconds types to attoseconds
        public static attoseconds_t ATTOSECONDS_IN_SEC(u32 x) { return (attoseconds_t)x * ATTOSECONDS_PER_SECOND; }
        public static attoseconds_t ATTOSECONDS_IN_MSEC(u32 x) { return (attoseconds_t)x * ATTOSECONDS_PER_MILLISECOND; }
        public static attoseconds_t ATTOSECONDS_IN_USEC(u32 x) { return (attoseconds_t)x * ATTOSECONDS_PER_MICROSECOND; }
        public static attoseconds_t ATTOSECONDS_IN_NSEC(u32 x) { return (attoseconds_t)x * ATTOSECONDS_PER_NANOSECOND; }


        // constants
        public static readonly attotime zero = new attotime(0, 0);
        public static readonly attotime never = new attotime(ATTOTIME_MAX_SECONDS, 0);


        // members
        public seconds_t m_seconds;
        public attoseconds_t m_attoseconds;


        // construction/destruction
        public attotime() : this(0, 0) { }

        public attotime(seconds_t secs, attoseconds_t attos)
        {
            m_seconds = secs;
            m_attoseconds = attos;
        }

        public attotime(attotime that) : this(that.m_seconds, that.m_attoseconds) { }


        // queries
        public bool is_zero() { return m_seconds == 0 && m_attoseconds == 0; }
        public bool is_never() { return m_seconds >= ATTOTIME_MAX_SECONDS; }


        // conversion to other forms

        public double as_double() { return (double)m_seconds + ATTOSECONDS_TO_DOUBLE(m_attoseconds); }

        //-------------------------------------------------
        //  as_attoseconds - convert to an attoseconds
        //  value, clamping to +/- 1 second
        //-------------------------------------------------
        public attoseconds_t as_attoseconds()
        {
            return
                    (m_seconds == 0) ? m_attoseconds :                              // positive values between 0 and 1 second
                    (m_seconds == -1) ? (m_attoseconds - ATTOSECONDS_PER_SECOND) :  // negative values between -1 and 0 seconds
                    (m_seconds > 0) ? ATTOSECONDS_PER_SECOND :                      // out-of-range positive values
                    -ATTOSECONDS_PER_SECOND;                                        // out-of-range negative values
        }

        //-------------------------------------------------
        //  as_ticks - convert to ticks at the given
        //  frequency
        //-------------------------------------------------
        u64 as_ticks(u32 frequency)
        {
            u32 fracticks = (u32)((new attotime(0, m_attoseconds) * frequency).m_seconds);
            return eminline_global.mulu_32x32((u32)m_seconds, frequency) + fracticks;
        }


        u64 as_ticks(XTAL xtal) { return as_ticks(xtal.value()); }


        //-------------------------------------------------
        //  as_string - return a temporary printable
        //  string describing an attotime
        //-------------------------------------------------
        public string as_string(int precision = 9)
        {
            //static char buffers[8][30];
            //static int nextbuf;
            //char *buffer = &buffers[nextbuf++ % 8][0];
            string buffer;

            // special case: never
            if (this == never)
                buffer = "(never)";

            // case 1: we want no precision; seconds only
            else if (precision == 0)
                buffer = string.Format("%d", m_seconds);

            // case 2: we want 9 or fewer digits of precision
            else if (precision <= 9)
            {
                Int64 upper = m_attoseconds / ATTOSECONDS_PER_SECOND_SQRT;
                int temp = precision;
                while (temp < 9)
                {
                    upper /= 10;
                    temp++;
                }
                buffer = string.Format("{0}.{1}", m_seconds, upper.ToString(new string('0', precision)));  // "%d.%0*d", seconds, precision, upper);
            }

            // case 3: more than 9 digits of precision
            else
            {
                UInt32 lower;
                UInt32 upper = eminline_global.divu_64x32_rem((UInt64)m_attoseconds, (UInt32)ATTOSECONDS_PER_SECOND_SQRT, out lower);
                int temp = precision;
                while (temp < 18)
                {
                    lower /= 10;
                    temp++;
                }
                buffer = string.Format("{0}.{1}{2}", m_seconds, upper, lower);  // "%d.%09d%0*d", seconds, upper, precision - 9, lower);
            }

            return buffer;
        }


        public attoseconds_t attoseconds() { return m_attoseconds; }
        public seconds_t seconds() { return m_seconds; }


        //static attotime from_double(double _time);
        //static attotime from_ticks(UINT64 ticks, UINT32 frequency);
        //static attotime from_ticks(u64 ticks, const XTAL &xtal) { return from_ticks(ticks, xtal.value()); }

        public static attotime from_seconds(int seconds) { return new attotime(seconds, 0); }
        public static attotime from_msec(s64 msec) { return new attotime((int)(msec / 1000), (msec % 1000) * (ATTOSECONDS_PER_SECOND / 1000)); }
        public static attotime from_usec(s64 usec) { return new attotime((int)(usec / 1000000), (usec % 1000000) * (ATTOSECONDS_PER_SECOND / 1000000)); }
        //static attotime from_nsec(INT64 nsec) { return attotime(nsec / 1000000000, (nsec % 1000000000) * (ATTOSECONDS_PER_SECOND / 1000000000)); }
        public static attotime from_hz(double frequency) { global.assert(frequency > 0); double d = 1 / frequency; return new attotime((seconds_t)Math.Floor(d), (attoseconds_t)((d - Math.Truncate(d)) * (double)ATTOSECONDS_PER_SECOND)); }
        public static attotime from_hz(u32 frequency) { return from_hz((double)frequency); }
        public static attotime from_hz(int frequency) { return from_hz((double)frequency); }
        public static attotime from_hz(XTAL xtal) { return from_hz(xtal.dvalue()); }


        //-------------------------------------------------
        //  operator+ - handle addition between two
        //  attotimes
        //-------------------------------------------------
        public static attotime operator+(attotime left, attotime right)
        {
            // if one of the items is never, return never
            if (left.m_seconds >= ATTOTIME_MAX_SECONDS || right.m_seconds >= ATTOTIME_MAX_SECONDS)
                return attotime.never;

            // add the seconds and attoseconds
            attoseconds_t attoseconds = left.m_attoseconds + right.m_attoseconds;
            seconds_t seconds = left.m_seconds + right.m_seconds;
            attotime result = new attotime(seconds, attoseconds);

            // normalize and return
            if (result.m_attoseconds >= ATTOSECONDS_PER_SECOND)
            {
                result.m_attoseconds -= ATTOSECONDS_PER_SECOND;
                result.m_seconds++;
            }

            // overflow
            if (result.m_seconds >= ATTOTIME_MAX_SECONDS)
                return attotime.never;

            return result;
        }

#if false
        public static attotime operator+=(attotime right)
        {
            // if one of the items is never, return never
            if (this.seconds >= ATTOTIME_MAX_SECONDS || right.seconds >= ATTOTIME_MAX_SECONDS)
                return this = never;

            // add the seconds and attoseconds
            attoseconds += right.attoseconds;
            seconds += right.seconds;

            // normalize and return
            if (this.attoseconds >= ATTOSECONDS_PER_SECOND)
            {
                this.attoseconds -= ATTOSECONDS_PER_SECOND;
                this.seconds++;
            }

            // overflow
            if (this.seconds >= ATTOTIME_MAX_SECONDS)
                return this = never;

            return this;
        }
#endif


        //-------------------------------------------------
        //  operator- - handle subtraction between two
        //  attotimes
        //-------------------------------------------------
        public static attotime operator-(attotime left, attotime right)
        {
            attotime result = new attotime();

            // if time1 is never, return never
            if (left.m_seconds >= ATTOTIME_MAX_SECONDS)
                return attotime.never;

            // add the seconds and attoseconds
            result.m_attoseconds = left.m_attoseconds - right.m_attoseconds;
            result.m_seconds = left.m_seconds - right.m_seconds;

            // normalize and return
            if (result.m_attoseconds < 0)
            {
                result.m_attoseconds += ATTOSECONDS_PER_SECOND;
                result.m_seconds--;
            }
            return result;
        }

#if false
        public static attotime operator-=(attotime right)
        {
            // if time1 is never, return never
            if (this.seconds >= ATTOTIME_MAX_SECONDS)
                return this = never;

            // add the seconds and attoseconds
            attoseconds -= right.attoseconds;
            seconds -= right.seconds;

            // normalize and return
            if (this.attoseconds < 0)
            {
                this.attoseconds += ATTOSECONDS_PER_SECOND;
                this.seconds--;
            }
            return this;
        }
#endif

        //-------------------------------------------------
        //  operator* - handle multiplication/division by
        //  an integral factor; defined in terms of the
        //  assignment operators
        //-------------------------------------------------
        public static attotime operator*(attotime left, u32 factor)
        {
            attotime result = left;
            result *= factor;
            return result;
        }

        public static attotime operator*(UInt32 factor, attotime right)
        {
            attotime result = right;
            result *= factor;
            return result;
        }

        public static attotime operator/(attotime left, u32 factor)
        {
            attotime result = left;
            result /= factor;
            return result;
        }

        //-------------------------------------------------
        //  operator== - handle comparisons between
        //  attotimes
        //-------------------------------------------------
        public static bool operator==(attotime left, attotime right) { return left.m_seconds == right.m_seconds && left.m_attoseconds == right.m_attoseconds; }
        public static bool operator!=(attotime left, attotime right) { return left.m_seconds != right.m_seconds || left.m_attoseconds != right.m_attoseconds; }
        public static bool operator<(attotime left, attotime right) { return left.m_seconds < right.m_seconds || (left.m_seconds == right.m_seconds && left.m_attoseconds < right.m_attoseconds); }
        public static bool operator<=(attotime left, attotime right) { return left.m_seconds < right.m_seconds || (left.m_seconds == right.m_seconds && left.m_attoseconds <= right.m_attoseconds); }
        public static bool operator>(attotime left, attotime right) { return left.m_seconds > right.m_seconds || (left.m_seconds == right.m_seconds && left.m_attoseconds > right.m_attoseconds); }
        public static bool operator>=(attotime left, attotime right) { return left.m_seconds > right.m_seconds || (left.m_seconds == right.m_seconds && left.m_attoseconds >= right.m_attoseconds); }

        public override bool Equals(Object obj)
        {
            if (obj == null || base.GetType() != obj.GetType()) return false;
            return this == (attotime)obj;
        }
        public override int GetHashCode() { return m_seconds.GetHashCode() ^ m_attoseconds.GetHashCode(); }

        public static attotime Min(attotime left, attotime right) { return !(right < left) ? left : right; }
        public static attotime Max(attotime left, attotime right) { return (left < right) ? right : left; }
    }
}
