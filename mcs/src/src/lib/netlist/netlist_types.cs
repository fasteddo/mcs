// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using model_map_t = mame.std_unordered_map<string, string>;


namespace mame.netlist
{
    //============================================================
    //  Performance tracking
    //============================================================

//#if NL_KEEP_STATISTICS
//    using nperftime_t = plib::chrono::timer<plib::chrono::exact_ticks, true>;
//    using nperfcount_t = plib::chrono::counter<true>;
//#else
//    using nperftime_t = plib::chrono::timer<plib::chrono::exact_ticks, false>;
//    using nperfcount_t = plib::chrono::counter<false>;
//#endif

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
