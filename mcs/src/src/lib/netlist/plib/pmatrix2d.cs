// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;
using size_t = System.UInt32;


namespace mame.plib
{
    //template<typename T, typename A = aligned_allocator<T>>
    public class pmatrix2d_nl_fptype
    {
        //using size_type = std::size_t;
        //using value_type = T;
        //using allocator_type = A;

        const size_t align_size = 8;  //static constexpr const std::size_t align_size = align_traits<A>::align_size;
        const size_t stride_size = 8;  //static constexpr const std::size_t stride_size = align_traits<A>::stride_size;


        size_t m_N;
        size_t m_M;
        size_t m_stride;

        std.vector<nl_fptype> m_v;  //std::vector<T, A> m_v;


        public pmatrix2d_nl_fptype()
        {
            m_N = 0;
            m_M = 0;
            m_stride = 8;
            m_v = new std.vector<nl_fptype>();
        }

        public pmatrix2d_nl_fptype(size_t N, size_t M)
        {
            m_N = N;
            m_M = M;
            m_v = new std.vector<nl_fptype>();

            m_stride = ((M + stride_size - 1) / stride_size) * stride_size;
            m_v.resize((int)(N * m_stride));
        }

        public void resize(size_t N, size_t M)
        {
            m_N = N;
            m_M = M;
            m_stride = ((M + stride_size - 1) / stride_size) * stride_size;
            m_v.resize((int)(N * m_stride));
        }


        public Pointer<nl_fptype> op(size_t row) { return new Pointer<nl_fptype>(m_v, (int)(m_stride * row)); } //C14CONSTEXPR T * operator[] (std::size_t row) noexcept { return assume_aligned_ptr<T, align_size>(&m_v[m_stride * row]); }


        //T & operator()(std::size_t r, std::size_t c) noexcept
        //{
        //    return (*this)[r][c];
        //}

        //const T & operator()(std::size_t r, std::size_t c) const noexcept
        //{
        //    return (*this)[r][c];
        //}

        public Pointer<nl_fptype> data() { return m_v.data(); }  //T * data() noexcept

        public size_t didx(size_t r, size_t c)
        {
            return m_stride * r + c;
        }
    }


    public class pmatrix2d_listpointer_nl_fptype
    {
        //using size_type = std::size_t;
        //using value_type = T;
        //using allocator_type = A;

        const size_t align_size = 8;  //static constexpr const std::size_t align_size = align_traits<A>::align_size;
        const size_t stride_size = 8;  //static constexpr const std::size_t stride_size = align_traits<A>::stride_size;


        size_t m_N;
        size_t m_M;
        size_t m_stride;

        std.vector<Pointer<nl_fptype>> m_v;  //std::vector<T, A> m_v;


        public pmatrix2d_listpointer_nl_fptype()
        {
            m_N = 0;
            m_M = 0;
            m_stride = 8;
            m_v = new std.vector<Pointer<nl_fptype>>();
        }

        public pmatrix2d_listpointer_nl_fptype(size_t N, size_t M)
        {
            m_N = N;
            m_M = M;
            m_v = new std.vector<Pointer<nl_fptype>>();

            m_stride = ((M + stride_size - 1) / stride_size) * stride_size;
            m_v.resize((int)(N * m_stride));
        }

        public void resize(size_t N, size_t M)
        {
            m_N = N;
            m_M = M;
            m_stride = ((M + stride_size - 1) / stride_size) * stride_size;
            m_v.resize((int)(N * m_stride));
        }

        //C14CONSTEXPR T * operator[] (std::size_t row) noexcept
        //{
        //    return assume_aligned_ptr<T, align_size>(&m_v[m_stride * row]);
        //}

        //constexpr const T * operator[] (std::size_t row) const noexcept
        //{
        //    return assume_aligned_ptr<T, align_size>(&m_v[m_stride * row]);
        //}

        public Pointer<Pointer<nl_fptype>> op(size_t row) { return new Pointer<Pointer<nl_fptype>>(m_v, (int)(m_stride * row)); } //C14CONSTEXPR T * operator[] (std::size_t row) noexcept { return assume_aligned_ptr<T, align_size>(&m_v[m_stride * row]); }


        //T & operator()(std::size_t r, std::size_t c) noexcept
        //{
        //    return (*this)[r][c];
        //}

        //const T & operator()(std::size_t r, std::size_t c) const noexcept
        //{
        //    return (*this)[r][c];
        //}

        public Pointer<Pointer<nl_fptype>> data() { return m_v.data(); }  //T * data() noexcept { return m_v.data(); }

        public size_t didx(size_t r, size_t c)
        {
            return m_stride * r + c;
        }
    }
} // namespace plib
