// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public class ui_manager : global_object
    {
        // instance variables
        protected running_machine m_machine;
        bool m_show_timecode_counter;
        bool m_show_timecode_total;


        // construction/destruction
        public ui_manager(running_machine machine)
        {
            m_machine = machine;
            m_show_timecode_counter = false;
            m_show_timecode_total = false;
        }


        public virtual void set_startup_text(string text, bool force) { }

        // is a menuing system active?  we want to disable certain keyboard/mouse inputs under such context
        public virtual bool is_menu_active() { return false; }

        public void set_show_timecode_counter(bool value) { m_show_timecode_counter = value; m_show_timecode_total = true; }

        public bool show_timecode_counter() { return m_show_timecode_counter; }
        public bool show_timecode_total() { return m_show_timecode_total; }

        protected virtual void popup_time_string(int seconds, string message) { }

        protected virtual void menu_reset() { }

        public void popup_time(int seconds, string format, params object [] args)
        {
            // extract the text
            popup_time_string(seconds, string.Format(format, args));
        }
    }
}
