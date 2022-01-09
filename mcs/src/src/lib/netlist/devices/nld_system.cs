// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using mt19937_64 = mame.plib.mersenne_twister_t_uint64<  //using mt19937_64 = mersenne_twister_t<
    //uint_fast64_t,
    mame.u64_const_64, mame.u64_const_312, mame.u64_const_156, mame.u64_const_31,  //64, 312, 156, 31,
    mame.u64_const_0xb5026f5aa96619e9, //0xb5026f5aa96619e9ULL,
    mame.u64_const_29, mame.u64_const_0x5555555555555555,  //29, 0x5555555555555555ULL,
    mame.u64_const_17, mame.u64_const_0x71d67fffeda60000,  //17, 0x71d67fffeda60000ULL,
    mame.u64_const_37, mame.u64_const_0xfff7eee000000000,  //37, 0xfff7eee000000000ULL,
    mame.u64_const_43,  //43,
    mame.u64_const_6364136223846793005>;  //6364136223846793005ULL>;

using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    class nld_system_global
    {
        //using NETLIB_NAME(sys_noise_mt_n) = NETLIB_NAME(sys_noise)<plib::mt19937_64, plib::normal_distribution_t>;
        //NETLIB_DEVICE_IMPL(sys_noise_mt_n,      "SYS_NOISE_MT_N",         "SIGMA")
        public static readonly factory.constructor_ptr_t decl_sys_noise_mt_n = NETLIB_DEVICE_IMPL<nld_sys_noise<mt19937_64, plib.normal_distribution_t, plib.distribution_ops_normal>>("SYS_NOISE_MT_N", "SIGMA");
    }
}
