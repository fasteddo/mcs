// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using uint8_t = System.Byte;

using static mame.diexec_global;
using static mame.util;


namespace mame
{
    partial class asteroid_state : driver_device
    {
        //INTERRUPT_GEN_MEMBER(asteroid_state::asteroid_interrupt)
        void asteroid_interrupt(device_t device)
        {
            /* Turn off interrupts if self-test is enabled */
            if ((ioport("IN0").read() & 0x80) == 0)
                device.execute().pulse_input_line(INPUT_LINE_NMI, attotime.zero);
        }


        //INTERRUPT_GEN_MEMBER(asteroid_state::asterock_interrupt)

        //INTERRUPT_GEN_MEMBER(asteroid_state::llander_interrupt)


        //WRITE_LINE_MEMBER(asteroid_state::cocktail_inv_w)
        void cocktail_inv_w(int state)
        {
            // Inverter circuit is only hooked up for Cocktail Asteroids
            int flip = (state != 0 && m_cocktail.op0.read() != 0) ? 1 : 0;
            m_dvg.op0.set_flip_x(flip != 0);
            m_dvg.op0.set_flip_y(flip != 0);
        }


        uint8_t asteroid_IN0_r(offs_t offset)
        {
            int res = (int)ioport("IN0").read();
            int bitmask = (1 << (int)offset);

            if ((res & bitmask) != 0)
                res = 0x80;
            else
                res = ~0x80;

            return (uint8_t)res;
        }


        //uint8_t asteroid_state::asterock_IN0_r(offs_t offset)


        /*
         * These 7 memory locations are used to read the player's controls.
         * Typically, only the high bit is used. This is handled by one input port.
         */
        uint8_t asteroid_IN1_r(offs_t offset)
        {
            int res = (int)ioport("IN1").read();
            int bitmask = (1 << (int)(offset & 0x7));

            if ((res & bitmask) != 0)
                res = 0x80;
            else
                res = ~0x80;

            return (uint8_t)res;
        }


        uint8_t asteroid_DSW1_r(offs_t offset)
        {
            // 765432--  not used
            // ------1-  ls253 dsw selector 2y
            // -------0  ls253 dsw selector 1y

            uint8_t val = (uint8_t)m_dsw1.op0.read();

            m_dsw_sel.op0.i3a_w(BIT(val, 0));
            m_dsw_sel.op0.i3b_w(BIT(val, 1));
            m_dsw_sel.op0.i2a_w(BIT(val, 2));
            m_dsw_sel.op0.i2b_w(BIT(val, 3));
            m_dsw_sel.op0.i1a_w(BIT(val, 4));
            m_dsw_sel.op0.i1b_w(BIT(val, 5));
            m_dsw_sel.op0.i0a_w(BIT(val, 6));
            m_dsw_sel.op0.i0b_w(BIT(val, 7));

            m_dsw_sel.op0.s_w((uint8_t)(offset & 0x03));

            return (uint8_t)(0xfc | (m_dsw_sel.op0.zb_r() << 1) | m_dsw_sel.op0.za_r());
        }


        protected override void machine_start()
        {
            /* configure RAM banks if present (not on llander) */
            if (m_ram1.target() != null)
            {
                /* swapped */
                m_ram1.op0.configure_entry(1, m_sram2.op);
                m_ram2.op0.configure_entry(1, m_sram1.op);
                /* normal */
                m_ram1.op0.configure_entry(0, m_sram1.op);
                m_ram2.op0.configure_entry(0, m_sram2.op);
            }
        }


        protected override void machine_reset()
        {
            m_dvg.op0.reset_w();
            if (m_earom.found())
                earom_control_w(0);
        
            /* reset RAM banks if present */
            if (m_ram1.target() != null)
            {
                m_ram1.op0.set_entry(0);
                m_ram2.op0.set_entry(0);
            }
        }
    }
}
