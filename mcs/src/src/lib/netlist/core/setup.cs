// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using abstract_t_connection_t = mame.std.pair<string, string>;  //using connection_t = std::pair<pstring, pstring>;
using log_type = mame.plib.plog_base<mame.netlist.nl_config_global.bool_const_NL_DEBUG>;  //using log_type =  plib::plog_base<NL_DEBUG>;
using models_t_map_t = mame.std.unordered_map<string, string>;  //using map_t = std::unordered_map<pstring, pstring>;
using models_t_raw_map_t = mame.std.unordered_map<string, string>;  //using raw_map_t = std::unordered_map<pstring, pstring>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using size_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.netlist.nl_config_global;
using static mame.netlist.nl_errstr_global;


namespace mame.netlist
{
    // ----------------------------------------------------------------------------------------
    // Collection of models
    // ----------------------------------------------------------------------------------------
    public class models_t
    {
        //using raw_map_t = std::unordered_map<pstring, pstring>;
        //using map_t = std::unordered_map<pstring, pstring>;


        public class model_t : param_value_interface
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
                if (entity != plib.pg.ucase(entity))
                    throw new nl_exception(MF_MODEL_PARAMETERS_NOT_UPPERCASE_1_2(entity, model_string(m_map)));
                var it = m_map.find(entity);
                if (it == default)
                    throw new nl_exception(MF_ENTITY_1_NOT_FOUND_IN_MODEL_2(entity, model_string(m_map)));

                return it;
            }


            public nl_fptype value(string entity)
            {
                string tmp = value_str(entity);

                nl_fptype factor = nlconst.one();
                var p = tmp[(int)tmp.length() - 1];  //auto p = std::next(tmp.begin(), plib::narrow_cast<pstring::difference_type>(tmp.length() - 1));
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
                            throw new nl_exception(MF_UNKNOWN_NUMBER_FACTOR_IN_2(m_model, entity));
                        break;
                }

                if (factor != nlconst.one())
                    tmp = plib.pg.left(tmp, tmp.length() - 1);
                // FIXME: check for errors
                bool err = false;
                var val = plib.pg.pstonum_ne_nl_fptype(false, tmp, out err);
                if (err)
                    throw new nl_exception(MF_MODEL_NUMBER_CONVERSION_ERROR(entity, tmp, "double", m_model));

                return val * factor;
            }


            public string type() { return value_str("COREMODEL"); }


            static string model_string(models_t_map_t map)
            {
                // operator [] has no const implementation
                string ret = map.at("COREMODEL") + "(";
                foreach (var i in map)
                    ret += i.first() + '=' + i.second() + ' ';

                return ret + ")";
            }
        }


        models_t_raw_map_t m_models;
        std.unordered_map<string, models_t_map_t> m_cache = new std.unordered_map<string, models_t_map_t>();


        public models_t(models_t_raw_map_t models)
        {
            m_models = models;
        }


        public model_t get_model(string model)
        {
            models_t_map_t map = m_cache[model];
            if (map == default)
            {
                map = new models_t_map_t();
                m_cache[model] = map;
            }

            if (map.empty())
                model_parse(model, map);

            return new model_t(model, map);
        }


        //std::vector<pstring> known_models() const


        void model_parse(string model_in, models_t_map_t map)
        {
            string model = model_in;
            size_t pos = 0;
            string key = "";

            while (true)
            {
                pos = model.find('(');
                if (pos != npos)
                    break;

                key = plib.pg.ucase(model);
                var i = m_models.find(key);
                if (i == default)
                    throw new nl_exception(MF_MODEL_NOT_FOUND("xx" + model));

                model = i;
            }

            string base_model = plib.pg.left(model, pos);

            if (base_model == "_")
            {
                map["COREMODEL"] = key;
            }
            else
            {
                var i = m_models.find(base_model);
                if (i != default)
                    model_parse(base_model, map);
                else
                    throw new nl_exception(MF_MODEL_NOT_FOUND(model_in));
            }

            string remainder = plib.pg.trim(model.substr(pos + 1));
            if (!plib.pg.endsWith(remainder, ")"))
                throw new nl_exception(MF_MODEL_ERROR_1(model));

            // FIMXE: Not optimal
            remainder = plib.pg.left(remainder, remainder.length() - 1);

            var pairs = plib.pg.psplit(remainder, ' ', true);
            foreach (string pe in pairs)
            {
                var pose = pe.find('=');
                if (pose == npos)
                    throw new nl_exception(MF_MODEL_ERROR_ON_PAIR_1(model));

                map[plib.pg.ucase(plib.pg.left(pe, pose))] = pe.substr(pose + 1);
            }
        }
    }


    namespace detail
    {
        public class alias_t
        {
            alias_type m_type;
            string m_alias;
            string m_references;


            public alias_t(alias_type type, string alias, string references)
            {
                m_type = type;
                m_alias = alias;
                m_references = references;
            }

            //alias_t(const alias_t &) = default;
            //alias_t &operator=(const alias_t &) = default;
            //alias_t(alias_t &&) noexcept = default;
            //alias_t &operator=(alias_t &&) noexcept = default;

            //pstring name() const { return m_alias; }
            public string references() { return m_references; }
            //alias_type type() const { return m_type; }
        }


        ///
        /// \brief class containing the abstract net list
        ///
        /// After parsing a net list this class contains all raw
        /// connections, parameter values and devices.
        public class abstract_t
        {
            //using connection_t = std::pair<pstring, pstring>;


            public std.unordered_map<string, alias_t> m_aliases = new std.unordered_map<string, alias_t>();
            public std.vector<abstract_t_connection_t> m_connections = new std.vector<abstract_t_connection_t>();
            public std.unordered_map<string, string> m_param_values = new std.unordered_map<string, string>();
            public models_t_raw_map_t m_models = new models_t_map_t();

            // need to preserve order of device creation ...
            public std.vector<std.pair<string, factory.element_t>> m_device_factory = new std.vector<std.pair<string, factory.element_t>>();
            // lifetime control only - can be cleared before run
            public std.vector<std.pair<string, string>> m_default_params = new std.vector<std.pair<string, string>>();
            public std.unordered_map<string, bool> m_hints = new std.unordered_map<string, bool>();
            public factory.list_t m_factory;


            public abstract_t(log_type log) { m_factory = new factory.list_t(log); }
        }
    } // namespace detail


    // -----------------------------------------------------------------------------
    // param_ref_t
    // -----------------------------------------------------------------------------
    public class param_ref_t
    {
        core_device_t m_device;
        param_t m_param;


        public param_ref_t() { m_device = null;  m_param = null; }

        public param_ref_t(core_device_t device, param_t param)
        {
            m_device = device;
            m_param = param;
        }


        //~param_ref_t() = default;
        //PCOPYASSIGNMOVE(param_ref_t, default)


        //const core_device_t &device() const noexcept { return *m_device; }


        public param_t param() { return m_param; }


        //bool is_valid() const noexcept { return (m_device != nullptr) && (m_param != nullptr); }
    }


    // ----------------------------------------------------------------------------------------
    // setup_t
    // ----------------------------------------------------------------------------------------
    public class setup_t
    {
        detail.abstract_t m_abstract;
        nlparse_t m_parser;
        netlist_state_t m_nlstate;

        models_t m_models;

        // FIXME: currently only used during setup
        devices.nld_netlistparams m_netlist_params;

        // FIXME: can be cleared before run
        std.unordered_map<string, detail.core_terminal_t> m_terminals = new std.unordered_map<string, detail.core_terminal_t>();
        // FIXME: Limited to 3 additional terminals
        std.unordered_map<terminal_t, std.array<terminal_t, u64_const_4>> m_connected_terminals = new std.unordered_map<terminal_t, std.array<terminal_t, u64_const_4>>();
        std.unordered_map<string, param_ref_t> m_params = new std.unordered_map<string, param_ref_t>();
        std.unordered_map<detail.core_terminal_t, devices.nld_base_proxy> m_proxies = new std.unordered_map<detail.core_terminal_t, devices.nld_base_proxy>();
        std.vector<param_t> m_defparam_lifetime;

        unsigned m_proxy_cnt;


        public setup_t(netlist_state_t nlstate)
        {
            m_abstract = new detail.abstract_t(nlstate.log());
            m_parser = new nlparse_t(nlstate.log(), m_abstract);
            m_nlstate = nlstate;
            m_models = new models_t(m_abstract.m_models); // FIXME : parse abstract_t only
            m_netlist_params = null;
            m_proxy_cnt = 0;
        }

        //~setup_t() noexcept = default;

        //PCOPYASSIGNMOVE(setup_t, delete)


        // called from param_t creation

        public void register_param_t(param_t param)
        {
            if (!m_params.insert(param.name(), new param_ref_t(param.device(), param)))
            {
                log().fatal.op(MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(param.name()));
                throw new nl_exception(MF_ADDING_PARAMETER_1_TO_PARAMETER_LIST(param.name()));
            }
        }


        public string get_initial_param_val(string name, string def)
        {
            // When `get_intial_param_val` is called the parameter `<name>` is already registered
            // and the value `(valstr())` is set to the default value, e.g. "74XX"
            // If thus `$(IC5E.A.MODEL)` is given for `name=="IC5E.A.MODEL"` `valstr()` below
            // will return the default.
            // FIXME: It may be more explicit and stable to test if pattern==name and return
            // `def` in this case.

            var i = m_abstract.m_param_values.find(name);
            var found_pat = false;
            string v = (i == default) ? def : i;

            do
            {
                found_pat = false;
                var sp = plib.pg.psplit(v, new std.vector<string>() {"$(", ")"});
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
                            v = v + k.param().value_string();
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
                log().fatal.op(MF_ADDING_1_2_TO_TERMINAL_LIST(termtype_as_str(term), term.name()));
                throw new nl_exception(MF_ADDING_1_2_TO_TERMINAL_LIST(termtype_as_str(term), term.name()));
            }
        }

        public void register_term(terminal_t term, terminal_t other_term, std.array<terminal_t, u64_const_2> splitter_terms)
        {
            this.register_term(term);
            m_connected_terminals.insert(term, new std.array<terminal_t, u64_const_4>(other_term, splitter_terms[0], splitter_terms[1], null));  //m_connected_terminals.insert({&term, {other_term, splitter_terms[0], splitter_terms[1], nullptr}});
        }


        // called from matrix_solver_t::get_connected_net
        // returns the terminal being part of a two terminal device.
        public terminal_t get_connected_terminal(terminal_t term)
        {
            var ret = m_connected_terminals.find(term);
            return (ret != default) ? ret[0] : null;
        }


        // called from net_splitter
        public std.array<terminal_t, u64_const_4> get_connected_terminals(terminal_t term)
        {
            var ret = m_connected_terminals.find(term);
            return (ret != default) ? ret : default;
        }


        // get family -> truth table
        public logic_family_desc_t family_from_model(string model)
        {
            family_type ft = family_type.CUSTOM;

            var mod = m_models.get_model(model);
            family_model_t modv = new family_model_t(mod);

            if (!plib.penum_base.set_from_string(modv.m_TYPE.op(), out ft))  //if (!ft.set_from_string(modv.m_TYPE()))
                throw new nl_exception(MF_UNKNOWN_FAMILY_TYPE_1(modv.m_TYPE.op(), model));

            var it = m_nlstate.family_cache().find(model);
            if (it != default)
                return it;

            var ret = new logic_family_std_proxy_t(ft);  //auto ret = plib::make_unique<logic_family_std_proxy_t, host_arena>(ft);

            ret.m_low_threshold_PCNT = modv.m_IVL.op();
            ret.m_high_threshold_PCNT = modv.m_IVH.op();
            ret.m_low_VO = modv.m_OVL.op();
            ret.m_high_VO = modv.m_OVH.op();
            ret.m_R_low = modv.m_ORL.op();
            ret.m_R_high = modv.m_ORH.op();

            switch (ft)
            {
                case family_type.CUSTOM:
                case family_type.TTL:
                case family_type.NMOS:
                    ret.m_vcc = "VCC";
                    ret.m_gnd = "GND";
                    break;
                case family_type.MOS:
                case family_type.CMOS:
                case family_type.PMOS:
                    ret.m_vcc = "VDD";
                    ret.m_gnd = "VSS";
                    break;
            }

            var retp = ret;

            m_nlstate.family_cache().emplace(model, ret);

            return retp;
        }


        public param_ref_t find_param(string param_in)
        {
            string outname = resolve_alias(param_in);
            var ret = m_params.find(outname);
            if (ret == default)
            {
                log().fatal.op(MF_PARAMETER_1_2_NOT_FOUND(param_in, outname));
                throw new nl_exception(MF_PARAMETER_1_2_NOT_FOUND(param_in, outname));
            }

            return ret;
        }


        // needed by nltool
        //std::vector<pstring> get_terminals_for_device_name(const pstring &devname) const;


        // needed by proxy device to check power terminals
        public detail.core_terminal_t find_terminal(string terminal_in, detail.terminal_type atype, bool required = true)
        {
            string tname = resolve_alias(terminal_in);
            var ret = m_terminals.find(tname);
            // look for default
            if (ret == default && atype == detail.terminal_type.OUTPUT)
            {
                // look for ".Q" std output
                ret = m_terminals.find(tname + ".Q");
            }

            if (ret == default && required)
            {
                log().fatal.op(MF_TERMINAL_1_2_NOT_FOUND(terminal_in, tname));
                throw new nl_exception(MF_TERMINAL_1_2_NOT_FOUND(terminal_in, tname));
            }

            detail.core_terminal_t term = (ret == default ? null : ret);

            if (term != null && term.type() != atype)
            {
                if (required)
                {
                    log().fatal.op(MF_OBJECT_1_2_WRONG_TYPE(terminal_in, tname));
                    throw new nl_exception(MF_OBJECT_1_2_WRONG_TYPE(terminal_in, tname));
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
            if (ret == default)
            {
                // look for ".Q" std output
                ret = m_terminals.find(tname + ".Q");
            }

            detail.core_terminal_t term = ret == default ? null : ret;

            if (term == null && required)
            {
                log().fatal.op(MF_TERMINAL_1_2_NOT_FOUND(terminal_in, tname));
                throw new nl_exception(MF_TERMINAL_1_2_NOT_FOUND(terminal_in, tname));
            }

            if (term != null)
            {
                log().debug.op("Found input {0}\n", tname);
            }

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
                foreach (var e in m_abstract.m_aliases)
                {
                    // FIXME: this will resolve first one found
                    if (e.second().references() == ret)
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
        bool connect(detail.core_terminal_t t1_in, detail.core_terminal_t t2_in)
        {
            //using namespace detail;

            log().debug.op("Connecting {0} to {1}\n", t1_in.name(), t2_in.name());
            detail.core_terminal_t t1 = resolve_proxy(t1_in);
            detail.core_terminal_t t2 = resolve_proxy(t2_in);
            detail.terminal_type t1_type = t1.type();
            detail.terminal_type t2_type = t2.type();
            bool ret = true;

            switch (t1_type)
            {
                case detail.terminal_type.TERMINAL:
                {
                    switch (t2_type)
                    {
                        case detail.terminal_type.TERMINAL:
                            connect_terminals(t1, t2);
                            break;
                        case detail.terminal_type.INPUT:
                            connect_terminal_input(t1, t2);
                            break;
                        case detail.terminal_type.OUTPUT:
                            connect_terminal_output(t1, t2);
                            break;
                    }
                    break;
                }
                case detail.terminal_type.INPUT:
                {
                    switch (t2_type)
                    {
                        case detail.terminal_type.TERMINAL:
                            connect_terminal_input(t2, t1);
                            break;
                        case detail.terminal_type.INPUT:
                            ret = connect_input_input(t1, t2);
                            break;
                        case detail.terminal_type.OUTPUT:
                            connect_input_output(t1, t2);
                            break;
                    }
                    break;
                }
                case detail.terminal_type.OUTPUT:
                {
                    switch (t2_type)
                    {
                        case detail.terminal_type.TERMINAL:
                            connect_terminal_output(t2, t1);
                            break;
                        case detail.terminal_type.INPUT:
                            connect_input_output(t2, t1);
                            break;
                        case detail.terminal_type.OUTPUT: ret = false; break;
                    }
                    break;
                }
            }

            return ret;
        }


        // run preparation

        public void prepare_to_run()
        {
            string envlog = plib.pg.environment("NL_LOGS", "");

            if (!envlog.empty())
            {
                var loglist = plib.pg.psplit(envlog, ':');
                m_parser.register_dynamic_log_devices(loglist);
            }

            // create defparams first!

            foreach (var e in m_abstract.m_default_params)
            {
                var param = new param_str_t(nlstate(), e.first, e.second);  //auto param(plib::make_unique<param_str_t, host_arena>(nlstate(), e.first, e.second));
                register_param_t(param);
                m_defparam_lifetime.push_back(param);
            }

            // make sure the solver and parameters are started first!

            foreach (var e in m_abstract.m_device_factory)
            {
                if ( m_parser.factory_().is_class<devices.nld_solver>(e.second)
                        || m_parser.factory_().is_class<devices.nld_netlistparams>(e.second))
                {
                    m_nlstate.register_device(e.first, e.second.make_device(nlstate().pool(), m_nlstate, e.first));
                }
            }

            log().debug.op("Searching for solver and parameters ...\n");

            var solver = m_nlstate.get_single_device<devices.nld_solver>("solver");
            m_netlist_params = m_nlstate.get_single_device<devices.nld_netlistparams>("parameter");

            // set default model parameters

            // FIXME: this is not optimal
            m_parser.register_model(new plib.pfmt("NMOS_DEFAULT _(CAPMOD={0})").op(m_netlist_params.m_mos_cap_model.op()));
            m_parser.register_model(new plib.pfmt("PMOS_DEFAULT _(CAPMOD={0})").op(m_netlist_params.m_mos_cap_model.op()));

            // create devices

            log().debug.op("Creating devices ...\n");
            foreach (var e in m_abstract.m_device_factory)
            {
                if ( !m_parser.factory_().is_class<devices.nld_solver>(e.second)
                        && !m_parser.factory_().is_class<devices.nld_netlistparams>(e.second))
                {
                    var dev = e.second.make_device(m_nlstate.pool(), m_nlstate, e.first);
                    m_nlstate.register_device(dev.name(), dev);
                }
            }

            int errcnt = 0;

            bool use_deactivate = m_netlist_params.m_use_deactivate.op();

            foreach (var d in m_nlstate.devices())
            {
                var p = m_abstract.m_hints.find(d.second.name() + sHINT_NO_DEACTIVATE);
                if (p != default)
                {
                    // suspect this is incorrect, need to mark the actual data in the map as 'used'
                    throw new emu_unimplemented();

                    p = true; // mark as used
                    d.second.set_hint_deactivate(false);
                }
                else
                {
                    d.second.set_hint_deactivate(use_deactivate);
                }
            }

            if (errcnt > 0)
            {
                log().fatal.op(MF_ERRORS_FOUND(errcnt));
                throw new nl_exception(MF_ERRORS_FOUND(errcnt));
            }

            // resolve inputs
            resolve_inputs();

            log().verbose.op("looking for two terms connected to rail nets ...");
            foreach (var t in m_nlstate.get_device_list<analog.nld_two_terminal>())
            {
                if (t.N().net().is_rail_net() && t.P().net().is_rail_net())
                {
                    log().info.op(MI_REMOVE_DEVICE_1_CONNECTED_ONLY_TO_RAILS_2_3(
                        t.name(), t.N().net().name(), t.P().net().name()));

                    // The following would remove internal devices in e.g. MOSFETs as well.
#if false
                    remove_terminal(t.setup_N().net(), t.setup_N());
                    remove_terminal(t.setup_P().net(), t.setup_P());
                    m_nlstate.remove_device(t);
#endif
                }
            }

            log().verbose.op("looking for unused hints ...");
            foreach (var h in m_abstract.m_hints)
            {
                if (!h.second())
                {
                    log().fatal.op(MF_UNUSED_HINT_1(h.first()));
                    throw new nl_exception(MF_UNUSED_HINT_1(h.first()));
                }
            }

            log().verbose.op("initialize solver ...\n");

            if (solver == null)
            {
                foreach (var p in m_nlstate.nets())
                {
                    if (p.is_analog())
                    {
                        log().fatal.op(MF_NO_SOLVER());
                        throw new nl_exception(MF_NO_SOLVER());
                    }
                }
            }
            else
            {
                solver.post_start();
            }

            errcnt = 0;
            log().debug.op("Looking for unknown parameters ...\n");
            foreach (var p in m_abstract.m_param_values)
            {
                var f = m_params.find(p.first());
                if (f == default)
                {
                    log().error.op(ME_UNKNOWN_PARAMETER(p.first()));
                    errcnt++;
                }
            }

            if (errcnt > 0)
            {
                log().fatal.op(MF_ERRORS_FOUND(errcnt));
                throw new nl_exception(MF_ERRORS_FOUND(errcnt));
            }

            foreach (var n in m_nlstate.nets())
            {
                foreach (var term in m_nlstate.core_terms(n))
                {
                    if (term.delegate_() == null)  //if (!term->delegate())
                    {
                        log().fatal.op(MF_DELEGATE_NOT_SET_1(term.name()));
                        throw new nl_exception(MF_DELEGATE_NOT_SET_1(term.name()));
                    }
                }

                n.rebuild_list();
            }
        }


        public models_t models() { return m_models; }


        public netlist_state_t nlstate() { return m_nlstate; }


        public nlparse_t parser() { return m_parser; }


        log_type log() { return m_nlstate.log(); }


        void resolve_inputs()
        {
            log().verbose.op("Resolving inputs ...");

            // Netlist can directly connect input to input.
            // We therefore first park connecting inputs and retry
            // after all other terminals were connected.

            unsigned tries = m_netlist_params.m_max_link_loops.op();

#if false
            // This code fails for some netlists when the element at position 0
            // is deleted. It will fail somewhere deep in std::pair releasing
            // std::string called from erase.
            //
#else
            while (!m_abstract.m_connections.empty() && tries > 0)
            {
                for (size_t i = 0; i < m_abstract.m_connections.size(); )
                {
                    string t1s = m_abstract.m_connections[i].first;
                    string t2s = m_abstract.m_connections[i].second;
                    detail.core_terminal_t t1 = find_terminal(t1s);
                    detail.core_terminal_t t2 = find_terminal(t2s);
                    if (connect(t1, t2))
                        m_abstract.m_connections.erase((int)i);  //m_abstract.m_links.erase(m_abstract.m_links.begin() + plib::narrow_cast<std::ptrdiff_t>(i));
                    else
                        i++;
                }

                tries--;
            }
#endif

            if (tries == 0)
            {
                foreach (var link in m_abstract.m_connections)
                    log().warning.op(MF_CONNECTING_1_TO_2(de_alias(link.first), de_alias(link.second)));

                log().fatal.op(MF_LINK_TRIES_EXCEEDED(m_netlist_params.m_max_link_loops.op()));
                throw new nl_exception(MF_LINK_TRIES_EXCEEDED(m_netlist_params.m_max_link_loops.op()));
            }

            log().verbose.op("deleting empty nets ...");

            // delete empty nets

            delete_empty_nets();

            bool err = false;

            log().verbose.op("looking for terminals not connected ...");
            foreach (var i in m_terminals)
            {
                detail.core_terminal_t term = i.second();
                string name_da = de_alias(term.name());
                bool is_nc_pin = term.device() is devices.nld_nc_pin;  //bool is_nc_pin(dynamic_cast< devices::NETLIB_NAME(nc_pin) *>(&term->device()) != nullptr);
                bool is_nc_flagged = false;

                var hnc = m_abstract.m_hints.find(name_da + sHINT_NC);
                if (hnc != default)
                {
                    // I suspect this is incorrect, we want to mark the actual data in the map as 'used'
                    throw new emu_unimplemented();

                    hnc = true; // mark as used
                    is_nc_flagged = true;
                }

                if (term.has_net() && is_nc_pin)
                {
                    log().error.op(ME_NC_PIN_1_WITH_CONNECTIONS(name_da));
                    err = true;
                }
                else if (is_nc_pin)
                {
                    /* ignore */
                }
                else if (!term.has_net())
                {
                    log().error.op(ME_TERMINAL_1_WITHOUT_NET(name_da));
                    err = true;
                }
                else if (nlstate().core_terms(term.net()).empty())
                {
                    if (term.is_logic_input())
                    {
                        log().warning.op(MW_LOGIC_INPUT_1_WITHOUT_CONNECTIONS(name_da));
                    }
                    else if (term.is_logic_output())
                    {
                        if (!is_nc_flagged)
                            log().info.op(MI_LOGIC_OUTPUT_1_WITHOUT_CONNECTIONS(name_da));
                    }
                    else if (term.is_analog_output())
                    {
                        if (!is_nc_flagged)
                            log().info.op(MI_ANALOG_OUTPUT_1_WITHOUT_CONNECTIONS(name_da));
                    }
                    else
                    {
                        log().warning.op(MW_TERMINAL_1_WITHOUT_CONNECTIONS(name_da));
                    }
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
                        log().error.op(ME_TRISTATE_NO_PROXY_FOUND_2(term.name(), term.device().name()));
                        err = true;
                    }
                    else if (iter_proxy != default && tri.is_force_logic())
                    {
                        log().error.op(ME_TRISTATE_PROXY_FOUND_2(term.name(), term.device().name()));
                        err = true;
                    }
                }
            }

            if (err)
            {
                log().fatal.op(MF_TERMINALS_WITHOUT_NET());
                throw new nl_exception(MF_TERMINALS_WITHOUT_NET());
            }
        }


        string resolve_alias(string name)
        {
            string temp = name;
            string ret;

            // FIXME: Detect endless loop
            do
            {
                ret = temp;
                var p = m_abstract.m_aliases.find(ret);
                temp = (p != default ? p.references() : "");
            } while (!temp.empty() && temp != ret);

            log().debug.op("{0}==>{1}\n", name, ret);
            return ret;
        }


        void merge_nets(detail.net_t this_net, detail.net_t other_net)
        {
            log().debug.op("merging nets ...\n");
            if (other_net == this_net)
            {
                log().warning.op(MW_CONNECTING_1_TO_ITSELF(this_net.name()));
                return; // Nothing to do
            }

            if (this_net.is_rail_net() && other_net.is_rail_net())
            {
                log().fatal.op(MF_MERGE_RAIL_NETS_1_AND_2(this_net.name(), other_net.name()));
                throw new nl_exception(MF_MERGE_RAIL_NETS_1_AND_2(this_net.name(), other_net.name()));
            }

            if (other_net.is_rail_net())
            {
                log().debug.op("other net is a rail net\n");
                merge_nets(other_net, this_net);
            }
            else
            {
                move_connections(other_net, this_net);
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
                var new_net_uptr = new analog_net_t(m_nlstate, "net." + t1.name());  //auto new_net_uptr = plib::make_owned<analog_net_t>(nlstate().pool(), m_nlstate,"net." + t1.name());
                var new_net_ptr = new_net_uptr;
                m_nlstate.register_net(new_net_uptr);
                t1.set_net(new_net_uptr);
                new_net_uptr.add_terminal(t2);
                new_net_uptr.add_terminal(t1);
            }
        }


        void connect_input_output(detail.core_terminal_t input, detail.core_terminal_t output)
        {
            if (input.has_net() && input.net().is_rail_net())
            {
                log().fatal.op(MF_INPUT_1_ALREADY_CONNECTED(input.name()));
                throw new nl_exception(MF_INPUT_1_ALREADY_CONNECTED(input.name()));
            }
            if (output.is_analog() && input.is_logic())
            {
                var proxy = get_a_d_proxy(input);

                output.net().add_terminal(proxy.proxy_term());
            }
            else if (output.is_logic() && input.is_analog())
            {
                devices.nld_base_proxy proxy = get_d_a_proxy(output);

                connect_terminals(proxy.proxy_term(), input);
            }
            else
            {
                if (input.has_net())
                    merge_nets(output.net(), input.net());
                else if (output.has_net())
                    output.net().add_terminal(input);
                else
                {
                    log().fatal.op(ME_TERMINALS_1_2_WITHOUT_NET(input.name(), output.name()));
                    throw new nl_exception(ME_TERMINALS_1_2_WITHOUT_NET(input.name(), output.name()));
                }
            }
        }


        void connect_terminal_output(detail.core_terminal_t terminal, detail.core_terminal_t output)
        {
            if (output.is_analog())
            {
                log().debug.op("connect_terminal_output: {0} {1}\n", terminal.name(), output.name());
                // no proxy needed, just merge existing terminal net
                if (terminal.has_net())
                {
                    if (output.net() != terminal.net())
                    {
                        merge_nets(output.net(), terminal.net());
                    }
                    else
                    {
                        // Only an info - some ICs (CD4538) connect pins internally to GND
                        // and the schematics again externally. This will cause this warning.
                        // FIXME: Add a hint to suppress the warning.
                        log().info.op(MI_CONNECTING_1_TO_2_SAME_NET(terminal.name(), output.name(), terminal.net().name()));
                    }
                }
                else
                {
                    output.net().add_terminal(terminal);
                }
            }
            else if (output.is_logic())
            {
                log().debug.op("connect_terminal_output: connecting proxy\n");
                devices.nld_base_proxy proxy = get_d_a_proxy(output);

                connect_terminals(proxy.proxy_term(), terminal);
            }
            else
            {
                log().fatal.op(MF_OBJECT_OUTPUT_TYPE_1(output.name()));
                throw new nl_exception(MF_OBJECT_OUTPUT_TYPE_1(output.name()));
            }
        }


        void connect_terminal_input(detail.core_terminal_t terminal, detail.core_terminal_t input)
        {
            if (input.is_analog())
            {
                connect_terminals(input, terminal);
            }
            else if (input.is_logic())
            {
                log().verbose.op("connect terminal {0} (in, {1}) to {2}\n", input.name(), input.is_analog() ? "analog" : input.is_logic() ? "logic" : "?", terminal.name());
                var proxy = get_a_d_proxy(input);

                //out.net().register_con(proxy->proxy_term());
                connect_terminals(terminal, proxy.proxy_term());

            }
            else
            {
                log().fatal.op(MF_OBJECT_INPUT_TYPE_1(input.name()));
                throw new nl_exception(MF_OBJECT_INPUT_TYPE_1(input.name()));
            }
        }


        bool connect_input_input(detail.core_terminal_t input1, detail.core_terminal_t input2)
        {
            bool ret = false;
            if (input1.has_net()) // if input 1 already has a net
            {
                if (input1.net().is_rail_net()) // and the net is a rail net
                    ret = connect(input2, input1.net().rail_terminal()); // try to
                                                                         // connect
                                                                         // input 2
                                                                         // to rail
                                                                         // terminal
                                                                         // of input
                                                                         // 1 net
                if (!ret)
                {
                    // the above was not successfull - try to connect input2 to
                    // TERMINAL type terminals of input 1 net's terminals
                    foreach (detail.core_terminal_t t in input1.net().core_terms_copy())
                    {
                        if (t.is_type(detail.terminal_type.TERMINAL))
                            ret = connect(input2, t);
                        if (ret)
                            break;
                    }
                }
            }
            // FIXME: We could use a helper connect_input_input_helper(input2,
            // input1) here.
            if (!ret && input2.has_net())
            {
                if (input2.net().is_rail_net())
                    ret = connect(input1, input2.net().rail_terminal());
                if (!ret)
                {
                    foreach (detail.core_terminal_t t in input2.net().core_terms_copy())
                    {
                        if (t.is_type(detail.terminal_type.TERMINAL))
                            ret = connect(input1, t);
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
            var iter_proxy = m_proxies.find(out_);

            if (iter_proxy != default)
                return iter_proxy;

            // create a new one ...

            var out_cast = (logic_output_t)out_;
            nl_assert_always(out_cast != null, "Not able to cast to logic_output_t&");

            string x = new plib.pfmt("proxy_da_{0}_{1}").op(out_.name(), m_proxy_cnt);
            var new_proxy = out_cast.logic_family().create_d_a_proxy(m_nlstate, x, out_cast);
            m_proxy_cnt++;
            // connect all existing terminals to new net

            // Get a copy first
            var temp_terminals = out_.net().core_terms_copy();
            // remove all terminals from out.net()
            out_.net().remove_all_terminals();

            foreach (detail.core_terminal_t p in temp_terminals)
            {
                p.clear_net(); // de-link from all nets ...
                if (!connect(new_proxy.proxy_term(), p))
                {
                    log().fatal.op(MF_CONNECTING_1_TO_2(new_proxy.proxy_term().name(), p.name()));
                    throw new nl_exception(MF_CONNECTING_1_TO_2(new_proxy.proxy_term().name(), p.name()));
                }
            }

            out_.net().add_terminal(new_proxy.in_());

            var proxy = new_proxy;
            if (!m_proxies.insert(out_, proxy))
                throw new nl_exception(MF_DUPLICATE_PROXY_1(out_.name()));

            m_nlstate.register_device(new_proxy.name(), new_proxy);
            return proxy;
        }


        devices.nld_base_proxy get_a_d_proxy(detail.core_terminal_t inp)
        {
            //throw new emu_unimplemented();
#if false
            gsl_Expects(inp.is_logic());
#endif

            var logic_input_terminal = (logic_input_t)inp;

            var iter_proxy = m_proxies.find(inp);

            if (iter_proxy != default)
                return iter_proxy;

            log().debug.op("connect_terminal_input: connecting proxy\n");
            var new_proxy = logic_input_terminal.logic_family().create_a_d_proxy(m_nlstate, new plib.pfmt("proxy_ad_{0}_{1}").op(inp.name(), m_proxy_cnt), logic_input_terminal);

            var ret = new_proxy;

            if (!m_proxies.insert(inp, ret))
                throw new nl_exception(MF_DUPLICATE_PROXY_1(inp.name()));

            m_proxy_cnt++;

            // connect all existing terminals to new net

            if (inp.has_net())
            {
                var temp_terminals = inp.net().core_terms_copy();

                inp.net().remove_all_terminals();

                foreach (detail.core_terminal_t p in temp_terminals)
                {
                    // inp may already belongs to the logic net. Thus skip it here.
                    // It will be removed by the clear further down.
                    if (p != inp)
                    {
                        p.clear_net(); // de-link from all nets ...
                        if (!connect(ret.proxy_term(), p))
                        {
                            log().fatal.op(MF_CONNECTING_1_TO_2(ret.proxy_term().name(), p.name()));
                            throw new nl_exception(MF_CONNECTING_1_TO_2(ret.proxy_term().name(), p.name()));
                        }
                    }
                }
            }

            inp.clear_net();
            ret.out_().net().add_terminal(inp);
            m_nlstate.register_device(new_proxy.name(), new_proxy);
            return ret;
        }


        detail.core_terminal_t resolve_proxy(detail.core_terminal_t term)
        {
            if (term.is_logic())
            {
                var out_ = (logic_t)term;
                var iter_proxy = m_proxies.find(out_);
                if (iter_proxy != default)
                    return iter_proxy.proxy_term();
            }

            return term;
        }


        // net manipulations

        //void remove_terminal(detail.net_t net, detail.core_terminal_t terminal)


        void move_connections(detail.net_t net, detail.net_t dest_net)
        {
            var temp = net.core_terms_copy();

            net.remove_all_terminals();

            foreach (detail.core_terminal_t ct in temp)
            {
                dest_net.add_terminal(ct);
            }
        }


        // ----------------------------------------------------------------------------------------
        // Device handling
        // ----------------------------------------------------------------------------------------

        void delete_empty_nets()
        {
            //m_nlstate.nets().erase(
            //    std::remove_if(m_nlstate.nets().begin(), m_nlstate.nets().end(),
            //        [](device_arena::owned_ptr<detail::net_t> &net)
            //        {
            //            if (net->state().core_terms(*net).empty())
            //            {
            //                // FIXME: need to remove from state->m_core_terms as well.
            //                net->state().log().verbose("Deleting net {1} ...", net->name());
            //                net->state().run_state_manager().remove_save_items(net.get());
            //                return true;
            //            }
            //            return false;
            //        }), m_nlstate.nets().end());
            for (int i = 0; i < m_nlstate.nets().Count; i++)
            {
                detail.net_t net = m_nlstate.nets()[i];
                if (net.state().core_terms(net).empty())
                {
                    // FIXME: need to remove from state->m_core_terms as well.
                    net.state().log().verbose.op("Deleting net {0} ...", net.name());
                    net.state().run_state_manager().remove_save_items(net);

                    m_nlstate.nets().erase(i);
                }
            }
        }
    }


    // ----------------------------------------------------------------------------------------
    // Specific netlist `psource_t` implementations
    // ----------------------------------------------------------------------------------------

    public abstract class source_netlist_t : plib.psource_t
    {
        //source_netlist_t() = default;

        //PCOPYASSIGNMOVE(source_netlist_t, delete)
        //~source_netlist_t() noexcept override = default;


        public virtual bool parse(nlparse_t setup, string name)
        {
            var strm = stream(name);
            return strm != null ? setup.parse_stream(strm, name) : false;  //return (!strm.empty()) ? setup.parse_stream(std::move(strm), name) : false;
        }
    }


    public abstract class source_data_t : plib.psource_t
    {
        //source_data_t() = default;

        //PCOPYASSIGNMOVE(source_data_t, delete)
        //~source_data_t() noexcept override = default;
    }


    //class source_string_t : public source_netlist_t

    //class source_file_t : public source_netlist_t

    //class source_pattern_t : public source_netlist_t

    //class source_mem_t : public source_netlist_t


    public class source_proc_t : source_netlist_t
    {
        nlsetup_func m_setup_func;
        string m_setup_func_name;


        public source_proc_t(string name, nlsetup_func setup_func)
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


        public override plib.istream_uptr stream(string name)  //plib::istream_uptr stream(const pstring &name) override;
        {
            return new plib.istream_uptr();
        }
    }


    //class source_token_t : public source_netlist_t

} // namespace netlist
