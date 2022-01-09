// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    class win32_monitor_module : monitor_module_base
    {
        //MODULE_DEFINITION(MONITOR_WIN32, win32_monitor_module)
        static osd_module module_creator_win32_monitor_module() { return new win32_monitor_module(); }
        public static readonly module_type MONITOR_WIN32 = MODULE_DEFINITION(module_creator_win32_monitor_module);


        win32_monitor_module()
            : base(OSD_MONITOR_PROVIDER, "win32")
        {
        }


        //std::shared_ptr<osd_monitor_info> monitor_from_rect(const osd_rect& rect) override
        //std::shared_ptr<osd_monitor_info> monitor_from_window(const osd_window& window) override


        protected override int init_internal(osd_options options)
        {
            //throw new emu_unimplemented();
#if false
            // make a list of monitors
            EnumDisplayMonitors(nullptr, nullptr, monitor_enum_callback, reinterpret_cast<std::intptr_t>(this));

            // if we're verbose, print the list of monitors
            {
                for (auto monitor : list())
                {
                    osd_printf_verbose("Video: Monitor %I64u = \"%s\" %s\n", monitor->oshandle(), monitor->devicename().c_str(), monitor->is_primary() ? "(primary)" : "");
                }
            }
#endif

            return 0;
        }


        //static BOOL CALLBACK monitor_enum_callback(HMONITOR handle, HDC dc, LPRECT rect, LPARAM data)
    }
}
