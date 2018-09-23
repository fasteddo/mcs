// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class rescap_global
    {
        /* Little helpers for magnitude conversions */
        //#define RES_R(res) ((double)(res))
        public static double RES_K(double res) { return res * 1e3; }
        //#define RES_M(res) ((double)(res) * 1e6)
        //#define RES_INF    (-1)
        public static double CAP_U(double cap) { return cap * 1e-6; }
        //#define CAP_N(cap) ((double)(cap) * 1e-9)
        //#define CAP_P(cap) ((double)(cap) * 1e-12)
        //#define IND_U(ind) ((double)(ind) * 1e-6)
        //#define IND_N(ind) ((double)(ind) * 1e-9)
        //#define IND_P(ind) ((double)(ind) * 1e-12)

        /*  vin --/\r1/\-- out --/\r2/\-- gnd  */
        public static double RES_VOLTAGE_DIVIDER(double r1, double r2) { return (double)r2 / ((double)r1 + (double)r2); }

        public static double RES_2_PARALLEL(double r1, double r2) { return (r1 * r2) / (r1 + r2); }
        public static double RES_3_PARALLEL(double r1, double r2, double r3) { return 1.0 / (1.0 / r1 + 1.0 / r2 + 1.0 / r3); }
        //#define RES_4_PARALLEL(r1, r2, r3, r4)  (1.0 / (1.0 / (r1) + 1.0 / (r2) + 1.0 / (r3) + 1.0 / (r4)))
        //#define RES_5_PARALLEL(r1, r2, r3, r4, r5)  (1.0 / (1.0 / (r1) + 1.0 / (r2) + 1.0 / (r3) + 1.0 / (r4) + 1.0 / (r5)))
        //#define RES_6_PARALLEL(r1, r2, r3, r4, r5, r6)  (1.0 / (1.0 / (r1) + 1.0 / (r2) + 1.0 / (r3) + 1.0 / (r4) + 1.0 / (r5) + 1.0 / (r6)))

        //#define RES_2_SERIAL(r1,r2)             ((r1)+(r2))
    }
}
