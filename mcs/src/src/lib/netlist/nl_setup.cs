// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using link_t = System.Collections.Generic.KeyValuePair<string, string>;  //using link_t = std::pair<pstring, pstring>;
using log_type = mame.plib.plog_base<mame.netlist.callbacks_t>;//, NL_DEBUG>;
using model_map_t = mame.std.unordered_map<string, string>;
using netlist_base_t = mame.netlist.netlist_state_t;
using nl_double = System.Double;
using source_t_list_t = mame.std.vector<mame.netlist.source_t>;  //using list_t = std::vector<std::unique_ptr<source_t>>;


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
        public static void NET_MODEL(setup_t setup, string model) { setup.register_model(model); }

        //#define ALIAS(alias, name)                                                        \
        //    setup.register_alias(# alias, # name);
        public static void ALIAS(setup_t setup, string alias, string name) { setup.register_alias(alias, name); }

        //#define DIPPINS(pin1, ...)                                                          \
        //        setup.register_dippins_arr( # pin1 ", " # __VA_ARGS__);
        public static void DIPPINS(setup_t setup, params string [] pin1) { setup.register_dippins_arr(string.Join(", ", pin1)); }

        /* to be used to reference new library truthtable devices */
        //#define NET_REGISTER_DEV(type, name)                                            \
        //        setup.register_dev(# type, # name);
        public static void NET_REGISTER_DEV(setup_t setup, string type, string name) { setup.register_dev(type, name); }

        //#define NET_CONNECT(name, input, output)                                        \
        //        setup.register_link(# name "." # input, # output);

        //#define NET_C(term1, ...)                                                       \
        //        setup.register_link_arr( # term1 ", " # __VA_ARGS__);
        public static void NET_C(setup_t setup, params string [] term1) { setup.register_link_arr(string.Join(", ", term1)); }

        //#define PARAM(name, val)                                                        \
        //        setup.register_param(# name, val);
        public static void PARAM(setup_t setup, string name, int val) { setup.register_param(name, val); }
        public static void PARAM(setup_t setup, string name, double val) { setup.register_param(name, val); }

        //#define HINT(name, val)                                                        \
        //        setup.register_param(# name ".HINT_" # val, 1);

        //#define NETDEV_PARAMI(name, param, val)                                         \
        //        setup.register_param(# name "." # param, val);
        public static void NETDEV_PARAMI(setup_t setup, string name, string param, int val) { setup.register_param(name + "." + param, val); }
        public static void NETDEV_PARAMI(setup_t setup, string name, string param, double val) { setup.register_param(name + "." + param, val); }
        public static void NETDEV_PARAMI(setup_t setup, string name, string param, string val) { setup.register_param(name + "." + param, val); }

        //#define NETLIST_NAME(name) netlist ## _ ## name

        //#define NETLIST_EXTERNAL(name)                                                 \
        //        void NETLIST_NAME(name)(netlist::setup_t &setup);

        //#define NETLIST_START(name)                                                    \
        //void NETLIST_NAME(name)(netlist::setup_t &setup)                               \
        //{
        public static void NETLIST_START() {}

        //#define NETLIST_END()  }
        public static void NETLIST_END() {}

        //#define LOCAL_SOURCE(name)                                                     \
        //        setup.register_source(plib::make_unique_base<netlist::source_t, netlist::source_proc_t>(setup, # name, &NETLIST_NAME(name)));
        public static void LOCAL_SOURCE(setup_t setup, string name, source_proc_t.setup_func_delegate netlist_name) { setup.register_source(new netlist.source_proc_t(setup, name, netlist_name)); }

        //#define LOCAL_LIB_ENTRY(name)                                                  \
        //        LOCAL_SOURCE(name)                                                     \
        //        setup.register_lib_entry(# name, __FILE__);
        public static void LOCAL_LIB_ENTRY(setup_t setup, string name, source_proc_t.setup_func_delegate nld_name)
        {
            LOCAL_SOURCE(setup, name, nld_name);
            setup.register_lib_entry(name, "__FILE__");
        }

        //#define INCLUDE(name)                                                          \
        //        setup.include(# name);
        public static void INCLUDE(setup_t setup, string name) { setup.include(name); }

        //#define SUBMODEL(model, name)                                                  \
        //        setup.namespace_push(# name);                                          \
        //        NETLIST_NAME(model)(setup);                                            \
        //        setup.namespace_pop();

        //#define OPTIMIZE_FRONTIER(attach, r_in, r_out)                                  \
        //        setup.register_frontier(# attach, r_in, r_out);


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
        //        desc.desc.push_back(x);
        public static void TT_HEAD(string x) { TRUTHTABLE_desc.desc.push_back(x); }

        //#define TT_LINE(x) \
        //        desc.desc.push_back(x);
        public static void TT_LINE(string x) { TRUTHTABLE_desc.desc.push_back(x); }

        //#define TT_FAMILY(x) \
        //        desc.family = x;
        public static void TT_FAMILY(string x) { TRUTHTABLE_desc.family = x; }

        //#define TRUTHTABLE_END() \
        //        setup.tt_factory_create(desc, __FILE__);       \
        //    }
        public static void TRUTHTABLE_END(setup_t setup) { setup.tt_factory_create(TRUTHTABLE_desc, "__FILE__");  TRUTHTABLE_desc = null; }
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
    }


    // ----------------------------------------------------------------------------------------
    // A Generic netlist sources implementation
    // ----------------------------------------------------------------------------------------

    public abstract class source_t
    {
        public enum type_t
        {
            SOURCE,
            DATA
        }


        //using list_t = std::vector<std::unique_ptr<source_t>>;


        setup_t m_setup;
        type_t m_type;


        public source_t(setup_t setup, type_t type = type_t.SOURCE)
        {
            m_setup = setup;
            m_type = type;
        }

        //~source_t() { }


        public virtual bool parse(string name)
        {
            if (m_type != type_t.SOURCE)
            {
                return false;
            }
            else
            {
                throw new emu_unimplemented();
#if false
                var rstream = stream(name);
                plib.putf8_reader reader(rstream);
                return m_setup.parse_stream(reader, name);
#endif
            }
        }


        protected abstract plib.pistream stream(string name);  //virtual std::unique_ptr<plib::pistream> stream(const pstring &name) = 0;

        protected setup_t setup() { return m_setup; }

        //type_t type() const { return m_type; }
    }


    // ----------------------------------------------------------------------------------------
    // setup_t
    // ----------------------------------------------------------------------------------------
    public class setup_t //: plib::nocopyassignmove
    {
        //using link_t = std::pair<pstring, pstring>;


        std.unordered_map<string, factory.element_t> m_device_factory = new std.unordered_map<string, factory.element_t>();

        std.unordered_map<string, string> m_alias = new std.unordered_map<string, string>();
        std.unordered_map<string, string> m_param_values = new std.unordered_map<string, string>();
        std.unordered_map<string, detail.core_terminal_t> m_terminals = new std.unordered_map<string, detail.core_terminal_t>();


        netlist_t m_netlist;
        std.unordered_map<string, param_ref_t> m_params = new std.unordered_map<string, param_ref_t>();
        std.vector<link_t> m_links = new std.vector<link_t>();
        factory.list_t m_factory;
        std.unordered_map<string, string> m_models = new std.unordered_map<string, string>();

        std.stack<string> m_namespace_stack = new std.stack<string>();  //std::stack<pstring>                         m_namespace_stack;
        source_t_list_t m_sources = new source_t_list_t();
        //std::vector<plib::ppreprocessor::define_t>  m_defines;

        UInt32 m_proxy_cnt;
        UInt32 m_frontier_cnt;


        public setup_t(netlist_t netlist)
        {
            m_netlist = netlist;
            m_factory = new factory.list_t(this);
            m_proxy_cnt = 0;
            m_frontier_cnt = 0;
        }

        //~setup_t()
        //{
        //    m_links.clear();
        //    m_alias.clear();
        //    m_params.clear();
        //    m_terminals.clear();
        //    m_param_values.clear();
        //
        //    m_sources.clear();
        //}


        netlist_state_t netlist() { return m_netlist.nlstate(); }


        public netlist_t exec() { return m_netlist; }


        string build_fqn(string obj_name)
        {
            if (m_namespace_stack.empty())
                //return netlist().name() + "." + obj_name;
                return obj_name;
            else
                return m_namespace_stack.top() + "." + obj_name;
        }


        public void register_param(string name, param_t param)
        {
            if (!m_params.insert(param.name(), new param_ref_t(param.name(), param.device(), param)))
                log().fatal.op(nl_errstr_global.MF_1_ADDING_PARAMETER_1_TO_PARAMETER_LIST, name);
        }


        public string get_initial_param_val(string name, string def)
        {
            var i = m_param_values.find(name);
            if (i != null)//m_param_values.end())
                return i;
            else
                return def;
        }


        public void register_term(detail.core_terminal_t term)
        {
            if (!m_terminals.insert(term.name(), term))
                log().fatal.op(nl_errstr_global.MF_2_ADDING_1_2_TO_TERMINAL_LIST, termtype_as_str(term), term.name());
            log().debug.op("{0} {1}\n", termtype_as_str(term), term.name());
        }


        public void register_dev(string classname, string name)
        {
            var f = factory().factory_by_name(classname);
            if (f == null)
                log().fatal.op(nl_errstr_global.MF_1_CLASS_1_NOT_FOUND, classname);
            /* make sure we parse macro library entries */
            f.macro_actions(netlist(), name);
            string key = build_fqn(name);
            if (device_exists(key))
                log().fatal.op(nl_errstr_global.MF_1_DEVICE_ALREADY_EXISTS_1, name);
            m_device_factory[key] = f;
        }


        public void register_lib_entry(string name, string sourcefile)
        {
            factory().register_device(new factory.library_element_t(this, name, name, "", sourcefile));  //plib::make_unique_base<factory::element_t, factory::library_element_t>(*this, name, name, "", sourcefile));
        }


        public void register_model(string model_in)
        {
            var pos = model_in.find(" ");
            if (pos == -1)
                log().fatal.op(nl_errstr_global.MF_1_UNABLE_TO_PARSE_MODEL_1, model_in);
            string model = plib.pstring_global.ucase(plib.pstring_global.trim(plib.pstring_global.left(model_in, pos)));
            string def = plib.pstring_global.trim(model_in.substr(pos + 1));
            string modelOLD = model_in.left(pos).trim().ucase();
            string defOLD = model_in.substr(pos + 1).trim();
            if (!m_models.insert(model, def))
                log().fatal.op(nl_errstr_global.MF_1_MODEL_ALREADY_EXISTS_1, model_in);
        }


        public void register_alias(string alias, string out_)
        {
            throw new emu_unimplemented();
        }


        public void register_dippins_arr(string terms)
        {
            throw new emu_unimplemented();
#if false
            std::vector<pstring> list(plib::psplit(terms,", "));
            if (list.size() == 0 || (list.size() % 2) == 1)
                log().fatal(MF_1_DIP_PINS_MUST_BE_AN_EQUAL_NUMBER_OF_PINS_1,
                        build_fqn(""));
            std::size_t n = list.size();
            for (std::size_t i = 0; i < n / 2; i++)
            {
                register_alias(plib::pfmt("{1}")(i+1), list[i * 2]);
                register_alias(plib::pfmt("{1}")(n-i), list[i * 2 + 1]);
            }
#endif
        }


        public void register_alias_nofqn(string alias, string out_)
        {
            if (!m_alias.insert(alias, out_))
                log().fatal.op(nl_errstr_global.MF_1_ADDING_ALI1_TO_ALIAS_LIST, alias);
        }


        public void register_link_arr(string terms)
        {
            std.vector<string> list = new std.vector<string>(terms.Split(new string[] { ", " }, StringSplitOptions.None));  //plib::psplit(terms, ", ");
            if (list.size() < 2)
                log().fatal.op(nl_errstr_global.MF_2_NET_C_NEEDS_AT_LEAST_2_TERMINAL);
            for (int i = 1; i < list.size(); i++)
            {
                register_link(list[0], list[i]);
            }
        }

        public void register_link_fqn(string sin, string sout)
        {
            link_t temp = new link_t(sin, sout);
            log().debug.op("link {0} <== {1}\n", sin, sout);
            m_links.push_back(temp);
        }

        public void register_link(string sin, string sout)
        {
            register_link_fqn(build_fqn(sin), build_fqn(sout));
        }

        public void register_param(string param, string value)
        {
            string fqn = build_fqn(param);

            var idx = m_param_values.find(fqn);
            if (idx == null)  //m_param_values.end())
            {
                if (!m_param_values.insert(fqn, value))
                    log().fatal.op(nl_errstr_global.MF_1_ADDING_PARAMETER_1_TO_PARAMETER_LIST, param);
            }
            else
            {
                log().warning.op(nl_errstr_global.MW_3_OVERWRITING_PARAM_1_OLD_2_NEW_3, fqn, idx, value);
                m_param_values[fqn] = value;
            }
        }

        public void register_param(string param, double value)
        {
            if (std.abs(value - std.floor(value)) > 1e-30 || std.abs(value) > 1e9)
                register_param(param, string.Format("{0:f9}", value));  //register_param(param, plib::pfmt("{1:.9}").e(value));
            else
                register_param(param, string.Format("{0}", (long)value));
        }


        //void register_frontier(const pstring &attach, const double r_IN, const double r_OUT);

        //void remove_connections(const pstring &attach);


        public bool connect(detail.core_terminal_t t1_in, detail.core_terminal_t t2_in)
        {
            log().debug.op("Connecting {0} to {1}\n", t1_in.name(), t2_in.name());
            detail.core_terminal_t t1 = resolve_proxy(t1_in);
            detail.core_terminal_t t2 = resolve_proxy(t2_in);
            bool ret = true;

            if (t1.is_type(detail.terminal_type.OUTPUT) && t2.is_type(detail.terminal_type.INPUT))
            {
                if (t2.has_net() && t2.net().isRailNet())
                    log().fatal.op(nl_errstr_global.MF_1_INPUT_1_ALREADY_CONNECTED, t2.name());
                connect_input_output(t2, t1);
            }
            else if (t1.is_type(detail.terminal_type.INPUT) && t2.is_type(detail.terminal_type.OUTPUT))
            {
                if (t1.has_net()  && t1.net().isRailNet())
                    log().fatal.op(nl_errstr_global.MF_1_INPUT_1_ALREADY_CONNECTED, t1.name());
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
                //netlist().error("Connecting {1} to {2} not supported!\n", t1.name(), t2.name());
            return ret;
        }


        public bool device_exists(string name)
        {
            return m_device_factory.find(name) != null;
        }


        public param_t find_param(string param_in, bool required = true)
        {
            string param_in_fqn = build_fqn(param_in);

            string outname = resolve_alias(param_in_fqn);
            var ret = m_params.find(outname);
            if (ret == null && required)
                log().fatal.op(nl_errstr_global.MF_2_PARAMETER_1_2_NOT_FOUND, param_in_fqn, outname);
            if (ret != null)
                log().debug.op("Found parameter {0}\n", outname);
            return ret == null ? null : ret.m_param;
        }


        void register_dynamic_log_devices()
        {
            string env = ""; //plib::util::environment("NL_LOGS", "");

            if (env != "")
            {
                log().debug.op("Creating dynamic logs ...");
                var loglist = env.Split(':');  //std::vector<pstring> loglist(plib::psplit(env, ":"));
                foreach (string ll in loglist)
                {
                    string name = "log_" + ll;
                    var nc = factory().factory_by_name("LOG").Create(netlist(), name);
                    register_link(name + ".I", ll);
                    log().debug.op("    dynamic link {0}: <{1}>\n", ll, name);
                    m_netlist.nlstate().add_dev(nc);
                }
            }
        }


        public void resolve_inputs()
        {
            log().verbose.op("Resolving inputs ...");

            /* Netlist can directly connect input to input.
             * We therefore first park connecting inputs and retry
             * after all other terminals were connected.
             */
            int tries = nl_config_global.NL_MAX_LINK_RESOLVE_LOOPS;
            while (m_links.size() > 0 && tries >  0)
            {
                for (int liIdx = 0; liIdx < m_links.Count;  )  //for (auto li = m_links.begin(); li != m_links.end(); )
                {
                    var li = m_links[liIdx];

                    string t1s = li.first();
                    string t2s = li.second();
                    detail.core_terminal_t t1 = find_terminal(t1s);
                    detail.core_terminal_t t2 = find_terminal(t2s);

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
                    log().warning.op(nl_errstr_global.MF_2_CONNECTING_1_TO_2, link.first(), link.second());

                log().fatal.op(nl_errstr_global.MF_0_LINK_TRIES_EXCEEDED);
            }

            log().verbose.op("deleting empty nets ...");

            // delete empty nets

            delete_empty_nets();

            string errstr = "";

            log().verbose.op("looking for terminals not connected ...");
            foreach (var i in m_terminals)
            {
                detail.core_terminal_t term = i.second();
                if (!term.has_net() && term.device() is devices.nld_dummy_input)  //dynamic_cast< devices::NETLIB_NAME(dummy_input) *>(&term->device()) != nullptr)
                    log().warning.op(nl_errstr_global.MW_1_DUMMY_1_WITHOUT_CONNECTIONS, term.name());
                else if (!term.has_net())
                    errstr += new plib.pfmt("Found terminal {0} without a net\n").op(term.name());
                else if (term.net().num_cons() == 0)
                    log().warning.op(nl_errstr_global.MW_1_TERMINAL_1_WITHOUT_CONNECTIONS, term.name());
            }

            //FIXME: error string handling
            if (errstr != "")
                log().fatal.op("{0}", errstr);
        }


        /* handle namespace */

        //void namespace_push(const pstring &aname);
        //void namespace_pop();

        /* parse a source */

        public void include(string netlist_name)
        {
            foreach (var source in m_sources)
            {
                if (source.parse(netlist_name))
                    return;
            }

            log().fatal.op(nl_errstr_global.MF_1_NOT_FOUND_IN_SOURCE_COLLECTION, netlist_name);
        }

        //std::unique_ptr<plib::pistream> get_data_stream(const pstring &name);

        //bool parse_stream(plib::putf8_reader &istrm, const pstring &name);

        /* register a source */

        public void register_source(source_t src)  //std::unique_ptr<source_t> &&src)
        {
            m_sources.push_back(src);  //std::move(src));
        }

        //void register_define(pstring def, pstring val) { m_defines.push_back(plib::ppreprocessor::define_t(def, val)); }
        //void register_define(pstring defstr);


        //factory::list_t &factory() { return m_factory; }
        //const factory::list_t &factory() const { return m_factory; }
        public factory.list_t factory() { return m_factory; }


        /* model / family related */

        string model_value_str(model_map_t map, string entity)
        {
            string ret = "";

            if (entity != plib.pstring_global.ucase(entity))
                log().fatal.op(nl_errstr_global.MF_2_MODEL_PARAMETERS_NOT_UPPERCASE_1_2, entity, model_string(map));
            if (map.find(entity) == null)
                log().fatal.op(nl_errstr_global.MF_2_ENTITY_1_NOT_FOUND_IN_MODEL_2, entity, model_string(map));
            else
                ret = map[entity];

            return ret;
        }


        public nl_double model_value(model_map_t map, string entity)
        {
            string tmp = model_value_str(map, entity);

            nl_double factor = nl_config_global.NL_FCONST(1.0);
            var p = tmp[tmp.Length - 1];  //auto p = std::next(tmp.begin(), static_cast<pstring::difference_type>(tmp.size() - 1));
            switch (p)
            {
                case 'M': factor = 1e6; break;
                case 'k': factor = 1e3; break;
                case 'm': factor = 1e-3; break;
                case 'u': factor = 1e-6; break;
                case 'n': factor = 1e-9; break;
                case 'p': factor = 1e-12; break;
                case 'f': factor = 1e-15; break;
                case 'a': factor = 1e-18; break;
                default:
                    if (p < '0' || p > '9')
                        log().fatal.op(nl_errstr_global.MF_1_UNKNOWN_NUMBER_FACTOR_IN_1, entity);
                    break;
            }

            if (factor != nl_config_global.NL_FCONST(1.0))
                tmp = plib.pstring_global.left(tmp, tmp.Length - 1);

            // FIXME: check for errors
            return plib.pstring_global.pstonum_double(tmp) * factor;
        }


        static string model_string(model_map_t map)
        {
            string ret = map["COREMODEL"] + "(";
            foreach (var i in map)
                ret = ret + i.first() + "=" + i.second() + " ";

            return ret + ")";
        }


        public void model_parse(string model_in, model_map_t map)
        {
            string model = model_in;
            int pos = 0;
            string key = "";

            while (true)
            {
                pos = model.find("(");
                if (pos != -1) break;

                key = plib.pstring_global.ucase(model);
                var i = m_models.find(key);
                if (i == null)
                    log().fatal.op(nl_errstr_global.MF_1_MODEL_NOT_FOUND, model);
                model = i;
            }

            string xmodel = plib.pstring_global.left(model, pos);

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
                    log().fatal.op(nl_errstr_global.MF_1_MODEL_NOT_FOUND, model_in);
            }

            string remainder = plib.pstring_global.trim(model.substr(pos + 1));
            if (!plib.pstring_global.endsWith(remainder, ")"))
                log().fatal.op(nl_errstr_global.MF_1_MODEL_ERROR_1, model);
            // FIMXE: Not optimal
            remainder = plib.pstring_global.left(remainder, remainder.Length - 1);

            var pairs = remainder.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string pe in pairs)
            {
                var pose = pe.find("=");
                if (pose == -1)
                    log().fatal.op(nl_errstr_global.MF_1_MODEL_ERROR_ON_PAIR_1, model);
                map[plib.pstring_global.ucase(plib.pstring_global.left(pe, pose))] = pe.substr(pose + 1);
            }
        }


        public logic_family_desc_t family_from_model(string model)
        {
            model_map_t map = new model_map_t();
            model_parse(model, map);

            if (model_value_str(map, "TYPE") == "TTL")
                return nl_base_global.family_TTL();
            if (model_value_str(map, "TYPE") == "CD4XXX")
                return nl_base_global.family_CD4XXX();

            foreach (var e in netlist().family_cache)
            {
                if (e.first() == model)
                    return e.second().get();
            }

            var ret = new logic_family_std_proxy_t();  //plib::make_unique_base<logic_family_desc_t, logic_family_std_proxy_t>();

            ret.fixed_V = model_value(map, "FV");
            ret.low_thresh_PCNT = model_value(map, "IVL");
            ret.high_thresh_PCNT = model_value(map, "IVH");
            ret.low_VO = model_value(map, "OVL");
            ret.high_VO = model_value(map, "OVH");
            ret.R_low = model_value(map, "ORL");
            ret.R_high = model_value(map, "ORH");

            var retp = ret.get();

            netlist().family_cache.emplace_back(new KeyValuePair<string, logic_family_desc_t>(model, ret));

            return retp;
        }


        public void tt_factory_create(tt_desc desc, string sourcefile)
        {
            devices.nlid_truthtable_global.tt_factory_create(this, desc, sourcefile);  //devices.tt_factory_create(this, desc, sourcefile);
        }


        /* helper - also used by nltool */
        string resolve_alias(string name)
        {
            string temp = name;
            string ret;

            /* FIXME: Detect endless loop */
            do {
                ret = temp;
                var p = m_alias.find(ret);
                temp = (p != null ? p : "");
            } while (temp != "" && temp != ret);

            log().debug.op("{0}==>{1}\n", name, ret);
            return ret;
        }


        public log_type log() { return netlist().log(); }


        /* needed by proxy */
        //detail::core_terminal_t *find_terminal(const pstring &outname_in, const detail::terminal_type atype, bool required = true);


        detail.core_terminal_t find_terminal(string terminal_in, bool required = true)
        {
            string tname = resolve_alias(terminal_in);
            var ret = m_terminals.find(tname);
            /* look for default */
            if (ret == null)
            {
                /* look for ".Q" std output */
                ret = m_terminals.find(tname + ".Q");
            }

            detail.core_terminal_t term = (ret == null ? null : ret);

            if (term == null && required)
                log().fatal.op(nl_errstr_global.MF_2_TERMINAL_1_2_NOT_FOUND, terminal_in, tname);
            if (term != null)
                log().debug.op("Found input {0}\n", tname);

            return term;
        }


        /* core net handling */
        // ----------------------------------------------------------------------------------------
        // Device handling
        // ----------------------------------------------------------------------------------------
        void delete_empty_nets()
        {
            //netlist().nets().erase(
            //    std::remove_if(netlist().nets().begin(), netlist().nets().end(),
            //        [](plib::owned_ptr<detail::net_t> &x)
            //        {
            //            if (x->num_cons() == 0)
            //            {
            //                x->state().log().verbose("Deleting net {1} ...", x->name());
            //                return true;
            //            }
            //            else
            //                return false;
            //        }), netlist().nets().end());
            netlist().nets().RemoveAll(x =>
            {
                if (x.num_cons() == 0)
                {
                    x.state().log().verbose.op("Deleting net {0} ...", x.name());
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }


        // ----------------------------------------------------------------------------------------
        // Run preparation
        // ----------------------------------------------------------------------------------------
        public void prepare_to_run()
        {
            register_dynamic_log_devices();

            /* load the library ... */

            /* make sure the solver and parameters are started first! */

            foreach (var e in m_device_factory)
            {
                if (factory().is_class<devices.nld_solver>(e.second()) || factory().is_class<devices.nld_netlistparams>(e.second()))
                {
                    var dev = e.second().Create(netlist(), e.first());
                    m_netlist.nlstate().add_dev(dev);
                }
            }

            log().debug.op("Searching for solver and parameters ...\n");

            var solver = netlist().get_single_device<devices.nld_solver>("solver");
            netlist().params_ = netlist().get_single_device<devices.nld_netlistparams>("parameter");

            /* create devices */

            log().debug.op("Creating devices ...\n");
            foreach (var e in m_device_factory)
            {
                if (!factory().is_class<devices.nld_solver>(e.second()) && !factory().is_class<devices.nld_netlistparams>(e.second()))
                {
                    var dev = e.second().Create(netlist(), e.first());
                    m_netlist.nlstate().add_dev(dev);
                }
            }

            bool use_deactivate = netlist().params_.use_deactivate.op() ? true : false;

            foreach (var d in netlist().devices())
            {
                if (use_deactivate)
                {
                    var p = m_param_values.find(d.name() + ".HINT_NO_DEACTIVATE");
                    if (p != null)
                    {
                        //FIXME: check for errors ...
                        double v = plib.pstring_global.pstonum_double(p);
                        if (std.abs(v - std.floor(v)) > 1e-6 )
                            log().fatal.op(nl_errstr_global.MF_1_HND_VAL_NOT_SUPPORTED, p);
                        d.set_hint_deactivate(v == 0.0);
                    }
                }
                else
                {
                    d.set_hint_deactivate(false);
                }
            }

            /* resolve inputs */
            resolve_inputs();

            log().verbose.op("looking for two terms connected to rail nets ...");
            foreach (var t in m_netlist.nlstate().get_device_list<analog.nld_twoterm>())
            {
                if (t.N.net().isRailNet() && t.P.net().isRailNet())
                {
                    log().warning.op(nl_errstr_global.MW_3_REMOVE_DEVICE_1_CONNECTED_ONLY_TO_RAILS_2_3,
                        t.name(), t.N.net().name(), t.P.net().name());
                    t.N.net().remove_terminal(t.N);
                    t.P.net().remove_terminal(t.P);
                    m_netlist.nlstate().remove_dev(t);
                }
            }

            log().verbose.op("initialize solver ...\n");

            if (solver == null)
            {
                foreach (var p in netlist().nets())
                {
                    if (p.is_analog())
                        log().fatal.op(nl_errstr_global.MF_0_NO_SOLVER);
                }
            }
            else
            {
                solver.post_start();
            }

            foreach (var n in netlist().nets())
            {
                foreach (var term in n.core_terms)
                {
                    //core_device_t *dev = reinterpret_cast<core_device_t *>(term->m_delegate.object());
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
                log().warning.op(nl_errstr_global.MW_1_CONNECTING_1_TO_ITSELF, thisnet.name());
                return; // Nothing to do
            }

            if (thisnet.isRailNet() && othernet.isRailNet())
                log().fatal.op(nl_errstr_global.MF_2_MERGE_RAIL_NETS_1_AND_2, thisnet.name(), othernet.name());

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
                var anet = new analog_net_t(netlist(), "net." + t1.name());  //plib::palloc<analog_net_t>(netlist(),"net." + t1.name());
                netlist().register_net(anet);  //plib::owned_ptr<analog_net_t>(anet, true));
                t1.set_net(anet);
                anet.add_terminal(t2);
                anet.add_terminal(t1);
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
                /* no proxy needed, just merge existing terminal net */
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
                log().fatal.op(nl_errstr_global.MF_1_OBJECT_OUTPUT_TYPE_1, out_.name());
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
                log().fatal.op(nl_errstr_global.MF_1_OBJECT_INPUT_TYPE_1, inp.name());
            }
        }


        static detail.core_terminal_t resolve_proxy(detail.core_terminal_t term)
        {
            if (term.is_logic())
            {
                logic_t out_ = (logic_t)term;
                if (out_.has_proxy())
                    return out_.get_proxy().proxy_term();
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
                    foreach (var t in t1.net().core_terms)
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
                    foreach (var t in t2.net().core_terms)
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
        string termtype_as_str(detail.core_terminal_t in_)
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

            log().fatal.op(nl_errstr_global.MF_1_UNKNOWN_OBJECT_TYPE_1, in_.type());
            return "Error";
        }


        devices.nld_base_proxy get_d_a_proxy(detail.core_terminal_t out_)
        {
            nl_base_global.nl_assert(out_.is_logic());

            logic_output_t out_cast = (logic_output_t)out_;
            devices.nld_base_proxy proxy = out_cast.get_proxy();

            if (proxy == null)
            {
                // create a new one ...
                string x = new plib.pfmt("proxy_da_{0}_{1}").op(out_.name(), m_proxy_cnt);
                var new_proxy = out_cast.logic_family().create_d_a_proxy(netlist(), x, out_cast);
                m_proxy_cnt++;

                //new_proxy->start_dev();

                /* connect all existing terminals to new net */

                foreach (var p in out_.net().core_terms)
                {
                    p.clear_net(); // de-link from all nets ...
                    if (!connect(new_proxy.proxy_term(), p))
                        log().fatal.op(nl_errstr_global.MF_2_CONNECTING_1_TO_2, new_proxy.proxy_term().name(), p.name());
                }
                out_.net().core_terms.clear(); // clear the list

                out_.net().add_terminal(new_proxy.in_());
                out_cast.set_proxy(proxy);

                proxy = new_proxy;

                m_netlist.nlstate().add_dev(new_proxy);
            }
            return proxy;
        }


        devices.nld_base_proxy get_a_d_proxy(detail.core_terminal_t inp)
        {
            nl_base_global.nl_assert(inp.is_logic());

            logic_input_t incast = (logic_input_t)inp;
            devices.nld_base_proxy proxy = incast.get_proxy();

            if (proxy != null)
            {
                return proxy;
            }
            else
            {
                log().debug.op("connect_terminal_input: connecting proxy\n");
                string x = new plib.pfmt("proxy_ad_{0}_{1}").op(inp.name(), m_proxy_cnt);
                var new_proxy = incast.logic_family().create_a_d_proxy(netlist(), x, incast);
                //auto new_proxy = plib::owned_ptr<devices::nld_a_to_d_proxy>::Create(netlist(), x, &incast);
                incast.set_proxy(new_proxy);
                m_proxy_cnt++;

                var ret = new_proxy;

                /* connect all existing terminals to new net */

                if (inp.has_net())
                {
                    foreach (var p in inp.net().core_terms)
                    {
                        p.clear_net(); // de-link from all nets ...
                        if (!connect(ret.proxy_term(), p))
                            log().fatal.op(nl_errstr_global.MF_2_CONNECTING_1_TO_2, ret.proxy_term().name(), p.name());
                    }
                    inp.net().core_terms.clear(); // clear the list
                }
                ret.out_().net().add_terminal(inp);
                m_netlist.nlstate().add_dev(new_proxy);
                return ret;
            }
        }
    }


    // ----------------------------------------------------------------------------------------
    // base sources
    // ----------------------------------------------------------------------------------------

    //class source_string_t : public source_t
    //{
    //public:
    //
    //    source_string_t(setup_t &setup, const pstring &source)
    //    : source_t(setup), m_str(source)
    //    {
    //    }
    //
    //    virtual std::unique_ptr<plib::pistream> stream(const pstring &name) override;
    //
    //private:
    //    pstring m_str;
    //};

    //class source_file_t : public source_t
    //{
    //public:
    //
    //    source_file_t(setup_t &setup, const pstring &filename)
    //    : source_t(setup), m_filename(filename)
    //    {
    //    }
    //
    //    virtual std::unique_ptr<plib::pistream> stream(const pstring &name) override;
    //
    //private:
    //    pstring m_filename;
    //};
    //
    //class source_mem_t : public source_t
    //{
    //public:
    //    source_mem_t(setup_t &setup, const char *mem)
    //    : source_t(setup), m_str(mem, pstring::UTF8)
    //    {
    //    }
    //
    //    virtual std::unique_ptr<plib::pistream> stream(const pstring &name) override;
    //
    //private:
    //    pstring m_str;
    //};


    public class source_proc_t : source_t
    {
        public delegate void setup_func_delegate(setup_t setup);


        setup_func_delegate m_setup_func;  //void (*m_setup_func)(setup_t &);
        string m_setup_func_name;


        public source_proc_t(setup_t setup, string name, setup_func_delegate setup_func)  //void (*setup_func)(setup_t &))
            : base(setup)
        {
            m_setup_func = setup_func;
            m_setup_func_name = name;
        }


        public override bool parse(string name)
        {
            if (name == m_setup_func_name)
            {
                m_setup_func(setup());
                return true;
            }
            else
            {
                return false;
            }
        }


        protected override plib.pistream stream(string name)
        {
            throw new emu_unimplemented();
        }
    }


    class logic_family_std_proxy_t : logic_family_desc_t
    {
        public logic_family_std_proxy_t() : base() { }


        public logic_family_std_proxy_t get() { return this; }  // for smart ptr


        public override devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_base_t anetlist, string name, logic_output_t proxied)
        {
            return new devices.nld_d_to_a_proxy(anetlist, name, proxied);  //return plib::owned_ptr<devices::nld_base_d_to_a_proxy>::Create<devices::nld_d_to_a_proxy>(anetlist, name, proxied);
        }
        public override devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_base_t anetlist, string name, logic_input_t proxied)
        {
            return new devices.nld_a_to_d_proxy(anetlist, name, proxied);  //return plib::owned_ptr<devices::nld_base_a_to_d_proxy>::Create<devices::nld_a_to_d_proxy>(anetlist, name, proxied);
        }
    }
}
