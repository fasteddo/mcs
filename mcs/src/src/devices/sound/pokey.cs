// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using int32_t = System.Int32;
using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using stream_buffer_sample_t = System.Single;  //using sample_t = float;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.device_global;
using static mame.diexec_global;
using static mame.emucore_global;
using static mame.pokey_global;
using static mame.util;


namespace mame
{
    // ======================> pokey_device
    public class pokey_device : device_t
                                //device_sound_interface,
                                //device_execute_interface,
                                //device_state_interface
    {
        //DEFINE_DEVICE_TYPE(POKEY, pokey_device, "pokey", "Atari C012294 POKEY")
        public static readonly emu.detail.device_type_impl POKEY = DEFINE_DEVICE_TYPE("pokey", "Atari C012294 POKEY", (type, mconfig, tag, owner, clock) => { return new pokey_device(mconfig, tag, owner, clock); });


        public class device_sound_interface_pokey : device_sound_interface
        {
            public device_sound_interface_pokey(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((pokey_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
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
            public uint8_t m_INTMask;
            public uint8_t m_AUDF;           // AUDFx (D200, D202, D204, D206)
            public uint8_t m_AUDC;           // AUDCx (D201, D203, D205, D207)
            public int32_t m_borrow_cnt;     // borrow counter
            int32_t m_counter;        // channel counter
            public uint8_t m_output;         // channel output signal (1 active, 0 inactive)
            public uint8_t m_filter_sample;  // high-pass filter sample


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
            public void reset_channel()     { m_counter = m_AUDF ^ 0xff; m_borrow_cnt = 0; }

            public void inc_chan(pokey_device host, int cycles)
            {
                m_counter = (m_counter + 1) & 0xff;
                if (m_counter == 0 && m_borrow_cnt == 0)
                {
                    m_borrow_cnt = cycles;
                    if ((host.m_IRQEN & m_INTMask) != 0)
                    {
                        /* Exposed state has changed: This should only be updated after a resync ... */
                        host.machine().scheduler().synchronize(host.sync_set_irqst, m_INTMask);
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


        const int VERBOSE_SOUND   = 1 << 1;
        const int VERBOSE_TIMER   = 1 << 2;
        const int VERBOSE_POLY    = 1 << 3;
        const int VERBOSE_RAND    = 1 << 4;
        const int VERBOSE_IRQ     = 1 << 5;
        const int VERBOSE         = 0;

        //#include "logmacro.h"
        const int LOG_GENERAL = 1 << 0;
        void LOG(string format, params object [] args) { logmacro_global.LOG(VERBOSE, this, format, args); }
        void LOGMASKED(int mask, string format, params object [] args) { logmacro_global.LOGMASKED(VERBOSE, mask, this, format, args); }

        void LOG_SOUND(string format, params object [] args) { LOGMASKED(VERBOSE_SOUND, format, args); }  //#define LOG_SOUND(...) LOGMASKED(VERBOSE_SOUND, __VA_ARGS__)
        void LOG_TIMER(string format, params object [] args) { LOGMASKED(VERBOSE_TIMER, format, args); }  //#define LOG_TIMER(...) LOGMASKED(VERBOSE_TIMER, __VA_ARGS__)
        void LOG_POLY(string format, params object [] args) { LOGMASKED(VERBOSE_POLY, format, args); }  //#define LOG_POLY(...) LOGMASKED(VERBOSE_POLY, __VA_ARGS__)
        void LOG_RAND(string format, params object [] args) { LOGMASKED(VERBOSE_RAND, format, args); }  //#define LOG_RAND(...) LOGMASKED(VERBOSE_RAND, __VA_ARGS__)
        void LOG_IRQ(string format, params object [] args) { LOGMASKED(VERBOSE_IRQ, format, args); }  //#define LOG_IRQ(...) LOGMASKED(VERBOSE_IRQ, __VA_ARGS__)


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
        const int SK_TWOTONE  = 0x08;    /* Two tone mode */
        const int SK_PADDLE   = 0x04;    /* fast paddle a/d conversion */
        const int SK_RESET    = 0x03;    /* reset serial/keyboard interface */
        const int SK_KEYSCAN  = 0x02;    /* key scanning enabled ? */
        const int SK_DEBOUNCE = 0x01;    /* Debouncing ?*/


        const int DIV_64      = 28;       /* divisor for 1.78979 MHz clock to 63.9211 kHz */
        const int DIV_15      = 114;      /* divisor for 1.78979 MHz clock to 15.6999 kHz */


        const int CLK_1 = 0;
        const int CLK_28 = 1;
        const int CLK_114 = 2;


        device_sound_interface_pokey m_disound;
        device_execute_interface_pokey m_diexec;
        device_state_interface_pokey m_distate;


        // other internal states
        intref m_icount = new intref();  //int m_icount;


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

        devcb_read8.array<u64_const_8> m_pot_r_cb;
        devcb_read8 m_allpot_r_cb;
        devcb_read8 m_serin_r_cb;
        devcb_write8 m_serout_w_cb;
        devcb_write_line m_irq_w_cb;

        kb_cb_delegate m_keyboard_r;

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
        stream_buffer_sample_t [] m_voltab = new stream_buffer_sample_t[0x10000];

        output_type m_output_type;
        double m_r_pullup;
        double m_cap;
        double m_v_ref;

        emu_timer m_serout_ready_timer;
        emu_timer m_serout_complete_timer;
        emu_timer m_serin_ready_timer;


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


            m_icount.i = 0;  //m_icount = 0;
            m_stream = null;
            m_pot_r_cb = new devcb_read8.array<u64_const_8>(this, () => { return new devcb_read8(this); });
            m_allpot_r_cb = new devcb_read8(this);
            m_serin_r_cb = new devcb_read8(this);
            m_serout_w_cb = new devcb_write8(this);
            m_irq_w_cb = new devcb_write_line(this);
            m_keyboard_r = null;
            m_output_type = output_type.LEGACY_LINEAR;
            m_serout_ready_timer = null;
            m_serout_complete_timer = null;
            m_serin_ready_timer = null;
        }


        public device_sound_interface_pokey disound { get { return m_disound; } }


        public devcb_read8.binder pot_r<unsigned_N>() where unsigned_N : u32_const, new() { unsigned N = new unsigned_N().value;  return m_pot_r_cb[N].bind(); }  //template <unsigned N> auto pot_r() { return m_pot_r_cb[N].bind(); }


        public devcb_read8.binder allpot_r() { return m_allpot_r_cb.bind(); }  //auto allpot_r() { return m_allpot_r_cb.bind(); }


        //auto serin_r() { return m_serin_r_cb.bind(); }
        //auto serout_w() { return m_serout_w_cb.bind(); }
        //auto irq_w() { return m_irq_w_cb.bind(); }


        /* k543210 = k5 ... k0 returns bit0: kr1, bit1: kr2 */
        /* all are, in contrast to actual hardware, ACTIVE_HIGH */

        //typedef device_delegate<uint8_t (uint8_t k543210)> kb_cb_delegate;
        delegate uint8_t kb_cb_delegate(uint8_t k543210);

        //template <typename... T> void set_keyboard_callback(T &&... args) { m_keyboard_r.set(std::forward<T>(args)...); }


        //-------------------------------------------------
        //  read - memory interface for reading the active status
        //-------------------------------------------------
        public uint8_t read(offs_t offset)
        {
            int data;
            int pot;

            machine().scheduler().synchronize(); /* force resync */

            switch (offset & 15)
            {
            case POT0_C: case POT1_C: case POT2_C: case POT3_C:
            case POT4_C: case POT5_C: case POT6_C: case POT7_C:
                pot = (int)(offset & 7);
                if ((m_ALLPOT & (1 << pot)) != 0)
                {
                    /* we have a value measured */
                    data = m_POTx[pot];
                    LOG("{0}: POKEY read POT{1} (final value)  {2}\n", machine().describe_context(), pot, data);  // $%02x
                }
                else
                {
                    data = m_pot_counter;
                    LOG("{0}: POKEY read POT{1} (interpolated) {2}\n", machine().describe_context(), pot, data);
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
                    LOG("{0}: POKEY ALLPOT internal {1} (reset)\n", machine().describe_context(), data);
                }
                else if (!m_allpot_r_cb.isnull())
                {
                    data = m_allpot_r_cb.op_u8(offset);
                    m_ALLPOT = (uint8_t)data;
                    LOG("{0}: POKEY ALLPOT callback {1}\n", machine().describe_context(), data);
                }
                else
                {
                    data = m_ALLPOT ^ 0xff;
                    LOG("{0}: POKEY ALLPOT internal {1}\n", machine().describe_context(), data);
                }
                break;

            case KBCODE_C:
                data = m_KBCODE;
                break;

            case RANDOM_C:
                if ((m_AUDCTL & POLY9) != 0)
                {
                    data = (int)(m_poly9[m_p9] & 0xff);
                    LOG_RAND("{0}: POKEY rand9[{1}]: {2}\n", machine().describe_context(), m_p9, data);  // $%05x  $%02x
                }
                else
                {
                    data = (int)((m_poly17[m_p17] >> 8) & 0xff);
                    LOG_RAND("{0}: POKEY rand17[{1}]: {2}\n", machine().describe_context(), m_p17, data);
                }
                break;

            case SERIN_C:
                if (!m_serin_r_cb.isnull())
                    m_SERIN = m_serin_r_cb.op_u8(offset);
                data = m_SERIN;
                LOG("{0}: POKEY SERIN  {1}\n", machine().describe_context(), data);
                break;

            case IRQST_C:
                /* IRQST is an active low input port; we keep it active high */
                /* internally to ease the (un-)masking of bits */
                data = m_IRQST ^ 0xff;
                LOG("{0}: POKEY IRQST  {1}\n", machine().describe_context(), data);
                break;

            case SKSTAT_C:
                /* SKSTAT is also an active low input port */
                data = m_SKSTAT ^ 0xff;
                LOG("{0}: POKEY SKSTAT {1}\n", machine().describe_context(), data);
                break;

            default:
                LOG("{0}: POKEY register {2}\n", machine().describe_context(), offset);
                data = 0xff;
                break;
            }

            return (uint8_t)data;
        }


        //-------------------------------------------------
        //  write - memory interface for write
        //-------------------------------------------------
        public void write(offs_t offset, uint8_t data)
        {
            machine().scheduler().synchronize(sync_write, (int)((offset << 8) | data));
        }


        //DECLARE_WRITE_LINE_MEMBER( sid_w ); // pin 24
        //void serin_ready(int after);


        // analog output configuration
        public void set_output_rc(double r, double c, double v)
        {
            m_output_type = output_type.RC_LOWPASS;
            m_r_pullup = r;
            m_cap = c;
            m_v_ref = v;
        }

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

            // Set up channels
            for (int i = 0; i < POKEY_CHANNELS; i++)  //for (pokey_channel &chan : m_channel)
            {
                m_channel[i] = new pokey_channel();

                m_channel[i].m_INTMask = 0;
            }
            m_channel[CHAN1].m_INTMask = IRQ_TIMR1;
            m_channel[CHAN2].m_INTMask = IRQ_TIMR2;
            m_channel[CHAN4].m_INTMask = IRQ_TIMR4;

            // bind callbacks
            //throw new emu_unimplemented();
#if false
            m_keyboard_r.resolve();
#endif

            /* calculate the A/D times
             * In normal, slow mode (SKCTL bit SK_PADDLE is clear) the conversion
             * takes N scanlines, where N is the paddle value. A single scanline
             * takes approximately 64us to finish (1.78979MHz clock).
             * In quick mode (SK_PADDLE set) the conversion is done very fast
             * (takes two scanlines) but the result is not as accurate.
             */

            /* initialize the poly counters */
            poly_init_4_5(m_poly4, 4);
            poly_init_4_5(m_poly5, 5);

            /* initialize 9 / 17 arrays */
            poly_init_9_17(m_poly9,   9);
            poly_init_9_17(m_poly17, 17);
            vol_init();

            for (int i = 0; i < 4; i++)
                m_channel[i].m_AUDC = 0xb0;

            /* The pokey does not have a reset line. These should be initialized
             * with random values.
             */

            m_KBCODE = 0x09; // Atari 800 'no key'
            m_SKCTL = 0;

            // TODO, remove this line:
            m_SKCTL = SK_RESET;
            // It's left in place to accomodate demos that don't explicitly reset pokey.
            // See https://atariage.com/forums/topic/337317-a7800-52-release/ and
            // https://atariage.com/forums/topic/268458-a7800-the-atari-7800-emulator/?do=findComment&comment=5079170)

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
            std.fill(m_clock_cnt, 0);
            std.fill<uint8_t>(m_POTx, 0);
            
            m_pot_r_cb.resolve_all();
            m_allpot_r_cb.resolve();
            m_serin_r_cb.resolve();
            m_serout_w_cb.resolve_safe();
            m_irq_w_cb.resolve_safe();

            m_stream = m_disound.stream_alloc(0, 1, clock());

            m_serout_ready_timer = timer_alloc(serout_ready_irq);
            m_serout_complete_timer = timer_alloc(serout_complete_irq);
            m_serin_ready_timer = timer_alloc(serin_ready_irq);


            //throw new emu_unimplemented();
#if false
            save_item(STRUCT_MEMBER(m_channel, m_borrow_cnt));
            save_item(STRUCT_MEMBER(m_channel, m_counter));
            save_item(STRUCT_MEMBER(m_channel, m_filter_sample));
            save_item(STRUCT_MEMBER(m_channel, m_output));
            save_item(STRUCT_MEMBER(m_channel, m_AUDF));
            save_item(STRUCT_MEMBER(m_channel, m_AUDC));
#endif

            save_item(NAME(new { m_clock_cnt }));
            save_item(NAME(new { m_p4 }));
            save_item(NAME(new { m_p5 }));
            save_item(NAME(new { m_p9 }));
            save_item(NAME(new { m_p17 }));

            save_item(NAME(new { m_POTx }));
            save_item(NAME(new { m_AUDCTL }));
            save_item(NAME(new { m_ALLPOT }));
            save_item(NAME(new { m_KBCODE }));
            save_item(NAME(new { m_SERIN }));
            save_item(NAME(new { m_SEROUT }));
            save_item(NAME(new { m_IRQST }));
            save_item(NAME(new { m_IRQEN }));
            save_item(NAME(new { m_SKSTAT }));
            save_item(NAME(new { m_SKCTL }));

            save_item(NAME(new { m_pot_counter }));
            save_item(NAME(new { m_kbd_cnt }));
            save_item(NAME(new { m_kbd_latch }));
            save_item(NAME(new { m_kbd_state }));

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
            set_icountptr(m_icount);
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
                    m_stream.set_sample_rate(clock());
                else
                    m_stream = m_disound.stream_alloc(0, 1, clock());
            }
        }


        // device_sound_interface overrides
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            var buffer = outputs[0];

            if (m_output_type == output_type.LEGACY_LINEAR)
            {
                int out_ = 0;
                for (int i = 0; i < 4; i++)
                    out_ += (int)((m_out_raw >> (4*i)) & 0x0f);
                out_ *= POKEY_DEFAULT_GAIN;
                out_ = (out_ > 0x7fff) ? 0x7fff : out_;
                stream_buffer_sample_t outsamp = out_ * (stream_buffer_sample_t)(1.0 / 32768.0);  //stream_buffer::sample_t outsamp = out * stream_buffer::sample_t(1.0 / 32768.0);
                buffer.fill(outsamp);
            }
            else if (m_output_type == output_type.RC_LOWPASS)
            {
                double rTot = m_voltab[m_out_raw];

                double V0 = rTot / (rTot + m_r_pullup) * m_v_ref / 5.0;
                double mult = (m_cap == 0.0) ? 1.0 : 1.0 - exp(-(rTot + m_r_pullup) / (m_cap * m_r_pullup * rTot) * m_clock_period.as_double());

                for (int sampindex = 0; sampindex < buffer.samples(); sampindex++)
                {
                    /* store sum of output signals into the buffer */
                    m_out_filter += (V0 - m_out_filter) * mult;
                    buffer.put(sampindex, (stream_buffer_sample_t)m_out_filter);
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

                double V0 = ((rTot+m_r_pullup) / rTot - 1.0) * m_v_ref  / 5.0;
                buffer.fill((stream_buffer_sample_t)V0);
            }
            else if (m_output_type == output_type.OPAMP_LOW_PASS)
            {
                double rTot = m_voltab[m_out_raw];
                /* This post-pokey stage usually has a low-pass filter behind it
                    * It is approximated by not adding in VRef below.
                    */

                double V0 = (m_r_pullup / rTot) * m_v_ref  / 5.0;
                double mult = (m_cap == 0.0) ? 1.0 : 1.0 - exp(-1.0 / (m_cap * m_r_pullup) * m_clock_period.as_double());

                for (int sampindex = 0; sampindex < buffer.samples(); sampindex++)
                {
                    /* store sum of output signals into the buffer */
                    m_out_filter += (V0 - m_out_filter) * mult;
                    buffer.put(sampindex, (stream_buffer_sample_t)m_out_filter);
                }
            }
            else if (m_output_type == output_type.DISCRETE_VAR_R)
            {
                buffer.fill(m_voltab[m_out_raw]);
            }
        }


        // device_execute_interface helpers
        public void set_icountptr(intref icount) { execute().set_icountptr(icount); }


        // device_execute_interface overrides
        void device_execute_interface_execute_run()
        {
            do
            {
                step_one_clock();
                m_icount.i--;
            } while (m_icount.i > 0);
        }


        //TIMER_CALLBACK_MEMBER(serout_ready_irq);
        void serout_ready_irq(s32 param) { throw new emu_unimplemented(); }

        //TIMER_CALLBACK_MEMBER(serout_complete_irq);
        void serout_complete_irq(s32 param) { throw new emu_unimplemented(); }

        //TIMER_CALLBACK_MEMBER(serin_ready_irq);
        void serin_ready_irq(s32 param) { throw new emu_unimplemented(); }

        //TIMER_CALLBACK_MEMBER(sync_write);
        void sync_write(s32 param) { throw new emu_unimplemented(); }

        //TIMER_CALLBACK_MEMBER(sync_pot);
        void sync_pot(s32 param) { throw new emu_unimplemented(); }

        //TIMER_CALLBACK_MEMBER(sync_set_irqst);
        void sync_set_irqst(s32 param) { throw new emu_unimplemented(); }


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
            if ((m_SKCTL & SK_RESET) != 0)
            {
                /* Clocks only count if we are not in a reset */

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

                if ((m_AUDCTL & CH1_HICLK) != 0 && (clock_triggered[CLK_1] != 0))
                {
                    if ((m_AUDCTL & CH12_JOINED) != 0)
                        m_channel[CHAN1].inc_chan(this, 7);
                    else
                        m_channel[CHAN1].inc_chan(this, 4);
                }

                int base_clock = (m_AUDCTL & CLK_15KHZ) != 0 ? CLK_114 : CLK_28;

                if (((m_AUDCTL & CH1_HICLK) == 0) && (clock_triggered[base_clock] != 0))
                    m_channel[CHAN1].inc_chan(this, 1);

                if ((m_AUDCTL & CH3_HICLK) != 0 && (clock_triggered[CLK_1] != 0))
                {
                    if ((m_AUDCTL & CH34_JOINED) != 0)
                        m_channel[CHAN3].inc_chan(this, 7);
                    else
                        m_channel[CHAN3].inc_chan(this, 4);
                }

                if (((m_AUDCTL & CH3_HICLK) == 0) && (clock_triggered[base_clock] != 0))
                    m_channel[CHAN3].inc_chan(this, 1);

                if (clock_triggered[base_clock] != 0)
                {
                    if ((m_AUDCTL & CH12_JOINED) == 0)
                        m_channel[CHAN2].inc_chan(this, 1);
                    if ((m_AUDCTL & CH34_JOINED) == 0)
                        m_channel[CHAN4].inc_chan(this, 1);
                }

                /* Potentiometer handling */
                if ((clock_triggered[CLK_114] != 0 || (m_SKCTL & SK_PADDLE) != 0) && (m_pot_counter < 228))
                    step_pot();

                /* Keyboard */
                if (clock_triggered[CLK_114] != 0 && (m_SKCTL & SK_KEYSCAN) != 0)
                    step_keyboard();
            }

            if (m_channel[CHAN3].check_borrow() != 0)
            {
                if ((m_AUDCTL & CH34_JOINED) != 0)
                    m_channel[CHAN4].inc_chan(this, 1);
                else
                    m_channel[CHAN3].reset_channel();

                process_channel(CHAN3);
                /* is this a filtering channel (3/4) and is the filter active? */
                if ((m_AUDCTL & CH1_FILTER) != 0)
                    m_channel[CHAN1].sample();
                else
                    m_channel[CHAN1].m_filter_sample = 1;

                m_old_raw_inval = true;
            }

            if (m_channel[CHAN4].check_borrow() != 0)
            {
                if ((m_AUDCTL & CH34_JOINED) != 0)
                    m_channel[CHAN3].reset_channel();

                m_channel[CHAN4].reset_channel();
                process_channel(CHAN4);

                /* is this a filtering channel (3/4) and is the filter active? */
                if ((m_AUDCTL & CH2_FILTER) != 0)
                    m_channel[CHAN2].sample();
                else
                    m_channel[CHAN2].m_filter_sample = 1;

                m_old_raw_inval = true;
            }

            if ((m_SKCTL & SK_TWOTONE) != 0 && (m_channel[CHAN2].m_borrow_cnt == 1))
            {
                m_channel[CHAN1].reset_channel();
                m_old_raw_inval = true;
            }

            if (m_channel[CHAN1].check_borrow() != 0)
            {
                if ((m_AUDCTL & CH12_JOINED) != 0)
                    m_channel[CHAN2].inc_chan(this, 1);
                else
                    m_channel[CHAN1].reset_channel();

                // TODO: If two-tone is enabled *and* serial output == 1 then reset the channel 2 timer.

                process_channel(CHAN1);
            }

            if (m_channel[CHAN2].check_borrow() != 0)
            {
                if ((m_AUDCTL & CH12_JOINED) != 0)
                    m_channel[CHAN1].reset_channel();

                m_channel[CHAN2].reset_channel();

                process_channel(CHAN2);
            }

            if (m_old_raw_inval)
            {
                uint32_t sum = 0;
                for (int ch = 0; ch < 4; ch++)
                {
                    sum |= (((m_channel[ch].m_output ^ m_channel[ch].m_filter_sample) != 0 || (m_channel[ch].m_AUDC & VOLUME_ONLY) != 0) ? (((UInt32)m_channel[ch].m_AUDC & VOLUME_MASK) << (ch * 4)) : 0);
                }

                if (m_out_raw != sum)
                    m_stream.update();

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
                uint8_t ret = m_keyboard_r(m_kbd_cnt);

                switch (m_kbd_cnt)
                {
                case POK_KEY_BREAK:
                    if ((ret & 2) != 0)
                    {
                        /* check if the break IRQ is enabled */
                        if ((m_IRQEN & IRQ_BREAK) != 0)
                        {
                            LOG_IRQ("POKEY BREAK IRQ raised\n");
                            m_IRQST |= IRQ_BREAK;
                            m_irq_w_cb.op_s32(ASSERT_LINE);
                        }
                    }
                    break;
                case POK_KEY_SHIFT:
                    m_kbd_latch = (uint8_t)((m_kbd_latch & 0xbf) | ((ret & 2) << 5));
                    if ((m_kbd_latch & 0x40) != 0)
                        m_SKSTAT |= SK_SHIFT;
                    else
                        m_SKSTAT &= unchecked((uint8_t)~SK_SHIFT);
                    /* FIXME: sync ? */
                    break;
                case POK_KEY_CTRL:
                    m_kbd_latch = (uint8_t)((m_kbd_latch & 0x7f) | ((ret & 2) << 6));
                    break;
                }

                switch (m_kbd_state)
                {
                case 0: /* waiting for key */
                    if ((ret & 1) != 0)
                    {
                        m_kbd_latch = (uint8_t)((m_kbd_latch & 0xc0) | m_kbd_cnt);
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
                                LOG_IRQ("POKEY KEYBD IRQ raised\n");
                                m_IRQST |= IRQ_KEYBD;
                                m_irq_w_cb.op_s32(ASSERT_LINE);
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
                            m_SKSTAT &= unchecked((uint8_t)~SK_KEYBD);
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
                    upd |= (uint8_t)(1 << pot);
                    /* latching is emulated in read */
                }
            }

            // some pots latched?
            if (upd != 0)
                machine().scheduler().synchronize(sync_pot, upd);
        }


        void poly_init_4_5(uint32_t [] poly, int size)
        {
            LOG_POLY("poly {0}\n", size);

            int mask = (1 << size) - 1;
            uint32_t lfsr = 0;

            int xorbit = size - 1;
            int polyIdx = 0;
            for (int i = 0; i < mask; i++)
            {
                lfsr = (lfsr << 1) | (~((lfsr >> 2) ^ (lfsr >> xorbit)) & 1);
                poly[polyIdx] = lfsr & (uint32_t)mask;  //*poly = lfsr & mask;
                polyIdx++;  //poly++;
            }
        }


        void poly_init_9_17(uint32_t [] poly, int size)
        {
            LOG_RAND("rand {0}\n", size);

            uint32_t mask = util.make_bitmask32(size);
            uint32_t lfsr = mask;

            if (size == 17)
            {
                int polyIdx = 0;
                for (uint32_t i = 0; i < mask; i++)
                {
                    // calculate next bit @ 7
                    uint32_t in8 = BIT(lfsr, 8) ^ BIT(lfsr, 13);
                    uint32_t in_ = BIT(lfsr, 0);
                    lfsr = lfsr >> 1;
                    lfsr = (lfsr & 0xff7f) | ((uint32_t)in8 << 7);
                    lfsr = ((uint32_t)in_ << 16) | lfsr;
                    poly[polyIdx] = lfsr;  //*poly = lfsr;
                    LOG_RAND("{0}: {1}\n", i, poly[polyIdx]);  // %05x: %02x
                    polyIdx++;  //poly++;
                }
            }
            else // size == 9
            {
                int polyIdx = 0;
                for (uint32_t i = 0; i < mask; i++)
                {
                    // calculate next bit
                    uint32_t in_ = BIT(lfsr, 0) ^ BIT(lfsr, 5);
                    lfsr = lfsr >> 1;
                    lfsr = ((uint32_t)in_ << 8) | lfsr;
                    poly[polyIdx] = lfsr;  //*poly = lfsr;
                    LOG_RAND("{0}: {1}\n", i, poly[polyIdx]);  // %05x: %02x
                    polyIdx++;  //poly++;
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

            if (VERBOSE != 0 & LOG_GENERAL != 0)
            {
                for (int j = 0; j < 16; j++)
                {
                    rTot = 1.0 / r_chan[j] + 3.0 / r_chan[0];
                    rTot = 1.0 / rTot;
                    LOG("{0} - {1}\n", j, rTot / (rTot + pull_up) * 4.75);  // %s: %3d - %4.3f
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
                m_voltab[j] = (stream_buffer_sample_t)rTot;
            }
        }


        void process_channel(int ch)
        {
            if ((m_channel[ch].m_AUDC & NOTPOLY5) != 0 || (m_poly5[m_p5] & 1) != 0)
            {
                if ((m_channel[ch].m_AUDC & PURE) != 0)
                    m_channel[ch].m_output ^= 1;
                else if ((m_channel[ch].m_AUDC & POLY4) != 0)
                    m_channel[ch].m_output = (uint8_t)(m_poly4[m_p4] & 1);
                else if ((m_AUDCTL & POLY9) != 0)
                    m_channel[ch].m_output = (uint8_t)(m_poly9[m_p9] & 1);
                else
                    m_channel[ch].m_output = (uint8_t)(m_poly17[m_p17] & 1);

                m_old_raw_inval = true;
            }
        }


        void pokey_potgo()
        {
            if ((m_SKCTL & SK_RESET) == 0)
                return;

            LOG("pokey_potgo\n");

            m_ALLPOT = 0x00;
            m_pot_counter = 0;

            for (int pot = 0; pot < 8; pot++)
            {
                m_POTx[pot] = 228;
                if (!m_pot_r_cb[pot].isnull())
                {
                    int r = m_pot_r_cb[pot].op_u8((offs_t)pot);

                    LOG("POKEY pot_r({0}) returned {1}\n", pot, r);  // $%02x
                    if (r >= 228)
                        r = 228;

                    if (r == 0)
                    {
                        /* immediately set the ready - bit of m_ALLPOT
                         * In this case, most likely no capacitor is connected
                         */
                        m_ALLPOT |= (uint8_t)(1U<<pot);
                    }

                    /* final value */
                    m_POTx[pot] = (uint8_t)r;
                }
            }
        }


        string audc2str(int val)
        {
            string buff = "";  //static char buff[80];
            if ((val & NOTPOLY5) != 0)
            {
                if ((val & PURE) != 0)
                    buff = "pure";
                else if ((val & POLY4) != 0)
                    buff = "poly4";
                else
                    buff = "poly9/17";
            }
            else
            {
                if ((val & PURE) != 0)
                    buff = "poly5";
                else if ((val & POLY4) != 0)
                    buff = "poly4+poly5";
                else
                    buff = "poly9/17+poly5";
            }

            return buff;
        }


        string audctl2str(int val)
        {
            string buff = "";  //static char buff[80];
            if ((val & POLY9) != 0)
                buff = "poly9";
            else
                buff = "poly17";

            if ((val & CH1_HICLK) != 0)
                buff += "+ch1hi";
            if ((val & CH3_HICLK) != 0)
                buff += "+ch3hi";
            if ((val & CH12_JOINED) != 0)
                buff += "+ch1/2";
            if ((val & CH34_JOINED) != 0)
                buff += "+ch3/4";
            if ((val & CH1_FILTER) != 0)
                buff += "+ch1filter";
            if ((val & CH2_FILTER) != 0)
                buff += "+ch2filter";
            if ((val & CLK_15KHZ) != 0)
                buff += "+clk15";

            return buff;
        }


        void write_internal(offs_t offset, uint8_t data)
        {
            /* determine which address was changed */
            switch (offset & 15)
            {
            case AUDF1_C:
                LOG_SOUND("{0}: POKEY AUDF1 = {1}\n", machine().describe_context(), data);  // $%02x
                m_channel[CHAN1].m_AUDF = data;
                break;

            case AUDC1_C:
                LOG_SOUND("{0}: POKEY AUDC1  {1} ({2})\n", machine().describe_context(), data, audc2str(data));  // $%02x (%s)
                m_channel[CHAN1].m_AUDC = data;
                m_old_raw_inval = true;
                break;

            case AUDF2_C:
                LOG_SOUND("{0}: POKEY AUDF2  {1}\n", machine().describe_context(), data);
                m_channel[CHAN2].m_AUDF = data;
                break;

            case AUDC2_C:
                LOG_SOUND("{0}: POKEY AUDC2  {1} ({2})\n", machine().describe_context(), data, audc2str(data));
                m_channel[CHAN2].m_AUDC = data;
                m_old_raw_inval = true;
                break;

            case AUDF3_C:
                LOG_SOUND("{0}: POKEY AUDF3  {1}\n", machine().describe_context(), data);
                m_channel[CHAN3].m_AUDF = data;
                break;

            case AUDC3_C:
                LOG_SOUND("{0}: POKEY AUDC3  {1} ({2})\n", machine().describe_context(), data, audc2str(data));
                m_channel[CHAN3].m_AUDC = data;
                m_old_raw_inval = true;
                break;

            case AUDF4_C:
                LOG_SOUND("{0}: POKEY AUDF4  {1}\n", machine().describe_context(), data);
                m_channel[CHAN4].m_AUDF = data;
                break;

            case AUDC4_C:
                LOG_SOUND("{0}: POKEY AUDC4  {1} ({2})\n", machine().describe_context(), data, audc2str(data));
                m_channel[CHAN4].m_AUDC = data;
                m_old_raw_inval = true;
                break;

            case AUDCTL_C:
                if (data == m_AUDCTL)
                    return;
                LOG_SOUND("{0}: POKEY AUDCTL {1} ({2})\n", machine().describe_context(), data, audctl2str(data));
                m_AUDCTL = data;
                m_old_raw_inval = true;
                break;

            case STIMER_C:
                LOG_TIMER("{0}: POKEY STIMER {1}\n", machine().describe_context(), data);

                /* From the pokey documentation:
                 * reset all counters to zero (side effect)
                 * Actually this takes 4 cycles to actually happen.
                 * FIXME: Use timer for delayed reset !
                 */
                for (int i = 0; i < POKEY_CHANNELS; i++)
                {
                    m_channel[i].reset_channel();
                    m_channel[i].m_output = 0;
                    m_channel[i].m_filter_sample = (i<2 ? (uint8_t)1 : (uint8_t)0);
                }

                m_old_raw_inval = true;
                break;

            case SKREST_C:
                /* reset SKSTAT */
                LOG("{0}: POKEY SKREST {1}\n", machine().describe_context(), data);
                m_SKSTAT &= unchecked((uint8_t)~(SK_FRAME|SK_OVERRUN|SK_KBERR));
                break;

            case POTGO_C:
                LOG("{0}: POKEY POTGO  {1}\n", machine().describe_context(), data);
                pokey_potgo();
                break;

            case SEROUT_C:
                LOG("{0}: POKEY SEROUT {1}\n", machine().describe_context(), data);

                // TODO: convert to real serial comms, fix timings
                // SEROC (1) serial out in progress (0) serial out complete
                // in progress status is necessary for a800 telelnk2 to boot
                m_IRQST &= unchecked((uint8_t)~IRQ_SEROC);

                m_serout_w_cb.op_u8(offset, data);
                m_SKSTAT |= SK_SEROUT;
                /*
                 * These are arbitrary values, tested with some custom boot
                 * loaders from Ballblazer and Escape from Fractalus
                 * The real times are unknown
                 */
                m_serout_ready_timer.adjust(attotime.from_usec(200));
                /* 10 bits (assumption 1 start, 8 data and 1 stop bit) take how long? */
                m_serout_complete_timer.adjust(attotime.from_usec(2000));
                break;

            case IRQEN_C:
                LOG("{0}: POKEY IRQEN  {1}\n", machine().describe_context(), data);

                /* acknowledge one or more IRQST bits ? */
                if ((m_IRQST & ~data) != 0)
                {
                    /* reset IRQST bits that are masked now, except the SEROC bit (acid5200 pokey_seroc test) */
                    m_IRQST &= (uint8_t)(IRQ_SEROC | data);
                }
                /* store irq enable */
                m_IRQEN = data;
                /* if SEROC irq is enabled trigger an irq (acid5200 pokey_seroc test) */
                if ((m_IRQEN & m_IRQST & IRQ_SEROC) != 0)
                {
                    LOG_IRQ("POKEY SEROC IRQ enabled\n");
                    m_irq_w_cb.op_s32(ASSERT_LINE);
                }
                else if ((m_IRQEN & m_IRQST) == 0)
                {
                    LOG_IRQ("POKEY IRQs all cleared\n");
                    m_irq_w_cb.op_s32(CLEAR_LINE);
                }
                break;

            case SKCTL_C:
                if (data == m_SKCTL)
                    return;

                LOG("{0}: POKEY SKCTL  {1}\n", machine().describe_context(), data);
                m_SKCTL = data;
                if ((data & SK_RESET) == 0)
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
                    /* FIXME: Serial port reset ! */
                }
                m_old_raw_inval = true;
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


    public static class pokey_global
    {
        public static pokey_device POKEY(machine_config mconfig, string tag, u32 clock) { return emu.detail.device_type_impl.op<pokey_device>(mconfig, tag, pokey_device.POKEY, clock); }
        public static pokey_device POKEY(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<pokey_device>(mconfig, tag, pokey_device.POKEY, clock); }
        public static pokey_device POKEY<bool_Required>(machine_config mconfig, device_finder<pokey_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, pokey_device.POKEY, clock); }
    }
}
