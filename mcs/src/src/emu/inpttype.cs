// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


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
        const ioport_type UI_TIMECODE = ioport_type.IPT_UI_TIMECODE;
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
        const ioport_type UI_PREV_GROUP = ioport_type.IPT_UI_PREV_GROUP;
        const ioport_type UI_NEXT_GROUP = ioport_type.IPT_UI_NEXT_GROUP;
        const ioport_type UI_ROTATE = ioport_type.IPT_UI_ROTATE;
        const ioport_type UI_SHOW_PROFILER = ioport_type.IPT_UI_SHOW_PROFILER;
        const ioport_type UI_TOGGLE_UI = ioport_type.IPT_UI_TOGGLE_UI;
        const ioport_type UI_TOGGLE_DEBUG = ioport_type.IPT_UI_TOGGLE_DEBUG;
        const ioport_type UI_PASTE = ioport_type.IPT_UI_PASTE;
        const ioport_type UI_SAVE_STATE = ioport_type.IPT_UI_SAVE_STATE;
        const ioport_type UI_LOAD_STATE = ioport_type.IPT_UI_LOAD_STATE;
        const ioport_type UI_TAPE_START = ioport_type.IPT_UI_TAPE_START;
        const ioport_type UI_TAPE_STOP = ioport_type.IPT_UI_TAPE_STOP;
        const ioport_type UI_DATS = ioport_type.IPT_UI_DATS;
        const ioport_type UI_FAVORITES = ioport_type.IPT_UI_FAVORITES;
        const ioport_type UI_EXPORT = ioport_type.IPT_UI_EXPORT;
        const ioport_type UI_AUDIT_FAST = ioport_type.IPT_UI_AUDIT_FAST;
        const ioport_type UI_AUDIT_ALL = ioport_type.IPT_UI_AUDIT_ALL;
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


        static readonly input_code KEYCODE_A = input_global.KEYCODE_A;
        static readonly input_code KEYCODE_B = input_global.KEYCODE_B;
        static readonly input_code KEYCODE_C = input_global.KEYCODE_C;
        static readonly input_code KEYCODE_D = input_global.KEYCODE_D;
        static readonly input_code KEYCODE_E = input_global.KEYCODE_E;
        static readonly input_code KEYCODE_F = input_global.KEYCODE_F;
        static readonly input_code KEYCODE_G = input_global.KEYCODE_G;
        static readonly input_code KEYCODE_H = input_global.KEYCODE_H;
        static readonly input_code KEYCODE_I = input_global.KEYCODE_I;
        static readonly input_code KEYCODE_J = input_global.KEYCODE_J;
        static readonly input_code KEYCODE_K = input_global.KEYCODE_K;
        static readonly input_code KEYCODE_L = input_global.KEYCODE_L;
        static readonly input_code KEYCODE_M = input_global.KEYCODE_M;
        static readonly input_code KEYCODE_N = input_global.KEYCODE_N;
        static readonly input_code KEYCODE_O = input_global.KEYCODE_O;
        static readonly input_code KEYCODE_P = input_global.KEYCODE_P;
        static readonly input_code KEYCODE_Q = input_global.KEYCODE_Q;
        static readonly input_code KEYCODE_R = input_global.KEYCODE_R;
        static readonly input_code KEYCODE_S = input_global.KEYCODE_S;
        static readonly input_code KEYCODE_T = input_global.KEYCODE_T;
        //static readonly input_code KEYCODE_U = input_global.KEYCODE_U;
        static readonly input_code KEYCODE_V = input_global.KEYCODE_V;
        static readonly input_code KEYCODE_W = input_global.KEYCODE_W;
        static readonly input_code KEYCODE_X = input_global.KEYCODE_X;
        static readonly input_code KEYCODE_Y = input_global.KEYCODE_Y;
        static readonly input_code KEYCODE_Z = input_global.KEYCODE_Z;
        static readonly input_code KEYCODE_0 = input_global.KEYCODE_0;
        static readonly input_code KEYCODE_1 = input_global.KEYCODE_1;
        static readonly input_code KEYCODE_2 = input_global.KEYCODE_2;
        static readonly input_code KEYCODE_3 = input_global.KEYCODE_3;
        static readonly input_code KEYCODE_4 = input_global.KEYCODE_4;
        static readonly input_code KEYCODE_5 = input_global.KEYCODE_5;
        static readonly input_code KEYCODE_6 = input_global.KEYCODE_6;
        static readonly input_code KEYCODE_7 = input_global.KEYCODE_7;
        static readonly input_code KEYCODE_8 = input_global.KEYCODE_8;
        static readonly input_code KEYCODE_9 = input_global.KEYCODE_9;
        static readonly input_code KEYCODE_F1 = input_global.KEYCODE_F1;
        static readonly input_code KEYCODE_F2 = input_global.KEYCODE_F2;
        static readonly input_code KEYCODE_F3 = input_global.KEYCODE_F3;
        static readonly input_code KEYCODE_F4 = input_global.KEYCODE_F4;
        static readonly input_code KEYCODE_F5 = input_global.KEYCODE_F5;
        static readonly input_code KEYCODE_F6 = input_global.KEYCODE_F6;
        static readonly input_code KEYCODE_F7 = input_global.KEYCODE_F7;
        static readonly input_code KEYCODE_F8 = input_global.KEYCODE_F8;
        static readonly input_code KEYCODE_F9 = input_global.KEYCODE_F9;
        static readonly input_code KEYCODE_F10 = input_global.KEYCODE_F10;
        static readonly input_code KEYCODE_F11 = input_global.KEYCODE_F11;
        static readonly input_code KEYCODE_F12 = input_global.KEYCODE_F12;
        //static readonly input_code KEYCODE_F13 = input_global.KEYCODE_F13;
        //static readonly input_code KEYCODE_F14 = input_global.KEYCODE_F14;
        //static readonly input_code KEYCODE_F15 = input_global.KEYCODE_F15;
        //static readonly input_code KEYCODE_F16 = input_global.KEYCODE_F16;
        //static readonly input_code KEYCODE_F17 = input_global.KEYCODE_F17;
        //static readonly input_code KEYCODE_F18 = input_global.KEYCODE_F18;
        //static readonly input_code KEYCODE_F19 = input_global.KEYCODE_F19;
        //static readonly input_code KEYCODE_F20 = input_global.KEYCODE_F20;
        static readonly input_code KEYCODE_ESC = input_global.KEYCODE_ESC;
        static readonly input_code KEYCODE_TILDE = input_global.KEYCODE_TILDE;
        static readonly input_code KEYCODE_MINUS = input_global.KEYCODE_MINUS;
        static readonly input_code KEYCODE_EQUALS = input_global.KEYCODE_EQUALS;
        static readonly input_code KEYCODE_BACKSPACE = input_global.KEYCODE_BACKSPACE;
        static readonly input_code KEYCODE_TAB = input_global.KEYCODE_TAB;
        static readonly input_code KEYCODE_OPENBRACE = input_global.KEYCODE_OPENBRACE;
        static readonly input_code KEYCODE_CLOSEBRACE = input_global.KEYCODE_CLOSEBRACE;
        static readonly input_code KEYCODE_ENTER = input_global.KEYCODE_ENTER;
        static readonly input_code KEYCODE_COLON = input_global.KEYCODE_COLON;

        static readonly input_code KEYCODE_COMMA = input_global.KEYCODE_COMMA;
        static readonly input_code KEYCODE_STOP = input_global.KEYCODE_STOP;
        static readonly input_code KEYCODE_SLASH = input_global.KEYCODE_SLASH;
        static readonly input_code KEYCODE_SPACE = input_global.KEYCODE_SPACE;
        static readonly input_code KEYCODE_INSERT = input_global.KEYCODE_INSERT;
        static readonly input_code KEYCODE_DEL = input_global.KEYCODE_DEL;
        static readonly input_code KEYCODE_HOME = input_global.KEYCODE_HOME;
        static readonly input_code KEYCODE_END = input_global.KEYCODE_END;
        static readonly input_code KEYCODE_PGUP = input_global.KEYCODE_PGUP;
        static readonly input_code KEYCODE_PGDN = input_global.KEYCODE_PGDN;
        static readonly input_code KEYCODE_LEFT = input_global.KEYCODE_LEFT;
        static readonly input_code KEYCODE_RIGHT = input_global.KEYCODE_RIGHT;
        static readonly input_code KEYCODE_UP = input_global.KEYCODE_UP;
        static readonly input_code KEYCODE_DOWN = input_global.KEYCODE_DOWN;
        static readonly input_code KEYCODE_0_PAD = input_global.KEYCODE_0_PAD;
        //static readonly input_code KEYCODE_1_PAD = input_global.KEYCODE_;
        static readonly input_code KEYCODE_2_PAD = input_global.KEYCODE_2_PAD;
        //static readonly input_code KEYCODE_3_PAD = input_global.KEYCODE_;
        static readonly input_code KEYCODE_4_PAD = input_global.KEYCODE_4_PAD;
        //static readonly input_code KEYCODE_5_PAD = input_global.KEYCODE_;
        static readonly input_code KEYCODE_6_PAD = input_global.KEYCODE_6_PAD;
        //static readonly input_code KEYCODE_7_PAD = input_global.KEYCODE_;
        static readonly input_code KEYCODE_8_PAD = input_global.KEYCODE_8_PAD;
        //static readonly input_code KEYCODE_9_PAD = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_SLASH_PAD = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_ASTERISK = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_MINUS_PAD = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_PLUS_PAD = input_global.KEYCODE_;
        static readonly input_code KEYCODE_DEL_PAD = input_global.KEYCODE_DEL_PAD;
        static readonly input_code KEYCODE_ENTER_PAD = input_global.KEYCODE_ENTER_PAD;
        //static readonly input_code KEYCODE_BS_PAD = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_TAB_PAD = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_00_PAD = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_000_PAD = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_COMMA_PAD = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_EQUALS_PAD = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_PRTSCR = input_global.KEYCODE_;
        //static readonly input_code KEYCODE_PAUSE = input_global.KEYCODE_;
        static readonly input_code KEYCODE_LSHIFT = input_global.KEYCODE_LSHIFT;
        static readonly input_code KEYCODE_RSHIFT = input_global.KEYCODE_RSHIFT;
        static readonly input_code KEYCODE_LCONTROL = input_global.KEYCODE_LCONTROL;
        static readonly input_code KEYCODE_RCONTROL = input_global.KEYCODE_RCONTROL;
        static readonly input_code KEYCODE_LALT = input_global.KEYCODE_LALT;
        static readonly input_code KEYCODE_RALT = input_global.KEYCODE_RALT;
        static readonly input_code KEYCODE_SCRLOCK = input_global.KEYCODE_SCRLOCK;

        static input_code MOUSECODE_X_INDEXED(int n) { return input_global.MOUSECODE_X_INDEXED(n); }
        static input_code MOUSECODE_Y_INDEXED(int n) { return input_global.MOUSECODE_Y_INDEXED(n); }
        //static input_code MOUSECODE_Z_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_RELATIVE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_ZAXIS); }

        static input_code MOUSECODE_BUTTON1_INDEXED(int n) { return input_global.MOUSECODE_BUTTON1_INDEXED(n); }
        static input_code MOUSECODE_BUTTON2_INDEXED(int n) { return input_global.MOUSECODE_BUTTON2_INDEXED(n); }
        static input_code MOUSECODE_BUTTON3_INDEXED(int n) { return input_global.MOUSECODE_BUTTON3_INDEXED(n); }
        //static input_code MOUSECODE_BUTTON4_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON4); }
        //static input_code MOUSECODE_BUTTON5_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON5); }
        //static input_code MOUSECODE_BUTTON6_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON6); }
        //static input_code MOUSECODE_BUTTON7_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON7); }
        //static input_code MOUSECODE_BUTTON8_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_MOUSE, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON8); }

        static input_code GUNCODE_X_INDEXED(int n) { return input_global.GUNCODE_X_INDEXED(n); }
        static input_code GUNCODE_Y_INDEXED(int n) { return input_global.GUNCODE_Y_INDEXED(n); }

        static input_code GUNCODE_BUTTON1_INDEXED(int n) { return input_global.GUNCODE_BUTTON1_INDEXED(n); }
        static input_code GUNCODE_BUTTON2_INDEXED(int n) { return input_global.GUNCODE_BUTTON2_INDEXED(n); }
        //static input_code GUNCODE_BUTTON3_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON3); }
        //static input_code GUNCODE_BUTTON4_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON4); }
        //static input_code GUNCODE_BUTTON5_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON5); }
        //static input_code GUNCODE_BUTTON6_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON6); }
        //static input_code GUNCODE_BUTTON7_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON7); }
        //static input_code GUNCODE_BUTTON8_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_LIGHTGUN, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON8); }

        static input_code JOYCODE_X_INDEXED(int n) { return input_global.JOYCODE_X_INDEXED(n); }
        static input_code JOYCODE_Y_INDEXED(int n) { return input_global.JOYCODE_Y_INDEXED(n); }
        static input_code JOYCODE_Z_INDEXED(int n) { return input_global.JOYCODE_Z_INDEXED(n); }
        //static input_code JOYCODE_U_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RXAXIS); }
        //static input_code JOYCODE_V_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RYAXIS); }
        //static input_code JOYCODE_W_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_RZAXIS); }

        //static input_code JOYCODE_X_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_XAXIS); }
        //static input_code JOYCODE_X_NEG_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_XAXIS); }
        //static input_code JOYCODE_Y_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_YAXIS); }
        //static input_code JOYCODE_Y_NEG_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_YAXIS); }
        //static input_code JOYCODE_Z_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_ZAXIS); }
        static input_code JOYCODE_Z_NEG_ABSOLUTE_INDEXED(int n) { return input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(n); }
        //static input_code JOYCODE_U_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_RXAXIS); }
        //static input_code JOYCODE_U_NEG_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_RXAXIS); }
        //static input_code JOYCODE_V_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_RYAXIS); }
        //static input_code JOYCODE_V_NEG_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_NEG, input_item_id.ITEM_ID_RYAXIS); }
        //static input_code JOYCODE_W_POS_ABSOLUTE_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_ABSOLUTE, input_item_modifier.ITEM_MODIFIER_POS, input_item_id.ITEM_ID_RZAXIS); }
        static input_code JOYCODE_W_NEG_ABSOLUTE_INDEXED(int n) { return input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(n); }

        static input_code JOYCODE_X_LEFT_SWITCH_INDEXED(int n) { return input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(n); }
        static input_code JOYCODE_X_RIGHT_SWITCH_INDEXED(int n) { return input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(n); }
        static input_code JOYCODE_Y_UP_SWITCH_INDEXED(int n) { return input_global.JOYCODE_Y_UP_SWITCH_INDEXED(n); }
        static input_code JOYCODE_Y_DOWN_SWITCH_INDEXED(int n) { return input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(n); }
        //static input_code JOYCODE_Z_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS,   input_item_id.ITEM_ID_ZAXIS); }
        //static input_code JOYCODE_Z_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG,   input_item_id.ITEM_ID_ZAXIS); }
        //static input_code JOYCODE_U_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS,   input_item_id.ITEM_ID_RXAXIS); }
        //static input_code JOYCODE_U_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG,   input_item_id.ITEM_ID_RXAXIS); }
        //static input_code JOYCODE_V_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS,   input_item_id.ITEM_ID_RYAXIS); }
        //static input_code JOYCODE_V_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG,   input_item_id.ITEM_ID_RYAXIS); }
        //static input_code JOYCODE_W_POS_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_POS,   input_item_id.ITEM_ID_RZAXIS); }
        //static input_code JOYCODE_W_NEG_SWITCH_INDEXED(int n)   { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NEG,   input_item_id.ITEM_ID_RZAXIS); }

        static input_code JOYCODE_BUTTON1_INDEXED(int n) { return input_global.JOYCODE_BUTTON1_INDEXED(n); }
        static input_code JOYCODE_BUTTON2_INDEXED(int n) { return input_global.JOYCODE_BUTTON2_INDEXED(n); }
        static input_code JOYCODE_BUTTON3_INDEXED(int n) { return input_global.JOYCODE_BUTTON3_INDEXED(n); }
        static input_code JOYCODE_BUTTON4_INDEXED(int n) { return input_global.JOYCODE_BUTTON4_INDEXED(n); }
        static input_code JOYCODE_BUTTON5_INDEXED(int n) { return input_global.JOYCODE_BUTTON5_INDEXED(n); }
        static input_code JOYCODE_BUTTON6_INDEXED(int n) { return input_global.JOYCODE_BUTTON6_INDEXED(n); }
        static input_code JOYCODE_BUTTON7_INDEXED(int n) { return input_global.JOYCODE_BUTTON7_INDEXED(n); }
        static input_code JOYCODE_BUTTON8_INDEXED(int n) { return input_global.JOYCODE_BUTTON8_INDEXED(n); }
        static input_code JOYCODE_BUTTON9_INDEXED(int n) { return input_global.JOYCODE_BUTTON9_INDEXED(n); }
        static input_code JOYCODE_BUTTON10_INDEXED(int n) { return input_global.JOYCODE_BUTTON10_INDEXED(n); }
        static input_code JOYCODE_BUTTON11_INDEXED(int n) { return input_global.JOYCODE_BUTTON11_INDEXED(n); }
        static input_code JOYCODE_BUTTON12_INDEXED(int n) { return input_global.JOYCODE_BUTTON12_INDEXED(n); }
        static input_code JOYCODE_BUTTON13_INDEXED(int n) { return input_global.JOYCODE_BUTTON13_INDEXED(n); }
        static input_code JOYCODE_BUTTON14_INDEXED(int n) { return input_global.JOYCODE_BUTTON14_INDEXED(n); }
        static input_code JOYCODE_BUTTON15_INDEXED(int n) { return input_global.JOYCODE_BUTTON15_INDEXED(n); }
        static input_code JOYCODE_BUTTON16_INDEXED(int n) { return input_global.JOYCODE_BUTTON16_INDEXED(n); }
        //static input_code JOYCODE_BUTTON17_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON17); }
        //static input_code JOYCODE_BUTTON18_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON18); }
        //static input_code JOYCODE_BUTTON19_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON19); }
        //static input_code JOYCODE_BUTTON20_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON20); }
        //static input_code JOYCODE_BUTTON21_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON21); }
        //static input_code JOYCODE_BUTTON22_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON22); }
        //static input_code JOYCODE_BUTTON23_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON23); }
        //static input_code JOYCODE_BUTTON24_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON24); }
        //static input_code JOYCODE_BUTTON25_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON25); }
        //static input_code JOYCODE_BUTTON26_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON26); }
        //static input_code JOYCODE_BUTTON27_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON27); }
        //static input_code JOYCODE_BUTTON28_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON28); }
        //static input_code JOYCODE_BUTTON29_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON29); }
        //static input_code JOYCODE_BUTTON30_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON30); }
        //static input_code JOYCODE_BUTTON31_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON31); }
        //static input_code JOYCODE_BUTTON32_INDEXED(int n) { return new input_code(input_device_class.DEVICE_CLASS_JOYSTICK, n, input_item_class.ITEM_CLASS_SWITCH, input_item_modifier.ITEM_MODIFIER_NONE, input_item_id.ITEM_ID_BUTTON32); }
        static input_code JOYCODE_START_INDEXED(int n) { return input_global.JOYCODE_START_INDEXED(n); }
        static input_code JOYCODE_SELECT_INDEXED(int n) { return input_global.JOYCODE_SELECT_INDEXED(n); }


        /***************************************************************************
            BUILT-IN CORE MAPPINGS
        ***************************************************************************/

        static void emplace_core_types_p1(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p1)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICK_UP,         "P1 Up",                  new input_seq(KEYCODE_UP, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICK_DOWN,       "P1 Down",                new input_seq(KEYCODE_DOWN, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICK_LEFT,       "P1 Left",                new input_seq(KEYCODE_LEFT, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICK_RIGHT,      "P1 Right",               new input_seq(KEYCODE_RIGHT, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKRIGHT_UP,    "P1 Right Stick/Up",      new input_seq(KEYCODE_I, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKRIGHT_DOWN,  "P1 Right Stick/Down",    new input_seq(KEYCODE_K, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKRIGHT_LEFT,  "P1 Right Stick/Left",    new input_seq(KEYCODE_J, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKRIGHT_RIGHT, "P1 Right Stick/Right",   new input_seq(KEYCODE_L, input_seq.or_code, JOYCODE_BUTTON4_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKLEFT_UP,     "P1 Left Stick/Up",       new input_seq(KEYCODE_E, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKLEFT_DOWN,   "P1 Left Stick/Down",     new input_seq(KEYCODE_D, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKLEFT_LEFT,   "P1 Left Stick/Left",     new input_seq(KEYCODE_S, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, JOYSTICKLEFT_RIGHT,  "P1 Left Stick/Right",    new input_seq(KEYCODE_F, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON1,             "P1 Button 1",            new input_seq(KEYCODE_LCONTROL, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(0), input_seq.or_code, MOUSECODE_BUTTON1_INDEXED(0), input_seq.or_code, GUNCODE_BUTTON1_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON2,             "P1 Button 2",            new input_seq(KEYCODE_LALT, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(0), input_seq.or_code, MOUSECODE_BUTTON3_INDEXED(0), input_seq.or_code, GUNCODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON3,             "P1 Button 3",            new input_seq(KEYCODE_SPACE, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(0), input_seq.or_code, MOUSECODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON4,             "P1 Button 4",            new input_seq(KEYCODE_LSHIFT, input_seq.or_code, JOYCODE_BUTTON4_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON5,             "P1 Button 5",            new input_seq(KEYCODE_Z, input_seq.or_code, JOYCODE_BUTTON5_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON6,             "P1 Button 6",            new input_seq(KEYCODE_X, input_seq.or_code, JOYCODE_BUTTON6_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON7,             "P1 Button 7",            new input_seq(KEYCODE_C, input_seq.or_code, JOYCODE_BUTTON7_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON8,             "P1 Button 8",            new input_seq(KEYCODE_V, input_seq.or_code, JOYCODE_BUTTON8_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON9,             "P1 Button 9",            new input_seq(KEYCODE_B, input_seq.or_code, JOYCODE_BUTTON9_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON10,            "P1 Button 10",           new input_seq(KEYCODE_N, input_seq.or_code, JOYCODE_BUTTON10_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON11,            "P1 Button 11",           new input_seq(KEYCODE_M, input_seq.or_code, JOYCODE_BUTTON11_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON12,            "P1 Button 12",           new input_seq(KEYCODE_COMMA, input_seq.or_code, JOYCODE_BUTTON12_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON13,            "P1 Button 13",           new input_seq(KEYCODE_STOP, input_seq.or_code, JOYCODE_BUTTON13_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON14,            "P1 Button 14",           new input_seq(KEYCODE_SLASH, input_seq.or_code, JOYCODE_BUTTON14_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON15,            "P1 Button 15",           new input_seq(KEYCODE_RSHIFT, input_seq.or_code, JOYCODE_BUTTON15_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, BUTTON16,            "P1 Button 16",           new input_seq(JOYCODE_BUTTON16_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, START,               "P1 Start",               new input_seq(KEYCODE_1, input_seq.or_code, JOYCODE_START_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SELECT,              "P1 Select",              new input_seq(KEYCODE_5, input_seq.or_code, JOYCODE_SELECT_INDEXED(0)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p1_mahjong(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p1_mahjong)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_A,           "P1 Mahjong A",           new input_seq(KEYCODE_A) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_B,           "P1 Mahjong B",           new input_seq(KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_C,           "P1 Mahjong C",           new input_seq(KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_D,           "P1 Mahjong D",           new input_seq(KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_E,           "P1 Mahjong E",           new input_seq(KEYCODE_E) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_F,           "P1 Mahjong F",           new input_seq(KEYCODE_F) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_G,           "P1 Mahjong G",           new input_seq(KEYCODE_G) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_H,           "P1 Mahjong H",           new input_seq(KEYCODE_H) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_I,           "P1 Mahjong I",           new input_seq(KEYCODE_I) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_J,           "P1 Mahjong J",           new input_seq(KEYCODE_J) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_K,           "P1 Mahjong K",           new input_seq(KEYCODE_K) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_L,           "P1 Mahjong L",           new input_seq(KEYCODE_L) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_M,           "P1 Mahjong M",           new input_seq(KEYCODE_M) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_N,           "P1 Mahjong N",           new input_seq(KEYCODE_N) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_O,           "P1 Mahjong O",           new input_seq(KEYCODE_O) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_P,           "P1 Mahjong P",           new input_seq(KEYCODE_COLON) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_Q,           "P1 Mahjong Q",           new input_seq(KEYCODE_Q) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_KAN,         "P1 Mahjong Kan",         new input_seq(KEYCODE_LCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_PON,         "P1 Mahjong Pon",         new input_seq(KEYCODE_LALT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_CHI,         "P1 Mahjong Chi",         new input_seq(KEYCODE_SPACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_REACH,       "P1 Mahjong Reach",       new input_seq(KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_RON,         "P1 Mahjong Ron",         new input_seq(KEYCODE_Z) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_FLIP_FLOP,   "P1 Mahjong Flip Flop",   new input_seq(KEYCODE_Y) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_BET,         "P1 Mahjong Bet",         new input_seq(KEYCODE_3) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_SCORE,       "P1 Mahjong Take Score",       new input_seq(KEYCODE_RCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_DOUBLE_UP,   "P1 Mahjong Double Up",   new input_seq(KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_BIG,         "P1 Mahjong Big",         new input_seq(KEYCODE_ENTER) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_SMALL,       "P1 Mahjong Small",       new input_seq(KEYCODE_BACKSPACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, MAHJONG_LAST_CHANCE, "P1 Mahjong Last Chance", new input_seq(KEYCODE_RALT) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p1_hanafuda(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p1_hanafuda)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_A,          "P1 Hanafuda A/1",        new input_seq(KEYCODE_A) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_B,          "P1 Hanafuda B/2",        new input_seq(KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_C,          "P1 Hanafuda C/3",        new input_seq(KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_D,          "P1 Hanafuda D/4",        new input_seq(KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_E,          "P1 Hanafuda E/5",        new input_seq(KEYCODE_E) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_F,          "P1 Hanafuda F/6",        new input_seq(KEYCODE_F) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_G,          "P1 Hanafuda G/7",        new input_seq(KEYCODE_G) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_H,          "P1 Hanafuda H/8",        new input_seq(KEYCODE_H) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_YES,        "P1 Hanafuda Yes",        new input_seq(KEYCODE_M) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, HANAFUDA_NO,         "P1 Hanafuda No",         new input_seq(KEYCODE_N) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_gamble(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(gamble)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_KEYIN,        "Key In",                 new input_seq(KEYCODE_Q) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_KEYOUT,       "Key Out",                new input_seq(KEYCODE_W) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_SERVICE,      "Service",                new input_seq(KEYCODE_9) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_BOOK,         "Book-Keeping",           new input_seq(KEYCODE_0) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_DOOR,         "Door",                   new input_seq(KEYCODE_O) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_PAYOUT,       "Payout",                 new input_seq(KEYCODE_I) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_BET,          "Bet",                    new input_seq(KEYCODE_M) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_DEAL,         "Deal",                   new input_seq(KEYCODE_2) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_STAND,        "Stand",                  new input_seq(KEYCODE_L) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_TAKE,         "Take Score",             new input_seq(KEYCODE_4) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_D_UP,         "Double Up",              new input_seq(KEYCODE_3) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_HALF,         "Half Gamble",            new input_seq(KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_HIGH,         "High",                   new input_seq(KEYCODE_A) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, GAMBLE_LOW,          "Low",                    new input_seq(KEYCODE_S) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_poker(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(poker)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_HOLD1,         "Hold 1",                 new input_seq(KEYCODE_Z) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_HOLD2,         "Hold 2",                 new input_seq(KEYCODE_X) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_HOLD3,         "Hold 3",                 new input_seq(KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_HOLD4,         "Hold 4",                 new input_seq(KEYCODE_V) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_HOLD5,         "Hold 5",                 new input_seq(KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, POKER_CANCEL,        "Cancel",                 new input_seq(KEYCODE_N) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_slot(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(slot)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SLOT_STOP1,          "Stop Reel 1",            new input_seq(KEYCODE_X) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SLOT_STOP2,          "Stop Reel 2",            new input_seq(KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SLOT_STOP3,          "Stop Reel 3",            new input_seq(KEYCODE_V) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SLOT_STOP4,          "Stop Reel 4",            new input_seq(KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, PLAYER1, SLOT_STOP_ALL,       "Stop All Reels",         new input_seq(KEYCODE_Z) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p2(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p2)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICK_UP,         "P2 Up",                  new input_seq(KEYCODE_R, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICK_DOWN,       "P2 Down",                new input_seq(KEYCODE_F, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICK_LEFT,       "P2 Left",                new input_seq(KEYCODE_D, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICK_RIGHT,      "P2 Right",               new input_seq(KEYCODE_G, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKRIGHT_UP,    "P2 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKRIGHT_DOWN,  "P2 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKRIGHT_LEFT,  "P2 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKRIGHT_RIGHT, "P2 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKLEFT_UP,     "P2 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKLEFT_DOWN,   "P2 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKLEFT_LEFT,   "P2 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, JOYSTICKLEFT_RIGHT,  "P2 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON1,             "P2 Button 1",            new input_seq(KEYCODE_A, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(1), input_seq.or_code, MOUSECODE_BUTTON1_INDEXED(1), input_seq.or_code, GUNCODE_BUTTON1_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON2,             "P2 Button 2",            new input_seq(KEYCODE_S, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(1), input_seq.or_code, MOUSECODE_BUTTON3_INDEXED(1), input_seq.or_code, GUNCODE_BUTTON2_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON3,             "P2 Button 3",            new input_seq(KEYCODE_Q, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(1), input_seq.or_code, MOUSECODE_BUTTON2_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON4,             "P2 Button 4",            new input_seq(KEYCODE_W, input_seq.or_code, JOYCODE_BUTTON4_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON5,             "P2 Button 5",            new input_seq(KEYCODE_E, input_seq.or_code, JOYCODE_BUTTON5_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON6,             "P2 Button 6",            new input_seq(JOYCODE_BUTTON6_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON7,             "P2 Button 7",            new input_seq(JOYCODE_BUTTON7_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON8,             "P2 Button 8",            new input_seq(JOYCODE_BUTTON8_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON9,             "P2 Button 9",            new input_seq(JOYCODE_BUTTON9_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON10,            "P2 Button 10",           new input_seq(JOYCODE_BUTTON10_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON11,            "P2 Button 11",           new input_seq(JOYCODE_BUTTON11_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON12,            "P2 Button 12",           new input_seq(JOYCODE_BUTTON12_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON13,            "P2 Button 13",           new input_seq(JOYCODE_BUTTON13_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON14,            "P2 Button 14",           new input_seq(JOYCODE_BUTTON14_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON15,            "P2 Button 15",           new input_seq(JOYCODE_BUTTON15_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, BUTTON16,            "P2 Button 16",           new input_seq(JOYCODE_BUTTON16_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, START,               "P2 Start",               new input_seq(KEYCODE_2, input_seq.or_code, JOYCODE_START_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, SELECT,              "P2 Select",              new input_seq(KEYCODE_6, input_seq.or_code, JOYCODE_SELECT_INDEXED(1)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p2_mahjong(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p2_mahjong)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_A,           "P2 Mahjong A",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_B,           "P2 Mahjong B",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_C,           "P2 Mahjong C",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_D,           "P2 Mahjong D",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_E,           "P2 Mahjong E",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_F,           "P2 Mahjong F",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_G,           "P2 Mahjong G",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_H,           "P2 Mahjong H",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_I,           "P2 Mahjong I",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_J,           "P2 Mahjong J",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_K,           "P2 Mahjong K",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_L,           "P2 Mahjong L",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_M,           "P2 Mahjong M",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_N,           "P2 Mahjong N",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_O,           "P2 Mahjong O",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_P,           "P2 Mahjong P",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_Q,           "P2 Mahjong Q",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_KAN,         "P2 Mahjong Kan",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_PON,         "P2 Mahjong Pon",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_CHI,         "P2 Mahjong Chi",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_REACH,       "P2 Mahjong Reach",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_RON,         "P2 Mahjong Ron",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_BET,         "P2 Mahjong Bet",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_LAST_CHANCE, "P2 Mahjong Last Chance", new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_SCORE,       "P2 Mahjong Take Score",  new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_DOUBLE_UP,   "P2 Mahjong Double Up",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_FLIP_FLOP,   "P2 Mahjong Flip Flop",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_BIG,         "P2 Mahjong Big",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, MAHJONG_SMALL,       "P2 Mahjong Small",       new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p2_hanafuda(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p2_hanafuda)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_A,          "P2 Hanafuda A/1",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_B,          "P2 Hanafuda B/2",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_C,          "P2 Hanafuda C/3",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_D,          "P2 Hanafuda D/4",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_E,          "P2 Hanafuda E/5",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_F,          "P2 Hanafuda F/6",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_G,          "P2 Hanafuda G/7",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_H,          "P2 Hanafuda H/8",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_YES,        "P2 Hanafuda Yes",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, PLAYER2, HANAFUDA_NO,         "P2 Hanafuda No",         new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p3(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p3)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICK_UP,         "P3 Up",                  new input_seq(KEYCODE_I, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICK_DOWN,       "P3 Down",                new input_seq(KEYCODE_K, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICK_LEFT,       "P3 Left",                new input_seq(KEYCODE_J, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICK_RIGHT,      "P3 Right",               new input_seq(KEYCODE_L, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKRIGHT_UP,    "P3 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKRIGHT_DOWN,  "P3 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKRIGHT_LEFT,  "P3 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKRIGHT_RIGHT, "P3 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKLEFT_UP,     "P3 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKLEFT_DOWN,   "P3 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKLEFT_LEFT,   "P3 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, JOYSTICKLEFT_RIGHT,  "P3 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON1,             "P3 Button 1",            new input_seq(KEYCODE_RCONTROL, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(2), input_seq.or_code, GUNCODE_BUTTON1_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON2,             "P3 Button 2",            new input_seq(KEYCODE_RSHIFT, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(2), input_seq.or_code, GUNCODE_BUTTON2_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON3,             "P3 Button 3",            new input_seq(KEYCODE_ENTER, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON4,             "P3 Button 4",            new input_seq(JOYCODE_BUTTON4_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON5,             "P3 Button 5",            new input_seq(JOYCODE_BUTTON5_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON6,             "P3 Button 6",            new input_seq(JOYCODE_BUTTON6_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON7,             "P3 Button 7",            new input_seq(JOYCODE_BUTTON7_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON8,             "P3 Button 8",            new input_seq(JOYCODE_BUTTON8_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON9,             "P3 Button 9",            new input_seq(JOYCODE_BUTTON9_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON10,            "P3 Button 10",           new input_seq(JOYCODE_BUTTON10_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON11,            "P3 Button 11",           new input_seq(JOYCODE_BUTTON11_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON12,            "P3 Button 12",           new input_seq(JOYCODE_BUTTON12_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON13,            "P3 Button 13",           new input_seq(JOYCODE_BUTTON13_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON14,            "P3 Button 14",           new input_seq(JOYCODE_BUTTON14_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON15,            "P3 Button 15",           new input_seq(JOYCODE_BUTTON15_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, BUTTON16,            "P3 Button 16",           new input_seq(JOYCODE_BUTTON16_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, START,               "P3 Start",               new input_seq(KEYCODE_3, input_seq.or_code, JOYCODE_START_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, PLAYER3, SELECT,              "P3 Select",              new input_seq(KEYCODE_7, input_seq.or_code, JOYCODE_SELECT_INDEXED(2)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p4(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p4)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICK_UP,         "P4 Up",                  new input_seq(KEYCODE_8_PAD, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICK_DOWN,       "P4 Down",                new input_seq(KEYCODE_2_PAD, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICK_LEFT,       "P4 Left",                new input_seq(KEYCODE_4_PAD, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICK_RIGHT,      "P4 Right",               new input_seq(KEYCODE_6_PAD, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKRIGHT_UP,    "P4 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKRIGHT_DOWN,  "P4 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKRIGHT_LEFT,  "P4 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKRIGHT_RIGHT, "P4 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKLEFT_UP,     "P4 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKLEFT_DOWN,   "P4 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKLEFT_LEFT,   "P4 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, JOYSTICKLEFT_RIGHT,  "P4 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON1,             "P4 Button 1",            new input_seq(KEYCODE_0_PAD, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON2,             "P4 Button 2",            new input_seq(KEYCODE_DEL_PAD, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON3,             "P4 Button 3",            new input_seq(KEYCODE_ENTER_PAD, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON4,             "P4 Button 4",            new input_seq(JOYCODE_BUTTON4_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON5,             "P4 Button 5",            new input_seq(JOYCODE_BUTTON5_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON6,             "P4 Button 6",            new input_seq(JOYCODE_BUTTON6_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON7,             "P4 Button 7",            new input_seq(JOYCODE_BUTTON7_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON8,             "P4 Button 8",            new input_seq(JOYCODE_BUTTON8_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON9,             "P4 Button 9",            new input_seq(JOYCODE_BUTTON9_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON10,            "P4 Button 10",           new input_seq(JOYCODE_BUTTON10_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON11,            "P4 Button 11",           new input_seq(JOYCODE_BUTTON11_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON12,            "P4 Button 12",           new input_seq(JOYCODE_BUTTON12_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON13,            "P4 Button 13",           new input_seq(JOYCODE_BUTTON13_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON14,            "P4 Button 14",           new input_seq(JOYCODE_BUTTON14_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON15,            "P4 Button 15",           new input_seq(JOYCODE_BUTTON15_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, BUTTON16,            "P4 Button 16",           new input_seq(JOYCODE_BUTTON16_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, START,               "P4 Start",               new input_seq(KEYCODE_4, input_seq.or_code, JOYCODE_START_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, PLAYER4, SELECT,              "P4 Select",              new input_seq(KEYCODE_8, input_seq.or_code, JOYCODE_SELECT_INDEXED(3)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p5(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p5)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICK_UP,         "P5 Up",                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICK_DOWN,       "P5 Down",                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICK_LEFT,       "P5 Left",                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICK_RIGHT,      "P5 Right",               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKRIGHT_UP,    "P5 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKRIGHT_DOWN,  "P5 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKRIGHT_LEFT,  "P5 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKRIGHT_RIGHT, "P5 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKLEFT_UP,     "P5 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKLEFT_DOWN,   "P5 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKLEFT_LEFT,   "P5 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, JOYSTICKLEFT_RIGHT,  "P5 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON2,             "P5 Button 2",            new input_seq(JOYCODE_BUTTON2_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON3,             "P5 Button 3",            new input_seq(JOYCODE_BUTTON3_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON4,             "P5 Button 4",            new input_seq(JOYCODE_BUTTON4_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON5,             "P5 Button 5",            new input_seq(JOYCODE_BUTTON5_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON6,             "P5 Button 6",            new input_seq(JOYCODE_BUTTON6_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON7,             "P5 Button 7",            new input_seq(JOYCODE_BUTTON7_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON1,             "P5 Button 1",            new input_seq(JOYCODE_BUTTON1_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON8,             "P5 Button 8",            new input_seq(JOYCODE_BUTTON8_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON9,             "P5 Button 9",            new input_seq(JOYCODE_BUTTON9_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON10,            "P5 Button 10",           new input_seq(JOYCODE_BUTTON10_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON11,            "P5 Button 11",           new input_seq(JOYCODE_BUTTON11_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON12,            "P5 Button 12",           new input_seq(JOYCODE_BUTTON12_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON13,            "P5 Button 13",           new input_seq(JOYCODE_BUTTON13_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON14,            "P5 Button 14",           new input_seq(JOYCODE_BUTTON14_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON15,            "P5 Button 15",           new input_seq(JOYCODE_BUTTON15_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, BUTTON16,            "P5 Button 16",           new input_seq(JOYCODE_BUTTON16_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, START,               "P5 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, PLAYER5, SELECT,              "P5 Select",              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p6(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p6)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICK_UP,         "P6 Up",                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICK_DOWN,       "P6 Down",                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICK_LEFT,       "P6 Left",                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICK_RIGHT,      "P6 Right",               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKRIGHT_UP,    "P6 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKRIGHT_DOWN,  "P6 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKRIGHT_LEFT,  "P6 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKRIGHT_RIGHT, "P6 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKLEFT_UP,     "P6 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKLEFT_DOWN,   "P6 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKLEFT_LEFT,   "P6 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, JOYSTICKLEFT_RIGHT,  "P6 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON1,             "P6 Button 1",            new input_seq(JOYCODE_BUTTON1_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON2,             "P6 Button 2",            new input_seq(JOYCODE_BUTTON2_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON3,             "P6 Button 3",            new input_seq(JOYCODE_BUTTON3_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON4,             "P6 Button 4",            new input_seq(JOYCODE_BUTTON4_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON5,             "P6 Button 5",            new input_seq(JOYCODE_BUTTON5_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON6,             "P6 Button 6",            new input_seq(JOYCODE_BUTTON6_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON7,             "P6 Button 7",            new input_seq(JOYCODE_BUTTON7_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON8,             "P6 Button 8",            new input_seq(JOYCODE_BUTTON8_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON9,             "P6 Button 9",            new input_seq(JOYCODE_BUTTON9_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON10,            "P6 Button 10",           new input_seq(JOYCODE_BUTTON10_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON11,            "P6 Button 11",           new input_seq(JOYCODE_BUTTON11_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON12,            "P6 Button 12",           new input_seq(JOYCODE_BUTTON12_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON13,            "P6 Button 13",           new input_seq(JOYCODE_BUTTON13_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON14,            "P6 Button 14",           new input_seq(JOYCODE_BUTTON14_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON15,            "P6 Button 15",           new input_seq(JOYCODE_BUTTON15_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, BUTTON16,            "P6 Button 16",           new input_seq(JOYCODE_BUTTON16_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, START,               "P6 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, PLAYER6, SELECT,              "P6 Select",              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p7(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p7)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICK_UP,         "P7 Up",                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICK_DOWN,       "P7 Down",                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICK_LEFT,       "P7 Left",                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICK_RIGHT,      "P7 Right",               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKRIGHT_UP,    "P7 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKRIGHT_DOWN,  "P7 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKRIGHT_LEFT,  "P7 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKRIGHT_RIGHT, "P7 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKLEFT_UP,     "P7 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKLEFT_DOWN,   "P7 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKLEFT_LEFT,   "P7 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, JOYSTICKLEFT_RIGHT,  "P7 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON1,             "P7 Button 1",            new input_seq(JOYCODE_BUTTON1_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON2,             "P7 Button 2",            new input_seq(JOYCODE_BUTTON2_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON3,             "P7 Button 3",            new input_seq(JOYCODE_BUTTON3_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON4,             "P7 Button 4",            new input_seq(JOYCODE_BUTTON4_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON5,             "P7 Button 5",            new input_seq(JOYCODE_BUTTON5_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON6,             "P7 Button 6",            new input_seq(JOYCODE_BUTTON6_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON7,             "P7 Button 7",            new input_seq(JOYCODE_BUTTON7_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON8,             "P7 Button 8",            new input_seq(JOYCODE_BUTTON8_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON9,             "P7 Button 9",            new input_seq(JOYCODE_BUTTON9_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON10,            "P7 Button 10",           new input_seq(JOYCODE_BUTTON10_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON11,            "P7 Button 11",           new input_seq(JOYCODE_BUTTON11_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON12,            "P7 Button 12",           new input_seq(JOYCODE_BUTTON12_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON13,            "P7 Button 13",           new input_seq(JOYCODE_BUTTON13_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON14,            "P7 Button 14",           new input_seq(JOYCODE_BUTTON14_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON15,            "P7 Button 15",           new input_seq(JOYCODE_BUTTON15_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, BUTTON16,            "P7 Button 16",           new input_seq(JOYCODE_BUTTON16_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, START,               "P7 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, PLAYER7, SELECT,              "P7 Select",              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p8(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p8)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICK_UP,         "P8 Up",                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICK_DOWN,       "P8 Down",                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICK_LEFT,       "P8 Left",                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICK_RIGHT,      "P8 Right",               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKRIGHT_UP,    "P8 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKRIGHT_DOWN,  "P8 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKRIGHT_LEFT,  "P8 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKRIGHT_RIGHT, "P8 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKLEFT_UP,     "P8 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKLEFT_DOWN,   "P8 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKLEFT_LEFT,   "P8 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, JOYSTICKLEFT_RIGHT,  "P8 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON1,             "P8 Button 1",            new input_seq(JOYCODE_BUTTON1_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON2,             "P8 Button 2",            new input_seq(JOYCODE_BUTTON2_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON3,             "P8 Button 3",            new input_seq(JOYCODE_BUTTON3_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON4,             "P8 Button 4",            new input_seq(JOYCODE_BUTTON4_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON5,             "P8 Button 5",            new input_seq(JOYCODE_BUTTON5_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON6,             "P8 Button 6",            new input_seq(JOYCODE_BUTTON6_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON7,             "P8 Button 7",            new input_seq(JOYCODE_BUTTON7_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON8,             "P8 Button 8",            new input_seq(JOYCODE_BUTTON8_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON9,             "P8 Button 9",            new input_seq(JOYCODE_BUTTON9_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON10,            "P8 Button 10",           new input_seq(JOYCODE_BUTTON10_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON11,            "P8 Button 11",           new input_seq(JOYCODE_BUTTON11_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON12,            "P8 Button 12",           new input_seq(JOYCODE_BUTTON12_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON13,            "P8 Button 13",           new input_seq(JOYCODE_BUTTON13_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON14,            "P8 Button 14",           new input_seq(JOYCODE_BUTTON14_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON15,            "P8 Button 15",           new input_seq(JOYCODE_BUTTON15_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, BUTTON16,            "P8 Button 16",           new input_seq(JOYCODE_BUTTON16_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, START,               "P8 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, PLAYER8, SELECT,              "P8 Select",              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p9(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p9)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICK_UP,         "P9 Up",                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICK_DOWN,       "P9 Down",                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICK_LEFT,       "P9 Left",                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICK_RIGHT,      "P9 Right",               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKRIGHT_UP,    "P9 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKRIGHT_DOWN,  "P9 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKRIGHT_LEFT,  "P9 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKRIGHT_RIGHT, "P9 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKLEFT_UP,     "P9 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKLEFT_DOWN,   "P9 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKLEFT_LEFT,   "P9 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, JOYSTICKLEFT_RIGHT,  "P9 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON1,             "P9 Button 1",            new input_seq(JOYCODE_BUTTON1_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON2,             "P9 Button 2",            new input_seq(JOYCODE_BUTTON2_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON3,             "P9 Button 3",            new input_seq(JOYCODE_BUTTON3_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON4,             "P9 Button 4",            new input_seq(JOYCODE_BUTTON4_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON5,             "P9 Button 5",            new input_seq(JOYCODE_BUTTON5_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON6,             "P9 Button 6",            new input_seq(JOYCODE_BUTTON6_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON7,             "P9 Button 7",            new input_seq(JOYCODE_BUTTON7_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON8,             "P9 Button 8",            new input_seq(JOYCODE_BUTTON8_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON9,             "P9 Button 9",            new input_seq(JOYCODE_BUTTON9_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON10,            "P9 Button 10",           new input_seq(JOYCODE_BUTTON10_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON11,            "P9 Button 11",           new input_seq(JOYCODE_BUTTON11_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON12,            "P9 Button 12",           new input_seq(JOYCODE_BUTTON12_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON13,            "P9 Button 13",           new input_seq(JOYCODE_BUTTON13_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON14,            "P9 Button 14",           new input_seq(JOYCODE_BUTTON14_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON15,            "P9 Button 15",           new input_seq(JOYCODE_BUTTON15_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, BUTTON16,            "P9 Button 16",           new input_seq(JOYCODE_BUTTON16_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, START,               "P9 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, PLAYER9, SELECT,              "P9 Select",              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_p10(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(p10)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICK_UP,         "P10 Up",                  new input_seq(JOYCODE_Y_UP_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICK_DOWN,       "P10 Down",                new input_seq(JOYCODE_Y_DOWN_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICK_LEFT,       "P10 Left",                new input_seq(JOYCODE_X_LEFT_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICK_RIGHT,      "P10 Right",               new input_seq(JOYCODE_X_RIGHT_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKRIGHT_UP,    "P10 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKRIGHT_DOWN,  "P10 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKRIGHT_LEFT,  "P10 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKRIGHT_RIGHT, "P10 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKLEFT_UP,     "P10 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKLEFT_DOWN,   "P10 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKLEFT_LEFT,   "P10 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, JOYSTICKLEFT_RIGHT,  "P10 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON1,             "P10 Button 1",            new input_seq(JOYCODE_BUTTON1_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON2,             "P10 Button 2",            new input_seq(JOYCODE_BUTTON2_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON3,             "P10 Button 3",            new input_seq(JOYCODE_BUTTON3_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON4,             "P10 Button 4",            new input_seq(JOYCODE_BUTTON4_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON5,             "P10 Button 5",            new input_seq(JOYCODE_BUTTON5_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON6,             "P10 Button 6",            new input_seq(JOYCODE_BUTTON6_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON7,             "P10 Button 7",            new input_seq(JOYCODE_BUTTON7_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON8,             "P10 Button 8",            new input_seq(JOYCODE_BUTTON8_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON9,             "P10 Button 9",            new input_seq(JOYCODE_BUTTON9_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON10,            "P10 Button 10",           new input_seq(JOYCODE_BUTTON10_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON11,            "P10 Button 11",           new input_seq(JOYCODE_BUTTON11_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON12,            "P10 Button 12",           new input_seq(JOYCODE_BUTTON12_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON13,            "P10 Button 13",           new input_seq(JOYCODE_BUTTON13_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON14,            "P10 Button 14",           new input_seq(JOYCODE_BUTTON14_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON15,            "P10 Button 15",           new input_seq(JOYCODE_BUTTON15_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, BUTTON16,            "P10 Button 16",           new input_seq(JOYCODE_BUTTON16_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, START,               "P10 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, PLAYER10, SELECT,              "P10 Select",              new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_start(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(start)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START1,              "1 Player Start",         new input_seq(KEYCODE_1, input_seq.or_code, JOYCODE_START_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START2,              "2 Players Start",        new input_seq(KEYCODE_2, input_seq.or_code, JOYCODE_START_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START3,              "3 Players Start",        new input_seq(KEYCODE_3, input_seq.or_code, JOYCODE_START_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START4,              "4 Players Start",        new input_seq(KEYCODE_4, input_seq.or_code, JOYCODE_START_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START5,              "5 Players Start",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START6,              "6 Players Start",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START7,              "7 Players Start",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   START8,              "8 Players Start",        new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_coin(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(coin)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN3,               "Coin 3",                 new input_seq(KEYCODE_7, input_seq.or_code, JOYCODE_SELECT_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN1,               "Coin 1",                 new input_seq(KEYCODE_5, input_seq.or_code, JOYCODE_SELECT_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN4,               "Coin 4",                 new input_seq(KEYCODE_8, input_seq.or_code, JOYCODE_SELECT_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN2,               "Coin 2",                 new input_seq(KEYCODE_6, input_seq.or_code, JOYCODE_SELECT_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN5,               "Coin 5",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN6,               "Coin 6",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN7,               "Coin 7",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN8,               "Coin 8",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN9,               "Coin 9",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN10,              "Coin 10",                new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN11,              "Coin 11",                new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   COIN12,              "Coin 12",                new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   BILL1,               "Bill 1",                 new input_seq(KEYCODE_BACKSPACE) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_service(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(service)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   SERVICE1,            "Service 1",              new input_seq(KEYCODE_9) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   SERVICE2,            "Service 2",              new input_seq(KEYCODE_0) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   SERVICE3,            "Service 3",              new input_seq(KEYCODE_MINUS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   SERVICE4,            "Service 4",              new input_seq(KEYCODE_EQUALS) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_tilt(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(tilt)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   TILT1,               "Tilt 1",                 new input_seq(KEYCODE_T) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   TILT2,               "Tilt 2",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   TILT3,               "Tilt 3",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   TILT4,               "Tilt 4",                 new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_other(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(other)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   POWER_ON,            "Power On",               new input_seq(KEYCODE_F1) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   POWER_OFF,           "Power Off",              new input_seq(KEYCODE_F2) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   SERVICE,             "Service",                new input_seq(KEYCODE_F2) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   TILT,                "Tilt",                   new input_seq(KEYCODE_T) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   INTERLOCK,           "Door Interlock",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   MEMORY_RESET,        "Memory Reset",           new input_seq(KEYCODE_F1) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   VOLUME_DOWN,         "Volume Down",            new input_seq(KEYCODE_MINUS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   VOLUME_UP,           "Volume Up",              new input_seq(KEYCODE_EQUALS) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_pedal(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(pedal)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, PEDAL,               "P1 Pedal 1",             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(0)), new input_seq(), new input_seq(KEYCODE_LCONTROL, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(0)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, PEDAL,               "P2 Pedal 1",             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(1)), new input_seq(), new input_seq(KEYCODE_A, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(1)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, PEDAL,               "P3 Pedal 1",             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(2)), new input_seq(), new input_seq(KEYCODE_RCONTROL, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(2)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, PEDAL,               "P4 Pedal 1",             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(3)), new input_seq(), new input_seq(KEYCODE_0_PAD, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(3)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, PEDAL,               "P5 Pedal 1",             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(4)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(4)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, PEDAL,               "P6 Pedal 1",             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(5)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(5)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, PEDAL,               "P7 Pedal 1",             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(6)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(6)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, PEDAL,               "P8 Pedal 1",             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(7)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(7)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, PEDAL,               "P9 Pedal 1",             new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(8)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(8)) );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, PEDAL,              "P10 Pedal 1",            new input_seq(JOYCODE_Z_NEG_ABSOLUTE_INDEXED(9)), new input_seq(), new input_seq(JOYCODE_BUTTON1_INDEXED(9)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_pedal2(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(pedal2)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, PEDAL2,              "P1 Pedal 2",             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(0)), new input_seq(), new input_seq(KEYCODE_LALT, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, PEDAL2,              "P2 Pedal 2",             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(1)), new input_seq(), new input_seq(KEYCODE_S, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(1)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, PEDAL2,              "P3 Pedal 2",             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(2)), new input_seq(), new input_seq(KEYCODE_RSHIFT, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(2)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, PEDAL2,              "P4 Pedal 2",             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(3)), new input_seq(), new input_seq(KEYCODE_DEL_PAD, input_seq.or_code, JOYCODE_BUTTON2_INDEXED(3)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, PEDAL2,              "P5 Pedal 2",             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(4)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(4)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, PEDAL2,              "P6 Pedal 2",             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(5)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(5)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, PEDAL2,              "P7 Pedal 2",             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(6)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(6)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, PEDAL2,              "P8 Pedal 2",             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(7)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(7)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, PEDAL2,              "P9 Pedal 2",             new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(8)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(8)) );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, PEDAL2,             "P10 Pedal 2",            new input_seq(JOYCODE_W_NEG_ABSOLUTE_INDEXED(9)), new input_seq(), new input_seq(JOYCODE_BUTTON2_INDEXED(9)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_pedal3(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(pedal3)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, PEDAL3,              "P1 Pedal 3",             new input_seq(), new input_seq(), new input_seq(KEYCODE_SPACE, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(0)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, PEDAL3,              "P2 Pedal 3",             new input_seq(), new input_seq(), new input_seq(KEYCODE_Q, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(1)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, PEDAL3,              "P3 Pedal 3",             new input_seq(), new input_seq(), new input_seq(KEYCODE_ENTER, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(2)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, PEDAL3,              "P4 Pedal 3",             new input_seq(), new input_seq(), new input_seq(KEYCODE_ENTER_PAD, input_seq.or_code, JOYCODE_BUTTON3_INDEXED(3)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, PEDAL3,              "P5 Pedal 3",             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(4)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, PEDAL3,              "P6 Pedal 3",             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(5)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, PEDAL3,              "P7 Pedal 3",             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(6)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, PEDAL3,              "P8 Pedal 3",             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(7)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, PEDAL3,              "P9 Pedal 3",             new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(8)) );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, PEDAL3,             "P10 Pedal 3",            new input_seq(), new input_seq(), new input_seq(JOYCODE_BUTTON3_INDEXED(9)) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_paddle(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(paddle)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, PADDLE,              "Paddle",                 new input_seq(JOYCODE_X_INDEXED(0), input_seq.or_code, MOUSECODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, PADDLE,              "Paddle 2",               new input_seq(JOYCODE_X_INDEXED(1), input_seq.or_code, MOUSECODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, PADDLE,              "Paddle 3",               new input_seq(JOYCODE_X_INDEXED(2), input_seq.or_code, MOUSECODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, PADDLE,              "Paddle 4",               new input_seq(JOYCODE_X_INDEXED(3), input_seq.or_code, MOUSECODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, PADDLE,              "Paddle 5",               new input_seq(JOYCODE_X_INDEXED(4), input_seq.or_code, MOUSECODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, PADDLE,              "Paddle 6",               new input_seq(JOYCODE_X_INDEXED(5), input_seq.or_code, MOUSECODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, PADDLE,              "Paddle 7",               new input_seq(JOYCODE_X_INDEXED(6), input_seq.or_code, MOUSECODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, PADDLE,              "Paddle 8",               new input_seq(JOYCODE_X_INDEXED(7), input_seq.or_code, MOUSECODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, PADDLE,              "Paddle 9",               new input_seq(JOYCODE_X_INDEXED(8), input_seq.or_code, MOUSECODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, PADDLE,             "Paddle 10",              new input_seq(JOYCODE_X_INDEXED(9), input_seq.or_code, MOUSECODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_paddle_v(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(paddle_v)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, PADDLE_V,            "Paddle V",               new input_seq(JOYCODE_Y_INDEXED(0), input_seq.or_code, MOUSECODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, PADDLE_V,            "Paddle V 2",             new input_seq(JOYCODE_Y_INDEXED(1), input_seq.or_code, MOUSECODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, PADDLE_V,            "Paddle V 3",             new input_seq(JOYCODE_Y_INDEXED(2), input_seq.or_code, MOUSECODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, PADDLE_V,            "Paddle V 4",             new input_seq(JOYCODE_Y_INDEXED(3), input_seq.or_code, MOUSECODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, PADDLE_V,            "Paddle V 5",             new input_seq(JOYCODE_Y_INDEXED(4), input_seq.or_code, MOUSECODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, PADDLE_V,            "Paddle V 6",             new input_seq(JOYCODE_Y_INDEXED(5), input_seq.or_code, MOUSECODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, PADDLE_V,            "Paddle V 7",             new input_seq(JOYCODE_Y_INDEXED(6), input_seq.or_code, MOUSECODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, PADDLE_V,            "Paddle V 8",             new input_seq(JOYCODE_Y_INDEXED(7), input_seq.or_code, MOUSECODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, PADDLE_V,            "Paddle V 9",             new input_seq(JOYCODE_Y_INDEXED(8), input_seq.or_code, MOUSECODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, PADDLE_V,            "Paddle V 10",           new input_seq(JOYCODE_Y_INDEXED(9), input_seq.or_code, MOUSECODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_positional(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(positional)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, POSITIONAL,          "Positional",             new input_seq(MOUSECODE_X_INDEXED(0), input_seq.or_code, JOYCODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, POSITIONAL,          "Positional 2",           new input_seq(MOUSECODE_X_INDEXED(1), input_seq.or_code, JOYCODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, POSITIONAL,          "Positional 3",           new input_seq(MOUSECODE_X_INDEXED(2), input_seq.or_code, JOYCODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, POSITIONAL,          "Positional 4",           new input_seq(MOUSECODE_X_INDEXED(3), input_seq.or_code, JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, POSITIONAL,          "Positional 5",           new input_seq(MOUSECODE_X_INDEXED(4), input_seq.or_code, JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, POSITIONAL,          "Positional 6",           new input_seq(MOUSECODE_X_INDEXED(5), input_seq.or_code, JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, POSITIONAL,          "Positional 7",           new input_seq(MOUSECODE_X_INDEXED(6), input_seq.or_code, JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, POSITIONAL,          "Positional 8",           new input_seq(MOUSECODE_X_INDEXED(7), input_seq.or_code, JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, POSITIONAL,          "Positional 9",           new input_seq(MOUSECODE_X_INDEXED(8), input_seq.or_code, JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, POSITIONAL,         "Positional 10",          new input_seq(MOUSECODE_X_INDEXED(9), input_seq.or_code, JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_positional_v(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(positional_v)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, POSITIONAL_V,        "Positional V",           new input_seq(MOUSECODE_Y_INDEXED(0), input_seq.or_code, JOYCODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, POSITIONAL_V,        "Positional V 2",         new input_seq(MOUSECODE_Y_INDEXED(1), input_seq.or_code, JOYCODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, POSITIONAL_V,        "Positional V 3",         new input_seq(MOUSECODE_Y_INDEXED(2), input_seq.or_code, JOYCODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, POSITIONAL_V,        "Positional V 4",         new input_seq(MOUSECODE_Y_INDEXED(3), input_seq.or_code, JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, POSITIONAL_V,        "Positional V 5",         new input_seq(MOUSECODE_Y_INDEXED(4), input_seq.or_code, JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, POSITIONAL_V,        "Positional V 6",         new input_seq(MOUSECODE_Y_INDEXED(5), input_seq.or_code, JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, POSITIONAL_V,        "Positional V 7",         new input_seq(MOUSECODE_Y_INDEXED(6), input_seq.or_code, JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, POSITIONAL_V,        "Positional V 8",         new input_seq(MOUSECODE_Y_INDEXED(7), input_seq.or_code, JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, POSITIONAL_V,        "Positional V 9",         new input_seq(MOUSECODE_Y_INDEXED(8), input_seq.or_code, JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, POSITIONAL_V,       "Positional V 10",        new input_seq(MOUSECODE_Y_INDEXED(9), input_seq.or_code, JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_dial(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(dial)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, DIAL,                "Dial",                   new input_seq(MOUSECODE_X_INDEXED(0), input_seq.or_code, JOYCODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, DIAL,                "Dial 2",                 new input_seq(MOUSECODE_X_INDEXED(1), input_seq.or_code, JOYCODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, DIAL,                "Dial 3",                 new input_seq(MOUSECODE_X_INDEXED(2), input_seq.or_code, JOYCODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, DIAL,                "Dial 4",                 new input_seq(MOUSECODE_X_INDEXED(3), input_seq.or_code, JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, DIAL,                "Dial 5",                 new input_seq(MOUSECODE_X_INDEXED(4), input_seq.or_code, JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, DIAL,                "Dial 6",                 new input_seq(MOUSECODE_X_INDEXED(5), input_seq.or_code, JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, DIAL,                "Dial 7",                 new input_seq(MOUSECODE_X_INDEXED(6), input_seq.or_code, JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, DIAL,                "Dial 8",                 new input_seq(MOUSECODE_X_INDEXED(7), input_seq.or_code, JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, DIAL,                "Dial 9",                 new input_seq(MOUSECODE_X_INDEXED(8), input_seq.or_code, JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, DIAL,               "Dial 10",                new input_seq(MOUSECODE_X_INDEXED(9), input_seq.or_code, JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_dial_v(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(dial_v)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, DIAL_V,              "Dial V",                 new input_seq(MOUSECODE_Y_INDEXED(0), input_seq.or_code, JOYCODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, DIAL_V,              "Dial V 2",               new input_seq(MOUSECODE_Y_INDEXED(1), input_seq.or_code, JOYCODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, DIAL_V,              "Dial V 3",               new input_seq(MOUSECODE_Y_INDEXED(2), input_seq.or_code, JOYCODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, DIAL_V,              "Dial V 4",               new input_seq(MOUSECODE_Y_INDEXED(3), input_seq.or_code, JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, DIAL_V,              "Dial V 5",               new input_seq(MOUSECODE_Y_INDEXED(4), input_seq.or_code, JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, DIAL_V,              "Dial V 6",               new input_seq(MOUSECODE_Y_INDEXED(5), input_seq.or_code, JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, DIAL_V,              "Dial V 7",               new input_seq(MOUSECODE_Y_INDEXED(6), input_seq.or_code, JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, DIAL_V,              "Dial V 8",               new input_seq(MOUSECODE_Y_INDEXED(7), input_seq.or_code, JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, DIAL_V,              "Dial V 9",               new input_seq(MOUSECODE_Y_INDEXED(8), input_seq.or_code, JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, DIAL_V,             "Dial V 10",              new input_seq(MOUSECODE_Y_INDEXED(9), input_seq.or_code, JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_trackball_x(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(trackball_x)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, TRACKBALL_X,         "Track X",                new input_seq(MOUSECODE_X_INDEXED(0), input_seq.or_code, JOYCODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, TRACKBALL_X,         "Track X 2",              new input_seq(MOUSECODE_X_INDEXED(1), input_seq.or_code, JOYCODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, TRACKBALL_X,         "Track X 3",              new input_seq(MOUSECODE_X_INDEXED(2), input_seq.or_code, JOYCODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, TRACKBALL_X,         "Track X 4",              new input_seq(MOUSECODE_X_INDEXED(3), input_seq.or_code, JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, TRACKBALL_X,         "Track X 5",              new input_seq(MOUSECODE_X_INDEXED(4), input_seq.or_code, JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, TRACKBALL_X,         "Track X 6",              new input_seq(MOUSECODE_X_INDEXED(5), input_seq.or_code, JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, TRACKBALL_X,         "Track X 7",              new input_seq(MOUSECODE_X_INDEXED(6), input_seq.or_code, JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, TRACKBALL_X,         "Track X 8",              new input_seq(MOUSECODE_X_INDEXED(7), input_seq.or_code, JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, TRACKBALL_X,         "Track X 9",              new input_seq(MOUSECODE_X_INDEXED(8), input_seq.or_code, JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, TRACKBALL_X,        "Track X 10",             new input_seq(MOUSECODE_X_INDEXED(9), input_seq.or_code, JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_trackball_y(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(trackball_y)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, TRACKBALL_Y,         "Track Y",                new input_seq(MOUSECODE_Y_INDEXED(0), input_seq.or_code, JOYCODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, TRACKBALL_Y,         "Track Y 2",              new input_seq(MOUSECODE_Y_INDEXED(1), input_seq.or_code, JOYCODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, TRACKBALL_Y,         "Track Y 3",              new input_seq(MOUSECODE_Y_INDEXED(2), input_seq.or_code, JOYCODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, TRACKBALL_Y,         "Track Y 4",              new input_seq(MOUSECODE_Y_INDEXED(3), input_seq.or_code, JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, TRACKBALL_Y,         "Track Y 5",              new input_seq(MOUSECODE_Y_INDEXED(4), input_seq.or_code, JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, TRACKBALL_Y,         "Track Y 6",              new input_seq(MOUSECODE_Y_INDEXED(5), input_seq.or_code, JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, TRACKBALL_Y,         "Track Y 7",              new input_seq(MOUSECODE_Y_INDEXED(6), input_seq.or_code, JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, TRACKBALL_Y,         "Track Y 8",              new input_seq(MOUSECODE_Y_INDEXED(7), input_seq.or_code, JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, TRACKBALL_Y,         "Track Y 9",              new input_seq(MOUSECODE_Y_INDEXED(8), input_seq.or_code, JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, TRACKBALL_Y,        "Track Y 10",             new input_seq(MOUSECODE_Y_INDEXED(9), input_seq.or_code, JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_ad_stick_x(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(ad_stick_x)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, AD_STICK_X,          "AD Stick X",             new input_seq(JOYCODE_X_INDEXED(0), input_seq.or_code, MOUSECODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, AD_STICK_X,          "AD Stick X 2",           new input_seq(JOYCODE_X_INDEXED(1), input_seq.or_code, MOUSECODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, AD_STICK_X,          "AD Stick X 3",           new input_seq(JOYCODE_X_INDEXED(2), input_seq.or_code, MOUSECODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, AD_STICK_X,          "AD Stick X 4",           new input_seq(JOYCODE_X_INDEXED(3), input_seq.or_code, MOUSECODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, AD_STICK_X,          "AD Stick X 5",           new input_seq(JOYCODE_X_INDEXED(4), input_seq.or_code, MOUSECODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, AD_STICK_X,          "AD Stick X 6",           new input_seq(JOYCODE_X_INDEXED(5), input_seq.or_code, MOUSECODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, AD_STICK_X,          "AD Stick X 7",           new input_seq(JOYCODE_X_INDEXED(6), input_seq.or_code, MOUSECODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, AD_STICK_X,          "AD Stick X 8",           new input_seq(JOYCODE_X_INDEXED(7), input_seq.or_code, MOUSECODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, AD_STICK_X,          "AD Stick X 9",           new input_seq(JOYCODE_X_INDEXED(8), input_seq.or_code, MOUSECODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, AD_STICK_X,         "AD Stick X 10",          new input_seq(JOYCODE_X_INDEXED(9), input_seq.or_code, MOUSECODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_ad_stick_y(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(ad_stick_y)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, AD_STICK_Y,          "AD Stick Y",             new input_seq(JOYCODE_Y_INDEXED(0), input_seq.or_code, MOUSECODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, AD_STICK_Y,          "AD Stick Y 2",           new input_seq(JOYCODE_Y_INDEXED(1), input_seq.or_code, MOUSECODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, AD_STICK_Y,          "AD Stick Y 3",           new input_seq(JOYCODE_Y_INDEXED(2), input_seq.or_code, MOUSECODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, AD_STICK_Y,          "AD Stick Y 4",           new input_seq(JOYCODE_Y_INDEXED(3), input_seq.or_code, MOUSECODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, AD_STICK_Y,          "AD Stick Y 5",           new input_seq(JOYCODE_Y_INDEXED(4), input_seq.or_code, MOUSECODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, AD_STICK_Y,          "AD Stick Y 6",           new input_seq(JOYCODE_Y_INDEXED(5), input_seq.or_code, MOUSECODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, AD_STICK_Y,          "AD Stick Y 7",           new input_seq(JOYCODE_Y_INDEXED(6), input_seq.or_code, MOUSECODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, AD_STICK_Y,          "AD Stick Y 8",           new input_seq(JOYCODE_Y_INDEXED(7), input_seq.or_code, MOUSECODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, AD_STICK_Y,          "AD Stick Y 9",           new input_seq(JOYCODE_Y_INDEXED(8), input_seq.or_code, MOUSECODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, AD_STICK_Y,         "AD Stick Y 10",          new input_seq(JOYCODE_Y_INDEXED(9), input_seq.or_code, MOUSECODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_ad_stick_z(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(ad_stick_z)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, AD_STICK_Z,          "AD Stick Z",             new input_seq(JOYCODE_Z_INDEXED(0)), new input_seq(KEYCODE_A), new input_seq(KEYCODE_Z) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, AD_STICK_Z,          "AD Stick Z 2",           new input_seq(JOYCODE_Z_INDEXED(1)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, AD_STICK_Z,          "AD Stick Z 3",           new input_seq(JOYCODE_Z_INDEXED(2)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, AD_STICK_Z,          "AD Stick Z 4",           new input_seq(JOYCODE_Z_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, AD_STICK_Z,          "AD Stick Z 5",           new input_seq(JOYCODE_Z_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, AD_STICK_Z,          "AD Stick Z 6",           new input_seq(JOYCODE_Z_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, AD_STICK_Z,          "AD Stick Z 7",           new input_seq(JOYCODE_Z_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, AD_STICK_Z,          "AD Stick Z 8",           new input_seq(JOYCODE_Z_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, AD_STICK_Z,          "AD Stick Z 9",           new input_seq(JOYCODE_Z_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, AD_STICK_Z,         "AD Stick Z 10",          new input_seq(JOYCODE_Z_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_lightgun_x(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(lightgun_x)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, LIGHTGUN_X,          "Lightgun X",             new input_seq(GUNCODE_X_INDEXED(0), input_seq.or_code, MOUSECODE_X_INDEXED(0), input_seq.or_code, JOYCODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, LIGHTGUN_X,          "Lightgun X 2",           new input_seq(GUNCODE_X_INDEXED(1), input_seq.or_code, MOUSECODE_X_INDEXED(1), input_seq.or_code, JOYCODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, LIGHTGUN_X,          "Lightgun X 3",           new input_seq(GUNCODE_X_INDEXED(2), input_seq.or_code, MOUSECODE_X_INDEXED(2), input_seq.or_code, JOYCODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, LIGHTGUN_X,          "Lightgun X 4",           new input_seq(GUNCODE_X_INDEXED(3), input_seq.or_code, MOUSECODE_X_INDEXED(3), input_seq.or_code, JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, LIGHTGUN_X,          "Lightgun X 5",           new input_seq(GUNCODE_X_INDEXED(4), input_seq.or_code, MOUSECODE_X_INDEXED(4), input_seq.or_code, JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, LIGHTGUN_X,          "Lightgun X 6",           new input_seq(GUNCODE_X_INDEXED(5), input_seq.or_code, MOUSECODE_X_INDEXED(5), input_seq.or_code, JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, LIGHTGUN_X,          "Lightgun X 7",           new input_seq(GUNCODE_X_INDEXED(6), input_seq.or_code, MOUSECODE_X_INDEXED(6), input_seq.or_code, JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, LIGHTGUN_X,          "Lightgun X 8",           new input_seq(GUNCODE_X_INDEXED(7), input_seq.or_code, MOUSECODE_X_INDEXED(7), input_seq.or_code, JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, LIGHTGUN_X,          "Lightgun X 9",           new input_seq(GUNCODE_X_INDEXED(8), input_seq.or_code, MOUSECODE_X_INDEXED(8), input_seq.or_code, JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, LIGHTGUN_X,         "Lightgun X 10",          new input_seq(GUNCODE_X_INDEXED(9), input_seq.or_code, MOUSECODE_X_INDEXED(9), input_seq.or_code, JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_lightgun_y(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(lightgun_y)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, LIGHTGUN_Y,          "Lightgun Y",             new input_seq(GUNCODE_Y_INDEXED(0), input_seq.or_code, MOUSECODE_Y_INDEXED(0), input_seq.or_code, JOYCODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, LIGHTGUN_Y,          "Lightgun Y 2",           new input_seq(GUNCODE_Y_INDEXED(1), input_seq.or_code, MOUSECODE_Y_INDEXED(1), input_seq.or_code, JOYCODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, LIGHTGUN_Y,          "Lightgun Y 3",           new input_seq(GUNCODE_Y_INDEXED(2), input_seq.or_code, MOUSECODE_Y_INDEXED(2), input_seq.or_code, JOYCODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, LIGHTGUN_Y,          "Lightgun Y 4",           new input_seq(GUNCODE_Y_INDEXED(3), input_seq.or_code, MOUSECODE_Y_INDEXED(3), input_seq.or_code, JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, LIGHTGUN_Y,          "Lightgun Y 5",           new input_seq(GUNCODE_Y_INDEXED(4), input_seq.or_code, MOUSECODE_Y_INDEXED(4), input_seq.or_code, JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, LIGHTGUN_Y,          "Lightgun Y 6",           new input_seq(GUNCODE_Y_INDEXED(5), input_seq.or_code, MOUSECODE_Y_INDEXED(5), input_seq.or_code, JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, LIGHTGUN_Y,          "Lightgun Y 7",           new input_seq(GUNCODE_Y_INDEXED(6), input_seq.or_code, MOUSECODE_Y_INDEXED(6), input_seq.or_code, JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, LIGHTGUN_Y,          "Lightgun Y 8",           new input_seq(GUNCODE_Y_INDEXED(7), input_seq.or_code, MOUSECODE_Y_INDEXED(7), input_seq.or_code, JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, LIGHTGUN_Y,          "Lightgun Y 9",           new input_seq(GUNCODE_Y_INDEXED(8), input_seq.or_code, MOUSECODE_Y_INDEXED(8), input_seq.or_code, JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, LIGHTGUN_Y,         "Lightgun Y 10",          new input_seq(GUNCODE_Y_INDEXED(9), input_seq.or_code, MOUSECODE_Y_INDEXED(9), input_seq.or_code, JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_mouse_x(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(mouse_x)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, MOUSE_X,             "Mouse X",                new input_seq(MOUSECODE_X_INDEXED(0)), new input_seq(KEYCODE_LEFT), new input_seq(KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, MOUSE_X,             "Mouse X 2",              new input_seq(MOUSECODE_X_INDEXED(1)), new input_seq(KEYCODE_D), new input_seq(KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, MOUSE_X,             "Mouse X 3",              new input_seq(MOUSECODE_X_INDEXED(2)), new input_seq(KEYCODE_J), new input_seq(KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, MOUSE_X,             "Mouse X 4",              new input_seq(MOUSECODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, MOUSE_X,             "Mouse X 5",              new input_seq(MOUSECODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, MOUSE_X,             "Mouse X 6",              new input_seq(MOUSECODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, MOUSE_X,             "Mouse X 7",              new input_seq(MOUSECODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, MOUSE_X,             "Mouse X 8",              new input_seq(MOUSECODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, MOUSE_X,             "Mouse X 9",              new input_seq(MOUSECODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, MOUSE_X,            "Mouse X 10",             new input_seq(MOUSECODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_mouse_y(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(mouse_y)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, PLAYER1, MOUSE_Y,             "Mouse Y",                new input_seq(MOUSECODE_Y_INDEXED(0)), new input_seq(KEYCODE_UP), new input_seq(KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, PLAYER2, MOUSE_Y,             "Mouse Y 2",              new input_seq(MOUSECODE_Y_INDEXED(1)), new input_seq(KEYCODE_R), new input_seq(KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, PLAYER3, MOUSE_Y,             "Mouse Y 3",              new input_seq(MOUSECODE_Y_INDEXED(2)), new input_seq(KEYCODE_I), new input_seq(KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, PLAYER4, MOUSE_Y,             "Mouse Y 4",              new input_seq(MOUSECODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, PLAYER5, MOUSE_Y,             "Mouse Y 5",              new input_seq(MOUSECODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, PLAYER6, MOUSE_Y,             "Mouse Y 6",              new input_seq(MOUSECODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, PLAYER7, MOUSE_Y,             "Mouse Y 7",              new input_seq(MOUSECODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, PLAYER8, MOUSE_Y,             "Mouse Y 8",              new input_seq(MOUSECODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, PLAYER9, MOUSE_Y,             "Mouse Y 9",              new input_seq(MOUSECODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, PLAYER10, MOUSE_Y,            "Mouse Y 10",             new input_seq(MOUSECODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_keypad(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(keypad)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   KEYPAD,              "Keypad",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   KEYBOARD,            "Keyboard",               new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_ui(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(ui)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_ON_SCREEN_DISPLAY,"On Screen Display",      new input_seq(KEYCODE_TILDE, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_DEBUG_BREAK,      "Break in Debugger",      new input_seq(KEYCODE_TILDE, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_CONFIGURE,        "Config Menu",            new input_seq(KEYCODE_TAB) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PAUSE,            "Pause",                  new input_seq(KEYCODE_P, input_seq.not_code, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PAUSE_SINGLE,     "Pause - Single Step",    new input_seq(KEYCODE_P, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_REWIND_SINGLE,    "Rewind - Single Step",   new input_seq(KEYCODE_TILDE, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_RESET_MACHINE,    "Reset Machine",          new input_seq(KEYCODE_F3, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SOFT_RESET,       "Soft Reset",             new input_seq(KEYCODE_F3, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SHOW_GFX,         "Show Gfx",               new input_seq(KEYCODE_F4) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FRAMESKIP_DEC,    "Frameskip Dec",          new input_seq(KEYCODE_F8) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FRAMESKIP_INC,    "Frameskip Inc",          new input_seq(KEYCODE_F9) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_THROTTLE,         "Throttle",               new input_seq(KEYCODE_F10) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FAST_FORWARD,     "Fast Forward",           new input_seq(KEYCODE_INSERT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SHOW_FPS,         "Show FPS",               new input_seq(KEYCODE_F11, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SNAPSHOT,         "Save Snapshot",          new input_seq(KEYCODE_F12, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_TIMECODE,         "Write current timecode", new input_seq(KEYCODE_F12, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_RECORD_MNG,       "Record MNG",             new input_seq(KEYCODE_F12, KEYCODE_LSHIFT, input_seq.not_code, KEYCODE_LCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_RECORD_AVI,       "Record AVI",             new input_seq(KEYCODE_F12, KEYCODE_LSHIFT, KEYCODE_LCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_TOGGLE_CHEAT,     "Toggle Cheat",           new input_seq(KEYCODE_F6) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_UP,               "UI Up",                  new input_seq(KEYCODE_UP, input_seq.or_code, JOYCODE_Y_UP_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_DOWN,             "UI Down",                new input_seq(KEYCODE_DOWN, input_seq.or_code, JOYCODE_Y_DOWN_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_LEFT,             "UI Left",                new input_seq(KEYCODE_LEFT, input_seq.or_code, JOYCODE_X_LEFT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_RIGHT,            "UI Right",               new input_seq(KEYCODE_RIGHT, input_seq.or_code, JOYCODE_X_RIGHT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_HOME,             "UI Home",                new input_seq(KEYCODE_HOME) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_END,              "UI End",                 new input_seq(KEYCODE_END) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PAGE_UP,          "UI Page Up",             new input_seq(KEYCODE_PGUP) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PAGE_DOWN,        "UI Page Down",           new input_seq(KEYCODE_PGDN) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FOCUS_NEXT,       "UI Focus Next",          new input_seq(KEYCODE_TAB, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FOCUS_PREV,       "UI Focus Previous",      new input_seq(KEYCODE_TAB, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SELECT,           "UI Select",              new input_seq(KEYCODE_ENTER, input_seq.or_code, JOYCODE_BUTTON1_INDEXED(0), input_seq.or_code, KEYCODE_ENTER_PAD) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_CANCEL,           "UI Cancel",              new input_seq(KEYCODE_ESC) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_DISPLAY_COMMENT,  "UI Display Comment",     new input_seq(KEYCODE_SPACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_CLEAR,            "UI Clear",               new input_seq(KEYCODE_DEL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_ZOOM_IN,          "UI Zoom In",             new input_seq(KEYCODE_EQUALS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_ZOOM_OUT,         "UI Zoom Out",            new input_seq(KEYCODE_MINUS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PREV_GROUP,       "UI Previous Group",      new input_seq(KEYCODE_OPENBRACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_NEXT_GROUP,       "UI Next Group",          new input_seq(KEYCODE_CLOSEBRACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_ROTATE,           "UI Rotate",              new input_seq(KEYCODE_R) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SHOW_PROFILER,    "Show Profiler",          new input_seq(KEYCODE_F11, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_TOGGLE_UI,        "UI Toggle",              new input_seq(KEYCODE_SCRLOCK, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_PASTE,            "UI Paste Text",          new input_seq(KEYCODE_SCRLOCK, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_TOGGLE_DEBUG,     "Toggle Debugger",        new input_seq(KEYCODE_F5) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_SAVE_STATE,       "Save State",             new input_seq(KEYCODE_F7, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_LOAD_STATE,       "Load State",             new input_seq(KEYCODE_F7, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_TAPE_START,       "UI (First) Tape Start",  new input_seq(KEYCODE_F2, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_TAPE_STOP,        "UI (First) Tape Stop",   new input_seq(KEYCODE_F2, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_DATS,             "UI External DAT View",   new input_seq(KEYCODE_LALT, KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_FAVORITES,        "UI Add/Remove favorites",new input_seq(KEYCODE_LALT, KEYCODE_F) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_EXPORT,           "UI Export list",         new input_seq(KEYCODE_LALT, KEYCODE_E) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_AUDIT_FAST,       "UI Audit Unavailable",   new input_seq(KEYCODE_F1, input_seq.not_code, KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      UI_AUDIT_ALL,        "UI Audit All",           new input_seq(KEYCODE_F1, KEYCODE_LSHIFT) );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_osd(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(osd)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_1,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_2,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_3,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_4,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_5,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_6,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_7,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_8,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_9,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_10,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_11,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_12,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_13,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_14,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_15,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, UI,      OSD_16,              null,                     new input_seq() );
        }  //CORE_INPUT_TYPES_END()


        static void emplace_core_types_invalid(std.vector<input_type_entry> typelist)  //CORE_INPUT_TYPES_BEGIN(invalid)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, UNKNOWN,             null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, UNUSED,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, SPECIAL,             null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, OTHER,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, ADJUSTER,            null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, DIPSWITCH,           null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, INVALID, CONFIG,              null,                     new input_seq() );
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
        CORE_INPUT_TYPES_P4
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
                    CORE_INPUT_TYPES_P4
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
            CORE_INPUT_TYPES_P4,
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
            typelist.reserve((int)core_input_types_count());

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
            emplace_core_types_p4(typelist);
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
