// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_timer_id = System.UInt32;
using device_type = mame.emu.detail.device_type_impl_base;
using netlist_base_t = mame.netlist.netlist_state_t;
using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;
using nl_double = System.Double;
using stream_sample_t = System.Int32;
using offs_t = System.UInt32;
using u32 = System.UInt32;
using uint32_t = System.UInt32;


namespace mame
{
    public delegate void func_type(netlist.nlparse_t setup);  //using func_type = std::function<void(netlist::nlparse_t &)>;


    public static class netlist_global
    {
        public static void MCFG_NETLIST_SETUP(device_t device, func_type _setup) { ((netlist_mame_device)device).set_setup_func(_setup); }

#if false
        //define MCFG_NETLIST_SETUP_MEMBER(_obj, _setup) downcast<netlist_mame_device &>(*device).set_constructor(_obj, _setup);
        //define MCFG_NETLIST_ANALOG_INPUT(_basetag, _tag, _name)                                        MCFG_DEVICE_ADD(_basetag ":" _tag, NETLIST_ANALOG_INPUT, 0)                                 netlist_mame_analog_input_t::static_set_name(*device, _name);
        public static void MCFG_NETLIST_ANALOG_MULT_OFFSET(device_t device, double _mult, double _offset) { ((device_t_with_netlist_mame_sub_interface)device).set_mult_offset(_mult, _offset); }
        //define MCFG_NETLIST_ANALOG_OUTPUT(_basetag, _tag, _IN, _class, _member, _class_tag)             MCFG_DEVICE_ADD(_basetag ":" _tag, NETLIST_ANALOG_OUTPUT, 0)                                netlist_mame_analog_output_t::static_set_params(*device, _IN,                                           FUNC(_class :: _member), _class_tag);
        //define MCFG_NETLIST_LOGIC_INPUT(_basetag, _tag, _name, _shift, _mask)                          MCFG_DEVICE_ADD(_basetag ":" _tag, NETLIST_LOGIC_INPUT, 0)                                  netlist_mame_logic_input_t::static_set_params(*device, _name, _mask, _shift);
        public static void MCFG_NETLIST_STREAM_INPUT(out device_t device, machine_config config, device_t owner, string basetag, int chan, string name)
        {
            mconfig_global.MCFG_DEVICE_ADD(out device, config, owner, string.Format("{0}:cin{1}", basetag, chan), netlist_mame_stream_input_device.NETLIST_STREAM_INPUT, 0);
            ((netlist_mame_stream_input_device)device).set_params(chan, name);
        }
        public static void MCFG_NETLIST_STREAM_OUTPUT(out device_t device, machine_config config, device_t owner, string basetag, int chan, string name)
        {
            mconfig_global.MCFG_DEVICE_ADD(out device, config, owner, string.Format("{0}:cout{1}", basetag, chan), netlist_mame_stream_output_device.NETLIST_STREAM_OUTPUT, 0);
            ((netlist_mame_stream_output_device)device).set_params(chan, name);
        }
#endif

        //define NETLIST_LOGIC_PORT_CHANGED(_base, _tag)                                                 PORT_CHANGED_MEMBER(_base ":" _tag, netlist_mame_logic_input_t, input_changed, 0)
        //define NETLIST_ANALOG_PORT_CHANGED(_base, _tag)                                                PORT_CHANGED_MEMBER(_base ":" _tag, netlist_mame_analog_input_t, input_changed, 0)


        //#define MEMREGION_SOURCE(_name)                 setup.register_source(palloc(netlist_source_memregion_t(_name)));

        //define NETDEV_ANALOG_CALLBACK_MEMBER(_name)             void _name(const double data, const attotime &time)


        // netlib #defines this and it fights with logmacro.h
        //#undef LOG

        const int LOG_GENERAL     = 1 << 0;
        const int LOG_DEV_CALLS   = 1 << 1;

        //#define VERBOSE (LOG_GENERAL | LOG_DEV_CALLS)
        //#define LOG_OUTPUT_FUNC printf
        //#include "logmacro.h"

        public static void LOGDEVCALLS(device_t device, string format, params object [] args) { logmacro_global.LOGMASKED(LOG_DEV_CALLS, device, format, args); }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_device_t
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NETLIST_CORE,  netlist_mame_device,       "netlist_core",  "Netlist Core Device")
        static device_t device_creator_netlist_mame_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_device(mconfig, tag, owner, clock); }
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
                    throw new emu_fatalerror(1, "netlist ERROR: {0}\n", ls.c_str());
                }
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
                    osd_printf_verbose("netlist INFO: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.WARNING:
                    osd_printf_warning("netlist WARNING: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.ERROR:
                    osd_printf_error("netlist ERROR: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.FATAL:
                    throw new emu_fatalerror(1, "netlist ERROR: {0}\n", ls.c_str());
                }
            }
        }


        protected class netlist_mame_t : netlist.netlist_t
        {
            netlist_mame_device m_parent;


            public netlist_mame_t(netlist_mame_device parent, string aname)
                : base(aname, new netlist_mame_device.netlist_mame_callbacks_t(parent))
            {
                m_parent = parent;
            }

            netlist_mame_t(netlist_mame_device parent, string aname, netlist.callbacks_t cbs)
                : base(aname, cbs)
            {
                m_parent = parent;
            }


            running_machine machine() { return m_parent.machine(); }

            netlist_mame_device parent() { return m_parent; }
        }


        int m_icount;

        netlist_time m_div;


        /* timing support here - so sound can hijack it ... */
        netlist_time m_rem;
        netlist_time m_old;

        netlist_mame_t m_netlist;  //netlist::poolptr<netlist_mame_t> m_netlist;

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
            m_old = netlist_time.zero();
            m_setup_func = null;
        }

        //virtual ~netlist_mame_device()
        //{
        //    netlist_global.LOGDEVCALLS(this, "~netlist_mame_device\n");
        //}


        public void set_setup_func(func_type func) { m_setup_func = func; }  //std::move(func); }


        protected netlist_time div { get { return m_div; } }


        public netlist.setup_t setup()
        {
            if (m_netlist == null)
                throw new device_missing_dependencies();

            return m_netlist.nlstate().setup();
        }


        protected netlist_mame_t netlist() { return m_netlist; }


        //ATTR_HOT void update_icount(netlist::netlist_time time);
        //ATTR_HOT void check_mame_abort_slice();


        //static void register_memregion_source(netlist::nlparse_t &setup, device_t &dev, const char *name);


        // Custom to netlist ...
        protected virtual void nl_register_devices(netlist.setup_t lsetup) { }


        // device_t overrides

        protected override void device_config_complete()
        {
            netlist_global.LOGDEVCALLS(this, "device_config_complete {0}\n", this.mconfig().gamedrv().name);
        }


        protected override void device_validity_check(validity_checker valid)
        {
#if true
            //rom_exists(mconfig().root_device());
            netlist_global.LOGDEVCALLS(this, "device_validity_check {0}\n", this.mconfig().gamedrv().name);

            try
            {
                //netlist_mame_t lnetlist(*this, "netlist", plib::make_unique<netlist_validate_callbacks_t>());
                netlist.netlist_t lnetlist = new netlist.netlist_t("netlist", new netlist_validate_callbacks_t());  //netlist::netlist_t lnetlist("netlist", plib::make_unique<netlist_validate_callbacks_t>());
                common_dev_start(lnetlist);
            }
            catch (memregion_not_set err)
            {
                osd_printf_verbose("{0}\n", err);
            }
            catch (emu_fatalerror err)
            {
                osd_printf_error("{0}\n", err.str());
            }
            catch (Exception err)
            {
                osd_printf_error("{0}\n", err);
            }
#endif
        }


        protected override void device_start()
        {
            netlist_global.LOGDEVCALLS(this, "device_start entry\n");

            m_netlist = new netlist_mame_t(this, "netlist");  //m_netlist = netlist::pool().make_poolptr<netlist_mame_t>(*this, "netlist");

            common_dev_start(m_netlist);

            m_netlist.nlstate().save(this, m_rem, this.name(), "m_rem");
            m_netlist.nlstate().save(this, m_div, this.name(), "m_div");
            m_netlist.nlstate().save(this, m_old, this.name(), "m_old");

            save_state();

            m_old = netlist_time.zero();
            m_rem = netlist_time.zero();

            netlist_global.LOGDEVCALLS(this, "device_start exit\n");
        }


        protected override void device_stop()
        {
            netlist_global.LOGDEVCALLS(this, "device_stop\n");

            netlist().stop();
        }


        protected override void device_reset()
        {
            netlist_global.LOGDEVCALLS(this, "device_reset\n");

            m_old = netlist_time.zero();
            m_rem = netlist_time.zero();
            netlist().reset();
        }


        protected override void device_post_load()
        {
            netlist_global.LOGDEVCALLS(this, "device_post_load\n");

            netlist().run_state_manager().post_load();
            netlist().nlstate().rebuild_lists();
        }


        protected override void device_pre_save()
        {
            netlist_global.LOGDEVCALLS(this, "device_pre_save\n");

            netlist().run_state_manager().pre_save();
        }


        protected override void device_clock_changed()
        {
            m_div = netlist_time.from_hz(clock());
            netlist().log().debug.op("Setting clock {0} and divisor {1}\n", clock(), m_div.as_double());
        }


        void save_state()
        {
            //throw new emu_unimplemented();
#if false
            for (auto const & s : netlist().run_state_manager().save_list())
            {
                netlist().log().debug.op("saving state for {0}\n", s.m_name.c_str());
                if (s.m_dt.is_float)
                {
                    if (s.m_dt.size == sizeof(double))
                        save_pointer((double *) s->m_ptr, s->m_name.c_str(), s->m_count);
                    else if (s.m_dt.size == sizeof(float))
                        save_pointer((float *) s->m_ptr, s->m_name.c_str(), s->m_count);
                    else
                        netlist().log().fatal.op("Unknown floating type for {0}\n", s.m_name.c_str());
                }
                else if (s.m_dt.is_integral)
                {
                    if (s.m_dt.size == sizeof(int64_t))
                        save_pointer((int64_t *) s->m_ptr, s->m_name.c_str(), s->m_count);
                    else if (s.m_dt.size == sizeof(int32_t))
                        save_pointer((int32_t *) s->m_ptr, s->m_name.c_str(), s->m_count);
                    else if (s->m_dt.size == sizeof(int16_t))
                        save_pointer((int16_t *) s->m_ptr, s->m_name.c_str(), s->m_count);
                    else if (s->m_dt.size == sizeof(int8_t))
                        save_pointer((int8_t *) s->m_ptr, s->m_name.c_str(), s->m_count);
#if (PHAS_INT128)
                    else if (s->m_dt.size == sizeof(INT128))
                        save_pointer((int64_t *) s->m_ptr, s->m_name.c_str(), s->m_count * 2);
#endif
                    else
                        netlist().log().fatal.op("Unknown integral type size {0} for {1}\n", s.m_dt.size, s.m_name.c_str());
                }
                else if (s.m_dt.is_custom)
                {
                    /* do nothing */
                }
                else
                {
                    netlist().log().fatal.op("found unsupported save element {0}\n", s.m_name);
                }
            }
#endif
        }


        void common_dev_start(netlist.netlist_t lnetlist)
        {
            var lsetup = lnetlist.nlstate().setup();

            // register additional devices

            nl_register_devices(lsetup);

            /* let sub-devices add sources and do stuff prior to parsing */
            foreach (device_t d in subdevices())
            {
                device_t_with_netlist_mame_sub_interface sdev = (device_t_with_netlist_mame_sub_interface)d;  //netlist_mame_sub_interface *sdev = dynamic_cast<netlist_mame_sub_interface *>(&d);
                if (sdev != null)
                {
                    netlist_global.LOGDEVCALLS(this, "Preparse subdevice {0}/{1}\n", d.name(), d.shortname());
                    sdev.pre_parse_action(lnetlist.nlstate());
                }
            }

            /* add default data provider for roms - if not in validity check*/
            //if (has_running_machine())
                lsetup.register_source(new netlist_data_memregions_t(this));

            m_setup_func(lsetup);

#if true
            /* let sub-devices tweak the netlist */
            foreach (device_t d in subdevices())
            {
                device_t_with_netlist_mame_sub_interface sdev = (device_t_with_netlist_mame_sub_interface)d;  //netlist_mame_sub_interface *sdev = dynamic_cast<netlist_mame_sub_interface *>(&d);
                if (sdev != null)
                {
                    netlist_global.LOGDEVCALLS(this, "Found subdevice {0}/{1}\n", d.name(), d.shortname());
                    sdev.custom_netlist_additions(lnetlist.nlstate());
                }
            }

            lsetup.prepare_to_run();
#endif
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
        static device_t device_creator_netlist_mame_cpu_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_cpu_device(mconfig, tag, owner, clock); }
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
        //virtual void nl_register_devices(netlist::setup_t &lsetup) override;


        // device_t overrides

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
        static device_t device_creator_netlist_mame_sound_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_sound_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_SOUND = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_sound_device, "netlist_sound", "Netlist Sound Device");


        public class device_sound_interface_netlist_mame_sound : device_sound_interface
        {
            public device_sound_interface_netlist_mame_sound(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples) { ((netlist_mame_sound_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs, samples); }
        }


        device_sound_interface_netlist_mame_sound m_disound;


        std.map<int, nld_sound_out> m_out = new std.map<int, nld_sound_out>();
        nld_sound_in m_in;
        sound_stream m_stream;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_sound_device_t
        // ----------------------------------------------------------------------------------------
        netlist_mame_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, NETLIST_SOUND, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_netlist_mame_sound(mconfig, this));  // device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_netlist_mame_sound>();

            m_in = null;
            m_stream = null;
        }


        public device_sound_interface_netlist_mame_sound disound { get { return m_disound; } }


        //netlist_mame_sound_device & set_source(void (*setup_func)(netlist::nlparse_t &))
        //template <typename T, typename F> netlist_mame_sound_device & set_source(T *obj, F && f)
        public netlist_mame_sound_device set_source(func_type setup_func) { set_setup_func(setup_func); return this; }


        public sound_stream get_stream() { return m_stream; }


        // device_sound_interface overrides
        void device_sound_interface_sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples)
        {
            foreach (var e in m_out)
            {
                e.second().buffer = outputs[e.first()];
                e.second().bufsize = samples;
            }

            if (m_in != null)
            {
                m_in.buffer_reset();
                for (int i = 0; i < m_in.num_channels(); i++)
                {
                    m_in.channels[i].m_buffer = inputs[i];
                }
            }

            netlist_time cur = netlist().time();

            netlist().process_queue(div * samples);

            cur += (div * samples);

            foreach (var e in m_out)
            {
                e.second().sound_update_fill(samples);
                e.second().buffer_reset(cur);
            }
        }


        // netlist_mame_device
        protected override void nl_register_devices(netlist.setup_t lsetup)
        {
            lsetup.factory().register_device<nld_sound_out>("NETDEV_SOUND_OUT", "nld_sound_out", "+CHAN");
            lsetup.factory().register_device<nld_sound_in>("NETDEV_SOUND_IN", "nld_sound_in", "-");
        }


        // device_t overrides

        protected override void device_start()
        {
            base.device_start();

            netlist_global.LOGDEVCALLS(this, "sound device_start\n");

            // Configure outputs

            std.vector<nld_sound_out> outdevs = netlist().nlstate().get_device_list<nld_sound_out>();
            if (outdevs.size() == 0)
                fatalerror("No output devices");

            //m_num_outputs = outdevs.size();

            /* resort channels */
            foreach (var outdev in outdevs)
            {
                int chan = outdev.channel.op();

                netlist().log().verbose.op("Output {0} on channel {1}", outdev.name(), chan);

                if (chan < 0 || chan >= outdevs.size())
                    fatalerror("illegal channel number");
                m_out[chan] = outdev;
                m_out[chan].sample_time = netlist_time.from_hz(clock());
                m_out[chan].buffer = null;
                m_out[chan].bufsize = 0;
            }

            // Configure inputs
            // FIXME: The limitation to one input device seems artificial.
            //        We should allow multiple devices with one channel each.

            m_in = null;

            std.vector<nld_sound_in> indevs = netlist().nlstate().get_device_list<nld_sound_in>();
            if (indevs.size() > 1)
                fatalerror("A maximum of one input device is allowed!");

            if (indevs.size() == 1)
            {
                m_in = indevs[0];
                m_in.resolve();
                m_in.inc = netlist_time.from_hz(clock());
            }

            /* initialize the stream(s) */
            m_stream = machine().sound().stream_alloc(this, m_in != null ? m_in.num_channels() : 0, m_out.size(), (int)clock());
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_sub_interface
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_sub_interface
    {
        double m_offset;
        double m_mult;

        netlist_mame_device m_owner;
        netlist_mame_sound_device m_sound;


        // construction/destruction
        public netlist_mame_sub_interface(device_t aowner)
        {
            m_offset = 0.0;
            m_mult = 1.0;

            m_owner = (netlist_mame_device)aowner;
            m_sound = (netlist_mame_sound_device)aowner;
        }

        //virtual ~netlist_mame_sub_interface() { }


        public double offset { get { return m_offset; } }
        public double mult { get { return m_mult; } }


        public virtual void custom_netlist_additions(netlist.netlist_state_t nlstate) { }
        public virtual void pre_parse_action(netlist.netlist_state_t nlstate) { }


        //inline netlist_mame_device &nl_owner() const { return *m_owner; }


        public void update_to_current_time() { if (m_sound != null) m_sound.get_stream().update(); }


        public void set_mult_offset(double mult, double offset)
        {
            m_mult = mult;
            m_offset = offset;
        }
    }


    public class device_t_with_netlist_mame_sub_interface : device_t
    {
        protected netlist_mame_sub_interface m_netlist_mame_sub_interface;

        public device_t_with_netlist_mame_sub_interface(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, type, tag, owner, clock)
        {
            m_netlist_mame_sub_interface = new netlist_mame_sub_interface(owner);
        }

        public virtual void custom_netlist_additions(netlist.netlist_state_t nlstate) { m_netlist_mame_sub_interface.custom_netlist_additions(nlstate); }
        public virtual void pre_parse_action(netlist.netlist_state_t nlstate) { }

        public void set_mult_offset(double mult, double offset) { m_netlist_mame_sub_interface.set_mult_offset(mult, offset); }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_analog_input_device
    // ----------------------------------------------------------------------------------------
    class netlist_mame_analog_input_device : device_t
                                             //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_ANALOG_INPUT,  netlist_mame_analog_input_device,  "nl_analog_in",  "Netlist Analog Input")
        static device_t device_creator_netlist_mame_analog_input_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_analog_input_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_ANALOG_INPUT = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_analog_input_device, "nl_analog_in",  "Netlist Analog Input");


        // TODO remove this when this class is finished
        public static void static_set_mult_offset(device_t device, double mult, double offset) { }


        //netlist::param_num_t<double> *m_param;
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
        //inline DECLARE_WRITE8_MEMBER(write8)               { write(data);   }
        //inline DECLARE_WRITE16_MEMBER(write16)             { write(data);   }
        //inline DECLARE_WRITE32_MEMBER(write32)             { write(data);   }
        //inline DECLARE_WRITE64_MEMBER(write64)             { write(data);   }


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
    // netlist_mame_logic_input_device
    // ----------------------------------------------------------------------------------------
    class netlist_mame_logic_input_device : device_t_with_netlist_mame_sub_interface
                                            //device_t
                                            //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_LOGIC_INPUT,   netlist_mame_logic_input_device,   "nl_logic_in",   "Netlist Logic Input")
        static device_t device_creator_netlist_mame_logic_input_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_logic_input_device(mconfig, tag, owner, clock); }
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


        void set_params(string param_name, uint32_t shift)
        {
            //throw new emu_unimplemented();
#if false
            if (LOG_DEV_CALLS) logerror("set_params\n");
#endif
            m_param_name = param_name;
            m_shift = shift;
        }


        void write(uint32_t val)
        {
            uint32_t v = (val >> (int)m_shift) & 1;
            if ((v != 0) != m_param.op())
                synchronize(0, (int)v);
        }


        //inline DECLARE_INPUT_CHANGED_MEMBER(input_changed) { write(newval); }

        //DECLARE_WRITE_LINE_MEMBER(write_line)       { write(state);  }
        public void write_line(int state) { write((uint32_t)state);  }
        //DECLARE_WRITE8_MEMBER(write8)               { write(data);   }
        //DECLARE_WRITE16_MEMBER(write16)             { write(data);   }
        //DECLARE_WRITE32_MEMBER(write32)             { write(data);   }
        //DECLARE_WRITE64_MEMBER(write64)             { write(data);   }


        // device-level overrides
        protected override void device_start()
        {
            netlist_global.LOGDEVCALLS(this, "start\n");

            netlist.param_t p = ((netlist_mame_device)this.owner()).setup().find_param(m_param_name);  //pstring(m_param_name, pstring.UTF8));
            m_param = (netlist.param_logic_t)p;
            if (m_param == null)
            {
                fatalerror("device {0} wrong parameter type for {1}\n", basetag(), m_param_name);
            }
        }

        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
        {
            m_netlist_mame_sub_interface.update_to_current_time();
            m_param.setTo(param != 0);
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_stream_input_device
    // ----------------------------------------------------------------------------------------
    public class netlist_mame_stream_input_device : device_t_with_netlist_mame_sub_interface
                                                    //device_t
                                                    //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_STREAM_INPUT,  netlist_mame_stream_input_device,  "nl_stream_in",  "Netlist Stream Input")
        static device_t device_creator_netlist_mame_stream_input_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_stream_input_device(mconfig, tag, owner, clock); }
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


        public override void custom_netlist_additions(netlist.netlist_state_t nlstate)
        {
            if (!nlstate.setup().device_exists("STREAM_INPUT"))
                nlstate.setup().register_dev("NETDEV_SOUND_IN", "STREAM_INPUT");

            string sparam = string.Format("STREAM_INPUT.CHAN{0}", m_channel);  //plib::pfmt("STREAM_INPUT.CHAN{1}")(m_channel);
            nlstate.setup().register_param(sparam, m_param_name);  //pstring(m_param_name, pstring::UTF8));
            sparam = string.Format("STREAM_INPUT.MULT{0}", m_channel);  //plib::pfmt("STREAM_INPUT.MULT{1}")(m_channel);
            nlstate.setup().register_param(sparam, m_netlist_mame_sub_interface.mult);
            sparam = string.Format("STREAM_INPUT.OFFSET{0}", m_channel);  //plib::pfmt("STREAM_INPUT.OFFSET{1}")(m_channel);
            nlstate.setup().register_param(sparam, m_netlist_mame_sub_interface.offset);
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_stream_output_device
    // ----------------------------------------------------------------------------------------

    public class netlist_mame_stream_output_device : device_t_with_netlist_mame_sub_interface
                                                     //device_t
                                                     //netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_STREAM_OUTPUT, netlist_mame_stream_output_device, "nl_stream_out", "Netlist Stream Output")
        static device_t device_creator_netlist_mame_stream_output_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_stream_output_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_STREAM_OUTPUT = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_stream_output_device, "nl_stream_out", "Netlist Stream Output");


        uint32_t m_channel;
        string m_out_name;


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
        }


        public void set_params(int channel, string out_name)
        {
            m_out_name = out_name;
            m_channel = (uint32_t)channel;
        }


        // device-level overrides
        protected override void device_start()
        {
            netlist_global.LOGDEVCALLS(this, "start\n");
        }


        public override void custom_netlist_additions(netlist.netlist_state_t nlstate)
        {
            //NETLIB_NAME(sound_out) *snd_out;
            string sname = string.Format("STREAM_OUT_{0}", m_channel);  //plib::pfmt("STREAM_OUT_{1}")(m_channel);

            //snd_out = dynamic_cast<NETLIB_NAME(sound_out) *>(setup.register_dev("nld_sound_out", sname));
            nlstate.setup().register_dev("NETDEV_SOUND_OUT", sname);

            nlstate.setup().register_param(sname + ".CHAN", m_channel);
            nlstate.setup().register_param(sname + ".MULT", m_netlist_mame_sub_interface.mult);
            nlstate.setup().register_param(sname + ".OFFSET", m_netlist_mame_sub_interface.offset);
            nlstate.setup().register_link(sname + ".IN", m_out_name);  //pstring(m_out_name, pstring::UTF8));
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
    //{
    //public:
    //    netlist_source_memregion_t(netlist::setup_t &setup, pstring name)
    //    : netlist::source_t(setup), m_name(name)
    //    {
    //    }
    //
    //    virtual std::unique_ptr<plib::pistream> stream(const pstring &name) override;
    //private:
    //    pstring m_name;
    //};


    class netlist_data_memregions_t : netlist.source_t
    {
        device_t m_dev;


        public netlist_data_memregions_t(device_t dev)
            : base(netlist.source_t.type_t.DATA)
        {
            m_dev = dev;
        }


        protected override plib.pistream stream(string name)  //virtual plib::unique_ptr<plib::pistream> stream(const pstring &name) override;
        {
            throw new emu_unimplemented();
        }
    }


    // ----------------------------------------------------------------------------------------
    // sound_out
    // ----------------------------------------------------------------------------------------
    //class NETLIB_NAME(sound_out) : public netlist::device_t
    class nld_sound_out : netlist.device_t
    {
        //const int BUFSIZE = 2048;

        netlist.param_int_t m_channel;
        netlist.param_double_t m_mult;
        netlist.param_double_t m_offset;
        ListPointer<stream_sample_t> m_buffer;  //stream_sample_t *m_buffer;
        int m_bufsize;

        netlist_time m_sample_time;

        netlist.analog_input_t m_in;
        double m_cur;
        int m_last_pos;
        netlist.state_var<netlist_time> m_last_buffer_time;


        //NETLIB_NAME(sound_out)(netlist::netlist_state_t &anetlist, const pstring &name)
        public nld_sound_out(netlist.netlist_state_t anetlist, string name)
            : base(anetlist, name)
        {
            m_channel = new netlist.param_int_t(this, "CHAN", 0);
            m_mult = new netlist.param_double_t(this, "MULT", 1000.0);
            m_offset = new netlist.param_double_t(this, "OFFSET", 0.0);
            m_buffer = null;
            m_bufsize = 0;
            m_sample_time = netlist_time.from_hz(1); //sufficiently big enough
            m_in = new netlist.analog_input_t(this, "IN");
            m_cur = 0.0;
            m_last_pos = 0;
            m_last_buffer_time = new netlist.state_var<netlist_time>(this, "m_last_buffer", netlist_time.zero());
        }


        public netlist.param_int_t channel { get { return m_channel; } }
        public ListPointer<stream_sample_t> buffer { get { return m_buffer; } set { m_buffer = value; } }
        public int bufsize { get { return m_bufsize; } set { m_bufsize = value; } }
        public netlist_time sample_time { get { return m_sample_time; } set { m_sample_time = value; } }


        //ATTR_COLD void reset() override
        public override void reset()
        {
            m_cur = 0.0;
            m_last_pos = 0;
            m_last_buffer_time.op = netlist_time.zero();
        }


        public void sound_update(netlist_time upto)
        {
            int pos = (int)(((upto - m_last_buffer_time.op) / m_sample_time).as_raw());
            if (pos > m_bufsize)
                throw new emu_fatalerror("sound {0}: pos {1} exceeded bufsize {2}\n", name().c_str(), pos, m_bufsize);

            while (m_last_pos < pos )
            {
                m_buffer[m_last_pos++] = (stream_sample_t)m_cur;
            }
        }


        public void sound_update_fill(int samples)
        {
            if (samples > m_bufsize)
                throw new emu_fatalerror("sound {0}: pos {1} exceeded bufsize {2}\n", name().c_str(), samples, m_bufsize);

            while (m_last_pos < samples )
            {
                m_buffer[m_last_pos++] = (stream_sample_t) m_cur;
            }
        }


        //NETLIB_UPDATEI()
        protected override void update()
        {
            nl_double val = m_in.op() * m_mult.op() + m_offset.op();
            sound_update(exec().time());
            /* ignore spikes */
            if (Math.Abs(val) < 32767.0)
                m_cur = val;
            else if (val > 0.0)
                m_cur = 32767.0;
            else
                m_cur = -32767.0;
        }


        public void buffer_reset(netlist_time upto)
        {
            m_last_pos = 0;
            m_last_buffer_time.op = upto;
        }
    }


    // ----------------------------------------------------------------------------------------
    // sound_in
    // ----------------------------------------------------------------------------------------
    //class NETLIB_NAME(sound_in) : public netlist::device_t
    class nld_sound_in : netlist.device_t
    {
        const int MAX_INPUT_CHANNELS = 16;


        public class channel
        {
            public netlist.param_str_t m_param_name;  //netlist::pool_owned_ptr<netlist::param_str_t> m_param_name;
            public netlist.param_double_t m_param;  //netlist::param_double_t *m_param;
            public ListPointer<stream_sample_t> m_buffer;  //stream_sample_t *m_buffer;
            public netlist.param_double_t m_param_mult;  //netlist::pool_owned_ptr<netlist::param_double_t> m_param_mult;
            public netlist.param_double_t m_param_offset;  //netlist::pool_owned_ptr<netlist::param_double_t> m_param_offset;
        }
        channel [] m_channels = new channel [MAX_INPUT_CHANNELS];
        netlist_time m_inc;

        netlist.logic_input_t m_feedback;
        netlist.logic_output_t m_Q;

        int m_pos;
        int m_num_channels;


        //NETLIB_NAME(sound_in)(netlist::netlist_state_t &anetlist, const pstring &name)
        public nld_sound_in(netlist.netlist_state_t anetlist, string name)
            : base(anetlist, name)
        {
            m_inc = netlist_time.from_nsec(1);
            m_feedback = new netlist.logic_input_t(this, "FB"); // clock part
            m_Q = new netlist.logic_output_t(this, "Q");
            m_pos = 0;
            m_num_channels = 0;


            connect(m_feedback, m_Q);
            m_inc = netlist_time.from_nsec(1);


            for (int i = 0; i < MAX_INPUT_CHANNELS; i++)
            {
                m_channels[i] = new channel();
                m_channels[i].m_param_name = new netlist.param_str_t(this, new plib.pfmt("CHAN{0}").op(i), "");  //m_channels[i].m_param_name = netlist::pool().make_poolptr<netlist::param_str_t>(*this, plib::pfmt("CHAN{1}")(i), "");
                m_channels[i].m_param_mult = new netlist.param_double_t(this, new plib.pfmt("MULT{0}").op(i), 1.0);  //m_channels[i].m_param_mult = netlist::pool().make_poolptr<netlist::param_double_t>(*this, plib::pfmt("MULT{1}")(i), 1.0);
                m_channels[i].m_param_offset = new netlist.param_double_t(this, new plib.pfmt("OFFSET{0}").op(i), 0.0);  //m_channels[i].m_param_offset = netlist::pool().make_poolptr<netlist::param_double_t>(*this, plib::pfmt("OFFSET{1}")(i), 0.0);
            }
        }


        public channel [] channels { get { return m_channels; } set { m_channels = value; } }
        public netlist_time inc { get { return m_inc; } set { m_inc = value; } }


        public int num_channels() { return m_num_channels; }


        public override void reset()
        {
            m_pos = 0;
            foreach (var elem in m_channels)
                elem.m_buffer = null;
        }


        public void resolve()
        {
            m_pos = 0;
            for (int i = 0; i < MAX_INPUT_CHANNELS; i++)
            {
                if (!string.IsNullOrEmpty(m_channels[i].m_param_name.op()))  //if ((*m_channels[i].m_param_name)() != pstring(""))
                {
                    if (i != m_num_channels)
                        state().log().fatal.op("sound input numbering has to be sequential!");

                    m_num_channels++;
                    m_channels[i].m_param = (netlist.param_double_t)(setup().find_param(m_channels[i].m_param_name.op(), true));  //dynamic_cast<netlist::param_double_t *>(setup().find_param((*m_param_name[i])(), true));
                }
            }
        }


        //NETLIB_UPDATEI()
        protected override void update()
        {
            for (int i = 0; i < m_num_channels; i++)
            {
                if (m_channels[i].m_buffer == null)
                    break; // stop, called outside of stream_update

                nl_double v = m_channels[i].m_buffer[m_pos];
                m_channels[i].m_param.setTo(v * m_channels[i].m_param_mult.op() + m_channels[i].m_param_offset.op());
            }

            m_pos++;
            m_Q.net().toggle_and_push_to_queue(m_inc);
        }


        public void buffer_reset()
        {
            m_pos = 0;
        }
    }
}
