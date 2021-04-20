// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public static class nld_roms_global
    {
        // PROM_82S126(name, cCE1Q, cCE2Q, cA0, cA1, cA2, cA3, cA4, cA5, cA6, cA7)
        //#define PROM_82S126(...) \
        //NET_REGISTER_DEVEXT(PROM_82S126, __VA_ARGS__)
        public static void PROM_82S126(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "PROM_82S126", name); }

        // PROM_74S287(name, cCE1Q, cCE2Q, cA0, cA1, cA2, cA3, cA4, cA5, cA6, cA7)
        //#define PROM_74S287(...) \
        //NET_REGISTER_DEVEXT(PROM_74S287, __VA_ARGS__)
        public static void PROM_74S287(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "PROM_74S287", name); }

        // PROM_82S123(name, cCEQ, cA0, cA1, cA2, cA3, cA4)
        //#define PROM_82S123(...) \
        //NET_REGISTER_DEVEXT(PROM_82S123, __VA_ARGS__)
        public static void PROM_82S123(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "PROM_82S123", name); }

        // EPROM_2716(name, cGQ, cEPQ, cA0, cA1, cA2, cA3, cA4, cA5, cA6, cA7, cA8, cA9, cA10)
        //#define EPROM_2716(...) \
        //NET_REGISTER_DEVEXT(EPROM_2716, __VA_ARGS__)
        public static void EPROM_2716(nlparse_t setup, string name) { nl_setup_global.NET_REGISTER_DEVEXT(setup, "EPROM_2716", name); }

        // PROM_MK28000(name, cOE1, cOE2, cAR, cA0, cA1, cA2, cA3, cA4, cA5, cA6, cA7, cA8, cA9, cA10, cA11)
        //#define PROM_MK28000(...) \
        //NET_REGISTER_DEVEXT(PROM_MK28000, __VA_ARGS__)


        //template <typename N, typename T>
        //constexpr bool TOR(N n, T &a)
        //{
        //    return (n == 0 ? false : TOR(n-1, a) || a[n-1]());
        //}

        //template <typename T>
        //constexpr bool TOR(T &a)
        //{
        //    return TOR(a.size(), a);
        //}
    }


    namespace devices
    { 
        //template <typename D>
        //NETLIB_OBJECT(generic_prom)


        //struct desc_82S126 : public desc_base


        //struct desc_74S287 : public desc_82S126


        //struct desc_82S123 : public desc_base


        //struct desc_2716 : public desc_base


        //using NETLIB_NAME(82S123) = NETLIB_NAME(generic_prom)<desc_82S123>; // 256 bits, 32x8, used as 256x4
        //using NETLIB_NAME(82S126) = NETLIB_NAME(generic_prom)<desc_82S126>; // 1024 bits, 32x32, used as 256x4
        //using NETLIB_NAME(74S287) = NETLIB_NAME(generic_prom)<desc_74S287>; // 1024 bits, 32x32, used as 256x4
        //using NETLIB_NAME(2716)   = NETLIB_NAME(generic_prom)<desc_2716>;   // CE2Q = OE, CE1Q = CE

        //NETLIB_DEVICE_IMPL(82S126,     "PROM_82S126",     "+CE1Q,+CE2Q,+A0,+A1,+A2,+A3,+A4,+A5,+A6,+A7,@VCC,@GND")
        //NETLIB_DEVICE_IMPL(74S287,     "PROM_74S287",     "+CE1Q,+CE2Q,+A0,+A1,+A2,+A3,+A4,+A5,+A6,+A7,@VCC,@GND")
        //NETLIB_DEVICE_IMPL(82S123,     "PROM_82S123",     "+CEQ,+A0,+A1,+A2,+A3,+A4,@VCC,@GND")
        //NETLIB_DEVICE_IMPL(2716,       "EPROM_2716",      "+CE2Q,+CE1Q,+A0,+A1,+A2,+A3,+A4,+A5,+A6,+A7,+A8,+A9,+A10,@VCC,@GND")
    } //namespace devices
} // namespace netlist
