// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using devcb_read8 = mame.devcb_read<System.Byte, System.Byte, mame.devcb_operators_u8_u8, mame.devcb_operators_u8_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<System.Byte, System.Byte, mame.devcb_operators_u8_u8, mame.devcb_operators_u8_u8>;  //using devcb_write8 = devcb_write<u8>;
using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;


namespace mame
{
    public class namco_53xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NAMCO_53XX, namco_53xx_device, "namco53", "Namco 53xx")
        static device_t device_creator_namco_53xx_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new namco_53xx_device(mconfig, tag, owner, clock); }
        public static readonly device_type NAMCO_53XX = DEFINE_DEVICE_TYPE(device_creator_namco_53xx_device, "namco53", "Namco 53xx");


        //ROM_START( namco_53xx )
        static readonly MemoryContainer<tiny_rom_entry> rom_namco_53xx = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x400, "mcu", 0 ),
            ROM_LOAD( "53xx.bin",     0x0000, 0x0400, CRC("b326fecb") + SHA1("758d8583d658e4f1df93184009d86c3eb8713899") ),
            ROM_END,
        };


        // internal state
        required_device<mb88_cpu_device> m_cpu;
        byte m_portO;
        devcb_read8 m_k;
        devcb_read8.array<uint32_constant_4> m_in;
        devcb_write8 m_p;


        namco_53xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_53XX, tag, owner, clock)
        {
            m_cpu = new required_device<mb88_cpu_device>(this, "mcu");
            m_portO = 0;
            m_k = new devcb_read8(this);
            m_in = new devcb_read8.array<uint32_constant_4>(this, () => { return new devcb_read8(this); });
            m_p = new devcb_write8(this);
        }


        public devcb_read8.binder input_callback(UInt32 N) { return m_in[N].bind(); }  //template <unsigned N> auto input_callback() { return m_in[N].bind(); }

        public devcb_read8.binder k_port_callback() { return m_k.bind(); }  //auto k_port_callback() { return m_k.bind(); }
        //auto p_port_callback() { return m_p.bind(); }


        uint8_t K_r()
        {
            return m_k.op(0);
        }

        uint8_t R0_r()
        {
            return m_in[0].op(0);
        }

        uint8_t R1_r()
        {
            return m_in[1].op(0);
        }

        uint8_t R2_r()
        {
            return m_in[2].op(0);
        }

        uint8_t R3_r()
        {
            return m_in[3].op(0);
        }

        void O_w(uint8_t data)
        {
            uint8_t out_value = (uint8_t)(data & 0x0f);
            if ((data & 0x10) != 0)
                m_portO = (uint8_t)((m_portO & 0x0f) | (out_value << 4));
            else
                m_portO = (uint8_t)((m_portO & 0xf0) | (out_value));
        }

        void P_w(uint8_t data)
        {
            m_p.op(0, data);
        }


        //WRITE_LINE_MEMBER( namco_53xx_device::reset )
        public void reset(int state)
        {
            // The incoming signal is active low
            m_cpu.op[0].set_input_line(g.INPUT_LINE_RESET, state == 0 ? 1 : 0);
        }


        //WRITE_LINE_MEMBER(namco_53xx_device::chip_select)
        public void chip_select(int state)
        {
            m_cpu.op[0].set_input_line(0, state);
        }


        public uint8_t read()
        {
            return m_portO;
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            /* resolve our read/write callbacks */
            m_k.resolve_safe(0);
            m_in.resolve_all_safe(0);
            m_p.resolve_safe();

            save_item(NAME(new { m_portO }));
        }

        //-------------------------------------------------
        //  device_rom_region - return a pointer to the
        //  the device's ROM definitions
        //-------------------------------------------------
        protected override Pointer<tiny_rom_entry> device_rom_region()
        {
            return new Pointer<tiny_rom_entry>(rom_namco_53xx);
        }


        //-------------------------------------------------
        //  device_add_mconfig - add device configuration
        //-------------------------------------------------
        protected override void device_add_mconfig(machine_config config)
        {
            MB8843(config, m_cpu, g.DERIVED_CLOCK(1,1)); /* parent clock, internally divided by 6 */
            m_cpu.op[0].read_k().set(K_r).reg();
            m_cpu.op[0].write_o().set(O_w).reg();
            m_cpu.op[0].write_p().set(P_w).reg();
            m_cpu.op[0].read_r(0).set(R0_r).reg();
            m_cpu.op[0].read_r(1).set(R1_r).reg();
            m_cpu.op[0].read_r(2).set(R2_r).reg();
            m_cpu.op[0].read_r(3).set(R3_r).reg();
        }
    }
}
