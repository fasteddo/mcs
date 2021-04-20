 // license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint16_t = System.UInt16;


namespace mame
{
    static class m68705_global
    {
        //#define LOG_GENERAL (1U <<  0)
        //#define LOG_INT     (1U <<  1)
        //#define LOG_IOPORT  (1U <<  2)
        public const int LOG_TIMER   = 1 <<  3;
        //#define LOG_EPROM   (1U <<  4)

        //#define VERBOSE (LOG_GENERAL | LOG_IOPORT | LOG_TIMER | LOG_EPROM)
        //#define LOG_OUTPUT_FUNC printf
        //#include "logmacro.h"

        //#define LOGINT(...)     LOGMASKED(LOG_INT,    __VA_ARGS__)
        //#define LOGIOPORT(...)  LOGMASKED(LOG_IOPORT, __VA_ARGS__)
        //#define LOGTIMER(...)   LOGMASKED(LOG_TIMER,  __VA_ARGS__)
        //#define LOGEPROM(...)   LOGMASKED(LOG_EPROM,  __VA_ARGS__)


        //enum : u16 {
        public const u16 M68705_VECTOR_BOOTSTRAP  = 0xfff6;
        public const u16 M6805_VECTOR_TIMER       = 0xfff8;
        public const u16 M6805_VECTOR_INT2        = 0xfff8;
        public const u16 M6805_VECTOR_INT         = 0xfffa;
        public const u16 M6805_VECTOR_SWI         = 0xfffc;
        public const u16 M6805_VECTOR_RESET       = 0xfffe;
        //};

        public const u16 M6805_INT_MASK           = 0x03;


        public const int M6805_INT_TIMER   = m6805_global.M6805_IRQ_LINE + 1;

        public const int M68705_IRQ_LINE   = m6805_global.M6805_IRQ_LINE + 0;
        public const int M68705_INT_TIMER  = m6805_global.M6805_IRQ_LINE + 1;
        public const int M68705_VPP_LINE   = m6805_global.M6805_IRQ_LINE + 2;
        public const int M68705_VIHTP_LINE = m6805_global.M6805_IRQ_LINE + 3;
    }


    public class m6805_timer : global_object
    {
        public enum timer_options //: u8
        {
            TIMER_PGM = 0x01, // programmable source and divisor
            TIMER_MOR = 0x02, // configured by mask option rom
            TIMER_NPC = 0x04, // no prescaler clear
        }

        public enum timer_source //: unsigned
        {
            CLOCK       = 0, // internal clock
            CLOCK_TIMER = 1, // internal clock AND external input
            DISABLED    = 2,
            TIMER       = 3, // external input
        }

        enum tcr_mask //: u8
        {
            TCR_PS  = 0x07, // prescaler value
            TCR_PSC = 0x08, // prescaler clear (write only, read as zero)
            TCR_TIE = 0x10, // timer external input enable
            TCR_TIN = 0x20, // timer input select
            TCR_TIM = 0x40, // timer interrupt mask
            TCR_TIR = 0x80, // timer interrupt request
        }


        // configuration state
        m6805_base_device m_parent;  //cpu_device &m_parent;
        timer_options m_options;  //u8 m_options;

        // internal state
        UInt32 m_divisor;
        timer_source m_source;
        bool m_timer;
        UInt32 m_timer_edges;  //unsigned m_timer_edges;
        u8 m_prescale;

        // visible state
        u8 m_tdr;
        u8 m_tcr;


        public m6805_timer(m6805_base_device parent)
        {
            m_parent = parent;
            m_options = 0;
            m_divisor = 7;
            m_source = timer_source.CLOCK;
            m_timer = false;
        }


        // configuration helpers
        public void set_options(timer_options options) { m_options = options; }
        public void set_divisor(UInt32 divisor) { m_divisor = divisor & 7; }
        public void set_source(timer_source source) { m_source = source; }


        public void start(UInt32 base_ = 0)
        {
            m_parent.save_item(NAME(new { m_timer }));
            m_parent.save_item(NAME(new { m_timer_edges }));
            m_parent.save_item(NAME(new { m_prescale }));
            m_parent.save_item(NAME(new { m_tdr }));
            m_parent.save_item(NAME(new { m_tcr }));

            m_parent.m_distate.state_add((int)base_ + 0, "PS", m_prescale);
            m_parent.m_distate.state_add((int)base_ + 1, "TDR", m_tdr);
            m_parent.m_distate.state_add((int)base_ + 2, "TCR", m_tcr);
        }


        public void reset()
        {
            m_timer_edges = 0;
            m_prescale = 0x7f;
            m_tdr = 0xff;
            m_tcr = 0x7f;
        }


        public u8 tdr_r() { return m_tdr; }
        public void tdr_w(u8 data) { m_tdr = data; }


        public u8 tcr_r()
        {
            throw new emu_unimplemented();
        }


        public void tcr_w(u8 data)
        {
            if ((logmacro_global.VERBOSE & m68705_global.LOG_TIMER) != 0)
                m_parent.logerror("tcr_w 0x{0}\n", data);

            if ((m_options & timer_options.TIMER_MOR) != 0)
                data |= (u8)tcr_mask.TCR_TIE;

            if ((m_options & timer_options.TIMER_PGM) != 0)
            {
                set_divisor((UInt32)(data & (u8)tcr_mask.TCR_PS));
                set_source((timer_source)((data & (u8)(tcr_mask.TCR_TIN | tcr_mask.TCR_TIE)) >> 4));
            }

            if ((data & (u8)tcr_mask.TCR_PSC) != 0 && (m_options & timer_options.TIMER_NPC) == 0)
                m_prescale = 0;

            m_tcr = (u8)((m_tcr & (data & (u8)tcr_mask.TCR_TIR)) | (data & (int)(~(tcr_mask.TCR_TIR | tcr_mask.TCR_PSC))));

            // this is a level-sensitive interrupt
            m_parent.set_input_line(m68705_global.M6805_INT_TIMER, ((m_tcr & (u8)tcr_mask.TCR_TIR) != 0 && (m_tcr & (u8)tcr_mask.TCR_TIM) == 0) ? 1 : 0);
        }


        public void update(UInt32 count)
        {
            if (m_source == timer_source.DISABLED || (m_source == timer_source.CLOCK_TIMER && !m_timer))
                return;

            // compute new prescaler value and counter decrements
            UInt32 prescale = (m_prescale & ((1U << (int)m_divisor) - 1)) + ((m_source == timer_source.TIMER) ? m_timer_edges : count);
            UInt32 decrements = prescale >> (int)m_divisor;

            // check for zero crossing
            bool interrupt = ((m_tdr != 0) ? (UInt32)m_tdr : 256U) <= decrements;

            // update state
            m_prescale = (u8)(prescale & 0x7f);
            m_tdr -= (u8)decrements;
            m_timer_edges = 0;

            if (interrupt)
            {
                if ((logmacro_global.VERBOSE & m68705_global.LOG_TIMER) != 0)
                    m_parent.logerror("timer interrupt\n");

                m_tcr |= (u8)tcr_mask.TCR_TIR;

                if ((m_tcr & (u8)tcr_mask.TCR_TIM) == 0)
                    m_parent.set_input_line(m68705_global.M6805_INT_TIMER, ASSERT_LINE);
            }
        }


        //void timer_w(int state);
    }


    // abstract device classes
    public class m6805_hmos_device : m6805_base_device
    {
        protected class device_execute_interface_m6805_hmos : device_execute_interface_m6805_base
        {
            public device_execute_interface_m6805_hmos(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void execute_set_input(int inputnum, int state) { ((m6805_hmos_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
        }


        // state index constants
        enum state_index
        {
            M68705_A = M6805_A,
            M68705_PC = M6805_PC,
            M68705_S = M6805_S,
            M68705_X = M6805_X,
            M68705_CC = M6805_CC,
            M68705_IRQ_STATE = M6805_IRQ_STATE,

            M68705_LATCHA = 0x10,
            M68705_LATCHB,
            M68705_LATCHC,
            M68705_LATCHD,
            M68705_DDRA,
            M68705_DDRB,
            M68705_DDRC,
            M68705_DDRD,

            M6805_PS,
            M6805_TDR,
            M6805_TCR,

            M68705_PCR,
            M68705_PLD,
            M68705_PLA,

            M68705_MOR
        }


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


        const int PORT_COUNT = 4;

        public class uint32_constant_PORT_COUNT : uint32_constant { public UInt32 value { get { return PORT_COUNT; } } }


        // timer/counter
        m6805_timer m_timer;

        // digital I/O
        bool [] m_port_open_drain;  //bool            m_port_open_drain[PORT_COUNT];
        u8 [] m_port_mask;  //u8              m_port_mask[PORT_COUNT];
        u8 [] m_port_input;  //u8              m_port_input[PORT_COUNT];
        u8 [] m_port_latch;  //u8              m_port_latch[PORT_COUNT];
        u8 [] m_port_ddr;  //u8              m_port_ddr[PORT_COUNT];
        devcb_read8.array<devcb_read8, uint32_constant_PORT_COUNT> m_port_cb_r;
        devcb_write8.array<devcb_write8, uint32_constant_PORT_COUNT> m_port_cb_w;

        // miscellaneous register
        //enum mr_mask : u8
        //{
        //    MR_INT2_MSK = 0x40, // INT2 interrupt mask
        //    MR_INT2_REQ = 0x80, // INT2 interrupt request
        //};
        u8 m_mr;

        UInt32 m_ram_size;  //unsigned const m_ram_size;


        protected m6805_hmos_device(machine_config mconfig, string tag, device_t owner, u32 clock, device_type type, u32 addr_width, UInt32 ram_size)
            : base(mconfig, tag, owner, clock, type, new configuration_params(addr_width > 13 ? s_hmos_b_ops : s_hmos_s_ops, s_hmos_cycles, addr_width, 0x007f, 0x0060, m68705_global.M6805_VECTOR_SWI), null)  //map)
        {
            m6805_base_device_after_ctor(map);


            //m_class_interfaces.Add(new device_execute_interface_m6805_hmos(mconfig, this));


            m_timer = new m6805_timer(this);
            m_port_open_drain = new bool [PORT_COUNT] { false, false, false, false };
            m_port_mask = new u8 [PORT_COUNT] { 0x00, 0x00, 0x00, 0x00 };
            m_port_input = new u8 [PORT_COUNT] { 0xff, 0xff, 0xff, 0xff };
            m_port_latch = new u8 [PORT_COUNT] { 0xff, 0xff, 0xff, 0xff };
            m_port_ddr = new u8 [PORT_COUNT] { 0x00, 0x00, 0x00, 0x00 };
            m_port_cb_r = new devcb_read8.array<devcb_read8, uint32_constant_PORT_COUNT>(this, () => { return new devcb_read8(this); });
            m_port_cb_w = new devcb_write8.array<devcb_write8, uint32_constant_PORT_COUNT>(this, () => { return new devcb_write8(this); });
            m_ram_size = ram_size;
        }


        protected m6805_timer timer { get { return m_timer; } }


        // configuration helpers
        public devcb_read.binder porta_r() { return m_port_cb_r[0].bind(); }
        //devcb_read.binder portb_r() { throw new emu_unimplemented(); } //return m_port_cb_r[1].bind(); }
        public devcb_read.binder portc_r() { return m_port_cb_r[2].bind(); }
        //devcb_read.binder portd_r() { throw new emu_unimplemented(); } //return m_port_cb_r[3].bind(); }
        public devcb_write.binder porta_w() { return m_port_cb_w[0].bind(); }
        public devcb_write.binder portb_w() { return m_port_cb_w[1].bind(); }
        //devcb_write.binder portc_w() { throw new emu_unimplemented(); } //return m_port_cb_w[2].bind(); }

        //WRITE_LINE_MEMBER(timer_w) { m_timer.timer_w(state); }


        void map(address_map map, device_t owner) { internal_map(map); }


        protected virtual void internal_map(address_map map)
        {
            map.unmap_value_high();

            map.op(0x0000, 0x0000).rw(port_r_0, port_latch_w_0);
            map.op(0x0001, 0x0001).rw(port_r_1, port_latch_w_1);
            map.op(0x0002, 0x0002).rw(port_r_2, port_latch_w_2);

            map.op(0x0004, 0x0004).w(port_ddr_w_0);
            map.op(0x0005, 0x0005).w(port_ddr_w_1);
            map.op(0x0006, 0x0006).w(port_ddr_w_2);

            map.op(0x0008, 0x0008).lrw8(() => { return m_timer.tdr_r(); }, "", (data) => { m_timer.tdr_w(data); }, "");  //map(0x0008, 0x0008).lrw8(NAME([this]() { return m_timer.tdr_r(); }), NAME([this](u8 data) { m_timer.tdr_w(data); }));
            map.op(0x0009, 0x0009).lrw8(() => { return m_timer.tcr_r(); }, "", (data) => { m_timer.tcr_w(data); }, "");  //map(0x0009, 0x0009).lrw8(NAME([this]() { return m_timer.tcr_r(); }), NAME([this](u8 data) { m_timer.tcr_w(data); }));

            // M68?05Px devices don't have Port D or the Miscellaneous register
            if (m_port_mask[3] != 0xff)
            {
                map.op(0x0003, 0x0003).rw(port_r_3, port_latch_w_3);
                map.op(0x000a, 0x000a).rw(misc_r, misc_w);
            }

            map.op(0x80 - m_ram_size, 0x007f).ram();
        }


        protected void set_port_open_drain(int N, bool value) { m_port_open_drain[N] = value; }  //template <std::size_t N> void set_port_open_drain(bool value);


        //template <std::size_t N>
        protected void set_port_mask(int N, u8 mask)
        {
            if (configured() || started())
                throw new emu_fatalerror("Attempt to set physical port mask after configuration");

            m_port_mask[N] = mask;
        }


        //template <std::size_t N> void port_input_w(uint8_t data) { m_port_input[N] = data & ~m_port_mask[N]; }


        u8 port_r(int N)  //template <std::size_t N> u8 port_r();
        {
            if (!m_port_cb_r[N].isnull())
            {
                u8 newval = (u8)(m_port_cb_r[N].op(0, (u8)(~m_port_ddr[N] & ~m_port_mask[N])) & ~m_port_mask[N]);
                if (newval != m_port_input[N])
                {
                    LOGIOPORT("read PORT{0}: new input = {1} & {2} (was {3})\n",
                            'A' + N, newval, ~m_port_ddr[N] & ~m_port_mask[N], m_port_input[N]);
                }

                m_port_input[N] = newval;
            }

            return (u8)(m_port_mask[N] | (m_port_latch[N] & m_port_ddr[N]) | (m_port_input[N] & ~m_port_ddr[N]));
        }

        protected u8 port_r_0() { return port_r(0); }
        protected u8 port_r_1() { return port_r(1); }
        protected u8 port_r_2() { return port_r(2); }
        protected u8 port_r_3() { return port_r(3); }


        void port_latch_w(int N, u8 data)  //template <std::size_t N> void port_latch_w(u8 data);
        {
            data = (u8)(data & ~m_port_mask[N]);
            u8 diff = (u8)(m_port_latch[N] ^ data);
            if (diff != 0)
                LOGIOPORT("write PORT{0} latch: {1} & {2} (was {3})\n", 'A' + N, data, m_port_ddr[N], m_port_latch[N]);
            m_port_latch[N] = data;
            if ((diff & m_port_ddr[N]) != 0)
                port_cb_w(N);
        }

        protected void port_latch_w_0(u8 data) { port_latch_w(0, data); }
        protected void port_latch_w_1(u8 data) { port_latch_w(1, data); }
        protected void port_latch_w_2(u8 data) { port_latch_w(2, data); }
        protected void port_latch_w_3(u8 data) { port_latch_w(3, data); }


        void port_ddr_w(int N, u8 data)  //template <std::size_t N> void port_ddr_w(u8 data);
        {
            data = (u8)(data & ~m_port_mask[N]);
            if (data != m_port_ddr[N])
            {
                LOGIOPORT("write DDR{0}: {1} (was {2})\n", 'A' + N, data, m_port_ddr[N]);
                m_port_ddr[N] = data;
                port_cb_w(N);
            }
        }

        protected void port_ddr_w_0(u8 data) { port_ddr_w(0, data); }
        protected void port_ddr_w_1(u8 data) { port_ddr_w(1, data); }
        protected void port_ddr_w_2(u8 data) { port_ddr_w(2, data); }
        protected void port_ddr_w_3(u8 data) { port_ddr_w(3, data); }


        void port_cb_w(int N)  //template <std::size_t N> void port_cb_w();
        {
            u8 data = m_port_open_drain[N] ? (u8)(m_port_latch[N] | ~m_port_ddr[N]) : m_port_latch[N];
            u8 mask = m_port_open_drain[N] ? (u8)(~m_port_latch[N] & m_port_ddr[N]) : m_port_ddr[N];
            m_port_cb_w[N].op(0, data, mask);
        }


        u8 misc_r() { return m_mr; }
        void misc_w(u8 data) { m_mr = data; }


        // A/D converter
        //u8 acr_r();
        //void acr_w(u8 data);
        //u8 arr_r();
        //void arr_w(u8 data);


        protected override void device_start()
        {
            base.device_start();

            save_item(NAME(new { m_port_input }));
            save_item(NAME(new { m_port_latch }));
            save_item(NAME(new { m_port_ddr }));

            // initialise digital I/O
            for (int i = 0; i < m_port_input.Length; i++) m_port_input[i] = 0xff;  //for (u8 &input : m_port_input) input = 0xff;
            m_port_cb_r.resolve_all();
            m_port_cb_w.resolve_all_safe();

            add_port_latch_state(0);
            add_port_latch_state(1);
            add_port_latch_state(2);
            // PORTD has no latch registered since it is either input-only or nonexistent
            add_port_ddr_state(0);
            add_port_ddr_state(1);
            add_port_ddr_state(2);

            m_timer.start((UInt32)state_index.M6805_PS);
        }


        protected override void device_reset()
        {
            base.device_reset();

            // reset digital I/O
            port_ddr_w_0(0x00);
            port_ddr_w_1(0x00);
            port_ddr_w_2(0x00);
            port_ddr_w_3(0x00);

            // reset timer/counter
            m_timer.reset();

            if (m_params.m_addr_width > 13)
                rm16(true, m68705_global.M6805_VECTOR_RESET, ref m_pc);
            else
                rm16(false, m68705_global.M6805_VECTOR_RESET, ref m_pc);
        }


        // device_execute_interface overrides
        protected virtual void device_execute_interface_execute_set_input(int inputnum, int state)
        {
            if (irq_state[inputnum] != state)
            {
                irq_state[inputnum] = (state == ASSERT_LINE) ? ASSERT_LINE : CLEAR_LINE;

                if (state != CLEAR_LINE)
                    pending_interrupts |= (uint16_t)(1 << inputnum);
                else if (m68705_global.M6805_INT_TIMER == inputnum)
                    pending_interrupts &= (uint16_t)(~(1 << inputnum)); // this one is level-sensitive
            }
        }


        protected override void interrupt()
        {
            if ((pending_interrupts & m68705_global.M6805_INT_MASK) != 0)
            {
                if ((CC & IFLAG) == 0)
                {
                    if (m_params.m_addr_width > 13)
                    {
                        pushword(true, m_pc);
                        pushbyte(true, m_x);
                        pushbyte(true, m_a);
                        pushbyte(true, m_cc);
                    }
                    else
                    {
                        pushword(false, m_pc);
                        pushbyte(false, m_x);
                        pushbyte(false, m_a);
                        pushbyte(false, m_cc);
                    }

                    SEI();
                    standard_irq_callback(0);

                    if (BIT(pending_interrupts, m6805_global.M6805_IRQ_LINE) != 0)
                    {
                        LOGINT("servicing /INT interrupt\n");
                        pending_interrupts = (uint16_t)(pending_interrupts & ~(1 << m6805_global.M6805_IRQ_LINE));
                        if (m_params.m_addr_width > 13)
                            rm16(true, m68705_global.M6805_VECTOR_INT, ref m_pc);
                        else
                            rm16(false, m68705_global.M6805_VECTOR_INT, ref m_pc);
                    }
                    else if (BIT(pending_interrupts, m68705_global.M6805_INT_TIMER) != 0)
                    {
                        LOGINT("servicing timer/counter interrupt\n");
                        if (m_params.m_addr_width > 13)
                            rm16(true, m68705_global.M6805_VECTOR_TIMER, ref m_pc);
                        else
                            rm16(false, m68705_global.M6805_VECTOR_TIMER, ref m_pc);
                    }
                    else
                    {
                        throw new emu_fatalerror("Unknown pending interrupt");
                    }

                    m_icount.i -= 11;
                    burn_cycles(11);
                }
            }
        }


        protected override void burn_cycles(UInt32 count)
        {
            m_timer.update(count);
        }


        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        //template <std::size_t N>
        protected void add_port_latch_state(int N)
        {
            //throw new emu_unimplemented();
#if false
            if (m_port_mask[N] != 0xff)
                state_add(M68705_LATCHA + N, util::string_format("LATCH%c", 'A' + N).c_str(), m_port_latch[N]).mask(~m_port_mask[N] & 0xff);
#endif
        }

        //template <std::size_t N>
        protected void add_port_ddr_state(int N)
        {
            //throw new emu_unimplemented();
#if false
            if (m_port_mask[N] != 0xff)
                state_add(M68705_DDRA + N, util::string_format("DDR%c", 'A' + N).c_str(), m_port_ddr[N]).mask(~m_port_mask[N] & 0xff);
#endif
        }
    }


    public abstract class m68705_device : m6805_hmos_device
                                          //device_nvram_interface
    {
        class device_execute_interface_m68705 : device_execute_interface_m6805_hmos
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


        enum mor_mask //: u8
        {
            MOR_PS   = 0x07, // prescaler divisor
            MOR_TIE  = 0x10, // timer external enable
            MOR_CLS  = 0x20, // clock source
            MOR_TOPT = 0x40, // timer option
            MOR_CLK  = 0x80, // oscillator type
        }


        device_execute_interface_m68705 m_diexecute;
        device_nvram_interface_m68705 m_nvram_interface;


        required_region_ptr_uint8_t m_user_rom;


        // EPROM control
        u8 m_vihtp;
        u8 m_pcr;
        u8 m_pl_data;
        u16 m_pl_addr;


        protected m68705_device(machine_config mconfig, string tag, device_t owner, u32 clock, device_type type, u32 addr_width, UInt32 ram_size)
            : base(mconfig, tag, owner, clock, type, addr_width, ram_size)
        {
            m_class_interfaces.Add(new device_execute_interface_m68705(mconfig, this));
            m_class_interfaces.Add(new device_nvram_interface_m68705(mconfig, this));  //device_nvram_interface(mconfig, *this)

            m_diexecute = GetClassInterface<device_execute_interface_m68705>();
            m_nvram_interface = GetClassInterface<device_nvram_interface_m68705>();


            m_user_rom = new required_region_ptr_uint8_t(this, DEVICE_SELF, 1U << (int)addr_width);
            m_vihtp = CLEAR_LINE;
            m_pcr = 0xff;
            m_pl_data = 0xff;
            m_pl_addr = 0xffff;
        }


        protected override void internal_map(address_map map)
        {
            base.internal_map(map);

            map.op(0x000b, 0x000b).rw(pcr_r, pcr_w);
        }


        protected override void device_start()
        {
            base.device_start();

            save_item(NAME(new { m_vihtp }));
            save_item(NAME(new { m_pcr }));
            save_item(NAME(new { m_pl_data }));
            save_item(NAME(new { m_pl_addr }));

            // initialise timer/counter
            u8 options = get_mask_options();
            if ((options & (u8)mor_mask.MOR_TOPT) != 0)
            {
                timer.set_options(m6805_timer.timer_options.TIMER_MOR);
                timer.set_divisor((UInt32)(options & (u8)mor_mask.MOR_PS));
                timer.set_source((m6805_timer.timer_source)((options & (u8)(mor_mask.MOR_CLS | mor_mask.MOR_TIE)) >> 4));
            }
            else
            {
                timer.set_options(m6805_timer.timer_options.TIMER_PGM);
            }

            // initialise EPROM control
            m_vihtp = CLEAR_LINE;
            m_pcr = 0xff;
            m_pl_data = 0xff;
            m_pl_addr = 0xffff;


            //throw new emu_unimplemented();
#if false
            state_add(M68705_PCR, "PCR", m_pcr).mask(0xff);
            state_add(M68705_PLA, "PLA", m_pl_addr).mask(0xffff);
            state_add(M68705_PLD, "PLD", m_pl_data).mask(0xff);
#endif
        }


        protected override void device_reset()
        {
            base.device_reset();

            // reset EPROM control
            m_pcr |= 0xfb; // b2 (/VPON) is driven by external input and hence unaffected by reset

            if (CLEAR_LINE != m_vihtp)
            {
                LOG("loading bootstrap vector\n");
                if (m_params.m_addr_width > 13)
                    rm16(true, m68705_global.M68705_VECTOR_BOOTSTRAP, ref m_pc);
                else
                    rm16(false, m68705_global.M68705_VECTOR_BOOTSTRAP, ref m_pc);
            }
            else
            {
                LOG("loading reset vector\n");
                if (m_params.m_addr_width > 13)
                    rm16(true, m68705_global.M6805_VECTOR_RESET, ref m_pc);
                else
                    rm16(false, m68705_global.M6805_VECTOR_RESET, ref m_pc);
            }
        }


        // device_execute_interface overrides
        protected override void device_execute_interface_execute_set_input(int inputnum, int state)
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
                    base.device_execute_interface_execute_set_input(inputnum, state);
                    break;
            }
        }


        // device_nvram_interface overrides
        //virtual void nvram_default() override;
        //virtual void nvram_read(emu_file &file) override;
        //virtual void nvram_write(emu_file &file) override;


        u8 eprom_r(offs_t B, offs_t offset)  //template <offs_t B> u8 eprom_r(offs_t offset);
        {
            if (pcr_vpon() && !pcr_ple())
                LOGEPROM("read EPROM {0} prevented when Vpp high and /PLE = 0\n", B + offset);

            // read locked out when /VPON and /PLE are asserted
            return (!pcr_vpon() || !pcr_ple()) ? m_user_rom.target[B + offset] : (u8)0xff;
        }

        protected u8 eprom_r_0x0080(offs_t offset) { return eprom_r(0x0080, offset); }
        protected u8 eprom_r_0x07f8(offs_t offset) { return eprom_r(0x07f8, offset); }


        void eprom_w(offs_t B, offs_t offset, u8 data)  //template <offs_t B> void eprom_w(offs_t offset, u8 data);
        {
            throw new emu_unimplemented();
        }

        protected void eprom_w_0x0080(offs_t offset, u8 data) { eprom_w(0x0080, offset, data); }
        protected void eprom_w_0x07f8(offs_t offset, u8 data) { eprom_w(0x07f8, offset, data); }


        protected u8 pcr_r()
        {
            throw new emu_unimplemented();
        }


        protected void pcr_w(u8 data)
        {
            throw new emu_unimplemented();
        }


        protected Pointer<u8> get_user_rom() { return m_user_rom.target; }  //u8 *const get_user_rom() const { return &m_user_rom[0]; }
        protected abstract u8 get_mask_options();


        bool pcr_vpon() { return BIT(m_pcr, 2) == 0; }
        bool pcr_pge()  { return BIT(m_pcr, 1) == 0; }
        bool pcr_ple()  { return BIT(m_pcr, 0) == 0; }
    }


    public abstract class m68705p_device : m68705_device
    {
        protected class device_disasm_interface_m68705p : device_disasm_interface_m6805_base
        {
            public device_disasm_interface_m68705p(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        protected m68705p_device(machine_config mconfig, string tag, device_t owner, u32 clock, device_type type)
            : base(mconfig, tag, owner, clock, type, 11, 112)
        {
            m_class_interfaces.Add(new device_disasm_interface_m68705p(mconfig, this));


            set_port_open_drain(0, true);   // Port A is open drain with internal pull-ups
            set_port_mask(2, 0xf0);         // Port C is four bits wide
            set_port_mask(3, 0xff);         // Port D isn't present
        }


        //void pa_w(u8 data) { port_input_w<0>(data); }
        //void pb_w(u8 data) { port_input_w<1>(data); }
        //void pc_w(u8 data) { port_input_w<2>(data); }


        protected override void internal_map(address_map map)
        {
            base.internal_map(map);

            map.op(0x0080, 0x0784).rw(eprom_r_0x0080, eprom_w_0x0080); // User EPROM
            map.op(0x0785, 0x07f7).rom().region("bootstrap", 0);
            map.op(0x07f8, 0x07ff).rw(eprom_r_0x07f8, eprom_w_0x07f8); // Interrupt vectors
        }


        protected override void device_start()
        {
            base.device_start();

            //throw new emu_unimplemented();
#if false
            state_add(M68705_MOR, "MOR", get_user_rom()[0x0784]).mask(0xff);
#endif
        }


        // device_disasm_interface overrides
        //protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
    }


    class m68705p5_device : m68705p_device
    {
        //DEFINE_DEVICE_TYPE(M68705P5, m68705p5_device, "m68705p5", "Motorola MC68705P5")
        static device_t device_creator_m68705p5_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new m68705p5_device(mconfig, tag, owner, clock); }
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
