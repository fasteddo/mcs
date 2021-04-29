// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using abstract_t_link_t = mame.std.pair<string, string>;  //using link_t = std::pair<pstring, pstring>;
using log_type = mame.plib.plog_base<mame.netlist.nl_config_global.bool_constant_NL_DEBUG>;  //using log_type =  plib::plog_base<NL_DEBUG>;
using models_t_map_t = mame.std.unordered_map<string, string>;  //using map_t = std::unordered_map<pstring, pstring>;
using models_t_raw_map_t = mame.std.unordered_map<string, string>;  //using raw_map_t = std::unordered_map<pstring, pstring>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;
using param_model_t_value_str_t = mame.netlist.param_model_t.value_base_t<string, mame.netlist.param_model_t.value_base_t_operators_string>;  //using value_str_t = value_base_t<pstring>;
using param_model_t_value_t = mame.netlist.param_model_t.value_base_t<System.Double, mame.netlist.param_model_t.value_base_t_operators_double>;  //using value_t = value_base_t<nl_fptype>;
using size_t = System.UInt32;
using unsigned = System.UInt32;


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
        public static void NET_REGISTER_DEVEXT(nlparse_t setup, string type, params string [] args)
        {
            throw new emu_unimplemented();
#if false
            setup.register_dev(type, args);
#endif
        }

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
        public static void NETLIST_START() {}

        //#define NETLIST_END()  }
        public static void NETLIST_END() {}

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

        //#define EXTERNAL_LIB_ENTRY(...) PCALLVARARG(LOCAL_LIB_ENTRY_, EXTERNAL, __VA_ARGS__)

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


        // -----------------------------------------------------------------------------
        // truthtable defines
        // -----------------------------------------------------------------------------

        static netlist.tt_desc TRUTHTABLE_desc;

        //#define TRUTHTABLE_START(cname, in, out, pdef_params)                           \
        //        NETLIST_START(cname) \
        //        netlist::tt_desc desc;                                                 \
        //        desc.name = #cname ;                                                   \
        //        desc.ni = in;                                                          \
        //        desc.no = out;                                                         \
        //        desc.family = "";                                                      \
        //        auto sloc = PSOURCELOC();                                              \
        //        const pstring def_params = pdef_params;
        public static void TRUTHTABLE_START(string cname, UInt32 in_, UInt32 out_, string def_params)
        {
            netlist.tt_desc desc = new netlist.tt_desc();
            desc.name = cname;
            desc.ni = in_;
            desc.no = out_;
            desc.family = "";

            // ref original above
            throw new emu_unimplemented();
#if false
#endif

            TRUTHTABLE_desc = desc;
        }

        //#define TT_HEAD(x) \
        //        desc.desc.emplace_back(x);
        public static void TT_HEAD(string x) { TRUTHTABLE_desc.desc.emplace_back(x); }

        //#define TT_LINE(x) \
        //        desc.desc.emplace_back(x);
        public static void TT_LINE(string x) { TRUTHTABLE_desc.desc.emplace_back(x); }

        //#define TT_FAMILY(x) \
        //        desc.family = x;
        public static void TT_FAMILY(string x) { TRUTHTABLE_desc.family = x; }

        //#define TRUTHTABLE_END() \
        //        setup.truthtable_create(desc, def_params, std::move(sloc)); \
        //        NETLIST_END()
        public static void TRUTHTABLE_END(nlparse_t setup)
        {
            // ref original above
            throw new emu_unimplemented();
#if false
            TRUTHTABLE_desc = null;
#endif
        }

        //#define TRUTHTABLE_ENTRY(name)                                                 \
        //    LOCAL_SOURCE(name)                                                         \
        //    INCLUDE(name)
    }


    // -----------------------------------------------------------------------------
    // truthtable desc
    // -----------------------------------------------------------------------------
    public class tt_desc
    {
        public string name;
        public UInt32 ni;  //unsigned long ni;
        public UInt32 no;  //unsigned long no;
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
        //plib::ppreprocessor.defines_map_type       m_defines;
        //plib::psource_collection_t                  m_includes;
        std.stack<string> m_namespace_stack;
        plib.psource_collection_t m_sources;
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
            if (pos == -1)
                throw new nl_exception(nl_errstr_global.MF_UNABLE_TO_PARSE_MODEL_1(model_in));

            string model = plib.pglobal.ucase(plib.pglobal.trim(plib.pglobal.left(model_in, pos)));
            string def = plib.pglobal.trim(model_in.substr(pos + 1));
            if (!m_abstract.m_models.insert(model, def))
            {
                // FIXME: Add an directive MODEL_OVERWRITE to netlist language
                //throw nl_exception(MF_MODEL_ALREADY_EXISTS_1(model_in));
                log().info.op(nl_errstr_global.MI_MODEL_OVERWRITE_1(model, model_in));
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
                log().fatal.op(nl_errstr_global.MF_ALIAS_ALREAD_EXISTS_1(alias));
                throw new nl_exception(nl_errstr_global.MF_ALIAS_ALREAD_EXISTS_1(alias));
            }
        }


        public void register_dip_alias_arr(string terms)
        {
            var list = plib.pglobal.psplit(terms, ", ");
            if (list.empty() || (list.size() % 2) == 1)
            {
                log().fatal.op(nl_errstr_global.MF_DIP_PINS_MUST_BE_AN_EQUAL_NUMBER_OF_PINS_1(build_fqn("")));
                throw new nl_exception(nl_errstr_global.MF_DIP_PINS_MUST_BE_AN_EQUAL_NUMBER_OF_PINS_1(build_fqn("")));
            }

            int n = list.size();
            for (int i = 0; i < n / 2; i++)
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
                log().fatal.op(nl_errstr_global.MF_DEVICE_ALREADY_EXISTS_1(key));
                throw new nl_exception(nl_errstr_global.MF_DEVICE_ALREADY_EXISTS_1(key));
            }

            m_abstract.m_device_factory.push_back(new std.pair<string, factory.element_t>(key, f));  //m_abstract.m_device_factory.insert(m_abstract.m_device_factory.end(), {key, f});

            var paramlist = plib.pglobal.psplit(f.param_desc(), ",");

            if (!params_and_connections.empty())
            {
                var ptokIdx = 0;  //auto ptok(params_and_connections.begin());
                var ptok_endIdx = params_and_connections.Count;  //auto ptok_end(params_and_connections.end());

                foreach (string tp in paramlist)
                {
                    if (plib.pglobal.startsWith(tp, "+"))
                    {
                        if (ptokIdx == ptok_endIdx)  //if (ptok == ptok_end)
                        {
                            var err = nl_errstr_global.MF_PARAM_COUNT_MISMATCH_2(name, params_and_connections.size());
                            log().fatal.op(err);
                            throw new nl_exception(err);
                            //break;
                        }

                        string output_name = params_and_connections[ptokIdx];  //pstring output_name = *ptok;
                        log().debug.op("Link: {0} {1}", tp, output_name);

                        register_link(name + "." + tp.substr(1), output_name);
                        ++ptokIdx;  //++ptok;
                    }
                    else if (plib.pglobal.startsWith(tp, "@"))
                    {
                        string term = tp.substr(1);
                        log().debug.op("Link: {0} {1}", tp, term);

                        register_link(name + "." + term, term);
                    }
                    else
                    {
                        if (ptokIdx == params_and_connections.Count)  //if (ptok == params_and_connections.end())
                        {
                            var err = nl_errstr_global.MF_PARAM_COUNT_MISMATCH_2(name, params_and_connections.size());
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
                    var err = nl_errstr_global.MF_PARAM_COUNT_EXCEEDED_2(name, params_and_connections.size());
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


        //void register_dev(string classname, std::initializer_list<const char *> more_parameters);


        public void register_dev(string classname, string name)
        {
            register_dev(classname, name, new std.vector<string>());
        }


        public void register_hint(string objname, string hintname)
        {
            var name = build_fqn(objname) + hintname;
            if (!m_abstract.m_hints.insert(name, false))
            {
                log().fatal.op(nl_errstr_global.MF_ADDING_HINT_1(name));
                throw new nl_exception(nl_errstr_global.MF_ADDING_HINT_1(name));
            }
        }


        public void register_link(string sin, string sout)
        {
            register_link_fqn(build_fqn(plib.pglobal.trim(sin)), build_fqn(plib.pglobal.trim(sout)));
        }


        public void register_link_arr(string terms)
        {
            var list = plib.pglobal.psplit(terms, ", ");
            if (list.size() < 2)
            {
                log().fatal.op(nl_errstr_global.MF_NET_C_NEEDS_AT_LEAST_2_TERMINAL());
                throw new nl_exception(nl_errstr_global.MF_NET_C_NEEDS_AT_LEAST_2_TERMINAL());
            }

            for (int i = 1; i < list.size(); i++)
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
            if (plib.pglobal.startsWith(value, "\"") && plib.pglobal.endsWith(value, "\""))
                val = value.substr(1, value.length() - 2);

            // Replace "@." with the current namespace
            val = plib.pglobal.replace_all(val, "@.", namespace_prefix());
            var idx = m_abstract.m_param_values.find(fqn);
            if (idx == default)
            {
                if (!m_abstract.m_param_values.insert(fqn, val))
                {
                    log().fatal.op(nl_errstr_global.MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(param));
                    throw new nl_exception(nl_errstr_global.MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(param));
                }
            }
            else
            {
                if (idx.find("$(") == -1)
                {
                    //* There may be reason ... so make it an INFO
                    log().info.op(nl_errstr_global.MI_OVERWRITING_PARAM_1_OLD_2_NEW_3(fqn, idx, val));
                }

                m_abstract.m_param_values[fqn] = val;
            }
        }


        // DEFPARAM support
        public void register_defparam(string name, string def)
        {
            // strip " from stringified strings
            string val = def;
            if (plib.pglobal.startsWith(def, "\"") && plib.pglobal.endsWith(def, "\""))
                val = def.substr(1, def.length() - 2);
            // Replace "@." with the current namespace
            val = plib.pglobal.replace_all(val, "@.", namespace_prefix());
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


        //void register_frontier(const pstring &attach, const pstring &r_IN, const pstring &r_OUT);


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


        void truthtable_create(tt_desc desc, string def_params, plib.source_location loc)
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

            log().fatal.op(nl_errstr_global.MF_NOT_FOUND_IN_SOURCE_COLLECTION(netlist_name));
            throw new nl_exception(nl_errstr_global.MF_NOT_FOUND_IN_SOURCE_COLLECTION(netlist_name));
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
            throw new emu_unimplemented();
        }


        //bool parse_tokens(const plib::detail::token_store &tokens, const pstring &name);


        //template <typename S, typename... Args>
        //void add_include(Args&&... args)
        //void add_define(const pstring &def, const pstring &val)
        //void add_define(const pstring &defstr);


        // DEFPARAM support
        public void defparam(string name, string def)
        {
            // strip " from stringified strings
            string val = def;
            if (plib.pglobal.startsWith(def, "\"") && plib.pglobal.endsWith(def, "\""))
                val = def.substr(1, def.length() - 2);

            // Replace "@." with the current namespace
            val = plib.pglobal.replace_all(val, "@.", namespace_prefix());
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
            return (m_namespace_stack.empty() ? "" : m_namespace_stack.top() + ".");
        }


        string build_fqn(string obj_name)
        {
            throw new emu_unimplemented();
        }


        void register_param_fp(string param, nl_fptype value)
        {
            throw new emu_unimplemented();
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
        public family_model_t(models_t.model_t model)
        {
            throw new emu_unimplemented();
#if false
            m_TYPE = new param_model_t.value_str_t(model, "TYPE");
            m_IVL = new param_model_t.value_t(model, "IVL");
            m_IVH = new param_model_t.value_t(model, "IVH");
            m_OVL = new param_model_t.value_t(model, "OVL");
            m_OVH = new param_model_t.value_t(model, "OVH");
            m_ORL = new param_model_t.value_t(model, "ORL");
            m_ORH = new param_model_t.value_t(model, "ORH");
#endif
        }
    }
}
