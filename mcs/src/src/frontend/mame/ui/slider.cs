// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int32_t = System.Int32;


namespace mame
{
    //typedef std::function<int32_t(running_machine&, void*, int, std::string*, int32_t)> slider_update;
    public delegate int32_t slider_update(running_machine machine, object arg, int id, out string str, int32_t newval);


    class slider_state
    {
        public const int SLIDER_NOCHANGE     = 0x12345678;

        public slider_update   update;             /* callback */
        public object arg;  //void *          arg;                /* argument */
        public int32_t minval;             /* minimum value */
        public int32_t defval;             /* default value */
        public int32_t maxval;             /* maximum value */
        public int32_t incval;             /* increment value */
        public int32_t id;
        public string description;     /* textual description */
    }
}
