// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int8_t = System.SByte;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    public partial class t11_device : cpu_device
    {
        abstract class register
        {
            protected t11_device m_owner;
            protected register(t11_device owner) { m_owner = owner; }
            public abstract void MAKE_EAW_(int r);
            public abstract void GET_SB_(uint16_t op);
            public abstract void GET_SW_(uint16_t op);
            public abstract void GET_DB_(uint16_t op);
            public abstract void GET_DW_(uint16_t op);
            public abstract void PUT_DB_(uint16_t op, int v);
            public abstract void PUT_DBT_(uint16_t op, int v);
            public abstract void PUT_DW_(uint16_t op, int v);
            public abstract void PUT_DWT_(uint16_t op, int v);
        }
        class register_RG : register
        {
            public register_RG(t11_device owner) : base(owner) { }
            public override void MAKE_EAW_(int r) { throw new emu_unimplemented(); }
            public override void GET_SB_(uint16_t op) { m_owner.GET_SB_RG(op); }
            public override void GET_SW_(uint16_t op) { m_owner.GET_SW_RG(op); }
            public override void GET_DB_(uint16_t op) { m_owner.GET_DB_RG(op); }
            public override void GET_DW_(uint16_t op) { m_owner.GET_DW_RG(op); }
            public override void PUT_DB_(uint16_t op, int v) { m_owner.PUT_DB_RG(op, v); }
            public override void PUT_DBT_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DW_(uint16_t op, int v) { m_owner.PUT_DW_RG(op, v); }
            public override void PUT_DWT_(uint16_t op, int v) { m_owner.PUT_DWT_RG(op, v); }
        }
        class register_RGD : register
        {
            public register_RGD(t11_device owner) : base(owner) { }
            public override void MAKE_EAW_(int r) { m_owner.MAKE_EAW_RGD(r); }
            public override void GET_SB_(uint16_t op) { m_owner.GET_SB_RGD(op); }
            public override void GET_SW_(uint16_t op) { m_owner.GET_SW_RGD(op); }
            public override void GET_DB_(uint16_t op) { m_owner.GET_DB_RGD(op); }
            public override void GET_DW_(uint16_t op) { m_owner.GET_DW_RGD(op); }
            public override void PUT_DB_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DBT_(uint16_t op, int v) { m_owner.PUT_DBT_RGD(op, v); }
            public override void PUT_DW_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DWT_(uint16_t op, int v) { m_owner.PUT_DWT_RGD(op, v); }
        }
        class register_IN : register
        {
            public register_IN(t11_device owner) : base(owner) { }
            public override void MAKE_EAW_(int r) { m_owner.MAKE_EAW_IN(r); }
            public override void GET_SB_(uint16_t op) { m_owner.GET_SB_IN(op); }
            public override void GET_SW_(uint16_t op) { m_owner.GET_SW_IN(op); }
            public override void GET_DB_(uint16_t op) { m_owner.GET_DB_IN(op); }
            public override void GET_DW_(uint16_t op) { m_owner.GET_DW_IN(op); }
            public override void PUT_DB_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DBT_(uint16_t op, int v) { m_owner.PUT_DBT_IN(op, v); }
            public override void PUT_DW_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DWT_(uint16_t op, int v) { m_owner.PUT_DWT_IN(op, v); }
        }
        class register_IND : register
        {
            public register_IND(t11_device owner) : base(owner) { }
            public override void MAKE_EAW_(int r) { m_owner.MAKE_EAW_IND(r); }
            public override void GET_SB_(uint16_t op) { m_owner.GET_SB_IND(op); }
            public override void GET_SW_(uint16_t op) { m_owner.GET_SW_IND(op); }
            public override void GET_DB_(uint16_t op) { m_owner.GET_DB_IND(op); }
            public override void GET_DW_(uint16_t op) { m_owner.GET_DW_IND(op); }
            public override void PUT_DB_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DBT_(uint16_t op, int v) { m_owner.PUT_DBT_IND(op, v); }
            public override void PUT_DW_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DWT_(uint16_t op, int v) { m_owner.PUT_DWT_IND(op, v); }
        }
        class register_DE : register
        {
            public register_DE(t11_device owner) : base(owner) { }
            public override void MAKE_EAW_(int r) { m_owner.MAKE_EAW_DE(r); }
            public override void GET_SB_(uint16_t op) { m_owner.GET_SB_DE(op); }
            public override void GET_SW_(uint16_t op) { m_owner.GET_SW_DE(op); }
            public override void GET_DB_(uint16_t op) { throw new emu_unimplemented(); }
            public override void GET_DW_(uint16_t op) { m_owner.GET_DW_DE(op); }
            public override void PUT_DB_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DBT_(uint16_t op, int v) { m_owner.PUT_DBT_DE(op, v); }
            public override void PUT_DW_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DWT_(uint16_t op, int v) { m_owner.PUT_DWT_DE(op, v); }
        }
        class register_DED : register
        {
            public register_DED(t11_device owner) : base(owner) { }
            public override void MAKE_EAW_(int r) { m_owner.MAKE_EAW_DED(r); }
            public override void GET_SB_(uint16_t op) { throw new emu_unimplemented(); }
            public override void GET_SW_(uint16_t op) { m_owner.GET_SW_DED(op); }
            public override void GET_DB_(uint16_t op) { throw new emu_unimplemented(); }
            public override void GET_DW_(uint16_t op) { m_owner.GET_DW_DED(op); }
            public override void PUT_DB_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DBT_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DW_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DWT_(uint16_t op, int v) { throw new emu_unimplemented(); }
        }
        class register_IX : register
        {
            public register_IX(t11_device owner) : base(owner) { }
            public override void MAKE_EAW_(int r) { m_owner.MAKE_EAW_IX(r); }
            public override void GET_SB_(uint16_t op) { m_owner.GET_SB_IX(op); }
            public override void GET_SW_(uint16_t op) { m_owner.GET_SW_IX(op); }
            public override void GET_DB_(uint16_t op) { m_owner.GET_DB_IX(op); }
            public override void GET_DW_(uint16_t op) { m_owner.GET_DW_IX(op); }
            public override void PUT_DB_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DBT_(uint16_t op, int v) { m_owner.PUT_DBT_IX(op, v); }
            public override void PUT_DW_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DWT_(uint16_t op, int v) { m_owner.PUT_DWT_IX(op, v); }
        }
        class register_IXD : register
        {
            public register_IXD(t11_device owner) : base(owner) { }
            public override void MAKE_EAW_(int r) { m_owner.MAKE_EAW_IXD(r); }
            public override void GET_SB_(uint16_t op) { m_owner.GET_SB_IXD(op); }
            public override void GET_SW_(uint16_t op) { m_owner.GET_SW_IXD(op); }
            public override void GET_DB_(uint16_t op) { m_owner.GET_DB_IXD(op); }
            public override void GET_DW_(uint16_t op) { m_owner.GET_DW_IXD(op); }
            public override void PUT_DB_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DBT_(uint16_t op, int v) { m_owner.PUT_DBT_IXD(op, v); }
            public override void PUT_DW_(uint16_t op, int v) { throw new emu_unimplemented(); }
            public override void PUT_DWT_(uint16_t op, int v) { m_owner.PUT_DWT_IXD(op, v); }
        }
        register_RG  RG;
        register_RGD RGD;
        register_IN  IN;
        register_IND IND;
        register_DE  DE;
        register_DED DED;
        register_IX  IX;
        register_IXD IXD;

        int dest_;
        int dreg_;
        int ea_;
        int result_;
        int source_;
        int sreg_;

        void ClearVars() { dest_ = 0; dreg_ = 0; ea_ = 0; result_ = 0; source_ = 0; sreg_ = 0; }


        /* given a register index 'r', this computes the effective address for a byte-sized operation
           and puts the result in 'ea' */
        void MAKE_EAB_RGD(int r) { ea_ = (int)REGD(r); }  //#define MAKE_EAB_RGD(r) ea = REGD(r)
        void MAKE_EAB_IN(int r) { ea_ = (int)REGD(r); REGW(r) += (r < 6 ? (uint16_t)1 : (uint16_t)2); }  //#define MAKE_EAB_IN(r)  ea = REGD(r); REGW(r) += ((r) < 6 ? 1 : 2)
        //#define MAKE_EAB_INS(r) ea = REGD(r); REGW(r) += ((r) < 6 ? 1 : 2)
        void MAKE_EAB_IND(int r) { ea_ = (int)REGD(r); REGW(r) += 2; ea_ = RWORD(ea_); }  //#define MAKE_EAB_IND(r) ea = REGD(r); REGW(r) += 2; ea = RWORD(ea)
        void MAKE_EAB_DE(int r) { REGW(r) -= (r < 6 ? (uint16_t)1 : (uint16_t)2); ea_ = (int)REGD(r); }  //#define MAKE_EAB_DE(r)  REGW(r) -= ((r) < 6 ? 1 : 2); ea = REGD(r)
        void MAKE_EAB_DED(int r) { REGW(r) -= 2; ea_ = (int)REGD(r); ea_ = RWORD(ea_); }  //#define MAKE_EAB_DED(r) REGW(r) -= 2; ea = REGD(r); ea = RWORD(ea)
        void MAKE_EAB_IX(int r)  { ea_ = ROPCODE(); ea_ = (int)(((uint32_t)ea_ + REGD(r)) & 0xffff); }  //#define MAKE_EAB_IX(r)  ea = ROPCODE(); ea = (ea + REGD(r)) & 0xffff
        void MAKE_EAB_IXD(int r) { ea_ = ROPCODE(); ea_ = (int)(((uint32_t)ea_ + REGD(r)) & 0xffff); ea_ = RWORD(ea_); }  //#define MAKE_EAB_IXD(r) ea = ROPCODE(); ea = (ea + REGD(r)) & 0xffff; ea = RWORD(ea)

        /* given a register index 'r', this computes the effective address for a word-sized operation
           and puts the result in 'ea' */
        /* note that word accesses ignore the low bit!! this fixes APB! */
        void MAKE_EAW_RGD(int r) { MAKE_EAB_RGD(r); }  //#define MAKE_EAW_RGD(r) MAKE_EAB_RGD(r)
        void MAKE_EAW_IN(int r)  { ea_ = (int)REGD(r); REGW(r) += 2; }  //#define MAKE_EAW_IN(r)  ea = REGD(r); REGW(r) += 2
        void MAKE_EAW_IND(int r) { MAKE_EAB_IND(r); }  //#define MAKE_EAW_IND(r) MAKE_EAB_IND(r)
        void MAKE_EAW_DE(int r)  { REGW(r) -= 2; ea_ = (int)REGD(r); }  //#define MAKE_EAW_DE(r)  REGW(r) -= 2; ea = REGD(r)
        void MAKE_EAW_DED(int r) { MAKE_EAB_DED(r); }  //#define MAKE_EAW_DED(r) MAKE_EAB_DED(r)
        void MAKE_EAW_IX(int r)  { MAKE_EAB_IX(r); }  //#define MAKE_EAW_IX(r)  MAKE_EAB_IX(r)
        void MAKE_EAW_IXD(int r) { MAKE_EAB_IXD(r); }  //#define MAKE_EAW_IXD(r) MAKE_EAB_IXD(r)

        /* extracts the source/destination register index from the opcode into 'sreg' or 'dreg' */
        void GET_SREG(uint16_t op) { sreg_ = (op >> 6) & 7; }  //#define GET_SREG sreg = (op >> 6) & 7
        void GET_DREG(uint16_t op) { dreg_ = op & 7; }  //#define GET_DREG dreg = op & 7

        /* for a byte-sized source operand: extracts 'sreg', computes 'ea', and loads the value into 'source' */
        void GET_SB_RG(uint16_t op) { GET_SREG(op); source_ = REGB(sreg_); }  //#define GET_SB_RG  GET_SREG; source = REGB(sreg)
        void GET_SB_RGD(uint16_t op) { GET_SREG(op); MAKE_EAB_RGD(sreg_); source_ = RBYTE(ea_); }  //#define GET_SB_RGD GET_SREG; MAKE_EAB_RGD(sreg); source = RBYTE(ea)
        void GET_SB_IN(uint16_t op) { GET_SREG(op); if (sreg_ == 7) { source_ = ROPCODE(); } else { MAKE_EAB_IN(sreg_); source_ = RBYTE(ea_); } }  //#define GET_SB_IN  GET_SREG; if (sreg == 7) { source = ROPCODE(); } else { MAKE_EAB_IN(sreg); source = RBYTE(ea); }
        void GET_SB_IND(uint16_t op) { GET_SREG(op); if (sreg_ == 7) { ea_ = ROPCODE(); } else { MAKE_EAB_IND(sreg_); } source_ = RBYTE(ea_); }  //#define GET_SB_IND GET_SREG; if (sreg == 7) { ea = ROPCODE(); } else { MAKE_EAB_IND(sreg); } source = RBYTE(ea)
        void GET_SB_DE(uint16_t op) { GET_SREG(op); MAKE_EAB_DE(sreg_); source_ = RBYTE(ea_); }  //#define GET_SB_DE  GET_SREG; MAKE_EAB_DE(sreg); source = RBYTE(ea)
        //#define GET_SB_DED GET_SREG; MAKE_EAB_DED(sreg); source = RBYTE(ea)
        void GET_SB_IX(uint16_t op) { GET_SREG(op); MAKE_EAB_IX(sreg_); source_ = RBYTE(ea_); }  //#define GET_SB_IX  GET_SREG; MAKE_EAB_IX(sreg); source = RBYTE(ea)
        void GET_SB_IXD(uint16_t op) { GET_SREG(op); MAKE_EAB_IXD(sreg_); source_ = RBYTE(ea_); }  //#define GET_SB_IXD GET_SREG; MAKE_EAB_IXD(sreg); source = RBYTE(ea)

        /* for a word-sized source operand: extracts 'sreg', computes 'ea', and loads the value into 'source' */
        void GET_SW_RG(uint16_t op) { GET_SREG(op); source_ = (int)REGD(sreg_); }  //#define GET_SW_RG  GET_SREG; source = REGD(sreg)
        void GET_SW_RGD(uint16_t op) { GET_SREG(op); MAKE_EAW_RGD(sreg_); source_ = RWORD(ea_); }  //#define GET_SW_RGD GET_SREG; MAKE_EAW_RGD(sreg); source = RWORD(ea)
        void GET_SW_IN(uint16_t op) { GET_SREG(op); if (sreg_ == 7) { source_ = ROPCODE(); } else { MAKE_EAW_IN(sreg_); source_ = RWORD(ea_); } }
        void GET_SW_IND(uint16_t op) { GET_SREG(op); if (sreg_ == 7) { ea_ = ROPCODE(); } else { MAKE_EAW_IND(sreg_); } source_ = RWORD(ea_); }
        void GET_SW_DE(uint16_t op) { GET_SREG(op); MAKE_EAW_DE(sreg_); source_ = RWORD(ea_); }  //#define GET_SW_DE  GET_SREG; MAKE_EAW_DE(sreg); source = RWORD(ea)
        void GET_SW_DED(uint16_t op) { GET_SREG(op); MAKE_EAW_DED(sreg_); source_ = RWORD(ea_); }  //#define GET_SW_DED GET_SREG; MAKE_EAW_DED(sreg); source = RWORD(ea)
        void GET_SW_IX(uint16_t op) { GET_SREG(op); MAKE_EAW_IX(sreg_); source_ = RWORD(ea_); }  //#define GET_SW_IX  GET_SREG; MAKE_EAW_IX(sreg); source = RWORD(ea)
        void GET_SW_IXD(uint16_t op) { GET_SREG(op); MAKE_EAW_IXD(sreg_); source_ = RWORD(ea_); }  //#define GET_SW_IXD GET_SREG; MAKE_EAW_IXD(sreg); source = RWORD(ea)

        /* for a byte-sized destination operand: extracts 'dreg', computes 'ea', and loads the value into 'dest' */
        void GET_DB_RG(uint16_t op) { GET_DREG(op); dest_ = REGB(dreg_); }  //#define GET_DB_RG  GET_DREG; dest = REGB(dreg)
        void GET_DB_RGD(uint16_t op) { GET_DREG(op); MAKE_EAB_RGD(dreg_); dest_ = RBYTE(ea_); }  //#define GET_DB_RGD GET_DREG; MAKE_EAB_RGD(dreg); dest = RBYTE(ea)
        void GET_DB_IN(uint16_t op) { GET_DREG(op); MAKE_EAB_IN(dreg_); dest_ = RBYTE(ea_); }  //#define GET_DB_IN  GET_DREG; MAKE_EAB_IN(dreg); dest = RBYTE(ea)
        void GET_DB_IND(uint16_t op) { GET_DREG(op); if (dreg_ == 7) { ea_ = ROPCODE(); } else { MAKE_EAB_IND(dreg_); } dest_ = RBYTE(ea_); }  //#define GET_DB_IND GET_DREG; if (dreg == 7) { ea = ROPCODE(); } else { MAKE_EAB_IND(dreg); } dest = RBYTE(ea)
        //#define GET_DB_DE  GET_DREG; MAKE_EAB_DE(dreg); dest = RBYTE(ea)
        //#define GET_DB_DED GET_DREG; MAKE_EAB_DED(dreg); dest = RBYTE(ea)
        void GET_DB_IX(uint16_t op) { GET_DREG(op); MAKE_EAB_IX(dreg_); dest_ = RBYTE(ea_); }  //#define GET_DB_IX  GET_DREG; MAKE_EAB_IX(dreg); dest = RBYTE(ea)
        void GET_DB_IXD(uint16_t op) { GET_DREG(op); MAKE_EAB_IXD(dreg_); dest_ = RBYTE(ea_); }  //#define GET_DB_IXD GET_DREG; MAKE_EAB_IXD(dreg); dest = RBYTE(ea)

        /* for a word-sized destination operand: extracts 'dreg', computes 'ea', and loads the value into 'dest' */
        void GET_DW_RG(uint16_t op) { GET_DREG(op); dest_ = (int)REGD(dreg_); }  //#define GET_DW_RG  GET_DREG; dest = REGD(dreg)
        void GET_DW_RGD(uint16_t op) { GET_DREG(op); MAKE_EAW_RGD(dreg_); dest_ = RWORD(ea_); }  //#define GET_DW_RGD GET_DREG; MAKE_EAW_RGD(dreg); dest = RWORD(ea)
        void GET_DW_IN(uint16_t op) { GET_DREG(op); MAKE_EAW_IN(dreg_); dest_ = RWORD(ea_); }  //#define GET_DW_IN  GET_DREG; MAKE_EAW_IN(dreg); dest = RWORD(ea)
        void GET_DW_IND(uint16_t op) { GET_DREG(op); if (dreg_ == 7) { ea_ = ROPCODE(); } else { MAKE_EAW_IND(dreg_); } dest_ = RWORD(ea_); }  //#define GET_DW_IND GET_DREG; if (dreg == 7) { ea = ROPCODE(); } else { MAKE_EAW_IND(dreg); } dest = RWORD(ea)
        void GET_DW_DE(uint16_t op) { GET_DREG(op); MAKE_EAW_DE(dreg_); dest_ = RWORD(ea_); }  //#define GET_DW_DE  GET_DREG; MAKE_EAW_DE(dreg); dest = RWORD(ea)
        void GET_DW_DED(uint16_t op) { GET_DREG(op); MAKE_EAW_DED(dreg_); dest_ = RWORD(ea_); }  //#define GET_DW_DED GET_DREG; MAKE_EAW_DED(dreg); dest = RWORD(ea)
        void GET_DW_IX(uint16_t op) { GET_DREG(op); MAKE_EAW_IX(dreg_); dest_ = RWORD(ea_); }  //#define GET_DW_IX  GET_DREG; MAKE_EAW_IX(dreg); dest = RWORD(ea)
        void GET_DW_IXD(uint16_t op) { GET_DREG(op); MAKE_EAW_IXD(dreg_); dest_ = RWORD(ea_); }  //#define GET_DW_IXD GET_DREG; MAKE_EAW_IXD(dreg); dest = RWORD(ea)

        /* writes a value to a previously computed 'ea' */
        void PUT_DB_EA(int v) { WBYTE(ea_, v); }  //#define PUT_DB_EA(v) WBYTE(ea, (v))
        void PUT_DW_EA(int v) { WWORD(ea_, v); }  //#define PUT_DW_EA(v) WWORD(ea, (v))

        /* writes a value to a previously computed 'dreg' register */
        void PUT_DB_DREG(int v) { REGB(dreg_) = (uint8_t)v; }  //#define PUT_DB_DREG(v) REGB(dreg) = (v)
        void PUT_DW_DREG(int v) { REGW(dreg_) = (uint16_t)v; }  //#define PUT_DW_DREG(v) REGW(dreg) = (v)

        /* for a byte-sized destination operand: extracts 'dreg', computes 'ea', and writes 'v' to it */
        void PUT_DB_RG(uint16_t op, int v) { GET_DREG(op); REGB(dreg_) = (uint8_t)v; }  //#define PUT_DB_RG(v)  GET_DREG; REGB(dreg) = (v)
        //#define PUT_DB_RGD(v) GET_DREG; MAKE_EAB_RGD(dreg); WBYTE(ea, (v))
        //#define PUT_DB_IN(v)  GET_DREG; MAKE_EAB_IN(dreg); WBYTE(ea, (v))
        //#define PUT_DB_IND(v) GET_DREG; if (dreg == 7) { ea = ROPCODE(); } else { MAKE_EAB_IND(dreg); } WBYTE(ea, (v))
        //#define PUT_DB_DE(v)  GET_DREG; MAKE_EAB_DE(dreg); WBYTE(ea, (v))
        //#define PUT_DB_DED(v) GET_DREG; MAKE_EAB_DED(dreg); WBYTE(ea, (v))
        //#define PUT_DB_IX(v)  GET_DREG; MAKE_EAB_IX(dreg); WBYTE(ea, (v))
        //#define PUT_DB_IXD(v) GET_DREG; MAKE_EAB_IXD(dreg); WBYTE(ea, (v))

        /* special bus sequence for MOV, CLR, SXT */
        void PUT_DBT_RGD(uint16_t op, int v) { GET_DREG(op); MAKE_EAB_RGD(dreg_); RBYTE(ea_); WBYTE(ea_, v); }  //#define PUT_DBT_RGD(v) GET_DREG; MAKE_EAB_RGD(dreg); RBYTE(ea); WBYTE(ea, (v))
        void PUT_DBT_IN(uint16_t op, int v) { GET_DREG(op); MAKE_EAB_IN(dreg_); RBYTE(ea_); WBYTE(ea_, v); }  //#define PUT_DBT_IN(v)  GET_DREG; MAKE_EAB_IN(dreg); RBYTE(ea); WBYTE(ea, (v))
        void PUT_DBT_IND(uint16_t op, int v) { GET_DREG(op); if (dreg_ == 7) { ea_ = ROPCODE(); } else { MAKE_EAB_IND(dreg_); } RBYTE(ea_); WBYTE(ea_, v); }  //#define PUT_DBT_IND(v) GET_DREG; if (dreg == 7) { ea = ROPCODE(); } else { MAKE_EAB_IND(dreg); } RBYTE(ea); WBYTE(ea, (v))
        void PUT_DBT_DE(uint16_t op, int v) { GET_DREG(op); MAKE_EAB_DE(dreg_); RBYTE(ea_); WBYTE(ea_, v); }  //#define PUT_DBT_DE(v)  GET_DREG; MAKE_EAB_DE(dreg); RBYTE(ea); WBYTE(ea, (v))
        //#define PUT_DBT_DED(v) GET_DREG; MAKE_EAB_DED(dreg); RBYTE(ea); WBYTE(ea, (v))
        void PUT_DBT_IX(uint16_t op, int v) { GET_DREG(op); MAKE_EAB_IX(dreg_); RBYTE(ea_); WBYTE(ea_, v); }  //#define PUT_DBT_IX(v)  GET_DREG; MAKE_EAB_IX(dreg); RBYTE(ea); WBYTE(ea, (v))
        void PUT_DBT_IXD(uint16_t op, int v) { GET_DREG(op); MAKE_EAB_IXD(dreg_); RBYTE(ea_); WBYTE(ea_, v); }  //#define PUT_DBT_IXD(v) GET_DREG; MAKE_EAB_IXD(dreg); RBYTE(ea); WBYTE(ea, (v))

        /* for a word-sized destination operand: extracts 'dreg', computes 'ea', and writes 'v' to it */
        void PUT_DW_RG(uint16_t op, int v) { GET_DREG(op); REGW(dreg_) = (uint16_t)v; }  //#define PUT_DW_RG(v)  GET_DREG; REGW(dreg) = (v)
        //#define PUT_DW_RGD(v) GET_DREG; MAKE_EAW_RGD(dreg); WWORD(ea, (v))
        //#define PUT_DW_IN(v)  GET_DREG; MAKE_EAW_IN(dreg); WWORD(ea, (v))
        //#define PUT_DW_IND(v) GET_DREG; if (dreg == 7) { ea = ROPCODE(); } else { MAKE_EAW_IND(dreg); } WWORD(ea, (v))
        //#define PUT_DW_DE(v)  GET_DREG; MAKE_EAW_DE(dreg); WWORD(ea, (v))
        //#define PUT_DW_DED(v) GET_DREG; MAKE_EAW_DED(dreg); WWORD(ea, (v))
        //#define PUT_DW_IX(v)  GET_DREG; MAKE_EAW_IX(dreg); WWORD(ea, (v))
        //#define PUT_DW_IXD(v) GET_DREG; MAKE_EAW_IXD(dreg); WWORD(ea, (v))

        /* special bus sequence for MOV, CLR, SXT */
        void PUT_DWT_RG(uint16_t op, int v) { PUT_DW_RG(op, v); }
        void PUT_DWT_RGD(uint16_t op, int v) { GET_DREG(op); MAKE_EAW_RGD(dreg_); RWORD(ea_); WWORD(ea_, v); }  //#define PUT_DWT_RGD(v) GET_DREG; MAKE_EAW_RGD(dreg); RWORD(ea); WWORD(ea, (v))
        void PUT_DWT_IN(uint16_t op, int v) { GET_DREG(op); MAKE_EAW_IN(dreg_); RWORD(ea_); WWORD(ea_, v); }  //#define PUT_DWT_IN(v)  GET_DREG; MAKE_EAW_IN(dreg); RWORD(ea); WWORD(ea, (v))
        void PUT_DWT_IND(uint16_t op, int v) { GET_DREG(op); if (dreg_ == 7) { ea_ = ROPCODE(); } else { MAKE_EAW_IND(dreg_); } RWORD(ea_); WWORD(ea_, v); }  //#define PUT_DWT_IND(v) GET_DREG; if (dreg == 7) { ea = ROPCODE(); } else { MAKE_EAW_IND(dreg); } RWORD(ea); WWORD(ea, (v))
        void PUT_DWT_DE(uint16_t op, int v) { GET_DREG(op); MAKE_EAW_DE(dreg_); RWORD(ea_); WWORD(ea_, v); }  //#define PUT_DWT_DE(v)  GET_DREG; MAKE_EAW_DE(dreg); RWORD(ea); WWORD(ea, (v))
        //#define PUT_DWT_DED(v) GET_DREG; MAKE_EAW_DED(dreg); RWORD(ea); WWORD(ea, (v))
        void PUT_DWT_IX(uint16_t op, int v) { GET_DREG(op); MAKE_EAW_IX(dreg_); RWORD(ea_); WWORD(ea_, v); }  //#define PUT_DWT_IX(v)  GET_DREG; MAKE_EAW_IX(dreg); RWORD(ea); WWORD(ea, (v))
        void PUT_DWT_IXD(uint16_t op, int v) { GET_DREG(op); MAKE_EAW_IXD(dreg_); RWORD(ea_); WWORD(ea_, v); }  //#define PUT_DWT_IXD(v) GET_DREG; MAKE_EAW_IXD(dreg); RWORD(ea); WWORD(ea, (v))

        /* flag clearing; must be done before setting */
        void CLR_ZV() { PSW &= unchecked((uint8_t)~(ZFLAG | VFLAG)); }  //#define CLR_ZV   (PSW &= ~(ZFLAG | VFLAG))
        void CLR_NZV() { PSW &= unchecked((uint8_t)~(NFLAG | ZFLAG | VFLAG)); }  //#define CLR_NZV  (PSW &= ~(NFLAG | ZFLAG | VFLAG))
        void CLR_NZVC() { PSW &= unchecked((uint8_t)~(NFLAG | ZFLAG | VFLAG | CFLAG)); }  //#define CLR_NZVC (PSW &= ~(NFLAG | ZFLAG | VFLAG | CFLAG))

        /* set individual flags byte-sized */
        void SETB_N() { PSW |= (uint8_t)((result_ >> 4) & 0x08); }  //#define SETB_N (PSW |= (result >> 4) & 0x08)
        void SETB_Z() { PSW |= (uint8_t)((((result_ & 0xff) == 0) ? 1 : 0) << 2); }  //#define SETB_Z (PSW |= ((result & 0xff) == 0) << 2)
        void SETB_V() { PSW |= (uint8_t)(((source_ ^ dest_ ^ result_ ^ (result_ >> 1)) >> 6) & 0x02); }  //#define SETB_V (PSW |= ((source ^ dest ^ result ^ (result >> 1)) >> 6) & 0x02)
        void SETB_C() { PSW |= (uint8_t)((result_ >> 8) & 0x01); }  //#define SETB_C (PSW |= (result >> 8) & 0x01)
        void SETB_NZ() { SETB_N(); SETB_Z(); }  //#define SETB_NZ SETB_N; SETB_Z
        //#define SETB_NZV SETB_N; SETB_Z; SETB_V
        void SETB_NZVC() { SETB_N(); SETB_Z(); SETB_V(); SETB_C(); }  //#define SETB_NZVC SETB_N; SETB_Z; SETB_V; SETB_C

        /* set individual flags word-sized */
        void SETW_N() { PSW |= (uint8_t)((result_ >> 12) & 0x08); }  //#define SETW_N (PSW |= (result >> 12) & 0x08)
        void SETW_Z() { PSW |= (uint8_t)((((result_ & 0xffff) == 0) ? 1 : 0) << 2); }  //#define SETW_Z (PSW |= ((result & 0xffff) == 0) << 2)
        void SETW_V() { PSW |= (uint8_t)(((source_ ^ dest_ ^ result_ ^ (result_ >> 1)) >> 14) & 0x02); }  //#define SETW_V (PSW |= ((source ^ dest ^ result ^ (result >> 1)) >> 14) & 0x02)
        void SETW_C() { PSW |= (uint8_t)((result_ >> 16) & 0x01); }  //#define SETW_C (PSW |= (result >> 16) & 0x01)
        void SETW_NZ() { SETW_N(); SETW_Z(); }  //#define SETW_NZ SETW_N; SETW_Z
        //#define SETW_NZV SETW_N; SETW_Z; SETW_V
        void SETW_NZVC() { SETW_N(); SETW_Z(); SETW_V(); SETW_C(); }  //#define SETW_NZVC SETW_N; SETW_Z; SETW_V; SETW_C

        /* operations */
        /* ADC: dst += C */
        void ADC_R(uint16_t op, register d) { /*int dreg, source, dest, result;*/      source_ = GET_C; d.GET_DW_(op); CLR_NZVC(); result_ = dest_ + source_; SETW_NZVC(); PUT_DW_DREG(result_); }  //#define ADC_R(d)    int dreg, source, dest, result;     source = GET_C; GET_DW_##d; CLR_NZVC; result = dest + source; SETW_NZVC; PUT_DW_DREG(result)
        void ADC_M(uint16_t op, register d) { /*int dreg, source, dest, result, ea;*/  source_ = GET_C; d.GET_DW_(op); CLR_NZVC(); result_ = dest_ + source_; SETW_NZVC(); PUT_DW_EA(result_); }  //#define ADC_M(d)    int dreg, source, dest, result, ea; source = GET_C; GET_DW_##d; CLR_NZVC; result = dest + source; SETW_NZVC; PUT_DW_EA(result)
        void ADCB_R(uint16_t op, register d) { /*int dreg, source, dest, result;*/     source_ = GET_C; d.GET_DB_(op); CLR_NZVC(); result_ = dest_ + source_; SETB_NZVC(); PUT_DB_DREG(result_); }  //#define ADCB_R(d)   int dreg, source, dest, result;     source = GET_C; GET_DB_##d; CLR_NZVC; result = dest + source; SETB_NZVC; PUT_DB_DREG(result)
        void ADCB_M(uint16_t op, register d) { /*int dreg, source, dest, result, ea;*/ source_ = GET_C; d.GET_DB_(op); CLR_NZVC(); result_ = dest_ + source_; SETB_NZVC(); PUT_DB_EA(result_); }  //#define ADCB_M(d)   int dreg, source, dest, result, ea; source = GET_C; GET_DB_##d; CLR_NZVC; result = dest + source; SETB_NZVC; PUT_DB_EA(result)
        /* ADD: dst += src */
        void ADD_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result;*/     s.GET_SW_(op); d.GET_DW_(op); CLR_NZVC(); result_ = dest_ + source_; SETW_NZVC(); PUT_DW_DREG(result_); }  //#define ADD_R(s,d)  int sreg, dreg, source, dest, result;     GET_SW_##s; GET_DW_##d; CLR_NZVC; result = dest + source; SETW_NZVC; PUT_DW_DREG(result)
        void ADD_X(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/ s.GET_SW_(op); d.GET_DW_(op); CLR_NZVC(); result_ = dest_ + source_; SETW_NZVC(); PUT_DW_DREG(result_); }  //#define ADD_X(s,d)  int sreg, dreg, source, dest, result, ea; GET_SW_##s; GET_DW_##d; CLR_NZVC; result = dest + source; SETW_NZVC; PUT_DW_DREG(result)
        void ADD_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/ s.GET_SW_(op); d.GET_DW_(op); CLR_NZVC(); result_ = dest_ + source_; SETW_NZVC(); PUT_DW_EA(result_); }  //#define ADD_M(s,d)  int sreg, dreg, source, dest, result, ea; GET_SW_##s; GET_DW_##d; CLR_NZVC; result = dest + source; SETW_NZVC; PUT_DW_EA(result)
        /* ASL: dst = (dst << 1); C = (dst >> 7) */
        void ASL_R(uint16_t op, register d) { /*int dreg, dest, result;*/      d.GET_DW_(op); CLR_NZVC(); result_ = dest_ << 1; SETW_NZ(); PSW |= (uint8_t)((dest_ >> 15) & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DW_DREG(result_); }  //#define ASL_R(d)    int dreg, dest, result;     GET_DW_##d; CLR_NZVC; result = dest << 1; SETW_NZ; PSW |= (dest >> 15) & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DW_DREG(result)
        void ASL_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/  d.GET_DW_(op); CLR_NZVC(); result_ = dest_ << 1; SETW_NZ(); PSW |= (uint8_t)((dest_ >> 15) & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DW_EA(result_); }  //#define ASL_M(d)    int dreg, dest, result, ea; GET_DW_##d; CLR_NZVC; result = dest << 1; SETW_NZ; PSW |= (dest >> 15) & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DW_EA(result)
        void ASLB_R(uint16_t op, register d) { /*int dreg, dest, result;*/     d.GET_DB_(op); CLR_NZVC(); result_ = dest_ << 1; SETB_NZ(); PSW |= (uint8_t)((dest_ >> 7) & 1);  PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DB_DREG(result_); }  //#define ASLB_R(d)   int dreg, dest, result;     GET_DB_##d; CLR_NZVC; result = dest << 1; SETB_NZ; PSW |= (dest >> 7) & 1;  PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DB_DREG(result)
        void ASLB_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/ d.GET_DB_(op); CLR_NZVC(); result_ = dest_ << 1; SETB_NZ(); PSW |= (uint8_t)((dest_ >> 7) & 1);  PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DB_EA(result_); }  //#define ASLB_M(d)   int dreg, dest, result, ea; GET_DB_##d; CLR_NZVC; result = dest << 1; SETB_NZ; PSW |= (dest >> 7) & 1;  PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DB_EA(result)
        /* ASR: dst = (dst << 1); C = (dst >> 7) */
        void ASR_R(uint16_t op, register d) { /*int dreg, dest, result;*/      d.GET_DW_(op); CLR_NZVC(); result_ = (dest_ >> 1) | (dest_ & 0x8000); SETW_NZ(); PSW |= (uint8_t)(dest_ & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DW_DREG(result_); }  //#define ASR_R(d)    int dreg, dest, result;     GET_DW_##d; CLR_NZVC; result = (dest >> 1) | (dest & 0x8000); SETW_NZ; PSW |= dest & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DW_DREG(result)
        void ASR_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/  d.GET_DW_(op); CLR_NZVC(); result_ = (dest_ >> 1) | (dest_ & 0x8000); SETW_NZ(); PSW |= (uint8_t)(dest_ & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DW_EA(result_); }  //#define ASR_M(d)    int dreg, dest, result, ea; GET_DW_##d; CLR_NZVC; result = (dest >> 1) | (dest & 0x8000); SETW_NZ; PSW |= dest & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DW_EA(result)
        void ASRB_R(uint16_t op, register d) { /*int dreg, dest, result;*/     d.GET_DB_(op); CLR_NZVC(); result_ = (dest_ >> 1) | (dest_ & 0x80);   SETB_NZ(); PSW |= (uint8_t)(dest_ & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DB_DREG(result_); }  //#define ASRB_R(d)   int dreg, dest, result;     GET_DB_##d; CLR_NZVC; result = (dest >> 1) | (dest & 0x80);   SETB_NZ; PSW |= dest & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DB_DREG(result)
        void ASRB_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/ d.GET_DB_(op); CLR_NZVC(); result_ = (dest_ >> 1) | (dest_ & 0x80);   SETB_NZ(); PSW |= (uint8_t)(dest_ & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DB_EA(result_); }  //#define ASRB_M(d)   int dreg, dest, result, ea; GET_DB_##d; CLR_NZVC; result = (dest >> 1) | (dest & 0x80);   SETB_NZ; PSW |= dest & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DB_EA(result)
        /* BIC: dst &= ~src */
        void BIC_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result;*/      s.GET_SW_(op); d.GET_DW_(op); CLR_NZV(); result_ = dest_ & ~source_; SETW_NZ(); PUT_DW_DREG(result_); }  //#define BIC_R(s,d)  int sreg, dreg, source, dest, result;     GET_SW_##s; GET_DW_##d; CLR_NZV; result = dest & ~source; SETW_NZ; PUT_DW_DREG(result)
        void BIC_X(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/  s.GET_SW_(op); d.GET_DW_(op); CLR_NZV(); result_ = dest_ & ~source_; SETW_NZ(); PUT_DW_DREG(result_); }  //#define BIC_X(s,d)  int sreg, dreg, source, dest, result, ea; GET_SW_##s; GET_DW_##d; CLR_NZV; result = dest & ~source; SETW_NZ; PUT_DW_DREG(result)
        void BIC_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/  s.GET_SW_(op); d.GET_DW_(op); CLR_NZV(); result_ = dest_ & ~source_; SETW_NZ(); PUT_DW_EA(result_); }  //#define BIC_M(s,d)  int sreg, dreg, source, dest, result, ea; GET_SW_##s; GET_DW_##d; CLR_NZV; result = dest & ~source; SETW_NZ; PUT_DW_EA(result)
        void BICB_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result;*/     s.GET_SB_(op); d.GET_DB_(op); CLR_NZV(); result_ = dest_ & ~source_; SETB_NZ(); PUT_DB_DREG(result_); }  //#define BICB_R(s,d) int sreg, dreg, source, dest, result;     GET_SB_##s; GET_DB_##d; CLR_NZV; result = dest & ~source; SETB_NZ; PUT_DB_DREG(result)
        void BICB_X(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/ s.GET_SB_(op); d.GET_DB_(op); CLR_NZV(); result_ = dest_ & ~source_; SETB_NZ(); PUT_DB_DREG(result_); }  //#define BICB_X(s,d) int sreg, dreg, source, dest, result, ea; GET_SB_##s; GET_DB_##d; CLR_NZV; result = dest & ~source; SETB_NZ; PUT_DB_DREG(result)
        void BICB_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/ s.GET_SB_(op); d.GET_DB_(op); CLR_NZV(); result_ = dest_ & ~source_; SETB_NZ(); PUT_DB_EA(result_); }  //#define BICB_M(s,d) int sreg, dreg, source, dest, result, ea; GET_SB_##s; GET_DB_##d; CLR_NZV; result = dest & ~source; SETB_NZ; PUT_DB_EA(result)
        /* BIS: dst |= src */
        void BIS_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result;*/      s.GET_SW_(op); d.GET_DW_(op); CLR_NZV(); result_ = dest_ | source_; SETW_NZ(); PUT_DW_DREG(result_); }  //#define BIS_R(s,d)  int sreg, dreg, source, dest, result;     GET_SW_##s; GET_DW_##d; CLR_NZV; result = dest | source; SETW_NZ; PUT_DW_DREG(result)
        void BIS_X(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/  s.GET_SW_(op); d.GET_DW_(op); CLR_NZV(); result_ = dest_ | source_; SETW_NZ(); PUT_DW_DREG(result_); }  //#define BIS_X(s,d)  int sreg, dreg, source, dest, result, ea; GET_SW_##s; GET_DW_##d; CLR_NZV; result = dest | source; SETW_NZ; PUT_DW_DREG(result)
        void BIS_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/  s.GET_SW_(op); d.GET_DW_(op); CLR_NZV(); result_ = dest_ | source_; SETW_NZ(); PUT_DW_EA(result_); }  //#define BIS_M(s,d)  int sreg, dreg, source, dest, result, ea; GET_SW_##s; GET_DW_##d; CLR_NZV; result = dest | source; SETW_NZ; PUT_DW_EA(result)
        void BISB_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result;*/     s.GET_SB_(op); d.GET_DB_(op); CLR_NZV(); result_ = dest_ | source_; SETB_NZ(); PUT_DB_DREG(result_); }  //#define BISB_R(s,d) int sreg, dreg, source, dest, result;     GET_SB_##s; GET_DB_##d; CLR_NZV; result = dest | source; SETB_NZ; PUT_DB_DREG(result)
        void BISB_X(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/ s.GET_SB_(op); d.GET_DB_(op); CLR_NZV(); result_ = dest_ | source_; SETB_NZ(); PUT_DB_DREG(result_); }  //#define BISB_X(s,d) int sreg, dreg, source, dest, result, ea; GET_SB_##s; GET_DB_##d; CLR_NZV; result = dest | source; SETB_NZ; PUT_DB_DREG(result)
        void BISB_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/ s.GET_SB_(op); d.GET_DB_(op); CLR_NZV(); result_ = dest_ | source_; SETB_NZ(); PUT_DB_EA(result_); }  //#define BISB_M(s,d) int sreg, dreg, source, dest, result, ea; GET_SB_##s; GET_DB_##d; CLR_NZV; result = dest | source; SETB_NZ; PUT_DB_EA(result)
        /* BIT: flags = dst & src */
        void BIT_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result;*/      s.GET_SW_(op); d.GET_DW_(op); CLR_NZV(); result_ = dest_ & source_; SETW_NZ(); }  //#define BIT_R(s,d)  int sreg, dreg, source, dest, result;     GET_SW_##s; GET_DW_##d; CLR_NZV; result = dest & source; SETW_NZ;
        void BIT_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/  s.GET_SW_(op); d.GET_DW_(op); CLR_NZV(); result_ = dest_ & source_; SETW_NZ(); }  //#define BIT_M(s,d)  int sreg, dreg, source, dest, result, ea; GET_SW_##s; GET_DW_##d; CLR_NZV; result = dest & source; SETW_NZ;
        void BITB_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result;*/     s.GET_SB_(op); d.GET_DB_(op); CLR_NZV(); result_ = dest_ & source_; SETB_NZ(); }  //#define BITB_R(s,d) int sreg, dreg, source, dest, result;     GET_SB_##s; GET_DB_##d; CLR_NZV; result = dest & source; SETB_NZ;
        void BITB_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/ s.GET_SB_(op); d.GET_DB_(op); CLR_NZV(); result_ = dest_ & source_; SETB_NZ(); }  //#define BITB_M(s,d) int sreg, dreg, source, dest, result, ea; GET_SB_##s; GET_DB_##d; CLR_NZV; result = dest & source; SETB_NZ;
        /* BR: if (condition) branch */
        void BR(uint16_t op, bool c) { if (c) { PC += (uint16_t)(2 * (int8_t)(op & 0xff)); } }  //#define BR(c)       if (c) { PC += 2 * (signed char)(op & 0xff); }
        void BR(uint16_t op, int c) { BR(op, c != 0); }
        /* CLR: dst = 0 */
        void CLR_R(uint16_t op, register d) { /*int dreg;*/      d.PUT_DW_(op, 0); CLR_NZVC(); SET_Z(); }  //#define CLR_R(d)    int dreg;     PUT_DW_##d(0); CLR_NZVC; SET_Z
        void CLR_M(uint16_t op, register d) { /*int dreg, ea;*/  d.PUT_DWT_(op, 0); CLR_NZVC(); SET_Z(); }  //#define CLR_M(d)    int dreg, ea; PUT_DWT_##d(0); CLR_NZVC; SET_Z
        void CLRB_R(uint16_t op, register d) { /*int dreg;*/     d.PUT_DB_(op, 0); CLR_NZVC(); SET_Z(); }  //#define CLRB_R(d)   int dreg;     PUT_DB_##d(0); CLR_NZVC; SET_Z
        void CLRB_M(uint16_t op, register d) { /*int dreg, ea;*/ d.PUT_DBT_(op, 0); CLR_NZVC(); SET_Z(); }  //#define CLRB_M(d)   int dreg, ea; PUT_DBT_##d(0); CLR_NZVC; SET_Z
        /* CMP: flags = src - dst */
        void CMP_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result;*/      s.GET_SW_(op); d.GET_DW_(op); CLR_NZVC(); result_ = source_ - dest_; SETW_NZVC(); }  //#define CMP_R(s,d)  int sreg, dreg, source, dest, result;     GET_SW_##s; GET_DW_##d; CLR_NZVC; result = source - dest; SETW_NZVC;
        void CMP_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/  s.GET_SW_(op); d.GET_DW_(op); CLR_NZVC(); result_ = source_ - dest_; SETW_NZVC(); }  //#define CMP_M(s,d)  int sreg, dreg, source, dest, result, ea; GET_SW_##s; GET_DW_##d; CLR_NZVC; result = source - dest; SETW_NZVC;
        void CMPB_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result;*/     s.GET_SB_(op); d.GET_DB_(op); CLR_NZVC(); result_ = source_ - dest_; SETB_NZVC(); }  //#define CMPB_R(s,d) int sreg, dreg, source, dest, result;     GET_SB_##s; GET_DB_##d; CLR_NZVC; result = source - dest; SETB_NZVC;
        void CMPB_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/ s.GET_SB_(op); d.GET_DB_(op); CLR_NZVC(); result_ = source_ - dest_; SETB_NZVC(); }  //#define CMPB_M(s,d) int sreg, dreg, source, dest, result, ea; GET_SB_##s; GET_DB_##d; CLR_NZVC; result = source - dest; SETB_NZVC;
        /* COM: dst = ~dst */
        void COM_R(uint16_t op, register d) { /*int dreg, dest, result;*/      d.GET_DW_(op); CLR_NZVC(); result_ = ~dest_; SETW_NZ(); SET_C(); PUT_DW_DREG(result_); }  //#define COM_R(d)    int dreg, dest, result;     GET_DW_##d; CLR_NZVC; result = ~dest; SETW_NZ; SET_C; PUT_DW_DREG(result)
        void COM_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/  d.GET_DW_(op); CLR_NZVC(); result_ = ~dest_; SETW_NZ(); SET_C(); PUT_DW_EA(result_); }  //#define COM_M(d)    int dreg, dest, result, ea; GET_DW_##d; CLR_NZVC; result = ~dest; SETW_NZ; SET_C; PUT_DW_EA(result)
        void COMB_R(uint16_t op, register d) { /*int dreg, dest, result;*/     d.GET_DB_(op); CLR_NZVC(); result_ = ~dest_; SETB_NZ(); SET_C(); PUT_DB_DREG(result_); }  //#define COMB_R(d)   int dreg, dest, result;     GET_DB_##d; CLR_NZVC; result = ~dest; SETB_NZ; SET_C; PUT_DB_DREG(result)
        void COMB_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/ d.GET_DB_(op); CLR_NZVC(); result_ = ~dest_; SETB_NZ(); SET_C(); PUT_DB_EA(result_); }  //#define COMB_M(d)   int dreg, dest, result, ea; GET_DB_##d; CLR_NZVC; result = ~dest; SETB_NZ; SET_C; PUT_DB_EA(result)
        /* DEC: dst -= 1 */
        void DEC_R(uint16_t op, register d) { /*int dreg, dest, result;*/      d.GET_DW_(op); CLR_NZV(); result_ = dest_ - 1; SETW_NZ(); if (dest_ == 0x8000) SET_V(); PUT_DW_DREG(result_); }  //#define DEC_R(d)    int dreg, dest, result;     GET_DW_##d; CLR_NZV; result = dest - 1; SETW_NZ; if (dest == 0x8000) SET_V; PUT_DW_DREG(result)
        void DEC_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/  d.GET_DW_(op); CLR_NZV(); result_ = dest_ - 1; SETW_NZ(); if (dest_ == 0x8000) SET_V(); PUT_DW_EA(result_); }  //#define DEC_M(d)    int dreg, dest, result, ea; GET_DW_##d; CLR_NZV; result = dest - 1; SETW_NZ; if (dest == 0x8000) SET_V; PUT_DW_EA(result)
        void DECB_R(uint16_t op, register d) { /*int dreg, dest, result;*/     d.GET_DB_(op); CLR_NZV(); result_ = dest_ - 1; SETB_NZ(); if (dest_ == 0x80)   SET_V(); PUT_DB_DREG(result_); }  //#define DECB_R(d)   int dreg, dest, result;     GET_DB_##d; CLR_NZV; result = dest - 1; SETB_NZ; if (dest == 0x80)   SET_V; PUT_DB_DREG(result)
        void DECB_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/ d.GET_DB_(op); CLR_NZV(); result_ = dest_ - 1; SETB_NZ(); if (dest_ == 0x80)   SET_V(); PUT_DB_EA(result_); }  //#define DECB_M(d)   int dreg, dest, result, ea; GET_DB_##d; CLR_NZV; result = dest - 1; SETB_NZ; if (dest == 0x80)   SET_V; PUT_DB_EA(result)
        /* INC: dst += 1 */
        void INC_R(uint16_t op, register d) { /*int dreg, dest, result;*/      d.GET_DW_(op); CLR_NZV(); result_ = dest_ + 1; SETW_NZ(); if (dest_ == 0x7fff) SET_V(); PUT_DW_DREG(result_); }  //#define INC_R(d)    int dreg, dest, result;     GET_DW_##d; CLR_NZV; result = dest + 1; SETW_NZ; if (dest == 0x7fff) SET_V; PUT_DW_DREG(result)
        void INC_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/  d.GET_DW_(op); CLR_NZV(); result_ = dest_ + 1; SETW_NZ(); if (dest_ == 0x7fff) SET_V(); PUT_DW_EA(result_); }  //#define INC_M(d)    int dreg, dest, result, ea; GET_DW_##d; CLR_NZV; result = dest + 1; SETW_NZ; if (dest == 0x7fff) SET_V; PUT_DW_EA(result)
        void INCB_R(uint16_t op, register d) { /*int dreg, dest, result;*/     d.GET_DB_(op); CLR_NZV(); result_ = dest_ + 1; SETB_NZ(); if (dest_ == 0x7f)   SET_V(); PUT_DB_DREG(result_); }  //#define INCB_R(d)   int dreg, dest, result;     GET_DB_##d; CLR_NZV; result = dest + 1; SETB_NZ; if (dest == 0x7f)   SET_V; PUT_DB_DREG(result)
        void INCB_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/ d.GET_DB_(op); CLR_NZV(); result_ = dest_ + 1; SETB_NZ(); if (dest_ == 0x7f)   SET_V(); PUT_DB_EA(result_); }  //#define INCB_M(d)   int dreg, dest, result, ea; GET_DB_##d; CLR_NZV; result = dest + 1; SETB_NZ; if (dest == 0x7f)   SET_V; PUT_DB_EA(result)
        /* JMP: PC = ea */
        void JMP(uint16_t op, register d) { /*int dreg, ea;*/ GET_DREG(op); d.MAKE_EAW_(dreg_); PC = (uint16_t)ea_; }  //#define JMP(d)      int dreg, ea; GET_DREG; MAKE_EAW_##d(dreg); PC = ea
        /* JSR: PUSH src, src = PC, PC = ea */
        void JSR(uint16_t op, register d) { /*int sreg, dreg, ea;*/ GET_SREG(op); GET_DREG(op); d.MAKE_EAW_(dreg_); PUSH(REGW(sreg_)); REGW(sreg_) = PC; PC = (uint16_t)ea_; }  //#define JSR(d)      int sreg, dreg, ea; GET_SREG; GET_DREG; MAKE_EAW_##d(dreg); PUSH(REGW(sreg)); REGW(sreg) = PC; PC = ea
        /* MFPS: dst = flags */
        void MFPS_R(uint16_t op, register d) { /*int dreg, result;*/     result_ = PSW; CLR_NZV(); SETB_NZ(); d.PUT_DW_(op, (int8_t)result_); }  //#define MFPS_R(d)   int dreg, result;     result = PSW; CLR_NZV; SETB_NZ; PUT_DW_##d((signed char)result)
        void MFPS_M(uint16_t op, register d) { /*int dreg, result, ea;*/ result_ = PSW; CLR_NZV(); SETB_NZ(); d.PUT_DB_(op, result_); }  //#define MFPS_M(d)   int dreg, result, ea; result = PSW; CLR_NZV; SETB_NZ; PUT_DB_##d(result)
        /* MOV: dst = src */
        void MOV_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, result;*/      s.GET_SW_(op); CLR_NZV(); result_ = source_; SETW_NZ(); d.PUT_DW_(op, result_); }  //#define MOV_R(s,d)  int sreg, dreg, source, result;     GET_SW_##s; CLR_NZV; result = source; SETW_NZ; PUT_DW_##d(result)
        void MOV_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, result, ea;*/  s.GET_SW_(op); CLR_NZV(); result_ = source_; SETW_NZ(); d.PUT_DWT_(op, result_); }  //#define MOV_M(s,d)  int sreg, dreg, source, result, ea; GET_SW_##s; CLR_NZV; result = source; SETW_NZ; PUT_DWT_##d(result)
        void MOVB_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, result;*/     s.GET_SB_(op); CLR_NZV(); result_ = source_; SETB_NZ(); d.PUT_DW_(op, (int8_t)result_); }  //#define MOVB_R(s,d) int sreg, dreg, source, result;     GET_SB_##s; CLR_NZV; result = source; SETB_NZ; PUT_DW_##d((signed char)result)
        void MOVB_X(uint16_t op, register s, register d) { /*int sreg, dreg, source, result, ea;*/ s.GET_SB_(op); CLR_NZV(); result_ = source_; SETB_NZ(); d.PUT_DW_(op, (int8_t)result_); }  //#define MOVB_X(s,d) int sreg, dreg, source, result, ea; GET_SB_##s; CLR_NZV; result = source; SETB_NZ; PUT_DW_##d((signed char)result)
        void MOVB_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, result, ea;*/ s.GET_SB_(op); CLR_NZV(); result_ = source_; SETB_NZ(); d.PUT_DBT_(op, result_); }  //#define MOVB_M(s,d) int sreg, dreg, source, result, ea; GET_SB_##s; CLR_NZV; result = source; SETB_NZ; PUT_DBT_##d(result)
        /* MTPS: flags = src */
        void MTPS_R(uint16_t op, register d) { /*int dreg, dest;*/     d.GET_DW_(op); PSW = (uint8_t)((PSW & ~0xef) | (dest_ & 0xef)); t11_check_irqs(); }  //#define MTPS_R(d)   int dreg, dest;     GET_DW_##d; PSW = (PSW & ~0xef) | (dest & 0xef); t11_check_irqs()
        void MTPS_M(uint16_t op, register d) { /*int dreg, dest, ea;*/ d.GET_DW_(op); PSW = (uint8_t)((PSW & ~0xef) | (dest_ & 0xef)); t11_check_irqs(); }  //#define MTPS_M(d)   int dreg, dest, ea; GET_DW_##d; PSW = (PSW & ~0xef) | (dest & 0xef); t11_check_irqs()
        /* NEG: dst = -dst */
        void NEG_R(uint16_t op, register d) { /*int dreg, dest, result;*/      d.GET_DW_(op); CLR_NZVC(); result_ = -dest_; SETW_NZ(); if (dest_ == 0x8000) SET_V(); if (result_ != 0) SET_C(); PUT_DW_DREG(result_); }  //#define NEG_R(d)    int dreg, dest, result;     GET_DW_##d; CLR_NZVC; result = -dest; SETW_NZ; if (dest == 0x8000) SET_V; if (result) SET_C; PUT_DW_DREG(result)
        void NEG_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/  d.GET_DW_(op); CLR_NZVC(); result_ = -dest_; SETW_NZ(); if (dest_ == 0x8000) SET_V(); if (result_ != 0) SET_C(); PUT_DW_EA(result_); }  //#define NEG_M(d)    int dreg, dest, result, ea; GET_DW_##d; CLR_NZVC; result = -dest; SETW_NZ; if (dest == 0x8000) SET_V; if (result) SET_C; PUT_DW_EA(result)
        void NEGB_R(uint16_t op, register d) { /*int dreg, dest, result;*/     d.GET_DB_(op); CLR_NZVC(); result_ = -dest_; SETB_NZ(); if (dest_ == 0x80)   SET_V(); if (result_ != 0) SET_C(); PUT_DB_DREG(result_); }  //#define NEGB_R(d)   int dreg, dest, result;     GET_DB_##d; CLR_NZVC; result = -dest; SETB_NZ; if (dest == 0x80)   SET_V; if (result) SET_C; PUT_DB_DREG(result)
        void NEGB_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/ d.GET_DB_(op); CLR_NZVC(); result_ = -dest_; SETB_NZ(); if (dest_ == 0x80)   SET_V(); if (result_ != 0) SET_C(); PUT_DB_EA(result_); }  //#define NEGB_M(d)   int dreg, dest, result, ea; GET_DB_##d; CLR_NZVC; result = -dest; SETB_NZ; if (dest == 0x80)   SET_V; if (result) SET_C; PUT_DB_EA(result)
        /* ROL: dst = (dst << 1) | C; C = (dst >> 7) */
        void ROL_R(uint16_t op, register d) { /*int dreg, dest, result;*/      d.GET_DW_(op); result_ = (dest_ << 1) | GET_C; CLR_NZVC(); SETW_NZ(); PSW |= (uint8_t)((dest_ >> 15) & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DW_DREG(result_); }  //#define ROL_R(d)    int dreg, dest, result;     GET_DW_##d; result = (dest << 1) | GET_C; CLR_NZVC; SETW_NZ; PSW |= (dest >> 15) & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DW_DREG(result)
        void ROL_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/  d.GET_DW_(op); result_ = (dest_ << 1) | GET_C; CLR_NZVC(); SETW_NZ(); PSW |= (uint8_t)((dest_ >> 15) & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DW_EA(result_); }  //#define ROL_M(d)    int dreg, dest, result, ea; GET_DW_##d; result = (dest << 1) | GET_C; CLR_NZVC; SETW_NZ; PSW |= (dest >> 15) & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DW_EA(result)
        void ROLB_R(uint16_t op, register d) { /*int dreg, dest, result;*/     d.GET_DB_(op); result_ = (dest_ << 1) | GET_C; CLR_NZVC(); SETB_NZ(); PSW |= (uint8_t)((dest_ >> 7) & 1);  PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DB_DREG(result_); }  //#define ROLB_R(d)   int dreg, dest, result;     GET_DB_##d; result = (dest << 1) | GET_C; CLR_NZVC; SETB_NZ; PSW |= (dest >> 7) & 1;  PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DB_DREG(result)
        void ROLB_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/ d.GET_DB_(op); result_ = (dest_ << 1) | GET_C; CLR_NZVC(); SETB_NZ(); PSW |= (uint8_t)((dest_ >> 7) & 1);  PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DB_EA(result_); }  //#define ROLB_M(d)   int dreg, dest, result, ea; GET_DB_##d; result = (dest << 1) | GET_C; CLR_NZVC; SETB_NZ; PSW |= (dest >> 7) & 1;  PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DB_EA(result)
        /* ROR: dst = (dst >> 1) | (C << 7); C = dst & 1 */
        void ROR_R(uint16_t op, register d) { /*int dreg, dest, result;*/      d.GET_DW_(op); result_ = (dest_ >> 1) | (GET_C << 15); CLR_NZVC(); SETW_NZ(); PSW |= (uint8_t)(dest_ & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DW_DREG(result_); }  //#define ROR_R(d)    int dreg, dest, result;     GET_DW_##d; result = (dest >> 1) | (GET_C << 15); CLR_NZVC; SETW_NZ; PSW |= dest & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DW_DREG(result)
        void ROR_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/  d.GET_DW_(op); result_ = (dest_ >> 1) | (GET_C << 15); CLR_NZVC(); SETW_NZ(); PSW |= (uint8_t)(dest_ & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DW_EA(result_); }  //#define ROR_M(d)    int dreg, dest, result, ea; GET_DW_##d; result = (dest >> 1) | (GET_C << 15); CLR_NZVC; SETW_NZ; PSW |= dest & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DW_EA(result)
        void RORB_R(uint16_t op, register d) { /*int dreg, dest, result;*/     d.GET_DB_(op); result_ = (dest_ >> 1) | (GET_C << 7);  CLR_NZVC(); SETB_NZ(); PSW |= (uint8_t)(dest_ & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DB_DREG(result_); }  //#define RORB_R(d)   int dreg, dest, result;     GET_DB_##d; result = (dest >> 1) | (GET_C << 7);  CLR_NZVC; SETB_NZ; PSW |= dest & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DB_DREG(result)
        void RORB_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/ d.GET_DB_(op); result_ = (dest_ >> 1) | (GET_C << 7);  CLR_NZVC(); SETB_NZ(); PSW |= (uint8_t)(dest_ & 1); PSW |= (uint8_t)(((PSW << 1) ^ (PSW >> 2)) & 2); PUT_DB_EA(result_); }  //#define RORB_M(d)   int dreg, dest, result, ea; GET_DB_##d; result = (dest >> 1) | (GET_C << 7);  CLR_NZVC; SETB_NZ; PSW |= dest & 1; PSW |= ((PSW << 1) ^ (PSW >> 2)) & 2; PUT_DB_EA(result)
        /* SBC: dst -= C */
        void SBC_R(uint16_t op, register d) { /*int dreg, source, dest, result;*/      source_ = GET_C; d.GET_DW_(op); CLR_NZVC(); result_ = dest_ - source_; SETW_NZVC(); PUT_DW_DREG(result_); }  //#define SBC_R(d)    int dreg, source, dest, result;     source = GET_C; GET_DW_##d; CLR_NZVC; result = dest - source; SETW_NZVC; PUT_DW_DREG(result)
        void SBC_M(uint16_t op, register d) { /*int dreg, source, dest, result, ea;*/  source_ = GET_C; d.GET_DW_(op); CLR_NZVC(); result_ = dest_ - source_; SETW_NZVC(); PUT_DW_EA(result_); }  //#define SBC_M(d)    int dreg, source, dest, result, ea; source = GET_C; GET_DW_##d; CLR_NZVC; result = dest - source; SETW_NZVC; PUT_DW_EA(result)
        void SBCB_R(uint16_t op, register d) { /*int dreg, source, dest, result;*/     source_ = GET_C; d.GET_DB_(op); CLR_NZVC(); result_ = dest_ - source_; SETB_NZVC(); PUT_DB_DREG(result_); }  //#define SBCB_R(d)   int dreg, source, dest, result;     source = GET_C; GET_DB_##d; CLR_NZVC; result = dest - source; SETB_NZVC; PUT_DB_DREG(result)
        void SBCB_M(uint16_t op, register d) { /*int dreg, source, dest, result, ea;*/ source_ = GET_C; d.GET_DB_(op); CLR_NZVC(); result_ = dest_ - source_; SETB_NZVC(); PUT_DB_EA(result_); }  //#define SBCB_M(d)   int dreg, source, dest, result, ea; source = GET_C; GET_DB_##d; CLR_NZVC; result = dest - source; SETB_NZVC; PUT_DB_EA(result)
        /* SUB: dst -= src */
        void SUB_R(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result;*/     s.GET_SW_(op); d.GET_DW_(op); CLR_NZVC(); result_ = dest_ - source_; SETW_NZVC(); PUT_DW_DREG(result_); }  //#define SUB_R(s,d)  int sreg, dreg, source, dest, result;     GET_SW_##s; GET_DW_##d; CLR_NZVC; result = dest - source; SETW_NZVC; PUT_DW_DREG(result)
        void SUB_X(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/ s.GET_SW_(op); d.GET_DW_(op); CLR_NZVC(); result_ = dest_ - source_; SETW_NZVC(); PUT_DW_DREG(result_); }  //#define SUB_X(s,d)  int sreg, dreg, source, dest, result, ea; GET_SW_##s; GET_DW_##d; CLR_NZVC; result = dest - source; SETW_NZVC; PUT_DW_DREG(result)
        void SUB_M(uint16_t op, register s, register d) { /*int sreg, dreg, source, dest, result, ea;*/ s.GET_SW_(op); d.GET_DW_(op); CLR_NZVC(); result_ = dest_ - source_; SETW_NZVC(); PUT_DW_EA(result_); }  //#define SUB_M(s,d)  int sreg, dreg, source, dest, result, ea; GET_SW_##s; GET_DW_##d; CLR_NZVC; result = dest - source; SETW_NZVC; PUT_DW_EA(result)
        //#define SUBB_R(s,d) int sreg, dreg, source, dest, result;     GET_SB_##s; GET_DB_##d; CLR_NZVC; result = dest - source; SETB_NZVC; PUT_DB_DREG(result)
        //#define SUBB_X(s,d) int sreg, dreg, source, dest, result, ea; GET_SB_##s; GET_DB_##d; CLR_NZVC; result = dest - source; SETB_NZVC; PUT_DB_DREG(result)
        //#define SUBB_M(s,d) int sreg, dreg, source, dest, result, ea; GET_SB_##s; GET_DB_##d; CLR_NZVC; result = dest - source; SETB_NZVC; PUT_DB_EA(result)
        /* SWAB: dst = (dst >> 8) + (dst << 8) */
        void SWAB_R(uint16_t op, register d) { /*int dreg, dest, result;*/     d.GET_DW_(op); CLR_NZVC(); result_ = ((dest_ >> 8) & 0xff) + (dest_ << 8); SETB_NZ(); PUT_DW_DREG(result_); }  //#define SWAB_R(d)   int dreg, dest, result;     GET_DW_##d; CLR_NZVC; result = ((dest >> 8) & 0xff) + (dest << 8); SETB_NZ; PUT_DW_DREG(result)
        void SWAB_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/ d.GET_DW_(op); CLR_NZVC(); result_ = ((dest_ >> 8) & 0xff) + (dest_ << 8); SETB_NZ(); PUT_DW_EA(result_); }  //#define SWAB_M(d)   int dreg, dest, result, ea; GET_DW_##d; CLR_NZVC; result = ((dest >> 8) & 0xff) + (dest << 8); SETB_NZ; PUT_DW_EA(result)
        /* SXT: dst = sign-extend dst */
        void SXT_M(uint16_t op, register d) { /*int dreg, result, ea;*/ CLR_ZV(); if (GET_N != 0) result_ = -1; else { result_ = 0; SET_Z(); } d.PUT_DWT_(op, result_); }  //#define SXT_M(d)    int dreg, result, ea; CLR_ZV; if (GET_N) result = -1; else { result = 0; SET_Z; } PUT_DWT_##d(result)
        void SXT_R(uint16_t op, register d) { /*int dreg, result;*/     CLR_ZV(); if (GET_N != 0) result_ = -1; else { result_ = 0; SET_Z(); } d.PUT_DW_(op, result_); }  //#define SXT_R(d)    int dreg, result;     CLR_ZV; if (GET_N) result = -1; else { result = 0; SET_Z; } PUT_DW_##d(result)
        /* TST: dst = ~dst */
        void TST_R(uint16_t op, register d) { /*int dreg, dest, result;*/      d.GET_DW_(op); CLR_NZVC(); result_ = dest_; SETW_NZ(); }  //#define TST_R(d)    int dreg, dest, result;     GET_DW_##d; CLR_NZVC; result = dest; SETW_NZ;
        void TST_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/  d.GET_DW_(op); CLR_NZVC(); result_ = dest_; SETW_NZ(); }  //#define TST_M(d)    int dreg, dest, result, ea; GET_DW_##d; CLR_NZVC; result = dest; SETW_NZ;
        void TSTB_R(uint16_t op, register d) { /*int dreg, dest, result;*/     d.GET_DB_(op); CLR_NZVC(); result_ = dest_; SETB_NZ(); }  //#define TSTB_R(d)   int dreg, dest, result;     GET_DB_##d; CLR_NZVC; result = dest; SETB_NZ;
        void TSTB_M(uint16_t op, register d) { /*int dreg, dest, result, ea;*/ d.GET_DB_(op); CLR_NZVC(); result_ = dest_; SETB_NZ(); }  //#define TSTB_M(d)   int dreg, dest, result, ea; GET_DB_##d; CLR_NZVC; result = dest; SETB_NZ;
        /* XOR: dst ^= src */
        void XOR_R(uint16_t op, register d) { /*int sreg, dreg, source, dest, result;*/     GET_SREG(op); source_ = REGW(sreg_); d.GET_DW_(op); CLR_NZV(); result_ = dest_ ^ source_; SETW_NZ(); PUT_DW_DREG(result_); }  //#define XOR_R(d)    int sreg, dreg, source, dest, result;     GET_SREG; source = REGW(sreg); GET_DW_##d; CLR_NZV; result = dest ^ source; SETW_NZ; PUT_DW_DREG(result)
        void XOR_M(uint16_t op, register d) { /*int sreg, dreg, source, dest, result, ea;*/ GET_SREG(op); source_ = REGW(sreg_); d.GET_DW_(op); CLR_NZV(); result_ = dest_ ^ source_; SETW_NZ(); PUT_DW_EA(result_); }  //#define XOR_M(d)    int sreg, dreg, source, dest, result, ea; GET_SREG; source = REGW(sreg); GET_DW_##d; CLR_NZV; result = dest ^ source; SETW_NZ; PUT_DW_EA(result)



        void op_0000(uint16_t op)
        {
            switch (op & 0x3f)
            {
                case 0x00:  /* HALT  */ halt(op); break;
                case 0x01:  /* WAIT  */ m_icount.i = 0; m_wait_state = 1; break;
                case 0x02:  /* RTI   */ m_icount.i -= 24; PC = (uint16_t)POP(); PSW = (uint8_t)POP(); t11_check_irqs(); break;
                case 0x03:  /* BPT   */ m_icount.i -= 48; PUSH(PSW); PUSH(PC); PC = (uint16_t)RWORD(0x0c); PSW = (uint8_t)RWORD(0x0e); t11_check_irqs(); break;
                case 0x04:  /* IOT   */ m_icount.i -= 48; PUSH(PSW); PUSH(PC); PC = (uint16_t)RWORD(0x10); PSW = (uint8_t)RWORD(0x12); t11_check_irqs(); break;
                case 0x05:  /* RESET */ m_out_reset_func.op(ASSERT_LINE); m_out_reset_func.op(CLEAR_LINE); m_icount.i -= 110; break;
                case 0x06:  /* RTT   */ m_icount.i -= 33; PC = (uint16_t)POP(); PSW = (uint8_t)POP(); t11_check_irqs(); break;
                case 0x07:  /* MFPT  */ REGB(0) = 4; break;

                default:    illegal(op); break;
            }
        }

        void halt(uint16_t op)
        {
            m_icount.i -= 48;
            PUSH(PSW);
            PUSH(PC);
            PC = (uint16_t)(m_initial_pc + 4);
            PSW = unchecked((uint8_t)0340);
            t11_check_irqs();
        }

        void illegal(uint16_t op)
        {
            m_icount.i -= 48;
            PUSH(PSW);
            PUSH(PC);
            PC = (uint16_t)RWORD(0x08);
            PSW = (uint8_t)RWORD(0x0a);
            t11_check_irqs();
        }

        void mark(uint16_t op)
        {
            m_icount.i -= 36;

            SP = (uint16_t)(PC + 2 * (op & 0x3f));
            PC = REGW(5);
            REGW(5) = (uint16_t)POP();
        }

        void jmp_rgd(uint16_t op)       { m_icount.i -= 15; { JMP(op, RGD); } }
        void jmp_in(uint16_t op)        { m_icount.i -= 18; { JMP(op, IN);  } }
        void jmp_ind(uint16_t op)       { m_icount.i -= 18; { JMP(op, IND); } }
        void jmp_de(uint16_t op)        { m_icount.i -= 18; { JMP(op, DE);  } }
        void jmp_ded(uint16_t op)       { m_icount.i -= 21; { JMP(op, DED); } }
        void jmp_ix(uint16_t op)        { m_icount.i -= 21; { JMP(op, IX);  } }
        void jmp_ixd(uint16_t op)       { m_icount.i -= 27; { JMP(op, IXD); } }

        void rts(uint16_t op)
        {
            //int dreg;
            m_icount.i -= 21;
            GET_DREG(op);
            PC = (uint16_t)REGD(dreg_);
            REGW(dreg_) = (uint16_t)POP();
        }

        void ccc(uint16_t op)           { m_icount.i -= 18; { PSW &= (uint8_t)~(op & 15); } }
        void scc(uint16_t op)           { m_icount.i -= 18; { PSW |=  (uint8_t)(op & 15); } }

        void swab_rg(uint16_t op)       { m_icount.i -= 12; { SWAB_R(op, RG); } }
        void swab_rgd(uint16_t op)      { m_icount.i -= 21; { SWAB_M(op, RGD); } }
        void swab_in(uint16_t op)       { m_icount.i -= 21; { SWAB_M(op, IN); } }
        void swab_ind(uint16_t op)      { m_icount.i -= 27; { SWAB_M(op, IND); } }
        void swab_de(uint16_t op)       { m_icount.i -= 24; { SWAB_M(op, DE); } }
        void swab_ded(uint16_t op)      { m_icount.i -= 30; { SWAB_M(op, DED); } }
        void swab_ix(uint16_t op)       { m_icount.i -= 30; { SWAB_M(op, IX); } }
        void swab_ixd(uint16_t op)      { m_icount.i -= 36; { SWAB_M(op, IXD); } }

        void br(uint16_t op)            { m_icount.i -= 12; { BR(op, 1); } }
        void bne(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_Z == 0); } }
        void beq(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_Z); } }
        void bge(uint16_t op)           { m_icount.i -= 12; { BR(op, ((GET_N >> 2) ^ GET_V) == 0); } }
        void blt(uint16_t op)           { m_icount.i -= 12; { BR(op, ((GET_N >> 2) ^ GET_V)); } }
        void bgt(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_Z == 0 && ((GET_N >> 2) ^ GET_V) == 0); } }
        void ble(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_Z != 0 || ((GET_N >> 2) ^ GET_V) != 0); } }

        void jsr_rgd(uint16_t op)       { m_icount.i -= 27; { JSR(op, RGD); } }
        void jsr_in(uint16_t op)        { m_icount.i -= 30; { JSR(op, IN);  } }
        void jsr_ind(uint16_t op)       { m_icount.i -= 30; { JSR(op, IND); } }
        void jsr_de(uint16_t op)        { m_icount.i -= 30; { JSR(op, DE);  } }
        void jsr_ded(uint16_t op)       { m_icount.i -= 33; { JSR(op, DED); } }
        void jsr_ix(uint16_t op)        { m_icount.i -= 33; { JSR(op, IX);  } }
        void jsr_ixd(uint16_t op)       { m_icount.i -= 39; { JSR(op, IXD); } }

        void clr_rg(uint16_t op)        { m_icount.i -= 12; { CLR_R(op, RG);  } }
        void clr_rgd(uint16_t op)       { m_icount.i -= 21; { CLR_M(op, RGD); } }
        void clr_in(uint16_t op)        { m_icount.i -= 21; { CLR_M(op, IN);  } }
        void clr_ind(uint16_t op)       { m_icount.i -= 27; { CLR_M(op, IND); } }
        void clr_de(uint16_t op)        { m_icount.i -= 24; { CLR_M(op, DE);  } }
        void clr_ded(uint16_t op)       { m_icount.i -= 30; { CLR_M(op, DED); } }
        void clr_ix(uint16_t op)        { m_icount.i -= 30; { CLR_M(op, IX);  } }
        void clr_ixd(uint16_t op)       { m_icount.i -= 36; { CLR_M(op, IXD); } }

        void com_rg(uint16_t op)        { m_icount.i -= 12; { COM_R(op, RG);  } }
        void com_rgd(uint16_t op)       { m_icount.i -= 21; { COM_M(op, RGD); } }
        void com_in(uint16_t op)        { m_icount.i -= 21; { COM_M(op, IN);  } }
        void com_ind(uint16_t op)       { m_icount.i -= 27; { COM_M(op, IND); } }
        void com_de(uint16_t op)        { m_icount.i -= 24; { COM_M(op, DE);  } }
        void com_ded(uint16_t op)       { m_icount.i -= 30; { COM_M(op, DED); } }
        void com_ix(uint16_t op)        { m_icount.i -= 30; { COM_M(op, IX);  } }
        void com_ixd(uint16_t op)       { m_icount.i -= 36; { COM_M(op, IXD); } }

        void inc_rg(uint16_t op)        { m_icount.i -= 12; { INC_R(op, RG);  } }
        void inc_rgd(uint16_t op)       { m_icount.i -= 21; { INC_M(op, RGD); } }
        void inc_in(uint16_t op)        { m_icount.i -= 21; { INC_M(op, IN);  } }
        void inc_ind(uint16_t op)       { m_icount.i -= 27; { INC_M(op, IND); } }
        void inc_de(uint16_t op)        { m_icount.i -= 24; { INC_M(op, DE);  } }
        void inc_ded(uint16_t op)       { m_icount.i -= 30; { INC_M(op, DED); } }
        void inc_ix(uint16_t op)        { m_icount.i -= 30; { INC_M(op, IX);  } }
        void inc_ixd(uint16_t op)       { m_icount.i -= 36; { INC_M(op, IXD); } }

        void dec_rg(uint16_t op)        { m_icount.i -= 12; { DEC_R(op, RG);  } }
        void dec_rgd(uint16_t op)       { m_icount.i -= 21; { DEC_M(op, RGD); } }
        void dec_in(uint16_t op)        { m_icount.i -= 21; { DEC_M(op, IN);  } }
        void dec_ind(uint16_t op)       { m_icount.i -= 27; { DEC_M(op, IND); } }
        void dec_de(uint16_t op)        { m_icount.i -= 24; { DEC_M(op, DE);  } }
        void dec_ded(uint16_t op)       { m_icount.i -= 30; { DEC_M(op, DED); } }
        void dec_ix(uint16_t op)        { m_icount.i -= 30; { DEC_M(op, IX);  } }
        void dec_ixd(uint16_t op)       { m_icount.i -= 36; { DEC_M(op, IXD); } }

        void neg_rg(uint16_t op)        { m_icount.i -= 12; { NEG_R(op, RG);  } }
        void neg_rgd(uint16_t op)       { m_icount.i -= 21; { NEG_M(op, RGD); } }
        void neg_in(uint16_t op)        { m_icount.i -= 21; { NEG_M(op, IN);  } }
        void neg_ind(uint16_t op)       { m_icount.i -= 27; { NEG_M(op, IND); } }
        void neg_de(uint16_t op)        { m_icount.i -= 24; { NEG_M(op, DE);  } }
        void neg_ded(uint16_t op)       { m_icount.i -= 30; { NEG_M(op, DED); } }
        void neg_ix(uint16_t op)        { m_icount.i -= 30; { NEG_M(op, IX);  } }
        void neg_ixd(uint16_t op)       { m_icount.i -= 36; { NEG_M(op, IXD); } }

        void adc_rg(uint16_t op)        { m_icount.i -= 12; { ADC_R(op, RG);  } }
        void adc_rgd(uint16_t op)       { m_icount.i -= 21; { ADC_M(op, RGD); } }
        void adc_in(uint16_t op)        { m_icount.i -= 21; { ADC_M(op, IN);  } }
        void adc_ind(uint16_t op)       { m_icount.i -= 27; { ADC_M(op, IND); } }
        void adc_de(uint16_t op)        { m_icount.i -= 24; { ADC_M(op, DE);  } }
        void adc_ded(uint16_t op)       { m_icount.i -= 30; { ADC_M(op, DED); } }
        void adc_ix(uint16_t op)        { m_icount.i -= 30; { ADC_M(op, IX);  } }
        void adc_ixd(uint16_t op)       { m_icount.i -= 36; { ADC_M(op, IXD); } }

        void sbc_rg(uint16_t op)        { m_icount.i -= 12; { SBC_R(op, RG);  } }
        void sbc_rgd(uint16_t op)       { m_icount.i -= 21; { SBC_M(op, RGD); } }
        void sbc_in(uint16_t op)        { m_icount.i -= 21; { SBC_M(op, IN);  } }
        void sbc_ind(uint16_t op)       { m_icount.i -= 27; { SBC_M(op, IND); } }
        void sbc_de(uint16_t op)        { m_icount.i -= 24; { SBC_M(op, DE);  } }
        void sbc_ded(uint16_t op)       { m_icount.i -= 30; { SBC_M(op, DED); } }
        void sbc_ix(uint16_t op)        { m_icount.i -= 30; { SBC_M(op, IX);  } }
        void sbc_ixd(uint16_t op)       { m_icount.i -= 36; { SBC_M(op, IXD); } }

        void tst_rg(uint16_t op)        { m_icount.i -= 12; { TST_R(op, RG);  } }
        void tst_rgd(uint16_t op)       { m_icount.i -= 18; { TST_M(op, RGD); } }
        void tst_in(uint16_t op)        { m_icount.i -= 18; { TST_M(op, IN);  } }
        void tst_ind(uint16_t op)       { m_icount.i -= 24; { TST_M(op, IND); } }
        void tst_de(uint16_t op)        { m_icount.i -= 21; { TST_M(op, DE);  } }
        void tst_ded(uint16_t op)       { m_icount.i -= 27; { TST_M(op, DED); } }
        void tst_ix(uint16_t op)        { m_icount.i -= 27; { TST_M(op, IX);  } }
        void tst_ixd(uint16_t op)       { m_icount.i -= 33; { TST_M(op, IXD); } }

        void ror_rg(uint16_t op)        { m_icount.i -= 12; { ROR_R(op, RG);  } }
        void ror_rgd(uint16_t op)       { m_icount.i -= 21; { ROR_M(op, RGD); } }
        void ror_in(uint16_t op)        { m_icount.i -= 21; { ROR_M(op, IN);  } }
        void ror_ind(uint16_t op)       { m_icount.i -= 27; { ROR_M(op, IND); } }
        void ror_de(uint16_t op)        { m_icount.i -= 24; { ROR_M(op, DE);  } }
        void ror_ded(uint16_t op)       { m_icount.i -= 30; { ROR_M(op, DED); } }
        void ror_ix(uint16_t op)        { m_icount.i -= 30; { ROR_M(op, IX);  } }
        void ror_ixd(uint16_t op)       { m_icount.i -= 36; { ROR_M(op, IXD); } }

        void rol_rg(uint16_t op)        { m_icount.i -= 12; { ROL_R(op, RG);  } }
        void rol_rgd(uint16_t op)       { m_icount.i -= 21; { ROL_M(op, RGD); } }
        void rol_in(uint16_t op)        { m_icount.i -= 21; { ROL_M(op, IN);  } }
        void rol_ind(uint16_t op)       { m_icount.i -= 27; { ROL_M(op, IND); } }
        void rol_de(uint16_t op)        { m_icount.i -= 24; { ROL_M(op, DE);  } }
        void rol_ded(uint16_t op)       { m_icount.i -= 30; { ROL_M(op, DED); } }
        void rol_ix(uint16_t op)        { m_icount.i -= 30; { ROL_M(op, IX);  } }
        void rol_ixd(uint16_t op)       { m_icount.i -= 36; { ROL_M(op, IXD); } }

        void asr_rg(uint16_t op)        { m_icount.i -= 12; { ASR_R(op, RG);  } }
        void asr_rgd(uint16_t op)       { m_icount.i -= 21; { ASR_M(op, RGD); } }
        void asr_in(uint16_t op)        { m_icount.i -= 21; { ASR_M(op, IN);  } }
        void asr_ind(uint16_t op)       { m_icount.i -= 27; { ASR_M(op, IND); } }
        void asr_de(uint16_t op)        { m_icount.i -= 24; { ASR_M(op, DE);  } }
        void asr_ded(uint16_t op)       { m_icount.i -= 30; { ASR_M(op, DED); } }
        void asr_ix(uint16_t op)        { m_icount.i -= 30; { ASR_M(op, IX);  } }
        void asr_ixd(uint16_t op)       { m_icount.i -= 36; { ASR_M(op, IXD); } }

        void asl_rg(uint16_t op)        { m_icount.i -= 12; { ASL_R(op, RG);  } }
        void asl_rgd(uint16_t op)       { m_icount.i -= 21; { ASL_M(op, RGD); } }
        void asl_in(uint16_t op)        { m_icount.i -= 21; { ASL_M(op, IN);  } }
        void asl_ind(uint16_t op)       { m_icount.i -= 27; { ASL_M(op, IND); } }
        void asl_de(uint16_t op)        { m_icount.i -= 24; { ASL_M(op, DE);  } }
        void asl_ded(uint16_t op)       { m_icount.i -= 30; { ASL_M(op, DED); } }
        void asl_ix(uint16_t op)        { m_icount.i -= 30; { ASL_M(op, IX);  } }
        void asl_ixd(uint16_t op)       { m_icount.i -= 36; { ASL_M(op, IXD); } }

        void sxt_rg(uint16_t op)        { m_icount.i -= 12; { SXT_R(op, RG);  } }
        void sxt_rgd(uint16_t op)       { m_icount.i -= 21; { SXT_M(op, RGD); } }
        void sxt_in(uint16_t op)        { m_icount.i -= 21; { SXT_M(op, IN);  } }
        void sxt_ind(uint16_t op)       { m_icount.i -= 27; { SXT_M(op, IND); } }
        void sxt_de(uint16_t op)        { m_icount.i -= 24; { SXT_M(op, DE);  } }
        void sxt_ded(uint16_t op)       { m_icount.i -= 30; { SXT_M(op, DED); } }
        void sxt_ix(uint16_t op)        { m_icount.i -= 30; { SXT_M(op, IX);  } }
        void sxt_ixd(uint16_t op)       { m_icount.i -= 36; { SXT_M(op, IXD); } }

        void mov_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { MOV_R(op, RG,RG);   } }
        void mov_rg_rgd(uint16_t op)    { m_icount.i -=  9+12; { MOV_M(op, RG,RGD);  } }
        void mov_rg_in(uint16_t op)     { m_icount.i -=  9+12; { MOV_M(op, RG,IN);   } }
        void mov_rg_ind(uint16_t op)    { m_icount.i -=  9+18; { MOV_M(op, RG,IND);  } }
        void mov_rg_de(uint16_t op)     { m_icount.i -=  9+15; { MOV_M(op, RG,DE);   } }
        void mov_rg_ded(uint16_t op)    { m_icount.i -=  9+21; { MOV_M(op, RG,DED);  } }
        void mov_rg_ix(uint16_t op)     { m_icount.i -=  9+21; { MOV_M(op, RG,IX);   } }
        void mov_rg_ixd(uint16_t op)    { m_icount.i -=  9+27; { MOV_M(op, RG,IXD);  } }
        void mov_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { MOV_M(op, RGD,RG);  } }
        void mov_rgd_rgd(uint16_t op)   { m_icount.i -= 15+12; { MOV_M(op, RGD,RGD); } }
        void mov_rgd_in(uint16_t op)    { m_icount.i -= 15+12; { MOV_M(op, RGD,IN);  } }
        void mov_rgd_ind(uint16_t op)   { m_icount.i -= 15+18; { MOV_M(op, RGD,IND); } }
        void mov_rgd_de(uint16_t op)    { m_icount.i -= 15+15; { MOV_M(op, RGD,DE);  } }
        void mov_rgd_ded(uint16_t op)   { m_icount.i -= 15+21; { MOV_M(op, RGD,DED); } }
        void mov_rgd_ix(uint16_t op)    { m_icount.i -= 15+21; { MOV_M(op, RGD,IX);  } }
        void mov_rgd_ixd(uint16_t op)   { m_icount.i -= 15+27; { MOV_M(op, RGD,IXD); } }
        void mov_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { MOV_M(op, IN,RG);   } }
        void mov_in_rgd(uint16_t op)    { m_icount.i -= 15+12; { MOV_M(op, IN,RGD);  } }
        void mov_in_in(uint16_t op)     { m_icount.i -= 15+12; { MOV_M(op, IN,IN);   } }
        void mov_in_ind(uint16_t op)    { m_icount.i -= 15+18; { MOV_M(op, IN,IND);  } }
        void mov_in_de(uint16_t op)     { m_icount.i -= 15+15; { MOV_M(op, IN,DE);   } }
        void mov_in_ded(uint16_t op)    { m_icount.i -= 15+21; { MOV_M(op, IN,DED);  } }
        void mov_in_ix(uint16_t op)     { m_icount.i -= 15+21; { MOV_M(op, IN,IX);   } }
        void mov_in_ixd(uint16_t op)    { m_icount.i -= 15+27; { MOV_M(op, IN,IXD);  } }
        void mov_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { MOV_M(op, IND,RG);  } }
        void mov_ind_rgd(uint16_t op)   { m_icount.i -= 21+12; { MOV_M(op, IND,RGD); } }
        void mov_ind_in(uint16_t op)    { m_icount.i -= 21+12; { MOV_M(op, IND,IN);  } }
        void mov_ind_ind(uint16_t op)   { m_icount.i -= 21+18; { MOV_M(op, IND,IND); } }
        void mov_ind_de(uint16_t op)    { m_icount.i -= 21+15; { MOV_M(op, IND,DE);  } }
        void mov_ind_ded(uint16_t op)   { m_icount.i -= 21+21; { MOV_M(op, IND,DED); } }
        void mov_ind_ix(uint16_t op)    { m_icount.i -= 21+21; { MOV_M(op, IND,IX);  } }
        void mov_ind_ixd(uint16_t op)   { m_icount.i -= 21+27; { MOV_M(op, IND,IXD); } }
        void mov_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { MOV_M(op, DE,RG);   } }
        void mov_de_rgd(uint16_t op)    { m_icount.i -= 18+12; { MOV_M(op, DE,RGD);  } }
        void mov_de_in(uint16_t op)     { m_icount.i -= 18+12; { MOV_M(op, DE,IN);   } }
        void mov_de_ind(uint16_t op)    { m_icount.i -= 18+18; { MOV_M(op, DE,IND);  } }
        void mov_de_de(uint16_t op)     { m_icount.i -= 18+15; { MOV_M(op, DE,DE);   } }
        void mov_de_ded(uint16_t op)    { m_icount.i -= 18+21; { MOV_M(op, DE,DED);  } }
        void mov_de_ix(uint16_t op)     { m_icount.i -= 18+21; { MOV_M(op, DE,IX);   } }
        void mov_de_ixd(uint16_t op)    { m_icount.i -= 18+27; { MOV_M(op, DE,IXD);  } }
        void mov_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { MOV_M(op, DED,RG);  } }
        void mov_ded_rgd(uint16_t op)   { m_icount.i -= 24+12; { MOV_M(op, DED,RGD); } }
        void mov_ded_in(uint16_t op)    { m_icount.i -= 24+12; { MOV_M(op, DED,IN);  } }
        void mov_ded_ind(uint16_t op)   { m_icount.i -= 24+18; { MOV_M(op, DED,IND); } }
        void mov_ded_de(uint16_t op)    { m_icount.i -= 24+15; { MOV_M(op, DED,DE);  } }
        void mov_ded_ded(uint16_t op)   { m_icount.i -= 24+21; { MOV_M(op, DED,DED); } }
        void mov_ded_ix(uint16_t op)    { m_icount.i -= 24+21; { MOV_M(op, DED,IX);  } }
        void mov_ded_ixd(uint16_t op)   { m_icount.i -= 24+27; { MOV_M(op, DED,IXD); } }
        void mov_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { MOV_M(op, IX,RG);   } }
        void mov_ix_rgd(uint16_t op)    { m_icount.i -= 24+12; { MOV_M(op, IX,RGD);  } }
        void mov_ix_in(uint16_t op)     { m_icount.i -= 24+12; { MOV_M(op, IX,IN);   } }
        void mov_ix_ind(uint16_t op)    { m_icount.i -= 24+18; { MOV_M(op, IX,IND);  } }
        void mov_ix_de(uint16_t op)     { m_icount.i -= 24+15; { MOV_M(op, IX,DE);   } }
        void mov_ix_ded(uint16_t op)    { m_icount.i -= 24+21; { MOV_M(op, IX,DED);  } }
        void mov_ix_ix(uint16_t op)     { m_icount.i -= 24+21; { MOV_M(op, IX,IX);   } }
        void mov_ix_ixd(uint16_t op)    { m_icount.i -= 24+27; { MOV_M(op, IX,IXD);  } }
        void mov_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { MOV_M(op, IXD,RG);  } }
        void mov_ixd_rgd(uint16_t op)   { m_icount.i -= 30+12; { MOV_M(op, IXD,RGD); } }
        void mov_ixd_in(uint16_t op)    { m_icount.i -= 30+12; { MOV_M(op, IXD,IN);  } }
        void mov_ixd_ind(uint16_t op)   { m_icount.i -= 30+18; { MOV_M(op, IXD,IND); } }
        void mov_ixd_de(uint16_t op)    { m_icount.i -= 30+15; { MOV_M(op, IXD,DE);  } }
        void mov_ixd_ded(uint16_t op)   { m_icount.i -= 30+21; { MOV_M(op, IXD,DED); } }
        void mov_ixd_ix(uint16_t op)    { m_icount.i -= 30+21; { MOV_M(op, IXD,IX);  } }
        void mov_ixd_ixd(uint16_t op)   { m_icount.i -= 30+27; { MOV_M(op, IXD,IXD); } }

        void cmp_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { CMP_R(op, RG,RG);   } }
        void cmp_rg_rgd(uint16_t op)    { m_icount.i -=  9+ 9; { CMP_M(op, RG,RGD);  } }
        void cmp_rg_in(uint16_t op)     { m_icount.i -=  9+ 9; { CMP_M(op, RG,IN);   } }
        void cmp_rg_ind(uint16_t op)    { m_icount.i -=  9+15; { CMP_M(op, RG,IND);  } }
        void cmp_rg_de(uint16_t op)     { m_icount.i -=  9+12; { CMP_M(op, RG,DE);   } }
        void cmp_rg_ded(uint16_t op)    { m_icount.i -=  9+18; { CMP_M(op, RG,DED);  } }
        void cmp_rg_ix(uint16_t op)     { m_icount.i -=  9+18; { CMP_M(op, RG,IX);   } }
        void cmp_rg_ixd(uint16_t op)    { m_icount.i -=  9+24; { CMP_M(op, RG,IXD);  } }
        void cmp_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { CMP_M(op, RGD,RG);  } }
        void cmp_rgd_rgd(uint16_t op)   { m_icount.i -= 15+ 9; { CMP_M(op, RGD,RGD); } }
        void cmp_rgd_in(uint16_t op)    { m_icount.i -= 15+ 9; { CMP_M(op, RGD,IN);  } }
        void cmp_rgd_ind(uint16_t op)   { m_icount.i -= 15+15; { CMP_M(op, RGD,IND); } }
        void cmp_rgd_de(uint16_t op)    { m_icount.i -= 15+12; { CMP_M(op, RGD,DE);  } }
        void cmp_rgd_ded(uint16_t op)   { m_icount.i -= 15+18; { CMP_M(op, RGD,DED); } }
        void cmp_rgd_ix(uint16_t op)    { m_icount.i -= 15+18; { CMP_M(op, RGD,IX);  } }
        void cmp_rgd_ixd(uint16_t op)   { m_icount.i -= 15+24; { CMP_M(op, RGD,IXD); } }
        void cmp_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { CMP_M(op, IN,RG);   } }
        void cmp_in_rgd(uint16_t op)    { m_icount.i -= 15+ 9; { CMP_M(op, IN,RGD);  } }
        void cmp_in_in(uint16_t op)     { m_icount.i -= 15+ 9; { CMP_M(op, IN,IN);   } }
        void cmp_in_ind(uint16_t op)    { m_icount.i -= 15+15; { CMP_M(op, IN,IND);  } }
        void cmp_in_de(uint16_t op)     { m_icount.i -= 15+12; { CMP_M(op, IN,DE);   } }
        void cmp_in_ded(uint16_t op)    { m_icount.i -= 15+18; { CMP_M(op, IN,DED);  } }
        void cmp_in_ix(uint16_t op)     { m_icount.i -= 15+18; { CMP_M(op, IN,IX);   } }
        void cmp_in_ixd(uint16_t op)    { m_icount.i -= 15+24; { CMP_M(op, IN,IXD);  } }
        void cmp_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { CMP_M(op, IND,RG);  } }
        void cmp_ind_rgd(uint16_t op)   { m_icount.i -= 21+ 9; { CMP_M(op, IND,RGD); } }
        void cmp_ind_in(uint16_t op)    { m_icount.i -= 21+ 9; { CMP_M(op, IND,IN);  } }
        void cmp_ind_ind(uint16_t op)   { m_icount.i -= 21+15; { CMP_M(op, IND,IND); } }
        void cmp_ind_de(uint16_t op)    { m_icount.i -= 21+12; { CMP_M(op, IND,DE);  } }
        void cmp_ind_ded(uint16_t op)   { m_icount.i -= 21+18; { CMP_M(op, IND,DED); } }
        void cmp_ind_ix(uint16_t op)    { m_icount.i -= 21+18; { CMP_M(op, IND,IX);  } }
        void cmp_ind_ixd(uint16_t op)   { m_icount.i -= 21+24; { CMP_M(op, IND,IXD); } }
        void cmp_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { CMP_M(op, DE,RG);   } }
        void cmp_de_rgd(uint16_t op)    { m_icount.i -= 18+ 9; { CMP_M(op, DE,RGD);  } }
        void cmp_de_in(uint16_t op)     { m_icount.i -= 18+ 9; { CMP_M(op, DE,IN);   } }
        void cmp_de_ind(uint16_t op)    { m_icount.i -= 18+15; { CMP_M(op, DE,IND);  } }
        void cmp_de_de(uint16_t op)     { m_icount.i -= 18+12; { CMP_M(op, DE,DE);   } }
        void cmp_de_ded(uint16_t op)    { m_icount.i -= 18+18; { CMP_M(op, DE,DED);  } }
        void cmp_de_ix(uint16_t op)     { m_icount.i -= 18+18; { CMP_M(op, DE,IX);   } }
        void cmp_de_ixd(uint16_t op)    { m_icount.i -= 18+24; { CMP_M(op, DE,IXD);  } }
        void cmp_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { CMP_M(op, DED,RG);  } }
        void cmp_ded_rgd(uint16_t op)   { m_icount.i -= 24+ 9; { CMP_M(op, DED,RGD); } }
        void cmp_ded_in(uint16_t op)    { m_icount.i -= 24+ 9; { CMP_M(op, DED,IN);  } }
        void cmp_ded_ind(uint16_t op)   { m_icount.i -= 24+15; { CMP_M(op, DED,IND); } }
        void cmp_ded_de(uint16_t op)    { m_icount.i -= 24+12; { CMP_M(op, DED,DE);  } }
        void cmp_ded_ded(uint16_t op)   { m_icount.i -= 24+18; { CMP_M(op, DED,DED); } }
        void cmp_ded_ix(uint16_t op)    { m_icount.i -= 24+18; { CMP_M(op, DED,IX);  } }
        void cmp_ded_ixd(uint16_t op)   { m_icount.i -= 24+24; { CMP_M(op, DED,IXD); } }
        void cmp_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { CMP_M(op, IX,RG);   } }
        void cmp_ix_rgd(uint16_t op)    { m_icount.i -= 24+ 9; { CMP_M(op, IX,RGD);  } }
        void cmp_ix_in(uint16_t op)     { m_icount.i -= 24+ 9; { CMP_M(op, IX,IN);   } }
        void cmp_ix_ind(uint16_t op)    { m_icount.i -= 24+15; { CMP_M(op, IX,IND);  } }
        void cmp_ix_de(uint16_t op)     { m_icount.i -= 24+12; { CMP_M(op, IX,DE);   } }
        void cmp_ix_ded(uint16_t op)    { m_icount.i -= 24+18; { CMP_M(op, IX,DED);  } }
        void cmp_ix_ix(uint16_t op)     { m_icount.i -= 24+18; { CMP_M(op, IX,IX);   } }
        void cmp_ix_ixd(uint16_t op)    { m_icount.i -= 24+24; { CMP_M(op, IX,IXD);  } }
        void cmp_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { CMP_M(op, IXD,RG);  } }
        void cmp_ixd_rgd(uint16_t op)   { m_icount.i -= 30+ 9; { CMP_M(op, IXD,RGD); } }
        void cmp_ixd_in(uint16_t op)    { m_icount.i -= 30+ 9; { CMP_M(op, IXD,IN);  } }
        void cmp_ixd_ind(uint16_t op)   { m_icount.i -= 30+15; { CMP_M(op, IXD,IND); } }
        void cmp_ixd_de(uint16_t op)    { m_icount.i -= 30+12; { CMP_M(op, IXD,DE);  } }
        void cmp_ixd_ded(uint16_t op)   { m_icount.i -= 30+18; { CMP_M(op, IXD,DED); } }
        void cmp_ixd_ix(uint16_t op)    { m_icount.i -= 30+18; { CMP_M(op, IXD,IX);  } }
        void cmp_ixd_ixd(uint16_t op)   { m_icount.i -= 30+24; { CMP_M(op, IXD,IXD); } }

        void bit_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { BIT_R(op, RG,RG);   } }
        void bit_rg_rgd(uint16_t op)    { m_icount.i -=  9+ 9; { BIT_M(op, RG,RGD);  } }
        void bit_rg_in(uint16_t op)     { m_icount.i -=  9+ 9; { BIT_M(op, RG,IN);   } }
        void bit_rg_ind(uint16_t op)    { m_icount.i -=  9+15; { BIT_M(op, RG,IND);  } }
        void bit_rg_de(uint16_t op)     { m_icount.i -=  9+12; { BIT_M(op, RG,DE);   } }
        void bit_rg_ded(uint16_t op)    { m_icount.i -=  9+18; { BIT_M(op, RG,DED);  } }
        void bit_rg_ix(uint16_t op)     { m_icount.i -=  9+18; { BIT_M(op, RG,IX);   } }
        void bit_rg_ixd(uint16_t op)    { m_icount.i -=  9+24; { BIT_M(op, RG,IXD);  } }
        void bit_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { BIT_M(op, RGD,RG);  } }
        void bit_rgd_rgd(uint16_t op)   { m_icount.i -= 15+ 9; { BIT_M(op, RGD,RGD); } }
        void bit_rgd_in(uint16_t op)    { m_icount.i -= 15+ 9; { BIT_M(op, RGD,IN);  } }
        void bit_rgd_ind(uint16_t op)   { m_icount.i -= 15+15; { BIT_M(op, RGD,IND); } }
        void bit_rgd_de(uint16_t op)    { m_icount.i -= 15+12; { BIT_M(op, RGD,DE);  } }
        void bit_rgd_ded(uint16_t op)   { m_icount.i -= 15+18; { BIT_M(op, RGD,DED); } }
        void bit_rgd_ix(uint16_t op)    { m_icount.i -= 15+18; { BIT_M(op, RGD,IX);  } }
        void bit_rgd_ixd(uint16_t op)   { m_icount.i -= 15+24; { BIT_M(op, RGD,IXD); } }
        void bit_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { BIT_M(op, IN,RG);   } }
        void bit_in_rgd(uint16_t op)    { m_icount.i -= 15+ 9; { BIT_M(op, IN,RGD);  } }
        void bit_in_in(uint16_t op)     { m_icount.i -= 15+ 9; { BIT_M(op, IN,IN);   } }
        void bit_in_ind(uint16_t op)    { m_icount.i -= 15+15; { BIT_M(op, IN,IND);  } }
        void bit_in_de(uint16_t op)     { m_icount.i -= 15+12; { BIT_M(op, IN,DE);   } }
        void bit_in_ded(uint16_t op)    { m_icount.i -= 15+18; { BIT_M(op, IN,DED);  } }
        void bit_in_ix(uint16_t op)     { m_icount.i -= 15+18; { BIT_M(op, IN,IX);   } }
        void bit_in_ixd(uint16_t op)    { m_icount.i -= 15+24; { BIT_M(op, IN,IXD);  } }
        void bit_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { BIT_M(op, IND,RG);  } }
        void bit_ind_rgd(uint16_t op)   { m_icount.i -= 21+ 9; { BIT_M(op, IND,RGD); } }
        void bit_ind_in(uint16_t op)    { m_icount.i -= 21+ 9; { BIT_M(op, IND,IN);  } }
        void bit_ind_ind(uint16_t op)   { m_icount.i -= 21+15; { BIT_M(op, IND,IND); } }
        void bit_ind_de(uint16_t op)    { m_icount.i -= 21+12; { BIT_M(op, IND,DE);  } }
        void bit_ind_ded(uint16_t op)   { m_icount.i -= 21+18; { BIT_M(op, IND,DED); } }
        void bit_ind_ix(uint16_t op)    { m_icount.i -= 21+18; { BIT_M(op, IND,IX);  } }
        void bit_ind_ixd(uint16_t op)   { m_icount.i -= 21+24; { BIT_M(op, IND,IXD); } }
        void bit_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { BIT_M(op, DE,RG);   } }
        void bit_de_rgd(uint16_t op)    { m_icount.i -= 18+ 9; { BIT_M(op, DE,RGD);  } }
        void bit_de_in(uint16_t op)     { m_icount.i -= 18+ 9; { BIT_M(op, DE,IN);   } }
        void bit_de_ind(uint16_t op)    { m_icount.i -= 18+15; { BIT_M(op, DE,IND);  } }
        void bit_de_de(uint16_t op)     { m_icount.i -= 18+12; { BIT_M(op, DE,DE);   } }
        void bit_de_ded(uint16_t op)    { m_icount.i -= 18+18; { BIT_M(op, DE,DED);  } }
        void bit_de_ix(uint16_t op)     { m_icount.i -= 18+18; { BIT_M(op, DE,IX);   } }
        void bit_de_ixd(uint16_t op)    { m_icount.i -= 18+24; { BIT_M(op, DE,IXD);  } }
        void bit_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { BIT_M(op, DED,RG);  } }
        void bit_ded_rgd(uint16_t op)   { m_icount.i -= 24+ 9; { BIT_M(op, DED,RGD); } }
        void bit_ded_in(uint16_t op)    { m_icount.i -= 24+ 9; { BIT_M(op, DED,IN);  } }
        void bit_ded_ind(uint16_t op)   { m_icount.i -= 24+15; { BIT_M(op, DED,IND); } }
        void bit_ded_de(uint16_t op)    { m_icount.i -= 24+12; { BIT_M(op, DED,DE);  } }
        void bit_ded_ded(uint16_t op)   { m_icount.i -= 24+18; { BIT_M(op, DED,DED); } }
        void bit_ded_ix(uint16_t op)    { m_icount.i -= 24+18; { BIT_M(op, DED,IX);  } }
        void bit_ded_ixd(uint16_t op)   { m_icount.i -= 24+24; { BIT_M(op, DED,IXD); } }
        void bit_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { BIT_M(op, IX,RG);   } }
        void bit_ix_rgd(uint16_t op)    { m_icount.i -= 24+ 9; { BIT_M(op, IX,RGD);  } }
        void bit_ix_in(uint16_t op)     { m_icount.i -= 24+ 9; { BIT_M(op, IX,IN);   } }
        void bit_ix_ind(uint16_t op)    { m_icount.i -= 24+15; { BIT_M(op, IX,IND);  } }
        void bit_ix_de(uint16_t op)     { m_icount.i -= 24+12; { BIT_M(op, IX,DE);   } }
        void bit_ix_ded(uint16_t op)    { m_icount.i -= 24+18; { BIT_M(op, IX,DED);  } }
        void bit_ix_ix(uint16_t op)     { m_icount.i -= 24+18; { BIT_M(op, IX,IX);   } }
        void bit_ix_ixd(uint16_t op)    { m_icount.i -= 24+24; { BIT_M(op, IX,IXD);  } }
        void bit_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { BIT_M(op, IXD,RG);  } }
        void bit_ixd_rgd(uint16_t op)   { m_icount.i -= 30+ 9; { BIT_M(op, IXD,RGD); } }
        void bit_ixd_in(uint16_t op)    { m_icount.i -= 30+ 9; { BIT_M(op, IXD,IN);  } }
        void bit_ixd_ind(uint16_t op)   { m_icount.i -= 30+15; { BIT_M(op, IXD,IND); } }
        void bit_ixd_de(uint16_t op)    { m_icount.i -= 30+12; { BIT_M(op, IXD,DE);  } }
        void bit_ixd_ded(uint16_t op)   { m_icount.i -= 30+18; { BIT_M(op, IXD,DED); } }
        void bit_ixd_ix(uint16_t op)    { m_icount.i -= 30+18; { BIT_M(op, IXD,IX);  } }
        void bit_ixd_ixd(uint16_t op)   { m_icount.i -= 30+24; { BIT_M(op, IXD,IXD); } }

        void bic_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { BIC_R(op, RG,RG);   } }
        void bic_rg_rgd(uint16_t op)    { m_icount.i -=  9+12; { BIC_M(op, RG,RGD);  } }
        void bic_rg_in(uint16_t op)     { m_icount.i -=  9+12; { BIC_M(op, RG,IN);   } }
        void bic_rg_ind(uint16_t op)    { m_icount.i -=  9+18; { BIC_M(op, RG,IND);  } }
        void bic_rg_de(uint16_t op)     { m_icount.i -=  9+15; { BIC_M(op, RG,DE);   } }
        void bic_rg_ded(uint16_t op)    { m_icount.i -=  9+21; { BIC_M(op, RG,DED);  } }
        void bic_rg_ix(uint16_t op)     { m_icount.i -=  9+21; { BIC_M(op, RG,IX);   } }
        void bic_rg_ixd(uint16_t op)    { m_icount.i -=  9+27; { BIC_M(op, RG,IXD);  } }
        void bic_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { BIC_X(op, RGD,RG);  } }
        void bic_rgd_rgd(uint16_t op)   { m_icount.i -= 15+12; { BIC_M(op, RGD,RGD); } }
        void bic_rgd_in(uint16_t op)    { m_icount.i -= 15+12; { BIC_M(op, RGD,IN);  } }
        void bic_rgd_ind(uint16_t op)   { m_icount.i -= 15+18; { BIC_M(op, RGD,IND); } }
        void bic_rgd_de(uint16_t op)    { m_icount.i -= 15+15; { BIC_M(op, RGD,DE);  } }
        void bic_rgd_ded(uint16_t op)   { m_icount.i -= 15+21; { BIC_M(op, RGD,DED); } }
        void bic_rgd_ix(uint16_t op)    { m_icount.i -= 15+21; { BIC_M(op, RGD,IX);  } }
        void bic_rgd_ixd(uint16_t op)   { m_icount.i -= 15+27; { BIC_M(op, RGD,IXD); } }
        void bic_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { BIC_X(op, IN,RG);   } }
        void bic_in_rgd(uint16_t op)    { m_icount.i -= 15+12; { BIC_M(op, IN,RGD);  } }
        void bic_in_in(uint16_t op)     { m_icount.i -= 15+12; { BIC_M(op, IN,IN);   } }
        void bic_in_ind(uint16_t op)    { m_icount.i -= 15+18; { BIC_M(op, IN,IND);  } }
        void bic_in_de(uint16_t op)     { m_icount.i -= 15+15; { BIC_M(op, IN,DE);   } }
        void bic_in_ded(uint16_t op)    { m_icount.i -= 15+21; { BIC_M(op, IN,DED);  } }
        void bic_in_ix(uint16_t op)     { m_icount.i -= 15+21; { BIC_M(op, IN,IX);   } }
        void bic_in_ixd(uint16_t op)    { m_icount.i -= 15+27; { BIC_M(op, IN,IXD);  } }
        void bic_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { BIC_X(op, IND,RG);  } }
        void bic_ind_rgd(uint16_t op)   { m_icount.i -= 21+12; { BIC_M(op, IND,RGD); } }
        void bic_ind_in(uint16_t op)    { m_icount.i -= 21+12; { BIC_M(op, IND,IN);  } }
        void bic_ind_ind(uint16_t op)   { m_icount.i -= 21+18; { BIC_M(op, IND,IND); } }
        void bic_ind_de(uint16_t op)    { m_icount.i -= 21+15; { BIC_M(op, IND,DE);  } }
        void bic_ind_ded(uint16_t op)   { m_icount.i -= 21+21; { BIC_M(op, IND,DED); } }
        void bic_ind_ix(uint16_t op)    { m_icount.i -= 21+21; { BIC_M(op, IND,IX);  } }
        void bic_ind_ixd(uint16_t op)   { m_icount.i -= 21+27; { BIC_M(op, IND,IXD); } }
        void bic_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { BIC_X(op, DE,RG);   } }
        void bic_de_rgd(uint16_t op)    { m_icount.i -= 18+12; { BIC_M(op, DE,RGD);  } }
        void bic_de_in(uint16_t op)     { m_icount.i -= 18+12; { BIC_M(op, DE,IN);   } }
        void bic_de_ind(uint16_t op)    { m_icount.i -= 18+18; { BIC_M(op, DE,IND);  } }
        void bic_de_de(uint16_t op)     { m_icount.i -= 18+15; { BIC_M(op, DE,DE);   } }
        void bic_de_ded(uint16_t op)    { m_icount.i -= 18+21; { BIC_M(op, DE,DED);  } }
        void bic_de_ix(uint16_t op)     { m_icount.i -= 18+21; { BIC_M(op, DE,IX);   } }
        void bic_de_ixd(uint16_t op)    { m_icount.i -= 18+27; { BIC_M(op, DE,IXD);  } }
        void bic_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { BIC_X(op, DED,RG);  } }
        void bic_ded_rgd(uint16_t op)   { m_icount.i -= 24+12; { BIC_M(op, DED,RGD); } }
        void bic_ded_in(uint16_t op)    { m_icount.i -= 24+12; { BIC_M(op, DED,IN);  } }
        void bic_ded_ind(uint16_t op)   { m_icount.i -= 24+18; { BIC_M(op, DED,IND); } }
        void bic_ded_de(uint16_t op)    { m_icount.i -= 24+15; { BIC_M(op, DED,DE);  } }
        void bic_ded_ded(uint16_t op)   { m_icount.i -= 24+21; { BIC_M(op, DED,DED); } }
        void bic_ded_ix(uint16_t op)    { m_icount.i -= 24+21; { BIC_M(op, DED,IX);  } }
        void bic_ded_ixd(uint16_t op)   { m_icount.i -= 24+27; { BIC_M(op, DED,IXD); } }
        void bic_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { BIC_X(op, IX,RG);   } }
        void bic_ix_rgd(uint16_t op)    { m_icount.i -= 24+12; { BIC_M(op, IX,RGD);  } }
        void bic_ix_in(uint16_t op)     { m_icount.i -= 24+12; { BIC_M(op, IX,IN);   } }
        void bic_ix_ind(uint16_t op)    { m_icount.i -= 24+18; { BIC_M(op, IX,IND);  } }
        void bic_ix_de(uint16_t op)     { m_icount.i -= 24+15; { BIC_M(op, IX,DE);   } }
        void bic_ix_ded(uint16_t op)    { m_icount.i -= 24+21; { BIC_M(op, IX,DED);  } }
        void bic_ix_ix(uint16_t op)     { m_icount.i -= 24+21; { BIC_M(op, IX,IX);   } }
        void bic_ix_ixd(uint16_t op)    { m_icount.i -= 24+27; { BIC_M(op, IX,IXD);  } }
        void bic_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { BIC_X(op, IXD,RG);  } }
        void bic_ixd_rgd(uint16_t op)   { m_icount.i -= 30+12; { BIC_M(op, IXD,RGD); } }
        void bic_ixd_in(uint16_t op)    { m_icount.i -= 30+12; { BIC_M(op, IXD,IN);  } }
        void bic_ixd_ind(uint16_t op)   { m_icount.i -= 30+18; { BIC_M(op, IXD,IND); } }
        void bic_ixd_de(uint16_t op)    { m_icount.i -= 30+15; { BIC_M(op, IXD,DE);  } }
        void bic_ixd_ded(uint16_t op)   { m_icount.i -= 30+21; { BIC_M(op, IXD,DED); } }
        void bic_ixd_ix(uint16_t op)    { m_icount.i -= 30+21; { BIC_M(op, IXD,IX);  } }
        void bic_ixd_ixd(uint16_t op)   { m_icount.i -= 30+27; { BIC_M(op, IXD,IXD); } }

        void bis_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { BIS_R(op, RG,RG);   } }
        void bis_rg_rgd(uint16_t op)    { m_icount.i -=  9+12; { BIS_M(op, RG,RGD);  } }
        void bis_rg_in(uint16_t op)     { m_icount.i -=  9+12; { BIS_M(op, RG,IN);   } }
        void bis_rg_ind(uint16_t op)    { m_icount.i -=  9+18; { BIS_M(op, RG,IND);  } }
        void bis_rg_de(uint16_t op)     { m_icount.i -=  9+15; { BIS_M(op, RG,DE);   } }
        void bis_rg_ded(uint16_t op)    { m_icount.i -=  9+21; { BIS_M(op, RG,DED);  } }
        void bis_rg_ix(uint16_t op)     { m_icount.i -=  9+21; { BIS_M(op, RG,IX);   } }
        void bis_rg_ixd(uint16_t op)    { m_icount.i -=  9+27; { BIS_M(op, RG,IXD);  } }
        void bis_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { BIS_X(op, RGD,RG);  } }
        void bis_rgd_rgd(uint16_t op)   { m_icount.i -= 15+12; { BIS_M(op, RGD,RGD); } }
        void bis_rgd_in(uint16_t op)    { m_icount.i -= 15+12; { BIS_M(op, RGD,IN);  } }
        void bis_rgd_ind(uint16_t op)   { m_icount.i -= 15+18; { BIS_M(op, RGD,IND); } }
        void bis_rgd_de(uint16_t op)    { m_icount.i -= 15+15; { BIS_M(op, RGD,DE);  } }
        void bis_rgd_ded(uint16_t op)   { m_icount.i -= 15+21; { BIS_M(op, RGD,DED); } }
        void bis_rgd_ix(uint16_t op)    { m_icount.i -= 15+21; { BIS_M(op, RGD,IX);  } }
        void bis_rgd_ixd(uint16_t op)   { m_icount.i -= 15+27; { BIS_M(op, RGD,IXD); } }
        void bis_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { BIS_X(op, IN,RG);   } }
        void bis_in_rgd(uint16_t op)    { m_icount.i -= 15+12; { BIS_M(op, IN,RGD);  } }
        void bis_in_in(uint16_t op)     { m_icount.i -= 15+12; { BIS_M(op, IN,IN);   } }
        void bis_in_ind(uint16_t op)    { m_icount.i -= 15+18; { BIS_M(op, IN,IND);  } }
        void bis_in_de(uint16_t op)     { m_icount.i -= 15+15; { BIS_M(op, IN,DE);   } }
        void bis_in_ded(uint16_t op)    { m_icount.i -= 15+21; { BIS_M(op, IN,DED);  } }
        void bis_in_ix(uint16_t op)     { m_icount.i -= 15+21; { BIS_M(op, IN,IX);   } }
        void bis_in_ixd(uint16_t op)    { m_icount.i -= 15+27; { BIS_M(op, IN,IXD);  } }
        void bis_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { BIS_X(op, IND,RG);  } }
        void bis_ind_rgd(uint16_t op)   { m_icount.i -= 21+12; { BIS_M(op, IND,RGD); } }
        void bis_ind_in(uint16_t op)    { m_icount.i -= 21+12; { BIS_M(op, IND,IN);  } }
        void bis_ind_ind(uint16_t op)   { m_icount.i -= 21+18; { BIS_M(op, IND,IND); } }
        void bis_ind_de(uint16_t op)    { m_icount.i -= 21+15; { BIS_M(op, IND,DE);  } }
        void bis_ind_ded(uint16_t op)   { m_icount.i -= 21+21; { BIS_M(op, IND,DED); } }
        void bis_ind_ix(uint16_t op)    { m_icount.i -= 21+21; { BIS_M(op, IND,IX);  } }
        void bis_ind_ixd(uint16_t op)   { m_icount.i -= 21+27; { BIS_M(op, IND,IXD); } }
        void bis_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { BIS_X(op, DE,RG);   } }
        void bis_de_rgd(uint16_t op)    { m_icount.i -= 18+12; { BIS_M(op, DE,RGD);  } }
        void bis_de_in(uint16_t op)     { m_icount.i -= 18+12; { BIS_M(op, DE,IN);   } }
        void bis_de_ind(uint16_t op)    { m_icount.i -= 18+18; { BIS_M(op, DE,IND);  } }
        void bis_de_de(uint16_t op)     { m_icount.i -= 18+15; { BIS_M(op, DE,DE);   } }
        void bis_de_ded(uint16_t op)    { m_icount.i -= 18+21; { BIS_M(op, DE,DED);  } }
        void bis_de_ix(uint16_t op)     { m_icount.i -= 18+21; { BIS_M(op, DE,IX);   } }
        void bis_de_ixd(uint16_t op)    { m_icount.i -= 18+27; { BIS_M(op, DE,IXD);  } }
        void bis_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { BIS_X(op, DED,RG);  } }
        void bis_ded_rgd(uint16_t op)   { m_icount.i -= 24+12; { BIS_M(op, DED,RGD); } }
        void bis_ded_in(uint16_t op)    { m_icount.i -= 24+12; { BIS_M(op, DED,IN);  } }
        void bis_ded_ind(uint16_t op)   { m_icount.i -= 24+18; { BIS_M(op, DED,IND); } }
        void bis_ded_de(uint16_t op)    { m_icount.i -= 24+15; { BIS_M(op, DED,DE);  } }
        void bis_ded_ded(uint16_t op)   { m_icount.i -= 24+21; { BIS_M(op, DED,DED); } }
        void bis_ded_ix(uint16_t op)    { m_icount.i -= 24+21; { BIS_M(op, DED,IX);  } }
        void bis_ded_ixd(uint16_t op)   { m_icount.i -= 24+27; { BIS_M(op, DED,IXD); } }
        void bis_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { BIS_X(op, IX,RG);   } }
        void bis_ix_rgd(uint16_t op)    { m_icount.i -= 24+12; { BIS_M(op, IX,RGD);  } }
        void bis_ix_in(uint16_t op)     { m_icount.i -= 24+12; { BIS_M(op, IX,IN);   } }
        void bis_ix_ind(uint16_t op)    { m_icount.i -= 24+18; { BIS_M(op, IX,IND);  } }
        void bis_ix_de(uint16_t op)     { m_icount.i -= 24+15; { BIS_M(op, IX,DE);   } }
        void bis_ix_ded(uint16_t op)    { m_icount.i -= 24+21; { BIS_M(op, IX,DED);  } }
        void bis_ix_ix(uint16_t op)     { m_icount.i -= 24+21; { BIS_M(op, IX,IX);   } }
        void bis_ix_ixd(uint16_t op)    { m_icount.i -= 24+27; { BIS_M(op, IX,IXD);  } }
        void bis_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { BIS_X(op, IXD,RG);  } }
        void bis_ixd_rgd(uint16_t op)   { m_icount.i -= 30+12; { BIS_M(op, IXD,RGD); } }
        void bis_ixd_in(uint16_t op)    { m_icount.i -= 30+12; { BIS_M(op, IXD,IN);  } }
        void bis_ixd_ind(uint16_t op)   { m_icount.i -= 30+18; { BIS_M(op, IXD,IND); } }
        void bis_ixd_de(uint16_t op)    { m_icount.i -= 30+15; { BIS_M(op, IXD,DE);  } }
        void bis_ixd_ded(uint16_t op)   { m_icount.i -= 30+21; { BIS_M(op, IXD,DED); } }
        void bis_ixd_ix(uint16_t op)    { m_icount.i -= 30+21; { BIS_M(op, IXD,IX);  } }
        void bis_ixd_ixd(uint16_t op)   { m_icount.i -= 30+27; { BIS_M(op, IXD,IXD); } }

        void add_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { ADD_R(op, RG,RG);   } }
        void add_rg_rgd(uint16_t op)    { m_icount.i -=  9+12; { ADD_M(op, RG,RGD);  } }
        void add_rg_in(uint16_t op)     { m_icount.i -=  9+12; { ADD_M(op, RG,IN);   } }
        void add_rg_ind(uint16_t op)    { m_icount.i -=  9+18; { ADD_M(op, RG,IND);  } }
        void add_rg_de(uint16_t op)     { m_icount.i -=  9+15; { ADD_M(op, RG,DE);   } }
        void add_rg_ded(uint16_t op)    { m_icount.i -=  9+21; { ADD_M(op, RG,DED);  } }
        void add_rg_ix(uint16_t op)     { m_icount.i -=  9+21; { ADD_M(op, RG,IX);   } }
        void add_rg_ixd(uint16_t op)    { m_icount.i -=  9+27; { ADD_M(op, RG,IXD);  } }
        void add_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { ADD_X(op, RGD,RG);  } }
        void add_rgd_rgd(uint16_t op)   { m_icount.i -= 15+12; { ADD_M(op, RGD,RGD); } }
        void add_rgd_in(uint16_t op)    { m_icount.i -= 15+12; { ADD_M(op, RGD,IN);  } }
        void add_rgd_ind(uint16_t op)   { m_icount.i -= 15+18; { ADD_M(op, RGD,IND); } }
        void add_rgd_de(uint16_t op)    { m_icount.i -= 15+15; { ADD_M(op, RGD,DE);  } }
        void add_rgd_ded(uint16_t op)   { m_icount.i -= 15+21; { ADD_M(op, RGD,DED); } }
        void add_rgd_ix(uint16_t op)    { m_icount.i -= 15+21; { ADD_M(op, RGD,IX);  } }
        void add_rgd_ixd(uint16_t op)   { m_icount.i -= 15+27; { ADD_M(op, RGD,IXD); } }
        void add_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { ADD_X(op, IN,RG);   } }
        void add_in_rgd(uint16_t op)    { m_icount.i -= 15+12; { ADD_M(op, IN,RGD);  } }
        void add_in_in(uint16_t op)     { m_icount.i -= 15+12; { ADD_M(op, IN,IN);   } }
        void add_in_ind(uint16_t op)    { m_icount.i -= 15+18; { ADD_M(op, IN,IND);  } }
        void add_in_de(uint16_t op)     { m_icount.i -= 15+15; { ADD_M(op, IN,DE);   } }
        void add_in_ded(uint16_t op)    { m_icount.i -= 15+21; { ADD_M(op, IN,DED);  } }
        void add_in_ix(uint16_t op)     { m_icount.i -= 15+21; { ADD_M(op, IN,IX);   } }
        void add_in_ixd(uint16_t op)    { m_icount.i -= 15+27; { ADD_M(op, IN,IXD);  } }
        void add_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { ADD_X(op, IND,RG);  } }
        void add_ind_rgd(uint16_t op)   { m_icount.i -= 21+12; { ADD_M(op, IND,RGD); } }
        void add_ind_in(uint16_t op)    { m_icount.i -= 21+12; { ADD_M(op, IND,IN);  } }
        void add_ind_ind(uint16_t op)   { m_icount.i -= 21+18; { ADD_M(op, IND,IND); } }
        void add_ind_de(uint16_t op)    { m_icount.i -= 21+15; { ADD_M(op, IND,DE);  } }
        void add_ind_ded(uint16_t op)   { m_icount.i -= 21+21; { ADD_M(op, IND,DED); } }
        void add_ind_ix(uint16_t op)    { m_icount.i -= 21+21; { ADD_M(op, IND,IX);  } }
        void add_ind_ixd(uint16_t op)   { m_icount.i -= 21+27; { ADD_M(op, IND,IXD); } }
        void add_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { ADD_X(op, DE,RG);   } }
        void add_de_rgd(uint16_t op)    { m_icount.i -= 18+12; { ADD_M(op, DE,RGD);  } }
        void add_de_in(uint16_t op)     { m_icount.i -= 18+12; { ADD_M(op, DE,IN);   } }
        void add_de_ind(uint16_t op)    { m_icount.i -= 18+18; { ADD_M(op, DE,IND);  } }
        void add_de_de(uint16_t op)     { m_icount.i -= 18+15; { ADD_M(op, DE,DE);   } }
        void add_de_ded(uint16_t op)    { m_icount.i -= 18+21; { ADD_M(op, DE,DED);  } }
        void add_de_ix(uint16_t op)     { m_icount.i -= 18+21; { ADD_M(op, DE,IX);   } }
        void add_de_ixd(uint16_t op)    { m_icount.i -= 18+27; { ADD_M(op, DE,IXD);  } }
        void add_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { ADD_X(op, DED,RG);  } }
        void add_ded_rgd(uint16_t op)   { m_icount.i -= 24+12; { ADD_M(op, DED,RGD); } }
        void add_ded_in(uint16_t op)    { m_icount.i -= 24+12; { ADD_M(op, DED,IN);  } }
        void add_ded_ind(uint16_t op)   { m_icount.i -= 24+18; { ADD_M(op, DED,IND); } }
        void add_ded_de(uint16_t op)    { m_icount.i -= 24+15; { ADD_M(op, DED,DE);  } }
        void add_ded_ded(uint16_t op)   { m_icount.i -= 24+21; { ADD_M(op, DED,DED); } }
        void add_ded_ix(uint16_t op)    { m_icount.i -= 24+21; { ADD_M(op, DED,IX);  } }
        void add_ded_ixd(uint16_t op)   { m_icount.i -= 24+27; { ADD_M(op, DED,IXD); } }
        void add_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { ADD_X(op, IX,RG);   } }
        void add_ix_rgd(uint16_t op)    { m_icount.i -= 24+12; { ADD_M(op, IX,RGD);  } }
        void add_ix_in(uint16_t op)     { m_icount.i -= 24+12; { ADD_M(op, IX,IN);   } }
        void add_ix_ind(uint16_t op)    { m_icount.i -= 24+18; { ADD_M(op, IX,IND);  } }
        void add_ix_de(uint16_t op)     { m_icount.i -= 24+15; { ADD_M(op, IX,DE);   } }
        void add_ix_ded(uint16_t op)    { m_icount.i -= 24+21; { ADD_M(op, IX,DED);  } }
        void add_ix_ix(uint16_t op)     { m_icount.i -= 24+21; { ADD_M(op, IX,IX);   } }
        void add_ix_ixd(uint16_t op)    { m_icount.i -= 24+27; { ADD_M(op, IX,IXD);  } }
        void add_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { ADD_X(op, IXD,RG);  } }
        void add_ixd_rgd(uint16_t op)   { m_icount.i -= 30+12; { ADD_M(op, IXD,RGD); } }
        void add_ixd_in(uint16_t op)    { m_icount.i -= 30+12; { ADD_M(op, IXD,IN);  } }
        void add_ixd_ind(uint16_t op)   { m_icount.i -= 30+18; { ADD_M(op, IXD,IND); } }
        void add_ixd_de(uint16_t op)    { m_icount.i -= 30+15; { ADD_M(op, IXD,DE);  } }
        void add_ixd_ded(uint16_t op)   { m_icount.i -= 30+21; { ADD_M(op, IXD,DED); } }
        void add_ixd_ix(uint16_t op)    { m_icount.i -= 30+21; { ADD_M(op, IXD,IX);  } }
        void add_ixd_ixd(uint16_t op)   { m_icount.i -= 30+27; { ADD_M(op, IXD,IXD); } }

        void xor_rg(uint16_t op)        { m_icount.i -= 12; { XOR_R(op, RG);  } }
        void xor_rgd(uint16_t op)       { m_icount.i -= 21; { XOR_M(op, RGD); } }
        void xor_in(uint16_t op)        { m_icount.i -= 21; { XOR_M(op, IN);  } }
        void xor_ind(uint16_t op)       { m_icount.i -= 27; { XOR_M(op, IND); } }
        void xor_de(uint16_t op)        { m_icount.i -= 24; { XOR_M(op, DE);  } }
        void xor_ded(uint16_t op)       { m_icount.i -= 30; { XOR_M(op, DED); } }
        void xor_ix(uint16_t op)        { m_icount.i -= 30; { XOR_M(op, IX);  } }
        void xor_ixd(uint16_t op)       { m_icount.i -= 36; { XOR_M(op, IXD); } }

        void sob(uint16_t op)
        {
            //int sreg, source;

            m_icount.i -= 18;
            GET_SREG(op); source_ = (int)REGD(sreg_);
            source_ -= 1;
            REGW(sreg_) = (uint16_t)source_;
            if (source_ != 0)
                PC -= (uint16_t)(2 * (op & 0x3f));
        }

        void bpl(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_N == 0); } }
        void bmi(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_N); } }
        void bhi(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_C == 0 && GET_Z == 0); } }
        void blos(uint16_t op)          { m_icount.i -= 12; { BR(op, GET_C != 0 || GET_Z != 0); } }
        void bvc(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_V == 0); } }
        void bvs(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_V); } }
        void bcc(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_C == 0); } }
        void bcs(uint16_t op)           { m_icount.i -= 12; { BR(op, GET_C); } }

        void emt(uint16_t op)
        {
            m_icount.i -= 48;
            PUSH(PSW);
            PUSH(PC);
            PC = (uint16_t)RWORD(0x18);
            PSW = (uint8_t)RWORD(0x1a);
            t11_check_irqs();
        }

        void trap(uint16_t op)
        {
            m_icount.i -= 48;
            PUSH(PSW);
            PUSH(PC);
            PC = (uint16_t)RWORD(0x1c);
            PSW = (uint8_t)RWORD(0x1e);
            t11_check_irqs();
        }

        void clrb_rg(uint16_t op)       { m_icount.i -= 12; { CLRB_R(op, RG);  } }
        void clrb_rgd(uint16_t op)      { m_icount.i -= 21; { CLRB_M(op, RGD); } }
        void clrb_in(uint16_t op)       { m_icount.i -= 21; { CLRB_M(op, IN);  } }
        void clrb_ind(uint16_t op)      { m_icount.i -= 27; { CLRB_M(op, IND); } }
        void clrb_de(uint16_t op)       { m_icount.i -= 24; { CLRB_M(op, DE);  } }
        void clrb_ded(uint16_t op)      { m_icount.i -= 30; { CLRB_M(op, DED); } }
        void clrb_ix(uint16_t op)       { m_icount.i -= 30; { CLRB_M(op, IX);  } }
        void clrb_ixd(uint16_t op)      { m_icount.i -= 36; { CLRB_M(op, IXD); } }

        void comb_rg(uint16_t op)       { m_icount.i -= 12; { COMB_R(op, RG);  } }
        void comb_rgd(uint16_t op)      { m_icount.i -= 21; { COMB_M(op, RGD); } }
        void comb_in(uint16_t op)       { m_icount.i -= 21; { COMB_M(op, IN);  } }
        void comb_ind(uint16_t op)      { m_icount.i -= 27; { COMB_M(op, IND); } }
        void comb_de(uint16_t op)       { m_icount.i -= 24; { COMB_M(op, DE);  } }
        void comb_ded(uint16_t op)      { m_icount.i -= 30; { COMB_M(op, DED); } }
        void comb_ix(uint16_t op)       { m_icount.i -= 30; { COMB_M(op, IX);  } }
        void comb_ixd(uint16_t op)      { m_icount.i -= 36; { COMB_M(op, IXD); } }

        void incb_rg(uint16_t op)       { m_icount.i -= 12; { INCB_R(op, RG);  } }
        void incb_rgd(uint16_t op)      { m_icount.i -= 21; { INCB_M(op, RGD); } }
        void incb_in(uint16_t op)       { m_icount.i -= 21; { INCB_M(op, IN);  } }
        void incb_ind(uint16_t op)      { m_icount.i -= 27; { INCB_M(op, IND); } }
        void incb_de(uint16_t op)       { m_icount.i -= 24; { INCB_M(op, DE);  } }
        void incb_ded(uint16_t op)      { m_icount.i -= 30; { INCB_M(op, DED); } }
        void incb_ix(uint16_t op)       { m_icount.i -= 30; { INCB_M(op, IX);  } }
        void incb_ixd(uint16_t op)      { m_icount.i -= 36; { INCB_M(op, IXD); } }

        void decb_rg(uint16_t op)       { m_icount.i -= 12; { DECB_R(op, RG);  } }
        void decb_rgd(uint16_t op)      { m_icount.i -= 21; { DECB_M(op, RGD); } }
        void decb_in(uint16_t op)       { m_icount.i -= 21; { DECB_M(op, IN);  } }
        void decb_ind(uint16_t op)      { m_icount.i -= 27; { DECB_M(op, IND); } }
        void decb_de(uint16_t op)       { m_icount.i -= 24; { DECB_M(op, DE);  } }
        void decb_ded(uint16_t op)      { m_icount.i -= 30; { DECB_M(op, DED); } }
        void decb_ix(uint16_t op)       { m_icount.i -= 30; { DECB_M(op, IX);  } }
        void decb_ixd(uint16_t op)      { m_icount.i -= 36; { DECB_M(op, IXD); } }

        void negb_rg(uint16_t op)       { m_icount.i -= 12; { NEGB_R(op, RG);  } }
        void negb_rgd(uint16_t op)      { m_icount.i -= 21; { NEGB_M(op, RGD); } }
        void negb_in(uint16_t op)       { m_icount.i -= 21; { NEGB_M(op, IN);  } }
        void negb_ind(uint16_t op)      { m_icount.i -= 27; { NEGB_M(op, IND); } }
        void negb_de(uint16_t op)       { m_icount.i -= 24; { NEGB_M(op, DE);  } }
        void negb_ded(uint16_t op)      { m_icount.i -= 30; { NEGB_M(op, DED); } }
        void negb_ix(uint16_t op)       { m_icount.i -= 30; { NEGB_M(op, IX);  } }
        void negb_ixd(uint16_t op)      { m_icount.i -= 36; { NEGB_M(op, IXD); } }

        void adcb_rg(uint16_t op)       { m_icount.i -= 12; { ADCB_R(op, RG);  } }
        void adcb_rgd(uint16_t op)      { m_icount.i -= 21; { ADCB_M(op, RGD); } }
        void adcb_in(uint16_t op)       { m_icount.i -= 21; { ADCB_M(op, IN);  } }
        void adcb_ind(uint16_t op)      { m_icount.i -= 27; { ADCB_M(op, IND); } }
        void adcb_de(uint16_t op)       { m_icount.i -= 24; { ADCB_M(op, DE);  } }
        void adcb_ded(uint16_t op)      { m_icount.i -= 30; { ADCB_M(op, DED); } }
        void adcb_ix(uint16_t op)       { m_icount.i -= 30; { ADCB_M(op, IX);  } }
        void adcb_ixd(uint16_t op)      { m_icount.i -= 36; { ADCB_M(op, IXD); } }

        void sbcb_rg(uint16_t op)       { m_icount.i -= 12; { SBCB_R(op, RG);  } }
        void sbcb_rgd(uint16_t op)      { m_icount.i -= 21; { SBCB_M(op, RGD); } }
        void sbcb_in(uint16_t op)       { m_icount.i -= 21; { SBCB_M(op, IN);  } }
        void sbcb_ind(uint16_t op)      { m_icount.i -= 27; { SBCB_M(op, IND); } }
        void sbcb_de(uint16_t op)       { m_icount.i -= 24; { SBCB_M(op, DE);  } }
        void sbcb_ded(uint16_t op)      { m_icount.i -= 30; { SBCB_M(op, DED); } }
        void sbcb_ix(uint16_t op)       { m_icount.i -= 30; { SBCB_M(op, IX);  } }
        void sbcb_ixd(uint16_t op)      { m_icount.i -= 36; { SBCB_M(op, IXD); } }

        void tstb_rg(uint16_t op)       { m_icount.i -= 12; { TSTB_R(op, RG);  } }
        void tstb_rgd(uint16_t op)      { m_icount.i -= 18; { TSTB_M(op, RGD); } }
        void tstb_in(uint16_t op)       { m_icount.i -= 18; { TSTB_M(op, IN);  } }
        void tstb_ind(uint16_t op)      { m_icount.i -= 24; { TSTB_M(op, IND); } }
        void tstb_de(uint16_t op)       { m_icount.i -= 21; { TSTB_M(op, DE);  } }
        void tstb_ded(uint16_t op)      { m_icount.i -= 27; { TSTB_M(op, DED); } }
        void tstb_ix(uint16_t op)       { m_icount.i -= 27; { TSTB_M(op, IX);  } }
        void tstb_ixd(uint16_t op)      { m_icount.i -= 33; { TSTB_M(op, IXD); } }

        void rorb_rg(uint16_t op)       { m_icount.i -= 12; { RORB_R(op, RG);  } }
        void rorb_rgd(uint16_t op)      { m_icount.i -= 21; { RORB_M(op, RGD); } }
        void rorb_in(uint16_t op)       { m_icount.i -= 21; { RORB_M(op, IN);  } }
        void rorb_ind(uint16_t op)      { m_icount.i -= 27; { RORB_M(op, IND); } }
        void rorb_de(uint16_t op)       { m_icount.i -= 24; { RORB_M(op, DE);  } }
        void rorb_ded(uint16_t op)      { m_icount.i -= 30; { RORB_M(op, DED); } }
        void rorb_ix(uint16_t op)       { m_icount.i -= 30; { RORB_M(op, IX);  } }
        void rorb_ixd(uint16_t op)      { m_icount.i -= 36; { RORB_M(op, IXD); } }

        void rolb_rg(uint16_t op)       { m_icount.i -= 12; { ROLB_R(op, RG);  } }
        void rolb_rgd(uint16_t op)      { m_icount.i -= 21; { ROLB_M(op, RGD); } }
        void rolb_in(uint16_t op)       { m_icount.i -= 21; { ROLB_M(op, IN);  } }
        void rolb_ind(uint16_t op)      { m_icount.i -= 27; { ROLB_M(op, IND); } }
        void rolb_de(uint16_t op)       { m_icount.i -= 24; { ROLB_M(op, DE);  } }
        void rolb_ded(uint16_t op)      { m_icount.i -= 30; { ROLB_M(op, DED); } }
        void rolb_ix(uint16_t op)       { m_icount.i -= 30; { ROLB_M(op, IX);  } }
        void rolb_ixd(uint16_t op)      { m_icount.i -= 36; { ROLB_M(op, IXD); } }

        void asrb_rg(uint16_t op)       { m_icount.i -= 12; { ASRB_R(op, RG);  } }
        void asrb_rgd(uint16_t op)      { m_icount.i -= 21; { ASRB_M(op, RGD); } }
        void asrb_in(uint16_t op)       { m_icount.i -= 21; { ASRB_M(op, IN);  } }
        void asrb_ind(uint16_t op)      { m_icount.i -= 27; { ASRB_M(op, IND); } }
        void asrb_de(uint16_t op)       { m_icount.i -= 24; { ASRB_M(op, DE);  } }
        void asrb_ded(uint16_t op)      { m_icount.i -= 30; { ASRB_M(op, DED); } }
        void asrb_ix(uint16_t op)       { m_icount.i -= 30; { ASRB_M(op, IX);  } }
        void asrb_ixd(uint16_t op)      { m_icount.i -= 36; { ASRB_M(op, IXD); } }

        void aslb_rg(uint16_t op)       { m_icount.i -= 12; { ASLB_R(op, RG);  } }
        void aslb_rgd(uint16_t op)      { m_icount.i -= 21; { ASLB_M(op, RGD); } }
        void aslb_in(uint16_t op)       { m_icount.i -= 21; { ASLB_M(op, IN);  } }
        void aslb_ind(uint16_t op)      { m_icount.i -= 27; { ASLB_M(op, IND); } }
        void aslb_de(uint16_t op)       { m_icount.i -= 24; { ASLB_M(op, DE);  } }
        void aslb_ded(uint16_t op)      { m_icount.i -= 30; { ASLB_M(op, DED); } }
        void aslb_ix(uint16_t op)       { m_icount.i -= 30; { ASLB_M(op, IX);  } }
        void aslb_ixd(uint16_t op)      { m_icount.i -= 36; { ASLB_M(op, IXD); } }

        void mtps_rg(uint16_t op)       { m_icount.i -= 24; { MTPS_R(op, RG);  } }
        void mtps_rgd(uint16_t op)      { m_icount.i -= 30; { MTPS_M(op, RGD); } }
        void mtps_in(uint16_t op)       { m_icount.i -= 30; { MTPS_M(op, IN);  } }
        void mtps_ind(uint16_t op)      { m_icount.i -= 36; { MTPS_M(op, IND); } }
        void mtps_de(uint16_t op)       { m_icount.i -= 33; { MTPS_M(op, DE);  } }
        void mtps_ded(uint16_t op)      { m_icount.i -= 39; { MTPS_M(op, DED); } }
        void mtps_ix(uint16_t op)       { m_icount.i -= 39; { MTPS_M(op, IX);  } }
        void mtps_ixd(uint16_t op)      { m_icount.i -= 45; { MTPS_M(op, IXD); } }

        void mfps_rg(uint16_t op)       { m_icount.i -= 12; { MFPS_R(op, RG);  } }
        void mfps_rgd(uint16_t op)      { m_icount.i -= 21; { MFPS_M(op, RGD); } }
        void mfps_in(uint16_t op)       { m_icount.i -= 21; { MFPS_M(op, IN);  } }
        void mfps_ind(uint16_t op)      { m_icount.i -= 27; { MFPS_M(op, IND); } }
        void mfps_de(uint16_t op)       { m_icount.i -= 24; { MFPS_M(op, DE);  } }
        void mfps_ded(uint16_t op)      { m_icount.i -= 30; { MFPS_M(op, DED); } }
        void mfps_ix(uint16_t op)       { m_icount.i -= 30; { MFPS_M(op, IX);  } }
        void mfps_ixd(uint16_t op)      { m_icount.i -= 36; { MFPS_M(op, IXD); } }

        void movb_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { MOVB_R(op, RG,RG);   } }
        void movb_rg_rgd(uint16_t op)    { m_icount.i -=  9+12; { MOVB_M(op, RG,RGD);  } }
        void movb_rg_in(uint16_t op)     { m_icount.i -=  9+12; { MOVB_M(op, RG,IN);   } }
        void movb_rg_ind(uint16_t op)    { m_icount.i -=  9+18; { MOVB_M(op, RG,IND);  } }
        void movb_rg_de(uint16_t op)     { m_icount.i -=  9+15; { MOVB_M(op, RG,DE);   } }
        void movb_rg_ded(uint16_t op)    { m_icount.i -=  9+21; { MOVB_M(op, RG,DED);  } }
        void movb_rg_ix(uint16_t op)     { m_icount.i -=  9+21; { MOVB_M(op, RG,IX);   } }
        void movb_rg_ixd(uint16_t op)    { m_icount.i -=  9+27; { MOVB_M(op, RG,IXD);  } }
        void movb_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { MOVB_X(op, RGD,RG);  } }
        void movb_rgd_rgd(uint16_t op)   { m_icount.i -= 15+12; { MOVB_M(op, RGD,RGD); } }
        void movb_rgd_in(uint16_t op)    { m_icount.i -= 15+12; { MOVB_M(op, RGD,IN);  } }
        void movb_rgd_ind(uint16_t op)   { m_icount.i -= 15+18; { MOVB_M(op, RGD,IND); } }
        void movb_rgd_de(uint16_t op)    { m_icount.i -= 15+15; { MOVB_M(op, RGD,DE);  } }
        void movb_rgd_ded(uint16_t op)   { m_icount.i -= 15+21; { MOVB_M(op, RGD,DED); } }
        void movb_rgd_ix(uint16_t op)    { m_icount.i -= 15+21; { MOVB_M(op, RGD,IX);  } }
        void movb_rgd_ixd(uint16_t op)   { m_icount.i -= 15+27; { MOVB_M(op, RGD,IXD); } }
        void movb_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { MOVB_X(op, IN,RG);   } }
        void movb_in_rgd(uint16_t op)    { m_icount.i -= 15+12; { MOVB_M(op, IN,RGD);  } }
        void movb_in_in(uint16_t op)     { m_icount.i -= 15+12; { MOVB_M(op, IN,IN);   } }
        void movb_in_ind(uint16_t op)    { m_icount.i -= 15+18; { MOVB_M(op, IN,IND);  } }
        void movb_in_de(uint16_t op)     { m_icount.i -= 15+15; { MOVB_M(op, IN,DE);   } }
        void movb_in_ded(uint16_t op)    { m_icount.i -= 15+21; { MOVB_M(op, IN,DED);  } }
        void movb_in_ix(uint16_t op)     { m_icount.i -= 15+21; { MOVB_M(op, IN,IX);   } }
        void movb_in_ixd(uint16_t op)    { m_icount.i -= 15+27; { MOVB_M(op, IN,IXD);  } }
        void movb_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { MOVB_X(op, IND,RG);  } }
        void movb_ind_rgd(uint16_t op)   { m_icount.i -= 21+12; { MOVB_M(op, IND,RGD); } }
        void movb_ind_in(uint16_t op)    { m_icount.i -= 21+12; { MOVB_M(op, IND,IN);  } }
        void movb_ind_ind(uint16_t op)   { m_icount.i -= 21+18; { MOVB_M(op, IND,IND); } }
        void movb_ind_de(uint16_t op)    { m_icount.i -= 21+15; { MOVB_M(op, IND,DE);  } }
        void movb_ind_ded(uint16_t op)   { m_icount.i -= 21+21; { MOVB_M(op, IND,DED); } }
        void movb_ind_ix(uint16_t op)    { m_icount.i -= 21+21; { MOVB_M(op, IND,IX);  } }
        void movb_ind_ixd(uint16_t op)   { m_icount.i -= 21+27; { MOVB_M(op, IND,IXD); } }
        void movb_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { MOVB_X(op, DE,RG);   } }
        void movb_de_rgd(uint16_t op)    { m_icount.i -= 18+12; { MOVB_M(op, DE,RGD);  } }
        void movb_de_in(uint16_t op)     { m_icount.i -= 18+12; { MOVB_M(op, DE,IN);   } }
        void movb_de_ind(uint16_t op)    { m_icount.i -= 18+18; { MOVB_M(op, DE,IND);  } }
        void movb_de_de(uint16_t op)     { m_icount.i -= 18+15; { MOVB_M(op, DE,DE);   } }
        void movb_de_ded(uint16_t op)    { m_icount.i -= 18+21; { MOVB_M(op, DE,DED);  } }
        void movb_de_ix(uint16_t op)     { m_icount.i -= 18+21; { MOVB_M(op, DE,IX);   } }
        void movb_de_ixd(uint16_t op)    { m_icount.i -= 18+27; { MOVB_M(op, DE,IXD);  } }
        void movb_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { MOVB_X(op, DED,RG);  } }
        void movb_ded_rgd(uint16_t op)   { m_icount.i -= 24+12; { MOVB_M(op, DED,RGD); } }
        void movb_ded_in(uint16_t op)    { m_icount.i -= 24+12; { MOVB_M(op, DED,IN);  } }
        void movb_ded_ind(uint16_t op)   { m_icount.i -= 24+18; { MOVB_M(op, DED,IND); } }
        void movb_ded_de(uint16_t op)    { m_icount.i -= 24+15; { MOVB_M(op, DED,DE);  } }
        void movb_ded_ded(uint16_t op)   { m_icount.i -= 24+21; { MOVB_M(op, DED,DED); } }
        void movb_ded_ix(uint16_t op)    { m_icount.i -= 24+21; { MOVB_M(op, DED,IX);  } }
        void movb_ded_ixd(uint16_t op)   { m_icount.i -= 24+27; { MOVB_M(op, DED,IXD); } }
        void movb_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { MOVB_X(op, IX,RG);   } }
        void movb_ix_rgd(uint16_t op)    { m_icount.i -= 24+12; { MOVB_M(op, IX,RGD);  } }
        void movb_ix_in(uint16_t op)     { m_icount.i -= 24+12; { MOVB_M(op, IX,IN);   } }
        void movb_ix_ind(uint16_t op)    { m_icount.i -= 24+18; { MOVB_M(op, IX,IND);  } }
        void movb_ix_de(uint16_t op)     { m_icount.i -= 24+15; { MOVB_M(op, IX,DE);   } }
        void movb_ix_ded(uint16_t op)    { m_icount.i -= 24+21; { MOVB_M(op, IX,DED);  } }
        void movb_ix_ix(uint16_t op)     { m_icount.i -= 24+21; { MOVB_M(op, IX,IX);   } }
        void movb_ix_ixd(uint16_t op)    { m_icount.i -= 24+27; { MOVB_M(op, IX,IXD);  } }
        void movb_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { MOVB_X(op, IXD,RG);  } }
        void movb_ixd_rgd(uint16_t op)   { m_icount.i -= 30+12; { MOVB_M(op, IXD,RGD); } }
        void movb_ixd_in(uint16_t op)    { m_icount.i -= 30+12; { MOVB_M(op, IXD,IN);  } }
        void movb_ixd_ind(uint16_t op)   { m_icount.i -= 30+18; { MOVB_M(op, IXD,IND); } }
        void movb_ixd_de(uint16_t op)    { m_icount.i -= 30+15; { MOVB_M(op, IXD,DE);  } }
        void movb_ixd_ded(uint16_t op)   { m_icount.i -= 30+21; { MOVB_M(op, IXD,DED); } }
        void movb_ixd_ix(uint16_t op)    { m_icount.i -= 30+21; { MOVB_M(op, IXD,IX);  } }
        void movb_ixd_ixd(uint16_t op)   { m_icount.i -= 30+27; { MOVB_M(op, IXD,IXD); } }

        void cmpb_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { CMPB_R(op, RG,RG);   } }
        void cmpb_rg_rgd(uint16_t op)    { m_icount.i -=  9+ 9; { CMPB_M(op, RG,RGD);  } }
        void cmpb_rg_in(uint16_t op)     { m_icount.i -=  9+ 9; { CMPB_M(op, RG,IN);   } }
        void cmpb_rg_ind(uint16_t op)    { m_icount.i -=  9+15; { CMPB_M(op, RG,IND);  } }
        void cmpb_rg_de(uint16_t op)     { m_icount.i -=  9+12; { CMPB_M(op, RG,DE);   } }
        void cmpb_rg_ded(uint16_t op)    { m_icount.i -=  9+18; { CMPB_M(op, RG,DED);  } }
        void cmpb_rg_ix(uint16_t op)     { m_icount.i -=  9+18; { CMPB_M(op, RG,IX);   } }
        void cmpb_rg_ixd(uint16_t op)    { m_icount.i -=  9+24; { CMPB_M(op, RG,IXD);  } }
        void cmpb_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { CMPB_M(op, RGD,RG);  } }
        void cmpb_rgd_rgd(uint16_t op)   { m_icount.i -= 15+ 9; { CMPB_M(op, RGD,RGD); } }
        void cmpb_rgd_in(uint16_t op)    { m_icount.i -= 15+ 9; { CMPB_M(op, RGD,IN);  } }
        void cmpb_rgd_ind(uint16_t op)   { m_icount.i -= 15+15; { CMPB_M(op, RGD,IND); } }
        void cmpb_rgd_de(uint16_t op)    { m_icount.i -= 15+12; { CMPB_M(op, RGD,DE);  } }
        void cmpb_rgd_ded(uint16_t op)   { m_icount.i -= 15+18; { CMPB_M(op, RGD,DED); } }
        void cmpb_rgd_ix(uint16_t op)    { m_icount.i -= 15+18; { CMPB_M(op, RGD,IX);  } }
        void cmpb_rgd_ixd(uint16_t op)   { m_icount.i -= 15+24; { CMPB_M(op, RGD,IXD); } }
        void cmpb_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { CMPB_M(op, IN,RG);   } }
        void cmpb_in_rgd(uint16_t op)    { m_icount.i -= 15+ 9; { CMPB_M(op, IN,RGD);  } }
        void cmpb_in_in(uint16_t op)     { m_icount.i -= 15+ 9; { CMPB_M(op, IN,IN);   } }
        void cmpb_in_ind(uint16_t op)    { m_icount.i -= 15+15; { CMPB_M(op, IN,IND);  } }
        void cmpb_in_de(uint16_t op)     { m_icount.i -= 15+12; { CMPB_M(op, IN,DE);   } }
        void cmpb_in_ded(uint16_t op)    { m_icount.i -= 15+18; { CMPB_M(op, IN,DED);  } }
        void cmpb_in_ix(uint16_t op)     { m_icount.i -= 15+18; { CMPB_M(op, IN,IX);   } }
        void cmpb_in_ixd(uint16_t op)    { m_icount.i -= 15+24; { CMPB_M(op, IN,IXD);  } }
        void cmpb_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { CMPB_M(op, IND,RG);  } }
        void cmpb_ind_rgd(uint16_t op)   { m_icount.i -= 21+ 9; { CMPB_M(op, IND,RGD); } }
        void cmpb_ind_in(uint16_t op)    { m_icount.i -= 21+ 9; { CMPB_M(op, IND,IN);  } }
        void cmpb_ind_ind(uint16_t op)   { m_icount.i -= 21+15; { CMPB_M(op, IND,IND); } }
        void cmpb_ind_de(uint16_t op)    { m_icount.i -= 21+12; { CMPB_M(op, IND,DE);  } }
        void cmpb_ind_ded(uint16_t op)   { m_icount.i -= 21+18; { CMPB_M(op, IND,DED); } }
        void cmpb_ind_ix(uint16_t op)    { m_icount.i -= 21+18; { CMPB_M(op, IND,IX);  } }
        void cmpb_ind_ixd(uint16_t op)   { m_icount.i -= 21+24; { CMPB_M(op, IND,IXD); } }
        void cmpb_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { CMPB_M(op, DE,RG);   } }
        void cmpb_de_rgd(uint16_t op)    { m_icount.i -= 18+ 9; { CMPB_M(op, DE,RGD);  } }
        void cmpb_de_in(uint16_t op)     { m_icount.i -= 18+ 9; { CMPB_M(op, DE,IN);   } }
        void cmpb_de_ind(uint16_t op)    { m_icount.i -= 18+15; { CMPB_M(op, DE,IND);  } }
        void cmpb_de_de(uint16_t op)     { m_icount.i -= 18+12; { CMPB_M(op, DE,DE);   } }
        void cmpb_de_ded(uint16_t op)    { m_icount.i -= 18+18; { CMPB_M(op, DE,DED);  } }
        void cmpb_de_ix(uint16_t op)     { m_icount.i -= 18+18; { CMPB_M(op, DE,IX);   } }
        void cmpb_de_ixd(uint16_t op)    { m_icount.i -= 18+24; { CMPB_M(op, DE,IXD);  } }
        void cmpb_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { CMPB_M(op, DED,RG);  } }
        void cmpb_ded_rgd(uint16_t op)   { m_icount.i -= 24+ 9; { CMPB_M(op, DED,RGD); } }
        void cmpb_ded_in(uint16_t op)    { m_icount.i -= 24+ 9; { CMPB_M(op, DED,IN);  } }
        void cmpb_ded_ind(uint16_t op)   { m_icount.i -= 24+15; { CMPB_M(op, DED,IND); } }
        void cmpb_ded_de(uint16_t op)    { m_icount.i -= 24+12; { CMPB_M(op, DED,DE);  } }
        void cmpb_ded_ded(uint16_t op)   { m_icount.i -= 24+18; { CMPB_M(op, DED,DED); } }
        void cmpb_ded_ix(uint16_t op)    { m_icount.i -= 24+18; { CMPB_M(op, DED,IX);  } }
        void cmpb_ded_ixd(uint16_t op)   { m_icount.i -= 24+24; { CMPB_M(op, DED,IXD); } }
        void cmpb_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { CMPB_M(op, IX,RG);   } }
        void cmpb_ix_rgd(uint16_t op)    { m_icount.i -= 24+ 9; { CMPB_M(op, IX,RGD);  } }
        void cmpb_ix_in(uint16_t op)     { m_icount.i -= 24+ 9; { CMPB_M(op, IX,IN);   } }
        void cmpb_ix_ind(uint16_t op)    { m_icount.i -= 24+15; { CMPB_M(op, IX,IND);  } }
        void cmpb_ix_de(uint16_t op)     { m_icount.i -= 24+12; { CMPB_M(op, IX,DE);   } }
        void cmpb_ix_ded(uint16_t op)    { m_icount.i -= 24+18; { CMPB_M(op, IX,DED);  } }
        void cmpb_ix_ix(uint16_t op)     { m_icount.i -= 24+18; { CMPB_M(op, IX,IX);   } }
        void cmpb_ix_ixd(uint16_t op)    { m_icount.i -= 24+24; { CMPB_M(op, IX,IXD);  } }
        void cmpb_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { CMPB_M(op, IXD,RG);  } }
        void cmpb_ixd_rgd(uint16_t op)   { m_icount.i -= 30+ 9; { CMPB_M(op, IXD,RGD); } }
        void cmpb_ixd_in(uint16_t op)    { m_icount.i -= 30+ 9; { CMPB_M(op, IXD,IN);  } }
        void cmpb_ixd_ind(uint16_t op)   { m_icount.i -= 30+15; { CMPB_M(op, IXD,IND); } }
        void cmpb_ixd_de(uint16_t op)    { m_icount.i -= 30+12; { CMPB_M(op, IXD,DE);  } }
        void cmpb_ixd_ded(uint16_t op)   { m_icount.i -= 30+18; { CMPB_M(op, IXD,DED); } }
        void cmpb_ixd_ix(uint16_t op)    { m_icount.i -= 30+18; { CMPB_M(op, IXD,IX);  } }
        void cmpb_ixd_ixd(uint16_t op)   { m_icount.i -= 30+24; { CMPB_M(op, IXD,IXD); } }

        void bitb_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { BITB_R(op, RG,RG);   } }
        void bitb_rg_rgd(uint16_t op)    { m_icount.i -=  9+ 9; { BITB_M(op, RG,RGD);  } }
        void bitb_rg_in(uint16_t op)     { m_icount.i -=  9+ 9; { BITB_M(op, RG,IN);   } }
        void bitb_rg_ind(uint16_t op)    { m_icount.i -=  9+15; { BITB_M(op, RG,IND);  } }
        void bitb_rg_de(uint16_t op)     { m_icount.i -=  9+12; { BITB_M(op, RG,DE);   } }
        void bitb_rg_ded(uint16_t op)    { m_icount.i -=  9+18; { BITB_M(op, RG,DED);  } }
        void bitb_rg_ix(uint16_t op)     { m_icount.i -=  9+18; { BITB_M(op, RG,IX);   } }
        void bitb_rg_ixd(uint16_t op)    { m_icount.i -=  9+24; { BITB_M(op, RG,IXD);  } }
        void bitb_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { BITB_M(op, RGD,RG);  } }
        void bitb_rgd_rgd(uint16_t op)   { m_icount.i -= 15+ 9; { BITB_M(op, RGD,RGD); } }
        void bitb_rgd_in(uint16_t op)    { m_icount.i -= 15+ 9; { BITB_M(op, RGD,IN);  } }
        void bitb_rgd_ind(uint16_t op)   { m_icount.i -= 15+15; { BITB_M(op, RGD,IND); } }
        void bitb_rgd_de(uint16_t op)    { m_icount.i -= 15+12; { BITB_M(op, RGD,DE);  } }
        void bitb_rgd_ded(uint16_t op)   { m_icount.i -= 15+18; { BITB_M(op, RGD,DED); } }
        void bitb_rgd_ix(uint16_t op)    { m_icount.i -= 15+18; { BITB_M(op, RGD,IX);  } }
        void bitb_rgd_ixd(uint16_t op)   { m_icount.i -= 15+24; { BITB_M(op, RGD,IXD); } }
        void bitb_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { BITB_M(op, IN,RG);   } }
        void bitb_in_rgd(uint16_t op)    { m_icount.i -= 15+ 9; { BITB_M(op, IN,RGD);  } }
        void bitb_in_in(uint16_t op)     { m_icount.i -= 15+ 9; { BITB_M(op, IN,IN);   } }
        void bitb_in_ind(uint16_t op)    { m_icount.i -= 15+15; { BITB_M(op, IN,IND);  } }
        void bitb_in_de(uint16_t op)     { m_icount.i -= 15+12; { BITB_M(op, IN,DE);   } }
        void bitb_in_ded(uint16_t op)    { m_icount.i -= 15+18; { BITB_M(op, IN,DED);  } }
        void bitb_in_ix(uint16_t op)     { m_icount.i -= 15+18; { BITB_M(op, IN,IX);   } }
        void bitb_in_ixd(uint16_t op)    { m_icount.i -= 15+24; { BITB_M(op, IN,IXD);  } }
        void bitb_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { BITB_M(op, IND,RG);  } }
        void bitb_ind_rgd(uint16_t op)   { m_icount.i -= 21+ 9; { BITB_M(op, IND,RGD); } }
        void bitb_ind_in(uint16_t op)    { m_icount.i -= 21+ 9; { BITB_M(op, IND,IN);  } }
        void bitb_ind_ind(uint16_t op)   { m_icount.i -= 21+15; { BITB_M(op, IND,IND); } }
        void bitb_ind_de(uint16_t op)    { m_icount.i -= 21+12; { BITB_M(op, IND,DE);  } }
        void bitb_ind_ded(uint16_t op)   { m_icount.i -= 21+18; { BITB_M(op, IND,DED); } }
        void bitb_ind_ix(uint16_t op)    { m_icount.i -= 21+18; { BITB_M(op, IND,IX);  } }
        void bitb_ind_ixd(uint16_t op)   { m_icount.i -= 21+24; { BITB_M(op, IND,IXD); } }
        void bitb_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { BITB_M(op, DE,RG);   } }
        void bitb_de_rgd(uint16_t op)    { m_icount.i -= 18+ 9; { BITB_M(op, DE,RGD);  } }
        void bitb_de_in(uint16_t op)     { m_icount.i -= 18+ 9; { BITB_M(op, DE,IN);   } }
        void bitb_de_ind(uint16_t op)    { m_icount.i -= 18+15; { BITB_M(op, DE,IND);  } }
        void bitb_de_de(uint16_t op)     { m_icount.i -= 18+12; { BITB_M(op, DE,DE);   } }
        void bitb_de_ded(uint16_t op)    { m_icount.i -= 18+18; { BITB_M(op, DE,DED);  } }
        void bitb_de_ix(uint16_t op)     { m_icount.i -= 18+18; { BITB_M(op, DE,IX);   } }
        void bitb_de_ixd(uint16_t op)    { m_icount.i -= 18+24; { BITB_M(op, DE,IXD);  } }
        void bitb_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { BITB_M(op, DED,RG);  } }
        void bitb_ded_rgd(uint16_t op)   { m_icount.i -= 24+ 9; { BITB_M(op, DED,RGD); } }
        void bitb_ded_in(uint16_t op)    { m_icount.i -= 24+ 9; { BITB_M(op, DED,IN);  } }
        void bitb_ded_ind(uint16_t op)   { m_icount.i -= 24+15; { BITB_M(op, DED,IND); } }
        void bitb_ded_de(uint16_t op)    { m_icount.i -= 24+12; { BITB_M(op, DED,DE);  } }
        void bitb_ded_ded(uint16_t op)   { m_icount.i -= 24+18; { BITB_M(op, DED,DED); } }
        void bitb_ded_ix(uint16_t op)    { m_icount.i -= 24+18; { BITB_M(op, DED,IX);  } }
        void bitb_ded_ixd(uint16_t op)   { m_icount.i -= 24+24; { BITB_M(op, DED,IXD); } }
        void bitb_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { BITB_M(op, IX,RG);   } }
        void bitb_ix_rgd(uint16_t op)    { m_icount.i -= 24+ 9; { BITB_M(op, IX,RGD);  } }
        void bitb_ix_in(uint16_t op)     { m_icount.i -= 24+ 9; { BITB_M(op, IX,IN);   } }
        void bitb_ix_ind(uint16_t op)    { m_icount.i -= 24+15; { BITB_M(op, IX,IND);  } }
        void bitb_ix_de(uint16_t op)     { m_icount.i -= 24+12; { BITB_M(op, IX,DE);   } }
        void bitb_ix_ded(uint16_t op)    { m_icount.i -= 24+18; { BITB_M(op, IX,DED);  } }
        void bitb_ix_ix(uint16_t op)     { m_icount.i -= 24+18; { BITB_M(op, IX,IX);   } }
        void bitb_ix_ixd(uint16_t op)    { m_icount.i -= 24+24; { BITB_M(op, IX,IXD);  } }
        void bitb_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { BITB_M(op, IXD,RG);  } }
        void bitb_ixd_rgd(uint16_t op)   { m_icount.i -= 30+ 9; { BITB_M(op, IXD,RGD); } }
        void bitb_ixd_in(uint16_t op)    { m_icount.i -= 30+ 9; { BITB_M(op, IXD,IN);  } }
        void bitb_ixd_ind(uint16_t op)   { m_icount.i -= 30+15; { BITB_M(op, IXD,IND); } }
        void bitb_ixd_de(uint16_t op)    { m_icount.i -= 30+12; { BITB_M(op, IXD,DE);  } }
        void bitb_ixd_ded(uint16_t op)   { m_icount.i -= 30+18; { BITB_M(op, IXD,DED); } }
        void bitb_ixd_ix(uint16_t op)    { m_icount.i -= 30+18; { BITB_M(op, IXD,IX);  } }
        void bitb_ixd_ixd(uint16_t op)   { m_icount.i -= 30+24; { BITB_M(op, IXD,IXD); } }

        void bicb_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { BICB_R(op, RG,RG);   } }
        void bicb_rg_rgd(uint16_t op)    { m_icount.i -=  9+12; { BICB_M(op, RG,RGD);  } }
        void bicb_rg_in(uint16_t op)     { m_icount.i -=  9+12; { BICB_M(op, RG,IN);   } }
        void bicb_rg_ind(uint16_t op)    { m_icount.i -=  9+18; { BICB_M(op, RG,IND);  } }
        void bicb_rg_de(uint16_t op)     { m_icount.i -=  9+15; { BICB_M(op, RG,DE);   } }
        void bicb_rg_ded(uint16_t op)    { m_icount.i -=  9+21; { BICB_M(op, RG,DED);  } }
        void bicb_rg_ix(uint16_t op)     { m_icount.i -=  9+21; { BICB_M(op, RG,IX);   } }
        void bicb_rg_ixd(uint16_t op)    { m_icount.i -=  9+27; { BICB_M(op, RG,IXD);  } }
        void bicb_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { BICB_X(op, RGD,RG);  } }
        void bicb_rgd_rgd(uint16_t op)   { m_icount.i -= 15+12; { BICB_M(op, RGD,RGD); } }
        void bicb_rgd_in(uint16_t op)    { m_icount.i -= 15+12; { BICB_M(op, RGD,IN);  } }
        void bicb_rgd_ind(uint16_t op)   { m_icount.i -= 15+18; { BICB_M(op, RGD,IND); } }
        void bicb_rgd_de(uint16_t op)    { m_icount.i -= 15+15; { BICB_M(op, RGD,DE);  } }
        void bicb_rgd_ded(uint16_t op)   { m_icount.i -= 15+21; { BICB_M(op, RGD,DED); } }
        void bicb_rgd_ix(uint16_t op)    { m_icount.i -= 15+21; { BICB_M(op, RGD,IX);  } }
        void bicb_rgd_ixd(uint16_t op)   { m_icount.i -= 15+27; { BICB_M(op, RGD,IXD); } }
        void bicb_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { BICB_X(op, IN,RG);   } }
        void bicb_in_rgd(uint16_t op)    { m_icount.i -= 15+12; { BICB_M(op, IN,RGD);  } }
        void bicb_in_in(uint16_t op)     { m_icount.i -= 15+12; { BICB_M(op, IN,IN);   } }
        void bicb_in_ind(uint16_t op)    { m_icount.i -= 15+18; { BICB_M(op, IN,IND);  } }
        void bicb_in_de(uint16_t op)     { m_icount.i -= 15+15; { BICB_M(op, IN,DE);   } }
        void bicb_in_ded(uint16_t op)    { m_icount.i -= 15+21; { BICB_M(op, IN,DED);  } }
        void bicb_in_ix(uint16_t op)     { m_icount.i -= 15+21; { BICB_M(op, IN,IX);   } }
        void bicb_in_ixd(uint16_t op)    { m_icount.i -= 15+27; { BICB_M(op, IN,IXD);  } }
        void bicb_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { BICB_X(op, IND,RG);  } }
        void bicb_ind_rgd(uint16_t op)   { m_icount.i -= 21+12; { BICB_M(op, IND,RGD); } }
        void bicb_ind_in(uint16_t op)    { m_icount.i -= 21+12; { BICB_M(op, IND,IN);  } }
        void bicb_ind_ind(uint16_t op)   { m_icount.i -= 21+18; { BICB_M(op, IND,IND); } }
        void bicb_ind_de(uint16_t op)    { m_icount.i -= 21+15; { BICB_M(op, IND,DE);  } }
        void bicb_ind_ded(uint16_t op)   { m_icount.i -= 21+21; { BICB_M(op, IND,DED); } }
        void bicb_ind_ix(uint16_t op)    { m_icount.i -= 21+21; { BICB_M(op, IND,IX);  } }
        void bicb_ind_ixd(uint16_t op)   { m_icount.i -= 21+27; { BICB_M(op, IND,IXD); } }
        void bicb_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { BICB_X(op, DE,RG);   } }
        void bicb_de_rgd(uint16_t op)    { m_icount.i -= 18+12; { BICB_M(op, DE,RGD);  } }
        void bicb_de_in(uint16_t op)     { m_icount.i -= 18+12; { BICB_M(op, DE,IN);   } }
        void bicb_de_ind(uint16_t op)    { m_icount.i -= 18+18; { BICB_M(op, DE,IND);  } }
        void bicb_de_de(uint16_t op)     { m_icount.i -= 18+15; { BICB_M(op, DE,DE);   } }
        void bicb_de_ded(uint16_t op)    { m_icount.i -= 18+21; { BICB_M(op, DE,DED);  } }
        void bicb_de_ix(uint16_t op)     { m_icount.i -= 18+21; { BICB_M(op, DE,IX);   } }
        void bicb_de_ixd(uint16_t op)    { m_icount.i -= 18+27; { BICB_M(op, DE,IXD);  } }
        void bicb_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { BICB_X(op, DED,RG);  } }
        void bicb_ded_rgd(uint16_t op)   { m_icount.i -= 24+12; { BICB_M(op, DED,RGD); } }
        void bicb_ded_in(uint16_t op)    { m_icount.i -= 24+12; { BICB_M(op, DED,IN);  } }
        void bicb_ded_ind(uint16_t op)   { m_icount.i -= 24+18; { BICB_M(op, DED,IND); } }
        void bicb_ded_de(uint16_t op)    { m_icount.i -= 24+15; { BICB_M(op, DED,DE);  } }
        void bicb_ded_ded(uint16_t op)   { m_icount.i -= 24+21; { BICB_M(op, DED,DED); } }
        void bicb_ded_ix(uint16_t op)    { m_icount.i -= 24+21; { BICB_M(op, DED,IX);  } }
        void bicb_ded_ixd(uint16_t op)   { m_icount.i -= 24+27; { BICB_M(op, DED,IXD); } }
        void bicb_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { BICB_X(op, IX,RG);   } }
        void bicb_ix_rgd(uint16_t op)    { m_icount.i -= 24+12; { BICB_M(op, IX,RGD);  } }
        void bicb_ix_in(uint16_t op)     { m_icount.i -= 24+12; { BICB_M(op, IX,IN);   } }
        void bicb_ix_ind(uint16_t op)    { m_icount.i -= 24+18; { BICB_M(op, IX,IND);  } }
        void bicb_ix_de(uint16_t op)     { m_icount.i -= 24+15; { BICB_M(op, IX,DE);   } }
        void bicb_ix_ded(uint16_t op)    { m_icount.i -= 24+21; { BICB_M(op, IX,DED);  } }
        void bicb_ix_ix(uint16_t op)     { m_icount.i -= 24+21; { BICB_M(op, IX,IX);   } }
        void bicb_ix_ixd(uint16_t op)    { m_icount.i -= 24+27; { BICB_M(op, IX,IXD);  } }
        void bicb_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { BICB_X(op, IXD,RG);  } }
        void bicb_ixd_rgd(uint16_t op)   { m_icount.i -= 30+12; { BICB_M(op, IXD,RGD); } }
        void bicb_ixd_in(uint16_t op)    { m_icount.i -= 30+12; { BICB_M(op, IXD,IN);  } }
        void bicb_ixd_ind(uint16_t op)   { m_icount.i -= 30+18; { BICB_M(op, IXD,IND); } }
        void bicb_ixd_de(uint16_t op)    { m_icount.i -= 30+15; { BICB_M(op, IXD,DE);  } }
        void bicb_ixd_ded(uint16_t op)   { m_icount.i -= 30+21; { BICB_M(op, IXD,DED); } }
        void bicb_ixd_ix(uint16_t op)    { m_icount.i -= 30+21; { BICB_M(op, IXD,IX);  } }
        void bicb_ixd_ixd(uint16_t op)   { m_icount.i -= 30+27; { BICB_M(op, IXD,IXD); } }

        void bisb_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { BISB_R(op, RG,RG);   } }
        void bisb_rg_rgd(uint16_t op)    { m_icount.i -=  9+12; { BISB_M(op, RG,RGD);  } }
        void bisb_rg_in(uint16_t op)     { m_icount.i -=  9+12; { BISB_M(op, RG,IN);   } }
        void bisb_rg_ind(uint16_t op)    { m_icount.i -=  9+18; { BISB_M(op, RG,IND);  } }
        void bisb_rg_de(uint16_t op)     { m_icount.i -=  9+15; { BISB_M(op, RG,DE);   } }
        void bisb_rg_ded(uint16_t op)    { m_icount.i -=  9+21; { BISB_M(op, RG,DED);  } }
        void bisb_rg_ix(uint16_t op)     { m_icount.i -=  9+21; { BISB_M(op, RG,IX);   } }
        void bisb_rg_ixd(uint16_t op)    { m_icount.i -=  9+27; { BISB_M(op, RG,IXD);  } }
        void bisb_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { BISB_X(op, RGD,RG);  } }
        void bisb_rgd_rgd(uint16_t op)   { m_icount.i -= 15+12; { BISB_M(op, RGD,RGD); } }
        void bisb_rgd_in(uint16_t op)    { m_icount.i -= 15+12; { BISB_M(op, RGD,IN);  } }
        void bisb_rgd_ind(uint16_t op)   { m_icount.i -= 15+18; { BISB_M(op, RGD,IND); } }
        void bisb_rgd_de(uint16_t op)    { m_icount.i -= 15+15; { BISB_M(op, RGD,DE);  } }
        void bisb_rgd_ded(uint16_t op)   { m_icount.i -= 15+21; { BISB_M(op, RGD,DED); } }
        void bisb_rgd_ix(uint16_t op)    { m_icount.i -= 15+21; { BISB_M(op, RGD,IX);  } }
        void bisb_rgd_ixd(uint16_t op)   { m_icount.i -= 15+27; { BISB_M(op, RGD,IXD); } }
        void bisb_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { BISB_X(op, IN,RG);   } }
        void bisb_in_rgd(uint16_t op)    { m_icount.i -= 15+12; { BISB_M(op, IN,RGD);  } }
        void bisb_in_in(uint16_t op)     { m_icount.i -= 15+12; { BISB_M(op, IN,IN);   } }
        void bisb_in_ind(uint16_t op)    { m_icount.i -= 15+18; { BISB_M(op, IN,IND);  } }
        void bisb_in_de(uint16_t op)     { m_icount.i -= 15+15; { BISB_M(op, IN,DE);   } }
        void bisb_in_ded(uint16_t op)    { m_icount.i -= 15+21; { BISB_M(op, IN,DED);  } }
        void bisb_in_ix(uint16_t op)     { m_icount.i -= 15+21; { BISB_M(op, IN,IX);   } }
        void bisb_in_ixd(uint16_t op)    { m_icount.i -= 15+27; { BISB_M(op, IN,IXD);  } }
        void bisb_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { BISB_X(op, IND,RG);  } }
        void bisb_ind_rgd(uint16_t op)   { m_icount.i -= 21+12; { BISB_M(op, IND,RGD); } }
        void bisb_ind_in(uint16_t op)    { m_icount.i -= 21+12; { BISB_M(op, IND,IN);  } }
        void bisb_ind_ind(uint16_t op)   { m_icount.i -= 21+18; { BISB_M(op, IND,IND); } }
        void bisb_ind_de(uint16_t op)    { m_icount.i -= 21+15; { BISB_M(op, IND,DE);  } }
        void bisb_ind_ded(uint16_t op)   { m_icount.i -= 21+21; { BISB_M(op, IND,DED); } }
        void bisb_ind_ix(uint16_t op)    { m_icount.i -= 21+21; { BISB_M(op, IND,IX);  } }
        void bisb_ind_ixd(uint16_t op)   { m_icount.i -= 21+27; { BISB_M(op, IND,IXD); } }
        void bisb_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { BISB_X(op, DE,RG);   } }
        void bisb_de_rgd(uint16_t op)    { m_icount.i -= 18+12; { BISB_M(op, DE,RGD);  } }
        void bisb_de_in(uint16_t op)     { m_icount.i -= 18+12; { BISB_M(op, DE,IN);   } }
        void bisb_de_ind(uint16_t op)    { m_icount.i -= 18+18; { BISB_M(op, DE,IND);  } }
        void bisb_de_de(uint16_t op)     { m_icount.i -= 18+15; { BISB_M(op, DE,DE);   } }
        void bisb_de_ded(uint16_t op)    { m_icount.i -= 18+21; { BISB_M(op, DE,DED);  } }
        void bisb_de_ix(uint16_t op)     { m_icount.i -= 18+21; { BISB_M(op, DE,IX);   } }
        void bisb_de_ixd(uint16_t op)    { m_icount.i -= 18+27; { BISB_M(op, DE,IXD);  } }
        void bisb_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { BISB_X(op, DED,RG);  } }
        void bisb_ded_rgd(uint16_t op)   { m_icount.i -= 24+12; { BISB_M(op, DED,RGD); } }
        void bisb_ded_in(uint16_t op)    { m_icount.i -= 24+12; { BISB_M(op, DED,IN);  } }
        void bisb_ded_ind(uint16_t op)   { m_icount.i -= 24+18; { BISB_M(op, DED,IND); } }
        void bisb_ded_de(uint16_t op)    { m_icount.i -= 24+15; { BISB_M(op, DED,DE);  } }
        void bisb_ded_ded(uint16_t op)   { m_icount.i -= 24+21; { BISB_M(op, DED,DED); } }
        void bisb_ded_ix(uint16_t op)    { m_icount.i -= 24+21; { BISB_M(op, DED,IX);  } }
        void bisb_ded_ixd(uint16_t op)   { m_icount.i -= 24+27; { BISB_M(op, DED,IXD); } }
        void bisb_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { BISB_X(op, IX,RG);   } }
        void bisb_ix_rgd(uint16_t op)    { m_icount.i -= 24+12; { BISB_M(op, IX,RGD);  } }
        void bisb_ix_in(uint16_t op)     { m_icount.i -= 24+12; { BISB_M(op, IX,IN);   } }
        void bisb_ix_ind(uint16_t op)    { m_icount.i -= 24+18; { BISB_M(op, IX,IND);  } }
        void bisb_ix_de(uint16_t op)     { m_icount.i -= 24+15; { BISB_M(op, IX,DE);   } }
        void bisb_ix_ded(uint16_t op)    { m_icount.i -= 24+21; { BISB_M(op, IX,DED);  } }
        void bisb_ix_ix(uint16_t op)     { m_icount.i -= 24+21; { BISB_M(op, IX,IX);   } }
        void bisb_ix_ixd(uint16_t op)    { m_icount.i -= 24+27; { BISB_M(op, IX,IXD);  } }
        void bisb_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { BISB_X(op, IXD,RG);  } }
        void bisb_ixd_rgd(uint16_t op)   { m_icount.i -= 30+12; { BISB_M(op, IXD,RGD); } }
        void bisb_ixd_in(uint16_t op)    { m_icount.i -= 30+12; { BISB_M(op, IXD,IN);  } }
        void bisb_ixd_ind(uint16_t op)   { m_icount.i -= 30+18; { BISB_M(op, IXD,IND); } }
        void bisb_ixd_de(uint16_t op)    { m_icount.i -= 30+15; { BISB_M(op, IXD,DE);  } }
        void bisb_ixd_ded(uint16_t op)   { m_icount.i -= 30+21; { BISB_M(op, IXD,DED); } }
        void bisb_ixd_ix(uint16_t op)    { m_icount.i -= 30+21; { BISB_M(op, IXD,IX);  } }
        void bisb_ixd_ixd(uint16_t op)   { m_icount.i -= 30+27; { BISB_M(op, IXD,IXD); } }

        void sub_rg_rg(uint16_t op)     { m_icount.i -=  9+ 3; { SUB_R(op, RG,RG);   } }
        void sub_rg_rgd(uint16_t op)    { m_icount.i -=  9+12; { SUB_M(op, RG,RGD);  } }
        void sub_rg_in(uint16_t op)     { m_icount.i -=  9+12; { SUB_M(op, RG,IN);   } }
        void sub_rg_ind(uint16_t op)    { m_icount.i -=  9+18; { SUB_M(op, RG,IND);  } }
        void sub_rg_de(uint16_t op)     { m_icount.i -=  9+15; { SUB_M(op, RG,DE);   } }
        void sub_rg_ded(uint16_t op)    { m_icount.i -=  9+21; { SUB_M(op, RG,DED);  } }
        void sub_rg_ix(uint16_t op)     { m_icount.i -=  9+21; { SUB_M(op, RG,IX);   } }
        void sub_rg_ixd(uint16_t op)    { m_icount.i -=  9+27; { SUB_M(op, RG,IXD);  } }
        void sub_rgd_rg(uint16_t op)    { m_icount.i -= 15+ 3; { SUB_X(op, RGD,RG);  } }
        void sub_rgd_rgd(uint16_t op)   { m_icount.i -= 15+12; { SUB_M(op, RGD,RGD); } }
        void sub_rgd_in(uint16_t op)    { m_icount.i -= 15+12; { SUB_M(op, RGD,IN);  } }
        void sub_rgd_ind(uint16_t op)   { m_icount.i -= 15+18; { SUB_M(op, RGD,IND); } }
        void sub_rgd_de(uint16_t op)    { m_icount.i -= 15+15; { SUB_M(op, RGD,DE);  } }
        void sub_rgd_ded(uint16_t op)   { m_icount.i -= 15+21; { SUB_M(op, RGD,DED); } }
        void sub_rgd_ix(uint16_t op)    { m_icount.i -= 15+21; { SUB_M(op, RGD,IX);  } }
        void sub_rgd_ixd(uint16_t op)   { m_icount.i -= 15+27; { SUB_M(op, RGD,IXD); } }
        void sub_in_rg(uint16_t op)     { m_icount.i -= 15+ 3; { SUB_X(op, IN,RG);   } }
        void sub_in_rgd(uint16_t op)    { m_icount.i -= 15+12; { SUB_M(op, IN,RGD);  } }
        void sub_in_in(uint16_t op)     { m_icount.i -= 15+12; { SUB_M(op, IN,IN);   } }
        void sub_in_ind(uint16_t op)    { m_icount.i -= 15+18; { SUB_M(op, IN,IND);  } }
        void sub_in_de(uint16_t op)     { m_icount.i -= 15+15; { SUB_M(op, IN,DE);   } }
        void sub_in_ded(uint16_t op)    { m_icount.i -= 15+21; { SUB_M(op, IN,DED);  } }
        void sub_in_ix(uint16_t op)     { m_icount.i -= 15+21; { SUB_M(op, IN,IX);   } }
        void sub_in_ixd(uint16_t op)    { m_icount.i -= 15+27; { SUB_M(op, IN,IXD);  } }
        void sub_ind_rg(uint16_t op)    { m_icount.i -= 21+ 3; { SUB_X(op, IND,RG);  } }
        void sub_ind_rgd(uint16_t op)   { m_icount.i -= 21+12; { SUB_M(op, IND,RGD); } }
        void sub_ind_in(uint16_t op)    { m_icount.i -= 21+12; { SUB_M(op, IND,IN);  } }
        void sub_ind_ind(uint16_t op)   { m_icount.i -= 21+18; { SUB_M(op, IND,IND); } }
        void sub_ind_de(uint16_t op)    { m_icount.i -= 21+15; { SUB_M(op, IND,DE);  } }
        void sub_ind_ded(uint16_t op)   { m_icount.i -= 21+21; { SUB_M(op, IND,DED); } }
        void sub_ind_ix(uint16_t op)    { m_icount.i -= 21+21; { SUB_M(op, IND,IX);  } }
        void sub_ind_ixd(uint16_t op)   { m_icount.i -= 21+27; { SUB_M(op, IND,IXD); } }
        void sub_de_rg(uint16_t op)     { m_icount.i -= 18+ 3; { SUB_X(op, DE,RG);   } }
        void sub_de_rgd(uint16_t op)    { m_icount.i -= 18+12; { SUB_M(op, DE,RGD);  } }
        void sub_de_in(uint16_t op)     { m_icount.i -= 18+12; { SUB_M(op, DE,IN);   } }
        void sub_de_ind(uint16_t op)    { m_icount.i -= 18+18; { SUB_M(op, DE,IND);  } }
        void sub_de_de(uint16_t op)     { m_icount.i -= 18+15; { SUB_M(op, DE,DE);   } }
        void sub_de_ded(uint16_t op)    { m_icount.i -= 18+21; { SUB_M(op, DE,DED);  } }
        void sub_de_ix(uint16_t op)     { m_icount.i -= 18+21; { SUB_M(op, DE,IX);   } }
        void sub_de_ixd(uint16_t op)    { m_icount.i -= 18+27; { SUB_M(op, DE,IXD);  } }
        void sub_ded_rg(uint16_t op)    { m_icount.i -= 24+ 3; { SUB_X(op, DED,RG);  } }
        void sub_ded_rgd(uint16_t op)   { m_icount.i -= 24+12; { SUB_M(op, DED,RGD); } }
        void sub_ded_in(uint16_t op)    { m_icount.i -= 24+12; { SUB_M(op, DED,IN);  } }
        void sub_ded_ind(uint16_t op)   { m_icount.i -= 24+18; { SUB_M(op, DED,IND); } }
        void sub_ded_de(uint16_t op)    { m_icount.i -= 24+15; { SUB_M(op, DED,DE);  } }
        void sub_ded_ded(uint16_t op)   { m_icount.i -= 24+21; { SUB_M(op, DED,DED); } }
        void sub_ded_ix(uint16_t op)    { m_icount.i -= 24+21; { SUB_M(op, DED,IX);  } }
        void sub_ded_ixd(uint16_t op)   { m_icount.i -= 24+27; { SUB_M(op, DED,IXD); } }
        void sub_ix_rg(uint16_t op)     { m_icount.i -= 24+ 3; { SUB_X(op, IX,RG);   } }
        void sub_ix_rgd(uint16_t op)    { m_icount.i -= 24+12; { SUB_M(op, IX,RGD);  } }
        void sub_ix_in(uint16_t op)     { m_icount.i -= 24+12; { SUB_M(op, IX,IN);   } }
        void sub_ix_ind(uint16_t op)    { m_icount.i -= 24+18; { SUB_M(op, IX,IND);  } }
        void sub_ix_de(uint16_t op)     { m_icount.i -= 24+15; { SUB_M(op, IX,DE);   } }
        void sub_ix_ded(uint16_t op)    { m_icount.i -= 24+21; { SUB_M(op, IX,DED);  } }
        void sub_ix_ix(uint16_t op)     { m_icount.i -= 24+21; { SUB_M(op, IX,IX);   } }
        void sub_ix_ixd(uint16_t op)    { m_icount.i -= 24+27; { SUB_M(op, IX,IXD);  } }
        void sub_ixd_rg(uint16_t op)    { m_icount.i -= 30+ 3; { SUB_X(op, IXD,RG);  } }
        void sub_ixd_rgd(uint16_t op)   { m_icount.i -= 30+12; { SUB_M(op, IXD,RGD); } }
        void sub_ixd_in(uint16_t op)    { m_icount.i -= 30+12; { SUB_M(op, IXD,IN);  } }
        void sub_ixd_ind(uint16_t op)   { m_icount.i -= 30+18; { SUB_M(op, IXD,IND); } }
        void sub_ixd_de(uint16_t op)    { m_icount.i -= 30+15; { SUB_M(op, IXD,DE);  } }
        void sub_ixd_ded(uint16_t op)   { m_icount.i -= 30+21; { SUB_M(op, IXD,DED); } }
        void sub_ixd_ix(uint16_t op)    { m_icount.i -= 30+21; { SUB_M(op, IXD,IX);  } }
        void sub_ixd_ixd(uint16_t op)   { m_icount.i -= 30+27; { SUB_M(op, IXD,IXD); } }
    }
}
