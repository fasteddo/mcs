// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;


namespace mame.ui
{
    // types of menu items (TODO: please expand)
    public enum menu_item_type
    {
        UNKNOWN,
        SLIDER,
        SEPARATOR
    }


    public class menu_item
    {
        public const string MENU_SEPARATOR_ITEM         = "---";


        public string text;
        public string subtext;
        public uint32_t flags;
        public object ref_;  //void            *ref;
        public menu_item_type type;   // item type (eventually will go away when itemref is proper ui_menu_item class rather than void*)


        //menu_item() = default;
        //menu_item(menu_item const &) = default;
        //menu_item(menu_item &&) = default;
        //menu_item &operator=(menu_item const &) = default;
        //menu_item &operator=(menu_item &&) = default;
    }
}
