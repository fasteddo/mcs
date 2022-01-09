// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.util;


namespace mame
{
    public class ui_manager
    {
        // instance variables
        protected running_machine m_machine;


        // construction/destruction
        public ui_manager(running_machine machine)
        {
            m_machine = machine;
        }


        public virtual void set_startup_text(string text, bool force) { }

        // is a menuing system active?  we want to disable certain keyboard/mouse inputs under such context
        public virtual bool is_menu_active() { return false; }

        protected virtual void popup_time_string(int seconds, string message) { }

        protected virtual void menu_reset() { }

        public void popup_time(int seconds, string format, params object [] args)
        {
            // extract the text
            popup_time_string(seconds, string_format(format, args));
        }
    }
}
