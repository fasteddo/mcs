// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;
using unsigned = System.UInt32;


namespace mame
{
    static class m6805_global
    {
        public const int M6805_IRQ_LINE = 0;
    }


    // ======================> m6805_base_device
    // Used by core CPU interface
    public partial class m6805_base_device : cpu_device
    {
        protected class device_execute_interface_m6805_base : device_execute_interface
        {
            public device_execute_interface_m6805_base(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint32_t execute_min_cycles() { return ((m6805_base_device)device()).device_execute_interface_execute_min_cycles(); }
            protected override uint32_t execute_max_cycles() { throw new emu_unimplemented(); }
            protected override uint32_t execute_input_lines() { throw new emu_unimplemented(); }
            protected override void execute_run() { ((m6805_base_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { throw new emu_unimplemented(); }
            protected override uint64_t execute_clocks_to_cycles(uint64_t clocks) { return (clocks + 3) / 4; }
            protected override uint64_t execute_cycles_to_clocks(uint64_t cycles) { return cycles * 4; }
            protected override bool execute_input_edge_triggered(int inputnum) { throw new emu_unimplemented(); }
        }


        protected class device_memory_interface_m6805_base : device_memory_interface
        {
            public device_memory_interface_m6805_base(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((m6805_base_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_m6805_base : device_state_interface
        {
            public device_state_interface_m6805_base(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override void state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }
        }


        protected class device_disasm_interface_m6805_base : device_disasm_interface
        {
            public device_disasm_interface_m6805_base(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        // addressing mode selector for opcode handler templates
        enum addr_mode { IM, DI, EX, IX, IX1, IX2 };


        // state index constants
        //enum
        //{
        protected const int M6805_PC = 1;
        protected const int M6805_S  = 2;
        protected const int M6805_CC = 3;
        protected const int M6805_A  = 4;
        protected const int M6805_X  = 5;
        protected const int M6805_IRQ_STATE = 6;
        //};


        // CC masks      H INZC
        //            7654 3210
        //enum
        //{
        const u8 CFLAG = 0x01;
        const u8 ZFLAG = 0x02;
        const u8 NFLAG = 0x04;
        protected const u8 IFLAG = 0x08;
        const u8 HFLAG = 0x10;
        //};


        static string OP(string name) { return string.Format("{0}_{1}", name, s_big ? "true" : "false"); }  //#define OP(name)        (&m6805_base_device::name)
        static string OPN(string name, int n) { return string.Format("{0}_{1}_{2}", name, s_big ? "true" : "false", n); }  //#define OPN(name,n)     (&m6805_base_device::name<big, n>)
        static string OP_T(string name) { return string.Format("{0}_{1}_{2}", name, s_big ? "true" : "false", "true"); }  //#define OP_T(name)      (&m6805_base_device::name<true>)
        static string OP_F(string name) { return string.Format("{0}_{1}_{2}", name, s_big ? "true" : "false", "false"); }  //#define OP_F(name)      (&m6805_base_device::name<false>)
        static string OP_IM(string name) { return string.Format("{0}_{1}_{2}", name, s_big ? "true" : "false", "IM"); }  //#define OP_IM(name)     (&m6805_base_device::name<addr_mode::IM>)
        static string OP_DI(string name) { return string.Format("{0}_{1}_{2}", name, s_big ? "true" : "false", "DI"); }  //#define OP_DI(name)     (&m6805_base_device::name<addr_mode::DI>)
        static string OP_EX(string name) { return string.Format("{0}_{1}_{2}", name, s_big ? "true" : "false", "EX"); }  //#define OP_EX(name)     (&m6805_base_device::name<addr_mode::EX>)
        static string OP_IX(string name) { return string.Format("{0}_{1}_{2}", name, s_big ? "true" : "false", "IX"); }  //#define OP_IX(name)     (&m6805_base_device::name<addr_mode::IX>)
        static string OP_IX1(string name) { return string.Format("{0}_{1}_{2}", name, s_big ? "true" : "false", "IX1"); }  //#define OP_IX1(name)    (&m6805_base_device::name<addr_mode::IX1>)
        static string OP_IX2(string name) { return string.Format("{0}_{1}_{2}", name, s_big ? "true" : "false", "IX2"); }  //#define OP_IX2(name)    (&m6805_base_device::name<addr_mode::IX2>)


        // opcode tables

        static bool s_big;  //#define big false

        protected static op_handler_func [] s_hmos_s_ops;  //static op_handler_table s_hmos_s_ops;

        void init_s_hmos_s_ops()
        {
            s_big = false;
            string [] hmos_s_ops = new string []
            {
                /*      0/8          1/9          2/A          3/B          4/C          5/D          6/E          7/F */
                /* 0 */ OPN("brset",0),OPN("brclr",0),OPN("brset",1),OPN("brclr",1),OPN("brset",2),OPN("brclr",2),OPN("brset",3),OPN("brclr",3),
                        OPN("brset",4),OPN("brclr",4),OPN("brset",5),OPN("brclr",5),OPN("brset",6),OPN("brclr",6),OPN("brset",7),OPN("brclr",7),
                /* 1 */ OPN("bset",0), OPN("bclr",0), OPN("bset",1), OPN("bclr",1), OPN("bset",2), OPN("bclr",2), OPN("bset",3), OPN("bclr",3),
                        OPN("bset",4), OPN("bclr",4), OPN("bset",5), OPN("bclr",5), OPN("bset",6), OPN("bclr",6), OPN("bset",7), OPN("bclr",7),
                /* 2 */ OP_T("bra"),   OP_F("bra"),   OP_T("bhi"),   OP_F("bhi"),   OP_T("bcc"),   OP_F("bcc"),   OP_T("bne"),   OP_F("bne"),
                        OP_T("bhcc"),  OP_F("bhcc"),  OP_T("bpl"),   OP_F("bpl"),   OP_T("bmc"),   OP_F("bmc"),   OP_T("bil"),   OP_F("bil"),
                /* 3 */ OP_DI("neg"),  OP("illegal"), OP("illegal"), OP_DI("com"),  OP_DI("lsr"),  OP("illegal"), OP_DI("ror"),  OP_DI("asr"),
                        OP_DI("lsl"),  OP_DI("rol"),  OP_DI("dec"),  OP("illegal"), OP_DI("inc"),  OP_DI("tst"),  OP("illegal"), OP_DI("clr"),
                /* 4 */ OP("nega"),    OP("illegal"), OP("illegal"), OP("coma"),    OP("lsra"),    OP("illegal"), OP("rora"),    OP("asra"),
                        OP("lsla"),    OP("rola"),    OP("deca"),    OP("illegal"), OP("inca"),    OP("tsta"),    OP("illegal"), OP("clra"),
                /* 5 */ OP("negx"),    OP("illegal"), OP("illegal"), OP("comx"),    OP("lsrx"),    OP("illegal"), OP("rorx"),    OP("asrx"),
                        OP("lslx"),    OP("rolx"),    OP("decx"),    OP("illegal"), OP("incx"),    OP("tstx"),    OP("illegal"), OP("clrx"),
                /* 6 */ OP_IX1("neg"), OP("illegal"), OP("illegal"), OP_IX1("com"), OP_IX1("lsr"), OP("illegal"), OP_IX1("ror"), OP_IX1("asr"),
                        OP_IX1("lsl"), OP_IX1("rol"), OP_IX1("dec"), OP("illegal"), OP_IX1("inc"), OP_IX1("tst"), OP("illegal"), OP_IX1("clr"),
                /* 7 */ OP_IX("neg"),  OP("illegal"), OP("illegal"), OP_IX("com"),  OP_IX("lsr"),  OP("illegal"), OP_IX("ror"),  OP_IX("asr"),
                        OP_IX("lsl"),  OP_IX("rol"),  OP_IX("dec"),  OP("illegal"), OP_IX("inc"),  OP_IX("tst"),  OP("illegal"), OP_IX("clr"),
                /* 8 */ OP("rti"),     OP("rts"),     OP("illegal"), OP("swi"),     OP("illegal"), OP("illegal"), OP("illegal"), OP("illegal"),
                        OP("illegal"), OP("illegal"), OP("illegal"), OP("illegal"), OP("illegal"), OP("illegal"), OP("illegal"), OP("illegal"),
                /* 9 */ OP("illegal"), OP("illegal"), OP("illegal"), OP("illegal"), OP("illegal"), OP("illegal"), OP("illegal"), OP("tax"),
                        OP("clc"),     OP("sec"),     OP("cli"),     OP("sei"),     OP("rsp"),     OP("nop"),     OP("illegal"), OP("txa"),
                /* A */ OP_IM("suba"), OP_IM("cmpa"), OP_IM("sbca"), OP_IM("cpx"),  OP_IM("anda"), OP_IM("bita"), OP_IM("lda"),  OP("illegal"),
                        OP_IM("eora"), OP_IM("adca"), OP_IM("ora"),  OP_IM("adda"), OP("illegal"), OP("bsr"),     OP_IM("ldx"),  OP("illegal"),
                /* B */ OP_DI("suba"), OP_DI("cmpa"), OP_DI("sbca"), OP_DI("cpx"),  OP_DI("anda"), OP_DI("bita"), OP_DI("lda"),  OP_DI("sta"),
                        OP_DI("eora"), OP_DI("adca"), OP_DI("ora"),  OP_DI("adda"), OP_DI("jmp"),  OP_DI("jsr"),  OP_DI("ldx"),  OP_DI("stx"),
                /* C */ OP_EX("suba"), OP_EX("cmpa"), OP_EX("sbca"), OP_EX("cpx"),  OP_EX("anda"), OP_EX("bita"), OP_EX("lda"),  OP_EX("sta"),
                        OP_EX("eora"), OP_EX("adca"), OP_EX("ora"),  OP_EX("adda"), OP_EX("jmp"),  OP_EX("jsr"),  OP_EX("ldx"),  OP_EX("stx"),
                /* D */ OP_IX2("suba"),OP_IX2("cmpa"),OP_IX2("sbca"),OP_IX2("cpx"), OP_IX2("anda"),OP_IX2("bita"),OP_IX2("lda"), OP_IX2("sta"),
                        OP_IX2("eora"),OP_IX2("adca"),OP_IX2("ora"), OP_IX2("adda"),OP_IX2("jmp"), OP_IX2("jsr"), OP_IX2("ldx"), OP_IX2("stx"),
                /* E */ OP_IX1("suba"),OP_IX1("cmpa"),OP_IX1("sbca"),OP_IX1("cpx"), OP_IX1("anda"),OP_IX1("bita"),OP_IX1("lda"), OP_IX1("sta"),
                        OP_IX1("eora"),OP_IX1("adca"),OP_IX1("ora"), OP_IX1("adda"),OP_IX1("jmp"), OP_IX1("jsr"), OP_IX1("ldx"), OP_IX1("stx"),
                /* F */ OP_IX("suba"), OP_IX("cmpa"), OP_IX("sbca"), OP_IX("cpx"),  OP_IX("anda"), OP_IX("bita"), OP_IX("lda"),  OP_IX("sta"),
                        OP_IX("eora"), OP_IX("adca"), OP_IX("ora"),  OP_IX("adda"), OP_IX("jmp"),  OP_IX("jsr"),  OP_IX("ldx"),  OP_IX("stx")
            };

            // https://www.red-gate.com/simple-talk/blogs/introduction-to-open-instance-delegates/
            s_hmos_s_ops = new op_handler_func[hmos_s_ops.Length];
            for (int i = 0; i < hmos_s_ops.Length; i++)
            {
                string methodName = hmos_s_ops[i];
                MethodInfo methodInfo = typeof(m6805_base_device).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                s_hmos_s_ops[i] = (op_handler_func)methodInfo.CreateDelegate(typeof(op_handler_func), null);
            }
        }


        protected static op_handler_func [] s_hmos_b_ops;  //static op_handler_table s_hmos_b_ops;

        void init_s_hmos_b_ops()
        {
            throw new emu_unimplemented();


            s_big = true;
            string [] hmos_b_ops = new string []
            {
            };

            // https://www.red-gate.com/simple-talk/blogs/introduction-to-open-instance-delegates/
            s_hmos_b_ops = new op_handler_func[hmos_b_ops.Length];
            for (int i = 0; i < hmos_b_ops.Length; i++)
            {
                string methodName = hmos_b_ops[i];
                MethodInfo methodInfo = typeof(m6805_base_device).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                s_hmos_b_ops[i] = (op_handler_func)methodInfo.CreateDelegate(typeof(op_handler_func), null);
            }
        }


        //static op_handler_table s_cmos_b_ops;
        //static op_handler_table s_hc_s_ops;
        //static op_handler_table s_hc_b_ops;


        protected static readonly u8 [] s_hmos_cycles = new u8 []
        {
                /* 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F */
            /*0*/ 10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,10,
            /*1*/  7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7,
            /*2*/  4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4,
            /*3*/  6, 0, 0, 6, 6, 0, 6, 6, 6, 6, 6, 0, 6, 6, 0, 6,
            /*4*/  4, 0, 0, 4, 4, 0, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4,
            /*5*/  4, 0, 0, 4, 4, 0, 4, 4, 4, 4, 4, 0, 4, 4, 0, 4,
            /*6*/  7, 0, 0, 7, 7, 0, 7, 7, 7, 7, 7, 0, 7, 7, 0, 7,
            /*7*/  6, 0, 0, 6, 6, 0, 6, 6, 6, 6, 6, 0, 6, 6, 0, 6,
            /*8*/  9, 6, 0,11, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            /*9*/  0, 0, 0, 0, 0, 0, 0, 2, 2, 2, 2, 2, 2, 2, 0, 2,
            /*A*/  2, 2, 2, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 8, 2, 0,
            /*B*/  4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 3, 7, 4, 5,
            /*C*/  5, 5, 5, 5, 5, 5, 5, 6, 5, 5, 5, 5, 4, 8, 5, 6,
            /*D*/  6, 6, 6, 6, 6, 6, 6, 7, 6, 6, 6, 6, 5, 9, 6, 7,
            /*E*/  5, 5, 5, 5, 5, 5, 5, 6, 5, 5, 5, 5, 4, 8, 5, 6,
            /*F*/  4, 4, 4, 4, 4, 4, 4, 5, 4, 4, 4, 4, 3, 7, 4, 5
        };


        //static cycle_count_table s_cmos_cycles;
        //static cycle_count_table s_hc_cycles;


        //typedef void (m6805_base_device::*op_handler_func)();
        public delegate void op_handler_func(m6805_base_device cpu);

        //typedef op_handler_func const op_handler_table[256];
        //typedef u8 const cycle_count_table[256];


        protected class configuration_params
        {
            public op_handler_func [] m_ops = new op_handler_func[256];  //op_handler_table m_ops;
            public u8 [] m_cycles = new u8[256];  //cycle_count_table m_cycles;
            public u32 m_addr_width;
            public u32 m_sp_mask;
            public u32 m_sp_floor;
            public u16 m_vector_mask;
            public u16 m_swi_vector;


            public configuration_params(
                    op_handler_func [] ops,  //op_handler_table &ops,
                    u8 [] cycles,  //cycle_count_table &cycles,
                    u32 addr_width,
                    u32 sp_mask,
                    u32 sp_floor,
                    u16 swi_vector)
            {
                m_ops = ops;
                m_cycles = cycles;
                m_addr_width = addr_width;
                m_sp_mask = sp_mask;
                m_sp_floor = sp_floor;
                m_vector_mask = (u16)((1U << (int)addr_width) - 1);
                m_swi_vector = swi_vector;
            }


            configuration_params(
                    op_handler_func [] ops,  //op_handler_table &ops,
                    u8 [] cycles,  //cycle_count_table &cycles,
                    u32 addr_width,
                    u32 sp_mask,
                    u32 sp_floor,
                    u16 vector_mask,
                    u16 swi_vector)
            {
                m_ops = ops;
                m_cycles = cycles;
                m_addr_width = addr_width;
                m_sp_mask = sp_mask;
                m_sp_floor = sp_floor;
                m_vector_mask = vector_mask;
                m_swi_vector = swi_vector;
            }
        }


        //static op_handler_table s_hmos_s_ops;
        //static op_handler_table s_hmos_b_ops;
        //static op_handler_table s_cmos_b_ops;
        //static op_handler_table s_hc_s_ops;
        //static op_handler_table s_hc_b_ops;
        //static cycle_count_table s_hmos_cycles;
        //static cycle_count_table s_cmos_cycles;
        //static cycle_count_table s_hc_cycles;


        device_memory_interface_m6805_base m_dimemory;
        public device_state_interface_m6805_base m_distate;


        protected configuration_params m_params;
        u32 m_min_cycles;
        u32 m_max_cycles;

        // address spaces
        address_space_config m_program_config;

        // CPU registers
        PAIR m_ea;           // effective address (should really be a temporary in opcode handlers)

        protected PAIR m_pc;           // Program counter
        PAIR m_s;            // Stack pointer
        protected u8 m_a;            // Accumulator
        protected u8 m_x;            // Index register
        protected u8 m_cc;           // Condition codes

        uint16_t m_pending_interrupts; /* MB */

        int [] m_irq_state = new int [9]; /* KW Additional lines for HD63705 */
        int m_nmi_state;

        // other internal states
        protected intref m_icount = new intref();  //int     m_icount;

        // address spaces
        memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache m_cprogram16 = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache();  //memory_access<16, 0, 0, ENDIANNESS_BIG>::cache m_cprogram16;
        memory_access<int_const_13, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache m_cprogram13 = new memory_access<int_const_13, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.cache();  //memory_access<13, 0, 0, ENDIANNESS_BIG>::cache m_cprogram13;
        memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific m_program16 = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific();  //memory_access<16, 0, 0, ENDIANNESS_BIG>::specific m_program16;
        memory_access<int_const_13, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific m_program13 = new memory_access<int_const_13, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific();  //memory_access<13, 0, 0, ENDIANNESS_BIG>::specific m_program13;


        protected m6805_base_device(
                machine_config mconfig,
                string tag,
                device_t owner,
                uint32_t clock,
                device_type type,
                configuration_params params_)
            : base(mconfig, type, tag, owner, clock)
        {
            //m_class_interfaces.Add(new device_execute_interface_m6805_base(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_m6805_base(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_m6805_base(mconfig, this));
            //m_class_interfaces.Add(new device_disasm_interface_m6805_base(mconfig, this));

            m_dimemory = GetClassInterface<device_memory_interface_m6805_base>();
            m_distate = GetClassInterface<device_state_interface_m6805_base>();


            init_s_hmos_s_ops();


            m_params = params_;
            m_program_config = new address_space_config("program", endianness_t.ENDIANNESS_BIG, 8, (u8)params_.m_addr_width);
        }


        protected m6805_base_device(
                machine_config mconfig,
                string tag,
                device_t owner,
                uint32_t clock,
                device_type type,
                configuration_params params_,
                address_map_constructor internal_map)
            : base(mconfig, type, tag, owner, clock)
        {
            //m_class_interfaces.Add(new device_execute_interface_m6805_base(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_m6805_base(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_m6805_base(mconfig, this));
            //m_class_interfaces.Add(new device_disasm_interface_m6805_base(mconfig, this));

            m_dimemory = GetClassInterface<device_memory_interface_m6805_base>();
            m_distate = GetClassInterface<device_state_interface_m6805_base>();


            init_s_hmos_s_ops();


            m_params = params_;
            m_program_config = new address_space_config("program", endianness_t.ENDIANNESS_BIG, 8, (u8)params_.m_addr_width, 0, internal_map);
        }


        // this function is needed when passing in a non-static address_map_constructor in the ctor.  'this' isn't available
        protected void m6805_base_device_after_ctor(address_map_constructor internal_map)
        {
            m_program_config = new address_space_config("program", endianness_t.ENDIANNESS_BIG, 8, (u8)m_params.m_addr_width, 0, internal_map);
        }


        protected uint16_t pending_interrupts { get { return m_pending_interrupts; } set { m_pending_interrupts = value; } }
        protected int [] irq_state { get { return m_irq_state; } }


        // device-level overrides
        protected override void device_start()
        {
            if (m_params.m_addr_width > 13)
            {
                m_dimemory.space(g.AS_PROGRAM).cache(m_cprogram16);
                m_dimemory.space(g.AS_PROGRAM).specific(m_program16);
            }
            else
            {
                m_dimemory.space(g.AS_PROGRAM).cache(m_cprogram13);
                m_dimemory.space(g.AS_PROGRAM).specific(m_program13);
            }

            // get the minimum not including the zero placeholders for illegal instructions
            //m_min_cycles = *std::min_element(
            //        std::begin(m_params.m_cycles),
            //        std::end(m_params.m_cycles),
            //        [] (u8 x, u8 y) { return u8(x - 1) < u8(y - 1); });
            //m_max_cycles = *std::max_element(std::begin(m_params.m_cycles), std::end(m_params.m_cycles));
            u32 result = u32.MaxValue;
            foreach (var c in m_params.m_cycles)
            {
                if (c == 0) continue;
                result = Math.Min(result, c);
            }
            m_min_cycles = result;
            m_max_cycles = m_params.m_cycles.Max();

            // set our instruction counter
            set_icountptr(m_icount);

            // register our state for the debugger
            m_distate.state_add(g.STATE_GENPC,     "GENPC",     m_pc.w.l).noshow();
            m_distate.state_add(g.STATE_GENPCBASE, "CURPC",     m_pc.w.l).noshow();
            m_distate.state_add(g.STATE_GENFLAGS,  "GENFLAGS",  m_cc).callimport().callexport().formatstr("%8s").noshow();
            m_distate.state_add(M6805_A,           "A",         m_a).mask(0xff);
            m_distate.state_add(M6805_PC,          "PC",        m_pc.w.l).mask(0xffff);
            m_distate.state_add(M6805_S,           "S",         m_s.w.l).mask(0xff);
            m_distate.state_add(M6805_X,           "X",         m_x).mask(0xff);
            m_distate.state_add(M6805_CC,          "CC",        m_cc).mask(0xff);

            // register for savestates
            save_item(g.NAME(new { EA }));
            save_item(g.NAME(new { A }));
            save_item(g.NAME(new { PC }));
            save_item(g.NAME(new { S }));
            save_item(g.NAME(new { X }));
            save_item(g.NAME(new { CC }));
            save_item(g.NAME(new { m_pending_interrupts }));
            save_item(g.NAME(new { m_irq_state }));
            save_item(g.NAME(new { m_nmi_state }));

            std.fill(m_irq_state, g.CLEAR_LINE);
        }


        protected override void device_reset()
        {
            m_ea.w.l = 0;
            m_pc.w.l = 0;
            m_s.w.l = (u16)SP_MASK;
            m_a = 0;
            m_x = 0;
            m_cc = 0;
            m_pending_interrupts = 0;

            m_nmi_state = 0;

            /* IRQ disabled */
            SEI();

            if (m_params.m_addr_width > 13)
                rm16(true, (u32)(0xfffe & m_params.m_vector_mask), ref m_pc);
            else
                rm16(false, (u32)(0xfffe & m_params.m_vector_mask), ref m_pc);
        }


        // device_execute_interface overrides
        uint32_t device_execute_interface_execute_min_cycles()
        {
            return m_min_cycles;
        }

        //virtual uint32_t execute_max_cycles() const override;
        //virtual uint32_t execute_input_lines() const override;

        /* execute instructions on this CPU until icount expires */
        void device_execute_interface_execute_run()
        {
            S = SP_ADJUST( S );     /* Taken from CPU_SET_CONTEXT when pointer'afying */

            do
            {
                if (m_pending_interrupts != 0)
                {
                    interrupt();
                }

                debugger_instruction_hook(PC);

                u8 ireg = m_params.m_addr_width > 13 ? (u8)rdop(true, PC++) : (u8)rdop(false, PC++);

                m_params.m_ops[ireg](this);
                m_icount.i -= m_params.m_cycles[ireg];
                burn_cycles(m_params.m_cycles[ireg]);
            }
            while (m_icount.i > 0);
        }

        //virtual void execute_set_input(int inputnum, int state) override;
        //virtual uint64_t execute_clocks_to_cycles(uint64_t clocks) const override;
        //virtual uint64_t execute_cycles_to_clocks(uint64_t cycles) const override;
        //virtual bool execute_input_edge_triggered(int inputnum) const override { return true; }

        // device_memory_interface overrides
        space_config_vector device_memory_interface_memory_space_config()
        {
            return new space_config_vector()
            {
                std.make_pair(g.AS_PROGRAM, m_program_config)
            };
        }

        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;

        // device_state_interface overrides
        //virtual void state_string_export(const device_state_entry &entry, std::string &str) const override;


        // for devices with timing-sensitive peripherals
        protected virtual void burn_cycles(UInt32 count) { }


        void clr_nz()   { m_cc = (u8)(m_cc & ~(NFLAG | ZFLAG)); }
        void clr_nzc()  { m_cc = (u8)(m_cc & ~(NFLAG | ZFLAG | CFLAG)); }
        void clr_hc()   { m_cc = (u8)(m_cc & ~(HFLAG | CFLAG)); }
        void clr_hnzc() { m_cc = (u8)(m_cc & ~(HFLAG | NFLAG | ZFLAG | CFLAG)); }


        // macros for CC -- CC bits affected should be reset before calling
        void set_z8(u8 a)                       { if (a == 0) m_cc |= ZFLAG; }
        void set_n8(u8 a)                       { m_cc |= (u8)((a & 0x80) >> 5); }
        void set_h(u8 a, u8 b, u8 r)            { m_cc |= (u8)((a ^ b ^ r) & 0x10); }
        void set_c8(u16 a)                      { m_cc |= (u8)g.BIT(a, 8); }

        // combos
        void set_nz8(u8 a)                      { set_n8(a); set_z8(a); }
        void set_nzc8(u16 a)                    { set_nz8((u8)a); set_c8(a); }
        void set_hnzc8(u8 a, u8 b, u16 r)       { set_h(a, b, (u8)r); set_nzc8(r); }


        unsigned rdmem(bool big, u32 addr)       { return big ? m_program16.read_byte(addr) : m_program13.read_byte(addr); }  //template <bool big> unsigned    rdmem(u32 addr)             { return big ? m_program16.read_byte(addr) : m_program13.read_byte(addr); }
        void wrmem(bool big, u32 addr, u8 value) { if (big) m_program16.write_byte(addr, value); else m_program13.write_byte(addr, value); }  //template <bool big> void        wrmem(u32 addr, u8 value)   { if(big) m_program16.write_byte(addr, value); else m_program13.write_byte(addr, value); }
        unsigned rdop(bool big, u32 addr)        { return big ? m_cprogram16.read_byte(addr) : m_cprogram13.read_byte(addr); }  //template <bool big> unsigned    rdop(u32 addr)              { return big ? m_cprogram16.read_byte(addr) : m_cprogram13.read_byte(addr); }
        unsigned rdop_arg(bool big, u32 addr)    { return big ? m_cprogram16.read_byte(addr) : m_cprogram13.read_byte(addr); }  //template <bool big> unsigned    rdop_arg(u32 addr)          { return big ? m_cprogram16.read_byte(addr) : m_cprogram13.read_byte(addr); }


        unsigned rm(bool big, u32 addr) { return rdmem(big, addr); }  //template <bool big> unsigned    rm(u32 addr)                { return rdmem<big>(addr); }
        //void rm16(u32 addr, PAIR &p);
        void wm(bool big, u32 addr, u8 value) { wrmem(big, addr, value); }  //template <bool big> void        wm(u32 addr, u8 value)      { wrmem<big>(addr, value); }


        protected virtual void interrupt() { throw new emu_unimplemented(); }
        protected virtual void interrupt_vector() { throw new emu_unimplemented(); }


        protected virtual bool test_il() { return g.CLEAR_LINE != m_irq_state[m6805_global.M6805_IRQ_LINE]; }
    }
}
