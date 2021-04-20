// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using link_t = System.Collections.Generic.KeyValuePair<string, string>;  //using link_t = std::pair<pstring, pstring>;
using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;//, NL_DEBUG>;
using models_t_map_t = mame.std.unordered_map<string, string>;
using models_t_raw_map_t = mame.std.unordered_map<string, string>;
using nl_fptype = System.Double;
using size_t = System.UInt32;
using unsigned = System.UInt32;


namespace mame.netlist
{
    public static class nl_setup_global
    {
        //============================================================
        //  MACROS / inline netlist definitions
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
        //        setup.defparam(NET_STR(name), NET_STR(val));
        public static void DEFPARAM(nlparse_t setup, string name, string val) { setup.defparam(name, val); }

        //#define HINT(name, val)                                                        \
        //        setup.register_param(# name ".HINT_" # val, "1");

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
        //        setup.register_source<netlist::source_proc_t>(# name, &NETLIST_NAME(name));
        public static void LOCAL_SOURCE(nlparse_t setup, string name, source_proc_t.setup_func_delegate netlist_name) { setup.register_source(new netlist.source_proc_t(name, netlist_name)); }

        // FIXME: Need to pass in parameter definition
        //#define LOCAL_LIB_ENTRY_1(name)                                                  \
        //        LOCAL_SOURCE(name)                                                     \
        //setup.register_lib_entry(# name,                                       \
        //    netlist::factory::properties("", PSOURCELOC()));
        public static void LOCAL_LIB_ENTRY_1(nlparse_t setup, string name, source_proc_t.setup_func_delegate nld_name)
        {
            LOCAL_SOURCE(setup, name, nld_name);
            setup.register_lib_entry(name, new netlist.factory.properties("", plib.pglobal.PSOURCELOC()));
        }

        //#define LOCAL_LIB_ENTRY_2(name, param_spec)                                    \
        //        LOCAL_SOURCE(name)                                                     \
        //        setup.register_lib_entry(# name,                                       \
        //            netlist::factory::properties(param_spec, PSOURCELOC()));

        //#define LOCAL_LIB_ENTRY(...) PCALLVARARG(LOCAL_LIB_ENTRY_, __VA_ARGS__)
        public static void LOCAL_LIB_ENTRY(nlparse_t setup, string name, source_proc_t.setup_func_delegate nld_name) { LOCAL_LIB_ENTRY_1(setup, name, nld_name); }

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

        //#define TRUTHTABLE_START(cname, in, out, def_params)                           \
        //    { \
        //        netlist::tt_desc desc;                                                 \
        //        desc.name = #cname ;                                                   \
        //        desc.ni = in;                                                          \
        //        desc.no = out;                                                         \
        //        desc.family = "";                                                      \
        //        netlist::factory::properties props(def_params, PSOURCELOC());
        public static void TRUTHTABLE_START(string cname, UInt32 in_, UInt32 out_, string def_params)
        {
            netlist.tt_desc desc = new netlist.tt_desc();
            desc.name = cname;
            desc.ni = in_;
            desc.no = out_;
            desc.family = "";

            // save this in global var?
            throw new emu_unimplemented();
#if false
            netlist.factory.properties props = new netlist.factory.properties(def_params, plib.pglobal.PSOURCELOC());
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
        //        setup.truthtable_create(desc, std::move(props)); \
        //    }
        public static void TRUTHTABLE_END(nlparse_t setup)
        {
            throw new emu_unimplemented();
#if false
            setup.truthtable_create(TRUTHTABLE_desc, props);
            TRUTHTABLE_desc = null;
#endif
        }
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


    // -----------------------------------------------------------------------------
    // param_ref_t
    // -----------------------------------------------------------------------------
    public class param_ref_t
    {
        core_device_t m_device;
        param_t m_param;


        public param_ref_t()
        {
            m_device = null;
            m_param = null;
        }


        public param_ref_t(core_device_t device, param_t param)
        {
            m_device = device;
            m_param = param;
        }


        //const pstring &name() const noexcept { return m_name; }
        //const core_device_t &device() const noexcept { return m_device; }
        public param_t param() { return m_param; }
        //bool is_valid() const noexcept { return (m_device != nullptr) && (m_param != nullptr); }
    }


    // ----------------------------------------------------------------------------------------
    // Specific netlist psource_t implementations
    // ----------------------------------------------------------------------------------------
    public abstract class source_netlist_t : plib.psource_t
    {
        //source_netlist_t() = default;
        //PCOPYASSIGNMOVE(source_netlist_t, delete)
        //virtual ~source_netlist_t() noexcept = default;

        public virtual bool parse(nlparse_t setup, string name)
        {
            var strm = stream(name);
            return strm != null ? setup.parse_stream(strm, name) : false;
        }
    }


    abstract class source_data_t : plib.psource_t
    {
        //source_data_t() = default;
        //PCOPYASSIGNMOVE(source_data_t, delete)
        //virtual ~source_data_t() noexcept = default;
    }


    // ----------------------------------------------------------------------------------------
    // Collection of models
    // ----------------------------------------------------------------------------------------
    public class models_t
    {
        //using raw_map_t = std::unordered_map<pstring, pstring>;
        //using map_t = std::unordered_map<pstring, pstring>;


        public class model_t
        {
            string m_model; // only for error messages
            models_t_map_t m_map;


            public model_t(string model, models_t_map_t map)
            {
                m_model = model;
                m_map = map;
            }


            public string value_str(string entity)
            {
                if (entity != plib.pglobal.ucase(entity))
                    throw new nl_exception(nl_errstr_global.MF_MODEL_PARAMETERS_NOT_UPPERCASE_1_2(entity, model_string(m_map)));

                var it = m_map.find(entity);
                if (it == default)
                    throw new nl_exception(nl_errstr_global.MF_ENTITY_1_NOT_FOUND_IN_MODEL_2(entity, model_string(m_map)));

                return it;  //it->second;
            }


            public nl_fptype value(string entity)
            {
                string tmp = value_str(entity);

                nl_fptype factor = nlconst.one();
                var p = tmp[tmp.Length - 1];  //auto p = std::next(tmp.begin(), plib::narrow_cast<pstring::difference_type>(tmp.size() - 1));
                switch (p)
                {
                    case 'M': factor = nlconst.magic(1e6); break; // NOLINT
                    case 'k':
                    case 'K': factor = nlconst.magic(1e3); break; // NOLINT
                    case 'm': factor = nlconst.magic(1e-3); break; // NOLINT
                    case 'u': factor = nlconst.magic(1e-6); break; // NOLINT
                    case 'n': factor = nlconst.magic(1e-9); break; // NOLINT
                    case 'p': factor = nlconst.magic(1e-12); break; // NOLINT
                    case 'f': factor = nlconst.magic(1e-15); break; // NOLINT
                    case 'a': factor = nlconst.magic(1e-18); break; // NOLINT
                    default:
                        if (p < '0' || p > '9')
                            throw new nl_exception(nl_errstr_global.MF_UNKNOWN_NUMBER_FACTOR_IN_2(m_model, entity));
                        break;
                }

                if (factor != nlconst.one())
                    tmp = plib.pglobal.left(tmp, tmp.size() - 1);

                // FIXME: check for errors
                bool err = false;
                var val = plib.pglobal.pstonum_ne_nl_fptype(false, tmp, out err);
                if (err)
                    throw new nl_exception(nl_errstr_global.MF_MODEL_NUMBER_CONVERSION_ERROR(entity, tmp, "double", m_model));

                return val * factor;
            }


            //pstring type() const { return value_str("COREMODEL"); }


            static string model_string(models_t_map_t map)
            {
                // operator [] has no const implementation
                string ret = map.at("COREMODEL") + "(";
                foreach (var i in map)
                    ret += (i.first() + '=' + i.second() + ' ');

                return ret + ")";
            }
        }


        models_t_raw_map_t m_models;
        std.unordered_map<string, models_t_map_t> m_cache;


        public models_t(models_t_raw_map_t models)
        {
            m_models = models;
        }


        public model_t get_model(string model)
        {
            models_t_map_t map = m_cache[model];

            if (map.empty())
                model_parse(model, map);

            return new model_t(model, map);
        }


        void model_parse(string model_in, models_t_map_t map)
        {
            string model = model_in;
            int pos = 0;
            string key = "";

            while (true)
            {
                pos = model.find('(');
                if (pos != -1) break;

                key = plib.pglobal.ucase(model);
                var i = m_models.find(key);
                if (i == null)
                    throw new nl_exception(nl_errstr_global.MF_MODEL_NOT_FOUND("xx" + model));

                model = i;
            }

            string xmodel = plib.pglobal.left(model, pos);

            if (xmodel == "_")
            {
                map["COREMODEL"] = key;
            }
            else
            {
                var i = m_models.find(xmodel);
                if (i != null)
                    model_parse(xmodel, map);
                else
                    throw new nl_exception(nl_errstr_global.MF_MODEL_NOT_FOUND(model_in));
            }

            string remainder = plib.pglobal.trim(model.substr(pos + 1));
            if (!plib.pglobal.endsWith(remainder, ")"))
                throw new nl_exception(nl_errstr_global.MF_MODEL_ERROR_1(model));
            // FIMXE: Not optimal
            remainder = plib.pglobal.left(remainder, remainder.Length - 1);

            var pairs = remainder.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);  //(plib::psplit(remainder," ", true));
            foreach (string pe in pairs)
            {
                var pose = pe.find('=');
                if (pose == -1)
                    throw new nl_exception(nl_errstr_global.MF_MODEL_ERROR_ON_PAIR_1(model));

                map[plib.pglobal.ucase(plib.pglobal.left(pe, pose))] = pe.substr(pose + 1);
            }
        }
    }


    // ----------------------------------------------------------------------------------------
    // nlparse_t
    // ----------------------------------------------------------------------------------------
    public class nlparse_t
    {
        //using link_t = std::pair<pstring, pstring>;


        public struct abstract_t
        {
            public std.unordered_map<string, string> m_alias;
            public std.vector<link_t> m_links;
            public std.unordered_map<string, string> m_param_values;
            public models_t_raw_map_t m_models;  //models_t::raw_map_t                         m_models;

            // need to preserve order of device creation ...
            public std.vector<std.pair<string, factory.element_t>> m_device_factory;  //std::vector<std::pair<pstring, factory::element_t *>> m_device_factory;
            // lifetime control only - can be cleared before run
            public std.vector<std.pair<string, string>> m_defparams;
        }


        //plib::ppreprocessor.defines_map_type       m_defines;
        //plib::psource_collection_t<>                m_includes;
        std.stack<string> m_namespace_stack;
        plib.psource_collection_t m_sources;  //plib::psource_collection_t<>                m_sources;
        // FIXME: convert to hash and deal with sorting in nltool
        factory.list_t m_factory;
        abstract_t m_abstract;

        log_type m_log;
        unsigned m_frontier_cnt;


        public nlparse_t(log_type log, abstract_t abstract_)
        {
            m_factory = new factory.list_t(log);
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
                throw new nl_exception(nl_errstr_global.MF_MODEL_ALREADY_EXISTS_1(model_in));
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
                log().fatal.op(nl_errstr_global.MF_ADDING_ALI1_TO_ALIAS_LIST(alias));
                throw new nl_exception(nl_errstr_global.MF_ADDING_ALI1_TO_ALIAS_LIST(alias));
            }
        }


        public void register_dip_alias_arr(string terms)
        {
            std.vector<string> list = plib.pglobal.psplit(terms, ", ");
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
            var f = m_factory.factory_by_name(classname);

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
                log().fatal.op(nl_errstr_global.MF_DEVICE_ALREADY_EXISTS_1(name));
                throw new nl_exception(nl_errstr_global.MF_DEVICE_ALREADY_EXISTS_1(name));
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
                        log().debug.op("Link: {0} {1}\n", tp, output_name);

                        register_link(name + "." + tp.substr(1), output_name);
                        ++ptokIdx;  //++ptok;
                    }
                    else if (plib.pglobal.startsWith(tp, "@"))
                    {
                        string term = tp.substr(1);
                        log().debug.op("Link: {0} {1}\n", tp, term);

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


        public void register_link(string sin, string sout)
        {
            register_link_fqn(build_fqn(sin), build_fqn(sout));
        }


        public void register_link_arr(string terms)
        {
            std.vector<string> list = new std.vector<string>(terms.Split(new string[] { ", " }, StringSplitOptions.None));  //plib::psplit(terms, ", ");
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
            link_t temp = new link_t(sin, sout);
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


        void register_param(string param, nl_fptype value)
        {
            if (plib.pglobal.abs(value - plib.pglobal.floor(value)) > nlconst.magic(1e-30)
                || plib.pglobal.abs(value) > nlconst.magic(1e9))
                register_param(param, new plib.pfmt("{0}").op(value));  //register_param(param, plib::pfmt("{1:.9}").e(value));
            else
                register_param(param, new plib.pfmt("{0}").op(value));
        }


        //template <typename T>
        //std::enable_if_t<plib::is_arithmetic<T>::value>
        public void register_param_val(string param, nl_fptype value)  //register_param(const pstring &param, T value)
        {
            register_param(param, value);  //register_param(param, plib::narrow_cast<nl_fptype>(value));
        }


        public void register_lib_entry(string name, factory.properties props)
        {
            m_factory.add(new factory.library_element_t(name, props));  //m_factory.add(plib::make_unique<factory::library_element_t, host_arena>(name, std::move(props)));
        }


        //void register_frontier(const pstring &attach, const pstring &r_IN, const pstring &r_OUT);


        // register a source
        //template <typename S, typename... Args>
        public void register_source(plib.psource_t args)  //void register_source(Args&&... args)
        {
            //throw new emu_unimplemented();
#if false
            static_assert(std::is_base_of<plib::psource_t, S>::value, "S must inherit from plib::psource_t");
#endif

            var src = args;  //auto src(std::make_unique<S>(std::forward<Args>(args)...));
            m_sources.add_source(src);  //m_sources.add_source(std::move(src));
        }


        public void truthtable_create(tt_desc desc, factory.properties props)
        {
            var fac = factory.nlid_truthtable_global.truthtable_create(desc, props);
            m_factory.add(fac);
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


        string namespace_prefix()
        {
            return m_namespace_stack.empty() ? "" : m_namespace_stack.top() + ".";
        }


        string build_fqn(string obj_name)
        {
            return namespace_prefix() + obj_name;
        }


        // include other files

        public void include(string netlist_name)
        {
            if (m_sources.for_all((src) =>  //if (m_sources.for_all<source_netlist_t>([this, &netlist_name] (source_netlist_t *src)
            {
                return src.parse(this, netlist_name);
            }))
                return;

            log().fatal.op(nl_errstr_global.MF_NOT_FOUND_IN_SOURCE_COLLECTION(netlist_name));
            throw new nl_exception(nl_errstr_global.MF_NOT_FOUND_IN_SOURCE_COLLECTION(netlist_name));
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
        public bool parse_stream(std.istream istrm, string name)  //bool nlparse_t::parse_stream(plib::psource_t::stream_ptr &&istrm, const pstring &name)
        {
            throw new emu_unimplemented();
        }


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


        public factory.list_t factory_() { return m_factory; }


        public log_type log() { return m_log; }


        //plib::psource_t.stream_ptr get_data_stream(string name);


        // FIXME: stale? - remove later
        //void remove_connections(const pstring &pin);
    }


    // ----------------------------------------------------------------------------------------
    // setup_t
    // ----------------------------------------------------------------------------------------
    public class setup_t
    {
        nlparse_t.abstract_t m_abstract;
        nlparse_t m_parser;
        netlist_state_t m_nlstate;

        models_t m_models;

        // FIXME: currently only used during setup
        devices.nld_netlistparams m_netlist_params;

        // FIXME: can be cleared before run
        std.unordered_map<string, detail.core_terminal_t> m_terminals;
        std.unordered_map<terminal_t, terminal_t> m_connected_terminals;
        std.unordered_map<string, param_ref_t> m_params;
        std.unordered_map<detail.core_terminal_t, devices.nld_base_proxy> m_proxies;
        std.vector<param_t> m_defparam_lifetime;  //std::vector<host_arena::unique_ptr<param_t>>           m_defparam_lifetime;

        unsigned m_proxy_cnt;


        public setup_t(netlist_state_t nlstate)
        {
            m_abstract = new nlparse_t.abstract_t();
            m_parser = new nlparse_t(nlstate.log(), m_abstract);
            m_nlstate = nlstate;
            m_models = new models_t(m_abstract.m_models); // FIXME : parse abstract_t only
            m_netlist_params = null;
            m_proxy_cnt = 0;
        }

        //~setup_t() { }

        //PCOPYASSIGNMOVE(setup_t, delete)


        // called from param_t creation
        public void register_param_t(param_t param)
        {
            if (!m_params.insert(param.name(), new param_ref_t(param.device(), param)))
            {
                log().fatal.op(nl_errstr_global.MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(param.name()));
                throw new nl_exception(nl_errstr_global.MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(param.name()));
            }
        }


        public string get_initial_param_val(string name, string def)
        {
            var i = m_abstract.m_param_values.find(name);
            var found_pat = false;
            string v = (i == default) ? def : i;

            do
            {
                found_pat = false;
                var sp = (plib.pglobal.psplit(v, new std.vector<string>(new string [] { "$(", ")" })));
                size_t p = 0;
                v = "";
                while (p < sp.size())
                {
                    if (sp[p] == "$(")
                    {
                        p++;
                        string r = "";
                        while (p < sp.size() && sp[p] != ")")
                            r += sp[p++];

                        p++;
                        var k = m_params.find(r);
                        if (k != default)
                        {
                            v = v + k.param().valstr();
                            found_pat = true;
                        }
                        else
                        {
                            // pass - on
                            v = v + "$(" + r + ")";
                        }
                    }
                    else
                    {
                        v += sp[p++];
                    }
                }
            } while (found_pat);

            return v;
        }


        public void register_term(detail.core_terminal_t term)
        {
            log().debug.op("{0} {1}\n", termtype_as_str(term), term.name());
            if (!m_terminals.insert(term.name(), term))
            {
                log().fatal.op(nl_errstr_global.MF_ADDING_1_2_TO_TERMINAL_LIST(termtype_as_str(term), term.name()));
                throw new nl_exception(nl_errstr_global.MF_ADDING_1_2_TO_TERMINAL_LIST(termtype_as_str(term), term.name()));
            }
        }


        public void register_term(terminal_t term, terminal_t other_term)
        {
            this.register_term(term);
            m_connected_terminals.insert(term, other_term);
        }


        // called from net_splitter
        public terminal_t get_connected_terminal(terminal_t term)
        {
            var ret = m_connected_terminals.find(term);
            return ret != null ? ret : null;
        }


        // get family -> truthtable
        public logic_family_desc_t family_from_model(string model)
        {
            family_type ft = family_type.CUSTOM;

            var mod = m_models.get_model(model);
            family_model_t modv = new family_model_t(mod);

            if (!plib.penum_base.set_from_string(modv.m_TYPE.op(), out ft))  //if (!ft.set_from_string(modv.m_TYPE()))
                throw new nl_exception(nl_errstr_global.MF_UNKNOWN_FAMILY_TYPE_1(modv.m_TYPE.op(), model));

            var it = m_nlstate.family_cache().find(model);
            if (it != default)
                return it;

            var ret = new logic_family_std_proxy_t(ft);  //auto ret = plib::make_unique<logic_family_std_proxy_t, host_arena>(ft);

            ret.m_low_thresh_PCNT = modv.m_IVL.op();
            ret.m_high_thresh_PCNT = modv.m_IVH.op();
            ret.m_low_VO = modv.m_OVL.op();
            ret.m_high_VO = modv.m_OVH.op();
            ret.m_R_low = modv.m_ORL.op();
            ret.m_R_high = modv.m_ORH.op();

            var retp = ret;

            m_nlstate.family_cache().emplace(model, ret);

            return retp;
        }


        // FIXME: return param_ref_t
        public param_ref_t find_param(string param_in)
        {
            string outname = resolve_alias(param_in);
            var ret = m_params.find(outname);
            if (ret == default)
            {
                log().fatal.op(nl_errstr_global.MF_PARAMETER_1_2_NOT_FOUND(param_in, outname));
                throw new nl_exception(nl_errstr_global.MF_PARAMETER_1_2_NOT_FOUND(param_in, outname));
            }

            return ret;
        }


        // needed by nltool
        //std::vector<pstring> get_terminals_for_device_name(const pstring &devname);


        // needed by proxy device to check power terminals
        detail.core_terminal_t find_terminal(string terminal_in,
                detail.terminal_type atype, bool required = true)
        {
            string tname = resolve_alias(terminal_in);
            var ret = m_terminals.find(tname);
            // look for default
            if (ret == null && atype == detail.terminal_type.OUTPUT)
            {
                // look for ".Q" std output
                ret = m_terminals.find(tname + ".Q");
            }

            if (ret == null && required)
            {
                log().fatal.op(nl_errstr_global.MF_TERMINAL_1_2_NOT_FOUND(terminal_in, tname));
                throw new nl_exception(nl_errstr_global.MF_TERMINAL_1_2_NOT_FOUND(terminal_in, tname));
            }

            detail.core_terminal_t term = ret == null ? null : ret;

            if (term != null && term.type() != atype)
            {
                if (required)
                {
                    log().fatal.op(nl_errstr_global.MF_OBJECT_1_2_WRONG_TYPE(terminal_in, tname));
                    throw new nl_exception(nl_errstr_global.MF_OBJECT_1_2_WRONG_TYPE(terminal_in, tname));
                }

                term = null;
            }

            if (term != null)
                log().debug.op("Found input {0}\n", tname);

            return term;
        }


        public detail.core_terminal_t find_terminal(string terminal_in, bool required = true)
        {
            string tname = resolve_alias(terminal_in);
            var ret = m_terminals.find(tname);
            // look for default
            if (ret == null)
            {
                // look for ".Q" std output
                ret = m_terminals.find(tname + ".Q");
            }

            detail.core_terminal_t term = (ret == null ? null : ret);

            if (term == null && required)
            {
                log().fatal.op(nl_errstr_global.MF_TERMINAL_1_2_NOT_FOUND(terminal_in, tname));
                throw new nl_exception(nl_errstr_global.MF_TERMINAL_1_2_NOT_FOUND(terminal_in, tname));
            }

            if (term != null)
            {
                log().debug.op("Found input {0}\n", tname);
            }

            // FIXME: this should resolve any proxy
            return term;
        }


        public string de_alias(string alias)
        {
            string temp = alias;
            string ret;

            // FIXME: Detect endless loop
            do
            {
                ret = temp;
                temp = "";
                foreach (var e in m_abstract.m_alias)
                {
                    // FIXME: this will resolve first one found
                    if (e.second() == ret)
                    {
                        temp = e.first();
                        break;
                    }
                }
            } while (!temp.empty() && temp != ret);

            log().debug.op("{0}==>{1}\n", alias, ret);

            return ret;
        }


        // FIXME: only needed by solver code outside of setup_t
        public bool connect(detail.core_terminal_t t1_in, detail.core_terminal_t t2_in)
        {
            log().debug.op("Connecting {0} to {1}\n", t1_in.name(), t2_in.name());
            detail.core_terminal_t t1 = resolve_proxy(t1_in);
            detail.core_terminal_t t2 = resolve_proxy(t2_in);
            bool ret = true;

            if (t1.is_type(detail.terminal_type.OUTPUT) && t2.is_type(detail.terminal_type.INPUT))
            {
                if (t2.has_net() && t2.net().is_rail_net())
                {
                    log().fatal.op(nl_errstr_global.MF_INPUT_1_ALREADY_CONNECTED(t2.name()));
                    throw new nl_exception(nl_errstr_global.MF_INPUT_1_ALREADY_CONNECTED(t2.name()));
                }
                connect_input_output(t2, t1);
            }
            else if (t1.is_type(detail.terminal_type.INPUT) && t2.is_type(detail.terminal_type.OUTPUT))
            {
                if (t1.has_net() && t1.net().is_rail_net())
                {
                    log().fatal.op(nl_errstr_global.MF_INPUT_1_ALREADY_CONNECTED(t1.name()));
                    throw new nl_exception(nl_errstr_global.MF_INPUT_1_ALREADY_CONNECTED(t1.name()));
                }
                connect_input_output(t1, t2);
            }
            else if (t1.is_type(detail.terminal_type.OUTPUT) && t2.is_type(detail.terminal_type.TERMINAL))
            {
                connect_terminal_output((terminal_t)t2, t1);
            }
            else if (t1.is_type(detail.terminal_type.TERMINAL) && t2.is_type(detail.terminal_type.OUTPUT))
            {
                connect_terminal_output((terminal_t)t1, t2);
            }
            else if (t1.is_type(detail.terminal_type.INPUT) && t2.is_type(detail.terminal_type.TERMINAL))
            {
                connect_terminal_input((terminal_t)t2, t1);
            }
            else if (t1.is_type(detail.terminal_type.TERMINAL) && t2.is_type(detail.terminal_type.INPUT))
            {
                connect_terminal_input((terminal_t)t1, t2);
            }
            else if (t1.is_type(detail.terminal_type.TERMINAL) && t2.is_type(detail.terminal_type.TERMINAL))
            {
                connect_terminals((terminal_t)t1, (terminal_t)t2);
            }
            else if (t1.is_type(detail.terminal_type.INPUT) && t2.is_type(detail.terminal_type.INPUT))
            {
                ret = connect_input_input(t1, t2);
            }
            else
            {
                ret = false;
            }

            return ret;
        }


        // run preparation
        // ----------------------------------------------------------------------------------------
        // Run preparation
        // ----------------------------------------------------------------------------------------
        public void prepare_to_run()
        {
            string envlog = plib.pglobal.environment("NL_LOGS", "");

            if (!envlog.empty())
            {
                std.vector<string> loglist = plib.pglobal.psplit(envlog, ":");
                m_parser.register_dynamic_log_devices(loglist);
            }

            // create defparams!

            foreach (var e in m_abstract.m_defparams)
            {
                var param = new param_str_t(nlstate(), e.first, e.second);  //auto param(plib::make_unique<param_str_t, host_arena>(nlstate(), e.first, e.second));
                register_param_t(param);
                m_defparam_lifetime.push_back(param);
            }

            // make sure the solver and parameters are started first!

            foreach (var e in m_abstract.m_device_factory)
            {
                if ( m_parser.factory_().is_class<devices.nld_solver>(e.second) || m_parser.factory_().is_class<devices.nld_netlistparams>(e.second))
                {
                    m_nlstate.register_device(e.first, e.second.make_device(nlstate().pool(), m_nlstate, e.first));
                }
            }

            log().debug.op("Searching for solver and parameters ...\n");

            var solver = m_nlstate.get_single_device<devices.nld_solver>("solver");
            m_netlist_params = m_nlstate.get_single_device<devices.nld_netlistparams>("parameter");


            // set default model parameters

            // FIXME: this is not optimal
            m_parser.register_model(new plib.pfmt("NMOS_DEFAULT _(CAPMOD={0})").op(m_netlist_params.m_mos_capmodel.op()));
            m_parser.register_model(new plib.pfmt("PMOS_DEFAULT _(CAPMOD={0})").op(m_netlist_params.m_mos_capmodel.op()));


            // create devices

            log().debug.op("Creating devices ...\n");
            foreach (var e in m_abstract.m_device_factory)
            {
                if (!m_parser.factory_().is_class<devices.nld_solver>(e.second) && !m_parser.factory_().is_class<devices.nld_netlistparams>(e.second))
                {
                    var dev = e.second.make_device(m_nlstate.pool(), m_nlstate, e.first);
                    m_nlstate.register_device(dev.name(), dev);
                }
            }

            int errcnt = (0);
            log().debug.op("Looking for unknown parameters ...\n");
            foreach (var p in m_abstract.m_param_values)
            {
                var f = m_params.find(p.first());
                if (f == null)  //m_params.end())
                {
                    if (plib.pglobal.endsWith(p.first(), nl_errstr_global.sHINT_NO_DEACTIVATE))
                    {
                        // FIXME: get device name, check for device
                        var dev = m_nlstate.find_device(plib.pglobal.replace_all(p.first(), nl_errstr_global.sHINT_NO_DEACTIVATE, ""));
                        if (dev == null)
                        {
                            log().error.op(nl_errstr_global.ME_DEVICE_NOT_FOUND_FOR_HINT(p.first()));
                            errcnt++;
                        }
                    }
                    else
                    {
                        log().error.op(nl_errstr_global.ME_UNKNOWN_PARAMETER(p.first()));
                        errcnt++;
                    }
                }
            }

            bool use_deactivate = m_netlist_params.m_use_deactivate.op();

            foreach (var d in m_nlstate.devices())
            {
                if (use_deactivate)
                {
                    var p = m_abstract.m_param_values.find(d.second.name() + nl_errstr_global.sHINT_NO_DEACTIVATE);
                    if (p != null)
                    {
                        //FIXME: check for errors ...
                        bool err = false;
                        var v = plib.pglobal.pstonum_ne_nl_fptype(true, p, out err);
                        if (err || plib.pglobal.abs(v - plib.pglobal.floor(v)) > nlconst.magic(1e-6) )
                        {
                            log().error.op(nl_errstr_global.ME_HND_VAL_NOT_SUPPORTED(p));
                            errcnt++;
                        }
                        else
                        {
                            // FIXME comparison with zero
                            d.second.set_hint_deactivate(v == nlconst.zero());
                        }
                    }
                }
                else
                {
                    d.second.set_hint_deactivate(false);
                }
            }

            if (errcnt > 0)
            {
                log().fatal.op(nl_errstr_global.MF_ERRORS_FOUND(errcnt));
                throw new nl_exception(nl_errstr_global.MF_ERRORS_FOUND(errcnt));
            }

            // resolve inputs
            resolve_inputs();

            log().verbose.op("looking for two terms connected to rail nets ...");
            foreach (var t in m_nlstate.get_device_list<analog.nld_twoterm>())
            {
                if (t.N().net().is_rail_net() && t.P().net().is_rail_net())
                {
                    log().info.op(nl_errstr_global.MI_REMOVE_DEVICE_1_CONNECTED_ONLY_TO_RAILS_2_3(
                        t.name(), t.N().net().name(), t.P().net().name()));
                    remove_terminal(t.setup_N().net(), t.setup_N());
                    remove_terminal(t.setup_P().net(), t.setup_P());
                    m_nlstate.remove_device(t);
                }
            }

            log().verbose.op("initialize solver ...\n");

            if (solver == null)
            {
                foreach (var p in m_nlstate.nets())
                {
                    if (p.is_analog())
                    {
                        log().fatal.op(nl_errstr_global.MF_NO_SOLVER());
                        throw new nl_exception(nl_errstr_global.MF_NO_SOLVER());
                    }
                }
            }
            else
            {
                solver.post_start();
            }

            foreach (var n in m_nlstate.nets())
            {
                foreach (var term in n.core_terms())
                {
                    core_device_t dev = term.device();
                    dev.set_default_delegate(term);
                }
            }
        }


        public models_t models() { return m_models; }


        netlist_state_t nlstate() { return m_nlstate; }


        public nlparse_t parser() { return m_parser; }


        log_type log() { return m_nlstate.log(); }


        // FIXME: needed from matrix_solver_t
        public void add_terminal(detail.net_t net, detail.core_terminal_t terminal)
        {
            foreach (var t in net.core_terms())
            {
                if (t == terminal)
                {
                    log().fatal.op(nl_errstr_global.MF_NET_1_DUPLICATE_TERMINAL_2(net.name(), t.name()));
                    throw new nl_exception(nl_errstr_global.MF_NET_1_DUPLICATE_TERMINAL_2(net.name(), t.name()));
                }
            }

            terminal.set_net(net);

            net.core_terms().push_back(terminal);
        }


        void resolve_inputs()
        {
            log().verbose.op("Resolving inputs ...");

            // Netlist can directly connect input to input.
            // We therefore first park connecting inputs and retry
            // after all other terminals were connected.

            unsigned tries = m_netlist_params.m_max_link_loops.op();

            while (!m_abstract.m_links.empty() && tries > 0)
            {
                for (size_t i = 0; i < m_abstract.m_links.size(); )
                {
                    string t1s = m_abstract.m_links[i].first();
                    string t2s = m_abstract.m_links[i].second();
                    detail.core_terminal_t t1 = find_terminal(t1s);
                    detail.core_terminal_t t2 = find_terminal(t2s);

                    if (connect(t1, t2))
                        m_abstract.m_links.erase((int)i);  //m_abstract.m_links.erase(m_abstract.m_links.begin() + plib::narrow_cast<std::ptrdiff_t>(i));
                    else
                        i++;
                }

                tries--;
            }

            if (tries == 0)
            {
                foreach (var link in m_abstract.m_links)
                    log().warning.op(nl_errstr_global.MF_CONNECTING_1_TO_2(de_alias(link.first()), de_alias(link.second())));

                log().fatal.op(nl_errstr_global.MF_LINK_TRIES_EXCEEDED(m_netlist_params.m_max_link_loops.op()));
                throw new nl_exception(nl_errstr_global.MF_LINK_TRIES_EXCEEDED(m_netlist_params.m_max_link_loops.op()));
            }

            log().verbose.op("deleting empty nets ...");

            // delete empty nets

            delete_empty_nets();

            bool err = false;

            log().verbose.op("looking for terminals not connected ...");
            foreach (var i in m_terminals)
            {
                detail.core_terminal_t term = i.second();
                bool is_nc = (devices.nld_nc_pin)term.device() != null;
                if (term.has_net() && is_nc)
                {
                    log().error.op(nl_errstr_global.ME_NC_PIN_1_WITH_CONNECTIONS(term.name()));
                    err = true;
                }
                else if (is_nc)
                {
                    /* ignore */
                }
                else if (!term.has_net())
                {
                    log().error.op(nl_errstr_global.ME_TERMINAL_1_WITHOUT_NET(de_alias(term.name())));
                    err = true;
                }
                else if (!term.net().has_connections())
                {
                    if (term.is_logic_input())
                        log().warning.op(nl_errstr_global.MW_LOGIC_INPUT_1_WITHOUT_CONNECTIONS(term.name()));
                    else if (term.is_logic_output())
                        log().info.op(nl_errstr_global.MI_LOGIC_OUTPUT_1_WITHOUT_CONNECTIONS(term.name()));
                    else if (term.is_analog_output())
                        log().info.op(nl_errstr_global.MI_ANALOG_OUTPUT_1_WITHOUT_CONNECTIONS(term.name()));
                    else
                        log().warning.op(nl_errstr_global.MW_TERMINAL_1_WITHOUT_CONNECTIONS(term.name()));
                }
            }

            log().verbose.op("checking tristate consistency  ...");

            foreach (var i in m_terminals)
            {
                detail.core_terminal_t term = i.second();
                if (term.is_tristate_output())
                {
                    var tri = (tristate_output_t)term;
                    // check if we are connected to a proxy
                    var iter_proxy = m_proxies.find(tri);

                    if (iter_proxy == default && !tri.is_force_logic())
                    {
                        log().error.op(nl_errstr_global.ME_TRISTATE_NO_PROXY_FOUND_2(term.name(), term.device().name()));
                        err = true;
                    }
                    else if (iter_proxy != default && tri.is_force_logic())
                    {
                        log().error.op(nl_errstr_global.ME_TRISTATE_PROXY_FOUND_2(term.name(), term.device().name()));
                        err = true;
                    }
                }
            }

            if (err)
            {
                log().fatal.op(nl_errstr_global.MF_TERMINALS_WITHOUT_NET());
                throw new nl_exception(nl_errstr_global.MF_TERMINALS_WITHOUT_NET());
            }
        }


        string resolve_alias(string name)
        {
            string temp = name;
            string ret;

            // FIXME: Detect endless loop
            do {
                ret = temp;
                var p = m_abstract.m_alias.find(ret);
                temp = p != default ? p : "";
            } while (!temp.empty() && temp != ret);

            log().debug.op("{0}==>{1}\n", name, ret);
            return ret;
        }


        void merge_nets(detail.net_t thisnet, detail.net_t othernet)
        {
            log().debug.op("merging nets ...\n");
            if (othernet == thisnet)
            {
                log().warning.op(nl_errstr_global.MW_CONNECTING_1_TO_ITSELF(thisnet.name()));
                return; // Nothing to do
            }

            if (thisnet.is_rail_net() && othernet.is_rail_net())
            {
                log().fatal.op(nl_errstr_global.MF_MERGE_RAIL_NETS_1_AND_2(thisnet.name(), othernet.name()));
                throw new nl_exception(nl_errstr_global.MF_MERGE_RAIL_NETS_1_AND_2(thisnet.name(), othernet.name()));
            }

            if (othernet.is_rail_net())
            {
                log().debug.op("othernet is railnet\n");
                merge_nets(othernet, thisnet);
            }
            else
            {
                move_connections(othernet, thisnet);
            }
        }


        void connect_terminals(detail.core_terminal_t t1, detail.core_terminal_t t2)
        {
            if (t1.has_net() && t2.has_net())
            {
                log().debug.op("T2 and T1 have net\n");
                merge_nets(t1.net(), t2.net());
            }
            else if (t2.has_net())
            {
                log().debug.op("T2 has net\n");
                add_terminal(t2.net(), t1);
            }
            else if (t1.has_net())
            {
                log().debug.op("T1 has net\n");
                add_terminal(t1.net(), t2);
            }
            else
            {
                log().debug.op("adding analog net ...\n");
                // FIXME: Nets should have a unique name
                var anet = new analog_net_t(m_nlstate, "net." + t1.name());  //auto anet = plib::make_owned<analog_net_t>(nlstate().pool(), m_nlstate,"net." + t1.name());
                var anetp = anet;  //auto anetp = anet.get();
                m_nlstate.register_net(anet);  //plib::owned_ptr<analog_net_t>(anet, true));
                t1.set_net(anetp);
                add_terminal(anetp, t2);
                add_terminal(anetp, t1);
            }
        }


        void connect_input_output(detail.core_terminal_t in_, detail.core_terminal_t out_)
        {
            if (out_.is_analog() && in_.is_logic())
            {
                var proxy = get_a_d_proxy(in_);

                add_terminal(out_.net(), proxy.proxy_term());
            }
            else if (out_.is_logic() && in_.is_analog())
            {
                devices.nld_base_proxy proxy = get_d_a_proxy(out_);

                connect_terminals(proxy.proxy_term(), in_);
                //proxy->out().net().register_con(in);
            }
            else
            {
                if (in_.has_net())
                    merge_nets(out_.net(), in_.net());
                else
                    add_terminal(out_.net(), in_);
            }
        }


        void connect_terminal_output(terminal_t in_, detail.core_terminal_t out_)
        {
            if (out_.is_analog())
            {
                log().debug.op("connect_terminal_output: {0} {1}\n", in_.name(), out_.name());
                // no proxy needed, just merge existing terminal net
                if (in_.has_net())
                {
                    if (out_.net() == in_.net())
                        log().warning.op(nl_errstr_global.MW_CONNECTING_1_TO_2_SAME_NET(in_.name(), out_.name(), in_.net().name()));

                    merge_nets(out_.net(), in_.net());
                }
                else
                {
                    add_terminal(out_.net(), in_);
                }
            }
            else if (out_.is_logic())
            {
                log().debug.op("connect_terminal_output: connecting proxy\n");
                devices.nld_base_proxy proxy = get_d_a_proxy(out_);

                connect_terminals(proxy.proxy_term(), in_);
            }
            else
            {
                log().fatal.op(nl_errstr_global.MF_OBJECT_OUTPUT_TYPE_1(out_.name()));
                throw new nl_exception(nl_errstr_global.MF_OBJECT_OUTPUT_TYPE_1(out_.name()));
            }
        }


        void connect_terminal_input(terminal_t term, detail.core_terminal_t inp)
        {
            if (inp.is_analog())
            {
                connect_terminals(inp, term);
            }
            else if (inp.is_logic())
            {
                log().verbose.op("connect terminal {0} (in, {1}) to {2}\n", inp.name(), inp.is_analog() ? "analog" : inp.is_logic() ? "logic" : "?", term.name());
                var proxy = get_a_d_proxy(inp);

                //out.net().register_con(proxy->proxy_term());
                connect_terminals(term, proxy.proxy_term());

            }
            else
            {
                log().fatal.op(nl_errstr_global.MF_OBJECT_INPUT_TYPE_1(inp.name()));
                throw new nl_exception(nl_errstr_global.MF_OBJECT_INPUT_TYPE_1(inp.name()));
            }
        }


        detail.core_terminal_t resolve_proxy(detail.core_terminal_t term)
        {
            if (term.is_logic())
            {
                logic_t out_ = (logic_t)term;  //auto &out = dynamic_cast<logic_t &>(term);
                var iter_proxy = m_proxies.find(out_);
                if (iter_proxy != null)  //if (iter_proxy != m_proxies.end())
                    return iter_proxy.proxy_term();  //return iter_proxy->second->proxy_term();
            }

            return term;
        }


        bool connect_input_input(detail.core_terminal_t t1, detail.core_terminal_t t2)
        {
            bool ret = false;
            if (t1.has_net())
            {
                if (t1.net().is_rail_net())
                    ret = connect(t2, t1.net().railterminal());

                if (!ret)
                {
                    foreach (var t in t1.net().core_terms())
                    {
                        if (t.is_type(detail.terminal_type.TERMINAL))
                            ret = connect(t2, t);
                        if (ret)
                            break;
                    }
                }
            }

            if (!ret && t2.has_net())
            {
                if (t2.net().is_rail_net())
                    ret = connect(t1, t2.net().railterminal());

                if (!ret)
                {
                    foreach (var t in t2.net().core_terms())
                    {
                        if (t.is_type(detail.terminal_type.TERMINAL))
                            ret = connect(t1, t);
                        if (ret)
                            break;
                    }
                }
            }

            return ret;
        }


        // helpers
        static string termtype_as_str(detail.core_terminal_t in_)
        {
            switch (in_.type())
            {
                case detail.terminal_type.TERMINAL:
                    return "TERMINAL";
                case detail.terminal_type.INPUT:
                    return "INPUT";
                case detail.terminal_type.OUTPUT:
                    return "OUTPUT";
            }

            return "Error"; // Tease gcc
        }


        devices.nld_base_proxy get_d_a_proxy(detail.core_terminal_t out_)
        {
            nl_config_global.nl_assert(out_.is_logic());

            var out_cast = (logic_output_t)out_;  //const auto &out_cast = dynamic_cast<const logic_output_t &>(out);
            var iter_proxy = m_proxies.find(out_);

            if (iter_proxy != null)  //if (iter_proxy != m_proxies.end())
                return iter_proxy;

            // create a new one ...
            string x = new plib.pfmt("proxy_da_{0}_{1}").op(out_.name(), m_proxy_cnt);
            var new_proxy = out_cast.logic_family().create_d_a_proxy(m_nlstate, x, out_cast);
            m_proxy_cnt++;

            // connect all existing terminals to new net

            foreach (var p in out_.net().core_terms())
            {
                p.clear_net(); // de-link from all nets ...
                if (!connect(new_proxy.proxy_term(), p))
                {
                    log().fatal.op(nl_errstr_global.MF_CONNECTING_1_TO_2(new_proxy.proxy_term().name(), p.name()));
                    throw new nl_exception(nl_errstr_global.MF_CONNECTING_1_TO_2(new_proxy.proxy_term().name(), p.name()));
                }
            }

            out_.net().core_terms().clear();

            add_terminal(out_.net(), new_proxy.in_());

            var proxy = new_proxy;  //auto proxy(new_proxy.get());
            if (!m_proxies.insert(out_, proxy))
                throw new nl_exception(nl_errstr_global.MF_DUPLICATE_PROXY_1(out_.name()));

            m_nlstate.register_device(new_proxy.name(), new_proxy);
            return proxy;
        }


        devices.nld_base_proxy get_a_d_proxy(detail.core_terminal_t inp)
        {
            nl_config_global.nl_assert(inp.is_logic());

            var incast = (logic_input_t)inp;  //const auto &incast = dynamic_cast<const logic_input_t &>(inp);
            var iter_proxy = m_proxies.find(inp);

            if (iter_proxy != null)  //if (iter_proxy != m_proxies.end())
                return iter_proxy;

            log().debug.op("connect_terminal_input: connecting proxy\n");
            string x = new plib.pfmt("proxy_ad_{0}_{1}").op(inp.name(), m_proxy_cnt);
            var new_proxy = incast.logic_family().create_a_d_proxy(m_nlstate, x, incast);
            //auto new_proxy = plib::owned_ptr<devices::nld_a_to_d_proxy>::Create(netlist(), x, &incast);

            var ret = new_proxy;  //auto ret(new_proxy.get());

            if (!m_proxies.insert(inp, ret))  //if (!m_proxies.insert({&inp, ret}).second)
                throw new nl_exception(nl_errstr_global.MF_DUPLICATE_PROXY_1(inp.name()));

            m_proxy_cnt++;

            // connect all existing terminals to new net

            if (inp.has_net())
            {
                foreach (var p in inp.net().core_terms())
                {
                    p.clear_net(); // de-link from all nets ...
                    if (!connect(ret.proxy_term(), p))
                    {
                        log().fatal.op(nl_errstr_global.MF_CONNECTING_1_TO_2(ret.proxy_term().name(), p.name()));
                        throw new nl_exception(nl_errstr_global.MF_CONNECTING_1_TO_2(ret.proxy_term().name(), p.name()));
                    }
                }
                inp.net().core_terms().clear(); // clear the list
            }
            add_terminal(ret.out_().net(), inp);
            m_nlstate.register_device(new_proxy.name(), new_proxy);
            return ret;
        }


        //detail::core_terminal_t &resolve_proxy(detail::core_terminal_t &term);


        // net manipulations

        void remove_terminal(detail.net_t net, detail.core_terminal_t terminal)
        {
            if (plib.container.contains(net.core_terms(), terminal))
            {
                terminal.set_net(null);
                plib.container.remove(net.core_terms(), terminal);
            }
            else
            {
                log().fatal.op(nl_errstr_global.MF_REMOVE_TERMINAL_1_FROM_NET_2(terminal.name(), net.name()));
                throw new nl_exception(nl_errstr_global.MF_REMOVE_TERMINAL_1_FROM_NET_2(terminal.name(), net.name()));
            }
        }


        void move_connections(detail.net_t net, detail.net_t dest_net)
        {
            foreach (var ct in net.core_terms())
                add_terminal(dest_net, ct);

            net.core_terms().clear();
        }


        // ----------------------------------------------------------------------------------------
        // Device handling
        // ----------------------------------------------------------------------------------------
        void delete_empty_nets()
        {
            throw new emu_unimplemented();
#if false
            m_nlstate.nets().erase(
                std::remove_if(m_nlstate.nets().begin(), m_nlstate.nets().end(),
                    [](device_arena::owned_ptr<detail::net_t> &net)
                    {
                        if (!net->has_connections())
                        {
                            net->state().log().verbose("Deleting net {1} ...", net->name());
                            net->state().run_state_manager().remove_save_items(net.get());
                            return true;
                        }
                        return false;
                    }), m_nlstate.nets().end());
#endif
        }
    }


    // ----------------------------------------------------------------------------------------
    // base sources
    // ----------------------------------------------------------------------------------------

    //class source_string_t : public source_netlist_t

    //class source_file_t : public source_netlist_t

    //class source_mem_t : public source_netlist_t


    public class source_proc_t : source_netlist_t
    {
        public delegate void setup_func_delegate(nlparse_t parse);


        setup_func_delegate m_setup_func;  //void (*m_setup_func)(nlparse_t &);
        string m_setup_func_name;


        public source_proc_t(string name, setup_func_delegate setup_func)  //source_proc_t(const pstring &name, void (*setup_func)(nlparse_t &))
        {
            m_setup_func = setup_func;
            m_setup_func_name = name;
        }


        public override bool parse(nlparse_t setup, string name)
        {
            if (name == m_setup_func_name)
            {
                m_setup_func(setup);
                return true;
            }

            return false;
        }


        public override std.istream stream(string name)
        {
            throw new emu_unimplemented();
        }
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
        public param_model_t.value_str_t m_TYPE; //!< Family type (TTL, CMOS, ...)
        public param_model_t.value_t m_IVL;      //!< Input voltage low threshold relative to supply voltage
        public param_model_t.value_t m_IVH;      //!< Input voltage high threshold relative to supply voltage
        public param_model_t.value_t m_OVL;      //!< Output voltage minimum voltage relative to supply voltage
        public param_model_t.value_t m_OVH;      //!< Output voltage maximum voltage relative to supply voltage
        public param_model_t.value_t m_ORL;      //!< Output output resistance for logic 0
        public param_model_t.value_t m_ORH;      //!< Output output resistance for logic 1


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
