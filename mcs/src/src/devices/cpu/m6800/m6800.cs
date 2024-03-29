// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using endianness_t = mame.util.endianness;  //using endianness_t = util::endianness;
using int16_t = System.Int16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.cpp_global;
using static mame.device_global;
using static mame.diexec_global;
using static mame.distate_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.m6800_global;


namespace mame
{
    public static partial class m6800_global
    {
        //enum
        //{
        public const int M6800_IRQ_LINE = 0;              /* IRQ line number */
        //}

        //enum
        //{
        public const int M6802_IRQ_LINE = M6800_IRQ_LINE;
        //};

        //enum
        //{
        public const int M6808_IRQ_LINE = M6800_IRQ_LINE;
        //};
    }


    public partial class m6800_cpu_device : cpu_device
    {
        //DEFINE_DEVICE_TYPE(M6800, m6800_cpu_device, "m6800", "Motorola MC6800")
        public static readonly emu.detail.device_type_impl M6800 = DEFINE_DEVICE_TYPE("m6800", "Motorola MC6800", (type, mconfig, tag, owner, clock) => { return new m6800_cpu_device(mconfig, tag, owner, clock); });


        protected class device_execute_interface_m6800 : device_execute_interface
        {
            public device_execute_interface_m6800(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint32_t execute_min_cycles() { return 1; }
            protected override uint32_t execute_max_cycles() { return 12; }
            protected override uint32_t execute_input_lines() { return 2; }
            protected override bool execute_input_edge_triggered(int inputnum) { return ((m6800_cpu_device)device()).device_execute_interface_execute_input_edge_triggered(inputnum); }
            protected override void execute_run() { ((m6800_cpu_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int irqline, int state) { ((m6800_cpu_device)device()).device_execute_interface_execute_set_input(irqline, state); }
        }


        protected class device_memory_interface_m6800 : device_memory_interface
        {
            public device_memory_interface_m6800(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((m6800_cpu_device)device()).device_memory_interface_memory_space_config(); }
        }


        protected class device_state_interface_m6800 : device_state_interface
        {
            public device_state_interface_m6800(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }
        }


        protected class device_disasm_interface_m6800 : device_disasm_interface
        {
            public device_disasm_interface_m6800(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        public delegate void op_func(m6800_cpu_device cpu);  //typedef void (m6800_cpu_device::*op_func)();


        const int VERBOSE = 0;  //#define VERBOSE 0

        public void LOG(string format, params object [] args) { if (VERBOSE != 0) logerror(format, args); }  //#define LOG(x)  do { if (VERBOSE) logerror x; } while (0)


        //enum
        //{
        const int M6800_PC = 1;
        const int M6800_S  = 2;
        const int M6800_A  = 3;
        const int M6800_B  = 4;
        const int M6800_X  = 5;
        const int M6800_CC = 6;
        const int M6800_WAI_STATE = 7;
        //}


        //enum
        //{
        const uint8_t M6800_WAI = 8;          /* set when WAI is waiting for an interrupt */
        protected const uint8_t M6800_SLP = 0x10;       /* HD63701 only */
        //}


        PAIR pPPC { get { return m_ppc; } set { m_ppc = value; } }
        PAIR pPC { get { return m_pc; } set { m_pc = value; } }
        //#define pS      m_s
        PAIR pX { get { return m_x; } set { m_x = value; } }
        //#define pD      m_d

        uint16_t PC { get { return m_pc.w.l; } set { m_pc.w.l = value; } }
        uint32_t PCD { get { return m_pc.d; } set { m_pc.d = value; } }
        uint16_t S { get { return m_s.w.l; } set { m_s.w.l = value; } }
        uint32_t SD { get { return m_s.d; } }
        uint16_t X { get { return m_x.w.l; } set { m_x.w.l = value; } }
        uint16_t D { get { return m_d.w.l; } set { m_d.w.l = value; } }
        uint8_t A { get { return m_d.b.h; } set { m_d.b.h = value; } }
        uint8_t B { get { return m_d.b.l; } set { m_d.b.l = value; } }
        uint8_t CC { get { return m_cc; } set { m_cc = value; } }

        uint32_t EAD { get { return m_ea.d; } set { m_ea.d = value; } }
        uint16_t EA { get { return m_ea.w.l; } set { m_ea.w.l = value; } }


        /* memory interface */

        /****************************************************************************/
        /* Read a byte from given memory location                                   */
        /****************************************************************************/
        uint32_t RM(uint32_t Addr) { return m_program.read_byte(Addr); }

        /****************************************************************************/
        /* Write a byte to given memory location                                    */
        /****************************************************************************/
        void WM(uint32_t Addr, uint8_t Value) { m_program.write_byte(Addr, Value); }

        /****************************************************************************/
        /* M6800_RDOP() is identical to M6800_RDMEM() except it is used for reading */
        /* opcodes. In case of system with memory mapped I/O, this function can be  */
        /* used to greatly speed up emulation                                       */
        /****************************************************************************/
        uint32_t M_RDOP(uint32_t Addr) { return m_copcodes.read_byte(Addr); }

        /****************************************************************************/
        /* M6800_RDOP_ARG() is identical to M6800_RDOP() but it's used for reading  */
        /* opcode arguments. This difference can be used to support systems that    */
        /* use different encoding mechanisms for opcodes and opcode arguments       */
        /****************************************************************************/
        uint32_t M_RDOP_ARG(uint32_t Addr) { return m_cprogram.read_byte(Addr); }


        /* macros to access memory */
        void IMMBYTE(out uint8_t b) { b = (uint8_t)M_RDOP_ARG(PCD); PC++; }
        void IMMBYTE(out uint16_t b) { b = (uint16_t)M_RDOP_ARG(PCD); PC++; }
        void IMMBYTE(out uint32_t b) { b = M_RDOP_ARG(PCD); PC++; }
        void IMMWORD(out PAIR w) { w = default; w.d = (M_RDOP_ARG(PCD) << 8) | M_RDOP_ARG((PCD + 1) & 0xffff); PC += 2; }

        void PUSHBYTE(uint8_t b) { WM(SD, b); --S; }
        void PUSHWORD(PAIR w) { WM(SD, w.b.l); --S; WM(SD, w.b.h); --S; }
        void PULLBYTE(out uint8_t b) { S++; b = (uint8_t)RM(SD); }
        void PULLWORD(out PAIR w) { S++; w = default; w.d = RM(SD) << 8; S++; w.d |= RM(SD); }

        // helpers because properties don't work with 'out'
        void IMMBYTE_A() { IMMBYTE(out uint8_t temp); A = temp; }
        void IMMBYTE_B() { IMMBYTE(out uint8_t temp); B = temp; }
        void IMMBYTE_EAD() { IMMBYTE(out uint8_t temp); EAD = temp; }
        void PULLBYTE_A() { PULLBYTE(out uint8_t temp); A = temp; }
        void PULLBYTE_B() { PULLBYTE(out uint8_t temp); B = temp; }
        void PULLBYTE_CC() { PULLBYTE(out uint8_t temp); CC = temp; }
        void PULLWORD_pPC() { PULLWORD(out PAIR temp); pPC = temp; }
        void PULLWORD_pX() { PULLWORD(out PAIR temp); pX = temp; }


        /* operate one instruction for */
        void ONE_MORE_INSN()
        {
            uint8_t ireg;
            pPPC = pPC;
            debugger_instruction_hook(PCD);
            ireg = (uint8_t)M_RDOP(PCD);
            PC++;
            this.m_insn[ireg](this);
            increment_counter(m_cycles[ireg]);
        }


        /* CC masks                       HI NZVC
                                        7654 3210   */
        void CLR_HNZVC() { CC &= 0xd0; }
        void CLR_NZV()   { CC &= 0xf1; }
        //#define CLR_HNZC    CC&=0xd2
        void CLR_NZVC()  { CC &= 0xf0; }
        void CLR_Z()     { CC &= 0xfb; }
        //#define CLR_ZC      CC&=0xfa
        void CLR_C()     { CC &= 0xfe; }


        /* macros for CC -- CC bits affected should be reset before calling */
        void SET_Z(uint32_t a) { if (a == 0) SEZ(); }
        void SET_Z8(uint32_t a) { SET_Z((uint8_t)a); }
        void SET_Z16(uint32_t a) { SET_Z((uint16_t)a); }
        void SET_N8(uint32_t a) { CC |= (uint8_t)((a & 0x80) >> 4); }
        void SET_N16(uint32_t a) { CC |= (uint8_t)((a & 0x8000) >> 12); }
        void SET_H(uint32_t a, uint32_t b, uint32_t r) { CC |= (uint8_t)(((a ^ b ^ r) & 0x10) << 1); }
        void SET_C8(uint32_t a) { CC |= (uint8_t)((a & 0x100) >> 8); }
        void SET_C16(uint32_t a) { CC |= (uint8_t)((a & 0x10000) >> 16); }
        void SET_V8(uint32_t a, uint32_t b, uint32_t r) { CC |= (uint8_t)(((a ^ b ^ r ^ (r >> 1)) & 0x80) >> 6); }
        void SET_V16(uint32_t a, uint32_t b, uint32_t r) { CC |= (uint8_t)(((a ^ b ^ r ^ (r >> 1)) & 0x8000) >> 14); }


        void SET_FLAGS8I(uint8_t a)      { CC |= flags8i[a & 0xff]; }
        void SET_FLAGS8D(uint8_t a)      { CC |= flags8d[a & 0xff]; }

        /* combos */
        void SET_NZ8(uint32_t a) { SET_N8(a); SET_Z8(a); }
        void SET_NZ16(uint32_t a) { SET_N16(a); SET_Z16(a); }
        void SET_FLAGS8(uint32_t a, uint32_t b, uint32_t r) { SET_N8(r); SET_Z8(r); SET_V8(a,b,r); SET_C8(r); }
        void SET_FLAGS16(uint32_t a, uint32_t b, uint32_t r) { SET_N16(r); SET_Z16(r); SET_V16(a,b,r); SET_C16(r); }


        /* for treating an uint8_t as a signed int16_t */
        int16_t SIGNED(uint8_t b) { return (int16_t)((b & 0x80) != 0 ? b | 0xff00 : b); }


        /* Macros for addressing modes */
        void DIRECT() { IMMBYTE_EAD(); }
        void IMM8() { EA = PC++; }
        void IMM16() { EA = PC; PC += 2; }
        void EXTENDED() { IMMWORD(out m_ea); }
        void INDEXED() { EA = (uint16_t)(X + (uint8_t)M_RDOP_ARG(PCD)); PC++; }


        /* macros to set status flags */
        //#if defined(SEC)
        //#undef SEC
        //#endif
        void SEC() { CC |= 0x01; }
        void CLC() { CC &= 0xfe; }
        void SEZ() { CC |= 0x04; }
        //#define CLZ CC&=0xfb
        //#define SEN CC|=0x08
        //#define CLN CC&=0xf7
        void SEV() { CC |= 0x02; }
        void CLV() { CC &= 0xfd; }
        //#define SEH CC|=0x20
        //#define CLH CC&=0xdf
        void SEI() { CC |= 0x10; }
        void CLI() { CC = (uint8_t)(CC & ~0x10); }  //{ CC&=~0x10; }


        /* macros for convenience */
        void DIRBYTE(out uint8_t b) { DIRECT(); b = (uint8_t)RM(EAD); }
        void DIRBYTE(out uint16_t b) { DIRECT(); b = (uint16_t)RM(EAD); }
        void DIRWORD(out PAIR w) { DIRECT(); w = default; w.d = RM16(EAD); }
        void EXTBYTE(out uint8_t b) { EXTENDED(); b = (uint8_t)RM(EAD); }
        void EXTBYTE(out uint16_t b) { EXTENDED(); b = (uint16_t)RM(EAD); }
        void EXTWORD(out PAIR w) { EXTENDED(); w = default; w.d = RM16(EAD); }

        void IDXBYTE(out uint8_t b)  { INDEXED(); b = (uint8_t)RM(EAD); }
        void IDXBYTE(out uint16_t b) { INDEXED(); b = (uint16_t)RM(EAD); }
        void IDXWORD(out PAIR w) { INDEXED(); w = default; w.d = RM16(EAD); }

        // helpers because properties don't work with 'out'
        void DIRBYTE_A() { DIRBYTE(out uint8_t temp); A = temp; }
        void DIRBYTE_B() { DIRBYTE(out uint8_t temp); B = temp; }
        void EXTBYTE_A() { EXTBYTE(out uint8_t temp); A = temp; }
        void EXTBYTE_B() { EXTBYTE(out uint8_t temp); B = temp; }
        void IDXBYTE_A() { IDXBYTE(out uint8_t temp); A = temp; }
        void IDXBYTE_B() { IDXBYTE(out uint8_t temp); B = temp; }

        /* Macros for branch instructions */
        void BRANCH(out uint8_t t, bool f) { IMMBYTE(out t); if (f) { PC = (uint16_t)(PC + SIGNED(t)); } }  //PC += SIGNED(t);
        uint8_t NXORV() { return (uint8_t)((CC & 0x08) ^ ((CC & 0x02) << 2)); }
        uint8_t NXORC() { return (uint8_t)((CC & 0x08) ^ ((CC & 0x01) << 3)); }


        address_space_config m_program_config;
        address_space_config m_decrypted_opcodes_config;
        //address_space_config m_io_config;

        PAIR m_ppc;            /* Previous program counter */
        PAIR m_pc;             /* Program counter */
        PAIR m_s;              /* Stack pointer */
        PAIR m_x;              /* Index register */
        PAIR m_d;              /* Accumulators */
        uint8_t m_cc;             /* Condition codes */
        uint8_t m_wai_state;      /* WAI opcode state ,(or sleep opcode state) */
        uint8_t m_nmi_state;      /* NMI line state */
        uint8_t m_nmi_pending;    /* NMI pending */
        protected uint8_t [] m_irq_state = new uint8_t [4];   /* IRQ line state [IRQ1,TIN,SC1,IS] */

        /* Memory spaces */
        memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache m_cprogram = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache();  //memory_access<16, 0, 0, ENDIANNESS_BIG>::cache m_cprogram;
        memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache m_copcodes = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache();  //memory_access<16, 0, 0, ENDIANNESS_BIG>::cache m_copcodes;
        memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific m_program = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific();  //memory_access<16, 0, 0, ENDIANNESS_BIG>::specific m_program;

        op_func [] m_insn;
        uint8_t [] m_cycles;            /* clock cycle of instruction table */

        intref m_icount = new intref();  //int m_icount;

        PAIR m_ea;        /* effective address */


        static readonly uint8_t [] flags8i = new uint8_t [256]  /* increment */
        {
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x0a, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08
        };

        static readonly uint8_t [] flags8d = new uint8_t [256]  /* decrement */
        {
            0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x02,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08,
            0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08, 0x08
        };


        protected static readonly uint8_t [] cycles_6800 = new uint8_t [256];
        //static const uint8_t cycles_nsc8105[256];
        protected static readonly op_func [] m6800_insn = new op_func [256];
        //static const op_func nsc8105_insn[256];


        protected device_execute_interface_m6800 m_diexec;
        protected device_memory_interface_m6800 m_dimemory;
        protected device_state_interface_m6800 m_distate;


        // construction/destruction
        protected m6800_cpu_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, M6800, tag, owner, clock, m6800_insn, cycles_6800, null)
        {
            m_class_interfaces.Add(new device_execute_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_m6800(mconfig, this));

            m_diexec = GetClassInterface<device_execute_interface_m6800>();
            m_dimemory = GetClassInterface<device_memory_interface_m6800>();
            m_distate = GetClassInterface<device_state_interface_m6800>();
        }


        protected m6800_cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, op_func [] insn, uint8_t [] cycles, address_map_constructor internal_) 
            : base(mconfig, type, tag, owner, clock)
        {
            m_program_config = new address_space_config("program", ENDIANNESS_BIG, 8, 16, 0, internal_);
            m_decrypted_opcodes_config = new address_space_config("program", ENDIANNESS_BIG, 8, 16, 0);
            m_insn = insn;
            m_cycles = cycles;
        }


        protected uint8_t cc { get { return m_cc; } }
        protected uint8_t wai_state { get { return m_wai_state; } set { m_wai_state = value; } }
        protected uint8_t [] irq_state { get { return m_irq_state; } }


        // device-level overrides
        protected override void device_start()
        {
            m_dimemory.space(AS_PROGRAM).cache(m_cprogram);
            m_dimemory.space(m_dimemory.has_space(AS_OPCODES) ? AS_OPCODES : AS_PROGRAM).cache(m_copcodes);
            m_dimemory.space(AS_PROGRAM).specific(m_program);

            m_pc.d = 0;
            m_s.d = 0;
            m_x.d = 0;
            m_d.d = 0;
            m_cc = 0;
            m_wai_state = 0;
            m_irq_state[0] = 0;
            m_irq_state[1] = 0;
            m_irq_state[2] = 0;

            save_item(NAME(new { m_ppc.w.l }));
            save_item(NAME(new { m_pc.w.l }));
            save_item(NAME(new { m_s.w.l }));
            save_item(NAME(new { m_x.w.l }));
            save_item(NAME(new { m_d.w.l }));
            save_item(NAME(new { m_cc }));
            save_item(NAME(new { m_wai_state }));
            save_item(NAME(new { m_nmi_state }));
            save_item(NAME(new { m_nmi_pending }));
            save_item(NAME(new { m_irq_state }));

            m_distate.state_add( M6800_A,         "A", m_d.b.h).formatstr("%02X");
            m_distate.state_add( M6800_B,         "B", m_d.b.l).formatstr("%02X");
            m_distate.state_add( M6800_PC,        "PC", m_pc.w.l).formatstr("%04X");
            m_distate.state_add( M6800_S,         "S", m_s.w.l).formatstr("%04X");
            m_distate.state_add( M6800_X,         "X", m_x.w.l).formatstr("%04X");
            m_distate.state_add( M6800_CC,        "CC", m_cc).formatstr("%02X");
            m_distate.state_add( M6800_WAI_STATE, "WAI", m_wai_state).formatstr("%01X");

            m_distate.state_add( STATE_GENPC, "GENPC", m_pc.w.l).noshow();
            m_distate.state_add( STATE_GENPCBASE, "CURPC", m_pc.w.l).noshow();
            m_distate.state_add( STATE_GENFLAGS, "GENFLAGS", m_cc).formatstr("%8s").noshow();

            set_icountptr(m_icount);
        }


        protected override void device_stop()
        {
        }


        protected override void device_reset()
        {
            m_cc = 0xc0;
            SEI();                /* IRQ disabled */
            PCD = RM16(0xfffe);

            m_wai_state = 0;
            m_nmi_state = 0;
            m_nmi_pending = 0;
            m_irq_state[M6800_IRQ_LINE] = 0;
        }


        // device_execute_interface overrides
        //virtual uint32_t execute_min_cycles() const override { return 1; }
        //virtual uint32_t execute_max_cycles() const override { return 12; }
        //virtual uint32_t execute_input_lines() const override { return 2; }
        bool device_execute_interface_execute_input_edge_triggered(int inputnum) { return inputnum == INPUT_LINE_NMI; }

        void device_execute_interface_execute_run()
        {
            check_irq_lines();

            cleanup_counters();

            do
            {
                if ((m_wai_state & (M6800_WAI | M6800_SLP)) != 0)
                {
                    eat_cycles();
                }
                else
                {
                    pPPC = pPC;
                    debugger_instruction_hook(PCD);
                    uint8_t ireg = (uint8_t)M_RDOP(PCD);
                    PC++;
                    m_insn[ireg](this);
                    increment_counter(m_cycles[ireg]);
                }
            } while (m_icount.i > 0);
        }

        protected void device_execute_interface_execute_set_input(int irqline, int state)
        {
            switch (irqline)
            {
            case INPUT_LINE_NMI:
                if (m_nmi_state == 0 && state != CLEAR_LINE)
                    m_nmi_pending = 1;

                m_nmi_state = (uint8_t)state;
                break;

            default:
                LOG("set_irq_line {0},{1}\n", irqline, state);
                m_irq_state[irqline] = (uint8_t)state;
                break;
            }
        }


        // device_memory_interface overrides
        space_config_vector device_memory_interface_memory_space_config()
        {
            if (memory().has_configured_map(AS_OPCODES))
            {
                return new space_config_vector
                {
                    std.make_pair(AS_PROGRAM, m_program_config),
                    std.make_pair(AS_OPCODES, m_decrypted_opcodes_config)
                };
            }
            else
            {
                return new space_config_vector
                {
                    std.make_pair(AS_PROGRAM, m_program_config)
                };
            }
        }


        // device_state_interface overrides
        //virtual void state_string_export(const device_state_entry &entry, std::string &str) const override;

        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        uint32_t RM16(uint32_t Addr)
        {
            uint32_t result = RM(Addr) << 8;
            return result | RM((Addr + 1) & 0xffff);
        }


        void WM16(uint32_t Addr, PAIR p)
        {
            WM(Addr, p.b.h);
            WM((Addr + 1) & 0xffff, p.b.l);
        }


        /* IRQ enter */
        protected void enter_interrupt(string message, uint16_t irq_vector)
        {
            int cycles_to_eat = 0;

            LOG(message);

            if ((m_wai_state & (M6800_WAI | M6800_SLP)) != 0)
            {
                if ((m_wai_state & M6800_WAI) != 0)
                    cycles_to_eat = 4;

                m_wai_state = (uint8_t)(m_wai_state & ~(M6800_WAI | M6800_SLP));
            }
            else
            {
                PUSHWORD(pPC);
                PUSHWORD(pX);
                PUSHBYTE(A);
                PUSHBYTE(B);
                PUSHBYTE(CC);
                cycles_to_eat = 12;
            }

            SEI();
            PCD = RM16(irq_vector);

            if (cycles_to_eat > 0)
                increment_counter(cycles_to_eat);
        }


        protected virtual void m6800_check_irq2() { }


        /* check the IRQ lines for pending interrupts */
        public void check_irq_lines()
        {
            // TODO: IS3 interrupt

            if (m_nmi_pending != 0)
            {
                if ((m_wai_state & M6800_SLP) != 0)
                    m_wai_state = (uint8_t)(m_wai_state & ~M6800_SLP);  //m_wai_state &= ~M6800_SLP;

                m_nmi_pending = 0;
                enter_interrupt("take NMI\n", 0xfffc);
            }
            else
            {
                if (m_irq_state[M6800_IRQ_LINE] != CLEAR_LINE)
                {
                    /* standard IRQ */
                    if ((m_wai_state & M6800_SLP) != 0)
                        m_wai_state = (uint8_t)(m_wai_state & ~M6800_SLP);  //m_wai_state &= ~M6800_SLP;

                    if ((CC & 0x10) == 0)
                    {
                        enter_interrupt("take IRQ1\n", 0xfff8);
                        standard_irq_callback(M6800_IRQ_LINE);
                    }
                }
                else
                {
                    if ((CC & 0x10) == 0)
                        m6800_check_irq2();
                }
            }
        }


        public virtual void increment_counter(int amount)
        {
            m_icount.i -= amount;
        }


        public virtual void eat_cycles() { throw new emu_unimplemented(); }
        public virtual void cleanup_counters() { }
        protected virtual void take_trap() { }
    }


    public class m6802_cpu_device : m6800_cpu_device
    {
        //DEFINE_DEVICE_TYPE(M6802, m6802_cpu_device, "m6802", "Motorola MC6802")
        public static readonly emu.detail.device_type_impl M6802 = DEFINE_DEVICE_TYPE("m6802", "Motorola MC6802", (type, mconfig, tag, owner, clock) => { return new m6802_cpu_device(mconfig, tag, owner, clock); });


        protected class device_execute_interface_m6802 : device_execute_interface_m6800
        {
            public device_execute_interface_m6802(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint64_t execute_clocks_to_cycles(uint64_t clocks) { return ((m6802_cpu_device)device()).device_execute_interface_execute_clocks_to_cycles(clocks); }
            protected override uint64_t execute_cycles_to_clocks(uint64_t cycles) { return ((m6802_cpu_device)device()).device_execute_interface_execute_cycles_to_clocks(cycles); }
        }


        new device_execute_interface_m6802 m_diexec;


        bool m_ram_enable;


        protected m6802_cpu_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, M6802, tag, owner, clock, m6800_insn, cycles_6800)
        {
            m_class_interfaces.Add(new device_execute_interface_m6802(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_m6800(mconfig, this));

            m_diexec = GetClassInterface<device_execute_interface_m6802>();
            m_dimemory = GetClassInterface<device_memory_interface_m6800>();
            m_distate = GetClassInterface<device_state_interface_m6800>();
        }


        protected m6802_cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, op_func [] insn, uint8_t [] cycles)
            : base(mconfig, type, tag, owner, clock, insn, cycles, ram_map)//, this)
        {
            m_ram_enable = true;
        }


        protected void set_ram_enable(bool re) { assert(!configured()); m_ram_enable = re; }


        protected virtual uint64_t device_execute_interface_execute_clocks_to_cycles(uint64_t clocks) { return (clocks + 4 - 1) / 4; }
        protected virtual uint64_t device_execute_interface_execute_cycles_to_clocks(uint64_t cycles) { return cycles * 4; }
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        static void ram_map(address_map map, device_t owner)
        {
            m6802_cpu_device this_ = (m6802_cpu_device)owner;

            if (this_.m_ram_enable)
                map.op(0x0000, 0x007f).ram();
        }
    }


    public class m6808_cpu_device : m6802_cpu_device
    {
        //DEFINE_DEVICE_TYPE(M6808, m6808_cpu_device, "m6808", "Motorola MC6808")
        public static readonly emu.detail.device_type_impl M6808 = DEFINE_DEVICE_TYPE("m6808", "Motorola MC6808", (type, mconfig, tag, owner, clock) => { return new m6808_cpu_device(mconfig, tag, owner, clock); });


        m6808_cpu_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, M6808, tag, owner, clock, m6800_insn, cycles_6800)
        {
            m_class_interfaces.Add(new device_execute_interface_m6802(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_m6800(mconfig, this));

            m_diexec = GetClassInterface<device_execute_interface_m6802>();
            m_dimemory = GetClassInterface<device_memory_interface_m6800>();
            m_distate = GetClassInterface<device_state_interface_m6800>();


            set_ram_enable(false);
        }


        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;
        //virtual void device_validity_check(validity_checker &valid) const override;
    }


    //class nsc8105_cpu_device : public m6802_cpu_device


    //DECLARE_DEVICE_TYPE(M6802, m6802_cpu_device)
    //DECLARE_DEVICE_TYPE(M6808, m6808_cpu_device)
    //DECLARE_DEVICE_TYPE(NSC8105, nsc8105_cpu_device)


    static partial class m6800_global
    {
        public static m6808_cpu_device M6808<bool_Required>(machine_config mconfig, device_finder<m6808_cpu_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, m6808_cpu_device.M6808, clock); }
    }
}
