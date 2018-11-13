// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using space_config_vector = mame.std_vector<System.Collections.Generic.KeyValuePair<int, mame.address_space_config>>;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    enum M6502_REG
    {
        M6502_PC = 1,
        M6502_A,
        M6502_X,
        M6502_Y,
        M6502_P,
        M6502_S,
        M6502_IR
    }


    public class device_execute_interface_m6502 : device_execute_interface
    {
        public device_execute_interface_m6502(machine_config mconfig, device_t device) : base(mconfig, device) { }

        // device_execute_interface overrides
        public override uint32_t execute_min_cycles() { return 1; }
        public override uint32_t execute_max_cycles() { return 10; }
        public override uint32_t execute_input_lines() { return (uint32_t)m6502_device.LINE.NMI_LINE + 1; }

        public override void execute_run()
        {
            m6502_device m6502 = (m6502_device)device();

            m6502.device_execute_interface_execute_run();
        }

        public override void execute_set_input(int inputnum, int state)
        {
            m6502_device m6502 = (m6502_device)device();

            switch (inputnum)
            {
            case (int)m6502_device.LINE.IRQ_LINE: m6502.irq_state = state == (int)line_state.ASSERT_LINE; break;
            case (int)m6502_device.LINE.APU_IRQ_LINE: m6502.apu_irq_state = state == (int)line_state.ASSERT_LINE; break;
            case (int)m6502_device.LINE.NMI_LINE: m6502.nmi_state = m6502.nmi_state || (state == (int)line_state.ASSERT_LINE); break;
            case (int)m6502_device.LINE.V_LINE:
                if (!m6502.v_state && state == (int)line_state.ASSERT_LINE)
                    m6502.P |= (byte)m6502_device.F.F_V;
                m6502.v_state = state == (int)line_state.ASSERT_LINE;
                break;
            }
        }

        public override bool execute_input_edge_triggered(int inputnum)
        {
            return inputnum == (int)m6502_device.LINE.NMI_LINE;
        }
    }


    public class device_memory_interface_m6502 : device_memory_interface
    {
        public device_memory_interface_m6502(machine_config mconfig, device_t device) : base(mconfig, device) { }

        // device_memory_interface overrides
        public override space_config_vector memory_space_config()
        {
            m6502_device m6502 = (m6502_device)device();

            if (has_configured_map(emumem_global.AS_OPCODES))
            {
                return new space_config_vector()
                {
                    global.make_pair(emumem_global.AS_PROGRAM, m6502.program_config),
                    global.make_pair(emumem_global.AS_OPCODES, m6502.sprogram_config)
                };
            }
            else
            {
                return new space_config_vector()
                {
                    global.make_pair(emumem_global.AS_PROGRAM, m6502.program_config)
                };
            }
        }
    }


    public class device_state_interface_m6502 : device_state_interface
    {
        public device_state_interface_m6502(machine_config mconfig, device_t device) : base(mconfig, device) { }

        // device_state_interface overrides
        public override void state_import(device_state_entry entry)
        {
            throw new emu_unimplemented();
        }

        public override void state_export(device_state_entry entry)
        {
            m6502_device m6502 = (m6502_device)device();

            m6502.device_state_interface_state_export(entry);
        }

        public override void state_string_export(device_state_entry entry, out string str)
        {
            throw new emu_unimplemented();
        }
    }


    public class device_disasm_interface_m6502 : device_disasm_interface
    {
        public device_disasm_interface_m6502(machine_config mconfig, device_t device) : base(mconfig, device) { }

        // device_disasm_interface overrides
        protected override util.disasm_interface create_disassembler()
        {
            throw new emu_unimplemented();
        }
    }


    partial class m6502_device : cpu_device
    {
        //DEFINE_DEVICE_TYPE(M6502, m6502_device, "m6502", "MOS Technology M6502")
        static device_t device_creator_mb6502_cpu_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new m6502_device(mconfig, tag, owner, clock); }
        public static readonly device_type M6502 = DEFINE_DEVICE_TYPE(device_creator_mb6502_cpu_device, "m6502", "MOS Technology M6502");


        public enum LINE
        {
            IRQ_LINE = INPUT_LINE.INPUT_LINE_IRQ0,
            APU_IRQ_LINE = INPUT_LINE.INPUT_LINE_IRQ1,
            NMI_LINE = INPUT_LINE.INPUT_LINE_NMI,
            V_LINE   = INPUT_LINE.INPUT_LINE_IRQ0 + 16
        }


        enum M6502_STATE
        {
            STATE_RESET = 0xff00
        }


        public enum F
        {
            F_N = 0x80,
            F_V = 0x40,
            F_E = 0x20, // 65ce02
            F_T = 0x20, // M740: replaces A with $00,X in some opcodes when set
            F_B = 0x10,
            F_D = 0x08,
            F_I = 0x04,
            F_Z = 0x02,
            F_C = 0x01
        }


        abstract class memory_interface
        {
            public address_space program;
            public address_space sprogram;
            public memory_access_cache/*<0, 0, ENDIANNESS_LITTLE>*/ cache;
            public memory_access_cache/*<0, 0, ENDIANNESS_LITTLE>*/ scache;

            //virtual ~memory_interface() {}
            public abstract uint8_t read(uint16_t adr);
            //virtual uint8_t read_9(uint16_t adr);
            public abstract uint8_t read_sync(uint16_t adr);
            public abstract uint8_t read_arg(uint16_t adr);
            public abstract void write(uint16_t adr, uint8_t val);
            //virtual void write_9(uint16_t adr, uint8_t val);
        }


        class mi_default_normal : memory_interface
        {
            //virtual ~mi_default_normal() {}

            public override uint8_t read(uint16_t adr) { return program.read_byte(adr); }
            public override uint8_t read_sync(uint16_t adr) { return scache.read_byte(adr); }
            public override uint8_t read_arg(uint16_t adr) { return cache.read_byte(adr); }
            public override void write(uint16_t adr, uint8_t val) { program.write_byte(adr, val); }
        }


        class mi_default_nd : mi_default_normal
        {
            //virtual ~mi_default_nd() {}
            //virtual uint8_t read_sync(uint16_t adr) override;
            //virtual uint8_t read_arg(uint16_t adr) override;
        }


        device_memory_interface_m6502 m_dimemory;
        device_execute_interface_m6502 m_diexec;
        device_state_interface_m6502 m_distate;


        devcb_write_line sync_w;

        public address_space_config program_config;
        public address_space_config sprogram_config;

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
        intref icountRef = new intref();  //int icount;
        int bcount;
        int count_before_instruction_step;
        public bool nmi_state;
        public bool irq_state;
        public bool apu_irq_state;
        public bool v_state;
        bool irq_taken;
        bool sync;
        bool cache_disabled;
        bool inhibit_interrupts;


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
            icountRef.i = 0;  //icount = 0;
            nmi_state = false;
            irq_state = false;
            apu_irq_state = false;
            v_state = false;
            irq_taken = false;
            sync = false;
            inhibit_interrupts = false;
            count_before_instruction_step = 0;


            cache_disabled = false;
        }


        int icount { get { return icountRef.i; } set { icountRef.i = value; } }


        //bool get_sync() const { return sync; }
        //void disable_direct() { direct_disabled = true; }

        //template<class Object> devcb_base &set_sync_callback(Object &&cb) { return sync_w.set_callback(std::forward<Object>(cb)); }


        protected virtual void init()
        {
            mintf.program  = m_dimemory.space(emumem_global.AS_PROGRAM);
            mintf.sprogram = m_dimemory.has_space(emumem_global.AS_OPCODES) ? m_dimemory.space(emumem_global.AS_OPCODES) : mintf.program;

            mintf.cache  = mintf.program.cache(0, 0, (int)endianness_t.ENDIANNESS_LITTLE);
            mintf.scache = mintf.sprogram.cache(0, 0, (int)endianness_t.ENDIANNESS_LITTLE);

            XPC = 0;

            sync_w.resolve_safe();

            m_distate.state_add((int)STATE.STATE_GENPC,     "GENPC",     XPC).callexport().noshow();
            m_distate.state_add((int)STATE.STATE_GENPCBASE, "CURPC",     XPC).callexport().noshow();
            m_distate.state_add((int)STATE.STATE_GENSP,     "GENSP",     SP).noshow();
            m_distate.state_add((int)STATE.STATE_GENFLAGS,  "GENFLAGS",  P).callimport().formatstr("%6s").noshow();
            m_distate.state_add((int)M6502_REG.M6502_PC,        "PC",        NPC).callimport();
            m_distate.state_add((int)M6502_REG.M6502_A,         "A",         A);
            m_distate.state_add((int)M6502_REG.M6502_X,         "X",         X);
            m_distate.state_add((int)M6502_REG.M6502_Y,         "Y",         Y);
            m_distate.state_add((int)M6502_REG.M6502_P,         "P",         P).callimport();
            m_distate.state_add((int)M6502_REG.M6502_S,         "SP",        SP);
            m_distate.state_add((int)M6502_REG.M6502_IR,        "IR",        IR);

            save_item(PC, "PC");
            save_item(NPC, "NPC");
            save_item(PPC, "PPC");
            save_item(A, "A");
            save_item(X, "X");
            save_item(Y, "Y");
            save_item(P, "P");
            save_item(SP, "SP");
            save_item(TMP, "TMP");
            save_item(TMP2, "TMP2");
            save_item(IR, "IR");
            save_item(nmi_state, "nmi_state");
            save_item(irq_state, "irq_state");
            save_item(apu_irq_state, "apu_irq_state");
            save_item(v_state, "v_state");
            save_item(inst_state, "inst_state");
            save_item(inst_substate, "inst_substate");
            save_item(inst_state_base, "inst_state_base");
            save_item(irq_taken, "irq_taken");
            save_item(inhibit_interrupts, "inhibit_interrupts");

            execute().set_icountptr(icountRef);

            PC = 0x0000;
            NPC = 0x0000;
            A = 0x00;
            X = 0x80;
            Y = 0x00;
            P = 0x36;
            SP = 0x01bd;
            TMP = 0x0000;
            TMP2 = 0x00;
            IR = 0x00;
            nmi_state = false;
            irq_state = false;
            apu_irq_state = false;
            irq_taken = false;
            v_state = false;
            inst_state = (int)M6502_STATE.STATE_RESET;
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


            if (cache_disabled)
                mintf = new mi_default_nd();
            else
                mintf = new mi_default_normal();

            init();
        }


        protected override void device_reset()
        {
            inst_state = (int)M6502_STATE.STATE_RESET;
            inst_substate = 0;
            inst_state_base = 0;
            nmi_state = false;
            irq_state = false;
            apu_irq_state = false;
            irq_taken = false;
            v_state = false;
            sync = false;
            sync_w.op((int)line_state.CLEAR_LINE);
            inhibit_interrupts = false;
        }


        // device_execute_interface overrides
        //virtual uint32_t execute_min_cycles() const override;
        //virtual uint32_t execute_max_cycles() const override;
        //virtual uint32_t execute_input_lines() const override;
        //virtual void execute_run() override;
        //virtual void execute_set_input(int inputnum, int state) override;
        //virtual bool execute_input_edge_triggered(int inputnum) const override;


        // device_memory_interface overrides
        //virtual space_config_vector memory_space_config() const override;


        // device_state_interface overrides
        //virtual void state_import(const device_state_entry &entry) override;
        //virtual void state_export(const device_state_entry &entry) override;
        //virtual void state_string_export(const device_state_entry &entry, std::string &str) const override;


        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        public void device_state_interface_state_export(device_state_entry entry)
        {
            switch (entry.index())
            {
                case (int)STATE.STATE_GENFLAGS:
                case (int)M6502_REG.M6502_P:
                    P = (uint8_t)(P | (uint8_t)(F.F_B|F.F_E));
                    break;

                case (int)M6502_REG.M6502_PC:
                    PC = NPC;
                    irq_taken = false;
                    prefetch();
                    PPC = NPC;
                    inst_state = IR | inst_state_base;
                    break;

                default:
                    break;
            }
        }


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
            sync_w.op((int)line_state.ASSERT_LINE);
            NPC = PC;
            IR = mintf.read_sync(PC);
            sync = false;
            sync_w.op((int)line_state.CLEAR_LINE);

            if ((nmi_state || ((irq_state || apu_irq_state) && (P & (byte)F.F_I) == 0)) && !inhibit_interrupts)
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
            P &= unchecked((byte)(~(F.F_Z|F.F_N)));
            if ((v & 0x80) != 0)
                P |= (byte)F.F_N;
            if (v == 0)
                P |= (byte)F.F_Z;
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
            byte c = (P & (byte)F.F_C) != 0 ? (byte)1 : (byte)0;
            P &= unchecked((byte)(~(F.F_N|F.F_V|F.F_Z|F.F_C)));
            byte al = (byte)((A & 15) + (val & 15) + c);
            if(al > 9)
                al += 6;
            byte ah = (byte)((A >> 4) + (val >> 4) + ((al > 15) ? 1 : 0));
            if((byte)(A + val + c) == 0)
                P |= (byte)F.F_Z;
            else if((ah & 8) != 0)
                P |= (byte)F.F_N;
            if((~(A^val) & (A^(ah << 4)) & 0x80) != 0)
                P |= (byte)F.F_V;
            if(ah > 9)
                ah += 6;
            if(ah > 15)
                P |= (byte)F.F_C;
            A = (byte)((ah << 4) | (al & 15));
        }

        void do_adc_nd(uint8_t val)
        {
            UInt16 sum;
            sum = (UInt16)(A + val + ((P & (byte)F.F_C) != 0 ? 1 : 0));
            P &= unchecked((byte)(~(F.F_N|F.F_V|F.F_Z|F.F_C)));
            if((byte)(sum) == 0)
                P |= (byte)F.F_Z;
            else if((sbyte)(sum) < 0)
                P |= (byte)F.F_N;
            if((~(A^val) & (A^sum) & 0x80) != 0)
                P |= (byte)F.F_V;
            if((sum & 0xff00) != 0)
                P |= (byte)F.F_C;
            A = (byte)sum;
        }

        void do_sbc_d(uint8_t val)
        {
            byte c = (P & (byte)F.F_C) != 0 ? (byte)0 : (byte)1;
            P &= unchecked((byte)(~(F.F_N|F.F_V|F.F_Z|F.F_C)));
            UInt16 diff = (UInt16)(A - val - c);
            byte al = (byte)((A & 15) - (val & 15) - c);
            if((sbyte)(al) < 0)
                al -= 6;
            byte ah = (byte)((A >> 4) - (val >> 4) - (((sbyte)(al) < 0) ? 1 : 0));
            if((byte)(diff) == 0)
                P |= (byte)F.F_Z;
            else if((diff & 0x80) != 0)
                P |= (byte)F.F_N;
            if(((A^val) & (A^diff) & 0x80) != 0)
                P |= (byte)F.F_V;
            if((diff & 0xff00) == 0)
                P |= (byte)F.F_C;
            if((sbyte)(ah) < 0)
                ah -= 6;
            A = (byte)((ah << 4) | (al & 15));
        }

        void do_sbc_nd(uint8_t val)
        {
            UInt16 diff = (UInt16)(A - val - ((P & (byte)F.F_C) != 0 ? 0 : 1));
            P &= unchecked((byte)(~(F.F_N|F.F_V|F.F_Z|F.F_C)));
            if((byte)(diff) == 0)
                P |= (byte)F.F_Z;
            else if((sbyte)(diff) < 0)
                P |= (byte)F.F_N;
            if(((A^val) & (A^diff) & 0x80) != 0)
                P |= (byte)F.F_V;
            if((diff & 0xff00) == 0)
                P |= (byte)F.F_C;
            A = (byte)diff;
        }

        void do_arr_d()
        {
            // The adc/ror interaction gives an extremely weird result
            bool c = (P & (byte)F.F_C) != 0;
            P &= unchecked((byte)(~(F.F_N|F.F_Z|F.F_C|F.F_V)));
            byte a = (byte)(A >> 1);
            if(c)
                a |= 0x80;
            if(a == 0)
                P |= (byte)F.F_Z;
            else if((sbyte)(a) < 0)
                P |= (byte)F.F_N;
            if(((a ^ A) & 0x40) != 0)
                P |= (byte)F.F_V;

            if((A & 0x0f) >= 0x05)
                a = (byte)(((a + 6) & 0x0f) | (a & 0xf0));

            if((A & 0xf0) >= 0x50) {
                a += 0x60;
                P |= (byte)F.F_C;
            }
            A = a;
        }

        void do_arr_nd()
        {
            bool c = (P & (byte)F.F_C) != 0;
            P &= unchecked((byte)(~(F.F_N|F.F_Z|F.F_C|F.F_V)));
            A >>= 1;
            if(c)
                A |= 0x80;
            if(A == 0)
                P |= (byte)F.F_Z;
            else if((sbyte)(A)<0)
                P |= (byte)F.F_N;
            if((A & 0x40) != 0)
                P |= (byte)(F.F_V|F.F_C);
            if((A & 0x20) != 0)
                P ^= (byte)F.F_V;
        }

        void do_adc(uint8_t val)
        {
            if((P & (byte)F.F_D) != 0)
                do_adc_d(val);
            else
                do_adc_nd(val);
        }

        void do_cmp(uint8_t val1, uint8_t val2)
        {
            P &= unchecked((byte)(~(F.F_N|F.F_Z|F.F_C)));
            UInt16 r = (UInt16)(val1-val2);
            if(r == 0)
                P |= (byte)F.F_Z;
            else if((byte)(r) < 0)
                P |= (byte)F.F_N;
            if((r & 0xff00) == 0)
                P |= (byte)F.F_C;
        }

        void do_sbc(uint8_t val)
        {
            if((P & (byte)F.F_D) != 0)
                do_sbc_d(val);
            else
                do_sbc_nd(val);
        }

        void do_bit(uint8_t val)
        {
            P &= unchecked((byte)(~(F.F_N|F.F_Z|F.F_V)));
            byte r = (byte)(A & val);
            if(r == 0)
                P |= (byte)F.F_Z;
            if((val & 0x80) != 0)
                P |= (byte)F.F_N;
            if((val & 0x40) != 0)
                P |= (byte)F.F_V;
        }

        void do_arr()
        {
            if((P & (byte)F.F_D) != 0)
                do_arr_d();
            else
                do_arr_nd();
        }

        uint8_t do_asl(uint8_t v)
        {
            P &= unchecked((byte)(~(F.F_N|F.F_Z|F.F_C)));
            byte r = (byte)(v<<1);
            if(r == 0)
                P |= (byte)F.F_Z;
            else if((sbyte)(r) < 0)
                P |= (byte)F.F_N;
            if((v & 0x80) != 0)
                P |= (byte)F.F_C;
            return r;
        }

        uint8_t do_lsr(uint8_t v)
        {
            P &= unchecked((byte)(~(F.F_N|F.F_Z|F.F_C)));
            if((v & 1) != 0)
                P |= (byte)F.F_C;
            v >>= 1;
            if(v == 0)
                P |= (byte)F.F_Z;
            return v;
        }

        uint8_t do_ror(uint8_t v)
        {
            bool c = (P & (byte)F.F_C) != 0;
            P &= unchecked((byte)(~(F.F_N|F.F_Z|F.F_C)));
            if((v & 1) != 0)
                P |= (byte)F.F_C;
            v >>= 1;
            if(c)
                v |= 0x80;
            if(v == 0)
                P |= (byte)F.F_Z;
            else if((sbyte)(v)<0)
                P |= (byte)F.F_N;
            return v;
        }

        uint8_t do_rol(uint8_t v)
        {
            bool c = (P & (byte)F.F_C) != 0;
            P &= unchecked((byte)(~(F.F_N|F.F_Z|F.F_C)));
            if((v & 0x80) != 0)
                P |= (byte)F.F_C;
            v <<= 1;
            if(c)
                v |= 0x01;
            if(v == 0)
                P |= (byte)F.F_Z;
            else if((sbyte)(v)<0)
                P |= (byte)F.F_N;
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


        public void device_execute_interface_execute_run()
        {
            if (inst_substate != 0)
                do_exec_partial();

            while (icountRef.i > 0)
            {
                if (inst_state < 0xff00)
                {
                    PPC = NPC;
                    inst_state = IR | inst_state_base;
                    if ((machine().debug_flags_get & running_machine.DEBUG_FLAG_ENABLED) != 0)
                        execute().debugger_instruction_hook(pc_to_external(NPC));
                }

                do_exec_full();
            }
        }
    }
}
