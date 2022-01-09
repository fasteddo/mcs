// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;


namespace mame
{
    class keyboard_input_none : input_module
    {
        //MODULE_DEFINITION(KEYBOARD_NONE, keyboard_input_none)
        static osd_module module_creator_keyboard_input_none() { return new keyboard_input_none(); }
        public static readonly module_type KEYBOARD_NONE = MODULE_DEFINITION(module_creator_keyboard_input_none);


        keyboard_input_none() : base(OSD_KEYBOARDINPUT_PROVIDER, "none") { }

        public override int init(osd_options options) { return 0; }
        public override void poll_if_necessary(running_machine machine) { }
        public override void input_init(running_machine machine) { }
        public override void pause() { }
        public override void resume() { }
    }


    class mouse_input_none : input_module
    {
        //MODULE_DEFINITION(MOUSE_NONE, mouse_input_none)
        static osd_module module_creator_mouse_input_none() { return new mouse_input_none(); }
        public static readonly module_type MOUSE_NONE = MODULE_DEFINITION(module_creator_mouse_input_none);

        mouse_input_none() : base(OSD_MOUSEINPUT_PROVIDER, "none") { }
        public override int init(osd_options options) { return 0; }
        public override void input_init(running_machine machine) { }
        public override void poll_if_necessary(running_machine machine) { }
        public override void pause() { }
        public override void resume() { }
    }


    class lightgun_input_none : input_module
    {
        //MODULE_DEFINITION(LIGHTGUN_NONE, lightgun_input_none)
        static osd_module module_creator_lightgun_input_none() { return new lightgun_input_none(); }
        public static readonly module_type LIGHTGUN_NONE = MODULE_DEFINITION(module_creator_lightgun_input_none);

        lightgun_input_none() : base(OSD_LIGHTGUNINPUT_PROVIDER, "none") { }
        public override int init(osd_options options) { return 0; }
        public override void input_init(running_machine machine) { }
        public override void poll_if_necessary(running_machine machine) { }
        public override void pause() { }
        public override void resume() { }
    }


    class joystick_input_none : input_module
    {
        //MODULE_DEFINITION(JOYSTICK_NONE, joystick_input_none)
        static osd_module module_creator_joystick_input_none() { return new joystick_input_none(); }
        public static readonly module_type JOYSTICK_NONE = MODULE_DEFINITION(module_creator_joystick_input_none);

        joystick_input_none() : base(OSD_JOYSTICKINPUT_PROVIDER, "none") { }
        public override int init(osd_options options) { return 0; }
        public override void input_init(running_machine machine) { }
        public override void poll_if_necessary(running_machine machine) { }
        public override void pause() { }
        public override void resume() { }
    }
}
