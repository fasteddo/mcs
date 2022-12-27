// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_read_line = mame.devcb_read<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_read_line = devcb_read<int, 1U>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using uint32_t = System.UInt32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;

using static mame.device_global;
using static mame.diexec_global;
using static mame.distate_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.i8085_global;


namespace mame
{
    public class i8085a_cpu_device : cpu_device
    {
        //DEFINE_DEVICE_TYPE(I8085A, i8085a_cpu_device, "i8085a", "Intel 8085A")
        public static readonly emu.detail.device_type_impl I8085A = DEFINE_DEVICE_TYPE("i8085a", "Intel 8085A", (type, mconfig, tag, owner, clock) => { return new i8085a_cpu_device(mconfig, tag, owner, clock); });


        class device_execute_interface_i8085a : device_execute_interface
        {
            public device_execute_interface_i8085a(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override u32 execute_min_cycles() { return ((i8085a_cpu_device)device()).device_execute_interface_execute_min_cycles(); }
            protected override u32 execute_max_cycles() { return ((i8085a_cpu_device)device()).device_execute_interface_execute_max_cycles(); }
            protected override u32 execute_input_lines() { return ((i8085a_cpu_device)device()).device_execute_interface_execute_input_lines(); }
            protected override uint32_t execute_default_irq_vector(int inputnum) { return ((i8085a_cpu_device)device()).device_execute_interface_execute_default_irq_vector(inputnum); }
            protected override bool execute_input_edge_triggered(int inputnum) { return ((i8085a_cpu_device)device()).device_execute_interface_execute_input_edge_triggered(inputnum); }
            protected override void execute_run() { ((i8085a_cpu_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { ((i8085a_cpu_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
            protected override u64 execute_clocks_to_cycles(u64 clocks) { return ((i8085a_cpu_device)device()).device_execute_interface_execute_clocks_to_cycles(clocks); }
            protected override u64 execute_cycles_to_clocks(u64 cycles) { return ((i8085a_cpu_device)device()).device_execute_interface_execute_cycles_to_clocks(cycles); }
        }


        public class device_memory_interface_i8085a : device_memory_interface
        {
            public device_memory_interface_i8085a(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((i8085a_cpu_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_i8085a : device_state_interface
        {
            public device_state_interface_i8085a(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void state_import(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_export(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }
        }


        public class device_disasm_interface_i8085a : device_disasm_interface
        {
            public device_disasm_interface_i8085a(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        const int VERBOSE = 0;
        //#include "logmacro.h"
        void LOG(string format, params object [] args) { logmacro_global.LOG(VERBOSE, this, format, args); }


        const int I8085_INTR_LINE     = 0;
        const int I8085_RST55_LINE    = 1;
        const int I8085_RST65_LINE    = 2;
        const int I8085_RST75_LINE    = 3;
        const int I8085_TRAP_LINE     = INPUT_LINE_NMI;


        const u8 SF  = 0x80;
        const u8 ZF  = 0x40;
        const u8 X5F = 0x20;
        const u8 HF  = 0x10;
        const u8 X3F = 0x08;
        const u8 PF  = 0x04;
        const u8 VF  = 0x02;
        const u8 CF  = 0x01;

        const u8 IM_SID = 0x80;
        const u8 IM_I75 = 0x40;
        const u8 IM_I65 = 0x20;
        const u8 IM_I55 = 0x10;
        const u8 IM_IE  = 0x08;
        const u8 IM_M75 = 0x04;
        const u8 IM_M65 = 0x02;
        const u8 IM_M55 = 0x01;

        const u16 ADDR_TRAP  = 0x0024;
        const u16 ADDR_RST55 = 0x002c;
        const u16 ADDR_RST65 = 0x0034;
        const u16 ADDR_RST75 = 0x003c;


        /* cycles lookup */
        static readonly u8 [] lut_cycles_8080 = { //[256]
        /*      0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F  */
        /* 0 */ 4, 10,7, 5, 5, 5, 7, 4, 4, 10,7, 5, 5, 5, 7, 4,
        /* 1 */ 4, 10,7, 5, 5, 5, 7, 4, 4, 10,7, 5, 5, 5, 7, 4,
        /* 2 */ 4, 10,16,5, 5, 5, 7, 4, 4, 10,16,5, 5, 5, 7, 4,
        /* 3 */ 4, 10,13,5, 10,10,10,4, 4, 10,13,5, 5, 5, 7, 4,
        /* 4 */ 5, 5, 5, 5, 5, 5, 7, 5, 5, 5, 5, 5, 5, 5, 7, 5,
        /* 5 */ 5, 5, 5, 5, 5, 5, 7, 5, 5, 5, 5, 5, 5, 5, 7, 5,
        /* 6 */ 5, 5, 5, 5, 5, 5, 7, 5, 5, 5, 5, 5, 5, 5, 7, 5,
        /* 7 */ 7, 7, 7, 7, 7, 7, 7, 7, 5, 5, 5, 5, 5, 5, 7, 5,
        /* 8 */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* 9 */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* A */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* B */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* C */ 5, 10,10,10,11,11,7, 11,5, 10,10,10,11,11,7, 11,
        /* D */ 5, 10,10,10,11,11,7, 11,5, 10,10,10,11,11,7, 11,
        /* E */ 5, 10,10,18,11,11,7, 11,5, 5, 10,5, 11,11,7, 11,
        /* F */ 5, 10,10,4, 11,11,7, 11,5, 5, 10,4, 11,11,7, 11 };

        static readonly u8 [] lut_cycles_8085 = { //[256]
        /*      0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F  */
        /* 0 */ 4, 10,7, 6, 4, 4, 7, 4, 10,10,7, 6, 4, 4, 7, 4,
        /* 1 */ 7, 10,7, 6, 4, 4, 7, 4, 10,10,7, 6, 4, 4, 7, 4,
        /* 2 */ 7, 10,16,6, 4, 4, 7, 4, 10,10,16,6, 4, 4, 7, 4,
        /* 3 */ 7, 10,13,6, 10,10,10,4, 10,10,13,6, 4, 4, 7, 4,
        /* 4 */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* 5 */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* 6 */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* 7 */ 7, 7, 7, 7, 7, 7, 5, 7, 4, 4, 4, 4, 4, 4, 7, 4,
        /* 8 */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* 9 */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* A */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* B */ 4, 4, 4, 4, 4, 4, 7, 4, 4, 4, 4, 4, 4, 4, 7, 4,
        /* C */ 6, 10,10,10,11,12,7, 12,6, 10,10,12,11,11,7, 12,
        /* D */ 6, 10,10,10,11,12,7, 12,6, 10,10,10,11,10,7, 12,
        /* E */ 6, 10,10,16,11,12,7, 12,6, 6, 10,5, 11,10,7, 12,
        /* F */ 6, 10,10,4, 11,12,7, 12,6, 6, 10,4, 11,10,7, 12 };


        // FIXME: public because drivers/altair.cpp, drivers/ipc.cpp and drivers/unior.cpp set initial PC through state interface
        // should fix boot vector loading in these drivers
        // machine/lviv.cpp and machine/poly88.cpp also access registers via state interface when handling snapshot files
        enum I8085
        {
            I8085_PC, I8085_SP, I8085_AF, I8085_BC, I8085_DE, I8085_HL,
            I8085_A, I8085_B, I8085_C, I8085_D, I8085_E, I8085_F, I8085_H, I8085_L,
            I8085_STATUS, I8085_SOD, I8085_SID, I8085_INTE,
            I8085_HALT, I8085_IM
        }


        protected enum CPUTYPE
        {
            CPUTYPE_8080 = 0,
            CPUTYPE_8080A,
            CPUTYPE_8085A
        }


        //static constexpr u8 STATUS_INTA   = 0x01;
        //static constexpr u8 STATUS_WO     = 0x02;
        //static constexpr u8 STATUS_STACK  = 0x04;
        //static constexpr u8 STATUS_HLTA   = 0x08;
        //static constexpr u8 STATUS_OUT    = 0x10;
        //static constexpr u8 STATUS_M1     = 0x20;
        //static constexpr u8 STATUS_INP    = 0x40;
        //static constexpr u8 STATUS_MEMR   = 0x80;


        device_memory_interface_i8085a m_dimemory;
        device_execute_interface_i8085a m_diexec;
        device_state_interface_i8085a m_distate;


        address_space_config m_program_config;
        address_space_config m_io_config;
        address_space_config m_opcode_config;

        devcb_read8 m_in_inta_func;
        devcb_write8 m_out_status_func;
        devcb_write_line m_out_inte_func;
        devcb_read_line m_in_sid_func;
        devcb_write_line m_out_sod_func;
        clock_update_delegate m_clk_out_func;


        int m_cputype;

        PAIR m_PC;
        PAIR m_SP;
        PAIR m_AF;
        PAIR m_BC;
        PAIR m_DE;
        PAIR m_HL;
        PAIR m_WZ;
        u8 m_halt;
        u8 m_im;             /* interrupt mask (8085A only) */
        u8 m_status;         /* status word */

        u8 m_after_ei;       /* post-EI processing; starts at 2, check for ints at 0 */
        u8 m_nmi_state;      /* raw NMI line state */
        u8 [] m_irq_state = new u8 [4];   /* raw IRQ line states */
        u8 m_trap_pending;   /* TRAP interrupt latched? */
        u8 m_trap_im_copy;   /* copy of IM register when TRAP was taken */
        u8 m_sod_state;      /* state of the SOD line */
        bool m_in_acknowledge;

        bool m_ietemp;       /* import/export temp space */

        memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.cache m_cprogram = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.cache();
        memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.cache m_copcodes = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.cache();
        memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.specific m_program = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.specific();
        memory_access<int_const_8,  int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.specific m_io = new memory_access<int_const_8, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.specific();
        intref m_icount = new intref();  //int m_icount;

        u8 [] lut_cycles = new u8 [256];
        /* flags lookup */
        u8 [] lut_zs = new u8 [256];
        u8 [] lut_zsp = new u8 [256];


        // construction/destruction
        protected i8085a_cpu_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : this(mconfig, I8085A, tag, owner, clock, (int)CPUTYPE.CPUTYPE_8085A)
        {
        }


        protected i8085a_cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock, int cputype)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_i8085a(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_i8085a(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_i8085a(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_i8085a(mconfig, this));
            m_dimemory = GetClassInterface<device_memory_interface_i8085a>();
            m_diexec = GetClassInterface<device_execute_interface_i8085a>();
            m_distate = GetClassInterface<device_state_interface_i8085a>();


            m_program_config = new address_space_config("program", ENDIANNESS_LITTLE, 8, 16, 0);
            m_io_config = new address_space_config("io", ENDIANNESS_LITTLE, 8, 8, 0);
            m_opcode_config = new address_space_config("opcodes", ENDIANNESS_LITTLE, 8, 16, 0);
            m_in_inta_func = new devcb_read8(this);
            m_out_status_func = new devcb_write8(this);
            m_out_inte_func = new devcb_write_line(this);
            m_in_sid_func = new devcb_read_line(this);
            m_out_sod_func = new devcb_write_line(this);
            m_clk_out_func = null;
            m_cputype = cputype;
        }


        // CLK rate callback (8085A only)
        //template <typename... T> void set_clk_out(T &&... args) { m_clk_out_func.set(std::forward<T>(args)...); }

        // INTA vector fetch callback
        //auto in_inta_func() { return m_in_inta_func.bind(); }

        // STATUS changed callback
        //auto out_status_func() { return m_out_status_func.bind(); }

        // INTE changed callback
        public devcb_write_line.binder out_inte_func() { return m_out_inte_func.bind(); }  //auto out_inte_func() { return m_out_inte_func.bind(); }

        // SID changed callback (8085A only)
        //auto in_sid_func() { return m_in_sid_func.bind(); }

        // SOD changed callback (8085A only)
        //auto out_sod_func() { return m_out_sod_func.bind(); }


        // device-level overrides
        protected override void device_config_complete()
        {
            //m_clk_out_func.resolve();
            if (m_clk_out_func != null)
                m_clk_out_func(clock() / 2);
        }


        protected override void device_clock_changed()
        {
            if (m_clk_out_func != null)
                m_clk_out_func(clock() / 2);
        }


        protected override void device_start()
        {
            m_PC.d = 0;
            m_SP.d = 0;
            m_AF.d = 0;
            m_BC.d = 0;
            m_DE.d = 0;
            m_HL.d = 0;
            m_WZ.d = 0;
            m_halt = 0;
            m_im = 0;
            m_status = 0;
            m_after_ei = 0;
            m_nmi_state = 0;
            m_irq_state[3] = m_irq_state[2] = m_irq_state[1] = m_irq_state[0] = 0;
            m_trap_pending = 0;
            m_trap_im_copy = 0;
            m_sod_state = 1; // SOD will go low at reset
            m_in_acknowledge = false;
            m_ietemp = false;

            init_tables();

            /* set up the state table */
            {
                m_distate.state_add((int)I8085.I8085_PC,     "PC",     m_PC.w.l);
                m_distate.state_add(STATE_GENPC,  "GENPC",  m_PC.w.l).noshow();
                m_distate.state_add(STATE_GENPCBASE, "CURPC", m_PC.w.l).noshow();
                m_distate.state_add((int)I8085.I8085_SP,     "SP",     m_SP.w.l);
                m_distate.state_add(STATE_GENFLAGS, "GENFLAGS", m_AF.b.l).noshow().formatstr("%8s");
                m_distate.state_add((int)I8085.I8085_A,      "A",      m_AF.b.h).noshow();
                m_distate.state_add((int)I8085.I8085_B,      "B",      m_BC.b.h).noshow();
                m_distate.state_add((int)I8085.I8085_C,      "C",      m_BC.b.l).noshow();
                m_distate.state_add((int)I8085.I8085_D,      "D",      m_DE.b.h).noshow();
                m_distate.state_add((int)I8085.I8085_E,      "E",      m_DE.b.l).noshow();
                m_distate.state_add((int)I8085.I8085_F,      "F",      m_AF.b.l).noshow();
                m_distate.state_add((int)I8085.I8085_H,      "H",      m_HL.b.h).noshow();
                m_distate.state_add((int)I8085.I8085_L,      "L",      m_HL.b.l).noshow();
                m_distate.state_add((int)I8085.I8085_AF,     "AF",     m_AF.w.l);
                m_distate.state_add((int)I8085.I8085_BC,     "BC",     m_BC.w.l);
                m_distate.state_add((int)I8085.I8085_DE,     "DE",     m_DE.w.l);
                m_distate.state_add((int)I8085.I8085_HL,     "HL",     m_HL.w.l);

                if (is_8080())
                {
                    m_distate.state_add((int)I8085.I8085_STATUS, "STATUS", m_status);
                    m_distate.state_add((int)I8085.I8085_INTE,   "INTE",   m_ietemp).mask(0x1).callimport().callexport();
                }
                if (is_8085())
                {
                    m_distate.state_add((int)I8085.I8085_IM,     "IM",     m_im);
                    m_distate.state_add((int)I8085.I8085_SOD,    "SOD",    m_sod_state).mask(0x1);
                    m_distate.state_add((int)I8085.I8085_SID,    "SID",    m_ietemp).mask(0x1).callimport().callexport();
                }
            }

            m_dimemory.space(AS_PROGRAM).cache(m_cprogram);
            m_dimemory.space(AS_PROGRAM).specific(m_program);
            m_dimemory.space(m_dimemory.has_space(AS_OPCODES) ? AS_OPCODES : AS_PROGRAM).cache(m_copcodes);
            m_dimemory.space(AS_IO).specific(m_io);

            /* resolve callbacks */
            m_in_inta_func.resolve();
            m_out_status_func.resolve_safe();
            m_out_inte_func.resolve_safe();
            m_in_sid_func.resolve_safe_s32(0);
            m_out_sod_func.resolve_safe();

            /* register for state saving */
            save_item(NAME(new { m_PC.w.l }));
            save_item(NAME(new { m_SP.w.l }));
            save_item(NAME(new { m_AF.w.l }));
            save_item(NAME(new { m_BC.w.l }));
            save_item(NAME(new { m_DE.w.l }));
            save_item(NAME(new { m_HL.w.l }));
            save_item(NAME(new { m_halt }));
            save_item(NAME(new { m_im }));
            save_item(NAME(new { m_status }));
            save_item(NAME(new { m_after_ei }));
            save_item(NAME(new { m_nmi_state }));
            save_item(NAME(new { m_irq_state }));
            save_item(NAME(new { m_trap_pending }));
            save_item(NAME(new { m_trap_im_copy }));
            save_item(NAME(new { m_sod_state }));
            save_item(NAME(new { m_in_acknowledge }));

            set_icountptr(m_icount);
        }


        protected override void device_reset()
        {
            m_PC.d = 0;
            m_halt = 0;
            m_im &= unchecked((u8)~IM_I75);
            m_im |= IM_M55 | IM_M65 | IM_M75;
            m_after_ei = 0;
            m_trap_pending = 0;
            m_trap_im_copy = 0;
            set_inte(0);
            set_sod(0);
        }


        // device_execute_interface overrides

        protected virtual u32 device_execute_interface_execute_min_cycles() { return 4; }
        protected virtual u32 device_execute_interface_execute_max_cycles() { return 16; }
        protected virtual u32 device_execute_interface_execute_input_lines() { return 4; }
        protected virtual uint32_t device_execute_interface_execute_default_irq_vector(int inputnum) { return 0xff; }
        protected virtual bool device_execute_interface_execute_input_edge_triggered(int inputnum) { return inputnum == I8085_RST75_LINE; }

        protected virtual void device_execute_interface_execute_run()  //virtual void execute_run() override;
        {
            /* check for TRAPs before diving in (can't do others because of after_ei) */
            if (m_trap_pending != 0 || m_after_ei == 0)
                check_for_interrupts();

            do
            {
                /* the instruction after an EI does not take an interrupt, so
                   we cannot check immediately; handle post-EI behavior here */
                if (m_after_ei != 0 && --m_after_ei == 0)
                    check_for_interrupts();

                m_in_acknowledge = false;
                debugger_instruction_hook(m_PC.d);

                /* here we go... */
                execute_one(read_op());

            } while (m_icount.i > 0);
        }

        protected virtual void device_execute_interface_execute_set_input(int irqline, int state)  //virtual void execute_set_input(int inputnum, int state) override;
        {
            int newstate = (state != CLEAR_LINE) ? 1 : 0;

            /* TRAP is level and edge-triggered NMI */
            if (irqline == I8085_TRAP_LINE)
            {
                if (m_nmi_state == 0 && newstate != 0)
                    m_trap_pending = 1;
                else if (newstate == 0)
                    m_trap_pending = 0;
                m_nmi_state = (u8)newstate;
            }

            /* RST7.5 is edge-triggered */
            else if (irqline == I8085_RST75_LINE)
            {
                if (m_irq_state[I8085_RST75_LINE] == 0 && newstate != 0)
                    m_im |= IM_I75;
                m_irq_state[I8085_RST75_LINE] = (u8)newstate;
            }

            /* remaining sources are level triggered */
            else if (irqline < (int)std.size(m_irq_state))
                m_irq_state[irqline] = (u8)state;
        }

        protected virtual u64 device_execute_interface_execute_clocks_to_cycles(u64 clocks) { return (clocks + 2 - 1) / 2; }
        protected virtual u64 device_execute_interface_execute_cycles_to_clocks(u64 cycles) { return (cycles * 2); }


        // device_memory_interface overrides
        protected virtual space_config_vector device_memory_interface_memory_space_config()  //virtual space_config_vector memory_space_config() const override;
        {
            return memory().has_configured_map(AS_OPCODES) ? new space_config_vector
            {
                std.make_pair(AS_PROGRAM, m_program_config),
                std.make_pair(AS_IO,      m_io_config),
                std.make_pair(AS_OPCODES, m_opcode_config)
            }
                                                  : new space_config_vector
            {
                std.make_pair(AS_PROGRAM, m_program_config),
                std.make_pair(AS_IO,      m_io_config)
            };
        }


        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        bool is_8080() { return m_cputype == (int)CPUTYPE.CPUTYPE_8080 || m_cputype == (int)CPUTYPE.CPUTYPE_8080A; }
        bool is_8085() { return m_cputype == (int)CPUTYPE.CPUTYPE_8085A; }


        void set_sod(int state)
        {
            if (state != 0 && m_sod_state == 0)
            {
                m_sod_state = 1;
                m_out_sod_func.op_u8(m_sod_state);
            }
            else if (state == 0 && m_sod_state != 0)
            {
                m_sod_state = 0;
                m_out_sod_func.op_u8(m_sod_state);
            }
        }

        void set_inte(int state)
        {
            if (state != 0 && (m_im & IM_IE) == 0)
            {
                m_im |= IM_IE;
                m_out_inte_func.op_s32(1);
            }
            else if (state == 0 && (m_im & IM_IE) != 0)
            {
                m_im &= unchecked((u8)~IM_IE);
                m_out_inte_func.op_s32(0);
            }
        }


        void set_status(u8 status)
        {
            if (status != m_status)
                m_out_status_func.op_u8(status);

            m_status = status;
        }


        u8 get_rim_value() { throw new emu_unimplemented(); }


        void break_halt_for_interrupt()
        {
            /* de-halt if necessary */
            if (m_halt != 0)
            {
                m_PC.w.l++;
                m_halt = 0;
                set_status(0x26); /* int ack while halt */
            }
            else
            {
                set_status(0x23); /* int ack */
            }

            m_in_acknowledge = true;
        }


        u8 read_op()
        {
            set_status(0xa2); // instruction fetch
            return m_copcodes.read_byte(m_PC.w.l++);
        }


        u8 read_inta()
        {
            if (m_in_inta_func.isnull())
                return (u8)standard_irq_callback(I8085_INTR_LINE);
            else
                return m_in_inta_func.op_u8(m_PC.w.l);
        }

        u8 read_arg()
        {
            set_status(0x82); // memory read
            if (m_in_acknowledge)
                return read_inta();
            else
                return m_cprogram.read_byte(m_PC.w.l++);
        }

        PAIR read_arg16()
        {
            PAIR p = default;
            set_status(0x82); // memory read
            if (m_in_acknowledge)
            {
                p.b.l = read_inta();
                p.b.h = read_inta();
            }
            else
            {
                p.b.l = m_cprogram.read_byte(m_PC.w.l++);
                p.b.h = m_cprogram.read_byte(m_PC.w.l++);
            }
            return p;
        }

        u8 read_mem(u32 a)
        {
            set_status(0x82); // memory read
            return m_program.read_byte(a);
        }

        void write_mem(u32 a, u8 v)
        {
            set_status(0x00); // memory write
            m_program.write_byte(a, v);
        }

        void op_push(PAIR p)
        {
            set_status(0x04); // stack push
            m_program.write_byte(--m_SP.w.l, p.b.h);
            m_program.write_byte(--m_SP.w.l, p.b.l);
        }

        PAIR op_pop()
        {
            PAIR p = default;
            set_status(0x86); // stack pop
            p.b.l = m_program.read_byte(m_SP.w.l++);
            p.b.h = m_program.read_byte(m_SP.w.l++);
            return p;
        }


        void check_for_interrupts()
        {
            /* TRAP is the highest priority */
            if (m_trap_pending != 0)
            {
                /* the first RIM after a TRAP reflects the original IE state; remember it here,
                   setting the high bit to indicate it is valid */
                m_trap_im_copy = (u8)(m_im | 0x80);

                /* reset the pending state */
                m_trap_pending = 0;

                /* break out of HALT state and call the IRQ ack callback */
                break_halt_for_interrupt();
                standard_irq_callback(I8085_TRAP_LINE);

                /* push the PC and jump to $0024 */
                op_push(m_PC);
                set_inte(0);
                m_PC.w.l = ADDR_TRAP;
                m_icount.i -= 11;
            }

            /* followed by RST7.5 */
            else if ((m_im & IM_I75) != 0 && (m_im & IM_M75) == 0 && (m_im & IM_IE) != 0)
            {
                /* reset the pending state (which is CPU-visible via the RIM instruction) */
                m_im &= unchecked((u8)~IM_I75);

                /* break out of HALT state and call the IRQ ack callback */
                break_halt_for_interrupt();
                standard_irq_callback(I8085_RST75_LINE);

                /* push the PC and jump to $003C */
                op_push(m_PC);
                set_inte(0);
                m_PC.w.l = ADDR_RST75;
                m_icount.i -= 11;
            }

            /* followed by RST6.5 */
            else if (m_irq_state[I8085_RST65_LINE] != 0 && (m_im & IM_M65) == 0 && (m_im & IM_IE) != 0)
            {
                /* break out of HALT state and call the IRQ ack callback */
                break_halt_for_interrupt();
                standard_irq_callback(I8085_RST65_LINE);

                /* push the PC and jump to $0034 */
                op_push(m_PC);
                set_inte(0);
                m_PC.w.l = ADDR_RST65;
                m_icount.i -= 11;
            }

            /* followed by RST5.5 */
            else if (m_irq_state[I8085_RST55_LINE] != 0 && (m_im & IM_M55) == 0 && (m_im & IM_IE) != 0)
            {
                /* break out of HALT state and call the IRQ ack callback */
                break_halt_for_interrupt();
                standard_irq_callback(I8085_RST55_LINE);

                /* push the PC and jump to $002C */
                op_push(m_PC);
                set_inte(0);
                m_PC.w.l = ADDR_RST55;
                m_icount.i -= 11;
            }

            /* followed by classic INTR */
            else if (m_irq_state[I8085_INTR_LINE] != 0 && (m_im & IM_IE) != 0)
            {
                /* break out of HALT state and call the IRQ ack callback */
                if (!m_in_inta_func.isnull())
                    standard_irq_callback(I8085_INTR_LINE);
                break_halt_for_interrupt();

                u8 vector = read_inta();

                /* use the resulting vector as an opcode to execute */
                set_inte(0);
                LOG("i8085 take int {0}\n", vector);
                execute_one(vector);
            }
        }


        void execute_one(int opcode)
        {
            m_icount.i -= lut_cycles[opcode];

            switch (opcode)
            {
                case 0x00: // NOP
                    break;
                case 0x01: // LXI B,nnnn
                    m_BC = read_arg16();
                    break;
                case 0x02: // STAX B
                    write_mem(m_BC.d, m_AF.b.h);
                    break;
                case 0x03: // INX B
                    m_BC.w.l++;
                    if (is_8085())
                    {
                        if (m_BC.w.l == 0x0000)
                            m_AF.b.l |= X5F;
                        else
                            m_AF.b.l &= unchecked((u8)~X5F);
                    }
                    break;
                case 0x04: // INR B
                    m_BC.b.h = op_inr(m_BC.b.h);
                    break;
                case 0x05: // DCR B
                    m_BC.b.h = op_dcr(m_BC.b.h);
                    break;
                case 0x06: // MVI B,nn
                    m_BC.b.h = read_arg();
                    break;
                case 0x07: // RLC
                    m_AF.b.h = (u8)((m_AF.b.h << 1) | (m_AF.b.h >> 7));
                    m_AF.b.l = (u8)((m_AF.b.l & 0xfe) | (m_AF.b.h & CF));
                    break;

                case 0x08: // 8085: DSUB, otherwise undocumented NOP
                    if (is_8085())
                    {
                        int q = m_HL.b.l - m_BC.b.l;
                        m_AF.b.l = (u8)(lut_zs[q & 0xff] | ((q >> 8) & CF) | VF | ((m_HL.b.l ^ q ^ m_BC.b.l) & HF) | (((m_BC.b.l ^ m_HL.b.l) & (m_HL.b.l ^ q) & SF) >> 5));
                        m_HL.b.l = (u8)q;
                        q = m_HL.b.h - m_BC.b.h - (m_AF.b.l & CF);
                        m_AF.b.l = (u8)(lut_zs[q & 0xff] | ((q >> 8) & CF) | VF | ((m_HL.b.h ^ q ^ m_BC.b.h) & HF) | (((m_BC.b.h ^ m_HL.b.h) & (m_HL.b.h ^ q) & SF) >> 5));
                        if (m_HL.b.l != 0)
                            m_AF.b.l &= unchecked((u8)~ZF);
                    }
                    break;
                case 0x09: // DAD B
                    op_dad(m_BC.w.l);
                    break;
                case 0x0a: // LDAX B
                    m_AF.b.h = read_mem(m_BC.d);
                    break;
                case 0x0b: // DCX B
                    m_BC.w.l--;
                    if (is_8085())
                    {
                        if (m_BC.w.l == 0xffff)
                            m_AF.b.l |= X5F;
                        else
                            m_AF.b.l &= unchecked((u8)~X5F);
                    }
                    break;
                case 0x0c: // INR C
                    m_BC.b.l = op_inr(m_BC.b.l);
                    break;
                case 0x0d: // DCR C
                    m_BC.b.l = op_dcr(m_BC.b.l);
                    break;
                case 0x0e: // MVI C,nn
                    m_BC.b.l = read_arg();
                    break;
                case 0x0f: // RRC
                    m_AF.b.l = (u8)((m_AF.b.l & 0xfe) | (m_AF.b.h & CF));
                    m_AF.b.h = (u8)((m_AF.b.h >> 1) | (m_AF.b.h << 7));
                    break;

                case 0x10: // 8085: ASRH, otherwise undocumented NOP
                    if (is_8085())
                    {
                        m_AF.b.l = (u8)((m_AF.b.l & ~CF) | (m_HL.b.l & CF));
                        m_HL.w.l = (u8)(m_HL.w.l >> 1);
                    }
                    break;
                case 0x11: // LXI D,nnnn
                    m_DE = read_arg16();
                    break;
                case 0x12: // STAX D
                    write_mem(m_DE.d, m_AF.b.h);
                    break;
                case 0x13: // INX D
                    m_DE.w.l++;
                    if (is_8085())
                    {
                        if (m_DE.w.l == 0x0000)
                            m_AF.b.l |= X5F;
                        else
                            m_AF.b.l &= unchecked((u8)~X5F);
                    }
                    break;
                case 0x14: // INR D
                    m_DE.b.h = op_inr(m_DE.b.h);
                    break;
                case 0x15: // DCR D
                    m_DE.b.h = op_dcr(m_DE.b.h);
                    break;
                case 0x16: // MVI D,nn
                    m_DE.b.h = read_arg();
                    break;
                case 0x17: // RAL
                {
                    int c = m_AF.b.l & CF;
                    m_AF.b.l = (u8)((m_AF.b.l & 0xfe) | (m_AF.b.h >> 7));
                    m_AF.b.h = (u8)((m_AF.b.h << 1) | c);
                    break;
                }

                case 0x18: // 8085: RLDE, otherwise undocumented NOP
                    if (is_8085())
                    {
                        m_AF.b.l = (u8)((m_AF.b.l & ~(CF | VF)) | (m_DE.b.h >> 7));
                        m_DE.w.l = (u8)((m_DE.w.l << 1) | (m_DE.w.l >> 15));
                        if ((((m_DE.w.l >> 15) ^ m_AF.b.l) & CF) != 0)
                            m_AF.b.l |= VF;
                    }
                    break;
                case 0x19: // DAD D
                    op_dad(m_DE.w.l);
                    break;
                case 0x1a: // LDAX D
                    m_AF.b.h = read_mem(m_DE.d);
                    break;
                case 0x1b: // DCX D
                    m_DE.w.l--;
                    if (is_8085())
                    {
                        if (m_DE.w.l == 0xffff)
                            m_AF.b.l |= X5F;
                        else
                            m_AF.b.l &= unchecked((u8)~X5F);
                    }
                    break;
                case 0x1c: // INR E
                    m_DE.b.l = op_inr(m_DE.b.l);
                    break;
                case 0x1d: // DCR E
                    m_DE.b.l = op_dcr(m_DE.b.l);
                    break;
                case 0x1e: // MVI E,nn
                    m_DE.b.l = read_arg();
                    break;
                case 0x1f: // RAR
                {
                    int c = (m_AF.b.l & CF) << 7;
                    m_AF.b.l = (u8)((m_AF.b.l & 0xfe) | (m_AF.b.h & CF));
                    m_AF.b.h = (u8)((m_AF.b.h >> 1) | c);
                    break;
                }

                case 0x20: // 8085: RIM, otherwise undocumented NOP
                    if (is_8085())
                    {
                        m_AF.b.h = get_rim_value();

                        // if we have remembered state from taking a TRAP, fix up the IE flag here
                        if ((m_trap_im_copy & 0x80) != 0)
                            m_AF.b.h = (u8)((m_AF.b.h & ~IM_IE) | (m_trap_im_copy & IM_IE));
                        m_trap_im_copy = 0;
                    }
                    break;
                case 0x21: // LXI H,nnnn
                    m_HL = read_arg16();
                    break;
                case 0x22: // SHLD nnnn
                    m_WZ = read_arg16();
                    write_mem(m_WZ.d, m_HL.b.l);
                    m_WZ.w.l++;
                    write_mem(m_WZ.d, m_HL.b.h);
                    break;
                case 0x23: // INX H
                    m_HL.w.l++;
                    if (is_8085())
                    {
                        if (m_HL.w.l == 0x0000)
                            m_AF.b.l |= X5F;
                        else
                            m_AF.b.l &= unchecked((u8)~X5F);
                    }
                    break;
                case 0x24: // INR H
                    m_HL.b.h = op_inr(m_HL.b.h);
                    break;
                case 0x25: // DCR H
                    m_HL.b.h = op_dcr(m_HL.b.h);
                    break;
                case 0x26: // MVI H,nn
                    m_HL.b.h = read_arg();
                    break;
                case 0x27: // DAA
                    m_WZ.b.h = m_AF.b.h;
                    if (is_8085() && (m_AF.b.l & VF) != 0)
                    {
                        if ((m_AF.b.l & HF) != 0 || ((m_AF.b.h & 0xf) > 9))
                            m_WZ.b.h -= 6;
                        if ((m_AF.b.l & CF) != 0 || (m_AF.b.h > 0x99))
                            m_WZ.b.h -= 0x60;
                    }
                    else
                    {
                        if ((m_AF.b.l & HF) != 0 || ((m_AF.b.h & 0xf) > 9))
                            m_WZ.b.h += 6;
                        if ((m_AF.b.l & CF) != 0 || (m_AF.b.h > 0x99))
                            m_WZ.b.h += 0x60;
                    }

                    m_AF.b.l = (u8)((m_AF.b.l & 3) | (m_AF.b.h & 0x28) | ((m_AF.b.h > 0x99) ? 1 : 0) | ((m_AF.b.h ^ m_WZ.b.h) & 0x10) | lut_zsp[m_WZ.b.h]);
                    m_AF.b.h = m_WZ.b.h;
                    break;

                case 0x28: // 8085: LDEH nn, otherwise undocumented NOP
                    if (is_8085())
                    {
                        m_WZ.d = read_arg();
                        m_DE.d = (m_HL.d + m_WZ.d) & 0xffff;
                    }
                    break;
                case 0x29: // DAD H
                    op_dad(m_HL.w.l);
                    break;
                case 0x2a: // LHLD nnnn
                    m_WZ = read_arg16();
                    m_HL.b.l = read_mem(m_WZ.d);
                    m_WZ.w.l++;
                    m_HL.b.h = read_mem(m_WZ.d);
                    break;
                case 0x2b: // DCX H
                    m_HL.w.l--;
                    if (is_8085())
                    {
                        if (m_HL.w.l == 0xffff)
                            m_AF.b.l |= X5F;
                        else
                            m_AF.b.l &= unchecked((u8)~X5F);
                    }
                    break;
                case 0x2c: // INR L
                    m_HL.b.l = op_inr(m_HL.b.l);
                    break;
                case 0x2d: // DCR L
                    m_HL.b.l = op_dcr(m_HL.b.l);
                    break;
                case 0x2e: // MVI L,nn
                    m_HL.b.l = read_arg();
                    break;
                case 0x2f: // CMA
                    m_AF.b.h ^= 0xff;
                    if (is_8085())
                        m_AF.b.l |= HF | VF;
                    break;

                case 0x30: // 8085: SIM, otherwise undocumented NOP
                    if (is_8085())
                    {
                        // if bit 3 is set, bits 0-2 become the new masks
                        if ((m_AF.b.h & 0x08) != 0)
                        {
                            m_im &= unchecked((u8)~(IM_M55 | IM_M65 | IM_M75 | IM_I55 | IM_I65));
                            m_im |= (u8)(m_AF.b.h & (IM_M55 | IM_M65 | IM_M75));

                            // update live state based on the new masks
                            if ((m_im & IM_M55) == 0 && m_irq_state[I8085_RST55_LINE] != 0)
                                m_im |= IM_I55;
                            if ((m_im & IM_M65) == 0 && m_irq_state[I8085_RST65_LINE] != 0)
                                m_im |= IM_I65;
                        }

                        // bit if 4 is set, the 7.5 flip-flop is cleared
                        if ((m_AF.b.h & 0x10) != 0)
                            m_im &= unchecked((u8)~IM_I75);

                        // if bit 6 is set, then bit 7 is the new SOD state
                        if ((m_AF.b.h & 0x40) != 0)
                            set_sod(m_AF.b.h >> 7);

                        // check for revealed interrupts
                        check_for_interrupts();
                    }
                    break;
                case 0x31: // LXI SP,nnnn
                    m_SP = read_arg16();
                    break;
                case 0x32: // STAX nnnn
                    m_WZ = read_arg16();
                    write_mem(m_WZ.d, m_AF.b.h);
                    break;
                case 0x33: // INX SP
                    m_SP.w.l++;
                    if (is_8085())
                    {
                        if (m_SP.w.l == 0x0000)
                            m_AF.b.l |= X5F;
                        else
                            m_AF.b.l &= unchecked((u8)~X5F);
                    }
                    break;
                case 0x34: // INR M
                    m_WZ.b.l = op_inr(read_mem(m_HL.d));
                    write_mem(m_HL.d, m_WZ.b.l);
                    break;
                case 0x35: // DCR M
                    m_WZ.b.l = op_dcr(read_mem(m_HL.d));
                    write_mem(m_HL.d, m_WZ.b.l);
                    break;
                case 0x36: // MVI M,nn
                    m_WZ.b.l = read_arg();
                    write_mem(m_HL.d, m_WZ.b.l);
                    break;
                case 0x37: // STC
                    m_AF.b.l = (u8)((m_AF.b.l & 0xfe) | CF);
                    break;

                case 0x38: // 8085: LDES nn, otherwise undocumented NOP
                    if (is_8085())
                    {
                        m_WZ.d = read_arg();
                        m_DE.d = (m_SP.d + m_WZ.d) & 0xffff;
                    }
                    break;
                case 0x39: // DAD SP
                    op_dad(m_SP.w.l);
                    break;
                case 0x3a: // LDAX nnnn
                    m_WZ = read_arg16();
                    m_AF.b.h = read_mem(m_WZ.d);
                    break;
                case 0x3b: // DCX SP
                    m_SP.w.l--;
                    if (is_8085())
                    {
                        if (m_SP.w.l == 0xffff)
                            m_AF.b.l |= X5F;
                        else
                            m_AF.b.l &= unchecked((u8)~X5F);
                    }
                    break;
                case 0x3c: // INR A
                    m_AF.b.h = op_inr(m_AF.b.h);
                    break;
                case 0x3d: // DCR A
                    m_AF.b.h = op_dcr(m_AF.b.h);
                    break;
                case 0x3e: // MVI A,nn
                    m_AF.b.h = read_arg();
                    break;
                case 0x3f: // CMC
                    m_AF.b.l = (u8)((m_AF.b.l & 0xfe) | (~m_AF.b.l & CF));
                    break;

                // MOV [B/C/D/E/H/L/M/A],[B/C/D/E/H/L/M/A]
                case 0x40: break; // MOV B,B
                case 0x41: m_BC.b.h = m_BC.b.l; break;
                case 0x42: m_BC.b.h = m_DE.b.h; break;
                case 0x43: m_BC.b.h = m_DE.b.l; break;
                case 0x44: m_BC.b.h = m_HL.b.h; break;
                case 0x45: m_BC.b.h = m_HL.b.l; break;
                case 0x46: m_BC.b.h = read_mem(m_HL.d); break;
                case 0x47: m_BC.b.h = m_AF.b.h; break;

                case 0x48: m_BC.b.l = m_BC.b.h; break;
                case 0x49: break; // MOV C,C
                case 0x4a: m_BC.b.l = m_DE.b.h; break;
                case 0x4b: m_BC.b.l = m_DE.b.l; break;
                case 0x4c: m_BC.b.l = m_HL.b.h; break;
                case 0x4d: m_BC.b.l = m_HL.b.l; break;
                case 0x4e: m_BC.b.l = read_mem(m_HL.d); break;
                case 0x4f: m_BC.b.l = m_AF.b.h; break;

                case 0x50: m_DE.b.h = m_BC.b.h; break;
                case 0x51: m_DE.b.h = m_BC.b.l; break;
                case 0x52: break; // MOV D,D
                case 0x53: m_DE.b.h = m_DE.b.l; break;
                case 0x54: m_DE.b.h = m_HL.b.h; break;
                case 0x55: m_DE.b.h = m_HL.b.l; break;
                case 0x56: m_DE.b.h = read_mem(m_HL.d); break;
                case 0x57: m_DE.b.h = m_AF.b.h; break;

                case 0x58: m_DE.b.l = m_BC.b.h; break;
                case 0x59: m_DE.b.l = m_BC.b.l; break;
                case 0x5a: m_DE.b.l = m_DE.b.h; break;
                case 0x5b: break; // MOV E,E
                case 0x5c: m_DE.b.l = m_HL.b.h; break;
                case 0x5d: m_DE.b.l = m_HL.b.l; break;
                case 0x5e: m_DE.b.l = read_mem(m_HL.d); break;
                case 0x5f: m_DE.b.l = m_AF.b.h; break;

                case 0x60: m_HL.b.h = m_BC.b.h; break;
                case 0x61: m_HL.b.h = m_BC.b.l; break;
                case 0x62: m_HL.b.h = m_DE.b.h; break;
                case 0x63: m_HL.b.h = m_DE.b.l; break;
                case 0x64: break; // MOV H,H
                case 0x65: m_HL.b.h = m_HL.b.l; break;
                case 0x66: m_HL.b.h = read_mem(m_HL.d); break;
                case 0x67: m_HL.b.h = m_AF.b.h; break;

                case 0x68: m_HL.b.l = m_BC.b.h; break;
                case 0x69: m_HL.b.l = m_BC.b.l; break;
                case 0x6a: m_HL.b.l = m_DE.b.h; break;
                case 0x6b: m_HL.b.l = m_DE.b.l; break;
                case 0x6c: m_HL.b.l = m_HL.b.h; break;
                case 0x6d: break; // MOV L,L
                case 0x6e: m_HL.b.l = read_mem(m_HL.d); break;
                case 0x6f: m_HL.b.l = m_AF.b.h; break;

                case 0x70: write_mem(m_HL.d, m_BC.b.h); break;
                case 0x71: write_mem(m_HL.d, m_BC.b.l); break;
                case 0x72: write_mem(m_HL.d, m_DE.b.h); break;
                case 0x73: write_mem(m_HL.d, m_DE.b.l); break;
                case 0x74: write_mem(m_HL.d, m_HL.b.h); break;
                case 0x75: write_mem(m_HL.d, m_HL.b.l); break;
                case 0x76: // HLT (instead of MOV M,M)
                    m_PC.w.l--;
                    m_halt = 1;
                    set_status(0x8a); // halt acknowledge
                    break;
                case 0x77: write_mem(m_HL.d, m_AF.b.h); break;

                case 0x78: m_AF.b.h = m_BC.b.h; break;
                case 0x79: m_AF.b.h = m_BC.b.l; break;
                case 0x7a: m_AF.b.h = m_DE.b.h; break;
                case 0x7b: m_AF.b.h = m_DE.b.l; break;
                case 0x7c: m_AF.b.h = m_HL.b.h; break;
                case 0x7d: m_AF.b.h = m_HL.b.l; break;
                case 0x7e: m_AF.b.h = read_mem(m_HL.d); break;
                case 0x7f: break; // MOV A,A

                // alu op [B/C/D/E/H/L/M/A]
                case 0x80: op_add(m_BC.b.h); break;
                case 0x81: op_add(m_BC.b.l); break;
                case 0x82: op_add(m_DE.b.h); break;
                case 0x83: op_add(m_DE.b.l); break;
                case 0x84: op_add(m_HL.b.h); break;
                case 0x85: op_add(m_HL.b.l); break;
                case 0x86: m_WZ.b.l = read_mem(m_HL.d); op_add(m_WZ.b.l); break;
                case 0x87: op_add(m_AF.b.h); break;

                case 0x88: op_adc(m_BC.b.h); break;
                case 0x89: op_adc(m_BC.b.l); break;
                case 0x8a: op_adc(m_DE.b.h); break;
                case 0x8b: op_adc(m_DE.b.l); break;
                case 0x8c: op_adc(m_HL.b.h); break;
                case 0x8d: op_adc(m_HL.b.l); break;
                case 0x8e: m_WZ.b.l = read_mem(m_HL.d); op_adc(m_WZ.b.l); break;
                case 0x8f: op_adc(m_AF.b.h); break;

                case 0x90: op_sub(m_BC.b.h); break;
                case 0x91: op_sub(m_BC.b.l); break;
                case 0x92: op_sub(m_DE.b.h); break;
                case 0x93: op_sub(m_DE.b.l); break;
                case 0x94: op_sub(m_HL.b.h); break;
                case 0x95: op_sub(m_HL.b.l); break;
                case 0x96: m_WZ.b.l = read_mem(m_HL.d); op_sub(m_WZ.b.l); break;
                case 0x97: op_sub(m_AF.b.h); break;

                case 0x98: op_sbb(m_BC.b.h); break;
                case 0x99: op_sbb(m_BC.b.l); break;
                case 0x9a: op_sbb(m_DE.b.h); break;
                case 0x9b: op_sbb(m_DE.b.l); break;
                case 0x9c: op_sbb(m_HL.b.h); break;
                case 0x9d: op_sbb(m_HL.b.l); break;
                case 0x9e: m_WZ.b.l = read_mem(m_HL.d); op_sbb(m_WZ.b.l); break;
                case 0x9f: op_sbb(m_AF.b.h); break;

                case 0xa0: op_ana(m_BC.b.h); break;
                case 0xa1: op_ana(m_BC.b.l); break;
                case 0xa2: op_ana(m_DE.b.h); break;
                case 0xa3: op_ana(m_DE.b.l); break;
                case 0xa4: op_ana(m_HL.b.h); break;
                case 0xa5: op_ana(m_HL.b.l); break;
                case 0xa6: m_WZ.b.l = read_mem(m_HL.d); op_ana(m_WZ.b.l); break;
                case 0xa7: op_ana(m_AF.b.h); break;

                case 0xa8: op_xra(m_BC.b.h); break;
                case 0xa9: op_xra(m_BC.b.l); break;
                case 0xaa: op_xra(m_DE.b.h); break;
                case 0xab: op_xra(m_DE.b.l); break;
                case 0xac: op_xra(m_HL.b.h); break;
                case 0xad: op_xra(m_HL.b.l); break;
                case 0xae: m_WZ.b.l = read_mem(m_HL.d); op_xra(m_WZ.b.l); break;
                case 0xaf: op_xra(m_AF.b.h); break;

                case 0xb0: op_ora(m_BC.b.h); break;
                case 0xb1: op_ora(m_BC.b.l); break;
                case 0xb2: op_ora(m_DE.b.h); break;
                case 0xb3: op_ora(m_DE.b.l); break;
                case 0xb4: op_ora(m_HL.b.h); break;
                case 0xb5: op_ora(m_HL.b.l); break;
                case 0xb6: m_WZ.b.l = read_mem(m_HL.d); op_ora(m_WZ.b.l); break;
                case 0xb7: op_ora(m_AF.b.h); break;

                case 0xb8: op_cmp(m_BC.b.h); break;
                case 0xb9: op_cmp(m_BC.b.l); break;
                case 0xba: op_cmp(m_DE.b.h); break;
                case 0xbb: op_cmp(m_DE.b.l); break;
                case 0xbc: op_cmp(m_HL.b.h); break;
                case 0xbd: op_cmp(m_HL.b.l); break;
                case 0xbe: m_WZ.b.l = read_mem(m_HL.d); op_cmp(m_WZ.b.l); break;
                case 0xbf: op_cmp(m_AF.b.h); break;

                case 0xc0: // RNZ
                    op_ret((m_AF.b.l & ZF) == 0 ? 1 : 0);
                    break;
                case 0xc1: // POP B
                    m_BC = op_pop();
                    break;
                case 0xc2: // JNZ nnnn
                    op_jmp((m_AF.b.l & ZF) == 0 ? 1 : 0);
                    break;
                case 0xc3: // JMP nnnn
                    op_jmp(1);
                    break;
                case 0xc4: // CNZ nnnn
                    op_call((m_AF.b.l & ZF) == 0 ? 1 : 0);
                    break;
                case 0xc5: // PUSH B
                    op_push(m_BC);
                    break;
                case 0xc6: // ADI nn
                    m_WZ.b.l = read_arg();
                    op_add(m_WZ.b.l);
                    break;
                case 0xc7: // RST 0
                    op_rst(0);
                    break;

                case 0xc8: // RZ
                    op_ret(m_AF.b.l & ZF);
                    break;
                case 0xc9: // RET
                    m_PC = op_pop();
                    break;
                case 0xca: // JZ  nnnn
                    op_jmp(m_AF.b.l & ZF);
                    break;
                case 0xcb: // 8085: RST V, otherwise undocumented JMP nnnn
                    if (is_8085())
                    {
                        if ((m_AF.b.l & VF) != 0)
                            op_rst(8);
                        else
                            m_icount.i += 6; // RST not taken
                    }
                    else
                        op_jmp(1);
                    break;
                case 0xcc: // CZ  nnnn
                    op_call(m_AF.b.l & ZF);
                    break;
                case 0xcd: // CALL nnnn
                    op_call(1);
                    break;
                case 0xce: // ACI nn
                    m_WZ.b.l = read_arg();
                    op_adc(m_WZ.b.l);
                    break;
                case 0xcf: // RST 1
                    op_rst(1);
                    break;

                case 0xd0: // RNC
                    op_ret((m_AF.b.l & CF) == 0 ? 1 : 0);
                    break;
                case 0xd1: // POP D
                    m_DE = op_pop();
                    break;
                case 0xd2: // JNC nnnn
                    op_jmp((m_AF.b.l & CF) == 0 ? 1 : 0);
                    break;
                case 0xd3: // OUT nn
                    set_status(0x10);
                    m_WZ.d = read_arg();
                    m_io.write_byte(m_WZ.d, m_AF.b.h);
                    break;
                case 0xd4: // CNC nnnn
                    op_call((m_AF.b.l & CF) == 0 ? 1 : 0);
                    break;
                case 0xd5: // PUSH D
                    op_push(m_DE);
                    break;
                case 0xd6: // SUI nn
                    m_WZ.b.l = read_arg();
                    op_sub(m_WZ.b.l);
                    break;
                case 0xd7: // RST 2
                    op_rst(2);
                    break;

                case 0xd8: // RC
                    op_ret(m_AF.b.l & CF);
                    break;
                case 0xd9: // 8085: SHLX, otherwise undocumented RET
                    if (is_8085())
                    {
                        m_WZ.w.l = m_DE.w.l;
                        write_mem(m_WZ.d, m_HL.b.l);
                        m_WZ.w.l++;
                        write_mem(m_WZ.d, m_HL.b.h);
                    }
                    else
                        m_PC = op_pop();
                    break;
                case 0xda: // JC nnnn
                    op_jmp(m_AF.b.l & CF);
                    break;
                case 0xdb: // IN nn
                    set_status(0x42);
                    m_WZ.d = read_arg();
                    m_AF.b.h = m_io.read_byte(m_WZ.d);
                    break;
                case 0xdc: // CC nnnn
                    op_call(m_AF.b.l & CF);
                    break;
                case 0xdd: // 8085: JNX nnnn, otherwise undocumented CALL nnnn
                    if (is_8085())
                        op_jmp((m_AF.b.l & X5F) == 0 ? 1 : 0);
                    else
                        op_call(1);
                    break;
                case 0xde: // SBI nn
                    m_WZ.b.l = read_arg();
                    op_sbb(m_WZ.b.l);
                    break;
                case 0xdf: // RST 3
                    op_rst(3);
                    break;

                case 0xe0: // RPO
                    op_ret((m_AF.b.l & PF) == 0 ? 1 : 0);
                    break;
                case 0xe1: // POP H
                    m_HL = op_pop();
                    break;
                case 0xe2: // JPO nnnn
                    op_jmp((m_AF.b.l & PF) == 0 ? 1 : 0);
                    break;
                case 0xe3: // XTHL
                    m_WZ = op_pop();
                    op_push(m_HL);
                    m_HL.d = m_WZ.d;
                    break;
                case 0xe4: // CPO nnnn
                    op_call((m_AF.b.l & PF) == 0 ? 1 : 0);
                    break;
                case 0xe5: // PUSH H
                    op_push(m_HL);
                    break;
                case 0xe6: // ANI nn
                    m_WZ.b.l = read_arg();
                    op_ana(m_WZ.b.l);
                    break;
                case 0xe7: // RST 4
                    op_rst(4);
                    break;

                case 0xe8: // RPE
                    op_ret(m_AF.b.l & PF);
                    break;
                case 0xe9: // PCHL
                    m_PC.d = m_HL.w.l;
                    break;
                case 0xea: // JPE nnnn
                    op_jmp(m_AF.b.l & PF);
                    break;
                case 0xeb: // XCHG
                    m_WZ.d = m_DE.d;
                    m_DE.d = m_HL.d;
                    m_HL.d = m_WZ.d;
                    break;
                case 0xec: // CPE nnnn
                    op_call(m_AF.b.l & PF);
                    break;
                case 0xed: // 8085: LHLX, otherwise undocumented CALL nnnn
                    if (is_8085())
                    {
                        m_WZ.w.l = m_DE.w.l;
                        m_HL.b.l = read_mem(m_WZ.d);
                        m_WZ.w.l++;
                        m_HL.b.h = read_mem(m_WZ.d);
                    }
                    else
                        op_call(1);
                    break;
                case 0xee: // XRI nn
                    m_WZ.b.l = read_arg();
                    op_xra(m_WZ.b.l);
                    break;
                case 0xef: // RST 5
                    op_rst(5);
                    break;

                case 0xf0: // RP
                    op_ret((m_AF.b.l & SF) == 0 ? 1 : 0);
                    break;
                case 0xf1: // POP A
                    m_AF = op_pop();
                    break;
                case 0xf2: // JP nnnn
                    op_jmp((m_AF.b.l & SF) == 0 ? 1 : 0);
                    break;
                case 0xf3: // DI
                    set_inte(0);
                    break;
                case 0xf4: // CP nnnn
                    op_call((m_AF.b.l & SF) == 0 ? 1 : 0);
                    break;
                case 0xf5: // PUSH A
                    // on 8080, VF=1 and X3F=0 and X5F=0 always! (we don't have to check for it elsewhere)
                    if (is_8080())
                        m_AF.b.l = (u8)((m_AF.b.l & ~(X3F | X5F)) | VF);
                    op_push(m_AF);
                    break;
                case 0xf6: // ORI nn
                    m_WZ.b.l = read_arg();
                    op_ora(m_WZ.b.l);
                    break;
                case 0xf7: // RST 6
                    op_rst(6);
                    break;

                case 0xf8: // RM
                    op_ret(m_AF.b.l & SF);
                    break;
                case 0xf9: // SPHL
                    m_SP.d = m_HL.d;
                    break;
                case 0xfa: // JM nnnn
                    op_jmp(m_AF.b.l & SF);
                    break;
                case 0xfb: // EI
                    set_inte(1);
                    m_after_ei = 2;
                    break;
                case 0xfc: // CM nnnn
                    op_call(m_AF.b.l & SF);
                    break;
                case 0xfd: // 8085: JX nnnn, otherwise undocumented CALL nnnn
                    if (is_8085())
                        op_jmp(m_AF.b.l & X5F);
                    else
                        op_call(1);
                    break;
                case 0xfe: // CPI nn
                    m_WZ.b.l = read_arg();
                    op_cmp(m_WZ.b.l);
                    break;
                case 0xff: // RST 7
                    op_rst(7);
                    break;
            } // end big switch
        }


        void init_tables()
        {
            u8 zs;
            int i;
            int p;
            for (i = 0; i < 256; i++)
            {
                /* cycles */
                lut_cycles[i] = is_8085() ? lut_cycles_8085[i] : lut_cycles_8080[i];

                /* flags */
                zs = 0;
                if (i == 0) zs |= ZF;
                if ((i & 128) != 0) zs |= SF;
                p = 0;
                if ((i & 1) != 0) ++p;
                if ((i & 2) != 0) ++p;
                if ((i & 4) != 0) ++p;
                if ((i & 8) != 0) ++p;
                if ((i & 16) != 0) ++p;
                if ((i & 32) != 0) ++p;
                if ((i & 64) != 0) ++p;
                if ((i & 128) != 0) ++p;
                lut_zs[i] = zs;
                lut_zsp[i] = (u8)(zs | ((p & 1) != 0 ? (u8)0 : PF));
            }
        }

        void op_ora(u8 v)
        {
            m_AF.b.h |= v;
            m_AF.b.l = lut_zsp[m_AF.b.h];
        }

        void op_xra(u8 v)
        {
            m_AF.b.h ^= v;
            m_AF.b.l = lut_zsp[m_AF.b.h];
        }

        void op_ana(u8 v)
        {
            u8 hc = (u8)(((m_AF.b.h | v) << 1) & HF);
            m_AF.b.h &= v;
            m_AF.b.l = lut_zsp[m_AF.b.h];
            if (is_8085())
                m_AF.b.l |= HF;
            else
                m_AF.b.l |= hc;
        }

        u8 op_inr(u8 v)
        {
            u8 hc = ((v & 0x0f) == 0x0f) ? HF : (u8)0;
            m_AF.b.l = (u8)((m_AF.b.l & CF) | lut_zsp[++v] | hc);
            return v;
        }

        u8 op_dcr(u8 v)
        {
            u8 hc = ((v & 0x0f) != 0x00) ? HF : (u8)0;
            m_AF.b.l = (u8)((m_AF.b.l & CF) | lut_zsp[--v] | hc | VF);
            return v;
        }

        void op_add(u8 v)
        {
            int q = m_AF.b.h + v;
            m_AF.b.l = (u8)(lut_zsp[q & 0xff] | ((q >> 8) & CF) | ((m_AF.b.h ^ q ^ v) & HF));
            m_AF.b.h = (u8)q;
        }

        void op_adc(u8 v)
        {
            int q = m_AF.b.h + v + (m_AF.b.l & CF);
            m_AF.b.l = (u8)(lut_zsp[q & 0xff] | ((q >> 8) & CF) | ((m_AF.b.h ^ q ^ v) & HF));
            m_AF.b.h = (u8)q;
        }

        void op_sub(u8 v)
        {
            int q = m_AF.b.h - v;
            m_AF.b.l = (u8)(lut_zsp[q & 0xff] | ((q >> 8) & CF) | (~(m_AF.b.h ^ q ^ v) & HF) | VF);
            m_AF.b.h = (u8)q;
        }

        void op_sbb(u8 v)
        {
            int q = m_AF.b.h - v - (m_AF.b.l & CF);
            m_AF.b.l = (u8)(lut_zsp[q & 0xff] | ((q >> 8) & CF) | (~(m_AF.b.h ^ q ^ v) & HF) | VF);
            m_AF.b.h = (u8)q;
        }

        void op_cmp(u8 v)
        {
            int q = m_AF.b.h - v;
            m_AF.b.l = (u8)(lut_zsp[q & 0xff] | ((q >> 8) & CF) | (~(m_AF.b.h ^ q ^ v) & HF) | VF);
        }

        void op_dad(u16 v)
        {
            int q = m_HL.w.l + v;
            m_AF.b.l = (u8)((m_AF.b.l & ~CF) | (q >> 16 & CF));
            m_HL.w.l = (u16)q;
        }

        void op_jmp(int cond)
        {
            // on 8085, jump if condition is not satisfied is shorter
            if (cond != 0)
            {
                m_PC = read_arg16();
            }
            else
            {
                m_PC.w.l += 2;
                m_icount.i += is_8085() ? 3 : 0;
            }
        }

        void op_call(int cond)
        {
            // on 8085, call if condition is not satisfied is 9 ticks
            if (cond != 0)
            {
                PAIR p = read_arg16();
                m_icount.i -= is_8085() ? 7 : 6 ;
                op_push(m_PC);
                m_PC = p;
            }
            else
            {
                m_PC.w.l += 2;
                m_icount.i += is_8085() ? 2 : 0;
            }
        }

        void op_ret(int cond)
        {
            // conditional RET only
            if (cond != 0)
            {
                m_icount.i -= 6;
                m_PC = op_pop();
            }
        }

        void op_rst(u8 v)
        {
            op_push(m_PC);
            m_PC.d = 8U * v;
        }
    }


    public class i8080_cpu_device : i8085a_cpu_device
    {
        //DEFINE_DEVICE_TYPE(I8080,  i8080_cpu_device,  "i8080",  "Intel 8080")
        public static readonly emu.detail.device_type_impl I8080 = DEFINE_DEVICE_TYPE("i8080", "Intel 8080", (type, mconfig, tag, owner, clock) => { return new i8080_cpu_device(mconfig, tag, owner, clock); });


        // construction/destruction
        i8080_cpu_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, I8080, tag, owner, clock, (int)CPUTYPE.CPUTYPE_8080)
        {
        }


        protected override u32 device_execute_interface_execute_input_lines() { return 1; }
        protected override u64 device_execute_interface_execute_clocks_to_cycles(u64 clocks) { return clocks; }
        protected override u64 device_execute_interface_execute_cycles_to_clocks(u64 cycles) { return cycles; }
    }


    //class i8080a_cpu_device : public i8085a_cpu_device


    static class i8085_global
    {
        public static i8080_cpu_device I8080<bool_Required>(machine_config mconfig, device_finder<i8080_cpu_device, bool_Required> finder, double clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, i8080_cpu_device.I8080, new XTAL(clock)); }
        public static i8085a_cpu_device I8085A<bool_Required>(machine_config mconfig, device_finder<i8085a_cpu_device, bool_Required> finder, u32 clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, i8085a_cpu_device.I8085A, clock); }
    }
}
