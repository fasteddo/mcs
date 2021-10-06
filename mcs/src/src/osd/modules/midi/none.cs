// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    class none_module : midi_module
    {
        //MODULE_DEFINITION(MIDI_NONE, none_module)
        static osd_module module_creator_none_module() { return new none_module(); }
        public static readonly module_type MIDI_NONE = MODULE_DEFINITION(module_creator_none_module);


        none_module()
        : base(OSD_MIDI_PROVIDER, "pm")
        {
        }

        public override int init(osd_options options) { return 0; }
        public override void exit() { }

        //public override osd_midi_device create_midi_device() { return new osd_midi_device_none(); }
        public override void list_midi_devices() { g.osd_printf_warning("\nMIDI is not supported in this build\n"); }
    }


    class osd_midi_device_none //: osd_midi_device
    {
        //virtual bool open_input(const char *devname) override { return false; }
        //virtual bool open_output(const char *devname) override { return false; }
        //virtual void close() override { }
        //virtual bool poll() override { return false; }
        //virtual int read(uint8_t *pOut) override { return 0; }
        //virtual void write(uint8_t data) override { }
    }
}
