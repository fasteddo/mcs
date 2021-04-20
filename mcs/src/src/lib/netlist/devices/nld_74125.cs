// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_74125_global
    {
        //#define TTL_74125_GATE(...)                                                    \
        //        NET_REGISTER_DEV(TTL_74125_GATE, __VA_ARGS__)
        public static void TTL_74125_GATE(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74125_GATE", name); }

        //#define TTL_74126_GATE(...)                                                    \
        //        NET_REGISTER_DEV(TTL_74126_GATE, __VA_ARGS__)
        public static void TTL_74126_GATE(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74126_GATE", name); }
    }


    //template <typename T>
    //struct uptr : public device_arena::unique_ptr<T>


    namespace devices
    {
        //template <typename D>
        //NETLIB_OBJECT(74125_base)


        //struct desc_74125 : public desc_base


        //struct desc_74126 : public desc_74125


        //using NETLIB_NAME(74125) = NETLIB_NAME(74125_base)<desc_74125>;
        //using NETLIB_NAME(74126) = NETLIB_NAME(74125_base)<desc_74126>;


        //NETLIB_DEVICE_IMPL(74125,     "TTL_74125_GATE",     "")
        //NETLIB_DEVICE_IMPL(74126,     "TTL_74126_GATE",     "")
    } //namespace devices
} // namespace netlist
