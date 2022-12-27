// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using offs_t = System.UInt32;  //using offs_t = u32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.disound_global;
using static mame.n2a03_global;
using static mame.nes_apu_global;


namespace mame
{
    public class n2a03_core_device : m6502_device
    {
        //DEFINE_DEVICE_TYPE(N2A03_CORE, n2a03_core_device, "n2a03_core", "Ricoh N2A03 core") // needed for some VT systems with XOP instead of standard APU
        public static readonly emu.detail.device_type_impl N2A03_CORE = DEFINE_DEVICE_TYPE("n2a03_core", "Ricoh N2A03 core", (type, mconfig, tag, owner, clock) => { return new n2a03_core_device(mconfig, tag, owner, clock); });


        n2a03_core_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, N2A03_CORE, tag, owner, clock)
        { }


        protected n2a03_core_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        { }


        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        protected override void do_exec_full() { throw new emu_unimplemented(); }
        protected override void do_exec_partial() { throw new emu_unimplemented(); }


        //#define O(o) void o ## _full(); void o ## _partial()

        // n2a03 opcodes - same as 6502 with D disabled
        //O(adc_nd_aba); O(adc_nd_abx); O(adc_nd_aby); O(adc_nd_idx); O(adc_nd_idy); O(adc_nd_imm); O(adc_nd_zpg); O(adc_nd_zpx);
        //O(arr_nd_imm);
        //O(isb_nd_aba); O(isb_nd_abx); O(isb_nd_aby); O(isb_nd_idx); O(isb_nd_idy); O(isb_nd_zpg); O(isb_nd_zpx);
        //O(rra_nd_aba); O(rra_nd_abx); O(rra_nd_aby); O(rra_nd_idx); O(rra_nd_idy); O(rra_nd_zpg); O(rra_nd_zpx);
        //O(sbc_nd_aba); O(sbc_nd_abx); O(sbc_nd_aby); O(sbc_nd_idx); O(sbc_nd_idy); O(sbc_nd_imm); O(sbc_nd_zpg); O(sbc_nd_zpx);

        //#undef O
    }


    public class n2a03_device : n2a03_core_device
                                //device_mixer_interface
    {
        //DEFINE_DEVICE_TYPE(N2A03, n2a03_device, "n2a03", "Ricoh N2A03")
        public static readonly emu.detail.device_type_impl N2A03 = DEFINE_DEVICE_TYPE("n2a03", "Ricoh N2A03", (type, mconfig, tag, owner, clock) => { return new n2a03_device(mconfig, tag, owner, clock); });


        device_mixer_interface m_dimixer;


        required_device<nesapu_device> m_apu;


        n2a03_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, N2A03, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_mixer_interface(mconfig, this, 1));  //, device_mixer_interface(mconfig, *this, 1)
            m_dimixer = GetClassInterface<device_mixer_interface>();


            m_apu = new required_device<nesapu_device>(this, "nesapu");


            program_config.m_internal_map = n2a03_map;
        }


        public device_mixer_interface dimixer { get { return m_dimixer; } }


        uint8_t psg1_4014_r() { throw new emu_unimplemented(); }
        uint8_t psg1_4015_r() { throw new emu_unimplemented(); }
        void psg1_4015_w(uint8_t data) { throw new emu_unimplemented(); }
        void psg1_4017_w(uint8_t data) { throw new emu_unimplemented(); }


        void n2a03_map(address_map map, device_t device)
        {
            map.op(0x4000, 0x4013).rw("nesapu", (offset) => { return ((nesapu_device)subdevice("nesapu")).read(offset); }, (offset, data) => { ((nesapu_device)subdevice("nesapu")).write(offset, data); });
            map.op(0x4014, 0x4014).r(psg1_4014_r); // .w(FUNC(nesapu_device::sprite_dma_0_w));
            map.op(0x4015, 0x4015).rw(psg1_4015_r, psg1_4015_w); /* PSG status / first control register */
            //map(0x4016, 0x4016).rw(FUNC(n2a03_device::vsnes_in0_r), FUNC(n2a03_device::vsnes_in0_w));
            map.op(0x4017, 0x4017) /*.r(FUNC(n2a03_device::vsnes_in1_r))*/ .w(psg1_4017_w);
        }


        protected override void device_add_mconfig(machine_config config)
        {
            NES_APU(config, m_apu, DERIVED_CLOCK(1,1));
            m_apu.op0.irq().set((write_line_delegate)apu_irq).reg();
            m_apu.op0.mem_read().set(apu_read_mem).reg();
            m_apu.op0.disound.add_route(ALL_OUTPUTS, this.m_dimixer, 1.0, AUTO_ALLOC_INPUT, 0);
        }


        //DECLARE_WRITE_LINE_MEMBER(apu_irq);
        public void apu_irq(int state) { throw new emu_unimplemented(); }


        uint8_t apu_read_mem(offs_t offset) { throw new emu_unimplemented(); }
    }


    static class n2a03_global
    {
        /* These are the official XTAL values and clock rates used by Nintendo for
           manufacturing throughout the production of the 2A03. PALC_APU_CLOCK is
           the clock rate devised by UMC(?) for PAL Famicom clone hardware.        */

        static readonly XTAL N2A03_NTSC_XTAL        = new XTAL(21_477_272);
        //#define N2A03_PAL_XTAL            XTAL(26'601'712)
        public static readonly XTAL NTSC_APU_CLOCK  = N2A03_NTSC_XTAL / 12;  /* 1.7897726666... MHz */
        //#define PAL_APU_CLOCK       (N2A03_PAL_XTAL/16) /* 1.662607 MHz */
        //#define PALC_APU_CLOCK      (N2A03_PAL_XTAL/15) /* 1.77344746666... MHz */


        //enum {
        //    N2A03_IRQ_LINE = m6502_device::IRQ_LINE,
        //    N2A03_APU_IRQ_LINE = m6502_device::APU_IRQ_LINE,
        //    N2A03_NMI_LINE = m6502_device::NMI_LINE,
        //    N2A03_SET_OVERFLOW = m6502_device::V_LINE
        //};


        public static n2a03_device N2A03(machine_config mconfig, string tag, XTAL clock) { return emu.detail.device_type_impl.op<n2a03_device>(mconfig, tag, n2a03_device.N2A03, clock); }
    }
}
