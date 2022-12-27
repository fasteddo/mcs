// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.IO;

using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int64_t = System.Int64;
using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<NL_PREFER_INT128 && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using offs_t = System.UInt32;  //using offs_t = u32;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;
using param_logic_t = mame.netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool>;  //using param_logic_t = param_num_t<bool>;
using size_t = System.UInt64;
using sound_in_type = mame.netlist.interface_.nld_buffered_param_setter<mame.netlist_mame_sound_input_buffer>;  //using sound_in_type = netlist::interface::NETLIB_NAME(buffered_param_setter)<netlist_mame_sound_input_buffer>;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.attotime_global;
using static mame.device_global;
using static mame.distate_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.netlist_global;
using static mame.osdcore_global;
using static mame.static_solvers_global;


namespace mame
{
    public delegate void func_type(netlist.nlparse_t setup);  //using func_type = std::function<void(netlist::nlparse_t &)>;


    public static partial class netlist_global
    {
        //#define NETLIST_LOGIC_PORT_CHANGED(_base, _tag)  PORT_CHANGED_MEMBER(_base ":" _tag, netlist_mame_logic_input_device, input_changed, 0)
        //#define NETLIST_INT_PORT_CHANGED(_base, _tag)    PORT_CHANGED_MEMBER(_base ":" _tag, netlist_mame_logic_input_device, input_changed, 0)
        //#define NETLIST_ANALOG_PORT_CHANGED(_base, _tag) PORT_CHANGED_MEMBER(_base ":" _tag, netlist_mame_analog_input_device, input_changed, 0)

        public static void MEMREGION_SOURCE(netlist.nlparse_t setup, device_t device, string _name) { netlist_mame_device.register_memregion_source(setup, device, _name); }
        //#define NETDEV_ANALOG_CALLBACK_MEMBER(_name) void _name(const double data, const attotime &time)
        //#define NETDEV_LOGIC_CALLBACK_MEMBER(_name) void _name(const int data, const attotime &time)


        // netlib #defines this and it fights with logmacro.h
        //#undef LOG

        const int LOG_GENERAL     = 1 << 0;
        const int LOG_DEV_CALLS   = 1 << 1;
        const int LOG_DEBUG       = 1 << 2;
        const int LOG_TIMING      = 1 << 3;

        //#define LOG_MASK (LOG_GENERAL | LOG_DEV_CALLS | LOG_DEBUG)
        //#define LOG_MASK        (LOG_TIMING)
        const int LOG_MASK = 0;  //#define LOG_MASK        (0)

        public static void LOGDEVCALLS(string format, params object [] args) { LOGMASKED(LOG_DEV_CALLS, format, args); }  //#define LOGDEVCALLS(...) LOGMASKED(LOG_DEV_CALLS, __VA_ARGS__)
        public static void LOGDEBUG(string format, params object [] args) { LOGMASKED(LOG_DEBUG, format, args); }  //#define LOGDEBUG(...) LOGMASKED(LOG_DEBUG, __VA_ARGS__)
        public static void LOGTIMING(string format, params object [] args) { LOGMASKED(LOG_TIMING, format, args); }  //#define LOGTIMING(...) LOGMASKED(LOG_TIMING, __VA_ARGS__)

        static void LOG_OUTPUT_FUNC_netlist(device_t device, string format, params object [] args) { osd_printf_debug(format, args); }  //#define LOG_OUTPUT_FUNC printf

        static void LOGMASKED(int mask, string format, params object [] args) { if ((LOG_MASK & mask) != 0) LOG_OUTPUT_FUNC_netlist(null, format, args); }  //#define LOGMASKED(mask, ...) do { if (LOG_MASK & (mask)) (LOG_OUTPUT_FUNC)(__VA_ARGS__); } while (false)


        public static netlist_time_ext nltime_from_attotime(attotime t)
        {
            netlist_time_ext nlmtime = netlist_time_ext.from_sec(t.seconds());
            nlmtime += netlist_time_ext.from_raw(t.attoseconds() / (ATTOSECONDS_PER_SECOND / netlist_time_ext.resolution()));
            return nlmtime;
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NETLIST_CORE,  netlist_mame_device,       "netlist_core",  "Netlist Core Device")
        public static readonly emu.detail.device_type_impl NETLIST_CORE = DEFINE_DEVICE_TYPE("netlist_core", "Netlist Core Device", (type, mconfig, tag, owner, clock) => { return new netlist_mame_device(mconfig, tag, owner, clock); });


        //using func_type = std::function<void(netlist::nlparse_t &)>;


        // ----------------------------------------------------------------------------------------
        // Special netlist extension devices  ....
        // ----------------------------------------------------------------------------------------

        public class netlist_mame_t : netlist.netlist_state_t
        {
            netlist_mame_device m_parent;


            public netlist_mame_t(netlist_mame_device parent, string name)
                : base()  //: netlist::netlist_state_t(name, plib::plog_delegate(&netlist_mame_t::logger, this))
            {
                m_parent = parent;

                netlist_state_t_after_ctor(name, logger);
            }


            //running_machine &machine() { return m_parent.machine(); }
            //netlist_mame_device &parent() const { return m_parent; }


            void logger(plib.plog_level l, string ls)
            {
                switch (l)
                {
                case plib.plog_level.DEBUG:
                    m_parent.logerror("netlist DEBUG: {0}\n", ls);
                    break;
                case plib.plog_level.VERBOSE:
                    m_parent.logerror("netlist VERBOSE: {0}\n", ls);
                    break;
                case plib.plog_level.INFO:
                    m_parent.logerror("netlist INFO: {0}\n", ls);
                    break;
                case plib.plog_level.WARNING:
                    m_parent.logerror("netlist WARNING: {0}\n", ls);
                    break;
                case plib.plog_level.ERROR:
                    m_parent.logerror("netlist ERROR: {0}\n", ls);
                    break;
                case plib.plog_level.FATAL:
                    m_parent.logerror("netlist FATAL: {0}\n", ls);
                    break;
                }
            }
        }


        netlist_mame_t m_netlist;  //std::unique_ptr<netlist_mame_t> m_netlist;

        func_type m_setup_func;
        bool m_device_reset_called;


        // construction/destruction
        public netlist_mame_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, NETLIST_CORE, tag, owner, clock)
        {
        }

        public netlist_mame_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_setup_func = null;
            m_device_reset_called = false;
        }

        //virtual ~netlist_mame_device()


        public void set_setup_func(func_type func) { m_setup_func = func; }  //std::move(func); }


        public netlist.setup_t setup()
        {
            if (m_netlist == null)
                throw new device_missing_dependencies();

            return m_netlist.setup();
        }


        protected netlist_mame_t netlist() { return m_netlist; }


        public static void register_memregion_source(netlist.nlparse_t parser, device_t dev, string name)  //static void register_memregion_source(netlist::nlparse_t &parser, device_t &dev, const char *name);
        {
            parser.register_source(new netlist_source_memregion_t(dev, name));  //parser.register_source<netlist_source_memregion_t>(dev, name);
        }


        // Custom to netlist ...
        protected virtual void nl_register_devices(netlist.nlparse_t parser) { }


        // device_t overrides

        protected override void device_config_complete()
        {
            LOGDEVCALLS("device_config_complete {0}\n", this.mconfig().gamedrv().name);
        }


        protected override void device_validity_check(validity_checker valid)
        {
            base_validity_check(valid);
            //rom_exists(mconfig().root_device());
            LOGDEVCALLS("device_validity_check {0}\n", this.mconfig().gamedrv().name);
        }


        protected override void device_start()
        {
            LOGDEVCALLS("device_start entry\n");

            device_start_common();
            save_state();

            LOGDEVCALLS("device_start exit\n");
        }


        protected override void device_stop()
        {
            LOGDEVCALLS("device_stop\n");

            if (m_netlist != null)
                netlist().exec().stop();

#if NETLIST_CREATE_CSV
            if (m_csv_file != nullptr)
            {
                log_flush();
                fclose(m_csv_file);
            }
#endif
        }


        protected override void device_reset()
        {
            LOGDEVCALLS("device_reset\n");
            if (!m_device_reset_called)
            {
                // netlists don't have a reset line, doing a soft-reset is pointless
                // the only reason we call these here once after device_start
                // is that netlist input devices may be started after the netlist device
                // and because the startup code may trigger actions which need all
                // devices set up.
                netlist().free_setup_resources();
                netlist().exec().reset();
                m_device_reset_called = true;
            }
        }


        protected override void device_post_load()
        {
            LOGDEVCALLS("device_post_load\n");

            netlist().run_state_manager().post_load();
            netlist().rebuild_lists();
        }


        protected override void device_pre_save()
        {
            LOGDEVCALLS("device_pre_save\n");

            netlist().run_state_manager().pre_save();
        }


        protected void device_start_common()
        {
            m_netlist = new netlist_mame_t(this, "netlist");  //m_netlist = std::make_unique<netlist_mame_t>(*this, "netlist");

            m_netlist.set_static_solver_lib(new plib.dynlib_static(nl_static_solver_syms));  //m_netlist->set_static_solver_lib(std::make_unique<plib::dynlib_static>(nl_static_solver_syms));

            if (!machine().options().verbose())
            {
                m_netlist.log().verbose.set_enabled(false);
                m_netlist.log().debug.set_enabled(false);
            }

            common_dev_start(m_netlist);
            m_netlist.setup().prepare_to_run();


            m_device_reset_called = false;

#if NETLIST_CREATE_CSV
            std::string name = machine().system().name;
            name += tag();
            for (int index = 0; index < name.size(); index++)
                if (name[index] == ':')
                    name[index] = '_';
            name += ".csv";
            m_csv_file = fopen(name.c_str(), "wb");
#endif

            LOGDEVCALLS("device_start exit\n");
        }


        //void save_state();


        struct validity_logger
        {
            public void log(plib.plog_level l, string ls)
            {
                string ls8 = ls;

                switch (l)
                {
                case plib.plog_level.DEBUG:
                    break;
                case plib.plog_level.VERBOSE:
                    break;
                case plib.plog_level.INFO:
                    osd_printf_verbose("netlist INFO: {0}\n", ls8);
                    break;
                case plib.plog_level.WARNING:
                    osd_printf_warning("netlist WARNING: {0}\n", ls8);
                    break;
                case plib.plog_level.ERROR:
                    osd_printf_error("netlist ERROR: {0}\n", ls8);
                    break;
                case plib.plog_level.FATAL:
                    osd_printf_error("netlist FATAL: {0}\n", ls8);
                    break;
                }
            }
        }


        protected netlist.netlist_state_t base_validity_check(validity_checker valid)
        {
            try
            {
                validity_logger logger;

                //throw new emu_unimplemented();
#if false
                plib::chrono::timer<plib::chrono::system_ticks> t;
                t.start();
#endif

                var lnetlist = new netlist.netlist_state_t();                // enable validation mode  //auto lnetlist = std::make_unique<netlist::netlist_state_t>("netlist", plib::plog_delegate(&validity_logger::log, &logger));                // enable validation mode
                lnetlist.netlist_state_t_after_ctor("netlist", logger.log);

                lnetlist.set_static_solver_lib(new plib.dynlib_static(null));  //lnetlist->set_static_solver_lib(std::make_unique<plib::dynlib_static>(nullptr));

                common_dev_start(lnetlist);
                lnetlist.setup().prepare_to_run();

                foreach (device_t d in subdevices())
                {
                    device_t_with_netlist_mame_sub_interface sdev = (device_t_with_netlist_mame_sub_interface)d;
                    if (sdev != null)
                    {
                        LOGDEVCALLS("Validity check on subdevice {0}/{1}\n", d.name(), d.shortname());
                        sdev.validity_helper(valid, lnetlist);
                    }
                }

                //throw new emu_unimplemented();
#if false
                t.stop();
                //printf("time %s %f\n", this->mconfig().gamedrv().name, t.as_seconds<double>());
#endif

                return lnetlist;
            }
            catch (memregion_not_set err)
            {
                // Do not report an error. Validity check has no access to ROM area.
                osd_printf_verbose("{0}\n", err);
            }
            catch (emu_fatalerror err)
            {
                osd_printf_error("{0}\n", err);
            }
            catch (Exception err)
            {
                osd_printf_error("{0}\n", err);
            }

            return null;  //return netlist::host_arena::unique_ptr<netlist::netlist_state_t>(nullptr);
        }


        protected void save_state()
        {
            //throw new emu_unimplemented();
#if false
            for (auto const & s : netlist().run_state_manager().save_list())
            {
                netlist().log().debug("saving state for {1}\n", s->name().c_str());
                if (s->dt().is_float())
                {
                    if (s->dt().size() == sizeof(double))
                        save_pointer((double *) s->ptr(), s->name().c_str(), s->count());
                    else if (s->dt().size() == sizeof(float))
                        save_pointer((float *) s->ptr(), s->name().c_str(), s->count());
                    else
                        netlist().log().fatal("Unknown floating type for {1}\n", s->name().c_str());
                }
                else if (s->dt().is_integral())
                {
                    if (s->dt().size() == sizeof(int64_t))
                        save_pointer((int64_t *) s->ptr(), s->name().c_str(), s->count());
                    else if (s->dt().size() == sizeof(int32_t))
                        save_pointer((int32_t *) s->ptr(), s->name().c_str(), s->count());
                    else if (s->dt().size() == sizeof(int16_t))
                        save_pointer((int16_t *) s->ptr(), s->name().c_str(), s->count());
                    else if (s->dt().size() == sizeof(int8_t))
                        save_pointer((int8_t *) s->ptr(), s->name().c_str(), s->count());
                    else if (plib::compile_info::has_int128::value && s->dt().size() == sizeof(INT128))
                        save_pointer((int64_t *) s->ptr(), s->name().c_str(), s->count() * 2);
                    else
                        netlist().log().fatal("Unknown integral type size {1} for {2}\n", s->dt().size(), s->name().c_str());
                }
                else if (s->dt().is_custom())
                {
                    /* do nothing */
                }
                else
                    netlist().log().fatal("found unsupported save element {1}\n", s->name());
            }
#endif
        }


        void common_dev_start(netlist.netlist_state_t lnetlist)
        {
            var lsetup = lnetlist.setup();

            //throw new emu_unimplemented();
#if false
            // Override log statistics
            pstring p = plib::util::environment("NL_STATS", "");
            if (p != "")
            {
                bool err=false;
                bool v = plib::pstonum_ne<bool>(p, err);
                if (err)
                    lsetup.log().warning("NL_STATS: invalid value {1}", p);
                else
                    lnetlist->exec().enable_stats(v);
            }
#endif

            // register additional devices

            nl_register_devices(lsetup.parser());

            /* let sub-devices add sources and do stuff prior to parsing */
            foreach (device_t d in subdevices())
            {
                device_t_with_netlist_mame_sub_interface sdev = (device_t_with_netlist_mame_sub_interface)d;  //netlist_mame_sub_interface *sdev = dynamic_cast<netlist_mame_sub_interface *>(&d);
                if (sdev != null)
                {
                    LOGDEVCALLS("Preparse subdevice {0}/{1}\n", d.name(), d.shortname());
                    sdev.pre_parse_action(lsetup.parser());
                }
            }

            // add default data provider for roms - if not in validity check
            lsetup.parser().register_source(new netlist_data_memregions_t(this));  //lsetup.parser().register_source<netlist_data_memregions_t>(*this);

            // Read the netlist
            m_setup_func(lsetup.parser());

            // let sub-devices tweak the netlist
            foreach (device_t d in subdevices())
            {
                device_t_with_netlist_mame_sub_interface sdev = (device_t_with_netlist_mame_sub_interface)d;  //netlist_mame_sub_interface *sdev = dynamic_cast<netlist_mame_sub_interface *>(&d);
                if (sdev != null)
                {
                    LOGDEVCALLS("Found subdevice {0}/{1}\n", d.name(), d.shortname());
                    sdev.custom_netlist_additions(lsetup.parser());
                }
            }
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_cpu_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_cpu_device : netlist_mame_device
                                           //device_execute_interface,
                                           //device_state_interface,
                                           //device_disasm_interface,
                                           //device_memory_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_CPU,   netlist_mame_cpu_device,   "netlist_cpu",   "Netlist CPU Device")
        public static readonly emu.detail.device_type_impl NETLIST_CPU = DEFINE_DEVICE_TYPE("netlist_cpu", "Netlist CPU Device", (type, mconfig, tag, owner, clock) => { return new netlist_mame_cpu_device(mconfig, tag, owner, clock); });


        public class device_execute_interface_netlist_mame : device_execute_interface
        {
            public device_execute_interface_netlist_mame(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void execute_run() { ((netlist_mame_cpu_device)device()).device_execute_interface_execute_run(); }
            protected override u64 execute_clocks_to_cycles(u64 clocks) { return ((netlist_mame_cpu_device)device()).device_execute_interface_execute_clocks_to_cycles(clocks); }
            protected override u64 execute_cycles_to_clocks(u64 cycles) { return ((netlist_mame_cpu_device)device()).device_execute_interface_execute_cycles_to_clocks(cycles); }
        }


        public class device_memory_interface_netlist_mame : device_memory_interface
        {
            public device_memory_interface_netlist_mame(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((netlist_mame_cpu_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_netlist_mame : device_state_interface
        {
            public device_state_interface_netlist_mame(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void state_import(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_export(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_string_export(device_state_entry entry, out string str) { ((netlist_mame_cpu_device)device()).device_state_interface_state_string_export(entry, out str); }
        }


        public class device_disasm_interface_netlist_mame : device_disasm_interface
        {
            public device_disasm_interface_netlist_mame(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { return ((netlist_mame_cpu_device)device()).device_disasm_interface_create_disassembler(); }
        }


        const unsigned MDIV_SHIFT = 16;


        device_memory_interface_netlist_mame m_dimemory;
        public device_execute_interface_netlist_mame m_diexec;
        device_state_interface_netlist_mame m_distate;


        address_space_config m_program_config;

        intref m_icount = new intref();  //int m_icount;
        netlist_time_ext m_div;
        netlist_time_ext m_rem;
        netlist_time_ext m_old;
        offs_t m_genPC;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_cpu_device
        // ----------------------------------------------------------------------------------------
        netlist_mame_cpu_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NETLIST_CPU, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_netlist_mame(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_netlist_mame(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_netlist_mame(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_netlist_mame(mconfig, this));
            m_diexec = GetClassInterface<device_execute_interface_netlist_mame>();
            m_distate = GetClassInterface<device_state_interface_netlist_mame>();
            m_dimemory = GetClassInterface<device_memory_interface_netlist_mame>();


            m_program_config = new address_space_config("program", ENDIANNESS_LITTLE, 8, 12); // Interface is needed to keep debugger happy
            m_icount.i = 0;


            m_old = netlist_time_ext.zero();
            m_genPC = 0;
        }


        //~netlist_mame_cpu_device() { }


        //offs_t genPC() const { return m_genPC; }


        //netlist_mame_cpu_device & set_source(void (*setup_func)(netlist::nlparse_t &))
        //template <typename T, typename F>
        public netlist_mame_cpu_device set_source(object obj, func_type f)  //netlist_mame_cpu_device & set_source(T *obj, F && f)
        {
            set_setup_func(f);
            return this;
        }


        public void update_icount(netlist_time_ext time)
        {
            netlist_time_ext delta = (time - m_old).shl(MDIV_SHIFT) + m_rem;
            uint64_t d = (uint64_t)(delta / m_div);
            m_old = time;
            m_rem = delta - (m_div * (int64_t)d);
            //printf("d %d m_rem %d\n", (int) d, (int) m_rem.as_raw());
            m_icount.i -= (int)d;
        }


        public void check_mame_abort_slice()
        {
            if (m_icount.i <= 0)
                netlist().exec().abort_current_queue_slice();
        }


        netlist_time_ext nltime_ext_from_clocks(unsigned c)
        {
            return (m_div * c).shr(MDIV_SHIFT);
        }

        //netlist::netlist_time nltime_from_clocks(unsigned c) const noexcept
        //{
        //    return static_cast<netlist::netlist_time>((m_div * c).shr(MDIV_SHIFT));
        //}


        // netlist_mame_device
        protected override void nl_register_devices(netlist.nlparse_t parser)
        {
        }


        // device_t overrides

        protected override void device_start()
        {
            LOGDEVCALLS("device_start entry\n");

            device_start_common();
            // FIXME: use save_helper
            netlist().save(this, m_rem, this.name(), "m_rem");
            netlist().save(this, m_div, this.name(), "m_div");
            netlist().save(this, m_old, this.name(), "m_old");

            m_old = netlist_time_ext.zero();
            m_rem = netlist_time_ext.zero();

            save_state();

            m_distate.state_add(STATE_GENPC, "GENPC", m_genPC).noshow();
            m_distate.state_add(STATE_GENPCBASE, "CURPC", m_genPC).noshow();

            int index = 0;
            foreach (var n in netlist().nets())
            {
                string name = n.name();  //putf8string name(n->name()); //plib::replace_all(n->name(), ".", "_");
                if (n.is_logic())
                {
                    var nl = (netlist.logic_net_t)n;  //auto nl = downcast<netlist::logic_net_t *>(n.get());
                    m_distate.state_add<netlist_sig_t, device_state_register_operators_u32>(index++, name,
                        () => { return nl.Q(); },  //[nl]() { return nl->Q(); },
                        (netlist_sig_t data) => { nl.set_Q_and_push(data, netlist_time.quantum()); });  //[nl](netlist::netlist_sig_t data) { nl->set_Q_and_push(data, netlist::netlist_time::quantum()); });
                }
                else
                {
                    var nl = (netlist.analog_net_t)n;  //auto nl = downcast<netlist::analog_net_t *>(n.get());
                    m_distate.state_add<double, device_state_register_operators_double>(
                        index++,
                        name,
                        () => { return nl.Q_Analog(); },  //[nl]() { return nl->Q_Analog(); },
                        (double data) => { nl.set_Q_Analog(data); });  //[nl](double data) { nl->set_Q_Analog(data); });
                }
            }

            // set our instruction counter
            m_diexec.set_icountptr(m_icount);

            LOGDEVCALLS("device_start exit\n");
        }


        protected override void device_clock_changed()
        {
            m_div = new netlist_time_ext(  //m_div = static_cast<netlist::netlist_time_ext>(
                (netlist_time_ext.resolution() << (int)MDIV_SHIFT) / clock());
            //printf("m_div %d\n", (int) m_div.as_raw());
            netlist().log().debug.op("Setting clock {0} and divisor {1}\n", clock(), m_div.as_double());
        }


        // device_execute_interface overrides

        protected virtual uint64_t device_execute_interface_execute_clocks_to_cycles(uint64_t clocks) { return clocks; }  //virtual uint64_t execute_clocks_to_cycles(uint64_t clocks) const noexcept override;
        protected virtual uint64_t device_execute_interface_execute_cycles_to_clocks(uint64_t cycles) { return cycles; }  //virtual uint64_t execute_cycles_to_clocks(uint64_t cycles) const noexcept override;

        protected virtual void device_execute_interface_execute_run()  //ATTR_HOT virtual void execute_run() override;
        {
            //m_ppc = m_pc; // copy PC to previous PC
            if (m_diexec.debugger_enabled())
            {
                while (m_icount.i > 0)
                {
                    m_genPC++;
                    m_genPC &= 255;
                    m_diexec.debugger_instruction_hook(m_genPC);
                    netlist().exec().process_queue(nltime_ext_from_clocks(1));
                    update_icount(netlist().exec().time());
                }
            }
            else
            {
                netlist().exec().process_queue(nltime_ext_from_clocks((unsigned)m_icount.i));
                update_icount(netlist().exec().time());
            }
        }


        // device_disasm_interface overrides
        protected virtual util.disasm_interface device_disasm_interface_create_disassembler() { throw new emu_unimplemented(); }  //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        // device_memory_interface overrides

        protected virtual space_config_vector device_memory_interface_memory_space_config()  //virtual space_config_vector memory_space_config() const override;
        {
            return new space_config_vector {
                std.make_pair(AS_PROGRAM, m_program_config)
            };
        }


        // device_state_interface overrides
        protected virtual void device_state_interface_state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }  //virtual void state_string_export(const device_state_entry &entry, std::string &str) const override;
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_cpu_device
    // ----------------------------------------------------------------------------------------
    //class netlist_disassembler : public util::disasm_interface


    // ----------------------------------------------------------------------------------------
    // netlist_mame_sound_input_buffer
    // ----------------------------------------------------------------------------------------
    class netlist_mame_sound_input_buffer : read_stream_view
    {
        public netlist_mame_sound_input_buffer()
            : base() { }

        public netlist_mame_sound_input_buffer(read_stream_view src)
            : base(src) { }


        //stream_buffer::sample_t operator[](std::size_t index) { return get(index); }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_sound_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_sound_device : netlist_mame_device
                                             //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_SOUND, netlist_mame_sound_device, "netlist_sound", "Netlist Sound Device")
        public static readonly emu.detail.device_type_impl NETLIST_SOUND = DEFINE_DEVICE_TYPE("netlist_sound", "Netlist Sound Device", (type, mconfig, tag, owner, clock) => { return new netlist_mame_sound_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_netlist_mame_sound : device_sound_interface
        {
            public device_sound_interface_netlist_mame_sound(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((netlist_mame_sound_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }
        }


        device_sound_interface_netlist_mame_sound m_disound;


        std.map<int, netlist_mame_stream_output_device> m_out = new std.map<int, netlist_mame_stream_output_device>();
        std.map<size_t, nld_sound_in> m_in = new std.map<size_t, nld_sound_in>();
        std.vector<netlist_mame_sound_input_buffer> m_inbuffer = new std.vector<netlist_mame_sound_input_buffer>();
        sound_stream m_stream;
        attotime m_cur_time;
        uint32_t m_sound_clock;
        attotime m_attotime_per_clock;
        attotime m_last_update_to_current_time;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_sound_device_t
        // ----------------------------------------------------------------------------------------
        netlist_mame_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, NETLIST_SOUND, tag, owner, 0)
        {
            m_class_interfaces.Add(new device_sound_interface_netlist_mame_sound(mconfig, this));  //device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_netlist_mame_sound>();

            m_cur_time = attotime.zero;
            m_sound_clock = clock;
            m_attotime_per_clock = attotime.zero;
            m_last_update_to_current_time = attotime.zero;
        }


        public device_sound_interface_netlist_mame_sound disound { get { return m_disound; } }


        //netlist_mame_sound_device & set_source(void (*setup_func)(netlist::nlparse_t &))
        //template <typename T, typename F> netlist_mame_sound_device & set_source(T *obj, F && f)
        public netlist_mame_sound_device set_source(func_type setup_func) { set_setup_func(setup_func); return this; }


        public sound_stream get_stream() { return m_stream; }


        public void update_to_current_time()
        {
            LOGDEBUG("before update\n");
            get_stream().update();

            if (machine().time() < m_last_update_to_current_time)
                LOGTIMING("machine.time() decreased 2\n");

            m_last_update_to_current_time = machine().time();

            var mtime = nltime_from_attotime(machine().time());
            var cur = netlist().exec().time();

            if (mtime > cur)
            {
                //expected don't log
                //LOGTIMING("%f us\n", (mtime - cur).as_double() * 1000000.0);
                netlist().exec().process_queue(mtime - cur);
            }
            else if (mtime < cur)
            {
                LOGTIMING("{0} : {1} us before machine time\n", this.name(), (cur - mtime).as_double() * 1000000.0);
            }
        }


        public void register_stream_output(int channel, netlist_mame_stream_output_device so)
        {
            m_out[channel] = so;
        }


        // netlist_mame_device
        protected override void nl_register_devices(netlist.nlparse_t parser)
        {
            ////parser.factory().add<nld_sound_out>("NETDEV_SOUND_OUT",
            ////  netlist::factory::properties("+CHAN", PSOURCELOC()));
            parser.factory_().add_nld_sound_in("NETDEV_SOUND_IN", new netlist.factory.properties("-", plib.pg.PSOURCELOC()));  //parser.factory_().add<nld_sound_in>("NETDEV_SOUND_IN", netlist.factory.properties("-", PSOURCELOC()));
        }


        // device_t overrides

        protected override void device_start()
        {
            LOGDEVCALLS("sound device_start\n");

            m_attotime_per_clock = attotime.from_hz(m_sound_clock);

            save_item(NAME(new { m_cur_time }));
            save_item(NAME(new { m_attotime_per_clock }));

            device_start_common();
            save_state();

            m_cur_time = attotime.zero;

            // Configure outputs

            if (m_out.size() == 0)
                fatalerror("No output devices");

            /* resort channels */
            foreach (var outdev in m_out)
            {
                if (outdev.first() < 0 || outdev.first() >= (int)m_out.size())
                    fatalerror("illegal output channel number {0}", outdev.first());
                outdev.second().set_sample_time(netlist_time.from_hz(m_sound_clock));
                outdev.second().buffer_reset(netlist_time_ext.zero());
            }

            // Configure inputs

            m_in.clear();

            std.vector<nld_sound_in> indevs = netlist().get_device_list<nld_sound_in>();
            foreach (var e in indevs)
            {
                m_in.emplace(e.id(), e);
                var sample_time = netlist_time.from_raw((int64_t)nltime_from_attotime(m_attotime_per_clock).as_raw());  //const auto sample_time = netlist::netlist_time::from_raw(static_cast<netlist::netlist_time::internal_type>(nltime_from_attotime(m_attotime_per_clock).as_raw()));
                e.resolve_params(sample_time);
            }

            foreach (var e in m_in)
            {
                if (e.first() < 0 || e.first() >= m_in.size())
                    fatalerror("illegal input channel number {0}", e.first());
            }

            m_inbuffer.resize(m_in.size());

            /* initialize the stream(s) */
            m_stream = m_disound.stream_alloc((int)m_in.size(), (int)m_out.size(), m_sound_clock, sound_stream_flags.STREAM_DISABLE_INPUT_RESAMPLING);

            LOGDEVCALLS("sound device_start exit\n");
        }


        // device_sound_interface overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs)
        {
            foreach (var e in m_in)
            {
                var clock_period = inputs[e.first()].sample_period();
                var sample_time = netlist_time.from_raw((int64_t)nltime_from_attotime(clock_period).as_raw());  //auto sample_time = netlist::netlist_time::from_raw(static_cast<netlist::netlist_time::internal_type>(nltime_from_attotime(clock_period).as_raw()));
                m_inbuffer[e.first()] = new netlist_mame_sound_input_buffer(inputs[e.first()]);
                e.second().buffer_reset(sample_time, m_inbuffer[e.first()].samples(), m_inbuffer[e.first()]);
            }

            int samples = (int)outputs[0].samples();
            LOGDEBUG("samples {0}\n", samples);

            // end_time() is the time at the END of the last sample we're generating
            // however, the sample value is the value at the START of that last sample,
            // so subtract one sample period so that we only process up to the minimum
            var nl_target_time = nltime_from_attotime(outputs[0].end_time() - outputs[0].sample_period());

            var nltime = netlist().exec().time();
            if (nltime < nl_target_time)
            {
                netlist().exec().process_queue(nl_target_time - nltime);
            }

            foreach (var e in m_out)
            {
                e.second().sound_update_fill(outputs[e.first()]);
                e.second().buffer_reset(nl_target_time);
            }
        }


        protected override void device_validity_check(validity_checker valid)
        {
            throw new emu_unimplemented();
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_sub_interface
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_sub_interface
    {
        public double m_offset;
        public double m_mult;

        netlist_mame_device m_owner;
        netlist_mame_sound_device m_sound;
        netlist_mame_cpu_device m_cpu;


        // construction/destruction
        public netlist_mame_sub_interface(device_t owner)
        {
            m_offset = 0.0;
            m_mult = 1.0;

            m_owner = owner is netlist_mame_device owner_device ? owner_device : null;
            m_sound = owner is netlist_mame_sound_device owner_sound ? owner_sound : null;
            m_cpu = owner is netlist_mame_cpu_device owner_cpu ? owner_cpu : null;
        }

        //virtual ~netlist_mame_sub_interface() { }


        public virtual void custom_netlist_additions(netlist.nlparse_t parser) { }
        public virtual void pre_parse_action(netlist.nlparse_t parser) { }


        public virtual void validity_helper(validity_checker valid, netlist.netlist_state_t nlstate) { throw new emu_unimplemented(); }  // need to re-do device_t_with_mame_sub_interface.  Doesn't do inheritance right


        public netlist_mame_device nl_owner() { return m_owner; }


        public void update_to_current_time() { if (m_sound != null) m_sound.update_to_current_time(); }


        public void set_mult_offset(double mult, double offset)
        {
            m_mult = mult;
            m_offset = offset;
        }


        public netlist_mame_sound_device sound() { return m_sound;}
        public netlist_mame_cpu_device cpu() { return m_cpu;}
    }


    public abstract class device_t_with_netlist_mame_sub_interface : device_t
    {
        protected netlist_mame_sub_interface m_netlist_mame_sub_interface;


        public device_t_with_netlist_mame_sub_interface(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, type, tag, owner, clock)
        {
            m_netlist_mame_sub_interface = new netlist_mame_sub_interface(owner);
        }


        protected double m_offset { get { return m_netlist_mame_sub_interface.m_offset; } set { m_netlist_mame_sub_interface.m_offset = value; } }
        protected double m_mult { get { return m_netlist_mame_sub_interface.m_mult; } set { m_netlist_mame_sub_interface.m_mult = value; } }

        public virtual void custom_netlist_additions(netlist.nlparse_t parser) { m_netlist_mame_sub_interface.custom_netlist_additions(parser); }
        public virtual void pre_parse_action(netlist.nlparse_t parser) { m_netlist_mame_sub_interface.pre_parse_action(parser); }

        public virtual void validity_helper(validity_checker valid, netlist.netlist_state_t nlstate) { m_netlist_mame_sub_interface.validity_helper(valid, nlstate); }

        protected netlist_mame_device nl_owner() { return m_netlist_mame_sub_interface.nl_owner(); }

        protected void update_to_current_time() { m_netlist_mame_sub_interface.update_to_current_time(); }

        public void set_mult_offset(double mult, double offset) { m_netlist_mame_sub_interface.set_mult_offset(mult, offset); }

        protected netlist_mame_sound_device sound() { return m_netlist_mame_sub_interface.sound();}
        protected netlist_mame_cpu_device cpu() { return m_netlist_mame_sub_interface.cpu(); }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_analog_input_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_analog_input_device : device_t_with_netlist_mame_sub_interface
                                                    //device_t
                                                    //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_ANALOG_INPUT,  netlist_mame_analog_input_device,  "nl_analog_in",  "Netlist Analog Input")
        public static readonly emu.detail.device_type_impl NETLIST_ANALOG_INPUT = DEFINE_DEVICE_TYPE("nl_analog_in", "Netlist Analog Input", (type, mconfig, tag, owner, clock) => { return new netlist_mame_analog_input_device(mconfig, tag, owner, clock); });


        netlist.param_num_t<nl_fptype, netlist.param_num_t_operators_double> m_param;  //netlist::param_num_t<netlist::nl_fptype> *m_param;
        bool m_auto_port;
        string m_param_name;
        double m_value_for_device_timer;


        // construction/destruction
        netlist_mame_analog_input_device(machine_config mconfig, string tag, device_t owner, string param_name)
            : base(mconfig, NETLIST_ANALOG_INPUT, tag, owner, 0)
        {
            m_param = null;
            m_auto_port = true;
            m_param_name = param_name;
            m_value_for_device_timer = 0;
        }

        netlist_mame_analog_input_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, NETLIST_ANALOG_INPUT, tag, owner, clock)
        {
            m_param = null;
            m_auto_port = true;
            m_param_name = "";
            m_value_for_device_timer = 0;
        }


        public void set_name(string param_name) { m_param_name = param_name; }


        void write(double val)
        {
            m_value_for_device_timer = val * m_mult + m_offset;
            if (m_value_for_device_timer != m_param.op())
            {
                synchronize();
            }
        }


        public void input_changed(ioport_field field, u32 param, ioport_value oldval, ioport_value newval)  //inline DECLARE_INPUT_CHANGED_MEMBER(input_changed)
        {
            if (m_auto_port)
                write(((double)newval - (double)field.minval()) / (double)(field.maxval() - field.minval()));
            else
                write(newval);
        }


        //inline DECLARE_WRITE_LINE_MEMBER(write_line)       { write(state);  }
        //inline void write8(uint8_t data)               { write(data);   }
        //inline void write16(uint16_t data)             { write(data);   }
        //inline void write32(uint32_t data)             { write(data);   }
        //inline void write64(uint64_t data)             { write(data);   }


        public override void validity_helper(validity_checker valid, netlist.netlist_state_t nlstate)
        {
            throw new emu_unimplemented();
        }


        // device-level overrides
        protected override void device_start()
        {
            LOGDEVCALLS("start\n");
            netlist.param_ref_t p = this.nl_owner().setup().find_param(m_param_name);
            // FIXME: m_param should be param_ref_t
            m_param = p.param() is param_fp_t param_fp ? param_fp : null;  //m_param = dynamic_cast<netlist::param_fp_t *>(&p.param());
            if (m_param == null)
            {
                fatalerror("device {0} wrong parameter type for {1}\n", basetag(), m_param_name);
            }

            if (m_mult != 1.0 || m_offset != 0.0)
            {
                // disable automatic scaling for ioports
                m_auto_port = false;
            }
        }


        protected override void device_timer(emu_timer timer, device_timer_id id, int param)
        {
            update_to_current_time();
#if NETLIST_CREATE_CSV
            nl_owner().log_add(m_param_name, m_value_for_device_timer, true);
#endif
            m_param.set(m_value_for_device_timer);
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_analog_output_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_analog_output_device : device_t_with_netlist_mame_sub_interface
                                                     //device_t,
                                                     //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_ANALOG_OUTPUT, netlist_mame_analog_output_device, "nl_analog_out", "Netlist Analog Output")
        public static readonly emu.detail.device_type_impl NETLIST_ANALOG_OUTPUT = DEFINE_DEVICE_TYPE("nl_analog_out", "Netlist Analog Output", (type, mconfig, tag, owner, clock) => { return new netlist_mame_analog_output_device(mconfig, tag, owner, clock); });


        public delegate void output_delegate(double data, attotime time);  //typedef device_delegate<void (const double, const attotime &)> output_delegate;


        string m_in;
        output_delegate m_delegate;


        // construction/destruction
        netlist_mame_analog_output_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, NETLIST_ANALOG_OUTPUT, tag, owner, clock)
        {
            m_in = "";
            m_delegate = null;  //, m_delegate(*this)
        }


        //template <typename... T>
        public void set_params(string in_name, string device, output_delegate args)  //void set_params(const char *in_name, T &&... args)
        {
            m_in = in_name;
            m_delegate = args;  //m_delegate.set(std::forward<T>(args)...);
        }


        // device-level overrides
        protected override void device_start()
        {
            LOGDEVCALLS("start\n");
        }


        public override void pre_parse_action(netlist.nlparse_t parser)
        {
            string pin = m_in;
            string dname = "OUT_" + pin;

            netlist.interface_.nld_analog_callback.FUNC lambda = (in_, val) =>  //const auto lambda = [this](auto &in, netlist::nl_fptype val)
            {
                this.cpu().update_icount(in_.exec().time());
                this.m_delegate(val, this.cpu().m_diexec.local_time());
                this.cpu().check_mame_abort_slice();
            };

            //using lb_t = decltype(lambda);
            //using cb_t = netlist::interface::NETLIB_NAME(analog_callback)<lb_t>;

            parser.factory_().add_nld_analog_callback<netlist.interface_.nld_analog_callback, nl_fptype, netlist.interface_.nld_analog_callback.FUNC>(dname,  //parser.factory().add<cb_t, netlist::nl_fptype, lb_t>(dname,
                new netlist.factory.properties("-", plib.pg.PSOURCELOC()), 1e-6, lambda);  //netlist::factory::properties("-", PSOURCELOC()), 1e-6, std::forward<lb_t>(lambda));
        }

        public override void custom_netlist_additions(netlist.nlparse_t parser)
        {
            /* ignore if no running machine -> called within device_validity_check context */
            //if (owner()->has_running_machine())
            //    m_delegate.resolve();

            string pin = m_in;
            string dname = "OUT_" + pin;

            parser.register_dev(dname, dname);
            parser.register_link(dname + ".IN", pin);
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_logic_output_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_logic_output_device : device_t_with_netlist_mame_sub_interface
                                                    //device_t,
                                                    //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_LOGIC_OUTPUT,  netlist_mame_logic_output_device,  "nl_logic_out",  "Netlist Logic Output")
        public static readonly emu.detail.device_type_impl NETLIST_LOGIC_OUTPUT = DEFINE_DEVICE_TYPE("nl_logic_out", "Netlist Logic Output", (type, mconfig, tag, owner, clock) => { return new netlist_mame_logic_output_device(mconfig, tag, owner, clock); });


        public delegate void output_delegate(int data, attotime time);  //typedef device_delegate<void(const int, const attotime &)> output_delegate;


        string m_in;
        output_delegate m_delegate;


        // construction/destruction
        netlist_mame_logic_output_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, NETLIST_LOGIC_OUTPUT, tag, owner, clock)
        {
            m_in = "";
            m_delegate = null;  //, m_delegate(*this)
        }


        //template <typename... T>
        public void set_params(string in_name, output_delegate args)  //void set_params(const char *in_name, T &&... args)
        {
            m_in = in_name;
            m_delegate = args;  //m_delegate.set(std::forward<T>(args)...);
        }


        // device-level overrides
        protected override void device_start()
        {
            LOGDEVCALLS("start\n");
        }


        public override void pre_parse_action(netlist.nlparse_t parser)
        {
            string pin = m_in;
            string dname = "OUT_" + pin;

            netlist.interface_.nld_logic_callback.FUNC lambda = (in_, val) =>  //const auto lambda = [this](auto &in, netlist::netlist_sig_t val)
            {
                this.cpu().update_icount(in_.exec().time());
                this.m_delegate((int)val, this.cpu().m_diexec.local_time());
                this.cpu().check_mame_abort_slice();
            };

            //using lb_t = decltype(lambda);
            //using cb_t = netlist::interface::NETLIB_NAME(logic_callback)<lb_t>;

            parser.factory_().add_nld_logic_callback(dname,
                new netlist.factory.properties("-", plib.pg.PSOURCELOC()), lambda);  //netlist::factory::properties("-", PSOURCELOC()), std::forward<lb_t>(lambda));
        }


        public override void custom_netlist_additions(netlist.nlparse_t parser)
        {
            /* ignore if no running machine -> called within device_validity_check context */
            //if (owner().has_running_machine())
            //    m_delegate.resolve();

            string pin = m_in;
            string dname = "OUT_" + pin;

            parser.register_dev(dname, dname);
            parser.register_link(dname + ".IN", pin);
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_int_input_device
    // ----------------------------------------------------------------------------------------
    //class netlist_mame_int_input_device : public device_t, public netlist_mame_sub_interface


    // ----------------------------------------------------------------------------------------
    // netlist_mame_logic_input_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_logic_input_device : device_t_with_netlist_mame_sub_interface
                                                   //device_t
                                                   //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_LOGIC_INPUT,   netlist_mame_logic_input_device,   "nl_logic_in",   "Netlist Logic Input")
        public static readonly emu.detail.device_type_impl NETLIST_LOGIC_INPUT = DEFINE_DEVICE_TYPE("nl_logic_in", "Netlist Logic Input", (type, mconfig, tag, owner, clock) => { return new netlist_mame_logic_input_device(mconfig, tag, owner, clock); });


        netlist.param_num_t<bool, netlist.param_num_t_operators_bool> m_param;
        uint32_t m_shift;
        string m_param_name;


        // construction/destruction

        netlist_mame_logic_input_device(machine_config mconfig, string tag, device_t owner, string param_name, uint32_t shift)
            : this(mconfig, tag, owner, 0)
        {
            set_params(param_name, shift);
        }

        netlist_mame_logic_input_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, NETLIST_LOGIC_INPUT, tag, owner, clock)
        {
            m_param = null;
            m_shift = 0;
            m_param_name = "";
        }


        public void set_params(string param_name, uint32_t shift)
        {
            //throw new emu_unimplemented();
#if false
            if (LOG_DEV_CALLS) logerror("set_params\n");
#endif
            m_param_name = param_name;
            m_shift = shift;
        }


        public void write(uint32_t val)
        {
            uint32_t v = (val >> (int)m_shift) & 1;
            if ((v != 0) != m_param.op())
            {
                LOGDEBUG("write {0}\n", this.tag());
                synchronize(0, (int)v);
            }
        }


        public void input_changed(ioport_field field, u32 param, ioport_value oldval, ioport_value newval) { write(newval); }  //inline DECLARE_INPUT_CHANGED_MEMBER(input_changed) { write(newval); }


        //DECLARE_WRITE_LINE_MEMBER(write_line)       { write(state);  }
        public void write_line(int state) { write((uint32_t)state);  }
        //void write8(uint8_t data)               { write(data);   }
        //void write16(uint16_t data)             { write(data);   }
        //void write32(uint32_t data)             { write(data);   }
        //void write64(uint64_t data)             { write(data);   }


        public override void validity_helper(validity_checker valid, netlist.netlist_state_t nlstate)
        {
            throw new emu_unimplemented();
        }


        // device-level overrides
        protected override void device_start()
        {
            LOGDEVCALLS("start\n");

            netlist.param_ref_t p = ((netlist_mame_device)this.owner()).setup().find_param(m_param_name);  //netlist::param_ref_t p = downcast<netlist_mame_device *>(this->owner())->setup().find_param(pstring(m_param_name));
            m_param = (param_logic_t)p.param();  //m_param = dynamic_cast<netlist::param_logic_t *>(&p.param());
            if (m_param == null)
            {
                fatalerror("device {0} wrong parameter type for {1}\n", basetag(), m_param_name);
            }
        }

        protected override void device_timer(emu_timer timer, device_timer_id id, int param)
        {
            m_netlist_mame_sub_interface.update_to_current_time();

#if NETLIST_CREATE_CSV
            nl_owner().log_add(m_param_name, param, false);
#endif

            m_param.set(param != 0);
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_ram_pointer_device
    // ----------------------------------------------------------------------------------------
    //class netlist_mame_ram_pointer_device : public device_t, public netlist_mame_sub_interface


    // ----------------------------------------------------------------------------------------
    // netlist_mame_stream_input_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_stream_input_device : device_t_with_netlist_mame_sub_interface
                                                    //device_t
                                                    //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_STREAM_INPUT,  netlist_mame_stream_input_device,  "nl_stream_in",  "Netlist Stream Input")
        public static readonly emu.detail.device_type_impl NETLIST_STREAM_INPUT = DEFINE_DEVICE_TYPE("nl_stream_in", "Netlist Stream Input", (type, mconfig, tag, owner, clock) => { return new netlist_mame_stream_input_device(mconfig, tag, owner, clock); });


        uint32_t m_channel;
        string m_param_name;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_stream_input_t
        // ----------------------------------------------------------------------------------------

        netlist_mame_stream_input_device(machine_config mconfig, string tag, device_t owner, int channel, string param_name)
            : this(mconfig, tag, owner, (uint32_t)0)
        {
            set_params(channel, param_name);
        }

        netlist_mame_stream_input_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, NETLIST_STREAM_INPUT, tag, owner, clock)
        {
            m_channel = 0;
            m_param_name = "";
        }


        public void set_params(int channel, string param_name)
        {
            m_param_name = param_name;
            m_channel = (uint32_t)channel;
        }


        // device-level overrides
        protected override void device_start()
        {
            LOGDEVCALLS("start\n");
        }


        public override void custom_netlist_additions(netlist.nlparse_t parser)
        {
            string name = new plib.pfmt("STREAM_INPUT_{0}").op(m_channel);
            parser.register_dev("NETDEV_SOUND_IN", name);

            parser.register_param(name + ".CHAN", m_param_name);
            parser.register_param(name + ".MULT", m_netlist_mame_sub_interface.m_mult);
            parser.register_param(name + ".OFFSET", m_netlist_mame_sub_interface.m_offset);
            parser.register_param(name + ".ID", m_channel);
        }
    }


    /// \brief save state helper for plib classes supporting the save_state interface
    ///
    public class save_helper : plib.save_state_helper_interface
    {
        device_t m_device;
        string m_prefix;


        public save_helper(device_t dev, string prefix)
        {
            m_device = dev;
            m_prefix = prefix;
        }


        //template<typename T, typename X = void *>
        public void save_item<T>(T item, string name)  //void save_item(T &&item, const pstring &name, X = nullptr)
        {
            m_device.save_item(new Tuple<T, string>(item, (m_prefix + "_" + name)));  //m_device->save_item(item, (m_prefix + "_" + name).c_str());
        }


        //template <typename X = void *>
        //std::enable_if_t<plib::compile_info::has_int128::value && std::is_pointer<X>::value, void>
        //save_item(INT128 &item, const pstring &name, X = nullptr)
        //{
        //    auto *p = reinterpret_cast<std::uint64_t *>(&item);
        //    m_device->save_item(p[0], (m_prefix + "_" + name + "_1").c_str());
        //    m_device->save_item(p[1], (m_prefix + "_" + name + "_2").c_str());
        //}
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_stream_output_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_stream_output_device : device_t_with_netlist_mame_sub_interface
                                                     //device_t
                                                     //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_STREAM_OUTPUT, netlist_mame_stream_output_device, "nl_stream_out", "Netlist Stream Output")
        public static readonly emu.detail.device_type_impl NETLIST_STREAM_OUTPUT = DEFINE_DEVICE_TYPE("nl_stream_out", "Netlist Stream Output", (type, mconfig, tag, owner, clock) => { return new netlist_mame_stream_output_device(mconfig, tag, owner, clock); });


        uint32_t m_channel;
        string m_out_name;
        std.vector<stream_buffer_sample_t> m_buffer = new std.vector<stream_buffer_sample_t>();
        double m_cur;

        netlist_time m_sample_time;
        netlist_time_ext m_last_buffer_time;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_stream_output_t
        // ----------------------------------------------------------------------------------------

        netlist_mame_stream_output_device(machine_config mconfig, string tag, device_t owner, int channel, string out_name)
            : this(mconfig, tag, owner, 0)
        {
            set_params(channel, out_name);
        }

        netlist_mame_stream_output_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, NETLIST_STREAM_OUTPUT, tag, owner, clock)
        {
            m_channel = 0;
            m_out_name = "";
            m_cur = 0.0;
            m_sample_time = netlist_time.from_hz(1);
            m_last_buffer_time = netlist_time_ext.zero();
        }


        public void set_params(int channel, string out_name)
        {
            m_out_name = out_name;
            m_channel = (uint32_t)channel;
            sound().register_stream_output(channel, this);
        }


        void process(netlist_time_ext tim, nl_fptype val)
        {
            throw new emu_unimplemented();
        }


        public void buffer_reset(netlist_time_ext upto)
        {
            m_last_buffer_time = upto;
            m_buffer.clear();
        }


        public void sound_update_fill(write_stream_view target)
        {
            if (target.samples() < m_buffer.size())
                osd_printf_warning("sound {0}: samples {1} less bufsize {2}\n", name(), target.samples(), m_buffer.size());

            int sampindex;
            for (sampindex = 0; sampindex < (int)m_buffer.size(); sampindex++)
                target.put(sampindex, m_buffer[sampindex]);

            if (sampindex < target.samples())
                target.fill((stream_buffer_sample_t)m_cur, sampindex);
        }


        public void set_sample_time(netlist_time t) { m_sample_time = t; }


        // device-level overrides
        protected override void device_start()
        {
            LOGDEVCALLS("start {0}\n", name());
            m_cur = 0.0;
            m_last_buffer_time = netlist_time_ext.zero();

            save_item(NAME(new { m_cur }));
            m_sample_time.save_state(new save_helper(this, "m_sample_time"));
            m_last_buffer_time.save_state(new save_helper(this, "m_last_buffer_time"));
        }


        protected override void device_reset()
        {
            LOGDEVCALLS("reset {0}\n", name());
#if false //#if 0
            m_cur = 0.0;
            m_last_buffer_time = netlist_time_ext.zero();
#endif
        }


        public override void custom_netlist_additions(netlist.nlparse_t parser)
        {
            string dname = new plib.pfmt("STREAM_OUT_{0}").op(m_channel);

            parser.register_dev(dname, dname);
            parser.register_link(dname + ".IN", m_out_name);
        }


        public override void pre_parse_action(netlist.nlparse_t parser)
        {
            string dname = new plib.pfmt("STREAM_OUT_{0}").op(m_channel);

            netlist.interface_.nld_analog_callback.FUNC lambda = (in_, val) =>  //const auto lambda = [this](auto &in, netlist::nl_fptype val)
            {
                this.process(in_.exec().time(), val);
            };

            //using lb_t = decltype(lambda);
            //using cb_t = netlist::interface::NETLIB_NAME(analog_callback)<lb_t>;

            parser.factory_().add_nld_analog_callback<netlist.interface_.nld_analog_callback, nl_fptype, netlist.interface_.nld_analog_callback.FUNC>(dname, new netlist.factory.properties("-", plib.pg.PSOURCELOC()), 1e-9, lambda);  //parser.factory().add<cb_t, netlist::nl_fptype, lb_t>(dname, netlist::factory::properties("-", PSOURCELOC()), 1e-9, std::forward<lb_t>(lambda));
        }
    }


    // ----------------------------------------------------------------------------------------
    // Extensions to interface netlist with MAME code ....
    // ----------------------------------------------------------------------------------------

    /*! Specific exception if memregion is not available.
     *  The exception is thrown if the memregions are not available.
     *  This may be the case in device_validity_check and needs
     *  to be ignored.
     */
    class memregion_not_set : netlist.nl_exception
    {
        public memregion_not_set(string text) : base(text) { }
        public memregion_not_set(string format, params object [] args) : base(format, args) { }
    }


    class netlist_source_memregion_t : netlist.source_netlist_t
    {
        device_t m_dev;
        string m_name;


        public netlist_source_memregion_t(device_t dev, string name)
            : base()
        {
            m_dev = dev;
            m_name = name;
        }


        public override plib.istream_uptr stream(string name)
        {
            if (m_dev.has_running_machine())
            {
                memory_region mem = m_dev.memregion(m_name);
                plib.istream_uptr ret = new plib.istream_uptr(new MemoryStream(mem.base_().ToArray()), name);  //plib::istream_uptr ret(std::make_unique<std::istringstream>(putf8string(reinterpret_cast<char *>(mem->base()), mem->bytes())), name);
                //ret->imbue(std::locale::classic());
                return ret;
            }
            else
            {
                throw new memregion_not_set("memregion unavailable for {0} in source {1}", name, m_name);
                //return plib::unique_ptr<plib::pimemstream>(nullptr);
            }
        }
    }


    class netlist_data_memregions_t : netlist.source_data_t
    {
        device_t m_dev;


        public netlist_data_memregions_t(device_t dev) : base()
        {
            m_dev = dev;
        }


        public override plib.istream_uptr stream(string name)
        {
            throw new emu_unimplemented();
        }
    }


    // ----------------------------------------------------------------------------------------
    // sound_in
    // ----------------------------------------------------------------------------------------

    public static class nld_sound_in_helper_global
    {
        public static void init()
        {
            netlist.factory.nld_sound_in_helper.is_nld_sound_in = (typeof_C) =>
            {
                return typeof_C == typeof(nld_sound_in);
            };

            netlist.factory.nld_sound_in_helper.new_nld_sound_in = (anetlist, name) =>
            {
                return new nld_sound_in(anetlist, name);
            };

            netlist.factory.nld_sound_in_helper.new_device_element_t_nld_sound_in = (name, props) =>
            {
                return new netlist.factory.device_element_t<nld_sound_in>(name, props);
            };
        }
    }


    //using sound_in_type = netlist::interface::NETLIB_NAME(buffered_param_setter)<netlist_mame_sound_input_buffer>;


    //class NETLIB_NAME(sound_in) : public sound_in_type
    class nld_sound_in : sound_in_type
    {
        //using base_type = sound_in_type;
        //using base_type::base_type;


        public nld_sound_in(object owner, string name)
            : base(owner, name)
        { }
    }


    static partial class netlist_global
    {
        public static netlist_mame_cpu_device NETLIST_CPU(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<netlist_mame_cpu_device>(mconfig, tag, netlist_mame_cpu_device.NETLIST_CPU, clock); }
        public static netlist_mame_sound_device NETLIST_SOUND(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<netlist_mame_sound_device>(mconfig, tag, netlist_mame_sound_device.NETLIST_SOUND, clock); }
        public static netlist_mame_sound_device NETLIST_SOUND(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<netlist_mame_sound_device>(mconfig, tag, netlist_mame_sound_device.NETLIST_SOUND, clock); }
        public static netlist_mame_analog_input_device NETLIST_ANALOG_INPUT(machine_config mconfig, string tag, string param_name)
        {
            var device = emu.detail.device_type_impl.op<netlist_mame_analog_input_device>(mconfig, tag, netlist_mame_analog_input_device.NETLIST_ANALOG_INPUT, 0);
            device.set_name(param_name);
            return device;
        }
        public static netlist_mame_analog_output_device NETLIST_ANALOG_OUTPUT(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<netlist_mame_analog_output_device>(mconfig, tag, netlist_mame_analog_output_device.NETLIST_ANALOG_OUTPUT, clock); }
        public static netlist_mame_logic_output_device NETLIST_LOGIC_OUTPUT(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<netlist_mame_logic_output_device>(mconfig, tag, netlist_mame_logic_output_device.NETLIST_LOGIC_OUTPUT, clock); }
        public static netlist_mame_logic_input_device NETLIST_LOGIC_INPUT(machine_config mconfig, string tag, string param_name, uint32_t shift)
        {
            var device = emu.detail.device_type_impl.op<netlist_mame_logic_input_device>(mconfig, tag, netlist_mame_logic_input_device.NETLIST_LOGIC_INPUT, 0);
            device.set_params(param_name, shift);
            return device;
        }
        public static netlist_mame_stream_input_device NETLIST_STREAM_INPUT(machine_config mconfig, string tag, int channel, string param_name)
        {
            var device = emu.detail.device_type_impl.op<netlist_mame_stream_input_device>(mconfig, tag, netlist_mame_stream_input_device.NETLIST_STREAM_INPUT, 0);
            device.set_params(channel, param_name);
            return device;
        }
        public static netlist_mame_stream_output_device NETLIST_STREAM_OUTPUT(machine_config mconfig, string tag, int channel, string out_name)
        {
            var device = emu.detail.device_type_impl.op<netlist_mame_stream_output_device>(mconfig, tag, netlist_mame_stream_output_device.NETLIST_STREAM_OUTPUT, 0);
            device.set_params(channel, out_name);
            return device;
        }
    }
}
