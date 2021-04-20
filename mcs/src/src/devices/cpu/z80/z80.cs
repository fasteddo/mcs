// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


/* On an NMOS Z80, if LD A,I or LD A,R is interrupted, P/V flag gets reset,
    even if IFF2 was set before this instruction. This issue was fixed on
    the CMOS Z80, so until knowing (most) Z80 types on hardware, it's disabled */
//#define HAS_LDAIR_QUIRK


namespace mame
{
    public class z80_device : cpu_device
                              //z80_daisy_chain_interface
    {
        //DEFINE_DEVICE_TYPE(Z80, z80_device, "z80", "Zilog Z80")
        static device_t device_creator_z80_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new z80_device(mconfig, tag, owner, clock); }
        public static readonly device_type Z80 = DEFINE_DEVICE_TYPE(device_creator_z80_device, "z80", "Zilog Z80");


        public class device_execute_interface_z80 : device_execute_interface
        {
            public device_execute_interface_z80(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint32_t execute_min_cycles() { return 2; }
            protected override uint32_t execute_max_cycles() { return 16; }
            protected override uint32_t execute_input_lines() { return 4; }
            protected override uint32_t execute_default_irq_vector(int inputnum) { return 0xff; }
            protected override bool execute_input_edge_triggered(int inputnum) { return ((z80_device)device()).device_execute_interface_execute_input_edge_triggered(inputnum); }
            protected override void execute_run() { ((z80_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { ((z80_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
        }


        public class device_memory_interface_z80 : device_memory_interface
        {
            public device_memory_interface_z80(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((z80_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_z80 : device_state_interface
        {
            public device_state_interface_z80(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void state_import(device_state_entry entry) { ((z80_device)device()).device_state_interface_state_import(entry); }
            protected override void state_export(device_state_entry entry) { ((z80_device)device()).device_state_interface_state_export(entry); }
            protected override void state_string_export(device_state_entry entry, out string str) { ((z80_device)device()).device_state_interface_state_string_export(entry, out str); }
        }


        public class device_disasm_interface_z80 : device_disasm_interface
        {
            public device_disasm_interface_z80(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        //enum
        //{
        const int NSC800_RSTA = device_execute_interface.INPUT_LINE_IRQ0 + 1;
        const int NSC800_RSTB = device_execute_interface.INPUT_LINE_IRQ0 + 2;
        const int NSC800_RSTC = device_execute_interface.INPUT_LINE_IRQ0 + 3;
        const int Z80_INPUT_LINE_WAIT  = device_execute_interface.INPUT_LINE_IRQ0 + 4;
        const int Z80_INPUT_LINE_BOGUSWAIT = device_execute_interface.INPUT_LINE_IRQ0 + 5; /* WAIT pin implementation used to be nonexistent, please remove this when all drivers are updated with Z80_INPUT_LINE_WAIT */
        public const int Z80_INPUT_LINE_BUSRQ = device_execute_interface.INPUT_LINE_IRQ0 + 6;
        //}


        //enum
        //{
        const int Z80_PC   =  STATE_GENPC;
        const int Z80_SP   =  1;
        const int Z80_A    =  2;
        const int Z80_B    =  3;
        const int Z80_C    =  4;
        const int Z80_D    =  5;
        const int Z80_E    =  6;
        const int Z80_H    =  7;
        const int Z80_L    =  8;
        const int Z80_AF   =  9;
        const int Z80_BC   = 10;
        const int Z80_DE   = 11;
        const int Z80_HL   = 12;
        const int Z80_IX   = 13;
        const int Z80_IY   = 14;
        const int Z80_AF2  = 15;
        const int Z80_BC2  = 16;
        const int Z80_DE2  = 17;
        const int Z80_HL2  = 18;
        const int Z80_R    = 19;
        const int Z80_I    = 20;
        const int Z80_IM   = 21;
        const int Z80_IFF1 = 22;
        const int Z80_IFF2 = 23;
        const int Z80_HALT = 24;
        const int Z80_DC0  = 25;
        const int Z80_DC1  = 26;
        const int Z80_DC2  = 27;
        const int Z80_DC3  = 28;
        const int Z80_WZ   = 29;
        //}


        const bool VERBOSE = false;
        void LOG(string format, params object [] args) { if (VERBOSE) logerror(format, args); }


        static bool tables_initialised = false;
        static uint8_t [] SZ = new uint8_t[256];       /* zero and sign flags */
        static uint8_t [] SZ_BIT = new uint8_t[256];   /* zero, sign and parity/overflow (=zero) flags for BIT opcode */
        static uint8_t [] SZP = new uint8_t[256];      /* zero, sign and parity flags */
        static uint8_t [] SZHV_inc = new uint8_t[256]; /* zero, sign, half carry and overflow flags INC r8 */
        static uint8_t [] SZHV_dec = new uint8_t[256]; /* zero, sign, half carry and overflow flags DEC r8 */

        static uint8_t [] SZHVC_add = new uint8_t[2*256*256];
        static uint8_t [] SZHVC_sub = new uint8_t[2*256*256];

        static readonly uint8_t [] cc_op = new uint8_t[0x100]
        {
            4,10, 7, 6, 4, 4, 7, 4, 4,11, 7, 6, 4, 4, 7, 4,
            8,10, 7, 6, 4, 4, 7, 4,12,11, 7, 6, 4, 4, 7, 4,
            7,10,16, 6, 4, 4, 7, 4, 7,11,16, 6, 4, 4, 7, 4,
            7,10,13, 6,11,11,10, 4, 7,11,13, 6, 4, 4, 7, 4,
            4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
            4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
            4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
            7, 7, 7, 7, 7, 7, 4, 7, 4, 4, 4, 4, 4, 4, 7, 4,
            4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
            4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
            4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
            4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
            5,10,10,10,10,11, 7,11, 5,10,10, 0,10,17, 7,11, /* cb -> cc_cb */
            5,10,10,11,10,11, 7,11, 5, 4,10,11,10, 0, 7,11, /* dd -> cc_xy */
            5,10,10,19,10,11, 7,11, 5, 4,10, 4,10, 0, 7,11, /* ed -> cc_ed */
            5,10,10, 4,10,11, 7,11, 5, 6,10, 4,10, 0, 7,11  /* fd -> cc_xy */
        };

        static readonly uint8_t [] cc_cb = new uint8_t[0x100]
        {
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,12, 8, 8, 8, 8, 8, 8, 8,12, 8,
            8, 8, 8, 8, 8, 8,12, 8, 8, 8, 8, 8, 8, 8,12, 8,
            8, 8, 8, 8, 8, 8,12, 8, 8, 8, 8, 8, 8, 8,12, 8,
            8, 8, 8, 8, 8, 8,12, 8, 8, 8, 8, 8, 8, 8,12, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8,
            8, 8, 8, 8, 8, 8,15, 8, 8, 8, 8, 8, 8, 8,15, 8
        };

        static readonly uint8_t [] cc_ed = new uint8_t[0x100]
        {
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            12,12,15,20,8,14, 8, 9,12,12,15,20, 8,14, 8, 9,
            12,12,15,20,8,14, 8, 9,12,12,15,20, 8,14, 8, 9,
            12,12,15,20,8,14, 8,18,12,12,15,20, 8,14, 8,18,
            12,12,15,20,8,14, 8, 8,12,12,15,20, 8,14, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            16,16,16,16,8, 8, 8, 8,16,16,16,16, 8, 8, 8, 8,
            16,16,16,16,8, 8, 8, 8,16,16,16,16, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8,
            8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8
        };

        /* ix/iy: with the exception of (i+offset) opcodes, t-states are main_opcode_table + 4 */
        static readonly uint8_t [] cc_xy = new uint8_t[0x100]
        {
            4+4,10+4, 7+4, 6+4, 4+4, 4+4, 7+4, 4+4, 4+4,11+4, 7+4, 6+4, 4+4, 4+4, 7+4, 4+4,
            8+4,10+4, 7+4, 6+4, 4+4, 4+4, 7+4, 4+4,12+4,11+4, 7+4, 6+4, 4+4, 4+4, 7+4, 4+4,
            7+4,10+4,16+4, 6+4, 4+4, 4+4, 7+4, 4+4, 7+4,11+4,16+4, 6+4, 4+4, 4+4, 7+4, 4+4,
            7+4,10+4,13+4, 6+4,23  ,23  ,19  , 4+4, 7+4,11+4,13+4, 6+4, 4+4, 4+4, 7+4, 4+4,
            4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4, 4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4,
            4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4, 4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4,
            4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4, 4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4,
            19  ,19  ,19  ,19  ,19  ,19  , 4+4,19  , 4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4,
            4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4, 4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4,
            4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4, 4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4,
            4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4, 4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4,
            4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4, 4+4, 4+4, 4+4, 4+4, 4+4, 4+4,19  , 4+4,
            5+4,10+4,10+4,10+4,10+4,11+4, 7+4,11+4, 5+4,10+4,10+4, 0  ,10+4,17+4, 7+4,11+4, /* cb -> cc_xycb */
            5+4,10+4,10+4,11+4,10+4,11+4, 7+4,11+4, 5+4, 4+4,10+4,11+4,10+4, 4  , 7+4,11+4, /* dd -> cc_xy again */
            5+4,10+4,10+4,19+4,10+4,11+4, 7+4,11+4, 5+4, 4+4,10+4, 4+4,10+4, 4  , 7+4,11+4, /* ed -> cc_ed */
            5+4,10+4,10+4, 4+4,10+4,11+4, 7+4,11+4, 5+4, 6+4,10+4, 4+4,10+4, 4  , 7+4,11+4  /* fd -> cc_xy again */
        };

        static readonly uint8_t [] cc_xycb = new uint8_t[0x100]
        {
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,
            20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,
            20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,
            20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,20,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,
            23,23,23,23,23,23,23,23,23,23,23,23,23,23,23,23
        };

        /* extra cycles if jr/jp/call taken and 'interrupt latency' on rst 0-7 */
        static readonly uint8_t [] cc_ex = new uint8_t[0x100]
        {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            5, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, /* DJNZ */
            5, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, /* JR NZ/JR Z */
            5, 0, 0, 0, 0, 0, 0, 0, 5, 0, 0, 0, 0, 0, 0, 0, /* JR NC/JR C */
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            5, 5, 5, 5, 0, 0, 0, 0, 5, 5, 5, 5, 0, 0, 0, 0, /* LDIR/CPIR/INIR/OTIR LDDR/CPDR/INDR/OTDR */
            6, 0, 0, 0, 7, 0, 0, 2, 6, 0, 0, 0, 7, 0, 0, 2,
            6, 0, 0, 0, 7, 0, 0, 2, 6, 0, 0, 0, 7, 0, 0, 2,
            6, 0, 0, 0, 7, 0, 0, 2, 6, 0, 0, 0, 7, 0, 0, 2,
            6, 0, 0, 0, 7, 0, 0, 2, 6, 0, 0, 0, 7, 0, 0, 2
        };


        device_memory_interface_z80 m_dimemory;
        device_execute_interface_z80 m_diexec;
        device_state_interface_z80 m_distate;


        z80_daisy_chain_interface m_daisy;


        // address spaces
        address_space_config m_program_config;
        address_space_config m_opcodes_config;
        address_space_config m_io_config;
        memory_access.cache m_args = new memory_access(16, 0, 0, endianness_t.ENDIANNESS_LITTLE).m_cache;  //memory_access<16, 0, 0, ENDIANNESS_LITTLE>::cache m_args;
        memory_access.cache m_opcodes = new memory_access(16, 0, 0, endianness_t.ENDIANNESS_LITTLE).m_cache;  //memory_access<16, 0, 0, ENDIANNESS_LITTLE>::cache m_opcodes;
        memory_access.specific m_data = new memory_access(16, 0, 0, endianness_t.ENDIANNESS_LITTLE).m_specific;  //memory_access<16, 0, 0, ENDIANNESS_LITTLE>::specific m_data;
        memory_access.specific m_io = new memory_access(16, 0, 0, endianness_t.ENDIANNESS_LITTLE).m_specific;  //memory_access<16, 0, 0, ENDIANNESS_LITTLE>::specific m_io;

        devcb_write_line m_irqack_cb;
        devcb_write8 m_refresh_cb;
        devcb_write_line m_halt_cb;

        PAIR m_prvpc;
        PAIR m_pc;
        PAIR m_sp;
        PAIR m_af;
        PAIR m_bc;
        PAIR m_de;
        PAIR m_hl;
        PAIR m_ix;
        PAIR m_iy;
        PAIR m_wz;
        PAIR m_af2;
        PAIR m_bc2;
        PAIR m_de2;
        PAIR m_hl2;
        uint8_t m_r;
        uint8_t m_r2;
        uint8_t m_iff1;
        uint8_t m_iff2;
        uint8_t m_halt;
        uint8_t m_im;
        uint8_t m_i;
        uint8_t m_nmi_state;          /* nmi line state */
        bool m_nmi_pending;        /* nmi pending */
        uint8_t m_irq_state;          /* irq line state */
        int m_wait_state;         // wait line state
        int m_busrq_state;        // bus request line state
        bool m_after_ei;           /* are we in the EI shadow? */
        bool m_after_ldair;        /* same, but for LD A,I or LD A,R */
        uint32_t m_ea;

        intref m_icount = new intref();  //int m_icount;
        uint8_t m_rtemp;
        uint8_t [] m_cc_op;  //const uint8_t *   m_cc_op;
        uint8_t [] m_cc_cb;
        uint8_t [] m_cc_ed;
        uint8_t [] m_cc_xy;
        uint8_t [] m_cc_xycb;
        uint8_t [] m_cc_ex;
        //#define m_cc_dd   m_cc_xy
        //#define m_cc_fd   m_cc_xy
        uint8_t [] m_cc_dd;
        uint8_t [] m_cc_fd;


        /****************************************************************************/
        /* The Z80 registers. halt is set to 1 when the CPU is halted, the refresh  */
        /* register is calculated as follows: refresh=(r&127)|(r2&128)    */
        /****************************************************************************/

        const uint8_t CF      = 0x01;
        const uint8_t NF      = 0x02;
        const uint8_t PF      = 0x04;
        const uint8_t VF      = PF;
        const uint8_t XF      = 0x08;
        const uint8_t HF      = 0x10;
        const uint8_t YF      = 0x20;
        const uint8_t ZF      = 0x40;
        const uint8_t SF      = 0x80;

        //#define INT_IRQ 0x01
        //#define NMI_IRQ 0x02

        uint32_t PRVPC { get { return m_prvpc.d; } set { m_prvpc.d = value; } }     /* previous program counter */

        uint32_t PCD { get { return m_pc.d; } set { m_pc.d = value; } }
        uint16_t PC { get { return m_pc.w.l; } set { m_pc.w.l = value; } }

        uint32_t SPD { get { return m_sp.d; } set { m_sp.d = value; } }
        uint16_t SP { get { return m_sp.w.l; } set { m_sp.w.l = value; } }

        uint32_t AFD { get { return m_af.d; } set { m_af.d = value; } }
        uint16_t AF { get { return m_af.w.l; } set { m_af.w.l = value; } }
        uint8_t A { get { return m_af.b.h; } set { m_af.b.h = value; } }
        uint8_t F { get { return m_af.b.l; } set { m_af.b.l = value; } }

        uint32_t BCD { get { return m_bc.d; } set { m_bc.d = value; } }
        uint16_t BC { get { return m_bc.w.l; } set { m_bc.w.l = value; } }
        uint8_t B { get { return m_bc.b.h; } set { m_bc.b.h = value; } }
        uint8_t C { get { return m_bc.b.l; } set { m_bc.b.l = value; } }

        uint32_t DED { get { return m_de.d; } set { m_de.d = value; } }
        uint16_t DE { get { return m_de.w.l; } set { m_de.w.l = value; } }
        uint8_t D { get { return m_de.b.h; } set { m_de.b.h = value; } }
        uint8_t E { get { return m_de.b.l; } set { m_de.b.l = value; } }

        uint32_t HLD { get { return m_hl.d; } set { m_hl.d = value; } }
        uint16_t HL { get { return m_hl.w.l; } set { m_hl.w.l = value; } }
        uint8_t H { get { return m_hl.b.h; } set { m_hl.b.h = value; } }
        uint8_t L { get { return m_hl.b.l; } set { m_hl.b.l = value; } }

        uint32_t IXD { get { return m_ix.d; } set { m_ix.d = value; } }
        uint16_t IX { get { return m_ix.w.l; } set { m_ix.w.l = value; } }
        uint8_t HX { get { return m_ix.b.h; } set { m_ix.b.h = value; } }
        uint8_t LX { get { return m_ix.b.l; } set { m_ix.b.l = value; } }

        uint32_t IYD { get { return m_iy.d; } set { m_iy.d = value; } }
        uint16_t IY { get { return m_iy.w.l; } set { m_iy.w.l = value; } }
        uint8_t HY { get { return m_iy.b.h; } set { m_iy.b.h = value; } }
        uint8_t LY { get { return m_iy.b.l; } set { m_iy.b.l = value; } }

        uint16_t WZ { get { return m_wz.w.l; } set { m_wz.w.l = value; } }
        uint8_t WZ_H { get { return m_wz.b.h; } set { m_wz.b.h = value; } }
        uint8_t WZ_L { get { return m_wz.b.l; } set { m_wz.b.l = value; } }


        public z80_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : this(mconfig, Z80, tag, owner, clock)
        {
        }


        public z80_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_z80(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_z80(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_z80(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_z80(mconfig, this));

            m_daisy = new z80_daisy_chain_interface(mconfig, this);


            m_program_config = new address_space_config("program", endianness_t.ENDIANNESS_LITTLE, 8, 16, 0);
            m_opcodes_config = new address_space_config("opcodes", endianness_t.ENDIANNESS_LITTLE, 8, 16, 0);
            m_io_config = new address_space_config("io", endianness_t.ENDIANNESS_LITTLE, 8, 16, 0);
            m_irqack_cb = new devcb_write_line(this);
            m_refresh_cb = new devcb_write8(this);
            m_halt_cb = new devcb_write_line(this);
        }


        //void z80_set_cycle_tables(const uint8_t *op, const uint8_t *cb, const uint8_t *ed, const uint8_t *xy, const uint8_t *xycb, const uint8_t *ex);
        //template <typename... T> void set_memory_map(T &&... args) { set_addrmap(AS_PROGRAM, std::forward<T>(args)...); }
        //template <typename... T> void set_m1_map(T &&... args) { set_addrmap(AS_OPCODES, std::forward<T>(args)...); }
        //template <typename... T> void set_io_map(T &&... args) { set_addrmap(AS_IO, std::forward<T>(args)...); }
        //auto irqack_cb() { return m_irqack_cb.bind(); }
        //auto refresh_cb() { return m_refresh_cb.bind(); }
        //auto halt_cb() { return m_halt_cb.bind(); }


        // device-level overrides

        /****************************************************************************
         * Processor initialization
         ****************************************************************************/
        protected override void device_start()
        {
            m_dimemory = GetClassInterface<device_memory_interface_z80>();
            m_diexec = GetClassInterface<device_execute_interface_z80>();
            m_distate = GetClassInterface<device_state_interface_z80>();


            if (!tables_initialised)
            {
                //UINT8 *padd = &SZHVC_add[  0*256];
                //UINT8 *padc = &SZHVC_add[256*256];
                //UINT8 *psub = &SZHVC_sub[  0*256];
                //UINT8 *psbc = &SZHVC_sub[256*256];
                UInt32 paddIdx =   0*256;
                UInt32 padcIdx = 256*256;
                UInt32 psubIdx =   0*256;
                UInt32 psbcIdx = 256*256;
                for (int oldval = 0; oldval < 256; oldval++)
                {
                    for (int newval = 0; newval < 256; newval++)
                    {
                        /* add or adc w/o carry set */
                        int val = newval - oldval;
                        //*padd = (newval) ? ((newval & 0x80) ? SF : 0) : ZF;
                        //*padd |= (newval & (YF | XF));  /* undocumented flag bits 5+3 */
                        SZHVC_add[paddIdx] = (newval) != 0 ? ((newval & 0x80) != 0 ? SF : (byte)0) : ZF;
                        SZHVC_add[paddIdx] |= (byte)(newval & (YF | XF));  /* undocumented flag bits 5+3 */
                        if ( (newval & 0x0f) < (oldval & 0x0f) ) SZHVC_add[paddIdx] |= HF;
                        if ( newval < oldval ) SZHVC_add[paddIdx] |= CF;
                        if (( (val^oldval^0x80) & (val^newval) & 0x80 ) != 0) SZHVC_add[paddIdx] |= VF;
                        paddIdx++;

                        /* adc with carry set */
                        val = newval - oldval - 1;
                        SZHVC_add[padcIdx] = (newval) != 0 ? ((newval & 0x80) != 0 ? SF : (byte)0) : ZF;
                        SZHVC_add[padcIdx] |= (byte)(newval & (YF | XF));  /* undocumented flag bits 5+3 */
                        if ( (newval & 0x0f) <= (oldval & 0x0f) ) SZHVC_add[padcIdx] |= HF;
                        if ( newval <= oldval ) SZHVC_add[padcIdx] |= CF;
                        if (( (val^oldval^0x80) & (val^newval) & 0x80 ) != 0) SZHVC_add[padcIdx] |= VF;
                        padcIdx++;

                        /* cp, sub or sbc w/o carry set */
                        val = oldval - newval;
                        SZHVC_sub[psubIdx] = (byte)(NF | ((newval) != 0 ? ((newval & 0x80) != 0 ? SF : (byte)0) : ZF));
                        SZHVC_sub[psubIdx] |= (byte)(newval & (YF | XF));  /* undocumented flag bits 5+3 */
                        if ( (newval & 0x0f) > (oldval & 0x0f) ) SZHVC_sub[psubIdx] |= HF;
                        if ( newval > oldval ) SZHVC_sub[psubIdx] |= CF;
                        if (( (val^oldval) & (oldval^newval) & 0x80 ) != 0) SZHVC_sub[psubIdx] |= VF;
                        psubIdx++;

                        /* sbc with carry set */
                        val = oldval - newval - 1;
                        SZHVC_sub[psbcIdx] = (byte)(NF | ((newval) != 0 ? ((newval & 0x80) != 0 ? SF : (byte)0) : ZF));
                        SZHVC_sub[psbcIdx] |= (byte)(newval & (YF | XF));  /* undocumented flag bits 5+3 */
                        if ( (newval & 0x0f) >= (oldval & 0x0f) ) SZHVC_sub[psbcIdx] |= HF;
                        if ( newval >= oldval ) SZHVC_sub[psbcIdx] |= CF;
                        if (( (val^oldval) & (oldval^newval) & 0x80 ) != 0) SZHVC_sub[psbcIdx] |= VF;
                        psbcIdx++;
                    }
                }

                for (int i = 0; i < 256; i++)
                {
                    int p = 0;
                    if (( i&0x01 ) != 0) ++p;
                    if (( i&0x02 ) != 0) ++p;
                    if (( i&0x04 ) != 0) ++p;
                    if (( i&0x08 ) != 0) ++p;
                    if (( i&0x10 ) != 0) ++p;
                    if (( i&0x20 ) != 0) ++p;
                    if (( i&0x40 ) != 0) ++p;
                    if (( i&0x80 ) != 0) ++p;
                    SZ[i] = i != 0 ? (byte)(i & SF) : ZF;
                    SZ[i] |= (byte)(i & (YF | XF));       /* undocumented flag bits 5+3 */
                    SZ_BIT[i] = i != 0 ? (byte)(i & SF) : (byte)(ZF | PF);
                    SZ_BIT[i] |= (byte)(i & (YF | XF));   /* undocumented flag bits 5+3 */
                    SZP[i] = (byte)(SZ[i] | ((p & 1) != 0 ? (byte)0 : PF));
                    SZHV_inc[i] = SZ[i];
                    if( i == 0x80 ) SZHV_inc[i] |= VF;
                    if( (i & 0x0f) == 0x00 ) SZHV_inc[i] |= HF;
                    SZHV_dec[i] = (byte)(SZ[i] | NF);
                    if( i == 0x7f ) SZHV_dec[i] |= VF;
                    if( (i & 0x0f) == 0x0f ) SZHV_dec[i] |= HF;
                }

                tables_initialised = true;
            }

            save_item(NAME(new { m_prvpc.w.l }));
            save_item(NAME(new { PC }));
            save_item(NAME(new { SP }));
            save_item(NAME(new { AF }));
            save_item(NAME(new { BC }));
            save_item(NAME(new { DE }));
            save_item(NAME(new { HL }));
            save_item(NAME(new { IX }));
            save_item(NAME(new { IY }));
            save_item(NAME(new { WZ }));
            save_item(NAME(new { m_af2.w.l }));
            save_item(NAME(new { m_bc2.w.l }));
            save_item(NAME(new { m_de2.w.l }));
            save_item(NAME(new { m_hl2.w.l }));
            save_item(NAME(new { m_r }));
            save_item(NAME(new { m_r2 }));
            save_item(NAME(new { m_iff1 }));
            save_item(NAME(new { m_iff2 }));
            save_item(NAME(new { m_halt }));
            save_item(NAME(new { m_im }));
            save_item(NAME(new { m_i }));
            save_item(NAME(new { m_nmi_state }));
            save_item(NAME(new { m_nmi_pending }));
            save_item(NAME(new { m_irq_state }));
            save_item(NAME(new { m_wait_state }));
            save_item(NAME(new { m_busrq_state }));
            save_item(NAME(new { m_after_ei }));
            save_item(NAME(new { m_after_ldair }));

            /* Reset registers to their initial values */
            PRVPC = 0;
            PCD = 0;
            SPD = 0;
            AFD = 0;
            BCD = 0;
            DED = 0;
            HLD = 0;
            IXD = 0;
            IYD = 0;
            WZ = 0;
            m_af2.d = 0;
            m_bc2.d = 0;
            m_de2.d = 0;
            m_hl2.d = 0;
            m_r = 0;
            m_r2 = 0;
            m_iff1 = 0;
            m_iff2 = 0;
            m_halt = 0;
            m_im = 0;
            m_i = 0;
            m_nmi_state = 0;
            m_nmi_pending = false;
            m_irq_state = 0;
            m_wait_state = 0;
            m_busrq_state = 0;
            m_after_ei = false;
            m_after_ldair = false;
            m_ea = 0;

            m_dimemory.space(AS_PROGRAM).cache(m_args.Width, m_args.AddrShift, m_args.Endian, m_args);
            m_dimemory.space(m_dimemory.has_space(AS_OPCODES) ? AS_OPCODES : AS_PROGRAM).cache(m_opcodes.Width, m_opcodes.AddrShift, m_opcodes.Endian, m_opcodes);
            m_dimemory.space(AS_PROGRAM).specific(m_data.Level, m_data.Width, m_data.AddrShift, m_data.Endian, m_data);
            m_dimemory.space(AS_IO).specific(m_io.Level, m_io.Width, m_io.AddrShift, m_io.Endian, m_io);

            IX = IY = 0xffff; /* IX and IY are FFFF after a reset! */
            F = ZF;           /* Zero flag is set */

            /* set up the state table */
            m_distate.state_add(STATE_GENPC,     "PC",        m_pc.w.l).callimport();
            m_distate.state_add(STATE_GENPCBASE, "CURPC",     m_prvpc.w.l).callimport().noshow();
            m_distate.state_add(Z80_SP,          "SP",        SP);
            m_distate.state_add(STATE_GENSP,     "GENSP",     SP).noshow();
            m_distate.state_add(STATE_GENFLAGS,  "GENFLAGS",  F).noshow().formatstr("%8s");
            m_distate.state_add(Z80_A,           "A",         A).noshow();
            m_distate.state_add(Z80_B,           "B",         B).noshow();
            m_distate.state_add(Z80_C,           "C",         C).noshow();
            m_distate.state_add(Z80_D,           "D",         D).noshow();
            m_distate.state_add(Z80_E,           "E",         E).noshow();
            m_distate.state_add(Z80_H,           "H",         H).noshow();
            m_distate.state_add(Z80_L,           "L",         L).noshow();
            m_distate.state_add(Z80_AF,          "AF",        AF);
            m_distate.state_add(Z80_BC,          "BC",        BC);
            m_distate.state_add(Z80_DE,          "DE",        DE);
            m_distate.state_add(Z80_HL,          "HL",        HL);
            m_distate.state_add(Z80_IX,          "IX",        IX);
            m_distate.state_add(Z80_IY,          "IY",        IY);
            m_distate.state_add(Z80_AF2,         "AF2",       m_af2.w.l);
            m_distate.state_add(Z80_BC2,         "BC2",       m_bc2.w.l);
            m_distate.state_add(Z80_DE2,         "DE2",       m_de2.w.l);
            m_distate.state_add(Z80_HL2,         "HL2",       m_hl2.w.l);
            m_distate.state_add(Z80_WZ,          "WZ",        WZ);
            m_distate.state_add(Z80_R,           "R",         m_rtemp).callimport().callexport();
            m_distate.state_add(Z80_I,           "I",         m_i);
            m_distate.state_add(Z80_IM,          "IM",        m_im).mask(0x3);
            m_distate.state_add(Z80_IFF1,        "IFF1",      m_iff1).mask(0x1);
            m_distate.state_add(Z80_IFF2,        "IFF2",      m_iff2).mask(0x1);
            m_distate.state_add(Z80_HALT,        "HALT",      m_halt).mask(0x1);

            // set our instruction counter
            set_icountptr(m_icount);

            /* setup cycle tables */
            m_cc_op = cc_op;
            m_cc_cb = cc_cb;
            m_cc_ed = cc_ed;
            m_cc_xy = cc_xy;
            m_cc_xycb = cc_xycb;
            m_cc_ex = cc_ex;
            m_cc_dd = cc_xy;
            m_cc_fd = cc_xy;

            m_irqack_cb.resolve_safe();
            m_refresh_cb.resolve_safe();
            m_halt_cb.resolve_safe();
        }


        protected override void device_stop()
        {
        }


        /****************************************************************************
         * Do a reset
         ****************************************************************************/
        protected override void device_reset()
        {
            leave_halt();

            PC = 0x0000;
            m_i = 0;
            m_r = 0;
            m_r2 = 0;
            m_nmi_pending = false;
            m_after_ei = false;
            m_after_ldair = false;
            m_iff1 = 0;
            m_iff2 = 0;

            WZ = (UInt16)PCD;
        }


        // device_execute_interface overrides
        //virtual UINT32 execute_min_cycles() const { return 2; }
        //virtual UINT32 execute_max_cycles() const { return 16; }
        //virtual UINT32 execute_input_lines() const { return 4; }
        //virtual uint32_t execute_default_irq_vector(int inputnum) const override { return 0xff; }
        bool device_execute_interface_execute_input_edge_triggered(int inputnum) { return inputnum == device_execute_interface.INPUT_LINE_NMI; }

        static int opcount = 0;  // for debugging purposes

        void device_execute_interface_execute_run()
        {
            do
            {
                if (m_wait_state != 0)
                {
                    // stalled
                    m_icount.i = 0;
                    return;
                }

                // check for interrupts before each instruction
                if (m_nmi_pending)
                    take_nmi();
                else if (m_irq_state != CLEAR_LINE && m_iff1 != 0 && !m_after_ei)
                    take_interrupt();

                m_after_ei = false;
                m_after_ldair = false;

                PRVPC = PCD;
                debugger_instruction_hook(PCD);
                m_r++;


                uint8_t r = rop();


                if (opcount % 200000 == 0)
                    osd_printf_debug("z80.execute_run() - {0} {1}: op_{2:x2}() - A: {3,3} B: {4,3} C: {5,3} F: {6,3} HL: {7,3}\n", tag(), opcount, r, A, B, C, F, HL);

                //if (opcount >= 0 && opcount < 500)
                //    global.osd_printf_debug("z80.execute_run() - {0} {1}: op_{2:x2}() - A: {3,3} B: {4,3} C: {5,3} F: {6,3} HL: {7,3}\n", tag(), opcount, r, A, B, C, F, HL);


                opcount++;


                EXEC_op(r);

            } while (m_icount.i > 0);
        }

        void device_execute_interface_execute_set_input(int inputnum, int state)
        {
            switch (inputnum)
            {
                case Z80_INPUT_LINE_BUSRQ:
                    m_busrq_state = state;
                    break;

                case device_execute_interface.INPUT_LINE_NMI:
                    /* mark an NMI pending on the rising edge */
                    if (m_nmi_state == CLEAR_LINE && state != CLEAR_LINE)
                        m_nmi_pending = true;
                    m_nmi_state = (byte)state;
                    break;

                case device_execute_interface.INPUT_LINE_IRQ0:
                    /* update the IRQ state via the daisy chain */
                    m_irq_state = (byte)state;
                    if (m_daisy.daisy_chain_present())
                        m_irq_state = (m_daisy.daisy_update_irq_state() == ASSERT_LINE) ? (byte)ASSERT_LINE : m_irq_state;

                    /* the main execute loop will take the interrupt */
                    break;

                case Z80_INPUT_LINE_WAIT:
                    m_wait_state = state;
                    break;

                default:
                    break;
            }
        }


        // device_memory_interface overrides
        space_config_vector device_memory_interface_memory_space_config()
        {
            if (memory().has_configured_map(AS_OPCODES))
            {
                return new space_config_vector()
                {
                    std.make_pair(AS_PROGRAM, m_program_config),
                    std.make_pair(AS_OPCODES, m_opcodes_config),
                    std.make_pair(AS_IO,      m_io_config)
                };
            }
            else
            {
                return new space_config_vector()
                {
                    std.make_pair(AS_PROGRAM, m_program_config),
                    std.make_pair(AS_IO,      m_io_config)
                };
            }
        }


        // device_state_interface overrides
        void device_state_interface_state_import(device_state_entry entry)
        {
            switch (entry.index())
            {
                case Z80_R:
                    m_r = (byte)(m_rtemp & 0x7f);
                    m_r2 = (byte)(m_rtemp & 0x80);
                    break;

                default:
                    throw new emu_fatalerror("CPU_IMPORT_STATE() called for unexpected value\n");
            }
        }

        void device_state_interface_state_export(device_state_entry entry)
        {
            switch (entry.index())
            {
                case Z80_R:
                    m_rtemp = (byte)((m_r & 0x7f) | (m_r2 & 0x80));
                    break;

                default:
                    throw new emu_fatalerror("CPU_EXPORT_STATE() called for unexpected value\n");
            }
        }

        void device_state_interface_state_string_export(device_state_entry entry, out string str)
        {
            str = "";

            switch (entry.index())
            {
                case STATE_GENFLAGS:
                    str = string.Format("{0}{1}{2}{3}{4}{5}{6}{7}",  // %c%c%c%c%c%c%c%c
                        (F & 0x80) > 0 ? 'S':'.',
                        (F & 0x40) > 0 ? 'Z':'.',
                        (F & 0x20) > 0 ? 'Y':'.',
                        (F & 0x10) > 0 ? 'H':'.',
                        (F & 0x08) > 0 ? 'X':'.',
                        (F & 0x04) > 0 ? 'P':'.',
                        (F & 0x02) > 0 ? 'N':'.',
                        (F & 0x01) > 0 ? 'C':'.');
                    break;
            }
        }


        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        /***************************************************************
         * adjust cycle count by n T-states
         ***************************************************************/

        //#define CC(prefix,opcode) do { m_icount -= m_cc_##prefix[opcode]; } while (0)
        void CC_ex(int opcode)
        {
            m_icount.i -= m_cc_ex[opcode];
        }


        //define EXEC(prefix,opcode) do { \
        public void EXEC_op(byte opcode)
        {
            //byte op = opcode; //unsigned op = opcode; \

            m_icount.i -= m_cc_op[opcode];  //CC(prefix,op);

            switch (opcode)
            {
                case 0x00:op_00();break; case 0x01:op_01();break; case 0x02:op_02();break; case 0x03:op_03();break;
                case 0x04:op_04();break; case 0x05:op_05();break; case 0x06:op_06();break; case 0x07:op_07();break;
                case 0x08:op_08();break; case 0x09:op_09();break; case 0x0a:op_0a();break; case 0x0b:op_0b();break;
                case 0x0c:op_0c();break; case 0x0d:op_0d();break; case 0x0e:op_0e();break; case 0x0f:op_0f();break;
                case 0x10:op_10();break; case 0x11:op_11();break; case 0x12:op_12();break; case 0x13:op_13();break;
                case 0x14:op_14();break; case 0x15:op_15();break; case 0x16:op_16();break; case 0x17:op_17();break;
                case 0x18:op_18();break; case 0x19:op_19();break; case 0x1a:op_1a();break; case 0x1b:op_1b();break;
                case 0x1c:op_1c();break; case 0x1d:op_1d();break; case 0x1e:op_1e();break; case 0x1f:op_1f();break;
                case 0x20:op_20();break; case 0x21:op_21();break; case 0x22:op_22();break; case 0x23:op_23();break;
                case 0x24:op_24();break; case 0x25:op_25();break; case 0x26:op_26();break; case 0x27:op_27();break;
                case 0x28:op_28();break; case 0x29:op_29();break; case 0x2a:op_2a();break; case 0x2b:op_2b();break;
                case 0x2c:op_2c();break; case 0x2d:op_2d();break; case 0x2e:op_2e();break; case 0x2f:op_2f();break;
                case 0x30:op_30();break; case 0x31:op_31();break; case 0x32:op_32();break; case 0x33:op_33();break;
                case 0x34:op_34();break; case 0x35:op_35();break; case 0x36:op_36();break; case 0x37:op_37();break;
                case 0x38:op_38();break; case 0x39:op_39();break; case 0x3a:op_3a();break; case 0x3b:op_3b();break;
                case 0x3c:op_3c();break; case 0x3d:op_3d();break; case 0x3e:op_3e();break; case 0x3f:op_3f();break;
                case 0x40:op_40();break; case 0x41:op_41();break; case 0x42:op_42();break; case 0x43:op_43();break;
                case 0x44:op_44();break; case 0x45:op_45();break; case 0x46:op_46();break; case 0x47:op_47();break;
                case 0x48:op_48();break; case 0x49:op_49();break; case 0x4a:op_4a();break; case 0x4b:op_4b();break;
                case 0x4c:op_4c();break; case 0x4d:op_4d();break; case 0x4e:op_4e();break; case 0x4f:op_4f();break;
                case 0x50:op_50();break; case 0x51:op_51();break; case 0x52:op_52();break; case 0x53:op_53();break;
                case 0x54:op_54();break; case 0x55:op_55();break; case 0x56:op_56();break; case 0x57:op_57();break;
                case 0x58:op_58();break; case 0x59:op_59();break; case 0x5a:op_5a();break; case 0x5b:op_5b();break;
                case 0x5c:op_5c();break; case 0x5d:op_5d();break; case 0x5e:op_5e();break; case 0x5f:op_5f();break;
                case 0x60:op_60();break; case 0x61:op_61();break; case 0x62:op_62();break; case 0x63:op_63();break;
                case 0x64:op_64();break; case 0x65:op_65();break; case 0x66:op_66();break; case 0x67:op_67();break;
                case 0x68:op_68();break; case 0x69:op_69();break; case 0x6a:op_6a();break; case 0x6b:op_6b();break;
                case 0x6c:op_6c();break; case 0x6d:op_6d();break; case 0x6e:op_6e();break; case 0x6f:op_6f();break;
                case 0x70:op_70();break; case 0x71:op_71();break; case 0x72:op_72();break; case 0x73:op_73();break;
                case 0x74:op_74();break; case 0x75:op_75();break; case 0x76:op_76();break; case 0x77:op_77();break;
                case 0x78:op_78();break; case 0x79:op_79();break; case 0x7a:op_7a();break; case 0x7b:op_7b();break;
                case 0x7c:op_7c();break; case 0x7d:op_7d();break; case 0x7e:op_7e();break; case 0x7f:op_7f();break;
                case 0x80:op_80();break; case 0x81:op_81();break; case 0x82:op_82();break; case 0x83:op_83();break;
                case 0x84:op_84();break; case 0x85:op_85();break; case 0x86:op_86();break; case 0x87:op_87();break;
                case 0x88:op_88();break; case 0x89:op_89();break; case 0x8a:op_8a();break; case 0x8b:op_8b();break;
                case 0x8c:op_8c();break; case 0x8d:op_8d();break; case 0x8e:op_8e();break; case 0x8f:op_8f();break;
                case 0x90:op_90();break; case 0x91:op_91();break; case 0x92:op_92();break; case 0x93:op_93();break;
                case 0x94:op_94();break; case 0x95:op_95();break; case 0x96:op_96();break; case 0x97:op_97();break;
                case 0x98:op_98();break; case 0x99:op_99();break; case 0x9a:op_9a();break; case 0x9b:op_9b();break;
                case 0x9c:op_9c();break; case 0x9d:op_9d();break; case 0x9e:op_9e();break; case 0x9f:op_9f();break;
                case 0xa0:op_a0();break; case 0xa1:op_a1();break; case 0xa2:op_a2();break; case 0xa3:op_a3();break;
                case 0xa4:op_a4();break; case 0xa5:op_a5();break; case 0xa6:op_a6();break; case 0xa7:op_a7();break;
                case 0xa8:op_a8();break; case 0xa9:op_a9();break; case 0xaa:op_aa();break; case 0xab:op_ab();break;
                case 0xac:op_ac();break; case 0xad:op_ad();break; case 0xae:op_ae();break; case 0xaf:op_af();break;
                case 0xb0:op_b0();break; case 0xb1:op_b1();break; case 0xb2:op_b2();break; case 0xb3:op_b3();break;
                case 0xb4:op_b4();break; case 0xb5:op_b5();break; case 0xb6:op_b6();break; case 0xb7:op_b7();break;
                case 0xb8:op_b8();break; case 0xb9:op_b9();break; case 0xba:op_ba();break; case 0xbb:op_bb();break;
                case 0xbc:op_bc();break; case 0xbd:op_bd();break; case 0xbe:op_be();break; case 0xbf:op_bf();break;
                case 0xc0:op_c0();break; case 0xc1:op_c1();break; case 0xc2:op_c2();break; case 0xc3:op_c3();break;
                case 0xc4:op_c4();break; case 0xc5:op_c5();break; case 0xc6:op_c6();break; case 0xc7:op_c7();break;
                case 0xc8:op_c8();break; case 0xc9:op_c9();break; case 0xca:op_ca();break; case 0xcb:op_cb();break;
                case 0xcc:op_cc();break; case 0xcd:op_cd();break; case 0xce:op_ce();break; case 0xcf:op_cf();break;
                case 0xd0:op_d0();break; case 0xd1:op_d1();break; case 0xd2:op_d2();break; case 0xd3:op_d3();break;
                case 0xd4:op_d4();break; case 0xd5:op_d5();break; case 0xd6:op_d6();break; case 0xd7:op_d7();break;
                case 0xd8:op_d8();break; case 0xd9:op_d9();break; case 0xda:op_da();break; case 0xdb:op_db();break;
                case 0xdc:op_dc();break; case 0xdd:op_dd();break; case 0xde:op_de();break; case 0xdf:op_df();break;
                case 0xe0:op_e0();break; case 0xe1:op_e1();break; case 0xe2:op_e2();break; case 0xe3:op_e3();break;
                case 0xe4:op_e4();break; case 0xe5:op_e5();break; case 0xe6:op_e6();break; case 0xe7:op_e7();break;
                case 0xe8:op_e8();break; case 0xe9:op_e9();break; case 0xea:op_ea();break; case 0xeb:op_eb();break;
                case 0xec:op_ec();break; case 0xed:op_ed();break; case 0xee:op_ee();break; case 0xef:op_ef();break;
                case 0xf0:op_f0();break; case 0xf1:op_f1();break; case 0xf2:op_f2();break; case 0xf3:op_f3();break;
                case 0xf4:op_f4();break; case 0xf5:op_f5();break; case 0xf6:op_f6();break; case 0xf7:op_f7();break;
                case 0xf8:op_f8();break; case 0xf9:op_f9();break; case 0xfa:op_fa();break; case 0xfb:op_fb();break;
                case 0xfc:op_fc();break; case 0xfd:op_fd();break; case 0xfe:op_fe();break; case 0xff:op_ff();break;
            }
        }

        public void EXEC_cb(byte opcode)
        {
            //byte op = opcode; //unsigned op = opcode; \

            m_icount.i -= m_cc_cb[opcode];  //CC(prefix,op);

            switch (opcode)
            {
                case 0x00:cb_00();break; case 0x01:cb_01();break; case 0x02:cb_02();break; case 0x03:cb_03();break;
                case 0x04:cb_04();break; case 0x05:cb_05();break; case 0x06:cb_06();break; case 0x07:cb_07();break;
                case 0x08:cb_08();break; case 0x09:cb_09();break; case 0x0a:cb_0a();break; case 0x0b:cb_0b();break;
                case 0x0c:cb_0c();break; case 0x0d:cb_0d();break; case 0x0e:cb_0e();break; case 0x0f:cb_0f();break;
                case 0x10:cb_10();break; case 0x11:cb_11();break; case 0x12:cb_12();break; case 0x13:cb_13();break;
                case 0x14:cb_14();break; case 0x15:cb_15();break; case 0x16:cb_16();break; case 0x17:cb_17();break;
                case 0x18:cb_18();break; case 0x19:cb_19();break; case 0x1a:cb_1a();break; case 0x1b:cb_1b();break;
                case 0x1c:cb_1c();break; case 0x1d:cb_1d();break; case 0x1e:cb_1e();break; case 0x1f:cb_1f();break;
                case 0x20:cb_20();break; case 0x21:cb_21();break; case 0x22:cb_22();break; case 0x23:cb_23();break;
                case 0x24:cb_24();break; case 0x25:cb_25();break; case 0x26:cb_26();break; case 0x27:cb_27();break;
                case 0x28:cb_28();break; case 0x29:cb_29();break; case 0x2a:cb_2a();break; case 0x2b:cb_2b();break;
                case 0x2c:cb_2c();break; case 0x2d:cb_2d();break; case 0x2e:cb_2e();break; case 0x2f:cb_2f();break;
                case 0x30:cb_30();break; case 0x31:cb_31();break; case 0x32:cb_32();break; case 0x33:cb_33();break;
                case 0x34:cb_34();break; case 0x35:cb_35();break; case 0x36:cb_36();break; case 0x37:cb_37();break;
                case 0x38:cb_38();break; case 0x39:cb_39();break; case 0x3a:cb_3a();break; case 0x3b:cb_3b();break;
                case 0x3c:cb_3c();break; case 0x3d:cb_3d();break; case 0x3e:cb_3e();break; case 0x3f:cb_3f();break;
                case 0x40:cb_40();break; case 0x41:cb_41();break; case 0x42:cb_42();break; case 0x43:cb_43();break;
                case 0x44:cb_44();break; case 0x45:cb_45();break; case 0x46:cb_46();break; case 0x47:cb_47();break;
                case 0x48:cb_48();break; case 0x49:cb_49();break; case 0x4a:cb_4a();break; case 0x4b:cb_4b();break;
                case 0x4c:cb_4c();break; case 0x4d:cb_4d();break; case 0x4e:cb_4e();break; case 0x4f:cb_4f();break;
                case 0x50:cb_50();break; case 0x51:cb_51();break; case 0x52:cb_52();break; case 0x53:cb_53();break;
                case 0x54:cb_54();break; case 0x55:cb_55();break; case 0x56:cb_56();break; case 0x57:cb_57();break;
                case 0x58:cb_58();break; case 0x59:cb_59();break; case 0x5a:cb_5a();break; case 0x5b:cb_5b();break;
                case 0x5c:cb_5c();break; case 0x5d:cb_5d();break; case 0x5e:cb_5e();break; case 0x5f:cb_5f();break;
                case 0x60:cb_60();break; case 0x61:cb_61();break; case 0x62:cb_62();break; case 0x63:cb_63();break;
                case 0x64:cb_64();break; case 0x65:cb_65();break; case 0x66:cb_66();break; case 0x67:cb_67();break;
                case 0x68:cb_68();break; case 0x69:cb_69();break; case 0x6a:cb_6a();break; case 0x6b:cb_6b();break;
                case 0x6c:cb_6c();break; case 0x6d:cb_6d();break; case 0x6e:cb_6e();break; case 0x6f:cb_6f();break;
                case 0x70:cb_70();break; case 0x71:cb_71();break; case 0x72:cb_72();break; case 0x73:cb_73();break;
                case 0x74:cb_74();break; case 0x75:cb_75();break; case 0x76:cb_76();break; case 0x77:cb_77();break;
                case 0x78:cb_78();break; case 0x79:cb_79();break; case 0x7a:cb_7a();break; case 0x7b:cb_7b();break;
                case 0x7c:cb_7c();break; case 0x7d:cb_7d();break; case 0x7e:cb_7e();break; case 0x7f:cb_7f();break;
                case 0x80:cb_80();break; case 0x81:cb_81();break; case 0x82:cb_82();break; case 0x83:cb_83();break;
                case 0x84:cb_84();break; case 0x85:cb_85();break; case 0x86:cb_86();break; case 0x87:cb_87();break;
                case 0x88:cb_88();break; case 0x89:cb_89();break; case 0x8a:cb_8a();break; case 0x8b:cb_8b();break;
                case 0x8c:cb_8c();break; case 0x8d:cb_8d();break; case 0x8e:cb_8e();break; case 0x8f:cb_8f();break;
                case 0x90:cb_90();break; case 0x91:cb_91();break; case 0x92:cb_92();break; case 0x93:cb_93();break;
                case 0x94:cb_94();break; case 0x95:cb_95();break; case 0x96:cb_96();break; case 0x97:cb_97();break;
                case 0x98:cb_98();break; case 0x99:cb_99();break; case 0x9a:cb_9a();break; case 0x9b:cb_9b();break;
                case 0x9c:cb_9c();break; case 0x9d:cb_9d();break; case 0x9e:cb_9e();break; case 0x9f:cb_9f();break;
                case 0xa0:cb_a0();break; case 0xa1:cb_a1();break; case 0xa2:cb_a2();break; case 0xa3:cb_a3();break;
                case 0xa4:cb_a4();break; case 0xa5:cb_a5();break; case 0xa6:cb_a6();break; case 0xa7:cb_a7();break;
                case 0xa8:cb_a8();break; case 0xa9:cb_a9();break; case 0xaa:cb_aa();break; case 0xab:cb_ab();break;
                case 0xac:cb_ac();break; case 0xad:cb_ad();break; case 0xae:cb_ae();break; case 0xaf:cb_af();break;
                case 0xb0:cb_b0();break; case 0xb1:cb_b1();break; case 0xb2:cb_b2();break; case 0xb3:cb_b3();break;
                case 0xb4:cb_b4();break; case 0xb5:cb_b5();break; case 0xb6:cb_b6();break; case 0xb7:cb_b7();break;
                case 0xb8:cb_b8();break; case 0xb9:cb_b9();break; case 0xba:cb_ba();break; case 0xbb:cb_bb();break;
                case 0xbc:cb_bc();break; case 0xbd:cb_bd();break; case 0xbe:cb_be();break; case 0xbf:cb_bf();break;
                case 0xc0:cb_c0();break; case 0xc1:cb_c1();break; case 0xc2:cb_c2();break; case 0xc3:cb_c3();break;
                case 0xc4:cb_c4();break; case 0xc5:cb_c5();break; case 0xc6:cb_c6();break; case 0xc7:cb_c7();break;
                case 0xc8:cb_c8();break; case 0xc9:cb_c9();break; case 0xca:cb_ca();break; case 0xcb:cb_cb();break;
                case 0xcc:cb_cc();break; case 0xcd:cb_cd();break; case 0xce:cb_ce();break; case 0xcf:cb_cf();break;
                case 0xd0:cb_d0();break; case 0xd1:cb_d1();break; case 0xd2:cb_d2();break; case 0xd3:cb_d3();break;
                case 0xd4:cb_d4();break; case 0xd5:cb_d5();break; case 0xd6:cb_d6();break; case 0xd7:cb_d7();break;
                case 0xd8:cb_d8();break; case 0xd9:cb_d9();break; case 0xda:cb_da();break; case 0xdb:cb_db();break;
                case 0xdc:cb_dc();break; case 0xdd:cb_dd();break; case 0xde:cb_de();break; case 0xdf:cb_df();break;
                case 0xe0:cb_e0();break; case 0xe1:cb_e1();break; case 0xe2:cb_e2();break; case 0xe3:cb_e3();break;
                case 0xe4:cb_e4();break; case 0xe5:cb_e5();break; case 0xe6:cb_e6();break; case 0xe7:cb_e7();break;
                case 0xe8:cb_e8();break; case 0xe9:cb_e9();break; case 0xea:cb_ea();break; case 0xeb:cb_eb();break;
                case 0xec:cb_ec();break; case 0xed:cb_ed();break; case 0xee:cb_ee();break; case 0xef:cb_ef();break;
                case 0xf0:cb_f0();break; case 0xf1:cb_f1();break; case 0xf2:cb_f2();break; case 0xf3:cb_f3();break;
                case 0xf4:cb_f4();break; case 0xf5:cb_f5();break; case 0xf6:cb_f6();break; case 0xf7:cb_f7();break;
                case 0xf8:cb_f8();break; case 0xf9:cb_f9();break; case 0xfa:cb_fa();break; case 0xfb:cb_fb();break;
                case 0xfc:cb_fc();break; case 0xfd:cb_fd();break; case 0xfe:cb_fe();break; case 0xff:cb_ff();break;
            }
        }

        public void EXEC_dd(byte opcode)
        {
            //byte op = opcode; //unsigned op = opcode; \

            m_icount.i -= m_cc_dd[opcode];  //CC(prefix,op);

            switch (opcode)
            {
                case 0x00:dd_00();break; case 0x01:dd_01();break; case 0x02:dd_02();break; case 0x03:dd_03();break;
                case 0x04:dd_04();break; case 0x05:dd_05();break; case 0x06:dd_06();break; case 0x07:dd_07();break;
                case 0x08:dd_08();break; case 0x09:dd_09();break; case 0x0a:dd_0a();break; case 0x0b:dd_0b();break;
                case 0x0c:dd_0c();break; case 0x0d:dd_0d();break; case 0x0e:dd_0e();break; case 0x0f:dd_0f();break;
                case 0x10:dd_10();break; case 0x11:dd_11();break; case 0x12:dd_12();break; case 0x13:dd_13();break;
                case 0x14:dd_14();break; case 0x15:dd_15();break; case 0x16:dd_16();break; case 0x17:dd_17();break;
                case 0x18:dd_18();break; case 0x19:dd_19();break; case 0x1a:dd_1a();break; case 0x1b:dd_1b();break;
                case 0x1c:dd_1c();break; case 0x1d:dd_1d();break; case 0x1e:dd_1e();break; case 0x1f:dd_1f();break;
                case 0x20:dd_20();break; case 0x21:dd_21();break; case 0x22:dd_22();break; case 0x23:dd_23();break;
                case 0x24:dd_24();break; case 0x25:dd_25();break; case 0x26:dd_26();break; case 0x27:dd_27();break;
                case 0x28:dd_28();break; case 0x29:dd_29();break; case 0x2a:dd_2a();break; case 0x2b:dd_2b();break;
                case 0x2c:dd_2c();break; case 0x2d:dd_2d();break; case 0x2e:dd_2e();break; case 0x2f:dd_2f();break;
                case 0x30:dd_30();break; case 0x31:dd_31();break; case 0x32:dd_32();break; case 0x33:dd_33();break;
                case 0x34:dd_34();break; case 0x35:dd_35();break; case 0x36:dd_36();break; case 0x37:dd_37();break;
                case 0x38:dd_38();break; case 0x39:dd_39();break; case 0x3a:dd_3a();break; case 0x3b:dd_3b();break;
                case 0x3c:dd_3c();break; case 0x3d:dd_3d();break; case 0x3e:dd_3e();break; case 0x3f:dd_3f();break;
                case 0x40:dd_40();break; case 0x41:dd_41();break; case 0x42:dd_42();break; case 0x43:dd_43();break;
                case 0x44:dd_44();break; case 0x45:dd_45();break; case 0x46:dd_46();break; case 0x47:dd_47();break;
                case 0x48:dd_48();break; case 0x49:dd_49();break; case 0x4a:dd_4a();break; case 0x4b:dd_4b();break;
                case 0x4c:dd_4c();break; case 0x4d:dd_4d();break; case 0x4e:dd_4e();break; case 0x4f:dd_4f();break;
                case 0x50:dd_50();break; case 0x51:dd_51();break; case 0x52:dd_52();break; case 0x53:dd_53();break;
                case 0x54:dd_54();break; case 0x55:dd_55();break; case 0x56:dd_56();break; case 0x57:dd_57();break;
                case 0x58:dd_58();break; case 0x59:dd_59();break; case 0x5a:dd_5a();break; case 0x5b:dd_5b();break;
                case 0x5c:dd_5c();break; case 0x5d:dd_5d();break; case 0x5e:dd_5e();break; case 0x5f:dd_5f();break;
                case 0x60:dd_60();break; case 0x61:dd_61();break; case 0x62:dd_62();break; case 0x63:dd_63();break;
                case 0x64:dd_64();break; case 0x65:dd_65();break; case 0x66:dd_66();break; case 0x67:dd_67();break;
                case 0x68:dd_68();break; case 0x69:dd_69();break; case 0x6a:dd_6a();break; case 0x6b:dd_6b();break;
                case 0x6c:dd_6c();break; case 0x6d:dd_6d();break; case 0x6e:dd_6e();break; case 0x6f:dd_6f();break;
                case 0x70:dd_70();break; case 0x71:dd_71();break; case 0x72:dd_72();break; case 0x73:dd_73();break;
                case 0x74:dd_74();break; case 0x75:dd_75();break; case 0x76:dd_76();break; case 0x77:dd_77();break;
                case 0x78:dd_78();break; case 0x79:dd_79();break; case 0x7a:dd_7a();break; case 0x7b:dd_7b();break;
                case 0x7c:dd_7c();break; case 0x7d:dd_7d();break; case 0x7e:dd_7e();break; case 0x7f:dd_7f();break;
                case 0x80:dd_80();break; case 0x81:dd_81();break; case 0x82:dd_82();break; case 0x83:dd_83();break;
                case 0x84:dd_84();break; case 0x85:dd_85();break; case 0x86:dd_86();break; case 0x87:dd_87();break;
                case 0x88:dd_88();break; case 0x89:dd_89();break; case 0x8a:dd_8a();break; case 0x8b:dd_8b();break;
                case 0x8c:dd_8c();break; case 0x8d:dd_8d();break; case 0x8e:dd_8e();break; case 0x8f:dd_8f();break;
                case 0x90:dd_90();break; case 0x91:dd_91();break; case 0x92:dd_92();break; case 0x93:dd_93();break;
                case 0x94:dd_94();break; case 0x95:dd_95();break; case 0x96:dd_96();break; case 0x97:dd_97();break;
                case 0x98:dd_98();break; case 0x99:dd_99();break; case 0x9a:dd_9a();break; case 0x9b:dd_9b();break;
                case 0x9c:dd_9c();break; case 0x9d:dd_9d();break; case 0x9e:dd_9e();break; case 0x9f:dd_9f();break;
                case 0xa0:dd_a0();break; case 0xa1:dd_a1();break; case 0xa2:dd_a2();break; case 0xa3:dd_a3();break;
                case 0xa4:dd_a4();break; case 0xa5:dd_a5();break; case 0xa6:dd_a6();break; case 0xa7:dd_a7();break;
                case 0xa8:dd_a8();break; case 0xa9:dd_a9();break; case 0xaa:dd_aa();break; case 0xab:dd_ab();break;
                case 0xac:dd_ac();break; case 0xad:dd_ad();break; case 0xae:dd_ae();break; case 0xaf:dd_af();break;
                case 0xb0:dd_b0();break; case 0xb1:dd_b1();break; case 0xb2:dd_b2();break; case 0xb3:dd_b3();break;
                case 0xb4:dd_b4();break; case 0xb5:dd_b5();break; case 0xb6:dd_b6();break; case 0xb7:dd_b7();break;
                case 0xb8:dd_b8();break; case 0xb9:dd_b9();break; case 0xba:dd_ba();break; case 0xbb:dd_bb();break;
                case 0xbc:dd_bc();break; case 0xbd:dd_bd();break; case 0xbe:dd_be();break; case 0xbf:dd_bf();break;
                case 0xc0:dd_c0();break; case 0xc1:dd_c1();break; case 0xc2:dd_c2();break; case 0xc3:dd_c3();break;
                case 0xc4:dd_c4();break; case 0xc5:dd_c5();break; case 0xc6:dd_c6();break; case 0xc7:dd_c7();break;
                case 0xc8:dd_c8();break; case 0xc9:dd_c9();break; case 0xca:dd_ca();break; case 0xcb:dd_cb();break;
                case 0xcc:dd_cc();break; case 0xcd:dd_cd();break; case 0xce:dd_ce();break; case 0xcf:dd_cf();break;
                case 0xd0:dd_d0();break; case 0xd1:dd_d1();break; case 0xd2:dd_d2();break; case 0xd3:dd_d3();break;
                case 0xd4:dd_d4();break; case 0xd5:dd_d5();break; case 0xd6:dd_d6();break; case 0xd7:dd_d7();break;
                case 0xd8:dd_d8();break; case 0xd9:dd_d9();break; case 0xda:dd_da();break; case 0xdb:dd_db();break;
                case 0xdc:dd_dc();break; case 0xdd:dd_dd();break; case 0xde:dd_de();break; case 0xdf:dd_df();break;
                case 0xe0:dd_e0();break; case 0xe1:dd_e1();break; case 0xe2:dd_e2();break; case 0xe3:dd_e3();break;
                case 0xe4:dd_e4();break; case 0xe5:dd_e5();break; case 0xe6:dd_e6();break; case 0xe7:dd_e7();break;
                case 0xe8:dd_e8();break; case 0xe9:dd_e9();break; case 0xea:dd_ea();break; case 0xeb:dd_eb();break;
                case 0xec:dd_ec();break; case 0xed:dd_ed();break; case 0xee:dd_ee();break; case 0xef:dd_ef();break;
                case 0xf0:dd_f0();break; case 0xf1:dd_f1();break; case 0xf2:dd_f2();break; case 0xf3:dd_f3();break;
                case 0xf4:dd_f4();break; case 0xf5:dd_f5();break; case 0xf6:dd_f6();break; case 0xf7:dd_f7();break;
                case 0xf8:dd_f8();break; case 0xf9:dd_f9();break; case 0xfa:dd_fa();break; case 0xfb:dd_fb();break;
                case 0xfc:dd_fc();break; case 0xfd:dd_fd();break; case 0xfe:dd_fe();break; case 0xff:dd_ff();break;
            }
        }

        public void EXEC_ed(byte opcode)
        {
            //byte op = opcode; //unsigned op = opcode; \

            m_icount.i -= m_cc_ed[opcode];  //CC(prefix,op);

            switch (opcode)
            {
                case 0x00:ed_00();break; case 0x01:ed_01();break; case 0x02:ed_02();break; case 0x03:ed_03();break;
                case 0x04:ed_04();break; case 0x05:ed_05();break; case 0x06:ed_06();break; case 0x07:ed_07();break;
                case 0x08:ed_08();break; case 0x09:ed_09();break; case 0x0a:ed_0a();break; case 0x0b:ed_0b();break;
                case 0x0c:ed_0c();break; case 0x0d:ed_0d();break; case 0x0e:ed_0e();break; case 0x0f:ed_0f();break;
                case 0x10:ed_10();break; case 0x11:ed_11();break; case 0x12:ed_12();break; case 0x13:ed_13();break;
                case 0x14:ed_14();break; case 0x15:ed_15();break; case 0x16:ed_16();break; case 0x17:ed_17();break;
                case 0x18:ed_18();break; case 0x19:ed_19();break; case 0x1a:ed_1a();break; case 0x1b:ed_1b();break;
                case 0x1c:ed_1c();break; case 0x1d:ed_1d();break; case 0x1e:ed_1e();break; case 0x1f:ed_1f();break;
                case 0x20:ed_20();break; case 0x21:ed_21();break; case 0x22:ed_22();break; case 0x23:ed_23();break;
                case 0x24:ed_24();break; case 0x25:ed_25();break; case 0x26:ed_26();break; case 0x27:ed_27();break;
                case 0x28:ed_28();break; case 0x29:ed_29();break; case 0x2a:ed_2a();break; case 0x2b:ed_2b();break;
                case 0x2c:ed_2c();break; case 0x2d:ed_2d();break; case 0x2e:ed_2e();break; case 0x2f:ed_2f();break;
                case 0x30:ed_30();break; case 0x31:ed_31();break; case 0x32:ed_32();break; case 0x33:ed_33();break;
                case 0x34:ed_34();break; case 0x35:ed_35();break; case 0x36:ed_36();break; case 0x37:ed_37();break;
                case 0x38:ed_38();break; case 0x39:ed_39();break; case 0x3a:ed_3a();break; case 0x3b:ed_3b();break;
                case 0x3c:ed_3c();break; case 0x3d:ed_3d();break; case 0x3e:ed_3e();break; case 0x3f:ed_3f();break;
                case 0x40:ed_40();break; case 0x41:ed_41();break; case 0x42:ed_42();break; case 0x43:ed_43();break;
                case 0x44:ed_44();break; case 0x45:ed_45();break; case 0x46:ed_46();break; case 0x47:ed_47();break;
                case 0x48:ed_48();break; case 0x49:ed_49();break; case 0x4a:ed_4a();break; case 0x4b:ed_4b();break;
                case 0x4c:ed_4c();break; case 0x4d:ed_4d();break; case 0x4e:ed_4e();break; case 0x4f:ed_4f();break;
                case 0x50:ed_50();break; case 0x51:ed_51();break; case 0x52:ed_52();break; case 0x53:ed_53();break;
                case 0x54:ed_54();break; case 0x55:ed_55();break; case 0x56:ed_56();break; case 0x57:ed_57();break;
                case 0x58:ed_58();break; case 0x59:ed_59();break; case 0x5a:ed_5a();break; case 0x5b:ed_5b();break;
                case 0x5c:ed_5c();break; case 0x5d:ed_5d();break; case 0x5e:ed_5e();break; case 0x5f:ed_5f();break;
                case 0x60:ed_60();break; case 0x61:ed_61();break; case 0x62:ed_62();break; case 0x63:ed_63();break;
                case 0x64:ed_64();break; case 0x65:ed_65();break; case 0x66:ed_66();break; case 0x67:ed_67();break;
                case 0x68:ed_68();break; case 0x69:ed_69();break; case 0x6a:ed_6a();break; case 0x6b:ed_6b();break;
                case 0x6c:ed_6c();break; case 0x6d:ed_6d();break; case 0x6e:ed_6e();break; case 0x6f:ed_6f();break;
                case 0x70:ed_70();break; case 0x71:ed_71();break; case 0x72:ed_72();break; case 0x73:ed_73();break;
                case 0x74:ed_74();break; case 0x75:ed_75();break; case 0x76:ed_76();break; case 0x77:ed_77();break;
                case 0x78:ed_78();break; case 0x79:ed_79();break; case 0x7a:ed_7a();break; case 0x7b:ed_7b();break;
                case 0x7c:ed_7c();break; case 0x7d:ed_7d();break; case 0x7e:ed_7e();break; case 0x7f:ed_7f();break;
                case 0x80:ed_80();break; case 0x81:ed_81();break; case 0x82:ed_82();break; case 0x83:ed_83();break;
                case 0x84:ed_84();break; case 0x85:ed_85();break; case 0x86:ed_86();break; case 0x87:ed_87();break;
                case 0x88:ed_88();break; case 0x89:ed_89();break; case 0x8a:ed_8a();break; case 0x8b:ed_8b();break;
                case 0x8c:ed_8c();break; case 0x8d:ed_8d();break; case 0x8e:ed_8e();break; case 0x8f:ed_8f();break;
                case 0x90:ed_90();break; case 0x91:ed_91();break; case 0x92:ed_92();break; case 0x93:ed_93();break;
                case 0x94:ed_94();break; case 0x95:ed_95();break; case 0x96:ed_96();break; case 0x97:ed_97();break;
                case 0x98:ed_98();break; case 0x99:ed_99();break; case 0x9a:ed_9a();break; case 0x9b:ed_9b();break;
                case 0x9c:ed_9c();break; case 0x9d:ed_9d();break; case 0x9e:ed_9e();break; case 0x9f:ed_9f();break;
                case 0xa0:ed_a0();break; case 0xa1:ed_a1();break; case 0xa2:ed_a2();break; case 0xa3:ed_a3();break;
                case 0xa4:ed_a4();break; case 0xa5:ed_a5();break; case 0xa6:ed_a6();break; case 0xa7:ed_a7();break;
                case 0xa8:ed_a8();break; case 0xa9:ed_a9();break; case 0xaa:ed_aa();break; case 0xab:ed_ab();break;
                case 0xac:ed_ac();break; case 0xad:ed_ad();break; case 0xae:ed_ae();break; case 0xaf:ed_af();break;
                case 0xb0:ed_b0();break; case 0xb1:ed_b1();break; case 0xb2:ed_b2();break; case 0xb3:ed_b3();break;
                case 0xb4:ed_b4();break; case 0xb5:ed_b5();break; case 0xb6:ed_b6();break; case 0xb7:ed_b7();break;
                case 0xb8:ed_b8();break; case 0xb9:ed_b9();break; case 0xba:ed_ba();break; case 0xbb:ed_bb();break;
                case 0xbc:ed_bc();break; case 0xbd:ed_bd();break; case 0xbe:ed_be();break; case 0xbf:ed_bf();break;
                case 0xc0:ed_c0();break; case 0xc1:ed_c1();break; case 0xc2:ed_c2();break; case 0xc3:ed_c3();break;
                case 0xc4:ed_c4();break; case 0xc5:ed_c5();break; case 0xc6:ed_c6();break; case 0xc7:ed_c7();break;
                case 0xc8:ed_c8();break; case 0xc9:ed_c9();break; case 0xca:ed_ca();break; case 0xcb:ed_cb();break;
                case 0xcc:ed_cc();break; case 0xcd:ed_cd();break; case 0xce:ed_ce();break; case 0xcf:ed_cf();break;
                case 0xd0:ed_d0();break; case 0xd1:ed_d1();break; case 0xd2:ed_d2();break; case 0xd3:ed_d3();break;
                case 0xd4:ed_d4();break; case 0xd5:ed_d5();break; case 0xd6:ed_d6();break; case 0xd7:ed_d7();break;
                case 0xd8:ed_d8();break; case 0xd9:ed_d9();break; case 0xda:ed_da();break; case 0xdb:ed_db();break;
                case 0xdc:ed_dc();break; case 0xdd:ed_dd();break; case 0xde:ed_de();break; case 0xdf:ed_df();break;
                case 0xe0:ed_e0();break; case 0xe1:ed_e1();break; case 0xe2:ed_e2();break; case 0xe3:ed_e3();break;
                case 0xe4:ed_e4();break; case 0xe5:ed_e5();break; case 0xe6:ed_e6();break; case 0xe7:ed_e7();break;
                case 0xe8:ed_e8();break; case 0xe9:ed_e9();break; case 0xea:ed_ea();break; case 0xeb:ed_eb();break;
                case 0xec:ed_ec();break; case 0xed:ed_ed();break; case 0xee:ed_ee();break; case 0xef:ed_ef();break;
                case 0xf0:ed_f0();break; case 0xf1:ed_f1();break; case 0xf2:ed_f2();break; case 0xf3:ed_f3();break;
                case 0xf4:ed_f4();break; case 0xf5:ed_f5();break; case 0xf6:ed_f6();break; case 0xf7:ed_f7();break;
                case 0xf8:ed_f8();break; case 0xf9:ed_f9();break; case 0xfa:ed_fa();break; case 0xfb:ed_fb();break;
                case 0xfc:ed_fc();break; case 0xfd:ed_fd();break; case 0xfe:ed_fe();break; case 0xff:ed_ff();break;
            }
        }

        public void EXEC_fd(byte opcode)
        {
            //byte op = opcode; //unsigned op = opcode; \

            m_icount.i -= m_cc_fd[opcode];  //CC(prefix,op);

            switch (opcode)
            {
                case 0x00:fd_00();break; case 0x01:fd_01();break; case 0x02:fd_02();break; case 0x03:fd_03();break;
                case 0x04:fd_04();break; case 0x05:fd_05();break; case 0x06:fd_06();break; case 0x07:fd_07();break;
                case 0x08:fd_08();break; case 0x09:fd_09();break; case 0x0a:fd_0a();break; case 0x0b:fd_0b();break;
                case 0x0c:fd_0c();break; case 0x0d:fd_0d();break; case 0x0e:fd_0e();break; case 0x0f:fd_0f();break;
                case 0x10:fd_10();break; case 0x11:fd_11();break; case 0x12:fd_12();break; case 0x13:fd_13();break;
                case 0x14:fd_14();break; case 0x15:fd_15();break; case 0x16:fd_16();break; case 0x17:fd_17();break;
                case 0x18:fd_18();break; case 0x19:fd_19();break; case 0x1a:fd_1a();break; case 0x1b:fd_1b();break;
                case 0x1c:fd_1c();break; case 0x1d:fd_1d();break; case 0x1e:fd_1e();break; case 0x1f:fd_1f();break;
                case 0x20:fd_20();break; case 0x21:fd_21();break; case 0x22:fd_22();break; case 0x23:fd_23();break;
                case 0x24:fd_24();break; case 0x25:fd_25();break; case 0x26:fd_26();break; case 0x27:fd_27();break;
                case 0x28:fd_28();break; case 0x29:fd_29();break; case 0x2a:fd_2a();break; case 0x2b:fd_2b();break;
                case 0x2c:fd_2c();break; case 0x2d:fd_2d();break; case 0x2e:fd_2e();break; case 0x2f:fd_2f();break;
                case 0x30:fd_30();break; case 0x31:fd_31();break; case 0x32:fd_32();break; case 0x33:fd_33();break;
                case 0x34:fd_34();break; case 0x35:fd_35();break; case 0x36:fd_36();break; case 0x37:fd_37();break;
                case 0x38:fd_38();break; case 0x39:fd_39();break; case 0x3a:fd_3a();break; case 0x3b:fd_3b();break;
                case 0x3c:fd_3c();break; case 0x3d:fd_3d();break; case 0x3e:fd_3e();break; case 0x3f:fd_3f();break;
                case 0x40:fd_40();break; case 0x41:fd_41();break; case 0x42:fd_42();break; case 0x43:fd_43();break;
                case 0x44:fd_44();break; case 0x45:fd_45();break; case 0x46:fd_46();break; case 0x47:fd_47();break;
                case 0x48:fd_48();break; case 0x49:fd_49();break; case 0x4a:fd_4a();break; case 0x4b:fd_4b();break;
                case 0x4c:fd_4c();break; case 0x4d:fd_4d();break; case 0x4e:fd_4e();break; case 0x4f:fd_4f();break;
                case 0x50:fd_50();break; case 0x51:fd_51();break; case 0x52:fd_52();break; case 0x53:fd_53();break;
                case 0x54:fd_54();break; case 0x55:fd_55();break; case 0x56:fd_56();break; case 0x57:fd_57();break;
                case 0x58:fd_58();break; case 0x59:fd_59();break; case 0x5a:fd_5a();break; case 0x5b:fd_5b();break;
                case 0x5c:fd_5c();break; case 0x5d:fd_5d();break; case 0x5e:fd_5e();break; case 0x5f:fd_5f();break;
                case 0x60:fd_60();break; case 0x61:fd_61();break; case 0x62:fd_62();break; case 0x63:fd_63();break;
                case 0x64:fd_64();break; case 0x65:fd_65();break; case 0x66:fd_66();break; case 0x67:fd_67();break;
                case 0x68:fd_68();break; case 0x69:fd_69();break; case 0x6a:fd_6a();break; case 0x6b:fd_6b();break;
                case 0x6c:fd_6c();break; case 0x6d:fd_6d();break; case 0x6e:fd_6e();break; case 0x6f:fd_6f();break;
                case 0x70:fd_70();break; case 0x71:fd_71();break; case 0x72:fd_72();break; case 0x73:fd_73();break;
                case 0x74:fd_74();break; case 0x75:fd_75();break; case 0x76:fd_76();break; case 0x77:fd_77();break;
                case 0x78:fd_78();break; case 0x79:fd_79();break; case 0x7a:fd_7a();break; case 0x7b:fd_7b();break;
                case 0x7c:fd_7c();break; case 0x7d:fd_7d();break; case 0x7e:fd_7e();break; case 0x7f:fd_7f();break;
                case 0x80:fd_80();break; case 0x81:fd_81();break; case 0x82:fd_82();break; case 0x83:fd_83();break;
                case 0x84:fd_84();break; case 0x85:fd_85();break; case 0x86:fd_86();break; case 0x87:fd_87();break;
                case 0x88:fd_88();break; case 0x89:fd_89();break; case 0x8a:fd_8a();break; case 0x8b:fd_8b();break;
                case 0x8c:fd_8c();break; case 0x8d:fd_8d();break; case 0x8e:fd_8e();break; case 0x8f:fd_8f();break;
                case 0x90:fd_90();break; case 0x91:fd_91();break; case 0x92:fd_92();break; case 0x93:fd_93();break;
                case 0x94:fd_94();break; case 0x95:fd_95();break; case 0x96:fd_96();break; case 0x97:fd_97();break;
                case 0x98:fd_98();break; case 0x99:fd_99();break; case 0x9a:fd_9a();break; case 0x9b:fd_9b();break;
                case 0x9c:fd_9c();break; case 0x9d:fd_9d();break; case 0x9e:fd_9e();break; case 0x9f:fd_9f();break;
                case 0xa0:fd_a0();break; case 0xa1:fd_a1();break; case 0xa2:fd_a2();break; case 0xa3:fd_a3();break;
                case 0xa4:fd_a4();break; case 0xa5:fd_a5();break; case 0xa6:fd_a6();break; case 0xa7:fd_a7();break;
                case 0xa8:fd_a8();break; case 0xa9:fd_a9();break; case 0xaa:fd_aa();break; case 0xab:fd_ab();break;
                case 0xac:fd_ac();break; case 0xad:fd_ad();break; case 0xae:fd_ae();break; case 0xaf:fd_af();break;
                case 0xb0:fd_b0();break; case 0xb1:fd_b1();break; case 0xb2:fd_b2();break; case 0xb3:fd_b3();break;
                case 0xb4:fd_b4();break; case 0xb5:fd_b5();break; case 0xb6:fd_b6();break; case 0xb7:fd_b7();break;
                case 0xb8:fd_b8();break; case 0xb9:fd_b9();break; case 0xba:fd_ba();break; case 0xbb:fd_bb();break;
                case 0xbc:fd_bc();break; case 0xbd:fd_bd();break; case 0xbe:fd_be();break; case 0xbf:fd_bf();break;
                case 0xc0:fd_c0();break; case 0xc1:fd_c1();break; case 0xc2:fd_c2();break; case 0xc3:fd_c3();break;
                case 0xc4:fd_c4();break; case 0xc5:fd_c5();break; case 0xc6:fd_c6();break; case 0xc7:fd_c7();break;
                case 0xc8:fd_c8();break; case 0xc9:fd_c9();break; case 0xca:fd_ca();break; case 0xcb:fd_cb();break;
                case 0xcc:fd_cc();break; case 0xcd:fd_cd();break; case 0xce:fd_ce();break; case 0xcf:fd_cf();break;
                case 0xd0:fd_d0();break; case 0xd1:fd_d1();break; case 0xd2:fd_d2();break; case 0xd3:fd_d3();break;
                case 0xd4:fd_d4();break; case 0xd5:fd_d5();break; case 0xd6:fd_d6();break; case 0xd7:fd_d7();break;
                case 0xd8:fd_d8();break; case 0xd9:fd_d9();break; case 0xda:fd_da();break; case 0xdb:fd_db();break;
                case 0xdc:fd_dc();break; case 0xdd:fd_dd();break; case 0xde:fd_de();break; case 0xdf:fd_df();break;
                case 0xe0:fd_e0();break; case 0xe1:fd_e1();break; case 0xe2:fd_e2();break; case 0xe3:fd_e3();break;
                case 0xe4:fd_e4();break; case 0xe5:fd_e5();break; case 0xe6:fd_e6();break; case 0xe7:fd_e7();break;
                case 0xe8:fd_e8();break; case 0xe9:fd_e9();break; case 0xea:fd_ea();break; case 0xeb:fd_eb();break;
                case 0xec:fd_ec();break; case 0xed:fd_ed();break; case 0xee:fd_ee();break; case 0xef:fd_ef();break;
                case 0xf0:fd_f0();break; case 0xf1:fd_f1();break; case 0xf2:fd_f2();break; case 0xf3:fd_f3();break;
                case 0xf4:fd_f4();break; case 0xf5:fd_f5();break; case 0xf6:fd_f6();break; case 0xf7:fd_f7();break;
                case 0xf8:fd_f8();break; case 0xf9:fd_f9();break; case 0xfa:fd_fa();break; case 0xfb:fd_fb();break;
                case 0xfc:fd_fc();break; case 0xfd:fd_fd();break; case 0xfe:fd_fe();break; case 0xff:fd_ff();break;
            }
        }


        public void EXEC_xycb(byte opcode)
        {
            //byte op = opcode; //unsigned op = opcode; \

            m_icount.i -= m_cc_xycb[opcode];  //CC(prefix,op);

            switch (opcode)
            {
                case 0x00:xycb_00();break; case 0x01:xycb_01();break; case 0x02:xycb_02();break; case 0x03:xycb_03();break;
                case 0x04:xycb_04();break; case 0x05:xycb_05();break; case 0x06:xycb_06();break; case 0x07:xycb_07();break;
                case 0x08:xycb_08();break; case 0x09:xycb_09();break; case 0x0a:xycb_0a();break; case 0x0b:xycb_0b();break;
                case 0x0c:xycb_0c();break; case 0x0d:xycb_0d();break; case 0x0e:xycb_0e();break; case 0x0f:xycb_0f();break;
                case 0x10:xycb_10();break; case 0x11:xycb_11();break; case 0x12:xycb_12();break; case 0x13:xycb_13();break;
                case 0x14:xycb_14();break; case 0x15:xycb_15();break; case 0x16:xycb_16();break; case 0x17:xycb_17();break;
                case 0x18:xycb_18();break; case 0x19:xycb_19();break; case 0x1a:xycb_1a();break; case 0x1b:xycb_1b();break;
                case 0x1c:xycb_1c();break; case 0x1d:xycb_1d();break; case 0x1e:xycb_1e();break; case 0x1f:xycb_1f();break;
                case 0x20:xycb_20();break; case 0x21:xycb_21();break; case 0x22:xycb_22();break; case 0x23:xycb_23();break;
                case 0x24:xycb_24();break; case 0x25:xycb_25();break; case 0x26:xycb_26();break; case 0x27:xycb_27();break;
                case 0x28:xycb_28();break; case 0x29:xycb_29();break; case 0x2a:xycb_2a();break; case 0x2b:xycb_2b();break;
                case 0x2c:xycb_2c();break; case 0x2d:xycb_2d();break; case 0x2e:xycb_2e();break; case 0x2f:xycb_2f();break;
                case 0x30:xycb_30();break; case 0x31:xycb_31();break; case 0x32:xycb_32();break; case 0x33:xycb_33();break;
                case 0x34:xycb_34();break; case 0x35:xycb_35();break; case 0x36:xycb_36();break; case 0x37:xycb_37();break;
                case 0x38:xycb_38();break; case 0x39:xycb_39();break; case 0x3a:xycb_3a();break; case 0x3b:xycb_3b();break;
                case 0x3c:xycb_3c();break; case 0x3d:xycb_3d();break; case 0x3e:xycb_3e();break; case 0x3f:xycb_3f();break;
                case 0x40:xycb_40();break; case 0x41:xycb_41();break; case 0x42:xycb_42();break; case 0x43:xycb_43();break;
                case 0x44:xycb_44();break; case 0x45:xycb_45();break; case 0x46:xycb_46();break; case 0x47:xycb_47();break;
                case 0x48:xycb_48();break; case 0x49:xycb_49();break; case 0x4a:xycb_4a();break; case 0x4b:xycb_4b();break;
                case 0x4c:xycb_4c();break; case 0x4d:xycb_4d();break; case 0x4e:xycb_4e();break; case 0x4f:xycb_4f();break;
                case 0x50:xycb_50();break; case 0x51:xycb_51();break; case 0x52:xycb_52();break; case 0x53:xycb_53();break;
                case 0x54:xycb_54();break; case 0x55:xycb_55();break; case 0x56:xycb_56();break; case 0x57:xycb_57();break;
                case 0x58:xycb_58();break; case 0x59:xycb_59();break; case 0x5a:xycb_5a();break; case 0x5b:xycb_5b();break;
                case 0x5c:xycb_5c();break; case 0x5d:xycb_5d();break; case 0x5e:xycb_5e();break; case 0x5f:xycb_5f();break;
                case 0x60:xycb_60();break; case 0x61:xycb_61();break; case 0x62:xycb_62();break; case 0x63:xycb_63();break;
                case 0x64:xycb_64();break; case 0x65:xycb_65();break; case 0x66:xycb_66();break; case 0x67:xycb_67();break;
                case 0x68:xycb_68();break; case 0x69:xycb_69();break; case 0x6a:xycb_6a();break; case 0x6b:xycb_6b();break;
                case 0x6c:xycb_6c();break; case 0x6d:xycb_6d();break; case 0x6e:xycb_6e();break; case 0x6f:xycb_6f();break;
                case 0x70:xycb_70();break; case 0x71:xycb_71();break; case 0x72:xycb_72();break; case 0x73:xycb_73();break;
                case 0x74:xycb_74();break; case 0x75:xycb_75();break; case 0x76:xycb_76();break; case 0x77:xycb_77();break;
                case 0x78:xycb_78();break; case 0x79:xycb_79();break; case 0x7a:xycb_7a();break; case 0x7b:xycb_7b();break;
                case 0x7c:xycb_7c();break; case 0x7d:xycb_7d();break; case 0x7e:xycb_7e();break; case 0x7f:xycb_7f();break;
                case 0x80:xycb_80();break; case 0x81:xycb_81();break; case 0x82:xycb_82();break; case 0x83:xycb_83();break;
                case 0x84:xycb_84();break; case 0x85:xycb_85();break; case 0x86:xycb_86();break; case 0x87:xycb_87();break;
                case 0x88:xycb_88();break; case 0x89:xycb_89();break; case 0x8a:xycb_8a();break; case 0x8b:xycb_8b();break;
                case 0x8c:xycb_8c();break; case 0x8d:xycb_8d();break; case 0x8e:xycb_8e();break; case 0x8f:xycb_8f();break;
                case 0x90:xycb_90();break; case 0x91:xycb_91();break; case 0x92:xycb_92();break; case 0x93:xycb_93();break;
                case 0x94:xycb_94();break; case 0x95:xycb_95();break; case 0x96:xycb_96();break; case 0x97:xycb_97();break;
                case 0x98:xycb_98();break; case 0x99:xycb_99();break; case 0x9a:xycb_9a();break; case 0x9b:xycb_9b();break;
                case 0x9c:xycb_9c();break; case 0x9d:xycb_9d();break; case 0x9e:xycb_9e();break; case 0x9f:xycb_9f();break;
                case 0xa0:xycb_a0();break; case 0xa1:xycb_a1();break; case 0xa2:xycb_a2();break; case 0xa3:xycb_a3();break;
                case 0xa4:xycb_a4();break; case 0xa5:xycb_a5();break; case 0xa6:xycb_a6();break; case 0xa7:xycb_a7();break;
                case 0xa8:xycb_a8();break; case 0xa9:xycb_a9();break; case 0xaa:xycb_aa();break; case 0xab:xycb_ab();break;
                case 0xac:xycb_ac();break; case 0xad:xycb_ad();break; case 0xae:xycb_ae();break; case 0xaf:xycb_af();break;
                case 0xb0:xycb_b0();break; case 0xb1:xycb_b1();break; case 0xb2:xycb_b2();break; case 0xb3:xycb_b3();break;
                case 0xb4:xycb_b4();break; case 0xb5:xycb_b5();break; case 0xb6:xycb_b6();break; case 0xb7:xycb_b7();break;
                case 0xb8:xycb_b8();break; case 0xb9:xycb_b9();break; case 0xba:xycb_ba();break; case 0xbb:xycb_bb();break;
                case 0xbc:xycb_bc();break; case 0xbd:xycb_bd();break; case 0xbe:xycb_be();break; case 0xbf:xycb_bf();break;
                case 0xc0:xycb_c0();break; case 0xc1:xycb_c1();break; case 0xc2:xycb_c2();break; case 0xc3:xycb_c3();break;
                case 0xc4:xycb_c4();break; case 0xc5:xycb_c5();break; case 0xc6:xycb_c6();break; case 0xc7:xycb_c7();break;
                case 0xc8:xycb_c8();break; case 0xc9:xycb_c9();break; case 0xca:xycb_ca();break; case 0xcb:xycb_cb();break;
                case 0xcc:xycb_cc();break; case 0xcd:xycb_cd();break; case 0xce:xycb_ce();break; case 0xcf:xycb_cf();break;
                case 0xd0:xycb_d0();break; case 0xd1:xycb_d1();break; case 0xd2:xycb_d2();break; case 0xd3:xycb_d3();break;
                case 0xd4:xycb_d4();break; case 0xd5:xycb_d5();break; case 0xd6:xycb_d6();break; case 0xd7:xycb_d7();break;
                case 0xd8:xycb_d8();break; case 0xd9:xycb_d9();break; case 0xda:xycb_da();break; case 0xdb:xycb_db();break;
                case 0xdc:xycb_dc();break; case 0xdd:xycb_dd();break; case 0xde:xycb_de();break; case 0xdf:xycb_df();break;
                case 0xe0:xycb_e0();break; case 0xe1:xycb_e1();break; case 0xe2:xycb_e2();break; case 0xe3:xycb_e3();break;
                case 0xe4:xycb_e4();break; case 0xe5:xycb_e5();break; case 0xe6:xycb_e6();break; case 0xe7:xycb_e7();break;
                case 0xe8:xycb_e8();break; case 0xe9:xycb_e9();break; case 0xea:xycb_ea();break; case 0xeb:xycb_eb();break;
                case 0xec:xycb_ec();break; case 0xed:xycb_ed();break; case 0xee:xycb_ee();break; case 0xef:xycb_ef();break;
                case 0xf0:xycb_f0();break; case 0xf1:xycb_f1();break; case 0xf2:xycb_f2();break; case 0xf3:xycb_f3();break;
                case 0xf4:xycb_f4();break; case 0xf5:xycb_f5();break; case 0xf6:xycb_f6();break; case 0xf7:xycb_f7();break;
                case 0xf8:xycb_f8();break; case 0xf9:xycb_f9();break; case 0xfa:xycb_fa();break; case 0xfb:xycb_fb();break;
                case 0xfc:xycb_fc();break; case 0xfd:xycb_fd();break; case 0xfe:xycb_fe();break; case 0xff:xycb_ff();break;
            }
        }


        //PROTOTYPES(op);
        //#define OP(prefix,opcode) inline void z80_device::prefix##_##opcode()
        /**********************************************************
         * main opcodes
         **********************************************************/
        void op_00() {                                                                       } /* NOP              */
        void op_01() { BC = arg16();                                                         } /* LD   BC,w        */
        void op_02() { wm(BC,A); WZ_L = (byte)((BC + 1) & 0xFF);  WZ_H = A;                          } /* LD (BC),A */
        void op_03() { BC++;                                                                 } /* INC  BC          */
        void op_04() { B = inc(B);                                                           } /* INC  B           */
        void op_05() { B = dec(B);                                                           } /* DEC  B           */
        void op_06() { B = arg();                                                            } /* LD   B,n         */
        void op_07() { rlca();                                                               } /* RLCA             */

        void op_08() { ex_af();                                                              } /* EX   AF,AF'      */
        void op_09() { add16(ref m_hl, m_bc);                                                    } /* ADD  HL,BC       */
        void op_0a() { A = rm(BC);  WZ=(UInt16)(BC+1);                                                 } /* LD   A,(BC)      */
        void op_0b() { BC--;                                                                 } /* DEC  BC          */
        void op_0c() { C = inc(C);                                                           } /* INC  C           */
        void op_0d() { C = dec(C);                                                           } /* DEC  C           */
        void op_0e() { C = arg();                                                            } /* LD   C,n         */
        void op_0f() { rrca();                                                               } /* RRCA             */

        void op_10() { B--; jr_cond(B > 0, 0x10);                                                } /* DJNZ o           */
        void op_11() { DE = arg16();                                                         } /* LD   DE,w        */
        void op_12() { wm(DE,A); WZ_L = (byte)((DE + 1) & 0xFF);  WZ_H = A;                          } /* LD (DE),A */
        void op_13() { DE++;                                                                 } /* INC  DE          */
        void op_14() { D = inc(D);                                                           } /* INC  D           */
        void op_15() { D = dec(D);                                                           } /* DEC  D           */
        void op_16() { D = arg();                                                            } /* LD   D,n         */
        void op_17() { rla();                                                                } /* RLA              */

        void op_18() { jr();                                                                 } /* JR   o           */
        void op_19() { add16(ref m_hl, m_de);                                                    } /* ADD  HL,DE       */
        void op_1a() { A = rm(DE); WZ = (UInt16)(DE + 1);                                              } /* LD   A,(DE)      */
        void op_1b() { DE--;                                                                 } /* DEC  DE          */
        void op_1c() { E = inc(E);                                                           } /* INC  E           */
        void op_1d() { E = dec(E);                                                           } /* DEC  E           */
        void op_1e() { E = arg();                                                            } /* LD   E,n         */
        void op_1f() { rra();                                                                } /* RRA              */

        void op_20() { jr_cond((F & ZF) == 0, 0x20);                                             } /* JR   NZ,o        */
        void op_21() { HL = arg16();                                                         } /* LD   HL,w        */
        void op_22() { m_ea = arg16(); wm16((UInt16)m_ea, m_hl); WZ = (UInt16)(m_ea + 1);                      } /* LD   (w),HL      */
        void op_23() { HL++;                                                                 } /* INC  HL          */
        void op_24() { H = inc(H);                                                           } /* INC  H           */
        void op_25() { H = dec(H);                                                           } /* DEC  H           */
        void op_26() { H = arg();                                                            } /* LD   H,n         */
        void op_27() { daa();                                                                } /* DAA              */

        void op_28() { jr_cond((F & ZF) > 0, 0x28);                                                } /* JR   Z,o         */
        void op_29() { add16(ref m_hl, m_hl);                                                    } /* ADD  HL,HL       */
        void op_2a() { m_ea = arg16(); rm16((UInt16)m_ea, ref m_hl); WZ = (UInt16)(m_ea + 1);                        } /* LD   HL,(w)      */
        void op_2b() { HL--;                                                                 } /* DEC  HL          */
        void op_2c() { L = inc(L);                                                           } /* INC  L           */
        void op_2d() { L = dec(L);                                                           } /* DEC  L           */
        void op_2e() { L = arg();                                                            } /* LD   L,n         */
        void op_2f() { A ^= 0xff; F = (byte)((F & (SF | ZF | PF | CF)) | HF | NF | (A & (YF | XF))); } /* CPL              */

        void op_30() { jr_cond((F & CF) == 0, 0x30);                                             } /* JR   NC,o        */
        void op_31() { SP = arg16();                                                         } /* LD   SP,w        */
        void op_32() { m_ea = arg16(); wm((UInt16)m_ea, A); WZ_L = (byte)((m_ea + 1) & 0xFF); WZ_H = A;      } /* LD   (w),A       */
        void op_33() { SP++;                                                                 } /* INC  SP          */
        void op_34() { wm(HL, inc(rm(HL)));                                                  } /* INC  (HL)        */
        void op_35() { wm(HL, dec(rm(HL)));                                                  } /* DEC  (HL)        */
        void op_36() { wm(HL, arg());                                                        } /* LD   (HL),n      */
        void op_37() { F = (byte)((F & (SF | ZF | YF | XF | PF)) | CF | (A & (YF | XF)));            } /* SCF              */

        void op_38() { jr_cond((F & CF) > 0, 0x38);                                                } /* JR   C,o         */
        void op_39() { add16(ref m_hl, m_sp);                                                    } /* ADD  HL,SP       */
        void op_3a() { m_ea = arg16(); A = rm((UInt16)m_ea); WZ = (UInt16)(m_ea + 1);                          } /* LD   A,(w)       */
        void op_3b() { SP--;                                                                 } /* DEC  SP          */
        void op_3c() { A = inc(A);                                                           } /* INC  A           */
        void op_3d() { A = dec(A);                                                           } /* DEC  A           */
        void op_3e() { A = arg();                                                            } /* LD   A,n         */
        void op_3f() { F = (byte)(((F&(SF|ZF|YF|XF|PF|CF))|((F&CF)<<4)|(A&(YF|XF)))^CF);             } /* CCF        */

        void op_40() {                                                                       } /* LD   B,B         */
        void op_41() { B = C;                                                                } /* LD   B,C         */
        void op_42() { B = D;                                                                } /* LD   B,D         */
        void op_43() { B = E;                                                                } /* LD   B,E         */
        void op_44() { B = H;                                                                } /* LD   B,H         */
        void op_45() { B = L;                                                                } /* LD   B,L         */
        void op_46() { B = rm(HL);                                                           } /* LD   B,(HL)      */
        void op_47() { B = A;                                                                } /* LD   B,A         */

        void op_48() { C = B;                                                                } /* LD   C,B         */
        void op_49() {                                                                       } /* LD   C,C         */
        void op_4a() { C = D;                                                                } /* LD   C,D         */
        void op_4b() { C = E;                                                                } /* LD   C,E         */
        void op_4c() { C = H;                                                                } /* LD   C,H         */
        void op_4d() { C = L;                                                                } /* LD   C,L         */
        void op_4e() { C = rm(HL);                                                           } /* LD   C,(HL)      */
        void op_4f() { C = A;                                                                } /* LD   C,A         */

        void op_50() { D = B;                                                                } /* LD   D,B         */
        void op_51() { D = C;                                                                } /* LD   D,C         */
        void op_52() {                                                                       } /* LD   D,D         */
        void op_53() { D = E;                                                                } /* LD   D,E         */
        void op_54() { D = H;                                                                } /* LD   D,H         */
        void op_55() { D = L;                                                                } /* LD   D,L         */
        void op_56() { D = rm(HL);                                                           } /* LD   D,(HL)      */
        void op_57() { D = A;                                                                } /* LD   D,A         */

        void op_58() { E = B;                                                                } /* LD   E,B         */
        void op_59() { E = C;                                                                } /* LD   E,C         */
        void op_5a() { E = D;                                                                } /* LD   E,D         */
        void op_5b() {                                                                       } /* LD   E,E         */
        void op_5c() { E = H;                                                                } /* LD   E,H         */
        void op_5d() { E = L;                                                                } /* LD   E,L         */
        void op_5e() { E = rm(HL);                                                           } /* LD   E,(HL)      */
        void op_5f() { E = A;                                                                } /* LD   E,A         */

        void op_60() { H = B;                                                                } /* LD   H,B         */
        void op_61() { H = C;                                                                } /* LD   H,C         */
        void op_62() { H = D;                                                                } /* LD   H,D         */
        void op_63() { H = E;                                                                } /* LD   H,E         */
        void op_64() {                                                                       } /* LD   H,H         */
        void op_65() { H = L;                                                                } /* LD   H,L         */
        void op_66() { H = rm(HL);                                                           } /* LD   H,(HL)      */
        void op_67() { H = A;                                                                } /* LD   H,A         */

        void op_68() { L = B;                                                                } /* LD   L,B         */
        void op_69() { L = C;                                                                } /* LD   L,C         */
        void op_6a() { L = D;                                                                } /* LD   L,D         */
        void op_6b() { L = E;                                                                } /* LD   L,E         */
        void op_6c() { L = H;                                                                } /* LD   L,H         */
        void op_6d() {                                                                       } /* LD   L,L         */
        void op_6e() { L = rm(HL);                                                           } /* LD   L,(HL)      */
        void op_6f() { L = A;                                                                } /* LD   L,A         */

        void op_70() { wm(HL, B);                                                            } /* LD   (HL),B      */
        void op_71() { wm(HL, C);                                                            } /* LD   (HL),C      */
        void op_72() { wm(HL, D);                                                            } /* LD   (HL),D      */
        void op_73() { wm(HL, E);                                                            } /* LD   (HL),E      */
        void op_74() { wm(HL, H);                                                            } /* LD   (HL),H      */
        void op_75() { wm(HL, L);                                                            } /* LD   (HL),L      */
        void op_76() { halt();                                                               } /* halt             */
        void op_77() { wm(HL, A);                                                            } /* LD   (HL),A      */

        void op_78() { A = B;                                                                } /* LD   A,B         */
        void op_79() { A = C;                                                                } /* LD   A,C         */
        void op_7a() { A = D;                                                                } /* LD   A,D         */
        void op_7b() { A = E;                                                                } /* LD   A,E         */
        void op_7c() { A = H;                                                                } /* LD   A,H         */
        void op_7d() { A = L;                                                                } /* LD   A,L         */
        void op_7e() { A = rm(HL);                                                           } /* LD   A,(HL)      */
        void op_7f() {                                                                       } /* LD   A,A         */

        void op_80() { add_a(B);                                                             } /* ADD  A,B         */
        void op_81() { add_a(C);                                                             } /* ADD  A,C         */
        void op_82() { add_a(D);                                                             } /* ADD  A,D         */
        void op_83() { add_a(E);                                                             } /* ADD  A,E         */
        void op_84() { add_a(H);                                                             } /* ADD  A,H         */
        void op_85() { add_a(L);                                                             } /* ADD  A,L         */
        void op_86() { add_a(rm(HL));                                                        } /* ADD  A,(HL)      */
        void op_87() { add_a(A);                                                             } /* ADD  A,A         */

        void op_88() { adc_a(B);                                                             } /* ADC  A,B         */
        void op_89() { adc_a(C);                                                             } /* ADC  A,C         */
        void op_8a() { adc_a(D);                                                             } /* ADC  A,D         */
        void op_8b() { adc_a(E);                                                             } /* ADC  A,E         */
        void op_8c() { adc_a(H);                                                             } /* ADC  A,H         */
        void op_8d() { adc_a(L);                                                             } /* ADC  A,L         */
        void op_8e() { adc_a(rm(HL));                                                        } /* ADC  A,(HL)      */
        void op_8f() { adc_a(A);                                                             } /* ADC  A,A         */

        void op_90() { sub(B);                                                               } /* SUB  B           */
        void op_91() { sub(C);                                                               } /* SUB  C           */
        void op_92() { sub(D);                                                               } /* SUB  D           */
        void op_93() { sub(E);                                                               } /* SUB  E           */
        void op_94() { sub(H);                                                               } /* SUB  H           */
        void op_95() { sub(L);                                                               } /* SUB  L           */
        void op_96() { sub(rm(HL));                                                          } /* SUB  (HL)        */
        void op_97() { sub(A);                                                               } /* SUB  A           */

        void op_98() { sbc_a(B);                                                             } /* SBC  A,B         */
        void op_99() { sbc_a(C);                                                             } /* SBC  A,C         */
        void op_9a() { sbc_a(D);                                                             } /* SBC  A,D         */
        void op_9b() { sbc_a(E);                                                             } /* SBC  A,E         */
        void op_9c() { sbc_a(H);                                                             } /* SBC  A,H         */
        void op_9d() { sbc_a(L);                                                             } /* SBC  A,L         */
        void op_9e() { sbc_a(rm(HL));                                                        } /* SBC  A,(HL)      */
        void op_9f() { sbc_a(A);                                                             } /* SBC  A,A         */

        void op_a0() { and_a(B);                                                             } /* AND  B           */
        void op_a1() { and_a(C);                                                             } /* AND  C           */
        void op_a2() { and_a(D);                                                             } /* AND  D           */
        void op_a3() { and_a(E);                                                             } /* AND  E           */
        void op_a4() { and_a(H);                                                             } /* AND  H           */
        void op_a5() { and_a(L);                                                             } /* AND  L           */
        void op_a6() { and_a(rm(HL));                                                        } /* AND  (HL)        */
        void op_a7() { and_a(A);                                                             } /* AND  A           */

        void op_a8() { xor_a(B);                                                             } /* XOR  B           */
        void op_a9() { xor_a(C);                                                             } /* XOR  C           */
        void op_aa() { xor_a(D);                                                             } /* XOR  D           */
        void op_ab() { xor_a(E);                                                             } /* XOR  E           */
        void op_ac() { xor_a(H);                                                             } /* XOR  H           */
        void op_ad() { xor_a(L);                                                             } /* XOR  L           */
        void op_ae() { xor_a(rm(HL));                                                        } /* XOR  (HL)        */
        void op_af() { xor_a(A);                                                             } /* XOR  A           */

        void op_b0() { or_a(B);                                                              } /* OR   B           */
        void op_b1() { or_a(C);                                                              } /* OR   C           */
        void op_b2() { or_a(D);                                                              } /* OR   D           */
        void op_b3() { or_a(E);                                                              } /* OR   E           */
        void op_b4() { or_a(H);                                                              } /* OR   H           */
        void op_b5() { or_a(L);                                                              } /* OR   L           */
        void op_b6() { or_a(rm(HL));                                                         } /* OR   (HL)        */
        void op_b7() { or_a(A);                                                              } /* OR   A           */

        void op_b8() { cp(B);                                                                } /* CP   B           */
        void op_b9() { cp(C);                                                                } /* CP   C           */
        void op_ba() { cp(D);                                                                } /* CP   D           */
        void op_bb() { cp(E);                                                                } /* CP   E           */
        void op_bc() { cp(H);                                                                } /* CP   H           */
        void op_bd() { cp(L);                                                                } /* CP   L           */
        void op_be() { cp(rm(HL));                                                           } /* CP   (HL)        */
        void op_bf() { cp(A);                                                                } /* CP   A           */

        void op_c0() { ret_cond((F & ZF) == 0, 0xc0);                                            } /* RET  NZ          */
        void op_c1() { pop(ref m_bc);                                                            } /* POP  BC          */
        void op_c2() { jp_cond((F & ZF) == 0);                                                   } /* JP   NZ,a        */
        void op_c3() { jp();                                                                 } /* JP   a           */
        void op_c4() { call_cond((F & ZF) == 0, 0xc4);                                           } /* CALL NZ,a        */
        void op_c5() { push(m_bc);                                                           } /* PUSH BC          */
        void op_c6() { add_a(arg());                                                         } /* ADD  A,n         */
        void op_c7() { rst(0x00);                                                            } /* RST  0           */

        void op_c8() { ret_cond((F & ZF) > 0, 0xc8);                                               } /* RET  Z           */
        void op_c9() { pop(ref m_pc); WZ = (UInt16)PCD;                                                  } /* RET              */
        void op_ca() { jp_cond((F & ZF) > 0);                                                      } /* JP   Z,a         */
        void op_cb() { m_r++; EXEC_cb(rop());                                                } /* **** CB xx       */
        void op_cc() { call_cond((F & ZF) > 0, 0xcc);                                              } /* CALL Z,a         */
        void op_cd() { call();                                                               } /* CALL a           */
        void op_ce() { adc_a(arg());                                                         } /* ADC  A,n         */
        void op_cf() { rst(0x08);                                                            } /* RST  1           */

        void op_d0() { ret_cond((F & CF) == 0, 0xd0);                                            } /* RET  NC          */
        void op_d1() { pop(ref m_de);                                                            } /* POP  DE          */
        void op_d2() { jp_cond((F & CF) == 0);                                                   } /* JP   NC,a        */
        void op_d3() { UInt32 n = (UInt32)(arg() | (A << 8)); out_((UInt16)n, A); WZ_L = (byte)(((n & 0xff) + 1) & 0xff);  WZ_H = A;   } /* OUT  (n),A       */
        void op_d4() { call_cond((F & CF) == 0, 0xd4);                                           } /* CALL NC,a        */
        void op_d5() { push(m_de);                                                           } /* PUSH DE          */
        void op_d6() { sub(arg());                                                           } /* SUB  n           */
        void op_d7() { rst(0x10);                                                            } /* RST  2           */

        void op_d8() { ret_cond((F & CF) > 0, 0xd8);                                               } /* RET  C           */
        void op_d9() { exx();                                                                } /* EXX              */
        void op_da() { jp_cond((F & CF) > 0);                                                      } /* JP   C,a         */
        void op_db() { UInt32 n = (UInt32)(arg() | (A << 8)); A = in_((UInt16)n); WZ = (UInt16)(n + 1);                 } /* IN   A,(n)  */
        void op_dc() { call_cond((F & CF) > 0, 0xdc);                                              } /* CALL C,a         */
        void op_dd() { m_r++; EXEC_dd(rop());                                                } /* **** DD xx       */
        void op_de() { sbc_a(arg());                                                         } /* SBC  A,n         */
        void op_df() { rst(0x18);                                                            } /* RST  3           */

        void op_e0() { ret_cond((F & PF) == 0, 0xe0);                                            } /* RET  PO          */
        void op_e1() { pop(ref m_hl);                                                            } /* POP  HL          */
        void op_e2() { jp_cond((F & PF) == 0);                                                   } /* JP   PO,a        */
        void op_e3() { ex_sp(ref m_hl);                                                          } /* EX   HL,(SP)     */
        void op_e4() { call_cond((F & PF) == 0, 0xe4);                                           } /* CALL PO,a        */
        void op_e5() { push(m_hl);                                                           } /* PUSH HL          */
        void op_e6() { and_a(arg());                                                         } /* AND  n           */
        void op_e7() { rst(0x20);                                                            } /* RST  4           */

        void op_e8() { ret_cond((F & PF) > 0, 0xe8);                                               } /* RET  PE          */
        void op_e9() { PC = HL;                                                              } /* JP   (HL)        */
        void op_ea() { jp_cond((F & PF) > 0);                                                      } /* JP   PE,a        */
        void op_eb() { ex_de_hl();                                                           } /* EX   DE,HL       */
        void op_ec() { call_cond((F & PF) > 0, 0xec);                                              } /* CALL PE,a        */
        void op_ed() { m_r++; EXEC_ed(rop());                                                } /* **** ED xx       */
        void op_ee() { xor_a(arg());                                                         } /* XOR  n           */
        void op_ef() { rst(0x28);                                                            } /* RST  5           */

        void op_f0() { ret_cond((F & SF) == 0, 0xf0);                                            } /* RET  P           */
        void op_f1() { pop(ref m_af);                                                            } /* POP  AF          */
        void op_f2() { jp_cond((F & SF) == 0);                                                   } /* JP   P,a         */
        void op_f3() { m_iff1 = m_iff2 = 0;                                                  } /* DI               */
        void op_f4() { call_cond((F & SF) == 0, 0xf4);                                           } /* CALL P,a         */
        void op_f5() { push(m_af);                                                           } /* PUSH AF          */
        void op_f6() { or_a(arg());                                                          } /* OR   n           */
        void op_f7() { rst(0x30);                                                            } /* RST  6           */

        void op_f8() { ret_cond((F & SF) > 0, 0xf8);                                               } /* RET  M           */
        void op_f9() { SP = HL;                                                              } /* LD   SP,HL       */
        void op_fa() { jp_cond((F & SF) > 0);                                                      } /* JP   M,a         */
        void op_fb() { ei();                                                                 } /* EI               */
        void op_fc() { call_cond((F & SF) > 0, 0xfc);                                              } /* CALL M,a         */
        void op_fd() { m_r++; EXEC_fd(rop());                                                } /* **** FD xx       */
        void op_fe() { cp(arg());                                                            } /* CP   n           */
        void op_ff() { rst(0x38);                                                            } /* RST  7           */


        //PROTOTYPES(cb);
        //#define void prefix,opcode) inline void z80_device::prefix##_##opcode()
        /**********************************************************
         * opcodes with CB prefix
         * rotate, shift and bit operations
         **********************************************************/
        void cb_00() { B = rlc(B);             } /* RLC  B           */
        void cb_01() { C = rlc(C);             } /* RLC  C           */
        void cb_02() { D = rlc(D);             } /* RLC  D           */
        void cb_03() { E = rlc(E);             } /* RLC  E           */
        void cb_04() { H = rlc(H);             } /* RLC  H           */
        void cb_05() { L = rlc(L);             } /* RLC  L           */
        void cb_06() { wm(HL, rlc(rm(HL)));    } /* RLC  (HL)        */
        void cb_07() { A = rlc(A);             } /* RLC  A           */

        void cb_08() { B = rrc(B);             } /* RRC  B           */
        void cb_09() { C = rrc(C);             } /* RRC  C           */
        void cb_0a() { D = rrc(D);             } /* RRC  D           */
        void cb_0b() { E = rrc(E);             } /* RRC  E           */
        void cb_0c() { H = rrc(H);             } /* RRC  H           */
        void cb_0d() { L = rrc(L);             } /* RRC  L           */
        void cb_0e() { wm(HL, rrc(rm(HL)));    } /* RRC  (HL)        */
        void cb_0f() { A = rrc(A);             } /* RRC  A           */

        void cb_10() { B = rl(B);              } /* RL   B           */
        void cb_11() { C = rl(C);              } /* RL   C           */
        void cb_12() { D = rl(D);              } /* RL   D           */
        void cb_13() { E = rl(E);              } /* RL   E           */
        void cb_14() { H = rl(H);              } /* RL   H           */
        void cb_15() { L = rl(L);              } /* RL   L           */
        void cb_16() { wm(HL, rl(rm(HL)));     } /* RL   (HL)        */
        void cb_17() { A = rl(A);              } /* RL   A           */

        void cb_18() { B = rr(B);              } /* RR   B           */
        void cb_19() { C = rr(C);              } /* RR   C           */
        void cb_1a() { D = rr(D);              } /* RR   D           */
        void cb_1b() { E = rr(E);              } /* RR   E           */
        void cb_1c() { H = rr(H);              } /* RR   H           */
        void cb_1d() { L = rr(L);              } /* RR   L           */
        void cb_1e() { wm(HL, rr(rm(HL)));     } /* RR   (HL)        */
        void cb_1f() { A = rr(A);              } /* RR   A           */

        void cb_20() { B = sla(B);             } /* SLA  B           */
        void cb_21() { C = sla(C);             } /* SLA  C           */
        void cb_22() { D = sla(D);             } /* SLA  D           */
        void cb_23() { E = sla(E);             } /* SLA  E           */
        void cb_24() { H = sla(H);             } /* SLA  H           */
        void cb_25() { L = sla(L);             } /* SLA  L           */
        void cb_26() { wm(HL, sla(rm(HL)));    } /* SLA  (HL)        */
        void cb_27() { A = sla(A);             } /* SLA  A           */

        void cb_28() { B = sra(B);             } /* SRA  B           */
        void cb_29() { C = sra(C);             } /* SRA  C           */
        void cb_2a() { D = sra(D);             } /* SRA  D           */
        void cb_2b() { E = sra(E);             } /* SRA  E           */
        void cb_2c() { H = sra(H);             } /* SRA  H           */
        void cb_2d() { L = sra(L);             } /* SRA  L           */
        void cb_2e() { wm(HL, sra(rm(HL)));    } /* SRA  (HL)        */
        void cb_2f() { A = sra(A);             } /* SRA  A           */

        void cb_30() { B = sll(B);             } /* SLL  B           */
        void cb_31() { C = sll(C);             } /* SLL  C           */
        void cb_32() { D = sll(D);             } /* SLL  D           */
        void cb_33() { E = sll(E);             } /* SLL  E           */
        void cb_34() { H = sll(H);             } /* SLL  H           */
        void cb_35() { L = sll(L);             } /* SLL  L           */
        void cb_36() { wm(HL, sll(rm(HL)));    } /* SLL  (HL)        */
        void cb_37() { A = sll(A);             } /* SLL  A           */

        void cb_38() { B = srl(B);             } /* SRL  B           */
        void cb_39() { C = srl(C);             } /* SRL  C           */
        void cb_3a() { D = srl(D);             } /* SRL  D           */
        void cb_3b() { E = srl(E);             } /* SRL  E           */
        void cb_3c() { H = srl(H);             } /* SRL  H           */
        void cb_3d() { L = srl(L);             } /* SRL  L           */
        void cb_3e() { wm(HL, srl(rm(HL)));    } /* SRL  (HL)        */
        void cb_3f() { A = srl(A);             } /* SRL  A           */

        void cb_40() { bit(0, B);              } /* BIT  0,B         */
        void cb_41() { bit(0, C);              } /* BIT  0,C         */
        void cb_42() { bit(0, D);              } /* BIT  0,D         */
        void cb_43() { bit(0, E);              } /* BIT  0,E         */
        void cb_44() { bit(0, H);              } /* BIT  0,H         */
        void cb_45() { bit(0, L);              } /* BIT  0,L         */
        void cb_46() { bit_hl(0, rm(HL));      } /* BIT  0,(HL)      */
        void cb_47() { bit(0, A);              } /* BIT  0,A         */

        void cb_48() { bit(1, B);              } /* BIT  1,B         */
        void cb_49() { bit(1, C);              } /* BIT  1,C         */
        void cb_4a() { bit(1, D);              } /* BIT  1,D         */
        void cb_4b() { bit(1, E);              } /* BIT  1,E         */
        void cb_4c() { bit(1, H);              } /* BIT  1,H         */
        void cb_4d() { bit(1, L);              } /* BIT  1,L         */
        void cb_4e() { bit_hl(1, rm(HL));      } /* BIT  1,(HL)      */
        void cb_4f() { bit(1, A);              } /* BIT  1,A         */

        void cb_50() { bit(2, B);              } /* BIT  2,B         */
        void cb_51() { bit(2, C);              } /* BIT  2,C         */
        void cb_52() { bit(2, D);              } /* BIT  2,D         */
        void cb_53() { bit(2, E);              } /* BIT  2,E         */
        void cb_54() { bit(2, H);              } /* BIT  2,H         */
        void cb_55() { bit(2, L);              } /* BIT  2,L         */
        void cb_56() { bit_hl(2, rm(HL));      } /* BIT  2,(HL)      */
        void cb_57() { bit(2, A);              } /* BIT  2,A         */

        void cb_58() { bit(3, B);              } /* BIT  3,B         */
        void cb_59() { bit(3, C);              } /* BIT  3,C         */
        void cb_5a() { bit(3, D);              } /* BIT  3,D         */
        void cb_5b() { bit(3, E);              } /* BIT  3,E         */
        void cb_5c() { bit(3, H);              } /* BIT  3,H         */
        void cb_5d() { bit(3, L);              } /* BIT  3,L         */
        void cb_5e() { bit_hl(3, rm(HL));      } /* BIT  3,(HL)      */
        void cb_5f() { bit(3, A);              } /* BIT  3,A         */

        void cb_60() { bit(4, B);              } /* BIT  4,B         */
        void cb_61() { bit(4, C);              } /* BIT  4,C         */
        void cb_62() { bit(4, D);              } /* BIT  4,D         */
        void cb_63() { bit(4, E);              } /* BIT  4,E         */
        void cb_64() { bit(4, H);              } /* BIT  4,H         */
        void cb_65() { bit(4, L);              } /* BIT  4,L         */
        void cb_66() { bit_hl(4, rm(HL));      } /* BIT  4,(HL)      */
        void cb_67() { bit(4, A);              } /* BIT  4,A         */

        void cb_68() { bit(5, B);              } /* BIT  5,B         */
        void cb_69() { bit(5, C);              } /* BIT  5,C         */
        void cb_6a() { bit(5, D);              } /* BIT  5,D         */
        void cb_6b() { bit(5, E);              } /* BIT  5,E         */
        void cb_6c() { bit(5, H);              } /* BIT  5,H         */
        void cb_6d() { bit(5, L);              } /* BIT  5,L         */
        void cb_6e() { bit_hl(5, rm(HL));      } /* BIT  5,(HL)      */
        void cb_6f() { bit(5, A);              } /* BIT  5,A         */

        void cb_70() { bit(6, B);              } /* BIT  6,B         */
        void cb_71() { bit(6, C);              } /* BIT  6,C         */
        void cb_72() { bit(6, D);              } /* BIT  6,D         */
        void cb_73() { bit(6, E);              } /* BIT  6,E         */
        void cb_74() { bit(6, H);              } /* BIT  6,H         */
        void cb_75() { bit(6, L);              } /* BIT  6,L         */
        void cb_76() { bit_hl(6, rm(HL));      } /* BIT  6,(HL)      */
        void cb_77() { bit(6, A);              } /* BIT  6,A         */

        void cb_78() { bit(7, B);              } /* BIT  7,B         */
        void cb_79() { bit(7, C);              } /* BIT  7,C         */
        void cb_7a() { bit(7, D);              } /* BIT  7,D         */
        void cb_7b() { bit(7, E);              } /* BIT  7,E         */
        void cb_7c() { bit(7, H);              } /* BIT  7,H         */
        void cb_7d() { bit(7, L);              } /* BIT  7,L         */
        void cb_7e() { bit_hl(7, rm(HL));      } /* BIT  7,(HL)      */
        void cb_7f() { bit(7, A);              } /* BIT  7,A         */

        void cb_80() { B = res(0, B);          } /* RES  0,B         */
        void cb_81() { C = res(0, C);          } /* RES  0,C         */
        void cb_82() { D = res(0, D);          } /* RES  0,D         */
        void cb_83() { E = res(0, E);          } /* RES  0,E         */
        void cb_84() { H = res(0, H);          } /* RES  0,H         */
        void cb_85() { L = res(0, L);          } /* RES  0,L         */
        void cb_86() { wm(HL, res(0, rm(HL))); } /* RES  0,(HL)      */
        void cb_87() { A = res(0, A);          } /* RES  0,A         */

        void cb_88() { B = res(1, B);          } /* RES  1,B         */
        void cb_89() { C = res(1, C);          } /* RES  1,C         */
        void cb_8a() { D = res(1, D);          } /* RES  1,D         */
        void cb_8b() { E = res(1, E);          } /* RES  1,E         */
        void cb_8c() { H = res(1, H);          } /* RES  1,H         */
        void cb_8d() { L = res(1, L);          } /* RES  1,L         */
        void cb_8e() { wm(HL, res(1, rm(HL))); } /* RES  1,(HL)      */
        void cb_8f() { A = res(1, A);          } /* RES  1,A         */

        void cb_90() { B = res(2, B);          } /* RES  2,B         */
        void cb_91() { C = res(2, C);          } /* RES  2,C         */
        void cb_92() { D = res(2, D);          } /* RES  2,D         */
        void cb_93() { E = res(2, E);          } /* RES  2,E         */
        void cb_94() { H = res(2, H);          } /* RES  2,H         */
        void cb_95() { L = res(2, L);          } /* RES  2,L         */
        void cb_96() { wm(HL, res(2, rm(HL))); } /* RES  2,(HL)      */
        void cb_97() { A = res(2, A);          } /* RES  2,A         */

        void cb_98() { B = res(3, B);          } /* RES  3,B         */
        void cb_99() { C = res(3, C);          } /* RES  3,C         */
        void cb_9a() { D = res(3, D);          } /* RES  3,D         */
        void cb_9b() { E = res(3, E);          } /* RES  3,E         */
        void cb_9c() { H = res(3, H);          } /* RES  3,H         */
        void cb_9d() { L = res(3, L);          } /* RES  3,L         */
        void cb_9e() { wm(HL, res(3, rm(HL))); } /* RES  3,(HL)      */
        void cb_9f() { A = res(3, A);          } /* RES  3,A         */

        void cb_a0() { B = res(4, B);          } /* RES  4,B         */
        void cb_a1() { C = res(4, C);          } /* RES  4,C         */
        void cb_a2() { D = res(4, D);          } /* RES  4,D         */
        void cb_a3() { E = res(4, E);          } /* RES  4,E         */
        void cb_a4() { H = res(4, H);          } /* RES  4,H         */
        void cb_a5() { L = res(4, L);          } /* RES  4,L         */
        void cb_a6() { wm(HL, res(4, rm(HL))); } /* RES  4,(HL)      */
        void cb_a7() { A = res(4, A);          } /* RES  4,A         */

        void cb_a8() { B = res(5, B);          } /* RES  5,B         */
        void cb_a9() { C = res(5, C);          } /* RES  5,C         */
        void cb_aa() { D = res(5, D);          } /* RES  5,D         */
        void cb_ab() { E = res(5, E);          } /* RES  5,E         */
        void cb_ac() { H = res(5, H);          } /* RES  5,H         */
        void cb_ad() { L = res(5, L);          } /* RES  5,L         */
        void cb_ae() { wm(HL, res(5, rm(HL))); } /* RES  5,(HL)      */
        void cb_af() { A = res(5, A);          } /* RES  5,A         */

        void cb_b0() { B = res(6, B);          } /* RES  6,B         */
        void cb_b1() { C = res(6, C);          } /* RES  6,C         */
        void cb_b2() { D = res(6, D);          } /* RES  6,D         */
        void cb_b3() { E = res(6, E);          } /* RES  6,E         */
        void cb_b4() { H = res(6, H);          } /* RES  6,H         */
        void cb_b5() { L = res(6, L);          } /* RES  6,L         */
        void cb_b6() { wm(HL, res(6, rm(HL))); } /* RES  6,(HL)      */
        void cb_b7() { A = res(6, A);          } /* RES  6,A         */

        void cb_b8() { B = res(7, B);          } /* RES  7,B         */
        void cb_b9() { C = res(7, C);          } /* RES  7,C         */
        void cb_ba() { D = res(7, D);          } /* RES  7,D         */
        void cb_bb() { E = res(7, E);          } /* RES  7,E         */
        void cb_bc() { H = res(7, H);          } /* RES  7,H         */
        void cb_bd() { L = res(7, L);          } /* RES  7,L         */
        void cb_be() { wm(HL, res(7, rm(HL))); } /* RES  7,(HL)      */
        void cb_bf() { A = res(7, A);          } /* RES  7,A         */

        void cb_c0() { B = set(0, B);          } /* SET  0,B         */
        void cb_c1() { C = set(0, C);          } /* SET  0,C         */
        void cb_c2() { D = set(0, D);          } /* SET  0,D         */
        void cb_c3() { E = set(0, E);          } /* SET  0,E         */
        void cb_c4() { H = set(0, H);          } /* SET  0,H         */
        void cb_c5() { L = set(0, L);          } /* SET  0,L         */
        void cb_c6() { wm(HL, set(0, rm(HL))); } /* SET  0,(HL)      */
        void cb_c7() { A = set(0, A);          } /* SET  0,A         */

        void cb_c8() { B = set(1, B);          } /* SET  1,B         */
        void cb_c9() { C = set(1, C);          } /* SET  1,C         */
        void cb_ca() { D = set(1, D);          } /* SET  1,D         */
        void cb_cb() { E = set(1, E);          } /* SET  1,E         */
        void cb_cc() { H = set(1, H);          } /* SET  1,H         */
        void cb_cd() { L = set(1, L);          } /* SET  1,L         */
        void cb_ce() { wm(HL, set(1, rm(HL))); } /* SET  1,(HL)      */
        void cb_cf() { A = set(1, A);          } /* SET  1,A         */

        void cb_d0() { B = set(2, B);          } /* SET  2,B         */
        void cb_d1() { C = set(2, C);          } /* SET  2,C         */
        void cb_d2() { D = set(2, D);          } /* SET  2,D         */
        void cb_d3() { E = set(2, E);          } /* SET  2,E         */
        void cb_d4() { H = set(2, H);          } /* SET  2,H         */
        void cb_d5() { L = set(2, L);          } /* SET  2,L         */
        void cb_d6() { wm(HL, set(2, rm(HL))); } /* SET  2,(HL)      */
        void cb_d7() { A = set(2, A);          } /* SET  2,A         */

        void cb_d8() { B = set(3, B);          } /* SET  3,B         */
        void cb_d9() { C = set(3, C);          } /* SET  3,C         */
        void cb_da() { D = set(3, D);          } /* SET  3,D         */
        void cb_db() { E = set(3, E);          } /* SET  3,E         */
        void cb_dc() { H = set(3, H);          } /* SET  3,H         */
        void cb_dd() { L = set(3, L);          } /* SET  3,L         */
        void cb_de() { wm(HL, set(3, rm(HL))); } /* SET  3,(HL)      */
        void cb_df() { A = set(3, A);          } /* SET  3,A         */

        void cb_e0() { B = set(4, B);          } /* SET  4,B         */
        void cb_e1() { C = set(4, C);          } /* SET  4,C         */
        void cb_e2() { D = set(4, D);          } /* SET  4,D         */
        void cb_e3() { E = set(4, E);          } /* SET  4,E         */
        void cb_e4() { H = set(4, H);          } /* SET  4,H         */
        void cb_e5() { L = set(4, L);          } /* SET  4,L         */
        void cb_e6() { wm(HL, set(4, rm(HL))); } /* SET  4,(HL)      */
        void cb_e7() { A = set(4, A);          } /* SET  4,A         */

        void cb_e8() { B = set(5, B);          } /* SET  5,B         */
        void cb_e9() { C = set(5, C);          } /* SET  5,C         */
        void cb_ea() { D = set(5, D);          } /* SET  5,D         */
        void cb_eb() { E = set(5, E);          } /* SET  5,E         */
        void cb_ec() { H = set(5, H);          } /* SET  5,H         */
        void cb_ed() { L = set(5, L);          } /* SET  5,L         */
        void cb_ee() { wm(HL, set(5, rm(HL))); } /* SET  5,(HL)      */
        void cb_ef() { A = set(5, A);          } /* SET  5,A         */

        void cb_f0() { B = set(6, B);          } /* SET  6,B         */
        void cb_f1() { C = set(6, C);          } /* SET  6,C         */
        void cb_f2() { D = set(6, D);          } /* SET  6,D         */
        void cb_f3() { E = set(6, E);          } /* SET  6,E         */
        void cb_f4() { H = set(6, H);          } /* SET  6,H         */
        void cb_f5() { L = set(6, L);          } /* SET  6,L         */
        void cb_f6() { wm(HL, set(6, rm(HL))); } /* SET  6,(HL)      */
        void cb_f7() { A = set(6, A);          } /* SET  6,A         */

        void cb_f8() { B = set(7, B);          } /* SET  7,B         */
        void cb_f9() { C = set(7, C);          } /* SET  7,C         */
        void cb_fa() { D = set(7, D);          } /* SET  7,D         */
        void cb_fb() { E = set(7, E);          } /* SET  7,E         */
        void cb_fc() { H = set(7, H);          } /* SET  7,H         */
        void cb_fd() { L = set(7, L);          } /* SET  7,L         */
        void cb_fe() { wm(HL, set(7, rm(HL))); } /* SET  7,(HL)      */
        void cb_ff() { A = set(7, A);          } /* SET  7,A         */


        //PROTOTYPES(dd);
        //#define void prefix,opcode) inline void z80_device::prefix##_##opcode()
        /**********************************************************
         * IX register related opcodes (DD prefix)
         **********************************************************/
        void dd_00() { illegal_1(); op_00();                            } /* DB   DD          */
        void dd_01() { illegal_1(); op_01();                            } /* DB   DD          */
        void dd_02() { illegal_1(); op_02();                            } /* DB   DD          */
        void dd_03() { illegal_1(); op_03();                            } /* DB   DD          */
        void dd_04() { illegal_1(); op_04();                            } /* DB   DD          */
        void dd_05() { illegal_1(); op_05();                            } /* DB   DD          */
        void dd_06() { illegal_1(); op_06();                            } /* DB   DD          */
        void dd_07() { illegal_1(); op_07();                            } /* DB   DD          */

        void dd_08() { illegal_1(); op_08();                            } /* DB   DD          */
        void dd_09() { add16(ref m_ix, m_bc);                               } /* ADD  IX,BC       */
        void dd_0a() { illegal_1(); op_0a();                            } /* DB   DD          */
        void dd_0b() { illegal_1(); op_0b();                            } /* DB   DD          */
        void dd_0c() { illegal_1(); op_0c();                            } /* DB   DD          */
        void dd_0d() { illegal_1(); op_0d();                            } /* DB   DD          */
        void dd_0e() { illegal_1(); op_0e();                            } /* DB   DD          */
        void dd_0f() { illegal_1(); op_0f();                            } /* DB   DD          */

        void dd_10() { illegal_1(); op_10();                            } /* DB   DD          */
        void dd_11() { illegal_1(); op_11();                            } /* DB   DD          */
        void dd_12() { illegal_1(); op_12();                            } /* DB   DD          */
        void dd_13() { illegal_1(); op_13();                            } /* DB   DD          */
        void dd_14() { illegal_1(); op_14();                            } /* DB   DD          */
        void dd_15() { illegal_1(); op_15();                            } /* DB   DD          */
        void dd_16() { illegal_1(); op_16();                            } /* DB   DD          */
        void dd_17() { illegal_1(); op_17();                            } /* DB   DD          */

        void dd_18() { illegal_1(); op_18();                            } /* DB   DD          */
        void dd_19() { add16(ref m_ix, m_de);                               } /* ADD  IX,DE       */
        void dd_1a() { illegal_1(); op_1a();                            } /* DB   DD          */
        void dd_1b() { illegal_1(); op_1b();                            } /* DB   DD          */
        void dd_1c() { illegal_1(); op_1c();                            } /* DB   DD          */
        void dd_1d() { illegal_1(); op_1d();                            } /* DB   DD          */
        void dd_1e() { illegal_1(); op_1e();                            } /* DB   DD          */
        void dd_1f() { illegal_1(); op_1f();                            } /* DB   DD          */

        void dd_20() { illegal_1(); op_20();                            } /* DB   DD          */
        void dd_21() { IX = arg16();                                    } /* LD   IX,w        */
        void dd_22() { m_ea = arg16(); wm16((UInt16)m_ea, m_ix); WZ = (UInt16)(m_ea + 1); } /* LD   (w),IX      */
        void dd_23() { IX++;                                            } /* INC  IX          */
        void dd_24() { HX = inc(HX);                                    } /* INC  HX          */
        void dd_25() { HX = dec(HX);                                    } /* DEC  HX          */
        void dd_26() { HX = arg();                                      } /* LD   HX,n        */
        void dd_27() { illegal_1(); op_27();                            } /* DB   DD          */

        void dd_28() { illegal_1(); op_28();                            } /* DB   DD          */
        void dd_29() { add16(ref m_ix, m_ix);                               } /* ADD  IX,IX       */
        void dd_2a() { m_ea = arg16(); rm16((UInt16)m_ea, ref m_ix); WZ = (UInt16)(m_ea + 1); } /* LD   IX,(w)      */
        void dd_2b() { IX--;                                            } /* DEC  IX          */
        void dd_2c() { LX = inc(LX);                                    } /* INC  LX          */
        void dd_2d() { LX = dec(LX);                                    } /* DEC  LX          */
        void dd_2e() { LX = arg();                                      } /* LD   LX,n        */
        void dd_2f() { illegal_1(); op_2f();                            } /* DB   DD          */

        void dd_30() { illegal_1(); op_30();                            } /* DB   DD          */
        void dd_31() { illegal_1(); op_31();                            } /* DB   DD          */
        void dd_32() { illegal_1(); op_32();                            } /* DB   DD          */
        void dd_33() { illegal_1(); op_33();                            } /* DB   DD          */
        void dd_34() { eax(); wm((UInt16)m_ea, inc(rm((UInt16)m_ea)));                  } /* INC  (IX+o)      */
        void dd_35() { eax(); wm((UInt16)m_ea, dec(rm((UInt16)m_ea)));                  } /* DEC  (IX+o)      */
        void dd_36() { eax(); wm((UInt16)m_ea, arg());                          } /* LD   (IX+o),n    */
        void dd_37() { illegal_1(); op_37();                            } /* DB   DD          */

        void dd_38() { illegal_1(); op_38();                            } /* DB   DD          */
        void dd_39() { add16(ref m_ix, m_sp);                               } /* ADD  IX,SP       */
        void dd_3a() { illegal_1(); op_3a();                            } /* DB   DD          */
        void dd_3b() { illegal_1(); op_3b();                            } /* DB   DD          */
        void dd_3c() { illegal_1(); op_3c();                            } /* DB   DD          */
        void dd_3d() { illegal_1(); op_3d();                            } /* DB   DD          */
        void dd_3e() { illegal_1(); op_3e();                            } /* DB   DD          */
        void dd_3f() { illegal_1(); op_3f();                            } /* DB   DD          */

        void dd_40() { illegal_1(); op_40();                            } /* DB   DD          */
        void dd_41() { illegal_1(); op_41();                            } /* DB   DD          */
        void dd_42() { illegal_1(); op_42();                            } /* DB   DD          */
        void dd_43() { illegal_1(); op_43();                            } /* DB   DD          */
        void dd_44() { B = HX;                                          } /* LD   B,HX        */
        void dd_45() { B = LX;                                          } /* LD   B,LX        */
        void dd_46() { eax(); B = rm((UInt16)m_ea);                             } /* LD   B,(IX+o)    */
        void dd_47() { illegal_1(); op_47();                            } /* DB   DD          */

        void dd_48() { illegal_1(); op_48();                            } /* DB   DD          */
        void dd_49() { illegal_1(); op_49();                            } /* DB   DD          */
        void dd_4a() { illegal_1(); op_4a();                            } /* DB   DD          */
        void dd_4b() { illegal_1(); op_4b();                            } /* DB   DD          */
        void dd_4c() { C = HX;                                          } /* LD   C,HX        */
        void dd_4d() { C = LX;                                          } /* LD   C,LX        */
        void dd_4e() { eax(); C = rm((UInt16)m_ea);                             } /* LD   C,(IX+o)    */
        void dd_4f() { illegal_1(); op_4f();                            } /* DB   DD          */

        void dd_50() { illegal_1(); op_50();                            } /* DB   DD          */
        void dd_51() { illegal_1(); op_51();                            } /* DB   DD          */
        void dd_52() { illegal_1(); op_52();                            } /* DB   DD          */
        void dd_53() { illegal_1(); op_53();                            } /* DB   DD          */
        void dd_54() { D = HX;                                          } /* LD   D,HX        */
        void dd_55() { D = LX;                                          } /* LD   D,LX        */
        void dd_56() { eax(); D = rm((UInt16)m_ea);                             } /* LD   D,(IX+o)    */
        void dd_57() { illegal_1(); op_57();                            } /* DB   DD          */

        void dd_58() { illegal_1(); op_58();                            } /* DB   DD          */
        void dd_59() { illegal_1(); op_59();                            } /* DB   DD          */
        void dd_5a() { illegal_1(); op_5a();                            } /* DB   DD          */
        void dd_5b() { illegal_1(); op_5b();                            } /* DB   DD          */
        void dd_5c() { E = HX;                                          } /* LD   E,HX        */
        void dd_5d() { E = LX;                                          } /* LD   E,LX        */
        void dd_5e() { eax(); E = rm((UInt16)m_ea);                             } /* LD   E,(IX+o)    */
        void dd_5f() { illegal_1(); op_5f();                            } /* DB   DD          */

        void dd_60() { HX = B;                                          } /* LD   HX,B        */
        void dd_61() { HX = C;                                          } /* LD   HX,C        */
        void dd_62() { HX = D;                                          } /* LD   HX,D        */
        void dd_63() { HX = E;                                          } /* LD   HX,E        */
        void dd_64() {                                                  } /* LD   HX,HX       */
        void dd_65() { HX = LX;                                         } /* LD   HX,LX       */
        void dd_66() { eax(); H = rm((UInt16)m_ea);                             } /* LD   H,(IX+o)    */
        void dd_67() { HX = A;                                          } /* LD   HX,A        */

        void dd_68() { LX = B;                                          } /* LD   LX,B        */
        void dd_69() { LX = C;                                          } /* LD   LX,C        */
        void dd_6a() { LX = D;                                          } /* LD   LX,D        */
        void dd_6b() { LX = E;                                          } /* LD   LX,E        */
        void dd_6c() { LX = HX;                                         } /* LD   LX,HX       */
        void dd_6d() {                                                  } /* LD   LX,LX       */
        void dd_6e() { eax(); L = rm((UInt16)m_ea);                             } /* LD   L,(IX+o)    */
        void dd_6f() { LX = A;                                          } /* LD   LX,A        */

        void dd_70() { eax(); wm((UInt16)m_ea, B);                              } /* LD   (IX+o),B    */
        void dd_71() { eax(); wm((UInt16)m_ea, C);                              } /* LD   (IX+o),C    */
        void dd_72() { eax(); wm((UInt16)m_ea, D);                              } /* LD   (IX+o),D    */
        void dd_73() { eax(); wm((UInt16)m_ea, E);                              } /* LD   (IX+o),E    */
        void dd_74() { eax(); wm((UInt16)m_ea, H);                              } /* LD   (IX+o),H    */
        void dd_75() { eax(); wm((UInt16)m_ea, L);                              } /* LD   (IX+o),L    */
        void dd_76() { illegal_1(); op_76();                            } /* DB   DD          */
        void dd_77() { eax(); wm((UInt16)m_ea, A);                              } /* LD   (IX+o),A    */

        void dd_78() { illegal_1(); op_78();                            } /* DB   DD          */
        void dd_79() { illegal_1(); op_79();                            } /* DB   DD          */
        void dd_7a() { illegal_1(); op_7a();                            } /* DB   DD          */
        void dd_7b() { illegal_1(); op_7b();                            } /* DB   DD          */
        void dd_7c() { A = HX;                                          } /* LD   A,HX        */
        void dd_7d() { A = LX;                                          } /* LD   A,LX        */
        void dd_7e() { eax(); A = rm((UInt16)m_ea);                             } /* LD   A,(IX+o)    */
        void dd_7f() { illegal_1(); op_7f();                            } /* DB   DD          */

        void dd_80() { illegal_1(); op_80();                            } /* DB   DD          */
        void dd_81() { illegal_1(); op_81();                            } /* DB   DD          */
        void dd_82() { illegal_1(); op_82();                            } /* DB   DD          */
        void dd_83() { illegal_1(); op_83();                            } /* DB   DD          */
        void dd_84() { add_a(HX);                                       } /* ADD  A,HX        */
        void dd_85() { add_a(LX);                                       } /* ADD  A,LX        */
        void dd_86() { eax(); add_a(rm((UInt16)m_ea));                          } /* ADD  A,(IX+o)    */
        void dd_87() { illegal_1(); op_87();                            } /* DB   DD          */

        void dd_88() { illegal_1(); op_88();                            } /* DB   DD          */
        void dd_89() { illegal_1(); op_89();                            } /* DB   DD          */
        void dd_8a() { illegal_1(); op_8a();                            } /* DB   DD          */
        void dd_8b() { illegal_1(); op_8b();                            } /* DB   DD          */
        void dd_8c() { adc_a(HX);                                       } /* ADC  A,HX        */
        void dd_8d() { adc_a(LX);                                       } /* ADC  A,LX        */
        void dd_8e() { eax(); adc_a(rm((UInt16)m_ea));                          } /* ADC  A,(IX+o)    */
        void dd_8f() { illegal_1(); op_8f();                            } /* DB   DD          */

        void dd_90() { illegal_1(); op_90();                            } /* DB   DD          */
        void dd_91() { illegal_1(); op_91();                            } /* DB   DD          */
        void dd_92() { illegal_1(); op_92();                            } /* DB   DD          */
        void dd_93() { illegal_1(); op_93();                            } /* DB   DD          */
        void dd_94() { sub(HX);                                         } /* SUB  HX          */
        void dd_95() { sub(LX);                                         } /* SUB  LX          */
        void dd_96() { eax(); sub(rm((UInt16)m_ea));                            } /* SUB  (IX+o)      */
        void dd_97() { illegal_1(); op_97();                            } /* DB   DD          */

        void dd_98() { illegal_1(); op_98();                            } /* DB   DD          */
        void dd_99() { illegal_1(); op_99();                            } /* DB   DD          */
        void dd_9a() { illegal_1(); op_9a();                            } /* DB   DD          */
        void dd_9b() { illegal_1(); op_9b();                            } /* DB   DD          */
        void dd_9c() { sbc_a(HX);                                       } /* SBC  A,HX        */
        void dd_9d() { sbc_a(LX);                                       } /* SBC  A,LX        */
        void dd_9e() { eax(); sbc_a(rm((UInt16)m_ea));                          } /* SBC  A,(IX+o)    */
        void dd_9f() { illegal_1(); op_9f();                            } /* DB   DD          */

        void dd_a0() { illegal_1(); op_a0();                            } /* DB   DD          */
        void dd_a1() { illegal_1(); op_a1();                            } /* DB   DD          */
        void dd_a2() { illegal_1(); op_a2();                            } /* DB   DD          */
        void dd_a3() { illegal_1(); op_a3();                            } /* DB   DD          */
        void dd_a4() { and_a(HX);                                       } /* AND  HX          */
        void dd_a5() { and_a(LX);                                       } /* AND  LX          */
        void dd_a6() { eax(); and_a(rm((UInt16)m_ea));                          } /* AND  (IX+o)      */
        void dd_a7() { illegal_1(); op_a7();                            } /* DB   DD          */

        void dd_a8() { illegal_1(); op_a8();                            } /* DB   DD          */
        void dd_a9() { illegal_1(); op_a9();                            } /* DB   DD          */
        void dd_aa() { illegal_1(); op_aa();                            } /* DB   DD          */
        void dd_ab() { illegal_1(); op_ab();                            } /* DB   DD          */
        void dd_ac() { xor_a(HX);                                       } /* XOR  HX          */
        void dd_ad() { xor_a(LX);                                       } /* XOR  LX          */
        void dd_ae() { eax(); xor_a(rm((UInt16)m_ea));                          } /* XOR  (IX+o)      */
        void dd_af() { illegal_1(); op_af();                            } /* DB   DD          */

        void dd_b0() { illegal_1(); op_b0();                            } /* DB   DD          */
        void dd_b1() { illegal_1(); op_b1();                            } /* DB   DD          */
        void dd_b2() { illegal_1(); op_b2();                            } /* DB   DD          */
        void dd_b3() { illegal_1(); op_b3();                            } /* DB   DD          */
        void dd_b4() { or_a(HX);                                        } /* OR   HX          */
        void dd_b5() { or_a(LX);                                        } /* OR   LX          */
        void dd_b6() { eax(); or_a(rm((UInt16)m_ea));                           } /* OR   (IX+o)      */
        void dd_b7() { illegal_1(); op_b7();                            } /* DB   DD          */

        void dd_b8() { illegal_1(); op_b8();                            } /* DB   DD          */
        void dd_b9() { illegal_1(); op_b9();                            } /* DB   DD          */
        void dd_ba() { illegal_1(); op_ba();                            } /* DB   DD          */
        void dd_bb() { illegal_1(); op_bb();                            } /* DB   DD          */
        void dd_bc() { cp(HX);                                          } /* CP   HX          */
        void dd_bd() { cp(LX);                                          } /* CP   LX          */
        void dd_be() { eax(); cp(rm((UInt16)m_ea));                             } /* CP   (IX+o)      */
        void dd_bf() { illegal_1(); op_bf();                            } /* DB   DD          */

        void dd_c0() { illegal_1(); op_c0();                            } /* DB   DD          */
        void dd_c1() { illegal_1(); op_c1();                            } /* DB   DD          */
        void dd_c2() { illegal_1(); op_c2();                            } /* DB   DD          */
        void dd_c3() { illegal_1(); op_c3();                            } /* DB   DD          */
        void dd_c4() { illegal_1(); op_c4();                            } /* DB   DD          */
        void dd_c5() { illegal_1(); op_c5();                            } /* DB   DD          */
        void dd_c6() { illegal_1(); op_c6();                            } /* DB   DD          */
        void dd_c7() { illegal_1(); op_c7();                            } /* DB   DD          */

        void dd_c8() { illegal_1(); op_c8();                            } /* DB   DD          */
        void dd_c9() { illegal_1(); op_c9();                            } /* DB   DD          */
        void dd_ca() { illegal_1(); op_ca();                            } /* DB   DD          */
        void dd_cb() { eax(); EXEC_xycb(arg());                         } /* **   DD CB xx    */
        void dd_cc() { illegal_1(); op_cc();                            } /* DB   DD          */
        void dd_cd() { illegal_1(); op_cd();                            } /* DB   DD          */
        void dd_ce() { illegal_1(); op_ce();                            } /* DB   DD          */
        void dd_cf() { illegal_1(); op_cf();                            } /* DB   DD          */

        void dd_d0() { illegal_1(); op_d0();                            } /* DB   DD          */
        void dd_d1() { illegal_1(); op_d1();                            } /* DB   DD          */
        void dd_d2() { illegal_1(); op_d2();                            } /* DB   DD          */
        void dd_d3() { illegal_1(); op_d3();                            } /* DB   DD          */
        void dd_d4() { illegal_1(); op_d4();                            } /* DB   DD          */
        void dd_d5() { illegal_1(); op_d5();                            } /* DB   DD          */
        void dd_d6() { illegal_1(); op_d6();                            } /* DB   DD          */
        void dd_d7() { illegal_1(); op_d7();                            } /* DB   DD          */

        void dd_d8() { illegal_1(); op_d8();                            } /* DB   DD          */
        void dd_d9() { illegal_1(); op_d9();                            } /* DB   DD          */
        void dd_da() { illegal_1(); op_da();                            } /* DB   DD          */
        void dd_db() { illegal_1(); op_db();                            } /* DB   DD          */
        void dd_dc() { illegal_1(); op_dc();                            } /* DB   DD          */
        void dd_dd() { illegal_1(); op_dd();                            } /* DB   DD          */
        void dd_de() { illegal_1(); op_de();                            } /* DB   DD          */
        void dd_df() { illegal_1(); op_df();                            } /* DB   DD          */

        void dd_e0() { illegal_1(); op_e0();                            } /* DB   DD          */
        void dd_e1() { pop(ref m_ix);                                       } /* POP  IX          */
        void dd_e2() { illegal_1(); op_e2();                            } /* DB   DD          */
        void dd_e3() { ex_sp(ref m_ix);                                     } /* EX   (SP),IX     */
        void dd_e4() { illegal_1(); op_e4();                            } /* DB   DD          */
        void dd_e5() { push(m_ix);                                      } /* PUSH IX          */
        void dd_e6() { illegal_1(); op_e6();                            } /* DB   DD          */
        void dd_e7() { illegal_1(); op_e7();                            } /* DB   DD          */

        void dd_e8() { illegal_1(); op_e8();                            } /* DB   DD          */
        void dd_e9() { PC = IX;                                         } /* JP   (IX)        */
        void dd_ea() { illegal_1(); op_ea();                            } /* DB   DD          */
        void dd_eb() { illegal_1(); op_eb();                            } /* DB   DD          */
        void dd_ec() { illegal_1(); op_ec();                            } /* DB   DD          */
        void dd_ed() { illegal_1(); op_ed();                            } /* DB   DD          */
        void dd_ee() { illegal_1(); op_ee();                            } /* DB   DD          */
        void dd_ef() { illegal_1(); op_ef();                            } /* DB   DD          */

        void dd_f0() { illegal_1(); op_f0();                            } /* DB   DD          */
        void dd_f1() { illegal_1(); op_f1();                            } /* DB   DD          */
        void dd_f2() { illegal_1(); op_f2();                            } /* DB   DD          */
        void dd_f3() { illegal_1(); op_f3();                            } /* DB   DD          */
        void dd_f4() { illegal_1(); op_f4();                            } /* DB   DD          */
        void dd_f5() { illegal_1(); op_f5();                            } /* DB   DD          */
        void dd_f6() { illegal_1(); op_f6();                            } /* DB   DD          */
        void dd_f7() { illegal_1(); op_f7();                            } /* DB   DD          */

        void dd_f8() { illegal_1(); op_f8();                            } /* DB   DD          */
        void dd_f9() { SP = IX;                                         } /* LD   SP,IX       */
        void dd_fa() { illegal_1(); op_fa();                            } /* DB   DD          */
        void dd_fb() { illegal_1(); op_fb();                            } /* DB   DD          */
        void dd_fc() { illegal_1(); op_fc();                            } /* DB   DD          */
        void dd_fd() { illegal_1(); op_fd();                            } /* DB   DD          */
        void dd_fe() { illegal_1(); op_fe();                            } /* DB   DD          */
        void dd_ff() { illegal_1(); op_ff();                            } /* DB   DD          */


        void illegal_2() { logerror("Z80 ill. opcode $ed ${0}\n", m_opcodes.read_byte((PCD-1)&0xffff)); }  // $%02x


        //PROTOTYPES(ed);
        //#define void prefix,opcode) inline void z80_device::prefix##_##opcode()
        /**********************************************************
         * special opcodes (ED prefix)
         **********************************************************/
        void ed_00() { illegal_2();                                     } /* DB   ED          */
        void ed_01() { illegal_2();                                     } /* DB   ED          */
        void ed_02() { illegal_2();                                     } /* DB   ED          */
        void ed_03() { illegal_2();                                     } /* DB   ED          */
        void ed_04() { illegal_2();                                     } /* DB   ED          */
        void ed_05() { illegal_2();                                     } /* DB   ED          */
        void ed_06() { illegal_2();                                     } /* DB   ED          */
        void ed_07() { illegal_2();                                     } /* DB   ED          */

        void ed_08() { illegal_2();                                     } /* DB   ED          */
        void ed_09() { illegal_2();                                     } /* DB   ED          */
        void ed_0a() { illegal_2();                                     } /* DB   ED          */
        void ed_0b() { illegal_2();                                     } /* DB   ED          */
        void ed_0c() { illegal_2();                                     } /* DB   ED          */
        void ed_0d() { illegal_2();                                     } /* DB   ED          */
        void ed_0e() { illegal_2();                                     } /* DB   ED          */
        void ed_0f() { illegal_2();                                     } /* DB   ED          */

        void ed_10() { illegal_2();                                     } /* DB   ED          */
        void ed_11() { illegal_2();                                     } /* DB   ED          */
        void ed_12() { illegal_2();                                     } /* DB   ED          */
        void ed_13() { illegal_2();                                     } /* DB   ED          */
        void ed_14() { illegal_2();                                     } /* DB   ED          */
        void ed_15() { illegal_2();                                     } /* DB   ED          */
        void ed_16() { illegal_2();                                     } /* DB   ED          */
        void ed_17() { illegal_2();                                     } /* DB   ED          */

        void ed_18() { illegal_2();                                     } /* DB   ED          */
        void ed_19() { illegal_2();                                     } /* DB   ED          */
        void ed_1a() { illegal_2();                                     } /* DB   ED          */
        void ed_1b() { illegal_2();                                     } /* DB   ED          */
        void ed_1c() { illegal_2();                                     } /* DB   ED          */
        void ed_1d() { illegal_2();                                     } /* DB   ED          */
        void ed_1e() { illegal_2();                                     } /* DB   ED          */
        void ed_1f() { illegal_2();                                     } /* DB   ED          */

        void ed_20() { illegal_2();                                     } /* DB   ED          */
        void ed_21() { illegal_2();                                     } /* DB   ED          */
        void ed_22() { illegal_2();                                     } /* DB   ED          */
        void ed_23() { illegal_2();                                     } /* DB   ED          */
        void ed_24() { illegal_2();                                     } /* DB   ED          */
        void ed_25() { illegal_2();                                     } /* DB   ED          */
        void ed_26() { illegal_2();                                     } /* DB   ED          */
        void ed_27() { illegal_2();                                     } /* DB   ED          */

        void ed_28() { illegal_2();                                     } /* DB   ED          */
        void ed_29() { illegal_2();                                     } /* DB   ED          */
        void ed_2a() { illegal_2();                                     } /* DB   ED          */
        void ed_2b() { illegal_2();                                     } /* DB   ED          */
        void ed_2c() { illegal_2();                                     } /* DB   ED          */
        void ed_2d() { illegal_2();                                     } /* DB   ED          */
        void ed_2e() { illegal_2();                                     } /* DB   ED          */
        void ed_2f() { illegal_2();                                     } /* DB   ED          */

        void ed_30() { illegal_2();                                     } /* DB   ED          */
        void ed_31() { illegal_2();                                     } /* DB   ED          */
        void ed_32() { illegal_2();                                     } /* DB   ED          */
        void ed_33() { illegal_2();                                     } /* DB   ED          */
        void ed_34() { illegal_2();                                     } /* DB   ED          */
        void ed_35() { illegal_2();                                     } /* DB   ED          */
        void ed_36() { illegal_2();                                     } /* DB   ED          */
        void ed_37() { illegal_2();                                     } /* DB   ED          */

        void ed_38() { illegal_2();                                     } /* DB   ED          */
        void ed_39() { illegal_2();                                     } /* DB   ED          */
        void ed_3a() { illegal_2();                                     } /* DB   ED          */
        void ed_3b() { illegal_2();                                     } /* DB   ED          */
        void ed_3c() { illegal_2();                                     } /* DB   ED          */
        void ed_3d() { illegal_2();                                     } /* DB   ED          */
        void ed_3e() { illegal_2();                                     } /* DB   ED          */
        void ed_3f() { illegal_2();                                     } /* DB   ED          */

        void ed_40() { B = in_(BC); F = (byte)((F & CF) | SZP[B]);               } /* IN   B,(C)       */
        void ed_41() { out_(BC, B);                                      } /* OUT  (C),B       */
        void ed_42() { sbc_hl(m_bc);                                    } /* SBC  HL,BC       */
        void ed_43() { m_ea = arg16(); wm16((UInt16)m_ea, m_bc); WZ = (UInt16)(m_ea + 1); } /* LD   (w),BC      */
        void ed_44() { neg();                                           } /* NEG              */
        void ed_45() { retn();                                          } /* RETN             */
        void ed_46() { m_im = 0;                                        } /* IM   0           */
        void ed_47() { ld_i_a();                                        } /* LD   i,A         */

        void ed_48() { C = in_(BC); F = (byte)((F & CF) | SZP[C]);               } /* IN   C,(C)       */
        void ed_49() { out_(BC, C);                                      } /* OUT  (C),C       */
        void ed_4a() { adc_hl(m_bc);                                    } /* ADC  HL,BC       */
        void ed_4b() { m_ea = arg16(); rm16((UInt16)m_ea, ref m_bc); WZ = (UInt16)(m_ea + 1); } /* LD   BC,(w)      */
        void ed_4c() { neg();                                           } /* NEG              */
        void ed_4d() { reti();                                          } /* RETI             */
        void ed_4e() { m_im = 0;                                        } /* IM   0           */
        void ed_4f() { ld_r_a();                                        } /* LD   r,A         */

        void ed_50() { D = in_(BC); F = (byte)((F & CF) | SZP[D]);               } /* IN   D,(C)       */
        void ed_51() { out_(BC, D);                                      } /* OUT  (C),D       */
        void ed_52() { sbc_hl(m_de);                                    } /* SBC  HL,DE       */
        void ed_53() { m_ea = arg16(); wm16((UInt16)m_ea, m_de); WZ = (UInt16)(m_ea + 1); } /* LD   (w),DE      */
        void ed_54() { neg();                                           } /* NEG              */
        void ed_55() { retn();                                          } /* RETN             */
        void ed_56() { m_im = 1;                                        } /* IM   1           */
        void ed_57() { ld_a_i();                                        } /* LD   A,i         */

        void ed_58() { E = in_(BC); F = (byte)((F & CF) | SZP[E]);               } /* IN   E,(C)       */
        void ed_59() { out_(BC, E);                                      } /* OUT  (C),E       */
        void ed_5a() { adc_hl(m_de);                                    } /* ADC  HL,DE       */
        void ed_5b() { m_ea = arg16(); rm16((UInt16)m_ea, ref m_de); WZ = (UInt16)(m_ea + 1); } /* LD   DE,(w)      */
        void ed_5c() { neg();                                           } /* NEG              */
        void ed_5d() { reti();                                          } /* RETI             */
        void ed_5e() { m_im = 2;                                        } /* IM   2           */
        void ed_5f() { ld_a_r();                                        } /* LD   A,r         */

        void ed_60() { H = in_(BC); F = (byte)((F & CF) | SZP[H]);               } /* IN   H,(C)       */
        void ed_61() { out_(BC, H);                                      } /* OUT  (C),H       */
        void ed_62() { sbc_hl(m_hl);                                    } /* SBC  HL,HL       */
        void ed_63() { m_ea = arg16(); wm16((UInt16)m_ea, m_hl); WZ = (UInt16)(m_ea + 1); } /* LD   (w),HL      */
        void ed_64() { neg();                                           } /* NEG              */
        void ed_65() { retn();                                          } /* RETN             */
        void ed_66() { m_im = 0;                                        } /* IM   0           */
        void ed_67() { rrd();                                           } /* RRD  (HL)        */

        void ed_68() { L = in_(BC); F = (byte)((F & CF) | SZP[L]);               } /* IN   L,(C)       */
        void ed_69() { out_(BC, L);                                      } /* OUT  (C),L       */
        void ed_6a() { adc_hl(m_hl);                                    } /* ADC  HL,HL       */
        void ed_6b() { m_ea = arg16(); rm16((UInt16)m_ea, ref m_hl); WZ = (UInt16)(m_ea + 1); } /* LD   HL,(w)      */
        void ed_6c() { neg();                                           } /* NEG              */
        void ed_6d() { reti();                                          } /* RETI             */
        void ed_6e() { m_im = 0;                                        } /* IM   0           */
        void ed_6f() { rld();                                           } /* RLD  (HL)        */

        void ed_70() { byte res = in_(BC); F = (byte)((F & CF) | SZP[res]);     } /* IN   0,(C)       */
        void ed_71() { out_(BC, 0);                                      } /* OUT  (C),0       */
        void ed_72() { sbc_hl(m_sp);                                    } /* SBC  HL,SP       */
        void ed_73() { m_ea = arg16(); wm16((UInt16)m_ea, m_sp); WZ = (UInt16)(m_ea + 1); } /* LD   (w),SP      */
        void ed_74() { neg();                                           } /* NEG              */
        void ed_75() { retn();                                          } /* RETN             */
        void ed_76() { m_im = 1;                                        } /* IM   1           */
        void ed_77() { illegal_2();                                     } /* DB   ED,77       */

        void ed_78() { A = in_(BC); F = (byte)((F & CF) | SZP[A]); WZ = (UInt16)(BC + 1);  } /* IN   A,(C)       */
        void ed_79() { out_(BC, A);  WZ = (UInt16)(BC + 1);                        } /* OUT  (C),A       */
        void ed_7a() { adc_hl(m_sp);                                    } /* ADC  HL,SP       */
        void ed_7b() { m_ea = arg16(); rm16((UInt16)m_ea, ref m_sp); WZ = (UInt16)(m_ea + 1); } /* LD   SP,(w)      */
        void ed_7c() { neg();                                           } /* NEG              */
        void ed_7d() { reti();                                          } /* RETI             */
        void ed_7e() { m_im = 2;                                        } /* IM   2           */
        void ed_7f() { illegal_2();                                     } /* DB   ED,7F       */

        void ed_80() { illegal_2();                                     } /* DB   ED          */
        void ed_81() { illegal_2();                                     } /* DB   ED          */
        void ed_82() { illegal_2();                                     } /* DB   ED          */
        void ed_83() { illegal_2();                                     } /* DB   ED          */
        void ed_84() { illegal_2();                                     } /* DB   ED          */
        void ed_85() { illegal_2();                                     } /* DB   ED          */
        void ed_86() { illegal_2();                                     } /* DB   ED          */
        void ed_87() { illegal_2();                                     } /* DB   ED          */

        void ed_88() { illegal_2();                                     } /* DB   ED          */
        void ed_89() { illegal_2();                                     } /* DB   ED          */
        void ed_8a() { illegal_2();                                     } /* DB   ED          */
        void ed_8b() { illegal_2();                                     } /* DB   ED          */
        void ed_8c() { illegal_2();                                     } /* DB   ED          */
        void ed_8d() { illegal_2();                                     } /* DB   ED          */
        void ed_8e() { illegal_2();                                     } /* DB   ED          */
        void ed_8f() { illegal_2();                                     } /* DB   ED          */

        void ed_90() { illegal_2();                                     } /* DB   ED          */
        void ed_91() { illegal_2();                                     } /* DB   ED          */
        void ed_92() { illegal_2();                                     } /* DB   ED          */
        void ed_93() { illegal_2();                                     } /* DB   ED          */
        void ed_94() { illegal_2();                                     } /* DB   ED          */
        void ed_95() { illegal_2();                                     } /* DB   ED          */
        void ed_96() { illegal_2();                                     } /* DB   ED          */
        void ed_97() { illegal_2();                                     } /* DB   ED          */

        void ed_98() { illegal_2();                                     } /* DB   ED          */
        void ed_99() { illegal_2();                                     } /* DB   ED          */
        void ed_9a() { illegal_2();                                     } /* DB   ED          */
        void ed_9b() { illegal_2();                                     } /* DB   ED          */
        void ed_9c() { illegal_2();                                     } /* DB   ED          */
        void ed_9d() { illegal_2();                                     } /* DB   ED          */
        void ed_9e() { illegal_2();                                     } /* DB   ED          */
        void ed_9f() { illegal_2();                                     } /* DB   ED          */

        void ed_a0() { ldi();                                           } /* LDI              */
        void ed_a1() { cpi();                                           } /* CPI              */
        void ed_a2() { ini();                                           } /* INI              */
        void ed_a3() { outi();                                          } /* OUTI             */
        void ed_a4() { illegal_2();                                     } /* DB   ED          */
        void ed_a5() { illegal_2();                                     } /* DB   ED          */
        void ed_a6() { illegal_2();                                     } /* DB   ED          */
        void ed_a7() { illegal_2();                                     } /* DB   ED          */

        void ed_a8() { ldd();                                           } /* LDD              */
        void ed_a9() { cpd();                                           } /* CPD              */
        void ed_aa() { ind();                                           } /* IND              */
        void ed_ab() { outd();                                          } /* OUTD             */
        void ed_ac() { illegal_2();                                     } /* DB   ED          */
        void ed_ad() { illegal_2();                                     } /* DB   ED          */
        void ed_ae() { illegal_2();                                     } /* DB   ED          */
        void ed_af() { illegal_2();                                     } /* DB   ED          */

        void ed_b0() { ldir();                                          } /* LDIR             */
        void ed_b1() { cpir();                                          } /* CPIR             */
        void ed_b2() { inir();                                          } /* INIR             */
        void ed_b3() { otir();                                          } /* OTIR             */
        void ed_b4() { illegal_2();                                     } /* DB   ED          */
        void ed_b5() { illegal_2();                                     } /* DB   ED          */
        void ed_b6() { illegal_2();                                     } /* DB   ED          */
        void ed_b7() { illegal_2();                                     } /* DB   ED          */

        void ed_b8() { lddr();                                          } /* LDDR             */
        void ed_b9() { cpdr();                                          } /* CPDR             */
        void ed_ba() { indr();                                          } /* INDR             */
        void ed_bb() { otdr();                                          } /* OTDR             */
        void ed_bc() { illegal_2();                                     } /* DB   ED          */
        void ed_bd() { illegal_2();                                     } /* DB   ED          */
        void ed_be() { illegal_2();                                     } /* DB   ED          */
        void ed_bf() { illegal_2();                                     } /* DB   ED          */

        void ed_c0() { illegal_2();                                     } /* DB   ED          */
        void ed_c1() { illegal_2();                                     } /* DB   ED          */
        void ed_c2() { illegal_2();                                     } /* DB   ED          */
        void ed_c3() { illegal_2();                                     } /* DB   ED          */
        void ed_c4() { illegal_2();                                     } /* DB   ED          */
        void ed_c5() { illegal_2();                                     } /* DB   ED          */
        void ed_c6() { illegal_2();                                     } /* DB   ED          */
        void ed_c7() { illegal_2();                                     } /* DB   ED          */

        void ed_c8() { illegal_2();                                     } /* DB   ED          */
        void ed_c9() { illegal_2();                                     } /* DB   ED          */
        void ed_ca() { illegal_2();                                     } /* DB   ED          */
        void ed_cb() { illegal_2();                                     } /* DB   ED          */
        void ed_cc() { illegal_2();                                     } /* DB   ED          */
        void ed_cd() { illegal_2();                                     } /* DB   ED          */
        void ed_ce() { illegal_2();                                     } /* DB   ED          */
        void ed_cf() { illegal_2();                                     } /* DB   ED          */

        void ed_d0() { illegal_2();                                     } /* DB   ED          */
        void ed_d1() { illegal_2();                                     } /* DB   ED          */
        void ed_d2() { illegal_2();                                     } /* DB   ED          */
        void ed_d3() { illegal_2();                                     } /* DB   ED          */
        void ed_d4() { illegal_2();                                     } /* DB   ED          */
        void ed_d5() { illegal_2();                                     } /* DB   ED          */
        void ed_d6() { illegal_2();                                     } /* DB   ED          */
        void ed_d7() { illegal_2();                                     } /* DB   ED          */

        void ed_d8() { illegal_2();                                     } /* DB   ED          */
        void ed_d9() { illegal_2();                                     } /* DB   ED          */
        void ed_da() { illegal_2();                                     } /* DB   ED          */
        void ed_db() { illegal_2();                                     } /* DB   ED          */
        void ed_dc() { illegal_2();                                     } /* DB   ED          */
        void ed_dd() { illegal_2();                                     } /* DB   ED          */
        void ed_de() { illegal_2();                                     } /* DB   ED          */
        void ed_df() { illegal_2();                                     } /* DB   ED          */

        void ed_e0() { illegal_2();                                     } /* DB   ED          */
        void ed_e1() { illegal_2();                                     } /* DB   ED          */
        void ed_e2() { illegal_2();                                     } /* DB   ED          */
        void ed_e3() { illegal_2();                                     } /* DB   ED          */
        void ed_e4() { illegal_2();                                     } /* DB   ED          */
        void ed_e5() { illegal_2();                                     } /* DB   ED          */
        void ed_e6() { illegal_2();                                     } /* DB   ED          */
        void ed_e7() { illegal_2();                                     } /* DB   ED          */

        void ed_e8() { illegal_2();                                     } /* DB   ED          */
        void ed_e9() { illegal_2();                                     } /* DB   ED          */
        void ed_ea() { illegal_2();                                     } /* DB   ED          */
        void ed_eb() { illegal_2();                                     } /* DB   ED          */
        void ed_ec() { illegal_2();                                     } /* DB   ED          */
        void ed_ed() { illegal_2();                                     } /* DB   ED          */
        void ed_ee() { illegal_2();                                     } /* DB   ED          */
        void ed_ef() { illegal_2();                                     } /* DB   ED          */

        void ed_f0() { illegal_2();                                     } /* DB   ED          */
        void ed_f1() { illegal_2();                                     } /* DB   ED          */
        void ed_f2() { illegal_2();                                     } /* DB   ED          */
        void ed_f3() { illegal_2();                                     } /* DB   ED          */
        void ed_f4() { illegal_2();                                     } /* DB   ED          */
        void ed_f5() { illegal_2();                                     } /* DB   ED          */
        void ed_f6() { illegal_2();                                     } /* DB   ED          */
        void ed_f7() { illegal_2();                                     } /* DB   ED          */

        void ed_f8() { illegal_2();                                     } /* DB   ED          */
        void ed_f9() { illegal_2();                                     } /* DB   ED          */
        void ed_fa() { illegal_2();                                     } /* DB   ED          */
        void ed_fb() { illegal_2();                                     } /* DB   ED          */
        void ed_fc() { illegal_2();                                     } /* DB   ED          */
        void ed_fd() { illegal_2();                                     } /* DB   ED          */
        void ed_fe() { illegal_2();                                     } /* DB   ED          */
        void ed_ff() { illegal_2();                                     } /* DB   ED          */


        //PROTOTYPES(fd);
        /**********************************************************
         * IY register related opcodes (FD prefix)
         **********************************************************/
        void fd_00() { illegal_1(); op_00();                            } /* DB   FD          */
        void fd_01() { illegal_1(); op_01();                            } /* DB   FD          */
        void fd_02() { illegal_1(); op_02();                            } /* DB   FD          */
        void fd_03() { illegal_1(); op_03();                            } /* DB   FD          */
        void fd_04() { illegal_1(); op_04();                            } /* DB   FD          */
        void fd_05() { illegal_1(); op_05();                            } /* DB   FD          */
        void fd_06() { illegal_1(); op_06();                            } /* DB   FD          */
        void fd_07() { illegal_1(); op_07();                            } /* DB   FD          */

        void fd_08() { illegal_1(); op_08();                            } /* DB   FD          */
        void fd_09() { add16(ref m_iy, m_bc);                               } /* ADD  IY,BC       */
        void fd_0a() { illegal_1(); op_0a();                            } /* DB   FD          */
        void fd_0b() { illegal_1(); op_0b();                            } /* DB   FD          */
        void fd_0c() { illegal_1(); op_0c();                            } /* DB   FD          */
        void fd_0d() { illegal_1(); op_0d();                            } /* DB   FD          */
        void fd_0e() { illegal_1(); op_0e();                            } /* DB   FD          */
        void fd_0f() { illegal_1(); op_0f();                            } /* DB   FD          */

        void fd_10() { illegal_1(); op_10();                            } /* DB   FD          */
        void fd_11() { illegal_1(); op_11();                            } /* DB   FD          */
        void fd_12() { illegal_1(); op_12();                            } /* DB   FD          */
        void fd_13() { illegal_1(); op_13();                            } /* DB   FD          */
        void fd_14() { illegal_1(); op_14();                            } /* DB   FD          */
        void fd_15() { illegal_1(); op_15();                            } /* DB   FD          */
        void fd_16() { illegal_1(); op_16();                            } /* DB   FD          */
        void fd_17() { illegal_1(); op_17();                            } /* DB   FD          */

        void fd_18() { illegal_1(); op_18();                            } /* DB   FD          */
        void fd_19() { add16(ref m_iy, m_de);                               } /* ADD  IY,DE       */
        void fd_1a() { illegal_1(); op_1a();                            } /* DB   FD          */
        void fd_1b() { illegal_1(); op_1b();                            } /* DB   FD          */
        void fd_1c() { illegal_1(); op_1c();                            } /* DB   FD          */
        void fd_1d() { illegal_1(); op_1d();                            } /* DB   FD          */
        void fd_1e() { illegal_1(); op_1e();                            } /* DB   FD          */
        void fd_1f() { illegal_1(); op_1f();                            } /* DB   FD          */

        void fd_20() { illegal_1(); op_20();                            } /* DB   FD          */
        void fd_21() { IY = arg16();                                    } /* LD   IY,w        */
        void fd_22() { m_ea = arg16(); wm16((UInt16)m_ea, m_iy); WZ = (UInt16)(m_ea + 1); } /* LD   (w),IY      */
        void fd_23() { IY++;                                            } /* INC  IY          */
        void fd_24() { HY = inc(HY);                                    } /* INC  HY          */
        void fd_25() { HY = dec(HY);                                    } /* DEC  HY          */
        void fd_26() { HY = arg();                                      } /* LD   HY,n        */
        void fd_27() { illegal_1(); op_27();                            } /* DB   FD          */

        void fd_28() { illegal_1(); op_28();                            } /* DB   FD          */
        void fd_29() { add16(ref m_iy, m_iy);                               } /* ADD  IY,IY       */
        void fd_2a() { m_ea = arg16(); rm16((UInt16)m_ea, ref m_iy); WZ = (UInt16)(m_ea + 1); } /* LD   IY,(w)      */
        void fd_2b() { IY--;                                            } /* DEC  IY          */
        void fd_2c() { LY = inc(LY);                                    } /* INC  LY          */
        void fd_2d() { LY = dec(LY);                                    } /* DEC  LY          */
        void fd_2e() { LY = arg();                                      } /* LD   LY,n        */
        void fd_2f() { illegal_1(); op_2f();                            } /* DB   FD          */

        void fd_30() { illegal_1(); op_30();                            } /* DB   FD          */
        void fd_31() { illegal_1(); op_31();                            } /* DB   FD          */
        void fd_32() { illegal_1(); op_32();                            } /* DB   FD          */
        void fd_33() { illegal_1(); op_33();                            } /* DB   FD          */
        void fd_34() { eay(); wm((UInt16)m_ea, inc(rm((UInt16)m_ea)));                  } /* INC  (IY+o)      */
        void fd_35() { eay(); wm((UInt16)m_ea, dec(rm((UInt16)m_ea)));                  } /* DEC  (IY+o)      */
        void fd_36() { eay(); wm((UInt16)m_ea, arg());                          } /* LD   (IY+o),n    */
        void fd_37() { illegal_1(); op_37();                            } /* DB   FD          */

        void fd_38() { illegal_1(); op_38();                            } /* DB   FD          */
        void fd_39() { add16(ref m_iy, m_sp);                               } /* ADD  IY,SP       */
        void fd_3a() { illegal_1(); op_3a();                            } /* DB   FD          */
        void fd_3b() { illegal_1(); op_3b();                            } /* DB   FD          */
        void fd_3c() { illegal_1(); op_3c();                            } /* DB   FD          */
        void fd_3d() { illegal_1(); op_3d();                            } /* DB   FD          */
        void fd_3e() { illegal_1(); op_3e();                            } /* DB   FD          */
        void fd_3f() { illegal_1(); op_3f();                            } /* DB   FD          */

        void fd_40() { illegal_1(); op_40();                            } /* DB   FD          */
        void fd_41() { illegal_1(); op_41();                            } /* DB   FD          */
        void fd_42() { illegal_1(); op_42();                            } /* DB   FD          */
        void fd_43() { illegal_1(); op_43();                            } /* DB   FD          */
        void fd_44() { B = HY;                                          } /* LD   B,HY        */
        void fd_45() { B = LY;                                          } /* LD   B,LY        */
        void fd_46() { eay(); B = rm((UInt16)m_ea);                             } /* LD   B,(IY+o)    */
        void fd_47() { illegal_1(); op_47();                            } /* DB   FD          */

        void fd_48() { illegal_1(); op_48();                            } /* DB   FD          */
        void fd_49() { illegal_1(); op_49();                            } /* DB   FD          */
        void fd_4a() { illegal_1(); op_4a();                            } /* DB   FD          */
        void fd_4b() { illegal_1(); op_4b();                            } /* DB   FD          */
        void fd_4c() { C = HY;                                          } /* LD   C,HY        */
        void fd_4d() { C = LY;                                          } /* LD   C,LY        */
        void fd_4e() { eay(); C = rm((UInt16)m_ea);                             } /* LD   C,(IY+o)    */
        void fd_4f() { illegal_1(); op_4f();                            } /* DB   FD          */

        void fd_50() { illegal_1(); op_50();                            } /* DB   FD          */
        void fd_51() { illegal_1(); op_51();                            } /* DB   FD          */
        void fd_52() { illegal_1(); op_52();                            } /* DB   FD          */
        void fd_53() { illegal_1(); op_53();                            } /* DB   FD          */
        void fd_54() { D = HY;                                          } /* LD   D,HY        */
        void fd_55() { D = LY;                                          } /* LD   D,LY        */
        void fd_56() { eay(); D = rm((UInt16)m_ea);                             } /* LD   D,(IY+o)    */
        void fd_57() { illegal_1(); op_57();                            } /* DB   FD          */

        void fd_58() { illegal_1(); op_58();                            } /* DB   FD          */
        void fd_59() { illegal_1(); op_59();                            } /* DB   FD          */
        void fd_5a() { illegal_1(); op_5a();                            } /* DB   FD          */
        void fd_5b() { illegal_1(); op_5b();                            } /* DB   FD          */
        void fd_5c() { E = HY;                                          } /* LD   E,HY        */
        void fd_5d() { E = LY;                                          } /* LD   E,LY        */
        void fd_5e() { eay(); E = rm((UInt16)m_ea);                             } /* LD   E,(IY+o)    */
        void fd_5f() { illegal_1(); op_5f();                            } /* DB   FD          */

        void fd_60() { HY = B;                                          } /* LD   HY,B        */
        void fd_61() { HY = C;                                          } /* LD   HY,C        */
        void fd_62() { HY = D;                                          } /* LD   HY,D        */
        void fd_63() { HY = E;                                          } /* LD   HY,E        */
        void fd_64() {                                                  } /* LD   HY,HY       */
        void fd_65() { HY = LY;                                         } /* LD   HY,LY       */
        void fd_66() { eay(); H = rm((UInt16)m_ea);                             } /* LD   H,(IY+o)    */
        void fd_67() { HY = A;                                          } /* LD   HY,A        */

        void fd_68() { LY = B;                                          } /* LD   LY,B        */
        void fd_69() { LY = C;                                          } /* LD   LY,C        */
        void fd_6a() { LY = D;                                          } /* LD   LY,D        */
        void fd_6b() { LY = E;                                          } /* LD   LY,E        */
        void fd_6c() { LY = HY;                                         } /* LD   LY,HY       */
        void fd_6d() {                                                  } /* LD   LY,LY       */
        void fd_6e() { eay(); L = rm((UInt16)m_ea);                             } /* LD   L,(IY+o)    */
        void fd_6f() { LY = A;                                          } /* LD   LY,A        */

        void fd_70() { eay(); wm((UInt16)m_ea, B);                              } /* LD   (IY+o),B    */
        void fd_71() { eay(); wm((UInt16)m_ea, C);                              } /* LD   (IY+o),C    */
        void fd_72() { eay(); wm((UInt16)m_ea, D);                              } /* LD   (IY+o),D    */
        void fd_73() { eay(); wm((UInt16)m_ea, E);                              } /* LD   (IY+o),E    */
        void fd_74() { eay(); wm((UInt16)m_ea, H);                              } /* LD   (IY+o),H    */
        void fd_75() { eay(); wm((UInt16)m_ea, L);                              } /* LD   (IY+o),L    */
        void fd_76() { illegal_1(); op_76();                            } /* DB   FD          */
        void fd_77() { eay(); wm((UInt16)m_ea, A);                              } /* LD   (IY+o),A    */

        void fd_78() { illegal_1(); op_78();                            } /* DB   FD          */
        void fd_79() { illegal_1(); op_79();                            } /* DB   FD          */
        void fd_7a() { illegal_1(); op_7a();                            } /* DB   FD          */
        void fd_7b() { illegal_1(); op_7b();                            } /* DB   FD          */
        void fd_7c() { A = HY;                                          } /* LD   A,HY        */
        void fd_7d() { A = LY;                                          } /* LD   A,LY        */
        void fd_7e() { eay(); A = rm((UInt16)m_ea);                             } /* LD   A,(IY+o)    */
        void fd_7f() { illegal_1(); op_7f();                            } /* DB   FD          */

        void fd_80() { illegal_1(); op_80();                            } /* DB   FD          */
        void fd_81() { illegal_1(); op_81();                            } /* DB   FD          */
        void fd_82() { illegal_1(); op_82();                            } /* DB   FD          */
        void fd_83() { illegal_1(); op_83();                            } /* DB   FD          */
        void fd_84() { add_a(HY);                                       } /* ADD  A,HY        */
        void fd_85() { add_a(LY);                                       } /* ADD  A,LY        */
        void fd_86() { eay(); add_a(rm((UInt16)m_ea));                          } /* ADD  A,(IY+o)    */
        void fd_87() { illegal_1(); op_87();                            } /* DB   FD          */

        void fd_88() { illegal_1(); op_88();                            } /* DB   FD          */
        void fd_89() { illegal_1(); op_89();                            } /* DB   FD          */
        void fd_8a() { illegal_1(); op_8a();                            } /* DB   FD          */
        void fd_8b() { illegal_1(); op_8b();                            } /* DB   FD          */
        void fd_8c() { adc_a(HY);                                       } /* ADC  A,HY        */
        void fd_8d() { adc_a(LY);                                       } /* ADC  A,LY        */
        void fd_8e() { eay(); adc_a(rm((UInt16)m_ea));                          } /* ADC  A,(IY+o)    */
        void fd_8f() { illegal_1(); op_8f();                            } /* DB   FD          */

        void fd_90() { illegal_1(); op_90();                            } /* DB   FD          */
        void fd_91() { illegal_1(); op_91();                            } /* DB   FD          */
        void fd_92() { illegal_1(); op_92();                            } /* DB   FD          */
        void fd_93() { illegal_1(); op_93();                            } /* DB   FD          */
        void fd_94() { sub(HY);                                         } /* SUB  HY          */
        void fd_95() { sub(LY);                                         } /* SUB  LY          */
        void fd_96() { eay(); sub(rm((UInt16)m_ea));                            } /* SUB  (IY+o)      */
        void fd_97() { illegal_1(); op_97();                            } /* DB   FD          */

        void fd_98() { illegal_1(); op_98();                            } /* DB   FD          */
        void fd_99() { illegal_1(); op_99();                            } /* DB   FD          */
        void fd_9a() { illegal_1(); op_9a();                            } /* DB   FD          */
        void fd_9b() { illegal_1(); op_9b();                            } /* DB   FD          */
        void fd_9c() { sbc_a(HY);                                       } /* SBC  A,HY        */
        void fd_9d() { sbc_a(LY);                                       } /* SBC  A,LY        */
        void fd_9e() { eay(); sbc_a(rm((UInt16)m_ea));                          } /* SBC  A,(IY+o)    */
        void fd_9f() { illegal_1(); op_9f();                            } /* DB   FD          */

        void fd_a0() { illegal_1(); op_a0();                            } /* DB   FD          */
        void fd_a1() { illegal_1(); op_a1();                            } /* DB   FD          */
        void fd_a2() { illegal_1(); op_a2();                            } /* DB   FD          */
        void fd_a3() { illegal_1(); op_a3();                            } /* DB   FD          */
        void fd_a4() { and_a(HY);                                       } /* AND  HY          */
        void fd_a5() { and_a(LY);                                       } /* AND  LY          */
        void fd_a6() { eay(); and_a(rm((UInt16)m_ea));                          } /* AND  (IY+o)      */
        void fd_a7() { illegal_1(); op_a7();                            } /* DB   FD          */

        void fd_a8() { illegal_1(); op_a8();                            } /* DB   FD          */
        void fd_a9() { illegal_1(); op_a9();                            } /* DB   FD          */
        void fd_aa() { illegal_1(); op_aa();                            } /* DB   FD          */
        void fd_ab() { illegal_1(); op_ab();                            } /* DB   FD          */
        void fd_ac() { xor_a(HY);                                       } /* XOR  HY          */
        void fd_ad() { xor_a(LY);                                       } /* XOR  LY          */
        void fd_ae() { eay(); xor_a(rm((UInt16)m_ea));                          } /* XOR  (IY+o)      */
        void fd_af() { illegal_1(); op_af();                            } /* DB   FD          */

        void fd_b0() { illegal_1(); op_b0();                            } /* DB   FD          */
        void fd_b1() { illegal_1(); op_b1();                            } /* DB   FD          */
        void fd_b2() { illegal_1(); op_b2();                            } /* DB   FD          */
        void fd_b3() { illegal_1(); op_b3();                            } /* DB   FD          */
        void fd_b4() { or_a(HY);                                        } /* OR   HY          */
        void fd_b5() { or_a(LY);                                        } /* OR   LY          */
        void fd_b6() { eay(); or_a(rm((UInt16)m_ea));                           } /* OR   (IY+o)      */
        void fd_b7() { illegal_1(); op_b7();                            } /* DB   FD          */

        void fd_b8() { illegal_1(); op_b8();                            } /* DB   FD          */
        void fd_b9() { illegal_1(); op_b9();                            } /* DB   FD          */
        void fd_ba() { illegal_1(); op_ba();                            } /* DB   FD          */
        void fd_bb() { illegal_1(); op_bb();                            } /* DB   FD          */
        void fd_bc() { cp(HY);                                          } /* CP   HY          */
        void fd_bd() { cp(LY);                                          } /* CP   LY          */
        void fd_be() { eay(); cp(rm((UInt16)m_ea));                             } /* CP   (IY+o)      */
        void fd_bf() { illegal_1(); op_bf();                            } /* DB   FD          */

        void fd_c0() { illegal_1(); op_c0();                            } /* DB   FD          */
        void fd_c1() { illegal_1(); op_c1();                            } /* DB   FD          */
        void fd_c2() { illegal_1(); op_c2();                            } /* DB   FD          */
        void fd_c3() { illegal_1(); op_c3();                            } /* DB   FD          */
        void fd_c4() { illegal_1(); op_c4();                            } /* DB   FD          */
        void fd_c5() { illegal_1(); op_c5();                            } /* DB   FD          */
        void fd_c6() { illegal_1(); op_c6();                            } /* DB   FD          */
        void fd_c7() { illegal_1(); op_c7();                            } /* DB   FD          */

        void fd_c8() { illegal_1(); op_c8();                            } /* DB   FD          */
        void fd_c9() { illegal_1(); op_c9();                            } /* DB   FD          */
        void fd_ca() { illegal_1(); op_ca();                            } /* DB   FD          */
        void fd_cb() { eay(); EXEC_xycb(arg());                         } /* **   FD CB xx    */
        void fd_cc() { illegal_1(); op_cc();                            } /* DB   FD          */
        void fd_cd() { illegal_1(); op_cd();                            } /* DB   FD          */
        void fd_ce() { illegal_1(); op_ce();                            } /* DB   FD          */
        void fd_cf() { illegal_1(); op_cf();                            } /* DB   FD          */

        void fd_d0() { illegal_1(); op_d0();                            } /* DB   FD          */
        void fd_d1() { illegal_1(); op_d1();                            } /* DB   FD          */
        void fd_d2() { illegal_1(); op_d2();                            } /* DB   FD          */
        void fd_d3() { illegal_1(); op_d3();                            } /* DB   FD          */
        void fd_d4() { illegal_1(); op_d4();                            } /* DB   FD          */
        void fd_d5() { illegal_1(); op_d5();                            } /* DB   FD          */
        void fd_d6() { illegal_1(); op_d6();                            } /* DB   FD          */
        void fd_d7() { illegal_1(); op_d7();                            } /* DB   FD          */

        void fd_d8() { illegal_1(); op_d8();                            } /* DB   FD          */
        void fd_d9() { illegal_1(); op_d9();                            } /* DB   FD          */
        void fd_da() { illegal_1(); op_da();                            } /* DB   FD          */
        void fd_db() { illegal_1(); op_db();                            } /* DB   FD          */
        void fd_dc() { illegal_1(); op_dc();                            } /* DB   FD          */
        void fd_dd() { illegal_1(); op_dd();                            } /* DB   FD          */
        void fd_de() { illegal_1(); op_de();                            } /* DB   FD          */
        void fd_df() { illegal_1(); op_df();                            } /* DB   FD          */

        void fd_e0() { illegal_1(); op_e0();                            } /* DB   FD          */
        void fd_e1() { pop(ref m_iy);                                       } /* POP  IY          */
        void fd_e2() { illegal_1(); op_e2();                            } /* DB   FD          */
        void fd_e3() { ex_sp(ref m_iy);                                     } /* EX   (SP),IY     */
        void fd_e4() { illegal_1(); op_e4();                            } /* DB   FD          */
        void fd_e5() { push(m_iy);                                      } /* PUSH IY          */
        void fd_e6() { illegal_1(); op_e6();                            } /* DB   FD          */
        void fd_e7() { illegal_1(); op_e7();                            } /* DB   FD          */

        void fd_e8() { illegal_1(); op_e8();                            } /* DB   FD          */
        void fd_e9() { PC = IY;                                         } /* JP   (IY)        */
        void fd_ea() { illegal_1(); op_ea();                            } /* DB   FD          */
        void fd_eb() { illegal_1(); op_eb();                            } /* DB   FD          */
        void fd_ec() { illegal_1(); op_ec();                            } /* DB   FD          */
        void fd_ed() { illegal_1(); op_ed();                            } /* DB   FD          */
        void fd_ee() { illegal_1(); op_ee();                            } /* DB   FD          */
        void fd_ef() { illegal_1(); op_ef();                            } /* DB   FD          */

        void fd_f0() { illegal_1(); op_f0();                            } /* DB   FD          */
        void fd_f1() { illegal_1(); op_f1();                            } /* DB   FD          */
        void fd_f2() { illegal_1(); op_f2();                            } /* DB   FD          */
        void fd_f3() { illegal_1(); op_f3();                            } /* DB   FD          */
        void fd_f4() { illegal_1(); op_f4();                            } /* DB   FD          */
        void fd_f5() { illegal_1(); op_f5();                            } /* DB   FD          */
        void fd_f6() { illegal_1(); op_f6();                            } /* DB   FD          */
        void fd_f7() { illegal_1(); op_f7();                            } /* DB   FD          */

        void fd_f8() { illegal_1(); op_f8();                            } /* DB   FD          */
        void fd_f9() { SP = IY;                                         } /* LD   SP,IY       */
        void fd_fa() { illegal_1(); op_fa();                            } /* DB   FD          */
        void fd_fb() { illegal_1(); op_fb();                            } /* DB   FD          */
        void fd_fc() { illegal_1(); op_fc();                            } /* DB   FD          */
        void fd_fd() { illegal_1(); op_fd();                            } /* DB   FD          */
        void fd_fe() { illegal_1(); op_fe();                            } /* DB   FD          */
        void fd_ff() { illegal_1(); op_ff();                            } /* DB   FD          */


        //PROTOTYPES(xycb);
        /**********************************************************
        * opcodes with DD/FD CB prefix
        * rotate, shift and bit operations with (IX+o)
        **********************************************************/
        void xycb_00() { B = rlc(rm((UInt16)m_ea)); wm((UInt16)m_ea, B);    } /* RLC  B=(XY+o)    */
        void xycb_01() { C = rlc(rm((UInt16)m_ea)); wm((UInt16)m_ea, C);    } /* RLC  C=(XY+o)    */
        void xycb_02() { D = rlc(rm((UInt16)m_ea)); wm((UInt16)m_ea, D);    } /* RLC  D=(XY+o)    */
        void xycb_03() { E = rlc(rm((UInt16)m_ea)); wm((UInt16)m_ea, E);    } /* RLC  E=(XY+o)    */
        void xycb_04() { H = rlc(rm((UInt16)m_ea)); wm((UInt16)m_ea, H);    } /* RLC  H=(XY+o)    */
        void xycb_05() { L = rlc(rm((UInt16)m_ea)); wm((UInt16)m_ea, L);    } /* RLC  L=(XY+o)    */
        void xycb_06() { wm((UInt16)m_ea, rlc(rm((UInt16)m_ea)));           } /* RLC  (XY+o)      */
        void xycb_07() { A = rlc(rm((UInt16)m_ea)); wm((UInt16)m_ea, A);    } /* RLC  A=(XY+o)    */

        void xycb_08() { B = rrc(rm((UInt16)m_ea)); wm((UInt16)m_ea, B);    } /* RRC  B=(XY+o)    */
        void xycb_09() { C = rrc(rm((UInt16)m_ea)); wm((UInt16)m_ea, C);    } /* RRC  C=(XY+o)    */
        void xycb_0a() { D = rrc(rm((UInt16)m_ea)); wm((UInt16)m_ea, D);    } /* RRC  D=(XY+o)    */
        void xycb_0b() { E = rrc(rm((UInt16)m_ea)); wm((UInt16)m_ea, E);    } /* RRC  E=(XY+o)    */
        void xycb_0c() { H = rrc(rm((UInt16)m_ea)); wm((UInt16)m_ea, H);    } /* RRC  H=(XY+o)    */
        void xycb_0d() { L = rrc(rm((UInt16)m_ea)); wm((UInt16)m_ea, L);    } /* RRC  L=(XY+o)    */
        void xycb_0e() { wm((UInt16)m_ea,rrc(rm((UInt16)m_ea)));            } /* RRC  (XY+o)      */
        void xycb_0f() { A = rrc(rm((UInt16)m_ea)); wm((UInt16)m_ea, A);    } /* RRC  A=(XY+o)    */

        void xycb_10() { B = rl(rm((UInt16)m_ea)); wm((UInt16)m_ea, B);     } /* RL   B=(XY+o)    */
        void xycb_11() { C = rl(rm((UInt16)m_ea)); wm((UInt16)m_ea, C);     } /* RL   C=(XY+o)    */
        void xycb_12() { D = rl(rm((UInt16)m_ea)); wm((UInt16)m_ea, D);     } /* RL   D=(XY+o)    */
        void xycb_13() { E = rl(rm((UInt16)m_ea)); wm((UInt16)m_ea, E);     } /* RL   E=(XY+o)    */
        void xycb_14() { H = rl(rm((UInt16)m_ea)); wm((UInt16)m_ea, H);     } /* RL   H=(XY+o)    */
        void xycb_15() { L = rl(rm((UInt16)m_ea)); wm((UInt16)m_ea, L);     } /* RL   L=(XY+o)    */
        void xycb_16() { wm((UInt16)m_ea,rl(rm((UInt16)m_ea)));             } /* RL   (XY+o)      */
        void xycb_17() { A = rl(rm((UInt16)m_ea)); wm((UInt16)m_ea, A);     } /* RL   A=(XY+o)    */

        void xycb_18() { B = rr(rm((UInt16)m_ea)); wm((UInt16)m_ea, B);     } /* RR   B=(XY+o)    */
        void xycb_19() { C = rr(rm((UInt16)m_ea)); wm((UInt16)m_ea, C);     } /* RR   C=(XY+o)    */
        void xycb_1a() { D = rr(rm((UInt16)m_ea)); wm((UInt16)m_ea, D);     } /* RR   D=(XY+o)    */
        void xycb_1b() { E = rr(rm((UInt16)m_ea)); wm((UInt16)m_ea, E);     } /* RR   E=(XY+o)    */
        void xycb_1c() { H = rr(rm((UInt16)m_ea)); wm((UInt16)m_ea, H);     } /* RR   H=(XY+o)    */
        void xycb_1d() { L = rr(rm((UInt16)m_ea)); wm((UInt16)m_ea, L);     } /* RR   L=(XY+o)    */
        void xycb_1e() { wm((UInt16)m_ea, rr(rm((UInt16)m_ea)));            } /* RR   (XY+o)      */
        void xycb_1f() { A = rr(rm((UInt16)m_ea)); wm((UInt16)m_ea, A);     } /* RR   A=(XY+o)    */

        void xycb_20() { B = sla(rm((UInt16)m_ea)); wm((UInt16)m_ea, B);    } /* SLA  B=(XY+o)    */
        void xycb_21() { C = sla(rm((UInt16)m_ea)); wm((UInt16)m_ea, C);    } /* SLA  C=(XY+o)    */
        void xycb_22() { D = sla(rm((UInt16)m_ea)); wm((UInt16)m_ea, D);    } /* SLA  D=(XY+o)    */
        void xycb_23() { E = sla(rm((UInt16)m_ea)); wm((UInt16)m_ea, E);    } /* SLA  E=(XY+o)    */
        void xycb_24() { H = sla(rm((UInt16)m_ea)); wm((UInt16)m_ea, H);    } /* SLA  H=(XY+o)    */
        void xycb_25() { L = sla(rm((UInt16)m_ea)); wm((UInt16)m_ea, L);    } /* SLA  L=(XY+o)    */
        void xycb_26() { wm((UInt16)m_ea, sla(rm((UInt16)m_ea)));           } /* SLA  (XY+o)      */
        void xycb_27() { A = sla(rm((UInt16)m_ea)); wm((UInt16)m_ea, A);    } /* SLA  A=(XY+o)    */

        void xycb_28() { B = sra(rm((UInt16)m_ea)); wm((UInt16)m_ea, B);    } /* SRA  B=(XY+o)    */
        void xycb_29() { C = sra(rm((UInt16)m_ea)); wm((UInt16)m_ea, C);    } /* SRA  C=(XY+o)    */
        void xycb_2a() { D = sra(rm((UInt16)m_ea)); wm((UInt16)m_ea, D);    } /* SRA  D=(XY+o)    */
        void xycb_2b() { E = sra(rm((UInt16)m_ea)); wm((UInt16)m_ea, E);    } /* SRA  E=(XY+o)    */
        void xycb_2c() { H = sra(rm((UInt16)m_ea)); wm((UInt16)m_ea, H);    } /* SRA  H=(XY+o)    */
        void xycb_2d() { L = sra(rm((UInt16)m_ea)); wm((UInt16)m_ea, L);    } /* SRA  L=(XY+o)    */
        void xycb_2e() { wm((UInt16)m_ea, sra(rm((UInt16)m_ea)));           } /* SRA  (XY+o)      */
        void xycb_2f() { A = sra(rm((UInt16)m_ea)); wm((UInt16)m_ea, A);    } /* SRA  A=(XY+o)    */

        void xycb_30() { B = sll(rm((UInt16)m_ea)); wm((UInt16)m_ea, B);    } /* SLL  B=(XY+o)    */
        void xycb_31() { C = sll(rm((UInt16)m_ea)); wm((UInt16)m_ea, C);    } /* SLL  C=(XY+o)    */
        void xycb_32() { D = sll(rm((UInt16)m_ea)); wm((UInt16)m_ea, D);    } /* SLL  D=(XY+o)    */
        void xycb_33() { E = sll(rm((UInt16)m_ea)); wm((UInt16)m_ea, E);    } /* SLL  E=(XY+o)    */
        void xycb_34() { H = sll(rm((UInt16)m_ea)); wm((UInt16)m_ea, H);    } /* SLL  H=(XY+o)    */
        void xycb_35() { L = sll(rm((UInt16)m_ea)); wm((UInt16)m_ea, L);    } /* SLL  L=(XY+o)    */
        void xycb_36() { wm((UInt16)m_ea, sll(rm((UInt16)m_ea)));           } /* SLL  (XY+o)      */
        void xycb_37() { A = sll(rm((UInt16)m_ea)); wm((UInt16)m_ea, A);    } /* SLL  A=(XY+o)    */

        void xycb_38() { B = srl(rm((UInt16)m_ea)); wm((UInt16)m_ea, B);    } /* SRL  B=(XY+o)    */
        void xycb_39() { C = srl(rm((UInt16)m_ea)); wm((UInt16)m_ea, C);    } /* SRL  C=(XY+o)    */
        void xycb_3a() { D = srl(rm((UInt16)m_ea)); wm((UInt16)m_ea, D);    } /* SRL  D=(XY+o)    */
        void xycb_3b() { E = srl(rm((UInt16)m_ea)); wm((UInt16)m_ea, E);    } /* SRL  E=(XY+o)    */
        void xycb_3c() { H = srl(rm((UInt16)m_ea)); wm((UInt16)m_ea, H);    } /* SRL  H=(XY+o)    */
        void xycb_3d() { L = srl(rm((UInt16)m_ea)); wm((UInt16)m_ea, L);    } /* SRL  L=(XY+o)    */
        void xycb_3e() { wm((UInt16)m_ea, srl(rm((UInt16)m_ea)));           } /* SRL  (XY+o)      */
        void xycb_3f() { A = srl(rm((UInt16)m_ea)); wm((UInt16)m_ea, A);    } /* SRL  A=(XY+o)    */

        void xycb_40() { xycb_46();                         } /* BIT  0,(XY+o)    */
        void xycb_41() { xycb_46();                         } /* BIT  0,(XY+o)    */
        void xycb_42() { xycb_46();                         } /* BIT  0,(XY+o)    */
        void xycb_43() { xycb_46();                         } /* BIT  0,(XY+o)    */
        void xycb_44() { xycb_46();                         } /* BIT  0,(XY+o)    */
        void xycb_45() { xycb_46();                         } /* BIT  0,(XY+o)    */
        void xycb_46() { bit_xy(0, rm((UInt16)m_ea));               } /* BIT  0,(XY+o)    */
        void xycb_47() { xycb_46();                         } /* BIT  0,(XY+o)    */

        void xycb_48() { xycb_4e();                         } /* BIT  1,(XY+o)    */
        void xycb_49() { xycb_4e();                         } /* BIT  1,(XY+o)    */
        void xycb_4a() { xycb_4e();                         } /* BIT  1,(XY+o)    */
        void xycb_4b() { xycb_4e();                         } /* BIT  1,(XY+o)    */
        void xycb_4c() { xycb_4e();                         } /* BIT  1,(XY+o)    */
        void xycb_4d() { xycb_4e();                         } /* BIT  1,(XY+o)    */
        void xycb_4e() { bit_xy(1, rm((UInt16)m_ea));               } /* BIT  1,(XY+o)    */
        void xycb_4f() { xycb_4e();                         } /* BIT  1,(XY+o)    */

        void xycb_50() { xycb_56();                         } /* BIT  2,(XY+o)    */
        void xycb_51() { xycb_56();                         } /* BIT  2,(XY+o)    */
        void xycb_52() { xycb_56();                         } /* BIT  2,(XY+o)    */
        void xycb_53() { xycb_56();                         } /* BIT  2,(XY+o)    */
        void xycb_54() { xycb_56();                         } /* BIT  2,(XY+o)    */
        void xycb_55() { xycb_56();                         } /* BIT  2,(XY+o)    */
        void xycb_56() { bit_xy(2, rm((UInt16)m_ea));               } /* BIT  2,(XY+o)    */
        void xycb_57() { xycb_56();                         } /* BIT  2,(XY+o)    */

        void xycb_58() { xycb_5e();                         } /* BIT  3,(XY+o)    */
        void xycb_59() { xycb_5e();                         } /* BIT  3,(XY+o)    */
        void xycb_5a() { xycb_5e();                         } /* BIT  3,(XY+o)    */
        void xycb_5b() { xycb_5e();                         } /* BIT  3,(XY+o)    */
        void xycb_5c() { xycb_5e();                         } /* BIT  3,(XY+o)    */
        void xycb_5d() { xycb_5e();                         } /* BIT  3,(XY+o)    */
        void xycb_5e() { bit_xy(3, rm((UInt16)m_ea));               } /* BIT  3,(XY+o)    */
        void xycb_5f() { xycb_5e();                         } /* BIT  3,(XY+o)    */

        void xycb_60() { xycb_66();                         } /* BIT  4,(XY+o)    */
        void xycb_61() { xycb_66();                         } /* BIT  4,(XY+o)    */
        void xycb_62() { xycb_66();                         } /* BIT  4,(XY+o)    */
        void xycb_63() { xycb_66();                         } /* BIT  4,(XY+o)    */
        void xycb_64() { xycb_66();                         } /* BIT  4,(XY+o)    */
        void xycb_65() { xycb_66();                         } /* BIT  4,(XY+o)    */
        void xycb_66() { bit_xy(4, rm((UInt16)m_ea));               } /* BIT  4,(XY+o)    */
        void xycb_67() { xycb_66();                         } /* BIT  4,(XY+o)    */

        void xycb_68() { xycb_6e();                         } /* BIT  5,(XY+o)    */
        void xycb_69() { xycb_6e();                         } /* BIT  5,(XY+o)    */
        void xycb_6a() { xycb_6e();                         } /* BIT  5,(XY+o)    */
        void xycb_6b() { xycb_6e();                         } /* BIT  5,(XY+o)    */
        void xycb_6c() { xycb_6e();                         } /* BIT  5,(XY+o)    */
        void xycb_6d() { xycb_6e();                         } /* BIT  5,(XY+o)    */
        void xycb_6e() { bit_xy(5, rm((UInt16)m_ea));               } /* BIT  5,(XY+o)    */
        void xycb_6f() { xycb_6e();                         } /* BIT  5,(XY+o)    */

        void xycb_70() { xycb_76();                         } /* BIT  6,(XY+o)    */
        void xycb_71() { xycb_76();                         } /* BIT  6,(XY+o)    */
        void xycb_72() { xycb_76();                         } /* BIT  6,(XY+o)    */
        void xycb_73() { xycb_76();                         } /* BIT  6,(XY+o)    */
        void xycb_74() { xycb_76();                         } /* BIT  6,(XY+o)    */
        void xycb_75() { xycb_76();                         } /* BIT  6,(XY+o)    */
        void xycb_76() { bit_xy(6, rm((UInt16)m_ea));               } /* BIT  6,(XY+o)    */
        void xycb_77() { xycb_76();                         } /* BIT  6,(XY+o)    */

        void xycb_78() { xycb_7e();                         } /* BIT  7,(XY+o)    */
        void xycb_79() { xycb_7e();                         } /* BIT  7,(XY+o)    */
        void xycb_7a() { xycb_7e();                         } /* BIT  7,(XY+o)    */
        void xycb_7b() { xycb_7e();                         } /* BIT  7,(XY+o)    */
        void xycb_7c() { xycb_7e();                         } /* BIT  7,(XY+o)    */
        void xycb_7d() { xycb_7e();                         } /* BIT  7,(XY+o)    */
        void xycb_7e() { bit_xy(7, rm((UInt16)m_ea));               } /* BIT  7,(XY+o)    */
        void xycb_7f() { xycb_7e();                         } /* BIT  7,(XY+o)    */

        void xycb_80() { B = res(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* RES  0,B=(XY+o)  */
        void xycb_81() { C = res(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* RES  0,C=(XY+o)  */
        void xycb_82() { D = res(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* RES  0,D=(XY+o)  */
        void xycb_83() { E = res(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* RES  0,E=(XY+o)  */
        void xycb_84() { H = res(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* RES  0,H=(XY+o)  */
        void xycb_85() { L = res(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* RES  0,L=(XY+o)  */
        void xycb_86() { wm((UInt16)m_ea, res(0, rm((UInt16)m_ea)));        } /* RES  0,(XY+o)    */
        void xycb_87() { A = res(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* RES  0,A=(XY+o)  */

        void xycb_88() { B = res(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* RES  1,B=(XY+o)  */
        void xycb_89() { C = res(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* RES  1,C=(XY+o)  */
        void xycb_8a() { D = res(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* RES  1,D=(XY+o)  */
        void xycb_8b() { E = res(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* RES  1,E=(XY+o)  */
        void xycb_8c() { H = res(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* RES  1,H=(XY+o)  */
        void xycb_8d() { L = res(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* RES  1,L=(XY+o)  */
        void xycb_8e() { wm((UInt16)m_ea, res(1, rm((UInt16)m_ea)));        } /* RES  1,(XY+o)    */
        void xycb_8f() { A = res(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* RES  1,A=(XY+o)  */

        void xycb_90() { B = res(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* RES  2,B=(XY+o)  */
        void xycb_91() { C = res(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* RES  2,C=(XY+o)  */
        void xycb_92() { D = res(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* RES  2,D=(XY+o)  */
        void xycb_93() { E = res(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* RES  2,E=(XY+o)  */
        void xycb_94() { H = res(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* RES  2,H=(XY+o)  */
        void xycb_95() { L = res(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* RES  2,L=(XY+o)  */
        void xycb_96() { wm((UInt16)m_ea, res(2, rm((UInt16)m_ea)));        } /* RES  2,(XY+o)    */
        void xycb_97() { A = res(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* RES  2,A=(XY+o)  */

        void xycb_98() { B = res(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* RES  3,B=(XY+o)  */
        void xycb_99() { C = res(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* RES  3,C=(XY+o)  */
        void xycb_9a() { D = res(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* RES  3,D=(XY+o)  */
        void xycb_9b() { E = res(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* RES  3,E=(XY+o)  */
        void xycb_9c() { H = res(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* RES  3,H=(XY+o)  */
        void xycb_9d() { L = res(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* RES  3,L=(XY+o)  */
        void xycb_9e() { wm((UInt16)m_ea, res(3, rm((UInt16)m_ea)));        } /* RES  3,(XY+o)    */
        void xycb_9f() { A = res(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* RES  3,A=(XY+o)  */

        void xycb_a0() { B = res(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* RES  4,B=(XY+o)  */
        void xycb_a1() { C = res(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* RES  4,C=(XY+o)  */
        void xycb_a2() { D = res(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* RES  4,D=(XY+o)  */
        void xycb_a3() { E = res(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* RES  4,E=(XY+o)  */
        void xycb_a4() { H = res(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* RES  4,H=(XY+o)  */
        void xycb_a5() { L = res(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* RES  4,L=(XY+o)  */
        void xycb_a6() { wm((UInt16)m_ea, res(4, rm((UInt16)m_ea)));        } /* RES  4,(XY+o)    */
        void xycb_a7() { A = res(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* RES  4,A=(XY+o)  */

        void xycb_a8() { B = res(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* RES  5,B=(XY+o)  */
        void xycb_a9() { C = res(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* RES  5,C=(XY+o)  */
        void xycb_aa() { D = res(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* RES  5,D=(XY+o)  */
        void xycb_ab() { E = res(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* RES  5,E=(XY+o)  */
        void xycb_ac() { H = res(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* RES  5,H=(XY+o)  */
        void xycb_ad() { L = res(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* RES  5,L=(XY+o)  */
        void xycb_ae() { wm((UInt16)m_ea, res(5, rm((UInt16)m_ea)));        } /* RES  5,(XY+o)    */
        void xycb_af() { A = res(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* RES  5,A=(XY+o)  */

        void xycb_b0() { B = res(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* RES  6,B=(XY+o)  */
        void xycb_b1() { C = res(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* RES  6,C=(XY+o)  */
        void xycb_b2() { D = res(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* RES  6,D=(XY+o)  */
        void xycb_b3() { E = res(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* RES  6,E=(XY+o)  */
        void xycb_b4() { H = res(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* RES  6,H=(XY+o)  */
        void xycb_b5() { L = res(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* RES  6,L=(XY+o)  */
        void xycb_b6() { wm((UInt16)m_ea, res(6, rm((UInt16)m_ea)));        } /* RES  6,(XY+o)    */
        void xycb_b7() { A = res(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* RES  6,A=(XY+o)  */

        void xycb_b8() { B = res(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* RES  7,B=(XY+o)  */
        void xycb_b9() { C = res(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* RES  7,C=(XY+o)  */
        void xycb_ba() { D = res(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* RES  7,D=(XY+o)  */
        void xycb_bb() { E = res(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* RES  7,E=(XY+o)  */
        void xycb_bc() { H = res(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* RES  7,H=(XY+o)  */
        void xycb_bd() { L = res(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* RES  7,L=(XY+o)  */
        void xycb_be() { wm((UInt16)m_ea, res(7, rm((UInt16)m_ea)));        } /* RES  7,(XY+o)    */
        void xycb_bf() { A = res(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* RES  7,A=(XY+o)  */

        void xycb_c0() { B = set(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* SET  0,B=(XY+o)  */
        void xycb_c1() { C = set(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* SET  0,C=(XY+o)  */
        void xycb_c2() { D = set(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* SET  0,D=(XY+o)  */
        void xycb_c3() { E = set(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* SET  0,E=(XY+o)  */
        void xycb_c4() { H = set(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* SET  0,H=(XY+o)  */
        void xycb_c5() { L = set(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* SET  0,L=(XY+o)  */
        void xycb_c6() { wm((UInt16)m_ea, set(0, rm((UInt16)m_ea)));        } /* SET  0,(XY+o)    */
        void xycb_c7() { A = set(0, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* SET  0,A=(XY+o)  */

        void xycb_c8() { B = set(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* SET  1,B=(XY+o)  */
        void xycb_c9() { C = set(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* SET  1,C=(XY+o)  */
        void xycb_ca() { D = set(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* SET  1,D=(XY+o)  */
        void xycb_cb() { E = set(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* SET  1,E=(XY+o)  */
        void xycb_cc() { H = set(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* SET  1,H=(XY+o)  */
        void xycb_cd() { L = set(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* SET  1,L=(XY+o)  */
        void xycb_ce() { wm((UInt16)m_ea, set(1, rm((UInt16)m_ea)));        } /* SET  1,(XY+o)    */
        void xycb_cf() { A = set(1, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* SET  1,A=(XY+o)  */

        void xycb_d0() { B = set(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* SET  2,B=(XY+o)  */
        void xycb_d1() { C = set(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* SET  2,C=(XY+o)  */
        void xycb_d2() { D = set(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* SET  2,D=(XY+o)  */
        void xycb_d3() { E = set(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* SET  2,E=(XY+o)  */
        void xycb_d4() { H = set(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* SET  2,H=(XY+o)  */
        void xycb_d5() { L = set(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* SET  2,L=(XY+o)  */
        void xycb_d6() { wm((UInt16)m_ea, set(2, rm((UInt16)m_ea)));        } /* SET  2,(XY+o)    */
        void xycb_d7() { A = set(2, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* SET  2,A=(XY+o)  */

        void xycb_d8() { B = set(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* SET  3,B=(XY+o)  */
        void xycb_d9() { C = set(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* SET  3,C=(XY+o)  */
        void xycb_da() { D = set(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* SET  3,D=(XY+o)  */
        void xycb_db() { E = set(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* SET  3,E=(XY+o)  */
        void xycb_dc() { H = set(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* SET  3,H=(XY+o)  */
        void xycb_dd() { L = set(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* SET  3,L=(XY+o)  */
        void xycb_de() { wm((UInt16)m_ea, set(3, rm((UInt16)m_ea)));        } /* SET  3,(XY+o)    */
        void xycb_df() { A = set(3, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* SET  3,A=(XY+o)  */

        void xycb_e0() { B = set(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* SET  4,B=(XY+o)  */
        void xycb_e1() { C = set(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* SET  4,C=(XY+o)  */
        void xycb_e2() { D = set(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* SET  4,D=(XY+o)  */
        void xycb_e3() { E = set(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* SET  4,E=(XY+o)  */
        void xycb_e4() { H = set(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* SET  4,H=(XY+o)  */
        void xycb_e5() { L = set(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* SET  4,L=(XY+o)  */
        void xycb_e6() { wm((UInt16)m_ea, set(4, rm((UInt16)m_ea)));        } /* SET  4,(XY+o)    */
        void xycb_e7() { A = set(4, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* SET  4,A=(XY+o)  */

        void xycb_e8() { B = set(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* SET  5,B=(XY+o)  */
        void xycb_e9() { C = set(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* SET  5,C=(XY+o)  */
        void xycb_ea() { D = set(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* SET  5,D=(XY+o)  */
        void xycb_eb() { E = set(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* SET  5,E=(XY+o)  */
        void xycb_ec() { H = set(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* SET  5,H=(XY+o)  */
        void xycb_ed() { L = set(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* SET  5,L=(XY+o)  */
        void xycb_ee() { wm((UInt16)m_ea, set(5, rm((UInt16)m_ea)));        } /* SET  5,(XY+o)    */
        void xycb_ef() { A = set(5, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* SET  5,A=(XY+o)  */

        void xycb_f0() { B = set(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* SET  6,B=(XY+o)  */
        void xycb_f1() { C = set(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* SET  6,C=(XY+o)  */
        void xycb_f2() { D = set(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* SET  6,D=(XY+o)  */
        void xycb_f3() { E = set(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* SET  6,E=(XY+o)  */
        void xycb_f4() { H = set(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* SET  6,H=(XY+o)  */
        void xycb_f5() { L = set(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* SET  6,L=(XY+o)  */
        void xycb_f6() { wm((UInt16)m_ea, set(6, rm((UInt16)m_ea)));        } /* SET  6,(XY+o)    */
        void xycb_f7() { A = set(6, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* SET  6,A=(XY+o)  */

        void xycb_f8() { B = set(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, B); } /* SET  7,B=(XY+o)  */
        void xycb_f9() { C = set(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, C); } /* SET  7,C=(XY+o)  */
        void xycb_fa() { D = set(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, D); } /* SET  7,D=(XY+o)  */
        void xycb_fb() { E = set(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, E); } /* SET  7,E=(XY+o)  */
        void xycb_fc() { H = set(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, H); } /* SET  7,H=(XY+o)  */
        void xycb_fd() { L = set(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, L); } /* SET  7,L=(XY+o)  */
        void xycb_fe() { wm((UInt16)m_ea, set(7, rm((UInt16)m_ea)));        } /* SET  7,(XY+o)    */
        void xycb_ff() { A = set(7, rm((UInt16)m_ea)); wm((UInt16)m_ea, A); } /* SET  7,A=(XY+o)  */


        void illegal_1() { logerror("Z80 ill. opcode ${0} ${1} (${2})\n", m_opcodes.read_byte((PCD-1)&0xffff), m_opcodes.read_byte(PCD), PCD-1); }  // $%02x $%02x ($%04x)


        /***************************************************************
         * Enter halt state; write 1 to callback on first execution
         ***************************************************************/
        void halt()
        {
            PC--;
            if (m_halt == 0)
            {
                m_halt = 1;
                m_halt_cb.op(1);
            }
        }

        /***************************************************************
         * Leave halt state; write 0 to callback
         ***************************************************************/
        public void leave_halt() { if ( m_halt > 0 ) { m_halt = 0; m_halt_cb.op(0); PC++; } }

        /***************************************************************
         * Input a byte from given I/O port
         ***************************************************************/
        byte in_(uint16_t port)
        {
            return m_io.read_byte(port);
        }

        /***************************************************************
         * Output a byte to given I/O port
         ***************************************************************/
        void out_(uint16_t port, uint8_t value)
        {
            m_io.write_byte(port, value);
        }

        /***************************************************************
         * Read a byte from given memory location
         ***************************************************************/
        public uint8_t rm(uint16_t addr)
        {
            return m_data.read_byte(addr);
        }

        /***************************************************************
         * Read a word from given memory location
         ***************************************************************/
        void rm16(uint16_t addr, ref PAIR r)
        {
            r.b.l = rm(addr);
            r.b.h = rm((uint16_t)(addr+1));
        }

        /***************************************************************
        * Write a byte to given memory location
        ***************************************************************/
        public void wm(uint16_t addr, uint8_t value)
        {
            m_data.write_byte(addr, value);
        }

        /***************************************************************
         * Write a word to given memory location
         ***************************************************************/
        void wm16(uint16_t addr, PAIR r)
        {
            wm(addr, r.b.l);
            wm((uint16_t)(addr+1), r.b.h);
        }

        /***************************************************************
         * rop() is identical to rm() except it is used for
         * reading opcodes. In case of system with memory mapped I/O,
         * this function can be used to greatly speed up emulation
         ***************************************************************/
        public uint8_t rop()
        {
            UInt32 pc = PCD;
            PC++;
            uint8_t res = m_opcodes.read_byte(pc);
            m_icount.i -= 2;  // m_icount -= 2;
            m_refresh_cb.op((UInt16)((m_i << 8) | (m_r2 & 0x80) | ((m_r-1) & 0x7f)), 0x00, 0xff);
            m_icount.i += 2;  //m_icount += 2;
            return res;
        }

        /****************************************************************
         * arg() is identical to rop() except it is used
         * for reading opcode arguments. This difference can be used to
         * support systems that use different encoding mechanisms for
         * opcodes and opcode arguments
         ***************************************************************/
        uint8_t arg()
        {
            UInt32 pc = PCD;
            PC++;
            return m_args.read_byte(pc);
        }

        uint16_t arg16()
        {
            UInt32 pc = PCD;
            PC += 2;
            return m_args.read_word(pc);
        }

        /***************************************************************
         * Calculate the effective address EA of an opcode using
         * IX+offset resp. IY+offset addressing.
         ***************************************************************/
        void eax()
        {
            m_ea = (UInt32)(UInt16)(IX + (sbyte)arg());
            WZ = (UInt16)m_ea;
        }

        void eay()
        {
            m_ea = (UInt32)(UInt16)(IY + (sbyte)arg());
            WZ = (UInt16)m_ea;
        }

        /***************************************************************
         * POP
         ***************************************************************/
        void pop(ref PAIR r)
        {
            rm16((UInt16)SPD, ref r);
            SP += 2;
        }

        /***************************************************************
         * PUSH
         ***************************************************************/
        public void push(PAIR r)
        {
            SP -= 2;
            wm16((UInt16)SPD, r);
        }

        /***************************************************************
         * JP
         ***************************************************************/
        void jp()
        {
            PCD = arg16();
            WZ = (UInt16)PCD;
        }

        /***************************************************************
         * JP_COND
         ***************************************************************/
        void jp_cond(bool cond)
        {
            if (cond)
            {
                PCD = arg16();
                WZ = (UInt16)PCD;
            }
            else
            {
                WZ = arg16(); /* implicit do PC += 2 */
            }
        }

        /***************************************************************
         * JR
         ***************************************************************/
        void jr()
        {
            sbyte a = (sbyte)arg();    /* arg() also increments PC */
            PC += (UInt16)a;           /* so don't do PC += arg() */
            WZ = PC;
        }

        /***************************************************************
         * JR_COND
         ***************************************************************/
        void jr_cond(bool cond, uint8_t opcode)
        {
            if (cond)
            {
                jr();
                CC_ex(opcode);
            }
            else
            {
                PC++;
            }
        }

        /***************************************************************
         * CALL
         ***************************************************************/
        void call()
        {
            m_ea = arg16();
            WZ = (UInt16)m_ea;
            push(m_pc);
            PCD = m_ea;
        }

        /***************************************************************
         * CALL_COND
         ***************************************************************/
        void call_cond(bool cond, uint8_t opcode)
        {
            if (cond)
            {
                m_ea = arg16();
                WZ = (UInt16)m_ea;
                push(m_pc);
                PCD = m_ea;
                CC_ex(opcode);
            }
            else
            {
                WZ = arg16();  /* implicit call PC+=2; */
            }
        }

        /***************************************************************
         * RET_COND
         ***************************************************************/
        void ret_cond(bool cond, uint8_t opcode)
        {
            if (cond)
            {
                pop(ref m_pc);
                WZ = PC;
                CC_ex(opcode);
            }
        }

        /***************************************************************
         * RETN
         ***************************************************************/
        void retn()
        {
            LOG("Z80 RETN m_iff1:{0} m_iff2:{1}\n", m_iff1, m_iff2);
            pop(ref m_pc);
            WZ = PC;
            m_iff1 = m_iff2;
        }

        /***************************************************************
         * RETI
         ***************************************************************/
        void reti()
        {
            pop(ref m_pc);
            WZ = PC;
            m_iff1 = m_iff2;
            m_daisy.daisy_call_reti_device();
        }

        /***************************************************************
         * LD   R,A
         ***************************************************************/
        void ld_r_a()
        {
            m_r = A;
            m_r2 = (byte)(A & 0x80);  /* keep bit 7 of r */
        }

        /***************************************************************
         * LD   A,R
         ***************************************************************/
        void ld_a_r()
        {
            A = (byte)((m_r & 0x7f) | m_r2);
            F = (byte)((F & CF) | SZ[A] | (m_iff2 << 2));
            m_after_ldair = true;
        }

        /***************************************************************
         * LD   I,A
         ***************************************************************/
        void ld_i_a()
        {
            m_i = A;
        }

        /***************************************************************
         * LD   A,I
         ***************************************************************/
        void ld_a_i()
        {
            A = m_i;
            F = (byte)((F & CF) | SZ[A] | (m_iff2 << 2));
            m_after_ldair = true;
        }

        /***************************************************************
         * RST
         ***************************************************************/
        void rst(uint16_t addr)
        {
            push(m_pc);
            PCD = addr;
            WZ = PC;
        }

        /***************************************************************
         * INC  r8
         ***************************************************************/
        byte inc(uint8_t value)
        {
            byte res = (byte)(value + 1);
            F = (byte)((F & CF) | SZHV_inc[res]);
            return (byte)res;
        }

        /***************************************************************
         * DEC  r8
         ***************************************************************/
        byte dec(uint8_t value)
        {
            byte res = (byte)(value - 1);
            F = (byte)((F & CF) | SZHV_dec[res]);
            return res;
        }

        /***************************************************************
         * RLCA
         ***************************************************************/
        void rlca()
        {
            A = (byte)((A << 1) | (A >> 7));
            F = (byte)((F & (SF | ZF | PF)) | (A & (YF | XF | CF)));
        }

        /***************************************************************
         * RRCA
         ***************************************************************/
        void rrca()
        {
            F = (byte)((F & (SF | ZF | PF)) | (A & CF));
            A = (byte)((A >> 1) | (A << 7));
            F |= (byte)(A & (YF | XF));
        }

        /***************************************************************
         * RLA
         ***************************************************************/
        void rla()
        {
            byte res = (byte)((A << 1) | (F & CF));
            byte c = (A & 0x80) != 0 ? CF : (byte)0;
            F = (byte)((F & (SF | ZF | PF)) | c | (res & (YF | XF)));
            A = res;
        }

        /***************************************************************
         * RRA
         ***************************************************************/
        void rra()
        {
            byte res = (byte)((A >> 1) | (F << 7));
            byte c = (A & 0x01) != 0 ? CF : (byte)0;
            F = (byte)((F & (SF | ZF | PF)) | c | (res & (YF | XF)));
            A = res;
        }

        /***************************************************************
         * RRD
         ***************************************************************/
        void rrd()
        {
            byte n = rm(HL);
            WZ = (UInt16)(HL+1);
            wm(HL, (byte)((n >> 4) | (A << 4)));
            A = (byte)((A & 0xf0) | (n & 0x0f));
            F = (byte)((F & CF) | SZP[A]);
        }

        /***************************************************************
         * RLD
         ***************************************************************/
        void rld()
        {
            byte n = rm(HL);
            WZ = (UInt16)(HL+1);
            wm(HL, (byte)((n << 4) | (A & 0x0f)));
            A = (byte)((A & 0xf0) | (n >> 4));
            F = (byte)((F & CF) | SZP[A]);
        }


        /***************************************************************
         * ADD  A,n
         ***************************************************************/
        void add_a(uint8_t value)
        {
            UInt32 ah = AFD & 0xff00;
            UInt32 res = (byte)((ah >> 8) + value);
            F = SZHVC_add[ah | res];
            A = (byte)res;
        }

        /***************************************************************
         * ADC  A,n
         ***************************************************************/
        void adc_a(uint8_t value)
        {
            UInt32 ah = AFD & 0xff00, c = AFD & 1;
            UInt32 res = (byte)((ah >> 8) + value + c);
            F = SZHVC_add[(c << 16) | ah | res];
            A = (byte)res;
        }

        /***************************************************************
         * SUB  n
         ***************************************************************/
        void sub(uint8_t value)
        {
            UInt32 ah = AFD & 0xff00;
            UInt32 res = (byte)((ah >> 8) - value);
            F = SZHVC_sub[ah | res];
            A = (byte)res;
        }

        /***************************************************************
         * SBC  A,n
         ***************************************************************/
        void sbc_a(uint8_t value)
        {
            UInt32 ah = AFD & 0xff00, c = AFD & 1;
            UInt32 res = (byte)((ah >> 8) - value - c);
            F = SZHVC_sub[(c<<16) | ah | res];
            A = (byte)res;
        }

        /***************************************************************
         * NEG
         ***************************************************************/
        void neg()
        {
            byte value = A;
            A = 0;
            sub(value);
        }

        /***************************************************************
         * DAA
         ***************************************************************/
        void daa()
        {
            byte a = A;
            if ((F & NF) != 0)
            {
                if (((F & HF) | (((A & 0xf) > 9) ? 1 : 0)) != 0)  a -= 6;
                if (((F & CF) | ((A > 0x99) ? 1 : 0)) != 0)       a -= 0x60;
            }
            else
            {
                if (((F & HF) | (((A & 0xf) > 9) ? 1 : 0)) != 0)  a += 6;
                if (((F & CF) | ((A > 0x99) ? 1 : 0)) != 0)       a += 0x60;
            }

            F = (byte)((F & (CF | NF)) | ((A > 0x99) ? 1 : 0) | ((A ^ a) & HF) | SZP[a]);
            A = a;
        }

        /***************************************************************
         * AND  n
         ***************************************************************/
        void and_a(uint8_t value)
        {
            A &= value;
            F = (byte)(SZP[A] | HF);
        }

        /***************************************************************
         * OR   n
         ***************************************************************/
        void or_a(uint8_t value)
        {
            A |= value;
            F = SZP[A];
        }

        /***************************************************************
         * XOR  n
         ***************************************************************/
        void xor_a(uint8_t value)
        {
            A ^= value;
            F = SZP[A];
        }

        /***************************************************************
         * CP   n
         ***************************************************************/
        void cp(uint8_t value)
        {
            UInt32 val = value;
            UInt32 ah = AFD & 0xff00;
            UInt32 res = (byte)((ah >> 8) - val);
            F = (byte)((SZHVC_sub[ah | res] & ~(YF | XF)) | (val & (YF | XF)));
        }

        /***************************************************************
         * EX   AF,AF'
         ***************************************************************/
        void ex_af()
        {
            PAIR tmp;
            tmp = m_af; m_af = m_af2; m_af2 = tmp;
        }

        /***************************************************************
         * EX   DE,HL
         ***************************************************************/
        void ex_de_hl()
        {
            PAIR tmp;
            tmp = m_de; m_de = m_hl; m_hl = tmp;
        }

        /***************************************************************
         * EXX
         ***************************************************************/
        void exx()
        {
            PAIR tmp;
            tmp = m_bc; m_bc = m_bc2; m_bc2 = tmp;
            tmp = m_de; m_de = m_de2; m_de2 = tmp;
            tmp = m_hl; m_hl = m_hl2; m_hl2 = tmp;
        }

        /***************************************************************
         * EX   (SP),r16
         ***************************************************************/
        void ex_sp(ref PAIR r)
        {
            PAIR tmp = new PAIR();
            rm16((UInt16)SPD, ref tmp);
            wm16((UInt16)SPD, r);
            r = tmp;
            WZ = (UInt16)r.d;
        }

        /***************************************************************
         * ADD16
         ***************************************************************/
        void add16(ref PAIR dr, PAIR sr)
        {
            UInt32 res = dr.d + sr.d;
            WZ = (UInt16)(dr.d + 1);
            F = (byte)((F & (SF | ZF | VF)) |
                (((dr.d ^ res ^ sr.d) >> 8) & HF) |
                ((res >> 16) & CF) | ((res >> 8) & (YF | XF)));
            dr.w.l = (UInt16)res;
        }

        /***************************************************************
         * ADC  HL,r16
         ***************************************************************/
        void adc_hl(PAIR r)
        {
            UInt32 res = (UInt32)(HLD + r.d + (F & CF));
            WZ = (UInt16)(HL + 1);
            F = (byte)((((HLD ^ res ^ r.d) >> 8) & HF) |
                ((res >> 16) & CF) |
                ((res >> 8) & (SF | YF | XF)) |
                (((res & 0xffff) != 0) ? 0 : ZF) |
                (((r.d ^ HLD ^ 0x8000) & (r.d ^ res) & 0x8000) >> 13));
            HL = (UInt16)res;
        }

        /***************************************************************
         * SBC  HL,r16
         ***************************************************************/
        void sbc_hl(PAIR r)
        {
            UInt32 res = (UInt32)(HLD - r.d - (F & CF));
            WZ = (UInt16)(HL + 1);
            F = (byte)((((HLD ^ res ^ r.d) >> 8) & HF) | NF |
                ((res >> 16) & CF) |
                ((res >> 8) & (SF | YF | XF)) |
                (((res & 0xffff) != 0) ? 0 : ZF) |
                (((r.d ^ HLD) & (HLD ^ res) &0x8000) >> 13));
            HL = (UInt16)res;
        }

        /***************************************************************
         * RLC  r8
         ***************************************************************/
        byte rlc(uint8_t value)
        {
            UInt32 res = value;
            UInt32 c = (res & 0x80) != 0 ? CF : 0U;
            res = ((res << 1) | (res >> 7)) & 0xff;
            F = (byte)(SZP[res] | c);
            return (byte)res;
        }

        /***************************************************************
         * RRC  r8
         ***************************************************************/
        byte rrc(uint8_t value)
        {
            UInt32 res = value;
            UInt32 c = (res & 0x01) != 0 ? CF : 0U;
            res = ((res >> 1) | (res << 7)) & 0xff;
            F = (byte)(SZP[res] | c);
            return (byte)res;
        }

        /***************************************************************
         * RL   r8
         ***************************************************************/
        byte rl(uint8_t value)
        {
            UInt32 res = value;
            UInt32 c = (res & 0x80) != 0 ? CF : 0U;
            res = (byte)(((res << 1) | (F & CF)) & 0xff);
            F = (byte)(SZP[res] | c);
            return (byte)res;
        }

        /***************************************************************
         * RR   r8
         ***************************************************************/
        byte rr(uint8_t value)
        {
            UInt32 res = value;
            UInt32 c = (res & 0x01) != 0 ? CF : 0U;
            res = (byte)(((res >> 1) | (F << 7)) & 0xff);
            F = (byte)(SZP[res] | c);
            return (byte)res;
        }

        /***************************************************************
         * SLA  r8
         ***************************************************************/
        byte sla(uint8_t value)
        {
            UInt32 res = value;
            UInt32 c = (res & 0x80) != 0 ? CF : 0U;
            res = (res << 1) & 0xff;
            F = (byte)(SZP[res] | c);
            return (byte)res;
        }

        /***************************************************************
         * SRA  r8
         ***************************************************************/
        byte sra(uint8_t value)
        {
            UInt32 res = value;
            UInt32 c = (res & 0x01) != 0 ? CF : 0U;
            res = ((res >> 1) | (res & 0x80)) & 0xff;
            F = (byte)(SZP[res] | c);
            return (byte)res;
        }

        /***************************************************************
         * SLL  r8
         ***************************************************************/
        byte sll(uint8_t value)
        {
            UInt32 res = value;
            UInt32 c = (res & 0x80) != 0 ? CF : 0U;
            res = ((res << 1) | 0x01) & 0xff;
            F = (byte)(SZP[res] | c);
            return (byte)res;
        }

        /***************************************************************
         * SRL  r8
         ***************************************************************/
        byte srl(uint8_t value)
        {
            UInt32 res = value;
            UInt32 c = (res & 0x01) != 0 ? CF : 0U;
            res = (res >> 1) & 0xff;
            F = (byte)(SZP[res] | c);
            return (byte)res;
        }

        /***************************************************************
         * BIT  bit,r8
         ***************************************************************/
        void bit(int bit, uint8_t value)
        {
            F = (byte)((F & CF) | HF | (SZ_BIT[value & (1 << bit)] & ~(YF | XF)) | (value & (YF | XF)));
        }

        /***************************************************************
         * BIT  bit,(HL)
         ***************************************************************/
        void bit_hl(int bit, uint8_t value)
        {
            F = (byte)((F & CF) | HF | (SZ_BIT[value & (1 << bit)] & ~(YF | XF)) | (WZ_H & (YF | XF)));
        }

        /***************************************************************
         * BIT  bit,(IX/Y+o)
         ***************************************************************/
        void bit_xy(int bit, uint8_t value)
        {
            F = (byte)((F & CF) | HF | (SZ_BIT[value & (1 << bit)] & ~(YF | XF)) | ((m_ea >> 8) & (YF | XF)));
        }

        /***************************************************************
         * RES  bit,r8
         ***************************************************************/
        byte res(int bit, uint8_t value)
        {
            return (byte)(value & ~(1 << bit));
        }

        /***************************************************************
         * SET  bit,r8
         ***************************************************************/
        byte set(int bit, uint8_t value)
        {
            return (byte)(value | (1 << bit));
        }

        /***************************************************************
         * LDI
         ***************************************************************/
        void ldi()
        {
            byte io = rm(HL);
            wm(DE, io);
            F &= SF | ZF | CF;
            if (((A + io) & 0x02) != 0) F |= YF; /* bit 1 -> flag 5 */
            if (((A + io) & 0x08) != 0) F |= XF; /* bit 3 -> flag 3 */
            HL++; DE++; BC--;
            if(BC != 0) F |= VF;
        }

        /***************************************************************
         * CPI
         ***************************************************************/
        void cpi()
        {
            uint8_t val = rm(HL);
            uint8_t res = (byte)(A - val);
            WZ++;
            HL++; BC--;
            F = (byte)((F & CF) | (SZ[res]&~(YF|XF)) | ((A^val^res)&HF) | NF);
            if ((F & HF) != 0) res -= 1;
            if ((res & 0x02) != 0) F |= YF; /* bit 1 -> flag 5 */
            if ((res & 0x08) != 0) F |= XF; /* bit 3 -> flag 3 */
            if (BC != 0) F |= VF;
        }

        /***************************************************************
         * INI
         ***************************************************************/
        void ini()
        {
            UInt32 t;
            uint8_t io = in_(BC);
            WZ = (UInt16)(BC + 1);
            B--;
            wm(HL, io);
            HL++;
            F = SZ[B];
            t = (UInt32)((C + 1) & 0xff) + (UInt32)io;
            if ((io & SF) != 0) F |= NF;
            if ((t & 0x100) != 0) F |= HF | CF;
            F |= (byte)(SZP[(byte)(t & 0x07) ^ B] & PF);
        }

        /***************************************************************
         * OUTI
         ***************************************************************/
        void outi()
        {
            UInt32 t;
            uint8_t io = rm(HL);
            B--;
            WZ = (UInt16)(BC + 1);
            out_(BC, io);
            HL++;
            F = SZ[B];
            t = (UInt32)L + (UInt32)io;
            if ((io & SF) != 0) F |= NF;
            if ((t & 0x100) != 0) F |= HF | CF;
            F |= (byte)(SZP[(byte)(t & 0x07) ^ B] & PF);
        }

        /***************************************************************
         * LDD
         ***************************************************************/
        void ldd()
        {
            uint8_t io = rm(HL);
            wm(DE, io);
            F &= SF | ZF | CF;
            if (((A + io) & 0x02) != 0) F |= YF; /* bit 1 -> flag 5 */
            if (((A + io) & 0x08) != 0) F |= XF; /* bit 3 -> flag 3 */
            HL--; DE--; BC--;
            if (BC != 0) F |= VF;
        }

        /***************************************************************
         * CPD
         ***************************************************************/
        void cpd()
        {
            uint8_t val = rm(HL);
            uint8_t res = (byte)(A - val);
            WZ--;
            HL--; BC--;
            F = (byte)((F & CF) | (SZ[res]&~(YF|XF)) | ((A^val^res)&HF) | NF);
            if ((F & HF) != 0) res -= 1;
            if ((res & 0x02) != 0) F |= YF; /* bit 1 -> flag 5 */
            if ((res & 0x08) != 0) F |= XF; /* bit 3 -> flag 3 */
            if (BC != 0) F |= VF;
        }

        /***************************************************************
         * IND
         ***************************************************************/
        void ind()
        {
            UInt32 t;
            uint8_t io = in_(BC);
            WZ = (UInt16)(BC - 1);
            B--;
            wm(HL, io);
            HL--;
            F = SZ[B];
            t = ((UInt32)(C - 1) & 0xff) + (UInt32)io;
            if ((io & SF) != 0) F |= NF;
            if ((t & 0x100) != 0) F |= HF | CF;
            F |= (byte)(SZP[(byte)(t & 0x07) ^ B] & PF);
        }

        /***************************************************************
         * OUTD
         ***************************************************************/
        void outd()
        {
            UInt32 t;
            uint8_t io = rm(HL);
            B--;
            WZ = (UInt16)(BC - 1);
            out_(BC, io);
            HL--;
            F = SZ[B];
            t = (UInt32)L + (UInt32)io;
            if ((io & SF) != 0) F |= NF;
            if ((t & 0x100) != 0) F |= HF | CF;
            F |= (byte)(SZP[(byte)(t & 0x07) ^ B] & PF);
        }

        /***************************************************************
         * LDIR
         ***************************************************************/
        void ldir()
        {
            ldi();
            if (BC != 0)
            {
                PC -= 2;
                WZ = (UInt16)(PC + 1);
                CC_ex(0xb0);
            }
        }

        /***************************************************************
         * CPIR
         ***************************************************************/
        void cpir()
        {
            cpi();
            if (BC != 0 && !((F & ZF) != 0))
            {
                PC -= 2;
                WZ = (UInt16)(PC + 1);
                CC_ex(0xb1);
            }
        }

        /***************************************************************
         * INIR
         ***************************************************************/
        void inir()
        {
            ini();
            if (B != 0)
            {
                PC -= 2;
                CC_ex(0xb2);
            }
        }

        /***************************************************************
         * OTIR
         ***************************************************************/
        void otir()
        {
            outi();
            if (B != 0)
            {
                PC -= 2;
                CC_ex(0xb3);
            }
        }

        /***************************************************************
         * LDDR
         ***************************************************************/
        void lddr()
        {
            ldd();
            if (BC != 0)
            {
                PC -= 2;
                WZ = (UInt16)(PC + 1);
                CC_ex(0xb8);
            }
        }

        /***************************************************************
         * CPDR
         ***************************************************************/
        void cpdr()
        {
            cpd();
            if (BC != 0 && !((F & ZF) != 0))
            {
                PC -= 2;
                WZ = (UInt16)(PC + 1);
                CC_ex(0xb9);
            }
        }

        /***************************************************************
         * INDR
         ***************************************************************/
        void indr()
        {
            ind();
            if (B != 0)
            {
                PC -= 2;
                CC_ex(0xba);
            }
        }

        /***************************************************************
         * OTDR
         ***************************************************************/
        void otdr()
        {
            outd();
            if (B != 0)
            {
                PC -= 2;
                CC_ex(0xbb);
            }
        }

        /***************************************************************
         * EI
         ***************************************************************/
        void ei()
        {
            m_iff1 = m_iff2 = 1;
            m_after_ei = true;
        }

        public void take_interrupt()
        {
            // Check if processor was halted
            leave_halt();

            // clear both interrupt flip flops
            m_iff1 = m_iff2 = 0;

            // say hi
            m_irqack_cb.op(1);

            // fetch the IRQ vector
            device_z80daisy_interface intf = m_daisy.daisy_get_irq_device();
            int irq_vector = (intf != null) ? intf.z80daisy_irq_ack() : m_diexec.standard_irq_callback_member(this, 0);
            LOG("Z80 single int. irq_vector ${0}\n", irq_vector);  // $%02x

            /* Interrupt mode 2. Call [i:databyte] */
            if (m_im == 2)
            {
                // Zilog's datasheet claims that "the least-significant bit must be a zero."
                // However, experiments have confirmed that IM 2 vectors do not have to be
                // even, and all 8 bits will be used; even $FF is handled normally.
                irq_vector = (irq_vector & 0xff) | (m_i << 8);
                push(m_pc);
                rm16((UInt16)irq_vector, ref m_pc);
                LOG("Z80 IM2 [${0}] = ${1}\n", irq_vector, PCD);  //[$%04x] = $%04x
                /* CALL opcode timing + 'interrupt latency' cycles */
                m_icount.i -= m_cc_op[0xcd] + m_cc_ex[0xff];
            }
            else
            /* Interrupt mode 1. RST 38h */
            if( m_im == 1 )
            {
                LOG("Z80 '{0}' IM1 $0038\n", tag());
                push(m_pc);
                PCD = 0x0038;
                /* RST $38 + 'interrupt latency' cycles */
                m_icount.i -= m_cc_op[0xff] + cc_ex[0xff];
            }
            else
            {
                /* Interrupt mode 0. We check for CALL and JP instructions, */
                /* if neither of these were found we assume a 1 byte opcode */
                /* was placed on the databus                                */
                LOG("Z80 IM0 ${1}\n", irq_vector);  // $%04x

                /* check for nop */
                if (irq_vector != 0x00)
                {
                    switch (irq_vector & 0xff0000)
                    {
                        case 0xcd0000:  /* call */
                            push(m_pc);
                            PCD = (UInt32)(irq_vector & 0xffff);
                                /* CALL $xxxx cycles */
                            m_icount.i -= m_cc_op[0xcd];
                            break;
                        case 0xc30000:  /* jump */
                            PCD = (UInt32)(irq_vector & 0xffff);
                            /* JP $xxxx cycles */
                            m_icount.i -= m_cc_op[0xc3];
                            break;
                        default:        /* rst (or other opcodes?) */
                            push(m_pc);
                            PCD = (UInt32)(irq_vector & 0x0038);
                            /* RST $xx cycles */
                            m_icount.i -= m_cc_op[0xff];
                            break;
                    }
                }

                /* 'interrupt latency' cycles */
                m_icount.i -= m_cc_ex[0xff];
            }

            WZ = (UInt16)PCD;

#if HAS_LDAIR_QUIRK
            /* reset parity flag after LD A,I or LD A,R */
            if (m_after_ldair) F &= ~PF;
#endif
        }


        public void take_nmi()
        {
            /* Check if processor was halted */
            leave_halt();

#if HAS_LDAIR_QUIRK
            /* reset parity flag after LD A,I or LD A,R */
            if (m_after_ldair) F &= ~PF;
#endif

            m_iff1 = 0;
            push(m_pc);
            PCD = 0x0066;
            WZ = (UInt16)PCD;
            m_icount.i -= 11;
            m_nmi_pending = false;
        }
    }
}
