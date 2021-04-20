// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    public static class namco54_global
    {
        /* discrete nodes */
        public static int NAMCO_54XX_0_DATA(int base_node) { return global_object.NODE_RELATIVE(base_node, 0); }
        public static int NAMCO_54XX_1_DATA(int base_node) { return global_object.NODE_RELATIVE(base_node, 1); }
        public static int NAMCO_54XX_2_DATA(int base_node) { return global_object.NODE_RELATIVE(base_node, 2); }
        public static int NAMCO_54XX_P_DATA(int base_node) { return global_object.NODE_RELATIVE(base_node, 3); }
    }


    public class namco_54xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NAMCO_54XX, namco_54xx_device, "namco54", "Namco 54xx")
        static device_t device_creator_namco_54xx_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new namco_54xx_device(mconfig, tag, owner, clock); }
        public static readonly device_type NAMCO_54XX = DEFINE_DEVICE_TYPE(device_creator_namco_54xx_device, "namco54", "Namco 54xx");


        //ROM_START( namco_54xx )
        static readonly List<tiny_rom_entry> rom_namco_54xx = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x400, "mcu", 0 ),
            ROM_LOAD( "54xx.bin",     0x0000, 0x0400, CRC("ee7357e0") + SHA1("01bdf984a49e8d0cc8761b2cc162fd6434d5afbe") ),

            ROM_END,
        };


        // internal state
        required_device<mb88_cpu_device> m_cpu;
        required_device<discrete_device> m_discrete;

        int m_basenode;
        byte m_latched_cmd;


        namco_54xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_54XX, tag, owner, clock)
        {
            m_cpu = new required_device<mb88_cpu_device>(this, "mcu");
            m_discrete = new required_device<discrete_device>(this, finder_base.DUMMY_TAG);
            m_basenode = 0;
            m_latched_cmd = 0;
        }


        //template <typename T> void set_discrete(T &&tag) { m_discrete.set_tag(std::forward<T>(tag)); }
        public void set_discrete(string tag) { m_discrete.set_tag(tag); }
        public void set_basenote(int node) { m_basenode = node; }


        //READ8_MEMBER( namco_54xx_device::K_r )
        u8 K_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return (u8)(m_latched_cmd >> 4);
        }

        //READ8_MEMBER( namco_54xx_device::R0_r )
        u8 R0_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            return (u8)(m_latched_cmd & 0x0f);
        }

        //WRITE8_MEMBER( namco_54xx_device::O_w )
        void O_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            byte out_ = (byte)(data & 0x0f);
            if ((data & 0x10) != 0)
                m_discrete.target.write((offs_t)namco54_global.NAMCO_54XX_1_DATA(m_basenode), out_);
            else
                m_discrete.target.write((offs_t)namco54_global.NAMCO_54XX_0_DATA(m_basenode), out_);
        }

        //WRITE8_MEMBER( namco_54xx_device::R1_w )
        void R1_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            byte out_ = (byte)(data & 0x0f);

            m_discrete.target.write((offs_t)namco54_global.NAMCO_54XX_2_DATA(m_basenode), out_);
        }

        //WRITE8_MEMBER( namco_54xx_device::write )
        public void write(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            machine().scheduler().synchronize(latch_callback, data);  //timer_expired_delegate(FUNC(namco_54xx_device::latch_callback),this), data);

            m_cpu.target.set_input_line(0, ASSERT_LINE);

            // The execution time of one instruction is ~4us, so we must make sure to
            // give the cpu time to poll the /IRQ input before we clear it.
            // The input clock to the 06XX interface chip is 64H, that is
            // 18432000/6/64 = 48kHz, so it makes sense for the irq line to be
            // asserted for one clock cycle ~= 21us.
            machine().scheduler().timer_set(attotime.from_usec(21), irq_clear);  //timer_expired_delegate(FUNC(namco_54xx_device::irq_clear),this), 0);
        }


        // device-level overrides

        //-------------------------------------------------
        //  device_start - device-specific startup
        //-------------------------------------------------
        protected override void device_start()
        {
            save_item(NAME(new { m_latched_cmd }));
        }

        //-------------------------------------------------
        //  device_rom_region - return a pointer to the
        //  the device's ROM definitions
        //-------------------------------------------------
        protected override List<tiny_rom_entry> device_rom_region()
        {
            return rom_namco_54xx;
        }


        //-------------------------------------------------
        // device_add_mconfig - add device configuration
        //-------------------------------------------------
        protected override void device_add_mconfig(machine_config config)
        {
            MB8844(config, m_cpu, DERIVED_CLOCK(1,1)); /* parent clock, internally divided by 6 */
            m_cpu.target.read_k().set(K_r).reg();
            m_cpu.target.write_o().set(O_w).reg();
            m_cpu.target.read_r(0).set(R0_r).reg();
            m_cpu.target.write_r(1).set(R1_w).reg();
        }


        //TIMER_CALLBACK_MEMBER( namco_54xx_device::latch_callback )
        void latch_callback(object ptr, int param)
        {
            m_latched_cmd = (byte)param;
        }

        //TIMER_CALLBACK_MEMBER( namco_54xx_device::irq_clear )
        void irq_clear(object ptr, int param)
        {
            m_cpu.target.set_input_line(0, CLEAR_LINE);
        }
    }
}
