// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int64_t = System.Int64;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using unsigned = System.UInt32;


namespace mame.plib
{
    public interface ptime_RES<T>
    {
        T value { get; }
    }

    public class ptime_RES_config_INTERNAL_RES : plib.ptime_RES<int64_t>
    {
        public int64_t value { get { return netlist.config.INTERNAL_RES; } }
    }


    public interface ptime_operators<T>
    {
        int CompareTo(T lhs, T rhs);

        T add(T a, T b);
        T subtract(T a, T b);
        T multiply(T a, T b);
        T divide(T a, T b);

        T shift_right(T a, int shift);
        T shift_left(T a, int shift);

        bool equal(T a, T b);
        bool less_than(T a, T b);
        bool less_than_equal(T a, T b);

        T cast(Int64 a);
        T cast(double a, T RES);
        double cast_double(T a);
    }


    public class ptime_operators_int64 : ptime_operators<int64_t>
    {
        public int CompareTo(int64_t lhs, int64_t rhs) { return lhs.CompareTo(rhs); }

        public int64_t add(int64_t a, int64_t b) { return a + b; }
        public int64_t subtract(int64_t a, int64_t b) { return a - b; }
        public int64_t multiply(int64_t a, int64_t b) { return a * b; }
        public int64_t divide(int64_t a, int64_t b) { return a / b; }

        public int64_t shift_right(int64_t a, int shift) { return a >> shift; }
        public int64_t shift_left(int64_t a, int shift) { return a << shift; }

        public bool equal(int64_t a, int64_t b) { return a == b; }
        public bool less_than(int64_t a, int64_t b) { return a < b; }
        public bool less_than_equal(int64_t a, int64_t b) { return a <= b; }

        public int64_t cast(Int64 a) { return a; }
        public int64_t cast(double a, int64_t RES) { return (int64_t)pglobal.floor<double, constants_operators_double>(a * RES + 0.5); }
        public double cast_double(int64_t a) { return a; }
    }


    //template <typename TYPE, TYPE RES>
    public class ptime<TYPE, TYPE_OPS, TYPE_RES> : IComparable
        where TYPE_OPS : ptime_operators<TYPE>, new()
        where TYPE_RES : ptime_RES<TYPE>, new()
    {
        static ptime_operators<TYPE> ops = new TYPE_OPS();
        static ptime_RES<TYPE> RES = new TYPE_RES();


        //using internal_type = TYPE;
        //using mult_type = TYPE;

        //template <typename altTYPE, altTYPE altRES>
        //friend struct ptime;


        TYPE m_time;


        public ptime() { m_time = default; }


        //~ptime() noexcept = default;

        //constexpr ptime(const ptime &rhs) noexcept = default;
        //constexpr ptime(ptime &&rhs) noexcept = default;
        public ptime(TYPE time) { m_time = time; }
        //constexpr explicit ptime(internal_type &&time) noexcept : m_time(time) {}
        //constexpr ptime &operator=(const ptime &rhs) noexcept = default;
        //constexpr ptime &operator=(ptime &&rhs) noexcept  = default;

        //constexpr explicit ptime(const double t) = delete;

        ptime(TYPE nom, TYPE den)
        {
            m_time = ops.multiply(nom, ops.divide(RES.value, den));  //: m_time(nom * (RES / den)) { }
        }


        //template <typename O>
        //constexpr explicit ptime(const ptime<O, RES> &rhs) noexcept
        //: m_time(static_cast<TYPE>(rhs.m_time))
        //{
        //}


        // IComparable
        public int CompareTo(object rhs) { return (rhs is ptime<TYPE, TYPE_OPS, TYPE_RES>) ? CompareTo((ptime<TYPE, TYPE_OPS, TYPE_RES>)rhs) : -1; }
        public int CompareTo(ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return ops.CompareTo(m_time, rhs.m_time); }
        public override bool Equals(object rhs) { return (rhs is ptime<TYPE, TYPE_OPS, TYPE_RES>) ? ops.equal(m_time, ((ptime<TYPE, TYPE_OPS, TYPE_RES>)rhs).m_time) : false; }
        public override int GetHashCode() { return m_time.GetHashCode(); }


        //template <typename O>
        //constexpr ptime &operator+=(const ptime<O, RES> &rhs) noexcept
        //{
        //    static_assert(ptime_le<ptime<O, RES>, ptime>::value, "Invalid ptime type");
        //    m_time += rhs.m_time;
        //    return *this;
        //}
        //template <typename O>
        //constexpr ptime &operator-=(const ptime<O, RES> &rhs) noexcept
        //{
        //    static_assert(ptime_le<ptime<O, RES>, ptime>::value, "Invalid ptime type");
        //    m_time -= rhs.m_time;
        //    return *this;
        //}

        //template <typename M>
        //constexpr ptime &operator*=(const M &factor) noexcept
        //{
        //    static_assert(plib::is_integral<M>::value, "Factor must be an integral type");
        //    m_time *= factor;
        //    return *this;
        //}

        //template <typename O>
        //constexpr ptime operator-(const ptime<O, RES> &rhs) const noexcept
        //{
        //    static_assert(ptime_le<ptime<O, RES>, ptime>::value, "Invalid ptime type");
        //    return ptime(m_time - rhs.m_time);
        //}
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> operator-(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.subtract(lhs.m_time, rhs.m_time)); }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> operator-(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, Int64 rhs) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.subtract(lhs.m_time, ops.cast(rhs))); }

        //template <typename O>
        //constexpr ptime operator+(const ptime<O, RES> &rhs) const noexcept
        //{
        //    static_assert(ptime_le<ptime<O, RES>, ptime>::value, "Invalid ptime type");
        //    return ptime(m_time + rhs.m_time);
        //}
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> operator+(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.add(lhs.m_time, rhs.m_time)); }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> operator+(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, Int64 rhs) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.add(lhs.m_time, ops.cast(rhs))); }

        //template <typename M>
        //constexpr ptime operator*(const M &factor) const noexcept
        //{
        //    static_assert(plib::is_integral<M>::value, "Factor must be an integral type");
        //    return ptime(m_time * static_cast<mult_type>(factor));
        //}
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> operator*(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.multiply(lhs.m_time, rhs.m_time)); }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> operator*(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, Int64 rhs) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.multiply(lhs.m_time, ops.cast(rhs))); }

        //template <typename O>
        //constexpr mult_type operator/(const ptime<O, RES> &rhs) const noexcept
        //{
        //    static_assert(ptime_le<ptime<O, RES>, ptime>::value, "Invalid ptime type");
        //    return static_cast<mult_type>(m_time / rhs.m_time);
        //}
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> operator/(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.divide(lhs.m_time, rhs.m_time)); }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> operator/(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, Int64 rhs) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.divide(lhs.m_time, ops.cast(rhs))); }

        //template <typename T>
        //constexpr std::enable_if_t<std::is_integral<T>::value, ptime>
        //operator/(const T &rhs) const noexcept
        //{
        //    return ptime(m_time / rhs);
        //}

        //template <typename O>
        //friend constexpr bool operator<(const ptime &lhs, const ptime<O, RES> &rhs) noexcept
        //{
        //    static_assert(ptime_le<ptime<O, RES>, ptime>::value, "Invalid ptime type");
        //    return (lhs.m_time < rhs.as_raw());
        //    //return (lhs.m_time < rhs.m_time);
        //}

        //template <typename O>
        //friend constexpr bool operator>(const ptime &lhs, const ptime<O, RES> &rhs) noexcept
        //{
        //    return (rhs < lhs);
        //}

        //template <typename O>
        //friend constexpr bool operator<=(const ptime &lhs, const ptime<O, RES> &rhs) noexcept
        //{
        //    return !(lhs > rhs);
        //}

        //template <typename O>
        //friend constexpr bool operator>=(const ptime &lhs, const ptime<O, RES> &rhs) noexcept
        //{
        //    return !(lhs < rhs);
        //}

        //template <typename O>
        //friend constexpr bool operator==(const ptime &lhs, const ptime<O, RES> &rhs) noexcept
        //{
        //    return lhs.m_time == rhs.m_time;
        //}

        //template <typename O>
        //friend constexpr bool operator!=(const ptime &lhs, const ptime<O, RES> &rhs) noexcept
        //{
        //    return !(lhs == rhs);
        //}
        public static bool operator <(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return ops.less_than(lhs.m_time, rhs.m_time); }
        public static bool operator <=(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return ops.less_than_equal(lhs.m_time, rhs.m_time); }
        public static bool operator >(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return rhs < lhs; }
        public static bool operator >=(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return rhs <= lhs; }
        public static bool operator ==(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return ops.equal(lhs.m_time, rhs.m_time); }
        public static bool operator !=(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return !(lhs == rhs); }


        public TYPE as_raw() { return m_time; }

        //template <typename FT, typename = std::enable_if<plib::is_floating_point<FT>::value, FT>>
        //constexpr FT
        public TYPE as_fp()
        {
            return ops.multiply(m_time, inv_res());  //return static_cast<FT>(m_time) * inv_res<FT>();
        }


        public double as_double() { return ops.cast_double(as_fp()); }  //constexpr double as_double() const noexcept { return as_fp<double>(); }
        //constexpr double as_float() const noexcept { return as_fp<float>(); }
        //constexpr double as_long_double() const noexcept { return as_fp<long double>(); }


        //constexpr ptime shl(unsigned shift) const noexcept { return ptime(m_time << shift); }
        public ptime<TYPE, TYPE_OPS, TYPE_RES> shr(unsigned shift) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.shift_right(m_time, (int)shift)); }  //constexpr ptime shr(unsigned shift) const noexcept { return ptime(m_time >> shift); }

        // for save states ....
#if false
        constexpr internal_type *get_internaltype_ptr() noexcept { return &m_time; }
#endif

        //template <typename ST>
        public void save_state(save_helper st)  //void save_state(ST &&st)
        {
            st.save_item(m_time, "m_time");
        }

        public static ptime<TYPE, TYPE_OPS, TYPE_RES> from_nsec(TYPE ns) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ns, ops.cast(1000000000)); }  //static constexpr ptime from_nsec(internal_type ns) noexcept { return ptime(ns, UINT64_C(1000000000)); }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> from_usec(TYPE us) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(us, ops.cast(1000000)); }  //static constexpr ptime from_usec(internal_type us) noexcept { return ptime(us, UINT64_C(   1000000)); }
        //static constexpr ptime from_msec(internal_type ms) noexcept { return ptime(ms, UINT64_C(      1000)); }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> from_sec(TYPE s)   { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(s,  ops.cast(1)); }  //static constexpr ptime from_sec(internal_type s) noexcept   { return ptime(s,  UINT64_C(         1)); }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> from_hz(TYPE hz) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.cast(1), hz); }  //static constexpr ptime from_hz(internal_type hz) noexcept { return ptime(1 , hz); }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> from_raw(TYPE raw) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(raw); }  //static constexpr ptime from_raw(internal_type raw) noexcept { return ptime(raw); }

        //template <typename FT>
        //static constexpr std::enable_if_t<plib::is_floating_point<FT>::value, ptime>
        //from_fp(FT t) noexcept { return ptime(static_cast<internal_type>(plib::floor(t * static_cast<FT>(RES) + static_cast<FT>(0.5))), RES); }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> from_fp(double t) { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.cast(t, RES.value), RES.value); }

        //static constexpr ptime from_double(double t) noexcept
        //{ return from_fp<double>(t); }

        //static constexpr ptime from_float(float t) noexcept
        //{ return from_fp<float>(t); }

        //static constexpr ptime from_long_double(long double t) noexcept
        //{ return from_fp<long double>(t); }

        public static ptime<TYPE, TYPE_OPS, TYPE_RES> zero() { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.cast(0), RES.value); }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> quantum() { return new ptime<TYPE, TYPE_OPS, TYPE_RES>(ops.cast(0), RES.value); }
        //static constexpr ptime never() noexcept { return ptime(plib::numeric_limits<internal_type>::max(), RES); }
        public static TYPE resolution() { return RES.value; }

        //constexpr internal_type in_nsec() const noexcept { return m_time / (RES / INT64_C(1000000000)); }
        //constexpr internal_type in_usec() const noexcept { return m_time / (RES / INT64_C(   1000000)); }
        //constexpr internal_type in_msec() const noexcept { return m_time / (RES / INT64_C(      1000)); }
        //constexpr internal_type in_sec()  const noexcept { return m_time / (RES / INT64_C(         1)); }

        //template <typename FT>
        static TYPE inv_res() { return ops.divide(ops.cast(1), RES.value); }  //static constexpr FT inv_res() noexcept { return static_cast<FT>(1.0) / static_cast<FT>(RES); }


        public static ptime<TYPE, TYPE_OPS, TYPE_RES> Max(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return ops.less_than(lhs.m_time, rhs.m_time) ? rhs : lhs; }
        public static ptime<TYPE, TYPE_OPS, TYPE_RES> Min(ptime<TYPE, TYPE_OPS, TYPE_RES> lhs, ptime<TYPE, TYPE_OPS, TYPE_RES> rhs) { return ops.less_than(lhs.m_time, rhs.m_time) ? lhs : rhs; }
    }
}
