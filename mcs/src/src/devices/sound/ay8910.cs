// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using int8_t = System.SByte;
using int32_t = System.Int32;
using offs_t = System.UInt32;
using stream_sample_t = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


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


    class device_sound_interface_ay8910 : device_sound_interface
    {
        public device_sound_interface_ay8910(machine_config mconfig, device_t device) : base(mconfig, device) { }


        // device_sound_interface overrides
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        public override void sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples)
        {
            ay8910_device ay8910 = (ay8910_device)device();

            ListPointer<stream_sample_t> [] buf = new ListPointer<stream_sample_t> [ay8910_device.NUM_CHANNELS];  //stream_sample_t *buf[NUM_CHANNELS];
            int chan;

            buf[0] = new ListPointer<stream_sample_t>(outputs[0]);
            buf[1] = null;
            buf[2] = null;
            if (ay8910.m_streams == ay8910_device.NUM_CHANNELS)
            {
                buf[1] = outputs[1];
                buf[2] = outputs[2];
            }

            /* hack to prevent us from hanging when starting filtered outputs */
            if (ay8910.m_ready == 0)
            {
                for (chan = 0; chan < ay8910_device.NUM_CHANNELS; chan++)
                {
                    if (buf[chan] != null)
                        global.memset(buf[chan], 0, (UInt32)samples);  //memset(buf[chan], 0, samples * sizeof_(*buf[chan]));
                }
            }

            /* The 8910 has three outputs, each output is the mix of one of the three */
            /* tone generators and of the (single) noise generator. The two are mixed */
            /* BEFORE going into the DAC. The formula to mix each channel is: */
            /* (ToneOn | ToneDisable) & (NoiseOn | NoiseDisable). */
            /* Note that this means that if both tone and noise are disabled, the output */
            /* is 1, not 0, and can be modulated changing the volume. */

            /* buffering loop */
            while (samples != 0)
            {
                for (chan = 0; chan < ay8910_device.NUM_CHANNELS; chan++)
                {
                    ay8910.m_count[chan]++;
                    if (ay8910.m_count[chan] >= ay8910.TONE_PERIOD(chan))
                    {
                        ay8910.m_output[chan] ^= 1;
                        ay8910.m_count[chan] = 0;
                    }
                }

                ay8910.m_count_noise++;
                if (ay8910.m_count_noise >= ay8910.NOISE_PERIOD())
                {
                    /* toggle the prescaler output. Noise is no different to
                     * channels.
                     */
                    ay8910.m_count_noise = 0;
                    ay8910.m_prescale_noise ^= 1;

                    if (ay8910.m_prescale_noise != 0)
                    {
                        /* The Random Number Generator of the 8910 is a 17-bit shift */
                        /* register. The input to the shift register is bit0 XOR bit3 */
                        /* (bit0 is the output). This was verified on AY-3-8910 and YM2149 chips. */

                        ay8910.m_rng ^= (((ay8910.m_rng & 1) ^ ((ay8910.m_rng >> 3) & 1)) << 17);
                        ay8910.m_rng >>= 1;
                    }
                }

                for (chan = 0; chan < ay8910_device.NUM_CHANNELS; chan++)
                {
                    ay8910.m_vol_enabled[chan] = (byte)((ay8910.m_output[chan] | ay8910.TONE_ENABLEQ(chan)) & (ay8910.NOISE_OUTPUT() | ay8910.NOISE_ENABLEQ(chan)));
                }

                /* update envelope */
                if (ay8910.m_holding == 0)
                {
                    ay8910.m_count_env++;
                    if (ay8910.m_count_env >= ay8910.ENVELOPE_PERIOD() * ay8910.m_step )
                    {
                        ay8910.m_count_env = 0;
                        ay8910.m_env_step--;

                        /* check envelope current position */
                        if (ay8910.m_env_step < 0)
                        {
                            if (ay8910.m_hold != 0)
                            {
                                if (ay8910.m_alternate != 0)
                                    ay8910.m_attack ^= ay8910.m_env_step_mask;
                                ay8910.m_holding = 1;
                                ay8910.m_env_step = 0;
                            }
                            else
                            {
                                /* if CountEnv has looped an odd number of times (usually 1), */
                                /* invert the output. */
                                if (ay8910.m_alternate != 0 && (ay8910.m_env_step & (ay8910.m_env_step_mask + 1)) != 0)
                                    ay8910.m_attack ^= ay8910.m_env_step_mask;

                                ay8910.m_env_step &= (sbyte)ay8910.m_env_step_mask;
                            }
                        }

                    }
                }
                ay8910.m_env_volume = (UInt32)(ay8910.m_env_step ^ ay8910.m_attack);

                if (ay8910.m_streams == 3)
                {
                    for (chan = 0; chan < ay8910_device.NUM_CHANNELS; chan++)
                    {
                        if (ay8910.TONE_ENVELOPE(chan) != 0)
                        {
                            if (ay8910.type() == ay8914_device.AY8914) // AY8914 Has a two bit tone_envelope field
                            {
                                buf[chan][0] = ay8910.m_env_table[chan, ay8910.m_vol_enabled[chan] != 0 ? ay8910.m_env_volume >> (3 - ay8910.TONE_ENVELOPE(chan)) : 0];  // *(buf[chan]++)
                                buf[chan]++;
                            }
                            else
                            {
                                buf[chan][0] = ay8910.m_env_table[chan, ay8910.m_vol_enabled[chan] != 0 ? ay8910.m_env_volume : 0];  // *(buf[chan]++)
                                buf[chan]++;
                            }
                        }
                        else
                        {
                            buf[chan][0] = ay8910.m_vol_table[chan, ay8910.m_vol_enabled[chan] != 0 ? ay8910.TONE_VOLUME(chan) : 0];  // *(buf[chan]++)
                            buf[chan]++;
                        }
                    }
                }
                else
                {
                    buf[0][0] = ay8910.mix_3D();  // *(buf[0]++)
                    buf[0]++;
                }

                samples--;
            }
        }
    }


    public class ay8910_device : device_t
                                 //public device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(AY8910, ay8910_device, "ay8910", "AY-3-8910A PSG")
        static device_t device_creator_ay8910_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new ay8910_device(mconfig, tag, owner, clock); }
        public static readonly device_type AY8910 = DEFINE_DEVICE_TYPE(device_creator_ay8910_device, "ay8910", "AY-3-8910A PSG");


        protected enum psg_type_t
        {
            PSG_TYPE_AY,
            PSG_TYPE_YM
        }


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
        const int AY8910_SINGLE_OUTPUT        = 0x02;

        /*
         * The following define causes the driver to output
         * resistor values. Intended to be used for
         * netlist interfacing.
         */
        const int AY8910_RESISTOR_OUTPUT      = 0x08;


        /*
         * This define specifies the initial state of YM2149
         * pin 26 (SEL pin). By default it is set to high,
         * compatible with AY8910.
         */
        /* TODO: make it controllable while it's running (used by any hw???) */
        const int YM2149_PIN26_HIGH           = 0x00; /* or N/C */
        const int YM2149_PIN26_LOW            = 0x10;


        public const int NUM_CHANNELS = 3;


        const int MAX_OUTPUT = 0x7fff;

        /* register id's */
        const int AY_AFINE    = 0;
        const int AY_ACOARSE  = 1;
        const int AY_BFINE    = 2;
        const int AY_BCOARSE  = 3;
        const int AY_CFINE    = 4;
        const int AY_CCOARSE  = 5;
        const int AY_NOISEPER = 6;
        const int AY_ENABLE   = 7;
        const int AY_AVOL     = 8;
        const int AY_BVOL     = 9;
        const int AY_CVOL     = 10;
        const int AY_EFINE    = 11;
        const int AY_ECOARSE  = 12;
        const int AY_ESHAPE   = 13;

        const int AY_PORTA    = 14;
        const int AY_PORTB    = 15;


        public int NOISE_ENABLEQ(int chan) { return (m_regs[AY_ENABLE] >> (3 + chan)) & 1; }
        public int TONE_ENABLEQ(int chan) { return (m_regs[AY_ENABLE] >> chan) & 1; }
        public int TONE_PERIOD(int chan) { return m_regs[chan << 1] | ((m_regs[(chan << 1) | 1] & 0x0f) << 8); }
        public int NOISE_PERIOD() { return m_regs[AY_NOISEPER] & 0x1f; }
        public int TONE_VOLUME(int chan) { return m_regs[AY_AVOL + chan] & 0x0f; }
        public int TONE_ENVELOPE(int chan) { return (m_regs[AY_AVOL + chan] >> 4) & (type() == ay8914_device.AY8914 ? 3 : 1); }
        public int ENVELOPE_PERIOD() { return m_regs[AY_EFINE] | (m_regs[AY_ECOARSE] << 8); }
        public int NOISE_OUTPUT() { return m_rng & 1; }


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
        public int m_streams;
        int m_ioports;
        public int m_ready;
        sound_stream m_channel;
        bool m_active;
        int32_t m_register_latch;
        uint8_t [] m_regs = new uint8_t[16];
        int32_t m_last_enable;
        public int32_t [] m_count = new int32_t[NUM_CHANNELS];
        public uint8_t [] m_output = new uint8_t[NUM_CHANNELS];
        public uint8_t m_prescale_noise;
        public int32_t m_count_noise;
        public int32_t m_count_env;
        public int8_t m_env_step;
        public uint32_t m_env_volume;
        public uint8_t m_hold;
        public uint8_t m_alternate;
        public uint8_t m_attack;
        public uint8_t m_holding;
        public int32_t m_rng;
        public uint8_t m_env_step_mask;
        ///* init parameters ... */
        public int m_step;
        int m_zero_is_off;
        public uint8_t [] m_vol_enabled = new uint8_t[NUM_CHANNELS];
        ay_ym_param m_par;
        ay_ym_param m_par_env;
        public int32_t [,] m_vol_table = new int32_t[NUM_CHANNELS, 16];
        public int32_t [,] m_env_table = new int32_t[NUM_CHANNELS, 32];
        int32_t [] m_vol3d_table;  //std::unique_ptr<int32_t[]> m_vol3d_table;
        int m_flags;          /* Flags */
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
            psg_type_t psg_type, int streams, int ioports)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_ay8910(mconfig, this));  // device_sound_interface(mconfig, *this);

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
            m_count_env = 0;
            m_env_step = 0;
            m_env_volume = 0;
            m_hold = 0;
            m_alternate = 0;
            m_attack = 0;
            m_holding = 0;
            m_rng = 0;
            m_env_step_mask = psg_type == psg_type_t.PSG_TYPE_AY ? (byte)0x0f : (byte)0x1f;
            m_step =          psg_type == psg_type_t.PSG_TYPE_AY ? 2 : 1;
            m_zero_is_off =   psg_type == psg_type_t.PSG_TYPE_AY ? 1 : 0;
            m_par =           psg_type == psg_type_t.PSG_TYPE_AY ? ay8910_param : ym2149_param;
            m_par_env =       psg_type == psg_type_t.PSG_TYPE_AY ? ay8910_param : ym2149_param_env;
            m_flags = AY8910_LEGACY_OUTPUT;
            m_port_a_read_cb = new devcb_read8(this);
            m_port_b_read_cb = new devcb_read8(this);
            m_port_a_write_cb = new devcb_write8(this);
            m_port_b_write_cb = new devcb_write8(this);


            global.memset(m_regs, (byte)0);
            global.memset(m_count, 0);
            global.memset(m_output, (byte)0);
            global.memset(m_vol_enabled, (byte)0);
            global.memset(m_vol_table, 0);
            global.memset(m_env_table, 0);
            m_res_load[0] = m_res_load[1] = m_res_load[2] = 1000; //Default values for resistor loads

            set_type(psg_type);
        }


        // configuration helpers
        public void set_flags(int flags) { m_flags = flags; }
        //void set_psg_type(psg_type_t psg_type) { set_type(psg_type); }
        public void set_resistors_load(int res_load0, int res_load1, int res_load2) { m_res_load[0] = res_load0; m_res_load[1] = res_load1; m_res_load[2] = res_load2; }

        public devcb_read.binder port_a_read_callback() { return m_port_a_read_cb.bind(); }
        public devcb_read.binder port_b_read_callback() { return m_port_b_read_cb.bind(); }
        //auto port_a_write_callback() { return m_port_a_write_cb.bind(); }
        //auto port_b_write_callback() { return m_port_b_write_cb.bind(); }


        //READ8_MEMBER( ay8910_device::data_r )
        public u8 data_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return ay8910_read_ym();
        }


        //WRITE8_MEMBER( ay8910_device::address_w )
        public void address_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
#if ENABLE_REGISTER_TEST
            return;
#else
            data_address_w(space, 1, data);
#endif
        }


        //WRITE8_MEMBER( ay8910_device::data_w )
        public void data_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
#if ENABLE_REGISTER_TEST
            return;
#else
            data_address_w(space, 0, data);
#endif
        }


        u8 read_data() { return ay8910_read_ym(); }
        void write_address(u8 data) { ay8910_write_ym(0, data); }
        void write_data(u8 data) { ay8910_write_ym(1, data); }


        /* /RES */
        //DECLARE_WRITE8_MEMBER( reset_w ) { ay8910_reset_ym(); }


        // use this when BC1 == A0; here, BC1=0 selects 'data' and BC1=1 selects 'latch address'
        //DECLARE_WRITE8_MEMBER( data_address_w ) { ay8910_write_ym(~offset & 1, data); } // note that directly connecting BC1 to A0 puts data on 0 and address on 1
        public void data_address_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { ay8910_write_ym((int)(~offset & 1), data); } // note that directly connecting BC1 to A0 puts data on 0 and address on 1

        // use this when BC1 == !A0; here, BC1=0 selects 'latch address' and BC1=1 selects 'data'
        //DECLARE_WRITE8_MEMBER( address_data_w ) { ay8910_write_ym(offset & 1, data); }

        // bc1=a0, bc2=a1
        //DECLARE_WRITE8_MEMBER(write_bc1_bc2);

        //void set_volume(int channel,int volume);

        void ay_set_clock(int clock)
        {
            //throw new emu_unimplemented();
#if false
            // FIXME: this doesn't belong here, it should be an input pin exposed via devcb
            if (type() == YM2149 && (m_flags & YM2149_PIN26_LOW))
            {
                clock /= 2;
            }
#endif

            m_channel.set_sample_rate( clock / 8 );
        }


        // internal interface for PSG component of YM device
        // FIXME: these should be private, but vector06 accesses them directly
        void ay8910_write_ym(int addr, uint8_t data)
        {
            if ((addr & 1) != 0)
            {
                if (m_active)
                {
                    /* Data port */
                    if (m_register_latch == AY_ESHAPE || m_regs[m_register_latch] != data)
                    {
                        /* update the output buffer before changing the register */
                        m_channel.update();
                    }

                    ay8910_write_reg(m_register_latch, data);
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


        uint8_t ay8910_read_ym()
        {
            device_type chip_type = type();
            int r = m_register_latch;

            if (!m_active) return 0xff; // high impedance

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
                byte [] mask = new byte[0x10] { 0xff,0x0f,0xff,0x0f,0xff,0x0f,0x1f,0xff,0x1f,0x1f,0x1f,0xff,0xff,0x0f,0xff,0xff };
                return (byte)(m_regs[r] & mask[r]);
            }
            else if (chip_type == ay8914_device.AY8914)
            {
                byte [] mask = new byte[0x10] { 0xff,0x0f,0xff,0x0f,0xff,0x0f,0x1f,0xff,0x3f,0x3f,0x3f,0xff,0xff,0x0f,0xff,0xff };
                return (byte)(m_regs[r] & mask[r]);
            }
            else
            {
                return m_regs[r];
            }
        }


        void ay8910_reset_ym()
        {
            int i;

            m_active = false;
            m_register_latch = 0;
            m_rng = 1;
            m_output[0] = 0;
            m_output[1] = 0;
            m_output[2] = 0;
            m_count[0] = 0;
            m_count[1] = 0;
            m_count[2] = 0;
            m_count_noise = 0;
            m_count_env = 0;
            m_prescale_noise = 0;
            m_last_enable = -1;  /* force a write */
            for (i = 0;i < AY_PORTA;i++)
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
            //#define AY_EFINE  (11)
            //#define AY_ECOARSE    (12)
            //#define AY_ESHAPE (13)
#endif
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_disound = GetClassInterface<device_sound_interface_ay8910>();


            int master_clock = (int)clock();

            if (m_ioports < 1 && !(m_port_a_read_cb.isnull() && m_port_a_write_cb.isnull()))
                global.fatalerror("Device '{0}' is a {1} and has no port A!", tag(), name());

            if (m_ioports < 2 && !(m_port_b_read_cb.isnull() && m_port_b_write_cb.isnull()))
                global.fatalerror("Device '{0}' is a {1} and has no port B!", tag(), name());

            m_port_a_read_cb.resolve();
            m_port_b_read_cb.resolve();
            m_port_a_write_cb.resolve();
            m_port_b_write_cb.resolve();

            if ((m_flags & AY8910_SINGLE_OUTPUT) != 0)
            {
                logerror("{0} device using single output!\n", name());
                m_streams = 1;
            }

            m_vol3d_table = new int[8*32*32*32];  // make_unique_clear<int32_t[]>(8*32*32*32);

            build_mixer_table();

            /* The envelope is pacing twice as fast for the YM2149 as for the AY-3-8910,    */
            /* This handled by the step parameter. Consequently we use a divider of 8 here. */
            m_channel = machine().sound().stream_alloc(this, 0, m_streams, master_clock / 8);

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
        //virtual void sound_stream_update(sound_stream &stream, stream_sample_t **inputs, stream_sample_t **outputs, int samples);


        // trampolines for callbacks from fm.cpp
        //static void psg_set_clock(device_t *device, int clock) { downcast<ay8910_device *>(device)->ay_set_clock(clock); }
        //static void psg_write(device_t *device, int address, int data) { downcast<ay8910_device *>(device)->ay8910_write_ym(address, data); }
        //static int psg_read(device_t *device) { return downcast<ay8910_device *>(device)->ay8910_read_ym(); }
        //static void psg_reset(device_t *device) { downcast<ay8910_device *>(device)->ay8910_reset_ym(); }


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
        }


        public uint16_t mix_3D()
        {
            int indx = 0;
            int chan;

            for (chan = 0; chan < NUM_CHANNELS; chan++)
            {
                if (TONE_ENVELOPE(chan) != 0)
                {
                    if (type() == ay8914_device.AY8914) // AY8914 Has a two bit tone_envelope field
                    {
                        indx |= (int)((1U << (chan + 15)) | ( m_vol_enabled[chan] != 0 ? ((m_env_volume >> (3 - TONE_ENVELOPE(chan))) << (chan*5)) : 0U));
                    }
                    else
                    {
                        indx |= (int)((1U << (chan + 15)) | ( m_vol_enabled[chan] != 0 ? m_env_volume << (chan * 5) : 0U));
                    }
                }
                else
                {
                    indx |= (m_vol_enabled[chan] != 0 ? TONE_VOLUME(chan) << (chan*5) : 0);
                }
            }

            return (UInt16)m_vol3d_table[indx];
        }


        void ay8910_write_reg(int r, int v)
        {
            //if (r >= 11 && r <= 13 ) printf("%d %x %02x\n", PSG->index, r, v);
            m_regs[r] = (byte)v;

            switch( r )
            {
                case AY_AFINE:
                case AY_ACOARSE:
                case AY_BFINE:
                case AY_BCOARSE:
                case AY_CFINE:
                case AY_CCOARSE:
                case AY_NOISEPER:
                case AY_AVOL:
                case AY_BVOL:
                case AY_CVOL:
                case AY_EFINE:
                    /* No action required */
                    break;
                case AY_ECOARSE:
                    if ( (v & 0x0f) > 0)
                        global.osd_printf_verbose("ECoarse\n");
                    /* No action required */
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
                case AY_ESHAPE:
                    if ( (v & 0x0f) > 0)
                        global.osd_printf_verbose("EShape\n");
                    m_attack = (m_regs[AY_ESHAPE] & 0x04) != 0 ? m_env_step_mask : (byte)0x00;
                    if ((m_regs[AY_ESHAPE] & 0x08) == 0)
                    {
                        /* if Continue = 0, map the shape to the equivalent one which has Continue = 1 */
                        m_hold = 1;
                        m_alternate = m_attack;
                    }
                    else
                    {
                        m_hold = (byte)(m_regs[AY_ESHAPE] & 0x01);
                        m_alternate = (byte)(m_regs[AY_ESHAPE] & 0x02);
                    }
                    m_env_step = (sbyte)m_env_step_mask;
                    m_holding = 0;
                    m_env_volume = (UInt32)(m_env_step ^ m_attack);
                    break;
                case AY_PORTA:
                    if ((m_regs[AY_ENABLE] & 0x40) != 0)
                    {
                        if (!m_port_a_write_cb.isnull())
                            m_port_a_write_cb.op((offs_t)0, m_regs[AY_PORTA]);
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
            }
        }


        void build_mixer_table()
        {
            int normalize = 0;
            int chan;

            if ((m_flags & AY8910_LEGACY_OUTPUT) != 0)
            {
                logerror("{0} using legacy output levels!\n", name());
                normalize = 1;
            }

            if ((m_flags & AY8910_RESISTOR_OUTPUT) != 0)
            {
                if (m_type != psg_type_t.PSG_TYPE_AY)
                    global.fatalerror("AY8910_RESISTOR_OUTPUT currently only supported for AY8910 devices.");

                for (chan=0; chan < NUM_CHANNELS; chan++)
                {
                    build_mosfet_resistor_table(ay8910_mosfet_param, m_res_load[chan], ref m_vol_table, chan);
                    build_mosfet_resistor_table(ay8910_mosfet_param, m_res_load[chan], ref m_env_table, chan);
                }
            }
            else if (m_streams == NUM_CHANNELS)
            {
                for (chan=0; chan < NUM_CHANNELS; chan++)
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
            save_item(m_active, "m_active");
            save_item(m_register_latch, "m_register_latch");
            save_item(m_regs, "m_regs");
            save_item(m_last_enable, "m_last_enable");

            save_item(m_count, "m_count");
            save_item(m_count_noise, "m_count_noise");
            save_item(m_count_env, "m_count_env");

            save_item(m_env_volume, "m_env_volume");

            save_item(m_output, "m_output");
            save_item(m_prescale_noise, "m_prescale_noise");

            save_item(m_env_step, "m_env_step");
            save_item(m_hold, "m_hold");
            save_item(m_alternate, "m_alternate");
            save_item(m_attack, "m_attack");
            save_item(m_holding, "m_holding");
            save_item(m_rng, "m_rng");
        }


        static void build_3D_table(double rl, ay_ym_param par, ay_ym_param par_env, int normalize, double factor, int zero_is_off, ref int [] tab)//, int32_t *tab)
        {
            double min = 10.0;
            double max = 0.0;

            std_vector<double> temp = new std_vector<double>(8*32*32*32, 0);

            for (int e=0; e < 8; e++)
            {
                ay_ym_param par_ch1 = (e & 0x01) != 0 ? par_env : par;
                ay_ym_param par_ch2 = (e & 0x02) != 0 ? par_env : par;
                ay_ym_param par_ch3 = (e & 0x04) != 0 ? par_env : par;

                for (int j1=0; j1 < par_ch1.res_count; j1++)
                {
                    for (int j2=0; j2 < par_ch2.res_count; j2++)
                    {
                        for (int j3=0; j3 < par_ch3.res_count; j3++)
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

                            int indx = (e << 15) | (j3<<10) | (j2<<5) | j1;
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
                for (int j=0; j < 32*32*32*8; j++)
                    tab[j] = (int)(MAX_OUTPUT * (((temp[j] - min)/(max-min))) * factor);
            }
            else
            {
                for (int j=0; j < 32*32*32*8; j++)
                    tab[j] = (int)(MAX_OUTPUT * temp[j]);
            }

            /* for (e=0;e<16;e++) printf("%d %d\n",e<<10, tab[e<<10]); */
        }


        static void build_single_table(double rl, ay_ym_param par, int normalize, ref int32_t [,] tab, int rank, int zero_is_off)//, int32_t *tab, int zero_is_off)
        {
            int j;
            double rt;
            double rw;
            double [] temp = new double[32];
            double min = 10.0;
            double max = 0.0;

            for (j=0; j < par.res_count; j++)
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
                for (j=0; j < par.res_count; j++)
                    tab[rank, j] = (int)(MAX_OUTPUT * (((temp[j] - min)/(max-min)) - 0.25) * 0.5);
            }
            else
            {
                for (j=0; j < par.res_count; j++)
                    tab[rank, j] = (int)(MAX_OUTPUT * temp[j]);
            }

        }


        static void build_mosfet_resistor_table(mosfet_param par, double rd, ref int32_t [,] tab, int rank)//, int32_t *tab)
        {
            int j;

            for (j=0; j < par.m_count; j++)
            {
                double Vd = 5.0;
                double Vg = par.m_Vg - par.m_Vth;
                double kn = par.m_Kn[j] / 1.0e6;
                double p2 = 1.0 / (2.0 * kn * rd) + Vg;
                double Vs = p2 - Math.Sqrt(p2 * p2 - Vg * Vg);

                double res = rd * ( Vd / Vs - 1.0);
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
        static device_t device_creator_ay8914_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new ay8914_device(mconfig, tag, owner, clock); }
        public static readonly device_type AY8914 = DEFINE_DEVICE_TYPE(device_creator_ay8914_device, "ay8914", "AY-3-8914A PSG");


        //ay8914_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock);
        ay8914_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, AY8914, tag, owner, clock, psg_type_t.PSG_TYPE_AY, 3, 2)
        {
        }


        /* AY8914 handlers needed due to different register map */
        //DECLARE_READ8_MEMBER( read );
        //DECLARE_WRITE8_MEMBER( write );
    }
}
