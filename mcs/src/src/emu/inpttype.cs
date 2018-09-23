// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class inpttype_global
    {
        static void INPUT_PORT_DIGITAL_TYPE(simple_list<input_type_entry> typelist, int _player, ioport_group _group, ioport_type _type, string _name, input_seq _seq)
        {
            typelist.append(new input_type_entry(_type, _group, (_player == 0) ? _player : (_player) - 1, (_player == 0) ? _type.ToString() : string.Format("P{0}_{1}", _player, _type), _name, _seq));
        }

        static void INPUT_PORT_ANALOG_TYPE(simple_list<input_type_entry> typelist, int _player, ioport_group _group, ioport_type _type, string _name, input_seq _seq, input_seq _decseq, input_seq _incseq)
        {
            typelist.append(new input_type_entry(_type, _group, (_player == 0) ? _player : (_player) - 1, (_player == 0) ? _type.ToString() : string.Format("P{0}_{1}", _player, _type), _name, _seq, _decseq, _incseq));
        }


        static void construct_core_types_P1(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICK_UP,         "P1 Up",                  new input_seq(input_global.KEYCODE_UP, input_seq.or_code, input_global.JOYCODE_Y_UP_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICK_DOWN,       "P1 Down",                new input_seq(input_global.KEYCODE_DOWN, input_seq.or_code, input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICK_LEFT,       "P1 Left",                new input_seq(input_global.KEYCODE_LEFT, input_seq.or_code, input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICK_RIGHT,      "P1 Right",               new input_seq(input_global.KEYCODE_RIGHT, input_seq.or_code, input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICKRIGHT_UP,    "P1 Right Stick/Up",      new input_seq(input_global.KEYCODE_I, input_seq.or_code, input_global.JOYCODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICKRIGHT_DOWN,  "P1 Right Stick/Down",    new input_seq(input_global.KEYCODE_K, input_seq.or_code, input_global.JOYCODE_BUTTON3_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICKRIGHT_LEFT,  "P1 Right Stick/Left",    new input_seq(input_global.KEYCODE_J, input_seq.or_code, input_global.JOYCODE_BUTTON1_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICKRIGHT_RIGHT, "P1 Right Stick/Right",   new input_seq(input_global.KEYCODE_L, input_seq.or_code, input_global.JOYCODE_BUTTON4_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICKLEFT_UP,     "P1 Left Stick/Up",       new input_seq(input_global.KEYCODE_E, input_seq.or_code, input_global.JOYCODE_Y_UP_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICKLEFT_DOWN,   "P1 Left Stick/Down",     new input_seq(input_global.KEYCODE_D, input_seq.or_code, input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICKLEFT_LEFT,   "P1 Left Stick/Left",     new input_seq(input_global.KEYCODE_S, input_seq.or_code, input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_JOYSTICKLEFT_RIGHT,  "P1 Left Stick/Right",    new input_seq(input_global.KEYCODE_F, input_seq.or_code, input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON1,             "P1 Button 1",            new input_seq(input_global.KEYCODE_LCONTROL, input_seq.or_code, input_global.JOYCODE_BUTTON1_INDEXED(0), input_seq.or_code, input_global.MOUSECODE_BUTTON1_INDEXED(0), input_seq.or_code, input_global.GUNCODE_BUTTON1_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON2,             "P1 Button 2",            new input_seq(input_global.KEYCODE_LALT, input_seq.or_code, input_global.JOYCODE_BUTTON2_INDEXED(0), input_seq.or_code, input_global.MOUSECODE_BUTTON3_INDEXED(0), input_seq.or_code, input_global.GUNCODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON3,             "P1 Button 3",            new input_seq(input_global.KEYCODE_SPACE, input_seq.or_code, input_global.JOYCODE_BUTTON3_INDEXED(0), input_seq.or_code, input_global.MOUSECODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON4,             "P1 Button 4",            new input_seq(input_global.KEYCODE_LSHIFT, input_seq.or_code, input_global.JOYCODE_BUTTON4_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON5,             "P1 Button 5",            new input_seq(input_global.KEYCODE_Z, input_seq.or_code, input_global.JOYCODE_BUTTON5_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON6,             "P1 Button 6",            new input_seq(input_global.KEYCODE_X, input_seq.or_code, input_global.JOYCODE_BUTTON6_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON7,             "P1 Button 7",            new input_seq(input_global.KEYCODE_C, input_seq.or_code, input_global.JOYCODE_BUTTON7_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON8,             "P1 Button 8",            new input_seq(input_global.KEYCODE_V, input_seq.or_code, input_global.JOYCODE_BUTTON8_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON9,             "P1 Button 9",            new input_seq(input_global.KEYCODE_B, input_seq.or_code, input_global.JOYCODE_BUTTON9_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON10,            "P1 Button 10",           new input_seq(input_global.KEYCODE_N, input_seq.or_code, input_global.JOYCODE_BUTTON10_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON11,            "P1 Button 11",           new input_seq(input_global.KEYCODE_M, input_seq.or_code, input_global.JOYCODE_BUTTON11_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON12,            "P1 Button 12",           new input_seq(input_global.KEYCODE_COMMA, input_seq.or_code, input_global.JOYCODE_BUTTON12_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON13,            "P1 Button 13",           new input_seq(input_global.KEYCODE_STOP, input_seq.or_code, input_global.JOYCODE_BUTTON13_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON14,            "P1 Button 14",           new input_seq(input_global.KEYCODE_SLASH, input_seq.or_code, input_global.JOYCODE_BUTTON14_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON15,            "P1 Button 15",           new input_seq(input_global.KEYCODE_RSHIFT, input_seq.or_code, input_global.JOYCODE_BUTTON15_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_BUTTON16,            "P1 Button 16",           new input_seq(input_global.JOYCODE_BUTTON16_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_START,               "P1 Start",               new input_seq(input_global.KEYCODE_1, input_seq.or_code, input_global.JOYCODE_START_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_SELECT,              "P1 Select",              new input_seq(input_global.KEYCODE_5, input_seq.or_code, input_global.JOYCODE_SELECT_INDEXED(0)) );
        }

        static void construct_core_types_P1_mahjong(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_A,           "P1 Mahjong A",           new input_seq(input_global.KEYCODE_A) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_B,           "P1 Mahjong B",           new input_seq(input_global.KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_C,           "P1 Mahjong C",           new input_seq(input_global.KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_D,           "P1 Mahjong D",           new input_seq(input_global.KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_E,           "P1 Mahjong E",           new input_seq(input_global.KEYCODE_E) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_F,           "P1 Mahjong F",           new input_seq(input_global.KEYCODE_F) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_G,           "P1 Mahjong G",           new input_seq(input_global.KEYCODE_G) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_H,           "P1 Mahjong H",           new input_seq(input_global.KEYCODE_H) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_I,           "P1 Mahjong I",           new input_seq(input_global.KEYCODE_I) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_J,           "P1 Mahjong J",           new input_seq(input_global.KEYCODE_J) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_K,           "P1 Mahjong K",           new input_seq(input_global.KEYCODE_K) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_L,           "P1 Mahjong L",           new input_seq(input_global.KEYCODE_L) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_M,           "P1 Mahjong M",           new input_seq(input_global.KEYCODE_M) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_N,           "P1 Mahjong N",           new input_seq(input_global.KEYCODE_N) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_O,           "P1 Mahjong O",           new input_seq(input_global.KEYCODE_O) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_P,           "P1 Mahjong P",           new input_seq(input_global.KEYCODE_COLON) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_Q,           "P1 Mahjong Q",           new input_seq(input_global.KEYCODE_Q) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_KAN,         "P1 Mahjong Kan",         new input_seq(input_global.KEYCODE_LCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_PON,         "P1 Mahjong Pon",         new input_seq(input_global.KEYCODE_LALT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_CHI,         "P1 Mahjong Chi",         new input_seq(input_global.KEYCODE_SPACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_REACH,       "P1 Mahjong Reach",       new input_seq(input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_RON,         "P1 Mahjong Ron",         new input_seq(input_global.KEYCODE_Z) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_BET,         "P1 Mahjong Bet",         new input_seq(input_global.KEYCODE_3) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_LAST_CHANCE, "P1 Mahjong Last Chance", new input_seq(input_global.KEYCODE_RALT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_SCORE,       "P1 Mahjong Score",       new input_seq(input_global.KEYCODE_RCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_DOUBLE_UP,   "P1 Mahjong Double Up",   new input_seq(input_global.KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_FLIP_FLOP,   "P1 Mahjong Flip Flop",   new input_seq(input_global.KEYCODE_Y) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_BIG,         "P1 Mahjong Big",         new input_seq(input_global.KEYCODE_ENTER) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MAHJONG_SMALL,       "P1 Mahjong Small",       new input_seq(input_global.KEYCODE_BACKSPACE) );
        }

        static void construct_core_types_P1_hanafuda(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_HANAFUDA_A,          "P1 Hanafuda A/1",        new input_seq(input_global.KEYCODE_A) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_HANAFUDA_B,          "P1 Hanafuda B/2",        new input_seq(input_global.KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_HANAFUDA_C,          "P1 Hanafuda C/3",        new input_seq(input_global.KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_HANAFUDA_D,          "P1 Hanafuda D/4",        new input_seq(input_global.KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_HANAFUDA_E,          "P1 Hanafuda E/5",        new input_seq(input_global.KEYCODE_E) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_HANAFUDA_F,          "P1 Hanafuda F/6",        new input_seq(input_global.KEYCODE_F) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_HANAFUDA_G,          "P1 Hanafuda G/7",        new input_seq(input_global.KEYCODE_G) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_HANAFUDA_H,          "P1 Hanafuda H/8",        new input_seq(input_global.KEYCODE_H) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_HANAFUDA_YES,        "P1 Hanafuda Yes",        new input_seq(input_global.KEYCODE_M) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_HANAFUDA_NO,         "P1 Hanafuda No",         new input_seq(input_global.KEYCODE_N) );
        }

        static void construct_core_types_gamble(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_HIGH,         "High",                   new input_seq(input_global.KEYCODE_A) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_LOW,          "Low",                    new input_seq(input_global.KEYCODE_S) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_HALF,         "Half Gamble",            new input_seq(input_global.KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_DEAL,         "Deal",                   new input_seq(input_global.KEYCODE_2) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_D_UP,         "Double Up",              new input_seq(input_global.KEYCODE_3) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_TAKE,         "Take",                   new input_seq(input_global.KEYCODE_4) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_STAND,        "Stand",                  new input_seq(input_global.KEYCODE_L) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_BET,          "Bet",                    new input_seq(input_global.KEYCODE_M) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_KEYIN,        "Key In",                 new input_seq(input_global.KEYCODE_Q) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_KEYOUT,       "Key Out",                new input_seq(input_global.KEYCODE_W) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_PAYOUT,       "Payout",                 new input_seq(input_global.KEYCODE_I) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_DOOR,         "Door",                   new input_seq(input_global.KEYCODE_O) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_SERVICE,      "Service",                new input_seq(input_global.KEYCODE_9) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_GAMBLE_BOOK,         "Book-Keeping",           new input_seq(input_global.KEYCODE_0) );
        }

        static void construct_core_types_poker(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_POKER_HOLD1,         "Hold 1",                 new input_seq(input_global.KEYCODE_Z) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_POKER_HOLD2,         "Hold 2",                 new input_seq(input_global.KEYCODE_X) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_POKER_HOLD3,         "Hold 3",                 new input_seq(input_global.KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_POKER_HOLD4,         "Hold 4",                 new input_seq(input_global.KEYCODE_V) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_POKER_HOLD5,         "Hold 5",                 new input_seq(input_global.KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_POKER_CANCEL,        "Cancel",                 new input_seq(input_global.KEYCODE_N) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_POKER_BET,           "Bet",                    new input_seq(input_global.KEYCODE_1) );
        }

        static void construct_core_types_slot(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_SLOT_STOP1,          "Stop Reel 1",            new input_seq(input_global.KEYCODE_X) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_SLOT_STOP2,          "Stop Reel 2",            new input_seq(input_global.KEYCODE_C) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_SLOT_STOP3,          "Stop Reel 3",            new input_seq(input_global.KEYCODE_V) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_SLOT_STOP4,          "Stop Reel 4",            new input_seq(input_global.KEYCODE_B) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 1, ioport_group.IPG_PLAYER1, ioport_type.IPT_SLOT_STOP_ALL,       "Stop All Reels",         new input_seq(input_global.KEYCODE_Z) );
        }

        static void construct_core_types_P2(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICK_UP,         "P2 Up",                  new input_seq(input_global.KEYCODE_R, input_seq.or_code, input_global.JOYCODE_Y_UP_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICK_DOWN,       "P2 Down",                new input_seq(input_global.KEYCODE_F, input_seq.or_code, input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICK_LEFT,       "P2 Left",                new input_seq(input_global.KEYCODE_D, input_seq.or_code, input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICK_RIGHT,      "P2 Right",               new input_seq(input_global.KEYCODE_G, input_seq.or_code, input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICKRIGHT_UP,    "P2 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICKRIGHT_DOWN,  "P2 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICKRIGHT_LEFT,  "P2 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICKRIGHT_RIGHT, "P2 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICKLEFT_UP,     "P2 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICKLEFT_DOWN,   "P2 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICKLEFT_LEFT,   "P2 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_JOYSTICKLEFT_RIGHT,  "P2 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON1,             "P2 Button 1",            new input_seq(input_global.KEYCODE_A, input_seq.or_code, input_global.JOYCODE_BUTTON1_INDEXED(1), input_seq.or_code, input_global.MOUSECODE_BUTTON1_INDEXED(1), input_seq.or_code, input_global.GUNCODE_BUTTON1_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON2,             "P2 Button 2",            new input_seq(input_global.KEYCODE_S, input_seq.or_code, input_global.JOYCODE_BUTTON2_INDEXED(1), input_seq.or_code, input_global.MOUSECODE_BUTTON3_INDEXED(1), input_seq.or_code, input_global.GUNCODE_BUTTON2_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON3,             "P2 Button 3",            new input_seq(input_global.KEYCODE_Q, input_seq.or_code, input_global.JOYCODE_BUTTON3_INDEXED(1), input_seq.or_code, input_global.MOUSECODE_BUTTON2_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON4,             "P2 Button 4",            new input_seq(input_global.KEYCODE_W, input_seq.or_code, input_global.JOYCODE_BUTTON4_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON5,             "P2 Button 5",            new input_seq(input_global.JOYCODE_BUTTON5_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON6,             "P2 Button 6",            new input_seq(input_global.JOYCODE_BUTTON6_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON7,             "P2 Button 7",            new input_seq(input_global.JOYCODE_BUTTON7_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON8,             "P2 Button 8",            new input_seq(input_global.JOYCODE_BUTTON8_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON9,             "P2 Button 9",            new input_seq(input_global.JOYCODE_BUTTON9_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON10,            "P2 Button 10",           new input_seq(input_global.JOYCODE_BUTTON10_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON11,            "P2 Button 11",           new input_seq(input_global.JOYCODE_BUTTON11_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON12,            "P2 Button 12",           new input_seq(input_global.JOYCODE_BUTTON12_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON13,            "P2 Button 13",           new input_seq(input_global.JOYCODE_BUTTON13_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON14,            "P2 Button 14",           new input_seq(input_global.JOYCODE_BUTTON14_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON15,            "P2 Button 15",           new input_seq(input_global.JOYCODE_BUTTON15_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_BUTTON16,            "P2 Button 16",           new input_seq(input_global.JOYCODE_BUTTON16_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_START,               "P2 Start",               new input_seq(input_global.KEYCODE_2, input_seq.or_code, input_global.JOYCODE_START_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_SELECT,              "P2 Select",              new input_seq(input_global.KEYCODE_6, input_seq.or_code, input_global.JOYCODE_SELECT_INDEXED(1)) );
        }

        static void construct_core_types_P2_mahjong(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_A,           "P2 Mahjong A",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_B,           "P2 Mahjong B",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_C,           "P2 Mahjong C",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_D,           "P2 Mahjong D",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_E,           "P2 Mahjong E",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_F,           "P2 Mahjong F",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_G,           "P2 Mahjong G",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_H,           "P2 Mahjong H",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_I,           "P2 Mahjong I",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_J,           "P2 Mahjong J",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_K,           "P2 Mahjong K",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_L,           "P2 Mahjong L",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_M,           "P2 Mahjong M",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_N,           "P2 Mahjong N",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_O,           "P2 Mahjong O",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_P,           "P2 Mahjong P",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_Q,           "P2 Mahjong Q",           new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_KAN,         "P2 Mahjong Kan",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_PON,         "P2 Mahjong Pon",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_CHI,         "P2 Mahjong Chi",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_REACH,       "P2 Mahjong Reach",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_RON,         "P2 Mahjong Ron",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_BET,         "P2 Mahjong Bet",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_LAST_CHANCE, "P2 Mahjong Last Chance", new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_SCORE,       "P2 Mahjong Score",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_DOUBLE_UP,   "P2 Mahjong Double Up",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_FLIP_FLOP,   "P2 Mahjong Flip Flop",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_BIG,         "P2 Mahjong Big",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MAHJONG_SMALL,       "P2 Mahjong Small",       new input_seq() );
        }

        static void construct_core_types_P2_hanafuda(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_HANAFUDA_A,          "P2 Hanafuda A/1",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_HANAFUDA_B,          "P2 Hanafuda B/2",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_HANAFUDA_C,          "P2 Hanafuda C/3",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_HANAFUDA_D,          "P2 Hanafuda D/4",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_HANAFUDA_E,          "P2 Hanafuda E/5",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_HANAFUDA_F,          "P2 Hanafuda F/6",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_HANAFUDA_G,          "P2 Hanafuda G/7",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_HANAFUDA_H,          "P2 Hanafuda H/8",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_HANAFUDA_YES,        "P2 Hanafuda Yes",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 2, ioport_group.IPG_PLAYER2, ioport_type.IPT_HANAFUDA_NO,         "P2 Hanafuda No",         new input_seq() );
        }

        static void construct_core_types_P3(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICK_UP,         "P3 Up",                  new input_seq(input_global.KEYCODE_I, input_seq.or_code, input_global.JOYCODE_Y_UP_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICK_DOWN,       "P3 Down",                new input_seq(input_global.KEYCODE_K, input_seq.or_code, input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICK_LEFT,       "P3 Left",                new input_seq(input_global.KEYCODE_J, input_seq.or_code, input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICK_RIGHT,      "P3 Right",               new input_seq(input_global.KEYCODE_L, input_seq.or_code, input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICKRIGHT_UP,    "P3 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICKRIGHT_DOWN,  "P3 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICKRIGHT_LEFT,  "P3 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICKRIGHT_RIGHT, "P3 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICKLEFT_UP,     "P3 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICKLEFT_DOWN,   "P3 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICKLEFT_LEFT,   "P3 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_JOYSTICKLEFT_RIGHT,  "P3 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON1,             "P3 Button 1",            new input_seq(input_global.KEYCODE_RCONTROL, input_seq.or_code, input_global.JOYCODE_BUTTON1_INDEXED(2), input_seq.or_code, input_global.GUNCODE_BUTTON1_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON2,             "P3 Button 2",            new input_seq(input_global.KEYCODE_RSHIFT, input_seq.or_code, input_global.JOYCODE_BUTTON2_INDEXED(2), input_seq.or_code, input_global.GUNCODE_BUTTON2_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON3,             "P3 Button 3",            new input_seq(input_global.KEYCODE_ENTER, input_seq.or_code, input_global.JOYCODE_BUTTON3_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON4,             "P3 Button 4",            new input_seq(input_global.JOYCODE_BUTTON4_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON5,             "P3 Button 5",            new input_seq(input_global.JOYCODE_BUTTON5_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON6,             "P3 Button 6",            new input_seq(input_global.JOYCODE_BUTTON6_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON7,             "P3 Button 7",            new input_seq(input_global.JOYCODE_BUTTON7_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON8,             "P3 Button 8",            new input_seq(input_global.JOYCODE_BUTTON8_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON9,             "P3 Button 9",            new input_seq(input_global.JOYCODE_BUTTON9_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON10,            "P3 Button 10",           new input_seq(input_global.JOYCODE_BUTTON10_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON11,            "P3 Button 11",           new input_seq(input_global.JOYCODE_BUTTON11_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON12,            "P3 Button 12",           new input_seq(input_global.JOYCODE_BUTTON12_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON13,            "P3 Button 13",           new input_seq(input_global.JOYCODE_BUTTON13_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON14,            "P3 Button 14",           new input_seq(input_global.JOYCODE_BUTTON14_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON15,            "P3 Button 15",           new input_seq(input_global.JOYCODE_BUTTON15_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_BUTTON16,            "P3 Button 16",           new input_seq(input_global.JOYCODE_BUTTON16_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_START,               "P3 Start",               new input_seq(input_global.KEYCODE_3, input_seq.or_code, input_global.JOYCODE_START_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 3, ioport_group.IPG_PLAYER3, ioport_type.IPT_SELECT,              "P3 Select",              new input_seq(input_global.KEYCODE_7, input_seq.or_code, input_global.JOYCODE_SELECT_INDEXED(2)) );
        }

        static void construct_core_types_P4(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICK_UP,         "P4 Up",                  new input_seq(input_global.KEYCODE_8_PAD, input_seq.or_code, input_global.JOYCODE_Y_UP_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICK_DOWN,       "P4 Down",                new input_seq(input_global.KEYCODE_2_PAD, input_seq.or_code, input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICK_LEFT,       "P4 Left",                new input_seq(input_global.KEYCODE_4_PAD, input_seq.or_code, input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICK_RIGHT,      "P4 Right",               new input_seq(input_global.KEYCODE_6_PAD, input_seq.or_code, input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICKRIGHT_UP,    "P4 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICKRIGHT_DOWN,  "P4 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICKRIGHT_LEFT,  "P4 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICKRIGHT_RIGHT, "P4 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICKLEFT_UP,     "P4 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICKLEFT_DOWN,   "P4 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICKLEFT_LEFT,   "P4 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_JOYSTICKLEFT_RIGHT,  "P4 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON1,             "P4 Button 1",            new input_seq(input_global.KEYCODE_0_PAD, input_seq.or_code, input_global.JOYCODE_BUTTON1_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON2,             "P4 Button 2",            new input_seq(input_global.KEYCODE_DEL_PAD, input_seq.or_code, input_global.JOYCODE_BUTTON2_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON3,             "P4 Button 3",            new input_seq(input_global.KEYCODE_ENTER_PAD, input_seq.or_code, input_global.JOYCODE_BUTTON3_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON4,             "P4 Button 4",            new input_seq(input_global.JOYCODE_BUTTON4_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON5,             "P4 Button 5",            new input_seq(input_global.JOYCODE_BUTTON5_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON6,             "P4 Button 6",            new input_seq(input_global.JOYCODE_BUTTON6_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON7,             "P4 Button 7",            new input_seq(input_global.JOYCODE_BUTTON7_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON8,             "P4 Button 8",            new input_seq(input_global.JOYCODE_BUTTON8_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON9,             "P4 Button 9",            new input_seq(input_global.JOYCODE_BUTTON9_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON10,            "P4 Button 10",           new input_seq(input_global.JOYCODE_BUTTON10_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON11,            "P4 Button 11",           new input_seq(input_global.JOYCODE_BUTTON11_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON12,            "P4 Button 12",           new input_seq(input_global.JOYCODE_BUTTON12_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON13,            "P4 Button 13",           new input_seq(input_global.JOYCODE_BUTTON13_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON14,            "P4 Button 14",           new input_seq(input_global.JOYCODE_BUTTON14_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON15,            "P4 Button 15",           new input_seq(input_global.JOYCODE_BUTTON15_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_BUTTON16,            "P4 Button 16",           new input_seq(input_global.JOYCODE_BUTTON16_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_START,               "P4 Start",               new input_seq(input_global.KEYCODE_4, input_seq.or_code, input_global.JOYCODE_START_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 4, ioport_group.IPG_PLAYER4, ioport_type.IPT_SELECT,              "P4 Select",              new input_seq(input_global.KEYCODE_8, input_seq.or_code, input_global.JOYCODE_SELECT_INDEXED(3)) );
        }

        static void construct_core_types_P5(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICK_UP,         "P5 Up",                  new input_seq(input_global.JOYCODE_Y_UP_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICK_DOWN,       "P5 Down",                new input_seq(input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICK_LEFT,       "P5 Left",                new input_seq(input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICK_RIGHT,      "P5 Right",               new input_seq(input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICKRIGHT_UP,    "P5 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICKRIGHT_DOWN,  "P5 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICKRIGHT_LEFT,  "P5 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICKRIGHT_RIGHT, "P5 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICKLEFT_UP,     "P5 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICKLEFT_DOWN,   "P5 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICKLEFT_LEFT,   "P5 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_JOYSTICKLEFT_RIGHT,  "P5 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON1,             "P5 Button 1",            new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON2,             "P5 Button 2",            new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON3,             "P5 Button 3",            new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON4,             "P5 Button 4",            new input_seq(input_global.JOYCODE_BUTTON4_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON5,             "P5 Button 5",            new input_seq(input_global.JOYCODE_BUTTON5_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON6,             "P5 Button 6",            new input_seq(input_global.JOYCODE_BUTTON6_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON7,             "P5 Button 7",            new input_seq(input_global.JOYCODE_BUTTON7_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON8,             "P5 Button 8",            new input_seq(input_global.JOYCODE_BUTTON8_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON9,             "P5 Button 9",            new input_seq(input_global.JOYCODE_BUTTON9_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON10,            "P5 Button 10",           new input_seq(input_global.JOYCODE_BUTTON10_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON11,            "P5 Button 11",           new input_seq(input_global.JOYCODE_BUTTON11_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON12,            "P5 Button 12",           new input_seq(input_global.JOYCODE_BUTTON12_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON13,            "P5 Button 13",           new input_seq(input_global.JOYCODE_BUTTON13_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON14,            "P5 Button 14",           new input_seq(input_global.JOYCODE_BUTTON14_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON15,            "P5 Button 15",           new input_seq(input_global.JOYCODE_BUTTON15_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_BUTTON16,            "P5 Button 16",           new input_seq(input_global.JOYCODE_BUTTON16_INDEXED(4)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_START,               "P5 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 5, ioport_group.IPG_PLAYER5, ioport_type.IPT_SELECT,              "P5 Select",              new input_seq() );
        }

        static void construct_core_types_P6(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICK_UP,         "P6 Up",                  new input_seq(input_global.JOYCODE_Y_UP_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICK_DOWN,       "P6 Down",                new input_seq(input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICK_LEFT,       "P6 Left",                new input_seq(input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICK_RIGHT,      "P6 Right",               new input_seq(input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICKRIGHT_UP,    "P6 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICKRIGHT_DOWN,  "P6 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICKRIGHT_LEFT,  "P6 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICKRIGHT_RIGHT, "P6 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICKLEFT_UP,     "P6 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICKLEFT_DOWN,   "P6 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICKLEFT_LEFT,   "P6 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_JOYSTICKLEFT_RIGHT,  "P6 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON1,             "P6 Button 1",            new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON2,             "P6 Button 2",            new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON3,             "P6 Button 3",            new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON4,             "P6 Button 4",            new input_seq(input_global.JOYCODE_BUTTON4_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON5,             "P6 Button 5",            new input_seq(input_global.JOYCODE_BUTTON5_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON6,             "P6 Button 6",            new input_seq(input_global.JOYCODE_BUTTON6_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON7,             "P6 Button 7",            new input_seq(input_global.JOYCODE_BUTTON7_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON8,             "P6 Button 8",            new input_seq(input_global.JOYCODE_BUTTON8_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON9,             "P6 Button 9",            new input_seq(input_global.JOYCODE_BUTTON9_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON10,            "P6 Button 10",           new input_seq(input_global.JOYCODE_BUTTON10_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON11,            "P6 Button 11",           new input_seq(input_global.JOYCODE_BUTTON11_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON12,            "P6 Button 12",           new input_seq(input_global.JOYCODE_BUTTON12_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON13,            "P6 Button 13",           new input_seq(input_global.JOYCODE_BUTTON13_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON14,            "P6 Button 14",           new input_seq(input_global.JOYCODE_BUTTON14_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON15,            "P6 Button 15",           new input_seq(input_global.JOYCODE_BUTTON15_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_BUTTON16,            "P6 Button 16",           new input_seq(input_global.JOYCODE_BUTTON16_INDEXED(5)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_START,               "P6 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 6, ioport_group.IPG_PLAYER6, ioport_type.IPT_SELECT,              "P6 Select",              new input_seq() );
        }

        static void construct_core_types_P7(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICK_UP,         "P7 Up",                  new input_seq(input_global.JOYCODE_Y_UP_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICK_DOWN,       "P7 Down",                new input_seq(input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICK_LEFT,       "P7 Left",                new input_seq(input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICK_RIGHT,      "P7 Right",               new input_seq(input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICKRIGHT_UP,    "P7 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICKRIGHT_DOWN,  "P7 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICKRIGHT_LEFT,  "P7 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICKRIGHT_RIGHT, "P7 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICKLEFT_UP,     "P7 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICKLEFT_DOWN,   "P7 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICKLEFT_LEFT,   "P7 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_JOYSTICKLEFT_RIGHT,  "P7 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON1,             "P7 Button 1",            new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON2,             "P7 Button 2",            new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON3,             "P7 Button 3",            new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON4,             "P7 Button 4",            new input_seq(input_global.JOYCODE_BUTTON4_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON5,             "P7 Button 5",            new input_seq(input_global.JOYCODE_BUTTON5_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON6,             "P7 Button 6",            new input_seq(input_global.JOYCODE_BUTTON6_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON7,             "P7 Button 7",            new input_seq(input_global.JOYCODE_BUTTON7_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON8,             "P7 Button 8",            new input_seq(input_global.JOYCODE_BUTTON8_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON9,             "P7 Button 9",            new input_seq(input_global.JOYCODE_BUTTON9_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON10,            "P7 Button 10",           new input_seq(input_global.JOYCODE_BUTTON10_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON11,            "P7 Button 11",           new input_seq(input_global.JOYCODE_BUTTON11_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON12,            "P7 Button 12",           new input_seq(input_global.JOYCODE_BUTTON12_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON13,            "P7 Button 13",           new input_seq(input_global.JOYCODE_BUTTON13_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON14,            "P7 Button 14",           new input_seq(input_global.JOYCODE_BUTTON14_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON15,            "P7 Button 15",           new input_seq(input_global.JOYCODE_BUTTON15_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_BUTTON16,            "P7 Button 16",           new input_seq(input_global.JOYCODE_BUTTON16_INDEXED(6)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_START,               "P7 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 7, ioport_group.IPG_PLAYER7, ioport_type.IPT_SELECT,              "P7 Select",              new input_seq() );
        }

        static void construct_core_types_P8(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICK_UP,         "P8 Up",                  new input_seq(input_global.JOYCODE_Y_UP_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICK_DOWN,       "P8 Down",                new input_seq(input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICK_LEFT,       "P8 Left",                new input_seq(input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICK_RIGHT,      "P8 Right",               new input_seq(input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICKRIGHT_UP,    "P8 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICKRIGHT_DOWN,  "P8 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICKRIGHT_LEFT,  "P8 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICKRIGHT_RIGHT, "P8 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICKLEFT_UP,     "P8 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICKLEFT_DOWN,   "P8 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICKLEFT_LEFT,   "P8 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_JOYSTICKLEFT_RIGHT,  "P8 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON1,             "P8 Button 1",            new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON2,             "P8 Button 2",            new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON3,             "P8 Button 3",            new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON4,             "P8 Button 4",            new input_seq(input_global.JOYCODE_BUTTON4_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON5,             "P8 Button 5",            new input_seq(input_global.JOYCODE_BUTTON5_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON6,             "P8 Button 6",            new input_seq(input_global.JOYCODE_BUTTON6_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON7,             "P8 Button 7",            new input_seq(input_global.JOYCODE_BUTTON7_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON8,             "P8 Button 8",            new input_seq(input_global.JOYCODE_BUTTON8_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON9,             "P8 Button 9",            new input_seq(input_global.JOYCODE_BUTTON9_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON10,            "P8 Button 10",           new input_seq(input_global.JOYCODE_BUTTON10_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON11,            "P8 Button 11",           new input_seq(input_global.JOYCODE_BUTTON11_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON12,            "P8 Button 12",           new input_seq(input_global.JOYCODE_BUTTON12_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON13,            "P8 Button 13",           new input_seq(input_global.JOYCODE_BUTTON13_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON14,            "P8 Button 14",           new input_seq(input_global.JOYCODE_BUTTON14_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON15,            "P8 Button 15",           new input_seq(input_global.JOYCODE_BUTTON15_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_BUTTON16,            "P8 Button 16",           new input_seq(input_global.JOYCODE_BUTTON16_INDEXED(7)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_START,               "P8 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 8, ioport_group.IPG_PLAYER8, ioport_type.IPT_SELECT,              "P8 Select",              new input_seq() );
        }

        static void construct_core_types_P9(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICK_UP,         "P9 Up",                  new input_seq(input_global.JOYCODE_Y_UP_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICK_DOWN,       "P9 Down",                new input_seq(input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICK_LEFT,       "P9 Left",                new input_seq(input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICK_RIGHT,      "P9 Right",               new input_seq(input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICKRIGHT_UP,    "P9 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICKRIGHT_DOWN,  "P9 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICKRIGHT_LEFT,  "P9 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICKRIGHT_RIGHT, "P9 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICKLEFT_UP,     "P9 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICKLEFT_DOWN,   "P9 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICKLEFT_LEFT,   "P9 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_JOYSTICKLEFT_RIGHT,  "P9 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON1,             "P9 Button 1",            new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON2,             "P9 Button 2",            new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON3,             "P9 Button 3",            new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON4,             "P9 Button 4",            new input_seq(input_global.JOYCODE_BUTTON4_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON5,             "P9 Button 5",            new input_seq(input_global.JOYCODE_BUTTON5_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON6,             "P9 Button 6",            new input_seq(input_global.JOYCODE_BUTTON6_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON7,             "P9 Button 7",            new input_seq(input_global.JOYCODE_BUTTON7_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON8,             "P9 Button 8",            new input_seq(input_global.JOYCODE_BUTTON8_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON9,             "P9 Button 9",            new input_seq(input_global.JOYCODE_BUTTON9_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON10,            "P9 Button 10",           new input_seq(input_global.JOYCODE_BUTTON10_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON11,            "P9 Button 11",           new input_seq(input_global.JOYCODE_BUTTON11_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON12,            "P9 Button 12",           new input_seq(input_global.JOYCODE_BUTTON12_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON13,            "P9 Button 13",           new input_seq(input_global.JOYCODE_BUTTON13_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON14,            "P9 Button 14",           new input_seq(input_global.JOYCODE_BUTTON14_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON15,            "P9 Button 15",           new input_seq(input_global.JOYCODE_BUTTON15_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_BUTTON16,            "P9 Button 16",           new input_seq(input_global.JOYCODE_BUTTON16_INDEXED(8)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_START,               "P9 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 9, ioport_group.IPG_PLAYER9, ioport_type.IPT_SELECT,              "P9 Select",              new input_seq() );
        }

        static void construct_core_types_P10(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICK_UP,         "P10 Up",                  new input_seq(input_global.JOYCODE_Y_UP_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICK_DOWN,       "P10 Down",                new input_seq(input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICK_LEFT,       "P10 Left",                new input_seq(input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICK_RIGHT,      "P10 Right",               new input_seq(input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICKRIGHT_UP,    "P10 Right Stick/Up",      new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICKRIGHT_DOWN,  "P10 Right Stick/Down",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICKRIGHT_LEFT,  "P10 Right Stick/Left",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICKRIGHT_RIGHT, "P10 Right Stick/Right",   new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICKLEFT_UP,     "P10 Left Stick/Up",       new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICKLEFT_DOWN,   "P10 Left Stick/Down",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICKLEFT_LEFT,   "P10 Left Stick/Left",     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_JOYSTICKLEFT_RIGHT,  "P10 Left Stick/Right",    new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON1,             "P10 Button 1",            new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON2,             "P10 Button 2",            new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON3,             "P10 Button 3",            new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON4,             "P10 Button 4",            new input_seq(input_global.JOYCODE_BUTTON4_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON5,             "P10 Button 5",            new input_seq(input_global.JOYCODE_BUTTON5_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON6,             "P10 Button 6",            new input_seq(input_global.JOYCODE_BUTTON6_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON7,             "P10 Button 7",            new input_seq(input_global.JOYCODE_BUTTON7_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON8,             "P10 Button 8",            new input_seq(input_global.JOYCODE_BUTTON8_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON9,             "P10 Button 9",            new input_seq(input_global.JOYCODE_BUTTON9_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON10,            "P10 Button 10",           new input_seq(input_global.JOYCODE_BUTTON10_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON11,            "P10 Button 11",           new input_seq(input_global.JOYCODE_BUTTON11_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON12,            "P10 Button 12",           new input_seq(input_global.JOYCODE_BUTTON12_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON13,            "P10 Button 13",           new input_seq(input_global.JOYCODE_BUTTON13_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON14,            "P10 Button 14",           new input_seq(input_global.JOYCODE_BUTTON14_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON15,            "P10 Button 15",           new input_seq(input_global.JOYCODE_BUTTON15_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_BUTTON16,            "P10 Button 16",           new input_seq(input_global.JOYCODE_BUTTON16_INDEXED(9)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_START,               "P10 Start",               new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_SELECT,              "P10 Select",              new input_seq() );
        }

        static void construct_core_types_start(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_START1,              "1 Player Start",         new input_seq(input_global.KEYCODE_1, input_seq.or_code, input_global.JOYCODE_START_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_START2,              "2 Players Start",        new input_seq(input_global.KEYCODE_2, input_seq.or_code, input_global.JOYCODE_START_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_START3,              "3 Players Start",        new input_seq(input_global.KEYCODE_3, input_seq.or_code, input_global.JOYCODE_START_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_START4,              "4 Players Start",        new input_seq(input_global.KEYCODE_4, input_seq.or_code, input_global.JOYCODE_START_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_START5,              "5 Players Start",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_START6,              "6 Players Start",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_START7,              "7 Players Start",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_START8,              "8 Players Start",        new input_seq() );
        }

        static void construct_core_types_coin(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN1,               "Coin 1",                 new input_seq(input_global.KEYCODE_5, input_seq.or_code, input_global.JOYCODE_SELECT_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN2,               "Coin 2",                 new input_seq(input_global.KEYCODE_6, input_seq.or_code, input_global.JOYCODE_SELECT_INDEXED(1)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN3,               "Coin 3",                 new input_seq(input_global.KEYCODE_7, input_seq.or_code, input_global.JOYCODE_SELECT_INDEXED(2)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN4,               "Coin 4",                 new input_seq(input_global.KEYCODE_8, input_seq.or_code, input_global.JOYCODE_SELECT_INDEXED(3)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN5,               "Coin 5",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN6,               "Coin 6",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN7,               "Coin 7",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN8,               "Coin 8",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN9,               "Coin 9",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN10,              "Coin 10",                new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN11,              "Coin 11",                new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_COIN12,              "Coin 12",                new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_BILL1,               "Bill 1",                 new input_seq(input_global.KEYCODE_BACKSPACE) );
        }

        static void construct_core_types_service(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_SERVICE1,            "Service 1",              new input_seq(input_global.KEYCODE_9) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_SERVICE2,            "Service 2",              new input_seq(input_global.KEYCODE_0) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_SERVICE3,            "Service 3",              new input_seq(input_global.KEYCODE_MINUS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_SERVICE4,            "Service 4",              new input_seq(input_global.KEYCODE_EQUALS) );
        }

        static void construct_core_types_tilt(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_TILT1,               "Tilt 1",                 new input_seq(input_global.KEYCODE_T) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_TILT2,               "Tilt 2",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_TILT3,               "Tilt 3",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_TILT4,               "Tilt 4",                 new input_seq() );
        }

        static void construct_core_types_other(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_POWER_ON,            "Power On",               new input_seq(input_global.KEYCODE_F1) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_POWER_OFF,           "Power Off",              new input_seq(input_global.KEYCODE_F2) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_SERVICE,             "Service",                new input_seq(input_global.KEYCODE_F2) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_TILT,                "Tilt",                   new input_seq(input_global.KEYCODE_T) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_INTERLOCK,           "Door Interlock",         new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_MEMORY_RESET,        "Memory Reset",           new input_seq(input_global.KEYCODE_F1) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_VOLUME_DOWN,         "Volume Down",            new input_seq(input_global.KEYCODE_MINUS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_VOLUME_UP,           "Volume Up",              new input_seq(input_global.KEYCODE_EQUALS) );
        }

        static void construct_core_types_pedal(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_PEDAL,               "P1 Pedal 1",             new input_seq(input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(0)), new input_seq(), new input_seq(input_global.KEYCODE_LCONTROL, input_seq.or_code, input_global.JOYCODE_BUTTON1_INDEXED(0)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_PEDAL,               "P2 Pedal 1",             new input_seq(input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(1)), new input_seq(), new input_seq(input_global.KEYCODE_A, input_seq.or_code, input_global.JOYCODE_BUTTON1_INDEXED(1)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_PEDAL,               "P3 Pedal 1",             new input_seq(input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(2)), new input_seq(), new input_seq(input_global.KEYCODE_RCONTROL, input_seq.or_code, input_global.JOYCODE_BUTTON1_INDEXED(2)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_PEDAL,               "P4 Pedal 1",             new input_seq(input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(3)), new input_seq(), new input_seq(input_global.KEYCODE_0_PAD, input_seq.or_code, input_global.JOYCODE_BUTTON1_INDEXED(3)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_PEDAL,               "P5 Pedal 1",             new input_seq(input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(4)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(4)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_PEDAL,               "P6 Pedal 1",             new input_seq(input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(5)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(5)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_PEDAL,               "P7 Pedal 1",             new input_seq(input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(6)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(6)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_PEDAL,               "P8 Pedal 1",             new input_seq(input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(7)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(7)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_PEDAL,               "P9 Pedal 1",             new input_seq(input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(8)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(8)) );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_PEDAL,              "P10 Pedal 1",            new input_seq(input_global.JOYCODE_Z_NEG_ABSOLUTE_INDEXED(9)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON1_INDEXED(9)) );
        }

        static void construct_core_types_pedal2(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_PEDAL2,              "P1 Pedal 2",             new input_seq(input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(0)), new input_seq(), new input_seq(input_global.KEYCODE_LALT, input_seq.or_code, input_global.JOYCODE_BUTTON2_INDEXED(0)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_PEDAL2,              "P2 Pedal 2",             new input_seq(input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(1)), new input_seq(), new input_seq(input_global.KEYCODE_S, input_seq.or_code, input_global.JOYCODE_BUTTON2_INDEXED(1)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_PEDAL2,              "P3 Pedal 2",             new input_seq(input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(2)), new input_seq(), new input_seq(input_global.KEYCODE_RSHIFT, input_seq.or_code, input_global.JOYCODE_BUTTON2_INDEXED(2)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_PEDAL2,              "P4 Pedal 2",             new input_seq(input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(3)), new input_seq(), new input_seq(input_global.KEYCODE_DEL_PAD, input_seq.or_code, input_global.JOYCODE_BUTTON2_INDEXED(3)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_PEDAL2,              "P5 Pedal 2",             new input_seq(input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(4)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(4)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_PEDAL2,              "P6 Pedal 2",             new input_seq(input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(5)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(5)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_PEDAL2,              "P7 Pedal 2",             new input_seq(input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(6)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(6)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_PEDAL2,              "P8 Pedal 2",             new input_seq(input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(7)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(7)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_PEDAL2,              "P9 Pedal 2",             new input_seq(input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(8)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(8)) );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_PEDAL2,             "P10 Pedal 2",            new input_seq(input_global.JOYCODE_W_NEG_ABSOLUTE_INDEXED(9)), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON2_INDEXED(9)) );
        }

        static void construct_core_types_pedal3(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_PEDAL3,              "P1 Pedal 3",             new input_seq(), new input_seq(), new input_seq(input_global.KEYCODE_SPACE, input_seq.or_code, input_global.JOYCODE_BUTTON3_INDEXED(0)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_PEDAL3,              "P2 Pedal 3",             new input_seq(), new input_seq(), new input_seq(input_global.KEYCODE_Q, input_seq.or_code, input_global.JOYCODE_BUTTON3_INDEXED(1)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_PEDAL3,              "P3 Pedal 3",             new input_seq(), new input_seq(), new input_seq(input_global.KEYCODE_ENTER, input_seq.or_code, input_global.JOYCODE_BUTTON3_INDEXED(2)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_PEDAL3,              "P4 Pedal 3",             new input_seq(), new input_seq(), new input_seq(input_global.KEYCODE_ENTER_PAD, input_seq.or_code, input_global.JOYCODE_BUTTON3_INDEXED(3)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_PEDAL3,              "P5 Pedal 3",             new input_seq(), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(4)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_PEDAL3,              "P6 Pedal 3",             new input_seq(), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(5)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_PEDAL3,              "P7 Pedal 3",             new input_seq(), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(6)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_PEDAL3,              "P8 Pedal 3",             new input_seq(), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(7)) );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_PEDAL3,              "P9 Pedal 3",             new input_seq(), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(8)) );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_PEDAL3,             "P10 Pedal 3",            new input_seq(), new input_seq(), new input_seq(input_global.JOYCODE_BUTTON3_INDEXED(9)) );
        }

        static void construct_core_types_paddle(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_PADDLE,              "Paddle",                 new input_seq(input_global.JOYCODE_X_INDEXED(0), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(0)), new input_seq(input_global.KEYCODE_LEFT), new input_seq(input_global.KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_PADDLE,              "Paddle 2",               new input_seq(input_global.JOYCODE_X_INDEXED(1), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(1)), new input_seq(input_global.KEYCODE_D), new input_seq(input_global.KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_PADDLE,              "Paddle 3",               new input_seq(input_global.JOYCODE_X_INDEXED(2), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(2)), new input_seq(input_global.KEYCODE_J), new input_seq(input_global.KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_PADDLE,              "Paddle 4",               new input_seq(input_global.JOYCODE_X_INDEXED(3), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_PADDLE,              "Paddle 5",               new input_seq(input_global.JOYCODE_X_INDEXED(4), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_PADDLE,              "Paddle 6",               new input_seq(input_global.JOYCODE_X_INDEXED(5), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_PADDLE,              "Paddle 7",               new input_seq(input_global.JOYCODE_X_INDEXED(6), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_PADDLE,              "Paddle 8",               new input_seq(input_global.JOYCODE_X_INDEXED(7), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_PADDLE,              "Paddle 9",               new input_seq(input_global.JOYCODE_X_INDEXED(8), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_PADDLE,             "Paddle 10",              new input_seq(input_global.JOYCODE_X_INDEXED(9), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_paddle_v(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_PADDLE_V,            "Paddle V",               new input_seq(input_global.JOYCODE_Y_INDEXED(0), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(0)), new input_seq(input_global.KEYCODE_UP), new input_seq(input_global.KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_PADDLE_V,            "Paddle V 2",             new input_seq(input_global.JOYCODE_Y_INDEXED(1), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(1)), new input_seq(input_global.KEYCODE_R), new input_seq(input_global.KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_PADDLE_V,            "Paddle V 3",             new input_seq(input_global.JOYCODE_Y_INDEXED(2), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(2)), new input_seq(input_global.KEYCODE_I), new input_seq(input_global.KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_PADDLE_V,            "Paddle V 4",             new input_seq(input_global.JOYCODE_Y_INDEXED(3), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_PADDLE_V,            "Paddle V 5",             new input_seq(input_global.JOYCODE_Y_INDEXED(4), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_PADDLE_V,            "Paddle V 6",             new input_seq(input_global.JOYCODE_Y_INDEXED(5), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_PADDLE_V,            "Paddle V 7",             new input_seq(input_global.JOYCODE_Y_INDEXED(6), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_PADDLE_V,            "Paddle V 8",             new input_seq(input_global.JOYCODE_Y_INDEXED(7), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_PADDLE_V,            "Paddle V 9",             new input_seq(input_global.JOYCODE_Y_INDEXED(8), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_PADDLE_V,           "Paddle V 10",            new input_seq(input_global.JOYCODE_Y_INDEXED(9), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_positional(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_POSITIONAL,          "Positional",             new input_seq(input_global.MOUSECODE_X_INDEXED(0), input_seq.or_code, input_global.JOYCODE_X_INDEXED(0)), new input_seq(input_global.KEYCODE_LEFT), new input_seq(input_global.KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_POSITIONAL,          "Positional 2",           new input_seq(input_global.MOUSECODE_X_INDEXED(1), input_seq.or_code, input_global.JOYCODE_X_INDEXED(1)), new input_seq(input_global.KEYCODE_D), new input_seq(input_global.KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_POSITIONAL,          "Positional 3",           new input_seq(input_global.MOUSECODE_X_INDEXED(2), input_seq.or_code, input_global.JOYCODE_X_INDEXED(2)), new input_seq(input_global.KEYCODE_J), new input_seq(input_global.KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_POSITIONAL,          "Positional 4",           new input_seq(input_global.MOUSECODE_X_INDEXED(3), input_seq.or_code, input_global.JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_POSITIONAL,          "Positional 5",           new input_seq(input_global.MOUSECODE_X_INDEXED(4), input_seq.or_code, input_global.JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_POSITIONAL,          "Positional 6",           new input_seq(input_global.MOUSECODE_X_INDEXED(5), input_seq.or_code, input_global.JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_POSITIONAL,          "Positional 7",           new input_seq(input_global.MOUSECODE_X_INDEXED(6), input_seq.or_code, input_global.JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_POSITIONAL,          "Positional 8",           new input_seq(input_global.MOUSECODE_X_INDEXED(7), input_seq.or_code, input_global.JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_POSITIONAL,          "Positional 9",           new input_seq(input_global.MOUSECODE_X_INDEXED(8), input_seq.or_code, input_global.JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_POSITIONAL,         "Positional 10",          new input_seq(input_global.MOUSECODE_X_INDEXED(9), input_seq.or_code, input_global.JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_positional_v(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_POSITIONAL_V,        "Positional V",           new input_seq(input_global.MOUSECODE_Y_INDEXED(0), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(0)), new input_seq(input_global.KEYCODE_UP), new input_seq(input_global.KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_POSITIONAL_V,        "Positional V 2",         new input_seq(input_global.MOUSECODE_Y_INDEXED(1), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(1)), new input_seq(input_global.KEYCODE_R), new input_seq(input_global.KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_POSITIONAL_V,        "Positional V 3",         new input_seq(input_global.MOUSECODE_Y_INDEXED(2), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(2)), new input_seq(input_global.KEYCODE_I), new input_seq(input_global.KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_POSITIONAL_V,        "Positional V 4",         new input_seq(input_global.MOUSECODE_Y_INDEXED(3), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_POSITIONAL_V,        "Positional V 5",         new input_seq(input_global.MOUSECODE_Y_INDEXED(4), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_POSITIONAL_V,        "Positional V 6",         new input_seq(input_global.MOUSECODE_Y_INDEXED(5), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_POSITIONAL_V,        "Positional V 7",         new input_seq(input_global.MOUSECODE_Y_INDEXED(6), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_POSITIONAL_V,        "Positional V 8",         new input_seq(input_global.MOUSECODE_Y_INDEXED(7), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_POSITIONAL_V,        "Positional V 9",         new input_seq(input_global.MOUSECODE_Y_INDEXED(8), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_POSITIONAL_V,       "Positional V 10",        new input_seq(input_global.MOUSECODE_Y_INDEXED(9), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_dial(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_DIAL,                "Dial",                   new input_seq(input_global.MOUSECODE_X_INDEXED(0), input_seq.or_code, input_global.JOYCODE_X_INDEXED(0)), new input_seq(input_global.KEYCODE_LEFT), new input_seq(input_global.KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_DIAL,                "Dial 2",                 new input_seq(input_global.MOUSECODE_X_INDEXED(1), input_seq.or_code, input_global.JOYCODE_X_INDEXED(1)), new input_seq(input_global.KEYCODE_D), new input_seq(input_global.KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_DIAL,                "Dial 3",                 new input_seq(input_global.MOUSECODE_X_INDEXED(2), input_seq.or_code, input_global.JOYCODE_X_INDEXED(2)), new input_seq(input_global.KEYCODE_J), new input_seq(input_global.KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_DIAL,                "Dial 4",                 new input_seq(input_global.MOUSECODE_X_INDEXED(3), input_seq.or_code, input_global.JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_DIAL,                "Dial 5",                 new input_seq(input_global.MOUSECODE_X_INDEXED(4), input_seq.or_code, input_global.JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_DIAL,                "Dial 6",                 new input_seq(input_global.MOUSECODE_X_INDEXED(5), input_seq.or_code, input_global.JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_DIAL,                "Dial 7",                 new input_seq(input_global.MOUSECODE_X_INDEXED(6), input_seq.or_code, input_global.JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_DIAL,                "Dial 8",                 new input_seq(input_global.MOUSECODE_X_INDEXED(7), input_seq.or_code, input_global.JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_DIAL,                "Dial 9",                 new input_seq(input_global.MOUSECODE_X_INDEXED(8), input_seq.or_code, input_global.JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_DIAL,               "Dial 10",                new input_seq(input_global.MOUSECODE_X_INDEXED(9), input_seq.or_code, input_global.JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_dial_v(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_DIAL_V,              "Dial V",                 new input_seq(input_global.MOUSECODE_Y_INDEXED(0), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(0)), new input_seq(input_global.KEYCODE_UP), new input_seq(input_global.KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_DIAL_V,              "Dial V 2",               new input_seq(input_global.MOUSECODE_Y_INDEXED(1), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(1)), new input_seq(input_global.KEYCODE_R), new input_seq(input_global.KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_DIAL_V,              "Dial V 3",               new input_seq(input_global.MOUSECODE_Y_INDEXED(2), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(2)), new input_seq(input_global.KEYCODE_I), new input_seq(input_global.KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_DIAL_V,              "Dial V 4",               new input_seq(input_global.MOUSECODE_Y_INDEXED(3), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_DIAL_V,              "Dial V 5",               new input_seq(input_global.MOUSECODE_Y_INDEXED(4), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_DIAL_V,              "Dial V 6",               new input_seq(input_global.MOUSECODE_Y_INDEXED(5), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_DIAL_V,              "Dial V 7",               new input_seq(input_global.MOUSECODE_Y_INDEXED(6), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_DIAL_V,              "Dial V 8",               new input_seq(input_global.MOUSECODE_Y_INDEXED(7), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_DIAL_V,              "Dial V 9",               new input_seq(input_global.MOUSECODE_Y_INDEXED(8), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_DIAL_V,             "Dial V 10",              new input_seq(input_global.MOUSECODE_Y_INDEXED(9), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_trackball_X(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_TRACKBALL_X,         "Track X",                new input_seq(input_global.MOUSECODE_X_INDEXED(0), input_seq.or_code, input_global.JOYCODE_X_INDEXED(0)), new input_seq(input_global.KEYCODE_LEFT), new input_seq(input_global.KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_TRACKBALL_X,         "Track X 2",              new input_seq(input_global.MOUSECODE_X_INDEXED(1), input_seq.or_code, input_global.JOYCODE_X_INDEXED(1)), new input_seq(input_global.KEYCODE_D), new input_seq(input_global.KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_TRACKBALL_X,         "Track X 3",              new input_seq(input_global.MOUSECODE_X_INDEXED(2), input_seq.or_code, input_global.JOYCODE_X_INDEXED(2)), new input_seq(input_global.KEYCODE_J), new input_seq(input_global.KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_TRACKBALL_X,         "Track X 4",              new input_seq(input_global.MOUSECODE_X_INDEXED(3), input_seq.or_code, input_global.JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_TRACKBALL_X,         "Track X 5",              new input_seq(input_global.MOUSECODE_X_INDEXED(4), input_seq.or_code, input_global.JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_TRACKBALL_X,         "Track X 6",              new input_seq(input_global.MOUSECODE_X_INDEXED(5), input_seq.or_code, input_global.JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_TRACKBALL_X,         "Track X 7",              new input_seq(input_global.MOUSECODE_X_INDEXED(6), input_seq.or_code, input_global.JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_TRACKBALL_X,         "Track X 8",              new input_seq(input_global.MOUSECODE_X_INDEXED(7), input_seq.or_code, input_global.JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_TRACKBALL_X,         "Track X 9",              new input_seq(input_global.MOUSECODE_X_INDEXED(8), input_seq.or_code, input_global.JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_TRACKBALL_X,        "Track X 10",             new input_seq(input_global.MOUSECODE_X_INDEXED(9), input_seq.or_code, input_global.JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_trackball_Y(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_TRACKBALL_Y,         "Track Y",                new input_seq(input_global.MOUSECODE_Y_INDEXED(0), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(0)), new input_seq(input_global.KEYCODE_UP), new input_seq(input_global.KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_TRACKBALL_Y,         "Track Y 2",              new input_seq(input_global.MOUSECODE_Y_INDEXED(1), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(1)), new input_seq(input_global.KEYCODE_R), new input_seq(input_global.KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_TRACKBALL_Y,         "Track Y 3",              new input_seq(input_global.MOUSECODE_Y_INDEXED(2), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(2)), new input_seq(input_global.KEYCODE_I), new input_seq(input_global.KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_TRACKBALL_Y,         "Track Y 4",              new input_seq(input_global.MOUSECODE_Y_INDEXED(3), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_TRACKBALL_Y,         "Track Y 5",              new input_seq(input_global.MOUSECODE_Y_INDEXED(4), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_TRACKBALL_Y,         "Track Y 6",              new input_seq(input_global.MOUSECODE_Y_INDEXED(5), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_TRACKBALL_Y,         "Track Y 7",              new input_seq(input_global.MOUSECODE_Y_INDEXED(6), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_TRACKBALL_Y,         "Track Y 8",              new input_seq(input_global.MOUSECODE_Y_INDEXED(7), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_TRACKBALL_Y,         "Track Y 9",              new input_seq(input_global.MOUSECODE_Y_INDEXED(8), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_TRACKBALL_Y,        "Track Y 10",             new input_seq(input_global.MOUSECODE_Y_INDEXED(9), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_AD_stick_X(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_AD_STICK_X,          "AD Stick X",             new input_seq(input_global.JOYCODE_X_INDEXED(0), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(0)), new input_seq(input_global.KEYCODE_LEFT), new input_seq(input_global.KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_AD_STICK_X,          "AD Stick X 2",           new input_seq(input_global.JOYCODE_X_INDEXED(1), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(1)), new input_seq(input_global.KEYCODE_D), new input_seq(input_global.KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_AD_STICK_X,          "AD Stick X 3",           new input_seq(input_global.JOYCODE_X_INDEXED(2), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(2)), new input_seq(input_global.KEYCODE_J), new input_seq(input_global.KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_AD_STICK_X,          "AD Stick X 4",           new input_seq(input_global.JOYCODE_X_INDEXED(3), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_AD_STICK_X,          "AD Stick X 5",           new input_seq(input_global.JOYCODE_X_INDEXED(4), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_AD_STICK_X,          "AD Stick X 6",           new input_seq(input_global.JOYCODE_X_INDEXED(5), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_AD_STICK_X,          "AD Stick X 7",           new input_seq(input_global.JOYCODE_X_INDEXED(6), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_AD_STICK_X,          "AD Stick X 8",           new input_seq(input_global.JOYCODE_X_INDEXED(7), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_AD_STICK_X,          "AD Stick X 9",           new input_seq(input_global.JOYCODE_X_INDEXED(8), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_AD_STICK_X,         "AD Stick X 10",          new input_seq(input_global.JOYCODE_X_INDEXED(9), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_AD_stick_Y(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_AD_STICK_Y,          "AD Stick Y",             new input_seq(input_global.JOYCODE_Y_INDEXED(0), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(0)), new input_seq(input_global.KEYCODE_UP), new input_seq(input_global.KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_AD_STICK_Y,          "AD Stick Y 2",           new input_seq(input_global.JOYCODE_Y_INDEXED(1), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(1)), new input_seq(input_global.KEYCODE_R), new input_seq(input_global.KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_AD_STICK_Y,          "AD Stick Y 3",           new input_seq(input_global.JOYCODE_Y_INDEXED(2), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(2)), new input_seq(input_global.KEYCODE_I), new input_seq(input_global.KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_AD_STICK_Y,          "AD Stick Y 4",           new input_seq(input_global.JOYCODE_Y_INDEXED(3), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_AD_STICK_Y,          "AD Stick Y 5",           new input_seq(input_global.JOYCODE_Y_INDEXED(4), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_AD_STICK_Y,          "AD Stick Y 6",           new input_seq(input_global.JOYCODE_Y_INDEXED(5), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_AD_STICK_Y,          "AD Stick Y 7",           new input_seq(input_global.JOYCODE_Y_INDEXED(6), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_AD_STICK_Y,          "AD Stick Y 8",           new input_seq(input_global.JOYCODE_Y_INDEXED(7), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_AD_STICK_Y,          "AD Stick Y 9",           new input_seq(input_global.JOYCODE_Y_INDEXED(8), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_AD_STICK_Y,         "AD Stick Y 10",          new input_seq(input_global.JOYCODE_Y_INDEXED(9), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_AD_stick_Z(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_AD_STICK_Z,          "AD Stick Z",             new input_seq(input_global.JOYCODE_Z_INDEXED(0)), new input_seq(input_global.KEYCODE_A), new input_seq(input_global.KEYCODE_Z) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_AD_STICK_Z,          "AD Stick Z 2",           new input_seq(input_global.JOYCODE_Z_INDEXED(1)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_AD_STICK_Z,          "AD Stick Z 3",           new input_seq(input_global.JOYCODE_Z_INDEXED(2)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_AD_STICK_Z,          "AD Stick Z 4",           new input_seq(input_global.JOYCODE_Z_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_AD_STICK_Z,          "AD Stick Z 5",           new input_seq(input_global.JOYCODE_Z_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_AD_STICK_Z,          "AD Stick Z 6",           new input_seq(input_global.JOYCODE_Z_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_AD_STICK_Z,          "AD Stick Z 7",           new input_seq(input_global.JOYCODE_Z_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_AD_STICK_Z,          "AD Stick Z 8",           new input_seq(input_global.JOYCODE_Z_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_AD_STICK_Z,          "AD Stick Z 9",           new input_seq(input_global.JOYCODE_Z_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_AD_STICK_Z,         "AD Stick Z 10",          new input_seq(input_global.JOYCODE_Z_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_lightgun_X(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_LIGHTGUN_X,          "Lightgun X",             new input_seq(input_global.GUNCODE_X_INDEXED(0), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(0), input_seq.or_code, input_global.JOYCODE_X_INDEXED(0)), new input_seq(input_global.KEYCODE_LEFT), new input_seq(input_global.KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_LIGHTGUN_X,          "Lightgun X 2",           new input_seq(input_global.GUNCODE_X_INDEXED(1), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(1), input_seq.or_code, input_global.JOYCODE_X_INDEXED(1)), new input_seq(input_global.KEYCODE_D), new input_seq(input_global.KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_LIGHTGUN_X,          "Lightgun X 3",           new input_seq(input_global.GUNCODE_X_INDEXED(2), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(2), input_seq.or_code, input_global.JOYCODE_X_INDEXED(2)), new input_seq(input_global.KEYCODE_J), new input_seq(input_global.KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_LIGHTGUN_X,          "Lightgun X 4",           new input_seq(input_global.GUNCODE_X_INDEXED(3), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(3), input_seq.or_code, input_global.JOYCODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_LIGHTGUN_X,          "Lightgun X 5",           new input_seq(input_global.GUNCODE_X_INDEXED(4), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(4), input_seq.or_code, input_global.JOYCODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_LIGHTGUN_X,          "Lightgun X 6",           new input_seq(input_global.GUNCODE_X_INDEXED(5), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(5), input_seq.or_code, input_global.JOYCODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_LIGHTGUN_X,          "Lightgun X 7",           new input_seq(input_global.GUNCODE_X_INDEXED(6), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(6), input_seq.or_code, input_global.JOYCODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_LIGHTGUN_X,          "Lightgun X 8",           new input_seq(input_global.GUNCODE_X_INDEXED(7), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(7), input_seq.or_code, input_global.JOYCODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_LIGHTGUN_X,          "Lightgun X 9",           new input_seq(input_global.GUNCODE_X_INDEXED(8), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(8), input_seq.or_code, input_global.JOYCODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_LIGHTGUN_X,         "Lightgun X 10",          new input_seq(input_global.GUNCODE_X_INDEXED(9), input_seq.or_code, input_global.MOUSECODE_X_INDEXED(9), input_seq.or_code, input_global.JOYCODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_lightgun_Y(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_LIGHTGUN_Y,          "Lightgun Y",             new input_seq(input_global.GUNCODE_Y_INDEXED(0), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(0), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(0)), new input_seq(input_global.KEYCODE_UP), new input_seq(input_global.KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_LIGHTGUN_Y,          "Lightgun Y 2",           new input_seq(input_global.GUNCODE_Y_INDEXED(1), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(1), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(1)), new input_seq(input_global.KEYCODE_R), new input_seq(input_global.KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_LIGHTGUN_Y,          "Lightgun Y 3",           new input_seq(input_global.GUNCODE_Y_INDEXED(2), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(2), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(2)), new input_seq(input_global.KEYCODE_I), new input_seq(input_global.KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_LIGHTGUN_Y,          "Lightgun Y 4",           new input_seq(input_global.GUNCODE_Y_INDEXED(3), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(3), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_LIGHTGUN_Y,          "Lightgun Y 5",           new input_seq(input_global.GUNCODE_Y_INDEXED(4), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(4), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_LIGHTGUN_Y,          "Lightgun Y 6",           new input_seq(input_global.GUNCODE_Y_INDEXED(5), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(5), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_LIGHTGUN_Y,          "Lightgun Y 7",           new input_seq(input_global.GUNCODE_Y_INDEXED(6), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(6), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_LIGHTGUN_Y,          "Lightgun Y 8",           new input_seq(input_global.GUNCODE_Y_INDEXED(7), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(7), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_LIGHTGUN_Y,          "Lightgun Y 9",           new input_seq(input_global.GUNCODE_Y_INDEXED(8), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(8), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_LIGHTGUN_Y,         "Lightgun Y 10",          new input_seq(input_global.GUNCODE_Y_INDEXED(9), input_seq.or_code, input_global.MOUSECODE_Y_INDEXED(9), input_seq.or_code, input_global.JOYCODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_mouse_X(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MOUSE_X,             "Mouse X",                new input_seq(input_global.MOUSECODE_X_INDEXED(0)), new input_seq(input_global.KEYCODE_LEFT), new input_seq(input_global.KEYCODE_RIGHT) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MOUSE_X,             "Mouse X 2",              new input_seq(input_global.MOUSECODE_X_INDEXED(1)), new input_seq(input_global.KEYCODE_D), new input_seq(input_global.KEYCODE_G) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_MOUSE_X,             "Mouse X 3",              new input_seq(input_global.MOUSECODE_X_INDEXED(2)), new input_seq(input_global.KEYCODE_J), new input_seq(input_global.KEYCODE_L) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_MOUSE_X,             "Mouse X 4",              new input_seq(input_global.MOUSECODE_X_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_MOUSE_X,             "Mouse X 5",              new input_seq(input_global.MOUSECODE_X_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_MOUSE_X,             "Mouse X 6",              new input_seq(input_global.MOUSECODE_X_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_MOUSE_X,             "Mouse X 7",              new input_seq(input_global.MOUSECODE_X_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_MOUSE_X,             "Mouse X 8",              new input_seq(input_global.MOUSECODE_X_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_MOUSE_X,             "Mouse X 9",              new input_seq(input_global.MOUSECODE_X_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_MOUSE_X,            "Mouse X 10",             new input_seq(input_global.MOUSECODE_X_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_mouse_Y(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_ANALOG_TYPE(typelist,  1, ioport_group.IPG_PLAYER1, ioport_type.IPT_MOUSE_Y,             "Mouse Y",                new input_seq(input_global.MOUSECODE_Y_INDEXED(0)), new input_seq(input_global.KEYCODE_UP), new input_seq(input_global.KEYCODE_DOWN) );
            INPUT_PORT_ANALOG_TYPE(typelist,  2, ioport_group.IPG_PLAYER2, ioport_type.IPT_MOUSE_Y,             "Mouse Y 2",              new input_seq(input_global.MOUSECODE_Y_INDEXED(1)), new input_seq(input_global.KEYCODE_R), new input_seq(input_global.KEYCODE_F) );
            INPUT_PORT_ANALOG_TYPE(typelist,  3, ioport_group.IPG_PLAYER3, ioport_type.IPT_MOUSE_Y,             "Mouse Y 3",              new input_seq(input_global.MOUSECODE_Y_INDEXED(2)), new input_seq(input_global.KEYCODE_I), new input_seq(input_global.KEYCODE_K) );
            INPUT_PORT_ANALOG_TYPE(typelist,  4, ioport_group.IPG_PLAYER4, ioport_type.IPT_MOUSE_Y,             "Mouse Y 4",              new input_seq(input_global.MOUSECODE_Y_INDEXED(3)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  5, ioport_group.IPG_PLAYER5, ioport_type.IPT_MOUSE_Y,             "Mouse Y 5",              new input_seq(input_global.MOUSECODE_Y_INDEXED(4)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  6, ioport_group.IPG_PLAYER6, ioport_type.IPT_MOUSE_Y,             "Mouse Y 6",              new input_seq(input_global.MOUSECODE_Y_INDEXED(5)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  7, ioport_group.IPG_PLAYER7, ioport_type.IPT_MOUSE_Y,             "Mouse Y 7",              new input_seq(input_global.MOUSECODE_Y_INDEXED(6)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  8, ioport_group.IPG_PLAYER8, ioport_type.IPT_MOUSE_Y,             "Mouse Y 8",              new input_seq(input_global.MOUSECODE_Y_INDEXED(7)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist,  9, ioport_group.IPG_PLAYER9, ioport_type.IPT_MOUSE_Y,             "Mouse Y 9",              new input_seq(input_global.MOUSECODE_Y_INDEXED(8)), new input_seq(), new input_seq() );
            INPUT_PORT_ANALOG_TYPE(typelist, 10, ioport_group.IPG_PLAYER10, ioport_type.IPT_MOUSE_Y,            "Mouse Y 10",             new input_seq(input_global.MOUSECODE_Y_INDEXED(9)), new input_seq(), new input_seq() );
        }

        static void construct_core_types_keypad(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_KEYPAD,              "Keypad",                 new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_OTHER,   ioport_type.IPT_KEYBOARD,            "Keyboard",               new input_seq() );
        }

        static void construct_core_types_UI(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_ON_SCREEN_DISPLAY,"On Screen Display",      new input_seq(input_global.KEYCODE_TILDE, input_seq.not_code, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_DEBUG_BREAK,      "Break in Debugger",      new input_seq(input_global.KEYCODE_TILDE, input_seq.not_code, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_CONFIGURE,        "Config Menu",            new input_seq(input_global.KEYCODE_TAB) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_PAUSE,            "Pause",                  new input_seq(input_global.KEYCODE_P, input_seq.not_code, input_global.KEYCODE_LSHIFT, input_seq.not_code, input_global.KEYCODE_RSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_PAUSE_SINGLE,     "Pause - Single Step",    new input_seq(input_global.KEYCODE_P, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_REWIND_SINGLE,    "Rewind - Single Step",   new input_seq(input_global.KEYCODE_TILDE, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_RESET_MACHINE,    "Reset Machine",          new input_seq(input_global.KEYCODE_F3, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_SOFT_RESET,       "Soft Reset",             new input_seq(input_global.KEYCODE_F3, input_seq.not_code, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_SHOW_GFX,         "Show Gfx",               new input_seq(input_global.KEYCODE_F4) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_FRAMESKIP_DEC,    "Frameskip Dec",          new input_seq(input_global.KEYCODE_F8) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_FRAMESKIP_INC,    "Frameskip Inc",          new input_seq(input_global.KEYCODE_F9) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_THROTTLE,         "Throttle",               new input_seq(input_global.KEYCODE_F10) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_FAST_FORWARD,     "Fast Forward",           new input_seq(input_global.KEYCODE_INSERT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_SHOW_FPS,         "Show FPS",               new input_seq(input_global.KEYCODE_F11, input_seq.not_code, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_SNAPSHOT,         "Save Snapshot",          new input_seq(input_global.KEYCODE_F12, input_seq.not_code, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_TIMECODE,         "Write current timecode", new input_seq(input_global.KEYCODE_F12, input_seq.not_code, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_RECORD_MNG,       "Record MNG",             new input_seq(input_global.KEYCODE_F12, input_global.KEYCODE_LSHIFT, input_seq.not_code, input_global.KEYCODE_LCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_RECORD_AVI,       "Record AVI",             new input_seq(input_global.KEYCODE_F12, input_global.KEYCODE_LSHIFT, input_global.KEYCODE_LCONTROL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_TOGGLE_CHEAT,     "Toggle Cheat",           new input_seq(input_global.KEYCODE_F6) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_TOGGLE_AUTOFIRE,  "Toggle Autofire",        new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_UP,               "UI Up",                  new input_seq(input_global.KEYCODE_UP, input_seq.or_code, input_global.JOYCODE_Y_UP_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_DOWN,             "UI Down",                new input_seq(input_global.KEYCODE_DOWN, input_seq.or_code, input_global.JOYCODE_Y_DOWN_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_LEFT,             "UI Left",                new input_seq(input_global.KEYCODE_LEFT, input_seq.or_code, input_global.JOYCODE_X_LEFT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_RIGHT,            "UI Right",               new input_seq(input_global.KEYCODE_RIGHT, input_seq.or_code, input_global.JOYCODE_X_RIGHT_SWITCH_INDEXED(0)) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_HOME,             "UI Home",                new input_seq(input_global.KEYCODE_HOME) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_END,              "UI End",                 new input_seq(input_global.KEYCODE_END) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_PAGE_UP,          "UI Page Up",             new input_seq(input_global.KEYCODE_PGUP) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_PAGE_DOWN,        "UI Page Down",           new input_seq(input_global.KEYCODE_PGDN) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_SELECT,           "UI Select",              new input_seq(input_global.KEYCODE_ENTER, input_seq.or_code, input_global.JOYCODE_BUTTON1_INDEXED(0), input_seq.or_code, input_global.KEYCODE_ENTER_PAD) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_CANCEL,           "UI Cancel",              new input_seq(input_global.KEYCODE_ESC) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_DISPLAY_COMMENT,  "UI Display Comment",     new input_seq(input_global.KEYCODE_SPACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_CLEAR,            "UI Clear",               new input_seq(input_global.KEYCODE_DEL) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_ZOOM_IN,          "UI Zoom In",             new input_seq(input_global.KEYCODE_EQUALS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_ZOOM_OUT,         "UI Zoom Out",            new input_seq(input_global.KEYCODE_MINUS) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_PREV_GROUP,       "UI Previous Group",      new input_seq(input_global.KEYCODE_OPENBRACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_NEXT_GROUP,       "UI Next Group",          new input_seq(input_global.KEYCODE_CLOSEBRACE) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_ROTATE,           "UI Rotate",              new input_seq(input_global.KEYCODE_R) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_SHOW_PROFILER,    "Show Profiler",          new input_seq(input_global.KEYCODE_F11, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_TOGGLE_UI,        "UI Toggle",              new input_seq(input_global.KEYCODE_SCRLOCK, input_seq.not_code, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_PASTE,            "UI Paste Text",          new input_seq(input_global.KEYCODE_SCRLOCK, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_TOGGLE_DEBUG,     "Toggle Debugger",        new input_seq(input_global.KEYCODE_F5) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_SAVE_STATE,       "Save State",             new input_seq(input_global.KEYCODE_F7, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_LOAD_STATE,       "Load State",             new input_seq(input_global.KEYCODE_F7, input_seq.not_code, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_TAPE_START,       "UI (First) Tape Start",  new input_seq(input_global.KEYCODE_F2, input_seq.not_code, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_TAPE_STOP,        "UI (First) Tape Stop",   new input_seq(input_global.KEYCODE_F2, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_DATS,             "UI External DAT View",   new input_seq(input_global.KEYCODE_LALT, input_global.KEYCODE_D) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_FAVORITES,        "UI Add/Remove favorites",new input_seq(input_global.KEYCODE_LALT, input_global.KEYCODE_F) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_EXPORT,           "UI Export list",         new input_seq(input_global.KEYCODE_LALT, input_global.KEYCODE_E) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_AUDIT_FAST,       "UI Audit Unavailable",   new input_seq(input_global.KEYCODE_F1, input_seq.not_code, input_global.KEYCODE_LSHIFT) );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_UI_AUDIT_ALL,        "UI Audit All",           new input_seq(input_global.KEYCODE_F1, input_global.KEYCODE_LSHIFT) );
        }

        static void construct_core_types_OSD(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_1,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_2,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_3,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_4,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_5,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_6,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_7,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_8,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_9,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_10,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_11,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_12,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_13,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_14,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_15,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_UI,      ioport_type.IPT_OSD_16,              null,                     new input_seq() );
        }

        static void construct_core_types_invalid(simple_list<input_type_entry> typelist)
        {
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_INVALID, ioport_type.IPT_UNKNOWN,             null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_INVALID, ioport_type.IPT_UNUSED,              null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_INVALID, ioport_type.IPT_SPECIAL,             null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_INVALID, ioport_type.IPT_OTHER,               null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_INVALID, ioport_type.IPT_ADJUSTER,            null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_INVALID, ioport_type.IPT_DIPSWITCH,           null,                     new input_seq() );
            INPUT_PORT_DIGITAL_TYPE(typelist, 0, ioport_group.IPG_INVALID, ioport_type.IPT_CONFIG,              null,                     new input_seq() );
        }


        public static void construct_core_types(simple_list<input_type_entry> typelist)
        {
            construct_core_types_P1(typelist);
            construct_core_types_P1_mahjong(typelist);
            construct_core_types_P1_hanafuda(typelist);
            construct_core_types_gamble(typelist);
            construct_core_types_poker(typelist);
            construct_core_types_slot(typelist);
            construct_core_types_P2(typelist);
            construct_core_types_P2_mahjong(typelist);
            construct_core_types_P2_hanafuda(typelist);
            construct_core_types_P3(typelist);
            construct_core_types_P4(typelist);
            construct_core_types_P5(typelist);
            construct_core_types_P6(typelist);
            construct_core_types_P7(typelist);
            construct_core_types_P8(typelist);
            construct_core_types_P9(typelist);
            construct_core_types_P10(typelist);
            construct_core_types_start(typelist);
            construct_core_types_coin(typelist);
            construct_core_types_service(typelist);
            construct_core_types_tilt(typelist);
            construct_core_types_other(typelist);
            construct_core_types_pedal(typelist);
            construct_core_types_pedal2(typelist);
            construct_core_types_pedal3(typelist);
            construct_core_types_paddle(typelist);
            construct_core_types_paddle_v(typelist);
            construct_core_types_positional(typelist);
            construct_core_types_positional_v(typelist);
            construct_core_types_dial(typelist);
            construct_core_types_dial_v(typelist);
            construct_core_types_trackball_X(typelist);
            construct_core_types_trackball_Y(typelist);
            construct_core_types_AD_stick_X(typelist);
            construct_core_types_AD_stick_Y(typelist);
            construct_core_types_AD_stick_Z(typelist);
            construct_core_types_lightgun_X(typelist);
            construct_core_types_lightgun_Y(typelist);
            construct_core_types_mouse_X(typelist);
            construct_core_types_mouse_Y(typelist);
            construct_core_types_keypad(typelist);
            construct_core_types_UI(typelist);
            construct_core_types_OSD(typelist);
            construct_core_types_invalid(typelist);
        }
    }
}
