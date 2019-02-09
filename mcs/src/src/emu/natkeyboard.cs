// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using char32_t = System.UInt32;
using keycode_map = mame.std.unordered_map<System.UInt32, mame.natural_keyboard.keycode_map_entry>;  //typedef std::unordered_map<char32_t, keycode_map_entry> keycode_map;
using u32 = System.UInt32;


namespace mame
{
    // ======================> natural_keyboard
    // buffer to handle copy/paste/insert of keys
    public class natural_keyboard : global_object
    {
        //DISABLE_COPYING(natural_keyboard);


        // keyboard helper function delegates
        //typedef delegate<int (const char32_t *, size_t)> ioport_queue_chars_delegate;
        delegate int ioport_queue_chars_delegate(ListBase<char32_t> param1, UInt32 param2, UInt32 param3);

        //typedef delegate<bool (char32_t)> ioport_accept_char_delegate;
        delegate bool ioport_accept_char_delegate(char32_t param1);

        //typedef delegate<bool ()> ioport_charqueue_empty_delegate;
        delegate bool ioport_charqueue_empty_delegate();


        //enum
        //{
        const int SHIFT_COUNT  = (int)(ioport_global.UCHAR_SHIFT_END - ioport_global.UCHAR_SHIFT_BEGIN + 1);
        const int SHIFT_STATES = 1 << SHIFT_COUNT;
        //}


        // internal keyboard code information
        public class keycode_map_entry
        {
            public ioport_field [] field = new ioport_field[SHIFT_COUNT + 1];
            public UInt32 shift;
        }
        //typedef std::unordered_map<char32_t, keycode_map_entry> keycode_map;


        const bool LOG_NATURAL_KEYBOARD = false;
        const int KEY_BUFFER_SIZE = 4096;
        const char32_t INVALID_CHAR = '?';


        // internal state
        running_machine m_machine;          // reference to our machine
        bool m_in_use;           // is natural keyboard in use?
        u32 m_bufbegin;         // index of starting character
        u32 m_bufend;           // index of ending character
        std.vector<char32_t> m_buffer;           // actual buffer
        UInt32 m_fieldnum;         // current step in multi-key sequence
        bool m_status_keydown;   // current keydown status
        bool m_last_cr;          // was the last char a CR?
        emu_timer m_timer;            // timer for posting characters
        attotime m_current_rate;     // current rate for posting
        ioport_queue_chars_delegate m_queue_chars;      // queue characters callback
        ioport_accept_char_delegate m_accept_char;      // accept character callback
        ioport_charqueue_empty_delegate m_charqueue_empty;  // character queue empty callback
        keycode_map m_keycode_map = new keycode_map();      // keycode map


        // construction/destruction
        //-------------------------------------------------
        //  natural_keyboard - constructor
        //-------------------------------------------------
        public natural_keyboard(running_machine machine)
        {
            m_machine = machine;
            m_in_use = false;
            m_bufbegin = 0;
            m_bufend = 0;
            m_fieldnum = 0;
            m_status_keydown = false;
            m_last_cr = false;
            m_timer = null;
            m_current_rate = attotime.zero;
            m_queue_chars = null;
            m_accept_char = null;
            m_charqueue_empty = null;


            // try building a list of keycodes; if none are available, don't bother
            build_codes(machine.ioport());
            if (!m_keycode_map.empty())
            {
                m_buffer.resize(KEY_BUFFER_SIZE);
                m_timer = machine.scheduler().timer_alloc(timer);
            }

            // retrieve option setting
            set_in_use(machine.options().natural_keyboard());
        }


        // getters and queries
        running_machine machine() { return m_machine; }
        bool empty() { return m_bufbegin == m_bufend; }
        //bool full() const { return ((m_bufend + 1) % m_buffer.size()) == m_bufbegin; }
        //bool can_post() const { return (!m_queue_chars.isnull() || !m_keycode_map.empty()); }
        //bool is_posting() const { return (!empty() || (!m_charqueue_empty.isnull() && !m_charqueue_empty())); }
        public bool in_use() { return m_in_use; }


        // configuration

        //void configure(ioport_queue_chars_delegate queue_chars, ioport_accept_char_delegate accept_char, ioport_charqueue_empty_delegate charqueue_empty);


        //-------------------------------------------------
        //  set_in_use - specify whether the natural
        //  keyboard is active
        //-------------------------------------------------
        void set_in_use(bool usage)
        {
            if (m_in_use != usage)
            {
                // update active usage
                m_in_use = usage;
                machine().options().set_value(emu_options.OPTION_NATURAL_KEYBOARD, usage ? 1 : 0, emu_options.OPTION_PRIORITY_CMDLINE);

                // lock out (or unlock) all keyboard inputs
                foreach (var port in machine().ioport().ports())
                {
                    foreach (ioport_field field in port.Value.fields())
                    {
                        if (field.type() == ioport_type.IPT_KEYBOARD)
                        {
                            field.live().lockout = usage;

                            // clear pressed status when going out of use
                            if (!usage)
                                field.set_value(0);
                        }
                    }
                }
            }
        }


        // posting
        //void post(char32_t ch);
        //void post(const char32_t *text, size_t length = 0, const attotime &rate = attotime::zero);
        //void post_utf8(const char *text, size_t length = 0, const attotime &rate = attotime::zero);
        //void post_coded(const char *text, size_t length = 0, const attotime &rate = attotime::zero);


        // debugging
        //void dump(std::ostream &str) const;
        //std::string dump();


        // internal helpers

        //-------------------------------------------------
        //  build_codes - given an input port table, create
        //  an input code table useful for mapping unicode
        //  chars
        //-------------------------------------------------
        void build_codes(ioport_manager manager)
        {
            // find all shift keys
            UInt32 mask = 0;
            ioport_field [] shift = new ioport_field[SHIFT_COUNT];
            for (int i = 0; i < shift.Length; i++) shift[i] = null;  //std::fill(std::begin(shift), std::end(shift), nullptr);
            foreach (var port in manager.ports())
            {
                foreach (ioport_field field in port.Value.fields())
                {
                    if (field.type() == ioport_type.IPT_KEYBOARD)
                    {
                        ListBase<char32_t> codes = field.keyboard_codes(0);
                        foreach (char32_t code in codes)
                        {
                            if ((code >= ioport_global.UCHAR_SHIFT_BEGIN) && (code <= ioport_global.UCHAR_SHIFT_END))
                            {
                                mask |= 1U << (int)(code - ioport_global.UCHAR_SHIFT_BEGIN);
                                shift[code - ioport_global.UCHAR_SHIFT_BEGIN] = field;
                            }
                        }
                    }
                }
            }

            // iterate over ports and fields
            foreach (var port in manager.ports())
            {
                foreach (ioport_field field in port.Value.fields())
                {
                    if (field.type() == ioport_type.IPT_KEYBOARD)
                    {
                        // iterate over all shift states
                        for (UInt32 curshift = 0; curshift < SHIFT_STATES; ++curshift)
                        {
                            if ((curshift & ~mask) == 0)
                            {
                                // fetch the code, ignoring 0 and shiters
                                ListBase<char32_t> codes = field.keyboard_codes((int)curshift);
                                foreach (char32_t code in codes)
                                {
                                    if (((code < ioport_global.UCHAR_SHIFT_BEGIN) || (code > ioport_global.UCHAR_SHIFT_END)) && (code != 0))
                                    {
                                        // prefer lowest shift state
                                        var found = m_keycode_map.find(code);
                                        if ((null == found) || (found.shift > curshift))
                                        {
                                            keycode_map_entry newcode = new keycode_map_entry();
                                            //std::fill(std::begin(newcode.field), std::end(newcode.field), nullptr);
                                            for (int i = 0; i < newcode.field.Length; i++)
                                                newcode.field[i] = null;
                                            newcode.shift = curshift;

                                            UInt32 fieldnum = 0;
                                            for (UInt32 i = 0, bits = curshift; (i < SHIFT_COUNT) && bits != 0; ++i, bits >>= 1)
                                            {
                                                if (BIT(bits, 0) != 0)
                                                    newcode.field[fieldnum++] = shift[i];
                                            }

                                            assert(fieldnum < newcode.field.Length);
                                            newcode.field[fieldnum] = field;
                                            if (null == found)
                                                m_keycode_map.emplace(code, newcode);
                                            else
                                                found = newcode;

                                            if (LOG_NATURAL_KEYBOARD)
                                            {
                                                machine().logerror("natural_keyboard: code={0} ({1}) port={2} field.name='{3}'\n",  // code=%u (%s) port=%p field.name='%s'\n
                                                        code, unicode_to_string(code), port, field.name());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        //bool can_post_directly(char32_t ch);
        //bool can_post_alternate(char32_t ch);


        //-------------------------------------------------
        //  choose_delay - determine the delay between
        //  posting keyboard events
        //-------------------------------------------------
        attotime choose_delay(char32_t ch)
        {
            // if we have a live rate, just use that
            if (m_current_rate != attotime.zero)
                return m_current_rate;

            // systems with queue_chars can afford a much smaller delay
            if (m_queue_chars != null)
                return attotime.from_msec(10);

            // otherwise, default to constant delay with a longer delay on CR
            return attotime.from_msec((ch == '\r') ? 200 : 50);
        }


        //void internal_post(char32_t ch);


        //-------------------------------------------------
        //  timer - timer callback to keep things flowing
        //  when posting a string of characters
        //-------------------------------------------------
        void timer(object ptr, int param)
        {
            if (m_queue_chars != null)
            {
                // the driver has a queue_chars handler
                while (!empty() && m_queue_chars(m_buffer, m_bufbegin, 1) != 0)
                {
                    m_bufbegin = (m_bufbegin + 1) % (UInt32)m_buffer.size();
                    if (m_current_rate != attotime.zero)
                        break;
                }
            }
            else
            {
                // the driver does not have a queue_chars handler

                // loop through this character's component codes
                keycode_map_entry code = find_code(m_buffer[(int)m_bufbegin]);
                bool advance;
                if (code != null)
                {
                    do
                    {
                        assert(m_fieldnum < code.field.Length);

                        ioport_field field = code.field[m_fieldnum];
                        if (field != null)
                        {
                            // special handling for toggle fields
                            if (!field.live().toggle)
                                field.set_value(!m_status_keydown ? (UInt32)1 : 0);
                            else if (!m_status_keydown)
                                field.set_value(!field.digital_value() ? (UInt32)1 : 0);
                        }
                    }
                    while (code.field[m_fieldnum] != null && (++m_fieldnum < code.field.Length) && m_status_keydown);
                    advance = (m_fieldnum >= code.field.Length) || code.field[m_fieldnum] == null;
                }
                else
                {
                    advance = true;
                }

                if (advance)
                {
                    m_fieldnum = 0;
                    m_status_keydown = !m_status_keydown;

                    // proceed to next character when keydown expires
                    if (!m_status_keydown)
                        m_bufbegin = (m_bufbegin + 1) % (UInt32)m_buffer.size();
                }
            }

            // need to make sure timerproc is called again if buffer not empty
            if (!empty())
                m_timer.adjust(choose_delay(m_buffer[(int)m_bufbegin]));
        }


        //-------------------------------------------------
        //  unicode_to_string - obtain a string
        //  representation of a given code; used for
        //  logging and debugging
        //-------------------------------------------------
        string unicode_to_string(char32_t ch)
        {
            string buffer = "";
            switch (ch)
            {
                // check some magic values
                case '\0':  buffer = "\\0";      break;
                case '\r':  buffer = "\\r";      break;
                case '\n':  buffer = "\\n";      break;
                case '\t':  buffer = "\\t";      break;

                default:
                    // seven bit ASCII is easy
                    if (ch >= 32 && ch < 128)
                    {
                        //char temp[2] = { char(ch), 0 };
                        buffer += (char)ch;
                    }
                    else if (ch >= ioport_global.UCHAR_MAMEKEY_BEGIN)
                    {
                        // try to obtain a codename with code_name(); this can result in an empty string
                        input_code code = new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, 0, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, (input_item_id)(ch - ioport_global.UCHAR_MAMEKEY_BEGIN));
                        buffer = machine().input().code_name(code);
                    }

                    // did we fail to resolve? if so, we have a last resort
                    if (buffer.empty())
                        buffer = string.Format("U+{0}", (UInt32)ch);  // U+%04X
                    break;
            }

            return buffer;
        }


        //-------------------------------------------------
        //  find_code - find a code in our lookup table
        //-------------------------------------------------
        keycode_map_entry find_code(char32_t ch)
        {
            var found = m_keycode_map.find(ch);
            return (null != found) ? found : null;
        }
    }
}
