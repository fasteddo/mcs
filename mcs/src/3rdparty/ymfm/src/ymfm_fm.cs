// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int16_t = System.Int16;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame.ymfm
{
    static class ymfm_fm_global
    {
        // the values here are stored as 4.8 logarithmic values for 1/4 phase
        // this matches the internal format of the OPN chip, extracted from the die
        static readonly uint16_t [] s_sin_table = new uint16_t[256]
        {
            0x859,0x6c3,0x607,0x58b,0x52e,0x4e4,0x4a6,0x471,0x443,0x41a,0x3f5,0x3d3,0x3b5,0x398,0x37e,0x365,
            0x34e,0x339,0x324,0x311,0x2ff,0x2ed,0x2dc,0x2cd,0x2bd,0x2af,0x2a0,0x293,0x286,0x279,0x26d,0x261,
            0x256,0x24b,0x240,0x236,0x22c,0x222,0x218,0x20f,0x206,0x1fd,0x1f5,0x1ec,0x1e4,0x1dc,0x1d4,0x1cd,
            0x1c5,0x1be,0x1b7,0x1b0,0x1a9,0x1a2,0x19b,0x195,0x18f,0x188,0x182,0x17c,0x177,0x171,0x16b,0x166,
            0x160,0x15b,0x155,0x150,0x14b,0x146,0x141,0x13c,0x137,0x133,0x12e,0x129,0x125,0x121,0x11c,0x118,
            0x114,0x10f,0x10b,0x107,0x103,0x0ff,0x0fb,0x0f8,0x0f4,0x0f0,0x0ec,0x0e9,0x0e5,0x0e2,0x0de,0x0db,
            0x0d7,0x0d4,0x0d1,0x0cd,0x0ca,0x0c7,0x0c4,0x0c1,0x0be,0x0bb,0x0b8,0x0b5,0x0b2,0x0af,0x0ac,0x0a9,
            0x0a7,0x0a4,0x0a1,0x09f,0x09c,0x099,0x097,0x094,0x092,0x08f,0x08d,0x08a,0x088,0x086,0x083,0x081,
            0x07f,0x07d,0x07a,0x078,0x076,0x074,0x072,0x070,0x06e,0x06c,0x06a,0x068,0x066,0x064,0x062,0x060,
            0x05e,0x05c,0x05b,0x059,0x057,0x055,0x053,0x052,0x050,0x04e,0x04d,0x04b,0x04a,0x048,0x046,0x045,
            0x043,0x042,0x040,0x03f,0x03e,0x03c,0x03b,0x039,0x038,0x037,0x035,0x034,0x033,0x031,0x030,0x02f,
            0x02e,0x02d,0x02b,0x02a,0x029,0x028,0x027,0x026,0x025,0x024,0x023,0x022,0x021,0x020,0x01f,0x01e,
            0x01d,0x01c,0x01b,0x01a,0x019,0x018,0x017,0x017,0x016,0x015,0x014,0x014,0x013,0x012,0x011,0x011,
            0x010,0x00f,0x00f,0x00e,0x00d,0x00d,0x00c,0x00c,0x00b,0x00a,0x00a,0x009,0x009,0x008,0x008,0x007,
            0x007,0x007,0x006,0x006,0x005,0x005,0x005,0x004,0x004,0x004,0x003,0x003,0x003,0x002,0x002,0x002,
            0x002,0x001,0x001,0x001,0x001,0x001,0x001,0x001,0x000,0x000,0x000,0x000,0x000,0x000,0x000,0x000
        };

        public static uint32_t abs_sin_attenuation(uint32_t input)
        {
            // if the top bit is set, we're in the second half of the curve
            // which is a mirror image, so invert the index
            if (ymfm_global.bitfield(input, 8) != 0)
                input = ~input;

            // return the value from the table
            return s_sin_table[input & 0xff];
        }
    }

    //*********************************************************
    //  GLOBAL ENUMERATORS
    //*********************************************************

    // three different keyon sources; actual keyon is an OR over all of these
    //enum keyon_type : uint32_t
    //{
    //    KEYON_NORMAL = 0,
    //    KEYON_RHYTHM = 1,
    //    KEYON_CSM = 2
    //};


    //*********************************************************
    //  CORE IMPLEMENTATION
    //*********************************************************
    // ======================> opdata_cache
    // this class holds data that is computed once at the start of clocking
    // and remains static during subsequent sound generation
    //struct opdata_cache


    // ======================> fm_registers_base
    // base class for family-specific register classes; this provides a few
    // constants, common defaults, and helpers, but mostly each derived class is
    // responsible for defining all commonly-called methods
    public class fm_registers_base
    {
        // this value is returned from the write() function for rhythm channels
        //static constexpr uint32_t RHYTHM_CHANNEL = 0xff;

        // this is the size of a full sin waveform
        protected const uint32_t WAVEFORM_LENGTH = 0x400;


        //
        // the following constants need to be defined per family:
        //          uint32_t OUTPUTS: The number of outputs exposed (1-4)
        //         uint32_t CHANNELS: The number of channels on the chip
        //     uint32_t ALL_CHANNELS: A bitmask of all channels
        //        uint32_t OPERATORS: The number of operators on the chip
        //        uint32_t WAVEFORMS: The number of waveforms offered
        //        uint32_t REGISTERS: The number of 8-bit registers allocated
        // uint32_t DEFAULT_PRESCALE: The starting clock prescale
        // uint32_t EG_CLOCK_DIVIDER: The clock divider of the envelope generator
        // uint32_t CSM_TRIGGER_MASK: Mask of channels to trigger in CSM mode
        //         uint32_t REG_MODE: The address of the "mode" register controlling timers
        //     uint8_t STATUS_TIMERA: Status bit to set when timer A fires
        //     uint8_t STATUS_TIMERB: Status bit to set when tiemr B fires
        //       uint8_t STATUS_BUSY: Status bit to set when the chip is busy
        //        uint8_t STATUS_IRQ: Status bit to set when an IRQ is signalled
        //
        // the following constants are uncommon:
        //          bool DYNAMIC_OPS: True if ops/channel can be changed at runtime (OPL3+)
        //       bool EG_HAS_DEPRESS: True if the chip has a DP ("depress"?) envelope stage (OPLL)
        //        bool EG_HAS_REVERB: True if the chip has a faux reverb envelope stage (OPQ/OPZ)
        //           bool EG_HAS_SSG: True if the chip has SSG envelope support (OPN)
        //      bool MODULATOR_DELAY: True if the modulator is delayed by 1 sample (OPL pre-OPL3)
        //
        //static constexpr bool DYNAMIC_OPS = false;
        //static constexpr bool EG_HAS_DEPRESS = false;
        //static constexpr bool EG_HAS_REVERB = false;
        //static constexpr bool EG_HAS_SSG = false;
        //static constexpr bool MODULATOR_DELAY = false;

        // system-wide register defaults
        //uint32_t status_mask() const                     { return 0; } // OPL only
        //uint32_t irq_reset() const                       { return 0; } // OPL only
        //uint32_t noise_enable() const                    { return 0; } // OPM only
        //uint32_t rhythm_enable() const                   { return 0; } // OPL only

        // per-operator register defaults
        //uint32_t op_ssg_eg_enable(uint32_t opoffs) const { return 0; } // OPN(A) only
        //uint32_t op_ssg_eg_mode(uint32_t opoffs) const   { return 0; } // OPN(A) only


        // helper to encode four operator numbers into a 32-bit value in the
        // operator maps for each register class
        protected static uint32_t operator_list(uint8_t o1 = 0xff, uint8_t o2 = 0xff, uint8_t o3 = 0xff, uint8_t o4 = 0xff)
        {
            return o1 | ((uint32_t)o2 << 8) | ((uint32_t)o3 << 16) | ((uint32_t)o4 << 24);
        }


        // helper to apply KSR to the raw ADSR rate, ignoring ksr if the
        // raw value is 0, and clamping to 63
        //static constexpr uint32_t effective_rate(uint32_t rawrate, uint32_t ksr)
        //{
        //    return (rawrate == 0) ? 0 : std::min<uint32_t>(rawrate + ksr, 63);
        //}
    }


    //*********************************************************
    //  CORE ENGINE CLASSES
    //*********************************************************

    // ======================> fm_operator
    // fm_operator represents an FM operator (or "slot" in FM parlance), which
    // produces an output sine wave modulated by an envelope
    //template<class RegisterType>
    class fm_operator<RegisterType, RegisterType_OPS>
        where RegisterType : opm_registers, new()
        where RegisterType_OPS : fm_engine_base_operators, new()
    {
        // "quiet" value, used to optimize when we can skip doing working
        //static constexpr uint32_t EG_QUIET = 0x200;


        // internal state
        uint32_t m_choffs;                     // channel offset in registers
        uint32_t m_opoffs;                     // operator offset in registers
        uint32_t m_phase;                      // current phase value (10.10 format)
        uint16_t m_env_attenuation;            // computed envelope attenuation (4.6 format)
        envelope_state m_env_state;            // current envelope state
        uint8_t m_ssg_inverted;                // non-zero if the output should be inverted (bit 0)
        uint8_t m_key_state;                   // current key state: on or off (bit 0)
        uint8_t m_keyon_live;                  // live key on state (bit 0 = direct, bit 1 = rhythm, bit 2 = CSM)
        //opdata_cache m_cache;                  // cached values for performance
        RegisterType m_regs;                  // direct reference to registers
        fm_engine_base<RegisterType, RegisterType_OPS> m_owner; // reference to the owning engine


        // constructor
        public fm_operator(fm_engine_base<RegisterType, RegisterType_OPS> owner, uint32_t opoffs)
        {
            m_choffs = 0;
            m_opoffs = opoffs;
            m_phase = 0;
            m_env_attenuation = 0x3ff;
            m_env_state = envelope_state.EG_RELEASE;
            m_ssg_inverted = 0;  //false;
            m_key_state = 0;
            m_keyon_live = 0;
            m_regs = owner.regs();
            m_owner = owner;
        }


        // save/restore
        //void save_restore(ymfm_saved_state &state);

        // reset the operator state
        //void reset();

        // return the operator/channel offset
        //uint32_t opoffs() const { return m_opoffs; }
        //uint32_t choffs() const { return m_choffs; }

        // set the current channel
        public void set_choffs(uint32_t choffs) { m_choffs = choffs; }

        // prepare prior to clocking
        //bool prepare();

        // master clocking function
        //void clock(uint32_t env_counter, int32_t lfo_raw_pm);

        // return the current phase value
        //uint32_t phase() const { return m_phase >> 10; }

        // compute operator volume
        //int32_t compute_volume(uint32_t phase, uint32_t am_offset) const;

        // compute volume for the OPM noise channel
        //int32_t compute_noise_volume(uint32_t am_offset) const;

        // key state control
        //void keyonoff(uint32_t on, keyon_type type);

        // return a reference to our registers
        //RegisterType &regs() const { return m_regs; }

        // simple getters for debugging
        //envelope_state debug_eg_state() const { return m_env_state; }
        //uint16_t debug_eg_attenuation() const { return m_env_attenuation; }
        //opdata_cache &debug_cache() { return m_cache; }


        // start the attack phase
        //void start_attack(bool is_restart = false);

        // start the release phase
        //void start_release();

        // clock phases
        //void clock_keystate(uint32_t keystate);
        //void clock_ssg_eg_state();
        //void clock_envelope(uint32_t env_counter);
        //void clock_phase(int32_t lfo_raw_pm);

        // return effective attenuation of the envelope
        //uint32_t envelope_attenuation(uint32_t am_offset) const;
    }


    // ======================> fm_channel
    // fm_channel represents an FM channel which combines the output of 2 or 4
    // operators into a final result
    //template<class RegisterType>
    class fm_channel<RegisterType, RegisterType_OPS>
        where RegisterType : opm_registers, new()
        where RegisterType_OPS : fm_engine_base_operators, new()
    {
        //using output_data = ymfm_output<RegisterType::OUTPUTS>;


        // internal state
        uint32_t m_choffs;                     // channel offset in registers
        int16_t [] m_feedback = new int16_t [2];                 // feedback memory for operator 1
        int16_t m_feedback_in;  //mutable int16_t m_feedback_in;         // next input value for op 1 feedback (set in output)
        fm_operator<RegisterType, RegisterType_OPS> [] m_op = new ymfm.fm_operator<RegisterType, RegisterType_OPS> [4];    // up to 4 operators
        RegisterType m_regs;  //RegisterType &m_regs;                  // direct reference to registers
        fm_engine_base<RegisterType, RegisterType_OPS> m_owner;  //fm_engine_base<RegisterType> &m_owner; // reference to the owning engine


        // constructor
        public fm_channel(fm_engine_base<RegisterType, RegisterType_OPS> owner, uint32_t choffs)
        {
            m_choffs = choffs;
            m_feedback = new int16_t[] { 0, 0 };
            m_feedback_in = 0;
            m_op = new ymfm.fm_operator<RegisterType, RegisterType_OPS>[] { null, null, null, null };
            m_regs = owner.regs();
            m_owner = owner;
        }


        // save/restore
        //void save_restore(ymfm_saved_state &state);

        // reset the channel state
        //void reset();

        // return the channel offset
        //uint32_t choffs() const { return m_choffs; }


        // assign operators
        public void assign(uint32_t index, fm_operator<RegisterType, RegisterType_OPS> op)
        {
            g.assert(index < m_op.Length);
            m_op[index] = op;
            if (op != null)
                op.set_choffs(m_choffs);
        }


        // signal key on/off to our operators
        //void keyonoff(uint32_t states, keyon_type type, uint32_t chnum);

        // prepare prior to clocking
        //bool prepare();

        // master clocking function
        //void clock(uint32_t env_counter, int32_t lfo_raw_pm);

        // specific 2-operator and 4-operator output handlers
        //void output_2op(output_data &output, uint32_t rshift, int32_t clipmax) const;
        //void output_4op(output_data &output, uint32_t rshift, int32_t clipmax) const;

        // compute the special OPL rhythm channel outputs
        //void output_rhythm_ch6(output_data &output, uint32_t rshift, int32_t clipmax) const;
        //void output_rhythm_ch7(uint32_t phase_select, output_data &output, uint32_t rshift, int32_t clipmax) const;
        //void output_rhythm_ch8(uint32_t phase_select, output_data &output, uint32_t rshift, int32_t clipmax) const;

        // are we a 4-operator channel or a 2-operator one?
        //bool is4op() const
        //{
        //    if (RegisterType::DYNAMIC_OPS)
        //        return (m_op[2] != nullptr);
        //    return (RegisterType::OPERATORS / RegisterType::CHANNELS == 4);
        //}

        // return a reference to our registers
        //RegisterType &regs() const { return m_regs; }

        // simple getters for debugging
        //fm_operator<RegisterType> *debug_operator(uint32_t index) const { return m_op[index]; }


        // helper to add values to the outputs based on channel enables
        //void add_to_output(uint32_t choffs, output_data &output, int32_t value) const
        //{
        //    // create these constants to appease overzealous compilers checking array
        //    // bounds in unreachable code (looking at you, clang)
        //    constexpr int out0_index = 0;
        //    constexpr int out1_index = 1 % RegisterType::OUTPUTS;
        //    constexpr int out2_index = 2 % RegisterType::OUTPUTS;
        //    constexpr int out3_index = 3 % RegisterType::OUTPUTS;
        //
        //    if (RegisterType::OUTPUTS == 1 || m_regs.ch_output_0(choffs))
        //        output.data[out0_index] += value;
        //    if (RegisterType::OUTPUTS >= 2 && m_regs.ch_output_1(choffs))
        //        output.data[out1_index] += value;
        //    if (RegisterType::OUTPUTS >= 3 && m_regs.ch_output_2(choffs))
        //        output.data[out2_index] += value;
        //    if (RegisterType::OUTPUTS >= 4 && m_regs.ch_output_3(choffs))
        //        output.data[out3_index] += value;
        //}
    }


    public interface fm_engine_base_operators
    {
        uint32_t OUTPUTS { get; }
        uint32_t CHANNELS { get; }
        uint32_t ALL_CHANNELS { get; }
        uint32_t OPERATORS { get; }
        uint32_t DEFAULT_PRESCALE { get; }
        uint8_t STATUS_TIMERA { get; }
        uint8_t STATUS_TIMERB { get; }

        uint32_t channel_offset(uint32_t chnum);
        uint32_t operator_offset(uint32_t opnum);
    }


    // ======================> fm_engine_base
    // fm_engine_base represents a set of operators and channels which together
    // form a Yamaha FM core; chips that implement other engines (ADPCM, wavetable,
    // etc) take this output and combine it with the others externally
    //template<class RegisterType>
    public class fm_engine_base<RegisterType, RegisterType_OPS> : ymfm_engine_callbacks
        where RegisterType : opm_registers, new()
        where RegisterType_OPS : fm_engine_base_operators, new()
    {
        static readonly RegisterType_OPS register_ops = new RegisterType_OPS();


        // expose some constants from the registers
        public static readonly uint32_t OUTPUTS = register_ops.OUTPUTS;
        static readonly uint32_t CHANNELS = register_ops.CHANNELS;
        static readonly uint32_t ALL_CHANNELS = register_ops.ALL_CHANNELS;
        static readonly uint32_t OPERATORS = register_ops.OPERATORS;

        // also expose status flags for consumers that inject additional bits
        static readonly uint8_t STATUS_TIMERA = register_ops.STATUS_TIMERA;
        static readonly uint8_t STATUS_TIMERB = register_ops.STATUS_TIMERB;
        //static constexpr uint8_t STATUS_BUSY = RegisterType::STATUS_BUSY;
        //static constexpr uint8_t STATUS_IRQ = RegisterType::STATUS_IRQ;

        public class int_const_OUTPUTS : int_const { public int value { get { throw new emu_unimplemented(); } } }


        // expose the correct output class
        //using output_data = ymfm_output<OUTPUTS>;
        public class output_data : ymfm_output<int_const_OUTPUTS>
        {
        }


        // internal state
        ymfm_interface m_intf;          // reference to the system interface
        uint32_t m_env_counter;          // envelope counter; low 2 bits are sub-counter
        uint8_t m_status;                // current status register
        uint8_t m_clock_prescale;        // prescale factor (2/3/6)
        uint8_t m_irq_mask;              // mask of which bits signal IRQs
        uint8_t m_irq_state;             // current IRQ state
        uint8_t [] m_timer_running = new uint8_t[2];      // current timer running state
        uint32_t m_active_channels;      // mask of active channels (computed by prepare)
        uint32_t m_modified_channels;    // mask of channels that have been modified
        uint32_t m_prepare_count;        // counter to do periodic prepare sweeps
        RegisterType m_regs;             // register accessor
        fm_channel<RegisterType, RegisterType_OPS> [] m_channel = new fm_channel<RegisterType, RegisterType_OPS>[CHANNELS]; // channel pointers  //std::unique_ptr<fm_channel<RegisterType>> m_channel[CHANNELS]; // channel pointers
        fm_operator<RegisterType, RegisterType_OPS> [] m_operator = new fm_operator<RegisterType, RegisterType_OPS>[OPERATORS]; // operator pointers  //std::unique_ptr<fm_operator<RegisterType>> m_operator[OPERATORS]; // operator pointers


        // constructor
        public fm_engine_base(ymfm_interface intf)
        {
            m_intf = intf;
            m_env_counter = 0;
            m_status = 0;
            m_clock_prescale = (uint8_t)register_ops.DEFAULT_PRESCALE;
            m_irq_mask = (uint8_t)(STATUS_TIMERA | STATUS_TIMERB);
            m_irq_state = 0;
            m_timer_running = new uint8_t[] {0, 0};
            m_active_channels = ALL_CHANNELS;
            m_modified_channels = ALL_CHANNELS;
            m_prepare_count = 0;


            // inform the interface of their engine
            m_intf.m_engine = this;


            m_regs = new RegisterType();


            // create the channels
            for (uint32_t chnum = 0; chnum < CHANNELS; chnum++)
                m_channel[chnum] = new fm_channel<RegisterType, RegisterType_OPS>(this, register_ops.channel_offset(chnum));

            // create the operators
            for (uint32_t opnum = 0; opnum < OPERATORS; opnum++)
                m_operator[opnum] = new fm_operator<RegisterType, RegisterType_OPS>(this, register_ops.operator_offset(opnum));

            // do the initial operator assignment
            assign_operators();
        }


        // save/restore
        //void save_restore(ymfm_saved_state &state);

        // reset the overall state
        //void reset();

        // master clocking function
        //uint32_t clock(uint32_t chanmask);

        // compute sum of channel outputs
        //void output(output_data &output, uint32_t rshift, int32_t clipmax, uint32_t chanmask) const;

        // write to the OPN registers
        //void write(uint16_t regnum, uint8_t data);

        // return the current status
        //uint8_t status() const;

        // set/reset bits in the status register, updating the IRQ status
        //uint8_t set_reset_status(uint8_t set, uint8_t reset)
        //{
        //    m_status = (m_status | set) & ~(reset | STATUS_BUSY);
        //    m_intf.ymfm_sync_check_interrupts();
        //    return m_status & ~m_regs.status_mask();
        //}

        // set the IRQ mask
        //void set_irq_mask(uint8_t mask) { m_irq_mask = mask; m_intf.ymfm_sync_check_interrupts(); }

        // return the current clock prescale
        //uint32_t clock_prescale() const { return m_clock_prescale; }

        // set prescale factor (2/3/6)
        //void set_clock_prescale(uint32_t prescale) { m_clock_prescale = prescale; }

        // compute sample rate
        public uint32_t sample_rate(uint32_t baseclock) { throw new emu_unimplemented(); }  //{ return baseclock / (m_clock_prescale * OPERATORS); }

        // return the owning device
        //ymfm_interface &intf() const { return m_intf; }

        // return a reference to our registers
        public RegisterType regs() { return m_regs; }

        // invalidate any caches
        //void invalidate_caches() { m_modified_channels = RegisterType::ALL_CHANNELS; }

        // simple getters for debugging
        //fm_channel<RegisterType> *debug_channel(uint32_t index) const { return m_channel[index].get(); }
        //fm_operator<RegisterType> *debug_operator(uint32_t index) const { return m_operator[index].get(); }


        // timer callback; called by the interface when a timer fires
        public void engine_timer_expired(uint32_t tnum)
        {
            throw new emu_unimplemented();
        }


        // check interrupts; called by the interface after synchronization
        public void engine_check_interrupts()
        {
            throw new emu_unimplemented();
        }


        // mode register write; called by the interface after synchronization
        public void engine_mode_write(uint8_t data)
        {
            throw new emu_unimplemented();
        }


        // assign the current set of operators to channels
        void assign_operators()
        {
            opm_registers.operator_mapping map;  //typename RegisterType::operator_mapping map;
            m_regs.operator_map(out map);

            for (uint32_t chnum = 0; chnum < CHANNELS; chnum++)
            {
                for (uint32_t index = 0; index < 4; index++)
                {
                    uint32_t opnum = ymfm_global.bitfield(map.chan[chnum], (int)(8 * index), 8);
                    m_channel[chnum].assign(index, (opnum == 0xff) ? null : m_operator[opnum]);
                }
            }
        }


        // update the state of the given timer
        //void update_timer(uint32_t which, uint32_t enable);
    }
}
