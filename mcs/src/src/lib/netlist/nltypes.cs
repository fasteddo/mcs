// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;//, NL_DEBUG>;
using model_map_t = mame.std.unordered_map<string, string>;


namespace mame.netlist
{
    /*! @brief netlist_sig_t is the type used for logic signals.
     *
     *  This may be any of bool, uint8_t, uint16_t, uin32_t and uint64_t.
     *  The choice has little to no impact on performance.
     */
    //using netlist_sig_t = std::uint32_t;

    /**
     * @brief Interface definition for netlist callbacks into calling code
     *
     * A class inheriting from netlist_callbacks_t has to be passed to the netlist_t
     * constructor. Netlist does processing during construction and thus needs
     * the object passed completely constructed.
     *
     */
    public abstract class callbacks_t
    {
        /* logging callback */
        public abstract void vlog(plib.plog_level l, string ls);
    }

    //using log_type =  plib::plog_base<callbacks_t, NL_DEBUG>;


    //============================================================
    //  Performance tracking
    //============================================================

    //template<bool enabled_>
    //using nperftime_t = plib::chrono::timer<plib::chrono::exact_ticks, enabled_>;

    //template<bool enabled_>
    //using nperfcount_t = plib::chrono::counter<enabled_>;


    //============================================================
    //  Types needed by various includes
    //============================================================

    namespace detail
    {
        /*! Enum specifying the type of object */
        public enum terminal_type
        {
            TERMINAL = 0, /*!< object is an analog terminal */
            INPUT    = 1, /*!< object is an input */
            OUTPUT   = 2, /*!< object is an output */
        }


        /*! Type of the model map used.
         *  This is used to hold all #Models in an unordered map
         */
        //using model_map_t = std::unordered_map<pstring, pstring>;
    }
}
