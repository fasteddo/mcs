// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_timer_id = System.UInt32;
using device_type = mame.emu.detail.device_type_impl_base;
using int32_t = System.Int32;
using offs_t = System.UInt32;
using stream_sample_t = System.Int32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    // ======================> pokey_device
    public class pokey_device : device_t
                                //device_sound_interface,
                                //device_execute_interface,
                                //device_state_interface
    {
        //DEFINE_DEVICE_TYPE(POKEY, pokey_device, "pokey", "Atari C012294 POKEY")
        static device_t device_creator_pokey_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new pokey_device(mconfig, tag, owner, clock); }
        public static readonly device_type POKEY = DEFINE_DEVICE_TYPE(device_creator_pokey_device, "pokey", "Atari C012294 POKEY");


        public class device_sound_interface_pokey : device_sound_interface
        {
            public device_sound_interface_pokey(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples) { ((pokey_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs, samples); }
        }


        public class device_execute_interface_pokey : device_execute_interface
        {
            public device_execute_interface_pokey(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void execute_run() { ((pokey_device)device()).device_execute_interface_execute_run(); }
        }


        public class device_state_interface_pokey : device_state_interface
        {
            public device_state_interface_pokey(machine_config mconfig, device_t device) : base(mconfig, device) { }
        }


        /* CONSTANT DEFINITIONS */

        /* exact 1.79 MHz clock freq (of the Atari 800 that is) */
        //static constexpr unsigned FREQ_17_EXACT = 1789790;


        //enum
        //{
        const uint8_t POK_KEY_BREAK = 0x30;
        const uint8_t POK_KEY_SHIFT = 0x20;
        const uint8_t POK_KEY_CTRL  = 0x00;
        //}

        //enum
        //{
        /* POKEY WRITE LOGICALS */
        const int AUDF1_C  =   0x00;
        const int AUDC1_C  =   0x01;
        const int AUDF2_C  =   0x02;
        const int AUDC2_C  =   0x03;
        const int AUDF3_C  =   0x04;
        const int AUDC3_C  =   0x05;
        const int AUDF4_C  =   0x06;
        const int AUDC4_C  =   0x07;
        const int AUDCTL_C =   0x08;
        const int STIMER_C =   0x09;
        const int SKREST_C =   0x0A;
        const int POTGO_C  =   0x0B;
        const int SEROUT_C =   0x0D;
        const int IRQEN_C  =   0x0E;
        const int SKCTL_C  =   0x0F;
        //}

        //enum
        //{
        /* POKEY READ LOGICALS */
        const int POT0_C   =  0x00;
        const int POT1_C   =  0x01;
        const int POT2_C   =  0x02;
        const int POT3_C   =  0x03;
        const int POT4_C   =  0x04;
        const int POT5_C   =  0x05;
        const int POT6_C   =  0x06;
        const int POT7_C   =  0x07;
        const int ALLPOT_C =  0x08;
        const int KBCODE_C =  0x09;
        const int RANDOM_C =  0x0A;
        const int SERIN_C  =  0x0D;
        const int IRQST_C  =  0x0E;
        const int SKSTAT_C =  0x0F;
        //}

        //enum  /* sync-operations */
        //{
        const int SYNC_NOOP       = 11;
        const int SYNC_SET_IRQST  = 12;
        const int SYNC_POT        = 13;
        const int SYNC_WRITE      = 14;
        //}


        enum output_type
        {
            LEGACY_LINEAR = 0,
            RC_LOWPASS,
            OPAMP_C_TO_GROUND,
            OPAMP_LOW_PASS,
            DISCRETE_VAR_R
        }


        class pokey_channel
        {
            public pokey_device m_parent;
            public uint8_t m_INTMask;
            public uint8_t m_AUDF;           /* AUDFx (D200, D202, D204, D206) */
            public uint8_t m_AUDC;           /* AUDCx (D201, D203, D205, D207) */
            int32_t m_borrow_cnt;     /* borrow counter */
            int32_t m_counter;        /* channel counter */
            public uint8_t m_output;         /* channel output signal (1 active, 0 inactive) */
            public uint8_t m_filter_sample;  /* high-pass filter sample */


            public pokey_channel()
            {
                m_AUDF = 0;
                m_AUDC = 0;
                m_borrow_cnt = 0;
                m_counter = 0;
                m_output = 0;
                m_filter_sample = 0;
            }


            public void sample()            { m_filter_sample = m_output; }
            public void reset_channel()     { m_counter = m_AUDF ^ 0xff; }

            public void inc_chan()
            {
                m_counter = (m_counter + 1) & 0xff;
                if (m_counter == 0 && m_borrow_cnt == 0)
                {
                    m_borrow_cnt = 3;
                    if ((m_parent.m_IRQEN & m_INTMask) != 0)
                    {
                        /* Exposed state has changed: This should only be updated after a resync ... */
                        m_parent.synchronize(SYNC_SET_IRQST, m_INTMask);
                    }
                }
            }


            public int check_borrow()
            {
                if (m_borrow_cnt > 0)
                {
                    m_borrow_cnt--;
                    return (m_borrow_cnt == 0) ? 1 : 0;
                }

                return 0;
            }
        }


        const int POKEY_CHANNELS = 4;


        const int POKEY_DEFAULT_GAIN = (32767/11/4);


        const bool VERBOSE         = false;
        const bool VERBOSE_SOUND   = false;
        const bool VERBOSE_TIMER   = false;
        const bool VERBOSE_POLY    = false;
        const bool VERBOSE_RAND    = false;

        void LOG(string format, params object [] args) { if (VERBOSE) logerror(format, args); }
        void LOG_SOUND(string format, params object [] args) { if (VERBOSE_SOUND) logerror(format, args); }
        void LOG_TIMER(string format, params object [] args) { if (VERBOSE_TIMER) logerror(format, args); }
        void LOG_POLY(string format, params object [] args) { if (VERBOSE_POLY) logerror(format, args); }
        void LOG_RAND(string format, params object [] args) { if (VERBOSE_RAND) logerror(format, args); }


        const int CHAN1   = 0;
        const int CHAN2   = 1;
        const int CHAN3   = 2;
        const int CHAN4   = 3;


        /* AUDCx */
        const int NOTPOLY5    = 0x80;    /* selects POLY5 or direct CLOCK */
        const int POLY4       = 0x40;    /* selects POLY4 or POLY17 */
        const int PURE        = 0x20;    /* selects POLY4/17 or PURE tone */
        const int VOLUME_ONLY = 0x10;    /* selects VOLUME OUTPUT ONLY */
        const int VOLUME_MASK = 0x0f;    /* volume mask */

        /* AUDCTL */
        const int POLY9       = 0x80;    /* selects POLY9 or POLY17 */
        const int CH1_HICLK   = 0x40;    /* selects 1.78979 MHz for Ch 1 */
        const int CH3_HICLK   = 0x20;    /* selects 1.78979 MHz for Ch 3 */
        const int CH12_JOINED = 0x10;    /* clocks channel 1 w/channel 2 */
        const int CH34_JOINED = 0x08;    /* clocks channel 3 w/channel 4 */
        const int CH1_FILTER  = 0x04;    /* selects channel 1 high pass filter */
        const int CH2_FILTER  = 0x02;    /* selects channel 2 high pass filter */
        const int CLK_15KHZ   = 0x01;    /* selects 15.6999 kHz or 63.9211 kHz */

        /* IRQEN (D20E) */
        const int IRQ_BREAK   = 0x80;    /* BREAK key pressed interrupt */
        const int IRQ_KEYBD   = 0x40;    /* keyboard data ready interrupt */
        const int IRQ_SERIN   = 0x20;    /* serial input data ready interrupt */
        const int IRQ_SEROR   = 0x10;    /* serial output register ready interrupt */
        const int IRQ_SEROC   = 0x08;    /* serial output complete interrupt */
        const int IRQ_TIMR4   = 0x04;    /* timer channel #4 interrupt */
        const int IRQ_TIMR2   = 0x02;    /* timer channel #2 interrupt */
        const int IRQ_TIMR1   = 0x01;    /* timer channel #1 interrupt */

        /* SKSTAT (R/D20F) */
        const int SK_FRAME    = 0x80;    /* serial framing error */
        const int SK_KBERR    = 0x40;    /* keyboard overrun error - pokey documentation states *some bit as IRQST */
        const int SK_OVERRUN  = 0x20;    /* serial overrun error - pokey documentation states *some bit as IRQST */
        const int SK_SERIN    = 0x10;    /* serial input high */
        const int SK_SHIFT    = 0x08;    /* shift key pressed */
        const int SK_KEYBD    = 0x04;    /* keyboard key pressed */
        const int SK_SEROUT   = 0x02;    /* serial output active */

        /* SKCTL (W/D20F) */
        const int SK_BREAK    = 0x80;    /* serial out break signal */
        const int SK_BPS      = 0x70;    /* bits per second */
        const int SK_FM       = 0x08;    /* FM mode */
        const int SK_PADDLE   = 0x04;    /* fast paddle a/d conversion */
        const int SK_RESET    = 0x03;    /* reset serial/keyboard interface */
        const int SK_KEYSCAN  = 0x02;    /* key scanning enabled ? */
        const int SK_DEBOUNCE = 0x01;    /* Debouncing ?*/


        const int DIV_64      = 28;       /* divisor for 1.78979 MHz clock to 63.9211 kHz */
        const int DIV_15      = 114;      /* divisor for 1.78979 MHz clock to 15.6999 kHz */


        const int CLK_1 = 0;
        const int CLK_28 = 1;
        const int CLK_114 = 2;


        //constexpr unsigned pokey_device::FREQ_17_EXACT;


        device_sound_interface_pokey m_disound;
        device_execute_interface_pokey m_diexec;
        device_state_interface_pokey m_distate;


        // other internal states
        intref m_icountRef = new intref();  //int m_icount;


        // internal state
        sound_stream m_stream;

        pokey_channel [] m_channel = new pokey_channel[POKEY_CHANNELS];

        uint32_t m_out_raw;        /* raw output */
        bool m_old_raw_inval;       /* true: recalc m_out_raw required */
        double m_out_filter;    /* filtered output */

        int [] m_clock_cnt = new int[3];       /* clock counters */
        uint32_t m_p4;              /* poly4 index */
        uint32_t m_p5;              /* poly5 index */
        uint32_t m_p9;              /* poly9 index */
        uint32_t m_p17;             /* poly17 index */

        devcb_read8.array<i8, devcb_read8> m_pot_r_cb;
        devcb_read8 m_allpot_r_cb;
        devcb_read8 m_serin_r_cb;
        devcb_write8 m_serout_w_cb;

        kb_cb_delegate m_keyboard_r;
        int_cb_delegate m_irq_f;

        uint8_t [] m_POTx = new uint8_t[8];        /* POTx   (R/D200-D207) */
        uint8_t m_AUDCTL;         /* AUDCTL (W/D208) */
        uint8_t m_ALLPOT;         /* ALLPOT (R/D208) */
        uint8_t m_KBCODE;         /* KBCODE (R/D209) */
        uint8_t m_SERIN;          /* SERIN  (R/D20D) */
        uint8_t m_SEROUT;         /* SEROUT (W/D20D) */
        uint8_t m_IRQST;          /* IRQST  (R/D20E) */
        uint8_t m_IRQEN;          /* IRQEN  (W/D20E) */
        uint8_t m_SKSTAT;         /* SKSTAT (R/D20F) */
        uint8_t m_SKCTL;          /* SKCTL  (W/D20F) */

        uint8_t m_pot_counter;
        uint8_t m_kbd_cnt;
        uint8_t m_kbd_latch;
        uint8_t m_kbd_state;

        attotime m_clock_period;

        uint32_t [] m_poly4 = new uint32_t[0x0f];
        uint32_t [] m_poly5 = new uint32_t[0x1f];
        uint32_t [] m_poly9 = new uint32_t[0x1ff];
        uint32_t [] m_poly17 = new uint32_t[0x1ffff];
        uint32_t [] m_voltab = new uint32_t[0x10000];

        output_type m_output_type;
        double m_r_pullup;
        double m_cap;
        double m_v_ref;


        // construction/destruction
        //-------------------------------------------------
        //  pokey_device - constructor
        //-------------------------------------------------
        pokey_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, POKEY, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_pokey(mconfig, this));  //device_sound_interface(mconfig, *this),
            m_class_interfaces.Add(new device_execute_interface_pokey(mconfig, this));  //device_execute_interface(mconfig, *this),
            m_class_interfaces.Add(new device_state_interface_pokey(mconfig, this));  //device_state_interface(mconfig, *this),
            m_disound = GetClassInterface<device_sound_interface_pokey>();
            m_diexec = GetClassInterface<device_execute_interface_pokey>();
            m_distate = GetClassInterface<device_state_interface_pokey>();


            m_icountRef.i = 0;  //m_icount = 0;
            m_stream = null;
            m_pot_r_cb = new devcb_read8.array<i8, devcb_read8>(this, () => { return new devcb_read8(this); });
            m_allpot_r_cb = new devcb_read8(this);
            m_serin_r_cb = new devcb_read8(this);
            m_serout_w_cb = new devcb_write8(this);
            m_keyboard_r = null;
            m_irq_f = null;
            m_output_type = output_type.LEGACY_LINEAR;
        }


        public device_sound_interface_pokey disound { get { return m_disound; } }


        //template <unsigned N> auto pot_r() { return m_pot_r_cb[N].bind(); }
        //auto allpot_r() { return m_allpot_r_cb.bind(); }
        //auto serin_r() { return m_serin_r_cb.bind(); }
        //auto serout_w() { return m_serout_w_cb.bind(); }


        /* k543210 = k5 ... k0 returns bit0: kr1, bit1: kr2 */
        /* all are, in contrast to actual hardware, ACTIVE_HIGH */

        //typedef device_delegate<uint8_t (uint8_t k543210)> kb_cb_delegate;
        delegate uint8_t kb_cb_delegate(uint8_t k543210);

        //template <typename... T> void set_keyboard_callback(T &&... args) { m_keyboard_r.set(std::forward<T>(args)...); }

        //typedef device_delegate<void (int mask)> int_cb_delegate;
        delegate void int_cb_delegate(int mask);

        //template <typename... T> void set_interrupt_callback(T &&... args) { m_irq_f.set(std::forward<T>(args)...); }


        //-------------------------------------------------
        //  read - memory interface for reading the active status
        //-------------------------------------------------
        public uint8_t read(offs_t offset)
        {
            int data;
            int pot;

            synchronize(SYNC_NOOP); /* force resync */

            switch (offset & 15)
            {
            case POT0_C: case POT1_C: case POT2_C: case POT3_C:
            case POT4_C: case POT5_C: case POT6_C: case POT7_C:
                pot = (int)(offset & 7);
                if ((m_ALLPOT & (1 << pot)) != 0)
                {
                    /* we have a value measured */
                    data = m_POTx[pot];
                    LOG("POKEY '{0}' read POT{1} (final value)  {2}\n", tag(), pot, data);  // $%02x
                }
                else
                {
                    data = m_pot_counter;
                    LOG("POKEY '{0}' read POT{1} (interpolated) {2}\n", tag(), pot, data);
                }
                break;

            case ALLPOT_C:
                /****************************************************************
                 * If the 2 least significant bits of SKCTL are 0, the ALLPOTs
                 * are disabled (SKRESET). Thanks to MikeJ for pointing this out.
                 ****************************************************************/
                if ((m_SKCTL & SK_RESET) == 0)
                {
                    data = m_ALLPOT;
                    LOG("POKEY '{0}' ALLPOT internal {1} (reset)\n", tag(), data);
                }
                else if (!m_allpot_r_cb.isnull())
                {
                    data = m_allpot_r_cb.op(offset);
                    m_ALLPOT = (uint8_t)data;
                    LOG("{0}: POKEY '{1}' ALLPOT callback {2}\n", machine().describe_context(), tag(), data);
                }
                else
                {
                    data = m_ALLPOT ^ 0xff;
                    LOG("POKEY '{0}' ALLPOT internal {1}\n", tag(), data);
                }
                break;

            case KBCODE_C:
                data = m_KBCODE;
                break;

            case RANDOM_C:
                if ((m_AUDCTL & POLY9) != 0)
                {
                    data = (int)(m_poly9[m_p9] & 0xff);
                    LOG_RAND("POKEY '{0}' rand9[{1}]: {2}\n", tag(), m_p9, data);  // $%05x  $%02x
                }
                else
                {
                    data = (int)((m_poly17[m_p17] >> 8) & 0xff);
                    LOG_RAND("POKEY '{0}' rand17[{1}]: {2}\n", tag(), m_p17, data);
                }
                break;

            case SERIN_C:
                if (!m_serin_r_cb.isnull())
                    m_SERIN = m_serin_r_cb.op(offset);
                data = m_SERIN;
                LOG("POKEY '{0}' SERIN  {1}\n", tag(), data);
                break;

            case IRQST_C:
                /* IRQST is an active low input port; we keep it active high */
                /* internally to ease the (un-)masking of bits */
                data = m_IRQST ^ 0xff;
                LOG("POKEY '{0}' IRQST  {1}\n", tag(), data);
                break;

            case SKSTAT_C:
                /* SKSTAT is also an active low input port */
                data = m_SKSTAT ^ 0xff;
                LOG("POKEY '{0}' SKSTAT {1}\n", tag(), data);
                break;

            default:
                LOG("POKEY '{0}' register {2}\n", tag(), offset);
                data = 0xff;
                break;
            }

            return (byte)data;
        }


        //-------------------------------------------------
        //  write - memory interface for write
        //-------------------------------------------------
        public void write(offs_t offset, uint8_t data)
        {
            synchronize(SYNC_WRITE, (int)((offset << 8) | data));
        }


        //DECLARE_WRITE_LINE_MEMBER( sid_w ); // pin 24
        //void serin_ready(int after);


        // analog output configuration
        //void set_output_rc(double r, double c, double v)
        //{
        //    m_output_type = pokey_device::RC_LOWPASS;
        //    m_r_pullup = r;
        //    m_cap = c;
        //    m_v_ref = v;
        //}

        /* C ignored, please see pokey.c */
        //void set_output_opamp(double r, double c, double v)
        //{
        //    m_output_type = pokey_device::OPAMP_C_TO_GROUND;
        //    m_r_pullup = r;
        //    m_cap = c;
        //    m_v_ref = v;
        //}

        public void set_output_opamp_low_pass(double r, double c, double v)
        {
            m_output_type = pokey_device.output_type.OPAMP_LOW_PASS;
            m_r_pullup = r;
            m_cap = c;
            m_v_ref = v;
        }

        //void set_output_discrete()
        //{
        //    m_output_type = pokey_device::DISCRETE_VAR_R;
        //}


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            //int sample_rate = clock();

            /* Setup channels */
            for (int i = 0; i < POKEY_CHANNELS; i++)
            {
                m_channel[i] = new pokey_channel();

                m_channel[i].m_parent = this;
                m_channel[i].m_INTMask = 0;
            }
            m_channel[CHAN1].m_INTMask = IRQ_TIMR1;
            m_channel[CHAN2].m_INTMask = IRQ_TIMR2;
            m_channel[CHAN4].m_INTMask = IRQ_TIMR4;

            // bind callbacks
            //throw new emu_unimplemented();
#if false
            m_keyboard_r.resolve();
            m_irq_f.resolve();
#endif

            /* calculate the A/D times
             * In normal, slow mode (SKCTL bit SK_PADDLE is clear) the conversion
             * takes N scanlines, where N is the paddle value. A single scanline
             * takes approximately 64us to finish (1.78979MHz clock).
             * In quick mode (SK_PADDLE set) the conversion is done very fast
             * (takes two scanlines) but the result is not as accurate.
             */

            /* initialize the poly counters */
            poly_init_4_5(m_poly4, 4, 1, 0);
            poly_init_4_5(m_poly5, 5, 2, 1);

            /* initialize 9 / 17 arrays */
            poly_init_9_17(m_poly9,   9);
            poly_init_9_17(m_poly17, 17);
            vol_init();

            /* The pokey does not have a reset line. These should be initialized
             * with random values.
             */

            m_KBCODE = 0x09;         /* Atari 800 'no key' */
            m_SKCTL = SK_RESET;  /* let the RNG run after reset */
            m_SKSTAT = 0;
            /* This bit should probably get set later. Acid5200 pokey_setoc test tests this. */
            m_IRQST = IRQ_SEROC;
            m_IRQEN = 0;
            m_AUDCTL = 0;
            m_p4 = 0;
            m_p5 = 0;
            m_p9 = 0;
            m_p17 = 0;
            m_ALLPOT = 0x00;

            m_pot_counter = 0;
            m_kbd_cnt = 0;
            m_out_filter = 0;
            m_out_raw = 0;
            m_old_raw_inval = true;
            m_kbd_state = 0;

            /* reset more internal state */
            std.fill(m_clock_cnt, 0);  //std::fill(std::begin(m_clock_cnt), std::end(m_clock_cnt), 0);
            std.fill<uint8_t>(m_POTx, 0);
            
            m_pot_r_cb.resolve_all();
            m_allpot_r_cb.resolve();
            m_serin_r_cb.resolve();
            m_serout_w_cb.resolve_safe();

            m_stream = m_disound.stream_alloc(0, 1, (int)clock());

            timer_alloc(SYNC_WRITE);    /* timer for sync operation */
            timer_alloc(SYNC_NOOP);
            timer_alloc(SYNC_POT);
            timer_alloc(SYNC_SET_IRQST);

            for (int i=0; i<POKEY_CHANNELS; i++)
            {
                //throw new emu_unimplemented();
#if false
                save_item(NAME(m_channel[i].m_borrow_cnt), i);
                save_item(NAME(m_channel[i].m_counter), i);
                save_item(NAME(m_channel[i].m_filter_sample), i);
                save_item(NAME(m_channel[i].m_output), i);
                save_item(NAME(m_channel[i].m_AUDF), i);
                save_item(NAME(m_channel[i].m_AUDC), i);
#endif
            }

            save_item(m_clock_cnt, "m_clock_cnt");
            save_item(m_p4, "m_p4");
            save_item(m_p5, "m_p5");
            save_item(m_p9, "m_p9");
            save_item(m_p17, "m_p17");

            save_item(m_POTx, "m_POTx");
            save_item(m_AUDCTL, "m_AUDCTL");
            save_item(m_ALLPOT, "m_ALLPOT");
            save_item(m_KBCODE, "m_KBCODE");
            save_item(m_SERIN, "m_SERIN");
            save_item(m_SEROUT, "m_SEROUT");
            save_item(m_IRQST, "m_IRQST");
            save_item(m_IRQEN, "m_IRQEN");
            save_item(m_SKSTAT, "m_SKSTAT");
            save_item(m_SKCTL, "m_SKCTL");

            save_item(m_pot_counter, "m_pot_counter");
            save_item(m_kbd_cnt, "m_kbd_cnt");
            save_item(m_kbd_latch, "m_kbd_latch");
            save_item(m_kbd_state, "m_kbd_state");

            // State support

            m_distate.state_add(AUDF1_C, "AUDF1", m_channel[0].m_AUDF);
            m_distate.state_add(AUDC1_C, "AUDC1", m_channel[0].m_AUDC);
            m_distate.state_add(AUDF2_C, "AUDF2", m_channel[1].m_AUDF);
            m_distate.state_add(AUDC2_C, "AUDC2", m_channel[1].m_AUDC);
            m_distate.state_add(AUDF3_C, "AUDF3", m_channel[2].m_AUDF);
            m_distate.state_add(AUDC3_C, "AUDC3", m_channel[2].m_AUDC);
            m_distate.state_add(AUDF4_C, "AUDF4", m_channel[3].m_AUDF);
            m_distate.state_add(AUDC4_C, "AUDC4", m_channel[3].m_AUDC);
            m_distate.state_add(AUDCTL_C, "AUDCTL", m_AUDCTL);
#if false
            state_add(STIMER_C, "STIMER", m_STIMER);
            state_add(SKREST_C, "SKREST_C", m_SKREST);
            state_add(POTGO_C, "POTGO", m_POTGO_C);
#endif
            m_distate.state_add(SEROUT_C, "SEROUT", m_SEROUT);
            m_distate.state_add(IRQEN_C, "IRQEN", m_IRQEN);
            m_distate.state_add(SKCTL_C, "SKCTL", m_SKCTL);

            // set our instruction counter
            set_icountptr(m_icountRef);
        }


        protected override void device_reset()
        {
            m_stream.update();
        }


        protected override void device_post_load()
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  device_clock_changed - called if the clock
        //  changes
        //-------------------------------------------------
        protected override void device_clock_changed()
        {
            m_clock_period = clocks_to_attotime(1);

            if (clock() != 0)
            {
                if (m_stream != null)
                    m_stream.set_sample_rate((int)clock());
                else
                    m_stream = m_disound.stream_alloc(0, 1, (int)clock());
            }
        }


        protected override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr)
        {
            switch (id)
            {
            case 3:
                /* serout_ready_cb */
                if ((m_IRQEN & IRQ_SEROR) != 0)
                {
                    m_IRQST |= IRQ_SEROR;
                    if (m_irq_f != null)
                        m_irq_f(IRQ_SEROR);
                }
                break;
            case 4:
                /* serout_complete */
                if ((m_IRQEN & IRQ_SEROC) != 0)
                {
                    m_IRQST |= IRQ_SEROC;
                    if (m_irq_f != null)
                        m_irq_f(IRQ_SEROC);
                }
                break;
            case 5:
                /* serin_ready */
                if ((m_IRQEN & IRQ_SERIN) != 0)
                {
                    m_IRQST |= IRQ_SERIN;
                    if (m_irq_f != null)
                        m_irq_f(IRQ_SERIN);
                }
                break;
            case SYNC_WRITE:
                {
                    offs_t offset = (offs_t)((param >> 8) & 0xff);
                    byte data = (byte)(param & 0xff);
                    write_internal(offset, data);
                }
                break;
            case SYNC_NOOP:
                /* do nothing, caused by a forced resync */
                break;
            case SYNC_POT:
                //logerror("x %02x \n", (param & 0x20));
                m_ALLPOT |= (byte)(param & 0xff);
                break;
            case SYNC_SET_IRQST:
                m_IRQST |=  (byte)(param & 0xff);
                break;
            default:
                throw new emu_fatalerror("Unknown id in pokey_device::device_timer");
            }
        }


        // device_sound_interface overrides
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        void device_sound_interface_sound_stream_update(sound_stream stream, ListPointer<stream_sample_t> [] inputs, ListPointer<stream_sample_t> [] outputs, int samples)
        {
            var buffer = new ListPointer<stream_sample_t>(outputs[0]);  //stream_sample_t *buffer = outputs[0];

            if (m_output_type == output_type.LEGACY_LINEAR)
            {
                int out_ = 0;
                for (int i = 0; i < 4; i++)
                    out_ += (int)((m_out_raw >> (4*i)) & 0x0f);
                out_ *= POKEY_DEFAULT_GAIN;
                out_ = (out_ > 0x7fff) ? 0x7fff : out_;
                while( samples > 0 )
                {
                    buffer[0] = out_;  //*buffer++ = out_;
                    buffer++;
                    samples--;
                }
            }
            else if (m_output_type == output_type.RC_LOWPASS)
            {
                double rTot = m_voltab[m_out_raw];

                double V0 = rTot / (rTot + m_r_pullup) * m_v_ref / 5.0 * 32767.0;
                double mult = (m_cap == 0.0) ? 1.0 : 1.0 - Math.Exp(-(rTot + m_r_pullup) / (m_cap * m_r_pullup * rTot) * m_clock_period.as_double());

                while( samples > 0 )
                {
                    /* store sum of output signals into the buffer */
                    m_out_filter += (V0 - m_out_filter) * mult;
                    buffer[0] = (int)m_out_filter;  //*buffer++ = pokey.m_out_filter;
                    buffer++;
                    samples--;

                }
            }
            else if (m_output_type == output_type.OPAMP_C_TO_GROUND)
            {
                double rTot = m_voltab[m_out_raw];
                /* In this configuration there is a capacitor in parallel to the pokey output to ground.
                    * With a LM324 in LTSpice this causes the opamp circuit to oscillate at around 100 kHz.
                    * We are ignoring the capacitor here, since this oscillation would not be audible.
                    */

                /* This post-pokey stage usually has a high-pass filter behind it
                    * It is approximated by eliminating m_v_ref ( -1.0 term)
                    */

                double V0 = ((rTot + m_r_pullup) / rTot - 1.0) * m_v_ref  / 5.0 * 32767.0;

                while (samples > 0)
                {
                    /* store sum of output signals into the buffer */
                    buffer[0] = (int)V0;  //*buffer++ = V0;
                    buffer++;
                    samples--;

                }
            }
            else if (m_output_type == output_type.OPAMP_LOW_PASS)
            {
                double rTot = m_voltab[m_out_raw];
                /* This post-pokey stage usually has a low-pass filter behind it
                    * It is approximated by not adding in VRef below.
                    */

                double V0 = (m_r_pullup / rTot) * m_v_ref  / 5.0 * 32767.0;
                double mult = (m_cap == 0.0) ? 1.0 : 1.0 - Math.Exp(-1.0 / (m_cap * m_r_pullup) * m_clock_period.as_double());

                while (samples > 0)
                {
                    /* store sum of output signals into the buffer */
                    m_out_filter += (V0 - m_out_filter) * mult;
                    buffer[0] = (int)m_out_filter;  //*buffer++ = pokey.m_out_filter /* + m_v_ref */;       // see above
                    buffer++;
                    samples--;
                }
            }
            else if (m_output_type == output_type.DISCRETE_VAR_R)
            {
                int out_ = (int)m_voltab[m_out_raw];
                while (samples > 0)
                {
                    buffer[0] = out_;  // *buffer++ = out_;
                    buffer++;
                    samples--;
                }
            }
        }


        // device_execute_interface overrides
        void device_execute_interface_execute_run()
        {
            do
            {
                step_one_clock();
                m_icountRef.i--;
            } while (m_icountRef.i > 0);
        }


        //virtual uint32_t execute_min_cycles() const { return 114; }


        /*
         * http://www.atariage.com/forums/topic/3328-sio-protocol/page__st__100#entry1680190:
         * I noticed that the Pokey counters have clocked carry (actually, "borrow") positions that delay the
         * counter by 3 cycles, plus the 1 reset clock. So 16 bit mode has 6 carry delays and a reset clock.
         * I'm sure this was done because the propagation delays limited the number of cells the subtraction could ripple though.
         *
         */
        void step_one_clock()
        {
            /* Clocks only count if we are not in a reset */

            if ((m_SKCTL & SK_RESET) != 0)
            {
                /* polynom pointers */
                if (++m_p4 == 0x0000f)
                    m_p4 = 0;
                if (++m_p5 == 0x0001f)
                    m_p5 = 0;
                if (++m_p9 == 0x001ff)
                    m_p9 = 0;
                if (++m_p17 == 0x1ffff)
                    m_p17 = 0;

                /* CLK_1: no presacler */
                int [] clock_triggered = new int[3] {1,0,0};

                /* CLK_28: prescaler 63.9211 kHz */
                if (++m_clock_cnt[CLK_28] >= DIV_64)
                {
                    m_clock_cnt[CLK_28] = 0;
                    clock_triggered[CLK_28] = 1;
                }

                /* CLK_114 prescaler 15.6999 kHz */
                if (++m_clock_cnt[CLK_114] >= DIV_15)
                {
                    m_clock_cnt[CLK_114] = 0;
                    clock_triggered[CLK_114] = 1;
                }

                int base_clock = (m_AUDCTL & CLK_15KHZ) != 0 ? CLK_114 : CLK_28;
                int clk = (m_AUDCTL & CH1_HICLK) != 0 ? CLK_1 : base_clock;
                if (clock_triggered[clk] != 0)
                    m_channel[CHAN1].inc_chan();

                clk = (m_AUDCTL & CH3_HICLK) != 0 ? CLK_1 : base_clock;
                if (clock_triggered[clk] != 0)
                    m_channel[CHAN3].inc_chan();

                if (clock_triggered[base_clock] != 0)
                {
                    if ((m_AUDCTL & CH12_JOINED) == 0)
                        m_channel[CHAN2].inc_chan();
                    if ((m_AUDCTL & CH34_JOINED) == 0)
                        m_channel[CHAN4].inc_chan();
                }

                /* Potentiometer handling */
                if ((clock_triggered[CLK_114] != 0 || (m_SKCTL & SK_PADDLE) != 0) && (m_pot_counter < 228))
                    step_pot();

                /* Keyboard */
                if (clock_triggered[CLK_114] != 0 && (m_SKCTL & SK_KEYSCAN) != 0)
                    step_keyboard();
            }

            /* do CHAN2 before CHAN1 because CHAN1 may set borrow! */
            if (m_channel[CHAN2].check_borrow() != 0)
            {
                bool isJoined = (m_AUDCTL & CH12_JOINED) != 0;
                if (isJoined)
                    m_channel[CHAN1].reset_channel();
                m_channel[CHAN2].reset_channel();
                process_channel(CHAN2);

                /* check if some of the requested timer interrupts are enabled */
                if ((m_IRQST & IRQ_TIMR2) != 0 && m_irq_f != null)
                        m_irq_f(IRQ_TIMR2);
            }

            if (m_channel[CHAN1].check_borrow() != 0)
            {
                bool isJoined = (m_AUDCTL & CH12_JOINED) != 0;
                if (isJoined)
                    m_channel[CHAN2].inc_chan();
                else
                    m_channel[CHAN1].reset_channel();
                process_channel(CHAN1);
                /* check if some of the requested timer interrupts are enabled */
                if ((m_IRQST & IRQ_TIMR1) != 0 && m_irq_f != null)
                    m_irq_f(IRQ_TIMR1);
            }

            /* do CHAN4 before CHAN3 because CHAN3 may set borrow! */
            if (m_channel[CHAN4].check_borrow() != 0)
            {
                bool isJoined = (m_AUDCTL & CH34_JOINED) != 0;
                if (isJoined)
                    m_channel[CHAN3].reset_channel();
                m_channel[CHAN4].reset_channel();
                process_channel(CHAN4);
                /* is this a filtering channel (3/4) and is the filter active? */
                if ((m_AUDCTL & CH2_FILTER) != 0)
                    m_channel[CHAN2].sample();
                else
                    m_channel[CHAN2].m_filter_sample = 1;
                if ((m_IRQST & IRQ_TIMR4) != 0 && m_irq_f != null)
                    m_irq_f(IRQ_TIMR4);
            }

            if (m_channel[CHAN3].check_borrow() != 0)
            {
                bool isJoined = (m_AUDCTL & CH34_JOINED) != 0;
                if (isJoined)
                    m_channel[CHAN4].inc_chan();
                else
                    m_channel[CHAN3].reset_channel();
                process_channel(CHAN3);
                /* is this a filtering channel (3/4) and is the filter active? */
                if ((m_AUDCTL & CH1_FILTER) != 0)
                    m_channel[CHAN1].sample();
                else
                    m_channel[CHAN1].m_filter_sample = 1;
            }

            if (m_old_raw_inval)
            {
                uint32_t sum = 0;
                for (int ch = 0; ch < 4; ch++)
                {
                    sum |= (((m_channel[ch].m_output ^ m_channel[ch].m_filter_sample) != 0 || (m_channel[ch].m_AUDC & VOLUME_ONLY) != 0) ? (((UInt32)m_channel[ch].m_AUDC & VOLUME_MASK) << (ch * 4)) : 0);
                }

                if (m_out_raw != sum)
                {
                    //printf("forced update %08d %08x\n", m_icount, m_out_raw);
                    m_stream.update();
                }

                m_old_raw_inval = false;
                m_out_raw = sum;
            }
        }


        //-------------------------------------------------
        //  step_one_clock - step the whole chip one
        //  clock cycle.
        //-------------------------------------------------
        void step_keyboard()
        {
            if (++m_kbd_cnt > 63)
                m_kbd_cnt = 0;
            if (m_keyboard_r != null)
            {
                byte ret = m_keyboard_r(m_kbd_cnt);

                switch (m_kbd_cnt)
                {
                case POK_KEY_BREAK:
                    if ((ret & 2) != 0)
                    {
                        /* check if the break IRQ is enabled */
                        if ((m_IRQEN & IRQ_BREAK) != 0)
                        {
                            m_IRQST |= IRQ_BREAK;
                            if (m_irq_f != null)
                                m_irq_f(IRQ_BREAK);
                        }
                    }
                    break;
                case POK_KEY_SHIFT:
                    m_kbd_latch = (byte)((m_kbd_latch & 0xbf) | ((ret & 2) << 5));
                    if ((m_kbd_latch & 0x40) != 0)
                        m_SKSTAT |= SK_SHIFT;
                    else
                        m_SKSTAT &= unchecked((byte)~SK_SHIFT);
                    /* FIXME: sync ? */
                    break;
                case POK_KEY_CTRL:
                    m_kbd_latch = (byte)((m_kbd_latch & 0x7f) | ((ret & 2) << 6));
                    break;
                }

                switch (m_kbd_state)
                {
                case 0: /* waiting for key */
                    if ((ret & 1) != 0)
                    {
                        m_kbd_latch = (byte)((m_kbd_latch & 0xc0) | m_kbd_cnt);
                        m_kbd_state++;
                    }
                    break;
                case 1: /* waiting for key confirmation */
                    if ((m_kbd_latch & 0x3f) == m_kbd_cnt)
                    {
                        if ((ret & 1) != 0)
                        {
                            m_KBCODE = m_kbd_latch;
                            m_SKSTAT |= SK_KEYBD;
                            if ((m_IRQEN & IRQ_KEYBD) != 0)
                            {
                                /* last interrupt not acknowledged ? */
                                if ((m_IRQST & IRQ_KEYBD) != 0)
                                    m_SKSTAT |= SK_KBERR;
                                m_IRQST |= IRQ_KEYBD;
                                if (m_irq_f != null)
                                    m_irq_f(IRQ_KEYBD);
                            }
                            m_kbd_state++;
                        }
                        else
                            m_kbd_state = 0;
                    }
                    break;
                case 2: /* waiting for release */
                    if ((m_kbd_latch & 0x3f) == m_kbd_cnt)
                    {
                        if ((ret & 1)==0)
                            m_kbd_state++;
                        else
                            m_SKSTAT |= SK_KEYBD;
                    }
                    break;
                case 3:
                    if ((m_kbd_latch & 0x3f) == m_kbd_cnt)
                    {
                        if ((ret & 1) != 0)
                            m_kbd_state = 2;
                        else
                        {
                            m_SKSTAT &= unchecked((byte)~SK_KEYBD);
                            m_kbd_state = 0;
                        }
                    }
                    break;
                }
            }
        }


        void step_pot()
        {
            m_pot_counter++;
            uint8_t upd = 0;
            for (int pot = 0; pot < 8; pot++)
            {
                if ((m_POTx[pot]<m_pot_counter) || (m_pot_counter == 228))
                {
                    upd |= (byte)(1 << pot);
                    /* latching is emulated in read */
                }
            }

            // some pots latched?
            if (upd != 0)
                synchronize(SYNC_POT, upd);
        }


        void poly_init_4_5(uint32_t [] poly, int size, int xorbit, int invert)
        {
            int mask = (1 << size) - 1;
            int i;
            UInt32 lfsr = 0;

            int polyIdx = 0;

            LOG_POLY("poly {0}\n", size);
            for (i = 0; i < mask; i++)
            {
                /* calculate next bit */
                int in_ = (int)((((lfsr >> 0) & 1) == 0 ? 1U : 0U) ^ ((lfsr >> xorbit) & 1));
                lfsr = lfsr >> 1;
                lfsr = ((UInt32)in_ << (size-1)) | lfsr;
                poly[polyIdx] = lfsr ^ (UInt32)invert;
                LOG_POLY("{0}: {1}\n", i, poly[polyIdx]);  // %05x: %02x
                polyIdx++;
            }
        }


        void poly_init_9_17(uint32_t [] poly, int size)
        {
            int mask = (1 << size) - 1;
            int i;
            UInt32 lfsr = (UInt32)mask;

            int polyIdx = 0;

            LOG_RAND("rand {0}\n", size);

            if (size == 17)
            {
                for (i = 0; i < mask; i++)
                {
                    /* calculate next bit @ 7 */
                    int in8 = (int)(((lfsr >> 8) & 1) ^ ((lfsr >> 13) & 1));
                    int in_ = (int)(lfsr & 1);
                    lfsr = lfsr >> 1;
                    lfsr = (lfsr & 0xff7f) | ((UInt32)in8 << 7);
                    lfsr = ((UInt32)in_ << 16) | lfsr;
                    poly[polyIdx] = lfsr;
                    LOG_RAND("{0}: {1}\n", i, poly[polyIdx]);  // %05x: %02x
                    polyIdx++;
                }
            }
            else
            {
                for (i = 0; i < mask; i++)
                {
                    /* calculate next bit */
                    int in_ = (int)(((lfsr >> 0) & 1) ^ ((lfsr >> 5) & 1));
                    lfsr = lfsr >> 1;
                    lfsr = ((UInt32)in_ << 8) | lfsr;
                    poly[polyIdx] = lfsr;
                    LOG_RAND("{0}: {1}\n", i, poly[polyIdx]);  // %05x: %02x
                    polyIdx++;
                }
            }
        }


        void vol_init()
        {
            double [] resistors = new double[4] { 90000, 26500, 8050, 3400 };
            double pull_up = 10000;
            /* just a guess, there has to be a resistance since the doc specifies that
             * Vout is at least 4.2V if all channels turned off.
             */
            double r_off = 8e6;
            double [] r_chan = new double[16];
            double rTot;

            for (int j = 0; j < 16; j++)
            {
                rTot = 1.0 / 1e12; /* avoid div by 0 */;
                for (int i = 0; i < 4; i++)
                {
                    if ((j & (1 << i)) != 0)
                        rTot += 1.0 / resistors[i];
                    else
                        rTot += 1.0 / r_off;
                }
                r_chan[j] = 1.0 / rTot;
            }

            if (VERBOSE)
            {
                for (int j = 0; j < 16; j++)
                {
                    rTot = 1.0 / r_chan[j] + 3.0 / r_chan[0];
                    rTot = 1.0 / rTot;
                    LOG("{0}: {1} - {2}\n", tag(), j, rTot / (rTot+pull_up)*4.75);  // %s: %3d - %4.3f
                }
            }

            for (int j = 0; j < 0x10000; j++)
            {
                rTot = 0;
                for (int i = 0; i < 4; i++)
                {
                    rTot += 1.0 / r_chan[(j >> (i*4)) & 0x0f];
                }
                rTot = 1.0 / rTot;
                m_voltab[j] = (UInt32)rTot;
            }
        }


        void process_channel(int ch)
        {
            if ((m_channel[ch].m_AUDC & NOTPOLY5) != 0 || (m_poly5[m_p5] & 1) != 0)
            {
                if ((m_channel[ch].m_AUDC & PURE) != 0)
                    m_channel[ch].m_output ^= 1;
                else if ((m_channel[ch].m_AUDC & POLY4) != 0)
                    m_channel[ch].m_output = (byte)(m_poly4[m_p4] & 1);
                else if ((m_AUDCTL & POLY9) != 0)
                    m_channel[ch].m_output = (byte)(m_poly9[m_p9] & 1);
                else
                    m_channel[ch].m_output = (byte)(m_poly17[m_p17] & 1);

                m_old_raw_inval = true;
            }
        }


        void pokey_potgo()
        {
            int pot;

            if( (m_SKCTL & SK_RESET) == 0)
                return;

            LOG("POKEY #{0} pokey_potgo\n", this);  // #%p

            m_ALLPOT = 0x00;
            m_pot_counter = 0;

            for( pot = 0; pot < 8; pot++ )
            {
                m_POTx[pot] = 228;
                if( !m_pot_r_cb[pot].isnull() )
                {
                    int r = m_pot_r_cb[pot].op((UInt32)pot);

                    LOG("POKEY {0} pot_r({1}) returned {2}\n", tag(), pot, r);  // $%02x
                    if (r >= 228)
                    {
                        r = 228;
                    }
                    if (r == 0)
                    {
                        /* immediately set the ready - bit of m_ALLPOT
                         * In this case, most likely no capacitor is connected
                         */
                        m_ALLPOT |= (byte)(1<<pot);
                    }

                    /* final value */
                    m_POTx[pot] = (byte)r;
                }
            }
        }


        string audc2str(int val)
        {
            string buff = "";  //static char buff[80];
            if(( val & NOTPOLY5 ) != 0)
            {
                if(( val & PURE ) != 0)
                    buff = "pure";
                else
                if(( val & POLY4 ) != 0)
                    buff = "poly4";
                else
                    buff = "poly9/17";
            }
            else
            {
                if(( val & PURE ) != 0)
                    buff = "poly5";
                else
                if(( val & POLY4 ) != 0)
                    buff = "poly4+poly5";
                else
                    buff = "poly9/17+poly5";
            }

            return buff;
        }


        string audctl2str(int val)
        {
            string buff = "";  //static char buff[80];
            if(( val & POLY9 ) != 0)
                buff = "poly9";
            else
                buff = "poly17";
            if(( val & CH1_HICLK ) != 0)
                buff += "+ch1hi";
            if(( val & CH3_HICLK ) != 0)
                buff += "+ch3hi";
            if(( val & CH12_JOINED ) != 0)
                buff += "+ch1/2";
            if(( val & CH34_JOINED ) != 0)
                buff += "+ch3/4";
            if(( val & CH1_FILTER ) != 0)
                buff += "+ch1filter";
            if(( val & CH2_FILTER ) != 0)
                buff += "+ch2filter";
            if(( val & CLK_15KHZ ) != 0)
                buff += "+clk15";
            return buff;
        }


        void write_internal(offs_t offset, uint8_t data)
        {
            /* determine which address was changed */
            switch (offset & 15)
            {
            case AUDF1_C:
                LOG_SOUND("POKEY '{0}' AUDF1  {1}\n", tag(), data);  // $%02x
                m_channel[CHAN1].m_AUDF = data;
                break;

            case AUDC1_C:
                LOG_SOUND("POKEY '{0}' AUDC1  {1} ({2})\n", tag(), data, audc2str(data));  // $%02x (%s)
                m_channel[CHAN1].m_AUDC = data;
                m_old_raw_inval = true;
                break;

            case AUDF2_C:
                LOG_SOUND("POKEY '{0}' AUDF2  {1}\n", tag(), data);
                m_channel[CHAN2].m_AUDF = data;
                break;

            case AUDC2_C:
                LOG_SOUND("POKEY '{0}' AUDC2  {1} ({2})\n", tag(), data, audc2str(data));
                m_channel[CHAN2].m_AUDC = data;
                m_old_raw_inval = true;
                break;

            case AUDF3_C:
                LOG_SOUND("POKEY '{0}' AUDF3  {1}\n", tag(), data);
                m_channel[CHAN3].m_AUDF = data;
                break;

            case AUDC3_C:
                LOG_SOUND("POKEY '{0}' AUDC3  {1} ({2})\n", tag(), data, audc2str(data));
                m_channel[CHAN3].m_AUDC = data;
                m_old_raw_inval = true;
                break;

            case AUDF4_C:
                LOG_SOUND("POKEY '{0}' AUDF4  {1}\n", tag(), data);
                m_channel[CHAN4].m_AUDF = data;
                break;

            case AUDC4_C:
                LOG_SOUND("POKEY '{0}' AUDC4  {1} ({2})\n", tag(), data, audc2str(data));
                m_channel[CHAN4].m_AUDC = data;
                m_old_raw_inval = true;
                break;

            case AUDCTL_C:
                if( data == m_AUDCTL )
                    return;
                LOG_SOUND("POKEY '{0}' AUDCTL {1} ({2})\n", tag(), data, audctl2str(data));
                m_AUDCTL = data;

                break;

            case STIMER_C:
                LOG_TIMER("POKEY '{0}' STIMER {1}\n", tag(), data);

                /* From the pokey documentation:
                 * reset all counters to zero (side effect)
                 * Actually this takes 4 cycles to actually happen.
                 * FIXME: Use timer for delayed reset !
                 */
                for (int i = 0; i < POKEY_CHANNELS; i++)
                {
                    m_channel[i].reset_channel();
                    m_channel[i].m_output = 0;
                    m_channel[i].m_filter_sample = (i<2 ? (byte)1 : (byte)0);
                }

                m_old_raw_inval = true;
                break;

            case SKREST_C:
                /* reset SKSTAT */
                LOG("POKEY '{0}' SKREST {1}\n", tag(), data);
                m_SKSTAT &= unchecked((byte)~(SK_FRAME|SK_OVERRUN|SK_KBERR));
                break;

            case POTGO_C:
                LOG("POKEY '{0}' POTGO  {1}\n", tag(), data);
                pokey_potgo();
                break;

            case SEROUT_C:
                LOG("POKEY '{0}' SEROUT {1}\n", tag(), data);
                m_serout_w_cb.op(offset, data);
                m_SKSTAT |= SK_SEROUT;
                /*
                 * These are arbitrary values, tested with some custom boot
                 * loaders from Ballblazer and Escape from Fractalus
                 * The real times are unknown
                 */
                timer_set(attotime.from_usec(200), 3);
                /* 10 bits (assumption 1 start, 8 data and 1 stop bit) take how long? */
                timer_set(attotime.from_usec(2000), 4);// FUNC(pokey_serout_complete), 0, p);
                break;

            case IRQEN_C:
                LOG("POKEY '{0}' IRQEN  {1}\n", tag(), data);

                /* acknowledge one or more IRQST bits ? */
                if(( m_IRQST & ~data ) != 0)
                {
                    /* reset IRQST bits that are masked now, except the SEROC bit (acid5200 pokey_seroc test) */
                    m_IRQST &= (byte)(IRQ_SEROC | data);
                }
                /* store irq enable */
                m_IRQEN = data;
                /* if SEROC irq is enabled trigger an irq (acid5200 pokey_seroc test) */
                if ((m_IRQEN & m_IRQST & IRQ_SEROC) != 0)
                {
                    if (m_irq_f != null)
                        m_irq_f(IRQ_SEROC);
                }
                break;

            case SKCTL_C:
                if( data == m_SKCTL )
                    return;

                LOG("POKEY '{0}' SKCTL  {1}\n", tag(), data);
                m_SKCTL = data;
                if( (data & SK_RESET) == 0 )
                {
                    write_internal(IRQEN_C,  0);
                    write_internal(SKREST_C, 0);
                    /****************************************************************
                     * If the 2 least significant bits of SKCTL are 0, the random
                     * number generator is disabled (SKRESET). Thanks to Eric Smith
                     * for pointing out this critical bit of info!
                     * Couriersud: Actually, the 17bit poly is reset and kept in a
                     * reset state.
                     ****************************************************************/
                    m_p9 = 0;
                    m_p17 = 0;
                    m_p4 = 0;
                    m_p5 = 0;
                    m_clock_cnt[0] = 0;
                    m_clock_cnt[1] = 0;
                    m_clock_cnt[2] = 0;
                    m_old_raw_inval = true;
                    /* FIXME: Serial port reset ! */
                }
                break;
            }

            /************************************************************
             * As defined in the manual, the exact counter values are
             * different depending on the frequency and resolution:
             *    64 kHz or 15 kHz - AUDF + 1
             *    1.79 MHz, 8-bit  - AUDF + 4
             *    1.79 MHz, 16-bit - AUDF[CHAN1]+256*AUDF[CHAN2] + 7
             ************************************************************/

        }
    }
}
