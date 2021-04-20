// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class rescap_global
    {
        // Little helpers for magnitude conversions
        //#define RES_R(res) ((double)(res))
        public static double RES_K(double res) { return res * 1e3; }
        public static double RES_M(double res) { return res * 1e6; }
        //#define RES_INF    (-1)
        public static double CAP_U(double cap) { return cap * 1e-6; }
        public static double CAP_N(double cap) { return cap * 1e-9; }
        //#define CAP_P(cap) ((double)(cap) * 1e-12)
        //#define IND_U(ind) ((double)(ind) * 1e-6)
        //#define IND_N(ind) ((double)(ind) * 1e-9)
        //#define IND_P(ind) ((double)(ind) * 1e-12)

        //  vin --/\r1/\-- out --/\r2/\-- gnd
        public static double RES_VOLTAGE_DIVIDER(double r1, double r2) { return (double)r2 / ((double)r1 + (double)r2); }

        public static double RES_2_PARALLEL(double r1, double r2) { return (r1 * r2) / (r1 + r2); }
        public static double RES_3_PARALLEL(double r1, double r2, double r3) { return 1.0 / (1.0 / r1 + 1.0 / r2 + 1.0 / r3); }
        //#define RES_4_PARALLEL(r1, r2, r3, r4)  (1.0 / (1.0 / (r1) + 1.0 / (r2) + 1.0 / (r3) + 1.0 / (r4)))
        //#define RES_5_PARALLEL(r1, r2, r3, r4, r5)  (1.0 / (1.0 / (r1) + 1.0 / (r2) + 1.0 / (r3) + 1.0 / (r4) + 1.0 / (r5)))
        //#define RES_6_PARALLEL(r1, r2, r3, r4, r5, r6)  (1.0 / (1.0 / (r1) + 1.0 / (r2) + 1.0 / (r3) + 1.0 / (r4) + 1.0 / (r5) + 1.0 / (r6)))

        //#define RES_2_SERIAL(r1,r2)             ((r1)+(r2))

        // macro for the RC time constant on a 74LS123 with C > 1000pF
        // R is in ohms, C is in farads
        //#define TIME_OF_74LS123(r,c)            (0.45 * (double)(r) * (double)(c))

        // macros for the RC time constant on a 555 timer IC
        // R is in ohms, C is in farads
        //#define PERIOD_OF_555_MONOSTABLE_NSEC(r,c)  ((attoseconds_t)(1100000000 * (double)(r) * (double)(c)))
        //#define PERIOD_OF_555_ASTABLE_NSEC(r1,r2,c) ((attoseconds_t)( 693000000 * ((double)(r1) + 2.0 * (double)(r2)) * (double)(c)))
        //#define PERIOD_OF_555_MONOSTABLE(r,c)       attotime::from_nsec(PERIOD_OF_555_MONOSTABLE_NSEC(r,c))
        //#define PERIOD_OF_555_ASTABLE(r1,r2,c)      attotime::from_nsec(PERIOD_OF_555_ASTABLE_NSEC(r1,r2,c))
    }
}
