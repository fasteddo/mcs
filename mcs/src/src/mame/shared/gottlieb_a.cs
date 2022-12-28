// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame._6532riot_global;
using static mame.dac_global;
using static mame.device_global;
using static mame.diexec_global;
using static mame.disound_global;
using static mame.emumem_global;
using static mame.gottlieb_global;
using static mame.input_merger_global;
using static mame.m6502_global;
using static mame.votrax_global;


namespace mame
{
    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // ======================> gottlieb_sound_p2_device
    //class gottlieb_sound_p2_device : public device_t, public device_mixer_interface


    // ======================> gottlieb_sound_p3_device
    //class gottlieb_sound_p3_device : public device_t, public device_mixer_interface


    // ======================> gottlieb_sound_r1_device
    // rev 1 sound board, with unpopulated VOTRAX
    public class gottlieb_sound_r1_device : device_t
                                            //device_mixer_interface
    {
        //DEFINE_DEVICE_TYPE(GOTTLIEB_SOUND_REV1,        gottlieb_sound_r1_device,             "gotsndr1",   "Gottlieb Sound rev. 1")
        public static readonly emu.detail.device_type_impl GOTTLIEB_SOUND_REV1 = DEFINE_DEVICE_TYPE("gotsndr1", "Gottlieb Sound rev. 1", (type, mconfig, tag, owner, clock) => { return new gottlieb_sound_r1_device(mconfig, tag, owner, clock); });


        device_mixer_interface m_dimixer;


        protected required_device<mc1408_device> m_dac;

        // devices
        required_device<riot6532_device> m_riot;
        //u8 m_dummy = 0;   // needed for save-state support


        static readonly XTAL SOUND1_CLOCK = new XTAL(3_579_545);
        //constexpr XTAL SOUND2_CLOCK(4'000'000);
        //constexpr XTAL SOUND2_SPEECH_CLOCK(3'120'000);


        // construction/destruction
        gottlieb_sound_r1_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : this(mconfig, GOTTLIEB_SOUND_REV1, tag, owner, clock)
        {
        }


        protected gottlieb_sound_r1_device(
                machine_config mconfig,
                device_type type,
                string tag,
                device_t owner,
                uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_mixer_interface(mconfig, this));  //, device_mixer_interface(mconfig, *this)
            m_dimixer = GetClassInterface<device_mixer_interface>();


            m_dac = new required_device<mc1408_device>(this, "dac");
            m_riot = new required_device<riot6532_device>(this, "riot");
        }


        public device_mixer_interface dimixer { get { return m_dimixer; } }


        // read/write
        //void write(u8 data);


        // device-level overrides
        protected override void device_add_mconfig(machine_config config)
        {
            // audio CPU
            m6502_device cpu = M6502(config, "audiocpu", SOUND1_CLOCK / 4); // the board can be set to /2 as well
            cpu.memory().set_addrmap(AS_PROGRAM, r1_map);

            INPUT_MERGER_ANY_HIGH(config, "nmi").output_handler().set_inputline("audiocpu", INPUT_LINE_NMI).reg();

            // I/O configuration
            RIOT6532(config, m_riot, SOUND1_CLOCK / 4);
            m_riot.op0.in_pb_callback().set_ioport("SB1").reg();
            m_riot.op0.out_pb_callback().set("nmi", (int state) => { ((input_merger_device)subdevice("nmi")).in_w<u32_const_0>(state); }).bit(7).invert_interface().reg();  //FUNC(input_merger_device::in_w<0>)).bit(7).invert(); // unsure if this is ever used, but the NMI is connected to the RIOT's PB7
            m_riot.op0.irq_callback().set_inputline("audiocpu", m6502_device.M6502_IRQ_LINE).reg();

            // sound devices
            MC1408(config, m_dac, 0).disound.add_route(ALL_OUTPUTS, this.dimixer, 0.25);
        }

        protected override ioport_constructor device_input_ports() { throw new emu_unimplemented(); }
        protected override void device_start() { throw new emu_unimplemented(); }


        protected virtual void r1_map(address_map map, device_t device) { throw new emu_unimplemented(); }
    }


    // ======================> gottlieb_sound_r1_with_votrax_device
    // fully populated rev 1 sound board
    public class gottlieb_sound_r1_with_votrax_device : gottlieb_sound_r1_device
    {
        //DEFINE_DEVICE_TYPE(GOTTLIEB_SOUND_REV1_VOTRAX, gottlieb_sound_r1_with_votrax_device, "gotsndr1vt", "Gottlieb Sound rev. 1 with Votrax")
        public static readonly emu.detail.device_type_impl GOTTLIEB_SOUND_REV1_VOTRAX = DEFINE_DEVICE_TYPE("gotsndr1vt", "Gottlieb Sound rev. 1 with Votrax", (type, mconfig, tag, owner, clock) => { return new gottlieb_sound_r1_with_votrax_device(mconfig, tag, owner, clock); });


        // devices
        required_device<votrax_sc01_device> m_votrax;

        // internal state
        uint8_t m_last_speech_clock = 0;


        // construction/destruction
        gottlieb_sound_r1_with_votrax_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, GOTTLIEB_SOUND_REV1_VOTRAX, tag, owner, clock)
        {
            m_votrax = new required_device<votrax_sc01_device>(this, "votrax");
            m_last_speech_clock = 0;
        }


        // device-level overrides
        protected override void device_add_mconfig(machine_config config)
        {
            base.device_add_mconfig(config);

            m_dac.op0.disound.reset_routes();
            m_dac.op0.disound.add_route(ALL_OUTPUTS, this.dimixer, 0.20);

            // add the VOTRAX
            VOTRAX_SC01(config, m_votrax, 720000);
            m_votrax.op0.ar_callback().set("nmi", (int state) => { ((input_merger_device)subdevice("nmi")).in_w<u32_const_1>(state); }).reg();  //m_votrax->ar_callback().set("nmi", FUNC(input_merger_device::in_w<1>));
            m_votrax.op0.disound.add_route(ALL_OUTPUTS, this.dimixer, 0.80);
        }

        protected override ioport_constructor device_input_ports() { throw new emu_unimplemented(); }
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_post_load() { throw new emu_unimplemented(); }


        // internal communications
        //void votrax_data_w(uint8_t data);
        //void speech_clock_dac_w(uint8_t data);

        protected override void r1_map(address_map map, device_t device) { throw new emu_unimplemented(); }
    }


    // ======================> gottlieb_sound_p4_device
    // fully populated pin 4 sound board
    class gottlieb_sound_p4_device : device_t
                                     //device_mixer_interface
    {
        //DEFINE_DEVICE_TYPE(GOTTLIEB_SOUND_PIN4,        gottlieb_sound_p4_device,             "gotsndp4",   "Gottlieb Sound pin. 4")
        public static readonly emu.detail.device_type_impl GOTTLIEB_SOUND_PIN4 = DEFINE_DEVICE_TYPE("gotsndp4", "Gottlieb Sound pin. 4", (type, mconfig, tag, owner, clock) => { return new gottlieb_sound_p4_device(mconfig, tag, owner, clock); });


        // devices
        //required_device<m6502_device>   m_dcpu;
        //optional_device<m6502_device>   m_dcpu2;
        //required_device<m6502_device>   m_ycpu;
        //required_device<ay8913_device>  m_ay1;
        //required_device<ay8913_device>  m_ay2;

        // internal state
        //emu_timer *   m_nmi_timer;
        //emu_timer *   m_nmi_clear_timer;
        //emu_timer *   m_latch_timer;
        //uint8_t       m_nmi_rate;
        //uint8_t       m_nmi_state;
        //uint8_t       m_dcpu_latch;
        //uint8_t       m_ycpu_latch;
        //uint8_t       m_speech_control;
        //uint8_t       m_last_command;
        //uint8_t       m_psg_latch;
        //uint8_t       m_psg_data_latch;
        //uint8_t       m_dcpu2_latch;


        // construction/destruction
        gottlieb_sound_p4_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : this(mconfig, GOTTLIEB_SOUND_PIN4, tag, owner, clock)
        {
        }


        protected gottlieb_sound_p4_device(
                machine_config mconfig,
                device_type type,
                string tag,
                device_t owner,
                uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            throw new emu_unimplemented();
#if false
            , device_mixer_interface(mconfig, *this)
            , m_dcpu(*this, "audiocpu")
            , m_dcpu2(*this, "dcpu2")
            , m_ycpu(*this, "speechcpu")
            , m_ay1(*this, "ay1")
            , m_ay2(*this, "ay2")
            , m_nmi_timer(nullptr)
            , m_nmi_clear_timer(nullptr)
            , m_latch_timer(nullptr)
            , m_nmi_rate(0)
            , m_nmi_state(0)
            , m_dcpu_latch(0)
            , m_ycpu_latch(0)
            , m_speech_control(0)
            , m_last_command(0)
            , m_psg_latch(0)
            , m_psg_data_latch(0)
            , m_dcpu2_latch(0)
#endif
        }


        // read/write
        //void write(u8 data);


        // device-level overrides
        protected override void device_add_mconfig(machine_config config) { throw new emu_unimplemented(); }
        protected override void device_start() { throw new emu_unimplemented(); }

        //TIMER_CALLBACK_MEMBER(set_nmi);
        //TIMER_CALLBACK_MEMBER(clear_nmi);
        //TIMER_CALLBACK_MEMBER(update_latch);


        // internal communications
        //uint8_t speech_data_r();
        //uint8_t audio_data_r();
        //uint8_t signal_audio_nmi_r();
        //void signal_audio_nmi_w(uint8_t data);
        //void nmi_rate_w(uint8_t data);
        //void speech_ctrl_w(uint8_t data);
        //void psg_latch_w(uint8_t data);

        //void p4_dmap(address_map &map);
        //void p4_ymap(address_map &map);

        // internal helpers
        //void nmi_timer_adjust();
        //void nmi_state_update();
    }


    // ======================> gottlieb_sound_r2_device
    // fully populated rev 2 sound board
    class gottlieb_sound_r2_device : gottlieb_sound_p4_device
    {
        //DEFINE_DEVICE_TYPE(GOTTLIEB_SOUND_REV2,        gottlieb_sound_r2_device,             "gotsndr2",   "Gottlieb Sound rev. 2")
        public static readonly emu.detail.device_type_impl GOTTLIEB_SOUND_REV2 = DEFINE_DEVICE_TYPE("gotsndr2", "Gottlieb Sound rev. 2", (type, mconfig, tag, owner, clock) => { return new gottlieb_sound_r2_device(mconfig, tag, owner, clock); });


        // devices
        //optional_device<sp0250_device>  m_sp0250;

        // internal state
        //bool     m_cobram3_mod = 0;
        //uint8_t  m_sp0250_latch = 0;


        // construction/destruction
        gottlieb_sound_r2_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, GOTTLIEB_SOUND_REV2, tag, owner, clock)
        {
            throw new emu_unimplemented();
#if false
            , m_sp0250(*this, "spsnd")
            , m_cobram3_mod(false)
            , m_sp0250_latch(0)
#endif
        }


        // configuration helpers
        //void enable_cobram3_mods() { m_cobram3_mod = true; }

        //CUSTOM_INPUT_MEMBER( speech_drq_custom_r );


        // device-level overrides
        protected override void device_add_mconfig(machine_config config) { throw new emu_unimplemented(); }
        protected override ioport_constructor device_input_ports() { throw new emu_unimplemented(); }
        protected override void device_start() { throw new emu_unimplemented(); }


        // internal communications
        //void sp0250_latch_w(uint8_t data);
        //void speech_control_w(uint8_t data);

        //void r2_dmap(address_map &map);
        //void r2_ymap(address_map &map);
    }


    // ======================> gottlieb_sound_p5_device
    // same as p5 plus a YM2151 in the expansion socket
    //class gottlieb_sound_p5_device : public gottlieb_sound_p4_device

    // ======================> gottlieb_sound_p6_device
    // same as p5 plus an extra dac, same as existing audiocpu. For bonebusters.
    //class gottlieb_sound_p6_device : public gottlieb_sound_p5_device

    // ======================> gottlieb_sound_p7_device
    // same as p5 plus MSM6295.
    //class gottlieb_sound_p7_device : public gottlieb_sound_p5_device


    public static class gottlieb_global
    {
        public static gottlieb_sound_r1_with_votrax_device GOTTLIEB_SOUND_REV1_VOTRAX<bool_Required>(machine_config mconfig, device_finder<gottlieb_sound_r1_with_votrax_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, gottlieb_sound_r1_with_votrax_device.GOTTLIEB_SOUND_REV1_VOTRAX, clock); }
    }
}
