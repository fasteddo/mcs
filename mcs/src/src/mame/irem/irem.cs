// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.ay8910_global;
using static mame.device_global;
using static mame.diexec_global;
using static mame.discrete_global;
using static mame.disound_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.irem_global;
using static mame.m6801_global;
using static mame.msm5205_global;
using static mame.rescap_global;
using static mame.speaker_global;


namespace mame
{
    public class irem_audio_device : device_t
    {
        optional_device<netlist_mame_logic_input_device> m_audio_SINH;

        protected required_device<m6803_cpu_device> m_cpu;  //required_device<cpu_device> m_cpu;
        protected required_device<msm5205_device> m_adpcm1;
        optional_device<msm5205_device> m_adpcm2;
        protected required_device<ay8910_device> m_ay_45L;
        protected required_device<ay8910_device> m_ay_45M;


        // internal state
        uint8_t           m_port1;
        uint8_t           m_port2;

        uint8_t           m_soundlatch;

        optional_device<netlist_mame_logic_input_device> m_audio_BD;
        optional_device<netlist_mame_logic_input_device> m_audio_SD;
        optional_device<netlist_mame_logic_input_device> m_audio_OH;
        optional_device<netlist_mame_logic_input_device> m_audio_CH;


        protected irem_audio_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_audio_SINH = new optional_device<netlist_mame_logic_input_device>(this, "snd_nl:sinh");
            m_cpu = new required_device<m6803_cpu_device>(this, "iremsound");
            m_adpcm1 = new required_device<msm5205_device>(this, "msm1");
            m_adpcm2 = new optional_device<msm5205_device>(this, "msm2");
            m_ay_45L = new required_device<ay8910_device>(this, "ay_45l");
            m_ay_45M = new required_device<ay8910_device>(this, "ay_45m");
            m_port1 = 0;
            m_port2 = 0;
            m_soundlatch = 0;
            m_audio_BD = new optional_device<netlist_mame_logic_input_device>(this, "snd_nl:ibd");
            m_audio_SD = new optional_device<netlist_mame_logic_input_device>(this, "snd_nl:isd");
            m_audio_OH = new optional_device<netlist_mame_logic_input_device>(this, "snd_nl:ioh");
            m_audio_CH = new optional_device<netlist_mame_logic_input_device>(this, "snd_nl:ich");
        }


        /*************************************
         *
         *  External writes to the sound
         *  command register
         *
         *************************************/
        public void cmd_w(uint8_t data)
        {
            m_soundlatch = data;
            if ((data & 0x80) == 0)
                m_cpu.op0.set_input_line(0, ASSERT_LINE);
        }


        /*************************************
        *
        *  6803 output ports
        *
        *************************************/
        protected void m6803_port1_w(uint8_t data)
        {
            m_port1 = data;
        }


        protected void m6803_port2_w(uint8_t data)
        {
            /* write latch */
            if (((m_port2 & 0x01) != 0) && !((data & 0x01) != 0))
            {
                /* control or data port? */
                if ((m_port2 & 0x04) != 0)
                {
                    /* PSG 0 or 1? */
                    if ((m_port2 & 0x08) != 0)
                        m_ay_45M.op0.address_w(m_port1);
                    if ((m_port2 & 0x10) != 0)
                        m_ay_45L.op0.address_w(m_port1);
                }
                else
                {
                    /* PSG 0 or 1? */
                    if ((m_port2 & 0x08) != 0)
                        m_ay_45M.op0.data_w(m_port1);
                    if ((m_port2 & 0x10) != 0)
                        m_ay_45L.op0.data_w(m_port1);
                }
            }

            m_port2 = data;
        }


        /*************************************
         *
         *  6803 input ports
         *
         *************************************/
        protected uint8_t m6803_port1_r()
        {
            /* PSG 0 or 1? */
            if ((m_port2 & 0x08) != 0)
                return m_ay_45M.op0.data_r();
            if ((m_port2 & 0x10) != 0)
                return m_ay_45L.op0.data_r();

            return 0xff;
        }


        protected uint8_t m6803_port2_r()
        {
            /*
             * Pin21, 6803 (Port 21) tied with 4.7k to +5V
             *
             */
            //printf("port2 read\n"); // used by 10yard
            return 0x0;
        }


        /*************************************
         *
         *  Memory-mapped accesses
         *
         *************************************/
        protected void sound_irq_ack_w(uint8_t data)
        {
            if ((m_soundlatch & 0x80) != 0)
                m_cpu.op0.set_input_line(0, CLEAR_LINE);
        }


        protected void m52_adpcm_w(offs_t offset, uint8_t data)
        {
            if ((offset & 1) != 0)
            {
                m_adpcm1.op0.data_w(data);
            }

            if ((offset & 2) != 0)
            {
                if (m_adpcm2.op0 != null)
                    m_adpcm2.op0.data_w(data);
            }
        }


        //void m62_adpcm_w(offs_t offset, uint8_t data);


        /*************************************
        *
        *  Sound latch read
        *
        *************************************/
        protected uint8_t soundlatch_r()
        {
            return m_soundlatch;
        }


        /*************************************
         *
         *  AY-8910 output ports
         *
         *************************************/
        protected void ay8910_45M_portb_w(uint8_t data)
        {
            /* bits 2-4 select MSM5205 clock & 3b/4b playback mode */
            m_adpcm1.op0.playmode_w((data >> 2) & 7);
            if (m_adpcm2.op0 != null)
                m_adpcm2.op0.playmode_w(((data >> 2) & 4) | 3); /* always in slave mode */

            /* bits 0 and 1 reset the two chips */
            m_adpcm1.op0.reset_w(data & 1);
            if (m_adpcm2.op0 != null)
                m_adpcm2.op0.reset_w(data & 2);
        }


        protected void ay8910_45L_porta_w(uint8_t data)
        {
            /*
             *  45L 21 IOA0  ==> BD
             *  45L 20 IOA1  ==> SD
             *  45L 19 IOA2  ==> OH
             *  45L 18 IOA3  ==> CH
             *
             */
            if (m_audio_BD.op0 != null) m_audio_BD.op0.write_line(((data & 0x01) != 0) ? 1: 0);
            if (m_audio_SD.op0 != null) m_audio_SD.op0.write_line(((data & 0x02) != 0) ? 1: 0);
            if (m_audio_OH.op0 != null) m_audio_OH.op0.write_line(((data & 0x04) != 0) ? 1: 0);
            if (m_audio_CH.op0 != null) m_audio_CH.op0.write_line(((data & 0x08) != 0) ? 1: 0);
#if MAME_DEBUG
            if (data & 0x0f) popmessage("analog sound %x",data&0x0f);
#endif
        }


        //void irem_sound_portmap(address_map &map);
        //void m52_large_sound_map(address_map &map);


        static readonly double M52_R9      = 560;
        static readonly double M52_R10     = 330;
        static readonly double M52_R12     = RES_K(10);
        static readonly double M52_R13     = RES_K(10);
        static readonly double M52_R14     = RES_K(10);
        static readonly double M52_R15     = RES_K(2.2);  /* schematics RES_K(22) , althought 10-Yard states 2.2 */
        static readonly double M52_R19     = RES_K(10);
        static readonly double M52_R22     = RES_K(47);
        static readonly double M52_R23     = RES_K(2.2);
        static readonly double M52_R25     = RES_K(10);
        static readonly double M52_VR1     = RES_K(50);

        static readonly double M52_C28     = CAP_U(1);
        static readonly double M52_C30     = CAP_U(0.022);
        static readonly double M52_C32     = CAP_U(0.022);
        static readonly double M52_C35     = CAP_U(47);
        static readonly double M52_C37     = CAP_U(0.1);
        static readonly double M52_C38     = CAP_U(0.0068);


        static readonly discrete_mixer_desc m52_sound_c_stage1 = new discrete_mixer_desc
            (DISC_MIXER_IS_RESISTOR,
                new double [] {M52_R19, M52_R22, M52_R23 },
                new int []    {      0,       0,       0 },   /* variable resistors   */
                new double [] {M52_C37,       0,       0 },   /* node capacitors      */
                        0,      0,              /* rI, rF               */
                M52_C35*0,                      /* cF                   */
                0,                              /* cAmp                 */
                0, 1);

        static readonly discrete_op_amp_filt_info m52_sound_c_sallen_key = new discrete_op_amp_filt_info
            ( M52_R13, M52_R14, 0, 0, 0,
                M52_C32, M52_C38, 0
            );

        static readonly discrete_mixer_desc m52_sound_c_mix1 = new discrete_mixer_desc
            (DISC_MIXER_IS_RESISTOR,
                new double [] {M52_R25, M52_R15 },
                new int []    {      0,       0 },    /* variable resistors   */
                new double [] {      0,       0 },    /* node capacitors      */
                        0, M52_VR1,     /* rI, rF               */
                0,                      /* cF                   */
                CAP_U(1),               /* cAmp                 */
                0, 1);


        //static DISCRETE_SOUND_START( m52_sound_c_discrete )
        protected static readonly discrete_block [] m52_sound_c_discrete = 
        {
            /* Chip AY8910/1 */
            DISCRETE_INPUTX_STREAM(NODE_01, 0, 1.0, 0),
            /* Chip AY8910/2 */
            DISCRETE_INPUTX_STREAM(NODE_02, 1, 1.0, 0),
            /* Chip MSM5250 */
            DISCRETE_INPUTX_STREAM(NODE_03, 2, 1.0, 0),

            /* Just mix the two AY8910s */
            DISCRETE_ADDER2(NODE_09, 1, NODE_01, NODE_02),
            DISCRETE_DIVIDE(NODE_10, 1, NODE_09, 2.0),

            /* Mix in 5 V to MSM5250 signal */
            DISCRETE_MIXER3(NODE_20, 1, NODE_03, 32767.0, 0, m52_sound_c_stage1),

            /* Sallen - Key Filter */
            /* TODO: R12, C30: This looks like a band pass */
            DISCRETE_RCFILTER(NODE_25, NODE_20, M52_R12, M52_C30),
            DISCRETE_SALLEN_KEY_FILTER(NODE_30, 1, NODE_25, DISC_SALLEN_KEY_LOW_PASS, m52_sound_c_sallen_key),

            /* Mix signals */
            DISCRETE_MIXER2(NODE_40, 1, NODE_10, NODE_25, m52_sound_c_mix1),
            DISCRETE_CRFILTER(NODE_45, NODE_40, M52_R10+M52_R9, M52_C28),

            DISCRETE_OUTPUT(NODE_40, 18.0),

            DISCRETE_SOUND_END,
        };


        /*************************************
         *
         *  Address maps
         *
         *************************************/

        /* complete address map verified from Moon Patrol/10 Yard Fight schematics */
        /* large map uses 8k ROMs, small map uses 4k ROMs; this is selected via a jumper */
        protected void m52_small_sound_map(address_map map, device_t device)
        {
            map.global_mask(0x7fff);
            map.op(0x0000, 0x0fff).w(m52_adpcm_w);
            map.op(0x1000, 0x1fff).w(sound_irq_ack_w);
            map.op(0x2000, 0x7fff).rom();
        }


        //void m62_sound_map(address_map &map);


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            save_item(NAME(new { m_port1 }));
            save_item(NAME(new { m_port2 }));
            save_item(NAME(new { m_soundlatch }));
        }

        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            m_port1 = 0; // ?
            m_port2 = 0; // ?
            m_soundlatch = 0;
            m_cpu.op0.set_input_line(0, ASSERT_LINE);
        }
    }


    public class m52_soundc_audio_device : irem_audio_device
    {
        //DEFINE_DEVICE_TYPE(IREM_M52_SOUNDC_AUDIO, m52_soundc_audio_device, "m52_soundc_audio", "Irem M52 SoundC Audio")
        public static readonly emu.detail.device_type_impl IREM_M52_SOUNDC_AUDIO = DEFINE_DEVICE_TYPE("m52_soundc_audio", "Irem M52 SoundC Audio", (type, mconfig, tag, owner, clock) => { return new m52_soundc_audio_device(mconfig, tag, owner, clock); });


        m52_soundc_audio_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, IREM_M52_SOUNDC_AUDIO, tag, owner, clock)
        {
        }


        protected override void device_add_mconfig(machine_config config)
        {
            /* basic machine hardware */
            m6803_cpu_device cpu = M6803(config, m_cpu, new XTAL(3579545)); /* verified on pcb */
            cpu.memory().set_addrmap(AS_PROGRAM, m52_small_sound_map);
            cpu.in_p1_cb().set(m6803_port1_r).reg();
            cpu.out_p1_cb().set(m6803_port1_w).reg();
            cpu.in_p2_cb().set(m6803_port2_r).reg();
            cpu.out_p2_cb().set(m6803_port2_w).reg();

            /* sound hardware */
            SPEAKER(config, "mono").front_center();

            AY8910(config, m_ay_45M, new XTAL(3579545)/4); /* verified on pcb */
            m_ay_45M.op0.set_flags(ay8910_device.AY8910_SINGLE_OUTPUT | AY8910_DISCRETE_OUTPUT);
            m_ay_45M.op0.set_resistors_load(470, 0, 0);
            m_ay_45M.op0.port_a_read_callback().set(soundlatch_r).reg();
            m_ay_45M.op0.port_b_write_callback().set(ay8910_45M_portb_w).reg();
            m_ay_45M.op0.add_route(0, "filtermix", 1.0, 0);

            AY8910(config, m_ay_45L, new XTAL(3579545)/4); /* verified on pcb */
            m_ay_45L.op0.set_flags(ay8910_device.AY8910_SINGLE_OUTPUT | AY8910_DISCRETE_OUTPUT);
            m_ay_45L.op0.set_resistors_load(470, 0, 0);
            m_ay_45L.op0.port_a_write_callback().set(ay8910_45L_porta_w).reg();
            m_ay_45L.op0.add_route(0, "filtermix", 1.0, 1);

            MSM5205(config, m_adpcm1, new XTAL(384000)); /* verified on pcb */
            m_adpcm1.op0.vck_callback().set_inputline(m_cpu, INPUT_LINE_NMI).reg(); // driven through NPN inverter
            m_adpcm1.op0.set_prescaler_selector(msm5205_device.S96_4B);      /* default to 4KHz, but can be changed at run time */
            m_adpcm1.op0.add_route(0, "filtermix", 1.0, 2);

            DISCRETE(config, "filtermix", m52_sound_c_discrete).disound.add_route(ALL_OUTPUTS, "mono", 1.0);
        }
    }


    static class irem_global
    {
        public static m52_soundc_audio_device IREM_M52_SOUNDC_AUDIO(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<m52_soundc_audio_device>(mconfig, tag, m52_soundc_audio_device.IREM_M52_SOUNDC_AUDIO, clock); }
    }
}
