// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Linq;

using ioport_value = System.UInt32;
using layout_element_environment = mame.emu.render.detail.layout_environment;  //using environment = emu::render::detail::layout_environment;
using layout_element_make_component_map = mame.std.map<string, mame.layout_element.make_component_func>;  //typedef std::map<std::string, make_component_func> make_component_map;
using layout_environment_entry_vector = mame.std.vector<mame.emu.render.detail.layout_environment.entry>;  //using entry_vector = std::vector<entry>;
using layout_file_element_map = mame.std.unordered_map<string, mame.layout_element>;  //using element_map = std::unordered_map<std::string, layout_element>;
using layout_file_environment = mame.emu.render.detail.layout_environment;  //using environment = emu::render::detail::layout_environment;
using layout_file_group_map = mame.std.unordered_map<string, mame.layout_group>;  //using group_map = std::unordered_map<std::string, layout_group>;
using layout_file_view_list = mame.std.list<mame.layout_view>;  //using view_list = std::list<layout_view>;
using layout_group_environment = mame.emu.render.detail.layout_environment;  //using environment = emu::render::detail::layout_environment;
using layout_group_group_map = mame.std.unordered_map<string, mame.layout_group>;  //using group_map = std::unordered_map<std::string, layout_group>;
using layout_view_element_map = mame.std.unordered_map<string, mame.layout_element>;  //using element_map = std::unordered_map<std::string, layout_element>;
using layout_view_group_map = mame.std.unordered_map<string, mame.layout_group>;  //using group_map = std::unordered_map<std::string, layout_group>;
using layout_view_item_list = mame.std.list<mame.layout_view.item>;  //using item_list = std::list<item>;
using layout_view_view_environment = mame.emu.render.detail.view_environment;  //using view_environment = emu::render::detail::view_environment;
using s64 = System.Int64;
using u8 = System.Byte;
using u32 = System.UInt32;
using unsigned = System.UInt32;


namespace mame
{
    public static class rendlay_global
    {
        public const int LOG_GROUP_BOUNDS_RESOLUTION = 1 << 1;
        public const int LOG_INTERACTIVE_ITEMS       = 1 << 2;

        ////#define VERBOSE (LOG_GROUP_BOUNDS_RESOLUTION | LOG_INTERACTIVE_ITEMS)
        public const int VERBOSE = 0;
        //#define LOG_OUTPUT_FUNC osd_printf_verbose
        //#include "logmacro.h"
        public static void LOGMASKED(int mask, string format, params object [] args) { if ((VERBOSE & mask) != 0) global_object.osd_printf_verbose(format, args); }


        public const int LAYOUT_VERSION = 2;


        //enum
        //{
        public const int LINE_CAP_NONE  = 0;
        public const int LINE_CAP_START = 1;
        public const int LINE_CAP_END   = 2;
        //}


        public static readonly float [,] identity_transform = new float[,] { { 1.0F, 0.0F, 0.0F }, { 0.0F, 1.0F, 0.0F }, { 0.0F, 0.0F, 1.0F } };  //layout_group.transform identity_transform {{ {{ 1.0F, 0.0F, 0.0F }}, {{ 0.0F, 1.0F, 0.0F }}, {{ 0.0F, 0.0F, 1.0F }} }};


        public static void render_bounds_transform(ref render_bounds bounds, float [,] trans)  //inline void render_bounds_transform(render_bounds &bounds, layout_group::transform const &trans)
        {
            bounds = new render_bounds()
            {
                x0 = (bounds.x0 * trans[0, 0]) + (bounds.y0 * trans[0, 1]) + trans[0, 2],
                y0 = (bounds.x0 * trans[1, 0]) + (bounds.y0 * trans[1, 1]) + trans[1, 2],
                x1 = (bounds.x1 * trans[0, 0]) + (bounds.y1 * trans[0, 1]) + trans[0, 2],
                y1 = (bounds.x1 * trans[1, 0]) + (bounds.y1 * trans[1, 1]) + trans[1, 2]
            };
        }


        public static render_color render_color_multiply(render_color x, render_color y)
        {
            return new render_color() { a = x.a * y.a, r = x.r * y.r, g = x.g * y.g, b = x.b * y.b };
        }
    }


    class layout_syntax_error : ArgumentException { public layout_syntax_error(string format, params object [] args) : base(string.Format(format, args)) { } }
    class layout_reference_error : ArgumentOutOfRangeException { public layout_reference_error(string format, params object [] args) : base(string.Format(format, args)) { } }



    namespace emu.render.detail
    {
        public class layout_environment : global_object
        {
            public class entry
            {
                string m_name;
                string m_text;
                s64 m_int = 0;
                s64 m_int_increment = 0;
                double m_float = 0.0;
                double m_float_increment = 0.0;
                int m_shift = 0;
                bool m_text_valid = false;
                bool m_int_valid = false;
                bool m_float_valid = false;
                bool m_generator = false;


                public entry(string name, string t)
                {
                    m_name = name;
                    m_text = t;
                    m_text_valid = true;
                }

                public entry(string name, s64 i)
                {
                    m_name = name;
                    m_int = i;
                    m_int_valid = true;
                }

                public entry(string name, double f)
                {
                    m_name = name;
                    m_float = f;
                    m_float_valid = true;
                }

                public entry(string name, string t, s64 i, int s)
                {
                    m_name = name;
                    m_text = t;
                    m_int_increment = i;
                    m_shift = s;
                    m_text_valid = true;
                    m_generator = true;
                }

                public entry(string name, string t, double i, int s)
                {
                    m_name = name;
                    m_text = t;
                    m_float_increment = i;
                    m_shift = s;
                    m_text_valid = true;
                    m_generator = true;
                }

                //entry(entry &&) = default;
                //entry &operator=(entry &&) = default;


                public void set(string t)
                {
                    m_text = t;
                    m_text_valid = true;
                    m_int_valid = false;
                    m_float_valid = false;
                }

                public void set(s64 i)
                {
                    m_int = i;
                    m_text_valid = false;
                    m_int_valid = true;
                    m_float_valid = false;
                }

                public void set(double f)
                {
                    m_float = f;
                    m_text_valid = false;
                    m_int_valid = false;
                    m_float_valid = true;
                }


                public string name() { return m_name; }
                public bool is_generator() { return m_generator; }


                public string get_text()
                {
                    if (!m_text_valid)
                    {
                        if (m_float_valid)
                        {
                            m_text = m_float.ToString();  //m_text = std::to_string(m_float);
                            m_text_valid = true;
                        }
                        else if (m_int_valid)
                        {
                            m_text = m_int.ToString();  //m_text = std::to_string(m_int);
                            m_text_valid = true;
                        }
                    }

                    return m_text;
                }


                //void increment()
                //static bool name_less(entry const &lhs, entry const &rhs) { return lhs.name() < rhs.name(); }
            }

            //using entry_vector = std::vector<entry>;

#if false
            template <typename T, typename U>
            void try_insert(T &&name, U &&value)
            {
                entry_vector::iterator const pos(
                        std::lower_bound(
                            m_entries.begin(),
                            m_entries.end(),
                            name,
                            [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                if ((m_entries.end() == pos) || (pos->name() != name))
                    m_entries.emplace(pos, std::forward<T>(name), std::forward<U>(value));
            }
#endif


            //template <typename T, typename U>
            void set(string name, object value)  //void set(T &&name, U &&value)
            {
                //entry_vector::iterator const pos(
                //        std::lower_bound(
                //            m_entries.begin(),
                //            m_entries.end(),
                //            name,
                //            [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                //if ((m_entries.end() == pos) || (pos->name() != name))
                //    m_entries.emplace(pos, std::forward<T>(name), std::forward<U>(value));
                //else
                //    pos->set(std::forward<U>(value));
                int pos = 0;
                for (; pos < m_entries.Count; pos++)
                {
                    if (m_entries[pos].name().CompareTo(name) >= 0)
                        break;
                }

                if ((m_entries.Count == pos) || (m_entries[pos].name() != name))
                {
                    if (value is string)      m_entries.emplace(pos, new entry(name, (string)value));
                    else if (value is s64)    m_entries.emplace(pos, new entry(name, (s64)value));
                    else if (value is double) m_entries.emplace(pos, new entry(name, (double)value));
                    else throw new emu_unimplemented();
                }
                else
                {
                    if (value is string)      m_entries[pos].set((string)value);
                    else if (value is s64)    m_entries[pos].set((s64)value);
                    else if (value is double) m_entries[pos].set((double)value);
                    else throw new emu_unimplemented();
                }
            }


            void cache_device_entries()
            {
                throw new emu_unimplemented();
            }


            entry find_entry(string name)  //entry *find_entry(char const *begin, char const *end)
            {
                cache_device_entries();
                //entry_vector::iterator const pos(
                //        std::lower_bound(
                //            m_entries.begin(),
                //            m_entries.end(),
                //            std::make_pair(begin, end - begin),
                //            [] (entry const &lhs, std::pair<char const *, std::ptrdiff_t> const &rhs)
                //            { return 0 > std::strncmp(lhs.name().c_str(), rhs.first, rhs.second); }));
                //if ((m_entries.end() != pos) && (pos->name().length() == (end - begin)) && !std::strncmp(pos->name().c_str(), begin, end - begin))
                //    return &*pos;
                //else
                //    return m_next ? m_next->find_entry(begin, end) : nullptr;
                int pos = 0;
                for (; pos < m_entries.Count; pos++)
                {
                    if (0 <= strcmp(m_entries[pos].name(), name))
                        break;
                }

                if ((m_entries.Count != pos) && (m_entries[pos].name() == name))
                    return m_entries[pos];
                else
                    return m_next != null ? m_next.find_entry(name) : null;
            }


            //template <typename... T>
            KeyValuePair<string, bool> get_variable_text(string name)  //std::tuple<char const *, char const *, bool> get_variable_text(T &&... args)
            {
                entry found = find_entry(name);
                if (found != null)
                {
                    string text = found.get_text();
                    string begin = text.c_str();
                    return new KeyValuePair<string, bool>(begin, true);  //return std::make_tuple(begin, begin + text.length(), true);
                }
                else
                {
                    return new KeyValuePair<string, bool>(null, false);  //return std::make_tuple(nullptr, nullptr, false);
                }
            }


            string expand(string begin)  //std::pair<char const *, char const *> expand(char const *str)
            {
                m_buffer = "";

                // search for candidate variable references
                int startIdx = 0;  //char const *start(begin);
                int i = begin.IndexOf(c => !char.IsWhiteSpace(c));
                int posIdx = begin.IndexOf(is_variable_start);  //char const *pos(std::find_if(start, end, is_variable_start));
                while (posIdx != -1)
                {
                    int termIdx = begin.Substring(1).IndexOf(c => !is_variable_char(c));  //char const *const term(std::find_if(pos + 1, end, [] (char ch) { return !is_variable_char(ch); }));
                    if ((termIdx == -1) || !is_variable_end(begin[termIdx]))
                    {
                        // not a valid variable name - keep searching
                        posIdx = begin.Substring(termIdx).IndexOf(c => is_variable_start(c));  //pos = std::find_if(term, end, is_variable_start);
                    }
                    else
                    {
                        // looks like a variable reference - try to look it up
                        var text = get_variable_text(begin.Substring(posIdx + 1, termIdx - (posIdx + 1)));  //std::tuple<char const *, char const *, bool> const text(get_variable_text(pos + 1, term));
                        if (text.Key != null)  //if (std::get<2>(text))
                        {
                            // variable found
                            if (0 == startIdx)  //if (begin == start)
                                m_buffer = "";  //m_buffer.seekp(0);
                            m_buffer += begin.Substring(startIdx, posIdx - startIdx);  //m_buffer.write(start, pos - start);
                            m_buffer += text.Key;  //m_buffer.write(std::get<0>(text), std::get<1>(text) - std::get<0>(text));
                            startIdx = termIdx + 1;  //start = term + 1;
                            posIdx = begin.Substring(startIdx).IndexOf(c => is_variable_start(c));  //pos = std::find_if(start, end, is_variable_start);
                        }
                        else
                        {
                            // variable not found - move on
                            posIdx = begin.Substring(1).IndexOf(c => is_variable_start(c));  //pos = std::find_if(pos + 1, end, is_variable_start);
                        }
                    }
                }

                // short-circuit the case where no substitutions were made
                if (startIdx == 0)  //if (start == begin)
                {
                    return begin;  //return std::make_pair(begin, end);
                }
                else
                {
                    m_buffer += begin.Substring(startIdx, posIdx - startIdx);  //m_buffer.write(start, pos - start);
                    m_buffer += '\0';  //m_buffer.put('\0');
                    //std.vector<char> vec = m_buffer.vec();
                    if (m_buffer.empty())  //if (vec.empty())
                        return null;  //return std::make_pair(nullptr, nullptr);
                    else
                        return m_buffer;  //return std::make_pair(&vec[0], &vec[0] + vec.size() - 1);
                }
            }


            int parse_int(string str, int defvalue)  //int parse_int(string begin, string end, int defvalue);
            {
                //std::istringstream stream;
                //stream.imbue(f_portable_locale);
                int result;
                if (str.Length >= 1 && str[0] == '$')  //if (begin[0] == '$')
                {
                    //stream.str(std::string(begin + 1, end));
                    //unsigned uvalue;
                    //stream >> std::hex >> uvalue;
                    //result = int(uvalue);
                    result = Convert.ToInt32(str);
                }
                else if (str.Length >= 2 && ((str[0] == '0') && ((str[1] == 'x') || (str[1] == 'X'))))  //else if ((begin[0] == '0') && ((begin[1] == 'x') || (begin[1] == 'X')))
                {
                    //stream.str(std::string(begin + 2, end));
                    //unsigned uvalue;
                    //stream >> std::hex >> uvalue;
                    //result = int(uvalue);
                    result = Convert.ToInt32(str);
                }
                else if (str.Length >= 1 && str[0] == '#')  //else if (begin[0] == '#')
                {
                    //stream.str(std::string(begin + 1, end));
                    //stream >> result;
                    result = Convert.ToInt32(str);
                }
                else
                {
                    //stream.str(std::string(begin, end));
                    //stream >> result;
                    result = Convert.ToInt32(str);
                }

                return !string.IsNullOrEmpty(str) ? result : defvalue;  //return stream ? result : defvalue;
            }


            string parameter_name(util.xml.data_node node)
            {
                string attrib = node.get_attribute_string("name", null);
                if (attrib == null)
                    throw new layout_syntax_error("parameter lacks name attribute");
                var expanded = expand(attrib);  //std::pair<char const *, char const *> const expanded(expand(attrib));
                return expanded;  //return std::string(expanded.first, expanded.second);
            }

            static bool is_variable_start(char ch) { return '~' == ch; }
            static bool is_variable_end(char ch) { return '~' == ch; }
            static bool is_variable_char(char ch) { return (('0' <= ch) && ('9' >= ch)) || (('A' <= ch) && ('Z' >= ch)) || (('a' <= ch) && ('z' >= ch)) || ('_' == ch); }


            layout_environment_entry_vector m_entries = new layout_environment_entry_vector();
            string m_buffer;  //util::ovectorstream m_buffer;
            device_t m_device;
            layout_environment m_next = null;
            //bool m_cached = false;


            public layout_environment(device_t device) { m_device = device; }
            public layout_environment(layout_environment next) { m_device = next.m_device; m_next = next; }
            //layout_environment(layout_environment const &) = delete;


            public device_t device() { return m_device; }
            public running_machine machine() { return device().machine(); }

            public bool is_root_device() { return device() == machine().root_device(); }

            public void set_parameter(string name, string value) { set(name, value); }
            public void set_parameter(string name, s64 value) { set(name, value); }
            public void set_parameter(string name, double value) { set(name, value); }

            public void set_parameter(util.xml.data_node node)
            {
                // do basic validation
                string name = parameter_name(node);
                if (node.has_attribute("start") || node.has_attribute("increment") || node.has_attribute("lshift") || node.has_attribute("rshift"))
                    throw new layout_syntax_error("start/increment/lshift/rshift attributes are only allowed for repeat parameters");
                string value = node.get_attribute_string("value", null);
                if (value == null)
                    throw new layout_syntax_error("parameter lacks value attribute");

                // expand value and stash
                var expanded = expand(value);  //std::pair<char const *, char const *> const expanded(expand(value));
                set(name, expanded);  //set(std::move(name), std::string(expanded.first, expanded.second));
            }


            public void set_repeat_parameter(util.xml.data_node node, bool init)
            {
                // two types are allowed here - static value, and start/increment/lshift/rshift
                string name = parameter_name(node);
                string start = node.get_attribute_string("start", null);
                if (start != null)
                {
                    // simple validity checks
                    if (node.has_attribute("value"))
                        throw new layout_syntax_error("start attribute may not be used in combination with value attribute");

                    int lshift = node.has_attribute("lshift") ? get_attribute_int(node, "lshift", -1) : 0;
                    int rshift = node.has_attribute("rshift") ? get_attribute_int(node, "rshift", -1) : 0;
                    if ((0 > lshift) || (0 > rshift))
                        throw new layout_syntax_error("lshift/rshift attributes must be non-negative integers");

                    // increment is more complex - it may be an integer or a floating-point number
                    s64 intincrement = 0;
                    double floatincrement = 0;
                    string increment = node.get_attribute_string("increment", null);
                    if (increment != null)
                    {
                        var expanded = expand(increment);  //std::pair<char const *, char const *> const expanded(expand(increment));
                        int hexprefix = (expanded[0] == '$') ? 1 : ((expanded[0] == '0') && ((expanded[1] == 'x') || (expanded[1] == 'X'))) ? 2 : 0;
                        int decprefix = (expanded[0] == '#') ? 1 : 0;
                        bool floatchars = expanded.Contains('.') || expanded.Contains('e') || expanded.Contains('E');  //bool floatchars = std.find_if(expanded.first, expanded.second, [] (char ch) { return ('.' == ch) || ('e' == ch) || ('E' == ch); }) != expanded.second;

                        //std::istringstream stream(std::string(expanded.first + hexprefix + decprefix, expanded.second));
                        //stream.imbue(f_portable_locale);
                        string stream = expanded.Substring(hexprefix + decprefix);
                        bool success = true;
                        if (hexprefix == 0 && decprefix == 0 && floatchars)
                        {
                            //stream >> floatincrement;
                            success = double.TryParse(stream, out floatincrement);
                        }
                        else if (hexprefix != 0)
                        {
                            //u64 uvalue;
                            //stream >> std::hex >> uvalue;
                            //intincrement = s64(uvalue);
                            try { intincrement = Convert.ToInt64(stream, 16); }
                            catch (Exception) { success = false; }
                        }
                        else
                        {
                            //stream >> intincrement;
                            success = s64.TryParse(stream, out intincrement);
                        }

                        // reject obviously bad stuff
                        if (!success)
                            throw new layout_syntax_error("increment attribute must be a number");
                    }

                    // don't allow generator parameters to be redefined
                    if (init)
                    {
                        //entry_vector::iterator const pos(
                        //        std::lower_bound(
                        //            m_entries.begin(),
                        //            m_entries.end(),
                        //            name,
                        //            [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                        //if ((m_entries.end() != pos) && (pos->name() == name))
                        //    throw new rendlay_global.layout_syntax_error("generator parameters must be defined exactly once per scope");
                        int pos = 0;
                        for (; pos < m_entries.Count; pos++)
                        {
                            if (m_entries[pos].name().CompareTo(name) >= 0)
                                break;
                        }

                        if (pos != m_entries.Count && m_entries[pos].name() == name)
                            throw new layout_syntax_error("generator parameters must be defined exactly once per scope");

                        var expanded = expand(start);  //std::pair<char const *, char const *> const expanded(expand(start));
                        if (floatincrement != 0)
                            m_entries.emplace(pos, new entry(name, expanded, floatincrement, lshift - rshift));
                        else
                            m_entries.emplace(pos, new entry(name, expanded, intincrement, lshift - rshift));
                    }
                }
                else if (node.has_attribute("increment") || node.has_attribute("lshift") || node.has_attribute("rshift"))
                {
                    throw new layout_syntax_error("increment/lshift/rshift attributes require start attribute");
                }
                else
                {
                    string value = node.get_attribute_string("value", null);
                    if (value == null)
                        throw new layout_syntax_error("parameter lacks value attribute");
                    var expanded = expand(value);  //std::pair<char const *, char const *> const expanded(expand(value));
                    //entry_vector::iterator const pos(
                    //        std::lower_bound(
                    //            m_entries.begin(),
                    //            m_entries.end(),
                    //            name,
                    //            [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                    //if ((m_entries.end() == pos) || (pos->name() != name))
                    //    m_entries.emplace(pos, std::move(name), std::string(expanded.first, expanded.second));
                    //else if (pos->is_generator())
                    //    throw new rendlay_global.layout_syntax_error("generator parameters must be defined exactly once per scope");
                    //else
                    //    pos->set(std::string(expanded.first, expanded.second));
                    int pos = 0;
                    for (; pos < m_entries.Count; pos++)
                    {
                        if (m_entries[pos].name().CompareTo(name) >= 0)
                            break;
                    }

                    if (pos == m_entries.Count && m_entries[pos].name() != name)
                        m_entries.emplace(pos, new entry(name, expanded));
                    else if (m_entries[pos].is_generator())
                        throw new layout_syntax_error("generator parameters must be defined exactly once per scope");
                    else
                        m_entries[pos].set(expanded);
                }
            }


            public void increment_parameters()
            {
                throw new emu_unimplemented();
            }


            public string get_attribute_string(util.xml.data_node node, string name, string defvalue)
            {
                string attrib = node.get_attribute_string(name, null);
                return attrib != null ? expand(attrib) : defvalue;
            }


            public int get_attribute_int(util.xml.data_node node, string name, int defvalue)
            {
                string attrib = node.get_attribute_string(name, null);
                if (attrib == null)
                    return defvalue;

                // similar to what XML nodes do
                var expanded = expand(attrib);  //std::pair<char const *, char const *> const expanded(expand(attrib));
                return parse_int(expanded, defvalue);
            }


            float get_attribute_float(util.xml.data_node node, string name, float defvalue)
            {
                string attrib = node.get_attribute_string(name, null);
                if (attrib == null)
                    return defvalue;

                // similar to what XML nodes do
                var expanded = expand(attrib);  //std::pair<char const *, char const *> const expanded(expand(attrib));
                //std::istringstream stream(std::string(expanded.first, expanded.second));
                //stream.imbue(f_portable_locale);
                //float result;
                //return (stream >> result) ? result : defvalue;
                string stream = expanded;
                float result;
                return float.TryParse(stream, out result) ? result : defvalue;
            }


            public bool get_attribute_bool(util.xml.data_node node, string name, bool defvalue)
            {
                string attrib = node.get_attribute_string(name, null);
                if (string.IsNullOrEmpty(attrib))
                    return defvalue;

                // first try yes/no strings
                var expanded = expand(attrib);  //std::pair<char const *, char const *> const expanded(expand(attrib));
                if (std.strcmp("yes", expanded) == 0 || std.strcmp("true", expanded) == 0)
                    return true;
                if (std.strcmp("no", expanded) == 0 || std.strcmp("false", expanded) == 0)
                    return false;

                // fall back to integer parsing
                return parse_int(expanded, defvalue ? 1 : 0) != 0;  //return parse_int(expanded.first, expanded.second, defvalue ? 1 : 0) != 0;
            }


            public void parse_bounds(util.xml.data_node node, out render_bounds result)
            {
                result = new render_bounds();

                // default to unit rectangle
                if (node == null)
                {
                    result.x0 = 0.0F;
                    result.y0 = 0.0F;
                    result.x1 = 1.0F;
                    result.y1 = 1.0F;
                }
                else
                {
                    // parse attributes
                    if (node.has_attribute("left"))
                    {
                        // left/right/top/bottom format
                        result.x0 = get_attribute_float(node, "left", 0.0F);
                        result.x1 = get_attribute_float(node, "right", 1.0F);
                        result.y0 = get_attribute_float(node, "top", 0.0F);
                        result.y1 = get_attribute_float(node, "bottom", 1.0F);
                    }
                    else if (node.has_attribute("x"))
                    {
                        // x/y/width/height format
                        result.x0 = get_attribute_float(node, "x", 0.0F);
                        result.x1 = result.x0 + get_attribute_float(node, "width", 1.0F);
                        result.y0 = get_attribute_float(node, "y", 0.0F);
                        result.y1 = result.y0 + get_attribute_float(node, "height", 1.0F);
                    }
                    else
                    {
                        throw new layout_syntax_error("bounds element requires either left or x attribute");
                    }

                    // check for errors
                    if ((result.x0 > result.x1) || (result.y0 > result.y1))
                        throw new layout_syntax_error(string_format("illegal bounds ({0}-{1})-({2}-{3})", result.x0, result.x1, result.y0, result.y1));
                }
            }


            public render_color parse_color(util.xml.data_node node)
            {
                // default to opaque white
                if (node == null)
                    return new render_color() { a = 1.0F, r = 1.0F, g = 1.0F, b = 1.0F };

                // parse attributes
                render_color result = new render_color()
                {
                    a = get_attribute_float(node, "alpha", 1.0F),
                    r = get_attribute_float(node, "red", 1.0F),
                    g = get_attribute_float(node, "green", 1.0F),
                    b = get_attribute_float(node, "blue", 1.0F)
                };

                // check for errors
                if ((0.0F > new [] { result.r, result.g, result.b, result.a }.Min()) || (1.0F < new [] { result.r, result.g, result.b, result.a }.Max()))
                    throw new layout_syntax_error(string_format("illegal RGBA color {0},{1},{2},{3}", result.r, result.g, result.b, result.a));

                return result;
            }


            public int parse_orientation(util.xml.data_node node)
            {
                // default to no transform
                if (node == null)
                    return (int)ROT0;

                // parse attributes
                int result;
                int rotate = get_attribute_int(node, "rotate", 0);
                switch (rotate)
                {
                    case 0:     result = (int)ROT0;      break;
                    case 90:    result = (int)ROT90;     break;
                    case 180:   result = (int)ROT180;    break;
                    case 270:   result = (int)ROT270;    break;
                    default:    throw new layout_syntax_error(string_format("invalid rotate attribute {0}", rotate));
                }

                if (get_attribute_bool(node, "swapxy", false))
                    result ^= (int)ORIENTATION_SWAP_XY;
                if (get_attribute_bool(node, "flipx", false))
                    result ^= (int)ORIENTATION_FLIP_X;
                if (get_attribute_bool(node, "flipy", false))
                    result ^= (int)ORIENTATION_FLIP_Y;

                return result;
            }
        }


        public class view_environment : layout_environment
        {
            view_environment m_next_view = null;
            string m_name;
            u32 m_visibility_mask = 0U;
            unsigned m_next_visibility_bit = 0U;


            public view_environment(layout_environment next, string name)
                : base(next)
            {
                m_name = name;
            }

            public view_environment(view_environment next, bool visibility)
                : base(next)
            {
                m_next_view = next;
                m_name = next.m_name;
                m_visibility_mask = next.m_visibility_mask | ((u32)(visibility ? 1 : 0) << (int)next.m_next_visibility_bit);
                m_next_visibility_bit = next.m_next_visibility_bit + (visibility ? 1U : 0);


                if (32U < m_next_visibility_bit)
                    throw new layout_syntax_error(string_format("view '{0}' contains too many visibility toggles", m_name));
            }

            //~view_environment()
            //{
            //    if (m_next_view)
            //        m_next_view->m_next_visibility_bit = m_next_visibility_bit;
            //}


            public u32 visibility_mask() { return m_visibility_mask; }
        }
    } // namespace emu::render::detail


    public partial class layout_element : global_object
    {
        public abstract partial class component
        {
            // construction/destruction
            //-------------------------------------------------
            //  component - constructor
            //-------------------------------------------------
            public component(layout_element_environment env, util.xml.data_node compnode, string dirname)
            {
                throw new emu_unimplemented();
#if false
#endif
            }


            // helpers

            //-------------------------------------------------
            //  statewrap - get state wraparound requirements
            //-------------------------------------------------
            //std::pair<int, bool> layout_element::component::statewrap() const


            //-------------------------------------------------
            //  overall_bounds - maximum bounds for all states
            //-------------------------------------------------
            //render_bounds layout_element::component::overall_bounds() const


            //-------------------------------------------------
            //  bounds - bounds for a given state
            //-------------------------------------------------
            //render_bounds layout_element::component::bounds(int state) const


            //-------------------------------------------------
            //  color - color for a given state
            //-------------------------------------------------
            //render_color layout_element::component::color(int state) const


            //-------------------------------------------------
            //  normalize_bounds - normalize component bounds
            //-------------------------------------------------
            public void normalize_bounds(float xoffs, float yoffs, float xscale, float yscale)
            {
                throw new emu_unimplemented();
            }


            //-------------------------------------------------
            //  draw_text - draw text in the specified color
            //-------------------------------------------------
            void draw_text(
                    render_font font,
                    bitmap_argb32 dest,
                    rectangle bounds,
                    string str,
                    int align,
                    render_color color)
            {
                // compute premultiplied colors
                u32 r = (u32)(color.r * 255.0f);
                u32 g = (u32)(color.g * 255.0f);
                u32 b = (u32)(color.b * 255.0f);
                u32 a = (u32)(color.a * 255.0f);

                // get the width of the string
                float aspect = 1.0f;
                int width;


                while (true)
                {
                    width = (int)font.string_width(bounds.height(), aspect, str);
                    if (width < bounds.width())
                        break;

                    aspect *= 0.9f;
                }


                // get alignment
                int curx;
                switch (align)
                {
                    // left
                    case 1:
                        curx = bounds.min_x;
                        break;

                    // right
                    case 2:
                        curx = bounds.max_x - width;
                        break;

                    // default to center
                    default:
                        curx = bounds.min_x + (bounds.width() - width) / 2;
                        break;
                }

                // allocate a temporary bitmap
                bitmap_argb32 tempbitmap = new bitmap_argb32(dest.width(), dest.height());

                // loop over characters
                //const char *origs = str;
                //const char *ends = origs + strlen(origs);
                //const char *s = origs;
                //char32_t schar;

                // loop over characters
                foreach (var s in str)
                {
                    // get the font bitmap
                    rectangle chbounds;
                    font.get_scaled_bitmap_and_bounds(tempbitmap, bounds.height(), aspect, s, out chbounds);

                    // copy the data into the target
                    for (int y = 0; y < chbounds.height(); y++)
                    {
                        int effy = bounds.min_y + y;
                        if (effy >= bounds.min_y && effy <= bounds.max_y)
                        {
                            throw new emu_unimplemented();
#if false
                            UInt32 *src = &tempbitmap.pix32(y);
                            UInt32 *d = &dest.pix32(effy);
                            for (int x = 0; x < chbounds.width(); x++)
                            {
                                int effx = curx + x + chbounds.get_min_x();
                                if (effx >= bounds.get_min_x() && effx <= bounds.get_max_x())
                                {
                                    UInt32 spix = (rgb_t)(src[x]).a();
                                    if (spix != 0)
                                    {
                                        rgb_t dpix = d[effx];
                                        UInt32 ta = (a * (spix + 1)) >> 8;
                                        UInt32 tr = (r * ta + dpix.r() * (0x100 - ta)) >> 8;
                                        UInt32 tg = (g * ta + dpix.g() * (0x100 - ta)) >> 8;
                                        UInt32 tb = (b * ta + dpix.b() * (0x100 - ta)) >> 8;
                                        d[effx] = (rgb_t)(tr, tg, tb);
                                    }
                                }
                            }
#endif
                        }
                    }

                    // advance in the X direction
                    curx += (int)font.char_width(bounds.height(), aspect, s);
                }
            }


            //-------------------------------------------------
            //  draw_segment_horizontal_caps - draw a
            //  horizontal LED segment with definable end
            //  and start points
            //-------------------------------------------------
            void draw_segment_horizontal_caps(bitmap_argb32 dest, int minx, int maxx, int midy, int width, int caps, rgb_t color)
            {
                // loop over the width of the segment
                for (int y = 0; y < width / 2; y++)
                {
                    throw new emu_unimplemented();
#if false
                    UInt32 *d0 = &dest.pix32(midy - y);
                    UInt32 *d1 = &dest.pix32(midy + y);
                    int ty = (y < width / 8) ? width / 8 : y;

                    // loop over the length of the segment
                    for (int x = minx + ((caps & LINE_CAP_START) ? ty : 0); x < maxx - ((caps & LINE_CAP_END) ? ty : 0); x++)
                        d0[x] = d1[x] = color;
#endif
                }
            }

            //-------------------------------------------------
            //  draw_segment_horizontal - draw a horizontal
            //  LED segment
            //-------------------------------------------------
            void draw_segment_horizontal(bitmap_argb32 dest, int minx, int maxx, int midy, int width, rgb_t color)
            {
                draw_segment_horizontal_caps(dest, minx, maxx, midy, width, rendlay_global.LINE_CAP_START | rendlay_global.LINE_CAP_END, color);
            }

            //-------------------------------------------------
            //  draw_segment_vertical_caps - draw a
            //  vertical LED segment with definable end
            //  and start points
            //-------------------------------------------------
            void draw_segment_vertical_caps(bitmap_argb32 dest, int miny, int maxy, int midx, int width, int caps, rgb_t color)
            {
                // loop over the width of the segment
                for (int x = 0; x < width / 2; x++)
                {
                    throw new emu_unimplemented();
#if false
                    UInt32 *d0 = &dest.pix32(0, midx - x);
                    UInt32 *d1 = &dest.pix32(0, midx + x);
                    int tx = (x < width / 8) ? width / 8 : x;

                    // loop over the length of the segment
                    for (int y = miny + ((caps & LINE_CAP.LINE_CAP_START) ? tx : 0); y < maxy - ((caps & LINE_CAP.LINE_CAP_END) ? tx : 0); y++)
                        d0[y * dest.rowpixels()] = d1[y * dest.rowpixels()] = color;
#endif
                }
            }

            //-------------------------------------------------
            //  draw_segment_vertical - draw a vertical
            //  LED segment
            //-------------------------------------------------
            void draw_segment_vertical(bitmap_argb32 dest, int miny, int maxy, int midx, int width, rgb_t color)
            {
                draw_segment_vertical_caps(dest, miny, maxy, midx, width, rendlay_global.LINE_CAP_START | rendlay_global.LINE_CAP_END, color);
            }

            //-------------------------------------------------
            //  draw_segment_diagonal_1 - draw a diagonal
            //  LED segment that looks like a backslash
            //-------------------------------------------------
            void draw_segment_diagonal_1(bitmap_argb32 dest, int minx, int maxx, int miny, int maxy, int width, rgb_t color)
            {
                // compute parameters
                width = (int)(width * 1.5);
                float ratio = (maxy - miny - width) / (float)(maxx - minx);

                // draw line
                for (int x = minx; x < maxx; x++)
                {
                    if (x >= 0 && x < dest.width())
                    {
                        throw new emu_unimplemented();
#if false
                        UInt32 *d = &dest.pix32(0, x);
                        int step = (int)((x - minx) * ratio);

                        for (int y = maxy - width - step; y < maxy - step; y++)
                        {
                            if (y >= 0 && y < dest.height())
                                d[y * dest.rowpixels()] = color;
                        }
#endif
                    }
                }
            }

            //-------------------------------------------------
            //  draw_segment_diagonal_2 - draw a diagonal
            //  LED segment that looks like a forward slash
            //-------------------------------------------------
            void draw_segment_diagonal_2(bitmap_argb32 dest, int minx, int maxx, int miny, int maxy, int width, rgb_t color)
            {
                // compute parameters
                width = (int)(width * 1.5);
                float ratio = (maxy - miny - width) / (float)(maxx - minx);

                // draw line
                for (int x = minx; x < maxx; x++)
                {
                    if (x >= 0 && x < dest.width())
                    {
                        throw new emu_unimplemented();
#if false
                        UInt32 *d = &dest.pix32(0, x);
                        int step = (int)((x - minx) * ratio);

                        for (int y = miny + step; y < miny + step + width; y++)
                        {
                            if (y >= 0 && y < dest.height())
                                d[y * dest.rowpixels()] = color;
                        }
#endif
                    }
                }
            }

            //-------------------------------------------------
            //  draw_segment_decimal - draw a decimal point
            //-------------------------------------------------
            void draw_segment_decimal(bitmap_argb32 dest, int midx, int midy, int width, rgb_t color)
            {
                // compute parameters
                width /= 2;
                float ooradius2 = 1.0f / (float)(width * width);

                // iterate over y
                for (UInt32 y = 0; y <= width; y++)
                {
                    throw new emu_unimplemented();
#if false
                    UInt32 *d0 = &dest.pix32(midy - y);
                    UInt32 *d1 = &dest.pix32(midy + y);
                    float xval = width * Math.Sqrt(1.0f - (float)(y * y) * ooradius2);
                    int left;
                    int right;

                    // compute left/right coordinates
                    left = midx - (int)(xval + 0.5f);
                    right = midx + (int)(xval + 0.5f);

                    // draw this scanline
                    for (UInt32 x = left; x < right; x++)
                        d0[x] = d1[x] = color;
#endif
                }
            }

            //void draw_segment_comma(bitmap_argb32 &dest, int minx, int maxx, int miny, int maxy, int width, rgb_t color);

            //-------------------------------------------------
            //  apply_skew - apply skew to a bitmap
            //-------------------------------------------------
            void apply_skew(bitmap_argb32 dest, int skewwidth)
            {
                for (int y = 0; y < dest.height(); y++)
                {
                    throw new emu_unimplemented();
#if false
                    UInt32 *destrow = &dest.pix32(y);
                    int offs = skewwidth * (dest.height() - y) / dest.height();
                    for (int x = dest.width() - skewwidth - 1; x >= 0; x--)
                        destrow[x + offs] = destrow[x];
                    for (int x = 0; x < offs; x++)
                        destrow[x] = 0;
#endif
                }
            }
        }


        public partial class texture : global_object, IDisposable
        {
            //-------------------------------------------------
            //  texture - constructor
            //-------------------------------------------------
            public texture()
            {
                m_element = null;
                m_texture = null;
                m_state = 0;
            }

            //texture(texture const &that) = delete;
            //texture(texture &&that);

            ~texture()
            {
                assert(m_isDisposed);  // can remove
            }

            bool m_isDisposed = false;
            public void Dispose()
            {
                if (m_element != null)
                    m_element.machine().render().texture_free(m_texture);

                m_isDisposed = true;
            }
        }


        // image
        class image_component : component
        {
            // internal state
            //bitmap_argb32       m_bitmap;                   // source bitmap for images
            string m_dirname;                  // directory name of image file (for lazy loading)
            string m_imagefile;                // name of the image file (for lazy loading)
            string m_alphafile;                // name of the alpha file (for lazy loading)
            bool m_hasalpha = false;         // is there any alpha component present?


            // construction/destruction
            public image_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
                m_dirname = !string.IsNullOrEmpty(dirname) ? dirname : "";
                m_imagefile = env.get_attribute_string(compnode, "file", "");
                m_alphafile = env.get_attribute_string(compnode, "alphafile", "");
            }


            // overrides
            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }


            // internal helpers
            //void load_bitmap(running_machine &machine)
        }


        // rectangle
        class rect_component : component
        {
            // construction/destruction
            rect_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
            }


            // overrides
            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // ellipse
        class disk_component : component
        {
            // construction/destruction
            disk_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
            }


            // overrides
            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // text string
        class text_component : component
        {
            // internal state
            string m_string;                   // string for text components
            int m_textalign;                // text alignment to box


            // construction/destruction
            text_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
                m_string = env.get_attribute_string(compnode, "string", "");
                m_textalign = env.get_attribute_int(compnode, "align", 0);
            }


            // overrides
            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 7-segment LCD
        class led7seg_component : component
        {
            // construction/destruction
            led7seg_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
            }


            // overrides
            public override int maxstate() { return 255; }


            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 8-segment fluorescent (Gottlieb System 1)
        class led8seg_gts1_component : component
        {
            // construction/destruction
            led8seg_gts1_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
            }


            // overrides
            public override int maxstate() { return 255; }


            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 14-segment LCD
        class led14seg_component : component
        {
            // construction/destruction
            led14seg_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
            }


            // overrides
            public override int maxstate() { return 16383; }


            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 16-segment LCD
        class led16seg_component : component
        {
            // construction/destruction
            led16seg_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
            }


            // overrides
            public override int maxstate() { return 65535; }


            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 14-segment LCD with semicolon (2 extra segments)
        class led14segsc_component : component
        {
            // construction/destruction
            led14segsc_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
            }


            // overrides
            public override int maxstate() { return 65535; }


            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 16-segment LCD with semicolon (2 extra segments)
        class led16segsc_component : component
        {
            // construction/destruction
            led16segsc_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
            }


            // overrides
            public override int maxstate() { return 262143; }


            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // row of dots for a dotmatrix
        class dotmatrix_component : component
        {
            // internal state
            int m_dots;


            // construction/destruction
            public dotmatrix_component(int dots, layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
                m_dots = dots;
            }


            // overrides
            public override int maxstate() { return (1 << m_dots) - 1; }


            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // simple counter
        class simplecounter_component : component
        {
            // internal state
            int m_digits;                   // number of digits for simple counters
            int m_textalign;                // text alignment to box
            int m_maxstate;


            // construction/destruction
            simplecounter_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
                m_digits = env.get_attribute_int(compnode, "digits", 2);
                m_textalign = env.get_attribute_int(compnode, "align", 0);
                m_maxstate = env.get_attribute_int(compnode, "maxstate", 999);
            }


            // overrides
            public override int maxstate() { return m_maxstate; }


            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // fruit machine reel
        class reel_component : component
        {
            const int MAX_BITMAPS = 32;

            // internal state
            //bitmap_argb32       m_bitmap[MAX_BITMAPS];      // source bitmap for images
            string m_dirname;                  // directory name of image file (for lazy loading)
            emu_file [] m_file = new emu_file [MAX_BITMAPS];        // file object for reading image/alpha files
            string [] m_imagefile = new string [MAX_BITMAPS];   // name of the image file (for lazy loading)
            //std::string         m_alphafile[MAX_BITMAPS];   // name of the alpha file (for lazy loading)
            bool [] m_hasalpha = new bool [MAX_BITMAPS];    // is there any alpha component present?

            // basically made up of multiple text strings / gfx
            int m_numstops;
            string [] m_stopnames = new string [MAX_BITMAPS];
            int m_stateoffset;
            int m_reelreversed;
            int m_numsymbolsvisible;
            int m_beltreel;


            // construction/destruction
            reel_component(layout_element_environment env, util.xml.data_node compnode, string dirname)
                : base(env, compnode, dirname)
            {
                for (int i = 0; i < m_hasalpha.Length; i++)
                    m_hasalpha[i] = false;

                string symbollist = env.get_attribute_string(compnode, "symbollist", "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15");

                // split out position names from string and figure out our number of symbols
                int location;
                m_numstops = 0;
                location = symbollist.find(',');
                while (location != -1)
                {
                    m_stopnames[m_numstops] = symbollist;
                    m_stopnames[m_numstops] = m_stopnames[m_numstops].substr(0, location);
                    symbollist = symbollist.substr(location+1, symbollist.length()-(location-1));
                    m_numstops++;
                    location = symbollist.find(',');
                }
                m_stopnames[m_numstops++] = symbollist;

                // careful, dirname is nullptr if we're coming from internal layout, and our string assignment doesn't like that
                if (dirname != null)
                    m_dirname = dirname;

                for (int i = 0; i < m_numstops; i++)
                {
                    location = m_stopnames[i].find(":");
                    if (location != -1)
                    {
                        m_imagefile[i] = m_stopnames[i];
                        m_stopnames[i] = m_stopnames[i].substr(0, location);
                        m_imagefile[i] = m_imagefile[i].substr(location+1, m_imagefile[i].length()-(location-1));

                        //m_alphafile[i] =
                        m_file[i] = new emu_file(env.machine().options().art_path(), OPEN_FLAG_READ);
                    }
                    else
                    {
                        //m_imagefile[i] = 0;
                        //m_alphafile[i] = 0;
                        m_file[i] = null;  //m_file[i].reset();
                    }
                }

                m_stateoffset = env.get_attribute_int(compnode, "stateoffset", 0);
                m_numsymbolsvisible = env.get_attribute_int(compnode, "numsymbolsvisible", 3);
                m_reelreversed = env.get_attribute_int(compnode, "reelreversed", 0);
                m_beltreel = env.get_attribute_int(compnode, "beltreel", 0);
            }


            // overrides
            public override int maxstate() { return 65535; }


            public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }


            // internal helpers
            //void draw_beltreel(running_machine &machine, bitmap_argb32 &dest, const rectangle &bounds, int state);
            //void load_reel_bitmap(int number);
        }


        layout_element_make_component_map s_make_component = new layout_element_make_component_map()
        {
            { "image",         make_component<image_component>         },
            { "text",          make_component<text_component>          },
            { "dotmatrix",     make_dotmatrix_component_8              },
            { "dotmatrix5dot", make_dotmatrix_component_5              },
            { "dotmatrixdot",  make_dotmatrix_component_1              },
            { "simplecounter", make_component<simplecounter_component> },
            { "reel",          make_component<reel_component>          },
            { "led7seg",       make_component<led7seg_component>       },
            { "led8seg_gts1",  make_component<led8seg_gts1_component>  },
            { "led14seg",      make_component<led14seg_component>      },
            { "led14segsc",    make_component<led14segsc_component>    },
            { "led16seg",      make_component<led16seg_component>      },
            { "led16segsc",    make_component<led16segsc_component>    },
            { "rect",          make_component<rect_component>          },
            { "disk",          make_component<disk_component>          }
        };


        // construction/destruction
        //-------------------------------------------------
        //  layout_element - constructor
        //-------------------------------------------------
        public layout_element(layout_element_environment env, util.xml.data_node elemnode, string dirname)
        {
            m_machine = env.machine();
            m_defstate = env.get_attribute_int(elemnode, "defstate", -1);
            m_statemask = 0;
            m_foldhigh = false;


            // parse components in order
            bool first = true;
            render_bounds bounds = new render_bounds() { x0 = 0.0f, y0 = 0.0f, x1 = 0.0f, y1 = 0.0f };
            for (util.xml.data_node compnode = elemnode.get_first_child(); compnode != null; compnode = compnode.get_next_sibling())
            {
                var make_func = s_make_component.find(compnode.get_name());
                if (make_func == null)
                    throw new layout_syntax_error(string_format("unknown element component {0}", compnode.get_name()));

                // insert the new component into the list
                //m_complist.emplace(m_complist.end(), make_func->second(env, compnode, dirname));
                component newcomp = make_func(env, compnode, dirname);
                m_complist.push_back(newcomp);

                // accumulate bounds
                if (first)
                    bounds = newcomp.overall_bounds();
                else
                    rendutil_global.union_render_bounds(bounds, newcomp.overall_bounds());

                first = false;

                // determine the maximum state
                std.pair<int, bool> wrap = newcomp.statewrap();
                m_statemask |= wrap.first;
                m_foldhigh = m_foldhigh || wrap.second;
            }

            if (!m_complist.empty())
            {
                // determine the scale/offset for normalization
                float xoffs = bounds.x0;
                float yoffs = bounds.y0;
                float xscale = 1.0f / (bounds.x1 - bounds.x0);
                float yscale = 1.0f / (bounds.y1 - bounds.y0);

                // normalize all the component bounds
                foreach (var curcomp in m_complist)
                    curcomp.normalize_bounds(xoffs, yoffs, xscale, yscale);
            }

            // allocate an array of element textures for the states
            m_elemtex.resize((m_statemask + 1) << (m_foldhigh ? 1 : 0));
        }


        //-------------------------------------------------
        //  state_texture - return a pointer to a
        //  render_texture for the given state, allocating
        //  one if needed
        //-------------------------------------------------
        public render_texture state_texture(int state)
        {
            if (m_foldhigh && (state & ~m_statemask) != 0)
                state = (state & m_statemask) | (((m_statemask << 1) | 1) & ~m_statemask);
            else
                state &= m_statemask;

            assert(m_elemtex.size() > state);

            if (m_elemtex[state].m_texture == null)
            {
                m_elemtex[state].m_element = this;
                m_elemtex[state].m_state = state;
                m_elemtex[state].m_texture = machine().render().texture_alloc(element_scale, m_elemtex[state]);
            }

            return m_elemtex[state].m_texture;
        }


        // internal helpers
        //-------------------------------------------------
        //  element_scale - scale an element by rendering
        //  all the components at the appropriate
        //  resolution
        //-------------------------------------------------
        void element_scale(bitmap_argb32 dest, bitmap_argb32 source, rectangle sbounds, layout_element.texture param) //, void *param)
        {
            texture elemtex = (texture)param;

            // iterate over components that are part of the current state
            foreach (var curcomp in elemtex.m_element.m_complist)
            {
                if ((elemtex.m_state & curcomp.statemask()) == curcomp.stateval())
                {
                    // get the local scaled bounds
                    render_bounds compbounds = curcomp.bounds(elemtex.m_state);
                    rectangle bounds = new rectangle(
                            (int)rendutil_global.render_round_nearest(compbounds.x0 * dest.width()),
                            (int)rendutil_global.render_round_nearest(compbounds.x1 * dest.width()),
                            (int)rendutil_global.render_round_nearest(compbounds.y0 * dest.height()),
                            (int)rendutil_global.render_round_nearest(compbounds.y1 * dest.height()));
                    bounds.intersection(dest.cliprect());  //bounds &= dest.cliprect();

                    // based on the component type, add to the texture
                    curcomp.draw(elemtex.m_element.machine(), dest, bounds, elemtex.m_state);
                }
            }
        }


        //-------------------------------------------------
        //  make_component - create component of given type
        //-------------------------------------------------
        //template <typename T>
        static component make_component<T>(layout_element_environment env, util.xml.data_node compnode, string dirname) where T : component
        {
            // return std::make_unique<T>(env, compnode, dirname);
            if (typeof(T) is image_component)
                return new image_component(env, compnode, dirname);
            else
                throw new emu_unimplemented();
        }


        //template <int D>
        static component make_dotmatrix_component(int D, layout_element_environment env, util.xml.data_node compnode, string dirname)
        {
            return new dotmatrix_component(D, env, compnode, dirname);  //return std::make_unique<dotmatrix_component>(D, env, compnode, dirname);
        }

        static component make_dotmatrix_component_1(layout_element_environment env, util.xml.data_node compnode, string dirname) { return make_dotmatrix_component(1, env, compnode, dirname); }
        static component make_dotmatrix_component_5(layout_element_environment env, util.xml.data_node compnode, string dirname) { return make_dotmatrix_component(5, env, compnode, dirname); }
        static component make_dotmatrix_component_8(layout_element_environment env, util.xml.data_node compnode, string dirname) { return make_dotmatrix_component(8, env, compnode, dirname); }
    }


    public partial class layout_group : global_object
    {
        //-------------------------------------------------
        //  layout_group - constructor
        //-------------------------------------------------
        public layout_group(util.xml.data_node groupnode)
        {
            m_groupnode = groupnode;
            m_bounds = new render_bounds(0.0f, 0.0f, 0.0f, 0.0f);
            m_bounds_resolved = false;
        }


        //-------------------------------------------------
        //  make_transform - create abbreviated transform
        //  matrix for given destination bounds
        //-------------------------------------------------
        public float [,] make_transform(int orientation, render_bounds dest)
        {
            assert(m_bounds_resolved);

            // make orientation matrix
            float [,] result = new float [,] { { 1.0F, 0.0F, 0.0F }, { 0.0F, 1.0F, 0.0F }, { 0.0F, 0.0F, 1.0F } };  //transform result{{ {{ 1.0F, 0.0F, 0.0F }}, {{ 0.0F, 1.0F, 0.0F }}, {{ 0.0F, 0.0F, 1.0F }} }};
            if ((orientation & ORIENTATION_SWAP_XY) != 0)
            {
                std.swap(ref result[0, 0], ref result[0, 1]);
                std.swap(ref result[1, 0], ref result[1, 1]);
            }
            if ((orientation & ORIENTATION_FLIP_X) != 0)
            {
                result[0, 0] = -result[0, 0];
                result[0, 1] = -result[0, 1];
            }
            if ((orientation & ORIENTATION_FLIP_Y) != 0)
            {
                result[1, 0] = -result[1, 0];
                result[1, 1] = -result[1, 1];
            }

            // apply to bounds and force into destination rectangle
            render_bounds bounds = m_bounds;
            rendlay_global.render_bounds_transform(ref bounds, result);
            result[0, 0] *= (dest.x1 - dest.x0) / std.fabs(bounds.x1 - bounds.x0);
            result[0, 1] *= (dest.x1 - dest.x0) / std.fabs(bounds.x1 - bounds.x0);
            result[0, 2] = dest.x0 - (std.min(bounds.x0, bounds.x1) * (dest.x1 - dest.x0) / std.fabs(bounds.x1 - bounds.x0));
            result[1, 0] *= (dest.y1 - dest.y0) / std.fabs(bounds.y1 - bounds.y0);
            result[1, 1] *= (dest.y1 - dest.y0) / std.fabs(bounds.y1 - bounds.y0);
            result[1, 2] = dest.y0 - (std.min(bounds.y0, bounds.y1) * (dest.y1 - dest.y0) / std.fabs(bounds.y1 - bounds.y0));
            return result;
        }


        public float [,] make_transform(int orientation, float [,] trans)  //layout_group::transform layout_group::make_transform(int orientation, transform const &trans) const
        {
            assert(m_bounds_resolved);

            render_bounds dest = new render_bounds()
            {
                x0 = m_bounds.x0,
                y0 = m_bounds.y0,
                x1 = (orientation & ORIENTATION_SWAP_XY) != 0 ? (m_bounds.x0 + m_bounds.y1 - m_bounds.y0) : m_bounds.x1,
                y1 = (orientation & ORIENTATION_SWAP_XY) != 0 ? (m_bounds.y0 + m_bounds.x1 - m_bounds.x0) : m_bounds.y1
            };
            return make_transform(orientation, dest, trans);
        }


        public float [,] make_transform(int orientation, render_bounds dest, float [,] trans)  //layout_group::transform layout_group::make_transform(int orientation, render_bounds const &dest, transform const &trans) const
        {
            float [,] next = make_transform(orientation, dest);
            float [,] result = new float [,] { { 0.0F, 0.0F, 0.0F }, { 0.0F, 0.0F, 0.0F }, { 0.0F, 0.0F, 0.0F } };  //transform result{{ {{ 0.0F, 0.0F, 0.0F }}, {{ 0.0F, 0.0F, 0.0F }}, {{ 0.0F, 0.0F, 0.0F }} }};
            for (UInt32 y = 0; 3U > y; ++y)
            {
                for (UInt32 x = 0; 3U > x; ++x)
                {
                    for (UInt32 i = 0; 3U > i; ++i)
                        result[y, x] += trans[y, i] * next[i, x];
                }
            }

            return result;
        }


        public void set_bounds_unresolved()
        {
            m_bounds_resolved = false;
        }


        public void resolve_bounds(layout_group_environment env, layout_group_group_map groupmap)
        {
            if (!m_bounds_resolved)
            {
                std.vector<layout_group> seen = new std.vector<layout_group>();
                resolve_bounds(env, groupmap, seen);
            }
        }


        void resolve_bounds(layout_group_environment env, layout_group_group_map groupmap, std.vector<layout_group> seen)
        {
            if (seen.Contains(this))  //if (seen.end() != std::find(seen.begin(), seen.end(), this))
            {
                // a wild loop appears!
                string path = "";  //std::ostringstream path;
                foreach (layout_group group in seen)
                    path += string.Format(" {0}", group.m_groupnode.get_attribute_string("name", null));  //path << ' ' << group->m_groupnode.get_attribute_string("name", nullptr);
                path += string.Format(" {0}", m_groupnode.get_attribute_string("name", null));  //path << ' ' << m_groupnode.get_attribute_string("name", nullptr);
                throw new layout_syntax_error(string_format("recursively nested groups {0}", path.str()));
            }

            seen.push_back(this);
            if (!m_bounds_resolved)
            {
                rendutil_global.set_render_bounds_xy(m_bounds, 0.0F, 0.0F, 1.0F, 1.0F);
                layout_group_environment local = new layout_group_environment(env);
                bool empty = true;
                resolve_bounds(local, m_groupnode, groupmap, seen, ref empty, false, false, true);
            }
            seen.pop_back();
        }


        void resolve_bounds(
                layout_group_environment env,
                util.xml.data_node parentnode,
                layout_group_group_map groupmap,
                std.vector<layout_group> seen,
                ref bool empty,
                bool vistoggle,
                bool repeat,
                bool init)
        {
            rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Group '{0}' resolve bounds empty={1} vistoggle={2} repeat={3} init={4}\n",
                parentnode.get_attribute_string("name", ""), empty, vistoggle, repeat, init);

            bool envaltered = false;
            bool unresolved = true;
            for (util.xml.data_node itemnode = parentnode.get_first_child(); !m_bounds_resolved && itemnode != null; itemnode = itemnode.get_next_sibling())
            {
                if (strcmp(itemnode.get_name(), "bounds") == 0)
                {
                    // use explicit bounds
                    env.parse_bounds(itemnode, out m_bounds);
                    m_bounds_resolved = true;
                }
                else if (strcmp(itemnode.get_name(), "param") == 0)
                {
                    envaltered = true;
                    if (!unresolved)
                    {
                        rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Environment altered{0}, unresolving groups\n", envaltered ? " again" : "");
                        unresolved = true;
                        foreach (var group in groupmap)
                            group.second().set_bounds_unresolved();
                    }

                    if (!repeat)
                        env.set_parameter(itemnode);
                    else
                        env.set_repeat_parameter(itemnode, init);
                }
                else if (strcmp(itemnode.get_name(), "element") == 0 ||
                    strcmp(itemnode.get_name(), "backdrop") == 0 ||
                    strcmp(itemnode.get_name(), "screen") == 0 ||
                    strcmp(itemnode.get_name(), "overlay") == 0 ||
                    strcmp(itemnode.get_name(), "bezel") == 0 ||
                    strcmp(itemnode.get_name(), "cpanel") == 0 ||
                    strcmp(itemnode.get_name(), "marquee") == 0)
                {
                    render_bounds itembounds;
                    env.parse_bounds(itemnode.get_child("bounds"), out itembounds);
                    if (empty)
                        m_bounds = itembounds;
                    else
                        rendutil_global.union_render_bounds(m_bounds, itembounds);

                    empty = false;

                    rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Accumulate item bounds ({0} {1} {2} {3}) -> ({4} {5} {6} {7})\n",
                        itembounds.x0, itembounds.y0, itembounds.x1, itembounds.y1,
                        m_bounds.x0, m_bounds.y0, m_bounds.x1, m_bounds.y1);
                }
                else if (strcmp(itemnode.get_name(), "group") == 0)
                {
                    util.xml.data_node itemboundsnode = itemnode.get_child("bounds");
                    if (itemboundsnode != null)
                    {
                        render_bounds itembounds;
                        env.parse_bounds(itemboundsnode, out itembounds);
                        if (empty)
                            m_bounds = itembounds;
                        else
                            rendutil_global.union_render_bounds(m_bounds, itembounds);

                        empty = false;

                        rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Accumulate group '{0}' reference explicit bounds ({1} {2} {3} {4}) -> ({5} {6} {7} {8})\n",
                            itemnode.get_attribute_string("ref", ""),
                            itembounds.x0, itembounds.y0, itembounds.x1, itembounds.y1,
                            m_bounds.x0, m_bounds.y0, m_bounds.x1, m_bounds.y1);
                    }
                    else
                    {
                        string ref_ = env.get_attribute_string(itemnode, "ref", null);
                        if (ref_ == null)
                            throw new layout_syntax_error("nested group must have ref attribute");

                        var found = groupmap.find(ref_);
                        if (found == null)
                            throw new layout_syntax_error(string_format("unable to find group {0}", ref_));

                        int orientation = env.parse_orientation(itemnode.get_child("orientation"));
                        layout_group_environment local = new layout_group_environment(env);
                        found.resolve_bounds(local, groupmap, seen);
                        render_bounds itembounds = new render_bounds()
                        {
                            x0 = found.m_bounds.x0,
                            y0 = found.m_bounds.y0,
                            x1 = (orientation & ORIENTATION_SWAP_XY) != 0 ? (found.m_bounds.x0 + found.m_bounds.y1 - found.m_bounds.y0) : found.m_bounds.x1,
                            y1 = (orientation & ORIENTATION_SWAP_XY) != 0 ? (found.m_bounds.y0 + found.m_bounds.x1 - found.m_bounds.x0) : found.m_bounds.y1
                        };

                        if (empty)
                            m_bounds = itembounds;
                        else
                            rendutil_global.union_render_bounds(m_bounds, itembounds);

                        empty = false;

                        unresolved = false;
                        rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Accumulate group '{0}' reference computed bounds ({1} {2} {3} {4}) -> ({5} {6} {7} {8})\n",
                            itemnode.get_attribute_string("ref", ""),
                            itembounds.x0, itembounds.y0, itembounds.x1, itembounds.y1,
                            m_bounds.x0, m_bounds.y0, m_bounds.x1, m_bounds.y1);
                    }
                }
                else if (strcmp(itemnode.get_name(), "repeat") == 0)
                {
                    int count = env.get_attribute_int(itemnode, "count", -1);
                    if (0 >= count)
                        throw new layout_syntax_error("repeat must have positive integer count attribute");

                    layout_group_environment local = new layout_group_environment(env);
                    for (int i = 0; !m_bounds_resolved && (count > i); ++i)
                    {
                        resolve_bounds(local, itemnode, groupmap, seen, ref empty, false, true, i == 0);
                        local.increment_parameters();
                    }
                }
                else if (strcmp(itemnode.get_name(), "collection") == 0)
                {
                    if (string.IsNullOrEmpty(env.get_attribute_string(itemnode, "name", null)))
                        throw new layout_syntax_error("collection must have name attribute");
                    layout_group_environment local = env;
                    resolve_bounds(local, itemnode, groupmap, seen, ref empty, true, false, true);
                }
                else
                {
                    throw new layout_syntax_error(string_format("unknown group element {0}", itemnode.get_name()));
                }
            }

            if (envaltered && !unresolved)
            {
                rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Environment was altered, marking groups unresolved\n");
                bool resolved = m_bounds_resolved;
                foreach (var group in groupmap)
                    group.second().set_bounds_unresolved();
                m_bounds_resolved = resolved;
            }

            if (!vistoggle && !repeat)
            {
                rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Marking group '{0}' bounds resolved\n", parentnode.get_attribute_string("name", ""));
                m_bounds_resolved = true;
            }
        }
    }


    public partial class layout_view : global_object
    {
        public partial class item : global_object
        {
            // construction/destruction
            //-------------------------------------------------
            //  item - constructor
            //-------------------------------------------------
            public item(
                    layout_view_view_environment env,
                    util.xml.data_node itemnode,
                    layout_view_element_map elemmap,
                    int orientation,
                    float [,] trans,  //layout_group::transform const &trans,
                    render_color color)
            {
                m_element = find_element(env, itemnode, elemmap);

                //throw new emu_unimplemented();
#if false
                m_output(env.device(), env.get_attribute_string(itemnode, "name", ""))
#endif

                m_have_output = env.get_attribute_string(itemnode, "name", "").Length != 0;
                m_input_port = null;
                m_input_field = null;
                m_input_mask = (ioport_value)env.get_attribute_int(itemnode, "inputmask", 0);
                m_input_shift = (u8)get_input_shift(m_input_mask);
                m_input_raw = env.get_attribute_bool(itemnode, "inputraw", false);  //, m_input_raw(env.get_attribute_bool(itemnode, "inputraw", 0))
                m_clickthrough = env.get_attribute_bool(itemnode, "clickthrough", true);  //, m_clickthrough(env.get_attribute_bool(itemnode, "clickthrough", "yes"))
                m_screen = null;
                m_orientation = rendutil_global.orientation_add(env.parse_orientation(itemnode.get_child("orientation")), orientation);
                m_color = rendlay_global.render_color_multiply(env.parse_color(itemnode.get_child("color")), color);
                m_blend_mode = get_blend_mode(env, itemnode);
                m_visibility_mask = env.visibility_mask();
                m_input_tag = make_input_tag(env, itemnode);
                m_rawbounds = make_bounds(env, itemnode, trans);
                m_has_clickthrough = !string.IsNullOrEmpty(env.get_attribute_string(itemnode, "clickthrough", ""));  //, m_has_clickthrough(env.get_attribute_string(itemnode, "clickthrough", "")[0])


                // fetch common data
                int index = env.get_attribute_int(itemnode, "index", -1);
                if (index != -1)
                    m_screen = new screen_device_iterator(env.machine().root_device()).byindex(index);

                // sanity checks
                if (strcmp(itemnode.get_name(), "screen") == 0)
                {
                    if (itemnode.has_attribute("tag"))
                    {
                        string tag = env.get_attribute_string(itemnode, "tag", "");
                        m_screen = (screen_device)env.device().subdevice(tag);
                        if (m_screen == null)
                            throw new layout_reference_error(string_format("invalid screen tag '{0}'", tag));
                    }
                    else if (m_screen == null)
                    {
                        throw new layout_reference_error(string_format("invalid screen index {0}", index));
                    }
                }
                else if (m_element == null)
                {
                    throw new layout_syntax_error(string_format("item of type {0} require an element tag", itemnode.get_name()));
                }
            }


            // fetch state based on configured source
            //-------------------------------------------------
            //  state - fetch state based on configured source
            //-------------------------------------------------
            public int state()
            {
                assert(m_element != null);

                if (m_have_output)
                {
                    throw new emu_unimplemented();
#if false
                    // if configured to track an output, fetch its value
                    return m_output;
#endif
                }
                else if (m_input_port != null)
                {
                    // if configured to an input, fetch the input value
                    if (m_input_raw)
                    {
                        return ((int)(m_input_port.read() & m_input_mask)) >> m_input_shift;
                    }
                    else
                    {
                        ioport_field field = m_input_field != null ? m_input_field : m_input_port.field(m_input_mask);
                        if (field != null)
                            return ((m_input_port.read() ^ field.defvalue()) & m_input_mask) != 0 ? 1 : 0;
                    }
                }

                // default to zero
                return 0;
            }


            // resolve tags, if any
            //---------------------------------------------
            //  resolve_tags - resolve tags, if any are set
            //---------------------------------------------
            public void resolve_tags()
            {
                if (m_have_output)
                {
                    throw new emu_unimplemented();
#if false
                    m_output.resolve();
                    if (m_element)
                        m_output = m_element->default_state();
#endif
                }

                if (!m_input_tag.empty())
                {
                    m_input_port = m_element.machine().root_device().ioport(m_input_tag);
                    if (m_input_port != null)
                    {
                        // if there's a matching unconditional field, cache it
                        foreach (ioport_field field in m_input_port.fields())
                        {
                            if ((field.mask() & m_input_mask) != 0)
                            {
                                if (field.condition().condition() == ioport_condition.condition_t.ALWAYS)
                                    m_input_field = field;

                                break;
                            }
                        }

                        // if clickthrough isn't explicitly configured, having an I/O port implies false
                        if (!m_has_clickthrough)
                            m_clickthrough = false;
                    }
                }
            }


            //---------------------------------------------
            //  find_element - find element definition
            //---------------------------------------------
            layout_element find_element(layout_view_view_environment env, util.xml.data_node itemnode, layout_view_element_map elemmap)
            {
                string name = env.get_attribute_string(itemnode, strcmp(itemnode.get_name(), "element") == 0 ? "ref" : "element", null);
                if (string.IsNullOrEmpty(name))
                    return null;

                // search the list of elements for a match, error if not found
                var found = elemmap.find(name);
                if (null != found)
                    return found;
                else
                    throw new layout_syntax_error("unable to find element {0}", name);
            }


            //---------------------------------------------
            //  make_bounds - get transformed bounds
            //---------------------------------------------
            render_bounds make_bounds(
                    layout_view_view_environment env,
                    util.xml.data_node itemnode,
                    float [,] trans)  //layout_group.transform trans)
            {
                render_bounds bounds;
                env.parse_bounds(itemnode.get_child("bounds"), out bounds);
                rendlay_global.render_bounds_transform(ref bounds, trans);
                if (bounds.x0 > bounds.x1)
                    std.swap(ref bounds.x0, ref bounds.x1);
                if (bounds.y0 > bounds.y1)
                    std.swap(ref bounds.y0, ref bounds.y1);
                return bounds;
            }


            //---------------------------------------------
            //  make_input_tag - get absolute input tag
            //---------------------------------------------
            string make_input_tag(layout_view_view_environment env, util.xml.data_node itemnode)
            {
                string tag = env.get_attribute_string(itemnode, "inputtag", null);
                return !string.IsNullOrEmpty(tag) ? env.device().subtag(tag) : "";
            }


            //---------------------------------------------
            //  get_blend_mode - explicit or implicit blend
            //---------------------------------------------
            int get_blend_mode(layout_view_view_environment env, util.xml.data_node itemnode)
            {
                // see if there's a blend mode attribute
                string mode = env.get_attribute_string(itemnode, "blend", null);
                if (!string.IsNullOrEmpty(mode))
                {
                    if (strcmp(mode, "none") == 0)
                        return BLENDMODE_NONE;
                    else if (strcmp(mode, "alpha") == 0)
                        return BLENDMODE_ALPHA;
                    else if (strcmp(mode, "multiply") == 0)
                        return BLENDMODE_RGB_MULTIPLY;
                    else if (strcmp(mode, "add") == 0)
                        return BLENDMODE_ADD;
                    else
                        throw new layout_syntax_error("unknown blend mode {0}", mode);
                }

                // fall back to implicit blend mode based on element type
                if (strcmp(itemnode.get_name(), "screen") == 0)
                    return -1; // magic number recognised by render.cpp to allow per-element blend mode
                else if (strcmp(itemnode.get_name(), "overlay") == 0)
                    return BLENDMODE_RGB_MULTIPLY;
                else
                    return BLENDMODE_ALPHA;
            }


            //---------------------------------------------
            //  get_input_shift - shift to right-align LSB
            //---------------------------------------------
            unsigned get_input_shift(ioport_value mask)
            {
                unsigned result = 0;
                while (mask != 0 && BIT(mask, 0) == 0)
                {
                    ++result;
                    mask >>= 1;
                }

                return result;
            }
        }


        class layer_lists
        {
            public layout_view_item_list backdrops = new layout_view_item_list();
            public layout_view_item_list screens = new layout_view_item_list();
            public layout_view_item_list overlays = new layout_view_item_list();
            public layout_view_item_list bezels = new layout_view_item_list();
            public layout_view_item_list cpanels = new layout_view_item_list();
            public layout_view_item_list marquees = new layout_view_item_list();
        }


        // construction/destruction
        //-------------------------------------------------
        //  layout_view - constructor
        //-------------------------------------------------
        public layout_view(
                emu.render.detail.layout_environment env,
                util.xml.data_node viewnode,
                layout_view_element_map elemmap,
                layout_view_group_map groupmap)
        {
            m_name = make_name(env, viewnode);
            m_effaspect = 1.0f;
            m_items = new layout_view_item_list();
            m_defvismask = 0;
            m_has_art = false;


            // parse the layout
            m_expbounds.x0 = m_expbounds.y0 = m_expbounds.x1 = m_expbounds.y1 = 0;
            layout_view_view_environment local = new layout_view_view_environment(env, m_name.c_str());
            layer_lists layers = new layer_lists();
            local.set_parameter("viewname", m_name);
            add_items(layers, local, viewnode, elemmap, groupmap, (int)ROT0, rendlay_global.identity_transform, new render_color(1.0F, 1.0F, 1.0F, 1.0F), true, false, true);

            // can't support legacy layers and modern visibility toggles at the same time
            if (!m_vistoggles.empty() && (!layers.backdrops.empty() || !layers.overlays.empty() || !layers.bezels.empty() || !layers.cpanels.empty() || !layers.marquees.empty()))
                throw new layout_syntax_error("view contains visibility toggles as well as legacy backdrop, overlay, bezel, cpanel and/or marquee elements");

            // create visibility toggles for legacy layers
            u32 mask = 1;
            if (!layers.backdrops.empty())
            {
                m_vistoggles.emplace_back(new visibility_toggle("Backdrops", mask));
                foreach (item backdrop in layers.backdrops)
                    backdrop.m_visibility_mask = mask;
                m_defvismask |= mask;
                mask <<= 1;
            }

            if (!layers.overlays.empty())
            {
                m_vistoggles.emplace_back(new visibility_toggle("Overlays", mask));
                foreach (item overlay in layers.overlays)
                    overlay.m_visibility_mask = mask;
                m_defvismask |= mask;
                mask <<= 1;
            }

            if (!layers.bezels.empty())
            {
                m_vistoggles.emplace_back(new visibility_toggle("Bezels", mask));
                foreach (item bezel in layers.bezels)
                    bezel.m_visibility_mask = mask;
                m_defvismask |= mask;
                mask <<= 1;
            }

            if (!layers.cpanels.empty())
            {
                m_vistoggles.emplace_back(new visibility_toggle("Control Panels", mask));
                foreach (item cpanel in layers.cpanels)
                    cpanel.m_visibility_mask = mask;
                m_defvismask |= mask;
                mask <<= 1;
            }

            if (!layers.marquees.empty())
            {
                m_vistoggles.emplace_back(new visibility_toggle("Backdrops", mask));
                foreach (item marquee in layers.marquees)
                    marquee.m_visibility_mask = mask;
                m_defvismask |= mask;
                mask <<= 1;
            }

            // deal with legacy element groupings
            if (!layers.overlays.empty() || (layers.backdrops.size() <= 1))
            {
                // screens (-1) + overlays (RGB multiply) + backdrop (add) + bezels (alpha) + cpanels (alpha) + marquees (alpha)
                foreach (item backdrop in layers.backdrops)
                    backdrop.m_blend_mode = BLENDMODE_ADD;

                foreach (var item in layers.screens) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.screens);
                foreach (var item in layers.overlays) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.overlays);
                foreach (var item in layers.backdrops) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.backdrops);
                foreach (var item in layers.bezels) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.bezels);
                foreach (var item in layers.cpanels) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.cpanels);
                foreach (var item in layers.marquees) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.marquees);
            }
            else
            {
                // multiple backdrop pieces and no overlays (Golly! Ghost! mode):
                // backdrop (alpha) + screens (add) + bezels (alpha) + cpanels (alpha) + marquees (alpha)
                foreach (item screen in layers.screens)
                {
                    if (screen.blend_mode() == -1)
                        screen.m_blend_mode = BLENDMODE_ADD;
                }

                foreach (var item in layers.backdrops) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.backdrops);
                foreach (var item in layers.screens) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.screens);
                foreach (var item in layers.bezels) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.bezels);
                foreach (var item in layers.cpanels) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.cpanels);
                foreach (var item in layers.marquees) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.marquees);
            }

            // calculate metrics
            recompute(default_visibility_mask(), false);
            foreach (var group in groupmap)  //for (group_map::value_type &group : groupmap)
                group.second().set_bounds_unresolved();
        }


        //-------------------------------------------------
        //  has_screen - return true if this view contains
        //  the given screen
        //-------------------------------------------------
        public bool has_screen(screen_device screen)
        {
            return std.find_if(m_screens, (scr) => { return scr.get() == screen; }) != null;  //return std::find_if(m_screens.begin(), m_screens.end(), [&screen](auto const &scr) { return &scr.get() == &screen; }) != m_screens.end();
        }


        // operations
        //-------------------------------------------------
        //  recompute - recompute the bounds and aspect
        //  ratio of a view and all of its contained items
        //-------------------------------------------------
        public void recompute(u32 visibility_mask, bool zoom_to_screen)
        {
            // reset the bounds and collected active items
            render_bounds scrbounds = new render_bounds(0.0f, 0.0f, 0.0f, 0.0f);
            m_bounds = scrbounds;
            m_screen_items.clear();
            m_interactive_items.clear();
            m_interactive_edges_x.clear();
            m_interactive_edges_y.clear();
            m_screens.clear();

            // loop over items and filter by visibility mask
            bool first = true;
            bool scrfirst = true;
            foreach (item curitem in m_items)
            {
                if ((visibility_mask & curitem.visibility_mask()) == curitem.visibility_mask())
                {
                    // accumulate bounds
                    if (first)
                        m_bounds = curitem.m_rawbounds;
                    else
                        rendutil_global.union_render_bounds(m_bounds, curitem.m_rawbounds);

                    first = false;

                    // accumulate visible screens and their bounds bounds
                    if (curitem.screen() != null)
                    {
                        if (scrfirst)
                            scrbounds = curitem.m_rawbounds;
                        else
                            rendutil_global.union_render_bounds(scrbounds, curitem.m_rawbounds);
                        scrfirst = false;

                        // accumulate active screens
                        m_screen_items.emplace_back(curitem);
                        m_screens.emplace_back(curitem.screen());
                    }

                    // accumulate interactive elements
                    if (!curitem.clickthrough() || curitem.has_input())
                        m_interactive_items.emplace_back(curitem);
                }
            }

            // if we have an explicit bounds, override it
            if (m_expbounds.x1 > m_expbounds.x0)
                m_bounds = m_expbounds;

            render_bounds target_bounds = new render_bounds();
            if (!zoom_to_screen || scrfirst)
            {
                // if we're handling things normally, the target bounds are (0,0)-(1,1)
                m_effaspect = ((m_bounds.x1 > m_bounds.x0) && (m_bounds.y1 > m_bounds.y0)) ? m_bounds.aspect() : 1.0f;
                target_bounds.x0 = target_bounds.y0 = 0.0f;
                target_bounds.x1 = target_bounds.y1 = 1.0f;
            }
            else
            {
                // if we're cropping, we want the screen area to fill (0,0)-(1,1)
                m_effaspect = ((scrbounds.x1 > scrbounds.x0) && (scrbounds.y1 > scrbounds.y0)) ? scrbounds.aspect() : 1.0f;
                target_bounds.x0 = (m_bounds.x0 - scrbounds.x0) / scrbounds.width();
                target_bounds.y0 = (m_bounds.y0 - scrbounds.y0) / scrbounds.height();
                target_bounds.x1 = target_bounds.x0 + (m_bounds.width() / scrbounds.width());
                target_bounds.y1 = target_bounds.y0 + (m_bounds.height() / scrbounds.height());
            }

            // determine the scale/offset for normalization
            float xoffs = m_bounds.x0;
            float yoffs = m_bounds.y0;
            float xscale = target_bounds.width() / m_bounds.width();
            float yscale = target_bounds.height() / m_bounds.height();

            // normalize all the item bounds
            foreach (item curitem in items())
            {
                curitem.m_bounds.x0 = target_bounds.x0 + (curitem.m_rawbounds.x0 - xoffs) * xscale;
                curitem.m_bounds.x1 = target_bounds.x0 + (curitem.m_rawbounds.x1 - xoffs) * xscale;
                curitem.m_bounds.y0 = target_bounds.y0 + (curitem.m_rawbounds.y0 - yoffs) * yscale;
                curitem.m_bounds.y1 = target_bounds.y0 + (curitem.m_rawbounds.y1 - yoffs) * yscale;
            }

            // sort edges of interactive items
            rendlay_global.LOGMASKED(rendlay_global.LOG_INTERACTIVE_ITEMS, "Recalculated view '{0}' with {1} interactive items\n", name(), m_interactive_items.size());
            m_interactive_edges_x.reserve(m_interactive_items.size() * 2);
            m_interactive_edges_y.reserve(m_interactive_items.size() * 2);
            for (unsigned i = 0; m_interactive_items.size() > i; ++i)
            {
                item curitem = m_interactive_items[i];
                rendlay_global.LOGMASKED(rendlay_global.LOG_INTERACTIVE_ITEMS, "{0}: ({1} {2} {3} {4}) hasinput={5} clickthrough={6}\n", i, curitem.bounds().x0, curitem.bounds().y0, curitem.bounds().x1, curitem.bounds().y1, curitem.has_input(), curitem.clickthrough());
                m_interactive_edges_x.emplace_back(new edge(i, curitem.bounds().x0, false));
                m_interactive_edges_x.emplace_back(new edge(i, curitem.bounds().x1, true));
                m_interactive_edges_y.emplace_back(new edge(i, curitem.bounds().y0, false));
                m_interactive_edges_y.emplace_back(new edge(i, curitem.bounds().y1, true));
            }

            m_interactive_edges_x.Sort();  //std::sort(m_interactive_edges_x.begin(), m_interactive_edges_x.end());
            m_interactive_edges_y.Sort();  //std::sort(m_interactive_edges_y.begin(), m_interactive_edges_y.end());

            if ((rendlay_global.VERBOSE & rendlay_global.LOG_INTERACTIVE_ITEMS) != 0)
            {
                foreach (edge e in m_interactive_edges_x)
                    rendlay_global.LOGMASKED(rendlay_global.LOG_INTERACTIVE_ITEMS, "x={0} {1}{2}\n", e.position(), e.trailing() ? ']' : '[', e.index());
                foreach (edge e in m_interactive_edges_y)
                    rendlay_global.LOGMASKED(rendlay_global.LOG_INTERACTIVE_ITEMS, "y={0} {1}{2}\n", e.position(), e.trailing() ? ']' : '[', e.index());
            }
        }


        // resolve tags, if any
        //-----------------------------
        //  resolve_tags - resolve tags
        //-----------------------------
        public void resolve_tags()
        {
            foreach (item curitem in items())
                curitem.resolve_tags();
        }


        //-------------------------------------------------
        //  add_items - add items, recursing for groups
        //-------------------------------------------------
        void add_items(
                layer_lists layers,
                layout_view_view_environment env,
                util.xml.data_node parentnode,
                layout_view_element_map elemmap,
                layout_view_group_map groupmap,
                int orientation,
                float [,] trans,  //transform trans,
                render_color color,
                bool root,
                bool repeat,
                bool init)
        {
            bool envaltered = false;
            bool unresolved = true;
            for (util.xml.data_node itemnode = parentnode.get_first_child(); itemnode != null; itemnode = itemnode.get_next_sibling())
            {
                if (strcmp(itemnode.get_name(), "bounds") == 0)
                {
                    // set explicit bounds
                    if (root)
                        env.parse_bounds(itemnode, out m_expbounds);
                }
                else if (strcmp(itemnode.get_name(), "param") == 0)
                {
                    envaltered = true;
                    if (!unresolved)
                    {
                        unresolved = true;
                        foreach (var group in groupmap)
                            group.second().set_bounds_unresolved();
                    }

                    if (!repeat)
                        env.set_parameter(itemnode);
                    else
                        env.set_repeat_parameter(itemnode, init);
                }
                else if (strcmp(itemnode.get_name(), "screen") == 0)
                {
                    layers.screens.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                }
                else if (strcmp(itemnode.get_name(), "element") == 0)
                {
                    layers.screens.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (strcmp(itemnode.get_name(), "backdrop") == 0)
                {
                    if (layers.backdrops.empty())
                        osd_printf_warning("Warning: layout view '{0}' contains deprecated backdrop element\n", name());
                    layers.backdrops.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (strcmp(itemnode.get_name(), "overlay") == 0)
                {
                    if (layers.overlays.empty())
                        osd_printf_warning("Warning: layout view '{0}' contains deprecated overlay element\n", name());
                    layers.overlays.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (strcmp(itemnode.get_name(), "bezel") == 0)
                {
                    if (layers.bezels.empty())
                        osd_printf_warning("Warning: layout view '{0}' contains deprecated bezel element\n", name());

                    layers.bezels.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (strcmp(itemnode.get_name(), "cpanel") == 0)
                {
                    if (layers.cpanels.empty())
                        osd_printf_warning("Warning: layout view '{0}' contains deprecated cpanel element\n", name());

                    layers.cpanels.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (strcmp(itemnode.get_name(), "marquee") == 0)
                {
                    if (layers.marquees.empty())
                        osd_printf_warning("Warning: layout view '{0}' contains deprecated marquee element\n", name());

                    layers.marquees.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (strcmp(itemnode.get_name(), "group") == 0)
                {
                    string ref_ = env.get_attribute_string(itemnode, "ref", null);
                    if (ref_ == null)
                        throw new layout_syntax_error("group instantiation must have ref attribute");

                    var found = groupmap.find(ref_);
                    if (found == null)
                        throw new layout_syntax_error(string_format("unable to find group {0}", ref_));

                    unresolved = false;
                    found.resolve_bounds(env, groupmap);

                    float [,] grouptrans = trans;
                    util.xml.data_node itemboundsnode = itemnode.get_child("bounds");
                    util.xml.data_node itemorientnode = itemnode.get_child("orientation");
                    int grouporient = env.parse_orientation(itemorientnode);
                    if (itemboundsnode != null)
                    {
                        render_bounds itembounds;
                        env.parse_bounds(itemboundsnode, out itembounds);
                        grouptrans = found.make_transform(grouporient, itembounds, trans);
                    }
                    else if (itemorientnode != null)
                    {
                        grouptrans = found.make_transform(grouporient, trans);
                    }

                    layout_view_view_environment local = new layout_view_view_environment(env, false);
                    add_items(
                            layers,
                            local,
                            found.get_groupnode(),
                            elemmap,
                            groupmap,
                            rendutil_global.orientation_add(grouporient, orientation),
                            grouptrans,
                            rendlay_global.render_color_multiply(env.parse_color(itemnode.get_child("color")), color),
                            false,
                            false,
                            true);
                }
                else if (strcmp(itemnode.get_name(), "repeat") == 0)
                {
                    int count = env.get_attribute_int(itemnode, "count", -1);
                    if (0 >= count)
                        throw new layout_syntax_error("repeat must have positive integer count attribute");

                    layout_view_view_environment local = new layout_view_view_environment(env, false);
                    for (int i = 0; count > i; ++i)
                    {
                        add_items(layers, local, itemnode, elemmap, groupmap, orientation, trans, color, false, true, i == 0);
                        local.increment_parameters();
                    }
                }
                else if (strcmp(itemnode.get_name(), "collection") == 0)
                {
                    string name = env.get_attribute_string(itemnode, "name", null);
                    if (string.IsNullOrEmpty(name))
                        throw new layout_syntax_error("collection must have name attribute");

                    var found = std.find_if(m_vistoggles, (x) => { return x.name() == name; });  //var found = std::find_if(m_vistoggles.begin(), m_vistoggles.end(), [name] (auto const &x) { return x.name() == name; });
                    if (default != found)
                        throw new layout_syntax_error(string_format("duplicate collection name '{0}'", name));

                    m_defvismask |= (u32)(env.get_attribute_bool(itemnode, "visible", true) ? 1 : 0) << m_vistoggles.size(); // TODO: make this less hacky
                    layout_view_view_environment local = new layout_view_view_environment(env, true);
                    m_vistoggles.emplace_back(new visibility_toggle(name, local.visibility_mask()));
                    add_items(layers, local, itemnode, elemmap, groupmap, orientation, trans, color, false, false, true);
                }
                else
                {
                    throw new layout_syntax_error(string_format("unknown view item {0}", itemnode.get_name()));
                }
            }

            if (envaltered && !unresolved)
            {
                foreach (var group in groupmap)
                    group.second().set_bounds_unresolved();
            }
        }


        string make_name(emu.render.detail.layout_environment env, util.xml.data_node viewnode)
        {
            string name = env.get_attribute_string(viewnode, "name", null);
            if (name == null)
                throw new layout_syntax_error("view must have name attribute");

            if (env.is_root_device())
            {
                return name;
            }
            else
            {
                string tag = env.device().tag();
                if (':' == tag[0])
                    tag = tag.Substring(1);  //++tag;

                return string_format("{0} {1}", tag, name);
            }
        }
    }


    partial class layout_file : global_object
    {
        // construction/destruction
        //-------------------------------------------------
        //  layout_file - constructor
        //-------------------------------------------------
        public layout_file(device_t device, util.xml.data_node rootnode, string dirname)
        {
            m_elemmap = new layout_file_element_map();
            m_viewlist = new layout_file_view_list();


            try
            {
                layout_file_environment env = new layout_file_environment(device);

                // find the layout node
                util.xml.data_node mamelayoutnode = rootnode.get_child("mamelayout");
                if (mamelayoutnode == null)
                    throw new layout_syntax_error("missing mamelayout node");

                // validate the config data version
                int version = (int)mamelayoutnode.get_attribute_int("version", 0);
                if (version != rendlay_global.LAYOUT_VERSION)
                    throw new layout_syntax_error(string_format("unsupported version {0}", version));

                // parse all the parameters, elements and groups
                layout_file_group_map groupmap = new layout_file_group_map();
                add_elements(dirname, env, mamelayoutnode, groupmap, false, true);

                // parse all the views
                for (util.xml.data_node viewnode = mamelayoutnode.get_child("view"); viewnode != null; viewnode = viewnode.get_next_sibling("view"))
                {
                    // the trouble with allowing errors to propagate here is that it wreaks havoc with screenless systems that use a terminal by default
                    // e.g. intlc44 and intlc440 have a terminal on the TTY port by default and have a view with the front panel with the terminal screen
                    // however, they have a second view with just the front panel which is very useful if you're using e.g. -tty null_modem with a socket
                    // if the error is allowed to propagate, the entire layout is dropped so you can't select the useful view
                    try
                    {
                        m_viewlist.emplace_back(new layout_view(env, viewnode, m_elemmap, groupmap));
                    }
                    catch (layout_reference_error err)
                    {
                        osd_printf_warning("Error instantiating layout view {0}: {1}\n", env.get_attribute_string(viewnode, "name", ""), err);
                    }
                }
            }
            catch (layout_syntax_error err)
            {
                // syntax errors are always fatal
                throw new emu_fatalerror("Error parsing XML layout: {0}", err);
            }
        }


        void add_elements(
                string dirname,
                layout_file_environment env,
                util.xml.data_node parentnode,
                layout_file_group_map groupmap,
                bool repeat,
                bool init)
        {
            for (util.xml.data_node childnode = parentnode.get_first_child(); childnode != null; childnode = childnode.get_next_sibling())
            {
                if (strcmp(childnode.get_name(), "param") == 0)
                {
                    if (!repeat)
                        env.set_parameter(childnode);
                    else
                        env.set_repeat_parameter(childnode, init);
                }
                else if (strcmp(childnode.get_name(), "element") == 0)
                {
                    string name = env.get_attribute_string(childnode, "name", null);
                    if (name == null)
                        throw new layout_syntax_error("element lacks name attribute");
                    if (!m_elemmap.emplace(name, new layout_element(env, childnode, dirname)))  //if (!m_elemmap.emplace(std::piecewise_construct, std::forward_as_tuple(name), std::forward_as_tuple(env, childnode, dirname)).second)
                        throw new layout_syntax_error(string_format("duplicate element name {0}", name));
                    m_elemmap.emplace(name, new layout_element(env, childnode, dirname));
                }
                else if (strcmp(childnode.get_name(), "group") == 0)
                {
                    string name = env.get_attribute_string(childnode, "name", null);
                    if (name == null)
                        throw new layout_syntax_error("group lacks name attribute");
                    if (!groupmap.emplace(name, new layout_group(childnode)))  //if (!groupmap.emplace(std::piecewise_construct, std::forward_as_tuple(name), std::forward_as_tuple(childnode)).second)
                        throw new layout_syntax_error(string_format("duplicate group name {0}", name));
                    groupmap.emplace(name, new layout_group(childnode));
                }
                else if (strcmp(childnode.get_name(), "repeat") == 0)
                {
                    int count = env.get_attribute_int(childnode, "count", -1);
                    if (0 >= count)
                        throw new layout_syntax_error("repeat must have positive integer count attribute");
                    layout_file_environment local = new layout_file_environment(env);
                    for (int i = 0; count > i; ++i)
                    {
                        add_elements(dirname, local, childnode, groupmap, true, i == 0);
                        local.increment_parameters();
                    }
                }
                else if (repeat || (strcmp(childnode.get_name(), "view") != 0 && strcmp(childnode.get_name(), "script") != 0))
                {
                    throw new layout_syntax_error(string_format("unknown layout item {0}", childnode.get_name()));
                }
            }
        }
    }
}
