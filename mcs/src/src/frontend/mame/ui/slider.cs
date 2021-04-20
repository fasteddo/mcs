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
        public object arg = null;  //void *          arg;                /* argument */
        public int32_t minval = 0;             /* minimum value */
        public int32_t defval = 0;             /* default value */
        public int32_t maxval = 0;             /* maximum value */
        public int32_t incval = 0;             /* increment value */
        public int32_t id = 0;
        public string description;     /* textual description */
    }
}
