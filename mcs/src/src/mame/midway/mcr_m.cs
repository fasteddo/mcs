// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using s32 = System.Int32;

using static mame.digfx_global;
using static mame.emucore_global;


namespace mame
{
        //#define VERBOSE 0
        //#define LOG(x) do { if (VERBOSE) logerror x; } while (0)


    partial class mcr_state : driver_device
    {
        /*************************************
         *
         *  Graphics declarations
         *
         *************************************/

        // moved to drivers/mcr.cs to avoid static initialization order issues with partial classes
        // https://stackoverflow.com/questions/29086844/static-field-initialization-order-with-partial-classes
        //const gfx_layout mcr_bg_layout =
        //const gfx_layout mcr_sprite_layout =


        /*************************************
         *
         *  Generic MCR CTC interface
         *
         *************************************/

        static readonly z80_daisy_config [] mcr_daisy_chain = 
        {
            new z80_daisy_config() { devname = "ctc" },
            new z80_daisy_config() { devname = null }
        };


        //const z80_daisy_config mcr_ipu_daisy_chain[] =
        //{
        //    { "ipu_ctc" },
        //    { "ipu_pio1" },
        //    { "ipu_sio" },
        //    { "ipu_pio0" },
        //    { nullptr }
        //};


        /*************************************
         *
         *  Generic MCR machine initialization
         *
         *************************************/

        protected override void machine_start()
        {
            save_item(NAME(new { m_mcr_cocktail_flip }));
        }


        //void mcr_nflfoot_state::machine_start()


        protected override void machine_reset()
        {
            /* reset cocktail flip */
            m_mcr_cocktail_flip = 0;
        }


        /*************************************
         *
         *  Generic MCR interrupt handler
         *
         *************************************/
        //TIMER_DEVICE_CALLBACK_MEMBER(mcr_state::mcr_interrupt)
        void mcr_interrupt(timer_device timer, object ptr, s32 param)  //void *ptr, s32 param
        {
            int scanline = param;

            /* CTC line 2 is connected to VBLANK, which is once every 1/2 frame */
            /* for the 30Hz interlaced display */
            if (scanline == 0 || scanline == 240)
            {
                m_ctc.op0.trg2(1);
                m_ctc.op0.trg2(0);
            }

            /* CTC line 3 is connected to 493, which is signalled once every */
            /* frame at 30Hz */
            if (scanline == 0)
            {
                m_ctc.op0.trg3(1);
                m_ctc.op0.trg3(0);
            }
        }
    }


        //TIMER_DEVICE_CALLBACK_MEMBER(mcr_nflfoot_state::ipu_interrupt)


        //WRITE_LINE_MEMBER(mcr_nflfoot_state::sio_txda_w)

        //WRITE_LINE_MEMBER(mcr_nflfoot_state::sio_txdb_w)

        //void mcr_nflfoot_state::ipu_laserdisk_w(offs_t offset, uint8_t data)

        //TIMER_CALLBACK_MEMBER(mcr_nflfoot_state::ipu_watchdog_reset)

        //uint8_t mcr_nflfoot_state::ipu_watchdog_r()

        //void mcr_nflfoot_state::ipu_watchdog_w(uint8_t data)
}
