// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using int8_t = System.SByte;
using int16_t = System.Int16;
using int32_t = System.Int32;
using int64_t = System.Int64;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.diexec_global;
using static mame.osdcore_global;


namespace mame
{
    public partial class z8002_device : cpu_device,
                                        z8000_disassembler.config
    {
        /******************************************
         helper functions
         ******************************************/

        /******************************************
         check new fcw for switch to system mode
         and swap stack pointer if needed
         ******************************************/
        protected virtual void CHANGE_FCW(uint16_t fcw)
        {
            uint16_t tmp;
            if (((fcw ^ m_fcw) & F_S_N) != 0)            /* system/user mode change? */
            {
                tmp = RW(15).op;
                RW(15).op = m_nspoff;
                m_nspoff = tmp;
            }

            fcw &= unchecked((uint16_t)~F_SEG);  /* never set segmented mode bit on Z8002 */

            if ((m_fcw & F_NVIE) == 0 && (fcw & F_NVIE) != 0 && (m_irq_state[0] != CLEAR_LINE))
            {
                m_irq_req |= Z8000_NVI;
            }

            if ((m_fcw & F_VIE) == 0 && (fcw & F_VIE) != 0 && (m_irq_state[1] != CLEAR_LINE))
            {
                m_irq_req |= Z8000_VI;
            }

            m_fcw = fcw;  /* set new m_fcw */
        }


        //void z8001_device::CHANGE_FCW(uint16_t fcw)


        uint32_t make_segmented_addr(uint32_t addr)
        {
            return ((addr & 0x007f0000) << 8) | 0x80000000 | (addr & 0xffff);
        }

        uint32_t segmented_addr(uint32_t addr)
        {
            return ((addr & 0x7f000000) >> 8) | (addr & 0xffff);
        }

        uint32_t addr_from_reg(int regno)
        {
            if (get_segmented_mode())
                return segmented_addr(RL(regno).op);
            else
                return RW(regno).op;
        }

        void addr_to_reg(int regno, uint32_t addr)
        {
            if (get_segmented_mode())
            {
                uint32_t segaddr = make_segmented_addr(addr);
                RW(regno).op = (uint16_t)(((uint32_t)RW(regno).op & 0x80ff) | ((segaddr >> 16) & 0x7f00));
                RW(regno | 1).op = (uint16_t)(segaddr & 0xffff);
            }
            else
            {
                RW(regno).op = (uint16_t)addr;
            }
        }

        void add_to_addr_reg(int regno, uint16_t addend)
        {
            if (get_segmented_mode())
                regno |= 1;
            RW(regno).op += addend;
        }

        void sub_from_addr_reg(int regno, uint16_t subtrahend)
        {
            if (get_segmented_mode())
                regno |= 1;
            RW(regno).op -= subtrahend;
        }

        void set_pc(uint32_t addr)
        {
            if (get_segmented_mode())
                m_pc = addr;
            else
                m_pc = (m_pc & 0xffff0000) | (addr & 0xffff);
        }

        uint8_t RDIR_B(uint8_t reg)
        {
            return RDMEM_B(reg == SP ? m_stack : m_data, addr_from_reg(reg));
        }

        uint16_t RDIR_W(uint8_t reg)
        {
            return RDMEM_W(reg == SP ? m_stack : m_data, addr_from_reg(reg));
        }

        uint32_t RDIR_L(uint8_t reg)
        {
            return RDMEM_L(reg == SP ? m_stack : m_data, addr_from_reg(reg));
        }

        void WRIR_B(uint8_t reg, uint8_t value)
        {
            WRMEM_B(reg == SP ? m_stack : m_data, addr_from_reg(reg), value);
        }

        void WRIR_W(uint8_t reg, uint16_t value)
        {
            WRMEM_W(reg == SP ? m_stack : m_data, addr_from_reg(reg), value);
        }

        void WRIR_L(uint8_t reg, uint32_t value)
        {
            WRMEM_L(reg == SP ? m_stack : m_data, addr_from_reg(reg), value);
        }

        uint8_t RDBX_B(uint8_t reg, uint16_t idx)
        {
            return RDMEM_B(reg == SP ? m_stack : m_data, addr_add(addr_from_reg(reg), idx));
        }

        uint16_t RDBX_W(uint8_t reg, uint16_t idx)
        {
            return RDMEM_W(reg == SP ? m_stack : m_data, addr_add(addr_from_reg(reg), idx));
        }

        uint32_t RDBX_L(uint8_t reg, uint16_t idx)
        {
            return RDMEM_L(reg == SP ? m_stack : m_data, addr_add(addr_from_reg(reg), idx));
        }

        void WRBX_B(uint8_t reg, uint16_t idx, uint8_t value)
        {
            WRMEM_B(reg == SP ? m_stack : m_data, addr_add(addr_from_reg(reg), idx), value);
        }

        void WRBX_W(uint8_t reg, uint16_t idx, uint16_t value)
        {
            WRMEM_W(reg == SP ? m_stack : m_data, addr_add(addr_from_reg(reg), idx), value);
        }

        void WRBX_L(uint8_t reg, uint16_t idx, uint32_t value)
        {
            WRMEM_L(reg == SP ? m_stack : m_data, addr_add(addr_from_reg(reg), idx), value);
        }

        void PUSHW(uint8_t dst, uint16_t value)
        {
            if (get_segmented_mode())
                RW(dst | 1).op -= 2;
            else
                RW(dst).op -= 2;
            WRIR_W(dst, value);
        }

        uint16_t POPW(uint8_t src)
        {
            uint16_t result = RDIR_W(src);
            if (get_segmented_mode())
                RW(src | 1).op += 2;
            else
                RW(src).op += 2;
            return result;
        }

        void PUSHL(uint8_t dst, uint32_t value)
        {
            if (get_segmented_mode())
                RW(dst | 1).op -= 4;
            else
                RW(dst).op -= 4;
            WRIR_L(dst, value);
        }

        uint32_t POPL(uint8_t src)
        {
            uint32_t result = RDIR_L(src);
            if (get_segmented_mode())
                RW(src | 1).op += 4;
            else
                RW(src).op += 4;
            return result;
        }

        /* check zero and sign flag for byte, word and long results */
        void CHK_XXXB_ZS(int8_t result) { if (result == 0) SET_Z(); else if ((int8_t) result < 0) SET_S(); }
        void CHK_XXXB_ZS(uint8_t result) { if (result == 0) SET_Z(); else if ((int8_t) result < 0) SET_S(); }
        void CHK_XXXW_ZS(int16_t result) { if (result == 0) SET_Z(); else if ((int16_t)result < 0) SET_S(); }
        void CHK_XXXW_ZS(uint32_t result) { if (result == 0) SET_Z(); else if ((int16_t)result < 0) SET_S(); }
        void CHK_XXXL_ZS(int32_t result) { if (result == 0) SET_Z(); else if ((int32_t)result < 0) SET_S(); }
        void CHK_XXXL_ZS(uint64_t result) { if (result == 0) SET_Z(); else if ((int32_t)result < 0) SET_S(); }
        void CHK_XXXQ_ZS(uint64_t result) { if (result == 0) SET_Z(); else if ((int64_t)result < 0) SET_S(); }

        void CHK_XXXB_ZSP(uint8_t result) { m_fcw |= z8000_zsp[result]; }

        /* check carry for addition and subtraction */
        void CHK_ADDX_C(uint32_t dest, uint32_t result) { if (result < dest) SET_C(); }
        void CHK_ADCX_C(uint16_t dest, uint16_t value, uint16_t result) { if (result < dest || (result == dest && value != 0)) SET_C(); }

        void CHK_SUBX_C(uint32_t dest, uint32_t result) { if (result > dest) SET_C(); }
        void CHK_SBCX_C(uint16_t dest, uint16_t value, uint16_t result) { if (result > dest || (result == dest && value != 0)) SET_C(); }

        /* check half carry for A addition and S subtraction */
        void CHK_ADDB_H(uint8_t dest, uint8_t result) { if ((result & 15) < (dest & 15)) SET_H(); }
        void CHK_ADCB_H(uint8_t dest, uint8_t value, uint8_t result) { if ((result & 15) < (dest & 15) || ((result & 15) == (dest & 15) && (value & 15) != 0)) SET_H(); }

        void CHK_SUBB_H(uint8_t dest, uint8_t result) { if ((result & 15) > (dest & 15)) SET_H(); }
        void CHK_SBCB_H(uint8_t dest, uint8_t value, uint8_t result) { if ((result & 15) > (dest & 15) || ((result & 15) == (dest & 15) && (value & 15) != 0)) SET_H(); }

        /* check overflow for addition for byte, word and long */
        void CHK_ADDB_V(uint8_t dest, uint8_t value, uint8_t result) { if ((((value & dest & ~result) | (~value & ~dest & result)) & S08) != 0) SET_V(); }
        void CHK_ADDW_V(uint16_t dest, uint16_t value, uint16_t result) { if ((((value & dest & ~result) | (~value & ~dest & result)) & S16) != 0) SET_V(); }
        void CHK_ADDL_V(uint32_t dest, uint32_t value, uint32_t result) { if ((((value & dest & ~result) | (~value & ~dest & result)) & S32) != 0) SET_V(); }

        /* check overflow for subtraction for byte, word and long */
        void CHK_SUBB_V(uint8_t dest, uint8_t value, uint8_t result) { if ((((~value & dest & ~result) | (value & ~dest & result)) & S08) != 0) SET_V(); }
        void CHK_SUBW_V(uint16_t dest, uint16_t value, uint16_t result) { if ((((~value & dest & ~result) | (value & ~dest & result)) & S16) != 0) SET_V(); }
        void CHK_SUBL_V(uint32_t dest, uint32_t value, uint32_t result) { if ((((~value & dest & ~result) | (value & ~dest & result)) & S32) != 0) SET_V(); }

        /* check for privileged instruction and trap if executed */
        void CHECK_PRIVILEGED_INSTR() { if ((m_fcw & F_S_N) == 0) { m_irq_req |= Z8000_TRAP; return; } }

        /* if no EPU is present (it isn't), raise an extended intstuction trap */
        void CHECK_EXT_INSTR() { if ((m_fcw & F_EPU) == 0) { m_irq_req |= Z8000_EPU; return; } }


        /******************************************
         add byte
         flags:  CZSVDH
         ******************************************/
        uint8_t ADDB(uint8_t dest, uint8_t value)
        {
            uint8_t result = (uint8_t)(dest + value);
            CLR_CZSVH();      /* first clear C, Z, S, P/V and H flags    */
            CLR_DA();         /* clear DA (decimal adjust) flag for addb */
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            CHK_ADDX_C(dest, result);     /* set C if result overflowed              */
            CHK_ADDB_V(dest, value, result);     /* set V if result has incorrect sign      */
            CHK_ADDB_H(dest, result);     /* set H if lower nibble overflowed        */
            return result;
        }

        /******************************************
         add word
         flags:  CZSV--
         ******************************************/
        uint16_t ADDW(uint16_t dest, uint16_t value)
        {
            uint16_t result = (uint16_t)(dest + value);
            CLR_CZSV();       /* first clear C, Z, S, P/V flags          */
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            CHK_ADDX_C(dest, result);     /* set C if result overflowed              */
            CHK_ADDW_V(dest, value, result);     /* set V if result has incorrect sign      */
            return result;
        }

        /******************************************
         add long
         flags:  CZSV--
         ******************************************/
        uint32_t ADDL(uint32_t dest, uint32_t value)
        {
            uint32_t result = dest + value;
            CLR_CZSV();       /* first clear C, Z, S, P/V flags          */
            CHK_XXXL_ZS(result);    /* set Z and S flags for result long       */
            CHK_ADDX_C(dest, result);     /* set C if result overflowed              */
            CHK_ADDL_V(dest, value, result);     /* set V if result has incorrect sign      */
            return result;
        }

        /******************************************
         add with carry byte
         flags:  CZSVDH
         ******************************************/
        uint8_t ADCB(uint8_t dest, uint8_t value)
        {
            uint8_t result = (uint8_t)(dest + value + GET_C);
            CLR_CZSVH();      /* first clear C, Z, S, P/V and H flags    */
            CLR_DA();         /* clear DA (decimal adjust) flag for adcb */
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            CHK_ADCX_C(dest, value, result);     /* set C if result overflowed              */
            CHK_ADDB_V(dest, value, result);     /* set V if result has incorrect sign      */
            CHK_ADCB_H(dest, value, result);     /* set H if lower nibble overflowed        */
            return result;
        }

        /******************************************
         add with carry word
         flags:  CZSV--
         ******************************************/
        uint16_t ADCW(uint16_t dest, uint16_t value)
        {
            uint16_t result = (uint16_t)(dest + value + GET_C);
            CLR_CZSV();       /* first clear C, Z, S, P/V flags          */
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            CHK_ADCX_C(dest, value, result);     /* set C if result overflowed              */
            CHK_ADDW_V(dest, value, result);     /* set V if result has incorrect sign      */
            return result;
        }

        /******************************************
         subtract byte
         flags:  CZSVDH
         ******************************************/
        uint8_t SUBB(uint8_t dest, uint8_t value)
        {
            uint8_t result = (uint8_t)(dest - value);
            CLR_CZSVH();      /* first clear C, Z, S, P/V and H flags    */
            SET_DA();         /* set DA (decimal adjust) flag for subb   */
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            CHK_SUBX_C(dest, result);     /* set C if result underflowed             */
            CHK_SUBB_V(dest, value, result);     /* set V if result has incorrect sign      */
            CHK_SUBB_H(dest, result);     /* set H if lower nibble underflowed       */
            return result;
        }

        /******************************************
         subtract word
         flags:  CZSV--
         ******************************************/
        uint16_t SUBW(uint16_t dest, uint16_t value)
        {
            uint16_t result = (uint16_t)(dest - value);
            CLR_CZSV();       /* first clear C, Z, S, P/V flags          */
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            CHK_SUBX_C(dest, result);     /* set C if result underflowed             */
            CHK_SUBW_V(dest, value, result);     /* set V if result has incorrect sign      */
            return result;
        }

        /******************************************
         subtract long
         flags:  CZSV--
         ******************************************/
        uint32_t SUBL(uint32_t dest, uint32_t value)
        {
            uint32_t result = dest - value;
            CLR_CZSV();       /* first clear C, Z, S, P/V flags          */
            CHK_XXXL_ZS(result);    /* set Z and S flags for result long       */
            CHK_SUBX_C(dest, result);     /* set C if result underflowed             */
            CHK_SUBL_V(dest, value, result);     /* set V if result has incorrect sign      */
            return result;
        }

        /******************************************
         subtract with carry byte
         flags:  CZSVDH
         ******************************************/
        uint8_t SBCB(uint8_t dest, uint8_t value)
        {
            uint8_t result = (uint8_t)(dest - value - GET_C);
            CLR_CZSVH();      /* first clear C, Z, S, P/V and H flags    */
            SET_DA();         /* set DA (decimal adjust) flag for sbcb   */
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            CHK_SBCX_C(dest, value, result);     /* set C if result underflowed             */
            CHK_SUBB_V(dest, value, result);     /* set V if result has incorrect sign      */
            CHK_SBCB_H(dest, value, result);     /* set H if lower nibble underflowed       */
            return result;
        }

        /******************************************
         subtract with carry word
         flags:  CZSV--
         ******************************************/
        uint16_t SBCW(uint16_t dest, uint16_t value)
        {
            uint16_t result = (uint16_t)(dest - value - GET_C);
            CLR_CZSV();       /* first clear C, Z, S, P/V flags          */
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            CHK_SBCX_C(dest, value, result);     /* set C if result underflowed             */
            CHK_SUBW_V(dest, value, result);     /* set V if result has incorrect sign      */
            return result;
        }

        /******************************************
         logical or byte
         flags:  -ZSP--
         ******************************************/
        uint8_t ORB(uint8_t dest, uint8_t value)
        {
            uint8_t result = (uint8_t)(dest | value);
            CLR_ZSP();        /* first clear Z, S, P/V flags             */
            CHK_XXXB_ZSP(result);   /* set Z, S and P flags for result byte    */
            return result;
        }

        /******************************************
         logical or word
         flags:  -ZS---
         ******************************************/
        uint16_t ORW(uint16_t dest, uint16_t value)
        {
            uint16_t result = (uint16_t)(dest | value);
            CLR_ZS();         /* first clear Z, and S flags              */
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            return result;
        }

        /******************************************
         logical and byte
         flags:  -ZSP--
         ******************************************/
        uint8_t ANDB(uint8_t dest, uint8_t value)
        {
            uint8_t result = (uint8_t)(dest & value);
            CLR_ZSP();        /* first clear Z,S and P/V flags           */
            CHK_XXXB_ZSP(result);   /* set Z, S and P flags for result byte    */
            return result;
        }

        /******************************************
         logical and word
         flags:  -ZS---
         ******************************************/
        uint16_t ANDW(uint16_t dest, uint16_t value)
        {
            uint16_t result = (uint16_t)(dest & value);
            CLR_ZS();         /* first clear Z and S flags               */
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            return result;
        }

        /******************************************
         logical exclusive or byte
         flags:  -ZSP--
         ******************************************/
        uint8_t XORB(uint8_t dest, uint8_t value)
        {
            uint8_t result = (uint8_t)(dest ^ value);
            CLR_ZSP();        /* first clear Z, S and P/V flags          */
            CHK_XXXB_ZSP(result);   /* set Z, S and P flags for result byte    */
            return result;
        }

        /******************************************
         logical exclusive or word
         flags:  -ZS---
         ******************************************/
        uint16_t XORW(uint16_t dest, uint16_t value)
        {
            uint16_t result = (uint16_t)(dest ^ value);
            CLR_ZS();         /* first clear Z and S flags               */
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            return result;
        }


        /******************************************
         compare byte
         flags:  CZSV--
         ******************************************/
        void CPB(uint8_t dest, uint8_t value)
        {
            uint8_t result = (uint8_t)(dest - value);
            CLR_CZSV();       /* first clear C, Z, S and P/V flags       */
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            CHK_SUBX_C(dest, result);     /* set C if result underflowed             */
            CHK_SUBB_V(dest, value, result);
        }

        /******************************************
         compare word
         flags:  CZSV--
         ******************************************/
        void CPW(uint16_t dest, uint16_t value)
        {
            uint16_t result = (uint16_t)(dest - value);
            CLR_CZSV();
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            CHK_SUBX_C(dest, result);     /* set C if result underflowed             */
            CHK_SUBW_V(dest, value, result);
        }

        /******************************************
         compare long
         flags:  CZSV--
         ******************************************/
        void CPL(uint32_t dest, uint32_t value)
        {
            uint32_t result = dest - value;
            CLR_CZSV();
            CHK_XXXL_ZS(result);    /* set Z and S flags for result long       */
            CHK_SUBX_C(dest, result);     /* set C if result underflowed             */
            CHK_SUBL_V(dest, value, result);
        }

        /******************************************
         complement byte
         flags: -ZSP--
         ******************************************/
        uint8_t COMB(uint8_t dest)
        {
            uint8_t result = (uint8_t)~dest;
            CLR_ZSP();
            CHK_XXXB_ZSP(result);   /* set Z, S and P flags for result byte    */
            return result;
        }

        /******************************************
         complement word
         flags: -ZS---
         ******************************************/
        uint16_t COMW(uint16_t dest)
        {
            uint16_t result = (uint16_t)~dest;
            CLR_ZS();
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            return result;
        }

        /******************************************
         negate byte
         flags:  CZSV--
         ******************************************/
        uint8_t NEGB(uint8_t dest)
        {
            uint8_t result = (uint8_t)(-dest);
            CLR_CZSV();
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            if (result > 0) SET_C();
            if (result == S08) SET_V();
            return result;
        }

        /******************************************
         negate word
         flags:  CZSV--
         ******************************************/
        uint16_t NEGW(uint16_t dest)
        {
            uint16_t result = (uint16_t)(-dest);
            CLR_CZSV();
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            if (result > 0) SET_C();
            if (result == S16) SET_V();
            return result;
        }

        /******************************************
         test byte
         flags:  -ZSP--
         ******************************************/
        void TESTB(uint8_t result)
        {
            CLR_ZSP();
            CHK_XXXB_ZSP(result);   /* set Z and S flags for result byte       */
        }

        /******************************************
         test word
         flags:  -ZS---
         ******************************************/
        void TESTW(uint16_t dest)
        {
            CLR_ZS();
            if (dest == 0) SET_Z(); else if ((dest & S16) != 0) SET_S();
        }

        /******************************************
         test long
         flags:  -ZS---
         ******************************************/
        void TESTL(uint32_t dest)
        {
            CLR_ZS();
            if (dest == 0) SET_Z(); else if ((dest & S32) != 0) SET_S();
        }

        /******************************************
         increment byte
         flags: -ZSV--
         ******************************************/
        uint8_t INCB(uint8_t dest, uint8_t value)
        {
            uint8_t result = (uint8_t)(dest + value);
            CLR_ZSV();
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            CHK_ADDB_V(dest, value, result);     /* set V if result overflowed              */
            return result;
        }

        /******************************************
         increment word
         flags: -ZSV--
         ******************************************/
        uint16_t INCW(uint16_t dest, uint16_t value)
        {
            uint16_t result = (uint16_t)(dest + value);
            CLR_ZSV();
            CHK_XXXW_ZS(result);    /* set Z and S flags for result byte       */
            CHK_ADDW_V(dest, value, result);     /* set V if result overflowed              */
            return result;
        }

        /******************************************
         decrement byte
         flags: -ZSV--
         ******************************************/
        uint8_t DECB(uint8_t dest, uint8_t value)
        {
            uint8_t result = (uint8_t)(dest - value);
            CLR_ZSV();
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            CHK_SUBB_V(dest, value, result);     /* set V if result overflowed              */
            return result;
        }

        /******************************************
         decrement word
         flags: -ZSV--
         ******************************************/
        uint16_t DECW(uint16_t dest, uint16_t value)
        {
            uint16_t result = (uint16_t)(dest - value);
            CLR_ZSV();
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            CHK_SUBW_V(dest, value, result);     /* set V if result overflowed              */
            return result;
        }

        /******************************************
         multiply words
         flags:  CZSV--
         ******************************************/
        uint32_t MULTW(uint16_t dest, uint16_t value)
        {
            uint32_t result = (uint32_t)((int32_t)(int16_t)dest * (int16_t)value);
            CLR_CZSV();
            CHK_XXXL_ZS(result);
            if (value == 0)
            {
                /* multiplication with zero is faster */
                m_icount.i += (70 - 18);
            }
            if((int32_t)result < -0x7fff || (int32_t)result >= 0x7fff) SET_C();
            return result;
        }

        /******************************************
         multiply longs
         flags:  CZSV--
         ******************************************/
        uint64_t MULTL(uint32_t dest, uint32_t value)
        {
            uint64_t result = (uint64_t)((int64_t)(int32_t)dest * (int32_t)value);
            if (value == 0)
            {
                /* multiplication with zero is faster */
                m_icount.i += (282 - 30);
            }
            else
            {
                int n;
                for(n = 0; n < 32; n++)
                    if ((dest & (1L << n)) != 0) m_icount.i -= 7;
            }
            CLR_CZSV();
            CHK_XXXQ_ZS(result);
            if((int64_t)result < -0x7fffffffL || (int64_t)result >= 0x7fffffffL) SET_C();
            return result;
        }

        /******************************************
         divide long by word
         flags: CZSV--
         ******************************************/
        uint32_t DIVW(uint32_t dest, uint16_t value)
        {
            uint32_t result = dest;
            uint16_t remainder = 0;
            CLR_CZSV();
            if (value != 0)
            {
                uint16_t qsign = (uint16_t)(((dest >> 16) ^ value) & S16);
                uint16_t rsign = (uint16_t)((dest >> 16) & S16);
                if ((int32_t)dest < 0) dest = (uint32_t)(-dest);
                if ((int16_t)value < 0) value = (uint16_t)(-value);
                result = dest / value;
                remainder = (uint16_t)(dest % value);
                if (qsign != 0) result = (uint32_t)(-result);
                if (rsign != 0) remainder = (uint16_t)(-remainder);
                if ((int32_t)result < -0x8000 || (int32_t)result > 0x7fff)
                {
                    int32_t temp = (int32_t)result >> 1;
                    SET_V();
                    if (temp >= -0x8000 && temp <= 0x7fff)
                    {
                        result = (temp < 0) ? unchecked((uint32_t)(-1)) : (uint32_t)0;
                        CHK_XXXW_ZS(result);
                        SET_C();
                    }
                }
                else
                {
                    CHK_XXXW_ZS(result);
                }
                result = ((uint32_t)remainder << 16) | (result & 0xffff);
            }
            else
            {
                SET_Z();
                SET_V();
            }
            return result;
        }

        /******************************************
         divide quad word by long
         flags: CZSV--
         ******************************************/
        uint64_t DIVL(uint64_t dest, uint32_t value)
        {
            uint64_t result = dest;
            uint32_t remainder = 0;
            CLR_CZSV();
            if (value != 0)
            {
                uint32_t qsign = (uint32_t)(((dest >> 32) ^ value) & S32);
                uint32_t rsign = (uint32_t)((dest >> 32) & S32);
                if ((int64_t)dest < 0) dest = (uint64_t)(-(int64_t)dest);
                if ((int32_t)value < 0) value = (uint32_t)(-value);
                result = dest / value;
                remainder = (uint32_t)(dest % value);
                if (qsign != 0) result = (uint64_t)(-(int64_t)result);
                if (rsign != 0) remainder = (uint32_t)(-remainder);
                if ((int64_t)result < -0x80000000L || (int64_t)result > 0x7fffffff)
                {
                    int64_t temp = (int64_t)result >> 1;
                    SET_V();
                    if (temp >= -0x80000000L && temp <= 0x7fffffff)
                    {
                        result = (temp < 0) ? unchecked((uint64_t)(-1)) : 0;
                        CHK_XXXL_ZS(result);
                        SET_C();
                    }
                }
                else
                {
                    CHK_XXXL_ZS(result);
                }
                result = ((uint64_t)remainder << 32) | (result & 0xffffffff);
            }
            else
            {
                SET_Z();
                SET_V();
            }
            return result;
        }

        /******************************************
         rotate left byte
         flags:  CZSV--
         ******************************************/
        uint8_t RLB(uint8_t dest, uint8_t twice)
        {
            uint8_t result = (uint8_t)((dest << 1) | (dest >> 7));
            CLR_CZSV();
            if (twice != 0) result = (uint8_t)((result << 1) | (result >> 7));
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            if ((result & 0x01) != 0) SET_C();
            if (((result ^ dest) & S08) != 0) SET_V();
            return result;
        }

        /******************************************
         rotate left word
         flags:  CZSV--
         ******************************************/
        uint16_t RLW(uint16_t dest, uint8_t twice)
        {
            uint16_t result = (uint16_t)((dest << 1) | (dest >> 15));
            CLR_CZSV();
            if (twice != 0) result = (uint16_t)((result << 1) | (result >> 15));
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            if ((result & 0x0001) != 0) SET_C();
            if (((result ^ dest) & S16) != 0) SET_V();
            return result;
        }

        /******************************************
         rotate left through carry byte
         flags:  CZSV--
         ******************************************/
        uint8_t RLCB(uint8_t dest, uint8_t twice)
        {
            uint8_t c = (uint8_t)(dest & S08);
            uint8_t result = (uint8_t)((dest << 1) | GET_C);
            CLR_CZSV();
            if (twice != 0)
            {
                uint8_t c1 = (uint8_t)(c >> 7);
                c = (uint8_t)(result & S08);
                result = (uint8_t)((result << 1) | c1);
            }
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S08) != 0) SET_V();
            return result;
        }

        /******************************************
         rotate left through carry word
         flags:  CZSV--
         ******************************************/
        uint16_t RLCW(uint16_t dest, uint8_t twice)
        {
            uint16_t c = (uint16_t)(dest & S16);
            uint16_t result = (uint16_t)((dest << 1) | GET_C);
            CLR_CZSV();
            if (twice != 0)
            {
                uint16_t c1 = (uint16_t)(c >> 15);
                c = (uint16_t)(result & S16);
                result = (uint16_t)((result << 1) | c1);
            }
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S16) != 0) SET_V();
            return result;
        }

        /******************************************
         rotate right byte
         flags:  CZSV--
         ******************************************/
        uint8_t RRB(uint8_t dest, uint8_t twice)
        {
            uint8_t result = (uint8_t)((dest >> 1) | (dest << 7));
            CLR_CZSV();
            if (twice != 0) result = (uint8_t)((result >> 1) | (result << 7));
            if (result == 0) SET_Z(); else if ((result & S08) != 0) SET_SC();
            if (((result ^ dest) & S08) != 0) SET_V();
            return result;
        }

        /******************************************
         rotate right word
         flags:  CZSV--
         ******************************************/
        uint16_t RRW(uint16_t dest, uint8_t twice)
        {
            uint16_t result = (uint16_t)((dest >> 1) | (dest << 15));
            CLR_CZSV();
            if (twice != 0) result = (uint16_t)((result >> 1) | (result << 15));
            if (result == 0) SET_Z(); else if ((result & S16) != 0) SET_SC();
            if (((result ^ dest) & S16) != 0) SET_V();
            return result;
        }

        /******************************************
         rotate right through carry byte
         flags:  CZSV--
         ******************************************/
        uint8_t RRCB(uint8_t dest, uint8_t twice)
        {
            uint8_t c = (uint8_t)(dest & 1);
            uint8_t result = (uint8_t)((dest >> 1) | (GET_C << 7));
            CLR_CZSV();
            if (twice != 0)
            {
                uint8_t c1 = (uint8_t)(c << 7);
                c = (uint8_t)(result & 1);
                result = (uint8_t)((result >> 1) | c1);
            }
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S08) != 0) SET_V();
            return result;
        }

        /******************************************
         rotate right through carry word
         flags:  CZSV--
         ******************************************/
        uint16_t RRCW(uint16_t dest, uint8_t twice)
        {
            uint16_t c = (uint16_t)(dest & 1);
            uint16_t result = (uint16_t)((dest >> 1) | (GET_C << 15));
            CLR_CZSV();
            if (twice != 0)
            {
                uint16_t c1 = (uint16_t)(c << 15);
                c = (uint16_t)(result & 1);
                result = (uint16_t)((result >> 1) | c1);
            }
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S16) != 0) SET_V();
            return result;
        }

        /******************************************
         shift dynamic arithmetic byte
         flags:  CZSV--
         ******************************************/
        uint8_t SDAB(uint8_t dest, int8_t count)
        {
            int8_t result = (int8_t) dest;
            uint8_t c = 0;
            CLR_CZSV();
            while (count > 0)
            {
                c = (uint8_t)(result & S08);
                result <<= 1;
                count--;
            }
            while (count < 0)
            {
                c = (uint8_t)(result & 0x01);
                result >>= 1;
                count++;
            }
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S08) != 0) SET_V();
            return (uint8_t)result;
        }

        /******************************************
         shift dynamic arithmetic word
         flags:  CZSV--
         ******************************************/
        uint16_t SDAW(uint16_t dest, int8_t count)
        {
            int16_t result = (int16_t) dest;
            uint16_t c = 0;
            CLR_CZSV();
            while (count > 0)
            {
                c = (uint16_t)(result & S16);
                result <<= 1;
                count--;
            }
            while (count < 0)
            {
                c = (uint16_t)(result & 0x0001);
                result >>= 1;
                count++;
            }
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S16) != 0) SET_V();
            return (uint16_t)result;
        }

        /******************************************
         shift dynamic arithmetic long
         flags:  CZSV--
         ******************************************/
        uint32_t SDAL(uint32_t dest, int8_t count)
        {
            int32_t result = (int32_t) dest;
            uint32_t c = 0;
            CLR_CZSV();
            while (count > 0)
            {
                c = (uint32_t)(result & S32);
                result <<= 1;
                count--;
            }
            while (count < 0)
            {
                c = (uint32_t)(result & 0x00000001);
                result >>= 1;
                count++;
            }
            CHK_XXXL_ZS(result);    /* set Z and S flags for result long       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S32) != 0) SET_V();
            return (uint32_t) result;
        }

        /******************************************
         shift dynamic logic byte
         flags:  CZSV--
         ******************************************/
        uint8_t SDLB(uint8_t dest, int8_t count)
        {
            uint8_t result = dest;
            uint8_t c = 0;
            CLR_CZSV();
            while (count > 0)
            {
                c = (uint8_t)(result & S08);
                result <<= 1;
                count--;
            }
            while (count < 0)
            {
                c = (uint8_t)(result & 0x01);
                result >>= 1;
                count++;
            }
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S08) != 0) SET_V();
            return result;
        }

        /******************************************
         shift dynamic logic word
         flags:  CZSV--
         ******************************************/
        uint16_t SDLW(uint16_t dest, int8_t count)
        {
            uint16_t result = dest;
            uint16_t c = 0;
            CLR_CZSV();
            while (count > 0)
            {
                c = (uint16_t)(result & S16);
                result <<= 1;
                count--;
            }
            while (count < 0)
            {
                c = (uint16_t)(result & 0x0001);
                result >>= 1;
                count++;
            }
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S16) != 0) SET_V();
            return result;
        }

        /******************************************
         shift dynamic logic long
         flags:  CZSV--
         ******************************************/
        uint32_t SDLL(uint32_t dest, int8_t count)
        {
            uint32_t result = dest;
            uint32_t c = 0;
            CLR_CZSV();
            while (count > 0) {
                c = result & S32;
                result <<= 1;
                count--;
            }
            while (count < 0) {
                c = result & 0x00000001;
                result >>= 1;
                count++;
            }
            CHK_XXXL_ZS(result);    /* set Z and S flags for result long       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S32) != 0) SET_V();
            return result;
        }

        /******************************************
         shift left arithmetic byte
         flags:  CZSV--
         ******************************************/
        uint8_t SLAB(uint8_t dest, uint8_t count)
        {
            uint8_t c = (count != 0) ? (uint8_t)((dest << (count - 1)) & S08) : (uint8_t)0;
            uint8_t result = (uint8_t)((int8_t)dest << count);
            CLR_CZSV();
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S08) != 0) SET_V();
            return result;
        }

        /******************************************
         shift left arithmetic word
         flags:  CZSV--
         ******************************************/
        uint16_t SLAW(uint16_t dest, uint8_t count)
        {
            uint16_t c = (count != 0) ? (uint16_t)((dest << (count - 1)) & S16) : (uint16_t)0;
            uint16_t result = (uint16_t)((int16_t)dest << count);
            CLR_CZSV();
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S16) != 0) SET_V();
            return result;
        }

        /******************************************
         shift left arithmetic long
         flags:  CZSV--
         ******************************************/
        uint32_t SLAL(uint32_t dest, uint8_t count)
        {
            uint32_t c = (count != 0) ? (dest << (count - 1)) & S32 : 0;
            uint32_t result = (uint32_t)((int32_t)dest << count);
            CLR_CZSV();
            CHK_XXXL_ZS(result);    /* set Z and S flags for result long       */
            if (c != 0) SET_C();
            if (((result ^ dest) & S32) != 0) SET_V();
            return result;
        }

        /******************************************
         shift left logic byte
         flags:  CZS---
         ******************************************/
        uint8_t SLLB(uint8_t dest, uint8_t count)
        {
            uint8_t c = (count != 0) ? (uint8_t)((dest << (count - 1)) & S08) : (uint8_t)0;
            uint8_t result = (uint8_t)(dest << count);
            CLR_CZS();
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            if (c != 0) SET_C();
            return result;
        }

        /******************************************
         shift left logic word
         flags:  CZS---
         ******************************************/
        uint16_t SLLW(uint16_t dest, uint8_t count)
        {
            uint16_t c = (count != 0) ? (uint16_t)((dest << (count - 1)) & S16) : (uint16_t)0;
            uint16_t result = (uint16_t)(dest << count);
            CLR_CZS();
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            if (c != 0) SET_C();
            return result;
        }

        /******************************************
         shift left logic long
         flags:  CZS---
         ******************************************/
        uint32_t SLLL(uint32_t dest, uint8_t count)
        {
            uint32_t c = (count != 0) ? (dest << (count - 1)) & S32 : 0;
            uint32_t result = dest << count;
            CLR_CZS();
            CHK_XXXL_ZS(result);    /* set Z and S flags for result long       */
            if (c != 0) SET_C();
            return result;
        }

        /******************************************
         shift right arithmetic byte
         flags:  CZSV--
         ******************************************/
        uint8_t SRAB(uint8_t dest, uint8_t count)
        {
            uint8_t c = (count != 0) ? (uint8_t)(((int8_t)dest >> (count - 1)) & 1) : (uint8_t)0;
            uint8_t result = (uint8_t)((int8_t)dest >> count);
            CLR_CZSV();
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            if (c != 0) SET_C();
            return result;
        }

        /******************************************
         shift right arithmetic word
         flags:  CZSV--
         ******************************************/
        uint16_t SRAW(uint16_t dest, uint8_t count)
        {
            uint8_t c = (count != 0) ? (uint8_t)(((int16_t)dest >> (count - 1)) & 1) : (uint8_t)0;
            uint16_t result = (uint16_t)((int16_t)dest >> count);
            CLR_CZSV();
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            if (c != 0) SET_C();
            return result;
        }

        /******************************************
         shift right arithmetic long
         flags:  CZSV--
         ******************************************/
        uint32_t SRAL(uint32_t dest, uint8_t count)
        {
            uint8_t c = (count != 0) ? (uint8_t)(((int32_t)dest >> (count - 1)) & 1) : (uint8_t)0;
            uint32_t result = (uint32_t)((int32_t)dest >> count);
            CLR_CZSV();
            CHK_XXXL_ZS(result);    /* set Z and S flags for result long       */
            if (c != 0) SET_C();
            return result;
        }

        /******************************************
         shift right logic byte
         flags:  CZSV--
         ******************************************/
        uint8_t SRLB(uint8_t dest, uint8_t count)
        {
            uint8_t c = (count != 0) ? (uint8_t)((dest >> (count - 1)) & 1) : (uint8_t)0;
            uint8_t result = (uint8_t)(dest >> count);
            CLR_CZS();
            CHK_XXXB_ZS(result);    /* set Z and S flags for result byte       */
            if (c != 0) SET_C();
            return result;
        }

        /******************************************
         shift right logic word
         flags:  CZSV--
         ******************************************/
        uint16_t SRLW(uint16_t dest, uint8_t count)
        {
            uint8_t c = (count != 0) ? (uint8_t)((dest >> (count - 1)) & 1) : (uint8_t)0;
            uint16_t result = (uint16_t)(dest >> count);
            CLR_CZS();
            CHK_XXXW_ZS(result);    /* set Z and S flags for result word       */
            if (c != 0) SET_C();
            return result;
        }

        /******************************************
         shift right logic long
         flags:  CZSV--
         ******************************************/
        uint32_t SRLL(uint32_t dest, uint8_t count)
        {
            uint8_t c = (count != 0) ? (uint8_t)((dest >> (count - 1)) & 1) : (uint8_t)0;
            uint32_t result = dest >> count;
            CLR_CZS();
            CHK_XXXL_ZS(result);    /* set Z and S flags for result long       */
            if (c != 0) SET_C();
            return result;
        }

        /******************************************
         invalid
         flags:  ------
         ******************************************/
        void zinvalid()
        {
            logerror("Z8000 invalid opcode {0}: {1}\n", m_pc, m_op[0]);
        }

        /******************************************
         addb    rbd,imm8
         flags:  CZSVDH
         ******************************************/
        void Z00_0000_dddd_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t imm8 = GET_IMM8(OP1);
            RB(dst).op = ADDB(RB(dst).op, imm8);
        }

        /******************************************
         addb    rbd,@rs
         flags:  CZSVDH
         ******************************************/
        void Z00_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = ADDB(RB(dst).op, RDIR_B(src));
        }

        /******************************************
         add     rd,imm16
         flags:  CZSV--
         ******************************************/
        void Z01_0000_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint16_t imm16 = GET_IMM16(OP1);
            RW(dst).op = ADDW(RW(dst).op, imm16);
        }

        /******************************************
         add     rd,@rs
         flags:  CZSV--
         ******************************************/
        void Z01_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = ADDW(RW(dst).op, RDIR_W(src));
        }

        /******************************************
         subb    rbd,imm8
         flags:  CZSVDH
         ******************************************/
        void Z02_0000_dddd_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t imm8 = GET_IMM8(OP1);
            RB(dst).op = SUBB(RB(dst).op, imm8);
        }

        /******************************************
         subb    rbd,@rs
         flags:  CZSVDH
         ******************************************/
        void Z02_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = SUBB(RB(dst).op, RDIR_B(src)); /* EHC */
        }

        /******************************************
         sub     rd,imm16
         flags:  CZSV--
         ******************************************/
        void Z03_0000_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint16_t imm16 = GET_IMM16(OP1);
            RW(dst).op = SUBW(RW(dst).op, imm16);
        }

        /******************************************
         sub     rd,@rs
         flags:  CZSV--
         ******************************************/
        void Z03_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = SUBW(RW(dst).op, RDIR_W(src));
        }

        /******************************************
         orb     rbd,imm8
         flags:  CZSP--
         ******************************************/
        void Z04_0000_dddd_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t imm8 = GET_IMM8(OP1);
            RB(dst).op = ORB(RB(dst).op, imm8);
        }

        /******************************************
         orb     rbd,@rs
         flags:  CZSP--
         ******************************************/
        void Z04_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = ORB(RB(dst).op, RDIR_B(src));
        }

        /******************************************
         or      rd,imm16
         flags:  CZS---
         ******************************************/
        void Z05_0000_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint16_t imm16 = GET_IMM16(OP1);
            RW(dst).op = ORW(RW(dst).op, imm16);
        }

        /******************************************
         or      rd,@rs
         flags:  CZS---
         ******************************************/
        void Z05_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = ORW(RW(dst).op, RDIR_W(src));
        }

        /******************************************
         andb    rbd,imm8
         flags:  -ZSP--
         ******************************************/
        void Z06_0000_dddd_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t imm8 = GET_IMM8(OP1);
            RB(dst).op = ANDB(RB(dst).op, imm8);
        }

        /******************************************
         andb    rbd,@rs
         flags:  -ZSP--
         ******************************************/
        void Z06_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = ANDB(RB(dst).op, RDIR_B(src));
        }

        /******************************************
         and     rd,imm16
         flags:  -ZS---
         ******************************************/
        void Z07_0000_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint16_t imm16 = GET_IMM16(OP1);
            RW(dst).op = ANDW(RW(dst).op, imm16);
        }

        /******************************************
         and     rd,@rs
         flags:  -ZS---
         ******************************************/
        void Z07_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = ANDW(RW(dst).op, RDIR_W(src));
        }

        /******************************************
         xorb    rbd,imm8
         flags:  -ZSP--
         ******************************************/
        void Z08_0000_dddd_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t imm8 = GET_IMM8(OP1);
            RB(dst).op = XORB(RB(dst).op, imm8);
        }

        /******************************************
         xorb    rbd,@rs
         flags:  -ZSP--
         ******************************************/
        void Z08_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = XORB(RB(dst).op, RDIR_B(src));
        }

        /******************************************
         xor     rd,imm16
         flags:  -ZS---
         ******************************************/
        void Z09_0000_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint16_t imm16 = GET_IMM16(OP1);
            RW(dst).op = XORW(RW(dst).op, imm16);
        }

        /******************************************
         xor     rd,@rs
         flags:  -ZS---
         ******************************************/
        void Z09_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = XORW(RW(dst).op, RDIR_W(src));
        }

        /******************************************
         cpb     rbd,imm8
         flags:  CZSV--
         ******************************************/
        void Z0A_0000_dddd_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t imm8 = GET_IMM8(OP1);
            CPB(RB(dst).op, imm8);
        }

        /******************************************
         cpb     rbd,@rs
         flags:  CZSV--
         ******************************************/
        void Z0A_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            CPB(RB(dst).op, RDIR_B(src));
        }

        /******************************************
         cp      rd,imm16
         flags:  CZSV--
         ******************************************/
        void Z0B_0000_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint16_t imm16 = GET_IMM16(OP1);
            CPW(RW(dst).op, imm16);
        }

        /******************************************
         cp      rd,@rs
         flags:  CZSV--
         ******************************************/
        void Z0B_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            CPW(RW(dst).op, RDIR_W(src));
        }

        /******************************************
         comb    @rd
         flags:  -ZSP--
         ******************************************/
        void Z0C_ddN0_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_B(space, addr, COMB(RDMEM_B(space, addr)));
        }

        /******************************************
         cpb     @rd,imm8
         flags:  CZSV--
         ******************************************/
        void Z0C_ddN0_0001_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm8 = GET_IMM8(OP1);
            CPB(RDIR_B(dst), imm8); // @@@done
        }

        /******************************************
         negb    @rd
         flags:  CZSV--
         ******************************************/
        void Z0C_ddN0_0010()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_B(space, addr, NEGB(RDMEM_B(space, addr)));
        }

        /******************************************
         testb   @rd
         flags:  -ZSP--
         ******************************************/
        void Z0C_ddN0_0100()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            TESTB(RDIR_B(dst));
        }

        /******************************************
         ldb     @rd,imm8
         flags:  ------
         ******************************************/
        void Z0C_ddN0_0101_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm8 = GET_IMM8(OP1);
            WRIR_B(dst, imm8);
        }

        /******************************************
         tsetb   @rd
         flags:  --S---
         ******************************************/
        void Z0C_ddN0_0110()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            if ((RDMEM_B(space, addr) & S08) != 0) SET_S(); else CLR_S();
            WRMEM_B(space, addr, 0xff);
        }

        /******************************************
         clrb    @rd
         flags:  ------
         ******************************************/
        void Z0C_ddN0_1000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            WRIR_B(dst, 0);
        }

        /******************************************
         com     @rd
         flags:  -ZS---
         ******************************************/
        void Z0D_ddN0_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_W(space, addr, COMW(RDMEM_W(space, addr)));
        }

        /******************************************
         cp      @rd,imm16
         flags:  CZSV--
         ******************************************/
        void Z0D_ddN0_0001_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            CPW(RDIR_W(dst), imm16);
        }

        /******************************************
         neg     @rd
         flags:  CZSV--
         ******************************************/
        void Z0D_ddN0_0010()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_W(space, addr, NEGW(RDMEM_W(space, addr)));
        }

        /******************************************
         test    @rd
         flags:  -ZS---
         ******************************************/
        void Z0D_ddN0_0100()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            TESTW(RDIR_W(dst));
        }

        /******************************************
         ld      @rd,imm16
         flags:  ------
         ******************************************/
        void Z0D_ddN0_0101_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            WRIR_W(dst, imm16);
        }

        /******************************************
         tset    @rd
         flags:  --S---
         ******************************************/
        void Z0D_ddN0_0110()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            if ((RDMEM_W(space, addr) & S16) != 0) SET_S(); else CLR_S();
            WRMEM_W(space, addr, 0xffff);
        }

        /******************************************
         clr     @rd
         flags:  ------
         ******************************************/
        void Z0D_ddN0_1000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            WRIR_W(dst, 0);
        }

        /******************************************
         push    @rd,imm16
         flags:  ------
         ******************************************/
        void Z0D_ddN0_1001_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            PUSHW(dst, imm16);
        }

        /******************************************
         ext0e   imm8
         flags:  ------
         ******************************************/
        void Z0E_imm8()
        {
            CHECK_EXT_INSTR();
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: ext0e  ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
        }

        /******************************************
         ext0f   imm8
         flags:  ------
         ******************************************/
        void Z0F_imm8()
        {
            CHECK_EXT_INSTR();
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: ext0f  ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
        }

        /******************************************
         cpl     rrd,imm32
         flags:  CZSV--
         ******************************************/
        void Z10_0000_dddd_imm32()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t imm32 = GET_IMM32;
            CPL(RL(dst).op, imm32);
        }

        /******************************************
         cpl     rrd,@rs
         flags:  CZSV--
         ******************************************/
        void Z10_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            CPL(RL(dst).op, RDIR_L(src));
        }

        /******************************************
         pushl   @rd,@rs
         flags:  ------
         ******************************************/
        void Z11_ddN0_ssN0()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            PUSHL(dst, RDIR_L(src));
        }

        /******************************************
         subl    rrd,imm32
         flags:  CZSV--
         ******************************************/
        void Z12_0000_dddd_imm32()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t imm32 = GET_IMM32;
            RL(dst).op = SUBL(RL(dst).op, imm32);
        }

        /******************************************
         subl    rrd,@rs
         flags:  CZSV--
         ******************************************/
        void Z12_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = SUBL(RL(dst).op, RDIR_L(src));
        }

        /******************************************
         push    @rd,@rs
         flags:  ------
         ******************************************/
        void Z13_ddN0_ssN0()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            PUSHW(dst, RDIR_W(src));
        }

        /******************************************
         ldl     rrd,imm32
         flags:  ------
         ******************************************/
        void Z14_0000_dddd_imm32()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t imm32 = GET_IMM32;
            RL(dst).op = imm32;
        }

        /******************************************
         ldl     rrd,@rs
         flags:  ------
         ******************************************/
        void Z14_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = RDIR_L(src);
        }

        /******************************************
         popl    rd,@rs
         flags:  ------
         ******************************************/
        void Z15_ssN0_ddN0()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = POPL(src);
        }

        /******************************************
         addl    rrd,imm32
         flags:  CZSV--
         ******************************************/
        void Z16_0000_dddd_imm32()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t imm32 = GET_IMM32;
            RL(dst).op = ADDL(RL(dst).op, imm32);
        }

        /******************************************
         addl    rrd,@rs
         flags:  CZSV--
         ******************************************/
        void Z16_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = ADDL(RL(dst).op, RDIR_L(src));
        }

        /******************************************
         pop     @rd,@rs
         flags:  ------
         ******************************************/
        void Z17_ssN0_ddN0()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            WRIR_W(dst, POPW(src));
        }

        /******************************************
         multl   rqd,imm32
         flags:  CZSV--
         ******************************************/
        void Z18_00N0_dddd_imm32()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t imm32 = GET_IMM32;
            RQ(dst).op = MULTL((uint32_t)RQ(dst).op, imm32);
        }

        /******************************************
         multl   rqd,@rs
         flags:  CZSV--
         ******************************************/
        void Z18_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RQ(dst).op = MULTL((uint32_t)RQ(dst).op, RL(src).op); //@@@
        }

        /******************************************
         mult    rrd,imm16
         flags:  CZSV--
         ******************************************/
        void Z19_0000_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint16_t imm16 = GET_IMM16(OP1);
            RL(dst).op = MULTW((uint16_t)RL(dst).op, imm16);
        }

        /******************************************
         mult    rrd,@rs
         flags:  CZSV--
         ******************************************/
        void Z19_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = MULTW((uint16_t)RL(dst).op, RDIR_W(src));
        }

        /******************************************
         divl    rqd,imm32
         flags:  CZSV--
         ******************************************/
        void Z1A_0000_dddd_imm32()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t imm32 = GET_IMM32;
            RQ(dst).op = DIVL(RQ(dst).op, imm32);
        }

        /******************************************
         divl    rqd,@rs
         flags:  CZSV--
         ******************************************/
        void Z1A_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RQ(dst).op = DIVL(RQ(dst).op, RDIR_L(src));
        }

        /******************************************
         div     rrd,imm16
         flags:  CZSV--
         ******************************************/
        void Z1B_0000_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint16_t imm16 = GET_IMM16(OP1);
            RL(dst).op = DIVW(RL(dst).op, imm16);
        }

        /******************************************
         div     rrd,@rs
         flags:  CZSV--
         ******************************************/
        void Z1B_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = DIVW(RL(dst).op, RDIR_W(src));
        }

        /******************************************
         testl   @rd
         flags:  -ZS---
         ******************************************/
        void Z1C_ddN0_1000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            TESTL(RDIR_L(dst));
        }

        /******************************************
         ldm     @rd,rs,n
         flags:  ------
         ******************************************/
        void Z1C_ddN0_1001_0000_ssss_0000_nmin1()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB3);
            uint8_t src = GET_SRC(OP1,NIB1);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            while (cnt-- >= 0)
            {
                WRMEM_W(space, addr, RW(src).op);
                addr = addr_add(addr, 2);
                src = (uint8_t)((src + 1) & 15);
            }
        }

        /******************************************
         ldm     rd,@rs,n
         flags:  ------
         ******************************************/
        void Z1C_ssN0_0001_0000_dddd_0000_nmin1()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB1);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = src == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(src);
            while (cnt-- >= 0)
            {
                RW(dst).op = RDMEM_W(space, addr);
                addr = addr_add(addr, 2);
                dst = (uint8_t)((dst + 1) & 15);
            }
        }

        /******************************************
         ldl     @rd,rrs
         flags:  ------
         ******************************************/
        void Z1D_ddN0_ssss()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            WRIR_L(dst, RL(src).op);
        }

        /******************************************
         jp      cc,rd
         flags:  ------
         ******************************************/
        void Z1E_ddN0_cccc()
        {
            uint8_t cc = GET_CCC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            switch (cc)
            {
                case  0: if (CC0) set_pc(addr_from_reg(dst)); break;
                case  1: if (CC1) set_pc(addr_from_reg(dst)); break;
                case  2: if (CC2) set_pc(addr_from_reg(dst)); break;
                case  3: if (CC3) set_pc(addr_from_reg(dst)); break;
                case  4: if (CC4) set_pc(addr_from_reg(dst)); break;
                case  5: if (CC5) set_pc(addr_from_reg(dst)); break;
                case  6: if (CC6) set_pc(addr_from_reg(dst)); break;
                case  7: if (CC7) set_pc(addr_from_reg(dst)); break;
                case  8: if (CC8) set_pc(addr_from_reg(dst)); break;
                case  9: if (CC9) set_pc(addr_from_reg(dst)); break;
                case 10: if (CCA) set_pc(addr_from_reg(dst)); break;
                case 11: if (CCB) set_pc(addr_from_reg(dst)); break;
                case 12: if (CCC) set_pc(addr_from_reg(dst)); break;
                case 13: if (CCD) set_pc(addr_from_reg(dst)); break;
                case 14: if (CCE) set_pc(addr_from_reg(dst)); break;
                case 15: if (CCF) set_pc(addr_from_reg(dst)); break;
            }
        }

        /******************************************
         call    @rd
         flags:  ------
         ******************************************/
        void Z1F_ddN0_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            if (get_segmented_mode())
                PUSHL(SP, make_segmented_addr(m_pc));
            else
                PUSHW(SP, (uint16_t)m_pc);
            set_pc(addr_from_reg(dst));
        }

        /******************************************
         ldb     rbd,@rs
         flags:  ------
         ******************************************/
        void Z20_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = RDIR_B(src);
        }

        /******************************************
         ld      rd,imm16
         flags:  ------
         ******************************************/
        void Z21_0000_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint16_t imm16 = GET_IMM16(OP1);
            RW(dst).op = imm16;
        }

        /******************************************
         ld      rd,@rs
         flags:  ------
         ******************************************/
        void Z21_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = RDIR_W(src);
        }

        /******************************************
         resb    rbd,rs
         flags:  ------
         ******************************************/
        void Z22_0000_ssss_0000_dddd_0000_0000()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP1,NIB1);
            RB(dst).op = (uint8_t)(RB(dst).op & ~(1 << (RW(src).op & 7)));
        }

        /******************************************
         resb    @rd,imm4
         flags:  ------
         ******************************************/
        void Z22_ddN0_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_B(space, addr, (uint8_t)(RDMEM_B(space, addr) & ~bit));
        }

        /******************************************
         res     rd,rs
         flags:  ------
         ******************************************/
        void Z23_0000_ssss_0000_dddd_0000_0000()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP1,NIB1);
            RW(dst).op = (uint16_t)(RW(dst).op & ~(1 << (RW(src).op & 15)));
        }

        /******************************************
         res     @rd,imm4
         flags:  ------
         ******************************************/
        void Z23_ddN0_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_W(space, addr, (uint16_t)(RDMEM_W(space, addr) & ~bit));
        }

        /******************************************
         setb    rbd,rs
         flags:  ------
         ******************************************/
        void Z24_0000_ssss_0000_dddd_0000_0000()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP1,NIB1);
            RB(dst).op = (uint8_t)(RB(dst).op | (1 << (RW(src).op & 7)));
        }

        /******************************************
         setb    @rd,imm4
         flags:  ------
         ******************************************/
        void Z24_ddN0_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_B(space, addr, (uint8_t)(RDMEM_B(space, addr) | bit));
        }

        /******************************************
         set     rd,rs
         flags:  ------
         ******************************************/
        void Z25_0000_ssss_0000_dddd_0000_0000()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP1,NIB1);
            RW(dst).op = (uint16_t)(RW(dst).op | (1 << (RW(src).op & 15)));
        }

        /******************************************
         set     @rd,imm4
         flags:  ------
         ******************************************/
        void Z25_ddN0_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_W(space, addr, (uint16_t)(RDMEM_W(space, addr) | bit));
        }

        /******************************************
         bitb    rbd,rs
         flags:  -Z----
         ******************************************/
        void Z26_0000_ssss_0000_dddd_0000_0000()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP1,NIB1);
            if ((RB(dst).op & (1 << (RW(src).op & 7))) != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         bitb    @rd,imm4
         flags:  -Z----
         ******************************************/
        void Z26_ddN0_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            if ((RDIR_B(dst) & bit) != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         bit     rd,rs
         flags:  -Z----
         ******************************************/
        void Z27_0000_ssss_0000_dddd_0000_0000()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP1,NIB1);
            if ((RW(dst).op & (1 << (RW(src).op & 15))) != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         bit     @rd,imm4
         flags:  -Z----
         ******************************************/
        void Z27_ddN0_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            if ((RDIR_W(dst) & bit) != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         incb    @rd,imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z28_ddN0_imm4m1()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_B(space, addr, INCB(RDMEM_B(space, addr), i4p1));
        }

        /******************************************
         inc     @rd,imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z29_ddN0_imm4m1()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_W(space, addr, INCW(RDMEM_W(space, addr), i4p1));
        }

        /******************************************
         decb    @rd,imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z2A_ddN0_imm4m1()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_B(space, addr, DECB(RDMEM_B(space, addr), i4p1));
        }

        /******************************************
         dec     @rd,imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z2B_ddN0_imm4m1()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = dst == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(dst);
            WRMEM_W(space, addr, DECW(RDMEM_W(space, addr), i4p1));
        }

        /******************************************
         exb     rbd,@rs
         flags:  ------
         ******************************************/
        void Z2C_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = src == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(src);
            uint8_t tmp = RDMEM_B(space, addr);
            WRMEM_B(space, addr, RB(dst).op);
            RB(dst).op = tmp;
        }

        /******************************************
         ex      rd,@rs
         flags:  ------
         ******************************************/
        void Z2D_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = src == SP ? m_stack : m_data;
            uint32_t addr = addr_from_reg(src);
            uint16_t tmp = RDMEM_W(space, addr);
            WRMEM_W(space, addr, RW(dst).op);
            RW(dst).op = tmp;
        }

        /******************************************
         ldb     @rd,rbs
         flags:  ------
         ******************************************/
        void Z2E_ddN0_ssss()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            WRIR_B(dst, RB(src).op);
        }

        /******************************************
         ld      @rd,rs
         flags:  ------
         ******************************************/
        void Z2F_ddN0_ssss()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            WRIR_W(dst, RW(src).op);
        }

        /******************************************
         ldrb    rbd,dsp16
         flags:  ------
         ******************************************/
        void Z30_0000_dddd_dsp16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t dsp16 = GET_DSP16;
            RB(dst).op = RDMEM_B(m_program, dsp16);
        }

        /******************************************
         ldb     rbd,rs(idx16)
         flags:  ------
         ******************************************/
        void Z30_ssN0_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t idx16 = GET_IDX16(OP1);
            RB(dst).op = RDBX_B(src, (uint16_t)idx16);
        }

        /******************************************
         ldr     rd,dsp16
         flags:  ------
         ******************************************/
        void Z31_0000_dddd_dsp16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t dsp16 = GET_DSP16;
            RW(dst).op = RDMEM_W(m_program, dsp16);
        }

        /******************************************
         ld      rd,rs(idx16)
         flags:  ------
         ******************************************/
        void Z31_ssN0_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t idx16 = GET_IDX16(OP1);
            RW(dst).op = RDBX_W(src, (uint16_t)idx16);
        }

        /******************************************
         ldrb    dsp16,rbs
         flags:  ------
         ******************************************/
        void Z32_0000_ssss_dsp16()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint32_t dsp16 = GET_DSP16;
            WRMEM_B(m_program, dsp16, RB(src).op);
        }

        /******************************************
         ldb     rd(idx16),rbs
         flags:  ------
         ******************************************/
        void Z32_ddN0_ssss_imm16()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t idx16 = GET_IDX16(OP1);
            WRBX_B(dst, (uint16_t)idx16, RB(src).op);
        }

        /******************************************
         ldr     dsp16,rs
         flags:  ------
         ******************************************/
        void Z33_0000_ssss_dsp16()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint32_t dsp16 = GET_DSP16;
            WRMEM_W(m_program, dsp16, RW(src).op);
        }

        /******************************************
         ld      rd(idx16),rs
         flags:  ------
         ******************************************/
        void Z33_ddN0_ssss_imm16()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t idx16 = GET_IDX16(OP1);
            WRBX_W(dst, (uint16_t)idx16, RW(src).op);
        }

        /******************************************
         ldar    prd,dsp16
         flags:  ------
         ******************************************/
        void Z34_0000_dddd_dsp16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t dsp16 = GET_DSP16;
            addr_to_reg(dst, dsp16);
        }

        /******************************************
         lda     prd,rs(idx16)
         flags:  ------
         ******************************************/
        void Z34_ssN0_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t idx16 = GET_IDX16(OP1);
            if (get_segmented_mode())
            {
                RL(dst).op = RL(src).op;
            }
            else
            {
                RW(dst).op = RW(src).op;
            }
            add_to_addr_reg(dst, (uint16_t)idx16);
        }

        /******************************************
         ldrl    rrd,dsp16
         flags:  ------
         ******************************************/
        void Z35_0000_dddd_dsp16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t dsp16 = GET_DSP16;
            RL(dst).op = RDMEM_L(m_program, dsp16);
        }

        /******************************************
         ldl     rrd,rs(idx16)
         flags:  ------
         ******************************************/
        void Z35_ssN0_dddd_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t idx16 = GET_IDX16(OP1);
            RL(dst).op = RDBX_L(src, (uint16_t)idx16);
        }

        /******************************************
         bpt
         flags:  ------
         ******************************************/
        void Z36_0000_0000()
        {
            /* execute break point trap m_irq_req */
            m_irq_req |= Z8000_TRAP;
        }

        /******************************************
         rsvd36
         flags:  ------
         ******************************************/
        void Z36_imm8()
        {
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: rsvd36 {1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
        }

        /******************************************
         ldrl    dsp16,rrs
         flags:  ------
         ******************************************/
        void Z37_0000_ssss_dsp16()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint32_t dsp16 = GET_DSP16;
            WRMEM_L(m_program,  dsp16, RL(src).op);
        }

        /******************************************
         ldl     rd(idx16),rrs
         flags:  ------
         ******************************************/
        void Z37_ddN0_ssss_imm16()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t idx16 = GET_IDX16(OP1);
            WRBX_L(dst, (uint16_t)idx16, RL(src).op);
        }

        /******************************************
         rsvd38
         flags:  ------
         ******************************************/
        void Z38_imm8()
        {
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: rsvd38 ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
        }

        /******************************************
         ldps    @rs
         flags:  CZSVDH
         ******************************************/
        void Z39_ssN0_0000()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            uint16_t fcw;
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific space = src == SP ? m_stack : m_data;
            if (get_segmented_mode()) {
                uint32_t addr = addr_from_reg(src);
                fcw = RDMEM_W(space, addr + 2);
                set_pc(segmented_addr(RDMEM_L(space, addr + 4)));
            }
            else {
                fcw = RDMEM_W(space, RW(src).op);
                set_pc(RDMEM_W(space, (uint16_t)(RW(src).op + 2)));
            }
            if (((fcw ^ m_fcw) & F_SEG) != 0) osd_printf_debug("ldps 1 (0x{0}): changing from {1}segmented mode to {2}segmented mode\n", m_pc, (m_fcw & F_SEG) != 0 ? "non-" : "", (fcw & F_SEG) != 0 ? "" : "non-");
            CHANGE_FCW(fcw); /* check for user/system mode change */
        }

        /******************************************
         inib(r) @rd,@rs,ra
         flags:  ---V--
         ******************************************/
        void Z3A_ssss_0000_0000_aaaa_dddd_x000()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_B(dst, RDPORT_B( 0, RW(src).op));
            add_to_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         sinib   @rd,@rs,ra
         sinibr  @rd,@rs,ra
         flags:  ------
         ******************************************/
        void Z3A_ssss_0001_0000_aaaa_dddd_x000()
        {//@@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_B(dst, RDPORT_B( 1, RW(src).op));
            RW(dst).op++;
            RW(src).op++;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         outib   @rd,@rs,ra
         outibr  @rd,@rs,ra
         flags:  ---V--
         ******************************************/
        void Z3A_ssss_0010_0000_aaaa_dddd_x000()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRPORT_B( 0, RW(dst).op, RDIR_B(src));
            add_to_addr_reg(src, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         soutib  @rd,@rs,ra
         soutibr @rd,@rs,ra
         flags:  ------
         ******************************************/
        void Z3A_ssss_0011_0000_aaaa_dddd_x000()
        {//@@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRPORT_B( 1, RW(dst).op, (uint8_t)RDIR_W(src));
            RW(dst).op++;
            RW(src).op++;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         inb     rbd,imm16
         flags:  ------
         ******************************************/
        void Z3A_dddd_0100_imm16()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            RB(dst).op = RDPORT_B( 0, imm16);
        }

        /******************************************
         sinb    rbd,imm16
         flags:  ------
         ******************************************/
        void Z3A_dddd_0101_imm16()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            RB(dst).op = RDPORT_B( 1, imm16);
        }

        /******************************************
         outb    imm16,rbs
         flags:  ---V--
         ******************************************/
        void Z3A_ssss_0110_imm16()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            WRPORT_B( 0, imm16, RB(src).op);
        }

        /******************************************
         soutb   imm16,rbs
         flags:  ------
         ******************************************/
        void Z3A_ssss_0111_imm16()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            WRPORT_B( 1, imm16, RB(src).op);
        }

        /******************************************
         indb    @rd,@rs,rba
         indbr   @rd,@rs,rba
         flags:  ---V--
         ******************************************/
        void Z3A_ssss_1000_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_B(dst, RDPORT_B( 0, RW(src).op));
            RW(dst).op--;
            RW(src).op--;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         sindb   @rd,@rs,rba
         sindbr  @rd,@rs,rba
         flags:  ------
         ******************************************/
        void Z3A_ssss_1001_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_B(dst, RDPORT_B( 1, RW(src).op));
            RW(dst).op--;
            RW(src).op--;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         outdb   @rd,@rs,rba
         outdbr  @rd,@rs,rba
         flags:  ---V--
         ******************************************/
        void Z3A_ssss_1010_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRPORT_B( 0, RW(dst).op, RDIR_B(src));
            RW(dst).op--;
            RW(src).op--;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         soutdb  @rd,@rs,rba
         soutdbr @rd,@rs,rba
         flags:  ------
         ******************************************/
        void Z3A_ssss_1011_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRPORT_B( 1, RW(dst).op, RDIR_B(src));
            RW(dst).op--;
            RW(src).op--;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         ini     @rd,@rs,ra
         inir    @rd,@rs,ra
         flags:  ---V--
         ******************************************/
        void Z3B_ssss_0000_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_W(dst, RDPORT_W( 0, RW(src).op));
            RW(dst).op += 2;
            RW(src).op += 2;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         sini    @rd,@rs,ra
         sinir   @rd,@rs,ra
         flags:  ------
         ******************************************/
        void Z3B_ssss_0001_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_W(dst, RDPORT_W( 1, RW(src).op));
            RW(dst).op += 2;
            RW(src).op += 2;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         outi    @rd,@rs,ra
         outir   @rd,@rs,ra
         flags:  ---V--
         ******************************************/
        void Z3B_ssss_0010_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRPORT_W( 0, RW(dst).op, RDIR_W(src));
            RW(dst).op += 2;
            RW(src).op += 2;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         souti   @rd,@rs,ra
         soutir  @rd,@rs,ra
         flags:  ------
         ******************************************/
        void Z3B_ssss_0011_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRPORT_W( 1, RW(dst).op, RDIR_W(src));
            RW(dst).op += 2;
            RW(src).op += 2;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         in      rd,imm16
         flags:  ------
         ******************************************/
        void Z3B_dddd_0100_imm16()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            RW(dst).op = RDPORT_W( 0, imm16);
        }

        /******************************************
         sin     rd,imm16
         flags:  ------
         ******************************************/
        void Z3B_dddd_0101_imm16()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            RW(dst).op = RDPORT_W( 1, imm16);
        }

        /******************************************
         out     imm16,rs
         flags:  ---V--
         ******************************************/
        void Z3B_ssss_0110_imm16()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            WRPORT_W( 0, imm16, RW(src).op);
        }

        /******************************************
         sout    imm16,rbs
         flags:  ------
         ******************************************/
        void Z3B_ssss_0111_imm16()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            WRPORT_W( 1, imm16, RW(src).op);
        }

        /******************************************
         ind     @rd,@rs,ra
         indr    @rd,@rs,ra
         flags:  ---V--
         ******************************************/
        void Z3B_ssss_1000_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_W(dst, RDPORT_W( 0, RW(src).op));
            RW(dst).op -= 2;
            RW(src).op -= 2;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         sind    @rd,@rs,ra
         sindr   @rd,@rs,ra
         flags:  ------
         ******************************************/
        void Z3B_ssss_1001_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_W(dst, RDPORT_W( 1, RW(src).op));
            RW(dst).op -= 2;
            RW(src).op -= 2;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         outd    @rd,@rs,ra
         outdr   @rd,@rs,ra
         flags:  ---V--
         ******************************************/
        void Z3B_ssss_1010_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRPORT_W( 0, RW(dst).op, RDIR_W(src));
            RW(dst).op -= 2;
            RW(src).op -= 2;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         soutd   @rd,@rs,ra
         soutdr  @rd,@rs,ra
         flags:  ------
         ******************************************/
        void Z3B_ssss_1011_0000_aaaa_dddd_x000()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRPORT_W( 1, RW(dst).op, RDIR_W(src));
            RW(dst).op -= 2;
            RW(src).op -= 2;
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         inb     rbd,@rs
         flags:  ------
         ******************************************/
        void Z3C_ssss_dddd()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t dst = GET_DST(OP0,NIB3);
            RB(dst).op = RDPORT_B( 0, RW(src).op);
        }

        /******************************************
         in      rd,@rs
         flags:  ------
         ******************************************/
        void Z3D_ssss_dddd()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t dst = GET_DST(OP0,NIB3);
            RW(dst).op = RDPORT_W( 0, RW(src).op);
        }

        /******************************************
         outb    @rd,rbs
         flags:  ---V--
         ******************************************/
        void Z3E_dddd_ssss()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP0,NIB3);
            WRPORT_B( 0, RW(dst).op, RB(src).op);
        }

        /******************************************
         out     @rd,rs
         flags:  ---V--
         ******************************************/
        void Z3F_dddd_ssss()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP0,NIB3);
            WRPORT_W( 0, RW(dst).op, RW(src).op);
        }

        /******************************************
         addb    rbd,addr
         flags:  CZSVDH
         ******************************************/
        void Z40_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RB(dst).op = ADDB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         addb    rbd,addr(rs)
         flags:  CZSVDH
         ******************************************/
        void Z40_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RB(dst).op = ADDB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         add     rd,addr
         flags:  CZSV--
         ******************************************/
        void Z41_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RW(dst).op = ADDW(RW(dst).op, RDMEM_W(m_data, addr)); /* EHC */
        }

        /******************************************
         add     rd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z41_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RW(dst).op = ADDW(RW(dst).op, RDMEM_W(m_data, addr));    /* ASG */
        }

        /******************************************
         subb    rbd,addr
         flags:  CZSVDH
         ******************************************/
        void Z42_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RB(dst).op = SUBB(RB(dst).op, RDMEM_B(m_data, addr)); /* EHC */
        }

        /******************************************
         subb    rbd,addr(rs)
         flags:  CZSVDH
         ******************************************/
        void Z42_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RB(dst).op = SUBB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         sub     rd,addr
         flags:  CZSV--
         ******************************************/
        void Z43_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RW(dst).op = SUBW(RW(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         sub     rd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z43_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RW(dst).op = SUBW(RW(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         orb     rbd,addr
         flags:  CZSP--
         ******************************************/
        void Z44_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RB(dst).op = ORB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         orb     rbd,addr(rs)
         flags:  CZSP--
         ******************************************/
        void Z44_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RB(dst).op = ORB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         or      rd,addr
         flags:  CZS---
         ******************************************/
        void Z45_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RW(dst).op = ORW(RW(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         or      rd,addr(rs)
         flags:  CZS---
         ******************************************/
        void Z45_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RW(dst).op = ORW(RW(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         andb    rbd,addr
         flags:  -ZSP--
         ******************************************/
        void Z46_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RB(dst).op = ANDB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         andb    rbd,addr(rs)
         flags:  -ZSP--
         ******************************************/
        void Z46_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RB(dst).op = ANDB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         and     rd,addr
         flags:  -ZS---
         ******************************************/
        void Z47_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RW(dst).op = ANDW(RW(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         and     rd,addr(rs)
         flags:  -ZS---
         ******************************************/
        void Z47_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RW(dst).op = ANDW(RW(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         xorb    rbd,addr
         flags:  -ZSP--
         ******************************************/
        void Z48_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RB(dst).op = XORB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         xorb    rbd,addr(rs)
         flags:  -ZSP--
         ******************************************/
        void Z48_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RB(dst).op = XORB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         xor     rd,addr
         flags:  -ZS---
         ******************************************/
        void Z49_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RW(dst).op = XORW(RW(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         xor     rd,addr(rs)
         flags:  -ZS---
         ******************************************/
        void Z49_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RW(dst).op = XORW(RW(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         cpb     rbd,addr
         flags:  CZSV--
         ******************************************/
        void Z4A_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            CPB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         cpb     rbd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z4A_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            CPB(RB(dst).op, RDMEM_B(m_data, addr));
        }

        /******************************************
         cp      rd,addr
         flags:  CZSV--
         ******************************************/
        void Z4B_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            CPW(RW(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         cp      rd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z4B_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            CPW(RW(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         comb    addr
         flags:  -ZSP--
         ******************************************/
        void Z4C_0000_0000_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_B(m_data,  addr, COMB((uint8_t)RDMEM_W(m_data, addr)));
        }

        /******************************************
         cpb     addr,imm8
         flags:  CZSV--
         ******************************************/
        void Z4C_0000_0001_addr_imm8()
        {
            uint32_t addr = GET_ADDR(OP1);
            uint8_t imm8 = GET_IMM8(OP2);
            CPB(RDMEM_B(m_data, addr), imm8);
        }

        /******************************************
         negb    addr
         flags:  CZSV--
         ******************************************/
        void Z4C_0000_0010_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_B(m_data,  addr, NEGB(RDMEM_B(m_data, addr)));
        }

        /******************************************
         testb   addr
         flags:  -ZSP--
         ******************************************/
        void Z4C_0000_0100_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            TESTB(RDMEM_B(m_data, addr));
        }

        /******************************************
         ldb     addr,imm8
         flags:  ------
         ******************************************/
        void Z4C_0000_0101_addr_imm8()
        {
            uint32_t addr = GET_ADDR(OP1);
            uint8_t imm8 = GET_IMM8(OP2);
            WRMEM_B(m_data,  addr, imm8);
        }

        /******************************************
         tsetb   addr
         flags:  --S---
         ******************************************/
        void Z4C_0000_0110_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            if ((RDMEM_B(m_data, addr) & S08) != 0) SET_S(); else CLR_S();
            WRMEM_B(m_data, addr, 0xff);
        }

        /******************************************
         clrb    addr
         flags:  ------
         ******************************************/
        void Z4C_0000_1000_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_B(m_data,  addr, 0);
        }

        /******************************************
         comb    addr(rd)
         flags:  -ZSP--
         ******************************************/
        void Z4C_ddN0_0000_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_B(m_data,  addr, COMB(RDMEM_B(m_data, addr)));
        }

        /******************************************
         cpb     addr(rd),imm8
         flags:  CZSV--
         ******************************************/
        void Z4C_ddN0_0001_addr_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            uint8_t imm8 = GET_IMM8(OP2);
            addr = addr_add(addr, RW(dst).op);
            CPB(RDMEM_B(m_data, addr), imm8);
        }

        /******************************************
         negb    addr(rd)
         flags:  CZSV--
         ******************************************/
        void Z4C_ddN0_0010_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_B(m_data, addr, NEGB(RDMEM_B(m_data, addr)));
        }

        /******************************************
         testb   addr(rd)
         flags:  -ZSP--
         ******************************************/
        void Z4C_ddN0_0100_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            TESTB(RDMEM_B(m_data, addr));
        }

        /******************************************
         ldb     addr(rd),imm8
         flags:  ------
         ******************************************/
        void Z4C_ddN0_0101_addr_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            uint8_t imm8 = GET_IMM8(OP2);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_B(m_data, addr, imm8);
        }

        /******************************************
         tsetb   addr(rd)
         flags:  --S---
         ******************************************/
        void Z4C_ddN0_0110_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            if ((RDMEM_B(m_data, addr) & S08) != 0) SET_S(); else CLR_S();
            WRMEM_B(m_data, addr, 0xff);
        }

        /******************************************
         clrb    addr(rd)
         flags:  ------
         ******************************************/
        void Z4C_ddN0_1000_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_B(m_data, addr, 0);
        }

        /******************************************
         com     addr
         flags:  -ZS---
         ******************************************/
        void Z4D_0000_0000_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_W(m_data,  addr, COMW(RDMEM_W(m_data, addr)));
        }

        /******************************************
         cp      addr,imm16
         flags:  CZSV--
         ******************************************/
        void Z4D_0000_0001_addr_imm16()
        {
            uint32_t addr = GET_ADDR(OP1);
            uint16_t imm16 = GET_IMM16(OP2);
            CPW(RDMEM_W(m_data, addr), imm16);
        }

        /******************************************
         neg     addr
         flags:  CZSV--
         ******************************************/
        void Z4D_0000_0010_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_W(m_data,  addr, NEGW(RDMEM_W(m_data, addr)));
        }

        /******************************************
         test    addr
         flags:  ------
         ******************************************/
        void Z4D_0000_0100_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            TESTW(RDMEM_W(m_data, addr));
        }

        /******************************************
         ld      addr,imm16
         flags:  ------
         ******************************************/
        void Z4D_0000_0101_addr_imm16()
        {
            uint32_t addr = GET_ADDR(OP1);
            uint16_t imm16 = GET_IMM16(OP2);
            WRMEM_W(m_data,  addr, imm16);
        }

        /******************************************
         tset    addr
         flags:  --S---
         ******************************************/
        void Z4D_0000_0110_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            if ((RDMEM_W(m_data, addr) & S16) != 0) SET_S(); else CLR_S();
            WRMEM_W(m_data, addr, 0xffff);
        }

        /******************************************
         clr     addr
         flags:  ------
         ******************************************/
        void Z4D_0000_1000_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_W(m_data,  addr, 0);
        }

        /******************************************
         com     addr(rd)
         flags:  -ZS---
         ******************************************/
        void Z4D_ddN0_0000_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_W(m_data, addr, COMW(RDMEM_W(m_data, addr)));
        }

        /******************************************
         cp      addr(rd),imm16
         flags:  CZSV--
         ******************************************/
        void Z4D_ddN0_0001_addr_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            uint16_t imm16 = GET_IMM16(OP2);
            addr = addr_add(addr, RW(dst).op);
            CPW(RDMEM_W(m_data, addr), imm16);
        }

        /******************************************
         neg     addr(rd)
         flags:  CZSV--
         ******************************************/
        void Z4D_ddN0_0010_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_W(m_data, addr, NEGW(RDMEM_W(m_data, addr)));
        }

        /******************************************
         test    addr(rd)
         flags:  ------
         ******************************************/
        void Z4D_ddN0_0100_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            TESTW(RDMEM_W(m_data, addr));
        }

        /******************************************
         ld      addr(rd),imm16
         flags:  ------
         ******************************************/
        void Z4D_ddN0_0101_addr_imm16()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            uint16_t imm16 = GET_IMM16(OP2);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_W(m_data, addr, imm16);
        }

        /******************************************
         tset    addr(rd)
         flags:  --S---
         ******************************************/
        void Z4D_ddN0_0110_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            if ((RDMEM_W(m_data, addr) & S16) != 0) SET_S(); else CLR_S();
            WRMEM_W(m_data, addr, 0xffff);
        }

        /******************************************
         clr     addr(rd)
         flags:  ------
         ******************************************/
        void Z4D_ddN0_1000_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_W(m_data, addr, 0);
        }

        /******************************************
         ldb     addr(rd),rbs
         flags:  ------
         ******************************************/
        void Z4E_ddN0_ssN0_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_B(m_data, addr, RB(src).op);
        }

        /******************************************
         cpl     rrd,addr
         flags:  CZSV--
         ******************************************/
        void Z50_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            CPL(RL(dst).op, RDMEM_L(m_data, addr));
        }

        /******************************************
         cpl     rrd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z50_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            CPL(RL(dst).op, RDMEM_L(m_data, addr));
        }

        /******************************************
         pushl   @rd,addr
         flags:  ------
         ******************************************/
        void Z51_ddN0_0000_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            PUSHL(dst, RDMEM_L(m_data, addr));
        }

        /******************************************
         pushl   @rd,addr(rs)
         flags:  ------
         ******************************************/
        void Z51_ddN0_ssN0_addr()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            PUSHL(dst, RDMEM_L(m_data, addr));
        }

        /******************************************
         subl    rrd,addr
         flags:  CZSV--
         ******************************************/
        void Z52_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RL(dst).op = SUBL(RL(dst).op, RDMEM_L(m_data, addr));
        }

        /******************************************
         subl    rrd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z52_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RL(dst).op = SUBL(RL(dst).op, RDMEM_L(m_data, addr));
        }

        /******************************************
         push    @rd,addr
         flags:  ------
         ******************************************/
        void Z53_ddN0_0000_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            PUSHW(dst, RDMEM_W(m_data, addr));
        }

        /******************************************
         push    @rd,addr(rs)
         flags:  ------
         ******************************************/
        void Z53_ddN0_ssN0_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            PUSHW(dst, RDMEM_W(m_data, addr));
        }

        /******************************************
         ldl     rrd,addr
         flags:  ------
         ******************************************/
        void Z54_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RL(dst).op = RDMEM_L(m_data, addr);
        }

        /******************************************
         ldl     rrd,addr(rs)
         flags:  ------
         ******************************************/
        void Z54_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RL(dst).op = RDMEM_L(m_data, addr);
        }

        /******************************************
         popl    addr,@rs
         flags:  ------
         ******************************************/
        void Z55_ssN0_0000_addr()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_L(m_data, addr, POPL(src));
        }

        /******************************************
         popl    addr(rd),@rs
         flags:  ------
         ******************************************/
        void Z55_ssN0_ddN0_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_L(m_data, addr, POPL(src));
        }

        /******************************************
         addl    rrd,addr
         flags:  CZSV--
         ******************************************/
        void Z56_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RL(dst).op = ADDL(RL(dst).op, RDMEM_L(m_data, addr));
        }

        /******************************************
         addl    rrd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z56_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RL(dst).op = ADDL(RL(dst).op, RDMEM_L(m_data, addr));
        }

        /******************************************
         pop     addr,@rs
         flags:  ------
         ******************************************/
        void Z57_ssN0_0000_addr()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_W(m_data, addr, POPW(src));
        }

        /******************************************
         pop     addr(rd),@rs
         flags:  ------
         ******************************************/
        void Z57_ssN0_ddN0_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_W(m_data, addr, POPW(src));
        }

        /******************************************
         multl   rqd,addr
         flags:  CZSV--
         ******************************************/
        void Z58_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RQ(dst).op = MULTL((uint32_t)RQ(dst).op, RDMEM_L(m_data, addr));
        }

        /******************************************
         multl   rqd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z58_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RQ(dst).op = MULTL((uint32_t)RQ(dst).op, RDMEM_L(m_data, addr));
        }

        /******************************************
         mult    rrd,addr
         flags:  CZSV--
         ******************************************/
        void Z59_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RL(dst).op = MULTW((uint16_t)RL(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         mult    rrd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z59_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RL(dst).op = MULTW((uint16_t)RL(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         divl    rqd,addr
         flags:  CZSV--
         ******************************************/
        void Z5A_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RQ(dst).op = DIVL(RQ(dst).op, RDMEM_L(m_data, addr));
        }

        /******************************************
         divl    rqd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z5A_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RQ(dst).op = DIVL(RQ(dst).op, RDMEM_L(m_data, addr));
        }

        /******************************************
         div     rrd,addr
         flags:  CZSV--
         ******************************************/
        void Z5B_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RL(dst).op = DIVW(RL(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         div     rrd,addr(rs)
         flags:  CZSV--
         ******************************************/
        void Z5B_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RL(dst).op = DIVW(RL(dst).op, RDMEM_W(m_data, addr));
        }

        /******************************************
         ldm     rd,addr,n
         flags:  ------
         ******************************************/
        void Z5C_0000_0001_0000_dddd_0000_nmin1_addr()
        {
            uint8_t dst = GET_DST(OP1,NIB1);
            int8_t cnt = GET_CNT(OP1,NIB3);
            uint32_t addr = GET_ADDR(OP2);
            while (cnt-- >= 0)
            {
                RW(dst).op = RDMEM_W(m_data, addr);
                dst = (uint8_t)((dst + 1) & 15);
                addr = addr_add (addr, 2);
            }
        }

        /******************************************
         testl   addr
         flags:  -ZS---
         ******************************************/
        void Z5C_0000_1000_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            TESTL(RDMEM_L(m_data, addr));
        }

        /******************************************
         ldm     addr,rs,n
         flags:  ------
         ******************************************/
        void Z5C_0000_1001_0000_ssss_0000_nmin1_addr()
        {
            uint8_t src = GET_SRC(OP1,NIB1);
            int8_t cnt = GET_CNT(OP1,NIB3);
            uint32_t addr = GET_ADDR(OP2);
            while (cnt-- >= 0)
            {
                WRMEM_W(m_data, addr, RW(src).op);
                src = (uint8_t)((src + 1) & 15);
                addr = addr_add (addr, 2);
            }
        }

        /******************************************
         testl   addr(rd)
         flags:  -ZS---
         ******************************************/
        void Z5C_ddN0_1000_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            TESTL(RDMEM_L(m_data, addr));
        }

        /******************************************
         ldm     addr(rd),rs,n
         flags:  ------
         ******************************************/
        void Z5C_ddN0_1001_0000_ssN0_0000_nmin1_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB1);
            int8_t cnt = GET_CNT(OP1,NIB3);
            uint32_t addr = GET_ADDR(OP2);
            addr = addr_add(addr, RW(dst).op);
            while (cnt-- >= 0)
            {
                WRMEM_W(m_data, addr, RW(src).op);
                src = (uint8_t)((src + 1) & 15);
                addr = addr_add(addr, 2);
            }
        }

        /******************************************
         ldm     rd,addr(rs),n
         flags:  ------
         ******************************************/
        void Z5C_ssN0_0001_0000_dddd_0000_nmin1_addr()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t dst = GET_DST(OP1,NIB1);
            int8_t cnt = GET_CNT(OP1,NIB3);
            uint32_t addr = GET_ADDR(OP2);
            addr = addr_add(addr, RW(src).op);
            while (cnt-- >= 0)
            {
                RW(dst).op = RDMEM_W(m_data, addr);
                dst = (uint8_t)((dst + 1) & 15);
                addr = addr_add(addr, 2);
            }
        }

        /******************************************
         ldl     addr,rrs
         flags:  ------
         ******************************************/
        void Z5D_0000_ssss_addr()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_L(m_data, addr, RL(src).op);
        }

        /******************************************
         ldl     addr(rd),rrs
         flags:  ------
         ******************************************/
        void Z5D_ddN0_ssss_addr()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_L(m_data, addr, RL(src).op);
        }

        /******************************************
         jp      cc,addr
         flags:  ------
         ******************************************/
        void Z5E_0000_cccc_addr()
        {
            uint8_t cc = GET_CCC(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            switch (cc)
            {
                case  0: if (CC0) set_pc(addr); break;
                case  1: if (CC1) set_pc(addr); break;
                case  2: if (CC2) set_pc(addr); break;
                case  3: if (CC3) set_pc(addr); break;
                case  4: if (CC4) set_pc(addr); break;
                case  5: if (CC5) set_pc(addr); break;
                case  6: if (CC6) set_pc(addr); break;
                case  7: if (CC7) set_pc(addr); break;
                case  8: if (CC8) set_pc(addr); break;
                case  9: if (CC9) set_pc(addr); break;
                case 10: if (CCA) set_pc(addr); break;
                case 11: if (CCB) set_pc(addr); break;
                case 12: if (CCC) set_pc(addr); break;
                case 13: if (CCD) set_pc(addr); break;
                case 14: if (CCE) set_pc(addr); break;
                case 15: if (CCF) set_pc(addr); break;
            }
        }

        /******************************************
         jp      cc,addr(rd)
         flags:  ------
         ******************************************/
        void Z5E_ddN0_cccc_addr()
        {
            uint8_t cc = GET_CCC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            switch (cc)
            {
                case  0: if (CC0) set_pc(addr); break;
                case  1: if (CC1) set_pc(addr); break;
                case  2: if (CC2) set_pc(addr); break;
                case  3: if (CC3) set_pc(addr); break;
                case  4: if (CC4) set_pc(addr); break;
                case  5: if (CC5) set_pc(addr); break;
                case  6: if (CC6) set_pc(addr); break;
                case  7: if (CC7) set_pc(addr); break;
                case  8: if (CC8) set_pc(addr); break;
                case  9: if (CC9) set_pc(addr); break;
                case 10: if (CCA) set_pc(addr); break;
                case 11: if (CCB) set_pc(addr); break;
                case 12: if (CCC) set_pc(addr); break;
                case 13: if (CCD) set_pc(addr); break;
                case 14: if (CCE) set_pc(addr); break;
                case 15: if (CCF) set_pc(addr); break;
            }
        }

        /******************************************
         call    addr
         flags:  ------
         ******************************************/
        void Z5F_0000_0000_addr()
        {
            uint32_t addr = GET_ADDR(OP1);
            if (get_segmented_mode())
                PUSHL(SP, make_segmented_addr(m_pc));
            else
                PUSHW(SP, (uint16_t)m_pc);
            set_pc(addr);
        }

        /******************************************
         call    addr(rd)
         flags:  ------
         ******************************************/
        void Z5F_ddN0_0000_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            if (get_segmented_mode())
                PUSHL(SP, make_segmented_addr(m_pc));
            else
                PUSHW(SP, (uint16_t)m_pc);
            addr = addr_add(addr, RW(dst).op);
            set_pc(addr);
        }

        /******************************************
         ldb     rbd,addr
         flags:  ------
         ******************************************/
        void Z60_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RB(dst).op = RDMEM_B(m_data, addr);
        }

        /******************************************
         ldb     rbd,addr(rs)
         flags:  ------
         ******************************************/
        void Z60_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RB(dst).op = RDMEM_B(m_data, addr);
        }

        /******************************************
         ld      rd,addr
         flags:  ------
         ******************************************/
        void Z61_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            RW(dst).op = RDMEM_W(m_data, addr);
        }

        /******************************************
         ld      rd,addr(rs)
         flags:  ------
         ******************************************/
        void Z61_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(src).op);
            RW(dst).op = RDMEM_W(m_data, addr);
        }

        /******************************************
         resb    addr,imm4
         flags:  ------
         ******************************************/
        void Z62_0000_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_B(m_data, addr, (uint8_t)(RDMEM_B(m_data, addr) & ~bit));
        }

        /******************************************
         resb    addr(rd),imm4
         flags:  ------
         ******************************************/
        void Z62_ddN0_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_B(m_data, addr, (uint8_t)(RDMEM_B(m_data, addr) & ~bit));
        }

        /******************************************
         res     addr,imm4
         flags:  ------
         ******************************************/
        void Z63_0000_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_W(m_data, addr, (uint16_t)(RDMEM_W(m_data, addr) & ~bit));
        }

        /******************************************
         res     addr(rd),imm4
         flags:  ------
         ******************************************/
        void Z63_ddN0_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_W(m_data, addr, (uint16_t)(RDMEM_W(m_data, addr) & ~bit));
        }

        /******************************************
         setb    addr,imm4
         flags:  ------
         ******************************************/
        void Z64_0000_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_B(m_data, addr, (uint8_t)(RDMEM_B(m_data, addr) | bit));
        }

        /******************************************
         setb    addr(rd),imm4
         flags:  ------
         ******************************************/
        void Z64_ddN0_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_B(m_data, addr, (uint8_t)(RDMEM_B(m_data, addr) | bit));
        }

        /******************************************
         set     addr,imm4
         flags:  ------
         ******************************************/
        void Z65_0000_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_W(m_data, addr, (uint16_t)(RDMEM_W(m_data, addr) | bit));
        }

        /******************************************
         set     addr(rd),imm4
         flags:  ------
         ******************************************/
        void Z65_ddN0_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_W(m_data, addr, (uint16_t)(RDMEM_W(m_data, addr) | bit));
        }

        /******************************************
         bitb    addr,imm4
         flags:  -Z----
         ******************************************/
        void Z66_0000_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint32_t addr = GET_ADDR(OP1);
            if ((RDMEM_B(m_data, addr) & bit) != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         bitb    addr(rd),imm4
         flags:  -Z----
         ******************************************/
        void Z66_ddN0_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            if ((RDMEM_B(m_data, addr) & bit) != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         bit     addr,imm4
         flags:  -Z----
         ******************************************/
        void Z67_0000_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint32_t addr = GET_ADDR(OP1);
            if ((RDMEM_W(m_data, addr) & bit) != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         bit     addr(rd),imm4
         flags:  -Z----
         ******************************************/
        void Z67_ddN0_imm4_addr()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            if ((RDMEM_W(m_data, addr) & bit) != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         incb    addr,imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z68_0000_imm4m1_addr()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_B(m_data, addr, INCB(RDMEM_B(m_data, addr), i4p1));
        }

        /******************************************
         incb    addr(rd),imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z68_ddN0_imm4m1_addr()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_B(m_data, addr, INCB(RDMEM_B(m_data, addr), i4p1));
        }

        /******************************************
         inc     addr,imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z69_0000_imm4m1_addr()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_W(m_data, addr, INCW(RDMEM_W(m_data, addr), i4p1));
        }

        /******************************************
         inc     addr(rd),imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z69_ddN0_imm4m1_addr()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_W(m_data, addr, INCW(RDMEM_W(m_data, addr), i4p1));
        }

        /******************************************
         decb    addr,imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z6A_0000_imm4m1_addr()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_B(m_data, addr, DECB(RDMEM_B(m_data, addr), i4p1));
        }

        /******************************************
         decb    addr(rd),imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z6A_ddN0_imm4m1_addr()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_B(m_data, addr, DECB(RDMEM_B(m_data, addr), i4p1));
        }

        /******************************************
         dec     addr,imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z6B_0000_imm4m1_addr()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_W(m_data, addr, DECW(RDMEM_W(m_data, addr), i4p1));
        }

        /******************************************
         dec     addr(rd),imm4m1
         flags:  -ZSV--
         ******************************************/
        void Z6B_ddN0_imm4m1_addr()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_W(m_data, addr, DECW(RDMEM_W(m_data, addr), i4p1));
        }

        /******************************************
         exb     rbd,addr
         flags:  ------
         ******************************************/
        void Z6C_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            uint8_t tmp = RDMEM_B(m_data, addr);
            WRMEM_B(m_data, addr, RB(dst).op);
            RB(dst).op = tmp;
        }

        /******************************************
         exb     rbd,addr(rs)
         flags:  ------
         ******************************************/
        void Z6C_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            uint8_t tmp;
            addr = addr_add(addr, RW(src).op);
            tmp = RDMEM_B(m_data, addr);
            WRMEM_B(m_data, addr, RB(dst).op);
            RB(dst).op = tmp;
        }

        /******************************************
         ex      rd,addr
         flags:  ------
         ******************************************/
        void Z6D_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            uint16_t tmp = RDMEM_W(m_data, addr);
            WRMEM_W(m_data, addr, RW(dst).op);
            RW(dst).op = tmp;
        }

        /******************************************
         ex      rd,addr(rs)
         flags:  ------
         ******************************************/
        void Z6D_ssN0_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            uint16_t tmp;
            addr = addr_add(addr, RW(src).op);
            tmp = RDMEM_W(m_data, addr);
            WRMEM_W(m_data, addr, RW(dst).op);
            RW(dst).op = tmp;
        }

        /******************************************
         ldb     addr,rbs
         flags:  ------
         ******************************************/
        void Z6E_0000_ssss_addr()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_B(m_data,  addr, RB(src).op);
        }

        /******************************************
         ldb     addr(rd),rbs
         flags:  ------
         ******************************************/
        void Z6E_ddN0_ssss_addr()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_B(m_data, addr, RB(src).op);
        }

        /******************************************
         ld      addr,rs
         flags:  ------
         ******************************************/
        void Z6F_0000_ssss_addr()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint32_t addr = GET_ADDR(OP1);
            WRMEM_W(m_data,  addr, RW(src).op);
        }

        /******************************************
         ld      addr(rd),rs
         flags:  ------
         ******************************************/
        void Z6F_ddN0_ssss_addr()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            addr = addr_add(addr, RW(dst).op);
            WRMEM_W(m_data, addr, RW(src).op);
        }

        /******************************************
         ldb     rbd,rs(rx)
         flags:  ------
         ******************************************/
        void Z70_ssN0_dddd_0000_xxxx_0000_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t idx = GET_IDX(OP1,NIB1);
            RB(dst).op = RDBX_B(src, RW(idx).op);
        }

        /******************************************
         ld      rd,rs(rx)
         flags:  ------
         ******************************************/
        void Z71_ssN0_dddd_0000_xxxx_0000_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t idx = GET_IDX(OP1,NIB1);
            RW(dst).op = RDBX_W(src, RW(idx).op);
        }

        /******************************************
         ldb     rd(rx),rbs
         flags:  ------
         ******************************************/
        void Z72_ddN0_ssss_0000_xxxx_0000_0000()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t idx = GET_IDX(OP1,NIB1);
            WRBX_B(dst, RW(idx).op, RB(src).op);
        }

        /******************************************
         ld      rd(rx),rs
         flags:  ------
         ******************************************/
        void Z73_ddN0_ssss_0000_xxxx_0000_0000()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t idx = GET_IDX(OP1,NIB1);
            WRBX_W(dst, RW(idx).op, RW(src).op);
        }

        /******************************************
         lda     prd,rs(rx)
         flags:  ------
         ******************************************/
        void Z74_ssN0_dddd_0000_xxxx_0000_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t idx = GET_IDX(OP1,NIB1);
            if (get_segmented_mode())
            {
                RL(dst).op = RL(src).op;
            }
            else
            {
                RW(dst).op = RW(src).op;
            }
            add_to_addr_reg(dst, RW(idx).op);
        }

        /******************************************
         ldl     rrd,rs(rx)
         flags:  ------
         ******************************************/
        void Z75_ssN0_dddd_0000_xxxx_0000_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t idx = GET_IDX(OP1,NIB1);
            RL(dst).op = RDBX_L(src, RW(idx).op);
        }

        /******************************************
         lda     prd,addr
         flags:  ------
         ******************************************/
        void Z76_0000_dddd_addr()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint32_t addr = GET_ADDR_RAW(OP1);
            if (get_segmented_mode())
            {
                RL(dst).op = addr;
            }
            else
            {
                RW(dst).op = (uint16_t)addr;
            }
        }

        /******************************************
         lda     prd,addr(rs)
         flags:  ------
         ******************************************/
        void Z76_ssN0_dddd_addr()
        {//@@@
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR_RAW(OP1);
            uint16_t temp = RW(src).op;  // store src in case dst == src
            if (get_segmented_mode())
            {
                RL(dst).op = addr;
            }
            else
            {
                RW(dst).op = (uint16_t)addr;
            }
            add_to_addr_reg(dst, temp);
        }

        /******************************************
         ldl     rd(rx),rrs
         flags:  ------
         ******************************************/
        void Z77_ddN0_ssss_0000_xxxx_0000_0000()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t idx = GET_IDX(OP1,NIB1);
            WRBX_L(dst, RW(idx).op, RL(src).op);
        }

        /******************************************
         rsvd78
         flags:  ------
         ******************************************/
        void Z78_imm8()
        {
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: rsvd78 ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
        }

        /******************************************
         ldps    addr
         flags:  CZSVDH
         ******************************************/
        void Z79_0000_0000_addr()
        {
            CHECK_PRIVILEGED_INSTR();
            uint32_t addr = GET_ADDR(OP1);
            uint16_t fcw;
            if (get_segmented_mode())
            {
                fcw = RDMEM_W(m_data, addr + 2);
                set_pc(segmented_addr(RDMEM_L(m_data, addr + 4)));
            }
            else
            {
                fcw = RDMEM_W(m_data, addr);
                set_pc(RDMEM_W(m_data, (uint16_t)(addr + 2)));
            }
            CHANGE_FCW(fcw); /* check for user/system mode change */
        }

        /******************************************
         ldps    addr(rs)
         flags:  CZSVDH
         ******************************************/
        void Z79_ssN0_0000_addr()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t src = GET_SRC(OP0,NIB2);
            uint32_t addr = GET_ADDR(OP1);
            uint16_t fcw;
            addr = addr_add(addr, RW(src).op);
            if (get_segmented_mode())
            {
                fcw = RDMEM_W(m_data, addr + 2);
                set_pc(segmented_addr(RDMEM_L(m_data, addr + 4)));
            }
            else
            {
                fcw = RDMEM_W(m_data, addr);
                m_pc = RDMEM_W(m_data, (uint16_t)(addr + 2));
            }
            if (((fcw ^ m_fcw) & F_SEG) != 0) osd_printf_debug("ldps 3 (0x{0}): changing from {1}segmented mode to {2}segmented mode\n", m_pc, (fcw & F_SEG) != 0 ? "non-" : "", (fcw & F_SEG) != 0 ? "" : "non-");
            CHANGE_FCW(fcw); /* check for user/system mode change */
        }

        /******************************************
         halt
         flags:  ------
         ******************************************/
        void Z7A_0000_0000()
        {
            CHECK_PRIVILEGED_INSTR();
            m_halt = true;
            if (m_icount.i > 0) m_icount.i = 0;
        }

        /******************************************
         iret
         flags:  CZSVDH
         ******************************************/
        void Z7B_0000_0000()
        {
            uint16_t tag, fcw;
            CHECK_PRIVILEGED_INSTR();
            tag = POPW(SP);   /* get type tag */
            fcw = POPW(SP);   /* get m_fcw  */
            if (get_segmented_mode())
                set_pc(segmented_addr(POPL(SP)));
            else
                m_pc    = POPW(SP);   /* get m_pc   */
            CHANGE_FCW(fcw);       /* check for user/system mode change */
            LOG("Z8K IRET tag ${0}, fcw ${1}, pc ${2}\n", tag, fcw, m_pc);
        }

        /******************************************
         mset
         flags:  ------
         ******************************************/
        void Z7B_0000_1000()
        {
            CHECK_PRIVILEGED_INSTR();
            /* set mu-0 line */
        }

        /******************************************
         mres
         flags:  ------
         ******************************************/
        void Z7B_0000_1001()
        {
            CHECK_PRIVILEGED_INSTR();
            /* reset mu-0 line */
        }

        /******************************************
         mbit
         flags:  CZS---
         ******************************************/
        void Z7B_0000_1010()
        {
            CHECK_PRIVILEGED_INSTR();
            /* test mu-I line */
        }

        /******************************************
         mreq    rd
         flags:  -ZS---
         ******************************************/
        void Z7B_dddd_1101()
        {
            CHECK_PRIVILEGED_INSTR();
            /* test mu-I line, invert cascade to mu-0  */
            if (m_mi != 0)
            {
                CLR_Z();
                CLR_S();
                m_mo_out.op_s32(CLEAR_LINE);
                return;
            }
            SET_Z();
            m_mo_out.op_s32(ASSERT_LINE);
            if (m_mi != 0)
            {
                SET_S();
            }
            else
            {
                CLR_S();
                m_mo_out.op_s32(CLEAR_LINE);
            }
        }

        /******************************************
         di      i2
         flags:  ------
         ******************************************/
        void Z7C_0000_00ii()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t imm2 = GET_IMM2(OP0,NIB3);
            uint16_t fcw = m_fcw;
            fcw &= (uint16_t)((imm2 << 11) | 0xe7ff);
            CHANGE_FCW(fcw);
        }

        /******************************************
         ei      i2
         flags:  ------
         ******************************************/
        void Z7C_0000_01ii()
        {
            CHECK_PRIVILEGED_INSTR();
            uint8_t imm2 = GET_IMM2(OP0,NIB3);
            uint16_t fcw = m_fcw;
            fcw |= (uint16_t)(((~imm2) << 11) & 0x1800);
            CHANGE_FCW(fcw);
        }

        /******************************************
         ldctl   rd,ctrl
         flags:  ------
         ******************************************/
        void Z7D_dddd_0ccc()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t imm3 = GET_IMM3(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            switch (imm3)
            {
                case 2:
                    RW(dst).op = m_fcw;
                    break;
                case 3:
                    RW(dst).op = m_refresh;
                    break;
                case 4:
                    RW(dst).op = m_psapseg;
                    break;
                case 5:
                    RW(dst).op = m_psapoff;
                    break;
                case 6:
                    RW(dst).op = m_nspseg;
                    break;
                case 7:
                    RW(dst).op = m_nspoff;
                    break;
                default:
                    LOG("Z8K LDCTL R{0},{1}\n", dst, imm3);
                    break;
            }
        }

        /******************************************
         ldctl   ctrl,rs
         flags:  ------
         ******************************************/
        void Z7D_ssss_1ccc()
        {//@@@
            CHECK_PRIVILEGED_INSTR();
            uint8_t imm3 = GET_IMM3(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            switch (imm3)
            {
                case 2:
                    {
                        uint16_t fcw;
                        fcw = RW(src).op;
                        CHANGE_FCW(fcw); /* check for user/system mode change */
                    }
                    break;
                case 3:
                    m_refresh = RW(src).op;
                    break;
                case 4:
                    m_psapseg = RW(src).op;
                    break;
                case 5:
                    m_psapoff = RW(src).op;
                    break;
                case 6:
                    m_nspseg = RW(src).op;
                    break;
                case 7:
                    m_nspoff = RW(src).op;
                    break;
                default:
                    LOG("Z8K LDCTL {0},R{1}\n", imm3, src);
                    break;
            }
        }

        /******************************************
         rsvd7e
         flags:  ------
         ******************************************/
        void Z7E_imm8()
        {
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: rsvd7e ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
        }

        /******************************************
         sc      imm8
         flags:  CZSVDH
         ******************************************/
        void Z7F_imm8()
        {
            uint8_t imm8 = GET_IMM8(0);
            /* execute system call via IRQ */
            m_irq_req |= Z8000_SYSCALL;
            //(void)imm8;
        }

        /******************************************
         addb    rbd,rbs
         flags:  CZSVDH
         ******************************************/
        void Z80_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = ADDB(RB(dst).op, RB(src).op);
        }

        /******************************************
         add     rd,rs
         flags:  CZSV--
         ******************************************/
        void Z81_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = ADDW(RW(dst).op, RW(src).op);
        }

        /******************************************
         subb    rbd,rbs
         flags:  CZSVDH
         ******************************************/
        void Z82_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = SUBB(RB(dst).op, RB(src).op);
        }

        /******************************************
         sub     rd,rs
         flags:  CZSV--
         ******************************************/
        void Z83_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = SUBW(RW(dst).op, RW(src).op);
        }

        /******************************************
         orb     rbd,rbs
         flags:  CZSP--
         ******************************************/
        void Z84_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = ORB(RB(dst).op, RB(src).op);
        }

        /******************************************
         or      rd,rs
         flags:  CZS---
         ******************************************/
        void Z85_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = ORW(RW(dst).op, RW(src).op);
        }

        /******************************************
         andb    rbd,rbs
         flags:  -ZSP--
         ******************************************/
        void Z86_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = ANDB(RB(dst).op, RB(src).op);
        }

        /******************************************
         and     rd,rs
         flags:  -ZS---
         ******************************************/
        void Z87_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = ANDW(RW(dst).op, RW(src).op);
        }

        /******************************************
         xorb    rbd,rbs
         flags:  -ZSP--
         ******************************************/
        void Z88_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = XORB(RB(dst).op, RB(src).op);
        }

        /******************************************
         xor     rd,rs
         flags:  -ZS---
         ******************************************/
        void Z89_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = XORW(RW(dst).op, RW(src).op);
        }

        /******************************************
         cpb     rbd,rbs
         flags:  CZSV--
         ******************************************/
        void Z8A_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            CPB(RB(dst).op, RB(src).op);
        }

        /******************************************
         cp      rd,rs
         flags:  CZSV--
         ******************************************/
        void Z8B_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            CPW(RW(dst).op, RW(src).op);
        }

        /******************************************
         comb    rbd
         flags:  -ZSP--
         ******************************************/
        void Z8C_dddd_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            RB(dst).op = COMB(RB(dst).op);
        }

        /******************************************
         negb    rbd
         flags:  CZSV--
         ******************************************/
        void Z8C_dddd_0010()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            RB(dst).op = NEGB(RB(dst).op);
        }

        /******************************************
         testb   rbd
         flags:  -ZSP--
         ******************************************/
        void Z8C_dddd_0100()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            TESTB(RB(dst).op);
        }

        /******************************************
         tsetb   rbd
         flags:  --S---
         ******************************************/
        void Z8C_dddd_0110()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            if ((RB(dst).op & S08) != 0) SET_S(); else CLR_S();
            RB(dst).op = 0xff;
        }

        /******************************************
         ldctlb rbd,flags
         flags:  CZSVDH
         ******************************************/
        void Z8C_dddd_0001()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            RB(dst).op = (uint8_t)(m_fcw & 0xfc);
        }

        /******************************************
         clrb    rbd
         flags:  ------
         ******************************************/
        void Z8C_dddd_1000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            RB(dst).op = 0;
        }

        /******************************************
         ldctlb flags,rbd
         flags:  ------
         ******************************************/
        void Z8C_dddd_1001()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            m_fcw &= unchecked((uint16_t)~0x00fc);
            m_fcw |= (uint16_t)(RB(dst).op & 0xfc);
        }

        /******************************************
         nop
         flags:  ------
         ******************************************/
        void Z8D_0000_0111()
        {
            /* nothing */
        }

        /******************************************
         com     rd
         flags:  -ZS---
         ******************************************/
        void Z8D_dddd_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            RW(dst).op = COMW(RW(dst).op);
        }

        /******************************************
         neg     rd
         flags:  CZSV--
         ******************************************/
        void Z8D_dddd_0010()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            RW(dst).op = NEGW(RW(dst).op);
        }

        /******************************************
         test    rd
         flags:  ------
         ******************************************/
        void Z8D_dddd_0100()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            TESTW(RW(dst).op);
        }

        /******************************************
         tset    rd
         flags:  --S---
         ******************************************/
        void Z8D_dddd_0110()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            if ((RW(dst).op & S16) != 0) SET_S(); else CLR_S();
            RW(dst).op = 0xffff;
        }

        /******************************************
         clr     rd
         flags:  ------
         ******************************************/
        void Z8D_dddd_1000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            RW(dst).op = 0;
        }

        /******************************************
         setflg  imm4
         flags:  CZSV--
         ******************************************/
        void Z8D_imm4_0001()
        {
            m_fcw |= (uint16_t)(m_op[0] & 0x00f0);
        }

        /******************************************
         resflg  imm4
         flags:  CZSV--
         ******************************************/
        void Z8D_imm4_0011()
        {
            m_fcw &= (uint16_t)~(m_op[0] & 0x00f0);
        }

        /******************************************
         comflg  flags
         flags:  CZSP--
         ******************************************/
        void Z8D_imm4_0101()
        {
            m_fcw ^= (uint16_t)(m_op[0] & 0x00f0);
        }

        /******************************************
         ext8e   imm8
         flags:  ------
         ******************************************/
        void Z8E_imm8()
        {
            CHECK_EXT_INSTR();
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: ext8e  ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
        }

        /******************************************
         ext8f   imm8
         flags:  ------
         ******************************************/
        void Z8F_imm8()
        {
            CHECK_EXT_INSTR();
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: ext8f  ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
        }

        /******************************************
         cpl     rrd,rrs
         flags:  CZSV--
         ******************************************/
        void Z90_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            CPL(RL(dst).op, RL(src).op);
        }

        /******************************************
         pushl   @rd,rrs
         flags:  ------
         ******************************************/
        void Z91_ddN0_ssss()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            PUSHL(dst, RL(src).op);
        }

        /******************************************
         subl    rrd,rrs
         flags:  CZSV--
         ******************************************/
        void Z92_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = SUBL(RL(dst).op, RL(src).op);
        }

        /******************************************
         push    @rd,rs
         flags:  ------
         ******************************************/
        void Z93_ddN0_ssss()
        {
            uint8_t src = GET_SRC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            PUSHW(dst, RW(src).op);
        }

        /******************************************
         ldl     rrd,rrs
         flags:  ------
         ******************************************/
        void Z94_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = RL(src).op;
        }

        /******************************************
         popl    rrd,@rs
         flags:  ------
         ******************************************/
        void Z95_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = POPL(src);
        }

        /******************************************
         addl    rrd,rrs
         flags:  CZSV--
         ******************************************/
        void Z96_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = ADDL(RL(dst).op, RL(src).op);
        }

        /******************************************
         pop     rd,@rs
         flags:  ------
         ******************************************/
        void Z97_ssN0_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = POPW(src);
        }

        /******************************************
         multl   rqd,rrs
         flags:  CZSV--
         ******************************************/
        void Z98_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RQ(dst).op = MULTL((uint32_t)RQ(dst).op, RL(src).op);
        }

        /******************************************
         mult    rrd,rs
         flags:  CZSV--
         ******************************************/
        void Z99_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = MULTW((uint16_t)RL(dst).op, RW(src).op);
        }

        /******************************************
         divl    rqd,rrs
         flags:  CZSV--
         ******************************************/
        void Z9A_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RQ(dst).op = DIVL(RQ(dst).op, RL(src).op);
        }

        /******************************************
         div     rrd,rs
         flags:  CZSV--
         ******************************************/
        void Z9B_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RL(dst).op = DIVW(RL(dst).op, RW(src).op);
        }

        /******************************************
         testl   rrd
         flags:  -ZS---
         ******************************************/
        void Z9C_dddd_1000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            CLR_ZS();
            if (RL(dst).op == 0) SET_Z();
            else if ((RL(dst).op & S32) != 0) SET_S();
        }

        /******************************************
         rsvd9d
         flags:  ------
         ******************************************/
        void Z9D_imm8()
        {
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: rsvd9d ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
        }

        /******************************************
         ret     cc
         flags:  ------
         ******************************************/
        void Z9E_0000_cccc()
        {
            uint8_t cc = GET_CCC(OP0,NIB3);
            if (get_segmented_mode())
            {
                switch (cc)
                {
                    case  0: if (CC0) set_pc(segmented_addr(POPL(SP))); break;
                    case  1: if (CC1) set_pc(segmented_addr(POPL(SP))); break;
                    case  2: if (CC2) set_pc(segmented_addr(POPL(SP))); break;
                    case  3: if (CC3) set_pc(segmented_addr(POPL(SP))); break;
                    case  4: if (CC4) set_pc(segmented_addr(POPL(SP))); break;
                    case  5: if (CC5) set_pc(segmented_addr(POPL(SP))); break;
                    case  6: if (CC6) set_pc(segmented_addr(POPL(SP))); break;
                    case  7: if (CC7) set_pc(segmented_addr(POPL(SP))); break;
                    case  8: if (CC8) set_pc(segmented_addr(POPL(SP))); break;
                    case  9: if (CC9) set_pc(segmented_addr(POPL(SP))); break;
                    case 10: if (CCA) set_pc(segmented_addr(POPL(SP))); break;
                    case 11: if (CCB) set_pc(segmented_addr(POPL(SP))); break;
                    case 12: if (CCC) set_pc(segmented_addr(POPL(SP))); break;
                    case 13: if (CCD) set_pc(segmented_addr(POPL(SP))); break;
                    case 14: if (CCE) set_pc(segmented_addr(POPL(SP))); break;
                    case 15: if (CCF) set_pc(segmented_addr(POPL(SP))); break;
                }
            }
            else
            {
                switch (cc)
                {
                    case  0: if (CC0) set_pc(POPW(SP)); break;
                    case  1: if (CC1) set_pc(POPW(SP)); break;
                    case  2: if (CC2) set_pc(POPW(SP)); break;
                    case  3: if (CC3) set_pc(POPW(SP)); break;
                    case  4: if (CC4) set_pc(POPW(SP)); break;
                    case  5: if (CC5) set_pc(POPW(SP)); break;
                    case  6: if (CC6) set_pc(POPW(SP)); break;
                    case  7: if (CC7) set_pc(POPW(SP)); break;
                    case  8: if (CC8) set_pc(POPW(SP)); break;
                    case  9: if (CC9) set_pc(POPW(SP)); break;
                    case 10: if (CCA) set_pc(POPW(SP)); break;
                    case 11: if (CCB) set_pc(POPW(SP)); break;
                    case 12: if (CCC) set_pc(POPW(SP)); break;
                    case 13: if (CCD) set_pc(POPW(SP)); break;
                    case 14: if (CCE) set_pc(POPW(SP)); break;
                    case 15: if (CCF) set_pc(POPW(SP)); break;
                }
            }
        }

        /******************************************
         rsvd9f
         flags:  ------
         ******************************************/
        void Z9F_imm8()
        {
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: rsvd9f ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
        }

        /******************************************
         ldb     rbd,rbs
         flags:  ------
         ******************************************/
        void ZA0_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = RB(src).op;
        }

        /******************************************
         ld      rd,rs
         flags:  ------
         ******************************************/
        void ZA1_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = RW(src).op;
        }

        /******************************************
         resb    rbd,imm4
         flags:  ------
         ******************************************/
        void ZA2_dddd_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            RB(dst).op &= (uint8_t)~bit;
        }

        /******************************************
         res     rd,imm4
         flags:  ------
         ******************************************/
        void ZA3_dddd_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            RW(dst).op &= (uint16_t)~bit;
        }

        /******************************************
         setb    rbd,imm4
         flags:  ------
         ******************************************/
        void ZA4_dddd_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            RB(dst).op |= (uint8_t)bit;
        }

        /******************************************
         set     rd,imm4
         flags:  ------
         ******************************************/
        void ZA5_dddd_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            RW(dst).op |= bit;
        }

        /******************************************
         bitb    rbd,imm4
         flags:  -Z----
         ******************************************/
        void ZA6_dddd_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            if ((RB(dst).op & bit) != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         bit     rd,imm4
         flags:  -Z----
         ******************************************/
        void ZA7_dddd_imm4()
        {
            uint16_t bit = GET_BIT(OP0);
            uint8_t dst = GET_DST(OP0,NIB2);
            if ((RW(dst).op & bit) != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         incb    rbd,imm4m1
         flags:  -ZSV--
         ******************************************/
        void ZA8_dddd_imm4m1()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            RB(dst).op = INCB(RB(dst).op, i4p1);
        }

        /******************************************
         inc     rd,imm4m1
         flags:  -ZSV--
         ******************************************/
        void ZA9_dddd_imm4m1()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            RW(dst).op = INCW(RW(dst).op, i4p1);
        }

        /******************************************
         decb    rbd,imm4m1
         flags:  -ZSV--
         ******************************************/
        void ZAA_dddd_imm4m1()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            RB(dst).op = DECB(RB(dst).op, i4p1);
        }

        /******************************************
         dec     rd,imm4m1
         flags:  -ZSV--
         ******************************************/
        void ZAB_dddd_imm4m1()
        {
            uint8_t i4p1 = GET_I4M1(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            RW(dst).op = DECW(RW(dst).op, i4p1);
        }

        /******************************************
         exb     rbd,rbs
         flags:  ------
         ******************************************/
        void ZAC_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t tmp = RB(src).op;
            RB(src).op = RB(dst).op;
            RB(dst).op = tmp;
        }

        /******************************************
         ex      rd,rs
         flags:  ------
         ******************************************/
        void ZAD_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            uint16_t tmp = RW(src).op;
            RW(src).op = RW(dst).op;
            RW(dst).op = tmp;
        }

        /******************************************
         tccb    cc,rbd
         flags:  ------
         ******************************************/
        void ZAE_dddd_cccc()
        {
            uint8_t cc = GET_CCC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t tmp = (uint8_t)(RB(dst).op & ~1);
            switch (cc)
            {
                case  0: if (CC0) tmp |= 1; break;
                case  1: if (CC1) tmp |= 1; break;
                case  2: if (CC2) tmp |= 1; break;
                case  3: if (CC3) tmp |= 1; break;
                case  4: if (CC4) tmp |= 1; break;
                case  5: if (CC5) tmp |= 1; break;
                case  6: if (CC6) tmp |= 1; break;
                case  7: if (CC7) tmp |= 1; break;
                case  8: if (CC8) tmp |= 1; break;
                case  9: if (CC9) tmp |= 1; break;
                case 10: if (CCA) tmp |= 1; break;
                case 11: if (CCB) tmp |= 1; break;
                case 12: if (CCC) tmp |= 1; break;
                case 13: if (CCD) tmp |= 1; break;
                case 14: if (CCE) tmp |= 1; break;
                case 15: if (CCF) tmp |= 1; break;
            }
            RB(dst).op = tmp;
        }

        /******************************************
         tcc     cc,rd
         flags:  ------
         ******************************************/
        void ZAF_dddd_cccc()
        {
            uint8_t cc = GET_CCC(OP0,NIB3);
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t tmp = (uint16_t)(RW(dst).op & ~1);
            switch (cc)
            {
                case  0: if (CC0) tmp |= 1; break;
                case  1: if (CC1) tmp |= 1; break;
                case  2: if (CC2) tmp |= 1; break;
                case  3: if (CC3) tmp |= 1; break;
                case  4: if (CC4) tmp |= 1; break;
                case  5: if (CC5) tmp |= 1; break;
                case  6: if (CC6) tmp |= 1; break;
                case  7: if (CC7) tmp |= 1; break;
                case  8: if (CC8) tmp |= 1; break;
                case  9: if (CC9) tmp |= 1; break;
                case 10: if (CCA) tmp |= 1; break;
                case 11: if (CCB) tmp |= 1; break;
                case 12: if (CCC) tmp |= 1; break;
                case 13: if (CCD) tmp |= 1; break;
                case 14: if (CCE) tmp |= 1; break;
                case 15: if (CCF) tmp |= 1; break;
            }
            RW(dst).op = tmp;
        }

        /******************************************
         dab     rbd
         flags:  CZS---
         ******************************************/
        void ZB0_dddd_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t result;
            uint16_t idx = RB(dst).op;
            if ((m_fcw & F_C) != 0)    idx |= 0x100;
            if ((m_fcw & F_H) != 0)    idx |= 0x200;
            if ((m_fcw & F_DA) != 0)   idx |= 0x400;
            result = (uint8_t)Z8000_dab[idx];
            CLR_CZS();
            CHK_XXXB_ZS(result);
            if ((Z8000_dab[idx] & 0x100) != 0) SET_C();
            RB(dst).op = result;
        }

        /******************************************
         extsb   rd
         flags:  ------
         ******************************************/
        void ZB1_dddd_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            RW(dst).op = (uint16_t)(int16_t)(int8_t)RW(dst).op;
        }

        /******************************************
         extsl   rqd
         flags:  ------
         ******************************************/
        void ZB1_dddd_0111()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            RQ(dst).op = (uint64_t)(int64_t)(int32_t)RQ(dst).op;
        }

        /******************************************
         exts    rrd
         flags:  ------
         ******************************************/
        void ZB1_dddd_1010()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            RL(dst).op = (uint32_t)(int32_t)(int16_t)RL(dst).op;
        }

        /******************************************
         sllb    rbd,imm8
         flags:  CZS---
         srlb    rbd,imm8
         flags:  CZSV--
         ******************************************/
        void ZB2_dddd_0001_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm8 = GET_IMM8(OP1);
            if ((imm8 & S08) != 0)
                RB(dst).op = SRLB(RB(dst).op, (uint8_t)(-(int8_t)imm8));
            else
                RB(dst).op = SLLB(RB(dst).op, imm8);
        }

        /******************************************
         sdlb    rbd,rs
         flags:  CZS---
         ******************************************/
        void ZB2_dddd_0011_0000_ssss_0000_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB1);
            RB(dst).op = SRLB(RB(dst).op, (uint8_t)(int8_t)RW(src).op);
        }

        /******************************************
         rlb     rbd,imm1or2
         flags:  CZSV--
         ******************************************/
        void ZB2_dddd_00I0()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm1 = GET_IMM1(OP0,NIB3);
            RB(dst).op = RLB(RB(dst).op, imm1);
        }

        /******************************************
         rrb     rbd,imm1or2
         flags:  CZSV--
         ******************************************/
        void ZB2_dddd_01I0()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm1 = GET_IMM1(OP0,NIB3);
            RB(dst).op = RRB(RB(dst).op, imm1);
        }

        /******************************************
         slab    rbd,imm8
         flags:  CZSV--
         srab    rbd,imm8
         flags:  CZSV--
         ******************************************/
        void ZB2_dddd_1001_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm8 = GET_IMM8(OP1);
            if ((imm8 & S08) != 0)
                RB(dst).op = SRAB(RB(dst).op, (uint8_t)(-(int8_t)imm8));
            else
                RB(dst).op = SLAB(RB(dst).op, imm8);
        }

        /******************************************
         sdab    rbd,rs
         flags:  CZSV--
         ******************************************/
        void ZB2_dddd_1011_0000_ssss_0000_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB1);
            RB(dst).op = SDAB(RB(dst).op, (int8_t) RW(src).op);
        }

        /******************************************
         rlcb    rbd,imm1or2
         flags:  -Z----
         ******************************************/
        void ZB2_dddd_10I0()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm1 = GET_IMM1(OP0,NIB3);
            RB(dst).op = RLCB(RB(dst).op, imm1);
        }

        /******************************************
         rrcb    rbd,imm1or2
         flags:  -Z----
         ******************************************/
        void ZB2_dddd_11I0()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm1 = GET_IMM1(OP0,NIB3);
            RB(dst).op = RRCB(RB(dst).op, imm1);
        }

        /******************************************
         sll     rd,imm8
         flags:  CZS---
         srl     rd,imm8
         flags:  CZSV--
         ******************************************/
        void ZB3_dddd_0001_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            if ((imm16 & S16) != 0)
                RW(dst).op = SRLW(RW(dst).op, (uint8_t)(-(int16_t)imm16));
            else
                RW(dst).op = SLLW(RW(dst).op, (uint8_t)imm16);
        }

        /******************************************
         sdl     rd,rs
         flags:  CZS---
         ******************************************/
        void ZB3_dddd_0011_0000_ssss_0000_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB1);
            RW(dst).op = SDLW(RW(dst).op, (int8_t)RW(src).op);
        }

        /******************************************
         rl      rd,imm1or2
         flags:  CZSV--
         ******************************************/
        void ZB3_dddd_00I0()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm1 = GET_IMM1(OP0,NIB3);
            RW(dst).op = RLW(RW(dst).op, imm1);
        }

        /******************************************
         slll    rrd,imm8
         flags:  CZS---
         srll    rrd,imm8
         flags:  CZSV--
         ******************************************/
        void ZB3_dddd_0101_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            if ((imm16 & S16) != 0)
                RL(dst).op = SRLL(RL(dst).op, (uint8_t)(-(int16_t)imm16));
            else
                RL(dst).op = SLLL(RL(dst).op, (uint8_t)imm16);
        }

        /******************************************
         sdll    rrd,rs
         flags:  CZS---
         ******************************************/
        void ZB3_dddd_0111_0000_ssss_0000_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB1);
            RL(dst).op = SDLL(RL(dst).op, (int8_t)(RW(src).op & 0xff));
        }

        /******************************************
         rr      rd,imm1or2
         flags:  CZSV--
         ******************************************/
        void ZB3_dddd_01I0()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm1 = GET_IMM1(OP0,NIB3);
            RW(dst).op = RRW(RW(dst).op, imm1);
        }

        /******************************************
         sla     rd,imm8
         flags:  CZSV--
         sra     rd,imm8
         flags:  CZSV--
         ******************************************/
        void ZB3_dddd_1001_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            if ((imm16 & S16) != 0)
                RW(dst).op = SRAW(RW(dst).op, (uint8_t)(-(int16_t)imm16));
            else
                RW(dst).op = SLAW(RW(dst).op, (uint8_t)imm16);
        }

        /******************************************
         sda     rd,rs
         flags:  CZSV--
         ******************************************/
        void ZB3_dddd_1011_0000_ssss_0000_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB1);
            RW(dst).op = SDAW(RW(dst).op, (int8_t)RW(src).op);
        }

        /******************************************
         rlc     rd,imm1or2
         flags:  CZSV--
         ******************************************/
        void ZB3_dddd_10I0()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm1 = GET_IMM1(OP0,NIB3);
            RW(dst).op = RLCW(RW(dst).op, imm1);
        }

        /******************************************
         slal    rrd,imm8
         flags:  CZSV--
         sral    rrd,imm8
         flags:  CZSV--
         ******************************************/
        void ZB3_dddd_1101_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint16_t imm16 = GET_IMM16(OP1);
            if ((imm16 & S16) != 0)
                RL(dst).op = SRAL(RL(dst).op, (uint8_t)(-(int16_t)imm16));
            else
                RL(dst).op = SLAL(RL(dst).op, (uint8_t)imm16);
        }

        /******************************************
         sdal    rrd,rs
         flags:  CZSV--
         ******************************************/
        void ZB3_dddd_1111_0000_ssss_0000_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB1);
            RL(dst).op = SDAL(RL(dst).op, (int8_t)(RW(src).op & 0xff));
        }

        /******************************************
         rrc     rd,imm1or2
         flags:  CZSV--
         ******************************************/
        void ZB3_dddd_11I0()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm1 = GET_IMM1(OP0,NIB3);
            RW(dst).op = RRCW(RW(dst).op, imm1);
        }

        /******************************************
         adcb    rbd,rbs
         flags:  CZSVDH
         ******************************************/
        void ZB4_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = ADCB(RB(dst).op, RB(src).op);
        }

        /******************************************
         adc     rd,rs
         flags:  CZSV--
         ******************************************/
        void ZB5_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = ADCW(RW(dst).op, RW(src).op);
        }

        /******************************************
         sbcb    rbd,rbs
         flags:  CZSVDH
         ******************************************/
        void ZB6_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RB(dst).op = SBCB(RB(dst).op, RB(src).op);
        }

        /******************************************
         sbc     rd,rs
         flags:  CZSV--
         ******************************************/
        void ZB7_ssss_dddd()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t src = GET_SRC(OP0,NIB2);
            RW(dst).op = SBCW(RW(dst).op, RW(src).op);
        }

        /******************************************
         trtib   @rd,@rs,rr
         flags:  -ZSV--
         ******************************************/
        void ZB8_ddN0_0010_0000_rrrr_ssN0_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t xlt = RDBX_B(src, RDIR_B(dst));
            RB(1).op = xlt;  /* load RH1 */
            if (xlt != 0) CLR_Z(); else SET_Z();
            add_to_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         trtirb  @rd,@rs,rbr
         flags:  -ZSV--
         ******************************************/
        void ZB8_ddN0_0110_0000_rrrr_ssN0_1110()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t xlt = RDBX_B(src, RDIR_B(dst));
            RB(1).op = xlt;  /* load RH1 */
            if (xlt != 0) CLR_Z(); else SET_Z();
            add_to_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0)
            {
                CLR_V();
                if (xlt == 0)
                m_pc -= 4;
            }
            else SET_V();
        }

        /******************************************
         trtdb   @rd,@rs,rbr
         flags:  -ZSV--
         ******************************************/
        void ZB8_ddN0_1010_0000_rrrr_ssN0_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t xlt = RDBX_B(src, RDIR_B(dst));
            RB(1).op = xlt;  /* load RH1 */
            if (xlt != 0) CLR_Z(); else SET_Z();
            sub_from_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         trtdrb  @rd,@rs,rbr
         flags:  -ZSV--
         ******************************************/
        void ZB8_ddN0_1110_0000_rrrr_ssN0_1110()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t xlt = RDBX_B(src, RDIR_B(dst));
            RB(1).op = xlt;  /* load RH1 */
            if (xlt != 0) CLR_Z(); else SET_Z();
            sub_from_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0)
            {
                CLR_V();
                if (xlt == 0)
                m_pc -= 4;
            }
            else SET_V();
        }

        /******************************************
         trib    @rd,@rs,rbr
         flags:  -ZSV--
         ******************************************/
        void ZB8_ddN0_0000_0000_rrrr_ssN0_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific dstspace = dst == SP ? m_stack : m_data;
            uint32_t dstaddr = addr_from_reg(dst);
            uint8_t xlt = RDBX_B(src, RDMEM_B(dstspace, dstaddr));
            WRMEM_B(dstspace, dstaddr, xlt);
            RB(1).op = xlt;  /* destroy RH1 */
            add_to_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         trirb   @rd,@rs,rbr
         flags:  -ZSV--
         ******************************************/
        void ZB8_ddN0_0100_0000_rrrr_ssN0_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific dstspace = dst == SP ? m_stack : m_data;
            uint32_t dstaddr = addr_from_reg(dst);
            uint8_t xlt = RDBX_B(src, RDMEM_B(dstspace, dstaddr));
            WRMEM_B(dstspace, dstaddr, xlt);
            RB(1).op = xlt;  /* destroy RH1 */
            add_to_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); m_pc -= 4; } else SET_V();
        }

        /******************************************
         trdb    @rd,@rs,rbr
         flags:  -ZSV--
         ******************************************/
        void ZB8_ddN0_1000_0000_rrrr_ssN0_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific dstspace = dst == SP ? m_stack : m_data;
            uint32_t dstaddr = addr_from_reg(dst);
            uint8_t xlt = RDBX_B(src, RDMEM_B(dstspace, dstaddr));
            WRMEM_B(dstspace, dstaddr, xlt);
            RB(1).op = xlt;  /* destroy RH1 */
            sub_from_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         trdrb   @rd,@rs,rbr
         flags:  -ZSV--
         ******************************************/
        void ZB8_ddN0_1100_0000_rrrr_ssN0_0000()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t src = GET_SRC(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            memory_access<int_const_23, int_const_1, int_const_0, endianness_t_const_ENDIANNESS_BIG>.specific dstspace = dst == SP ? m_stack : m_data;
            uint32_t dstaddr = addr_from_reg(dst);
            uint8_t xlt = RDBX_B(src, RDMEM_B(dstspace, dstaddr));
            WRMEM_B(dstspace, dstaddr, xlt);
            RB(1).op = xlt;  /* destroy RH1 */
            sub_from_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); m_pc -= 4; } else SET_V();
        }

        /******************************************
         rsvdb9
         flags:  ------
         ******************************************/
        void ZB9_imm8()
        {
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: rsvdb9 ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
            //(void)imm8;
        }

        /******************************************
         cpib    rbd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBA_ssN0_0000_0000_rrrr_dddd_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPB(RB(dst).op, RDIR_B(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            add_to_addr_reg(src, 1);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         ldib    @rd,@rs,rr
         ldibr   @rd,@rs,rr
         flags:  ---V--
         ******************************************/
        void ZBA_ssN0_0001_0000_rrrr_ddN0_x000()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);  /* repeat? */
            WRIR_B(dst, RDIR_B(src));
            add_to_addr_reg(src, 1);
            add_to_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpsib   @rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBA_ssN0_0010_0000_rrrr_ddN0_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPB(RDIR_B(dst), RDIR_B(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            add_to_addr_reg(src, 1);
            add_to_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); if ((m_fcw & F_Z) == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpirb   rbd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBA_ssN0_0100_0000_rrrr_dddd_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPB(RB(dst).op, RDIR_B(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            add_to_addr_reg(src, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); if ((m_fcw & F_Z) == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpsirb  @rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBA_ssN0_0110_0000_rrrr_ddN0_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPB(RDIR_B(dst), RDIR_B(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            add_to_addr_reg(src, 1);
            add_to_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); if ((m_fcw & F_Z) == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpdb    rbd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBA_ssN0_1000_0000_rrrr_dddd_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPB(RB(dst).op, RDIR_B(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            sub_from_addr_reg(src, 1);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         lddb    @rs,@rd,rr
         lddbr   @rs,@rd,rr
         flags:  ---V--
         ******************************************/
        void ZBA_ssN0_1001_0000_rrrr_ddN0_x000()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_B(dst, RDIR_B(src));
            sub_from_addr_reg(src, 1);
            sub_from_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpsdb   @rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBA_ssN0_1010_0000_rrrr_ddN0_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPB(RDIR_B(dst), RDIR_B(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            sub_from_addr_reg(src, 1);
            sub_from_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         cpdrb   rbd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBA_ssN0_1100_0000_rrrr_dddd_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPB(RB(dst).op, RDIR_B(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            sub_from_addr_reg(src, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); if ((m_fcw & F_Z) == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpsdrb  @rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBA_ssN0_1110_0000_rrrr_ddN0_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPB(RDIR_B(dst), RDIR_B(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            sub_from_addr_reg(src, 1);
            sub_from_addr_reg(dst, 1);
            if ((--RW(cnt).op) != 0) { CLR_V(); if ((m_fcw & F_Z) == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpi     rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBB_ssN0_0000_0000_rrrr_dddd_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPW(RW(dst).op, RDIR_W(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            add_to_addr_reg(src, 2);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         ldi     @rd,@rs,rr
         ldir    @rd,@rs,rr
         flags:  ---V--
         ******************************************/
        void ZBB_ssN0_0001_0000_rrrr_ddN0_x000()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_W(dst, RDIR_W(src));
            add_to_addr_reg(src, 2);
            add_to_addr_reg(dst, 2);
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpsi    @rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBB_ssN0_0010_0000_rrrr_ddN0_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPW(RDIR_W(dst), RDIR_W(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            add_to_addr_reg(src, 2);
            add_to_addr_reg(dst, 2);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         cpir    rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBB_ssN0_0100_0000_rrrr_dddd_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPW(RW(dst).op, RDIR_W(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            add_to_addr_reg(src, 2);
            if ((--RW(cnt).op) != 0) { CLR_V(); if ((m_fcw & F_Z) == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpsir   @rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBB_ssN0_0110_0000_rrrr_ddN0_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPW(RDIR_W(dst), RDIR_W(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            add_to_addr_reg(src, 2);
            add_to_addr_reg(dst, 2);
            if ((--RW(cnt).op) != 0) { CLR_V(); if ((m_fcw & F_Z) == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpd     rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBB_ssN0_1000_0000_rrrr_dddd_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPW(RW(dst).op, RDIR_W(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            sub_from_addr_reg(src, 2);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         ldd     @rs,@rd,rr
         lddr    @rs,@rd,rr
         flags:  ---V--
         ******************************************/
        void ZBB_ssN0_1001_0000_rrrr_ddN0_x000()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            uint8_t dst = GET_DST(OP1,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            WRIR_W(dst, RDIR_W(src));
            sub_from_addr_reg(src, 2);
            sub_from_addr_reg(dst, 2);
            if ((--RW(cnt).op) != 0) { CLR_V(); if (cc == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpsd    @rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBB_ssN0_1010_0000_rrrr_ddN0_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPW(RDIR_W(dst), RDIR_W(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            sub_from_addr_reg(src, 2);
            sub_from_addr_reg(dst, 2);
            if ((--RW(cnt).op) != 0) CLR_V(); else SET_V();
        }

        /******************************************
         cpdr    rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBB_ssN0_1100_0000_rrrr_dddd_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPW(RW(dst).op, RDIR_W(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            sub_from_addr_reg(src, 2);
            if ((--RW(cnt).op) != 0) { CLR_V(); if ((m_fcw & F_Z) == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         cpsdr   @rd,@rs,rr,cc
         flags:  CZSV--
         ******************************************/
        void ZBB_ssN0_1110_0000_rrrr_ddN0_cccc()
        {
            uint8_t src = GET_SRC(OP0,NIB2);
            uint8_t cc = GET_CCC(OP1,NIB3);
            uint8_t dst = GET_DST(OP1,NIB2);
            int8_t cnt = GET_CNT(OP1,NIB1);
            CPW(RDIR_W(dst), RDIR_W(src));
            switch (cc)
            {
                case  0: if (CC0) SET_Z(); else CLR_Z(); break;
                case  1: if (CC1) SET_Z(); else CLR_Z(); break;
                case  2: if (CC2) SET_Z(); else CLR_Z(); break;
                case  3: if (CC3) SET_Z(); else CLR_Z(); break;
                case  4: if (CC4) SET_Z(); else CLR_Z(); break;
                case  5: if (CC5) SET_Z(); else CLR_Z(); break;
                case  6: if (CC6) SET_Z(); else CLR_Z(); break;
                case  7: if (CC7) SET_Z(); else CLR_Z(); break;
                case  8: if (CC8) SET_Z(); else CLR_Z(); break;
                case  9: if (CC9) SET_Z(); else CLR_Z(); break;
                case 10: if (CCA) SET_Z(); else CLR_Z(); break;
                case 11: if (CCB) SET_Z(); else CLR_Z(); break;
                case 12: if (CCC) SET_Z(); else CLR_Z(); break;
                case 13: if (CCD) SET_Z(); else CLR_Z(); break;
                case 14: if (CCE) SET_Z(); else CLR_Z(); break;
                case 15: if (CCF) SET_Z(); else CLR_Z(); break;
            }
            sub_from_addr_reg(src, 2);
            sub_from_addr_reg(dst, 2);
            if ((--RW(cnt).op) != 0) { CLR_V(); if ((m_fcw & F_Z) == 0) m_pc -= 4; } else SET_V();
        }

        /******************************************
         rrdb    rbb,rba
         flags:  -Z----
         ******************************************/
        void ZBC_aaaa_bbbb()
        {
            uint8_t b = (uint8_t)(m_op[0] & 15);
            uint8_t a = (uint8_t)((m_op[0] >> 4) & 15);
            uint8_t tmp = RB(b).op;
            RB(a).op = (uint8_t)((RB(a).op >> 4) | (RB(b).op << 4));
            RB(b).op = (uint8_t)((RB(b).op & 0xf0) | (tmp & 0x0f));
            if (RB(b).op != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         ldk     rd,imm4
         flags:  ------
         ******************************************/
        void ZBD_dddd_imm4()
        {
            uint8_t dst = GET_DST(OP0,NIB2);
            uint8_t imm4 = GET_IMM4(OP0,NIB3);
            RW(dst).op = imm4;
        }

        /******************************************
         rldb    rbb,rba
         flags:  -Z----
         ******************************************/
        void ZBE_aaaa_bbbb()
        {
            uint8_t b = (uint8_t)(m_op[0] & 15);
            uint8_t a = (uint8_t)((m_op[0] >> 4) & 15);
            uint8_t tmp = RB(a).op;
            RB(a).op = (uint8_t)((RB(a).op << 4) | (RB(b).op & 0x0f));
            RB(b).op = (uint8_t)((RB(b).op & 0xf0) | (tmp >> 4));
            if (RB(b).op != 0) CLR_Z(); else SET_Z();
        }

        /******************************************
         rsvdbf
         flags:  ------
         ******************************************/
        void ZBF_imm8()
        {
            uint8_t imm8 = GET_IMM8(0);
            LOG("Z8K {0}: rsvdbf ${1}\n", m_pc, imm8);
            if ((m_fcw & F_EPU) != 0)
            {
                /* Z8001 EPU code goes here */
                //(void)imm8;
            }
            //(void)imm8;
        }

        /******************************************
         ldb     rbd,imm8   (long version)
         flags:  ------
         ******************************************/
        void Z20_0000_dddd_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB3);
            uint8_t imm8 = GET_IMM8(OP1);
            RB(dst).op = imm8;
        }

        /******************************************
         ldb     rbd,imm8
         flags:  ------
         ******************************************/
        void ZC_dddd_imm8()
        {
            uint8_t dst = GET_DST(OP0,NIB1);
            uint8_t imm8 = GET_IMM8(0);
            RB(dst).op = imm8;
        }

        /******************************************
         calr    dsp12
         flags:  ------
         ******************************************/
        void ZD_dsp12()
        {
            int16_t dsp12 = (int16_t)(m_op[0] & 0xfff);
            if (get_segmented_mode())
                PUSHL(SP, make_segmented_addr(m_pc));
            else
                PUSHW(SP, (uint16_t)m_pc);
            dsp12 = (dsp12 & 2048) != 0 ? (int16_t)(4096 - 2 * (dsp12 & 2047)) : (int16_t)(-2 * (dsp12 & 2047));
            set_pc(addr_add(m_pc, (uint32_t)dsp12));
        }

        /******************************************
         jr      cc,dsp8
         flags:  ------
         ******************************************/
        void ZE_cccc_dsp8()
        {
            int8_t dsp8 = GET_DSP8;
            uint8_t cc = GET_CCC(OP0,NIB1);
            switch (cc)
            {
                case  0: if (CC0) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  1: if (CC1) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  2: if (CC2) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  3: if (CC3) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  4: if (CC4) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  5: if (CC5) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  6: if (CC6) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  7: if (CC7) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  8: if (CC8) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  9: if (CC9) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  10: if (CCA) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  11: if (CCB) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  12: if (CCC) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  13: if (CCD) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  14: if (CCE) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
                case  15: if (CCF) set_pc(addr_add(m_pc, (uint32_t)(dsp8 * 2))); break;
            }
        }

        /******************************************
         dbjnz   rbd,dsp7
         flags:  ------
         ******************************************/
        void ZF_dddd_0dsp7()
        {
            uint8_t dst = GET_DST(OP0,NIB1);
            uint8_t dsp7 = GET_DSP7;
            RB(dst).op -= 1;
            if (RB(dst).op != 0)
            {
                set_pc(addr_sub(m_pc, 2U * dsp7));
            }
        }

        /******************************************
         djnz    rd,dsp7
         flags:  ------
         ******************************************/
        void ZF_dddd_1dsp7()
        {
            uint8_t dst = GET_DST(OP0,NIB1);
            uint8_t dsp7 = GET_DSP7;
            RW(dst).op -= 1;
            if (RW(dst).op != 0)
            {
                set_pc(addr_sub(m_pc, 2U * dsp7));
            }
        }
    }
}
