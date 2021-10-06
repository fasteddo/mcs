// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int32_t = System.Int32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame.ymfm
{
    //*********************************************************
    //  DEBUGGING
    //*********************************************************
    //class debug


    //*********************************************************
    //  GLOBAL HELPERS
    //*********************************************************
    static class ymfm_global
    {
        //-------------------------------------------------
        //  bitfield - extract a bitfield from the given
        //  value, starting at bit 'start' for a length of
        //  'length' bits
        //-------------------------------------------------
        public static uint32_t bitfield(uint32_t value, int start, int length = 1)
        {
            return (value >> start) & ((1U << length) - 1);
        }


        //-------------------------------------------------
        //  clamp - clamp between the minimum and maximum
        //  values provided
        //-------------------------------------------------
        //inline int32_t clamp(int32_t value, int32_t minval, int32_t maxval)
        //{
        //    if (value < minval)
        //        return minval;
        //    if (value > maxval)
        //        return maxval;
        //    return value;
        //}


        //-------------------------------------------------
        //  array_size - return the size of an array
        //-------------------------------------------------
        //template<typename ArrayType, int ArraySize>
        //constexpr uint32_t array_size(ArrayType (&array)[ArraySize])
        //{
        //    return ArraySize;
        //}


        //-------------------------------------------------
        //  count_leading_zeros - return the number of
        //  leading zeros in a 32-bit value; CPU-optimized
        //  versions for various architectures are included
        //  below
        //-------------------------------------------------
        //inline uint8_t count_leading_zeros(uint32_t value)
        //{
        //    if (value == 0)
        //        return 32;
        //    uint8_t count;
        //    for (count = 0; int32_t(value) >= 0; count++)
        //        value <<= 1;
        //    return count;
        //}


        //-------------------------------------------------
        //  encode_fp - given a 32-bit signed input value
        //  convert it to a signed 3.10 floating-point
        //  value
        //-------------------------------------------------
        //inline int16_t encode_fp(int32_t value)
        //{
        //    // handle overflows first
        //    if (value < -32768)
        //        return (7 << 10) | 0x000;
        //    if (value > 32767)
        //        return (7 << 10) | 0x3ff;
        //
        //    // we need to count the number of leading sign bits after the sign
        //    // we can use count_leading_zeros if we invert negative values
        //    int32_t scanvalue = value ^ (int32_t(value) >> 31);
        //
        //    // exponent is related to the number of leading bits starting from bit 14
        //    int exponent = 7 - count_leading_zeros(scanvalue << 17);
        //
        //    // smallest exponent value allowed is 1
        //    exponent = std::max(exponent, 1);
        //
        //    // mantissa
        //    int32_t mantissa = value >> (exponent - 1);
        //
        //    // assemble into final form, inverting the sign
        //    return ((exponent << 10) | (mantissa & 0x3ff)) ^ 0x200;
        //}


        //-------------------------------------------------
        //  decode_fp - given a 3.10 floating-point value,
        //  convert it to a signed 16-bit value
        //-------------------------------------------------
        //inline int16_t decode_fp(int16_t value)
        //{
        //    // invert the sign and the exponent
        //    value ^= 0x1e00;
        //
        //    // shift mantissa up to 16 bits then apply inverted exponent
        //    return int16_t(value << 6) >> bitfield(value, 10, 3);
        //}


        //-------------------------------------------------
        //  roundtrip_fp - compute the result of a round
        //  trip through the encode/decode process above
        //-------------------------------------------------
        //inline int16_t roundtrip_fp(int32_t value)
        //{
        //    // handle overflows first
        //    if (value < -32768)
        //        return -32768;
        //    if (value > 32767)
        //        return 32767;
        //
        //    // we need to count the number of leading sign bits after the sign
        //    // we can use count_leading_zeros if we invert negative values
        //    int32_t scanvalue = value ^ (int32_t(value) >> 31);
        //
        //    // exponent is related to the number of leading bits starting from bit 14
        //    int exponent = 7 - count_leading_zeros(scanvalue << 17);
        //
        //    // smallest exponent value allowed is 1
        //    exponent = std::max(exponent, 1);
        //
        //    // apply the shift back and forth to zero out bits that are lost
        //    exponent -= 1;
        //    return (value >> exponent) << exponent;
        //}
    }


    //*********************************************************
    //  HELPER CLASSES
    //*********************************************************
    // various envelope states
    enum envelope_state : uint32_t
    {
        EG_DEPRESS = 0,     // OPLL only; set EG_HAS_DEPRESS to enable
        EG_ATTACK = 1,
        EG_DECAY = 2,
        EG_SUSTAIN = 3,
        EG_RELEASE = 4,
        EG_REVERB = 5,      // OPQ/OPZ only; set EG_HAS_REVERB to enable
        EG_STATES = 6
    }

    // external I/O access classes
    public enum access_class : uint32_t
    {
        ACCESS_IO = 0,
        ACCESS_ADPCM_A,
        ACCESS_ADPCM_B,
        ACCESS_PCM,
        ACCESS_CLASSES
    }


    //*********************************************************
    //  HELPER CLASSES
    //*********************************************************
    // ======================> ymfm_output
    // struct containing an array of output values
    //template<int NumOutputs>
    //struct ymfm_output
    public class ymfm_output<int_NumOutputs>
        where int_NumOutputs : int_const, new()
    {
        static readonly int NumOutputs = new int_NumOutputs().value;


        // internal state
        public int32_t [] data = new int32_t [NumOutputs];


        // clear all outputs to 0
        //ymfm_output &clear()
        //{
        //    for (uint32_t index = 0; index < NumOutputs; index++)
        //        data[index] = 0;
        //    return *this;
        //}

        // clamp all outputs to a 16-bit signed value
        //ymfm_output &clamp16()
        //{
        //    for (uint32_t index = 0; index < NumOutputs; index++)
        //        data[index] = clamp(data[index], -32768, 32767);
        //    return *this;
        //}

        // run each output value through the floating-point processor
        //ymfm_output &roundtrip_fp()
        //{
        //    for (uint32_t index = 0; index < NumOutputs; index++)
        //        data[index] = ymfm::roundtrip_fp(data[index]);
        //    return *this;
        //}
    }


    // ======================> ymfm_saved_state
    // this class contains a managed vector of bytes that is used to save and
    // restore state
    public class ymfm_saved_state
    {
        // internal state
        std.vector<uint8_t> m_buffer;
        int32_t m_offset;


        // construction
        public ymfm_saved_state(std.vector<uint8_t> buffer, bool saving)
        {
            m_buffer = buffer;
            m_offset = saving ? -1 : 0;


            if (saving)
                buffer.resize(0);
        }


        // are we saving or restoring?
        //bool saving() const { return (m_offset < 0); }

        // generic save/restore
        //template<typename DataType>
        //void save_restore(DataType &data)
        //{
        //    if (saving())
        //        save(data);
        //    else
        //        restore(data);
        //}


        // save data to the buffer
        //void save(bool &data) { write(data ? 1 : 0); }
        //void save(int8_t &data) { write(data); }
        //void save(uint8_t &data) { write(data); }
        //void save(int16_t &data) { write(data).write(data >> 8); }
        //void save(uint16_t &data) { write(data).write(data >> 8); }
        //void save(int32_t &data) { write(data).write(data >> 8).write(data >> 16).write(data >> 24); }
        //void save(uint32_t &data) { write(data).write(data >> 8).write(data >> 16).write(data >> 24); }
        //void save(envelope_state &data) { write(uint8_t(data)); }
        //template<typename DataType, int Count>
        //void save(DataType (&data)[Count]) { for (uint32_t index = 0; index < Count; index++) save(data[index]); }

        // restore data from the buffer
        //void restore(bool &data) { data = read() ? true : false; }
        //void restore(int8_t &data) { data = read(); }
        //void restore(uint8_t &data) { data = read(); }
        //void restore(int16_t &data) { data = read(); data |= read() << 8; }
        //void restore(uint16_t &data) { data = read(); data |= read() << 8; }
        //void restore(int32_t &data) { data = read(); data |= read() << 8; data |= read() << 16; data |= read() << 24; }
        //void restore(uint32_t &data) { data = read(); data |= read() << 8; data |= read() << 16; data |= read() << 24; }
        //void restore(envelope_state &data) { data = envelope_state(read()); }
        //template<typename DataType, int Count>
        //void restore(DataType (&data)[Count]) { for (uint32_t index = 0; index < Count; index++) restore(data[index]); }

        // internal helper
        //ymfm_saved_state &write(uint8_t data) { m_buffer.push_back(data); return *this; }
        //uint8_t read() { return (m_offset < int32_t(m_buffer.size())) ? m_buffer[m_offset++] : 0; }
    }


    //*********************************************************
    //  INTERFACE CLASSES
    //*********************************************************
    // ======================> ymfm_engine_callbacks
    // this class represents functions in the engine that the ymfm_interface
    // needs to be able to call; it is represented here as a separate interface
    // that is independent of the actual engine implementation
    public interface ymfm_engine_callbacks
    {
        // timer callback; called by the interface when a timer fires
        void engine_timer_expired(uint32_t tnum);

        // check interrupts; called by the interface after synchronization
        void engine_check_interrupts();

        // mode register write; called by the interface after synchronization
        void engine_mode_write(uint8_t data);
    }


    // ======================> ymfm_interface
    // this class represents the interface between the fm_engine and the outside
    // world; it provides hooks for timers, synchronization, and I/O
    public class ymfm_interface
    {
        // the engine is our friend
        //template<typename RegisterType> friend class fm_engine_base;


        // pointer to engine callbacks -- this is set directly by the engine at
        // construction time
        public ymfm_engine_callbacks m_engine;


        // the following functions must be implemented by any derived classes; the
        // default implementations are sufficient for some minimal operation, but will
        // likely need to be overridden to integrate with the outside world; they are
        // all prefixed with ymfm_ to reduce the likelihood of namespace collisions

        //
        // timing and synchronizaton
        //

        // the chip implementation calls this when a write happens to the mode
        // register, which could affect timers and interrupts; our responsibility
        // is to ensure the system is up to date before calling the engine's
        // engine_mode_write() method
        protected virtual void ymfm_sync_mode_write(uint8_t data) { m_engine.engine_mode_write(data); }

        // the chip implementation calls this when the chip's status has changed,
        // which may affect the interrupt state; our responsibility is to ensure
        // the system is up to date before calling the engine's
        // engine_check_interrupts() method
        protected virtual void ymfm_sync_check_interrupts() { m_engine.engine_check_interrupts(); }

        // the chip implementation calls this when one of the two internal timers
        // has changed state; our responsibility is to arrange to call the engine's
        // engine_timer_expired() method after the provided number of clocks; if
        // duration_in_clocks is negative, we should cancel any outstanding timers
        protected virtual void ymfm_set_timer(uint32_t tnum, int32_t duration_in_clocks) { }

        // the chip implementation calls this to indicate that the chip should be
        // considered in a busy state until the given number of clocks has passed;
        // our responsibility is to compute and remember the ending time based on
        // the chip's clock for later checking
        protected virtual void ymfm_set_busy_end(uint32_t clocks) { }

        // the chip implementation calls this to see if the chip is still currently
        // is a busy state, as specified by a previous call to ymfm_set_busy_end();
        // our responsibility is to compare the current time against the previously
        // noted busy end time and return true if we haven't yet passed it
        protected virtual bool ymfm_is_busy() { return false; }

        //
        // I/O functions
        //

        // the chip implementation calls this when the state of the IRQ signal has
        // changed due to a status change; our responsibility is to respond as
        // needed to the change in IRQ state, signaling any consumers
        protected virtual void ymfm_update_irq(bool asserted) { }

        // the chip implementation calls this whenever data is read from outside
        // of the chip; our responsibility is to provide the data requested
        protected virtual uint8_t ymfm_external_read(access_class type, uint32_t address) { return 0; }

        // the chip implementation calls this whenever data is written outside
        // of the chip; our responsibility is to pass the written data on to any consumers
        protected virtual void ymfm_external_write(access_class type, uint32_t address, uint8_t data) { }
    }
}
