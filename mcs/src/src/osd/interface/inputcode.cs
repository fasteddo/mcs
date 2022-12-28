// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using u32 = System.UInt32;

using static mame.cpp_global;
using static mame.inputcode_global;


namespace mame
{
    //**************************************************************************
    //  CONSTANTS
    //**************************************************************************

    // maximum number of axis/buttons/hats with ITEM_IDs for use by osd layer
    //constexpr int INPUT_MAX_AXIS = 8;
    //constexpr int INPUT_MAX_BUTTONS = 32;
    //constexpr int INPUT_MAX_HATS = 4;
    //constexpr int INPUT_MAX_ADD_SWITCH = 16;
    //constexpr int INPUT_MAX_ADD_ABSOLUTE = 16;
    //constexpr int INPUT_MAX_ADD_RELATIVE = 16;


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


    public static partial class inputcode_global
    {
        // device index
        public const int DEVICE_INDEX_MAXIMUM = 0xff;
    }


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
        ITEM_MODIFIER_REVERSE,
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


    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // a combined code that describes a particular input on a particular device
    public class input_code : IComparable<input_code>
    {
        u32 m_internal;


        // construction/destruction
        public input_code(
                input_device_class devclass = input_device_class.DEVICE_CLASS_INVALID,
                int devindex = 0,
                input_item_class itemclass = input_item_class.ITEM_CLASS_INVALID,
                input_item_modifier modifier = input_item_modifier.ITEM_MODIFIER_NONE,
                input_item_id itemid = input_item_id.ITEM_ID_INVALID)
        {
            m_internal = ((((u32)devclass & 0xf) << 28) | (((u32)devindex & 0xff) << 20) | (((u32)itemclass & 0xf) << 16) | (((u32)modifier & 0xf) << 12) | ((u32)itemid & 0xfff));


            assert(devclass >= 0 && devclass < input_device_class.DEVICE_CLASS_MAXIMUM);
            assert(devindex >= 0 && devindex < DEVICE_INDEX_MAXIMUM);
            assert(itemclass >= 0 && itemclass < input_item_class.ITEM_CLASS_MAXIMUM);
            assert(modifier >= 0 && modifier < input_item_modifier.ITEM_MODIFIER_MAXIMUM);
            assert(itemid >= 0 && itemid < input_item_id.ITEM_ID_ABSOLUTE_MAXIMUM);
        }

        //constexpr input_code(const input_code &src) noexcept = default;


        // operators
        public static bool operator==(input_code left, input_code right) { return left.m_internal == right.m_internal; }
        public static bool operator!=(input_code left, input_code right) { return left.m_internal != right.m_internal; }
        //constexpr bool operator<(const input_code &rhs) const noexcept { return m_internal < rhs.m_internal; }
        public override bool Equals(Object obj)
        {
            if (obj == null || base.GetType() != obj.GetType()) return false;
            return this == (input_code)obj;
        }
        public override int GetHashCode() { return m_internal.GetHashCode(); }
        public int CompareTo(input_code other) { return m_internal.CompareTo(other.m_internal); }


        // getters
        public bool internal_() { return device_class() == input_device_class.DEVICE_CLASS_INTERNAL; }
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
            assert(devindex >= 0 && devindex <= 0xff);
            m_internal = (u32)((m_internal & ~(0xff << 20)) | (((u32)devindex & 0xff) << 20));
        }

        //void set_item_class(input_item_class itemclass) noexcept
        //{
        //    assert(itemclass >= 0 && itemclass <= 0xf);
        //    m_internal = (m_internal & ~(0xf << 16)) | ((itemclass & 0xf) << 16);
        //}
        //void set_item_modifier(input_item_modifier modifier) noexcept
        //{
        //    assert(modifier >= 0 && modifier <= 0xf);
        //    m_internal = (m_internal & ~(0xf << 12)) | ((modifier & 0xf) << 12);
        //}
        //void set_item_id(input_item_id itemid) noexcept
        //{
        //    assert(itemid >= 0 && itemid <= 0xfff);
        //    m_internal = (m_internal & ~0xfff) | (itemid & 0xfff);
        //}
    }


    public static partial class inputcode_global
    {
        //**************************************************************************
        //  MACROS
        //**************************************************************************

        // invalid codes
        public static readonly input_code INPUT_CODE_INVALID = new input_code();

        // keyboard codes
        public static input_code KEYCODE_A_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_A); }
        public static input_code KEYCODE_B_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_B); }
        public static input_code KEYCODE_C_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_C); }
        public static input_code KEYCODE_D_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_D); }
        public static input_code KEYCODE_E_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_E); }
        public static input_code KEYCODE_F_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F); }
        public static input_code KEYCODE_G_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_G); }
        public static input_code KEYCODE_H_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_H); }
        public static input_code KEYCODE_I_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_I); }
        public static input_code KEYCODE_J_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_J); }
        public static input_code KEYCODE_K_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_K); }
        public static input_code KEYCODE_L_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_L); }
        public static input_code KEYCODE_M_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_M); }
        public static input_code KEYCODE_N_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_N); }
        public static input_code KEYCODE_O_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_O); }
        public static input_code KEYCODE_P_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_P); }
        public static input_code KEYCODE_Q_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_Q); }
        public static input_code KEYCODE_R_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_R); }
        public static input_code KEYCODE_S_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_S); }
        public static input_code KEYCODE_T_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_T); }
        public static input_code KEYCODE_U_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_U); }
        public static input_code KEYCODE_V_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_V); }
        public static input_code KEYCODE_W_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_W); }
        public static input_code KEYCODE_X_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_X); }
        public static input_code KEYCODE_Y_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_Y); }
        public static input_code KEYCODE_Z_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_Z); }
        public static input_code KEYCODE_0_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_0); }
        public static input_code KEYCODE_1_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_1); }
        public static input_code KEYCODE_2_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_2); }
        public static input_code KEYCODE_3_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_3); }
        public static input_code KEYCODE_4_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_4); }
        public static input_code KEYCODE_5_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_5); }
        public static input_code KEYCODE_6_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_6); }
        public static input_code KEYCODE_7_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_7); }
        public static input_code KEYCODE_8_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_8); }
        public static input_code KEYCODE_9_INDEXED(int n)          { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_9); }
        public static input_code KEYCODE_F1_INDEXED(int n)         { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F1); }
        public static input_code KEYCODE_F2_INDEXED(int n)         { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F2); }
        public static input_code KEYCODE_F3_INDEXED(int n)         { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F3); }
        public static input_code KEYCODE_F4_INDEXED(int n)         { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F4); }
        public static input_code KEYCODE_F5_INDEXED(int n)         { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F5); }
        public static input_code KEYCODE_F6_INDEXED(int n)         { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F6); }
        public static input_code KEYCODE_F7_INDEXED(int n)         { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F7); }
        public static input_code KEYCODE_F8_INDEXED(int n)         { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F8); }
        public static input_code KEYCODE_F9_INDEXED(int n)         { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F9); }
        public static input_code KEYCODE_F10_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F10); }
        public static input_code KEYCODE_F11_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F11); }
        public static input_code KEYCODE_F12_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F12); }
        public static input_code KEYCODE_F13_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F13); }
        public static input_code KEYCODE_F14_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F14); }
        public static input_code KEYCODE_F15_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F15); }
        public static input_code KEYCODE_F16_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F16); }
        public static input_code KEYCODE_F17_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F17); }
        public static input_code KEYCODE_F18_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F18); }
        public static input_code KEYCODE_F19_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F19); }
        public static input_code KEYCODE_F20_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_F20); }
        public static input_code KEYCODE_ESC_INDEXED(int n)        { return new input_code(input_device_class.DEVICE_CLASS_KEYBOARD, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_ESC); }
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
        public static readonly input_code KEYCODE_QUOTE = KEYCODE_QUOTE_INDEXED(0);
        public static readonly input_code KEYCODE_BACKSLASH = KEYCODE_BACKSLASH_INDEXED(0);
        public static readonly input_code KEYCODE_BACKSLASH2 = KEYCODE_BACKSLASH2_INDEXED(0);
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
        public static readonly input_code KEYCODE_NUMLOCK = KEYCODE_NUMLOCK_INDEXED(0);
        public static readonly input_code KEYCODE_CAPSLOCK = KEYCODE_CAPSLOCK_INDEXED(0);
        public static readonly input_code KEYCODE_LWIN = KEYCODE_LWIN_INDEXED(0);
        public static readonly input_code KEYCODE_RWIN = KEYCODE_RWIN_INDEXED(0);
        public static readonly input_code KEYCODE_MENU = KEYCODE_MENU_INDEXED(0);
        public static readonly input_code KEYCODE_CANCEL = KEYCODE_CANCEL_INDEXED(0);

        // mouse axes as relative devices
        public static input_code MOUSECODE_X_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_RELATIVE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_XAXIS); }
        public static input_code MOUSECODE_Y_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_RELATIVE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_YAXIS); }
        public static input_code MOUSECODE_Z_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_RELATIVE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_ZAXIS); }

        public static readonly input_code MOUSECODE_X = MOUSECODE_X_INDEXED(0);
        public static readonly input_code MOUSECODE_Y = MOUSECODE_Y_INDEXED(0);
        public static readonly input_code MOUSECODE_Z = MOUSECODE_Z_INDEXED(0);

        // mouse axes as switches in +/- direction
        public static input_code MOUSECODE_X_POS_SWITCH_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_XAXIS); }
        public static input_code MOUSECODE_X_NEG_SWITCH_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_XAXIS); }
        public static input_code MOUSECODE_Y_POS_SWITCH_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_YAXIS); }
        public static input_code MOUSECODE_Y_NEG_SWITCH_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_YAXIS); }
        public static input_code MOUSECODE_Z_POS_SWITCH_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_ZAXIS); }
        public static input_code MOUSECODE_Z_NEG_SWITCH_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_ZAXIS); }

        public static readonly input_code MOUSECODE_X_POS_SWITCH = MOUSECODE_X_POS_SWITCH_INDEXED(0);
        public static readonly input_code MOUSECODE_X_NEG_SWITCH = MOUSECODE_X_NEG_SWITCH_INDEXED(0);
        public static readonly input_code MOUSECODE_Y_POS_SWITCH = MOUSECODE_Y_POS_SWITCH_INDEXED(0);
        public static readonly input_code MOUSECODE_Y_NEG_SWITCH = MOUSECODE_Y_NEG_SWITCH_INDEXED(0);
        public static readonly input_code MOUSECODE_Z_POS_SWITCH = MOUSECODE_Z_POS_SWITCH_INDEXED(0);
        public static readonly input_code MOUSECODE_Z_NEG_SWITCH = MOUSECODE_Z_NEG_SWITCH_INDEXED(0);

        // mouse buttons
        public static input_code MOUSECODE_BUTTON1_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON1); }
        public static input_code MOUSECODE_BUTTON2_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON2); }
        public static input_code MOUSECODE_BUTTON3_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON3); }
        public static input_code MOUSECODE_BUTTON4_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON4); }
        public static input_code MOUSECODE_BUTTON5_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON5); }
        public static input_code MOUSECODE_BUTTON6_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON6); }
        public static input_code MOUSECODE_BUTTON7_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON7); }
        public static input_code MOUSECODE_BUTTON8_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON8); }

        public static readonly input_code MOUSECODE_BUTTON1 = MOUSECODE_BUTTON1_INDEXED(0);
        public static readonly input_code MOUSECODE_BUTTON2 = MOUSECODE_BUTTON2_INDEXED(0);
        public static readonly input_code MOUSECODE_BUTTON3 = MOUSECODE_BUTTON3_INDEXED(0);
        public static readonly input_code MOUSECODE_BUTTON4 = MOUSECODE_BUTTON4_INDEXED(0);
        public static readonly input_code MOUSECODE_BUTTON5 = MOUSECODE_BUTTON5_INDEXED(0);
        public static readonly input_code MOUSECODE_BUTTON6 = MOUSECODE_BUTTON6_INDEXED(0);
        public static readonly input_code MOUSECODE_BUTTON7 = MOUSECODE_BUTTON7_INDEXED(0);
        public static readonly input_code MOUSECODE_BUTTON8 = MOUSECODE_BUTTON8_INDEXED(0);

        // gun axes as absolute devices
        public static input_code GUNCODE_X_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_XAXIS); }
        public static input_code GUNCODE_Y_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_YAXIS); }

        public static readonly input_code GUNCODE_X = GUNCODE_X_INDEXED(0);
        public static readonly input_code GUNCODE_Y = GUNCODE_Y_INDEXED(0);

        // gun buttons
        public static input_code GUNCODE_BUTTON1_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON1); }
        public static input_code GUNCODE_BUTTON2_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON2); }
        public static input_code GUNCODE_BUTTON3_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON3); }
        public static input_code GUNCODE_BUTTON4_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON4); }
        public static input_code GUNCODE_BUTTON5_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON5); }
        public static input_code GUNCODE_BUTTON6_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON6); }
        public static input_code GUNCODE_BUTTON7_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON7); }
        public static input_code GUNCODE_BUTTON8_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON8); }

        public static readonly input_code GUNCODE_BUTTON1 = GUNCODE_BUTTON1_INDEXED(0);
        public static readonly input_code GUNCODE_BUTTON2 = GUNCODE_BUTTON2_INDEXED(0);
        public static readonly input_code GUNCODE_BUTTON3 = GUNCODE_BUTTON3_INDEXED(0);
        public static readonly input_code GUNCODE_BUTTON4 = GUNCODE_BUTTON4_INDEXED(0);
        public static readonly input_code GUNCODE_BUTTON5 = GUNCODE_BUTTON5_INDEXED(0);
        public static readonly input_code GUNCODE_BUTTON6 = GUNCODE_BUTTON6_INDEXED(0);
        public static readonly input_code GUNCODE_BUTTON7 = GUNCODE_BUTTON7_INDEXED(0);
        public static readonly input_code GUNCODE_BUTTON8 = GUNCODE_BUTTON8_INDEXED(0);

        // joystick axes as absolute devices
        public static input_code JOYCODE_X_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_XAXIS); }
        public static input_code JOYCODE_Y_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_YAXIS); }
        public static input_code JOYCODE_Z_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_ZAXIS); }
        public static input_code JOYCODE_U_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RXAXIS); }
        public static input_code JOYCODE_V_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RYAXIS); }
        public static input_code JOYCODE_W_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RZAXIS); }

        public static readonly input_code JOYCODE_X = JOYCODE_X_INDEXED(0);
        public static readonly input_code JOYCODE_Y = JOYCODE_Y_INDEXED(0);
        public static readonly input_code JOYCODE_Z = JOYCODE_Z_INDEXED(0);
        public static readonly input_code JOYCODE_U = JOYCODE_U_INDEXED(0);
        public static readonly input_code JOYCODE_V = JOYCODE_V_INDEXED(0);
        public static readonly input_code JOYCODE_W = JOYCODE_W_INDEXED(0);

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

        public static readonly input_code JOYCODE_X_POS_ABSOLUTE = JOYCODE_X_POS_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_X_NEG_ABSOLUTE = JOYCODE_X_NEG_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_Y_POS_ABSOLUTE = JOYCODE_Y_POS_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_Y_NEG_ABSOLUTE = JOYCODE_Y_NEG_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_Z_POS_ABSOLUTE = JOYCODE_Z_POS_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_Z_NEG_ABSOLUTE = JOYCODE_Z_NEG_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_U_POS_ABSOLUTE = JOYCODE_U_POS_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_U_NEG_ABSOLUTE = JOYCODE_U_NEG_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_V_POS_ABSOLUTE = JOYCODE_V_POS_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_V_NEG_ABSOLUTE = JOYCODE_V_NEG_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_W_POS_ABSOLUTE = JOYCODE_W_POS_ABSOLUTE_INDEXED(0);
        public static readonly input_code JOYCODE_W_NEG_ABSOLUTE = JOYCODE_W_NEG_ABSOLUTE_INDEXED(0);

        // joystick axes as switches; X/Y are specially handled for left/right/up/down mapping
        public static input_code JOYCODE_X_LEFT_SWITCH_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_LEFT, input_item_id.ITEM_ID_XAXIS); }
        public static input_code JOYCODE_X_RIGHT_SWITCH_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_RIGHT, input_item_id.ITEM_ID_XAXIS); }
        public static input_code JOYCODE_Y_UP_SWITCH_INDEXED(int n)    { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_UP, input_item_id.ITEM_ID_YAXIS); }
        public static input_code JOYCODE_Y_DOWN_SWITCH_INDEXED(int n)  { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_DOWN, input_item_id.ITEM_ID_YAXIS); }
        public static input_code JOYCODE_Z_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_ZAXIS); }
        public static input_code JOYCODE_Z_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_ZAXIS); }
        public static input_code JOYCODE_U_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_RXAXIS); }
        public static input_code JOYCODE_U_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_RXAXIS); }
        public static input_code JOYCODE_V_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_RYAXIS); }
        public static input_code JOYCODE_V_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_RYAXIS); }
        public static input_code JOYCODE_W_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_RZAXIS); }
        public static input_code JOYCODE_W_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_RZAXIS); }

        public static readonly input_code JOYCODE_X_LEFT_SWITCH = JOYCODE_X_LEFT_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_X_RIGHT_SWITCH = JOYCODE_X_RIGHT_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_Y_UP_SWITCH = JOYCODE_Y_UP_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_Y_DOWN_SWITCH = JOYCODE_Y_DOWN_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_Z_POS_SWITCH = JOYCODE_Z_POS_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_Z_NEG_SWITCH = JOYCODE_Z_NEG_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_U_POS_SWITCH = JOYCODE_U_POS_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_U_NEG_SWITCH = JOYCODE_U_NEG_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_V_POS_SWITCH = JOYCODE_V_POS_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_V_NEG_SWITCH = JOYCODE_V_NEG_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_W_POS_SWITCH = JOYCODE_W_POS_SWITCH_INDEXED(0);
        public static readonly input_code JOYCODE_W_NEG_SWITCH = JOYCODE_W_NEG_SWITCH_INDEXED(0);

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

        public static readonly input_code JOYCODE_BUTTON1 = JOYCODE_BUTTON1_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON2 = JOYCODE_BUTTON2_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON3 = JOYCODE_BUTTON3_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON4 = JOYCODE_BUTTON4_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON5 = JOYCODE_BUTTON5_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON6 = JOYCODE_BUTTON6_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON7 = JOYCODE_BUTTON7_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON8 = JOYCODE_BUTTON8_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON9 = JOYCODE_BUTTON9_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON10 = JOYCODE_BUTTON10_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON11 = JOYCODE_BUTTON11_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON12 = JOYCODE_BUTTON12_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON13 = JOYCODE_BUTTON13_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON14 = JOYCODE_BUTTON14_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON15 = JOYCODE_BUTTON15_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON16 = JOYCODE_BUTTON16_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON17 = JOYCODE_BUTTON17_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON18 = JOYCODE_BUTTON18_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON19 = JOYCODE_BUTTON19_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON20 = JOYCODE_BUTTON20_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON21 = JOYCODE_BUTTON21_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON22 = JOYCODE_BUTTON22_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON23 = JOYCODE_BUTTON23_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON24 = JOYCODE_BUTTON24_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON25 = JOYCODE_BUTTON25_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON26 = JOYCODE_BUTTON26_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON27 = JOYCODE_BUTTON27_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON28 = JOYCODE_BUTTON28_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON29 = JOYCODE_BUTTON29_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON30 = JOYCODE_BUTTON30_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON31 = JOYCODE_BUTTON31_INDEXED(0);
        public static readonly input_code JOYCODE_BUTTON32 = JOYCODE_BUTTON32_INDEXED(0);
        public static readonly input_code JOYCODE_START = JOYCODE_START_INDEXED(0);
        public static readonly input_code JOYCODE_SELECT = JOYCODE_SELECT_INDEXED(0);
    }
}
