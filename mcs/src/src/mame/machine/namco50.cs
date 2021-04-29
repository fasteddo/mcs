// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;


namespace mame
{
    /* device get info callback */
    public class namco_50xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NAMCO_50XX, namco_50xx_device, "namco50", "Namco 50xx")
        static device_t device_creator_namco_50xx_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new namco_50xx_device(mconfig, tag, owner, clock); }
        public static readonly device_type NAMCO_50XX = DEFINE_DEVICE_TYPE(device_creator_namco_50xx_device, "namco50", "Namco 50xx");


        //ROM_START( namco_50xx )
        static readonly MemoryContainer<tiny_rom_entry> rom_namco_50xx = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x800, "mcu", 0 ),
            ROM_LOAD( "50xx.bin",     0x0000, 0x0800, CRC("a0acbaf7") + SHA1("f03c79451e73b3a93c1591cdb27fedc9f130508d") ),
            ROM_END,
        };


        // internal state
        required_device<mb88_cpu_device> m_cpu;
        uint8_t m_rw;
        uint8_t m_cmd;
        uint8_t m_portO;
        emu_timer m_irq_cleared_timer;


        namco_50xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_50XX, tag, owner, clock)
        {
            m_cpu = new required_device<mb88_cpu_device>(this, "mcu");
            m_rw = 0;
            m_cmd = 0;
            m_portO = 0;
        }


        //WRITE_LINE_MEMBER( namco_50xx_device::reset )
        public void reset(int state)
        {
            // The incoming signal is active low
            m_cpu.op[0].set_input_line(g.INPUT_LINE_RESET, state == 0 ? 1 : 0);
        }


        //WRITE_LINE_MEMBER( namco_50xx_device::chip_select )
        public void chip_select(int state)
        {
            m_cpu.op[0].set_input_line(0, state);
        }


        //WRITE_LINE_MEMBER( namco_50xx_device::rw )
        public void rw(int state)
        {
            machine().scheduler().synchronize(rw_sync, state);
        }


        public void write(uint8_t data)
        {
            machine().scheduler().synchronize(write_sync, data);
        }


        public uint8_t read()
        {
            return m_portO;
        }


        uint8_t K_r()
        {
            return (uint8_t)(m_cmd >> 4);
        }

        uint8_t R0_r()
        {
            return (uint8_t)(m_cmd & 0x0f);
        }

        uint8_t R2_r()
        {
            return (uint8_t)(m_rw & 1);
        }

        void O_w(uint8_t data)
        {
            uint8_t out_ = (uint8_t)(data & 0x0f);
            if ((data & 0x10) != 0)
                m_portO = (uint8_t)((m_portO & 0x0f) | (out_ << 4));
            else
                m_portO = (uint8_t)((m_portO & 0xf0) | (out_));
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            save_item(NAME(new { m_rw }));
            save_item(NAME(new { m_cmd }));
            save_item(NAME(new { m_portO }));
        }

        //-------------------------------------------------
        //  device_rom_region - return a pointer to the
        //  the device's ROM definitions
        //-------------------------------------------------
        protected override Pointer<tiny_rom_entry> device_rom_region()
        {
            return new Pointer<tiny_rom_entry>(rom_namco_50xx);
        }

        //-------------------------------------------------
        //  device_add_mconfig - add device configuration
        //-------------------------------------------------
        protected override void device_add_mconfig(machine_config config)
        {
            MB8842(config, m_cpu, g.DERIVED_CLOCK(1,1)); /* parent clock, internally divided by 6 */
            m_cpu.op[0].read_k().set(K_r).reg();
            m_cpu.op[0].read_r(0).set(R0_r).reg();
            m_cpu.op[0].read_r(2).set(R2_r).reg();
            m_cpu.op[0].write_o().set(O_w).reg();
        }


        //TIMER_CALLBACK_MEMBER( namco_50xx_device::rw_sync )
        void rw_sync(object ptr, int param)
        {
            m_rw = (uint8_t)param;
        }


        //TIMER_CALLBACK_MEMBER( namco_50xx_device::write_sync )
        void write_sync(object ptr, int param)
        {
            m_cmd = (uint8_t)param;
        }
    }
}
