// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;
using uint16_t = System.UInt16;

using static mame.emucore_global;


namespace mame
{
    public partial class m6809_base_device : cpu_device
    {
        //-------------------------------------------------
        //  rotate_right
        //-------------------------------------------------
#if false
        template<class T>
        inline ATTR_FORCE_INLINE T m6809_base_device::rotate_right(T value)
        {
            bool new_carry = (value & 1) ? true : false;
            value = value >> 1;

            T high_bit = ((T) 1) << (sizeof(T) * 8 - 1);
            if (m_cc & CC_C)
                value |= high_bit;
            else
                value &= ~high_bit;

            if (new_carry)
                m_cc |= CC_C;
            else
                m_cc &= ~CC_C;
            return value;
        }


        //-------------------------------------------------
        //  rotate_left
        //-------------------------------------------------

        template<class T>
        inline ATTR_FORCE_INLINE uint32_t m6809_base_device::rotate_left(T value)
        {
            T high_bit = ((T) 1) << (sizeof(T) * 8 - 1);
            bool new_carry = (value & high_bit) ? true : false;

            uint32_t new_value = value;
            new_value <<= 1;

            if (m_cc & CC_C)
                new_value |= 1;
            else
                new_value &= ~1;

            if (new_carry)
                m_cc |= CC_C;
            else
                m_cc &= ~CC_C;
            return new_value;
        }
#endif

        //-------------------------------------------------
        //  read_operand
        //-------------------------------------------------
        uint8_t read_operand()
        {
            switch (m_addressing_mode)
            {
                case ADDRESSING_MODE_EA:            return read_memory(m_ea.w);
                case ADDRESSING_MODE_IMMEDIATE:     return read_opcode_arg();
                case ADDRESSING_MODE_REGISTER_A:    return m_q.r.a;
                case ADDRESSING_MODE_REGISTER_B:    return m_q.r.b;
                default:                            fatalerror("Unexpected");   return 0x00;
            }
        }


        //-------------------------------------------------
        //  read_operand
        //-------------------------------------------------
        uint8_t read_operand(int ordinal)
        {
            throw new emu_unimplemented();
#if false
            switch(m_addressing_mode)
            {
                case ADDRESSING_MODE_EA:            return read_memory(m_ea.w + ordinal);
                case ADDRESSING_MODE_IMMEDIATE:     return read_opcode_arg();
                default:                            fatalerror("Unexpected");   return 0x00;
            }
#endif
        }


        //-------------------------------------------------
        //  write_operand
        //-------------------------------------------------
        void write_operand(uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            switch(m_addressing_mode)
            {
                case ADDRESSING_MODE_IMMEDIATE:     /* do nothing */                break;
                case ADDRESSING_MODE_EA:            write_memory(m_ea.w, data);     break;
                case ADDRESSING_MODE_REGISTER_A:    m_q.r.a = data;                 break;
                case ADDRESSING_MODE_REGISTER_B:    m_q.r.b = data;                 break;
                default:                            fatalerror("Unexpected");       break;
            }
#endif
        }


        //-------------------------------------------------
        //  write_operand
        //-------------------------------------------------
        void write_operand(int ordinal, uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            switch(m_addressing_mode)
            {
                case ADDRESSING_MODE_IMMEDIATE:     /* do nothing */                break;
                case ADDRESSING_MODE_EA:            write_memory(m_ea.w + ordinal, data);   break;
                default:                            fatalerror("Unexpected");       break;
            }
#endif
        }


        //-------------------------------------------------
        //  daa - decimal arithmetic adjustment instruction
        //-------------------------------------------------
        void daa()
        {
            throw new emu_unimplemented();
#if false
            uint16_t t, cf = 0;
            uint8_t msn = m_q.r.a & 0xF0;
            uint8_t lsn = m_q.r.a & 0x0F;

            // compute the carry
            if (lsn > 0x09 || m_cc & CC_H)  cf |= 0x06;
            if (msn > 0x80 && lsn > 0x09 )  cf |= 0x60;
            if (msn > 0x90 || m_cc & CC_C)  cf |= 0x60;

            // calculate the result
            t = m_q.r.a + cf;

            m_cc &= ~CC_V;
            if (t & 0x0100)     // keep carry from previous operation
                m_cc |= CC_C;

            // and put it back into A
            m_q.r.a = set_flags(CC_NZ, (uint8_t) t);
#endif
        }


        //-------------------------------------------------
        //  mul
        //-------------------------------------------------
        void mul()
        {
            throw new emu_unimplemented();
#if false
            // perform multiply
            uint16_t result = ((uint16_t) m_q.r.a) * ((uint16_t) m_q.r.b);

            // set result and Z flag
            m_q.r.d = set_flags(CC_Z, result);

            // set C flag
            if (m_q.r.d & 0x0080)
                m_cc |= CC_C;
            else
                m_cc &= ~CC_C;
#endif
        }


        //-------------------------------------------------
        //  ireg
        //-------------------------------------------------
        ref uint16_t ireg()
        {
            throw new emu_unimplemented();
#if false
            switch(m_opcode & 0x60)
            {
                case 0x00:  return m_x.w;
                case 0x20:  return m_y.w;
                case 0x40:  return m_u.w;
                case 0x60:  return m_s.w;
                default:
                    fatalerror("Unexpected");
                    return m_x.w;
            }
#endif
        }


#if false
        //-------------------------------------------------
        //  set_flags
        //-------------------------------------------------

        template<class T>
        inline T m6809_base_device::set_flags(uint8_t mask, T a, T b, uint32_t r)
        {
            T hi_bit = (T) (1 << (sizeof(T) * 8 - 1));

            m_cc &= ~mask;
            if (mask & CC_H)
                m_cc |= ((a ^ b ^ r) & 0x10) ? CC_H : 0;
            if (mask & CC_N)
                m_cc |= (r & hi_bit) ? CC_N : 0;
            if (mask & CC_Z)
                m_cc |= (((T)r) == 0) ? CC_Z : 0;
            if (mask & CC_V)
                m_cc |= ((a ^ b ^ r ^ (r >> 1)) & hi_bit) ? CC_V : 0;
            if (mask & CC_C)
                m_cc |= (r & (hi_bit << 1)) ? CC_C : 0;
            return (T) r;
        }


        //-------------------------------------------------
        //  set_flags
        //-------------------------------------------------

        template<class T>
        inline T m6809_base_device::set_flags(uint8_t mask, T r)
        {
            return set_flags(mask, (T)0, r, r);
        }
#endif


        //-------------------------------------------------
        //  eat_remaining
        //-------------------------------------------------
        void eat_remaining()
        {
            throw new emu_unimplemented();
#if false
            // we do this in order to be nice to people debugging
            uint16_t real_pc = m_pc.w;

            eat(m_icount);

            m_pc.w = m_ppc.w;
            debugger_instruction_hook(m_pc.w);
            m_pc.w = real_pc;
#endif
        }


        //-------------------------------------------------
        //  is_register_addressing_mode
        //-------------------------------------------------
        bool is_register_addressing_mode()
        {
            throw new emu_unimplemented();
            //return (m_addressing_mode != ADDRESSING_MODE_IMMEDIATE)
            //    && (m_addressing_mode != ADDRESSING_MODE_EA);
        }


        //-------------------------------------------------
        //  get_pending_interrupt
        //-------------------------------------------------
        uint16_t get_pending_interrupt()
        {
            if (m_nmi_asserted)
                return VECTOR_NMI;
            else if ((m_cc & CC_F) == 0 && m_firq_line)
                return VECTOR_FIRQ;
            else if ((m_cc & CC_I) == 0 && m_irq_line)
                return VECTOR_IRQ;
            else
                return 0;
        }
    }
}
