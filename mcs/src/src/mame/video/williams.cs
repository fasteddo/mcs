// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using PointerU8 = mame.Pointer<System.Byte>;
using u8 = System.Byte;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.emucore_global;
using static mame.resnet_global;
using static mame.util;
using static mame.williams_global;


namespace mame
{
    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  Williams video startup
         *
         *************************************/

        void state_save_register()
        {
            save_item(NAME(new { m_blitter_window_enable }));
            save_item(NAME(new { m_cocktail }));
            save_item(NAME(new { m_blitterram }));
            save_item(NAME(new { m_blitter_remap_index }));
        }


        protected override void video_start()
        {
            blitter_init(m_blitter_config, null);
            state_save_register();
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


    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  Williams palette I/O
         *
         *************************************/
        void palette_init(palette_device palette)
        {
            int [] resistances_rg = new int [3] { 1200, 560, 330 };  //static constexpr int resistances_rg[3] = { 1200, 560, 330 };
            int [] resistances_b = new int [2]  { 560, 330 };

            // compute palette information
            // note that there really are pullup/pulldown resistors, but this situation is complicated
            // by the use of transistors, so we ignore that and just use the relative resistor weights
            double [] weights_r = new double [3];
            double [] weights_g = new double [3];
            double [] weights_b = new double [2];
            compute_resistor_weights(0, 255, -1.0,
                    3, resistances_rg, out weights_r, 0, 0,
                    3, resistances_rg, out weights_g, 0, 0,
                    2, resistances_b,  out weights_b, 0, 0);

            // build a palette lookup
            for (int i = 0; i < 256; i++)
            {
                int r = combine_weights(weights_r, BIT(i, 0), BIT(i, 1), BIT(i, 2));
                int g = combine_weights(weights_g, BIT(i, 3), BIT(i, 4), BIT(i, 5));
                int b = combine_weights(weights_b, BIT(i, 6), BIT(i, 7));

                palette.set_pen_color((pen_t)i, new rgb_t((u8)r, (u8)g, (u8)b));
            }
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


    partial class williams_state : driver_device
    {
        /*************************************
         *
         *  Blitter setup and control
         *
         *************************************/

        void blitter_init(int blitter_config, PointerU8 remap_prom)  //void williams_state::blitter_init(int blitter_config, const uint8_t *remap_prom)
        {
            std.fill(m_blitterram, (u8)0);
            MemoryU8 dummy_table = new MemoryU8() { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 };  //static const uint8_t dummy_table[] = { 0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15 };

            /* by default, there is no clipping window - this will be touched only by games that have one */
            m_blitter_window_enable = 0;

            /* switch off the video config */
            m_blitter_xor = (blitter_config == WILLIAMS_BLITTER_SC1) ? (uint8_t)4 : (uint8_t)0;

            /* create the remap table; if no PROM, make an identity remap table */
            m_blitter_remap_lookup = new MemoryU8(256 * 256, true);  //m_blitter_remap_lookup = std::make_unique<uint8_t[]>(256 * 256);
            m_blitter_remap_index = 0;
            m_blitter_remap = new PointerU8(m_blitter_remap_lookup);
            for (int i = 0; i < 256; i++)
            {
                PointerU8 table = remap_prom != null ? (remap_prom + (i & 0x7f) * 16) : new PointerU8(dummy_table);  //const uint8_t *table = remap_prom ? (remap_prom + (i & 0x7f) * 16) : dummy_table;
                for (int j = 0; j < 256; j++)
                    m_blitter_remap_lookup[i * 256 + j] = (u8)((table[j >> 4] << 4) | table[j & 0x0f]);
            }
        }


        void blitter_w(address_space space, offs_t offset, u8 data)
        {
            throw new emu_unimplemented();
        }
    }


        //void williams2_state::blit_window_enable_w(u8 data)


        /*************************************
         *
         *  Blitter core
         *
         *************************************/

        //inline void williams_state::blit_pixel(address_space &space, int dstaddr, int srcdata, int controlbyte)

        //int williams_state::blitter_core(address_space &space, int sstart, int dstart, int w, int h, int controlbyte)
}
