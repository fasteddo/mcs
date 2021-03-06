// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using pmatrix2d_size_type = System.UInt32;  //using size_type = std::size_t;
using pmatrix2d_vrl_size_type = System.UInt32;  //using size_type = std::size_t;
using size_t = System.UInt32;


namespace mame.plib
{
    //template<typename T, typename A = aligned_arena>
    class pmatrix2d<T>
    {
        //using size_type = std::size_t;
        //using arena_type = A;
        //using allocator_type = typename A::template allocator_type<T, PALIGN_VECTOROPT>;

        //using value_type = T;
        //using reference = T &;
        //using const_reference = const T &;

        //using pointer = T *;
        //using const_pointer = const T *;


        const pmatrix2d_size_type align_size = 8;  //static constexpr const size_type align_size = align_traits<allocator_type>::align_size;
        const pmatrix2d_size_type stride_size = 8;  //static constexpr const size_type stride_size = align_traits<allocator_type>::stride_size;


        pmatrix2d_size_type m_N;
        pmatrix2d_size_type m_M;
        pmatrix2d_size_type m_stride;

        std.vector<T> m_v;  //T * __restrict m_v;

        //allocator_type m_a;


        public pmatrix2d()
        {
            m_N = 0;
            m_M = 0;
            m_stride = 8;
            m_v = null;
        }


        public pmatrix2d(pmatrix2d_size_type N, pmatrix2d_size_type M)
        {
            m_N = N;
            m_M = M;
            m_v = new std.vector<T>();


            //gsl_Expects(N>0);
            //gsl_Expects(M>0);
            m_stride = ((M + stride_size-1) / stride_size) * stride_size;
            m_v = new std.vector<T>(N * m_stride);  //m_v = m_a.allocate(N * m_stride);
            for (size_t i = 0; i < N * m_stride; i++)
                m_v[i] = default;  //::new(&m_v[i]) T();
        }

        //~pmatrix2d()


        void resize(pmatrix2d_size_type N, pmatrix2d_size_type M)
        {
            throw new emu_unimplemented();
#if false
            gsl_Expects(N>0);
            gsl_Expects(M>0);
            if (m_v != nullptr)
            {
                for (std::size_t i = 0; i < N * m_stride; i++)
                    (&m_v[i])->~T();
                m_a.deallocate(m_v, N * m_stride);
            }
            m_N = N;
            m_M = M;
            m_stride = ((M + stride_size-1) / stride_size) * stride_size;
            m_v = m_a.allocate(N * m_stride);
            for (std::size_t i = 0; i < N * m_stride; i++)
                ::new(&m_v[i]) T();
#endif
        }

        //constexpr pointer operator[] (size_type row) noexcept
        //{
        //    return &m_v[m_stride * row];
        //}

        //constexpr const_pointer operator[] (size_type row) const noexcept
        //{
        //    return &m_v[m_stride * row];
        //}

        //reference operator()(size_type r, size_type c) noexcept
        //{
        //    return (*this)[r][c];
        //}

        //const_reference operator()(size_type r, size_type c) const noexcept
        //{
        //    return (*this)[r][c];
        //}


        // for compatibility with vrl variant
        //void set(size_type r, size_type c, const value_type &v) noexcept

        //pointer data() noexcept

        //const_pointer data() const noexcept

        //size_type didx(size_type r, size_type c) const noexcept
    }


    // variable row length matrix
    //template<typename T, typename A = aligned_arena>
    public class pmatrix2d_vrl<T>
    {
        //using size_type = std::size_t;
        //using value_type = T;
        //using arena_type = A;
        //using allocator_type = typename A::template allocator_type<T, PALIGN_VECTOROPT>;


        //static constexpr const size_type align_size = align_traits<allocator_type>::align_size;
        //static constexpr const size_type stride_size = align_traits<allocator_type>::stride_size;


        pmatrix2d_vrl_size_type m_N;
        pmatrix2d_vrl_size_type m_M;
        std.vector<pmatrix2d_vrl_size_type> m_row;
        std.vector<T> m_v;


        public pmatrix2d_vrl()
        {
            m_N = 0;
            m_M = 0;
            m_v = new std.vector<T>();
        }

        public pmatrix2d_vrl(pmatrix2d_vrl_size_type N, pmatrix2d_vrl_size_type M)
        {
            m_N = N;
            m_M = M;
            m_v = new std.vector<T>();


            m_row.resize((int)N + 1, 0);
            m_v.resize((int)N); //FIXME
        }

        //PCOPYASSIGNMOVE(pmatrix2d_vrl, default)
        //~pmatrix2d_vrl() = default;


        public void resize(pmatrix2d_vrl_size_type N, pmatrix2d_vrl_size_type M)
        {
            m_N = N;
            m_M = M;
            m_row.resize((int)N + 1);
            for (size_t i = 0; i < m_N; i++)
                m_row[i] = 0;

            m_v.resize((int)N); //FIXME
        }


        //constexpr T * operator[] (size_type row) noexcept

        //constexpr const T * operator[] (size_type row) const noexcept
        public T op(pmatrix2d_vrl_size_type row)
        {
            return m_v[m_row[row]];
        }


        //FIXME: no check!
        //T & operator()(size_type r, size_type c) noexcept


        public void set(pmatrix2d_vrl_size_type r, pmatrix2d_vrl_size_type c, T v)  //void set(size_type r, size_type c, const T &v) noexcept
        {
            throw new emu_unimplemented();
#if false
            if (c + m_row[r] >= m_row[r + 1])
            {
                m_v.insert(m_v.begin() + narrow_cast<std::ptrdiff_t>(m_row[r+1]), v);
                for (size_type i = r + 1; i <= m_N; i++)
                    m_row[i] = m_row[i] + 1;
            }
            else
                (*this)[r][c] = v;
#endif
        }


        //FIXME: no check!
        //const T & operator()(size_type r, size_type c) const noexcept

        //T * data() noexcept

        //const T * data() const noexcept

        //FIXME: no check!
        //size_type didx(size_type r, size_type c) const noexcept


        public size_t colcount(pmatrix2d_vrl_size_type row)  //size_type colcount(size_type row) const noexcept
        {
            throw new emu_unimplemented();
#if false
            return m_row[row + 1] - m_row[row];
#endif
        }


        //size_type tx() const { return m_v.size(); }
    }
} // namespace plib
