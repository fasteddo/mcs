// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    static class logmacro_global
    {
        const int VERBOSE = 0;

        static void LOG_OUTPUT_FUNC(device_t device, string format, params object [] args) { device.logerror(format, args); }

        const int LOG_GENERAL = 1 << 0;

        static void LOGMASKED(int mask, device_t device, string format, params object [] args) { if ((VERBOSE & mask) != 0) LOG_OUTPUT_FUNC(device, format, args); }  //#define LOGMASKED(mask, ...) do { if (VERBOSE & (mask)) (LOG_OUTPUT_FUNC)(__VA_ARGS__); } while (false)

        public static void LOG(device_t device, string format, params object [] args) { LOGMASKED(LOG_GENERAL, device, format, args); }  //LOGMASKED(LOG_GENERAL, __VA_ARGS__)
    }
}
