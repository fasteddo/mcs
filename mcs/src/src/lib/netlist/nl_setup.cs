// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    public class setup_t // : plib::nocopyassignmove
    {
        // ----------------------------------------------------------------------------------------
        // A Generic netlist sources implementation
        // ----------------------------------------------------------------------------------------

        class source_t
        {
#if false
            typedef plist_t<source_t *> list_t;

            source_t()
            {}

            virtual ~source_t() { }

            virtual bool parse(setup_t &setup, const pstring &name) = 0;
#endif
        }

        struct link_t
        {
#if false
            link_t() { }
            link_t(void *) { }
            // Copy constructor
            link_t(const link_t &from)
            {
                e1 = from.e1;
                e2 = from.e2;
            }

            link_t(const pstring &ae1, const pstring &ae2)
            {
                e1 = ae1;
                e2 = ae2;
            }
            pstring e1;
            pstring e2;

            bool operator==(const link_t &rhs) const { return (e1 == rhs.e1) && (e2 == rhs.e2); }
            link_t &operator=(const link_t &rhs) { e1 = rhs.e1; e2 = rhs.e2; return *this; }

            const pstring &name() const { return e1; }
#endif
        }


        //netlist_t *m_netlist;

        //phashmap_t<pstring, pstring> m_alias;
        //phashmap_t<pstring, param_t *>  m_params;
        //phashmap_t<pstring, pstring> m_params_temp;
        //phashmap_t<pstring, core_terminal_t *> m_terminals;

        //plist_t<link_t> m_links;

        //factory_list_t *m_factory;

        //phashmap_t<pstring, pstring> m_models;

        //int m_proxy_cnt;

        //pstack_t<pstring> m_stack;
        //source_t::list_t m_sources;
        //plist_t<pstring> m_lib;


        setup_t(netlist_t netlist)
        {
            throw new emu_unimplemented();
        }


        ~setup_t()
        {
            throw new emu_unimplemented();
        }


        //void init();


        //netlist_t &netlist() { return *m_netlist; }
        //const netlist_t &netlist() const { return *m_netlist; }


        //pstring build_fqn(const pstring &obj_name) const;


        //device_t *register_dev(device_t *dev, const pstring &name);
        //device_t *register_dev(const pstring &classname, const pstring &name);
        //void remove_dev(const pstring &name);


        //void register_lib_entry(const pstring &name);


        //void register_model(const pstring &model_in);
        //void register_alias(const pstring &alias, const pstring &out);
        //void register_dippins_arr(const pstring &terms);


        //void register_alias_nofqn(const pstring &alias, const pstring &out);


        //void register_link_arr(const pstring &terms);
        //void register_link_fqn(const pstring &sin, const pstring &sout);
        //void register_link(const pstring &sin, const pstring &sout);


        //void register_param(const pstring &param, const pstring &value);
        //void register_param(const pstring &param, const double value);


        //void register_frontier(const pstring attach, const double r_IN, const double r_OUT);
        //void remove_connections(const pstring attach);


        //void register_object(device_t &dev, const pstring &name, object_t &obj);
        //bool connect(core_terminal_t &t1, core_terminal_t &t2);


        //core_terminal_t *find_terminal(const pstring &outname_in, bool required = true);
        //core_terminal_t *find_terminal(const pstring &outname_in, object_t::type_t atype, bool required = true);


        //param_t *find_param(const pstring &param_in, bool required = true);


        //void start_devices();
        //void resolve_inputs();


        /* handle namespace */

        //void namespace_push(const pstring &aname);
        //void namespace_pop();


        /* parse a source */

        //void include(const pstring &netlist_name);


        /* register a source */

        //void register_source(source_t *src) { m_sources.add(src); }


        //factory_list_t &factory() { return *m_factory; }
        //const factory_list_t &factory() const { return *m_factory; }


        //bool is_library_item(const pstring &name) const { return m_lib.contains(name); }


        //void print_stats() const;


        /* model / family related */

        //logic_family_desc_t *family_from_model(const pstring &model);
        //const pstring model_value_str(model_map_t &map, const pstring &entity);
        //nl_double model_value(model_map_t &map, const pstring &entity);


        //void model_parse(const pstring &model, model_map_t &map);


        //plog_base<NL_DEBUG> &log() { return netlist().log(); }
        //const plog_base<NL_DEBUG> &log() const { return netlist().log(); }


        //void connect_terminals(core_terminal_t &in, core_terminal_t &out);
        //void connect_input_output(core_terminal_t &in, core_terminal_t &out);
        //void connect_terminal_output(terminal_t &in, core_terminal_t &out);
        //void connect_terminal_input(terminal_t &term, core_terminal_t &inp);
        //bool connect_input_input(core_terminal_t &t1, core_terminal_t &t2);


        // helpers
        //pstring objtype_as_astr(object_t &in) const;


        //const pstring resolve_alias(const pstring &name) const;
        //devices::nld_base_proxy *get_d_a_proxy(core_terminal_t &out);
    }
}
