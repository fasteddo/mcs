// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using abstract_t_link_t = mame.std.pair<string, string>;  //using link_t = std::pair<pstring, pstring>;
using log_type = mame.plib.plog_base<mame.netlist.nl_config_global.bool_const_NL_DEBUG>;  //using log_type =  plib::plog_base<NL_DEBUG>;
using models_t_map_t = mame.std.unordered_map<string, string>;  //using map_t = std::unordered_map<pstring, pstring>;
using models_t_raw_map_t = mame.std.unordered_map<string, string>;  //using raw_map_t = std::unordered_map<pstring, pstring>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using param_model_t_value_str_t = mame.netlist.param_model_t.value_base_t<string, mame.netlist.param_model_t.value_base_t_operators_string>;  //using value_str_t = value_base_t<pstring>;
using param_model_t_value_t = mame.netlist.param_model_t.value_base_t<System.Double, mame.netlist.param_model_t.value_base_t_operators_double>;  //using value_t = value_base_t<nl_fptype>;
using parser_t_token_store = mame.plib.detail.token_store;  //using token_store = plib::ptokenizer::token_store;
using ppreprocessor_defines_map_type = mame.std.unordered_map<string, mame.plib.ppreprocessor.define_t>;  //using defines_map_type = std::unordered_map<pstring, define_t>;
using size_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.netlist.nl_errstr_global;
using static mame.netlist.nl_setup_global;


namespace mame.netlist
{
    public static class nl_setup_global
    {
        //============================================================
        //  MACROS - netlist definitions
        //============================================================

        //#define NET_STR(x) # x

        //#define NET_MODEL(model)                                                           \
        //    setup.register_model(model);
        public static void NET_MODEL(nlparse_t setup, string model) { setup.register_model(model); }

        //#define ALIAS(alias, name)                                                        \
        //    setup.register_alias(# alias, # name);
        public static void ALIAS(nlparse_t setup, string alias, string name) { setup.register_alias(alias, name); }

        //#define DIPPINS(pin1, ...)                                                          \
        //        setup.register_dippins_arr( # pin1 ", " # __VA_ARGS__);
        public static void DIPPINS(nlparse_t setup, params string [] pin1) { setup.register_dip_alias_arr(string.Join(", ", pin1)); }

        // to be used to reference new library truthtable devices
        //#define NET_REGISTER_DEV(type, name)                                            \
        //        setup.register_dev(# type, # name);
        public static void NET_REGISTER_DEV(nlparse_t setup, string type, string name) { setup.register_dev(type, name); }

        // name is first element so that __VA_ARGS__ always has one element
        //#define NET_REGISTER_DEVEXT(type, ...)                                   \
        //        setup.register_dev(# type, { PSTRINGIFY_VA(__VA_ARGS__) });
        public static void NET_REGISTER_DEVEXT(nlparse_t setup, string type, params string [] args) { setup.register_dev(type, args); }

        //#define NET_CONNECT(name, input, output)                                        \
        //        setup.register_link(# name "." # input, # output);

        //#define NET_C(term1, ...)                                                       \
        //        setup.register_link_arr( # term1 ", " # __VA_ARGS__);
        public static void NET_C(nlparse_t setup, params string [] term1) { setup.register_link_arr(string.Join(", ", term1)); }

        //#define PARAM(name, val)                                                        \
        //        setup.register_param(NET_STR(name), NET_STR(val));
        public static void PARAM(nlparse_t setup, string name, int val) { PARAM(setup, name, val.ToString()); }
        public static void PARAM(nlparse_t setup, string name, double val) { PARAM(setup, name, val.ToString()); }
        public static void PARAM(nlparse_t setup, string name, string val) { setup.register_param(name, val); }

        //#define DEFPARAM(name, val)                                                       \
        //        setup.register_defparam(NET_STR(name), NET_STR(val));
        public static void DEFPARAM(nlparse_t setup, string name, string val) { setup.register_defparam(name, val); }

        //#define HINT(name, val)                                                        \
        //        setup.register_hint(# name , ".HINT_" # val);
        public static void HINT(nlparse_t setup, string name, string val) { setup.register_hint(name, ".HINT_" + val); }

        //#define NETDEV_PARAMI(name, param, val)                                         \
        //        setup.register_param(# name "." # param, val);
        public static void NETDEV_PARAMI(nlparse_t setup, string name, string param, string val) { setup.register_param(name + "." + param, val); }

        //#define NETLIST_NAME(name) netlist ## _ ## name

        //#define NETLIST_EXTERNAL(name)                                                 \
        //        void NETLIST_NAME(name)(netlist::nlparse_t &setup);

        //#define NETLIST_START(name)                                                    \
        //void NETLIST_NAME(name)(netlist::nlparse_t &setup)                               \
        //{
        //    plib::unused_var(setup);
        public static void NETLIST_START() { }

        //#define NETLIST_END()  }
        public static void NETLIST_END() { }

        //#define LOCAL_SOURCE(name)                                                     \
        //        setup.register_source_proc(# name, &NETLIST_NAME(name));
        public static void LOCAL_SOURCE(nlparse_t setup, string name, nlsetup_func netlist_name) { setup.register_source_proc(name, netlist_name); }

        //#define EXTERNAL_SOURCE(name)                                                  \
        //        NETLIST_EXTERNAL(name)                                                 \
        //        setup.register_source_proc(# name, &NETLIST_NAME(name));
        public static void EXTERNAL_SOURCE(nlparse_t setup, string name, nlsetup_func netlist_name) { setup.register_source_proc(name, netlist_name); }

        //#define LOCAL_LIB_ENTRY_2(type, name)                                          \
        //        type ## _SOURCE(name)                                                  \
        //        setup.register_lib_entry(# name, "", PSOURCELOC());

        //#define LOCAL_LIB_ENTRY_3(type, name, param_spec)                              \
        //        type ## _SOURCE(name)                                                  \
        //        setup.register_lib_entry(# name, param_spec, PSOURCELOC());

        //#define LOCAL_LIB_ENTRY(...) PCALLVARARG(LOCAL_LIB_ENTRY_, LOCAL, __VA_ARGS__)
        public static void LOCAL_LIB_ENTRY(nlparse_t setup, string name, nlsetup_func netlist_name)
        {
            LOCAL_SOURCE(setup, name, netlist_name);
            setup.register_lib_entry(name, "", plib.pg.PSOURCELOC());
        }

        //#define EXTERNAL_LIB_ENTRY(...) PCALLVARARG(LOCAL_LIB_ENTRY_, EXTERNAL, __VA_ARGS__)
        public static void EXTERNAL_LIB_ENTRY(nlparse_t setup, string name, nlsetup_func netlist_name)
        {
            EXTERNAL_SOURCE(setup, name, netlist_name);
            setup.register_lib_entry(name, "", plib.pg.PSOURCELOC());
        }

        //#define INCLUDE(name)                                                          \
        //        setup.include(# name);
        public static void INCLUDE(nlparse_t setup, string name) { setup.include(name); }

        //#define SUBMODEL(model, name)                                                  \
        //        setup.namespace_push(# name);                                          \
        //        setup.include(# model);                                                \
        //        setup.namespace_pop();
        public static void SUBMODEL(nlparse_t setup, string model, string name)
        {
            setup.namespace_push(name);
            setup.include(model);
            setup.namespace_pop();
        }

        //#define OPTIMIZE_FRONTIER(attach, r_in, r_out)                                  \
        //        setup.register_frontier(# attach, PSTRINGIFY_VA(r_in), PSTRINGIFY_VA(r_out));
        public static void OPTIMIZE_FRONTIER(nlparse_t setup, string attach, double r_in, double r_out)
        {
            setup.register_frontier(attach, r_in.ToString(), r_out.ToString());
        }


        // -----------------------------------------------------------------------------
        // truthtable defines
        // -----------------------------------------------------------------------------

        //#define TRUTHTABLE_START(cname, in, out, pdef_params)                           \
        //        NETLIST_START(cname) \
        //        netlist::tt_desc desc;                                                 \
        //        desc.name = #cname ;                                                   \
        //        desc.ni = in;                                                          \
        //        desc.no = out;                                                         \
        //        desc.family = "";                                                      \
        //        auto sloc = PSOURCELOC();                                              \
        //        const pstring def_params = pdef_params;
        public static void TRUTHTABLE_START(string cname, unsigned in_, unsigned out_, string pdef_params, out tt_desc desc_, out plib.source_location sloc_, out string def_params_)
        {
            netlist.tt_desc desc = new netlist.tt_desc();
            desc.name = cname;
            desc.ni = in_;
            desc.no = out_;
            desc.family = "";
            var sloc = plib.pg.PSOURCELOC();
            string def_params = pdef_params;

            desc_ = desc;
            sloc_ = sloc;
            def_params_ = def_params;
        }

        //#define TT_HEAD(x) \
        //        desc.desc.emplace_back(x);
        public static void TT_HEAD(tt_desc desc, string x) { desc.desc.emplace_back(x); }

        //#define TT_LINE(x) \
        //        desc.desc.emplace_back(x);
        public static void TT_LINE(tt_desc desc, string x) { desc.desc.emplace_back(x); }

        //#define TT_FAMILY(x) \
        //        desc.family = x;
        public static void TT_FAMILY(tt_desc desc, string x) { desc.family = x; }

        //#define TRUTHTABLE_END() \
        //        setup.truthtable_create(desc, def_params, std::move(sloc)); \
        //        NETLIST_END()
        public static void TRUTHTABLE_END(nlparse_t setup, tt_desc desc, plib.source_location sloc, string def_params)
        {
            setup.truthtable_create(desc, def_params, sloc);
            NETLIST_END();
        }

        //#define TRUTHTABLE_ENTRY(name)                                                 \
        //    LOCAL_SOURCE(name)                                                         \
        //    INCLUDE(name)
        public static void TRUTHTABLE_ENTRY(nlparse_t setup, string name, nlsetup_func func)
        {
            LOCAL_SOURCE(setup, name, func);
            INCLUDE(setup, name);
        }
    }


    public class helper
    {
        Stack<nlparse_t> m_helper_setups = new Stack<nlparse_t>();

        nlparse_t helper_setup { get { return m_helper_setups.Peek(); } }

        void helper_setup_push(netlist.nlparse_t setup) { m_helper_setups.Push(setup); }
        void helper_setup_pop() { m_helper_setups.Pop(); }


        // for truthtable
        netlist.tt_desc m_helper_desc;
        plib.source_location m_helper_sloc;
        string m_helper_def_params;


        // net_lib
        public void SOLVER(string name, int freq) { devices.net_lib_global.SOLVER(helper_setup, name, freq); }


        // netlist
        public void MEMREGION_SOURCE(mame.device_t device, string _name) { netlist_global.MEMREGION_SOURCE(helper_setup, device, _name); }


        // nl_setup
        public void NET_MODEL(string model) { nl_setup_global.NET_MODEL(helper_setup, model); }
        public void ALIAS(string alias, string name) { nl_setup_global.ALIAS(helper_setup, alias, name); }
        public void DIPPINS(params string [] pin1) { nl_setup_global.DIPPINS(helper_setup, pin1); }
        public void NET_REGISTER_DEV(string type, string name) { nl_setup_global.NET_REGISTER_DEV(helper_setup, type, name); }
        public void NET_C(params string [] term1) { nl_setup_global.NET_C(helper_setup, term1); }
        public void PARAM(string name, int val) { nl_setup_global.PARAM(helper_setup, name, val); }
        public void PARAM(string name, double val) { nl_setup_global.PARAM(helper_setup, name, val); }
        public void PARAM(string name, string val) { nl_setup_global.PARAM(helper_setup, name, val); }
        public void DEFPARAM(string name, string val) { nl_setup_global.DEFPARAM(helper_setup, name, val); }
        public void HINT(string name, string val) { nl_setup_global.HINT(helper_setup, name, val); }
        public void NETLIST_START(netlist.nlparse_t setup) { helper_setup_push(setup);  nl_setup_global.NETLIST_START(); }
        public void NETLIST_END() { nl_setup_global.NETLIST_END();  helper_setup_pop(); }
        public void LOCAL_SOURCE(string name, nlsetup_func netlist_name) { nl_setup_global.LOCAL_SOURCE(helper_setup, name, netlist_name); }
        public void EXTERNAL_SOURCE(string name, nlsetup_func netlist_name) { nl_setup_global.EXTERNAL_SOURCE(helper_setup, name, netlist_name); }
        public void LOCAL_LIB_ENTRY(string name, nlsetup_func netlist_name) { nl_setup_global.LOCAL_LIB_ENTRY(helper_setup, name, netlist_name); }
        public void EXTERNAL_LIB_ENTRY(string name, nlsetup_func netlist_name) { nl_setup_global.EXTERNAL_LIB_ENTRY(helper_setup, name, netlist_name); }
        public void INCLUDE(string name) { nl_setup_global.INCLUDE(helper_setup, name); }
        public void SUBMODEL(string model, string name) { nl_setup_global.SUBMODEL(helper_setup, model, name); }
        public void OPTIMIZE_FRONTIER(string attach, double r_in, double r_out) { nl_setup_global.OPTIMIZE_FRONTIER(helper_setup, attach, r_in, r_out); }
        public void TRUTHTABLE_START(netlist.nlparse_t setup, string cname, unsigned in_, unsigned out_, string pdef_params) { helper_setup_push(setup);  nl_setup_global.TRUTHTABLE_START(cname, in_, out_, pdef_params, out m_helper_desc, out m_helper_sloc, out m_helper_def_params); }
        public void TT_HEAD(string x) { nl_setup_global.TT_HEAD(m_helper_desc, x); }
        public void TT_LINE(string x) { nl_setup_global.TT_LINE(m_helper_desc, x); }
        public void TT_FAMILY(string x) { nl_setup_global.TT_FAMILY(m_helper_desc, x); }
        public void TRUTHTABLE_END() { nl_setup_global.TRUTHTABLE_END(helper_setup, m_helper_desc, m_helper_sloc, m_helper_def_params);  helper_setup_pop(); }
        public void TRUTHTABLE_ENTRY(string name, nlsetup_func func) { nl_setup_global.TRUTHTABLE_ENTRY(helper_setup, name, func); }


        // nld_devinc
        public void QBJT_EB(string name, string MODEL) { nld_devinc_global.QBJT_EB(helper_setup, name, MODEL); }
        public void OPAMP(string name, string MODEL) { nld_devinc_global.OPAMP(helper_setup, name, MODEL); }
        public void RES(string name, double R) { nld_devinc_global.RES(helper_setup, name, R); }
        public void POT(string name, double R) { nld_devinc_global.POT(helper_setup, name, R); }
        public void CAP(string name, double C) { nld_devinc_global.CAP(helper_setup, name, C); }
        public void ZDIODE(string name, string MODEL) { nld_devinc_global.ZDIODE(helper_setup, name, MODEL); }
        public void CD4066_GATE(string name) { nld_devinc_global.CD4066_GATE(helper_setup, name); }
        public void ANALOG_INPUT(string name, double IN) { nld_devinc_global.ANALOG_INPUT(helper_setup, name, IN); }
        public void CLOCK(string name, double FREQ) { nld_devinc_global.CLOCK(helper_setup, name, FREQ); }
        public void SYS_DSW(string name, string I, string _1, string _2) { nld_devinc_global.SYS_DSW(helper_setup, name, I, _1, _2); }
        public void SYS_NOISE_MT_N(string name, double SIGMA) { nld_devinc_global.SYS_NOISE_MT_N(helper_setup, name, SIGMA); }
        public void LOGIC_INPUT(string name, double IN, string MODEL) { nld_devinc_global.LOGIC_INPUT(helper_setup, name, IN, MODEL); }
        public void TTL_INPUT(string name, double IN) { nld_devinc_global.TTL_INPUT(helper_setup, name, IN); }
        public void UA741_DIP8(string name) { nld_devinc_global.UA741_DIP8(helper_setup, name); }
    }


    // -----------------------------------------------------------------------------
    // truthtable desc
    // -----------------------------------------------------------------------------
    public class tt_desc
    {
        public string name;
        public unsigned ni;  //unsigned long ni;
        public unsigned no;  //unsigned long no;
        public std.vector<string> desc = new std.vector<string>();
        public string family;

        public tt_desc() { ni = 0; no = 0; }
    }


    // ----------------------------------------------------------------------------------------
    // static compiled netlist.
    // ----------------------------------------------------------------------------------------

    public delegate void nlsetup_func(nlparse_t parse);  //using nlsetup_func = void (*)(nlparse_t &);


    // ----------------------------------------------------------------------------------------
    // nlparse_t
    // ----------------------------------------------------------------------------------------
    public class nlparse_t
    {
        ppreprocessor_defines_map_type m_defines = new ppreprocessor_defines_map_type();
        plib.psource_collection_t m_includes = new plib.psource_collection_t();
        std.stack<string> m_namespace_stack = new std.stack<string>();
        plib.psource_collection_t m_sources = new plib.psource_collection_t();
        detail.abstract_t m_abstract;

        //std::unordered_map<pstring, parser_t::token_store>    m_source_cache;
        log_type m_log;
        unsigned m_frontier_cnt;


        public nlparse_t(log_type log, detail.abstract_t abstract_)
        {
            m_abstract = abstract_;
            m_log = log;
            m_frontier_cnt = 0;
        }


        public void register_model(string model_in)
        {
            var pos = model_in.find(' ');
            if (pos == npos)
                throw new nl_exception(MF_UNABLE_TO_PARSE_MODEL_1(model_in));

            string model = plib.pg.ucase(plib.pg.trim(plib.pg.left(model_in, pos)));
            string def = plib.pg.trim(model_in.substr(pos + 1));
            if (!m_abstract.m_models.insert(model, def))
            {
                // FIXME: Add an directive MODEL_OVERWRITE to netlist language
                //throw nl_exception(MF_MODEL_ALREADY_EXISTS_1(model_in));
                log().info.op(MI_MODEL_OVERWRITE_1(model, model_in));
                m_abstract.m_models[model] = def;
            }
        }


        public void register_alias(string alias, string out_)
        {
            string alias_fqn = build_fqn(alias);
            string out_fqn = build_fqn(out_);
            register_alias_nofqn(alias_fqn, out_fqn);
        }


        public void register_alias_nofqn(string alias, string out_)
        {
            if (!m_abstract.m_alias.insert(alias, out_))
            {
                log().fatal.op(MF_ALIAS_ALREAD_EXISTS_1(alias));
                throw new nl_exception(MF_ALIAS_ALREAD_EXISTS_1(alias));
            }
        }


        public void register_dip_alias_arr(string terms)
        {
            var list = plib.pg.psplit(terms, ", ");
            if (list.empty() || (list.size() % 2) == 1)
            {
                log().fatal.op(MF_DIP_PINS_MUST_BE_AN_EQUAL_NUMBER_OF_PINS_1(build_fqn("")));
                throw new nl_exception(MF_DIP_PINS_MUST_BE_AN_EQUAL_NUMBER_OF_PINS_1(build_fqn("")));
            }

            size_t n = list.size();
            for (size_t i = 0; i < n / 2; i++)
            {
                register_alias(new plib.pfmt("{0}").op(i+1), list[i * 2]);
                register_alias(new plib.pfmt("{0}").op(n-i), list[i * 2 + 1]);
            }
        }


        // last argument only needed by nltool
        void register_dev(string classname, string name, std.vector<string> params_and_connections, factory.element_t felem = null)
        {
            var f = factory_().factory_by_name(classname);

            // make sure we parse macro library entries
            // FIXME: this could be done here if e.g. f
            //        would have an indication that this is macro element.
            if (f.type() == factory.element_type.MACRO)
            {
                namespace_push(name);
                include(f.name());
                namespace_pop();
            }

            string key = build_fqn(name);
            if (device_exists(key))
            {
                log().fatal.op(MF_DEVICE_ALREADY_EXISTS_1(key));
                throw new nl_exception(MF_DEVICE_ALREADY_EXISTS_1(key));
            }

            m_abstract.m_device_factory.push_back(new std.pair<string, factory.element_t>(key, f));  //m_abstract.m_device_factory.insert(m_abstract.m_device_factory.end(), {key, f});

            var paramlist = plib.pg.psplit(f.param_desc(), ",");

            if (!params_and_connections.empty())
            {
                var ptokIdx = 0;  //auto ptok(params_and_connections.begin());
                var ptok_endIdx = params_and_connections.Count;  //auto ptok_end(params_and_connections.end());

                foreach (string tp in paramlist)
                {
                    if (plib.pg.startsWith(tp, "+"))
                    {
                        if (ptokIdx == ptok_endIdx)  //if (ptok == ptok_end)
                        {
                            var err = MF_PARAM_COUNT_MISMATCH_2(name, params_and_connections.size());
                            log().fatal.op(err);
                            throw new nl_exception(err);
                            //break;
                        }

                        string output_name = params_and_connections[ptokIdx];  //pstring output_name = *ptok;
                        log().debug.op("Link: {0} {1}", tp, output_name);

                        register_link(name + "." + tp.substr(1), output_name);
                        ++ptokIdx;  //++ptok;
                    }
                    else if (plib.pg.startsWith(tp, "@"))
                    {
                        string term = tp.substr(1);
                        log().debug.op("Link: {0} {1}", tp, term);

                        register_link(name + "." + term, term);
                    }
                    else
                    {
                        if (ptokIdx == params_and_connections.Count)  //if (ptok == params_and_connections.end())
                        {
                            var err = MF_PARAM_COUNT_MISMATCH_2(name, params_and_connections.size());
                            log().fatal.op(err);
                            throw new nl_exception(err);
                        }

                        string paramfq = name + "." + tp;

                        log().debug.op("Defparam: {0}\n", paramfq);

                        register_param(paramfq, params_and_connections[ptokIdx]);  //register_param(paramfq, *ptok);

                        ++ptokIdx;  //++ptok;
                    }
                }

                if (ptokIdx != params_and_connections.Count)  //if (ptok != params_and_connections.end())
                {
                    var err = MF_PARAM_COUNT_EXCEEDED_2(name, params_and_connections.size());
                    log().fatal.op(err);
                    throw new nl_exception(err);
                }
            }

            if (felem != null)
            {
                throw new emu_unimplemented();
#if false
                *felem = f;
#endif
            }
        }


        public void register_dev(string classname, params string [] more_parameters)  //void register_dev(string classname, std::initializer_list<const char *> more_parameters);
        {
            //std::vector<pstring> params;
            //const auto *i(more_parameters.begin());
            //pstring name(*i);
            //++i;
            //for (; i != more_parameters.end(); ++i)
            //{
            //    params.emplace_back(*i);
            //}
            //register_dev(classname, name, params);
            string name = more_parameters[0];
            std.vector<string> params_ = new std.vector<string>(more_parameters.Skip(1));
            register_dev(classname, name, params_);
        }


        public void register_dev(string classname, string name)
        {
            register_dev(classname, name, new std.vector<string>());
        }


        public void register_hint(string objname, string hintname)
        {
            var name = build_fqn(objname) + hintname;
            if (!m_abstract.m_hints.insert(name, false))
            {
                log().fatal.op(MF_ADDING_HINT_1(name));
                throw new nl_exception(MF_ADDING_HINT_1(name));
            }
        }


        public void register_link(string sin, string sout)
        {
            register_link_fqn(build_fqn(plib.pg.trim(sin)), build_fqn(plib.pg.trim(sout)));
        }


        public void register_link_arr(string terms)
        {
            var list = plib.pg.psplit(terms, ", ");
            if (list.size() < 2)
            {
                log().fatal.op(MF_NET_C_NEEDS_AT_LEAST_2_TERMINAL());
                throw new nl_exception(MF_NET_C_NEEDS_AT_LEAST_2_TERMINAL());
            }

            for (size_t i = 1; i < list.size(); i++)
            {
                register_link(list[0], list[i]);
            }
        }


        // also called from devices for latebinding connected terminals
        public void register_link_fqn(string sin, string sout)
        {
            abstract_t_link_t temp = new abstract_t_link_t(sin, sout);
            log().debug.op("link {0} <== {1}", sin, sout);
            m_abstract.m_links.push_back(temp);
        }


        public void register_param(string param, string value)
        {
            string fqn = build_fqn(param);
            string val = value;

            // strip " from stringified strings
            if (plib.pg.startsWith(value, "\"") && plib.pg.endsWith(value, "\""))
                val = value.substr(1, value.length() - 2);

            // Replace "@." with the current namespace
            val = plib.pg.replace_all(val, "@.", namespace_prefix());
            var idx = m_abstract.m_param_values.find(fqn);
            if (idx == default)
            {
                if (!m_abstract.m_param_values.insert(fqn, val))
                {
                    log().fatal.op(MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(param));
                    throw new nl_exception(MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(param));
                }
            }
            else
            {
                if (idx.find("$(") == npos)
                {
                    //* There may be reason ... so make it an INFO
                    log().info.op(MI_OVERWRITING_PARAM_1_OLD_2_NEW_3(fqn, idx, val));
                }

                m_abstract.m_param_values[fqn] = val;
            }
        }


        // DEFPARAM support
        public void register_defparam(string name, string def)
        {
            // strip " from stringified strings
            string val = def;
            if (plib.pg.startsWith(def, "\"") && plib.pg.endsWith(def, "\""))
                val = def.substr(1, def.length() - 2);
            // Replace "@." with the current namespace
            val = plib.pg.replace_all(val, "@.", namespace_prefix());
            m_abstract.m_defparams.emplace_back(new std.pair<string, string>(namespace_prefix() + name, val));
        }


        //template <typename T>
        //std::enable_if_t<plib::is_arithmetic<T>::value>
        public void register_param(string param, nl_fptype value)  //register_param(const pstring &param, T value)
        {
            register_param_fp(param, value);  //register_param_fp(param, plib::narrow_cast<nl_fptype>(value));
        }


        public void register_lib_entry(string name, string def_params, plib.source_location loc)
        {
            factory_().add(new factory.library_element_t(name, new factory.properties(def_params, loc)));  //factory().add(plib::make_unique<factory::library_element_t, host_arena>(name, factory::properties(def_params, std::move(loc))));
        }


        public void register_frontier(string attach, string r_IN, string r_OUT)
        {
            string frontier_name = new plib.pfmt("frontier_{0}").op(m_frontier_cnt);
            m_frontier_cnt++;
            register_dev("FRONTIER_DEV", frontier_name);
            register_param(frontier_name + ".RIN", r_IN);
            register_param(frontier_name + ".ROUT", r_OUT);
            register_link(frontier_name + ".G", "GND");
            string attfn = build_fqn(attach);
            string front_fqn = build_fqn(frontier_name);
            bool found = false;
            for (int i = 0; i < m_abstract.m_links.Count; i++)  //for (auto & link  : m_abstract.m_links)
            {
                var link = m_abstract.m_links[i];

                if (link.first == attfn)
                {
                    m_abstract.m_links[i] = new abstract_t_link_t(front_fqn + ".I", link.second);  //link.first = front_fqn + ".I";
                    found = true;
                }
                else if (link.second == attfn)
                {
                    m_abstract.m_links[i] = new abstract_t_link_t(link.first, front_fqn + ".I");  //link.second = front_fqn + ".I";
                    found = true;
                }
            }

            if (!found)
            {
                log().fatal.op(MF_FOUND_NO_OCCURRENCE_OF_1(attach));
                throw new nl_exception(MF_FOUND_NO_OCCURRENCE_OF_1(attach));
            }

            register_link(attach, frontier_name + ".Q");
        }


        // register a source
        //template <typename S, typename... Args>
        public void register_source(plib.psource_t args)  //void register_source(Args&&... args)
        {
            m_sources.add_source(args);  //m_sources.add_source<S>(std::forward<Args>(args)...);
        }


        public void register_source_proc(string name, nlsetup_func func)
        {
            register_source(new netlist.source_proc_t(name, func));  //register_source<netlist::source_proc_t>(name, func);
        }


        public void truthtable_create(tt_desc desc, string def_params, plib.source_location loc)
        {
            var fac = factory.nlid_truthtable_global.truthtable_create(desc, new netlist.factory.properties(def_params, loc));  //auto fac = factory::truthtable_create(desc, netlist::factory.properties(def_params, std::move(loc)));
            factory_().add(fac);
        }


        // handle namespace

        // include other files

        public void include(string netlist_name)
        {
            if (m_sources.for_all<source_netlist_t>((source_netlist_t src) =>  //if (m_sources.for_all<source_netlist_t>([this, &netlist_name] (source_netlist_t *src)
            {
                return src.parse(this, netlist_name);
            }))
                return;

            log().fatal.op(MF_NOT_FOUND_IN_SOURCE_COLLECTION(netlist_name));
            throw new nl_exception(MF_NOT_FOUND_IN_SOURCE_COLLECTION(netlist_name));
        }


        // handle namespace

        public void namespace_push(string aname)
        {
            if (m_namespace_stack.empty())
                //m_namespace_stack.push(netlist().name() + "." + aname);
                m_namespace_stack.push(aname);
            else
                m_namespace_stack.push(m_namespace_stack.top() + "." + aname);
        }


        public void namespace_pop()
        {
            m_namespace_stack.pop();
        }


        // used from netlist.cpp (mame)
        public bool device_exists(string name)
        {
            foreach (var d in m_abstract.m_device_factory)
            {
                if (d.first == name)
                    return true;
            }

            return false;
        }


        // FIXME: used by source_t - need a different approach at some time
        public bool parse_stream(plib.istream_uptr istrm, string name)  //bool parse_stream(plib::istream_uptr &&istrm, const pstring &name);
        {
            var filename = istrm.filename();
            var preprocessed = new MemoryStream(  //auto preprocessed = std::make_unique<std::stringstream>(putf8string(
                    Encoding.ASCII.GetBytes(new plib.ppreprocessor(m_includes, m_defines).process(istrm, filename)));  //plib::ppreprocessor(m_includes, &m_defines).process(std::move(istrm), filename)));

            parser_t_token_store st = new parser_t_token_store();
            parser_t parser = new parser_t(this);
            parser.parse_tokens(new plib.istream_uptr(preprocessed, filename), st);
            return parser.parse(st, name);
        }


        //bool parse_tokens(const plib::detail::token_store &tokens, const pstring &name);


        //template <typename S, typename... Args>
        public void add_include(plib.psource_t args)  //void add_include(Args&&... args)
        {
            m_includes.add_source(args);  //m_includes.add_source<S>(std::forward<Args>(args)...);
        }


        //void add_define(const pstring &def, const pstring &val)
        //void add_define(const pstring &defstr);


        // DEFPARAM support
        public void defparam(string name, string def)
        {
            // strip " from stringified strings
            string val = def;
            if (plib.pg.startsWith(def, "\"") && plib.pg.endsWith(def, "\""))
                val = def.substr(1, def.length() - 2);

            // Replace "@." with the current namespace
            val = plib.pg.replace_all(val, "@.", namespace_prefix());
            m_abstract.m_defparams.emplace_back(new std.pair<string, string>(namespace_prefix() + name, val));
        }


        // register a list of logs
        public void register_dynamic_log_devices(std.vector<string> loglist)
        {
            log().debug.op("Creating dynamic logs ...");
            foreach (string ll in loglist)
            {
                string name = "log_" + ll;

                register_dev("LOG", name);
                register_link(name + ".I", ll);
            }
        }


        public factory.list_t factory_() { return m_abstract.m_factory; }


        public log_type log() { return m_log; }


        //plib::istream_uptr get_data_stream(const pstring &name);


        string namespace_prefix()
        {
            return m_namespace_stack.empty() ? "" : m_namespace_stack.top() + ".";
        }


        string build_fqn(string obj_name)
        {
            return namespace_prefix() + obj_name;
        }


        void register_param_fp(string param, nl_fptype value)
        {
            if (plib.pg.abs(value - plib.pg.floor(value)) > nlconst.magic(1e-30)
                || plib.pg.abs(value) > nlconst.magic(1e9))
                register_param(param, new plib.pfmt("{0}").op(value));
            else
                register_param(param, new plib.pfmt("{0}").op(value));
        }


        //bool device_exists(const pstring &name) const;


        // FIXME: stale? - remove later
        //void remove_connections(const pstring &pin);
    }


    // FIXME: all this belongs elsewhere

    public enum family_type  //PENUM(family_type,
    {
        CUSTOM,
        TTL,
        MOS,
        CMOS,
        NMOS,
        PMOS
    }

    class logic_family_std_proxy_t : logic_family_desc_t
    {
        family_type m_family_type;


        public logic_family_std_proxy_t(family_type ft)
        {
            m_family_type = ft;
        }


        // FIXME: create proxies based on family type (far future)
        public override devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_state_t anetlist, string name, logic_output_t proxied)  //device_arena::unique_ptr<devices::nld_base_d_to_a_proxy> create_d_a_proxy(netlist_state_t &anetlist, const pstring &name, const logic_output_t *proxied) const override
        {
            switch (m_family_type)
            {
                case family_type.CUSTOM:
                case family_type.TTL:
                case family_type.MOS:
                case family_type.CMOS:
                case family_type.NMOS:
                case family_type.PMOS:
                    return new devices.nld_d_to_a_proxy(anetlist, name, proxied);  //return anetlist.make_pool_object<devices::nld_d_to_a_proxy>(anetlist, name, proxied);
            }

            return new devices.nld_d_to_a_proxy(anetlist, name, proxied);  //return anetlist.make_pool_object<devices::nld_d_to_a_proxy>(anetlist, name, proxied);
        }


        public override devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_state_t anetlist, string name, logic_input_t proxied)  //device_arena::unique_ptr<devices::nld_base_a_to_d_proxy> create_a_d_proxy(netlist_state_t &anetlist, const pstring &name, const logic_input_t *proxied) const override
        {
            switch (m_family_type)
            {
                case family_type.CUSTOM:
                case family_type.TTL:
                case family_type.MOS:
                case family_type.CMOS:
                case family_type.NMOS:
                case family_type.PMOS:
                    return new devices.nld_a_to_d_proxy(anetlist, name, proxied);  //return anetlist.make_pool_object<devices::nld_a_to_d_proxy>(anetlist, name, proxied);
            }

            return new devices.nld_a_to_d_proxy(anetlist, name, proxied);  //return anetlist.make_pool_object<devices::nld_a_to_d_proxy>(anetlist, name, proxied);
        }
    }


    /// \brief Class representing the logic families.
    ///
    ///  This is the model representation of the logic families. This is a
    ///  netlist specific model. Examples give values for TTL family
    ///
    //
    ///   |NL? |name  |parameter                                                  |units| TTL   |
    ///   |:--:|:-----|:----------------------------------------------------------|:----|------:|
    ///   | Y  |IVL   |Input voltage low threshold relative to supply voltage     |     |1.0e-14|
    ///   | Y  |IVH   |Input voltage high threshold relative to supply voltage    |     |      0|
    ///   | Y  |OVL   |Output voltage minimum voltage relative to supply voltage  |     |1.0e-14|
    ///   | Y  |OVL   |Output voltage maximum voltage relative to supply voltage  |     |1.0e-14|
    ///   | Y  |ORL   |Output output resistance for logic 0                       |     |      0|
    ///   | Y  |ORH   |Output output resistance for logic 1                       |     |      0|
    ///
    class family_model_t
    {
        public param_model_t_value_str_t m_TYPE; //!< Family type (TTL, CMOS, ...)
        public param_model_t_value_t m_IVL;      //!< Input voltage low threshold relative to supply voltage
        public param_model_t_value_t m_IVH;      //!< Input voltage high threshold relative to supply voltage
        public param_model_t_value_t m_OVL;      //!< Output voltage minimum voltage relative to supply voltage
        public param_model_t_value_t m_OVH;      //!< Output voltage maximum voltage relative to supply voltage
        public param_model_t_value_t m_ORL;      //!< Output output resistance for logic 0
        public param_model_t_value_t m_ORH;      //!< Output output resistance for logic 1


        //template <typename P>
        public family_model_t(param_value_interface model)
        {
            m_TYPE = new param_model_t_value_str_t(model, "TYPE");
            m_IVL = new param_model_t_value_t(model, "IVL");
            m_IVH = new param_model_t_value_t(model, "IVH");
            m_OVL = new param_model_t_value_t(model, "OVL");
            m_OVH = new param_model_t_value_t(model, "OVH");
            m_ORL = new param_model_t_value_t(model, "ORL");
            m_ORH = new param_model_t_value_t(model, "ORH");
        }
    }
}
