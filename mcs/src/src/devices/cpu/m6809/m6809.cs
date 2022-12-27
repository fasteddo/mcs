// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Runtime.InteropServices;

using devcb_write_line = mame.devcb_write<mame.Type_constant_s32, mame.devcb_value_const_unsigned_1<mame.Type_constant_s32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.device_global;
using static mame.distate_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.m6809_global;


namespace mame
{
    // ======================> m6809_base_device
    // Used by core CPU interface
    public partial class m6809_base_device : cpu_device
    {
        public class device_execute_interface_m6809 : device_execute_interface
        {
            public device_execute_interface_m6809(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint32_t execute_min_cycles() { return ((m6809_base_device)device()).device_execute_interface_execute_min_cycles(); }
            protected override uint32_t execute_max_cycles() { return ((m6809_base_device)device()).device_execute_interface_execute_max_cycles(); }
            protected override uint32_t execute_input_lines() { return ((m6809_base_device)device()).device_execute_interface_execute_input_lines(); }
            protected override void execute_run() { ((m6809_base_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { ((m6809_base_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
            protected override bool execute_input_edge_triggered(int inputnum) { return ((m6809_base_device)device()).device_execute_interface_execute_input_edge_triggered(inputnum); }
            protected override uint64_t execute_clocks_to_cycles(uint64_t clocks) { return ((m6809_base_device)device()).device_execute_interface_execute_clocks_to_cycles(clocks); }
            protected override uint64_t execute_cycles_to_clocks(uint64_t cycles) { return ((m6809_base_device)device()).device_execute_interface_execute_cycles_to_clocks(cycles); }
        }


        public class device_memory_interface_m6809 : device_memory_interface
        {
            public device_memory_interface_m6809(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((m6809_base_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_m6809 : device_state_interface
        {
            public device_state_interface_m6809(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void state_import(device_state_entry entry) { ((m6809_base_device)device()).device_state_interface_state_import(entry); }
            protected override void state_string_export(device_state_entry entry, out string str) { ((m6809_base_device)device()).device_state_interface_state_string_export(entry, out str); }
        }


        public class device_disasm_interface_m6809 : device_disasm_interface
        {
            public device_disasm_interface_m6809(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { return ((m6809_base_device)device()).device_disasm_interface_create_disassembler(); }
        }


        abstract class memory_interface
        {
            public memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache cprogram = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache();  //memory_access<16, 0, 0, ENDIANNESS_BIG>::cache cprogram, csprogram;
            public memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache csprogram = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache();
            public memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific program = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific();  //memory_access<16, 0, 0, ENDIANNESS_BIG>::specific program;

            protected abstract uint8_t read(uint16_t adr);
            public abstract uint8_t read_opcode(uint16_t adr);
            public abstract uint8_t read_opcode_arg(uint16_t adr);
            protected abstract void write(uint16_t adr, uint8_t val);
        }


        class mi_default : memory_interface
        {
            //virtual ~mi_default() {}
            protected override uint8_t read(uint16_t adr) { throw new emu_unimplemented(); }
            public override uint8_t read_opcode(uint16_t adr) { return csprogram.read_byte(adr); }
            public override uint8_t read_opcode_arg(uint16_t adr) { return cprogram.read_byte(adr); }
            protected override void write(uint16_t adr, uint8_t val) { throw new emu_unimplemented(); }
        }


        // addressing modes
        //enum
        //{
            const int ADDRESSING_MODE_IMMEDIATE   = 0;
            const int ADDRESSING_MODE_EA          = 1;
            const int ADDRESSING_MODE_REGISTER_A  = 2;
            const int ADDRESSING_MODE_REGISTER_B  = 3;
            const int ADDRESSING_MODE_REGISTER_D  = 4;
        //};


        // register transfer
        struct exgtfr_register
        {
            uint8_t   byte_value;
            uint16_t  word_value;
        }


        // flag bits in the cc register
        //enum
        //{
            const uint8_t CC_C        = 0x01;         // Carry
            const uint8_t CC_V        = 0x02;         // Overflow
            const uint8_t CC_Z        = 0x04;         // Zero
            const uint8_t CC_N        = 0x08;         // Negative
            const uint8_t CC_I        = 0x10;         // Inhibit IRQ
            const uint8_t CC_H        = 0x20;         // Half (auxiliary) carry
            const uint8_t CC_F        = 0x40;         // Inhibit FIRQ
            const uint8_t CC_E        = 0x80;         // Entire state pushed
        //};


        // flag combinations
        //enum
        //{
            const uint8_t CC_VC   = CC_V | CC_C;
            const uint8_t CC_ZC   = CC_Z | CC_C;
            const uint8_t CC_NZ   = CC_N | CC_Z;
            const uint8_t CC_NZC  = CC_N | CC_Z | CC_C;
            const uint8_t CC_NZV  = CC_N | CC_Z | CC_V;
            const uint8_t CC_NZVC = CC_N | CC_Z | CC_V | CC_C;
            const uint8_t CC_HNZVC = CC_H | CC_N | CC_Z | CC_V | CC_C;
        //};


        // interrupt vectors
        //enum
        //{
            const uint16_t VECTOR_SWI3         = 0xFFF2;
            const uint16_t VECTOR_SWI2         = 0xFFF4;
            const uint16_t VECTOR_FIRQ         = 0xFFF6;
            const uint16_t VECTOR_IRQ          = 0xFFF8;
            const uint16_t VECTOR_SWI          = 0xFFFA;
            const uint16_t VECTOR_NMI          = 0xFFFC;
            const uint16_t VECTOR_RESET_FFFE   = 0xFFFE;
        //};


        [StructLayout(LayoutKind.Explicit)]
        struct M6809Q  //union M6809Q
        {
            //#ifdef LSB_FIRST
            [StructLayout(LayoutKind.Explicit)]
            public struct M6809Q_R  //union
            {
                [FieldOffset(0)] public uint8_t f;  //struct { uint8_t f, e, b, a; };
                [FieldOffset(1)] public uint8_t e;
                [FieldOffset(2)] public uint8_t b;
                [FieldOffset(3)] public uint8_t a;

                [FieldOffset(0)] public uint16_t w;  //struct { uint16_t w, d; };
                [FieldOffset(2)] public uint16_t d;
            }  //} r;

            [StructLayout(LayoutKind.Explicit)]
            public struct M6809Q_P  //union
            {
                [FieldOffset(0)] public PAIR16 w;
                [FieldOffset(2)] public PAIR16 d;
            }

            [FieldOffset(0)] public M6809Q_R r;
            [FieldOffset(0)] public M6809Q_P p;  //struct { PAIR16 w, d; } p;
            //#else
            //union
            //{
            //    struct { uint8_t a, b, e, f; };
            //    struct { uint16_t d, w; };
            //} r;
            //struct { PAIR16 d, w; } p;
            //#endif

            [FieldOffset(0)] public uint32_t q;
        }


        device_execute_interface_m6809 m_diexec;
        device_memory_interface_m6809 m_dimemory;
        device_state_interface_m6809 m_distate;


        // Memory interface
        memory_interface m_mintf;  //std::unique_ptr<memory_interface> m_mintf;

        // CPU registers
        PAIR16 m_pc;               // program counter
        PAIR16 m_ppc;              // previous program counter
        M6809Q m_q;                // accumulator a and b (plus e and f on 6309)
        PAIR16 m_x;                // index registers
        PAIR16 m_y;                // index registers
        PAIR16 m_u;                // stack pointers
        PAIR16 m_s;                // stack pointers
        uint8_t m_dp;              // direct page register
        uint8_t m_cc;
        PAIR16 m_temp;
        uint8_t m_opcode;

        // other internal state
        Pointer<uint8_t> m_reg8;  //uint8_t *                     m_reg8;
        Pointer<PAIR16> m_reg16;  //PAIR16 *                    m_reg16;
        int m_reg;
        bool m_nmi_line;
        bool m_nmi_asserted;
        bool m_firq_line;
        bool m_irq_line;
        bool m_lds_encountered;
        intref m_icount = new intref();  //int                         m_icount;
        int m_addressing_mode;
        PAIR16 m_ea;               // effective address

        // Callbacks
        devcb_write_line m_lic_func;         // LIC pin on the 6809E

        // address spaces
        address_space_config m_program_config;
        address_space_config m_sprogram_config;

        // other state
        uint32_t m_state;
        bool m_cond;

        // incidentals
        int m_clock_divider;


        // construction/destruction
        //-------------------------------------------------
        //  m6809_base_device - constructor
        //-------------------------------------------------
        protected m6809_base_device(machine_config mconfig, string tag, device_t owner, uint32_t clock, device_type type, int divider)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_m6809(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_m6809(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_m6809(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_m6809(mconfig, this));

            m_diexec = GetClassInterface<device_execute_interface_m6809>();
            m_dimemory = GetClassInterface<device_memory_interface_m6809>();
            m_distate = GetClassInterface<device_state_interface_m6809>();


            m_lic_func = new devcb_write_line(this);
            m_program_config = new address_space_config("program", ENDIANNESS_BIG, 8, 16);
            m_sprogram_config = new address_space_config("decrypted_opcodes", ENDIANNESS_BIG, 8, 16);
            m_clock_divider = divider;


            m_mintf = null;
        }


        // device-level overrides
        protected override void device_start()
        {
            if (m_mintf == null)
                m_mintf = new mi_default();  //m_mintf = std::make_unique<mi_default>();

            m_dimemory.space(AS_PROGRAM).cache(m_mintf.cprogram);
            m_dimemory.space(AS_PROGRAM).specific(m_mintf.program);
            m_dimemory.space(m_dimemory.has_space(AS_OPCODES) ? AS_OPCODES : AS_PROGRAM).cache(m_mintf.csprogram);

            m_lic_func.resolve_safe();

            // register our state for the debugger
            m_distate.state_add(STATE_GENPCBASE, "CURPC",     m_ppc.w).callimport().noshow();
            m_distate.state_add(STATE_GENFLAGS,  "CURFLAGS",  m_cc).formatstr("%8s").noshow();
            m_distate.state_add(M6809_PC,        "PC",        m_pc.w).callimport().mask(0xffff);
            m_distate.state_add(M6809_S,         "S",         m_s.w).mask(0xffff);
            m_distate.state_add(M6809_CC,        "CC",        m_cc).mask(0xff);
            m_distate.state_add(M6809_DP,        "DP",        m_dp).mask(0xff);

            if (is_6809())
            {
                m_distate.state_add(M6809_A,     "A",         m_q.r.a).mask(0xff);
                m_distate.state_add(M6809_B,     "B",         m_q.r.b).mask(0xff);
                m_distate.state_add(M6809_D,     "D",         m_q.r.d).mask(0xffff);
                m_distate.state_add(M6809_X,     "X",         m_x.w).mask(0xffff);
                m_distate.state_add(M6809_Y,     "Y",         m_y.w).mask(0xffff);
                m_distate.state_add(M6809_U,     "U",         m_u.w).mask(0xffff);
            }

            // initialize variables
            m_cc = 0;
            m_pc.w = 0;
            m_s.w = 0;
            m_u.w = 0;
            m_q.q = 0;
            m_x.w = 0;
            m_y.w = 0;
            m_dp = 0;
            m_reg = 0;
            m_reg8 = null;
            m_reg16 = null;

            // setup regtable
            save_item(NAME(new { m_pc.w }));
            save_item(NAME(new { m_ppc.w }));
            save_item(NAME(new { m_q.q }));
            save_item(NAME(new { m_dp }));
            save_item(NAME(new { m_u.w }));
            save_item(NAME(new { m_s.w }));
            save_item(NAME(new { m_x.w }));
            save_item(NAME(new { m_y.w }));
            save_item(NAME(new { m_cc }));
            save_item(NAME(new { m_temp.w }));
            save_item(NAME(new { m_opcode }));
            save_item(NAME(new { m_nmi_asserted }));
            save_item(NAME(new { m_nmi_line }));
            save_item(NAME(new { m_firq_line }));
            save_item(NAME(new { m_irq_line }));
            save_item(NAME(new { m_lds_encountered }));
            save_item(NAME(new { m_state }));
            save_item(NAME(new { m_ea.w }));
            save_item(NAME(new { m_addressing_mode }));
            save_item(NAME(new { m_reg }));
            save_item(NAME(new { m_cond }));

            // set our instruction counter
            set_icountptr(m_icount);
            m_icount.i = 0;
        }


        protected override void device_reset()
        {
            m_nmi_line = false;
            m_nmi_asserted = false;
            m_firq_line = false;
            m_irq_line = false;
            m_lds_encountered = false;

            m_dp = 0x00;        // reset direct page register

            m_cc |= CC_I;       // IRQ disabled
            m_cc |= CC_F;       // FIRQ disabled

            m_pc.b.h = m_dimemory.space(AS_PROGRAM).read_byte(VECTOR_RESET_FFFE + 0);
            m_pc.b.l = m_dimemory.space(AS_PROGRAM).read_byte(VECTOR_RESET_FFFE + 1);

            // reset sub-instruction state
            reset_state();
        }


        protected override void device_pre_save() { throw new emu_unimplemented(); }
        protected override void device_post_load() { throw new emu_unimplemented(); }

        // device_execute_interface overrides
        protected virtual uint32_t device_execute_interface_execute_min_cycles() { return 1; }
        protected virtual uint32_t device_execute_interface_execute_max_cycles() { throw new emu_unimplemented(); }
        protected virtual uint32_t device_execute_interface_execute_input_lines() { throw new emu_unimplemented(); }
        protected virtual void device_execute_interface_execute_run()
        {
            do
            {
                execute_one();
            } while (m_icount.i > 0);
        }
        protected virtual void device_execute_interface_execute_set_input(int inputnum, int state) { throw new emu_unimplemented(); }
        protected virtual bool device_execute_interface_execute_input_edge_triggered(int inputnum) { throw new emu_unimplemented(); }
        protected virtual uint64_t device_execute_interface_execute_clocks_to_cycles(uint64_t clocks) { return (clocks + (uint64_t)m_clock_divider - 1) / (uint64_t)m_clock_divider; }
        protected virtual uint64_t device_execute_interface_execute_cycles_to_clocks(uint64_t cycles) { throw new emu_unimplemented(); }


        // device_memory_interface overrides
        //-------------------------------------------------
        //  memory_space_config - return the configuration
        //  of the specified address space, or nullptr if
        //  the space doesn't exist
        //-------------------------------------------------
        protected virtual space_config_vector device_memory_interface_memory_space_config()
        {
            if (memory().has_configured_map(AS_OPCODES))
            {
                return new space_config_vector
                {
                    std.make_pair(AS_PROGRAM, m_program_config),
                    std.make_pair(AS_OPCODES, m_sprogram_config)
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


        // device_disasm_interface overrides
        protected virtual util.disasm_interface device_disasm_interface_create_disassembler() { throw new emu_unimplemented(); }

        // device_state_interface overrides
        protected virtual void device_state_interface_state_import(device_state_entry entry) { throw new emu_unimplemented(); }
        protected virtual void device_state_interface_state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }


        protected virtual bool is_6809() { return true; }


        // eat cycles
        void eat(int cycles)                              { m_icount.i -= cycles; }

        //m6809inl.cs
        //void eat_remaining();

        // read a byte from given memory location
        uint8_t read_memory(uint16_t address)             { throw new emu_unimplemented(); }  //{ eat(1); return m_mintf->read(address); }

        // write a byte to given memory location
        void write_memory(uint16_t address, uint8_t data) { throw new emu_unimplemented(); }  //{ eat(1); m_mintf->write(address, data); }

        // read_opcode() is like read_memory() except it is used for reading opcodes. In  the case of a system
        // with memory mapped I/O, this function can be used  to greatly speed up emulation.
        uint8_t read_opcode(uint16_t address)             { eat(1); return m_mintf.read_opcode(address); }

        // read_opcode_arg() is identical to read_opcode() except it is used for reading opcode  arguments. This
        // difference can be used to support systems that use different encoding mechanisms for opcodes
        // and opcode arguments.
        uint8_t read_opcode_arg(uint16_t address)         { eat(1); return m_mintf.read_opcode_arg(address); }

        // read_opcode() and bump the program counter
        uint8_t read_opcode()                             { return read_opcode(m_pc.w++); }
        uint8_t read_opcode_arg()                         { return read_opcode_arg(m_pc.w++); }
        void dummy_read_opcode_arg(uint16_t delta)        { read_opcode_arg((uint16_t)(m_pc.w + delta)); }
        void dummy_vma(int count)                         { for (int i = 0; i != count; i++) { read_opcode_arg(0xffff); } }

        // state stack - implemented as a uint32_t
        void push_state(uint16_t state)                   { m_state = (m_state << 9) | state; }
        uint16_t pop_state()                              { uint16_t result = (uint16_t)(m_state & 0x1ff); m_state >>= 9; return result; }
        void reset_state()                                { m_state = 0; }

        // effective address reading/writing
        //uint8_t read_ea()                                 { return read_memory(m_ea.w); }
        void write_ea(uint8_t data)                       { throw new emu_unimplemented(); }//{ write_memory(m_ea.w, data); }
        void set_ea(uint16_t ea)                          { m_ea.w = ea; m_addressing_mode = ADDRESSING_MODE_EA; }
        void set_ea_h(uint8_t ea_h)                       { m_ea.b.h = ea_h; }
        void set_ea_l(uint8_t ea_l)                       { m_ea.b.l = ea_l; m_addressing_mode = ADDRESSING_MODE_EA; }

        // m6809inl.cs
        // operand reading/writing
        //uint8_t read_operand();
        //uint8_t read_operand(int ordinal);
        //void write_operand(uint8_t data);
        //void write_operand(int ordinal, uint8_t data);

        // m6809inl.cs
        // instructions
        //void daa();
        //void mul();

        // miscellaneous
        void nop()                                        { }
        uint8_t rotate_right(uint8_t value)               { throw new emu_unimplemented(); }  //template<class T> T rotate_right(T value);
        uint16_t rotate_right(uint16_t value)             { throw new emu_unimplemented(); }  //template<class T> T rotate_right(T value);
        uint32_t rotate_left(uint8_t value)               { throw new emu_unimplemented(); }  //template<class T> uint32_t rotate_left(T value);
        uint32_t rotate_left(uint16_t value)              { throw new emu_unimplemented(); }  //template<class T> uint32_t rotate_left(T value);
        void set_a()                                      { m_addressing_mode = ADDRESSING_MODE_REGISTER_A; }
        void set_b()                                      { m_addressing_mode = ADDRESSING_MODE_REGISTER_B; }
        //void set_d()                                      { m_addressing_mode = ADDRESSING_MODE_REGISTER_D; }
        void set_imm()                                    { m_addressing_mode = ADDRESSING_MODE_IMMEDIATE; }
        void set_regop8(uint8_t reg)                      { throw new emu_unimplemented(); }  //{ m_reg8 = &reg; m_reg16 = nullptr; }  //void set_regop8(uint8_t &reg)                     { m_reg8 = &reg; m_reg16 = nullptr; }
        void set_regop16(PAIR16 reg, bool is_m_s = false) { throw new emu_unimplemented(); }  //{ m_reg16 = &reg; m_reg8 = null; }  //void set_regop16(PAIR16 &reg)                   { m_reg16 = &reg; m_reg8 = nullptr; }
        ref byte regop8()                                 { throw new emu_unimplemented(); }  //{ assert(m_reg8 != nullptr); return *m_reg8; }  //uint8_t &regop8()                                 { assert(m_reg8 != nullptr); return *m_reg8; }
        ref PAIR16 regop16()                              { throw new emu_unimplemented(); }  //{ assert(m_reg16 != nullptr); return *m_reg16; }  //PAIR16 &regop16()                               { assert(m_reg16 != nullptr); return *m_reg16; }
        bool regop16_is_m_s()                             { throw new emu_unimplemented(); }
        //bool is_register_register_op_16_bit()             { return m_reg16 != nullptr; }
        bool add8_sets_h()                                { return true; }
        bool hd6309_native_mode()                         { return false; }

        // m6809inl.cs
        // index reg
        //uint16_t &ireg();

        // flags
        uint8_t set_flags_u8(uint8_t mask, uint8_t a, uint8_t b, uint32_t r) { throw new emu_unimplemented(); }  //template<class T> T set_flags(uint8_t mask, T a, T b, uint32_t r);
        uint16_t set_flags_u16(uint8_t mask, uint16_t a, uint16_t b, uint32_t r) { throw new emu_unimplemented(); }  //template<class T> T set_flags(uint8_t mask, T a, T b, uint32_t r);
        uint8_t set_flags_u8(uint8_t mask, uint8_t r) { throw new emu_unimplemented(); }  //template<class T> T set_flags(uint8_t mask, T r);
        uint16_t set_flags_u16(uint8_t mask, uint16_t r) { throw new emu_unimplemented(); }  //template<class T> T set_flags(uint8_t mask, T r);

        // branch conditions
        bool cond_hi() { return (m_cc & CC_ZC) == 0; }                                                // BHI/BLS
        bool cond_cc() { return (m_cc & CC_C) == 0;   }                                               // BCC/BCS
        bool cond_ne() { return (m_cc & CC_Z) == 0;   }                                               // BNE/BEQ
        bool cond_vc() { return (m_cc & CC_V) == 0;   }                                               // BVC/BVS
        bool cond_pl() { return (m_cc & CC_N) == 0;   }                                               // BPL/BMI
        bool cond_ge() { return ((m_cc & CC_N) != 0 ? true : false) == ((m_cc & CC_V) != 0 ? true : false); }   // BGE/BLT
        bool cond_gt() { return cond_ge() && (m_cc & CC_Z) == 0; }                                    // BGT/BLE
        void set_cond(bool cond)  { m_cond = cond; }
        bool branch_taken()       { return m_cond; }

        // interrupt registers
        bool firq_saves_entire_state()        { return false; }
        uint16_t partial_state_registers()    { return 0x81; }
        uint16_t entire_state_registers()     { return 0xFF; }

        // miscellaneous
        exgtfr_register read_exgtfr_register(uint8_t reg) { throw new emu_unimplemented(); }
        void write_exgtfr_register(uint8_t reg, exgtfr_register value) { throw new emu_unimplemented(); }

        // m6809inl.cs
        //bool is_register_addressing_mode();

        //bool is_ea_addressing_mode() { return m_addressing_mode == ADDRESSING_MODE_EA; }

        // m6809inl.cs
        //uint16_t get_pending_interrupt();

        void log_illegal() { throw new emu_unimplemented(); }


        // functions
        void execute_one()
        {
            //switch (pop_state())
            //{
            //    #include "cpu/m6809/m6809.hxx"
            //}
            execute_one_switch();
        }


        //const char *inputnum_string(int inputnum);
    }


    // ======================> mc6809_device
    //class mc6809_device : public m6809_base_device


    // ======================> mc6809e_device
    public class mc6809e_device : m6809_base_device
    {
        //DEFINE_DEVICE_TYPE(MC6809E, mc6809e_device, "mc6809e", "Motorola MC6809E")
        public static readonly emu.detail.device_type_impl MC6809E = DEFINE_DEVICE_TYPE("mc6809e", "Motorola MC6809E", (type, mconfig, tag, owner, clock) => { return new mc6809e_device(mconfig, tag, owner, clock); });

        // construction/destruction
        mc6809e_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, tag, owner, clock, MC6809E, 1)
        {
        }

        // MC6809E has LIC line to indicate opcode/data fetch
        //auto lic() { return m_lic_func.bind(); }
    }


    // ======================> m6809_device (LEGACY)
    //class m6809_device : public m6809_base_device


    static class m6809_global
    {
        //enum
        //{
            public const int M6809_PC = STATE_GENPC;
            public const int M6809_S  = 0;
            public const int M6809_CC = 1;
            public const int M6809_A  = 2;
            public const int M6809_B  = 3;
            public const int M6809_D  = 4;
            public const int M6809_U  = 5;
            public const int M6809_X  = 6;
            public const int M6809_Y  = 7;
            public const int M6809_DP = 8;
        //};


        public const int M6809_IRQ_LINE  = 0;   /* IRQ line number */
        public const int M6809_FIRQ_LINE = 1;   /* FIRQ line number */
        public const int M6809_SWI       = 2;   /* Virtual SWI line to be used during SWI acknowledge cycle */


        public static mc6809e_device MC6809E<bool_Required>(machine_config mconfig, device_finder<mc6809e_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, mc6809e_device.MC6809E, clock); }
    }
}
