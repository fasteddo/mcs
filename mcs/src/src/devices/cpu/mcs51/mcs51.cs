// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using devcb_read8 = mame.devcb_read<mame.Type_constant_u8>;  //using devcb_read8 = devcb_read<u8>;
using devcb_write8 = mame.devcb_write<mame.Type_constant_u8>;  //using devcb_write8 = devcb_write<u8>;
using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int32_t = System.Int32;
using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.device_global;
using static mame.diexec_global;
using static mame.distate_global;
using static mame.emucore_global;
using static mame.emumem_global;
using static mame.mcs51_global;
using static mame.mcs51_internal;


namespace mame
{
    static partial class mcs51_global
    {
        //enum
        //{
            public const int MCS51_PC   =  1;
            public const int MCS51_SP   =  2;
            public const int MCS51_PSW  =  3;
            public const int MCS51_ACC  =  4;
            public const int MCS51_B    =  5;
            public const int MCS51_DPTR =  6;
            public const int MCS51_DPH  =  7;
            public const int MCS51_DPL  =  8;
            public const int MCS51_IE   =  9;
            public const int MCS51_IP   = 10;

            public const int MCS51_P0   = 11;
            public const int MCS51_P1   = 12;
            public const int MCS51_P2   = 13;
            public const int MCS51_P3   = 14;

            public const int MCS51_R0   = 15;
            public const int MCS51_R1   = 16;
            public const int MCS51_R2   = 17;
            public const int MCS51_R3   = 18;
            public const int MCS51_R4   = 19;
            public const int MCS51_R5   = 20;
            public const int MCS51_R6   = 21;
            public const int MCS51_R7   = 22;
            public const int MCS51_RB   = 23;

            public const int MCS51_TCON = 24;
            public const int MCS51_TMOD = 25;
            public const int MCS51_TL0  = 26;
            public const int MCS51_TL1  = 27;
            public const int MCS51_TH0  = 28;
            public const int MCS51_TH1  = 29;
        //};


        //enum
        //{
            public const int MCS51_INT0_LINE   = 0;    /* P3.2: External Interrupt 0 */
            public const int MCS51_INT1_LINE   = 1;        /* P3.3: External Interrupt 1 */
            public const int MCS51_RX_LINE     = 2;          /* P3.0: Serial Port Receive Line */
            public const int MCS51_T0_LINE     = 3;          /* P3,4: Timer 0 External Input */
            public const int MCS51_T1_LINE     = 4;          /* P3.5: Timer 1 External Input */
            public const int MCS51_T2_LINE     = 5;          /* P1.0: Timer 2 External Input */
            public const int MCS51_T2EX_LINE   = 6;        /* P1.1: Timer 2 Capture Reload Trigger */

            public const int DS5002FP_PFI_LINE = 7;       /* DS5002FP Power fail interrupt */
        //};
    }


    public class mcs51_cpu_device : cpu_device
    {
        public class device_execute_interface_mcs51 : device_execute_interface
        {
            public device_execute_interface_mcs51(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override uint64_t execute_clocks_to_cycles(uint64_t clocks) { return ((mcs51_cpu_device)device()).device_execute_interface_execute_clocks_to_cycles(clocks); }
            protected override uint64_t execute_cycles_to_clocks(uint64_t cycles) { throw new emu_unimplemented(); }
            protected override uint32_t execute_min_cycles() { return ((mcs51_cpu_device)device()).device_execute_interface_execute_min_cycles(); }
            protected override uint32_t execute_max_cycles() { throw new emu_unimplemented(); }
            protected override uint32_t execute_input_lines() { throw new emu_unimplemented(); }
            protected override void execute_run() { ((mcs51_cpu_device)device()).device_execute_interface_execute_run(); }
            protected override void execute_set_input(int inputnum, int state) { ((mcs51_cpu_device)device()).device_execute_interface_execute_set_input(inputnum, state); }
        }


        public class device_memory_interface_mcs51 : device_memory_interface
        {
            public device_memory_interface_mcs51(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((mcs51_cpu_device)device()).device_memory_interface_memory_space_config(); }
        }


        public class device_state_interface_mcs51 : device_state_interface
        {
            public device_state_interface_mcs51(machine_config mconfig, device_t device) : base(mconfig, device) { }

            public override void state_import(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_export(device_state_entry entry) { throw new emu_unimplemented(); }
            protected override void state_string_export(device_state_entry entry, out string str) { throw new emu_unimplemented(); }
        }


        public class device_disasm_interface_mcs51 : device_disasm_interface
        {
            public device_disasm_interface_mcs51(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override util.disasm_interface create_disassembler() { throw new emu_unimplemented(); }
        }


        const int VERBOSE = 0;  //#define VERBOSE 0

        public void LOG(string format, params object [] args) { if (VERBOSE != 0) logerror(format, args); }  //#define LOG(x)  do { if (VERBOSE) logerror x; } while (0)


        /***************************************************************************
            MACROS
        ***************************************************************************/

        /* Read Opcode/Opcode Arguments from Program Code */
        //#define ROP(pc)         m_program.read_byte(pc)
        //#define ROP_ARG(pc)     m_program.read_byte(pc)

        /* Read a byte from External Code Memory (Usually Program Rom(s) Space) */
        //#define CODEMEM_R(a)    (uint8_t)m_program.read_byte(a)

        /* Read/Write a byte from/to External Data Memory (Usually RAM or other I/O) */
        //#define DATAMEM_R(a)    (uint8_t)m_io.read_byte(a)
        //#define DATAMEM_W(a,v)  m_io.write_byte(a, v)

        /* Read/Write a byte from/to the Internal RAM */

        //#define IRAM_R(a)       iram_read(a)
        void IRAM_W(size_t a, uint8_t d) { iram_write(a, d); }

        /* Read/Write a byte from/to the Internal RAM indirectly */
        /* (called from indirect addressing)                     */
        //uint8_t mcs51_cpu_device::iram_iread(offs_t a) { return (a <= m_ram_mask) ? m_data.read_byte(a) : 0xff; }
        //void mcs51_cpu_device::iram_iwrite(offs_t a, uint8_t d) { if (a <= m_ram_mask) m_data.write_byte(a, d); }

        //#define IRAM_IR(a)      iram_iread(a)
        //#define IRAM_IW(a, d)   iram_iwrite(a, d)

        /* Form an Address to Read/Write to External RAM indirectly */
        /* (called from indirect addressing)                        */
        //#define ERAM_ADDR(a,m)  external_ram_iaddr(a,m)

        /* Read/Write a bit from Bit Addressable Memory */
        //#define BIT_R(a)        bit_address_r(a)
        //#define BIT_W(a,v)      bit_address_w(a, v)


        /***************************************************************************
            SHORTCUTS
        ***************************************************************************/

        uint16_t PPC { get { return m_ppc; } set { m_ppc = value; } }
        uint16_t PC { get { return m_pc; } set { m_pc = value; } }
        uint8_t RWM { get { return m_rwm; } set { m_rwm = value; } }

        /* SFR Registers - These are accessed directly for speed on read */
        /* Read accessors                                                */

        PointerU8 SFR_A(uint8_t a) { return m_sfr_ram[a]; }
        //#define SET_SFR_A(a,v)  do { SFR_A(a) = (v); } while (0)

        uint8_t ACC { get { return SFR_A(ADDR_ACC).op; } }
        uint8_t PSW { get { return SFR_A(ADDR_PSW).op; } }

        //#define P0          ((const uint8_t) SFR_A(ADDR_P0))
        //#define P1          ((const uint8_t) SFR_A(ADDR_P1))
        //#define P2          ((const uint8_t) SFR_A(ADDR_P2))
        //#define P3          ((const uint8_t) SFR_A(ADDR_P3))

        uint8_t SP { get { return SFR_A(ADDR_SP).op; } set { SFR_A(ADDR_SP).op = value; } }
        uint8_t DPL { get { return SFR_A(ADDR_DPL).op; } set { SFR_A(ADDR_DPL).op = value; } }
        uint8_t DPH { get { return SFR_A(ADDR_DPH).op; } set { SFR_A(ADDR_DPH).op = value; } }
        uint8_t PCON { get { return SFR_A(ADDR_PCON).op; } set { SFR_A(ADDR_PCON).op = value; } }
        uint8_t TCON { get { return SFR_A(ADDR_TCON).op; } set { SFR_A(ADDR_TCON).op = value; } }
        uint8_t TMOD { get { return SFR_A(ADDR_TMOD).op; } set { SFR_A(ADDR_TMOD).op = value; } }
        uint8_t TL0 { get { return SFR_A(ADDR_TL0).op; } set { SFR_A(ADDR_TL0).op = value; } }
        uint8_t TL1 { get { return SFR_A(ADDR_TL1).op; } set { SFR_A(ADDR_TL1).op = value; } }
        uint8_t TH0 { get { return SFR_A(ADDR_TH0).op; } set { SFR_A(ADDR_TH0).op = value; } }
        uint8_t TH1 { get { return SFR_A(ADDR_TH1).op; } set { SFR_A(ADDR_TH1).op = value; } }
        uint8_t SCON { get { return SFR_A(ADDR_SCON).op; } set { SFR_A(ADDR_SCON).op = value; } }
        uint8_t IE { get { return SFR_A(ADDR_IE).op; } set { SFR_A(ADDR_IE).op = value; } }
        uint8_t IP { get { return SFR_A(ADDR_IP).op; } set { SFR_A(ADDR_IP).op = value; } }
        uint8_t B { get { return SFR_A(ADDR_B).op; } set { SFR_A(ADDR_B).op = value; } }
        //#define SBUF        SFR_A(ADDR_SBUF)

        //#define R_REG(r)    m_scratchpad[(r) | (PSW & 0x18)]
        //#define DPTR        ((DPH<<8) | DPL)

        /* 8052 Only registers */
        uint8_t T2CON { get { return SFR_A(ADDR_T2CON).op; } set { SFR_A(ADDR_T2CON).op = value; } }
        uint8_t RCAP2L { get { return SFR_A(ADDR_RCAP2L).op; } set { SFR_A(ADDR_RCAP2L).op = value; } }
        uint8_t RCAP2H { get { return SFR_A(ADDR_RCAP2H).op; } set { SFR_A(ADDR_RCAP2H).op = value; } }
        uint8_t TL2 { get { return SFR_A(ADDR_TL2).op; } set { SFR_A(ADDR_TL2).op = value; } }
        uint8_t TH2 { get { return SFR_A(ADDR_TH2).op; } set { SFR_A(ADDR_TH2).op = value; } }

        /* 80C52 Only registers */
        uint8_t IPH { get { return SFR_A(ADDR_IPH).op; } set { SFR_A(ADDR_IPH).op = value; } }
        uint8_t SADDR { get { return SFR_A(ADDR_SADDR).op; } set { SFR_A(ADDR_SADDR).op = value; } }
        uint8_t SADEN { get { return SFR_A(ADDR_SADEN).op; } set { SFR_A(ADDR_SADEN).op = value; } }

        /* Philips 80C52 */
        /* ============= */
        /* Reduced EMI Mode
         * The AO bit (AUXR.0) in the AUXR register when set disables the
         * ALE output.
         */
        //#define AUXR        SFR_A(ADDR_AUXR)

        /* The dual DPTR structure (see Figure 12) is a way by which the
         * 80C52/54/58 will specify the address of an external data memory
         * location. There are two 16-bit DPTR registers that address the
         * external memory, and a single bit called DPS = AUXR1/bit0 that
         * allows the program code to switch between them.
         */
        //#define AUXR1       SFR_A(ADDR_AUXR1)

        /* DS5002FP only registers */
        uint8_t CRCR { get { return SFR_A(ADDR_CRCR).op; } set { SFR_A(ADDR_CRCR).op = value; } }
        uint8_t CRCL { get { return SFR_A(ADDR_CRCL).op; } set { SFR_A(ADDR_CRCL).op = value; } }
        uint8_t CRCH { get { return SFR_A(ADDR_CRCH).op; } set { SFR_A(ADDR_CRCH).op = value; } }
        uint8_t MCON { get { return SFR_A(ADDR_MCON).op; } set { SFR_A(ADDR_MCON).op = value; } }
        uint8_t TA { get { return SFR_A(ADDR_TA).op; } set { SFR_A(ADDR_TA).op = value; } }
        uint8_t RNR { get { return SFR_A(ADDR_RNR).op; } set { SFR_A(ADDR_RNR).op = value; } }
        uint8_t RPCTL { get { return SFR_A(ADDR_RPCTL).op; } set { SFR_A(ADDR_RPCTL).op = value; } }
        uint8_t RPS { get { return SFR_A(ADDR_RPS).op; } set { SFR_A(ADDR_RPS).op = value; } }


        /* WRITE accessors */

        /* Shortcuts */

        void SET_PSW(uint8_t v) { SFR_A(ADDR_PSW).op = v; SET_PARITY(); }
        void SET_ACC(uint8_t v) { SFR_A(ADDR_ACC).op = v; SET_PARITY(); }

        /* These trigger actions on modification and have to be written through SFR_W */
        void SET_P0(uint8_t v) { IRAM_W(ADDR_P0, v); }
        void SET_P1(uint8_t v) { IRAM_W(ADDR_P1, v); }
        void SET_P2(uint8_t v) { IRAM_W(ADDR_P2, v); }
        void SET_P3(uint8_t v) { IRAM_W(ADDR_P3, v); }

        /* Within the cpu core, do not trigger a send */
        //#define SET_SBUF(v) SET_SFR_A(ADDR_SBUF, v)

        /* No actions triggered on write */
        //#define SET_REG(r, v)   do { m_scratchpad[(r) | (PSW & 0x18)] = (v); } while (0)

        //#define SET_DPTR(n)     do { DPH = ((n) >> 8) & 0xff; DPL = (n) & 0xff; } while (0)

        /* Macros for Setting Flags */
        //#define SET_X(R, v) do { R = (v);} while (0)

        //#define SET_CY(n)       SET_PSW((PSW & 0x7f) | (n<<7))  //Carry Flag
        //#define SET_AC(n)       SET_PSW((PSW & 0xbf) | (n<<6))  //Aux.Carry Flag
        //#define SET_FO(n)       SET_PSW((PSW & 0xdf) | (n<<5))  //User Flag
        //#define SET_RS(n)       SET_PSW((PSW & 0xe7) | (n<<3))  //R Bank Select
        //#define SET_OV(n)       SET_PSW((PSW & 0xfb) | (n<<2))  //Overflow Flag
        //#define SET_P(n)        SET_PSW((PSW & 0xfe) | (n<<0))  //Parity Flag

        void SET_BIT(ref uint8_t R, int n, uint32_t v) { R = (uint8_t)((R & ~(1U << n)) | (v << n)); }
        uint8_t GET_BIT(uint32_t R, int n) { return (uint8_t)((R >> n) & 0x01); }

        //#define SET_EA(n)       SET_BIT(IE, 7, n)       //Global Interrupt Enable/Disable
        //#define SET_ES(n)       SET_BIT(IE, 4, v)       //Serial Interrupt Enable/Disable
        //#define SET_ET1(n)      SET_BIT(IE, 3, n)       //Timer 1 Interrupt Enable/Disable
        //#define SET_EX1(n)      SET_BIT(IE, 2, n)       //External Int 1 Interrupt Enable/Disable
        //#define SET_ET0(n)      SET_BIT(IE, 1, n)       //Timer 0 Interrupt Enable/Disable
        //#define SET_EX0(n)      SET_BIT(IE, 0, n)       //External Int 0 Interrupt Enable/Disable
        /* 8052 Only flags */
        //#define SET_ET2(n)      SET_BIT(IE, 5, n)       //Timer 2 Interrupt Enable/Disable

        /* 8052 Only flags */
        //#define SET_PT2(n)      SET_BIT(IP, 5, n);  //Set Timer 2 Priority Level

        //#define SET_PS0(n)      SET_BIT(IP, 4, n)       //Set Serial Priority Level
        //#define SET_PT1(n)      SET_BIT(IP, 3, n)       //Set Timer 1 Priority Level
        //#define SET_PX1(n)      SET_BIT(IP, 2, n)       //Set External Int 1 Priority Level
        //#define SET_PT0(n)      SET_BIT(IP, 1, n)       //Set Timer 0 Priority Level
        //#define SET_PX0(n)      SET_BIT(IP, 0, n)       //Set External Int 0 Priority Level

        //#define SET_TF1(n)      SET_BIT(TCON, 7, n) //Indicated Timer 1 Overflow Int Triggered
        //#define SET_TR1(n)      SET_BIT(TCON, 6, n)  //IndicateS Timer 1 is running
        //#define SET_TF0(n)      SET_BIT(TCON, 5, n) //Indicated Timer 0 Overflow Int Triggered
        //#define SET_TR0(n)      SET_BIT(TCON, 4, n)  //IndicateS Timer 0 is running
        void SET_IE1(uint8_t n) { var temp = TCON; SET_BIT(ref temp, 3, n); TCON = temp; }  //Indicated External Int 1 Triggered
        //#define SET_IT1(n)      SET_BIT(TCON, 2, n)  //Indicates how External Int 1 is Triggered
        void SET_IE0(uint8_t n) { var temp = TCON; SET_BIT(ref temp, 1, n); TCON = temp; }  //Indicated External Int 0 Triggered
        //#define SET_IT0(n)      SET_BIT(TCON, 0, n)  //Indicates how External Int 0 is Triggered

        //#define SET_SM0(n)      SET_BIT(SCON, 7, n) //Sets Serial Port Mode
        //#define SET_SM1(n)      SET_BIT(SCON, 6, n)  //Sets Serial Port Mode
        //#define SET_SM2(n)      SET_BIT(SCON, 5, n) //Sets Serial Port Mode (Multiprocesser mode)
        //#define SET_REN(n)      SET_BIT(SCON, 4, n)  //Sets Serial Port Receive Enable
        //#define SET_TB8(n)      SET_BIT(SCON, 3, n)  //Transmit 8th Bit
        //#define SET_RB8(n)      SET_BIT(SCON, 2, n)  //Receive 8th Bit
        //#define SET_TI(n)       SET_BIT(SCON, 1, n)  //Indicates Transmit Interrupt Occurred
        //#define SET_RI(n)       SET_BIT(SCON, 0, n)  //Indicates Receive Interrupt Occurred

        //#define SET_GATE1(n)    SET_BIT(TMOD, 7, n) //Timer 1 Gate Mode
        //#define SET_CT1(n)      SET_BIT(TMOD, 6, n)  //Timer 1 Counter Mode
        //#define SET_M1_1(n)     SET_BIT(TMOD, 5, n) //Timer 1 Timer Mode Bit 1
        //#define SET_M1_0(n)     SET_BIT(TMOD, 4, n)  //Timer 1 Timer Mode Bit 0
        //#define SET_GATE0(n)    SET_BIT(TMOD, 3, n)  //Timer 0 Gate Mode
        //#define SET_CT0(n)      SET_BIT(TMOD, 2, n)  //Timer 0 Counter Mode
        //#define SET_M0_1(n)     SET_BIT(TMOD, 1, n)  //Timer 0 Timer Mode Bit 1
        //#define SET_M0_0(n)     SET_BIT(TMOD, 0, n)  //Timer 0 Timer Mode Bit 0

        /* 8052 Only flags - T2CON Flags */
        //#define SET_TF2(n)      SET_BIT(T2CON, 7, n)    //Indicated Timer 2 Overflow Int Triggered
        void SET_EXF2(uint8_t n) { var temp = T2CON; SET_BIT(ref temp, 6, n); T2CON = temp; }    //Indicates Timer 2 External Flag
        //#define SET_RCLK(n)     SET_BIT(T2CON, 5, n)    //Receive Clock
        //#define SET_TCLK(n)     SET_BIT(T2CON, 4, n)    //Transmit Clock
        //#define SET_EXEN2(n)    SET_BIT(T2CON, 3, n)    //Timer 2 External Interrupt Enable
        //#define SET_TR2(n)      SET_BIT(T2CON, 2, n)    //Indicates Timer 2 is running
        //#define SET_CT2(n)      SET_BIT(T2CON, 1, n)    //Sets Timer 2 Counter/Timer Mode
        //#define SET_CP(n)       SET_BIT(T2CON, 0, n)    //Sets Timer 2 Capture/Reload Mode

        //#define SET_GF1(n)      SET_BIT(PCON, 3, n)
        //#define SET_GF0(n)      SET_BIT(PCON, 2, n)
        //#define SET_PD(n)       SET_BIT(PCON, 1, n)
        //#define SET_IDL(n)      SET_BIT(PCON, 0, n)

        /* Macros for accessing flags */

        //#define GET_CY          GET_BIT(PSW, 7)
        //#define GET_AC          GET_BIT(PSW, 6)
        //#define GET_FO          GET_BIT(PSW, 5)
        //#define GET_RS          GET_BIT(PSW, 3)
        //#define GET_OV          GET_BIT(PSW, 2)
        //#define GET_P           GET_BIT(PSW, 0)

        //#define GET_EA          GET_BIT(IE, 7)
        //#define GET_ET2         GET_BIT(IE, 5)
        //#define GET_ES          GET_BIT(IE, 4)
        //#define GET_ET1         GET_BIT(IE, 3)
        //#define GET_EX1         GET_BIT(IE, 2)
        //#define GET_ET0         GET_BIT(IE, 1)
        //#define GET_EX0         GET_BIT(IE, 0)

        /* 8052 Only flags */
        //#define GET_PT2         GET_BIT(IP, 5)

        //#define GET_PS          GET_BIT(IP, 4)
        //#define GET_PT1         GET_BIT(IP, 3)
        //#define GET_PX1         GET_BIT(IP, 2)
        //#define GET_PT0         GET_BIT(IP, 1)
        //#define GET_PX0         GET_BIT(IP, 0)

        //#define GET_TF1         GET_BIT(TCON, 7)
        uint8_t GET_TR1 { get { return GET_BIT(TCON, 6); } }
        //#define GET_TF0         GET_BIT(TCON, 5)
        uint8_t GET_TR0 { get { return GET_BIT(TCON, 4); } }
        //#define GET_IE1         GET_BIT(TCON, 3)
        uint8_t GET_IT1 { get { return GET_BIT(TCON, 2); } }
        uint8_t GET_IE0 { get { return GET_BIT(TCON, 1); } }
        uint8_t GET_IT0 { get { return GET_BIT(TCON, 0); } }

        //#define GET_SM0         GET_BIT(SCON, 7)
        //#define GET_SM1         GET_BIT(SCON, 6)
        //#define GET_SM2         GET_BIT(SCON, 5)
        //#define GET_REN         GET_BIT(SCON, 4)
        //#define GET_TB8         GET_BIT(SCON, 3)
        //#define GET_RB8         GET_BIT(SCON, 2)
        //#define GET_TI          GET_BIT(SCON, 1)
        //#define GET_RI          GET_BIT(SCON, 0)

        //#define GET_GATE1       GET_BIT(TMOD, 7)
        //#define GET_CT1         GET_BIT(TMOD, 6)
        //#define GET_M1_1        GET_BIT(TMOD, 5)
        //#define GET_M1_0        GET_BIT(TMOD, 4)
        //#define GET_GATE0       GET_BIT(TMOD, 3)
        //#define GET_CT0         GET_BIT(TMOD, 2)
        //#define GET_M0_1        GET_BIT(TMOD, 1)
        //#define GET_M0_0        GET_BIT(TMOD, 0)

        //#define GET_SMOD        GET_BIT(PCON, 7)

        /* Only in 80C51BH & other cmos */

        //#define GET_GF1         GET_BIT(PCON, 3)
        //#define GET_GF0         GET_BIT(PCON, 2)
        //#define GET_PD          GET_BIT(PCON, 1)
        //#define GET_IDL         (GET_BIT(PCON, 0) & ~(GET_PD))  /* PD takes precedence! */

        /* 8052 Only flags */
        //#define GET_TF2         GET_BIT(T2CON, 7)
        //#define GET_EXF2        GET_BIT(T2CON, 6)
        //#define GET_RCLK        GET_BIT(T2CON, 5)
        //#define GET_TCLK        GET_BIT(T2CON, 4)
        //#define GET_EXEN2       GET_BIT(T2CON, 3)
        //#define GET_TR2         GET_BIT(T2CON, 2)
        //#define GET_CT2         GET_BIT(T2CON, 1)
        //#define GET_CP          GET_BIT(T2CON, 0)

        /* DS5002FP Only flags */

        /* PCON Flags - DS5002FP */

        //#define GET_POR         GET_BIT(PCON, 6)
        //#define GET_PFW         GET_BIT(PCON, 5)
        //#define GET_WTR         GET_BIT(PCON, 4)
        //#define GET_EPFW        GET_BIT(PCON, 3)
        //#define GET_EWT         GET_BIT(PCON, 2)

        void SET_PFW(uint8_t n) { var temp = PCON; SET_BIT(ref temp, 5, n); PCON = temp; }

        /* MCON Flags - DS5002FP */

        //#define GET_PA          ((MCON & 0xf0)>>4)
        uint8_t GET_RG1 { get { return GET_BIT(MCON, 3); } }
        //#define GET_PES         GET_BIT(MCON, 2)
        //#define GET_PM          GET_BIT(MCON, 1)
        //#define GET_SL          GET_BIT(MCON, 0)

        /* RPCTL Flags - DS5002FP */
        //#define GET_RNR         GET_BIT(RPCTL, 7) /* Bit 6 ?? */
        //#define GET_EXBS        GET_BIT(RPCTL, 5)
        //#define GET_AE          GET_BIT(RPCTL, 4)
        //#define GET_IBI         GET_BIT(RPCTL, 3)
        //#define GET_DMA         GET_BIT(RPCTL, 2)
        //#define GET_RPCON       GET_BIT(RPCTL, 1)
        uint8_t GET_RG0 { get { return GET_BIT(RPCTL, 0); } }


        /*Add and Subtract Flag settings*/
        //#define DO_ADD_FLAGS(a,d,c) do_add_flags(a, d, c)
        //#define DO_SUB_FLAGS(a,d,c) do_sub_flags(a, d, c)

        void SET_PARITY() { m_recalc_parity |= 1; }
        //#define PUSH_PC()       push_pc()
        //#define POP_PC()        pop_pc()

        /* Clear Current IRQ  */
        //#define CLEAR_CURRENT_IRQ() clear_current_irq()


        device_memory_interface_mcs51 m_dimemory;
        device_execute_interface_mcs51 m_diexec;
        device_state_interface_mcs51 m_distate;


        address_space_config m_program_config;
        address_space_config m_data_config;
        address_space_config m_io_config;

        //Internal stuff
        uint16_t m_ppc;            //previous pc
        uint16_t m_pc;             //current pc
        uint16_t m_features;       //features of this cpu
        uint8_t m_rwm;            //Signals that the current instruction is a read/write/modify instruction

        //int     m_inst_cycles;        /* cycles for the current instruction */
        readonly uint32_t m_rom_size;    /* size (in bytes) of internal program ROM/EPROM */
        int m_ram_mask;           /* second ram bank for indirect access available ? */
        int m_num_interrupts;     /* number of interrupts supported */
        int m_recalc_parity;      /* recalculate parity before next instruction */
        uint32_t m_last_line_state;    /* last state of input lines line */
        int m_t0_cnt;             /* number of 0->1 transitions on T0 line */
        int m_t1_cnt;             /* number of 0->1 transitions on T1 line */
        int m_t2_cnt;             /* number of 0->1 transitions on T2 line */
        int m_t2ex_cnt;           /* number of 0->1 transitions on T2EX line */
        int m_cur_irq_prio;       /* Holds value of the current IRQ Priority Level; -1 if no irq */
        uint8_t m_irq_active;         /* mask which irq levels are serviced */
        uint8_t [] m_irq_prio = new uint8_t [8];        /* interrupt priority */

        //uint8_t   m_forced_inputs[4];   /* allow read even if configured as output */

        // JB-related hacks
        uint8_t m_last_op;
        uint8_t m_last_bit;

        intref m_icount = new intref();  //int     m_icount;

        struct mcs51_uart
        {
            public uint8_t data_out;       //Data to send out
            public uint8_t bits_to_send;   //How many bits left to send when transmitting out the serial port
        
            public int smod_div;       /* signal divided by 2^SMOD */
            public int rx_clk;         /* rx clock */
            public int tx_clk;         /* tx clock */
            public uint8_t delay_cycles;   //Gross Hack;
        }
        mcs51_uart m_uart;            /* internal uart */

        /* Internal Ram */
        required_shared_ptr<uint8_t> m_sfr_ram;           /* 128 SFR - these are in 0x80 - 0xFF */
        required_shared_ptr<uint8_t> m_scratchpad;        /* 128 RAM (8031/51) + 128 RAM in second bank (8032/52) */


        /* Memory spaces */
        memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.cache m_program = new memory_access<int_const_16, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.cache();
        memory_access< int_const_9, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.specific m_data = new memory_access<int_const_9, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.specific();
        memory_access<int_const_17, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.specific m_io = new memory_access<int_const_17, int_const_0, int_const_0, endianness_t_const_ENDIANNESS_LITTLE>.specific();

        devcb_read8.array<u64_const_4> m_port_in_cb;
        devcb_write8.array<u64_const_4> m_port_out_cb;

        /* Serial Port TX/RX Callbacks */
        devcb_write8 m_serial_tx_cb;    //Call back function when sending data out of serial port
        devcb_read8 m_serial_rx_cb;    //Call back function to retrieve data when receiving serial port data

        /* DS5002FP */
        struct ds5002fp
        {
            public uint8_t previous_ta;        /* Previous Timed Access value */
            public uint8_t ta_window;          /* Limed Access window */
            public uint8_t range;              /* Memory Range */
            /* Bootstrap Configuration */
            public uint8_t mcon;                   /* bootstrap loader MCON register */
            public uint8_t rpctl;                  /* bootstrap loader RPCTL register */
            public uint8_t crc;                    /* bootstrap loader CRC register */
            public int32_t rnr_delay;              /* delay before new random number available */
        }
        ds5002fp m_ds5002fp;

        // for the debugger
        uint8_t m_rtemp;

        //static const uint8_t mcs51_cycles[256];


        // construction/destruction
        protected mcs51_cpu_device(machine_config mconfig, device_type type, string tag, device_t owner, uint32_t clock, int program_width, int data_width, uint8_t features = 0)
            : base(mconfig, type, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_execute_interface_mcs51(mconfig, this));
            m_class_interfaces.Add(new device_memory_interface_mcs51(mconfig, this));
            m_class_interfaces.Add(new device_state_interface_mcs51(mconfig, this));
            m_class_interfaces.Add(new device_disasm_interface_mcs51(mconfig, this));
            m_dimemory = GetClassInterface<device_memory_interface_mcs51>();
            m_diexec = GetClassInterface<device_execute_interface_mcs51>();
            m_distate = GetClassInterface<device_state_interface_mcs51>();


            m_program_config = new address_space_config("program", ENDIANNESS_LITTLE, 8, 16, 0, program_internal);
            m_data_config = new address_space_config("data", ENDIANNESS_LITTLE, 8, 9, 0, data_internal);
            m_io_config = new address_space_config("io", ENDIANNESS_LITTLE, 8, (features & FEATURE_DS5002FP) != 0 ? (uint8_t)17 : (uint8_t)16, 0);
            m_pc = 0;
            m_features = features;
            m_rom_size = program_width > 0 ? 1U << program_width : 0;
            m_ram_mask = (data_width == 8) ? 0xFF : 0x7F;
            m_num_interrupts = 5;
            m_sfr_ram = new required_shared_ptr<uint8_t>(this, "sfr_ram");
            m_scratchpad = new required_shared_ptr<uint8_t>(this, "scratchpad");
            m_port_in_cb = new devcb_read8.array<u64_const_4>(this, () => { return new devcb_read8(this); });
            m_port_out_cb = new devcb_write8.array<u64_const_4>(this, () => { return new devcb_write8(this); });
            m_serial_tx_cb = new devcb_write8(this);
            m_serial_rx_cb = new devcb_read8(this);
            m_rtemp = 0;


            m_ds5002fp.mcon = 0;
            m_ds5002fp.rpctl = 0;
            m_ds5002fp.crc = 0;

            /* default to standard cmos interfacing */
            //for (auto & elem : m_forced_inputs)
            //    elem = 0;
        }


        /* At least CMOS devices may be forced to read from ports configured as output.
         * All you need is a low impedance output connect to the port.
         */
        //void set_port_forced_input(uint8_t port, uint8_t forced_input) { m_forced_inputs[port] = forced_input; }

        //template <unsigned N> auto port_in_cb() { return m_port_in_cb[N].bind(); }
        public devcb_write8.binder port_out_cb<unsigned_N>() where unsigned_N : u32_const, new() { unsigned N = new unsigned_N().value;  return m_port_out_cb[N].bind(); }  //template <unsigned N> auto port_out_cb() { return m_port_out_cb[N].bind(); }
        //auto serial_rx_cb() { return m_serial_rx_cb.bind(); }
        //auto serial_tx_cb() { return m_serial_tx_cb.bind(); }


        void program_internal(address_map map, device_t owner)
        {
            if (m_rom_size > 0)
                map.op(0, m_rom_size - 1).rom().region(DEVICE_SELF, 0);
        }


        void data_internal(address_map map, device_t owner)
        {
            map.op(0x0000, (offs_t)m_ram_mask).ram().share("scratchpad");
            map.op(0x0100, 0x01ff).ram().share("sfr_ram"); /* SFR */
        }


        // device-level overrides
        protected override void device_start()
        {
            m_dimemory.space(AS_PROGRAM).cache(m_program);
            m_dimemory.space(AS_DATA).specific(m_data);
            m_dimemory.space(AS_IO).specific(m_io);

            m_port_in_cb.resolve_all_safe_u8(0xff);
            m_port_out_cb.resolve_all_safe();
            m_serial_rx_cb.resolve_safe_u8(0);
            m_serial_tx_cb.resolve_safe();

            /* Save states */
            save_item(NAME(new { m_ppc }));
            save_item(NAME(new { m_pc }));
            save_item(NAME(new { m_last_op }));
            save_item(NAME(new { m_last_bit }));
            save_item(NAME(new { m_rwm }));
            save_item(NAME(new { m_cur_irq_prio }));
            save_item(NAME(new { m_last_line_state }));
            save_item(NAME(new { m_t0_cnt }));
            save_item(NAME(new { m_t1_cnt }));
            save_item(NAME(new { m_t2_cnt }));
            save_item(NAME(new { m_t2ex_cnt }));
            save_item(NAME(new { m_recalc_parity }));
            save_item(NAME(new { m_irq_prio }));
            save_item(NAME(new { m_irq_active }));
            save_item(NAME(new { m_ds5002fp.previous_ta }));
            save_item(NAME(new { m_ds5002fp.ta_window }));
            save_item(NAME(new { m_ds5002fp.rnr_delay }));
            save_item(NAME(new { m_ds5002fp.range }));
            save_item(NAME(new { m_uart.data_out }));
            save_item(NAME(new { m_uart.bits_to_send }));
            save_item(NAME(new { m_uart.smod_div }));
            save_item(NAME(new { m_uart.rx_clk }));
            save_item(NAME(new { m_uart.tx_clk }));
            save_item(NAME(new { m_uart.delay_cycles }));

            m_distate.state_add( MCS51_PC,  "PC", m_pc).formatstr("%04X");
            m_distate.state_add( MCS51_SP,  "SP", SP).formatstr("%02X");
            m_distate.state_add( MCS51_PSW, "PSW", PSW).formatstr("%02X");
            m_distate.state_add( MCS51_ACC, "A", ACC).formatstr("%02X");
            m_distate.state_add( MCS51_B,   "B", B).formatstr("%02X");

            //throw new emu_unimplemented();
#if false
            state_add<uint16_t>( MCS51_DPTR, "DPTR", [this](){ return DPTR; }, [this](uint16_t dp){ SET_DPTR(dp); }).formatstr("%04X");
#endif
            m_distate.state_add( MCS51_DPH, "DPH", DPH).noshow();
            m_distate.state_add( MCS51_DPL, "DPL", DPL).noshow();
            m_distate.state_add( MCS51_IE,  "IE", IE).formatstr("%02X");
            m_distate.state_add( MCS51_IP,  "IP", IP).formatstr("%02X");

            //throw new emu_unimplemented();
#if false
            if (m_rom_size > 0)
                state_add<uint8_t>( MCS51_P0,  "P0", [this](){ return P0; }, [this](uint8_t p){ SET_P0(p); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_P1,  "P1", [this](){ return P1; }, [this](uint8_t p){ SET_P1(p); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_P2,  "P2", [this](){ return P2; }, [this](uint8_t p){ SET_P2(p); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_P3,  "P3", [this](){ return P3; }, [this](uint8_t p){ SET_P3(p); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_R0,  "R0", [this](){ return R_REG(0); }, [this](uint8_t r){ SET_REG(0, r); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_R1,  "R1", [this](){ return R_REG(1); }, [this](uint8_t r){ SET_REG(1, r); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_R2,  "R2", [this](){ return R_REG(2); }, [this](uint8_t r){ SET_REG(2, r); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_R3,  "R3", [this](){ return R_REG(3); }, [this](uint8_t r){ SET_REG(3, r); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_R4,  "R4", [this](){ return R_REG(4); }, [this](uint8_t r){ SET_REG(4, r); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_R5,  "R5", [this](){ return R_REG(5); }, [this](uint8_t r){ SET_REG(5, r); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_R6,  "R6", [this](){ return R_REG(6); }, [this](uint8_t r){ SET_REG(6, r); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_R7,  "R7", [this](){ return R_REG(7); }, [this](uint8_t r){ SET_REG(7, r); }).formatstr("%02X");
            state_add<uint8_t>( MCS51_RB,  "RB", [this](){ return (PSW & 0x18)>>3; }, [this](uint8_t rb){ SET_RS(rb); }).mask(0x03).formatstr("%02X");
#endif

            m_distate.state_add( MCS51_TCON, "TCON", TCON).formatstr("%02X");
            m_distate.state_add( MCS51_TMOD, "TMOD", TMOD).formatstr("%02X");
            m_distate.state_add( MCS51_TL0,  "TL0",  TL0).formatstr("%02X");
            m_distate.state_add( MCS51_TH0,  "TH0",  TH0).formatstr("%02X");
            m_distate.state_add( MCS51_TL1,  "TL1",  TL1).formatstr("%02X");
            m_distate.state_add( MCS51_TH1,  "TH1",  TH1).formatstr("%02X");

            m_distate.state_add( STATE_GENPC, "GENPC", m_pc ).noshow();
            m_distate.state_add( STATE_GENPCBASE, "CURPC", m_pc ).noshow();
            m_distate.state_add( STATE_GENFLAGS, "GENFLAGS", m_rtemp).formatstr("%8s").noshow();

            set_icountptr(m_icount);
        }


        /* Reset registers to the initial values */
        protected override void device_reset()
        {
            m_last_line_state = 0;
            m_t0_cnt = 0;
            m_t1_cnt = 0;
            m_t2_cnt = 0;
            m_t2ex_cnt = 0;

            /* Flag as NO IRQ in Progress */
            m_irq_active = 0;
            m_cur_irq_prio = -1;
            m_last_op = 0;
            m_last_bit = 0;

            /* these are all defined reset states */
            RWM = 0;
            PPC = PC;
            PC = 0;
            SP = 0x7;
            SET_PSW(0);
            SET_ACC(0);
            DPH = 0;
            DPL = 0;
            B = 0;
            IP = 0;
            update_irq_prio(IP, 0);
            IE = 0;
            SCON = 0;
            TCON = 0;
            TMOD = 0;
            PCON = 0;
            TH1 = 0;
            TH0 = 0;
            TL1 = 0;
            TL0 = 0;

            /* set the port configurations to all 1's */
            SET_P3(0xff);
            SET_P2(0xff);
            SET_P1(0xff);
            SET_P0(0xff);

            /* 8052 Only registers */
            if ((m_features & FEATURE_I8052) != 0)
            {
                T2CON = 0;
                RCAP2L = 0;
                RCAP2H = 0;
                TL2 = 0;
                TH2 = 0;
            }

            /* 80C52 Only registers */
            if ((m_features & FEATURE_I80C52) != 0)
            {
                IPH = 0;
                update_irq_prio(IP, IPH);
                SADDR = 0;
                SADEN = 0;
            }

            /* DS5002FP Only registers */
            if ((m_features & FEATURE_DS5002FP) != 0)
            {
                // set initial values (some of them are set using the bootstrap loader)
                PCON = 0;
                MCON = m_sfr_ram[ADDR_MCON - 0x80].op;
                RPCTL = m_sfr_ram[ADDR_RPCTL - 0x80].op;
                RPS = 0;
                RNR = 0;
                CRCR = m_sfr_ram[ADDR_CRCR - 0x80].op;
                CRCL = 0;
                CRCH = 0;
                TA = 0;

                // set internal CPU state
                m_ds5002fp.previous_ta = 0;
                m_ds5002fp.ta_window = 0;
                m_ds5002fp.range = (uint8_t)(((uint32_t)GET_RG1 << 1) | GET_RG0);
                m_ds5002fp.rnr_delay = 160;
            }

            m_uart.data_out = 0;
            m_uart.rx_clk = 0;
            m_uart.tx_clk = 0;
            m_uart.bits_to_send = 0;
            m_uart.delay_cycles = 0;
            m_uart.smod_div = 0;

            m_recalc_parity = 0;
        }


        // device_execute_interface overrides
        protected virtual uint64_t device_execute_interface_execute_clocks_to_cycles(uint64_t clocks) { return (clocks + 12 - 1) / 12; }
        protected virtual uint64_t device_execute_interface_execute_cycles_to_clocks(uint64_t cycles) { throw new emu_unimplemented(); }// { return (cycles * 12); }
        protected virtual uint32_t device_execute_interface_execute_min_cycles() { return 1; }
        protected virtual uint32_t device_execute_interface_execute_max_cycles() { throw new emu_unimplemented(); }//{ return 20; }
        protected virtual uint32_t device_execute_interface_execute_input_lines() { throw new emu_unimplemented(); }//{ return 6; }
        protected virtual void device_execute_interface_execute_run() { throw new emu_unimplemented(); }

        protected virtual void device_execute_interface_execute_set_input(int irqline, int state)
        {
            /* From the manual:
             *
             * <cite>In operation all the interrupt flags are latched into the
             * interrupt control system during State 5 of every machine cycle.
             * The samples are polled during the following machine cycle.</cite>
             *
             * ==> Since we do not emulate sub-states, this assumes that the signal is present
             * for at least one cycle (12 states)
             *
             */
            uint32_t new_state = (m_last_line_state & ~(1U << irqline)) | ((state != CLEAR_LINE) ? 1U : 0U << irqline);
            /* detect 0->1 transitions */
            uint32_t tr_state = (~m_last_line_state) & new_state;

            switch (irqline)
            {
                //External Interrupt 0
                case MCS51_INT0_LINE:
                    //Line Asserted?
                    if (state != CLEAR_LINE)
                    {
                        //Need cleared->active line transition? (Logical 1-0 Pulse on the line) - CLEAR->ASSERT Transition since INT0 active lo!
                        if (GET_IT0 != 0)
                        {
                            if (GET_BIT(tr_state, MCS51_INT0_LINE) != 0)
                                SET_IE0(1);
                        }
                        else
                        {
                            SET_IE0(1);     //Nope, just set it..
                        }
                    }
                    else
                    {
                        if (GET_IT0 == 0) /* clear if level triggered */
                            SET_IE0(0);
                    }

                    break;

                //External Interrupt 1
                case MCS51_INT1_LINE:
                    //Line Asserted?
                    if (state != CLEAR_LINE)
                    {
                        //Need cleared->active line transition? (Logical 1-0 Pulse on the line) - CLEAR->ASSERT Transition since INT1 active lo!
                        if (GET_IT1 != 0)
                        {
                            if (GET_BIT(tr_state, MCS51_INT1_LINE) != 0)
                                SET_IE1(1);
                        }
                        else
                            SET_IE1(1);     //Nope, just set it..
                    }
                    else
                    {
                        if (GET_IT1 == 0) /* clear if level triggered */
                            SET_IE1(0);
                    }
                    break;

                case MCS51_T0_LINE:
                    if (GET_BIT(tr_state, MCS51_T0_LINE) != 0 && GET_TR0 != 0)
                        m_t0_cnt++;
                    break;

                case MCS51_T1_LINE:
                    if (GET_BIT(tr_state, MCS51_T1_LINE) != 0 && GET_TR1 != 0)
                        m_t1_cnt++;
                    break;

                case MCS51_T2_LINE:
                    if ((m_features & FEATURE_I8052) != 0)
                    {
                        if (GET_BIT(tr_state, MCS51_T2_LINE) != 0 && GET_TR1 != 0)
                            m_t2_cnt++;
                    }
                    else
                        fatalerror("mcs51: Trying to set T2_LINE on a non I8052 type cpu.\n");
                    break;

                case MCS51_T2EX_LINE:
                    if ((m_features & FEATURE_I8052) != 0)
                    {
                        if (GET_BIT(tr_state, MCS51_T2EX_LINE) != 0)
                        {
                            SET_EXF2(1);
                            m_t2ex_cnt++;
                        }
                    }
                    else
                        fatalerror("mcs51: Trying to set T2EX_LINE on a non I8052 type cpu.\n");
                    break;

                case MCS51_RX_LINE: /* Serial Port Receive */
                    /* Is the enable flags for this interrupt set? */
                    if (state != CLEAR_LINE)
                    {
                        serial_receive();
                    }
                    break;

                /* Power Fail Interrupt */
                case DS5002FP_PFI_LINE:
                    if ((m_features & FEATURE_DS5002FP) != 0)
                    {
                        /* Need cleared->active line transition? (Logical 1-0 Pulse on the line) - CLEAR->ASSERT Transition since INT1 active lo! */
                        if (GET_BIT(tr_state, MCS51_INT1_LINE) != 0)
                            SET_PFW(1);
                    }
                    else
                        fatalerror("mcs51: Trying to set DS5002FP_PFI_LINE on a non DS5002FP type cpu.\n");
                    break;
            }

            m_last_line_state = new_state;
        }



        // device_memory_interface overrides
        protected virtual space_config_vector device_memory_interface_memory_space_config()
        {
            return new space_config_vector
            {
                std.make_pair(AS_PROGRAM, m_program_config),
                std.make_pair(AS_DATA,    m_data_config),
                std.make_pair(AS_IO,      m_io_config)
            };
        }



        // device_state_interface overrides
        protected virtual void device_state_interface_state_string_export(device_state_entry entry, string str) { throw new emu_unimplemented(); }


        // device_disasm_interface overrides
        //virtual std::unique_ptr<util::disasm_interface> create_disassembler() override;


        /* SFR Callbacks */
        protected virtual void sfr_write(size_t offset, uint8_t data)
        {
            /* update register */
            assert(offset >= 0x80 && offset <= 0xff);

            switch (offset)
            {
                case ADDR_P0:   m_port_out_cb[0].op_u8(data);             break;
                case ADDR_P1:   m_port_out_cb[1].op_u8(data);             break;
                case ADDR_P2:   m_port_out_cb[2].op_u8(data);             break;
                case ADDR_P3:   m_port_out_cb[3].op_u8(data);             break;
                case ADDR_SBUF: serial_transmit(data);         break;
                case ADDR_PSW:  SET_PARITY();                       break;
                case ADDR_ACC:  SET_PARITY();                       break;
                case ADDR_IP:   update_irq_prio(data, 0);  break;
                /* R_SBUF = data;        //This register is used only for "Receiving data coming in!" */

                case ADDR_B:
                case ADDR_SP:
                case ADDR_DPL:
                case ADDR_DPH:
                case ADDR_PCON:
                case ADDR_TCON:
                case ADDR_TMOD:
                case ADDR_IE:
                case ADDR_TL0:
                case ADDR_TL1:
                case ADDR_TH0:
                case ADDR_TH1:
                case ADDR_SCON:
                    break;

                default:
                    LOG("mcs51 '{0}': attemping to write to an invalid/non-implemented SFR address: {1} at 0x{2}, data={3}\n", tag(), (uint32_t)offset, PC, data);
                    /* no write in this case according to manual */
                    return;
            }

            m_data.write_byte((offs_t)((size_t)offset | 0x100), data);
        }


        protected virtual uint8_t sfr_read(size_t offset) { throw new emu_unimplemented(); }


        //uint8_t iram_iread(offs_t a);
        //void iram_iwrite(offs_t a, uint8_t d);
        //void clear_current_irq();
        //uint8_t r_acc();
        //uint8_t r_psw();
        //offs_t external_ram_iaddr(offs_t offset, offs_t mem_mask);
        //uint8_t iram_read(size_t offset);


        void iram_write(size_t offset, uint8_t data)
        {
            if (offset < 0x80)
                m_data.write_byte((offs_t)offset, data);
            else
                sfr_write(offset, data);
        }


        //void push_pc();
        //void pop_pc();
        //void set_parity();
        //uint8_t bit_address_r(uint8_t offset);
        //void bit_address_w(uint8_t offset, uint8_t bit);
        //void do_add_flags(uint8_t a, uint8_t data, uint8_t c);
        //void do_sub_flags(uint8_t a, uint8_t data, uint8_t c);
        //void transmit_receive(int source);
        //void update_timer_t0(int cycles);
        //void update_timer_t1(int cycles);
        //void update_timer_t2(int cycles);
        //void update_timers(int cycles);
        void serial_transmit(uint8_t data) { throw new emu_unimplemented(); }
        void serial_receive() { throw new emu_unimplemented(); }
        //void update_serial(int cycles);


        /* Check and update status of serial port */
        void update_irq_prio(uint8_t ipl, uint8_t iph)
        {
            for (int i = 0; i < 8; i++)
                m_irq_prio[i] = (uint8_t)((((uint32_t)ipl >> i) & 1) | ((((uint32_t)iph >> i) & 1) << 1));
        }


        //void execute_op(uint8_t op);
        //void check_irqs();
        //void burn_cycles(int cycles);
        //void acall(uint8_t r);
        //void add_a_byte(uint8_t r);
        //void add_a_mem(uint8_t r);
        //void add_a_ir(uint8_t r);
        //void add_a_r(uint8_t r);
        //void addc_a_byte(uint8_t r);
        //void addc_a_mem(uint8_t r);
        //void addc_a_ir(uint8_t r);
        //void addc_a_r(uint8_t r);
        //void ajmp(uint8_t r);
        //void anl_mem_a(uint8_t r);
        //void anl_mem_byte(uint8_t r);
        //void anl_a_byte(uint8_t r);
        //void anl_a_mem(uint8_t r);
        //void anl_a_ir(uint8_t r);
        //void anl_a_r(uint8_t r);
        //void anl_c_bitaddr(uint8_t r);
        //void anl_c_nbitaddr(uint8_t r);
        //void cjne_a_byte(uint8_t r);
        //void cjne_a_mem(uint8_t r);
        //void cjne_ir_byte(uint8_t r);
        //void cjne_r_byte(uint8_t r);
        //void clr_bitaddr(uint8_t r);
        //void clr_c(uint8_t r);
        //void clr_a(uint8_t r);
        //void cpl_bitaddr(uint8_t r);
        //void cpl_c(uint8_t r);
        //void cpl_a(uint8_t r);
        //void da_a(uint8_t r);
        //void dec_a(uint8_t r);
        //void dec_mem(uint8_t r);
        //void dec_ir(uint8_t r);
        //void dec_r(uint8_t r);
        //void div_ab(uint8_t r);
        //void djnz_mem(uint8_t r);
        //void djnz_r(uint8_t r);
        //void inc_a(uint8_t r);
        //void inc_mem(uint8_t r);
        //void inc_ir(uint8_t r);
        //void inc_r(uint8_t r);
        //void inc_dptr(uint8_t r);
        //void jb(uint8_t r);
        //void jbc(uint8_t r);
        //void jc(uint8_t r);
        //void jmp_iadptr(uint8_t r);
        //void jnb(uint8_t r);
        //void jnc(uint8_t r);
        //void jnz(uint8_t r);
        //void jz(uint8_t r);
        //void lcall(uint8_t r);
        //void ljmp(uint8_t r);
        //void mov_a_byte(uint8_t r);
        //void mov_a_mem(uint8_t r);
        //void mov_a_ir(uint8_t r);
        //void mov_a_r(uint8_t r);
        //void mov_mem_byte(uint8_t r);
        //void mov_mem_mem(uint8_t r);
        //void mov_ir_byte(uint8_t r);
        //void mov_r_byte(uint8_t r);
        //void mov_mem_ir(uint8_t r);
        //void mov_mem_r(uint8_t r);
        //void mov_dptr_byte(uint8_t r);
        //void mov_bitaddr_c(uint8_t r);
        //void mov_ir_mem(uint8_t r);
        //void mov_r_mem(uint8_t r);
        //void mov_mem_a(uint8_t r);
        //void mov_ir_a(uint8_t r);
        //void mov_r_a(uint8_t r);
        //void movc_a_iapc(uint8_t r);
        //void mov_c_bitaddr(uint8_t r);
        //void movc_a_iadptr(uint8_t r);
        //void movx_a_idptr(uint8_t r);
        //void movx_a_ir(uint8_t r);
        //void movx_idptr_a(uint8_t r);
        //void movx_ir_a(uint8_t r);
        //void mul_ab(uint8_t r);
        //void nop(uint8_t r);
        //void orl_mem_a(uint8_t r);
        //void orl_mem_byte(uint8_t r);
        //void orl_a_byte(uint8_t r);
        //void orl_a_mem(uint8_t r);
        //void orl_a_ir(uint8_t r);
        //void orl_a_r(uint8_t r);
        //void orl_c_bitaddr(uint8_t r);
        //void orl_c_nbitaddr(uint8_t r);
        //void pop(uint8_t r);
        //void push(uint8_t r);
        //void ret(uint8_t r);
        //void reti(uint8_t r);
        //void rl_a(uint8_t r);
        //void rlc_a(uint8_t r);
        //void rr_a(uint8_t r);
        //void rrc_a(uint8_t r);
        //void setb_c(uint8_t r);
        //void setb_bitaddr(uint8_t r);
        //void sjmp(uint8_t r);
        //void subb_a_byte(uint8_t r);
        //void subb_a_mem(uint8_t r);
        //void subb_a_ir(uint8_t r);
        //void subb_a_r(uint8_t r);
        //void swap_a(uint8_t r);
        //void xch_a_mem(uint8_t r);
        //void xch_a_ir(uint8_t r);
        //void xch_a_r(uint8_t r);
        //void xchd_a_ir(uint8_t r);
        //void xrl_mem_a(uint8_t r);
        //void xrl_mem_byte(uint8_t r);
        //void xrl_a_byte(uint8_t r);
        //void xrl_a_mem(uint8_t r);
        //void xrl_a_ir(uint8_t r);
        //void xrl_a_r(uint8_t r);
        //void illegal(uint8_t r);
        //uint8_t ds5002fp_protected(size_t offset, uint8_t data, uint8_t ta_mask, uint8_t mask);
    }


    //class i8031_device : public mcs51_cpu_device

    //class i8051_device : public mcs51_cpu_device


    public class i8751_device : mcs51_cpu_device
    {
        //DEFINE_DEVICE_TYPE(I8751, i8751_device, "i8751", "Intel 8751")
        public static readonly emu.detail.device_type_impl I8751 = DEFINE_DEVICE_TYPE("i8751", "Intel 8751", (type, mconfig, tag, owner, clock) => { return new i8751_device(mconfig, tag, owner, clock); });


        // construction/destruction
        i8751_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, I8751, tag, owner, clock, 12, 7)
        { }
    }


    //class am8753_device : public mcs51_cpu_device

    //class i8052_device : public mcs51_cpu_device

    //class i8032_device : public i8052_device

    //class i8752_device : public i8052_device

    //class i80c31_device : public i8052_device

    //class i80c51_device : public mcs51_cpu_device

    //class i87c51_device : public i80c51_device

    //class i80c52_device : public i8052_device

    //class i80c32_device : public i80c52_device

    //class i87c52_device : public i80c52_device

    //class i87c51fa_device : public i80c52_device

    //class i80c51gb_device : public i87c51fa_device

    //class at89c52_device : public i80c52_device

    //class at89s52_device : public i80c52_device

    //class at89c4051_device : public i80c51_device

    //class ds80c320_device : public i80c52_device

    //class sab80c535_device : public i80c51_device

    //class i8344_device : public mcs51_cpu_device

    //class i8744_device : public mcs51_cpu_device


    /* these allow the default state of RAM to be set from a region */
    //#define DS5002FP_SET_MON( _mcon) \
    //    ROM_FILL( 0xc6, 1, _mcon)

    //#define DS5002FP_SET_RPCTL( _rpctl) \
    //    ROM_FILL( 0xd8, 1, _rpctl)

    //#define DS5002FP_SET_CRCR( _crcr) \
    //    ROM_FILL( 0xc1, 1, _crcr)


    //class ds5002fp_device : public mcs51_cpu_device, public device_nvram_interface


    static class mcs51_internal
    {
        //enum
        //{
            const uint8_t FEATURE_NONE            = 0x00;
            public const uint8_t FEATURE_I8052           = 0x01;
            const uint8_t FEATURE_CMOS            = 0x02;
            public const uint8_t FEATURE_I80C52          = 0x04;
            public const uint8_t FEATURE_DS5002FP        = 0x08;
        //};


        /* Internal address in SFR of registers */
        //enum
        //{
            public const uint8_t ADDR_PSW    = 0xd0;
            public const uint8_t ADDR_ACC    = 0xe0;
            public const uint8_t ADDR_B      = 0xf0;

            public const uint8_t ADDR_P0     = 0x80;
            public const uint8_t ADDR_SP     = 0x81;
            public const uint8_t ADDR_DPL    = 0x82;
            public const uint8_t ADDR_DPH    = 0x83;
            public const uint8_t ADDR_PCON   = 0x87;
            public const uint8_t ADDR_TCON   = 0x88;
            public const uint8_t ADDR_TMOD   = 0x89;
            public const uint8_t ADDR_TL0    = 0x8a;
            public const uint8_t ADDR_TL1    = 0x8b;
            public const uint8_t ADDR_TH0    = 0x8c;
            public const uint8_t ADDR_TH1    = 0x8d;
            public const uint8_t ADDR_P1     = 0x90;
            public const uint8_t ADDR_SCON   = 0x98;
            public const uint8_t ADDR_SBUF   = 0x99;
            public const uint8_t ADDR_P2     = 0xa0;
            public const uint8_t ADDR_IE     = 0xa8;
            public const uint8_t ADDR_P3     = 0xb0;
            public const uint8_t ADDR_IP     = 0xb8;

            /* 8052 Only registers */
            public const uint8_t ADDR_T2CON  = 0xc8;
            public const uint8_t ADDR_RCAP2L = 0xca;
            public const uint8_t ADDR_RCAP2H = 0xcb;
            public const uint8_t ADDR_TL2    = 0xcc;
            public const uint8_t ADDR_TH2    = 0xcd;

            /* 80C52 Only registers */
            public const uint8_t ADDR_IPH    = 0xb7;
            public const uint8_t ADDR_SADDR  = 0xa9;
            public const uint8_t ADDR_SADEN  = 0xb9;

            /* Philips 80C52 */
            //ADDR_AUXR   = 0x8e,
            //ADDR_AUXR1  = 0xa2,

            /* DS5002FP */
            public const uint8_t ADDR_CRCR   = 0xc1;
            public const uint8_t ADDR_CRCL   = 0xc2;
            public const uint8_t ADDR_CRCH   = 0xc3;
            public const uint8_t ADDR_MCON   = 0xc6;
            public const uint8_t ADDR_TA     = 0xc7;
            public const uint8_t ADDR_RNR    = 0xcf;
            public const uint8_t ADDR_RPCTL  = 0xd8;
            public const uint8_t ADDR_RPS    = 0xda;
        //};
    }


    static partial class mcs51_global
    {
        public static i8751_device I8751<bool_Required>(machine_config mconfig, device_finder<i8751_device, bool_Required> finder, XTAL clock) where bool_Required : bool_const, new() { return emu.detail.device_type_impl.op(mconfig, finder, i8751_device.I8751, clock); }
    }
}
