// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;


namespace mame.plib
{
    public interface constants_operators<T>
    {
        T default_ { get; }

        T add(T a, T b);
        T subtract(T a, T b);
        T multiply(T a, T b);
        T divide(T a, T b);

        T neg(T a);

        T abs(T a);
        T cos(T a);
        T exp(T a);
        T floor(T a);
        T log(T a);
        T pow(T a, T b);
        T sin(T a);
        T sqrt(T a);
        T trunc(T a);

        T cast(UInt64 a);
        T cast(double a);
        int cast_int(T a);
        double cast_double(T a);

        T min(T a, T b);
        T max(T a, T b);

        bool equals(T a, T b);
        bool not_equals(T a, T b);
        bool greater_than(T a, T b);
        bool greater_than_or_equal(T a, T b);
        bool less_than(T a, T b);
        bool less_than_or_equal(T a, T b);

        T pstonum_ne(bool CLOCALE, string arg, out bool err);
    }


    public class constants_operators_double : constants_operators<double>
    {
        public double default_ { get { return default; } }

        public double add(double a, double b) { return a + b; }
        public double subtract(double a, double b) { return a - b; }
        public double multiply(double a, double b) { return a * b; }
        public double divide(double a, double b) { return a / b; }

        public double neg(double a) { return -a; }

        public double abs(double a) { return std.abs(a); }
        public double cos(double a) { return std.cos(a); }
        public double exp(double a) { return std.exp(a); }
        public double floor(double a) { return std.floor(a); }
        public double log(double a) { return std.log(a); }
        public double pow(double a, double b) { return std.pow(a, b); }
        public double sin(double a) { return std.sin(a); }
        public double sqrt(double a) { return std.sqrt(a); }
        public double trunc(double a) { return std.trunc(a); }

        public double cast(UInt64 a) { return a; }
        public double cast(double a) { return a; }
        public int cast_int(double a) { return (int)a; }
        public double cast_double(double a) { return a; }

        public double min(double a, double b) { return std.min(a, b); }
        public double max(double a, double b) { return std.max(a, b); }

        public bool equals(double a, double b) { return a == b; }
        public bool not_equals(double a, double b) { return a != b; }
        public bool greater_than(double a, double b) { return a > b; }
        public bool greater_than_or_equal(double a, double b) { return a >= b; }
        public bool less_than(double a, double b) { return a < b; }
        public bool less_than_or_equal(double a, double b) { return a <= b; }

        public double pstonum_ne(bool CLOCALE, string arg, out bool err) { return plib.pglobal.pstonum_ne_nl_fptype(CLOCALE, arg, out err); }
    }


    /// \brief Holds constants used repeatedly.
    ///
    ///  \tparam T floating point type
    ///
    ///  Using the structure members we can avoid magic numbers in the code.
    ///  In addition, this is a typesafe approach.
    ///
    //template <typename T>
    public class constants<T, T_OPS>
        where T_OPS : constants_operators<T>, new()
    {
        protected static constants_operators<T> ops = new T_OPS();

        public static T zero() { return ops.cast(0); }  //static inline constexpr T zero()   noexcept { return static_cast<T>(0); }
        public static T half() { return ops.cast(0.5); }  //static inline constexpr T half()   noexcept { return static_cast<T>(0.5); }
        public static T one() { return ops.cast(1); }  //static inline constexpr T one()    noexcept { return static_cast<T>(1); }
        public static T two() { return ops.cast(2); }  //static inline constexpr T two()    noexcept { return static_cast<T>(2); }
        //static inline constexpr T three()  noexcept { return static_cast<T>(3); }
        public static T four() { return ops.cast(4); }  //static inline constexpr T four()   noexcept { return static_cast<T>(4); }
        //static inline constexpr T hundred()noexcept { return static_cast<T>(100); } // NOLINT

        //static inline constexpr T one_thirds()    noexcept { return fraction(one(), three()); }
        //static inline constexpr T two_thirds()    noexcept { return fraction(two(), three()); }

        //static inline constexpr T ln2()  noexcept { return static_cast<T>(0.6931471805599453094172321214581766L); } // NOLINT
        //static inline constexpr T sqrt2()  noexcept { return static_cast<T>(1.414213562373095048801688724209L); }
        //static inline constexpr T sqrt3()  noexcept { return static_cast<T>(1.7320508075688772935274463415058723L); } // NOLINT
        //static inline constexpr T sqrt3_2()  noexcept { return static_cast<T>(0.8660254037844386467637231707529362L); } // NOLINT
        public static T pi() { return ops.cast(3.14159265358979323846264338327950); }  //static inline constexpr T pi()     noexcept { return static_cast<T>(3.14159265358979323846264338327950L); }

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
        public static T k_b() { return ops.cast(1.38064852e-23); }  //static inline constexpr T k_b() noexcept { return static_cast<T>(1.38064852e-23); }

        /// \brief room temperature (gives VT = 0.02585 at T=300)
        ///
        public static T T0() { return ops.cast(300); }  //static inline constexpr T T0() noexcept { return static_cast<T>(300); }

        /// \brief Elementary charge
        ///
        public static T Q_e() { return ops.cast(1.6021765314e-19); }  //static inline constexpr T Q_e() noexcept { return static_cast<T>(1.6021765314e-19); }

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
        public static T magic(T v) { return v; }  //static inline constexpr const T magic(V &&v) noexcept { return static_cast<T>(v); }

        //template <typename V>
        //static inline constexpr T fraction(V &&v1, V &&v2) noexcept { return static_cast<T>(v1 / v2); }
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
        public static T reciprocal<T, T_OPS>(T v) where T_OPS : constants_operators<T>, new()  //reciprocal(T v) noexcept
        {
            constants_operators<T> ops = new T_OPS();
            return ops.divide(constants<T, T_OPS>.one(), v);  //return constants<T>::one() / v;
        }

        /// \brief abs function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return absolute value of argument
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static T abs<T, T_OPS>(T v) where T_OPS : constants_operators<T>, new()  //abs(T v) noexcept
        {
            constants_operators<T> ops = new T_OPS();
            return ops.abs(v);
        }

        /// \brief sqrt function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return absolute value of argument
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static T sqrt<T, T_OPS>(T v) where T_OPS : constants_operators<T>, new()  //sqrt(T v) noexcept
        {
            constants_operators<T> ops = new T_OPS();
            return ops.sqrt(v);
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
        public static T exp<T, T_OPS>(T v) where T_OPS : constants_operators<T>, new()  //exp(T v) noexcept
        {
            constants_operators<T> ops = new T_OPS();
            return ops.exp(v);
        }

        /// \brief log function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return log(v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static T log<T, T_OPS>(T v) where T_OPS : constants_operators<T>, new()  //log(T v) noexcept
        {
            constants_operators<T> ops = new T_OPS();
            return ops.log(v);
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
        public static T floor<T, T_OPS>(T v) where T_OPS : constants_operators<T>, new()  //floor(T v) noexcept
        {
            constants_operators<T> ops = new T_OPS();
            return ops.floor(v);
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
        public static T sin<T, T_OPS>(T v) where T_OPS : constants_operators<T>, new()  //sin(T v) noexcept
        {
            constants_operators<T> ops = new T_OPS();
            return ops.sin(v);
        }

        /// \brief cos function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return cos(v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static T cos<T, T_OPS>(T v) where T_OPS : constants_operators<T>, new()  //cos(T v) noexcept
        {
            constants_operators<T> ops = new T_OPS();
            return ops.cos(v);
        }

        /// \brief trunc function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \return trunc(v)
        ///
        //template <typename T>
        //static inline constexpr typename std::enable_if<std::is_floating_point<T>::value, T>::type
        public static T trunc<T, T_OPS>(T v) where T_OPS : constants_operators<T>, new()  //trunc(T v) noexcept
        {
            constants_operators<T> ops = new T_OPS();
            return ops.trunc(v);
        }

        /// \brief signum function
        ///
        /// \tparam T type of the argument
        /// \param  v argument
        /// \param  r optional argument, if given will return r and -r instead of 1 and -1
        /// \return signum(v)
        ///
        //template <typename T>
        //static inline constexpr std::enable_if_t<std::is_floating_point<T>::value, T>
        //signum(T v, T r = static_cast<T>(1))
        //{
        //    constexpr const auto z(static_cast<T>(0));
        //    return (v > z) ? r : ((v < z) ? -r : v);
        //}

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
        public static T pow<T, T_OPS>(T v, T p) where T_OPS : constants_operators<T>, new()  //pow(T1 v, T2 p) noexcept
        {
            constants_operators<T> ops = new T_OPS();
            return ops.pow(v, p);
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
