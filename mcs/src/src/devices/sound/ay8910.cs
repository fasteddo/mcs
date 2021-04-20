// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using s8 = System.SByte;
using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    public static class ay8910_global
    {
        /* Internal resistance at Volume level 7. */

        public const int AY8910_INTERNAL_RESISTANCE  = 356;
        //#define YM2149_INTERNAL_RESISTANCE  (353)


        /*
         * The following define is the default behavior.
         * Output level 0 is 0V and 7ffff corresponds to 5V.
         * Use this to specify that a discrete mixing stage
         * follows.
         */
        public const int AY8910_DISCRETE_OUTPUT      = 0x04;

        /*
         * The following define causes the driver to output
         * resistor values. Intended to be used for
         * netlist interfacing.
         */
        public const int AY8910_RESISTOR_OUTPUT = 0x08;
    }


    public class ay8910_device : device_t
                                 //public device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(AY8910, ay8910_device, "ay8910", "AY-3-8910A PSG")
        static device_t device_creator_ay8910_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new ay8910_device(mconfig, tag, owner, clock); }
        public static readonly device_type AY8910 = DEFINE_DEVICE_TYPE(device_creator_ay8910_device, "ay8910", "AY-3-8910A PSG");


        public class device_sound_interface_ay8910 : device_sound_interface
        {
            public device_sound_interface_ay8910(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((ay8910_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        protected enum psg_type_t
        {
            PSG_TYPE_AY,
            PSG_TYPE_YM
        }


        protected enum config_t
        {
            PSG_DEFAULT = 0x0,
            PSG_PIN26_IS_CLKSEL = 0x1,
            PSG_HAS_INTERNAL_DIVIDER = 0x2,
            PSG_EXTENDED_ENVELOPE = 0x4,
            PSG_HAS_EXPANDED_MODE = 0x8
        }


        /* register id's */
        //enum
        //{
        const int AY_AFINE    = 0x00;
        const int AY_ACOARSE  = 0x01;
        const int AY_BFINE    = 0x02;
        const int AY_BCOARSE  = 0x03;
        const int AY_CFINE    = 0x04;
        const int AY_CCOARSE  = 0x05;
        const int AY_NOISEPER = 0x06;
        const int AY_ENABLE   = 0x07;
        const int AY_AVOL     = 0x08;
        const int AY_BVOL     = 0x09;
        const int AY_CVOL     = 0x0a;
        const int AY_EAFINE   = 0x0b;
        const int AY_EACOARSE = 0x0c;
        const int AY_EASHAPE  = 0x0d;
        const int AY_PORTA    = 0x0e;
        const int AY_PORTB    = 0x0f;
        const int AY_EBFINE   = 0x10;
        const int AY_EBCOARSE = 0x11;
        const int AY_ECFINE   = 0x12;
        const int AY_ECCOARSE = 0x13;
        const int AY_EBSHAPE  = 0x14;
        const int AY_ECSHAPE  = 0x15;
        const int AY_ADUTY    = 0x16;
        const int AY_BDUTY    = 0x17;
        const int AY_CDUTY    = 0x18;
        const int AY_NOISEAND = 0x19;
        const int AY_NOISEOR  = 0x1a;
        const int AY_TEST     = 0x1f;
        //}


        class ay_ym_param
        {
            public double r_up;
            public double r_down;
            public int res_count;
            public double [] res; // = new double[32];

            public ay_ym_param(double r_up, double r_down, int res_count, double [] res) { this.r_up = r_up; this.r_down = r_down; this.res_count = res_count; this.res = res; }
        }


        class mosfet_param
        {
            public double m_Vth;
            public double m_Vg;
            public int m_count;
            public double [] m_Kn; // = new double[32];

            public mosfet_param(double m_Vth, double m_Vg, int m_count, double [] m_Kn) { this.m_Vth = m_Vth; this.m_Vg = m_Vg; this.m_count = m_count; this.m_Kn = m_Kn; }
        }


        // structs
        struct tone_t
        {
            public u32 period;
            public u8 volume;
            public u8 duty;
            public s32 count;
            public u8 duty_cycle;
            public u8 output;


            public void reset()
            {
                period = 0;
                volume = 0;
                duty = 0;
                count = 0;
                duty_cycle = 0;
                output = 0;
            }

            public void set_period(u8 fine, u8 coarse)
            {
                period = (u32)(fine | (coarse << 8));
            }

            public void set_volume(u8 val)
            {
                volume = val;
            }

            public void set_duty(u8 val)
            {
                duty = val;
            }
        }

        struct envelope_t
        {
            public u32 period;
            public s32 count;
            public s8 step;
            public u32 volume;
            public u8 hold;
            public u8 alternate;
            public u8 attack;
            public u8 holding;


            public void reset()
            {
                period = 0;
                count = 0;
                step = 0;
                volume = 0;
                hold = 0;
                alternate = 0;
                attack = 0;
                holding = 0;
            }

            public void set_period(u8 fine, u8 coarse)
            {
                period = (u32)(fine | (coarse << 8));
            }

            public void set_shape(u8 shape, u8 mask)
            {
                attack = (shape & 0x04) != 0 ? mask : (u8)0x00;
                if ((shape & 0x08) == 0)
                {
                    /* if Continue = 0, map the shape to the equivalent one which has Continue = 1 */
                    hold = 1;
                    alternate = attack;
                }
                else
                {
                    hold = (u8)(shape & 0x01);
                    alternate = (u8)(shape & 0x02);
                }
                step = (s8)mask;
                holding = 0;
                volume = (u32)(step ^ attack);
            }
        }


        /*
         * The following is used by all drivers not reviewed yet.
         * This will like the old behavior, output between
         * 0 and 7FFF
         */
        const int AY8910_LEGACY_OUTPUT        = 0x01;

        /*
         * Specifying the next define will simulate the special
         * cross channel mixing if outputs are tied together.
         * The driver will only provide one stream in this case.
         */
        public const int AY8910_SINGLE_OUTPUT        = 0x02;

        /*
         * The following define causes the driver to output
         * resistor values. Intended to be used for
         * netlist interfacing.
         */
        const int AY8910_RESISTOR_OUTPUT      = 0x08;


        /*
         * This define specifies the initial state of YM2149
         * YM2149, YM3439, AY8930 pin 26 (SEL pin).
         * By default it is set to high,
         * compatible with AY8910.
         */
        /* TODO: make it controllable while it's running (used by any hw???) */
        const int YM2149_PIN26_HIGH           = 0x00; /* or N/C */
        const int YM2149_PIN26_LOW            = 0x10;


        const int NUM_CHANNELS = 3;


        const int MAX_OUTPUT = 0x7fff;


        // duty cycle used for AY8930 expanded mode
        static readonly u32 [] duty_cycle = new u32[9]
        {
            0x80000000, // 3.125 %
            0xc0000000, // 6.25 %
            0xf0000000, // 12.50 %
            0xff000000, // 25.00 %
            0xffff0000, // 50.00 %
            0xffffff00, // 75.00 %
            0xfffffff0, // 87.50 %
            0xfffffffc, // 93.75 %
            0xfffffffe  // 96.875 %
        };


        static readonly ay_ym_param ym2149_param = new ay_ym_param
        (
            630, 801,
            16,
            new double [] { 73770, 37586, 27458, 21451, 15864, 12371, 8922,  6796,
                4763,  3521,  2403,  1737,  1123,   762,  438,   251 }
        );

        static readonly ay_ym_param ym2149_param_env = new ay_ym_param
        (
            630, 801,
            32,
            new double [] { 103350, 73770, 52657, 37586, 32125, 27458, 24269, 21451,
                18447, 15864, 14009, 12371, 10506,  8922,  7787,  6796,
                5689,  4763,  4095,  3521,  2909,  2403,  2043,  1737,
                1397,  1123,   925,   762,   578,   438,   332,   251 }
        );


        static readonly ay_ym_param ay8910_param = new ay_ym_param
        (
            800000, 8000000,
            16,
            new double [] { 15950, 15350, 15090, 14760, 14275, 13620, 12890, 11370,
                10600,  8590,  7190,  5985,  4820,  3945,  3017,  2345 }
        );


        static readonly mosfet_param ay8910_mosfet_param = new mosfet_param
        (
            1.465385778,
            4.9,
            16,
            new double [] {
                0.00076,
                0.80536,
                1.13106,
                1.65952,
                2.42261,
                3.60536,
                5.34893,
                8.96871,
                10.97202,
                19.32370,
                29.01935,
                38.82026,
                55.50539,
                78.44395,
                109.49257,
                153.72985,
            }
        );


        device_sound_interface_ay8910 m_disound;


        psg_type_t m_type;
        int m_streams;
        int m_ioports;
        int m_ready;
        sound_stream m_channel;
        bool m_active;
        s32 m_register_latch;
        u8 [] m_regs = new u8[16];
        s32 m_last_enable;
        tone_t [] m_tone = new tone_t[NUM_CHANNELS];
        envelope_t [] m_envelope = new envelope_t[NUM_CHANNELS];
        u8 m_prescale_noise;
        s32 m_count_noise;
        s32 m_rng;
        u8 m_mode;
        u8 m_env_step_mask;
        /* init parameters ... */
        int m_step;
        int m_zero_is_off;
        u8 [] m_vol_enabled = new u8[NUM_CHANNELS];
        ay_ym_param m_par;  // const ay_ym_param *m_par;
        ay_ym_param m_par_env;  // const ay_ym_param *m_par_env;
        s32 [,] m_vol_table = new s32[NUM_CHANNELS, 16];
        s32 [,] m_env_table = new s32[NUM_CHANNELS, 32];
        s32 [] m_vol3d_table;  //std::unique_ptr<s32[]> m_vol3d_table;
        int m_flags;          /* Flags */
        int m_feature;        /* Chip specific features */
        int [] m_res_load = new int[3];    /* Load on channel in ohms */
        devcb_read8 m_port_a_read_cb;
        devcb_read8 m_port_b_read_cb;
        devcb_write8 m_port_a_write_cb;
        devcb_write8 m_port_b_write_cb;


        // construction/destruction
        ay8910_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : this(mconfig, AY8910, tag, owner, clock, psg_type_t.PSG_TYPE_AY, 3, 2)
        {
        }


        protected ay8910_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock,
            psg_type_t psg_type, int streams, int ioports, int feature = (int)config_t.PSG_DEFAULT)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_ay8910(mconfig, this));  // device_sound_interface(mconfig, *this);
            m_disound = GetClassInterface<device_sound_interface_ay8910>();

            m_type = psg_type;
            m_streams = streams;
            m_ioports = ioports;
            m_ready = 0;
            m_channel = null;
            m_active = false;
            m_register_latch = 0;
            m_last_enable = 0;
            m_prescale_noise = 0;
            m_count_noise = 0;
            m_rng = 0;
            m_mode = 0;
            m_env_step_mask = (((feature & (int)config_t.PSG_HAS_EXPANDED_MODE) == 0) && (psg_type == psg_type_t.PSG_TYPE_AY)) ? (u8)0x0f : (u8)0x1f;
            m_step =          (((feature & (int)config_t.PSG_HAS_EXPANDED_MODE) == 0) && (psg_type == psg_type_t.PSG_TYPE_AY)) ? 2 : 1;
            m_zero_is_off =   (((feature & (int)config_t.PSG_HAS_EXPANDED_MODE) == 0) && (psg_type == psg_type_t.PSG_TYPE_AY)) ? 1 : 0;
            m_par =           (((feature & (int)config_t.PSG_HAS_EXPANDED_MODE) == 0) && (psg_type == psg_type_t.PSG_TYPE_AY)) ? ay8910_param : ym2149_param;
            m_par_env =       (((feature & (int)config_t.PSG_HAS_EXPANDED_MODE) == 0) && (psg_type == psg_type_t.PSG_TYPE_AY)) ? ay8910_param : ym2149_param_env;
            m_flags = AY8910_LEGACY_OUTPUT;
            m_feature = feature;
            m_port_a_read_cb = new devcb_read8(this);
            m_port_b_read_cb = new devcb_read8(this);
            m_port_a_write_cb = new devcb_write8(this);
            m_port_b_write_cb = new devcb_write8(this);


            memset(m_regs, (u8)0);
            m_tone = new tone_t[NUM_CHANNELS];  //memset(&m_tone,0,sizeof(m_tone));
            m_envelope = new envelope_t[NUM_CHANNELS];  //memset(&m_envelope,0,sizeof(m_envelope));
            memset(m_vol_enabled, (u8)0);
            memset(m_vol_table, 0);
            memset(m_env_table, 0);
            m_res_load[0] = m_res_load[1] = m_res_load[2] = 1000; //Default values for resistor loads

            // TODO : measure ay8930 volume parameters (PSG_TYPE_YM for temporary 5 bit handling)
            set_type((m_feature & (int)config_t.PSG_HAS_EXPANDED_MODE) != 0 ? psg_type_t.PSG_TYPE_YM : psg_type);
        }


        public device_sound_interface_ay8910 disound { get { return m_disound; } }


        // configuration helpers
        public void set_flags(int flags) { m_flags = flags; }
        //void set_psg_type(psg_type_t psg_type) { set_type(psg_type); }
        public void set_resistors_load(int res_load0, int res_load1, int res_load2) { m_res_load[0] = res_load0; m_res_load[1] = res_load1; m_res_load[2] = res_load2; }

        public devcb_read.binder port_a_read_callback() { return m_port_a_read_cb.bind(); }
        public devcb_read.binder port_b_read_callback() { return m_port_b_read_cb.bind(); }
        public devcb_write.binder port_a_write_callback() { return m_port_a_write_cb.bind(); }
        public devcb_write.binder port_b_write_callback() { return m_port_b_write_cb.bind(); }


        public u8 data_r() { return ay8910_read_ym(); }


        public void address_w(u8 data)
        {
#if ENABLE_REGISTER_TEST
            return;
#else
            ay8910_write_ym(0, data);
#endif
        }


        public void data_w(u8 data)
        {
#if ENABLE_REGISTER_TEST
            return;
#else
            ay8910_write_ym(1, data);
#endif
        }


        u8 read_data() { return ay8910_read_ym(); }
        void write_address(u8 data) { ay8910_write_ym(0, data); }
        void write_data(u8 data) { ay8910_write_ym(1, data); }


        // /RES
        //void reset_w(u8 data = 0) { ay8910_reset_ym(); }


        // /SEL
        //void set_pin26_low_w(u8 data = 0);
        //void set_pin26_high_w(u8 data = 0);


        // use this when BC1 == A0; here, BC1=0 selects 'data' and BC1=1 selects 'latch address'
        public void data_address_w(offs_t offset, u8 data) { ay8910_write_ym((int)(~offset & 1), data); }  // note that directly connecting BC1 to A0 puts data on 0 and address on 1

        // use this when BC1 == !A0; here, BC1=0 selects 'latch address' and BC1=1 selects 'data'
        public void address_data_w(offs_t offset, u8 data) { ay8910_write_ym((int)(offset & 1), data); }

        // bc1=a0, bc2=a1
        //void write_bc1_bc2(offs_t offset, u8 data);

        //void set_volume(int channel,int volume);

        void ay_set_clock(int clock)
        {
            // FIXME: this doesn't belong here, it should be an input pin exposed via devcb
            if (((m_feature & (int)config_t.PSG_PIN26_IS_CLKSEL) != 0 && (m_flags & YM2149_PIN26_LOW) != 0) || (m_feature & (int)config_t.PSG_HAS_INTERNAL_DIVIDER) != 0)
                m_channel.set_sample_rate((m_feature & (int)config_t.PSG_HAS_EXPANDED_MODE) != 0 ? (u32)clock : (u32)(clock / 16));
            else
                m_channel.set_sample_rate((m_feature & (int)config_t.PSG_HAS_EXPANDED_MODE) != 0 ? (u32)clock * 2 : (u32)(clock / 8));
        }


        // internal interface for PSG component of YM device
        // FIXME: these should be private, but vector06 accesses them directly
        void ay8910_write_ym(int addr, u8 data)
        {
            if ((addr & 1) != 0)
            {
                if (m_active)
                {
                    u8 register_latch = (u8)(m_register_latch + get_register_bank());

                    /* Data port */
                    if (m_register_latch == AY_EASHAPE || m_regs[m_register_latch] != data)
                    {
                        /* update the output buffer before changing the register */
                        m_channel.update();
                    }

                    ay8910_write_reg(register_latch, data);
                }
            }
            else
            {
                m_active = (data >> 4) == 0; // mask programmed 4-bit code
                if (m_active)
                {
                    /* Register port */
                    m_register_latch = data & 0x0f;
                }
                else
                {
                    logerror("{0}: warning - {1} upper address mismatch\n", machine().describe_context(), name());
                }
            }
        }


        u8 ay8910_read_ym()
        {
            device_type chip_type = type();
            int r = m_register_latch + get_register_bank();

            if (!m_active) return 0xff; // high impedance

            if ((r & 0xf) == AY_EASHAPE) // shared register
                r &= 0xf;

            /* There are no state dependent register in the AY8910! */
            /* m_channel->update(); */

            switch (r)
            {
            case AY_PORTA:
                if ((m_regs[AY_ENABLE] & 0x40) != 0)
                    logerror("{0}: warning - read from {1} Port A set as output\n", machine().describe_context(), name());
                /*
                   even if the port is set as output, we still need to return the external
                   data. Some games, like kidniki, need this to work.

                   FIXME: The io ports are designed as open collector outputs. Bits 7 and 8 of AY_ENABLE
                   only enable (low) or disable (high) the pull up resistors. The YM2149 datasheet
                   specifies those pull up resistors as 60k to 600k (min / max).
                   We do need a callback for those two flags. Kid Niki (Irem m62) is one such
                   case were it makes a difference in comparison to a standard TTL output.
                 */
                if (!m_port_a_read_cb.isnull())
                    m_regs[AY_PORTA] = m_port_a_read_cb.op(0);
                else
                    logerror("{0}: warning - read 8910 Port A\n", machine().describe_context());
                break;

            case AY_PORTB:
                if ((m_regs[AY_ENABLE] & 0x80) != 0)
                    logerror("{0}: warning - read from 8910 Port B set as output\n", machine().describe_context());
                if (!m_port_b_read_cb.isnull())
                    m_regs[AY_PORTB] = m_port_b_read_cb.op(0);
                else
                    logerror("{0}: warning - read 8910 Port B\n", machine().describe_context());
                break;
            }

            /* Depending on chip type, unused bits in registers may or may not be accessible.
            Untested chips are assumed to regard them as 'ram'
            Tested and confirmed on hardware:
            - AY-3-8910: inaccessible bits (see masks below) read back as 0
            - AY-3-8914: same as 8910 except regs B,C,D (8,9,A below due to 8910->8914 remapping) are 0x3f
            - AY-3-8916/8917 (used on ECS INTV expansion): inaccessible bits mirror one of the i/o ports, needs further testing
            - YM2149: no anomaly
            */
            if (chip_type == AY8910)
            {
                u8 [] mask = new u8[0x10] { 0xff,0x0f,0xff,0x0f,0xff,0x0f,0x1f,0xff,0x1f,0x1f,0x1f,0xff,0xff,0x0f,0xff,0xff };
                return (u8)(m_regs[r] & mask[r]);
            }
            else if (chip_type == ay8914_device.AY8914)
            {
                u8 [] mask = new u8[0x10] { 0xff,0x0f,0xff,0x0f,0xff,0x0f,0x1f,0xff,0x3f,0x3f,0x3f,0xff,0xff,0x0f,0xff,0xff };
                return (u8)(m_regs[r] & mask[r]);
            }
            else
            {
                return m_regs[r];
            }
        }


        void ay8910_reset_ym()
        {
            m_active = false;
            m_register_latch = 0;
            m_rng = 1;
            m_mode = 0; // ay-3-8910 compatible mode
            for (int chan = 0; chan < NUM_CHANNELS; chan++)
            {
                m_tone[chan].reset();
                m_envelope[chan].reset();
            }
            m_count_noise = 0;
            m_prescale_noise = 0;
            m_last_enable = -1;  /* force a write */
            for (int i = 0; i < AY_PORTA; i++)
                ay8910_write_reg(i,0);
            m_ready = 1;
#if ENABLE_REGISTER_TEST
            ay8910_write_reg(AY_AFINE, 0);
            ay8910_write_reg(AY_ACOARSE, 1);
            ay8910_write_reg(AY_BFINE, 0);
            ay8910_write_reg(AY_BCOARSE, 2);
            ay8910_write_reg(AY_CFINE, 0);
            ay8910_write_reg(AY_CCOARSE, 4);
            //#define AY_NOISEPER   (6)
            ay8910_write_reg(AY_ENABLE, ~7);
            ay8910_write_reg(AY_AVOL, 10);
            ay8910_write_reg(AY_BVOL, 10);
            ay8910_write_reg(AY_CVOL, 10);
            //#define AY_EAFINE  (11)
            //#define AY_EACOARSE    (12)
            //#define AY_EASHAPE (13)
#endif
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            int master_clock = (int)clock();

            if (m_ioports < 1 && !(m_port_a_read_cb.isnull() && m_port_a_write_cb.isnull()))
                fatalerror("Device '{0}' is a {1} and has no port A!", tag(), name());

            if (m_ioports < 2 && !(m_port_b_read_cb.isnull() && m_port_b_write_cb.isnull()))
                fatalerror("Device '{0}' is a {1} and has no port B!", tag(), name());

            m_port_a_read_cb.resolve();
            m_port_b_read_cb.resolve();
            m_port_a_write_cb.resolve();
            m_port_b_write_cb.resolve();

            if ((m_flags & AY8910_SINGLE_OUTPUT) != 0)
            {
                logerror("{0} device using single output!\n", name());
                m_streams = 1;
            }

            m_vol3d_table = new s32[8*32*32*32];  // make_unique_clear<int32_t[]>(8*32*32*32);

            build_mixer_table();

            /* The envelope is pacing twice as fast for the YM2149 as for the AY-3-8910,    */
            /* This handled by the step parameter. Consequently we use a multipler of 2 here. */
            m_channel = m_disound.stream_alloc(0, m_streams, (u32)(master_clock / 8));

            ay_set_clock(master_clock);
            ay8910_statesave();
        }


        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            ay8910_reset_ym();
        }


        protected override void device_clock_changed()
        {
            ay_set_clock((int)clock());
        }


        // device_sound_interface - sound stream update overrides
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            //tone_t *tone;
            //envelope_t *envelope;

            int samples = (int)outputs[0].samples();

            /* hack to prevent us from hanging when starting filtered outputs */
            if (m_ready == 0)
            {
                for (int chan = 0; chan < m_streams; chan++)
                {
                    outputs[chan].fill(0);
                }
            }

            /* The 8910 has three outputs, each output is the mix of one of the three */
            /* tone generators and of the (single) noise generator. The two are mixed */
            /* BEFORE going into the DAC. The formula to mix each channel is: */
            /* (ToneOn | ToneDisable) & (NoiseOn | NoiseDisable). */
            /* Note that this means that if both tone and noise are disabled, the output */
            /* is 1, not 0, and can be modulated changing the volume. */

            /* buffering loop */
            for (int sampindex = 0; sampindex < samples; sampindex++)
            {
                for (int chan = 0; chan < NUM_CHANNELS; chan++)
                {
                    ref tone_t tone = ref m_tone[chan];  //tone = &m_tone[chan];
                    int period = std.max(1, (int)tone.period);
                    tone.count += is_expanded_mode() ? 16 : 1;
                    while (tone.count >= period)
                    {
                        tone.duty_cycle = (u8)((tone.duty_cycle - 1) & 0x1f);
                        tone.output = is_expanded_mode() ? (u8)BIT(duty_cycle[tone_duty(ref tone)], tone.duty_cycle) : (u8)BIT(tone.duty_cycle, 0);
                        tone.count -= period;
                    }
                }

                m_count_noise++;
                if (m_count_noise >= noise_period())
                {
                    /* toggle the prescaler output. Noise is no different to
                    * channels.
                    */
                    m_count_noise = 0;
                    m_prescale_noise ^= 1;

                    if (m_prescale_noise == 0 || is_expanded_mode()) // AY8930 noise generator rate is twice compares as compatibility mode
                    {
                        /* The Random Number Generator of the 8910 is a 17-bit shift */
                        /* register. The input to the shift register is bit0 XOR bit3 */
                        /* (bit0 is the output). This was verified on AY-3-8910 and YM2149 chips. */

                        // TODO : get actually algorithm for AY8930
                        m_rng ^= (((m_rng & 1) ^ ((m_rng >> 3) & 1)) << 17);
                        m_rng >>= 1;
                    }
                }

                for (int chan = 0; chan < NUM_CHANNELS; chan++)
                {
                    ref tone_t tone = ref m_tone[chan];
                    m_vol_enabled[chan] = (u8)((tone.output | (tone_enable(chan) ? 1 : 0)) & (noise_output() | (noise_enable(chan) ? 1 : 0)));
                }

                /* update envelope */
                for (int chan = 0; chan < NUM_CHANNELS; chan++)
                {
                    ref envelope_t envelope = ref m_envelope[chan];  //envelope = &m_envelope[chan];
                    if (envelope.holding == 0)
                    {
                        u32 period = envelope.period * (u32)m_step;
                        envelope.count++;
                        if (envelope.count >= period)
                        {
                            envelope.count = 0;
                            envelope.step--;

                            /* check envelope current position */
                            if (envelope.step < 0)
                            {
                                if (envelope.hold != 0)
                                {
                                    if (envelope.alternate != 0)
                                        envelope.attack ^= m_env_step_mask;

                                    envelope.holding = 1;
                                    envelope.step = 0;
                                }
                                else
                                {
                                    /* if CountEnv has looped an odd number of times (usually 1), */
                                    /* invert the output. */
                                    if (envelope.alternate != 0 && (envelope.step & (m_env_step_mask + 1)) != 0)
                                        envelope.attack ^= m_env_step_mask;

                                    envelope.step = (s8)(envelope.step & m_env_step_mask);
                                }
                            }

                        }
                    }

                    envelope.volume = (u32)(envelope.step ^ envelope.attack);
                }

                if (m_streams == 3)
                {
                    for (int chan = 0; chan < NUM_CHANNELS; chan++)
                    {
                        ref tone_t tone = ref m_tone[chan];
                        if (tone_envelope(ref tone) != 0)
                        {
                            ref envelope_t envelope = ref m_envelope[get_envelope_chan(chan)];
                            u32 env_volume = envelope.volume;
                            if ((m_feature & (int)config_t.PSG_HAS_EXPANDED_MODE) != 0)
                            {
                                if (!is_expanded_mode())
                                {
                                    env_volume >>= 1;
                                    if ((m_feature & (int)config_t.PSG_EXTENDED_ENVELOPE) != 0) // AY8914 Has a two bit tone_envelope field
                                        outputs[chan].put(sampindex, m_vol_table[chan, m_vol_enabled[chan] != 0 ? env_volume >> (3 - tone_envelope(ref tone)) : 0]);
                                    else
                                        outputs[chan].put(sampindex, m_vol_table[chan, m_vol_enabled[chan] != 0 ? env_volume : 0]);
                                }
                                else
                                {
                                    if ((m_feature & (int)config_t.PSG_EXTENDED_ENVELOPE) != 0) // AY8914 Has a two bit tone_envelope field
                                        outputs[chan].put(sampindex, m_env_table[chan, m_vol_enabled[chan] != 0 ? env_volume >> (3 - tone_envelope(ref tone)) : 0]);
                                    else
                                        outputs[chan].put(sampindex, m_env_table[chan, m_vol_enabled[chan] != 0 ? env_volume : 0]);
                                }
                            }
                            else
                            {
                                if ((m_feature & (int)config_t.PSG_EXTENDED_ENVELOPE) != 0) // AY8914 Has a two bit tone_envelope field
                                    outputs[chan].put(sampindex, m_env_table[chan, m_vol_enabled[chan] != 0 ? env_volume >> (3 - tone_envelope(ref tone)) : 0]);
                                else
                                    outputs[chan].put(sampindex, m_env_table[chan, m_vol_enabled[chan] != 0 ? env_volume : 0]);
                            }
                        }
                        else
                        {
                            if (is_expanded_mode())
                                outputs[chan].put(sampindex, m_env_table[chan, m_vol_enabled[chan] != 0 ? tone_volume(ref tone) : 0]);
                            else
                                outputs[chan].put(sampindex, m_vol_table[chan, m_vol_enabled[chan] != 0 ? tone_volume(ref tone) : 0]);
                        }
                    }
                }
                else
                {
                    outputs[0].put(sampindex, mix_3D());
                }
            }
        }


        // trampolines for callbacks from fm.cpp
        //static void psg_set_clock(device_t *device, int clock) { downcast<ay8910_device *>(device)->ay_set_clock(clock); }
        //static void psg_write(device_t *device, int address, int data) { downcast<ay8910_device *>(device)->ay8910_write_ym(address, data); }
        //static int psg_read(device_t *device) { return downcast<ay8910_device *>(device)->ay8910_read_ym(); }
        //static void psg_reset(device_t *device) { downcast<ay8910_device *>(device)->ay8910_reset_ym(); }


        // inlines
        bool tone_enable(int chan) { return BIT(m_regs[AY_ENABLE], chan) != 0; }
        u8 tone_volume(ref tone_t tone) { return (u8)(tone.volume & (is_expanded_mode() ? 0x1f : 0x0f)); }
        u8 tone_envelope(ref tone_t tone) { return (u8)((tone.volume >> (is_expanded_mode() ? 5 : 4)) & ((m_feature & (int)config_t.PSG_EXTENDED_ENVELOPE) != 0 ? 3 : 1)); }
        u8 tone_duty(ref tone_t tone) { return is_expanded_mode() ? ((tone.duty & 0x8) != 0 ? (u8)0x8 : (u8)(tone.duty & 0xf)) : (u8)0x4; }
        u8 get_envelope_chan(int chan) { return is_expanded_mode() ? (u8)chan : (u8)0; }

        bool noise_enable(int chan) { return BIT(m_regs[AY_ENABLE], 3 + chan) != 0; }
        u8 noise_period() { return is_expanded_mode() ? (u8)(m_regs[AY_NOISEPER] & 0xff) : (u8)((m_regs[AY_NOISEPER] & 0x1f) << 1); }
        u8 noise_output() { return (u8)(m_rng & 1); }

        bool is_expanded_mode() { return (m_feature & (int)config_t.PSG_HAS_EXPANDED_MODE) != 0 && ((m_mode & 0xe) == 0xa); }
        u8 get_register_bank() { return is_expanded_mode() ? (u8)((m_mode & 0x1) << 4) : (u8)0; }


        // internal helpers

        void set_type(psg_type_t psg_type)
        {
            m_type = psg_type;
            if (psg_type == psg_type_t.PSG_TYPE_AY)
            {
                m_env_step_mask = 0x0f;
                m_step = 2;
                m_zero_is_off = 1;
                m_par = ay8910_param;
                m_par_env = ay8910_param;
            }
            else
            {
                m_env_step_mask = 0x1f;
                m_step = 1;
                m_zero_is_off = 0;
                m_par = ym2149_param;
                m_par_env = ym2149_param_env;
            }

            if ((m_feature & (int)config_t.PSG_HAS_EXPANDED_MODE) != 0)
                m_step *= 16;
        }


        u16 mix_3D()
        {
            int indx = 0;

            for (int chan = 0; chan < NUM_CHANNELS; chan++)
            {
                ref tone_t tone = ref m_tone[chan];  //tone_t *tone = &m_tone[chan];
                if (tone_envelope(ref tone) != 0)
                {
                    ref envelope_t envelope = ref m_envelope[get_envelope_chan(chan)];  //envelope_t *envelope = &m_envelope[get_envelope_chan(chan)];
                    u32 env_volume = envelope.volume;
                    u32 env_mask = (1U << (chan + 15));
                    if ((m_feature & (int)config_t.PSG_HAS_EXPANDED_MODE) != 0)
                    {
                        if (!is_expanded_mode())
                        {
                            env_volume >>= 1;
                            env_mask = 0;
                        }
                    }
                    if ((m_feature & (int)config_t.PSG_EXTENDED_ENVELOPE) != 0) // AY8914 Has a two bit tone_envelope field
                    {
                        indx = indx | (int)(env_mask | (m_vol_enabled[chan] != 0 ? ((env_volume >> (3-tone_envelope(ref tone))) << (chan*5)) : 0U));
                    }
                    else
                    {
                        indx = indx | (int)(env_mask | (m_vol_enabled[chan] != 0 ? env_volume << (chan*5) : 0U));
                    }
                }
                else
                {
                    u32 tone_mask = is_expanded_mode() ? (1U << (chan + 15)) : 0U;
                    indx = indx | (int)(tone_mask | (m_vol_enabled[chan] != 0 ? (u32)tone_volume(ref tone) << (chan*5) : 0U));
                }
            }

            return (u16)m_vol3d_table[indx];
        }


        void ay8910_write_reg(int r, int v)
        {
            if ((r & 0xf) == AY_EASHAPE) // shared register
                r &= 0xf;

            //if (r >= 11 && r <= 13) printf("%d %x %02x\n", PSG->index, r, v);

            m_regs[r] = (byte)v;
            u8 coarse;

            switch (r)
            {
                case AY_AFINE:
                case AY_ACOARSE:
                    coarse = (u8)(m_regs[AY_ACOARSE] & (is_expanded_mode() ? 0xff : 0xf));
                    m_tone[0].set_period(m_regs[AY_AFINE], coarse);
                    break;
                case AY_BFINE:
                case AY_BCOARSE:
                    coarse = (u8)(m_regs[AY_BCOARSE] & (is_expanded_mode() ? 0xff : 0xf));
                    m_tone[1].set_period(m_regs[AY_BFINE], coarse);
                    break;
                case AY_CFINE:
                case AY_CCOARSE:
                    coarse = (u8)(m_regs[AY_CCOARSE] & (is_expanded_mode() ? 0xff : 0xf));
                    m_tone[2].set_period(m_regs[AY_CFINE], coarse);
                    break;
                case AY_NOISEPER:
                    /* No action required */
                    break;
                case AY_AVOL:
                    m_tone[0].set_volume(m_regs[AY_AVOL]);
                    break;
                case AY_BVOL:
                    m_tone[1].set_volume(m_regs[AY_BVOL]);
                    break;
                case AY_CVOL:
                    m_tone[2].set_volume(m_regs[AY_CVOL]);
                    break;
                case AY_EACOARSE:
                    if ((v & 0x0f) > 0)
                        osd_printf_verbose("ECoarse\n");
                    goto case AY_EAFINE;  // intentional fall-through
                case AY_EAFINE:
                    m_envelope[0].set_period(m_regs[AY_EAFINE], m_regs[AY_EACOARSE]);
                    break;
                case AY_ENABLE:
                    if ((m_last_enable == -1) ||
                        ((m_last_enable & 0x40) != (m_regs[AY_ENABLE] & 0x40)))
                    {
                        /* write out 0xff if port set to input */
                        if (!m_port_a_write_cb.isnull())
                            m_port_a_write_cb.op((offs_t)0, (m_regs[AY_ENABLE] & 0x40) != 0 ? m_regs[AY_PORTA] : (byte)0xff);
                    }

                    if ((m_last_enable == -1) ||
                        ((m_last_enable & 0x80) != (m_regs[AY_ENABLE] & 0x80)))
                    {
                        /* write out 0xff if port set to input */
                        if (!m_port_b_write_cb.isnull())
                            m_port_b_write_cb.op((offs_t)0, (m_regs[AY_ENABLE] & 0x80) != 0 ? m_regs[AY_PORTB] : (byte)0xff);
                    }
                    m_last_enable = m_regs[AY_ENABLE];
                    break;
                case AY_EASHAPE:
                    if ((m_feature & (int)config_t.PSG_HAS_EXPANDED_MODE) != 0)
                    {
                        u8 old_mode = m_mode;
                        m_mode = (u8)((v >> 4) & 0xf);
                        if (old_mode != m_mode)
                        {
                            if (((old_mode & 0xe) == 0xa) ^ ((m_mode & 0xe) == 0xa)) // AY8930 expanded mode
                            {
                                for (int i = 0; i < AY_EASHAPE; i++)
                                {
                                    ay8910_write_reg(i, 0);
                                    ay8910_write_reg(i + 0x10, 0);
                                }
                            }
                            else if ((m_mode & 0xf) != 0)
                            {
                                logerror("warning: activated unknown mode {0} at {1}\n", m_mode & 0xf, name());
                            }
                        }
                    }
                    if ((v & 0x0f) > 0)
                        osd_printf_verbose("EShape\n");

                    m_envelope[0].set_shape(m_regs[AY_EASHAPE], m_env_step_mask);
                    break;
                case AY_PORTA:
                    if ((m_regs[AY_ENABLE] & 0x40) != 0)
                    {
                        if (!m_port_a_write_cb.isnull())
                            m_port_a_write_cb.op(0, m_regs[AY_PORTA]);
                        else
                            logerror("warning: unmapped write {0} to {1} Port A\n", v, name());  // %02x
                    }
                    else
                    {
#if LOG_IGNORED_WRITES
                        logerror("warning: write %02x to %s Port A set as input - ignored\n", v, name());
#endif
                    }
                    break;
                case AY_PORTB:
                    if ((m_regs[AY_ENABLE] & 0x80) != 0)
                    {
                        if (!m_port_b_write_cb.isnull())
                            m_port_b_write_cb.op((offs_t)0, m_regs[AY_PORTB]);
                        else
                            logerror("warning: unmapped write {0} to {1} Port B\n", v, name());  // %02x
                    }
                    else
                    {
#if LOG_IGNORED_WRITES
                        logerror("warning: write %02x to %s Port B set as input - ignored\n", v, name());
#endif
                    }
                    break;
                case AY_EBFINE:
                case AY_EBCOARSE:
                    m_envelope[1].set_period(m_regs[AY_EBFINE], m_regs[AY_EBCOARSE]);
                    break;
                case AY_ECFINE:
                case AY_ECCOARSE:
                    m_envelope[2].set_period(m_regs[AY_ECFINE], m_regs[AY_ECCOARSE]);
                    break;
                case AY_EBSHAPE:
                    m_envelope[1].set_shape(m_regs[AY_EBSHAPE], m_env_step_mask);
                    break;
                case AY_ECSHAPE:
                    m_envelope[2].set_shape(m_regs[AY_ECSHAPE], m_env_step_mask);
                    break;
                case AY_ADUTY:
                    m_tone[0].set_duty(m_regs[AY_ADUTY]);
                    break;
                case AY_BDUTY:
                    m_tone[1].set_duty(m_regs[AY_BDUTY]);
                    break;
                case AY_CDUTY:
                    m_tone[2].set_duty(m_regs[AY_CDUTY]);
                    break;
                case AY_NOISEAND:
                case AY_NOISEOR:
                    // not implemented
                    break;
                default:
                    m_regs[r] = 0; // reserved, set as 0
                    break;
            }
        }


        void build_mixer_table()
        {
            int normalize = 0;

            if ((m_flags & AY8910_LEGACY_OUTPUT) != 0)
            {
                logerror("{0} using legacy output levels!\n", name());
                normalize = 1;
            }

            if ((m_flags & AY8910_RESISTOR_OUTPUT) != 0)
            {
                if (m_type != psg_type_t.PSG_TYPE_AY)
                    fatalerror("AY8910_RESISTOR_OUTPUT currently only supported for AY8910 devices.");

                for (int chan = 0; chan < NUM_CHANNELS; chan++)
                {
                    build_mosfet_resistor_table(ay8910_mosfet_param, m_res_load[chan], ref m_vol_table, chan);
                    build_mosfet_resistor_table(ay8910_mosfet_param, m_res_load[chan], ref m_env_table, chan);
                }
            }
            else if (m_streams == NUM_CHANNELS)
            {
                for (int chan = 0; chan < NUM_CHANNELS; chan++)
                {
                    build_single_table(m_res_load[chan], m_par, normalize, ref m_vol_table, chan, m_zero_is_off);
                    build_single_table(m_res_load[chan], m_par_env, normalize, ref m_env_table, chan, 0);
                }
            }
            /*
             * The previous implementation added all three channels up instead of averaging them.
             * The factor of 3 will force the same levels if normalizing is used.
             */
            else
            {
                build_3D_table(m_res_load[0], m_par, m_par_env, normalize, 3, m_zero_is_off, ref m_vol3d_table);
            }
        }


        void ay8910_statesave()
        {
            //throw new emu_unimplemented();
#if false
            save_item(STRUCT_MEMBER(m_tone, period));
            save_item(STRUCT_MEMBER(m_tone, volume));
            save_item(STRUCT_MEMBER(m_tone, duty));
            save_item(STRUCT_MEMBER(m_tone, count));
            save_item(STRUCT_MEMBER(m_tone, duty_cycle));
            save_item(STRUCT_MEMBER(m_tone, output));

            save_item(STRUCT_MEMBER(m_envelope, period));
            save_item(STRUCT_MEMBER(m_envelope, count));
            save_item(STRUCT_MEMBER(m_envelope, step));
            save_item(STRUCT_MEMBER(m_envelope, volume));
            save_item(STRUCT_MEMBER(m_envelope, hold));
            save_item(STRUCT_MEMBER(m_envelope, alternate));
            save_item(STRUCT_MEMBER(m_envelope, attack));
            save_item(STRUCT_MEMBER(m_envelope, holding));
#endif

            save_item(NAME(new { m_active }));
            save_item(NAME(new { m_register_latch }));
            save_item(NAME(new { m_regs }));
            save_item(NAME(new { m_last_enable }));

            save_item(NAME(new { m_count_noise }));
            save_item(NAME(new { m_prescale_noise }));

            save_item(NAME(new { m_rng }));
            save_item(NAME(new { m_mode }));
            save_item(NAME(new { m_flags }));
        }


        static void build_3D_table(double rl, ay_ym_param par, ay_ym_param par_env, int normalize, double factor, int zero_is_off, ref s32 [] tab)//, s32 *tab)
        {
            double min = 10.0;
            double max = 0.0;

            std.vector<double> temp = new std.vector<double>(8*32*32*32, 0);

            for (int e = 0; e < 8; e++)
            {
                ay_ym_param par_ch1 = (e & 0x01) != 0 ? par_env : par;
                ay_ym_param par_ch2 = (e & 0x02) != 0 ? par_env : par;
                ay_ym_param par_ch3 = (e & 0x04) != 0 ? par_env : par;

                for (int j1 = 0; j1 < par_ch1.res_count; j1++)
                {
                    for (int j2 = 0; j2 < par_ch2.res_count; j2++)
                    {
                        for (int j3 = 0; j3 < par_ch3.res_count; j3++)
                        {
                            double n;
                            if (zero_is_off != 0)
                            {
                                n  = (j1 != 0 || (e & 0x01) != 0) ? 1 : 0;
                                n += (j2 != 0 || (e & 0x02) != 0) ? 1 : 0;
                                n += (j3 != 0 || (e & 0x04) != 0) ? 1 : 0;
                            }
                            else
                            {
                                n = 3.0;
                            }

                            double rt = n / par.r_up + 3.0 / par.r_down + 1.0 / rl;
                            double rw = n / par.r_up;

                            rw += 1.0 / par_ch1.res[j1];
                            rt += 1.0 / par_ch1.res[j1];
                            rw += 1.0 / par_ch2.res[j2];
                            rt += 1.0 / par_ch2.res[j2];
                            rw += 1.0 / par_ch3.res[j3];
                            rt += 1.0 / par_ch3.res[j3];

                            int indx = (e << 15) | (j3 << 10) | (j2 << 5) | j1;
                            temp[indx] = rw / rt;
                            if (temp[indx] < min)
                                min = temp[indx];
                            if (temp[indx] > max)
                                max = temp[indx];
                        }
                    }
                }
            }

            if (normalize != 0)
            {
                for (int j = 0; j < 32*32*32*8; j++)
                    tab[j] = (int)(MAX_OUTPUT * (((temp[j] - min)/(max-min))) * factor);
            }
            else
            {
                for (int j = 0; j < 32*32*32*8; j++)
                    tab[j] = (int)(MAX_OUTPUT * temp[j]);
            }

            /* for (e = 0;e<16;e++) printf("%d %d\n",e << 10, tab[e << 10]); */
        }


        static void build_single_table(double rl, ay_ym_param par, int normalize, ref s32 [,] tab, int rank, int zero_is_off)//s32 *tab, int zero_is_off)
        {
            double rt;
            double rw;
            double [] temp = new double[32];
            double min = 10.0;
            double max = 0.0;

            for (int j = 0; j < par.res_count; j++)
            {
                rt = 1.0 / par.r_down + 1.0 / rl;

                rw = 1.0 / par.res[j];
                rt += 1.0 / par.res[j];

                if (!(zero_is_off != 0 && j == 0))
                {
                    rw += 1.0 / par.r_up;
                    rt += 1.0 / par.r_up;
                }

                temp[j] = rw / rt;
                if (temp[j] < min)
                    min = temp[j];
                if (temp[j] > max)
                    max = temp[j];
            }

            if (normalize != 0)
            {
                for (int j = 0; j < par.res_count; j++)
                    tab[rank, j] = (int)(MAX_OUTPUT * (((temp[j] - min)/(max-min)) - 0.25) * 0.5);
            }
            else
            {
                for (int j = 0; j < par.res_count; j++)
                    tab[rank, j] = (int)(MAX_OUTPUT * temp[j]);
            }
        }


        static void build_mosfet_resistor_table(mosfet_param par, double rd, ref s32 [,] tab, int rank)//, s32 *tab)
        {
            for (int j = 0; j < par.m_count; j++)
            {
                double Vd = 5.0;
                double Vg = par.m_Vg - par.m_Vth;
                double kn = par.m_Kn[j] / 1.0e6;
                double p2 = 1.0 / (2.0 * kn * rd) + Vg;
                double Vs = p2 - std.sqrt(p2 * p2 - Vg * Vg);

                double res = rd * (Vd / Vs - 1.0);
                /* That's the biggest value we can stream on to netlist. */

                if (res > (1 << 28))
                    tab[rank, j] = (1 << 28);
                else
                    tab[rank, j] = (int)res;

                //printf("%d %f %10d\n", j, rd / (res + rd) * 5.0, tab[j]);
            }
        }
    }


    class ay8914_device : ay8910_device
    {
        //DEFINE_DEVICE_TYPE(AY8914, ay8914_device, "ay8914", "AY-3-8914A PSG")
        static device_t device_creator_ay8914_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new ay8914_device(mconfig, tag, owner, clock); }
        public static readonly device_type AY8914 = DEFINE_DEVICE_TYPE(device_creator_ay8914_device, "ay8914", "AY-3-8914A PSG");


        static readonly u8 [] mapping8914to8910 = new u8[16] { 0, 2, 4, 11, 1, 3, 5, 12, 7, 6, 13, 8, 9, 10, 14, 15 };


        //ay8914_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock);
        ay8914_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, AY8914, tag, owner, clock, psg_type_t.PSG_TYPE_AY, 3, 2, (int)config_t.PSG_EXTENDED_ENVELOPE)
        {
        }


        /* AY8914 handlers needed due to different register map */
        u8 read(offs_t offset)
        {
            u8 rv;
            address_w(mapping8914to8910[offset & 0xf]);
            rv = data_r();
            return rv;
        }

        void write(offs_t offset, u8 data)
        {
            address_w(mapping8914to8910[offset & 0xf]);
            data_w((u8)(data & 0xff));
        }
    }
}
