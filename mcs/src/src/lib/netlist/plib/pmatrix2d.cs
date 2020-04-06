// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;


namespace mame.plib
{
    //template<typename T, typename A = aligned_allocator<T>>
    class pmatrix2d_nl_fptype
    {
        //using value_type = T;
        //using allocator_type = A;

        const UInt32 align_size = 8;  //static constexpr const std::size_t align_size = align_traits<A>::align_size;
        const UInt32 stride_size = 8;  //static constexpr const std::size_t stride_size = align_traits<A>::stride_size;


        UInt32 m_N;
        UInt32 m_M;
        UInt32 m_stride;

        std.vector<nl_fptype> [] m_v;  //std::vector<T, A> m_v;


        public pmatrix2d_nl_fptype()
        {
            m_N = 0;
            m_M = 0;
            m_stride = 8;
            m_v = null;
        }

        public pmatrix2d_nl_fptype(UInt32 N, UInt32 M)
        {
            m_N = N;
            m_M = M;
            m_v = null;

            m_stride = ((M + stride_size-1) / stride_size) * stride_size;

            //m_v.resize(N * m_stride);
            resize(N, M);
        }

        public void resize(UInt32 N, UInt32 M)
        {
            m_N = N;
            m_M = M;
            m_stride = ((M + stride_size-1) / stride_size) * stride_size;

            //m_v.resize(N * m_stride);
            m_v = new std.vector<nl_fptype> [N];
            for (int i = 0; i < N; i++)
                m_v[i] = new std.vector<nl_fptype>(M);
        }

        //C14CONSTEXPR T * operator[] (std::size_t row) noexcept
        //{
        //    return assume_aligned_ptr<T, align_size>(&m_v[m_stride * row]);
        //}

        //constexpr const T * operator[] (std::size_t row) const noexcept
        //{
        //    return assume_aligned_ptr<T, align_size>(&m_v[m_stride * row]);
        //}

        public ListPointer<nl_fptype> op(UInt32 row) { return new ListPointer<nl_fptype>(m_v[row]); }


        //T & operator()(std::size_t r, std::size_t c) noexcept
        //{
        //    return (*this)[r][c];
        //}

        //const T & operator()(std::size_t r, std::size_t c) const noexcept
        //{
        //    return (*this)[r][c];
        //}
    }


    class pmatrix2d_listpointer_nl_fptype
    {
        //using value_type = T;
        //using allocator_type = A;

        const UInt32 align_size = 8;  //static constexpr const std::size_t align_size = align_traits<A>::align_size;
        const UInt32 stride_size = 8;  //static constexpr const std::size_t stride_size = align_traits<A>::stride_size;


        UInt32 m_N;
        UInt32 m_M;
        UInt32 m_stride;

        std.vector<ListPointer<nl_fptype>> [] m_v;  //std::vector<T, A> m_v;


        public pmatrix2d_listpointer_nl_fptype()
        {
            m_N = 0;
            m_M = 0;
            m_stride = 8;
            m_v = null;
        }

        public pmatrix2d_listpointer_nl_fptype(UInt32 N, UInt32 M)
        {
            m_N = N;
            m_M = M;
            m_v = null;

            m_stride = ((M + stride_size-1) / stride_size) * stride_size;

            //m_v.resize(N * m_stride);
            resize(N, M);
        }

        public void resize(UInt32 N, UInt32 M)
        {
            m_N = N;
            m_M = M;
            m_stride = ((M + stride_size-1) / stride_size) * stride_size;

            //m_v.resize(N * m_stride);
            m_v = new std.vector<ListPointer<nl_fptype>> [N];
            for (int i = 0; i < N; i++)
                m_v[i] = new std.vector<ListPointer<nl_fptype>>(M);
        }

        //C14CONSTEXPR T * operator[] (std::size_t row) noexcept
        //{
        //    return assume_aligned_ptr<T, align_size>(&m_v[m_stride * row]);
        //}

        //constexpr const T * operator[] (std::size_t row) const noexcept
        //{
        //    return assume_aligned_ptr<T, align_size>(&m_v[m_stride * row]);
        //}

        public ListPointer<ListPointer<nl_fptype>> op(UInt32 row) { return new ListPointer<ListPointer<nl_fptype>>(m_v[row]); }


        //T & operator()(std::size_t r, std::size_t c) noexcept
        //{
        //    return (*this)[r][c];
        //}

        //const T & operator()(std::size_t r, std::size_t c) const noexcept
        //{
        //    return (*this)[r][c];
        //}
    }
} // namespace plib
