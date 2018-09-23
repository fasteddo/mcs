// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


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


#if false
        std::shared_ptr<osd_monitor_info> monitor_from_rect(const osd_rect& rect) override
        {
            if (!m_initialized)
                return nullptr;

            RECT p;
            p.top = rect.top();
            p.left = rect.left();
            p.bottom = rect.bottom();
            p.right = rect.right();

            auto nearest = monitor_from_handle(reinterpret_cast<std::uintptr_t>(MonitorFromRect(&p, MONITOR_DEFAULTTONEAREST)));
            assert(nearest != nullptr);
            return nearest;
        }

        std::shared_ptr<osd_monitor_info> monitor_from_window(const osd_window& window) override
        {
            if (!m_initialized)
                return nullptr;

            auto nearest = monitor_from_handle(reinterpret_cast<std::uintptr_t>(MonitorFromWindow(static_cast<const win_window_info &>(window).platform_window(), MONITOR_DEFAULTTONEAREST)));
            assert(nearest != nullptr);
            return nearest;
        }
#endif


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


#if false
        static BOOL CALLBACK monitor_enum_callback(HMONITOR handle, HDC dc, LPRECT rect, LPARAM data)
        {
            win32_monitor_module* self = reinterpret_cast<win32_monitor_module*>(data);
            MONITORINFOEX info;
            BOOL result;

            // get the monitor info
            info.cbSize = sizeof(info);
            result = GetMonitorInfo(handle, static_cast<LPMONITORINFO>(&info));
            assert(result);
            (void)result; // to silence gcc 4.6

                          // guess the aspect ratio assuming square pixels
            float aspect = static_cast<float>(info.rcMonitor.right - info.rcMonitor.left) / static_cast<float>(info.rcMonitor.bottom - info.rcMonitor.top);

            // allocate a new monitor info
            auto temp = osd::text::from_tstring(info.szDevice);

            // copy in the data
            auto monitor = std::make_shared<win32_monitor_info>(*self, handle, temp.c_str(), aspect);

            // hook us into the list
            self->add_monitor(monitor);

            // enumerate all the available monitors so to list their names in verbose mode
            return TRUE;
        }
#endif
    }
}
