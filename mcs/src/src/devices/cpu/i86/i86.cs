// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_write32 = mame.devcb_write<mame.Type_constant_u32>;  //using devcb_write32 = devcb_write<u32>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.i86_global;
using static mame.util;


namespace mame
{
    //#define INPUT_LINE_INT0         INPUT_LINE_IRQ0
    //#define INPUT_LINE_TEST         20


    //enum
    //{
    //    I8086_PC = STATE_GENPC,
    //    I8086_IP = 1, I8086_AX, I8086_CX, I8086_DX, I8086_BX, I8086_SP, I8086_BP, I8086_SI, I8086_DI,
    //    I8086_AL, I8086_AH, I8086_CL, I8086_CH, I8086_DL, I8086_DH, I8086_BL, I8086_BH,
    //    I8086_FLAGS, I8086_ES, I8086_CS, I8086_SS, I8086_DS,
    //    I8086_VECTOR, I8086_HALT
    //};


    public abstract class i8086_common_cpu_device : cpu_device
                                                    //i386_disassembler::config
    {
        protected class device_execute_interface_i8086_common : device_execute_interface
        {
            public device_execute_interface_i8086_common(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override u32 execute_min_cycles() { throw new emu_unimplemented(); }
            protected override u32 execute_max_cycles() { throw new emu_unimplemented(); }
            protected override bool execute_input_edge_triggered(int inputnum) { throw new emu_unimplemented(); }
            protected override void execute_run() { throw new emu_unimplemented(); }
            protected override void execute_set_input(int inputnum, int state) { throw new emu_unimplemented(); }
        }


        public class device_state_interface_i8086_common : device_state_interface
        {
            public device_state_interface_i8086_common(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void state_import(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }
        }


        public class device_disasm_interface_i8086_common : device_disasm_interface
        {
            public device_disasm_interface_i8086_common(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
            //protected override int get_mode() { throw new emu_unimplemented(); }
        }


        protected device_state_interface_i8086_common m_distate;


#if false
        enum
        {
            EXCEPTION, IRET,                                /* EXCEPTION, iret */
            INT3, INT_IMM, INTO_NT, INTO_T,                 /* intS */
            OVERRIDE,                                       /* SEGMENT OVERRIDES */
            FLAG_OPS, LAHF, SAHF,                           /* FLAG OPERATIONS */
            AAA, AAS, AAM, AAD,                             /* ARITHMETIC ADJUSTS */
            DAA, DAS,                                       /* DECIMAL ADJUSTS */
            CBW, CWD,                                       /* SIGN EXTENSION */
            HLT, LOAD_PTR, LEA, NOP, WAIT, XLAT,            /* MISC */

            JMP_SHORT, JMP_NEAR, JMP_FAR,                   /* DIRECT jmpS */
            JMP_R16, JMP_M16, JMP_M32,                      /* INDIRECT jmpS */
            CALL_NEAR, CALL_FAR,                            /* DIRECT callS */
            CALL_R16, CALL_M16, CALL_M32,                   /* INDIRECT callS */
            RET_NEAR, RET_FAR, RET_NEAR_IMM, RET_FAR_IMM,   /* RETURNS */
            JCC_NT, JCC_T, JCXZ_NT, JCXZ_T,                 /* CONDITIONAL jmpS */
            LOOP_NT, LOOP_T, LOOPE_NT, LOOPE_T,             /* LOOPS */

            IN_IMM8, IN_IMM16, IN_DX8, IN_DX16,             /* PORT READS */
            OUT_IMM8, OUT_IMM16, OUT_DX8, OUT_DX16,         /* PORT WRITES */

            MOV_RR8, MOV_RM8, MOV_MR8,                      /* MOVE, 8-BIT */
            MOV_RI8, MOV_MI8,                               /* MOVE, 8-BIT IMMEDIATE */
            MOV_RR16, MOV_RM16, MOV_MR16,                   /* MOVE, 16-BIT */
            MOV_RI16, MOV_MI16,                             /* MOVE, 16-BIT IMMEDIATE */
            MOV_AM8, MOV_AM16, MOV_MA8, MOV_MA16,           /* MOVE, al/ax MEMORY */
            MOV_SR, MOV_SM, MOV_RS, MOV_MS,                 /* MOVE, SEGMENT REGISTERS */
            XCHG_RR8, XCHG_RM8,                             /* EXCHANGE, 8-BIT */
            XCHG_RR16, XCHG_RM16, XCHG_AR16,                /* EXCHANGE, 16-BIT */

            PUSH_R16, PUSH_M16, PUSH_SEG, PUSHF,            /* PUSHES */
            POP_R16, POP_M16, POP_SEG, POPF,                /* POPS */

            ALU_RR8, ALU_RM8, ALU_MR8,                      /* alu OPS, 8-BIT */
            ALU_RI8, ALU_MI8, ALU_MI8_RO,                   /* alu OPS, 8-BIT IMMEDIATE */
            ALU_RR16, ALU_RM16, ALU_MR16,                   /* alu OPS, 16-BIT */
            ALU_RI16, ALU_MI16, ALU_MI16_RO,                /* alu OPS, 16-BIT IMMEDIATE */
            ALU_R16I8, ALU_M16I8, ALU_M16I8_RO,             /* alu OPS, 16-BIT W/8-BIT IMMEDIATE */
            MUL_R8, MUL_R16, MUL_M8, MUL_M16,               /* mul */
            IMUL_R8, IMUL_R16, IMUL_M8, IMUL_M16,           /* imul */
            DIV_R8, DIV_R16, DIV_M8, DIV_M16,               /* div */
            IDIV_R8, IDIV_R16, IDIV_M8, IDIV_M16,           /* idiv */
            INCDEC_R8, INCDEC_R16, INCDEC_M8, INCDEC_M16,   /* inc/dec */
            NEGNOT_R8, NEGNOT_R16, NEGNOT_M8, NEGNOT_M16,   /* neg/not */

            ROT_REG_1, ROT_REG_BASE, ROT_REG_BIT,           /* REG SHIFT/ROTATE */
            ROT_M8_1, ROT_M8_BASE, ROT_M8_BIT,              /* M8 SHIFT/ROTATE */
            ROT_M16_1, ROT_M16_BASE, ROT_M16_BIT,           /* M16 SHIFT/ROTATE */

            CMPS8, REP_CMPS8_BASE, REP_CMPS8_COUNT,         /* cmps 8-BIT */
            CMPS16, REP_CMPS16_BASE, REP_CMPS16_COUNT,      /* cmps 16-BIT */
            SCAS8, REP_SCAS8_BASE, REP_SCAS8_COUNT,         /* scas 8-BIT */
            SCAS16, REP_SCAS16_BASE, REP_SCAS16_COUNT,      /* scas 16-BIT */
            LODS8, REP_LODS8_BASE, REP_LODS8_COUNT,         /* lods 8-BIT */
            LODS16, REP_LODS16_BASE, REP_LODS16_COUNT,      /* lods 16-BIT */
            STOS8, REP_STOS8_BASE, REP_STOS8_COUNT,         /* stos 8-BIT */
            STOS16, REP_STOS16_BASE, REP_STOS16_COUNT,      /* stos 16-BIT */
            MOVS8, REP_MOVS8_BASE, REP_MOVS8_COUNT,         /* movs 8-BIT */
            MOVS16, REP_MOVS16_BASE, REP_MOVS16_COUNT,      /* movs 16-BIT */

            INS8, REP_INS8_BASE, REP_INS8_COUNT,            /* (80186) ins 8-BIT */
            INS16, REP_INS16_BASE, REP_INS16_COUNT,         /* (80186) ins 16-BIT */
            OUTS8, REP_OUTS8_BASE, REP_OUTS8_COUNT,         /* (80186) outs 8-BIT */
            OUTS16, REP_OUTS16_BASE, REP_OUTS16_COUNT,      /* (80186) outs 16-BIT */
            PUSH_IMM, PUSHA, POPA,                          /* (80186) push IMMEDIATE, pusha/popa */
            IMUL_RRI8, IMUL_RMI8,                           /* (80186) imul IMMEDIATE 8-BIT */
            IMUL_RRI16, IMUL_RMI16,                         /* (80186) imul IMMEDIATE 16-BIT */
            ENTER0, ENTER1, ENTER_BASE, ENTER_COUNT, LEAVE, /* (80186) enter/leave */
            BOUND                                           /* (80186) bound */
        };
#endif

        //enum SREGS { ES=0, CS, SS, DS };
        enum WREGS { AX = 0, CX, DX, BX, SP, BP, SI, DI }


        //union
        //{                   /* eight general registers */
        //    uint16_t w[8];    /* viewed as 16 bits registers */
        //    uint8_t  b[16];   /* or as 8 bit registers */
        //} m_regs;


        //enum BREGS
        //{
        static readonly int AL = NATIVE_ENDIAN_VALUE_LE_BE(0x0, 0x1);
        static readonly int AH = NATIVE_ENDIAN_VALUE_LE_BE(0x1, 0x0);
        static readonly int CL = NATIVE_ENDIAN_VALUE_LE_BE(0x2, 0x3);
        static readonly int CH = NATIVE_ENDIAN_VALUE_LE_BE(0x3, 0x2);
        static readonly int DL = NATIVE_ENDIAN_VALUE_LE_BE(0x4, 0x5);
        static readonly int DH = NATIVE_ENDIAN_VALUE_LE_BE(0x5, 0x4);
        static readonly int BL = NATIVE_ENDIAN_VALUE_LE_BE(0x6, 0x7);
        static readonly int BH = NATIVE_ENDIAN_VALUE_LE_BE(0x7, 0x6);
        static readonly int SPL = NATIVE_ENDIAN_VALUE_LE_BE(0x8, 0x9);
        static readonly int SPH = NATIVE_ENDIAN_VALUE_LE_BE(0x9, 0x8);
        static readonly int BPL = NATIVE_ENDIAN_VALUE_LE_BE(0xa, 0xb);
        static readonly int BPH = NATIVE_ENDIAN_VALUE_LE_BE(0xb, 0xa);
        static readonly int SIL = NATIVE_ENDIAN_VALUE_LE_BE(0xc, 0xd);
        static readonly int SIH = NATIVE_ENDIAN_VALUE_LE_BE(0xd, 0xc);
        static readonly int DIL = NATIVE_ENDIAN_VALUE_LE_BE(0xe, 0xf);
        static readonly int DIH = NATIVE_ENDIAN_VALUE_LE_BE(0xf, 0xe);
        //}


        //enum {
        //    I8086_READ,
        //    I8086_WRITE,
        //    I8086_FETCH,
        //    I8086_NONE
        //};

        //uint16_t  m_sregs[4];

        uint16_t m_ip;
        //uint16_t  m_prev_ip;

        //int32_t   m_SignVal;
        //uint32_t  m_AuxVal, m_OverVal, m_ZeroVal, m_CarryVal, m_ParityVal; /* 0 or non-0 valued flags */
        uint8_t m_TF;
        uint8_t m_IF;
        uint8_t m_DF;     /* 0 or 1 valued flags */
        //uint8_t   m_IOPL, m_NT, m_MF;
        uint32_t m_int_vector;
        uint32_t m_pending_irq;
        uint32_t m_nmi_state;
        uint32_t m_irq_state;
        //uint8_t   m_no_interrupt;
        //uint8_t   m_fire_trap;
        uint8_t m_test_state;

        //address_space *m_program, *m_opcodes;
        //memory_access<20, 0, 0, ENDIANNESS_LITTLE>::cache m_cache8;
        //memory_access<20, 1, 0, ENDIANNESS_LITTLE>::cache m_cache16;

        //std::function<u8 (offs_t)> m_or8;
        //address_space *m_io;
        //int m_icount;

        //uint32_t m_prefix_seg;   /* the latest prefix segment */
        //bool m_seg_prefix;      /* prefix segment indicator */
        //bool m_seg_prefix_next; /* prefix segment for next instruction */

        //uint32_t m_ea;
        //uint16_t m_eo;
        //int m_easeg;

        // Used during execution of instructions
        //uint8_t   m_modrm;
        //uint32_t  m_dst;
        //uint32_t  m_src;
        uint32_t m_pc;

        // Lookup tables
        uint8_t [] m_parity_table = new uint8_t [256];

        class Mod_RM
        {
            public class reg_s
            {
                public int [] w = new int [256];
                public int [] b = new int [256];
            }
            public reg_s reg = new reg_s();

            public class RM_s
            {
                public int [] w = new int [256];
                public int [] b = new int [256];
            }
            public RM_s RM = new RM_s();
        }
        Mod_RM m_Mod_RM = new Mod_RM();

        protected uint8_t [] m_timing = new uint8_t [200];
        //bool m_halt;

        bool m_lock;
        devcb_write_line m_lock_handler;


        // construction/destruction
        protected i8086_common_cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_ip = 0;
            m_TF = 0;
            m_int_vector = 0;
            m_pending_irq = 0;
            m_nmi_state = 0;
            m_irq_state = 0;
            m_test_state = 1;
            m_pc = 0;
            m_lock = false;
            m_lock_handler = new devcb_write_line(this);


            int  [] reg_name = new int [8] { AL, CL, DL, BL, AH, CH, DH, BH };

            /* Set up parity lookup table. */
            for (uint16_t i = 0; i < 256; i++)
            {
                uint16_t c = 0;
                for (uint16_t j = i; j > 0; j >>= 1)
                {
                    if ((j & 1) != 0) c++;
                }
                m_parity_table[i] = (c & 1) == 0 ? (uint8_t)1 : (uint8_t)0;
            }

            for (uint16_t i = 0; i < 256; i++)
            {
                m_Mod_RM.reg.b[i] = reg_name[(i & 0x38) >> 3];
                m_Mod_RM.reg.w[i] = (int)(WREGS)( (i & 0x38) >> 3);
            }

            for (uint16_t i = 0xc0; i < 0x100; i++)
            {
                m_Mod_RM.RM.w[i] = (int)(WREGS)( i & 7 );
                m_Mod_RM.RM.b[i] = reg_name[i & 7];
            }

            //memset(&m_regs, 0x00, sizeof(m_regs));
            //memset(m_sregs, 0x00, sizeof(m_sregs));
        }


        // device-level overrides
        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_reset() { throw new emu_unimplemented(); }


        // device_execute_interface overrides
        //virtual uint32_t execute_min_cycles() const noexcept override { return 1; }
        //virtual uint32_t execute_max_cycles() const noexcept override { return 50; }
        //virtual void execute_set_input(int inputnum, int state) override;
        //virtual bool execute_input_edge_triggered(int inputnum) const noexcept override { return inputnum == INPUT_LINE_NMI; }


        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;
        //virtual int get_mode() const override;


        // device_state_interface overrides
        //virtual void state_import(const device_state_entry &entry) override;
        //virtual void state_string_export(const device_state_entry &entry, std::string &str) const override;

        protected virtual void interrupt(int int_num, int trap = 1) { throw new emu_unimplemented(); }
        protected virtual bool common_op(uint8_t op) { throw new emu_unimplemented(); }

        // Accessing memory and io
        protected virtual uint8_t read_byte(uint32_t addr) { throw new emu_unimplemented(); }
        protected virtual uint16_t read_word(uint32_t addr) { throw new emu_unimplemented(); }
        protected virtual void write_byte(uint32_t addr, uint8_t data) { throw new emu_unimplemented(); }
        protected virtual void write_word(uint32_t addr, uint16_t data) { throw new emu_unimplemented(); }
        protected virtual uint8_t read_port_byte(uint16_t port) { throw new emu_unimplemented(); }
        protected virtual uint16_t read_port_word(uint16_t port) { throw new emu_unimplemented(); }
        protected virtual void write_port_byte(uint16_t port, uint8_t data) { throw new emu_unimplemented(); }
        protected virtual void write_port_word(uint16_t port, uint16_t data) { throw new emu_unimplemented(); }


        //auto lock_handler() { return m_lock_handler.bind(); }

        // Executing instructions
        //uint8_t fetch_op() { return fetch(); }

        protected abstract uint8_t fetch();


        //inline uint16_t fetch_word();
        //inline uint8_t repx_op();

        // Cycles passed while executing instructions
        //inline void CLK(uint8_t op);
        //inline void CLKM(uint8_t op_reg, uint8_t op_mem);

        // Memory handling while executing instructions
        protected virtual uint32_t calc_addr(int seg, uint16_t offset, int size, int op, bool override_ = true) { throw new emu_unimplemented(); }


        //inline uint32_t get_ea(int size, int op);
        //inline void PutbackRMByte(uint8_t data);
        //inline void PutbackRMWord(uint16_t data);
        //inline void RegByte(uint8_t data);
        //inline void RegWord(uint16_t data);
        //inline uint8_t RegByte();
        //inline uint16_t RegWord();
        //inline uint16_t GetRMWord();
        //inline uint16_t GetnextRMWord();
        //inline uint8_t GetRMByte();
        //inline void PutMemB(int seg, uint16_t offset, uint8_t data);
        //inline void PutMemW(int seg, uint16_t offset, uint16_t data);
        //inline uint8_t GetMemB(int seg, uint16_t offset);
        //inline uint16_t GetMemW(int seg, uint16_t offset);
        //inline void PutImmRMWord();
        //inline void PutRMWord(uint16_t val);
        //inline void PutRMByte(uint8_t val);
        //inline void PutImmRMByte();
        //inline void DEF_br8();
        //inline void DEF_wr16();
        //inline void DEF_r8b();
        //inline void DEF_r16w();
        //inline void DEF_ald8();
        //inline void DEF_axd16();

        // Flags
        //inline void set_CFB(uint32_t x);
        //inline void set_CFW(uint32_t x);
        //inline void set_AF(uint32_t x,uint32_t y,uint32_t z);
        //inline void set_SF(uint32_t x);
        //inline void set_ZF(uint32_t x);
        //inline void set_PF(uint32_t x);
        //inline void set_SZPF_Byte(uint32_t x);
        //inline void set_SZPF_Word(uint32_t x);
        //inline void set_OFW_Add(uint32_t x,uint32_t y,uint32_t z);
        //inline void set_OFB_Add(uint32_t x,uint32_t y,uint32_t z);
        //inline void set_OFW_Sub(uint32_t x,uint32_t y,uint32_t z);
        //inline void set_OFB_Sub(uint32_t x,uint32_t y,uint32_t z);
        //inline uint16_t CompressFlags() const;
        //inline void ExpandFlags(uint16_t f);

        // rep instructions
        //inline void i_insb();
        //inline void i_insw();
        //inline void i_outsb();
        //inline void i_outsw();
        //inline void i_movsb();
        //inline void i_movsw();
        //inline void i_cmpsb();
        //inline void i_cmpsw();
        //inline void i_stosb();
        //inline void i_stosw();
        //inline void i_lodsb();
        //inline void i_lodsw();
        //inline void i_scasb();
        //inline void i_scasw();
        //inline void i_popf();

        // sub implementations
        //inline uint32_t ADDB();
        //inline uint32_t ADDX();
        //inline uint32_t SUBB();
        //inline uint32_t SUBX();
        //inline void ORB();
        //inline void ORW();
        //inline void ANDB();
        //inline void ANDX();
        //inline void XORB();
        //inline void XORW();
        //inline void ROL_BYTE();
        //inline void ROL_WORD();
        //inline void ROR_BYTE();
        //inline void ROR_WORD();
        //inline void ROLC_BYTE();
        //inline void ROLC_WORD();
        //inline void RORC_BYTE();
        //inline void RORC_WORD();
        //inline void SHL_BYTE(uint8_t c);
        //inline void SHL_WORD(uint8_t c);
        //inline void SHR_BYTE(uint8_t c);
        //inline void SHR_WORD(uint8_t c);
        //inline void SHRA_BYTE(uint8_t c);
        //inline void SHRA_WORD(uint8_t c);
        //inline void XchgAXReg(uint8_t reg);
        //inline void IncWordReg(uint8_t reg);
        //inline void DecWordReg(uint8_t reg);
        //inline void PUSH(uint16_t data);
        //inline uint16_t POP();
        //inline void JMP(bool cond);
        //inline void ADJ4(int8_t param1, int8_t param2);
        //inline void ADJB(int8_t param1, int8_t param2);
    }


    public class i8086_cpu_device : i8086_common_cpu_device
    {
        //DEFINE_DEVICE_TYPE(I8086, i8086_cpu_device, "i8086", "Intel 8086")
        public static readonly emu.detail.device_type_impl I8086 = DEFINE_DEVICE_TYPE("i8086", "Intel 8086", (type, mconfig, tag, owner, clock) => { return new i8086_cpu_device(mconfig, tag, owner, clock); });


        class device_execute_interface_i8086 : device_execute_interface_i8086_common
        {
            public device_execute_interface_i8086(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override u32 execute_input_lines() { throw new emu_unimplemented(); }
            protected override void execute_run() { throw new emu_unimplemented(); }
        }


        public class device_memory_interface_i8086 : device_memory_interface
        {
            public device_memory_interface_i8086(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((i8086_cpu_device)device()).device_memory_interface_memory_space_config(); }
        }


        device_memory_interface_i8086 m_dimemory;
        device_execute_interface_i8086 m_diexec;


        //enum {
        const int AS_STACK = AS_OPCODES + 1;
        const int AS_CODE  = AS_STACK + 1; // data reads from CS are still different from opcode fetches
        const int AS_EXTRA = AS_CODE + 1;
        //};


        address_space_config m_program_config;
        address_space_config m_opcodes_config;
        address_space_config m_stack_config;
        address_space_config m_code_config;
        address_space_config m_extra_config;
        address_space_config m_io_config;
        //static const uint8_t m_i8086_timing[200];
        devcb_write_line m_out_if_func;
        devcb_write32 m_esc_opcode_handler;
        devcb_write32 m_esc_data_handler;

        //address_space *m_stack, *m_code, *m_extra;


        protected static readonly uint8_t [] m_i8086_timing =
        {
            51,32,          /* exception, IRET */
                2, 0, 4, 2, /* INTs */
                2,              /* segment overrides */
                2, 4, 4,        /* flag operations */
                4, 4,83,60, /* arithmetic adjusts */
                4, 4,           /* decimal adjusts */
                2, 5,           /* sign extension */
                2,24, 2, 2, 3,11,   /* misc */

            15,15,15,       /* direct JMPs */
            11,18,24,       /* indirect JMPs */
            19,28,          /* direct CALLs */
            16,21,37,       /* indirect CALLs */
            20,32,24,31,    /* returns */
                4,16, 6,18, /* conditional JMPs */
                5,17, 6,18, /* loops */

            10,14, 8,12,    /* port reads */
            10,14, 8,12,    /* port writes */

                2, 8, 9,        /* move, 8-bit */
                4,10,           /* move, 8-bit immediate */
                2, 8, 9,        /* move, 16-bit */
                4,10,           /* move, 16-bit immediate */
            10,10,10,10,    /* move, AL/AX memory */
                2, 8, 2, 9, /* move, segment registers */
                4,17,           /* exchange, 8-bit */
                4,17, 3,        /* exchange, 16-bit */

            15,24,14,14,    /* pushes */
            12,25,12,12,    /* pops */

                3, 9,16,        /* ALU ops, 8-bit */
                4,17,10,        /* ALU ops, 8-bit immediate */
                3, 9,16,        /* ALU ops, 16-bit */
                4,17,10,        /* ALU ops, 16-bit immediate */
                4,17,10,        /* ALU ops, 16-bit w/8-bit immediate */
            70,118,76,128,  /* MUL */
            80,128,86,138,  /* IMUL */
            80,144,86,154,  /* DIV */
            101,165,107,175,/* IDIV */
                3, 2,15,15, /* INC/DEC */
                3, 3,16,16, /* NEG/NOT */

                2, 8, 4,        /* reg shift/rotate */
            15,20, 4,       /* m8 shift/rotate */
            15,20, 4,       /* m16 shift/rotate */

            22, 9,21,       /* CMPS 8-bit */
            22, 9,21,       /* CMPS 16-bit */
            15, 9,14,       /* SCAS 8-bit */
            15, 9,14,       /* SCAS 16-bit */
            12, 9,11,       /* LODS 8-bit */
            12, 9,11,       /* LODS 16-bit */
            11, 9,10,       /* STOS 8-bit */
            11, 9,10,       /* STOS 16-bit */
            18, 9,17,       /* MOVS 8-bit */
            18, 9,17,       /* MOVS 16-bit */
        };


        // construction/destruction
        i8086_cpu_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, I8086, tag, owner, clock, 16)
        {
            throw new emu_unimplemented();
#if false
            memcpy(m_timing, m_i8086_timing, sizeof(m_i8086_timing));
#endif
        }


        protected i8086_cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, int data_bus_size)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_memory_interface_i8086(mconfig, this));
            m_class_interfaces.Add(new device_execute_interface_i8086(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_i8086_common(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_i8086_common(mconfig, this));
            m_dimemory = GetClassInterface<device_memory_interface_i8086>();
            m_diexec = GetClassInterface<device_execute_interface_i8086>();
            m_distate = GetClassInterface<device_state_interface_i8086_common>();


            m_program_config = new address_space_config("program", ENDIANNESS_LITTLE, (uint8_t)data_bus_size, 20, 0);
            m_opcodes_config = new address_space_config("opcodes", ENDIANNESS_LITTLE, (uint8_t)data_bus_size, 20, 0);
            m_stack_config = new address_space_config("stack", ENDIANNESS_LITTLE, (uint8_t)data_bus_size, 20, 0);
            m_code_config = new address_space_config("code", ENDIANNESS_LITTLE, (uint8_t)data_bus_size, 20, 0);
            m_extra_config = new address_space_config("extra", ENDIANNESS_LITTLE, (uint8_t)data_bus_size, 20, 0);
            m_io_config = new address_space_config("io", ENDIANNESS_LITTLE, (uint8_t)data_bus_size, 16, 0);
            m_out_if_func = new devcb_write_line(this);
            m_esc_opcode_handler = new devcb_write32(this);
            m_esc_data_handler = new devcb_write32(this);
        }


        // device_memory_interface overrides
        //virtual space_config_vector memory_space_config() const override;
        space_config_vector device_memory_interface_memory_space_config()
        {
            space_config_vector spaces = new space_config_vector
            {
                std.make_pair(AS_PROGRAM, m_program_config),
                std.make_pair(AS_IO,      m_io_config)
            };

            if (memory().has_configured_map(AS_OPCODES))
                spaces.push_back(std.make_pair(AS_OPCODES, m_opcodes_config));
            if (memory().has_configured_map(AS_STACK))
                spaces.push_back(std.make_pair(AS_STACK, m_stack_config));
            if (memory().has_configured_map(AS_CODE))
                spaces.push_back(std.make_pair(AS_CODE, m_code_config));
            if (memory().has_configured_map(AS_EXTRA))
                spaces.push_back(std.make_pair(AS_EXTRA, m_extra_config));

            return spaces;
        }


        //auto if_handler() { return m_out_if_func.bind(); }
        //auto esc_opcode_handler() { return m_esc_opcode_handler.bind(); }
        //auto esc_data_handler() { return m_esc_data_handler.bind(); }


        //virtual void execute_run() override;


        protected override void device_start() { throw new emu_unimplemented(); }


        //virtual uint32_t execute_input_lines() const noexcept override { return 1; }

        protected override uint8_t fetch() { throw new emu_unimplemented(); }


        //inline address_space *sreg_to_space(int sreg) const;


        protected override uint8_t read_byte(uint32_t addr) { throw new emu_unimplemented(); }
        protected override uint16_t read_word(uint32_t addr) { throw new emu_unimplemented(); }
        protected override void write_byte(uint32_t addr, uint8_t data) { throw new emu_unimplemented(); }
        protected override void write_word(uint32_t addr, uint16_t data) { throw new emu_unimplemented(); }


        //uint32_t update_pc() { return m_pc = (m_sregs[CS] << 4) + m_ip; }
    }


    public class i8088_cpu_device : i8086_cpu_device
    {
        //DEFINE_DEVICE_TYPE(I8088, i8088_cpu_device, "i8088", "Intel 8088")
        public static readonly emu.detail.device_type_impl I8088 = DEFINE_DEVICE_TYPE("i8088", "Intel 8088", (type, mconfig, tag, owner, clock) => { return new i8088_cpu_device(mconfig, tag, owner, clock); });


        // construction/destruction
        i8088_cpu_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, I8088, tag, owner, clock, 8)
        {
            Array.Copy(m_i8086_timing, m_timing, m_i8086_timing.Length);  //memcpy(m_timing, m_i8086_timing, sizeof(m_i8086_timing));
        }
    }



    public static class i86_global
    {
        public static i8088_cpu_device I8088<bool_Required>(machine_config mconfig, device_finder<i8088_cpu_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, i8088_cpu_device.I8088, clock); }
    }
}
