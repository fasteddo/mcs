// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;
using size_t = System.UInt32;


namespace mame.plib
{
    //template<typename T, typename A = aligned_arena>
    public class pmatrix2d_nl_fptype
    {
        //using size_type = std::size_t;
        //using arena_type = A;
        //using allocator_type = typename A::template allocator_type<T>;

        const size_t align_size = 8;  //static constexpr const size_type align_size = align_traits<allocator_type>::align_size;
        const size_t stride_size = 8;  //static constexpr const size_type stride_size = align_traits<allocator_type>::stride_size;

        //using value_type = T;
        //using reference = T &;
        //using const_reference = const T &;

        //using pointer = T *;
        //using const_pointer = const T *;


        size_t m_N;
        size_t m_M;
        size_t m_stride;

        std.vector<nl_fptype> m_v;  //T * __restrict m_v;


        //allocator_type m_a;


        public pmatrix2d_nl_fptype()
        {
            m_N = 0;
            m_M = 0;
            m_stride = 8;
            m_v = null;
        }

        public pmatrix2d_nl_fptype(size_t N, size_t M)
        {
            m_N = N;
            m_M = M;
            m_v = new std.vector<nl_fptype>();

            //gsl_Expects(N>0);
            //gsl_Expects(M>0);
            m_stride = ((M + stride_size - 1) / stride_size) * stride_size;

            m_v = new std.vector<nl_fptype>(N * m_stride);  //m_v = m_a.allocate(N * m_stride);
            for (size_t i = 0; i < N * m_stride; i++)
                m_v[i] = new nl_fptype();  //::new(&m_v[i]) T();
        }

        public void resize(size_t N, size_t M)
        {
            //gsl_Expects(N>0);
            //gsl_Expects(M>0);
            if (m_v != null)
            {
                //for (std::size_t i = 0; i < N * m_stride; i++)
                //    (&m_v[i])->~T();
                //m_a.deallocate(m_v, N * m_stride);
            }

            m_N = N;
            m_M = M;
            m_stride = ((M + stride_size - 1) / stride_size) * stride_size;

            m_v = new std.vector<nl_fptype>(N * m_stride);  //m_v = m_a.allocate(N * m_stride);
            for (size_t i = 0; i < N * m_stride; i++)
                m_v[i] = new nl_fptype();  //::new(&m_v[i]) T();
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


        public Pointer<nl_fptype> op(size_t row) { return new Pointer<nl_fptype>(m_v, (int)(m_stride * row)); } //C14CONSTEXPR T * operator[] (std::size_t row) noexcept { return assume_aligned_ptr<T, align_size>(&m_v[m_stride * row]); }


        // for compatibility with vrl variant
        //void set(size_type r, size_type c, value_type &v)
        //{
        //    (*this)[r][c] = v;
        //}

        //pointer data()
        //{
        //    return m_v;
        //}

        //const_pointer data()
        //{
        //    return m_v;
        //}

        //size_type didx(size_type r, size_type c)
        //{
        //    return m_stride * r + c;
        //}
    }


    public class pmatrix2d_listpointer_nl_fptype
    {
        //using size_type = std::size_t;
        //using arena_type = A;
        //using allocator_type = typename A::template allocator_type<T>;

        const size_t align_size = 8;  //static constexpr const size_type align_size = align_traits<allocator_type>::align_size;
        const size_t stride_size = 8;  //static constexpr const size_type stride_size = align_traits<allocator_type>::stride_size;

        //using value_type = T;
        //using reference = T &;
        //using const_reference = const T &;

        //using pointer = T *;
        //using const_pointer = const T *;


        size_t m_N;
        size_t m_M;
        size_t m_stride;

        std.vector<Pointer<nl_fptype>> m_v;  //T * __restrict m_v;


        //allocator_type m_a;


        public pmatrix2d_listpointer_nl_fptype()
        {
            m_N = 0;
            m_M = 0;
            m_stride = 8;
            m_v = null;
        }

        public pmatrix2d_listpointer_nl_fptype(size_t N, size_t M)
        {
            m_N = N;
            m_M = M;
            m_v = new std.vector<Pointer<nl_fptype>>();

            //gsl_Expects(N>0);
            //gsl_Expects(M>0);
            m_stride = ((M + stride_size - 1) / stride_size) * stride_size;

            m_v = new std.vector<Pointer<nl_fptype>>(N * m_stride);  //m_v = m_a.allocate(N * m_stride);
            for (size_t i = 0; i < N * m_stride; i++)
                m_v[i] = new Pointer<nl_fptype>();  //::new(&m_v[i]) T();
        }

        public void resize(size_t N, size_t M)
        {
            //gsl_Expects(N>0);
            //gsl_Expects(M>0);
            if (m_v != null)
            {
                //for (std::size_t i = 0; i < N * m_stride; i++)
                //    (&m_v[i])->~T();
                //m_a.deallocate(m_v, N * m_stride);
            }

            m_N = N;
            m_M = M;
            m_stride = ((M + stride_size - 1) / stride_size) * stride_size;

            m_v = new std.vector<Pointer<nl_fptype>>(N * m_stride);  //m_v = m_a.allocate(N * m_stride);
            for (size_t i = 0; i < N * m_stride; i++)
                m_v[i] = new Pointer<nl_fptype>();  //::new(&m_v[i]) T();
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


        public Pointer<Pointer<nl_fptype>> op(size_t row) { return new Pointer<Pointer<nl_fptype>>(m_v, (int)(m_stride * row)); } //C14CONSTEXPR T * operator[] (std::size_t row) noexcept { return assume_aligned_ptr<T, align_size>(&m_v[m_stride * row]); }


        // for compatibility with vrl variant
        //void set(size_type r, size_type c, value_type &v)
        //{
        //    (*this)[r][c] = v;
        //}

        //pointer data()
        //{
        //    return m_v;
        //}

        //const_pointer data()
        //{
        //    return m_v;
        //}

        //size_type didx(size_type r, size_type c)
        //{
        //    return m_stride * r + c;
        //}
    }


    // variable row length matrix
    //template<typename T, typename A = aligned_arena>
    public class pmatrix2d_vrl_nl_fptype
    {
        //using size_type = std::size_t;
        //using value_type = T;
        //using arena_type = A;
        //using allocator_type = typename A::template allocator_type<T>;

        //static constexpr const size_type align_size = align_traits<allocator_type>::align_size;
        //static constexpr const size_type stride_size = align_traits<allocator_type>::stride_size;


        size_t m_N;
        size_t m_M;
        std.vector<size_t> m_row;  //std::vector<size_type, typename A::template allocator_type<size_type>> m_row;
        std.vector<nl_fptype> m_v;  //std::vector<T, allocator_type> m_v;


        pmatrix2d_vrl_nl_fptype()
        {
            m_N = 0;
            m_M = 0;
            m_v = new std.vector<nl_fptype>();
        }

        pmatrix2d_vrl_nl_fptype(size_t N, size_t M)
        {
            m_N = N;
            m_M = M;
            m_v = new std.vector<nl_fptype>();


            m_row.resize((int)N + 1, 0);
            m_v.resize((int)N); //FIXME
        }

        public void resize(size_t N, size_t M)
        {
            m_N = N;
            m_M = M;
            m_row.resize((int)N + 1);
            for (size_t i = 0; i < m_N; i++)
                m_row[i] = 0;
            m_v.resize((int)N); //FIXME
        }

        //constexpr T * operator[] (size_type row) noexcept
        //{
        //    return &(m_v[m_row[row]]);
        //}

        //constexpr const T * operator[] (size_type row) const noexcept
        //{
        //    return &(m_v[m_row[row]]);
        //}

        //FIXME: no check!
        //T & operator()(size_type r, size_type c) noexcept
        //{
        //    return (*this)[r][c];
        //}

        public Pointer<nl_fptype> op(size_t row) { return new Pointer<nl_fptype>(m_v, (int)m_row[row]); }  //constexpr T * operator[] (size_type row) noexcept { return &(m_v[m_row[row]]); }


        public void set(size_t r, size_t c, nl_fptype v)
        {
            if (c + m_row[r] >= m_row[r + 1])
            {
                m_v.insert((int)m_row[r + 1], v);  //m_v.insert(m_v.begin() + narrow_cast<std::ptrdiff_t>(m_row[r+1]), v);
                for (size_t i = r + 1; i <= m_N; i++)
                    m_row[i] = m_row[i] + 1;
            }
            else
            {
                throw new emu_unimplemented();
#if false
                this[r][c] = v;  //(*this)[r][c] = v;
#endif
            }
        }


        //FIXME: no check!
        //const T & operator()(size_type r, size_type c) const noexcept
        //{
        //    return (*this)[r][c];
        //}

        //T * data() noexcept
        //{
        //    return m_v.data();
        //}

        //const T * data() const noexcept
        //{
        //    return m_v.data();
        //}

        //FIXME: no check!
        public size_t didx(size_t r, size_t c)  //size_type didx(size_type r, size_type c) const noexcept
        {
            return m_row[r] + c;
        }

        //std::size_t tx() const { return m_v.size(); }
    }


    // variable row length matrix
    //template<typename T, typename A = aligned_arena>
    public class pmatrix2d_vrl_listpointer_nl_fptype
    {
        //using size_type = std::size_t;
        //using value_type = T;
        //using arena_type = A;
        //using allocator_type = typename A::template allocator_type<T>;

        //static constexpr const size_type align_size = align_traits<allocator_type>::align_size;
        //static constexpr const size_type stride_size = align_traits<allocator_type>::stride_size;


        size_t m_N;
        size_t m_M;
        std.vector<size_t> m_row;  //std::vector<size_type, typename A::template allocator_type<size_type>> m_row;
        std.vector<Pointer<nl_fptype>> m_v;  //std::vector<T, allocator_type> m_v;


        pmatrix2d_vrl_listpointer_nl_fptype()
        {
            m_N = 0;
            m_M = 0;
            m_v = new std.vector<Pointer<nl_fptype>>();
        }

        pmatrix2d_vrl_listpointer_nl_fptype(size_t N, size_t M)
        {
            m_N = N;
            m_M = M;
            m_v = new std.vector<Pointer<nl_fptype>>();


            m_row.resize((int)N + 1, 0);
            m_v.resize((int)N); //FIXME
        }

        public void resize(size_t N, size_t M)
        {
            m_N = N;
            m_M = M;
            m_row.resize((int)N + 1);
            for (size_t i = 0; i < m_N; i++)
                m_row[i] = 0;
            m_v.resize((int)N); //FIXME
        }

        //constexpr T * operator[] (size_type row) noexcept
        //{
        //    return &(m_v[m_row[row]]);
        //}

        //constexpr const T * operator[] (size_type row) const noexcept
        //{
        //    return &(m_v[m_row[row]]);
        //}

        //FIXME: no check!
        //T & operator()(size_type r, size_type c) noexcept
        //{
        //    return (*this)[r][c];
        //}

        public Pointer<nl_fptype> op(size_t row)  //constexpr T * operator[] (size_type row) noexcept { return &(m_v[m_row[row]]); }
        {
            throw new emu_unimplemented();
#if false
            return new Pointer<nl_fptype>(m_v, (int)m_row[row]); 
#endif
        }


        public void set(size_t r, size_t c, Pointer<nl_fptype> v)
        {
            if (c + m_row[r] >= m_row[r + 1])
            {
                m_v.insert((int)m_row[r + 1], v);  //m_v.insert(m_v.begin() + narrow_cast<std::ptrdiff_t>(m_row[r+1]), v);
                for (size_t i = r + 1; i <= m_N; i++)
                    m_row[i] = m_row[i] + 1;
            }
            else
            {
                throw new emu_unimplemented();
#if false
                this[r][c] = v;  //(*this)[r][c] = v;
#endif
            }
        }


        //FIXME: no check!
        //const T & operator()(size_type r, size_type c) const noexcept
        //{
        //    return (*this)[r][c];
        //}

        //T * data() noexcept
        //{
        //    return m_v.data();
        //}

        //const T * data() const noexcept
        //{
        //    return m_v.data();
        //}

        //FIXME: no check!
        public size_t didx(size_t r, size_t c)  //size_type didx(size_type r, size_type c) const noexcept
        {
            return m_row[r] + c;
        }

        //std::size_t tx() const { return m_v.size(); }
    }
} // namespace plib
