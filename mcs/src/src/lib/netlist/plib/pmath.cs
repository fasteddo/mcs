// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;


namespace mame.plib
{
    /// \brief Holds constants used repeatedly.
    ///
    ///  \tparam T floating point type
    ///
    ///  Using the structure members we can avoid magic numbers in the code.
    ///  In addition, this is a typesafe approach.
    ///
    //template <typename T>
    public class constants
    {
        public static nl_fptype zero() { return 0; }  //static inline constexpr T zero()   noexcept { return static_cast<T>(0); }
        public static nl_fptype half() { return 0.5; }  //static inline constexpr T half()   noexcept { return static_cast<T>(0.5); }
        public static nl_fptype one() { return 1; }  //static inline constexpr T one()    noexcept { return static_cast<T>(1); }
        public static nl_fptype two() { return 2; }  //static inline constexpr T two()    noexcept { return static_cast<T>(2); }
        //static inline constexpr T three()  noexcept { return static_cast<T>(3); }
        public static nl_fptype four() { return 4; }  //static inline constexpr T four()   noexcept { return static_cast<T>(4); }
        //static inline constexpr T sqrt2()  noexcept { return static_cast<T>(1.414213562373095048801688724209L); }
        public static nl_fptype pi() { return 3.14159265358979323846264338327950; }  //static inline constexpr T pi()     noexcept { return static_cast<T>(3.14159265358979323846264338327950L); }

        /// \brief Electric constant of vacuum
        ///
        //static inline constexpr T eps_0() noexcept { return static_cast<T>(8.854187817e-12); }

        // \brief Relative permittivity of Silicon dioxide
        ///
        //static inline constexpr T eps_SiO2() noexcept { return static_cast<T>(3.9); }

        /// \brief Relative permittivity of Silicon
        ///
        //static inline constexpr T eps_Si() noexcept { return static_cast<T>(11.7); }

        /// \brief Boltzmann constant
        ///
        public static nl_fptype k_b() { return 1.38064852e-23; }  //static inline constexpr T k_b() noexcept { return static_cast<T>(1.38064852e-23); }

        /// \brief room temperature (gives VT = 0.02585 at T=300)
        ///
        public static nl_fptype T0() { return 300; }  //static inline constexpr T T0() noexcept { return static_cast<T>(300); }

        /// \brief Elementary charge
        ///
        public static nl_fptype Q_e() { return 1.6021765314e-19; }  //static inline constexpr T Q_e() noexcept { return static_cast<T>(1.6021765314e-19); }

        /// \brief Intrinsic carrier concentration in 1/m^3 of Silicon
        ///
        //static inline constexpr T NiSi() noexcept { return static_cast<T>(1.45e16); }

        /// \brief clearly identify magic numbers in code
        ///
        /// Magic numbers should be avoided. The magic member at least clearly
        /// identifies them and makes it easier to convert them to named constants
        /// later.
        ///
        //template <typename V>
        public static nl_fptype magic(nl_fptype v) { return v; }  //static inline constexpr const T magic(V &&v) noexcept { return static_cast<T>(v); }
    }


    static class pmath_global
    {
        /// \brief typesafe reciprocal function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return reciprocal of argument
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static nl_fptype reciprocal(nl_fptype v)
        {
            return constants.one() / v;  //return constants<T>::one() / v;
        }

        /// \brief abs function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return absolute value of argument
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static nl_fptype abs(nl_fptype v)  //abs(T v) noexcept
        {
            return std.abs(v);
        }

        /// \brief sqrt function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return absolute value of argument
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static nl_fptype sqrt(nl_fptype v)  //sqrt(T v) noexcept
        {
            return std.sqrt(v);
        }

        /// \brief hypot function
        ///
        /// \tparam T type of the arguments
        /// \param  v1 first argument
        /// \param  v2 second argument
        /// \return sqrt(v1*v1+v2*v2)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        //hypot(T v1, T v2) noexcept
        //{
        //    return std::hypot(v1, v2);
        //}

        /// \brief exp function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return exp(v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static nl_fptype exp(nl_fptype v)  //exp(T v) noexcept
        {
            return std.exp(v);
        }

        /// \brief log function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return log(v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static nl_fptype log(nl_fptype v)  //log(T v) noexcept
        {
            return std.log(v);
        }

        /// \brief tanh function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return tanh(v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        //tanh(T v) noexcept
        //{
        //    return std::tanh(v);
        //}

        /// \brief floor function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return floor(v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static nl_fptype floor(nl_fptype v)  //floor(T v) noexcept
        {
            return std.floor(v);
        }

        /// \brief log1p function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return log(1 + v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        //log1p(T v) noexcept
        //{
        //    return std::log1p(v);
        //}

        /// \brief sin function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return sin(v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static nl_fptype sin(nl_fptype v)  //sin(T v) noexcept
        {
            return std.sin(v);
        }

        /// \brief cos function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return cos(v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static nl_fptype cos(nl_fptype v)  //cos(T v) noexcept
        {
            return std.cos(v);
        }

        /// \brief trunc function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return trunc(v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static nl_fptype trunc(nl_fptype v)  //trunc(T v) noexcept
        {
            return std.trunc(v);
        }

        /// \brief pow function
        ///
        /// \tparam T1 type of the first argument
        /// \tparam T2 type of the second argument
        /// \param  v argument
        /// \param  p power
        /// \return v^p
        ///
        /// FIXME: limited implementation
        ///
        //template <typename T1, typename T2>
        //static inline T1
        public static nl_fptype pow(nl_fptype v, nl_fptype p)  //pow(T1 v, T2 p) noexcept
        {
            return std.pow(v, p);
        }

#if (PUSE_FLOAT128)
        static inline constexpr __float128 reciprocal(__float128 v) noexcept
        {
            return constants<__float128>::one() / v;
        }

        static inline __float128 abs(__float128 v) noexcept
        {
            return fabsq(v);
        }

        static inline __float128 sqrt(__float128 v) noexcept
        {
            return sqrtq(v);
        }

        static inline __float128 hypot(__float128 v1, __float128 v2) noexcept
        {
            return hypotq(v1, v2);
        }

        static inline __float128 exp(__float128 v) noexcept
        {
            return expq(v);
        }

        static inline __float128 log(__float128 v) noexcept
        {
            return logq(v);
        }

        static inline __float128 tanh(__float128 v) noexcept
        {
            return tanhq(v);
        }

        static inline __float128 floor(__float128 v) noexcept
        {
            return floorq(v);
        }

        static inline __float128 log1p(__float128 v) noexcept
        {
            return log1pq(v);
        }

        static inline __float128 sin(__float128 v) noexcept
        {
            return sinq(v);
        }

        static inline __float128 cos(__float128 v) noexcept
        {
            return cosq(v);
        }

        static inline __float128 trunc(__float128 v) noexcept
        {
            return truncq(v);
        }

        template <typename T>
        static inline __float128 pow(__float128 v, T p) noexcept
        {
            return powq(v, static_cast<__float128>(p));
        }

        static inline __float128 pow(__float128 v, int p) noexcept
        {
            if (p==2)
                return v*v;
            else
                return powq(v, static_cast<__float128>(p));
        }
#endif

        //static_assert(noexcept(constants<double>::one()) == true, "Not evaluated as constexpr");
    }
}
