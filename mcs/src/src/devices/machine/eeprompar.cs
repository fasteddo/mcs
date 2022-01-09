// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;


namespace mame
{
    public class eeprom_parallel_base_device : eeprom_base_device
    {
        const int VERBOSE = 0;  //#define VERBOSE 1
        protected void LOG(string format, params object [] args) { LOG(VERBOSE, format, args); }


        // construction/destruction
        protected eeprom_parallel_base_device(machine_config mconfig, device_type devtype, string tag, device_t owner)
            : base(mconfig, devtype, tag, owner)
        {
        }


        // device-level overrides
        protected override void device_start()
        {
            // start the base class
            base.device_start();
        }


        protected override void device_reset()
        {
            // reset the base class
            base.device_reset();
        }
    }


    public class eeprom_parallel_28xx_device : eeprom_parallel_base_device
    {
        // set this to 1 to break Prop Cycle (28C64 page write emulation needed)
        const bool EMULATE_POLLING = false;


        // configuration state
        bool m_lock_after_write;         // lock EEPROM after writes

        // runtime state
        int m_oe;                       // state of OE line (-1 = synchronized with read)


        // construction/destruction
        //-------------------------------------------------
        //  eeprom_parallel_28xx_device - constructor
        //-------------------------------------------------
        protected eeprom_parallel_28xx_device(machine_config mconfig, device_type devtype, string tag, device_t owner)
            : base(mconfig, devtype, tag, owner)
        {
            m_lock_after_write = false;
            m_oe = -1;
        }


        // configuration helpers
        //void lock_after_write(bool lock) { m_lock_after_write = lock; }


        // read/write data lines

        //-------------------------------------------------
        //  read/write - read/write handlers
        //-------------------------------------------------
        public void write(offs_t offset, uint8_t data)
        {
            if (m_oe == 0)
            {
                // Master Boy writes every byte twice, resetting a control line in between, for some reason not clear
                if (internal_read(offset) != data)
                    LOG("{0}: Write attempted while /OE active (offset = {1}, data = {2})\n", machine().describe_context(), offset, data);
            }
            else
            {
                LOG("{0}: Write cycle started (offset = {1}, data = {2})\n", machine().describe_context(), offset, data);
                base.write(offset, data);
                if (m_lock_after_write)
                    m_oe = 0;
            }
        }


        public uint8_t read(address_space space, offs_t offset)
        {
            if (m_oe == 1)
            {
                LOG("{0}: Read attempted while /OE inactive (offset = {1})\n", machine().describe_context(), offset);
                return (uint8_t)space.unmap();
            }

            // if a write has not completed yet, the highest bit of data written will be read back inverted when polling the offset
            if (ready() || !EMULATE_POLLING)
            {
                return (uint8_t)base.read(offset);
            }
            else
            {
                LOG("{0}: Data read back before write completed (offset = {1})\n", machine().describe_context(), offset);
                return (uint8_t)(~internal_read(offset) & 0x80);
            }
        }


        // control lines
        //DECLARE_WRITE_LINE_MEMBER(oe_w);
        //void unlock_write8(uint8_t data);
        //void unlock_write16(uint16_t data);
        //void unlock_write32(uint32_t data);


        // device-level overrides
        protected override void device_start()
        {
            // start the base class
            base.device_start();

            save_item(NAME(new { m_oe }));
        }


        //-------------------------------------------------
        //  device_reset - device-specific reset
        //-------------------------------------------------
        protected override void device_reset()
        {
            // reset the base class
            base.device_reset();

            if (m_lock_after_write)
                m_oe = 0;
        }
    }


    //**************************************************************************
    //  DERIVED TYPES
    //**************************************************************************

    // macro for declaring a new device class
    //#define DECLARE_PARALLEL_EEPROM_DEVICE(_baseclass, _lowercase, _uppercase) \
    //class eeprom_parallel_##_lowercase##_device : public eeprom_parallel_##_baseclass##_device \
    //{ \
    //public: \
    //    eeprom_parallel_##_lowercase##_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock = 0); \
    //}; \
    //DECLARE_DEVICE_TYPE(EEPROM_##_uppercase, eeprom_parallel_##_lowercase##_device)

    // macro for defining a new device class
    //#define DEFINE_PARALLEL_EEPROM_DEVICE(_baseclass, _lowercase, _uppercase, _bits, _cells) \
    //eeprom_parallel_##_lowercase##_device::eeprom_parallel_##_lowercase##_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock) \
    //    : eeprom_parallel_##_baseclass##_device(mconfig, EEPROM_##_uppercase, tag, owner) \
    //{ \
    //    size(_cells, _bits); \
    //} \
    //DEFINE_DEVICE_TYPE(EEPROM_##_uppercase, eeprom_parallel_##_lowercase##_device, #_lowercase, "Parallel EEPROM " #_uppercase " (" #_cells "x" #_bits ")")

    // standard 28XX class of 8-bit EEPROMs
    //DECLARE_PARALLEL_EEPROM_DEVICE(28xx, 2804, 2804)
    public class eeprom_parallel_2804_device : eeprom_parallel_28xx_device
    {
        //DEFINE_DEVICE_TYPE(EEPROM_##_uppercase, eeprom_parallel_##_lowercase##_device, #_lowercase, "Parallel EEPROM " #_uppercase " (" #_cells "x" #_bits ")")
        static device_t device_creator_eeprom_parallel_2804_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new eeprom_parallel_2804_device(mconfig, tag, owner, clock); }
        public static readonly device_type EEPROM_2804 = DEFINE_DEVICE_TYPE(device_creator_eeprom_parallel_2804_device, "2804", "Parallel EEPROM 2804 (512x8)");

        //DEFINE_PARALLEL_EEPROM_DEVICE(28xx, 2804, 2804, 8, 512)
        eeprom_parallel_2804_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, EEPROM_2804, tag, owner)
        {
            size(512, 8);
        }
    }

    //DECLARE_PARALLEL_EEPROM_DEVICE(28xx, 2816, 2816)
    //DECLARE_PARALLEL_EEPROM_DEVICE(28xx, 2864, 2864)
    //DECLARE_PARALLEL_EEPROM_DEVICE(28xx, 28256, 28256)
    //DECLARE_PARALLEL_EEPROM_DEVICE(28xx, 28512, 28512)
    //DECLARE_PARALLEL_EEPROM_DEVICE(28xx, 28010, 28010)
    //DECLARE_PARALLEL_EEPROM_DEVICE(28xx, 28020, 28020)
    //DECLARE_PARALLEL_EEPROM_DEVICE(28xx, 28040, 28040)
}
