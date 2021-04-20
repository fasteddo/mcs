// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    public class mb88_cpu_device : cpu_device
    {
        public class device_execute_interface_mb88 : device_execute_interface
        {
            public device_execute_interface_mb88(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint32_t execute_min_cycles() { return 1; }
            protected override uint32_t execute_max_cycles() { return 3; }
            protected override uint32_t execute_input_lines() { return 1; }
            protected override void execute_run() { ((mb88_cpu_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { ((mb88_cpu_device)device()).device_execute_interface_execute_set_input(state); }
            protected override uint64_t execute_clocks_to_cycles(uint64_t clocks) { return (clocks + 6 - 1) / 6; }
            protected override uint64_t execute_cycles_to_clocks(uint64_t cycles) { return cycles * 6; }
        }


        public class device_memory_interface_mb88 : device_memory_interface
        {
            public device_memory_interface_mb88(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((mb88_cpu_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_mb88 : device_state_interface
        {
            public device_state_interface_mb88(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void state_import(device_state_entry entry) { ((mb88_cpu_device)device()).device_state_interface_state_import(entry); }
            protected override void state_export(device_state_entry entry) { ((mb88_cpu_device)device()).device_state_interface_state_export(entry); }
            protected override void state_string_export(device_state_entry entry, out string str) { ((mb88_cpu_device)device()).device_state_interface_state_string_export(entry, out str); }
        }


        public class device_disasm_interface_mb88 : device_disasm_interface
        {
            public device_disasm_interface_mb88(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        //enum
        //{
        const int MB88_PC  = 1;
        const int MB88_PA  = 2;
        const int MB88_FLAGS = 3;
        const int MB88_SI  = 4;
        const int MB88_A   = 5;
        const int MB88_X   = 6;
        const int MB88_Y   = 7;
        const int MB88_PIO = 8;
        const int MB88_TH  = 9;
        const int MB88_TL  = 10;
        const int MB88_SB  = 11;
        //}


        /***************************************************************************
            ADDRESS MAPS
        ***************************************************************************/

        void program_9bit(address_map map, device_t owner)
        {
            map.op(0x000, 0x1ff).rom();
        }

        void program_10bit(address_map map, device_t owner)
        {
            map.op(0x000, 0x3ff).rom();
        }

        void program_11bit(address_map map, device_t owner)
        {
            map.op(0x000, 0x7ff).rom();
        }

        void data_4bit(address_map map, device_t owner)
        {
            map.op(0x00, 0x0f).ram();
        }

        void data_5bit(address_map map, device_t owner)
        {
            map.op(0x00, 0x1f).ram();
        }

        void data_6bit(address_map map, device_t owner)
        {
            map.op(0x00, 0x3f).ram();
        }

        void data_7bit(address_map map, device_t owner)
        {
            map.op(0x00, 0x7f).ram();
        }


        const int SERIAL_PRESCALE = 6;       /* guess */
        const int TIMER_PRESCALE = 32;      /* guess */

        const int SERIAL_DISABLE_THRESH = 1000;    /* at this value, we give up driving the serial port */

        const int INT_CAUSE_SERIAL = 0x01;
        const int INT_CAUSE_TIMER  = 0x02;
        const int INT_CAUSE_EXTERNAL = 0x04;


        byte READOP(offs_t a) { return m_cache.read_byte(a); }

        byte RDMEM(offs_t a) { return m_data.read_byte(a); }
        void WRMEM(offs_t a, byte v) { m_data.write_byte(a, v); }

        int TEST_ST() { return m_st & 1; }
        int TEST_ZF() { return m_zf & 1; }
        int TEST_CF() { return m_cf & 1; }
        int TEST_VF() { return m_vf & 1; }
        int TEST_SF() { return m_sf & 1; }
        int TEST_NF() { return m_nf & 1; }

        void UPDATE_ST_C(byte v) { m_st=(v&0x10) != 0 ? (byte)0 : (byte)1; }
        void UPDATE_ST_Z(byte v) { m_st=(v==0) ? (byte)0 : (byte)1; }

        void UPDATE_CF(byte v) { m_cf=((v&0x10)==0) ? (byte)0 : (byte)1; }
        void UPDATE_ZF(byte v) { m_zf=(v!=0) ? (byte)0 : (byte)1; }

        void CYCLES(int x) { m_icount.i -= x; }

        UInt16 GETPC() { return (UInt16)(((int)m_PA << 6) + m_PC); }
        offs_t GETEA() { return (offs_t)((m_X << 4) + m_Y); }

        void INCPC() { m_PC++; if ( m_PC >= 0x40 ) { m_PC = 0; m_PA++; } }


        device_memory_interface_mb88 m_dimemory;
        device_execute_interface_mb88 m_diexec;
        device_state_interface_mb88 m_distate;

        address_space_config m_program_config;
        address_space_config m_data_config;

        uint8_t m_PC;     /* Program Counter: 6 bits */
        uint8_t m_PA;     /* Page Address: 4 bits */
        uint16_t [] m_SP = new uint16_t[4];  /* Stack is 4*10 bit addresses deep, but we also use 3 top bits per address to store flags during irq */
        uint8_t m_SI;     /* Stack index: 2 bits */
        uint8_t m_A;      /* Accumulator: 4 bits */
        uint8_t m_X;      /* Index X: 4 bits */
        uint8_t m_Y;      /* Index Y: 4 bits */
        uint8_t m_st;     /* State flag: 1 bit */
        uint8_t m_zf;     /* Zero flag: 1 bit */
        uint8_t m_cf;     /* Carry flag: 1 bit */
        uint8_t m_vf;     /* Timer overflow flag: 1 bit */
        uint8_t m_sf;     /* Serial Full/Empty flag: 1 bit */
        uint8_t m_nf;     /* Interrupt flag: 1 bit */

        /* Peripheral Control */
        uint8_t m_pio; /* Peripheral enable bits: 8 bits */

        /* Timer registers */
        uint8_t m_TH; /* Timer High: 4 bits */
        uint8_t m_TL; /* Timer Low: 4 bits */
        uint8_t m_TP; /* Timer Prescale: 6 bits? */
        uint8_t m_ctr; /* current external counter value */

        /* Serial registers */
        uint8_t m_SB; /* Serial buffer: 4 bits */
        uint16_t m_SBcount;    /* number of bits received */
        emu_timer m_serial;

        /* PLA configuration and port callbacks */
        uint8_t [] m_PLA;  //UINT8 * m_PLA;
        devcb_read8 m_read_k;
        devcb_write8 m_write_o;
        devcb_write8 m_write_p;
        devcb_read8.array<devcb_read8, uint32_constant_4> m_read_r;
        devcb_write8.array<devcb_write8, uint32_constant_4> m_write_r;
        devcb_read_line m_read_si;
        devcb_write_line m_write_so;

        /* IRQ handling */
        uint8_t m_pending_interrupt;

        memory_access.cache m_cache = new memory_access(11, 0, 0, endianness_t.ENDIANNESS_BIG).m_cache;  //memory_access<11, 0, 0, ENDIANNESS_BIG>::cache m_cache;
        memory_access.specific m_program = new memory_access(11, 0, 0, endianness_t.ENDIANNESS_BIG).m_specific;  //memory_access<11, 0, 0, ENDIANNESS_BIG>::specific m_program;
        memory_access.specific m_data = new memory_access(7, 0, 0, endianness_t.ENDIANNESS_BIG).m_specific;  //memory_access< 7, 0, 0, ENDIANNESS_BIG>::specific m_data;

        //int m_icount;
        intref m_icount = new intref();

        // For the debugger
        uint16_t m_debugger_pc;
        uint8_t m_debugger_flags;


        // construction/destruction
        public mb88_cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, int program_width, int data_width)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_mb88(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_mb88(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_mb88(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_mb88(mconfig, this));


            m_program_config = new address_space_config("program", endianness_t.ENDIANNESS_BIG, 8, (byte)program_width, 0, (program_width == 9) ? program_9bit : (program_width == 10) ? program_10bit : (address_map_constructor)program_11bit);
            m_data_config = new address_space_config("data", endianness_t.ENDIANNESS_BIG, 8, (byte)data_width, 0, (data_width == 4) ? data_4bit : (data_width == 5) ? data_5bit : (data_width == 6) ? data_6bit : (address_map_constructor)data_7bit);
            m_PLA = null;
            m_read_k = new devcb_read8(this);
            m_write_o = new devcb_write8(this);
            m_write_p = new devcb_write8(this);
            m_read_r = new devcb_read8.array<devcb_read8, uint32_constant_4>(this, () => { return new devcb_read8(this); });
            m_write_r = new devcb_write8.array<devcb_write8, uint32_constant_4>(this, () => { return new devcb_write8(this); });
            m_read_si = new devcb_read_line(this);
            m_write_so = new devcb_write_line(this);
        }


        // configuration helpers

        // K (K3-K0): input-only port
        public devcb_read.binder read_k() { return m_read_k.bind(); }  //auto read_k() { return m_read_k.bind(); }

        // O (O7-O4 = OH, O3-O0 = OL): output through PLA
        public devcb_write.binder write_o() { return m_write_o.bind(); }  //auto write_o() { return m_write_o.bind(); }

        // P (P3-P0): output-only port
        public devcb_write.binder write_p() { return m_write_p.bind(); }  //auto write_p() { return m_write_p.bind(); }

        // R0 (R3-R0): input/output port
        public devcb_read.binder read_r(int Port) { return m_read_r[Port].bind(); }  //template <std::size_t Port> auto read_r() { return m_read_r[Port].bind(); }
        public devcb_write.binder write_r(int Port) { return m_write_r[Port].bind(); }  //template <std::size_t Port> auto write_r() { return m_write_r[Port].bind(); }

        // SI: serial input
        //auto read_si() { return m_read_si.bind(); }

        // SO: serial output
        //auto write_so() { return m_write_so.bind(); }


        //static void set_pla(device_t &device, UINT8 *pla) { downcast<mb88_cpu_device &>(device).m_PLA = pla; }


        //WRITE_LINE_MEMBER( mb88_cpu_device::clock_w )
        public void clock_w(int state)
        {
            if (state != m_ctr)
            {
                m_ctr = (uint8_t)state;

                /* on a falling clock, increment the timer, but only if enabled */
                if (m_ctr == 0 && (m_pio & 0x40) != 0)
                    increment_timer();
            }
        }


        //void data_4bit(address_map &map);
        //void data_5bit(address_map &map);
        //void data_6bit(address_map &map);
        //void data_7bit(address_map &map);
        //void program_10bit(address_map &map);
        //void program_11bit(address_map &map);
        //void program_9bit(address_map &map);


        // device-level overrides

        /***************************************************************************
            INITIALIZATION AND SHUTDOWN
        ***************************************************************************/
        protected override void device_start()
        {
            m_dimemory = GetClassInterface<device_memory_interface_mb88>();
            m_diexec = GetClassInterface<device_execute_interface_mb88>();
            m_distate = GetClassInterface<device_state_interface_mb88>();


            m_dimemory.space(AS_PROGRAM).cache(m_cache.Width, m_cache.AddrShift, m_cache.Endian, m_cache);
            m_dimemory.space(AS_PROGRAM).specific(m_program.Level, m_program.Width, m_program.AddrShift, m_program.Endian, m_program);
            m_dimemory.space(AS_DATA).specific(m_data.Level, m_data.Width, m_data.AddrShift, m_data.Endian, m_data);

            m_read_k.resolve_safe(0);
            m_write_o.resolve_safe();
            m_write_p.resolve_safe();
            m_read_r.resolve_all_safe(0);
            m_write_r.resolve_all_safe();
            m_read_si.resolve_safe(0);
            m_write_so.resolve_safe();

            m_serial = machine().scheduler().timer_alloc(serial_timer);  //timer_expired_delegate(FUNC(mb88_cpu_device::serial_timer), this));

            m_ctr = 0;

            save_item(NAME(new { m_PC }));
            save_item(NAME(new { m_PA }));
            save_item(NAME(new { m_SP }));  //save_item(NAME(m_SP[0]));
            //save_item(NAME(new { m_SP[1] }));
            //save_item(NAME(new { m_SP[2] }));
            //save_item(NAME(new { m_SP[3] }));
            save_item(NAME(new { m_SI }));
            save_item(NAME(new { m_A }));
            save_item(NAME(new { m_X }));
            save_item(NAME(new { m_Y }));
            save_item(NAME(new { m_st }));
            save_item(NAME(new { m_zf }));
            save_item(NAME(new { m_cf }));
            save_item(NAME(new { m_vf }));
            save_item(NAME(new { m_sf }));
            save_item(NAME(new { m_nf }));
            save_item(NAME(new { m_pio }));
            save_item(NAME(new { m_TH }));
            save_item(NAME(new { m_TL }));
            save_item(NAME(new { m_TP }));
            save_item(NAME(new { m_ctr }));
            save_item(NAME(new { m_SB }));
            save_item(NAME(new { m_SBcount }));
            save_item(NAME(new { m_pending_interrupt }));

            m_distate.state_add( MB88_PC,  "PC",  m_PC).formatstr("%02X");
            m_distate.state_add( MB88_PA,  "PA",  m_PA).formatstr("%02X");
            m_distate.state_add( MB88_SI,  "SI",  m_SI).formatstr("%01X");
            m_distate.state_add( MB88_A,   "A",   m_A).formatstr("%01X");
            m_distate.state_add( MB88_X,   "X",   m_X).formatstr("%01X");
            m_distate.state_add( MB88_Y,   "Y",   m_Y).formatstr("%01X");
            m_distate.state_add( MB88_PIO, "PIO", m_pio).formatstr("%02X");
            m_distate.state_add( MB88_TH,  "TH",  m_TH).formatstr("%01X");
            m_distate.state_add( MB88_TL,  "TL",  m_TL).formatstr("%01X");
            m_distate.state_add( MB88_SB,  "SB",  m_SB).formatstr("%01X");

            m_distate.state_add( STATE_GENPC, "GENPC", m_debugger_pc).callimport().callexport().noshow();
            m_distate.state_add( STATE_GENPCBASE, "CURPC", m_debugger_pc ).callimport().callexport().noshow();
            m_distate.state_add( STATE_GENFLAGS, "GENFLAGS", m_debugger_flags).callimport().callexport().formatstr("%6s").noshow();

            set_icountptr(m_icount);
        }


        protected override void device_stop()
        {
        }


        protected override void device_reset()
        {
            /* zero registers and flags */
            m_PC = 0;
            m_PA = 0;
            m_SP[0] = m_SP[1] = m_SP[2] = m_SP[3] = 0;
            m_SI = 0;
            m_A = 0;
            m_X = 0;
            m_Y = 0;
            m_st = 1;   /* start off with st=1 */
            m_zf = 0;
            m_cf = 0;
            m_vf = 0;
            m_sf = 0;
            m_nf = 0;
            m_pio = 0;
            m_TH = 0;
            m_TL = 0;
            m_TP = 0;
            m_SB = 0;
            m_SBcount = 0;
            m_pending_interrupt = 0;
        }


        // device_execute_interface overrides
        //virtual uint32_t execute_min_cycles() const noexcept override { return 1; }
        //virtual uint32_t execute_max_cycles() const noexcept override { return 3; }
        //virtual uint32_t execute_input_lines() const noexcept override { return 1; }

        void device_execute_interface_execute_run()
        {
            while (m_icount.i > 0)
            {
                byte opcode;
                byte arg;
                byte oc;

                /* fetch the opcode */
                debugger_instruction_hook(GETPC());
                opcode = READOP(GETPC());

                /* increment the PC */
                INCPC();

                /* start with instruction doing 1 cycle */
                oc = 1;

                switch (opcode)
                {
                    case 0x00: /* nop ZCS:...*/
                        m_st = 1;
                        break;

                    case 0x01: /* outO ZCS:...*/
                        m_write_o.op((byte)pla(m_A, TEST_CF()));
                        m_st = 1;
                        break;

                    case 0x02: /* outP ZCS:... */
                        m_write_p.op(m_A);
                        m_st = 1;
                        break;

                    case 0x03: /* outR ZCS:... */
                        arg = m_Y;
                        m_write_r[arg & 3].op(m_A);
                        m_st = 1;
                        break;

                    case 0x04: /* tay ZCS:... */
                        m_Y = m_A;
                        m_st = 1;
                        break;

                    case 0x05: /* tath ZCS:... */
                        m_TH = m_A;
                        m_st = 1;
                        break;

                    case 0x06: /* tatl ZCS:... */
                        m_TL = m_A;
                        m_st = 1;
                        break;

                    case 0x07: /* tas ZCS:... */
                        m_SB = m_A;
                        m_st = 1;
                        break;

                    case 0x08: /* icy ZCS:x.x */
                        m_Y++;
                        UPDATE_ST_C(m_Y);
                        m_Y &= 0x0f;
                        UPDATE_ZF(m_Y);
                        break;

                    case 0x09: /* icm ZCS:x.x */
                        arg=RDMEM(GETEA());
                        arg++;
                        UPDATE_ST_C(arg);
                        arg &= 0x0f;
                        UPDATE_ZF(arg);
                        WRMEM(GETEA(),arg);
                        break;

                    case 0x0a: /* stic ZCS:x.x */
                        WRMEM(GETEA(),m_A);
                        m_Y++;
                        UPDATE_ST_C(m_Y);
                        m_Y &= 0x0f;
                        UPDATE_ZF(m_Y);
                        break;

                    case 0x0b: /* x ZCS:x.. */
                        arg = RDMEM(GETEA());
                        WRMEM(GETEA(),m_A);
                        m_A = arg;
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0x0c: /* rol ZCS:xxx */
                        m_A <<= 1;
                        m_A |= (byte)TEST_CF();
                        UPDATE_ST_C(m_A);
                        m_cf = (byte)(m_st ^ 1);
                        m_A &= 0x0f;
                        UPDATE_ZF(m_A);
                        break;

                    case 0x0d: /* l ZCS:x.. */
                        m_A = RDMEM(GETEA());
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0x0e: /* adc ZCS:xxx */
                        arg = RDMEM(GETEA());
                        arg += m_A;
                        arg += (byte)TEST_CF();
                        UPDATE_ST_C(arg);
                        m_cf = (byte)(m_st ^ 1);
                        m_A = (byte)(arg & 0x0f);
                        UPDATE_ZF(m_A);
                        break;

                    case 0x0f: /* and ZCS:x.x */
                        m_A &= RDMEM(GETEA());
                        UPDATE_ZF(m_A);
                        m_st = (byte)(m_zf ^ 1);
                        break;

                    case 0x10: /* daa ZCS:.xx */
                        if ( TEST_CF() != 0 || m_A > 9 ) m_A += 6;
                        UPDATE_ST_C(m_A);
                        m_cf = (byte)(m_st ^ 1);
                        m_A &= 0x0f;
                        break;

                    case 0x11: /* das ZCS:.xx */
                        if ( TEST_CF() != 0 || m_A > 9 ) m_A += 10;
                        UPDATE_ST_C(m_A);
                        m_cf = (byte)(m_st ^ 1);
                        m_A &= 0x0f;
                        break;

                    case 0x12: /* inK ZCS:x.. */
                        m_A = (byte)(m_read_k.op() & 0x0f);
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0x13: /* inR ZCS:x.. */
                        arg = m_Y;
                        m_A = (byte)(m_read_r[arg & 3].op() & 0x0f);
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0x14: /* tya ZCS:x.. */
                        m_A = m_Y;
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0x15: /* ttha ZCS:x.. */
                        m_A = m_TH;
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0x16: /* ttla ZCS:x.. */
                        m_A = m_TL;
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0x17: /* tsa ZCS:x.. */
                        m_A = m_SB;
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0x18: /* dcy ZCS:..x */
                        m_Y--;
                        UPDATE_ST_C(m_Y);
                        m_Y &= 0x0f;
                        break;

                    case 0x19: /* dcm ZCS:x.x */
                        arg=RDMEM(GETEA());
                        arg--;
                        UPDATE_ST_C(arg);
                        arg &= 0x0f;
                        UPDATE_ZF(arg);
                        WRMEM(GETEA(),arg);
                        break;

                    case 0x1a: /* stdc ZCS:x.x */
                        WRMEM(GETEA(),m_A);
                        m_Y--;
                        UPDATE_ST_C(m_Y);
                        m_Y &= 0x0f;
                        UPDATE_ZF(m_Y);
                        break;

                    case 0x1b: /* xx ZCS:x.. */
                        arg = m_X;
                        m_X = m_A;
                        m_A = arg;
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0x1c: /* ror ZCS:xxx */
                        m_A |= (byte)(TEST_CF() << 4);
                        UPDATE_ST_C((byte)(m_A << 4));
                        m_cf = (byte)(m_st ^ 1);
                        m_A >>= 1;
                        m_A &= 0x0f;
                        UPDATE_ZF(m_A);
                        break;

                    case 0x1d: /* st ZCS:x.. */
                        WRMEM(GETEA(),m_A);
                        m_st = 1;
                        break;

                    case 0x1e: /* sbc ZCS:xxx */
                        arg = RDMEM(GETEA());
                        arg -= m_A;
                        arg -= (byte)TEST_CF();
                        UPDATE_ST_C(arg);
                        m_cf = (byte)(m_st ^ 1);
                        m_A = (byte)(arg & 0x0f);
                        UPDATE_ZF(m_A);
                        break;

                    case 0x1f: /* or ZCS:x.x */
                        m_A |= RDMEM(GETEA());
                        UPDATE_ZF(m_A);
                        m_st = (byte)(m_zf ^ 1);
                        break;

                    case 0x20: /* setR ZCS:... */
                        arg = m_read_r[m_Y/4].op();
                        m_write_r[m_Y/4].op((byte)(arg | (1 << (m_Y%4))));
                        m_st = 1;
                        break;

                    case 0x21: /* setc ZCS:.xx */
                        m_cf = 1;
                        m_st = 1;
                        break;

                    case 0x22: /* rstR ZCS:... */
                        arg = m_read_r[m_Y/4].op();
                        m_write_r[m_Y/4].op((byte)(arg & ~(1 << (m_Y%4))));
                        m_st = 1;
                        break;

                    case 0x23: /* rstc ZCS:.xx */
                        m_cf = 0;
                        m_st = 1;
                        break;

                    case 0x24: /* tstr ZCS:..x */
                        arg = m_read_r[m_Y/4].op();
                        m_st = ( arg & ( 1 << (m_Y%4) ) ) != 0 ? (byte)0 : (byte)1;
                        break;

                    case 0x25: /* tsti ZCS:..x */
                        m_st = (byte)(m_nf ^ 1);
                        break;

                    case 0x26: /* tstv ZCS:..x */
                        m_st = (byte)(m_vf ^ 1);
                        m_vf = 0;
                        break;

                    case 0x27: /* tsts ZCS:..x */
                        m_st = (byte)(m_sf ^ 1);
                        if (m_sf != 0)
                        {
                            /* re-enable the timer if we disabled it previously */
                            if (m_SBcount >= mb88_cpu_device.SERIAL_DISABLE_THRESH)
                                m_serial.adjust(attotime.from_hz(clock() / mb88_cpu_device.SERIAL_PRESCALE), 0, attotime.from_hz(clock() / mb88_cpu_device.SERIAL_PRESCALE));
                            m_SBcount = 0;
                        }
                        m_sf = 0;
                        break;

                    case 0x28: /* tstc ZCS:..x */
                        m_st = (byte)(m_cf ^ 1);
                        break;

                    case 0x29: /* tstz ZCS:..x */
                        m_st = (byte)(m_zf ^ 1);
                        break;

                    case 0x2a: /* sts ZCS:x.. */
                        WRMEM(GETEA(),m_SB);
                        UPDATE_ZF(m_SB);
                        m_st = 1;
                        break;

                    case 0x2b: /* ls ZCS:x.. */
                        m_SB = RDMEM(GETEA());
                        UPDATE_ZF(m_SB);
                        m_st = 1;
                        break;

                    case 0x2c: /* rts ZCS:... */
                        m_SI = (byte)(( m_SI - 1 ) & 3);
                        m_PC = (byte)(m_SP[m_SI] & 0x3f);
                        m_PA = (byte)((m_SP[m_SI] >> 6) & 0x1f);
                        m_st = 1;
                        break;

                    case 0x2d: /* neg ZCS: ..x */
                        m_A = (byte)((~m_A)+1);
                        m_A &= 0x0f;
                        UPDATE_ST_Z(m_A);
                        break;

                    case 0x2e: /* c ZCS:xxx */
                        arg = RDMEM(GETEA());
                        arg -= m_A;
                        UPDATE_CF(arg);
                        arg &= 0x0f;
                        UPDATE_ST_Z(arg);
                        m_zf = (byte)(m_st ^ 1);
                        break;

                    case 0x2f: /* eor ZCS:x.x */
                        m_A ^= RDMEM(GETEA());
                        UPDATE_ST_Z(m_A);
                        m_zf = (byte)(m_st ^ 1);
                        break;

                    case 0x30: case 0x31: case 0x32: case 0x33: /* sbit ZCS:... */
                        arg = RDMEM(GETEA());
                        WRMEM(GETEA(), (byte)(arg | (1 << (opcode&3))));
                        m_st = 1;
                        break;

                    case 0x34: case 0x35: case 0x36: case 0x37: /* rbit ZCS:... */
                        arg = RDMEM(GETEA());
                        WRMEM(GETEA(), (byte)(arg & ~(1 << (opcode&3))));
                        m_st = 1;
                        break;

                    case 0x38: case 0x39: case 0x3a: case 0x3b: /* tbit ZCS:... */
                        arg = RDMEM(GETEA());
                        m_st = ( arg & (1 << (opcode&3) ) ) != 0 ? (byte)0 : (byte)1;
                        break;

                    case 0x3c: /* rti ZCS:... */
                        /* restore address and saved state flags on the top bits of the stack */
                        m_SI = (byte)(( m_SI - 1 ) & 3);
                        m_PC = (byte)(m_SP[m_SI] & 0x3f);
                        m_PA = (byte)((m_SP[m_SI] >> 6) & 0x1f);
                        m_st = (byte)((m_SP[m_SI] >> 13)&1);
                        m_zf = (byte)((m_SP[m_SI] >> 14)&1);
                        m_cf = (byte)((m_SP[m_SI] >> 15)&1);
                        break;

                    case 0x3d: /* jpa imm ZCS:..x */
                        m_PA = (byte)(READOP(GETPC()) & 0x1f);
                        m_PC = (byte)(m_A * 4);
                        oc = 2;
                        m_st = 1;
                        break;

                    case 0x3e: /* en imm ZCS:... */
                        update_pio_enable((byte)(m_pio | READOP(GETPC())));
                        INCPC();
                        oc = 2;
                        m_st = 1;
                        break;

                    case 0x3f: /* dis imm ZCS:... */
                        update_pio_enable((byte)(m_pio & ~(READOP(GETPC()))));
                        INCPC();
                        oc = 2;
                        m_st = 1;
                        break;

                    case 0x40:  case 0x41:  case 0x42:  case 0x43: /* setD ZCS:... */
                        arg = m_read_r[0].op();
                        arg |= (byte)(1 << (opcode&3));
                        m_write_r[0].op(arg);
                        m_st = 1;
                        break;

                    case 0x44:  case 0x45:  case 0x46:  case 0x47: /* rstD ZCS:... */
                        arg = m_read_r[0].op();
                        arg &= (byte)(~(1 << (opcode&3)));
                        m_write_r[0].op(arg);
                        m_st = 1;
                        break;

                    case 0x48:  case 0x49:  case 0x4a:  case 0x4b: /* tstD ZCS:..x */
                        arg = m_read_r[2].op();
                        m_st = (arg & (1 << (opcode&3))) != 0 ? (byte)0 : (byte)1;
                        break;

                    case 0x4c:  case 0x4d:  case 0x4e:  case 0x4f: /* tba ZCS:..x */
                        m_st = (m_A & (1 << (opcode&3))) != 0 ? (byte)0 : (byte)1;
                        break;

                    case 0x50:  case 0x51:  case 0x52:  case 0x53: /* xd ZCS:x.. */
                        arg = RDMEM((UInt32)(opcode&3));
                        WRMEM((UInt32)(opcode&3),m_A);
                        m_A = arg;
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0x54:  case 0x55:  case 0x56:  case 0x57: /* xyd ZCS:x.. */
                        arg = RDMEM((UInt32)((opcode&3)+4));
                        WRMEM((UInt32)((opcode&3)+4),m_Y);
                        m_Y = arg;
                        UPDATE_ZF(m_Y);
                        m_st = 1;
                        break;

                    case 0x58:  case 0x59:  case 0x5a:  case 0x5b:
                    case 0x5c:  case 0x5d:  case 0x5e:  case 0x5f: /* lxi ZCS:x.. */
                        m_X = (byte)(opcode & 7);
                        UPDATE_ZF(m_X);
                        m_st = 1;
                        break;

                    case 0x60:  case 0x61:  case 0x62:  case 0x63:
                    case 0x64:  case 0x65:  case 0x66:  case 0x67: /* call imm ZCS:..x */
                        arg = READOP(GETPC());
                        INCPC();
                        oc = 2;
                        if ( TEST_ST() != 0 )
                        {
                            m_SP[m_SI] = GETPC();
                            m_SI = (byte)(( m_SI + 1 ) & 3);
                            m_PC = (byte)(arg & 0x3f);
                            m_PA = (byte)(( ( opcode & 7 ) << 2 ) | ( arg >> 6 ));
                        }
                        m_st = 1;
                        break;

                    case 0x68:  case 0x69:  case 0x6a:  case 0x6b:
                    case 0x6c:  case 0x6d:  case 0x6e:  case 0x6f: /* jpl imm ZCS:..x */
                        arg = READOP(GETPC());
                        INCPC();
                        oc = 2;
                        if ( TEST_ST() != 0 )
                        {
                            m_PC = (byte)(arg & 0x3f);
                            m_PA = (byte)(( ( opcode & 7 ) << 2 ) | ( arg >> 6 ));
                        }
                        m_st = 1;
                        break;

                    case 0x70:  case 0x71:  case 0x72:  case 0x73:
                    case 0x74:  case 0x75:  case 0x76:  case 0x77:
                    case 0x78:  case 0x79:  case 0x7a:  case 0x7b:
                    case 0x7c:  case 0x7d:  case 0x7e:  case 0x7f: /* ai ZCS:xxx */
                        arg = (byte)(opcode & 0x0f);
                        arg += m_A;
                        UPDATE_ST_C(arg);
                        m_cf = (byte)(m_st ^ 1);
                        m_A = (byte)(arg & 0x0f);
                        UPDATE_ZF(m_A);
                        break;

                    case 0x80:  case 0x81:  case 0x82:  case 0x83:
                    case 0x84:  case 0x85:  case 0x86:  case 0x87:
                    case 0x88:  case 0x89:  case 0x8a:  case 0x8b:
                    case 0x8c:  case 0x8d:  case 0x8e:  case 0x8f: /* lxi ZCS:x.. */
                        m_Y = (byte)(opcode & 0x0f);
                        UPDATE_ZF(m_Y);
                        m_st = 1;
                        break;

                    case 0x90:  case 0x91:  case 0x92:  case 0x93:
                    case 0x94:  case 0x95:  case 0x96:  case 0x97:
                    case 0x98:  case 0x99:  case 0x9a:  case 0x9b:
                    case 0x9c:  case 0x9d:  case 0x9e:  case 0x9f: /* li ZCS:x.. */
                        m_A = (byte)(opcode & 0x0f);
                        UPDATE_ZF(m_A);
                        m_st = 1;
                        break;

                    case 0xa0:  case 0xa1:  case 0xa2:  case 0xa3:
                    case 0xa4:  case 0xa5:  case 0xa6:  case 0xa7:
                    case 0xa8:  case 0xa9:  case 0xaa:  case 0xab:
                    case 0xac:  case 0xad:  case 0xae:  case 0xaf: /* cyi ZCS:xxx */
                        arg = (byte)((opcode & 0x0f) - m_Y);
                        UPDATE_CF(arg);
                        arg &= 0x0f;
                        UPDATE_ST_Z(arg);
                        m_zf = (byte)(m_st ^ 1);
                        break;

                    case 0xb0:  case 0xb1:  case 0xb2:  case 0xb3:
                    case 0xb4:  case 0xb5:  case 0xb6:  case 0xb7:
                    case 0xb8:  case 0xb9:  case 0xba:  case 0xbb:
                    case 0xbc:  case 0xbd:  case 0xbe:  case 0xbf: /* ci ZCS:xxx */
                        arg = (byte)((opcode & 0x0f) - m_A);
                        UPDATE_CF(arg);
                        arg &= 0x0f;
                        UPDATE_ST_Z(arg);
                        m_zf = (byte)(m_st ^ 1);
                        break;

                    default: /* jmp ZCS:..x */
                        if ( TEST_ST() != 0 )
                        {
                            m_PC = (byte)(opcode & 0x3f);
                        }
                        m_st = 1;
                        break;
                }

                /* update cycle counts */
                CYCLES( oc );

                /* update interrupts, serial and timer flags */
                update_pio(oc);
            }
        }

        void device_execute_interface_execute_set_input(int state)
        {
            /* on rising edge trigger interrupt */
            if ( (m_pio & 0x04) != 0 && m_nf == 0 && state != CLEAR_LINE )
            {
                m_pending_interrupt |= INT_CAUSE_EXTERNAL;
            }

            m_nf = state != CLEAR_LINE ? (uint8_t)1 : (uint8_t)0;
        }

        //virtual uint64_t execute_clocks_to_cycles(uint64_t clocks) const noexcept override { return (clocks + 6 - 1) / 6; }
        //virtual uint64_t execute_cycles_to_clocks(uint64_t cycles) const noexcept override { return (cycles * 6); }


        // device_memory_interface overrides
        space_config_vector device_memory_interface_memory_space_config()
        {
            return new space_config_vector()
            {
                std.make_pair(AS_PROGRAM, m_program_config),
                std.make_pair(AS_DATA,    m_data_config)
            };
        }


        // device_state_interface overrides
        void device_state_interface_state_import(device_state_entry entry)
        {
            switch (entry.index())
            {
                case STATE_GENFLAGS:
                    m_st = (m_debugger_flags & 0x01) != 0 ? (byte)1 : (byte)0;
                    m_zf = (m_debugger_flags & 0x02) != 0 ? (byte)1 : (byte)0;
                    m_cf = (m_debugger_flags & 0x04) != 0 ? (byte)1 : (byte)0;
                    m_vf = (m_debugger_flags & 0x08) != 0 ? (byte)1 : (byte)0;
                    m_sf = (m_debugger_flags & 0x10) != 0 ? (byte)1 : (byte)0;
                    m_nf = (m_debugger_flags & 0x20) != 0 ? (byte)1 : (byte)0;
                    break;

                case STATE_GENPC:
                case STATE_GENPCBASE:
                    m_PC = (byte)(m_debugger_pc & 0x3f);
                    m_PA = (byte)(( m_debugger_pc >> 6 ) & 0x1f);
                    break;
            }
        }

        void device_state_interface_state_export(device_state_entry entry)
        {
            switch (entry.index())
            {
                case STATE_GENFLAGS:
                    m_debugger_flags = 0;
                    if (TEST_ST() != 0) m_debugger_flags |= 0x01;
                    if (TEST_ZF() != 0) m_debugger_flags |= 0x02;
                    if (TEST_CF() != 0) m_debugger_flags |= 0x04;
                    if (TEST_VF() != 0) m_debugger_flags |= 0x08;
                    if (TEST_SF() != 0) m_debugger_flags |= 0x10;
                    if (TEST_NF() != 0) m_debugger_flags |= 0x20;
                    break;

                case STATE_GENPC:
                case STATE_GENPCBASE:
                    m_debugger_pc = GETPC();
                    break;
            }
        }

        void device_state_interface_state_string_export(device_state_entry entry, out string str)
        {
            str = "";
            switch (entry.index())
            {
                case STATE_GENFLAGS:
                    str = string.Format("{0}{1}{2}{3}{4}{5}",
                            TEST_ST() != 0 ? 'T' : 't',
                            TEST_ZF() != 0 ? 'Z' : 'z',
                            TEST_CF() != 0 ? 'C' : 'c',
                            TEST_VF() != 0 ? 'V' : 'v',
                            TEST_SF() != 0 ? 'S' : 's',
                            TEST_NF() != 0 ? 'I' : 'i');
                    break;
            }
        }


        // moved to device_disasm_interface_mb88
        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        //TIMER_CALLBACK_MEMBER( mb88_cpu_device::serial_timer )
        void serial_timer(object ptr, int param)
        {
            m_SBcount++;

            /* if we get too many interrupts with no servicing, disable the timer
               until somebody does something */
            if (m_SBcount >= SERIAL_DISABLE_THRESH)
                m_serial.adjust(attotime.never);

            /* only read if not full; this is needed by the Namco 52xx to ensure that
               the program can write to S and recover the value even if serial is enabled */
            if (m_sf == 0)
            {
                m_SB = (byte)((m_SB >> 1) | (m_read_si.op() != 0 ? 8 : 0));

                if (m_SBcount >= 4)
                {
                    m_sf = 1;
                    m_pending_interrupt |= INT_CAUSE_SERIAL;
                }
            }
        }

        int pla(int inA, int inB)
        {
            int index = ((inB&1) << 4) | (inA&0x0f);

            if (m_PLA != null)
                return m_PLA[index];

            return index;
        }

        void update_pio_enable( uint8_t newpio )
        {
            /* if the serial state has changed, configure the timer */
            if (((m_pio ^ newpio) & 0x30) != 0)
            {
                if ((newpio & 0x30) == 0)
                    m_serial.adjust(attotime.never);
                else if ((newpio & 0x30) == 0x20)
                    m_serial.adjust(attotime.from_hz(clock() / SERIAL_PRESCALE), 0, attotime.from_hz(clock() / SERIAL_PRESCALE));
                else
                    throw new emu_fatalerror("mb88xx: update_pio_enable set serial enable to unsupported value {0}\n", newpio & 0x30);
            }

            m_pio = newpio;
        }

        void increment_timer()
        {
            m_TL = (byte)((m_TL + 1) & 0x0f);
            if (m_TL == 0)
            {
                m_TH = (byte)((m_TH + 1) & 0x0f);
                if (m_TH == 0)
                {
                    m_vf = 1;
                    m_pending_interrupt |= INT_CAUSE_TIMER;
                }
            }
        }

        void update_pio( int cycles )
        {
            /* TODO: improve/validate serial and timer support */

            /* internal clock enable */
            if (( m_pio & 0x80 ) != 0)
            {
                m_TP += (byte)cycles;
                while (m_TP >= TIMER_PRESCALE)
                {
                    m_TP -= TIMER_PRESCALE;
                    increment_timer();
                }
            }

            /* process pending interrupts */
            if ((m_pending_interrupt & m_pio) != 0)
            {
                m_SP[m_SI] = GETPC();
                m_SP[m_SI] |= (UInt16)(TEST_CF() << 15);
                m_SP[m_SI] |= (UInt16)(TEST_ZF() << 14);
                m_SP[m_SI] |= (UInt16)(TEST_ST() << 13);
                m_SI = (byte)(( m_SI + 1 ) & 3);

                /* the datasheet doesn't mention interrupt vectors but
                the Arabian MCU program expects the following */
                if ((m_pending_interrupt & m_pio & INT_CAUSE_EXTERNAL) != 0)
                {
                    /* if we have a live external source, call the irqcallback */
                    standard_irq_callback( 0 );
                    m_PC = 0x02;
                }
                else if ((m_pending_interrupt & m_pio & INT_CAUSE_TIMER) != 0)
                {
                    m_PC = 0x04;
                }
                else if ((m_pending_interrupt & m_pio & INT_CAUSE_SERIAL) != 0)
                {
                    m_PC = 0x06;
                }

                m_PA = 0x00;
                m_st = 1;
                m_pending_interrupt = 0;

                CYCLES(3); /* ? */
            }
        }
    }


#if false
    class mb88201_cpu_device : public mb88_cpu_device
    {
    public:
        // construction/destruction
        mb88201_cpu_device(const machine_config &mconfig, const char *tag, device_t *owner, UINT32 clock);
    };
#endif


#if false
    class mb88202_cpu_device : public mb88_cpu_device
    {
    public:
        // construction/destruction
        mb88202_cpu_device(const machine_config &mconfig, const char *tag, device_t *owner, UINT32 clock);
    };
#endif


#if false
    class mb8841_cpu_device : public mb88_cpu_device
    {
    public:
        // construction/destruction
        mb8841_cpu_device(const machine_config &mconfig, const char *tag, device_t *owner, UINT32 clock);
    };
#endif


    class mb8842_cpu_device : mb88_cpu_device
    {
        //DEFINE_DEVICE_TYPE(MB8842,  mb8842_cpu_device,  "mb8842",  "Fujitsu MB8842")
        static device_t device_creator_mb8842_cpu_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mb8842_cpu_device(mconfig, tag, owner, clock); }
        public static readonly device_type MB8842 = DEFINE_DEVICE_TYPE(device_creator_mb8842_cpu_device, "mb8842",  "Fujitsu MB8842");


        // construction/destruction
        mb8842_cpu_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, MB8842, tag, owner, clock, 11, 7)
        {
        }
    }


    class mb8843_cpu_device : mb88_cpu_device
    {
        //DEFINE_DEVICE_TYPE(MB8843,  mb8843_cpu_device,  "mb8843",  "Fujitsu MB8843")
        static device_t device_creator_mb8843_cpu_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mb8843_cpu_device(mconfig, tag, owner, clock); }
        public static readonly device_type MB8843 = DEFINE_DEVICE_TYPE(device_creator_mb8843_cpu_device, "mb8843",  "Fujitsu MB8843");


        // construction/destruction
        mb8843_cpu_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, MB8843, tag, owner, clock, 10, 6)
        {
        }
    }


    class mb8844_cpu_device : mb88_cpu_device
    {
        //DEFINE_DEVICE_TYPE(MB8844,  mb8844_cpu_device,  "mb8844",  "Fujitsu MB8844")
        static device_t device_creator_mb8844_cpu_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mb8844_cpu_device(mconfig, tag, owner, clock); }
        public static readonly device_type MB8844 = DEFINE_DEVICE_TYPE(device_creator_mb8844_cpu_device, "mb8844", "Fujitsu MB8844");


        // construction/destruction
        mb8844_cpu_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, MB8844, tag, owner, clock, 10, 6)
        {
        }
    }
}
