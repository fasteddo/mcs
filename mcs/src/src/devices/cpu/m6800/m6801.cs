// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;


namespace mame
{
    class m6801_cpu_device : m6800_cpu_device
    {
        //DEFINE_DEVICE_TYPE(M6801, m6801_cpu_device, "m6801", "Motorola M6801")
        static device_t device_creator_m6801_cpu_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new m6801_cpu_device(mconfig, tag, owner, clock); }
        public static readonly device_type M6801 = DEFINE_DEVICE_TYPE(device_creator_m6801_cpu_device, "m6801", "Motorola M6801");


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
        const int M6801_TIN_LINE = M6800_IRQ_LINE + 1;                 /* P20/Tin Input Capture line (eddge sense)     */
                                        /* Active eddge is selecrable by internal reg.  */
                                        /* raise eddge : CLEAR_LINE  -> ASSERT_LINE     */
                                        /* fall  eddge : ASSERT_LINE -> CLEAR_LINE      */
        const int M6801_SC1_LINE = M6800_IRQ_LINE + 2;
        //}


        //#define LOG_GENERAL (1U << 0)
        const int LOG_TX      = 1 << 1;
        //#define LOG_TXTICK  (1U << 2)
        //#define LOG_RX      (1U << 3)
        //#define LOG_RXTICK  (1U << 4)
        const int LOG_PORT    = 1 << 5;

        //#define VERBOSE (LOG_GENERAL | LOG_TX | LOG_RX | LOG_PORT)
        //#define LOG_OUTPUT_STREAM std::cerr
        //#include "logmacro.h"

        void LOGTX(string format, params object [] args) { LOGMASKED(LOG_TX, format, args); }
        //#define LOGTXTICK(...)  LOGMASKED(LOG_TXTICK, __VA_ARGS__)
        //#define LOGRX(...)      LOGMASKED(LOG_RX, __VA_ARGS__)
        //#define LOGRXTICK(...)  LOGMASKED(LOG_RXTICK, __VA_ARGS__)
        public void LOGPORT(string format, params object [] args) { LOGMASKED(LOG_PORT, format, args); }


        uint16_t CT { get { return m_counter.w.l; } set { m_counter.w.l = value; } }
        uint16_t CTH { get { return m_counter.w.h; } set { m_counter.w.h = value; } }
        uint32_t CTD { get { return m_counter.d; } set { m_counter.d = value; } }
        uint16_t OC { get { return m_output_compare.w.l; } }
        uint16_t OCH { get { return m_output_compare.w.h; } set { m_output_compare.w.h = value; } }
        uint32_t OCD { get { return m_output_compare.d; } set { m_output_compare.d = value; } }
        uint16_t TOH { get { return m_timer_over.w.l; } set { m_timer_over.w.l = value; } }
        uint32_t TOD { get { return m_timer_over.d; } set { m_timer_over.d = value; } }


        void MODIFIED_tcsr() { m_irq2 = (uint8_t)((m_tcsr & (m_tcsr << 3)) & (TCSR_ICF | TCSR_OCF | TCSR_TOF)); }

        void SET_TIMER_EVENT() { m_timer_next = (OCD - CTD < TOD - CTD) ? OCD : TOD; }


        /* when change freerunningcounter or outputcapture */
        void MODIFIED_counters()
        {
            OCH = (OC >= CT) ? CTH : (uint16_t)(CTH + 1);
            SET_TIMER_EVENT();
        }


        // I/O registers

        //enum
        //{
        const int IO_P1DDR  =  0;
        const int IO_P2DDR  =  1;
        const int IO_P1DATA =  2;
        const int IO_P2DATA =  3;
        const int IO_P3DDR  =  4;
        const int IO_P4DDR  =  5;
        const int IO_P3DATA =  6;
        const int IO_P4DATA =  7;
        const int IO_TCSR   =  8;
        const int IO_CH     =  9;
        const int IO_CL     = 10;
        const int IO_OCRH   = 11;
        const int IO_OCRL   = 12;
        const int IO_ICRH   = 13;
        const int IO_ICRL   = 14;
        const int IO_P3CSR  = 15;
        const int IO_RMCR   = 16;
        const int IO_TRCSR  = 17;
        const int IO_RDR    = 18;
        const int IO_TDR    = 19;
        const int IO_RCR    = 20;
        const int IO_CAAH   = 21;
        const int IO_CAAL   = 22;
        const int IO_TCR1   = 23;
        const int IO_TCR2   = 24;
        const int IO_TSR    = 25;
        const int IO_OCR2H  = 26;
        const int IO_OCR2L  = 27;
        const int IO_OCR3H  = 28;
        const int IO_OCR3L  = 29;
        const int IO_ICR2H  = 30;
        const int IO_ICR2L  = 31;
        //}


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


        devcb_read8 [] m_in_port_func = new devcb_read8 [4];
        devcb_write8 [] m_out_port_func = new devcb_write8 [4];

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
        int m_port2_written;

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
                System.Reflection.MethodInfo methodInfo = typeof(m6801_cpu_device).GetMethod(insn[i], System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                m6803_insn[i] = (op_func)Delegate.CreateDelegate(typeof(op_func), null, methodInfo);
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

            m_in_port_func = new devcb_read8 [4] { new devcb_read8(this), new devcb_read8(this), new devcb_read8(this), new devcb_read8(this) };
            m_out_port_func = new devcb_write8 [4] { new devcb_write8(this), new devcb_write8(this), new devcb_write8(this), new devcb_write8(this) };
            m_out_sc2_func = new devcb_write_line(this);
            m_out_sertx_func = new devcb_write_line(this);
        }


        public uint8_t [] port_ddr { get { return m_port_ddr; } }
        public devcb_read8 [] in_port_func { get { return m_in_port_func; } }
        public uint8_t [] port_data { get { return m_port_data; } }
        public uint8_t p3csr { get { return m_p3csr; } set { m_p3csr = value; } }
        public uint8_t tcsr { get { return m_tcsr; } set { m_tcsr = value; } }
        public uint8_t pending_tcsr { get { return m_pending_tcsr; } set { m_pending_tcsr = value; } }
        public uint16_t input_capture { get { return m_input_capture; } set { m_input_capture = value; } }
        public int port3_latched { get { return m_port3_latched; } set { m_port3_latched = value; } }
        public int sc1_state { get { return m_sc1_state; } set { m_sc1_state = value; } }


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


        //READ8_MEMBER( m6801_cpu_device::m6801_io_r )
        protected u8 m6801_io_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            uint8_t data = 0;

            switch (offset)
            {
            case IO_P1DDR:
                data = m_port_ddr[0];
                break;

            case IO_P2DDR:
                data = m_port_ddr[1];
                break;

            case IO_P1DATA:
                if (m_port_ddr[0] == 0xff)
                    data = m_port_data[0];
                else
                    data = (uint8_t)((m_in_port_func[0].op() & (m_port_ddr[0] ^ 0xff)) | (m_port_data[0] & m_port_ddr[0]));
                break;

            case IO_P2DATA:
                if (m_port_ddr[1] == 0xff)
                    data = m_port_data[1];
                else
                    data = (uint8_t)((m_in_port_func[1].op() & (m_port_ddr[1] ^ 0xff)) | (m_port_data[1] & m_port_ddr[1]));
                break;

            case IO_P3DDR:
                data = 0xff;
                break;

            case IO_P4DDR:
                data = m_port_ddr[3];
                break;

            case IO_P3DATA:
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
                        set_os3(ASSERT_LINE);
                    }
                }

                if ((m_p3csr & M6801_P3CSR_LE) != 0 || (m_port_ddr[2] == 0xff))
                    data = m_port_data[2];
                else
                    data = (uint8_t)((m_in_port_func[2].op() & (m_port_ddr[2] ^ 0xff)) | (m_port_data[2] & m_port_ddr[2]));

                if (!machine().side_effects_disabled())
                {
                    m_port3_latched = 0;

                    if ((m_p3csr & M6801_P3CSR_OSS) == 0)
                    {
                        set_os3(CLEAR_LINE);
                    }
                }
                break;

            case IO_P4DATA:
                if (m_port_ddr[3] == 0xff)
                    data = m_port_data[3];
                else
                    data = (uint8_t)((m_in_port_func[3].op() & (m_port_ddr[3] ^ 0xff)) | (m_port_data[3] & m_port_ddr[3]));
                break;

            case IO_TCSR:
                m_pending_tcsr = 0;
                data = m_tcsr;
                break;

            case IO_CH:
                if ((m_pending_tcsr & TCSR_TOF) == 0 && !machine().side_effects_disabled())
                {
                    m_tcsr = (uint8_t)(m_tcsr & ~TCSR_TOF);
                    MODIFIED_tcsr();
                }
                data = m_counter.b.h;
                break;

            case IO_CL:
                data = m_counter.b.l;
                // HACK there should be a break here, but Coleco Adam won't boot with it present, proper fix required to the free-running counter
                if ((m_pending_tcsr & TCSR_OCF) == 0 && !machine().side_effects_disabled())
                {
                    m_tcsr = (uint8_t)(m_tcsr & ~TCSR_OCF);
                    MODIFIED_tcsr();
                }
                data = m_output_compare.b.h;
                break;

            case IO_OCRH:
                if ((m_pending_tcsr & TCSR_OCF) == 0 && !machine().side_effects_disabled())
                {
                    m_tcsr = (uint8_t)(m_tcsr & ~TCSR_OCF);
                    MODIFIED_tcsr();
                }
                data = m_output_compare.b.h;
                break;

            case IO_OCRL:
                if ((m_pending_tcsr & TCSR_OCF) == 0 && !machine().side_effects_disabled())
                {
                    m_tcsr = (uint8_t)(m_tcsr & ~TCSR_OCF);
                    MODIFIED_tcsr();
                }
                data = m_output_compare.b.l;
                break;

            case IO_ICRH:
                if ((m_pending_tcsr & TCSR_ICF) == 0 && !machine().side_effects_disabled())
                {
                    m_tcsr = (uint8_t)(m_tcsr & ~TCSR_ICF);
                    MODIFIED_tcsr();
                }
                data = (uint8_t)((m_input_capture >> 0) & 0xff);
                break;

            case IO_ICRL:
                data = (uint8_t)((m_input_capture >> 8) & 0xff);
                break;

            case IO_P3CSR:
                if ((m_p3csr & M6801_P3CSR_IS3_FLAG) != 0 && !machine().side_effects_disabled())
                {
                    m_p3csr_is3_flag_read = 1;
                }

                data = m_p3csr;
                break;

            case IO_RMCR:
                data = m_rmcr;
                break;

            case IO_TRCSR:
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

                data = m_trcsr;
                break;

            case IO_RDR:
                if (!machine().side_effects_disabled())
                {
                    if (m_trcsr_read_orfe != 0)
                    {
                        LOG("Cleared ORFE\n");
                        m_trcsr_read_orfe = 0;
                        m_trcsr = (uint8_t)(m_trcsr & ~M6801_TRCSR_ORFE);
                    }

                    if (m_trcsr_read_rdrf != 0)
                    {
                        LOG("Cleared RDRF\n");
                        m_trcsr_read_rdrf = 0;
                        m_trcsr = (uint8_t)(m_trcsr & ~M6801_TRCSR_RDRF);
                    }
                }

                data = m_rdr;
                break;

            case IO_TDR:
                data = m_tdr;
                break;

            case IO_RCR:
                data = m_ram_ctrl;
                break;

            case IO_CAAH:
            case IO_CAAL:
            case IO_TCR1:
            case IO_TCR2:
            case IO_TSR:
            case IO_OCR2H:
            case IO_OCR2L:
            case IO_OCR3H:
            case IO_OCR3L:
            case IO_ICR2H:
            case IO_ICR2L:
            default:
                logerror("PC {0}: warning - read from reserved internal register {1}\n", state().pc(), offset);
                break;
            }

            return data;
        }


        //WRITE8_MEMBER( m6801_cpu_device::m6801_io_w )
        protected void m6801_io_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            switch (offset)
            {
            case IO_P1DDR:
                LOGPORT("Port 1 Data Direction Register: {0}\n", data);

                if (m_port_ddr[0] != data)
                {
                    m_port_ddr[0] = data;
                    m_out_port_func[0].op((uint8_t)((m_port_data[0] & m_port_ddr[0]) | (m_port_ddr[0] ^ 0xff)));
                }
                break;

            case IO_P2DDR:
                LOGPORT("Port 2 Data Direction Register: {0}\n", data);

                if (m_port_ddr[1] != data)
                {
                    m_port_ddr[1] = data;
                    write_port2();
                }
                break;

            case IO_P1DATA:
                LOGPORT("Port 1 Data Register: {0}\n", data);

                m_port_data[0] = data;
                m_out_port_func[0].op((uint8_t)((m_port_data[0] & m_port_ddr[0]) | (m_port_ddr[0] ^ 0xff)));
                break;

            case IO_P2DATA:
                LOGPORT("Port 2 Data Register: {0}\n", data);

                m_port_data[1] = data;
                m_port2_written = 1;
                write_port2();
                break;

            case IO_P3DDR:
                LOGPORT("Port 3 Data Direction Register: {0}\n", data);

                if (m_port_ddr[2] != data)
                {
                    m_port_ddr[2] = data;
                    m_out_port_func[2].op((uint8_t)((m_port_data[2] & m_port_ddr[2]) | (m_port_ddr[2] ^ 0xff)));
                }
                break;

            case IO_P4DDR:
                LOGPORT("Port 4 Data Direction Register: {0}\n", data);

                if (m_port_ddr[3] != data)
                {
                    m_port_ddr[3] = data;
                    m_out_port_func[3].op((uint8_t)((m_port_data[3] & m_port_ddr[3]) | (m_port_ddr[3] ^ 0xff)));
                }
                break;

            case IO_P3DATA:
                LOGPORT("Port 3 Data Register: {0}\n", data);

                if (m_p3csr_is3_flag_read != 0)
                {
                    LOGPORT("Cleared IS3\n");
                    m_p3csr = (uint8_t)(m_p3csr & ~M6801_P3CSR_IS3_FLAG);
                    m_p3csr_is3_flag_read = 0;
                }

                if ((m_p3csr & M6801_P3CSR_OSS) != 0)
                {
                    set_os3(ASSERT_LINE);
                }

                m_port_data[2] = data;
                m_out_port_func[2].op((uint8_t)((m_port_data[2] & m_port_ddr[2]) | (m_port_ddr[2] ^ 0xff)));

                if ((m_p3csr & M6801_P3CSR_OSS) != 0)
                {
                    set_os3(CLEAR_LINE);
                }
                break;

            case IO_P4DATA:
                LOGPORT("Port 4 Data Register: {0}\n", data);

                m_port_data[3] = data;
                m_out_port_func[3].op((uint8_t)((m_port_data[3] & m_port_ddr[3]) | (m_port_ddr[3] ^ 0xff)));
                break;

            case IO_TCSR:
                LOG("Timer Control and Status Register: {0}\n", data);

                m_tcsr = data;
                m_pending_tcsr &= m_tcsr;
                MODIFIED_tcsr();
                if ((cc & 0x10) == 0)
                    m6800_check_irq2();
                break;

            case IO_CH:
                LOG("Counter High Register: {0}\n", data);

                m_latch09 = data & 0xff;    /* 6301 only */
                CT  = 0xfff8;
                TOH = CTH;
                MODIFIED_counters();
                break;

            case IO_CL: /* 6301 only */
                LOG("Counter Low Register: {0}\n", data);

                CT = (uint16_t)((m_latch09 << 8) | (data & 0xff));
                TOH = CTH;
                MODIFIED_counters();
                break;

            case IO_OCRH:
                LOG("Output Compare High Register: {0}\n", data);

                if( m_output_compare.b.h != data)
                {
                    m_output_compare.b.h = data;
                    MODIFIED_counters();
                }
                break;

            case IO_OCRL:
                LOG("Output Compare Low Register: {0}\n", data);

                if( m_output_compare.b.l != data)
                {
                    m_output_compare.b.l = data;
                    MODIFIED_counters();
                }
                break;

            case IO_ICRH:
            case IO_ICRL:
            case IO_RDR:
                LOG("PC {0}: warning - write {1} to read only internal register {2}\n", state().pc(), data, offset);
                break;

            case IO_P3CSR:
                LOGPORT("Port 3 Control and Status Register: {0}\n", data);

                m_p3csr = data;
                break;

            case IO_RMCR:
                LOG("Rate and Mode Control Register: {0}\n", data);

                set_rmcr(data);
                break;

            case IO_TRCSR:
                LOG("Transmit/Receive Control and Status Register: {0}\n", data);

                if ((data & M6801_TRCSR_TE) != 0 && (m_trcsr & M6801_TRCSR_TE) == 0)
                {
                    m_txstate = M6801_TX_STATE_INIT;
                    m_txbits = 0;
                    m_tx = 1;
                }

                if ((data & M6801_TRCSR_RE) != 0 && (m_trcsr & M6801_TRCSR_RE) == 0)
                {
                    m_rxbits = 0;
                }

                m_trcsr = (uint8_t)((m_trcsr & 0xe0) | (data & 0x1f));
                break;

            case IO_TDR:
                LOGTX("Transmit Data Register: {0}\n", data);

                if (m_trcsr_read_tdre != 0)
                {
                    m_trcsr_read_tdre = 0;
                    m_trcsr = (uint8_t)(m_trcsr & ~M6801_TRCSR_TDRE);
                }
                m_tdr = data;
                break;

            case IO_RCR:
                LOG("RAM Control Register: {0}\n", data);

                m_ram_ctrl = data;
                break;

            case IO_CAAH:
            case IO_CAAL:
            case IO_TCR1:
            case IO_TCR2:
            case IO_TSR:
            case IO_OCR2H:
            case IO_OCR2L:
            case IO_OCR3H:
            case IO_OCR3L:
            case IO_ICR2H:
            case IO_ICR2L:
            default:
                logerror("PC {0}: warning - write {1} to reserved internal register {2}\n", state().pc(), data, offset);
                break;
            }
        }


        //void m6801_clock_serial();


        // device-level overrides
        protected override void device_resolve_objects()
        {
            foreach (var cb in m_in_port_func)
                cb.resolve_safe(0xff);

            foreach (var cb in m_out_port_func)
                cb.resolve_safe();

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

            save_item(m_port_ddr, "m_port_ddr");
            save_item(m_port_data, "m_port_data");
            save_item(m_p3csr, "m_p3csr");
            save_item(m_tcsr, "m_tcsr");
            save_item(m_pending_tcsr, "m_pending_tcsr");
            save_item(m_irq2, "m_irq2");
            save_item(m_ram_ctrl, "m_ram_ctrl");

            save_item(m_counter.d, "m_counter.d");
            save_item(m_output_compare.d, "m_output_compare.d");
            save_item(m_input_capture, "m_input_capture");
            save_item(m_p3csr_is3_flag_read, "m_p3csr_is3_flag_read");
            save_item(m_port3_latched, "m_port3_latched");
            save_item(m_port2_written, "m_port2_written");

            save_item(m_trcsr, "m_trcsr");
            save_item(m_rmcr, "m_rmcr");
            save_item(m_rdr, "m_rdr");
            save_item(m_tdr, "m_tdr");
            save_item(m_rsr, "m_rsr");
            save_item(m_tsr, "m_tsr");
            save_item(m_rxbits, "m_rxbits");
            save_item(m_txbits, "m_txbits");
            save_item(m_txstate, "m_txstate");
            save_item(m_trcsr_read_tdre, "m_trcsr_read_tdre");
            save_item(m_trcsr_read_orfe, "m_trcsr_read_orfe");
            save_item(m_trcsr_read_rdrf, "m_trcsr_read_rdrf");
            save_item(m_tx, "m_tx");
            save_item(m_ext_serclock, "m_ext_serclock");
            save_item(m_use_ext_serclock, "m_use_ext_serclock");

            save_item(m_latch09, "m_latch09");

            save_item(m_timer_over.d, "m_timer_over.d");

            save_item(m_timer_next, "m_timer_next");

            save_item(m_sc1_state, "m_sc1_state");
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
            m_port2_written = 0;
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
                if (sc1_state == 0 && (CLEAR_LINE != state))
                {
                    if (port3_latched == 0 && (p3csr & M6801_P3CSR_LE) != 0)
                    {
                        // latch input data to port 3
                        port_data[2] = (uint8_t)((in_port_func[2].op() & (port_ddr[2] ^ 0xff)) | (port_data[2] & port_ddr[2]));
                        port3_latched = 1;
                        LOGPORT("Latched Port 3 Data: {0}\n", port_data[2]);

                        // set IS3 flag bit
                        p3csr |= M6801_P3CSR_IS3_FLAG;
                    }
                    else
                    {
                        LOGPORT("Not latching Port 3 Data:{0}{1}", port3_latched != 0 ? " already latched" : "", (p3csr & M6801_P3CSR_LE) != 0 ? "" : " LE clear");
                    }
                }

                sc1_state = ASSERT_LINE == state ? 1 : 0;
                if (CLEAR_LINE != state)
                    standard_irq_callback(M6801_SC1_LINE); // re-entrant - do it after setting m_sc1_state

                break;

            case M6801_TIN_LINE:
                irq_state[M6801_TIN_LINE] = (uint8_t)state;

                if (state != irq_state[M6801_TIN_LINE])
                {
                    //eddge = (state == CLEAR_LINE ) ? 2 : 0;
                    if (((tcsr & TCSR_IEDG) ^ (state == CLEAR_LINE ? TCSR_IEDG : 0)) == 0)
                        return;

                    /* active edge in */
                    tcsr |= TCSR_ICF;
                    pending_tcsr |= TCSR_ICF;
                    input_capture = CT;
                    MODIFIED_tcsr();
                }
                break;

            default:
                base.device_execute_interface_execute_set_input(irqline, state);  //execute_set_input(irqline, state);
                break;
            }
        }


        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        protected static void m6803_mem(address_map map, device_t owner)
        {
            m6801_cpu_device m6801 = (m6801_cpu_device)owner;

            map.op(0x0000, 0x001f).rw(m6801.m6801_io_r, m6801.m6801_io_w);
            map.op(0x0020, 0x007f).noprw();        /* unused */
            map.op(0x0080, 0x00ff).ram();        /* 6803 internal RAM */
        }


        protected override void m6800_check_irq2()
        {
            if ((m_tcsr & (TCSR_EICI | TCSR_ICF)) == (TCSR_EICI | TCSR_ICF))
            {
                TAKE_ICI();
                diexec.standard_irq_callback(M6801_TIN_LINE);
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
            SET_TIMER_EVENT();
        }


        /* check OCI or TOI */
        void check_timer_event()
        {
            /* OCI */
            if (CTD >= OCD)
            {
                OCH++;  // next IRQ point
                m_tcsr |= TCSR_OCF;
                m_pending_tcsr |= TCSR_OCF;
                MODIFIED_tcsr();
                if ((m_tcsr & TCSR_EOCI) != 0 && (wai_state & M6800_SLP) != 0)
                    wai_state = (uint8_t)(wai_state & ~M6800_SLP);
                if ((cc & 0x10) == 0 && (m_tcsr & TCSR_EOCI) != 0)
                    TAKE_OCI();

                // if output on P21 is enabled, let's do it
                if ((m_port_ddr[1] & 2) != 0)
                {
                    m_port_data[1] = (uint8_t)(m_port_data[1] & ~2);
                    m_port_data[1] |= (uint8_t)((m_tcsr & TCSR_OLVL) << 1);
                    m_port2_written = 1;
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
                MODIFIED_tcsr();
                if ((m_tcsr & TCSR_ETOI) != 0 && (wai_state & M6800_SLP) != 0)
                    wai_state = (uint8_t)(wai_state & ~M6800_SLP);
                if ((cc & 0x10) == 0 && (m_tcsr & TCSR_ETOI) != 0)
                    TAKE_TOI();
            }

            /* set next event */
            SET_TIMER_EVENT();
        }


        void set_rmcr(uint8_t data)
        {
            if (m_rmcr == data) return;

            m_rmcr = data;

            switch ((m_rmcr & M6801_RMCR_CC_MASK) >> 2)
            {
            case 0:
                m_sci_timer.enable(false);
                m_use_ext_serclock = false;
                break;

            case 3: // external clock
                m_use_ext_serclock = true;
                m_sci_timer.enable(false);
                break;

            case 1:
            case 2:
                {
                    int divisor = M6801_RMCR_SS[m_rmcr & M6801_RMCR_SS_MASK];
                    attotime period = cycles_to_attotime((u64)divisor);

                    m_sci_timer.adjust(period, 0, period);
                    m_use_ext_serclock = false;
                }
                break;
            }
        }


        void write_port2()
        {
            if (m_port2_written == 0) return;

            uint8_t data = m_port_data[1];
            uint8_t ddr = (uint8_t)(m_port_ddr[1] & 0x1f);

            if ((ddr != 0x1f) && (ddr != 0))
            {
                data = (uint8_t)((m_port_data[1] & ddr) | (ddr ^ 0xff));
            }

            if ((m_trcsr & M6801_TRCSR_TE) != 0)
            {
                data = (uint8_t)((data & 0xef) | (m_tx << 4));
            }

            data &= 0x1f;

            m_out_port_func[1].op(data);
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


    class m6803_cpu_device : m6801_cpu_device
    {
        //DEFINE_DEVICE_TYPE(M6803, m6803_cpu_device, "m6803", "Motorola M6803")
        static device_t device_creator_m6803_cpu_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new m6803_cpu_device(mconfig, tag, owner, clock); }
        public static readonly device_type M6803 = DEFINE_DEVICE_TYPE(device_creator_m6803_cpu_device, "m6803", "Motorola M6803");


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
    class hd6301_cpu_device : public m6801_cpu_device
    {
    public:
        hd6301_cpu_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock);

    protected:
        hd6301_cpu_device(const machine_config &mconfig, device_type type, const char *tag, device_t *owner, uint32_t clock);

        virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;
    };


    class hd63701_cpu_device : public m6801_cpu_device
    {
    public:
        hd63701_cpu_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock);

    protected:
        virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;

        virtual void TAKE_TRAP() override;
    };


    // DP-40 package: HD6303RP,  HD63A03RP,  HD63B03RP,
    // FP-54 package: HD6303RF,  HD63A03RF,  HD63B03RF,
    // CG-40 package: HD6303RCG, HD63A03RCG, HD63B03RCG,
    // Not fully emulated yet
    class hd6303r_cpu_device : public hd6301_cpu_device
    {
    public:
        hd6303r_cpu_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock);
    };


    // DP-64S package: HD6303YP,  HD63A03YP,  HD63B03YP,  HD63C03YP
    // FP-64  package: HD6303YF,  HD63A03YF,  HD63B03YF,  HD63C03YF
    // FP-64A package: HD6303YH,  HD63A03YH,  HD63B03YH,  HD63C03YH
    // CP-68  package: HD6303YCP, HD63A03YCP, HD63B03YCP, HD63C03YCP
    // Not fully emulated yet
    class hd6303y_cpu_device : public hd6301_cpu_device
    {
    public:
        hd6303y_cpu_device(const machine_config &mconfig, const char *tag, device_t *owner, uint32_t clock);
    };
#endif


    //DECLARE_DEVICE_TYPE(HD6301, hd6301_cpu_device)
    //DECLARE_DEVICE_TYPE(HD63701, hd63701_cpu_device)
    //DECLARE_DEVICE_TYPE(HD6303R, hd6303r_cpu_device)
    //DECLARE_DEVICE_TYPE(HD6303Y, hd6303y_cpu_device)
}
