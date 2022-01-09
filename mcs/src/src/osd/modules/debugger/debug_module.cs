// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    abstract class debug_module
    {
        public const string OSD_DEBUG_PROVIDER = "debugger";


        public abstract void init_debugger(running_machine machine);
        public abstract void wait_for_debugger(device_t device, bool firststop);
        public abstract void debugger_update();
    }
}
