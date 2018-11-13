// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;
using stream_sample_t = System.Int32;
using offs_t = System.UInt32;
using u32 = System.UInt32;
using uint32_t = System.UInt32;


namespace mame
{
    public delegate void setup_func(netlist.setup_t setup);  //void (*setup_func)(netlist_setup_t &));


    public static class netlist_global
    {
        public static void MCFG_NETLIST_SETUP(device_t device, setup_func _setup) { ((netlist_mame_device)device).set_constructor(_setup); }
        //define MCFG_NETLIST_ANALOG_INPUT(_basetag, _tag, _name)                                        MCFG_DEVICE_ADD(_basetag ":" _tag, NETLIST_ANALOG_INPUT, 0)                                 netlist_mame_analog_input_t::static_set_name(*device, _name);
        public static void MCFG_NETLIST_ANALOG_MULT_OFFSET(device_t device, double _mult, double _offset) { ((device_t_with_netlist_mame_sub_interface)device).set_mult_offset(_mult, _offset); }
        //define MCFG_NETLIST_ANALOG_OUTPUT(_basetag, _tag, _IN, _class, _member, _class_tag)             MCFG_DEVICE_ADD(_basetag ":" _tag, NETLIST_ANALOG_OUTPUT, 0)                                netlist_mame_analog_output_t::static_set_params(*device, _IN,                                           netlist_analog_output_delegate(& _class :: _member,                 						# _class "::" # _member, _class_tag, (_class *) 0)   );
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

        //define NETLIST_LOGIC_PORT_CHANGED(_base, _tag)                                                 PORT_CHANGED_MEMBER(_base ":" _tag, netlist_mame_logic_input_t, input_changed, 0)
        //define NETLIST_ANALOG_PORT_CHANGED(_base, _tag)                                                PORT_CHANGED_MEMBER(_base ":" _tag, netlist_mame_analog_input_t, input_changed, 0)


        // ----------------------------------------------------------------------------------------
        // Extensions to interface netlist with MAME code ....
        // ----------------------------------------------------------------------------------------

#if false
        class netlist_source_memregion_t : public netlist::setup_t::source_t
        {
        public:
            netlist_source_memregion_t(pstring name)
            : netlist::setup_t::source_t(), m_name(name)
            {
            }

            bool parse(netlist::setup_t &setup, const pstring &name);
        private:
            pstring m_name;
        };
#endif

        //#define MEMREGION_SOURCE(_name)                 setup.register_source(palloc(netlist_source_memregion_t(_name)));

        //define NETDEV_ANALOG_CALLBACK_MEMBER(_name)             void _name(const double data, const attotime &time)


        // netlib #defines this and it fights with logmacro.h
        //#undef LOG

        //#define LOG_GENERAL     (1U << 0)
        //#define LOG_DEV_CALLS   (1U << 1)

        //#define VERBOSE (LOG_GENERAL | LOG_DEV_CALLS)
        //#define LOG_OUTPUT_FUNC printf
        //#include "logmacro.h"

        //#define LOGDEVCALLS(...) LOGMASKED(LOG_DEV_CALLS, __VA_ARGS__)
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_device_t
    // ----------------------------------------------------------------------------------------
    class netlist_mame_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NETLIST_CORE,  netlist_mame_device,       "netlist_core",  "Netlist Core Device")
        static device_t device_creator_netlist_mame_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_CORE = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_device, "netlist_core",  "Netlist Core Device");


        // ----------------------------------------------------------------------------------------
        // Special netlist extension devices  ....
        // ----------------------------------------------------------------------------------------
        protected class netlist_mame_t : netlist.netlist_t
        {
            netlist_mame_device m_parent;


            public netlist_mame_t(netlist_mame_device parent, string aname)
                : base(aname)
            {
                m_parent = parent;
            }


            running_machine machine() { return m_parent.machine(); }

            netlist_mame_device parent() { return m_parent; }


            public override void vlog(plib.plog_level l, string ls)
            {
                switch (l)
                {
                case plib.plog_level.DEBUG:
                    m_parent.logerror("netlist DEBUG: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.INFO:
                    m_parent.logerror("netlist INFO: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.VERBOSE:
                    m_parent.logerror("netlist VERBOSE: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.WARNING:
                    m_parent.logerror("netlist WARNING: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.ERROR:
                    m_parent.logerror("netlist ERROR: {0}\n", ls.c_str());
                    break;
                case plib.plog_level.FATAL:
                    emu_fatalerror error = new emu_fatalerror("netlist ERROR: {0}\n", ls.c_str());
                    throw error;
                }
            }
        }


        int m_icount;

        netlist_time m_div;


        /* timing support here - so sound can hijack it ... */
        netlist_time m_rem;
        netlist_time m_old;

        netlist_mame_t m_netlist;

        setup_func m_setup_func;  //void (*m_setup_func)(netlist::setup_t &);


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
            m_netlist = null;
            m_setup_func = null;
        }

        ~netlist_mame_device()
        {
            //throw new emu_unimplemented();
#if false
            LOGDEVCALLS("~netlist_mame_device\n");
#endif
        }


        public void set_constructor(setup_func setup_func)  //(void (*setup_func)(netlist::setup_t &))
        {
            //throw new emu_unimplemented();
#if false
            if (LOG_DEV_CALLS) logerror("set_constructor\n");
#endif
            m_setup_func = setup_func;
        }


        protected netlist.setup_t setup() { return m_netlist.setup(); }
        protected netlist_mame_t netlist() { return m_netlist; }


        //ATTR_HOT inline netlist_time last_time_update() { return m_old; }
        //ATTR_HOT void update_time_x();
        //ATTR_HOT void check_mame_abort_slice();


        //static void register_memregion_source(netlist::setup_t &setup, const char *name);


        // Custom to netlist ...
        protected virtual void nl_register_devices() { }


        // device_t overrides

        protected override void device_config_complete()
        {
            //throw new emu_unimplemented();
#if false
            LOGDEVCALLS("device_config_complete %s\n", this->mconfig().gamedrv().name);
#endif
        }


        protected override void device_validity_check(validity_checker valid)
        {
            //throw new emu_unimplemented();
#if false
            LOGDEVCALLS("device_validity_check %s\n", this->mconfig().gamedrv().name);
#endif
        }


        protected override void device_start()
        {
            //throw new emu_unimplemented();
#if false
            LOGDEVCALLS("device_start entry\n");
#endif

            //printf("clock is %d\n", clock());

            m_netlist = new netlist_mame_t(this, "netlist");

            // register additional devices

            nl_register_devices();

            /* let sub-devices add sources and do stuff prior to parsing */
            foreach (device_t d in subdevices())
            {
                device_t_with_netlist_mame_sub_interface sdev = (device_t_with_netlist_mame_sub_interface)d;
                if ( sdev != null )
                {
                    //throw new emu_unimplemented();
#if false
                    LOGDEVCALLS("Preparse subdevice %s/%s\n", d.name(), d.shortname());
#endif
                    sdev.pre_parse_action(setup());
                }
            }

            /* add default data provider for roms */
            setup().register_source(new netlist_data_memregions_t(setup()));  //plib::make_unique_base<netlist::source_t, netlist_data_memregions_t>(setup()));

            m_setup_func(setup());

            /* let sub-devices tweak the netlist */
            foreach (device_t d in subdevices())
            {
                device_t_with_netlist_mame_sub_interface sdev = (device_t_with_netlist_mame_sub_interface)d;
                if ( sdev != null )
                {
                    //throw new emu_unimplemented();
#if false
                    LOGDEVCALLS("Found subdevice %s/%s\n", d.name(), d.shortname());
#endif
                    sdev.custom_netlist_additions(setup());
                }
            }

            netlist().start();

            netlist().save(this, m_rem, "m_rem");
            netlist().save(this, m_div, "m_div");
            netlist().save(this, m_old, "m_old");

            save_state();

            m_old = netlist_time.zero();
            m_rem = netlist_time.zero();

            //throw new emu_unimplemented();
#if false
            LOGDEVCALLS("device_start exit\n");
#endif
        }


        protected override void device_stop()
        {
            throw new emu_unimplemented();
#if false
            LOGDEVCALLS("device_stop\n");
            netlist().stop();

            global_free(m_netlist);
            m_netlist = nullptr;
#endif
        }


        protected override void device_reset()
        {
            //throw new emu_unimplemented();
#if false
            LOGDEVCALLS("device_reset\n");
#endif

            m_old = netlist_time.zero();
            m_rem = netlist_time.zero();
            netlist().reset();
        }


        protected override void device_post_load()
        {
            throw new emu_unimplemented();
#if false
            LOGDEVCALLS("device_post_load\n");

            netlist().state().post_load();
            netlist().rebuild_lists();
#endif
        }


        protected override void device_pre_save()
        {
            throw new emu_unimplemented();
#if false
            LOGDEVCALLS("device_pre_save\n");

            netlist().state().pre_save();
#endif
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
            foreach (var s in netlist().state().save_list())
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
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_cpu_device_t
    // ----------------------------------------------------------------------------------------
    class netlist_mame_cpu_device : netlist_mame_device
                                    //public device_execute_interface,
                                    //public device_state_interface,
                                    //public device_disasm_interface,
                                    //public device_memory_interface
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


        //offs_t genPC() const { return m_genPC; }


        // netlist_mame_device
        //virtual void nl_register_devices();


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


    class device_sound_interface_netlist_mame_sound_device : device_sound_interface
    {
        public device_sound_interface_netlist_mame_sound_device(machine_config mconfig, device_t device) : base(mconfig, device) { }


        // device_sound_interface overrides
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        public override void sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples)
        {
            throw new emu_unimplemented();
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_sound_device
    // ----------------------------------------------------------------------------------------
    class netlist_mame_sound_device : netlist_mame_device
                                      //public device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_SOUND, netlist_mame_sound_device, "netlist_sound", "Netlist Sound Device")
        static device_t device_creator_netlist_mame_sound_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_sound_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_SOUND = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_sound_device, "netlist_sound", "Netlist Sound Device");


        device_sound_interface_netlist_mame_sound_device m_disound;


        const int MAX_OUT = 10;
        nld_sound_out [] m_out;  //nld_sound_out *m_out[MAX_OUT];
        nld_sound_in m_in;
        sound_stream m_stream;
        int m_num_inputs;
        int m_num_outputs;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_sound_device_t
        // ----------------------------------------------------------------------------------------
        netlist_mame_sound_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, NETLIST_SOUND, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_netlist_mame_sound_device(mconfig, this));  // device_sound_interface(mconfig, *this);

            m_out = new nld_sound_out[MAX_OUT];
            m_in = null;
            m_stream = null;
            m_num_inputs = 0;
            m_num_outputs = 0;
        }


        //inline sound_stream *get_stream() { return m_stream; }


        // device_sound_interface overrides
        //virtual void sound_stream_update(sound_stream &stream, stream_sample_t **inputs, stream_sample_t **outputs, int samples) override;


        // netlist_mame_device
        protected override void nl_register_devices()
        {
            setup().factory().register_device<nld_sound_out>("NETDEV_SOUND_OUT", "nld_sound_out", "+CHAN");
            setup().factory().register_device<nld_sound_in>("NETDEV_SOUND_IN", "nld_sound_in", "-");
        }


        // device_t overrides

        protected override void device_start()
        {
            base.device_start();

            //throw new emu_unimplemented();
#if false
            LOGDEVCALLS("sound device_start\n");
#endif

            // Configure outputs

            std_vector<nld_sound_out> outdevs = netlist().get_device_list<nld_sound_out>();
            if (outdevs.size() == 0)
                global.fatalerror("No output devices");

            m_num_outputs = outdevs.size();

            /* resort channels */
            for (int i = 0; i < MAX_OUT; i++) m_out[i] = null;
            for (int i = 0; i < m_num_outputs; i++)
            {
                int chan = outdevs[i].channel.op();  //outdevs[i].m_channel();

                netlist().log().verbose.op("Output {0} on channel {1}", i, chan);

                if (chan < 0 || chan >= MAX_OUT || chan >= outdevs.size())
                    global.fatalerror("illegal channel number");
                m_out[chan] = outdevs[i];
                m_out[chan].sample = netlist_time.from_hz(clock());
                m_out[chan].buffer = 0;  //null;
            }

            // Configure inputs

            m_num_inputs = 0;
            m_in = null;

            std_vector<nld_sound_in> indevs = netlist().get_device_list<nld_sound_in>();
            if (indevs.size() > 1)
                global.fatalerror("A maximum of one input device is allowed!");

            if (indevs.size() == 1)
            {
                m_in = indevs[0];
                m_num_inputs = m_in.resolve();
                m_in.inc = netlist_time.from_hz(clock());
            }

            /* initialize the stream(s) */
            m_stream = machine().sound().stream_alloc(this, m_num_inputs, m_num_outputs, (int)clock());
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_sub_interface
    // ----------------------------------------------------------------------------------------
    class netlist_mame_sub_interface
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


        public double offset { get { return m_offset; } }
        public double mult { get { return m_mult; } }


        public virtual void custom_netlist_additions(netlist.setup_t setup) { }
        protected virtual void pre_parse_action(netlist.setup_t setup) { }


        //inline netlist_mame_device &nl_owner() const { return *m_owner; }


        //inline bool is_sound_device() const { return bool(m_sound); }

        //inline void update_to_current_time() { m_sound->get_stream()->update(); }


        public void set_mult_offset(double mult, double offset)
        {
            m_mult = mult;
            m_offset = offset;
        }
    }


    class device_t_with_netlist_mame_sub_interface : device_t
    {
        protected netlist_mame_sub_interface m_netlist_mame_sub_interface;

        public device_t_with_netlist_mame_sub_interface(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, type, tag, owner, clock)
        {
            m_netlist_mame_sub_interface = new netlist_mame_sub_interface(owner);
        }

        public virtual void custom_netlist_additions(netlist.setup_t setup) { m_netlist_mame_sub_interface.custom_netlist_additions(setup); }
        public virtual void pre_parse_action(netlist.setup_t setup) { }

        public void set_mult_offset(double mult, double offset) { m_netlist_mame_sub_interface.set_mult_offset(mult, offset); }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_analog_input_device
    // ----------------------------------------------------------------------------------------
    class netlist_mame_analog_input_device : device_t
                                             //public netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_ANALOG_INPUT,  netlist_mame_analog_input_device,  "nl_analog_in",  "Netlist Analog Input")
        static device_t device_creator_netlist_mame_analog_input_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_analog_input_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_ANALOG_INPUT = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_analog_input_device, "nl_analog_in",  "Netlist Analog Input");


        // TODO remove this when this class is finished
        public static void static_set_mult_offset(device_t device, double mult, double offset) { }

        //netlist_param_double_t *m_param;
        //bool   m_auto_port;
        //string m_param_name;


        // construction/destruction
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
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_stream_input_device
    // ----------------------------------------------------------------------------------------
    class netlist_mame_stream_input_device : device_t_with_netlist_mame_sub_interface
                                             //device_t
                                             //public netlist_mame_sub_interface
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
            //throw new emu_unimplemented();
#if false
            LOGDEVCALLS("start\n");
#endif
        }


        public override void custom_netlist_additions(netlist.setup_t setup)
        {
            if (!setup.device_exists("STREAM_INPUT"))
                setup.register_dev("NETDEV_SOUND_IN", "STREAM_INPUT");

            string sparam = string.Format("STREAM_INPUT.CHAN{0}", m_channel);  //plib::pfmt("STREAM_INPUT.CHAN{1}")(m_channel);
            setup.register_param(sparam, m_param_name);  //pstring(m_param_name, pstring::UTF8));
            sparam = string.Format("STREAM_INPUT.MULT{0}", m_channel);  //plib::pfmt("STREAM_INPUT.MULT{1}")(m_channel);
            setup.register_param(sparam, m_netlist_mame_sub_interface.mult);
            sparam = string.Format("STREAM_INPUT.OFFSET{0}", m_channel);  //plib::pfmt("STREAM_INPUT.OFFSET{1}")(m_channel);
            setup.register_param(sparam, m_netlist_mame_sub_interface.offset);
        }
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_stream_output_device
    // ----------------------------------------------------------------------------------------

    class netlist_mame_stream_output_device : device_t_with_netlist_mame_sub_interface
                                              //device_t
                                              //public netlist_mame_sub_interface
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
            //throw new emu_unimplemented();
#if false
            LOGDEVCALLS("start\n");
#endif
        }


        public override void custom_netlist_additions(netlist.setup_t setup)
        {
            //NETLIB_NAME(sound_out) *snd_out;
            string sname = string.Format("STREAM_OUT_{0}", m_channel);  //plib::pfmt("STREAM_OUT_{1}")(m_channel);

            //snd_out = dynamic_cast<NETLIB_NAME(sound_out) *>(setup.register_dev("nld_sound_out", sname));
            setup.register_dev("NETDEV_SOUND_OUT", sname);

            setup.register_param(sname + ".CHAN", m_channel);
            setup.register_param(sname + ".MULT", m_netlist_mame_sub_interface.mult);
            setup.register_param(sname + ".OFFSET", m_netlist_mame_sub_interface.offset);
            setup.register_link(sname + ".IN", m_out_name);  //pstring(m_out_name, pstring::UTF8));
        }
    }


    // ----------------------------------------------------------------------------------------
    // Extensions to interface netlist with MAME code ....
    // ----------------------------------------------------------------------------------------

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
        public netlist_data_memregions_t(netlist.setup_t setup)
            : base(setup, netlist.source_t.type_t.DATA)
        {
        }


        protected override plib.pistream stream(string name)  //virtual std::unique_ptr<plib::pistream> stream(const pstring &name) override;
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
        const int BUFSIZE = 2048;

        netlist.param_int_t m_channel;
        netlist.param_double_t m_mult;
        netlist.param_double_t m_offset;
        stream_sample_t m_buffer;
        netlist_time m_sample;

        netlist.analog_input_t m_in;
        double m_cur;
        int m_last_pos;
        netlist.state_var<netlist_time> m_last_buffer;  //netlist::state_var<netlist::netlist_time> m_last_buffer;


        //NETLIB_NAME(sound_out)(netlist::netlist_t &anetlist, const pstring &name)
        public nld_sound_out(netlist.netlist_t anetlist, string name)
            : base(anetlist, name)
        {
            m_channel = new netlist.param_int_t(this, "CHAN", 0);
            m_mult = new netlist.param_double_t(this, "MULT", 1000.0);
            m_offset = new netlist.param_double_t(this, "OFFSET", 0.0);
            m_buffer = 0;  //null;
            m_sample = netlist_time.from_hz(1); //sufficiently big enough
            m_in = new netlist.analog_input_t(this, "IN");
            m_cur = 0.0;
            m_last_pos = 0;
            m_last_buffer = new netlist.state_var<netlist_time>(this, "m_last_buffer", netlist_time.zero());
        }


        public netlist.param_int_t channel { get { return m_channel; } }
        public stream_sample_t buffer { get { return m_buffer; } set { m_buffer = value; } }
        public netlist_time sample { get { return m_sample; } set { m_sample = value; } }


        //ATTR_COLD void reset() override
        //{
        //    m_cur = 0.0;
        //    m_last_pos = 0;
        //    m_last_buffer = netlist::netlist_time::zero();
        //}

        //ATTR_HOT void sound_update(const netlist::netlist_time &upto)
        //{
        //    int pos = (upto - m_last_buffer) / m_sample;
        //    if (pos >= BUFSIZE)
        //        netlist().log().fatal("sound {1}: exceeded BUFSIZE\n", name().c_str());
        //    while (m_last_pos < pos )
        //    {
        //        m_buffer[m_last_pos++] = (stream_sample_t) m_cur;
        //    }
        //}

        //NETLIB_UPDATEI()
        //{
        //    nl_double val = m_in() * m_mult() + m_offset();
        //    sound_update(netlist().time());
        //    /* ignore spikes */
        //    if (std::abs(val) < 32767.0)
        //        m_cur = val;
        //    else if (val > 0.0)
        //        m_cur = 32767.0;
        //    else
        //        m_cur = -32767.0;
        //
        //}

        //ATTR_HOT void buffer_reset(const netlist::netlist_time &upto)
        //{
        //    m_last_pos = 0;
        //    m_last_buffer = upto;
        //    m_cur = 0.0;
        //}
    }


    // ----------------------------------------------------------------------------------------
    // sound_in
    // ----------------------------------------------------------------------------------------
    //class NETLIB_NAME(sound_in) : public netlist::device_t
    class nld_sound_in : netlist.device_t
    {
        const int MAX_INPUT_CHANNELS = 10;


        netlist.param_str_t [] m_param_name = new netlist.param_str_t[MAX_INPUT_CHANNELS];  //std::unique_ptr<netlist::param_str_t> m_param_name[MAX_INPUT_CHANNELS];
        netlist.param_double_t [] m_param = new netlist.param_double_t[MAX_INPUT_CHANNELS];  //netlist::param_double_t *m_param[MAX_INPUT_CHANNELS];
        //stream_sample_t *m_buffer[MAX_INPUT_CHANNELS];
        netlist.param_double_t [] m_param_mult = new netlist.param_double_t[MAX_INPUT_CHANNELS];  //std::unique_ptr<netlist::param_double_t> m_param_mult[MAX_INPUT_CHANNELS];
        netlist.param_double_t [] m_param_offset = new netlist.param_double_t[MAX_INPUT_CHANNELS];  //std::unique_ptr<netlist::param_double_t> m_param_offset[MAX_INPUT_CHANNELS];
        netlist_time m_inc;

        netlist.logic_input_t m_feedback;
        netlist.logic_output_t m_Q;

        int m_pos;
        int m_num_channel;


        //NETLIB_NAME(sound_in)(netlist::netlist_t &anetlist, const pstring &name)
        public nld_sound_in(netlist.netlist_t anetlist, string name)
            : base(anetlist, name)
        {
            m_feedback = new netlist.logic_input_t(this, "FB"); // clock part
            m_Q = new netlist.logic_output_t(this, "Q");
            m_pos = 0;
            m_num_channel = 0;


            connect(m_feedback, m_Q);
            m_inc = netlist_time.from_nsec(1);


            for (int i = 0; i < MAX_INPUT_CHANNELS; i++)
            {
                m_param_name[i] = new netlist.param_str_t(this, new plib.pfmt("CHAN{0}").op(i), "");  //std::make_unique<netlist::param_str_t>(this, plib::pfmt("CHAN{1}")(i), "");
                m_param_mult[i] = new netlist.param_double_t(this, new plib.pfmt("MULT{0}").op(i), 1.0);  //std::make_unique<netlist::param_double_t>(this, plib::pfmt("MULT{1}")(i), 1.0);
                m_param_offset[i] = new netlist.param_double_t(this, new plib.pfmt("OFFSET{0}").op(i), 0.0);  //std::make_unique<netlist::param_double_t>(this, plib::pfmt("OFFSET{1}")(i), 0.0);
            }
        }


        public netlist_time inc { get { return m_inc; } set { m_inc = value; } }


        //ATTR_COLD void reset() override
        //{
        //    m_pos = 0;
        //    for (auto & elem : m_buffer)
        //        elem = nullptr;
        //}


        public int resolve()
        {
            m_pos = 0;
            for (int i = 0; i < MAX_INPUT_CHANNELS; i++)
            {
                if (!string.IsNullOrEmpty(m_param_name[i].op()))  //((*m_param_name[i])() != pstring(""))
                {
                    if (i != m_num_channel)
                        netlist().log().fatal.op("sound input numbering has to be sequential!");

                    m_num_channel++;
                    m_param[i] = (netlist.param_double_t)(setup().find_param(m_param_name[i].op(), true));  //dynamic_cast<netlist::param_double_t *>(setup().find_param((*m_param_name[i])(), true));
                }
            }

            return m_num_channel;
        }


        //NETLIB_UPDATEI()
        //{
        //    for (int i=0; i<m_num_channel; i++)
        //    {
        //        if (m_buffer[i] == nullptr)
        //            break; // stop, called outside of stream_update
        //        const nl_double v = m_buffer[i][m_pos];
        //        m_param[i]->setTo(v * (*m_param_mult[i])() + (*m_param_offset[i])());
        //    }
        //    m_pos++;
        //    m_Q.net().toggle_and_push_to_queue(m_inc);
        //}


        //ATTR_HOT void buffer_reset()
        //{
        //    m_pos = 0;
        //}
    }
}
