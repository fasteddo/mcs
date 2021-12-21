// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int8_t = System.SByte;
using int16_t = System.Int16;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using ym2151_fm_engine = mame.ymfm.fm_engine_base<mame.ymfm.opm_registers, mame.ymfm.fm_engine_base_operators_opm_registers>;  //using fm_engine = fm_engine_base<opm_registers>;
using ym2151_output_data = mame.ymfm.fm_engine_base<mame.ymfm.opm_registers, mame.ymfm.fm_engine_base_operators_opm_registers>.output_data;  //using output_data = fm_engine::output_data;


namespace mame.ymfm
{
    //*********************************************************
    //  REGISTER CLASSES
    //*********************************************************

    public class fm_engine_base_operators_opm_registers : fm_engine_base_operators
    {
        public uint32_t OUTPUTS { get { return opm_registers.OUTPUTS; } }
        public uint32_t CHANNELS { get { return opm_registers.CHANNELS; } }
        public uint32_t ALL_CHANNELS { get { return opm_registers.ALL_CHANNELS; } }
        public uint32_t OPERATORS { get { return opm_registers.OPERATORS; } }
        public uint32_t DEFAULT_PRESCALE { get { return opm_registers.DEFAULT_PRESCALE; } }
        public uint8_t STATUS_TIMERA { get { return opm_registers.STATUS_TIMERA; } }
        public uint8_t STATUS_TIMERB { get { return opm_registers.STATUS_TIMERB; } }

        public uint32_t channel_offset(uint32_t chnum) { return opm_registers.channel_offset(chnum); }
        public uint32_t operator_offset(uint32_t opnum) { return opm_registers.operator_offset(opnum); }
    }


    // ======================> opm_registers
    public class opm_registers : fm_registers_base
    {
        // LFO waveforms are 256 entries long
        const uint32_t LFO_WAVEFORM_LENGTH = 256;


        // constants
        public const uint32_t OUTPUTS = 2;
        public const uint32_t CHANNELS = 8;
        public const uint32_t ALL_CHANNELS = (1U << (int)CHANNELS) - 1;
        public const uint32_t OPERATORS = CHANNELS * 4;
        public const uint32_t WAVEFORMS = 1;
        //static constexpr uint32_t REGISTERS = 0x100;
        public const uint32_t DEFAULT_PRESCALE = 2;
        //static constexpr uint32_t EG_CLOCK_DIVIDER = 3;
        //static constexpr uint32_t CSM_TRIGGER_MASK = ALL_CHANNELS;
        //static constexpr uint32_t REG_MODE = 0x14;
        public const uint8_t STATUS_TIMERA = 0x01;
        public const uint8_t STATUS_TIMERB = 0x02;
        //static constexpr uint8_t STATUS_BUSY = 0x80;
        //static constexpr uint8_t STATUS_IRQ = 0;


        // internal state
        uint32_t m_lfo_counter;               // LFO counter
        uint32_t m_noise_lfsr;                // noise LFSR state
        uint8_t m_noise_counter;              // noise counter
        uint8_t m_noise_state;                // latched noise state
        uint8_t m_noise_lfo;                  // latched LFO noise value
        uint8_t m_lfo_am;                     // current LFO AM value
        //uint8_t m_regdata[REGISTERS];         // register data
        int16_t [,] m_lfo_waveform = new int16_t [4, LFO_WAVEFORM_LENGTH]; // LFO waveforms; AM in low 8, PM in upper 8
        uint16_t [,] m_waveform = new uint16_t [WAVEFORMS, WAVEFORM_LENGTH]; // waveforms


        // constructor
        public opm_registers()
        {
            m_lfo_counter = 0;
            m_noise_lfsr = 1;
            m_noise_counter = 0;
            m_noise_state = 0;
            m_noise_lfo = 0;
            m_lfo_am = 0;


            // create the waveforms
            for (uint32_t index = 0; index < WAVEFORM_LENGTH; index++)
                m_waveform[0, index] = (uint16_t)(ymfm_fm_global.abs_sin_attenuation(index) | (ymfm_global.bitfield(index, 9) << 15));

            // create the LFO waveforms; AM in the low 8 bits, PM in the upper 8
            // waveforms are adjusted to match the pictures in the application manual
            for (uint32_t index = 0; index < LFO_WAVEFORM_LENGTH; index++)
            {
                // waveform 0 is a sawtooth
                uint8_t am = (uint8_t)(index ^ 0xff);
                int8_t pm = (int8_t)index;
                m_lfo_waveform[0, index] = (int16_t)(am | (pm << 8));

                // waveform 1 is a square wave
                am = ymfm_global.bitfield(index, 7) != 0 ? (uint8_t)0 : (uint8_t)0xff;
                pm = (int8_t)(am ^ 0x80);
                m_lfo_waveform[1, index] = (int16_t)(am | (pm << 8));

                // waveform 2 is a triangle wave
                am = ymfm_global.bitfield(index, 7) != 0 ? (uint8_t)(index << 1) : (uint8_t)((index ^ 0xff) << 1);
                pm = (int8_t)(ymfm_global.bitfield(index, 6) != 0 ? am : ~am);
                m_lfo_waveform[2, index] = (int16_t)(am | (pm << 8));

                // waveform 3 is noise; it is filled in dynamically
            }
        }


        // reset to initial state
        //void reset();

        // save/restore
        //void save_restore(ymfm_saved_state &state);

        // map channel number to register offset
        public static uint32_t channel_offset(uint32_t chnum)
        {
            g.assert(chnum < CHANNELS);
            return chnum;
        }

        // map operator number to register offset
        public static uint32_t operator_offset(uint32_t opnum)
        {
            g.assert(opnum < OPERATORS);
            return opnum;
        }


        // return an array of operator indices for each channel
        public class operator_mapping { public uint32_t [] chan = new uint32_t[CHANNELS]; }  //struct operator_mapping { uint32_t chan[CHANNELS]; };

        //-------------------------------------------------
        //  operator_map - return an array of operator
        //  indices for each channel; for OPM this is fixed
        //-------------------------------------------------

        // Note that the channel index order is 0,2,1,3, so we bitswap the index.
        //
        // This is because the order in the map is:
        //    carrier 1, carrier 2, modulator 1, modulator 2
        //
        // But when wiring up the connections, the more natural order is:
        //    carrier 1, modulator 1, carrier 2, modulator 2
        static readonly operator_mapping s_fixed_map = new operator_mapping()
        {
            chan = new uint32_t []
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
        //bool write(uint16_t index, uint8_t data, uint32_t &chan, uint32_t &opmask);

        // clock the noise and LFO, if present, returning LFO PM value
        //int32_t clock_noise_and_lfo();

        // return the AM offset from LFO for the given channel
        //uint32_t lfo_am_offset(uint32_t choffs) const;

        // return the current noise state, gated by the noise clock
        //uint32_t noise_state() const { return m_noise_state; }

        // caching helpers
        //void cache_operator_data(uint32_t choffs, uint32_t opoffs, opdata_cache &cache);

        // compute the phase step, given a PM value
        //uint32_t compute_phase_step(uint32_t choffs, uint32_t opoffs, opdata_cache const &cache, int32_t lfo_raw_pm);

        // log a key-on event
        //std::string log_keyon(uint32_t choffs, uint32_t opoffs);

        // system-wide registers
        //uint32_t test() const                            { return byte(0x01, 0, 8); }
        //uint32_t lfo_reset() const                       { return byte(0x01, 1, 1); }
        //uint32_t noise_frequency() const                 { return byte(0x0f, 0, 5); }
        //uint32_t noise_enable() const                    { return byte(0x0f, 7, 1); }
        //uint32_t timer_a_value() const                   { return word(0x10, 0, 8, 0x11, 0, 2); }
        //uint32_t timer_b_value() const                   { return byte(0x12, 0, 8); }
        //uint32_t csm() const                             { return byte(0x14, 7, 1); }
        //uint32_t reset_timer_b() const                   { return byte(0x14, 5, 1); }
        //uint32_t reset_timer_a() const                   { return byte(0x14, 4, 1); }
        //uint32_t enable_timer_b() const                  { return byte(0x14, 3, 1); }
        //uint32_t enable_timer_a() const                  { return byte(0x14, 2, 1); }
        //uint32_t load_timer_b() const                    { return byte(0x14, 1, 1); }
        //uint32_t load_timer_a() const                    { return byte(0x14, 0, 1); }
        //uint32_t lfo_rate() const                        { return byte(0x18, 0, 8); }
        //uint32_t lfo_am_depth() const                    { return byte(0x19, 0, 7); }
        //uint32_t lfo_pm_depth() const                    { return byte(0x1a, 0, 7); }
        //uint32_t output_bits() const                     { return byte(0x1b, 6, 2); }
        //uint32_t lfo_waveform() const                    { return byte(0x1b, 0, 2); }

        // per-channel registers
        //uint32_t ch_output_any(uint32_t choffs) const    { return byte(0x20, 6, 2, choffs); }
        //uint32_t ch_output_0(uint32_t choffs) const      { return byte(0x20, 6, 1, choffs); }
        //uint32_t ch_output_1(uint32_t choffs) const      { return byte(0x20, 7, 1, choffs); }
        //uint32_t ch_output_2(uint32_t choffs) const      { return 0; }
        //uint32_t ch_output_3(uint32_t choffs) const      { return 0; }
        //uint32_t ch_feedback(uint32_t choffs) const      { return byte(0x20, 3, 3, choffs); }
        //uint32_t ch_algorithm(uint32_t choffs) const     { return byte(0x20, 0, 3, choffs); }
        //uint32_t ch_block_freq(uint32_t choffs) const    { return word(0x28, 0, 7, 0x30, 2, 6, choffs); }
        //uint32_t ch_lfo_pm_sens(uint32_t choffs) const   { return byte(0x38, 4, 3, choffs); }
        //uint32_t ch_lfo_am_sens(uint32_t choffs) const   { return byte(0x38, 0, 2, choffs); }

        // per-operator registers
        //uint32_t op_detune(uint32_t opoffs) const        { return byte(0x40, 4, 3, opoffs); }
        //uint32_t op_multiple(uint32_t opoffs) const      { return byte(0x40, 0, 4, opoffs); }
        //uint32_t op_total_level(uint32_t opoffs) const   { return byte(0x60, 0, 7, opoffs); }
        //uint32_t op_ksr(uint32_t opoffs) const           { return byte(0x80, 6, 2, opoffs); }
        //uint32_t op_attack_rate(uint32_t opoffs) const   { return byte(0x80, 0, 5, opoffs); }
        //uint32_t op_lfo_am_enable(uint32_t opoffs) const { return byte(0xa0, 7, 1, opoffs); }
        //uint32_t op_decay_rate(uint32_t opoffs) const    { return byte(0xa0, 0, 5, opoffs); }
        //uint32_t op_detune2(uint32_t opoffs) const       { return byte(0xc0, 6, 2, opoffs); }
        //uint32_t op_sustain_rate(uint32_t opoffs) const  { return byte(0xc0, 0, 5, opoffs); }
        //uint32_t op_sustain_level(uint32_t opoffs) const { return byte(0xe0, 4, 4, opoffs); }
        //uint32_t op_release_rate(uint32_t opoffs) const  { return byte(0xe0, 0, 4, opoffs); }


        // return a bitfield extracted from a byte
        //uint32_t byte(uint32_t offset, uint32_t start, uint32_t count, uint32_t extra_offset = 0) const
        //{
        //    return bitfield(m_regdata[offset + extra_offset], start, count);
        //}

        // return a bitfield extracted from a pair of bytes, MSBs listed first
        //uint32_t word(uint32_t offset1, uint32_t start1, uint32_t count1, uint32_t offset2, uint32_t start2, uint32_t count2, uint32_t extra_offset = 0) const
        //{
        //    return (byte(offset1, start1, count1, extra_offset) << count2) | byte(offset2, start2, count2, extra_offset);
        //}
    }


    //*********************************************************
    //  OPM IMPLEMENTATION CLASSES
    //*********************************************************
    // ======================> ym2151
    public class ym2151 : ChipClass_operators<opm_registers, fm_engine_base_operators_opm_registers>
    {
        //using fm_engine = fm_engine_base<opm_registers>;
        //using output_data = fm_engine::output_data;


        public uint32_t OUTPUTS { get { return ym2151_fm_engine.OUTPUTS; } }  //static constexpr uint32_t OUTPUTS = fm_engine::OUTPUTS;


        // variants
        enum opm_variant
        {
            VARIANT_YM2151,
            VARIANT_YM2164
        }


        // internal state
        opm_variant m_variant;           // chip variant
        uint8_t m_address;               // address register
        ym2151_fm_engine m_fm;                  // core FM engine


        // constructor
        public ym2151(ymfm_interface intf) : this(intf, opm_variant.VARIANT_YM2151) { }


        // internal constructor
        ym2151(ymfm_interface intf, opm_variant variant)
        {
            m_variant = variant;
            m_address = 0;
            m_fm = new ym2151_fm_engine(intf);
        }


        // reset
        public void reset() { throw new emu_unimplemented(); }

        // save/restore
        public void save_restore(ymfm_saved_state state) { throw new emu_unimplemented(); }

        // pass-through helpers
        public uint32_t sample_rate(uint32_t input_clock) { return m_fm.sample_rate(input_clock); }
        //void invalidate_caches() { m_fm.invalidate_caches(); }

        // read access
        public uint8_t read_status() { throw new emu_unimplemented(); }
        public uint8_t read(uint32_t offset) { throw new emu_unimplemented(); }

        // write access
        public void write_address(uint8_t data) { throw new emu_unimplemented(); }
        public void write_data(uint8_t data) { throw new emu_unimplemented(); }
        public void write(uint32_t offset, uint8_t data) { throw new emu_unimplemented(); }

        // generate one sample of sound
        public void generate(ym2151_output_data [] output, uint32_t numsamples = 1) { throw new emu_unimplemented(); }
    }



    //*********************************************************
    //  OPP IMPLEMENTATION CLASSES
    //*********************************************************

    // ======================> ym2164
    //class ym2164 : public ym2151
}
