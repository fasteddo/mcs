// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    abstract class midi_module : osd_module
    {
        public const string OSD_MIDI_PROVIDER = "midiprovider";


        protected midi_module(string type, string name) : base(type, name) { }


        //abstract osd_midi_device create_midi_device();
        // FIXME: should return a list of strings ...
        public abstract void list_midi_devices();
    }
}
