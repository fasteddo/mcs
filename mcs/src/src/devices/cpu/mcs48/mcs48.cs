// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using space_config_vector = mame.std.vector<System.Collections.Generic.KeyValuePair<int, mame.address_space_config>>;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    public class mcs48_cpu_device : cpu_device
    {
        public class device_execute_interface_mcs48 : device_execute_interface
        {
            public device_execute_interface_mcs48(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint64_t execute_clocks_to_cycles(uint64_t clocks) { return (clocks + 15 - 1) / 15; }
            protected override uint64_t execute_cycles_to_clocks(uint64_t cycles) { return cycles * 15; }
            protected override uint32_t execute_min_cycles() { return 1; }
            protected override uint32_t execute_max_cycles() { return 3; }
            protected override uint32_t execute_input_lines() { return 2; }
            protected override void execute_run() { ((mcs48_cpu_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { ((mcs48_cpu_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
        }


        public class device_memory_interface_mcs48 : device_memory_interface
        {
            public device_memory_interface_mcs48(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((mcs48_cpu_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_mcs48 : device_state_interface
        {
            public device_state_interface_mcs48(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void state_import(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_export(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }
        }


        public class device_disasm_interface_mcs48 : device_disasm_interface
        {
            public device_disasm_interface_mcs48(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        public delegate int mcs48_ophandler(mcs48_cpu_device cpu);  //typedef int (mcs48_cpu_device::*mcs48_ophandler)();


        /***************************************************************************
            CONSTANTS
        ***************************************************************************/

        /* register access indexes */
        //enum
        //{
        const int MCS48_PC   = 0;
        const int MCS48_PSW  = 1;
        const int MCS48_A    = 2;
        const int MCS48_TC   = 3;
        const int MCS48_TPRE = 4;
        const int MCS48_P0   = 5;   // 8021/8022 only
        const int MCS48_P1   = 6;
        const int MCS48_P2   = 7;
        const int MCS48_R0   = 8;
        const int MCS48_R1   = 9;
        const int MCS48_R2   = 10;
        const int MCS48_R3   = 11;
        const int MCS48_R4   = 12;
        const int MCS48_R5   = 13;
        const int MCS48_R6   = 14;
        const int MCS48_R7   = 15;
        const int MCS48_EA   = 16;
        const int MCS48_STS  = 17;  // UPI-41 only
        const int MCS48_DBBO = 18; // UPI-41 only
        const int MCS48_DBBI = 19;  // UPI-41 only
        //};


        /* I/O port access indexes */
        //enum
        //{
        const int MCS48_INPUT_IRQ = 0;
        const int UPI41_INPUT_IBF = 0;
        const int MCS48_INPUT_EA  = 1;
        //};


        // 8243 expander operations
        enum expander_op
        {
            EXPANDER_OP_READ = 0,
            EXPANDER_OP_WRITE = 1,
            EXPANDER_OP_OR = 2,
            EXPANDER_OP_AND = 3
        }


        /* timer/counter enable bits */
        const uint8_t TIMER_ENABLED   = 0x01;
        const uint8_t COUNTER_ENABLED = 0x02;

        /* flag bits */
        const uint8_t C_FLAG          = 0x80;
        const uint8_t A_FLAG          = 0x40;
        const uint8_t F_FLAG          = 0x20;
        const uint8_t B_FLAG          = 0x10;

        /* status bits (UPI-41) */
        const uint8_t STS_F1          = 0x08;
        const uint8_t STS_F0          = 0x04;
        const uint8_t STS_IBF         = 0x02;
        const uint8_t STS_OBF         = 0x01;

        /* port 2 bits (UPI-41) */
        const uint8_t P2_OBF          = 0x10;
        const uint8_t P2_NIBF         = 0x20;
        const uint8_t P2_DRQ          = 0x40;
        const uint8_t P2_NDACK        = 0x80;

        /* enable bits (UPI-41) */
        //#define ENABLE_FLAGS    0x01
        //#define ENABLE_DMA      0x02

        /* feature masks */
        const uint8_t MB_FEATURE      = 0x01;
        const uint8_t EXT_BUS_FEATURE = 0x02;
        const uint8_t UPI41_FEATURE   = 0x04;
        const uint8_t I802X_FEATURE   = 0x08;
        protected const uint8_t I8048_FEATURE   = MB_FEATURE | EXT_BUS_FEATURE;


        /***************************************************************************
            MACROS
        ***************************************************************************/

        /* r0-r7 map to memory via the regptr */
        uint8_t R0 { get { return m_regptr[0]; } set { m_regptr[0] = value; } }
        uint8_t R1 { get { return m_regptr[1]; } set { m_regptr[1] = value; } }
        uint8_t R2 { get { return m_regptr[2]; } set { m_regptr[2] = value; } }
        uint8_t R3 { get { return m_regptr[3]; } set { m_regptr[3] = value; } }
        uint8_t R4 { get { return m_regptr[4]; } set { m_regptr[4] = value; } }
        uint8_t R5 { get { return m_regptr[5]; } set { m_regptr[5] = value; } }
        uint8_t R6 { get { return m_regptr[6]; } set { m_regptr[6] = value; } }
        uint8_t R7 { get { return m_regptr[7]; } set { m_regptr[7] = value; } }


        address_space_config m_program_config;
        address_space_config m_data_config;
        address_space_config m_io_config;

        devcb_read8.array<i2, devcb_read8> m_port_in_cb;
        devcb_write8.array<i2, devcb_write8> m_port_out_cb;
        devcb_read8 m_bus_in_cb;
        devcb_write8 m_bus_out_cb;

        devcb_read_line.array<i2, devcb_read_line> m_test_in_cb;
        clock_update_delegate m_t0_clk_func;
        devcb_write_line m_prog_out_cb;

        uint16_t      m_prevpc;             /* 16-bit previous program counter */
        uint16_t      m_pc;                 /* 16-bit program counter */

        uint8_t       m_a;                  /* 8-bit accumulator */
        ListPointer<uint8_t> m_regptr;  //uint8_t *     m_regptr;             /* pointer to r0-r7 */
        uint8_t       m_psw;                /* 8-bit psw */
        uint8_t       m_p1;                 /* 8-bit latched port 1 */
        uint8_t       m_p2;                 /* 8-bit latched port 2 */
        uint8_t       m_ea;                 /* 1-bit latched ea input */
        uint8_t       m_timer;              /* 8-bit timer */
        uint8_t       m_prescaler;          /* 5-bit timer prescaler */
        uint8_t       m_t1_history;         /* 8-bit history of the T1 input */
        uint8_t       m_sts;                /* 8-bit status register (UPI-41 only, except for F1) */
        uint8_t       m_dbbi;               /* 8-bit input data buffer (UPI-41 only) */
        uint8_t       m_dbbo;               /* 8-bit output data buffer (UPI-41 only) */

        bool          m_irq_state;          /* true if the IRQ line is active */
        bool          m_irq_polled;         /* true if last instruction was JNI (and not taken) */
        bool          m_irq_in_progress;    /* true if an IRQ is in progress */
        bool          m_timer_overflow;     /* true on a timer overflow; cleared by taking interrupt */
        bool          m_timer_flag;         /* true on a timer overflow; cleared on JTF */
        bool          m_tirq_enabled;       /* true if the timer IRQ is enabled */
        bool          m_xirq_enabled;       /* true if the external IRQ is enabled */
        uint8_t       m_timecount_enabled;  /* bitmask of timer/counter enabled */
        bool          m_flags_enabled;      /* true if I/O flags have been enabled (UPI-41 only) */
        bool          m_dma_enabled;        /* true if DMA has been enabled (UPI-41 only) */

        uint16_t      m_a11;                /* A11 value, either 0x000 or 0x800 */

        intref m_icountRef = new intref();  //int         m_icount;

        /* Memory spaces */
        address_space m_program;
        memory_access_cache m_cache;  //memory_access_cache<0, 0, ENDIANNESS_LITTLE> *m_cache;
        address_space m_data;
        address_space m_io;

        required_shared_ptr_uint8_t m_dataptr;

        uint8_t       m_feature_mask;       /* processor feature flags */
        uint16_t      m_int_rom_size;       /* internal rom size */

        uint8_t       m_rtemp;              /* temporary for import/export */


        /***************************************************************************
            OPCODE HANDLERS
        ***************************************************************************/

        //#define OPHANDLER(_name) int mcs48_cpu_device::_name()

        int illegal()
        {
            logerror("MCS-48 PC:{0} - Illegal opcode = {1}\n", m_pc - 1, program_r((offs_t)(m_pc - 1)));
            return 1;
        }

        int add_a_r0()       { execute_add(R0); return 1; }
        int add_a_r1()       { execute_add(R1); return 1; }
        int add_a_r2()       { execute_add(R2); return 1; }
        int add_a_r3()       { execute_add(R3); return 1; }
        int add_a_r4()       { execute_add(R4); return 1; }
        int add_a_r5()       { execute_add(R5); return 1; }
        int add_a_r6()       { execute_add(R6); return 1; }
        int add_a_r7()       { execute_add(R7); return 1; }
        int add_a_xr0()      { execute_add(ram_r(R0)); return 1; }
        int add_a_xr1()      { execute_add(ram_r(R1)); return 1; }
        int add_a_n()        { execute_add(argument_fetch()); return 2; }

        int adc_a_r0()       { execute_addc(R0); return 1; }
        int adc_a_r1()       { execute_addc(R1); return 1; }
        int adc_a_r2()       { execute_addc(R2); return 1; }
        int adc_a_r3()       { execute_addc(R3); return 1; }
        int adc_a_r4()       { execute_addc(R4); return 1; }
        int adc_a_r5()       { execute_addc(R5); return 1; }
        int adc_a_r6()       { execute_addc(R6); return 1; }
        int adc_a_r7()       { execute_addc(R7); return 1; }
        int adc_a_xr0()      { execute_addc(ram_r(R0)); return 1; }
        int adc_a_xr1()      { execute_addc(ram_r(R1)); return 1; }
        int adc_a_n()        { execute_addc(argument_fetch()); return 2; }

        int anl_a_r0()       { m_a &= R0; return 1; }
        int anl_a_r1()       { m_a &= R1; return 1; }
        int anl_a_r2()       { m_a &= R2; return 1; }
        int anl_a_r3()       { m_a &= R3; return 1; }
        int anl_a_r4()       { m_a &= R4; return 1; }
        int anl_a_r5()       { m_a &= R5; return 1; }
        int anl_a_r6()       { m_a &= R6; return 1; }
        int anl_a_r7()       { m_a &= R7; return 1; }
        int anl_a_xr0()      { m_a &= ram_r(R0); return 1; }
        int anl_a_xr1()      { m_a &= ram_r(R1); return 1; }
        int anl_a_n()        { m_a &= argument_fetch(); return 2; }

        int anl_bus_n()      { bus_w((uint8_t)(bus_r() & argument_fetch())); return 2; }
        int anl_p1_n()       { port_w(1, m_p1 &= argument_fetch()); return 2; }
        int anl_p2_n()       { port_w(2, m_p2 &= (uint8_t)(argument_fetch() | ~p2_mask())); return 2; }
        int anld_p4_a()      { expander_operation(expander_op.EXPANDER_OP_AND, 4); return 2; }
        int anld_p5_a()      { expander_operation(expander_op.EXPANDER_OP_AND, 5); return 2; }
        int anld_p6_a()      { expander_operation(expander_op.EXPANDER_OP_AND, 6); return 2; }
        int anld_p7_a()      { expander_operation(expander_op.EXPANDER_OP_AND, 7); return 2; }

        int call_0()         { execute_call((uint16_t)(argument_fetch() | 0x000)); return 2; }
        int call_1()         { execute_call((uint16_t)(argument_fetch() | 0x100)); return 2; }
        int call_2()         { execute_call((uint16_t)(argument_fetch() | 0x200)); return 2; }
        int call_3()         { execute_call((uint16_t)(argument_fetch() | 0x300)); return 2; }
        int call_4()         { execute_call((uint16_t)(argument_fetch() | 0x400)); return 2; }
        int call_5()         { execute_call((uint16_t)(argument_fetch() | 0x500)); return 2; }
        int call_6()         { execute_call((uint16_t)(argument_fetch() | 0x600)); return 2; }
        int call_7()         { execute_call((uint16_t)(argument_fetch() | 0x700)); return 2; }

        int clr_a()          { m_a = 0; return 1; }
        int clr_c()          { m_psw = (uint8_t)(m_psw & ~C_FLAG); return 1; }
        int clr_f0()         { m_psw = (uint8_t)(m_psw & ~F_FLAG); m_sts = (uint8_t)(m_sts & ~STS_F0); return 1; }
        int clr_f1()         { m_sts = (uint8_t)(m_sts & ~STS_F1); return 1; }

        int cpl_a()          { m_a ^= 0xff; return 1; }
        int cpl_c()          { m_psw ^= C_FLAG; return 1; }
        int cpl_f0()         { m_psw ^= F_FLAG; m_sts ^= STS_F0; return 1; }
        int cpl_f1()         { m_sts ^= STS_F1; return 1; }

        int da_a()
        {
            if ((m_a & 0x0f) > 0x09 || (m_psw & A_FLAG) != 0)
            {
                m_a += 0x06;
                if ((m_a & 0xf0) == 0x00)
                    m_psw |= C_FLAG;
            }
            if ((m_a & 0xf0) > 0x90 || (m_psw & C_FLAG) != 0)
            {
                m_a += 0x60;
                m_psw |= C_FLAG;
            }
            else
            {
                m_psw = (uint8_t)(m_psw & ~C_FLAG);
            }

            return 1;
        }

        int dec_a()          { m_a--; return 1; }
        int dec_r0()         { R0--; return 1; }
        int dec_r1()         { R1--; return 1; }
        int dec_r2()         { R2--; return 1; }
        int dec_r3()         { R3--; return 1; }
        int dec_r4()         { R4--; return 1; }
        int dec_r5()         { R5--; return 1; }
        int dec_r6()         { R6--; return 1; }
        int dec_r7()         { R7--; return 1; }

        int dis_i()          { m_xirq_enabled = false; return 1; }
        int dis_tcnti()      { m_tirq_enabled = false; m_timer_overflow = false; return 1; }

        int djnz_r0()        { execute_jcc(--R0 != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int djnz_r1()        { execute_jcc(--R1 != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int djnz_r2()        { execute_jcc(--R2 != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int djnz_r3()        { execute_jcc(--R3 != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int djnz_r4()        { execute_jcc(--R4 != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int djnz_r5()        { execute_jcc(--R5 != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int djnz_r6()        { execute_jcc(--R6 != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int djnz_r7()        { execute_jcc(--R7 != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }

        int en_i()           { m_xirq_enabled = true; return 1 + check_irqs(); }
        int en_tcnti()       { m_tirq_enabled = true; return 1 + check_irqs(); }
        int en_dma()         { m_dma_enabled = true; port_w(2, m_p2); return 1; }
        int en_flags()       { m_flags_enabled = true; port_w(2, m_p2); return 1; }
        int ent0_clk()
        {
            if (m_t0_clk_func != null)
                m_t0_clk_func(clock() / 3);
            else
                logerror("T0 clock enabled\n");
            return 1;
        }

        int in_a_p0()        { m_a = (uint8_t)(bus_r() & m_dbbo); return 2; }
        int in_a_p1()        { m_a = (uint8_t)(port_r(1) & m_p1); return 2; }
        int in_a_p2()        { m_a = (uint8_t)(port_r(2) & m_p2); return 2; }
        int ins_a_bus()      { m_a = bus_r(); return 2; }
        int in_a_dbb()
        {
            /* acknowledge the IBF IRQ and clear the bit in STS */
            if ((m_sts & STS_IBF) != 0)
                standard_irq_callback(UPI41_INPUT_IBF);
            m_sts = (uint8_t)(m_sts & ~STS_IBF);

            /* if P2 flags are enabled, update the state of P2 */
            if (m_flags_enabled && (m_p2 & P2_NIBF) == 0)
                port_w(2, m_p2 |= P2_NIBF);
            m_a = m_dbbi;

            return 2;
        }

        int inc_a()          { m_a++; return 1; }
        int inc_r0()         { R0++; return 1; }
        int inc_r1()         { R1++; return 1; }
        int inc_r2()         { R2++; return 1; }
        int inc_r3()         { R3++; return 1; }
        int inc_r4()         { R4++; return 1; }
        int inc_r5()         { R5++; return 1; }
        int inc_r6()         { R6++; return 1; }
        int inc_r7()         { R7++; return 1; }
        int inc_xr0()        { ram_w(R0, (uint8_t)(ram_r(R0) + 1)); return 1; }
        int inc_xr1()        { ram_w(R1, (uint8_t)(ram_r(R1) + 1)); return 1; }

        int jb_0()           { execute_jcc((m_a & 0x01) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jb_1()           { execute_jcc((m_a & 0x02) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jb_2()           { execute_jcc((m_a & 0x04) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jb_3()           { execute_jcc((m_a & 0x08) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jb_4()           { execute_jcc((m_a & 0x10) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jb_5()           { execute_jcc((m_a & 0x20) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jb_6()           { execute_jcc((m_a & 0x40) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jb_7()           { execute_jcc((m_a & 0x80) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jc()             { execute_jcc((m_psw & C_FLAG) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jf0()            { execute_jcc((m_psw & F_FLAG) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jf1()            { execute_jcc((m_sts & STS_F1) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jnc()            { execute_jcc((m_psw & C_FLAG) == 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jni()            { m_irq_polled = m_irq_state == false; execute_jcc(m_irq_state ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jnibf()          { m_irq_polled = (m_sts & STS_IBF) != 0; execute_jcc((m_sts & STS_IBF) == 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jnt_0()          { execute_jcc(test_r(0) == 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jnt_1()          { execute_jcc(test_r(1) == 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jnz()            { execute_jcc(m_a != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jobf()           { execute_jcc((m_sts & STS_OBF) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jtf()            { execute_jcc(m_timer_flag ? (uint8_t)1 : (uint8_t)0); m_timer_flag = false; return 2; }
        int jt_0()           { execute_jcc(test_r(0) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jt_1()           { execute_jcc(test_r(1) != 0 ? (uint8_t)1 : (uint8_t)0); return 2; }
        int jz()             { execute_jcc(m_a == 0 ? (uint8_t)1 : (uint8_t)0); return 2; }

        int jmp_0()          { execute_jmp((uint16_t)(argument_fetch() | 0x000)); return 2; }
        int jmp_1()          { execute_jmp((uint16_t)(argument_fetch() | 0x100)); return 2; }
        int jmp_2()          { execute_jmp((uint16_t)(argument_fetch() | 0x200)); return 2; }
        int jmp_3()          { execute_jmp((uint16_t)(argument_fetch() | 0x300)); return 2; }
        int jmp_4()          { execute_jmp((uint16_t)(argument_fetch() | 0x400)); return 2; }
        int jmp_5()          { execute_jmp((uint16_t)(argument_fetch() | 0x500)); return 2; }
        int jmp_6()          { execute_jmp((uint16_t)(argument_fetch() | 0x600)); return 2; }
        int jmp_7()          { execute_jmp((uint16_t)(argument_fetch() | 0x700)); return 2; }
        int jmpp_xa()        { m_pc &= 0xf00; m_pc |= program_r((offs_t)(m_pc | m_a)); return 2; }

        int mov_a_n()        { m_a = argument_fetch(); return 2; }
        int mov_a_psw()      { m_a = m_psw; return 1; }
        int mov_a_r0()       { m_a = R0; return 1; }
        int mov_a_r1()       { m_a = R1; return 1; }
        int mov_a_r2()       { m_a = R2; return 1; }
        int mov_a_r3()       { m_a = R3; return 1; }
        int mov_a_r4()       { m_a = R4; return 1; }
        int mov_a_r5()       { m_a = R5; return 1; }
        int mov_a_r6()       { m_a = R6; return 1; }
        int mov_a_r7()       { m_a = R7; return 1; }
        int mov_a_xr0()      { m_a = ram_r(R0); return 1; }
        int mov_a_xr1()      { m_a = ram_r(R1); return 1; }
        int mov_a_t()        { m_a = (uint8_t)(m_timer + ((m_timecount_enabled & TIMER_ENABLED) != 0 ? 1 : 0)); return 1; }

        int mov_psw_a()      { m_psw = m_a; update_regptr(); return 1; }
        int mov_sts_a()      { m_sts = (uint8_t)((m_sts & 0x0f) | (m_a & 0xf0)); return 1; }
        int mov_r0_a()       { R0 = m_a; return 1; }
        int mov_r1_a()       { R1 = m_a; return 1; }
        int mov_r2_a()       { R2 = m_a; return 1; }
        int mov_r3_a()       { R3 = m_a; return 1; }
        int mov_r4_a()       { R4 = m_a; return 1; }
        int mov_r5_a()       { R5 = m_a; return 1; }
        int mov_r6_a()       { R6 = m_a; return 1; }
        int mov_r7_a()       { R7 = m_a; return 1; }
        int mov_r0_n()       { R0 = argument_fetch(); return 2; }
        int mov_r1_n()       { R1 = argument_fetch(); return 2; }
        int mov_r2_n()       { R2 = argument_fetch(); return 2; }
        int mov_r3_n()       { R3 = argument_fetch(); return 2; }
        int mov_r4_n()       { R4 = argument_fetch(); return 2; }
        int mov_r5_n()       { R5 = argument_fetch(); return 2; }
        int mov_r6_n()       { R6 = argument_fetch(); return 2; }
        int mov_r7_n()       { R7 = argument_fetch(); return 2; }
        int mov_t_a()        { m_timer = m_a; return 1; }
        int mov_xr0_a()      { ram_w(R0, m_a); return 1; }
        int mov_xr1_a()      { ram_w(R1, m_a); return 1; }
        int mov_xr0_n()      { ram_w(R0, argument_fetch()); return 2; }
        int mov_xr1_n()      { ram_w(R1, argument_fetch()); return 2; }

        int movd_a_p4()      { expander_operation(expander_op.EXPANDER_OP_READ, 4); return 2; }
        int movd_a_p5()      { expander_operation(expander_op.EXPANDER_OP_READ, 5); return 2; }
        int movd_a_p6()      { expander_operation(expander_op.EXPANDER_OP_READ, 6); return 2; }
        int movd_a_p7()      { expander_operation(expander_op.EXPANDER_OP_READ, 7); return 2; }
        int movd_p4_a()      { expander_operation(expander_op.EXPANDER_OP_WRITE, 4); return 2; }
        int movd_p5_a()      { expander_operation(expander_op.EXPANDER_OP_WRITE, 5); return 2; }
        int movd_p6_a()      { expander_operation(expander_op.EXPANDER_OP_WRITE, 6); return 2; }
        int movd_p7_a()      { expander_operation(expander_op.EXPANDER_OP_WRITE, 7); return 2; }

        int movp_a_xa()      { m_a = program_r((offs_t)((m_pc & 0xf00) | m_a)); return 2; }
        int movp3_a_xa()     { m_a = program_r((offs_t)(0x300 | m_a)); return 2; }

        int movx_a_xr0()     { m_a = ext_r(R0); return 2; }
        int movx_a_xr1()     { m_a = ext_r(R1); return 2; }
        int movx_xr0_a()     { ext_w(R0, m_a); return 2; }
        int movx_xr1_a()     { ext_w(R1, m_a); return 2; }

        int nop()            { return 1; }

        int orl_a_r0()       { m_a |= R0; return 1; }
        int orl_a_r1()       { m_a |= R1; return 1; }
        int orl_a_r2()       { m_a |= R2; return 1; }
        int orl_a_r3()       { m_a |= R3; return 1; }
        int orl_a_r4()       { m_a |= R4; return 1; }
        int orl_a_r5()       { m_a |= R5; return 1; }
        int orl_a_r6()       { m_a |= R6; return 1; }
        int orl_a_r7()       { m_a |= R7; return 1; }
        int orl_a_xr0()      { m_a |= ram_r(R0); return 1; }
        int orl_a_xr1()      { m_a |= ram_r(R1); return 1; }
        int orl_a_n()        { m_a |= argument_fetch(); return 2; }

        int orl_bus_n()      { bus_w((uint8_t)(bus_r() | argument_fetch())); return 2; }
        int orl_p1_n()       { port_w(1, m_p1 |= argument_fetch()); return 2; }
        int orl_p2_n()       { port_w(2, m_p2 |= (uint8_t)(argument_fetch() & p2_mask())); return 2; }
        int orld_p4_a()      { expander_operation(expander_op.EXPANDER_OP_OR, 4); return 2; }
        int orld_p5_a()      { expander_operation(expander_op.EXPANDER_OP_OR, 5); return 2; }
        int orld_p6_a()      { expander_operation(expander_op.EXPANDER_OP_OR, 6); return 2; }
        int orld_p7_a()      { expander_operation(expander_op.EXPANDER_OP_OR, 7); return 2; }

        int outl_bus_a()     { bus_w(m_a); return 2; }
        int outl_p0_a()      { bus_w(m_dbbo = m_a); return 2; }
        int outl_p1_a()      { port_w(1, m_p1 = m_a); return 2; }
        int outl_p2_a()      { uint8_t mask = p2_mask(); port_w(2, m_p2 = (uint8_t)((m_p2 & ~mask) | (m_a & mask))); return 2; }
        int out_dbb_a()
        {
            /* copy to the DBBO and update the bit in STS */
            m_dbbo = m_a;
            m_sts |= STS_OBF;

            /* if P2 flags are enabled, update the state of P2 */
            if (m_flags_enabled && (m_p2 & P2_OBF) == 0)
                port_w(2, m_p2 |= P2_OBF);

            return 2;
        }

        int ret()            { pull_pc(); return 2; }
        int retr()
        {
            pull_pc_psw();

            /* implicitly clear the IRQ in progress flip flop and re-check interrupts */
            m_irq_in_progress = false;
            return 2 + check_irqs();
        }

        int rl_a()           { m_a = (uint8_t)((m_a << 1) | (m_a >> 7)); return 1; }
        int rlc_a()          { uint8_t newc = (uint8_t)(m_a & C_FLAG); m_a = (uint8_t)((m_a << 1) | (m_psw >> 7)); m_psw = (uint8_t)((m_psw & ~C_FLAG) | newc); return 1; }

        int rr_a()           { m_a = (uint8_t)((m_a >> 1) | (m_a << 7)); return 1; }
        int rrc_a()          { uint8_t newc = (uint8_t)((m_a << 7) & C_FLAG); m_a = (uint8_t)((m_a >> 1) | (m_psw & C_FLAG)); m_psw = (uint8_t)((m_psw & ~C_FLAG) | newc); return 1; }

        int sel_mb0()        { m_a11 = 0x000; return 1; }
        int sel_mb1()        { m_a11 = 0x800; return 1; }

        int sel_rb0()        { m_psw = (uint8_t)(m_psw & ~B_FLAG); update_regptr(); return 1; }
        int sel_rb1()        { m_psw |=  B_FLAG; update_regptr(); return 1; }

        int stop_tcnt()      { m_timecount_enabled = 0; return 1; }

        int strt_cnt()       { m_timecount_enabled = COUNTER_ENABLED; m_t1_history = (uint8_t)test_r(1); return 1; }
        int strt_t()         { m_timecount_enabled = TIMER_ENABLED; m_prescaler = 0; return 1; }

        int swap_a()         { m_a = (uint8_t)((m_a << 4) | (m_a >> 4)); return 1; }

        int xch_a_r0()       { uint8_t tmp = m_a; m_a = R0; R0 = tmp; return 1; }
        int xch_a_r1()       { uint8_t tmp = m_a; m_a = R1; R1 = tmp; return 1; }
        int xch_a_r2()       { uint8_t tmp = m_a; m_a = R2; R2 = tmp; return 1; }
        int xch_a_r3()       { uint8_t tmp = m_a; m_a = R3; R3 = tmp; return 1; }
        int xch_a_r4()       { uint8_t tmp = m_a; m_a = R4; R4 = tmp; return 1; }
        int xch_a_r5()       { uint8_t tmp = m_a; m_a = R5; R5 = tmp; return 1; }
        int xch_a_r6()       { uint8_t tmp = m_a; m_a = R6; R6 = tmp; return 1; }
        int xch_a_r7()       { uint8_t tmp = m_a; m_a = R7; R7 = tmp; return 1; }
        int xch_a_xr0()      { uint8_t tmp = m_a; m_a = ram_r(R0); ram_w(R0, tmp); return 1; }
        int xch_a_xr1()      { uint8_t tmp = m_a; m_a = ram_r(R1); ram_w(R1, tmp); return 1; }

        int xchd_a_xr0()     { uint8_t oldram = ram_r(R0); ram_w(R0, (uint8_t)((oldram & 0xf0) | (m_a & 0x0f))); m_a = (uint8_t)((m_a & 0xf0) | (oldram & 0x0f)); return 1; }
        int xchd_a_xr1()     { uint8_t oldram = ram_r(R1); ram_w(R1, (uint8_t)((oldram & 0xf0) | (m_a & 0x0f))); m_a = (uint8_t)((m_a & 0xf0) | (oldram & 0x0f)); return 1; }

        int xrl_a_r0()       { m_a ^= R0; return 1; }
        int xrl_a_r1()       { m_a ^= R1; return 1; }
        int xrl_a_r2()       { m_a ^= R2; return 1; }
        int xrl_a_r3()       { m_a ^= R3; return 1; }
        int xrl_a_r4()       { m_a ^= R4; return 1; }
        int xrl_a_r5()       { m_a ^= R5; return 1; }
        int xrl_a_r6()       { m_a ^= R6; return 1; }
        int xrl_a_r7()       { m_a ^= R7; return 1; }
        int xrl_a_xr0()      { m_a ^= ram_r(R0); return 1; }
        int xrl_a_xr1()      { m_a ^= ram_r(R1); return 1; }
        int xrl_a_n()        { m_a ^= argument_fetch(); return 2; }


        /***************************************************************************
            OPCODE TABLES
        ***************************************************************************/

        static string OP(string _a) { return _a; }  //define OP(_a) &mcs48_cpu_device::_a

        protected static mcs48_ophandler [] s_mcs48_opcodes = new mcs48_ophandler [256];

        void init_s_mcs48_opcodes()
        {
            string [] mcs48_opcodes = new string [256]
            {
                OP("nop"),        OP("illegal"),    OP("outl_bus_a"),OP("add_a_n"),   OP("jmp_0"),     OP("en_i"),       OP("illegal"),   OP("dec_a"),         /* 00 */
                OP("ins_a_bus"),  OP("in_a_p1"),    OP("in_a_p2"),   OP("illegal"),   OP("movd_a_p4"), OP("movd_a_p5"),  OP("movd_a_p6"), OP("movd_a_p7"),
                OP("inc_xr0"),    OP("inc_xr1"),    OP("jb_0"),      OP("adc_a_n"),   OP("call_0"),    OP("dis_i"),      OP("jtf"),       OP("inc_a"),         /* 10 */
                OP("inc_r0"),     OP("inc_r1"),     OP("inc_r2"),    OP("inc_r3"),    OP("inc_r4"),    OP("inc_r5"),     OP("inc_r6"),    OP("inc_r7"),
                OP("xch_a_xr0"),  OP("xch_a_xr1"),  OP("illegal"),   OP("mov_a_n"),   OP("jmp_1"),     OP("en_tcnti"),   OP("jnt_0"),     OP("clr_a"),         /* 20 */
                OP("xch_a_r0"),   OP("xch_a_r1"),   OP("xch_a_r2"),  OP("xch_a_r3"),  OP("xch_a_r4"),  OP("xch_a_r5"),   OP("xch_a_r6"),  OP("xch_a_r7"),
                OP("xchd_a_xr0"), OP("xchd_a_xr1"), OP("jb_1"),      OP("illegal"),   OP("call_1"),    OP("dis_tcnti"),  OP("jt_0"),      OP("cpl_a"),         /* 30 */
                OP("illegal"),    OP("outl_p1_a"),  OP("outl_p2_a"), OP("illegal"),   OP("movd_p4_a"), OP("movd_p5_a"),  OP("movd_p6_a"), OP("movd_p7_a"),
                OP("orl_a_xr0"),  OP("orl_a_xr1"),  OP("mov_a_t"),   OP("orl_a_n"),   OP("jmp_2"),     OP("strt_cnt"),   OP("jnt_1"),     OP("swap_a"),        /* 40 */
                OP("orl_a_r0"),   OP("orl_a_r1"),   OP("orl_a_r2"),  OP("orl_a_r3"),  OP("orl_a_r4"),  OP("orl_a_r5"),   OP("orl_a_r6"),  OP("orl_a_r7"),
                OP("anl_a_xr0"),  OP("anl_a_xr1"),  OP("jb_2"),      OP("anl_a_n"),   OP("call_2"),    OP("strt_t"),     OP("jt_1"),      OP("da_a"),          /* 50 */
                OP("anl_a_r0"),   OP("anl_a_r1"),   OP("anl_a_r2"),  OP("anl_a_r3"),  OP("anl_a_r4"),  OP("anl_a_r5"),   OP("anl_a_r6"),  OP("anl_a_r7"),
                OP("add_a_xr0"),  OP("add_a_xr1"),  OP("mov_t_a"),   OP("illegal"),   OP("jmp_3"),     OP("stop_tcnt"),  OP("illegal"),   OP("rrc_a"),         /* 60 */
                OP("add_a_r0"),   OP("add_a_r1"),   OP("add_a_r2"),  OP("add_a_r3"),  OP("add_a_r4"),  OP("add_a_r5"),   OP("add_a_r6"),  OP("add_a_r7"),
                OP("adc_a_xr0"),  OP("adc_a_xr1"),  OP("jb_3"),      OP("illegal"),   OP("call_3"),    OP("ent0_clk"),   OP("jf1"),       OP("rr_a"),          /* 70 */
                OP("adc_a_r0"),   OP("adc_a_r1"),   OP("adc_a_r2"),  OP("adc_a_r3"),  OP("adc_a_r4"),  OP("adc_a_r5"),   OP("adc_a_r6"),  OP("adc_a_r7"),
                OP("movx_a_xr0"), OP("movx_a_xr1"), OP("illegal"),   OP("ret"),       OP("jmp_4"),     OP("clr_f0"),     OP("jni"),       OP("illegal"),       /* 80 */
                OP("orl_bus_n"),  OP("orl_p1_n"),   OP("orl_p2_n"),  OP("illegal"),   OP("orld_p4_a"), OP("orld_p5_a"),  OP("orld_p6_a"), OP("orld_p7_a"),
                OP("movx_xr0_a"), OP("movx_xr1_a"), OP("jb_4"),      OP("retr"),      OP("call_4"),    OP("cpl_f0"),     OP("jnz"),       OP("clr_c"),         /* 90 */
                OP("anl_bus_n"),  OP("anl_p1_n"),   OP("anl_p2_n"),  OP("illegal"),   OP("anld_p4_a"), OP("anld_p5_a"),  OP("anld_p6_a"), OP("anld_p7_a"),
                OP("mov_xr0_a"),  OP("mov_xr1_a"),  OP("illegal"),   OP("movp_a_xa"), OP("jmp_5"),     OP("clr_f1"),     OP("illegal"),   OP("cpl_c"),         /* A0 */
                OP("mov_r0_a"),   OP("mov_r1_a"),   OP("mov_r2_a"),  OP("mov_r3_a"),  OP("mov_r4_a"),  OP("mov_r5_a"),   OP("mov_r6_a"),  OP("mov_r7_a"),
                OP("mov_xr0_n"),  OP("mov_xr1_n"),  OP("jb_5"),      OP("jmpp_xa"),   OP("call_5"),    OP("cpl_f1"),     OP("jf0"),       OP("illegal"),       /* B0 */
                OP("mov_r0_n"),   OP("mov_r1_n"),   OP("mov_r2_n"),  OP("mov_r3_n"),  OP("mov_r4_n"),  OP("mov_r5_n"),   OP("mov_r6_n"),  OP("mov_r7_n"),
                OP("illegal"),    OP("illegal"),    OP("illegal"),   OP("illegal"),   OP("jmp_6"),     OP("sel_rb0"),    OP("jz"),        OP("mov_a_psw"),     /* C0 */
                OP("dec_r0"),     OP("dec_r1"),     OP("dec_r2"),    OP("dec_r3"),    OP("dec_r4"),    OP("dec_r5"),     OP("dec_r6"),    OP("dec_r7"),
                OP("xrl_a_xr0"),  OP("xrl_a_xr1"),  OP("jb_6"),      OP("xrl_a_n"),   OP("call_6"),    OP("sel_rb1"),    OP("illegal"),   OP("mov_psw_a"),     /* D0 */
                OP("xrl_a_r0"),   OP("xrl_a_r1"),   OP("xrl_a_r2"),  OP("xrl_a_r3"),  OP("xrl_a_r4"),  OP("xrl_a_r5"),   OP("xrl_a_r6"),  OP("xrl_a_r7"),
                OP("illegal"),    OP("illegal"),    OP("illegal"),   OP("movp3_a_xa"),OP("jmp_7"),     OP("sel_mb0"),    OP("jnc"),       OP("rl_a"),          /* E0 */
                OP("djnz_r0"),    OP("djnz_r1"),    OP("djnz_r2"),   OP("djnz_r3"),   OP("djnz_r4"),   OP("djnz_r5"),    OP("djnz_r6"),   OP("djnz_r7"),
                OP("mov_a_xr0"),  OP("mov_a_xr1"),  OP("jb_7"),      OP("illegal"),   OP("call_7"),    OP("sel_mb1"),    OP("jc"),        OP("rlc_a"),         /* F0 */
                OP("mov_a_r0"),   OP("mov_a_r1"),   OP("mov_a_r2"),  OP("mov_a_r3"),  OP("mov_a_r4"),  OP("mov_a_r5"),   OP("mov_a_r6"),  OP("mov_a_r7")
            };

            // https://www.red-gate.com/simple-talk/blogs/introduction-to-open-instance-delegates/
            for (int i = 0; i < mcs48_opcodes.Length; i++)
            {
                System.Reflection.MethodInfo methodInfo = typeof(mcs48_cpu_device).GetMethod(mcs48_opcodes[i], System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                s_mcs48_opcodes[i] = (mcs48_ophandler)Delegate.CreateDelegate(typeof(mcs48_ophandler), null, methodInfo);
            }
        }


        //static const mcs48_ophandler s_upi41_opcodes[256];
        //static const mcs48_ophandler s_i8021_opcodes[256];
        //static const mcs48_ophandler s_i8022_opcodes[256];

        mcs48_ophandler [] m_opcode_table;


        device_memory_interface m_dimemory;
        device_execute_interface m_diexec;
        device_state_interface m_distate;


        // construction/destruction
        protected mcs48_cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, int rom_size, int ram_size, uint8_t feature_mask, mcs48_ophandler [] opcode_table)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_mcs48(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_mcs48(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_mcs48(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_mcs48(mconfig, this));


            init_s_mcs48_opcodes();


            m_program_config = new address_space_config("program", endianness_t.ENDIANNESS_LITTLE, 8, (feature_mask & MB_FEATURE) != 0 ? (u8)12 : (u8)11, 0
                               , (rom_size == 1024) ? program_10bit : (rom_size == 2048) ? program_11bit : (rom_size == 4096) ? program_12bit : (address_map_constructor)null);
            m_data_config = new address_space_config("data", endianness_t.ENDIANNESS_LITTLE, 8, ( ( ram_size == 64 ) ? (u8)6 : ( ( ram_size == 128 ) ? (u8)7 : (u8)8 ) ), 0
                            , (ram_size == 64) ? data_6bit : (ram_size == 128) ? data_7bit : (address_map_constructor)data_8bit);
            m_io_config = new address_space_config("io", endianness_t.ENDIANNESS_LITTLE, 8, 8, 0);
            m_port_in_cb = new devcb_read8.array<i2, devcb_read8>(this, () => { return new devcb_read8(this); });
            m_port_out_cb = new devcb_write8.array<i2, devcb_write8>(this, () => { return new devcb_write8(this); });
            m_bus_in_cb = new devcb_read8(this);
            m_bus_out_cb = new devcb_write8(this);
            m_test_in_cb = new devcb_read_line.array<i2, devcb_read_line>(this, () => { return new devcb_read_line(this); });
            m_t0_clk_func = null;
            m_prog_out_cb = new devcb_write_line(this);
            m_psw = 0;
            m_dataptr = new required_shared_ptr_uint8_t(this, "data");
            m_feature_mask = feature_mask;
            m_int_rom_size = (uint16_t)rom_size;
            m_opcode_table = opcode_table;


            // Sanity checks
            if ( ram_size != 64 && ram_size != 128 && ram_size != 256 )
                fatalerror("mcs48: Invalid RAM size\n");

            if ( rom_size != 0 && rom_size != 1024 && rom_size != 2048 && rom_size != 4096 )
                fatalerror("mcs48: Invalid ROM size\n");
        }


        public uint8_t feature_mask { get { return m_feature_mask; } }


        //// 8243 expander operations
        //enum expander_op
        //{
        //    EXPANDER_OP_READ = 0,
        //    EXPANDER_OP_WRITE = 1,
        //    EXPANDER_OP_OR = 2,
        //    EXPANDER_OP_AND = 3
        //};

        // configuration
        //auto p1_in_cb() { return m_port_in_cb[0].bind(); }
        public devcb_read.binder p2_in_cb() { return m_port_in_cb[1].bind(); }
        public devcb_write.binder p1_out_cb() { return m_port_out_cb[0].bind(); }
        public devcb_write.binder p2_out_cb() { return m_port_out_cb[1].bind(); }
        public devcb_read.binder bus_in_cb() { return m_bus_in_cb.bind(); }
        public devcb_write.binder bus_out_cb() { return m_bus_out_cb.bind(); }
        public devcb_read.binder t0_in_cb() { return m_test_in_cb[0].bind(); }
        public devcb_read.binder t1_in_cb() { return m_test_in_cb[1].bind(); }

        // PROG line to 8243 expander
        //auto prog_out_cb() { return m_prog_out_cb.bind(); }

        //uint8_t p1_r() { return m_p1; }
        //uint8_t p2_r() { return m_p2; }


        /***************************************************************************
            ADDRESS MAPS
        ***************************************************************************/

        /* FIXME: the memory maps should probably support rom banking for EA */
        void program_10bit(address_map map, device_t owner)
        {
            map.op(0x000, 0x3ff).rom();
        }

        void program_11bit(address_map map, device_t owner)
        {
            map.op(0x000, 0x7ff).rom();
        }

        void program_12bit(address_map map, device_t owner)
        {
            map.op(0x000, 0xfff).rom();
        }

        void data_6bit(address_map map, device_t owner)
        {
            map.op(0x00, 0x3f).ram().share("data");
        }

        void data_7bit(address_map map, device_t owner)
        {
            map.op(0x00, 0x7f).ram().share("data");
        }

        void data_8bit(address_map map, device_t owner)
        {
            map.op(0x00, 0xff).ram().share("data");
        }


        //template <typename... T> void set_t0_clk_cb(T &&... args) { m_t0_clk_func.set(std::forward<T>(args)...); }


        // device-level overrides

        protected override void device_start()
        {
            m_dimemory = GetClassInterface<device_memory_interface>();
            m_diexec = GetClassInterface<device_execute_interface>();
            m_distate = GetClassInterface<device_state_interface>();


            /* External access line
             * EA=1 : read from external rom
             * EA=0 : read from internal rom
             */

            m_a = 0;
            m_timer = 0;
            m_prescaler = 0;
            m_t1_history = 0;
            m_dbbi = 0;
            m_dbbo = 0;
            m_irq_state = false;
            m_irq_polled = false;

            /* FIXME: Current implementation suboptimal */
            m_ea = m_int_rom_size != 0 ? (uint8_t)0 : (uint8_t)1;

            m_program = m_dimemory.space(AS_PROGRAM);
            m_cache = m_program.cache(0, 0, (int)endianness_t.ENDIANNESS_LITTLE);
            m_data = m_dimemory.space(AS_DATA);
            m_io = (m_feature_mask & EXT_BUS_FEATURE) != 0 ? m_dimemory.space(AS_IO) : null;

            // resolve callbacks
            m_port_in_cb.resolve_all_safe(0xff);
            m_port_out_cb.resolve_all_safe();
            m_bus_in_cb.resolve_safe(0xff);
            m_bus_out_cb.resolve_safe();
            m_test_in_cb.resolve_all_safe(0);
            m_prog_out_cb.resolve_safe();

            /* set up the state table */
            {
                m_distate.state_add(MCS48_PC,        "PC",        m_pc).mask(0xfff);
                m_distate.state_add(STATE_GENPC,     "GENPC",     m_pc).mask(0xfff).noshow();
                m_distate.state_add(STATE_GENPCBASE, "CURPC",     m_prevpc).mask(0xfff).noshow();
                m_distate.state_add(STATE_GENSP,     "GENSP",     m_psw).mask(0x7).noshow();
                m_distate.state_add(STATE_GENFLAGS,  "GENFLAGS",  m_psw).noshow().formatstr("%11s");
                m_distate.state_add(MCS48_A,         "A",         m_a);
                m_distate.state_add(MCS48_TC,        "TC",        m_timer);
                m_distate.state_add(MCS48_TPRE,      "TPRE",      m_prescaler).mask(0x1f);

                if ((m_feature_mask & I802X_FEATURE) != 0)
                    m_distate.state_add(MCS48_P0,    "P0",        m_dbbo);
                m_distate.state_add(MCS48_P1,        "P1",        m_p1);
                m_distate.state_add(MCS48_P2,        "P2",        m_p2);

                for (int regnum = 0; regnum < 8; regnum++)
                {
                    m_distate.state_add(MCS48_R0 + regnum, string_format("R{0}", regnum).c_str(), m_rtemp).callimport().callexport();
                }

                if ((m_feature_mask & EXT_BUS_FEATURE) != 0)
                    m_distate.state_add(MCS48_EA,    "EA",        m_ea).mask(0x1);

                if ((m_feature_mask & UPI41_FEATURE) != 0)
                {
                    m_distate.state_add(MCS48_STS,   "STS",       m_sts);
                    m_distate.state_add(MCS48_DBBI,  "DBBI",      m_dbbi);
                    m_distate.state_add(MCS48_DBBO,  "DBBO",      m_dbbo);
                }
            }

            /* ensure that regptr is valid before get_info gets called */
            update_regptr();

            save_item(m_prevpc, "m_prevpc");
            save_item(m_pc, "m_pc");

            save_item(m_a, "m_a");
            save_item(m_psw, "m_psw");
            save_item(m_p1, "m_p1");
            save_item(m_p2, "m_p2");
            save_item(m_ea, "m_ea");
            save_item(m_timer, "m_timer");
            save_item(m_prescaler, "m_prescaler");
            save_item(m_t1_history, "m_t1_history");
            save_item(m_sts, "m_sts");
            save_item(m_dbbi, "m_dbbi");
            save_item(m_dbbo, "m_dbbo");

            save_item(m_irq_state, "m_irq_state");
            save_item(m_irq_polled, "m_irq_polled");
            save_item(m_irq_in_progress, "m_irq_in_progress");
            save_item(m_timer_overflow, "m_timer_overflow");
            save_item(m_timer_flag, "m_timer_flag");
            save_item(m_tirq_enabled, "m_tirq_enabled");
            save_item(m_xirq_enabled, "m_xirq_enabled");
            save_item(m_timecount_enabled, "m_timecount_enabled");
            save_item(m_flags_enabled, "m_flags_enabled");
            save_item(m_dma_enabled, "m_dma_enabled");

            save_item(m_a11, "m_a11");

            set_icountptr(m_icountRef);
        }


        protected override void device_stop()
        {
            if (m_io != null) m_io.Dispose();
            if (m_data != null) m_data.Dispose();
            if (m_cache != null) m_cache.Dispose();
            if (m_program != null) m_program.Dispose();
        }


        protected override void device_config_complete()
        {
            //m_t0_clk_func.resolve();
            if (m_t0_clk_func != null)
                m_t0_clk_func(clock() / 3);
        }


        protected override void device_reset()
        {
            /* confirmed from reset description */
            m_pc = 0;
            m_psw = (uint8_t)((m_psw & (C_FLAG | A_FLAG)) | 0x08);
            m_a11 = 0x000;
            m_dbbo = 0xff;
            bus_w(0xff);
            m_p1 = 0xff;
            m_p2 = 0xff;
            port_w(1, m_p1);
            port_w(2, m_p2);
            m_tirq_enabled = false;
            m_xirq_enabled = false;
            m_timecount_enabled = 0;
            m_timer_flag = false;
            m_sts = 0;
            m_flags_enabled = false;
            m_dma_enabled = false;
            if (m_t0_clk_func != null)
                m_t0_clk_func(0);

            /* confirmed from interrupt logic description */
            m_irq_in_progress = false;
            m_timer_overflow = false;
        }


        // device_execute_interface overrides
        //virtual uint64_t execute_clocks_to_cycles(uint64_t clocks) const override { return (clocks + 15 - 1) / 15; }
        //virtual uint64_t execute_cycles_to_clocks(uint64_t cycles) const override { return (cycles * 15); }
        //virtual uint32_t execute_min_cycles() const override { return 1; }
        //virtual uint32_t execute_max_cycles() const override { return 3; }
        //virtual uint32_t execute_input_lines() const override { return 2; }

        void device_execute_interface_execute_run()
        {
            int curcycles;

            update_regptr();

            /* external interrupts may have been set since we last checked */
            curcycles = check_irqs();
            m_icountRef.i -= curcycles;
            if (m_timecount_enabled != 0)
                burn_cycles(curcycles);

            /* iterate over remaining cycles, guaranteeing at least one instruction */
            do
            {
                uint32_t opcode;

                /* fetch next opcode */
                m_prevpc = m_pc;
                m_irq_polled = false;
                debugger_instruction_hook(m_pc);
                opcode = opcode_fetch();

                /* process opcode and count cycles */
                curcycles = m_opcode_table[opcode](this);

                /* burn the cycles */
                m_icountRef.i -= curcycles;
                if (m_timecount_enabled != 0)
                    burn_cycles(curcycles);

            } while (m_icountRef.i > 0);
        }

        void device_execute_interface_execute_set_input(int inputnum, int state)
        {
            switch (inputnum)
            {
                case MCS48_INPUT_IRQ:
                    m_irq_state = (state != CLEAR_LINE);
                    break;

                case MCS48_INPUT_EA:
                    m_ea = (state != CLEAR_LINE) ? (uint8_t)1 : (uint8_t)0;
                    break;
            }
        }


        // device_memory_interface overrides
        space_config_vector device_memory_interface_memory_space_config()
        {
            if ((feature_mask & EXT_BUS_FEATURE) != 0)
                return new space_config_vector()
                {
                    std.make_pair(AS_PROGRAM, m_program_config),
                    std.make_pair(AS_DATA,    m_data_config),
                    std.make_pair(AS_IO,      m_io_config)
                };
            else
                return new space_config_vector()
                {
                    std.make_pair(AS_PROGRAM, m_program_config),
                    std.make_pair(AS_DATA,    m_data_config)
                };
        }


        // device_state_interface overrides
        //virtual void state_import(const device_state_entry &entry) override;
        //virtual void state_export(const device_state_entry &entry) override;
        //virtual void state_string_export(const device_state_entry &entry, std::string &str) const override;

        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        /* ROM is mapped to AS_PROGRAM */
        uint8_t program_r(offs_t a)         { return m_program.read_byte(a); }

        /* RAM is mapped to AS_DATA */
        uint8_t ram_r(offs_t a)             { return m_data.read_byte(a); }
        void    ram_w(offs_t a, uint8_t v)  { m_data.write_byte(a, v); }

        /* ports are mapped to AS_IO and callbacks */
        uint8_t ext_r(offs_t a)             { return m_io.read_byte(a); }
        void    ext_w(offs_t a, uint8_t v)  { m_io.write_byte(a, v); }
        uint8_t port_r(offs_t a)            { return m_port_in_cb[a - 1].op(); }
        void    port_w(offs_t a, uint8_t v) { m_port_out_cb[a - 1].op(v); }
        int     test_r(offs_t a)            { return m_test_in_cb[a].op(); }
        uint8_t bus_r()                     { return m_bus_in_cb.op(); }
        void    bus_w(uint8_t v)            { m_bus_out_cb.op(v); }
        void    prog_w(int v)               { m_prog_out_cb.op(v); }


        /*-------------------------------------------------
            opcode_fetch - fetch an opcode byte
        -------------------------------------------------*/
        uint8_t opcode_fetch()
        {
            uint16_t address = m_pc;
            m_pc = (uint16_t)(((m_pc + 1) & 0x7ff) | (m_pc & 0x800));
            return m_cache.read_byte(address);
        }


        /*-------------------------------------------------
            argument_fetch - fetch an opcode argument
            byte
        -------------------------------------------------*/
        uint8_t argument_fetch()
        {
            uint16_t address = m_pc;
            m_pc = (uint16_t)(((m_pc + 1) & 0x7ff) | (m_pc & 0x800));
            return m_cache.read_byte(address);
        }


        /*-------------------------------------------------
            update_regptr - update the regptr member to
            point to the appropriate register bank
        -------------------------------------------------*/
        void update_regptr()
        {
            m_regptr = new ListPointer<u8>(m_dataptr.target, (m_psw & B_FLAG) != 0 ? 24 : 0);  //m_regptr = &m_dataptr[(m_psw & B_FLAG) ? 24 : 0];
        }


        /*-------------------------------------------------
            push_pc_psw - push the m_pc and m_psw values onto
            the stack
        -------------------------------------------------*/
        void push_pc_psw()
        {
            uint8_t sp = (uint8_t)(m_psw & 0x07);
            ram_w((offs_t)(8 + 2 * sp), (uint8_t)m_pc);
            ram_w((offs_t)(9 + 2 * sp), (uint8_t)(((m_pc >> 8) & 0x0f) | (m_psw & 0xf0)));
            m_psw = (uint8_t)((m_psw & 0xf8) | ((sp + 1) & 0x07));
        }


        /*-------------------------------------------------
            pull_pc_psw - pull the PC and PSW values from
            the stack
        -------------------------------------------------*/
        void pull_pc_psw()
        {
            uint8_t sp = (uint8_t)((m_psw - 1) & 0x07);
            m_pc = ram_r((offs_t)(8 + 2 * sp));
            m_pc |= (uint16_t)(ram_r((offs_t)(9 + 2 * sp)) << 8);
            m_psw = (uint8_t)(((m_pc >> 8) & 0xf0) | 0x08 | sp);
            m_pc &= 0xfff;
            update_regptr();
        }


        /*-------------------------------------------------
            pull_pc - pull the PC value from the stack,
            leaving the upper part of PSW intact
        -------------------------------------------------*/
        void pull_pc()
        {
            uint8_t sp = (uint8_t)((m_psw - 1) & 0x07);
            m_pc = ram_r((offs_t)(8 + 2 * sp));
            m_pc |= (uint16_t)(ram_r((offs_t)(9 + 2 * sp)) << 8);
            m_pc &= 0xfff;
            m_psw = (uint8_t)((m_psw & 0xf0) | 0x08 | sp);
        }


        /*-------------------------------------------------
            execute_add - perform the logic of an ADD
            instruction
        -------------------------------------------------*/
        void execute_add(uint8_t dat)
        {
            uint16_t temp = (uint16_t)(m_a + dat);
            uint16_t temp4 = (uint16_t)((m_a & 0x0f) + (dat & 0x0f));

            m_psw = (uint8_t)(m_psw & ~(C_FLAG | A_FLAG));
            m_psw |= (uint8_t)((temp4 << 2) & A_FLAG);
            m_psw |= (uint8_t)((temp >> 1) & C_FLAG);
            m_a = (uint8_t)temp;
        }


        /*-------------------------------------------------
            execute_addc - perform the logic of an ADDC
            instruction
        -------------------------------------------------*/
        void execute_addc(uint8_t dat)
        {
            uint8_t carryin = (uint8_t)((m_psw & C_FLAG) >> 7);
            uint16_t temp = (uint16_t)(m_a + dat + carryin);
            uint16_t temp4 = (uint16_t)((m_a & 0x0f) + (dat & 0x0f) + carryin);

            m_psw = (uint8_t)(m_psw & ~(C_FLAG | A_FLAG));
            m_psw |= (uint8_t)((temp4 << 2) & A_FLAG);
            m_psw |= (uint8_t)((temp >> 1) & C_FLAG);
            m_a = (uint8_t)temp;
        }


        /*-------------------------------------------------
            execute_jmp - perform the logic of a JMP
            instruction
        -------------------------------------------------*/
        void execute_jmp(uint16_t address)
        {
            uint16_t a11 = (m_irq_in_progress) ? (uint16_t)0 : (uint16_t)m_a11;
            m_pc = (uint16_t)(address | a11);
        }


        /*-------------------------------------------------
            execute_call - perform the logic of a CALL
            instruction
        -------------------------------------------------*/
        void execute_call(uint16_t address)
        {
            push_pc_psw();
            execute_jmp(address);
        }


        /*-------------------------------------------------
            execute_jcc - perform the logic of a
            conditional jump instruction
        -------------------------------------------------*/
        void execute_jcc(uint8_t result)
        {
            uint8_t offset = argument_fetch();
            if (result != 0)
                m_pc = (uint16_t)(((m_pc - 1) & 0xf00) | offset);
        }


        /*-------------------------------------------------
            p2_mask - return the mask of bits that the
            code can directly affect
        -------------------------------------------------*/
        uint8_t p2_mask()
        {
            uint8_t result = 0xff;
            if ((m_feature_mask & UPI41_FEATURE) == 0)
                return result;
            if (m_flags_enabled)
                result = (uint8_t)(m_psw & ~(P2_OBF | P2_NIBF));
            if (m_dma_enabled)
                result = (uint8_t)(m_psw & ~(P2_DRQ | P2_NDACK));
            return result;
        }


        /*-------------------------------------------------
            expander_operation - perform an operation via
            the 8243 expander chip
        -------------------------------------------------*/
        void expander_operation(expander_op operation, uint8_t port)
        {
            // put opcode on low 4 bits of P2 (overwriting latch)
            port_w(2, m_p2 = (uint8_t)((m_p2 & 0xf0) | ((uint8_t)operation << 2) | (port & 3)));

            // generate high-to-low transition on PROG line
            prog_w(0);

            // transfer data on low 4 bits of P2
            if (operation != expander_op.EXPANDER_OP_READ)
            {
                port_w(2, m_p2 = (uint8_t)((m_p2 & 0xf0) | (m_a & 0x0f)));
            }
            else
            {
                // place P20-P23 in input mode
                port_w(2, m_p2 |= 0x0f);

                // input data to lower 4 bits of A (upper 4 bits are cleared)
                m_a = (uint8_t)(port_r(2) & 0x0f);
            }

            // generate low-to-high transition on PROG line
            prog_w(1);
        }


        /*-------------------------------------------------
            check_irqs - check for and process IRQs
        -------------------------------------------------*/
        int check_irqs()
        {
            /* if something is in progress, we do nothing */
            if (m_irq_in_progress)
                return 0;

            /* external interrupts take priority */
            if ((m_irq_state || (m_sts & STS_IBF) != 0) && m_xirq_enabled)
            {
                m_irq_in_progress = true;

                // force JNI to be taken (hack)
                if (m_irq_polled)
                    m_pc = (uint16_t)(((m_pc - 1) & 0xf00) | m_cache.read_byte(m_pc - 1U));

                /* transfer to location 0x03 */
                push_pc_psw();
                m_pc = 0x03;

                /* indicate we took the external IRQ */
                standard_irq_callback(0);
                return 2;
            }

            /* timer overflow interrupts follow */
            if (m_timer_overflow && m_tirq_enabled)
            {
                m_irq_in_progress = true;

                /* transfer to location 0x07 */
                push_pc_psw();
                m_pc = 0x07;

                /* timer overflow flip-flop is reset once taken */
                m_timer_overflow = false;
                return 2;
            }

            return 0;
        }


        /*-------------------------------------------------
            burn_cycles - burn cycles, processing timers
            and counters
        -------------------------------------------------*/
        void burn_cycles(int count)
        {
            bool timerover = false;

            /* if the timer is enabled, accumulate prescaler cycles */
            if ((m_timecount_enabled & TIMER_ENABLED) != 0)
            {
                uint8_t oldtimer = m_timer;
                m_prescaler += (uint8_t)count;
                m_timer += (uint8_t)(m_prescaler >> 5);
                m_prescaler &= 0x1f;
                timerover = (oldtimer != 0 && m_timer == 0);
            }

            /* if the counter is enabled, poll the T1 test input once for each cycle */
            else if ((m_timecount_enabled & COUNTER_ENABLED) != 0)
            {
                for ( ; count > 0; count--)
                {
                    m_t1_history = (uint8_t)((m_t1_history << 1) | (test_r(1) & 1));
                    if ((m_t1_history & 3) == 2)
                        timerover = (++m_timer == 0);
                }
            }

            /* if either source caused a timer overflow, set the flags and check IRQs */
            if (timerover)
            {
                m_timer_flag = true;

                /* according to the docs, if an overflow occurs with interrupts disabled, the overflow is not stored */
                if (m_tirq_enabled)
                {
                    m_timer_overflow = true;
                    check_irqs();
                }
            }
        }
    }


    class mb8884_device : mcs48_cpu_device
    {
        //DEFINE_DEVICE_TYPE(MB8884, mb8884_device, "mb8884", "MB8884")
        static device_t device_creator_mb8884_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new mb8884_device(mconfig, tag, owner, clock); }
        public static readonly device_type MB8884 = DEFINE_DEVICE_TYPE(device_creator_mb8884_device, "mb8884", "MB8884");

        // construction/destruction
        mb8884_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, MB8884, tag, owner, clock, 0, 64, I8048_FEATURE, s_mcs48_opcodes)
        {
        }
    }
}
