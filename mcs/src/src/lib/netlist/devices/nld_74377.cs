// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_74377_global
    {
        //#define TTL_74377_GATE(name)                                                   \
        //        NET_REGISTER_DEV(TTL_74377_GATE, name)
        public static void TTL_74377_GATE(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74377_GATE", name); }
    }


    namespace devices
    {
        //constexpr const std::array<netlist_time, 2> delay = { NLTIME_FROM_NS(25), NLTIME_FROM_NS(25) };

        //NETLIB_OBJECT(74377_GATE)


        //NETLIB_DEVICE_IMPL(74377_GATE, "TTL_74377_GATE", "")
    } //namespace devices
} // namespace netlist
