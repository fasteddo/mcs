// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_double = System.Double;
using uint64_t = System.UInt64;


namespace mame.netlist
{
    public static class nl_config_global
    {
        //============================================================
        //  SETUP
        //============================================================


        //============================================================
        //  GENERAL
        //============================================================

        /*! Make use of a memory pool for performance related objects.
         *
         * Set to 1 to compile netlist with memory allocations from a
         * linear memory pool. This is based of the assumption that
         * due to enhanced locality there will be less cache misses.
         * Your mileage may vary.
         *
         */
        //#ifndef NL_USE_MEMPOOL
        //#define NL_USE_MEMPOOL                 (0)
        //#endif

        /*! Enable queue statistics.
         *
         * Queue statistics come at a performance cost. Although
         * the cost is low, we disable them here since they are
         * only needed during development.
         *
         */
        //#ifndef NL_USE_QUEUE_STATS
        public const bool NL_USE_QUEUE_STATS = false;
        //#endif

        /*! Store input values in logic_terminal_t.
         *
         * Set to 1 to store values in logic_terminal_t instead of
         * accessing them indirectly by pointer from logic_net_t.
         * This approach is stricter and should identify bugs in
         * the netlist core faster.
         * By default it is disabled since it is not as fast as
         * the default approach. It is up to 10% slower.
         *
         */
        //#ifndef NL_USE_COPY_INSTEAD_OF_REFERENCE
        //#define NL_USE_COPY_INSTEAD_OF_REFERENCE (0)
        //#endif

        /*
         * FIXME: Using truthtable is a lot slower than the explicit device
         *        in breakout. Performance drops by 20%. This can be fixed by
         *        setting param USE_DEACTIVATE for the device.
         */

        //#ifndef NL_USE_TRUTHTABLE_7448
        //#define NL_USE_TRUTHTABLE_7448 (0)
        //#endif

        /*
         * FIXME: The truthtable implementation of 74107 (JK-Flipflop)
         *        is included for educational purposes to demonstrate how
         *        to implement state holding devices as truthtables.
         *        It will completely nuke performance for pong.
         */

        //#ifndef NL_USE_TRUTHTABLE_74107
        //#define NL_USE_TRUTHTABLE_74107 (0)
        //#endif


        // How many times do we try to resolve links (connections)
        public const int NL_MAX_LINK_RESOLVE_LOOPS   = 100;


        //============================================================
        //  DEBUGGING
        //============================================================

        //#ifndef NL_DEBUG
        public const bool NL_DEBUG = false;
        //#define NL_DEBUG                    (true)
        //#endif


        //============================================================
        // Time resolution
        //============================================================

        // Use nano-second resolution - Sufficient for now
        public const uint64_t NETLIST_INTERNAL_RES = 1000000000;  //static constexpr const auto NETLIST_INTERNAL_RES = 1000000000;
        //static constexpr const auto NETLIST_CLOCK = NETLIST_INTERNAL_RES;

        //#define NETLIST_INTERNAL_RES        (UINT64_C(1000000000))
        //static constexpr const auto NETLIST_INTERNAL_RES = 1000000000000;
        //#define NETLIST_CLOCK               (NETLIST_INTERNAL_RES)
        //#define NETLIST_INTERNAL_RES      (UINT64_C(1000000000000))
        //#define NETLIST_CLOCK               (UINT64_C(1000000000))


        //#define nl_double float
        //using nl_double = double;
    }
}
