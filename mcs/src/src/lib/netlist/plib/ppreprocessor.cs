// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;

using ppreprocessor_defines_map_type = mame.std.unordered_map<string, mame.plib.ppreprocessor.define_t>;  //using defines_map_type = std::unordered_map<pstring, define_t>;
using ppreprocessor_string_list = mame.std.vector<string>;  //using string_list = std::vector<pstring>;
using size_t = System.UInt64;
using uint_least64_t = System.UInt64;

using static mame.osdcore_global;


namespace mame.plib
{
    class ppreprocessor
    {
        //using string_list = std::vector<pstring>;
        //using defines_map_type = std::unordered_map<pstring, define_t>;


        public class define_t
        {
            public string m_name;
            public string m_replace;
            public bool m_has_params;
            public ppreprocessor_string_list m_params = new ppreprocessor_string_list();


            public define_t(string name, string replace)
            {
                m_name = name;
                m_replace = replace;
                m_has_params = false;
            }

            public define_t(string name)
            {
                m_name = name;
                m_replace = "";
                m_has_params = false;
            }
        }


        class input_context
        {
            public TextReader m_reader;  //putf8_reader m_reader;
            public int m_lineno;
            public string m_local_path;
            public string m_name;


            //template <typename T>
            public input_context(istream_uptr istrm, string local_path, string name)  //input_context(T &&istrm, const pstring &local_path, const pstring &name)
            {
                m_reader = new StreamReader(istrm.release_stream());  //: m_reader(std::forward<T>(istrm))
                m_lineno = 0;
                m_local_path = local_path;
                m_name = name;
            }
        }


        //template <typename PP, typename L = ppreprocessor::string_list>
        class simple_iter
        {
            ppreprocessor_string_list m_tokens;  //const L m_tokens;
            ppreprocessor m_parent;  //PP *m_parent;
            size_t m_pos;


            public simple_iter(ppreprocessor parent, ppreprocessor_string_list tokens)  //simple_iter(PP *parent, const L &tokens)
            {
                m_tokens = tokens;
                m_parent = parent;
                m_pos = 0;
            }


            /// \brief skip white space in token list
            ///
            public void skip_ws()
            {
                while (m_pos < m_tokens.size() && (m_tokens[m_pos] == " " || m_tokens[m_pos] == "\t"))
                    m_pos++;
            }

            /// \brief return next token skipping white space
            ///
            /// \return next token
            ///
            public string next()
            {
                skip_ws();
                if (m_pos >= m_tokens.size())
                    m_parent.error("unexpected end of line");

                return m_tokens[m_pos++];
            }

            /// \brief return next token including white space
            ///
            /// \return next token
            ///
            public string next_ws()
            {
                if (m_pos >= m_tokens.size())
                    m_parent.error("unexpected end of line");

                return m_tokens[m_pos++];
            }

            //const pstring &peek_ws()
            //{
            //    if (m_pos >= m_tokens.size())
            //        error("unexpected end of line");
            //    return m_tokens[m_pos];
            //}

            //const pstring &last()
            //{
            //    if (m_pos == 0)
            //        error("no last token at beginning of line");
            //    if (m_pos > m_tokens.size())
            //        error("unexpected end of line");
            //    return m_tokens[m_pos-1];
            //}

            public bool eod()
            {
                return m_pos >= m_tokens.size();
            }

            //void error(const pstring &err)
            //{
            //    m_parent->error(err);
            //}
        }


        enum state_e
        {
            PROCESS,
            LINE_CONTINUATION
        }


        ppreprocessor_defines_map_type m_defines = new ppreprocessor_defines_map_type();
        psource_collection_t m_sources;
        ppreprocessor_string_list m_expr_sep = new ppreprocessor_string_list();

        uint_least64_t m_if_flag; // 63 if levels
        uint_least64_t m_if_seen; // 63 if levels
        uint_least64_t m_elif; // 63 if levels - for #elif
        int m_if_level;


        // vector used as stack because we need to loop through stack
        std.vector<input_context> m_stack = new std.vector<input_context>();
        string m_outbuf = "";
        state_e m_state;
        string m_line = "";
        bool m_comment;
        bool m_debug_out;


        public ppreprocessor(psource_collection_t sources, ppreprocessor_defines_map_type defines = null)
        {
            m_sources = sources;
            m_if_flag = 0;
            m_if_seen = 0;
            m_elif = 0;
            m_if_level = 0;
            m_state = state_e.PROCESS;
            m_comment = false;


            m_expr_sep.emplace_back("!");
            m_expr_sep.emplace_back("(");
            m_expr_sep.emplace_back(")");
            m_expr_sep.emplace_back("+");
            m_expr_sep.emplace_back("-");
            m_expr_sep.emplace_back("*");
            m_expr_sep.emplace_back("/");
            m_expr_sep.emplace_back("&&");
            m_expr_sep.emplace_back("||");
            m_expr_sep.emplace_back("==");
            m_expr_sep.emplace_back(",");
            m_expr_sep.emplace_back(";");
            m_expr_sep.emplace_back(".");
            m_expr_sep.emplace_back("##");
            m_expr_sep.emplace_back("#");
            m_expr_sep.emplace_back(" ");
            m_expr_sep.emplace_back("\t");
            m_expr_sep.emplace_back("\"");

            if (defines != null)
                m_defines = defines;
            m_defines.insert("__PLIB_PREPROCESSOR__", new define_t("__PLIB_PREPROCESSOR__", "1"));
            var idx = m_defines.find("__PREPROCESSOR_DEBUG__");
            m_debug_out = idx != default;
        }


        //PCOPYASSIGNMOVE(ppreprocessor, delete)

        //~ppreprocessor() = default;


        /// \brief process stream
        ///
        /// \param filename a filename or identifier identifying the stream.
        ///
        /// FIXME: this is sub-optimal. Refactor input_context into pinput_context
        /// and pass this to ppreprocessor.
        ///
        //template <typename T>
        public string process(istream_uptr istrm, string filename)  //pstring process(T &&istrm, const pstring &filename)
        {
            m_outbuf = m_outbuf.clear_();
            m_stack.emplace_back(new input_context(istrm, plib.util.path(filename), filename));  //m_stack.emplace_back(input_context(istrm.release_stream(),plib::util::path(filename), filename));
            process_stack();
            return m_outbuf;
        }


        void error(string err)
        {
            string s = "";
            string trail       = "                 from ";
            string trail_first = "In file included from ";
            string e = new plib.pfmt("{0}:{1}:0: error: {2}\n")
                    .op(m_stack.back().m_name, m_stack.back().m_lineno, err);
            m_stack.pop_back();
            while (!m_stack.empty())
            {
                if (m_stack.size() == 1)
                    trail = trail_first;
                s = new plib.pfmt("{0}{1}:{2}:0\n{3}").op(trail, m_stack.back().m_name, m_stack.back().m_lineno, s);
                m_stack.pop_back();
            }

            throw new pexception("\n" + s + e + " " + m_line + "\n");
        }


        define_t get_define(string name)
        {
            var idx = m_defines.find(name);
            return (idx != default) ? idx : null;  //return (idx != m_defines.end()) ? &idx->second : nullptr;
        }


        string replace_macros(string line)
        {
            //std::vector<pstring> elems(psplit(line, m_expr_sep));
            bool repeat = false;
            string tmpret = line;
            do
            {
                repeat = false;
                simple_iter elems = new simple_iter(this, tokenize(tmpret, m_expr_sep, false, true));  //simple_iter<ppreprocessor> elems(this, tokenize(tmpret, m_expr_sep, false, true));
                tmpret = "";
                while (!elems.eod())
                {
                    var token = elems.next_ws();
                    define_t def = get_define(token);
                    if (def == null)
                    {
                        tmpret += token;
                    }
                    else if (!def.m_has_params)
                    {
                        tmpret += def.m_replace;
                        repeat = true;
                    }
                    else
                    {
                        token = elems.next();
                        if (token != "(")
                            error("expected '(' in macro expansion of " + def.m_name);

                        ppreprocessor_string_list rep = new ppreprocessor_string_list();
                        token = elems.next();
                        while (token != ")")
                        {
                            string par = "";
                            int parenthesis_count = 1;
                            while (true)
                            {
                                if (parenthesis_count == 1 && token == ",")
                                {
                                    token = elems.next();
                                    break;
                                }

                                if (token == "(")
                                    parenthesis_count++;

                                if (token == ")")
                                {
                                    if (--parenthesis_count == 0)
                                        break;
                                }

                                par += token;
                                token = elems.next();
                            }

                            rep.push_back(par);
                        }

                        repeat = true;
                        if (def.m_params.size() != rep.size())
                            error(new pfmt("Expected {0} parameters, got {1}").op(def.m_params.size(), rep.size()));

                        simple_iter r = new simple_iter(this, tokenize(def.m_replace, m_expr_sep, false, false));  //simple_iter<ppreprocessor> r(this, tokenize(def->m_replace, m_expr_sep, false, false));
                        bool stringify_next = false;
                        while (!r.eod())
                        {
                            token = r.next();
                            if (token == "#")
                            {
                                stringify_next = true;
                            }
                            else if (token != " " && token != "\t")
                            {
                                for (size_t i = 0; i < def.m_params.size(); i++)
                                {
                                    if (def.m_params[i] == token)
                                    {
                                        if (stringify_next)
                                        {
                                            stringify_next = false;
                                            token = "\"" + rep[i] + "\"";
                                        }
                                        else
                                        {
                                            token = rep[i];
                                        }
                                        break;
                                    }
                                }

                                if (stringify_next)
                                    error("'#' is not followed by a macro parameter");

                                tmpret += token;
                                tmpret += " "; // make sure this is not concatenated with next token
                            }
                            else
                            {
                                tmpret += token;
                            }
                        }
                    }
                }
            } while (repeat);

            return tmpret;
        }


        void push_out(string s)
        {
            m_outbuf += s;
            if (m_debug_out)
                osd_printf_debug(s);  //std::cerr << putf8string(s);
        }


        void process_stack()
        {
            while (!m_stack.empty())
            {
                string line;  //putf8string line;
                string line_marker = new pfmt("# {0} \"{1}\"\n").op(m_stack.back().m_lineno, m_stack.back().m_name);
                push_out(line_marker);
                bool last_skipped = false;
                while ((line = m_stack.back().m_reader.ReadLine()) != null)
                {
                    m_stack.back().m_lineno++;
                    var r = process_line(line);
                    if (r.second)
                    {
                        if (last_skipped)
                            push_out(new pfmt("# {0} \"{1}\"\n").op(m_stack.back().m_lineno, m_stack.back().m_name));

                        push_out(r.first + "\n");
                        last_skipped = false;
                    }
                    else
                    {
                        last_skipped = true;
                    }
                }

                m_stack.pop_back();
                if (!m_stack.empty())
                {
                    line_marker = new pfmt("# {0} \"{1}\" 2\n").op(m_stack.back().m_lineno, m_stack.back().m_name);
                    push_out(line_marker);
                }
            }
        }


        ppreprocessor_string_list tokenize(string str, ppreprocessor_string_list sep, bool remove_ws, bool concat)
        {
            string STR = "\"";
            ppreprocessor_string_list tmp_ret = new ppreprocessor_string_list();
            ppreprocessor_string_list tmp = plib.pg.psplit(str, sep);
            size_t pi = 0;

            while (pi < tmp.size())
            {
                if (tmp[pi] == STR)
                {
                    string s = STR;
                    pi++;
                    while (pi < tmp.size() && tmp[pi] != STR)
                    {
                        s += tmp[pi];
                        pi++;
                    }

                    s += STR;
                    tmp_ret.push_back(s);
                }
                else
                {
                    string tok = tmp[pi];
                    if (tok.length() >= 2 && pi < tmp.size() - 2 )
                    {
                        var sc = tok.substr(0,1);
                        var ec = tok.substr(tok.length() - 1, 1);
                        if ((sc == "." || (sc[0] >= '0' && sc[0] <= '9')) && (ec == "e" || ec == "E"))
                        {
                            // looks like an incomplete float due splitting by - or +
                            tok = tok + tmp[pi + 1] + tmp[pi + 2];
                            pi += 2;
                        }
                    }

                    if (!remove_ws || (tok != " " && tok != "\t"))
                        tmp_ret.push_back(tok);
                }

                pi++;
            }

            if (!concat)
                return tmp_ret;

            // FIXME: error if concat at beginning or end
            ppreprocessor_string_list ret = new ppreprocessor_string_list();
            pi = 0;
            while (pi < tmp_ret.size())
            {
                if (tmp_ret[pi] == "##")
                {
                    while (ret.back() == " " || ret.back() == "\t")
                        ret.pop_back();

                    string cc = ret.back();
                    ret.pop_back();
                    pi++;
                    while (pi < tmp_ret.size() && (tmp_ret[pi] == " " || tmp_ret[pi] == "\t"))
                        pi++;

                    if (pi == tmp_ret.size())
                        error("## found at end of sequence");

                    ret.push_back(cc + tmp_ret[pi]);
                }
                else
                {
                    ret.push_back(tmp_ret[pi]);
                }

                pi++;
            }

            return ret;
        }


        static bool is_valid_token(string str)
        {
            if (str.empty())
                return false;

            var c = str[0];  //pstring::value_type c(str.at(0));
            return ((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_');
        }


        std.pair<string, bool> process_line(string line_in)
        {
            bool line_cont = plib.pg.right(line_in, 1) == "\\";

            string line = line_cont ? plib.pg.left(line_in, line_in.length() - 1) : line_in;

            if (m_state == state_e.LINE_CONTINUATION)
                m_line += line;
            else
                m_line = line;

            if (line_cont)
            {
                m_state = state_e.LINE_CONTINUATION;
                return new std.pair<string, bool>("", false);
            }

            m_state = state_e.PROCESS;

            line = process_comments(m_line);

            string lt = plib.pg.trim(plib.pg.replace_all(line, "\t", " "));
            if (plib.pg.startsWith(lt, "#"))
            {
                ppreprocessor_string_list lti = plib.pg.psplit(lt, ' ', true);
                if (lti[0] == "#if")
                {
                    m_if_level++;
                    m_if_seen |= (1U << m_if_level);
                    if (m_if_flag == 0)
                    {
                        lt = replace_macros(lt);
                        simple_iter t = new simple_iter(this, tokenize(lt.substr(3), m_expr_sep, true, true));  //simple_iter<ppreprocessor> t(this, tokenize(lt.substr(3), m_expr_sep, true, true));
                        var val = (int)prepro_expr(t, 255);
                        t.skip_ws();
                        if (!t.eod())
                            error("found unprocessed content at end of line");
                        if (val == 0)
                            m_if_flag |= (1U << m_if_level);
                        else
                            m_elif |= (1U << m_if_level);
                    }
                }
                else if (lti[0] == "#ifdef")
                {
                    m_if_level++;
                    m_if_seen |= (1U << m_if_level);
                    if (get_define(lti[1]) == null)
                        m_if_flag |= (1U << m_if_level);
                    else
                        m_elif |= (1U << m_if_level);
                }
                else if (lti[0] == "#ifndef")
                {
                    m_if_level++;
                    m_if_seen |= (1U << m_if_level);
                    if (get_define(lti[1]) != null)
                        m_if_flag |= (1U << m_if_level);
                    else
                        m_elif |= (1U << m_if_level);
                }
                else if (lti[0] == "#else") // basically #elif (1)
                {
                    if ((m_if_seen & (1U << m_if_level)) == 0)
                        error("#else without #if");

                    if ((m_elif & (1U << m_if_level)) != 0) // elif disabled
                        m_if_flag |= (1U << m_if_level);
                    else
                        m_if_flag &= ~(1U << m_if_level);

                    m_elif |= (1U << m_if_level);
                }
                else if (lti[0] == "#elif")
                {
                    if ((m_if_seen & (1U << m_if_level)) == 0)
                        error("#elif without #if");

                    //if ((m_if_flag & (1 << m_if_level)) == 0)
                    //  m_if_flag ^= (1 << m_if_level);
                    if ((m_elif & (1U << m_if_level)) != 0) // elif disabled
                        m_if_flag |= (1U << m_if_level);
                    else
                        m_if_flag &= ~(1U << m_if_level);

                    if (m_if_flag == 0)
                    {
                        //m_if_flag ^= (1 << m_if_level);
                        lt = replace_macros(lt);
                        simple_iter t = new simple_iter(this, tokenize(lt.substr(5), m_expr_sep, true, true));  //simple_iter<ppreprocessor> t(this, tokenize(lt.substr(5), m_expr_sep, true, true));
                        var val = (int)prepro_expr(t, 255);
                        t.skip_ws();
                        if (!t.eod())
                            error("found unprocessed content at end of line");
                        if (val == 0)
                            m_if_flag |= (1U << m_if_level);
                        else
                            m_elif |= (1U << m_if_level);
                    }
                }
                else if (lti[0] == "#endif")
                {
                    if ((m_if_seen & (1U << m_if_level)) == 0)
                        error("#else without #if");
                    m_if_seen &= ~(1U << m_if_level);
                    m_elif &= ~(1U << m_if_level);
                    m_if_flag &= ~(1U << m_if_level);
                    m_if_level--;
                }
                else if (lti[0] == "#include")
                {
                    if (m_if_flag == 0)
                    {
                        string arg = "";
                        for (size_t i = 1; i < lti.size(); i++)
                            arg += (lti[i] + " ");

                        arg = plib.pg.trim(arg);

                        if (plib.pg.startsWith(arg, "\"") && plib.pg.endsWith(arg, "\""))
                        {
                            arg = arg.substr(1, arg.length() - 2);
                            // first try local context
                            var l = plib.util.build_path(m_stack.back().m_local_path, arg);
                            var lstrm = m_sources.get_stream(l);
                            if (!lstrm.empty())
                            {
                                m_stack.emplace_back(new input_context(lstrm, plib.util.path(l), l));  //m_stack.emplace_back(input_context(lstrm.release_stream(), plib::util::path(l), l));
                            }
                            else
                            {
                                var strm = m_sources.get_stream(arg);
                                if (!strm.empty())
                                {
                                    m_stack.emplace_back(new input_context(strm, plib.util.path(arg), arg));  //m_stack.emplace_back(input_context(strm.release_stream(), plib::util::path(arg), arg));
                                }
                                else
                                {
                                    error("include not found:" + arg);
                                }
                            }
                        }
                        else
                        {
                            error("include misspelled:" + arg);
                        }

                        string line_marker = new pfmt("# {0} \"{1}\" 1\n").op(m_stack.back().m_lineno, m_stack.back().m_name);
                        push_out(line_marker);
                    }
                }
                else if (lti[0] == "#pragma")
                {
                    if (m_if_flag == 0 && lti.size() > 3 && lti[1] == "NETLIST")
                    {
                        if (lti[2] == "warning")
                            error("NETLIST: " + cat_remainder(lti, 3, " "));
                    }
                }
                else if (lti[0] == "#define")
                {
                    if (m_if_flag == 0)
                    {
                        if (lti.size() < 2)
                            error("define needs at least one argument");

                        simple_iter args = new simple_iter(this, tokenize(lt.substr(8), m_expr_sep, false, false));  //simple_iter<ppreprocessor> args(this, tokenize(lt.substr(8), m_expr_sep, false, false));
                        string n = args.next();
                        if (!is_valid_token(n))
                            error("define expected identifier");

                        var previous_define = get_define(n);
                        if (lti.size() == 2)
                        {
                            if (previous_define != null && !previous_define.m_replace.empty())
                                error("redefinition of " + n);

                            m_defines.insert(n, new define_t(n, ""));
                        }
                        else if (args.next_ws() == "(")
                        {
                            define_t def = new define_t(n);
                            def.m_has_params = true;
                            var token = args.next();
                            while (true)
                            {
                                if (token == ")")
                                    break;

                                def.m_params.push_back(token);
                                token = args.next();
                                if (token != "," && token != ")")
                                    error(new pfmt("expected , or ), found <{0}>").op(token));
                                if (token == ",")
                                    token = args.next();
                            }

                            string r = "";
                            while (!args.eod())
                                r += args.next_ws();

                            def.m_replace = r;
                            if (previous_define != null && previous_define.m_replace != r)
                                error("redefinition of " + n);

                            m_defines.insert(n, def);
                        }
                        else
                        {
                            string r = "";
                            while (!args.eod())
                                r += args.next_ws();

                            if (previous_define != null && previous_define.m_replace != r)
                                error("redefinition of " + n);

                            m_defines.insert(n, new define_t(n, r));
                        }
                    }
                }
                else if (lti[0] == "#undef")
                {
                    if (m_if_flag == 0)
                    {
                        if (lti.size() < 2)
                            error("undef needs at least one argument");

                        simple_iter args = new simple_iter(this, tokenize(lt.substr(7), m_expr_sep, false, false));  //simple_iter<ppreprocessor> args(this, tokenize(lt.substr(7), m_expr_sep, false, false));
                        string n = args.next();
                        if (!is_valid_token(n))
                            error("undef expected identifier");

                        m_defines.erase(n);
                    }
                }
                else
                {
                    if (m_if_flag == 0)
                        error("unknown directive");
                }
                return new std.pair<string, bool>("", false);
            }

            if (m_if_flag == 0)
                return new std.pair<string, bool>(replace_macros(lt), true);

            return new std.pair<string, bool>("", false);
        }


        string process_comments(string line)
        {
            bool in_string = false;

            string ret = "";
            for (int cIdx = 0; cIdx != line.Length; )  //for (auto c = line.begin(); c != line.end(); )
            {
                var c = line[cIdx];

                if (!m_comment)
                {
                    if (c == '"')
                    {
                        in_string = !in_string;
                        ret += c;
                    }
                    else if (in_string && c == '\\')
                    {
                        ret += c;
                        ++cIdx; if (cIdx != line.Length) c = line[cIdx];  //++c;
                        if (cIdx == line.Length)
                            break;

                        ret += c;
                    }
                    else if (!in_string && c == '/')
                    {
                        ++cIdx; if (cIdx != line.Length) c = line[cIdx];  //++c;
                        if (cIdx == line.Length)
                            break;

                        if (c == '*')
                            m_comment = true;
                        else if (c == '/')
                            break;
                        else
                            ret += c;
                    }
                    else
                    {
                        ret += c;
                    }
                }
                else
                {
                    if (c == '*')
                    {
                        cIdx++; if (cIdx != line.Length) c = line[cIdx];  //c++;
                        if (cIdx == line.Length)
                            break;

                        if (c == '/')
                            m_comment = false;
                    }
                }

                cIdx++; if (cIdx != line.Length) c = line[cIdx];  //c++;
            }

            return ret;
        }


        //template <typename PP>
        static int prepro_expr(simple_iter sexpr, int prio)  //static int prepro_expr(simple_iter<PP> &sexpr, int prio)
        {
            throw new emu_unimplemented();
#if false
            int val(0);
            bool has_val(false);

            pstring tok=sexpr.peek_ws();
            if (tok == "(")
            {
                sexpr.next();
                val = prepro_expr(sexpr, 255);
                if (sexpr.next() != ")")
                    sexpr.error("expected ')'");
                has_val = true;
            }
            while (!sexpr.eod())
            {
                tok = sexpr.peek_ws();
                if (tok == ")")
                {
                    if (!has_val)
                        sexpr.error("Found ')' but have no value computed");
                    else
                        return val;
                }
                else if (tok == "!")
                {
                    if (prio < 3)
                    {
                        if (!has_val)
                            sexpr.error("parsing error!");
                        else
                            return val;
                    }
                    sexpr.next();
                    val = !prepro_expr(sexpr, 3);
                    has_val = true;
                }
                CHECKTOK2(*,  5)
                CHECKTOK2(/,  5) // NOLINT(clang-analyzer-core.DivideZero)
                CHECKTOK2(+,  6)
                CHECKTOK2(-,  6)
                CHECKTOK2(==, 10)
                CHECKTOK2(&&, 14)
                CHECKTOK2(||, 15)
                else
                {
                    try
                    {
                        val = plib::pstonum<decltype(val)>(tok);
                    }
                    catch (pexception &e)
                    {
                        sexpr.error(e.text());
                    }
                    has_val = true;
                    sexpr.next();
                }
            }
            if (!has_val)
                sexpr.error("No value computed. Empty expression ?");
            return val;
#endif
        }


        static string cat_remainder(std.vector<string> elems, size_t start, string sep)
        {
            throw new emu_unimplemented();
#if false
            pstring ret("");
            for (std::size_t i = start; i < elems.size(); i++)
            {
                ret += elems[i];
                ret += sep;
            }
            return ret;
#endif
        }
    }
}
