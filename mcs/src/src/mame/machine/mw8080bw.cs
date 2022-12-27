// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using s32 = System.Int32;
using uint8_t = System.Byte;

using static mame.diexec_global;


namespace mame
{
    partial class mw8080bw_state : driver_device
    {
        /*************************************
         *
         *  Interrupt generation
         *
         *************************************/

        uint8_t vpos_to_vysnc_chain_counter(int vpos)
        {
            /* convert from a vertical position to the actual values on the vertical sync counters */
            uint8_t counter;
            int vblank = (vpos >= MW8080BW_VBSTART) ? 1 : 0;
        
            if (vblank != 0)
                counter = (uint8_t)(vpos - MW8080BW_VBSTART + MW8080BW_VCOUNTER_START_VBLANK);
            else
                counter = (uint8_t)(vpos + MW8080BW_VCOUNTER_START_NO_VBLANK);
        
            return counter;
        }


        int vysnc_chain_counter_to_vpos(uint8_t counter, int vblank)
        {
            /* convert from the vertical sync counters to an actual vertical position */
            int vpos;
        
            if (vblank != 0)
                vpos = counter - MW8080BW_VCOUNTER_START_VBLANK + MW8080BW_VBSTART;
            else
                vpos = counter - MW8080BW_VCOUNTER_START_NO_VBLANK;
        
            return vpos;
        }


        //TIMER_CALLBACK_MEMBER(mw8080bw_state::interrupt_trigger)
        void interrupt_trigger(object ptr, s32 param)  //void *ptr, s32 param)
        {
            int vpos = m_screen.op0.vpos();
            uint8_t counter = vpos_to_vysnc_chain_counter(vpos);

            m_maincpu.op0.set_input_line(0, ASSERT_LINE);

            m_interrupt_time = machine().time();

            /* set up for next interrupt */
            uint8_t next_counter;
            int next_vblank;
            if (counter == MW8080BW_INT_TRIGGER_COUNT_1)
            {
                next_counter = MW8080BW_INT_TRIGGER_COUNT_2;
                next_vblank = MW8080BW_INT_TRIGGER_VBLANK_2;
            }
            else
            {
                next_counter = MW8080BW_INT_TRIGGER_COUNT_1;
                next_vblank = MW8080BW_INT_TRIGGER_VBLANK_1;
            }

            int next_vpos = vysnc_chain_counter_to_vpos(next_counter, next_vblank);
            m_interrupt_timer.adjust(m_screen.op0.time_until_pos(next_vpos));
        }


        //IRQ_CALLBACK_MEMBER(mw8080bw_state::interrupt_vector)
        int interrupt_vector(device_t device, int irqline)
        {
            int vpos = m_screen.op0.vpos();
            // MAME scheduling quirks cause this to happen more often than you might think, in fact far too often
            if (machine().time() < m_interrupt_time)
                vpos++;

            uint8_t counter = vpos_to_vysnc_chain_counter(vpos);
            uint8_t vector = (uint8_t)(0xc7 | ((counter & 0x40) >> 2) | ((~counter & 0x40) >> 3));

            m_maincpu.op0.set_input_line(0, CLEAR_LINE);
            return vector;
        }


        void mw8080bw_create_interrupt_timer()
        {
            m_interrupt_timer = machine().scheduler().timer_alloc(interrupt_trigger);
        }


        void mw8080bw_start_interrupt_timer()
        {
            int vpos = vysnc_chain_counter_to_vpos(MW8080BW_INT_TRIGGER_COUNT_1, MW8080BW_INT_TRIGGER_VBLANK_1);
            m_interrupt_timer.adjust(m_screen.op0.time_until_pos(vpos));
        
            m_interrupt_time = attotime.zero;
        }


        /*************************************
         *
         *  Machine setup
         *
         *************************************/

        protected override void machine_start()
        {
            mw8080bw_create_interrupt_timer();
        }


        /*************************************
         *
         *  Machine reset
         *
         *************************************/

        //MACHINE_RESET_MEMBER(mw8080bw_state,mw8080bw)
        void machine_reset_mw8080bw()
        {
            mw8080bw_start_interrupt_timer();
        }
    }
}
