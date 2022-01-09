// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using mersenne_twister_t_uint64_T = System.UInt64;
using mt19937_64 = mame.plib.mersenne_twister_t_uint64<  //using mt19937_64 = mersenne_twister_t<
    //uint_fast64_t,
    mame.u64_const_64, mame.u64_const_312, mame.u64_const_156, mame.u64_const_31,  //64, 312, 156, 31,
    mame.u64_const_0xb5026f5aa96619e9, //0xb5026f5aa96619e9ULL,
    mame.u64_const_29, mame.u64_const_0x5555555555555555,  //29, 0x5555555555555555ULL,
    mame.u64_const_17, mame.u64_const_0x71d67fffeda60000,  //17, 0x71d67fffeda60000ULL,
    mame.u64_const_37, mame.u64_const_0xfff7eee000000000,  //37, 0xfff7eee000000000ULL,
    mame.u64_const_43,  //43,
    mame.u64_const_6364136223846793005>;  //6364136223846793005ULL>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using size_t = System.UInt64;


namespace mame.plib
{
    /// \brief Mersenne Twister implementation which is state saveable
    ///
    /// This is a Mersenne Twister implementation which is state saveable.
    /// It has been written following this wikipedia entry:
    ///
    ///     https://en.wikipedia.org/wiki/Mersenne_Twister
    ///
    /// The implementation has basic support for the interface described here
    ///
    ///     https://en.cppreference.com/w/cpp/numeric/random/mersenne_twister_engine
    ///
    /// so that it can be used with the C++11 random environment
    ///
    //template<typename T,
    //    std::size_t w, std::size_t N, std::size_t m, std::size_t r,
    //    T a,
    //    std::size_t u, T d,
    //    std::size_t s, T b,
    //    std::size_t t, T c,
    //    std::size_t l, T f>
    class mersenne_twister_t_uint64<
        //typename T,
        size_t_w, size_t_N, size_t_m, size_t_r,  //std::size_t w, std::size_t N, std::size_t m, std::size_t r,
        T_a,  //T a,
        size_t_u, T_d,  //std::size_t u, T d,
        size_t_s, T_b,  //std::size_t s, T b,
        size_t_t, T_c,  //std::size_t t, T c,
        size_t_l, T_f>  //std::size_t l, T f>
        where size_t_w : u64_const, new()
        where size_t_N : u64_const, new()
        where T_f : u64_const, new()
    {
        static readonly size_t w = new size_t_w().value;
        static readonly size_t N = new size_t_N().value;
        static readonly mersenne_twister_t_uint64_T f = new T_f().value;


        size_t m_p;
        std.array<mersenne_twister_t_uint64_T, size_t_N> m_mt;  //std::array<T, N> m_mt;


        mersenne_twister_t_uint64()
        {
            m_p = N;


            seed(5489);
        }


        //static constexpr T min() noexcept { return T(0); }
        //static constexpr T max() noexcept { return ~T(0) >> (sizeof(T)*8 - w); }

        //template <typename ST>
        //void save_state(ST &st)
        //{
        //    st.save_item(m_p,  "index");
        //    st.save_item(m_mt, "mt");
        //}


        void seed(mersenne_twister_t_uint64_T val)  //void seed(T val) noexcept
        {
            mersenne_twister_t_uint64_T lowest_w = mersenne_twister_t_uint64_T.MaxValue >> (int)(8 * 8 - w);  //const T lowest_w(~T(0) >> (sizeof(T)*8 - w));
            m_p = N;
            m_mt[0] = val;
            for (size_t i = 1; i < N; i++)
                m_mt[i] = (f * (m_mt[i - 1] ^ (m_mt[i - 1] >> (int)(w - 2))) + i) & lowest_w;
        }


        //T operator()() noexcept
        //{
        //    const T lowest_w(~T(0) >> (sizeof(T)*8 - w));
        //    if (m_p >= N)
        //        twist();
        //
        //    T y = m_mt[m_p++];
        //    y = y ^ ((y >> u) & d);
        //    y = y ^ ((y << s) & b);
        //    y = y ^ ((y << t) & c);
        //    y = y ^ (y >> l);
        //
        //    return y & lowest_w;
        //}

        //void discard(std::size_t v) noexcept
        //{
        //    if  (v > N - m_p)
        //    {
        //        v -= N - m_p;
        //        twist();
        //    }
        //    while (v > N)
        //    {
        //        v -= N;
        //        twist();
        //    }
        //    m_p += v;
        //}

        //void twist() noexcept
        //{
        //    const T lowest_w(~T(0) >> (sizeof(T)*8 - w));
        //    const T lower_mask((T(1) << r) - 1); // That is, the binary number of r 1's
        //    const T upper_mask((~lower_mask) & lowest_w);
        //
        //    for (std::size_t i=0; i<N; i++)
        //    {
        //        const T x((m_mt[i] & upper_mask) + (m_mt[(i+1) % N] & lower_mask));
        //        const T xA((x >> 1) ^ ((x & 1) ? a : 0));
        //        m_mt[i] = m_mt[(i + m) % N] ^ xA;
        //     }
        //    m_p = 0;
        //}
    }


    //template <typename FT, typename T>
    //FT normalize_uniform(T &p, FT m = constants<FT>::one(), FT b = constants<FT>::zero()) noexcept
    //{
    //    constexpr const auto mmin(narrow_cast<FT>(T::min()));
    //    constexpr const auto mmax(narrow_cast<FT>(T::max()));
    //    // -> 0 to a
    //    return (narrow_cast<FT>(p())- mmin) / (mmax - mmin) * m - b;
    //}


    //template<typename FT>
    //class uniform_distribution_t


    public interface distribution_ops<T> { T new_(nl_fptype dev); }

    public class distribution_ops_normal : distribution_ops<normal_distribution_t> { public normal_distribution_t new_(nl_fptype dev) { return new normal_distribution_t(dev); } }


    //template<typename FT>
    public class normal_distribution_t
    {
        std.array<nl_fptype, u64_const_256> m_buf = new std.array<nl_fptype, u64_const_256>();  //std::array<FT, 256> m_buf;
        size_t m_p;
        nl_fptype m_stddev;  //FT m_stddev;


        public normal_distribution_t(nl_fptype dev)  //normal_distribution_t(FT dev)
        {
            m_p = m_buf.size();
            m_stddev = dev;
        }


        // Donald Knuth, Algorithm P (Polar method)

        //template <typename P>
        //FT operator()(P &p) noexcept
        //{
        //    if (m_p >= m_buf.size())
        //        fill(p);
        //    return m_buf[m_p++];
        //}

        //template <typename ST>
        //void save_state(ST &st)
        //{
        //    st.save_item(m_p,   "m_p");
        //    st.save_item(m_buf, "m_buf");
        //}

        //template <typename P>
        //void fill(P &p) noexcept
        //{
        //    for (std::size_t i = 0; i < m_buf.size(); i += 2)
        //    {
        //        FT s;
        //        FT v1;
        //        FT v2;
        //        do
        //        {
        //            v1 = normalize_uniform(p, constants<FT>::two(), constants<FT>::one()); // [-1..1[
        //            v2 = normalize_uniform(p, constants<FT>::two(), constants<FT>::one()); // [-1..1[
        //            s = v1 * v1 + v2 * v2;
        //        } while (s >= constants<FT>::one());
        //        if (s == constants<FT>::zero())
        //        {
        //            m_buf[i] = s;
        //            m_buf[i+1] = s;
        //        }
        //        else
        //        {
        //            // last value without error for log(s)/s
        //            // double: 1.000000e-305
        //            // float: 9.999999e-37
        //            // FIXME: with 128 bit randoms log(s)/w will fail 1/(2^128) ~ 2.9e-39
        //            const auto m(m_stddev * plib::sqrt(-constants<FT>::two() * plib::log(s)/s));
        //            m_buf[i] = /*mean+*/ m * v1;
        //            m_buf[i+1] = /*mean+*/ m * v2;
        //        }
        //    }
        //    m_p = 0;
        //}
    }


    //using mt19937_64 = mersenne_twister_t<
    //    uint_fast64_t,
    //    64, 312, 156, 31,
    //    0xb5026f5aa96619e9ULL,
    //    29, 0x5555555555555555ULL,
    //    17, 0x71d67fffeda60000ULL,
    //    37, 0xfff7eee000000000ULL,
    //    43,
    //    6364136223846793005ULL>;
}
