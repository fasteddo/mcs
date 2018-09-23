// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using environment = mame.emu.render.detail.layout_environment;
using item_list = mame.std_list<mame.layout_view.item>;
using view_list = mame.std_list<mame.layout_view>;


namespace mame
{
    enum LINE_CAP
    {
        LINE_CAP_NONE = 0,
        LINE_CAP_START = 1,
        LINE_CAP_END = 2
    }


    namespace emu.render.detail
    {
        public class layout_environment
        {
            class entry
            {
#if false
                entry(std::string &&name, std::string &&t)
                    : m_name(std::move(name))
                    , m_text(std::move(t))
                    , m_text_valid(true)
                { }
                entry(std::string &&name, s64 i)
                    : m_name(std::move(name))
                    , m_int(i)
                    , m_int_valid(true)
                { }
                entry(std::string &&name, double f)
                    : m_name(std::move(name))
                    , m_float(f)
                    , m_float_valid(true)
                { }
                entry(std::string &&name, std::string &&t, s64 i, int s)
                    : m_name(std::move(name))
                    , m_text(std::move(t))
                    , m_int_increment(i)
                    , m_shift(s)
                    , m_text_valid(true)
                    , m_incrementing(true)
                { }
                entry(std::string &&name, std::string &&t, double i, int s)
                    : m_name(std::move(name))
                    , m_text(std::move(t))
                    , m_float_increment(i)
                    , m_shift(s)
                    , m_text_valid(true)
                    , m_incrementing(true)
                { }
                entry(entry &&) = default;
                entry &operator=(entry &&) = default;

                void set(std::string &&t)
                {
                    m_text = std::move(t);
                    m_text_valid = true;
                    m_int_valid = false;
                    m_float_valid = false;
                }
                void set(s64 i)
                {
                    m_int = i;
                    m_text_valid = false;
                    m_int_valid = true;
                    m_float_valid = false;
                }
                void set(double f)
                {
                    m_float = f;
                    m_text_valid = false;
                    m_int_valid = false;
                    m_float_valid = true;
                }

                std::string const &name() const { return m_name; }
                bool is_incrementing() const { return m_incrementing; }

                std::string const &get_text()
                {
                    if (!m_text_valid)
                    {
                        if (m_float_valid)
                        {
                            m_text = std::to_string(m_float);
                            m_text_valid = true;
                        }
                        else if (m_int_valid)
                        {
                            m_text = std::to_string(m_int);
                            m_text_valid = true;
                        }
                    }
                    return m_text;
                }

                void increment()
                {
                    if (is_incrementing())
                    {
                        // apply increment
                        if (m_float_increment)
                        {
                            if (m_int_valid && !m_float_valid)
                            {
                                m_float = m_int;
                                m_float_valid = true;
                            }
                            if (m_text_valid && !m_float_valid)
                            {
                                std::istringstream stream(m_text);
                                stream.imbue(f_portable_locale);
                                m_text.c_str();
                                if (m_text[0] == '$')
                                {
                                    stream.get();
                                    u64 uvalue;
                                    stream >> std::hex >> uvalue;
                                    m_float = uvalue;
                                }
                                else if ((m_text[0] == '0') && ((m_text[1] == 'x') || (m_text[1] == 'X')))
                                {
                                    stream.get();
                                    stream.get();
                                    u64 uvalue;
                                    stream >> std::hex >> uvalue;
                                    m_float = uvalue;
                                }
                                else if (m_text[0] == '#')
                                {
                                    stream.get();
                                    stream >> m_int;
                                    m_float = m_int;
                                }
                                else
                                {
                                    stream >> m_float;
                                }
                                m_float_valid = bool(stream);
                            }
                            m_float += m_float_increment;
                            m_int_valid = m_text_valid = false;
                        }
                        else
                        {
                            if (m_text_valid && !m_int_valid && !m_float_valid)
                            {
                                std::istringstream stream(m_text);
                                stream.imbue(f_portable_locale);
                                m_text.c_str();
                                if (m_text[0] == '$')
                                {
                                    stream.get();
                                    u64 uvalue;
                                    stream >> std::hex >> uvalue;
                                    m_int = s64(uvalue);
                                    m_int_valid = bool(stream);
                                }
                                else if ((m_text[0] == '0') && ((m_text[1] == 'x') || (m_text[1] == 'X')))
                                {
                                    stream.get();
                                    stream.get();
                                    u64 uvalue;
                                    stream >> std::hex >> uvalue;
                                    m_int = s64(uvalue);
                                    m_int_valid = bool(stream);
                                }
                                else if (m_text[0] == '#')
                                {
                                    stream.get();
                                    stream >> m_int;
                                    m_int_valid = bool(stream);
                                }
                                else if (m_text.find_first_of(".eE") != std::string::npos)
                                {
                                    stream >> m_float;
                                    m_float_valid = bool(stream);
                                }
                                else
                                {
                                    stream >> m_int;
                                    m_int_valid = bool(stream);
                                }
                            }

                            if (m_float_valid)
                            {
                                m_float += m_int_increment;
                                m_int_valid = m_text_valid = false;
                            }
                            else
                            {
                                m_int += m_int_increment;
                                m_float_valid = m_text_valid = false;
                            }
                        }

                        // apply shift
                        if (m_shift)
                        {
                            if (m_float_valid && !m_int_valid)
                            {
                                m_int = s64(m_float);
                                m_int_valid = true;
                            }
                            if (m_text_valid && !m_int_valid)
                            {
                                std::istringstream stream(m_text);
                                stream.imbue(f_portable_locale);
                                m_text.c_str();
                                if (m_text[0] == '$')
                                {
                                    stream.get();
                                    u64 uvalue;
                                    stream >> std::hex >> uvalue;
                                    m_int = s64(uvalue);
                                }
                                else if ((m_text[0] == '0') && ((m_text[1] == 'x') || (m_text[1] == 'X')))
                                {
                                    stream.get();
                                    stream.get();
                                    u64 uvalue;
                                    stream >> std::hex >> uvalue;
                                    m_int = s64(uvalue);
                                }
                                else
                                {
                                    if (m_text[0] == '#')
                                        stream.get();
                                    stream >> m_int;
                                }
                                m_int_valid = bool(stream);
                            }
                            if (0 > m_shift)
                                m_int >>= -m_shift;
                            else
                                m_int <<= m_shift;
                            m_text_valid = m_float_valid = false;
                        }
                    }
                }

                static bool name_less(entry const &lhs, entry const &rhs) { return lhs.name() < rhs.name(); }

            private:
                std::string m_name;
                std::string m_text;
                s64 m_int = 0, m_int_increment = 0;
                double m_float = 0.0, m_float_increment = 0.0;
                int m_shift = 0;
                bool m_text_valid = false;
                bool m_int_valid = false;
                bool m_float_valid = false;
                bool m_incrementing = false;
#endif
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

            template <typename T, typename U>
            void set(T &&name, U &&value)
            {
                entry_vector::iterator const pos(
                        std::lower_bound(
                            m_entries.begin(),
                            m_entries.end(),
                            name,
                            [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                if ((m_entries.end() == pos) || (pos->name() != name))
                    m_entries.emplace(pos, std::forward<T>(name), std::forward<U>(value));
                else
                    pos->set(std::forward<U>(value));
            }

            void cache_device_entries()
            {
                if (!m_next && !m_cached)
                {
                    try_insert("devicetag", device().tag());
                    try_insert("devicebasetag", device().basetag());
                    try_insert("devicename", device().name());
                    try_insert("deviceshortname", device().shortname());
                    util::ovectorstream tmp;
                    unsigned i(0U);
                    for (screen_device const &screen : screen_device_iterator(machine().root_device()))
                    {
                        s64 const w(screen.visible_area().width()), h(screen.visible_area().height());
                        s64 xaspect(w), yaspect(h);
                        reduce_fraction(xaspect, yaspect);

                        tmp.seekp(0);
                        util::stream_format(tmp, "scr%unativexaspect", i);
                        tmp.put('\0');
                        try_insert(&tmp.vec()[0], xaspect);

                        tmp.seekp(0);
                        util::stream_format(tmp, "scr%unativeyaspect", i);
                        tmp.put('\0');
                        try_insert(&tmp.vec()[0], yaspect);

                        tmp.seekp(0);
                        util::stream_format(tmp, "scr%uwidth", i);
                        tmp.put('\0');
                        try_insert(&tmp.vec()[0], w);

                        tmp.seekp(0);
                        util::stream_format(tmp, "scr%uheight", i);
                        tmp.put('\0');
                        try_insert(&tmp.vec()[0], h);

                        ++i;
                    }
                    m_cached = true;
                }
            }

            entry *find_entry(char const *begin, char const *end)
            {
                cache_device_entries();
                entry_vector::iterator const pos(
                        std::lower_bound(
                            m_entries.begin(),
                            m_entries.end(),
                            std::make_pair(begin, end - begin),
                            [] (entry const &lhs, std::pair<char const *, std::ptrdiff_t> const &rhs)
                            { return 0 > std::strncmp(lhs.name().c_str(), rhs.first, rhs.second); }));
                if ((m_entries.end() != pos) && (pos->name().length() == (end - begin)) && !std::strncmp(pos->name().c_str(), begin, end - begin))
                    return &*pos;
                else
                    return m_next ? m_next->find_entry(begin, end) : nullptr;
            }

            template <typename... T>
            std::tuple<char const *, char const *, bool> get_variable_text(T &&... args)
            {
                entry *const found(find_entry(std::forward<T>(args)...));
                if (found)
                {
                    std::string const &text(found->get_text());
                    char const *const begin(text.c_str());
                    return std::make_tuple(begin, begin + text.length(), true);
                }
                else
                {
                    return std::make_tuple(nullptr, nullptr, false);
                }
            }

            std::pair<char const *, char const *> expand(char const *begin, char const *end)
            {
                // search for candidate variable references
                char const *start(begin);
                char const *pos(std::find_if(start, end, is_variable_start));
                while (pos != end)
                {
                    char const *const term(std::find_if(pos + 1, end, [] (char ch) { return !is_variable_char(ch); }));
                    if ((term == end) || !is_variable_end(*term))
                    {
                        // not a valid variable name - keep searching
                        pos = std::find_if(term, end, is_variable_start);
                    }
                    else
                    {
                        // looks like a variable reference - try to look it up
                        std::tuple<char const *, char const *, bool> const text(get_variable_text(pos + 1, term));
                        if (std::get<2>(text))
                        {
                            // variable found
                            if (begin == start)
                                m_buffer.seekp(0);
                            m_buffer.write(start, pos - start);
                            m_buffer.write(std::get<0>(text), std::get<1>(text) - std::get<0>(text));
                            start = term + 1;
                            pos = std::find_if(start, end, is_variable_start);
                        }
                        else
                        {
                            // variable not found - move on
                            pos = std::find_if(pos + 1, end, is_variable_start);
                        }
                    }
                }

                // short-circuit the case where no substitutions were made
                if (start == begin)
                {
                    return std::make_pair(begin, end);
                }
                else
                {
                    m_buffer.write(start, pos - start);
                    m_buffer.put('\0');
                    std::vector<char> const &vec(m_buffer.vec());
                    if (vec.empty())
                        return std::make_pair(nullptr, nullptr);
                    else
                        return std::make_pair(&vec[0], &vec[0] + vec.size() - 1);
                }
            }

            std::pair<char const *, char const *> expand(char const *str)
            {
                return expand(str, str + strlen(str));
            }

            std::string parameter_name(util::xml::data_node const &node)
            {
                char const *const attrib(node.get_attribute_string("name", nullptr));
                if (!attrib)
                    throw layout_syntax_error("parameter lacks name attribute");
                std::pair<char const *, char const *> const expanded(expand(attrib));
                return std::string(expanded.first, expanded.second);
            }

            static constexpr bool is_variable_start(char ch)
            {
                return '~' == ch;
            }
            static constexpr bool is_variable_end(char ch)
            {
                return '~' == ch;
            }
            static constexpr bool is_variable_char(char ch)
            {
                return (('0' <= ch) && ('9' >= ch)) || (('A' <= ch) && ('Z' >= ch)) || (('a' <= ch) && ('z' >= ch)) || ('_' == ch);
            }
#endif

            //entry_vector m_entries;
            //util::ovectorstream m_buffer;
            device_t m_device;
            //layout_environment *const m_next = nullptr;
            //bool m_cached = false;


            public layout_environment(device_t device) { m_device = device; }
            //explicit layout_environment(layout_environment &next) : m_device(next.m_device), m_next(&next) { }
            //layout_environment(layout_environment const &) = delete;

            device_t device() { return m_device; }
            public running_machine machine() { return device().machine(); }

#if false
            bool is_root_device() { return &device() == &machine().root_device(); }

            void set_parameter(std::string &&name, std::string &&value)
            {
                set(std::move(name), std::move(value));
            }

            void set_parameter(std::string &&name, s64 value)
            {
                set(std::move(name), value);
            }

            void set_parameter(std::string &&name, double value)
            {
                set(std::move(name), value);
            }

            void set_parameter(util::xml::data_node const &node)
            {
                // do basic validation
                std::string name(parameter_name(node));
                if (node.has_attribute("start") || node.has_attribute("increment") || node.has_attribute("lshift") || node.has_attribute("rshift"))
                    throw layout_syntax_error("start/increment/lshift/rshift attributes are only allowed for repeat parameters");
                char const *const value(node.get_attribute_string("value", nullptr));
                if (!value)
                    throw layout_syntax_error("parameter lacks value attribute");

                // expand value and stash
                std::pair<char const *, char const *> const expanded(expand(value));
                set(std::move(name), std::string(expanded.first, expanded.second));
            }

            void set_repeat_parameter(util::xml::data_node const &node, bool init)
            {
                // two types are allowed here - static value, and start/increment/lshift/rshift
                std::string name(parameter_name(node));
                char const *const start(node.get_attribute_string("start", nullptr));
                if (start)
                {
                    // simple validity checks
                    if (node.has_attribute("value"))
                        throw layout_syntax_error("start attribute may not be used in combination with value attribute");
                    int const lshift(node.has_attribute("lshift") ? get_attribute_int(node, "lshift", -1) : 0);
                    int const rshift(node.has_attribute("rshift") ? get_attribute_int(node, "rshift", -1) : 0);
                    if ((0 > lshift) || (0 > rshift))
                        throw layout_syntax_error("lshift/rshift attributes must be non-negative integers");

                    // increment is more complex - it may be an integer or a floating-point number
                    s64 intincrement(0);
                    double floatincrement(0);
                    char const *const increment(node.get_attribute_string("increment", nullptr));
                    if (increment)
                    {
                        std::pair<char const *, char const *> const expanded(expand(increment));
                        unsigned const hexprefix((expanded.first[0] == '$') ? 1U : ((expanded.first[0] == '0') && ((expanded.first[1] == 'x') || (expanded.first[1] == 'X'))) ? 2U : 0U);
                        unsigned const decprefix((expanded.first[0] == '#') ? 1U : 0U);
                        bool const floatchars(std::find_if(expanded.first, expanded.second, [] (char ch) { return ('.' == ch) || ('e' == ch) || ('E' == ch); }) != expanded.second);
                        std::istringstream stream(std::string(expanded.first + hexprefix + decprefix, expanded.second));
                        stream.imbue(f_portable_locale);
                        if (!hexprefix && !decprefix && floatchars)
                        {
                            stream >> floatincrement;
                        }
                        else if (hexprefix)
                        {
                            u64 uvalue;
                            stream >> std::hex >> uvalue;
                            intincrement = s64(uvalue);
                        }
                        else
                        {
                            stream >> intincrement;
                        }

                        // reject obviously bad stuff
                        if (!stream)
                            throw layout_syntax_error("increment attribute must be a number");
                    }

                    // don't allow incrementing parameters to be redefined
                    if (init)
                    {
                        entry_vector::iterator const pos(
                                std::lower_bound(
                                    m_entries.begin(),
                                    m_entries.end(),
                                    name,
                                    [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                        if ((m_entries.end() != pos) && (pos->name() == name))
                            throw layout_syntax_error("incrementing parameters must be defined exactly once per scope");

                        std::pair<char const *, char const *> const expanded(expand(start));
                        if (floatincrement)
                            m_entries.emplace(pos, std::move(name), std::string(expanded.first, expanded.second), floatincrement, lshift - rshift);
                        else
                            m_entries.emplace(pos, std::move(name), std::string(expanded.first, expanded.second), intincrement, lshift - rshift);
                    }
                }
                else if (node.has_attribute("increment") || node.has_attribute("lshift") || node.has_attribute("rshift"))
                {
                    throw layout_syntax_error("increment/lshift/rshift attributes require start attribute");
                }
                else
                {
                    char const *const value(node.get_attribute_string("value", nullptr));
                    if (!value)
                        throw layout_syntax_error("parameter lacks value attribute");
                    std::pair<char const *, char const *> const expanded(expand(value));
                    entry_vector::iterator const pos(
                            std::lower_bound(
                                m_entries.begin(),
                                m_entries.end(),
                                name,
                                [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                    if ((m_entries.end() == pos) || (pos->name() != name))
                        m_entries.emplace(pos, std::move(name), std::string(expanded.first, expanded.second));
                    else if (pos->is_incrementing())
                        throw layout_syntax_error("incrementing parameters must be defined exactly once per scope");
                    else
                        pos->set(std::string(expanded.first, expanded.second));
                }
            }

            void increment_parameters()
            {
                for (entry &e : m_entries)
                    e.increment();
            }

            char const *get_attribute_string(util::xml::data_node const &node, char const *name, char const *defvalue)
            {
                char const *const attrib(node.get_attribute_string(name, nullptr));
                return attrib ? expand(attrib).first : defvalue;
            }

            int get_attribute_int(util::xml::data_node const &node, const char *name, int defvalue)
            {
                char const *const attrib(node.get_attribute_string(name, nullptr));
                if (!attrib)
                    return defvalue;

                // similar to what XML nodes do
                std::pair<char const *, char const *> const expanded(expand(attrib));
                std::istringstream stream;
                stream.imbue(f_portable_locale);
                int result;
                if (expanded.first[0] == '$')
                {
                    stream.str(std::string(expanded.first + 1, expanded.second));
                    unsigned uvalue;
                    stream >> std::hex >> uvalue;
                    result = int(uvalue);
                }
                else if ((expanded.first[0] == '0') && ((expanded.first[1] == 'x') || (expanded.first[1] == 'X')))
                {
                    stream.str(std::string(expanded.first + 2, expanded.second));
                    unsigned uvalue;
                    stream >> std::hex >> uvalue;
                    result = int(uvalue);
                }
                else if (expanded.first[0] == '#')
                {
                    stream.str(std::string(expanded.first + 1, expanded.second));
                    stream >> result;
                }
                else
                {
                    stream.str(std::string(expanded.first, expanded.second));
                    stream >> result;
                }

                return stream ? result : defvalue;
            }

            float get_attribute_float(util::xml::data_node const &node, char const *name, float defvalue)
            {
                char const *const attrib(node.get_attribute_string(name, nullptr));
                if (!attrib)
                    return defvalue;

                // similar to what XML nodes do
                std::pair<char const *, char const *> const expanded(expand(attrib));
                std::istringstream stream(std::string(expanded.first, expanded.second));
                stream.imbue(f_portable_locale);
                float result;
                return (stream >> result) ? result : defvalue;
            }

            void parse_bounds(util::xml::data_node const *node, render_bounds &result)
            {
                // default to unit rectangle
                if (!node)
                {
                    result.x0 = result.y0 = 0.0F;
                    result.x1 = result.y1 = 1.0F;
                }
                else
                {
                    // parse attributes
                    if (node->has_attribute("left"))
                    {
                        // left/right/top/bottom format
                        result.x0 = get_attribute_float(*node, "left", 0.0F);
                        result.x1 = get_attribute_float(*node, "right", 1.0F);
                        result.y0 = get_attribute_float(*node, "top", 0.0F);
                        result.y1 = get_attribute_float(*node, "bottom", 1.0F);
                    }
                    else if (node->has_attribute("x"))
                    {
                        // x/y/width/height format
                        result.x0 = get_attribute_float(*node, "x", 0.0F);
                        result.x1 = result.x0 + get_attribute_float(*node, "width", 1.0F);
                        result.y0 = get_attribute_float(*node, "y", 0.0F);
                        result.y1 = result.y0 + get_attribute_float(*node, "height", 1.0F);
                    }
                    else
                    {
                        throw layout_syntax_error("bounds element requires either left or x attribute");
                    }

                    // check for errors
                    if ((result.x0 > result.x1) || (result.y0 > result.y1))
                        throw layout_syntax_error(util::string_format("illegal bounds (%f-%f)-(%f-%f)", result.x0, result.x1, result.y0, result.y1));
                }
            }

            void parse_color(util::xml::data_node const *node, render_color &result)
            {
                // default to white
                if (!node)
                {
                    result.r = result.g = result.b = result.a = 1.0F;
                }
                else
                {
                    // parse attributes
                    result.r = get_attribute_float(*node, "red", 1.0F);
                    result.g = get_attribute_float(*node, "green", 1.0F);
                    result.b = get_attribute_float(*node, "blue", 1.0F);
                    result.a = get_attribute_float(*node, "alpha", 1.0F);

                    // check for errors
                    if ((0.0F > (std::min)({ result.r, result.g, result.b, result.a })) || (1.0F < (std::max)({ result.r, result.g, result.b, result.a })))
                        throw layout_syntax_error(util::string_format("illegal RGBA color %f,%f,%f,%f", result.r, result.g, result.b, result.a));
                }
            }

            void parse_orientation(util::xml::data_node const *node, int &result)
            {
                // default to no transform
                if (!node)
                {
                    result = ROT0;
                }
                else
                {
                    // parse attributes
                    int const rotate(get_attribute_int(*node, "rotate", 0));
                    switch (rotate)
                    {
                        case 0:     result = ROT0;      break;
                        case 90:    result = ROT90;     break;
                        case 180:   result = ROT180;    break;
                        case 270:   result = ROT270;    break;
                        default:    throw layout_syntax_error(util::string_format("invalid rotate attribute %d", rotate));
                    }
                    if (!std::strcmp("yes", get_attribute_string(*node, "swapxy", "no")))
                        result ^= ORIENTATION_SWAP_XY;
                    if (!std::strcmp("yes", get_attribute_string(*node, "flipx", "no")))
                        result ^= ORIENTATION_FLIP_X;
                    if (!std::strcmp("yes", get_attribute_string(*node, "flipy", "no")))
                        result ^= ORIENTATION_FLIP_Y;
                }
            }
#endif
        }
    } // namespace emu::render::detail


    public partial class layout_element
    {
        abstract partial class component
        {
            // construction/destruction
            //-------------------------------------------------
            //  component - constructor
            //-------------------------------------------------
            public component(environment env, util.xml.data_node compnode, string dirname)
            {
                m_state = 0;


                //throw new emu_unimplemented();
#if false
                for (int i = 0; i < MAX_BITMAPS; i++)
                    m_hasalpha[i] = false;


                // fetch common data
                m_state = xml_get_attribute_int_with_subst(machine, compnode, "state", -1);
                parse_bounds(machine, xml_get_sibling(compnode.child, "bounds"), m_bounds);
                parse_color(machine, xml_get_sibling(compnode.child, "color"), m_color);

                // image nodes
                if (strcmp(compnode.name, "image") == 0)
                {
                    m_type = CTYPE_IMAGE;
                    if (dirname != NULL)
                        m_dirname = dirname;
                    m_imagefile[0] = xml_get_attribute_string_with_subst(machine, compnode, "file", "");
                    m_alphafile[0] = xml_get_attribute_string_with_subst(machine, compnode, "alphafile", "");
                    m_file[0].reset(global_alloc(emu_file(machine.options().art_path(), OPEN_FLAG_READ)));
                }

                // text nodes
                else if (strcmp(compnode.name, "text") == 0)
                {
                    m_type = CTYPE_TEXT;
                    m_string = xml_get_attribute_string_with_subst(machine, compnode, "string", "");
                    m_textalign = xml_get_attribute_int_with_subst(machine, compnode, "align", 0);
                }

                // dotmatrix nodes
                else if (strcmp(compnode.name, "dotmatrix") == 0)
                {
                    m_type = CTYPE_DOTMATRIX;
                }
                else if (strcmp(compnode.name, "dotmatrix5dot") == 0)
                {
                    m_type = CTYPE_DOTMATRIX5DOT;
                }
                else if (strcmp(compnode.name, "dotmatrixdot") == 0)
                {
                    m_type = CTYPE_DOTMATRIXDOT;
                }

                // simplecounter nodes
                else if (strcmp(compnode.name, "simplecounter") == 0)
                {
                    m_type = CTYPE_SIMPLECOUNTER;
                    m_digits = xml_get_attribute_int_with_subst(machine, compnode, "digits", 2);
                    m_textalign = xml_get_attribute_int_with_subst(machine, compnode, "align", 0);
                }

                // fruit machine reels
                else if (strcmp(compnode.name, "reel") == 0)
                {
                    m_type = CTYPE_REEL;

                    astring symbollist = xml_get_attribute_string_with_subst(machine, compnode, "symbollist", "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15");

                    // split out position names from string and figure out our number of symbols
                    int location;
                    m_numstops = 0;
                    location=symbollist.find(0,",");
                    while (location!=-1)
                    {
                        m_stopnames[m_numstops] = symbollist;
                        m_stopnames[m_numstops].substr(0, location);
                        symbollist.substr(location+1, symbollist.len()-(location-1));
                        m_numstops++;
                        location=symbollist.find(0,",");
                    }
                    m_stopnames[m_numstops++] = symbollist;

                    // careful, dirname is NULL if we're coming from internal layout, and our string assignment doesn't like that
                    if (dirname != NULL)
                        m_dirname = dirname;

                    for (int i=0;i<m_numstops;i++)
                    {
                        location=m_stopnames[i].find(0,":");
                        if (location!=-1)
                        {
                            m_imagefile[i] = m_stopnames[i];
                            m_stopnames[i].substr(0, location);
                            m_imagefile[i].substr(location+1, m_imagefile[i].len()-(location-1));

                            //m_alphafile[i] =
                            m_file[i].reset(global_alloc(emu_file(machine.options().art_path(), OPEN_FLAG_READ)));
                        }
                        else
                        {
                            //m_imagefile[i] = 0;
                            //m_alphafile[i] = 0;
                            m_file[i].reset();
                        }
                    }

                    m_stateoffset = xml_get_attribute_int_with_subst(machine, compnode, "stateoffset", 0);
                    m_numsymbolsvisible = xml_get_attribute_int_with_subst(machine, compnode, "numsymbolsvisible", 3);
                    m_reelreversed = xml_get_attribute_int_with_subst(machine, compnode, "reelreversed", 0);
                    m_beltreel = xml_get_attribute_int_with_subst(machine, compnode, "beltreel", 0);

                }

                // led7seg nodes
                else if (strcmp(compnode.name, "led7seg") == 0)
                    m_type = CTYPE_LED7SEG;

                // led8seg_gts1 nodes
                else if (strcmp(compnode.name, "led8seg_gts1") == 0)
                    m_type = CTYPE_LED8SEG_GTS1;

                // led14seg nodes
                else if (strcmp(compnode.name, "led14seg") == 0)
                    m_type = CTYPE_LED14SEG;

                // led14segsc nodes
                else if (strcmp(compnode.name, "led14segsc") == 0)
                    m_type = CTYPE_LED14SEGSC;

                // led16seg nodes
                else if (strcmp(compnode.name, "led16seg") == 0)
                    m_type = CTYPE_LED16SEG;

                // led16segsc nodes
                else if (strcmp(compnode.name, "led16segsc") == 0)
                    m_type = CTYPE_LED16SEGSC;

                // rect nodes
                else if (strcmp(compnode.name, "rect") == 0)
                    m_type = CTYPE_RECT;

                // disk nodes
                else if (strcmp(compnode.name, "disk") == 0)
                    m_type = CTYPE_DISK;

                // error otherwise
                else
                    throw emu_fatalerror("Unknown element component: %s", compnode.name);
#endif
            }


            // helpers

            //-------------------------------------------------
            //  draw_text - draw text in the specified color
            //-------------------------------------------------
            void draw_text(render_font font, bitmap_argb32 dest, rectangle bounds, string str, int align)
            {
                // compute premultiplied colors
                UInt32 r = (UInt32)(m_color.r * 255.0f);
                UInt32 g = (UInt32)(m_color.g * 255.0f);
                UInt32 b = (UInt32)(m_color.b * 255.0f);
                UInt32 a = (UInt32)(m_color.a * 255.0f);

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
                draw_segment_horizontal_caps(dest, minx, maxx, midy, width, (int)(LINE_CAP.LINE_CAP_START | LINE_CAP.LINE_CAP_END), color);
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
                draw_segment_vertical_caps(dest, miny, maxy, midx, width, (int)(LINE_CAP.LINE_CAP_START | LINE_CAP.LINE_CAP_END), color);
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


        public partial class texture
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

            //-------------------------------------------------
            //  ~texture - destructor
            //-------------------------------------------------
            ~texture()
            {
                if (m_element != null)
                    m_element.machine().render().texture_free(m_texture);
            }
        }


#if false
        // image
        class image_component : component
        {
            // internal state
            //bitmap_argb32       m_bitmap;                   // source bitmap for images
            //std::string         m_dirname;                  // directory name of image file (for lazy loading)
            //std::unique_ptr<emu_file> m_file;               // file object for reading image/alpha files
            //std::string         m_imagefile;                // name of the image file (for lazy loading)
            //std::string         m_alphafile;                // name of the alpha file (for lazy loading)
            //bool                m_hasalpha;                 // is there any alpha component present?


            // construction/destruction
            //image_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);

            // internal helpers
            //void load_bitmap();
        }


        // rectangle
        class rect_component : component
        {
            // construction/destruction
            //rect_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // ellipse
        class disk_component : component
        {
            // construction/destruction
            //disk_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // text string
        class text_component : component
        {
            // internal state
            //std::string         m_string;                   // string for text components
            //int                 m_textalign;                // text alignment to box


            // construction/destruction
            //text_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // 7-segment LCD
        class led7seg_component : component
        {
            // construction/destruction
            //led7seg_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            protected override int maxstate() { return 255; }
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // 8-segment fluorescent (Gottlieb System 1)
        class led8seg_gts1_component : component
        {
            // construction/destruction
            //led8seg_gts1_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            protected override int maxstate() { return 255; }
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // 14-segment LCD
        class led14seg_component : component
        {
            // construction/destruction
            //led14seg_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            protected override int maxstate() { return 16383; }
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // 16-segment LCD
        class led16seg_component : component
        {
            // construction/destruction
            //led16seg_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            protected override int maxstate() { return 65535; }
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // 14-segment LCD with semicolon (2 extra segments)
        class led14segsc_component : component
        {
            // construction/destruction
            //led14segsc_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            protected override int maxstate() { return 65535; }
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // 16-segment LCD with semicolon (2 extra segments)
        class led16segsc_component : component
        {
            // construction/destruction
            //led16segsc_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            protected override int maxstate() { return 262143; }
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // row of dots for a dotmatrix
        class dotmatrix_component : component
        {
            // internal state
            //int                 m_dots;

            // construction/destruction
            //dotmatrix_component(int dots, running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            //protected override int maxstate() { return (1 << m_dots) - 1; }
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // simple counter
        class simplecounter_component : component
        {
            // internal state
            //int                 m_digits;                   // number of digits for simple counters
            //int                 m_textalign;                // text alignment to box
            //int                 m_maxstate;

            // construction/destruction
            //simplecounter_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            //protected override int maxstate() { return m_maxstate; }
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);
        }


        // fruit machine reel
        class reel_component : component
        {
            //static constexpr unsigned MAX_BITMAPS = 32;

            // internal state
            //bitmap_argb32       m_bitmap[MAX_BITMAPS];      // source bitmap for images
            //std::string         m_dirname;                  // directory name of image file (for lazy loading)
            //std::unique_ptr<emu_file> m_file[MAX_BITMAPS];        // file object for reading image/alpha files
            //std::string         m_imagefile[MAX_BITMAPS];   // name of the image file (for lazy loading)
            //std::string         m_alphafile[MAX_BITMAPS];   // name of the alpha file (for lazy loading)
            //bool                m_hasalpha[MAX_BITMAPS];    // is there any alpha component present?

            // basically made up of multiple text strings / gfx
            //int                 m_numstops;
            //std::string         m_stopnames[MAX_BITMAPS];
            //int                 m_stateoffset;
            //int                 m_reelreversed;
            //int                 m_numsymbolsvisible;
            //int                 m_beltreel;

            // construction/destruction
            //reel_component(running_machine machine, xml_data_node compnode, string dirname);

            // overrides
            protected override int maxstate() { return 65535; }
            //public override void draw(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state);

            // internal helpers
            //void draw_beltreel(running_machine &machine, bitmap_argb32 &dest, const rectangle &bounds, int state);
            //void load_reel_bitmap(int number);
        }
#endif


        // construction/destruction
        //-------------------------------------------------
        //  layout_element - constructor
        //-------------------------------------------------
        public layout_element(environment env, util.xml.data_node elemnode, string dirname)
        {
            m_machine = env.machine();
            m_defstate = 0;
            m_maxstate = 0;

            //throw new emu_unimplemented();
#if false
            // extract the name
            string name = xml_get_attribute_string_with_subst(machine, elemnode, "name", NULL);
            if (name == NULL)
                throw emu_fatalerror("All layout elements must have a name!\n");
            m_name = name;

            // get the default state
            m_defstate = xml_get_attribute_int_with_subst(machine, elemnode, "defstate", -1);

            // parse components in order
            bool first = true;
            render_bounds bounds = { 0 };
            for (xml_data_node *compnode = elemnode.child; compnode != NULL; compnode = compnode->next)
            {
                // allocate a new component
                component &newcomp = m_complist.append(*global_alloc(component(machine, *compnode, dirname)));

                // accumulate bounds
                if (first)
                    bounds = newcomp.m_bounds;
                else
                    union_render_bounds(&bounds, &newcomp.m_bounds);
                first = false;

                // determine the maximum state
                if (newcomp.m_state > m_maxstate)
                    m_maxstate = newcomp.m_state;
                if (newcomp.m_type == component::CTYPE_LED7SEG || newcomp.m_type == component::CTYPE_LED8SEG_GTS1)
                    m_maxstate = 255;
                if (newcomp.m_type == component::CTYPE_LED14SEG)
                    m_maxstate = 16383;
                if (newcomp.m_type == component::CTYPE_LED14SEGSC || newcomp.m_type == component::CTYPE_LED16SEG)
                    m_maxstate = 65535;
                if (newcomp.m_type == component::CTYPE_LED16SEGSC)
                    m_maxstate = 262143;
                if (newcomp.m_type == component::CTYPE_DOTMATRIX)
                    m_maxstate = 255;
                if (newcomp.m_type == component::CTYPE_DOTMATRIX5DOT)
                    m_maxstate = 31;
                if (newcomp.m_type == component::CTYPE_DOTMATRIXDOT)
                    m_maxstate = 1;
                if (newcomp.m_type == component::CTYPE_SIMPLECOUNTER)
                    m_maxstate = xml_get_attribute_int_with_subst(machine, *compnode, "maxstate", 999);
                if (newcomp.m_type == component::CTYPE_REEL)
                    m_maxstate = 65536;
            }

            if (m_complist.first() != NULL)
            {
                // determine the scale/offset for normalization
                float xoffs = bounds.x0;
                float yoffs = bounds.y0;
                float xscale = 1.0f / (bounds.x1 - bounds.x0);
                float yscale = 1.0f / (bounds.y1 - bounds.y0);

                // normalize all the component bounds
                for (component *curcomp = m_complist.first(); curcomp != NULL; curcomp = curcomp->next())
                {
                    curcomp->m_bounds.x0 = (curcomp->m_bounds.x0 - xoffs) * xscale;
                    curcomp->m_bounds.x1 = (curcomp->m_bounds.x1 - xoffs) * xscale;
                    curcomp->m_bounds.y0 = (curcomp->m_bounds.y0 - yoffs) * yscale;
                    curcomp->m_bounds.y1 = (curcomp->m_bounds.y1 - yoffs) * yscale;
                }
            }

            // allocate an array of element textures for the states
            m_elemtex.resize(m_maxstate + 1);
#endif
        }


        //-------------------------------------------------
        //  state_texture - return a pointer to a
        //  render_texture for the given state, allocating
        //  one if needed
        //-------------------------------------------------
        public render_texture state_texture(int state)
        {
            //assert(state <= m_maxstate);
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
            foreach (component curcomp in elemtex.m_element.m_complist)
            {
                if (curcomp.m_state == -1 || curcomp.m_state == elemtex.m_state)
                {
                    // get the local scaled bounds
                    rectangle bounds = new rectangle();
                    bounds.min_x = (int)rendutil_global.render_round_nearest(curcomp.bounds().x0 * dest.width());
                    bounds.min_y = (int)rendutil_global.render_round_nearest(curcomp.bounds().y0 * dest.height());
                    bounds.max_x = (int)rendutil_global.render_round_nearest(curcomp.bounds().x1 * dest.width());
                    bounds.max_y = (int)rendutil_global.render_round_nearest(curcomp.bounds().y1 * dest.height());
                    bounds.intersection(dest.cliprect());

                    // based on the component type, add to the texture
                    curcomp.draw(elemtex.m_element.machine(), dest, bounds, elemtex.m_state);
                }
            }
        }
    }


    public partial class layout_group
    {
        //-------------------------------------------------
        //  layout_group - constructor
        //-------------------------------------------------
        layout_group(util.xml.data_node groupnode)
        {
            m_groupnode = groupnode;
            m_bounds = new render_bounds(0.0f, 0.0f, 0.0f, 0.0f);
            m_bounds_resolved = false;
        }
    }


    public partial class layout_view
    {
        public partial class item
        {
            // construction/destruction
            //-------------------------------------------------
            //  item - constructor
            //-------------------------------------------------
            public item(
                    environment env,
                    util.xml.data_node itemnode,
                    Dictionary<string, layout_element> elemmap,
                    render_bounds transform)
            {
                m_element = null;
                //m_output(env.device(), env.get_attribute_string(itemnode, "name", ""))
                //m_have_output(env.get_attribute_string(itemnode, "name", "")[0])
                //m_input_tag(env.get_attribute_string(itemnode, "inputtag", ""))
                m_input_port = null;
                m_input_mask = 0;
                m_screen = null;
                m_orientation = (int)emucore_global.ROT0;

                //throw new emu_unimplemented();
#if false
                // allocate a copy of the output name
                m_output_name = xml_get_attribute_string_with_subst(machine, itemnode, "name", "");

                // allocate a copy of the input tag
                m_input_tag = xml_get_attribute_string_with_subst(machine, itemnode, "inputtag", "");

                // find the associated element
                const char *name = xml_get_attribute_string_with_subst(machine, itemnode, "element", NULL);
                if (name != NULL)
                {
                    // search the list of elements for a match
                    for (m_element = elemlist.first(); m_element != NULL; m_element = m_element->next())
                        if (strcmp(name, m_element->name()) == 0)
                            break;

                    // error if not found
                    if (m_element == NULL)
                        throw emu_fatalerror("Unable to find layout element %s", name);
                }

                // fetch common data
                int index = xml_get_attribute_int_with_subst(machine, itemnode, "index", -1);
                if (index != -1)
                {
                    screen_device_iterator iter(machine.root_device());
                    m_screen = iter.byindex(index);
                }
                m_input_mask = xml_get_attribute_int_with_subst(machine, itemnode, "inputmask", 0);
                if (m_output_name[0] != 0 && m_element != NULL)
                    output_set_value(m_output_name, m_element->default_state());
                parse_bounds(machine, xml_get_sibling(itemnode.child, "bounds"), m_rawbounds);
                parse_color(machine, xml_get_sibling(itemnode.child, "color"), m_color);
                parse_orientation(machine, xml_get_sibling(itemnode.child, "orientation"), m_orientation);

                // sanity checks
                if (strcmp(itemnode.name, "screen") == 0)
                {
                    if (m_screen == NULL)
                        throw emu_fatalerror("Layout references invalid screen index %d", index);
                }
                else
                {
                    if (m_element == NULL)
                        throw emu_fatalerror("Layout item of type %s require an element tag", itemnode.name);
                }

                if (has_input())
                {
                    m_input_port = m_element->machine().root_device().ioport(m_input_tag.c_str());
                }
#endif

                // HACK instead of xml parsing
                //int index = xml_get_attribute_int_with_subst(machine, itemnode, "index", -1);
                int index = 0;
                screen_device_iterator iter = new screen_device_iterator(env.machine().root_device());
                m_screen = iter.byindex(index);
                //parse_bounds(machine, xml_get_sibling(itemnode.child, "bounds"), m_rawbounds);
                m_rawbounds.x0 = 0;  //xml_get_attribute_float_with_subst(machine, *boundsnode, "left", 0.0f);
                m_rawbounds.x1 = 3;  //xml_get_attribute_float_with_subst(machine, *boundsnode, "right", 1.0f);
                m_rawbounds.y0 = 0;  //xml_get_attribute_float_with_subst(machine, *boundsnode, "top", 0.0f);
                m_rawbounds.y1 = 4;  //xml_get_attribute_float_with_subst(machine, *boundsnode, "bottom", 1.0f);
                //parse_color(machine, xml_get_sibling(itemnode.child, "color"), m_color);
                m_color.r = m_color.g = m_color.b = m_color.a = 1.0f;
                //parse_orientation(machine, xml_get_sibling(itemnode.child, "orientation"), m_orientation);
                m_orientation = (int)emucore_global.ROT0;

                if (has_input())
                {
                    m_input_port = m_element.machine().root_device().ioport(m_input_tag);
                }
            }


            // fetch state based on configured source
            //-------------------------------------------------
            //  state - fetch state based on configured source
            //-------------------------------------------------
            public int state()
            {
                global.assert(m_element != null);

                if (m_have_output)
                {
                    throw new emu_unimplemented();
#if false
                    // if configured to track an output, fetch its value
                    return m_output;
#endif
                }
                else if (!m_input_tag.empty())
                {
                    // if configured to an input, fetch the input value
                    if (m_input_port != null)
                    {
                        ioport_field field = m_input_port.field(m_input_mask);
                        if (field != null)
                            return ((m_input_port.read() ^ field.defvalue()) & m_input_mask) != 0 ? 1 : 0;
                    }
                }

                return 0;
            }


            // resolve tags, if any
            //---------------------------------------------
            //  resolve_tags - resolve tags, if any are set
            //---------------------------------------------
            public void resolve_tags()
            {
                if (has_input())
                {
                    m_input_port = m_element.machine().root_device().ioport(m_input_tag);
                }
            }
        }


        // construction/destruction
        //-------------------------------------------------
        //  layout_view - constructor
        //-------------------------------------------------
        public layout_view(
                environment env,
                util.xml.data_node viewnode,
                int nodeIdx_HACK,
                Dictionary<string, layout_element> elemmap,  //element_map elemmap,
                Dictionary<string, layout_group> groupmap)  //group_map groupmap)
        {
            //m_name(make_name(env, viewnode))
            m_aspect = 1.0f;
            m_scraspect = 1.0f;


            // HACK instead of xml parsing
            //throw new emu_unimplemented();
#if false
            <?xml version=\"1.0\"?>
            <mamelayout version=\"2\">
                <view name=\"Standard (4:3)\">
                    <screen index=\"0\">
                        <bounds left=\"0\" top=\"0\" right=\"4\" bottom=\"3\" />
                    </screen>
                </view>
                <view name=\"Pixel Aspect (~scr0nativexaspect~:~scr0nativeyaspect~)\">
                    <screen index=\"0\">
                        <bounds left=\"0\" top=\"0\" right=\"~scr0width~\" bottom=\"~scr0height~\" />
                    </screen>
                </view>
                <view name=\"Cocktail\">
                    <screen index=\"0\">
                        <bounds x=\"0\" y=\"-3.03\" width=\"4\" height=\"3\" />
                        <orientation rotate=\"180\" />
                    </screen>
                    <screen index=\"0\">
                        <bounds x=\"0\" y=\"0\" width=\"4\" height=\"3\" />
                    </screen>
                </view>
            </mamelayout>
#endif

            if (nodeIdx_HACK == 1)
            {
                m_name = "Standard (3:4)";
                m_expbounds = new render_bounds();
                //parse_bounds(machine, xml_get_sibling(boundsnode, "bounds"), m_expbounds);
                m_expbounds.x0 = 0;  //xml_get_attribute_float_with_subst(machine, *boundsnode, "left", 0.0f);
                m_expbounds.x1 = 3;  //xml_get_attribute_float_with_subst(machine, *boundsnode, "right", 1.0f);
                m_expbounds.y0 = 0;  //xml_get_attribute_float_with_subst(machine, *boundsnode, "top", 0.0f);
                m_expbounds.y1 = 4;  //xml_get_attribute_float_with_subst(machine, *boundsnode, "bottom", 1.0f);
                //m_backdrop_list.append(new item(machine, *itemnode, elemlist));
                m_screen_list.push_back(new item(env, new util.xml.data_node(), elemmap, m_expbounds));
                //m_overlay_list.append(new item(machine, *itemnode, elemlist));
                //m_bezel_list.append(new item(machine, *itemnode, elemlist));
                //m_cpanel_list.append(new item(machine, *itemnode, elemlist));
                //m_marquee_list.append(new item(machine, *itemnode, elemlist));
            }

            // recompute the data for the view based on a default layer config
            recompute(new render_layer_config());

            //throw new emu_unimplemented();
#if false
            // allocate a copy of the name
            m_name = xml_get_attribute_string_with_subst(machine, viewnode, "name", "");

            // if we have a bounds item, load it
            xml_data_node *boundsnode = xml_get_sibling(viewnode.child, "bounds");
            m_expbounds.x0 = m_expbounds.y0 = m_expbounds.x1 = m_expbounds.y1 = 0;
            if (boundsnode != NULL)
                parse_bounds(machine, xml_get_sibling(boundsnode, "bounds"), m_expbounds);

            // load backdrop items
            for (xml_data_node *itemnode = xml_get_sibling(viewnode.child, "backdrop"); itemnode != NULL; itemnode = xml_get_sibling(itemnode->next, "backdrop"))
                m_backdrop_list.append(*global_alloc(item(machine, *itemnode, elemlist)));

            // load screen items
            for (xml_data_node *itemnode = xml_get_sibling(viewnode.child, "screen"); itemnode != NULL; itemnode = xml_get_sibling(itemnode->next, "screen"))
                m_screen_list.append(*global_alloc(item(machine, *itemnode, elemlist)));

            // load overlay items
            for (xml_data_node *itemnode = xml_get_sibling(viewnode.child, "overlay"); itemnode != NULL; itemnode = xml_get_sibling(itemnode->next, "overlay"))
                m_overlay_list.append(*global_alloc(item(machine, *itemnode, elemlist)));

            // load bezel items
            for (xml_data_node *itemnode = xml_get_sibling(viewnode.child, "bezel"); itemnode != NULL; itemnode = xml_get_sibling(itemnode->next, "bezel"))
                m_bezel_list.append(*global_alloc(item(machine, *itemnode, elemlist)));

            // load cpanel items
            for (xml_data_node *itemnode = xml_get_sibling(viewnode.child, "cpanel"); itemnode != NULL; itemnode = xml_get_sibling(itemnode->next, "cpanel"))
                m_cpanel_list.append(*global_alloc(item(machine, *itemnode, elemlist)));

            // load marquee items
            for (xml_data_node *itemnode = xml_get_sibling(viewnode.child, "marquee"); itemnode != NULL; itemnode = xml_get_sibling(itemnode->next, "marquee"))
                m_marquee_list.append(*global_alloc(item(machine, *itemnode, elemlist)));

            // recompute the data for the view based on a default layer config
            recompute(render_layer_config());
#endif
        }


        //-------------------------------------------------
        //  items - return the appropriate list
        //-------------------------------------------------
        public item_list items(item_layer layer)
        {
            switch (layer)
            {
                case item_layer.ITEM_LAYER_BACKDROP:   return m_backdrop_list;
                case item_layer.ITEM_LAYER_SCREEN:     return m_screen_list;
                case item_layer.ITEM_LAYER_OVERLAY:    return m_overlay_list;
                case item_layer.ITEM_LAYER_BEZEL:      return m_bezel_list;
                case item_layer.ITEM_LAYER_CPANEL:     return m_cpanel_list;
                case item_layer.ITEM_LAYER_MARQUEE:    return m_marquee_list;
                default:                               throw new emu_exception();  // false; // calling this with an invalid layer is bad, m'kay?
            }
        }


        // operations
        //-------------------------------------------------
        //  recompute - recompute the bounds and aspect
        //  ratio of a view and all of its contained items
        //-------------------------------------------------
        public void recompute(render_layer_config layerconfig)
        {
            // reset the bounds
            m_bounds.x0 = 0.0f;
            m_bounds.y0 = 0.0f;
            m_bounds.x1 = 0.0f;
            m_bounds.y1 = 0.0f;
            m_scrbounds.x0 = 0.0f;
            m_scrbounds.y0 = 0.0f;
            m_scrbounds.x1 = 0.0f;
            m_scrbounds.y1 = 0.0f;
            m_screens.reset();

            // loop over all layers
            bool first = true;
            bool scrfirst = true;
            for (item_layer layer = item_layer.ITEM_LAYER_FIRST; layer < item_layer.ITEM_LAYER_MAX; layer++)
            {
                // determine if this layer should be visible
                switch (layer)
                {
                    case item_layer.ITEM_LAYER_BACKDROP:   m_layenabled[(int)layer] = layerconfig.backdrops_enabled();  break;
                    case item_layer.ITEM_LAYER_OVERLAY:    m_layenabled[(int)layer] = layerconfig.overlays_enabled();   break;
                    case item_layer.ITEM_LAYER_BEZEL:      m_layenabled[(int)layer] = layerconfig.bezels_enabled();     break;
                    case item_layer.ITEM_LAYER_CPANEL:     m_layenabled[(int)layer] = layerconfig.cpanels_enabled();    break;
                    case item_layer.ITEM_LAYER_MARQUEE:    m_layenabled[(int)layer] = layerconfig.marquees_enabled();   break;
                    default:                               m_layenabled[(int)layer] = true;                             break;
                }

                // only do it if requested
                if (m_layenabled[(int)layer])
                {
                    foreach (item curitem in items(layer))
                    {
                        // accumulate bounds
                        if (first)
                            m_bounds = new render_bounds(curitem.rawbounds());
                        else
                            rendutil_global.union_render_bounds(m_bounds, curitem.rawbounds());
                        first = false;

                        // accumulate screen bounds
                        if (curitem.screen() != null)
                        {
                            if (scrfirst)
                                m_scrbounds = new render_bounds(curitem.rawbounds());
                            else
                                rendutil_global.union_render_bounds(m_scrbounds, curitem.rawbounds());
                            scrfirst = false;

                            // accumulate the screens in use while we're scanning
                            m_screens.add(curitem.screen());
                        }
                    }
                }
            }

            // if we have an explicit bounds, override it
            if (m_expbounds.x1 > m_expbounds.x0)
                m_bounds = m_expbounds;

            // if we're handling things normally, the target bounds are (0,0)-(1,1)
            render_bounds target_bounds = new render_bounds();
            if (!layerconfig.zoom_to_screen() || m_screens.count() == 0)
            {
                // compute the aspect ratio of the view
                m_aspect = (m_bounds.x1 - m_bounds.x0) / (m_bounds.y1 - m_bounds.y0);

                target_bounds.x0 = target_bounds.y0 = 0.0f;
                target_bounds.x1 = target_bounds.y1 = 1.0f;
            }

            // if we're cropping, we want the screen area to fill (0,0)-(1,1)
            else
            {
                // compute the aspect ratio of the screen
                m_scraspect = (m_scrbounds.x1 - m_scrbounds.x0) / (m_scrbounds.y1 - m_scrbounds.y0);

                float targwidth = (m_bounds.x1 - m_bounds.x0) / (m_scrbounds.x1 - m_scrbounds.x0);
                float targheight = (m_bounds.y1 - m_bounds.y0) / (m_scrbounds.y1 - m_scrbounds.y0);
                target_bounds.x0 = (m_bounds.x0 - m_scrbounds.x0) / (m_bounds.x1 - m_bounds.x0) * targwidth;
                target_bounds.y0 = (m_bounds.y0 - m_scrbounds.y0) / (m_bounds.y1 - m_bounds.y0) * targheight;
                target_bounds.x1 = target_bounds.x0 + targwidth;
                target_bounds.y1 = target_bounds.y0 + targheight;
            }

            // determine the scale/offset for normalization
            float xoffs = m_bounds.x0;
            float yoffs = m_bounds.y0;
            float xscale = (target_bounds.x1 - target_bounds.x0) / (m_bounds.x1 - m_bounds.x0);
            float yscale = (target_bounds.y1 - target_bounds.y0) / (m_bounds.y1 - m_bounds.y0);

            // normalize all the item bounds
            for (item_layer layer = item_layer.ITEM_LAYER_FIRST; layer < item_layer.ITEM_LAYER_MAX; layer++)
            {
                foreach (item curitem in items(layer))
                {
                    curitem.bounds().x0 = target_bounds.x0 + (curitem.rawbounds().x0 - xoffs) * xscale;
                    curitem.bounds().x1 = target_bounds.x0 + (curitem.rawbounds().x1 - xoffs) * xscale;
                    curitem.bounds().y0 = target_bounds.y0 + (curitem.rawbounds().y0 - yoffs) * yscale;
                    curitem.bounds().y1 = target_bounds.y0 + (curitem.rawbounds().y1 - yoffs) * yscale;
                }
            }
        }


        // resolve tags, if any
        //-----------------------------
        //  resolve_tags - resolve tags
        //-----------------------------
        public void resolve_tags()
        {
            for (item_layer layer = item_layer.ITEM_LAYER_FIRST; layer < item_layer.ITEM_LAYER_MAX; ++layer)
            {
                foreach (item curitem in items(layer))
                {
                    curitem.resolve_tags();
                }
            }
        }
    }


    partial class layout_file
    {
        // construction/destruction
        //-------------------------------------------------
        //  layout_file - constructor
        //-------------------------------------------------
        public layout_file(device_t device, util.xml.data_node rootnode, string dirname)
        {
            environment env = new environment(device);

            m_elemmap = new Dictionary<string, layout_element>();
            m_viewlist = new view_list();


            // HACK instead of xml parsing
            //throw new emu_unimplemented();
#if false
            <?xml version=\"1.0\"?>
            <mamelayout version=\"2\">
                <view name=\"Standard (4:3)\">
                    <screen index=\"0\">
                        <bounds left=\"0\" top=\"0\" right=\"4\" bottom=\"3\" />
                    </screen>
                </view>
                <view name=\"Pixel Aspect (~scr0nativexaspect~:~scr0nativeyaspect~)\">
                    <screen index=\"0\">
                        <bounds left=\"0\" top=\"0\" right=\"~scr0width~\" bottom=\"~scr0height~\" />
                    </screen>
                </view>
                <view name=\"Cocktail\">
                    <screen index=\"0\">
                        <bounds x=\"0\" y=\"-3.03\" width=\"4\" height=\"3\" />
                        <orientation rotate=\"180\" />
                    </screen>
                    <screen index=\"0\">
                        <bounds x=\"0\" y=\"0\" width=\"4\" height=\"3\" />
                    </screen>
                </view>
            </mamelayout>
#endif

            //m_elemlist.append(*global_alloc(layout_element(machine, *elemnode, dirname)));
            m_viewlist.push_back(new layout_view(env, rootnode, 1, m_elemmap, new Dictionary<string, layout_group>()));
            //m_viewlist.append(new layout_view(machine, /*viewnode,*/ 2, m_elemlist));
            //m_viewlist.append(new layout_view(machine, /*viewnode,*/ 3, m_elemlist));


            //throw new emu_unimplemented();
#if false
            : m_next(NULL)


            // find the layout node
            xml_data_node *mamelayoutnode = xml_get_sibling(rootnode.child, "mamelayout");
            if (mamelayoutnode == NULL)
                throw emu_fatalerror("Invalid XML file: missing mamelayout node");

            // validate the config data version
            int version = xml_get_attribute_int(mamelayoutnode, "version", 0);
            if (version != LAYOUT_VERSION)
                throw emu_fatalerror("Invalid XML file: unsupported version");

            // parse all the elements
            for (xml_data_node *elemnode = xml_get_sibling(mamelayoutnode->child, "element"); elemnode != NULL; elemnode = xml_get_sibling(elemnode->next, "element"))
                m_elemlist.append(*global_alloc(layout_element(machine, *elemnode, dirname)));

            // parse all the views
            for (xml_data_node *viewnode = xml_get_sibling(mamelayoutnode->child, "view"); viewnode != NULL; viewnode = xml_get_sibling(viewnode->next, "view"))
                m_viewlist.append(*global_alloc(layout_view(machine, *viewnode, m_elemlist)));
#endif
        }
    }
}
