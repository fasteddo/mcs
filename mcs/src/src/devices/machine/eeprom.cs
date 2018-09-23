// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;


namespace mame
{
    // ======================> eeprom_base_device
    class eeprom_base_device : device_t
                               //device_nvram_interface
    {
        // timing constants
        enum timing_type
        {
            WRITE_TIME,         // default = 2ms
            WRITE_ALL_TIME,     // default = 8ms
            ERASE_TIME,         // default = 1ms
            ERASE_ALL_TIME,     // default = 8ms
            TIMING_COUNT
        }


        //optional_memory_region  m_region;

        //std::unique_ptr<uint8_t []> m_data;

        // configuration state
        //uint32_t                  m_cells;
        //uint8_t                   m_address_bits;
        //uint8_t                   m_data_bits;
        //const void *            m_default_data;
        //uint32_t                  m_default_data_size;
        //uint32_t                  m_default_value;
        //bool                    m_default_value_set;
        //attotime                m_operation_time[TIMING_COUNT];

        // live state
        //attotime                m_completion_time;


        // inline configuration helpers
        //eeprom_base_device& size(int cells, int cellbits);
        //eeprom_base_device& default_data(const uint8_t *data, uint32_t size);
        //eeprom_base_device& default_data(const uint16_t *data, uint32_t size);
        //eeprom_base_device& default_value(uint32_t value) { m_default_value = value; m_default_value_set = true; return *this; }
        //eeprom_base_device& timing(timing_type type, const attotime &duration) { m_operation_time[type] = duration; return *this; }
        //eeprom_base_device& write_time(const attotime &duration) { m_operation_time[eeprom_base_device::WRITE_TIME] = duration; return *this; }
        //eeprom_base_device& write_all_time(const attotime &duration) { m_operation_time[eeprom_base_device::WRITE_ALL_TIME] = duration; return *this; }
        //eeprom_base_device& erase_time(const attotime &duration) { m_operation_time[eeprom_base_device::ERASE_TIME] = duration; return *this; }
        //eeprom_base_device& erase_all_time(const attotime &duration) { m_operation_time[eeprom_base_device::ERASE_ALL_TIME] = duration; return *this; }


        // read/write/erase data
        //uint32_t read(offs_t address);
        //void write(offs_t address, uint32_t data);
        //void write_all(uint32_t data);
        //void erase(offs_t address);
        //void erase_all();


        // status
        //bool ready() const { return machine().time() >= m_completion_time; }

        // internal read/write without side-effects
        //uint32_t internal_read(offs_t address);
        //void internal_write(offs_t address, uint32_t data);


        // construction/destruction
        protected eeprom_base_device(machine_config mconfig, device_type devtype, string tag, device_t owner)
            : base(mconfig, devtype, tag, owner, 0)
        {
            throw new emu_unimplemented();
        }


        // device-level overrides
        //virtual void device_validity_check(validity_checker &valid) const override;
        //virtual void device_start() override;
        //virtual void device_reset() override;


        // device_nvram_interface overrides
        //virtual void nvram_default() override;
        //virtual void nvram_read(emu_file &file) override;
        //virtual void nvram_write(emu_file &file) override;
    }
}
