// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_timer_id = System.UInt32;
using int64_t = System.Int64;
using netlist_time = mame.plib.ptime_i64;  //using netlist_time = plib::ptime<std::int64_t, NETLIST_INTERNAL_RES>;
using netlist_time_ext = mame.plib.ptime_i64;  //netlist_time
using nl_fptype = System.Double;
using offs_t = System.UInt32;
using size_t = System.UInt32;
using stream_sample_t = System.Int32;
using u32 = System.UInt32;
using uint32_t = System.UInt32;


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

        //#define LOG_MASK (LOG_GENERAL | LOG_DEV_CALLS | LOG_DEBUG)
        //#define LOG_MASK        (0)

        public static void LOGDEVCALLS(device_t device, string format, params object [] args) { logmacro_global.LOGMASKED(LOG_DEV_CALLS, device, format, args); }  //#define LOGDEVCALLS(...) LOGMASKED(LOG_DEV_CALLS, __VA_ARGS__)
        public static void LOGDEBUG(device_t device, string format, params object [] args) { logmacro_global.LOGMASKED(LOG_DEBUG, device, format, args); }  //#define LOGDEBUG(...) LOGMASKED(LOG_DEBUG, __VA_ARGS__)

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

        class netlist_mame_callbacks_t : netlist.callbacks_t
        {
            netlist_mame_device m_parent;


            public netlist_mame_callbacks_t(netlist_mame_device parent) : base() { m_parent = parent; }


            public override void vlog(plib.plog_level l, string ls)
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


            public override plib.dynlib_base static_solver_lib()
            {
                //return plib::make_unique<plib::dynlib_static>(nullptr);
                return new plib.dynlib_static(static_solvers_global.nl_static_solver_syms);  //return plib::make_unique<plib::dynlib_static>(nl_static_solver_syms);
            }
        }


        class netlist_validate_callbacks_t : netlist.callbacks_t
        {
            public netlist_validate_callbacks_t() : base() { }


            public override void vlog(plib.plog_level l, string ls)
            {
                switch (l)
                {
                case plib.plog_level.DEBUG:
                    break;
                case plib.plog_level.VERBOSE:
                    break;
                case plib.plog_level.INFO:
                    osd_printf_verbose("netlist INFO: {0}\n", ls);
                    break;
                case plib.plog_level.WARNING:
                    osd_printf_warning("netlist WARNING: {0}\n", ls);
                    break;
                case plib.plog_level.ERROR:
                    osd_printf_error("netlist ERROR: {0}\n", ls);
                    break;
                case plib.plog_level.FATAL:
                    osd_printf_error("netlist FATAL: {0}\n", ls);
                    break;
                }
            }


            public override plib.dynlib_base static_solver_lib() { throw new emu_unimplemented(); }
        }


        protected class netlist_mame_t : netlist.netlist_state_t
        {
            netlist_mame_device m_parent;


            public netlist_mame_t(netlist_mame_device parent, string name)
                : base(name, new netlist_mame_device.netlist_mame_callbacks_t(parent))
            {
                m_parent = parent;
            }

            netlist_mame_t(netlist_mame_device parent, string name, netlist.callbacks_t cbs)
                : base(name, cbs)
            {
                m_parent = parent;
            }


            running_machine machine() { return m_parent.machine(); }

            netlist_mame_device parent() { return m_parent; }
        }


        const int MDIV_SHIFT = 16;


        int m_icount;

        protected attotime m_cur_time;
        protected attotime m_attotime_per_clock;

        netlist_time_ext m_div = new netlist_time();
        netlist_time_ext m_rem;
        netlist_time_ext m_old;

        netlist_mame_t m_netlist;  //std::unique_ptr<netlist_mame_t> m_netlist;

        func_type m_setup_func;


        // construction/destruction
        public netlist_mame_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, NETLIST_CORE, tag, owner, clock)
        {
        }

        public netlist_mame_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_icount = 0;
            m_cur_time = attotime.zero;
            m_attotime_per_clock = attotime.zero;
            m_old = netlist_time_ext.zero();
            m_setup_func = null;
        }

        //virtual ~netlist_mame_device()
        //{
        //    netlist_global.LOGDEVCALLS(this, "~netlist_mame_device\n");
        //}


        public void set_setup_func(func_type func) { m_setup_func = func; }  //std::move(func); }


        public netlist.setup_t setup()
        {
            if (m_netlist == null)
                throw new device_missing_dependencies();

            return m_netlist.setup();
        }


        protected netlist_mame_t netlist() { return m_netlist; }


        //void update_icount(netlist::netlist_time_ext time) noexcept;
        //void check_mame_abort_slice() noexcept;


        //static void register_memregion_source(netlist::nlparse_t &parser, device_t &dev, const char *name);


        protected netlist_time_ext nltime_ext_from_clocks(UInt32 c)
        {
            return (m_div * c).shr(MDIV_SHIFT);
        }


        protected netlist_time nltime_from_clocks(UInt32 c)
        {
            return (netlist_time)((m_div * c).shr(MDIV_SHIFT));
        }


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

            m_attotime_per_clock = new attotime(0, m_attoseconds_per_clock);

            //netlist().save(*this, m_cur_time, pstring(this->name()), "m_cur_time");
            save_item(NAME(new { m_cur_time }));
            save_item(NAME(new { m_attotime_per_clock }));

            m_netlist = new netlist_mame_t(this, "netlist");  //m_netlist = std::make_unique<netlist_mame_t>(*this, "netlist");

            if (!machine().options().verbose())
            {
                m_netlist.log().verbose.set_enabled(false);
                m_netlist.log().debug.set_enabled(false);
            }

            common_dev_start(m_netlist);
            m_netlist.setup().prepare_to_run();

            // FIXME: use save_helper
            m_netlist.save(this, m_rem, this.name(), "m_rem");
            m_netlist.save(this, m_div, this.name(), "m_div");
            m_netlist.save(this, m_old, this.name(), "m_old");

            save_state();

            m_old = netlist_time_ext.zero();
            m_rem = netlist_time_ext.zero();

            netlist_global.LOGDEVCALLS(this, "device_start exit\n");
        }


        protected override void device_stop()
        {
            netlist_global.LOGDEVCALLS(this, "device_stop\n");

            netlist().exec().stop();
        }


        protected override void device_reset()
        {
            netlist_global.LOGDEVCALLS(this, "device_reset\n");

            m_cur_time = attotime.zero;
            m_old = netlist_time_ext.zero();
            m_rem = netlist_time_ext.zero();
            netlist().exec().reset();
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


        protected override void device_clock_changed()
        {
            m_div = new netlist_time_ext(
                (netlist_time_ext.resolution() << MDIV_SHIFT) / clock());
            //printf("m_div %d\n", (int) m_div.as_raw());
            netlist().log().debug.op("Setting clock {0} and divisor {1}\n", clock(), m_div.as_double());
            m_attotime_per_clock = new attotime(0, m_attoseconds_per_clock);
        }


        protected netlist.netlist_state_t base_validity_check(validity_checker valid)
        {
            try
            {
                var lnetlist = new netlist.netlist_state_t("netlist", new netlist_validate_callbacks_t());  //auto lnetlist = plib::make_unique<netlist::netlist_state_t, netlist::host_arena>("netlist", plib::make_unique<netlist_validate_callbacks_t, netlist::host_arena>());
                // enable validation mode
                lnetlist.set_extended_validation(true);
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


        void save_state()
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


        // netlist_mame_device
        //virtual void nl_register_devices(netlist::nlparse_t &parser) const override;


        // device_t overrides

        // Fixes overflow error in device_pseudo_state_register
        //template<>
        //class device_pseudo_state_register<double> : public device_state_entry

        //virtual void device_start();


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

            public override void sound_stream_update(sound_stream stream, Pointer<stream_sample_t> [] inputs, Pointer<stream_sample_t> [] outputs, int samples) { ((netlist_mame_sound_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs, samples); }
        }


        device_sound_interface_netlist_mame_sound m_disound;
        static attotime last;


        std.map<int, netlist_mame_stream_output_device> m_out = new std.map<int, netlist_mame_stream_output_device>();
        nld_sound_in m_in;
        sound_stream m_stream;
        bool m_is_device_call;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_sound_device_t
        // ----------------------------------------------------------------------------------------
        netlist_mame_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, NETLIST_SOUND, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_netlist_mame_sound(mconfig, this));  //device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_netlist_mame_sound>();

            m_in = null;
            m_stream = null;
            m_is_device_call = false;
        }


        public device_sound_interface_netlist_mame_sound disound { get { return m_disound; } }


        //netlist_mame_sound_device & set_source(void (*setup_func)(netlist::nlparse_t &))
        //template <typename T, typename F> netlist_mame_sound_device & set_source(T *obj, F && f)
        public netlist_mame_sound_device set_source(func_type setup_func) { set_setup_func(setup_func); return this; }


        public sound_stream get_stream() { return m_stream; }


        public void update_to_current_time()
        {
            netlist_global.LOGDEBUG(this, "before update\n");
            m_is_device_call = true;
            get_stream().update();
            m_is_device_call = false;

            if (machine().time() < last)
                netlist_global.LOGDEBUG(this, "machine.time() decreased 2\n");

            last = machine().time();

            var mtime = netlist_global.nltime_from_attotime(machine().time());
            var cur = netlist().exec().time();

            if (mtime > cur)
            {
                if ((mtime - cur) >= nltime_from_clocks(1))
                    netlist_global.LOGDEBUG(this, "{0} us\n", (mtime - cur).as_double() * 1000000.0);

                netlist().exec().process_queue(mtime - cur);
            }
            else if (mtime < cur)
            {
                netlist_global.LOGDEBUG(this, "{0} : {1} ns before machine time\n", this.name(), (cur - mtime).as_double() * 1000000000.0);
            }
        }


        // device_sound_interface overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, Pointer<stream_sample_t> [] inputs, Pointer<stream_sample_t> [] outputs, int samples)
        {
            if (machine().time() < last)
                netlist_global.LOGDEBUG(this, "machine.time() decreased 1\n");

            last = machine().time();
            netlist_global.LOGDEBUG(this, "samples {0} {1}\n", m_is_device_call, samples);

            if (m_in != null)
            {
                var sample_time = netlist_time.from_raw((int64_t)netlist_global.nltime_from_attotime(m_attotime_per_clock).as_raw());  //auto sample_time = netlist::netlist_time::from_raw(static_cast<netlist::netlist_time::internal_type>(nltime_from_attotime(m_attotime_per_clock).as_raw()));
                m_in.buffer_reset(sample_time, (size_t)samples, inputs);
            }

            m_cur_time += ((UInt32)samples * m_attotime_per_clock);
            var nl_target_time = netlist_global.nltime_from_attotime(m_cur_time);

            if (!m_is_device_call)
                nl_target_time -= netlist_time_ext.from_usec(2); // FIXME make adjustment a parameter

            var nltime = (netlist().exec().time());

            if (nltime < nl_target_time)
            {
                netlist().exec().process_queue(nl_target_time - nltime);
            }

            foreach (var e in m_out)
            {
                e.second().sound_update_fill((size_t)samples, outputs[e.first()]);
                e.second().buffer_reset(nl_target_time);
            }
        }


        protected override void device_validity_check(validity_checker valid)
        {
            throw new emu_unimplemented();
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
            base.device_start();

            netlist_global.LOGDEVCALLS(this, "sound device_start\n");

            // Configure outputs

            if (m_out.size() == 0)
                fatalerror("No output devices");

            //m_num_outputs = outdevs.size();

            /* resort channels */
            foreach (var outdev in m_out)
            {
                if (outdev.first() < 0 || outdev.first() >= m_out.size())
                    fatalerror("illegal channel number {0}", outdev.first());
                outdev.second().set_sample_time(netlist_time.from_hz(clock()));
                outdev.second().buffer_reset(netlist_time_ext.zero());
            }

            // Configure inputs
            // FIXME: The limitation to one input device seems artificial.
            //        We should allow multiple devices with one channel each.

            m_in = null;

            std.vector<nld_sound_in> indevs = netlist().get_device_list<nld_sound_in>();
            if (indevs.size() > 1)
                fatalerror("A maximum of one input device is allowed!");

            if (indevs.size() == 1)
            {
                m_in = indevs[0];
                var sample_time = netlist_time.from_raw((int64_t)netlist_global.nltime_from_attotime(clocks_to_attotime(1)).as_raw());  //auto sample_time = netlist.netlist_time.from_raw(static_cast<netlist.netlist_time.internal_type>(nltime_from_attotime(clocks_to_attotime(1)).as_raw()));
                m_in.resolve_params(sample_time);
            }

            /* initialize the stream(s) */
            m_is_device_call = false;
            m_stream = machine().sound().stream_alloc(this, m_in != null ? m_in.num_channels() : 0, m_out.size(), (int)clock());
        }


        protected override void device_reset()
        {
            base.device_reset();
        }


        protected override void device_clock_changed()
        {
            base.device_clock_changed();

            foreach (var e in m_out)
            {
                e.second().set_sample_time(nltime_from_clocks(1));
            }
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


        netlist.param_num_t_bool m_param;
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
            m_param = (netlist.param_logic_t)p.param();  //m_param = dynamic_cast<netlist::param_logic_t *>(&p.param());
            if (m_param == null)
            {
                fatalerror("device {0} wrong parameter type for {1}\n", basetag(), m_param_name);
            }
        }

        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
        {
            m_netlist_mame_sub_interface.update_to_current_time();
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
            if (!parser.device_exists("STREAM_INPUT"))
                parser.register_dev("NETDEV_SOUND_IN", "STREAM_INPUT");

            string sparam = new plib.pfmt("STREAM_INPUT.CHAN{0}").op(m_channel);
            parser.register_param(sparam, m_param_name);
            sparam = new plib.pfmt("STREAM_INPUT.MULT{0}").op(m_channel);
            parser.register_param_val(sparam, m_netlist_mame_sub_interface.m_mult);
            sparam = new plib.pfmt("STREAM_INPUT.OFFSET{0}").op(m_channel);
            parser.register_param_val(sparam, m_netlist_mame_sub_interface.m_offset);
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
        std.vector<stream_sample_t> m_buffer;
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


        public void sound_update_fill(size_t samples, Pointer<stream_sample_t> target)  //void sound_update_fill(std::size_t samples, stream_sample_t *target);
        {
            if (samples < m_buffer.size())
                throw new emu_fatalerror("sound {0}: samples {1} less bufsize {2}\n", name(), samples, m_buffer.size());

            std.memcpy(target, new Pointer<stream_sample_t>(m_buffer), (UInt32)m_buffer.Count);  //std::copy(m_buffer.begin(), m_buffer.end(), target);
            size_t pos = (size_t)m_buffer.size();
            while (pos < samples)
            {
                target[pos++] = (stream_sample_t)m_cur;
            }
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
            m_cur = 0.0;
            m_last_buffer_time = netlist_time_ext.zero();
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


        public override std.istream stream(string name)
        {
            throw new emu_unimplemented();
        }
    }


    // ----------------------------------------------------------------------------------------
    // sound_in
    // ----------------------------------------------------------------------------------------

    //using sound_in_type = netlist::interface::NETLIB_NAME(buffered_param_setter)<stream_sample_t, 16>;
    class sound_in_type : netlist.interface_.nld_buffered_param_setter_stream_sample_t//<stream_sample_t, 16>
    {
        protected sound_in_type(netlist.netlist_state_t anetlist, string name)
            : base(16, anetlist, name)
        { }
    }

    //class NETLIB_NAME(sound_in) : public sound_in_type
    class nld_sound_in : sound_in_type
    {
        //using base_type = sound_in_type;
        //using base_type::base_type;


        public nld_sound_in(netlist.netlist_state_t anetlist, string name)
            : base(anetlist, name)
        { }
    }
}
