// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using log_type = mame.plib.plog_base<mame.netlist.nl_config_global.bool_const_NL_DEBUG>;  //using log_type =  plib::plog_base<NL_DEBUG>;
using netlist_state_t_devices_collection_type = mame.std.vector<mame.std.pair<string, mame.netlist.core_device_t>>;  //using devices_collection_type = std::vector<std::pair<pstring, poolptr<core_device_t>>>;
using netlist_state_t_family_collection_type = mame.std.unordered_map<string, mame.netlist.logic_family_desc_t>;  //using family_collection_type = std::unordered_map<pstring, host_arena::unique_ptr<logic_family_desc_t>>;
using netlist_state_t_nets_collection_type = mame.std.vector<mame.netlist.detail.net_t>;  //using nets_collection_type = std::vector<poolptr<detail::net_t>>;
using size_t = System.UInt64;

using static mame.netlist.nl_config_global;
using static mame.netlist.nl_errstr_global;
using static mame.nlm_base_global;


namespace mame.netlist
{
    // -----------------------------------------------------------------------------
    // netlist_state__t
    // -----------------------------------------------------------------------------
    public class netlist_state_t
    {
        //using nets_collection_type = std::vector<device_arena::owned_ptr<detail::net_t>>;
        //using family_collection_type = std::unordered_map<pstring, host_arena::unique_ptr<logic_family_desc_t>>;

        // need to preserve order of device creation ...
        //using devices_collection_type = std::vector<std::pair<pstring, device_arena::owned_ptr<core_device_t>>>;


        device_arena m_pool; // must be deleted last!

        netlist_t m_netlist;  //device_arena::unique_ptr<netlist_t>        m_netlist;
        plib.dynlib_base m_lib;  //std::unique_ptr<plib::dynlib_base>         m_lib;
        plib.state_manager_t m_state = new plib.state_manager_t();
        log_type m_log;

        // FIXME: should only be available during device construction
        setup_t m_setup;  //host_arena::unique_ptr<setup_t>            m_setup;

        netlist_state_t_nets_collection_type m_nets = new netlist_state_t_nets_collection_type();
        // sole use is to manage lifetime of net objects
        netlist_state_t_devices_collection_type m_devices = new netlist_state_t_devices_collection_type();
        // sole use is to manage lifetime of family objects
        netlist_state_t_family_collection_type m_family_cache = new netlist_state_t_family_collection_type();
        // all terms for a net
        std.unordered_map<detail.net_t, std.vector<detail.core_terminal_t>> m_core_terms = new std.unordered_map<detail.net_t, std.vector<detail.core_terminal_t>>();  //std::unordered_map<const detail::net_t *, std::vector<detail::core_terminal_t *>> m_core_terms;

        // dummy version
        int m_dummy_version;


        public netlist_state_t()  //netlist_state_t(const pstring &name, plib::plog_delegate logger);
        {
            //see netlist_state_t_after_ctor()
        }

        public void netlist_state_t_after_ctor(string name, plib.plog_delegate logger)
        {
            m_log = new log_type(logger);
            m_dummy_version = 1;

            m_setup = new setup_t(this);  //m_setup = plib::make_unique<setup_t, host_arena>(*this);
            // create the run interface
            m_netlist = new netlist_t(this, name);  //m_netlist = plib::make_unique<netlist_t>(m_pool, *this, name);

            // Make sure save states are invalidated when a new version is deployed

            m_state.save_item(this, m_dummy_version, "V" + version());

            // Initialize factory
            netlist.devices.net_lib_global.initialize_factory(m_setup.parser().factory_());

            // Add default include file
            string content =
            "#define RES_R(res) (res)            \n" +
            "#define RES_K(res) ((res) * 1e3)    \n" +
            "#define RES_M(res) ((res) * 1e6)    \n" +
            "#define CAP_U(cap) ((cap) * 1e-6)   \n" +
            "#define CAP_N(cap) ((cap) * 1e-9)   \n" +
            "#define CAP_P(cap) ((cap) * 1e-12)  \n" +
            "#define IND_U(ind) ((ind) * 1e-6)   \n" +
            "#define IND_N(ind) ((ind) * 1e-9)   \n" +
            "#define IND_P(ind) ((ind) * 1e-12)  \n";

            m_setup.parser().add_include(new plib.psource_str_t("netlist/devices/net_lib.h", content));  //m_setup->parser().add_include<plib::psource_str_t>("netlist/devices/net_lib.h", content);

            // This is for core macro libraries
            m_setup.parser().add_include(new plib.psource_str_t("devices/net_lib.h", content));  //m_setup->parser().add_include<plib::psource_str_t>("devices/net_lib.h", content);

            netlist_base_lib(m_setup.parser());  //NETLIST_NAME(base_lib)(m_setup->parser());

            //throw new emu_unimplemented();
#if false
            m_setup->parser().register_source<source_pattern_t>("src/lib/netlist/macro/nlm_{1}.cpp", true);
            m_setup->parser().register_source<source_pattern_t>("src/lib/netlist/generated/nlm_{1}.cpp", true);
            m_setup->parser().register_source<source_pattern_t>("src/lib/netlist/macro/modules/nlmod_{1}.cpp", true);
            m_setup->parser().include("base_lib");
#endif
        }


        //PCOPYASSIGNMOVE(netlist_state_t, delete)

        /// \brief Destructor
        ///
        /// The destructor is virtual to allow implementation specific devices
        /// to connect to the outside world. For examples see MAME netlist.cpp.
        ///
        //virtual ~netlist_state_t() noexcept = default;


        //template<class C>
        static bool check_class<C>(core_device_t p) where C : core_device_t
        {
            return p is C;  //return dynamic_cast<C *>(p) != nullptr;
        }


        delegate bool get_single_device_func(core_device_t device);

        core_device_t get_single_device(string classname, get_single_device_func cc)  //core_device_t *get_single_device(const pstring &classname, bool (*cc)(core_device_t *)) const;
        {
            core_device_t ret = null;
            foreach (var d in m_devices)
            {
                if (cc(d.second))
                {
                    if (ret != null)
                    {
                        m_log.fatal.op(MF_MORE_THAN_ONE_1_DEVICE_FOUND(classname));
                        throw new nl_exception(MF_MORE_THAN_ONE_1_DEVICE_FOUND(classname));
                    }
        
                    ret = d.second;
                }
            }
        
            return ret;
        }


        /// \brief Get single device filtered by class and name
        ///
        /// \tparam C Device class for which devices will be returned
        /// \param  name Name of the device
        ///
        /// \return pointers to device
        //template<class C>
        public C get_single_device<C>(string name) where C : core_device_t
        {
            return (C)get_single_device(name, check_class<C>);  //return dynamic_cast<C *>(get_single_device(name, check_class<C>));
        }


        /// \brief Get vector of devices
        ///
        /// \tparam C Device class for which devices will be returned
        ///
        /// \return vector with pointers to devices

        //template<class C>
        public std.vector<C> get_device_list<C>() where C : core_device_t
        {
            std.vector<C> tmp = new std.vector<C>();
            foreach (var d in m_devices)
            {
                var dev = d.second is C ? (C)d.second : null;  //auto dev = dynamic_cast<C *>(d.second.get());
                if (dev != null)
                    tmp.push_back(dev);
            }
            return tmp;
        }


        // logging

        public log_type log() { return m_log; }


        public plib.dynlib_base static_solver_lib() { return m_lib; }


        /// \brief provide library with static solver implementations.
        ///
        /// By default no static solvers are provided since these are
        /// determined by the specific use case. You can pass such a collection
        /// of symbols with this method.
        ///
        public void set_static_solver_lib(plib.dynlib_base lib)  //void set_static_solver_lib(std::unique_ptr<plib::dynlib_base> &&lib);
        {
            m_lib = lib;
        }


        public netlist_t exec() { return m_netlist; }


        // state handling
        public plib.state_manager_t run_state_manager() { return m_state; }


        //template<typename O, typename C>
        public void save(object owner, object state, string module, string stname)  //void save(O &owner, C &state, const pstring &module, const pstring &stname)
        {
            this.run_state_manager().save_item(owner, state, module + "." + stname);  //this->run_state_manager().save_item(static_cast<void *>(&owner), state, module + pstring(".") + stname);
        }
        //template<typename O, typename C>
        public void save(object owner, object state, string module, string stname, size_t count)  //void save(O &owner, C *state, const pstring &module, const pstring &stname, const std::size_t count)
        {
            this.run_state_manager().save_state_ptr(owner, module + "." + stname, null, count, state);  //this->run_state_manager().save_state_ptr(static_cast<void *>(&owner), module + pstring(".") + stname, plib::state_manager_t::dtype<C>(), count, state);
        }


        // FIXME: only used by queue_t save state

        public size_t find_net_id(detail.net_t net)
        {
            for (size_t i = 0; i < m_nets.size(); i++)
                if (m_nets[i] == net)
                    return i;

            return size_t.MaxValue;  //return std::numeric_limits<std::size_t>::max();
        }


        public detail.net_t net_by_id(size_t id)
        {
            return m_nets[id];
        }


        //template <typename T>
        public void register_net(detail.net_t net) { m_nets.push_back(net); }  //void register_net(device_arena::owned_ptr<T> &&net) { m_nets.push_back(std::move(net)); }


        /// \brief Get device pointer by name
        ///
        ///
        /// \param name Name of the device
        ///
        /// \return core_device_t pointer if device exists, else nullptr

        //core_device_t *find_device(const pstring &name) const
        //{
        //    for (const auto & d : m_devices)
        //        if (d.first == name)
        //            return d.second.get();
        //    return nullptr;
        //}

        /// \brief Register device using owned_ptr
        ///
        /// Used to register owned devices. These are devices declared as objects
        /// in another devices.
        ///
        /// \param name Name of the device
        /// \param dev Device to be registered
        //template <typename T>
        public void register_device(string name, core_device_t dev)  //void register_device(const pstring &name, device_arena::owned_ptr<T> &&dev) noexcept(false)
        {
            foreach (var d in m_devices)
            {
                if (d.first == name)
                {
                    //dev.release();
                    log().fatal.op(MF_DUPLICATE_NAME_DEVICE_LIST(name));
                    throw new nl_exception(MF_DUPLICATE_NAME_DEVICE_LIST(name));
                }
            }

            //m_devices.push_back(dev);
            m_devices.Add(new std.pair<string, core_device_t>(name, dev));  //m_devices.insert(m_devices.end(), { name, std::move(dev) });
        }

        /// \brief Register device using unique_ptr
        ///
        /// Used to register devices.
        ///
        /// \param name Name of the device
        /// \param dev Device to be registered

        //template <typename T>
        //void register_device(const pstring &name, device_arena::unique_ptr<T> &&dev)
        //{
        //    register_device(name, device_arena::owned_ptr<T>(dev.release(), true, dev.get_deleter()));
        //}

        /// \brief Remove device
        ///
        /// Care needs to be applied if this is called to remove devices with
        /// sub-devices which may have registered state.
        ///
        /// \param dev Device to be removed

        //void remove_device(core_device_t *dev);


        public setup_t setup() { return m_setup; }


        public nlparse_t parser() { return m_setup.parser(); }


        // FIXME: make a postload member and include code there
        public void rebuild_lists()  // must be called after post_load !
        {
            foreach (var net in m_nets)
                net.rebuild_list();
        }


        //static void compile_defines(std::vector<std::pair<pstring, pstring>> &defs);


        static string version()
        {
            return new plib.pfmt("{0}.{1}").op(NL_VERSION_MAJOR, NL_VERSION_MINOR);
        }


        //static pstring version_patchlevel();


        public netlist_state_t_nets_collection_type nets() { return m_nets; }


        public netlist_state_t_devices_collection_type devices() { return m_devices; }


        public netlist_state_t_family_collection_type family_cache() { return m_family_cache; }


        //template<typename T, typename... Args>
        //device_arena::unique_ptr<T> make_pool_object(Args&&... args)
        //{
        //    return plib::make_unique<T>(m_pool, std::forward<Args>(args)...);
        //}
        // memory pool - still needed in some places
        public device_arena pool() { return m_pool; }


        //struct stats_info
        //{
        //    const detail::queue_t               &m_queue;// performance
        //    const plib::pperftime_t<true>       &m_stat_mainloop;
        //    const plib::pperfcount_t<true>      &m_perf_out_processed;
        //};

        /// \brief print statistics gathered during run
        ///
        //void print_stats(stats_info &si) const;


        /// \brief call reset on all netlist components
        ///
        public void reset()
        {
            // Reset all nets once !
            log().verbose.op("Call reset on all nets:");
            foreach (var n in nets())
                n.reset();

            // Reset all devices once !
            log().verbose.op("Call reset on all devices:");
            foreach (var dev in m_devices)
                dev.second.reset();

            // Make sure everything depending on parameters is set
            // Currently analog input and logic input also
            // push their outputs to queue.

            std.vector<core_device_t> devices_called = new std.vector<core_device_t>();
            log().verbose.op("Call update_param on all devices:");
            foreach (var dev in m_devices)
            {
                dev.second.update_param();
                if (!plib.container.contains(devices_called, dev.second))
                    devices_called.push_back(dev.second);
            }

            // Step all devices once !
            //
            // INFO: The order here affects power up of e.g. breakout. However, such
            // variations are explicitly stated in the breakout manual.

            var netlist_params = get_single_device<devices.nld_netlistparams>("parameter");

            switch (netlist_params.m_startup_strategy.op())
            {
                case 0:
                {
                    std.vector<nldelegate> t = new std.vector<nldelegate>();
                    log().verbose.op("Using default startup strategy");
                    foreach (var n in m_nets)
                    {
                        n.update_inputs(); // only used if USE_COPY_INSTEAD_OF_REFERENCE == 1
                        foreach (var term in core_terms(n))
                        {
                            if (!plib.container.contains(t, term.delegate_()))
                            {
                                t.push_back(term.delegate_());
                                term.run_delegate();
                            }

                            // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
                            var dev = (core_device_t)term.delegate_device();  //auto *dev = reinterpret_cast<core_device_t *>(term->delegate().object());
                            if (!plib.container.contains(devices_called, dev))
                                devices_called.push_back(dev);
                        }
                    }

                    log().verbose.op("Devices not yet updated:");
                    foreach (var dev in m_devices)
                    { 
                        if (!plib.container.contains(devices_called, dev.second))
                        {
                            // FIXME: doesn't seem to be needed, use cases include
                            // analog output devices. Check and remove
                            log().error.op("\t Device {0} not yet updated", dev.second.name());
                            //dev.second->update();
                        }
                    }
                }
                break;
            }

            // the above may screw up m_active and the list
            rebuild_lists();
        }


        /// \brief prior to running free no longer needed resources
        ///
        public void free_setup_resources()
        {
            m_setup = null;
        }


        public std.vector<detail.core_terminal_t> core_terms(detail.net_t net)  //std::vector<detail::core_terminal_t *> &core_terms(const detail::net_t &net) noexcept
        {
            if (m_core_terms[net] == default)
                m_core_terms[net] = new std.vector<detail.core_terminal_t>();

            return m_core_terms[net];
        }
    }
} // namespace netlist
