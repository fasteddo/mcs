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
    public class namco_51xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NAMCO_51XX, namco_51xx_device, "namco51", "Namco 51xx")
        static device_t device_creator_namco_51xx_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new namco_51xx_device(mconfig, tag, owner, clock); }
        public static readonly device_type NAMCO_51XX = DEFINE_DEVICE_TYPE(device_creator_namco_51xx_device, "namco51", "Namco 51xx");


        const bool VERBOSE = false;


        //ROM_START( namco_51xx )
        static readonly List<tiny_rom_entry> rom_namco_51xx = new List<tiny_rom_entry>()
        {
            ROM_REGION( 0x400, "mcu", 0 ),
            ROM_LOAD( "51xx.bin",     0x0000, 0x0400, CRC("c2f57ef8") + SHA1("50de79e0d6a76bda95ffb02fcce369a79e6abfec") ),
            ROM_END,
        };


        // internal state
        required_device<mb88_cpu_device> m_cpu;
        uint8_t m_portO;
        uint8_t m_rw;
        devcb_read8.array<devcb_read8> m_in;
        devcb_write8 m_out;
        devcb_write_line m_lockout;


        namco_51xx_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, NAMCO_51XX, tag, owner, clock)
        {
            m_cpu = new required_device<mb88_cpu_device>(this, "mcu");
            m_portO = 0;
            m_rw = 0;
            m_in = new devcb_read8.array<devcb_read8>(4, this, () => { return new devcb_read8(this); });
            m_out = new devcb_write8(this);
            m_lockout = new devcb_write_line(this);
        }


        public devcb_read.binder input_callback(int N) { return m_in[N].bind(); }  //template <unsigned N> auto input_callback() { return m_in[N].bind(); }
        public devcb_write.binder output_callback() { return m_out.bind(); }  //auto output_callback() { return m_out.bind(); }
        public devcb_write.binder lockout_callback() { return m_lockout.bind(); }  //auto lockout_callback() { return m_lockout.bind(); }


        //WRITE_LINE_MEMBER( namco_51xx_device::reset )
        public void reset(int state)
        {
            // Reset line is active low.
            m_cpu.target.set_input_line(device_execute_interface.INPUT_LINE_RESET, state == 0 ? 1 : 0);
        }


        //WRITE_LINE_MEMBER( namco_51xx_device::vblank )
        public void vblank(int state)
        {
            // The timer is active on falling edges.
            m_cpu.target.clock_w(state == 0 ? 1 : 0);
        }


        //WRITE_LINE_MEMBER(namco_51xx_device::rw)
        public void rw(int state)
        {
            machine().scheduler().synchronize(rw_sync, state);
        }


        //WRITE_LINE_MEMBER( namco_51xx_device::chip_select )
        public void chip_select(int state)
        {
            m_cpu.target.set_input_line(0, state);
        }


        public void write(uint8_t data)
        {
            machine().scheduler().synchronize(write_sync, data);
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
            /* resolve our read callbacks */
            m_in.resolve_all_safe(0);

            /* resolve our write callbacks */
            m_out.resolve_safe();
            m_lockout.resolve_safe();

            save_item(NAME(new { m_portO }));
            save_item(NAME(new { m_rw }));
        }

        //-------------------------------------------------
        //  device_rom_region - return a pointer to the
        //  the device's ROM definitions
        //-------------------------------------------------
        protected override List<tiny_rom_entry> device_rom_region()
        {
            return rom_namco_51xx;
        }

        //-------------------------------------------------
        //  device_add_mconfig - add device configuration
        //-------------------------------------------------
        protected override void device_add_mconfig(machine_config config)
        {
            MB8843(config, m_cpu, DERIVED_CLOCK(1,1));     /* parent clock, internally divided by 6 */
            m_cpu.target.read_k().set(K_r).reg();
            m_cpu.target.read_r(0).set(R0_r).reg();
            m_cpu.target.read_r(1).set(R1_r).reg();
            m_cpu.target.read_r(2).set(R2_r).reg();
            m_cpu.target.read_r(3).set(R3_r).reg();
            m_cpu.target.write_o().set(O_w).reg();
        }


        uint8_t K_r()
        {
            return (uint8_t)((m_rw << 3) | (m_portO & 0x07));
        }


        uint8_t R0_r()
        {
            return m_in[0].op();
        }

        uint8_t R1_r()
        {
            return m_in[1].op();
        }

        uint8_t R2_r()
        {
            return m_in[2].op();
        }

        uint8_t R3_r()
        {
            return m_in[3].op();
        }

        void O_w(uint8_t data)
        {
            uint8_t out_ = (uint8_t)(data & 0x0f);
            if ((data & 0x10) != 0)
                m_portO = (uint8_t)((m_portO & 0x0f) | (out_ << 4));
            else
                m_portO = (uint8_t)((m_portO & 0xf0) | (out_));
        }


        //TIMER_CALLBACK_MEMBER( namco_51xx_device::rw_sync )
        void rw_sync(object ptr, int param)
        {
            m_rw = (uint8_t)param;
        }


        //TIMER_CALLBACK_MEMBER( namco_51xx_device::write_sync )
        void write_sync(object ptr, int param)
        {
            m_portO = (uint8_t)param;
        }
    }
}
