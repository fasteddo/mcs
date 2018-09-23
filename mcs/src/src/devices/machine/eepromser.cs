// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;


namespace mame
{
    // ======================> eeprom_serial_base_device
    abstract class eeprom_serial_base_device : eeprom_base_device
    {
        // read interfaces differ between implementations

        public enum eeprom_serial_streaming
        {
            DISABLE = 0,
            ENABLE = 1
        }


        // commands
        enum eeprom_command
        {
            COMMAND_INVALID,
            COMMAND_READ,
            COMMAND_WRITE,
            COMMAND_ERASE,
            COMMAND_LOCK,
            COMMAND_UNLOCK,
            COMMAND_WRITEALL,
            COMMAND_ERASEALL,
            COMMAND_COPY_EEPROM_TO_RAM,
            COMMAND_COPY_RAM_TO_EEPROM
        }


        // states
        enum eeprom_state
        {
            STATE_IN_RESET,
            STATE_WAIT_FOR_START_BIT,
            STATE_WAIT_FOR_COMMAND,
            STATE_READING_DATA,
            STATE_WAIT_FOR_DATA,
            STATE_WAIT_FOR_COMPLETION
        }


        // events
        enum eeprom_event
        {
            EVENT_CS_RISING_EDGE = 1 << 0,
            EVENT_CS_FALLING_EDGE = 1 << 1,
            EVENT_CLK_RISING_EDGE = 1 << 2,
            EVENT_CLK_FALLING_EDGE = 1 << 3
        }


        // configuration state
        //uint8_t         m_command_address_bits;     // number of address bits in a command
        //bool            m_streaming_enabled;        // true if streaming is enabled
        //bool            m_output_on_falling_clock_enabled;  // true if the output pin is updated on the falling edge of the clock
        //devcb_write_line m_do_cb;                   // callback to push state of DO line

        // runtime state
        //eeprom_state    m_state;                    // current internal state
        //uint8_t         m_cs_state;                 // state of the CS line
        //attotime        m_last_cs_rising_edge_time; // time of the last CS rising edge
        //uint8_t         m_oe_state;                 // state of the OE line
        //uint8_t         m_clk_state;                // state of the CLK line
        //uint8_t         m_di_state;                 // state of the DI line
        //bool            m_locked;                   // are we locked against writes?
        //uint32_t        m_bits_accum;               // number of bits accumulated
        //uint32_t        m_command_address_accum;    // accumulator of command+address bits
        //eeprom_command  m_command;                  // current command
        //uint32_t        m_address;                  // current address extracted from command
        //uint32_t        m_shift_register;           // holds data coming in/going out


        // inline configuration helpers
        //void enable_streaming(bool enable) { m_streaming_enabled = enable; }
        //void enable_output_on_falling_clock(bool enable) { m_output_on_falling_clock_enabled = enable; }
        //template<class Object> devcb_base &set_do_callback(Object &&cb) { return m_do_cb.set_callback(std::forward<Object>(cb)); }
        //auto do_callback() { return m_do_cb.bind(); }


        // construction/destruction
        protected eeprom_serial_base_device(machine_config mconfig, device_type devtype, string tag, device_t owner, eeprom_serial_streaming enable_streaming)
            : base(mconfig, devtype, tag, owner)
        {
            throw new emu_unimplemented();
        }


        // device-level overrides
        //virtual void device_start() override;
        //virtual void device_reset() override;


        // internal helpers
        //void set_address_bits(int addrbits) { m_command_address_bits = addrbits; }
        //void set_state(eeprom_state newstate);
        //void execute_write_command();

        // subclass helpers
        //void base_cs_write(int state);
        //void base_clk_write(int state);
        //void base_di_write(int state);
        //int base_do_read();
        //int base_ready_read();


        // subclass overrides

        //virtual void handle_event(eeprom_event event);

        protected abstract void parse_command_and_address();

        //virtual void execute_command();
    }


    // ======================> eeprom_serial_93cxx_device
    class eeprom_serial_93cxx_device : eeprom_serial_base_device
    {
        // read handlers
        //DECLARE_READ_LINE_MEMBER(do_read);  // combined DO+READY/BUSY

        // write handlers
        //DECLARE_WRITE_LINE_MEMBER(cs_write);        // CS signal (active high)
        //DECLARE_WRITE_LINE_MEMBER(clk_write);       // CLK signal (active high)
        //DECLARE_WRITE_LINE_MEMBER(di_write);        // DI


        // construction/destruction
        //using eeprom_serial_base_device::eeprom_serial_base_device;
        eeprom_serial_93cxx_device(machine_config mconfig, device_type devtype, string tag, device_t owner, eeprom_serial_streaming enable_streaming)
            : base(mconfig, devtype, tag, owner, enable_streaming)
        {
            throw new emu_unimplemented();
        }


        // subclass overrides
        protected override void parse_command_and_address() { throw new emu_unimplemented(); }
    }
}
