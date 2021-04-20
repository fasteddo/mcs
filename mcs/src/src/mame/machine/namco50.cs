// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
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
        static readonly List<tiny_rom_entry> rom_namco_50xx = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x800, "mcu", 0 ),
            ROM_LOAD( "50xx.bin",     0x0000, 0x0800, CRC("a0acbaf7") + SHA1("f03c79451e73b3a93c1591cdb27fedc9f130508d") ),
            ROM_END,
        };


        // internal state
        required_device<mb88_cpu_device> m_cpu;
        uint8_t m_latched_cmd;
        uint8_t m_latched_rw;
        uint8_t m_portO;
        emu_timer m_irq_cleared_timer;


        namco_50xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_50XX, tag, owner, clock)
        {
            m_cpu = new required_device<mb88_cpu_device>(this, "mcu");
            m_latched_cmd = 0;
            m_latched_rw = 0;
            m_portO = 0;
        }


        //WRITE8_MEMBER( namco_50xx_device::write )
        public void write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            machine().scheduler().synchronize(latch_callback);  //timer_expired_delegate(FUNC(namco_50xx_device::latch_callback),this), data);

            irq_set();
        }


        //WRITE_LINE_MEMBER(namco_50xx_device::read_request)
        public void read_request(int state)
        {
            machine().scheduler().synchronize(readrequest_callback);  //timer_expired_delegate(FUNC(namco_50xx_device::readrequest_callback),this), 0);

            irq_set();
        }


        //READ8_MEMBER( namco_50xx_device::read )
        public u8 read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            byte res = m_portO;

            read_request(0);

            return res;
        }


        //READ8_MEMBER( namco_50xx_device::K_r )
        u8 K_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return (byte)(m_latched_cmd >> 4);
        }

        //READ8_MEMBER( namco_50xx_device::R0_r )
        u8 R0_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return (byte)(m_latched_cmd & 0x0f);
        }

        //READ8_MEMBER( namco_50xx_device::R2_r )
        u8 R2_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return (byte)(m_latched_rw & 1);
        }

        //WRITE8_MEMBER( namco_50xx_device::O_w )
        void O_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            byte out_value = (byte)(data & 0x0f);
            if ((data & 0x10) != 0)
                m_portO = (byte)((m_portO & 0x0f) | (out_value << 4));
            else
                m_portO = (byte)((m_portO & 0xf0) | (out_value));
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            m_irq_cleared_timer = machine().scheduler().timer_alloc(irq_clear);  //timer_expired_delegate(FUNC(namco_50xx_device::irq_clear), this));

            save_item(NAME(new { m_latched_cmd }));
            save_item(NAME(new { m_latched_rw }));
            save_item(NAME(new { m_portO }));
        }

        //-------------------------------------------------
        //  device_rom_region - return a pointer to the
        //  the device's ROM definitions
        //-------------------------------------------------
        protected override List<tiny_rom_entry> device_rom_region()
        {
            return rom_namco_50xx;
        }

        //-------------------------------------------------
        //  device_add_mconfig - add device configuration
        //-------------------------------------------------
        protected override void device_add_mconfig(machine_config config)
        {
            MB8842(config, m_cpu, DERIVED_CLOCK(1,1)); /* parent clock, internally divided by 6 */
            m_cpu.target.read_k().set(K_r).reg();
            m_cpu.target.write_o().set(O_w).reg();
            m_cpu.target.read_r(0).set(R0_r).reg();
            m_cpu.target.read_r(2).set(R2_r).reg();
        }

        //TIMER_CALLBACK_MEMBER( namco_50xx_device::latch_callback )
        void latch_callback(object ptr, int param)
        {
            m_latched_cmd = (byte)param;
            m_latched_rw = 0;
        }

        //TIMER_CALLBACK_MEMBER( namco_50xx_device::readrequest_callback )
        void readrequest_callback(object ptr, int param)
        {
            m_latched_rw = 1;
        }

        //TIMER_CALLBACK_MEMBER( namco_50xx_device::irq_clear )
        void irq_clear(object ptr, int param)
        {
            m_cpu.target.set_input_line(0, CLEAR_LINE);
        }

        void irq_set()
        {
            m_cpu.target.set_input_line(0, ASSERT_LINE);

            // The execution time of one instruction is ~4us, so we must make sure to
            // give the cpu time to poll the /IRQ input before we clear it.
            // The input clock to the 06XX interface chip is 64H, that is
            // 18432000/6/64 = 48kHz, so it makes sense for the irq line to be
            // asserted for one clock cycle ~= 21us.
            m_irq_cleared_timer.adjust(attotime.from_usec(21), 0);
        }
    }
}
