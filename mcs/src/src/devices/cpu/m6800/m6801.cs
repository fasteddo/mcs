// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Reflection;

using devcb_read8 = mame.devcb_read<System.Byte, System.Byte, mame.devcb_operators_u8_u8, mame.devcb_operators_u8_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<System.Byte, System.Byte, mame.devcb_operators_u8_u8, mame.devcb_operators_u8_u8>;  //using devcb_write8 = devcb_write<u8>;
using devcb_write_line = mame.devcb_write<int, uint, mame.devcb_operators_s32_u32, mame.devcb_operators_u32_s32, mame.devcb_constant_1<uint, uint, mame.devcb_operators_u32_u32>>;  //using devcb_write_line = devcb_write<int, 1U>;
using offs_t = System.UInt32;  //using offs_t = u32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    public class m6801_cpu_device : m6800_cpu_device
    {
        //DEFINE_DEVICE_TYPE(M6801, m6801_cpu_device, "m6801", "Motorola MC6801")
        static device_t device_creator_m6801_cpu_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new m6801_cpu_device(mconfig, tag, owner, clock); }
        public static readonly device_type M6801 = DEFINE_DEVICE_TYPE(device_creator_m6801_cpu_device, "m6801", "Motorola MC6801");


        class device_execute_interface_m6801 : device_execute_interface_m6800
        {
            public device_execute_interface_m6801(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint64_t execute_clocks_to_cycles(uint64_t clocks) { return (clocks + 4 - 1) / 4; }
            protected override uint64_t execute_cycles_to_clocks(uint64_t cycles) { return cycles * 4; }
            protected override void execute_set_input(int irqline, int state) { ((m6801_cpu_device)device()).device_execute_interface_execute_set_input(irqline, state); }
        }


        protected class device_disasm_interface_m6801 : device_disasm_interface_m6800
        {
            public device_disasm_interface_m6801(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        //enum
        //{
        const int M6801_IRQ_LINE = M6800_IRQ_LINE;
        const int M6801_TIN_LINE = M6800_IRQ_LINE + 1; // P20/Tin Input Capture line (edge sense). Active edge is selectable by internal reg.
        const int M6801_SC1_LINE = M6800_IRQ_LINE + 2;
        //}


        //#define LOG_GENERAL (1U << 0)
        const int LOG_TX      = 1 << 1;
        //#define LOG_TXTICK  (1U << 2)
        //#define LOG_RX      (1U << 3)
        //#define LOG_RXTICK  (1U << 4)
        const int LOG_PORT    = 1 << 5;
        const int LOG_SER     = 1 << 6;
        const int LOG_TIMER   = 1 << 7;

        //#define VERBOSE (LOG_SER)
        //#define LOG_OUTPUT_STREAM std::cout
        //#define LOG_OUTPUT_STREAM std::cerr
        //#include "logmacro.h"

        void LOGTX(string format, params object [] args) { LOGMASKED(LOG_TX, format, args); }
        //#define LOGTXTICK(...)  LOGMASKED(LOG_TXTICK, __VA_ARGS__)
        //#define LOGRX(...)      LOGMASKED(LOG_RX, __VA_ARGS__)
        //#define LOGRXTICK(...)  LOGMASKED(LOG_RXTICK, __VA_ARGS__)
        public void LOGPORT(string format, params object [] args) { LOGMASKED(LOG_PORT, format, args); }
        public void LOGSER(string format, params object [] args) { LOGMASKED(LOG_SER, format, args); }  //#define LOGSER(...)     LOGMASKED(LOG_SER, __VA_ARGS__)
        public void LOGTIMER(string format, params object [] args) { LOGMASKED(LOG_TIMER, format, args); }  //#define LOGTIMER(...)   LOGMASKED(LOG_TIMER, __VA_ARGS__)


        uint16_t CT { get { return m_counter.w.l; } set { m_counter.w.l = value; } }
        uint16_t CTH { get { return m_counter.w.h; } set { m_counter.w.h = value; } }
        uint32_t CTD { get { return m_counter.d; } set { m_counter.d = value; } }
        uint16_t OC { get { return m_output_compare.w.l; } }
        uint16_t OCH { get { return m_output_compare.w.h; } set { m_output_compare.w.h = value; } }
        uint32_t OCD { get { return m_output_compare.d; } set { m_output_compare.d = value; } }
        //#define OC2     m_output_compare2.w.l
        //#define OC2H    m_output_compare2.w.h
        //#define OC2D    m_output_compare2.d
        uint16_t TOH { get { return m_timer_over.w.l; } set { m_timer_over.w.l = value; } }
        uint32_t TOD { get { return m_timer_over.d; } set { m_timer_over.d = value; } }


        // serial I/O

        const uint8_t M6801_RMCR_SS_MASK      = 0x03; // Speed Select
        //#define M6801_RMCR_SS_4096      0x03 // E / 4096
        //#define M6801_RMCR_SS_1024      0x02 // E / 1024
        //#define M6801_RMCR_SS_128       0x01 // E / 128
        //#define M6801_RMCR_SS_16        0x00 // E / 16
        const uint8_t M6801_RMCR_CC_MASK      = 0x0c; // Clock Control/Format Select

        const uint8_t M6801_TRCSR_RDRF        = 0x80; // Receive Data Register Full
        const uint8_t M6801_TRCSR_ORFE        = 0x40; // Over Run Framing Error
        const uint8_t M6801_TRCSR_TDRE        = 0x20; // Transmit Data Register Empty
        const uint8_t M6801_TRCSR_RIE         = 0x10; // Receive Interrupt Enable
        const uint8_t M6801_TRCSR_RE          = 0x08; // Receive Enable
        const uint8_t M6801_TRCSR_TIE         = 0x04; // Transmit Interrupt Enable
        const uint8_t M6801_TRCSR_TE          = 0x02; // Transmit Enable
        //#define M6801_TRCSR_WU          0x01 // Wake Up

        //#define M6801_PORT2_IO4         0x10
        //#define M6801_PORT2_IO3         0x08

        public const uint8_t M6801_P3CSR_LE          = 0x08;
        const uint8_t M6801_P3CSR_OSS                = 0x10;
        //#define M6801_P3CSR_IS3_ENABLE  0x40
        public const uint8_t M6801_P3CSR_IS3_FLAG    = 0x80;

        static readonly int [] M6801_RMCR_SS = new int [4] { 16, 128, 1024, 4096 };

        //#define M6801_SERIAL_START      0
        //#define M6801_SERIAL_STOP       9

        //enum
        //{
        const int M6801_TX_STATE_INIT  = 0;
        const int M6801_TX_STATE_READY = 1;
        //}


        /* take interrupt */
        void TAKE_ICI() { enter_interrupt("take ICI\n", 0xfff6); }
        void TAKE_OCI() { enter_interrupt("take OCI\n", 0xfff4); }
        void TAKE_TOI() { enter_interrupt("take TOI\n", 0xfff2); }
        void TAKE_SCI() { enter_interrupt("take SCI\n", 0xfff0); }

        /* mnemonicos for the Timer Control and Status Register bits */
        const uint8_t TCSR_OLVL = 0x01;
        const uint8_t TCSR_IEDG = 0x02;
        const uint8_t TCSR_ETOI = 0x04;
        const uint8_t TCSR_EOCI = 0x08;
        const uint8_t TCSR_EICI = 0x10;
        const uint8_t TCSR_TOF  = 0x20;
        const uint8_t TCSR_OCF  = 0x40;
        const uint8_t TCSR_ICF  = 0x80;

        //#define TCSR2_OE1   0x01
        //#define TCSR2_OE2   0x02
        //#define TCSR2_OLVL2 0x04
        //#define TCSR2_EOCI2 0x08
        //#define TCSR2_OCF2  0x20


        /* Note: don't use 0 cycles here for invalid opcodes so that we don't */
        /* hang in an infinite loop if we hit one */
        const uint8_t XX = 5;  // invalid opcode unknown cc
        protected static readonly uint8_t [] cycles_6803 = new uint8_t [256]
        {
                /* 0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F */
            /*0*/ XX, 2,XX,XX, 3, 3, 2, 2, 3, 3, 2, 2, 2, 2, 2, 2,
            /*1*/  2, 2,XX,XX,XX,XX, 2, 2,XX, 2,XX, 2,XX,XX,XX,XX,
            /*2*/  3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3,
            /*3*/  3, 3, 4, 4, 3, 3, 3, 3, 5, 5, 3,10, 4,10, 9,12,
            /*4*/  2,XX,XX, 2, 2,XX, 2, 2, 2, 2, 2,XX, 2, 2,XX, 2,
            /*5*/  2,XX,XX, 2, 2,XX, 2, 2, 2, 2, 2,XX, 2, 2,XX, 2,
            /*6*/  6,XX,XX, 6, 6,XX, 6, 6, 6, 6, 6,XX, 6, 6, 3, 6,
            /*7*/  6,XX,XX, 6, 6,XX, 6, 6, 6, 6, 6,XX, 6, 6, 3, 6,
            /*8*/  2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 2, 4, 6, 3, 3,
            /*9*/  3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 3, 5, 5, 4, 4,
            /*A*/  4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 6, 6, 5, 5,
            /*B*/  4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 6, 6, 5, 5,
            /*C*/  2, 2, 2, 4, 2, 2, 2, 2, 2, 2, 2, 2, 3,XX, 3, 3,
            /*D*/  3, 3, 3, 5, 3, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4,
            /*E*/  4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5,
            /*F*/  4, 4, 4, 6, 4, 4, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5
        };


        devcb_read8.array<uint32_constant_4> m_in_port_func;
        devcb_write8.array<uint32_constant_4> m_out_port_func;

        devcb_write_line m_out_sc2_func;
        devcb_write_line m_out_sertx_func;

        /* internal registers */
        uint8_t [] m_port_ddr = new uint8_t [4];
        uint8_t [] m_port_data = new uint8_t [4];
        uint8_t m_p3csr;          // Port 3 Control/Status Register
        uint8_t m_tcsr;           /* Timer Control and Status Register */
        uint8_t m_pending_tcsr;   /* pending IRQ flag for clear IRQflag process */
        uint8_t m_irq2;           /* IRQ2 flags */
        uint8_t m_ram_ctrl;
        PAIR m_counter;        /* free running counter */
        PAIR m_output_compare; /* output compare       */
        uint16_t m_input_capture;  /* input capture        */
        int m_p3csr_is3_flag_read;
        int m_port3_latched;

        uint8_t m_trcsr;
        uint8_t m_rmcr;
        uint8_t m_rdr;
        uint8_t m_tdr;
        uint8_t m_rsr;
        uint8_t m_tsr;
        int m_rxbits;
        int m_txbits;
        int m_txstate;
        int m_trcsr_read_tdre;
        int m_trcsr_read_orfe;
        int m_trcsr_read_rdrf;
        int m_tx;
        int m_ext_serclock;
        bool m_use_ext_serclock;
        bool m_port2_written;

        int m_latch09;

        PAIR m_timer_over;
        emu_timer m_sci_timer;

        /* point of next timer event */
        uint32_t m_timer_next;

        int m_sc1_state;

        //static const uint8_t cycles_63701[256];

        protected static op_func [] m6803_insn = new op_func [0x100];

        void init_m6803_insn()
        {
            string [] insn = new string [0x100]
            {
                "illegl1", "nop",     "illegl1", "illegl1", "lsrd",    "asld",    "tap",     "tpa",
                "inx",     "dex",     "clv",     "sev",     "clc",     "sec",     "cli",     "sei",
                "sba",     "cba",     "illegl1", "illegl1", "illegl1", "illegl1", "tab",     "tba",
                "illegl1", "daa",     "illegl1", "aba",     "illegl1", "illegl1", "illegl1", "illegl1",
                "bra",     "brn",     "bhi",     "bls",     "bcc",     "bcs",     "bne",     "beq",
                "bvc",     "bvs",     "bpl",     "bmi",     "bge",     "blt",     "bgt",     "ble",
                "tsx",     "ins",     "pula",    "pulb",    "des",     "txs",     "psha",    "pshb",
                "pulx",    "rts",     "abx",     "rti",     "pshx",    "mul",     "wai",     "swi",
                "nega",    "illegl1", "illegl1", "coma",    "lsra",    "illegl1", "rora",    "asra",
                "asla",    "rola",    "deca",    "illegl1", "inca",    "tsta",    "illegl1", "clra",
                "negb",    "illegl1", "illegl1", "comb",    "lsrb",    "illegl1", "rorb",    "asrb",
                "aslb",    "rolb",    "decb",    "illegl1", "incb",    "tstb",    "illegl1", "clrb",
                "neg_ix",  "illegl1", "illegl1", "com_ix",  "lsr_ix",  "illegl1", "ror_ix",  "asr_ix",
                "asl_ix",  "rol_ix",  "dec_ix",  "illegl1", "inc_ix",  "tst_ix",  "jmp_ix",  "clr_ix",
                "neg_ex",  "illegl1", "illegl1", "com_ex",  "lsr_ex",  "illegl1", "ror_ex",  "asr_ex",
                "asl_ex",  "rol_ex",  "dec_ex",  "illegl1", "inc_ex",  "tst_ex",  "jmp_ex",  "clr_ex",
                "suba_im", "cmpa_im", "sbca_im", "subd_im", "anda_im", "bita_im", "lda_im",  "sta_im",
                "eora_im", "adca_im", "ora_im",  "adda_im", "cpx_im",  "bsr",     "lds_im",  "sts_im",
                "suba_di", "cmpa_di", "sbca_di", "subd_di", "anda_di", "bita_di", "lda_di",  "sta_di",
                "eora_di", "adca_di", "ora_di",  "adda_di", "cpx_di",  "jsr_di",  "lds_di",  "sts_di",
                "suba_ix", "cmpa_ix", "sbca_ix", "subd_ix", "anda_ix", "bita_ix", "lda_ix",  "sta_ix",
                "eora_ix", "adca_ix", "ora_ix",  "adda_ix", "cpx_ix",  "jsr_ix",  "lds_ix",  "sts_ix",
                "suba_ex", "cmpa_ex", "sbca_ex", "subd_ex", "anda_ex", "bita_ex", "lda_ex",  "sta_ex",
                "eora_ex", "adca_ex", "ora_ex",  "adda_ex", "cpx_ex",  "jsr_ex",  "lds_ex",  "sts_ex",
                "subb_im", "cmpb_im", "sbcb_im", "addd_im", "andb_im", "bitb_im", "ldb_im",  "stb_im",
                "eorb_im", "adcb_im", "orb_im",  "addb_im", "ldd_im",  "std_im",  "ldx_im",  "stx_im",
                "subb_di", "cmpb_di", "sbcb_di", "addd_di", "andb_di", "bitb_di", "ldb_di",  "stb_di",
                "eorb_di", "adcb_di", "orb_di",  "addb_di", "ldd_di",  "std_di",  "ldx_di",  "stx_di",
                "subb_ix", "cmpb_ix", "sbcb_ix", "addd_ix", "andb_ix", "bitb_ix", "ldb_ix",  "stb_ix",
                "eorb_ix", "adcb_ix", "orb_ix",  "addb_ix", "ldd_ix",  "std_ix",  "ldx_ix",  "stx_ix",
                "subb_ex", "cmpb_ex", "sbcb_ex", "addd_ex", "andb_ex", "bitb_ex", "ldb_ex",  "stb_ex",
                "eorb_ex", "adcb_ex", "orb_ex",  "addb_ex", "ldd_ex",  "std_ex",  "ldx_ex",  "stx_ex"
            };


            // https://www.red-gate.com/simple-talk/blogs/introduction-to-open-instance-delegates/
            for (int i = 0; i < insn.Length; i++)
            {
                MethodInfo methodInfo = typeof(m6801_cpu_device).GetMethod(insn[i], BindingFlags.NonPublic | BindingFlags.Instance);
                m6803_insn[i] = (op_func)methodInfo.CreateDelegate(typeof(op_func), null);
            }
        }

        //static const op_func hd63701_insn[256];


        protected m6801_cpu_device(machine_config mconfig, string tag, device_t owner, uint32_t clock) 
            : this(mconfig, M6801, tag, owner, clock, m6803_insn, cycles_6803, null)
        {
            m_class_interfaces.Add(new device_disasm_interface_m6801(mconfig, this));
        }

        protected m6801_cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, op_func [] insn, uint8_t [] cycles, address_map_constructor internal_ = null) 
            : base(mconfig, type, tag, owner, clock, insn, cycles, internal_)
        {
            m_class_interfaces.Add(new device_execute_interface_m6801(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_m6800(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_m6800(mconfig, this));

            init_m6803_insn();

            m_in_port_func = new devcb_read8.array<uint32_constant_4>(this, () => { return new devcb_read8(this); });
            m_out_port_func = new devcb_write8.array<uint32_constant_4>(this, () => { return new devcb_write8(this); });
            m_out_sc2_func = new devcb_write_line(this);
            m_out_sertx_func = new devcb_write_line(this);
        }


        public devcb_read8.binder in_p1_cb() { return m_in_port_func[0].bind(); }
        public devcb_write8.binder out_p1_cb() { return m_out_port_func[0].bind(); }
        public devcb_read8.binder in_p2_cb() { return m_in_port_func[1].bind(); }
        public devcb_write8.binder out_p2_cb() { return m_out_port_func[1].bind(); }
        //auto in_p3_cb() { return m_in_port_func[2].bind(); }
        //auto out_p3_cb() { return m_out_port_func[2].bind(); }
        //auto in_p4_cb() { return m_in_port_func[3].bind(); }
        //auto out_p4_cb() { return m_out_port_func[3].bind(); }

        //auto out_sc2_cb() { return m_out_sc2_func.bind(); }
        //auto out_ser_tx_cb() { return m_out_sertx_func.bind(); }


        static void m6801_io(address_map map, m6801_cpu_device m6801) // FIXME: privatize this
        {
            map.op(0x0000, 0x0000).rw(m6801.ff_r, m6801.p1_ddr_w);
            map.op(0x0001, 0x0001).rw(m6801.ff_r, m6801.p2_ddr_w);
            map.op(0x0002, 0x0002).rw(m6801.p1_data_r, m6801.p1_data_w);
            map.op(0x0003, 0x0003).rw(m6801.p2_data_r, m6801.p2_data_w);
            map.op(0x0004, 0x0004).rw(m6801.ff_r, m6801.p3_ddr_w); // TODO: external in 6801 modes 0�3 & 6
            map.op(0x0005, 0x0005).rw(m6801.ff_r, m6801.p4_ddr_w); // TODO: external in 6801 modes 0�3
            map.op(0x0006, 0x0006).rw(m6801.p3_data_r, m6801.p3_data_w); // TODO: external in 6801 modes 0�3 & 6
            map.op(0x0007, 0x0007).rw(m6801.p4_data_r, m6801.p4_data_w); // TODO: external in 6801 modes 0�3
            map.op(0x0008, 0x0008).rw(m6801.tcsr_r, m6801.tcsr_w);
            map.op(0x0009, 0x0009).rw(m6801.ch_r, m6801.ch_w);
            map.op(0x000a, 0x000a).rw(m6801.cl_r, m6801.cl_w);
            map.op(0x000b, 0x000b).rw(m6801.ocrh_r, m6801.ocrh_w);
            map.op(0x000c, 0x000c).rw(m6801.ocrl_r, m6801.ocrl_w);
            map.op(0x000d, 0x000d).r(m6801.icrh_r);
            map.op(0x000e, 0x000e).r(m6801.icrl_r);
            map.op(0x000f, 0x000f).rw(m6801.p3_csr_r, m6801.p3_csr_w); // TODO: external in 6801 modes 0�3, 5 & 6
            map.op(0x0010, 0x0010).rw(m6801.sci_rmcr_r, m6801.sci_rmcr_w);
            map.op(0x0011, 0x0011).rw(m6801.sci_trcsr_r, m6801.sci_trcsr_w);
            map.op(0x0012, 0x0012).r(m6801.sci_rdr_r);
            map.op(0x0013, 0x0013).w(m6801.sci_tdr_w);
            map.op(0x0014, 0x0014).rw(m6801.rcr_r, m6801.rcr_w);
        }


        //void m6801_clock_serial();


        // device-level overrides
        protected override void device_resolve_objects()
        {
            base.device_resolve_objects();

            m_in_port_func.resolve_all_safe(0xff);
            m_out_port_func.resolve_all_safe();
            m_out_sc2_func.resolve_safe();
            m_out_sertx_func.resolve_safe();
        }


        protected override void device_start()
        {
            base.device_start();

            m_sci_timer = machine().scheduler().timer_alloc(sci_tick);

            m_port_ddr[3] = 0;
            m_port_data[3] = 0;
            m_input_capture = 0;
            m_rdr = 0;
            m_tdr = 0;
            m_rmcr = 0;
            m_ram_ctrl = 0;

            save_item(NAME(new { m_port_ddr }));
            save_item(NAME(new { m_port_data }));
            save_item(NAME(new { m_p3csr }));
            save_item(NAME(new { m_tcsr }));
            save_item(NAME(new { m_pending_tcsr }));
            save_item(NAME(new { m_irq2 }));
            save_item(NAME(new { m_ram_ctrl }));

            save_item(NAME(new { m_counter.d }));
            save_item(NAME(new { m_output_compare.d }));
            save_item(NAME(new { m_input_capture }));
            save_item(NAME(new { m_p3csr_is3_flag_read }));
            save_item(NAME(new { m_port3_latched }));
            save_item(NAME(new { m_port2_written }));

            save_item(NAME(new { m_trcsr }));
            save_item(NAME(new { m_rmcr }));
            save_item(NAME(new { m_rdr }));
            save_item(NAME(new { m_tdr }));
            save_item(NAME(new { m_rsr }));
            save_item(NAME(new { m_tsr }));
            save_item(NAME(new { m_rxbits }));
            save_item(NAME(new { m_txbits }));
            save_item(NAME(new { m_txstate }));
            save_item(NAME(new { m_trcsr_read_tdre }));
            save_item(NAME(new { m_trcsr_read_orfe }));
            save_item(NAME(new { m_trcsr_read_rdrf }));
            save_item(NAME(new { m_tx }));
            save_item(NAME(new { m_ext_serclock }));
            save_item(NAME(new { m_use_ext_serclock }));

            save_item(NAME(new { m_latch09 }));

            save_item(NAME(new { m_timer_over.d }));

            save_item(NAME(new { m_timer_next }));

            save_item(NAME(new { m_sc1_state }));
        }


        protected override void device_reset()
        {
            base.device_reset();

            irq_state[M6801_TIN_LINE] = 0;
            m_sc1_state = 0;

            m_port_ddr[0] = 0x00;
            m_port_ddr[1] = 0x00;
            m_port_ddr[2] = 0x00;
            m_port_data[0] = 0;
            m_p3csr = 0x00;
            m_p3csr_is3_flag_read = 0;
            m_port2_written = false;
            m_port3_latched = 0;
            /* TODO: on reset port 2 should be read to determine the operating mode (bits 0-2) */
            m_tcsr = 0x00;
            m_pending_tcsr = 0x00;
            m_irq2 = 0;
            CTD = 0x0000;
            OCD = 0xffff;
            TOD = 0xffff;
            m_timer_next = 0xffff;
            m_ram_ctrl |= 0x40;
            m_latch09 = 0;

            m_trcsr = M6801_TRCSR_TDRE;

            m_txstate = M6801_TX_STATE_INIT;
            m_txbits = m_rxbits = 0;
            m_tx = 1;
            m_trcsr_read_tdre = 0;
            m_trcsr_read_orfe = 0;
            m_trcsr_read_rdrf = 0;
            m_ext_serclock = 0;
            m_use_ext_serclock = false;

            set_rmcr(0);
        }


        // device_execute_interface overrides
        //virtual uint64_t execute_clocks_to_cycles(uint64_t clocks) const override { return (clocks + 4 - 1) / 4; }
        //virtual uint64_t execute_cycles_to_clocks(uint64_t cycles) const override { return (cycles * 4); }

        new void device_execute_interface_execute_set_input(int irqline, int state)
        {
            switch (irqline)
            {
            case M6801_SC1_LINE:
                if (m_sc1_state == 0 && (g.CLEAR_LINE != state))
                {
                    if (m_port3_latched == 0 && (m_p3csr & M6801_P3CSR_LE) != 0)
                    {
                        // latch input data to port 3
                        m_port_data[2] = (uint8_t)((m_in_port_func[2].op() & (m_port_ddr[2] ^ 0xff)) | (m_port_data[2] & m_port_ddr[2]));
                        m_port3_latched = 1;
                        LOGPORT("Latched Port 3 Data: {0}\n", m_port_data[2]);

                        // set IS3 flag bit
                        m_p3csr |= M6801_P3CSR_IS3_FLAG;
                    }
                    else
                    {
                        LOGPORT("Not latching Port 3 Data:{0}{1}", m_port3_latched != 0 ? " already latched" : "", (m_p3csr & M6801_P3CSR_LE) != 0 ? "" : " LE clear");
                    }
                }

                m_sc1_state = g.ASSERT_LINE == state ? 1 : 0;
                if (g.CLEAR_LINE != state)
                    standard_irq_callback(M6801_SC1_LINE); // re-entrant - do it after setting m_sc1_state

                break;

            case M6801_TIN_LINE:
                irq_state[M6801_TIN_LINE] = (uint8_t)state;

                if (state != irq_state[M6801_TIN_LINE])
                {
                    //edge = (state == CLEAR_LINE ) ? 2 : 0;
                    if (((m_tcsr & TCSR_IEDG) ^ (state == g.CLEAR_LINE ? TCSR_IEDG : 0)) == 0)
                        return;

                    /* active edge in */
                    m_tcsr |= TCSR_ICF;
                    m_pending_tcsr |= TCSR_ICF;
                    m_input_capture = CT;
                    modified_tcsr();
                }
                break;

            default:
                base.device_execute_interface_execute_set_input(irqline, state);  //execute_set_input(irqline, state);
                break;
            }
        }


        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        void p1_ddr_w(uint8_t data)
        {
            LOGPORT("Port 1 Data Direction Register: {0}\n", data);

            if (m_port_ddr[0] != data)
            {
                m_port_ddr[0] = data;
                m_out_port_func[0].op(0, (uint8_t)((m_port_data[0] & m_port_ddr[0]) | (m_port_ddr[0] ^ 0xff)), m_port_ddr[0]);
            }
        }


        uint8_t p1_data_r()
        {
            if (m_port_ddr[0] == 0xff)
                return m_port_data[0];
            else
                return (uint8_t)((m_in_port_func[0].op() & (m_port_ddr[0] ^ 0xff)) | (m_port_data[0] & m_port_ddr[0]));
        }


        void p1_data_w(uint8_t data)
        {
            LOGPORT("Port 1 Data Register: {0}\n", data);

            m_port_data[0] = data;
            m_out_port_func[0].op(0, (uint8_t)((m_port_data[0] & m_port_ddr[0]) | (m_port_ddr[0] ^ 0xff)), m_port_ddr[0]);
        }


        void p2_ddr_w(uint8_t data)
        {
            LOGPORT("Port 2 Data Direction Register: {0}\n", data);

            if (m_port_ddr[1] != data)
            {
                m_port_ddr[1] = data;
                write_port2();
            }
        }


        uint8_t p2_data_r()
        {
            if(m_port_ddr[1] == 0xff)
                return m_port_data[1];
            else
                return (uint8_t)((m_in_port_func[1].op() & (m_port_ddr[1] ^ 0xff)) | (m_port_data[1] & m_port_ddr[1]));
        }


        void p2_data_w(uint8_t data)
        {
            LOGPORT("Port 2 Data Register: {0}\n", data);

            m_port_data[1] = data;
            m_port2_written = true;
            write_port2();
        }


        void p3_ddr_w(uint8_t data)
        {
            LOGPORT("Port 3 Data Direction Register: {0}\n", data);

            if (m_port_ddr[2] != data)
            {
                m_port_ddr[2] = data;
                m_out_port_func[2].op(0, (uint8_t)((m_port_data[2] & m_port_ddr[2]) | (m_port_ddr[2] ^ 0xff)), m_port_ddr[2]);
            }
        }


        uint8_t p3_data_r()
        {
            uint8_t data;

            if (!machine().side_effects_disabled())
            {
                if (m_p3csr_is3_flag_read != 0)
                {
                    LOGPORT("Cleared IS3\n");
                    m_p3csr = (uint8_t)(m_p3csr & ~M6801_P3CSR_IS3_FLAG);
                    m_p3csr_is3_flag_read = 0;
                }

                if ((m_p3csr & M6801_P3CSR_OSS) == 0)
                {
                    set_os3(g.ASSERT_LINE);
                }
            }

            if (((m_p3csr & M6801_P3CSR_LE) != 0) || (m_port_ddr[2] == 0xff))
                data = m_port_data[2];
            else
                data = (uint8_t)((m_in_port_func[2].op() & (m_port_ddr[2] ^ 0xff))
                    | (m_port_data[2] & m_port_ddr[2]));

            if (!machine().side_effects_disabled())
            {
                m_port3_latched = 0;

                if ((m_p3csr & M6801_P3CSR_OSS) == 0)
                {
                    set_os3(g.CLEAR_LINE);
                }
            }

            return data;
        }


        void p3_data_w(uint8_t data)
        {
            LOGPORT("Port 3 Data Register: {0}\n", data);

            if (m_p3csr_is3_flag_read != 0)
            {
                LOGPORT("Cleared IS3\n");
                m_p3csr = (uint8_t)(m_p3csr & ~M6801_P3CSR_IS3_FLAG);
                m_p3csr_is3_flag_read = 0;
            }

            if ((m_p3csr & M6801_P3CSR_OSS) != 0)
            {
                set_os3(g.ASSERT_LINE);
            }

            m_port_data[2] = data;
            m_out_port_func[2].op(0, (uint8_t)((m_port_data[2] & m_port_ddr[2]) | (m_port_ddr[2] ^ 0xff)), m_port_ddr[2]);

            if ((m_p3csr & M6801_P3CSR_OSS) != 0)
            {
                set_os3(g.CLEAR_LINE);
            }
        }


        uint8_t p3_csr_r()
        {
            if (((m_p3csr & M6801_P3CSR_IS3_FLAG) != 0) && !machine().side_effects_disabled())
            {
                m_p3csr_is3_flag_read = 1;
            }

            return m_p3csr;
        }


        void p3_csr_w(uint8_t data)
        {
            LOGPORT("Port 3 Control and Status Register: {0}\n", data);

            m_p3csr = data;
        }


        void p4_ddr_w(uint8_t data)
        {
            LOGPORT("Port 4 Data Direction Register: {0}\n", data);

            if (m_port_ddr[3] != data)
            {
                m_port_ddr[3] = data;
                m_out_port_func[3].op(0, (uint8_t)((m_port_data[3] & m_port_ddr[3]) | (m_port_ddr[3] ^ 0xff)), m_port_ddr[3]);
            }
        }


        uint8_t p4_data_r()
        {
            if(m_port_ddr[3] == 0xff)
                return m_port_data[3];
            else
                return (uint8_t)((m_in_port_func[3].op() & (m_port_ddr[3] ^ 0xff)) | (m_port_data[3] & m_port_ddr[3]));
        }


        void p4_data_w(uint8_t data)
        {
            LOGPORT("Port 4 Data Register: {0}\n", data);

            m_port_data[3] = data;
            m_out_port_func[3].op(0, (uint8_t)((m_port_data[3] & m_port_ddr[3]) | (m_port_ddr[3] ^ 0xff)), m_port_ddr[3]);
        }


        uint8_t tcsr_r()
        {
            if (!machine().side_effects_disabled())
                m_pending_tcsr = 0;

            return m_tcsr;
        }


        void tcsr_w(uint8_t data)
        {
            LOGTIMER("Timer Control and Status Register: {0}\n", data);

            m_tcsr = data;
            m_pending_tcsr &= m_tcsr;
            modified_tcsr();
            if ((cc & 0x10) == 0)
                m6800_check_irq2();
        }


        uint8_t ch_r()
        {
            if (((m_pending_tcsr & TCSR_TOF) == 0) && !machine().side_effects_disabled())
            {
                m_tcsr = (uint8_t)(m_tcsr & ~TCSR_TOF);
                modified_tcsr();
            }

            return m_counter.b.h;
        }


        uint8_t cl_r()
        {
            uint8_t data = m_counter.b.l;

            // HACK there should be a break here, but Coleco Adam won't boot with it present, proper fix required to the free-running counter
            //(void)data;

            return ocrh_r();
        }


        void ch_w(uint8_t data)
        {
            LOGTIMER("Counter High Register: {0}\n", data);

            m_latch09 = data & 0xff;    /* 6301 only */
            CT  = 0xfff8;
            TOH = CTH;
            modified_counters();
        }


        void cl_w(uint8_t data)
        {
            LOGTIMER("Counter Low Register: {0}\n", data);

            CT = (uint16_t)((m_latch09 << 8) | (data & 0xff));
            TOH = CTH;
            modified_counters();
        }


        uint8_t ocrh_r()
        {
            if (((m_pending_tcsr & TCSR_OCF) == 0) && !machine().side_effects_disabled())
            {
                m_tcsr = (uint8_t)(m_tcsr & ~TCSR_OCF);
                modified_tcsr();
            }

            return m_output_compare.b.h;
        }


        uint8_t ocrl_r()
        {
            if (((m_pending_tcsr & TCSR_OCF) == 0) && !machine().side_effects_disabled())
            {
                m_tcsr = (uint8_t)(m_tcsr & ~TCSR_OCF);
                modified_tcsr();
            }

            return m_output_compare.b.l;
        }


        void ocrh_w(uint8_t data)
        {
            LOGTIMER("Output Compare High Register: {0}\n", data);

            if( m_output_compare.b.h != data)
            {
                m_output_compare.b.h = data;
                modified_counters();
            }
        }


        void ocrl_w(uint8_t data)
        {
            LOGTIMER("Output Compare Low Register: {0}\n", data);

            if( m_output_compare.b.l != data)
            {
                m_output_compare.b.l = data;
                modified_counters();
            }
        }


        uint8_t icrh_r()
        {
            if (((m_pending_tcsr & TCSR_ICF) == 0) && !machine().side_effects_disabled())
            {
                m_tcsr = (uint8_t)(m_tcsr & ~TCSR_ICF);
                modified_tcsr();
            }
            return (uint8_t)((m_input_capture >> 0) & 0xff);
        }


        uint8_t icrl_r()
        {
            return (uint8_t)((m_input_capture >> 8) & 0xff);
        }


        uint8_t sci_rmcr_r()
        {
            return m_rmcr;
        }


        void sci_rmcr_w(uint8_t data)
        {
            LOGSER("SCI Rate and Mode Control Register: {0}\n", data);

            set_rmcr(data);
        }


        uint8_t sci_trcsr_r()
        {
            if (!machine().side_effects_disabled())
            {
                if ((m_trcsr & M6801_TRCSR_TDRE) != 0)
                {
                    m_trcsr_read_tdre = 1;
                }

                if ((m_trcsr & M6801_TRCSR_ORFE) != 0)
                {
                    m_trcsr_read_orfe = 1;
                }

                if ((m_trcsr & M6801_TRCSR_RDRF) != 0)
                {
                    m_trcsr_read_rdrf = 1;
                }
            }

            return m_trcsr;
        }


        void sci_trcsr_w(uint8_t data)
        {
            LOGSER("SCI Transmit/Receive Control and Status Register: {0}\n", data);

            if (((data & M6801_TRCSR_TE) != 0) && ((m_trcsr & M6801_TRCSR_TE) == 0))
            {
                m_txstate = M6801_TX_STATE_INIT;
                m_txbits = 0;
                m_tx = 1;
            }

            if (((data & M6801_TRCSR_RE) != 0) && ((m_trcsr & M6801_TRCSR_RE) == 0))
            {
                m_rxbits = 0;
            }

            m_trcsr = (uint8_t)((m_trcsr & 0xe0) | (data & 0x1f));
        }


        uint8_t sci_rdr_r()
        {
            if (!machine().side_effects_disabled())
            {
                if (m_trcsr_read_orfe != 0)
                {
                    LOGSER("Cleared ORFE\n");
                    m_trcsr_read_orfe = 0;
                    m_trcsr = (uint8_t)(m_trcsr & ~M6801_TRCSR_ORFE);
                }

                if (m_trcsr_read_rdrf != 0)
                {
                    LOGSER("Cleared RDRF\n");
                    m_trcsr_read_rdrf = 0;
                    m_trcsr = (uint8_t)(m_trcsr & ~M6801_TRCSR_RDRF);
                }
            }

            return m_rdr;
        }


        void sci_tdr_w(uint8_t data)
        {
            LOGSER("SCI Transmit Data Register: ${0}/{1}\n", data, data);

            if (m_trcsr_read_tdre != 0)
            {
                m_trcsr_read_tdre = 0;
                m_trcsr = (uint8_t)(m_trcsr & ~M6801_TRCSR_TDRE);
            }
            m_tdr = data;
        }


        uint8_t rcr_r()
        {
            return m_ram_ctrl;
        }


        void rcr_w(uint8_t data)
        {
            LOG("RAM Control Register: {0}\n", data);

            m_ram_ctrl = data;
        }


        uint8_t ff_r()
        {
            if (!machine().side_effects_disabled())
                logerror("PC {0}: warning - read from write-only internal register\n", state().pc());

            return 0xff;
        }


        protected static void m6803_mem(address_map map, device_t owner)
        {
            m6801_cpu_device m6801 = (m6801_cpu_device)owner;

            m6801_io(map, m6801);
            map.op(0x0080, 0x00ff).ram();        /* 6803 internal RAM */
        }


        protected override void m6800_check_irq2()
        {
            if ((m_tcsr & (TCSR_EICI | TCSR_ICF)) == (TCSR_EICI | TCSR_ICF))
            {
                TAKE_ICI();
                standard_irq_callback(M6801_TIN_LINE);
            }
            else if ((m_tcsr & (TCSR_EOCI | TCSR_OCF)) == (TCSR_EOCI | TCSR_OCF))
            {
                TAKE_OCI();
            }
            else if ((m_tcsr & (TCSR_ETOI | TCSR_TOF)) == (TCSR_ETOI | TCSR_TOF))
            {
                TAKE_TOI();
            }
            else if (((m_trcsr & (M6801_TRCSR_RIE | M6801_TRCSR_RDRF)) == (M6801_TRCSR_RIE | M6801_TRCSR_RDRF)) ||
                     ((m_trcsr & (M6801_TRCSR_RIE | M6801_TRCSR_ORFE)) == (M6801_TRCSR_RIE | M6801_TRCSR_ORFE)) ||
                     ((m_trcsr & (M6801_TRCSR_TIE | M6801_TRCSR_TDRE)) == (M6801_TRCSR_TIE | M6801_TRCSR_TDRE)))
            {
                LOG("SCI interrupt\n");
                TAKE_SCI();
            }
        }

        public override void increment_counter(int amount)
        {
            base.increment_counter(amount);
            CTD = (uint32_t)(CTD + amount);
            if (CTD >= m_timer_next)
                check_timer_event();
        }


        public override void EAT_CYCLES() { throw new emu_unimplemented(); }


        /* cleanup high-word of counters */
        public override void CLEANUP_COUNTERS()
        {
            OCH -= CTH;
            TOH -= CTH;
            CTH = 0;
            set_timer_event();
            if (CTD >= m_timer_next)
                check_timer_event();
        }


        protected virtual void modified_tcsr()
        {
            m_irq2 = (uint8_t)((m_tcsr & (m_tcsr << 3)) & (TCSR_ICF | TCSR_OCF | TCSR_TOF));
        }


        protected virtual void set_timer_event()
        {
            m_timer_next = (OCD - CTD < TOD - CTD) ? OCD : TOD;
        }


        /* when change freerunningcounter or outputcapture */
        protected virtual void modified_counters()
        {
            OCH = (OC >= CT) ? CTH : (uint16_t)(CTH + 1);
            set_timer_event();
        }


        /* check OCI or TOI */
        protected virtual void check_timer_event()
        {
            /* OCI */
            if (CTD >= OCD)
            {
                OCH++;  // next IRQ point
                m_tcsr |= TCSR_OCF;
                m_pending_tcsr |= TCSR_OCF;
                modified_tcsr();

                // if output on P21 is enabled, let's do it
                if ((m_port_ddr[1] & 2) != 0)
                {
                    m_port_data[1] = (uint8_t)(m_port_data[1] & ~2);
                    m_port_data[1] |= (uint8_t)((m_tcsr & TCSR_OLVL) << 1);
                    m_port2_written = true;
                    write_port2();
                }
            }
            /* TOI */
            if (CTD >= TOD)
            {
                TOH++;  // next IRQ point
#if false
                CLEANUP_COUNTERS();
#endif
                m_tcsr |= TCSR_TOF;
                m_pending_tcsr |= TCSR_TOF;
                modified_tcsr();
            }

            if ((m_irq2 & (TCSR_OCF | TCSR_TOF)) != 0)
            {
                if ((wai_state & M6800_SLP) != 0)
                    wai_state = (uint8_t)(wai_state & ~M6800_SLP);
                if ((cc & 0x10) == 0)
                    m6800_check_irq2();
            }

            /* set next event */
            set_timer_event();
        }


        void set_rmcr(uint8_t data)
        {
            if (m_rmcr == data) return;

            m_rmcr = data;

            switch ((m_rmcr & M6801_RMCR_CC_MASK) >> 2)
            {
            case 0:
                LOGSER("SCI: Using external serial clock: false\n");
                m_sci_timer.enable(false);
                m_use_ext_serclock = false;
                break;

            case 3: // external clock
                LOGSER("SCI: Using external serial clock: true\n");
                m_use_ext_serclock = true;
                m_sci_timer.enable(false);
                break;

            case 1:
            case 2:
                {
                    int divisor = M6801_RMCR_SS[m_rmcr & M6801_RMCR_SS_MASK];
                    attotime period = cycles_to_attotime((uint64_t)divisor);
                    LOGSER("SCI: Setting serial rate, Divisor: {0} Hz: {1}\n", divisor, period.as_hz());
                    m_sci_timer.adjust(period, 0, period);
                    m_use_ext_serclock = false;
                }
                break;
            }
        }


        protected virtual void write_port2()
        {
            if (!m_port2_written) return;

            uint8_t data = m_port_data[1];
            uint8_t ddr = (uint8_t)(m_port_ddr[1] & 0x1f);

            if ((ddr != 0x1f) && (ddr != 0))
            {
                data = (uint8_t)((m_port_data[1] & ddr) | (ddr ^ 0xff));
            }

            if ((m_trcsr & M6801_TRCSR_TE) != 0)
            {
                data = (uint8_t)((data & 0xef) | (m_tx << 4));
                ddr |= 0x10;
            }

            data &= 0x1f;

            m_out_port_func[1].op(0, data, ddr);
        }


        //int m6800_rx();
        //void serial_transmit();
        //void serial_receive();


        //TIMER_CALLBACK_MEMBER( sci_tick );
        void sci_tick(object ptr, int param)
        {
            throw new emu_unimplemented();
        }


        /*
            if change_pc() direccted these areas ,Call hd63701_trap_pc().
            'mode' is selected by the sense of p2.0,p2.1,and p2.3 at reset timming.
            mode 0,1,2,4,6 : $0000-$001f
            mode 5         : $0000-$001f,$0200-$efff
            mode 7         : $0000-$001f,$0100-$efff
        */

        void set_os3(int state)
        {
            LOG("OS3: {0}\n", state);

            m_out_sc2_func.op(state);
        }
    }


    public class m6803_cpu_device : m6801_cpu_device
    {
        //DEFINE_DEVICE_TYPE(M6803, m6803_cpu_device, "m6803", "Motorola MC6803")
        static device_t device_creator_m6803_cpu_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new m6803_cpu_device(mconfig, tag, owner, clock); }
        public static readonly device_type M6803 = DEFINE_DEVICE_TYPE(device_creator_m6803_cpu_device, "m6803", "Motorola MC6803");


        class device_disasm_interface_m6803 : device_disasm_interface_m6801
        {
            public device_disasm_interface_m6803(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        m6803_cpu_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, M6803, tag, owner, clock, m6803_insn, cycles_6803, m6803_mem)
        {
            m_class_interfaces.Add(new device_disasm_interface_m6803(mconfig, this));
        }
    }


#if false
    class m6803e_cpu_device : public m6801_cpu_device

    class hd6301_cpu_device : public m6801_cpu_device

    // DP-40 package: HD6301V1P,  HD63A01V1P,  HD63B01V1P
    // FP-54 package: HD6301V1F,  HD63A01V1F,  HD63B01V1F
    // CG-40 package: HD6301V1CG, HD63A01V1CG, HD63B01V1CG
    // CP-52 package: HD6301V1CP, HD63A01V1CP, HD63B01V1CP
    // CP-44 package: HD6301V1L,  HD63A01V1L,  HD63B01V1L
    // Not fully emulated yet
    class hd6301v1_cpu_device : public hd6301_cpu_device

    // DC-40 package: HD63701V0C, HD63A701V0C, HD63B701V0C
    // Not fully emulated yet
    class hd63701v0_cpu_device : public hd6301_cpu_device

    // DP-40 package: HD6303RP,  HD63A03RP,  HD63B03RP
    // FP-54 package: HD6303RF,  HD63A03RF,  HD63B03RF
    // CG-40 package: HD6303RCG, HD63A03RCG, HD63B03RCG
    // Not fully emulated yet
    class hd6303r_cpu_device : public hd6301_cpu_device

    class hd6301x_cpu_device : public hd6301_cpu_device

    // DP-64S package: HD6301X0P,  HD63A01X0P,  HD63B01X0P
    // FP-80  package: HD6301X0F,  HD63A01X0F,  HD63B01X0F
    // CP-68  package: HD6301X0CP, HD63A01X0CP, HD63B01X0CP
    // Not fully emulated yet
    class hd6301x0_cpu_device : public hd6301x_cpu_device

    // DC-64S package: HD63701X0C, HD63A701X0C, HD63B701X0C
    // Not fully emulated yet
    class hd63701x0_cpu_device : public hd6301x_cpu_device

    // DP-64S package: HD6303XP,  HD63A03XP,  HD63B03XP
    // FP-80  package: HD6303XF,  HD63A03XF,  HD63B03XF
    // CP-68  package: HD6303XCP, HD63A03XCP, HD63B03XCP
    // Not fully emulated yet
    class hd6303x_cpu_device : public hd6301x_cpu_device

    class hd6301y_cpu_device : public hd6301x_cpu_device

    // DP-64S package: HD6301Y0P,  HD63A01Y0P,  HD63B01Y0P,  HD63C01Y0P
    // FP-64  package: HD6301Y0F,  HD63A01Y0F,  HD63B01Y0F,  HD63C01Y0F
    // FP-64A package: HD6301Y0H,  HD63A01Y0H,  HD63B01Y0H,  HD63C01Y0H
    // CP-68  package: HD6301Y0CP, HD63A01Y0CP, HD63B01Y0CP, HD63C01Y0CP
    // Not fully emulated yet
    class hd6301y0_cpu_device : public hd6301y_cpu_device

    // DC-64S package: HD63701Y0C, HD63A701Y0C, HD63B701Y0C, HD63C701Y0C
    // Not fully emulated yet
    class hd63701y0_cpu_device : public hd6301y_cpu_device

    // DP-64S package: HD6303YP,  HD63A03YP,  HD63B03YP,  HD63C03YP
    // FP-64  package: HD6303YF,  HD63A03YF,  HD63B03YF,  HD63C03YF
    // FP-64A package: HD6303YH,  HD63A03YH,  HD63B03YH,  HD63C03YH
    // CP-68  package: HD6303YCP, HD63A03YCP, HD63B03YCP, HD63C03YCP
    // Not fully emulated yet
    class hd6303y_cpu_device : public hd6301y_cpu_device
#endif


    //DECLARE_DEVICE_TYPE(M6801, m6801_cpu_device)
    //DECLARE_DEVICE_TYPE(M6803, m6803_cpu_device)
    //DECLARE_DEVICE_TYPE(M6803E, m6803e_cpu_device)
    //DECLARE_DEVICE_TYPE(HD6301V1, hd6301v1_cpu_device)
    //DECLARE_DEVICE_TYPE(HD6301X0, hd6301x0_cpu_device)
    //DECLARE_DEVICE_TYPE(HD6301Y0, hd6301y0_cpu_device)
    //DECLARE_DEVICE_TYPE(HD63701V0, hd63701v0_cpu_device)
    //DECLARE_DEVICE_TYPE(HD63701X0, hd63701x0_cpu_device)
    //DECLARE_DEVICE_TYPE(HD63701Y0, hd63701y0_cpu_device)
    //DECLARE_DEVICE_TYPE(HD6303R, hd6303r_cpu_device)
    //DECLARE_DEVICE_TYPE(HD6303X, hd6303x_cpu_device)
    //DECLARE_DEVICE_TYPE(HD6303Y, hd6303y_cpu_device)
}
