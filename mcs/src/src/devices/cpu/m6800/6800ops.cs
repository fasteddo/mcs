// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    public partial class m6800_cpu_device : cpu_device
    {
        //define OP_HANDLER(_name) void m6800_cpu_device::_name ()

        //OP_HANDLER( illegl1 )
        protected void illegl1()
        {
            logerror("m6800: illegal 1-byte opcode: address {0}, op {1}\n", PC - 1, (int)M_RDOP_ARG((uint32_t)PC - 1) & 0xFF);
        }

        //OP_HANDLER( illegl2 )
        protected void illegl2()
        {
            logerror("m6800: illegal 2-byte opcode: address {0}, op {1}\n", PC - 1, (int)M_RDOP_ARG((uint32_t)PC - 1) & 0xFF);
            PC++;
        }

        //OP_HANDLER( illegl3 )
        void illegl3()
        {
            logerror("m6800: illegal 3-byte opcode: address {0}, op {1}\n", PC - 1, (int)M_RDOP_ARG((uint32_t)PC - 1) & 0xFF);
            PC += 2;
        }

        /* HD63701 only */
        //OP_HANDLER( trap )
        void trap()
        {
            logerror("m6800: illegal opcode: address {0}, op {1}\n", PC - 1, (int)M_RDOP_ARG((uint32_t)PC - 1) & 0xFF);
            take_trap();
        }

        /* $00 ILLEGAL */

        /* $01 NOP */
        protected void nop()
        {
        }

        /* $02 ILLEGAL */

        /* $03 ILLEGAL */

        /* $04 LSRD inherent -0*-* */
        //OP_HANDLER( lsrd )
        protected void lsrd()
        {
            uint16_t t;
            CLR_NZVC(); t = D; CC |= (uint8_t)(t & 0x0001);
            t >>= 1; SET_Z16(t);
            if (NXORC() != 0) SEV();
            D = t;
        }

        /* $05 ASLD inherent ?**** */
        //OP_HANDLER( asld )
        protected void asld()
        {
            int r;
            uint16_t t;
            t = D; r = t << 1;
            CLR_NZVC(); SET_FLAGS16(t, t, (uint32_t)r);
            D = (uint16_t)r;
        }

        /* $06 TAP inherent ##### */
        //OP_HANDLER( tap )
        protected void tap()
        {
            CC = A;
            ONE_MORE_INSN();
            check_irq_lines();
        }

        /* $07 TPA inherent ----- */
        //OP_HANDLER( tpa )
        protected void tpa()
        {
            A = CC;
        }

        /* $08 INX inherent --*-- */
        //OP_HANDLER( inx )
        protected void inx()
        {
            ++X;
            CLR_Z(); SET_Z16(X);
        }

        /* $09 DEX inherent --*-- */
        //OP_HANDLER( dex )
        protected void dex()
        {
            --X;
            CLR_Z(); SET_Z16(X);
        }

        /* $0a CLV */
        //OP_HANDLER( clv )
        protected void clv()
        {
            CLV();
        }

        /* $0b SEV */
        //OP_HANDLER( sev )
        protected void sev()
        {
            SEV();
        }

        /* $0c CLC */
        //OP_HANDLER( clc )
        protected void clc()
        {
            CLC();
        }

        /* $0d SEC */
        //OP_HANDLER( sec )
        protected void sec()
        {
            SEC();
        }

        /* $0e CLI */
        //OP_HANDLER( cli )
        protected void cli()
        {
            CLI();
            ONE_MORE_INSN();
            check_irq_lines();
        }

        /* $0f SEI */
        //OP_HANDLER( sei )
        protected void sei()
        {
            SEI();
            ONE_MORE_INSN();
            check_irq_lines();
        }

        /* $10 SBA inherent -**** */
        //OP_HANDLER( sba )
        protected void sba()
        {
            uint16_t t;
            t = (uint16_t)(A - B);
            CLR_NZVC(); SET_FLAGS8(A, B, t);
            A = (uint8_t)t;
        }

        /* $11 CBA inherent -**** */
        //OP_HANDLER( cba )
        protected void cba()
        {
            uint16_t t;
            t = (uint16_t)(A - B);
            CLR_NZVC(); SET_FLAGS8(A, B, t);
        }

        /* $12 ILLEGAL */
        //OP_HANDLER( undoc1 )
        protected void undoc1()
        {
            X += (uint16_t)RM((uint32_t)(S + 1));
        }

        /* $13 ILLEGAL */
        //OP_HANDLER( undoc2 )
        protected void undoc2()
        {
            X += (uint16_t)RM((uint32_t)(S + 1));
        }


        /* $14 ILLEGAL */

        /* $15 ILLEGAL */

        /* $16 TAB inherent -**0- */
        //OP_HANDLER( tab )
        protected void tab()
        {
            B = A;
            CLR_NZV(); SET_NZ8(B);
        }

        /* $17 TBA inherent -**0- */
        //OP_HANDLER( tba )
        protected void tba()
        {
            A = B;
            CLR_NZV(); SET_NZ8(A);
        }

        /* $18 XGDX inherent ----- */ /* HD63701YO only */
        //OP_HANDLER( xgdx )
        protected void xgdx()
        {
            uint16_t t = X;
            X = D;
            D = t;
        }

        /* $19 DAA inherent (A) -**0* */
        //OP_HANDLER( daa )
        protected void daa()
        {
            uint8_t msn;
            uint8_t lsn;
            uint16_t t;
            uint16_t cf = 0;
            msn = (uint8_t)(A & 0xf0); lsn = (uint8_t)(A & 0x0f);
            if (lsn > 0x09 || (CC & 0x20) != 0 ) cf |= 0x06;
            if (msn > 0x80 && lsn > 0x09) cf |= 0x60;
            if (msn > 0x90 || (CC & 0x01) != 0 ) cf |= 0x60;
            t = (uint16_t)(cf + A);
            CLR_NZV(); /* keep carry from previous operation */
            SET_NZ8((uint8_t)t); SET_C8(t);
            A = (uint8_t)t;
        }

        /* $1a ILLEGAL */

        /* $1a SLP */ /* HD63701YO only */
        //OP_HANDLER( slp )
        protected void slp()
        {
            /* wait for next IRQ (same as waiting of wai) */
            m_wai_state |= M6800_SLP;
            eat_cycles();
        }

        /* $1b ABA inherent ***** */
        //OP_HANDLER( aba )
        protected void aba()
        {
            uint16_t t;
            t = (uint16_t)(A + B);
            CLR_HNZVC(); SET_FLAGS8(A, B, t); SET_H(A, B, t);
            A = (uint8_t)t;
        }

        /* $1c ILLEGAL */

        /* $1d ILLEGAL */

        /* $1e ILLEGAL */

        /* $1f ILLEGAL */

        /* $20 BRA relative ----- */
        //OP_HANDLER( bra )
        protected void bra()
        {
            uint8_t t;
            BRANCH(out t, true);
        }

        /* $21 BRN relative ----- */
        //OP_HANDLER( brn )
        protected void brn()
        {
            uint8_t t;
            BRANCH(out t, false);
        }

        /* $22 BHI relative ----- */
        //OP_HANDLER( bhi )
        protected void bhi()
        {
            uint8_t t;
            BRANCH(out t, (CC & 0x05) == 0);
        }

        /* $23 BLS relative ----- */
        //OP_HANDLER( bls )
        protected void bls()
        {
            uint8_t t;
            BRANCH(out t, (CC & 0x05) != 0);
        }

        /* $24 BCC relative ----- */
        //OP_HANDLER( bcc )
        protected void bcc()
        {
            uint8_t t;
            BRANCH(out t, (CC & 0x01) == 0);
        }

        /* $25 BCS relative ----- */
        //OP_HANDLER( bcs )
        protected void bcs()
        {
            uint8_t t;
            BRANCH(out t, (CC & 0x01) == 0);
        }

        /* $26 BNE relative ----- */
        //OP_HANDLER( bne )
        protected void bne()
        {
            uint8_t t;
            BRANCH(out t, (CC & 0x04) == 0);
        }

        /* $27 BEQ relative ----- */
        //OP_HANDLER( beq )
        protected void beq()
        {
            uint8_t t;
            BRANCH(out t, (CC & 0x04) != 0);
        }

        /* $28 BVC relative ----- */
        //OP_HANDLER( bvc )
        protected void bvc()
        {
            uint8_t t;
            BRANCH(out t, (CC & 0x02) == 0);
        }

        /* $29 BVS relative ----- */
        //OP_HANDLER( bvs )
        protected void bvs()
        {
            uint8_t t;
            BRANCH(out t, (CC & 0x02) != 0);
        }

        /* $2a BPL relative ----- */
        //OP_HANDLER( bpl )
        protected void bpl()
        {
            uint8_t t;
            BRANCH(out t, (CC & 0x08) == 0);
        }

        /* $2b BMI relative ----- */
        //OP_HANDLER( bmi )
        protected void bmi()
        {
            uint8_t t;
            BRANCH(out t, (CC & 0x08) != 0);
        }

        /* $2c BGE relative ----- */
        //OP_HANDLER( bge )
        protected void bge()
        {
            uint8_t t;
            BRANCH(out t, NXORV() == 0);
        }

        /* $2d BLT relative ----- */
        //OP_HANDLER( blt )
        protected void blt()
        {
            uint8_t t;
            BRANCH(out t, NXORV() != 0);
        }

        /* $2e BGT relative ----- */
        //OP_HANDLER( bgt )
        protected void bgt()
        {
            uint8_t t;
            BRANCH(out t, !(NXORV() != 0 || (CC & 0x04) != 0));
        }

        /* $2f BLE relative ----- */
        //OP_HANDLER( ble )
        protected void ble()
        {
            uint8_t t;
            BRANCH(out t, NXORV() != 0 || (CC & 0x04) != 0);
        }

        /* $30 TSX inherent ----- */
        //OP_HANDLER( tsx )
        protected void tsx()
        {
            X = (uint16_t)(S + 1);
        }

        /* $31 INS inherent ----- */
        //OP_HANDLER( ins )
        protected void ins()
        {
            ++S;
        }

        /* $32 PULA inherent ----- */
        //OP_HANDLER( pula )
        protected void pula()
        {
            PULLBYTE(out m_d.b.h);
        }

        /* $33 PULB inherent ----- */
        //OP_HANDLER( pulb )
        protected void pulb()
        {
            PULLBYTE(out m_d.b.l);
        }

        /* $34 DES inherent ----- */
        //OP_HANDLER( des )
        protected void des()
        {
            --S;
        }

        /* $35 TXS inherent ----- */
        //OP_HANDLER( txs )
        protected void txs()
        {
            S = (uint16_t)(X - 1);
        }

        /* $36 PSHA inherent ----- */
        //OP_HANDLER( psha )
        protected void psha()
        {
            PUSHBYTE(m_d.b.h);
        }

        /* $37 PSHB inherent ----- */
        //OP_HANDLER( pshb )
        protected void pshb()
        {
            PUSHBYTE(m_d.b.l);
        }

        /* $38 PULX inherent ----- */
        //OP_HANDLER( pulx )
        protected void pulx()
        {
            PULLWORD_pX();
        }

        /* $39 RTS inherent ----- */
        //OP_HANDLER( rts )
        protected void rts()
        {
            PULLWORD_pPC();
        }

        /* $3a ABX inherent ----- */
        //OP_HANDLER( abx )
        protected void abx()
        {
            X += B;
        }

        /* $3b RTI inherent ##### */
        //OP_HANDLER( rti )
        protected void rti()
        {
            PULLBYTE_CC();
            PULLBYTE_B();
            PULLBYTE_A();
            PULLWORD_pX();
            PULLWORD_pPC();
            check_irq_lines();
        }

        /* $3c PSHX inherent ----- */
        //OP_HANDLER( pshx )
        protected void pshx()
        {
            PUSHWORD(pX);
        }

        /* $3d MUL inherent --*-@ */
        //OP_HANDLER( mul )
        protected void mul()
        {
            uint16_t t;
            t = (uint16_t)(A * B);
            CLR_C();
            if ((t & 0x80) != 0) SEC();
            D = t;
        }

        /* $3e WAI inherent ----- */
        //OP_HANDLER( wai )
        protected void wai()
        {
            /*
             * WAI stacks the entire machine state on the
             * hardware stack, then waits for an interrupt.
             */
            m_wai_state |= M6800_WAI;
            PUSHWORD(pPC);
            PUSHWORD(pX);
            PUSHBYTE(A);
            PUSHBYTE(B);
            PUSHBYTE(CC);
            check_irq_lines();
            if ((m_wai_state & M6800_WAI) != 0) eat_cycles();
        }

        /* $3f SWI absolute indirect ----- */
        //OP_HANDLER( swi )
        protected void swi()
        {
            PUSHWORD(pPC);
            PUSHWORD(pX);
            PUSHBYTE(A);
            PUSHBYTE(B);
            PUSHBYTE(CC);
            SEI();
            PCD = RM16(0xfffa);
        }

        /* $40 NEGA inherent ?**** */
        //OP_HANDLER( nega )
        protected void nega()
        {
            uint16_t r;
            r = (uint16_t)(-A);
            CLR_NZVC(); SET_FLAGS8(0, A, r);
            A = (uint8_t)r;
        }

        /* $41 ILLEGAL */

        /* $42 ILLEGAL */

        /* $43 COMA inherent -**01 */
        //OP_HANDLER( coma )
        protected void coma()
        {
            A = (uint8_t)(~A);
            CLR_NZV(); SET_NZ8(A); SEC();
        }

        /* $44 LSRA inherent -0*-* */
        //OP_HANDLER( lsra )
        protected void lsra()
        {
            CLR_NZVC(); CC |= (uint8_t)(A & 0x01);
            A >>= 1; SET_Z8(A);
            if (NXORC() != 0) SEV();
        }

        /* $45 ILLEGAL */

        /* $46 RORA inherent -**-* */
        //OP_HANDLER( rora )
        protected void rora()
        {
            uint8_t r;
            r = (uint8_t)((CC & 0x01) << 7);
            CLR_NZVC(); CC |= (uint8_t)(A & 0x01);
            r |= (uint8_t)(A >> 1); SET_NZ8(r);
            if (NXORC() != 0) SEV();
            A = r;
        }

        /* $47 ASRA inherent ?**-* */
        //OP_HANDLER( asra )
        protected void asra()
        {
            CLR_NZVC(); CC |= (uint8_t)(A & 0x01);
            A >>= 1; A |= (uint8_t)((A&0x40)<<1);
            SET_NZ8(A);
            if (NXORC() != 0) SEV();
        }

        /* $48 ASLA inherent ?**** */
        //OP_HANDLER( asla )
        protected void asla()
        {
            uint16_t r;
            r = (uint16_t)(A << 1);
            CLR_NZVC(); SET_FLAGS8(A, A, r);
            A = (uint8_t)r;
        }

        /* $49 ROLA inherent -**** */
        //OP_HANDLER( rola )
        protected void rola()
        {
            uint16_t t;
            uint16_t r;
            t = A; r = (uint16_t)(CC & 0x01); r |= (uint16_t)(t << 1);
            CLR_NZVC(); SET_FLAGS8(t, t, r);
            A = (uint8_t)r;
        }

        /* $4a DECA inherent -***- */
        //OP_HANDLER( deca )
        protected void deca()
        {
            --A;
            CLR_NZV(); SET_FLAGS8D(A);
        }

        /* $4b ILLEGAL */

        /* $4c INCA inherent -***- */
        //OP_HANDLER( inca )
        protected void inca()
        {
            ++A;
            CLR_NZV(); SET_FLAGS8I(A);
        }

        /* $4d TSTA inherent -**0- */
        //OP_HANDLER( tsta )
        protected void tsta()
        {
            CLR_NZVC(); SET_NZ8(A);
        }

        /* $4e ILLEGAL */

        /* $4f CLRA inherent -0100 */
        //OP_HANDLER( clra )
        protected void clra()
        {
            A = 0;
            CLR_NZVC(); SEZ();
        }

        /* $50 NEGB inherent ?**** */
        //OP_HANDLER( negb )
        protected void negb()
        {
            uint16_t r;
            r = (uint16_t)(-B);
            CLR_NZVC(); SET_FLAGS8(0, B, r);
            B = (uint8_t)r;
        }

        /* $51 ILLEGAL */

        /* $52 ILLEGAL */

        /* $53 COMB inherent -**01 */
        //OP_HANDLER( comb )
        protected void comb()
        {
            B = (uint8_t)(~B);
            CLR_NZV(); SET_NZ8(B); SEC();
        }

        /* $54 LSRB inherent -0*-* */
        //OP_HANDLER( lsrb )
        protected void lsrb()
        {
            CLR_NZVC(); CC |= (uint8_t)(B & 0x01);
            B >>= 1; SET_Z8(B);
            if (NXORC() != 0) SEV();
        }

        /* $55 ILLEGAL */

        /* $56 RORB inherent -**-* */
        //OP_HANDLER( rorb )
        protected void rorb()
        {
            uint8_t r;
            r = (uint8_t)((CC & 0x01) << 7);
            CLR_NZVC(); CC |= (uint8_t)(B & 0x01);
            r |= (uint8_t)(B >> 1); SET_NZ8(r);
            if (NXORC() != 0) SEV();
            B = r;
        }

        /* $57 ASRB inherent ?**-* */
        //OP_HANDLER( asrb )
        protected void asrb()
        {
            CLR_NZVC(); CC |= (uint8_t)(B & 0x01);
            B >>= 1; B |= (uint8_t)((B & 0x40) << 1);
            SET_NZ8(B);
            if (NXORC() != 0) SEV();
        }

        /* $58 ASLB inherent ?**** */
        //OP_HANDLER( aslb )
        protected void aslb()
        {
            uint16_t r;
            r = (uint16_t)(B << 1);
            CLR_NZVC(); SET_FLAGS8(B, B, r);
            B = (uint8_t)r;
        }

        /* $59 ROLB inherent -**** */
        //OP_HANDLER( rolb )
        protected void rolb()
        {
            uint16_t t;
            uint16_t r;
            t = B; r = (uint16_t)(CC & 0x01); r |= (uint16_t)(t << 1);
            CLR_NZVC(); SET_FLAGS8(t, t, r);
            B = (uint8_t)r;
        }

        /* $5a DECB inherent -***- */
        //OP_HANDLER( decb )
        protected void decb()
        {
            --B;
            CLR_NZV(); SET_FLAGS8D(B);
        }

        /* $5b ILLEGAL */

        /* $5c INCB inherent -***- */
        //OP_HANDLER( incb )
        protected void incb()
        {
            ++B;
            CLR_NZV(); SET_FLAGS8I(B);
        }

        /* $5d TSTB inherent -**0- */
        //OP_HANDLER( tstb )
        protected void tstb()
        {
            CLR_NZVC(); SET_NZ8(B);
        }

        /* $5e ILLEGAL */

        /* $5f CLRB inherent -0100 */
        //OP_HANDLER( clrb )
        protected void clrb()
        {
            B=0;
            CLR_NZVC(); SEZ();
        }

        /* $60 NEG indexed ?**** */
        //OP_HANDLER( neg_ix )
        protected void neg_ix()
        {
            uint16_t r;
            uint16_t t;
            IDXBYTE(out t); r = (uint16_t)(-t);
            CLR_NZVC(); SET_FLAGS8(0, t, r);
            WM(EAD, (uint8_t)r);
        }

        /* $61 AIM --**0- */ /* HD63701YO only */
        //OP_HANDLER( aim_ix )
        protected void aim_ix()
        {
            uint8_t t;
            uint8_t r;
            IMMBYTE(out t);
            IDXBYTE(out r);
            r &= t;
            CLR_NZV(); SET_NZ8(r);
            WM(EAD, r);
        }

        /* $62 OIM --**0- */ /* HD63701YO only */
        //OP_HANDLER( oim_ix )
        protected void oim_ix()
        {
            uint8_t t;
            uint8_t r;
            IMMBYTE(out t);
            IDXBYTE(out r);
            r |= t;
            CLR_NZV(); SET_NZ8(r);
            WM(EAD, r);
        }

        /* $63 COM indexed -**01 */
        //OP_HANDLER( com_ix )
        protected void com_ix()
        {
            uint8_t t;
            IDXBYTE(out t); t = (uint8_t)(~t);
            CLR_NZV(); SET_NZ8(t); SEC();
            WM(EAD, t);
        }

        /* $64 LSR indexed -0*-* */
        //OP_HANDLER( lsr_ix )
        protected void lsr_ix()
        {
            uint8_t t;
            IDXBYTE(out t); CLR_NZVC(); CC |= (uint8_t)(t & 0x01);
            t >>= 1; SET_Z8(t);
            if (NXORC() != 0) SEV();
            WM(EAD, t);
        }

        /* $65 EIM --**0- */ /* HD63701YO only */
        //OP_HANDLER( eim_ix )
        protected void eim_ix()
        {
            uint8_t t;
            uint8_t r;
            IMMBYTE(out t);
            IDXBYTE(out r);
            r ^= t;
            CLR_NZV(); SET_NZ8(r);
            WM(EAD, r);
        }

        /* $66 ROR indexed -**-* */
        //OP_HANDLER( ror_ix )
        protected void ror_ix()
        {
            uint8_t t;
            uint8_t r;
            IDXBYTE(out t); r = (uint8_t)((CC & 0x01) << 7);
            CLR_NZVC(); CC |= (uint8_t)(t & 0x01);
            r |= (uint8_t)(t >> 1); SET_NZ8(r);
            if (NXORC() != 0) SEV();
            WM(EAD, r);
        }

        /* $67 ASR indexed ?**-* */
        //OP_HANDLER( asr_ix )
        protected void asr_ix()
        {
            uint8_t t;
            IDXBYTE(out t); CLR_NZVC(); CC |= (uint8_t)(t & 0x01);
            t >>= 1; t |= (uint8_t)((t & 0x40) << 1);
            SET_NZ8(t);
            if (NXORC() != 0) SEV();
            WM(EAD, t);
        }

        /* $68 ASL indexed ?**** */
        //OP_HANDLER( asl_ix )
        protected void asl_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t); r = (uint16_t)(t << 1);
            CLR_NZVC(); SET_FLAGS8(t, t, r);
            WM(EAD, (uint8_t)r);
        }

        /* $69 ROL indexed -**** */
        //OP_HANDLER( rol_ix )
        protected void rol_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t); r = (uint16_t)(CC & 0x01); r |= (uint16_t)(t << 1);
            CLR_NZVC(); SET_FLAGS8(t, t, r);
            WM(EAD, (uint8_t)r);
        }

        /* $6a DEC indexed -***- */
        //OP_HANDLER( dec_ix )
        protected void dec_ix()
        {
            uint8_t t;
            IDXBYTE(out t); --t;
            CLR_NZV(); SET_FLAGS8D(t);
            WM(EAD, t);
        }

        /* $6b TIM --**0- */ /* HD63701YO only */
        //OP_HANDLER( tim_ix )
        protected void tim_ix()
        {
            uint8_t t;
            uint8_t r;
            IMMBYTE(out t);
            IDXBYTE(out r);
            r &= t;
            CLR_NZV(); SET_NZ8(r);
        }

        /* $6c INC indexed -***- */
        //OP_HANDLER( inc_ix )
        protected void inc_ix()
        {
            uint8_t t;
            IDXBYTE(out t); ++t;
            CLR_NZV(); SET_FLAGS8I(t);
            WM(EAD, t);
        }

        /* $6d TST indexed -**0- */
        //OP_HANDLER( tst_ix )
        protected void tst_ix()
        {
            uint8_t t;
            IDXBYTE(out t); CLR_NZVC(); SET_NZ8(t);
        }

        /* $6e JMP indexed ----- */
        //OP_HANDLER( jmp_ix )
        protected void jmp_ix()
        {
            INDEXED(); PC = EA;
        }

        /* $6f CLR indexed -0100 */
        //OP_HANDLER( clr_ix )
        protected void clr_ix()
        {
            INDEXED(); WM(EAD, 0);
            CLR_NZVC(); SEZ();
        }

        /* $70 NEG extended ?**** */
        //OP_HANDLER( neg_ex )
        protected void neg_ex()
        {
            uint16_t r;
            uint16_t t;
            EXTBYTE(out t); r = (uint16_t)(-t);
            CLR_NZVC(); SET_FLAGS8(0, t, r);
            WM(EAD, (uint8_t)r);
        }

        /* $71 AIM --**0- */ /* HD63701YO only */
        //OP_HANDLER( aim_di )
        protected void aim_di()
        {
            uint8_t t, r;
            IMMBYTE(out t);
            DIRBYTE(out r);
            r &= t;
            CLR_NZV(); SET_NZ8(r);
            WM(EAD, r);
        }

        /* $72 OIM --**0- */ /* HD63701YO only */
        //OP_HANDLER( oim_di )
        protected void oim_di()
        {
            uint8_t t, r;
            IMMBYTE(out t);
            DIRBYTE(out r);
            r |= t;
            CLR_NZV(); SET_NZ8(r);
            WM(EAD, r);
        }

        /* $73 COM extended -**01 */
        //OP_HANDLER( com_ex )
        protected void com_ex()
        {
            uint8_t t;
            EXTBYTE(out t); t = (uint8_t)(~t);
            CLR_NZV(); SET_NZ8(t); SEC();
            WM(EAD, t);
        }

        /* $74 LSR extended -0*-* */
        //OP_HANDLER( lsr_ex )
        protected void lsr_ex()
        {
            uint8_t t;
            EXTBYTE(out t);
            CLR_NZVC();
            CC |= (uint8_t)(t & 0x01);
            t >>= 1;
            SET_Z8(t);
            if (NXORC() != 0) SEV();
            WM(EAD, t);
        }

        /* $75 EIM --**0- */ /* HD63701YO only */
        //OP_HANDLER( eim_di )
        protected void eim_di()
        {
            uint8_t t;
            uint8_t r;
            IMMBYTE(out t);
            DIRBYTE(out r);
            r ^= t;
            CLR_NZV(); SET_NZ8(r);
            WM(EAD, r);
        }

        /* $76 ROR extended -**-* */
        //OP_HANDLER( ror_ex )
        protected void ror_ex()
        {
            uint8_t t;
            uint8_t r;
            EXTBYTE(out t); r = (uint8_t)((CC & 0x01) << 7);
            CLR_NZVC(); CC |= (uint8_t)(t & 0x01);
            r |= (uint8_t)(t >> 1); SET_NZ8(r);
            if (NXORC() != 0) SEV();
            WM(EAD, r);
        }

        /* $77 ASR extended ?**-* */
        //OP_HANDLER( asr_ex )
        protected void asr_ex()
        {
            uint8_t t;
            EXTBYTE(out t); CLR_NZVC(); CC |= (uint8_t)(t & 0x01);
            t >>= 1; t |= (uint8_t)((t & 0x40) << 1);
            SET_NZ8(t);
            if (NXORC() != 0) SEV();
            WM(EAD, t);
        }

        /* $78 ASL extended ?**** */
        //OP_HANDLER( asl_ex )
        protected void asl_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t); r = (uint16_t)(t << 1);
            CLR_NZVC(); SET_FLAGS8(t, t, r);
            WM(EAD, (uint8_t)r);
        }

        /* $79 ROL extended -**** */
        //OP_HANDLER( rol_ex )
        protected void rol_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t); r = (uint16_t)(CC & 0x01); r |= (uint16_t)(t << 1);
            CLR_NZVC(); SET_FLAGS8(t, t, r);
            WM(EAD, (uint8_t)r);
        }

        /* $7a DEC extended -***- */
        //OP_HANDLER( dec_ex )
        protected void dec_ex()
        {
            uint8_t t;
            EXTBYTE(out t); --t;
            CLR_NZV(); SET_FLAGS8D(t);
            WM(EAD, t);
        }

        /* $7b TIM --**0- */ /* HD63701YO only */
        //OP_HANDLER( tim_di )
        protected void tim_di()
        {
            uint8_t t;
            uint8_t r;
            IMMBYTE(out t);
            DIRBYTE(out r);
            r &= t;
            CLR_NZV(); SET_NZ8(r);
        }

        /* $7c INC extended -***- */
        //OP_HANDLER( inc_ex )
        protected void inc_ex()
        {
            uint8_t t;
            EXTBYTE(out t); ++t;
            CLR_NZV(); SET_FLAGS8I(t);
            WM(EAD, t);
        }

        /* $7d TST extended -**0- */
        //OP_HANDLER( tst_ex )
        protected void tst_ex()
        {
            uint8_t t;
            EXTBYTE(out t); CLR_NZVC(); SET_NZ8(t);
        }

        /* $7e JMP extended ----- */
        //OP_HANDLER( jmp_ex )
        protected void jmp_ex()
        {
            EXTENDED(); PC = EA;
        }

        /* $7f CLR extended -0100 */
        //OP_HANDLER( clr_ex )
        protected void clr_ex()
        {
            EXTENDED(); WM(EAD, 0);
            CLR_NZVC(); SEZ();
        }

        /* $80 SUBA immediate ?**** */
        //OP_HANDLER( suba_im )
        protected void suba_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t); r = (uint16_t)(A - t);
            CLR_NZVC(); SET_FLAGS8(A, t, r);
            A = (uint8_t)r;
        }

        /* $81 CMPA immediate ?**** */
        //OP_HANDLER( cmpa_im )
        protected void cmpa_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t); r = (uint16_t)(A - t);
            CLR_NZVC(); SET_FLAGS8(A, t, r);
        }

        /* $82 SBCA immediate ?**** */
        //OP_HANDLER( sbca_im )
        protected void sbca_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t); r = (uint16_t)(A - t - (CC & 0x01));
            CLR_NZVC(); SET_FLAGS8(A, t, r);
            A = (uint8_t)r;
        }

        /* $83 SUBD immediate -**** */
        //OP_HANDLER( subd_im )
        protected void subd_im()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            IMMWORD(out b);
            d = D;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
            D = (uint16_t)r;
        }

        /* $84 ANDA immediate -**0- */
        //OP_HANDLER( anda_im )
        protected void anda_im()
        {
            uint8_t t;
            IMMBYTE(out t); A &= t;
            CLR_NZV(); SET_NZ8(A);
        }

        /* $85 BITA immediate -**0- */
        //OP_HANDLER( bita_im )
        protected void bita_im()
        {
            uint8_t t;
            uint8_t r;
            IMMBYTE(out t); r = (uint8_t)(A & t);
            CLR_NZV(); SET_NZ8(r);
        }

        /* $86 LDA immediate -**0- */
        //OP_HANDLER( lda_im )
        protected void lda_im()
        {
            IMMBYTE_A();
            CLR_NZV(); SET_NZ8(A);
        }

        /* is this a legal instruction? */
        /* $87 STA immediate -**0- */
        //OP_HANDLER( sta_im )
        protected void sta_im()
        {
            CLR_NZV(); SET_NZ8(A);
            IMM8(); WM(EAD, A);
        }

        /* $88 EORA immediate -**0- */
        //OP_HANDLER( eora_im )
        protected void eora_im()
        {
            uint8_t t;
            IMMBYTE(out t); A ^= t;
            CLR_NZV(); SET_NZ8(A);
        }

        /* $89 ADCA immediate ***** */
        //OP_HANDLER( adca_im )
        protected void adca_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t); r = (uint16_t)(A + t + (CC & 0x01));
            CLR_HNZVC(); SET_FLAGS8(A, t, r); SET_H(A, t, r);
            A = (uint8_t)r;
        }

        /* $8a ORA immediate -**0- */
        //OP_HANDLER( ora_im )
        protected void ora_im()
        {
            uint8_t t;
            IMMBYTE(out t); A |= t;
            CLR_NZV(); SET_NZ8(A);
        }

        /* $8b ADDA immediate ***** */
        //OP_HANDLER( adda_im )
        protected void adda_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t); r = (uint16_t)(A + t);
            CLR_HNZVC(); SET_FLAGS8(A, t, r); SET_H(A, t, r);
            A = (uint8_t)r;
        }

        /* $8c CMPX immediate -***- */
        //OP_HANDLER( cmpx_im )
        protected void cmpx_im()
        {
            PAIR r = new PAIR();
            PAIR d = new PAIR();
            PAIR b;
            IMMWORD(out b);
            d.d = X;
            r.w.l = (uint16_t)(d.b.h - b.b.h);
            CLR_NZV();
            SET_N8(r.b.l);
            SET_V8(d.b.h, b.b.h, r.w.l);
            r.d = d.d - b.d;
            SET_Z16(r.d);
        }

        /* $8c CPX immediate -**** (6803) */
        //OP_HANDLER( cpx_im )
        protected void cpx_im()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            IMMWORD(out b);
            d = X;
            r = d - b.d;
            CLR_NZVC(); SET_FLAGS16(d, b.d, r);
        }


        /* $8d BSR ----- */
        //OP_HANDLER( bsr )
        protected void bsr()
        {
            uint8_t t;
            IMMBYTE(out t);
            PUSHWORD(pPC);
            PC = (uint16_t)(PC + SIGNED(t));
        }

        /* $8e LDS immediate -**0- */
        //OP_HANDLER( lds_im )
        protected void lds_im()
        {
            IMMWORD(out m_s);
            CLR_NZV();
            SET_NZ16(S);
        }

        /* $8f STS immediate -**0- */
        //OP_HANDLER( sts_im )
        protected void sts_im()
        {
            CLR_NZV();
            SET_NZ16(S);
            IMM16();
            WM16(EAD, m_s);
        }

        /* $90 SUBA direct ?**** */
        //OP_HANDLER( suba_di )
        protected void suba_di()
        {
            uint16_t t;
            uint16_t r;
            DIRBYTE(out t); r = (uint16_t)(A - t);
            CLR_NZVC(); SET_FLAGS8(A, t, r);
            A = (uint8_t)r;
        }

        /* $91 CMPA direct ?**** */
        //OP_HANDLER( cmpa_di )
        protected void cmpa_di()
        {
            uint16_t t;
            uint16_t r;
            DIRBYTE(out t); r = (uint16_t)(A - t);
            CLR_NZVC(); SET_FLAGS8(A, t, r);
        }

        /* $92 SBCA direct ?**** */
        //OP_HANDLER( sbca_di )
        protected void sbca_di()
        {
            uint16_t t;
            uint16_t r;
            DIRBYTE(out t); r = (uint16_t)(A - t - (CC & 0x01));
            CLR_NZVC(); SET_FLAGS8(A, t, r);
            A = (uint8_t)r;
        }

        /* $93 SUBD direct -**** */
        //OP_HANDLER( subd_di )
        protected void subd_di()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            DIRWORD(out b);
            d = D;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
            D = (uint16_t)r;
        }

        /* $94 ANDA direct -**0- */
        //OP_HANDLER( anda_di )
        protected void anda_di()
        {
            uint8_t t;
            DIRBYTE(out t); A &= t;
            CLR_NZV(); SET_NZ8(A);
        }

        /* $95 BITA direct -**0- */
        //OP_HANDLER( bita_di )
        protected void bita_di()
        {
            uint8_t t;
            uint8_t r;
            DIRBYTE(out t); r = (uint8_t)(A & t);
            CLR_NZV(); SET_NZ8(r);
        }

        /* $96 LDA direct -**0- */
        //OP_HANDLER( lda_di )
        protected void lda_di()
        {
            DIRBYTE_A();
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $97 STA direct -**0- */
        //OP_HANDLER( sta_di )
        protected void sta_di()
        {
            CLR_NZV();
            SET_NZ8(A);
            DIRECT();
            WM(EAD,A);
        }

        /* $98 EORA direct -**0- */
        //OP_HANDLER( eora_di )
        protected void eora_di()
        {
            uint8_t t;
            DIRBYTE(out t);
            A ^= t;
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $99 ADCA direct ***** */
        //OP_HANDLER( adca_di )
        protected void adca_di()
        {
            uint16_t t;
            uint16_t r;
            DIRBYTE(out t);
            r = (uint16_t)(A + t + (CC & 0x01));
            CLR_HNZVC();
            SET_FLAGS8(A, t, r);
            SET_H(A, t, r);
            A = (uint8_t)r;
        }

        /* $9a ORA direct -**0- */
        //OP_HANDLER( ora_di )
        protected void ora_di()
        {
            uint8_t t;
            DIRBYTE(out t);
            A |= t;
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $9b ADDA direct ***** */
        //OP_HANDLER( adda_di )
        protected void adda_di()
        {
            uint16_t t;
            uint16_t r;
            DIRBYTE(out t);
            r = (uint16_t)(A + t);
            CLR_HNZVC();
            SET_FLAGS8(A, t, r);
            SET_H(A, t, r);
            A = (uint8_t)r;
        }

        /* $9c CMPX direct -***- */
        //OP_HANDLER( cmpx_di )
        protected void cmpx_di()
        {
            PAIR r = new PAIR();
            PAIR d = new PAIR();
            PAIR b;
            DIRWORD(out b);
            d.d = X;
            r.w.l = (uint16_t)(d.b.h - b.b.h);
            CLR_NZV();
            SET_N8(r.b.l);
            SET_V8(d.b.h, b.b.h, r.w.l);
            r.d = d.d - b.d;
            SET_Z16(r.d);
        }

        /* $9c CPX direct -**** (6803) */
        //OP_HANDLER( cpx_di )
        protected void cpx_di()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            DIRWORD(out b);
            d = X;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
        }

        /* $9d JSR direct ----- */
        //OP_HANDLER( jsr_di )
        protected void jsr_di()
        {
            DIRECT();
            PUSHWORD(pPC);
            PC = EA;
        }

        /* $9e LDS direct -**0- */
        //OP_HANDLER( lds_di )
        protected void lds_di()
        {
            DIRWORD(out m_s);
            CLR_NZV();
            SET_NZ16(S);
        }

        /* $9f STS direct -**0- */
        //OP_HANDLER( sts_di )
        protected void sts_di()
        {
            CLR_NZV();
            SET_NZ16(S);
            DIRECT();
            WM16(EAD, m_s);
        }

        /* $a0 SUBA indexed ?**** */
        //OP_HANDLER( suba_ix )
        protected void suba_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t);
            r = (uint16_t)(A - t);
            CLR_NZVC();
            SET_FLAGS8(A, t, r);
            A = (uint8_t)r;
        }

        /* $a1 CMPA indexed ?**** */
        //OP_HANDLER( cmpa_ix )
        protected void cmpa_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t);
            r = (uint16_t)(A - t);
            CLR_NZVC();
            SET_FLAGS8(A, t, r);
        }

        /* $a2 SBCA indexed ?**** */
        //OP_HANDLER( sbca_ix )
        protected void sbca_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t);
            r = (uint16_t)(A - t - (CC & 0x01));
            CLR_NZVC();
            SET_FLAGS8(A, t, r);
            A = (uint8_t)r;
        }

        /* $a3 SUBD indexed -**** */
        //OP_HANDLER( subd_ix )
        protected void subd_ix()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            IDXWORD(out b);
            d = D;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
            D = (uint16_t)r;
        }

        /* $a4 ANDA indexed -**0- */
        //OP_HANDLER( anda_ix )
        protected void anda_ix()
        {
            uint8_t t;
            IDXBYTE(out t); A &= t;
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $a5 BITA indexed -**0- */
        //OP_HANDLER( bita_ix )
        protected void bita_ix()
        {
            uint8_t t;
            uint8_t r;
            IDXBYTE(out t); r = (uint8_t)(A & t);
            CLR_NZV();
            SET_NZ8(r);
        }

        /* $a6 LDA indexed -**0- */
        //OP_HANDLER( lda_ix )
        protected void lda_ix()
        {
            IDXBYTE_A();
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $a7 STA indexed -**0- */
        //OP_HANDLER( sta_ix )
        protected void sta_ix()
        {
            CLR_NZV();
            SET_NZ8(A);
            INDEXED();
            WM(EAD,A);
        }

        /* $a8 EORA indexed -**0- */
        //OP_HANDLER( eora_ix )
        protected void eora_ix()
        {
            uint8_t t;
            IDXBYTE(out t);
            A ^= t;
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $a9 ADCA indexed ***** */
        //OP_HANDLER( adca_ix )
        protected void adca_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t);
            r = (uint16_t)(A + t + (CC & 0x01));
            CLR_HNZVC();
            SET_FLAGS8(A, t, r);
            SET_H(A, t, r);
            A = (uint8_t)r;
        }

        /* $aa ORA indexed -**0- */
        //OP_HANDLER( ora_ix )
        protected void ora_ix()
        {
            uint8_t t;
            IDXBYTE(out t);
            A |= t;
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $ab ADDA indexed ***** */
        //OP_HANDLER( adda_ix )
        protected void adda_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t);
            r = (uint16_t)(A + t);
            CLR_HNZVC();
            SET_FLAGS8(A, t, r);
            SET_H(A, t, r);
            A = (uint8_t)r;
        }

        /* $ac CMPX indexed -***- */
        //OP_HANDLER( cmpx_ix )
        protected void cmpx_ix()
        {
            PAIR r = new PAIR();
            PAIR d = new PAIR();
            PAIR b;
            IDXWORD(out b);
            d.d = X;
            r.w.l = (uint16_t)(d.b.h - b.b.h);
            CLR_NZV();
            SET_N8(r.b.l);
            SET_V8(d.b.h, b.b.h, r.w.l);
            r.d = d.d - b.d;
            SET_Z16(r.d);
        }

        /* $ac CPX indexed -**** (6803)*/
        //OP_HANDLER( cpx_ix )
        protected void cpx_ix()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            IDXWORD(out b);
            d = X;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
        }

        /* $ad JSR indexed ----- */
        //OP_HANDLER( jsr_ix )
        protected void jsr_ix()
        {
            INDEXED();
            PUSHWORD(pPC);
            PC = EA;
        }

        /* $ae LDS indexed -**0- */
        //OP_HANDLER( lds_ix )
        protected void lds_ix()
        {
            IDXWORD(out m_s);
            CLR_NZV();
            SET_NZ16(S);
        }

        /* $af STS indexed -**0- */
        //OP_HANDLER( sts_ix )
        protected void sts_ix()
        {
            CLR_NZV();
            SET_NZ16(S);
            INDEXED();
            WM16(EAD, m_s);
        }

        /* $b0 SUBA extended ?**** */
        //OP_HANDLER( suba_ex )
        protected void suba_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t);
            r = (uint16_t)(A - t);
            CLR_NZVC();
            SET_FLAGS8(A, t, r);
            A = (uint8_t)r;
        }

        /* $b1 CMPA extended ?**** */
        //OP_HANDLER( cmpa_ex )
        protected void cmpa_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t);
            r = (uint16_t)(A - t);
            CLR_NZVC();
            SET_FLAGS8(A, t, r);
        }

        /* $b2 SBCA extended ?**** */
        //OP_HANDLER( sbca_ex )
        protected void sbca_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t);
            r = (uint16_t)(A - t - (CC & 0x01));
            CLR_NZVC();
            SET_FLAGS8(A, t, r);
            A = (uint8_t)r;
        }

        /* $b3 SUBD extended -**** */
        //OP_HANDLER( subd_ex )
        protected void subd_ex()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            EXTWORD(out b);
            d = D;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
            D = (uint16_t)r;
        }

        /* $b4 ANDA extended -**0- */
        //OP_HANDLER( anda_ex )
        protected void anda_ex()
        {
            uint8_t t;
            EXTBYTE(out t);
            A &= t;
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $b5 BITA extended -**0- */
        //OP_HANDLER( bita_ex )
        protected void bita_ex()
        {
            uint8_t t;
            uint8_t r;
            EXTBYTE(out t);
            r = (uint8_t)(A & t);
            CLR_NZV();
            SET_NZ8(r);
        }

        /* $b6 LDA extended -**0- */
        //OP_HANDLER( lda_ex )
        protected void lda_ex()
        {
            EXTBYTE_A();
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $b7 STA extended -**0- */
        //OP_HANDLER( sta_ex )
        protected void sta_ex()
        {
            CLR_NZV();
            SET_NZ8(A);
            EXTENDED();
            WM(EAD,A);
        }

        /* $b8 EORA extended -**0- */
        //OP_HANDLER( eora_ex )
        protected void eora_ex()
        {
            uint8_t t;
            EXTBYTE(out t);
            A ^= t;
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $b9 ADCA extended ***** */
        //OP_HANDLER( adca_ex )
        protected void adca_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t);
            r = (uint16_t)(A + t + (CC & 0x01));
            CLR_HNZVC();
            SET_FLAGS8(A, t, r);
            SET_H(A, t, r);
            A = (uint8_t)r;
        }

        /* $ba ORA extended -**0- */
        //OP_HANDLER( ora_ex )
        protected void ora_ex()
        {
            uint8_t t;
            EXTBYTE(out t);
            A |= t;
            CLR_NZV();
            SET_NZ8(A);
        }

        /* $bb ADDA extended ***** */
        //OP_HANDLER( adda_ex )
        protected void adda_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t);
            r = (uint16_t)(A + t);
            CLR_HNZVC();
            SET_FLAGS8(A, t, r);
            SET_H(A, t, r);
            A = (uint8_t)r;
        }

        /* $bc CMPX extended -***- */
        //OP_HANDLER( cmpx_ex )
        protected void cmpx_ex()
        {
            PAIR r = new PAIR();
            PAIR d = new PAIR();
            PAIR b;
            EXTWORD(out b);
            d.d = X;
            r.w.l = (uint16_t)(d.b.h - b.b.h);
            CLR_NZV();
            SET_N8(r.b.l);
            SET_V8(d.b.h, b.b.h, r.w.l);
            r.d = d.d - b.d;
            SET_Z16(r.d);
        }

        /* $bc CPX extended -**** (6803) */
        //OP_HANDLER( cpx_ex )
        protected void cpx_ex()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            EXTWORD(out b);
            d = X;
            r = d - b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
        }

        /* $bd JSR extended ----- */
        //OP_HANDLER( jsr_ex )
        protected void jsr_ex()
        {
            EXTENDED();
            PUSHWORD(pPC);
            PC = EA;
        }

        /* $be LDS extended -**0- */
        //OP_HANDLER( lds_ex )
        protected void lds_ex()
        {
            EXTWORD(out m_s);
            CLR_NZV();
            SET_NZ16(S);
        }

        /* $bf STS extended -**0- */
        //OP_HANDLER( sts_ex )
        protected void sts_ex()
        {
            CLR_NZV();
            SET_NZ16(S);
            EXTENDED();
            WM16(EAD, m_s);
        }

        /* $c0 SUBB immediate ?**** */
        //OP_HANDLER( subb_im )
        protected void subb_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t);
            r = (uint16_t)(B - t);
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
            B = (uint8_t)r;
        }

        /* $c1 CMPB immediate ?**** */
        //OP_HANDLER( cmpb_im )
        protected void cmpb_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t);
            r = (uint16_t)(B - t);
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
        }

        /* $c2 SBCB immediate ?**** */
        //OP_HANDLER( sbcb_im )
        protected void sbcb_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t);
            r = (uint16_t)(B - t - (CC & 0x01));
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
            B = (uint8_t)r;
        }

        /* $c3 ADDD immediate -**** */
        //OP_HANDLER( addd_im )
        protected void addd_im()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            IMMWORD(out b);
            d = D;
            r = d + b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
            D = (uint16_t)r;
        }

        /* $c4 ANDB immediate -**0- */
        //OP_HANDLER( andb_im )
        protected void andb_im()
        {
            uint8_t t;
            IMMBYTE(out t);
            B &= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $c5 BITB immediate -**0- */
        //OP_HANDLER( bitb_im )
        protected void bitb_im()
        {
            uint8_t t;
            uint8_t r;
            IMMBYTE(out t);
            r = (uint8_t)(B & t);
            CLR_NZV();
            SET_NZ8(r);
        }

        /* $c6 LDB immediate -**0- */
        //OP_HANDLER( ldb_im )
        protected void ldb_im()
        {
            IMMBYTE_B();
            CLR_NZV();
            SET_NZ8(B);
        }

        /* is this a legal instruction? */
        /* $c7 STB immediate -**0- */
        //OP_HANDLER( stb_im )
        protected void stb_im()
        {
            CLR_NZV();
            SET_NZ8(B);
            IMM8();
            WM(EAD,B);
        }

        /* $c8 EORB immediate -**0- */
        //OP_HANDLER( eorb_im )
        protected void eorb_im()
        {
            uint8_t t;
            IMMBYTE(out t);
            B ^= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $c9 ADCB immediate ***** */
        //OP_HANDLER( adcb_im )
        protected void adcb_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t);
            r = (uint16_t)(B + t + (CC & 0x01));
            CLR_HNZVC();
            SET_FLAGS8(B, t, r);
            SET_H(B, t, r);
            B = (uint8_t)r;
        }

        /* $ca ORB immediate -**0- */
        //OP_HANDLER( orb_im )
        protected void orb_im()
        {
            uint8_t t;
            IMMBYTE(out t);
            B |= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $cb ADDB immediate ***** */
        //OP_HANDLER( addb_im )
        protected void addb_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t);
            r = (uint16_t)(B + t);
            CLR_HNZVC();
            SET_FLAGS8(B, t, r);
            SET_H(B, t, r);
            B = (uint8_t)r;
        }

        /* $CC LDD immediate -**0- */
        //OP_HANDLER( ldd_im )
        protected void ldd_im()
        {
            IMMWORD(out m_d);
            CLR_NZV();
            SET_NZ16(D);
        }

        /* is this a legal instruction? */
        /* $cd STD immediate -**0- */
        //OP_HANDLER( std_im )
        protected void std_im()
        {
            IMM16();
            CLR_NZV();
            SET_NZ16(D);
            WM16(EAD, m_d);
        }

        /* $ce LDX immediate -**0- */
        //OP_HANDLER( ldx_im )
        protected void ldx_im()
        {
            IMMWORD(out m_x);
            CLR_NZV();
            SET_NZ16(X);
        }

        /* $cf STX immediate -**0- */
        //OP_HANDLER( stx_im )
        protected void stx_im()
        {
            CLR_NZV();
            SET_NZ16(X);
            IMM16();
            WM16(EAD, m_x);
        }

        /* $d0 SUBB direct ?**** */
        //OP_HANDLER( subb_di )
        protected void subb_di()
        {
            uint16_t t;
            uint16_t r;
            DIRBYTE(out t);
            r = (uint16_t)(B - t);
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
            B = (uint8_t)r;
        }

        /* $d1 CMPB direct ?**** */
        //OP_HANDLER( cmpb_di )
        protected void cmpb_di()
        {
            uint16_t t;
            uint16_t r;
            DIRBYTE(out t);
            r = (uint16_t)(B - t);
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
        }

        /* $d2 SBCB direct ?**** */
        //OP_HANDLER( sbcb_di )
        protected void sbcb_di()
        {
            uint16_t t;
            uint16_t r;
            DIRBYTE(out t);
            r = (uint16_t)(B - t - (CC & 0x01));
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
            B = (uint8_t)r;
        }

        /* $d3 ADDD direct -**** */
        //OP_HANDLER( addd_di )
        protected void addd_di()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            DIRWORD(out b);
            d = D;
            r = d + b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
            D = (uint16_t)r;
        }

        /* $d4 ANDB direct -**0- */
        //OP_HANDLER( andb_di )
        protected void andb_di()
        {
            uint8_t t;
            DIRBYTE(out t);
            B &= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $d5 BITB direct -**0- */
        //OP_HANDLER( bitb_di )
        protected void bitb_di()
        {
            uint8_t t;
            uint8_t r;
            DIRBYTE(out t);
            r = (uint8_t)(B & t);
            CLR_NZV();
            SET_NZ8(r);
        }

        /* $d6 LDB direct -**0- */
        //OP_HANDLER( ldb_di )
        protected void ldb_di()
        {
            DIRBYTE_B();
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $d7 STB direct -**0- */
        //OP_HANDLER( stb_di )
        protected void stb_di()
        {
            CLR_NZV();
            SET_NZ8(B);
            DIRECT();
            WM(EAD,B);
        }

        /* $d8 EORB direct -**0- */
        //OP_HANDLER( eorb_di )
        protected void eorb_di()
        {
            uint8_t t;
            DIRBYTE(out t);
            B ^= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $d9 ADCB direct ***** */
        //OP_HANDLER( adcb_di )
        protected void adcb_di()
        {
            uint16_t t;
            uint16_t r;
            DIRBYTE(out t);
            r = (uint16_t)(B + t + (CC & 0x01));
            CLR_HNZVC();
            SET_FLAGS8(B, t, r);
            SET_H(B, t, r);
            B = (uint8_t)r;
        }

        /* $da ORB direct -**0- */
        //OP_HANDLER( orb_di )
        protected void orb_di()
        {
            uint8_t t;
            DIRBYTE(out t);
            B |= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $db ADDB direct ***** */
        //OP_HANDLER( addb_di )
        protected void addb_di()
        {
            uint16_t t;
            uint16_t r;
            DIRBYTE(out t);
            r = (uint16_t)(B + t);
            CLR_HNZVC();
            SET_FLAGS8(B, t, r);
            SET_H(B, t, r);
            B = (uint8_t)r;
        }

        /* $dc LDD direct -**0- */
        //OP_HANDLER( ldd_di )
        protected void ldd_di()
        {
            DIRWORD(out m_d);
            CLR_NZV();
            SET_NZ16(D);
        }

        /* $dd STD direct -**0- */
        //OP_HANDLER( std_di )
        protected void std_di()
        {
            DIRECT();
            CLR_NZV();
            SET_NZ16(D);
            WM16(EAD, m_d);
        }

        /* $de LDX direct -**0- */
        //OP_HANDLER( ldx_di )
        protected void ldx_di()
        {
            DIRWORD(out m_x);
            CLR_NZV();
            SET_NZ16(X);
        }

        /* $dF STX direct -**0- */
        //OP_HANDLER( stx_di )
        protected void stx_di()
        {
            CLR_NZV();
            SET_NZ16(X);
            DIRECT();
            WM16(EAD, m_x);
        }

        /* $e0 SUBB indexed ?**** */
        //OP_HANDLER( subb_ix )
        protected void subb_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t);
            r = (uint16_t)(B - t);
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
            B = (uint8_t)r;
        }

        /* $e1 CMPB indexed ?**** */
        //OP_HANDLER( cmpb_ix )
        protected void cmpb_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t);
            r = (uint16_t)(B - t);
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
        }

        /* $e2 SBCB indexed ?**** */
        //OP_HANDLER( sbcb_ix )
        protected void sbcb_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t);
            r = (uint16_t)(B - t - (CC & 0x01));
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
            B = (uint8_t)r;
        }

        /* $e3 ADDD indexed -**** */
        //OP_HANDLER( addd_ix )
        protected void addd_ix()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            IDXWORD(out b);
            d = D;
            r = d + b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
            D = (uint16_t)r;
        }

        /* $e4 ANDB indexed -**0- */
        //OP_HANDLER( andb_ix )
        protected void andb_ix()
        {
            uint8_t t;
            IDXBYTE(out t);
            B &= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $e5 BITB indexed -**0- */
        //OP_HANDLER( bitb_ix )
        protected void bitb_ix()
        {
            uint8_t t;
            uint8_t r;
            IDXBYTE(out t);
            r = (uint8_t)(B & t);
            CLR_NZV();
            SET_NZ8(r);
        }

        /* $e6 LDB indexed -**0- */
        //OP_HANDLER( ldb_ix )
        protected void ldb_ix()
        {
            IDXBYTE_B();
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $e7 STB indexed -**0- */
        //OP_HANDLER( stb_ix )
        protected void stb_ix()
        {
            CLR_NZV();
            SET_NZ8(B);
            INDEXED();
            WM(EAD,B);
        }

        /* $e8 EORB indexed -**0- */
        //OP_HANDLER( eorb_ix )
        protected void eorb_ix()
        {
            uint8_t t;
            IDXBYTE(out t);
            B ^= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $e9 ADCB indexed ***** */
        //OP_HANDLER( adcb_ix )
        protected void adcb_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t);
            r = (uint16_t)(B + t + (CC & 0x01));
            CLR_HNZVC();
            SET_FLAGS8(B, t, r);
            SET_H(B, t, r);
            B = (uint8_t)r;
        }

        /* $ea ORB indexed -**0- */
        //OP_HANDLER( orb_ix )
        protected void orb_ix()
        {
            uint8_t t;
            IDXBYTE(out t);
            B |= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $eb ADDB indexed ***** */
        //OP_HANDLER( addb_ix )
        protected void addb_ix()
        {
            uint16_t t;
            uint16_t r;
            IDXBYTE(out t);
            r = (uint16_t)(B + t);
            CLR_HNZVC();
            SET_FLAGS8(B, t, r);
            SET_H(B, t, r);
            B = (uint8_t)r;
        }

        /* $ec LDD indexed -**0- */
        //OP_HANDLER( ldd_ix )
        protected void ldd_ix()
        {
            IDXWORD(out m_d);
            CLR_NZV();
            SET_NZ16(D);
        }

        /* $ec ADCX immediate -****    NSC8105 only.  Flags are a guess - copied from addb_im() */
        // actually this is ADDX, causes garbage in nightgal.cpp otherwise
        //OP_HANDLER( adcx_im )
        protected void adcx_im()
        {
            uint16_t t;
            uint16_t r;
            IMMBYTE(out t);
            r = (uint16_t)(X + t);
            CLR_HNZVC();
            SET_FLAGS8(X, t, r);
            SET_H(X, t, r);
            X = r;
        }

        /* $ed STD indexed -**0- */
        //OP_HANDLER( std_ix )
        protected void std_ix()
        {
            INDEXED();
            CLR_NZV();
            SET_NZ16(D);
            WM16(EAD, m_d);
        }

        /* $ee LDX indexed -**0- */
        //OP_HANDLER( ldx_ix )
        protected void ldx_ix()
        {
            IDXWORD(out m_x);
            CLR_NZV();
            SET_NZ16(X);
        }

        /* $ef STX indexed -**0- */
        //OP_HANDLER( stx_ix )
        protected void stx_ix()
        {
            CLR_NZV();
            SET_NZ16(X);
            INDEXED();
            WM16(EAD, m_x);
        }

        /* $f0 SUBB extended ?**** */
        //OP_HANDLER( subb_ex )
        protected void subb_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t);
            r = (uint16_t)(B - t);
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
            B = (uint8_t)r;
        }

        /* $f1 CMPB extended ?**** */
        //OP_HANDLER( cmpb_ex )
        protected void cmpb_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t);
            r = (uint16_t)(B - t);
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
        }

        /* $f2 SBCB extended ?**** */
        //OP_HANDLER( sbcb_ex )
        protected void sbcb_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t);
            r = (uint16_t)(B - t - (CC & 0x01));
            CLR_NZVC();
            SET_FLAGS8(B, t, r);
            B = (uint8_t)r;
        }

        /* $f3 ADDD extended -**** */
        //OP_HANDLER( addd_ex )
        protected void addd_ex()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            EXTWORD(out b);
            d = D;
            r = d + b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
            D = (uint16_t)r;
        }

        /* $f4 ANDB extended -**0- */
        //OP_HANDLER( andb_ex )
        protected void andb_ex()
        {
            uint8_t t;
            EXTBYTE(out t);
            B &= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $f5 BITB extended -**0- */
        //OP_HANDLER( bitb_ex )
        protected void bitb_ex()
        {
            uint8_t t;
            uint8_t r;
            EXTBYTE(out t);
            r = (uint8_t)(B & t);
            CLR_NZV();
            SET_NZ8(r);
        }

        /* $f6 LDB extended -**0- */
        //OP_HANDLER( ldb_ex )
        protected void ldb_ex()
        {
            EXTBYTE_B();
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $f7 STB extended -**0- */
        //OP_HANDLER( stb_ex )
        protected void stb_ex()
        {
            CLR_NZV();
            SET_NZ8(B);
            EXTENDED();
            WM(EAD,B);
        }

        /* $f8 EORB extended -**0- */
        //OP_HANDLER( eorb_ex )
        protected void eorb_ex()
        {
            uint8_t t;
            EXTBYTE(out t);
            B ^= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $f9 ADCB extended ***** */
        //OP_HANDLER( adcb_ex )
        protected void adcb_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t);
            r = (uint16_t)(B + t + (CC & 0x01));
            CLR_HNZVC();
            SET_FLAGS8(B, t, r);
            SET_H(B, t, r);
            B = (uint8_t)r;
        }

        /* $fa ORB extended -**0- */
        //OP_HANDLER( orb_ex )
        protected void orb_ex()
        {
            uint8_t t;
            EXTBYTE(out t);
            B |= t;
            CLR_NZV();
            SET_NZ8(B);
        }

        /* $fb ADDB extended ***** */
        //OP_HANDLER( addb_ex )
        protected void addb_ex()
        {
            uint16_t t;
            uint16_t r;
            EXTBYTE(out t);
            r = (uint16_t)(B + t);
            CLR_HNZVC();
            SET_FLAGS8(B, t, r);
            SET_H(B, t, r);
            B = (uint8_t)r;
        }

        /* $fc LDD extended -**0- */
        //OP_HANDLER( ldd_ex )
        protected void ldd_ex()
        {
            EXTWORD(out m_d);
            CLR_NZV();
            SET_NZ16(D);
        }

        /* $fc ADDX extended -****    NSC8105 only.  Flags are a guess */
        //OP_HANDLER( addx_ex )
        protected void addx_ex()
        {
            uint32_t r;
            uint32_t d;
            PAIR b;
            EXTWORD(out b);
            d = X;
            r = d + b.d;
            CLR_NZVC();
            SET_FLAGS16(d, b.d, r);
            X = (uint16_t)r;
        }

        /* $fd STD extended -**0- */
        //OP_HANDLER( std_ex )
        protected void std_ex()
        {
            EXTENDED();
            CLR_NZV();
            SET_NZ16(D);
            WM16(EAD, m_d);
        }

        /* $fe LDX extended -**0- */
        //OP_HANDLER( ldx_ex )
        protected void ldx_ex()
        {
            EXTWORD(out m_x);
            CLR_NZV();
            SET_NZ16(X);
        }

        /* $ff STX extended -**0- */
        //OP_HANDLER( stx_ex )
        protected void stx_ex()
        {
            CLR_NZV();
            SET_NZ16(X);
            EXTENDED();
            WM16(EAD, m_x);
        }

        /* NSC8105 specific, guessed opcodes (tested by Night Gal Summer) */
        // $bb - $mask & [X + $disp8]
        //OP_HANDLER( btst_ix )
        protected void btst_ix()
        {
            uint8_t val;
            uint8_t mask = (uint8_t)M_RDOP_ARG(PCD);
            { EA = (uint16_t)(X + (M_RDOP_ARG(PCD + 1))); PC += 2; }
            val = (uint8_t)(RM(EAD) & mask);
            CLR_NZVC(); SET_NZ8(val);
        }

        // $b2 - assuming correct, store first byte to (X + $disp8)
        //OP_HANDLER( stx_nsc )
        protected void stx_nsc()
        {
            IMM8();
            uint8_t val = (uint8_t)RM(EAD);
            IMM8();
            EA = (uint16_t)(X + RM(EAD));
            CLR_NZV();
            SET_NZ8(val);
            WM(EAD,val);
        }
    }
}
