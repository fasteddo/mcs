// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read_line = mame.devcb_read<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_read_line = devcb_read<int, 1U>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int8_t = System.SByte;
using int16_t = System.Int16;
using int32_t = System.Int32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using unsigned = System.UInt32;
using unsigned_char = System.Byte;

using static mame.device_global;
using static mame.emucore_global;
using static mame.tms5110r_global;
using static mame.tms5220_global;


namespace mame
{
    public class tms5220_device : device_t
                                  //device_sound_interface
    {
        //DEFINE_DEVICE_TYPE(TMS5220,   tms5220_device,   "tms5220",   "TMS5220")
        public static readonly emu.detail.device_type_impl TMS5220 = DEFINE_DEVICE_TYPE("tms5220", "TMS5220", (type, mconfig, tag, owner, clock) => { return new tms5220_device(mconfig, tag, owner, clock); });


        /* *****debugging defines***** */
        // general, somewhat obsolete, catch all for debugs which don't fit elsewhere
        const int LOG_GENERAL = 1 << 0;
        /* 5220 only; above dumps the data written to the tms52xx to stdout, useful
           for making logged data dumps for real hardware tests */
        const int LOG_DUMP_INPUT_DATA = 1 << 1;
        // 5220 only; above debugs FIFO stuff: writes, reads and flag updates
        const int LOG_FIFO = 1 << 2;
        // dumps each speech frame as binary
        const int LOG_PARSE_FRAME_DUMP_BIN = 1 << 3;
        // dumps each speech frame as hex
        const int LOG_PARSE_FRAME_DUMP_HEX = 1 << 4;
        // dumps info if a frame ran out of data
        const int LOG_FRAME_ERRORS = 1 << 6;
        // dumps all non-speech-data command writes
        const int LOG_COMMAND_DUMP = 1 << 7;
        // dumps decoded info about command writes
        const int LOG_COMMAND_VERBOSE = 1 << 8;
        // spams the errorlog with i/o ready messages whenever the ready or irq pin is read
        const int LOG_PIN_READS = 1 << 9;
        // dumps debug information related to the sample generation loop, i.e. whether interpolation is inhibited or not, and what the current and target values for each frame are.
        const int LOG_GENERATION = 1 << 10;
        // dumps MUCH MORE debug information related to the sample generation loop, namely the excitation, energy, pitch, k*, and output values for EVERY SINGLE SAMPLE during a frame.
        const int LOG_GENERATION_VERBOSE = 1 << 11;
        // dumps the lattice filter state data each sample.
        const int LOG_LATTICE = 1 << 12;
        // dumps info to stderr whenever the analog clip hardware is (or would be) clipping the signal.
        const int LOG_CLIP = 1 << 13;
        // debugs the io ready callback timer
        const int LOG_IO_READY = 1 << 14;
        // debugs the tms5220_data_r and data_w access methods which actually respect rs and ws
        const int LOG_RS_WS = 1 << 15;

        const int VERBOSE = 1;  //#define VERBOSE (LOG_GENERAL | LOG_DUMP_INPUT_DATA | LOG_FIFO | LOG_PARSE_FRAME_DUMP_HEX | LOG_FRAME_ERRORS | LOG_COMMAND_DUMP | LOG_COMMAND_VERBOSE | LOG_PIN_READS | LOG_GENERATION | LOG_GENERATION_VERBOSE | LOG_LATTICE | LOG_CLIP | LOG_IO_READY | LOG_RS_WS)
        //#include "logmacro.h"
        void LOGMASKED(int mask, string format, params object [] args) { logmacro_global.LOGMASKED(VERBOSE, mask, this, format, args); }


        public class device_sound_interface_tms5220 : device_sound_interface
        {
            public device_sound_interface_tms5220(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs) { ((tms5220_device)device()).device_sound_interface_sound_stream_update(stream, inputs, outputs); }  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override
        }


        /* *****optional defines***** */

        /* Hacky improvements which don't match patent: */
        /* Interpolation shift logic:
         * One of the following two lines should be used, and the other commented
         * The second line is more accurate mathematically but not accurate to the patent
         */
        int INTERP_SHIFT { get { return m_coeff.interp_coeff[m_IP]; } }  //#define INTERP_SHIFT >> m_coeff->interp_coeff[m_IP]
        //define INTERP_SHIFT / (1<<m_coeff->interp_coeff[m_IP])

        /* Other hacks */
        /* HACK: if defined, outputs the low 4 bits of the lattice filter to the i/o
         * or clip logic, even though the real hardware doesn't do this, partially verified by decap */
        //#undef ALLOW_4_LSB

        /* forces m_TALK active instantly whenever m_SPEN would be activated, causing speech delay to be reduced by up to one frame time */
        /* for some reason, this hack makes victory behave better, though it does not match the patent */
        const bool FAST_START_HACK = true;  //#define FAST_START_HACK 1


        /* *****configuration of chip connection stuff***** */
        /* must be defined; if 0, output the waveform as if it was tapped on the
           speaker pin as usual, if 1, output the waveform as if it was tapped on the
           i/o pin (volume is much lower in the latter case) */
        const bool FORCE_DIGITAL = false;

        /* 5220 only; must be defined; if 1, normal speech (one A cycle, one B cycle
           per interpolation step); if 0; speak as if SPKSLOW was used (two A cycles,
           one B cycle per interpolation step) */
        const uint8_t FORCE_SUBC_RELOAD = 1;


        const int MAX_SAMPLE_CHUNK    = 512;


        const int TMS5220_IS_TMC0281  = 1;
        const int TMS5220_IS_TMC0281D = 2;
        const int TMS5220_IS_CD2801   = 3;
        const int TMS5220_IS_CD2802   = 4;
        const int TMS5220_IS_TMS5110A = 5;
        const int TMS5220_IS_M58817   = 6;
        protected const int TMS5220_IS_5220C = 7;
        const int TMS5220_IS_5200     = 8;
        const int TMS5220_IS_5220  = 9;
        const int TMS5220_IS_CD2501ECD = 10;

        //#define TMS5220_IS_CD2501E  TMS5220_IS_5200

        // 52xx: decide whether we have rate control or not
        bool TMS5220_HAS_RATE_CONTROL { get { return (m_variant == TMS5220_IS_5220C) || (m_variant == TMS5220_IS_CD2501ECD); } }

        // All: decide whether we are a 51xx or a 52xx
        bool TMS5220_IS_52xx { get { return (m_variant == TMS5220_IS_5220C) || (m_variant == TMS5220_IS_5200) || (m_variant == TMS5220_IS_5220) || (m_variant == TMS5220_IS_CD2501ECD); } }

        /* 51xx: States for CTL */
        // ctl bus is input to tms51xx
        //#define CTL_STATE_INPUT               (0)
        // ctl bus is outputting a test talk command on CTL1(bit 0)
        //#define CTL_STATE_TTALK_OUTPUT        (1)
        // ctl bus is switching direction, next will be above
        //#define CTL_STATE_NEXT_TTALK_OUTPUT   (2)
        // ctl bus is outputting a read nybble 'output' command on CTL1,2,4,8 (bits 0-3)
        //#define CTL_STATE_OUTPUT              (3)
        // ctl bus is switching direction, next will be above
        //#define CTL_STATE_NEXT_OUTPUT         (4)

        static readonly uint8_t [] reload_table = new uint8_t[4] { 0, 2, 4, 6 }; //sample count reload for 5220c and cd2501ecd only; 5200 and 5220 always reload with 0; keep in mind this is loaded on IP=0 PC=12 subcycle=1 so it immediately will increment after one sample, effectively being 1,3,5,7 as in the comments above.


        //enum
        //{
        //    RS=2,
        //    WS=1
        //};


        const uint32_t FIFO_SIZE = 16;


        device_sound_interface_tms5220 m_disound;


        // internal state

        /* coefficient tables */
        int m_variant;                /* Variant of the 5xxx - see tms5110r.h */

        /* coefficient tables */
        tms5100_coeffs m_coeff;

        /* these contain global status bits for the 5100 */
        uint8_t m_PDC;
        uint8_t m_CTL_pins;
        uint8_t m_state;

        /* New VSM interface */
        uint32_t m_address;
        bool m_next_is_address;
        bool m_schedule_dummy_read;
        uint8_t m_addr_bit;
        /* read byte */
        uint8_t m_CTL_buffer;

        /* Old VSM interface; R Nabet : These have been added to emulate speech Roms */
        //bool m_schedule_dummy_read;          /* set after each load address, so that next read operation is preceded by a dummy read */
        uint8_t m_read_byte_register;       /* a serial->parallel shifter, used by "read byte" command to store 8 bits from the VSM */
        bool m_RDB_flag;                   /* whether we should read data register or status register */

        /* these contain data that describes the 128-bit data FIFO */
        uint8_t [] m_fifo = new uint8_t [FIFO_SIZE];
        uint8_t m_fifo_head;
        uint8_t m_fifo_tail;
        uint8_t m_fifo_count;
        uint8_t m_fifo_bits_taken;


        /* these contain global status bits (booleans) */
        bool m_previous_talk_status;/* this is the OLD value of talk_status (i.e. previous value of m_SPEN|m_TALKD), needed for generating interrupts on a falling talk_status edge */
        bool m_SPEN;                /* set on speak(or speak external and BL falling edge) command, cleared on stop command, reset command, or buffer out */
        bool m_DDIS;                /* If 1, DDIS is 1, i.e. Speak External command in progress, writes go to FIFO. */
        bool m_TALK;                /* set on SPEN & RESETL4(pc12->pc0 transition), cleared on stop command or reset command */
        bool m_TALKD;               /* TALK(TCON) value, latched every RESETL4 */
        bool m_buffer_low;          /* If 1, FIFO has less than 8 bytes in it */
        bool m_buffer_empty;        /* If 1, FIFO is empty */
        bool m_irq_pin;             /* state of the IRQ pin (output) */
        bool m_ready_pin;           /* state of the READY pin (output) */

        /* these contain data describing the current and previous voice frames */
        bool m_OLDE;
        bool m_OLDP;

        uint8_t m_new_frame_energy_idx;
        uint8_t m_new_frame_pitch_idx;
        uint8_t [] m_new_frame_k_idx = new uint8_t [10];


        /* these are all used to contain the current state of the sound generation */
#if !TMS5220_PERFECT_INTERPOLATION_HACK
        int16_t m_current_energy;
        int16_t m_current_pitch;
        int16_t [] m_current_k = new int16_t [10];
#else
        uint8_t m_old_frame_energy_idx;
        uint8_t m_old_frame_pitch_idx;
        uint8_t m_old_frame_k_idx[10];
        bool m_old_zpar;
        bool m_old_uv_zpar;

        int32_t m_current_energy;
        int32_t m_current_pitch;
        int32_t m_current_k[10];
#endif

        uint16_t m_previous_energy; /* needed for lattice filter to match patent */

        uint8_t m_subcycle;         /* contains the current subcycle for a given PC: 0 is A' (only used on SPKSLOW mode on 51xx), 1 is A, 2 is B */
        uint8_t m_subc_reload;      /* contains 1 for normal speech, 0 when SPKSLOW is active */
        uint8_t m_PC;               /* current parameter counter (what param is being interpolated), ranges from 0 to 12 */
        /* NOTE: the interpolation period counts 1,2,3,4,5,6,7,0 for divide by 8,8,8,4,4,2,2,1 */
        uint8_t m_IP;               /* the current interpolation period */
        bool m_inhibit;             /* If 1, interpolation is inhibited until the DIV1 period */
        bool m_uv_zpar;             /* If 1, zero k5 thru k10 coefficients */
        bool m_zpar;                /* If 1, zero ALL parameters. */
        bool m_pitch_zero;          /* circuit 412; pitch is forced to zero under certain circumstances */
        uint8_t m_c_variant_rate;   /* only relevant for tms5220C's multi frame rate feature; is the actual 4 bit value written on a 0x2* or 0x0* command */
        uint16_t m_pitch_count;     /* pitch counter; provides chirp rom address */

        int32_t [] m_u = new int32_t [11];
        int32_t [] m_x = new int32_t [10];

        uint16_t m_RNG;             /* the random noise generator configuration is: 1 + x + x^3 + x^4 + x^13 TODO: no it isn't */
        int16_t m_excitation_data;

        /* The TMS52xx has two different ways of providing output data: the
           analog speaker pin (which was usually used) and the Digital I/O pin.
           The internal DAC used to feed the analog pin is only 8 bits, and has the
           funny clipping/clamping logic, while the digital pin gives full 10 bit
           resolution of the output data.
           TODO: add an MCFG macro to set this other than the FORCE_DIGITAL define
         */
        bool m_digital_select;

        /* io_ready: page 3 of the datasheet specifies that READY will be asserted until
         * data is available or processed by the system.
         */
        bool m_io_ready;

        /* flag for "true" timing involving rs/ws */
        bool m_true_timing;

        /* rsws - state, rs bit 1, ws bit 0 */
        uint8_t m_rs_ws;
        uint8_t m_read_latch;
        uint8_t m_write_latch;

        sound_stream m_stream;
        emu_timer m_timer_io_ready;

        /* callbacks */
        devcb_write_line m_irq_handler;
        devcb_write_line m_readyq_handler;
        // next 2 lines are old speechrom handler, remove me!
        string m_speechrom_tag;
        speechrom_device m_speechrom;
        // next lines are new speechrom handler
        devcb_write_line m_m0_cb;      // the M0 line
        devcb_write_line m_m1_cb;      // the M1 line
        devcb_write8 m_addr_cb;    // Write to ADD1,2,4,8 - 4 address bits
        devcb_read_line m_data_cb;    // Read one bit from ADD8/Data - voice data
        // On a real chip rom_clk is running all the time
        // Here, we only use it to properly emulate the protocol.
        // Do not rely on it to be a timed signal.
        devcb_write_line m_romclk_cb;  // rom clock - Only used to drive the data lines


        protected tms5220_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, TMS5220, tag, owner, clock, TMS5220_IS_5220)
        {
        }


        protected tms5220_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, int variant)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_sound_interface_tms5220(mconfig, this));  //device_sound_interface(mconfig, *this)

            m_disound = GetClassInterface<device_sound_interface_tms5220>();


            m_variant = variant;
            m_irq_handler = new devcb_write_line(this);
            m_readyq_handler = new devcb_write_line(this);
            m_speechrom_tag = null;
            m_m0_cb = new devcb_write_line(this);
            m_m1_cb = new devcb_write_line(this);
            m_addr_cb = new devcb_write8(this);
            m_data_cb = new devcb_read_line(this);
            m_romclk_cb = new devcb_write_line(this);
        }


        public device_sound_interface_tms5220 disound { get { return m_disound; } }


        // IRQ callback function, active low, i.e. state=0
        //auto irq_cb() { return m_irq_handler.bind(); }

        // Ready callback function, active low, i.e. state=0
        //auto ready_cb() { return m_readyq_handler.bind(); }

        // old VSM support, remove me!
        //void set_speechrom_tag(const char *_tag) { m_speechrom_tag = _tag; }

        // new VSM support
        //auto m0_cb() { return m_m0_cb.bind(); }
        //auto m1_cb() { return m_m1_cb.bind(); }
        //auto addr_cb() { return m_addr_cb.bind(); }
        //auto data_cb() { return m_data_cb.bind(); }
        //auto romclk_cb() { return m_romclk_cb.bind(); }

        // Control lines - once written to will switch interface into * "true" timing behaviour.

        // all lines with suffix q are active low!


        /*
         * /RS line write handler
         */
        //WRITE_LINE_MEMBER( rsq_w );
        public void rsq_w(int state)
        {
            m_true_timing = true;
            state &= 0x01;
            LOGMASKED(LOG_RS_WS, "/RS written with data: {0}\n", state);

            uint8_t new_val = (uint8_t)((m_rs_ws & 0x01) | (state << 1));
            if (new_val != m_rs_ws)
            {
                m_rs_ws = new_val;
                if (new_val == 0)
                {
                    if (TMS5220_HAS_RATE_CONTROL) // correct for 5220c, probably also correct for cd2501ecd
                        reset();
                    else
                        /* illegal */
                        LOGMASKED(LOG_RS_WS, "tms5220_rsq_w: illegal\n");
                    return;
                }
                else if (new_val == 3)
                {
                    /* high impedance */
                    m_read_latch = 0xff;
                    return;
                }

                if (state != 0)
                {
                    /* low to high */
                }
                else
                {
                    /* high to low - schedule ready cycle */
                    LOGMASKED(LOG_RS_WS, "Scheduling ready cycle for /RS...\n");
                    /* upon /RS being activated, /READY goes inactive after 100 nsec from data sheet, through 3 asynchronous gates on patent. This is effectively within one clock, so we immediately set io_ready to 0 and activate the callback. */
                    m_io_ready = false;
                    update_ready_state();
                    // The datasheet doesn't give an exact time when /READY should change, but the data is valid 6-11 usec after /RS goes low.
                    // It looks like /READY goes high soon after that (although the datasheet graph is not to scale).
                    // The value of 13 was measured on a real chip with an oscilloscope, and it fits the datasheet.
                    m_timer_io_ready.adjust(attotime.from_usec(13), 1);
                }
            }
        }


        /*
         * /WS line write handler
         */
        //WRITE_LINE_MEMBER( tms5220_device::wsq_w )
        public void wsq_w(int state)
        {
            m_true_timing = true;
            state &= 0x01;
            LOGMASKED(LOG_RS_WS, "/WS written with data: {0}\n", state);

            uint8_t new_val = (uint8_t)(((uint32_t)m_rs_ws & 0x02) | ((uint32_t)state << 0));
            if (new_val != m_rs_ws)
            {
                m_rs_ws = new_val;
                if (new_val == 0)
                {
                    if (TMS5220_HAS_RATE_CONTROL) // correct for 5220c, probably also correct for cd2501ecd
                        reset();
                    else
                        /* illegal */
                        LOGMASKED(LOG_RS_WS, "tms5220_wsq_w: illegal\n");
                    return;
                }
                else if ( new_val == 3)
                {
                    /* high impedance */
                    m_read_latch = 0xff;
                    return;
                }

                if (state != 0)
                {
                    /* low to high  */
                }
                else
                {
                    /* high to low - schedule ready cycle */
                    LOGMASKED(LOG_RS_WS, "Scheduling ready cycle for /WS...\n");

                    /* upon /WS being activated, /READY goes inactive after 100 nsec from data sheet, through 3 asynchronous gates on patent. This is effectively within one clock, so we immediately set io_ready to 0 and activate the callback. */
                    m_io_ready = false;
                    update_ready_state();
                    /* Now comes the complicated part: how long does /READY stay inactive, when /WS is pulled low? This depends ENTIRELY on the command written, or whether the chip is in speak external mode or not...
                    Speak external mode: ~16 cycles
                    Command Mode:
                    SPK: ? cycles
                    SPKEXT: ? cycles
                    RDBY: between 60 and 140 cycles
                    RB: ? cycles (80?)
                    RST: between 60 and 140 cycles
                    SET RATE (5220C and CD2501ECD only): ? cycles (probably ~16)
                    */
                    // TODO: actually HANDLE the timing differences! currently just assuming always 16 cycles
                    m_timer_io_ready.adjust(clocks_to_attotime(16), 1); // this should take around 10-16 (closer to ~15) cycles to complete for FIFO writes, TODO: but actually depends on what command is written if in command mode
                }
            }
        }


        //void combined_rsq_wsq_w(u8 data);
        /* this combined_rsq_wsq_w hack is necessary for specific systems such as
        the TI 99/8 since the 5220c and cd2501ecd do specific things if both lines
        go active or inactive at slightly different times by separate write_line
        writes, which causes the chip to incorrectly reset itself on the 99/8,
        where the writes are supposed to happen simultaneously;
        /RS is bit 1, /WS is bit 0
        Note this is a hack and probably can be removed later, once the 'real'
        line handlers above defer by at least 4 clock cycles before taking effect */


        /**********************************************************************************************
             tms5220_data_w -- write data to the sound chip
        ***********************************************************************************************/
        public void data_w(uint8_t data)
        {
            LOGMASKED(LOG_RS_WS, "tms5220_write_data: data {0}\n", data);
            /* bring up to date first */
            m_stream.update();
            m_write_latch = data;
            if (!m_true_timing) // if we're in the default hacky mode where we don't bother with rsq_w and wsq_w...
            {
                data_write(m_write_latch); // ...force the write through instantly.
            }
            else
            {
                /* actually in a write ? */
                if (!(m_rs_ws == 0x02))
                    LOGMASKED(LOG_RS_WS, "tms5220_data_w: data written outside ws, status: {0}!\n", m_rs_ws);
            }
        }


        //uint8_t status_r();


        /**********************************************************************************************
             tms5220_ready_r -- return the not ready status from the sound chip
        ***********************************************************************************************/
        //READ_LINE_MEMBER( tms5220_device::readyq_r )
        public int readyq_r()
        {
            // prevent debugger from changing the internal state
            if (!machine().side_effects_disabled())
                m_stream.update(); /* bring up to date first */

            return !ready_read() ? 1 : 0;
        }


        //READ_LINE_MEMBER( intq_r );


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            if (!string.IsNullOrEmpty(m_speechrom_tag))
            {
                throw new emu_unimplemented();
#if false
                m_speechrom = siblingdevice<speechrom_device>( m_speechrom_tag );
                if (!m_speechrom)
                {
                    throw new emu_fatalerror("Error: {0} '{1}' can't find speechrom '{2}'\n", shortname(), tag(), m_speechrom_tag );
                }
#endif
            }
            else
            {
                //throw new emu_unimplemented();
#if false
                m_speechrom = null;
#endif
            }

            switch (m_variant)
            {
                case TMS5220_IS_TMC0281:
                    throw new emu_unimplemented();
#if false
                    m_coeff = &T0280B_0281A_coeff;
#endif
                    break;
                case TMS5220_IS_TMC0281D:
                    throw new emu_unimplemented();
#if false
                    m_coeff = &T0280D_0281D_coeff;
#endif
                    break;
                case TMS5220_IS_CD2801:
                    throw new emu_unimplemented();
#if false
                    m_coeff = &T0280F_2801A_coeff;
#endif
                    break;
                case TMS5220_IS_M58817:
                    throw new emu_unimplemented();
#if false
                    m_coeff = &M58817_coeff;
#endif
                    break;
                case TMS5220_IS_CD2802:
                    throw new emu_unimplemented();
#if false
                    m_coeff = &T0280F_2802_coeff;
#endif
                    break;
                case TMS5220_IS_TMS5110A:
                    throw new emu_unimplemented();
#if false
                    m_coeff = &tms5110a_coeff;
#endif
                    break;
                case TMS5220_IS_5200:
                case TMS5220_IS_CD2501ECD:
                    throw new emu_unimplemented();
#if false
                    m_coeff = &T0285_2501E_coeff;
#endif
                    break;
                case TMS5220_IS_5220C:
                case TMS5220_IS_5220:
                    m_coeff = tms5220_coeff;
                    break;
                default:
                    fatalerror("Unknown variant in tms5220_set_variant\n");
                    break;
            }

            /* resolve callbacks */
            m_irq_handler.resolve_safe();
            m_readyq_handler.resolve();
            m_m0_cb.resolve();
            m_m1_cb.resolve();
            m_romclk_cb.resolve();
            m_addr_cb.resolve();
            m_data_cb.resolve();

            /* initialize a stream */
            m_stream = m_disound.stream_alloc(0, 1, clock() / 80);

            m_timer_io_ready = timer_alloc(0);

            /* not during reset which is called from within a write! */
            m_io_ready = true;
            m_true_timing = false;
            m_rs_ws = 0x03; // rs and ws are assumed to be inactive on device startup
            m_write_latch = 0; // assume on start that nothing is driving the data bus

            register_for_save_states();
        }


        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            m_digital_select = FORCE_DIGITAL; // assume analog output
            /* initialize the FIFO */
            m_fifo.Fill((uint8_t)0);  //std::fill(std::begin(m_fifo), std::end(m_fifo), 0);
            m_fifo_head = m_fifo_tail = m_fifo_count = m_fifo_bits_taken = 0;

            /* initialize the chip state */
            /* Note that we do not actually clear IRQ on start-up : IRQ is even raised if m_buffer_empty or m_buffer_low are 0 */
            m_SPEN = m_DDIS = m_TALK = m_TALKD = m_previous_talk_status = m_irq_pin = m_ready_pin = false;
            set_interrupt_state(0);
            update_ready_state();
            m_buffer_empty = m_buffer_low = true;

            m_RDB_flag = false;

            /* initialize the energy/pitch/k states */
#if TMS5220_PERFECT_INTERPOLATION_HACK
            m_old_frame_energy_idx = m_old_frame_pitch_idx = 0;
            std::fill(std::begin(m_old_frame_k_idx), std::end(m_old_frame_k_idx), 0);
            m_old_zpar = false;
#endif
            m_previous_energy = 0;
            m_current_energy = 0;
            m_new_frame_energy_idx = 0;
            m_current_pitch = 0;
            m_new_frame_pitch_idx = 0;
            m_zpar = m_uv_zpar = false;
            m_new_frame_k_idx.Fill((uint8_t)0);  //std::fill(std::begin(m_new_frame_k_idx), std::end(m_new_frame_k_idx), 0);
            m_current_k.Fill((int16_t)0);  //std::fill(std::begin(m_current_k), std::end(m_current_k), 0);

            /* initialize the sample generators */
            m_inhibit = true;
            m_PC = 0;
            m_pitch_count = 0;
            m_c_variant_rate = 0;
            m_subcycle = 0;
            m_subc_reload = FORCE_SUBC_RELOAD;
            m_OLDE = m_OLDP = true;
            m_IP = reload_table[m_c_variant_rate & 0x3];
            m_RNG = 0x1FFF;
            m_u.Fill(0);  //std::fill(std::begin(m_u), std::end(m_u), 0);
            m_x.Fill(0);  //std::fill(std::begin(m_x), std::end(m_x), 0);
            m_schedule_dummy_read = false;

            if (m_speechrom != null)
            {
                throw new emu_unimplemented();
#if false
                m_speechrom.load_address(0);
                // MZ: Do the dummy read immediately. The previous line will cause a
                // shift in the address pointer in the VSM. When the next command is a
                // load_address, no dummy read will occur, hence the address will be
                // incorrectly shifted.
                m_speechrom.read(1);
                m_schedule_dummy_read = false;
#endif
            }

            // 5110 specific stuff
            m_PDC = 0;
            m_CTL_pins = 0;
            m_state = 0;
            m_address = 0;
            m_next_is_address = false;
            m_addr_bit = 0;
            m_CTL_buffer = 0;
        }


        protected override void device_clock_changed()
        {
            m_stream.set_sample_rate(clock() / 80);
        }


        /**********************************************************************************************
             True timing
        ***********************************************************************************************/
        protected override void device_timer(emu_timer timer, device_timer_id id, int param)
        {
            switch(id)
            {
            case 0: // m_timer_io_ready
                /* bring up to date first */
                m_stream.update();
                LOGMASKED(LOG_IO_READY, "m_timer_io_ready timer fired, param = {0}, m_rs_ws = {1}\n", param, m_rs_ws);
                if (param != 0) // low->high ready state
                {
                    switch (m_rs_ws)
                    {
                    case 0x02:
                        /* Write */
                        LOGMASKED(LOG_IO_READY, "m_timer_io_ready: Attempting to service write...\n");
                        if ((m_fifo_count >= FIFO_SIZE) && m_DDIS) // if FIFO is full and we're in speak external mode
                        {
                            LOGMASKED(LOG_IO_READY, "m_timer_io_ready: in SPKEXT and FIFO was full! cannot service write now, delaying 16 cycles...\n");
                            m_timer_io_ready.adjust(clocks_to_attotime(16), 1);
                            break;
                        }
                        else
                        {
                            LOGMASKED(LOG_IO_READY, "m_timer_io_ready: Serviced write: {0}\n", m_write_latch);
                            data_write(m_write_latch);
                            m_io_ready = param != 0;
                            break;
                        }
                    case 0x01:
                        /* Read */
                        m_read_latch = status_read(true);
                        LOGMASKED(LOG_IO_READY, "m_timer_io_ready: Serviced read, returning {0}\n", m_read_latch);
                        m_io_ready = param != 0;
                        break;
                    case 0x03:
                        /* High Impedance */
                        m_io_ready = param != 0;
                        break;
                    case 0x00:
                        /* illegal */
                        m_io_ready = param != 0;
                        break;
                    }
                }

                update_ready_state();
                break;
            }
        }


        // sound stream update overrides
        /**********************************************************************************************
             tms5220_update -- update the sound chip so that it is in sync with CPU execution
        ***********************************************************************************************/
        //-------------------------------------------------
        //  sound_stream_update - handle a stream update
        //-------------------------------------------------
        void device_sound_interface_sound_stream_update(sound_stream stream, std.vector<read_stream_view> inputs, std.vector<write_stream_view> outputs)  //virtual void sound_stream_update(sound_stream &stream, std::vector<read_stream_view> const &inputs, std::vector<write_stream_view> &outputs) override;
        {
            int16_t [] sample_data = new int16_t[MAX_SAMPLE_CHUNK];
            var output = outputs[0];

            /* loop while we still have samples to generate */
            for (int sampindex = 0; sampindex < output.samples(); )
            {
                int length = (output.samples() > MAX_SAMPLE_CHUNK) ? MAX_SAMPLE_CHUNK : (int)output.samples();

                /* generate the samples and copy to the target buffer */
                process(sample_data, (unsigned)length);
                for (int index = 0; index < length; index++)
                    output.put_int(sampindex++, sample_data[index], 32768);
            }
        }


        // 51xx and VSM related
        //void new_int_write(uint8_t rc, uint8_t m0, uint8_t m1, uint8_t addr);
        //void new_int_write_addr(uint8_t addr);
        //uint8_t new_int_read();
        //void perform_dummy_read();


        // 52xx or common

        void register_for_save_states()
        {
            // for sanity purposes these variables should be in the same order as in tms5220.h!

            // 5110 specific stuff
            save_item(NAME(new { m_PDC }));
            save_item(NAME(new { m_CTL_pins }));
            save_item(NAME(new { m_state }));

            // new VSM stuff
            save_item(NAME(new { m_address }));
            save_item(NAME(new { m_next_is_address }));
            save_item(NAME(new { m_schedule_dummy_read }));
            save_item(NAME(new { m_addr_bit }));
            save_item(NAME(new { m_CTL_buffer }));

            // old VSM stuff
            save_item(NAME(new { m_read_byte_register }));
            save_item(NAME(new { m_RDB_flag }));

            // FIFO
            save_item(NAME(new { m_fifo }));
            save_item(NAME(new { m_fifo_head }));
            save_item(NAME(new { m_fifo_tail }));
            save_item(NAME(new { m_fifo_count }));
            save_item(NAME(new { m_fifo_bits_taken }));

            // global status bits (booleans)
            save_item(NAME(new { m_previous_talk_status }));
            save_item(NAME(new { m_SPEN }));
            save_item(NAME(new { m_DDIS }));
            save_item(NAME(new { m_TALK }));
            save_item(NAME(new { m_TALKD }));
            save_item(NAME(new { m_buffer_low }));
            save_item(NAME(new { m_buffer_empty }));
            save_item(NAME(new { m_irq_pin }));
            save_item(NAME(new { m_ready_pin }));

            // current and previous frames
            save_item(NAME(new { m_OLDE }));
            save_item(NAME(new { m_OLDP }));

            save_item(NAME(new { m_new_frame_energy_idx }));
            save_item(NAME(new { m_new_frame_pitch_idx }));
            save_item(NAME(new { m_new_frame_k_idx }));
#if TMS5220_PERFECT_INTERPOLATION_HACK
            save_item(NAME(m_old_frame_energy_idx));
            save_item(NAME(m_old_frame_pitch_idx));
            save_item(NAME(m_old_frame_k_idx));
            save_item(NAME(m_old_zpar));
            save_item(NAME(m_old_uv_zpar));
#endif
            save_item(NAME(new { m_current_energy }));
            save_item(NAME(new { m_current_pitch }));
            save_item(NAME(new { m_current_k }));

            save_item(NAME(new { m_previous_energy }));

            save_item(NAME(new { m_subcycle }));
            save_item(NAME(new { m_subc_reload }));
            save_item(NAME(new { m_PC }));
            save_item(NAME(new { m_IP }));
            save_item(NAME(new { m_inhibit }));
            save_item(NAME(new { m_uv_zpar }));
            save_item(NAME(new { m_zpar }));
            save_item(NAME(new { m_pitch_zero }));
            save_item(NAME(new { m_c_variant_rate }));
            save_item(NAME(new { m_pitch_count }));

            save_item(NAME(new { m_u }));
            save_item(NAME(new { m_x }));

            save_item(NAME(new { m_RNG }));
            save_item(NAME(new { m_excitation_data }));

            save_item(NAME(new { m_digital_select }));

            save_item(NAME(new { m_io_ready }));

            // "proper" rs+ws emulation
            save_item(NAME(new { m_true_timing }));

            save_item(NAME(new { m_rs_ws }));
            save_item(NAME(new { m_read_latch }));
            save_item(NAME(new { m_write_latch }));
        }


        /**********************************************************************************************
             tms5220_device::data_write -- handle a write to the TMS5220
        ***********************************************************************************************/
        void data_write(int data)
        {
            bool old_buffer_low = m_buffer_low;
            LOGMASKED(LOG_DUMP_INPUT_DATA, "{0}", data);

            if (m_DDIS) // If we're in speak external mode
            {
                // add this byte to the FIFO
                if (m_fifo_count < FIFO_SIZE)
                {
                    m_fifo[m_fifo_tail] = (uint8_t)data;
                    m_fifo_tail = (uint8_t)(((uint32_t)m_fifo_tail + 1) % FIFO_SIZE);
                    m_fifo_count++;
                    LOGMASKED(LOG_FIFO, "data_write: Added byte to FIFO (current count={0})\n", m_fifo_count);
                    update_fifo_status_and_ints();

                    // if we just unset buffer low with that last write, and SPEN *was* zero (see circuit 251, sheet 12)
                    if ((!m_SPEN) && (old_buffer_low && (!m_buffer_low))) // MUST HAVE EDGE DETECT
                    {
                        LOGMASKED(LOG_FIFO, "data_write triggered SPEN to go active!\n");
                        // ...then we now have enough bytes to start talking; set zpar and clear out the new frame parameters (it will become old frame just before the first call to parse_frame() )
                        m_zpar = true;
                        m_uv_zpar = true; // zero k4-k10 as well
                        m_OLDE = true; // 'silence/zpar' frames are zero energy
                        m_OLDP = true; // 'silence/zpar' frames are zero pitch
#if TMS5220_PERFECT_INTERPOLATION_HACK
                        m_old_zpar = true; // zero all the old parameters
                        m_old_uv_zpar = true; // zero old k4-k10 as well
#endif
                        m_SPEN = true;
//#if FAST_START_HACK
                        if (FAST_START_HACK)
                            m_TALK = true;
//#endif
                        m_new_frame_energy_idx = 0;
                        m_new_frame_pitch_idx = 0;
                        for (int i = 0; i < 4; i++)
                            m_new_frame_k_idx[i] = 0;
                        for (int i = 4; i < 7; i++)
                            m_new_frame_k_idx[i] = 0xF;
                        for (int i = 7; i < m_coeff.num_k; i++)
                            m_new_frame_k_idx[i] = 0x7;

                    }
                }
                else
                {
                    LOGMASKED(LOG_FIFO, "data_write: Ran out of room in the tms52xx FIFO! this should never happen!\n");
                    // at this point, /READY should remain HIGH/inactive until the FIFO has at least one byte open in it.
                }
            }
            else //(! m_DDIS)
            {
                // R Nabet : we parse commands at once.  It is necessary for such commands as read.
                process_command((unsigned_char)data);
            }
        }


        void update_fifo_status_and_ints()
        {
            /* update 52xx FIFO flags and set ints if needed */
            if (!TMS5220_IS_52xx) return; // bail out if not a 52xx chip
            update_ready_state();

            /* BL is set if neither byte 9 nor 8 of the FIFO are in use; this
            translates to having fifo_count (which ranges from 0 bytes in use to 16
            bytes used) being less than or equal to 8. Victory/Victorba depends on this. */
            if (m_fifo_count <= 8)
            {
                // generate an interrupt if necessary; if /BL was inactive and is now active, set int.
                if (!m_buffer_low)
                {
                    m_buffer_low = true;
                    set_interrupt_state(1);
                }
            }
            else
            {
                m_buffer_low = false;
            }

            /* BE is set if neither byte 15 nor 14 of the FIFO are in use; this
            translates to having fifo_count equal to exactly 0
            */
            if (m_fifo_count == 0)
            {
                // generate an interrupt if necessary; if /BE was inactive and is now active, set int.
                if (!m_buffer_empty)
                {
                    m_buffer_empty = true;
                    set_interrupt_state(1);
                }

                if (m_DDIS)
                    m_TALK = m_SPEN = false; // /BE being active clears the TALK status via TCON, which in turn clears SPEN, but ONLY if m_DDIS is set! See patent page 16, gate 232b
            }
            else
            {
                m_buffer_empty = false;
            }

            // generate an interrupt if /TS was active, and is now inactive.
            // also, in this case, regardless if DDIS was set, unset it.
            if (m_previous_talk_status && !talk_status())
            {
                LOGMASKED(LOG_GENERAL, "Talk status WAS 1, is now 0, unsetting DDIS and firing an interrupt!\n");
                set_interrupt_state(1);
                m_DDIS = false;
            }

            m_previous_talk_status = talk_status();
        }


        /**********************************************************************************************
             extract_bits -- extract a specific number of bits from the current input stream (FIFO or VSM)
        ***********************************************************************************************/
        int extract_bits(int count)
        {
            int val = 0;

            if (m_DDIS)
            {
                // extract from FIFO
                while (count-- != 0)
                {
                    val = (val << 1) | ((m_fifo[m_fifo_head] >> m_fifo_bits_taken) & 1);
                    m_fifo_bits_taken++;
                    if (m_fifo_bits_taken >= 8)
                    {
                        m_fifo_count--;
                        m_fifo[m_fifo_head] = 0; // zero the newly depleted FIFO head byte
                        m_fifo_head = (uint8_t)(((uint32_t)m_fifo_head + 1) % FIFO_SIZE);
                        m_fifo_bits_taken = 0;
                        update_fifo_status_and_ints();
                    }
                }
            }
            else
            {
#if !USE_NEW_TMS6100_CODE
/** TODO: get rid of this old code */
                // extract from VSM (speech ROM)
                if (m_speechrom != null)
                    val = m_speechrom.read(count);
                else
                    val = (1 << count) - 1; // assume the input floats high if nothing is connected, so a spurious speak vsm command will eventually return a 0xF (STOP) frame which will halt speech
#else
                while (count--)
                {
                    val = (val << 1) | new_int_read();
                    LOGMASKED(LOG_GENERAL, "bit read: %d\n", val&1);
                }
#endif
            }

            return val;
        }


        /**********************************************************************************************
             tms5220_status_read -- read status or data from the TMS5220; if the bool is 1, clear interrupt
             state.
        ***********************************************************************************************/
        uint8_t status_read(bool clear_int)
        {
            if (m_RDB_flag)
            {
                /* if last command was read, return data register */
                m_RDB_flag = false;
                return m_read_byte_register;
            }
            else
            {
                /* read status */
                /* clear the interrupt pin on status read */
                if (clear_int)
                    set_interrupt_state(0);

                LOGMASKED(LOG_PIN_READS, "Status read: TS={0} BL={1} BE={2}\n", talk_status(), m_buffer_low, m_buffer_empty);
                return (uint8_t)(((talk_status() ? 1U : 0U) << 7) | ((m_buffer_low ? 1U : 0U) << 6) | ((m_buffer_empty ? 1U : 0U) << 5));// | (m_write_latch & 0x1f); // low 5 bits are open bus, so use the m_write_latch value.
            }
        }


        /**********************************************************************************************
             tms5220_ready_read -- returns the ready state of the TMS5220
        ***********************************************************************************************/
        bool ready_read()
        {
            LOGMASKED(LOG_PIN_READS, "ready_read: ready pin read, io_ready is {0}, FIFO count is {1}, DDIS(speak external) is {2}\n", m_io_ready, m_fifo_count, m_DDIS);
            /* if m_true_timing is NOT set (we're in 'hacky instant write mode'), the
               m_timer_io_ready timer doesn't run and will never de-assert m_io_ready
               if the FIFO is full, so we need to explicitly check for FIFO full here
               and return the proper value.

               SEVERE CAVEAT: This makes the assumption that the ready_read was after
               wsq was 'virtually asserted', so if the FIFO has no room in it ready
               will always return inactive, even if no write happened! i.e., after a
               read command when the FIFO was exactly filled, but no write attempted
               to overfill it. This behavior is inaccurate to hardware and may cause
               issues! You have been warned!
            */
            if (!m_true_timing)
                return ((m_fifo_count < FIFO_SIZE) || (!m_DDIS)) && m_io_ready;
            else
                return m_io_ready;
        }


        //bool int_read();


        /**********************************************************************************************
             tms5220_process -- fill the buffer with a specific number of samples
        ***********************************************************************************************/
        void process(int16_t [] buffer, unsigned size)  //void tms5220_device::process(int16_t *buffer, unsigned int size)
        {
            int buf_count = 0;
            int i;
            int bitout;
            int32_t this_sample;

            LOGMASKED(LOG_GENERAL, "process called with size of {0}; IP={1}, PC={2}, subcycle={3}, m_SPEN={4}, m_TALK={5}, m_TALKD={6}\n", size, m_IP, m_PC, m_subcycle, m_SPEN, m_TALK, m_TALKD);

            /* loop until the buffer is full or we've stopped speaking */
            while (size > 0)
            {
                if (m_TALKD) // speaking
                {
                    /* if we're ready for a new frame to be applied, i.e. when IP=0, PC=12, Sub=1
                    * (In reality, the frame was really loaded incrementally during the entire IP=0
                    * PC=x time period, but it doesn't affect anything until IP=0 PC=12 happens)
                    */
                    if ((m_IP == 0) && (m_PC == 12) && (m_subcycle == 1))
                    {
                        // HACK for regression testing, be sure to comment out before release!
                        //m_RNG = 0x1234;
                        // end HACK

                        /* appropriately override the interp count if needed; this will be incremented after the frame parse! */
                        m_IP = reload_table[m_c_variant_rate&0x3];

#if TMS5220_PERFECT_INTERPOLATION_HACK
                        /* remember previous frame energy, pitch, and coefficients */
                        m_old_frame_energy_idx = m_new_frame_energy_idx;
                        m_old_frame_pitch_idx = m_new_frame_pitch_idx;
                        for (i = 0; i < m_coeff->num_k; i++)
                            m_old_frame_k_idx[i] = m_new_frame_k_idx[i];
#endif

                        /* Parse a new frame into the new_target_energy, new_target_pitch and new_target_k[] */
                        parse_frame();

                        /* if the new frame is a stop frame, unset both TALK and SPEN (via TCON). TALKD remains active while the energy is ramping to 0. */
                        if (new_frame_stop_flag())
                        {
                            m_TALK = m_SPEN = false;
                            update_fifo_status_and_ints(); // probably not necessary...
                        }

                        /* in all cases where interpolation would be inhibited, set the inhibit flag; otherwise clear it.
                         * Interpolation inhibit cases:
                         * Old frame was voiced, new is unvoiced
                         * Old frame was silence/zero energy, new has non-zero energy
                         * Old frame was unvoiced, new is voiced
                         * Old frame was unvoiced, new frame is silence/zero energy (non-existent on tms51xx rev D and F (present and working on tms52xx, present but buggy on tms51xx rev A and B))
                         */
                        if ( (!old_frame_unvoiced_flag() && new_frame_unvoiced_flag())
                            || (old_frame_unvoiced_flag() && !new_frame_unvoiced_flag())
                            || (old_frame_silence_flag() && !new_frame_silence_flag())
                            //|| (m_inhibit && old_frame_unvoiced_flag() && new_frame_silence_flag()) ) //TMS51xx INTERP BUG1
                            || (old_frame_unvoiced_flag() && new_frame_silence_flag()) )
                            m_inhibit = true;
                        else // normal frame, normal interpolation
                            m_inhibit = false;

                        /* Debug info for current parsed frame */
                        LOGMASKED(LOG_GENERATION, "OLDE: {0}; NEWE: {1}; OLDP: {2}; NEWP: {3} ", old_frame_silence_flag(), new_frame_silence_flag(), old_frame_unvoiced_flag(), new_frame_unvoiced_flag());
                        LOGMASKED(LOG_GENERATION, "Processing new frame: ");
                        if (!m_inhibit)
                            LOGMASKED(LOG_GENERATION, "Normal Frame\n");
                        else
                            LOGMASKED(LOG_GENERATION, "Interpolation Inhibited\n");
                        LOGMASKED(LOG_GENERATION, "*** current Energy, Pitch and Ks =      {0},   {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}\n", m_current_energy, m_current_pitch, m_current_k[0], m_current_k[1], m_current_k[2], m_current_k[3], m_current_k[4], m_current_k[5], m_current_k[6], m_current_k[7], m_current_k[8], m_current_k[9]);
                        LOGMASKED(LOG_GENERATION, "*** target Energy(idx), Pitch, and Ks = {0}({1}),{2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}\n",
                            (m_coeff.energytable[m_new_frame_energy_idx] * (1 - (m_zpar ? 1 : 0))),
                            m_new_frame_energy_idx,
                            (m_coeff.pitchtable[m_new_frame_pitch_idx] * (1 - (m_zpar ? 1 : 0))),
                            (m_coeff.ktable[0, m_new_frame_k_idx[0]] * (1 - (m_zpar ? 1 : 0))),
                            (m_coeff.ktable[1, m_new_frame_k_idx[1]] * (1 - (m_zpar ? 1 : 0))),
                            (m_coeff.ktable[2, m_new_frame_k_idx[2]] * (1 - (m_zpar ? 1 : 0))),
                            (m_coeff.ktable[3, m_new_frame_k_idx[3]] * (1 - (m_zpar ? 1 : 0))),
                            (m_coeff.ktable[4, m_new_frame_k_idx[4]] * (1 - (m_uv_zpar ? 1 : 0))),
                            (m_coeff.ktable[5, m_new_frame_k_idx[5]] * (1 - (m_uv_zpar ? 1 : 0))),
                            (m_coeff.ktable[6, m_new_frame_k_idx[6]] * (1 - (m_uv_zpar ? 1 : 0))),
                            (m_coeff.ktable[7, m_new_frame_k_idx[7]] * (1 - (m_uv_zpar ? 1 : 0))),
                            (m_coeff.ktable[8, m_new_frame_k_idx[8]] * (1 - (m_uv_zpar ? 1 : 0))),
                            (m_coeff.ktable[9, m_new_frame_k_idx[9]] * (1 - (m_uv_zpar ? 1 : 0))));
                    }
                    else // Not a new frame, just interpolate the existing frame.
                    {
                        bool inhibit_state = (m_inhibit && (m_IP != 0)); // disable inhibit when reaching the last interp period, but don't overwrite the m_inhibit value
#if TMS5220_PERFECT_INTERPOLATION_HACK
                        int samples_per_frame = m_subc_reload?175:266; // either (13 A cycles + 12 B cycles) * 7 interps for normal SPEAK/SPKEXT, or (13*2 A cycles + 12 B cycles) * 7 interps for SPKSLOW
                        //int samples_per_frame = m_subc_reload?200:304; // either (13 A cycles + 12 B cycles) * 8 interps for normal SPEAK/SPKEXT, or (13*2 A cycles + 12 B cycles) * 8 interps for SPKSLOW
                        int current_sample = (m_subcycle - m_subc_reload)+(m_PC*(3-m_subc_reload))+((m_subc_reload?25:38)*((m_IP-1)&7));
                        //logerror( "CS: %03d", current_sample);
                        // reset the current energy, pitch, etc to what it was at frame start
                        m_current_energy = (m_coeff->energytable[m_old_frame_energy_idx] * (1-m_old_zpar));
                        m_current_pitch = (m_coeff->pitchtable[m_old_frame_pitch_idx] * (1-m_old_zpar));
                        for (i = 0; i < m_coeff->num_k; i++)
                            m_current_k[i] = (m_coeff->ktable[i][m_old_frame_k_idx[i]] * (1-((i<4)?m_old_zpar:m_old_uv_zpar)));
                        // now adjust each value to be exactly correct for each of the samples per frame
                        if (m_IP != 0) // if we're still interpolating...
                        {
                            m_current_energy = (m_current_energy + (((m_coeff->energytable[m_new_frame_energy_idx] - m_current_energy)*(1-inhibit_state))*current_sample)/samples_per_frame)*(1-m_zpar);
                            m_current_pitch = (m_current_pitch + (((m_coeff->pitchtable[m_new_frame_pitch_idx] - m_current_pitch)*(1-inhibit_state))*current_sample)/samples_per_frame)*(1-m_zpar);
                            for (i = 0; i < m_coeff->num_k; i++)
                                m_current_k[i] = (m_current_k[i] + (((m_coeff->ktable[i][m_new_frame_k_idx[i]] - m_current_k[i])*(1-inhibit_state))*current_sample)/samples_per_frame)*(1-((i<4)?m_zpar:m_uv_zpar));
                        }
                        else // we're done, play this frame for 1/8 frame.
                        {
                            if (m_subcycle == 2) m_pitch_zero = false; // this reset happens around the second subcycle during IP=0
                            m_current_energy = (m_coeff->energytable[m_new_frame_energy_idx] * (1-m_zpar));
                            m_current_pitch = (m_coeff->pitchtable[m_new_frame_pitch_idx] * (1-m_zpar));
                            for (i = 0; i < m_coeff->num_k; i++)
                                m_current_k[i] = (m_coeff->ktable[i][m_new_frame_k_idx[i]] * (1-((i<4)?m_zpar:m_uv_zpar)));
                        }
#else
                        //Updates to parameters only happen on subcycle '2' (B cycle) of PCs.
                        if (m_subcycle == 2)
                        {
                            switch(m_PC)
                            {
                                case 0: /* PC = 0, B cycle, write updated energy */
                                if (m_IP == 0) m_pitch_zero = false; // this reset happens around the second subcycle during IP=0
                                m_current_energy = (int16_t)((m_current_energy + (((m_coeff.energytable[m_new_frame_energy_idx] - m_current_energy) * (1 - (inhibit_state ? 1 : 0))) >> INTERP_SHIFT)) * (1 - (m_zpar ? 1 : 0)));
                                break;
                                case 1: /* PC = 1, B cycle, write updated pitch */
                                m_current_pitch = (int16_t)((m_current_pitch + (((m_coeff.pitchtable[m_new_frame_pitch_idx] - m_current_pitch) * (1 - (inhibit_state ? 1 : 0))) >> INTERP_SHIFT)) * (1 - (m_zpar ? 1 : 0)));
                                break;
                                case 2: case 3: case 4: case 5: case 6: case 7: case 8: case 9: case 10: case 11:
                                /* PC = 2 through 11, B cycle, write updated K1 through K10 */
                                m_current_k[m_PC - 2] = (int16_t)((m_current_k[m_PC - 2] + (((m_coeff.ktable[m_PC - 2, m_new_frame_k_idx[m_PC - 2]] - m_current_k[m_PC - 2]) * (1 - (inhibit_state ? 1 : 0))) >> INTERP_SHIFT)) * (1 - (((m_PC - 2) < 4) ? (m_zpar ? 1 : 0) : (m_uv_zpar ? 1 : 0))));
                                break;
                                case 12: /* PC = 12 */
                                /* we should NEVER reach this point, PC=12 doesn't have a subcycle 2 */
                                break;
                            }
                        }
#endif
                    }

                    // calculate the output
                    if (old_frame_unvoiced_flag())
                    {
                        // generate unvoiced samples here
                        if ((m_RNG & 1) != 0)
                            m_excitation_data = ~0x3F; /* according to the patent it is (either + or -) half of the maximum value in the chirp table, so either 01000000(0x40) or 11000000(0xC0)*/
                        else
                            m_excitation_data = 0x40;
                    }
                    else /* (!old_frame_unvoiced_flag()) */
                    {
                        // generate voiced samples here
                        /* US patent 4331836 Figure 14B shows, and logic would hold, that a pitch based chirp
                         * function has a chirp/peak and then a long chain of zeroes.
                         * The last entry of the chirp rom is at address 0b110011 (51d), the 52nd sample,
                         * and if the address reaches that point the ADDRESS incrementer is
                         * disabled, forcing all samples beyond 51d to be == 51d
                         */
                        if (m_pitch_count >= 51)
                            m_excitation_data = (int8_t)m_coeff.chirptable[51];
                        else /*m_pitch_count < 51*/
                            m_excitation_data = (int8_t)m_coeff.chirptable[m_pitch_count];
                    }

                    // Update LFSR *20* times every sample (once per T cycle), like patent shows
                    for (i = 0; i < 20; i++)
                    {
                        bitout = ((m_RNG >> 12) & 1) ^
                                ((m_RNG >>  3) & 1) ^
                                ((m_RNG >>  2) & 1) ^
                                ((m_RNG >>  0) & 1);
                        m_RNG <<= 1;
                        m_RNG |= (uint16_t)bitout;
                    }

                    this_sample = lattice_filter(); /* execute lattice filter */

                    //LOGMASKED(LOG_GENERATION_VERBOSE, "C:%01d; ",m_subcycle);
                    LOGMASKED(LOG_GENERATION_VERBOSE, "IP:{0} PC:{1} X:{2} E:{3} P:{4} Pc:{5} ",m_IP, m_PC, m_excitation_data, m_current_energy, m_current_pitch, m_pitch_count);
                    //LOGMASKED(LOG_GENERATION_VERBOSE, "X:%04d E:%03d P:%03d Pc:%03d ", m_excitation_data, m_current_energy, m_current_pitch, m_pitch_count);
                    for (i=0; i<10; i++)
                        LOGMASKED(LOG_GENERATION_VERBOSE, "K{0}:{1} ", i + 1, m_current_k[i]);
                    LOGMASKED(LOG_GENERATION_VERBOSE, "Out:{0} ", this_sample);
//#ifdef TMS5220_PERFECT_INTERPOLATION_HACK
//                  LOGMASKED(LOG_GENERATION_VERBOSE, "%d%d%d%d",m_old_zpar,m_zpar,m_old_uv_zpar,m_uv_zpar);
//#else
//                  LOGMASKED(LOG_GENERATION_VERBOSE, "x%dx%d",m_zpar,m_uv_zpar);
//#endif
                    LOGMASKED(LOG_GENERATION_VERBOSE, "\n");

                    /* next, force result to 14 bits (since its possible that the addition at the final (k1) stage of the lattice overflowed) */
                    while (this_sample > 16383) this_sample -= 32768;
                    while (this_sample < -16384) this_sample += 32768;
                    if (m_digital_select == false) // analog SPK pin output is only 8 bits, with clipping
                    {
                        buffer[buf_count] = clip_analog((int16_t)this_sample);
                    }
                    else // digital I/O pin output is 12 bits
                    {
#if ALLOW_4_LSB
                        // input:  ssss ssss ssss ssss ssnn nnnn nnnn nnnn
                        // N taps:                       ^                 = 0x2000;
                        // output: ssss ssss ssss ssss snnn nnnn nnnn nnnN
                        buffer[buf_count] = (this_sample<<1)|((this_sample&0x2000)>>13);
#else
                        this_sample &= ~0xF;
                        // input:  ssss ssss ssss ssss ssnn nnnn nnnn 0000
                        // N taps:                       ^^ ^^^            = 0x3E00;
                        // output: ssss ssss ssss ssss snnn nnnn nnnN NNNN
                        buffer[buf_count] = (int16_t)((this_sample << 1) | ((this_sample & 0x3E00) >> 9));
#endif
                    }
                    // Update all counts

                    m_subcycle++;
                    if ((m_subcycle == 2) && (m_PC == 12)) // RESETF3
                    {
                        /* Circuit 412 in the patent acts a reset, resetting the pitch counter to 0
                         * if INHIBIT was true during the most recent frame transition.
                         * The exact time this occurs is betwen IP=7, PC=12 sub=0, T=t12
                         * and m_IP = 0, PC=0 sub=0, T=t12, a period of exactly 20 cycles,
                         * which overlaps the time OLDE and OLDP are updated at IP=7 PC=12 T17
                         * (and hence INHIBIT itself 2 t-cycles later).
                         * According to testing the pitch zeroing lasts approximately 2 samples.
                         * We set the zeroing latch here, and unset it on PC=1 in the generator.
                         */
                        if ((m_IP == 7) && m_inhibit) m_pitch_zero = true;
                        if (m_IP == 7) // RESETL4
                        {
                            // Latch OLDE and OLDP
                            //if (old_frame_silence_flag()) m_uv_zpar = false; // TMS51xx INTERP BUG2
                            m_OLDE = new_frame_silence_flag(); // old_frame_silence_flag()
                            m_OLDP = new_frame_unvoiced_flag(); // old_frame_unvoiced_flag()
                            /* if TALK was clear last frame, halt speech now, since TALKD (latched from TALK on new frame) just went inactive. */

                            LOGMASKED(LOG_GENERATION, "RESETL4, about to update status: IP={0}, PC={1}, subcycle={2}, m_SPEN={3}, m_TALK={4}, m_TALKD={5}\n", m_IP, m_PC, m_subcycle, m_SPEN, m_TALK, m_TALKD);
                            if ((!m_TALK) && (!m_SPEN))
                                LOGMASKED(LOG_GENERATION, "tms5220_process: processing frame: TALKD = 0 caused by stop frame or buffer empty, halting speech.\n");

                            m_TALKD = m_TALK; // TALKD is latched from TALK
                            update_fifo_status_and_ints(); // to trigger an interrupt if talk_status has changed
                            if ((!m_TALK) && m_SPEN) m_TALK = true; // TALK is only activated if it wasn't already active, if m_SPEN is active, and if we're in RESETL4 (which we are).

                            LOGMASKED(LOG_GENERATION, "RESETL4, status updated: IP={0}, PC={1}, subcycle={2}, m_SPEN={3}, m_TALK={4}, m_TALKD={5}\n", m_IP, m_PC, m_subcycle, m_SPEN, m_TALK, m_TALKD);
                        }
                        m_subcycle = m_subc_reload;
                        m_PC = 0;
                        m_IP++;
                        m_IP &= 0x7;
                    }
                    else if (m_subcycle == 3)
                    {
                        m_subcycle = m_subc_reload;
                        m_PC++;
                    }
                    m_pitch_count++;
                    if ((m_pitch_count >= m_current_pitch) || m_pitch_zero) m_pitch_count = 0;
                    m_pitch_count &= 0x1FF;
                }
                else // m_TALKD == 0
                {
                    m_subcycle++;
                    if ((m_subcycle == 2) && (m_PC == 12)) // RESETF3
                    {
                        if (m_IP == 7) // RESETL4
                        {
                            m_TALKD = m_TALK; // TALKD is latched from TALK
                            update_fifo_status_and_ints(); // probably not necessary
                            if ((!m_TALK) && m_SPEN) m_TALK = true; // TALK is only activated if it wasn't already active, if m_SPEN is active, and if we're in RESETL4 (which we are).
                        }
                        m_subcycle = m_subc_reload;
                        m_PC = 0;
                        m_IP++;
                        m_IP &= 0x7;
                    }
                    else if (m_subcycle == 3)
                    {
                        m_subcycle = m_subc_reload;
                        m_PC++;
                    }
                    buffer[buf_count] = -1; /* should be just -1; actual chip outputs -1 every idle sample; (cf note in data sheet, p 10, table 4) */
                }

                buf_count++;
                size--;
            }
        }



        int16_t clip_analog(int16_t cliptemp)
        {
            /* clipping, just like the patent shows:
             * the top 10 bits of this result are visible on the digital output IO pin.
             * next, if the top 3 bits of the 14 bit result are all the same, the
             * lowest of those 3 bits plus the next 7 bits are the signed analog
             * output, otherwise the low bits are all forced to match the inverse of
             * the topmost bit, i.e.:
             * 1x xxxx xxxx xxxx -> 0b10000000
             * 11 1bcd efgh xxxx -> 0b1bcdefgh
             * 00 0bcd efgh xxxx -> 0b0bcdefgh
             * 0x xxxx xxxx xxxx -> 0b01111111
             */
            if ((cliptemp > 2047) || (cliptemp < -2048))
                LOGMASKED(LOG_CLIP, "clipping cliptemp to range; was {0}\n", cliptemp);

            if (cliptemp > 2047) cliptemp = 2047;
            else if (cliptemp < -2048) cliptemp = -2048;

            /* at this point the analog output is tapped */
#if ALLOW_4_LSB
            // input:  ssss snnn nnnn nnnn
            // N taps:       ^^^ ^         = 0x0780
            // output: snnn nnnn nnnn NNNN
            return (cliptemp << 4)|((cliptemp&0x780)>>7); // upshift and range adjust
#else
            cliptemp &= ~0xF;
            // input:  ssss snnn nnnn 0000
            // N taps:       ^^^ ^^^^      = 0x07F0
            // P taps:       ^             = 0x0400
            // output: snnn nnnn NNNN NNNP
            return (int16_t)((cliptemp << 4) | ((cliptemp & 0x7F0) >> 3) | ((cliptemp & 0x400) >> 10)); // upshift and range adjust
#endif
        }


        /**********************************************************************************************
             matrix_multiply -- does the proper multiply and shift
             a is the k coefficient and is clamped to 10 bits (9 bits plus a sign)
             b is the running result and is clamped to 14 bits.
             output is 14 bits, but note the result LSB bit is always 1.
             Because the low 4 bits of the result are trimmed off before
             output, this makes almost no difference in the computation.
        **********************************************************************************************/
        int32_t matrix_multiply(int32_t a, int32_t b)
        {
            int32_t result;
            while (a > 511) { a -= 1024; }
            while (a < -512) { a += 1024; }
            while (b > 16383) { b -= 32768; }
            while (b < -16384) { b += 32768; }
            result = (a * b) >> 9; /** TODO: this isn't technically right to the chip, which truncates the lowest result bit, but it causes glitches otherwise. **/
            if (result > 16383) LOGMASKED(LOG_GENERAL, "matrix multiplier overflowed! a: {0}, b: {1}, result: {2}", a, b, result);
            if (result < -16384) LOGMASKED(LOG_GENERAL, "matrix multiplier underflowed! a: {0}, b: {1}, result: {2}", a, b, result);
            return result;
        }


        /**********************************************************************************************
             lattice_filter -- executes one 'full run' of the lattice filter on a specific byte of
             excitation data, and specific values of all the current k constants,  and returns the
             resulting sample.
        ***********************************************************************************************/
        int32_t lattice_filter()
        {
            // Lattice filter here
            // Aug/05/07: redone as unrolled loop, for clarity - LN
            /* Originally Copied verbatim from table I in US patent 4,209,804, now
              updated to be in same order as the actual chip does it, not that it matters.

              notation equivalencies from table:
              Yn(i) == m_u[n-1]
              Kn = m_current_k[n-1]
              bn = m_x[n-1]
             */
            /*
                int ep = matrix_multiply(m_previous_energy, (m_excitation_data<<6));  //Y(11)
                 m_u[10] = ep;
                for (int i = 0; i < 10; i++)
                {
                    int ii = 10-i; // for m = 10, this would be 11 - i, and since i is from 1 to 10, then ii ranges from 10 to 1
                    // int jj = ii+1; // this variable, even on the fortran version, is
                    // never used. It probably was intended to be used on the two lines
                    // below the next one to save some redundant additions on each.
                    ep = ep - (((m_current_k[ii-1] * m_x[ii-1])>>9)|1); // subtract reflection from lower stage 'top of lattice'
                     m_u[ii-1] = ep;
                    m_x[ii] = m_x[ii-1] + (((m_current_k[ii-1] * ep)>>9)|1); // add reflection from upper stage 'bottom of lattice'
                }
            m_x[0] = ep; // feed the last section of the top of the lattice directly to the bottom of the lattice
            */
                m_u[10] = matrix_multiply(m_previous_energy, (m_excitation_data<<6));  //Y(11)
                m_u[9] = m_u[10] - matrix_multiply(m_current_k[9], m_x[9]);
                m_u[8] = m_u[9] - matrix_multiply(m_current_k[8], m_x[8]);
                m_u[7] = m_u[8] - matrix_multiply(m_current_k[7], m_x[7]);
                m_u[6] = m_u[7] - matrix_multiply(m_current_k[6], m_x[6]);
                m_u[5] = m_u[6] - matrix_multiply(m_current_k[5], m_x[5]);
                m_u[4] = m_u[5] - matrix_multiply(m_current_k[4], m_x[4]);
                m_u[3] = m_u[4] - matrix_multiply(m_current_k[3], m_x[3]);
                m_u[2] = m_u[3] - matrix_multiply(m_current_k[2], m_x[2]);
                m_u[1] = m_u[2] - matrix_multiply(m_current_k[1], m_x[1]);
                m_u[0] = m_u[1] - matrix_multiply(m_current_k[0], m_x[0]);
                int32_t err = m_x[9] + matrix_multiply(m_current_k[9], m_u[9]); //x_10, real chip doesn't use or calculate this
                m_x[9] = m_x[8] + matrix_multiply(m_current_k[8], m_u[8]);
                m_x[8] = m_x[7] + matrix_multiply(m_current_k[7], m_u[7]);
                m_x[7] = m_x[6] + matrix_multiply(m_current_k[6], m_u[6]);
                m_x[6] = m_x[5] + matrix_multiply(m_current_k[5], m_u[5]);
                m_x[5] = m_x[4] + matrix_multiply(m_current_k[4], m_u[4]);
                m_x[4] = m_x[3] + matrix_multiply(m_current_k[3], m_u[3]);
                m_x[3] = m_x[2] + matrix_multiply(m_current_k[2], m_u[2]);
                m_x[2] = m_x[1] + matrix_multiply(m_current_k[1], m_u[1]);
                m_x[1] = m_x[0] + matrix_multiply(m_current_k[0], m_u[0]);
                m_x[0] = m_u[0];
                m_previous_energy = (uint16_t)m_current_energy;

                LOGMASKED(LOG_LATTICE, "V:{0} ", m_u[10]);
                for (int i = 9; i >= 0; i--)
                {
                    LOGMASKED(LOG_LATTICE, "Y{0}:{1} ", i + 1, m_u[i]);
                }

                LOGMASKED(LOG_LATTICE, "\n");
                LOGMASKED(LOG_LATTICE, "E:{0} ", err);

                for (int i = 9; i >= 0; i--)
                {
                    LOGMASKED(LOG_LATTICE, "b{0}:{1} ", i + 1, m_x[i]);
                }

                LOGMASKED(LOG_LATTICE, "\n");

                return m_u[0];
        }


        /**********************************************************************************************
             process_command -- extract a byte from the FIFO and interpret it as a command
        ***********************************************************************************************/
        void process_command(unsigned_char cmd)  //void tms5220_device::process_command(unsigned char cmd)
        {
            LOGMASKED(LOG_COMMAND_DUMP, "process_command called with parameter {0}\n", cmd);

            /* parse the command */
            switch (cmd & 0x70)
            {
            case 0x10 : /* read byte */
                LOGMASKED(LOG_COMMAND_VERBOSE, "Read Byte command received\n");
                if (!talk_status()) /* TALKST must be clear for RDBY */
                {
                    if (m_schedule_dummy_read)
                    {
                        m_schedule_dummy_read = false;
                        if (m_speechrom != null)
                            m_speechrom.read(1);
                    }

                    if (m_speechrom != null)
                        m_read_byte_register = (uint8_t)m_speechrom.read(8);    /* read one byte from speech ROM... */

                    m_RDB_flag = true;
                }
                else
                {
                    LOGMASKED(LOG_COMMAND_VERBOSE, "Read Byte command received during TALK state, ignoring!\n");
                }
                break;

            case 0x00: case 0x20: /* set rate (tms5220c and cd2501ecd only), otherwise NOP */
                if (TMS5220_HAS_RATE_CONTROL)
                {
                    LOGMASKED(LOG_COMMAND_VERBOSE, "Set Rate (or NOP) command received\n");
                    m_c_variant_rate = (uint8_t)(cmd & 0x0F);
                }
                else
                {
                    LOGMASKED(LOG_COMMAND_VERBOSE, "NOP command received\n");
                }
                break;

            case 0x30 : /* read and branch */
                if (!talk_status()) /* TALKST must be clear for RB */
                {
                    LOGMASKED(LOG_COMMAND_VERBOSE, "Read and Branch command received\n");
                    m_RDB_flag = false;
                    if (m_speechrom != null)
                        m_speechrom.read_and_branch();
                }
                break;

            case 0x40 : /* load address */
                LOGMASKED(LOG_COMMAND_VERBOSE, "Load Address command received\n");
                if (!talk_status()) /* TALKST must be clear for LA */
                {
                    /* tms5220 data sheet says that if we load only one 4-bit nibble, it won't work.
                       This code does not care about this. */
                    if (m_speechrom != null)
                        m_speechrom.load_address(cmd & 0x0f);
                    m_schedule_dummy_read = true;
                }
                else
                    LOGMASKED(LOG_COMMAND_VERBOSE, "Load Address command received during TALK state, ignoring!\n");
                break;

            case 0x50 : /* speak */
                LOGMASKED(LOG_COMMAND_VERBOSE, "Speak (VSM) command received\n");
                if (m_schedule_dummy_read)
                {
                    m_schedule_dummy_read = false;
                    if (m_speechrom != null)
                        m_speechrom.read(1);
                }
                m_SPEN = true;  //1;

//#if FAST_START_HACK
                if (FAST_START_HACK)
                    m_TALK = true;  //1;
//#endif

                m_DDIS = false; // speak using VSM
                m_zpar = true; // zero all the parameters
                m_uv_zpar = true; // zero k4-k10 as well
                m_OLDE = true; // 'silence/zpar' frames are zero energy
                m_OLDP = true; // 'silence/zpar' frames are zero pitch
#if TMS5220_PERFECT_INTERPOLATION_HACK
                m_old_zpar = true; // zero all the old parameters
                m_old_uv_zpar = true; // zero old k4-k10 as well
#endif
                // following is semi-hack but matches idle state observed on chip
                m_new_frame_energy_idx = 0;
                m_new_frame_pitch_idx = 0;
                for (int i = 0; i < 4; i++)
                    m_new_frame_k_idx[i] = 0;
                for (int i = 4; i < 7; i++)
                    m_new_frame_k_idx[i] = 0xF;
                for (int i = 7; i < m_coeff.num_k; i++)
                    m_new_frame_k_idx[i] = 0x7;
                break;

            case 0x60 : /* speak external */
                LOGMASKED(LOG_COMMAND_VERBOSE, "Speak External command received\n");

                // SPKEXT going active asserts /SPKEE for 2 clocks, which clears the FIFO and its counters
                m_fifo.Fill((uint8_t)0);  //std::fill(std::begin(m_fifo), std::end(m_fifo), 0);
                m_fifo_head = m_fifo_tail = m_fifo_count = m_fifo_bits_taken = 0;
                // SPEN is enabled when the FIFO passes half full (falling edge of BL signal)
                m_DDIS = true; // speak using FIFO
                m_zpar = true; // zero all the parameters
                m_uv_zpar = true; // zero k4-k10 as well
                m_OLDE = true; // 'silence/zpar' frames are zero energy
                m_OLDP = true; // 'silence/zpar' frames are zero pitch
#if TMS5220_PERFECT_INTERPOLATION_HACK
                m_old_zpar = true; // zero all the old parameters
                m_old_uv_zpar = true; // zero old k4-k10 as well
#endif
                // following is semi-hack but matches idle state observed on chip
                m_new_frame_energy_idx = 0;
                m_new_frame_pitch_idx = 0;
                for (int i = 0; i < 4; i++)
                    m_new_frame_k_idx[i] = 0;
                for (int i = 4; i < 7; i++)
                    m_new_frame_k_idx[i] = 0xF;
                for (int i = 7; i < m_coeff.num_k; i++)
                    m_new_frame_k_idx[i] = 0x7;
                m_RDB_flag = false;
                break;

            case 0x70 : /* reset */
                LOGMASKED(LOG_COMMAND_VERBOSE, "Reset command received\n");
                if (m_schedule_dummy_read)
                {
                    m_schedule_dummy_read = false;
                    if (m_speechrom != null)
                        m_speechrom.read(1);
                }
                reset();
                break;
            }

            /* update the buffer low state */
            update_fifo_status_and_ints();
        }


        /******************************************************************************************
             parse_frame -- parse a new frame's worth of data; returns 0 if not enough bits in buffer
        ******************************************************************************************/
        void parse_frame()
        {
            int i;
            int rep_flag;
#if TMS5220_PERFECT_INTERPOLATION_HACK
            m_old_uv_zpar = m_uv_zpar;
            m_old_zpar = m_zpar;
#endif
            /* Since we're parsing a frame, we must be talking, so clear zpar here.
            Also, before we started parsing a frame, the P=0 and E=0 latches were both
            reset by RESETL4, so clear m_uv_zpar here.
            */
            m_uv_zpar = m_zpar = false;  //0;

            /* We actually don't care how many bits are left in the FIFO here;
            the frame subpart will be processed normally, and any bits extracted
            'past the end' of the FIFO will be read as zeroes; the FIFO being emptied
            will set the /BE latch which will halt speech exactly as if a stop frame
            had been encountered (instead of whatever partial frame was read).
            The same exact circuitry is used for both functions on the real chip, see
            us patent 4335277 sheet 16, gates 232a (decode stop frame) and 232b
            (decode /BE plus DDIS (decode disable) which is active during speak external).
            */

            /* if the chip is a tms5220C, and the rate mode is set to that each frame (0x04 bit set)
            has a 2 bit rate preceding it, grab two bits here and store them as the rate; */
            if (TMS5220_HAS_RATE_CONTROL && (m_c_variant_rate & 0x04) != 0)
            {
                i = extract_bits(2);
                printbits(i, 2);
                LOGMASKED(LOG_PARSE_FRAME_DUMP_BIN | LOG_PARSE_FRAME_DUMP_HEX, " ");
                m_IP = reload_table[i];
            }
            else // non-5220C and 5220C in fixed rate mode
            {
                m_IP = reload_table[m_c_variant_rate & 0x3];
            }

            update_fifo_status_and_ints();
            if (m_DDIS && m_buffer_empty) goto ranout;

            // attempt to extract the energy index
            m_new_frame_energy_idx = (uint8_t)extract_bits(m_coeff.energy_bits);
            printbits(m_new_frame_energy_idx, m_coeff.energy_bits);
            LOGMASKED(LOG_PARSE_FRAME_DUMP_BIN | LOG_PARSE_FRAME_DUMP_HEX, " ");
            update_fifo_status_and_ints();
            if (m_DDIS && m_buffer_empty) goto ranout;
            // if the energy index is 0 or 15, we're done
            if ((m_new_frame_energy_idx == 0) || (m_new_frame_energy_idx == 15))
                return;


            // attempt to extract the repeat flag
            rep_flag = extract_bits(1);
            printbits(rep_flag, 1);
            LOGMASKED(LOG_PARSE_FRAME_DUMP_BIN | LOG_PARSE_FRAME_DUMP_HEX, " ");

            // attempt to extract the pitch
            m_new_frame_pitch_idx = (uint8_t)extract_bits(m_coeff.pitch_bits);
            printbits(m_new_frame_pitch_idx, m_coeff.pitch_bits);
            LOGMASKED(LOG_PARSE_FRAME_DUMP_BIN | LOG_PARSE_FRAME_DUMP_HEX, " ");
            // if the new frame is unvoiced, be sure to zero out the k5-k10 parameters
            m_uv_zpar = new_frame_unvoiced_flag();
            update_fifo_status_and_ints();
            if (m_DDIS && m_buffer_empty) goto ranout;
            // if this is a repeat frame, just do nothing, it will reuse the old coefficients
            if (rep_flag != 0)
                return;

            // extract first 4 K coefficients
            for (i = 0; i < 4; i++)
            {
                m_new_frame_k_idx[i] = (uint8_t)extract_bits(m_coeff.kbits[i]);
                printbits(m_new_frame_k_idx[i], m_coeff.kbits[i]);
                LOGMASKED(LOG_PARSE_FRAME_DUMP_BIN | LOG_PARSE_FRAME_DUMP_HEX, " ");
                update_fifo_status_and_ints();
                if (m_DDIS && m_buffer_empty) goto ranout;
            }

            // if the pitch index was zero, we only need 4 K's...
            if (m_new_frame_pitch_idx == 0)
            {
                /* and the rest of the coefficients are zeroed, but that's done in the generator code */
                return;
            }

            // If we got here, we need the remaining 6 K's
            for (i = 4; i < m_coeff.num_k; i++)
            {
                m_new_frame_k_idx[i] = (uint8_t)extract_bits(m_coeff.kbits[i]);
                printbits(m_new_frame_k_idx[i], m_coeff.kbits[i]);
                LOGMASKED(LOG_PARSE_FRAME_DUMP_BIN | LOG_PARSE_FRAME_DUMP_HEX, " ");
                update_fifo_status_and_ints();
                if (m_DDIS && m_buffer_empty) goto ranout;
            }
            LOGMASKED(LOG_PARSE_FRAME_DUMP_BIN | LOG_PARSE_FRAME_DUMP_HEX, "\n");

            if (m_DDIS)
                LOGMASKED(LOG_GENERAL, "Parsed a frame successfully in FIFO - {0} bits remaining\n", (m_fifo_count * 8) - m_fifo_bits_taken);
            else
                LOGMASKED(LOG_GENERAL, "Parsed a frame successfully in ROM\n");
            return;

            ranout:
            LOGMASKED(LOG_FRAME_ERRORS, "Ran out of bits on a parse!\n");
            return;
        }


        /**********************************************************************************************
             set_interrupt_state -- generate an interrupt
        ***********************************************************************************************/
        void set_interrupt_state(int state)
        {
            if (!TMS5220_IS_52xx) return; // bail out if not a 52xx chip, since there's no int pin

            LOGMASKED(LOG_PIN_READS, "irq pin set to state {0}\n", state);

            if ((state != 0) != m_irq_pin)
            {
                m_irq_pin = state != 0;
                m_irq_handler.op_s32(state == 0 ? 1 : 0);
            }
        }


        /**********************************************************************************************
             update_ready_state -- update the ready line
        ***********************************************************************************************/
        void update_ready_state()
        {
            bool state = m_io_ready;
            if (m_ready_pin != state)
            {
                LOGMASKED(LOG_PIN_READS, "ready pin set to state {0}\n", state);
                if (!m_readyq_handler.isnull())
                    m_readyq_handler.op_s32((!state) ? 1 : 0);
                m_ready_pin = state;
            }
        }


        bool talk_status() { return m_SPEN || m_TALKD; }
        bool old_frame_silence_flag() { return m_OLDE; }  //bool &old_frame_silence_flag() { return m_OLDE; } // 1 if E=0, 0 otherwise.
        bool old_frame_unvoiced_flag() { return m_OLDP; }  //bool &old_frame_unvoiced_flag() { return m_OLDP; } // 1 if P=0 (unvoiced), 0 if voiced
        bool new_frame_stop_flag() { return m_new_frame_energy_idx == 0x0F; } // 1 if this is a stop (Energy = 0xF) frame
        bool new_frame_silence_flag() { return m_new_frame_energy_idx == 0; } // ditto as above
        bool new_frame_unvoiced_flag() { return m_new_frame_pitch_idx == 0; } // ditto as above


        // debugging helper
        /**********************************************************************************************
              printbits helper function: takes a long int input and prints the resulting bits to stderr
        ***********************************************************************************************/
        void printbits(long data, int num)
        {
            for (int i = num - 1; i >= 0; i--)
                LOGMASKED(LOG_PARSE_FRAME_DUMP_BIN, "{0}", (data >> i) & 1);

            switch ((num - 1) & 0xfc)
            {
            case 0:
                LOGMASKED(LOG_PARSE_FRAME_DUMP_HEX, "{0}", data);
                break;
            case 4:
                LOGMASKED(LOG_PARSE_FRAME_DUMP_HEX, "{0}", data);
                break;
            case 8:
                LOGMASKED(LOG_PARSE_FRAME_DUMP_HEX, "{0}", data);
                break;
            case 12:
                LOGMASKED(LOG_PARSE_FRAME_DUMP_HEX, "{0}", data);
                break;
            default:
                LOGMASKED(LOG_PARSE_FRAME_DUMP_HEX, "{0}", data);
                break;
            }
        }
    }


    public class tms5220c_device : tms5220_device
    {
        //DEFINE_DEVICE_TYPE(TMS5220C,  tms5220c_device,  "tms5220c",  "TMS5220C")
        public static readonly emu.detail.device_type_impl TMS5220C = DEFINE_DEVICE_TYPE("tms5220c", "TMS5220C", (type, mconfig, tag, owner, clock) => { return new tms5220c_device(mconfig, tag, owner, clock); });

        tms5220c_device(machine_config mconfig, string tag, device_t owner, uint32_t clock) : base(mconfig, TMS5220C, tag, owner, clock, TMS5220_IS_5220C) { }
    }


    //class cd2501e_device : public tms5220_device
    //class tms5200_device : public tms5220_device
    //class cd2501ecd_device : public tms5220_device


    //DECLARE_DEVICE_TYPE(TMS5220,   tms5220_device)
    //DECLARE_DEVICE_TYPE(TMS5220C,  tms5220c_device)
    //DECLARE_DEVICE_TYPE(CD2501E,   cd2501e_device)
    //DECLARE_DEVICE_TYPE(TMS5200,   tms5200_device)
    //DECLARE_DEVICE_TYPE(CD2501ECD, cd2501ecd_device)


    public static class tms5220_global
    {
        public static tms5220c_device TMS5220C<bool_Required>(machine_config mconfig, device_finder<tms5220c_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, tms5220c_device.TMS5220C, clock); }
    }
}
