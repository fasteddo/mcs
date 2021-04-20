// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using size_t = System.UInt32;
using size_t_constant_16 = mame.uint32_constant_16;
using u32 = System.UInt32;


namespace mame
{
    // callback for getting the value of an item on a device
    //typedef INT32 (*item_get_state_func)(void *device_internal, void *item_internal);
    public delegate int item_get_state_func(object device_internal, object item_internal);


    // device classes
    public enum input_device_class
    {
        DEVICE_CLASS_INVALID,
        DEVICE_CLASS_FIRST_VALID,
        DEVICE_CLASS_KEYBOARD = DEVICE_CLASS_FIRST_VALID,
        DEVICE_CLASS_MOUSE,
        DEVICE_CLASS_LIGHTGUN,
        DEVICE_CLASS_JOYSTICK,
        DEVICE_CLASS_LAST_VALID = DEVICE_CLASS_JOYSTICK,
        DEVICE_CLASS_INTERNAL,
        DEVICE_CLASS_MAXIMUM
    }
    //DECLARE_ENUM_INCDEC_OPERATORS(input_device_class)


    // input item classes
    public enum input_item_class
    {
        ITEM_CLASS_INVALID,
        ITEM_CLASS_SWITCH,
        ITEM_CLASS_ABSOLUTE,
        ITEM_CLASS_RELATIVE,
        ITEM_CLASS_MAXIMUM
    }


    // input item modifiers
    public enum input_item_modifier
    {
        ITEM_MODIFIER_NONE,
        ITEM_MODIFIER_POS,
        ITEM_MODIFIER_NEG,
        ITEM_MODIFIER_LEFT,
        ITEM_MODIFIER_RIGHT,
        ITEM_MODIFIER_UP,
        ITEM_MODIFIER_DOWN,
        ITEM_MODIFIER_MAXIMUM
    }


    // standard item IDs
    public enum input_item_id
    {
        ITEM_ID_INVALID,
        ITEM_ID_FIRST_VALID,

        // standard keyboard IDs
        ITEM_ID_A = ITEM_ID_FIRST_VALID,
        ITEM_ID_B,
        ITEM_ID_C,
        ITEM_ID_D,
        ITEM_ID_E,
        ITEM_ID_F,
        ITEM_ID_G,
        ITEM_ID_H,
        ITEM_ID_I,
        ITEM_ID_J,
        ITEM_ID_K,
        ITEM_ID_L,
        ITEM_ID_M,
        ITEM_ID_N,
        ITEM_ID_O,
        ITEM_ID_P,
        ITEM_ID_Q,
        ITEM_ID_R,
        ITEM_ID_S,
        ITEM_ID_T,
        ITEM_ID_U,
        ITEM_ID_V,
        ITEM_ID_W,
        ITEM_ID_X,
        ITEM_ID_Y,
        ITEM_ID_Z,
        ITEM_ID_0,
        ITEM_ID_1,
        ITEM_ID_2,
        ITEM_ID_3,
        ITEM_ID_4,
        ITEM_ID_5,
        ITEM_ID_6,
        ITEM_ID_7,
        ITEM_ID_8,
        ITEM_ID_9,
        ITEM_ID_F1,
        ITEM_ID_F2,
        ITEM_ID_F3,
        ITEM_ID_F4,
        ITEM_ID_F5,
        ITEM_ID_F6,
        ITEM_ID_F7,
        ITEM_ID_F8,
        ITEM_ID_F9,
        ITEM_ID_F10,
        ITEM_ID_F11,
        ITEM_ID_F12,
        ITEM_ID_F13,
        ITEM_ID_F14,
        ITEM_ID_F15,
        ITEM_ID_F16,
        ITEM_ID_F17,
        ITEM_ID_F18,
        ITEM_ID_F19,
        ITEM_ID_F20,
        ITEM_ID_ESC,
        ITEM_ID_TILDE,
        ITEM_ID_MINUS,
        ITEM_ID_EQUALS,
        ITEM_ID_BACKSPACE,
        ITEM_ID_TAB,
        ITEM_ID_OPENBRACE,
        ITEM_ID_CLOSEBRACE,
        ITEM_ID_ENTER,
        ITEM_ID_COLON,
        ITEM_ID_QUOTE,
        ITEM_ID_BACKSLASH,
        ITEM_ID_BACKSLASH2,
        ITEM_ID_COMMA,
        ITEM_ID_STOP,
        ITEM_ID_SLASH,
        ITEM_ID_SPACE,
        ITEM_ID_INSERT,
        ITEM_ID_DEL,
        ITEM_ID_HOME,
        ITEM_ID_END,
        ITEM_ID_PGUP,
        ITEM_ID_PGDN,
        ITEM_ID_LEFT,
        ITEM_ID_RIGHT,
        ITEM_ID_UP,
        ITEM_ID_DOWN,
        ITEM_ID_0_PAD,
        ITEM_ID_1_PAD,
        ITEM_ID_2_PAD,
        ITEM_ID_3_PAD,
        ITEM_ID_4_PAD,
        ITEM_ID_5_PAD,
        ITEM_ID_6_PAD,
        ITEM_ID_7_PAD,
        ITEM_ID_8_PAD,
        ITEM_ID_9_PAD,
        ITEM_ID_SLASH_PAD,
        ITEM_ID_ASTERISK,
        ITEM_ID_MINUS_PAD,
        ITEM_ID_PLUS_PAD,
        ITEM_ID_DEL_PAD,
        ITEM_ID_ENTER_PAD,
        ITEM_ID_BS_PAD,
        ITEM_ID_TAB_PAD,
        ITEM_ID_00_PAD,
        ITEM_ID_000_PAD,
        ITEM_ID_COMMA_PAD,
        ITEM_ID_EQUALS_PAD,
        ITEM_ID_PRTSCR,
        ITEM_ID_PAUSE,
        ITEM_ID_LSHIFT,
        ITEM_ID_RSHIFT,
        ITEM_ID_LCONTROL,
        ITEM_ID_RCONTROL,
        ITEM_ID_LALT,
        ITEM_ID_RALT,
        ITEM_ID_SCRLOCK,
        ITEM_ID_NUMLOCK,
        ITEM_ID_CAPSLOCK,
        ITEM_ID_LWIN,
        ITEM_ID_RWIN,
        ITEM_ID_MENU,
        ITEM_ID_CANCEL,

        // standard mouse/joystick/gun IDs
        ITEM_ID_XAXIS,
        ITEM_ID_YAXIS,
        ITEM_ID_ZAXIS,
        ITEM_ID_RXAXIS,
        ITEM_ID_RYAXIS,
        ITEM_ID_RZAXIS,
        ITEM_ID_SLIDER1,
        ITEM_ID_SLIDER2,
        ITEM_ID_BUTTON1,
        ITEM_ID_BUTTON2,
        ITEM_ID_BUTTON3,
        ITEM_ID_BUTTON4,
        ITEM_ID_BUTTON5,
        ITEM_ID_BUTTON6,
        ITEM_ID_BUTTON7,
        ITEM_ID_BUTTON8,
        ITEM_ID_BUTTON9,
        ITEM_ID_BUTTON10,
        ITEM_ID_BUTTON11,
        ITEM_ID_BUTTON12,
        ITEM_ID_BUTTON13,
        ITEM_ID_BUTTON14,
        ITEM_ID_BUTTON15,
        ITEM_ID_BUTTON16,
        ITEM_ID_BUTTON17,
        ITEM_ID_BUTTON18,
        ITEM_ID_BUTTON19,
        ITEM_ID_BUTTON20,
        ITEM_ID_BUTTON21,
        ITEM_ID_BUTTON22,
        ITEM_ID_BUTTON23,
        ITEM_ID_BUTTON24,
        ITEM_ID_BUTTON25,
        ITEM_ID_BUTTON26,
        ITEM_ID_BUTTON27,
        ITEM_ID_BUTTON28,
        ITEM_ID_BUTTON29,
        ITEM_ID_BUTTON30,
        ITEM_ID_BUTTON31,
        ITEM_ID_BUTTON32,
        ITEM_ID_START,
        ITEM_ID_SELECT,

        // Hats
        ITEM_ID_HAT1UP,
        ITEM_ID_HAT1DOWN,
        ITEM_ID_HAT1LEFT,
        ITEM_ID_HAT1RIGHT,
        ITEM_ID_HAT2UP,
        ITEM_ID_HAT2DOWN,
        ITEM_ID_HAT2LEFT,
        ITEM_ID_HAT2RIGHT,
        ITEM_ID_HAT3UP,
        ITEM_ID_HAT3DOWN,
        ITEM_ID_HAT3LEFT,
        ITEM_ID_HAT3RIGHT,
        ITEM_ID_HAT4UP,
        ITEM_ID_HAT4DOWN,
        ITEM_ID_HAT4LEFT,
        ITEM_ID_HAT4RIGHT,

        // Additional IDs
        ITEM_ID_ADD_SWITCH1,
        ITEM_ID_ADD_SWITCH2,
        ITEM_ID_ADD_SWITCH3,
        ITEM_ID_ADD_SWITCH4,
        ITEM_ID_ADD_SWITCH5,
        ITEM_ID_ADD_SWITCH6,
        ITEM_ID_ADD_SWITCH7,
        ITEM_ID_ADD_SWITCH8,
        ITEM_ID_ADD_SWITCH9,
        ITEM_ID_ADD_SWITCH10,
        ITEM_ID_ADD_SWITCH11,
        ITEM_ID_ADD_SWITCH12,
        ITEM_ID_ADD_SWITCH13,
        ITEM_ID_ADD_SWITCH14,
        ITEM_ID_ADD_SWITCH15,
        ITEM_ID_ADD_SWITCH16,

        ITEM_ID_ADD_ABSOLUTE1,
        ITEM_ID_ADD_ABSOLUTE2,
        ITEM_ID_ADD_ABSOLUTE3,
        ITEM_ID_ADD_ABSOLUTE4,
        ITEM_ID_ADD_ABSOLUTE5,
        ITEM_ID_ADD_ABSOLUTE6,
        ITEM_ID_ADD_ABSOLUTE7,
        ITEM_ID_ADD_ABSOLUTE8,
        ITEM_ID_ADD_ABSOLUTE9,
        ITEM_ID_ADD_ABSOLUTE10,
        ITEM_ID_ADD_ABSOLUTE11,
        ITEM_ID_ADD_ABSOLUTE12,
        ITEM_ID_ADD_ABSOLUTE13,
        ITEM_ID_ADD_ABSOLUTE14,
        ITEM_ID_ADD_ABSOLUTE15,
        ITEM_ID_ADD_ABSOLUTE16,

        ITEM_ID_ADD_RELATIVE1,
        ITEM_ID_ADD_RELATIVE2,
        ITEM_ID_ADD_RELATIVE3,
        ITEM_ID_ADD_RELATIVE4,
        ITEM_ID_ADD_RELATIVE5,
        ITEM_ID_ADD_RELATIVE6,
        ITEM_ID_ADD_RELATIVE7,
        ITEM_ID_ADD_RELATIVE8,
        ITEM_ID_ADD_RELATIVE9,
        ITEM_ID_ADD_RELATIVE10,
        ITEM_ID_ADD_RELATIVE11,
        ITEM_ID_ADD_RELATIVE12,
        ITEM_ID_ADD_RELATIVE13,
        ITEM_ID_ADD_RELATIVE14,
        ITEM_ID_ADD_RELATIVE15,
        ITEM_ID_ADD_RELATIVE16,

        // generic other IDs
        ITEM_ID_OTHER_SWITCH,
        ITEM_ID_OTHER_AXIS_ABSOLUTE,
        ITEM_ID_OTHER_AXIS_RELATIVE,
        ITEM_ID_MAXIMUM,

        // internal codes for sequences
        ITEM_ID_SEQ_END,
        ITEM_ID_SEQ_DEFAULT,
        ITEM_ID_SEQ_NOT,
        ITEM_ID_SEQ_OR,

        // absolute maximum ID
        ITEM_ID_ABSOLUTE_MAXIMUM = 0xfff
    }
    //DECLARE_ENUM_INCDEC_OPERATORS(input_item_id)


    // ======================> input_code
    // a combined code that describes a particular input on a particular device
    public class input_code : global_object
    {
        public static readonly input_code INPUT_CODE_INVALID = new input_code();


        u32 m_internal;


        // construction/destruction
        public input_code(
            input_device_class devclass = input_device_class.DEVICE_CLASS_INVALID,
            int devindex = 0,
            input_item_class itemclass = input_item_class.ITEM_CLASS_INVALID,
            input_item_modifier modifier = input_item_modifier.ITEM_MODIFIER_NONE,
            input_item_id itemid = input_item_id.ITEM_ID_INVALID)
        {
            m_internal = ((((UInt32)devclass & 0xf) << 28) | (((UInt32)devindex & 0xff) << 20) | (((UInt32)itemclass & 0xf) << 16) | (((UInt32)modifier & 0xf) << 12) | ((UInt32)itemid & 0xfff));

            assert(devclass >= 0 && devclass < input_device_class.DEVICE_CLASS_MAXIMUM);
            assert(devindex >= 0 && devindex < input_global.DEVICE_INDEX_MAXIMUM);
            assert(itemclass >= 0 && itemclass < input_item_class.ITEM_CLASS_MAXIMUM);
            assert(modifier >= 0 && modifier < input_item_modifier.ITEM_MODIFIER_MAXIMUM);
            assert(itemid >= 0 && itemid < input_item_id.ITEM_ID_ABSOLUTE_MAXIMUM);
        }

        //public input_code(input_code src) { m_internal = src.m_internal; }


        // operators
        public static bool operator ==(input_code left, input_code right) { return left.m_internal == right.m_internal; }
        public static bool operator !=(input_code left, input_code right) { return left.m_internal != right.m_internal; }


        // getters
        public bool internal_get() { return device_class() == input_device_class.DEVICE_CLASS_INTERNAL; }
        public input_device_class device_class() { return (input_device_class)((m_internal >> 28) & 0xf); }
        public int device_index() { return (int)((m_internal >> 20) & 0xff); }
        public input_item_class item_class() { return (input_item_class)((m_internal >> 16) & 0xf); }
        public input_item_modifier item_modifier() { return (input_item_modifier)((m_internal >> 12) & 0xf); }
        public input_item_id item_id() { return (input_item_id)(m_internal & 0xfff); }


        // setters
        //void set_device_class(input_device_class devclass) noexcept
        //{
        //    assert(devclass >= 0 && devclass <= 0xf);
        //    m_internal = (m_internal & ~(0xf << 28)) | ((devclass & 0xf) << 28);
        //}
        public void set_device_index(int devindex)
        {
            assert(devindex >= 0 && (UInt32)devindex <= 0xff);
            m_internal = (UInt32)((m_internal & ~(0xff << 20)) | (((UInt32)devindex & 0xff) << 20));
        }
        public void set_item_class(input_item_class itemclass)
        {
            assert(itemclass >= 0 && (UInt32)itemclass <= 0xf);
            m_internal = (UInt32)((m_internal & ~(0xf << 16)) | (((UInt32)itemclass & 0xf) << 16));
        }
        public void set_item_modifier(input_item_modifier modifier)
        {
            assert(modifier >= 0 && (UInt32)modifier <= 0xf);
            m_internal = (UInt32)((m_internal & ~(0xf << 12)) | (((UInt32)modifier & 0xf) << 12));
        }
        //void set_item_id(input_item_id itemid) noexcept
        //{
        //    assert(itemid >= 0 && itemid <= 0xfff);
        //    m_internal = (m_internal & ~0xfff) | (itemid & 0xfff);
        //}
    }


    // ======================> input_seq
    // a sequence of input_codes, supporting AND/OR and inversion
    public class input_seq : global_object
    {
        // constant codes used in sequences
        public static readonly input_code end_code = new input_code(input_device_class.DEVICE_CLASS_INTERNAL,     0, input_item_class.ITEM_CLASS_INVALID, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SEQ_END);
        public static readonly input_code default_code = new input_code(input_device_class.DEVICE_CLASS_INTERNAL, 0, input_item_class.ITEM_CLASS_INVALID, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SEQ_DEFAULT);
        public static readonly input_code not_code = new input_code(input_device_class.DEVICE_CLASS_INTERNAL,     0, input_item_class.ITEM_CLASS_INVALID, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SEQ_NOT);
        public static readonly input_code or_code = new input_code(input_device_class.DEVICE_CLASS_INTERNAL,      0, input_item_class.ITEM_CLASS_INVALID, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SEQ_OR);

        // constant sequences
        public static readonly input_seq empty_seq = new input_seq();


        // internal state
        std.array<input_code, size_t_constant_16> m_code = new std.array<input_code, size_t_constant_16>();


        // construction/destruction
        //input_seq() noexcept : input_seq(std::make_index_sequence<std::tuple_size<decltype(m_code)>::value>()) { }
        //template <typename... T> input_seq(input_code code_0, T... code_n) noexcept : input_seq(std::make_index_sequence<std::tuple_size<decltype(m_code)>::value - sizeof...(T) - 1>(), code_0, code_n...) { }
        //constexpr input_seq(const input_seq &rhs) noexcept = default;
        //template <size_t... N, typename... T> input_seq(std::integer_sequence<size_t, N...>, T... code) noexcept : m_code({ code..., get_end_code(N)... }) { }
        //template <size_t... N> input_seq(std::integer_sequence<size_t, N...>) noexcept : m_code({ get_end_code(N)... }) { }
        public input_seq(params input_code [] codes)
        {
            set(codes);
        }


        // operators
        public static bool operator ==(input_seq lhs, input_seq rhs) { return lhs.m_code == rhs.m_code; }
        public static bool operator !=(input_seq lhs, input_seq rhs) { return lhs.m_code != rhs.m_code; }
        public input_code this[int index] { get { return (index >= 0 && index < m_code.size()) ? m_code[index] : end_code; } }  //constexpr input_code operator[](int index) const noexcept { return (index >= 0 && index < m_code.size()) ? m_code[index] : end_code; }

        //input_seq &operator+=(input_code code) noexcept;
        //-------------------------------------------------
        //  operator+= - append a code to the end of an
        //  input sequence
        //-------------------------------------------------
        public input_seq append_code_to_sequence_plus(input_code code)
        {
            // if not enough room, return FALSE
            int curlength = length();
            if (curlength < m_code.size() - 1)
            {
                m_code[curlength++] = code;
                if ((curlength + 1) < m_code.size())
                    m_code[curlength + 1] = end_code;
            }

            return this;
        }

        //input_seq &operator|=(input_code code) noexcept;
        //-------------------------------------------------
        //  operator|= - append a code to a sequence; if
        //  the sequence is non-empty, insert an OR
        //  before the new code
        //-------------------------------------------------
        public input_seq append_code_to_sequence_or(input_code code)
        {
            // overwrite end/default with the new code
            if (m_code[0] == default_code)
            {
                m_code[0] = code;
                m_code[1] = end_code;
            }
            else
            {
                // otherwise, append an OR token and then the new code
                int curlength = length();
                if ((curlength + 1) < m_code.size())
                {
                    m_code[curlength] = or_code;
                    m_code[curlength + 1] = code;
                    if ((curlength + 2) < m_code.size())
                        m_code[curlength + 2] = end_code;
                }
            }

            return this;
        }


        // getters
        public bool empty() { return m_code[0] == end_code; }
        //constexpr int max_size() const noexcept { return std::tuple_size<decltype(m_code)>::value; }


        int length()
        {
            // find the end token; error if none found
            for (int seqnum = 0; seqnum < m_code.size(); seqnum++)
            {
                if (m_code[seqnum] == end_code)
                    return seqnum;
            }

            return m_code.size();
        }


        //bool is_valid() const noexcept;


        public bool is_default() { return m_code[0] == default_code; }


        // setters

        //template <typename... T> void set(input_code code_0, T... code_n) noexcept
        //{
        //    static_assert(sizeof...(T) < std::tuple_size<decltype(m_code)>::value, "too many codes for input_seq");
        //    set<0>(code_0, code_n...);
        //}
        void set(params input_code [] codes)
        {
            assert(codes.Length <= m_code.size(), "too many codes for input_seq");

            for (int i = 0; i < m_code.size(); i++)
                m_code[i] = i < codes.Length ? codes[i] : end_code;
        }


        //void reset() noexcept { set(end_code); }
        public void set_default() { var codes = new input_code[16]; std.fill(codes, end_code); codes[0] = default_code; set(codes); }  //void set_default() noexcept { set(default_code); }


        public void backspace()
        {
            // if we have at least one entry, remove it
            int curlength = length();
            if (curlength > 0)
                m_code[curlength - 1] = end_code;
        }


        //void replace(input_code oldcode, input_code newcode) noexcept;

        //static constexpr input_code get_end_code(size_t) noexcept { return end_code; }

        //template <unsigned N> void set() noexcept
        //{
        //    std::fill(std::next(m_code.begin(), N), m_code.end(), end_code);
        //}
        //template <unsigned N, typename... T> void set(input_code code_0, T... code_n) noexcept
        //{
        //    m_code[N] = code_0;
        //    set<N + 1>(code_n...);
        //}
    }


    public class size_t_constant_DEVICE_CLASS_MAXIMUM : uint32_constant { public UInt32 value { get { return (UInt32)input_device_class.DEVICE_CLASS_MAXIMUM; } } }


    // ======================> input_manager
    // global machine-level information about devices
    public class input_manager : global_object
    {
        // internal state
        running_machine m_machine;
        input_code [] m_switch_memory = new input_code[64];

        // classes
        std.array<input_class, size_t_constant_DEVICE_CLASS_MAXIMUM> m_class = new std.array<input_class, size_t_constant_DEVICE_CLASS_MAXIMUM>();  //std::array<std::unique_ptr<input_class>, DEVICE_CLASS_MAXIMUM> m_class;


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


        // getters
        public running_machine machine() { return m_machine; }
        public input_class device_class(input_device_class devclass) { assert(devclass >= input_device_class.DEVICE_CLASS_FIRST_VALID && devclass <= input_device_class.DEVICE_CLASS_LAST_VALID); return m_class[(int)devclass]; }


        // input code readers

        //-------------------------------------------------
        //  code_value - return the value of a given
        //  input code
        //-------------------------------------------------
        int code_value(input_code code)
        {
            profiler_global.g_profiler.start(profile_type.PROFILER_INPUT);


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
            profiler_global.g_profiler.stop();

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
            for (int memnum = 0; memnum < m_switch_memory.Length; memnum++)
            {
                // were we previous pressed on the last time through here?
                if (m_switch_memory[memnum] == code)
                {
                    // if no longer pressed, clear entry
                    if (curvalue == false)
                        m_switch_memory[memnum] = input_code.INPUT_CODE_INVALID;

                    // always return false
                    return false;
                }

                // remember the first empty entry
                if (empty == -1 && m_switch_memory[memnum] == input_code.INPUT_CODE_INVALID)
                    empty = memnum;
            }

            // if we get here, we were not previously pressed; if still not pressed, return 0
            if (curvalue == false)
                return false;

            // otherwise, add ourself to the memory and return 1
            //assert(empty != -1);
            if (empty != -1)
                m_switch_memory[empty] = code;

            return true;
        }


        // input code polling

        //-------------------------------------------------
        //  reset_polling - reset memories in preparation
        //  for polling
        //-------------------------------------------------
        public void reset_polling()
        {
            // reset switch memory
            reset_memory();

            // iterate over device classes and devices
            for (input_device_class devclass = input_device_class.DEVICE_CLASS_FIRST_VALID; devclass <= input_device_class.DEVICE_CLASS_LAST_VALID; devclass++)
            {
                for (int devnum = 0; devnum <= m_class[(int)devclass].maxindex(); devnum++)
                {
                    // fetch the device; ignore if NULL
                    input_device device = m_class[(int)devclass].device(devnum);
                    if (device == null)
                        continue;

                    // iterate over items within each device
                    for (input_item_id itemid = input_item_id.ITEM_ID_FIRST_VALID; itemid <= device.maxitem(); itemid++)
                    {
                        // for any non-switch items, set memory equal to the current value
                        input_device_item item = device.item(itemid);
                        if (item != null && item.itemclass() != input_item_class.ITEM_CLASS_SWITCH)
                            item.set_memory(code_value(item.code()));
                    }
                }
            }
        }

        //input_code poll_axes();

        //-------------------------------------------------
        //  poll_switches - poll for any input
        //-------------------------------------------------
        public input_code poll_switches()
        {
            // iterate over device classes and devices
            for (input_device_class devclass = input_device_class.DEVICE_CLASS_FIRST_VALID; devclass <= input_device_class.DEVICE_CLASS_LAST_VALID; devclass++)
            {
                for (int devnum = 0; devnum <= m_class[(int)devclass].maxindex(); devnum++)
                {
                    // fetch the device; ignore if NULL
                    input_device device = m_class[(int)devclass].device(devnum);
                    if (device == null)
                        continue;

                    // iterate over items within each device
                    for (input_item_id itemid = input_item_id.ITEM_ID_FIRST_VALID; itemid <= device.maxitem(); itemid++)
                    {
                        input_device_item item = device.item(itemid);
                        if (item != null)
                        {
                            input_code code = item.code();

                            // if the item is natively a switch, poll it
                            if (item.itemclass() == input_item_class.ITEM_CLASS_SWITCH)
                            {
                                if (code_pressed_once(code))
                                    return code;
                                else
                                    continue;
                            }

                            // skip if there is not enough axis movement
                            if (!code_check_axis(item, code))
                                continue;

                            // otherwise, poll axes digitally
                            code.set_item_class(input_item_class.ITEM_CLASS_SWITCH);

                            // if this is a joystick X axis, check with left/right modifiers
                            if (devclass == input_device_class.DEVICE_CLASS_JOYSTICK && code.item_id() == input_item_id.ITEM_ID_XAXIS)
                            {
                                code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_LEFT);
                                if (code_pressed_once(code))
                                    return code;
                                code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_RIGHT);
                                if (code_pressed_once(code))
                                    return code;
                            }

                            // if this is a joystick Y axis, check with up/down modifiers
                            else if (devclass == input_device_class.DEVICE_CLASS_JOYSTICK && code.item_id() == input_item_id.ITEM_ID_YAXIS)
                            {
                                code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_UP);
                                if (code_pressed_once(code))
                                    return code;
                                code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_DOWN);
                                if (code_pressed_once(code))
                                    return code;
                            }

                            // any other axis, check with pos/neg modifiers
                            else
                            {
                                code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_POS);
                                if (code_pressed_once(code))
                                    return code;
                                code.set_item_modifier(input_item_modifier.ITEM_MODIFIER_NEG);
                                if (code_pressed_once(code))
                                    return code;
                            }
                        }
                    }
                }
            }

            // if nothing, return an invalid code
            return input_code.INPUT_CODE_INVALID;
        }

        //input_code poll_keyboard_switches();


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
            string devclass = code_string_table.Find(input_global.devclass_string_table, (UInt32)code.device_class());

            // determine the devindex part
            string devindex = string.Format("{0}", code.device_index() + 1);

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
            string modifier = code_string_table.Find(input_global.modifier_string_table, (UInt32)code.item_modifier());

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
            return str.Trim();
        }


        //string code_to_token(input_code code) const;
        //input_code code_from_token(const char *_token);


        //-------------------------------------------------
        //  standard_token - return the standard token for
        //  the given input item ID
        //-------------------------------------------------
        public string standard_token(input_item_id itemid)
        {
            return itemid <= input_item_id.ITEM_ID_MAXIMUM ? input_global.itemid_token_table[(int)itemid].m_string : null;
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
                // handle NOT
                input_code code = seq[codenum];
                if (code == input_seq.not_code)
                    invert = true;

                // handle OR and END
                else if (code == input_seq.or_code || code == input_seq.end_code)
                {
                    // if we have a positive result from the previous set, we're done
                    if (result || code == input_seq.end_code)
                        break;

                    // otherwise, reset our state
                    result = false;
                    invert = false;
                    first = true;
                }

                // handle everything else as a series of ANDs
                else
                {
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
                // handle NOT
                input_code code = seq[codenum];
                if (code == input_seq.not_code)
                    invert = true;

                // handle OR and END
                else if (code == input_seq.or_code || code == input_seq.end_code)
                {
                    // if we have a positive result from the previous set, we're done
                    if (itemclass != input_item_class.ITEM_CLASS_INVALID || code == input_seq.end_code)
                        break;

                    // otherwise, reset our state
                    result = 0;
                    invert = false;
                    enable = true;
                }

                // handle everything else only if we're still enabled
                else if (enable)
                {
                    // switch codes serve as enables
                    if (code.item_class() == input_item_class.ITEM_CLASS_SWITCH)
                    {
                        // AND against previous digital codes
                        if (enable)
                            enable &= code_pressed(code) ^ invert;
                    }

                    // non-switch codes are analog values
                    else
                    {
                        int value = code_value(code);

                        // if we got a 0 value, don't do anything except remember the first type
                        if (value == 0)
                        {
                            if (itemclasszero == input_item_class.ITEM_CLASS_INVALID)
                                itemclasszero = code.item_class();
                        }

                        // non-zero absolute values stick
                        else if (code.item_class() == input_item_class.ITEM_CLASS_ABSOLUTE)
                        {
                            itemclass = input_item_class.ITEM_CLASS_ABSOLUTE;
                            result = value;
                        }

                        // non-zero relative values accumulate
                        else if (code.item_class() == input_item_class.ITEM_CLASS_RELATIVE)
                        {
                            itemclass = input_item_class.ITEM_CLASS_RELATIVE;
                            result += value;
                        }
                    }

                    // clear the invert flag
                    invert = false;
                }
            }

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
                if (!code.internal_get() && code_name(code).empty())
                {
                    while (clean_index > 0 && clean_codes[clean_index - 1].internal_get())
                    {
                        clean_codes.backspace();
                        clean_index--;
                    }
                }
                else if (clean_index > 0 || !code.internal_get() || code == input_seq.not_code)
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
        //bool map_device_to_controller(const devicemap_table_type *devicemap_table = nullptr);


        // internal helpers

        //-------------------------------------------------
        //  reset_memory - reset the array of memory for
        //  pressed switches
        //-------------------------------------------------
        void reset_memory()
        {
            // reset all entries in switch memory to invalid
            for (int memnum = 0; memnum < m_switch_memory.Length; memnum++)
                m_switch_memory[memnum] = input_code.INPUT_CODE_INVALID;
        }

        //-------------------------------------------------
        //  code_check_axis - see if axis has moved far
        //  enough to trigger a read when polling
        //-------------------------------------------------
        bool code_check_axis(input_device_item item, input_code code)
        {
            // if we've already reported this one, don't bother
            if (item.memory() == input_global.INVALID_AXIS_VALUE)
                return false;

            // ignore min/max for lightguns
            // so the selection will not be affected by a gun going out of range
            int curval = code_value(code);
            if (code.device_class() == input_device_class.DEVICE_CLASS_LIGHTGUN &&
                (code.item_id() == input_item_id.ITEM_ID_XAXIS || code.item_id() == input_item_id.ITEM_ID_YAXIS) &&
                (curval == input_global.INPUT_ABSOLUTE_MAX || curval == input_global.INPUT_ABSOLUTE_MIN))
                return false;

            // compute the diff against memory
            int diff = curval - item.memory();
            if (diff < 0)
                diff = -diff;

            // for absolute axes, look for 25% of maximum
            if (item.itemclass() == input_item_class.ITEM_CLASS_ABSOLUTE && diff > (input_global.INPUT_ABSOLUTE_MAX - input_global.INPUT_ABSOLUTE_MIN) / 4)
            {
                item.set_memory(input_global.INVALID_AXIS_VALUE);
                return true;
            }

            // for relative axes, look for ~20 pixels movement
            if (item.itemclass() == input_item_class.ITEM_CLASS_RELATIVE && diff > 20 * input_global.INPUT_RELATIVE_PER_PIXEL)
            {
                item.set_memory(input_global.INVALID_AXIS_VALUE);
                return true;
            }

            return false;
        }
    }


    // ======================> code_string_table
    // simple class to match codes to strings
    public struct code_string_table
    {
        public u32 m_code;
        public string m_string;

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


    public static class input_global
    {
        // invalid memory value for axis polling
        public const int INVALID_AXIS_VALUE      = 0x7fffffff;

        // relative devices return ~512 units per onscreen pixel
        public const int INPUT_RELATIVE_PER_PIXEL = 512;

        // absolute devices return values between -65536 and +65536
        public const int INPUT_ABSOLUTE_MIN = -65536;
        public const int INPUT_ABSOLUTE_MAX = 65536;

        // maximum number of axis/buttons/hats with ITEM_IDs for use by osd layer
        const int INPUT_MAX_AXIS = 8;
        const int INPUT_MAX_BUTTONS = 32;
        const int INPUT_MAX_HATS = 4;
        const int INPUT_MAX_ADD_SWITCH = 16;
        const int INPUT_MAX_ADD_ABSOLUTE = 16;
        const int INPUT_MAX_ADD_RELATIVE = 16;


        // device index
        public const int DEVICE_INDEX_MAXIMUM = 0xff;


        // invalid codes
        //#define INPUT_CODE_INVALID input_code()

        // keyboard codes
        public static input_code KEYCODE_A_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_A); }
        public static input_code KEYCODE_B_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_B); }
        public static input_code KEYCODE_C_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_C); }
        public static input_code KEYCODE_D_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_D); }
        public static input_code KEYCODE_E_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_E); }
        public static input_code KEYCODE_F_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F); }
        public static input_code KEYCODE_G_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_G); }
        public static input_code KEYCODE_H_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_H); }
        public static input_code KEYCODE_I_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_I); }
        public static input_code KEYCODE_J_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_J); }
        public static input_code KEYCODE_K_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_K); }
        public static input_code KEYCODE_L_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_L); }
        public static input_code KEYCODE_M_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_M); }
        public static input_code KEYCODE_N_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_N); }
        public static input_code KEYCODE_O_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_O); }
        public static input_code KEYCODE_P_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_P); }
        public static input_code KEYCODE_Q_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_Q); }
        public static input_code KEYCODE_R_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_R); }
        public static input_code KEYCODE_S_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_S); }
        public static input_code KEYCODE_T_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_T); }
        public static input_code KEYCODE_U_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_U); }
        public static input_code KEYCODE_V_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_V); }
        public static input_code KEYCODE_W_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_W); }
        public static input_code KEYCODE_X_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_X); }
        public static input_code KEYCODE_Y_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_Y); }
        public static input_code KEYCODE_Z_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_Z); }
        public static input_code KEYCODE_0_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_0); }
        public static input_code KEYCODE_1_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_1); }
        public static input_code KEYCODE_2_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_2); }
        public static input_code KEYCODE_3_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_3); }
        public static input_code KEYCODE_4_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_4); }
        public static input_code KEYCODE_5_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_5); }
        public static input_code KEYCODE_6_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_6); }
        public static input_code KEYCODE_7_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_7); }
        public static input_code KEYCODE_8_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_8); }
        public static input_code KEYCODE_9_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_9); }
        public static input_code KEYCODE_F1_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F1); }
        public static input_code KEYCODE_F2_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F2); }
        public static input_code KEYCODE_F3_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F3); }
        public static input_code KEYCODE_F4_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F4); }
        public static input_code KEYCODE_F5_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F5); }
        public static input_code KEYCODE_F6_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F6); }
        public static input_code KEYCODE_F7_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F7); }
        public static input_code KEYCODE_F8_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F8); }
        public static input_code KEYCODE_F9_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F9); }
        public static input_code KEYCODE_F10_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F10); }
        public static input_code KEYCODE_F11_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F11); }
        public static input_code KEYCODE_F12_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F12); }
        public static input_code KEYCODE_F13_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F13); }
        public static input_code KEYCODE_F14_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F14); }
        public static input_code KEYCODE_F15_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F15); }
        public static input_code KEYCODE_F16_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F16); }
        public static input_code KEYCODE_F17_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F17); }
        public static input_code KEYCODE_F18_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F18); }
        public static input_code KEYCODE_F19_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F19); }
        public static input_code KEYCODE_F20_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F20); }
        public static input_code KEYCODE_ESC_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_ESC); }
        public static input_code KEYCODE_TILDE_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_TILDE); }
        public static input_code KEYCODE_MINUS_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_MINUS); }
        public static input_code KEYCODE_EQUALS_INDEXED(int n)     { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_EQUALS); }
        public static input_code KEYCODE_BACKSPACE_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BACKSPACE); }
        public static input_code KEYCODE_TAB_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_TAB); }
        public static input_code KEYCODE_OPENBRACE_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_OPENBRACE); }
        public static input_code KEYCODE_CLOSEBRACE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_CLOSEBRACE); }
        public static input_code KEYCODE_ENTER_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_ENTER); }
        public static input_code KEYCODE_COLON_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_COLON); }
        public static input_code KEYCODE_QUOTE_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_QUOTE); }
        public static input_code KEYCODE_BACKSLASH_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BACKSLASH); }
        public static input_code KEYCODE_BACKSLASH2_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BACKSLASH2); }
        public static input_code KEYCODE_COMMA_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_COMMA); }
        public static input_code KEYCODE_STOP_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_STOP); }
        public static input_code KEYCODE_SLASH_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SLASH); }
        public static input_code KEYCODE_SPACE_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SPACE); }
        public static input_code KEYCODE_INSERT_INDEXED(int n)     { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_INSERT); }
        public static input_code KEYCODE_DEL_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_DEL); }
        public static input_code KEYCODE_HOME_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_HOME); }
        public static input_code KEYCODE_END_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_END); }
        public static input_code KEYCODE_PGUP_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_PGUP); }
        public static input_code KEYCODE_PGDN_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_PGDN); }
        public static input_code KEYCODE_LEFT_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_LEFT); }
        public static input_code KEYCODE_RIGHT_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RIGHT); }
        public static input_code KEYCODE_UP_INDEXED(int n)         { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_UP); }
        public static input_code KEYCODE_DOWN_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_DOWN); }
        public static input_code KEYCODE_0_PAD_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_0_PAD); }
        public static input_code KEYCODE_1_PAD_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_1_PAD); }
        public static input_code KEYCODE_2_PAD_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_2_PAD); }
        public static input_code KEYCODE_3_PAD_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_3_PAD); }
        public static input_code KEYCODE_4_PAD_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_4_PAD); }
        public static input_code KEYCODE_5_PAD_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_5_PAD); }
        public static input_code KEYCODE_6_PAD_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_6_PAD); }
        public static input_code KEYCODE_7_PAD_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_7_PAD); }
        public static input_code KEYCODE_8_PAD_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_8_PAD); }
        public static input_code KEYCODE_9_PAD_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_9_PAD); }
        public static input_code KEYCODE_SLASH_PAD_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SLASH_PAD); }
        public static input_code KEYCODE_ASTERISK_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_ASTERISK); }
        public static input_code KEYCODE_MINUS_PAD_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_MINUS_PAD); }
        public static input_code KEYCODE_PLUS_PAD_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_PLUS_PAD); }
        public static input_code KEYCODE_DEL_PAD_INDEXED(int n)    { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_DEL_PAD); }
        public static input_code KEYCODE_ENTER_PAD_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_ENTER_PAD); }
        public static input_code KEYCODE_BS_PAD_INDEXED(int n)     { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BS_PAD); }
        public static input_code KEYCODE_TAB_PAD_INDEXED(int n)    { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_TAB_PAD); }
        public static input_code KEYCODE_00_PAD_INDEXED(int n)     { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_00_PAD); }
        public static input_code KEYCODE_000_PAD_INDEXED(int n)    { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_000_PAD); }
        public static input_code KEYCODE_COMMA_PAD_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_COMMA_PAD); }
        public static input_code KEYCODE_EQUALS_PAD_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_EQUALS_PAD); }
        public static input_code KEYCODE_PRTSCR_INDEXED(int n)     { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_PRTSCR); }
        public static input_code KEYCODE_PAUSE_INDEXED(int n)      { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_PAUSE); }
        public static input_code KEYCODE_LSHIFT_INDEXED(int n)     { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_LSHIFT); }
        public static input_code KEYCODE_RSHIFT_INDEXED(int n)     { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RSHIFT); }
        public static input_code KEYCODE_LCONTROL_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_LCONTROL); }
        public static input_code KEYCODE_RCONTROL_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RCONTROL); }
        public static input_code KEYCODE_LALT_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_LALT); }
        public static input_code KEYCODE_RALT_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RALT); }
        public static input_code KEYCODE_SCRLOCK_INDEXED(int n)    { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SCRLOCK); }
        public static input_code KEYCODE_NUMLOCK_INDEXED(int n)    { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_NUMLOCK); }
        public static input_code KEYCODE_CAPSLOCK_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_CAPSLOCK); }
        public static input_code KEYCODE_LWIN_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_LWIN); }
        public static input_code KEYCODE_RWIN_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RWIN); }
        public static input_code KEYCODE_MENU_INDEXED(int n)       { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_MENU); }
        public static input_code KEYCODE_CANCEL_INDEXED(int n)     { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_CANCEL); }


        public static readonly input_code KEYCODE_A = KEYCODE_A_INDEXED(0);
        public static readonly input_code KEYCODE_B = KEYCODE_B_INDEXED(0);
        public static readonly input_code KEYCODE_C = KEYCODE_C_INDEXED(0);
        public static readonly input_code KEYCODE_D = KEYCODE_D_INDEXED(0);
        public static readonly input_code KEYCODE_E = KEYCODE_E_INDEXED(0);
        public static readonly input_code KEYCODE_F = KEYCODE_F_INDEXED(0);
        public static readonly input_code KEYCODE_G = KEYCODE_G_INDEXED(0);
        public static readonly input_code KEYCODE_H = KEYCODE_H_INDEXED(0);
        public static readonly input_code KEYCODE_I = KEYCODE_I_INDEXED(0);
        public static readonly input_code KEYCODE_J = KEYCODE_J_INDEXED(0);
        public static readonly input_code KEYCODE_K = KEYCODE_K_INDEXED(0);
        public static readonly input_code KEYCODE_L = KEYCODE_L_INDEXED(0);
        public static readonly input_code KEYCODE_M = KEYCODE_M_INDEXED(0);
        public static readonly input_code KEYCODE_N = KEYCODE_N_INDEXED(0);
        public static readonly input_code KEYCODE_O = KEYCODE_O_INDEXED(0);
        public static readonly input_code KEYCODE_P = KEYCODE_P_INDEXED(0);
        public static readonly input_code KEYCODE_Q = KEYCODE_Q_INDEXED(0);
        public static readonly input_code KEYCODE_R = KEYCODE_R_INDEXED(0);
        public static readonly input_code KEYCODE_S = KEYCODE_S_INDEXED(0);
        public static readonly input_code KEYCODE_T = KEYCODE_T_INDEXED(0);
        public static readonly input_code KEYCODE_U = KEYCODE_U_INDEXED(0);
        public static readonly input_code KEYCODE_V = KEYCODE_V_INDEXED(0);
        public static readonly input_code KEYCODE_W = KEYCODE_W_INDEXED(0);
        public static readonly input_code KEYCODE_X = KEYCODE_X_INDEXED(0);
        public static readonly input_code KEYCODE_Y = KEYCODE_Y_INDEXED(0);
        public static readonly input_code KEYCODE_Z = KEYCODE_Z_INDEXED(0);
        public static readonly input_code KEYCODE_0 = KEYCODE_0_INDEXED(0);
        public static readonly input_code KEYCODE_1 = KEYCODE_1_INDEXED(0);
        public static readonly input_code KEYCODE_2 = KEYCODE_2_INDEXED(0);
        public static readonly input_code KEYCODE_3 = KEYCODE_3_INDEXED(0);
        public static readonly input_code KEYCODE_4 = KEYCODE_4_INDEXED(0);
        public static readonly input_code KEYCODE_5 = KEYCODE_5_INDEXED(0);
        public static readonly input_code KEYCODE_6 = KEYCODE_6_INDEXED(0);
        public static readonly input_code KEYCODE_7 = KEYCODE_7_INDEXED(0);
        public static readonly input_code KEYCODE_8 = KEYCODE_8_INDEXED(0);
        public static readonly input_code KEYCODE_9 = KEYCODE_9_INDEXED(0);
        public static readonly input_code KEYCODE_F1 = KEYCODE_F1_INDEXED(0);
        public static readonly input_code KEYCODE_F2 = KEYCODE_F2_INDEXED(0);
        public static readonly input_code KEYCODE_F3 = KEYCODE_F3_INDEXED(0);
        public static readonly input_code KEYCODE_F4 = KEYCODE_F4_INDEXED(0);
        public static readonly input_code KEYCODE_F5 = KEYCODE_F5_INDEXED(0);
        public static readonly input_code KEYCODE_F6 = KEYCODE_F6_INDEXED(0);
        public static readonly input_code KEYCODE_F7 = KEYCODE_F7_INDEXED(0);
        public static readonly input_code KEYCODE_F8 = KEYCODE_F8_INDEXED(0);
        public static readonly input_code KEYCODE_F9 = KEYCODE_F9_INDEXED(0);
        public static readonly input_code KEYCODE_F10 = KEYCODE_F10_INDEXED(0);
        public static readonly input_code KEYCODE_F11 = KEYCODE_F11_INDEXED(0);
        public static readonly input_code KEYCODE_F12 = KEYCODE_F12_INDEXED(0);
        public static readonly input_code KEYCODE_F13 = KEYCODE_F13_INDEXED(0);
        public static readonly input_code KEYCODE_F14 = KEYCODE_F14_INDEXED(0);
        public static readonly input_code KEYCODE_F15 = KEYCODE_F15_INDEXED(0);
        public static readonly input_code KEYCODE_F16 = KEYCODE_F16_INDEXED(0);
        public static readonly input_code KEYCODE_F17 = KEYCODE_F17_INDEXED(0);
        public static readonly input_code KEYCODE_F18 = KEYCODE_F18_INDEXED(0);
        public static readonly input_code KEYCODE_F19 = KEYCODE_F19_INDEXED(0);
        public static readonly input_code KEYCODE_F20 = KEYCODE_F20_INDEXED(0);
        public static readonly input_code KEYCODE_ESC = KEYCODE_ESC_INDEXED(0);
        public static readonly input_code KEYCODE_TILDE = KEYCODE_TILDE_INDEXED(0);
        public static readonly input_code KEYCODE_MINUS = KEYCODE_MINUS_INDEXED(0);
        public static readonly input_code KEYCODE_EQUALS = KEYCODE_EQUALS_INDEXED(0);
        public static readonly input_code KEYCODE_BACKSPACE = KEYCODE_BACKSPACE_INDEXED(0);
        public static readonly input_code KEYCODE_TAB = KEYCODE_TAB_INDEXED(0);
        public static readonly input_code KEYCODE_OPENBRACE = KEYCODE_OPENBRACE_INDEXED(0);
        public static readonly input_code KEYCODE_CLOSEBRACE = KEYCODE_CLOSEBRACE_INDEXED(0);
        public static readonly input_code KEYCODE_ENTER = KEYCODE_ENTER_INDEXED(0);
        public static readonly input_code KEYCODE_COLON = KEYCODE_COLON_INDEXED(0);
        //#define KEYCODE_QUOTE KEYCODE_QUOTE_INDEXED(0)
        //#define KEYCODE_BACKSLASH KEYCODE_BACKSLASH_INDEXED(0)
        //#define KEYCODE_BACKSLASH2 KEYCODE_BACKSLASH2_INDEXED(0)
        public static readonly input_code KEYCODE_COMMA = KEYCODE_COMMA_INDEXED(0);
        public static readonly input_code KEYCODE_STOP = KEYCODE_STOP_INDEXED(0);
        public static readonly input_code KEYCODE_SLASH = KEYCODE_SLASH_INDEXED(0);
        public static readonly input_code KEYCODE_SPACE = KEYCODE_SPACE_INDEXED(0);
        public static readonly input_code KEYCODE_INSERT = KEYCODE_INSERT_INDEXED(0);
        public static readonly input_code KEYCODE_DEL = KEYCODE_DEL_INDEXED(0);
        public static readonly input_code KEYCODE_HOME = KEYCODE_HOME_INDEXED(0);
        public static readonly input_code KEYCODE_END = KEYCODE_END_INDEXED(0);
        public static readonly input_code KEYCODE_PGUP = KEYCODE_PGUP_INDEXED(0);
        public static readonly input_code KEYCODE_PGDN = KEYCODE_PGDN_INDEXED(0);
        public static readonly input_code KEYCODE_LEFT = KEYCODE_LEFT_INDEXED(0);
        public static readonly input_code KEYCODE_RIGHT = KEYCODE_RIGHT_INDEXED(0);
        public static readonly input_code KEYCODE_UP = KEYCODE_UP_INDEXED(0);
        public static readonly input_code KEYCODE_DOWN = KEYCODE_DOWN_INDEXED(0);
        public static readonly input_code KEYCODE_0_PAD = KEYCODE_0_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_1_PAD = KEYCODE_1_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_2_PAD = KEYCODE_2_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_3_PAD = KEYCODE_3_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_4_PAD = KEYCODE_4_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_5_PAD = KEYCODE_5_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_6_PAD = KEYCODE_6_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_7_PAD = KEYCODE_7_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_8_PAD = KEYCODE_8_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_9_PAD = KEYCODE_9_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_SLASH_PAD = KEYCODE_SLASH_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_ASTERISK = KEYCODE_ASTERISK_INDEXED(0);
        public static readonly input_code KEYCODE_MINUS_PAD = KEYCODE_MINUS_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_PLUS_PAD = KEYCODE_PLUS_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_DEL_PAD = KEYCODE_DEL_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_ENTER_PAD = KEYCODE_ENTER_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_BS_PAD = KEYCODE_BS_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_TAB_PAD = KEYCODE_TAB_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_00_PAD = KEYCODE_00_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_000_PAD = KEYCODE_000_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_COMMA_PAD = KEYCODE_COMMA_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_EQUALS_PAD = KEYCODE_EQUALS_PAD_INDEXED(0);
        public static readonly input_code KEYCODE_PRTSCR = KEYCODE_PRTSCR_INDEXED(0);
        public static readonly input_code KEYCODE_PAUSE = KEYCODE_PAUSE_INDEXED(0);
        public static readonly input_code KEYCODE_LSHIFT = KEYCODE_LSHIFT_INDEXED(0);
        public static readonly input_code KEYCODE_RSHIFT = KEYCODE_RSHIFT_INDEXED(0);
        public static readonly input_code KEYCODE_LCONTROL = KEYCODE_LCONTROL_INDEXED(0);
        public static readonly input_code KEYCODE_RCONTROL = KEYCODE_RCONTROL_INDEXED(0);
        public static readonly input_code KEYCODE_LALT = KEYCODE_LALT_INDEXED(0);
        public static readonly input_code KEYCODE_RALT = KEYCODE_RALT_INDEXED(0);
        public static readonly input_code KEYCODE_SCRLOCK = KEYCODE_SCRLOCK_INDEXED(0);
        //#define KEYCODE_NUMLOCK KEYCODE_NUMLOCK_INDEXED(0)
        //#define KEYCODE_CAPSLOCK KEYCODE_CAPSLOCK_INDEXED(0)
        //#define KEYCODE_LWIN KEYCODE_LWIN_INDEXED(0)
        //#define KEYCODE_RWIN KEYCODE_RWIN_INDEXED(0)
        //#define KEYCODE_MENU KEYCODE_MENU_INDEXED(0)
        //#define KEYCODE_CANCEL KEYCODE_CANCEL_INDEXED(0)

        // mouse axes as relative devices
        public static input_code MOUSECODE_X_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_RELATIVE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_XAXIS); }
        public static input_code MOUSECODE_Y_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_RELATIVE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_YAXIS); }
        public static input_code MOUSECODE_Z_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_RELATIVE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_ZAXIS); }

        //#define MOUSECODE_X MOUSECODE_X_INDEXED(0)
        //#define MOUSECODE_Y MOUSECODE_Y_INDEXED(0)
        //#define MOUSECODE_Z MOUSECODE_Z_INDEXED(0)

        // mouse axes as switches in +/- direction
        //#define MOUSECODE_X_POS_SWITCH_INDEXED(n) input_code(DEVICE_CLASS_MOUSE, n, ITEM_CLASS_SWITCH, ITEM_MODIFIER_POS, ITEM_ID_XAXIS)
        //#define MOUSECODE_X_NEG_SWITCH_INDEXED(n) input_code(DEVICE_CLASS_MOUSE, n, ITEM_CLASS_SWITCH, ITEM_MODIFIER_NEG, ITEM_ID_XAXIS)
        //#define MOUSECODE_Y_POS_SWITCH_INDEXED(n) input_code(DEVICE_CLASS_MOUSE, n, ITEM_CLASS_SWITCH, ITEM_MODIFIER_POS, ITEM_ID_YAXIS)
        //#define MOUSECODE_Y_NEG_SWITCH_INDEXED(n) input_code(DEVICE_CLASS_MOUSE, n, ITEM_CLASS_SWITCH, ITEM_MODIFIER_NEG, ITEM_ID_YAXIS)
        //#define MOUSECODE_Z_POS_SWITCH_INDEXED(n) input_code(DEVICE_CLASS_MOUSE, n, ITEM_CLASS_SWITCH, ITEM_MODIFIER_POS, ITEM_ID_ZAXIS)
        //#define MOUSECODE_Z_NEG_SWITCH_INDEXED(n) input_code(DEVICE_CLASS_MOUSE, n, ITEM_CLASS_SWITCH, ITEM_MODIFIER_NEG, ITEM_ID_ZAXIS)

        //#define MOUSECODE_X_POS_SWITCH MOUSECODE_X_POS_SWITCH_INDEXED(0)
        //#define MOUSECODE_X_NEG_SWITCH MOUSECODE_X_NEG_SWITCH_INDEXED(0)
        //#define MOUSECODE_Y_POS_SWITCH MOUSECODE_Y_POS_SWITCH_INDEXED(0)
        //#define MOUSECODE_Y_NEG_SWITCH MOUSECODE_Y_NEG_SWITCH_INDEXED(0)
        //#define MOUSECODE_Z_POS_SWITCH MOUSECODE_Z_POS_SWITCH_INDEXED(0)
        //#define MOUSECODE_Z_NEG_SWITCH MOUSECODE_Z_NEG_SWITCH_INDEXED(0)

        // mouse buttons
        public static input_code MOUSECODE_BUTTON1_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON1); }
        public static input_code MOUSECODE_BUTTON2_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON2); }
        public static input_code MOUSECODE_BUTTON3_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON3); }
        public static input_code MOUSECODE_BUTTON4_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON4); }
        public static input_code MOUSECODE_BUTTON5_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON5); }
        public static input_code MOUSECODE_BUTTON6_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON6); }
        public static input_code MOUSECODE_BUTTON7_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON7); }
        public static input_code MOUSECODE_BUTTON8_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON8); }

        //#define MOUSECODE_BUTTON1 MOUSECODE_BUTTON1_INDEXED(0)
        //#define MOUSECODE_BUTTON2 MOUSECODE_BUTTON2_INDEXED(0)
        //#define MOUSECODE_BUTTON3 MOUSECODE_BUTTON3_INDEXED(0)
        //#define MOUSECODE_BUTTON4 MOUSECODE_BUTTON4_INDEXED(0)
        //#define MOUSECODE_BUTTON5 MOUSECODE_BUTTON5_INDEXED(0)
        //#define MOUSECODE_BUTTON6 MOUSECODE_BUTTON6_INDEXED(0)
        //#define MOUSECODE_BUTTON7 MOUSECODE_BUTTON7_INDEXED(0)
        //#define MOUSECODE_BUTTON8 MOUSECODE_BUTTON8_INDEXED(0)

        // gun axes as absolute devices
        public static input_code GUNCODE_X_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_XAXIS); }
        public static input_code GUNCODE_Y_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_YAXIS); }

        //#define GUNCODE_X GUNCODE_X_INDEXED(0)
        //#define GUNCODE_Y GUNCODE_Y_INDEXED(0)

        // gun buttons
        public static input_code GUNCODE_BUTTON1_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON1); }
        public static input_code GUNCODE_BUTTON2_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON2); }
        public static input_code GUNCODE_BUTTON3_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON3); }
        public static input_code GUNCODE_BUTTON4_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON4); }
        public static input_code GUNCODE_BUTTON5_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON5); }
        public static input_code GUNCODE_BUTTON6_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON6); }
        public static input_code GUNCODE_BUTTON7_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON7); }
        public static input_code GUNCODE_BUTTON8_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON8); }

        //#define GUNCODE_BUTTON1 GUNCODE_BUTTON1_INDEXED(0)
        //#define GUNCODE_BUTTON2 GUNCODE_BUTTON2_INDEXED(0)
        //#define GUNCODE_BUTTON3 GUNCODE_BUTTON3_INDEXED(0)
        //#define GUNCODE_BUTTON4 GUNCODE_BUTTON4_INDEXED(0)
        //#define GUNCODE_BUTTON5 GUNCODE_BUTTON5_INDEXED(0)
        //#define GUNCODE_BUTTON6 GUNCODE_BUTTON6_INDEXED(0)
        //#define GUNCODE_BUTTON7 GUNCODE_BUTTON7_INDEXED(0)
        //#define GUNCODE_BUTTON8 GUNCODE_BUTTON8_INDEXED(0)

        // joystick axes as absolute devices
        public static input_code JOYCODE_X_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_XAXIS); }
        public static input_code JOYCODE_Y_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_YAXIS); }
        public static input_code JOYCODE_Z_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_ZAXIS); }
        public static input_code JOYCODE_U_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RXAXIS); }
        public static input_code JOYCODE_V_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RYAXIS); }
        public static input_code JOYCODE_W_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RZAXIS); }

        //#define JOYCODE_X JOYCODE_X_INDEXED(0)
        //#define JOYCODE_Y JOYCODE_Y_INDEXED(0)
        //#define JOYCODE_Z JOYCODE_Z_INDEXED(0)
        //#define JOYCODE_U JOYCODE_U_INDEXED(0)
        //#define JOYCODE_V JOYCODE_V_INDEXED(0)
        //#define JOYCODE_W JOYCODE_W_INDEXED(0)

        // joystick axes as absolute half-axes
        public static input_code JOYCODE_X_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_XAXIS); }
        public static input_code JOYCODE_X_NEG_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_XAXIS); }
        public static input_code JOYCODE_Y_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_YAXIS); }
        public static input_code JOYCODE_Y_NEG_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_YAXIS); }
        public static input_code JOYCODE_Z_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_ZAXIS); }
        public static input_code JOYCODE_Z_NEG_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_ZAXIS); }
        public static input_code JOYCODE_U_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_RXAXIS); }
        public static input_code JOYCODE_U_NEG_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_RXAXIS); }
        public static input_code JOYCODE_V_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_RYAXIS); }
        public static input_code JOYCODE_V_NEG_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_RYAXIS); }
        public static input_code JOYCODE_W_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_RZAXIS); }
        public static input_code JOYCODE_W_NEG_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_RZAXIS); }

        //#define JOYCODE_X_POS_ABSOLUTE JOYCODE_X_POS_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_X_NEG_ABSOLUTE JOYCODE_X_NEG_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_Y_POS_ABSOLUTE JOYCODE_Y_POS_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_Y_NEG_ABSOLUTE JOYCODE_Y_NEG_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_Z_POS_ABSOLUTE JOYCODE_Z_POS_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_Z_NEG_ABSOLUTE JOYCODE_Z_NEG_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_U_POS_ABSOLUTE JOYCODE_U_POS_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_U_NEG_ABSOLUTE JOYCODE_U_NEG_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_V_POS_ABSOLUTE JOYCODE_V_POS_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_V_NEG_ABSOLUTE JOYCODE_V_NEG_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_W_POS_ABSOLUTE JOYCODE_W_POS_ABSOLUTE_INDEXED(0)
        //#define JOYCODE_W_NEG_ABSOLUTE JOYCODE_W_NEG_ABSOLUTE_INDEXED(0)

        // joystick axes as switches; X/Y are specially handled for left/right/up/down mapping
        public static input_code JOYCODE_X_LEFT_SWITCH_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_LEFT,  input_item_id.ITEM_ID_XAXIS); }
        public static input_code JOYCODE_X_RIGHT_SWITCH_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_RIGHT, input_item_id.ITEM_ID_XAXIS); }
        public static input_code JOYCODE_Y_UP_SWITCH_INDEXED(int n)    { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_UP,    input_item_id.ITEM_ID_YAXIS); }
        public static input_code JOYCODE_Y_DOWN_SWITCH_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_DOWN,  input_item_id.ITEM_ID_YAXIS); }
        public static input_code JOYCODE_Z_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS,   input_item_id.ITEM_ID_ZAXIS); }
        public static input_code JOYCODE_Z_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG,   input_item_id.ITEM_ID_ZAXIS); }
        public static input_code JOYCODE_U_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS,   input_item_id.ITEM_ID_RXAXIS); }
        public static input_code JOYCODE_U_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG,   input_item_id.ITEM_ID_RXAXIS); }
        public static input_code JOYCODE_V_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS,   input_item_id.ITEM_ID_RYAXIS); }
        public static input_code JOYCODE_V_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG,   input_item_id.ITEM_ID_RYAXIS); }
        public static input_code JOYCODE_W_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS,   input_item_id.ITEM_ID_RZAXIS); }
        public static input_code JOYCODE_W_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG,   input_item_id.ITEM_ID_RZAXIS); }

        //#define JOYCODE_X_LEFT_SWITCH JOYCODE_X_LEFT_SWITCH_INDEXED(0)
        //#define JOYCODE_X_RIGHT_SWITCH JOYCODE_X_RIGHT_SWITCH_INDEXED(0)
        //#define JOYCODE_Y_UP_SWITCH JOYCODE_Y_UP_SWITCH_INDEXED(0)
        //#define JOYCODE_Y_DOWN_SWITCH JOYCODE_Y_DOWN_SWITCH_INDEXED(0)
        //#define JOYCODE_Z_POS_SWITCH JOYCODE_Z_POS_SWITCH_INDEXED(0)
        //#define JOYCODE_Z_NEG_SWITCH JOYCODE_Z_NEG_SWITCH_INDEXED(0)
        //#define JOYCODE_U_POS_SWITCH JOYCODE_U_POS_SWITCH_INDEXED(0)
        //#define JOYCODE_U_NEG_SWITCH JOYCODE_U_NEG_SWITCH_INDEXED(0)
        //#define JOYCODE_V_POS_SWITCH JOYCODE_V_POS_SWITCH_INDEXED(0)
        //#define JOYCODE_V_NEG_SWITCH JOYCODE_V_NEG_SWITCH_INDEXED(0)
        //#define JOYCODE_W_POS_SWITCH JOYCODE_W_POS_SWITCH_INDEXED(0)
        //#define JOYCODE_W_NEG_SWITCH JOYCODE_W_NEG_SWITCH_INDEXED(0)

        // joystick buttons
        public static input_code JOYCODE_BUTTON1_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON1); }
        public static input_code JOYCODE_BUTTON2_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON2); }
        public static input_code JOYCODE_BUTTON3_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON3); }
        public static input_code JOYCODE_BUTTON4_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON4); }
        public static input_code JOYCODE_BUTTON5_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON5); }
        public static input_code JOYCODE_BUTTON6_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON6); }
        public static input_code JOYCODE_BUTTON7_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON7); }
        public static input_code JOYCODE_BUTTON8_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON8); }
        public static input_code JOYCODE_BUTTON9_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON9); }
        public static input_code JOYCODE_BUTTON10_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON10); }
        public static input_code JOYCODE_BUTTON11_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON11); }
        public static input_code JOYCODE_BUTTON12_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON12); }
        public static input_code JOYCODE_BUTTON13_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON13); }
        public static input_code JOYCODE_BUTTON14_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON14); }
        public static input_code JOYCODE_BUTTON15_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON15); }
        public static input_code JOYCODE_BUTTON16_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON16); }
        public static input_code JOYCODE_BUTTON17_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON17); }
        public static input_code JOYCODE_BUTTON18_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON18); }
        public static input_code JOYCODE_BUTTON19_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON19); }
        public static input_code JOYCODE_BUTTON20_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON20); }
        public static input_code JOYCODE_BUTTON21_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON21); }
        public static input_code JOYCODE_BUTTON22_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON22); }
        public static input_code JOYCODE_BUTTON23_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON23); }
        public static input_code JOYCODE_BUTTON24_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON24); }
        public static input_code JOYCODE_BUTTON25_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON25); }
        public static input_code JOYCODE_BUTTON26_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON26); }
        public static input_code JOYCODE_BUTTON27_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON27); }
        public static input_code JOYCODE_BUTTON28_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON28); }
        public static input_code JOYCODE_BUTTON29_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON29); }
        public static input_code JOYCODE_BUTTON30_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON30); }
        public static input_code JOYCODE_BUTTON31_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON31); }
        public static input_code JOYCODE_BUTTON32_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON32); }
        public static input_code JOYCODE_START_INDEXED(int n)    { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_START); }
        public static input_code JOYCODE_SELECT_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_SELECT); }

        //#define JOYCODE_BUTTON1 JOYCODE_BUTTON1_INDEXED(0)
        //#define JOYCODE_BUTTON2 JOYCODE_BUTTON2_INDEXED(0)
        //#define JOYCODE_BUTTON3 JOYCODE_BUTTON3_INDEXED(0)
        //#define JOYCODE_BUTTON4 JOYCODE_BUTTON4_INDEXED(0)
        //#define JOYCODE_BUTTON5 JOYCODE_BUTTON5_INDEXED(0)
        //#define JOYCODE_BUTTON6 JOYCODE_BUTTON6_INDEXED(0)
        //#define JOYCODE_BUTTON7 JOYCODE_BUTTON7_INDEXED(0)
        //#define JOYCODE_BUTTON8 JOYCODE_BUTTON8_INDEXED(0)
        //#define JOYCODE_BUTTON9 JOYCODE_BUTTON9_INDEXED(0)
        //#define JOYCODE_BUTTON10 JOYCODE_BUTTON10_INDEXED(0)
        //#define JOYCODE_BUTTON11 JOYCODE_BUTTON11_INDEXED(0)
        //#define JOYCODE_BUTTON12 JOYCODE_BUTTON12_INDEXED(0)
        //#define JOYCODE_BUTTON13 JOYCODE_BUTTON13_INDEXED(0)
        //#define JOYCODE_BUTTON14 JOYCODE_BUTTON14_INDEXED(0)
        //#define JOYCODE_BUTTON15 JOYCODE_BUTTON15_INDEXED(0)
        //#define JOYCODE_BUTTON16 JOYCODE_BUTTON16_INDEXED(0)
        //#define JOYCODE_BUTTON17 JOYCODE_BUTTON17_INDEXED(0)
        //#define JOYCODE_BUTTON18 JOYCODE_BUTTON18_INDEXED(0)
        //#define JOYCODE_BUTTON19 JOYCODE_BUTTON19_INDEXED(0)
        //#define JOYCODE_BUTTON20 JOYCODE_BUTTON20_INDEXED(0)
        //#define JOYCODE_BUTTON21 JOYCODE_BUTTON21_INDEXED(0)
        //#define JOYCODE_BUTTON22 JOYCODE_BUTTON22_INDEXED(0)
        //#define JOYCODE_BUTTON23 JOYCODE_BUTTON23_INDEXED(0)
        //#define JOYCODE_BUTTON24 JOYCODE_BUTTON24_INDEXED(0)
        //#define JOYCODE_BUTTON25 JOYCODE_BUTTON25_INDEXED(0)
        //#define JOYCODE_BUTTON26 JOYCODE_BUTTON26_INDEXED(0)
        //#define JOYCODE_BUTTON27 JOYCODE_BUTTON27_INDEXED(0)
        //#define JOYCODE_BUTTON28 JOYCODE_BUTTON28_INDEXED(0)
        //#define JOYCODE_BUTTON29 JOYCODE_BUTTON29_INDEXED(0)
        //#define JOYCODE_BUTTON30 JOYCODE_BUTTON30_INDEXED(0)
        //#define JOYCODE_BUTTON31 JOYCODE_BUTTON31_INDEXED(0)
        //#define JOYCODE_BUTTON32 JOYCODE_BUTTON32_INDEXED(0)
        //#define JOYCODE_START JOYCODE_START_INDEXED(0)
        //#define JOYCODE_SELECT JOYCODE_SELECT_INDEXED(0)


        // token strings for device classes
        public static readonly code_string_table [] devclass_token_table = new code_string_table[]
        {
            new code_string_table() { m_code = (UInt32)input_device_class.DEVICE_CLASS_KEYBOARD, m_string = "KEYCODE" },
            new code_string_table() { m_code = (UInt32)input_device_class.DEVICE_CLASS_MOUSE,    m_string = "MOUSECODE" },
            new code_string_table() { m_code = (UInt32)input_device_class.DEVICE_CLASS_LIGHTGUN, m_string = "GUNCODE" },
            new code_string_table() { m_code = (UInt32)input_device_class.DEVICE_CLASS_JOYSTICK, m_string = "JOYCODE" },
            new code_string_table() { m_code = UInt32.MaxValue,                                  m_string = "UNKCODE" }
        };

        // friendly strings for device classes
        public static readonly code_string_table [] devclass_string_table = new code_string_table[]
        {
            new code_string_table() { m_code = (UInt32)input_device_class.DEVICE_CLASS_KEYBOARD, m_string = "Kbd" },
            new code_string_table() { m_code = (UInt32)input_device_class.DEVICE_CLASS_MOUSE,    m_string = "Mouse" },
            new code_string_table() { m_code = (UInt32)input_device_class.DEVICE_CLASS_LIGHTGUN, m_string = "Gun" },
            new code_string_table() { m_code = (UInt32)input_device_class.DEVICE_CLASS_JOYSTICK, m_string = "Joy" },
            new code_string_table() { m_code = UInt32.MaxValue,                                  m_string = "Unk" }
        };

        // token strings for item modifiers
        public static readonly code_string_table [] modifier_token_table = new code_string_table[]
        {
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_POS,     m_string = "POS" },
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_NEG,     m_string = "NEG" },
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_LEFT,    m_string = "LEFT" },
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_RIGHT,   m_string = "RIGHT" },
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_UP,      m_string = "UP" },
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_DOWN,    m_string = "DOWN" },
            new code_string_table() { m_code = UInt32.MaxValue,                                   m_string = "" }
        };

        // friendly strings for item modifiers
        public static readonly code_string_table [] modifier_string_table = new code_string_table[]
        {
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_POS,     m_string = "+" },
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_NEG,     m_string = "-" },
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_LEFT,    m_string = "Left" },
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_RIGHT,   m_string = "Right" },
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_UP,      m_string = "Up" },
            new code_string_table() { m_code = (UInt32)input_item_modifier.ITEM_MODIFIER_DOWN,    m_string = "Down" },
            new code_string_table() { m_code = UInt32.MaxValue,                                   m_string = "" }
        };

        // token strings for item classes
        public static readonly code_string_table [] itemclass_token_table = new code_string_table[]
        {
            new code_string_table() { m_code = (UInt32)input_item_class.ITEM_CLASS_SWITCH,     m_string = "SWITCH" },
            new code_string_table() { m_code = (UInt32)input_item_class.ITEM_CLASS_ABSOLUTE,   m_string = "ABSOLUTE" },
            new code_string_table() { m_code = (UInt32)input_item_class.ITEM_CLASS_RELATIVE,   m_string = "RELATIVE" },
            new code_string_table() { m_code = UInt32.MaxValue,                                m_string = "" }
        };

        // token strings for standard item ids
        public static readonly code_string_table [] itemid_token_table = new code_string_table[]
        {
            // standard keyboard codes
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_A,             m_string = "A" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_B,             m_string = "B" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_C,             m_string = "C" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_D,             m_string = "D" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_E,             m_string = "E" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F,             m_string = "F" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_G,             m_string = "G" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_H,             m_string = "H" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_I,             m_string = "I" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_J,             m_string = "J" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_K,             m_string = "K" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_L,             m_string = "L" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_M,             m_string = "M" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_N,             m_string = "N" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_O,             m_string = "O" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_P,             m_string = "P" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_Q,             m_string = "Q" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_R,             m_string = "R" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_S,             m_string = "S" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_T,             m_string = "T" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_U,             m_string = "U" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_V,             m_string = "V" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_W,             m_string = "W" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_X,             m_string = "X" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_Y,             m_string = "Y" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_Z,             m_string = "Z" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_0,             m_string = "0" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_1,             m_string = "1" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_2,             m_string = "2" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_3,             m_string = "3" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_4,             m_string = "4" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_5,             m_string = "5" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_6,             m_string = "6" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_7,             m_string = "7" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_8,             m_string = "8" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_9,             m_string = "9" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F1,            m_string = "F1" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F2,            m_string = "F2" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F3,            m_string = "F3" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F4,            m_string = "F4" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F5,            m_string = "F5" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F6,            m_string = "F6" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F7,            m_string = "F7" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F8,            m_string = "F8" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F9,            m_string = "F9" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F10,           m_string = "F10" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F11,           m_string = "F11" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F12,           m_string = "F12" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F13,           m_string = "F13" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F14,           m_string = "F14" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F15,           m_string = "F15" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F16,           m_string = "F16" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F17,           m_string = "F17" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F18,           m_string = "F18" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F19,           m_string = "F19" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_F20,           m_string = "F20" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ESC,           m_string = "ESC" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_TILDE,         m_string = "TILDE" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_MINUS,         m_string = "MINUS" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_EQUALS,        m_string = "EQUALS" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BACKSPACE,     m_string = "BACKSPACE" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_TAB,           m_string = "TAB" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_OPENBRACE,     m_string = "OPENBRACE" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_CLOSEBRACE,    m_string = "CLOSEBRACE" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ENTER,         m_string = "ENTER" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_COLON,         m_string = "COLON" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_QUOTE,         m_string = "QUOTE" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BACKSLASH,     m_string = "BACKSLASH" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BACKSLASH2,    m_string = "BACKSLASH2" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_COMMA,         m_string = "COMMA" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_STOP,          m_string = "STOP" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_SLASH,         m_string = "SLASH" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_SPACE,         m_string = "SPACE" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_INSERT,        m_string = "INSERT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_DEL,           m_string = "DEL" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HOME,          m_string = "HOME" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_END,           m_string = "END" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_PGUP,          m_string = "PGUP" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_PGDN,          m_string = "PGDN" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_LEFT,          m_string = "LEFT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_RIGHT,         m_string = "RIGHT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_UP,            m_string = "UP" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_DOWN,          m_string = "DOWN" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_0_PAD,         m_string = "0PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_1_PAD,         m_string = "1PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_2_PAD,         m_string = "2PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_3_PAD,         m_string = "3PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_4_PAD,         m_string = "4PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_5_PAD,         m_string = "5PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_6_PAD,         m_string = "6PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_7_PAD,         m_string = "7PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_8_PAD,         m_string = "8PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_9_PAD,         m_string = "9PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_SLASH_PAD,     m_string = "SLASHPAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ASTERISK,      m_string = "ASTERISK" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_MINUS_PAD,     m_string = "MINUSPAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_PLUS_PAD,      m_string = "PLUSPAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_DEL_PAD,       m_string = "DELPAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ENTER_PAD,     m_string = "ENTERPAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BS_PAD,        m_string = "BSPAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_TAB_PAD,       m_string = "TABPAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_00_PAD,        m_string = "00PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_000_PAD,       m_string = "000PAD" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_PRTSCR,        m_string = "PRTSCR" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_PAUSE,         m_string = "PAUSE" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_LSHIFT,        m_string = "LSHIFT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_RSHIFT,        m_string = "RSHIFT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_LCONTROL,      m_string = "LCONTROL" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_RCONTROL,      m_string = "RCONTROL" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_LALT,          m_string = "LALT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_RALT,          m_string = "RALT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_SCRLOCK,       m_string = "SCRLOCK" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_NUMLOCK,       m_string = "NUMLOCK" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_CAPSLOCK,      m_string = "CAPSLOCK" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_LWIN,          m_string = "LWIN" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_RWIN,          m_string = "RWIN" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_MENU,          m_string = "MENU" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_CANCEL,        m_string = "CANCEL" },

            // standard mouse/joystick/gun codes
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_XAXIS,         m_string = "XAXIS" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_YAXIS,         m_string = "YAXIS" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ZAXIS,         m_string = "ZAXIS" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_RXAXIS,        m_string = "RXAXIS" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_RYAXIS,        m_string = "RYAXIS" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_RZAXIS,        m_string = "RZAXIS" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_SLIDER1,       m_string = "SLIDER1" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_SLIDER2,       m_string = "SLIDER2" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON1,       m_string = "BUTTON1" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON2,       m_string = "BUTTON2" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON3,       m_string = "BUTTON3" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON4,       m_string = "BUTTON4" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON5,       m_string = "BUTTON5" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON6,       m_string = "BUTTON6" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON7,       m_string = "BUTTON7" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON8,       m_string = "BUTTON8" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON9,       m_string = "BUTTON9" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON10,      m_string = "BUTTON10" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON11,      m_string = "BUTTON11" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON12,      m_string = "BUTTON12" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON13,      m_string = "BUTTON13" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON14,      m_string = "BUTTON14" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON15,      m_string = "BUTTON15" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON16,      m_string = "BUTTON16" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON17,      m_string = "BUTTON17" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON18,      m_string = "BUTTON18" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON19,      m_string = "BUTTON19" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON20,      m_string = "BUTTON20" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON21,      m_string = "BUTTON21" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON22,      m_string = "BUTTON22" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON23,      m_string = "BUTTON23" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON24,      m_string = "BUTTON24" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON25,      m_string = "BUTTON25" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON26,      m_string = "BUTTON26" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON27,      m_string = "BUTTON27" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON28,      m_string = "BUTTON28" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON29,      m_string = "BUTTON29" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON30,      m_string = "BUTTON30" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON31,      m_string = "BUTTON31" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_BUTTON32,      m_string = "BUTTON32" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_START,         m_string = "START" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_SELECT,        m_string = "SELECT" },

            // Hats
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT1UP,        m_string = "HAT1UP" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT1DOWN,      m_string = "HAT1DOWN" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT1LEFT,      m_string = "HAT1LEFT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT1RIGHT,     m_string = "HAT1RIGHT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT2UP,        m_string = "HAT2UP" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT2DOWN,      m_string = "HAT2DOWN" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT2LEFT,      m_string = "HAT2LEFT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT2RIGHT,     m_string = "HAT2RIGHT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT3UP,        m_string = "HAT3UP" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT3DOWN,      m_string = "HAT3DOWN" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT3LEFT,      m_string = "HAT3LEFT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT3RIGHT,     m_string = "HAT3RIGHT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT4UP,        m_string = "HAT4UP" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT4DOWN,      m_string = "HAT4DOWN" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT4LEFT,      m_string = "HAT4LEFT" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_HAT4RIGHT,     m_string = "HAT4RIGHT" },

            // Additional IDs
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH1,   m_string = "ADDSW1" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH2,   m_string = "ADDSW2" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH3,   m_string = "ADDSW3" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH4,   m_string = "ADDSW4" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH5,   m_string = "ADDSW5" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH6,   m_string = "ADDSW6" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH7,   m_string = "ADDSW7" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH8,   m_string = "ADDSW8" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH9,   m_string = "ADDSW9" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH10,  m_string = "ADDSW10" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH11,  m_string = "ADDSW11" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH12,  m_string = "ADDSW12" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH13,  m_string = "ADDSW13" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH14,  m_string = "ADDSW14" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH15,  m_string = "ADDSW15" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_SWITCH16,  m_string = "ADDSW16" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE1, m_string = "ADDAXIS1" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE2, m_string = "ADDAXIS2" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE3, m_string = "ADDAXIS3" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE4, m_string = "ADDAXIS4" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE5, m_string = "ADDAXIS5" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE6, m_string = "ADDAXIS6" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE7, m_string = "ADDAXIS7" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE8, m_string = "ADDAXIS8" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE9, m_string = "ADDAXIS9" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE10,m_string = "ADDAXIS10" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE11,m_string = "ADDAXIS11" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE12,m_string = "ADDAXIS12" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE13,m_string = "ADDAXIS13" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE14,m_string = "ADDAXIS14" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE15,m_string = "ADDAXIS15" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_ABSOLUTE16,m_string = "ADDAXIS16" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE1, m_string = "ADDREL1" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE2, m_string = "ADDREL2" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE3, m_string = "ADDREL3" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE4, m_string = "ADDREL4" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE5, m_string = "ADDREL5" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE6, m_string = "ADDREL6" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE7, m_string = "ADDREL7" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE8, m_string = "ADDREL8" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE9, m_string = "ADDREL9" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE10,m_string = "ADDREL10" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE11,m_string = "ADDREL11" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE12,m_string = "ADDREL12" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE13,m_string = "ADDREL13" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE14,m_string = "ADDREL14" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE15,m_string = "ADDREL15" },
            new code_string_table() { m_code = (UInt32)input_item_id.ITEM_ID_ADD_RELATIVE16,m_string = "ADDREL16" },

            new code_string_table() { m_code = UInt32.MaxValue,                             m_string = null }
        };
    }
}
