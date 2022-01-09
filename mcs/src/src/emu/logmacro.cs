// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.logmacro_global;


namespace mame
{
    public static class logmacro_global
    {
        //#ifndef LOG_OUTPUT_FUNC
        //#ifdef LOG_OUTPUT_STREAM
        //#define LOG_OUTPUT_FUNC [] (auto &&... args) { util::stream_format((LOG_OUTPUT_STREAM), std::forward<decltype(args)>(args)...); }
        //#else
        //#define LOG_OUTPUT_FUNC logerror
        //#endif
        //#endif
        public delegate void LOG_OUTPUT_FUNC(device_t device, string format, params object [] args);
        static void LOG_OUTPUT_FUNC_default(device_t device, string format, params object [] args) { device.logerror(format, args); }


        const int LOG_GENERAL = 1 << 0;


        public static void LOGMASKED(int VERBOSE, int mask, device_t device, string format, params object [] args) { LOGMASKED(VERBOSE, mask, device, LOG_OUTPUT_FUNC_default, format, args); }
        public static void LOGMASKED(int VERBOSE, int mask, device_t device, LOG_OUTPUT_FUNC log_output_func, string format, params object [] args)  //#define LOGMASKED(mask, ...) do { if (VERBOSE & (mask)) (LOG_OUTPUT_FUNC)(__VA_ARGS__); } while (false)
        {
            if ((VERBOSE & mask) != 0)
            {
                if (log_output_func == null)
                    LOG_OUTPUT_FUNC_default(device, format, args);
                else
                    log_output_func(device, format, args);
            }
        }

        public static void LOG(int VERBOSE, device_t device, string format, params object [] args) { LOG(VERBOSE, device, LOG_OUTPUT_FUNC_default, format, args); }
        public static void LOG(int VERBOSE, device_t device, LOG_OUTPUT_FUNC log_output_func, string format, params object [] args) { LOGMASKED(VERBOSE, LOG_GENERAL, device, log_output_func, format, args); }  //LOGMASKED(LOG_GENERAL, __VA_ARGS__)
    }
}
