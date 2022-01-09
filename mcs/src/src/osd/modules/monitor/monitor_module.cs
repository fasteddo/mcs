// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    //============================================================
    //  monitor_module
    //============================================================
    abstract class monitor_module : osd_module
    {
        public const string OSD_MONITOR_PROVIDER   = "monitorprovider";


        protected monitor_module(string type, string name)
            : base(type, name)
        {
        }
    }
}
