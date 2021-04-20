// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;//, NL_DEBUG>;
using models_t_map_t = mame.std.unordered_map<string, string>;
using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using netlist_time_ext = mame.plib.ptime_i64;  //netlist_time
using nl_fptype = System.Double;


namespace mame.netlist
{
    /// \brief Constants and const calculations for the library
    ///
    //template<typename T>
    class nlconst_base_nl_fptype : plib.constants_nl_fptype
    {
        //using BC = plib::constants<T>;


        public static nl_fptype np_VT() { return np_VT(plib.constants_nl_fptype.one()); }
        public static nl_fptype np_VT(nl_fptype n) { return np_VT(n, plib.constants_nl_fptype.T0()); }
        public static nl_fptype np_VT(nl_fptype n, nl_fptype temp) { return n * temp * plib.constants_nl_fptype.k_b() / plib.constants_nl_fptype.Q_e(); }  //static inline constexpr T np_VT(T n=BC::one(), T temp=BC::T0()) noexcept { return n * temp * BC::k_b() / BC::Q_e(); }

        public static nl_fptype np_Is() { return (nl_fptype)1e-15; }  //static inline constexpr T np_Is() noexcept { return static_cast<T>(1e-15); } // NOLINT

        /// \brief constant startup gmin
        ///
        /// This should be used during object creation to initialize
        /// conductivities with a reasonable value.
        /// During reset, the object should than make use of exec().gmin()
        /// to use the actual gmin() value.
        public static nl_fptype cgmin() { return plib.constants_nl_fptype.magic(1e-9); } // NOLINT  //static inline constexpr T cgmin() noexcept { return BC::magic(1e-9); } // NOLINT

        // FIXME: Some objects used 1e-15 for initial gmin. Needs to be
        //        aligned with cgmin
        public static nl_fptype cgminalt() { return plib.constants_nl_fptype.magic(1e-15); }  //static inline constexpr T cgminalt() noexcept { return BC::magic(1e-15); } // NOLINT

        /// \brief Multiplier applied to VT in np diode models to determine range for constant model
        ///
        //static inline constexpr T diode_min_cutoff_mult() noexcept { return BC::magic(-5.0); } // NOLINT

        /// \brief Startup voltage used by np diode models
        ///
        public static nl_fptype diode_start_voltage() { return plib.constants_nl_fptype.magic(0.7); }  //static inline constexpr T diode_start_voltage() noexcept { return BC::magic(0.7); } // NOLINT
    }


    /// \brief nlconst_base struct specialized for nl_fptype.
    ///
    class nlconst : nlconst_base_nl_fptype  //struct nlconst : public nlconst_base<nl_fptype>
    {
    }


    /// \brief netlist_sig_t is the type used for logic signals.
    ///
    /// This may be any of bool, uint8_t, uint16_t, uin32_t and uint64_t.
    /// The choice has little to no impact on performance.
    ///
    //using netlist_sig_t = std::uint32_t;


    /// \brief The memory pool for netlist objects
    ///
    /// \note This is not the right location yet.
    ///
    //using device_arena = std::conditional_t<config::use_mempool::value,
    //    plib::mempool_arena<plib::aligned_arena>,
    //    plib::aligned_arena>;
    public class device_arena
    {
    }


    //using host_arena   = plib::aligned_arena;


    /// \brief Interface definition for netlist callbacks into calling code
    ///
    /// A class inheriting from netlist_callbacks_t has to be passed to the netlist_t
    /// constructor. Netlist does processing during construction and thus needs
    /// the object passed completely constructed.
    ///
    public abstract class callbacks_t
    {
        /// \brief logging callback.
        ///
        public abstract void vlog(plib.plog_level l, string ls);

        /// \brief provide library with static solver implementations.
        ///
        /// By default no static solvers are provided since these are
        /// determined by the specific use case. It is up to the implementor
        /// of a callbacks_t implementation to optionally provide such a collection
        /// of symbols.
        ///
        // nl_base.cpp
        public virtual plib.dynlib_base static_solver_lib()  //virtual host_arena::unique_ptr<plib::dynlib_base> static_solver_lib() const;
        {
            return new plib.dynlib_static(null);  //return plib::make_unique<plib::dynlib_static, host_arena>(nullptr);
        }
    }


    //using log_type =  plib::plog_base<callbacks_t, NL_DEBUG>;


    //============================================================
    //  Types needed by various includes
    //============================================================

    namespace detail
    {
        /// \brief Enum specifying the type of object
        ///
        public enum terminal_type
        {
            TERMINAL = 0, ///< object is an analog terminal
            INPUT    = 1, ///< object is an input
            OUTPUT   = 2, ///< object is an output
        }
    }


    //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
    //using netlist_time_ext = plib::ptime<std::conditional<NL_PREFER_INT128 && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
    //static_assert(noexcept(netlist_time::from_nsec(1)), "Not evaluated as constexpr");


    //============================================================
    //  MACROS
    //============================================================
    //template <typename T> inline constexpr netlist_time NLTIME_FROM_NS(T &&t) noexcept { return netlist_time::from_nsec(t); }
    //template <typename T> inline constexpr netlist_time NLTIME_FROM_US(T &&t) noexcept { return netlist_time::from_usec(t); }
    //template <typename T> inline constexpr netlist_time NLTIME_FROM_MS(T &&t) noexcept { return netlist_time::from_msec(t); }


    //struct desc_base
}
