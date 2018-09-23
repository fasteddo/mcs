// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using stream_sample_t = System.Int32;
using offs_t = System.UInt32;
using u32 = System.UInt32;
using uint32_t = System.UInt32;


namespace mame
{
    //void (*setup_func)(netlist_setup_t &));
    public delegate void setup_func_delegate(netlist.setup_t setup);


    public static class netlist_global
    {
        public static void MCFG_NETLIST_SETUP(device_t device, setup_func_delegate setup) { ((netlist_mame_device)device).set_constructor(setup); }
        //define MCFG_NETLIST_ANALOG_INPUT(_basetag, _tag, _name)                                        MCFG_DEVICE_ADD(_basetag ":" _tag, NETLIST_ANALOG_INPUT, 0)                                 netlist_mame_analog_input_t::static_set_name(*device, _name);
        public static void MCFG_NETLIST_ANALOG_MULT_OFFSET(device_t device, double mult, double offset) { throw new emu_unimplemented(); }
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
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_device_t
    // ----------------------------------------------------------------------------------------
    class netlist_mame_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NETLIST_CORE,  netlist_mame_device,       "netlist_core",  "Netlist Core Device")
        static device_t device_creator_netlist_mame_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_CORE = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_device, "netlist_core",  "Netlist Core Device");


        //int m_icount;

        //netlist::netlist_time m_div;


        /* timing support here - so sound can hijack it ... */
        //netlist::netlist_time        m_rem;
        //netlist::netlist_time        m_old;

        //netlist_mame_t *    m_netlist;

        //void (*m_setup_func)(netlist::setup_t &);


        // construction/destruction
        public netlist_mame_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : this(mconfig, NETLIST_CORE, tag, owner, clock)
        {
        }

        public netlist_mame_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            throw new emu_unimplemented();
        }

        ~netlist_mame_device()
        {
            throw new emu_unimplemented();
        }


        public void set_constructor(setup_func_delegate setup_func)  // void (*setup_func)(netlist_setup_t &))
        {
            throw new emu_unimplemented();
        }


        //ATTR_HOT inline netlist_setup_t &setup() { return *m_setup; }
        //ATTR_HOT inline netlist_mame_t &netlist() { return *m_netlist; }


        //ATTR_HOT inline netlist_time last_time_update() { return m_old; }
        //ATTR_HOT void update_time_x();
        //ATTR_HOT void check_mame_abort_slice();


        //static void register_memregion_source(netlist::setup_t &setup, const char *name);


        // Custom to netlist ...
        //virtual void nl_register_devices() { };


        // device_t overrides
        //virtual void device_config_complete();
        //virtual void device_validity_check(validity_checker &valid) const override;
        //virtual void device_start();
        //virtual void device_stop();
        //virtual void device_reset();
        //virtual void device_post_load();
        //virtual void device_pre_save();
        //virtual void device_clock_changed();


        //void save_state();
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
    // netlist_mame_sound_device_t
    // ----------------------------------------------------------------------------------------
    class netlist_mame_sound_device : netlist_mame_device
                                      //public device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_SOUND, netlist_mame_sound_device, "netlist_sound", "Netlist Sound Device")
        static device_t device_creator_netlist_mame_sound_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_sound_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_SOUND = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_sound_device, "netlist_sound", "Netlist Sound Device");


        device_sound_interface_netlist_mame_sound_device m_disound;


        //static const int MAX_OUT = 10;
        //nld_sound_out *m_out[MAX_OUT];
        //nld_sound_in *m_in;
        //sound_stream *m_stream;
        //int m_num_inputs;
        //int m_num_outputs;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_sound_device_t
        // ----------------------------------------------------------------------------------------
        netlist_mame_sound_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NETLIST_SOUND, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_netlist_mame_sound_device(mconfig, this));  // device_sound_interface(mconfig, *this);

            throw new emu_unimplemented();
        }


        //inline sound_stream *get_stream() { return m_stream; }


        // device_sound_interface overrides
        //virtual void sound_stream_update(sound_stream &stream, stream_sample_t **inputs, stream_sample_t **outputs, int samples);


        // netlist_mame_device
        //virtual void nl_register_devices();


        // device_t overrides

        //virtual void device_start();
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_sub_interface
    // ----------------------------------------------------------------------------------------
    class netlist_mame_sub_interface
    {
        //double m_offset;
        //double m_mult;

        //netlist_mame_device_t *m_owner;
        //netlist_mame_sound_device_t *m_sound;


        // construction/destruction
        netlist_mame_sub_interface(device_t aowner)
        {
            throw new emu_unimplemented();
        }


        //virtual void custom_netlist_additions(netlist::setup_t &setup) { }
        //virtual void pre_parse_action(netlist::setup_t &setup) { }


        //inline netlist_mame_device &nl_owner() const { return *m_owner; }


        //inline bool is_sound_device() const { return bool(m_sound); }

        //inline void update_to_current_time() { m_sound->get_stream()->update(); }


        public void set_mult_offset(double mult, double offset)
        {
            throw new emu_unimplemented();
        }
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
        //virtual void device_start();
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_stream_input_device
    // ----------------------------------------------------------------------------------------
    class netlist_mame_stream_input_device : device_t
                                             //public netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_STREAM_INPUT,  netlist_mame_stream_input_device,  "nl_stream_in",  "Netlist Stream Input")
        static device_t device_creator_netlist_mame_stream_input_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_stream_input_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_STREAM_INPUT = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_stream_input_device, "nl_stream_in",  "Netlist Stream Input");


        //UINT32 m_channel;
        //string m_param_name;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_stream_input_t
        // ----------------------------------------------------------------------------------------
        netlist_mame_stream_input_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
                : base(mconfig, NETLIST_STREAM_INPUT, tag, owner, clock)
        {
            throw new emu_unimplemented();
        }


        public void set_params(int channel, string param_name)
        {
            throw new emu_unimplemented();
        }


        // device-level overrides
        //virtual void device_start();
        //virtual void custom_netlist_additions(netlist_setup_t &setup);
    }


    // ----------------------------------------------------------------------------------------
    // netlist_mame_stream_output_device
    // ----------------------------------------------------------------------------------------

    class netlist_mame_stream_output_device : device_t
                                              //public netlist_mame_sub_interface
    {
        //DEFINE_DEVICE_TYPE(NETLIST_STREAM_OUTPUT, netlist_mame_stream_output_device, "nl_stream_out", "Netlist Stream Output")
        static device_t device_creator_netlist_mame_stream_output_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new netlist_mame_stream_output_device(mconfig, tag, owner, clock); }
        public static readonly device_type NETLIST_STREAM_OUTPUT = DEFINE_DEVICE_TYPE(device_creator_netlist_mame_stream_output_device, "nl_stream_out", "Netlist Stream Output");


        //UINT32 m_channel;
        //string m_out_name;


        // construction/destruction
        // ----------------------------------------------------------------------------------------
        // netlist_mame_stream_output_t
        // ----------------------------------------------------------------------------------------
        netlist_mame_stream_output_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
                : base(mconfig, NETLIST_STREAM_OUTPUT, tag, owner, clock)
        {
            throw new emu_unimplemented();
        }


        public void set_params(int channel, string out_name)
        {
            throw new emu_unimplemented();
        }


        // device-level overrides
        //virtual void device_start();
        //virtual void custom_netlist_additions(netlist_setup_t &setup);
    }
}
