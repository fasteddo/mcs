// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using link_t = System.Collections.Generic.KeyValuePair<string, string>;  //using link_t = std::pair<pstring, pstring>;
using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;//, NL_DEBUG>;
using model_map_t = mame.std.unordered_map<string, string>;
using nl_fptype = System.Double;


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
        //        setup.register_devx(# type, { PSTRINGIFY_VA(__VA_ARGS__) });
        public static void NET_REGISTER_DEVEXT(nlparse_t setup, string type, params string [] args) { setup.register_devx(type, args); }

        //#define NET_CONNECT(name, input, output)                                        \
        //        setup.register_link(# name "." # input, # output);

        //#define NET_C(term1, ...)                                                       \
        //        setup.register_link_arr( # term1 ", " # __VA_ARGS__);
        public static void NET_C(nlparse_t setup, params string [] term1) { setup.register_link_arr(string.Join(", ", term1)); }

        //#define PARAM(name, val)                                                        \
        //        setup.register_param(# name, # val);
        public static void PARAM(nlparse_t setup, string name, int val) { PARAM(setup, name, val.ToString()); }
        public static void PARAM(nlparse_t setup, string name, double val) { PARAM(setup, name, val.ToString()); }
        public static void PARAM(nlparse_t setup, string name, string val) { setup.register_param(name, val); }

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
        public static void NETLIST_START() {}

        //#define NETLIST_END()  }
        public static void NETLIST_END() {}

        //#define LOCAL_SOURCE(name)                                                     \
        //        setup.register_source(plib::make_unique<netlist::source_proc_t>(# name, &NETLIST_NAME(name)));
        public static void LOCAL_SOURCE(nlparse_t setup, string name, source_proc_t.setup_func_delegate netlist_name) { setup.register_source(new netlist.source_proc_t(name, netlist_name)); }

        //#define LOCAL_LIB_ENTRY(name)                                                  \
        //        LOCAL_SOURCE(name)                                                     \
        //        setup.register_lib_entry(# name, __FILE__);
        public static void LOCAL_LIB_ENTRY(nlparse_t setup, string name, source_proc_t.setup_func_delegate nld_name)
        {
            LOCAL_SOURCE(setup, name, nld_name);
            setup.register_lib_entry(name, "__FILE__");
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


        // -----------------------------------------------------------------------------
        // truthtable defines
        // -----------------------------------------------------------------------------

        static netlist.tt_desc TRUTHTABLE_desc;

        //#define TRUTHTABLE_START(cname, in, out, def_params) \
        //    { \
        //        netlist::tt_desc desc; \
        //        desc.name = #cname ; \
        //        desc.classname = #cname ; \
        //        desc.ni = in; \
        //        desc.no = out; \
        //        desc.def_param = def_params; \
        //        desc.family = "";
        public static void TRUTHTABLE_START(string cname, UInt32 in_, UInt32 out_, string def_params)
        {
            netlist.tt_desc desc = new netlist.tt_desc();
            desc.name = cname;
            desc.classname = cname;
            desc.ni = in_;
            desc.no = out_;
            desc.def_param = def_params;
            desc.family = "";

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
        //        setup.truthtable_create(desc, __FILE__);       \
        //    }
        public static void TRUTHTABLE_END(nlparse_t setup) { setup.truthtable_create(TRUTHTABLE_desc, "__FILE__");  TRUTHTABLE_desc = null; }
    }


    // -----------------------------------------------------------------------------
    // truthtable desc
    // -----------------------------------------------------------------------------
    public class tt_desc
    {
        public string name;
        public string classname;
        public UInt32 ni;  //unsigned long ni;
        public UInt32 no;  //unsigned long no;
        public string def_param;
        public std.vector<string> desc = new std.vector<string>();
        public string family;

        public tt_desc() { ni = 0; no = 0; }
    }


    // -----------------------------------------------------------------------------
    // param_ref_t
    // -----------------------------------------------------------------------------
    class param_ref_t
    {
        string m_name;
        core_device_t m_device;
        public param_t m_param;

        public param_ref_t(string name, core_device_t device, param_t param)
        {
            m_name = name;
            m_device = device;
            m_param = param;
        }

        //const pstring &name() const noexcept { return m_name; }
        //const core_device_t &device() const noexcept { return m_device; }
        //param_t *param() const noexcept { return &m_param; }
    }


    // ----------------------------------------------------------------------------------------
    // Specific netlist psource_t implementations
    // ----------------------------------------------------------------------------------------
    public abstract class source_netlist_t : plib.psource_t
    {
        //friend class setup_t;

        //source_netlist_t() = default;
        //COPYASSIGNMOVE(source_netlist_t, delete)
        //virtual ~source_netlist_t() noexcept = default;

        public virtual bool parse(nlparse_t setup, string name)
        {
            var strm = stream(name);
            return strm != null ? setup.parse_stream(strm, name) : false;
        }
    }


    abstract class source_data_t : plib.psource_t
    {
        //friend class setup_t;

        //source_data_t() = default;
        //COPYASSIGNMOVE(source_data_t, delete)
        //virtual ~source_data_t() noexcept = default;
    }


    // ----------------------------------------------------------------------------------------
    // Collection of models
    // ----------------------------------------------------------------------------------------
    public class models_t
    {
        //using model_map_t = std::unordered_map<pstring, pstring>;

        std.unordered_map<string, string> m_models = new std.unordered_map<string, string>();
        std.unordered_map<string, model_map_t> m_cache = new std.unordered_map<string, model_map_t>();


        public void register_model(string model_in)
        {
            var pos = model_in.find(' ');
            if (pos == -1)
                throw new nl_exception(nl_errstr_global.MF_UNABLE_TO_PARSE_MODEL_1(model_in));
            string model = plib.pglobal.ucase(plib.pglobal.trim(plib.pglobal.left(model_in, pos)));
            string def = plib.pglobal.trim(model_in.substr(pos + 1));
            if (!m_models.insert(model, def))
                throw new nl_exception(nl_errstr_global.MF_MODEL_ALREADY_EXISTS_1(model_in));
        }

        // model / family related

        public string value_str(string model, string entity)
        {
            model_map_t map = m_cache[model];

            if (map == null)
            {
                map = new model_map_t();
                m_cache[model] = map;
            }

            if (map.empty())
                model_parse(model , map);

            if (entity != plib.pglobal.ucase(entity))
                throw new nl_exception(nl_errstr_global.MF_MODEL_PARAMETERS_NOT_UPPERCASE_1_2(entity, model_string(map)));
            if (map.find(entity) == null)
                throw new nl_exception(nl_errstr_global.MF_ENTITY_1_NOT_FOUND_IN_MODEL_2(entity, model_string(map)));

            return map[entity];
        }


        public nl_fptype value(string model, string entity)
        {
            model_map_t map = m_cache[model];

            if (map == null)
            {
                map = new model_map_t();
                m_cache[model] = map;
            }

            if (map.empty())
                model_parse(model , map);

            string tmp = value_str(model, entity);

            nl_fptype factor = nlconst.one();
            var p = tmp[tmp.Length - 1];  //auto p = std::next(tmp.begin(), static_cast<pstring::difference_type>(tmp.size() - 1));
            switch (p)
            {
                case 'M': factor = nlconst.magic(1e6); break;
                case 'k':
                case 'K': factor = nlconst.magic(1e3); break;
                case 'm': factor = nlconst.magic(1e-3); break;
                case 'u': factor = nlconst.magic(1e-6); break;
                case 'n': factor = nlconst.magic(1e-9); break;
                case 'p': factor = nlconst.magic(1e-12); break;
                case 'f': factor = nlconst.magic(1e-15); break;
                case 'a': factor = nlconst.magic(1e-18); break;
                default:
                    if (p < '0' || p > '9')
                        throw new nl_exception(nl_errstr_global.MF_UNKNOWN_NUMBER_FACTOR_IN_1(entity));
                    break;
            }
            if (factor != nlconst.one())
                tmp = plib.pglobal.left(tmp, tmp.Length - 1);

            // FIXME: check for errors
            //printf("%s %s %e %e\n", entity.c_str(), tmp.c_str(), plib::pstonum<nl_fptype>(tmp), factor);
            bool err = false;
            var val = plib.pglobal.pstonum_ne_nl_fptype(true, tmp, out err);
            if (err)
                throw new nl_exception(nl_errstr_global.MF_MODEL_NUMBER_CONVERSION_ERROR(entity, tmp, "double", model));

            return val * factor;
        }


        //pstring type(pstring model) { return value_str(model, "COREMODEL"); }


        void model_parse(string model_in, model_map_t map)
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
                    throw new nl_exception(nl_errstr_global.MF_MODEL_NOT_FOUND(model));

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


        static string model_string(model_map_t map)
        {
            // operator [] has no const implementation
            string ret = map["COREMODEL"] + "(";
            foreach (var i in map)
                ret += (i.first() + '=' + i.second() + ' ');

            return ret + ")";
        }
    }


    // ----------------------------------------------------------------------------------------
    // nlparse_t
    // ----------------------------------------------------------------------------------------

    public class nlparse_t
    {
        //using link_t = std::pair<pstring, pstring>;


        protected models_t m_models = new models_t();
        std.stack<string> m_namespace_stack = new std.stack<string>();
        protected std.unordered_map<string, string> m_alias = new std.unordered_map<string, string>();
        protected std.vector<link_t> m_links = new std.vector<link_t>();
        protected std.unordered_map<string, string> m_param_values = new std.unordered_map<string, string>();

        plib.psource_collection_t m_sources = new plib.psource_collection_t();  //plib::psource_collection_t<>                m_sources;

        factory.list_t m_factory;

        // need to preserve order of device creation ...
        protected std.vector<std.pair<string, factory.element_t>> m_device_factory = new std.vector<std.pair<string, factory.element_t>>();


        //plib::ppreprocessor::defines_map_type       m_defines;
        //plib::psource_collection_t<>                m_includes;

        protected setup_t m_setup;
        log_type m_log;
        UInt32 m_frontier_cnt;


        protected nlparse_t(setup_t setup, log_type log)
        {
            m_factory = new factory.list_t(log);
            m_setup = setup;
            m_log = log;
            m_frontier_cnt = 0;
        }


        public void register_model(string model_in) { m_models.register_model(model_in); }


        public void register_alias(string alias, string out_)
        {
            string alias_fqn = build_fqn(alias);
            string out_fqn = build_fqn(out_);
            register_alias_nofqn(alias_fqn, out_fqn);
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


        public void register_dev(string classname, string name)
        {
            var f = factory().factory_by_name(classname);
            if (f == null)
            {
                log().fatal.op(nl_errstr_global.MF_CLASS_1_NOT_FOUND(classname));
                throw new nl_exception(nl_errstr_global.MF_CLASS_1_NOT_FOUND(classname));
            }

            // make sure we parse macro library entries
            f.macro_actions(this, name);
            string key = build_fqn(name);
            if (device_exists(key))
            {
                log().fatal.op(nl_errstr_global.MF_DEVICE_ALREADY_EXISTS_1(name));
                throw new nl_exception(nl_errstr_global.MF_DEVICE_ALREADY_EXISTS_1(name));
            }

            m_device_factory.Add(new std.pair<string, factory.element_t>(key, f));  //m_device_factory.insert(m_device_factory.end(), {key, f});
        }


        void register_dev(string classname, string name, std.vector<string> params_and_connections)
        {
            var f = m_setup.factory().factory_by_name(classname);  //factory::element_t *f = m_setup.factory().factory_by_name(classname);
            var paramlist = plib.pglobal.psplit(f.param_desc(), ",");

            register_dev(classname, name);

            if (!params_and_connections.empty())
            {
                var ptokIdx = 0;  //auto ptok(params_and_connections.begin());
                var ptok_endIdx = params_and_connections.Count;  //auto ptok_end(params_and_connections.end());

                foreach (var tp in paramlist)
                {
                    //printf("x %s %s\n", tp.c_str(), ptok->c_str());
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
                        m_setup.log().debug.op("Link: {0} {1}\n", tp, term);

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
        }


        public void register_devx(string classname, string [] params_and_connections)
        {
            std.vector<string> params_ = new std.vector<string>();
            int i = 0;  //auto i(params_and_connections.begin());
            string name = params_and_connections[i];  //pstring name(*i);
            ++i;
            for (; i < params_and_connections.Length; i++)  //for (; i != params_and_connections.end(); ++i)
            {
                params_.emplace_back(params_and_connections[i]);
            }
            register_dev(classname, name, params_);
        }


        //void register_dev(const pstring &classname, const pstring &name, const char *params_and_connections);


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


        public void register_param(string param, string value)
        {
            string fqn = build_fqn(param);
            string val = value;

            // strip " from stringified strings
            if (plib.pglobal.startsWith(value, "\"") && plib.pglobal.endsWith(value, "\""))
                val = value.substr(1, value.length() - 2);

            var idx = m_param_values.find(fqn);
            if (idx == null)  //m_param_values.end())
            {
                if (!m_param_values.insert(fqn, val))
                    log().fatal.op(nl_errstr_global.MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(param));
            }
            else
            {
                log().warning.op(nl_errstr_global.MW_OVERWRITING_PARAM_1_OLD_2_NEW_3(fqn, idx, val));
                m_param_values[fqn] = val;
            }
        }


        // FIXME: quick hack
        //void register_param_x(const pstring &param, const nl_fptype value);
        void register_param_x(string param, nl_fptype value)
        {
            if (plib.pglobal.abs(value - plib.pglobal.floor(value)) > nlconst.magic(1e-30)
                || plib.pglobal.abs(value) > nlconst.magic(1e9))
                register_param(param, new plib.pfmt("{0}").op(value));  //register_param(param, plib::pfmt("{1:.9}").e(value));
            else
                register_param(param, new plib.pfmt("{0}").op(value));  //register_param(param, plib::pfmt("{1}")(static_cast<long>(value)));
        }


        //template <typename T>
        //typename std::enable_if<std::is_floating_point<T>::value || std::is_integral<T>::value>::type
        public void register_param_val(string param, nl_fptype value)  //register_param(const pstring &param, T value)
        {
            register_param_x(param, value);  //register_param_x(param, static_cast<nl_fptype>(value));
        }


#if PUSE_FLOAT128
        void register_param(const pstring &param, __float128 value)
        {
            register_param_x(param, static_cast<nl_fptype>(value));
        }
#endif


        public void register_lib_entry(string name, string sourcefile)
        {
            m_factory.register_device(new factory.library_element_t(name, name, "", sourcefile));  //m_factory.register_device(plib::make_unique<factory::library_element_t>(name, name, "", sourcefile));
        }


        //void register_frontier(const pstring &attach, const pstring &r_IN, const pstring &r_OUT);


        // register a source
        public void register_source(plib.psource_t src)  //void register_source(plib::unique_ptr<plib::psource_t> &&src)
        {
            m_sources.add_source(src);  //m_sources.add_source(std::move(src));
        }


        public void truthtable_create(tt_desc desc, string sourcefile)
        {
            var fac = netlist.factory.nlid_truthtable_global.truthtable_create(desc, sourcefile);
            m_factory.register_device(fac);
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


        protected string build_fqn(string obj_name)
        {
            return m_namespace_stack.empty() ? obj_name
                : m_namespace_stack.top() + "." + obj_name;
        }


        public void register_alias_nofqn(string alias, string out_)
        {
            if (!m_alias.insert(alias, out_))
            {
                log().fatal.op(nl_errstr_global.MF_ADDING_ALI1_TO_ALIAS_LIST(alias));
                throw new nl_exception(nl_errstr_global.MF_ADDING_ALI1_TO_ALIAS_LIST(alias));
            }
        }


        // also called from devices for latebinding connected terminals
        public void register_link_fqn(string sin, string sout)
        {
            link_t temp = new link_t(sin, sout);
            log().debug.op("link {0} <== {1}", sin, sout);
            m_links.push_back(temp);
        }


        // used from netlist.cpp (mame)
        public bool device_exists(string name)
        {
            foreach (var d in m_device_factory)
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


        //void add_include(plib::unique_ptr<plib::psource_t> &&inc)
        //void add_define(const pstring &def, const pstring &val)
        //void add_define(const pstring &defstr);


        //factory::list_t &factory() { return m_factory; }
        //const factory::list_t &factory() const { return m_factory; }
        public factory.list_t factory() { return m_factory; }

        //log_type &log() { return m_log; }
        //const log_type &log() const { return m_log; }
        public log_type log() { return m_log; }


        // FIXME: sources may need access to the netlist parent type
        // since they may be created in a context in which they don't
        // have access to their environment.
        // Example is the MAME memregion source.
        // We thus need a better approach to creating netlists in a context
        // other than static procedures.
        protected setup_t setup() { return m_setup; }
        //const setup_t &setup() const { return m_setup; }

        public models_t models() { return m_models; }
    }


    // ----------------------------------------------------------------------------------------
    // setup_t
    // ----------------------------------------------------------------------------------------

    public class setup_t : nlparse_t
    {
        std.unordered_map<string, detail.core_terminal_t> m_terminals = new std.unordered_map<string, detail.core_terminal_t>();
        std.unordered_map<terminal_t, terminal_t> m_connected_terminals = new std.unordered_map<terminal_t, terminal_t>();

        netlist_state_t m_nlstate;
        devices.nld_netlistparams m_netlist_params;
        std.unordered_map<string, param_ref_t> m_params = new std.unordered_map<string, param_ref_t>();
        std.unordered_map<detail.core_terminal_t, devices.nld_base_proxy> m_proxies = new std.unordered_map<detail.core_terminal_t, devices.nld_base_proxy>();

        UInt32 m_proxy_cnt;


        public setup_t(netlist_state_t nlstate)
            : base(null, nlstate.log())  //: nlparse_t(*this, nlstate.log())
        {
            m_setup = this;  // NOTE, can't pass this above so we do it here


            m_nlstate = nlstate;
            m_netlist_params = null;
            m_proxy_cnt = 0;
        }

        //~setup_t() { }

        //COPYASSIGNMOVE(setup_t, delete)


        netlist_state_t nlstate() { return m_nlstate; }


        public void register_param_t(string name, param_t param)
        {
            if (!m_params.insert(param.name(), new param_ref_t(param.name(), param.device(), param)))
            {
                log().fatal.op(nl_errstr_global.MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(name));
                throw new nl_exception(nl_errstr_global.MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(name));
            }
        }


        public string get_initial_param_val(string name, string def)
        {
            var i = m_param_values.find(name);
            return (i != null) ? i : def;  //return (i != m_param_values.end()) ? i->second : def;
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


        public terminal_t get_connected_terminal(terminal_t term)
        {
            var ret = m_connected_terminals.find(term);
            return ret != null ? ret : null;
        }


        //void remove_connections(const pstring &pin);


        public bool connect(detail.core_terminal_t t1_in, detail.core_terminal_t t2_in)
        {
            log().debug.op("Connecting {0} to {1}\n", t1_in.name(), t2_in.name());
            detail.core_terminal_t t1 = resolve_proxy(t1_in);
            detail.core_terminal_t t2 = resolve_proxy(t2_in);
            bool ret = true;

            if (t1.is_type(detail.terminal_type.OUTPUT) && t2.is_type(detail.terminal_type.INPUT))
            {
                if (t2.has_net() && t2.net().isRailNet())
                {
                    log().fatal.op(nl_errstr_global.MF_INPUT_1_ALREADY_CONNECTED(t2.name()));
                    throw new nl_exception(nl_errstr_global.MF_INPUT_1_ALREADY_CONNECTED(t2.name()));
                }
                connect_input_output(t2, t1);
            }
            else if (t1.is_type(detail.terminal_type.INPUT) && t2.is_type(detail.terminal_type.OUTPUT))
            {
                if (t1.has_net()  && t1.net().isRailNet())
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
                ret = false;

            return ret;
        }


        public param_t find_param(string param_in, bool required = true)
        {
            string param_in_fqn = build_fqn(param_in);

            string outname = resolve_alias(param_in_fqn);
            var ret = m_params.find(outname);
            if (ret == null && required)
            {
                log().fatal.op(nl_errstr_global.MF_PARAMETER_1_2_NOT_FOUND(param_in_fqn, outname));
                throw new nl_exception(nl_errstr_global.MF_PARAMETER_1_2_NOT_FOUND(param_in_fqn, outname));
            }

            if (ret != null)
                log().debug.op("Found parameter {0}\n", outname);

            return ret == null ? null : ret.m_param;
        }


        // get family
        public logic_family_desc_t family_from_model(string model)
        {
            if (m_models.value_str(model, "TYPE") == "TTL")
                return nl_base_global.family_TTL();
            if (m_models.value_str(model, "TYPE") == "CD4XXX")
                return nl_base_global.family_CD4XXX();

            var it = m_nlstate.m_family_cache.find(model);
            if (it != null)  //if (it != m_nlstate.m_family_cache.end())
                return it;   //return it->second.get();

            var ret = new logic_family_std_proxy_t();  //plib::make_unique_base<logic_family_desc_t, logic_family_std_proxy_t>();

            ret.m_low_thresh_PCNT = m_models.value(model, "IVL");
            ret.m_high_thresh_PCNT = m_models.value(model, "IVH");
            ret.m_low_VO = m_models.value(model, "OVL");
            ret.m_high_VO = m_models.value(model, "OVH");
            ret.m_R_low = m_models.value(model, "ORL");
            ret.m_R_high = m_models.value(model, "ORH");

            var retp = ret.get();

            m_nlstate.m_family_cache.emplace(model, ret);  //m_nlstate.m_family_cache.emplace(model, std::move(ret));

            return retp;
        }


        void register_dynamic_log_devices(std.vector<string> loglist)
        {
            log().debug.op("Creating dynamic logs ...");
            foreach (var ll in  loglist)
            {
                string name = "log_" + ll;
                var nc = factory().factory_by_name("LOG").Create(m_nlstate.pool(), m_nlstate, name);
                register_link(name + ".I", ll);
                log().debug.op("    dynamic link {0}: <{1}>\n", ll, name);
                m_nlstate.register_device(nc.name(), nc);
            }
        }


        public void resolve_inputs()
        {
            log().verbose.op("Resolving inputs ...");

            // Netlist can directly connect input to input.
            // We therefore first park connecting inputs and retry
            // after all other terminals were connected.

            UInt32 tries = m_netlist_params.m_max_link_loops.op();
            while (!m_links.empty() && tries > 0)
            {
                for (int liIdx = 0; liIdx < m_links.Count;  )  //for (auto li = m_links.begin(); li != m_links.end(); )
                {
                    var li = m_links[liIdx];

                    string t1s = li.first();
                    string t2s = li.second();
                    detail.core_terminal_t t1 = find_terminal(t1s);
                    detail.core_terminal_t t2 = find_terminal(t2s);

                    //printf("%s %s\n", t1s.c_str(), t2s.c_str());
                    if (connect(t1, t2))
                    {
                        //li = m_links.erase(li);
                        m_links.erase(liIdx);
                    }
                    else
                    {
                        liIdx++;  //li++;
                    }
                }
                tries--;
            }

            if (tries == 0)
            {
                foreach (var link in m_links)
                    log().warning.op(nl_errstr_global.MF_CONNECTING_1_TO_2(setup().de_alias(link.first()), setup().de_alias(link.second())));

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
                bool is_nc = term.device() is devices.nld_nc_pin;  //bool is_nc(dynamic_cast< devices::NETLIB_NAME(nc_pin) *>(&term->device()) != nullptr);
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
                    log().error.op(nl_errstr_global.ME_TERMINAL_1_WITHOUT_NET(setup().de_alias(term.name())));
                    err = true;
                }
                else if (term.net().num_cons() == 0)
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

            if (err)
            {
                log().fatal.op(nl_errstr_global.MF_TERMINALS_WITHOUT_NET());
                throw new nl_exception(nl_errstr_global.MF_TERMINALS_WITHOUT_NET());
            }
        }


        //plib::psource_t::stream_ptr get_data_stream(const pstring &name);


        //factory::list_t &factory() { return m_factory; }
        //const factory::list_t &factory() const { return m_factory; }


        // helper - also used by nltool
        string resolve_alias(string name)
        {
            string temp = name;
            string ret;

            // FIXME: Detect endless loop
            do {
                ret = temp;
                var p = m_alias.find(ret);
                temp = (p != null ? p : "");
            } while (temp != "" && temp != ret);

            log().debug.op("{0}==>{1}\n", name, ret);
            return ret;
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
                foreach (var e in m_alias)
                {
                    // FIXME: this will resolve first one found
                    if (e.second() == ret)
                    {
                        temp = e.first();
                        break;
                    }
                }
            } while (temp != "" && temp != ret);

            log().debug.op("{0}==>{1}\n", alias, ret);
            return ret;
        }


        // needed by nltool
        //std::vector<pstring> get_terminals_for_device_name(const pstring &devname);


        //log_type &log();
        //const log_type &log() const;


        // needed by proxy
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


        // core net handling
        // ----------------------------------------------------------------------------------------
        // Device handling
        // ----------------------------------------------------------------------------------------
        void delete_empty_nets()
        {
            //netlist().nets().erase(
            //    std::remove_if(netlist().nets().begin(), netlist().nets().end(),
            //        [](owned_pool_ptr<detail::net_t> &x)
            //        {
            //            if (x->num_cons() == 0)
            //            {
            //                x->state().log().verbose("Deleting net {1} ...", x->name());
            //                return true;
            //            }
            //            else
            //                return false;
            //        }), netlist().nets().end());
            m_nlstate.nets().RemoveAll(x =>
            {
                if (x.num_cons() == 0)
                {
                    x.state().log().verbose.op("Deleting net {0} ...", x.name());
                    return true;
                }

                return false;
            });
        }


        // run preparation
        // ----------------------------------------------------------------------------------------
        // Run preparation
        // ----------------------------------------------------------------------------------------
        public void prepare_to_run()
        {
            string envlog = plib.pglobal.environment("NL_LOGS", "");

            if (envlog != "")
            {
                std.vector<string> loglist = plib.pglobal.psplit(envlog, ":");
                register_dynamic_log_devices(loglist);
            }

            // make sure the solver and parameters are started first!

            foreach (var e in m_device_factory)
            {
                if (factory().is_class<devices.nld_solver>(e.second) || factory().is_class<devices.nld_netlistparams>(e.second))
                {
                    m_nlstate.register_device(e.first, e.second.Create(nlstate().pool(), m_nlstate, e.first));
                }
            }

            log().debug.op("Searching for solver and parameters ...\n");

            var solver = m_nlstate.get_single_device<devices.nld_solver>("solver");
            m_netlist_params = m_nlstate.get_single_device<devices.nld_netlistparams>("parameter");


            // set default model parameters

            m_models.register_model(new plib.pfmt("NMOS_DEFAULT _(CAPMOD={0})").op(m_netlist_params.m_mos_capmodel.op()));
            m_models.register_model(new plib.pfmt("PMOS_DEFAULT _(CAPMOD={0})").op(m_netlist_params.m_mos_capmodel.op()));


            // create devices

            log().debug.op("Creating devices ...\n");
            foreach (var e in m_device_factory)
            {
                if (!factory().is_class<devices.nld_solver>(e.second) && !factory().is_class<devices.nld_netlistparams>(e.second))
                {
                    var dev = e.second.Create(m_nlstate.pool(), m_nlstate, e.first);
                    m_nlstate.register_device(dev.name(), dev);
                }
            }

            log().debug.op("Looking for unknown parameters ...\n");
            foreach (var p in m_param_values)
            {
                var f = m_params.find(p.first());
                if (f == null)  //m_params.end())
                {
                    if (plib.pglobal.endsWith(p.first(), nl_errstr_global.sHINT_NO_DEACTIVATE))
                    {
                        // FIXME: get device name, check for device
                        var dev = m_nlstate.find_device(plib.pglobal.replace_all(p.first(), nl_errstr_global.sHINT_NO_DEACTIVATE, ""));
                        if (dev == null)
                            log().warning.op(nl_errstr_global.MW_DEVICE_NOT_FOUND_FOR_HINT(p.first()));
                    }
                    else
                    {
                        log().warning.op(nl_errstr_global.MW_UNKNOWN_PARAMETER(p.first()));
                    }
                }
            }

            bool use_deactivate = m_netlist_params.m_use_deactivate.op();

            foreach (var d in m_nlstate.devices())
            {
                if (use_deactivate)
                {
                    var p = m_param_values.find(d.second.name() + nl_errstr_global.sHINT_NO_DEACTIVATE);
                    if (p != null)
                    {
                        //FIXME: check for errors ...
                        bool err = false;
                        var v = plib.pglobal.pstonum_ne_nl_fptype(true, p, out err);
                        if (err || plib.pglobal.abs(v - plib.pglobal.floor(v)) > nlconst.magic(1e-6) )
                        {
                            log().fatal.op(nl_errstr_global.MF_HND_VAL_NOT_SUPPORTED(p));
                            throw new nl_exception(nl_errstr_global.MF_HND_VAL_NOT_SUPPORTED(p));
                        }
                        // FIXME comparison with zero
                        d.second.set_hint_deactivate(v == nlconst.zero());
                    }
                }
                else
                {
                    d.second.set_hint_deactivate(false);
                }
            }

            // resolve inputs
            resolve_inputs();

            log().verbose.op("looking for two terms connected to rail nets ...");
            foreach (var t in m_nlstate.get_device_list<analog.nld_twoterm>())
            {
                if (t.m_N.net().isRailNet() && t.m_P.net().isRailNet())
                {
                    log().info.op(nl_errstr_global.MI_REMOVE_DEVICE_1_CONNECTED_ONLY_TO_RAILS_2_3(
                        t.name(), t.m_N.net().name(), t.m_P.net().name()));
                    t.m_N.net().remove_terminal(t.m_N);
                    t.m_P.net().remove_terminal(t.m_P);
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


        void merge_nets(detail.net_t thisnet, detail.net_t othernet)
        {
            log().debug.op("merging nets ...\n");
            if (othernet == thisnet)
            {
                log().warning.op(nl_errstr_global.MW_CONNECTING_1_TO_ITSELF(thisnet.name()));
                return; // Nothing to do
            }

            if (thisnet.isRailNet() && othernet.isRailNet())
            {
                log().fatal.op(nl_errstr_global.MF_MERGE_RAIL_NETS_1_AND_2(thisnet.name(), othernet.name()));
                throw new nl_exception(nl_errstr_global.MF_MERGE_RAIL_NETS_1_AND_2(thisnet.name(), othernet.name()));
            }

            if (othernet.isRailNet())
            {
                log().debug.op("othernet is railnet\n");
                merge_nets(othernet, thisnet);
            }
            else
            {
                othernet.move_connections(thisnet);
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
                t2.net().add_terminal(t1);
            }
            else if (t1.has_net())
            {
                log().debug.op("T1 has net\n");
                t1.net().add_terminal(t2);
            }
            else
            {
                log().debug.op("adding analog net ...\n");
                // FIXME: Nets should have a unique name
                var anet = new analog_net_t(m_nlstate,"net." + t1.name());  //auto anet = nlstate().pool().make_owned<analog_net_t>(m_nlstate,"net." + t1.name());
                var anetp = anet;  //auto anetp = anet.get();
                m_nlstate.register_net(anet);  //plib::owned_ptr<analog_net_t>(anet, true));
                t1.set_net(anetp);
                anetp.add_terminal(t2);
                anetp.add_terminal(t1);
            }
        }


        void connect_input_output(detail.core_terminal_t in_, detail.core_terminal_t out_)
        {
            if (out_.is_analog() && in_.is_logic())
            {
                var proxy = get_a_d_proxy(in_);

                out_.net().add_terminal(proxy.proxy_term());
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
                    out_.net().add_terminal(in_);
            }
        }


        void connect_terminal_output(terminal_t in_, detail.core_terminal_t out_)
        {
            if (out_.is_analog())
            {
                log().debug.op("connect_terminal_output: {0} {1}\n", in_.name(), out_.name());
                // no proxy needed, just merge existing terminal net
                if (in_.has_net())
                    merge_nets(out_.net(), in_.net());
                else
                    out_.net().add_terminal(in_);
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
                if (t1.net().isRailNet())
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
                if (t2.net().isRailNet())
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

            // FIXME: in.type() will have thrown already
            // log().fatal(MF_UNKNOWN_OBJECT_TYPE_1(static_cast<unsigned>(in.type())));
            return "Error"; // Tease gcc
        }


        devices.nld_base_proxy get_d_a_proxy(detail.core_terminal_t out_)
        {
            nl_config_global.nl_assert(out_.is_logic());

            var out_cast = (logic_output_t)out_;  //auto &out_cast = static_cast<logic_output_t &>(out);
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

            out_.net().add_terminal(new_proxy.in_());

            var proxy = new_proxy;  //auto proxy(new_proxy.get());
            if (!m_proxies.insert(out_, proxy))
                throw new nl_exception(nl_errstr_global.MF_DUPLICATE_PROXY_1(out_.name()));

            m_nlstate.register_device(new_proxy.name(), new_proxy);
            return proxy;
        }


        devices.nld_base_proxy get_a_d_proxy(detail.core_terminal_t inp)
        {
            nl_config_global.nl_assert(inp.is_logic());

            var incast = (logic_input_t)inp;  //auto &incast = dynamic_cast<logic_input_t &>(inp);
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
            ret.out_().net().add_terminal(inp);
            m_nlstate.register_device(new_proxy.name(), new_proxy);
            return ret;
        }


        //detail::core_terminal_t &resolve_proxy(detail::core_terminal_t &term);
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


    class logic_family_std_proxy_t : logic_family_desc_t
    {
        public logic_family_std_proxy_t() : base() { }


        public logic_family_std_proxy_t get() { return this; }  // for smart ptr


        public override devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_state_t anetlist, string name, logic_output_t proxied)
        {
            return new devices.nld_d_to_a_proxy(anetlist, name, proxied);  //return anetlist.make_object<devices::nld_d_to_a_proxy>(anetlist, name, proxied);
        }
        public override devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_state_t anetlist, string name, logic_input_t proxied)
        {
            return new devices.nld_a_to_d_proxy(anetlist, name, proxied);  //return anetlist.make_object<devices::nld_a_to_d_proxy>(anetlist, name, proxied);
        }
    }
}
