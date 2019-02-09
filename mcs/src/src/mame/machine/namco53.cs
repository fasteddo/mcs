// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    public class namco_53xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NAMCO_53XX, namco_53xx_device, "namco53", "Namco 53xx")
        static device_t device_creator_namco_53xx_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new namco_53xx_device(mconfig, tag, owner, clock); }
        public static readonly device_type NAMCO_53XX = DEFINE_DEVICE_TYPE(device_creator_namco_53xx_device, "namco53", "Namco 53xx");


        //ROM_START( namco_53xx )
        static readonly List<tiny_rom_entry> rom_namco_53xx = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x400, "mcu", 0 ),
            ROM_LOAD( "53xx.bin",     0x0000, 0x0400, CRC("b326fecb") + SHA1("758d8583d658e4f1df93184009d86c3eb8713899") ),
            ROM_END(),
        };


        // internal state
        required_device<mb88_cpu_device> m_cpu;
        byte m_portO;
        devcb_read8 m_k;
        devcb_read8 [] m_in = new devcb_read8[4];
        devcb_write8 m_p;
        emu_timer m_irq_cleared_timer;


        namco_53xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_53XX, tag, owner, clock)
        {
            m_cpu = new required_device<mb88_cpu_device>(this, "mcu");
            m_portO = 0;
            m_k = new devcb_read8(this);
            for (int i = 0; i < 4; i++)
                m_in[i] = new devcb_read8(this);
            m_p = new devcb_write8(this);
        }


        public devcb_read8.binder input_callback(UInt32 N) { return m_in[N].bind(); }  //template <unsigned N> auto input_callback() { return m_in[N].bind(); }

        public devcb_read8.binder k_port_callback() { return m_k.bind(); }  //auto k_port_callback() { return m_k.bind(); }
        //auto p_port_callback() { return m_p.bind(); }


        //READ8_MEMBER( namco_53xx_device::K_r )
        public u8 K_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return m_k.op(0);
        }

        //READ8_MEMBER( namco_53xx_device::R0_r )
        public u8 R0_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return m_in[0].op(0);
        }

        //READ8_MEMBER( namco_53xx_device::R1_r )
        public u8 R1_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return m_in[1].op(0);
        }

        //READ8_MEMBER( namco_53xx_device::R2_r )
        public u8 R2_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return m_in[2].op(0);
        }

        //READ8_MEMBER( namco_53xx_device::R3_r )
        public u8 R3_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return m_in[3].op(0);
        }

        //WRITE8_MEMBER( namco_53xx_device::O_w )
        public void O_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            byte out_value = (byte)(data & 0x0f);
            if ((data & 0x10) != 0)
                m_portO = (byte)((m_portO & 0x0f) | (out_value << 4));
            else
                m_portO = (byte)((m_portO & 0xf0) | (out_value));
        }

        //WRITE8_MEMBER( namco_53xx_device::P_w )
        public void P_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_p.op(space, 0, data);
        }


        //WRITE_LINE_MEMBER(namco_53xx_device::read_request)
        public void read_request(int state)
        {
            m_cpu.target.set_input_line(0, ASSERT_LINE);

            // The execution time of one instruction is ~4us, so we must make sure to
            // give the cpu time to poll the /IRQ input before we clear it.
            // The input clock to the 06XX interface chip is 64H, that is
            // 18432000/6/64 = 48kHz, so it makes sense for the irq line to be
            // asserted for one clock cycle ~= 21us.
            m_irq_cleared_timer.adjust(attotime.from_usec(21), 0);
        }


        //READ8_MEMBER( namco_53xx_device::read )
        public u8 read(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            byte res = m_portO;

            read_request(0);

            return res;
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            /* resolve our read/write callbacks */
            m_k.resolve_safe(0);
            foreach (devcb_read8 cb in m_in)
                cb.resolve_safe(0);
            m_p.resolve_safe();

            m_irq_cleared_timer = machine().scheduler().timer_alloc(irq_clear);  //timer_expired_delegate(FUNC(namco_53xx_device::irq_clear), this));

            save_item(m_portO, "m_portO");
        }

        //-------------------------------------------------
        //  device_rom_region - return a pointer to the
        //  the device's ROM definitions
        //-------------------------------------------------
        protected override List<tiny_rom_entry> device_rom_region()
        {
            return rom_namco_53xx;
        }


        //-------------------------------------------------
        //  device_add_mconfig - add device configuration
        //-------------------------------------------------
        protected override void device_add_mconfig(machine_config config)
        {
            MB8843(config, m_cpu, DERIVED_CLOCK(1,1)); /* parent clock, internally divided by 6 */
            m_cpu.target.read_k().set(K_r).reg();
            m_cpu.target.write_o().set(O_w).reg();
            m_cpu.target.write_p().set(P_w).reg();
            m_cpu.target.read_r(0).set(R0_r).reg();
            m_cpu.target.read_r(1).set(R1_r).reg();
            m_cpu.target.read_r(2).set(R2_r).reg();
            m_cpu.target.read_r(3).set(R3_r).reg();
        }


        //TIMER_CALLBACK_MEMBER( namco_53xx_device::irq_clear )
        void irq_clear(object ptr, int param)
        {
            m_cpu.target.set_input_line(0, CLEAR_LINE);
        }
    }
}
