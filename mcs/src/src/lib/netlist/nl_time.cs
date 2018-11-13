// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;
using uint64_t = System.UInt64;


namespace mame.netlist
{
    //============================================================
    //  MACROS
    //============================================================

    //#define NLTIME_FROM_NS(t)  netlist_time::from_nsec(t)
    //#define NLTIME_FROM_US(t)  netlist_time::from_usec(t)
    //#define NLTIME_FROM_MS(t)  netlist_time::from_msec(t)
    //#define NLTIME_IMMEDIATE   netlist_time::from_nsec(1)


    // ----------------------------------------------------------------------------------------
    // netlist_time
    // ----------------------------------------------------------------------------------------

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
        //constexpr double as_double() const noexcept
        //{
        //    return static_cast<double>(m_time) / static_cast<double>(RES);
        //}

        // for save states ....
        //C14CONSTEXPR internal_type *get_internaltype_ptr() noexcept { return &m_time; }

        //static constexpr ptime from_nsec(const internal_type &ns) noexcept { return ptime(ns, UINT64_C(1000000000)); }
        //static constexpr ptime from_usec(const internal_type &us) noexcept { return ptime(us, UINT64_C(1000000)); }
        //static constexpr ptime from_msec(const internal_type &ms) noexcept { return ptime(ms, UINT64_C(1000)); }
        //static constexpr ptime from_hz(const internal_type &hz) noexcept { return ptime(1 , hz); }
        //static constexpr ptime from_raw(const internal_type &raw) noexcept { return ptime(raw); }
        //static constexpr ptime from_double(const double t) noexcept { return ptime(static_cast<internal_type>( t * static_cast<double>(RES)), RES); }

        //static constexpr ptime zero() noexcept { return ptime(0, RES); }
        //static constexpr ptime quantum() noexcept { return ptime(1, RES); }
        //static constexpr ptime never() noexcept { return ptime(plib::numeric_limits<internal_type>::max(), RES); }
        //static constexpr internal_type resolution() noexcept { return RES; }
    }


    public class ptime_u64 : ptime
    {
        const uint64_t RES = nl_config_global.NETLIST_INTERNAL_RES;

        uint64_t m_time;  //internal_type m_time;

        public ptime_u64() : this(0) { }  //constexpr ptime() noexcept : m_time(0) {}
        protected ptime_u64(uint64_t nom, uint64_t den) : this(nom * (RES / den)) { }
        protected ptime_u64(uint64_t time) : base() { m_time = time; }  //constexpr explicit ptime(const internal_type &time) : m_time(time) {}


        public static bool operator <(ptime_u64 lhs, ptime_u64 rhs) { return lhs.m_time < rhs.m_time; }
        public static bool operator >(ptime_u64 lhs, ptime_u64 rhs) { return rhs < lhs; }
        public static bool operator ==(ptime_u64 lhs, ptime_u64 rhs) { return lhs.m_time == rhs.m_time; }
        public static bool operator !=(ptime_u64 lhs, ptime_u64 rhs) { return !(lhs == rhs); }


        public double as_double() { return (double)m_time / (double)RES; }


        public static ptime_u64 from_nsec(uint64_t ns) { return new ptime_u64(ns, 1000000000); }
        public static ptime_u64 from_usec(uint64_t us) { return new ptime_u64(us, 1000000); }
        public static ptime_u64 from_msec(uint64_t ms) { return new ptime_u64(ms, 1000); }
        public static ptime_u64 from_hz(uint64_t hz) { return new ptime_u64(1 , hz); }
        public static ptime_u64 from_raw(uint64_t raw) { return new ptime_u64(raw); }
        public static ptime_u64 from_double(double t) { return new ptime_u64((uint64_t)(t * RES), RES); }

        public static ptime_u64 zero() { return new ptime_u64(0); }
        static ptime_u64 quantum() { return new ptime_u64(1, RES); }
        public static ptime_u64 never() { return new ptime_u64(uint64_t.MaxValue, RES); }  //{ return ptime(plib::numeric_limits<internal_type>::max(), RES); }
        static ptime_u64 resolution() { return new netlist_time(RES); }


        public static ptime_u64 NLTIME_FROM_NS(uint64_t t) { return netlist_time.from_nsec(t); }
        //#define NLTIME_FROM_US(t)  netlist_time::from_usec(t)
        //#define NLTIME_FROM_MS(t)  netlist_time::from_msec(t)
        //#define NLTIME_IMMEDIATE   netlist_time::from_nsec(1)
    }


//#if (PHAS_INT128)
//	using netlist_time = ptime<UINT128, NETLIST_INTERNAL_RES>;
//#else
//	using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;
//#endif
}


//namespace plib {
//template<> inline void state_manager_t::save_item(const void *owner, netlist::netlist_time &nlt, const pstring &stname)
//{
//	save_state_ptr(owner, stname, datatype_t(sizeof(netlist::netlist_time::internal_type), true, false), 1, nlt.get_internaltype_ptr());
//}
//}
