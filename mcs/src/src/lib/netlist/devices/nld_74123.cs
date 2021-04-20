// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_74123_global
    {
        //#define TTL_74123(name)                                                         \
        //        NET_REGISTER_DEV(TTL_74123, name)
        public static void TTL_74123(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74123", name); }

        //#define TTL_74121(name)                                                         \
        //        NET_REGISTER_DEV(TTL_74121, name)
        public static void TTL_74121(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "TTL_74121", name); }

        //#define TTL_9602(name)                                                         \
        //        NET_REGISTER_DEV(TTL_9602, name)
        public static void TTL_9602(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "TTL_9602", name); }

        //#define CD4538(name)                                                         \
        //        NET_REGISTER_DEV(CD4538, name)
        public static void CD4538(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEV(setup, "CD4538", name); }
    }
}
