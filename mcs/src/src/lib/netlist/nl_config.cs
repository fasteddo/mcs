// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int64_t = System.Int64;
using nl_fptype = System.Double;


namespace mame.netlist
{
    public static class nl_config_global
    {
        ///
        /// \brief Version - Major.
        ///
        public const int NL_VERSION_MAJOR           = 0;
        ///
        /// \brief Version - Minor.
        ///
        public const int NL_VERSION_MINOR           = 12;
        /// \brief Version - Patch level.
        ///
        const int NL_VERSION_PATCHLEVEL      = 0;


        //============================================================
        //  SETUP
        //============================================================


        //============================================================
        //  GENERAL
        //============================================================

        /// \brief  Make use of a memory pool for performance related objects.
        ///
        /// Set to 1 to compile netlist with memory allocations from a
        /// linear memory pool. This is based of the assumption that
        /// due to enhanced locality there will be less cache misses.
        /// Your mileage may vary.
        ///
        //#ifndef NL_USE_MEMPOOL
        //#define NL_USE_MEMPOOL                 (0)
        //#endif

        /// \brief  Enable queue statistics.
        ///
        /// Queue statistics come at a performance cost. Although
        /// the cost is low, we disable them here since they are
        /// only needed during development.
        ///
        //#ifndef NL_USE_QUEUE_STATS
        public const bool NL_USE_QUEUE_STATS = false;
        //#endif

        /// \brief  Compile in academic solvers
        ///
        /// Set to 0 to disable compiling the following solvers:
        ///
        /// Sherman-Morrison, Woodbury, SOR and GMRES
        ///
        /// In addition, compilation of FLOAT, LONGDOUBLE and FLOATQ128 matrix
        /// solvers will be disabled.
        /// GMRES may be added to productive solvers in the future
        /// again. Compiling in all solvers may increase compile
        /// time significantly.
        ///
        //#ifndef NL_USE_ACADEMIC_SOLVERS
        //#define NL_USE_ACADEMIC_SOLVERS (1)
        //#endif

        /// \brief  Store input values in logic_terminal_t.
        ///
        /// Set to 1 to store values in logic_terminal_t instead of
        /// accessing them indirectly by pointer from logic_net_t.
        /// This approach is stricter and should identify bugs in
        /// the netlist core faster.
        /// By default it is disabled since it is not as fast as
        /// the default approach. It is up to 20% slower.
        ///
        //#ifndef NL_USE_COPY_INSTEAD_OF_REFERENCE
        //#define NL_USE_COPY_INSTEAD_OF_REFERENCE (0)
        //#endif

        /// \brief  Use the truthtable implementation of 7448 instead of the coded device
        ///
        /// FIXME: Using truthtable is a lot slower than the explicit device
        ///        in breakout. Performance drops by 20%. This can be fixed by
        ///        setting param USE_DEACTIVATE for the device.

        //#ifndef NL_USE_TRUTHTABLE_7448
        //#define NL_USE_TRUTHTABLE_7448 (0)
        //#endif

        /// \brief  Use the truthtable implementation of 74107 instead of the coded device
        ///
        /// FIXME: The truthtable implementation of 74107 (JK-Flipflop)
        ///        is included for educational purposes to demonstrate how
        ///        to implement state holding devices as truthtables.
        ///        It will completely nuke performance for pong.

        //#ifndef NL_USE_TRUTHTABLE_74107
        //#define NL_USE_TRUTHTABLE_74107 (0)
        //#endif

        /// \brief  Use the __float128 type for matrix calculations.
        ///
        /// Defaults to \ref PUSE_FLOAT128

        //#ifndef NL_USE_FLOAT128
        //#define NL_USE_FLOAT128 PUSE_FLOAT128
        //#endif

        /// \brief Support float type for matrix calculations.
        ///
        /// Defaults to NL_USE_ACADEMIC_SOLVERS to provide faster build times

        //#ifndef NL_USE_FLOAT_MATRIX
        ////#define NL_USE_FLOAT_MATRIX (NL_USE_ACADEMIC_SOLVERS)
        //#define NL_USE_FLOAT_MATRIX 1
        //#endif

        /// \brief Support long double type for matrix calculations.
        ///
        /// Defaults to NL_USE_ACADEMIC_SOLVERS to provide faster build times

        //#ifndef NL_USE_LONG_DOUBLE_MATRIX
        ////#define NL_USE_LONG_DOUBLE_MATRIX (NL_USE_ACADEMIC_SOLVERS)
        //#define NL_USE_LONG_DOUBLE_MATRIX 1
        //#endif


        //============================================================
        //  DEBUGGING
        //============================================================

        /// \brief Enable compile time debugging code
        ///

        //#ifndef NL_DEBUG
        public const bool NL_DEBUG = false;
        //#define NL_DEBUG                    (true)
        //#endif


        //============================================================
        // Time resolution
        //============================================================

        /// \brief Resolution as clocks per second for timing
        ///
        /// Uses 100 pico second resolution. This is aligned to MAME's
        /// attotime resolution.
        ///
        /// The table below shows the maximum run times depending on
        /// time type size and resolution.
        ///
        ///  | Bits |               Res |       Seconds |   Days | Years |
        ///  | ====-|               ===-|       =======-|   ====-| =====-|
        ///  |  63  |     1,000,000,000 | 9,223,372,037 | 106,752| 292.3 |
        ///  |  63  |    10,000,000,000 |   922,337,204 |  10,675|  29.2 |
        ///  |  63  |   100,000,000,000 |    92,233,720 |   1,068|   2.9 |
        ///  |  63  | 1,000,000,000,000 |     9,223,372 |     107|   0.3 |
        ///
        public const int64_t NETLIST_INTERNAL_RES = 10000000000L;

        /// \brief Recommended clock to be used
        ///
        /// This is the recommended clock to be used in fixed clock applications limited
        /// to 32 bit clock resolution. The MAME code (netlist.cpp) contains code
        /// illustrating how to deal with remainders if \ref NETLIST_INTERNAL_RES is
        /// bigger than NETLIST_CLOCK.
        //static constexpr const int NETLIST_CLOCK = 1'000'000'000;

        //#define NETLIST_INTERNAL_RES        (UINT64_C(1000000000))
        //static constexpr const auto NETLIST_INTERNAL_RES = 1000000000000;

        // FIXME: Belongs into MAME netlist.h
        //#define NETLIST_CLOCK               (NETLIST_INTERNAL_RES)
        //#define NETLIST_INTERNAL_RES      (UINT64_C(1000000000000))
        //#define NETLIST_CLOCK               (UINT64_C(1000000000))


        /// \brief  Floating point types used
        ///
        /// nl_fptype is the floating point type used throughout the
        /// netlist core.
        ///
        ///  Don't change this! Simple analog circuits like pong
        ///  work with float. Kidniki just doesn't work at all.
        ///
        ///  FIXME: More work needed. Review magic numbers.
        ///
        ///

        //using nl_fptype = double;
        ////using nl_fptype = long double;
        ////using nl_fptype = float;


        //============================================================
        //  Asserts
        //============================================================

        //#if defined(MAME_DEBUG) || (NL_DEBUG == true)
        public static void nl_assert(bool x) { global_object.assert(x); }  //#define nl_assert(x)    passert_always(x)  //#define nl_assert(x)    passert_always(x)
        //#else
        //#define nl_assert(x)    do { } while (0)
        //#endif
        public static void nl_assert_always(bool x, string msg) { global_object.assert(x, msg); }  //#define nl_assert_always(x, msg) passert_always_msg(x, msg)
    }


    /// \brief  Specific constants for double floating point type
    ///
    //template <>
    static class fp_constants//<double>
    {
        public static double DIODE_MAXDIFF() { return  1e100; }
        public static double DIODE_MAXVOLT() { return  300.0; }

        public static double TIMESTEP_MAXDIFF() { return  1e100; }
        public static double TIMESTEP_MINDIV() { return  1e-60; }

        public static string name() { return "double"; }
        public static string suffix() { return ""; }
    }
}
