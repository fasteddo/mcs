// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using PointerU8 = mame.Pointer<System.Byte>;
using u8 = System.Byte;
using uint8_t = System.Byte;


namespace mame
{
    partial class taitosj_state : driver_device
    {
        protected override void machine_start()
        {
            membank("bank1").configure_entry(0, new PointerU8(memregion("maincpu").base_(), 0x6000));
            membank("bank1").configure_entry(1, new PointerU8(memregion("maincpu").base_(), 0x10000));

            save_item(NAME(new { m_spacecr_prot_value }));
            save_item(NAME(new { m_protection_value }));
        }


        protected override void machine_reset()
        {
            /* set the default ROM bank (many games only have one bank and */
            /* never write to the bank selector register) */
            taitosj_bankswitch_w(0);

            m_spacecr_prot_value = 0;
        }


        void taitosj_bankswitch_w(uint8_t data)
        {
            machine().bookkeeping().coin_lockout_global_w(~data & 1);

            /* this is a bit of a hack, but works.
                Eventually the mixing of the ay1 outs and
                amplitude-overdrive-mute stuff done by
                bit 1 here should be done on a netlist.
            */
            m_ay1.target.disound.set_output_gain(0, (data & 0x2) != 0 ? 1.0f : 0.0f); // 3 outputs for Ay1 since it doesn't use tied together outs
            m_ay1.target.disound.set_output_gain(1, (data & 0x2) != 0 ? 1.0f : 0.0f);
            m_ay1.target.disound.set_output_gain(2, (data & 0x2) != 0 ? 1.0f : 0.0f);
            m_ay2.target.disound.set_output_gain(0, (data & 0x2) != 0 ? 1.0f : 0.0f);
            m_ay3.target.disound.set_output_gain(0, (data & 0x2) != 0 ? 1.0f : 0.0f);
            m_ay4.target.disound.set_output_gain(0, (data & 0x2) != 0 ? 1.0f : 0.0f);
            m_dac.target.disound.set_output_gain(0, (data & 0x2) != 0 ? 1.0f : 0.0f);

            if ((data & 0x80) != 0) membank("bank1").set_entry(1);
            else membank("bank1").set_entry(0);
        }


        /***************************************************************************

                                   PROTECTION HANDLING

         Some of the games running on this hardware are protected with a 68705 mcu.
         It can either be on a daughter board containing Z80+68705+one ROM, which
         replaces the Z80 on an unprotected main board; or it can be built-in on the
         main board. The two are functionally equivalent.

         The 68705 can read commands from the Z80, send back result codes, and has
         direct access to the Z80 memory space. It can also trigger IRQs on the Z80.

        ***************************************************************************/
        uint8_t taitosj_fake_data_r()
        {
            LOG("{0}: protection read\n", m_maincpu.target.state().pc());
            return 0;
        }


        void taitosj_fake_data_w(uint8_t data)
        {
            LOG("{0}: protection write {1}\n", m_maincpu.target.state().pc(), data);
        }


        uint8_t taitosj_fake_status_r()
        {
            LOG("{0}: protection status read\n", m_maincpu.target.state().pc());
            return 0xff;
        }


        uint8_t mcu_mem_r(offs_t offset)
        {
            return m_maincpu.target.memory().space(AS_PROGRAM).read_byte(offset);
        }


        void mcu_mem_w(offs_t offset, uint8_t data)
        {
            m_maincpu.target.memory().space(AS_PROGRAM).write_byte(offset, data);
        }


        //WRITE_LINE_MEMBER(taitosj_state::mcu_intrq_w)
        void mcu_intrq_w(int state)
        {
            // FIXME: there's a logic network here that makes this edge sensitive or something and mixes it with other interrupt sources
            if (CLEAR_LINE != state)
                LOG("68705  68INTRQ **NOT SUPPORTED**!\n");
        }


        //WRITE_LINE_MEMBER(taitosj_state::mcu_busrq_w)
        void mcu_busrq_w(int state)
        {
            // this actually goes to the Z80 BUSRQ (aka WAIT) pin, and the MCU waits for the bus to become available
            // we're pretending this happens immediately to make life easier
            m_mcu.target.busak_w(state);
        }
    }
}
