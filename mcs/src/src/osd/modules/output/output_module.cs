// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    abstract class output_module : osd_module
    {
        public const string OSD_OUTPUT_PROVIDER = "output";

        //#define IM_MAME_PAUSE               0
        //#define IM_MAME_SAVESTATE           1


        running_machine m_machine;


        public output_module(string type, string name) : base(type, name) { m_machine = null; }

        public abstract void notify(string outname, int value);


        public void set_machine(running_machine machine) { m_machine = machine; }
        running_machine machine() { return m_machine; }
    }
}
