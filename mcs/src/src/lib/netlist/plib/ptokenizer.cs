// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;

using token_reader_t_token_store = mame.plib.detail.token_store_t;  //using token_store = tokenizer_t::token_store_t;  //using token_store_t = detail::token_store_t;
using tokenizer_t_token_t = mame.plib.detail.token_t;  //using token_t = detail::token_t;
using tokenizer_t_token_id_t = mame.plib.detail.token_id_t;  //using token_id_t = detail::token_id_t;
using tokenizer_t_token_store_t = mame.plib.detail.token_store_t;  //using token_store_t = detail::token_store_t;
using tokenizer_t_token_type = mame.plib.detail.token_type;  //using token_type = detail::token_type;
using size_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.plib.pfmtlog_global;


namespace mame.plib
{
    namespace detail
    {
        enum token_type  //PENUM(token_type,
        {
            IDENTIFIER,
            NUMBER,
            TOKEN,
            STRING,
            COMMENT,
            LINEMARKER,
            SOURCELINE,
            UNKNOWN,
            ENDOFFILE
        }


        class token_id_t
        {
            //static constexpr std::size_t npos = static_cast<std::size_t>(-1);


            size_t m_id;
            string m_name;


            public token_id_t()
            {
                m_id = npos;
            }

            public token_id_t(size_t id, string name)
            {
                m_id = id;
                m_name = name;
            }

            //token_id_t(const token_id_t &) = default;
            //token_id_t &operator=(const token_id_t &) = default;
            //token_id_t(token_id_t &&) noexcept = default;
            //token_id_t &operator=(token_id_t &&) noexcept = default;

            //~token_id_t() = default;


            public size_t id() { return m_id; }
            public string name() { return m_name; }
        }


        class token_t
        {
            token_type m_type;
            size_t m_id;
            string m_token;


            public token_t(token_type type)
            {
                m_type = type;
                m_id = npos;
                m_token = "";
            }

            public token_t(token_type type, string str)
            {
                m_type = type;
                m_id = npos;
                m_token = str;
            }

            public token_t(token_id_t id)
            {
                m_type = token_type.TOKEN;
                m_id = id.id();
                m_token = id.name();
            }

            public token_t(token_id_t id, string str)
            {
                m_type = token_type.TOKEN;
                m_id = id.id();
                m_token = str;
            }


            //token_t(const token_t &) = default;
            //token_t &operator=(const token_t &) = default;
            //token_t(token_t &&) noexcept = default;
            //token_t &operator=(token_t &&) noexcept = default;

            //~token_t() = default;


            public bool is_(token_id_t tok_id) { return m_id == tok_id.id(); }
            public bool is_not(token_id_t tok_id) { return !is_(tok_id); }

            public bool is_type(token_type type) { return m_type == type; }

            //token_type type() const noexcept { return m_type; }

            public string str() { return m_token; }
        }


        class token_store_t : std.vector<token_t>
        {
            //using std::vector<token_t>::vector;
        }
    }


    class tokenizer_t
    {
        //using token_type = detail::token_type;
        //using token_id_t = detail::token_id_t;
        //using token_t = detail::token_t;
        //using token_store_t = detail::token_store_t;


        TextReader m_strm;  //putf8_reader *m_strm;

        string m_cur_line;
        int m_pxIdx;  //pstring::const_iterator m_px;
        char m_unget;  //pstring::value_type m_unget;

        // tokenizer stuff follows ...

        string m_identifier_chars;
        string m_number_chars;
        string m_number_chars_start;
        std.unordered_map<string, tokenizer_t_token_id_t> m_tokens = new std.unordered_map<string, tokenizer_t_token_id_t>();
        string m_whitespace;
        char m_string;  //pstring::value_type  m_string;

        tokenizer_t_token_id_t m_tok_comment_start;
        tokenizer_t_token_id_t m_tok_comment_end;
        tokenizer_t_token_id_t m_tok_line_comment;


        bool m_support_line_markers;
        tokenizer_t_token_store_t m_token_queue;


        public tokenizer_t() // NOLINT(misc-forwarding-reference-overload, bugprone-forwarding-reference-overload)
        {
            m_strm = null;
            m_unget = (char)0;
            m_string = '"';
            m_support_line_markers = true; // FIXME
            m_token_queue = null;


            clear();
        }


        //tokenizer_t(const tokenizer_t &) = delete;
        //tokenizer_t &operator=(const tokenizer_t &) = delete;
        //tokenizer_t(tokenizer_t &&) noexcept = delete;
        //tokenizer_t &operator=(tokenizer_t &&) noexcept = delete;

        //virtual ~ptokenizer() = default;


        // tokenizer stuff follows ...

        public tokenizer_t_token_id_t register_token(string token)
        {
            tokenizer_t_token_id_t ret = new tokenizer_t_token_id_t(m_tokens.size(), token);
            m_tokens.emplace(token, ret);
            return ret;
        }

        public tokenizer_t identifier_chars(string s) { m_identifier_chars = s; return this; }
        public tokenizer_t number_chars(string st, string rem) { m_number_chars_start = st; m_number_chars = rem; return this; }
        //ptokenizer & string_char(pstring::value_type c) { m_string = c; return *this; }
        public tokenizer_t whitespace(string s) { m_whitespace = s; return this; }
        public tokenizer_t comment(string start, string end, string line)
        {
            m_tok_comment_start = register_token(start);
            m_tok_comment_end = register_token(end);
            m_tok_line_comment = register_token(line);
            return this;
        }

        public void append_to_store(TextReader reader, tokenizer_t_token_store_t store)  //void append_to_store(putf8_reader *reader, token_store_t &store);
        {
            clear();

            m_strm = reader;

            // Process tokens into queue
            tokenizer_t_token_t ret = new tokenizer_t_token_t(tokenizer_t_token_type.UNKNOWN);
            m_token_queue = store;

            do
            {
                ret = get_token_comment();
                store.push_back(ret);
            } while (!ret.is_type(tokenizer_t_token_type.ENDOFFILE));

            m_token_queue = null;
        }


        void clear()
        {
            m_cur_line = "";
            m_pxIdx = 0;  //m_px = m_cur_line.begin();
            m_unget = (char)0;
        }


        tokenizer_t_token_t get_token_internal()
        {
            // skip ws
            char c = getc();  //pstring::value_type c = getc();
            while (m_whitespace.find(c) != npos)  //while (m_whitespace.find(c) != pstring::npos)
            {
                c = getc();
                if (eof())
                {
                    return new tokenizer_t_token_t(tokenizer_t_token_type.ENDOFFILE);
                }
            }

            if (m_support_line_markers && c == '#')
            {
                string lm = "#";
                do
                {
                    c = getc();
                    if (eof())
                        return new tokenizer_t_token_t(tokenizer_t_token_type.ENDOFFILE);

                    if (c == '\r' || c == '\n')
                        return new tokenizer_t_token_t(tokenizer_t_token_type.LINEMARKER, lm);

                    lm += c;
                } while (true);
            }

            if (m_number_chars_start.find(c) != npos)  //if (m_number_chars_start.find(c) != pstring::npos)
            {
                // read number while we receive number or identifier chars
                // treat it as an identifier when there are identifier chars in it
                tokenizer_t_token_type ret = tokenizer_t_token_type.NUMBER;
                string tokstr = "";
                while (true)
                {
                    if (m_identifier_chars.find(c) != npos && m_number_chars.find(c) == npos)  //if (m_identifier_chars.find(c) != pstring::npos && m_number_chars.find(c) == pstring::npos)
                        ret = tokenizer_t_token_type.IDENTIFIER;
                    else if (m_number_chars.find(c) == npos)  //else if (m_number_chars.find(c) == pstring::npos)
                        break;

                    tokstr += c;
                    c = getc();
                }

                ungetc(c);
                return new tokenizer_t_token_t(ret, tokstr);
            }

            // not a number, try identifier
            if (m_identifier_chars.find(c) != npos)  //if (m_identifier_chars.find(c) != pstring::npos)
            {
                // read identifier till non identifier char
                string tokstr = "";
                while (m_identifier_chars.find(c) != npos)  //while (m_identifier_chars.find(c) != pstring::npos)
                {
                    tokstr += c;
                    c = getc();
                }

                ungetc(c);
                var id = m_tokens.find(tokstr);
                return (id != default) ?
                        new tokenizer_t_token_t(id, tokstr)
                    :   new tokenizer_t_token_t(tokenizer_t_token_type.IDENTIFIER, tokstr);
            }

            if (c == m_string)
            {
                string tokstr = "";
                c = getc();
                while (c != m_string)
                {
                    tokstr += c;
                    c = getc();
                }

                return new tokenizer_t_token_t(tokenizer_t_token_type.STRING, tokstr);
            }

            {
                // read identifier till first identifier char or ws
                string tokstr = "";
                while ((m_identifier_chars.find(c) == npos) && (m_whitespace.find(c) == npos))
                {
                    tokstr += c;
                    // expensive, check for single char tokens
                    if (tokstr.length() == 1)
                    {
                        var id = m_tokens.find(tokstr);
                        if (id != default)
                            return new tokenizer_t_token_t(id, tokstr);
                    }

                    c = getc();
                }

                ungetc(c);

                {
                    var id = m_tokens.find(tokstr);
                    return (id != default) ?
                            new tokenizer_t_token_t(id, tokstr)
                        :   new tokenizer_t_token_t(tokenizer_t_token_type.UNKNOWN, tokstr);
                }
            }
        }


        // get internal token with comment processing
        tokenizer_t_token_t get_token_comment()
        {
            tokenizer_t_token_t ret = get_token_internal();
            while (true)
            {
                if (ret.is_type(tokenizer_t_token_type.ENDOFFILE))
                    return ret;

                if (ret.is_(m_tok_comment_start))
                {
                    do
                    {
                        ret = get_token_internal();
                    } while (ret.is_not(m_tok_comment_end));

                    ret = get_token_internal();
                }
                else if (ret.is_(m_tok_line_comment))
                {
                    skip_eol();
                    ret = get_token_internal();
                }
                else
                {
                    return ret;
                }
            }
        }


        void skip_eol() { throw new emu_unimplemented(); }


        char getc()  //pstring::value_type getc();
        {
            if (m_unget != 0)
            {
                char c2 = m_unget;  //pstring::value_type c = m_unget;
                m_unget = (char)0;
                return c2;
            }

            if (m_pxIdx == m_cur_line.Length)  //if (m_px == m_cur_line.end())
            {
                //++m_source_location.back();
                string line;  //putf8string line;
                if ((line = m_strm.ReadLine()) != null)  //if (m_strm->read_line_lf(line))
                {
                    line += '\n';
                    m_cur_line = line;
                    m_pxIdx = 0;  //m_px = m_cur_line.begin();
                    if (m_pxIdx != m_cur_line.Length && m_cur_line[m_pxIdx] != '#')  //if (*m_px != '#')
                        m_token_queue.push_back(new tokenizer_t_token_t(tokenizer_t_token_type.SOURCELINE, m_cur_line));
                }
                else
                {
                    return (char)0;
                }
            }

            char c = m_pxIdx != m_cur_line.Length ? m_cur_line[m_pxIdx++] : (char)0;  //pstring::value_type c = *(m_px++);
            return c;
        }


        void ungetc(char c)  //void ungetc(pstring::value_type c);
        {
            m_unget = c;
        }


        bool eof() { return m_strm.Peek() == -1; }  //bool eof() const { return m_strm->eof(); }
    }


    abstract class token_reader_t
    {
        //using token_t = tokenizer_t::token_t;
        //using token_type = tokenizer_t::token_type;
        //using token_id_t = tokenizer_t::token_id_t;
        //using token_store = tokenizer_t::token_store_t;


        static string MF_EXPECTED_TOKEN_1_GOT_2(params object [] args)  { return PERRMSGV(2, "Expected token <{0}>, got <{1}>", args); }
        //PERRMSGV(MF_EXPECTED_STRING_GOT_1,          1, "Expected a string, got <{1}>")
        protected static string MF_EXPECTED_IDENTIFIER_GOT_1(params object [] args)  { return PERRMSGV(1, "Expected an identifier, got <{0}>", args); }
        static string MF_EXPECTED_ID_OR_NUM_GOT_1(params object [] args)  { return PERRMSGV(1, "Expected an identifier or number, got <{0}>", args); }
        //PERRMSGV(MF_EXPECTED_NUMBER_GOT_1,          1, "Expected a number, got <{1}>")
        //PERRMSGV(MF_EXPECTED_LONGINT_GOT_1,         1, "Expected a long int, got <{1}>")
        static string MF_EXPECTED_LINENUM_GOT_1(params object [] args)  { return PERRMSGV(1, "Expected line number after line marker but got <{0}>", args); }
        static string MF_EXPECTED_FILENAME_GOT_1(params object [] args) { return PERRMSGV(1, "Expected file name after line marker but got <{0}>", args); }


        // source locations, vector used as stack because we need to loop through stack

        std.vector<plib.source_location> m_source_location = new std.vector<source_location>();
        string m_line = "";
        protected size_t m_idx;
        tokenizer_t_token_store_t m_token_store;


        protected token_reader_t()
        {
            m_idx = 0;
            m_token_store = null;


            // add a first entry to the stack
            m_source_location.emplace_back(new plib.source_location("Unknown", 0));
        }


        //token_reader_t(const token_reader_t &) = delete;
        //token_reader_t &operator=(const token_reader_t &) = delete;
        //token_reader_t(token_reader_t &&) noexcept = delete;
        //token_reader_t &operator=(token_reader_t &&) noexcept = delete;

        //virtual ~token_reader_t() = default;


        protected void set_token_source(tokenizer_t_token_store_t store)
        {
            m_token_store = store;
        }

        //pstring current_line_str() const;

        // tokenizer stuff follows ...

        protected tokenizer_t_token_t get_token()
        {
            tokenizer_t_token_t ret = get_token_queue();
            while (true)
            {
                if (ret.is_type(tokenizer_t_token_type.ENDOFFILE))
                    return ret;

                //printf("%s\n", ret.str().c_str());
                if (process_line_token(ret))
                {
                    ret = get_token_queue();
                }
                else
                {
                    return ret;
                }
            }
        }

        protected tokenizer_t_token_t get_token_raw()  // includes line information
        {
            tokenizer_t_token_t ret = get_token_queue();
            process_line_token(ret);
            return ret;
        }


        //pstring get_string();


        protected string get_identifier()
        {
            tokenizer_t_token_t tok = get_token();
            if (!tok.is_type(tokenizer_t_token_type.IDENTIFIER))
            {
                error(MF_EXPECTED_IDENTIFIER_GOT_1(tok.str()));
            }

            return tok.str();
        }


        protected string get_identifier_or_number()
        {
            tokenizer_t_token_t tok = get_token();
            if (!(tok.is_type(tokenizer_t_token_type.IDENTIFIER) || tok.is_type(tokenizer_t_token_type.NUMBER)))
            {
                error(MF_EXPECTED_ID_OR_NUM_GOT_1(tok.str()));
            }

            return tok.str();
        }


        //double get_number_double();
        //long get_number_long();


        protected void require_token(tokenizer_t_token_id_t token_num)
        {
            require_token(get_token(), token_num);
        }

        protected void require_token(tokenizer_t_token_t tok, tokenizer_t_token_id_t token_num)
        {
            if (!tok.is_(token_num))
            {
                error(MF_EXPECTED_TOKEN_1_GOT_2(token_num.name(), tok.str()));
            }
        }


        protected void error(string errs)  //void ptoken_reader::error(const perrmsg &errs)
        {
            string s = "";
            string trail       = "                 from ";
            string trail_first = "In file included from ";
            string e = new plib.pfmt("{0}:{1}:0: error: {2}\n")
                    .op(m_source_location.back().file_name(), m_source_location.back().line(), errs);
            m_source_location.pop_back();
            while (!m_source_location.empty())
            {
                if (m_source_location.size() == 1)
                    trail = trail_first;
                s = new plib.pfmt("{0}{1}:{2}:0\n{3}").op(trail, m_source_location.back().file_name(), m_source_location.back().line(), s);
                m_source_location.pop_back();
            }

            verror("\n" + s + e + " " + m_line + "\n");
        }


        protected plib.source_location location() { return m_source_location.back(); }

        //pstring current_line() const { return m_line; }


        protected abstract void verror(string msg);


        bool process_line_token(tokenizer_t_token_t tok)
        {
            if (tok.is_type(tokenizer_t_token_type.LINEMARKER))
            {
                bool benter = false;
                bool bexit = false;
                string file;
                unsigned lineno = 0;

                var sp = plib.pg.psplit(tok.str(), ' ');
                //printf("%d %s\n", (int) sp.size(), ret.str().c_str());

                bool err = false;
                lineno = plib.pg.pstonum_ne_unsigned(false, sp[1], out err);
                if (err)
                    error(MF_EXPECTED_LINENUM_GOT_1(tok.str()));

                if (sp[2].substr(0,1) != "\"")
                    error(MF_EXPECTED_FILENAME_GOT_1(tok.str()));

                file = sp[2].substr(1, sp[2].length() - 2);

                for (size_t i = 3; i < sp.size(); i++)
                {
                    if (sp[i] == "1")
                        benter = true;
                    if (sp[i] == "2")
                        bexit = true;
                    // FIXME: process flags; actually only 1 (file enter) and 2 (after file exit)
                }

                if (bexit) // pop the last location
                    m_source_location.pop_back();

                if (!benter) // new location!
                    m_source_location.pop_back();

                m_source_location.emplace_back(new plib.source_location(file, lineno));
                return true;
            }

            if (tok.is_type(tokenizer_t_token_type.SOURCELINE))
            {
                m_line = tok.str();
                m_source_location.back().inc();  //++m_source_location.back();
                return true;
            }

            return false;
        }


        tokenizer_t_token_t get_token_queue()
        {
            if (m_idx < m_token_store.size())
                return m_token_store[m_idx++];
            return new tokenizer_t_token_t(tokenizer_t_token_type.ENDOFFILE);
        }
    }
}
