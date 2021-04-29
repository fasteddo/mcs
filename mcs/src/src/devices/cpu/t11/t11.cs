// license:BSD-3-Clause
// copyright-holders:Edward Fast

//#define MCS_DEBUG

using System;
using System.Collections.Generic;

using devcb_read8 = mame.devcb_read<System.Byte, System.Byte, mame.devcb_operators_u8_u8, mame.devcb_operators_u8_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write_line = mame.devcb_write<int, uint, mame.devcb_operators_s32_u32, mame.devcb_operators_u32_s32, mame.devcb_constant_1<uint, uint, mame.devcb_operators_u32_u32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using offs_t = System.UInt32;  //using offs_t = u32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    //#define T11_IRQ0        0      /* IRQ0 */
    //#define T11_IRQ1        1      /* IRQ1 */
    //#define T11_IRQ2        2      /* IRQ2 */
    //#define T11_IRQ3        3      /* IRQ3 */


    public partial class t11_device : cpu_device
    {
        //DEFINE_DEVICE_TYPE(T11,      t11_device,      "t11",      "DEC T11")
        static device_t device_creator_t11_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new t11_device(mconfig, tag, owner, clock); }
        public static readonly device_type T11 = DEFINE_DEVICE_TYPE(device_creator_t11_device, "t11", "DEC T11");


        static uint8_t OCTAL_U8(int value) { return Convert.ToByte(value.ToString(), 8); }
        static uint16_t OCTAL_U16(int value) { return Convert.ToUInt16(value.ToString(), 8); }


        //enum
        //{
        const int CP0_LINE = 0;           // -AI4 (at PI time)
        const int CP1_LINE = 1;           // -AI3 (at PI time)
        const int CP2_LINE = 2;           // -AI2 (at PI time)
        const int CP3_LINE = 3;           // -AI1 (at PI time)
        const int VEC_LINE = 4;           // -AI5 (at PI time)
        const int PF_LINE  = 5;            // -AI6 (at PI time)
        const int HLT_LINE = 6;            // -AI7 (at PI time)
        //};


        //enum
        //{
        static readonly uint8_t T11_RESERVED    = OCTAL_U8(000);  // Reserved vector
        static readonly uint8_t T11_TIMEOUT     = OCTAL_U8(004);  // Time-out/system error vector
        static readonly uint8_t T11_ILLINST     = OCTAL_U8(010);  // Illegal and reserved instruction vector
        static readonly uint8_t T11_BPT         = OCTAL_U8(014);  // BPT instruction vector
        static readonly uint8_t T11_IOT         = OCTAL_U8(020);  // IOT instruction vector
        static readonly uint8_t T11_PWRFAIL     = OCTAL_U8(024);  // Power fail vector
        static readonly uint8_t T11_EMT         = OCTAL_U8(030);  // EMT instruction vector
        static readonly uint8_t T11_TRAP        = OCTAL_U8(034);  // TRAP instruction vector
        //};


        //enum
        //{
        const int T11_R0  = 1;
        const int T11_R1  = 2;
        const int T11_R2  = 3;
        const int T11_R3  = 4;
        const int T11_R4  = 5;
        const int T11_R5  = 6;
        const int T11_SP  = 7;
        const int T11_PC  = 8;
        const int T11_PSW = 9;
        //};


        /* registers of various sizes */
        ref uint32_t REGD(int x) { return ref m_reg[x].d; }  //#define REGD(x) m_reg[x].d
        ref uint16_t REGW(int x) { return ref m_reg[x].w.l; }  //#define REGW(x) m_reg[x].w.l
        ref uint8_t REGB(int x) { return ref m_reg[x].b.l; }  //#define REGB(x) m_reg[x].b.l

        /* PC, SP, and PSW definitions */
        uint16_t SP  { get { return REGW(6); } set { REGW(6) = value; } }  //#define SP      REGW(6)
        uint16_t PC  { get { return REGW(7); } set { REGW(7) = value; } }  //#define PC      REGW(7)
        uint32_t SPD { get { return REGD(6); } set { REGD(6) = value; } }  //#define SPD     REGD(6)
        uint32_t PCD { get { return REGD(7); } set { REGD(7) = value; } }  //#define PCD     REGD(7)
        uint8_t PSW  { get { return m_psw.b.l; } set { m_psw.b.l = value; } }  //#define PSW     m_psw.b.l


        /*************************************
         *
         *  Flag definitions and operations
         *
         *************************************/

        /* flag definitions */
        const uint8_t CFLAG = 1;
        const uint8_t VFLAG = 2;
        const uint8_t ZFLAG = 4;
        const uint8_t NFLAG = 8;

        /* extracts flags */
        uint8_t GET_C { get { return (uint8_t)(PSW & CFLAG); } }  //#define GET_C (PSW & CFLAG)
        uint8_t GET_V { get { return (uint8_t)(PSW & VFLAG); } }  //#define GET_V (PSW & VFLAG)
        uint8_t GET_Z { get { return (uint8_t)(PSW & ZFLAG); } }  //#define GET_Z (PSW & ZFLAG)
        uint8_t GET_N { get { return (uint8_t)(PSW & NFLAG); } }  //#define GET_N (PSW & NFLAG)

        /* clears flags */
        //#define CLR_C (PSW &= ~CFLAG)
        //#define CLR_V (PSW &= ~VFLAG)
        //#define CLR_Z (PSW &= ~ZFLAG)
        //#define CLR_N (PSW &= ~NFLAG)

        /* sets flags */
        void SET_C() { PSW |= CFLAG; }  //#define SET_C (PSW |= CFLAG)
        void SET_V() { PSW |= VFLAG; }  //#define SET_V (PSW |= VFLAG)
        void SET_Z() { PSW |= ZFLAG; }  //#define SET_Z (PSW |= ZFLAG)
        //#define SET_N (PSW |= NFLAG)


        class device_execute_interface_t11 : device_execute_interface
        {
            public device_execute_interface_t11(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint32_t execute_min_cycles() { return 12; }
            protected override uint32_t execute_max_cycles() { return 114; }
            protected override uint32_t execute_input_lines() { return 7; }
            protected override bool execute_input_edge_triggered(int inputnum) { return inputnum == PF_LINE || inputnum == HLT_LINE; }
            protected override void execute_run() { ((t11_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { ((t11_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
        }


        public class device_memory_interface_t11 : device_memory_interface
        {
            public device_memory_interface_t11(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((t11_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_t11 : device_state_interface
        {
            public device_state_interface_t11(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void state_string_export(device_state_entry entry, out string str) { ((t11_device)device()).device_state_interface_state_string_export(entry, out str); }
        }


        public class device_disasm_interface_t11 : device_disasm_interface
        {
            public device_disasm_interface_t11(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        delegate void opcode_func(uint16_t op);  //typedef void ( t11_device::*opcode_func )(uint16_t op);
        //static const opcode_func s_opcode_table[65536 >> 3];


        address_space_config m_program_config;

        uint16_t c_initial_mode;

        PAIR m_ppc;    /* previous program counter */
        PAIR [] m_reg = new PAIR [8];
        PAIR m_psw;
        uint16_t m_initial_pc;
        uint8_t m_wait_state;
        uint8_t m_cp_state;
        bool m_vec_active;
        bool m_pf_active;
        bool m_hlt_active;
        bool m_power_fail;
        bool m_ext_halt;
        intref m_icount = new intref();  //int m_icount;
        memory_access<int_constant_16, int_constant_1, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.cache m_cache = new memory_access<int_constant_16, int_constant_1, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.cache();  //memory_access<16, 1, 0, ENDIANNESS_LITTLE>::cache m_cache;
        memory_access<int_constant_16, int_constant_1, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.specific m_program = new memory_access<int_constant_16, int_constant_1, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.specific();  //memory_access<16, 1, 0, ENDIANNESS_LITTLE>::specific m_program;

        devcb_write_line m_out_reset_func;
        devcb_read8 m_in_iack_func;


        device_execute_interface m_diexec;
        device_memory_interface m_dimemory;
        device_state_interface m_distate;


#if MCS_DEBUG
        int opcount = 0;
#endif


        // construction/destruction
        t11_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : this(mconfig, T11, tag, owner, clock)
        {
        }

        t11_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_t11(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_t11(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_t11(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_t11(mconfig, this));


            init_opcode_table();

            RG  = new register_RG(this);
            RGD = new register_RGD(this);
            IN  = new register_IN(this);
            IND = new register_IND(this);
            DE  = new register_DE(this);
            DED = new register_DED(this);
            IX  = new register_IX(this);
            IXD = new register_IXD(this);


            m_program_config = new address_space_config("program", endianness_t.ENDIANNESS_LITTLE, 16, 16, 0);
            c_initial_mode = 0;
            m_cp_state = 0;
            m_vec_active = false;
            m_pf_active = false;
            m_hlt_active = false;
            m_out_reset_func = new devcb_write_line(this);
            m_in_iack_func = new devcb_read8(this);


            m_program_config.m_is_octal = true;

            for (int i = 0; i < m_reg.Length; i++)  //for (auto &reg : m_reg)
                m_reg[i].d = 0;
            m_psw.d = 0;
            m_ppc.d = 0;
        }


        // configuration helpers
        public void set_initial_mode(uint16_t mode) { c_initial_mode = mode; }
        //auto out_reset() { return m_out_reset_func.bind(); }
        //auto in_iack() { return m_in_iack_func.bind(); }


        // device-level overrides

        /*************************************
         *
         *  Low-level initialization/cleanup
         *
         *************************************/

        static readonly uint16_t [] device_start_initial_pc =
        {
            0xc000, 0x8000, 0x4000, 0x2000,
            0x1000, 0x0000, 0xf600, 0xf400
        };

        protected override void device_start()
        {
            m_diexec = GetClassInterface<device_execute_interface_t11>();
            m_dimemory = GetClassInterface<device_memory_interface_t11>();
            m_distate = GetClassInterface<device_state_interface_t11>();


            m_initial_pc = device_start_initial_pc[c_initial_mode >> 13];
            m_dimemory.space(AS_PROGRAM).cache(m_cache);
            m_dimemory.space(AS_PROGRAM).specific(m_program);
            m_out_reset_func.resolve_safe();
            m_in_iack_func.resolve_safe(0); // default vector (T-11 User's Guide, p. A-11)

            save_item(NAME(new { m_ppc.w.l }));
            save_item(NAME(new { m_reg[0].w.l }));
            save_item(NAME(new { m_reg[1].w.l }));
            save_item(NAME(new { m_reg[2].w.l }));
            save_item(NAME(new { m_reg[3].w.l }));
            save_item(NAME(new { m_reg[4].w.l }));
            save_item(NAME(new { m_reg[5].w.l }));
            save_item(NAME(new { m_reg[6].w.l }));
            save_item(NAME(new { m_reg[7].w.l }));
            save_item(NAME(new { m_psw.w.l }));
            save_item(NAME(new { m_initial_pc }));
            save_item(NAME(new { m_wait_state }));
            save_item(NAME(new { m_cp_state }));
            save_item(NAME(new { m_vec_active }));
            save_item(NAME(new { m_pf_active }));
            save_item(NAME(new { m_hlt_active }));
            save_item(NAME(new { m_power_fail }));
            save_item(NAME(new { m_ext_halt }));

            // Register debugger state
            m_distate.state_add( T11_PC,  "PC",  m_reg[7].w.l).formatstr("%06O");
            m_distate.state_add( T11_SP,  "SP",  m_reg[6].w.l).formatstr("%06O");
            m_distate.state_add( T11_PSW, "PSW", m_psw.b.l).formatstr("%03O");
            m_distate.state_add( T11_R0,  "R0",  m_reg[0].w.l).formatstr("%06O");
            m_distate.state_add( T11_R1,  "R1",  m_reg[1].w.l).formatstr("%06O");
            m_distate.state_add( T11_R2,  "R2",  m_reg[2].w.l).formatstr("%06O");
            m_distate.state_add( T11_R3,  "R3",  m_reg[3].w.l).formatstr("%06O");
            m_distate.state_add( T11_R4,  "R4",  m_reg[4].w.l).formatstr("%06O");
            m_distate.state_add( T11_R5,  "R5",  m_reg[5].w.l).formatstr("%06O");

            m_distate.state_add(g.STATE_GENPC, "GENPC", m_reg[7].w.l).noshow();
            m_distate.state_add(g.STATE_GENPCBASE, "CURPC", m_ppc.w.l).noshow();
            m_distate.state_add(g.STATE_GENFLAGS, "GENFLAGS", m_psw.b.l).formatstr("%8s").noshow();

            set_icountptr(m_icount);
        }


        /*************************************
         *
         *  CPU reset
         *
         *************************************/
        protected override void device_reset()
        {
            // initial SP is 376 octal, or 0xfe
            SP = OCTAL_U16(0376);

            // initial PC comes from the setup word
            PC = m_initial_pc;

            // PSW starts off at highest priority
            PSW = OCTAL_U8(0340);

            m_wait_state = 0;
            m_power_fail = false;
            m_ext_halt = false;
        }


        // device_execute_interface overrides

        /*************************************
         *
         *  Core execution
         *
         *************************************/
        void device_execute_interface_execute_run()
        {
            t11_check_irqs();

            if (m_wait_state != 0)
            {
                m_icount.i = 0;
                return;
            }

            while (m_icount.i > 0)
            {
                uint16_t op;

                m_ppc = m_reg[7];   /* copy PC to previous PC */

                debugger_instruction_hook(PCD);

                op = (uint16_t)ROPCODE();


#if MCS_DEBUG
                var atari_sound_comm = (atari_sound_comm_device)owner().subdevice("soundcomm");
                int s2m = atari_sound_comm.sound_to_main_ready();
                int m2s = atari_sound_comm.main_to_sound_ready();
                uint8_t m2sd = atari_sound_comm.m_main_to_sound_data;

                if (opcount >= 0 && opcount % 50000 == 0)
                    osd_printf_debug("t11_device.execute_run() - {0,6} - {1,5} - {2,5} - {3,5} - {4,5} - {5,5} - {6} - {7} - {8}\n", opcount, op, m_icount.i, PC, GET_Z, PSW, s2m, m2s, m_reg[0].d);
                //if (opcount >= 9000 && opcount <= 10000)
                //    osd_printf_debug("t11_device.execute_run() - {0,6} - {1,5} - {2,5} - {3,5} - {4,5} - {5,5} - {6} - {7} - {8}\n", opcount, op, m_icount.i, PC, GET_Z, PSW, s2m, m2s, m_reg[0].d);
                //if (opcount == 10000)
                //    osd_printf_debug("t11_device.execute_run() - {0,6} - {1,5} - {2,5} - {3,5} - {4,5} - {5,5} - {6} - {7} - {8}\n", opcount, op, m_icount.i, PC, GET_Z, PSW, s2m, m2s, m_reg[0].d);
#endif


                (s_opcode_table[op >> 3])(op);  //(this->*s_opcode_table[op >> 3])(op);


#if MCS_DEBUG
                opcount++;
#endif
            }
        }


        /*************************************
         *
         *  Interrupt handling
         *
         *************************************/
        void device_execute_interface_execute_set_input(int irqline, int state)
        {
            switch (irqline)
            {
            case CP0_LINE:
            case CP1_LINE:
            case CP2_LINE:
            case CP3_LINE:
                // set the appropriate bit
                if (state == g.CLEAR_LINE)
                    m_cp_state &= (uint8_t)(~(1 << irqline));
                else
                    m_cp_state |= (uint8_t)(1 << irqline);
                break;

            case VEC_LINE:
                m_vec_active = (state != g.CLEAR_LINE);
                break;

            case PF_LINE:
                if (state != g.CLEAR_LINE && !m_pf_active)
                    m_power_fail = true;
                m_pf_active = (state != g.CLEAR_LINE);
                break;

            case HLT_LINE:
                if (state != g.CLEAR_LINE && !m_hlt_active)
                    m_ext_halt = true;
                m_hlt_active = (state != g.CLEAR_LINE);
                break;
            }
        }


        //virtual uint32_t execute_default_irq_vector(int inputnum) const noexcept override { return -1; }


        // device_memory_interface overrides
        space_config_vector device_memory_interface_memory_space_config()
        {
            return new space_config_vector { std.make_pair(AS_PROGRAM, m_program_config) };
        }


        // device_state_interface overrides
        void device_state_interface_state_string_export(device_state_entry entry, out string str)
        {
            throw new emu_unimplemented();
        }


        // device_disasm_interface overrides
        util.disasm_interface create_disassembler()
        {
            throw new emu_unimplemented();
        }


        /*************************************
         *
         *  Low-level memory operations
         *
         *************************************/
        int ROPCODE()
        {
            PC &= 0xfffe;
            int val = m_cache.read_word(PC);
            PC += 2;
            return val;
        }


        int RBYTE(int addr)
        {
            return m_program.read_byte((offs_t)addr);
        }

        void WBYTE(int addr, int data)
        {
            m_program.write_byte((offs_t)addr, (uint8_t)data);
        }

        int RWORD(int addr)
        {
            return m_program.read_word((offs_t)(addr & 0xfffe));
        }

        void WWORD(int addr, int data)
        {
            m_program.write_word((offs_t)(addr & 0xfffe), (uint16_t)data);
        }


        /*************************************
         *
         *  Low-level stack operations
         *
         *************************************/

        void PUSH(int val)
        {
            SP -= 2;
            WWORD((int)SPD, val);
        }


        int POP()
        {
            int result = RWORD((int)SPD);
            SP += 2;
            return result;
        }


        /*************************************
         *
         *  Interrupt handling
         *
         *************************************/

        struct irq_table_entry
        {
            public uint8_t priority;
            public uint8_t vector;

            public irq_table_entry(uint8_t priority, uint8_t vector) { this.priority = priority; this.vector = vector; }
        }

        static readonly irq_table_entry [] irq_table = new irq_table_entry []
        {
            new irq_table_entry( 0<<5, OCTAL_U8(0000)),
            new irq_table_entry( 4<<5, OCTAL_U8(0070)),
            new irq_table_entry( 4<<5, OCTAL_U8(0064)),
            new irq_table_entry( 4<<5, OCTAL_U8(0060)),
            new irq_table_entry( 5<<5, OCTAL_U8(0134)),
            new irq_table_entry( 5<<5, OCTAL_U8(0130)),
            new irq_table_entry( 5<<5, OCTAL_U8(0124)),
            new irq_table_entry( 5<<5, OCTAL_U8(0120)),
            new irq_table_entry( 6<<5, OCTAL_U8(0114)),
            new irq_table_entry( 6<<5, OCTAL_U8(0110)),
            new irq_table_entry( 6<<5, OCTAL_U8(0104)),
            new irq_table_entry( 6<<5, OCTAL_U8(0100)),
            new irq_table_entry( 7<<5, OCTAL_U8(0154)),
            new irq_table_entry( 7<<5, OCTAL_U8(0150)),
            new irq_table_entry( 7<<5, OCTAL_U8(0144)),
            new irq_table_entry( 7<<5, OCTAL_U8(0140))
        };

        void t11_check_irqs()
        {
            // HLT is nonmaskable
            if (m_ext_halt)
            {
                m_ext_halt = false;

                // push the old state, set the new one
                PUSH(PSW);
                PUSH(PC);
                PCD = m_initial_pc + 4U;
                PSW = OCTAL_U8(0340);

                // count cycles and clear the WAIT flag
                m_icount.i -= 114;
                m_wait_state = 0;

                return;
            }

            // PF has next-highest priority
            int priority = PSW & 0340;
            if (m_power_fail && priority != 0340)
            {
                m_power_fail = false;
                take_interrupt(T11_PWRFAIL);
                return;
            }

            // compare the priority of the CP interrupt to the PSW
            ref irq_table_entry irq = ref irq_table[m_cp_state & 15];
            if (irq.priority > priority)
            {
                // call the callback
                standard_irq_callback(m_cp_state & 15);

                // T11 encodes the interrupt level on DAL<12:8>
                uint8_t iaddr = (uint8_t)g.bitswap(~m_cp_state & 15, 0, 1, 2, 3);
                if (!m_vec_active)
                    iaddr |= 16;

                // vector is input on DAL<7:2>
                uint8_t vector = m_in_iack_func.op(iaddr);

                // nonvectored or vectored interrupt depending on VEC
                if (g.BIT(iaddr, 4) != 0)
                    take_interrupt(irq.vector);
                else
                    take_interrupt((uint8_t)(vector & ~3));
            }
        }


        void take_interrupt(uint8_t vector)
        {
            throw new emu_unimplemented();
        }
    }


    //class k1801vm2_device : public t11_device


    //DECLARE_DEVICE_TYPE(T11,      t11_device)
    //DECLARE_DEVICE_TYPE(K1801VM2, k1801vm2_device)
}
