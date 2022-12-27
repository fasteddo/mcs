// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using u8 = System.Byte;


namespace mame
{
    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  Older Williams interrupts
         *
         *************************************/

        //TIMER_DEVICE_CALLBACK_MEMBER(williams_state::va11_callback)
        protected virtual void va11_callback(timer_device timer, object ptr, int param)
        {
            throw new emu_unimplemented();
#if false
            int scanline = param;

            // must not fire at line 256
            if (scanline == 256)
                return;

            /* the IRQ signal comes into CB1, and is set to VA11 */
            m_pia[1]->cb1_w(BIT(scanline, 5));
#endif
        }


        //TIMER_DEVICE_CALLBACK_MEMBER(williams_state::count240_callback)
        protected virtual void count240_callback(timer_device timer, object ptr, int param)
        {
            throw new emu_unimplemented();
#if false
#endif
        }


        /*************************************
         *
         *  Older Williams initialization
         *
         *************************************/

        protected override void machine_start()
        {
            /* configure the memory bank */
            m_mainbank.op0.configure_entry(1, new PointerU8(memregion("maincpu").base_()) + 0x10000);
            m_mainbank.op0.configure_entry(0, m_videoram.op);
        }
    }


        /*************************************
         *
         *  Newer Williams interrupts
         *
         *************************************/

        //TIMER_DEVICE_CALLBACK_MEMBER(williams2_state::va11_callback)

        //TIMER_DEVICE_CALLBACK_MEMBER(williams2_state::endscreen_callback)


        /*************************************
         *
         *  Newer Williams initialization
         *
         *************************************/

        //void williams2_state::machine_start()

        //void williams2_state::machine_reset()


    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  VRAM/ROM banking
         *
         *************************************/

        protected virtual void vram_select_w(u8 data)
        {
            throw new emu_unimplemented();
#if false
            /* VRAM/ROM banking from bit 0 */
            m_mainbank->set_entry(data & 0x01);

            /* cocktail flip from bit 1 */
            m_cocktail = data & 0x02;
#endif
        }
    }


        //void williams2_state::bank_select_w(u8 data)


        /*************************************
         *
         *  Sound commands
         *
         *************************************/

        //TIMER_CALLBACK_MEMBER(williams_state::deferred_snd_cmd_w)


    partial class williams_state : driver_device
    {
        void snd_cmd_w(u8 data)
        {
            throw new emu_unimplemented();
        }
    }


        //void playball_state::snd_cmd_w(u8 data)

        //TIMER_CALLBACK_MEMBER(williams2_state::deferred_snd_cmd_w)

        //void williams2_state::snd_cmd_w(u8 data)


    partial class williams_state : driver_device
    {
        /*
         *  Williams 49-way joystick
         *
         *  The joystick works on a 7x7 grid system:
         *
         *      + + + | + + +
         *      + + + | + + +
         *      + + + | + + +
         *      ------+------
         *      + + + | + + +
         *      + + + | + + +
         *      + + + | + + +
         *
         *  Each axis has 7 positions, reported as follows
         *  in 4 bits/axis:
         *
         *      0000 = left/up full
         *      0100 = left/up 2/3
         *      0110 = left/up 1/3
         *      0111 = center
         *      1011 = right/down 1/3
         *      1001 = right/down 2/3
         *      1000 = right/down full
         */
        protected u8 port_0_49way_r()
        {
            throw new emu_unimplemented();
        }


        /*************************************
         *
         *  CMOS access
         *
         *************************************/

        protected virtual void cmos_w(offs_t offset, u8 data)
        {
            throw new emu_unimplemented();
#if false
            /* only 4 bits are valid */
            m_nvram[offset] = data | 0xf0;
#endif
        }
    }


#if false
        void bubbles_state::cmos_w(offs_t offset, u8 data)
        {
            /* bubbles has additional CMOS for a full 8 bits */
            m_nvram[offset] = data;
        }
#endif


    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  Watchdog
         *
         *************************************/

        protected virtual void watchdog_reset_w(u8 data)
        {
            throw new emu_unimplemented();
#if false
            /* yes, the data bits are checked for this specific value */
            if (data == 0x39)
                m_watchdog->watchdog_reset();
#endif
        }
    }


        //void williams2_state::watchdog_reset_w(u8 data)


        /*************************************
         *
         *  Diagnostic controls
         *
         *************************************/

        //void williams2_state::segments_w(u8 data)


    partial class defender_state : williams_state
    {
        /*************************************
         *
         *  Defender-specific routines
         *
         *************************************/

        protected override void machine_reset()
        {
            throw new emu_unimplemented();
#if false
            bank_select_w(0);
#endif
        }


        //void defender_state::video_control_w(u8 data)


        void bank_select_w(u8 data) { throw new emu_unimplemented(); }
    }


        /*************************************
         *
         *  Mayday-specific routines
         *
         *************************************/

        //u8 mayday_state::protection_r(offs_t offset)


    partial class sinistar_state : williams_state
    {
        /*************************************
         *
         *  Sinistar-specific routines
         *
         *************************************/
        protected override void vram_select_w(u8 data)
        {
            throw new emu_unimplemented();
#if false
            /* low two bits are standard */
            williams_state::vram_select_w(data);

            /* window enable from bit 2 (clips to 0x7400) */
            m_blitter_window_enable = data & 0x04;
#endif
        }
    }


        /*************************************
         *
         *  Blaster-specific routines
         *
         *************************************/

        //void blaster_state::machine_start()

        //inline void blaster_state::update_blaster_banking()

        //void blaster_state::vram_select_w(u8 data)

        //void blaster_state::bank_select_w(u8 data)

        //TIMER_CALLBACK_MEMBER(blaster_state::deferred_snd_cmd_w)

        //void blaster_state::snd_cmd_w(u8 data)


        /*************************************
         *
         *  Lotto Fun-specific routines
         *
         *************************************/

        //WRITE_LINE_MEMBER(williams_state::lottofun_coin_lock_w)


        /*************************************
         *
         *  Williams 2nd-gen-specific routines
         *
         *************************************/

        //void williams2_state::video_control_w(u8 data)


        /*************************************
         *
         *  Turkey Shoot-specific routines
         *
         *************************************/

        //void tshoot_state::machine_start()

        //WRITE_LINE_MEMBER(tshoot_state::maxvol_w)

        //void tshoot_state::lamp_w(u8 data)


        /*************************************
         *
         *  Joust 2-specific routines
         *
         *************************************/

        //void joust2_state::machine_start()

        //TIMER_CALLBACK_MEMBER(joust2_state::deferred_snd_cmd_w)

        //WRITE_LINE_MEMBER(joust2_state::pia_s11_bg_strobe_w)

        //void joust2_state::snd_cmd_w(u8 data)
}
