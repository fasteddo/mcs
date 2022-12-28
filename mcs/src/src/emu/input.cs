// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using mame.osd;

using size_t = System.UInt64;
using u32 = System.UInt32;

using static mame.cpp_global;
using static mame.input_internal;
using static mame.inputcode_global;
using static mame.osd.inputman_global;
using static mame.profiler_global;
using static mame.util;


namespace mame
{
    public class size_t_const_DEVICE_CLASS_MAXIMUM : u64_const { public UInt64 value { get { return (UInt64)input_device_class.DEVICE_CLASS_MAXIMUM; } } }


    // ======================> input_manager
    // global machine-level information about devices
    public class input_manager : osd.input_manager
    {
        // controller alias table typedef
        //using devicemap_table = std::map<std::string, std::string>;


        // internal state
        running_machine m_machine;
        input_code [] m_switch_memory = new input_code[64];

        // classes
        std.array<input_class, size_t_const_DEVICE_CLASS_MAXIMUM> m_class = new std.array<input_class, size_t_const_DEVICE_CLASS_MAXIMUM>();  //std::array<std::unique_ptr<input_class>, DEVICE_CLASS_MAXIMUM> m_class;


        // construction/destruction
        public input_manager(running_machine machine)
        {
            m_machine = machine;


            // reset code memory
            reset_memory();

            // create pointers for the classes
            m_class[(int)input_device_class.DEVICE_CLASS_KEYBOARD] = new input_class_keyboard(this);
            m_class[(int)input_device_class.DEVICE_CLASS_MOUSE] = new input_class_mouse(this);
            m_class[(int)input_device_class.DEVICE_CLASS_LIGHTGUN] = new input_class_lightgun(this);
            m_class[(int)input_device_class.DEVICE_CLASS_JOYSTICK] = new input_class_joystick(this);

#if MAME_DEBUG
            for (input_device_class devclass = DEVICE_CLASS_FIRST_VALID; devclass <= DEVICE_CLASS_LAST_VALID; ++devclass)
            {
                assert(m_class[devclass] != nullptr);
                assert(m_class[devclass]->devclass() == devclass);
            }
#endif
        }


        // OSD interface
        public osd.input_device add_device(input_device_class devclass, string name, string id, object internal_) { throw new emu_unimplemented(); }


        // getters
        public running_machine machine() { return m_machine; }
        public input_class device_class(input_device_class devclass) { assert(devclass >= input_device_class.DEVICE_CLASS_FIRST_VALID && devclass <= input_device_class.DEVICE_CLASS_LAST_VALID); return m_class[(int)devclass]; }


        // input code readers

        //-------------------------------------------------
        //  code_value - return the value of a given
        //  input code
        //-------------------------------------------------
        public int code_value(input_code code)
        {
            g_profiler.start(profile_type.PROFILER_INPUT);


            int result = 0;

            // dummy loop to allow clean early exits
            do
            {
                // return 0 for any invalid devices
                input_device device = device_from_code(code);
                if (device == null)
                    break;

                // also return 0 if the device class is disabled
                input_class devclass = m_class[(int)code.device_class()];
                if (!devclass.enabled())
                    break;

                // if this is not a multi device, only return data for item 0 and iterate over all
                int startindex = code.device_index();
                int stopindex = startindex;
                if (!devclass.multi())
                {
                    if (startindex != 0)
                        break;
                    stopindex = devclass.maxindex();
                }

                // iterate over all device indices
                input_item_class targetclass = code.item_class();
                for (int curindex = startindex; curindex <= stopindex; curindex++)
                {
                    // lookup the item for the appropriate index
                    code.set_device_index(curindex);
                    input_device_item item = item_from_code(code);
                    if (item == null)
                        continue;

                    // process items according to their native type
                    switch (targetclass)
                    {
                        case input_item_class.ITEM_CLASS_ABSOLUTE:
                            if (result == 0)
                                result = item.read_as_absolute(code.item_modifier());
                            break;

                        case input_item_class.ITEM_CLASS_RELATIVE:
                            result += item.read_as_relative(code.item_modifier());
                            break;

                        case input_item_class.ITEM_CLASS_SWITCH:
                            result |= item.read_as_switch(code.item_modifier());
                            break;

                        default:
                            break;
                    }
                }
            } while (false);


            // stop the profiler before exiting
            g_profiler.stop();

            return result;
        }

        public bool code_pressed(input_code code) { return code_value(code) != 0; }

        //-------------------------------------------------
        //  code_pressed_once - return non-zero if a given
        //  input code has transitioned from off to on
        //  since the last call
        //-------------------------------------------------
        public bool code_pressed_once(input_code code)
        {
            // look for the code in the memory
            bool curvalue = code_pressed(code);
            int empty = -1;
            for (int memnum = 0; memnum < (int)std.size(m_switch_memory); memnum++)
            {
                // were we previous pressed on the last time through here?
                if (m_switch_memory[memnum] == code)
                {
                    // if no longer pressed, clear entry
                    if (!curvalue)
                        m_switch_memory[memnum] = INPUT_CODE_INVALID;

                    // always return false
                    return false;
                }

                // remember the first empty entry
                if (empty == -1 && m_switch_memory[memnum] == INPUT_CODE_INVALID)
                    empty = memnum;
            }

            // if we get here, we were not previously pressed; if still not pressed, return 0
            if (!curvalue)
                return false;

            // otherwise, add the code to the memory and return true
            //assert(empty != -1);
            if (empty != -1)
                m_switch_memory[empty] = code;

            return true;
        }


        // input code helpers

        //-------------------------------------------------
        //  device_from_code - given an input_code return
        //  a pointer to the associated device
        //-------------------------------------------------
        input_device device_from_code(input_code code)
        {
            // if the class is valid, return the appropriate device pointer
            input_device_class devclass = code.device_class();
            if (devclass >= input_device_class.DEVICE_CLASS_FIRST_VALID && devclass <= input_device_class.DEVICE_CLASS_LAST_VALID)
                return m_class[(int)devclass].device(code.device_index());

            // otherwise, return NULL
            return null;
        }

        //-------------------------------------------------
        //  item_from_code - given an input_code return
        //  a pointer to the appropriate input_device_item
        //-------------------------------------------------
        input_device_item item_from_code(input_code code)
        {
            // first get the device; if none, then we don't have an item
            input_device device = device_from_code(code);
            if (device == null)
                return null;

            // then return the device's item
            return device.item(code.item_id());
        }

        //input_code code_from_itemid(input_item_id itemid) const;

        //-------------------------------------------------
        //  code_name - convert an input code into a
        //  friendly name
        //-------------------------------------------------
        public string code_name(input_code code)
        {
            string str = "";

            // if nothing there, return an empty string
            input_device_item item = item_from_code(code);
            if (item == null)
                return str;

            // determine the devclass part
            string devclass = code_string_table.Find(devclass_string_table, (u32)code.device_class());

            // determine the devindex part
            string devindex = string_format("{0}", code.device_index() + 1);

            // if we're unifying all devices, don't display a number
            if (!m_class[(int)code.device_class()].multi())
                devindex = "";

            // keyboard 0 doesn't show a class or index if it is the only one
            input_device_class device_class = item.device().devclass();
            if (device_class == input_device_class.DEVICE_CLASS_KEYBOARD && m_class[(int)device_class].maxindex() == 0)
            {
                devclass = "";
                devindex = "";
            }

            // devcode part comes from the item name
            string devcode = item.name();

            // determine the modifier part
            string modifier = code_string_table.Find(modifier_string_table, (u32)code.item_modifier());

            // devcode is redundant with joystick switch left/right/up/down
            if (device_class == input_device_class.DEVICE_CLASS_JOYSTICK && code.item_class() == input_item_class.ITEM_CLASS_SWITCH)
            {
                if (code.item_modifier() >= input_item_modifier.ITEM_MODIFIER_LEFT && code.item_modifier() <= input_item_modifier.ITEM_MODIFIER_DOWN)
                    devcode = "";
            }

            // concatenate the strings
            str = devclass;
            if (!string.IsNullOrEmpty(devindex))
                str += " " + devindex;
            if (!string.IsNullOrEmpty(devcode))
                str += " " + devcode;
            if (!string.IsNullOrEmpty(modifier))
                str += " " + modifier;

            // delete any leading spaces
            return str.Trim();  //return std::string(strtrimspace(str));
        }


        //string code_to_token(input_code code) const;
        //input_code code_from_token(const char *_token);


        //-------------------------------------------------
        //  standard_token - return the standard token for
        //  the given input item ID
        //-------------------------------------------------
        public string standard_token(input_item_id itemid)
        {
            return itemid <= input_item_id.ITEM_ID_MAXIMUM ? itemid_token_table[(int)itemid].m_string : null;
        }


        // input sequence readers

        //-------------------------------------------------
        //  seq_pressed - return true if the given sequence
        //  of switch inputs is "pressed"
        //-------------------------------------------------
        public bool seq_pressed(input_seq seq)
        {
            // iterate over all of the codes
            bool result = false;
            bool invert = false;
            bool first = true;
            for (int codenum = 0; ; codenum++)
            {
                input_code code = seq[codenum];
                if (code == input_seq.not_code)
                {
                    // handle NOT
                    invert = true;
                }
                else if (code == input_seq.or_code || code == input_seq.end_code)
                {
                    // handle OR and END

                    // if we have a positive result from the previous set, we're done
                    if (result || code == input_seq.end_code)
                        break;

                    // otherwise, reset our state
                    result = false;
                    invert = false;
                    first = true;
                }
                else
                {
                    // handle everything else as a series of ANDs

                    // if this is the first in the sequence, result is set equal
                    if (first)
                        result = code_pressed(code) ^ invert;

                    // further values are ANDed
                    else if (result)
                        result &= code_pressed(code) ^ invert;

                    // no longer first, and clear the invert flag
                    first = invert = false;
                }
            }

            // return the result if we queried at least one switch
            return result;
        }

        //-------------------------------------------------
        //  seq_axis_value - return the value of an axis
        //  defined in an input sequence
        //-------------------------------------------------
        public int seq_axis_value(input_seq seq, out input_item_class itemclass)
        {
            // start with no valid classes
            input_item_class itemclasszero = input_item_class.ITEM_CLASS_INVALID;
            itemclass = input_item_class.ITEM_CLASS_INVALID;

            // iterate over all of the codes
            int result = 0;
            bool invert = false;
            bool enable = true;
            for (int codenum = 0; ; codenum++)
            {
                input_code code = seq[codenum];
                if (code == input_seq.not_code)
                {
                    // handle NOT
                    invert = true;
                }
                else if (code == input_seq.end_code)
                {
                    // handle OR and END
                    break;
                }
                else if (code == input_seq.or_code)
                {
                    // handle OR

                    // reset invert and enable for the next group
                    invert = false;
                    enable = true;
                }
                else if (enable)
                {
                    // handle everything else only if we're still enabled

                    // switch codes serve as enables
                    if (code.item_class() == input_item_class.ITEM_CLASS_SWITCH)
                    {
                        // AND against previous digital codes
                        if (enable)
                            enable &= code_pressed(code) ^ invert;

                        // FIXME: need to clear current group value if enable became false
                        // you can't create a sequence where this matters using the internal UI,
                        // but you can by editing a CFG file (or controller config file)
                    }
                    else
                    {
                        // non-switch codes are analog values

                        int value = code_value(code);

                        // if we got a 0 value, don't do anything except remember the first type
                        if (value == 0)
                        {
                            if (itemclasszero == input_item_class.ITEM_CLASS_INVALID)
                                itemclasszero = code.item_class();
                        }
                        else if (code.item_class() == input_item_class.ITEM_CLASS_ABSOLUTE)
                        {
                            // non-zero absolute values stick
                            if (itemclass == input_item_class.ITEM_CLASS_ABSOLUTE)
                                result += value;
                            else
                                result = value;

                            itemclass = input_item_class.ITEM_CLASS_ABSOLUTE;
                        }
                        else if (code.item_class() == input_item_class.ITEM_CLASS_RELATIVE)
                        {
                            // non-zero relative values accumulate in the absence of absolute values
                            if (itemclass != input_item_class.ITEM_CLASS_ABSOLUTE)
                            {
                                result += value;
                                itemclass = input_item_class.ITEM_CLASS_RELATIVE;
                            }
                        }
                    }

                    // clear the invert flag
                    invert = false;
                }
            }

            // saturate mixed absolute values
            if (itemclass == input_item_class.ITEM_CLASS_ABSOLUTE)
                result = std.clamp(result, INPUT_ABSOLUTE_MIN, INPUT_ABSOLUTE_MAX);

            // if the caller wants to know the type, provide it
            if (result == 0)
                itemclass = itemclasszero;

            return result;
        }


        // input sequence helpers

        //-------------------------------------------------
        //  seq_clean - clean the sequence, removing
        //  any invalid bits
        //-------------------------------------------------
        input_seq seq_clean(input_seq seq)
        {
            // make a copy of our sequence, removing any invalid bits
            input_seq clean_codes = new input_seq();
            int clean_index = 0;

            for (int codenum = 0; seq[codenum] != input_seq.end_code; codenum++)
            {
                // if this is a code item which is not valid, don't copy it and remove any preceding ORs/NOTs
                input_code code = seq[codenum];
                if (!code.internal_() && (((code.device_index() > 0) && !m_class[(int)code.device_class()].multi()) || item_from_code(code) == null))
                {
                    while (clean_index > 0 && clean_codes[clean_index - 1].internal_())
                    {
                        clean_codes.backspace();
                        clean_index--;
                    }
                }
                else if (clean_index > 0 || !code.internal_() || code == input_seq.not_code)
                {
                    clean_codes.append_code_to_sequence_plus(code);  //clean_codes += code;
                    clean_index++;
                }
            }

            return clean_codes;
        }


        //-------------------------------------------------
        //  seq_name - generate the friendly name of a
        //  sequence
        //-------------------------------------------------
        public string seq_name(input_seq seq)
        {
            // make a copy of our sequence, removing any invalid bits
            input_seq cleaned_seq = seq_clean(seq);

            // special case: empty
            if (cleaned_seq[0] == input_seq.end_code)
                return seq.empty() ? "None" : "n/a";

            // start with an empty buffer
            string str = "";

            // loop until we hit the end
            for (int codenum = 0; cleaned_seq[codenum] != input_seq.end_code; codenum++)
            {
                // append a space if not the first code
                if (codenum != 0)
                    str = str.append_(" ");

                // handle OR/NOT codes here
                input_code code = cleaned_seq[codenum];
                if (code == input_seq.or_code)
                    str = str.append_("or");
                else if (code == input_seq.not_code)
                    str = str.append_("not");

                // otherwise, assume it is an input code and ask the input system to generate it
                else
                    str = str.append_(code_name(code));
            }

            return str;
        }


        //string seq_to_tokens(const input_seq &seq) const;
        //void seq_from_tokens(input_seq &seq, const char *_token);


        // misc
        //bool map_device_to_controller(const devicemap_table &table);


        // internal helpers

        //-------------------------------------------------
        //  reset_memory - reset the array of memory for
        //  pressed switches
        //-------------------------------------------------
        void reset_memory()
        {
            // reset all entries in switch memory to invalid
            for (int memnum = 0; memnum < m_switch_memory.Length; memnum++)
                m_switch_memory[memnum] = INPUT_CODE_INVALID;
        }
    }


    // ======================> code_string_table
    // simple class to match codes to strings
    public struct code_string_table
    {
        public u32 m_code;
        public string m_string;

        public code_string_table(u32 code, string string_) { m_code = code; m_string = string_; }

#if false
        u32 operator[](const char *string) const
        {
            for (const code_string_table *current = this; current->m_code != ~0; current++)
                if (strcmp(current->m_string, string) == 0)
                    return current->m_code;
            return ~0;
        }

        const char *operator[](u32 code) const
        {
            for (const code_string_table *current = this; current->m_code != ~0; current++)
                if (current->m_code == code)
                    return current->m_string;
            return nullptr;
        }
#endif

        public static string Find(code_string_table [] table, u32 code) { return Array.Find<code_string_table>(table, x => x.m_code == code).m_string; }
        public static u32 Find(code_string_table [] table, string str) { return Array.Find<code_string_table>(table, x => x.m_string == str).m_code; }
    }


    static class input_internal
    {
        // token strings for device classes
        public static readonly code_string_table [] devclass_token_table = 
        {
            new code_string_table((u32)input_device_class.DEVICE_CLASS_KEYBOARD, "KEYCODE"),
            new code_string_table((u32)input_device_class.DEVICE_CLASS_MOUSE,    "MOUSECODE"),
            new code_string_table((u32)input_device_class.DEVICE_CLASS_LIGHTGUN, "GUNCODE"),
            new code_string_table((u32)input_device_class.DEVICE_CLASS_JOYSTICK, "JOYCODE"),
            new code_string_table(u32.MaxValue,                                  "UNKCODE")
        };

        // friendly strings for device classes
        public static readonly code_string_table [] devclass_string_table = 
        {
            new code_string_table((u32)input_device_class.DEVICE_CLASS_KEYBOARD, "Kbd"),
            new code_string_table((u32)input_device_class.DEVICE_CLASS_MOUSE,    "Mouse"),
            new code_string_table((u32)input_device_class.DEVICE_CLASS_LIGHTGUN, "Gun"),
            new code_string_table((u32)input_device_class.DEVICE_CLASS_JOYSTICK, "Joy"),
            new code_string_table(u32.MaxValue,                                  "Unk")
        };

        // token strings for item modifiers
        public static readonly code_string_table [] modifier_token_table = 
        {
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_REVERSE, "REVERSE"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_POS,     "POS"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_NEG,     "NEG"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_LEFT,    "LEFT"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_RIGHT,   "RIGHT"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_UP,      "UP"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_DOWN,    "DOWN"),
            new code_string_table(u32.MaxValue,                                   "")
        };

        // friendly strings for item modifiers
        public static readonly code_string_table [] modifier_string_table = 
        {
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_REVERSE, "Reverse"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_POS,     "+"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_NEG,     "-"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_LEFT,    "Left"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_RIGHT,   "Right"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_UP,      "Up"),
            new code_string_table((u32)input_item_modifier.ITEM_MODIFIER_DOWN,    "Down"),
            new code_string_table(u32.MaxValue,                                   "")
        };

        // token strings for item classes
        public static readonly code_string_table [] itemclass_token_table = 
        {
            new code_string_table((u32)input_item_class.ITEM_CLASS_SWITCH,   "SWITCH"),
            new code_string_table((u32)input_item_class.ITEM_CLASS_ABSOLUTE, "ABSOLUTE"),
            new code_string_table((u32)input_item_class.ITEM_CLASS_RELATIVE, "RELATIVE"),
            new code_string_table(u32.MaxValue,                              "")
        };

        // token strings for standard item ids
        public static readonly code_string_table [] itemid_token_table = 
        {
            // standard keyboard codes
            new code_string_table((u32)input_item_id.ITEM_ID_A,              "A"),
            new code_string_table((u32)input_item_id.ITEM_ID_B,              "B"),
            new code_string_table((u32)input_item_id.ITEM_ID_C,              "C"),
            new code_string_table((u32)input_item_id.ITEM_ID_D,              "D"),
            new code_string_table((u32)input_item_id.ITEM_ID_E,              "E"),
            new code_string_table((u32)input_item_id.ITEM_ID_F,              "F"),
            new code_string_table((u32)input_item_id.ITEM_ID_G,              "G"),
            new code_string_table((u32)input_item_id.ITEM_ID_H,              "H"),
            new code_string_table((u32)input_item_id.ITEM_ID_I,              "I"),
            new code_string_table((u32)input_item_id.ITEM_ID_J,              "J"),
            new code_string_table((u32)input_item_id.ITEM_ID_K,              "K"),
            new code_string_table((u32)input_item_id.ITEM_ID_L,              "L"),
            new code_string_table((u32)input_item_id.ITEM_ID_M,              "M"),
            new code_string_table((u32)input_item_id.ITEM_ID_N,              "N"),
            new code_string_table((u32)input_item_id.ITEM_ID_O,              "O"),
            new code_string_table((u32)input_item_id.ITEM_ID_P,              "P"),
            new code_string_table((u32)input_item_id.ITEM_ID_Q,              "Q"),
            new code_string_table((u32)input_item_id.ITEM_ID_R,              "R"),
            new code_string_table((u32)input_item_id.ITEM_ID_S,              "S"),
            new code_string_table((u32)input_item_id.ITEM_ID_T,              "T"),
            new code_string_table((u32)input_item_id.ITEM_ID_U,              "U"),
            new code_string_table((u32)input_item_id.ITEM_ID_V,              "V"),
            new code_string_table((u32)input_item_id.ITEM_ID_W,              "W"),
            new code_string_table((u32)input_item_id.ITEM_ID_X,              "X"),
            new code_string_table((u32)input_item_id.ITEM_ID_Y,              "Y"),
            new code_string_table((u32)input_item_id.ITEM_ID_Z,              "Z"),
            new code_string_table((u32)input_item_id.ITEM_ID_0,              "0"),
            new code_string_table((u32)input_item_id.ITEM_ID_1,              "1"),
            new code_string_table((u32)input_item_id.ITEM_ID_2,              "2"),
            new code_string_table((u32)input_item_id.ITEM_ID_3,              "3"),
            new code_string_table((u32)input_item_id.ITEM_ID_4,              "4"),
            new code_string_table((u32)input_item_id.ITEM_ID_5,              "5"),
            new code_string_table((u32)input_item_id.ITEM_ID_6,              "6"),
            new code_string_table((u32)input_item_id.ITEM_ID_7,              "7"),
            new code_string_table((u32)input_item_id.ITEM_ID_8,              "8"),
            new code_string_table((u32)input_item_id.ITEM_ID_9,              "9"),
            new code_string_table((u32)input_item_id.ITEM_ID_F1,             "F1"),
            new code_string_table((u32)input_item_id.ITEM_ID_F2,             "F2"),
            new code_string_table((u32)input_item_id.ITEM_ID_F3,             "F3"),
            new code_string_table((u32)input_item_id.ITEM_ID_F4,             "F4"),
            new code_string_table((u32)input_item_id.ITEM_ID_F5,             "F5"),
            new code_string_table((u32)input_item_id.ITEM_ID_F6,             "F6"),
            new code_string_table((u32)input_item_id.ITEM_ID_F7,             "F7"),
            new code_string_table((u32)input_item_id.ITEM_ID_F8,             "F8"),
            new code_string_table((u32)input_item_id.ITEM_ID_F9,             "F9"),
            new code_string_table((u32)input_item_id.ITEM_ID_F10,            "F10"),
            new code_string_table((u32)input_item_id.ITEM_ID_F11,            "F11"),
            new code_string_table((u32)input_item_id.ITEM_ID_F12,            "F12"),
            new code_string_table((u32)input_item_id.ITEM_ID_F13,            "F13"),
            new code_string_table((u32)input_item_id.ITEM_ID_F14,            "F14"),
            new code_string_table((u32)input_item_id.ITEM_ID_F15,            "F15"),
            new code_string_table((u32)input_item_id.ITEM_ID_F16,            "F16"),
            new code_string_table((u32)input_item_id.ITEM_ID_F17,            "F17"),
            new code_string_table((u32)input_item_id.ITEM_ID_F18,            "F18"),
            new code_string_table((u32)input_item_id.ITEM_ID_F19,            "F19"),
            new code_string_table((u32)input_item_id.ITEM_ID_F20,            "F20"),
            new code_string_table((u32)input_item_id.ITEM_ID_ESC,            "ESC"),
            new code_string_table((u32)input_item_id.ITEM_ID_TILDE,          "TILDE"),
            new code_string_table((u32)input_item_id.ITEM_ID_MINUS,          "MINUS"),
            new code_string_table((u32)input_item_id.ITEM_ID_EQUALS,         "EQUALS"),
            new code_string_table((u32)input_item_id.ITEM_ID_BACKSPACE,      "BACKSPACE"),
            new code_string_table((u32)input_item_id.ITEM_ID_TAB,            "TAB"),
            new code_string_table((u32)input_item_id.ITEM_ID_OPENBRACE,      "OPENBRACE"),
            new code_string_table((u32)input_item_id.ITEM_ID_CLOSEBRACE,     "CLOSEBRACE"),
            new code_string_table((u32)input_item_id.ITEM_ID_ENTER,          "ENTER"),
            new code_string_table((u32)input_item_id.ITEM_ID_COLON,          "COLON"),
            new code_string_table((u32)input_item_id.ITEM_ID_QUOTE,          "QUOTE"),
            new code_string_table((u32)input_item_id.ITEM_ID_BACKSLASH,      "BACKSLASH"),
            new code_string_table((u32)input_item_id.ITEM_ID_BACKSLASH2,     "BACKSLASH2"),
            new code_string_table((u32)input_item_id.ITEM_ID_COMMA,          "COMMA"),
            new code_string_table((u32)input_item_id.ITEM_ID_STOP,           "STOP"),
            new code_string_table((u32)input_item_id.ITEM_ID_SLASH,          "SLASH"),
            new code_string_table((u32)input_item_id.ITEM_ID_SPACE,          "SPACE"),
            new code_string_table((u32)input_item_id.ITEM_ID_INSERT,         "INSERT"),
            new code_string_table((u32)input_item_id.ITEM_ID_DEL,            "DEL"),
            new code_string_table((u32)input_item_id.ITEM_ID_HOME,           "HOME"),
            new code_string_table((u32)input_item_id.ITEM_ID_END,            "END"),
            new code_string_table((u32)input_item_id.ITEM_ID_PGUP,           "PGUP"),
            new code_string_table((u32)input_item_id.ITEM_ID_PGDN,           "PGDN"),
            new code_string_table((u32)input_item_id.ITEM_ID_LEFT,           "LEFT"),
            new code_string_table((u32)input_item_id.ITEM_ID_RIGHT,          "RIGHT"),
            new code_string_table((u32)input_item_id.ITEM_ID_UP,             "UP"),
            new code_string_table((u32)input_item_id.ITEM_ID_DOWN,           "DOWN"),
            new code_string_table((u32)input_item_id.ITEM_ID_0_PAD,          "0PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_1_PAD,          "1PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_2_PAD,          "2PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_3_PAD,          "3PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_4_PAD,          "4PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_5_PAD,          "5PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_6_PAD,          "6PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_7_PAD,          "7PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_8_PAD,          "8PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_9_PAD,          "9PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_SLASH_PAD,      "SLASHPAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_ASTERISK,       "ASTERISK"),
            new code_string_table((u32)input_item_id.ITEM_ID_MINUS_PAD,      "MINUSPAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_PLUS_PAD,       "PLUSPAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_DEL_PAD,        "DELPAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_ENTER_PAD,      "ENTERPAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_BS_PAD,         "BSPAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_TAB_PAD,        "TABPAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_00_PAD,         "00PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_000_PAD,        "000PAD"),
            new code_string_table((u32)input_item_id.ITEM_ID_PRTSCR,         "PRTSCR"),
            new code_string_table((u32)input_item_id.ITEM_ID_PAUSE,          "PAUSE"),
            new code_string_table((u32)input_item_id.ITEM_ID_LSHIFT,         "LSHIFT"),
            new code_string_table((u32)input_item_id.ITEM_ID_RSHIFT,         "RSHIFT"),
            new code_string_table((u32)input_item_id.ITEM_ID_LCONTROL,       "LCONTROL"),
            new code_string_table((u32)input_item_id.ITEM_ID_RCONTROL,       "RCONTROL"),
            new code_string_table((u32)input_item_id.ITEM_ID_LALT,           "LALT"),
            new code_string_table((u32)input_item_id.ITEM_ID_RALT,           "RALT"),
            new code_string_table((u32)input_item_id.ITEM_ID_SCRLOCK,        "SCRLOCK"),
            new code_string_table((u32)input_item_id.ITEM_ID_NUMLOCK,        "NUMLOCK"),
            new code_string_table((u32)input_item_id.ITEM_ID_CAPSLOCK,       "CAPSLOCK"),
            new code_string_table((u32)input_item_id.ITEM_ID_LWIN,           "LWIN"),
            new code_string_table((u32)input_item_id.ITEM_ID_RWIN,           "RWIN"),
            new code_string_table((u32)input_item_id.ITEM_ID_MENU,           "MENU"),
            new code_string_table((u32)input_item_id.ITEM_ID_CANCEL,         "CANCEL"),

            // standard mouse/joystick/gun codes
            new code_string_table((u32)input_item_id.ITEM_ID_XAXIS,          "XAXIS"),
            new code_string_table((u32)input_item_id.ITEM_ID_YAXIS,          "YAXIS"),
            new code_string_table((u32)input_item_id.ITEM_ID_ZAXIS,          "ZAXIS"),
            new code_string_table((u32)input_item_id.ITEM_ID_RXAXIS,         "RXAXIS"),
            new code_string_table((u32)input_item_id.ITEM_ID_RYAXIS,         "RYAXIS"),
            new code_string_table((u32)input_item_id.ITEM_ID_RZAXIS,         "RZAXIS"),
            new code_string_table((u32)input_item_id.ITEM_ID_SLIDER1,        "SLIDER1"),
            new code_string_table((u32)input_item_id.ITEM_ID_SLIDER2,        "SLIDER2"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON1,        "BUTTON1"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON2,        "BUTTON2"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON3,        "BUTTON3"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON4,        "BUTTON4"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON5,        "BUTTON5"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON6,        "BUTTON6"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON7,        "BUTTON7"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON8,        "BUTTON8"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON9,        "BUTTON9"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON10,       "BUTTON10"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON11,       "BUTTON11"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON12,       "BUTTON12"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON13,       "BUTTON13"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON14,       "BUTTON14"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON15,       "BUTTON15"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON16,       "BUTTON16"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON17,       "BUTTON17"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON18,       "BUTTON18"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON19,       "BUTTON19"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON20,       "BUTTON20"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON21,       "BUTTON21"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON22,       "BUTTON22"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON23,       "BUTTON23"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON24,       "BUTTON24"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON25,       "BUTTON25"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON26,       "BUTTON26"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON27,       "BUTTON27"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON28,       "BUTTON28"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON29,       "BUTTON29"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON30,       "BUTTON30"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON31,       "BUTTON31"),
            new code_string_table((u32)input_item_id.ITEM_ID_BUTTON32,       "BUTTON32"),
            new code_string_table((u32)input_item_id.ITEM_ID_START,          "START"),
            new code_string_table((u32)input_item_id.ITEM_ID_SELECT,         "SELECT"),

            // Hats
            new code_string_table((u32)input_item_id.ITEM_ID_HAT1UP,         "HAT1UP"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT1DOWN,       "HAT1DOWN"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT1LEFT,       "HAT1LEFT"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT1RIGHT,      "HAT1RIGHT"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT2UP,         "HAT2UP"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT2DOWN,       "HAT2DOWN"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT2LEFT,       "HAT2LEFT"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT2RIGHT,      "HAT2RIGHT"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT3UP,         "HAT3UP"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT3DOWN,       "HAT3DOWN"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT3LEFT,       "HAT3LEFT"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT3RIGHT,      "HAT3RIGHT"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT4UP,         "HAT4UP"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT4DOWN,       "HAT4DOWN"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT4LEFT,       "HAT4LEFT"),
            new code_string_table((u32)input_item_id.ITEM_ID_HAT4RIGHT,      "HAT4RIGHT"),

            // Additional IDs
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH1,    "ADDSW1"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH2,    "ADDSW2"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH3,    "ADDSW3"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH4,    "ADDSW4"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH5,    "ADDSW5"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH6,    "ADDSW6"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH7,    "ADDSW7"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH8,    "ADDSW8"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH9,    "ADDSW9"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH10,   "ADDSW10"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH11,   "ADDSW11"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH12,   "ADDSW12"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH13,   "ADDSW13"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH14,   "ADDSW14"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH15,   "ADDSW15"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_SWITCH16,   "ADDSW16"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE1,  "ADDAXIS1"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE2,  "ADDAXIS2"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE3,  "ADDAXIS3"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE4,  "ADDAXIS4"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE5,  "ADDAXIS5"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE6,  "ADDAXIS6"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE7,  "ADDAXIS7"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE8,  "ADDAXIS8"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE9,  "ADDAXIS9"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE10, "ADDAXIS10"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE11, "ADDAXIS11"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE12, "ADDAXIS12"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE13, "ADDAXIS13"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE14, "ADDAXIS14"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE15, "ADDAXIS15"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_ABSOLUTE16, "ADDAXIS16"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE1,  "ADDREL1"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE2,  "ADDREL2"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE3,  "ADDREL3"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE4,  "ADDREL4"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE5,  "ADDREL5"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE6,  "ADDREL6"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE7,  "ADDREL7"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE8,  "ADDREL8"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE9,  "ADDREL9"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE10, "ADDREL10"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE11, "ADDREL11"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE12, "ADDREL12"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE13, "ADDREL13"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE14, "ADDREL14"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE15, "ADDREL15"),
            new code_string_table((u32)input_item_id.ITEM_ID_ADD_RELATIVE16, "ADDREL16"),

            new code_string_table(u32.MaxValue,                              null)
        };
    }
}
