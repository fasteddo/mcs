// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Runtime.InteropServices;

using devcb_read16 = mame.devcb_read<mame.Type_constant_u16>;  //using devcb_read16 = devcb_read<u16>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using u8 = System.Byte;
using u16 = System.UInt16;
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
using static mame.z8000_global;


namespace mame
{
    public partial class z8002_device : cpu_device,
                                        z8000_disassembler.config
    {
        //DEFINE_DEVICE_TYPE(Z8002, z8002_device, "z8002", "Zilog Z8002")
        public static readonly emu.detail.device_type_impl Z8002 = DEFINE_DEVICE_TYPE("z8002", "Zilog Z8002", (type, mconfig, tag, owner, clock) => { return new z8002_device(mconfig, tag, owner, clock); });


        class device_execute_interface_z8002 : device_execute_interface
        {
            public device_execute_interface_z8002(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override u32 execute_min_cycles() { return ((z8002_device)device()).device_execute_interface_execute_min_cycles(); }
            protected override u32 execute_max_cycles() { return ((z8002_device)device()).device_execute_interface_execute_max_cycles(); }
            protected override u32 execute_input_lines() { return ((z8002_device)device()).device_execute_interface_execute_input_lines(); }
            protected override uint32_t execute_default_irq_vector(int inputnum) { return ((z8002_device)device()).device_execute_interface_execute_default_irq_vector(inputnum); }
            protected override bool execute_input_edge_triggered(int inputnum) { return ((z8002_device)device()).device_execute_interface_execute_input_edge_triggered(inputnum); }
            protected override void execute_run() { ((z8002_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { ((z8002_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
        }


        public class device_memory_interface_z8002 : device_memory_interface
        {
            public device_memory_interface_z8002(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((z8002_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_z8002 : device_state_interface
        {
            public device_state_interface_z8002(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void state_import(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_export(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }
        }


        public class device_disasm_interface_z8002 : device_disasm_interface
        {
            public device_disasm_interface_z8002(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        const int VERBOSE = 0;  //#define VERBOSE 1
        //#include "logmacro.h"
        void LOG(string format, params object [] args) { logmacro_global.LOG(VERBOSE, this, format, args); }


        //enum
        //{
            const int Z8000_PC      = 1;
            const int Z8000_NSPSEG  = 2;
            const int Z8000_NSPOFF  = 3;
            const int Z8000_FCW     = 4;
            const int Z8000_PSAPSEG = 5;
            const int Z8000_PSAPOFF = 6;
            const int Z8000_REFRESH = 7;
            const int Z8000_IRQ_REQ = 8;
            const int Z8000_IRQ_SRV = 9;
            const int Z8000_IRQ_VEC = 10;
            const int Z8000_R0      = 11;
            const int Z8000_R1      = 12;
            const int Z8000_R2      = 13;
            const int Z8000_R3      = 14;
            const int Z8000_R4      = 15;
            const int Z8000_R5      = 16;
            const int Z8000_R6      = 17;
            const int Z8000_R7      = 18;
            const int Z8000_R8      = 19;
            const int Z8000_R9      = 20;
            const int Z8000_R10     = 21;
            const int Z8000_R11     = 22;
            const int Z8000_R12     = 23;
            const int Z8000_R13     = 24;
            const int Z8000_R14     = 25;
            const int Z8000_R15     = 26;
        //};


        /* Interrupt Types that can be generated by outside sources */
        const uint8_t Z8000_EPU     = 0x80;  /* extended instruction trap */
        const uint8_t Z8000_TRAP    = 0x40;  /* privileged instruction trap */
        const uint8_t Z8000_NMI     = 0x20;  /* non maskable interrupt */
        const uint8_t Z8000_SEGTRAP = 0x10;  /* segment trap (Z8001) */
        const uint8_t Z8000_NVI     = 0x08;  /* non vectored interrupt */
        const uint8_t Z8000_VI      = 0x04;  /* vectored interrupt (LSB is vector)  */
        const uint8_t Z8000_SYSCALL = 0x02;  /* system call (lsb is vector) */
        const uint8_t Z8000_RESET   = 0x01;  /* reset flag  */


        //enum
        //{
            public const int NVI_LINE = 0;
            public const int VI_LINE = 1;
            public const int NMI_LINE = INPUT_LINE_NMI;
        //};

        //enum
        //{
            const int AS_STACK = AS_OPCODES + 1;
            const int AS_SIO   = AS_OPCODES + 2;
        //};


        device_memory_interface_z8002 m_dimemory;
        device_execute_interface_z8002 m_diexec;
        device_state_interface_z8002 m_distate;


        address_space_config m_program_config;
        address_space_config m_data_config;
        address_space_config m_io_config;
        address_space_config m_opcodes_config;
        address_space_config m_stack_config;
        address_space_config m_sio_config;
        devcb_read16.array<u64_const_4> m_iack_in;
        devcb_write_line m_mo_out;

        uint32_t [] m_op = new uint32_t [4];      /* opcodes/data of current instruction */
        uint32_t m_ppc;        /* previous program counter */
        uint32_t m_pc;         /* program counter */
        uint16_t m_psapseg;    /* program status pointer, segment (Z8001 only) */
        uint16_t m_psapoff;    /* program status pointer, offset */
        uint16_t m_fcw;        /* flags and control word */
        uint16_t m_refresh;    /* refresh timer/counter */
        uint16_t m_nspseg;     /* system stack pointer, segment (Z8001 only) */
        uint16_t m_nspoff;     /* system stack pointer, offset */
        uint8_t m_irq_req;    /* reset, interrupt or trap request */
        uint16_t m_irq_vec;    /* interrupt vector */
        uint32_t m_op_valid;   /* bit field indicating if given op[] field is already initialized */

        public class REGS  //union
        {
            public MemoryU8 B = new MemoryU8(8 * 4, true);  //uint8_t   B[16]; /* RL0,RH0,RL1,RH1...RL7,RH7 */
            public PointerU16 W { get { return new PointerU16(B); } }  //uint16_t  W[16]; /* R0,R1,R2...R15 */
            public PointerU32 L { get { return new PointerU32(B); } }  //uint32_t  L[8];  /* RR0,RR2,RR4..RR14 */
            public PointerU64 Q { get { return new PointerU64(B); } }  //uint64_t  Q[4];  /* RQ0,RQ4,..RQ12 */
        }// m_regs;             /* registers */
        REGS m_regs = new REGS();

        int m_nmi_state;      /* NMI line state */
        int [] m_irq_state = new int [2];   /* IRQ line states (NVI, VI) */
        int m_mi;
        bool m_halt;
        memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache m_cache = new memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache();
        memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache m_opcache = new memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache();
        memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific m_program = new memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific();
        memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific m_data = new memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific();
        memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific m_stack = new memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific();
        memory_access<int_const_16, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific m_io = new memory_access<int_const_16, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific();
        memory_access<int_const_16, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific m_sio = new memory_access<int_const_16, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific();
        intref m_icount = new intref();  //int m_icount;
        int m_vector_mult;


        // structure for the opcode definition table
        delegate void opcode_func();  //typedef void (z8002_device::*opcode_func)();

        struct Z8000_init
        {
            public int beg;
            public int end;
            public int step;
            public int size;
            public int cycles;
            public opcode_func opcode;

            public Z8000_init(int beg, int end, int step, int size, int cycles, opcode_func opcode) { this.beg = beg; this.end = end; this.step = step; this.size = size; this.cycles = cycles; this.opcode = opcode; }
        }

        // z8000tbl.cs
        /* opcode execution table */
        Z8000_init [] table;  //static const Z8000_init table[];

        u16 [] z8000_exec = new u16 [0x10000];

        /* zero, sign and parity flags for logical byte operations */
        u8 [] z8000_zsp = new u8 [256];


        // construction/destruction
        z8002_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, Z8002, tag, owner, clock, 16, 1)
        { }


        z8002_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, int addrbits, int vecmult)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_z8002(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_z8002(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_z8002(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_z8002(mconfig, this));

            m_dimemory = GetClassInterface<device_memory_interface_z8002>();
            m_diexec = GetClassInterface<device_execute_interface_z8002>();
            m_distate = GetClassInterface<device_state_interface_z8002>();


            init_table();


            m_program_config = new address_space_config("program", ENDIANNESS_BIG, 16, (uint8_t)addrbits, 0);
            m_data_config = new address_space_config("data", ENDIANNESS_BIG, 16, (uint8_t)addrbits, 0);
            m_io_config = new address_space_config("io_std", ENDIANNESS_BIG, 16, 16, 0);
            m_opcodes_config = new address_space_config("first_word", ENDIANNESS_BIG, 16, (uint8_t)addrbits, 0);
            m_stack_config = new address_space_config("stack", ENDIANNESS_BIG, 16, (uint8_t)addrbits, 0);
            m_sio_config = new address_space_config("io_spc", ENDIANNESS_BIG, 16, 16, 0);
            m_iack_in = new devcb_read16.array<u64_const_4>(this, () => { return new devcb_read16(this); });
            m_mo_out = new devcb_write_line(this);
            m_ppc = 0;
            m_pc = 0;
            m_psapseg = 0;
            m_psapoff = 0;
            m_fcw = 0;
            m_refresh = 0;
            m_nspseg = 0;
            m_nspoff = 0;
            m_irq_req = 0;
            m_irq_vec = 0;
            m_op_valid = 0;
            m_nmi_state = 0;
            m_mi = 0;
            m_halt = false;
            m_icount.i = 0;
            m_vector_mult = vecmult;
        }


        //auto segtack() { return m_iack_in[0].bind(); }
        //auto nmiack() { return m_iack_in[1].bind(); }
        //auto nviack() { return m_iack_in[2].bind(); }
        //auto viack() { return m_iack_in[3].bind(); }
        //auto mo() { return m_mo_out.bind(); }
        //DECLARE_WRITE_LINE_MEMBER(mi_w) { m_mi = state; } // XXX: this has to apply in the middle of an insn for now


        // device-level overrides
        protected override void device_start()
        {
            clear_internal_state();

            init_spaces();
            init_tables();

            register_debug_state();
            register_save_state();

            set_icountptr(m_icount);
            m_iack_in.resolve_all_safe_u16(0xffff);
            m_mo_out.resolve_safe();
            m_mi = CLEAR_LINE;
        }


        protected override void device_reset()
        {
            m_irq_req |= Z8000_RESET;
            m_refresh &= 0x7fff;
            m_halt = false;
        }


        // device_execute_interface overrides
        protected virtual uint32_t device_execute_interface_execute_min_cycles() { return 2; }
        protected virtual uint32_t device_execute_interface_execute_max_cycles() { throw new emu_unimplemented(); }  //{ return 744; }
        protected virtual uint32_t device_execute_interface_execute_input_lines() { throw new emu_unimplemented(); }  //{ return 2; }
        protected virtual uint32_t device_execute_interface_execute_default_irq_vector(int inputnum) { return 0xff; }
        protected virtual bool device_execute_interface_execute_input_edge_triggered(int inputnum) { throw new emu_unimplemented(); }  //{ return inputnum == INPUT_LINE_NMI; }


        protected virtual void device_execute_interface_execute_run()
        {
            do
            {
                /* any interrupt request pending? */
                if (m_irq_req != 0)
                    Interrupt();

                m_ppc = m_pc;
                debugger_instruction_hook(m_pc);

                if (m_halt)
                {
                    m_icount.i = 0;
                }
                else
                {
                    m_op[0] = RDOP();
                    m_op_valid = 1;
                    ref Z8000_init exec = ref table[z8000_exec[m_op[0]]];  //const Z8000_init &exec = table[z8000_exec[m_op[0]]];

                    m_icount.i -= exec.cycles;
                    exec.opcode();  //(this->*exec.opcode)();
                    m_op_valid = 0;
                }
            } while (m_icount.i > 0);
        }


        protected virtual void device_execute_interface_execute_set_input(int inputnum, int state) { throw new emu_unimplemented(); }


        // device_memory_interface overrides
        protected virtual space_config_vector device_memory_interface_memory_space_config()
        {
            var spaces = new space_config_vector()
            {
                std.make_pair(AS_PROGRAM, m_program_config),
                std.make_pair(AS_IO,      m_io_config)
            };

            if (memory().has_configured_map(AS_DATA))
                spaces.push_back(std.make_pair(AS_DATA, m_data_config));
            if (memory().has_configured_map(AS_OPCODES))
                spaces.push_back(std.make_pair(AS_OPCODES, m_opcodes_config));
            if (memory().has_configured_map(AS_STACK))
                spaces.push_back(std.make_pair(AS_STACK, m_stack_config));
            if (memory().has_configured_map(AS_SIO))
                spaces.push_back(std.make_pair(AS_SIO, m_sio_config));

            return spaces;
        }


        // device_state_interface overrides
        protected virtual void device_state_interface_state_string_export(device_state_entry entry, string str) { throw new emu_unimplemented(); }


        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        void init_spaces()
        {
            m_dimemory.space(AS_PROGRAM).cache(m_cache);
            m_dimemory.space(AS_PROGRAM).specific(m_program);

            /* If the system decodes STn lines to distinguish between data and program memory fetches,
               install the data space. If it doesn't, install the program memory into data memory space. */
            m_dimemory.space(m_dimemory.has_space(AS_DATA) ? AS_DATA : AS_PROGRAM).specific(m_data);
            m_dimemory.space(m_dimemory.has_space(AS_STACK) ? AS_STACK : m_dimemory.has_space(AS_DATA) ? AS_DATA : AS_PROGRAM).specific(m_stack);
            m_dimemory.space(m_dimemory.has_space(AS_OPCODES) ? AS_OPCODES : AS_PROGRAM).cache(m_opcache);
            m_dimemory.space(AS_IO).specific(m_io);
            m_dimemory.space(m_dimemory.has_space(AS_SIO) ? AS_SIO : AS_IO).specific(m_sio);
        }


        void init_tables()
        {
            /* set up the zero, sign, parity lookup table */
            for (int i = 0; i < 256; i++)
                z8000_zsp[i] = (u8)(((i == 0) ? F_Z : 0) |
                                ((i & 128) != 0 ? F_S : 0) |
                                ((((i>>7)^(i>>6)^(i>>5)^(i>>4)^(i>>3)^(i>>2)^(i>>1)^i) & 1) != 0 ? 0 : F_PV));

            for (int opcIdx = 0; table[opcIdx].size != 0; opcIdx++)  //for (const Z8000_init *opc = table; opc->size; opc++)
            {
                ref Z8000_init opc = ref table[opcIdx];
                for (u32 val = (u32)opc.beg; val <= opc.end; val += (u32)opc.step)
                    z8000_exec[val] = (u16)opcIdx;  //z8000_exec[val] = opc - table;
            }
        }


        void clear_internal_state()
        {
            m_op[0] = 0;
            m_op[1] = 0;
            m_op[2] = 0;
            m_op[3] = 0;
            m_ppc = 0;
            m_pc = 0;
            m_psapseg = 0;
            m_psapoff = 0;
            m_fcw = 0;
            m_refresh = 0;
            m_nspseg = 0;
            m_nspoff = 0;
            m_irq_req = 0;
            m_irq_vec = 0;
            m_op_valid = 0;
            m_regs.Q[0] = 0;
            m_regs.Q[1] = 0;
            m_regs.Q[2] = 0;
            m_regs.Q[3] = 0;
            m_nmi_state = 0;
            m_irq_state[0] = 0;
            m_irq_state[1] = 0;
        }


        void register_debug_state()
        {
            m_distate.state_add( Z8000_PC,      "PC",      m_pc      ).mask(m_program.space().addrmask());
            m_distate.state_add( Z8000_NSPOFF,  "NSPOFF",  m_nspoff  ).formatstr("%04X");
            m_distate.state_add( Z8000_NSPSEG,  "NSPSEG",  m_nspseg  ).formatstr("%04X");
            m_distate.state_add( Z8000_FCW,     "FCW",     m_fcw     ).formatstr("%04X");
            m_distate.state_add( Z8000_PSAPOFF, "PSAPOFF", m_psapoff ).formatstr("%04X");
            m_distate.state_add( Z8000_PSAPSEG, "PSAPSEG", m_psapseg ).formatstr("%04X");
            m_distate.state_add( Z8000_REFRESH, "REFR",    m_refresh ).formatstr("%04X");
            m_distate.state_add( Z8000_IRQ_VEC, "IRQV",    m_irq_vec ).formatstr("%04X");
            m_distate.state_add( Z8000_R0,      "R0",      RW(0).op  ).formatstr("%04X");
            m_distate.state_add( Z8000_R1,      "R1",      RW(1).op  ).formatstr("%04X");
            m_distate.state_add( Z8000_R2,      "R2",      RW(2).op  ).formatstr("%04X");
            m_distate.state_add( Z8000_R3,      "R3",      RW(3).op  ).formatstr("%04X");
            m_distate.state_add( Z8000_R4,      "R4",      RW(4).op  ).formatstr("%04X");
            m_distate.state_add( Z8000_R5,      "R5",      RW(5).op  ).formatstr("%04X");
            m_distate.state_add( Z8000_R6,      "R6",      RW(6).op  ).formatstr("%04X");
            m_distate.state_add( Z8000_R7,      "R7",      RW(7).op  ).formatstr("%04X");
            m_distate.state_add( Z8000_R8,      "R8",      RW(8).op  ).formatstr("%04X");
            m_distate.state_add( Z8000_R9,      "R9",      RW(9).op  ).formatstr("%04X");
            m_distate.state_add( Z8000_R10,     "R10",     RW(10).op ).formatstr("%04X");
            m_distate.state_add( Z8000_R11,     "R11",     RW(11).op ).formatstr("%04X");
            m_distate.state_add( Z8000_R12,     "R12",     RW(12).op ).formatstr("%04X");
            m_distate.state_add( Z8000_R13,     "R13",     RW(13).op ).formatstr("%04X");
            m_distate.state_add( Z8000_R14,     "R14",     RW(14).op ).formatstr("%04X");
            m_distate.state_add( Z8000_R15,     "R15",     RW(15).op ).formatstr("%04X");

            m_distate.state_add( STATE_GENPC, "GENPC", m_pc ).noshow();
            m_distate.state_add( STATE_GENPCBASE, "CURPC", m_ppc ).noshow();
            m_distate.state_add( STATE_GENFLAGS, "GENFLAGS", m_fcw ).formatstr("%16s").noshow();
        }


        void register_save_state()
        {
            save_item(NAME(new { m_op }));
            save_item(NAME(new { m_ppc }));
            save_item(NAME(new { m_pc }));
            save_item(NAME(new { m_psapseg }));
            save_item(NAME(new { m_psapoff }));
            save_item(NAME(new { m_fcw }));
            save_item(NAME(new { m_refresh }));
            save_item(NAME(new { m_nspseg }));
            save_item(NAME(new { m_nspoff }));
            save_item(NAME(new { m_irq_req }));
            save_item(NAME(new { m_irq_vec }));
            save_item(NAME(new { m_op_valid }));
            save_item(NAME(new { m_regs.Q }));
            save_item(NAME(new { m_nmi_state }));
            save_item(NAME(new { m_irq_state }));
            save_item(NAME(new { m_mi }));
            save_item(NAME(new { m_halt }));
            save_item(NAME(new { m_icount }));
        }


        public bool get_segmented_mode()
        {
            return false;
        }


        static uint32_t addr_add(uint32_t addr, uint32_t addend) { throw new emu_unimplemented(); }

        static uint32_t addr_sub(uint32_t addr, uint32_t subtrahend)
        {
            return (addr & 0xffff0000) | ((addr - subtrahend) & 0xffff);
        }

        uint16_t RDOP()
        {
            uint16_t res = m_opcache.read_word(m_pc);
            m_pc += 2;
            return res;
        }

        uint32_t get_operand(int opnum)
        {
            int i;

            for (i = 0; i < opnum; i++)
            {
                assert((m_op_valid & (1 << i)) != 0);
            }

            if ((m_op_valid & (1U << opnum)) == 0)
            {
                m_op[opnum] = m_cache.read_word(m_pc);
                m_pc += 2;
                m_op_valid |= (1U << opnum);
            }

            return m_op[opnum];
        }


        uint32_t get_addr_operand(int opnum) { throw new emu_unimplemented(); }
        uint32_t get_raw_addr_operand(int opnum) { throw new emu_unimplemented(); }


        protected virtual uint32_t adjust_addr_for_nonseg_mode(uint32_t addr)
        {
            return addr;
        }


        uint8_t RDMEM_B(memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space, uint32_t addr)
        {
            addr = adjust_addr_for_nonseg_mode(addr);
            return space.read_byte(addr);
        }


        uint16_t RDMEM_W(memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space, uint32_t addr)
        {
            addr = adjust_addr_for_nonseg_mode(addr);
            addr &= unchecked((uint32_t)~1);

            /* hack for m20 driver: BIOS accesses 0x7f0000 and expects a segmentation violation */
            if (addr >= 0x7f0000)
            {
                m_irq_req = Z8000_SEGTRAP;
                return 0xffff;
            }

            return space.read_word(addr);
        }


        uint32_t RDMEM_L(memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space, uint32_t addr)
        {
            throw new emu_unimplemented();
        }


        void WRMEM_B(memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space, uint32_t addr, uint8_t value)
        {
            throw new emu_unimplemented();
        }

        void WRMEM_W(memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space, uint32_t addr, uint16_t value)
        {
            throw new emu_unimplemented();
        }

        void WRMEM_L(memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space, uint32_t addr, uint32_t value)
        {
            throw new emu_unimplemented();
        }


        uint8_t RDPORT_B(int mode, uint16_t addr) { throw new emu_unimplemented(); }
        uint16_t RDPORT_W(int mode, uint16_t addr) { throw new emu_unimplemented(); }
        void WRPORT_B(int mode, uint16_t addr, uint8_t value) { throw new emu_unimplemented(); }
        void WRPORT_W(int mode, uint16_t addr, uint16_t value) { throw new emu_unimplemented(); }
        //inline void cycles(int cycles);
        protected virtual void PUSH_PC() { throw new emu_unimplemented(); }


        void Interrupt()
        {
            uint16_t fcw = m_fcw;

            if ((m_irq_req & Z8000_RESET) != 0)
            {
                m_irq_req &= Z8000_NVI | Z8000_VI;
                CHANGE_FCW(RDMEM_W(m_program, 2)); /* get reset m_fcw */
                m_pc = get_reset_pc(); /* get reset m_pc  */
            }
            else
            /* trap ? */
            if ((m_irq_req & Z8000_EPU) != 0)
            {
                CHANGE_FCW((uint16_t)((uint32_t)fcw | (uint32_t)F_S_N | F_SEG_Z8001()));/* switch to segmented (on Z8001) system mode */
                PUSH_PC();
                PUSHW(SP, fcw);       /* save current m_fcw */
                PUSHW(SP, (uint16_t)m_op[0]);   /* for internal traps, the 1st word of the instruction is pushed */
                m_irq_req &= unchecked((uint8_t)~Z8000_EPU);
                CHANGE_FCW(GET_FCW(EPU));
                m_pc = GET_PC(EPU);
                LOG("Z8K ext instr trap ${0}\n", m_pc);
            }
            else
            if ((m_irq_req & Z8000_TRAP) != 0)
            {
                CHANGE_FCW((uint16_t)((uint32_t)fcw | (uint32_t)F_S_N | F_SEG_Z8001()));/* switch to segmented (on Z8001) system mode */
                PUSH_PC();
                PUSHW(SP, fcw);       /* save current m_fcw */
                PUSHW(SP, (uint16_t)m_op[0]);   /* for internal traps, the 1st word of the instruction is pushed */
                m_irq_req &= unchecked((uint8_t)~Z8000_TRAP);
                CHANGE_FCW(GET_FCW(TRAP));
                m_pc = GET_PC(TRAP);
                LOG("Z8K priv instr trap ${0}\n", m_pc);
            }
            else
            if ((m_irq_req & Z8000_SYSCALL) != 0)
            {
                CHANGE_FCW((uint16_t)((uint32_t)fcw | (uint32_t)F_S_N | F_SEG_Z8001()));/* switch to segmented (on Z8001) system mode */
                PUSH_PC();
                PUSHW(SP, fcw);       /* save current m_fcw */
                PUSHW(SP, (uint16_t)m_op[0]);   /* for internal traps, the 1st word of the instruction is pushed */
                m_irq_req &= unchecked((uint8_t)~Z8000_SYSCALL);
                CHANGE_FCW(GET_FCW(SYSCALL));
                m_pc = GET_PC(SYSCALL);
                LOG("Z8K syscall [${0}/${1}]\n", m_op[0] & 0xff, m_pc);
            }
            else
            if ((m_irq_req & Z8000_SEGTRAP) != 0)
            {
                //standard_irq_callback(SEGT_LINE);
                m_irq_vec = m_iack_in[0].op_u16(m_pc);

                CHANGE_FCW((uint16_t)((uint32_t)fcw | (uint32_t)F_S_N | F_SEG_Z8001()));/* switch to segmented (on Z8001) system mode */
                PUSH_PC();
                PUSHW(SP, fcw);       /* save current m_fcw */
                PUSHW(SP, m_irq_vec);   /* save interrupt/trap type tag */
                m_irq_req &= unchecked((uint8_t)~Z8000_SEGTRAP);
                CHANGE_FCW(GET_FCW(SEGTRAP));
                m_pc = GET_PC(SEGTRAP);
                LOG("Z8K segtrap ${0}\n", m_pc);
            }
            else
            if ((m_irq_req & Z8000_NMI) != 0)
            {
                standard_irq_callback(NMI_LINE);
                m_irq_vec = m_iack_in[1].op_u16(m_pc);
                m_halt = false;

                CHANGE_FCW((uint16_t)((uint32_t)fcw | (uint32_t)F_S_N | F_SEG_Z8001()));/* switch to segmented (on Z8001) system mode */
                PUSH_PC();
                PUSHW(SP, fcw);       /* save current m_fcw */
                PUSHW(SP, m_irq_vec);   /* save interrupt/trap type tag */
                m_pc = RDMEM_W(m_program, NMI);
                m_irq_req &= unchecked((uint8_t)~Z8000_NMI);
                CHANGE_FCW(GET_FCW(NMI));
                m_pc = GET_PC(NMI);
                LOG("Z8K NMI ${0}\n", m_pc);
            }
            else
            if ((m_irq_req & Z8000_NVI) != 0 && (m_fcw & F_NVIE) != 0)
            {
                standard_irq_callback(NVI_LINE);
                m_irq_vec = m_iack_in[2].op_u16(m_pc);
                m_halt = false;

                CHANGE_FCW((uint16_t)((uint32_t)fcw | (uint32_t)F_S_N | F_SEG_Z8001()));/* switch to segmented (on Z8001) system mode */
                PUSH_PC();
                PUSHW(SP, fcw);       /* save current m_fcw */
                PUSHW(SP, m_irq_vec);   /* save interrupt/trap type tag */
                m_pc = GET_PC(NVI);
                m_irq_req &= unchecked((uint8_t)~Z8000_NVI);
                CHANGE_FCW(GET_FCW(NVI));
                LOG("Z8K NVI ${0}\n", m_pc);
            }
            else
            if ((m_irq_req & Z8000_VI) != 0 && (m_fcw & F_VIE) != 0)
            {
                standard_irq_callback(VI_LINE);
                m_irq_vec = m_iack_in[3].op_u16(m_pc);
                m_halt = false;

                CHANGE_FCW((uint16_t)((uint32_t)fcw | (uint32_t)F_S_N | F_SEG_Z8001()));/* switch to segmented (on Z8001) system mode */
                PUSH_PC();
                PUSHW(SP, fcw);       /* save current m_fcw */
                PUSHW(SP, m_irq_vec);   /* save interrupt/trap type tag */
                m_pc = read_irq_vector();
                m_irq_req &= unchecked((uint8_t)~Z8000_VI);
                CHANGE_FCW(GET_FCW(VI));
                LOG("Z8K VI [${0}/${1}] fcw ${2}, pc ${3}\n", m_irq_vec, VEC00 + 2 * (m_irq_vec & 0xff), m_fcw, m_pc);
            }
        }


        protected virtual uint32_t GET_PC(uint32_t VEC) { throw new emu_unimplemented(); }

        protected virtual uint32_t get_reset_pc()
        {
            return RDMEM_W(m_program, 4);
        }

        protected virtual uint16_t GET_FCW(uint32_t VEC) { throw new emu_unimplemented(); }
        protected virtual uint32_t F_SEG_Z8001() { throw new emu_unimplemented(); }
        protected virtual uint32_t PSA_ADDR() { throw new emu_unimplemented(); }
        protected virtual uint32_t read_irq_vector() { throw new emu_unimplemented(); }
    }


    //class z8001_device : public z8002_device


    /* possible values for z8k_segm_mode */
    //#define Z8K_SEGM_MODE_NONSEG 0
    //#define Z8K_SEGM_MODE_SEG    1
    //#define Z8K_SEGM_MODE_AUTO   2


    public static class z8000_global
    {
        public static z8002_device Z8002<bool_Required>(machine_config mconfig, device_finder<z8002_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, z8002_device.Z8002, clock); }
    }
}
