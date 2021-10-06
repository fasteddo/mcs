// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devcb_write_line = mame.devcb_write<int, uint, mame.devcb_operators_s32_u32, mame.devcb_operators_u32_s32, mame.devcb_constant_1<uint, uint, mame.devcb_operators_u32_u32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using s8 = System.SByte;
using s16 = System.Int16;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using ymopm_engine = mame.ymfm_engine_base<mame.ymopm_registers, mame.ymfm_engine_base_operators_ymopm_registers>;  //using ymopm_engine = ymfm_engine_base<ymopm_registers>;


namespace mame
{
    //*********************************************************
    //  MACROS
    //*********************************************************

    // special naming helper to keep our namespace isolated from other
    // same-named objects in the device's namespace (mostly necessary
    // for chips which derive from AY-8910 classes and may have clashing
    // names)
    //#define YMFM_NAME(x) x, "ymfm." #x


    //*********************************************************
    //  GLOBAL ENUMERATORS
    //*********************************************************

    enum ymfm_envelope_state : u32
    {
        YMFM_ENV_DEPRESS = 0,
        YMFM_ENV_ATTACK = 1,
        YMFM_ENV_DECAY = 2,
        YMFM_ENV_SUSTAIN = 3,
        YMFM_ENV_RELEASE = 4,
        YMFM_ENV_STATES = 5
    }


    //*********************************************************
    //  GLOBAL HELPERS
    //*********************************************************

    //inline s16 ymfm_encode_fp(s32 value)

    //inline s16 ymfm_decode_fp(s16 value)

    //inline s16 ymfm_roundtrip_fp(s32 value)

    static class ymfm_global
    {
        //*********************************************************
        //  GLOBAL TABLE LOOKUPS
        //*********************************************************

        //-------------------------------------------------
        //  abs_sin_attenuation - given a sin (phase) input
        //  where the range 0-2*PI is mapped onto 10 bits,
        //  return the absolute value of sin(input),
        //  logarithmically-adjusted and treated as an
        //  attenuation value, in 4.8 fixed point format
        //-------------------------------------------------

        // the values here are stored as 4.8 logarithmic values for 1/4 phase
        // this matches the internal format of the OPN chip, extracted from the die
        static readonly u16 [] s_sin_table = new u16 [256]
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

        public static u32 abs_sin_attenuation(u32 input)
        {
            // if the top bit is set, we're in the second half of the curve
            // which is a mirror image, so invert the index
            if (g.BIT(input, 8) != 0)
                input = ~input;

            // return the value from the table
            return s_sin_table[input & 0xff];
        }
    }


    //*********************************************************
    //  DEBUGGING
    //*********************************************************

    // set this mask to only play certain channels
    //constexpr u32 global_chanmask = 0xffffffff;


    //*********************************************************
    //  REGISTER CLASSES
    //*********************************************************

    // ======================> ymfm_opdata_cache

    // this class holds data that is computed once at the start of clocking
    // and remains static during subsequent sound generation
    //struct ymfm_opdata_cache


    // ======================> ymfm_registers_base
    // base class for family-specific register classes; this provides a few
    // constants, common defaults, and helpers, but mostly each derived
    // class is responsible for defining all commonly-called methods
    class ymfm_registers_base : global_object
    {
        // this value is returned from the write() function for rhythm channels
        public const u32 YMFM_RHYTHM_CHANNEL = 0xff;

        // this is the size of a full sin waveform
        protected const u32 WAVEFORM_LENGTH = 0x400;


        //
        // the following constants need to be defined per family:
        //          u32 OUTPUTS: The number of outputs exposed (1-4)
        //         u32 CHANNELS: The number of channels on the chip
        //     u32 ALL_CHANNELS: A bitmask of all channels
        //        u32 OPERATORS: The number of operators on the chip
        //     bool DYNAMIC_OPS: True if ops/channel can be changed at runtime
        //        u32 WAVEFORMS: The number of waveforms offered
        //        u32 REGISTERS: The number of 8-bit registers allocated
        //         u32 REG_MODE: The address of the "mode" register controlling timers
        // u32 DEFAULT_PRESCALE: The starting clock prescale
        // u32 EG_CLOCK_DIVIDER: The clock divider of the envelope generator
        //  bool EG_HAS_DEPRESS: True if the chip has a DP ("depress"?) envelope stage
        //      bool EG_HAS_SSG: True if the chip has SSG envelope support
        // bool MODULATOR_DELAY: True if the modulator is delayed by 1 sample (OPL pre-OPL3)
        // u32 CSM_TRIGGER_MASK: Mask of channels to trigger in CSM mode
        //     u8 STATUS_TIMERA: Status bit to set when timer A fires
        //     u8 STATUS_TIMERB: Status bit to set when tiemr B fires
        //       u8 STATUS_BUSY: Status bit to set when the chip is busy
        //        u8 STATUS_IRQ: Status bit to set when an IRQ is signalled
        //

        // system-wide register defaults
        public u32 status_mask() { return 0; } // OPL only
        public u32 irq_reset() { return 0; } // OPL only
        //u32 noise_enable() const               { return 0; } // OPM only
        //u32 rhythm_enable() const              { return 0; } // OPL only

        // per-operator register defaults
        //u32 op_ssg_eg_enable(u32 opoffs) const { return 0; } // OPN(A) only
        //u32 op_ssg_eg_mode(u32 opoffs) const   { return 0; } // OPN(A) only

        // helper to encode four operator numbers into a 32-bit value in the
        // operator maps for each register class
        protected static u32 operator_list(u8 o1 = 0xff, u8 o2 = 0xff, u8 o3 = 0xff, u8 o4 = 0xff)
        {
            return o1 | ((u32)o2 << 8) | ((u32)o3 << 16) | ((u32)o4 << 24);
        }

        // helper to apply KSR to the raw ADSR rate, ignoring ksr if the
        // raw value is 0, and clamping to 63
        //static constexpr u32 effective_rate(u32 rawrate, u32 ksr)
        //{
        //    return (rawrate == 0) ? 0 : std::min<u32>(rawrate + ksr, 63);
        //}
    }


    // ======================> ymopm_registers
    class ymopm_registers : ymfm_registers_base
    {
        // LFO waveforms are 256 entries long
        const u32 LFO_WAVEFORM_LENGTH = 256;


        // constants
        //static constexpr u32 OUTPUTS = 2;
        public const u32 CHANNELS = 8;
        public const u32 ALL_CHANNELS = (1U << (int)CHANNELS) - 1;
        public const u32 OPERATORS = CHANNELS * 4;
        //static constexpr bool DYNAMIC_OPS = false;
        const u32 WAVEFORMS = 1;
        const u32 REGISTERS = 0x100;
        public const u32 REG_MODE = 0x14;
        public const u32 DEFAULT_PRESCALE = 2;
        //static constexpr u32 EG_CLOCK_DIVIDER = 3;
        //static constexpr bool EG_HAS_DEPRESS = false;
        //static constexpr bool EG_HAS_SSG = false;
        //static constexpr bool MODULATOR_DELAY = false;
        //static constexpr u32 CSM_TRIGGER_MASK = ALL_CHANNELS;
        public const u8 STATUS_TIMERA = 0x01;
        public const u8 STATUS_TIMERB = 0x02;
        public const u8 STATUS_BUSY = 0x80;
        public const u8 STATUS_IRQ = 0;


        // internal state
        u32 m_lfo_counter;               // LFO counter
        u32 m_noise_lfsr;                // noise LFSR state
        u8 m_noise_counter;              // noise counter
        u8 m_noise_state;                // latched noise state
        u8 m_noise_lfo;                  // latched LFO noise value
        u8 m_lfo_am;                     // current LFO AM value
        u8 [] m_regdata = new u8[REGISTERS];         // register data
        s16 [,] m_lfo_waveform = new s16[4, LFO_WAVEFORM_LENGTH]; // LFO waveforms; AM in low 8, PM in upper 8  //s16 m_lfo_waveform[4][LFO_WAVEFORM_LENGTH]; // LFO waveforms; AM in low 8, PM in upper 8
        u16 [,] m_waveform = new u16[WAVEFORMS, WAVEFORM_LENGTH]; // waveforms


        // constructor
        public ymopm_registers()
        {
            m_lfo_counter = 0;
            m_noise_lfsr = 1;
            m_noise_counter = 0;
            m_noise_state = 0;
            m_noise_lfo = 0;
            m_lfo_am = 0;


            // create the waveforms
            for (int index = 0; index < WAVEFORM_LENGTH; index++)
                m_waveform[0, index] = (u16)(g.abs_sin_attenuation((u32)index) | (g.BIT((u32)index, 9) << 15));

            // create the LFO waveforms; AM in the low 8 bits, PM in the upper 8
            // waveforms are adjusted to match the pictures in the application manual
            for (int index = 0; index < LFO_WAVEFORM_LENGTH; index++)
            {
                // waveform 0 is a sawtooth
                u8 am = (u8)(index ^ 0xff);
                s8 pm = (s8)index;
                m_lfo_waveform[0, index] = (s16)(am | (pm << 8));

                // waveform 1 is a square wave
                am = g.BIT(index, 7) != 0 ? (u8)0 : (u8)0xff;
                pm = (s8)(am ^ 0x80);
                m_lfo_waveform[1, index] = (s16)(am | (pm << 8));

                // waveform 2 is a triangle wave
                am = g.BIT(index, 7) != 0 ? (u8)(index << 1) : (u8)((index ^ 0xff) << 1);
                pm = (s8)(g.BIT(index, 6) != 0 ? am : ~am);
                m_lfo_waveform[2, index] = (s16)(am | (pm << 8));

                // waveform 3 is noise; it is filled in dynamically
            }
        }


        // register for save states
        //void save(device_t &device);

        // reset to initial state
        //void reset();


        // map channel number to register offset
        public static u32 channel_offset(u32 chnum)
        {
            assert(chnum < CHANNELS);
            return chnum;
        }

        // map operator number to register offset
        public static u32 operator_offset(u32 opnum)
        {
            assert(opnum < OPERATORS);
            return opnum;
        }


        // return an array of operator indices for each channel
        //-------------------------------------------------
        //  operator_map - return an array of operator
        //  indices for each channel; for OPM this is fixed
        //-------------------------------------------------

        public class operator_mapping { public u32 [] chan = new u32[CHANNELS]; };

        // Note that the channel index order is 0,2,1,3, so we bitswap the index.
        //
        // This is because the order in the map is:
        //    carrier 1, carrier 2, modulator 1, modulator 2
        //
        // But when wiring up the connections, the more natural order is:
        //    carrier 1, modulator 1, carrier 2, modulator 2
        static readonly operator_mapping s_fixed_map = new operator_mapping()
        {
            chan = new u32 []
            {
                operator_list(  0, 16,  8, 24 ),  // Channel 0 operators
                operator_list(  1, 17,  9, 25 ),  // Channel 1 operators
                operator_list(  2, 18, 10, 26 ),  // Channel 2 operators
                operator_list(  3, 19, 11, 27 ),  // Channel 3 operators
                operator_list(  4, 20, 12, 28 ),  // Channel 4 operators
                operator_list(  5, 21, 13, 29 ),  // Channel 5 operators
                operator_list(  6, 22, 14, 30 ),  // Channel 6 operators
                operator_list(  7, 23, 15, 31 ),  // Channel 7 operators
            }
        };

        public void operator_map(out operator_mapping dest)
        {
            dest = s_fixed_map;
        }


        // handle writes to the register array
        //-------------------------------------------------
        //  write - handle writes to the register array
        //-------------------------------------------------
        public bool write(u16 index, u8 data, out u32 channel, out u32 opmask)
        {
            assert(index < REGISTERS);

            // LFO AM/PM depth are written to the same register (0x19);
            // redirect the PM depth to an unused neighbor (0x1a)
            if (index == 0x19)
                m_regdata[index + g.BIT(data, 7)] = data;
            else if (index != 0x1a)
                m_regdata[index] = data;

            // handle writes to the key on index
            if (index == 0x08)
            {
                channel = g.BIT(data, 0, 3);
                opmask = g.BIT(data, 3, 4);
                return true;
            }

            channel = 0;
            opmask = 0;
            return false;
        }


        // clock the noise and LFO, if present, returning LFO PM value
        //s32 clock_noise_and_lfo();

        // reset the LFO
        public void reset_lfo() { m_lfo_counter = 0; }

        // return the AM offset from LFO for the given channel
        //u32 lfo_am_offset(u32 choffs) const;

        // return the current noise state, gated by the noise clock
        //u32 noise_state() const { return m_noise_state; }

        // caching helpers
        //void cache_operator_data(u32 choffs, u32 opoffs, ymfm_opdata_cache &cache);

        // compute the phase step, given a PM value
        //u32 compute_phase_step(u32 choffs, u32 opoffs, ymfm_opdata_cache const &cache, s32 lfo_raw_pm);

        // log a key-on event
        //void log_keyon(u32 choffs, u32 opoffs);

        // system-wide registers
        //u32 test() const                       { return byte(0x01, 0, 8); }
        //u32 noise_frequency() const            { return byte(0x0f, 0, 5); }
        //u32 noise_enable() const               { return byte(0x0f, 7, 1); }
        public u32 timer_a_value() { return word(0x10, 0, 8, 0x11, 0, 2); }
        public u32 timer_b_value() { return byte_(0x12, 0, 8); }
        //u32 csm() const                        { return byte(0x14, 7, 1); }
        public u32 reset_timer_b() { return byte_(0x14, 5, 1); }
        public u32 reset_timer_a() { return byte_(0x14, 4, 1); }
        //u32 enable_timer_b() const             { return byte(0x14, 3, 1); }
        //u32 enable_timer_a() const             { return byte(0x14, 2, 1); }
        public u32 load_timer_b() { return byte_(0x14, 1, 1); }
        public u32 load_timer_a() { return byte_(0x14, 0, 1); }
        //u32 lfo_rate() const                   { return byte(0x18, 0, 8); }
        //u32 lfo_am_depth() const               { return byte(0x19, 0, 7); }
        //u32 lfo_pm_depth() const               { return byte(0x1a, 0, 7); }
        //u32 lfo_waveform() const               { return byte(0x1b, 0, 2); }

        // per-channel registers
        //u32 ch_output_any(u32 choffs) const    { return byte(0x20, 6, 2, choffs); }
        //u32 ch_output_0(u32 choffs) const      { return byte(0x20, 6, 1, choffs); }
        //u32 ch_output_1(u32 choffs) const      { return byte(0x20, 7, 1, choffs); }
        //u32 ch_output_2(u32 choffs) const      { return 0; }
        //u32 ch_output_3(u32 choffs) const      { return 0; }
        //u32 ch_feedback(u32 choffs) const      { return byte(0x20, 3, 3, choffs); }
        //u32 ch_algorithm(u32 choffs) const     { return byte(0x20, 0, 3, choffs); }
        //u32 ch_block_freq(u32 choffs) const    { return word(0x28, 0, 7, 0x30, 2, 6, choffs); }
        //u32 ch_lfo_pm_sens(u32 choffs) const   { return byte(0x38, 4, 3, choffs); }
        //u32 ch_lfo_am_sens(u32 choffs) const   { return byte(0x38, 0, 2, choffs); }

        // per-operator registers
        //u32 op_detune(u32 opoffs) const        { return byte(0x40, 4, 3, opoffs); }
        //u32 op_multiple(u32 opoffs) const      { return byte(0x40, 0, 4, opoffs); }
        //u32 op_total_level(u32 opoffs) const   { return byte(0x60, 0, 7, opoffs); }
        //u32 op_ksr(u32 opoffs) const           { return byte(0x80, 6, 2, opoffs); }
        //u32 op_attack_rate(u32 opoffs) const   { return byte(0x80, 0, 5, opoffs); }
        //u32 op_lfo_am_enable(u32 opoffs) const { return byte(0xa0, 7, 1, opoffs); }
        //u32 op_decay_rate(u32 opoffs) const    { return byte(0xa0, 0, 5, opoffs); }
        //u32 op_detune2(u32 opoffs) const       { return byte(0xc0, 6, 2, opoffs); }
        //u32 op_sustain_rate(u32 opoffs) const  { return byte(0xc0, 0, 5, opoffs); }
        //u32 op_sustain_level(u32 opoffs) const { return byte(0xe0, 4, 4, opoffs); }
        //u32 op_release_rate(u32 opoffs) const  { return byte(0xe0, 0, 4, opoffs); }


        // return a bitfield extracted from a byte
        u32 byte_(u32 offset, u32 start, u32 count, u32 extra_offset = 0)
        {
            return g.BIT(m_regdata[offset + extra_offset], start, count);
        }

        // return a bitfield extracted from a pair of bytes, MSBs listed first
        u32 word(u32 offset1, u32 start1, u32 count1, u32 offset2, u32 start2, u32 count2, u32 extra_offset = 0)
        {
            return (byte_(offset1, start1, count1, extra_offset) << (int)count2) | byte_(offset2, start2, count2, extra_offset);
        }
    }


    // ======================> ymopn_registers
    //template<bool IsOpnA>
    //class ymopn_registers_base : public ymfm_registers_base

    //using ymopn_registers = ymopn_registers_base<false>;
    //using ymopna_registers = ymopn_registers_base<true>;


    // ======================> ymopl_registers_base
    //template<int Revision>
    //class ymopl_registers_base : public ymfm_registers_base

    //using ymopl_registers = ymopl_registers_base<1>;
    //using ymopl2_registers = ymopl_registers_base<2>;
    //using ymopl3_registers = ymopl_registers_base<3>;
    //using ymopl4_registers = ymopl_registers_base<4>;


    // ======================> ymopll_registers
    //class ymopll_registers : public ymfm_registers_base


    //*********************************************************
    //  CORE ENGINE CLASSES
    //*********************************************************

    // three different keyon sources; actual keyon is an OR over all of these
    enum ymfm_keyon_type : u32
    {
        YMFM_KEYON_NORMAL = 0,
        YMFM_KEYON_RHYTHM = 1,
        YMFM_KEYON_CSM = 2
    }


    // ======================> ymfm_operator
    // ymfm_operator represents an FM operator (or "slot" in FM parlance), which
    // produces an output sine wave modulated by an envelope
    //template<class RegisterType>
    class ymfm_operator<RegisterType, RegisterType_OPS>
        where RegisterType : ymopm_registers, new()
        where RegisterType_OPS : ymfm_engine_base_operators, new()
    {
        // "quiet" value, used to optimize when we can skip doing working
        //static constexpr u32 ENV_QUIET = 0x200;


        // internal state
        u32 m_choffs;                    // channel offset in registers
        u32 m_opoffs;                    // operator offset in registers
        u32 m_phase;                     // current phase value (10.10 format)
        u16 m_env_attenuation;           // computed envelope attenuation (4.6 format)
        ymfm_envelope_state m_env_state; // current envelope state
        u8 m_ssg_inverted;               // non-zero if the output should be inverted (bit 0)
        u8 m_key_state;                  // current key state: on or off (bit 0)
        u8 m_keyon_live;                 // live key on state (bit 0 = direct, bit 1 = rhythm, bit 2 = CSM)
        //ymfm_opdata_cache m_cache;       // cached values for performance
        RegisterType m_regs;            // direct reference to registers
        ymfm_engine_base<RegisterType, RegisterType_OPS> m_owner; // reference to the owning engine


        // constructor
        public ymfm_operator(ymfm_engine_base<RegisterType, RegisterType_OPS> owner, u32 opoffs)
        {
            m_choffs = 0;
            m_opoffs = opoffs;
            m_phase = 0;
            m_env_attenuation = 0x3ff;
            m_env_state = ymfm_envelope_state.YMFM_ENV_RELEASE;
            m_ssg_inverted = 0;  //false;
            m_key_state = 0;
            m_keyon_live = 0;
            m_regs = owner.regs();
            m_owner = owner;
        }


        // register for save states
        //void save(device_t &device, u32 index);

        // reset the operator state
        //void reset();

        // set the current channel
        public void set_choffs(u32 choffs) { m_choffs = choffs; }

        // prepare prior to clocking
        //bool prepare();

        // master clocking function
        //void clock(u32 env_counter, s32 lfo_raw_pm);

        // return the current phase value
        //u32 phase() const { return m_phase >> 10; }

        // compute operator volume
        //s32 compute_volume(u32 phase, u32 am_offset) const;

        // compute volume for the OPM noise channel
        //s32 compute_noise_volume(u32 am_offset) const;

        // key state control
        //-------------------------------------------------
        //  keyonoff - signal a key on/off event
        //-------------------------------------------------
        public void keyonoff(u32 on, ymfm_keyon_type type)
        {
            m_keyon_live = (u8)((m_keyon_live & ~(1U << (int)type)) | (g.BIT(on, 0) << (int)type));
        }


        // return a reference to our registers
        //RegisterType &regs() { return m_regs; }


        // start the attack phase
        //void start_attack();

        // start the release phase
        //void start_release();

        // clock phases
        //void clock_keystate(u32 keystate);
        //void clock_ssg_eg_state();
        //void clock_envelope(u32 env_counter);
        //void clock_phase(s32 lfo_raw_pm);

        // return effective attenuation of the envelope
        //u32 envelope_attenuation(u32 am_offset) const;
    }


    // ======================> ymfm_channel
    //template<class RegisterType>
    // ymfm_channel represents an FM channel which combines the output of 2 or 4
    // operators into a final result
    class ymfm_channel<RegisterType, RegisterType_OPS> : global_object
        where RegisterType : ymopm_registers, new()
        where RegisterType_OPS : ymfm_engine_base_operators, new()
    {
        // internal state
        u32 m_choffs;                         // channel offset in registers
        s16 [] m_feedback = new s16[2];                    // feedback memory for operator 1
        s16 m_feedback_in;            // next input value for op 1 feedback (set in output)  //mutable s16 m_feedback_in;            // next input value for op 1 feedback (set in output)
        ymfm_operator<RegisterType, RegisterType_OPS> [] m_op = new ymfm_operator<RegisterType, RegisterType_OPS>[4]; // up to 4 operators  //ymfm_operator<RegisterType> *m_op[4]; // up to 4 operators
        RegisterType m_regs;                 // direct reference to registers
        ymfm_engine_base<RegisterType, RegisterType_OPS> m_owner; // reference to the owning engine


        // constructor
        public ymfm_channel(ymfm_engine_base<RegisterType, RegisterType_OPS> owner, u32 choffs)
        {
            m_choffs = choffs;
            m_feedback = new s16[2] { 0, 0 };
            m_feedback_in = 0;
            m_op = new ymfm_operator<RegisterType, RegisterType_OPS>[4] { null, null, null, null };
            m_regs = owner.regs();
            m_owner = owner;
        }


        // register for save states
        //void save(device_t &device, u32 index);

        // reset the channel state
        //void reset();


        // assign operators
        public void assign(int index, ymfm_operator<RegisterType, RegisterType_OPS> op)
        {
            assert(index < std.size(m_op));
            m_op[index] = op;
            if (op != null)
                op.set_choffs(m_choffs);
        }


        // signal key on/off to our operators
        //-------------------------------------------------
        //  keyonoff - signal key on/off to our operators
        //-------------------------------------------------
        public void keyonoff(u32 states, ymfm_keyon_type type)
        {
            for (int opnum = 0; opnum < std.size(m_op); opnum++)
                if (m_op[opnum] != null)
                    m_op[opnum].keyonoff(g.BIT(states, opnum), type);
        }


        // prepare prior to clocking
        //bool prepare();

        // master clocking function
        //void clock(u32 env_counter, s32 lfo_raw_pm);

        // specific 2-operator and 4-operator output handlers
        //void output_2op(s32 outputs[RegisterType::OUTPUTS], u32 rshift, s32 clipmax) const;
        //void output_4op(s32 outputs[RegisterType::OUTPUTS], u32 rshift, s32 clipmax) const;

        // compute the special OPL rhythm channel outputs
        //void output_rhythm_ch6(s32 outputs[RegisterType::OUTPUTS], u32 rshift, s32 clipmax) const;
        //void output_rhythm_ch7(u32 phase_select, s32 outputs[RegisterType::OUTPUTS], u32 rshift, s32 clipmax) const;
        //void output_rhythm_ch8(u32 phase_select, s32 outputs[RegisterType::OUTPUTS], u32 rshift, s32 clipmax) const;

        // are we a 4-operator channel or a 2-operator one?
        //bool is4op() const
        //{
        //    if (RegisterType::DYNAMIC_OPS)
        //        return (m_op[2] != nullptr);
        //    return (RegisterType::OPERATORS / RegisterType::CHANNELS == 4);
        //}

        // return a reference to our registers
        //RegisterType &regs() { return m_regs; }


        // helper to add values to the outputs based on channel enables
        //void add_to_output(u32 choffs, s32 *outputs, s32 value) const
        //{
        //    if (RegisterType::OUTPUTS == 1 || m_regs.ch_output_0(choffs))
        //        outputs[0] += value;
        //    if (RegisterType::OUTPUTS >= 2 && m_regs.ch_output_1(choffs))
        //        outputs[1] += value;
        //    if (RegisterType::OUTPUTS >= 3 && m_regs.ch_output_2(choffs))
        //        outputs[2] += value;
        //    if (RegisterType::OUTPUTS >= 4 && m_regs.ch_output_3(choffs))
        //        outputs[3] += value;
        //}
    }


    interface ymfm_engine_base_operators 
    {
        u32 YMFM_RHYTHM_CHANNEL { get; }

        u32 CHANNELS { get; }
        u32 ALL_CHANNELS { get; }
        u32 OPERATORS { get; }
        u32 REG_MODE { get; }
        u8 STATUS_TIMERA { get; }
        u8 STATUS_TIMERB { get; }
        u8 STATUS_BUSY { get; }
        u8 STATUS_IRQ { get; }
        u32 DEFAULT_PRESCALE { get; }

        u32 channel_offset(u32 chnum);
        u32 operator_offset(u32 opnum);
    }

    class ymfm_engine_base_operators_ymopm_registers : ymfm_engine_base_operators
    {
        public u32 YMFM_RHYTHM_CHANNEL { get { return ymopm_registers.YMFM_RHYTHM_CHANNEL; } }

        public u32 CHANNELS { get { return ymopm_registers.CHANNELS; } }
        public u32 ALL_CHANNELS { get { return ymopm_registers.ALL_CHANNELS; } }
        public u32 OPERATORS { get { return ymopm_registers.OPERATORS; } }
        public u32 REG_MODE { get { return ymopm_registers.REG_MODE; } }
        public u8 STATUS_TIMERA { get { return ymopm_registers.STATUS_TIMERA; } }
        public u8 STATUS_TIMERB { get { return ymopm_registers.STATUS_TIMERB; } }
        public u8 STATUS_BUSY { get { return ymopm_registers.STATUS_BUSY; } }
        public u8 STATUS_IRQ { get { return ymopm_registers.STATUS_IRQ; } }
        public u32 DEFAULT_PRESCALE { get { return ymopm_registers.DEFAULT_PRESCALE; } }

        public u32 channel_offset(u32 chnum) { return ymopm_registers.channel_offset(chnum); }
        public u32 operator_offset(u32 opnum) { return ymopm_registers.operator_offset(opnum); }
    }


    // ======================> ymfm_engine_base
    // ymfm_engine_base represents a set of operators and channels which together
    // form a Yamaha FM core; chips that implement other engines (ADPCM, wavetable,
    // etc) take this output and combine it with the others externally
    //template<class RegisterType>
    class ymfm_engine_base<RegisterType, RegisterType_OPS> : global_object
        where RegisterType : ymopm_registers, new()
        where RegisterType_OPS : ymfm_engine_base_operators, new()
    {
        static readonly RegisterType_OPS ops = new RegisterType_OPS();


        // expose some constants from the registers
        //static constexpr u32 OUTPUTS = RegisterType::OUTPUTS;
        static readonly u32 CHANNELS = ops.CHANNELS;  //static constexpr u32 CHANNELS = RegisterType::CHANNELS;
        static readonly u32 ALL_CHANNELS = ops.ALL_CHANNELS;  //static constexpr u32 ALL_CHANNELS = RegisterType::ALL_CHANNELS;
        static readonly u32 OPERATORS = ops.OPERATORS;  //static constexpr u32 OPERATORS = RegisterType::OPERATORS;

        // also expose status flags for consumers that inject additional bits
        static readonly u8 STATUS_TIMERA = ops.STATUS_TIMERA;  //static constexpr u8 STATUS_TIMERA = RegisterType::STATUS_TIMERA;
        static readonly u8 STATUS_TIMERB = ops.STATUS_TIMERB;  //static constexpr u8 STATUS_TIMERB = RegisterType::STATUS_TIMERB;
        static readonly u8 STATUS_BUSY = ops.STATUS_BUSY;  //static constexpr u8 STATUS_BUSY = RegisterType::STATUS_BUSY;
        static readonly u8 STATUS_IRQ = ops.STATUS_IRQ;  //static constexpr u8 STATUS_IRQ = RegisterType::STATUS_IRQ;


        // internal state
        device_t m_device;              // reference to the owning device
        u32 m_env_counter;               // envelope counter; low 2 bits are sub-counter
        u8 m_status;                     // current status register
        u8 m_clock_prescale;             // prescale factor (2/3/6)
        u8 m_irq_mask;                   // mask of which bits signal IRQs
        u8 m_irq_state;                  // current IRQ state
        u32 m_active_channels;           // mask of active channels (computed by prepare)
        u32 m_modified_channels;         // mask of channels that have been modified
        u32 m_prepare_count;             // counter to do periodic prepare sweeps
        attotime m_busy_end;             // end of the busy time
        emu_timer [] m_timer = new emu_timer[2];           // our two timers
        devcb_write_line m_irq_handler;  // IRQ callback
        RegisterType m_regs;             // register accessor
        ymfm_channel<RegisterType, RegisterType_OPS> [] m_channel = new ymfm_channel<RegisterType, RegisterType_OPS>[CHANNELS]; // channel pointers  //std::unique_ptr<ymfm_channel<RegisterType>> m_channel[CHANNELS]; // channel pointers
        ymfm_operator<RegisterType, RegisterType_OPS> [] m_operator = new ymfm_operator<RegisterType, RegisterType_OPS>[OPERATORS]; // operator pointers  //std::unique_ptr<ymfm_operator<RegisterType>> m_operator[OPERATORS]; // operator pointers


        // constructor
        public ymfm_engine_base(device_t device)
        {
            m_device = device;
            m_env_counter = 0;
            m_status = 0;
            m_clock_prescale = (u8)ops.DEFAULT_PRESCALE;  //m_clock_prescale(RegisterType::DEFAULT_PRESCALE),
            m_irq_mask = (u8)(STATUS_TIMERA | STATUS_TIMERB);
            m_irq_state = 0;
            m_active_channels = ALL_CHANNELS;
            m_modified_channels = ALL_CHANNELS;
            m_prepare_count = 0;
            m_busy_end = attotime.zero;
            m_timer = new emu_timer[2] { null, null };
            m_irq_handler = new devcb_write_line(device);


            m_regs = new RegisterType();


            // create the channels
            m_channel = new ymfm_channel<RegisterType, RegisterType_OPS>[CHANNELS];
            for (int chnum = 0; chnum < CHANNELS; chnum++)
                m_channel[chnum] = new ymfm_channel<RegisterType, RegisterType_OPS>(this, ops.channel_offset((u32)chnum));  //m_channel[chnum] = std::make_unique<ymfm_channel<RegisterType>>(*this, RegisterType::channel_offset(chnum));

            // create the operators
            for (int opnum = 0; opnum < OPERATORS; opnum++)
                m_operator[opnum] = new ymfm_operator<RegisterType, RegisterType_OPS>(this, ops.operator_offset((u32)opnum));  //m_operator[opnum] = std::make_unique<ymfm_operator<RegisterType>>(*this, RegisterType::operator_offset(opnum));

            // do the initial operator assignment
            assign_operators();
        }


        // configuration helpers
        //auto irq_handler() { return m_irq_handler.bind(); }

        // register for save states
        //void save(device_t &device);

        // reset the overall state
        //void reset();

        // master clocking function
        //u32 clock(u32 chanmask);

        // compute sum of channel outputs
        //void output(s32 outputs[RegisterType::OUTPUTS], u32 rshift, s32 clipmax, u32 chanmask) const;


        // write to the OPN registers
        //-------------------------------------------------
        //  write - handle writes to the OPN registers
        //-------------------------------------------------
        //template<class RegisterType>
        public void write(u16 regnum, u8 data)
        {
            // special case: writes to the mode register can impact IRQs;
            // schedule these writes to ensure ordering with timers
            if (regnum == ops.REG_MODE)  //if (regnum == RegisterType::REG_MODE)
            {
                m_device.machine().scheduler().synchronize(synced_mode_w, data);  //m_device.machine().scheduler().synchronize(timer_expired_delegate(FUNC(ymfm_engine_base<RegisterType>::synced_mode_w), this), data);
                return;
            }

            // for now just mark all channels as modified
            m_modified_channels = ALL_CHANNELS;

            // most writes are passive, consumed only when needed
            u32 keyon_channel;
            u32 keyon_opmask;
            if (m_regs.write(regnum, data, out keyon_channel, out keyon_opmask))
            {
                // handle writes to the keyon register(s)
                if (keyon_channel < CHANNELS)
                {
                    // normal channel on/off
                    m_channel[keyon_channel].keyonoff(keyon_opmask, ymfm_keyon_type.YMFM_KEYON_NORMAL);
                }
                else if (CHANNELS >= 9 && keyon_channel == ops.YMFM_RHYTHM_CHANNEL)
                {
                    // special case for the OPL rhythm channels
                    m_channel[6].keyonoff(g.BIT(keyon_opmask, 4) != 0 ? 3U : 0, ymfm_keyon_type.YMFM_KEYON_RHYTHM);
                    m_channel[7].keyonoff(g.BIT(keyon_opmask, 0) | (g.BIT(keyon_opmask, 3) << 1), ymfm_keyon_type.YMFM_KEYON_RHYTHM);
                    m_channel[8].keyonoff(g.BIT(keyon_opmask, 2) | (g.BIT(keyon_opmask, 1) << 1), ymfm_keyon_type.YMFM_KEYON_RHYTHM);
                }
            }
        }


        // return the current status
        //-------------------------------------------------
        //  status - return the current state of the
        //  status flags
        //-------------------------------------------------
        //template<class RegisterType>
        public u8 status()
        {
            u8 result = (u8)(m_status & (u32)(u8)~STATUS_BUSY & (u32)~m_regs.status_mask());
            if (m_device.machine().time() < m_busy_end)
                result |= STATUS_BUSY;
            return result;
        }


        // set/reset bits in the status register, updating the IRQ status
        u8 set_reset_status(u8 set, u8 reset)
        {
            m_status = (u8)((m_status | set) & ~reset);
            schedule_check_interrupts();
            return m_status;
        }


        // set the IRQ mask
        //void set_irq_mask(u8 mask) { m_irq_mask = mask; schedule_check_interrupts(); }


        // helper to compute the busy duration
        public attotime compute_busy_duration(u32 cycles = 32)
        {
            return attotime.from_hz(m_device.clock()) * (cycles * m_clock_prescale);
        }

        // set the time when the busy flag in the status register should be cleared
        public void set_busy_end(attotime end) { m_busy_end = end; }

        // return the current clock prescale
        //u32 clock_prescale() const { return m_clock_prescale; }

        // set prescale factor (2/3/6)
        //void set_clock_prescale(u32 prescale) { m_clock_prescale = prescale; }

        // compute sample rate
        //u32 sample_rate(u32 baseclock) const { return baseclock / (m_clock_prescale * OPERATORS); }

        // reset the LFO state
        public void reset_lfo() { m_regs.reset_lfo(); }

        // return the owning device
        //device_t &device() const { return m_device; }

        // return a reference to our registers
        public RegisterType regs() { return m_regs; }


        // assign the current set of operators to channels
        //-------------------------------------------------
        //  assign_operators - get the current mapping of
        //  operators to channels and assign them all
        //-------------------------------------------------
        void assign_operators()
        {
            ymopm_registers.operator_mapping map;  //typename RegisterType::operator_mapping map;
            m_regs.operator_map(out map);

            for (int chnum = 0; chnum < CHANNELS; chnum++)
            {
                for (int index = 0; index < 4; index++)
                {
                    u32 opnum = g.BIT(map.chan[chnum], 8 * index, 8);
                    m_channel[chnum].assign(index, (opnum == 0xff) ? null : m_operator[opnum]);
                }
            }
        }


        // update the state of the given timer
        //-------------------------------------------------
        //  update_timer - update the state of the given
        //  timer
        //-------------------------------------------------
        //template<class RegisterType>
        void update_timer(u32 tnum, u32 enable)
        {
            // if the timer is live, but not currently enabled, set the timer
            if (enable != 0 && !m_timer[tnum].enable())
            {
                // each timer clock is n operators * prescale factor (2/3/6)
                u32 clockscale = OPERATORS * m_clock_prescale;

                // period comes from the registers, and is different for each
                u32 period = (tnum == 0) ? (1024U - m_regs.timer_a_value()) : 16U * (256U - m_regs.timer_b_value());

                // reset it
                m_timer[tnum].adjust(attotime.from_hz(m_device.clock()) * (period * clockscale), (int)tnum);
            }

            // if the timer is not live, ensure it is not enabled
            else if (enable == 0)
                m_timer[tnum].enable(false);
        }


        // timer callback
        //TIMER_CALLBACK_MEMBER(timer_handler);


        // schedule an interrupt check
        //-------------------------------------------------
        //  schedule_check_interrupts - schedule an
        //  interrupt check via timer
        //-------------------------------------------------
        //template<class RegisterType>
        void schedule_check_interrupts()
        {
            // if we're currently executing a CPU, schedule the interrupt check;
            // otherwise, do it directly
            var scheduler = m_device.machine().scheduler();
            if (scheduler.currently_executing() != null)
                scheduler.synchronize(check_interrupts, 0);  //scheduler.synchronize(timer_expired_delegate(FUNC(ymfm_engine_base<RegisterType>::check_interrupts), this), 0);
            else
                check_interrupts(null, 0);
        }


        // check interrupts
        //TIMER_CALLBACK_MEMBER(check_interrupts);
        //-------------------------------------------------
        //  check_interrupts - check the interrupt sources
        //  for interrupts
        //-------------------------------------------------
        //template<class RegisterType>
        void check_interrupts(object ptr, int param)
        {
            // update the state
            u8 old_state = m_irq_state;
            m_irq_state = ((m_status & m_irq_mask & ~m_regs.status_mask()) != 0) ? (u8)1 : (u8)0;

            // set the IRQ status bit
            if (m_irq_state != 0)
                m_status |= STATUS_IRQ;
            else
                m_status &= (u8)~STATUS_IRQ;

            // if changed, signal the new state
            if (old_state != m_irq_state && !m_irq_handler.isnull())
                m_irq_handler.op(m_irq_state != 0 ? g.ASSERT_LINE : g.CLEAR_LINE);
        }


        // handle a mode register write
        //TIMER_CALLBACK_MEMBER(synced_mode_w);
        //-------------------------------------------------
        //  synced_mode_w - handle a mode register write
        //  via timer callback
        //-------------------------------------------------
        //template<class RegisterType>
        void synced_mode_w(object ptr, int param)
        {
            // mark all channels as modified
            m_modified_channels = ALL_CHANNELS;

            // actually write the mode register now
            u32 dummy1, dummy2;
            m_regs.write((u16)ops.REG_MODE, (u8)param, out dummy1, out dummy2);

            // reset IRQ status -- when written, all other bits are ignored
            // QUESTION: should this maybe just reset the IRQ bit and not all the bits?
            //   That is, check_interrupts would only set, this would only clear?
            if (m_regs.irq_reset() != 0)
                set_reset_status(0, 0x78);
            else
            {
                // reset timer status
                u8 reset_mask = 0;
                if (m_regs.reset_timer_b() != 0)
                    reset_mask |= ops.STATUS_TIMERB;  //reset_mask |= RegisterType::STATUS_TIMERB;
                if (m_regs.reset_timer_a() != 0)
                    reset_mask |= ops.STATUS_TIMERA;  //reset_mask |= RegisterType::STATUS_TIMERA;
                set_reset_status(0, reset_mask);

                // load timers
                update_timer(1, m_regs.load_timer_b());
                update_timer(0, m_regs.load_timer_a());
            }
        }
    }


    // ======================> template instantiations
    //extern template class ymfm_engine_base<ymopm_registers>;
    //extern template class ymfm_engine_base<ymopn_registers>;
    //extern template class ymfm_engine_base<ymopna_registers>;
    //extern template class ymfm_engine_base<ymopl_registers>;
    //extern template class ymfm_engine_base<ymopl2_registers>;
    //extern template class ymfm_engine_base<ymopl3_registers>;

    //using ymopm_engine = ymfm_engine_base<ymopm_registers>;
    //using ymopn_engine = ymfm_engine_base<ymopn_registers>;
    //using ymopna_engine = ymfm_engine_base<ymopna_registers>;
    //using ymopl_engine = ymfm_engine_base<ymopl_registers>;
    //using ymopl2_engine = ymfm_engine_base<ymopl2_registers>;
    //using ymopl3_engine = ymfm_engine_base<ymopl3_registers>;
    //using ymopl4_engine = ymfm_engine_base<ymopl4_registers>;


    // ======================> ymopll_engine
    //class ymopll_engine : public ymfm_engine_base<ymopll_registers>
}
