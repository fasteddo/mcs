// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int16_t = System.Int16;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;


namespace mame
{
    public partial class m6805_base_device : cpu_device
    {
        u32 SP_MASK { get { return m_params.m_sp_mask; } }  // stack pointer mask
        u32 SP_LOW { get { return m_params.m_sp_floor; } }  // stack pointer low water mark
        u16 PC { get { return m_pc.w.l; } set { m_pc.w.l = value; } }            // program counter lower word
        u16 S { get { return m_s.w.l; } set { m_s.w.l = value; } }             // stack pointer lower word
        u8 A { get { return m_a; } set { m_a = value; } }                 // accumulator
        u8 X { get { return m_x; } set { m_x = value; } }                 // index register
        protected u8 CC { get { return m_cc; } set { m_cc = value; } }                // condition codes

        u32 EAD { get { return m_ea.d; } set { m_ea.d = value; } }
        u16 EA { get { return m_ea.w.l; } set { m_ea.w.l = value; } }


        // pre-clear a PAIR union; clearing h2 and h3 only might be faster?
        void clear_pair(ref PAIR p) { p.d = 0; }

        /* macros to tweak the PC and SP */
        void SP_INC() { if (++S > SP_MASK) S = (u16)SP_LOW; }
        void SP_DEC() { if (--S < SP_LOW) S = (u16)SP_MASK; }
        u16 SP_ADJUST(u16 s) { return (u16)((s & SP_MASK) | SP_LOW); }


        protected void rm16(u32 addr, ref PAIR p)
        {
            clear_pair(ref p);
            p.b.h = (u8)rm(addr);
            p.b.l = (u8)rm(addr + 1);
        }

        protected void pushbyte(u8 b)
        {
            wm(S, b);
            SP_DEC();
        }

        protected void pushword(PAIR p)
        {
            pushbyte(p.b.l);
            pushbyte(p.b.h);
        }

        void pullbyte(out u8 b)
        {
            SP_INC();
            b = (u8)rm(S);
        }

        void pullword(ref PAIR p)
        {
            clear_pair(ref p);
            pullbyte(out p.b.h);
            pullbyte(out p.b.l);
        }


        /* macros to access memory */
        void immbyte(out u8 b) { b = (u8)rdop_arg(PC++); }  //template <typename T> inline void m6805_base_device::immbyte(T &b) { b = rdop_arg(PC++); }
        void immbyte(out u16 b) { b = (u16)rdop_arg(PC++); }  //template <typename T> inline void m6805_base_device::immbyte(T &b) { b = rdop_arg(PC++); }
        void immword(ref PAIR w) { w.d = 0; immbyte(out w.b.h); immbyte(out w.b.l); }
        //inline void m6805_base_device::skipbyte() { rdop_arg(PC++); }


        /* for treating an unsigned uint8_t as a signed int16_t */
        int16_t SIGNED(uint8_t b) { return (int16_t)((b & 0x80) != 0 ? b | 0xff00 : b); }  //#define SIGNED(b) (int16_t(b & 0x80 ? b | 0xff00 : b))


        void DIRECT() { EAD = 0; immbyte(out m_ea.b.l); }
        //#define IMM8 do { EA = PC++; } while (false)
        void EXTENDED() { immword(ref m_ea); }
        void INDEXED() { EA = X; }
        void INDEXED1() { EAD = 0; immbyte(out m_ea.b.l); EA += X; }
        void INDEXED2() { immword(ref m_ea); EA += X; }


        /* macros to set status flags */
        //#if defined(SEC)
        //#undef SEC
        //#endif
        void SEC() { CC |= CFLAG; }
        void CLC() { CC = (u8)(CC & ~CFLAG); }
        void SEZ() { CC |= ZFLAG; }
        void CLZ() { CC = (u8)(CC & ~ZFLAG); }
        void SEN() { CC |= NFLAG; }
        void CLN() { CC = (u8)(CC & ~NFLAG); }
        void SEH() { CC |= HFLAG; }
        void CLH() { CC = (u8)(CC & ~HFLAG); }
        protected void SEI() { CC |= IFLAG; }
        void CLI() { CC = (u8)(CC & ~IFLAG); }


        /* macros for convenience */
        void ARGADDR(addr_mode M)
        {
            switch (M)
            {
                case addr_mode.IM: static_assert(addr_mode.IM != M, "invalid mode for this instruction"); break;
                case addr_mode.DI: DIRECT(); break;
                case addr_mode.EX: EXTENDED(); break;
                case addr_mode.IX: INDEXED(); break;
                case addr_mode.IX1: INDEXED1(); break;
                case addr_mode.IX2: INDEXED2(); break;
                default: break;
            }
        }
        void DIRBYTE(out u8 b) { DIRECT(); b = (u8)rm(EAD); }
        void DIRBYTE(out u16 b) { DIRECT(); b = (u16)rm(EAD); }
        void EXTBYTE(out u8 b) { EXTENDED(); b = (u8)rm(EAD); }
        void EXTBYTE(out u16 b) { EXTENDED(); b = (u16)rm(EAD); }
        void IDXBYTE(out u8 b) { INDEXED(); b = (u8)rm(EAD); }
        void IDXBYTE(out u16 b) { INDEXED(); b = (u16)rm(EAD); }
        void IDX1BYTE(out u8 b) { INDEXED1(); b = (u8)rm(EAD); }
        void IDX1BYTE(out u16 b) { INDEXED1(); b = (u16)rm(EAD); }
        void IDX2BYTE(out u8 b) { INDEXED2(); b = (u8)rm(EAD); }
        void IDX2BYTE(out u16 b) { INDEXED2(); b = (u16)rm(EAD); }
        void ARGBYTE(addr_mode M, out u8 b)
        {
            switch (M)
            {
                case addr_mode.IM: immbyte(out b); break;
                case addr_mode.DI: DIRBYTE(out b); break;
                case addr_mode.EX: EXTBYTE(out b); break;
                case addr_mode.IX: IDXBYTE(out b); break;
                case addr_mode.IX1: IDX1BYTE(out b); break;
                case addr_mode.IX2: IDX2BYTE(out b); break;
                default: b = 0; break;
            }
        }
        void ARGBYTE(addr_mode M, out u16 b)
        {
            switch (M)
            {
                case addr_mode.IM: immbyte(out b); break;
                case addr_mode.DI: DIRBYTE(out b); break;
                case addr_mode.EX: EXTBYTE(out b); break;
                case addr_mode.IX: IDXBYTE(out b); break;
                case addr_mode.IX1: IDX1BYTE(out b); break;
                case addr_mode.IX2: IDX2BYTE(out b); break;
                default: b = 0; break;
            }
        }


        /* Macros for branch instructions */
        void BRANCH(bool C, bool f) { u8 t; immbyte(out t); if ((bool)f == (bool)C) PC = (u16)(PC + SIGNED(t)); }
    }
}
