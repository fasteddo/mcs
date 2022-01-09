// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using u8 = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  Williams video startup
         *
         *************************************/

        //void williams_state::state_save_register()


        protected override void video_start()
        {
            throw new emu_unimplemented();
#if false
            blitter_init(m_blitter_config, nullptr);
            state_save_register();
#endif
        }
    }


        //void blaster_state::video_start()

        //void williams2_state::video_start()


    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  Williams video update
         *
         *************************************/

        protected virtual uint32_t screen_update(screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
#if false
            rgb_t pens[16];

            /* precompute the palette */
            for (int x = 0; x < 16; x++)
                pens[x] = m_palette->pen_color(m_paletteram[x]);

            /* loop over rows */
            for (int y = cliprect.min_y; y <= cliprect.max_y; y++)
            {
                uint8_t const *const source = &m_videoram[y];
                uint32_t *const dest = &bitmap.pix(y);

                /* loop over columns */
                for (int x = cliprect.min_x & ~1; x <= cliprect.max_x; x += 2)
                {
                    int const pix = source[(x/2) * 256];
                    dest[x+0] = pens[pix >> 4];
                    dest[x+1] = pens[pix & 0x0f];
                }
            }
            return 0;
#endif
        }
    }


        //uint32_t blaster_state::screen_update(screen_device &screen, bitmap_rgb32 &bitmap, const rectangle &cliprect)

        //uint32_t williams2_state::screen_update(screen_device &screen, bitmap_rgb32 &bitmap, const rectangle &cliprect)

        //uint32_t mysticm_state::screen_update(screen_device &screen, bitmap_rgb32 &bitmap, const rectangle &cliprect)


        /*************************************
         *
         *  Williams palette I/O
         *
         *************************************/

    partial class williams_state : driver_device
    {
        void palette_init(palette_device palette)
        {
            throw new emu_unimplemented();
        }
    }


        //rgb_t williams2_state::calc_col(uint16_t lo, uint16_t hi)

        //void williams2_state::paletteram_w(offs_t offset, u8 data)

        //void williams2_state::rebuild_palette()

        //void williams2_state::fg_select_w(u8 data)


    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  Video position readout
         *
         *************************************/

        protected virtual u8 video_counter_r()
        {
            throw new emu_unimplemented();
#if false
            if (m_screen->vpos() < 0x100)
                return m_screen->vpos() & 0xfc;
            else
                return 0xfc;
#endif
        }
    }


        //u8 williams2_state::video_counter_r()


        /*************************************
         *
         *  Tilemap handling
         *
         *************************************/

        //TILE_GET_INFO_MEMBER(williams2_state::get_tile_info)

        //int mysticm_state::color_decode(uint8_t base_col, int sig_J1, int y)

        //TILE_GET_INFO_MEMBER(mysticm_state::get_tile_info)

        //TILE_GET_INFO_MEMBER(joust2_state::get_tile_info)

        /* based on the board type, only certain bits are used */
        /* the rest are determined by other factors */

        //void williams2_state::bg_select_w(u8 data)

        //void mysticm_state::bg_select_w(u8 data)

        //void joust2_state::bg_select_w(u8 data)

        //void williams2_state::tileram_w(offs_t offset, u8 data)

        //void williams2_state::xscroll_low_w(u8 data)

        //void williams2_state::xscroll_high_w(u8 data)


        /*************************************
         *
         *  Blaster-specific enhancements
         *
         *************************************/

        //void blaster_state::remap_select_w(u8 data)

        //void blaster_state::video_control_w(u8 data)


        /*************************************
         *
         *  Blitter setup and control
         *
         *************************************/

        //void williams_state::blitter_init(int blitter_config, const uint8_t *remap_prom)

        //void williams_state::blitter_w(address_space &space, offs_t offset, u8 data)

        //void williams2_state::blit_window_enable_w(u8 data)


        /*************************************
         *
         *  Blitter core
         *
         *************************************/

        //inline void williams_state::blit_pixel(address_space &space, int dstaddr, int srcdata, int controlbyte)

        //int williams_state::blitter_core(address_space &space, int sstart, int dstart, int w, int h, int controlbyte)
}
