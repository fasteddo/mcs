// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using char32_t = System.UInt32;
using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using PointerU8 = mame.Pointer<System.Byte>;
using s32 = System.Int32;
using s64 = System.Int64;
using time_t = System.Int64;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;


namespace mame
{
    // input ports support up to 32 bits each
    //typedef u32 ioport_value;


    // I/O port callback function delegates
    public delegate ioport_value ioport_field_read_delegate();  //typedef device_delegate<ioport_value ()> ioport_field_read_delegate;
    public delegate void ioport_field_write_delegate(ioport_field field, u32 param, ioport_value param1, ioport_value param2);  //typedef device_delegate<void (ioport_field &, u32, ioport_value, ioport_value)> ioport_field_write_delegate;
    public delegate float ioport_field_crossmap_delegate(float param);  //typedef device_delegate<float (float)> ioport_field_crossmap_delegate;


    // sequence types for input_port_seq() call
    public enum input_seq_type
    {
        SEQ_TYPE_INVALID = -1,
        SEQ_TYPE_STANDARD = 0,
        SEQ_TYPE_INCREMENT,
        SEQ_TYPE_DECREMENT,
        SEQ_TYPE_TOTAL
    }
    //DECLARE_ENUM_INCDEC_OPERATORS(input_seq_type)


    // crosshair types
    public enum crosshair_axis_t
    {
        CROSSHAIR_AXIS_NONE = 0,
        CROSSHAIR_AXIS_X,
        CROSSHAIR_AXIS_Y
    }


    // groups for input ports
    public enum ioport_group
    {
        IPG_UI = 0,
        IPG_PLAYER1,
        IPG_PLAYER2,
        IPG_PLAYER3,
        IPG_PLAYER4,
        IPG_PLAYER5,
        IPG_PLAYER6,
        IPG_PLAYER7,
        IPG_PLAYER8,
        IPG_PLAYER9,
        IPG_PLAYER10,
        IPG_OTHER,
        IPG_TOTAL_GROUPS,
        IPG_INVALID
    }


    // various input port types
    public enum ioport_type
    {
        // pseudo-port types
        IPT_INVALID = 0,
        IPT_UNUSED,
        IPT_END,
        IPT_UNKNOWN,
        IPT_PORT,
        IPT_DIPSWITCH,
        IPT_CONFIG,

        // start buttons
        IPT_START1,
        IPT_START2,
        IPT_START3,
        IPT_START4,
        IPT_START5,
        IPT_START6,
        IPT_START7,
        IPT_START8,
        IPT_START9,
        IPT_START10,

        // coin slots
        IPT_COIN1,
        IPT_COIN2,
        IPT_COIN3,
        IPT_COIN4,
        IPT_COIN5,
        IPT_COIN6,
        IPT_COIN7,
        IPT_COIN8,
        IPT_COIN9,
        IPT_COIN10,
        IPT_COIN11,
        IPT_COIN12,
        IPT_BILL1,

        // service coin
        IPT_SERVICE1,
        IPT_SERVICE2,
        IPT_SERVICE3,
        IPT_SERVICE4,

        // tilt inputs
        IPT_TILT1,
        IPT_TILT2,
        IPT_TILT3,
        IPT_TILT4,

        // misc other digital inputs
        IPT_POWER_ON,
        IPT_POWER_OFF,
        IPT_SERVICE,
        IPT_TILT,
        IPT_INTERLOCK,
        IPT_MEMORY_RESET,
        IPT_VOLUME_UP,
        IPT_VOLUME_DOWN,
        IPT_START,              // use the numbered start button(s) for coin-ops
        IPT_SELECT,
        IPT_KEYPAD,
        IPT_KEYBOARD,

        // digital joystick inputs
        IPT_DIGITAL_JOYSTICK_FIRST,

            // use IPT_JOYSTICK for panels where the player has one single joystick
            IPT_JOYSTICK_UP,
            IPT_JOYSTICK_DOWN,
            IPT_JOYSTICK_LEFT,
            IPT_JOYSTICK_RIGHT,

            // use IPT_JOYSTICKLEFT and IPT_JOYSTICKRIGHT for dual joystick panels
            IPT_JOYSTICKRIGHT_UP,
            IPT_JOYSTICKRIGHT_DOWN,
            IPT_JOYSTICKRIGHT_LEFT,
            IPT_JOYSTICKRIGHT_RIGHT,
            IPT_JOYSTICKLEFT_UP,
            IPT_JOYSTICKLEFT_DOWN,
            IPT_JOYSTICKLEFT_LEFT,
            IPT_JOYSTICKLEFT_RIGHT,

        IPT_DIGITAL_JOYSTICK_LAST,

        // action buttons
        IPT_BUTTON1,
        IPT_BUTTON2,
        IPT_BUTTON3,
        IPT_BUTTON4,
        IPT_BUTTON5,
        IPT_BUTTON6,
        IPT_BUTTON7,
        IPT_BUTTON8,
        IPT_BUTTON9,
        IPT_BUTTON10,
        IPT_BUTTON11,
        IPT_BUTTON12,
        IPT_BUTTON13,
        IPT_BUTTON14,
        IPT_BUTTON15,
        IPT_BUTTON16,

        // mahjong inputs
        IPT_MAHJONG_FIRST,

            IPT_MAHJONG_A,
            IPT_MAHJONG_B,
            IPT_MAHJONG_C,
            IPT_MAHJONG_D,
            IPT_MAHJONG_E,
            IPT_MAHJONG_F,
            IPT_MAHJONG_G,
            IPT_MAHJONG_H,
            IPT_MAHJONG_I,
            IPT_MAHJONG_J,
            IPT_MAHJONG_K,
            IPT_MAHJONG_L,
            IPT_MAHJONG_M,
            IPT_MAHJONG_N,
            IPT_MAHJONG_O,
            IPT_MAHJONG_P,
            IPT_MAHJONG_Q,
            IPT_MAHJONG_KAN,
            IPT_MAHJONG_PON,
            IPT_MAHJONG_CHI,
            IPT_MAHJONG_REACH,
            IPT_MAHJONG_RON,
            IPT_MAHJONG_FLIP_FLOP,
            IPT_MAHJONG_BET,
            IPT_MAHJONG_SCORE,
            IPT_MAHJONG_DOUBLE_UP,
            IPT_MAHJONG_BIG,
            IPT_MAHJONG_SMALL,
            IPT_MAHJONG_LAST_CHANCE,

        IPT_MAHJONG_LAST,

        // hanafuda inputs
        IPT_HANAFUDA_FIRST,

            IPT_HANAFUDA_A,
            IPT_HANAFUDA_B,
            IPT_HANAFUDA_C,
            IPT_HANAFUDA_D,
            IPT_HANAFUDA_E,
            IPT_HANAFUDA_F,
            IPT_HANAFUDA_G,
            IPT_HANAFUDA_H,
            IPT_HANAFUDA_YES,
            IPT_HANAFUDA_NO,

        IPT_HANAFUDA_LAST,

        // gambling inputs
        IPT_GAMBLING_FIRST,

            IPT_GAMBLE_KEYIN,   // attendant
            IPT_GAMBLE_KEYOUT,  // attendant
            IPT_GAMBLE_SERVICE, // attendant
            IPT_GAMBLE_BOOK,    // attendant
            IPT_GAMBLE_DOOR,    // attendant
        //  IPT_GAMBLE_DOOR2,   // many gambling games have several doors.
        //  IPT_GAMBLE_DOOR3,
        //  IPT_GAMBLE_DOOR4,
        //  IPT_GAMBLE_DOOR5,

            IPT_GAMBLE_PAYOUT,  // player
            IPT_GAMBLE_BET,     // player
            IPT_GAMBLE_DEAL,    // player
            IPT_GAMBLE_STAND,   // player
            IPT_GAMBLE_TAKE,    // player
            IPT_GAMBLE_D_UP,    // player
            IPT_GAMBLE_HALF,    // player
            IPT_GAMBLE_HIGH,    // player
            IPT_GAMBLE_LOW,     // player

            // poker-specific inputs
            IPT_POKER_HOLD1,
            IPT_POKER_HOLD2,
            IPT_POKER_HOLD3,
            IPT_POKER_HOLD4,
            IPT_POKER_HOLD5,
            IPT_POKER_CANCEL,

            // slot-specific inputs
            IPT_SLOT_STOP1,
            IPT_SLOT_STOP2,
            IPT_SLOT_STOP3,
            IPT_SLOT_STOP4,
            IPT_SLOT_STOP_ALL,

        IPT_GAMBLING_LAST,

        // analog inputs
        IPT_ANALOG_FIRST,

            IPT_ANALOG_ABSOLUTE_FIRST,

                IPT_AD_STICK_X,     // absolute // autocenter
                IPT_AD_STICK_Y,     // absolute // autocenter
                IPT_AD_STICK_Z,     // absolute // autocenter
                IPT_PADDLE,         // absolute // autocenter
                IPT_PADDLE_V,       // absolute // autocenter
                IPT_PEDAL,          // absolute // autocenter
                IPT_PEDAL2,         // absolute // autocenter
                IPT_PEDAL3,         // absolute // autocenter
                IPT_LIGHTGUN_X,     // absolute
                IPT_LIGHTGUN_Y,     // absolute
                IPT_POSITIONAL,     // absolute // autocenter if not wraps
                IPT_POSITIONAL_V,   // absolute // autocenter if not wraps

            IPT_ANALOG_ABSOLUTE_LAST,

            IPT_DIAL,           // relative
            IPT_DIAL_V,         // relative
            IPT_TRACKBALL_X,    // relative
            IPT_TRACKBALL_Y,    // relative
            IPT_MOUSE_X,        // relative
            IPT_MOUSE_Y,        // relative

        IPT_ANALOG_LAST,

        // analog adjuster support
        IPT_ADJUSTER,

        // the following are special codes for user interface handling - not to be used by drivers!
        IPT_UI_FIRST,

            IPT_UI_CONFIGURE,
            IPT_UI_ON_SCREEN_DISPLAY,
            IPT_UI_DEBUG_BREAK,
            IPT_UI_PAUSE,
            IPT_UI_PAUSE_SINGLE,
            IPT_UI_REWIND_SINGLE,
            IPT_UI_RESET_MACHINE,
            IPT_UI_SOFT_RESET,
            IPT_UI_SHOW_GFX,
            IPT_UI_FRAMESKIP_DEC,
            IPT_UI_FRAMESKIP_INC,
            IPT_UI_THROTTLE,
            IPT_UI_FAST_FORWARD,
            IPT_UI_SHOW_FPS,
            IPT_UI_SNAPSHOT,
            IPT_UI_TIMECODE,
            IPT_UI_RECORD_MNG,
            IPT_UI_RECORD_AVI,
            IPT_UI_TOGGLE_CHEAT,
            IPT_UI_UP,
            IPT_UI_DOWN,
            IPT_UI_LEFT,
            IPT_UI_RIGHT,
            IPT_UI_HOME,
            IPT_UI_END,
            IPT_UI_PAGE_UP,
            IPT_UI_PAGE_DOWN,
            IPT_UI_FOCUS_NEXT,
            IPT_UI_FOCUS_PREV,
            IPT_UI_SELECT,
            IPT_UI_CANCEL,
            IPT_UI_DISPLAY_COMMENT,
            IPT_UI_CLEAR,
            IPT_UI_ZOOM_IN,
            IPT_UI_ZOOM_OUT,
            IPT_UI_PREV_GROUP,
            IPT_UI_NEXT_GROUP,
            IPT_UI_ROTATE,
            IPT_UI_SHOW_PROFILER,
            IPT_UI_TOGGLE_UI,
            IPT_UI_RELEASE_POINTER,
            IPT_UI_TOGGLE_DEBUG,
            IPT_UI_PASTE,
            IPT_UI_SAVE_STATE,
            IPT_UI_LOAD_STATE,
            IPT_UI_TAPE_START,
            IPT_UI_TAPE_STOP,
            IPT_UI_DATS,
            IPT_UI_FAVORITES,
            IPT_UI_EXPORT,
            IPT_UI_AUDIT_FAST,
            IPT_UI_AUDIT_ALL,

            // additional OSD-specified UI port types (up to 16)
            IPT_OSD_1,
            IPT_OSD_2,
            IPT_OSD_3,
            IPT_OSD_4,
            IPT_OSD_5,
            IPT_OSD_6,
            IPT_OSD_7,
            IPT_OSD_8,
            IPT_OSD_9,
            IPT_OSD_10,
            IPT_OSD_11,
            IPT_OSD_12,
            IPT_OSD_13,
            IPT_OSD_14,
            IPT_OSD_15,
            IPT_OSD_16,

        IPT_UI_LAST,

        IPT_OTHER, // not mapped to standard defaults

        IPT_SPECIAL, // uninterpreted characters
        IPT_CUSTOM, // handled by custom code

        IPT_OUTPUT,

        IPT_COUNT
    }
    //DECLARE_ENUM_INCDEC_OPERATORS(ioport_type)


    // input type classes
    public enum ioport_type_class
    {
        INPUT_CLASS_INTERNAL,
        INPUT_CLASS_KEYBOARD,
        INPUT_CLASS_CONTROLLER,
        INPUT_CLASS_CONFIG,
        INPUT_CLASS_DIPSWITCH,
        INPUT_CLASS_MISC
    }


    // default strings used in port definitions
    public enum INPUT_STRING
    {
        INPUT_STRING_Off = 1,
        INPUT_STRING_On,
        INPUT_STRING_No,
        INPUT_STRING_Yes,
        INPUT_STRING_Lives,
        INPUT_STRING_Bonus_Life,
        INPUT_STRING_Difficulty,
        INPUT_STRING_Demo_Sounds,
        INPUT_STRING_Coinage,
        INPUT_STRING_Coin_A,
        INPUT_STRING_Coin_B,
    //  INPUT_STRING_20C_1C,    //  0.050000
    //  INPUT_STRING_15C_1C,    //  0.066667
    //  INPUT_STRING_10C_1C,    //  0.100000
    //#define __input_string_coinage_start INPUT_STRING_9C_1C
        INPUT_STRING_9C_1C,     //  0.111111
        INPUT_STRING_8C_1C,     //  0.125000
        INPUT_STRING_7C_1C,     //  0.142857
        INPUT_STRING_6C_1C,     //  0.166667
    //  INPUT_STRING_10C_2C,    //  0.200000
        INPUT_STRING_5C_1C,     //  0.200000
    //  INPUT_STRING_9C_2C,     //  0.222222
    //  INPUT_STRING_8C_2C,     //  0.250000
        INPUT_STRING_4C_1C,     //  0.250000
    //  INPUT_STRING_7C_2C,     //  0.285714
    //  INPUT_STRING_10C_3C,    //  0.300000
    //  INPUT_STRING_9C_3C,     //  0.333333
    //  INPUT_STRING_6C_2C,     //  0.333333
        INPUT_STRING_3C_1C,     //  0.333333
        INPUT_STRING_8C_3C,     //  0.375000
    //  INPUT_STRING_10C_4C,    //  0.400000
    //  INPUT_STRING_7C_3C,     //  0.428571
    //  INPUT_STRING_9C_4C,     //  0.444444
    //  INPUT_STRING_10C_5C,    //  0.500000
    //  INPUT_STRING_8C_4C,     //  0.500000
    //  INPUT_STRING_6C_3C,     //  0.500000
        INPUT_STRING_4C_2C,     //  0.500000
        INPUT_STRING_5C_2C,     //  0.500000
        INPUT_STRING_2C_1C,     //  0.500000
    //  INPUT_STRING_9C_5C,     //  0.555556
    //  INPUT_STRING_7C_4C,     //  0.571429
    //  INPUT_STRING_10C_6C,    //  0.600000
        INPUT_STRING_5C_3C,     //  0.600000
    //  INPUT_STRING_8C_5C,     //  0.625000
    //  INPUT_STRING_9C_6C,     //  0.666667
    //  INPUT_STRING_6C_4C,     //  0.666667
        INPUT_STRING_3C_2C,     //  0.666667
    //  INPUT_STRING_10C_7C,    //  0.700000
    //  INPUT_STRING_7C_5C,     //  0.714286
    //  INPUT_STRING_8C_6C,     //  0.750000
        INPUT_STRING_4C_3C,     //  0.750000
    //  INPUT_STRING_9C_7C,     //  0.777778
    //  INPUT_STRING_10C_8C,    //  0.800000
    //  INPUT_STRING_5C_4C,     //  0.800000
    //  INPUT_STRING_6C_5C,     //  0.833333
    //  INPUT_STRING_7C_6C,     //  0.857143
    //  INPUT_STRING_8C_7C,     //  0.875000
    //  INPUT_STRING_9C_8C,     //  0.888889
    //  INPUT_STRING_10C_9C,    //  0.900000
    //  INPUT_STRING_10C_10C,   //  1.000000
    //  INPUT_STRING_9C_9C,     //  1.000000
    //  INPUT_STRING_8C_8C,     //  1.000000
    //  INPUT_STRING_7C_7C,     //  1.000000
    //  INPUT_STRING_6C_6C,     //  1.000000
    //  INPUT_STRING_5C_5C,     //  1.000000
        INPUT_STRING_4C_4C,     //  1.000000
        INPUT_STRING_3C_3C,     //  1.000000
        INPUT_STRING_2C_2C,     //  1.000000
        INPUT_STRING_1C_1C,     //  1.000000
    //  INPUT_STRING_9C_10C,    //  1.111111
    //  INPUT_STRING_8C_9C,     //  1.125000
    //  INPUT_STRING_7C_8C,     //  1.142857
    //  INPUT_STRING_6C_7C,     //  1.166667
    //  INPUT_STRING_5C_6C,     //  1.200000
    //  INPUT_STRING_8C_10C,    //  1.250000
        INPUT_STRING_3C_5C,     //  1.250000
        INPUT_STRING_4C_5C,     //  1.250000
    //  INPUT_STRING_7C_9C,     //  1.285714
    //  INPUT_STRING_6C_8C,     //  1.333333
        INPUT_STRING_3C_4C,     //  1.333333
    //  INPUT_STRING_5C_7C,     //  1.400000
    //  INPUT_STRING_7C_10C,    //  1.428571
    //  INPUT_STRING_6C_9C,     //  1.500000
    //  INPUT_STRING_4C_6C,     //  1.500000
        INPUT_STRING_2C_3C,     //  1.500000
    //  INPUT_STRING_5C_8C,     //  1.600000
    //  INPUT_STRING_6C_10C,    //  1.666667
    //  INPUT_STRING_3C_5C,     //  1.666667
        INPUT_STRING_4C_7C,     //  1.750000
    //  INPUT_STRING_5C_9C,     //  1.800000
    //  INPUT_STRING_5C_10C,    //  2.000000
    //  INPUT_STRING_4C_8C,     //  2.000000
    //  INPUT_STRING_3C_6C,     //  2.000000
        INPUT_STRING_2C_4C,     //  2.000000
        INPUT_STRING_1C_2C,     //  2.000000
    //  INPUT_STRING_4C_9C,     //  2.250000
    //  INPUT_STRING_3C_7C,     //  2.333333
    //  INPUT_STRING_4C_10C,    //  2.500000
        INPUT_STRING_2C_5C,     //  2.500000
    //  INPUT_STRING_3C_8C,     //  2.666667
    //  INPUT_STRING_3C_9C,     //  3.000000
        INPUT_STRING_2C_6C,     //  3.000000
        INPUT_STRING_1C_3C,     //  3.000000
    //  INPUT_STRING_3C_10C,    //  3.333333
        INPUT_STRING_2C_7C,     //  3.500000
        INPUT_STRING_2C_8C,     //  4.000000
        INPUT_STRING_1C_4C,     //  4.000000
    //  INPUT_STRING_2C_9C,     //  4.500000
    //  INPUT_STRING_2C_10C,    //  5.000000
        INPUT_STRING_1C_5C,     //  5.000000
        INPUT_STRING_1C_6C,     //  6.000000
        INPUT_STRING_1C_7C,     //  7.000000
        INPUT_STRING_1C_8C,     //  8.000000
        INPUT_STRING_1C_9C,     //  9.000000
    //#define __input_string_coinage_end INPUT_STRING_1C_9C
    //  INPUT_STRING_1C_10C,    //  10.000000
    //  INPUT_STRING_1C_11C,    //  11.000000
    //  INPUT_STRING_1C_12C,    //  12.000000
    //  INPUT_STRING_1C_13C,    //  13.000000
    //  INPUT_STRING_1C_14C,    //  14.000000
    //  INPUT_STRING_1C_15C,    //  15.000000
    //  INPUT_STRING_1C_20C,    //  20.000000
    //  INPUT_STRING_1C_25C,    //  25.000000
    //  INPUT_STRING_1C_30C,    //  30.000000
    //  INPUT_STRING_1C_40C,    //  40.000000
    //  INPUT_STRING_1C_50C,    //  50.000000
    //  INPUT_STRING_1C_99C,    //  99.000000
    //  INPUT_STRING_1C_100C,   //  100.000000
    //  INPUT_STRING_1C_120C,   //  120.000000
    //  INPUT_STRING_1C_125C,   //  125.000000
    //  INPUT_STRING_1C_150C,   //  150.000000
    //  INPUT_STRING_1C_200C,   //  200.000000
    //  INPUT_STRING_1C_250C,   //  250.000000
    //  INPUT_STRING_1C_500C,   //  500.000000
    //  INPUT_STRING_1C_1000C,  //  1000.000000
        INPUT_STRING_Free_Play,
        INPUT_STRING_Cabinet,
        INPUT_STRING_Upright,
        INPUT_STRING_Cocktail,
        INPUT_STRING_Flip_Screen,
        INPUT_STRING_Service_Mode,
        INPUT_STRING_Pause,
        INPUT_STRING_Test,
        INPUT_STRING_Tilt,
        INPUT_STRING_Version,
        INPUT_STRING_Region,
        INPUT_STRING_International,
        INPUT_STRING_Japan,
        INPUT_STRING_USA,
        INPUT_STRING_Europe,
        INPUT_STRING_Asia,
        INPUT_STRING_China,
        INPUT_STRING_Hong_Kong,
        INPUT_STRING_Korea,
        INPUT_STRING_Southeast_Asia,
        INPUT_STRING_Taiwan,
        INPUT_STRING_World,
        INPUT_STRING_Language,
        INPUT_STRING_English,
        INPUT_STRING_Japanese,
        INPUT_STRING_Chinese,
        INPUT_STRING_French,
        INPUT_STRING_German,
        INPUT_STRING_Italian,
        INPUT_STRING_Korean,
        INPUT_STRING_Spanish,
        INPUT_STRING_Very_Easy,
        INPUT_STRING_Easiest,
        INPUT_STRING_Easier,
        INPUT_STRING_Easy,
        INPUT_STRING_Medium_Easy,
        INPUT_STRING_Normal,
        INPUT_STRING_Medium,
        INPUT_STRING_Medium_Hard,
        INPUT_STRING_Hard,
        INPUT_STRING_Harder,
        INPUT_STRING_Hardest,
        INPUT_STRING_Very_Hard,
        INPUT_STRING_Medium_Difficult,
        INPUT_STRING_Difficult,
        INPUT_STRING_Very_Difficult,
        INPUT_STRING_Very_Low,
        INPUT_STRING_Low,
        INPUT_STRING_High,
        INPUT_STRING_Higher,
        INPUT_STRING_Highest,
        INPUT_STRING_Very_High,
        INPUT_STRING_Players,
        INPUT_STRING_Controls,
        INPUT_STRING_Dual,
        INPUT_STRING_Single,
        INPUT_STRING_Game_Time,
        INPUT_STRING_Continue_Price,
        INPUT_STRING_Controller,
        INPUT_STRING_Light_Gun,
        INPUT_STRING_Joystick,
        INPUT_STRING_Trackball,
        INPUT_STRING_Continues,
        INPUT_STRING_Allow_Continue,
        INPUT_STRING_Level_Select,
    //  INPUT_STRING_Allow,
    //  INPUT_STRING_Forbid,
    //  INPUT_STRING_Enable,
    //  INPUT_STRING_Disable,
        INPUT_STRING_Infinite,
    //  INPUT_STRING_Invincibility,
    //  INPUT_STRING_Invulnerability,
        INPUT_STRING_Stereo,
        INPUT_STRING_Mono,
        INPUT_STRING_Unused,
        INPUT_STRING_Unknown,
    //  INPUT_STRING_Undefined,
        INPUT_STRING_Standard,
        INPUT_STRING_Reverse,
        INPUT_STRING_Alternate,
    //  INPUT_STRING_Reserve,
    //  INPUT_STRING_Spare,
    //  INPUT_STRING_Invalid,
        INPUT_STRING_None,

        INPUT_STRING_COUNT
    };


    public delegate void ioport_constructor(device_t owner, ioport_list portlist, ref string errorbuf); // possibly out


    public static class ioport_global
    {
        // active high/low values for input ports
        public const ioport_value IP_ACTIVE_HIGH = 0x00000000;
        public const ioport_value IP_ACTIVE_LOW = 0xffffffff;

        // maximum number of players supported
        public const int MAX_PLAYERS = 10;

        // unicode constants
        public const char32_t UCHAR_PRIVATE = 0x100000;
        public const char32_t UCHAR_SHIFT_1 = UCHAR_PRIVATE + 0;
        public const char32_t UCHAR_SHIFT_2 = UCHAR_PRIVATE + 1;
        public const char32_t UCHAR_SHIFT_BEGIN = UCHAR_SHIFT_1;
        public const char32_t UCHAR_SHIFT_END = UCHAR_SHIFT_2;
        public const char32_t UCHAR_MAMEKEY_BEGIN = UCHAR_PRIVATE + 2;


        public static readonly Dictionary<INPUT_STRING, string> input_port_default_strings = new Dictionary<INPUT_STRING, string>()
        {
            { INPUT_STRING.INPUT_STRING_Off, "Off" },
            { INPUT_STRING.INPUT_STRING_On, "On" },
            { INPUT_STRING.INPUT_STRING_No, "No" },
            { INPUT_STRING.INPUT_STRING_Yes, "Yes" },
            { INPUT_STRING.INPUT_STRING_Lives, "Lives" },
            { INPUT_STRING.INPUT_STRING_Bonus_Life, "Bonus Life" },
            { INPUT_STRING.INPUT_STRING_Difficulty, "Difficulty" },
            { INPUT_STRING.INPUT_STRING_Demo_Sounds, "Demo Sounds" },
            { INPUT_STRING.INPUT_STRING_Coinage, "Coinage" },
            { INPUT_STRING.INPUT_STRING_Coin_A, "Coin A" },
            { INPUT_STRING.INPUT_STRING_Coin_B, "Coin B" },
            { INPUT_STRING.INPUT_STRING_9C_1C, "9 Coins/1 Credit" },
            { INPUT_STRING.INPUT_STRING_8C_1C, "8 Coins/1 Credit" },
            { INPUT_STRING.INPUT_STRING_7C_1C, "7 Coins/1 Credit" },
            { INPUT_STRING.INPUT_STRING_6C_1C, "6 Coins/1 Credit" },
            { INPUT_STRING.INPUT_STRING_5C_1C, "5 Coins/1 Credit" },
            { INPUT_STRING.INPUT_STRING_4C_1C, "4 Coins/1 Credit" },
            { INPUT_STRING.INPUT_STRING_3C_1C, "3 Coins/1 Credit" },
            { INPUT_STRING.INPUT_STRING_8C_3C, "8 Coins/3 Credits" },
            { INPUT_STRING.INPUT_STRING_4C_2C, "4 Coins/2 Credits" },
            { INPUT_STRING.INPUT_STRING_5C_2C, "5 Coins/2 Credits" },
            { INPUT_STRING.INPUT_STRING_2C_1C, "2 Coins/1 Credit" },
            { INPUT_STRING.INPUT_STRING_5C_3C, "5 Coins/3 Credits" },
            { INPUT_STRING.INPUT_STRING_3C_2C, "3 Coins/2 Credits" },
            { INPUT_STRING.INPUT_STRING_4C_3C, "4 Coins/3 Credits" },
            { INPUT_STRING.INPUT_STRING_4C_4C, "4 Coins/4 Credits" },
            { INPUT_STRING.INPUT_STRING_3C_3C, "3 Coins/3 Credits" },
            { INPUT_STRING.INPUT_STRING_2C_2C, "2 Coins/2 Credits" },
            { INPUT_STRING.INPUT_STRING_1C_1C, "1 Coin/1 Credit" },
            { INPUT_STRING.INPUT_STRING_3C_5C, "3 Coins/5 Credits" },
            { INPUT_STRING.INPUT_STRING_4C_5C, "4 Coins/5 Credits" },
            { INPUT_STRING.INPUT_STRING_3C_4C, "3 Coins/4 Credits" },
            { INPUT_STRING.INPUT_STRING_2C_3C, "2 Coins/3 Credits" },
            { INPUT_STRING.INPUT_STRING_4C_7C, "4 Coins/7 Credits" },
            { INPUT_STRING.INPUT_STRING_2C_4C, "2 Coins/4 Credits" },
            { INPUT_STRING.INPUT_STRING_1C_2C, "1 Coin/2 Credits" },
            { INPUT_STRING.INPUT_STRING_2C_5C, "2 Coins/5 Credits" },
            { INPUT_STRING.INPUT_STRING_2C_6C, "2 Coins/6 Credits" },
            { INPUT_STRING.INPUT_STRING_1C_3C, "1 Coin/3 Credits" },
            { INPUT_STRING.INPUT_STRING_2C_7C, "2 Coins/7 Credits" },
            { INPUT_STRING.INPUT_STRING_2C_8C, "2 Coins/8 Credits" },
            { INPUT_STRING.INPUT_STRING_1C_4C, "1 Coin/4 Credits" },
            { INPUT_STRING.INPUT_STRING_1C_5C, "1 Coin/5 Credits" },
            { INPUT_STRING.INPUT_STRING_1C_6C, "1 Coin/6 Credits" },
            { INPUT_STRING.INPUT_STRING_1C_7C, "1 Coin/7 Credits" },
            { INPUT_STRING.INPUT_STRING_1C_8C, "1 Coin/8 Credits" },
            { INPUT_STRING.INPUT_STRING_1C_9C, "1 Coin/9 Credits" },
            { INPUT_STRING.INPUT_STRING_Free_Play, "Free Play" },
            { INPUT_STRING.INPUT_STRING_Cabinet, "Cabinet" },
            { INPUT_STRING.INPUT_STRING_Upright, "Upright" },
            { INPUT_STRING.INPUT_STRING_Cocktail, "Cocktail" },
            { INPUT_STRING.INPUT_STRING_Flip_Screen, "Flip Screen" },
            { INPUT_STRING.INPUT_STRING_Service_Mode, "Service Mode" },
            { INPUT_STRING.INPUT_STRING_Pause, "Pause" },
            { INPUT_STRING.INPUT_STRING_Test, "Test" },
            { INPUT_STRING.INPUT_STRING_Tilt, "Tilt" },
            { INPUT_STRING.INPUT_STRING_Version, "Version" },
            { INPUT_STRING.INPUT_STRING_Region, "Region" },
            { INPUT_STRING.INPUT_STRING_International, "International" },
            { INPUT_STRING.INPUT_STRING_Japan, "Japan" },
            { INPUT_STRING.INPUT_STRING_USA, "USA" },
            { INPUT_STRING.INPUT_STRING_Europe, "Europe" },
            { INPUT_STRING.INPUT_STRING_Asia, "Asia" },
            { INPUT_STRING.INPUT_STRING_China, "China" },
            { INPUT_STRING.INPUT_STRING_Hong_Kong, "Hong Kong" },
            { INPUT_STRING.INPUT_STRING_Korea, "Korea" },
            { INPUT_STRING.INPUT_STRING_Southeast_Asia, "Southeast Asia" },
            { INPUT_STRING.INPUT_STRING_Taiwan, "Taiwan" },
            { INPUT_STRING.INPUT_STRING_World, "World" },
            { INPUT_STRING.INPUT_STRING_Language, "Language" },
            { INPUT_STRING.INPUT_STRING_English, "English" },
            { INPUT_STRING.INPUT_STRING_Japanese, "Japanese" },
            { INPUT_STRING.INPUT_STRING_Chinese, "Chinese" },
            { INPUT_STRING.INPUT_STRING_French, "French" },
            { INPUT_STRING.INPUT_STRING_German, "German" },
            { INPUT_STRING.INPUT_STRING_Italian, "Italian" },
            { INPUT_STRING.INPUT_STRING_Korean, "Korean" },
            { INPUT_STRING.INPUT_STRING_Spanish, "Spanish" },
            { INPUT_STRING.INPUT_STRING_Very_Easy, "Very Easy" },
            { INPUT_STRING.INPUT_STRING_Easiest, "Easiest" },
            { INPUT_STRING.INPUT_STRING_Easier, "Easier" },
            { INPUT_STRING.INPUT_STRING_Easy, "Easy" },
            { INPUT_STRING.INPUT_STRING_Medium_Easy, "Medium Easy" },
            { INPUT_STRING.INPUT_STRING_Normal, "Normal" },
            { INPUT_STRING.INPUT_STRING_Medium, "Medium" },
            { INPUT_STRING.INPUT_STRING_Medium_Hard, "Medium Hard" },
            { INPUT_STRING.INPUT_STRING_Hard, "Hard" },
            { INPUT_STRING.INPUT_STRING_Harder, "Harder" },
            { INPUT_STRING.INPUT_STRING_Hardest, "Hardest" },
            { INPUT_STRING.INPUT_STRING_Very_Hard, "Very Hard" },
            { INPUT_STRING.INPUT_STRING_Medium_Difficult, "Medium Difficult" },
            { INPUT_STRING.INPUT_STRING_Difficult, "Difficult" },
            { INPUT_STRING.INPUT_STRING_Very_Difficult, "Very Difficult" },
            { INPUT_STRING.INPUT_STRING_Very_Low, "Very Low" },
            { INPUT_STRING.INPUT_STRING_Low, "Low" },
            { INPUT_STRING.INPUT_STRING_High, "High" },
            { INPUT_STRING.INPUT_STRING_Higher, "Higher" },
            { INPUT_STRING.INPUT_STRING_Highest, "Highest" },
            { INPUT_STRING.INPUT_STRING_Very_High, "Very High" },
            { INPUT_STRING.INPUT_STRING_Players, "Players" },
            { INPUT_STRING.INPUT_STRING_Controls, "Controls" },
            { INPUT_STRING.INPUT_STRING_Dual, "Dual" },
            { INPUT_STRING.INPUT_STRING_Single, "Single" },
            { INPUT_STRING.INPUT_STRING_Game_Time, "Game Time" },
            { INPUT_STRING.INPUT_STRING_Continue_Price, "Continue Price" },
            { INPUT_STRING.INPUT_STRING_Controller, "Controller" },
            { INPUT_STRING.INPUT_STRING_Light_Gun, "Light Gun" },
            { INPUT_STRING.INPUT_STRING_Joystick, "Joystick" },
            { INPUT_STRING.INPUT_STRING_Trackball, "Trackball" },
            { INPUT_STRING.INPUT_STRING_Continues, "Continues" },
            { INPUT_STRING.INPUT_STRING_Allow_Continue, "Allow Continue" },
            { INPUT_STRING.INPUT_STRING_Level_Select, "Level Select" },
            { INPUT_STRING.INPUT_STRING_Infinite, "Infinite" },
            { INPUT_STRING.INPUT_STRING_Stereo, "Stereo" },
            { INPUT_STRING.INPUT_STRING_Mono, "Mono" },
            { INPUT_STRING.INPUT_STRING_Unused, "Unused" },
            { INPUT_STRING.INPUT_STRING_Unknown, "Unknown" },
            { INPUT_STRING.INPUT_STRING_Standard, "Standard" },
            { INPUT_STRING.INPUT_STRING_Reverse, "Reverse" },
            { INPUT_STRING.INPUT_STRING_Alternate, "Alternate" },
            { INPUT_STRING.INPUT_STRING_None, "None" },
        };


        // temporary: set this to 1 to enable the originally defined behavior that
        // a field specified via PORT_MODIFY which intersects a previously-defined
        // field completely wipes out the previous definition
        public const bool INPUT_PORT_OVERRIDE_FULLY_NUKES_PREVIOUS    = true;


        public const int SPACE_COUNT = 3;


        //**************************************************************************
        //  MACROS FOR BUILDING INPUT PORTS
        //**************************************************************************

        // so that "0" can be used for unneeded input ports
        //#define construct_ioport_0 NULL

        // name of table
        //#define INPUT_PORTS_NAME(_name) construct_ioport_##_name

        // start of table
        //define INPUT_PORTS_START(_name)         ATTR_COLD void INPUT_PORTS_NAME(_name)(device_t &owner, ioport_list &portlist, astring &errorbuf)         {             ioport_configurer configurer(owner, portlist, errorbuf);
        // end of table
        //define INPUT_PORTS_END         }

        // aliasing
        //define INPUT_PORTS_EXTERN(_name)             extern void INPUT_PORTS_NAME(_name)(device_t &owner, ioport_list &portlist, astring &errorbuf)

        // including
        public static void PORT_INCLUDE(ioport_constructor name, device_t owner, ioport_list portlist, ref string errorbuf) { name(owner, portlist, ref errorbuf); }  //INPUT_PORTS_NAME(_name)(owner, portlist, errorbuf);
        // start of a new input port (with included tag)
        public static void PORT_START(ioport_configurer configurer, string tag) { configurer.port_alloc(tag); }
        // modify an existing port
        public static void PORT_MODIFY(ioport_configurer configurer, string tag) { configurer.port_modify(tag); }
        // input bit definition
        public static void PORT_BIT(ioport_configurer configurer, ioport_value mask, ioport_value defval, ioport_type type) { configurer.field_alloc(type, defval, mask); }
        public static void PORT_SPECIAL_ONOFF(ioport_configurer configurer, ioport_value mask, ioport_value defval, INPUT_STRING strindex) { PORT_SPECIAL_ONOFF_DIPLOC(configurer, mask, defval, strindex, null); }

        static void PORT_SPECIAL_ONOFF_DIPLOC(ioport_configurer configurer, ioport_value mask, ioport_value defval, INPUT_STRING strindex, string diploc) { configurer.onoff_alloc(DEF_STR(strindex), defval, mask, diploc); }
        // append a code
        public static void PORT_CODE(ioport_configurer configurer, input_code code) { configurer.field_add_code(input_seq_type.SEQ_TYPE_STANDARD, code); }
        //define PORT_CODE_DEC(_code)             configurer.field_add_code(SEQ_TYPE_DECREMENT, _code);
        //define PORT_CODE_INC(_code)             configurer.field_add_code(SEQ_TYPE_INCREMENT, _code);

        // joystick flags
        public static void PORT_2WAY(ioport_configurer configurer) { configurer.field_set_way(2); }
        public static void PORT_4WAY(ioport_configurer configurer) { configurer.field_set_way(4); }
        public static void PORT_8WAY(ioport_configurer configurer) { configurer.field_set_way(8); }
        //define PORT_16WAY             configurer.field_set_way(16);
        //define PORT_ROTATED             configurer.field_set_rotated();

        // general flags
        public static void PORT_NAME(ioport_configurer configurer, string _name) { configurer.field_set_name(_name); }
        public static void PORT_PLAYER(ioport_configurer configurer, int player) { configurer.field_set_player(player); }
        public static void PORT_COCKTAIL(ioport_configurer configurer) { configurer.field_set_cocktail(); }
        //define PORT_TOGGLE             configurer.field_set_toggle();
        public static void PORT_IMPULSE(ioport_configurer configurer, u8 duration) { configurer.field_set_impulse(duration); }
        public static void PORT_REVERSE(ioport_configurer configurer) { configurer.field_set_analog_reverse(); }
        //define PORT_RESET             configurer.field_set_analog_reset();
        //#define PORT_OPTIONAL     configurer.field_set_optional();

        // analog settings
        // if this macro is not used, the minimum defaults to 0 and maximum defaults to the mask value
        public static void PORT_MINMAX(ioport_configurer configurer, ioport_value _min, ioport_value _max) { configurer.field_set_min_max(_min, _max); }
        public static void PORT_SENSITIVITY(ioport_configurer configurer, int sensitivity) { configurer.field_set_sensitivity(sensitivity); }
        public static void PORT_KEYDELTA(ioport_configurer configurer, int delta) { configurer.field_set_delta(delta); }
        // note that PORT_CENTERDELTA must appear after PORT_KEYDELTA
        //define PORT_CENTERDELTA(_delta)             configurer.field_set_centerdelta(_delta);
        //define PORT_CROSSHAIR(axis, scale, offset, altaxis)             configurer.field_set_crosshair(CROSSHAIR_AXIS_##axis, altaxis, scale, offset);
        //define PORT_CROSSHAIR_MAPPER(_callback)             configurer.field_set_crossmapper(ioport_field_crossmap_delegate(_callback, #_callback, DEVICE_SELF, (device_t *)NULL));
        //define PORT_CROSSHAIR_MAPPER_MEMBER(_device, _class, _member)             configurer.field_set_crossmapper(ioport_field_crossmap_delegate(&_class::_member, #_class "::" #_member, _device, (_class *)NULL));

        // how many optical counts for 1 full turn of the control
        public static void PORT_FULL_TURN_COUNT(ioport_configurer configurer, u16 _count) { configurer.field_set_full_turn_count(_count); }

        // positional controls can be binary or 1 of X
        // 1 of X not completed yet
        // if it is specified as PORT_REMAP_TABLE then it is binary, but remapped
        // otherwise it is binary
        //define PORT_POSITIONS(_positions)             configurer.field_set_min_max(0, _positions);

        // positional control wraps at min/max
        //define PORT_WRAPS             configurer.field_set_analog_wraps();

        // positional control uses this remap table
        //define PORT_REMAP_TABLE(_table)             configurer.field_set_remap_table(_table);

        // positional control bits are active low
        //define PORT_INVERT             configurer.field_set_analog_invert();

        // read callbacks
        public static void PORT_CUSTOM_MEMBER(ioport_configurer configurer, string device, ioport_field_read_delegate callback) { configurer.field_set_dynamic_read(callback); }  //#define PORT_CUSTOM_MEMBER(_class, _member) configurer.field_set_dynamic_read(ioport_field_read_delegate(&_class::_member, #_class "::" #_member, DEVICE_SELF, (_class *)nullptr));
        //#define PORT_CUSTOM_DEVICE_MEMBER(_device, _class, _member) configurer.field_set_dynamic_read(ioport_field_read_delegate(owner, _device, &_class::_member, #_class "::" #_member));

        // write callbacks
        //#define PORT_CHANGED_MEMBER(_device, _class, _member, _param) configurer.field_set_dynamic_write(ioport_field_write_delegate(owner, _device, &_class::_member, #_class "::" #_member), (_param));

        // input device handler
        //#define PORT_READ_LINE_MEMBER(_class, _member) \
        //    configurer.field_set_dynamic_read( \
        //            ioport_field_read_delegate( \
        //                owner, \
        //                DEVICE_SELF, \
        //                static_cast<ioport_value (*)(_class &)>([] (_class &device) -> ioport_value { return (device._member() & 1) ? ~ioport_value(0) : 0; }), \
        //                #_class "::" #_member));
        //#define PORT_READ_LINE_DEVICE_MEMBER(_device, _class, _member) \
        //    configurer.field_set_dynamic_read( \
        //            ioport_field_read_delegate( \
        //                owner, \
        //                _device, \
        //                static_cast<ioport_value (*)(_class &)>([] (_class &device) -> ioport_value { return (device._member() & 1) ? ~ioport_value(0) : 0; }), \
        //                #_class "::" #_member));
        public static void PORT_READ_LINE_DEVICE_MEMBER(ioport_configurer configurer, string device, Func<int> _member)
        {
            configurer.field_set_dynamic_read(() =>
            {
                return (_member() & 1) != 0 ? ~(ioport_value)0 : 0;
            });
        }

        // output device handler
        //#define PORT_WRITE_LINE_MEMBER(_class, _member) \
        //    configurer.field_set_dynamic_write( \
        //            ioport_field_write_delegate( \
        //                owner, \
        //                DEVICE_SELF, \
        //                static_cast<void (*)(_class &, ioport_field &, u32, ioport_value, ioport_value)>([] (_class &device, ioport_field &field, u32 param, ioport_value oldval, ioport_value newval) { device._member(newval); }), \
        //                #_class "::" #_member));
        //#define PORT_WRITE_LINE_DEVICE_MEMBER(_device, _class, _member) \
        //    configurer.field_set_dynamic_write( \
        //            ioport_field_write_delegate( \
        //                owner, \
        //                _device, \
        //                static_cast<void (*)(_class &, ioport_field &, u32, ioport_value, ioport_value)>([] (_class &device, ioport_field &field, u32 param, ioport_value oldval, ioport_value newval) { device._member(newval); }), \
        //                #_class "::" #_member));

        // dip switch definition
        public static void PORT_DIPNAME(ioport_configurer configurer, ioport_value mask, ioport_value default_, string name) { configurer.field_alloc(ioport_type.IPT_DIPSWITCH, default_, mask, name); }
        public static void PORT_DIPSETTING(ioport_configurer configurer, ioport_value default_, string name) { configurer.setting_alloc(default_, name); }
        // physical location, of the form: name:[!]sw,[name:][!]sw,...
        // note that these are specified LSB-first
        public static void PORT_DIPLOCATION(ioport_configurer configurer, string location) { configurer.field_set_diplocation(location); }
        // conditionals for dip switch settings
        public static void PORT_CONDITION(ioport_configurer configurer, string tag, ioport_value mask, ioport_condition.condition_t condition, ioport_value value) { configurer.set_condition(condition, tag, mask, value); }
        // analog adjuster definition
        public static void PORT_ADJUSTER(ioport_configurer configurer, ioport_value default_, string name) { configurer.field_alloc(ioport_type.IPT_ADJUSTER, default_, 0xff, name);  configurer.field_set_min_max(0, 100); }
        // config definition
        public static void PORT_CONFNAME(ioport_configurer configurer, ioport_value mask, ioport_value default_, string name) { configurer.field_alloc(ioport_type.IPT_CONFIG, default_, mask, name); }
        public static void PORT_CONFSETTING(ioport_configurer configurer, ioport_value default_, string name) { configurer.setting_alloc(default_, name); }

        // keyboard chars
        //define PORT_CHAR(...)     configurer.field_add_char({ __VA_ARGS__ });


        // name of table
        //#define DEVICE_INPUT_DEFAULTS_NAME(_name) device_iptdef_##_name

        //#define device_iptdef_0 NULL
        //#define device_iptdef_0L NULL
        //#define device_iptdef_0LL NULL
        //#define device_iptdef___null NULL

        // start of table
        //define DEVICE_INPUT_DEFAULTS_START(_name)             const input_device_default DEVICE_INPUT_DEFAULTS_NAME(_name)[] = {
        // end of table
        //define DEVICE_INPUT_DEFAULTS(_tag,_mask,_defval)             { _tag ,_mask, _defval },
        // end of table
        //define DEVICE_INPUT_DEFAULTS_END             {NULL,0,0} };



        //**************************************************************************
        //  HELPER MACROS
        //**************************************************************************

        public static void PORT_DIPUNUSED_DIPLOC(ioport_configurer configurer, ioport_value mask, ioport_value default_, string diploc) { PORT_SPECIAL_ONOFF_DIPLOC(configurer, mask, default_, INPUT_STRING.INPUT_STRING_Unused, diploc); }
        public static void PORT_DIPUNUSED(ioport_configurer configurer, ioport_value mask, ioport_value default_) { PORT_SPECIAL_ONOFF(configurer, mask, default_, INPUT_STRING.INPUT_STRING_Unused); }
        public static void PORT_DIPUNKNOWN_DIPLOC(ioport_configurer configurer, ioport_value mask, ioport_value default_, string diploc) { PORT_SPECIAL_ONOFF_DIPLOC(configurer, mask, default_, INPUT_STRING.INPUT_STRING_Unknown, diploc); }
        //define PORT_DIPUNKNOWN(_mask, _default)             PORT_SPECIAL_ONOFF(_mask, _default, Unknown)
        public static void PORT_SERVICE_DIPLOC(ioport_configurer configurer, ioport_value mask, ioport_value default_, string diploc) { PORT_SPECIAL_ONOFF_DIPLOC(configurer, mask, default_, INPUT_STRING.INPUT_STRING_Service_Mode, diploc); }
        public static void PORT_SERVICE(ioport_configurer configurer, ioport_value mask, ioport_value default_) { PORT_SPECIAL_ONOFF(configurer, mask, default_, INPUT_STRING.INPUT_STRING_Service_Mode); }
        //define PORT_SERVICE_NO_TOGGLE(_mask, _default)             PORT_BIT( _mask, _mask & _default, IPT_SERVICE ) PORT_NAME( DEF_STR( Service_Mode ))
        public static void PORT_VBLANK(ioport_configurer configurer, string screen, screen_device device) { PORT_READ_LINE_DEVICE_MEMBER(configurer, screen, device.vblank); }
        //define PORT_HBLANK(_screen)             PORT_READ_LINE_DEVICE_MEMBER(_screen, screen_device, hblank)

        public static char32_t UCHAR_MAMEKEY(char32_t code) { return UCHAR_MAMEKEY_BEGIN + code; }  //#define UCHAR_MAMEKEY(code) (UCHAR_MAMEKEY_BEGIN + ITEM_ID_##code)

        // macro for a read callback function (PORT_CUSTOM)
        //#define CUSTOM_INPUT_MEMBER(name)   ioport_value name()
        //#define DECLARE_CUSTOM_INPUT_MEMBER(name)   ioport_value name()

        // macro for port write callback functions (PORT_CHANGED)
        //#define INPUT_CHANGED_MEMBER(name)  void name(ioport_field &field, u32 param, ioport_value oldval, ioport_value newval)
        //#define DECLARE_INPUT_CHANGED_MEMBER(name)  void name(ioport_field &field, u32 param, ioport_value oldval, ioport_value newval)

        // macro for port changed callback functions (PORT_CROSSHAIR_MAPPER)
        //#define CROSSHAIR_MAPPER_MEMBER(name)   float name(float linear_value)
        //#define DECLARE_CROSSHAIR_MAPPER_MEMBER(name)   float name(float linear_value)

        // macro for wrapping a default string
        public const string DEFAULT_STR_PREFIX = "**DEFAULT_ENUM_";
        public static string DEF_STR(INPUT_STRING str_num) { return DEFAULT_STR_PREFIX + str_num.ToString(); }  //#define DEF_STR(str_num) ((const char *)INPUT_STRING_##str_num)


        //-------------------------------------------------
        //  compute_scale -- compute an 8.24 scale value
        //  from a numerator and a denominator
        //-------------------------------------------------
        public static Int64 compute_scale(int num, int den)
        {
            return ((Int64)num << 24) / den;
        }

        //-------------------------------------------------
        //  recip_scale -- compute an 8.24 reciprocal of
        //  an 8.24 scale value
        //-------------------------------------------------
        public static Int64 recip_scale(Int64 scale)
        {
            return ((Int64)1 << 48) / scale;
        }

        //-------------------------------------------------
        //  apply_scale -- apply an 8.24 scale value to
        //  a 32-bit value
        //-------------------------------------------------
        public static int apply_scale(int value, Int64 scale)
        {
            return (int)(((Int64)(value) * scale) / (1 << 24));
        }
    }


    // ======================> inp_header
    // header at the front of INP files
    class inp_header : global_object
    {
        // parameters
        public const UInt32 MAJVERSION = 3;
        public const UInt32 MINVERSION = 0;


        const UInt32 OFFS_MAGIC       = 0x00;    // 0x08 bytes
        const UInt32 OFFS_BASETIME    = 0x08;    // 0x08 bytes (little-endian binary integer)
        const UInt32 OFFS_MAJVERSION  = 0x10;    // 0x01 bytes (binary integer)
        const UInt32 OFFS_MINVERSION  = 0x11;    // 0x01 bytes (binary integer)
                                                                    // 0x02 bytes reserved
        const UInt32 OFFS_SYSNAME     = 0x14;    // 0x0c bytes (ASCII)
        const UInt32 OFFS_APPDESC     = 0x20;    // 0x20 bytes (ASCII)
        const UInt32 OFFS_END         = 0x40;

        static MemoryU8 MAGIC = new MemoryU8((int)(OFFS_BASETIME - OFFS_MAGIC), true);  //static u8 const                 MAGIC[OFFS_BASETIME - OFFS_MAGIC];

        MemoryU8 m_data = new MemoryU8((int)OFFS_END, true);  //u8                              m_data[OFFS_END];


        public bool read(emu_file f) { return f.read(new PointerU8(m_data), (UInt32)m_data.Count) == m_data.Count; }
        public bool write(emu_file f) { return f.write(new PointerU8(m_data), (UInt32)m_data.Count) == m_data.Count; }

        public bool check_magic() { return 0 == memcmp(new PointerU8(MAGIC), new PointerU8(m_data, (int)OFFS_MAGIC), OFFS_BASETIME - OFFS_MAGIC); }
        public UInt64 get_basetime()
        {
            return
                    ((UInt64)(m_data[OFFS_BASETIME + 0]) << (0 * 8)) |
                    ((UInt64)(m_data[OFFS_BASETIME + 1]) << (1 * 8)) |
                    ((UInt64)(m_data[OFFS_BASETIME + 2]) << (2 * 8)) |
                    ((UInt64)(m_data[OFFS_BASETIME + 3]) << (3 * 8)) |
                    ((UInt64)(m_data[OFFS_BASETIME + 4]) << (4 * 8)) |
                    ((UInt64)(m_data[OFFS_BASETIME + 5]) << (5 * 8)) |
                    ((UInt64)(m_data[OFFS_BASETIME + 6]) << (6 * 8)) |
                    ((UInt64)(m_data[OFFS_BASETIME + 7]) << (7 * 8));
        }
        public UInt32 get_majversion() { return m_data[OFFS_MAJVERSION]; }
        public UInt32 get_minversion() { return m_data[OFFS_MINVERSION]; }
        public string get_sysname() { return get_string(OFFS_SYSNAME, OFFS_APPDESC); }
        public string get_appdesc() { return get_string(OFFS_APPDESC, OFFS_END); }

        public void set_magic() { memcpy(new PointerU8(m_data, (int)OFFS_MAGIC), new PointerU8(MAGIC), OFFS_BASETIME - OFFS_MAGIC); }  // std::memcpy(m_data + OFFS_MAGIC, MAGIC, OFFS_BASETIME - OFFS_MAGIC); }
        public void set_basetime(UInt64 time)
        {
            m_data[OFFS_BASETIME + 0] = (byte)((time >> (0 * 8)) & 0x00ff);
            m_data[OFFS_BASETIME + 1] = (byte)((time >> (1 * 8)) & 0x00ff);
            m_data[OFFS_BASETIME + 2] = (byte)((time >> (2 * 8)) & 0x00ff);
            m_data[OFFS_BASETIME + 3] = (byte)((time >> (3 * 8)) & 0x00ff);
            m_data[OFFS_BASETIME + 4] = (byte)((time >> (4 * 8)) & 0x00ff);
            m_data[OFFS_BASETIME + 5] = (byte)((time >> (5 * 8)) & 0x00ff);
            m_data[OFFS_BASETIME + 6] = (byte)((time >> (6 * 8)) & 0x00ff);
            m_data[OFFS_BASETIME + 7] = (byte)((time >> (7 * 8)) & 0x00ff);
        }
        public void set_version()
        {
            m_data[OFFS_MAJVERSION] = (byte)MAJVERSION;
            m_data[OFFS_MINVERSION] = (byte)MINVERSION;
        }
        public void set_sysname(string name) { set_string(OFFS_SYSNAME, OFFS_APPDESC, name); }
        public void set_appdesc(string desc) { set_string(OFFS_APPDESC, OFFS_END, desc); }


        //template <std::size_t BEGIN, std::size_t END> void set_string(std::string const &str)
        void set_string(UInt32 BEGIN, UInt32 END, string str)
        {
            UInt32 used = std.min((UInt32)str.size() + 1, END - BEGIN);
            byte[] strBytes = System.Text.Encoding.ASCII.GetBytes(str);
            std.memcpy(new PointerU8(m_data, (int)BEGIN), new PointerU8(new MemoryU8(strBytes)), used);  //std::memcpy(m_data + BEGIN, str.c_str(), used);
            if ((END - BEGIN) > used)
                std.memset(new PointerU8(m_data, (int)BEGIN), (u8)0, (END - BEGIN) - used);  //std::memset(m_data + BEGIN + used, 0, (END - BEGIN) - used);
        }

        //template <std::size_t BEGIN, std::size_t END> std::string get_string() const
        string get_string(UInt32 BEGIN, UInt32 END)
        {
            //char const *const begin = reinterpret_cast<char const *>(m_data + BEGIN);
            //return std::string(begin, std::find(begin, reinterpret_cast<char const *>(m_data + END), '\0'));
            return m_data.ToString((int)BEGIN, m_data.IndexOf((int)BEGIN, (int)END, Convert.ToByte('\0')) - (int)BEGIN);
        }
    }


    // ======================> input_device_default
    // device defined default input settings
    public class input_device_default
    {
        public string tag;            // tag of port to update
        public ioport_value mask;           // mask to apply to the port
        public ioport_value defvalue;       // new default value
    }


    public class size_t_constant_SEQ_TYPE_TOTAL : uint32_constant { public UInt32 value { get { return (UInt32)input_seq_type.SEQ_TYPE_TOTAL; } } }


    // ======================> input_type_entry
    // describes a fundamental input type, including default input sequences
    public class input_type_entry
    {
        // internal state
        ioport_type m_type;             // IPT_* for this entry
        ioport_group m_group;            // which group the port belongs to
        u8 m_player;           // player number (0 is player 1)
        string m_token;            // token used to store settings
        string m_name;             // user-friendly name
        std.array<input_seq, size_t_constant_SEQ_TYPE_TOTAL> m_defseq = new std.array<input_seq, size_t_constant_SEQ_TYPE_TOTAL>(); // default input sequence
        std.array<input_seq, size_t_constant_SEQ_TYPE_TOTAL> m_seq = new std.array<input_seq, size_t_constant_SEQ_TYPE_TOTAL>(); // currently configured sequences


        // construction/destruction
        //-------------------------------------------------
        //  input_type_entry - constructors
        //-------------------------------------------------
        public input_type_entry(ioport_type type, ioport_group group, int player, string token, string name, input_seq standard)
        {
            m_type = type;
            m_group = group;
            m_player = (u8)player;
            m_token = token;
            m_name = name;


            m_seq[(int)input_seq_type.SEQ_TYPE_STANDARD] = standard;
            m_defseq[(int)input_seq_type.SEQ_TYPE_STANDARD] = standard;
        }

        public input_type_entry(ioport_type type, ioport_group group, int player, string token, string name, input_seq standard, input_seq decrement, input_seq increment)
        {
            m_type = type;
            m_group = group;
            m_player = (u8)player;
            m_token = token;
            m_name = name;


            m_seq[(int)input_seq_type.SEQ_TYPE_STANDARD] = standard;
            m_defseq[(int)input_seq_type.SEQ_TYPE_STANDARD] = standard;
            m_seq[(int)input_seq_type.SEQ_TYPE_INCREMENT] = increment;
            m_defseq[(int)input_seq_type.SEQ_TYPE_INCREMENT] = increment;
            m_seq[(int)input_seq_type.SEQ_TYPE_DECREMENT] = decrement;
            m_defseq[(int)input_seq_type.SEQ_TYPE_DECREMENT] = decrement;
        }


        // getters

        public ioport_type type() { return m_type; }
        public ioport_group group() { return m_group; }
        public byte player() { return m_player; }
        //const char *token() const { return m_token; }
        public string name() { return m_name; }
        input_seq defseq(input_seq_type seqtype = input_seq_type.SEQ_TYPE_STANDARD) { return m_defseq[(int)seqtype]; }
        public input_seq seq(input_seq_type seqtype = input_seq_type.SEQ_TYPE_STANDARD) { return m_seq[(int)seqtype]; }


        // setters

        public void restore_default_seq() { m_seq = m_defseq; }

        //void set_seq(input_seq_type seqtype, const input_seq &seq) noexcept { m_seq[seqtype] = seq; }
        //void replace_code(input_code oldcode, input_code newcode) noexcept;
        //void configure_osd(const char *token, const char *name);


        //-------------------------------------------------
        //  restore_default_seq - restores the sequence
        //  from the default
        //-------------------------------------------------
        //void input_type_entry::restore_default_seq() noexcept
        //{
        //    m_seq = m_defseq;
        //}
    }


    // ======================> digital_joystick
    // tracking information about a digital joystick input
    public class digital_joystick : simple_list_item<digital_joystick>
    {
        // directions
        public enum direction_t
        {
            JOYDIR_UP,
            JOYDIR_DOWN,
            JOYDIR_LEFT,
            JOYDIR_RIGHT,
            JOYDIR_COUNT
        }
        //DECLARE_ENUM_INCDEC_OPERATORS(digital_joystick::direction_t)


        // bit constants
        const u8 UP_BIT = 1 << (int)direction_t.JOYDIR_UP;
        const u8 DOWN_BIT = 1 << (int)direction_t.JOYDIR_DOWN;
        const u8 LEFT_BIT = 1 << (int)direction_t.JOYDIR_LEFT;
        const u8 RIGHT_BIT = 1 << (int)direction_t.JOYDIR_RIGHT;


        // internal state
        digital_joystick m_next;                                         // next joystick in the list
        int m_player;                                       // player number represented
        int m_number;                                       // joystick number represented
        std.forward_list<ioport_field> [] m_field = new std.forward_list<ioport_field>[(int)direction_t.JOYDIR_COUNT];  //std::forward_list<std::reference_wrapper<ioport_field> > m_field[JOYDIR_COUNT];  // potential input fields for each direction
        u8 m_current;                                      // current value
        u8 m_current4way;                                  // current 4-way value
        u8 m_previous;                                     // previous value


        // construction/destruction
        //-------------------------------------------------
        //  digital_joystick - constructor
        //-------------------------------------------------
        public digital_joystick(int player, int number)
        {
            m_next = null;
            m_player = player;
            m_number = number;
            m_current = 0;
            m_current4way = 0;
            m_previous = 0;


            for (int i = 0; i < m_field.Length; i++)
                m_field[i] = new std.forward_list<ioport_field>();
        }


        // getters
        public digital_joystick next() { return m_next; }
        public digital_joystick m_next_get() { return m_next; }
        public void m_next_set(digital_joystick value) { m_next = value; }

        public int player() { return m_player; }
        public int number() { return m_number; }
        public u8 current() { return m_current; }
        public u8 current4way() { return m_current4way; }


        // configuration
        //-------------------------------------------------
        //  set_axis - configure a single axis of a
        //  digital joystick
        //-------------------------------------------------
        public direction_t add_axis(ioport_field field)
        {
            direction_t direction = (direction_t)((field.type() - (ioport_type.IPT_DIGITAL_JOYSTICK_FIRST + 1)) % 4);
            m_field[(int)direction].emplace_front(field);
            return direction;
        }


        // updates
        //-------------------------------------------------
        //  frame_update - update the state of digital
        //  joysticks prior to accumulating the results
        //  in a port
        //-------------------------------------------------
        public void frame_update()
        {
            // remember previous state and reset current state
            m_previous = m_current;
            m_current = 0;

            // read all the associated ports
            running_machine machine = null;
            for (direction_t direction = direction_t.JOYDIR_UP; direction < direction_t.JOYDIR_COUNT; direction++)
            {
                foreach (ioport_field i in m_field[(int)direction])  //for (const std::reference_wrapper<ioport_field> &i : m_field[direction])
                {
                    machine = i.machine();
                    if (machine.input().seq_pressed(i.seq(input_seq_type.SEQ_TYPE_STANDARD)))
                        m_current |= (byte)((byte)1 << (byte)direction);
                }
            }

            // lock out opposing directions (left + right or up + down)
            if ((m_current & (UP_BIT | DOWN_BIT)) == (UP_BIT | DOWN_BIT))
                m_current = (byte)(m_current & (~(UP_BIT | DOWN_BIT)));
            if ((m_current & (LEFT_BIT | RIGHT_BIT)) == (LEFT_BIT | RIGHT_BIT))
                m_current = (byte)(m_current & (~(LEFT_BIT | RIGHT_BIT)));

            // only update 4-way case if joystick has moved
            if (m_current != m_previous)
            {
                m_current4way = m_current;

                //
                //  If joystick is pointing at a diagonal, acknowledge that the player moved
                //  the joystick by favoring a direction change.  This minimizes frustration
                //  and maximizes responsiveness.
                //
                //  For example, if you are holding "left" then switch to "up" (where both left
                //  and up are briefly pressed at the same time), we'll transition immediately
                //  to "up."
                //
                //  Zero any switches that didn't change from the previous to current state.
                //
                if ((m_current4way & (UP_BIT | DOWN_BIT)) != 0 &&
                    (m_current4way & (LEFT_BIT | RIGHT_BIT)) != 0)
                {
                    m_current4way = (byte)(m_current4way ^ (m_current4way & m_previous));
                }

                //
                //  If we are still pointing at a diagonal, we are in an indeterminant state.
                //
                //  This could happen if the player moved the joystick from the idle position directly
                //  to a diagonal, or from one diagonal directly to an extreme diagonal.
                //
                //  The chances of this happening with a keyboard are slim, but we still need to
                //  constrain this case. Let's pick the horizontal axis.
                //
                if ((m_current4way & (UP_BIT | DOWN_BIT)) != 0 &&
                    (m_current4way & (LEFT_BIT | RIGHT_BIT)) != 0)
                {
                    m_current4way = (byte)(m_current4way & ~(UP_BIT | DOWN_BIT));
                }
            }
        }
    }
    //DECLARE_ENUM_OPERATORS(digital_joystick::direction_t)


    // ======================> ioport_condition
    // encapsulates a condition on a port field or setting
    public class ioport_condition
    {
        // condition types
        public enum condition_t
        {
            ALWAYS = 0,
            EQUALS,
            NOTEQUALS,
            GREATERTHAN,
            NOTGREATERTHAN,
            LESSTHAN,
            NOTLESSTHAN
        }


        // internal state
        condition_t m_condition;    // condition to use
        string m_tag;          // tag of port whose condition is to be tested
        ioport_port m_port;         // reference to the port to be tested
        ioport_value m_mask;         // mask to apply to the port
        ioport_value m_value;        // value to compare against


        // construction/destruction
        public ioport_condition() { reset(); }
        public ioport_condition(condition_t condition, string tag, ioport_value mask, ioport_value value) { set(condition, tag, mask, value); }


        // getters
        public condition_t condition() { return m_condition; }
        string tag() { return m_tag; }
        //ioport_value mask() const { return m_mask; }
        //ioport_value value() const { return m_value; }


        // operators

        //bool operator==(const ioport_condition &rhs) const { return (m_mask == rhs.m_mask && m_value == rhs.m_value && m_condition == rhs.m_condition && strcmp(m_tag, rhs.m_tag) == 0); }

        //-------------------------------------------------
        //  eval - evaluate condition
        //-------------------------------------------------
        public bool eval()
        {
            // always condition is always true
            if (m_condition == condition_t.ALWAYS)
                return true;

            // otherwise, read the referenced port and switch off the condition type
            ioport_value condvalue = m_port.read();
            switch (m_condition)
            {
                case condition_t.ALWAYS:            return true;
                case condition_t.EQUALS:            return ((condvalue & m_mask) == m_value);
                case condition_t.NOTEQUALS:         return ((condvalue & m_mask) != m_value);
                case condition_t.GREATERTHAN:       return ((condvalue & m_mask) > m_value);
                case condition_t.NOTGREATERTHAN:    return ((condvalue & m_mask) <= m_value);
                case condition_t.LESSTHAN:          return ((condvalue & m_mask) < m_value);
                case condition_t.NOTLESSTHAN:       return ((condvalue & m_mask) >= m_value);
            }

            return true;
        }

        public bool none() { return (m_condition == condition_t.ALWAYS); }


        // configuration

        void reset() { set(condition_t.ALWAYS, null, 0, 0); }

        public void set(condition_t condition, string tag, ioport_value mask, ioport_value value)
        {
            m_condition = condition;
            m_tag = tag;
            m_mask = mask;
            m_value = value;
        }


        //-------------------------------------------------
        //  initialize - create the live state
        //-------------------------------------------------
        public void initialize(device_t device)
        {
            if (m_tag != null)
                m_port = device.ioport(m_tag);
        }
    }


    // ======================> ioport_setting
    // a single setting for a configuration or DIP switch
    public class ioport_setting : simple_list_item<ioport_setting>
    {
        // internal state
        ioport_setting m_next;             // pointer to next setting in sequence
        ioport_field m_field;            // pointer back to the field that owns us
        ioport_value m_value;            // value of the bits in this setting
        string m_name;             // user-friendly name to display
        ioport_condition m_condition = new ioport_condition();        // condition under which this setting is valid


        // construction/destruction
        //-------------------------------------------------
        //  ioport_setting - constructor
        //-------------------------------------------------
        public ioport_setting(ioport_field field, ioport_value _value, string _name)
        {
            m_next = null;
            m_field = field;
            m_value = _value;
            m_name = _name;
        }


        // getters
        public ioport_setting next() { return m_next; }
        public ioport_setting m_next_get() { return m_next; }
        public void m_next_set(ioport_setting value) { m_next = value; }

        //ioport_field &field() const { return m_field; }
        public device_t device() { return m_field.device(); }
        //running_machine &machine() const;
        public ioport_value value() { return m_value; }
        public ioport_condition condition() { return m_condition; }
        //const char *name() const { return m_name; }


        // helpers
        public bool enabled() { return m_condition.eval(); }
    }


    // ======================> ioport_diplocation
    // a mapping from a bit to a physical DIP switch description
    class ioport_diplocation : simple_list_item<ioport_diplocation>
    {
        ioport_diplocation m_next;         // pointer to the next bit
        string m_name;         // name of the physical DIP switch
        u8 m_number;       // physical switch number
        bool m_invert;       // is this an active-high DIP?


        // construction/destruction
        //-------------------------------------------------
        //  ioport_diplocation - constructor
        //-------------------------------------------------
        public ioport_diplocation(string name, u8 swnum, bool invert)
        {
            m_next = null;
            m_name = name;
            m_number = swnum;
            m_invert = invert;
        }


        // getters
        public ioport_diplocation next() { return m_next; }
        public ioport_diplocation m_next_get() { return m_next; }
        public void m_next_set(ioport_diplocation value) { m_next = value; }

        //const char *name() const { return m_name; }
        //UINT8 number() const { return m_number; }
        //bool inverted() const { return m_invert; }
    }


    // ======================> ioport_field
    // a single bitfield within an input port
    public class ioport_field : global_object, simple_list_item<ioport_field>
    {
        //friend class simple_list<ioport_field>;
        //friend class ioport_configurer;
        //friend class dynamic_field;


        // flags for ioport_fields
        const int FIELD_FLAG_OPTIONAL = 0x0001;    // set if this field is not required but recognized by hw
        public const int FIELD_FLAG_COCKTAIL = 0x0002;    // set if this field is relevant only for cocktail cabinets
        public const int FIELD_FLAG_TOGGLE =   0x0004;    // set if this field should behave as a toggle
        const int FIELD_FLAG_ROTATED =  0x0008;    // set if this field represents a rotated control
        public const int ANALOG_FLAG_REVERSE = 0x0010;    // analog only: reverse the sense of the axis
        const int ANALOG_FLAG_RESET =   0x0020;    // analog only: always preload in->default for relative axes, returning only deltas
        const int ANALOG_FLAG_WRAPS =   0x0040;    // analog only: positional count wraps around
        const int ANALOG_FLAG_INVERT =  0x0080;    // analog only: bitwise invert bits


        // internal state
        ioport_field m_next;             // pointer to next field in sequence
        ioport_port m_port;             // reference to the port that owns us
        ioport_field_live m_live;         // live state of field (NULL if not live)
        int m_modcount;         // modification count
        simple_list<ioport_setting> m_settinglist = new simple_list<ioport_setting>();      // list of input_setting_configs
        simple_list<ioport_diplocation> m_diploclist = new simple_list<ioport_diplocation>();   // list of locations for various bits

        // generally-applicable data
        ioport_value m_mask;             // mask of bits belonging to the field
        ioport_value m_defvalue;         // default value of these bits
        ioport_condition m_condition = new ioport_condition();        // condition under which this field is relevant
        ioport_type m_type;             // IPT_* type for this port
        u8 m_player;           // player number (0-based)
        public u32 m_flags;            // combination of FIELD_FLAG_* and ANALOG_FLAG_* above
        public u8 m_impulse;          // number of frames before reverting to defvalue
        public string m_name;             // user-friendly name to display
        public input_seq [] m_seq = new input_seq[(int)input_seq_type.SEQ_TYPE_TOTAL];// sequences of all types
        public ioport_field_read_delegate m_read;             // read callback routine
        public ioport_field_write_delegate m_write;            // write callback routine
        public u32 m_write_param;  // parameter for write callback routine

        // data relevant to digital control types
        bool m_digital_value;    // externally set value

        // data relevant to analog control types
        public ioport_value m_min;              // minimum value for absolute axes
        public ioport_value m_max;              // maximum value for absolute axes
        public s32 m_sensitivity;      // sensitivity (100=normal)
        public s32 m_delta;            // delta to apply each frame a digital inc/dec key is pressed
        public s32 m_centerdelta;      // delta to apply each frame no digital inputs are pressed
        crosshair_axis_t m_crosshair_axis;   // crosshair axis
        float m_crosshair_scale;  // crosshair scale
        float m_crosshair_offset; // crosshair offset
        float m_crosshair_altaxis;// crosshair alternate axis value
        ioport_field_crossmap_delegate m_crosshair_mapper; // crosshair mapping function
        public u16 m_full_turn_count;  // number of optical counts for 1 full turn of the original control
        ioport_value [] m_remap_table;  //const ioport_value *        m_remap_table;      // pointer to an array that remaps the port value

        // data relevant to other specific types
        public u8 m_way;              // digital joystick 2/4/8-way descriptions
        char32_t [,] m_chars = new char32_t[1 << (int)(ioport_global.UCHAR_SHIFT_END - ioport_global.UCHAR_SHIFT_BEGIN + 1), 2];         // unicode key data


        // construction/destruction
        //-------------------------------------------------
        //  ioport_field - constructor
        //-------------------------------------------------
        public ioport_field(ioport_port port, ioport_type type, ioport_value defvalue, ioport_value maskbits, string name = null)
        {
            m_next = null;
            m_port = port;
            m_modcount = port.modcount();
            m_mask = maskbits;
            m_defvalue = defvalue & maskbits;
            m_type = type;
            m_player = 0;
            m_flags = 0;
            m_impulse = 0;
            m_name = name;
            m_read = null;
            m_write = null;
            m_write_param = 0;
            m_digital_value = false;
            m_min = 0;
            m_max = maskbits;
            m_sensitivity = 0;
            m_delta = 0;
            m_centerdelta = 0;
            m_crosshair_axis = crosshair_axis_t.CROSSHAIR_AXIS_NONE;
            m_crosshair_scale = 1.0f;
            m_crosshair_offset = 0;
            m_crosshair_altaxis = 0;
            m_crosshair_mapper = null;
            m_full_turn_count = 0;
            m_remap_table = null;
            m_way = 0;


            // reset sequences and chars
            for (input_seq_type seqtype = input_seq_type.SEQ_TYPE_STANDARD; seqtype < input_seq_type.SEQ_TYPE_TOTAL; seqtype++)
            {
                m_seq[(int)seqtype] = new input_seq();
                m_seq[(int)seqtype].set_default();
            }

            //for (int i = 0; i < ARRAY_LENGTH(m_chars); i++)
            //    std::fill(std::begin(m_chars[i]), std::end(m_chars[i]), char32_t(0));
            for (int i = 0; i < m_chars.GetLength(0); i++)
                for (int j = 0; j < m_chars.GetLength(1); j++)
                    m_chars[i, j] = 0;

            // for DIP switches and configs, look for a default value from the owner
            if (type == ioport_type.IPT_DIPSWITCH || type == ioport_type.IPT_CONFIG)
            {
                input_device_default [] def = device().input_ports_defaults();
                if (def != null)
                {
                    string fulltag = port.tag();
                    for (int i = 0; !string.IsNullOrEmpty(def[i].tag); i++)  //for ( ; !string.IsNullOrEmpty(def.tag); def++)
                    {
                        if ((device().subtag(def[i].tag)) == fulltag && def[i].mask == m_mask)
                            m_defvalue = def[i].defvalue & m_mask;
                    }
                }

                m_flags |= FIELD_FLAG_TOGGLE;
            }
        }


        // getters
        public ioport_field next() { return m_next; }
        public ioport_field m_next_get() { return m_next; }
        public void m_next_set(ioport_field value) { m_next = value; }

        public ioport_port port() { return m_port; }
        public device_t device() { return m_port.device(); }
        public ioport_manager manager() { return m_port.manager(); }
        public running_machine machine() { return m_port.machine(); }
        public int modcount() { return m_modcount; }
        public simple_list<ioport_setting> settings() { return m_settinglist; }
        simple_list<ioport_diplocation> diplocations() { return m_diploclist; }

        public ioport_value mask() { return m_mask; }
        public ioport_value defvalue() { return m_defvalue; }
        public ioport_condition condition() { return m_condition; }
        public ioport_type type() { return m_type; }
        public u8 player() { return m_player; }
        public bool digital_value() { return m_digital_value; }
        public void set_value(ioport_value value)
        {
            if (is_analog())
                live().analog.set_value((s32)value);
            else
                m_digital_value = value != 0;
        }

        bool optional() { return ((m_flags & FIELD_FLAG_OPTIONAL) != 0); }
        //bool cocktail() const { return ((m_flags & FIELD_FLAG_COCKTAIL) != 0); }
        public bool toggle() { return ((m_flags & FIELD_FLAG_TOGGLE) != 0); }
        public bool rotated() { return (m_flags & FIELD_FLAG_ROTATED) != 0; }
        public bool analog_reverse() { return (m_flags & ANALOG_FLAG_REVERSE) != 0; }
        public bool analog_reset() { return (m_flags & ANALOG_FLAG_RESET) != 0; }
        public bool analog_wraps() { return (m_flags & ANALOG_FLAG_WRAPS) != 0; }
        public bool analog_invert() { return (m_flags & ANALOG_FLAG_INVERT) != 0; }


        //u8 impulse() const noexcept { return m_impulse; }


        //-------------------------------------------------
        //  name - return the field name for a given input
        //  field
        //-------------------------------------------------
        public string name()
        {
            // if we have a non-default name, use that
            if (m_live != null && !string.IsNullOrEmpty(m_live.name))
                return m_live.name;

            if (m_name != null)
                return m_name;

            // otherwise, return the name associated with the type
            return manager().type_name(m_type, m_player);
        }

        public string specific_name() { return m_name; }

        //-------------------------------------------------
        //  seq - return the live input sequence for the
        //  given input field
        //-------------------------------------------------
        public input_seq seq(input_seq_type seqtype = input_seq_type.SEQ_TYPE_STANDARD)
        {
            // if no live state, return default
            if (m_live == null)
                return defseq(seqtype);

            // if the sequence is the special default code, return the expanded default value
            if (m_live.seq[(int)seqtype].is_default())
                return manager().type_seq(m_type, m_player, seqtype);

            // otherwise, return the sequence as-is
            return m_live.seq[(int)seqtype];
        }

        //-------------------------------------------------
        //  defseq - return the default input sequence for
        //  the given input field
        //-------------------------------------------------
        input_seq defseq(input_seq_type seqtype = input_seq_type.SEQ_TYPE_STANDARD)
        {
            // if the sequence is the special default code, return the expanded default value
            if (m_seq[(int)seqtype].is_default())
                return manager().type_seq(m_type, m_player, seqtype);

            // otherwise, return the sequence as-is
            return m_seq[(int)seqtype];
        }

        public input_seq defseq_unresolved(input_seq_type seqtype = input_seq_type.SEQ_TYPE_STANDARD) { return m_seq[(int)seqtype]; }
        public void set_defseq(input_seq newseq) { set_defseq(input_seq_type.SEQ_TYPE_STANDARD, newseq); }

        //-------------------------------------------------
        //  set_defseq - dynamically alter the default
        //  input sequence for the given input field
        //-------------------------------------------------
        void set_defseq(input_seq_type seqtype, input_seq newseq)
        {
            bool was_changed = seq(seqtype) != defseq(seqtype);

            // set the new sequence
            m_seq[(int)seqtype] = newseq;

            // also update live state unless previously customized
            if (m_live != null && !was_changed)
                m_live.seq[(int)seqtype] = newseq;
        }

        public bool has_dynamic_read() { return m_read != null; }
        public bool has_dynamic_write() { return m_write != null; }

        public ioport_value minval() { return m_min; }
        public ioport_value maxval() { return m_max; }
        public s32 sensitivity() { return m_sensitivity; }
        public s32 delta() { return m_delta; }
        public s32 centerdelta() { return m_centerdelta; }
        public crosshair_axis_t crosshair_axis() { return m_crosshair_axis; }
        //float crosshair_scale() const noexcept { return m_crosshair_scale; }
        //float crosshair_offset() const noexcept { return m_crosshair_offset; }
        //float crosshair_altaxis() const noexcept { return m_crosshair_altaxis; }
        //u16 full_turn_count() const noexcept { return m_full_turn_count; }
        public ioport_value [] remap_table() { return m_remap_table; }


        u8 way() { return m_way; }


        //-------------------------------------------------
        //  keyboard_codes - accesses a particular keyboard
        //  code list
        //-------------------------------------------------
        public std.vector<char32_t> keyboard_codes(int which)
        {
            if (which >= m_chars.GetLength(0))  // ARRAY_LENGTH(m_chars))
                throw new emu_fatalerror("Tried to access keyboard_code with out-of-range index {0}\n", which);

            std.vector<char32_t> result = new std.vector<char32_t>();
            for (int i = 0; i < m_chars.GetLength(which) && m_chars[which, i] != 0; i++)  //ARRAY_LENGTH(m_chars[which]) && m_chars[which][i] != 0; i++)
                result.push_back(m_chars[which, i]);

            return result;
        }


        //-------------------------------------------------
        //  key_name - returns the name of a specific key
        //-------------------------------------------------
        public string key_name(int which)
        {
            std.vector<char32_t> codes = keyboard_codes(which);
            char32_t ch = codes.empty() ? 0 : codes[0];

            // attempt to get the string from the character info table
            //switch (ch)
            {
                if (ch == 8) return "Backspace";
                else if (ch == 9) return "Tab";
                else if (ch == 12) return "Clear";
                else if (ch == 13) return "Enter";
                else if (ch == 27) return "Esc";
                else if (ch == 32) return "Space";
                else if (ch == ioport_global.UCHAR_SHIFT_1) return "Shift";
                else if (ch == ioport_global.UCHAR_SHIFT_2) return "Ctrl";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_ESC)) return "Esc";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_INSERT)) return "Insert";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_DEL)) return "Delete";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_HOME)) return "Home";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_END)) return "End";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_PGUP)) return "Page Up";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_PGDN)) return "Page Down";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_LEFT)) return "Cursor Left";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_RIGHT)) return "Cursor Right";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_UP)) return "Cursor Up";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_DOWN)) return "Cursor Down";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_SLASH_PAD)) return "Keypad /";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_ASTERISK)) return "Keypad *";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_MINUS_PAD)) return "Keypad -";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_PLUS_PAD)) return "Keypad +";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_DEL_PAD)) return "Keypad .";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_ENTER_PAD)) return "Keypad Enter";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_BS_PAD)) return "Keypad Backspace";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_TAB_PAD)) return "Keypad Tab";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_00_PAD)) return "Keypad 00";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_000_PAD)) return "Keypad 000";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_PRTSCR)) return "Print Screen";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_PAUSE)) return "Pause";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_LSHIFT)) return "Left Shift";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_RSHIFT)) return "Right Shift";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_LCONTROL)) return "Left Ctrl";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_RCONTROL)) return "Right Ctrl";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_LALT)) return "Left Alt";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_RALT)) return "Right Alt";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_SCRLOCK)) return "Scroll Lock";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_NUMLOCK)) return "Num Lock";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_CAPSLOCK)) return "Caps Lock";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_LWIN)) return "Left Win";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_RWIN)) return "Right Win";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_MENU)) return "Menu";
                else if (ch == ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_CANCEL)) return "Break";
            }

            // handle function keys
            if (ch >= ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_F1) && ch <= ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_F20))
                return string.Format("F{0}", ch - ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_F1) + 1);

            // handle 0-9 on numeric keypad
            if (ch >= ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_0_PAD) && ch <= ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_9_PAD))
                return string.Format("Keypad {0}", ch - ioport_global.UCHAR_MAMEKEY((UInt32)input_item_id.ITEM_ID_0_PAD));

            // if that doesn't work, convert to UTF-8
            if (ch > 0x7F || unicode_global.isprint(ch))
                return unicode_global.utf8_from_uchar(ch);

            // otherwise, opt for question marks
            return "???";
        }


        public ioport_field_live live() { /*assert(m_live != NULL);*/ return m_live; }


        // setters
        //void set_crosshair_scale(float scale) { m_crosshair_scale = scale; }
        //void set_crosshair_offset(float offset) { m_crosshair_offset = offset; }
        public void set_player(u8 player) { m_player = player; }


        // derived getters

        //-------------------------------------------------
        //  type_class - return the type class for this
        //  field
        //-------------------------------------------------
        public ioport_type_class type_class()
        {
            // inputs associated with specific players
            ioport_group group = manager().type_group(m_type, m_player);
            if (group >= ioport_group.IPG_PLAYER1 && group <= ioport_group.IPG_PLAYER10)
                return ioport_type_class.INPUT_CLASS_CONTROLLER;

            // keys (names derived from character codes)
            if (m_type == ioport_type.IPT_KEYPAD || m_type == ioport_type.IPT_KEYBOARD)
                return ioport_type_class.INPUT_CLASS_KEYBOARD;

            // configuration settings (specific names required)
            if (m_type == ioport_type.IPT_CONFIG)
                return ioport_type_class.INPUT_CLASS_CONFIG;

            // DIP switches (specific names required)
            if (m_type == ioport_type.IPT_DIPSWITCH)
                return ioport_type_class.INPUT_CLASS_DIPSWITCH;

            // miscellaneous non-player inputs (named and user-mappable)
            if (group == ioport_group.IPG_OTHER || (group == ioport_group.IPG_INVALID && !string.IsNullOrEmpty(m_name)))
                return ioport_type_class.INPUT_CLASS_MISC;

            // internal inputs (these may be anonymous)
            return ioport_type_class.INPUT_CLASS_INTERNAL;
        }


        public bool is_analog() { return m_type > ioport_type.IPT_ANALOG_FIRST && m_type < ioport_type.IPT_ANALOG_LAST; }
        public bool is_digital_joystick() { return m_type > ioport_type.IPT_DIGITAL_JOYSTICK_FIRST && m_type < ioport_type.IPT_DIGITAL_JOYSTICK_LAST; }


        // additional operations
        public bool enabled() { return m_condition.eval(); }
        //const char *setting_name() const;
        //bool has_previous_setting() const;
        //void select_previous_setting();
        //bool has_next_setting() const;

        //-------------------------------------------------
        //  select_next_setting - select the next item for
        //  a DIP switch or configuration field
        //-------------------------------------------------
        void select_next_setting()
        {
            // only makes sense if we have settings
            assert(!m_settinglist.empty());

            // scan the list of settings looking for a match on the current value
            ioport_setting nextsetting = null;
            ioport_setting setting;
            for (setting = m_settinglist.first(); setting != null; setting = setting.next())
            {
                if (setting.enabled())
                {
                    if (setting.value() == m_live.value)
                        break;
                }
            }

            // if we found one, scan forward for the next valid one
            if (setting != null)
            {
                for (nextsetting = setting.next(); nextsetting != null; nextsetting = nextsetting.next())
                {
                    if (nextsetting.enabled())
                        break;
                }
            }

            // if we hit the end, search from the beginning
            if (nextsetting == null)
            {
                for (nextsetting = m_settinglist.first(); nextsetting != null; nextsetting = nextsetting.next())
                {
                    if (nextsetting.enabled())
                        break;
                }
            }

            // update the value to the previous one
            if (nextsetting != null)
                m_live.value = nextsetting.value();
        }


        //-------------------------------------------------
        //  crosshair_read - compute the crosshair
        //  position
        //-------------------------------------------------
        float crosshair_read()
        {
            float value = m_live.analog.crosshair_read();

            // apply the scale and offset
            if (m_crosshair_scale < 0)
                value = -(1.0f - value) * m_crosshair_scale;
            else
                value *= m_crosshair_scale;

            value += m_crosshair_offset;

            // apply custom mapping if necessary
            if (m_crosshair_mapper != null)
                value = m_crosshair_mapper(value);

            return value;
        }


        //-------------------------------------------------
        //  init_live_state - create live state structures
        //-------------------------------------------------
        public void init_live_state(analog_field analog)
        {
            //throw new emu_unimplemented();
#if false
            // resolve callbacks
            m_read.resolve();
            m_write.resolve();
            m_crosshair_mapper.resolve();
#endif

            // allocate live state
            m_live = new ioport_field_live(this, analog); // .reset(global_alloc(ioport_field_live(*this, analog)));

            m_condition.initialize(device());

            foreach (ioport_setting setting in m_settinglist)
                setting.condition().initialize(setting.device());
        }

        //-------------------------------------------------
        //  frame_update_digital - get the state of a
        //  digital field
        //-------------------------------------------------
        public void frame_update(ref ioport_value result)
        {
            // skip if not enabled
            if (!enabled())
                return;

            // handle analog inputs first
            if (m_live.analog != null)
            {
                m_live.analog.frame_update(machine());
                return;
            }

            // if UI is active, ignore digital inputs
            if (machine().ui().is_menu_active())
                return;

            // if user input is locked out here, bail
            if (m_live.lockout)
            {
                // use just the digital value
                if (m_digital_value)
                    result |= m_mask;
                return;
            }

            // if the state changed, look for switch down/switch up
            bool curstate = m_digital_value || machine().input().seq_pressed(seq());
            bool changed = false;
            if (curstate != m_live.last)
            {
                m_live.last = curstate;
                changed = true;
            }

            // coin impulse option
            int effective_impulse = m_impulse;
            int impulse_option_val = machine().options().coin_impulse();
            if (impulse_option_val != 0)
            {
                if (impulse_option_val < 0)
                    effective_impulse = 0;
                else if ((m_type >= ioport_type.IPT_COIN1 && m_type <= ioport_type.IPT_COIN12) || m_impulse != 0)
                    effective_impulse = impulse_option_val;
            }

            // if this is a switch-down event, handle impulse and toggle
            if (changed && curstate)
            {
                // impulse controls: reset the impulse counter
                if (effective_impulse != 0 && m_live.impulse == 0)
                    m_live.impulse = (byte)effective_impulse;

                // toggle controls: flip the toggle state or advance to the next setting
                if (m_live.toggle)
                {
                    if (m_settinglist.count() == 0)
                        m_live.value ^= m_mask;
                    else
                        select_next_setting();
                }
            }

            // update the current state with the impulse state
            if (effective_impulse != 0)
            {
                curstate = (m_live.impulse != 0);
                if (curstate)
                    m_live.impulse--;
            }

            // for toggle switches, the current value is folded into the port's default value
            // so we always return FALSE here
            if (m_live.toggle)
                curstate = false;

            // additional logic to restrict digital joysticks
            if (curstate && !m_digital_value && m_live.joystick != null && m_way != 16 && !machine().options().joystick_contradictory())
            {
                byte mask = (m_way == 4) ? m_live.joystick.current4way() : m_live.joystick.current();
                if ((mask & (1 << (int)m_live.joydir)) == 0)
                    curstate = false;
            }

            // skip locked-out coin inputs
            if (curstate && m_type >= ioport_type.IPT_COIN1 && m_type <= ioport_type.IPT_COIN12 && machine().bookkeeping().coin_lockout_get_state(m_type - ioport_type.IPT_COIN1) != 0)
            {
                bool verbose = machine().options().verbose();
#if MAME_DEBUG
                verbose = true;
#endif
                if (machine().options().coin_lockout())
                {
                    if (verbose)
                        machine().ui().popup_time(3, string.Format("Coinlock disabled {0}.", name()));

                    curstate = false;
                }
                else
                {
                    if (verbose)
                        machine().ui().popup_time(3, string.Format("Coinlock disabled, but broken through {0}.", name()));
                }
            }

            // if we're active, set the appropriate bits in the digital state
            if (curstate)
                result |= m_mask;
        }

        public void reduce_mask(ioport_value bits_to_remove) { m_mask &= ~bits_to_remove; }

#if false
        // user-controllable settings for a field
        struct user_settings
        {
            ioport_value    value = 0;              // for DIP switches
            input_seq       seq[SEQ_TYPE_TOTAL];    // sequences of all types
            s32             sensitivity = 0;        // for analog controls
            s32             delta = 0;              // for analog controls
            s32             centerdelta = 0;        // for analog controls
            bool            reverse = false;        // for analog controls
            bool            toggle = false;         // for non-analog controls
        };
#endif

        //void get_user_settings(user_settings &settings);
        //void set_user_settings(const user_settings &settings);


        //-------------------------------------------------
        //  expand_diplocation - expand a string-based
        //  DIP location into a linked list of
        //  descriptions
        //-------------------------------------------------
        public void expand_diplocation(string location, ref string errorbuf)
        {
            // if nothing present, bail
            if (string.IsNullOrEmpty(location))
                return;

            m_diploclist.reset();

            // parse the string
            string name = ""; // Don't move this variable inside the loop, lastname's lifetime depends on it being outside
            //const char *lastname = NULL;
            string lastname = "";
            //const char *curentry = location;
            int curentryIdx = 0;
            int entries = 0;
            //while (*curentry != 0)
            while (curentryIdx < location.Length)
            {
                // find the end of this entry
                //const char *comma = strchr(curentry, ',');
                int commaIdx = location.Substring(curentryIdx).IndexOf(',');
                if (commaIdx == -1)
                    commaIdx = curentryIdx + location.Substring(curentryIdx).Length;
                else
                    commaIdx += curentryIdx;

                // extract it to tempbuf
                string tempstr;
                //tempstr.cpy(curentry, comma - curentry);
                tempstr = location.Substring(curentryIdx, commaIdx - curentryIdx);

                // first extract the switch name if present
                //const char *number = tempstr;
                int numberIdx = 0;
                //const char *colon = strchr(tempstr, ':');
                int colonIdx = tempstr.IndexOf(':');

                // allocate and copy the name if it is present
                if (colonIdx != -1)
                {
                    //lastname = name.cpy(number, colon - number);
                    name = tempstr.Substring(numberIdx, colonIdx - numberIdx);
                    lastname = name;
                    numberIdx = colonIdx + 1;
                }
                // otherwise, just copy the last name
                else
                {
                    if (string.IsNullOrEmpty(lastname))
                    {
                        errorbuf += string.Format("Switch location '{0}' missing switch name!\n", location);
                        lastname = "UNK";
                    }
                    name = lastname;
                }

                // if the number is preceded by a '!' it's active high
                bool invert = false;
                if (tempstr[numberIdx] == '!')
                {
                    invert = true;
                    numberIdx++;
                }

                // now scan the switch number
                int swnum = -1;
                //if (sscanf(number, "%d", &swnum) != 1)
                if (!int.TryParse(tempstr.Substring(numberIdx).Split()[0], out swnum))
                    errorbuf += string.Format("Switch location '{0}' has invalid format!\n", location);

                // allocate a new entry
                m_diploclist.append(new ioport_diplocation(name, (byte)swnum, invert));
                entries++;

                // advance to the next item
                curentryIdx = commaIdx;
                if (curentryIdx < location.Length)
                    curentryIdx++;
            }

            // then verify the number of bits in the mask matches
            ioport_value temp;
            int bits;
            for (bits = 0, temp = m_mask; temp != 0 && bits < 32; bits++)
                temp &= temp - 1;
            if (bits != entries)
                errorbuf += string.Format("Switch location '{0}' does not describe enough bits for mask {1}\n", location, m_mask);
        }
    }


    // ======================> ioport_field_live
    // internal live state of an input field
    public class ioport_field_live
    {
        // public state
        public analog_field analog;             // pointer to live analog data if this is an analog field
        public digital_joystick joystick;           // pointer to digital joystick information
        public input_seq [] seq;  // currently configured input sequences
        public ioport_value value;              // current value of this port
        public u8 impulse;            // counter for impulse controls
        public bool last;               // were we pressed last time?
        public bool toggle;             // current toggle setting
        public digital_joystick.direction_t joydir;       // digital joystick direction index
        public bool lockout;            // user lockout
        public string name;               // overridden name


        // construction/destruction
        //-------------------------------------------------
        //  ioport_field_live - constructor
        //-------------------------------------------------
        public ioport_field_live(ioport_field field, analog_field _analog)
        {
            analog = _analog;
            joystick = null;
            value = field.defvalue();
            impulse = 0;
            last = false;
            toggle = field.toggle();
            joydir = digital_joystick.direction_t.JOYDIR_COUNT;
            lockout = false;


            // fill in the basic values
            seq = new input_seq[(int)input_seq_type.SEQ_TYPE_TOTAL];
            for (input_seq_type seqtype = input_seq_type.SEQ_TYPE_STANDARD; seqtype < input_seq_type.SEQ_TYPE_TOTAL; seqtype++)
                seq[(int)seqtype] = field.defseq_unresolved(seqtype);

            // if this is a digital joystick field, make a note of it
            if (field.is_digital_joystick())
            {
                joystick = field.manager().digjoystick(field.player(), (field.type() - (ioport_type.IPT_DIGITAL_JOYSTICK_FIRST + 1)) / 4);
                joydir = joystick.add_axis(field);
            }

            name = "";

            // Name keyboard key names
            if (field.type_class() == ioport_type_class.INPUT_CLASS_KEYBOARD && field.specific_name() == null)
            {
                // loop through each character on the field
                for (int which = 0; which < (1 << (int)(ioport_global.UCHAR_SHIFT_END - ioport_global.UCHAR_SHIFT_BEGIN + 1)); which++)
                {
                    std.vector<char32_t> codes = field.keyboard_codes(which);
                    if (codes.empty())
                        break;

                    name += util_.string_format("{0}{1} ", std.max(ioport_global.SPACE_COUNT - 1, 0), field.key_name(which));
                }

                // trim extra spaces
                name = name.Trim();  //name = strtrimspace(name);

                // special case
                if (name.empty())
                    name = "Unnamed Key";
            }
        }
    }


    // ======================> ioport_list
    // class that holds a list of I/O ports
    public class ioport_list : std.map<string, ioport_port>
    {
        public ioport_list() { }


        //-------------------------------------------------
        //  append - append the given device's input ports
        //  to the current list
        //-------------------------------------------------
        public void append(device_t device, out string errorbuf)
        {
            errorbuf = "";

            // no constructor, no list
            ioport_constructor constructor = device.input_ports();
            if (constructor == null)
                return;

            // reset error buffer
            errorbuf = "";

            // detokenize into the list
            constructor(device, this, ref errorbuf);

            // collapse fields and sort the list
            foreach (var port in this)
                port.Value.collapse_fields(ref errorbuf);
        }
    }


    // ======================> ioport_port
    // a single input port configuration
    public class ioport_port : simple_list_item<ioport_port>
    {
        // internal state
        ioport_port m_next;         // pointer to next port
        device_t m_device;       // associated device
        simple_list<ioport_field> m_fieldlist = new simple_list<ioport_field>();    // list of ioport_fields
        string m_tag;          // copy of this port's tag
        int m_modcount;     // modification count
        ioport_value m_active;       // mask of active bits in the port
        ioport_port_live m_live;      // live state of port (NULL if not live)


        // construction/destruction
        //-------------------------------------------------
        //  ioport_port - constructor
        //-------------------------------------------------
        public ioport_port(device_t owner, string tag)
        {
            m_next = null;
            m_device = owner;
            m_tag = tag;
            m_modcount = 0;
            m_active = 0;
        }


        // getters
        public ioport_port next() { return m_next; }
        public ioport_port m_next_get() { return m_next; }
        public void m_next_set(ioport_port value) { m_next = value; }

        //-------------------------------------------------
        //  manager - return a reference to the
        //  ioport_manager on the running machine
        //-------------------------------------------------
        public ioport_manager manager() { return machine().ioport(); }

        public device_t device() { return m_device; }

        //-------------------------------------------------
        //  machine - return a reference to the running
        //  machine
        //-------------------------------------------------
        public running_machine machine() { return m_device.machine(); }

        public simple_list<ioport_field> fields() { return m_fieldlist; }
        public string tag() { return m_tag; }
        public int modcount() { return m_modcount; }
        public ioport_value active() { return m_active; }
        public ioport_port_live live() { /*assert(m_live != NULL);*/ return m_live; }


        // setters
        public void modcount_set(int value) { m_modcount = value; }
        public void active_set(ioport_value value) { m_active = value; }


        // read/write to the port

        //-------------------------------------------------
        //  read - return the value of an I/O port
        //-------------------------------------------------
        public ioport_value read()
        {
            if (!manager().safe_to_read())
                throw new emu_fatalerror("Input ports cannot be read at init time!");

            // start with the digital state
            ioport_value result = m_live.digital;

            // insert dynamic read values
            foreach (dynamic_field dynfield in m_live.readlist)
                dynfield.read(ref result);

            // apply active high/low state to digital and dynamic read inputs
            result ^= m_live.defvalue;

            // insert analog portions
            foreach (analog_field analog in m_live.analoglist)
                analog.read(ref result);

            return result;
        }


        //-------------------------------------------------
        //  write - write a value to a port
        //-------------------------------------------------
        public void write(ioport_value data, ioport_value mem_mask = ioport_value.MaxValue)// ~0)
        {
            // call device line write handlers
            emumem_global.COMBINE_DATA(ref m_live.outputvalue, data, mem_mask);
            foreach (dynamic_field dynfield in m_live.writelist)
            {
                if (dynfield.field().type() == ioport_type.IPT_OUTPUT)
                    dynfield.write(m_live.outputvalue ^ dynfield.field().defvalue());
            }
        }


        // other operations

        //-------------------------------------------------
        //  field - return a pointer to the first field
        //  that intersects the given mask
        //-------------------------------------------------
        public ioport_field field(ioport_value mask)
        {
            // if we got the port, look for the field
            foreach (ioport_field field in fields())
                if ((field.mask() & mask) != 0 && field.enabled())
                    return field;

            return null;
        }

        //-------------------------------------------------
        //  collapse_fields - remove any fields that are
        //  wholly overlapped by other fields
        //-------------------------------------------------
        public void collapse_fields(ref string errorbuf)
        {
            ioport_value maskbits = 0;
            int lastmodcount = -1;

            // remove the whole list and start from scratch
            ioport_field field = m_fieldlist.detach_all();
            while (field != null)
            {
                // if this modcount doesn't match, reset
                if (field.modcount() != lastmodcount)
                {
                    lastmodcount = field.modcount();
                    maskbits = 0;
                }

                // reinsert this field
                ioport_field current = field;
                field = field.next();
                insert_field(current, maskbits, ref errorbuf);
            }
        }

        //-------------------------------------------------
        //  frame_update - once/frame update
        //-------------------------------------------------
        public void frame_update()
        {
            // start with 0 values for the digital bits
            m_live.digital = 0;

            // now loop back and modify based on the inputs
            foreach (ioport_field field in fields())
                field.frame_update(ref m_live.digital);
        }

        //-------------------------------------------------
        //  init_live_state - create the live state
        //-------------------------------------------------
        public void init_live_state() { m_live = new ioport_port_live(this); }


        //-------------------------------------------------
        //  update_defvalue - force an update to the input
        //  port values based on current conditions
        //-------------------------------------------------
        public void update_defvalue(bool flush_defaults)
        {
            // only clear on the first pass
            if (flush_defaults)
                m_live.defvalue = 0;

            // recompute the default value for the entire port
            foreach (ioport_field field in m_fieldlist)
            {
                if (field.enabled())
                    m_live.defvalue = (m_live.defvalue & ~field.mask()) | (field.live().value & field.mask());
            }
        }


        //-------------------------------------------------
        //  insert_field - insert a new field, checking
        //  for errors
        //-------------------------------------------------
        void insert_field(ioport_field newfield, ioport_value disallowedbits, ref string errorbuf)
        {
            // verify against the disallowed bits, but only if we are condition-free
            if (newfield.condition().none())
            {
                if ((newfield.mask() & disallowedbits) != 0)
                    errorbuf += string.Format("INPUT_TOKEN_FIELD specifies duplicate port bits (port={0} mask={1})\n", tag(), newfield.mask());
                disallowedbits |= newfield.mask();
            }

            // first modify/nuke any entries that intersect our maskbits
            ioport_field nextfield;
            for (ioport_field field = m_fieldlist.first(); field != null; field = nextfield)
            {
                nextfield = field.next();
                if ((field.mask() & newfield.mask()) != 0 &&
                    (newfield.condition().none() || field.condition().none() || field.condition() == newfield.condition()))
                {
                    // reduce the mask of the field we found
                    field.reduce_mask(newfield.mask());

                    // if the new entry fully overrides the previous one, we nuke
                    if (ioport_global.INPUT_PORT_OVERRIDE_FULLY_NUKES_PREVIOUS || field.mask() == 0)
                        m_fieldlist.remove(field);
                }
            }

            // make a mask of just the low bit
            ioport_value lowbit = (newfield.mask() ^ (newfield.mask() - 1)) & newfield.mask();

            {
                // scan forward to find where to insert ourselves
                ioport_field field;
                for (field = m_fieldlist.first(); field != null; field = field.next())
                {
                    if (field.mask() > lowbit)
                        break;
                }

                // insert it into the list
                m_fieldlist.insert_before(newfield, field);
            }
        }
    }


    // ======================> analog_field
    // live analog field information
    public class analog_field : global_object, simple_list_item<analog_field>
    {
        // internal state
        analog_field m_next;                 // link to the next analog state for this port
        ioport_field m_field;                // pointer to the input field referenced

        // adjusted values (right-justified and tweaked)
        u8 m_shift;                // shift to align final value in the port
        s32 m_adjdefvalue;          // adjusted default value from the config
        s32 m_adjmin;               // adjusted minimum value from the config
        s32 m_adjmax;               // adjusted maximum value from the config

        // live values of configurable parameters
        s32 m_sensitivity;          // current live sensitivity (100=normal)
        bool m_reverse;              // current live reverse flag
        s32 m_delta;                // current live delta to apply each frame a digital inc/dec key is pressed
        s32 m_centerdelta;          // current live delta to apply each frame no digital inputs are pressed

        // live analog value tracking
        s32 m_accum;                // accumulated value (including relative adjustments)
        s32 m_previous;             // previous adjusted value
        s32 m_previousanalog;       // previous analog value
        s32 m_prog_analog_value;    // programmatically set analog value

        // parameters for modifying live values
        s32 m_minimum;              // minimum adjusted value
        s32 m_maximum;              // maximum adjusted value
        s32 m_center;               // center adjusted value for autocentering
        s32 m_reverse_val;          // value where we subtract from to reverse directions

        // scaling factors
        s64 m_scalepos;             // scale factor to apply to positive adjusted values
        s64 m_scaleneg;             // scale factor to apply to negative adjusted values
        s64 m_keyscalepos;          // scale factor to apply to the key delta field when pos
        s64 m_keyscaleneg;          // scale factor to apply to the key delta field when neg
        s64 m_positionalscale;      // scale factor to divide a joystick into positions

        // misc flags
        bool m_absolute;             // is this an absolute or relative input?
        bool m_wraps;                // does the control wrap around?
        bool m_autocenter;           // autocenter this input?
        bool m_single_scale;         // scale joystick differently if default is between min/max
        bool m_interpolate;          // should we do linear interpolation for mid-frame reads?
        bool m_lastdigital;          // was the last modification caused by a digital form?
        bool m_was_written;          // was the last modification caused programmatically?


        // construction/destruction
        //-------------------------------------------------
        //  analog_field - constructor
        //-------------------------------------------------
        public analog_field(ioport_field field)
        {
            m_next = null;
            m_field = field;
            m_shift = 0;
            m_adjdefvalue = (int)(field.defvalue() & field.mask());
            m_adjmin = (int)(field.minval() & field.mask());
            m_adjmax = (int)(field.maxval() & field.mask());
            m_sensitivity = field.sensitivity();
            m_reverse = field.analog_reverse();
            m_delta = field.delta();
            m_centerdelta = field.centerdelta();
            m_accum = 0;
            m_previous = 0;
            m_previousanalog = 0;
            m_prog_analog_value = 0;
            m_minimum = inputdev_global.INPUT_ABSOLUTE_MIN;
            m_maximum = inputdev_global.INPUT_ABSOLUTE_MAX;
            m_center = 0;
            m_reverse_val = 0;
            m_scalepos = 0;
            m_scaleneg = 0;
            m_keyscalepos = 0;
            m_keyscaleneg = 0;
            m_positionalscale = 0;
            m_absolute = false;
            m_wraps = false;
            m_autocenter = false;
            m_single_scale = false;
            m_interpolate = false;
            m_lastdigital = false;
            m_was_written = false;


            // compute the shift amount and number of bits
            for (ioport_value mask = field.mask(); ((int)mask & 1) == 0; mask >>= 1)
                m_shift++;

            // initialize core data
            m_adjdefvalue >>= m_shift;
            m_adjmin >>= m_shift;
            m_adjmax >>= m_shift;

            // set basic parameters based on the configured type
            switch (field.type())
            {
                // paddles and analog joysticks are absolute and autocenter
                case ioport_type.IPT_AD_STICK_X:
                case ioport_type.IPT_AD_STICK_Y:
                case ioport_type.IPT_AD_STICK_Z:
                case ioport_type.IPT_PADDLE:
                case ioport_type.IPT_PADDLE_V:
                    m_absolute = true;
                    m_autocenter = true;
                    m_interpolate = !field.analog_reset();
                    break;

                // pedals start at and autocenter to the min range
                case ioport_type.IPT_PEDAL:
                case ioport_type.IPT_PEDAL2:
                case ioport_type.IPT_PEDAL3:
                    m_center = inputdev_global.INPUT_ABSOLUTE_MIN;
                    m_accum = apply_inverse_sensitivity(m_center);
                    m_absolute = true;
                    m_autocenter = true;
                    m_interpolate = !field.analog_reset();
                    break;

                // lightguns are absolute as well, but don't autocenter and don't interpolate their values
                case ioport_type.IPT_LIGHTGUN_X:
                case ioport_type.IPT_LIGHTGUN_Y:
                    m_absolute = true;
                    m_autocenter = false;
                    m_interpolate = false;
                    break;

                // positional devices are absolute, but can also wrap like relative devices
                // set each position to be 512 units
                case ioport_type.IPT_POSITIONAL:
                case ioport_type.IPT_POSITIONAL_V:
                    m_positionalscale = ioport_global.compute_scale((int)field.maxval(), inputdev_global.INPUT_ABSOLUTE_MAX - inputdev_global.INPUT_ABSOLUTE_MIN);
                    m_adjmin = 0;
                    m_adjmax = (int)field.maxval() - 1;
                    m_wraps = field.analog_wraps();
                    m_autocenter = !m_wraps;
                    break;

                // dials, mice and trackballs are relative devices
                // these have fixed "min" and "max" values based on how many bits are in the port
                // in addition, we set the wrap around min/max values to 512 * the min/max values
                // this takes into account the mapping that one mouse unit ~= 512 analog units
                case ioport_type.IPT_DIAL:
                case ioport_type.IPT_DIAL_V:
                case ioport_type.IPT_TRACKBALL_X:
                case ioport_type.IPT_TRACKBALL_Y:
                case ioport_type.IPT_MOUSE_X:
                case ioport_type.IPT_MOUSE_Y:
                    m_absolute = false;
                    m_wraps = true;
                    m_interpolate = !field.analog_reset();
                    break;

                default:
                    throw new emu_fatalerror("Unknown analog port type -- don't know if it is absolute or not\n");
            }

            // further processing for absolute controls
            if (m_absolute)
            {
                // if the default value is pegged at the min or max, use a single scale value for the whole axis
                m_single_scale = (m_adjdefvalue == m_adjmin) || (m_adjdefvalue == m_adjmax);

                // if not "single scale", compute separate scales for each side of the default
                if (!m_single_scale)
                {
                    // unsigned
                    m_scalepos = ioport_global.compute_scale(m_adjmax - m_adjdefvalue, inputdev_global.INPUT_ABSOLUTE_MAX - 0);
                    m_scaleneg = ioport_global.compute_scale(m_adjdefvalue - m_adjmin, 0 - inputdev_global.INPUT_ABSOLUTE_MIN);

                    if (m_adjmin > m_adjmax)
                        m_scaleneg = -m_scaleneg;

                    // reverse point is at center
                    m_reverse_val = 0;
                }
                else
                {
                    // single axis that increases from default
                    m_scalepos = ioport_global.compute_scale(m_adjmax - m_adjmin, inputdev_global.INPUT_ABSOLUTE_MAX - inputdev_global.INPUT_ABSOLUTE_MIN);

                    // make the scaling the same for easier coding when we need to scale
                    m_scaleneg = m_scalepos;

                    // reverse point is at max
                    m_reverse_val = m_maximum;
                }
            }

            // relative and positional controls all map directly with a 512x scale factor
            else
            {
                // The relative code is set up to allow specifing PORT_MINMAX and default values.
                // The validity checks are purposely set up to not allow you to use anything other
                // a default of 0 and PORT_MINMAX(0,mask).  This is in case the need arises to use
                // this feature in the future.  Keeping the code in does not hurt anything.
                if (m_adjmin > m_adjmax)
                    // adjust for signed
                    m_adjmin = -m_adjmin;

                if (m_wraps)
                    m_adjmax++;

                m_minimum = (m_adjmin - m_adjdefvalue) * inputdev_global.INPUT_RELATIVE_PER_PIXEL;
                m_maximum = (m_adjmax - m_adjdefvalue) * inputdev_global.INPUT_RELATIVE_PER_PIXEL;

                // make the scaling the same for easier coding when we need to scale
                m_scaleneg = m_scalepos = ioport_global.compute_scale(1, inputdev_global.INPUT_RELATIVE_PER_PIXEL);

                if (m_field.analog_reset())
                    // delta values reverse from center
                    m_reverse_val = 0;
                else
                {
                    // positional controls reverse from their max range
                    m_reverse_val = m_maximum + m_minimum;

                    // relative controls reverse from 1 past their max range
                    if (m_wraps)
                    {
                        // FIXME: positional needs -1, using INPUT_RELATIVE_PER_PIXEL skips a position (and reads outside the table array)
                        if(field.type() == ioport_type.IPT_POSITIONAL || field.type() == ioport_type.IPT_POSITIONAL_V)
                            m_reverse_val --;
                        else
                            m_reverse_val -= inputdev_global.INPUT_RELATIVE_PER_PIXEL;
                    }
                }
            }

            // compute scale for keypresses
            m_keyscalepos = ioport_global.recip_scale(m_scalepos);
            m_keyscaleneg = ioport_global.recip_scale(m_scaleneg);
        }


        // getters
        public analog_field next() { return m_next; }
        public analog_field m_next_get() { return m_next; }
        public void m_next_set(analog_field value) { m_next = value; }

        ioport_manager manager() { return m_field.manager(); }
        //ioport_field &field() const { return m_field; }
        //INT32 sensitivity() const { return m_sensitivity; }
        //bool reverse() const { return m_reverse; }
        //INT32 delta() const { return m_delta; }
        //INT32 centerdelta() const { return m_centerdelta; }


        // readers

        //-------------------------------------------------
        //  read - read the current value and insert into
        //  the provided ioport_value
        //-------------------------------------------------
        public void read(ref ioport_value result)
        {
            // do nothing if we're not enabled
            if (!m_field.enabled())
                return;

            // start with the raw value
            int value = m_accum;

            // interpolate if appropriate and if time has passed since the last update
            if (m_interpolate)
                value = manager().frame_interpolate(m_previous, m_accum);

            // apply standard analog settings
            value = apply_settings(value);

            // remap the value if needed
            if (m_field.remap_table() != null)
                value = (int)m_field.remap_table()[value];

            // invert bits if needed
            if (m_field.analog_invert())
                value = ~value;

            // insert into the port
            result = (ioport_value)((result & ~m_field.mask()) | ((value << m_shift) & m_field.mask()));
        }


        //-------------------------------------------------
        //  crosshair_read - read a value for crosshairs,
        //  scaled between 0 and 1
        //-------------------------------------------------
        public float crosshair_read()
        {
            int rawvalue = apply_settings(m_accum) & ((int)m_field.mask() >> m_shift);
            return (float)(rawvalue - m_adjmin) / (float)(m_adjmax - m_adjmin);
        }


        //-------------------------------------------------
        //  frame_update - update the internals of a
        //  single analog field periodically
        //-------------------------------------------------
        public void frame_update(running_machine machine)
        {
            // clamp the previous value to the min/max range
            if (!m_wraps)
                m_accum = apply_min_max(m_accum);

            // remember the previous value in case we need to interpolate
            m_previous = m_accum;

            // get the new raw analog value and its type
            input_item_class itemclass;
            s32 rawvalue = machine.input().seq_axis_value(m_field.seq(input_seq_type.SEQ_TYPE_STANDARD), out itemclass);

            // use programmatically set value if avaiable
            if (m_was_written)
            {
                m_was_written = false;
                rawvalue = m_prog_analog_value;
            }

            // if we got an absolute input, it overrides everything else
            if (itemclass == input_item_class.ITEM_CLASS_ABSOLUTE)
            {
                if (m_previousanalog != rawvalue)
                {
                    // only update if analog value changed
                    m_previousanalog = rawvalue;

                    // apply the inverse of the sensitivity to the raw value so that
                    // it will still cover the full min->max range requested after
                    // we apply the sensitivity adjustment
                    if (m_absolute || m_field.analog_reset())
                    {
                        // if port is absolute, then just return the absolute data supplied
                        m_accum = apply_inverse_sensitivity(rawvalue);
                    }
                    else if (m_positionalscale != 0)
                    {
                        // if port is positional, we will take the full analog control and divide it
                        // into positions, that way as the control is moved full scale,
                        // it moves through all the positions
                        rawvalue = ioport_global.apply_scale(rawvalue - inputdev_global.INPUT_ABSOLUTE_MIN, m_positionalscale) * inputdev_global.INPUT_RELATIVE_PER_PIXEL + m_minimum;

                        // clamp the high value so it does not roll over
                        rawvalue = Math.Min(rawvalue, m_maximum);
                        m_accum = apply_inverse_sensitivity(rawvalue);
                    }
                    else
                    {
                        // if port is relative, we use the value to simulate the speed of relative movement
                        // sensitivity adjustment is allowed for this mode
                        m_accum += rawvalue;
                    }

                    m_lastdigital = false;
                    // do not bother with other control types if the analog data is changing
                    return;
                }
                else
                {
                    // we still have to update fake relative from joystick control
                    if (!m_absolute && m_positionalscale == 0)
                        m_accum += rawvalue;
                }
            }

            // if we got it from a relative device, use that as the starting delta
            // also note that the last input was not a digital one
            s32 delta = 0;
            if (itemclass == input_item_class.ITEM_CLASS_RELATIVE && rawvalue != 0)
            {
                delta = rawvalue;
                m_lastdigital = false;
            }

            s64 keyscale = (m_accum >= 0) ? m_keyscalepos : m_keyscaleneg;

            // if the decrement code sequence is pressed, add the key delta to
            // the accumulated delta; also note that the last input was a digital one
            bool keypressed = false;
            if (machine.input().seq_pressed(m_field.seq(input_seq_type.SEQ_TYPE_DECREMENT)))
            {
                keypressed = true;
                if (m_delta != 0)
                {
                    delta -= ioport_global.apply_scale(m_delta, keyscale);
                }
                else if (!m_lastdigital)
                {
                    // decrement only once when first pressed
                    delta -= ioport_global.apply_scale(1, keyscale);
                }

                m_lastdigital = true;
            }

            // same for the increment code sequence
            if (machine.input().seq_pressed(m_field.seq(input_seq_type.SEQ_TYPE_INCREMENT)))
            {
                keypressed = true;
                if (m_delta != 0)
                {
                    delta += ioport_global.apply_scale(m_delta, keyscale);
                }
                else if (!m_lastdigital)
                {
                    // increment only once when first pressed
                    delta += ioport_global.apply_scale(1, keyscale);
                }

                m_lastdigital = true;
            }

            // if resetting is requested, clear the accumulated position to 0 before
            // applying the deltas so that we only return this frame's delta
            // note that centering only works for relative controls
            // no need to check if absolute here because it is checked by the validity tests
            if (m_field.analog_reset())
                m_accum = 0;

            // apply the delta to the accumulated value
            m_accum += delta;

            // if our last movement was due to a digital input, and if this control
            // type autocenters, and if neither the increment nor the decrement seq
            // was pressed, apply autocentering
            if (m_autocenter)
            {
                s32 center = apply_inverse_sensitivity(m_center);
                if (m_lastdigital && !keypressed)
                {
                    // autocenter from positive values
                    if (m_accum >= center)
                    {
                        m_accum -= ioport_global.apply_scale(m_centerdelta, m_keyscalepos);
                        if (m_accum < center)
                        {
                            m_accum = center;
                            m_lastdigital = false;
                        }
                    }

                    // autocenter from negative values
                    else
                    {
                        m_accum += ioport_global.apply_scale(m_centerdelta, m_keyscaleneg);
                        if (m_accum > center)
                        {
                            m_accum = center;
                            m_lastdigital = false;
                        }
                    }
                }
            }
            else if (!keypressed)
            {
                m_lastdigital = false;
            }
        }


        // setters
        //-------------------------------------------------
        //  set_value - take a new value to be used
        //  at next frame update
        //-------------------------------------------------
        public void set_value(s32 value)
        {
            m_was_written = true;
            m_prog_analog_value = value;
        }


        // helpers

        //-------------------------------------------------
        //  apply_min_max - clamp the given input value to
        //  the appropriate min/max for the analog control
        //-------------------------------------------------
        int apply_min_max(int value)
        {
            // take the analog minimum and maximum values and apply the inverse of the
            // sensitivity so that we can clamp against them before applying sensitivity
            int adjmin = apply_inverse_sensitivity(m_minimum);
            int adjmax = apply_inverse_sensitivity(m_maximum);

            // clamp to the bounds absolutely
            if (value > adjmax)
                value = adjmax;
            else if (value < adjmin)
                value = adjmin;

            return value;
        }

        //-------------------------------------------------
        //  apply_settings - return the value of an
        //  analog input
        //-------------------------------------------------
        int apply_settings(int value)
        {
            // apply the min/max and then the sensitivity
            if (!m_wraps)
                value = apply_min_max(value);
            value = apply_sensitivity(value);

            // apply reversal if needed
            if (m_reverse)
                value = m_reverse_val - value;
            else if (m_single_scale)
                // it's a pedal or the default value is equal to min/max
                // so we need to adjust the center to the minimum
                value -= inputdev_global.INPUT_ABSOLUTE_MIN;

            // map differently for positive and negative values
            if (value >= 0)
                value = ioport_global.apply_scale(value, m_scalepos);
            else
                value = ioport_global.apply_scale(value, m_scaleneg);
            value += m_adjdefvalue;

            // for relative devices, wrap around when we go past the edge
            // (this is done last to prevent rounding errors)
            if (m_wraps)
            {
                int range = m_adjmax - m_adjmin;
                // rolls to other end when 1 position past end.
                value = (value - m_adjmin) % range;
                if (value < 0)
                    value += range;
                value += m_adjmin;
            }

            return value;
        }

        //-------------------------------------------------
        //  apply_sensitivity - apply a sensitivity
        //  adjustment for a current value
        //-------------------------------------------------
        int apply_sensitivity(int value)
        {
            return lround(((Int64)value * m_sensitivity) / 100.0);
        }

        //-------------------------------------------------
        //  apply_inverse_sensitivity - reverse-apply the
        //  sensitivity adjustment for a current value
        //-------------------------------------------------
        int apply_inverse_sensitivity(int value)
        {
            return (int)(((Int64)value * 100) / m_sensitivity);
        }
    }


    // ======================> dynamic_field
    // live device field information
    public class dynamic_field : simple_list_item<dynamic_field>
    {
        // internal state
        dynamic_field m_next;             // linked list of info for this port
        ioport_field m_field;            // reference to the input field
        u8 m_shift;            // shift to apply to the final result
        ioport_value m_oldval;           // last value


        // construction/destruction
        //-------------------------------------------------
        //  dynamic_field - constructor
        //-------------------------------------------------
        public dynamic_field(ioport_field field)
        {
            m_next = null;
            m_field = field;
            m_shift = 0;
            m_oldval = field.defvalue();


            // fill in the data
            for (ioport_value mask = field.mask(); !((mask & 1) != 0); mask >>= 1)
                m_shift++;

            m_oldval >>= m_shift;
        }


        // getters
        public dynamic_field next() { return m_next; }
        public dynamic_field m_next_get() { return m_next; }
        public void m_next_set(dynamic_field value) { m_next = value; }

        public ioport_field field() { return m_field; }


        // read/write

        //-------------------------------------------------
        //  read - read the updated value and merge it
        //  into the target
        //-------------------------------------------------
        public void read(ref ioport_value result)
        {
            // skip if not enabled
            if (!m_field.enabled())
                return;

            // call the callback to read a new value
            ioport_value newval = m_field.m_read();
            m_oldval = newval;

            // merge in the bits (don't invert yet, as all digitals are inverted together)
            result = (result & ~m_field.mask()) | ((newval << m_shift) & m_field.mask());
        }

        //-------------------------------------------------
        //  write - track a change to a value and call
        //  the write callback if there's something new
        //-------------------------------------------------
        public void write(ioport_value newval)
        {
            // skip if not enabled
            if (!m_field.enabled())
                return;

            // if the bits have changed, call the handler
            newval = (newval & m_field.mask()) >> m_shift;
            if (m_oldval != newval)
            {
                m_field.m_write(m_field, m_field.m_write_param, m_oldval, newval);
                m_oldval = newval;
            }
        }
    }


    // ======================> ioport_port_live
    // internal live state of an input port
    public class ioport_port_live
    {
        // public state
        public simple_list<analog_field> analoglist = new simple_list<analog_field>();       // list of analog port info
        public simple_list<dynamic_field> readlist = new simple_list<dynamic_field>();        // list of dynamic read fields
        public simple_list<dynamic_field> writelist = new simple_list<dynamic_field>();       // list of dynamic write fields
        public ioport_value defvalue;           // combined default value across the port
        public ioport_value digital;            // current value from all digital inputs
        public ioport_value outputvalue;        // current value for outputs


        // construction/destruction
        //-------------------------------------------------
        //  ioport_port_live - constructor
        //-------------------------------------------------
        public ioport_port_live(ioport_port port)
        {
            defvalue = 0;
            digital = 0;
            outputvalue = 0;


            // iterate over fields
            foreach (ioport_field field in port.fields())
            {
                // allocate analog state if it's analog
                analog_field analog = null;
                if (field.is_analog())
                    analog = analoglist.append(new analog_field(field));

                // allocate a dynamic field for reading
                if (field.has_dynamic_read())
                    readlist.append(new dynamic_field(field));

                // allocate a dynamic field for writing
                if (field.has_dynamic_write())
                    writelist.append(new dynamic_field(field));

                // let the field initialize its live state
                field.init_live_state(analog);
            }
        }
    }


    // ======================> ioport_configurer
    // class to wrap helper functions
    public class ioport_configurer
    {
        // internal state
        device_t m_owner;
        ioport_list m_portlist;
        string m_errorbuf;

        ioport_port m_curport;
        ioport_field m_curfield;
        ioport_setting m_cursetting;


        // construction/destruction
        //-------------------------------------------------
        //  ioport_configurer - constructor
        //-------------------------------------------------
        public ioport_configurer(device_t owner, ioport_list portlist, ref string errorbuf)
        {
            m_owner = owner;
            m_portlist = portlist;
            m_errorbuf = errorbuf;
            m_curport = null;
            m_curfield = null;
            m_cursetting = null;
        }


        // static helpers

        //-------------------------------------------------
        //  string_from_token - convert an
        //  ioport_token to a default string
        //-------------------------------------------------
        public static string string_from_token(string str)
        {
            // 0 is an invalid index
            if (string.IsNullOrEmpty(str))
                return null;

            // if the index is greater than the count, assume it to be a pointer
            //if (uintptr_t(string) >= INPUT_STRING_COUNT)
            //    return string;
            if (!str.StartsWith(ioport_global.DEFAULT_STR_PREFIX))
                return str;

#if false // Set TRUE, If you want to take care missing-token or wrong-sorting

            // otherwise, scan the list for a matching string and return it
            {
            int index;
            for (index = 0; index < ARRAY_LENGTH(input_port_default_strings); index++)
                if (input_port_default_strings[index].id == FPTR(string))
                    return input_port_default_strings[index].string;
            }
            return "(Unknown Default)";

#else

            //return input_port_default_strings[uintptr_t(string)-1].string;
            var replace_str = str.Replace(ioport_global.DEFAULT_STR_PREFIX, "");
            INPUT_STRING result;
            if (Enum.TryParse(replace_str, out result))
                return ioport_global.input_port_default_strings[result];
            else
                throw new emu_unimplemented();

#endif
        }


        // port helpers

        //-------------------------------------------------
        //  port_alloc - allocate a new port
        //-------------------------------------------------
        public ioport_configurer port_alloc(string tag)
        {
            // create the full tag
            string fulltag = m_owner.subtag(tag);

            // add it to the list, and reset current field/setting
            if (m_portlist.find(fulltag) != null) throw new tag_add_exception(fulltag);
            m_portlist.emplace(fulltag, new ioport_port(m_owner, fulltag));
            m_curport = m_portlist.find(fulltag);
            m_curfield = null;
            m_cursetting = null;
            return this;
        }


        //-------------------------------------------------
        //  port_modify - find an existing port and
        //  modify it
        //-------------------------------------------------
        public ioport_configurer port_modify(string tag)
        {
            // create the full tag
            string fulltag = m_owner.subtag(tag);

            // find the existing port
            m_curport = m_portlist.find(fulltag);
            if (m_curport == null)
                throw new emu_fatalerror("Requested to modify nonexistent port '{0}'", fulltag);

            // bump the modification count, and reset current field/setting
            m_curport.modcount_set(m_curport.modcount() + 1);  //m_curport.m_modcount++;
            m_curfield = null;
            m_cursetting = null;
            return this;
        }


        // field helpers

        //-------------------------------------------------
        //  field_alloc - allocate a new field
        //-------------------------------------------------
        public ioport_configurer field_alloc(ioport_type type, ioport_value defval, ioport_value mask, string name = null)
        {
            // make sure we have a port
            if (m_curport == null)
                throw new emu_fatalerror("alloc_field called with no active port (mask={0} defval={1})\n", mask, defval);

            // append the field
            if (type != ioport_type.IPT_UNKNOWN && type != ioport_type.IPT_UNUSED)
                m_curport.active_set(m_curport.active() | mask);  //m_curport.m_active |= mask;

            m_curfield = m_curport.fields().append(new ioport_field(m_curport, type, defval, mask, string_from_token(name)));

            // reset the current setting
            m_cursetting = null;
            return this;
        }

        //ioport_configurer& field_add_char(std::initializer_list<char32_t> charlist);

        //-------------------------------------------------
        //  field_add_code - add a character to a field
        //-------------------------------------------------
        public ioport_configurer field_add_code(input_seq_type which, input_code code) { m_curfield.m_seq[(int)which].append_code_to_sequence_or(code); return this; }  //{ m_curfield.m_seq[which] |= code; return this; }

        public ioport_configurer field_set_way(int way) { m_curfield.m_way = (u8)way; return this; }  //{ m_curfield->m_way = way; return *this; }
        //ioport_configurer field_set_rotated() const { m_curfield->m_flags |= ioport_field::FIELD_FLAG_ROTATED; }
        public ioport_configurer field_set_name(string name) { global_object.assert(m_curfield != null); m_curfield.m_name = string_from_token(name); return this; }
        public ioport_configurer field_set_player(int player) { m_curfield.set_player((byte)(player - 1)); return this; }
        public ioport_configurer field_set_cocktail() { m_curfield.m_flags |= ioport_field.FIELD_FLAG_COCKTAIL;  field_set_player(2); return this; }  //  m_curfield.m_flags |= ioport_field.FIELD_FLAG_COCKTAIL; field_set_player(2); }
        ioport_configurer field_set_toggle() { m_curfield.m_flags |= ioport_field.FIELD_FLAG_TOGGLE; return this; }  //{ m_curfield.m_flags |= ioport_field::FIELD_FLAG_TOGGLE; }
        public ioport_configurer field_set_impulse(u8 impulse) { m_curfield.m_impulse = impulse; return this; }
        public ioport_configurer field_set_analog_reverse() { m_curfield.m_flags |= ioport_field.ANALOG_FLAG_REVERSE; return this; }
        //ioport_configurer field_set_analog_reset() const { m_curfield->m_flags |= ioport_field::ANALOG_FLAG_RESET; }
        //ioport_configurer field_set_optional() const { m_curfield->m_flags |= ioport_field::FIELD_FLAG_OPTIONAL; }
        public ioport_configurer field_set_min_max(ioport_value minval, ioport_value maxval) { m_curfield.m_min = minval; m_curfield.m_max = maxval; return this; }
        public ioport_configurer field_set_sensitivity(s32 sensitivity) { m_curfield.m_sensitivity = sensitivity; return this; }
        public ioport_configurer field_set_delta(s32 delta) { m_curfield.m_centerdelta = m_curfield.m_delta = delta; return this; }
        //ioport_configurer field_set_centerdelta(INT32 delta) const { m_curfield->m_centerdelta = delta; }
        //ioport_configurer field_set_crosshair(crosshair_axis_t axis, double altaxis, double scale, double offset) const { m_curfield->m_crosshair_axis = axis; m_curfield->m_crosshair_altaxis = altaxis; m_curfield->m_crosshair_scale = scale; m_curfield->m_crosshair_offset = offset; }
        //ioport_configurer field_set_crossmapper(ioport_field_crossmap_delegate callback) const { m_curfield->m_crosshair_mapper = callback; }
        public ioport_configurer field_set_full_turn_count(u16 count) { m_curfield.m_full_turn_count = count; return this; }
        //ioport_configurer field_set_analog_wraps() const { m_curfield->m_flags |= ioport_field::ANALOG_FLAG_WRAPS; }
        //ioport_configurer field_set_remap_table(const ioport_value *table) { m_curfield->m_remap_table = table; }
        //ioport_configurer field_set_analog_invert() const { m_curfield->m_flags |= ioport_field::ANALOG_FLAG_INVERT; }
        public ioport_configurer field_set_dynamic_read(ioport_field_read_delegate callback) { m_curfield.m_read = callback; return this; }
        //ioport_configurer& field_set_dynamic_write(ioport_field_write_delegate delegate, u32 param = 0) { m_curfield->m_write = delegate; m_curfield->m_write_param = param; return *this; }
        public ioport_configurer field_set_diplocation(string location) { m_curfield.expand_diplocation(location, ref m_errorbuf); return this; }


        // setting helpers
        //-------------------------------------------------
        //  setting_alloc - allocate a new setting
        //-------------------------------------------------
        public ioport_configurer setting_alloc(ioport_value value, string name)
        {
            // make sure we have a field
            if (m_curfield == null)
                throw new emu_fatalerror("alloc_setting called with no active field (value={0} name={1})\n", value, name);

            m_cursetting = new ioport_setting(m_curfield, value & m_curfield.mask(), string_from_token(name));
            // append a new setting
            m_curfield.settings().append(m_cursetting);
            return this;
        }


        // misc helpers

        //-------------------------------------------------
        //  set_condition - set the condition for either
        //  the current setting or field
        //-------------------------------------------------
        public ioport_configurer set_condition(ioport_condition.condition_t condition, string tag, ioport_value mask, ioport_value value)
        {
            ioport_condition target = (m_cursetting != null) ? m_cursetting.condition() : m_curfield.condition();
            target.set(condition, tag, mask, value);
            return this;
        }

        //-------------------------------------------------
        //  onoff_alloc - allocate an on/off DIP switch
        //-------------------------------------------------
        public ioport_configurer onoff_alloc(string name, ioport_value defval, ioport_value mask, string diplocation)
        {
            // allocate a field normally
            field_alloc(ioport_type.IPT_DIPSWITCH, defval, mask, name);

            // expand the diplocation
            if (!string.IsNullOrEmpty(diplocation))
                field_set_diplocation(diplocation);

            // allocate settings
            setting_alloc(defval & mask, ioport_global.input_port_default_strings[INPUT_STRING.INPUT_STRING_Off]);
            setting_alloc(~defval & mask, ioport_global.input_port_default_strings[INPUT_STRING.INPUT_STRING_On]);

            // clear cursettings set by setting_alloc
            m_cursetting = null;
            return this;
        }
    }


    // ======================> ioport_manager
    // private input port state
    public class ioport_manager : global_object
    {
        // XML attributes for the different types
        static readonly string [] seqtypestrings = { "standard", "increment", "decrement" };


        // internal state
        running_machine m_machine;              // reference to owning machine
        bool m_safe_to_read;         // clear at start; set after state is loaded
        ioport_list m_portlist = new ioport_list();             // list of input port configurations

        // types
        std.vector<input_type_entry> m_typelist = new std.vector<input_type_entry>();       // list of live type states
        input_type_entry [,] m_type_to_entry = new input_type_entry[(int)ioport_type.IPT_COUNT, ioport_global.MAX_PLAYERS]; // map from type/player to type state

        // specific special global input states
        simple_list<digital_joystick> m_joystick_list = new simple_list<digital_joystick>();  // list of digital joysticks

        // frame time tracking
        attotime m_last_frame_time;      // time of the last frame callback
        attoseconds_t m_last_delta_nsec;      // nanoseconds that passed since the previous callback

        // playback/record information
        emu_file m_record_file;          // recording file (NULL if not recording)
        emu_file m_playback_file;        // playback file (NULL if not recording)
        u64 m_playback_accumulated_speed; // accumulated speed during playback
        u32 m_playback_accumulated_frames; // accumulated frames during playback
        emu_file m_timecode_file;        // timecode/frames playback file (nullptr if not recording)
        int m_timecode_count;
        attotime m_timecode_last_time;


        // construction/destruction

        //-------------------------------------------------
        //  ioport_manager - constructor
        //-------------------------------------------------
        public ioport_manager(running_machine machine)
        {
            m_machine = machine;
            m_safe_to_read = false;
            m_last_frame_time = attotime.zero;
            m_last_delta_nsec = 0;
            m_record_file = new emu_file(machine.options().input_directory(), osdfile_global.OPEN_FLAG_WRITE | osdfile_global.OPEN_FLAG_CREATE | osdfile_global.OPEN_FLAG_CREATE_PATHS);
            m_playback_file = new emu_file(machine.options().input_directory(), osdfile_global.OPEN_FLAG_READ);
            m_playback_accumulated_speed = 0;
            m_playback_accumulated_frames = 0;
            m_timecode_file = new emu_file(machine.options().input_directory(), osdfile_global.OPEN_FLAG_WRITE | osdfile_global.OPEN_FLAG_CREATE | osdfile_global.OPEN_FLAG_CREATE_PATHS);
            m_timecode_count = 0;
            m_timecode_last_time = attotime.zero;


            //memset(m_type_to_entry, 0, sizeof(m_type_to_entry));
        }

        //-------------------------------------------------
        //  initialize - walk the configured ports and
        //  create live state information
        //-------------------------------------------------
        public time_t initialize()
        {
            // add an exit callback and a frame callback
            machine().add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, exit);
            machine().add_notifier(machine_notification.MACHINE_NOTIFY_FRAME, frame_update_callback);

            // initialize the default port info from the OSD
            init_port_types();

            // if we have a token list, proceed
            device_enumerator iter = new device_enumerator(machine().root_device());
            foreach (var device in iter)
            {
                string errors;
                m_portlist.append(device, out errors);
                if (!string.IsNullOrEmpty(errors))
                    osd_printf_error("Input port errors:\n{0}", errors);
            }

            // renumber player numbers for controller ports
            int player_offset = 0;
            foreach (device_t device in iter)
            {
                int players = 0;
                foreach (var port in m_portlist)
                {
                    if (port.Value.device() == device)
                    {
                        foreach (ioport_field field in port.Value.fields())
                        {
                            if (field.type_class() == ioport_type_class.INPUT_CLASS_CONTROLLER)
                            {
                                if (players < field.player() + 1)
                                    players = field.player() + 1;
                                field.set_player((u8)(field.player() + player_offset));
                            }
                        }
                    }
                }

                player_offset += players;
            }

            // allocate live structures to mirror the configuration
            foreach (var port in m_portlist)
                port.Value.init_live_state();

            // handle autoselection of devices
            init_autoselect_devices((int)ioport_type.IPT_AD_STICK_X,  (int)ioport_type.IPT_AD_STICK_Y,   (int)ioport_type.IPT_AD_STICK_Z, emu_options.OPTION_ADSTICK_DEVICE,    "analog joystick");
            init_autoselect_devices((int)ioport_type.IPT_PADDLE,      (int)ioport_type.IPT_PADDLE_V,     0,                               emu_options.OPTION_PADDLE_DEVICE,     "paddle");
            init_autoselect_devices((int)ioport_type.IPT_PEDAL,       (int)ioport_type.IPT_PEDAL2,       (int)ioport_type.IPT_PEDAL3,     emu_options.OPTION_PEDAL_DEVICE,      "pedal");
            init_autoselect_devices((int)ioport_type.IPT_LIGHTGUN_X,  (int)ioport_type.IPT_LIGHTGUN_Y,   0,                               emu_options.OPTION_LIGHTGUN_DEVICE,   "lightgun");
            init_autoselect_devices((int)ioport_type.IPT_POSITIONAL,  (int)ioport_type.IPT_POSITIONAL_V, 0,                               emu_options.OPTION_POSITIONAL_DEVICE, "positional");
            init_autoselect_devices((int)ioport_type.IPT_DIAL,        (int)ioport_type.IPT_DIAL_V,       0,                               emu_options.OPTION_DIAL_DEVICE,       "dial");
            init_autoselect_devices((int)ioport_type.IPT_TRACKBALL_X, (int)ioport_type.IPT_TRACKBALL_Y,  0,                               emu_options.OPTION_TRACKBALL_DEVICE,  "trackball");
            init_autoselect_devices((int)ioport_type.IPT_MOUSE_X,     (int)ioport_type.IPT_MOUSE_Y,      0,                               emu_options.OPTION_MOUSE_DEVICE,      "mouse");

            // look for 4-way diagonal joysticks and change the default map if we find any
            string joystick_map_default = machine().options().joystick_map();
            if (string.IsNullOrEmpty(joystick_map_default) || joystick_map_default == "auto")
            {
                foreach (var port in m_portlist)
                {
                    foreach (ioport_field field in port.Value.fields())
                    {
                        if (field.live().joystick != null && field.rotated())
                        {
                            input_class_joystick devclass = (input_class_joystick)machine().input().device_class(input_device_class.DEVICE_CLASS_JOYSTICK);
                            devclass.set_global_joystick_map(input_class_joystick.map_4way_diagonal);
                            break;
                        }
                    }
                }
            }

            // register callbacks for when we load configurations
            machine().configuration().config_register("input", load_config, save_config);

            // open playback and record files if specified
            time_t basetime = playback_init();
            record_init();
            timecode_init();

            return basetime;
        }


        // getters
        running_machine machine() { return m_machine; }
        public ioport_list ports() { return m_portlist; }
        public bool safe_to_read() { return m_safe_to_read; }


        // type helpers

        std.vector<input_type_entry> types() { return m_typelist; }

        //-------------------------------------------------
        //  type_pressed - return true if the sequence for
        //  the given input type/player is pressed
        //-------------------------------------------------
        public bool type_pressed(ioport_type type, int player = 0) { return machine().input().seq_pressed(type_seq(type, player)); }

        //-------------------------------------------------
        //  type_name - return the name for the given
        //  type/player
        //-------------------------------------------------
        public string type_name(ioport_type type, byte player)
        {
            // if we have a machine, use the live state and quick lookup
            input_type_entry entry = m_type_to_entry[(int)type, player];
            if (entry != null && entry.name() != null)
                return entry.name();

            // if we find nothing, return a default string (not a null pointer)
            return "???";
        }


        public ioport_group type_group(ioport_type type, int player)
        {
            input_type_entry entry = m_type_to_entry[(int)type, player];
            if (entry != null)
                return entry.group();

            // if we find nothing, return an invalid group
            return ioport_group.IPG_INVALID;
        }


        //-------------------------------------------------
        //  type_seq - return the input sequence for the
        //  given type/player
        //-------------------------------------------------
        public input_seq type_seq(ioport_type type, int player = 0, input_seq_type seqtype = input_seq_type.SEQ_TYPE_STANDARD)
        {
            //assert(type >= 0 && type < IPT_COUNT);
            //assert(player >= 0 && player < MAX_PLAYERS);

            // if we have a machine, use the live state and quick lookup
            input_type_entry entry = m_type_to_entry[(int)type, player];
            if (entry != null)
                return entry.seq(seqtype);

            // if we find nothing, return an empty sequence
            return input_seq.empty_seq;
        }

        //void set_type_seq(ioport_type type, int player, input_seq_type seqtype, const input_seq &newseq);
        //static bool type_is_analog(ioport_type type) { return (type > IPT_ANALOG_FIRST && type < IPT_ANALOG_LAST); }
        //bool type_class_present(ioport_type_class inputclass);


        // other helpers

        //-------------------------------------------------
        //  frame_update - core logic for per-frame input
        //  port updating
        //-------------------------------------------------
        public digital_joystick digjoystick(int player, int number)
        {
            // find it in the list
            foreach (digital_joystick joystick in m_joystick_list)
            {
                if (joystick.player() == player && joystick.number() == number)
                    return joystick;
            }

            // create a new one
            return m_joystick_list.append(new digital_joystick(player, number));
        }


        //int count_players() const;


        //-------------------------------------------------
        //  frame_interpolate - interpolate between two
        //  values based on the time between frames
        //-------------------------------------------------
        public int frame_interpolate(int oldval, int newval)
        {
            // if no last delta, just use new value
            if (m_last_delta_nsec == 0)
                return newval;

            // otherwise, interpolate
            attoseconds_t nsec_since_last = (machine().time() - m_last_frame_time).as_attoseconds() / attotime.ATTOSECONDS_PER_NANOSECOND;
            return (int)(oldval + ((Int64)(newval - oldval) * nsec_since_last / m_last_delta_nsec));
        }


        //ioport_type token_to_input_type(const char *string, int &player) const;
        //string input_type_to_token(ioport_type type, int player);


        // internal helpers

        //-------------------------------------------------
        //  init_port_types - initialize the default
        //  type list
        //-------------------------------------------------
        void init_port_types()
        {
            // convert the array into a list of type states that can be modified
            inpttype_global.emplace_core_types(m_typelist);

            // ask the OSD to customize the list
            machine().osd().customize_input_type_list(m_typelist);

            // now iterate over the OSD-modified types
            foreach (input_type_entry curtype in m_typelist)
            {
                // first copy all the OSD-updated sequences into our current state
                curtype.restore_default_seq();

                // also make a lookup table mapping type/player to the appropriate type list entry
                m_type_to_entry[(int)curtype.type(), curtype.player()] = curtype;
            }
        }

        //-------------------------------------------------
        //  init_autoselect_devices - autoselect a single
        //  device based on the input port list passed
        //  in and the corresponding option
        //-------------------------------------------------
        void init_autoselect_devices(int type1, int type2, int type3, string option, string ananame)
        {
            // if nothing specified, ignore the option
            string stemp = machine().options().value(option);
            if (string.IsNullOrEmpty(stemp) || strcmp(stemp, "none") == 0)
                return;

            // extract valid strings
            input_class autoenable_class = null;
            for (input_device_class devclass = input_device_class.DEVICE_CLASS_FIRST_VALID; devclass <= input_device_class.DEVICE_CLASS_LAST_VALID; ++devclass)
            {
                if (strcmp(stemp, machine().input().device_class(devclass).name()) == 0)
                {
                    autoenable_class = machine().input().device_class(devclass);
                    break;
                }
            }

            if (autoenable_class == null)
            {
                osd_printf_error("Invalid {0} value {1}; reverting to keyboard\n", option, stemp);
                autoenable_class = machine().input().device_class(input_device_class.DEVICE_CLASS_KEYBOARD);
            }

            // only scan the list if we haven't already enabled this class of control
            if (!autoenable_class.enabled())
            {
                foreach (var port in m_portlist)
                {
                    foreach (ioport_field field in port.Value.fields())
                    {
                        // if this port type is in use, apply the autoselect criteria
                        if ((type1 != 0 && (int)field.type() == type1) || (type2 != 0 && (int)field.type() == type2) || (type3 != 0 && (int)field.type() == type3))
                        {
                            osd_printf_verbose("Input: Autoenabling {0} due to presence of a {1}\n", autoenable_class.name(), ananame);
                            autoenable_class.enable();
                            break;
                        }
                    }
                }
            }
        }

        //-------------------------------------------------
        //  frame_update - callback for once/frame updating
        //-------------------------------------------------
        void frame_update_callback(running_machine machine_)
        {
            // if we're paused, don't do anything
            if (!machine().paused())
                frame_update();
        }

        //-------------------------------------------------
        //  frame_update_internal - core logic for
        //  per-frame input port updating
        //-------------------------------------------------
        void frame_update()
        {
            profiler_global.g_profiler.start(profile_type.PROFILER_INPUT);


            // record/playback information about the current frame
            attotime curtime = machine().time();

            playback_frame(curtime);
            record_frame(curtime);

            // track the duration of the previous frame
            m_last_delta_nsec = (curtime - m_last_frame_time).as_attoseconds() / attotime.ATTOSECONDS_PER_NANOSECOND;
            m_last_frame_time = curtime;

            // update the digital joysticks
            foreach (digital_joystick joystick in m_joystick_list)
                joystick.frame_update();

            // compute default values for all the ports
            // two passes to catch conditionals properly
            foreach (var port in m_portlist)
                port.Value.update_defvalue(true);
            foreach (var port in m_portlist)
                port.Value.update_defvalue(false);

            // loop over all input ports
            foreach (var port in m_portlist)
            {
                port.Value.frame_update();

                // handle playback/record
                playback_port(port.Value);
                record_port(port.Value);

                // call device line write handlers
                ioport_value newvalue = port.Value.read();
                foreach (dynamic_field dynfield in port.Value.live().writelist)
                {
                    if (dynfield.field().type() != ioport_type.IPT_OUTPUT)
                        dynfield.write(newvalue);
                }
            }


            profiler_global.g_profiler.stop();
        }


        public ioport_port port(string tag) { var search = m_portlist.find(tag); if (search != default) return search; else return null; }


        //-------------------------------------------------
        //  exit - exit callback to ensure we clean up
        //  and close our files
        //-------------------------------------------------
        void exit(running_machine machine_)
        {
            // close any playback or recording files
            playback_end();
            record_end();
            timecode_end();
        }


        //input_seq_type token_to_seq_type(const char *string);
        //static const char *const seqtypestrings[];


        //-------------------------------------------------
        //  load_config - callback to extract configuration
        //  data from the XML nodes
        //-------------------------------------------------
        void load_config(config_type cfg_type, util.xml.data_node parentnode)
        {
            // in the completion phase, we finish the initialization with the final ports
            if (cfg_type == config_type.FINAL)
            {
                m_safe_to_read = true;
                frame_update();
            }

            // early exit if no data to parse
            if (parentnode == null)
                return;

            // iterate over all the remap nodes for controller configs only
            if (cfg_type == config_type.CONTROLLER)
                load_remap_table(parentnode);

            throw new emu_unimplemented();
#if false
#endif
        }

        //-------------------------------------------------
        //  load_remap_table - extract and apply the
        //  global remapping table
        //-------------------------------------------------
        void load_remap_table(util.xml.data_node parentnode)
        {
            throw new emu_unimplemented();
        }

        //bool load_default_config(xml_data_node *portnode, int type, int player, const input_seq *newseq);
        //bool load_game_config(xml_data_node *portnode, int type, int player, const input_seq *newseq);

        //-------------------------------------------------
        //  save_config - config callback for saving input
        //  port configuration
        //-------------------------------------------------
        void save_config(config_type cfg_type, util.xml.data_node parentnode)
        {
            // if no parentnode, ignore
            if (parentnode == null)
                return;

            throw new emu_unimplemented();
        }

        //void save_sequence(xml_data_node *parentnode, input_seq_type type, ioport_type porttype, const input_seq &seq);
        //bool save_this_input_field_type(ioport_type type);
        //void save_default_inputs(xml_data_node *parentnode);
        //void save_game_inputs(xml_data_node *parentnode);

        //template<typename _Type> _Type playback_read(_Type &result);


        //-------------------------------------------------
        //  playback_init - initialize INP playback
        //-------------------------------------------------
        time_t playback_init()
        {
            // if no file, nothing to do
            string filename = machine().options().playback();
            if (string.IsNullOrEmpty(filename))
                return 0;

            // open the playback file
            osd_file.error filerr = m_playback_file.open(filename);

            // return an explicit error if file isn't found in given path
            if (filerr == osd_file.error.NOT_FOUND)
                fatalerror("Input file {0} not found\n", filename);

            // TODO: bail out any other error laconically for now
            if (filerr != osd_file.error.NONE)
                fatalerror("Failed to open file {0} for playback (code error={1})\n", filename, (int)filerr);

            // read the header and verify that it is a modern version; if not, print an error
            inp_header header = new inp_header();
            if (!header.read(m_playback_file))
                throw new emu_fatalerror("Input file is corrupt or invalid (missing header)\n");
            if (!header.check_magic())
                throw new emu_fatalerror("Input file invalid or in an older, unsupported format\n");
            if (header.get_majversion() != inp_header.MAJVERSION)
                throw new emu_fatalerror("Input file format version mismatch\n");

            // output info to console
            osd_printf_info("Input file: {0}\n", filename);
            osd_printf_info("INP version {0}.{1}", header.get_majversion(), header.get_minversion());  // %u.%u\n
            time_t basetime = (time_t)header.get_basetime();
            osd_printf_info("Created {0}\n", new DateTime(1970, 1, 1).ToLocalTime().AddSeconds(basetime).ToString());  //ctime(&basetime));
            osd_printf_info("Recorded using {0}\n", header.get_appdesc());

            // verify the header against the current game
            string sysname = header.get_sysname();
            if (sysname != machine().system().name)
                osd_printf_info("Input file is for machine '{0}', not for current machine '{1}'\n", sysname, machine().system().name);

            // enable compression
            m_playback_file.compress(util_.FCOMPRESS_MEDIUM);
            return basetime;
        }


        //-------------------------------------------------
        //  playback_end - end INP playback
        //-------------------------------------------------
        void playback_end(string message = null)
        {
            // only applies if we have a live file
            if (m_playback_file.is_open())
            {
                // close the file
                m_playback_file.close();

                // pop a message
                if (message != null)
                    machine().popmessage(string.Format("Playback Ended\nReason: {0}", message));

                // display speed stats
                if (m_playback_accumulated_speed > 0)
                    m_playback_accumulated_speed /= m_playback_accumulated_frames;
                osd_printf_info("Total playback frames: {0}\n", m_playback_accumulated_frames);
                osd_printf_info("Average recorded speed: {1}%%\n", (m_playback_accumulated_speed * 200 + 1) >> 21);

                // close the program at the end of inp file playback
                if (machine().options().exit_after_playback())
                {
                    osd_printf_info("Exiting MAME now...\n");
                    machine().schedule_exit();
                }
            }
        }

        //-------------------------------------------------
        //  playback_frame - start of frame callback for
        //  playback
        //-------------------------------------------------
        void playback_frame(attotime curtime)
        {
            // if playing back, fetch the information and verify
            if (m_playback_file.is_open())
            {
                throw new emu_unimplemented();
            }
        }

        //-------------------------------------------------
        //  playback_port - per-port callback for playback
        //-------------------------------------------------
        void playback_port(ioport_port port)
        {
            // if playing back, fetch information about this port
            if (m_playback_file.is_open())
            {
                throw new emu_unimplemented();
            }
        }


        //-------------------------------------------------
        //  record_write - write a value to the record file
        //-------------------------------------------------
        //template<typename _Type>
        void record_write(PointerU8 buffer)  //void record_write(_Type value)
        {
            // protect against nullptr handles if previous reads fail
            if (!m_record_file.is_open())
                return;

            // read the value; if we fail, end playback
            if (m_record_file.write(buffer, (UInt32)buffer.Count) != buffer.Count)  //if (m_record_file.write(&value, sizeof(value)) != sizeof(value))
                record_end("Out of space");
        }


        //-------------------------------------------------
        //  record_init - initialize INP recording
        //-------------------------------------------------
        void record_init()
        {
            // if no file, nothing to do
            string filename = machine().options().record();
            if (string.IsNullOrEmpty(filename))
                return;

            // open the record file
            osd_file.error filerr = m_record_file.open(filename);
            if (filerr != osd_file.error.NONE)
                throw new emu_fatalerror("ioport_manager::record_init: Failed to open file for recording");

            // get the base time
            system_time systime;
            machine().base_datetime(out systime);

            // fill in the header
            inp_header header = new inp_header();
            header.set_magic();
            header.set_basetime((UInt64)systime.time);
            header.set_version();
            header.set_sysname(machine().system().name);
            header.set_appdesc(string.Format("{0} {1}", emulator_info.get_appname(), emulator_info.get_build_version()));

            // write it
            header.write(m_record_file);

            // enable compression
            m_record_file.compress(util_.FCOMPRESS_MEDIUM);
        }


        //-------------------------------------------------
        //  record_end - end INP recording
        //-------------------------------------------------
        void record_end(string message = null)
        {
            // only applies if we have a live file
            if (m_record_file.is_open())
            {
                // close the file
                m_record_file.close();

                // pop a message
                if (message != null)
                    machine().popmessage(string.Format("Recording Ended\nReason: {0}", message));
            }
        }


        //-------------------------------------------------
        //  record_frame - start of frame callback for
        //  recording
        //-------------------------------------------------
        void record_frame(attotime curtime)
        {
            // if recording, record information about the current frame
            if (m_record_file.is_open())
            {
                // first the absolute time
                //record_write(curtime.seconds());
                PointerU8 secondsBuf = new PointerU8(new MemoryU8(4, true));
                secondsBuf.SetUInt32(0, (UInt32)curtime.seconds());
                record_write(secondsBuf);
                //record_write(curtime.attoseconds());
                PointerU8 attosecondsBuf = new PointerU8(new MemoryU8(8, true));
                attosecondsBuf.SetUInt64(0, (UInt64)curtime.attoseconds());
                record_write(attosecondsBuf);

                // then the current speed
                //record_write(u32(machine().video().speed_percent() * double(1 << 20)));
                PointerU8 speedBuf = new PointerU8(new MemoryU8(4, true));
                speedBuf.SetUInt32(0, (UInt32)(machine().video().speed_percent() * (double)(1 << 20)));
                record_write(speedBuf);
            }

            if (m_timecode_file.is_open() && machine().video().get_timecode_write())
            {
                // Display the timecode
                m_timecode_count++;
                string current_time_str = string.Format("{0}:{1}:{2}.{3}",  //"%02d:%02d:%02d.%03d",
                        (int)curtime.seconds() / (60 * 60),
                        (curtime.seconds() / 60) % 60,
                        curtime.seconds() % 60,
                        (int)(curtime.attoseconds()/attotime.ATTOSECONDS_PER_MILLISECOND));

                // Elapsed from previous timecode
                attotime elapsed_time = curtime - m_timecode_last_time;
                m_timecode_last_time = curtime;
                string elapsed_time_str = string.Format("{0}:{1}:{2}.{3}",  //"%02d:%02d:%02d.%03d",
                        elapsed_time.seconds() / (60 * 60),
                        (elapsed_time.seconds() / 60) % 60,
                        elapsed_time.seconds() % 60,
                        (int)(elapsed_time.attoseconds()/attotime.ATTOSECONDS_PER_MILLISECOND));

                // Number of ms from beginning of playback
                int mseconds_start = (int)(curtime.seconds()*1000 + curtime.attoseconds()/attotime.ATTOSECONDS_PER_MILLISECOND);
                string mseconds_start_str = string.Format("{0}", mseconds_start);  // %015d

                // Number of ms from previous timecode
                int mseconds_elapsed = (int)(elapsed_time.seconds()*1000 + elapsed_time.attoseconds()/attotime.ATTOSECONDS_PER_MILLISECOND);
                string mseconds_elapsed_str = string.Format("{0}", mseconds_elapsed);  // "%015d"

                // Number of frames from beginning of playback
                int frame_start = mseconds_start * 60 / 1000;
                string frame_start_str = string.Format("{0}", frame_start);  // "%015d"

                // Number of frames from previous timecode
                int frame_elapsed = mseconds_elapsed * 60 / 1000;
                string frame_elapsed_str = string.Format("{0}", frame_elapsed);  // "%015d"

                string message;
                string timecode_text = "";
                string timecode_key;
                bool show_timecode_counter = false;
                if (m_timecode_count==1)
                {
                    message = string.Format("TIMECODE: Intro started at {0}", current_time_str);
                    timecode_key = "INTRO_START";
                    timecode_text = "INTRO";
                    show_timecode_counter = true;
                }
                else if (m_timecode_count==2)
                {
                    machine().video().add_to_total_time(elapsed_time);
                    message = string.Format("TIMECODE: Intro duration {0}", elapsed_time_str);
                    timecode_key = "INTRO_STOP";
                    //timecode_text = "INTRO";
                }
                else if (m_timecode_count==3)
                {
                    message = string.Format("TIMECODE: Gameplay started at {0}", current_time_str);
                    timecode_key = "GAMEPLAY_START";
                    timecode_text = "GAMEPLAY";
                    show_timecode_counter = true;
                }
                else if (m_timecode_count==4) {
                    machine().video().add_to_total_time(elapsed_time);
                    message = string.Format("TIMECODE: Gameplay duration {0}", elapsed_time_str);
                    timecode_key = "GAMEPLAY_STOP";
                    //timecode_text = "GAMEPLAY";
                }
                else if (m_timecode_count % 2 == 1)
                {
                    message = string.Format("TIMECODE: Extra {0} started at {1}", (m_timecode_count-3)/2, current_time_str);
                    timecode_key = string.Format("EXTRA_START_{0}", (m_timecode_count-3)/2);  // %03d
                    timecode_text = string.Format("EXTRA {0}", (m_timecode_count-3)/2);
                    show_timecode_counter = true;
                }
                else
                {
                    machine().video().add_to_total_time(elapsed_time);
                    message = string.Format("TIMECODE: Extra {0} duration {1}", (m_timecode_count-4)/2, elapsed_time_str);
                    timecode_key = string.Format("EXTRA_STOP_{0}", (m_timecode_count-4)/2);  //%03d
                }

                osd_printf_info("{0} \n", message);
                machine().popmessage("{0} \n", message);

                m_timecode_file.printf(
                        "{0} {1} {2} {3} {4} {5} {6}\n",  //"%-19s %s %s %s %s %s %s\n",
                        timecode_key,
                        current_time_str, elapsed_time_str,
                        mseconds_start_str, mseconds_elapsed_str,
                        frame_start_str, frame_elapsed_str);

                machine().video().set_timecode_write(false);
                machine().video().set_timecode_text(timecode_text);
                machine().video().set_timecode_start(m_timecode_last_time);
                machine().ui().set_show_timecode_counter(show_timecode_counter);
            }
        }


        //-------------------------------------------------
        //  record_port - per-port callback for record
        //-------------------------------------------------
        void record_port(ioport_port port)
        {
            // if recording, store information about this port
            if (m_record_file.is_open())
            {
                throw new emu_unimplemented();
            }
        }


        //template<typename _Type> void timecode_write(_Type value);


        void timecode_init()
        {
            // check if option -record_timecode is enabled
            if (!machine().options().record_timecode())
            {
                machine().video().set_timecode_enabled(false);
                return;
            }

            // if no file, nothing to do
            string record_filename = machine().options().record();
            if (string.IsNullOrEmpty(record_filename))  //if (record_filename[0] == 0)
            {
                machine().video().set_timecode_enabled(false);
                return;
            }

            machine().video().set_timecode_enabled(true);

            // open the record file
            string filename;
            filename = record_filename + ".timecode";
            osd_printf_info("Record input timecode file: {0}\n", record_filename);

            osd_file.error filerr = m_timecode_file.open(filename);
            if (filerr != osd_file.error.NONE)
                throw new emu_fatalerror("ioport_manager::timecode_init: Failed to open file for input timecode recording");

            m_timecode_file.puts("# ==========================================\n");
            m_timecode_file.puts("# TIMECODE FILE FOR VIDEO PREVIEW GENERATION\n");
            m_timecode_file.puts("# ==========================================\n");
            m_timecode_file.puts("#\n");
            m_timecode_file.puts("# VIDEO_PART:     code of video timecode\n");
            m_timecode_file.puts("# START:          start time (hh:mm:ss.mmm)\n");
            m_timecode_file.puts("# ELAPSED:        elapsed time (hh:mm:ss.mmm)\n");
            m_timecode_file.puts("# MSEC_START:     start time (milliseconds)\n");
            m_timecode_file.puts("# MSEC_ELAPSED:   elapsed time (milliseconds)\n");
            m_timecode_file.puts("# FRAME_START:    start time (frames)\n");
            m_timecode_file.puts("# FRAME_ELAPSED:  elapsed time (frames)\n");
            m_timecode_file.puts("#\n");
            m_timecode_file.puts("# VIDEO_PART======= START======= ELAPSED===== MSEC_START===== MSEC_ELAPSED=== FRAME_START==== FRAME_ELAPSED==\n");
        }


        void timecode_end(string message = null)
        {
            // only applies if we have a live file
            if (m_timecode_file.is_open())
            {
                // close the file
                m_timecode_file.close();

                // pop a message
                if (message != null)
                    machine().popmessage("Recording Timecode Ended\nReason: {0}", message);
            }
        }
    }
}
