// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int64_t = System.Int64;
using nl_fptype = System.Double;


namespace mame.plib
{
    //template <typename TYPE, TYPE RES>
    public class ptime
    {
        //using internal_type = TYPE;
        //using mult_type = std::uint64_t;


        //internal_type m_time;  // moved to derived type


        public ptime() { }  //constexpr ptime() noexcept : m_time(0) {}


        //constexpr ptime(const ptime &rhs) noexcept = default;
        //constexpr ptime(ptime &&rhs) noexcept = default;
        //C14CONSTEXPR ptime &operator=(const ptime &rhs) noexcept = default;
        //C14CONSTEXPR ptime &operator=(ptime &&rhs) noexcept  = default;

        //constexpr explicit ptime(const double t) = delete;
        //: m_time((internal_type) ( t * (double) resolution)) { }
        //constexpr explicit ptime(const internal_type &nom, const internal_type &den) noexcept
        //: m_time(nom * (RES / den)) { }

        //constexpr explicit ptime(const internal_type &time) : m_time(time) {}
        //constexpr explicit ptime(internal_type &&time) : m_time(time) {}


        //C14CONSTEXPR ptime &operator+=(const ptime &rhs) noexcept { m_time += rhs.m_time; return *this; }
        //C14CONSTEXPR ptime &operator-=(const ptime &rhs) noexcept { m_time -= rhs.m_time; return *this; }
        //C14CONSTEXPR ptime &operator*=(const mult_type &factor) noexcept { m_time *= static_cast<internal_type>(factor); return *this; }

        //friend C14CONSTEXPR ptime operator-(ptime lhs, const ptime &rhs) noexcept
        //{
        //    return lhs -= rhs;
        //}

        //friend C14CONSTEXPR ptime operator+(ptime lhs, const ptime &rhs) noexcept
        //{
        //    return lhs += rhs;
        //}

        //friend C14CONSTEXPR ptime operator*(ptime lhs, const mult_type &factor) noexcept
        //{
        //    return lhs *= factor;
        //}

        //friend constexpr mult_type operator/(const ptime &lhs, const ptime &rhs) noexcept
        //{
        //    return static_cast<mult_type>(lhs.m_time / rhs.m_time);
        //}

        //friend constexpr bool operator<(const ptime &lhs, const ptime &rhs) noexcept
        //{
        //    return (lhs.m_time < rhs.m_time);
        //}

        //friend constexpr bool operator>(const ptime &lhs, const ptime &rhs) noexcept
        //{
        //    return (rhs < lhs);
        //}

        //friend constexpr bool operator<=(const ptime &lhs, const ptime &rhs) noexcept
        //{
        //    return !(lhs > rhs);
        //}

        //friend constexpr bool operator>=(const ptime &lhs, const ptime &rhs) noexcept
        //{
        //    return !(lhs < rhs);
        //}

        //friend constexpr bool operator==(const ptime &lhs, const ptime &rhs) noexcept
        //{
        //    return lhs.m_time == rhs.m_time;
        //}

        //friend constexpr bool operator!=(const ptime &lhs, const ptime &rhs) noexcept
        //{
        //    return !(lhs == rhs);
        //}

        //constexpr internal_type as_raw() const noexcept { return m_time; }

        //template <typename FT, typename = std::enable_if<std::is_floating_point<FT>::value, FT>>
        //constexpr FT
        //as_fp() const noexcept
        //{
        //    return static_cast<FT>(m_time) * inv_res<FT>();
        //}

#if PUSE_FLOAT128
        //constexpr __float128
        //as_fp() const noexcept
        //{
        //    return static_cast<__float128>(m_time) * inv_res<__float128>();
        //}
#endif

        //constexpr double as_double() const noexcept { return as_fp<double>(); }
        //constexpr double as_float() const noexcept { return as_fp<float>(); }
        //constexpr double as_long_double() const noexcept { return as_fp<long double>(); }

        // for save states ....
        //C14CONSTEXPR internal_type *get_internaltype_ptr() noexcept { return &m_time; }

        //static constexpr ptime from_nsec(const internal_type &ns) noexcept { return ptime(ns, UINT64_C(1000000000)); }
        //static constexpr ptime from_usec(const internal_type &us) noexcept { return ptime(us, UINT64_C(1000000)); }
        //static constexpr ptime from_msec(const internal_type &ms) noexcept { return ptime(ms, UINT64_C(1000)); }
        //static constexpr ptime from_hz(const internal_type &hz) noexcept { return ptime(1 , hz); }
        //static constexpr ptime from_raw(const internal_type &raw) noexcept { return ptime(raw); }

        //template <typename FT>
        //static constexpr const typename std::enable_if<std::is_floating_point<FT>::value
#if PUSE_FLOAT128
        //    || std::is_same<FT, __float128>::value
#endif
        //, ptime>::type
        //from_fp(const FT t) noexcept { return ptime(static_cast<internal_type>(plib::floor(t * static_cast<FT>(RES) + static_cast<FT>(0.5))), RES); }

        //static constexpr const ptime from_double(const double t) noexcept
        //{ return from_fp<double>(t); }

        //static constexpr const ptime from_float(const float t) noexcept
        //{ return from_fp<float>(t); }

        //static constexpr const ptime from_long_double(const long double t) noexcept
        //{ return from_fp<long double>(t); }

        //static constexpr ptime zero() noexcept { return ptime(0, RES); }
        //static constexpr ptime quantum() noexcept { return ptime(1, RES); }
        //static constexpr ptime never() noexcept { return ptime(plib::numeric_limits<internal_type>::max(), RES); }
        //static uint64_t resolution() { return RES; }  //static constexpr const internal_type resolution() noexcept { return RES; }

        //constexpr internal_type in_nsec() const noexcept { return m_time / (RES / UINT64_C(1000000000)); }
        //constexpr internal_type in_usec() const noexcept { return m_time / (RES / UINT64_C(   1000000)); }
        //constexpr internal_type in_msec() const noexcept { return m_time / (RES / UINT64_C(      1000)); }
        //constexpr internal_type in_sec()  const noexcept { return m_time / (RES / UINT64_C(         1)); }

        //template <typename FT>
        //static constexpr FT inv_res() noexcept { return static_cast<FT>(1.0) / static_cast<FT>(RES); }
    }


    public class ptime_i64 : ptime
    {
        const int64_t RES = netlist.nl_config_global.NETLIST_INTERNAL_RES;

        int64_t m_time;  //internal_type m_time;


        public ptime_i64() : this(0) { }  //constexpr ptime() noexcept : m_time(0) {}
        protected ptime_i64(int64_t nom, int64_t den) : this(nom * (RES / den)) { }
        protected ptime_i64(int64_t time) : base() { m_time = time; }  //constexpr explicit ptime(const internal_type &time) : m_time(time) {}


        public static ptime_i64 operator+(ptime_i64 lhs, ptime_i64 rhs) { return new ptime_i64(lhs.m_time + rhs.m_time); }
        public static ptime_i64 operator-(ptime_i64 lhs, ptime_i64 rhs) { return new ptime_i64(lhs.m_time - rhs.m_time); }

        public static ptime_i64 operator*(ptime_i64 lhs, Int64 rhs) { return new ptime_i64(lhs.m_time * rhs); }
        public static ptime_i64 operator*(ptime_i64 lhs, int rhs) { return new ptime_i64(lhs.m_time * (Int64)rhs); }
        public static ptime_i64 operator/(ptime_i64 lhs, ptime_i64 rhs) { return new ptime_i64(lhs.m_time / rhs.m_time); }

        public static bool operator <(ptime_i64 lhs, ptime_i64 rhs) { return lhs.m_time < rhs.m_time; }
        public static bool operator <=(ptime_i64 lhs, ptime_i64 rhs) { return lhs.m_time <= rhs.m_time; }
        public static bool operator >(ptime_i64 lhs, ptime_i64 rhs) { return rhs < lhs; }
        public static bool operator >=(ptime_i64 lhs, ptime_i64 rhs) { return rhs <= lhs; }
        public static bool operator ==(ptime_i64 lhs, ptime_i64 rhs) { return lhs.m_time == rhs.m_time; }
        public static bool operator !=(ptime_i64 lhs, ptime_i64 rhs) { return !(lhs == rhs); }


        public static ptime_i64 Max(ptime_i64 lhs, ptime_i64 rhs) { return lhs > rhs ? lhs : rhs; }


        public int64_t as_raw() { return m_time; }

        //template <typename FT, typename = std::enable_if<std::is_floating_point<FT>::value, FT>>
        //constexpr FT
        public nl_fptype as_fp()
        {
            return m_time * inv_res();  //return static_cast<FT>(m_time) * inv_res<FT>();
        }

#if PUSE_FLOAT128
        //constexpr __float128
        //as_fp() const noexcept
        //{
        //    return static_cast<__float128>(m_time) * inv_res<__float128>();
        //}
#endif

        public double as_double() { return as_fp(); }
        //constexpr double as_float() const noexcept { return as_fp<float>(); }
        //constexpr double as_long_double() const noexcept { return as_fp<long double>(); }


        public static ptime_i64 from_nsec(int64_t ns) { return new ptime_i64(ns, 1000000000); }
        public static ptime_i64 from_usec(int64_t us) { return new ptime_i64(us, 1000000); }
        public static ptime_i64 from_msec(int64_t ms) { return new ptime_i64(ms, 1000); }
        public static ptime_i64 from_hz(int64_t hz) { return new ptime_i64(1 , hz); }
        public static ptime_i64 from_raw(int64_t raw) { return new ptime_i64(raw); }

        //template <typename FT>
        //static constexpr const typename std::enable_if<std::is_floating_point<FT>::value
#if PUSE_FLOAT128
        //    || std::is_same<FT, __float128>::value
#endif
        //, ptime>::type
        //from_fp(const FT t) noexcept { return ptime(static_cast<internal_type>(plib::floor(t * static_cast<FT>(RES) + static_cast<FT>(0.5))), RES); }
        public static ptime_i64 from_fp(double t) { return new ptime_i64((int64_t)(t * (double)RES + 0.5), RES); }

        //static constexpr const ptime from_double(const double t) noexcept
        //{ return from_fp<double>(t); }

        //static constexpr const ptime from_float(const float t) noexcept
        //{ return from_fp<float>(t); }

        //static constexpr const ptime from_long_double(const long double t) noexcept
        //{ return from_fp<long double>(t); }

        public static ptime_i64 zero() { return new ptime_i64(0); }
        public static ptime_i64 quantum() { return new ptime_i64(1, RES); }
        public static ptime_i64 never() { return new ptime_i64(int64_t.MaxValue, RES); }  //{ return ptime(plib::numeric_limits<internal_type>::max(), RES); }
        static int64_t resolution() { return RES; }  //static constexpr const internal_type resolution() noexcept { return RES; }


        //constexpr internal_type in_nsec() const noexcept { return m_time / (RES / UINT64_C(1000000000)); }
        //constexpr internal_type in_usec() const noexcept { return m_time / (RES / UINT64_C(   1000000)); }
        //constexpr internal_type in_msec() const noexcept { return m_time / (RES / UINT64_C(      1000)); }
        //constexpr internal_type in_sec()  const noexcept { return m_time / (RES / UINT64_C(         1)); }

        //template <typename FT>
        static double inv_res() { return 1.0 / RES; }  //static constexpr FT inv_res() noexcept { return static_cast<FT>(1.0) / static_cast<FT>(RES); }
    }
}
