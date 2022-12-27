// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int8_t = System.SByte;
using int16_t = System.Int16;
using PointerU8 = mame.Pointer<System.Byte>;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.util;


namespace mame
{
    public partial class z8002_device : cpu_device
    {
        PointerU8 RB(int n) { return new PointerU8(m_regs.B, (int)BYTE8_XOR_BE(((n & 7) << 1) | ((n & 8) >> 3))); }
        PointerU16 RW(int n) { return new PointerU16(m_regs.W, BYTE4_XOR_BE(n)); }  //#define RW(n)   m_regs.W[BYTE4_XOR_BE(n)]
        PointerU32 RL(int n) { return new PointerU32(m_regs.L, BYTE_XOR_BE((n) >> 1)); }
        PointerU64 RQ(int n) { return new PointerU64(m_regs.Q, n >> 2); }

        /* the register used as stack pointer */
        uint8_t SP { get { return get_segmented_mode() ? (uint8_t)14 : (uint8_t)15; } }

        /* these vectors are based on m_psap */
        //#define RST     (PSA_ADDR() + 0)  /* start up m_fcw and m_pc */
        uint32_t EPU     { get { return PSA_ADDR() + (uint32_t)m_vector_mult * 0x0004; } }  /* extension processor unit? trap */
        uint32_t TRAP    { get { return PSA_ADDR() + (uint32_t)m_vector_mult * 0x0008; } }  /* privilege violation trap */
        uint32_t SYSCALL { get { return PSA_ADDR() + (uint32_t)m_vector_mult * 0x000c; } }  /* system call SC */
        uint32_t SEGTRAP { get { return PSA_ADDR() + (uint32_t)m_vector_mult * 0x0010; } }  /* segment trap */
        uint32_t NMI     { get { return PSA_ADDR() + (uint32_t)m_vector_mult * 0x0014; } }  /* non maskable interrupt */
        uint32_t NVI     { get { return PSA_ADDR() + (uint32_t)m_vector_mult * 0x0018; } }  /* non vectored interrupt */
        uint32_t VI      { get { return PSA_ADDR() + (uint32_t)m_vector_mult * 0x001c; } }  /* vectored interrupt */
        uint32_t VEC00   { get { return PSA_ADDR() + (uint32_t)m_vector_mult * 0x001e; } }  /* vector n m_pc value */

        /* bits of the m_fcw */
        const uint16_t F_SEG   = 0x8000;              /* segmented mode (Z8001 only) */
        const uint16_t F_S_N   = 0x4000;              /* system / normal mode */
        const uint16_t F_EPU   = 0x2000;              /* extension processor unit? */
        const uint16_t F_VIE   = 0x1000;              /* vectored interrupt enable */
        const uint16_t F_NVIE  = 0x0800;              /* non vectored interrupt enable */
        //#define F_10    0x0400              /* unused */
        //#define F_9     0x0200              /* unused */
        //#define F_8     0x0100              /* unused */
        const uint16_t F_C     = 0x0080;              /* carry flag */
        const uint16_t F_Z     = 0x0040;              /* zero flag */
        const uint16_t F_S     = 0x0020;              /* sign flag */
        const uint16_t F_PV    = 0x0010;              /* parity/overflow flag */
        const uint16_t F_DA    = 0x0008;              /* decimal adjust flag (0 add/adc, 1 sub/sbc) */
        const uint16_t F_H     = 0x0004;              /* half carry flag (byte arithmetic only) */
        //#define F_1     0x0002              /* unused */
        //#define F_0     0x0001              /* unused */

        /* opcode word numbers in m_op[] array */
        const int OP0     = 0;
        const int OP1     = 1;
        const int OP2     = 2;

        /* nibble shift factors for an opcode word */
        /* left to right: 0x1340 -> NIB0=1, NIB1=3, NIB2=4, NIB3=0 */
        //#define NIB0    12
        const int NIB1    = 8;
        const int NIB2    = 4;
        const int NIB3    = 0;

        /* sign bit masks for byte, word and long */
        const uint8_t S08 = 0x80;
        const uint16_t S16 = 0x8000;
        const uint32_t S32 = 0x80000000;

        /* get a single flag bit 0/1 */
        uint16_t GET_C { get { return (uint16_t)((m_fcw >> 7) & 1); } }
        uint16_t GET_Z { get { return (uint16_t)((m_fcw >> 6) & 1); } }
        uint16_t GET_S { get { return (uint16_t)((m_fcw >> 5) & 1); } }
        uint16_t GET_PV { get { return (uint16_t)((m_fcw >> 4) & 1); } }
        //#define GET_DA      ((m_fcw >> 3) & 1)
        //#define GET_H       ((m_fcw >> 2) & 1)

        /* clear a single flag bit */
        //#define CLR_C       m_fcw &= ~F_C
        void CLR_Z() { m_fcw &= unchecked((uint16_t)~F_Z); }
        void CLR_S() { m_fcw &= unchecked((uint16_t)~F_S); }
        //#define CLR_P       m_fcw &= ~F_PV
        void CLR_V() { m_fcw &= unchecked((uint16_t)~F_PV); }
        void CLR_DA() { m_fcw &= unchecked((uint16_t)~F_DA); }
        //#define CLR_H       m_fcw &= ~F_H

        /* clear a flag bit combination */
        void CLR_CZS() { m_fcw &= unchecked((uint16_t)~(F_C|F_Z|F_S)); }
        //#define CLR_CZSP    m_fcw &= ~(F_C|F_Z|F_S|F_PV)
        void CLR_CZSV() { m_fcw &= unchecked((uint16_t)~(F_C|F_Z|F_S|F_PV)); }
        void CLR_CZSVH() { m_fcw &= unchecked((uint16_t)~(F_C|F_Z|F_S|F_PV|F_H)); }
        void CLR_ZS() { m_fcw &= unchecked((uint16_t)~(F_Z|F_S)); }
        void CLR_ZSV() { m_fcw &= unchecked((uint16_t)~(F_Z|F_S|F_PV)); }
        void CLR_ZSP() { m_fcw &= unchecked((uint16_t)~(F_Z|F_S|F_PV)); }

        /* set a single flag bit */
        void SET_C() { m_fcw |= F_C; }
        void SET_Z() { m_fcw |= F_Z; }
        void SET_S() { m_fcw |= F_S; }
        //#define SET_P       m_fcw |= F_PV
        void SET_V() { m_fcw |= F_PV; }
        void SET_DA() { m_fcw |= F_DA; }
        void SET_H() { m_fcw |= F_H; }

        /* set a flag bit combination */
        void SET_SC() { m_fcw |= F_C | F_S; }

        /* check condition codes */
        bool CC0 { get { return false; } }                         /* always false */
        bool CC1 { get { return (GET_PV ^ GET_S) != 0; } }              /* less than */
        bool CC2 { get { return (GET_Z | (GET_PV ^ GET_S)) != 0; } }      /* less than or equal */
        bool CC3 { get { return (GET_Z | GET_C) != 0; } }               /* unsigned less than or equal */
        bool CC4 { get { return GET_PV != 0; } }                      /* parity even / overflow */
        bool CC5 { get { return GET_S != 0; } }                       /* minus (signed) */
        bool CC6 { get { return GET_Z != 0; } }                       /* zero / equal */
        bool CC7 { get { return GET_C != 0; } }                       /* carry / unsigned less than */

        bool CC8 { get { return true; } }                         /* always true */
        bool CC9 { get { return (GET_PV ^ GET_S) == 0; } }             /* greater than or equal */
        bool CCA { get { return (GET_Z | (GET_PV ^ GET_S)) == 0; } }     /* greater than */
        bool CCB { get { return (GET_Z | GET_C) == 0; } }              /* unsigned greater than */
        bool CCC { get { return GET_PV == 0; } }                     /* parity odd / no overflow */
        bool CCD { get { return GET_S == 0; } }                      /* plus (not signed) */
        bool CCE { get { return GET_Z == 0; } }                      /* not zero / not equal */
        bool CCF { get { return GET_C == 0; } }                      /* not carry / unsigned greater than */

        /* get data from the opcode words */
        /* o is the opcode word offset    */
        /* s is a nibble shift factor     */
        uint16_t GET_BIT(int o) { return (uint16_t)(1U << (int)(get_operand(o) & 15)); }
        uint8_t GET_CCC(int o, int s) { return (uint8_t)((get_operand(o) >> s) & 15); }

        uint8_t GET_DST(int o, int s) { return (uint8_t)((get_operand(o) >> s) & 15); }
        uint8_t GET_SRC(int o, int s) { return (uint8_t)((get_operand(o) >> s) & 15); }
        uint8_t GET_IDX(int o, int s) { return (uint8_t)((get_operand(o) >> s) & 15); }
        int8_t GET_CNT(int o, int s) { return (int8_t)((get_operand(o) >> s) & 15); }
        uint8_t GET_IMM4(int o, int s) { return (uint8_t)((get_operand(o) >> s) & 15); }

        uint8_t GET_I4M1(int o, int s) { return (uint8_t)(((get_operand(o) >> s) & 15) + 1); }
        uint8_t GET_IMM1(int o, int s) { return (uint8_t)((get_operand(o) >> s) & 2); }
        uint8_t GET_IMM2(int o, int s) { return (uint8_t)((get_operand(o) >> s) & 3); }
        uint8_t GET_IMM3(int o, int s) { return (uint8_t)((get_operand(o) >> s) & 7); }

        uint8_t GET_IMM8(int o) { return (uint8_t)get_operand(o); }

        // Be very careful with order of operations since get_operand has side effects
        uint16_t GET_IMM16(int o) { return (uint16_t)get_operand(o); }
        uint32_t GET_IDX16(int o) { return get_operand(o); }
        uint32_t GET_IMM32 { get { uint32_t imm32 = get_operand(1); return (imm32 << 16) + get_operand(2); } }
        uint8_t GET_DSP7 { get { return (uint8_t)(get_operand(0) & 127); } }
        int8_t GET_DSP8 { get { return (int8_t)get_operand(0); } }
        uint32_t GET_DSP16 { get { uint16_t tmp16 = (uint16_t)get_operand(1); return addr_add(m_pc, (uint32_t)(int16_t)tmp16); } }
        uint32_t GET_ADDR(int o) { return (uint32_t)get_addr_operand(o); }
        uint32_t GET_ADDR_RAW(int o) { return (uint32_t)get_raw_addr_operand(o); }
    }
}
