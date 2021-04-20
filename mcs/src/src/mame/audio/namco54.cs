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
        static readonly MemoryContainer<tiny_rom_entry> rom_namco_54xx = new MemoryContainer<tiny_rom_entry>()
        {
            ROM_REGION( 0x400, "mcu", 0 ),
            ROM_LOAD( "54xx.bin",     0x0000, 0x0400, CRC("ee7357e0") + SHA1("01bdf984a49e8d0cc8761b2cc162fd6434d5afbe") ),

            ROM_END,
        };


        // internal state
        required_device<mb88_cpu_device> m_cpu;
        required_device<discrete_device> m_discrete;

        attotime m_irq_duration;
        int m_basenode;
        byte m_latched_cmd;


        namco_54xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_54XX, tag, owner, clock)
        {
            m_cpu = new required_device<mb88_cpu_device>(this, "mcu");
            m_discrete = new required_device<discrete_device>(this, finder_base.DUMMY_TAG);
            m_irq_duration = attotime.from_usec(100);
            m_basenode = 0;
            m_latched_cmd = 0;
        }


        //template <typename T> void set_discrete(T &&tag) { m_discrete.set_tag(std::forward<T>(tag)); }
        public void set_discrete(string tag) { m_discrete.set_tag(tag); }
        public void set_basenote(int node) { m_basenode = node; }

        //namco_54xx_device &set_irq_duration(attotime t) { m_irq_duration = t; return *this; }


        //WRITE_LINE_MEMBER( namco_54xx_device::reset )
        public void reset(int state)
        {
            // The incoming signal is active low
            m_cpu.target.set_input_line(device_execute_interface.INPUT_LINE_RESET, state == 0 ? 1 : 0);
        }


        //WRITE_LINE_MEMBER( chip_select );
        public void chip_select(int state)
        {
            // TODO: broken sound when using this
            //m_cpu->set_input_line(0, state);
        }


        uint8_t K_r()
        {
            return (uint8_t)(m_latched_cmd >> 4);
        }

        uint8_t R0_r()
        {
            return (uint8_t)(m_latched_cmd & 0x0f);
        }

        void O_w(uint8_t data)
        {
            uint8_t out_ = (uint8_t)(data & 0x0f);
            if ((data & 0x10) != 0)
                m_discrete.target.write((offs_t)namco54_global.NAMCO_54XX_1_DATA(m_basenode), out_);
            else
                m_discrete.target.write((offs_t)namco54_global.NAMCO_54XX_0_DATA(m_basenode), out_);
        }

        void R1_w(uint8_t data)
        {
            uint8_t out_ = (uint8_t)(data & 0x0f);

            m_discrete.target.write((offs_t)namco54_global.NAMCO_54XX_2_DATA(m_basenode), out_);
        }

        public void write(uint8_t data)
        {
            machine().scheduler().synchronize(latch_callback, data);  //timer_expired_delegate(FUNC(namco_54xx_device::latch_callback),this), data);

            // TODO: should use chip_select line for this
            m_cpu.target.pulse_input_line(0, m_irq_duration);
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
        protected override Pointer<tiny_rom_entry> device_rom_region()
        {
            return new Pointer<tiny_rom_entry>(rom_namco_54xx);
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
    }
}
