// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using pmatrix2d_size_type = System.UInt64;  //using size_type = std::size_t;
using pmatrix2d_vrl_size_type = System.UInt64;  //using size_type = std::size_t;
using size_t = System.UInt64;


namespace mame.plib
{
    //template<typename A, typename T>
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


        public pmatrix2d()  //pmatrix2d(A &arena) noexcept
        {
            m_N = 0;
            m_M = 0;
            m_stride = 8;
            m_v = null;
            //, m_a(arena)
        }


        public pmatrix2d(pmatrix2d_size_type N, pmatrix2d_size_type M)  //pmatrix2d(A &arena, size_type N, size_type M)
        {
            m_N = N;
            m_M = M;
            m_v = new std.vector<T>();
            //, m_a(arena)


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
        public Pointer<T> op(pmatrix2d_size_type row)
        {
            return new Pointer<T>(m_v, (int)(m_stride * row));
        }


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
    //template<typename A, typename T>
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
        std.vector<pmatrix2d_vrl_size_type> m_row = new std.vector<pmatrix2d_size_type>();  //plib::arena_vector<A, size_type, PALIGN_VECTOROPT> m_row;
        std.vector<T> m_v = new std.vector<T>();  //plib::arena_vector<A, T, PALIGN_VECTOROPT> m_v;


        public pmatrix2d_vrl()  //pmatrix2d_vrl(A &arena) noexcept
        {
            m_N = 0;
            m_M = 0;
            //m_row(arena)
            //m_v(arena)
        }

        public pmatrix2d_vrl(pmatrix2d_vrl_size_type N, pmatrix2d_vrl_size_type M)  //pmatrix2d_vrl(A &arena, size_type N, size_type M)
        {
            m_N = N;
            m_M = M;
            //m_row(arena)
            //m_v(arena)


            m_row.resize(N + 1, 0);
            m_v.resize(N); //FIXME
        }

        //PCOPYASSIGNMOVE(pmatrix2d_vrl, default)
        //~pmatrix2d_vrl() = default;


        public void resize(pmatrix2d_vrl_size_type N, pmatrix2d_vrl_size_type M)
        {
            m_N = N;
            m_M = M;
            m_row.resize(N + 1);
            for (size_t i = 0; i < m_N; i++)
                m_row[i] = 0;

            m_v.resize(N); //FIXME
        }


        //constexpr T * operator[] (size_type row) noexcept
        public Pointer<T> op(pmatrix2d_vrl_size_type row)
        {
            return new Pointer<T>(m_v, (int)m_row[row]);  //return &(m_v[m_row[row]]);
        }


        //FIXME: no check!
        //T & operator()(size_type r, size_type c) noexcept


        public void set(pmatrix2d_vrl_size_type r, pmatrix2d_vrl_size_type c, T v)  //void set(size_type r, size_type c, const T &v) noexcept
        {
            if (c + m_row[r] >= m_row[r + 1])
            {
                m_v.insert((int)m_row[r + 1], v);  //m_v.insert(m_v.begin() + narrow_cast<std::ptrdiff_t>(m_row[r+1]), v);
                for (pmatrix2d_vrl_size_type i = r + 1; i <= m_N; i++)
                    m_row[i] = m_row[i] + 1;
            }
            else
            {
                this.op(r)[c] = v;  //(*this)[r][c] = v;
            }
        }


        //FIXME: no check!
        //const T & operator()(size_type r, size_type c) const noexcept

        //T * data() noexcept

        //const T * data() const noexcept

        //FIXME: no check!
        public pmatrix2d_vrl_size_type didx(pmatrix2d_vrl_size_type r, pmatrix2d_vrl_size_type c)
        {
            return m_row[r] + c;
        }


        public pmatrix2d_vrl_size_type col_count(pmatrix2d_vrl_size_type row)  //constexpr size_type col_count(size_type row) const noexcept
        {
            return m_row[row + 1] - m_row[row];
        }


        //size_type tx() const { return m_v.size(); }
    }
} // namespace plib
