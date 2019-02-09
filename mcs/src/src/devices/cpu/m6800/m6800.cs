// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using int16_t = System.Int16;
using space_config_vector = mame.std.vector<System.Collections.Generic.KeyValuePair<int, mame.address_space_config>>;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    class device_execute_interface_m6800 : device_execute_interface
    {
        public device_execute_interface_m6800(machine_config mconfig, device_t device) : base(mconfig, device) { }


        // device_execute_interface overrides
        public override uint32_t execute_min_cycles() { return 1; }
        public override uint32_t execute_max_cycles() { return 12; }
        public override uint32_t execute_input_lines() { return 2; }
        public override bool execute_input_edge_triggered(int inputnum) { m6800_cpu_device m6800 = (m6800_cpu_device)device(); return m6800.device_execute_interface_execute_input_edge_triggered(inputnum); }
        public override void execute_run() { m6800_cpu_device m6800 = (m6800_cpu_device)device(); m6800.device_execute_interface_execute_run(); }
        public override void execute_set_input(int irqline, int state) { m6800_cpu_device m6800 = (m6800_cpu_device)device(); m6800.device_execute_interface_execute_set_input(irqline, state); }
    }


    class device_memory_interface_m6800 : device_memory_interface
    {
        public device_memory_interface_m6800(machine_config mconfig, device_t device) : base(mconfig, device) { }

        // device_memory_interface overrides
        public override space_config_vector memory_space_config() { m6800_cpu_device m6800 = (m6800_cpu_device)device(); return m6800.device_memory_interface_memory_space_config(); }
    }


    class device_state_interface_m6800 : device_state_interface
    {
        public device_state_interface_m6800(machine_config mconfig, device_t device) : base(mconfig, device) { }

        // device_state_interface overrides
        public override void state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }
    }


    class device_disasm_interface_m6800 : device_disasm_interface
    {
        public device_disasm_interface_m6800(machine_config mconfig, device_t device) : base(mconfig, device) { }

        // device_disasm_interface overrides
        protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
    }


    partial class m6800_cpu_device : cpu_device
    {
        //DEFINE_DEVICE_TYPE(M6800, m6800_cpu_device, "m6800", "Motorola M6800")
        static device_t device_creator_m6800_cpu_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new m6800_cpu_device(mconfig, tag, owner, clock); }
        public static readonly device_type M6800 = DEFINE_DEVICE_TYPE(device_creator_m6800_cpu_device, "m6800", "Motorola M6800");


        //typedef void (m6800_cpu_device::*op_func)();
        public delegate void op_func(m6800_cpu_device cpu);


        //enum
        //{
        protected const int M6800_IRQ_LINE = 0;              /* IRQ line number */
        //}


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
        uint32_t M_RDOP(uint32_t Addr) { return m_opcodes_cache.read_byte(Addr); }

        /****************************************************************************/
        /* M6800_RDOP_ARG() is identical to M6800_RDOP() but it's used for reading  */
        /* opcode arguments. This difference can be used to support systems that    */
        /* use different encoding mechanisms for opcodes and opcode arguments       */
        /****************************************************************************/
        uint32_t M_RDOP_ARG(uint32_t Addr) { return m_cache.read_byte(Addr); }


        /* macros to access memory */
        void IMMBYTE(out uint8_t b) { b = (uint8_t)M_RDOP_ARG(PCD); PC++; }
        void IMMBYTE(out uint16_t b) { b = (uint16_t)M_RDOP_ARG(PCD); PC++; }
        void IMMBYTE(out uint32_t b) { b = (uint32_t)M_RDOP_ARG(PCD); PC++; }
        void IMMWORD(ref PAIR w) { w.d = (M_RDOP_ARG(PCD) << 8) | M_RDOP_ARG((PCD + 1) & 0xffff); PC += 2; }

        void PUSHBYTE(uint8_t b) { WM(SD, b); --S; }
        void PUSHWORD(PAIR w) { WM(SD, w.b.l); --S; WM(SD, w.b.h); --S; }
        void PULLBYTE(out uint8_t b) { S++; b = (uint8_t)RM(SD); }
        void PULLWORD(ref PAIR w) { S++; w.d = RM(SD)<<8; S++; w.d |= RM(SD); }

        // helpers due to properties can't use 'out'
        void IMMBYTE_A() { var temp = A; IMMBYTE(out temp); A = temp; }
        void IMMBYTE_B() { var temp = B; IMMBYTE(out temp); B = temp; }
        void IMMBYTE_EAD() { var temp = EAD; IMMBYTE(out temp); EAD = temp; }
        void PULLBYTE_A() { var temp = A; PULLBYTE(out temp); A = temp; }
        void PULLBYTE_B() { var temp = B; PULLBYTE(out temp); B = temp; }
        void PULLBYTE_CC() { var temp = CC; PULLBYTE(out temp); CC = temp; }
        void PULLWORD_pPC() { var temp = pPC; PULLWORD(ref temp); pPC = temp; }
        void PULLWORD_pX() { var temp = pX; PULLWORD(ref temp); pX = temp; }


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
        int16_t SIGNED(uint8_t b) { return (int16_t)(((b & 0x80) != 0) ? b | 0xff00 : b); }


        /* Macros for addressing modes */
        void DIRECT() { IMMBYTE_EAD(); }
        void IMM8() { EA = PC++; }
        void IMM16() { EA = PC; PC += 2; }
        void EXTENDED() { IMMWORD(ref m_ea); }
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
        void DIRWORD(ref PAIR w) { DIRECT(); w.d = RM16(EAD); }
        void EXTBYTE(out uint8_t b) { EXTENDED(); b = (uint8_t)RM(EAD); }
        void EXTBYTE(out uint16_t b) { EXTENDED(); b = (uint16_t)RM(EAD); }
        void EXTWORD(ref PAIR w) { EXTENDED(); w.d = RM16(EAD); }

        void IDXBYTE(out uint8_t b)  { INDEXED(); b = (uint8_t)RM(EAD); }
        void IDXBYTE(out uint16_t b) { INDEXED(); b = (uint16_t)RM(EAD); }
        void IDXWORD(ref PAIR w) { INDEXED(); w.d = RM16(EAD); }

        // helpers due to properties can't use 'out'
        void DIRBYTE_A() { var temp = A; DIRBYTE(out temp); A = temp; }
        void DIRBYTE_B() { var temp = B; DIRBYTE(out temp); B = temp; }
        void EXTBYTE_A() { var temp = A; EXTBYTE(out temp); A = temp; }
        void EXTBYTE_B() { var temp = B; EXTBYTE(out temp); B = temp; }
        void IDXBYTE_A() { var temp = A; IDXBYTE(out temp); A = temp; }
        void IDXBYTE_B() { var temp = B; IDXBYTE(out temp); B = temp; }

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
        uint8_t [] m_irq_state = new uint8_t [3];   /* IRQ line state [IRQ1,TIN,SC1] */

        /* Memory spaces */
        address_space m_program;
        address_space m_opcodes;
        memory_access_cache m_cache;
        memory_access_cache m_opcodes_cache;  //memory_access_cache<0, 0, ENDIANNESS_BIG> *m_cache, *m_opcodes_cache;

        op_func [] m_insn;
        uint8_t [] m_cycles;            /* clock cycle of instruction table */

        intref m_icountRef = new intref();  //int m_icount;

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


        static readonly uint8_t [] cycles_6800 = new uint8_t [256];
        //static const uint8_t cycles_nsc8105[256];
        static readonly op_func [] m6800_insn = new op_func [256];
        //static const op_func nsc8105_insn[256];


        device_memory_interface_m6800 m_dimemory;
        device_execute_interface_m6800 m_diexec;
        device_state_interface_m6800 m_distate;


        // construction/destruction
        protected m6800_cpu_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, M6800, tag, owner, clock, m6800_insn, cycles_6800, null)
        {
            m_class_interfaces.Add(new device_execute_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_m6800(mconfig, this));
        }


        protected m6800_cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, op_func [] insn, uint8_t [] cycles, address_map_constructor internal_) 
            : base(mconfig, type, tag, owner, clock)
        {
            m_program_config = new address_space_config("program", endianness_t.ENDIANNESS_BIG, 8, 16, 0, internal_);
            m_decrypted_opcodes_config = new address_space_config("program", endianness_t.ENDIANNESS_BIG, 8, 16, 0);
            m_insn = insn;
            m_cycles = cycles;
        }


        protected uint8_t cc { get { return m_cc; } }
        protected uint8_t wai_state { get { return m_wai_state; } set { m_wai_state = value; } }
        protected uint8_t [] irq_state { get { return m_irq_state; } }
        protected device_execute_interface_m6800 diexec { get { return m_diexec; } }


        // device-level overrides
        protected override void device_start()
        {
            m_dimemory = GetClassInterface<device_memory_interface_m6800>();
            m_diexec = GetClassInterface<device_execute_interface_m6800>();
            m_distate = GetClassInterface<device_state_interface_m6800>();


            m_program = m_dimemory.space(AS_PROGRAM);
            m_cache = m_program.cache(0, 0, (int)endianness_t.ENDIANNESS_BIG);
            m_opcodes = m_dimemory.has_space(AS_OPCODES) ? m_dimemory.space(AS_OPCODES) : m_program;
            m_opcodes_cache = m_opcodes.cache(0, 0, (int)endianness_t.ENDIANNESS_BIG);

            m_pc.d = 0;
            m_s.d = 0;
            m_x.d = 0;
            m_d.d = 0;
            m_cc = 0;
            m_wai_state = 0;
            m_irq_state[0] = 0;
            m_irq_state[1] = 0;
            m_irq_state[2] = 0;

            save_item(m_ppc.w.l, "m_ppc.w.l");
            save_item(m_pc.w.l, "m_pc.w.l");
            save_item(m_s.w.l, "m_s.w.l");
            save_item(m_x.w.l, "m_x.w.l");
            save_item(m_d.w.l, "m_d.w.l");
            save_item(m_cc, "m_cc");
            save_item(m_wai_state, "m_wai_state");
            save_item(m_nmi_state, "m_nmi_state");
            save_item(m_nmi_pending, "m_nmi_pending");
            save_item(m_irq_state, "m_irq_state");

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

            set_icountptr(m_icountRef);
        }


        protected override void device_stop()
        {
            if (m_opcodes_cache != null) m_opcodes_cache.Dispose();
            if (m_opcodes != null) m_opcodes.Dispose();
            if (m_cache != null) m_cache.Dispose();
            if (m_program != null) m_program.Dispose();
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
        public bool device_execute_interface_execute_input_edge_triggered(int inputnum) { return inputnum == device_execute_interface.INPUT_LINE_NMI; }

        public void device_execute_interface_execute_run()
        {
            uint8_t ireg;

            CHECK_IRQ_LINES(); /* HJB 990417 */

            CLEANUP_COUNTERS();

            do
            {
                if ((m_wai_state & (M6800_WAI | M6800_SLP)) != 0)
                {
                    EAT_CYCLES();
                }
                else
                {
                    pPPC = pPC;
                    debugger_instruction_hook(PCD);
                    ireg = (uint8_t)M_RDOP(PCD);
                    PC++;
                    m_insn[ireg](this);
                    increment_counter(m_cycles[ireg]);
                }
            } while (m_icountRef.i > 0);
        }

        public void device_execute_interface_execute_set_input(int irqline, int state)
        {
            switch (irqline)
            {
            case device_execute_interface.INPUT_LINE_NMI:
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
        public space_config_vector device_memory_interface_memory_space_config()
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
            return result | RM((Addr+1)&0xffff);
        }


        void WM16(uint32_t Addr, ref PAIR p)
        {
            WM(Addr, p.b.h);
            WM((Addr + 1) & 0xffff, p.b.l);
        }


        /* IRQ enter */
        protected void enter_interrupt(string message, uint16_t irq_vector)
        {
            LOG(message);

            if ((m_wai_state & (M6800_WAI | M6800_SLP)) != 0)
            {
                if ((m_wai_state & M6800_WAI) != 0)
                    m_icountRef.i -= 4;

                m_wai_state = (uint8_t)(m_wai_state & ~(M6800_WAI | M6800_SLP));
            }
            else
            {
                PUSHWORD(pPC);
                PUSHWORD(pX);
                PUSHBYTE(A);
                PUSHBYTE(B);
                PUSHBYTE(CC);
                m_icountRef.i -= 12;
            }

            SEI();
            PCD = RM16( irq_vector );
        }


        protected virtual void m6800_check_irq2() { }


        /* check the IRQ lines for pending interrupts */
        public void CHECK_IRQ_LINES()
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
                        m_diexec.standard_irq_callback(M6800_IRQ_LINE);
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
            m_icountRef.i -= amount;
        }


        public virtual void EAT_CYCLES() { throw new emu_unimplemented(); }
        public virtual void CLEANUP_COUNTERS() { }
        protected virtual void TAKE_TRAP() { }

#if false
        void aba();
        void abx();
        void adca_di();
        void adca_ex();
        void adca_im();
        void adca_ix();
        void adcb_di();
        void adcb_ex();
        void adcb_im();
        void adcb_ix();
        void adcx_im();
        void adda_di();
        void adda_ex();
        void adda_im();
        void adda_ix();
        void addb_di();
        void addb_ex();
        void addb_im();
        void addb_ix();
        void addd_di();
        void addd_ex();
        void addx_ex();
        void addd_im();
        void addd_ix();
        void aim_di();
        void aim_ix();
        void anda_di();
        void anda_ex();
        void anda_im();
        void anda_ix();
        void andb_di();
        void andb_ex();
        void andb_im();
        void andb_ix();
        void asl_ex();
        void asl_ix();
        void asla();
        void aslb();
        void asld();
        void asr_ex();
        void asr_ix();
        void asra();
        void asrb();
        void bcc();
        void bcs();
        void beq();
        void bge();
        void bgt();
        void bhi();
        void bita_di();
        void bita_ex();
        void bita_im();
        void bita_ix();
        void bitb_di();
        void bitb_ex();
        void bitb_im();
        void bitb_ix();
        void ble();
        void bls();
        void blt();
        void bmi();
        void bne();
        void bpl();
        void bra();
        void brn();
        void bsr();
        void bvc();
        void bvs();
        void cba();
        void clc();
        void cli();
        void clr_ex();
        void clr_ix();
        void clra();
        void clrb();
        void clv();
        void cmpa_di();
        void cmpa_ex();
        void cmpa_im();
        void cmpa_ix();
        void cmpb_di();
        void cmpb_ex();
        void cmpb_im();
        void cmpb_ix();
        void cmpx_di();
        void cmpx_ex();
        void cmpx_im();
        void cmpx_ix();
        void com_ex();
        void com_ix();
        void coma();
        void comb();
        void daa();
        void dec_ex();
        void dec_ix();
        void deca();
        void decb();
        void des();
        void dex();
        void eim_di();
        void eim_ix();
        void eora_di();
        void eora_ex();
        void eora_im();
        void eora_ix();
        void eorb_di();
        void eorb_ex();
        void eorb_im();
        void eorb_ix();
        void illegl1();
        void illegl2();
        void illegl3();
        void inc_ex();
        void inc_ix();
        void inca();
        void incb();
        void ins();
        void inx();
        void jmp_ex();
        void jmp_ix();
        void jsr_di();
        void jsr_ex();
        void jsr_ix();
        void lda_di();
        void lda_ex();
        void lda_im();
        void lda_ix();
        void ldb_di();
        void ldb_ex();
        void ldb_im();
        void ldb_ix();
        void ldd_di();
        void ldd_ex();
        void ldd_im();
        void ldd_ix();
        void lds_di();
        void lds_ex();
        void lds_im();
        void lds_ix();
        void ldx_di();
        void ldx_ex();
        void ldx_im();
        void ldx_ix();
        void lsr_ex();
        void lsr_ix();
        void lsra();
        void lsrb();
        void lsrd();
        void mul();
        void neg_ex();
        void neg_ix();
        void nega();
        void negb();
        void nop();
        void oim_di();
        void oim_ix();
        void ora_di();
        void ora_ex();
        void ora_im();
        void ora_ix();
        void orb_di();
        void orb_ex();
        void orb_im();
        void orb_ix();
        void psha();
        void pshb();
        void pshx();
        void pula();
        void pulb();
        void pulx();
        void rol_ex();
        void rol_ix();
        void rola();
        void rolb();
        void ror_ex();
        void ror_ix();
        void rora();
        void rorb();
        void rti();
        void rts();
        void sba();
        void sbca_di();
        void sbca_ex();
        void sbca_im();
        void sbca_ix();
        void sbcb_di();
        void sbcb_ex();
        void sbcb_im();
        void sbcb_ix();
        void sec();
        void sei();
        void sev();
        void slp();
        void sta_di();
        void sta_ex();
        void sta_im();
        void sta_ix();
        void stb_di();
        void stb_ex();
        void stb_im();
        void stb_ix();
        void std_di();
        void std_ex();
        void std_im();
        void std_ix();
        void sts_di();
        void sts_ex();
        void sts_im();
        void sts_ix();
        void stx_di();
        void stx_ex();
        void stx_im();
        void stx_ix();
        void suba_di();
        void suba_ex();
        void suba_im();
        void suba_ix();
        void subb_di();
        void subb_ex();
        void subb_im();
        void subb_ix();
        void subd_di();
        void subd_ex();
        void subd_im();
        void subd_ix();
        void swi();
        void tab();
        void tap();
        void tba();
        void tim_di();
        void tim_ix();
        void tpa();
        void tst_ex();
        void tst_ix();
        void tsta();
        void tstb();
        void tsx();
        void txs();
        void undoc1();
        void undoc2();
        void wai();
        void xgdx();
        void cpx_di();
        void cpx_ex();
        void cpx_im();
        void cpx_ix();
        void trap();
        void btst_ix();
        void stx_nsc();
#endif
    }


#if false
    class m6802_cpu_device : public m6800_cpu_device
    {
    public:
        m6802_cpu_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock);

    protected:
        m6802_cpu_device(const machine_config &mconfig, device_type type, const char *tag, device_t *owner, uint32_t clock, const m6800_cpu_device::op_func *insn, const uint8_t *cycles);

        virtual uint64_t execute_clocks_to_cycles(uint64_t clocks) const override { return (clocks + 4 - 1) / 4; }
        virtual uint64_t execute_cycles_to_clocks(uint64_t cycles) const override { return (cycles * 4); }
        virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;
    };


    class m6808_cpu_device : public m6802_cpu_device
    {
    public:
        m6808_cpu_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock);

    protected:
        virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;
    };


    class nsc8105_cpu_device : public m6802_cpu_device
    {
    public:
        nsc8105_cpu_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock);

    protected:
        virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;
    };
#endif


    //DECLARE_DEVICE_TYPE(M6802, m6802_cpu_device)
    //DECLARE_DEVICE_TYPE(M6808, m6808_cpu_device)
    //DECLARE_DEVICE_TYPE(NSC8105, nsc8105_cpu_device)
}
