// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint16_t = System.UInt16;


namespace mame
{
    static class m68705_global
    {
        public const u16 M68705_VECTOR_BOOTSTRAP   = 0xfff6;
        public const u16 M68705_VECTOR_TIMER       = 0xfff8;
        //constexpr u16 M68705_VECTOR_INT2        = 0xfff8;
        public const u16 M68705_VECTOR_INT         = 0xfffa;
        public const u16 M68705_VECTOR_SWI         = 0xfffc;
        public const u16 M68705_VECTOR_RESET       = 0xfffe;

        public const u16 M68705_INT_MASK           = 0x03;


        /****************************************************************************
         * 68705 section
         ****************************************************************************/
        public const int M68705_IRQ_LINE   = m6805_global.M6805_IRQ_LINE + 0;
        public const int M68705_INT_TIMER  = m6805_global.M6805_IRQ_LINE + 1;
        public const int M68705_VPP_LINE   = m6805_global.M6805_IRQ_LINE + 2;
        public const int M68705_VIHTP_LINE = m6805_global.M6805_IRQ_LINE + 3;
    }


    // ======================> m68705_device
    public abstract class m68705_device : m6805_base_device
                                          //device_nvram_interface
    {
        class device_execute_interface_m68705 : device_execute_interface_m6805_base
        {
            public device_execute_interface_m68705(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void execute_set_input(int inputnum, int state) { ((m68705_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
        }


        class device_nvram_interface_m68705 : device_nvram_interface
        {
            public device_nvram_interface_m68705(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void nvram_default() { }
            protected override void nvram_read(emu_file file) { throw new emu_unimplemented(); }
            protected override void nvram_write(emu_file file) { throw new emu_unimplemented(); }
        }


        //enum
        //{
        const int PORT_COUNT = 4;
        //};


        /****************************************************************************
         * Configurable logging
         ****************************************************************************/

        //#define LOG_GENERAL (1U <<  0)
        const int LOG_INT    = 1 << 1;
        const int LOG_IOPORT = 1 << 2;
        const int LOG_TIMER  = 1 << 3;
        const int LOG_EPROM  = 1 << 4;

        //#define VERBOSE (LOG_GENERAL | LOG_IOPORT | LOG_TIMER | LOG_EPROM)
        //#define LOG_OUTPUT_FUNC printf
        //#include "logmacro.h"

        public void LOGINT(string format, params object [] args) { LOGMASKED(LOG_INT, format, args); }
        public void LOGIOPORT(string format, params object [] args) { LOGMASKED(LOG_IOPORT, format, args); }
        public void LOGTIMER(string format, params object [] args) { LOGMASKED(LOG_TIMER, format, args); }
        public void LOGEPROM(string format, params object [] args) { LOGMASKED(LOG_EPROM, format, args); }


        device_execute_interface_m68705 m_diexecute;
        device_nvram_interface_m68705 m_nvram_interface;


        required_region_ptr_uint8_t m_user_rom;

        // digital I/O
        bool [] m_port_open_drain;
        u8 [] m_port_mask;
        u8 [] m_port_input;
        u8 [] m_port_latch;
        u8 [] m_port_ddr;
        devcb_read8 [] m_port_cb_r;
        devcb_write8 [] m_port_cb_w;

        // timer/counter
        u8 m_prescaler;
        u8 m_tdr;
        u8 m_tcr;

        // EPROM control
        u8 m_vihtp;
        u8 m_pcr;
        u8 m_pl_data;
        u16 m_pl_addr;


        protected m68705_device(
                machine_config mconfig,
                string tag,
                device_t owner,
                u32 clock,
                device_type type,
                u32 addr_width,
                address_map_constructor internal_map)
            : base(
                    mconfig,
                    tag,
                    owner,
                    clock,
                    type,
                    new configuration_params(s_hmos_ops, s_hmos_cycles, addr_width, 0x007f, 0x0060, m68705_global.M68705_VECTOR_SWI),
                    internal_map)
        {
            m_class_interfaces.Add(new device_execute_interface_m68705(mconfig, this));
            m_class_interfaces.Add(new device_nvram_interface_m68705(mconfig, this));  //device_nvram_interface(mconfig, *this)

            m_diexecute = GetClassInterface<device_execute_interface_m68705>();
            m_nvram_interface = GetClassInterface<device_nvram_interface_m68705>();


            m_user_rom = new required_region_ptr_uint8_t(this, DEVICE_SELF, 1U << (int)addr_width);
            m_port_open_drain = new bool [PORT_COUNT] { false, false, false, false };
            m_port_mask = new u8 [PORT_COUNT] { 0x00, 0x00, 0x00, 0x00 };
            m_port_input = new u8 [PORT_COUNT] { 0xff, 0xff, 0xff, 0xff };
            m_port_latch = new u8 [PORT_COUNT] { 0xff, 0xff, 0xff, 0xff };
            m_port_ddr = new u8 [PORT_COUNT] { 0x00, 0x00, 0x00, 0x00 };
            m_port_cb_r = new devcb_read8 [PORT_COUNT] { new devcb_read8(this), new devcb_read8(this), new devcb_read8(this), new devcb_read8(this) };
            m_port_cb_w = new devcb_write8 [PORT_COUNT] { new devcb_write8(this), new devcb_write8(this), new devcb_write8(this), new devcb_write8(this) };
            m_prescaler = 0x7f;
            m_tdr = 0xff;
            m_tcr = 0x7f;
            m_vihtp = CLEAR_LINE;
            m_pcr = 0xff;
            m_pl_data = 0xff;
            m_pl_addr = 0xffff;
        }


        protected override void device_start()
        {
            base.device_start();

            save_item(m_port_input, "m_port_input");
            save_item(m_port_latch, "m_port_latch");
            save_item(m_port_ddr, "m_port_ddr");

            save_item(m_prescaler, "m_prescaler");
            save_item(m_tdr, "m_tdr");
            save_item(m_tcr, "m_tcr");

            save_item(m_vihtp, "m_vihtp");
            save_item(m_pcr, "m_pcr");
            save_item(m_pl_data, "m_pl_data");
            save_item(m_pl_addr, "m_pl_addr");

            // initialise digital I/O
            for (int i = 0; i < m_port_input.Length; i++) m_port_input[i] = 0xff;  //for (u8 &input : m_port_input) input = 0xff;
            foreach (devcb_read8 cb in m_port_cb_r) cb.resolve();
            foreach (devcb_write8 cb in m_port_cb_w) cb.resolve_safe();

            // initialise timer/counter
            u8 options = get_mask_options();
            m_tcr = (u8)(0x40 | (options & 0x37));
            if (BIT(options, 6) != 0)
                m_tcr |= 0x18;

            // initialise EPROM control
            m_vihtp = CLEAR_LINE;
            m_pcr = 0xff;
            m_pl_data = 0xff;
            m_pl_addr = 0xffff;
        }


        protected override void device_reset()
        {
            base.device_reset();

            // reset digital I/O
            port_ddr_w_0(memory().space(AS_PROGRAM), 0, 0x00, 0xff);
            port_ddr_w_1(memory().space(AS_PROGRAM), 0, 0x00, 0xff);
            port_ddr_w_2(memory().space(AS_PROGRAM), 0, 0x00, 0xff);
            port_ddr_w_3(memory().space(AS_PROGRAM), 0, 0x00, 0xff);

            // reset timer/counter
            u8 options = get_mask_options();
            m_prescaler = 0x7f;
            m_tdr = 0xff;
            m_tcr = BIT(options, 6) != 0 ? (u8)(0x58 | (options & 0x27)) : (u8)(0x40 | (m_tcr & 0x37));

            // reset EPROM control
            m_pcr |= 0xfb; // b2 (/VPON) is driven by external input and hence unaffected by reset

            if (CLEAR_LINE != m_vihtp)
            {
                LOG("loading bootstrap vector\n");
                rm16(m68705_global.M68705_VECTOR_BOOTSTRAP, ref m_pc);
            }
            else
            {
                LOG("loading reset vector\n");
                rm16(m68705_global.M68705_VECTOR_RESET, ref m_pc);
            }
        }


        // device_execute_interface overrides
        void device_execute_interface_execute_set_input(int inputnum, int state)
        {
            switch (inputnum)
            {
                case m68705_global.M68705_VPP_LINE:
                    m_pcr = (u8)((m_pcr & 0xfb) | (ASSERT_LINE == state ? 0x00 : 0x04));
                    break;
                case m68705_global.M68705_VIHTP_LINE:
                    // TODO: this is actually the same physical pin as the timer input, so they should be tied up
                    m_vihtp = ASSERT_LINE == state ? (u8)ASSERT_LINE : (u8)CLEAR_LINE;
                    break;
                default:
                    if (irq_state[inputnum] != state)
                    {
                        irq_state[inputnum] = state == ASSERT_LINE ? ASSERT_LINE : CLEAR_LINE;

                        if (state != CLEAR_LINE)
                            pending_interrupts |= (uint16_t)(1 << inputnum);
                        else if (m68705_global.M68705_INT_TIMER == inputnum)
                            pending_interrupts = (uint16_t)(pending_interrupts & ~(1 << inputnum)); // this one's is level-sensitive
                    }
                    break;
            }
        }


        // device_nvram_interface overrides
        //virtual void nvram_default() override;
        //virtual void nvram_read(emu_file &file) override;
        //virtual void nvram_write(emu_file &file) override;


        protected override void interrupt()
        {
            if ((pending_interrupts & m68705_global.M68705_INT_MASK) != 0)
            {
                if ((CC & IFLAG) == 0)
                {
                    pushword(m_pc);
                    pushbyte(m_x);
                    pushbyte(m_a);
                    pushbyte(m_cc);
                    SEI();
                    standard_irq_callback(0);

                    if (BIT(pending_interrupts, m68705_global.M68705_IRQ_LINE) != 0)
                    {
                        LOGINT("servicing /INT interrupt\n");
                        pending_interrupts = (uint16_t)(pending_interrupts & ~(1 << m68705_global.M68705_IRQ_LINE));
                        rm16(m68705_global.M68705_VECTOR_INT, ref m_pc);
                    }
                    else if (BIT(pending_interrupts, m68705_global.M68705_INT_TIMER) != 0)
                    {
                        LOGINT("servicing timer/counter interrupt\n");
                        rm16(m68705_global.M68705_VECTOR_TIMER, ref m_pc);
                    }
                    else
                    {
                        throw new emu_fatalerror("Unknown pending interrupt");
                    }

                    m_icountRef.i -= 11;
                    burn_cycles(11);
                }
            }
        }


        protected override void burn_cycles(UInt32 count)
        {
            // handle internal timer/counter source
            if (!tcr_tin()) // TODO: check tcr_tie() and gate on TIMER if appropriate
            {
                int ps_opt = tcr_ps();
                UInt32 ps_mask = (UInt32)((1 << ps_opt) - 1);
                UInt32 decrements = (count + (m_prescaler & ps_mask)) >> ps_opt;

                if ((m_tdr != 0 ? (UInt32)m_tdr : 256U) <= decrements)
                {
                    LOGTIMER("timer/counter expired{0}{1}\n", tcr_tir() ? " [overrun]" : "", tcr_tim() ? " [masked]" : "");
                    m_tcr |= 0x80;
                    if (!tcr_tim())
                        set_input_line(m68705_global.M68705_INT_TIMER, ASSERT_LINE);
                }

                m_prescaler = (u8)((count + m_prescaler) & 0x7f);
                m_tdr = (u8)((m_tdr - decrements) & 0xff);
            }
        }


        public devcb_read.binder porta_r() { return m_port_cb_r[0].bind(); }
        //devcb_read.binder portb_r() { throw new emu_unimplemented(); } //return m_port_cb_r[1].bind(); }
        public devcb_read.binder portc_r() { return m_port_cb_r[2].bind(); }
        //devcb_read.binder portd_r() { throw new emu_unimplemented(); } //return m_port_cb_r[3].bind(); }
        public devcb_write.binder porta_w() { return m_port_cb_w[0].bind(); }
        public devcb_write.binder portb_w() { return m_port_cb_w[1].bind(); }
        //devcb_write.binder portc_w() { throw new emu_unimplemented(); } //return m_port_cb_w[2].bind(); }


        //template <offs_t B> READ8_MEMBER(m68705_device::eprom_r)
        u8 eprom_r(offs_t B, address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            if (pcr_vpon() && !pcr_ple())
                LOGEPROM("read EPROM {0} prevented when Vpp high and /PLE = 0\n", B + offset);

            // read locked out when /VPON and /PLE are asserted
            return (!pcr_vpon() || !pcr_ple()) ? m_user_rom.target[B + offset] : (u8)0xff;
        }

        protected u8 eprom_r_0x0080(address_space space, offs_t offset, u8 mem_mask = 0xff) { return eprom_r(0x0080, space, offset, mem_mask); }
        protected u8 eprom_r_0x07f8(address_space space, offs_t offset, u8 mem_mask = 0xff) { return eprom_r(0x07f8, space, offset, mem_mask); }


        //template <offs_t B> WRITE8_MEMBER(m68705_device::eprom_w)
        void eprom_w(offs_t B, address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
        }

        protected void eprom_w_0x0080(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { eprom_w(0x0080, space, offset, data, mem_mask); }
        protected void eprom_w_0x07f8(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { eprom_w(0x07f8, space, offset, data, mem_mask); }


        protected void set_port_open_drain(int N, bool value) { m_port_open_drain[N] = value; }  //template <std::size_t N> void set_port_open_drain(bool value);

        //template <std::size_t N>
        protected void set_port_mask(int N, u8 mask)
        {
            if (configured() || started())
                throw new emu_fatalerror("Attempt to set physical port mask after configuration");

            m_port_mask[N] = mask;
        }


        //template <std::size_t N> READ8_MEMBER(m68705_device::port_r)
        u8 port_r(int N, address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            if (!m_port_cb_r[N].isnull())
            {
                u8 newval = (u8)(m_port_cb_r[N].op(space, 0, (u8)(~m_port_ddr[N] & ~m_port_mask[N])) & ~m_port_mask[N]);
                if (newval != m_port_input[N])
                {
                    LOGIOPORT("read PORT{0}: new input = {1} & {2} (was {3})\n",
                            'A' + N, newval, ~m_port_ddr[N] & ~m_port_mask[N], m_port_input[N]);
                }

                m_port_input[N] = newval;
            }

            return (u8)(m_port_mask[N] | (m_port_latch[N] & m_port_ddr[N]) | (m_port_input[N] & ~m_port_ddr[N]));
        }

        protected u8 port_r_0(address_space space, offs_t offset, u8 mem_mask = 0xff) { return port_r(0, space, offset, mem_mask); }
        protected u8 port_r_1(address_space space, offs_t offset, u8 mem_mask = 0xff) { return port_r(1, space, offset, mem_mask); }
        protected u8 port_r_2(address_space space, offs_t offset, u8 mem_mask = 0xff) { return port_r(2, space, offset, mem_mask); }


        //template <std::size_t N> WRITE8_MEMBER(m68705_device::port_latch_w)
        void port_latch_w(int N, address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            data = (u8)(data & ~m_port_mask[N]);
            u8 diff = (u8)(m_port_latch[N] ^ data);
            if (diff != 0)
                LOGIOPORT("write PORT{0} latch: {1} & {2} (was {3})\n", 'A' + N, data, m_port_ddr[N], m_port_latch[N]);
            m_port_latch[N] = data;
            if ((diff & m_port_ddr[N]) != 0)
                port_cb_w(N);
        }

        protected void port_latch_w_0(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { port_latch_w(0, space, offset, data, mem_mask); }
        protected void port_latch_w_1(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { port_latch_w(1, space, offset, data, mem_mask); }
        protected void port_latch_w_2(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { port_latch_w(2, space, offset, data, mem_mask); }


        //template <std::size_t N> WRITE8_MEMBER(m68705_device::port_ddr_w)
        void port_ddr_w(int N, address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            data = (u8)(data & ~m_port_mask[N]);
            if (data != m_port_ddr[N])
            {
                LOGIOPORT("write DDR{0}: {1} (was {2})\n", 'A' + N, data, m_port_ddr[N]);
                m_port_ddr[N] = data;
                port_cb_w(N);
            }
        }

        protected void port_ddr_w_0(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { port_ddr_w(0, space, offset, data, mem_mask); }
        protected void port_ddr_w_1(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { port_ddr_w(1, space, offset, data, mem_mask); }
        protected void port_ddr_w_2(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { port_ddr_w(2, space, offset, data, mem_mask); }
        protected void port_ddr_w_3(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { port_ddr_w(3, space, offset, data, mem_mask); }


        //template <std::size_t N> void port_cb_w();
        void port_cb_w(int N)
        {
            u8 data = m_port_open_drain[N] ? (u8)(m_port_latch[N] | ~m_port_ddr[N]) : m_port_latch[N];
            u8 mask = m_port_open_drain[N] ? (u8)(~m_port_latch[N] & m_port_ddr[N]) : m_port_ddr[N];
            m_port_cb_w[N].op(memory().space(AS_PROGRAM), 0, data, mask);
        }


        //READ8_MEMBER(m68705_device::tdr_r)
        protected u8 tdr_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
        }


        //WRITE8_MEMBER(m68705_device::tdr_w)
        protected void tdr_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            LOGTIMER("write TDR: {0} * (1 << {1})\n", data, tcr_ps());
            m_tdr = data;
        }


        //READ8_MEMBER(m68705_device::tcr_r)
        protected u8 tcr_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
        }


        //WRITE8_MEMBER(m68705_device::tcr_w)
        protected void tcr_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            // 7  TIR   RW  Timer Interrupt Request Status
            // 6  TIM   RW  Timer Interrupt Mask
            // 5  TIN   RW  Timer Input Select
            // 4  TIE   RW  Timer External Input Enable
            // 3  TOPT  R   Timer Mask/Programmable Option
            // 3  PSC    W  Prescaler Clear
            // 2  PS2   RW  Prescaler Option
            // 1  PS1   RW  Prescaler Option
            // 0  PS0   RW  Prescaler Option

            // TIN  TIE  CLOCK
            //  0    0   Internal Clock (phase 2)
            //  0    1   Gated (AND) of External and Internal Clocks
            //  1    0   No Clock
            //  1    1   External Clock

            // in MOR controlled mode, TIN/PS2/PS1/PS0 are loaded from MOR on reset and TIE is always 1
            // in MOR controlled mode, TIN, TIE, PS2, PS1, and PS0 always read as 1

            // TOPT isn't a real bit in this register, it's a pass-through to the MOR register
            // it's theoretically possible to get into a weird state by writing to the MOR while running
            // for simplicity, we don't emulate this - we just check the MOR on initialisation and reset

            if (tcr_topt())
            {
                LOGTIMER("write TCR: TIR={0} ({1}) TIM={2}\n",
                        BIT(data, 7), (tcr_tir() && BIT(data, 7) == 0) ? "cleared" : "unaffected", BIT(data, 6));

                m_tcr = (u8)((m_tcr & ((data & 0x80) | 0x3f)) | (data & 0x40));
            }
            else
            {
                LOGTIMER("write TCR: TIR={0} ({1}) TIM={2} TIN={3} TIE={4} PSC={5} PS=(1 << {6})\n",
                        BIT(data, 7), (tcr_tir() && BIT(data, 7) == 0) ? "cleared" : "unaffected", BIT(data, 6),
                        BIT(data, 5), BIT(data, 4),
                        BIT(data, 3), data & 0x07);

                if (BIT(data, 3) != 0)
                    m_prescaler = 0;

                m_tcr = (u8)((m_tcr & ((data & 0x80) | 0x08)) | (data & 0x77));
            }

            // this is a level-sensitive interrupt (unlike the edge-triggered inputs)
            device_execute_interface_execute_set_input(m68705_global.M68705_INT_TIMER, (tcr_tir() && !tcr_tim()) ? ASSERT_LINE : CLEAR_LINE);
        }


        //READ8_MEMBER(m68705_device::pcr_r)
        protected u8 pcr_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
        }


        //WRITE8_MEMBER(m68705_device::pcr_w)
        protected void pcr_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
        }


        protected ListPointer<u8> get_user_rom() { return m_user_rom.target; }  //u8 *const get_user_rom() const { return &m_user_rom[0]; }
        protected abstract u8 get_mask_options();


        //template <std::size_t N>
        protected void add_port_latch_state(int N)
        {
            //throw new emu_unimplemented();
#if false
            state_add(M68705_LATCHA + N, util::string_format("LATCH%c", 'A' + N).c_str(), m_port_latch[N]).mask(~m_port_mask[N] & 0xff);
#endif
        }

        //template <std::size_t N>
        protected void add_port_ddr_state(int N)
        {
            //throw new emu_unimplemented();
#if false
            state_add(M68705_DDRA + N, util::string_format("DDR%c", 'A' + N).c_str(), m_port_ddr[N]).mask(~m_port_mask[N] & 0xff);
#endif
        }


        protected void add_timer_state()
        {
            //throw new emu_unimplemented();
#if false
            state_add(M68705_PS, "PS", m_prescaler).mask(0x7f);
            state_add(M68705_TDR, "TDR", m_tdr).mask(0xff);
            state_add(M68705_TCR, "TCR", m_tcr).mask(0xff);
#endif
        }


        protected void add_eprom_state()
        {
            //throw new emu_unimplemented();
#if false
            state_add(M68705_PCR, "PCR", m_pcr).mask(0xff);
            state_add(M68705_PLA, "PLA", m_pl_addr).mask(0xffff);
            state_add(M68705_PLD, "PLD", m_pl_data).mask(0xff);
#endif
        }


        bool tcr_tir() { return BIT(m_tcr, 7) != 0; }
        bool tcr_tim() { return BIT(m_tcr, 6) != 0; }
        bool tcr_tin() { return BIT(m_tcr, 5) != 0; }
        bool tcr_tie() { return BIT(m_tcr, 4) != 0; }
        bool tcr_topt() { return BIT(m_tcr, 3) != 0; }
        u8 tcr_ps() { return (u8)(m_tcr & 0x07); }


        bool pcr_vpon() { return BIT(m_pcr, 2) == 0; }
        bool pcr_pge()  { return BIT(m_pcr, 1) == 0; }
        bool pcr_ple()  { return BIT(m_pcr, 0) == 0; }
    }


    // ======================> m68705p_device
    public abstract class m68705p_device : m68705_device
    {
        protected class device_disasm_interface_m68705p : device_disasm_interface_m6805_base
        {
            public device_disasm_interface_m68705p(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        protected m68705p_device(
                machine_config mconfig,
                string tag,
                device_t owner,
                u32 clock,
                device_type type)
            : base(mconfig, tag, owner, clock, type, 11, p_map)
        {
            m_class_interfaces.Add(new device_disasm_interface_m68705p(mconfig, this));


            set_port_open_drain(0, true);   // Port A is open drain with internal pull-ups
            set_port_mask(2, 0xf0);         // Port C is four bits wide
            set_port_mask(3, 0xff);         // Port D isn't present
        }


        //DECLARE_WRITE8_MEMBER(pa_w) { port_input_w<0>(space, offset, data, mem_mask); }
        //DECLARE_WRITE8_MEMBER(pb_w) { port_input_w<1>(space, offset, data, mem_mask); }
        //DECLARE_WRITE8_MEMBER(pc_w) { port_input_w<2>(space, offset, data, mem_mask); }


        static void p_map(address_map map, device_t owner)
        {
            m68705p_device m68705p = (m68705p_device)owner;

            map.global_mask(0x07ff);
            map.unmap_value_high();

            map.op(0x0000, 0x0000).rw(m68705p.port_r_0, m68705p.port_latch_w_0);
            map.op(0x0001, 0x0001).rw(m68705p.port_r_1, m68705p.port_latch_w_1);
            map.op(0x0002, 0x0002).rw(m68705p.port_r_2, m68705p.port_latch_w_2);
            // 0x0003 not used (no port D)
            map.op(0x0004, 0x0004).w(m68705p.port_ddr_w_0);
            map.op(0x0005, 0x0005).w(m68705p.port_ddr_w_1);
            map.op(0x0006, 0x0006).w(m68705p.port_ddr_w_2);
            // 0x0007 not used (no port D)
            map.op(0x0008, 0x0008).rw(m68705p.tdr_r, m68705p.tdr_w);
            map.op(0x0009, 0x0009).rw(m68705p.tcr_r, m68705p.tcr_w);
            // 0x000a not used
            map.op(0x000b, 0x000b).rw(m68705p.pcr_r, m68705p.pcr_w);
            // 0x000c-0x000f not used
            map.op(0x0010, 0x007f).ram();
            map.op(0x0080, 0x0784).rw(m68705p.eprom_r_0x0080, m68705p.eprom_w_0x0080); // User EPROM
            map.op(0x0785, 0x07f7).rom().region("bootstrap", 0);
            map.op(0x07f8, 0x07ff).rw(m68705p.eprom_r_0x07f8, m68705p.eprom_w_0x07f8); // Interrupt vectors
        }


        protected override void device_start()
        {
            base.device_start();

            add_port_latch_state(0);
            add_port_latch_state(1);
            add_port_latch_state(2);
            add_port_ddr_state(0);
            add_port_ddr_state(1);
            add_port_ddr_state(2);
            add_timer_state();
            add_eprom_state();

            //throw new emu_unimplemented();
#if false
            state_add(M68705_MOR, "MOR", get_user_rom()[0x0784]).mask(0xff);
#endif
        }


        // device_disasm_interface overrides
        //protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
    }


    // ======================> m68705p5_device
    class m68705p5_device : m68705p_device
    {
        //DEFINE_DEVICE_TYPE(M68705P5, m68705p5_device, "m68705p5", "Motorola MC68705P5")
        static device_t device_creator_m68705p5_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new m68705p5_device(mconfig, tag, owner, clock); }
        public static readonly device_type M68705P5 = DEFINE_DEVICE_TYPE(device_creator_m68705p5_device, "m68705p5", "Motorola MC68705P5");


        //ROM_START( m68705p5 )
        static readonly List<tiny_rom_entry> rom_m68705p5 = new List<tiny_rom_entry>()
        {
            ROM_REGION(0x0073, "bootstrap", 0),
            ROM_LOAD("bootstrap.bin", 0x0000, 0x0073, CRC("f70a8620") + SHA1("c154f78c23f10bb903a531cb19e99121d5f7c19c")),
            ROM_END
        };


        m68705p5_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, tag, owner, clock, M68705P5)
        { }


        protected override List<tiny_rom_entry> device_rom_region()
        {
            return rom_m68705p5;
        }

        protected override u8 get_mask_options()
        {
            return get_user_rom()[0x0784];
        }
    }
}
