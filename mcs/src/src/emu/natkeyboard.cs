// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using char32_t = System.UInt32;
using natural_keyboard_keycode_map_entries = mame.std.vector<mame.natural_keyboard.keycode_map_entry>; //typedef std::vector<keycode_map_entry> keycode_map_entries;
using natural_keyboard_keycode_map = mame.std.unordered_map<System.UInt32, mame.std.vector<mame.natural_keyboard.keycode_map_entry>>;  //typedef std::unordered_map<char32_t, keycode_map_entries> keycode_map;
using size_t = System.UInt32;
using u32 = System.UInt32;
using unsigned = System.UInt32;


namespace mame
{
    // ======================> natural_keyboard
    // buffer to handle copy/paste/insert of keys
    public class natural_keyboard : global_object
    {
        //DISABLE_COPYING(natural_keyboard);


        // keyboard helper function delegates
        //using ioport_queue_chars_delegate = delegate<int (const char32_t *, size_t)>;
        delegate int ioport_queue_chars_delegate(Pointer<char32_t> param1, size_t param2);

        //using ioport_accept_char_delegate = delegate<bool (char32_t)>;
        delegate bool ioport_accept_char_delegate(char32_t param1);

        //using ioport_charqueue_empty_delegate = delegate<bool ()>;
        delegate bool ioport_charqueue_empty_delegate();


        //enum
        //{
        const int SHIFT_COUNT  = (int)(ioport_global.UCHAR_SHIFT_END - ioport_global.UCHAR_SHIFT_BEGIN + 1);
        const int SHIFT_STATES = 1 << SHIFT_COUNT;
        //}


        public class size_t_constant_SHIFT_COUNT : uint32_constant { public UInt32 value { get { return SHIFT_COUNT; } } }
        public class size_t_constant_SHIFT_COUNT_1 : uint32_constant { public UInt32 value { get { return SHIFT_COUNT + 1; } } }


        // internal keyboard code information
        public class keycode_map_entry
        {
            public std.array<ioport_field, size_t_constant_SHIFT_COUNT_1> field = new std.array<ioport_field, size_t_constant_SHIFT_COUNT_1>();
            public unsigned shift;
            public ioport_condition condition;
        }
        //typedef std::vector<keycode_map_entry> keycode_map_entries;
        //typedef std::unordered_map<char32_t, keycode_map_entries> keycode_map;


        // per-device character-to-key mapping
        class kbd_dev_info
        {
            public device_t device;
            public std.vector<ioport_field> keyfields;
            public natural_keyboard_keycode_map codemap;
            public bool keyboard = false;
            public bool keypad = false;
            public bool enabled = false;


            public kbd_dev_info(device_t dev)
            {
                device = dev;
            }
        }


        const bool LOG_NATURAL_KEYBOARD = false;
        const int KEY_BUFFER_SIZE = 4096;
        const char32_t INVALID_CHAR = '?';


        // internal state
        std.vector<kbd_dev_info> m_keyboards = new std.vector<kbd_dev_info>();        // info on keyboard devices in system
        running_machine m_machine;          // reference to our machine
        bool m_have_charkeys;    // are there keys with character?
        bool m_in_use;           // is natural keyboard in use?
        u32 m_bufbegin;         // index of starting character
        u32 m_bufend;           // index of ending character
        std.vector<char32_t> m_buffer;           // actual buffer
        keycode_map_entry m_current_code;     // current code being typed
        unsigned m_fieldnum;         // current step in multi-key sequence
        bool m_status_keydown;   // current keydown status
        bool m_last_cr;          // was the last char a CR?
        emu_timer m_timer;            // timer for posting characters
        attotime m_current_rate;     // current rate for posting
        ioport_queue_chars_delegate m_queue_chars;      // queue characters callback
        ioport_accept_char_delegate m_accept_char;      // accept character callback
        ioport_charqueue_empty_delegate m_charqueue_empty;  // character queue empty callback


        // construction/destruction
        //-------------------------------------------------
        //  natural_keyboard - constructor
        //-------------------------------------------------
        public natural_keyboard(running_machine machine)
        {
            m_machine = machine;
            m_have_charkeys = false;
            m_in_use = false;
            m_bufbegin = 0;
            m_bufend = 0;
            m_current_code = null;
            m_fieldnum = 0;
            m_status_keydown = false;
            m_last_cr = false;
            m_timer = null;
            m_current_rate = attotime.zero;
            m_queue_chars = null;
            m_accept_char = null;
            m_charqueue_empty = null;


            // try building a list of keycodes; if none are available, don't bother
            build_codes();
            if (!m_keyboards.empty())
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
        //bool can_post() const { return m_have_charkeys || !m_queue_chars.isnull(); }
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
                foreach (kbd_dev_info devinfo in m_keyboards)
                {
                    foreach (ioport_field field in devinfo.keyfields)
                    {
                        bool is_keyboard = field.type() == ioport_type.IPT_KEYBOARD;
                        field.live().lockout = !devinfo.enabled || (is_keyboard && usage);

                        // clear pressed status when going out of use
                        if (is_keyboard && !usage)
                            field.set_value(0);
                    }
                }
            }
        }


        //size_t keyboard_count() const { return m_keyboards.size(); }
        //device_t &keyboard_device(size_t n) const { return m_keyboards[n].device; }
        //bool keyboard_is_keypad(size_t n) const { return !m_keyboards[n].keyboard; }
        //bool keyboard_enabled(size_t n) const { return m_keyboards[n].enabled; }
        //void enable_keyboard(size_t n) { set_keyboard_enabled(n, true); }
        //void disable_keyboard(size_t n) { set_keyboard_enabled(n, false); }


        // posting
        //void post_char(char32_t ch, bool normalize_crlf = false);
        //void post(const char32_t *text, size_t length = 0, const attotime &rate = attotime::zero);
        //void post_utf8(const char *text, size_t length = 0, const attotime &rate = attotime::zero);
        //void post_utf8(const std::string &text, const attotime &rate = attotime::zero);
        //void post_coded(const char *text, size_t length = 0, const attotime &rate = attotime::zero);
        //void post_coded(const std::string &text, const attotime &rate = attotime::zero);


        //-------------------------------------------------
        //  paste - does a paste from the keyboard
        //-------------------------------------------------
        public void paste()
        {
            throw new emu_unimplemented();
        }


        // debugging
        //void dump(std::ostream &str) const;
        //std::string dump();


        // internal helpers

        //-------------------------------------------------
        //  build_codes - given an input port table, create
        //  an input code table useful for mapping unicode
        //  chars
        //-------------------------------------------------
        void build_codes()
        {
            ioport_manager manager = machine().ioport();

            // find all the devices with keyboard or keypad inputs
            foreach (var port in manager.ports())
            {
                var devinfo = 
                        std.find_if(
                            m_keyboards,
                            (kbd_dev_info info) =>
                            {
                                return port.second().device() == info.device.get();
                            });

                foreach (ioport_field field in port.second().fields())
                {
                    bool is_keyboard = field.type() == ioport_type.IPT_KEYBOARD;
                    if (is_keyboard || (field.type() == ioport_type.IPT_KEYPAD))
                    {
                        if (default == devinfo)
                        {
                            //devinfo = m_keyboards.emplace(devinfo, port.second->device());
                            devinfo = new kbd_dev_info(port.second().device());
                            m_keyboards.push_back(devinfo);
                        }

                        devinfo.keyfields.emplace_back(field);

                        if (is_keyboard)
                            devinfo.keyboard = true;
                        else
                            devinfo.keypad = true;
                    }
                }
            }

            std.sort(
                    m_keyboards,
                    (kbd_dev_info l, kbd_dev_info r) =>
                    {
                        return std.strcmp(l.device.get().tag(), r.device.get().tag());
                    });

            // set up key mappings for each keyboard
            std.array<ioport_field, size_t_constant_SHIFT_COUNT> shift = new std.array<ioport_field, size_t_constant_SHIFT_COUNT>();
            unsigned mask;
            bool have_keyboard = false;
            foreach (kbd_dev_info devinfo in m_keyboards)
            {
                if (LOG_NATURAL_KEYBOARD)
                    machine().logerror("natural_keyboard: building codes for {0}... ({1} fields)\n", devinfo.device.get().tag(), devinfo.keyfields.size());

                // enable all pure keypads and the first keyboard
                if (!devinfo.keyboard || !have_keyboard)
                    devinfo.enabled = true;
                have_keyboard = have_keyboard || devinfo.keyboard;

                // find all shift keys
                std.fill(shift, null);  //std::fill(std::begin(shift), std::end(shift), nullptr);
                mask = 0;
                foreach (ioport_field field in devinfo.keyfields)
                {
                    if (field.type() == ioport_type.IPT_KEYBOARD)
                    {
                        std.vector<char32_t> codes = field.keyboard_codes(0);
                        foreach (char32_t code in codes)
                        {
                            if ((code >= ioport_global.UCHAR_SHIFT_BEGIN) && (code <= ioport_global.UCHAR_SHIFT_END))
                            {
                                mask |= 1U << (int)(code - ioport_global.UCHAR_SHIFT_BEGIN);
                                shift[code - ioport_global.UCHAR_SHIFT_BEGIN] = field;
                                if (LOG_NATURAL_KEYBOARD)
                                    machine().logerror("natural_keyboard: UCHAR_SHIFT_{0} found\n", code - ioport_global.UCHAR_SHIFT_BEGIN + 1);
                            }
                        }
                    }
                }

                // iterate over keyboard/keypad fields
                foreach (ioport_field field in devinfo.keyfields)
                {
                    field.live().lockout = !devinfo.enabled;
                    if (field.type() == ioport_type.IPT_KEYBOARD)
                    {
                        // iterate over all shift states
                        for (unsigned curshift = 0; curshift < SHIFT_STATES; ++curshift)
                        {
                            if ((curshift & ~mask) == 0)
                            {
                                // fetch the code, ignoring 0 and shifters
                                std.vector<char32_t> codes = field.keyboard_codes((int)curshift);
                                foreach (char32_t code in codes)
                                {
                                    if (((code < ioport_global.UCHAR_SHIFT_BEGIN) || (code > ioport_global.UCHAR_SHIFT_END)) && (code != 0))
                                    {
                                        m_have_charkeys = true;
                                        var found = devinfo.codemap.find(code);  //keycode_map::iterator const found(devinfo.codemap.find(code));
                                        keycode_map_entry newcode = new keycode_map_entry();
                                        std.fill(newcode.field, null);  //std::fill(std::begin(newcode.field), std::end(newcode.field), nullptr);
                                        newcode.shift = curshift;
                                        newcode.condition = field.condition();

                                        unsigned fieldnum = 0;
                                        for (unsigned i = 0, bits = curshift; (i < SHIFT_COUNT) && (bits != 0); ++i, bits >>= 1)
                                        {
                                            if (BIT(bits, 0) != 0)
                                                newcode.field[fieldnum++] = shift[i];
                                        }

                                        newcode.field[fieldnum] = field;
                                        if (default == found)
                                        {
                                            natural_keyboard_keycode_map_entries entries = new natural_keyboard_keycode_map_entries();
                                            entries.emplace_back(newcode);
                                            devinfo.codemap.emplace(code, entries);
                                        }
                                        else
                                        {
                                            found.emplace_back(newcode);
                                        }

                                        if (LOG_NATURAL_KEYBOARD)
                                        {
                                            machine().logerror("natural_keyboard: code={0} ({1}) port={2} field.name='{3}'\n",
                                                    code, unicode_to_string(code), field.port(), field.name());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // sort mapping entries by shift state
                foreach (var mapping in devinfo.codemap)
                {
                    std.sort(
                            mapping.second(),
                            (keycode_map_entry x, keycode_map_entry y) => { return x.shift.CompareTo(y.shift); });  //[] (keycode_map_entry const &x, keycode_map_entry const &y) { return x.shift < y.shift; });
                }
            }
        }


        //void set_keyboard_enabled(size_t n, bool enable);
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
                while (!empty() && m_queue_chars(new Pointer<char32_t>(m_buffer, (int)m_bufbegin), 1) != 0)  //while (!empty() && m_queue_chars(&m_buffer[m_bufbegin], 1))
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
                if (m_fieldnum == 0)
                    m_current_code = find_code(m_buffer[m_bufbegin]);

                bool advance;

                if (m_current_code != null)
                {
                    keycode_map_entry code = m_current_code;

                    do
                    {
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
                    while (code.field[m_fieldnum] != null && (++m_fieldnum < code.field.size()) && m_status_keydown);

                    advance = (m_fieldnum >= code.field.size()) || code.field[m_fieldnum] == null;
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
            foreach (kbd_dev_info devinfo in m_keyboards)
            {
                if (devinfo.enabled)
                {
                    var found = devinfo.codemap.find(ch);  //keycode_map::const_iterator found = devinfo.codemap.find(ch);
                    if (default != found)
                    {
                        foreach (keycode_map_entry entry in found)
                        {
                            if (entry.condition.eval())
                                return entry;
                        }
                    }
                }
            }

            return null;
        }
    }
}
