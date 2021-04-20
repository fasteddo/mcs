// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using offs_t = System.UInt32;
using PointerU8 = mame.Pointer<System.Byte>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    // ======================> eeprom_base_device
    public class eeprom_base_device : device_t
                                      //device_nvram_interface
    {
        class device_nvram_interface_eeprom : device_nvram_interface
        {
            public device_nvram_interface_eeprom(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void nvram_default() { ((eeprom_base_device)device()).device_nvram_interface_nvram_default(); }
            protected override void nvram_read(emu_file file) { throw new emu_unimplemented(); }
            protected override void nvram_write(emu_file file) { throw new emu_unimplemented(); }
        }


        // timing constants
        enum timing_type
        {
            WRITE_TIME,         // default = 2ms
            WRITE_ALL_TIME,     // default = 8ms
            ERASE_TIME,         // default = 1ms
            ERASE_ALL_TIME,     // default = 8ms
            TIMING_COUNT
        }


        device_nvram_interface_eeprom m_nvram_interface;


        optional_memory_region m_region;

        MemoryU8 m_data;  //std::unique_ptr<uint8_t []> m_data;

        // configuration state
        uint32_t m_cells;
        uint8_t m_address_bits;
        uint8_t m_data_bits;
        PointerU8 m_default_data;
        uint32_t m_default_data_size;
        uint32_t m_default_value;
        bool m_default_value_set;
        attotime [] m_operation_time = new attotime[(int)timing_type.TIMING_COUNT];

        // live state
        attotime m_completion_time;


        // construction/destruction
        //-------------------------------------------------
        //  eeprom_base_device - constructor
        //-------------------------------------------------
        protected eeprom_base_device(machine_config mconfig, device_type devtype, string tag, device_t owner)
            : base(mconfig, devtype, tag, owner, 0)
        {
            m_class_interfaces.Add(new device_nvram_interface_eeprom(mconfig, this));  //device_nvram_interface(mconfig, *this),

            m_nvram_interface = GetClassInterface<device_nvram_interface_eeprom>();


            m_region = new optional_memory_region(this, DEVICE_SELF);
            m_cells = 0;
            m_address_bits = 0;
            m_data_bits = 0;
            m_default_data = null;
            m_default_data_size = 0;
            m_default_value = 0;
            m_default_value_set = false;
            m_completion_time = attotime.zero;


            // a 2ms write time is too long for rfjetsa
            m_operation_time[(int)timing_type.WRITE_TIME]        = attotime.from_usec(1750);
            m_operation_time[(int)timing_type.WRITE_ALL_TIME]    = attotime.from_usec(8000);
            m_operation_time[(int)timing_type.ERASE_TIME]        = attotime.from_usec(1000);
            m_operation_time[(int)timing_type.ERASE_ALL_TIME]    = attotime.from_usec(8000);
        }


        // inline configuration helpers

        //-------------------------------------------------
        //  set_default_data - configuration helpers
        //  to set the default data
        //-------------------------------------------------
        protected eeprom_base_device size(int cells, int cellbits)
        {
            m_cells = (uint32_t)cells;
            m_data_bits = (uint8_t)cellbits;

            // compute address bits (validation checks verify cells was an even power of 2)
            cells--;
            m_address_bits = 0;
            while (cells != 0)
            {
                cells >>= 1;
                m_address_bits++;
            }

            return this;
        }


        //eeprom_base_device& default_data(const uint8_t *data, uint32_t size);
        //eeprom_base_device& default_data(const uint16_t *data, uint32_t size);
        //eeprom_base_device& default_value(uint32_t value) { m_default_value = value; m_default_value_set = true; return *this; }
        //eeprom_base_device& timing(timing_type type, const attotime &duration) { m_operation_time[type] = duration; return *this; }
        //eeprom_base_device& write_time(const attotime &duration) { m_operation_time[eeprom_base_device::WRITE_TIME] = duration; return *this; }
        //eeprom_base_device& write_all_time(const attotime &duration) { m_operation_time[eeprom_base_device::WRITE_ALL_TIME] = duration; return *this; }
        //eeprom_base_device& erase_time(const attotime &duration) { m_operation_time[eeprom_base_device::ERASE_TIME] = duration; return *this; }
        //eeprom_base_device& erase_all_time(const attotime &duration) { m_operation_time[eeprom_base_device::ERASE_ALL_TIME] = duration; return *this; }


        // read/write/erase data

        //-------------------------------------------------
        //  read - read data at the given address
        //-------------------------------------------------
        protected uint32_t read(offs_t address)
        {
            if (!ready())
                logerror("EEPROM: Read performed before previous operation completed!\n");
            return internal_read(address);
        }


        //-------------------------------------------------
        //  write - write data at the given address
        //-------------------------------------------------
        protected void write(offs_t address, uint32_t data)
        {
            if (!ready())
                logerror("EEPROM: Write performed before previous operation completed!\n");
            internal_write(address, data);
            m_completion_time = machine().time() + m_operation_time[(int)timing_type.WRITE_TIME];
        }



        //void write_all(uint32_t data);
        //void erase(offs_t address);
        //void erase_all();


        // status
        protected bool ready() { return machine().time() >= m_completion_time; }


        // internal read/write without side-effects

        //-------------------------------------------------
        //  internal_read - read data at the given address
        //-------------------------------------------------
        protected uint32_t internal_read(offs_t address)
        {
            if (m_data_bits == 16)
                return (uint32_t)m_data[address * 2] | ((uint32_t)m_data[address * 2 + 1] << 8);
            else
                return m_data[address];
        }


        //-------------------------------------------------
        //  internal_write - write data at the given
        //  address
        //-------------------------------------------------
        void internal_write(offs_t address, uint32_t data)
        {
            if (m_data_bits == 16)
            {
                m_data[address * 2] = (uint8_t)data;
                m_data[address * 2 + 1] = (uint8_t)(data >> 8);
            }
            else
            {
                m_data[address] = (uint8_t)data;
            }
        }


        // device-level overrides
        protected override void device_validity_check(validity_checker valid)
        {
            throw new emu_unimplemented();
        }


        protected override void device_start()
        {
            uint32_t size = (m_data_bits == 8 ? 1U : 2U) << m_address_bits;
            m_data = new MemoryU8((int)size, true);  //m_data = std::make_unique<uint8_t []>(size);

            // save states
            save_item(NAME(new { m_completion_time }));

            //throw new emu_unimplemented();
#if false
            save_pointer(NAME(m_data), size);
#endif
        }


        protected override void device_reset()
        {
            // reset any pending operations
            m_completion_time = attotime.zero;
        }


        // device_nvram_interface overrides

        //-------------------------------------------------
        //  nvram_default - called to initialize NVRAM to
        //  its default state
        //-------------------------------------------------
        void device_nvram_interface_nvram_default()
        {
            uint32_t eeprom_length = 1U << m_address_bits;
            uint32_t eeprom_bytes = eeprom_length * m_data_bits / 8;

            // initialize to the default value
            uint32_t default_value = m_default_value_set ? m_default_value : ~0U;
            for (offs_t offs = 0; offs < eeprom_length; offs++)
                internal_write(offs, default_value);

            // handle hard-coded data from the driver
            if (m_default_data != null)
            {
                osd_printf_verbose("Warning: Driver-specific EEPROM defaults are going away soon.\n");
                for (offs_t offs = 0; offs < m_default_data_size; offs++)
                {
                    if (m_data_bits == 8)
                        internal_write(offs, new PointerU8(m_default_data)[offs]);  //internal_write(offs, static_cast<const u8 *>(m_default_data)[offs]);
                    else
                        internal_write(offs, new PointerU16(m_default_data)[offs]);  //internal_write(offs, static_cast<const u16 *>(m_default_data)[offs]);
                }
            }

            // populate from a memory region if present
            if (m_region.found())
            {
                if (m_region.target.bytes() != eeprom_bytes)
                    fatalerror("eeprom region '{0}' wrong size (expected size = 0x{1})\n", tag(), eeprom_bytes);
                if (m_data_bits == 8 && m_region.target.bytewidth() != 1)
                    fatalerror("eeprom region '{0}' needs to be an 8-bit region\n", tag());
                if (m_data_bits == 16 && m_region.target.bytewidth() != 2)
                    fatalerror("eeprom region '{0}' needs to be a 16-bit region\n", tag());
                osd_printf_verbose("Loading data from EEPROM region '{0}'\n", tag());

                memcpy(m_data, m_region.target.base_(), eeprom_bytes);
            }
        }


        //virtual void nvram_read(emu_file &file) override;
        //virtual void nvram_write(emu_file &file) override;
    }
}
