// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using attoseconds_t = System.Int64;  //typedef s64 attoseconds_t;
using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using s32 = System.Int32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.diexec_global;
using static mame.emucore_global;
using static mame.hash_global;
using static mame.mb88xx_global;
using static mame.namco52_global;
using static mame.romentry_global;


namespace mame
{
    class namco_52xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(NAMCO_52XX, namco_52xx_device, "namco52", "Namco 52xx")
        public static readonly emu.detail.device_type_impl NAMCO_52XX = DEFINE_DEVICE_TYPE("namco52", "Namco 52xx", (type, mconfig, tag, owner, clock) => { return new namco_52xx_device(mconfig, tag, owner, clock); });


        //ROM_START( namco_52xx )
        static readonly tiny_rom_entry [] rom_namco_52xx = 
        {
            ROM_REGION( 0x400, "mcu", 0 ),
            ROM_LOAD( "52xx.bin",     0x0000, 0x0400, CRC("3257d11e") + SHA1("4883b2fdbc99eb7b9906357fcc53915842c2c186") ),

            ROM_END,
        };


        // internal state
        required_device<mb88_cpu_device> m_cpu;
        required_device<discrete_device> m_discrete;

        int m_basenode;
        attoseconds_t m_extclock;
        emu_timer m_extclock_pulse_timer;
        devcb_read8 m_romread;
        devcb_read8 m_si;

        uint8_t m_latched_cmd;
        uint32_t m_address;


        namco_52xx_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, NAMCO_52XX, tag, owner, clock)
        {
            m_cpu = new required_device<mb88_cpu_device>(this, "mcu");
            m_discrete = new required_device<discrete_device>(this, finder_base.DUMMY_TAG);
            m_basenode = 0;
            m_extclock = 0;
            m_romread = new devcb_read8(this);
            m_si = new devcb_read8(this);
            m_latched_cmd = 0;
            m_address = 0;
        }


        //template <typename T>
        public void set_discrete(string tag) { m_discrete.set_tag(tag); }  //template <typename T> void set_discrete(T &&tag) { m_discrete.set_tag(std::forward<T>(tag)); }
        public void set_basenote(int node) { m_basenode = node; }  //void set_basenote(int node) { m_basenode = node; }
        //void set_extclock(attoseconds_t clk) { m_extclock = clk; }
        public devcb_read8.binder romread_callback() { return m_romread.bind(); }  //auto romread_callback() { return m_romread.bind(); }
        public devcb_read8.binder si_callback() { return m_si.bind(); }  //auto si_callback() { return m_si.bind(); }

        //DECLARE_WRITE_LINE_MEMBER( reset );
        public void reset(int state)
        {
            // The incoming signal is active low
            m_cpu.op0.set_input_line(INPUT_LINE_RESET, state == 0 ? 1 : 0);
        }

        //WRITE_LINE_MEMBER( chip_select );
        public void chip_select(int state)
        {
            m_cpu.op0.set_input_line(0, state);
        }

        public void write(uint8_t data) { throw new emu_unimplemented(); }


        // device-level overrides
        protected override void device_start()
        {
            /* resolve our read/write callbacks */
            m_romread.resolve_safe_u8(0);
            m_si.resolve_safe_u8(0);

            /* start the external clock */
            if (m_extclock != 0)
            {
                m_extclock_pulse_timer = machine().scheduler().timer_alloc(external_clock_pulse);
                m_extclock_pulse_timer.adjust(new attotime(0, m_extclock), 0, new attotime(0, m_extclock));
            }

            save_item(NAME(new { m_latched_cmd }));
            save_item(NAME(new { m_address }));
        }


        protected override Pointer<tiny_rom_entry> device_rom_region()
        {
            return new Pointer<tiny_rom_entry>(new MemoryContainer<tiny_rom_entry>(rom_namco_52xx));  //return ROM_NAME(namco_52xx );
        }


        protected override void device_add_mconfig(machine_config config)
        {
            MB8843(config, m_cpu, DERIVED_CLOCK(1,1));     /* parent clock, internally divided by 6 */
            m_cpu.op0.read_k().set(K_r).reg();
            m_cpu.op0.write_o().set(O_w).reg();
            m_cpu.op0.write_p().set(P_w).reg();
            m_cpu.op0.read_si().set(SI_r).reg();
            m_cpu.op0.read_r(0).set(R0_r).reg();
            m_cpu.op0.read_r(1).set(R1_r).reg();
            m_cpu.op0.write_r(2).set(R2_w).reg();
            m_cpu.op0.write_r(3).set(R3_w).reg();
        }


        //TIMER_CALLBACK_MEMBER( write_sync );


        //TIMER_CALLBACK_MEMBER( namco_52xx_device::external_clock_pulse )
        void external_clock_pulse(object ptr, s32 param)  //void *ptr, s32 param)
        {
            m_cpu.op0.clock_w(ASSERT_LINE);
            m_cpu.op0.clock_w(CLEAR_LINE);
        }


        uint8_t K_r() { throw new emu_unimplemented(); }
        int SI_r() { throw new emu_unimplemented(); }  //DECLARE_READ_LINE_MEMBER( SI_r );
        uint8_t R0_r() { throw new emu_unimplemented(); }
        uint8_t R1_r() { throw new emu_unimplemented(); }
        void P_w(uint8_t data) { throw new emu_unimplemented(); }
        void R2_w(uint8_t data) { throw new emu_unimplemented(); }
        void R3_w(uint8_t data) { throw new emu_unimplemented(); }
        void O_w(uint8_t data) { throw new emu_unimplemented(); }
    }


    static class namco52_global
    {
        /* discrete nodes */
        public static int NAMCO_52XX_P_DATA(int base_) { return base_; }


        public static namco_52xx_device NAMCO_52XX(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<namco_52xx_device>(mconfig, tag, namco_52xx_device.NAMCO_52XX, clock); }
    }
}
