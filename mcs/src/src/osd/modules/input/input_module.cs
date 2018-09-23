// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    abstract class input_module : osd_module
    {
        public const string OSD_KEYBOARDINPUT_PROVIDER = "keyboardprovider";
        public const string OSD_MOUSEINPUT_PROVIDER = "mouseprovider";
        public const string OSD_LIGHTGUNINPUT_PROVIDER = "lightgunprovider";
        public const string OSD_JOYSTICKINPUT_PROVIDER = "joystickprovider";


        public input_module(string type, string name) : base(type, name) { }


        public abstract void input_init(running_machine machine);
        public abstract void poll_if_necessary(running_machine machine);
        public abstract void pause();
        public abstract void resume();
    }
}
