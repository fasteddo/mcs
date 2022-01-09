// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using char32_t = System.UInt32;
using size_t = System.UInt64;
using u16 = System.UInt16;

using static mame.cpp_global;
using static mame.emucore_global;
using static mame.render_global;
using static mame.rendertypes_global;
using static mame.unicode_global;


namespace mame.ui
{
    public class text_layout
    {
        // justification options for text
        public enum text_justify
        {
            LEFT = 0,
            CENTER,
            RIGHT
        }


        // word wrapping options
        public enum word_wrapping
        {
            NEVER,
            TRUNCATE,
            WORD
        }


        // text style information - in a struct to facilitate copying
        struct char_style
        {
            public rgb_t fgcolor;
            public rgb_t bgcolor;
            public float size;
        }


        // information about the "source" of a character - also in a struct
        // to facilitate copying
        struct source_info
        {
            public size_t start;
            public size_t span;
        }


        // this should really be "positioned glyph" as glyphs != characters, but
        // we'll get there eventually
        struct positioned_char
        {
            public char32_t character;
            public char_style style;
            public source_info source;
            public float xoffset;
            public float xwidth;
        }


        // class to represent a line
        class line
        {
            //using size_type = size_t;
            //static constexpr size_type npos = ~size_type(0);


            std.vector<positioned_char> m_characters = new std.vector<positioned_char>();
            size_t m_center_justify_start = npos;
            size_t m_right_justify_start = npos;
            float m_yoffset;
            float m_height;
            float m_width = 0.0f;
            float m_anchor_pos = 0.0f;
            float m_anchor_target = 0.0f;


            public line(float yoffset, float height)
            {
                m_yoffset = yoffset;
                m_height = height;
            }


            // methods

            //-------------------------------------------------
            //  line::add_character
            //-------------------------------------------------
            public void add_character(text_layout layout, char32_t ch, char_style style, source_info source)
            {
                // get the width of this character
                float chwidth = layout.get_char_width(ch, style.size);

                // append the positioned character
                m_characters.emplace_back(new positioned_char() { character = ch, style = style, source = source, xoffset = m_width, xwidth = chwidth });
                m_width += chwidth;

                // we might be bigger
                m_height = std.max(m_height, style.size * layout.yscale());
            }


            //-------------------------------------------------
            //  line::truncate
            //-------------------------------------------------
            public void truncate(size_t position)
            {
                assert(position <= m_characters.size());

                // are we actually truncating?
                if (position < m_characters.size())
                {
                    // set the width as appropriate
                    m_width = m_characters[position].xoffset;

                    // and resize the array
                    m_characters.resize(position);
                }
            }


            public void set_justification(text_justify justify)
            {
                switch (justify)
                {
                case text_justify.RIGHT:
                    if (npos == m_right_justify_start)
                        m_right_justify_start = m_characters.size();
                    goto case text_justify.CENTER;  //[[fallthrough]];
                case text_justify.CENTER:
                    if (npos == m_center_justify_start)
                        m_center_justify_start = m_characters.size();
                    break;
                case text_justify.LEFT:
                    break;
                }
            }


            public void align_text(text_layout layout)
            {
                assert(m_right_justify_start >= m_center_justify_start);

                if (m_characters.empty() || m_center_justify_start != 0)
                {
                    // at least some of the text is left-justified - anchor to left
                    m_anchor_pos = 0.0f;
                    m_anchor_target = 0.0f;
                    if ((layout.width() > m_width) && (m_characters.size() > m_center_justify_start))
                    {
                        // at least some text is not left-justified
                        if (m_right_justify_start == m_center_justify_start)
                        {
                            // all text that isn't left-justified is right-justified
                            float right_offset = layout.width() - m_width;
                            for (size_t i = m_right_justify_start; m_characters.size() > i; ++i)
                                m_characters[i] = new positioned_char() { character = m_characters[i].character, style = m_characters[i].style, source = m_characters[i].source, xoffset = m_characters[i].xoffset + right_offset, xwidth = m_characters[i].xwidth };  //m_characters[i].xoffset += right_offset;

                            m_width = layout.width();
                        }
                        else if (m_characters.size() <= m_right_justify_start)
                        {
                            // all text that isn't left-justified is center-justified
                            float center_width = m_width - m_characters[m_center_justify_start].xoffset;
                            float center_offset = ((layout.width() - center_width) * 0.5f) - m_characters[m_center_justify_start].xoffset;
                            if (0.0f < center_offset)
                            {
                                for (size_t i = m_center_justify_start; m_characters.size() > i; ++i)
                                    m_characters[i] = new positioned_char() { character = m_characters[i].character, style = m_characters[i].style, source = m_characters[i].source, xoffset = m_characters[i].xoffset + center_offset, xwidth = m_characters[i].xwidth };  //m_characters[i].xoffset += center_offset;

                                m_width += center_offset;
                            }
                        }
                        else
                        {
                            // left, right and center-justified parts
                            float center_width = m_characters[m_right_justify_start].xoffset - m_characters[m_center_justify_start].xoffset;
                            float center_offset = ((layout.width() - center_width) * 0.5f) - m_characters[m_center_justify_start].xoffset;
                            float right_offset = layout.width() - m_width;
                            if (center_offset > right_offset)
                            {
                                // right-justified text pushes centre-justified text to the left
                                for (size_t i = m_center_justify_start; m_right_justify_start > i; ++i)
                                    m_characters[i] = new positioned_char() { character = m_characters[i].character, style = m_characters[i].style, source = m_characters[i].source, xoffset = m_characters[i].xoffset + right_offset, xwidth = m_characters[i].xwidth };  //m_characters[i].xoffset += right_offset;
                            }
                            else if (0.0f < center_offset)
                            {
                                // left-justified text doesn't push centre-justified text to the right
                                for (size_t i = m_center_justify_start; m_right_justify_start > i; ++i)
                                    m_characters[i] = new positioned_char() { character = m_characters[i].character, style = m_characters[i].style, source = m_characters[i].source, xoffset = m_characters[i].xoffset + center_offset, xwidth = m_characters[i].xwidth };  //m_characters[i].xoffset += center_offset;
                            }

                            for (size_t i = m_right_justify_start; m_characters.size() > i; ++i)
                                m_characters[i] = new positioned_char() { character = m_characters[i].character, style = m_characters[i].style, source = m_characters[i].source, xoffset = m_characters[i].xoffset + right_offset, xwidth = m_characters[i].xwidth };  //m_characters[i].xoffset += right_offset;

                            m_width = layout.width();
                        }
                    }
                }
                else if (m_characters.size() <= m_right_justify_start)
                {
                    // all text is center-justified - anchor to center
                    m_anchor_pos = 0.5f;
                    m_anchor_target = 0.5f;
                }
                else
                {
                    // at least some text is right-justified - anchor to right
                    m_anchor_pos = 1.0f;
                    m_anchor_target = 1.0f;
                    if ((layout.width() > m_width) && (m_right_justify_start > m_center_justify_start))
                    {
                        // mixed center-justified and right-justified text
                        float center_width = m_characters[m_right_justify_start].xoffset;
                        float center_offset = (layout.width() - m_width + (center_width * 0.5f)) - (layout.width() * 0.5f);
                        if (0.0f < center_offset)
                        {
                            for (size_t i = m_right_justify_start; m_characters.size() > i; ++i)
                                m_characters[i] = new positioned_char() { character = m_characters[i].character, style = m_characters[i].style, source = m_characters[i].source, xoffset = m_characters[i].xoffset + center_offset, xwidth = m_characters[i].xwidth };  //m_characters[i].xoffset += center_offset;

                            m_width += center_offset;
                        }
                    }
                }
            }


            // accessors
            public float xoffset(text_layout layout) { return (layout.width() * m_anchor_target) - (m_width * m_anchor_pos); }
            public float yoffset() { return m_yoffset; }
            public float width() { return m_width; }
            public float height() { return m_height; }
            public size_t character_count() { return m_characters.size(); }
            public size_t center_justify_start() { return m_center_justify_start; }
            public size_t right_justify_start() { return m_right_justify_start; }
            public positioned_char character(size_t index) { return m_characters[index]; }  //positioned_char &character(size_t index) const { return m_characters[index]; }
        }


        // instance variables
        render_font m_font;
        float m_xscale;
        float m_yscale;
        float m_width;
        float m_calculated_actual_width;
        text_justify m_justify;
        word_wrapping m_wrap;
        std.vector<line> m_lines = new std.vector<line>();
        line m_current_line;
        size_t m_last_break;
        size_t m_text_position;
        bool m_truncating;


        // ctor/dtor
        //-------------------------------------------------
        //  ctor
        //-------------------------------------------------
        public text_layout(render_font font, float xscale, float yscale, float width, text_justify justify, word_wrapping wrap)
        {
            m_font = font;
            m_xscale = xscale;
            m_yscale = yscale;
            m_width = width;
            m_justify = justify;
            m_wrap = wrap;
            m_current_line = null;
            m_last_break = 0;
            m_text_position = 0;
            m_truncating = false;


            invalidate_calculated_actual_width();
        }


        //-------------------------------------------------
        //  ctor (move)
        //-------------------------------------------------
        text_layout(text_layout that)
        {
            m_font = that.m_font;
            m_xscale = that.m_xscale;
            m_yscale = that.m_yscale;
            m_width = that.m_width;
            m_calculated_actual_width = that.m_calculated_actual_width;
            m_justify = that.m_justify;
            m_wrap = that.m_wrap;
            m_lines = new std.vector<line>(that.m_lines);
            m_current_line = that.m_current_line;
            m_last_break = that.m_last_break;
            m_text_position = that.m_text_position;
            m_truncating = false;


            that.invalidate_calculated_actual_width();
        }


        // accessors
        render_font font() { return m_font; }
        float xscale() { return m_xscale;  }
        float yscale() { return m_yscale; }
        public float width() { return m_width; }
        text_justify justify() { return m_justify; }
        word_wrapping wrap() { return m_wrap; }


        // methods

        //-------------------------------------------------
        //  actual_left
        //-------------------------------------------------
        public float actual_left()
        {
            if (m_current_line != null)
            {
                // TODO: is there a sane way to allow an open line to be temporarily finalised and rolled back?
                m_current_line.align_text(this);
                m_current_line = null;
            }

            if (empty()) // degenerate scenario
                return 0.0f;

            float result = 1.0f;
            foreach (var line in m_lines)
            {
                if (line.width() != 0)
                    result = std.min(result, line.xoffset(this));
            }
            return result;
        }


        //-------------------------------------------------
        //  actual_width
        //-------------------------------------------------
        public float actual_width()
        {
            if (m_current_line != null)
            {
                // TODO: is there a sane way to allow an open line to be temporarily finalised and rolled back?
                m_current_line.align_text(this);
                m_current_line = null;
            }

            // do we need to calculate the width?
            if (m_calculated_actual_width < 0)
            {
                // calculate the actual width
                m_calculated_actual_width = 0;
                foreach (var line in m_lines)
                    m_calculated_actual_width = std.max(m_calculated_actual_width, line.width());

            }

            // return it
            return m_calculated_actual_width;
        }


        //-------------------------------------------------
        //  actual_height
        //-------------------------------------------------
        public float actual_height()
        {
            if (!m_lines.empty())
                return m_lines.back().yoffset() + m_lines.back().height();
            else
                return 0.0f;
        }


        public bool empty() { return m_lines.empty(); }
        public size_t lines() { return m_lines.size(); }
        //bool hit_test(float x, float y, size_t &start, size_t &span);
        //void restyle(size_t start, size_t span, rgb_t *fgcolor, rgb_t *bgcolor);


        //-------------------------------------------------
        //  emit
        //-------------------------------------------------

        public void emit(render_container container, float x, float y)
        {
            emit(container, 0, m_lines.size(), x, y);
        }

        public void emit(render_container container, size_t start, size_t lines, float x, float y)
        {
            if (m_current_line != null)
            {
                // TODO: is there a sane way to allow an open line to be temporarily finalised and rolled back?
                m_current_line.align_text(this);
                m_current_line = null;
            }

            float base_y = (m_lines.size() > start) ? m_lines[start].yoffset() : 0.0f;
            for (size_t l = start; ((start + lines) > l) && (m_lines.size() > l); ++l)
            {
                var line = m_lines[l];
                float line_xoffset = line.xoffset(this);
                float char_y = y + line.yoffset() - base_y;
                float char_height = line.height();

                // emit every single character
                for (size_t i = 0; i < line.character_count(); i++)
                {
                    var ch = line.character(i);

                    // position this specific character correctly (TODO - this doesn't handle differently sized text (yet)
                    float char_x = x + line_xoffset + ch.xoffset;
                    float char_width = ch.xwidth;

                    // render the background of the character (if present)
                    if (ch.style.bgcolor.a() != 0)
                        container.add_rect(char_x, char_y, char_x + char_width, char_y + char_height, ch.style.bgcolor, PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA));

                    // render the foreground
                    container.add_char(
                            char_x,
                            char_y,
                            char_height,
                            xscale() / yscale(),
                            ch.style.fgcolor,
                            font(),
                            (u16)ch.character);
                }
            }
        }


        public void add_text(string text) { add_text(text, rgb_t.white()); }
        public void add_text(string text, rgb_t fgcolor) { add_text(text, fgcolor, rgb_t.transparent()); }
        public void add_text(string text, rgb_t fgcolor, rgb_t bgcolor, float size = 1.0f)  //void add_text(std::string_view text, rgb_t fgcolor = rgb_t::white(), rgb_t bgcolor = rgb_t::transparent(), float size = 1.0)
        {
            add_text(text, justify(), new char_style() { fgcolor = fgcolor, bgcolor = bgcolor, size = size });
        }
        public void add_text(string text, text_justify line_justify) { add_text(text, line_justify, rgb_t.white(), rgb_t.transparent()); }
        public void add_text(string text, text_justify line_justify, rgb_t fgcolor) { add_text(text, line_justify, fgcolor, rgb_t.transparent()); }
        public void add_text(string text, text_justify line_justify, rgb_t fgcolor, rgb_t bgcolor, float size = 1.0f)  //void add_text(std::string_view text, text_justify line_justify, rgb_t fgcolor = rgb_t::white(), rgb_t bgcolor = rgb_t::transparent(), float size = 1.0)
        {
            add_text(text, line_justify, new char_style() { fgcolor = fgcolor, bgcolor = bgcolor, size = size });
        }


        //-------------------------------------------------
        //  add_text
        //-------------------------------------------------
        void add_text(string text, text_justify line_justify, char_style style)
        {
            while (!text.empty())
            {
                // adding a character - we might change the width
                invalidate_calculated_actual_width();

                // do we need to create a new line?
                if (m_current_line == null)
                    start_new_line(style.size);

                m_current_line.set_justification(line_justify);

                // get the current character
                char ch;  //char32_t ch;
                int scharcount = uchar_from_utf8(out ch, text);
                if (scharcount < 0)
                    break;

                text = text.remove_prefix_((size_t)scharcount);

                // set up source information
                source_info source = new source_info() { start = 0, span = 0 };
                source.start = m_text_position;
                source.span = (size_t)scharcount;
                m_text_position += (size_t)scharcount;

                // is this an endline?
                if (ch == '\n')
                {
                    // close up the current line
                    m_current_line.align_text(this);
                    m_current_line = null;
                }
                else if (!m_truncating)
                {
                    // if we hit a space, remember the location and width *without* the space
                    bool is_space = is_space_character(ch);
                    if (is_space)
                        m_last_break = m_current_line.character_count();

                    // append the character
                    m_current_line.add_character(this, ch, style, source);

                    // do we have to wrap?
                    if ((wrap() != word_wrapping.NEVER) && (m_current_line.width() > m_width))
                    {
                        switch (wrap())
                        {
                        case word_wrapping.TRUNCATE:
                            truncate_wrap();
                            break;

                        case word_wrapping.WORD:
                            word_wrap();
                            break;

                        case word_wrapping.NEVER:
                            // can't happen due to if condition, but compile warns about it
                            break;
                        }
                    }
                    else
                    {
                        // we didn't wrap - if we hit any non-space breakable character,
                        // remember the location and width *with* the breakable character
                        if (!is_space && is_breakable_char(ch))
                            m_last_break = m_current_line.character_count();
                    }
                }
            }
        }


        //-------------------------------------------------
        //  start_new_line
        //-------------------------------------------------
        void start_new_line(float height)
        {
            // update the current line
            m_current_line = new line(actual_height(), height * yscale());  //m_current_line = m_lines.emplace_back(std::make_unique<line>(actual_height(), height * yscale())).get();
            m_lines.emplace_back(m_current_line);
            m_last_break = 0;
            m_truncating = false;
        }


        //-------------------------------------------------
        //  get_char_width
        //-------------------------------------------------
        float get_char_width(char32_t ch, float size)
        {
            return font().char_width(size * yscale(), xscale() / yscale(), ch);
        }


        //-------------------------------------------------
        //  truncate_wrap
        //-------------------------------------------------
        void truncate_wrap()
        {
            char32_t elipsis = 0x2026;

            // for now, lets assume that we're only truncating the last character
            size_t truncate_position = m_current_line.character_count() - 1;
            var truncate_char = m_current_line.character(truncate_position);

            // copy style information
            char_style style = truncate_char.style;

            // copy source information
            source_info source;
            source.start = truncate_char.source.start + truncate_char.source.span;
            source.span = 0;

            // figure out how wide an ellipsis is
            float elipsis_width = get_char_width(elipsis, style.size);

            // where should we really truncate from?
            while (truncate_position > 0 && m_current_line.character(truncate_position).xoffset + elipsis_width > width())
                truncate_position--;

            // truncate!!!
            m_current_line.truncate(truncate_position);

            // and append the ellipsis
            m_current_line.add_character(this, elipsis, style, source);

            // take note that we are truncating; suppress new characters
            m_truncating = true;
        }


        //-------------------------------------------------
        //  word_wrap
        //-------------------------------------------------
        void word_wrap()
        {
            // keep track of the last line and break
            line last_line = m_current_line;
            size_t last_break = m_last_break != 0 ? m_last_break : (last_line.character_count() - 1);

            // start a new line with the same justification
            start_new_line(last_line.character(last_line.character_count() - 1).style.size);

            // find the beginning of the word to wrap
            size_t position = last_break;
            while ((last_line.character_count() > position) && is_space_character(last_line.character(position).character))
                position++;

            // carry over justification
            if (last_line.right_justify_start() <= position)
                m_current_line.set_justification(text_justify.RIGHT);
            else if (last_line.center_justify_start() <= position)
                m_current_line.set_justification(text_justify.CENTER);

            // transcribe the characters
            for (size_t i = position; i < last_line.character_count(); i++)
            {
                if (last_line.right_justify_start() == i)
                    m_current_line.set_justification(text_justify.RIGHT);
                else if (last_line.center_justify_start() == i)
                    m_current_line.set_justification(text_justify.CENTER);

                var ch = last_line.character(i);
                m_current_line.add_character(this, ch.character, ch.style, ch.source);
            }

            // and finally, truncate the previous line and adjust spacing
            last_line.truncate(last_break);
            last_line.align_text(this);
        }


        //-------------------------------------------------
        //  invalidate_calculated_actual_width
        //-------------------------------------------------
        void invalidate_calculated_actual_width()
        {
            m_calculated_actual_width = -1;
        }


        //-------------------------------------------------
        //  is_space_character
        //-------------------------------------------------
        bool is_space_character(char32_t ch)
        {
            return ch == ' ';
        }


        //-------------------------------------------------
        //  is_breakable_char - is a given unicode
        //  character a possible line break?
        //-------------------------------------------------
        bool is_breakable_char(char32_t ch)
        {
            // regular spaces and hyphens are breakable
            if (is_space_character(ch) || ch == '-')
                return true;

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
                return true;

            // Hangul Syllables (AC00-D7AF) are breakable
            if (ch >= 0xac00 && ch <= 0xd7af)
                return true;

            // CJK Compatibility Ideographs (F900-FAFF) are breakable
            if (ch >= 0xf900 && ch <= 0xfaff)
                return true;

            return false;
        }
    }
}
