// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using s32 = System.Int32;
using s64 = System.Int64;
using seconds_t = System.Int32;  //typedef s32 seconds_t;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // core components of the attotime structure
    //typedef s64 attoseconds_t;
    //typedef s32 seconds_t;


    public struct attotime
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
        public static attoseconds_t HZ_TO_ATTOSECONDS(double x) { return (attoseconds_t)((double)ATTOSECONDS_PER_SECOND / x); }

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
        public attotime(seconds_t secs, attoseconds_t attos) { m_seconds = secs; m_attoseconds = attos; }
        public attotime(attotime that) : this(that.m_seconds, that.m_attoseconds) { }


        // queries
        public bool is_zero() { return m_seconds == 0 && m_attoseconds == 0; }
        /** Test if value is above @ref ATTOTIME_MAX_SECONDS (considered an overflow) */
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

        public double as_hz() { g.assert(!is_zero()); return m_seconds == 0 ? ATTOSECONDS_TO_HZ(m_attoseconds) : is_never() ? 0.0 : 1.0 / as_double(); }

        //double as_khz() const noexcept { assert(!is_zero()); return m_seconds == 0 ? double(ATTOSECONDS_PER_MILLISECOND) / double(m_attoseconds) : is_never() ? 0.0 : 1e-3 / as_double(); }
        //double as_mhz() const noexcept { assert(!is_zero()); return m_seconds == 0 ? double(ATTOSECONDS_PER_MICROSECOND) / double(m_attoseconds) : is_never() ? 0.0 : 1e-6 / as_double(); }


        //-------------------------------------------------
        //  as_ticks - convert to ticks at the given
        //  frequency
        //-------------------------------------------------
        u64 as_ticks(u32 frequency)
        {
            u32 fracticks = (u32)((new attotime(0, m_attoseconds) * frequency).m_seconds);
            return g.mulu_32x32((u32)m_seconds, frequency) + fracticks;
        }


        u64 as_ticks(XTAL xtal) { return as_ticks(xtal.value()); }


        /** Convert to string using at @p precision */
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
                buffer = "(never)";  //sprintf(buffer, "%-*s", precision, "(never)");

            // case 1: we want no precision; seconds only
            else if (precision == 0)
                buffer = string.Format("%d", m_seconds);  //sprintf(buffer, "%d", m_seconds);

            // case 2: we want 9 or fewer digits of precision
            else if (precision <= 9)
            {
                s64 upper = m_attoseconds / ATTOSECONDS_PER_SECOND_SQRT;
                int temp = precision;
                while (temp < 9)
                {
                    upper /= 10;
                    temp++;
                }
                buffer = string.Format("{0}.{1}", m_seconds, upper.ToString(new string('0', precision)));  //sprintf(buffer, "%d.%0*d", m_seconds, precision, upper);
            }

            // case 3: more than 9 digits of precision
            else
            {
                u32 lower;
                u32 upper = g.divu_64x32_rem((u64)m_attoseconds, (u32)ATTOSECONDS_PER_SECOND_SQRT, out lower);
                int temp = precision;
                while (temp < 18)
                {
                    lower /= 10;
                    temp++;
                }
                buffer = string.Format("{0}.{1}{2}", m_seconds, upper, lower);  //sprintf(buffer, "%d.%09d%0*d", m_seconds, upper, precision - 9, lower);
            }

            return buffer;
        }


        /** Convert to string for human readability in logs */
        //std::string to_string() const;


        /** @return the attoseconds portion. */
        public attoseconds_t attoseconds() { return m_attoseconds; }
        /** @return the seconds portion. */
        public seconds_t seconds() { return m_seconds; }


        //static attotime from_double(double _time);


        /** Create an attotime from a tick count @p ticks at the given frequency @p frequency  */
        public static attotime from_ticks(u64 ticks, u32 frequency)
        {
            if (frequency > 0)
            {
                attoseconds_t attos_per_tick = HZ_TO_ATTOSECONDS(frequency);

                if (ticks < frequency)
                    return new attotime(0, (attoseconds_t)ticks * attos_per_tick);  //return attotime(0, ticks * attos_per_tick);

                u32 remainder;
                s32 secs = (s32)g.divu_64x32_rem(ticks, frequency, out remainder);
                return new attotime(secs, (attoseconds_t)remainder * attos_per_tick);  //return attotime(secs, u64(remainder) * attos_per_tick);
            }
            else
            {
                return attotime.never;
            }
        }

        public static attotime from_ticks(u64 ticks, XTAL xtal) { return from_ticks(ticks, xtal.value()); }


        /** Create an attotime from an integer count of seconds @p seconds */
        public static attotime from_seconds(s32 seconds) { return new attotime(seconds, 0); }
        /** Create an attotime from an integer count of milliseconds @p msec */
        public static attotime from_msec(s64 msec) { return new attotime((int)(msec / 1000), (msec % 1000) * (ATTOSECONDS_PER_SECOND / 1000)); }
        /** Create an attotime from an integer count of microseconds @p usec */
        public static attotime from_usec(s64 usec) { return new attotime((int)(usec / 1000000), (usec % 1000000) * (ATTOSECONDS_PER_SECOND / 1000000)); }
        /** Create an attotime from an integer count of nanoseconds @p nsec */
        public static attotime from_nsec(s64 nsec) { return new attotime((int)(nsec / 1000000000), (nsec % 1000000000) * (ATTOSECONDS_PER_SECOND / 1000000000)); }

        /** Create an attotime from at the given frequency @p frequency */
        public static attotime from_hz(u32 frequency) { return (frequency > 1) ? new attotime(0, HZ_TO_ATTOSECONDS(frequency)) : (frequency == 1) ? new attotime(1, 0) : never; }
        public static attotime from_hz(int frequency) { return (frequency > 0) ? from_hz((u32)frequency) : never; }
        public static attotime from_hz(XTAL xtal) { return (xtal.dvalue() > 1.0) ? new attotime(0, HZ_TO_ATTOSECONDS(xtal)) : from_hz(xtal.dvalue()); }
        public static attotime from_hz(double frequency)
        {
            if (frequency > 1.0)
            {
                return new attotime(0, HZ_TO_ATTOSECONDS(frequency));
            }
            else if (frequency > 0.0)
            {
                //f = modf(1.0 / frequency, &i);
                double d = 1.0 / frequency;
                double i = (int)d;
                double f = d % 1;

                return new attotime((seconds_t)i, (attoseconds_t)(f * ATTOSECONDS_PER_SECOND));
            }
            else
            {
                return attotime.never;
            }
        }


        //-------------------------------------------------
        //  operator+ - handle addition between two
        //  attotimes
        //-------------------------------------------------
        public static attotime operator+(attotime left, attotime right)  //public static attotime operator+=(attotime right)
        {
            // if one of the items is never, return never
            if (left.m_seconds >= ATTOTIME_MAX_SECONDS || right.m_seconds >= ATTOTIME_MAX_SECONDS)
                return attotime.never;  //return this = never;

            // add the seconds and attoseconds
            attoseconds_t attoseconds = left.m_attoseconds + right.m_attoseconds;  //attoseconds += right.attoseconds;
            seconds_t seconds = left.m_seconds + right.m_seconds;  //seconds += right.seconds;
            attotime result = new attotime(seconds, attoseconds);

            // normalize and return
            if (result.m_attoseconds >= ATTOSECONDS_PER_SECOND)
            {
                result.m_attoseconds -= ATTOSECONDS_PER_SECOND;
                result.m_seconds++;
            }

            // overflow
            if (result.m_seconds >= ATTOTIME_MAX_SECONDS)
                return attotime.never;  //return this = never;

            return result;  //return this;
        }


        //-------------------------------------------------
        //  operator- - handle subtraction between two
        //  attotimes
        //-------------------------------------------------
        public static attotime operator-(attotime left, attotime right)  //public static attotime operator-=(attotime right)
        {
            // if time1 is never, return never
            if (left.m_seconds >= ATTOTIME_MAX_SECONDS)
                return attotime.never;  //return this = never;

            // add the seconds and attoseconds
            attoseconds_t attoseconds = left.m_attoseconds - right.m_attoseconds;  //attoseconds -= right.attoseconds;
            seconds_t seconds = left.m_seconds - right.m_seconds;  //seconds -= right.seconds;
            attotime result = new attotime(seconds, attoseconds);

            // normalize and return
            if (result.m_attoseconds < 0)
            {
                result.m_attoseconds += ATTOSECONDS_PER_SECOND;
                result.m_seconds--;
            }
            return result;  //return this;
        }


        /** handle multiplication by an integral factor; defined in terms of the assignment operators */
        //-------------------------------------------------
        //  operator*= - multiply an attotime by a
        //  constant
        //-------------------------------------------------
        public static attotime operator*(attotime left, u32 factor)  //attotime &attotime::operator*=(u32 factor)
        {
            // if one of the items is attotime::never, return attotime::never
            if (left.m_seconds >= ATTOTIME_MAX_SECONDS)
                return never;  //return *this = never;

            // 0 times anything is zero
            if (factor == 0)
                return zero;  //return *this = zero;

            // split attoseconds into upper and lower halves which fit into 32 bits
            u32 attolo;
            u32 attohi = g.divu_64x32_rem((u64)left.m_attoseconds, (u32)ATTOSECONDS_PER_SECOND_SQRT, out attolo);

            // scale the lower half, then split into high/low parts
            u64 temp = g.mulu_32x32(attolo, factor);
            u32 reslo;
            temp = g.divu_64x32_rem(temp, (u32)ATTOSECONDS_PER_SECOND_SQRT, out reslo);

            // scale the upper half, then split into high/low parts
            temp += g.mulu_32x32(attohi, factor);
            u32 reshi;
            temp = g.divu_64x32_rem(temp, (u32)ATTOSECONDS_PER_SECOND_SQRT, out reshi);

            // scale the seconds
            temp += g.mulu_32x32((u32)left.m_seconds, factor);
            if (temp >= ATTOTIME_MAX_SECONDS)
                return never;  //return *this = never;

            // build the result
            seconds_t seconds = (seconds_t)temp;  //m_seconds = temp;
            attoseconds_t attoseconds = (attoseconds_t)reslo + g.mul_32x32((s32)reshi, (s32)ATTOSECONDS_PER_SECOND_SQRT);  //m_attoseconds = (attoseconds_t)reslo + mul_32x32(reshi, ATTOSECONDS_PER_SECOND_SQRT);
            return new attotime(seconds, attoseconds);  //return *this;
        }


        public static attotime operator*(u32 factor, attotime right) { return right * factor; }


        //-------------------------------------------------
        //  operator/= - divide an attotime by a constant
        //-------------------------------------------------
        public static attotime operator/(attotime left, u32 factor)  //attotime &attotime::operator/=(u32 factor)
        {
            // if one of the items is attotime::never, return attotime::never
            if (left.m_seconds >= ATTOTIME_MAX_SECONDS)
                return never;  //return *this = never;

            // ignore divide by zero
            if (factor == 0)
                return left;  //return *this;

            // split attoseconds into upper and lower halves which fit into 32 bits
            u32 attolo;
            u32 attohi = g.divu_64x32_rem((u64)left.m_attoseconds, (u32)ATTOSECONDS_PER_SECOND_SQRT, out attolo);

            // divide the seconds and get the remainder
            u32 remainder;
            seconds_t seconds = (seconds_t)g.divu_64x32_rem((u64)left.m_seconds, factor, out remainder);  //m_seconds = divu_64x32_rem(m_seconds, factor, &remainder);

            // combine the upper half of attoseconds with the remainder and divide that
            u64 temp = (u64)attohi + g.mulu_32x32(remainder, (u32)ATTOSECONDS_PER_SECOND_SQRT);  //u64 temp = s64(attohi) + mulu_32x32(remainder, ATTOSECONDS_PER_SECOND_SQRT);
            u32 reshi = g.divu_64x32_rem(temp, factor, out remainder);

            // combine the lower half of attoseconds with the remainder and divide that
            temp = attolo + g.mulu_32x32(remainder, (u32)ATTOSECONDS_PER_SECOND_SQRT);
            u32 reslo = g.divu_64x32_rem(temp, factor, out remainder);

            // round based on the remainder
            attoseconds_t attoseconds = (attoseconds_t)reslo + (attoseconds_t)g.mulu_32x32(reshi, (u32)ATTOSECONDS_PER_SECOND_SQRT);  //m_attoseconds = (attoseconds_t)reslo + mulu_32x32(reshi, ATTOSECONDS_PER_SECOND_SQRT);
            if (remainder >= factor / 2)
            {
                if (++attoseconds >= ATTOSECONDS_PER_SECOND)  //if (++m_attoseconds >= ATTOSECONDS_PER_SECOND)
                {
                    attoseconds = 0;
                    seconds++;
                }
            }

            return new attotime(seconds, attoseconds);  //return *this;
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
