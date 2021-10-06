// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    static class vector_ops_global<T, T_OPS>
        where T_OPS : constants_operators<T>, new()
    {
        static readonly T_OPS ops = new T_OPS();


        //void vec_set_scalar(const std::size_t n, VT &v, T && scalar) noexcept
        //void vec_set(const std::size_t n, VT &v, const VS & source) noexcept
        //T vec_mult(const std::size_t n, const V1 & v1, const V2 & v2 ) noexcept
        //T vec_mult2(const std::size_t n, const VT &v) noexcept
        //T vec_sum(const std::size_t n, const VT &v) noexcept
        //void vec_mult_scalar(const std::size_t n, VR & result, const VV & v, T && scalar) noexcept
        //void vec_add_mult_scalar(const std::size_t n, VR & result, const VV & v, T && scalar) noexcept


        public static void vec_add_mult_scalar_p(int n, Pointer<T> result, Pointer<T> v, T scalar)
        {
            for (int i = 0; i < n; i++)
                result[i] = ops.add(result[i], ops.multiply(scalar, v[i]));  //result[i] += scalar * v[i];
        }


        //void vec_add_ip(const std::size_t n, R & result, const V & v) noexcept
        //void vec_sub(const std::size_t n, VR & result, const V1 &v1, const V2 & v2) noexcept
        //void vec_scale(const std::size_t n, V & v, T &&scalar) noexcept
        //T vec_maxabs(const std::size_t n, const V & v) noexcept
    }
} // namespace plib
