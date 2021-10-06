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
        //#define OP_HANDLER(name) template<bool big> void m6805_base_device::name()
        //#define OP_HANDLER_BIT(name) template <bool big, unsigned B> void m6805_base_device::name()
        //#define OP_HANDLER_BRA(name) template <bool big, bool C> void m6805_base_device::name()
        //#define OP_HANDLER_MODE(name) template <bool big, m6805_base_device::addr_mode M> void m6805_base_device::name()


        //OP_HANDLER( illegal )
        void illegal()
        {
            logerror("M6805: illegal opcode\n");
        }

        void illegal_true() { illegal(); }
        void illegal_false() { illegal(); }


        // $00/$02/$04/$06/$08/$0A/$0C/$0E BRSET direct,relative ---*
        //OP_HANDLER_BIT( brset )
        void brset(bool big, int B)
        {
            u8 t;
            u8 r;
            DIRBYTE(big, out r);
            immbyte(big, out t);
            CLC();
            if (g.BIT(r, B) != 0) { SEC(); PC = (u16)(PC + SIGNED(t)); }
        }

        void brset_true_0() { brset(true, 0); }
        void brset_false_0() { brset(false, 0); }
        void brset_true_1() { brset(true, 1); }
        void brset_false_1() { brset(false, 1); }
        void brset_true_2() { brset(true, 2); }
        void brset_false_2() { brset(false, 2); }
        void brset_true_3() { brset(true, 3); }
        void brset_false_3() { brset(false, 3); }
        void brset_true_4() { brset(true, 4); }
        void brset_false_4() { brset(false, 4); }
        void brset_true_5() { brset(true, 5); }
        void brset_false_5() { brset(false, 5); }
        void brset_true_6() { brset(true, 6); }
        void brset_false_6() { brset(false, 6); }
        void brset_true_7() { brset(true, 7); }
        void brset_false_7() { brset(false, 7); }

        // $01/$03/$05/$07/$09/$0B/$0D/$0F BRCLR direct,relative ---*
        //OP_HANDLER_BIT( brclr )
        void brclr(bool big, int B)
        {
            u8 t;
            u8 r;
            DIRBYTE(big, out r);
            immbyte(big, out t);
            SEC();
            if (g.BIT(r, B) == 0) { CLC(); PC = (u16)(PC + SIGNED(t)); }
        }

        void brclr_true_0() { brclr(true, 0); }
        void brclr_false_0() { brclr(false, 0); }
        void brclr_true_1() { brclr(true, 1); }
        void brclr_false_1() { brclr(false, 1); }
        void brclr_true_2() { brclr(true, 2); }
        void brclr_false_2() { brclr(false, 2); }
        void brclr_true_3() { brclr(true, 3); }
        void brclr_false_3() { brclr(false, 3); }
        void brclr_true_4() { brclr(true, 4); }
        void brclr_false_4() { brclr(false, 4); }
        void brclr_true_5() { brclr(true, 5); }
        void brclr_false_5() { brclr(false, 5); }
        void brclr_true_6() { brclr(true, 6); }
        void brclr_false_6() { brclr(false, 6); }
        void brclr_true_7() { brclr(true, 7); }
        void brclr_false_7() { brclr(false, 7); }

        // $10/$12/$14/$16/$18/$1A/$1C/$1E BSET direct ----
        //OP_HANDLER_BIT( bset )
        void bset(bool big, int B)
        {
            u8 t;
            DIRBYTE(big, out t);
            wm(big, EAD, (u8)(t | (1 << B)));
        }

        void bset_true_0() { bset(true, 0); }
        void bset_false_0() { bset(false, 0); }
        void bset_true_1() { bset(true, 1); }
        void bset_false_1() { bset(false, 1); }
        void bset_true_2() { bset(true, 2); }
        void bset_false_2() { bset(false, 2); }
        void bset_true_3() { bset(true, 3); }
        void bset_false_3() { bset(false, 3); }
        void bset_true_4() { bset(true, 4); }
        void bset_false_4() { bset(false, 4); }
        void bset_true_5() { bset(true, 5); }
        void bset_false_5() { bset(false, 5); }
        void bset_true_6() { bset(true, 6); }
        void bset_false_6() { bset(false, 6); }
        void bset_true_7() { bset(true, 7); }
        void bset_false_7() { bset(false, 7); }

        // $11/$13/$15/$17/$19/$1B/$1D/$1F BCLR direct ----
        //OP_HANDLER_BIT( bclr )
        void bclr(bool big, int B)
        {
            u8 t;
            DIRBYTE(big, out t);
            wm(big, EAD, (u8)(t & ~(1 << B)));
        }

        void bclr_true_0() { bclr(true, 0); }
        void bclr_false_0() { bclr(false, 0); }
        void bclr_true_1() { bclr(true, 1); }
        void bclr_false_1() { bclr(false, 1); }
        void bclr_true_2() { bclr(true, 2); }
        void bclr_false_2() { bclr(false, 2); }
        void bclr_true_3() { bclr(true, 3); }
        void bclr_false_3() { bclr(false, 3); }
        void bclr_true_4() { bclr(true, 4); }
        void bclr_false_4() { bclr(false, 4); }
        void bclr_true_5() { bclr(true, 5); }
        void bclr_false_5() { bclr(false, 5); }
        void bclr_true_6() { bclr(true, 6); }
        void bclr_false_6() { bclr(false, 6); }
        void bclr_true_7() { bclr(true, 7); }
        void bclr_false_7() { bclr(false, 7); }

        // $20 BRA relative ----
        // $21 BRN relative ----
        //OP_HANDLER_BRA( bra ) { BRANCH( true ); }
        void bra(bool big, bool C) { BRANCH(big, C, true); }
        void bra_true_true() { bra(true, true); }
        void bra_false_true() { bra(false, true); }
        void bra_true_false() { bra(true, false); }
        void bra_false_false() { bra(false, false); }

        // $22 BHI relative ----
        // $23 BLS relative ----
        //OP_HANDLER_BRA( bhi ) { BRANCH( !(CC & (CFLAG | ZFLAG)) ); }
        void bhi(bool big, bool C) { BRANCH(big, C, (CC & (CFLAG | ZFLAG)) == 0 ); }
        void bhi_true_true() { bhi(true, true); }
        void bhi_false_true() { bhi(false, true); }
        void bhi_true_false() { bhi(true, false); }
        void bhi_false_false() { bhi(false, false); }

        // $24 BCC relative ----
        // $25 BCS relative ----
        //OP_HANDLER_BRA( bcc ) { BRANCH( !(CC & CFLAG) ); }
        void bcc(bool big, bool C) { BRANCH(big, C, (CC & CFLAG) == 0 ); }
        void bcc_true_true() { bcc(true, true); }
        void bcc_false_true() { bcc(false, true); }
        void bcc_true_false() { bcc(true, false); }
        void bcc_false_false() { bcc(false, false); }

        // $26 BNE relative ----
        // $27 BEQ relative ----
        //OP_HANDLER_BRA( bne ) { BRANCH( !(CC & ZFLAG) ); }
        void bne(bool big, bool C) { BRANCH(big, C, (CC & ZFLAG) == 0 ); }
        void bne_true_true() { bne(true, true); }
        void bne_false_true() { bne(false, true); }
        void bne_true_false() { bne(true, false); }
        void bne_false_false() { bne(false, false); }

        // $28 BHCC relative ----
        // $29 BHCS relative ----
        //OP_HANDLER_BRA( bhcc ) { BRANCH( !(CC & HFLAG) ); }
        void bhcc(bool big, bool C) { BRANCH(big, C, (CC & HFLAG) == 0 ); }
        void bhcc_true_true() { bhcc(true, true); }
        void bhcc_false_true() { bhcc(false, true); }
        void bhcc_true_false() { bhcc(true, false); }
        void bhcc_false_false() { bhcc(false, false); }

        // $2a BPL relative ----
        // $2b BMI relative ----
        //OP_HANDLER_BRA( bpl ) { BRANCH( !(CC & NFLAG) ); }
        void bpl(bool big, bool C) { BRANCH(big, C, (CC & NFLAG) == 0 ); }
        void bpl_true_true() { bpl(true, true); }
        void bpl_false_true() { bpl(false, true); }
        void bpl_true_false() { bpl(true, false); }
        void bpl_false_false() { bpl(false, false); }

        // $2c BMC relative ----
        // $2d BMS relative ----
        //OP_HANDLER_BRA( bmc ) { BRANCH( !(CC & IFLAG) ); }
        void bmc(bool big, bool C) { BRANCH(big, C, (CC & IFLAG) == 0 ); }
        void bmc_true_true() { bmc(true, true); }
        void bmc_false_true() { bmc(false, true); }
        void bmc_true_false() { bmc(true, false); }
        void bmc_false_false() { bmc(false, false); }

        // $2e BIL relative ----
        // $2f BIH relative ----
        //OP_HANDLER_BRA( bil ) { BRANCH( test_il() ); }
        void bil(bool big, bool C) { BRANCH(big, C, test_il() ); }
        void bil_true_true() { bil(true, true); }
        void bil_false_true() { bil(false, true); }
        void bil_true_false() { bil(true, false); }
        void bil_false_false() { bil(false, false); }

        // $30 NEG direct                   -***
        // $60 NEG indexed, 1 byte offset   -***
        // $70 NEG indexed                  -***
        //OP_HANDLER_MODE( neg )
        void neg(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            u16 r = (u16)(-t);
            clr_nzc();
            set_nzc8(r);
            wm(big, EAD, (u8)r);
        }

        void neg_true_DI() { neg(true, addr_mode.DI); }
        void neg_false_DI() { neg(false, addr_mode.DI); }
        void neg_true_IX() { neg(true, addr_mode.IX); }
        void neg_false_IX() { neg(false, addr_mode.IX); }
        void neg_true_IX1() { neg(true, addr_mode.IX1); }
        void neg_false_IX1() { neg(false, addr_mode.IX1); }

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
        void com(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            t = (u8)~t;
            clr_nz();
            set_nz8(t);
            SEC();
            wm(big, EAD, t);
        }

        void com_true_DI() { com(true, addr_mode.DI); }
        void com_false_DI() { com(false, addr_mode.DI); }
        void com_true_IX() { com(true, addr_mode.IX); }
        void com_false_IX() { com(false, addr_mode.IX); }
        void com_true_IX1() { com(true, addr_mode.IX1); }
        void com_false_IX1() { com(false, addr_mode.IX1); }

        // $34 LSR direct                   -0**
        // $64 LSR indexed, 1 byte offset   -0**
        // $74 LSR indexed                  -0**
        //OP_HANDLER_MODE( lsr )
        void lsr(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            clr_nzc();
            CC |= (u8)g.BIT(t, 0);
            t >>= 1;
            set_z8(t);
            wm(big, EAD, t);
        }

        void lsr_true_DI() { lsr(true, addr_mode.DI); }
        void lsr_false_DI() { lsr(false, addr_mode.DI); }
        void lsr_true_IX() { lsr(true, addr_mode.IX); }
        void lsr_false_IX() { lsr(false, addr_mode.IX); }
        void lsr_true_IX1() { lsr(true, addr_mode.IX1); }
        void lsr_false_IX1() { lsr(false, addr_mode.IX1); }

        // $35 ILLEGAL
        // $65 ILLEGAL
        // $75 ILLEGAL

        // $36 ROR direct                   -***
        // $66 ROR indexed, 1 byte offset   -***
        // $76 ROR indexed                  -***
        //OP_HANDLER_MODE( ror )
        void ror(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            u8 r = (u8)(g.BIT(CC, 0) << 7);
            clr_nzc();
            CC |= (u8)g.BIT(t, 0);
            r |= (u8)(t >> 1);
            set_nz8(r);
            wm(big, EAD, r);
        }

        void ror_true_DI() { ror(true, addr_mode.DI); }
        void ror_false_DI() { ror(false, addr_mode.DI); }
        void ror_true_IX() { ror(true, addr_mode.IX); }
        void ror_false_IX() { ror(false, addr_mode.IX); }
        void ror_true_IX1() { ror(true, addr_mode.IX1); }
        void ror_false_IX1() { ror(false, addr_mode.IX1); }

        // $37 ASR direct                   -***
        // $67 ASR indexed, 1 byte offset   -***
        // $77 ASR indexed                  -***
        //OP_HANDLER_MODE( asr )
        void asr(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            clr_nzc();
            CC |= (u8)g.BIT(t, 0);
            t = (u8)((t >> 1) | (t & 0x80));
            set_nz8(t);
            wm(big, EAD, t);
        }

        void asr_true_DI() { asr(true, addr_mode.DI); }
        void asr_false_DI() { asr(false, addr_mode.DI); }
        void asr_true_IX() { asr(true, addr_mode.IX); }
        void asr_false_IX() { asr(false, addr_mode.IX); }
        void asr_true_IX1() { asr(true, addr_mode.IX1); }
        void asr_false_IX1() { asr(false, addr_mode.IX1); }

        // $38 LSL direct                   -***
        // $68 LSL indexed, 1 byte offset   -***
        // $78 LSL indexed                  -***
        //OP_HANDLER_MODE( lsl )
        void lsl(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            u16 r = (u16)(t << 1);
            clr_nzc();
            set_nzc8(r);
            wm(big, EAD, (u8)r);
        }

        void lsl_true_DI() { lsl(true, addr_mode.DI); }
        void lsl_false_DI() { lsl(false, addr_mode.DI); }
        void lsl_true_IX() { lsl(true, addr_mode.IX); }
        void lsl_false_IX() { lsl(false, addr_mode.IX); }
        void lsl_true_IX1() { lsl(true, addr_mode.IX1); }
        void lsl_false_IX1() { lsl(false, addr_mode.IX1); }

        // $39 ROL direct                   -***
        // $69 ROL indexed, 1 byte offset   -***
        // $79 ROL indexed                  -***
        //OP_HANDLER_MODE( rol )
        void rol(bool big, addr_mode M)
        {
            u16 t;
            ARGBYTE(big, M, out t);
            u16 r = (u16)(g.BIT(CC, 0) | (t << 1));
            clr_nzc();
            set_nzc8(r);
            wm(big, EAD, (u8)r);
        }

        void rol_true_DI() { rol(true, addr_mode.DI); }
        void rol_false_DI() { rol(false, addr_mode.DI); }
        void rol_true_IX() { rol(true, addr_mode.IX); }
        void rol_false_IX() { rol(false, addr_mode.IX); }
        void rol_true_IX1() { rol(true, addr_mode.IX1); }
        void rol_false_IX1() { rol(false, addr_mode.IX1); }

        // $3a DEC direct                   -**-
        // $6a DEC indexed, 1 byte offset   -**-
        //OP_HANDLER_MODE( dec )
        void dec(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            --t;
            clr_nz();
            set_nz8(t);
            wm(big, EAD, t);
        }

        void dec_true_DI() { dec(true, addr_mode.DI); }
        void dec_false_DI() { dec(false, addr_mode.DI); }
        void dec_true_IX() { dec(true, addr_mode.IX); }
        void dec_false_IX() { dec(false, addr_mode.IX); }
        void dec_true_IX1() { dec(true, addr_mode.IX1); }
        void dec_false_IX1() { dec(false, addr_mode.IX1); }

        // $3b ILLEGAL
        // $6b ILLEGAL
        // $7b ILLEGAL

        // $3c INC direct                   -**-
        // $6c INC indexed, 1 byte offset   -**-
        // $7c INC indexed                  -**-
        //OP_HANDLER_MODE( inc )
        void inc(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            ++t;
            clr_nz();
            set_nz8(t);
            wm(big, EAD, t);
        }

        void inc_true_DI() { inc(true, addr_mode.DI); }
        void inc_false_DI() { inc(false, addr_mode.DI); }
        void inc_true_IX() { inc(true, addr_mode.IX); }
        void inc_false_IX() { inc(false, addr_mode.IX); }
        void inc_true_IX1() { inc(true, addr_mode.IX1); }
        void inc_false_IX1() { inc(false, addr_mode.IX1); }

        // $3d TST direct                   -**-
        // $6d TST indexed, 1 byte offset   -**-
        // $7d TST indexed                  -**-
        //OP_HANDLER_MODE( tst )
        void tst(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            clr_nz();
            set_nz8(t);
        }

        void tst_true_DI() { tst(true, addr_mode.DI); }
        void tst_false_DI() { tst(false, addr_mode.DI); }
        void tst_true_IX() { tst(true, addr_mode.IX); }
        void tst_false_IX() { tst(false, addr_mode.IX); }
        void tst_true_IX1() { tst(true, addr_mode.IX1); }
        void tst_false_IX1() { tst(false, addr_mode.IX1); }

        // $3e ILLEGAL
        // $6e ILLEGAL
        // $7e ILLEGAL

        // $3f CLR direct                   -01-
        // $6f CLR indexed, 1 byte offset   -01-
        // $7f CLR indexed                  -01-
        //OP_HANDLER_MODE( clr )
        void clr(bool big, addr_mode M)
        {
            ARGADDR(big, M);
            clr_nz();
            SEZ();
            wm(big, EAD, 0);
        }

        void clr_true_DI() { clr(true, addr_mode.DI); }
        void clr_false_DI() { clr(false, addr_mode.DI); }
        void clr_true_IX() { clr(true, addr_mode.IX); }
        void clr_false_IX() { clr(false, addr_mode.IX); }
        void clr_true_IX1() { clr(true, addr_mode.IX1); }
        void clr_false_IX1() { clr(false, addr_mode.IX1); }

        // $40 NEGA inherent -***
        //OP_HANDLER( nega )
        void nega()
        {
            u16 r = (u16)(-A);
            clr_nzc();
            set_nzc8(r);
            A = (u8)r;
        }

        void nega_true() { nega(); }
        void nega_false() { nega(); }

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

        void coma_true() { coma(); }
        void coma_false() { coma(); }

        // $44 LSRA inherent -0**
        //OP_HANDLER( lsra )
        void lsra()
        {
            clr_nzc();
            CC |= (u8)g.BIT(A, 0);
            A >>= 1;
            set_z8(A);
        }

        void lsra_true() { lsra(); }
        void lsra_false() { lsra(); }

        // $45 ILLEGAL

        // $46 RORA inherent -***
        //OP_HANDLER( rora )
        void rora()
        {
            u8 r = (u8)(g.BIT(CC, 0) << 7);
            clr_nzc();
            CC |= (u8)g.BIT(A, 0);
            r |= (u8)(A >> 1);
            set_nz8(r);
            A = r;
        }

        void rora_true() { rora(); }
        void rora_false() { rora(); }

        // $47 ASRA inherent -***
        //OP_HANDLER( asra )
        void asra()
        {
            clr_nzc();
            CC |= (u8)g.BIT(A, 0);
            A = (u8)((A & 0x80) | (A >> 1));
            set_nz8(A);
        }

        void asra_true() { asra(); }
        void asra_false() { asra(); }

        // $48 LSLA inherent -***
        //OP_HANDLER( lsla )
        void lsla()
        {
            u16 r = (u16)(A << 1);
            clr_nzc();
            set_nzc8(r);
            A = (u8)r;
        }

        void lsla_true() { lsla(); }
        void lsla_false() { lsla(); }

        // $49 ROLA inherent -***
        //OP_HANDLER( rola )
        void rola()
        {
            u16 t = A;
            u16 r = (u16)(g.BIT(CC, 0) | (t << 1));
            clr_nzc();
            set_nzc8(r);
            A = (u8)r;
        }

        void rola_true() { rola(); }
        void rola_false() { rola(); }

        // $4a DECA inherent -**-
        //OP_HANDLER( deca )
        void deca()
        {
            --A;
            clr_nz();
            set_nz8(A);
        }

        void deca_true() { deca(); }
        void deca_false() { deca(); }

        // $4b ILLEGAL

        // $4c INCA inherent -**-
        //OP_HANDLER( inca )
        void inca()
        {
            ++A;
            clr_nz();
            set_nz8(A);
        }

        void inca_true() { inca(); }
        void inca_false() { inca(); }

        // $4d TSTA inherent -**-
        //OP_HANDLER( tsta )
        void tsta()
        {
            clr_nz();
            set_nz8(A);
        }

        void tsta_true() { tsta(); }
        void tsta_false() { tsta(); }

        // $4e ILLEGAL

        // $4f CLRA inherent -01-
        //OP_HANDLER( clra )
        void clra()
        {
            A = 0;
            clr_nz();
            SEZ();
        }

        void clra_true() { clra(); }
        void clra_false() { clra(); }

        // $50 NEGX inherent -***
        //OP_HANDLER( negx )
        void negx()
        {
            u16 r = (u16)(-X);
            clr_nzc();
            set_nzc8(r);
            X = (u8)r;
        }

        void negx_true() { negx(); }
        void negx_false() { negx(); }

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

        void comx_true() { comx(); }
        void comx_false() { comx(); }

        // $54 LSRX inherent -0**
        //OP_HANDLER( lsrx )
        void lsrx()
        {
            clr_nzc();
            CC |= (u8)g.BIT(X, 0);
            X >>= 1;
            set_z8(X);
        }

        void lsrx_true() { lsrx(); }
        void lsrx_false() { lsrx(); }

        // $55 ILLEGAL

        // $56 RORX inherent -***
        //OP_HANDLER( rorx )
        void rorx()
        {
            u8 r = (u8)(g.BIT(CC, 0) << 7);
            clr_nzc();
            CC |= (u8)g.BIT(X, 0);
            r |= (u8)(X >> 1);
            set_nz8(r);
            X = r;
        }

        void rorx_true() { rorx(); }
        void rorx_false() { rorx(); }

        // $57 ASRX inherent -***
        //OP_HANDLER( asrx )
        void asrx()
        {
            clr_nzc();
            CC |= (u8)g.BIT(X, 0);
            X = (u8)((X & 0x80) | (X >> 1));
            set_nz8(X);
        }

        void asrx_true() { asrx(); }
        void asrx_false() { asrx(); }

        // $58 LSLX inherent -***
        //OP_HANDLER( lslx )
        void lslx()
        {
            u16 r = (u16)(X << 1);
            clr_nzc();
            set_nzc8(r);
            X = (u8)r;
        }

        void lslx_true() { lslx(); }
        void lslx_false() { lslx(); }

        // $59 ROLX inherent -***
        //OP_HANDLER( rolx )
        void rolx()
        {
            u16 t = X;
            u16 r = (u16)(g.BIT(CC, 0) | (t << 1));
            clr_nzc();
            set_nzc8(r);
            X = (u8)r;
        }

        void rolx_true() { rolx(); }
        void rolx_false() { rolx(); }

        // $5a DECX inherent -**-
        //OP_HANDLER( decx )
        void decx()
        {
            --X;
            clr_nz();
            set_nz8(X);
        }

        void decx_true() { decx(); }
        void decx_false() { decx(); }

        // $5b ILLEGAL

        // $5c INCX inherent -**-
        //OP_HANDLER( incx )
        void incx()
        {
            ++X;
            clr_nz();
            set_nz8(X);
        }

        void incx_true() { incx(); }
        void incx_false() { incx(); }

        // $5d TSTX inherent -**-
        //OP_HANDLER( tstx )
        void tstx()
        {
            clr_nz();
            set_nz8(X);
        }

        void tstx_true() { tstx(); }
        void tstx_false() { tstx(); }

        // $5e ILLEGAL

        // $5f CLRX inherent -01-
        //OP_HANDLER( clrx )
        void clrx()
        {
            X = 0;
            clr_nz();
            SEZ();
        }

        void clrx_true() { clrx(); }
        void clrx_false() { clrx(); }

        // $80 RTI inherent ####
        //OP_HANDLER( rti )
        void rti(bool big)
        {
            u8 temp;
            pullbyte(big, out temp); CC = temp;
            pullbyte(big, out temp); A = temp;
            pullbyte(big, out temp); X = temp;
            pullword(big, ref m_pc);
        }

        void rti_true() { rti(true); }
        void rti_false() { rti(false); }

        // $81 RTS inherent ----
        //OP_HANDLER( rts )
        void rts(bool big)
        {
            pullword(big, ref m_pc);
        }

        void rts_true() { rts(true); }
        void rts_false() { rts(false); }

        // $82 ILLEGAL

        // $83 SWI absolute indirect ----
        //OP_HANDLER( swi )
        void swi(bool big)
        {
            pushword(big, m_pc);
            pushbyte(big, m_x);
            pushbyte(big, m_a);
            pushbyte(big, m_cc);
            SEI();
            rm16(big, (u32)(m_params.m_swi_vector & m_params.m_vector_mask), ref m_pc);
        }

        void swi_true() { swi(true); }
        void swi_false() { swi(false); }

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
        new void stop()
        {
            g.fatalerror("m6805[{0}]: unimplemented STOP", tag());
        }

        void stop_true() { stop(); }
        void stop_false() { stop(); }

        // $8F WAIT inherent    ----
        //OP_HANDLER( wait )
        void wait()
        {
            g.fatalerror("m6805[{0}]: unimplemented WAIT", tag());
        }

        void wait_true() { wait(); }
        void wait_false() { wait(); }


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

        void tax_true() { tax(); }
        void tax_false() { tax(); }

        // $98 CLC
        //OP_HANDLER( clc ) { CLC; }
        void clc() { CLC(); }

        void clc_true() { clc(); }
        void clc_false() { clc(); }

        // $99 SEC
        //OP_HANDLER( sec ) { SEC; }
        void sec() { SEC(); }

        void sec_true() { sec(); }
        void sec_false() { sec(); }

        // $9A CLI
        //OP_HANDLER( cli ) { CLI; }
        void cli() { CLI(); }

        void cli_true() { cli(); }
        void cli_false() { cli(); }

        // $9B SEI
        //OP_HANDLER( sei ) { SEI; } // TODO: check behaviour if edge-triggered interrupt was pending when this happens
        void sei() { SEI(); } // TODO: check behaviour if edge-triggered interrupt was pending when this happens

        void sei_true() { sei(); }
        void sei_false() { sei(); }

        // $9C RSP inherent ----
        //OP_HANDLER( rsp ) { S = SP_MASK; }
        void rsp() { S = (u16)SP_MASK; }

        void rsp_true() { rsp(); }
        void rsp_false() { rsp(); }

        // $9D NOP inherent ----
        //OP_HANDLER( nop ) { }
        void nop() { }

        void nop_true() { nop(); }
        void nop_false() { nop(); }

        // $9E ILLEGAL

        // $9F TXA inherent ----
        //OP_HANDLER( txa )
        void txa()
        {
            A = X;
        }

        void txa_true() { txa(); }
        void txa_false() { txa(); }

        // $a0 SUBA immediate               -***
        // $b0 SUBA direct                  -***
        // $c0 SUBA extended                -***
        // $d0 SUBA indexed, 2 byte offset  -***
        // $e0 SUBA indexed, 1 byte offset  -***
        // $f0 SUBA indexed                 -***
        //OP_HANDLER_MODE( suba )
        void suba(bool big, addr_mode M)
        {
            u16 t;
            ARGBYTE(big, M, out t);
            u16 r = (u16)(A - t);
            clr_nzc();
            set_nzc8(r);
            A = (u8)r;
        }

        void suba_true_IM() { suba(true, addr_mode.IM); }
        void suba_false_IM() { suba(false, addr_mode.IM); }
        void suba_true_DI() { suba(true, addr_mode.DI); }
        void suba_false_DI() { suba(false, addr_mode.DI); }
        void suba_true_EX() { suba(true, addr_mode.EX); }
        void suba_false_EX() { suba(false, addr_mode.EX); }
        void suba_true_IX() { suba(true, addr_mode.IX); }
        void suba_false_IX() { suba(false, addr_mode.IX); }
        void suba_true_IX1() { suba(true, addr_mode.IX1); }
        void suba_false_IX1() { suba(false, addr_mode.IX1); }
        void suba_true_IX2() { suba(true, addr_mode.IX2); }
        void suba_false_IX2() { suba(false, addr_mode.IX2); }

        // $a1 CMPA immediate               -***
        // $b1 CMPA direct                  -***
        // $c1 CMPA extended                -***
        // $d1 CMPA indexed, 2 byte offset  -***
        // $e1 CMPA indexed, 1 byte offset  -***
        // $f1 CMPA indexed                 -***
        //OP_HANDLER_MODE( cmpa )
        void cmpa(bool big, addr_mode M)
        {
            u16 t;
            ARGBYTE(big, M, out t);
            u16 r = (u16)(A - t);
            clr_nzc();
            set_nzc8(r);
        }

        void cmpa_true_IM() { cmpa(true, addr_mode.IM); }
        void cmpa_false_IM() { cmpa(false, addr_mode.IM); }
        void cmpa_true_DI() { cmpa(true, addr_mode.DI); }
        void cmpa_false_DI() { cmpa(false, addr_mode.DI); }
        void cmpa_true_EX() { cmpa(true, addr_mode.EX); }
        void cmpa_false_EX() { cmpa(false, addr_mode.EX); }
        void cmpa_true_IX() { cmpa(true, addr_mode.IX); }
        void cmpa_false_IX() { cmpa(false, addr_mode.IX); }
        void cmpa_true_IX1() { cmpa(true, addr_mode.IX1); }
        void cmpa_false_IX1() { cmpa(false, addr_mode.IX1); }
        void cmpa_true_IX2() { cmpa(true, addr_mode.IX2); }
        void cmpa_false_IX2() { cmpa(false, addr_mode.IX2); }

        // $a2 SBCA immediate               -***
        // $b2 SBCA direct                  -***
        // $c2 SBCA extended                -***
        // $d2 SBCA indexed, 2 byte offset  -***
        // $e2 SBCA indexed, 1 byte offset  -***
        // $f2 SBCA indexed                 -***
        //OP_HANDLER_MODE( sbca )
        void sbca(bool big, addr_mode M)
        {
            u16 t;
            ARGBYTE(big, M, out t);
            u16 r = (u16)(A - t - g.BIT(CC, 0));
            clr_nzc();
            set_nzc8(r);
            A = (u8)r;
        }

        void sbca_true_IM() { sbca(true, addr_mode.IM); }
        void sbca_false_IM() { sbca(false, addr_mode.IM); }
        void sbca_true_DI() { sbca(true, addr_mode.DI); }
        void sbca_false_DI() { sbca(false, addr_mode.DI); }
        void sbca_true_EX() { sbca(true, addr_mode.EX); }
        void sbca_false_EX() { sbca(false, addr_mode.EX); }
        void sbca_true_IX() { sbca(true, addr_mode.IX); }
        void sbca_false_IX() { sbca(false, addr_mode.IX); }
        void sbca_true_IX1() { sbca(true, addr_mode.IX1); }
        void sbca_false_IX1() { sbca(false, addr_mode.IX1); }
        void sbca_true_IX2() { sbca(true, addr_mode.IX2); }
        void sbca_false_IX2() { sbca(false, addr_mode.IX2); }

        // $a3 CPX immediate                -***
        // $b3 CPX direct                   -***
        // $c3 CPX extended                 -***
        // $d3 CPX indexed, 2 byte offset   -***
        // $e3 CPX indexed, 1 byte offset   -***
        // $f3 CPX indexed                  -***
        //OP_HANDLER_MODE( cpx )
        void cpx(bool big, addr_mode M)
        {
            u16 t;
            ARGBYTE(big, M, out t);
            u16 r = (u16)(X - t);
            clr_nzc();
            set_nzc8(r);
        }

        void cpx_true_IM() { cpx(true, addr_mode.IM); }
        void cpx_false_IM() { cpx(false, addr_mode.IM); }
        void cpx_true_DI() { cpx(true, addr_mode.DI); }
        void cpx_false_DI() { cpx(false, addr_mode.DI); }
        void cpx_true_EX() { cpx(true, addr_mode.EX); }
        void cpx_false_EX() { cpx(false, addr_mode.EX); }
        void cpx_true_IX() { cpx(true, addr_mode.IX); }
        void cpx_false_IX() { cpx(false, addr_mode.IX); }
        void cpx_true_IX1() { cpx(true, addr_mode.IX1); }
        void cpx_false_IX1() { cpx(false, addr_mode.IX1); }
        void cpx_true_IX2() { cpx(true, addr_mode.IX2); }
        void cpx_false_IX2() { cpx(false, addr_mode.IX2); }

        // $a4 ANDA immediate               -**-
        // $b4 ANDA direct                  -**-
        // $c4 ANDA extended                -**-
        // $d4 ANDA indexed, 2 byte offset  -**-
        // $e4 ANDA indexed, 1 byte offset  -**-
        // $f4 ANDA indexed                 -**-
        //OP_HANDLER_MODE( anda )
        void anda(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            A &= t;
            clr_nz();
            set_nz8(A);
        }

        void anda_true_IM() { anda(true, addr_mode.IM); }
        void anda_false_IM() { anda(false, addr_mode.IM); }
        void anda_true_DI() { anda(true, addr_mode.DI); }
        void anda_false_DI() { anda(false, addr_mode.DI); }
        void anda_true_EX() { anda(true, addr_mode.EX); }
        void anda_false_EX() { anda(false, addr_mode.EX); }
        void anda_true_IX() { anda(true, addr_mode.IX); }
        void anda_false_IX() { anda(false, addr_mode.IX); }
        void anda_true_IX1() { anda(true, addr_mode.IX1); }
        void anda_false_IX1() { anda(false, addr_mode.IX1); }
        void anda_true_IX2() { anda(true, addr_mode.IX2); }
        void anda_false_IX2() { anda(false, addr_mode.IX2); }

        // $a5 BITA immediate               -**-
        // $b5 BITA direct                  -**-
        // $c5 BITA extended                -**-
        // $d5 BITA indexed, 2 byte offset  -**-
        // $e5 BITA indexed, 1 byte offset  -**-
        // $f5 BITA indexed                 -**-
        //OP_HANDLER_MODE( bita )
        void bita(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            u8 r = (u8)(A & t);
            clr_nz();
            set_nz8(r);
        }

        void bita_true_IM() { bita(true, addr_mode.IM); }
        void bita_false_IM() { bita(false, addr_mode.IM); }
        void bita_true_DI() { bita(true, addr_mode.DI); }
        void bita_false_DI() { bita(false, addr_mode.DI); }
        void bita_true_EX() { bita(true, addr_mode.EX); }
        void bita_false_EX() { bita(false, addr_mode.EX); }
        void bita_true_IX() { bita(true, addr_mode.IX); }
        void bita_false_IX() { bita(false, addr_mode.IX); }
        void bita_true_IX1() { bita(true, addr_mode.IX1); }
        void bita_false_IX1() { bita(false, addr_mode.IX1); }
        void bita_true_IX2() { bita(true, addr_mode.IX2); }
        void bita_false_IX2() { bita(false, addr_mode.IX2); }

        // $a6 LDA immediate                -**-
        // $b6 LDA direct                   -**-
        // $c6 LDA extended                 -**-
        // $d6 LDA indexed, 2 byte offset   -**-
        // $e6 LDA indexed, 1 byte offset   -**-
        // $f6 LDA indexed                  -**-
        //OP_HANDLER_MODE( lda )
        void lda(bool big, addr_mode M)
        {
            u8 temp;
            ARGBYTE(big, M, out temp); A = temp;
            clr_nz();
            set_nz8(A);
        }

        void lda_true_IM() { lda(true, addr_mode.IM); }
        void lda_false_IM() { lda(false, addr_mode.IM); }
        void lda_true_DI() { lda(true, addr_mode.DI); }
        void lda_false_DI() { lda(false, addr_mode.DI); }
        void lda_true_EX() { lda(true, addr_mode.EX); }
        void lda_false_EX() { lda(false, addr_mode.EX); }
        void lda_true_IX() { lda(true, addr_mode.IX); }
        void lda_false_IX() { lda(false, addr_mode.IX); }
        void lda_true_IX1() { lda(true, addr_mode.IX1); }
        void lda_false_IX1() { lda(false, addr_mode.IX1); }
        void lda_true_IX2() { lda(true, addr_mode.IX2); }
        void lda_false_IX2() { lda(false, addr_mode.IX2); }

        // $a7 ILLEGAL
        // $b7 STA direct                   -**-
        // $c7 STA extended                 -**-
        // $d7 STA indexed, 2 byte offset   -**-
        // $e7 STA indexed, 1 byte offset   -**-
        // $f7 STA indexed                  -**-
        //OP_HANDLER_MODE( sta )
        void sta(bool big, addr_mode M)
        {
            clr_nz();
            set_nz8(A);
            ARGADDR(big, M);
            wm(big, EAD, A);
        }

        void sta_true_IM() { sta(true, addr_mode.IM); }
        void sta_false_IM() { sta(false, addr_mode.IM); }
        void sta_true_DI() { sta(true, addr_mode.DI); }
        void sta_false_DI() { sta(false, addr_mode.DI); }
        void sta_true_EX() { sta(true, addr_mode.EX); }
        void sta_false_EX() { sta(false, addr_mode.EX); }
        void sta_true_IX() { sta(true, addr_mode.IX); }
        void sta_false_IX() { sta(false, addr_mode.IX); }
        void sta_true_IX1() { sta(true, addr_mode.IX1); }
        void sta_false_IX1() { sta(false, addr_mode.IX1); }
        void sta_true_IX2() { sta(true, addr_mode.IX2); }
        void sta_false_IX2() { sta(false, addr_mode.IX2); }

        // $a8 EORA immediate               -**-
        // $b8 EORA direct                  -**-
        // $c8 EORA extended                -**-
        // $d8 EORA indexed, 2 byte offset  -**-
        // $e8 EORA indexed, 1 byte offset  -**-
        // $f8 EORA indexed                 -**-
        //OP_HANDLER_MODE( eora )
        void eora(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            A ^= t;
            clr_nz();
            set_nz8(A);
        }

        void eora_true_IM() { eora(true, addr_mode.IM); }
        void eora_false_IM() { eora(false, addr_mode.IM); }
        void eora_true_DI() { eora(true, addr_mode.DI); }
        void eora_false_DI() { eora(false, addr_mode.DI); }
        void eora_true_EX() { eora(true, addr_mode.EX); }
        void eora_false_EX() { eora(false, addr_mode.EX); }
        void eora_true_IX() { eora(true, addr_mode.IX); }
        void eora_false_IX() { eora(false, addr_mode.IX); }
        void eora_true_IX1() { eora(true, addr_mode.IX1); }
        void eora_false_IX1() { eora(false, addr_mode.IX1); }
        void eora_true_IX2() { eora(true, addr_mode.IX2); }
        void eora_false_IX2() { eora(false, addr_mode.IX2); }

        // $a9 ADCA immediate               ****
        // $b9 ADCA direct                  ****
        // $c9 ADCA extended                ****
        // $d9 ADCA indexed, 2 byte offset  ****
        // $e9 ADCA indexed, 1 byte offset  ****
        // $f9 ADCA indexed                 ****
        //OP_HANDLER_MODE( adca )
        void adca(bool big, addr_mode M)
        {
            u16 t;
            ARGBYTE(big, M, out t);
            u16 r = (u16)(A + t + g.BIT(CC, 0));
            clr_hnzc();
            set_hnzc8(A, (u8)t, r);
            A = (u8)r;
        }

        void adca_true_IM() { adca(true, addr_mode.IM); }
        void adca_false_IM() { adca(false, addr_mode.IM); }
        void adca_true_DI() { adca(true, addr_mode.DI); }
        void adca_false_DI() { adca(false, addr_mode.DI); }
        void adca_true_EX() { adca(true, addr_mode.EX); }
        void adca_false_EX() { adca(false, addr_mode.EX); }
        void adca_true_IX() { adca(true, addr_mode.IX); }
        void adca_false_IX() { adca(false, addr_mode.IX); }
        void adca_true_IX1() { adca(true, addr_mode.IX1); }
        void adca_false_IX1() { adca(false, addr_mode.IX1); }
        void adca_true_IX2() { adca(true, addr_mode.IX2); }
        void adca_false_IX2() { adca(false, addr_mode.IX2); }

        // $aa ORA immediate                -**-
        // $ba ORA direct                   -**-
        // $ca ORA extended                 -**-
        // $da ORA indexed, 2 byte offset   -**-
        // $ea ORA indexed, 1 byte offset   -**-
        // $fa ORA indexed                  -**-
        //OP_HANDLER_MODE( ora )
        void ora(bool big, addr_mode M)
        {
            u8 t;
            ARGBYTE(big, M, out t);
            A |= t;
            clr_nz();
            set_nz8(A);
        }

        void ora_true_IM() { ora(true, addr_mode.IM); }
        void ora_false_IM() { ora(false, addr_mode.IM); }
        void ora_true_DI() { ora(true, addr_mode.DI); }
        void ora_false_DI() { ora(false, addr_mode.DI); }
        void ora_true_EX() { ora(true, addr_mode.EX); }
        void ora_false_EX() { ora(false, addr_mode.EX); }
        void ora_true_IX() { ora(true, addr_mode.IX); }
        void ora_false_IX() { ora(false, addr_mode.IX); }
        void ora_true_IX1() { ora(true, addr_mode.IX1); }
        void ora_false_IX1() { ora(false, addr_mode.IX1); }
        void ora_true_IX2() { ora(true, addr_mode.IX2); }
        void ora_false_IX2() { ora(false, addr_mode.IX2); }

        // $ab ADDA immediate               ****
        // $bb ADDA direct                  ****
        // $cb ADDA extended                ****
        // $db ADDA indexed, 2 byte offset  ****
        // $eb ADDA indexed, 1 byte offset  ****
        // $fb ADDA indexed                 ****
        //OP_HANDLER_MODE( adda )
        void adda(bool big, addr_mode M)
        {
            u16 t;
            ARGBYTE(big, M, out t);
            u16 r = (u16)(A + t);
            clr_hnzc();
            set_hnzc8(A, (u8)t, r);
            A = (u8)r;
        }

        void adda_true_IM() { adda(true, addr_mode.IM); }
        void adda_false_IM() { adda(false, addr_mode.IM); }
        void adda_true_DI() { adda(true, addr_mode.DI); }
        void adda_false_DI() { adda(false, addr_mode.DI); }
        void adda_true_EX() { adda(true, addr_mode.EX); }
        void adda_false_EX() { adda(false, addr_mode.EX); }
        void adda_true_IX() { adda(true, addr_mode.IX); }
        void adda_false_IX() { adda(false, addr_mode.IX); }
        void adda_true_IX1() { adda(true, addr_mode.IX1); }
        void adda_false_IX1() { adda(false, addr_mode.IX1); }
        void adda_true_IX2() { adda(true, addr_mode.IX2); }
        void adda_false_IX2() { adda(false, addr_mode.IX2); }

        // $ac ILLEGAL
        // $bc JMP direct                   -***
        // $cc JMP extended                 -***
        // $dc JMP indexed, 2 byte offset   -***
        // $ec JMP indexed, 1 byte offset   -***
        // $fc JMP indexed                  -***
        //OP_HANDLER_MODE( jmp )
        void jmp(bool big, addr_mode M)
        {
            ARGADDR(big, M);
            PC = EA;
        }

        void jmp_true_IM() { jmp(true, addr_mode.IM); }
        void jmp_false_IM() { jmp(false, addr_mode.IM); }
        void jmp_true_DI() { jmp(true, addr_mode.DI); }
        void jmp_false_DI() { jmp(false, addr_mode.DI); }
        void jmp_true_EX() { jmp(true, addr_mode.EX); }
        void jmp_false_EX() { jmp(false, addr_mode.EX); }
        void jmp_true_IX() { jmp(true, addr_mode.IX); }
        void jmp_false_IX() { jmp(false, addr_mode.IX); }
        void jmp_true_IX1() { jmp(true, addr_mode.IX1); }
        void jmp_false_IX1() { jmp(false, addr_mode.IX1); }
        void jmp_true_IX2() { jmp(true, addr_mode.IX2); }
        void jmp_false_IX2() { jmp(false, addr_mode.IX2); }

        // $ad BSR ----
        //OP_HANDLER( bsr )
        void bsr(bool big)
        {
            u8 t;
            immbyte(big, out t);
            pushword(big, m_pc);
            PC = (u16)(PC + SIGNED(t));
        }

        void bsr_true() { bsr(true); }
        void bsr_false() { bsr(false); }

        // $bd JSR direct                   ----
        // $cd JSR extended                 ----
        // $dd JSR indexed, 2 byte offset   ----
        // $ed JSR indexed, 1 byte offset   ----
        // $fd JSR indexed                  ----
        //OP_HANDLER_MODE( jsr )
        void jsr(bool big, addr_mode M)
        {
            ARGADDR(big, M);
            pushword(big, m_pc);
            PC = EA;
        }

        void jsr_true_IM() { jsr(true, addr_mode.IM); }
        void jsr_false_IM() { jsr(false, addr_mode.IM); }
        void jsr_true_DI() { jsr(true, addr_mode.DI); }
        void jsr_false_DI() { jsr(false, addr_mode.DI); }
        void jsr_true_EX() { jsr(true, addr_mode.EX); }
        void jsr_false_EX() { jsr(false, addr_mode.EX); }
        void jsr_true_IX() { jsr(true, addr_mode.IX); }
        void jsr_false_IX() { jsr(false, addr_mode.IX); }
        void jsr_true_IX1() { jsr(true, addr_mode.IX1); }
        void jsr_false_IX1() { jsr(false, addr_mode.IX1); }
        void jsr_true_IX2() { jsr(true, addr_mode.IX2); }
        void jsr_false_IX2() { jsr(false, addr_mode.IX2); }

        // $ae LDX immediate                -**-
        // $be LDX direct                   -**-
        // $ce LDX extended                 -**-
        // $de LDX indexed, 2 byte offset   -**-
        // $ee LDX indexed, 1 byte offset   -**-
        // $fe LDX indexed                  -**-
        //OP_HANDLER_MODE( ldx )
        void ldx(bool big, addr_mode M)
        {
            u8 temp;
            ARGBYTE(big, M, out temp); X = temp;
            clr_nz();
            set_nz8(X);
        }

        void ldx_true_IM() { ldx(true, addr_mode.IM); }
        void ldx_false_IM() { ldx(false, addr_mode.IM); }
        void ldx_true_DI() { ldx(true, addr_mode.DI); }
        void ldx_false_DI() { ldx(false, addr_mode.DI); }
        void ldx_true_EX() { ldx(true, addr_mode.EX); }
        void ldx_false_EX() { ldx(false, addr_mode.EX); }
        void ldx_true_IX() { ldx(true, addr_mode.IX); }
        void ldx_false_IX() { ldx(false, addr_mode.IX); }
        void ldx_true_IX1() { ldx(true, addr_mode.IX1); }
        void ldx_false_IX1() { ldx(false, addr_mode.IX1); }
        void ldx_true_IX2() { ldx(true, addr_mode.IX2); }
        void ldx_false_IX2() { ldx(false, addr_mode.IX2); }

        // $af ILLEGAL
        // $bf STX direct                   -**-
        // $cf STX extended                 -**-
        // $df STX indexed, 2 byte offset   -**-
        // $ef STX indexed, 1 byte offset   -**-
        // $ff STX indexed                  -**-
        //OP_HANDLER_MODE( stx )
        void stx(bool big, addr_mode M)
        {
            clr_nz();
            set_nz8(X);
            ARGADDR(big, M);
            wm(big, EAD, X);
        }

        void stx_true_IM() { stx(true, addr_mode.IM); }
        void stx_false_IM() { stx(false, addr_mode.IM); }
        void stx_true_DI() { stx(true, addr_mode.DI); }
        void stx_false_DI() { stx(false, addr_mode.DI); }
        void stx_true_EX() { stx(true, addr_mode.EX); }
        void stx_false_EX() { stx(false, addr_mode.EX); }
        void stx_true_IX() { stx(true, addr_mode.IX); }
        void stx_false_IX() { stx(false, addr_mode.IX); }
        void stx_true_IX1() { stx(true, addr_mode.IX1); }
        void stx_false_IX1() { stx(false, addr_mode.IX1); }
        void stx_true_IX2() { stx(true, addr_mode.IX2); }
        void stx_false_IX2() { stx(false, addr_mode.IX2); }
    }
}
