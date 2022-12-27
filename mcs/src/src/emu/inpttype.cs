// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.input_global;
using static mame.language_global;


namespace mame
{
    public static class inpttype_global
    {
        const ioport_group UI = ioport_group.IPG_UI;
        const ioport_group PLAYER1 = ioport_group.IPG_PLAYER1;
        const ioport_group PLAYER2 = ioport_group.IPG_PLAYER2;
        const ioport_group PLAYER3 = ioport_group.IPG_PLAYER3;
        const ioport_group PLAYER4 = ioport_group.IPG_PLAYER4;
        const ioport_group PLAYER5 = ioport_group.IPG_PLAYER5;
        const ioport_group PLAYER6 = ioport_group.IPG_PLAYER6;
        const ioport_group PLAYER7 = ioport_group.IPG_PLAYER7;
        const ioport_group PLAYER8 = ioport_group.IPG_PLAYER8;
        const ioport_group PLAYER9 = ioport_group.IPG_PLAYER9;
        const ioport_group PLAYER10 = ioport_group.IPG_PLAYER10;
        //const ioport_group TOTAL_GROUPS = ioport_group.IPG_TOTAL_GROUPS;
        const ioport_group INVALID = ioport_group.IPG_INVALID;


        const ioport_type UNUSED = ioport_type.IPT_UNUSED;
        const ioport_type UNKNOWN = ioport_type.IPT_UNKNOWN;
        const ioport_type DIPSWITCH = ioport_type.IPT_DIPSWITCH;
        const ioport_type CONFIG = ioport_type.IPT_CONFIG;
        const ioport_type START1 = ioport_type.IPT_START1;
        const ioport_type START2 = ioport_type.IPT_START2;
        const ioport_type START3 = ioport_type.IPT_START3;
        const ioport_type START4 = ioport_type.IPT_START4;
        const ioport_type START5 = ioport_type.IPT_START5;
        const ioport_type START6 = ioport_type.IPT_START6;
        const ioport_type START7 = ioport_type.IPT_START7;
        const ioport_type START8 = ioport_type.IPT_START8;
        //const ioport_type START9 = ioport_type.IPT_START9;
        //const ioport_type START10 = ioport_type.IPT_START10;

        const ioport_type COIN1 = ioport_type.IPT_COIN1;
        const ioport_type COIN2 = ioport_type.IPT_COIN2;
        const ioport_type COIN3 = ioport_type.IPT_COIN3;
        const ioport_type COIN4 = ioport_type.IPT_COIN4;
        const ioport_type COIN5 = ioport_type.IPT_COIN5;
        const ioport_type COIN6 = ioport_type.IPT_COIN6;
        const ioport_type COIN7 = ioport_type.IPT_COIN7;
        const ioport_type COIN8 = ioport_type.IPT_COIN8;
        const ioport_type COIN9 = ioport_type.IPT_COIN9;
        const ioport_type COIN10 = ioport_type.IPT_COIN10;
        const ioport_type COIN11 = ioport_type.IPT_COIN11;
        const ioport_type COIN12 = ioport_type.IPT_COIN12;
        const ioport_type BILL1 = ioport_type.IPT_BILL1;

        const ioport_type SERVICE1 = ioport_type.IPT_SERVICE1;
        const ioport_type SERVICE2 = ioport_type.IPT_SERVICE2;
        const ioport_type SERVICE3 = ioport_type.IPT_SERVICE3;
        const ioport_type SERVICE4 = ioport_type.IPT_SERVICE4;

        const ioport_type TILT1 = ioport_type.IPT_TILT1;
        const ioport_type TILT2 = ioport_type.IPT_TILT2;
        const ioport_type TILT3 = ioport_type.IPT_TILT3;
        const ioport_type TILT4 = ioport_type.IPT_TILT4;

        const ioport_type POWER_ON = ioport_type.IPT_POWER_ON;
        const ioport_type POWER_OFF = ioport_type.IPT_POWER_OFF;
        const ioport_type SERVICE = ioport_type.IPT_SERVICE;
        const ioport_type TILT = ioport_type.IPT_TILT;
        const ioport_type INTERLOCK = ioport_type.IPT_INTERLOCK;
        const ioport_type MEMORY_RESET = ioport_type.IPT_MEMORY_RESET;
        const ioport_type VOLUME_UP = ioport_type.IPT_VOLUME_UP;
        const ioport_type VOLUME_DOWN = ioport_type.IPT_VOLUME_DOWN;
        const ioport_type START = ioport_type.IPT_START;
        const ioport_type SELECT = ioport_type.IPT_SELECT;
        const ioport_type KEYPAD = ioport_type.IPT_KEYPAD;
        const ioport_type KEYBOARD = ioport_type.IPT_KEYBOARD;

        const ioport_type JOYSTICK_UP = ioport_type.IPT_JOYSTICK_UP;
        const ioport_type JOYSTICK_DOWN = ioport_type.IPT_JOYSTICK_DOWN;
        const ioport_type JOYSTICK_LEFT = ioport_type.IPT_JOYSTICK_LEFT;
        const ioport_type JOYSTICK_RIGHT = ioport_type.IPT_JOYSTICK_RIGHT;
        const ioport_type JOYSTICKRIGHT_UP = ioport_type.IPT_JOYSTICKRIGHT_UP;
        const ioport_type JOYSTICKRIGHT_DOWN = ioport_type.IPT_JOYSTICKRIGHT_DOWN;
        const ioport_type JOYSTICKRIGHT_LEFT = ioport_type.IPT_JOYSTICKRIGHT_LEFT;
        const ioport_type JOYSTICKRIGHT_RIGHT = ioport_type.IPT_JOYSTICKRIGHT_RIGHT;
        const ioport_type JOYSTICKLEFT_UP = ioport_type.IPT_JOYSTICKLEFT_UP;
        const ioport_type JOYSTICKLEFT_DOWN = ioport_type.IPT_JOYSTICKLEFT_DOWN;
        const ioport_type JOYSTICKLEFT_LEFT = ioport_type.IPT_JOYSTICKLEFT_LEFT;
        const ioport_type JOYSTICKLEFT_RIGHT = ioport_type.IPT_JOYSTICKLEFT_RIGHT;

        const ioport_type BUTTON1 = ioport_type.IPT_BUTTON1;
        const ioport_type BUTTON2 = ioport_type.IPT_BUTTON2;
        const ioport_type BUTTON3 = ioport_type.IPT_BUTTON3;
        const ioport_type BUTTON4 = ioport_type.IPT_BUTTON4;
        const ioport_type BUTTON5 = ioport_type.IPT_BUTTON5;
        const ioport_type BUTTON6 = ioport_type.IPT_BUTTON6;
        const ioport_type BUTTON7 = ioport_type.IPT_BUTTON7;
        const ioport_type BUTTON8 = ioport_type.IPT_BUTTON8;
        const ioport_type BUTTON9 = ioport_type.IPT_BUTTON9;
        const ioport_type BUTTON10 = ioport_type.IPT_BUTTON10;
        const ioport_type BUTTON11 = ioport_type.IPT_BUTTON11;
        const ioport_type BUTTON12 = ioport_type.IPT_BUTTON12;
        const ioport_type BUTTON13 = ioport_type.IPT_BUTTON13;
        const ioport_type BUTTON14 = ioport_type.IPT_BUTTON14;
        const ioport_type BUTTON15 = ioport_type.IPT_BUTTON15;
        const ioport_type BUTTON16 = ioport_type.IPT_BUTTON16;

        const ioport_type MAHJONG_A = ioport_type.IPT_MAHJONG_A;
        const ioport_type MAHJONG_B = ioport_type.IPT_MAHJONG_B;
        const ioport_type MAHJONG_C = ioport_type.IPT_MAHJONG_C;
        const ioport_type MAHJONG_D = ioport_type.IPT_MAHJONG_D;
        const ioport_type MAHJONG_E = ioport_type.IPT_MAHJONG_E;
        const ioport_type MAHJONG_F = ioport_type.IPT_MAHJONG_F;
        const ioport_type MAHJONG_G = ioport_type.IPT_MAHJONG_G;
        const ioport_type MAHJONG_H = ioport_type.IPT_MAHJONG_H;
        const ioport_type MAHJONG_I = ioport_type.IPT_MAHJONG_I;
        const ioport_type MAHJONG_J = ioport_type.IPT_MAHJONG_J;
        const ioport_type MAHJONG_K = ioport_type.IPT_MAHJONG_K;
        const ioport_type MAHJONG_L = ioport_type.IPT_MAHJONG_L;
        const ioport_type MAHJONG_M = ioport_type.IPT_MAHJONG_M;
        const ioport_type MAHJONG_N = ioport_type.IPT_MAHJONG_N;
        const ioport_type MAHJONG_O = ioport_type.IPT_MAHJONG_O;
        const ioport_type MAHJONG_P = ioport_type.IPT_MAHJONG_P;
        const ioport_type MAHJONG_Q = ioport_type.IPT_MAHJONG_Q;
        const ioport_type MAHJONG_KAN = ioport_type.IPT_MAHJONG_KAN;
        const ioport_type MAHJONG_PON = ioport_type.IPT_MAHJONG_PON;
        const ioport_type MAHJONG_CHI = ioport_type.IPT_MAHJONG_CHI;
        const ioport_type MAHJONG_REACH = ioport_type.IPT_MAHJONG_REACH;
        const ioport_type MAHJONG_RON = ioport_type.IPT_MAHJONG_RON;
        const ioport_type MAHJONG_BET = ioport_type.IPT_MAHJONG_BET;
        const ioport_type MAHJONG_LAST_CHANCE = ioport_type.IPT_MAHJONG_LAST_CHANCE;
        const ioport_type MAHJONG_SCORE = ioport_type.IPT_MAHJONG_SCORE;
        const ioport_type MAHJONG_DOUBLE_UP = ioport_type.IPT_MAHJONG_DOUBLE_UP;
        const ioport_type MAHJONG_FLIP_FLOP = ioport_type.IPT_MAHJONG_FLIP_FLOP;
        const ioport_type MAHJONG_BIG = ioport_type.IPT_MAHJONG_BIG;
        const ioport_type MAHJONG_SMALL = ioport_type.IPT_MAHJONG_SMALL;
        const ioport_type HANAFUDA_A = ioport_type.IPT_HANAFUDA_A;
        const ioport_type HANAFUDA_B = ioport_type.IPT_HANAFUDA_B;
        const ioport_type HANAFUDA_C = ioport_type.IPT_HANAFUDA_C;
        const ioport_type HANAFUDA_D = ioport_type.IPT_HANAFUDA_D;
        const ioport_type HANAFUDA_E = ioport_type.IPT_HANAFUDA_E;
        const ioport_type HANAFUDA_F = ioport_type.IPT_HANAFUDA_F;
        const ioport_type HANAFUDA_G = ioport_type.IPT_HANAFUDA_G;
        const ioport_type HANAFUDA_H = ioport_type.IPT_HANAFUDA_H;
        const ioport_type HANAFUDA_YES = ioport_type.IPT_HANAFUDA_YES;
        const ioport_type HANAFUDA_NO = ioport_type.IPT_HANAFUDA_NO;
        const ioport_type GAMBLE_KEYIN = ioport_type.IPT_GAMBLE_KEYIN;
        const ioport_type GAMBLE_KEYOUT = ioport_type.IPT_GAMBLE_KEYOUT;
        const ioport_type GAMBLE_SERVICE = ioport_type.IPT_GAMBLE_SERVICE;
        const ioport_type GAMBLE_BOOK = ioport_type.IPT_GAMBLE_BOOK;
        const ioport_type GAMBLE_DOOR = ioport_type.IPT_GAMBLE_DOOR;
        const ioport_type GAMBLE_HIGH = ioport_type.IPT_GAMBLE_HIGH;
        const ioport_type GAMBLE_LOW = ioport_type.IPT_GAMBLE_LOW;
        const ioport_type GAMBLE_HALF = ioport_type.IPT_GAMBLE_HALF;
        const ioport_type GAMBLE_DEAL = ioport_type.IPT_GAMBLE_DEAL;
        const ioport_type GAMBLE_D_UP = ioport_type.IPT_GAMBLE_D_UP;
        const ioport_type GAMBLE_TAKE = ioport_type.IPT_GAMBLE_TAKE;
        const ioport_type GAMBLE_STAND = ioport_type.IPT_GAMBLE_STAND;
        const ioport_type GAMBLE_BET = ioport_type.IPT_GAMBLE_BET;
        const ioport_type GAMBLE_PAYOUT = ioport_type.IPT_GAMBLE_PAYOUT;
        const ioport_type POKER_HOLD1 = ioport_type.IPT_POKER_HOLD1;
        const ioport_type POKER_HOLD2 = ioport_type.IPT_POKER_HOLD2;
        const ioport_type POKER_HOLD3 = ioport_type.IPT_POKER_HOLD3;
        const ioport_type POKER_HOLD4 = ioport_type.IPT_POKER_HOLD4;
        const ioport_type POKER_HOLD5 = ioport_type.IPT_POKER_HOLD5;
        const ioport_type POKER_CANCEL = ioport_type.IPT_POKER_CANCEL;
        const ioport_type SLOT_STOP1 = ioport_type.IPT_SLOT_STOP1;
        const ioport_type SLOT_STOP2 = ioport_type.IPT_SLOT_STOP2;
        const ioport_type SLOT_STOP3 = ioport_type.IPT_SLOT_STOP3;
        const ioport_type SLOT_STOP4 = ioport_type.IPT_SLOT_STOP4;
        const ioport_type SLOT_STOP_ALL = ioport_type.IPT_SLOT_STOP_ALL;
        const ioport_type AD_STICK_X = ioport_type.IPT_AD_STICK_X;
        const ioport_type AD_STICK_Y = ioport_type.IPT_AD_STICK_Y;
        const ioport_type AD_STICK_Z = ioport_type.IPT_AD_STICK_Z;
        const ioport_type PADDLE = ioport_type.IPT_PADDLE;
        const ioport_type PADDLE_V = ioport_type.IPT_PADDLE_V;
        const ioport_type PEDAL = ioport_type.IPT_PEDAL;
        const ioport_type PEDAL2 = ioport_type.IPT_PEDAL2;
        const ioport_type PEDAL3 = ioport_type.IPT_PEDAL3;
        const ioport_type LIGHTGUN_X = ioport_type.IPT_LIGHTGUN_X;
        const ioport_type LIGHTGUN_Y = ioport_type.IPT_LIGHTGUN_Y;
        const ioport_type POSITIONAL = ioport_type.IPT_POSITIONAL;
        const ioport_type POSITIONAL_V = ioport_type.IPT_POSITIONAL_V;
        const ioport_type DIAL = ioport_type.IPT_DIAL;
        const ioport_type DIAL_V = ioport_type.IPT_DIAL_V;
        const ioport_type TRACKBALL_X = ioport_type.IPT_TRACKBALL_X;
        const ioport_type TRACKBALL_Y = ioport_type.IPT_TRACKBALL_Y;
        const ioport_type MOUSE_X = ioport_type.IPT_MOUSE_X;
        const ioport_type MOUSE_Y = ioport_type.IPT_MOUSE_Y;
        const ioport_type ADJUSTER = ioport_type.IPT_ADJUSTER;
        const ioport_type UI_CONFIGURE = ioport_type.IPT_UI_CONFIGURE;
        const ioport_type UI_ON_SCREEN_DISPLAY = ioport_type.IPT_UI_ON_SCREEN_DISPLAY;
        const ioport_type UI_DEBUG_BREAK = ioport_type.IPT_UI_DEBUG_BREAK;
        const ioport_type UI_PAUSE = ioport_type.IPT_UI_PAUSE;
        const ioport_type UI_PAUSE_SINGLE = ioport_type.IPT_UI_PAUSE_SINGLE;
        const ioport_type UI_REWIND_SINGLE = ioport_type.IPT_UI_REWIND_SINGLE;
        const ioport_type UI_RESET_MACHINE = ioport_type.IPT_UI_RESET_MACHINE;
        const ioport_type UI_SOFT_RESET = ioport_type.IPT_UI_SOFT_RESET;
        const ioport_type UI_SHOW_GFX = ioport_type.IPT_UI_SHOW_GFX;
        const ioport_type UI_FRAMESKIP_DEC = ioport_type.IPT_UI_FRAMESKIP_DEC;
        const ioport_type UI_FRAMESKIP_INC = ioport_type.IPT_UI_FRAMESKIP_INC;
        const ioport_type UI_THROTTLE = ioport_type.IPT_UI_THROTTLE;
        const ioport_type UI_FAST_FORWARD = ioport_type.IPT_UI_FAST_FORWARD;
        const ioport_type UI_SHOW_FPS = ioport_type.IPT_UI_SHOW_FPS;
        const ioport_type UI_SNAPSHOT = ioport_type.IPT_UI_SNAPSHOT;
        const ioport_type UI_RECORD_MNG = ioport_type.IPT_UI_RECORD_MNG;
        const ioport_type UI_RECORD_AVI = ioport_type.IPT_UI_RECORD_AVI;
        const ioport_type UI_TOGGLE_CHEAT = ioport_type.IPT_UI_TOGGLE_CHEAT;
        const ioport_type UI_UP = ioport_type.IPT_UI_UP;
        const ioport_type UI_DOWN = ioport_type.IPT_UI_DOWN;
        const ioport_type UI_LEFT = ioport_type.IPT_UI_LEFT;
        const ioport_type UI_RIGHT = ioport_type.IPT_UI_RIGHT;
        const ioport_type UI_HOME = ioport_type.IPT_UI_HOME;
        const ioport_type UI_END = ioport_type.IPT_UI_END;
        const ioport_type UI_PAGE_UP = ioport_type.IPT_UI_PAGE_UP;
        const ioport_type UI_PAGE_DOWN = ioport_type.IPT_UI_PAGE_DOWN;
        const ioport_type UI_FOCUS_NEXT = ioport_type.IPT_UI_FOCUS_NEXT;
        const ioport_type UI_FOCUS_PREV = ioport_type.IPT_UI_FOCUS_PREV;
        const ioport_type UI_SELECT = ioport_type.IPT_UI_SELECT;
        const ioport_type UI_CANCEL = ioport_type.IPT_UI_CANCEL;
        const ioport_type UI_DISPLAY_COMMENT = ioport_type.IPT_UI_DISPLAY_COMMENT;
        const ioport_type UI_CLEAR = ioport_type.IPT_UI_CLEAR;
        const ioport_type UI_ZOOM_IN = ioport_type.IPT_UI_ZOOM_IN;
        const ioport_type UI_ZOOM_OUT = ioport_type.IPT_UI_ZOOM_OUT;
        const ioport_type UI_ZOOM_DEFAULT = ioport_type.IPT_UI_ZOOM_DEFAULT;
        const ioport_type UI_PREV_GROUP = ioport_type.IPT_UI_PREV_GROUP;
        const ioport_type UI_NEXT_GROUP = ioport_type.IPT_UI_NEXT_GROUP;
        const ioport_type UI_ROTATE = ioport_type.IPT_UI_ROTATE;
        const ioport_type UI_SHOW_PROFILER = ioport_type.IPT_UI_SHOW_PROFILER;
        const ioport_type UI_TOGGLE_UI = ioport_type.IPT_UI_TOGGLE_UI;
        const ioport_type UI_RELEASE_POINTER = ioport_type.IPT_UI_RELEASE_POINTER;
        const ioport_type UI_PASTE = ioport_type.IPT_UI_PASTE;
        const ioport_type UI_SAVE_STATE = ioport_type.IPT_UI_SAVE_STATE;
        const ioport_type UI_LOAD_STATE = ioport_type.IPT_UI_LOAD_STATE;
        const ioport_type UI_TAPE_START = ioport_type.IPT_UI_TAPE_START;
        const ioport_type UI_TAPE_STOP = ioport_type.IPT_UI_TAPE_STOP;
        const ioport_type UI_DATS = ioport_type.IPT_UI_DATS;
        const ioport_type UI_FAVORITES = ioport_type.IPT_UI_FAVORITES;
        const ioport_type UI_EXPORT = ioport_type.IPT_UI_EXPORT;
        const ioport_type UI_AUDIT = ioport_type.IPT_UI_AUDIT;
        const ioport_type OSD_1 = ioport_type.IPT_OSD_1;
        const ioport_type OSD_2 = ioport_type.IPT_OSD_2;
        const ioport_type OSD_3 = ioport_type.IPT_OSD_3;
        const ioport_type OSD_4 = ioport_type.IPT_OSD_4;
        const ioport_type OSD_5 = ioport_type.IPT_OSD_5;
        const ioport_type OSD_6 = ioport_type.IPT_OSD_6;
        const ioport_type OSD_7 = ioport_type.IPT_OSD_7;
        const ioport_type OSD_8 = ioport_type.IPT_OSD_8;
        const ioport_type OSD_9 = ioport_type.IPT_OSD_9;
        const ioport_type OSD_10 = ioport_type.IPT_OSD_10;
        const ioport_type OSD_11 = ioport_type.IPT_OSD_11;
        const ioport_type OSD_12 = ioport_type.IPT_OSD_12;
        const ioport_type OSD_13 = ioport_type.IPT_OSD_13;
        const ioport_type OSD_14 = ioport_type.IPT_OSD_14;
        const ioport_type OSD_15 = ioport_type.IPT_OSD_15;
        const ioport_type OSD_16 = ioport_type.IPT_OSD_16;
        const ioport_type OTHER = ioport_type.IPT_OTHER;
        const ioport_type SPECIAL = ioport_type.IPT_SPECIAL;
        //const ioport_type CUSTOM = ioport_type.IPT_CUSTOM;


        /***************************************************************************
            BUILT-IN CORE MAPPINGS
        ***************************************************************************/

        static void emplace_core_types_p1(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p1)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICK_UP,          N_p("input-name", "%p Up"),                  new input_seq(KEYCODE_UP, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICK_DOWN,        N_p("input-name", "%p Down"),                new input_seq(KEYCODE_DOWN, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICK_LEFT,        N_p("input-name", "%p Left"),                new input_seq(KEYCODE_LEFT, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICK_RIGHT,       N_p("input-name", "%p Right"),               new input_seq(KEYCODE_RIGHT, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKRIGHT_UP,     N_p("input-name", "%p Right Stick/Up"),      new input_seq(KEYCODE_I, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKRIGHT_DOWN,   N_p("input-name", "%p Right Stick/Down"),    new input_seq(KEYCODE_K, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKRIGHT_LEFT,   N_p("input-name", "%p Right Stick/Left"),    new input_seq(KEYCODE_J, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKRIGHT_RIGHT,  N_p("input-name", "%p Right Stick/Right"),   new input_seq(KEYCODE_L, input_seq.or_code, JOYCODE_BUTTON4_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKLEFT_UP,      N_p("input-name", "%p Left Stick/Up"),       new input_seq(KEYCODE_E, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKLEFT_DOWN,    N_p("input-name", "%p Left Stick/Down"),     new input_seq(KEYCODE_D, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKLEFT_LEFT,    N_p("input-name", "%p Left Stick/Left"),     new input_seq(KEYCODE_S, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKLEFT_RIGHT,   N_p("input-name", "%p Left Stick/Right"),    new input_seq(KEYCODE_F, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON1,              N_p("input-name", "%p Button 1"),            new input_seq(KEYCODE_LCONTROL, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(0), input_seq.or_code, MOUSECODE_BUTTON1_INDEXED(0), input_seq.or_code, GUNCODE_BUTTON1_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON2,              N_p("input-name", "%p Button 2"),            new input_seq(KEYCODE_LALT, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(0), input_seq.or_code, MOUSECODE_BUTTON3_INDEXED(0), input_seq.or_code, GUNCODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON3,              N_p("input-name", "%p Button 3"),            new input_seq(KEYCODE_SPACE, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(0), input_seq.or_code, MOUSECODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON4,              N_p("input-name", "%p Button 4"),            new input_seq(KEYCODE_LSHIFT, input_seq.or_code, JOYCODE_BUTTON4_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON5,              N_p("input-name", "%p Button 5"),            new input_seq(KEYCODE_Z, input_seq.or_code, JOYCODE_BUTTON5_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON6,              N_p("input-name", "%p Button 6"),            new input_seq(KEYCODE_X, input_seq.or_code, JOYCODE_BUTTON6_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON7,              N_p("input-name", "%p Button 7"),            new input_seq(KEYCODE_C, input_seq.or_code, JOYCODE_BUTTON7_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON8,              N_p("input-name", "%p Button 8"),            new input_seq(KEYCODE_V, input_seq.or_code, JOYCODE_BUTTON8_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON9,              N_p("input-name", "%p Button 9"),            new input_seq(KEYCODE_B, input_seq.or_code, JOYCODE_BUTTON9_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON10,             N_p("input-name", "%p Button 10"),           new input_seq(KEYCODE_N, input_seq.or_code, JOYCODE_BUTTON10_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON11,             N_p("input-name", "%p Button 11"),           new input_seq(KEYCODE_M, input_seq.or_code, JOYCODE_BUTTON11_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON12,             N_p("input-name", "%p Button 12"),           new input_seq(KEYCODE_COMMA, input_seq.or_code, JOYCODE_BUTTON12_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON13,             N_p("input-name", "%p Button 13"),           new input_seq(KEYCODE_STOP, input_seq.or_code, JOYCODE_BUTTON13_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON14,             N_p("input-name", "%p Button 14"),           new input_seq(KEYCODE_SLASH, input_seq.or_code, JOYCODE_BUTTON14_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON15,             N_p("input-name", "%p Button 15"),           new input_seq(KEYCODE_RSHIFT, input_seq.or_code, JOYCODE_BUTTON15_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON16,             N_p("input-name", "%p Button 16"),           new input_seq(JOYCODE_BUTTON16_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, START,                N_p("input-name", "%p Start"),               new input_seq(KEYCODE_1, input_seq.or_code, JOYCODE_START_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SELECT,               N_p("input-name", "%p Select"),              new input_seq(KEYCODE_5, input_seq.or_code, JOYCODE_SELECT_INDEXED(0)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p1_mahjong(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p1_mahjong)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_A,            N_p("input-name", "%p Mahjong A"),           new input_seq(KEYCODE_A) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_B,            N_p("input-name", "%p Mahjong B"),           new input_seq(KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_C,            N_p("input-name", "%p Mahjong C"),           new input_seq(KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_D,            N_p("input-name", "%p Mahjong D"),           new input_seq(KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_E,            N_p("input-name", "%p Mahjong E"),           new input_seq(KEYCODE_E) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_F,            N_p("input-name", "%p Mahjong F"),           new input_seq(KEYCODE_F) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_G,            N_p("input-name", "%p Mahjong G"),           new input_seq(KEYCODE_G) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_H,            N_p("input-name", "%p Mahjong H"),           new input_seq(KEYCODE_H) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_I,            N_p("input-name", "%p Mahjong I"),           new input_seq(KEYCODE_I) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_J,            N_p("input-name", "%p Mahjong J"),           new input_seq(KEYCODE_J) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_K,            N_p("input-name", "%p Mahjong K"),           new input_seq(KEYCODE_K) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_L,            N_p("input-name", "%p Mahjong L"),           new input_seq(KEYCODE_L) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_M,            N_p("input-name", "%p Mahjong M"),           new input_seq(KEYCODE_M) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_N,            N_p("input-name", "%p Mahjong N"),           new input_seq(KEYCODE_N) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_O,            N_p("input-name", "%p Mahjong O"),           new input_seq(KEYCODE_O) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_P,            N_p("input-name", "%p Mahjong P"),           new input_seq(KEYCODE_COLON) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_Q,            N_p("input-name", "%p Mahjong Q"),           new input_seq(KEYCODE_Q) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_KAN,          N_p("input-name", "%p Mahjong Kan"),         new input_seq(KEYCODE_LCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_PON,          N_p("input-name", "%p Mahjong Pon"),         new input_seq(KEYCODE_LALT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_CHI,          N_p("input-name", "%p Mahjong Chi"),         new input_seq(KEYCODE_SPACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_REACH,        N_p("input-name", "%p Mahjong Reach"),       new input_seq(KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_RON,          N_p("input-name", "%p Mahjong Ron"),         new input_seq(KEYCODE_Z) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_FLIP_FLOP,    N_p("input-name", "%p Mahjong Flip Flop"),   new input_seq(KEYCODE_Y) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_BET,          N_p("input-name", "%p Mahjong Bet"),         new input_seq(KEYCODE_3) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_SCORE,        N_p("input-name", "%p Mahjong Take Score"),  new input_seq(KEYCODE_RCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_DOUBLE_UP,    N_p("input-name", "%p Mahjong Double Up"),   new input_seq(KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_BIG,          N_p("input-name", "%p Mahjong Big"),         new input_seq(KEYCODE_ENTER) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_SMALL,        N_p("input-name", "%p Mahjong Small"),       new input_seq(KEYCODE_BACKSPACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_LAST_CHANCE,  N_p("input-name", "%p Mahjong Last Chance"), new input_seq(KEYCODE_RALT) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p1_hanafuda(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p1_hanafuda)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_A,           N_p("input-name", "%p Hanafuda A/1"),        new input_seq(KEYCODE_A) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_B,           N_p("input-name", "%p Hanafuda B/2"),        new input_seq(KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_C,           N_p("input-name", "%p Hanafuda C/3"),        new input_seq(KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_D,           N_p("input-name", "%p Hanafuda D/4"),        new input_seq(KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_E,           N_p("input-name", "%p Hanafuda E/5"),        new input_seq(KEYCODE_E) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_F,           N_p("input-name", "%p Hanafuda F/6"),        new input_seq(KEYCODE_F) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_G,           N_p("input-name", "%p Hanafuda G/7"),        new input_seq(KEYCODE_G) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_H,           N_p("input-name", "%p Hanafuda H/8"),        new input_seq(KEYCODE_H) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_YES,         N_p("input-name", "%p Hanafuda Yes"),        new input_seq(KEYCODE_M) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_NO,          N_p("input-name", "%p Hanafuda No"),         new input_seq(KEYCODE_N) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_gamble(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(gamble)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_KEYIN,         N_p("input-name", "Key In"),                 new input_seq(KEYCODE_Q) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_KEYOUT,        N_p("input-name", "Key Out"),                new input_seq(KEYCODE_W) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_SERVICE,       N_p("input-name", "Service"),                new input_seq(KEYCODE_9) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_BOOK,          N_p("input-name", "Book-Keeping"),           new input_seq(KEYCODE_0) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_DOOR,          N_p("input-name", "Door"),                   new input_seq(KEYCODE_O) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_PAYOUT,        N_p("input-name", "Payout"),                 new input_seq(KEYCODE_I) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_BET,           N_p("input-name", "Bet"),                    new input_seq(KEYCODE_M) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_DEAL,          N_p("input-name", "Deal"),                   new input_seq(KEYCODE_2) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_STAND,         N_p("input-name", "Stand"),                  new input_seq(KEYCODE_L) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_TAKE,          N_p("input-name", "Take Score"),             new input_seq(KEYCODE_4) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_D_UP,          N_p("input-name", "Double Up"),              new input_seq(KEYCODE_3) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_HALF,          N_p("input-name", "Half Gamble"),            new input_seq(KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_HIGH,          N_p("input-name", "High"),                   new input_seq(KEYCODE_A) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_LOW,           N_p("input-name", "Low"),                    new input_seq(KEYCODE_S) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_poker(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(poker)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_HOLD1,          N_p("input-name", "Hold 1"),                 new input_seq(KEYCODE_Z) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_HOLD2,          N_p("input-name", "Hold 2"),                 new input_seq(KEYCODE_X) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_HOLD3,          N_p("input-name", "Hold 3"),                 new input_seq(KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_HOLD4,          N_p("input-name", "Hold 4"),                 new input_seq(KEYCODE_V) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_HOLD5,          N_p("input-name", "Hold 5"),                 new input_seq(KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_CANCEL,         N_p("input-name", "Cancel"),                 new input_seq(KEYCODE_N) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_slot(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(slot)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SLOT_STOP1,           N_p("input-name", "Stop Reel 1"),            new input_seq(KEYCODE_X) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SLOT_STOP2,           N_p("input-name", "Stop Reel 2"),            new input_seq(KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SLOT_STOP3,           N_p("input-name", "Stop Reel 3"),            new input_seq(KEYCODE_V) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SLOT_STOP4,           N_p("input-name", "Stop Reel 4"),            new input_seq(KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SLOT_STOP_ALL,        N_p("input-name", "Stop All Reels"),         new input_seq(KEYCODE_Z) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p2(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p2)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICK_UP,          N_p("input-name", "%p Up"),                  new input_seq(KEYCODE_R, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICK_DOWN,        N_p("input-name", "%p Down"),                new input_seq(KEYCODE_F, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICK_LEFT,        N_p("input-name", "%p Left"),                new input_seq(KEYCODE_D, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICK_RIGHT,       N_p("input-name", "%p Right"),               new input_seq(KEYCODE_G, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKRIGHT_UP,     N_p("input-name", "%p Right Stick/Up"),      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKRIGHT_DOWN,   N_p("input-name", "%p Right Stick/Down"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKRIGHT_LEFT,   N_p("input-name", "%p Right Stick/Left"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKRIGHT_RIGHT,  N_p("input-name", "%p Right Stick/Right"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKLEFT_UP,      N_p("input-name", "%p Left Stick/Up"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKLEFT_DOWN,    N_p("input-name", "%p Left Stick/Down"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKLEFT_LEFT,    N_p("input-name", "%p Left Stick/Left"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKLEFT_RIGHT,   N_p("input-name", "%p Left Stick/Right"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON1,              N_p("input-name", "%p Button 1"),            new input_seq(KEYCODE_A, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(1), input_seq.or_code, MOUSECODE_BUTTON1_INDEXED(1), input_seq.or_code, GUNCODE_BUTTON1_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON2,              N_p("input-name", "%p Button 2"),            new input_seq(KEYCODE_S, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(1), input_seq.or_code, MOUSECODE_BUTTON3_INDEXED(1), input_seq.or_code, GUNCODE_BUTTON2_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON3,              N_p("input-name", "%p Button 3"),            new input_seq(KEYCODE_Q, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(1), input_seq.or_code, MOUSECODE_BUTTON2_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON4,              N_p("input-name", "%p Button 4"),            new input_seq(KEYCODE_W, input_seq.or_code, JOYCODE_BUTTON4_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON5,              N_p("input-name", "%p Button 5"),            new input_seq(KEYCODE_E, input_seq.or_code, JOYCODE_BUTTON5_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON6,              N_p("input-name", "%p Button 6"),            new input_seq(JOYCODE_BUTTON6_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON7,              N_p("input-name", "%p Button 7"),            new input_seq(JOYCODE_BUTTON7_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON8,              N_p("input-name", "%p Button 8"),            new input_seq(JOYCODE_BUTTON8_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON9,              N_p("input-name", "%p Button 9"),            new input_seq(JOYCODE_BUTTON9_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON10,             N_p("input-name", "%p Button 10"),           new input_seq(JOYCODE_BUTTON10_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON11,             N_p("input-name", "%p Button 11"),           new input_seq(JOYCODE_BUTTON11_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON12,             N_p("input-name", "%p Button 12"),           new input_seq(JOYCODE_BUTTON12_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON13,             N_p("input-name", "%p Button 13"),           new input_seq(JOYCODE_BUTTON13_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON14,             N_p("input-name", "%p Button 14"),           new input_seq(JOYCODE_BUTTON14_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON15,             N_p("input-name", "%p Button 15"),           new input_seq(JOYCODE_BUTTON15_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON16,             N_p("input-name", "%p Button 16"),           new input_seq(JOYCODE_BUTTON16_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, START,                N_p("input-name", "%p Start"),               new input_seq(KEYCODE_2, input_seq.or_code, JOYCODE_START_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, SELECT,               N_p("input-name", "%p Select"),              new input_seq(KEYCODE_6, input_seq.or_code, JOYCODE_SELECT_INDEXED(1)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p2_mahjong(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p2_mahjong)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_A,            N_p("input-name", "%p Mahjong A"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_B,            N_p("input-name", "%p Mahjong B"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_C,            N_p("input-name", "%p Mahjong C"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_D,            N_p("input-name", "%p Mahjong D"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_E,            N_p("input-name", "%p Mahjong E"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_F,            N_p("input-name", "%p Mahjong F"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_G,            N_p("input-name", "%p Mahjong G"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_H,            N_p("input-name", "%p Mahjong H"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_I,            N_p("input-name", "%p Mahjong I"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_J,            N_p("input-name", "%p Mahjong J"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_K,            N_p("input-name", "%p Mahjong K"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_L,            N_p("input-name", "%p Mahjong L"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_M,            N_p("input-name", "%p Mahjong M"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_N,            N_p("input-name", "%p Mahjong N"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_O,            N_p("input-name", "%p Mahjong O"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_P,            N_p("input-name", "%p Mahjong P"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_Q,            N_p("input-name", "%p Mahjong Q"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_KAN,          N_p("input-name", "%p Mahjong Kan"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_PON,          N_p("input-name", "%p Mahjong Pon"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_CHI,          N_p("input-name", "%p Mahjong Chi"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_REACH,        N_p("input-name", "%p Mahjong Reach"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_RON,          N_p("input-name", "%p Mahjong Ron"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2,  MAHJONG_FLIP_FLOP,   N_p("input-name", "%p Mahjong Flip Flop"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2,  MAHJONG_BET,         N_p("input-name", "%p Mahjong Bet"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2,  MAHJONG_SCORE,       N_p("input-name", "%p Mahjong Take Score"),  new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2,  MAHJONG_DOUBLE_UP,   N_p("input-name", "%p Mahjong Double Up"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2,  MAHJONG_BIG,         N_p("input-name", "%p Mahjong Big"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2,  MAHJONG_SMALL,       N_p("input-name", "%p Mahjong Small"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2,  MAHJONG_LAST_CHANCE, N_p("input-name", "%p Mahjong Last Chance"), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p2_hanafuda(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p2_hanafuda)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_A,           N_p("input-name", "%p Hanafuda A/1"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_B,           N_p("input-name", "%p Hanafuda B/2"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_C,           N_p("input-name", "%p Hanafuda C/3"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_D,           N_p("input-name", "%p Hanafuda D/4"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_E,           N_p("input-name", "%p Hanafuda E/5"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_F,           N_p("input-name", "%p Hanafuda F/6"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_G,           N_p("input-name", "%p Hanafuda G/7"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_H,           N_p("input-name", "%p Hanafuda H/8"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_YES,         N_p("input-name", "%p Hanafuda Yes"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_NO,          N_p("input-name", "%p Hanafuda No"),         new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p3(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p3)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICK_UP,          N_p("input-name", "%p Up"),                  new input_seq(KEYCODE_I, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICK_DOWN,        N_p("input-name", "%p Down"),                new input_seq(KEYCODE_K, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICK_LEFT,        N_p("input-name", "%p Left"),                new input_seq(KEYCODE_J, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICK_RIGHT,       N_p("input-name", "%p Right"),               new input_seq(KEYCODE_L, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKRIGHT_UP,     N_p("input-name", "%p Right Stick/Up"),      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKRIGHT_DOWN,   N_p("input-name", "%p Right Stick/Down"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKRIGHT_LEFT,   N_p("input-name", "%p Right Stick/Left"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKRIGHT_RIGHT,  N_p("input-name", "%p Right Stick/Right"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKLEFT_UP,      N_p("input-name", "%p Left Stick/Up"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKLEFT_DOWN,    N_p("input-name", "%p Left Stick/Down"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKLEFT_LEFT,    N_p("input-name", "%p Left Stick/Left"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKLEFT_RIGHT,   N_p("input-name", "%p Left Stick/Right"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON1,              N_p("input-name", "%p Button 1"),            new input_seq(KEYCODE_RCONTROL, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(2), input_seq.or_code, GUNCODE_BUTTON1_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON2,              N_p("input-name", "%p Button 2"),            new input_seq(KEYCODE_RSHIFT, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(2), input_seq.or_code, GUNCODE_BUTTON2_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON3,              N_p("input-name", "%p Button 3"),            new input_seq(KEYCODE_ENTER, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON4,              N_p("input-name", "%p Button 4"),            new input_seq(JOYCODE_BUTTON4_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON5,              N_p("input-name", "%p Button 5"),            new input_seq(JOYCODE_BUTTON5_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON6,              N_p("input-name", "%p Button 6"),            new input_seq(JOYCODE_BUTTON6_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON7,              N_p("input-name", "%p Button 7"),            new input_seq(JOYCODE_BUTTON7_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON8,              N_p("input-name", "%p Button 8"),            new input_seq(JOYCODE_BUTTON8_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON9,              N_p("input-name", "%p Button 9"),            new input_seq(JOYCODE_BUTTON9_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON10,             N_p("input-name", "%p Button 10"),           new input_seq(JOYCODE_BUTTON10_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON11,             N_p("input-name", "%p Button 11"),           new input_seq(JOYCODE_BUTTON11_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON12,             N_p("input-name", "%p Button 12"),           new input_seq(JOYCODE_BUTTON12_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON13,             N_p("input-name", "%p Button 13"),           new input_seq(JOYCODE_BUTTON13_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON14,             N_p("input-name", "%p Button 14"),           new input_seq(JOYCODE_BUTTON14_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON15,             N_p("input-name", "%p Button 15"),           new input_seq(JOYCODE_BUTTON15_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON16,             N_p("input-name", "%p Button 16"),           new input_seq(JOYCODE_BUTTON16_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, START,                N_p("input-name", "%p Start"),               new input_seq(KEYCODE_3, input_seq.or_code, JOYCODE_START_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, SELECT,               N_p("input-name", "%p Select"),              new input_seq(KEYCODE_7, input_seq.or_code, JOYCODE_SELECT_INDEXED(2)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p3_mahjong(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p3_mahjong)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_A,           N_p("input-name", "%p Mahjong A"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_B,           N_p("input-name", "%p Mahjong B"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_C,           N_p("input-name", "%p Mahjong C"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_D,           N_p("input-name", "%p Mahjong D"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_E,           N_p("input-name", "%p Mahjong E"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_F,           N_p("input-name", "%p Mahjong F"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_G,           N_p("input-name", "%p Mahjong G"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_H,           N_p("input-name", "%p Mahjong H"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_I,           N_p("input-name", "%p Mahjong I"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_J,           N_p("input-name", "%p Mahjong J"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_K,           N_p("input-name", "%p Mahjong K"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_L,           N_p("input-name", "%p Mahjong L"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_M,           N_p("input-name", "%p Mahjong M"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_N,           N_p("input-name", "%p Mahjong N"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_O,           N_p("input-name", "%p Mahjong O"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_P,           N_p("input-name", "%p Mahjong P"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_Q,           N_p("input-name", "%p Mahjong Q"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_KAN,         N_p("input-name", "%p Mahjong Kan"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_PON,         N_p("input-name", "%p Mahjong Pon"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_CHI,         N_p("input-name", "%p Mahjong Chi"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_REACH,       N_p("input-name", "%p Mahjong Reach"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_RON,         N_p("input-name", "%p Mahjong Ron"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_FLIP_FLOP,   N_p("input-name", "%p Mahjong Flip Flop"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_BET,         N_p("input-name", "%p Mahjong Bet"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_SCORE,       N_p("input-name", "%p Mahjong Take Score"),  new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_DOUBLE_UP,   N_p("input-name", "%p Mahjong Double Up"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_BIG,         N_p("input-name", "%p Mahjong Big"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_SMALL,       N_p("input-name", "%p Mahjong Small"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3,  MAHJONG_LAST_CHANCE, N_p("input-name", "%p Mahjong Last Chance"), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p4(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p4)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICK_UP,          N_p("input-name", "%p Up"),                  new input_seq(KEYCODE_8_PAD, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICK_DOWN,        N_p("input-name", "%p Down"),                new input_seq(KEYCODE_2_PAD, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICK_LEFT,        N_p("input-name", "%p Left"),                new input_seq(KEYCODE_4_PAD, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICK_RIGHT,       N_p("input-name", "%p Right"),               new input_seq(KEYCODE_6_PAD, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKRIGHT_UP,     N_p("input-name", "%p Right Stick/Up"),      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKRIGHT_DOWN,   N_p("input-name", "%p Right Stick/Down"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKRIGHT_LEFT,   N_p("input-name", "%p Right Stick/Left"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKRIGHT_RIGHT,  N_p("input-name", "%p Right Stick/Right"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKLEFT_UP,      N_p("input-name", "%p Left Stick/Up"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKLEFT_DOWN,    N_p("input-name", "%p Left Stick/Down"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKLEFT_LEFT,    N_p("input-name", "%p Left Stick/Left"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKLEFT_RIGHT,   N_p("input-name", "%p Left Stick/Right"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON1,              N_p("input-name", "%p Button 1"),            new input_seq(KEYCODE_0_PAD, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON2,              N_p("input-name", "%p Button 2"),            new input_seq(KEYCODE_DEL_PAD, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON3,              N_p("input-name", "%p Button 3"),            new input_seq(KEYCODE_ENTER_PAD, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON4,              N_p("input-name", "%p Button 4"),            new input_seq(JOYCODE_BUTTON4_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON5,              N_p("input-name", "%p Button 5"),            new input_seq(JOYCODE_BUTTON5_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON6,              N_p("input-name", "%p Button 6"),            new input_seq(JOYCODE_BUTTON6_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON7,              N_p("input-name", "%p Button 7"),            new input_seq(JOYCODE_BUTTON7_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON8,              N_p("input-name", "%p Button 8"),            new input_seq(JOYCODE_BUTTON8_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON9,              N_p("input-name", "%p Button 9"),            new input_seq(JOYCODE_BUTTON9_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON10,             N_p("input-name", "%p Button 10"),           new input_seq(JOYCODE_BUTTON10_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON11,             N_p("input-name", "%p Button 11"),           new input_seq(JOYCODE_BUTTON11_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON12,             N_p("input-name", "%p Button 12"),           new input_seq(JOYCODE_BUTTON12_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON13,             N_p("input-name", "%p Button 13"),           new input_seq(JOYCODE_BUTTON13_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON14,             N_p("input-name", "%p Button 14"),           new input_seq(JOYCODE_BUTTON14_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON15,             N_p("input-name", "%p Button 15"),           new input_seq(JOYCODE_BUTTON15_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON16,             N_p("input-name", "%p Button 16"),           new input_seq(JOYCODE_BUTTON16_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, START,                N_p("input-name", "%p Start"),               new input_seq(KEYCODE_4, input_seq.or_code, JOYCODE_START_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, SELECT,               N_p("input-name", "%p Select"),              new input_seq(KEYCODE_8, input_seq.or_code, JOYCODE_SELECT_INDEXED(3)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p4_mahjong(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p4_mahjong)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_A,           N_p("input-name", "%p Mahjong A"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_B,           N_p("input-name", "%p Mahjong B"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_C,           N_p("input-name", "%p Mahjong C"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_D,           N_p("input-name", "%p Mahjong D"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_E,           N_p("input-name", "%p Mahjong E"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_F,           N_p("input-name", "%p Mahjong F"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_G,           N_p("input-name", "%p Mahjong G"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_H,           N_p("input-name", "%p Mahjong H"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_I,           N_p("input-name", "%p Mahjong I"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_J,           N_p("input-name", "%p Mahjong J"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_K,           N_p("input-name", "%p Mahjong K"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_L,           N_p("input-name", "%p Mahjong L"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_M,           N_p("input-name", "%p Mahjong M"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_N,           N_p("input-name", "%p Mahjong N"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_O,           N_p("input-name", "%p Mahjong O"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_P,           N_p("input-name", "%p Mahjong P"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_Q,           N_p("input-name", "%p Mahjong Q"),           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_KAN,         N_p("input-name", "%p Mahjong Kan"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_PON,         N_p("input-name", "%p Mahjong Pon"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_CHI,         N_p("input-name", "%p Mahjong Chi"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_REACH,       N_p("input-name", "%p Mahjong Reach"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_RON,         N_p("input-name", "%p Mahjong Ron"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_FLIP_FLOP,   N_p("input-name", "%p Mahjong Flip Flop"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_BET,         N_p("input-name", "%p Mahjong Bet"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_SCORE,       N_p("input-name", "%p Mahjong Take Score"),  new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_DOUBLE_UP,   N_p("input-name", "%p Mahjong Double Up"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_BIG,         N_p("input-name", "%p Mahjong Big"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_SMALL,       N_p("input-name", "%p Mahjong Small"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4,  MAHJONG_LAST_CHANCE, N_p("input-name", "%p Mahjong Last Chance"), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p5(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p5)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICK_UP,          N_p("input-name", "%p Up"),                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICK_DOWN,        N_p("input-name", "%p Down"),                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICK_LEFT,        N_p("input-name", "%p Left"),                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICK_RIGHT,       N_p("input-name", "%p Right"),               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKRIGHT_UP,     N_p("input-name", "%p Right Stick/Up"),      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKRIGHT_DOWN,   N_p("input-name", "%p Right Stick/Down"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKRIGHT_LEFT,   N_p("input-name", "%p Right Stick/Left"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKRIGHT_RIGHT,  N_p("input-name", "%p Right Stick/Right"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKLEFT_UP,      N_p("input-name", "%p Left Stick/Up"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKLEFT_DOWN,    N_p("input-name", "%p Left Stick/Down"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKLEFT_LEFT,    N_p("input-name", "%p Left Stick/Left"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKLEFT_RIGHT,   N_p("input-name", "%p Left Stick/Right"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON2,              N_p("input-name", "%p Button 2"),            new input_seq(JOYCODE_BUTTON2_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON3,              N_p("input-name", "%p Button 3"),            new input_seq(JOYCODE_BUTTON3_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON4,              N_p("input-name", "%p Button 4"),            new input_seq(JOYCODE_BUTTON4_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON5,              N_p("input-name", "%p Button 5"),            new input_seq(JOYCODE_BUTTON5_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON6,              N_p("input-name", "%p Button 6"),            new input_seq(JOYCODE_BUTTON6_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON7,              N_p("input-name", "%p Button 7"),            new input_seq(JOYCODE_BUTTON7_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON1,              N_p("input-name", "%p Button 1"),            new input_seq(JOYCODE_BUTTON1_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON8,              N_p("input-name", "%p Button 8"),            new input_seq(JOYCODE_BUTTON8_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON9,              N_p("input-name", "%p Button 9"),            new input_seq(JOYCODE_BUTTON9_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON10,             N_p("input-name", "%p Button 10"),           new input_seq(JOYCODE_BUTTON10_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON11,             N_p("input-name", "%p Button 11"),           new input_seq(JOYCODE_BUTTON11_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON12,             N_p("input-name", "%p Button 12"),           new input_seq(JOYCODE_BUTTON12_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON13,             N_p("input-name", "%p Button 13"),           new input_seq(JOYCODE_BUTTON13_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON14,             N_p("input-name", "%p Button 14"),           new input_seq(JOYCODE_BUTTON14_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON15,             N_p("input-name", "%p Button 15"),           new input_seq(JOYCODE_BUTTON15_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON16,             N_p("input-name", "%p Button 16"),           new input_seq(JOYCODE_BUTTON16_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, START,                N_p("input-name", "%p Start"),               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, SELECT,               N_p("input-name", "%p Select"),              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p6(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p6)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICK_UP,          N_p("input-name", "%p Up"),                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICK_DOWN,        N_p("input-name", "%p Down"),                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICK_LEFT,        N_p("input-name", "%p Left"),                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICK_RIGHT,       N_p("input-name", "%p Right"),               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKRIGHT_UP,     N_p("input-name", "%p Right Stick/Up"),      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKRIGHT_DOWN,   N_p("input-name", "%p Right Stick/Down"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKRIGHT_LEFT,   N_p("input-name", "%p Right Stick/Left"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKRIGHT_RIGHT,  N_p("input-name", "%p Right Stick/Right"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKLEFT_UP,      N_p("input-name", "%p Left Stick/Up"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKLEFT_DOWN,    N_p("input-name", "%p Left Stick/Down"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKLEFT_LEFT,    N_p("input-name", "%p Left Stick/Left"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKLEFT_RIGHT,   N_p("input-name", "%p Left Stick/Right"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON1,              N_p("input-name", "%p Button 1"),            new input_seq(JOYCODE_BUTTON1_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON2,              N_p("input-name", "%p Button 2"),            new input_seq(JOYCODE_BUTTON2_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON3,              N_p("input-name", "%p Button 3"),            new input_seq(JOYCODE_BUTTON3_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON4,              N_p("input-name", "%p Button 4"),            new input_seq(JOYCODE_BUTTON4_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON5,              N_p("input-name", "%p Button 5"),            new input_seq(JOYCODE_BUTTON5_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON6,              N_p("input-name", "%p Button 6"),            new input_seq(JOYCODE_BUTTON6_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON7,              N_p("input-name", "%p Button 7"),            new input_seq(JOYCODE_BUTTON7_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON8,              N_p("input-name", "%p Button 8"),            new input_seq(JOYCODE_BUTTON8_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON9,              N_p("input-name", "%p Button 9"),            new input_seq(JOYCODE_BUTTON9_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON10,             N_p("input-name", "%p Button 10"),           new input_seq(JOYCODE_BUTTON10_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON11,             N_p("input-name", "%p Button 11"),           new input_seq(JOYCODE_BUTTON11_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON12,             N_p("input-name", "%p Button 12"),           new input_seq(JOYCODE_BUTTON12_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON13,             N_p("input-name", "%p Button 13"),           new input_seq(JOYCODE_BUTTON13_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON14,             N_p("input-name", "%p Button 14"),           new input_seq(JOYCODE_BUTTON14_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON15,             N_p("input-name", "%p Button 15"),           new input_seq(JOYCODE_BUTTON15_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON16,             N_p("input-name", "%p Button 16"),           new input_seq(JOYCODE_BUTTON16_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, START,                N_p("input-name", "%p Start"),               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, SELECT,               N_p("input-name", "%p Select"),              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p7(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p7)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICK_UP,          N_p("input-name", "%p Up"),                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICK_DOWN,        N_p("input-name", "%p Down"),                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICK_LEFT,        N_p("input-name", "%p Left"),                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICK_RIGHT,       N_p("input-name", "%p Right"),               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKRIGHT_UP,     N_p("input-name", "%p Right Stick/Up"),      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKRIGHT_DOWN,   N_p("input-name", "%p Right Stick/Down"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKRIGHT_LEFT,   N_p("input-name", "%p Right Stick/Left"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKRIGHT_RIGHT,  N_p("input-name", "%p Right Stick/Right"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKLEFT_UP,      N_p("input-name", "%p Left Stick/Up"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKLEFT_DOWN,    N_p("input-name", "%p Left Stick/Down"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKLEFT_LEFT,    N_p("input-name", "%p Left Stick/Left"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKLEFT_RIGHT,   N_p("input-name", "%p Left Stick/Right"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON1,              N_p("input-name", "%p Button 1"),            new input_seq(JOYCODE_BUTTON1_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON2,              N_p("input-name", "%p Button 2"),            new input_seq(JOYCODE_BUTTON2_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON3,              N_p("input-name", "%p Button 3"),            new input_seq(JOYCODE_BUTTON3_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON4,              N_p("input-name", "%p Button 4"),            new input_seq(JOYCODE_BUTTON4_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON5,              N_p("input-name", "%p Button 5"),            new input_seq(JOYCODE_BUTTON5_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON6,              N_p("input-name", "%p Button 6"),            new input_seq(JOYCODE_BUTTON6_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON7,              N_p("input-name", "%p Button 7"),            new input_seq(JOYCODE_BUTTON7_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON8,              N_p("input-name", "%p Button 8"),            new input_seq(JOYCODE_BUTTON8_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON9,              N_p("input-name", "%p Button 9"),            new input_seq(JOYCODE_BUTTON9_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON10,             N_p("input-name", "%p Button 10"),           new input_seq(JOYCODE_BUTTON10_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON11,             N_p("input-name", "%p Button 11"),           new input_seq(JOYCODE_BUTTON11_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON12,             N_p("input-name", "%p Button 12"),           new input_seq(JOYCODE_BUTTON12_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON13,             N_p("input-name", "%p Button 13"),           new input_seq(JOYCODE_BUTTON13_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON14,             N_p("input-name", "%p Button 14"),           new input_seq(JOYCODE_BUTTON14_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON15,             N_p("input-name", "%p Button 15"),           new input_seq(JOYCODE_BUTTON15_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON16,             N_p("input-name", "%p Button 16"),           new input_seq(JOYCODE_BUTTON16_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, START,                N_p("input-name", "%p Start"),               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, SELECT,               N_p("input-name", "%p Select"),              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p8(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p8)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICK_UP,          N_p("input-name", "%p Up"),                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICK_DOWN,        N_p("input-name", "%p Down"),                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICK_LEFT,        N_p("input-name", "%p Left"),                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICK_RIGHT,       N_p("input-name", "%p Right"),               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKRIGHT_UP,     N_p("input-name", "%p Right Stick/Up"),      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKRIGHT_DOWN,   N_p("input-name", "%p Right Stick/Down"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKRIGHT_LEFT,   N_p("input-name", "%p Right Stick/Left"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKRIGHT_RIGHT,  N_p("input-name", "%p Right Stick/Right"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKLEFT_UP,      N_p("input-name", "%p Left Stick/Up"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKLEFT_DOWN,    N_p("input-name", "%p Left Stick/Down"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKLEFT_LEFT,    N_p("input-name", "%p Left Stick/Left"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKLEFT_RIGHT,   N_p("input-name", "%p Left Stick/Right"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON1,              N_p("input-name", "%p Button 1"),            new input_seq(JOYCODE_BUTTON1_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON2,              N_p("input-name", "%p Button 2"),            new input_seq(JOYCODE_BUTTON2_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON3,              N_p("input-name", "%p Button 3"),            new input_seq(JOYCODE_BUTTON3_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON4,              N_p("input-name", "%p Button 4"),            new input_seq(JOYCODE_BUTTON4_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON5,              N_p("input-name", "%p Button 5"),            new input_seq(JOYCODE_BUTTON5_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON6,              N_p("input-name", "%p Button 6"),            new input_seq(JOYCODE_BUTTON6_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON7,              N_p("input-name", "%p Button 7"),            new input_seq(JOYCODE_BUTTON7_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON8,              N_p("input-name", "%p Button 8"),            new input_seq(JOYCODE_BUTTON8_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON9,              N_p("input-name", "%p Button 9"),            new input_seq(JOYCODE_BUTTON9_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON10,             N_p("input-name", "%p Button 10"),           new input_seq(JOYCODE_BUTTON10_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON11,             N_p("input-name", "%p Button 11"),           new input_seq(JOYCODE_BUTTON11_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON12,             N_p("input-name", "%p Button 12"),           new input_seq(JOYCODE_BUTTON12_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON13,             N_p("input-name", "%p Button 13"),           new input_seq(JOYCODE_BUTTON13_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON14,             N_p("input-name", "%p Button 14"),           new input_seq(JOYCODE_BUTTON14_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON15,             N_p("input-name", "%p Button 15"),           new input_seq(JOYCODE_BUTTON15_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON16,             N_p("input-name", "%p Button 16"),           new input_seq(JOYCODE_BUTTON16_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, START,                N_p("input-name", "%p Start"),               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, SELECT,               N_p("input-name", "%p Select"),              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p9(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p9)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICK_UP,          N_p("input-name", "%p Up"),                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICK_DOWN,        N_p("input-name", "%p Down"),                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICK_LEFT,        N_p("input-name", "%p Left"),                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICK_RIGHT,       N_p("input-name", "%p Right"),               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKRIGHT_UP,     N_p("input-name", "%p Right Stick/Up"),      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKRIGHT_DOWN,   N_p("input-name", "%p Right Stick/Down"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKRIGHT_LEFT,   N_p("input-name", "%p Right Stick/Left"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKRIGHT_RIGHT,  N_p("input-name", "%p Right Stick/Right"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKLEFT_UP,      N_p("input-name", "%p Left Stick/Up"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKLEFT_DOWN,    N_p("input-name", "%p Left Stick/Down"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKLEFT_LEFT,    N_p("input-name", "%p Left Stick/Left"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKLEFT_RIGHT,   N_p("input-name", "%p Left Stick/Right"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON1,              N_p("input-name", "%p Button 1"),            new input_seq(JOYCODE_BUTTON1_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON2,              N_p("input-name", "%p Button 2"),            new input_seq(JOYCODE_BUTTON2_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON3,              N_p("input-name", "%p Button 3"),            new input_seq(JOYCODE_BUTTON3_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON4,              N_p("input-name", "%p Button 4"),            new input_seq(JOYCODE_BUTTON4_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON5,              N_p("input-name", "%p Button 5"),            new input_seq(JOYCODE_BUTTON5_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON6,              N_p("input-name", "%p Button 6"),            new input_seq(JOYCODE_BUTTON6_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON7,              N_p("input-name", "%p Button 7"),            new input_seq(JOYCODE_BUTTON7_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON8,              N_p("input-name", "%p Button 8"),            new input_seq(JOYCODE_BUTTON8_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON9,              N_p("input-name", "%p Button 9"),            new input_seq(JOYCODE_BUTTON9_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON10,             N_p("input-name", "%p Button 10"),           new input_seq(JOYCODE_BUTTON10_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON11,             N_p("input-name", "%p Button 11"),           new input_seq(JOYCODE_BUTTON11_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON12,             N_p("input-name", "%p Button 12"),           new input_seq(JOYCODE_BUTTON12_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON13,             N_p("input-name", "%p Button 13"),           new input_seq(JOYCODE_BUTTON13_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON14,             N_p("input-name", "%p Button 14"),           new input_seq(JOYCODE_BUTTON14_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON15,             N_p("input-name", "%p Button 15"),           new input_seq(JOYCODE_BUTTON15_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON16,             N_p("input-name", "%p Button 16"),           new input_seq(JOYCODE_BUTTON16_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, START,                N_p("input-name", "%p Start"),               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, SELECT,               N_p("input-name", "%p Select"),              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p10(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p10)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICK_UP,         N_p("input-name", "%p Up"),                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICK_DOWN,       N_p("input-name", "%p Down"),                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICK_LEFT,       N_p("input-name", "%p Left"),                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICK_RIGHT,      N_p("input-name", "%p Right"),               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKRIGHT_UP,    N_p("input-name", "%p Right Stick/Up"),      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKRIGHT_DOWN,  N_p("input-name", "%p Right Stick/Down"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKRIGHT_LEFT,  N_p("input-name", "%p Right Stick/Left"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKRIGHT_RIGHT, N_p("input-name", "%p Right Stick/Right"),   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKLEFT_UP,     N_p("input-name", "%p Left Stick/Up"),       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKLEFT_DOWN,   N_p("input-name", "%p Left Stick/Down"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKLEFT_LEFT,   N_p("input-name", "%p Left Stick/Left"),     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKLEFT_RIGHT,  N_p("input-name", "%p Left Stick/Right"),    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON1,             N_p("input-name", "%p Button 1"),            new input_seq(JOYCODE_BUTTON1_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON2,             N_p("input-name", "%p Button 2"),            new input_seq(JOYCODE_BUTTON2_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON3,             N_p("input-name", "%p Button 3"),            new input_seq(JOYCODE_BUTTON3_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON4,             N_p("input-name", "%p Button 4"),            new input_seq(JOYCODE_BUTTON4_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON5,             N_p("input-name", "%p Button 5"),            new input_seq(JOYCODE_BUTTON5_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON6,             N_p("input-name", "%p Button 6"),            new input_seq(JOYCODE_BUTTON6_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON7,             N_p("input-name", "%p Button 7"),            new input_seq(JOYCODE_BUTTON7_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON8,             N_p("input-name", "%p Button 8"),            new input_seq(JOYCODE_BUTTON8_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON9,             N_p("input-name", "%p Button 9"),            new input_seq(JOYCODE_BUTTON9_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON10,            N_p("input-name", "%p Button 10"),           new input_seq(JOYCODE_BUTTON10_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON11,            N_p("input-name", "%p Button 11"),           new input_seq(JOYCODE_BUTTON11_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON12,            N_p("input-name", "%p Button 12"),           new input_seq(JOYCODE_BUTTON12_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON13,            N_p("input-name", "%p Button 13"),           new input_seq(JOYCODE_BUTTON13_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON14,            N_p("input-name", "%p Button 14"),           new input_seq(JOYCODE_BUTTON14_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON15,            N_p("input-name", "%p Button 15"),           new input_seq(JOYCODE_BUTTON15_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON16,            N_p("input-name", "%p Button 16"),           new input_seq(JOYCODE_BUTTON16_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, START,               N_p("input-name", "%p Start"),               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, SELECT,              N_p("input-name", "%p Select"),              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_start(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(start)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START1,              N_p("input-name", "1 Player Start"),         new input_seq(KEYCODE_1, input_seq.or_code, JOYCODE_START_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START2,              N_p("input-name", "2 Players Start"),        new input_seq(KEYCODE_2, input_seq.or_code, JOYCODE_START_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START3,              N_p("input-name", "3 Players Start"),        new input_seq(KEYCODE_3, input_seq.or_code, JOYCODE_START_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START4,              N_p("input-name", "4 Players Start"),        new input_seq(KEYCODE_4, input_seq.or_code, JOYCODE_START_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START5,              N_p("input-name", "5 Players Start"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START6,              N_p("input-name", "6 Players Start"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START7,              N_p("input-name", "7 Players Start"),        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START8,              N_p("input-name", "8 Players Start"),        new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_coin(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(coin)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN3,               N_p("input-name", "Coin 3"),                 new input_seq(KEYCODE_7, input_seq.or_code, JOYCODE_SELECT_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN1,               N_p("input-name", "Coin 1"),                 new input_seq(KEYCODE_5, input_seq.or_code, JOYCODE_SELECT_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN4,               N_p("input-name", "Coin 4"),                 new input_seq(KEYCODE_8, input_seq.or_code, JOYCODE_SELECT_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN2,               N_p("input-name", "Coin 2"),                 new input_seq(KEYCODE_6, input_seq.or_code, JOYCODE_SELECT_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN5,               N_p("input-name", "Coin 5"),                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN6,               N_p("input-name", "Coin 6"),                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN7,               N_p("input-name", "Coin 7"),                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN8,               N_p("input-name", "Coin 8"),                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN9,               N_p("input-name", "Coin 9"),                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN10,              N_p("input-name", "Coin 10"),                new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN11,              N_p("input-name", "Coin 11"),                new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN12,              N_p("input-name", "Coin 12"),                new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   BILL1,               N_p("input-name", "Bill 1"),                 new input_seq(KEYCODE_BACKSPACE) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_service(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(service)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   SERVICE1,            N_p("input-name", "Service 1"),              new input_seq(KEYCODE_9) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   SERVICE2,            N_p("input-name", "Service 2"),              new input_seq(KEYCODE_0) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   SERVICE3,            N_p("input-name", "Service 3"),              new input_seq(KEYCODE_MINUS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   SERVICE4,            N_p("input-name", "Service 4"),              new input_seq(KEYCODE_EQUALS) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_tilt(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(tilt)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   TILT1,               N_p("input-name", "Tilt 1"),                 new input_seq(KEYCODE_T) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   TILT2,               N_p("input-name", "Tilt 2"),                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   TILT3,               N_p("input-name", "Tilt 3"),                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   TILT4,               N_p("input-name", "Tilt 4"),                 new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_other(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(other)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   POWER_ON,            N_p("input-name", "Power On"),               new input_seq(KEYCODE_F1) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   POWER_OFF,           N_p("input-name", "Power Off"),              new input_seq(KEYCODE_F2) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   SERVICE,             N_p("input-name", "Service"),                new input_seq(KEYCODE_F2) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   TILT,                N_p("input-name", "Tilt"),                   new input_seq(KEYCODE_T) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   INTERLOCK,           N_p("input-name", "Door Interlock"),         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   MEMORY_RESET,        N_p("input-name", "Memory Reset"),           new input_seq(KEYCODE_F1) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   VOLUME_DOWN,         N_p("input-name", "Volume Down"),            new input_seq(KEYCODE_MINUS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   VOLUME_UP,           N_p("input-name", "Volume Up"),              new input_seq(KEYCODE_EQUALS) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_pedal(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(pedal)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  PEDAL,               N_p("input-name", "%p Pedal 1"),             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(0)), new input_seq(), new input_seq(KEYCODE_LCONTROL, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(0)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  PEDAL,               N_p("input-name", "%p Pedal 1"),             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(1)), new input_seq(), new input_seq(KEYCODE_A, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(1)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  PEDAL,               N_p("input-name", "%p Pedal 1"),             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(2)), new input_seq(), new input_seq(KEYCODE_RCONTROL, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(2)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  PEDAL,               N_p("input-name", "%p Pedal 1"),             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(3)), new input_seq(), new input_seq(KEYCODE_0_PAD, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(3)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  PEDAL,               N_p("input-name", "%p Pedal 1"),             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(4)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(4)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  PEDAL,               N_p("input-name", "%p Pedal 1"),             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(5)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(5)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  PEDAL,               N_p("input-name", "%p Pedal 1"),             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(6)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(6)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  PEDAL,               N_p("input-name", "%p Pedal 1"),             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(7)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(7)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  PEDAL,               N_p("input-name", "%p Pedal 1"),             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(8)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(8)) );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, PEDAL,               N_p("input-name", "%p Pedal 1"),             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(9)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(9)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_pedal2(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(pedal2)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  PEDAL2,              N_p("input-name", "%p Pedal 2"),             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(0)), new input_seq(), new input_seq(KEYCODE_LALT, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  PEDAL2,              N_p("input-name", "%p Pedal 2"),             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(1)), new input_seq(), new input_seq(KEYCODE_S, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(1)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  PEDAL2,              N_p("input-name", "%p Pedal 2"),             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(2)), new input_seq(), new input_seq(KEYCODE_RSHIFT, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(2)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  PEDAL2,              N_p("input-name", "%p Pedal 2"),             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(3)), new input_seq(), new input_seq(KEYCODE_DEL_PAD, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(3)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  PEDAL2,              N_p("input-name", "%p Pedal 2"),             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(4)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(4)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  PEDAL2,              N_p("input-name", "%p Pedal 2"),             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(5)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(5)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  PEDAL2,              N_p("input-name", "%p Pedal 2"),             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(6)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(6)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  PEDAL2,              N_p("input-name", "%p Pedal 2"),             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(7)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(7)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  PEDAL2,              N_p("input-name", "%p Pedal 2"),             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(8)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(8)) );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, PEDAL2,              N_p("input-name", "%p Pedal 2"),             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(9)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(9)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_pedal3(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(pedal3)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  PEDAL3,              N_p("input-name", "%p Pedal 3"),             new input_seq(), new input_seq(), new input_seq(KEYCODE_SPACE, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(0)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  PEDAL3,              N_p("input-name", "%p Pedal 3"),             new input_seq(), new input_seq(), new input_seq(KEYCODE_Q, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(1)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  PEDAL3,              N_p("input-name", "%p Pedal 3"),             new input_seq(), new input_seq(), new input_seq(KEYCODE_ENTER, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(2)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  PEDAL3,              N_p("input-name", "%p Pedal 3"),             new input_seq(), new input_seq(), new input_seq(KEYCODE_ENTER_PAD, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(3)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  PEDAL3,              N_p("input-name", "%p Pedal 3"),             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(4)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  PEDAL3,              N_p("input-name", "%p Pedal 3"),             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(5)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  PEDAL3,              N_p("input-name", "%p Pedal 3"),             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(6)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  PEDAL3,              N_p("input-name", "%p Pedal 3"),             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(7)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  PEDAL3,              N_p("input-name", "%p Pedal 3"),             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(8)) );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, PEDAL3,              N_p("input-name", "%p Pedal 3"),             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(9)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_paddle(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(paddle)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  PADDLE,              N_p("input-name", "Paddle"),                 new input_seq(JOYCODE_X_INDEXED(0), input_seq.or_code, MOUSECODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  PADDLE,              N_p("input-name", "Paddle 2"),               new input_seq(JOYCODE_X_INDEXED(1), input_seq.or_code, MOUSECODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  PADDLE,              N_p("input-name", "Paddle 3"),               new input_seq(JOYCODE_X_INDEXED(2), input_seq.or_code, MOUSECODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  PADDLE,              N_p("input-name", "Paddle 4"),               new input_seq(JOYCODE_X_INDEXED(3), input_seq.or_code, MOUSECODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  PADDLE,              N_p("input-name", "Paddle 5"),               new input_seq(JOYCODE_X_INDEXED(4), input_seq.or_code, MOUSECODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  PADDLE,              N_p("input-name", "Paddle 6"),               new input_seq(JOYCODE_X_INDEXED(5), input_seq.or_code, MOUSECODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  PADDLE,              N_p("input-name", "Paddle 7"),               new input_seq(JOYCODE_X_INDEXED(6), input_seq.or_code, MOUSECODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  PADDLE,              N_p("input-name", "Paddle 8"),               new input_seq(JOYCODE_X_INDEXED(7), input_seq.or_code, MOUSECODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  PADDLE,              N_p("input-name", "Paddle 9"),               new input_seq(JOYCODE_X_INDEXED(8), input_seq.or_code, MOUSECODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, PADDLE,              N_p("input-name", "Paddle 10"),              new input_seq(JOYCODE_X_INDEXED(9), input_seq.or_code, MOUSECODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_paddle_v(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(paddle_v)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  PADDLE_V,            N_p("input-name", "Paddle V"),               new input_seq(JOYCODE_Y_INDEXED(0), input_seq.or_code, MOUSECODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  PADDLE_V,            N_p("input-name", "Paddle V 2"),             new input_seq(JOYCODE_Y_INDEXED(1), input_seq.or_code, MOUSECODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  PADDLE_V,            N_p("input-name", "Paddle V 3"),             new input_seq(JOYCODE_Y_INDEXED(2), input_seq.or_code, MOUSECODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  PADDLE_V,            N_p("input-name", "Paddle V 4"),             new input_seq(JOYCODE_Y_INDEXED(3), input_seq.or_code, MOUSECODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  PADDLE_V,            N_p("input-name", "Paddle V 5"),             new input_seq(JOYCODE_Y_INDEXED(4), input_seq.or_code, MOUSECODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  PADDLE_V,            N_p("input-name", "Paddle V 6"),             new input_seq(JOYCODE_Y_INDEXED(5), input_seq.or_code, MOUSECODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  PADDLE_V,            N_p("input-name", "Paddle V 7"),             new input_seq(JOYCODE_Y_INDEXED(6), input_seq.or_code, MOUSECODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  PADDLE_V,            N_p("input-name", "Paddle V 8"),             new input_seq(JOYCODE_Y_INDEXED(7), input_seq.or_code, MOUSECODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  PADDLE_V,            N_p("input-name", "Paddle V 9"),             new input_seq(JOYCODE_Y_INDEXED(8), input_seq.or_code, MOUSECODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, PADDLE_V,            N_p("input-name", "Paddle V 10"),           new input_seq(JOYCODE_Y_INDEXED(9), input_seq.or_code, MOUSECODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_positional(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(positional)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  POSITIONAL,          N_p("input-name", "Positional"),             new input_seq(MOUSECODE_X_INDEXED(0), input_seq.or_code, JOYCODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  POSITIONAL,          N_p("input-name", "Positional 2"),           new input_seq(MOUSECODE_X_INDEXED(1), input_seq.or_code, JOYCODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  POSITIONAL,          N_p("input-name", "Positional 3"),           new input_seq(MOUSECODE_X_INDEXED(2), input_seq.or_code, JOYCODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  POSITIONAL,          N_p("input-name", "Positional 4"),           new input_seq(MOUSECODE_X_INDEXED(3), input_seq.or_code, JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  POSITIONAL,          N_p("input-name", "Positional 5"),           new input_seq(MOUSECODE_X_INDEXED(4), input_seq.or_code, JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  POSITIONAL,          N_p("input-name", "Positional 6"),           new input_seq(MOUSECODE_X_INDEXED(5), input_seq.or_code, JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  POSITIONAL,          N_p("input-name", "Positional 7"),           new input_seq(MOUSECODE_X_INDEXED(6), input_seq.or_code, JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  POSITIONAL,          N_p("input-name", "Positional 8"),           new input_seq(MOUSECODE_X_INDEXED(7), input_seq.or_code, JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  POSITIONAL,          N_p("input-name", "Positional 9"),           new input_seq(MOUSECODE_X_INDEXED(8), input_seq.or_code, JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, POSITIONAL,          N_p("input-name", "Positional 10"),          new input_seq(MOUSECODE_X_INDEXED(9), input_seq.or_code, JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_positional_v(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(positional_v)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  POSITIONAL_V,        N_p("input-name", "Positional V"),           new input_seq(MOUSECODE_Y_INDEXED(0), input_seq.or_code, JOYCODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  POSITIONAL_V,        N_p("input-name", "Positional V 2"),         new input_seq(MOUSECODE_Y_INDEXED(1), input_seq.or_code, JOYCODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  POSITIONAL_V,        N_p("input-name", "Positional V 3"),         new input_seq(MOUSECODE_Y_INDEXED(2), input_seq.or_code, JOYCODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  POSITIONAL_V,        N_p("input-name", "Positional V 4"),         new input_seq(MOUSECODE_Y_INDEXED(3), input_seq.or_code, JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  POSITIONAL_V,        N_p("input-name", "Positional V 5"),         new input_seq(MOUSECODE_Y_INDEXED(4), input_seq.or_code, JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  POSITIONAL_V,        N_p("input-name", "Positional V 6"),         new input_seq(MOUSECODE_Y_INDEXED(5), input_seq.or_code, JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  POSITIONAL_V,        N_p("input-name", "Positional V 7"),         new input_seq(MOUSECODE_Y_INDEXED(6), input_seq.or_code, JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  POSITIONAL_V,        N_p("input-name", "Positional V 8"),         new input_seq(MOUSECODE_Y_INDEXED(7), input_seq.or_code, JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  POSITIONAL_V,        N_p("input-name", "Positional V 9"),         new input_seq(MOUSECODE_Y_INDEXED(8), input_seq.or_code, JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, POSITIONAL_V,        N_p("input-name", "Positional V 10"),        new input_seq(MOUSECODE_Y_INDEXED(9), input_seq.or_code, JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_dial(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(dial)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  DIAL,                N_p("input-name", "Dial"),                   new input_seq(MOUSECODE_X_INDEXED(0), input_seq.or_code, JOYCODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  DIAL,                N_p("input-name", "Dial 2"),                 new input_seq(MOUSECODE_X_INDEXED(1), input_seq.or_code, JOYCODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  DIAL,                N_p("input-name", "Dial 3"),                 new input_seq(MOUSECODE_X_INDEXED(2), input_seq.or_code, JOYCODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  DIAL,                N_p("input-name", "Dial 4"),                 new input_seq(MOUSECODE_X_INDEXED(3), input_seq.or_code, JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  DIAL,                N_p("input-name", "Dial 5"),                 new input_seq(MOUSECODE_X_INDEXED(4), input_seq.or_code, JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  DIAL,                N_p("input-name", "Dial 6"),                 new input_seq(MOUSECODE_X_INDEXED(5), input_seq.or_code, JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  DIAL,                N_p("input-name", "Dial 7"),                 new input_seq(MOUSECODE_X_INDEXED(6), input_seq.or_code, JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  DIAL,                N_p("input-name", "Dial 8"),                 new input_seq(MOUSECODE_X_INDEXED(7), input_seq.or_code, JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  DIAL,                N_p("input-name", "Dial 9"),                 new input_seq(MOUSECODE_X_INDEXED(8), input_seq.or_code, JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, DIAL,                N_p("input-name", "Dial 10"),                new input_seq(MOUSECODE_X_INDEXED(9), input_seq.or_code, JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_dial_v(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(dial_v)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  DIAL_V,              N_p("input-name", "Dial V"),                 new input_seq(MOUSECODE_Y_INDEXED(0), input_seq.or_code, JOYCODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  DIAL_V,              N_p("input-name", "Dial V 2"),               new input_seq(MOUSECODE_Y_INDEXED(1), input_seq.or_code, JOYCODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  DIAL_V,              N_p("input-name", "Dial V 3"),               new input_seq(MOUSECODE_Y_INDEXED(2), input_seq.or_code, JOYCODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  DIAL_V,              N_p("input-name", "Dial V 4"),               new input_seq(MOUSECODE_Y_INDEXED(3), input_seq.or_code, JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  DIAL_V,              N_p("input-name", "Dial V 5"),               new input_seq(MOUSECODE_Y_INDEXED(4), input_seq.or_code, JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  DIAL_V,              N_p("input-name", "Dial V 6"),               new input_seq(MOUSECODE_Y_INDEXED(5), input_seq.or_code, JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  DIAL_V,              N_p("input-name", "Dial V 7"),               new input_seq(MOUSECODE_Y_INDEXED(6), input_seq.or_code, JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  DIAL_V,              N_p("input-name", "Dial V 8"),               new input_seq(MOUSECODE_Y_INDEXED(7), input_seq.or_code, JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  DIAL_V,              N_p("input-name", "Dial V 9"),               new input_seq(MOUSECODE_Y_INDEXED(8), input_seq.or_code, JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, DIAL_V,              N_p("input-name", "Dial V 10"),              new input_seq(MOUSECODE_Y_INDEXED(9), input_seq.or_code, JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_trackball_x(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(trackball_x)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  TRACKBALL_X,         N_p("input-name", "Track X"),                new input_seq(MOUSECODE_X_INDEXED(0), input_seq.or_code, JOYCODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  TRACKBALL_X,         N_p("input-name", "Track X 2"),              new input_seq(MOUSECODE_X_INDEXED(1), input_seq.or_code, JOYCODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  TRACKBALL_X,         N_p("input-name", "Track X 3"),              new input_seq(MOUSECODE_X_INDEXED(2), input_seq.or_code, JOYCODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  TRACKBALL_X,         N_p("input-name", "Track X 4"),              new input_seq(MOUSECODE_X_INDEXED(3), input_seq.or_code, JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  TRACKBALL_X,         N_p("input-name", "Track X 5"),              new input_seq(MOUSECODE_X_INDEXED(4), input_seq.or_code, JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  TRACKBALL_X,         N_p("input-name", "Track X 6"),              new input_seq(MOUSECODE_X_INDEXED(5), input_seq.or_code, JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  TRACKBALL_X,         N_p("input-name", "Track X 7"),              new input_seq(MOUSECODE_X_INDEXED(6), input_seq.or_code, JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  TRACKBALL_X,         N_p("input-name", "Track X 8"),              new input_seq(MOUSECODE_X_INDEXED(7), input_seq.or_code, JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  TRACKBALL_X,         N_p("input-name", "Track X 9"),              new input_seq(MOUSECODE_X_INDEXED(8), input_seq.or_code, JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, TRACKBALL_X,         N_p("input-name", "Track X 10"),             new input_seq(MOUSECODE_X_INDEXED(9), input_seq.or_code, JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_trackball_y(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(trackball_y)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  TRACKBALL_Y,         N_p("input-name", "Track Y"),                new input_seq(MOUSECODE_Y_INDEXED(0), input_seq.or_code, JOYCODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  TRACKBALL_Y,         N_p("input-name", "Track Y 2"),              new input_seq(MOUSECODE_Y_INDEXED(1), input_seq.or_code, JOYCODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  TRACKBALL_Y,         N_p("input-name", "Track Y 3"),              new input_seq(MOUSECODE_Y_INDEXED(2), input_seq.or_code, JOYCODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  TRACKBALL_Y,         N_p("input-name", "Track Y 4"),              new input_seq(MOUSECODE_Y_INDEXED(3), input_seq.or_code, JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  TRACKBALL_Y,         N_p("input-name", "Track Y 5"),              new input_seq(MOUSECODE_Y_INDEXED(4), input_seq.or_code, JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  TRACKBALL_Y,         N_p("input-name", "Track Y 6"),              new input_seq(MOUSECODE_Y_INDEXED(5), input_seq.or_code, JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  TRACKBALL_Y,         N_p("input-name", "Track Y 7"),              new input_seq(MOUSECODE_Y_INDEXED(6), input_seq.or_code, JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  TRACKBALL_Y,         N_p("input-name", "Track Y 8"),              new input_seq(MOUSECODE_Y_INDEXED(7), input_seq.or_code, JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  TRACKBALL_Y,         N_p("input-name", "Track Y 9"),              new input_seq(MOUSECODE_Y_INDEXED(8), input_seq.or_code, JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, TRACKBALL_Y,         N_p("input-name", "Track Y 10"),             new input_seq(MOUSECODE_Y_INDEXED(9), input_seq.or_code, JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_ad_stick_x(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(ad_stick_x)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  AD_STICK_X,          N_p("input-name", "AD Stick X"),             new input_seq(JOYCODE_X_INDEXED(0), input_seq.or_code, MOUSECODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  AD_STICK_X,          N_p("input-name", "AD Stick X 2"),           new input_seq(JOYCODE_X_INDEXED(1), input_seq.or_code, MOUSECODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  AD_STICK_X,          N_p("input-name", "AD Stick X 3"),           new input_seq(JOYCODE_X_INDEXED(2), input_seq.or_code, MOUSECODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  AD_STICK_X,          N_p("input-name", "AD Stick X 4"),           new input_seq(JOYCODE_X_INDEXED(3), input_seq.or_code, MOUSECODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  AD_STICK_X,          N_p("input-name", "AD Stick X 5"),           new input_seq(JOYCODE_X_INDEXED(4), input_seq.or_code, MOUSECODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  AD_STICK_X,          N_p("input-name", "AD Stick X 6"),           new input_seq(JOYCODE_X_INDEXED(5), input_seq.or_code, MOUSECODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  AD_STICK_X,          N_p("input-name", "AD Stick X 7"),           new input_seq(JOYCODE_X_INDEXED(6), input_seq.or_code, MOUSECODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  AD_STICK_X,          N_p("input-name", "AD Stick X 8"),           new input_seq(JOYCODE_X_INDEXED(7), input_seq.or_code, MOUSECODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  AD_STICK_X,          N_p("input-name", "AD Stick X 9"),           new input_seq(JOYCODE_X_INDEXED(8), input_seq.or_code, MOUSECODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, AD_STICK_X,          N_p("input-name", "AD Stick X 10"),          new input_seq(JOYCODE_X_INDEXED(9), input_seq.or_code, MOUSECODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_ad_stick_y(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(ad_stick_y)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  AD_STICK_Y,          N_p("input-name", "AD Stick Y"),             new input_seq(JOYCODE_Y_INDEXED(0), input_seq.or_code, MOUSECODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  AD_STICK_Y,          N_p("input-name", "AD Stick Y 2"),           new input_seq(JOYCODE_Y_INDEXED(1), input_seq.or_code, MOUSECODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  AD_STICK_Y,          N_p("input-name", "AD Stick Y 3"),           new input_seq(JOYCODE_Y_INDEXED(2), input_seq.or_code, MOUSECODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  AD_STICK_Y,          N_p("input-name", "AD Stick Y 4"),           new input_seq(JOYCODE_Y_INDEXED(3), input_seq.or_code, MOUSECODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  AD_STICK_Y,          N_p("input-name", "AD Stick Y 5"),           new input_seq(JOYCODE_Y_INDEXED(4), input_seq.or_code, MOUSECODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  AD_STICK_Y,          N_p("input-name", "AD Stick Y 6"),           new input_seq(JOYCODE_Y_INDEXED(5), input_seq.or_code, MOUSECODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  AD_STICK_Y,          N_p("input-name", "AD Stick Y 7"),           new input_seq(JOYCODE_Y_INDEXED(6), input_seq.or_code, MOUSECODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  AD_STICK_Y,          N_p("input-name", "AD Stick Y 8"),           new input_seq(JOYCODE_Y_INDEXED(7), input_seq.or_code, MOUSECODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  AD_STICK_Y,          N_p("input-name", "AD Stick Y 9"),           new input_seq(JOYCODE_Y_INDEXED(8), input_seq.or_code, MOUSECODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, AD_STICK_Y,          N_p("input-name", "AD Stick Y 10"),          new input_seq(JOYCODE_Y_INDEXED(9), input_seq.or_code, MOUSECODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_ad_stick_z(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(ad_stick_z)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  AD_STICK_Z,          N_p("input-name", "AD Stick Z"),             new input_seq(JOYCODE_Z_INDEXED(0)), new input_seq(KEYCODE_A), new input_seq(KEYCODE_Z) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  AD_STICK_Z,          N_p("input-name", "AD Stick Z 2"),           new input_seq(JOYCODE_Z_INDEXED(1)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  AD_STICK_Z,          N_p("input-name", "AD Stick Z 3"),           new input_seq(JOYCODE_Z_INDEXED(2)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  AD_STICK_Z,          N_p("input-name", "AD Stick Z 4"),           new input_seq(JOYCODE_Z_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  AD_STICK_Z,          N_p("input-name", "AD Stick Z 5"),           new input_seq(JOYCODE_Z_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  AD_STICK_Z,          N_p("input-name", "AD Stick Z 6"),           new input_seq(JOYCODE_Z_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  AD_STICK_Z,          N_p("input-name", "AD Stick Z 7"),           new input_seq(JOYCODE_Z_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  AD_STICK_Z,          N_p("input-name", "AD Stick Z 8"),           new input_seq(JOYCODE_Z_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  AD_STICK_Z,          N_p("input-name", "AD Stick Z 9"),           new input_seq(JOYCODE_Z_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, AD_STICK_Z,          N_p("input-name", "AD Stick Z 10"),          new input_seq(JOYCODE_Z_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_lightgun_x(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(lightgun_x)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  LIGHTGUN_X,          N_p("input-name", "Lightgun X"),             new input_seq(GUNCODE_X_INDEXED(0), input_seq.or_code, MOUSECODE_X_INDEXED(0), input_seq.or_code, JOYCODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  LIGHTGUN_X,          N_p("input-name", "Lightgun X 2"),           new input_seq(GUNCODE_X_INDEXED(1), input_seq.or_code, MOUSECODE_X_INDEXED(1), input_seq.or_code, JOYCODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  LIGHTGUN_X,          N_p("input-name", "Lightgun X 3"),           new input_seq(GUNCODE_X_INDEXED(2), input_seq.or_code, MOUSECODE_X_INDEXED(2), input_seq.or_code, JOYCODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  LIGHTGUN_X,          N_p("input-name", "Lightgun X 4"),           new input_seq(GUNCODE_X_INDEXED(3), input_seq.or_code, MOUSECODE_X_INDEXED(3), input_seq.or_code, JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  LIGHTGUN_X,          N_p("input-name", "Lightgun X 5"),           new input_seq(GUNCODE_X_INDEXED(4), input_seq.or_code, MOUSECODE_X_INDEXED(4), input_seq.or_code, JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  LIGHTGUN_X,          N_p("input-name", "Lightgun X 6"),           new input_seq(GUNCODE_X_INDEXED(5), input_seq.or_code, MOUSECODE_X_INDEXED(5), input_seq.or_code, JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  LIGHTGUN_X,          N_p("input-name", "Lightgun X 7"),           new input_seq(GUNCODE_X_INDEXED(6), input_seq.or_code, MOUSECODE_X_INDEXED(6), input_seq.or_code, JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  LIGHTGUN_X,          N_p("input-name", "Lightgun X 8"),           new input_seq(GUNCODE_X_INDEXED(7), input_seq.or_code, MOUSECODE_X_INDEXED(7), input_seq.or_code, JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  LIGHTGUN_X,          N_p("input-name", "Lightgun X 9"),           new input_seq(GUNCODE_X_INDEXED(8), input_seq.or_code, MOUSECODE_X_INDEXED(8), input_seq.or_code, JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, LIGHTGUN_X,          N_p("input-name", "Lightgun X 10"),          new input_seq(GUNCODE_X_INDEXED(9), input_seq.or_code, MOUSECODE_X_INDEXED(9), input_seq.or_code, JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_lightgun_y(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(lightgun_y)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  LIGHTGUN_Y,          N_p("input-name", "Lightgun Y"),             new input_seq(GUNCODE_Y_INDEXED(0), input_seq.or_code, MOUSECODE_Y_INDEXED(0), input_seq.or_code, JOYCODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  LIGHTGUN_Y,          N_p("input-name", "Lightgun Y 2"),           new input_seq(GUNCODE_Y_INDEXED(1), input_seq.or_code, MOUSECODE_Y_INDEXED(1), input_seq.or_code, JOYCODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  LIGHTGUN_Y,          N_p("input-name", "Lightgun Y 3"),           new input_seq(GUNCODE_Y_INDEXED(2), input_seq.or_code, MOUSECODE_Y_INDEXED(2), input_seq.or_code, JOYCODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  LIGHTGUN_Y,          N_p("input-name", "Lightgun Y 4"),           new input_seq(GUNCODE_Y_INDEXED(3), input_seq.or_code, MOUSECODE_Y_INDEXED(3), input_seq.or_code, JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  LIGHTGUN_Y,          N_p("input-name", "Lightgun Y 5"),           new input_seq(GUNCODE_Y_INDEXED(4), input_seq.or_code, MOUSECODE_Y_INDEXED(4), input_seq.or_code, JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  LIGHTGUN_Y,          N_p("input-name", "Lightgun Y 6"),           new input_seq(GUNCODE_Y_INDEXED(5), input_seq.or_code, MOUSECODE_Y_INDEXED(5), input_seq.or_code, JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  LIGHTGUN_Y,          N_p("input-name", "Lightgun Y 7"),           new input_seq(GUNCODE_Y_INDEXED(6), input_seq.or_code, MOUSECODE_Y_INDEXED(6), input_seq.or_code, JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  LIGHTGUN_Y,          N_p("input-name", "Lightgun Y 8"),           new input_seq(GUNCODE_Y_INDEXED(7), input_seq.or_code, MOUSECODE_Y_INDEXED(7), input_seq.or_code, JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  LIGHTGUN_Y,          N_p("input-name", "Lightgun Y 9"),           new input_seq(GUNCODE_Y_INDEXED(8), input_seq.or_code, MOUSECODE_Y_INDEXED(8), input_seq.or_code, JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, LIGHTGUN_Y,          N_p("input-name", "Lightgun Y 10"),          new input_seq(GUNCODE_Y_INDEXED(9), input_seq.or_code, MOUSECODE_Y_INDEXED(9), input_seq.or_code, JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_mouse_x(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(mouse_x)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  MOUSE_X,             N_p("input-name", "Mouse X"),                new input_seq(MOUSECODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  MOUSE_X,             N_p("input-name", "Mouse X 2"),              new input_seq(MOUSECODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  MOUSE_X,             N_p("input-name", "Mouse X 3"),              new input_seq(MOUSECODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  MOUSE_X,             N_p("input-name", "Mouse X 4"),              new input_seq(MOUSECODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  MOUSE_X,             N_p("input-name", "Mouse X 5"),              new input_seq(MOUSECODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  MOUSE_X,             N_p("input-name", "Mouse X 6"),              new input_seq(MOUSECODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  MOUSE_X,             N_p("input-name", "Mouse X 7"),              new input_seq(MOUSECODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  MOUSE_X,             N_p("input-name", "Mouse X 8"),              new input_seq(MOUSECODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  MOUSE_X,             N_p("input-name", "Mouse X 9"),              new input_seq(MOUSECODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, MOUSE_X,             N_p("input-name", "Mouse X 10"),             new input_seq(MOUSECODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_mouse_y(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(mouse_y)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1,  MOUSE_Y,             N_p("input-name", "Mouse Y"),                new input_seq(MOUSECODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2,  MOUSE_Y,             N_p("input-name", "Mouse Y 2"),              new input_seq(MOUSECODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3,  MOUSE_Y,             N_p("input-name", "Mouse Y 3"),              new input_seq(MOUSECODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4,  MOUSE_Y,             N_p("input-name", "Mouse Y 4"),              new input_seq(MOUSECODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5,  MOUSE_Y,             N_p("input-name", "Mouse Y 5"),              new input_seq(MOUSECODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6,  MOUSE_Y,             N_p("input-name", "Mouse Y 6"),              new input_seq(MOUSECODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7,  MOUSE_Y,             N_p("input-name", "Mouse Y 7"),              new input_seq(MOUSECODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8,  MOUSE_Y,             N_p("input-name", "Mouse Y 8"),              new input_seq(MOUSECODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9,  MOUSE_Y,             N_p("input-name", "Mouse Y 9"),              new input_seq(MOUSECODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, MOUSE_Y,             N_p("input-name", "Mouse Y 10"),             new input_seq(MOUSECODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_keypad(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(keypad)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   KEYPAD,              N_p("input-name", "Keypad"),                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   KEYBOARD,            N_p("input-name", "Keyboard"),               new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_ui(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(ui)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_ON_SCREEN_DISPLAY, N_p("input-name", "On Screen Display"),      new input_seq(KEYCODE_TILDE, input_seq.not_code, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_DEBUG_BREAK,       N_p("input-name", "Break in Debugger"),      new input_seq(KEYCODE_TILDE, input_seq.not_code, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_CONFIGURE,         N_p("input-name", "Config Menu"),            new input_seq(KEYCODE_TAB) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PAUSE,             N_p("input-name", "Pause"),                  new input_seq(KEYCODE_P, input_seq.not_code, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PAUSE_SINGLE,      N_p("input-name", "Pause - Single Step"),    new input_seq(KEYCODE_P, KEYCODE_LSHIFT, input_seq.or_code, KEYCODE_P, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_REWIND_SINGLE,     N_p("input-name", "Rewind - Single Step"),   new input_seq(KEYCODE_TILDE, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_RESET_MACHINE,     N_p("input-name", "Reset Machine"),          new input_seq(KEYCODE_F3, KEYCODE_LSHIFT, input_seq.or_code, KEYCODE_F3, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SOFT_RESET,        N_p("input-name", "Soft Reset"),             new input_seq(KEYCODE_F3, input_seq.not_code, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SHOW_GFX,          N_p("input-name", "Show Decoded Graphics"),  new input_seq(KEYCODE_F4) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FRAMESKIP_DEC,     N_p("input-name", "Frameskip Dec"),          new input_seq(KEYCODE_F8) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FRAMESKIP_INC,     N_p("input-name", "Frameskip Inc"),          new input_seq(KEYCODE_F9) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_THROTTLE,          N_p("input-name", "Throttle"),               new input_seq(KEYCODE_F10) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FAST_FORWARD,      N_p("input-name", "Fast Forward"),           new input_seq(KEYCODE_INSERT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SHOW_FPS,          N_p("input-name", "Show FPS"),               new input_seq(KEYCODE_F11, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SNAPSHOT,          N_p("input-name", "Save Snapshot"),          new input_seq(KEYCODE_F12, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_RECORD_MNG,        N_p("input-name", "Record MNG"),             new input_seq(KEYCODE_F12, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_LCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_RECORD_AVI,        N_p("input-name", "Record AVI"),             new input_seq(KEYCODE_F12, KEYCODE_LSHIFT, KEYCODE_LCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_TOGGLE_CHEAT,      N_p("input-name", "Toggle Cheat"),           new input_seq(KEYCODE_F6) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_UP,                N_p("input-name", "UI Up"),                  new input_seq(KEYCODE_UP, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_DOWN,              N_p("input-name", "UI Down"),                new input_seq(KEYCODE_DOWN, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_LEFT,              N_p("input-name", "UI Left"),                new input_seq(KEYCODE_LEFT, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_RIGHT,             N_p("input-name", "UI Right"),               new input_seq(KEYCODE_RIGHT, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_HOME,              N_p("input-name", "UI Home"),                new input_seq(KEYCODE_HOME) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_END,               N_p("input-name", "UI End"),                 new input_seq(KEYCODE_END) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PAGE_UP,           N_p("input-name", "UI Page Up"),             new input_seq(KEYCODE_PGUP) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PAGE_DOWN,         N_p("input-name", "UI Page Down"),           new input_seq(KEYCODE_PGDN) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FOCUS_NEXT,        N_p("input-name", "UI Focus Next"),          new input_seq(KEYCODE_TAB, input_seq.not_code, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FOCUS_PREV,        N_p("input-name", "UI Focus Previous"),      new input_seq(KEYCODE_TAB, KEYCODE_LSHIFT, input_seq.or_code, KEYCODE_TAB, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SELECT,            N_p("input-name", "UI Select"),              new input_seq(KEYCODE_ENTER, input_seq.not_code, KEYCODE_LALT, input_seq.not_code, KEYCODE_RALT, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(0), input_seq.or_code, KEYCODE_ENTER_PAD) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_CANCEL,            N_p("input-name", "UI Cancel"),              new input_seq(KEYCODE_ESC) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_DISPLAY_COMMENT,   N_p("input-name", "UI Display Comment"),     new input_seq(KEYCODE_SPACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_CLEAR,             N_p("input-name", "UI Clear"),               new input_seq(KEYCODE_DEL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_ZOOM_IN,           N_p("input-name", "UI Zoom In"),             new input_seq(KEYCODE_EQUALS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_ZOOM_OUT,          N_p("input-name", "UI Zoom Out"),            new input_seq(KEYCODE_MINUS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_ZOOM_DEFAULT,      N_p("input-name", "UI Default Zoom"),        new input_seq(KEYCODE_0) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PREV_GROUP,        N_p("input-name", "UI Previous Group"),      new input_seq(KEYCODE_OPENBRACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_NEXT_GROUP,        N_p("input-name", "UI Next Group"),          new input_seq(KEYCODE_CLOSEBRACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_ROTATE,            N_p("input-name", "UI Rotate"),              new input_seq(KEYCODE_R) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SHOW_PROFILER,     N_p("input-name", "Show Profiler"),          new input_seq(KEYCODE_F11, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_TOGGLE_UI,         N_p("input-name", "UI Toggle"),              new input_seq(KEYCODE_SCRLOCK, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_RELEASE_POINTER,   N_p("input-name", "UI Release Pointer"),     new input_seq(KEYCODE_RCONTROL, KEYCODE_RALT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PASTE,             N_p("input-name", "UI Paste Text"),          new input_seq(KEYCODE_SCRLOCK, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SAVE_STATE,        N_p("input-name", "Save State"),             new input_seq(KEYCODE_F7, KEYCODE_LSHIFT, input_seq.or_code, KEYCODE_F7, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_LOAD_STATE,        N_p("input-name", "Load State"),             new input_seq(KEYCODE_F7, input_seq.not_code, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_TAPE_START,        N_p("input-name", "UI (First) Tape Start"),  new input_seq(KEYCODE_F2, input_seq.not_code, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_TAPE_STOP,         N_p("input-name", "UI (First) Tape Stop"),   new input_seq(KEYCODE_F2, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_DATS,              N_p("input-name", "UI External DAT View"),   new input_seq(KEYCODE_LALT, KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FAVORITES,         N_p("input-name", "UI Add/Remove favorite"), new input_seq(KEYCODE_LALT, KEYCODE_F) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_EXPORT,            N_p("input-name", "UI Export List"),         new input_seq(KEYCODE_LALT, KEYCODE_E) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_AUDIT,             N_p("input-name", "UI Audit Media"),         new input_seq(KEYCODE_F1, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_RSHIFT) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_osd(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(osd)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_1,                null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_2,                null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_3,                null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_4,                null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_5,                null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_6,                null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_7,                null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_8,                null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_9,                null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_10,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_11,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_12,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_13,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_14,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_15,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_16,               null,                     new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_invalid(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(invalid)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, UNKNOWN,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, UNUSED,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, SPECIAL,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, OTHER,                null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, ADJUSTER,             null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, DIPSWITCH,            null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, CONFIG,               null,                     new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_digital_type(std.vector<input_type_entry> typelist, ioport_type type, ioport_group group, int player, string token, string name, input_seq seq)
        {
            typelist.emplace_back(new input_type_entry(type, group, (player == 0) ? player : (player - 1), token, name, seq));
        }

        static void emplace_core_analog_type(std.vector<input_type_entry> typelist, ioport_type type, ioport_group group, int player, string token, string name, input_seq seq, input_seq decseq, input_seq incseq)
        {
            typelist.emplace_back(new input_type_entry(type, group, (player == 0) ? player : (player - 1), token, name, seq, decseq, incseq));
        }


        static void INPUT_PORT_DIGITAL_TYPE(std.vector<input_type_entry> typelist, int _player, ioport_group _group, ioport_type _type, string _name, input_seq _seq)
        {
            emplace_core_digital_type(typelist, _type, _group, _player, (_player == 0) ? _type.ToString() : string.Format("P{0}_{1}", _player, _type), _name, _seq);
        }

        static void INPUT_PORT_ANALOG_TYPE(std.vector<input_type_entry> typelist, int _player, ioport_group _group, ioport_type _type, string _name, input_seq _seq, input_seq _decseq, input_seq _incseq)
        {
            emplace_core_analog_type(typelist, _type, _group, _player, (_player == 0) ? _type.ToString() : string.Format("P{0}_{1}", _player, _type), _name, _seq, _decseq, _incseq);
        }

#if false
        // instantiate the contruct functions
        define CORE_INPUT_TYPES_BEGIN(_name) \
                ATTR_COLD inline void emplace_core_types_##_name(std::vector<input_type_entry> &typelist) \
                {
        define INPUT_PORT_DIGITAL_TYPE(_player, _group, _type, _name, _seq) \
                emplace_core_digital_type(typelist, IPT_##_type, IPG_##_group, _player, (_player == 0) ? #_type : ("P" #_player "_" #_type), _name, _seq);
        define INPUT_PORT_ANALOG_TYPE(_player, _group, _type, _name, _seq, _decseq, _incseq) \
                emplace_core_analog_type(typelist, IPT_##_type, IPG_##_group, _player, (_player == 0) ? #_type : ("P" #_player "_" #_type), _name, _seq, _decseq, _incseq);
        define CORE_INPUT_TYPES_END() \
                }
        CORE_INPUT_TYPES_P1
        CORE_INPUT_TYPES_P1_MAHJONG
        CORE_INPUT_TYPES_P1_HANAFUDA
        CORE_INPUT_TYPES_GAMBLE
        CORE_INPUT_TYPES_POKER
        CORE_INPUT_TYPES_SLOT
        CORE_INPUT_TYPES_P2
        CORE_INPUT_TYPES_P2_MAHJONG
        CORE_INPUT_TYPES_P2_HANAFUDA
        CORE_INPUT_TYPES_P3
        CORE_INPUT_TYPES_P3_MAHJONG
        CORE_INPUT_TYPES_P4
        CORE_INPUT_TYPES_P4_MAHJONG
        CORE_INPUT_TYPES_P5
        CORE_INPUT_TYPES_P6
        CORE_INPUT_TYPES_P7
        CORE_INPUT_TYPES_P8
        CORE_INPUT_TYPES_P9
        CORE_INPUT_TYPES_P10
        CORE_INPUT_TYPES_START
        CORE_INPUT_TYPES_COIN
        CORE_INPUT_TYPES_SERVICE
        CORE_INPUT_TYPES_TILT
        CORE_INPUT_TYPES_OTHER
        CORE_INPUT_TYPES_PEDAL
        CORE_INPUT_TYPES_PEDAL2
        CORE_INPUT_TYPES_PEDAL3
        CORE_INPUT_TYPES_PADDLE
        CORE_INPUT_TYPES_PADDLE_V
        CORE_INPUT_TYPES_POSITIONAL
        CORE_INPUT_TYPES_POSITIONAL_V
        CORE_INPUT_TYPES_DIAL
        CORE_INPUT_TYPES_DIAL_V
        CORE_INPUT_TYPES_TRACKBALL_X
        CORE_INPUT_TYPES_TRACKBALL_Y
        CORE_INPUT_TYPES_AD_STICK_X
        CORE_INPUT_TYPES_AD_STICK_Y
        CORE_INPUT_TYPES_AD_STICK_Z
        CORE_INPUT_TYPES_LIGHTGUN_X
        CORE_INPUT_TYPES_LIGHTGUN_Y
        CORE_INPUT_TYPES_MOUSE_X
        CORE_INPUT_TYPES_MOUSE_Y
        CORE_INPUT_TYPES_KEYPAD
        CORE_INPUT_TYPES_UI
        CORE_INPUT_TYPES_OSD
        CORE_INPUT_TYPES_INVALID
        undef CORE_INPUT_TYPES_BEGIN
        undef INPUT_PORT_DIGITAL_TYPE
        undef INPUT_PORT_ANALOG_TYPE
        undef CORE_INPUT_TYPES_END
#endif

#if false
        // make a count function so we don't have to reallocate the vector
        define CORE_INPUT_TYPES_BEGIN(_name)
        define INPUT_PORT_DIGITAL_TYPE(_player, _group, _type, _name, _seq) + 1
        define INPUT_PORT_ANALOG_TYPE(_player, _group, _type, _name, _seq, _decseq, _incseq) + 1
        define CORE_INPUT_TYPES_END()
        constexpr size_t core_input_types_count()
        {
            return 0
                    CORE_INPUT_TYPES_P1
                    CORE_INPUT_TYPES_P1_MAHJONG
                    CORE_INPUT_TYPES_P1_HANAFUDA
                    CORE_INPUT_TYPES_GAMBLE
                    CORE_INPUT_TYPES_POKER
                    CORE_INPUT_TYPES_SLOT
                    CORE_INPUT_TYPES_P2
                    CORE_INPUT_TYPES_P2_MAHJONG
                    CORE_INPUT_TYPES_P2_HANAFUDA
                    CORE_INPUT_TYPES_P3
                    CORE_INPUT_TYPES_P3_MAHJONG
                    CORE_INPUT_TYPES_P4
                    CORE_INPUT_TYPES_P4_MAHJONG
                    CORE_INPUT_TYPES_P5
                    CORE_INPUT_TYPES_P6
                    CORE_INPUT_TYPES_P7
                    CORE_INPUT_TYPES_P8
                    CORE_INPUT_TYPES_P9
                    CORE_INPUT_TYPES_P10
                    CORE_INPUT_TYPES_START
                    CORE_INPUT_TYPES_COIN
                    CORE_INPUT_TYPES_SERVICE
                    CORE_INPUT_TYPES_TILT
                    CORE_INPUT_TYPES_OTHER
                    CORE_INPUT_TYPES_PEDAL
                    CORE_INPUT_TYPES_PEDAL2
                    CORE_INPUT_TYPES_PEDAL3
                    CORE_INPUT_TYPES_PADDLE
                    CORE_INPUT_TYPES_PADDLE_V
                    CORE_INPUT_TYPES_POSITIONAL
                    CORE_INPUT_TYPES_POSITIONAL_V
                    CORE_INPUT_TYPES_DIAL
                    CORE_INPUT_TYPES_DIAL_V
                    CORE_INPUT_TYPES_TRACKBALL_X
                    CORE_INPUT_TYPES_TRACKBALL_Y
                    CORE_INPUT_TYPES_AD_STICK_X
                    CORE_INPUT_TYPES_AD_STICK_Y
                    CORE_INPUT_TYPES_AD_STICK_Z
                    CORE_INPUT_TYPES_LIGHTGUN_X
                    CORE_INPUT_TYPES_LIGHTGUN_Y
                    CORE_INPUT_TYPES_MOUSE_X
                    CORE_INPUT_TYPES_MOUSE_Y
                    CORE_INPUT_TYPES_KEYPAD
                    CORE_INPUT_TYPES_UI
                    CORE_INPUT_TYPES_OSD
                    CORE_INPUT_TYPES_INVALID
                    ;
        }
        #undef CORE_INPUT_TYPES_BEGIN
        #undef INPUT_PORT_DIGITAL_TYPE
        #undef INPUT_PORT_ANALOG_TYPE
        #undef CORE_INPUT_TYPES_END
#endif
        enum CORE_INPUT_TYPES
        {
            CORE_INPUT_TYPES_P1,
            CORE_INPUT_TYPES_P1_MAHJONG,
            CORE_INPUT_TYPES_P1_HANAFUDA,
            CORE_INPUT_TYPES_GAMBLE,
            CORE_INPUT_TYPES_POKER,
            CORE_INPUT_TYPES_SLOT,
            CORE_INPUT_TYPES_P2,
            CORE_INPUT_TYPES_P2_MAHJONG,
            CORE_INPUT_TYPES_P2_HANAFUDA,
            CORE_INPUT_TYPES_P3,
            CORE_INPUT_TYPES_P3_MAHJONG,
            CORE_INPUT_TYPES_P4,
            CORE_INPUT_TYPES_P4_MAHJONG,
            CORE_INPUT_TYPES_P5,
            CORE_INPUT_TYPES_P6,
            CORE_INPUT_TYPES_P7,
            CORE_INPUT_TYPES_P8,
            CORE_INPUT_TYPES_P9,
            CORE_INPUT_TYPES_P10,
            CORE_INPUT_TYPES_START,
            CORE_INPUT_TYPES_COIN,
            CORE_INPUT_TYPES_SERVICE,
            CORE_INPUT_TYPES_TILT,
            CORE_INPUT_TYPES_OTHER,
            CORE_INPUT_TYPES_PEDAL,
            CORE_INPUT_TYPES_PEDAL2,
            CORE_INPUT_TYPES_PEDAL3,
            CORE_INPUT_TYPES_PADDLE,
            CORE_INPUT_TYPES_PADDLE_V,
            CORE_INPUT_TYPES_POSITIONAL,
            CORE_INPUT_TYPES_POSITIONAL_V,
            CORE_INPUT_TYPES_DIAL,
            CORE_INPUT_TYPES_DIAL_V,
            CORE_INPUT_TYPES_TRACKBALL_X,
            CORE_INPUT_TYPES_TRACKBALL_Y,
            CORE_INPUT_TYPES_AD_STICK_X,
            CORE_INPUT_TYPES_AD_STICK_Y,
            CORE_INPUT_TYPES_AD_STICK_Z,
            CORE_INPUT_TYPES_LIGHTGUN_X,
            CORE_INPUT_TYPES_LIGHTGUN_Y,
            CORE_INPUT_TYPES_MOUSE_X,
            CORE_INPUT_TYPES_MOUSE_Y,
            CORE_INPUT_TYPES_KEYPAD,
            CORE_INPUT_TYPES_UI,
            CORE_INPUT_TYPES_OSD,
            CORE_INPUT_TYPES_INVALID,

            CORE_INPUT_TYPES_COUNT
        }

        static UInt32 core_input_types_count()
        {
            return (UInt32)CORE_INPUT_TYPES.CORE_INPUT_TYPES_COUNT;
        }


        public static void emplace_core_types(std.vector<input_type_entry> typelist)
        {
            typelist.reserve(core_input_types_count());

            emplace_core_types_p1(typelist);
            emplace_core_types_p1_mahjong(typelist);
            emplace_core_types_p1_hanafuda(typelist);
            emplace_core_types_gamble(typelist);
            emplace_core_types_poker(typelist);
            emplace_core_types_slot(typelist);
            emplace_core_types_p2(typelist);
            emplace_core_types_p2_mahjong(typelist);
            emplace_core_types_p2_hanafuda(typelist);
            emplace_core_types_p3(typelist);
            emplace_core_types_p3_mahjong(typelist);
            emplace_core_types_p4(typelist);
            emplace_core_types_p4_mahjong(typelist);
            emplace_core_types_p5(typelist);
            emplace_core_types_p6(typelist);
            emplace_core_types_p7(typelist);
            emplace_core_types_p8(typelist);
            emplace_core_types_p9(typelist);
            emplace_core_types_p10(typelist);
            emplace_core_types_start(typelist);
            emplace_core_types_coin(typelist);
            emplace_core_types_service(typelist);
            emplace_core_types_tilt(typelist);
            emplace_core_types_other(typelist);
            emplace_core_types_pedal(typelist);
            emplace_core_types_pedal2(typelist);
            emplace_core_types_pedal3(typelist);
            emplace_core_types_paddle(typelist);
            emplace_core_types_paddle_v(typelist);
            emplace_core_types_positional(typelist);
            emplace_core_types_positional_v(typelist);
            emplace_core_types_dial(typelist);
            emplace_core_types_dial_v(typelist);
            emplace_core_types_trackball_x(typelist);
            emplace_core_types_trackball_y(typelist);
            emplace_core_types_ad_stick_x(typelist);
            emplace_core_types_ad_stick_y(typelist);
            emplace_core_types_ad_stick_z(typelist);
            emplace_core_types_lightgun_x(typelist);
            emplace_core_types_lightgun_y(typelist);
            emplace_core_types_mouse_x(typelist);
            emplace_core_types_mouse_y(typelist);
            emplace_core_types_keypad(typelist);
            emplace_core_types_ui(typelist);
            emplace_core_types_osd(typelist);
            emplace_core_types_invalid(typelist);
        }
    }
}
