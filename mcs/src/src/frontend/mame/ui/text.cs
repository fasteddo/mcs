// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using char32_t = System.UInt32;


namespace mame.ui
{
    public class text_layout : global_object
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
            public UInt32 start;
            public UInt32 span;
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
            std.vector<positioned_char> m_characters = new std.vector<positioned_char>();
            text_layout m_layout;
            text_justify m_justify;
            float m_yoffset;
            float m_width;
            float m_height;


            //-------------------------------------------------
            //  line::ctor
            //-------------------------------------------------
            public line(text_layout layout, text_justify justify, float yoffset, float height)
            {
                m_layout = layout;
                m_justify = justify;
                m_yoffset = yoffset;
                m_width = 0;
                m_height = height;
            }


            // methods

            //-------------------------------------------------
            //  line::add_character
            //-------------------------------------------------
            public void add_character(char32_t ch, char_style style, source_info source)
            {
                // get the width of this character
                float chwidth = m_layout.get_char_width(ch, style.size);

                // create the positioned character
                positioned_char positioned_char;
                positioned_char.character = ch;
                positioned_char.xoffset = m_width;
                positioned_char.xwidth = chwidth;
                positioned_char.style = style;
                positioned_char.source = source;

                // append the character
                m_characters.push_back(positioned_char);
                m_width += chwidth;

                // we might be bigger
                m_height = Math.Max(m_height, style.size * m_layout.yscale());
            }


            //-------------------------------------------------
            //  line::truncate
            //-------------------------------------------------
            public void truncate(UInt32 position)
            {
                assert(position <= m_characters.size());

                // are we actually truncating?
                if (position < m_characters.size())
                {
                    // set the width as appropriate
                    m_width = m_characters[(int)position].xoffset;

                    // and resize the array
                    m_characters.resize((int)position);
                }
            }


            // accessors

            //-------------------------------------------------
            //  line::xoffset
            //-------------------------------------------------
            public float xoffset()
            {
                float result;
                switch (justify())
                {
                    case text_justify.LEFT:
                    default:
                        result = 0;
                        break;
                    case text_justify.CENTER:
                        result = (m_layout.width() - width()) / 2;
                        break;
                    case text_justify.RIGHT:
                        result = m_layout.width() - width();
                        break;
                }
                return result;
            }

            public float yoffset() { return m_yoffset; }
            public float width() { return m_width; }
            public float height() { return m_height; }
            public text_justify justify() { return m_justify; }
            public UInt32 character_count() { return (UInt32)m_characters.size(); }
            public positioned_char character(UInt32 index) { return m_characters[(int)index]; }
            //positioned_char &character(size_t index) { return m_characters[index]; }
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
        UInt32 m_last_break;
        UInt32 m_text_position;
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
        float width() { return m_width; }
        text_justify justify() { return m_justify; }
        word_wrapping wrap() { return m_wrap; }


        // methods

        //-------------------------------------------------
        //  actual_left
        //-------------------------------------------------
        public float actual_left()
        {
            float result;

            if (empty())
            {
                // degenerate scenario
                result = 0;
            }
            else
            {
                result = 1.0f;
                foreach (var line in m_lines)
                {
                    result = Math.Min(result, line.xoffset());

                    // take an opportunity to break out easily
                    if (result <= 0)
                        break;
                }
            }

            return result;
        }


        //-------------------------------------------------
        //  actual_width
        //-------------------------------------------------
        public float actual_width()
        {
            // do we need to calculate the width?
            if (m_calculated_actual_width < 0)
            {
                // calculate the actual width
                m_calculated_actual_width = 0;
                foreach (var line in m_lines)
                    m_calculated_actual_width = Math.Max(m_calculated_actual_width, line.width());

            }

            // return it
            return m_calculated_actual_width;
        }


        //-------------------------------------------------
        //  actual_height
        //-------------------------------------------------
        public float actual_height()
        {
            line last_line = (m_lines.size() > 0)
                ? m_lines[m_lines.size() - 1]
                : null;
            return last_line != null
                ? last_line.yoffset() + last_line.height()
                : 0;
        }


        public bool empty() { return m_lines.size() == 0; }
        //bool hit_test(float x, float y, size_t &start, size_t &span) const;
        //void restyle(size_t start, size_t span, rgb_t *fgcolor, rgb_t *bgcolor);


        //-------------------------------------------------
        //  get_wrap_info
        //-------------------------------------------------
        public int get_wrap_info(out std.vector<int> xstart, out std.vector<int> xend)
        {
            xstart = new std.vector<int>();
            xend = new std.vector<int>();

            // this is a hacky method (tailored to the need to implement
            // mame_ui_manager::wrap_text) but so be it
            int line_count = 0;
            foreach (var line in m_lines)
            {
                int start_pos = 0;
                int end_pos = 0;

                var line_character_count = line.character_count();
                if (line_character_count > 0)
                {
                    start_pos = (int)line.character(0).source.start;
                    end_pos = (int)(line.character(line_character_count - 1).source.start
                        + line.character(line_character_count - 1).source.span);
                }

                line_count++;
                xstart.push_back(start_pos);
                xend.push_back(end_pos);
            }

            return line_count;
        }


        //-------------------------------------------------
        //  emit
        //-------------------------------------------------
        public void emit(render_container container, float x, float y)
        {
            foreach (var line in m_lines)
            {
                float line_xoffset = line.xoffset();

                // emit every single character
                for (UInt32 i = 0; i < line.character_count(); i++)
                {
                    var ch = line.character(i);

                    // position this specific character correctly (TODO - this doesn't
                    // handle differently sized text (yet)
                    float char_x = x + line_xoffset + ch.xoffset;
                    float char_y = y + line.yoffset();
                    float char_width = ch.xwidth;
                    float char_height = line.height();

                    // render the background of the character (if present)
                    if (ch.style.bgcolor.a() != 0)
                        container.add_rect(char_x, char_y, char_x + char_width, char_y + char_height, ch.style.bgcolor, PRIMFLAG_BLENDMODE(rendertypes_global.BLENDMODE_ALPHA));

                    // render the foreground
                    container.add_char(
                        char_x,
                        char_y,
                        char_height,
                        xscale() / yscale(),
                        ch.style.fgcolor,
                        font(),
                        (UInt16)ch.character);
                }
            }
        }


        public void add_text(string text, rgb_t fgcolor = null, rgb_t bgcolor = null, float size = 1.0f)  // fgcolor = rgb_t.white, rgb_t bgcolor = rgb_t.transparent
        {
            if (fgcolor == null)
                fgcolor = rgb_t.white();
            if (bgcolor == null)
                bgcolor = rgb_t.transparent();


            // create the style
            char_style style;
            style.fgcolor = fgcolor;
            style.bgcolor = bgcolor;
            style.size = size;

            // and add the text
            add_text(text, style);
        }


        // methods

        //-------------------------------------------------
        //  add_text
        //-------------------------------------------------
        void add_text(string text, char_style style)
        {
            while (!text.empty())
            {
                // adding a character - we might change the width
                invalidate_calculated_actual_width();

                // do we need to create a new line?
                if (m_current_line == null)
                {
                    // get the current character
                    char schar;  //char32_t schar;
                    int scharcount = unicode_global.uchar_from_utf8(out schar, text);  //int const scharcount = uchar_from_utf8(&schar, text);
                    if (scharcount < 0)
                        break;

                    // if the line starts with a tab character, center it regardless
                    text_justify line_justify = justify();
                    if (schar == '\t')
                    {
                        text = text.Substring(scharcount);  //text.remove_prefix(scharcount);
                        line_justify = text_justify.CENTER;
                    }

                    // start a new line
                    start_new_line(line_justify, style.size);
                }

                {
                    // get the current character
                    char ch;  //char32_t ch;
                    int scharcount = unicode_global.uchar_from_utf8(out ch, text);  //int const scharcount = uchar_from_utf8(&ch, text);
                    if (scharcount < 0)
                        break;
                    text = text.Substring(scharcount);  //text.remove_prefix(scharcount);

                    // set up source information
                    source_info source;
                    source.start = m_text_position;
                    source.span = (UInt32)scharcount;
                    m_text_position += (UInt32)scharcount;

                    // is this an endline?
                    if (ch == '\n')
                    {
                        // first, start a line if we have not already
                        if (m_current_line == null)
                            start_new_line(text_justify.LEFT, style.size);

                        // and then close up the current line
                        m_current_line = null;
                    }
                    else if (!m_truncating)
                    {
                        // if we hit a space, remember the location and width *without* the space
                        if (is_space_character(ch))
                            m_last_break = m_current_line.character_count();

                        // append the character
                        m_current_line.add_character(ch, style, source);

                        // do we have to wrap?
                        if (wrap() != word_wrapping.NEVER && m_current_line.width() > m_width)
                        {
                            switch (wrap())
                            {
                                case word_wrapping.TRUNCATE:
                                    truncate_wrap();
                                    break;

                                case word_wrapping.WORD:
                                    word_wrap();
                                    break;

                                default:
                                    fatalerror("invalid word wrapping value");
                                    break;
                            }
                        }
                        else
                        {
                            // we didn't wrap - if we hit any non-space breakable character, remember the location and width
                            // *with* the breakable character
                            if (ch != ' ' && is_breakable_char(ch))
                                m_last_break = m_current_line.character_count();
                        }
                    }
                }
            }
        }


        //-------------------------------------------------
        //  start_new_line
        //-------------------------------------------------
        void start_new_line(text_layout.text_justify justify, float height)
        {
            // create a new line
            line new_line = new line(this, justify, actual_height(), height * yscale());

            // update the current line
            m_current_line = new_line;
            m_last_break = 0;
            m_truncating = false;

            // append it
            m_lines.emplace_back(new_line);
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
            UInt32 truncate_position = m_current_line.character_count() - 1;
            var truncate_char = m_current_line.character(truncate_position);

            // copy style information
            char_style style = truncate_char.style;

            // copy source information
            source_info source;
            source.start = truncate_char.source.start + truncate_char.source.span;
            source.span = 0;

            // figure out how wide an elipsis is
            float elipsis_width = get_char_width(elipsis, style.size);

            // where should we really truncate from?
            while (truncate_position > 0 && m_current_line.character(truncate_position).xoffset + elipsis_width > width())
                truncate_position--;

            // truncate!!!
            m_current_line.truncate(truncate_position);

            // and append the elipsis
            m_current_line.add_character(elipsis, style, source);

            // take note that we are truncating; supress new characters
            m_truncating = true;
        }


        //-------------------------------------------------
        //  word_wrap
        //-------------------------------------------------
        void word_wrap()
        {
            // keep track of the last line and break
            line last_line = m_current_line;
            UInt32 last_break = m_last_break;

            // start a new line with the same justification
            start_new_line(last_line.justify(), last_line.character(last_line.character_count() - 1).style.size);

            // find the begining of the word to wrap
            UInt32 position = last_break;
            while (position + 1 < last_line.character_count() && is_space_character(last_line.character(position).character))
                position++;

            // transcribe the characters
            for (UInt32 i = position; i < last_line.character_count(); i++)
            {
                var ch = last_line.character(i);
                m_current_line.add_character(ch.character, ch.style, ch.source);
            }

            // and finally, truncate the last line
            last_line.truncate(last_break);
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
