// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_timer_id = System.UInt32;
using int64_t = System.Int64;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<NL_PREFER_INT128 && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using offs_t = System.UInt32;
using param_logic_t = mame.netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool>;  //using param_logic_t = param_num_t<bool>;
using plog_delegate = System.Action<mame.plib.plog_level, string>;  //using plog_delegate = plib::pmfp<void, plog_level, const pstring &>;
using size_t = System.UInt32;
using sound_in_type = mame.netlist.interface_.nld_buffered_param_setter<mame.netlist_mame_sound_input_buffer>;  //using sound_in_type = netlist::interface::NETLIB_NAME(buffered_param_setter)<netlist_mame_sound_input_buffer>;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using u32 = System.UInt32;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;


namespace mame
{
    public delegate void func_type(netlist.nlparse_t setup);  //using func_type = std::function<void(netlist::nlparse_t &)>;


    public static class netlist_global
    {
        //define NETLIST_LOGIC_PORT_CHANGED(_base, _tag)                                                 PORT_CHANGED_MEMBER(_base ":" _tag, netlist_mame_logic_input_t, input_changed, 0)
        //define NETLIST_ANALOG_PORT_CHANGED(_base, _tag)                                                PORT_CHANGED_MEMBER(_base ":" _tag, netlist_mame_analog_input_t, input_changed, 0)


        //#define MEMREGION_SOURCE(_name)                 setup.register_source(palloc(netlist_source_memregion_t(_name)));

        //define NETDEV_ANALOG_CALLBACK_MEMBER(_name)             void _name(const double data, const attotime &time)


        // netlib #defines this and it fights with logmacro.h
        //#undef LOG

        const int LOG_GENERAL     = 1 << 0;
        const int LOG_DEV_CALLS   = 1 << 1;
        const int LOG_DEBUG       = 1 << 2;
        const int LOG_TIMING      = 1 << 3;

        //#define LOG_MASK (LOG_GENERAL | LOG_DEV_CALLS | LOG_DEBUG)
        //#define LOG_MASK        (LOG_TIMING)
        //#define LOG_MASK        (0)

        public static void LOGDEVCALLS(device_t device, string format, params object [] args) { logmacro_global.LOGMASKED(LOG_DEV_CALLS, device, format, args); }  //#define LOGDEVCALLS(...) LOGMASKED(LOG_DEV_CALLS, __VA_ARGS__)
        public static void LOGDEBUG(device_t device, string format, params object [] args) { logmacro_global.LOGMASKED(LOG_DEBUG, device, format, args); }  //#define LOGDEBUG(...) LOGMASKED(LOG_DEBUG, __VA_ARGS__)
        public static void LOGTIMING(device_t device, string format, params object [] args) { logmacro_global.LOGMASKED(LOG_TIMING, device, format, args); }  //#define LOGTIMING(...) LOGMASKED(LOG_TIMING, __VA_ARGS__)

        //#define LOG_OUTPUT_FUNC printf

        //#define LOGMASKED(mask, ...) do { if (LOG_MASK & (mask)) (LOG_OUTPUT_FUNC)(__VA_ARGS__); } while (false)


        public static netlist_time_ext nltime_from_attotime(attotime t)
        {
            netlist_time_ext nlmtime = netlist_time_ext.from_sec(t.seconds());
            nlmtime += netlist_time_ext.from_raw(t.attoseconds() / (attotime.ATTOSECONDS_PER_SECOND / netlist_time_ext.resolution()));
            return nlmtime;
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NETLIST_CORE,  netlist_mame_device,       "netlist_core",  "Netlist Core Device")
        static device_t device_creator_netlist_mame_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_CORE = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_device, "netlist_core",  "Netlist Core Device");


        //using func_type = std::function<void(netlist::nlparse_t &)>;


        // ----------------------------------------------------------------------------------------
        // Special netlist extension devices  ....
        // ----------------------------------------------------------------------------------------

        public class netlist_mame_t : netlist.netlist_state_t
        {
            netlist_mame_device m_parent;


            public netlist_mame_t(netlist_mame_device parent, string name)
                : base(name)  //: netlist::netlist_state_t(name, plib::plog_delegate(&netlist_mame_t::logger, this))
            {
                m_parent = parent;

                netlist_state_t_after_ctor(logger);
            }


            //running_machine &machine() { return m_parent.machine(); }
            //netlist_mame_device &parent() const { return m_parent; }


            void logger(plib.plog_level l, string ls)
            {
                switch (l)
                {
                case plib.plog_level.DEBUG:
                    m_parent.logerror("netlist DEBUG: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.VERBOSE:
                    m_parent.logerror("netlist VERBOSE: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.INFO:
                    m_parent.logerror("netlist INFO: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.WARNING:
                    m_parent.logerror("netlist WARNING: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.ERROR:
                    m_parent.logerror("netlist ERROR: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.FATAL:
                    m_parent.logerror("netlist FATAL: {0}\n", ls.c_str());
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


        //static void register_memregion_source(netlist::nlparse_t &parser, device_t &dev, const char *name);


        // Custom to netlist ...
        protected virtual void nl_register_devices(netlist.nlparse_t parser) { }


        // device_t overrides

        protected override void device_config_complete()
        {
            netlist_global.LOGDEVCALLS(this, "device_config_complete {0}\n", this.mconfig().gamedrv().name);
        }


        protected override void device_validity_check(validity_checker valid)
        {
            base_validity_check(valid);
            //rom_exists(mconfig().root_device());
            netlist_global.LOGDEVCALLS(this, "device_validity_check {0}\n", this.mconfig().gamedrv().name);
        }


        protected override void device_start()
        {
            netlist_global.LOGDEVCALLS(this, "device_start entry\n");

            device_start_common();
            save_state();

            netlist_global.LOGDEVCALLS(this, "device_start exit\n");
        }


        protected override void device_stop()
        {
            netlist_global.LOGDEVCALLS(this, "device_stop\n");

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
            netlist_global.LOGDEVCALLS(this, "device_reset\n");
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
            netlist_global.LOGDEVCALLS(this, "device_post_load\n");

            netlist().run_state_manager().post_load();
            netlist().rebuild_lists();
        }


        protected override void device_pre_save()
        {
            netlist_global.LOGDEVCALLS(this, "device_pre_save\n");

            netlist().run_state_manager().pre_save();
        }


        protected void device_start_common()
        {
            m_netlist = new netlist_mame_t(this, "netlist");  //m_netlist = std::make_unique<netlist_mame_t>(*this, "netlist");

            m_netlist.set_static_solver_lib(new plib.dynlib_static(static_solvers_global.nl_static_solver_syms));  //m_netlist->set_static_solver_lib(std::make_unique<plib::dynlib_static>(nl_static_solver_syms));

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

            netlist_global.LOGDEVCALLS(this, "device_start exit\n");
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

                var lnetlist = new netlist.netlist_state_t("netlist");                // enable validation mode  //auto lnetlist = std::make_unique<netlist::netlist_state_t>("netlist", plib::plog_delegate(&validity_logger::log, &logger));                // enable validation mode
                lnetlist.netlist_state_t_after_ctor(logger.log);

                lnetlist.set_static_solver_lib(new plib.dynlib_static(null));  //lnetlist->set_static_solver_lib(std::make_unique<plib::dynlib_static>(nullptr));

                common_dev_start(lnetlist);
                lnetlist.setup().prepare_to_run();

                foreach (device_t d in subdevices())
                {
                    device_t_with_netlist_mame_sub_interface sdev = (device_t_with_netlist_mame_sub_interface)d;
                    if (sdev != null)
                    {
                        netlist_global.LOGDEVCALLS(this, "Validity check on subdevice {0}/{1}\n", d.name(), d.shortname());
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
                    netlist_global.LOGDEVCALLS(this, "Preparse subdevice {0}/{1}\n", d.name(), d.shortname());
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
                    netlist_global.LOGDEVCALLS(this, "Found subdevice {0}/{1}\n", d.name(), d.shortname());
                    sdev.custom_netlist_additions(lsetup.parser());
                }
            }
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_cpu_device_t
    // ----------------------------------------------------------------------------------------
    class netlist_mame_cpu_device : netlist_mame_device
                                    //device_execute_interface,
                                    //device_state_interface,
                                    //device_disasm_interface,
                                    //device_memory_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_CPU,   netlist_mame_cpu_device,   "netlist_cpu",   "Netlist CPU Device")
        static device_t device_creator_netlist_mame_cpu_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_cpu_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_CPU = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_cpu_device, "netlist_cpu",   "Netlist CPU Device");


        //static constexpr const unsigned MDIV_SHIFT = 16;


        //int m_icount;
        //netlist::netlist_time_ext    m_div;
        //netlist::netlist_time_ext    m_rem;
        //netlist::netlist_time_ext    m_old;
        offs_t m_genPC;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_cpu_device_t
        // ----------------------------------------------------------------------------------------
        netlist_mame_cpu_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NETLIST_CPU, tag, owner, clock)
        {
            throw new emu_unimplemented();
        }


        //~netlist_mame_cpu_device() { }


        //offs_t genPC() const { return m_genPC; }


        //netlist_mame_cpu_device & set_source(void (*setup_func)(netlist::nlparse_t &))
        //template <typename T, typename F> netlist_mame_cpu_device & set_source(T *obj, F && f)


        //void update_icount(netlist::netlist_time_ext time) noexcept;
        //void check_mame_abort_slice() noexcept;

        //netlist::netlist_time_ext nltime_ext_from_clocks(unsigned c) const noexcept
        //{
        //    return (m_div * c).shr(MDIV_SHIFT);
        //}

        //netlist::netlist_time nltime_from_clocks(unsigned c) const noexcept
        //{
        //    return static_cast<netlist::netlist_time>((m_div * c).shr(MDIV_SHIFT));
        //}


        // netlist_mame_device
        //virtual void nl_register_devices(netlist::nlparse_t &parser) const override;


        // device_t overrides

        //protected virtual void device_start();


        //protected virtual void device_clock_changed();


        // device_execute_interface overrides

        //virtual UINT64 execute_clocks_to_cycles(UINT64 clocks) const;
        //virtual UINT64 execute_cycles_to_clocks(UINT64 cycles) const;

        //ATTR_HOT virtual void execute_run();


        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        // device_memory_interface overrides

        //address_space_config m_program_config;

#if false
        virtual const address_space_config *memory_space_config(address_spacenum spacenum = AS_0) const
        {
            return (AS_PROGRAM == spacenum) ? &m_program_config : nullptr;
        }
#endif


        //  device_state_interface overrides
        //virtual void state_string_export(const device_state_entry &entry, astring &string);
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
        static device_t device_creator_netlist_mame_sound_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_sound_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_SOUND = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_sound_device, "netlist_sound", "Netlist Sound Device");


        public class device_sound_interface_netlist_mame_sound : device_sound_interface
        {
            public device_sound_interface_netlist_mame_sound(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((netlist_mame_sound_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }
        }


        device_sound_interface_netlist_mame_sound m_disound;


        std.map<int, netlist_mame_stream_output_device> m_out = new std.map<int, netlist_mame_stream_output_device>();
        std.map<size_t, nld_sound_in> m_in;
        std.vector<netlist_mame_sound_input_buffer> m_inbuffer;
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
            netlist_global.LOGDEBUG(this, "before update\n");
            get_stream().update();

            if (machine().time() < m_last_update_to_current_time)
                netlist_global.LOGTIMING(this, "machine.time() decreased 2\n");

            m_last_update_to_current_time = machine().time();

            var mtime = netlist_global.nltime_from_attotime(machine().time());
            var cur = netlist().exec().time();

            if (mtime > cur)
            {
                //expected don't log
                //LOGTIMING("%f us\n", (mtime - cur).as_double() * 1000000.0);
                netlist().exec().process_queue(mtime - cur);
            }
            else if (mtime < cur)
            {
                netlist_global.LOGTIMING(this, "{0} : {1} us before machine time\n", this.name(), (cur - mtime).as_double() * 1000000.0);
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
            parser.factory_().add_nld_sound_in("NETDEV_SOUND_IN", new netlist.factory.properties("-", plib.pglobal.PSOURCELOC()));  //parser.factory_().add<nld_sound_in>("NETDEV_SOUND_IN", netlist.factory.properties("-", PSOURCELOC()));
        }


        // device_t overrides

        protected override void device_start()
        {
            netlist_global.LOGDEVCALLS(this, "sound device_start\n");

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
                if (outdev.first() < 0 || outdev.first() >= m_out.size())
                    fatalerror("illegal output channel number {0}", outdev.first());
                outdev.second().set_sample_time(netlist_time.from_hz(m_sound_clock));
                outdev.second().buffer_reset(netlist_time_ext.zero());
            }

            // Configure inputs

            m_in.clear();

            std.vector<nld_sound_in> indevs = netlist().get_device_list<nld_sound_in>();
            foreach (var e in indevs)
            {
                //old m_in = indevs[0];
                m_in.emplace(e.id(), e);
                var sample_time = netlist_time.from_raw((int64_t)netlist_global.nltime_from_attotime(m_attotime_per_clock).as_raw());  //const auto sample_time = netlist::netlist_time::from_raw(static_cast<netlist::netlist_time::internal_type>(nltime_from_attotime(m_attotime_per_clock).as_raw()));
                e.resolve_params(sample_time);
            }

            foreach (var e in m_in)
            {
                if (e.first() < 0 || e.first() >= m_in.size())
                    fatalerror("illegal input channel number {0}", e.first());
            }

            m_inbuffer.resize(m_in.size());

            /* initialize the stream(s) */
            m_stream = m_disound.stream_alloc(m_in.size(), m_out.size(), m_sound_clock, sound_stream_flags.STREAM_DISABLE_INPUT_RESAMPLING);

            netlist_global.LOGDEVCALLS(this, "sound device_start exit\n");
        }


        // device_sound_interface overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs)
        {
            foreach (var e in m_in)
            {
                var clock_period = inputs[e.first()].sample_period();
                var sample_time = netlist_time.from_raw((int64_t)netlist_global.nltime_from_attotime(clock_period).as_raw());  //auto sample_time = netlist::netlist_time::from_raw(static_cast<netlist::netlist_time::internal_type>(nltime_from_attotime(clock_period).as_raw()));
                m_inbuffer[e.first()] = new netlist_mame_sound_input_buffer(inputs[e.first()]);
                e.second().buffer_reset(sample_time, m_inbuffer[e.first()].samples(), m_inbuffer[e.first()]);
            }

            int samples = (int)outputs[0].samples();
            netlist_global.LOGDEBUG(this, "samples {0}\n", samples);

            // end_time() is the time at the END of the last sample we're generating
            // however, the sample value is the value at the START of that last sample,
            // so subtract one sample period so that we only process up to the minimum
            var nl_target_time = netlist_global.nltime_from_attotime(outputs[0].end_time() - outputs[0].sample_period());

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

            m_owner = owner is netlist_mame_device ? (netlist_mame_device)owner : null;
            m_sound = owner is netlist_mame_sound_device ? (netlist_mame_sound_device)owner : null;
            m_cpu = owner is netlist_mame_cpu_device ? (netlist_mame_cpu_device)owner : null;
        }

        //virtual ~netlist_mame_sub_interface() { }


        public virtual void custom_netlist_additions(netlist.nlparse_t parser) { }
        public virtual void pre_parse_action(netlist.nlparse_t parser) { }


        public virtual void validity_helper(validity_checker valid, netlist.netlist_state_t nlstate) { throw new emu_unimplemented(); }  // need to re-do device_t_with_mame_sub_interface.  Doesn't do inheritance right


        //inline netlist_mame_device &nl_owner() const { return *m_owner; }


        public void update_to_current_time() { if (m_sound != null) m_sound.update_to_current_time(); }


        public void set_mult_offset(double mult, double offset)
        {
            m_mult = mult;
            m_offset = offset;
        }


        public netlist_mame_sound_device sound() { return m_sound;}
        //netlist_mame_cpu_device   *cpu()   { return m_cpu;}
    }


    public abstract class device_t_with_netlist_mame_sub_interface : device_t
    {
        protected netlist_mame_sub_interface m_netlist_mame_sub_interface;

        public device_t_with_netlist_mame_sub_interface(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, type, tag, owner, clock)
        {
            m_netlist_mame_sub_interface = new netlist_mame_sub_interface(owner);
        }


        public virtual void custom_netlist_additions(netlist.nlparse_t parser) { m_netlist_mame_sub_interface.custom_netlist_additions(parser); }
        public virtual void pre_parse_action(netlist.nlparse_t parser) { m_netlist_mame_sub_interface.pre_parse_action(parser); }

        public virtual void validity_helper(validity_checker valid, netlist.netlist_state_t nlstate) { m_netlist_mame_sub_interface.validity_helper(valid, nlstate); }

        public void set_mult_offset(double mult, double offset) { m_netlist_mame_sub_interface.set_mult_offset(mult, offset); }

        protected netlist_mame_sound_device sound() { return m_netlist_mame_sub_interface.sound();}
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_analog_input_device
    // ----------------------------------------------------------------------------------------
    class netlist_mame_analog_input_device : device_t_with_netlist_mame_sub_interface
                                             //device_t
                                             //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_ANALOG_INPUT,  netlist_mame_analog_input_device,  "nl_analog_in",  "Netlist Analog Input")
        static device_t device_creator_netlist_mame_analog_input_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_analog_input_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_ANALOG_INPUT = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_analog_input_device, "nl_analog_in",  "Netlist Analog Input");


        // TODO remove this when this class is finished
        public static void static_set_mult_offset(device_t device, double mult, double offset) { }


        //netlist::param_num_t<netlist::nl_fptype> *m_param;
        //bool   m_auto_port;
        //const char *m_param_name;
        //double m_value_for_device_timer;


        // construction/destruction
        netlist_mame_analog_input_device(machine_config mconfig, string tag, device_t owner, string param_name)
            : base(mconfig, NETLIST_ANALOG_INPUT, tag, owner, 0)
        {
            throw new emu_unimplemented();
        }

        netlist_mame_analog_input_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, NETLIST_ANALOG_INPUT, tag, owner, clock)
        {
            throw new emu_unimplemented();
        }


        //void set_name(const char *param_name) { m_param_name = param_name; }

        //inline void write(const double val)

#if false
        inline DECLARE_INPUT_CHANGED_MEMBER(input_changed)
        {
            if (m_auto_port)
                write((double(newval) - double(field.minval())) / double(field.maxval() - field.minval()));
            else
                write(newval);
        }
#endif

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
            throw new emu_unimplemented();
        }


        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
        {
            throw new emu_unimplemented();
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_analog_output_device
    // ----------------------------------------------------------------------------------------
    //class netlist_mame_analog_output_device : public device_t, public netlist_mame_sub_interface


    // ----------------------------------------------------------------------------------------
    // netlist_mame_logic_output_device
    // ----------------------------------------------------------------------------------------
    //class netlist_mame_logic_output_device : public device_t, public netlist_mame_sub_interface


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
        static device_t device_creator_netlist_mame_logic_input_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_logic_input_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_LOGIC_INPUT = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_logic_input_device, "nl_logic_in",  "Netlist Logic Input");


        netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool> m_param;
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
                netlist_global.LOGDEBUG(this, "write {0}\n", this.tag());
                synchronize(0, (int)v);
            }
        }


        //inline DECLARE_INPUT_CHANGED_MEMBER(input_changed) { write(newval); }

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
            netlist_global.LOGDEVCALLS(this, "start\n");

            netlist.param_ref_t p = ((netlist_mame_device)this.owner()).setup().find_param(m_param_name);  //netlist::param_ref_t p = downcast<netlist_mame_device *>(this->owner())->setup().find_param(pstring(m_param_name));
            m_param = (param_logic_t)p.param();  //m_param = dynamic_cast<netlist::param_logic_t *>(&p.param());
            if (m_param == null)
            {
                fatalerror("device {0} wrong parameter type for {1}\n", basetag(), m_param_name);
            }
        }

        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
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
        static device_t device_creator_netlist_mame_stream_input_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_stream_input_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_STREAM_INPUT = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_stream_input_device, "nl_stream_in",  "Netlist Stream Input");


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
            netlist_global.LOGDEVCALLS(this, "start\n");
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
    public struct save_helper
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
            m_device.save_item(new Tuple<T, string>(item, (m_prefix + "_" + name).c_str()));  //m_device->save_item(item, (m_prefix + "_" + name).c_str());
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
        static device_t device_creator_netlist_mame_stream_output_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_stream_output_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_STREAM_OUTPUT = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_stream_output_device, "nl_stream_out", "Netlist Stream Output");


        uint32_t m_channel;
        string m_out_name;
        std.vector<stream_buffer_sample_t> m_buffer;
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


        //void process(netlist::netlist_time_ext tim, netlist::nl_fptype val);


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
            for (sampindex = 0; sampindex < m_buffer.size(); sampindex++)
                target.put(sampindex, m_buffer[sampindex]);

            target.fill((stream_buffer_sample_t)m_cur, sampindex);
        }


        public void set_sample_time(netlist_time t) { m_sample_time = t; }


        // device-level overrides
        protected override void device_start()
        {
            netlist_global.LOGDEVCALLS(this, "start {0}\n", name());
            m_cur = 0.0;
            m_last_buffer_time = netlist_time_ext.zero();

            save_item(NAME(m_cur));
            m_sample_time.save_state(new save_helper(this, "m_sample_time"));
            m_last_buffer_time.save_state(new save_helper(this, "m_last_buffer_time"));
        }


        protected override void device_reset()
        {
            netlist_global.LOGDEVCALLS(this, "reset {0}\n", name());
#if false
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

            //const auto lambda = [this](auto &in_, netlist::nl_fptype val)
            //{
            //    this->process(in_.exec().time(), val);;
            //};

            //using lb_t = decltype(lambda);
            //using cb_t = netlist::interface::NETLIB_NAME(analog_callback)<lb_t>;

            parser.factory_().add_nld_analog_callback(dname, new netlist.factory.properties("-", plib.pglobal.PSOURCELOC()));  //parser.factory().add<cb_t, netlist::nl_fptype, lb_t>(dname, netlist::factory::properties("-", PSOURCELOC()), 1e-9, std::forward<lb_t>(lambda));
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
        memregion_not_set(string text) : base(text) { }
        memregion_not_set(string format, params object [] args) : base(format, args) { }
    }


    //class netlist_source_memregion_t : public netlist::source_t


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

    //using sound_in_type = netlist::interface::NETLIB_NAME(buffered_param_setter)<netlist_mame_sound_input_buffer>;


    //class NETLIB_NAME(sound_in) : public sound_in_type
    class nld_sound_in : sound_in_type
    {
        //using base_type = sound_in_type;
        //using base_type::base_type;


        protected nld_sound_in(object owner, string name)
            : base(owner, name)
        { }
    }
}
