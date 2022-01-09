// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int32_t = System.Int32;


namespace mame
{
    public delegate int32_t slider_update(out string str, int32_t newval);  //typedef std::function<std::int32_t (std::string *, std::int32_t)> slider_update;


    class slider_state
    {
        public const int SLIDER_NOCHANGE     = 0x12345678;


        public slider_update   update;             // callback
        public int32_t minval = 0;             // minimum value
        public int32_t defval = 0;             // default value
        public int32_t maxval = 0;             // maximum value
        public int32_t incval = 0;             // increment value
        public string description;     // textual description


        public slider_state(string title, int32_t min, int32_t def, int32_t max, int32_t inc, slider_update func)
        {
            update = func;
            minval = min;
            defval = def;
            maxval = max;
            incval = inc;
            description = title;
        }
    }
}
