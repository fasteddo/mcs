// license:BSD-3-Clause
// copyright-holders:Edward Fast

//#define MCS_DEBUG

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    //#define T11_IRQ0        0      /* IRQ0 */
    //#define T11_IRQ1        1      /* IRQ1 */
    //#define T11_IRQ2        2      /* IRQ2 */
    //#define T11_IRQ3        3      /* IRQ3 */

    //#define T11_RESERVED    0x000   /* Reserved vector */
    //#define T11_TIMEOUT     0x004   /* Time-out/system error vector */
    //#define T11_ILLINST     0x008   /* Illegal and reserved instruction vector */
    //#define T11_BPT         0x00C   /* BPT instruction vector */
    //#define T11_IOT         0x010   /* IOT instruction vector */
    //#define T11_PWRFAIL     0x014   /* Power fail vector */
    //#define T11_EMT         0x018   /* EMT instruction vector */
    //#define T11_TRAP        0x01C   /* TRAP instruction vector */


    public partial class t11_device : cpu_device
    {
        //DEFINE_DEVICE_TYPE(T11,      t11_device,      "t11",      "DEC T11")
        static device_t device_creator_t11_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new t11_device(mconfig, tag, owner, clock); }
        public static readonly device_type T11 = DEFINE_DEVICE_TYPE(device_creator_t11_device, "t11", "DEC T11");


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
            protected override uint32_t execute_input_lines() { return 4; }
            protected override void execute_run() { ((t11_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { ((t11_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
            protected override uint32_t execute_default_irq_vector(int inputnum) { return uint32_t.MaxValue; }  //return -1; }
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
        uint8_t m_irq_state;
        intref m_icount = new intref();  //int m_icount;
        address_space m_program;
        memory_access_cache m_cache;  //memory_access_cache<1, 0, ENDIANNESS_LITTLE> *m_cache;
        devcb_write_line m_out_reset_func;


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
            m_out_reset_func = new devcb_write_line(this);


            m_program_config.m_is_octal = true;

            //memset(m_reg, 0x00, sizeof(m_reg));
            //memset(&m_psw, 0x00, sizeof(m_psw));
        }


        // configuration helpers
        public void set_initial_mode(uint16_t mode) { c_initial_mode = mode; }
        //auto out_reset() { return m_out_reset_func.bind(); }


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
            m_program = m_dimemory.space(AS_PROGRAM);
            m_cache = m_program.cache(1, 0, (int)endianness_t.ENDIANNESS_LITTLE);
            m_out_reset_func.resolve_safe();

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
            save_item(NAME(new { m_irq_state }));

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

            m_distate.state_add(STATE_GENPC, "GENPC", m_reg[7].w.l).noshow();
            m_distate.state_add(STATE_GENPCBASE, "CURPC", m_ppc.w.l).noshow();
            m_distate.state_add(STATE_GENFLAGS, "GENFLAGS", m_psw.b.l).formatstr("%8s").noshow();

            set_icountptr(m_icount);
        }


        /*************************************
         *
         *  CPU reset
         *
         *************************************/
        protected override void device_reset()
        {
            /* initial SP is 376 octal, or 0xfe */
            SP = 0x00fe;

            /* initial PC comes from the setup word */
            PC = m_initial_pc;

            /* PSW starts off at highest priority */
            PSW = 0xe0;

            /* initialize the IRQ state */
            m_irq_state = 0;

            /* reset the remaining state */
            REGD(0) = 0;
            REGD(1) = 0;
            REGD(2) = 0;
            REGD(3) = 0;
            REGD(4) = 0;
            REGD(5) = 0;
            m_ppc.d = 0;
            m_wait_state = 0;
        }


        // device_execute_interface overrides
        //virtual uint32_t execute_min_cycles() const noexcept override { return 12; }
        //virtual uint32_t execute_max_cycles() const noexcept override { return 114; }
        //virtual uint32_t execute_input_lines() const noexcept override { return 4; }


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
            /* set the appropriate bit */
            if (state == CLEAR_LINE)
                m_irq_state &= (uint8_t)~(1U << irqline);
            else
                m_irq_state |= (uint8_t)(1U << irqline);
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
            new irq_table_entry( 0<<5, 0x00 ),
            new irq_table_entry( 4<<5, 0x38 ),
            new irq_table_entry( 4<<5, 0x34 ),
            new irq_table_entry( 4<<5, 0x30 ),
            new irq_table_entry( 5<<5, 0x5c ),
            new irq_table_entry( 5<<5, 0x58 ),
            new irq_table_entry( 5<<5, 0x54 ),
            new irq_table_entry( 5<<5, 0x50 ),
            new irq_table_entry( 6<<5, 0x4c ),
            new irq_table_entry( 6<<5, 0x48 ),
            new irq_table_entry( 6<<5, 0x44 ),
            new irq_table_entry( 6<<5, 0x40 ),
            new irq_table_entry( 7<<5, 0x6c ),
            new irq_table_entry( 7<<5, 0x68 ),
            new irq_table_entry( 7<<5, 0x64 ),
            new irq_table_entry( 7<<5, 0x60 )
        };

        void t11_check_irqs()
        {
            irq_table_entry irq = irq_table[m_irq_state & 15];
            int priority = PSW & 0xe0;

            /* compare the priority of the interrupt to the PSW */
            if (irq.priority > priority)
            {
                int vector = irq.vector;
                int new_pc;
                int new_psw;

                /* call the callback; if we don't get -1 back, use the return value as our vector */
                int new_vector = standard_irq_callback(m_irq_state & 15);
                if (new_vector != -1)
                    vector = new_vector;

                /* fetch the new PC and PSW from that vector */
                assert((vector & 3) == 0);
                new_pc = RWORD(vector);
                new_psw = RWORD(vector + 2);

                /* push the old state, set the new one */
                PUSH(PSW);
                PUSH(PC);
                PCD = (uint32_t)new_pc;
                PSW = (uint8_t)new_psw;
                //t11_check_irqs();

                /* count cycles and clear the WAIT flag */
                m_icount.i -= 114;
                m_wait_state = 0;
            }
        }
    }


    //class k1801vm2_device : public t11_device


    //DECLARE_DEVICE_TYPE(T11,      t11_device)
    //DECLARE_DEVICE_TYPE(K1801VM2, k1801vm2_device)
}
