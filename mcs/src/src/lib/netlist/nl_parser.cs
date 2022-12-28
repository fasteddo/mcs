// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;

using parser_t_token_store_t = mame.plib.detail.token_store_t;  //using token_store_t = plib::tokenizer_t::token_store_t;
using parser_t_token_id_t = mame.plib.detail.token_id_t;  //using token_id_t = plib::tokenizer_t::token_id_t;
using parser_t_token_t = mame.plib.detail.token_t;  //using token_t = plib::tokenizer_t::token_t;
using parser_t_token_type = mame.plib.detail.token_type;  //using token_type = plib::tokenizer_t::token_type;

using static mame.netlist.nl_errstr_global;


namespace mame.netlist
{
    class parser_t : plib.token_reader_t
    {
        //using token_t = plib::tokenizer_t::token_t;
        //using token_type = plib::tokenizer_t::token_type;
        //using token_id_t = plib::tokenizer_t::token_id_t;
        //using token_store_t = plib::tokenizer_t::token_store_t;


        parser_t_token_id_t m_tok_paren_left;
        parser_t_token_id_t m_tok_paren_right;
        parser_t_token_id_t m_tok_comma;
        parser_t_token_id_t m_tok_static;
        parser_t_token_id_t m_tok_ALIAS;
        parser_t_token_id_t m_tok_NET_C;
        parser_t_token_id_t m_tok_DIPPINS;
        parser_t_token_id_t m_tok_FRONTIER;
        parser_t_token_id_t m_tok_PARAM;
        parser_t_token_id_t m_tok_DEFPARAM;
        parser_t_token_id_t m_tok_HINT;
        parser_t_token_id_t m_tok_NET_MODEL;
        parser_t_token_id_t m_tok_NET_REGISTER_DEV;
        parser_t_token_id_t m_tok_NETLIST_START;
        parser_t_token_id_t m_tok_NETLIST_END;
        parser_t_token_id_t m_tok_NETLIST_EXTERNAL;
        parser_t_token_id_t m_tok_SUBMODEL;
        parser_t_token_id_t m_tok_INCLUDE;
        parser_t_token_id_t m_tok_EXTERNAL_SOURCE;
        parser_t_token_id_t m_tok_LOCAL_SOURCE;
        parser_t_token_id_t m_tok_LOCAL_LIB_ENTRY;
        parser_t_token_id_t m_tok_EXTERNAL_LIB_ENTRY;
        parser_t_token_id_t m_tok_TRUTHTABLE_START;
        parser_t_token_id_t m_tok_TRUTHTABLE_END;
        parser_t_token_id_t m_tok_TRUTHTABLE_ENTRY;
        parser_t_token_id_t m_tok_TT_HEAD;
        parser_t_token_id_t m_tok_TT_LINE;
        parser_t_token_id_t m_tok_TT_FAMILY;

        plib.tokenizer_t m_tokenizer = new plib.tokenizer_t();
        nlparse_t m_setup;

        std.unordered_map<string, parser_t_token_store_t> m_local = new std.unordered_map<string, parser_t_token_store_t>();
        parser_t_token_store_t m_cur_local;


        public parser_t(nlparse_t setup)
            : base()
        {
            m_setup = setup;
            m_cur_local = null;


            m_tokenizer.identifier_chars("abcdefghijklmnopqrstuvwvxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890_.-$@")
                .number_chars(".0123456789", "0123456789eE-.") //FIXME: processing of numbers
                .whitespace("" + " " + (char)9 + (char)10 + (char)13)
                .comment("/*", "*/", "//");
            m_tok_paren_left = m_tokenizer.register_token("(");
            m_tok_paren_right = m_tokenizer.register_token(")");
            m_tok_comma = m_tokenizer.register_token(",");

            m_tok_static = m_tokenizer.register_token("static");
            m_tok_ALIAS = m_tokenizer.register_token("ALIAS");
            m_tok_DIPPINS = m_tokenizer.register_token("DIPPINS");
            m_tok_NET_C = m_tokenizer.register_token("NET_C");
            m_tok_FRONTIER = m_tokenizer.register_token("OPTIMIZE_FRONTIER");
            m_tok_PARAM = m_tokenizer.register_token("PARAM");
            m_tok_DEFPARAM = m_tokenizer.register_token("DEFPARAM");
            m_tok_HINT = m_tokenizer.register_token("HINT");
            m_tok_NET_MODEL = m_tokenizer.register_token("NET_MODEL");
            m_tok_NET_REGISTER_DEV = m_tokenizer.register_token("NET_REGISTER_DEV");
            m_tok_INCLUDE = m_tokenizer.register_token("INCLUDE");
            m_tok_LOCAL_SOURCE = m_tokenizer.register_token("LOCAL_SOURCE");
            m_tok_LOCAL_LIB_ENTRY = m_tokenizer.register_token("LOCAL_LIB_ENTRY");
            m_tok_EXTERNAL_LIB_ENTRY = m_tokenizer.register_token("EXTERNAL_LIB_ENTRY");
            m_tok_SUBMODEL = m_tokenizer.register_token("SUBMODEL");
            m_tok_NETLIST_START = m_tokenizer.register_token("NETLIST_START");
            m_tok_NETLIST_END = m_tokenizer.register_token("NETLIST_END");
            m_tok_NETLIST_EXTERNAL = m_tokenizer.register_token("NETLIST_EXTERNAL");
            m_tok_EXTERNAL_SOURCE = m_tokenizer.register_token("EXTERNAL_SOURCE");
            m_tok_TRUTHTABLE_START = m_tokenizer.register_token("TRUTHTABLE_START");
            m_tok_TRUTHTABLE_END = m_tokenizer.register_token("TRUTHTABLE_END");
            m_tok_TRUTHTABLE_ENTRY = m_tokenizer.register_token("TRUTHTABLE_ENTRY");
            m_tok_TT_HEAD = m_tokenizer.register_token("TT_HEAD");
            m_tok_TT_LINE = m_tokenizer.register_token("TT_LINE");
            m_tok_TT_FAMILY = m_tokenizer.register_token("TT_FAMILY");

            m_tokenizer.register_token("RES_R");
            m_tokenizer.register_token("RES_K");
            m_tokenizer.register_token("RES_M");
            m_tokenizer.register_token("CAP_U");
            m_tokenizer.register_token("CAP_N");
            m_tokenizer.register_token("CAP_P");

        }


        //bool parse(plib::istream_uptr &&strm, const pstring &nlname);


        public bool parse(parser_t_token_store_t store, string nlname)
        {
            set_token_source(store);

            bool in_nl = false;

            while (true)
            {
                // FIXME: line numbers in cached local netlists are wrong
                //        need to process raw tokens here.
                parser_t_token_t token = get_token_raw();

                if (token.is_type(parser_t_token_type.ENDOFFILE))
                {
                    return false;
                }

                if (token.is_(m_tok_NETLIST_END) || token.is_(m_tok_TRUTHTABLE_END))
                {
                    if (!in_nl)
                    {
                        error(MF_PARSER_UNEXPECTED_1(token.str()));
                    }
                    else
                    {
                        in_nl = false;
                    }

                    require_token(m_tok_paren_left);
                    require_token(m_tok_paren_right);

                    m_cur_local.push_back(token);
                    m_cur_local.push_back(new parser_t_token_t(m_tok_paren_left));
                    m_cur_local.push_back(new parser_t_token_t(m_tok_paren_right));

                }
                else if (token.is_(m_tok_NETLIST_START) || token.is_(m_tok_TRUTHTABLE_START))
                {
                    if (in_nl)
                        error(MF_PARSER_UNEXPECTED_1(token.str()));

                    require_token(m_tok_paren_left);
                    parser_t_token_t name = get_token();
                    if (token.is_(m_tok_NETLIST_START) && (name.str() == nlname || nlname.empty()))
                    {
                        require_token(m_tok_paren_right);
                        parse_netlist();
                        return true;
                    }

                    if (token.is_(m_tok_TRUTHTABLE_START) && name.str() == nlname)
                    {
                        net_truth_table_start(nlname);
                        return true;
                    }

                    // create a new cached local store
                    m_local.emplace(name.str(), new parser_t_token_store_t());
                    m_cur_local = m_local[name.str()];
                    var sl = location();
                    var li = new plib.pfmt("# {0} \"{1}\"").op(sl.line(), sl.file_name());

                    m_cur_local.push_back(new parser_t_token_t(parser_t_token_type.LINEMARKER, li));
                    m_cur_local.push_back(token);
                    m_cur_local.push_back(new parser_t_token_t(m_tok_paren_left));
                    m_cur_local.push_back(name);
                    //m_cur_local->push_back(token_t(m_tok_paren_right));
                    in_nl = true;
                }
                // FIXME: do we really need this going forward ? there should be no need
                //        for NETLIST_EXTERNAL in netlist files
                else if (token.is_(m_tok_NETLIST_EXTERNAL))
                {
                    if (in_nl)
                        error(MF_UNEXPECTED_NETLIST_EXTERNAL());

                    require_token(m_tok_paren_left);
                    parser_t_token_t name = get_token();
                    require_token(m_tok_paren_right);
                }
                else if (!in_nl)
                {
                    if (!token.is_(m_tok_static) && !token.is_type(parser_t_token_type.SOURCELINE)
                            && !token.is_type(parser_t_token_type.LINEMARKER))
                        error(MF_EXPECTED_NETLIST_START_1(token.str()));
                }
                else
                {
                    m_cur_local.push_back(token);
                }
            }
        }


        public void parse_tokens(plib.istream_uptr strm, parser_t_token_store_t tokstore)
        {
            //plib::putf8_reader u8reader(strm.release_stream());
            m_tokenizer.append_to_store(new StreamReader(strm.release_stream()), tokstore);
        }


        void parse_netlist()
        {
            while (true)
            {
                parser_t_token_t token = get_token();

                if (token.is_type(parser_t_token_type.ENDOFFILE))
                    error(MF_UNEXPECTED_END_OF_FILE());

                m_setup.log().debug.op("Parser: Device: {0}\n", token.str());

                if (token.is_(m_tok_ALIAS))
                    net_alias();
                else if (token.is_(m_tok_DIPPINS))
                    dip_pins();
                else if (token.is_(m_tok_NET_C))
                    net_c();
                else if (token.is_(m_tok_FRONTIER))
                    frontier();
                else if (token.is_(m_tok_PARAM))
                    netdev_param();
                else if (token.is_(m_tok_DEFPARAM))
                    netdev_default_param();
                else if (token.is_(m_tok_HINT))
                    netdev_hint();
                else if (token.is_(m_tok_NET_MODEL))
                    net_model();
                else if (token.is_(m_tok_SUBMODEL))
                    net_sub_model();
                else if (token.is_(m_tok_INCLUDE))
                    net_include();
                else if (token.is_(m_tok_LOCAL_SOURCE))
                    net_local_source();
                else if (token.is_(m_tok_EXTERNAL_SOURCE))
                    net_external_source();
                else if (token.is_(m_tok_LOCAL_LIB_ENTRY))
                    net_lib_entry(true);
                else if (token.is_(m_tok_EXTERNAL_LIB_ENTRY))
                    net_lib_entry(false);
                else if (token.is_(m_tok_TRUTHTABLE_ENTRY))
                {
                    require_token(m_tok_paren_left);
                    string name = get_identifier();
                    register_local_as_source(name);
                    m_setup.include(name);
                    require_token(m_tok_paren_right);
                }
                else if (token.is_(m_tok_NET_REGISTER_DEV))
                {
                    net_register_dev();
                }
                else if (token.is_(m_tok_NETLIST_END))
                {
                    netdev_netlist_end();
                    return;
                }
                else if (!token.is_type(parser_t_token_type.IDENTIFIER))
                {
                    error(MF_EXPECTED_IDENTIFIER_GOT_1(token.str()));
                }
                else
                {
                    device(token.str());
                }
            }
        }


        void net_alias()
        {
            require_token(m_tok_paren_left);
            string alias = get_identifier_or_number();

            require_token(m_tok_comma);

            string out_ = get_identifier();

            require_token(m_tok_paren_right);

            m_setup.log().debug.op("Parser: Alias: {0} {1}\n", alias, out_);
            m_setup.register_alias(alias, out_);
        }


        void dip_pins() { throw new emu_unimplemented(); }


        void netdev_param()
        {
            require_token(m_tok_paren_left);
            string param = get_identifier();
            require_token(m_tok_comma);
            parser_t_token_t tok = get_token();
            if (tok.is_type(parser_t_token_type.STRING))
            {
                m_setup.log().debug.op("Parser: Param: {0} {1}\n", param, tok.str());
                m_setup.register_param(param, tok.str());
                require_token(m_tok_paren_right);
            }
            else
            {
                var val = stringify_expression(ref tok);
                m_setup.log().debug.op("Parser: Param: {0} {1}\n", param, val);
                m_setup.register_param(param, val);
                require_token(tok, m_tok_paren_right);
            }
        }

        void netdev_default_param() { throw new emu_unimplemented(); }
        void netdev_hint() { throw new emu_unimplemented(); }


        void net_c()
        {
            require_token(m_tok_paren_left);
            string first = get_identifier();
            require_token(m_tok_comma);

            while (true)
            {
                string t1 = get_identifier();
                m_setup.register_link(first , t1);
                m_setup.log().debug.op("Parser: Connect: {0} {1}\n", first, t1);
                parser_t_token_t n = get_token();
                if (n.is_(m_tok_paren_right))
                    break;

                if (!n.is_(m_tok_comma))
                    error(MF_EXPECTED_COMMA_OR_RP_1(n.str()));
            }
        }


        void frontier() { throw new emu_unimplemented(); }

        void device(string dev_type)
        {
            std.vector<string> params_ = new std.vector<string>();

            require_token(m_tok_paren_left);
            string devname = get_identifier();

            m_setup.log().debug.op("Parser: IC: {0}\n", devname);

            var tok = get_token();

            while (tok.is_(m_tok_comma))
            {
                tok = get_token();
                if (tok.is_type(parser_t_token_type.STRING))
                {
                    params_.push_back(tok.str());
                    tok = get_token();
                }
                else
                {
                    var value = stringify_expression(ref tok);
                    params_.push_back(value);
                }
            }

            require_token(tok, m_tok_paren_right);

            std.vector<string> params_temp = new std.vector<string>();
            params_temp.Add(devname);
            params_temp.AddRange(params_);
            m_setup.register_dev(dev_type, params_temp.ToArray());  //m_setup.register_dev(dev_type, devname, params);
        }


        void netdev_netlist_end()
        {
            // don't do much
            require_token(m_tok_paren_left);
            require_token(m_tok_paren_right);
        }


        void net_model() { throw new emu_unimplemented(); }
        void net_sub_model() { throw new emu_unimplemented(); }
        void net_include() { throw new emu_unimplemented(); }
        void net_local_source() { throw new emu_unimplemented(); }
        void net_external_source() { throw new emu_unimplemented(); }
        void net_lib_entry(bool is_local) { throw new emu_unimplemented(); }
        void net_register_dev() { throw new emu_unimplemented(); }
        void net_truth_table_start(string nlname) { throw new emu_unimplemented(); }

        protected override void verror(string msg)
        {
            m_setup.log().fatal.op("{0}", msg);
            throw new nl_exception(new plib.pfmt("{0}").op(msg));
        }


        void register_local_as_source(string name) { throw new emu_unimplemented(); }

        string stringify_expression(ref parser_t_token_t tok)
        {
            int pc = 0;
            string ret = "";
            while (!tok.is_(m_tok_comma))
            {
                if (tok.is_(m_tok_paren_left))
                {
                    pc++;
                }
                else if (tok.is_(m_tok_paren_right))
                {
                    if (pc <= 0)
                        break;

                    pc--;
                }

                ret += tok.str();
                tok = get_token();
            }

            return ret;
        }
    }


    //class source_token_t : public source_netlist_t
}
