// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using cassette_device_enumerator = mame.device_type_enumerator<mame.cassette_image_device>;  //typedef device_type_enumerator<cassette_image_device> cassette_device_enumerator;
using char32_t = System.UInt32;
using device_t_feature_type = mame.emu.detail.device_feature.type;  //using feature_type = emu::detail::device_feature::type;
using image_interface_enumerator = mame.device_interface_enumerator<mame.device_image_interface>;  //typedef device_interface_enumerator<device_image_interface> image_interface_enumerator;
using int32_t = System.Int32;
using mame_ui_manager_device_feature_set = mame.std.set<mame.std.pair<string, string>>;  //using device_feature_set = std::set<std::pair<std::string, std::string> >;
using osd_ticks_t = System.UInt64;  //typedef uint64_t osd_ticks_t;
using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using std_time_t = System.Int64;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    enum ui_callback_type
    {
        GENERAL,
        MODAL,
        MENU,
        VIEWER
    }


    public static class ui_global
    {
        //enum
        //{
        //LOADSAVE_NONE,
        //LOADSAVE_LOAD,
        //LOADSAVE_SAVE
        //}


        /* preferred font height; use ui_get_line_height() to get actual height */
        public const float UI_MAX_FONT_HEIGHT      = 1.0f / 20.0f;

        /* width of lines drawn in the UI */
        public const float UI_LINE_WIDTH           = 1.0f / 500.0f;

        /* handy colors */
        public static readonly rgb_t UI_GREEN_COLOR          = new rgb_t(0xef,0x10,0x60,0x10);
        public static readonly rgb_t UI_YELLOW_COLOR         = new rgb_t(0xef,0x60,0x60,0x10);
        public static readonly rgb_t UI_RED_COLOR            = new rgb_t(0xf0,0x60,0x10,0x10);

        /* cancel return value for a UI handler */
        public const uint32_t UI_HANDLER_CANCEL       = uint32_t.MaxValue;

        // list of natural keyboard keys that are not associated with UI_EVENT_CHARs
        public static readonly input_item_id [] non_char_keys = 
        {
            input_item_id.ITEM_ID_ESC,
            input_item_id.ITEM_ID_F1,
            input_item_id.ITEM_ID_F2,
            input_item_id.ITEM_ID_F3,
            input_item_id.ITEM_ID_F4,
            input_item_id.ITEM_ID_F5,
            input_item_id.ITEM_ID_F6,
            input_item_id.ITEM_ID_F7,
            input_item_id.ITEM_ID_F8,
            input_item_id.ITEM_ID_F9,
            input_item_id.ITEM_ID_F10,
            input_item_id.ITEM_ID_F11,
            input_item_id.ITEM_ID_F12,
            input_item_id.ITEM_ID_NUMLOCK,
            input_item_id.ITEM_ID_0_PAD,
            input_item_id.ITEM_ID_1_PAD,
            input_item_id.ITEM_ID_2_PAD,
            input_item_id.ITEM_ID_3_PAD,
            input_item_id.ITEM_ID_4_PAD,
            input_item_id.ITEM_ID_5_PAD,
            input_item_id.ITEM_ID_6_PAD,
            input_item_id.ITEM_ID_7_PAD,
            input_item_id.ITEM_ID_8_PAD,
            input_item_id.ITEM_ID_9_PAD,
            input_item_id.ITEM_ID_DEL_PAD,
            input_item_id.ITEM_ID_PLUS_PAD,
            input_item_id.ITEM_ID_MINUS_PAD,
            input_item_id.ITEM_ID_INSERT,
            input_item_id.ITEM_ID_DEL,
            input_item_id.ITEM_ID_HOME,
            input_item_id.ITEM_ID_END,
            input_item_id.ITEM_ID_PGUP,
            input_item_id.ITEM_ID_PGDN,
            input_item_id.ITEM_ID_UP,
            input_item_id.ITEM_ID_DOWN,
            input_item_id.ITEM_ID_LEFT,
            input_item_id.ITEM_ID_RIGHT,
            input_item_id.ITEM_ID_PAUSE,
            input_item_id.ITEM_ID_CANCEL
        };


        public static readonly uint32_t [] mouse_bitmap = new UInt32[32*32]
        {
            0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x09a46f30,0x81ac7c43,0x24af8049,0x00ad7d45,0x00a8753a,0x00a46f30,0x009f6725,0x009b611c,0x00985b14,0x0095560d,0x00935308,0x00915004,0x00904e02,0x008f4e01,0x008f4d00,0x008f4d00,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x00a16a29,0xa2aa783d,0xffbb864a,0xc0b0824c,0x5aaf7f48,0x09ac7b42,0x00a9773c,0x00a67134,0x00a26b2b,0x009e6522,0x009a5e19,0x00965911,0x0094550b,0x00925207,0x00915004,0x008f4e01,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x009a5e18,0x39a06827,0xffb97c34,0xffe8993c,0xffc88940,0xedac7c43,0x93ad7c44,0x2dac7c43,0x00ab793f,0x00a87438,0x00a46f30,0x00a06827,0x009c611d,0x00985c15,0x0095570e,0x00935309,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x00935308,0x00965810,0xcc9a5e19,0xffe78a21,0xfffb9929,0xfff49931,0xffd88e39,0xffb9813f,0xc9ac7c43,0x66ad7c44,0x0cac7a41,0x00a9773c,0x00a67134,0x00a26b2b,0x009e6522,0x009a5e19,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4e01,0x00904e02,0x60925106,0xffba670a,0xfff88b11,0xfff98f19,0xfff99422,0xfff9982b,0xffe89434,0xffc9883c,0xf3ac7a41,0x9cad7c44,0x39ac7c43,0x00ab7a40,0x00a87539,0x00a56f31,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008e4d00,0x008e4d00,0x098e4d00,0xea8f4d00,0xffee7f03,0xfff68407,0xfff6870d,0xfff78b15,0xfff78f1d,0xfff79426,0xfff49730,0xffd98d38,0xffbc823f,0xd2ac7c43,0x6fad7c44,0x12ac7b42,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008e4d00,0x008e4d00,0x008e4c00,0x8a8e4c00,0xffc46800,0xfff37e00,0xfff37f02,0xfff38106,0xfff3830a,0xfff48711,0xfff48b19,0xfff58f21,0xfff5942b,0xffe79134,0xffcb863b,0xf9ac7a41,0xa5ac7c43,0x3fac7c43,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008e4d00,0x008e4d00,0x008e4c00,0x218d4c00,0xfc8e4c00,0xffee7a00,0xfff07c00,0xfff17c00,0xfff17d02,0xfff17e04,0xfff18008,0xfff2830d,0xfff28614,0xfff38a1c,0xfff38f25,0xfff2932e,0xffd98b37,0xffbc813e,0xdbac7c43,0x78ad7c44,0x15ac7b42,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008e4d00,0x008e4d00,0x008e4d00,0x008e4c00,0xb18d4c00,0xffcf6b00,0xffed7900,0xffed7900,0xffee7900,0xffee7a01,0xffee7a01,0xffee7b03,0xffee7c06,0xffef7e0a,0xffef8110,0xfff08618,0xfff08a20,0xfff18f2a,0xffe78f33,0xffcc863b,0xfcab7a40,0xaeac7c43,0x4bac7c43,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x488d4c00,0xffa85800,0xffe97500,0xffea7600,0xffea7600,0xffeb7600,0xffeb7600,0xffeb7600,0xffeb7701,0xffeb7702,0xffeb7804,0xffec7a07,0xffec7d0d,0xffec8013,0xffed851c,0xffee8a25,0xffee8f2e,0xffd98937,0xffbe813d,0xe4ab7a40,0x81ab7a40,0x1ba9763b,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x008d4c00,0xdb8d4c00,0xffd86c00,0xffe77300,0xffe77300,0xffe87300,0xffe87300,0xffe87300,0xffe87300,0xffe87300,0xffe87401,0xffe87401,0xffe87503,0xffe97606,0xffe9780a,0xffe97c10,0xffea7f16,0xffeb831d,0xffeb8623,0xffe48426,0xffc67725,0xffa5661f,0xb7985c15,0x54935309,0x038e4d00,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008e4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x6f8d4c00,0xffb25b00,0xffe36f00,0xffe47000,0xffe47000,0xffe57000,0xffe57000,0xffe57000,0xffe57000,0xffe57000,0xffe57000,0xffe57000,0xffe57000,0xffe57101,0xffe57000,0xffe47000,0xffe16e00,0xffde6c00,0xffd86900,0xffd06600,0xffc76200,0xffaa5500,0xff8a4800,0xea743f00,0x5a7a4200,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x0f8d4c00,0xf38d4c00,0xffdc6a00,0xffe16d00,0xffe16d00,0xffe26d00,0xffe26d00,0xffe26d00,0xffe26d00,0xffe26d00,0xffe16d00,0xffe06c00,0xffde6b00,0xffd96900,0xffd16500,0xffc76000,0xffb95900,0xffab5200,0xff9c4b00,0xff894300,0xff6b3600,0xf9512c00,0xa5542d00,0x3c5e3200,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x008d4c00,0x968d4c00,0xffbc5d00,0xffde6a00,0xffde6a00,0xffde6a00,0xffdf6a00,0xffdf6a00,0xffdf6a00,0xffde6a00,0xffdc6800,0xffd66600,0xffcc6100,0xffbf5b00,0xffaf5300,0xff9d4a00,0xff8a4200,0xff6d3500,0xff502900,0xe7402300,0x7b3f2200,0x15442500,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x2a8d4c00,0xff9b5000,0xffda6600,0xffdb6700,0xffdb6700,0xffdc6700,0xffdc6700,0xffdb6700,0xffd96500,0xffd16200,0xffc25b00,0xffad5100,0xff974700,0xff7f3c00,0xff602f00,0xff472500,0xbd3d2100,0x513d2100,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x008e4c00,0xc08d4c00,0xffc35c00,0xffd76300,0xffd76300,0xffd86300,0xffd86300,0xffd76300,0xffd06000,0xffc05800,0xffa54c00,0xff7f3b00,0xff582c00,0xf03f2200,0x903c2000,0x2a3e2100,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x548d4c00,0xffa55200,0xffd35f00,0xffd46000,0xffd46000,0xffd46000,0xffd25e00,0xffc65900,0xffac4e00,0xff833c00,0xe7472600,0x693c2000,0x0c3d2100,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x038d4c00,0xe48d4c00,0xffc95a00,0xffd15d00,0xffd15d00,0xffd15d00,0xffcb5a00,0xffb95200,0xff984300,0xff5f2e00,0x723f2200,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x7b8d4c00,0xffad5200,0xffce5a00,0xffce5a00,0xffcd5900,0xffc35500,0xffaa4a00,0xff853a00,0xf9472600,0x15432400,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x188d4c00,0xf98e4c00,0xffc95600,0xffcb5700,0xffc75500,0xffb94f00,0xff9b4200,0xff6c3100,0xab442500,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4d00,0x008e4c00,0xa58d4c00,0xffb35000,0xffc75300,0xffc05000,0xffac4800,0xff8b3a00,0xff542a00,0x45462500,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x398d4c00,0xff994d00,0xffc24f00,0xffb74b00,0xff9e4000,0xff763200,0xde472600,0x03492800,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x008e4c00,0xcf8d4c00,0xffb24b00,0xffab4500,0xff8d3900,0xff5e2b00,0x7e452500,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x638d4c00,0xff984800,0xffa03f00,0xff7e3200,0xfc492800,0x1b472600,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x098b4b00,0xed824600,0xff903800,0xff692c00,0xb4462600,0x004c2900,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008e4d00,0x008e4c00,0x008a4a00,0x8a7e4400,0xff793500,0xff572900,0x51472600,0x00542d00,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008d4c00,0x00884900,0x247a4200,0xfc633500,0xe74f2a00,0x034d2900,0x005e3300,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008e4d00,0x008d4c00,0x00884900,0x00794100,0xb4643600,0x87552e00,0x00593000,0x006b3900,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008f4d00,0x008d4c00,0x00884900,0x007c4300,0x486d3b00,0x24643600,0x00693800,0x00774000,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,
            0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff,0x00ffffff
        };
    }


    // ======================> ui_colors

    public class ui_colors
    {
        rgb_t m_border_color;
        rgb_t m_background_color;
        rgb_t m_gfxviewer_bg_color;
        rgb_t m_unavailable_color;
        rgb_t m_text_color;
        rgb_t m_text_bg_color;
        rgb_t m_subitem_color;
        rgb_t m_clone_color;
        rgb_t m_selected_color;
        rgb_t m_selected_bg_color;
        rgb_t m_mouseover_color;
        rgb_t m_mouseover_bg_color;
        rgb_t m_mousedown_color;
        rgb_t m_mousedown_bg_color;
        rgb_t m_dipsw_color;
        rgb_t m_slider_color;


        public rgb_t border_color() { return m_border_color; }
        public rgb_t background_color() { return m_background_color; }
        public rgb_t gfxviewer_bg_color() { return m_gfxviewer_bg_color; }
        //rgb_t unavailable_color() const { return m_unavailable_color; }
        public rgb_t text_color() { return m_text_color; }
        public rgb_t text_bg_color() { return m_text_bg_color; }
        public rgb_t subitem_color() { return m_subitem_color; }
        public rgb_t clone_color() { return m_clone_color; }
        public rgb_t selected_color() { return m_selected_color; }
        public rgb_t selected_bg_color() { return m_selected_bg_color; }
        public rgb_t mouseover_color() { return m_mouseover_color; }
        public rgb_t mouseover_bg_color() { return m_mouseover_bg_color; }
        //rgb_t mousedown_color() const { return m_mousedown_color; }
        //rgb_t mousedown_bg_color() const { return m_mousedown_bg_color; }
        //rgb_t dipsw_color() const { return m_dipsw_color; }
        public rgb_t slider_color() { return m_slider_color; }


        public void refresh(ui_options options)
        {
            m_border_color = options.border_color();
            m_background_color = options.background_color();
            m_gfxviewer_bg_color = options.gfxviewer_bg_color();
            m_unavailable_color = options.unavailable_color();
            m_text_color = options.text_color();
            m_text_bg_color = options.text_bg_color();
            m_subitem_color = options.subitem_color();
            m_clone_color = options.clone_color();
            m_selected_color = options.selected_color();
            m_selected_bg_color = options.selected_bg_color();
            m_mouseover_color = options.mouseover_color();
            m_mouseover_bg_color = options.mouseover_bg_color();
            m_mousedown_color = options.mousedown_color();
            m_mousedown_bg_color = options.mousedown_bg_color();
            m_dipsw_color = options.dipsw_color();
            m_slider_color = options.slider_color();
        }
    }


    // ======================> mame_ui_manager
    public class mame_ui_manager : ui_manager
    {
        delegate uint32_t handler_callback_func(render_container container, mame_ui_manager mui);  //using handler_callback_func = std::function<uint32_t (render_container &)>;
        //using device_feature_set = std::set<std::pair<std::string, std::string> >;


        public enum draw_mode
        {
            NONE,
            NORMAL,
            OPAQUE_
        }


        // instance variables
        render_font m_font;
        handler_callback_func m_handler_callback;
        ui_callback_type m_handler_callback_type;
        uint32_t m_handler_param;
        bool m_single_step;
        bool m_showfps;
        osd_ticks_t m_showfps_end;
        bool m_show_profiler;
        osd_ticks_t m_popup_text_end;
        byte [] m_non_char_keys_down;
        bitmap_argb32 m_mouse_bitmap;
        render_texture m_mouse_arrow_texture;
        bool m_mouse_show;
        ui_options m_ui_options = new ui_options();
        ui_colors m_ui_colors = new ui_colors();
        float m_target_font_height;
        bool m_has_warnings;
        bool m_unthrottle_mute;

        ui.machine_info m_machine_info;
        mame_ui_manager_device_feature_set m_unemulated_features;
        mame_ui_manager_device_feature_set m_imperfect_features;
        std_time_t m_last_launch_time;
        std_time_t m_last_warning_time;

        // static variables
        static string messagebox_text;
        static string messagebox_poptext;

        static std.vector<ui.menu_item> slider_list;
        static slider_state slider_current;  //static slider_state     *slider_current;


        std.vector<slider_state> m_sliders;  //std::vector<std::unique_ptr<slider_state>> m_sliders;


        // construction/destruction

        //-------------------------------------------------
        //  ctor - set up the user interface
        //-------------------------------------------------
        public mame_ui_manager(running_machine machine)
            : base(machine)
        {
            m_font = null;
            m_handler_callback = null;
            m_handler_callback_type = ui_callback_type.GENERAL;
            m_handler_param = 0;
            m_single_step = false;
            m_showfps = false;
            m_showfps_end = 0;
            m_show_profiler = false;
            m_popup_text_end = 0;
            m_mouse_bitmap = new bitmap_argb32(32, 32);
            m_mouse_arrow_texture = null;
            m_mouse_show = false;
            m_target_font_height = 0;
            m_has_warnings = false;
            m_unthrottle_mute = false;
            m_machine_info = null;
            m_unemulated_features = null;
            m_imperfect_features = null;
            m_last_launch_time = (std_time_t)(-1);
            m_last_warning_time = (std_time_t)(-1);
        }


        public void init()
        {
            load_ui_options(machine());

            // initialize the other UI bits
            ui.menu.init(machine(), options());
            viewgfx_global.ui_gfx_init(machine());

            m_ui_colors.refresh(options());

            // update font row info from setting
            update_target_font_height();

            // more initialization
            set_handler(
                    ui_callback_type.GENERAL,
                    (render_container container, mame_ui_manager mui) =>
                    {
                        draw_text_box(container, messagebox_text, ui.text_layout.text_justify.LEFT, 0.5f, 0.5f, colors().background_color());
                        return 0;
                    });
            m_non_char_keys_down = new byte [(std.size(ui_global.non_char_keys) + 7) / 8]; // auto_alloc_array(machine, UINT8, (ARRAY_LENGTH(non_char_keys) + 7) / 8);
            m_mouse_show = ((UInt64)machine().system().flags & g.MACHINE_CLICKABLE_ARTWORK) == g.MACHINE_CLICKABLE_ARTWORK ? true : false;

            // request notification callbacks
            machine().add_notifier(machine_notification.MACHINE_NOTIFY_EXIT, exit);
            machine().configuration().config_register(
                    "ui_warnings",
                    config_load,
                    config_save);

            // create mouse bitmap
            PointerU32 dst = m_mouse_bitmap.pix(0);  //uint32_t *dst = &m_mouse_bitmap.pix(0);
            //memcpy(dst,mouse_bitmap,32*32*sizeof(UINT32));
            for (int i = 0; i < 32*32; i++)
                dst[i] = ui_global.mouse_bitmap[i];

            m_mouse_arrow_texture = machine().render().texture_alloc();
            m_mouse_arrow_texture.set_bitmap(m_mouse_bitmap, m_mouse_bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);
        }


        // getters
        public running_machine machine() { return m_machine; }
        bool single_step() { return m_single_step; }
        public ui_options options() { return m_ui_options; }
        public ui_colors colors() { return m_ui_colors; }
        public ui.machine_info machine_info() { g.assert(m_machine_info != null); return m_machine_info; }


        // setters
        void set_single_step(bool single_step) { m_single_step = single_step; }


        // methods

        //-------------------------------------------------
        //  initialize - initialize ui lists
        //-------------------------------------------------
        public void initialize(running_machine machine)
        {
            m_machine_info = new ui.machine_info(machine);

            // initialize the on-screen display system
            slider_list = slider_init(machine);
            if (slider_list.Count > 0)
            {
                slider_current = (slider_state)slider_list[0].ref_;
            }
            else
            {
                slider_current = null;
            }

            // if no test switch found, assign its input sequence to a service mode DIP
            if (!m_machine_info.has_test_switch() && m_machine_info.has_dips())
            {
                string service_mode_dipname = ioport_configurer.string_from_token(g.DEF_STR(INPUT_STRING.INPUT_STRING_Service_Mode));
                foreach (var port in machine.ioport().ports())
                {
                    foreach (ioport_field field in port.Value.fields())
                    {
                        if (field.type() == ioport_type.IPT_DIPSWITCH && std.strcmp(field.name(), service_mode_dipname) == 0)
                            field.set_defseq(machine.ioport().type_seq(ioport_type.IPT_SERVICE));
                    }
                }
            }

            // handle throttle-related options and initial muting state now that the sound manager has been brought up
            bool starting_throttle = machine.options().throttle();
            machine.video().set_throttled(starting_throttle);
            m_unthrottle_mute = options().unthrottle_mute();
            if (!starting_throttle && m_unthrottle_mute)
                machine.sound().ui_mute(true);
        }


        //----------------------------------------------------------
        //  mame_ui_manager::slider_init - initialize the list of slider
        //  controls
        //----------------------------------------------------------
        std.vector<ui.menu_item> slider_init(running_machine machine)
        {
            //throw new emu_unimplemented();
#if false
#endif

            return new std.vector<ui.menu_item>();
        }


        //-------------------------------------------------
        //  set_handler - set a callback/parameter
        //  pair for the current UI handler
        //-------------------------------------------------
        void set_handler(ui_callback_type callback_type, handler_callback_func callback)  //void mame_ui_manager::set_handler(ui_callback_type callback_type, const std::function<uint32_t (render_container &)> &&callback)
        {
            m_handler_callback = callback;
            m_handler_callback_type = callback_type;
        }


        //-------------------------------------------------
        //  output_joined_collection
        //-------------------------------------------------
        delegate void TEmitMemberFunc(string img);
        delegate void TEmitDelimFunc();
        //template<typename TColl, typename TEmitMemberFunc, typename TEmitDelimFunc>
        static void output_joined_collection(std.vector<string> collection, TEmitMemberFunc emit_member, TEmitDelimFunc emit_delim)  //static void output_joined_collection(const TColl &collection, TEmitMemberFunc emit_member, TEmitDelimFunc emit_delim)
        {
            bool is_first = true;

            foreach (var member in collection)
            {
                if (is_first)
                    is_first = false;
                else
                    emit_delim();

                emit_member(member);
            }
        }


        //-------------------------------------------------
        //  display_startup_screens - display the
        //  various startup screens
        //-------------------------------------------------
        public void display_startup_screens(bool first_time)
        {
            const int maxstate = 3;
            int str = machine().options().seconds_to_run();
            bool show_gameinfo = !machine().options().skip_gameinfo();
            bool show_warnings = true;
            bool show_mandatory_fileman = true;
            bool video_none = std.strcmp(((osd_options)machine().options()).video(), osd_options.OSDOPTVAL_NONE) == 0;

            // disable everything if we are using -str for 300 or fewer seconds, or if we're the empty driver,
            // or if we are debugging, or if there's no mame window to send inputs to
            if (!first_time || (str > 0 && str < 60 * 5) || machine().system() == ___empty.driver____empty || (machine().debug_flags & g.DEBUG_FLAG_ENABLED) != 0 || video_none)
                show_gameinfo = show_warnings = show_mandatory_fileman = false;

#if EMSCRIPTEN
            // also disable for the JavaScript port since the startup screens do not run asynchronously
            show_gameinfo = show_warnings = FALSE;
#endif


            // BYPASS STARTUP SCREENS
            show_gameinfo = show_warnings = show_mandatory_fileman = false;


            // set up event handlers
            //using namespace std::placeholders;
            switch_code_poller poller = new switch_code_poller(machine().input());
            string warning_text = "";
            rgb_t warning_color = new rgb_t();
            bool config_menu = false;
            handler_callback_func handler_messagebox_anykey =
                (render_container container, mame_ui_manager mui) =>
                {
                    // draw a standard message window
                    draw_text_box(container, warning_text, ui.text_layout.text_justify.LEFT, 0.5f, 0.5f, warning_color);

                    if (machine().ui_input().pressed((int)ioport_type.IPT_UI_CANCEL))
                    {
                        // if the user cancels, exit out completely
                        machine().schedule_exit();
                        return g.UI_HANDLER_CANCEL;
                    }
                    else if (machine().ui_input().pressed((int)ioport_type.IPT_UI_CONFIGURE))
                    {
                        config_menu = true;
                        return g.UI_HANDLER_CANCEL;
                    }
                    else if (poller.poll() != input_code.INPUT_CODE_INVALID)
                    {
                        // if any key is pressed, just exit
                        return g.UI_HANDLER_CANCEL;
                    }

                    return 0;
                };

            set_handler(ui_callback_type.GENERAL, handler_ingame);

            // loop over states
            for (int state = 0; state < maxstate && !machine().scheduled_event_pending() && !ui.menu.stack_has_special_main_menu(machine()); state++)
            {
                // default to standard colors
                warning_color = colors().background_color();
                warning_text = "";  //warning_text.clear();

                // pick the next state
                switch (state)
                {
                    case 0:
                        if (show_gameinfo)
                            warning_text = machine_info().game_info_string();

                        if (!warning_text.empty())
                        {
                            warning_text += "\n\nPress any key to continue";
                            set_handler(ui_callback_type.MODAL, handler_messagebox_anykey);
                        }
                        break;

                    case 1:
                        warning_text = machine_info().warnings_string();
                        m_has_warnings = !warning_text.empty();
                        if (show_warnings)
                        {
                            bool need_warning = m_has_warnings;
                            if (machine_info().has_severe_warnings() || !m_has_warnings)
                            {
                                // critical warnings - no need to persist stuff
                                m_unemulated_features.clear();
                                m_imperfect_features.clear();

                                throw new emu_unimplemented();
#if false
                                m_last_launch_time = std::time_t(-1);
                                m_last_warning_time = std::time_t(-1);
#endif
                            }
                            else
                            {
                                // non-critical warnings - map current unemulated/imperfect features
                                mame_ui_manager_device_feature_set unemulated_features = new mame_ui_manager_device_feature_set();
                                mame_ui_manager_device_feature_set imperfect_features = new mame_ui_manager_device_feature_set();
                                foreach (device_t device in new device_enumerator(machine().root_device()))
                                {
                                    device_t_feature_type unemulated = device.type().unemulated_features();
                                    for (device_t_feature_type feature = (device_t_feature_type)1; unemulated != 0; feature = (device_t_feature_type)((uint32_t)feature << 1))  //for (std::underlying_type_t<device_t::feature_type> feature = 1U; unemulated; feature <<= 1)
                                    {
                                        if ((uint32_t)unemulated != 0 & (uint32_t)feature != 0)
                                        {
                                            throw new emu_unimplemented();
#if false
                                            string name = info_xml_creator.feature_name((device_t_feature_type)feature);
                                            if (!string.IsNullOrEmpty(name))
                                                unemulated_features.emplace(new std.pair<string, string>(device.type().shortname(), name));
                                            unemulated &= (device_t_feature_type)(~feature);
#endif
                                        }
                                    }

                                    device_t_feature_type imperfect = device.type().imperfect_features();
                                    for (device_t_feature_type feature = (device_t_feature_type)1; imperfect != 0; feature = (device_t_feature_type)((uint32_t)feature << 1))  //for (std::underlying_type_t<device_t::feature_type> feature = 1U; imperfect; feature <<= 1)
                                    {
                                        if ((uint32_t)imperfect != 0 & (uint32_t)feature != 0)
                                        {
                                            throw new emu_unimplemented();
#if false
                                            string name = info_xml_creator.feature_name((device_t_feature_type)feature);
                                            if (!string.IsNullOrEmpty(name))
                                                imperfect_features.emplace(new std.pair<string, string>(device.type().shortname(), name));
                                            imperfect &= (device_t_feature_type)(~feature);
#endif
                                        }
                                    }
                                }

                                // machine flags can cause warnings, too
                                if ((machine_info().machine_flags_get() & machine_flags.type.NO_COCKTAIL) != 0)
                                    unemulated_features.emplace(new std.pair<string, string>(machine().root_device().type().shortname(), "cocktail"));

                                // if the warnings match what was shown sufficiently recently, it's skippable
                                if ((unemulated_features != m_unemulated_features) || (imperfect_features != m_imperfect_features))
                                {
                                    m_last_launch_time = (std_time_t)(-1);
                                }
                                else if (machine().rom_load().warnings() == 0 && ((std_time_t)(-1) != m_last_launch_time) && ((std_time_t)(-1) != m_last_warning_time) && options().skip_warnings())
                                {
                                    var now = std.chrono.system_clock.now();  //auto const now = std::chrono::system_clock::now();
                                    if (((std.chrono.system_clock.from_time_t(m_last_launch_time) + std.chrono.hours(7 * 24)) >= now) && ((std.chrono.system_clock.from_time_t(m_last_warning_time) + std.chrono.hours(14 * 24)) >= now))  //if (((std::chrono::system_clock::from_time_t(m_last_launch_time) + std::chrono::hours(7 * 24)) >= now) && ((std::chrono::system_clock::from_time_t(m_last_warning_time) + std::chrono::hours(14 * 24)) >= now))
                                        need_warning = false;
                                }

                                // update the information to save out
                                m_unemulated_features = unemulated_features;
                                m_imperfect_features = imperfect_features;
                                if (need_warning)
                                    m_last_warning_time = std.chrono.system_clock.to_time_t(std.chrono.system_clock.now());  //m_last_warning_time = std::chrono::system_clock::to_time_t(std::chrono::system_clock::now());
                            }

                            if (need_warning)
                            {
                                warning_text += "\n\nPress any key to continue";
                                set_handler(ui_callback_type.MODAL, handler_messagebox_anykey);
                                warning_color = machine_info().warnings_color();
                            }
                        }
                        break;

                    case 2:
                        std.vector<string> mandatory_images = mame_machine_manager.instance().missing_mandatory_images();
                        if (!mandatory_images.empty() && show_mandatory_fileman)
                        {
                            string warning = "";
                            warning += "This driver requires images to be loaded in the following device(s): ";

                            output_joined_collection(mandatory_images,
                            (img) => { warning += "\"" + img + "\""; },  //    [&warning](const std::reference_wrapper<const std::string> &img)    { warning << "\"" << img.get() << "\""; },
                            () =>    { warning += ","; });  //    [&warning]()                                                        { warning << ","; });

                            ui.menu_file_manager.force_file_manager(this, machine().render().ui_container(), warning);
                        }
                        break;
                }

                // clear the input memory and wait for all keys to be released
                poller.reset();
                while (poller.poll() != input_code.INPUT_CODE_INVALID) { }

                if (m_handler_callback_type == ui_callback_type.MODAL)
                {
                    config_menu = false;

                    // loop while we have a handler
                    while (m_handler_callback_type == ui_callback_type.MODAL && !machine().scheduled_event_pending() && !ui.menu.stack_has_special_main_menu(machine()))
                        machine().video().frame_update();
                }

                // clear the handler and force an update
                set_handler(ui_callback_type.GENERAL, handler_ingame);
                machine().video().frame_update();
            }

            // update last launch time if this was a run that was eligible for emulation warnings
            if (m_has_warnings && show_warnings && !machine().scheduled_event_pending())
                m_last_launch_time = std.chrono.system_clock.to_time_t(std.chrono.system_clock.now());  //m_last_launch_time = std::chrono::system_clock::to_time_t(std::chrono::system_clock::now());

            // if we're the empty driver, force the menus on
            if (ui.menu.stack_has_special_main_menu(machine()))
            {
                show_menu();
            }
            else if (config_menu)
            {
                show_menu();

                // loop while we have a handler
                while (m_handler_callback_type != ui_callback_type.GENERAL && !machine().scheduled_event_pending())
                    machine().video().frame_update();
            }
        }


        static osd_ticks_t lastupdatetime_set_startup_text = 0;

        //-------------------------------------------------
        //  set_startup_text - set the text to display
        //  at startup
        //-------------------------------------------------
        public override void set_startup_text(string text, bool force)
        {
            //static osd_ticks_t lastupdatetime = 0;
            osd_ticks_t curtime = osdcore_global.m_osdcore.osd_ticks();

            // copy in the new text
            messagebox_text = text;

            // don't update more than 4 times/second
            if (force || (curtime - lastupdatetime_set_startup_text) > osdcore_global.m_osdcore.osd_ticks_per_second() / 4)
            {
                lastupdatetime_set_startup_text = curtime;
                machine().video().frame_update();
            }
        }

        //-------------------------------------------------
        //  update_and_render - update the UI and
        //  render it; called by video.c
        //-------------------------------------------------
        public void update_and_render(render_container container)
        {
            // always start clean
            container.empty();

            // if we're paused, dim the whole screen
            if (machine().phase() >= machine_phase.RESET && (single_step() || machine().paused()))
            {
                int alpha = (int)((1.0f - machine().options().pause_brightness()) * 255.0f);
                if (ui.menu.stack_has_special_main_menu(machine()))
                    alpha = 255;
                if (alpha > 255)
                    alpha = 255;
                if (alpha >= 0)
                    container.add_rect(0.0f, 0.0f, 1.0f, 1.0f, new rgb_t((uint8_t)alpha,0x00,0x00,0x00), g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
            }

            // render any cheat stuff at the bottom
            if (machine().phase() >= machine_phase.RESET)
                mame_machine_manager.instance().cheat().render_text(this, container);

            // call the current UI handler
            m_handler_param = m_handler_callback(container, this);

            // display any popup messages
            if (osdcore_global.m_osdcore.osd_ticks() < m_popup_text_end)
                draw_text_box(container, messagebox_poptext, ui.text_layout.text_justify.CENTER, 0.5f, 0.9f, colors().background_color());
            else
                m_popup_text_end = 0;

            // display the internal mouse cursor
            if (m_mouse_show || (is_menu_active() && machine().options().ui_mouse()))
            {
                int32_t mouse_target_x;
                int32_t mouse_target_y;
                bool mouse_button;
                render_target mouse_target = machine().ui_input().find_mouse(out mouse_target_x, out mouse_target_y, out mouse_button);

                if (mouse_target != null)
                {
                    float mouse_y = -1;
                    float mouse_x = -1;
                    if (mouse_target.map_point_container(mouse_target_x, mouse_target_y, container, out mouse_x, out mouse_y))
                    {
                        float cursor_size = 0.6f * get_line_height();
                        container.add_quad(mouse_x, mouse_y, mouse_x + cursor_size * container.manager().ui_aspect(container), mouse_y + cursor_size, colors().text_color(), m_mouse_arrow_texture, g.PRIMFLAG_ANTIALIAS(1) | g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
                    }
                }
            }

            // cancel takes us back to the ingame handler
            if (m_handler_param == g.UI_HANDLER_CANCEL)
            {
                //using namespace std::placeholders;
                set_handler(ui_callback_type.GENERAL, handler_ingame);
            }
        }

        //-------------------------------------------------
        //  get_font - return the UI font
        //-------------------------------------------------
        public render_font get_font()
        {
            // allocate the font and messagebox string
            if (m_font == null)
                m_font = machine().render().font_alloc(machine().options().ui_font());

            return m_font;
        }

        //-------------------------------------------------
        //  get_line_height - return the current height
        //  of a line
        //-------------------------------------------------
        public float get_line_height()
        {
            int32_t raw_font_pixel_height = get_font().pixel_height();
            render_target ui_target = machine().render().ui_target();
            int32_t target_pixel_height = ui_target.height();
            float one_to_one_line_height;
            float scale_factor;

            // compute the font pixel height at the nominal size
            one_to_one_line_height = (float)raw_font_pixel_height / (float)target_pixel_height;

            // determine the scale factor
            scale_factor = target_font_height() / one_to_one_line_height;

            // if our font is small-ish, do integral scaling
            if (raw_font_pixel_height < 24)
            {
                // do we want to scale smaller? only do so if we exceed the threshold
                if (scale_factor <= 1.0f)
                {
                    if (one_to_one_line_height < g.UI_MAX_FONT_HEIGHT || raw_font_pixel_height < 12)
                        scale_factor = 1.0f;
                }

                // otherwise, just ensure an integral scale factor
                else
                {
                    scale_factor = std.floor(scale_factor);
                }
            }

            // otherwise, just make sure we hit an even number of pixels
            else
            {
                int32_t height = (int32_t)(scale_factor * one_to_one_line_height * (float)target_pixel_height);
                scale_factor = (float)height / (one_to_one_line_height * (float)target_pixel_height);
            }

            return scale_factor * one_to_one_line_height;
        }


        //-------------------------------------------------
        //  get_char_width - return the width of a
        //  single character
        //-------------------------------------------------
        public float get_char_width(char32_t ch)
        {
            return get_font().char_width(get_line_height(), machine().render().ui_aspect(), ch);
        }


        //-------------------------------------------------
        //  get_string_width - return the width of a
        //  character string
        //-------------------------------------------------

        public float get_string_width(string s, float text_size = 1.0f)
        {
            return get_font().utf8string_width(get_line_height() * text_size, machine().render().ui_aspect(), s);
        }

        //-------------------------------------------------
        //  draw_outlined_box - add primitives to draw
        //  an outlined box with the given background
        //  color
        //-------------------------------------------------
        public void draw_outlined_box(render_container container, float x0, float y0, float x1, float y1, rgb_t backcolor)
        {
            draw_outlined_box(container, x0, y0, x1, y1, colors().border_color(), backcolor);
        }

        public void draw_outlined_box(render_container container, float x0, float y0, float x1, float y1, rgb_t fgcolor, rgb_t bgcolor)
        {
            container.add_rect(x0, y0, x1, y1, bgcolor, g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
            container.add_line(x0, y0, x1, y0, g.UI_LINE_WIDTH, fgcolor, g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
            container.add_line(x1, y0, x1, y1, g.UI_LINE_WIDTH, fgcolor, g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
            container.add_line(x1, y1, x0, y1, g.UI_LINE_WIDTH, fgcolor, g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
            container.add_line(x0, y1, x0, y0, g.UI_LINE_WIDTH, fgcolor, g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
        }

        //-------------------------------------------------
        //  draw_text - simple text renderer
        //-------------------------------------------------
        void draw_text(render_container container, string buf, float x, float y)
        {
            float unused1;
            float unused2;
            draw_text_full(container, buf, x, y, 1.0f - x, ui.text_layout.text_justify.LEFT, ui.text_layout.word_wrapping.WORD, draw_mode.NORMAL, colors().text_color(), colors().text_bg_color(), out unused1, out unused2);
        }

        //-------------------------------------------------
        //  draw_text_full - full featured text
        //  renderer with word wrapping, justification,
        //  and full size computation
        //-------------------------------------------------
        public void draw_text_full(render_container container, string origs, float x, float y, float origwrapwidth, ui.text_layout.text_justify justify, ui.text_layout.word_wrapping wrap, draw_mode draw, rgb_t fgcolor, rgb_t bgcolor, out float totalwidth, out float totalheight, float text_size = 1.0f)
        {
            // create the layout
            var layout = create_layout(container, origwrapwidth, justify, wrap);

            // append text to it
            layout.add_text(
                    origs,
                    fgcolor,
                    draw == draw_mode.OPAQUE_ ? bgcolor : rgb_t.transparent(),
                    text_size);

            // and emit it (if we are asked to do so)
            if (draw != draw_mode.NONE)
                layout.emit(container, x, y);

            // return width/height
            //if (totalwidth)
                totalwidth = layout.actual_width();
            //if (totalheight)
                totalheight = layout.actual_height();
        }


        //-------------------------------------------------
        //  draw_text_box - draw a multiline text
        //  message with a box around it
        //-------------------------------------------------
        public void draw_text_box(render_container container, string text, ui.text_layout.text_justify justify, float xpos, float ypos, rgb_t backcolor)
        {
            // cap the maximum width
            float maximum_width = 1.0f - box_lr_border() * 2;

            // create a layout
            ui.text_layout layout = create_layout(container, maximum_width, justify);

            // add text to it
            layout.add_text(text);

            // and draw the result
            draw_text_box(container, layout, xpos, ypos, backcolor);
        }


        //-------------------------------------------------
        //  draw_text_box - draw a multiline text
        //  message with a box around it
        //-------------------------------------------------
        void draw_text_box(render_container container, ui.text_layout layout, float xpos, float ypos, rgb_t backcolor)
        {
            // xpos and ypos are where we want to "pin" the layout, but we need to adjust for the actual size of the payload
            var actual_left = layout.actual_left();
            var actual_width = layout.actual_width();
            var actual_height = layout.actual_height();
            var x = std.min(std.max(xpos - actual_width / 2, box_lr_border()), 1.0f - actual_width - box_lr_border());
            var y = std.min(std.max(ypos - actual_height / 2, box_tb_border()), 1.0f - actual_height - box_tb_border());

            // add a box around that
            draw_outlined_box(container,
                    x - box_lr_border(),
                    y - box_tb_border(),
                    x + actual_width + box_lr_border(),
                    y + actual_height + box_tb_border(), backcolor);

            // emit the text
            layout.emit(container, x - actual_left, y);
        }


        //void draw_message_window(render_container *container, const char *text);


        // load/save options to file

        //void load_ui_options();


        //-------------------------------------------------
        //  save ui options
        //-------------------------------------------------
        public void save_ui_options()
        {
            // attempt to open the output file
            emu_file file = new emu_file(machine().options().ini_path(), g.OPEN_FLAG_WRITE | g.OPEN_FLAG_CREATE | g.OPEN_FLAG_CREATE_PATHS);
            if (file.open("ui.ini") == osd_file.error.NONE)
            {
                // generate the updated INI
                file.puts(options().output_ini());
                file.close();
            }
            else
            {
                machine().popmessage("**Error saving ui.ini**");
            }
        }



        //void save_main_option();


        //-------------------------------------------------
        //  popup_time - popup a message for a specific
        //  amount of time
        //-------------------------------------------------
        public void popup_time(int seconds, string text)
        {
            // extract the text
            messagebox_poptext = text;

            // set a timer
            m_popup_text_end = osdcore_global.m_osdcore.osd_ticks() + osdcore_global.m_osdcore.osd_ticks_per_second() * (UInt64)seconds;
        }

        //-------------------------------------------------
        //  show_fps_temp - show the FPS counter for
        //  a specific period of time
        //-------------------------------------------------
        void show_fps_temp(double seconds) { if (!m_showfps) m_showfps_end = osdcore_global.m_osdcore.osd_ticks() + (osd_ticks_t)(seconds * osdcore_global.m_osdcore.osd_ticks_per_second()); }

        //-------------------------------------------------
        //  set_show_fps - show/hide the FPS counter
        //-------------------------------------------------
        void set_show_fps(bool show)
        {
            m_showfps = show;
            if (!show)
            {
                m_showfps = false;
                m_showfps_end = 0;
            }
        }

        //-------------------------------------------------
        //  show_fps - return the current FPS
        //  counter visibility state
        //-------------------------------------------------
        bool show_fps() { return m_showfps || (m_showfps_end != 0); }

        //-------------------------------------------------
        //  show_fps_counter
        //-------------------------------------------------
        public bool show_fps_counter()
        {
            bool result = m_showfps || osdcore_global.m_osdcore.osd_ticks() < m_showfps_end;
            if (!result)
                m_showfps_end = 0;

            return result;
        }

        //-------------------------------------------------
        //  set_show_profiler - show/hide the profiler
        //-------------------------------------------------
        void set_show_profiler(bool show)
        {
            m_show_profiler = show;
            profiler_global.g_profiler.enable(show);
        }

        //-------------------------------------------------
        //  show_profiler - return the current
        //  profiler visibility state
        //-------------------------------------------------
        bool show_profiler() { return m_show_profiler; }


        //-------------------------------------------------
        //  show_menu - show the menus
        //-------------------------------------------------
        public void show_menu()
        {
            //using namespace std::placeholders;
            set_handler(ui_callback_type.MENU, ui.menu.ui_handler);  //, _1, std::ref_(this)));
        }


        //void show_mouse(bool status);


        //-------------------------------------------------
        //  is_menu_active - return true if the menu
        //  UI handler is active
        //-------------------------------------------------
        public override bool is_menu_active()
        {
            return m_handler_callback_type == ui_callback_type.MENU
                || m_handler_callback_type == ui_callback_type.VIEWER;
        }


        //-------------------------------------------------
        //  can_paste
        //-------------------------------------------------
        bool can_paste()
        {
            throw new emu_unimplemented();
        }


        public bool found_machine_warnings() { return m_has_warnings; }


        //-------------------------------------------------
        //  image_handler_ingame - execute display
        //  callback function for each image device
        //-------------------------------------------------
        void image_handler_ingame()
        {
            // run display routine for devices
            if (machine().phase() == machine_phase.RUNNING)
            {
                var layout = create_layout(machine().render().ui_container());

                // loop through all devices, build their text into the layout
                foreach (device_image_interface image in new image_interface_enumerator(machine().root_device()))
                {
                    string str = image.call_display();
                    if (!str.empty())
                    {
                        layout.add_text(str);
                        layout.add_text("\n");
                    }
                }

                // did we actually create anything?
                if (!layout.empty())
                {
                    float x = 0.2f;
                    float y = 0.5f * get_line_height() + 2.0f * box_tb_border();
                    draw_text_box(machine().render().ui_container(), layout, x, y, colors().background_color());
                }
            }
        }

        //-------------------------------------------------
        //  increase_frameskip
        //-------------------------------------------------
        void increase_frameskip()
        {
            // get the current value and increment it
            int newframeskip = machine().video().frameskip() + 1;
            if (newframeskip > video_manager.MAX_FRAMESKIP)
                newframeskip = -1;
            machine().video().set_frameskip(newframeskip);

            // display the FPS counter for 2 seconds
            show_fps_temp(2.0);
        }

        //-------------------------------------------------
        //  decrease_frameskip
        //-------------------------------------------------
        void decrease_frameskip()
        {
            // get the current value and decrement it
            int newframeskip = machine().video().frameskip() - 1;
            if (newframeskip < -1)
                newframeskip = video_manager.MAX_FRAMESKIP;
            machine().video().set_frameskip(newframeskip);

            // display the FPS counter for 2 seconds
            show_fps_temp(2.0);
        }

        //-------------------------------------------------
        //  request_quit
        //-------------------------------------------------
        public void request_quit()
        {
            //using namespace std::placeholders;
            if (!machine().options().confirm_quit())
                machine().schedule_exit();
            else
                set_handler(ui_callback_type.GENERAL, handler_confirm_quit);
        }


        //-------------------------------------------------
        //  draw_fps_counter
        //-------------------------------------------------
        void draw_fps_counter(render_container container)
        {
            float unused1;
            float unused2;
            draw_text_full(container, machine().video().speed_text(), 0.0f, 0.0f, 1.0f,
                ui.text_layout.text_justify.RIGHT, ui.text_layout.word_wrapping.WORD, draw_mode.OPAQUE_, rgb_t.white(), rgb_t.black(), out unused1, out unused2);
        }


        //-------------------------------------------------
        //  draw_timecode_counter
        //-------------------------------------------------
        void draw_timecode_counter(render_container container)
        {
            string tempstring;
            float unused1;
            float unused2;
            draw_text_full(container, machine().video().timecode_text(out tempstring), 0.0f, 0.0f, 1.0f,
                ui.text_layout.text_justify.RIGHT, ui.text_layout.word_wrapping.WORD, draw_mode.OPAQUE_, new rgb_t(0xf0, 0xf0, 0x10, 0x10), rgb_t.black(), out unused1, out unused2);
        }


        //-------------------------------------------------
        //  draw_timecode_total
        //-------------------------------------------------
        void draw_timecode_total(render_container container)
        {
            string tempstring;
            float unused1;
            float unused2;
            draw_text_full(container, machine().video().timecode_total_text(out tempstring), 0.0f, 0.0f, 1.0f,
                ui.text_layout.text_justify.LEFT, ui.text_layout.word_wrapping.WORD, draw_mode.OPAQUE_, new rgb_t(0xf0, 0x10, 0xf0, 0x10), rgb_t.black(), out unused1, out unused2);
        }


        //-------------------------------------------------
        //  draw_profiler
        //-------------------------------------------------
        void draw_profiler(render_container container)
        {
            string text = profiler_global.g_profiler.text(machine());
            float unused1;
            float unused2;
            draw_text_full(container, text, 0.0f, 0.0f, 1.0f, ui.text_layout.text_justify.LEFT, ui.text_layout.word_wrapping.WORD, draw_mode.OPAQUE_, rgb_t.white(), rgb_t.black(), out unused1, out unused2);
        }


        //void start_save_state();
        //void start_load_state();


        // slider controls
        //-------------------------------------------------
        //  ui_get_slider_list - get the list of sliders
        //-------------------------------------------------
        public std.vector<ui.menu_item> get_slider_list()
        {
            return slider_list;
        }


        // metrics
        float target_font_height() { return m_target_font_height; }
        public float box_lr_border() { return target_font_height() * 0.25f; }
        public float box_tb_border() { return target_font_height() * 0.25f; }
        //void update_target_font_height();


        // other
        //-------------------------------------------------
        //  process_natural_keyboard - processes any
        //  natural keyboard input
        //-------------------------------------------------
        void process_natural_keyboard()
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  create_layout
        //-------------------------------------------------
        ui.text_layout create_layout(render_container container, float width = 1.0f, ui.text_layout.text_justify justify = ui.text_layout.text_justify.LEFT, ui.text_layout.word_wrapping wrap = ui.text_layout.word_wrapping.WORD)
        {
            // determine scale factors
            float yscale = get_line_height();
            float xscale = yscale * machine().render().ui_aspect(container);

            // create the layout
            return new ui.text_layout(get_font(), xscale, yscale, width, justify, wrap);
        }


        // word wrap
        //-------------------------------------------------
        //  wrap_text
        //-------------------------------------------------
        public int wrap_text(render_container container, string origs, float x, float y, float origwrapwidth, out std.vector<int> xstart, out std.vector<int> xend, float text_size = 1.0f)
        {
            // create the layout
            var layout = create_layout(container, origwrapwidth, ui.text_layout.text_justify.LEFT, ui.text_layout.word_wrapping.WORD);

            // add the text
            layout.add_text(
                    origs,
                    rgb_t.black(),
                    rgb_t.black(),
                    text_size);

            // and get the wrapping info
            return layout.get_wrap_info(out xstart, out xend);
        }


        // draw an outlined box with given line color and filled with a texture
        //-------------------------------------------------
        //  draw_textured_box - add primitives to
        //  draw an outlined box with the given
        //  textured background and line color
        //-------------------------------------------------
        public void draw_textured_box(render_container container, float x0, float y0, float x1, float y1, rgb_t backcolor, rgb_t linecolor) { draw_textured_box(container, x0, y0, x1, y1, backcolor, linecolor, null); }
        public void draw_textured_box(render_container container, float x0, float y0, float x1, float y1, rgb_t backcolor, rgb_t linecolor, render_texture texture) { draw_textured_box(container, x0, y0, x1, y1, backcolor, linecolor, texture, g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA)); }
        public void draw_textured_box(render_container container, float x0, float y0, float x1, float y1, rgb_t backcolor, rgb_t linecolor, render_texture texture, uint32_t flags)  //void draw_textured_box(render_container &container, float x0, float y0, float x1, float y1, rgb_t backcolor, rgb_t linecolor, render_texture *texture = nullptr, uint32_t flags = PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));
        {
            container.add_quad(x0, y0, x1, y1, backcolor, texture, flags);
            container.add_line(x0, y0, x1, y0, g.UI_LINE_WIDTH, linecolor, g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
            container.add_line(x1, y0, x1, y1, g.UI_LINE_WIDTH, linecolor, g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
            container.add_line(x1, y1, x0, y1, g.UI_LINE_WIDTH, linecolor, g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
            container.add_line(x0, y1, x0, y0, g.UI_LINE_WIDTH, linecolor, g.PRIMFLAG_BLENDMODE(g.BLENDMODE_ALPHA));
        }


        protected override void popup_time_string(int seconds, string message)
        {
            // extract the text
            messagebox_poptext = message;

            // set a timer
            m_popup_text_end = osdcore_global.m_osdcore.osd_ticks() + osdcore_global.m_osdcore.osd_ticks_per_second() * (osd_ticks_t)seconds;
        }


        protected override void menu_reset()
        {
            ui.menu.stack_reset(machine());
        }


        // UI handlers

        //-------------------------------------------------
        //  handler_ingame - in-game handler takes care
        //  of the standard keypresses
        //-------------------------------------------------
        uint32_t handler_ingame(render_container container, mame_ui_manager mui)
        {
            bool is_paused = machine().paused();

            if (show_fps_counter())
                draw_fps_counter(container);

            // Show the duration of current part (intro or gameplay or extra)
            if (show_timecode_counter())
                draw_timecode_counter(container);

            // Show the total time elapsed for the video preview (all parts intro, gameplay, extras)
            if (show_timecode_total())
                draw_timecode_total(container);

            // draw the profiler if visible
            if (show_profiler())
                draw_profiler(container);

            // if we're single-stepping, pause now
            if (single_step())
            {
                machine().pause();
                set_single_step(false);
            }

            // determine if we should disable the rest of the UI
            bool has_keyboard = machine_info().has_keyboard();
            bool ui_disabled = (has_keyboard && !machine().ui_active());

            // is ScrLk UI toggling applicable here?
            if (has_keyboard)
            {
                // are we toggling the UI with ScrLk?
                if (machine().ui_input().pressed((int)ioport_type.IPT_UI_TOGGLE_UI))
                {
                    // toggle the UI
                    machine().set_ui_active(!machine().ui_active());

                    // display a popup indicating the new status
                    string name = machine().input().seq_name(machine().ioport().type_seq(ioport_type.IPT_UI_TOGGLE_UI));
                    if (machine().ui_active())
                        popup_time(2, g.__("UI controls enabled\nUse {0} to toggle"), name);
                    else
                        popup_time(2, g.__("UI controls disabled\nUse {0} to toggle"), name);
                }
            }

            // is the natural keyboard enabled?
            if (machine().natkeyboard().in_use() && (machine().phase() == machine_phase.RUNNING))
                process_natural_keyboard();

            if (!ui_disabled)
            {
                // paste command
                if (machine().ui_input().pressed((int)ioport_type.IPT_UI_PASTE))
                    machine().natkeyboard().paste();
            }

            image_handler_ingame();

            // handle a save input timecode request
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_TIMECODE))
                machine().video().save_input_timecode();

            if (ui_disabled)
                return ui_disabled ? 1U : 0U;

            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_CANCEL))
            {
                request_quit();
                return 0;
            }

            // turn on menus if requested
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_CONFIGURE))
            {
                show_menu();
                return 0;
            }

            // if the on-screen display isn't up and the user has toggled it, turn it on
            if ((machine().debug_flags & g.DEBUG_FLAG_ENABLED) == 0 && machine().ui_input().pressed((int)ioport_type.IPT_UI_ON_SCREEN_DISPLAY))
            {
                //using namespace std::placeholders;
                set_handler(ui_callback_type.MENU, ui.menu_sliders.ui_handler);  //, _1, std::ref_(*this)));
                return 1;
            }

            // handle a reset request
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_RESET_MACHINE))
                machine().schedule_hard_reset();
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_SOFT_RESET))
                machine().schedule_soft_reset();

            // handle a request to display graphics/palette
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_SHOW_GFX))
            {
                if (!is_paused)
                    machine().pause();

                //using namespace std::placeholders;
                set_handler(ui_callback_type.VIEWER, viewgfx_global.ui_gfx_ui_handler);  //, _1, std::ref_(*this), is_paused));
                return is_paused ? (UInt32)1 : 0;
            }

            // handle a tape control key
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_TAPE_START))
            {
                foreach (cassette_image_device cass in new cassette_device_enumerator(machine().root_device()))
                {
                    cass.change_state(cassette_state.CASSETTE_PLAY, cassette_state.CASSETTE_MASK_UISTATE);
                    return 0;
                }
            }
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_TAPE_STOP))
            {
                foreach (cassette_image_device cass in new cassette_device_enumerator(machine().root_device()))
                {
                    cass.change_state(cassette_state.CASSETTE_STOPPED, cassette_state.CASSETTE_MASK_UISTATE);
                    return 0;
                }
            }

            // handle a save state request
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_SAVE_STATE))
            {
                throw new emu_unimplemented();
#if false
                start_save_state();
                return LOADSAVE_SAVE;
#endif
            }

            // handle a load state request
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_LOAD_STATE))
            {
                throw new emu_unimplemented();
#if false
                start_load_state();
                return LOADSAVE_LOAD;
#endif
            }

            // handle a save snapshot request
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_SNAPSHOT))
                machine().video().save_active_screen_snapshots();

            // toggle pause
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_PAUSE))
                machine().toggle_pause();

            // pause single step
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_PAUSE_SINGLE))
            {
                machine().rewind_capture();
                set_single_step(true);
                machine().resume();
            }

            // rewind single step
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_REWIND_SINGLE))
                machine().rewind_step();

            // handle a toggle cheats request
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_TOGGLE_CHEAT))
                mame_machine_manager.instance().cheat().set_enable(!mame_machine_manager.instance().cheat().enabled());

            // toggle MNG recording
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_RECORD_MNG))
                machine().video().toggle_record_movie(movie_recording.format.MNG);

            // toggle AVI recording
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_RECORD_AVI))
                machine().video().toggle_record_movie(movie_recording.format.AVI);

            // toggle profiler display
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_SHOW_PROFILER))
                set_show_profiler(!show_profiler());

            // toggle FPS display
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_SHOW_FPS))
                set_show_fps(!show_fps());

            // increment frameskip?
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_FRAMESKIP_INC))
                increase_frameskip();

            // decrement frameskip?
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_FRAMESKIP_DEC))
                decrease_frameskip();

            // toggle throttle?
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_THROTTLE))
            {
                bool new_throttle_state = !machine().video().throttled();
                machine().video().set_throttled(new_throttle_state);
                if (m_unthrottle_mute)
                    machine().sound().ui_mute(!new_throttle_state);
            }

            // check for fast forward
            if (machine().ioport().type_pressed(ioport_type.IPT_UI_FAST_FORWARD))
            {
                machine().video().set_fastforward(true);
                show_fps_temp(0.5);
            }
            else
            {
                machine().video().set_fastforward(false);
            }

            return 0;
        }


        //-------------------------------------------------
        //  handler_confirm_quit - leads the user through
        //  confirming quit emulation
        //-------------------------------------------------
        UInt32 handler_confirm_quit(render_container container, mame_ui_manager mui)
        {
            UInt32 state = 0;

            // get the text for 'UI Select'
            string ui_select_text = machine().input().seq_name(machine().ioport().type_seq(ioport_type.IPT_UI_SELECT, 0, input_seq_type.SEQ_TYPE_STANDARD));

            // get the text for 'UI Cancel'
            string ui_cancel_text = machine().input().seq_name(machine().ioport().type_seq(ioport_type.IPT_UI_CANCEL, 0, input_seq_type.SEQ_TYPE_STANDARD));

            // assemble the quit message
            string quit_message = string.Format("Are you sure you want to quit?\n\n" +
                                                "Press ''{0}'' to quit,\n" + 
                                                "Press ''{1}'' to return to emulation.", ui_select_text, ui_cancel_text);

            draw_text_box(container, quit_message, ui.text_layout.text_justify.CENTER, 0.5f, 0.5f, g.UI_RED_COLOR);
            machine().pause();

            // if the user press ENTER, quit the game
            if (machine().ui_input().pressed((int)ioport_type.IPT_UI_SELECT))
                machine().schedule_exit();

            // if the user press ESC, just continue
            else if (machine().ui_input().pressed((int)ioport_type.IPT_UI_CANCEL))
            {
                machine().resume();
                state = g.UI_HANDLER_CANCEL;
            }

            return state;
        }


        // private methods

        //-------------------------------------------------
        //  update_target_font_height
        //-------------------------------------------------
        void update_target_font_height()
        {
            m_target_font_height = 1.0f / options().font_rows();
        }


        //-------------------------------------------------
        //  exit - clean up ourselves on exit
        //-------------------------------------------------
        void exit(running_machine machine_)
        {
            // free the mouse texture
            machine().render().texture_free(m_mouse_arrow_texture);
            m_mouse_arrow_texture = null;

            // free the font
            m_font = null;  //m_font.reset();
        }


        //-------------------------------------------------
        //  config_load - load configuration data
        //-------------------------------------------------
        void config_load(config_type cfg_type, config_level cfg_level, util.xml.data_node parentnode)
        {
            // make sure it's relevant and there's data available
            if (config_type.SYSTEM == cfg_type)
            {
                m_unemulated_features.clear();
                m_imperfect_features.clear();
                if (parentnode == null)
                {
                    m_last_launch_time = (std_time_t)(-1);
                    m_last_warning_time = (std_time_t)(-1);
                }
                else
                {
                    m_last_launch_time = (std_time_t)parentnode.get_attribute_int("launched", -1);
                    m_last_warning_time = (std_time_t)parentnode.get_attribute_int("warned", -1);
                    for (util.xml.data_node node = parentnode.get_first_child(); node != null; node = node.get_next_sibling())
                    {
                        if (std.strcmp(node.get_name(), "feature") == 0)
                        {
                            string device = node.get_attribute_string("device", null);
                            string feature = node.get_attribute_string("type", null);
                            string status = node.get_attribute_string("status", null);
                            if (!string.IsNullOrEmpty(device) && !string.IsNullOrEmpty(feature) && !string.IsNullOrEmpty(status))  //if (device && *device && feature && *feature && status && *status)
                            {
                                if (std.strcmp(status, "unemulated") == 0)
                                    m_unemulated_features.emplace(new std.pair<string, string>(device, feature));
                                else if (std.strcmp(status, "imperfect") == 0)
                                    m_imperfect_features.emplace(new std.pair<string, string>(device, feature));
                            }
                        }
                    }
                }
            }
        }


        //-------------------------------------------------
        //  config_save - save configuration data
        //-------------------------------------------------
        void config_save(config_type cfg_type, util.xml.data_node parentnode)
        {
            // only save system-level configuration when times are valid
            if ((config_type.SYSTEM == cfg_type) && ((std_time_t)(-1) != m_last_launch_time) && ((std_time_t)(-1) != m_last_warning_time))
            {
                parentnode.set_attribute_int("launched", (Int64)m_last_launch_time);
                parentnode.set_attribute_int("warned", (Int64)m_last_warning_time);

                foreach (var feature in m_unemulated_features)
                {
                    util.xml.data_node feature_node = parentnode.add_child("feature", null);
                    feature_node.set_attribute("device", feature.first);
                    feature_node.set_attribute("type", feature.second);
                    feature_node.set_attribute("status", "unemulated");
                }

                foreach (var feature in m_imperfect_features)
                {
                    util.xml.data_node feature_node = parentnode.add_child("feature", null);
                    feature_node.set_attribute("device", feature.first);
                    feature_node.set_attribute("type", feature.second);
                    feature_node.set_attribute("status", "imperfect");
                }
            }
        }


        // slider controls
        //-------------------------------------------------
        //  slider_alloc - allocate a new slider entry
        //-------------------------------------------------
        //template <typename... Params>
        void slider_alloc(string title, int32_t min, int32_t def, int32_t max, int32_t inc, slider_update func) { m_sliders.push_back(new slider_state(title, min, def, max, inc, func)); }  //template <typename... Params> void slider_alloc(Params &&...args) { m_sliders.push_back(std::make_unique<slider_state>(std::forward<Params>(args)...)); }


        // slider controls
        //int32_t slider_volume(std::string *str, int32_t newval);
        //int32_t slider_mixervol(int item, std::string *str, int32_t newval);
        //int32_t slider_adjuster(ioport_field &field, std::string *str, int32_t newval);
        //int32_t slider_overclock(device_t &device, std::string *str, int32_t newval);
        //int32_t slider_refresh(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_brightness(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_contrast(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_gamma(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_xscale(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_yscale(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_xoffset(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_yoffset(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_overxscale(laserdisc_device &laserdisc, std::string *str, int32_t newval);
        //int32_t slider_overyscale(laserdisc_device &laserdisc, std::string *str, int32_t newval);
        //int32_t slider_overxoffset(laserdisc_device &laserdisc, std::string *str, int32_t newval);
        //int32_t slider_overyoffset(laserdisc_device &laserdisc, std::string *str, int32_t newval);
        //int32_t slider_flicker(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_beam_width_min(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_beam_width_max(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_beam_dot_size(screen_device &screen, std::string *str, int32_t newval);
        //int32_t slider_beam_intensity_weight(screen_device &screen, std::string *str, int32_t newval);


        //-------------------------------------------------
        //  slider_get_screen_desc - returns the
        //  description for a given screen
        //-------------------------------------------------
        public static string slider_get_screen_desc(screen_device screen)
        {
            if (new screen_device_enumerator(screen.machine().root_device()).count() > 1)
                return string.Format("Screen '{0}'", screen.tag());  // %1$s
            else
                return "Screen";
        }


#if MAME_DEBUG
        int32_t slider_crossscale(ioport_field &field, std::string *str, int32_t newval);
        int32_t slider_crossoffset(ioport_field &field, std::string *str, int32_t newval);
#endif


        //-------------------------------------------------
        //  load ui options
        //-------------------------------------------------
        void load_ui_options(running_machine machine)
        {
            // parse the file
            // attempt to open the output file
            emu_file file = new emu_file(machine.options().ini_path(), g.OPEN_FLAG_READ);
            if (file.open("ui.ini") == osd_file.error.NONE)
            {
                try
                {
                    options().parse_ini_file(file.core_file_get(), mame_options.OPTION_PRIORITY_MAME_INI, mame_options.OPTION_PRIORITY_MAME_INI < mame_options.OPTION_PRIORITY_DRIVER_INI, true);  //options().parse_ini_file((util::core_file &)file, OPTION_PRIORITY_MAME_INI, OPTION_PRIORITY_MAME_INI < OPTION_PRIORITY_DRIVER_INI, true);
                }
                catch (options_exception )
                {
                    g.osd_printf_error("**Error loading ui.ini**\n");
                }

                file.close();
            }
        }


        //-------------------------------------------------
        //  is_breakable_char - is a given unicode
        //  character a possible line break?
        //-------------------------------------------------
        static int is_breakable_char(char32_t ch)
        {
            // regular spaces and hyphens are breakable
            if (ch == ' ' || ch == '-')
                return 1;

            // In the following character sets, any character is breakable:
            //  Hiragana (3040-309F)
            //  Katakana (30A0-30FF)
            //  Bopomofo (3100-312F)
            //  Hangul Compatibility Jamo (3130-318F)
            //  Kanbun (3190-319F)
            //  Bopomofo Extended (31A0-31BF)
            //  CJK Strokes (31C0-31EF)
            //  Katakana Phonetic Extensions (31F0-31FF)
            //  Enclosed CJK Letters and Months (3200-32FF)
            //  CJK Compatibility (3300-33FF)
            //  CJK Unified Ideographs Extension A (3400-4DBF)
            //  Yijing Hexagram Symbols (4DC0-4DFF)
            //  CJK Unified Ideographs (4E00-9FFF)
            if (ch >= 0x3040 && ch <= 0x9fff)
                return 1;

            // Hangul Syllables (AC00-D7AF) are breakable
            if (ch >= 0xac00 && ch <= 0xd7af)
                return 1;

            // CJK Compatibility Ideographs (F900-FAFF) are breakable
            if (ch >= 0xf900 && ch <= 0xfaff)
                return 1;

            return 0;
        }
    }
}
