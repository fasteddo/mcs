// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    // ======================> er2055_device
    public class er2055_device : device_t
                                 //public device_nvram_interface
    {
        // device type definition
        //DEFINE_DEVICE_TYPE(ER2055, er2055_device, "er2055", "ER2055 EAROM (64x8)")
        static device_t device_creator_er2055_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new er2055_device(mconfig, tag, owner, clock); }
        public static readonly device_type ER2055 = DEFINE_DEVICE_TYPE(device_creator_er2055_device, "er2055", "ER2055 EAROM (64x8)");


        public class device_nvram_interface_er2055 : device_nvram_interface
        {
            public device_nvram_interface_er2055(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void nvram_default() { ((er2055_device)device()).device_nvram_interface_nvram_default(); }
            protected override void nvram_read(emu_file file) { ((er2055_device)device()).device_nvram_interface_nvram_read(file); }
            protected override void nvram_write(emu_file file) { ((er2055_device)device()).device_nvram_interface_nvram_write(file); }
        }


        const int SIZE_DATA = 0x40;

        const uint8_t CK  = 0x01;
        const uint8_t C1  = 0x02;
        const uint8_t C2  = 0x04;
        const uint8_t CS1 = 0x08;
        const uint8_t CS2 = 0x10;


        optional_region_ptr<uint8_t> m_default_data;

        // internal state
        uint8_t m_control_state;
        uint8_t m_address;
        uint8_t m_data;
        uint8_t [] m_rom_data;  //std::unique_ptr<uint8_t[]> m_rom_data;


        // construction/destruction
        er2055_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, ER2055, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_nvram_interface_er2055(mconfig, this));  //device_nvram_interface(mconfig, *this),

            m_default_data = new optional_region_ptr<uint8_t>(this, g.DEVICE_SELF);
            m_control_state = 0;
            m_address = 0;
            m_data = 0;
        }


        // I/O operations
        public uint8_t data() { return m_data; }
        public void set_address(uint8_t address) { m_address = (uint8_t)(address & 0x3f); }
        public void set_data(uint8_t data) { m_data = data; }


        // control lines -- all lines are specified as active-high (even CS2)
        //-------------------------------------------------
        //  set_control - set the control lines; these
        //  must be done simultaneously because the chip
        //  reacts to various combinations
        //-------------------------------------------------
        public void set_control(uint8_t cs1, uint8_t cs2, uint8_t c1, uint8_t c2)
        {
            // create a new composite control state
            uint8_t oldstate = m_control_state;
            m_control_state = (uint8_t)(oldstate & CK);
            m_control_state |= (c1 != 0) ? C1 : (uint8_t)0;
            m_control_state |= (c2 != 0) ? C2 : (uint8_t)0;
            m_control_state |= (cs1 != 0) ? CS1 : (uint8_t)0;
            m_control_state |= (cs2 != 0) ? CS2 : (uint8_t)0;

            // if not selected, or if change from previous, we're done
            if ((m_control_state & (CS1 | CS2)) != (CS1 | CS2) || m_control_state == oldstate)
                return;

            update_state();
        }


        //-------------------------------------------------
        //  set_clk - set the CLK line, pulses on which
        //  are required for every read operation and for
        //  successive write or erase operations
        //-------------------------------------------------
        //WRITE_LINE_MEMBER(er2055_device::set_clk)
        public void set_clk(int state)
        {
            uint8_t oldstate = m_control_state;
            if (state != 0)
                m_control_state |= CK;
            else
                m_control_state &= unchecked((byte)~CK);

            // updates occur on falling edge when chip is selected
            if ((m_control_state & (CS1 | CS2)) == (CS1 | CS2) && (m_control_state != oldstate) && state == 0)
            {
                // read mode (C2 is "Don't Care")
                if ((m_control_state & C1) == C1)
                {
                    m_data = m_rom_data[m_address];
                    LOG("Read {0} = {1}\n", m_address, m_data);  // %02X = %02X
                }

                update_state();
            }
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            save_item(NAME(new { m_control_state }));
            save_item(NAME(new { m_address }));
            save_item(NAME(new { m_data }));

            m_rom_data = new uint8_t[SIZE_DATA];  // std::make_unique<uint8_t[]>(SIZE_DATA);

            //save_pointer(NAME(m_rom_data), SIZE_DATA);

            m_control_state = 0;
        }


        // device_nvram_interface overrides
        //-------------------------------------------------
        //  nvram_default - called to initialize NVRAM to
        //  its default state
        //-------------------------------------------------
        void device_nvram_interface_nvram_default()
        {
            //throw new emu_unimplemented();
#if false
            // default to all-0xff
            std::fill_n(&m_rom_data[0], SIZE_DATA, 0xff);

            // populate from a memory region if present
            if (m_default_data.found())
                std::copy_n(&m_default_data[0], SIZE_DATA, &m_rom_data[0]);
#endif
        }


        //-------------------------------------------------
        //  nvram_read - called to read NVRAM from the
        //  .nv file
        //-------------------------------------------------
        void device_nvram_interface_nvram_read(emu_file file)
        {
            throw new emu_unimplemented();
#if false
            file.read(&m_rom_data[0], SIZE_DATA);
#endif
        }

        //-------------------------------------------------
        //  nvram_write - called to write NVRAM to the
        //  .nv file
        //-------------------------------------------------
        void device_nvram_interface_nvram_write(emu_file file)
        {
            throw new emu_unimplemented();
#if false
            file.write(&m_rom_data[0], SIZE_DATA);
#endif
        }


        //-------------------------------------------------
        //  update_state - update internal state following
        //  a transition on clock, control or chip select
        //  lines based on what mode we're in
        //-------------------------------------------------
        void update_state()
        {
            switch (m_control_state & (C1 | C2))
            {
                // write mode; erasing is required, so we perform an AND against previous
                // data to simulate incorrect behavior if erasing was not done
                case 0:
                    m_rom_data[m_address] &= m_data;
                    LOG("Write {0} = {1}\n", m_address, m_data);  // %02X = %02X
                    break;

                // erase mode
                case C2:
                    m_rom_data[m_address] = 0xff;
                    LOG("Erase {0}\n", m_address);  // %02X
                    break;
            }
        }
    }
}
