// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    public partial class m6805_base_device : cpu_device
    {
        //#define OP_HANDLER(name) void m6805_base_device::name()
        //#define OP_HANDLER_BIT(name) template <unsigned B> void m6805_base_device::name()
        //#define OP_HANDLER_BRA(name) template <bool C> void m6805_base_device::name()
        //#define OP_HANDLER_MODE(name) template <m6805_base_device::addr_mode M> void m6805_base_device::name()


        //OP_HANDLER( illegal )
        void illegal()
        {
            logerror("M6805: illegal opcode\n");
        }

        // $00/$02/$04/$06/$08/$0A/$0C/$0E BRSET direct,relative ---*
        //OP_HANDLER_BIT( brset )
        void brset(int B)
        {
            u8 t;
            u8 r;
            DIRBYTE(out r);
            immbyte(out t);
            CLC();
            if (BIT(r, B) != 0) { SEC(); PC = (u16)(PC + SIGNED(t)); }
        }

        void brset_0() { brset(0); }
        void brset_1() { brset(1); }
        void brset_2() { brset(2); }
        void brset_3() { brset(3); }
        void brset_4() { brset(4); }
        void brset_5() { brset(5); }
        void brset_6() { brset(6); }
        void brset_7() { brset(7); }

        // $01/$03/$05/$07/$09/$0B/$0D/$0F BRCLR direct,relative ---*
        //OP_HANDLER_BIT( brclr )
        void brclr(int B)
        {
            u8 t;
            u8 r;
            DIRBYTE(out r);
            immbyte(out t);
            SEC();
            if (BIT(r, B) == 0) { CLC(); PC = (u16)(PC + SIGNED(t)); }
        }

        void brclr_0() { brclr(0); }
        void brclr_1() { brclr(1); }
        void brclr_2() { brclr(2); }
        void brclr_3() { brclr(3); }
        void brclr_4() { brclr(4); }
        void brclr_5() { brclr(5); }
        void brclr_6() { brclr(6); }
        void brclr_7() { brclr(7); }

        // $10/$12/$14/$16/$18/$1A/$1C/$1E BSET direct ----
        //OP_HANDLER_BIT( bset )
        void bset(int B)
        {
            u8 t;
            DIRBYTE(out t);
            wm(EAD, (u8)(t | (1 << B)));
        }

        void bset_0() { bset(0); }
        void bset_1() { bset(1); }
        void bset_2() { bset(2); }
        void bset_3() { bset(3); }
        void bset_4() { bset(4); }
        void bset_5() { bset(5); }
        void bset_6() { bset(6); }
        void bset_7() { bset(7); }

        // $11/$13/$15/$17/$19/$1B/$1D/$1F BCLR direct ----
        //OP_HANDLER_BIT( bclr )
        void bclr(int B)
        {
            u8 t;
            DIRBYTE(out t);
            wm(EAD, (u8)(t & ~(1 << B)));
        }

        void bclr_0() { bclr(0); }
        void bclr_1() { bclr(1); }
        void bclr_2() { bclr(2); }
        void bclr_3() { bclr(3); }
        void bclr_4() { bclr(4); }
        void bclr_5() { bclr(5); }
        void bclr_6() { bclr(6); }
        void bclr_7() { bclr(7); }

        // $20 BRA relative ----
        // $21 BRN relative ----
        //OP_HANDLER_BRA( bra ) { BRANCH( true ); }
        void bra(bool C) { BRANCH(C, true); }
        void bra_true() { bra(true); }
        void bra_false() { bra(false); }

        // $22 BHI relative ----
        // $23 BLS relative ----
        //OP_HANDLER_BRA( bhi ) { BRANCH( !(CC & (CFLAG | ZFLAG)) ); }
        void bhi(bool C) { BRANCH(C, (CC & (CFLAG | ZFLAG)) == 0 ); }
        void bhi_true() { bhi(true); }
        void bhi_false() { bhi(false); }

        // $24 BCC relative ----
        // $25 BCS relative ----
        //OP_HANDLER_BRA( bcc ) { BRANCH( !(CC & CFLAG) ); }
        void bcc(bool C) { BRANCH(C, (CC & CFLAG) == 0 ); }
        void bcc_true() { bcc(true); }
        void bcc_false() { bcc(false); }

        // $26 BNE relative ----
        // $27 BEQ relative ----
        //OP_HANDLER_BRA( bne ) { BRANCH( !(CC & ZFLAG) ); }
        void bne(bool C) { BRANCH(C, (CC & ZFLAG) == 0 ); }
        void bne_true() { bne(true); }
        void bne_false() { bne(false); }

        // $28 BHCC relative ----
        // $29 BHCS relative ----
        //OP_HANDLER_BRA( bhcc ) { BRANCH( !(CC & HFLAG) ); }
        void bhcc(bool C) { BRANCH(C, (CC & HFLAG) == 0 ); }
        void bhcc_true() { bhcc(true); }
        void bhcc_false() { bhcc(false); }

        // $2a BPL relative ----
        // $2b BMI relative ----
        //OP_HANDLER_BRA( bpl ) { BRANCH( !(CC & NFLAG) ); }
        void bpl(bool C) { BRANCH(C, (CC & NFLAG) == 0 ); }
        void bpl_true() { bpl(true); }
        void bpl_false() { bpl(false); }

        // $2c BMC relative ----
        // $2d BMS relative ----
        //OP_HANDLER_BRA( bmc ) { BRANCH( !(CC & IFLAG) ); }
        void bmc(bool C) { BRANCH(C, (CC & IFLAG) == 0 ); }
        void bmc_true() { bmc(true); }
        void bmc_false() { bmc(false); }

        // $2e BIL relative ----
        // $2f BIH relative ----
        //OP_HANDLER_BRA( bil ) { BRANCH( test_il() ); }
        void bil(bool C) { BRANCH(C, test_il() ); }
        void bil_true() { bil(true); }
        void bil_false() { bil(false); }

        // $30 NEG direct                   -***
        // $60 NEG indexed, 1 byte offset   -***
        // $70 NEG indexed                  -***
        //OP_HANDLER_MODE( neg )
        void neg(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            u16 r = (u16)(-t);
            clr_nzc();
            set_nzc8(r);
            wm(EAD, (u8)r);
        }

        void neg_DI() { neg(addr_mode.DI); }
        void neg_IX() { neg(addr_mode.IX); }
        void neg_IX1() { neg(addr_mode.IX1); }

        // $31 ILLEGAL
        // $61 ILLEGAL
        // $71 ILLEGAL

        // $32 ILLEGAL
        // $62 ILLEGAL
        // $72 ILLEGAL

        // $33 COM direct                   -**1
        // $63 COM indexed, 1 byte offset   -**1
        // $73 COM indexed                  -**1
        //OP_HANDLER_MODE( com )
        void com(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            t = (u8)~t;
            clr_nz();
            set_nz8(t);
            SEC();
            wm(EAD, t);
        }

        void com_DI() { com(addr_mode.DI); }
        void com_IX() { com(addr_mode.IX); }
        void com_IX1() { com(addr_mode.IX1); }

        // $34 LSR direct                   -0**
        // $64 LSR indexed, 1 byte offset   -0**
        // $74 LSR indexed                  -0**
        //OP_HANDLER_MODE( lsr )
        void lsr(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            clr_nzc();
            CC |= (u8)BIT(t, 0);
            t >>= 1;
            set_z8(t);
            wm(EAD, t);
        }

        void lsr_DI() { lsr(addr_mode.DI); }
        void lsr_IX() { lsr(addr_mode.IX); }
        void lsr_IX1() { lsr(addr_mode.IX1); }

        // $35 ILLEGAL
        // $65 ILLEGAL
        // $75 ILLEGAL

        // $36 ROR direct                   -***
        // $66 ROR indexed, 1 byte offset   -***
        // $76 ROR indexed                  -***
        //OP_HANDLER_MODE( ror )
        void ror(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            u8 r = (u8)(BIT(CC, 0) << 7);
            clr_nzc();
            CC |= (u8)BIT(t, 0);
            r |= (u8)(t >> 1);
            set_nz8(r);
            wm(EAD, r);
        }

        void ror_DI() { ror(addr_mode.DI); }
        void ror_IX() { ror(addr_mode.IX); }
        void ror_IX1() { ror(addr_mode.IX1); }

        // $37 ASR direct                   -***
        // $67 ASR indexed, 1 byte offset   -***
        // $77 ASR indexed                  -***
        //OP_HANDLER_MODE( asr )
        void asr(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            clr_nzc();
            CC |= (u8)BIT(t, 0);
            t = (u8)((t >> 1) | (t & 0x80));
            set_nz8(t);
            wm(EAD, t);
        }

        void asr_DI() { asr(addr_mode.DI); }
        void asr_IX() { asr(addr_mode.IX); }
        void asr_IX1() { asr(addr_mode.IX1); }

        // $38 LSL direct                   -***
        // $68 LSL indexed, 1 byte offset   -***
        // $78 LSL indexed                  -***
        //OP_HANDLER_MODE( lsl )
        void lsl(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            u16 r = (u16)(t << 1);
            clr_nzc();
            set_nzc8(r);
            wm(EAD, (u8)r);
        }

        void lsl_DI() { lsl(addr_mode.DI); }
        void lsl_IX() { lsl(addr_mode.IX); }
        void lsl_IX1() { lsl(addr_mode.IX1); }

        // $39 ROL direct                   -***
        // $69 ROL indexed, 1 byte offset   -***
        // $79 ROL indexed                  -***
        //OP_HANDLER_MODE( rol )
        void rol(addr_mode M)
        {
            u16 t;
            ARGBYTE(M, out t);
            u16 r = (u16)(BIT(CC, 0) | (t << 1));
            clr_nzc();
            set_nzc8(r);
            wm(EAD, (u8)r);
        }

        void rol_DI() { rol(addr_mode.DI); }
        void rol_IX() { rol(addr_mode.IX); }
        void rol_IX1() { rol(addr_mode.IX1); }

        // $3a DEC direct                   -**-
        // $6a DEC indexed, 1 byte offset   -**-
        //OP_HANDLER_MODE( dec )
        void dec(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            --t;
            clr_nz();
            set_nz8(t);
            wm(EAD, t);
        }

        void dec_DI() { dec(addr_mode.DI); }
        void dec_IX() { dec(addr_mode.IX); }
        void dec_IX1() { dec(addr_mode.IX1); }

        // $3b ILLEGAL
        // $6b ILLEGAL
        // $7b ILLEGAL

        // $3c INC direct                   -**-
        // $6c INC indexed, 1 byte offset   -**-
        // $7c INC indexed                  -**-
        //OP_HANDLER_MODE( inc )
        void inc(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            ++t;
            clr_nz();
            set_nz8(t);
            wm(EAD, t);
        }

        void inc_DI() { inc(addr_mode.DI); }
        void inc_IX() { inc(addr_mode.IX); }
        void inc_IX1() { inc(addr_mode.IX1); }

        // $3d TST direct                   -**-
        // $6d TST indexed, 1 byte offset   -**-
        // $7d TST indexed                  -**-
        //OP_HANDLER_MODE( tst )
        void tst(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            clr_nz();
            set_nz8(t);
        }

        void tst_DI() { tst(addr_mode.DI); }
        void tst_IX() { tst(addr_mode.IX); }
        void tst_IX1() { tst(addr_mode.IX1); }

        // $3e ILLEGAL
        // $6e ILLEGAL
        // $7e ILLEGAL

        // $3f CLR direct                   -01-
        // $6f CLR indexed, 1 byte offset   -01-
        // $7f CLR indexed                  -01-
        //OP_HANDLER_MODE( clr )
        void clr(addr_mode M)
        {
            ARGADDR(M);
            clr_nz();
            SEZ();
            wm(EAD, 0);
        }

        void clr_DI() { clr(addr_mode.DI); }
        void clr_IX() { clr(addr_mode.IX); }
        void clr_IX1() { clr(addr_mode.IX1); }

        // $40 NEGA inherent -***
        //OP_HANDLER( nega )
        void nega()
        {
            u16 r = (u16)(-A);
            clr_nzc();
            set_nzc8(r);
            A = (u8)r;
        }

        // $41 ILLEGAL

        // $42 MUL inherent 0--0
        //OP_HANDLER( mul )
        void mul()
        {
            u16 r = (u16)((u16)A * X);
            clr_hc();
            X = (u8)(r >> 8);
            A = (u8)r;
        }

        // $43 COMA inherent -**1
        //OP_HANDLER( coma )
        void coma()
        {
            A = (u8)~A;
            clr_nz();
            set_nz8(A);
            SEC();
        }

        // $44 LSRA inherent -0**
        //OP_HANDLER( lsra )
        void lsra()
        {
            clr_nzc();
            CC |= (u8)BIT(A, 0);
            A >>= 1;
            set_z8(A);
        }

        // $45 ILLEGAL

        // $46 RORA inherent -***
        //OP_HANDLER( rora )
        void rora()
        {
            u8 r = (u8)(BIT(CC, 0) << 7);
            clr_nzc();
            CC |= (u8)BIT(A, 0);
            r |= (u8)(A >> 1);
            set_nz8(r);
            A = r;
        }

        // $47 ASRA inherent -***
        //OP_HANDLER( asra )
        void asra()
        {
            clr_nzc();
            CC |= (u8)BIT(A, 0);
            A = (u8)((A & 0x80) | (A >> 1));
            set_nz8(A);
        }

        // $48 LSLA inherent -***
        //OP_HANDLER( lsla )
        void lsla()
        {
            u16 r = (u16)(A << 1);
            clr_nzc();
            set_nzc8(r);
            A = (u8)r;
        }

        // $49 ROLA inherent -***
        //OP_HANDLER( rola )
        void rola()
        {
            u16 t = A;
            u16 r = (u16)(BIT(CC, 0) | (t << 1));
            clr_nzc();
            set_nzc8(r);
            A = (u8)r;
        }

        // $4a DECA inherent -**-
        //OP_HANDLER( deca )
        void deca()
        {
            --A;
            clr_nz();
            set_nz8(A);
        }

        // $4b ILLEGAL

        // $4c INCA inherent -**-
        //OP_HANDLER( inca )
        void inca()
        {
            ++A;
            clr_nz();
            set_nz8(A);
        }

        // $4d TSTA inherent -**-
        //OP_HANDLER( tsta )
        void tsta()
        {
            clr_nz();
            set_nz8(A);
        }

        // $4e ILLEGAL

        // $4f CLRA inherent -01-
        //OP_HANDLER( clra )
        void clra()
        {
            A = 0;
            clr_nz();
            SEZ();
        }

        // $50 NEGX inherent -***
        //OP_HANDLER( negx )
        void negx()
        {
            u16 r = (u16)(-X);
            clr_nzc();
            set_nzc8(r);
            X = (u8)r;
        }

        // $51 ILLEGAL

        // $52 ILLEGAL

        // $53 COMX inherent -**1
        //OP_HANDLER( comx )
        void comx()
        {
            X = (u8)~X;
            clr_nz();
            set_nz8(X);
            SEC();
        }

        // $54 LSRX inherent -0**
        //OP_HANDLER( lsrx )
        void lsrx()
        {
            clr_nzc();
            CC |= (u8)BIT(X, 0);
            X >>= 1;
            set_z8(X);
        }

        // $55 ILLEGAL

        // $56 RORX inherent -***
        //OP_HANDLER( rorx )
        void rorx()
        {
            u8 r = (u8)(BIT(CC, 0) << 7);
            clr_nzc();
            CC |= (u8)BIT(X, 0);
            r |= (u8)(X >> 1);
            set_nz8(r);
            X = r;
        }

        // $57 ASRX inherent -***
        //OP_HANDLER( asrx )
        void asrx()
        {
            clr_nzc();
            CC |= (u8)BIT(X, 0);
            X = (u8)((X & 0x80) | (X >> 1));
            set_nz8(X);
        }

        // $58 LSLX inherent -***
        //OP_HANDLER( lslx )
        void lslx()
        {
            u16 r = (u16)(X << 1);
            clr_nzc();
            set_nzc8(r);
            X = (u8)r;
        }

        // $59 ROLX inherent -***
        //OP_HANDLER( rolx )
        void rolx()
        {
            u16 t = X;
            u16 r = (u16)(BIT(CC, 0) | (t << 1));
            clr_nzc();
            set_nzc8(r);
            X = (u8)r;
        }

        // $5a DECX inherent -**-
        //OP_HANDLER( decx )
        void decx()
        {
            --X;
            clr_nz();
            set_nz8(X);
        }

        // $5b ILLEGAL

        // $5c INCX inherent -**-
        //OP_HANDLER( incx )
        void incx()
        {
            ++X;
            clr_nz();
            set_nz8(X);
        }

        // $5d TSTX inherent -**-
        //OP_HANDLER( tstx )
        void tstx()
        {
            clr_nz();
            set_nz8(X);
        }

        // $5e ILLEGAL

        // $5f CLRX inherent -01-
        //OP_HANDLER( clrx )
        void clrx()
        {
            X = 0;
            clr_nz();
            SEZ();
        }

        // $80 RTI inherent ####
        //OP_HANDLER( rti )
        void rti()
        {
            u8 temp;
            pullbyte(out temp); CC = temp;
            pullbyte(out temp); A = temp;
            pullbyte(out temp); X = temp;
            pullword(ref m_pc);
        }

        // $81 RTS inherent ----
        //OP_HANDLER( rts )
        void rts()
        {
            pullword(ref m_pc);
        }

        // $82 ILLEGAL

        // $83 SWI absolute indirect ----
        //OP_HANDLER( swi )
        void swi()
        {
            pushword(m_pc);
            pushbyte(m_x);
            pushbyte(m_a);
            pushbyte(m_cc);
            SEI();
            rm16((u32)(m_params.m_swi_vector & m_params.m_vector_mask), ref m_pc);
        }

        // $84 ILLEGAL

        // $85 ILLEGAL

        // $86 ILLEGAL

        // $87 ILLEGAL

        // $88 ILLEGAL

        // $89 ILLEGAL

        // $8A ILLEGAL

        // $8B ILLEGAL

        // $8C ILLEGAL

        // $8D ILLEGAL

        // $8E STOP inherent    ----
        //OP_HANDLER( stop )
        void stop()
        {
            fatalerror("m6805[{0}]: unimplemented STOP", tag());
        }

        // $8F WAIT inherent    ----
        //OP_HANDLER( wait )
        void wait()
        {
            fatalerror("m6805[{0}]: unimplemented WAIT", tag());
        }


        // $90 ILLEGAL

        // $91 ILLEGAL

        // $92 ILLEGAL

        // $93 ILLEGAL

        // $94 ILLEGAL

        // $95 ILLEGAL

        // $96 ILLEGAL

        // $97 TAX inherent ----
        //OP_HANDLER( tax ) { X = A; }
        void tax() { X = A; }

        // $98 CLC
        //OP_HANDLER( clc ) { CLC; }
        void clc() { CLC(); }

        // $99 SEC
        //OP_HANDLER( sec ) { SEC; }
        void sec() { SEC(); }

        // $9A CLI
        //OP_HANDLER( cli ) { CLI; }
        void cli() { CLI(); }

        // $9B SEI
        //OP_HANDLER( sei ) { SEI; } // TODO: check behaviour if edge-triggered interrupt was pending when this happens
        void sei() { SEI(); } // TODO: check behaviour if edge-triggered interrupt was pending when this happens

        // $9C RSP inherent ----
        //OP_HANDLER( rsp ) { S = SP_MASK; }
        void rsp() { S = (u16)SP_MASK; }

        // $9D NOP inherent ----
        //OP_HANDLER( nop ) { }
        void nop() { }

        // $9E ILLEGAL

        // $9F TXA inherent ----
        //OP_HANDLER( txa )
        void txa()
        {
            A = X;
        }

        // $a0 SUBA immediate               -***
        // $b0 SUBA direct                  -***
        // $c0 SUBA extended                -***
        // $d0 SUBA indexed, 2 byte offset  -***
        // $e0 SUBA indexed, 1 byte offset  -***
        // $f0 SUBA indexed                 -***
        //OP_HANDLER_MODE( suba )
        void suba(addr_mode M)
        {
            u16 t;
            ARGBYTE(M, out t);
            u16 r = (u16)(A - t);
            clr_nzc();
            set_nzc8(r);
            A = (u8)r;
        }

        void suba_IM() { suba(addr_mode.IM); }
        void suba_DI() { suba(addr_mode.DI); }
        void suba_EX() { suba(addr_mode.EX); }
        void suba_IX() { suba(addr_mode.IX); }
        void suba_IX1() { suba(addr_mode.IX1); }
        void suba_IX2() { suba(addr_mode.IX2); }

        // $a1 CMPA immediate               -***
        // $b1 CMPA direct                  -***
        // $c1 CMPA extended                -***
        // $d1 CMPA indexed, 2 byte offset  -***
        // $e1 CMPA indexed, 1 byte offset  -***
        // $f1 CMPA indexed                 -***
        //OP_HANDLER_MODE( cmpa )
        void cmpa(addr_mode M)
        {
            u16 t;
            ARGBYTE(M, out t);
            u16 r = (u16)(A - t);
            clr_nzc();
            set_nzc8(r);
        }

        void cmpa_IM() { cmpa(addr_mode.IM); }
        void cmpa_DI() { cmpa(addr_mode.DI); }
        void cmpa_EX() { cmpa(addr_mode.EX); }
        void cmpa_IX() { cmpa(addr_mode.IX); }
        void cmpa_IX1() { cmpa(addr_mode.IX1); }
        void cmpa_IX2() { cmpa(addr_mode.IX2); }

        // $a2 SBCA immediate               -***
        // $b2 SBCA direct                  -***
        // $c2 SBCA extended                -***
        // $d2 SBCA indexed, 2 byte offset  -***
        // $e2 SBCA indexed, 1 byte offset  -***
        // $f2 SBCA indexed                 -***
        //OP_HANDLER_MODE( sbca )
        void sbca(addr_mode M)
        {
            u16 t;
            ARGBYTE(M, out t);
            u16 r = (u16)(A - t - BIT(CC, 0));
            clr_nzc();
            set_nzc8(r);
            A = (u8)r;
        }

        void sbca_IM() { sbca(addr_mode.IM); }
        void sbca_DI() { sbca(addr_mode.DI); }
        void sbca_EX() { sbca(addr_mode.EX); }
        void sbca_IX() { sbca(addr_mode.IX); }
        void sbca_IX1() { sbca(addr_mode.IX1); }
        void sbca_IX2() { sbca(addr_mode.IX2); }

        // $a3 CPX immediate                -***
        // $b3 CPX direct                   -***
        // $c3 CPX extended                 -***
        // $d3 CPX indexed, 2 byte offset   -***
        // $e3 CPX indexed, 1 byte offset   -***
        // $f3 CPX indexed                  -***
        //OP_HANDLER_MODE( cpx )
        void cpx(addr_mode M)
        {
            u16 t;
            ARGBYTE(M, out t);
            u16 r = (u16)(X - t);
            clr_nzc();
            set_nzc8(r);
        }

        void cpx_IM() { cpx(addr_mode.IM); }
        void cpx_DI() { cpx(addr_mode.DI); }
        void cpx_EX() { cpx(addr_mode.EX); }
        void cpx_IX() { cpx(addr_mode.IX); }
        void cpx_IX1() { cpx(addr_mode.IX1); }
        void cpx_IX2() { cpx(addr_mode.IX2); }

        // $a4 ANDA immediate               -**-
        // $b4 ANDA direct                  -**-
        // $c4 ANDA extended                -**-
        // $d4 ANDA indexed, 2 byte offset  -**-
        // $e4 ANDA indexed, 1 byte offset  -**-
        // $f4 ANDA indexed                 -**-
        //OP_HANDLER_MODE( anda )
        void anda(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            A &= t;
            clr_nz();
            set_nz8(A);
        }

        void anda_IM() { anda(addr_mode.IM); }
        void anda_DI() { anda(addr_mode.DI); }
        void anda_EX() { anda(addr_mode.EX); }
        void anda_IX() { anda(addr_mode.IX); }
        void anda_IX1() { anda(addr_mode.IX1); }
        void anda_IX2() { anda(addr_mode.IX2); }

        // $a5 BITA immediate               -**-
        // $b5 BITA direct                  -**-
        // $c5 BITA extended                -**-
        // $d5 BITA indexed, 2 byte offset  -**-
        // $e5 BITA indexed, 1 byte offset  -**-
        // $f5 BITA indexed                 -**-
        //OP_HANDLER_MODE( bita )
        void bita(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            u8 r = (u8)(A & t);
            clr_nz();
            set_nz8(r);
        }

        void bita_IM() { bita(addr_mode.IM); }
        void bita_DI() { bita(addr_mode.DI); }
        void bita_EX() { bita(addr_mode.EX); }
        void bita_IX() { bita(addr_mode.IX); }
        void bita_IX1() { bita(addr_mode.IX1); }
        void bita_IX2() { bita(addr_mode.IX2); }

        // $a6 LDA immediate                -**-
        // $b6 LDA direct                   -**-
        // $c6 LDA extended                 -**-
        // $d6 LDA indexed, 2 byte offset   -**-
        // $e6 LDA indexed, 1 byte offset   -**-
        // $f6 LDA indexed                  -**-
        //OP_HANDLER_MODE( lda )
        void lda(addr_mode M)
        {
            u8 temp;
            ARGBYTE(M, out temp); A = temp;
            clr_nz();
            set_nz8(A);
        }

        void lda_IM() { lda(addr_mode.IM); }
        void lda_DI() { lda(addr_mode.DI); }
        void lda_EX() { lda(addr_mode.EX); }
        void lda_IX() { lda(addr_mode.IX); }
        void lda_IX1() { lda(addr_mode.IX1); }
        void lda_IX2() { lda(addr_mode.IX2); }

        // $a7 ILLEGAL
        // $b7 STA direct                   -**-
        // $c7 STA extended                 -**-
        // $d7 STA indexed, 2 byte offset   -**-
        // $e7 STA indexed, 1 byte offset   -**-
        // $f7 STA indexed                  -**-
        //OP_HANDLER_MODE( sta )
        void sta(addr_mode M)
        {
            clr_nz();
            set_nz8(A);
            ARGADDR(M);
            wm(EAD, A);
        }

        void sta_IM() { sta(addr_mode.IM); }
        void sta_DI() { sta(addr_mode.DI); }
        void sta_EX() { sta(addr_mode.EX); }
        void sta_IX() { sta(addr_mode.IX); }
        void sta_IX1() { sta(addr_mode.IX1); }
        void sta_IX2() { sta(addr_mode.IX2); }

        // $a8 EORA immediate               -**-
        // $b8 EORA direct                  -**-
        // $c8 EORA extended                -**-
        // $d8 EORA indexed, 2 byte offset  -**-
        // $e8 EORA indexed, 1 byte offset  -**-
        // $f8 EORA indexed                 -**-
        //OP_HANDLER_MODE( eora )
        void eora(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            A ^= t;
            clr_nz();
            set_nz8(A);
        }

        void eora_IM() { eora(addr_mode.IM); }
        void eora_DI() { eora(addr_mode.DI); }
        void eora_EX() { eora(addr_mode.EX); }
        void eora_IX() { eora(addr_mode.IX); }
        void eora_IX1() { eora(addr_mode.IX1); }
        void eora_IX2() { eora(addr_mode.IX2); }

        // $a9 ADCA immediate               ****
        // $b9 ADCA direct                  ****
        // $c9 ADCA extended                ****
        // $d9 ADCA indexed, 2 byte offset  ****
        // $e9 ADCA indexed, 1 byte offset  ****
        // $f9 ADCA indexed                 ****
        //OP_HANDLER_MODE( adca )
        void adca(addr_mode M)
        {
            u16 t;
            ARGBYTE(M, out t);
            u16 r = (u16)(A + t + BIT(CC, 0));
            clr_hnzc();
            set_hnzc8(A, (u8)t, r);
            A = (u8)r;
        }

        void adca_IM() { adca(addr_mode.IM); }
        void adca_DI() { adca(addr_mode.DI); }
        void adca_EX() { adca(addr_mode.EX); }
        void adca_IX() { adca(addr_mode.IX); }
        void adca_IX1() { adca(addr_mode.IX1); }
        void adca_IX2() { adca(addr_mode.IX2); }

        // $aa ORA immediate                -**-
        // $ba ORA direct                   -**-
        // $ca ORA extended                 -**-
        // $da ORA indexed, 2 byte offset   -**-
        // $ea ORA indexed, 1 byte offset   -**-
        // $fa ORA indexed                  -**-
        //OP_HANDLER_MODE( ora )
        void ora(addr_mode M)
        {
            u8 t;
            ARGBYTE(M, out t);
            A |= t;
            clr_nz();
            set_nz8(A);
        }

        void ora_IM() { ora(addr_mode.IM); }
        void ora_DI() { ora(addr_mode.DI); }
        void ora_EX() { ora(addr_mode.EX); }
        void ora_IX() { ora(addr_mode.IX); }
        void ora_IX1() { ora(addr_mode.IX1); }
        void ora_IX2() { ora(addr_mode.IX2); }

        // $ab ADDA immediate               ****
        // $bb ADDA direct                  ****
        // $cb ADDA extended                ****
        // $db ADDA indexed, 2 byte offset  ****
        // $eb ADDA indexed, 1 byte offset  ****
        // $fb ADDA indexed                 ****
        //OP_HANDLER_MODE( adda )
        void adda(addr_mode M)
        {
            u16 t;
            ARGBYTE(M, out t);
            u16 r = (u16)(A + t);
            clr_hnzc();
            set_hnzc8(A, (u8)t, r);
            A = (u8)r;
        }

        void adda_IM() { adda(addr_mode.IM); }
        void adda_DI() { adda(addr_mode.DI); }
        void adda_EX() { adda(addr_mode.EX); }
        void adda_IX() { adda(addr_mode.IX); }
        void adda_IX1() { adda(addr_mode.IX1); }
        void adda_IX2() { adda(addr_mode.IX2); }

        // $ac ILLEGAL
        // $bc JMP direct                   -***
        // $cc JMP extended                 -***
        // $dc JMP indexed, 2 byte offset   -***
        // $ec JMP indexed, 1 byte offset   -***
        // $fc JMP indexed                  -***
        //OP_HANDLER_MODE( jmp )
        void jmp(addr_mode M)
        {
            ARGADDR(M);
            PC = EA;
        }

        void jmp_IM() { jmp(addr_mode.IM); }
        void jmp_DI() { jmp(addr_mode.DI); }
        void jmp_EX() { jmp(addr_mode.EX); }
        void jmp_IX() { jmp(addr_mode.IX); }
        void jmp_IX1() { jmp(addr_mode.IX1); }
        void jmp_IX2() { jmp(addr_mode.IX2); }

        // $ad BSR ----
        //OP_HANDLER( bsr )
        void bsr()
        {
            u8 t;
            immbyte(out t);
            pushword(m_pc);
            PC = (u16)(PC + SIGNED(t));
        }

        // $bd JSR direct                   ----
        // $cd JSR extended                 ----
        // $dd JSR indexed, 2 byte offset   ----
        // $ed JSR indexed, 1 byte offset   ----
        // $fd JSR indexed                  ----
        //OP_HANDLER_MODE( jsr )
        void jsr(addr_mode M)
        {
            ARGADDR(M);
            pushword(m_pc);
            PC = EA;
        }

        void jsr_IM() { jsr(addr_mode.IM); }
        void jsr_DI() { jsr(addr_mode.DI); }
        void jsr_EX() { jsr(addr_mode.EX); }
        void jsr_IX() { jsr(addr_mode.IX); }
        void jsr_IX1() { jsr(addr_mode.IX1); }
        void jsr_IX2() { jsr(addr_mode.IX2); }

        // $ae LDX immediate                -**-
        // $be LDX direct                   -**-
        // $ce LDX extended                 -**-
        // $de LDX indexed, 2 byte offset   -**-
        // $ee LDX indexed, 1 byte offset   -**-
        // $fe LDX indexed                  -**-
        //OP_HANDLER_MODE( ldx )
        void ldx(addr_mode M)
        {
            u8 temp;
            ARGBYTE(M, out temp); X = temp;
            clr_nz();
            set_nz8(X);
        }

        void ldx_IM() { ldx(addr_mode.IM); }
        void ldx_DI() { ldx(addr_mode.DI); }
        void ldx_EX() { ldx(addr_mode.EX); }
        void ldx_IX() { ldx(addr_mode.IX); }
        void ldx_IX1() { ldx(addr_mode.IX1); }
        void ldx_IX2() { ldx(addr_mode.IX2); }

        // $af ILLEGAL
        // $bf STX direct                   -**-
        // $cf STX extended                 -**-
        // $df STX indexed, 2 byte offset   -**-
        // $ef STX indexed, 1 byte offset   -**-
        // $ff STX indexed                  -**-
        //OP_HANDLER_MODE( stx )
        void stx(addr_mode M)
        {
            clr_nz();
            set_nz8(X);
            ARGADDR(M);
            wm(EAD, X);
        }

        void stx_IM() { stx(addr_mode.IM); }
        void stx_DI() { stx(addr_mode.DI); }
        void stx_EX() { stx(addr_mode.EX); }
        void stx_IX() { stx(addr_mode.IX); }
        void stx_IX1() { stx(addr_mode.IX1); }
        void stx_IX2() { stx(addr_mode.IX2); }
    }
}
