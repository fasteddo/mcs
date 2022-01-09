// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;
using unsigned = System.UInt32;


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


        string m_text;
        string m_subtext;
        object m_ref;  //void            *m_ref;
        uint32_t m_flags;
        unsigned m_generation = 0;
        menu_item_type m_type;


        //menu_item(menu_item const &) = default;
        //menu_item(menu_item &&) = default;
        //menu_item &operator=(menu_item const &) = default;
        //menu_item &operator=(menu_item &&) = default;

        public menu_item(menu_item_type t = menu_item_type.UNKNOWN, object r = null, uint32_t f = 0) { m_ref = r; m_flags = f; m_type = t; }  //menu_item(menu_item_type t = menu_item_type::UNKNOWN, void *r = nullptr, uint32_t f = 0) : m_ref(r), m_flags(f), m_type(t)


        public string text() { return m_text; }
        public string subtext() { return m_subtext; }
        public object ref_() { return m_ref; }  //void *ref() const noexcept { return m_ref; }
        public uint32_t flags() { return m_flags; }
        //unsigned generation() const noexcept { return m_generation; }
        public menu_item_type type() { return m_type; }

        public void set_text(string args) { m_text = args; ++m_generation; }  //template <typename... T> void set_text(T &&... args) { m_text.assign(std::forward<T>(args)...); ++m_generation; }
        public void set_subtext(string args) { m_subtext = args; ++m_generation; }  //template <typename... T> void set_subtext(T &&... args) { m_subtext.assign(std::forward<T>(args)...); ++m_generation; }
        //void set_flags(uint32_t f) noexcept { m_flags = f; ++m_generation; }
    }
}
