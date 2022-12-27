// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int64_t = System.Int64;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using size_t = System.UInt64;

using static mame.cpp_global;
using static mame.netlist.nl_config_global;


namespace mame.netlist
{
    public static partial class nl_config_global
    {
        ///
        /// \brief Version - Major.
        ///
        public const int NL_VERSION_MAJOR           = 0;
        ///
        /// \brief Version - Minor.
        ///
        public const int NL_VERSION_MINOR           = 14;
        /// \brief Version - Patch level.
        ///
        const int NL_VERSION_PATCHLEVEL      = 0;


        //============================================================
        //  SETUP
        //============================================================


        //============================================================
        //  GENERAL
        //============================================================

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
        public const bool NL_USE_ACADEMIC_SOLVERS = false;  //#define NL_USE_ACADEMIC_SOLVERS (1)
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
        //#define NL_USE_COPY_INSTEAD_OF_REFERENCE (0) // FIXME: Move to config struct later
        //#endif

        /// \brief Use backward Euler integration
        ///
        /// This will use backward Euler instead of trapezoidal integration.
        ///
        /// FIXME: Long term this will become a runtime setting. Only the capacitor model
        /// currently has a trapezoidal version and there is no support currently for
        /// variable capacitors.
        /// The change will have impact on timings since trapezoidal improves timing
        /// accuracy.
        //#ifndef NL_USE_BACKWARD_EULER
        //#define NL_USE_BACKWARD_EULER (1) // FIXME: Move to config struct later
        //#endif

        /// \brief  Compile matrix solvers using the __float128 type.
        ///
        /// Defaults to \ref PUSE_FLOAT128
        //#ifndef NL_USE_FLOAT128
        //#define NL_USE_FLOAT128 PUSE_FLOAT128
        //#endif

        /// \brief Avoid unnecessary queue pushes
        ///
        /// Enable the setting below to avoid queue pushes were at execution
        /// no action will be taken. This is academically cleaner, but slower than
        /// allowing this to happen and filter it during during "process".
        /// This is used in core/nets.h
        //#ifndef AVOID_NOOP_QUEUE_PUSHES
        //#define AVOID_NOOP_QUEUE_PUSHES (0)  // FIXME: Move to config struct later


        //============================================================
        //  DEBUGGING
        //============================================================

        /// \brief Enable compile time debugging code
        ///

        //#ifndef NL_DEBUG
        public const bool NL_DEBUG = false;
        //#endif

        public class bool_const_NL_DEBUG : bool_const { public bool value { get { return NL_DEBUG; } } }
    }


    //============================================================
    //  GENERAL
    //============================================================
    public class config_default
    {
        /// \brief  Make use of a memory pool for performance related objects.
        ///
        /// Set to 1 to compile netlist with memory allocations from a
        /// linear memory pool. This is based of the assumption that
        /// due to enhanced locality there will be less cache misses.
        /// Your mileage may vary.
        ///
        //using use_mempool = std::integral_constant<bool, true>;

        /// brief default minimum alignment of mempool_arena
        ///
        /// 256 is the best compromise between logic applications like MAME
        /// TTL games (e.g. pong) and analog applications like e.g. kidnikik sound.
        ///
        /// Best performance for pong is achieved with a value of 16, but this degrades
        /// kidniki performance by ~10%.
        ///
        /// More work is needed here.
        //using mempool_align = std::integral_constant<std::size_t, 16>;

        /// \brief Prefer 128bit int type for ptime if supported
        ///
        /// Set this to one if you want to use 128 bit int for ptime.
        /// This is about 10% slower on a skylake processor for pongf.
        ///
        //using prefer_int128 = std::integral_constant<bool, false>;

        /// \brief  Enable queue statistics.
        ///
        /// Queue statistics come at a performance cost. Although
        /// the cost is low, we disable them here since they are
        /// only needed during development.
        ///
        public const bool use_queue_stats = false;  //using use_queue_stats = std::integral_constant<bool, false>;

        //============================================================
        // Time resolution
        //============================================================

        /// \brief Resolution as clocks per second for timing
        ///
        /// Uses 100 picosecond resolution. This is aligned to MAME's
        /// `attotime` resolution.
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
        public const int64_t INTERNAL_RES = 10_000_000_000L;  //using INTERNAL_RES = std::integral_constant<long long int, 10'000'000'000LL>; // NOLINT

        /// \brief Recommended clock to be used
        ///
        /// This is the recommended clock to be used in fixed clock applications limited
        /// to 32 bit clock resolution. The MAME code (netlist.cpp) contains code
        /// illustrating how to deal with remainders if \ref INTERNAL_RES is bigger than
        /// NETLIST_CLOCK.
        public static int DEFAULT_CLOCK() { return 1_000_000_000; }  //using DEFAULT_CLOCK = std::integral_constant<int, 1'000'000'000>; // NOLINT


        /// \brief Default logic family
        ///
        public static string DEFAULT_LOGIC_FAMILY() { return "74XX"; }


        /// \brief Maximum queue size
        ///
        public const size_t max_queue_size = 1024;  //using max_queue_size = std::integral_constant<std::size_t, 1024>; // NOLINT


        /// \brief Maximum queue size for solvers
        ///
        public const size_t max_solver_queue_size = 512;  //using max_solver_queue_size = std::integral_constant<std::size_t, 512>; // NOLINT


        /// \brief Support float type for matrix calculations.
        ///
        /// Defaults to NL_USE_ACADEMIC_SOLVERS to provide faster build times
        public const bool use_float_matrix = NL_USE_ACADEMIC_SOLVERS;  //using use_float_matrix = std::integral_constant<bool, NL_USE_ACADEMIC_SOLVERS>;

        /// \brief Support long double type for matrix calculations.
        ///
        /// Defaults to NL_USE_ACADEMIC_SOLVERS to provide faster build times
        public const bool use_long_double_matrix = NL_USE_ACADEMIC_SOLVERS;  //using use_long_double_matrix = std::integral_constant<bool, NL_USE_ACADEMIC_SOLVERS>;

        //using use_float128_matrix = std::integral_constant<bool, NL_USE_FLOAT128>;

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
        //using fptype = double;
    }


    /// \brief  Netlist configuration.
    ///
    /// You can override all netlist build defaults here which are defined
    /// in \ref config_default.
    ///
    public class config : config_default
    {
    }


    //using nl_fptype = config::fptype;


    /// \brief  Specific constants for double floating point type
    ///
    //template <>
    static class fp_constants_double  //struct fp_constants<double>
    {
        public static double DIODE_MAXDIFF() { return  1e100; }
        public static double DIODE_MAXVOLT() { return  300.0; }

        public static double TIMESTEP_MAXDIFF() { return  1e100; }
        public static double TIMESTEP_MINDIV() { return  1e-60; }

        public static string name() { return "double"; }
        public static string suffix() { return ""; }
    }


    public static partial class nl_config_global
    {
        //============================================================
        //  Asserts
        //============================================================

        public static void nl_assert(bool x) { assert(x); }  //#define nl_assert(x)    do { if (NL_DEBUG) passert_always(x); } while (0)
        public static void nl_assert_always(bool x, string msg) { assert(x, msg); }  //#define nl_assert_always(x, msg) passert_always_msg(x, msg)
    }
}
