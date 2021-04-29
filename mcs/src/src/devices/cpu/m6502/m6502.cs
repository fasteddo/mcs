// license:BSD-3-Clause
// copyright-holders:Edward Fast

//#define MCS_DEBUG

using System;
using System.Collections.Generic;

using devcb_write_line = mame.devcb_write<int, uint, mame.devcb_operators_s32_u32, mame.devcb_operators_u32_s32, mame.devcb_constant_1<uint, uint, mame.devcb_operators_u32_u32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using offs_t = System.UInt32;  //using offs_t = u32;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    public partial class m6502_device : cpu_device
    {
        //DEFINE_DEVICE_TYPE(M6502, m6502_device, "m6502", "MOS Technology 6502")
        static device_t device_creator_mb6502_cpu_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new m6502_device(mconfig, tag, owner, clock); }
        public static readonly device_type M6502 = DEFINE_DEVICE_TYPE(device_creator_mb6502_cpu_device, "m6502", "MOS Technology 6502");


        class device_execute_interface_m6502 : device_execute_interface
        {
            public device_execute_interface_m6502(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint32_t execute_min_cycles() { return 1; }
            protected override uint32_t execute_max_cycles() { return 10; }
            protected override uint32_t execute_input_lines() { return ((m6502_device)device()).device_execute_interface_execute_input_lines(); }
            protected override void execute_run() { ((m6502_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { ((m6502_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
            protected override bool execute_input_edge_triggered(int inputnum) { return ((m6502_device)device()).device_execute_interface_execute_input_edge_triggered(inputnum); }
        }


        public class device_memory_interface_m6502 : device_memory_interface
        {
            public device_memory_interface_m6502(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((m6502_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_m6502 : device_state_interface
        {
            public device_state_interface_m6502(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void state_import(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_export(device_state_entry entry) { ((m6502_device)device()).device_state_interface_state_export(entry); }
            protected override void state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }
        }


        public class device_disasm_interface_m6502 : device_disasm_interface
        {
            public device_disasm_interface_m6502(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        //enum
        //{
        const int M6502_PC = 1;
        const int M6502_A  = 2;
        const int M6502_X  = 3;
        const int M6502_Y  = 4;
        const int M6502_P  = 5;
        const int M6502_S  = 6;
        const int M6502_IR = 7; 
        //}


        //enum
        //{
        public const int M6502_IRQ_LINE = IRQ_LINE;
        const int M6502_NMI_LINE = NMI_LINE;
        const int M6502_SET_OVERFLOW = V_LINE;
        //}


        //enum
        //{
        public const int IRQ_LINE = g.INPUT_LINE_IRQ0;
        const int APU_IRQ_LINE = g.INPUT_LINE_IRQ1;
        public const int NMI_LINE = g.INPUT_LINE_NMI;
        const int V_LINE   = g.INPUT_LINE_IRQ0 + 16;
        //}


        //enum
        //{
        const int STATE_RESET = 0xff00;
        //}


        //enum
        //{
        const uint8_t F_N = 0x80;
        const uint8_t F_V = 0x40;
        const uint8_t F_E = 0x20; // 65ce02
        const uint8_t F_T = 0x20; // M740: replaces A with $00,X in some opcodes when set
        const uint8_t F_B = 0x10;
        const uint8_t F_D = 0x08;
        const uint8_t F_I = 0x04;
        const uint8_t F_Z = 0x02;
        const uint8_t F_C = 0x01;
        //}


        abstract class memory_interface
        {
            public memory_access<int_constant_16, int_constant_0, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.cache cprogram = new memory_access<int_constant_16, int_constant_0, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.cache();  //memory_access<16, 0, 0, ENDIANNESS_LITTLE>::cache cprogram;
            public memory_access<int_constant_16, int_constant_0, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.cache csprogram = new memory_access<int_constant_16, int_constant_0, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.cache();  //memory_access<16, 0, 0, ENDIANNESS_LITTLE>::cache csprogram;
            public memory_access<int_constant_16, int_constant_0, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.specific program = new memory_access<int_constant_16, int_constant_0, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.specific();  //memory_access<16, 0, 0, ENDIANNESS_LITTLE>::specific program;
            public memory_access<int_constant_14, int_constant_0, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.specific program14 = new memory_access<int_constant_14, int_constant_0, int_constant_0, endianness_t_constant_ENDIANNESS_LITTLE>.specific();  //memory_access<14, 0, 0, ENDIANNESS_LITTLE>::specific program14;

            //virtual ~memory_interface() {}
            public abstract uint8_t read(uint16_t adr);
            //virtual uint8_t read_9(uint16_t adr);
            public abstract uint8_t read_sync(uint16_t adr);
            public abstract uint8_t read_arg(uint16_t adr);
            public abstract void write(uint16_t adr, uint8_t val);
            //virtual void write_9(uint16_t adr, uint8_t val);
        }


        class mi_default : memory_interface
        {
            //virtual ~mi_default() {}

            public override uint8_t read(uint16_t adr) { return program.read_byte(adr); }
            public override uint8_t read_sync(uint16_t adr) { return csprogram.read_byte(adr); }
            public override uint8_t read_arg(uint16_t adr) { return cprogram.read_byte(adr); }
            public override void write(uint16_t adr, uint8_t val) { program.write_byte(adr, val); }
        }


        class mi_default14 : mi_default
        {
            //virtual ~mi_default14() = default;

            public override uint8_t read(uint16_t adr) { throw new emu_unimplemented(); }
            public override void write(uint16_t adr, uint8_t val) { throw new emu_unimplemented(); }
        }


        device_memory_interface_m6502 m_dimemory;
        device_execute_interface_m6502 m_diexec;
        device_state_interface_m6502 m_distate;


        devcb_write_line sync_w;

        address_space_config program_config;
        address_space_config sprogram_config;

        uint16_t PPC;                    /* previous program counter */
        uint16_t NPC;                    /* next start-of-instruction program counter */
        uint16_t PC;                     /* program counter */
        uint16_t SP;                     /* stack pointer (always 100 - 1FF) */
        uint16_t TMP;                    /* temporary internal values */
        uint8_t TMP2;                   /* another temporary internal value, 8 bits this time */
        uint8_t A;                      /* Accumulator */
        uint8_t X;                      /* X index register */
        uint8_t Y;                      /* Y index register */
        public uint8_t P;                      /* Processor status */
        uint8_t IR;                     /* Prefetched instruction register */
        int inst_state_base;        /* Current instruction bank */

        memory_interface mintf;
        int inst_state;
        int inst_substate;
        intref icount_ = new intref();  //int icount;
        int bcount;
        int count_before_instruction_step;
        bool nmi_state;
        bool irq_state;
        bool apu_irq_state;
        bool v_state;
        bool nmi_pending;
        bool irq_taken;
        bool sync;
        bool inhibit_interrupts;


#if MCS_DEBUG
        int m6502opcount = 0;
#endif


        m6502_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : this(mconfig, M6502, tag, owner, clock)
        {
        }


        m6502_device(machine_config mconfig, device_type type, string tag, device_t owner, u32 clock)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_m6502(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_m6502(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_m6502(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_m6502(mconfig, this));


            sync_w = new devcb_write_line(this);
            program_config = new address_space_config("program", endianness_t.ENDIANNESS_LITTLE, 8, 16);
            sprogram_config = new address_space_config("decrypted_opcodes", endianness_t.ENDIANNESS_LITTLE, 8, 16);
            PPC = 0;
            NPC = 0;
            PC = 0;
            SP = 0;
            TMP = 0;
            TMP2 = 0;
            A = 0;
            X = 0;
            Y = 0;
            P = 0;
            IR = 0;
            inst_state_base = 0;
            mintf = null;
            inst_state = 0;
            inst_substate = 0;
            icount_.i = 0;  //icount = 0;
            nmi_state = false;
            irq_state = false;
            apu_irq_state = false;
            v_state = false;
            nmi_pending = false;
            irq_taken = false;
            sync = false;
            inhibit_interrupts = false;
            count_before_instruction_step = 0;
        }


        int icount { get { return icount_.i; } set { icount_.i = value; } }


        //bool get_sync() const { return sync; }

        //auto sync_cb() { return sync_w.bind(); }


        protected virtual void init()
        {
            m_dimemory.space(AS_PROGRAM).cache(mintf.cprogram);
            m_dimemory.space(m_dimemory.has_space(AS_OPCODES) ? AS_OPCODES : AS_PROGRAM).cache(mintf.csprogram);
            if (m_dimemory.space(AS_PROGRAM).addr_width() > 14)
                m_dimemory.space(AS_PROGRAM).specific(mintf.program);
            else
                m_dimemory.space(AS_PROGRAM).specific(mintf.program14);

            sync_w.resolve_safe();

            XPC = 0;

            m_distate.state_add(g.STATE_GENPC,     "GENPC",     XPC).callexport().noshow();
            m_distate.state_add(g.STATE_GENPCBASE, "CURPC",     XPC).callexport().noshow();
            m_distate.state_add(g.STATE_GENSP,     "GENSP",     SP).noshow();
            m_distate.state_add(g.STATE_GENFLAGS,  "GENFLAGS",  P).callimport().formatstr("%6s").noshow();
            m_distate.state_add(M6502_PC,          "PC",        NPC).callimport();
            m_distate.state_add(M6502_A,           "A",         A);
            m_distate.state_add(M6502_X,           "X",         X);
            m_distate.state_add(M6502_Y,           "Y",         Y);
            m_distate.state_add(M6502_P,           "P",         P).callimport();
            m_distate.state_add(M6502_S,           "SP",        SP);
            m_distate.state_add(M6502_IR,          "IR",        IR);

            save_item(NAME(new { PC }));
            save_item(NAME(new { NPC }));
            save_item(NAME(new { PPC }));
            save_item(NAME(new { A }));
            save_item(NAME(new { X }));
            save_item(NAME(new { Y }));
            save_item(NAME(new { P }));
            save_item(NAME(new { SP }));
            save_item(NAME(new { TMP }));
            save_item(NAME(new { TMP2 }));
            save_item(NAME(new { IR }));
            save_item(NAME(new { nmi_state }));
            save_item(NAME(new { irq_state }));
            save_item(NAME(new { apu_irq_state }));
            save_item(NAME(new { v_state }));
            save_item(NAME(new { nmi_pending }));
            save_item(NAME(new { irq_taken }));
            save_item(NAME(new { inst_state }));
            save_item(NAME(new { inst_substate }));
            save_item(NAME(new { inst_state_base }));
            save_item(NAME(new { inhibit_interrupts }));

            set_icountptr(icount_);

            PC = 0x0000;
            NPC = 0x0000;
            A = 0x00;
            X = 0x80;
            Y = 0x00;
            P = 0x36;
            SP = 0x0100;
            TMP = 0x0000;
            TMP2 = 0x00;
            IR = 0x00;
            nmi_state = false;
            irq_state = false;
            apu_irq_state = false;
            v_state = false;
            nmi_pending = false;
            irq_taken = false;
            inst_state = STATE_RESET;
            inst_substate = 0;
            inst_state_base = 0;
            sync = false;
            inhibit_interrupts = false;
            count_before_instruction_step = 0;
        }


        // device-level overrides
        protected override void device_start()
        {
            m_dimemory = GetClassInterface<device_memory_interface_m6502>();
            m_diexec = GetClassInterface<device_execute_interface_m6502>();
            m_distate = GetClassInterface<device_state_interface_m6502>();


            mintf = m_dimemory.space(AS_PROGRAM).addr_width() > 14 ? new mi_default() : new mi_default14();

            init();
        }


        protected override void device_stop()
        {
        }


        protected override void device_reset()
        {
            inst_state = STATE_RESET;
            inst_substate = 0;
            inst_state_base = 0;
            irq_state = false;
            nmi_state = false;
            apu_irq_state = false;
            v_state = false;
            nmi_pending = false;
            irq_taken = false;
            sync = false;
            sync_w.op(g.CLEAR_LINE);
            inhibit_interrupts = false;
        }


        // device_execute_interface overrides
        //virtual uint32_t execute_min_cycles() const override;
        //virtual uint32_t execute_max_cycles() const override;
        uint32_t device_execute_interface_execute_input_lines() { return NMI_LINE + 1; }


        void device_execute_interface_execute_run()
        {
            if (inst_substate != 0)
                do_exec_partial();

            while (icount > 0)
            {
                if (inst_state < 0xff00)
                {
                    PPC = NPC;
                    inst_state = IR | inst_state_base;
                    if ((machine().debug_flags & DEBUG_FLAG_ENABLED) != 0)
                        debugger_instruction_hook(pc_to_external(NPC));
                }


#if MCS_DEBUG
                var atari_sound_comm = (atari_sound_comm_device)owner().subdevice("soundcomm");
                int s2m = atari_sound_comm.sound_to_main_ready();
                int m2s = atari_sound_comm.main_to_sound_ready();
                uint8_t m2sd = atari_sound_comm.m_main_to_sound_data;


                if (m6502opcount >= 0 && m6502opcount % 50000 == 0)
                    osd_printf_debug("m6502_device.execute_run() - {0,6} - {1,5} - {2,5} - {3,5} - {4,5} - {5,5} - {6,5} - {7,5}\n", m6502opcount, inst_state, icount, A, Y, read(513), X, m2sd);
                //if (m6502opcount >= 9000 && m6502opcount <= 10000)
                //    osd_printf_debug("m6502_device.execute_run() - {0,6} - {1,5} - {2,5} - {3,5} - {4,5} - {5,5} - {6,5} - {7,5}\n", m6502opcount, inst_state, icount, A, Y, read(513), X, m2sd);
                //if (opcount == 10000)
                //    osd_printf_debug("m6502_device.execute_run() - {0,6} - {1,5} - {2,5}\n", opcount, inst_state, icount);
#endif


                do_exec_full();


#if MCS_DEBUG
                m6502opcount++;
#endif
            }
        }

        void device_execute_interface_execute_set_input(int inputnum, int state)
        {
            switch (inputnum)
            {
            case IRQ_LINE: irq_state = state == g.ASSERT_LINE; break;
            case APU_IRQ_LINE: apu_irq_state = state == g.ASSERT_LINE; break;
            case NMI_LINE:
                if (!nmi_state && state == g.ASSERT_LINE)
                    nmi_pending = true;
                nmi_state = state == g.ASSERT_LINE;
                break;
            case V_LINE:
                if (!v_state && state == g.ASSERT_LINE)
                    P |= F_V;
                v_state = state == g.ASSERT_LINE;
                break;
            }
        }

        bool device_execute_interface_execute_input_edge_triggered(int inputnum) { return inputnum == NMI_LINE; }


        // device_memory_interface overrides
        space_config_vector device_memory_interface_memory_space_config()
        {
            if (memory().has_configured_map(AS_OPCODES))
            {
                return new space_config_vector()
                {
                    std.make_pair(AS_PROGRAM, program_config),
                    std.make_pair(AS_OPCODES, sprogram_config)
                };
            }
            else
            {
                return new space_config_vector()
                {
                    std.make_pair(AS_PROGRAM, program_config)
                };
            }
        }


        // device_state_interface overrides
        //virtual void state_import(const device_state_entry &entry) override;

        void device_state_interface_state_export(device_state_entry entry)
        {
            switch (entry.index())
            {
                case g.STATE_GENPC:     XPC = pc_to_external(PPC); break;
                case g.STATE_GENPCBASE: XPC = pc_to_external(NPC); break;
            }
        }

        //virtual void state_string_export(const device_state_entry &entry, std::string &str) const override;


        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        uint8_t read(uint16_t adr) { return mintf.read(adr); }
        //uint8_t read_9(uint16_t adr) { return mintf->read_9(adr); }
        void write(uint16_t adr, uint8_t val) { mintf.write(adr, val); }
        //void write_9(uint16_t adr, uint8_t val) { mintf->write_9(adr, val); }
        uint8_t read_arg(uint16_t adr) { return mintf.read_arg(adr); }
        uint8_t read_pc() { return mintf.read_arg(PC++); }
        uint8_t read_pc_noinc() { return mintf.read_arg(PC); }

        void prefetch()
        {
            sync = true;
            sync_w.op(g.ASSERT_LINE);
            NPC = PC;
            IR = mintf.read_sync(PC);
            sync = false;
            sync_w.op(g.CLEAR_LINE);

            if ((nmi_pending || ((irq_state || apu_irq_state) && (P & F_I) == 0)) && !inhibit_interrupts)
            {
                irq_taken = true;
                IR = 0x00;
            }
            else
            {
                PC++;
            }
        }


        //void prefetch_noirq();


        void set_nz(byte v)
        {
            P = (uint8_t)(P & ~(F_Z | F_N));
            if ((v & 0x80) != 0)
                P |= F_N;
            if (v == 0)
                P |= F_Z;
        }


        u32 XPC;
        protected virtual offs_t pc_to_external(u16 pc) { return pc; }  // For paged PCs


        // build/generated/emu/cpu/m6502/m6502.cs
        //virtual void do_exec_full();
        //virtual void do_exec_partial();


        // inline helpers
        static bool page_changing(uint16_t base_, int delta) { return (((base_ + delta) ^ base_) & 0xff00) != 0; }
        static uint16_t set_l(uint16_t base_, uint8_t val) { return (uint16_t)((base_ & 0xff00) | val); }
        static uint16_t set_h(uint16_t base_, uint8_t val) { return (uint16_t)((base_ & 0x00ff) | (val << 8)); }

        void dec_SP() { SP = set_l(SP, (byte)(SP-1)); }
        void inc_SP() { SP = set_l(SP, (byte)(SP+1)); }

        void do_adc_d(uint8_t val)
        {
            byte c = (P & F_C) != 0 ? (byte)1 : (byte)0;
            P = (uint8_t)(P & ~(F_N | F_V | F_Z | F_C));
            byte al = (byte)((A & 15) + (val & 15) + c);
            if(al > 9)
                al += 6;
            byte ah = (byte)((A >> 4) + (val >> 4) + ((al > 15) ? 1 : 0));
            if((byte)(A + val + c) == 0)
                P |= F_Z;
            else if((ah & 8) != 0)
                P |= F_N;
            if((~(A^val) & (A^(ah << 4)) & 0x80) != 0)
                P |= F_V;
            if(ah > 9)
                ah += 6;
            if(ah > 15)
                P |= F_C;
            A = (byte)((ah << 4) | (al & 15));
        }

        void do_adc_nd(uint8_t val)
        {
            UInt16 sum;
            sum = (UInt16)(A + val + ((P & F_C) != 0 ? 1 : 0));
            P = (uint8_t)(P & ~(F_N | F_V | F_Z | F_C));
            if((byte)(sum) == 0)
                P |= F_Z;
            else if((sbyte)(sum) < 0)
                P |= F_N;
            if((~(A^val) & (A^sum) & 0x80) != 0)
                P |= F_V;
            if((sum & 0xff00) != 0)
                P |= F_C;
            A = (byte)sum;
        }

        void do_sbc_d(uint8_t val)
        {
            byte c = (P & F_C) != 0 ? (byte)0 : (byte)1;
            P = (uint8_t)(P & ~(F_N | F_V | F_Z | F_C));
            UInt16 diff = (UInt16)(A - val - c);
            byte al = (byte)((A & 15) - (val & 15) - c);
            if((sbyte)(al) < 0)
                al -= 6;
            byte ah = (byte)((A >> 4) - (val >> 4) - (((sbyte)(al) < 0) ? 1 : 0));
            if((byte)(diff) == 0)
                P |= F_Z;
            else if((diff & 0x80) != 0)
                P |= F_N;
            if(((A^val) & (A^diff) & 0x80) != 0)
                P |= F_V;
            if((diff & 0xff00) == 0)
                P |= F_C;
            if((sbyte)(ah) < 0)
                ah -= 6;
            A = (byte)((ah << 4) | (al & 15));
        }

        void do_sbc_nd(uint8_t val)
        {
            UInt16 diff = (UInt16)(A - val - ((P & F_C) != 0 ? 0 : 1));
            P = (uint8_t)(P & ~(F_N | F_V | F_Z | F_C));
            if((byte)(diff) == 0)
                P |= F_Z;
            else if((sbyte)(diff) < 0)
                P |= F_N;
            if(((A^val) & (A^diff) & 0x80) != 0)
                P |= F_V;
            if((diff & 0xff00) == 0)
                P |= F_C;
            A = (byte)diff;
        }

        void do_arr_d()
        {
            // The adc/ror interaction gives an extremely weird result
            bool c = (P & F_C) != 0;
            P = (uint8_t)(P & ~(F_N | F_Z | F_C | F_V));
            byte a = (byte)(A >> 1);
            if(c)
                a |= 0x80;
            if(a == 0)
                P |= F_Z;
            else if((sbyte)(a) < 0)
                P |= F_N;
            if(((a ^ A) & 0x40) != 0)
                P |= F_V;

            if((A & 0x0f) >= 0x05)
                a = (byte)(((a + 6) & 0x0f) | (a & 0xf0));

            if((A & 0xf0) >= 0x50) {
                a += 0x60;
                P |= F_C;
            }
            A = a;
        }

        void do_arr_nd()
        {
            bool c = (P & F_C) != 0;
            P = (uint8_t)(P & ~(F_N | F_Z | F_C | F_V));
            A >>= 1;
            if(c)
                A |= 0x80;
            if(A == 0)
                P |= F_Z;
            else if((sbyte)(A)<0)
                P |= F_N;
            if((A & 0x40) != 0)
                P |= (F_V | F_C);
            if((A & 0x20) != 0)
                P ^= F_V;
        }

        void do_adc(uint8_t val)
        {
            if((P & F_D) != 0)
                do_adc_d(val);
            else
                do_adc_nd(val);
        }

        void do_cmp(uint8_t val1, uint8_t val2)
        {
            P = (uint8_t)(P & ~(F_N | F_Z | F_C));
            UInt16 r = (UInt16)(val1-val2);
            if(r == 0)
                P |= F_Z;
            else if((byte)(r) < 0)
                P |= F_N;
            if((r & 0xff00) == 0)
                P |= F_C;
        }

        void do_sbc(uint8_t val)
        {
            if((P & F_D) != 0)
                do_sbc_d(val);
            else
                do_sbc_nd(val);
        }

        void do_bit(uint8_t val)
        {
            P = (uint8_t)(P & ~(F_N | F_Z | F_V));
            byte r = (byte)(A & val);
            if(r == 0)
                P |= F_Z;
            if((val & 0x80) != 0)
                P |= F_N;
            if((val & 0x40) != 0)
                P |= F_V;
        }

        void do_arr()
        {
            if((P & F_D) != 0)
                do_arr_d();
            else
                do_arr_nd();
        }

        uint8_t do_asl(uint8_t v)
        {
            P = (uint8_t)(P & ~(F_N | F_Z | F_C));
            byte r = (byte)(v<<1);
            if(r == 0)
                P |= F_Z;
            else if((sbyte)(r) < 0)
                P |= F_N;
            if((v & 0x80) != 0)
                P |= F_C;
            return r;
        }

        uint8_t do_lsr(uint8_t v)
        {
            P = (uint8_t)(P & ~(F_N | F_Z | F_C));
            if((v & 1) != 0)
                P |= F_C;
            v >>= 1;
            if(v == 0)
                P |= F_Z;
            return v;
        }

        uint8_t do_ror(uint8_t v)
        {
            bool c = (P & F_C) != 0;
            P = (uint8_t)(P & ~(F_N | F_Z | F_C));
            if((v & 1) != 0)
                P |= F_C;
            v >>= 1;
            if(c)
                v |= 0x80;
            if(v == 0)
                P |= F_Z;
            else if((sbyte)(v)<0)
                P |= F_N;
            return v;
        }

        uint8_t do_rol(uint8_t v)
        {
            bool c = (P & F_C) != 0;
            P = (uint8_t)(P & ~(F_N | F_Z | F_C));
            if((v & 0x80) != 0)
                P |= F_C;
            v <<= 1;
            if(c)
                v |= 0x01;
            if(v == 0)
                P |= F_Z;
            else if((sbyte)(v)<0)
                P |= F_N;
            return v;
        }

        //uint8_t do_asr(uint8_t v);

        //#define O(o) void o ## _full(); void o ## _partial()

        // NMOS 6502 opcodes
        //   documented opcodes
        //O(adc_aba); O(adc_abx); O(adc_aby); O(adc_idx); O(adc_idy); O(adc_imm); O(adc_zpg); O(adc_zpx);
        //O(and_aba); O(and_abx); O(and_aby); O(and_imm); O(and_idx); O(and_idy); O(and_zpg); O(and_zpx);
        //O(asl_aba); O(asl_abx); O(asl_acc); O(asl_zpg); O(asl_zpx);
        //O(bcc_rel);
        //O(bcs_rel);
        //O(beq_rel);
        //O(bit_aba); O(bit_zpg);
        //O(bmi_rel);
        //O(bne_rel);
        //O(bpl_rel);
        //O(brk_imp);
        //O(bvc_rel);
        //O(bvs_rel);
        //O(clc_imp);
        //O(cld_imp);
        //O(cli_imp);
        //O(clv_imp);
        //O(cmp_aba); O(cmp_abx); O(cmp_aby); O(cmp_idx); O(cmp_idy); O(cmp_imm); O(cmp_zpg); O(cmp_zpx);
        //O(cpx_aba); O(cpx_imm); O(cpx_zpg);
        //O(cpy_aba); O(cpy_imm); O(cpy_zpg);
        //O(dec_aba); O(dec_abx); O(dec_zpg); O(dec_zpx);
        //O(dex_imp);
        //O(dey_imp);
        //O(eor_aba); O(eor_abx); O(eor_aby); O(eor_idx); O(eor_idy); O(eor_imm); O(eor_zpg); O(eor_zpx);
        //O(inc_aba); O(inc_abx); O(inc_zpg); O(inc_zpx);
        //O(inx_imp);
        //O(iny_imp);
        //O(jmp_adr); O(jmp_ind);
        //O(jsr_adr);
        //O(lda_aba); O(lda_abx); O(lda_aby); O(lda_idx); O(lda_idy); O(lda_imm); O(lda_zpg); O(lda_zpx);
        //O(ldx_aba); O(ldx_aby); O(ldx_imm); O(ldx_zpg); O(ldx_zpy);
        //O(ldy_aba); O(ldy_abx); O(ldy_imm); O(ldy_zpg); O(ldy_zpx);
        //O(lsr_aba); O(lsr_abx); O(lsr_acc); O(lsr_zpg); O(lsr_zpx);
        //O(nop_imp);
        //O(ora_aba); O(ora_abx); O(ora_aby); O(ora_imm); O(ora_idx); O(ora_idy); O(ora_zpg); O(ora_zpx);
        //O(pha_imp);
        //O(php_imp);
        //O(pla_imp);
        //O(plp_imp);
        //O(rol_aba); O(rol_abx); O(rol_acc); O(rol_zpg); O(rol_zpx);
        //O(ror_aba); O(ror_abx); O(ror_acc); O(ror_zpg); O(ror_zpx);
        //O(rti_imp);
        //O(rts_imp);
        //O(sbc_aba); O(sbc_abx); O(sbc_aby); O(sbc_idx); O(sbc_idy); O(sbc_imm); O(sbc_zpg); O(sbc_zpx);
        //O(sec_imp);
        //O(sed_imp);
        //O(sei_imp);
        //O(sta_aba); O(sta_abx); O(sta_aby); O(sta_idx); O(sta_idy); O(sta_zpg); O(sta_zpx);
        //O(stx_aba); O(stx_zpg); O(stx_zpy);
        //O(sty_aba); O(sty_zpg); O(sty_zpx);
        //O(tax_imp);
        //O(tay_imp);
        //O(tsx_imp);
        //O(txa_imp);
        //O(txs_imp);
        //O(tya_imp);

        //   exceptions
        //O(reset);

        //   undocumented reliable instructions
        //O(dcp_aba); O(dcp_abx); O(dcp_aby); O(dcp_idx); O(dcp_idy); O(dcp_zpg); O(dcp_zpx);
        //O(isb_aba); O(isb_abx); O(isb_aby); O(isb_idx); O(isb_idy); O(isb_zpg); O(isb_zpx);
        //O(lax_aba); O(lax_aby); O(lax_idx); O(lax_idy); O(lax_zpg); O(lax_zpy);
        //O(rla_aba); O(rla_abx); O(rla_aby); O(rla_idx); O(rla_idy); O(rla_zpg); O(rla_zpx);
        //O(rra_aba); O(rra_abx); O(rra_aby); O(rra_idx); O(rra_idy); O(rra_zpg); O(rra_zpx);
        //O(sax_aba); O(sax_idx); O(sax_zpg); O(sax_zpy);
        //O(sbx_imm);
        //O(sha_aby); O(sha_idy);
        //O(shs_aby);
        //O(shx_aby);
        //O(shy_abx);
        //O(slo_aba); O(slo_abx); O(slo_aby); O(slo_idx); O(slo_idy); O(slo_zpg); O(slo_zpx);
        //O(sre_aba); O(sre_abx); O(sre_aby); O(sre_idx); O(sre_idy); O(sre_zpg); O(sre_zpx);

        //   undocumented unreliable instructions
        //     behaviour differs between visual6502 and online docs, which
        //     is a clear sign reliability is not to be expected
        //     implemented version follows visual6502
        //O(anc_imm);
        //O(ane_imm);
        //O(arr_imm);
        //O(asr_imm);
        //O(las_aby);
        //O(lxa_imm);

        //   nop variants
        //O(nop_imm); O(nop_aba); O(nop_abx); O(nop_zpg); O(nop_zpx);

        //   system killers
        //O(kil_non);

        //#undef O
    }


    //class m6502_mcu_device : public m6502_device {

    //class m6512_device : public m6502_device {
}
