// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using tilemap_memory_index = System.UInt32;  //typedef u32 tilemap_memory_index;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;

using static mame.emucore_global;
using static mame.resnet_global;
using static mame.tilemap_global;
using static mame.util;


namespace mame
{
    partial class cclimber_state : driver_device
    {
        const uint64_t CCLIMBER_BG_PEN     = 0;
        //#define SWIMMER_SIDE_BG_PEN (0x120)
        //#define SWIMMER_BG_SPLIT    (0x18 * 8)
        //#define YAMATO_SKY_PEN_BASE (0x60)


        /***************************************************************************

          Convert the color PROMs into a more useable format.

          Crazy Climber has three 32x8 palette PROMs.
          The palette PROMs are connected to the RGB output this way:

          bit 7 -- 220 ohm resistor  -- BLUE
                -- 470 ohm resistor  -- BLUE
                -- 220 ohm resistor  -- GREEN
                -- 470 ohm resistor  -- GREEN
                -- 1  kohm resistor  -- GREEN
                -- 220 ohm resistor  -- RED
                -- 470 ohm resistor  -- RED
          bit 0 -- 1  kohm resistor  -- RED

        ***************************************************************************/
        void cclimber_palette(palette_device palette)
        {
            Pointer<uint8_t> color_prom = new Pointer<uint8_t>(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();
            int [] resistances_rg = new int [3] { 1000, 470, 220 };
            int [] resistances_b = new int [2] { 470, 220 };

            // compute the color output resistor weights
            double [] weights_rg = new double [3];
            double [] weights_b = new double [2];
            compute_resistor_weights(0, 255, -1.0,
                    3, resistances_rg, out weights_rg, 0, 0,
                    2, resistances_b,  out weights_b,  0, 0,
                    0, null, out _, 0, 0);

            for (int i = 0;i < palette.dipalette.entries(); i++)
            {
                int bit0;
                int bit1;
                int bit2;

                // red component
                bit0 = BIT(color_prom[i], 0);
                bit1 = BIT(color_prom[i], 1);
                bit2 = BIT(color_prom[i], 2);
                int r = combine_weights(weights_rg, bit0, bit1, bit2);

                // green component
                bit0 = BIT(color_prom[i], 3);
                bit1 = BIT(color_prom[i], 4);
                bit2 = BIT(color_prom[i], 5);
                int g = combine_weights(weights_rg, bit0, bit1, bit2);

                // blue component
                bit0 = BIT(color_prom[i], 6);
                bit1 = BIT(color_prom[i], 7);
                int b = combine_weights(weights_b, bit0, bit1);

                palette.set_pen_color((pen_t)i, new rgb_t((uint8_t)r, (uint8_t)g, (uint8_t)b));
            }
        }


        /***************************************************************************

          Convert the color PROMs into a more useable format.

          Swimmer has two 256x4 char/sprite palette PROMs and one 32x8 big sprite
          palette PROM.
          The palette PROMs are connected to the RGB output this way:
          (the 500 and 250 ohm resistors are made of 1 kohm resistors in parallel)

          bit 3 -- 250 ohm resistor  -- BLUE
                -- 500 ohm resistor  -- BLUE
                -- 250 ohm resistor  -- GREEN
          bit 0 -- 500 ohm resistor  -- GREEN
          bit 3 -- 1  kohm resistor  -- GREEN
                -- 250 ohm resistor  -- RED
                -- 500 ohm resistor  -- RED
          bit 0 -- 1  kohm resistor  -- RED

          bit 7 -- 250 ohm resistor  -- BLUE
                -- 500 ohm resistor  -- BLUE
                -- 250 ohm resistor  -- GREEN
                -- 500 ohm resistor  -- GREEN
                -- 1  kohm resistor  -- GREEN
                -- 250 ohm resistor  -- RED
                -- 500 ohm resistor  -- RED
          bit 0 -- 1  kohm resistor  -- RED

          Additionally, the background color of the score panel is determined by
          these resistors:

                          /--- tri-state --  470 -- BLUE
          +5V -- 1kohm ------- tri-state --  390 -- GREEN
                          \--- tri-state -- 1000 -- RED

        ***************************************************************************/

        //void cclimber_state::swimmer_palette(palette_device &palette) const

        //void cclimber_state::yamato_palette(palette_device &palette) const

        //void cclimber_state::toprollr_palette(palette_device &palette) const


        /***************************************************************************

          Swimmer can directly set the background color.
          The latch is connected to the RGB output this way:
          (the 500 and 250 ohm resistors are made of 1 kohm resistors in parallel)

          bit 7 -- 250 ohm resistor  -- RED
                -- 500 ohm resistor  -- RED
                -- 250 ohm resistor  -- GREEN
                -- 500 ohm resistor  -- GREEN
                -- 1  kohm resistor  -- GREEN
                -- 250 ohm resistor  -- BLUE
                -- 500 ohm resistor  -- BLUE
          bit 0 -- 1  kohm resistor  -- BLUE

        ***************************************************************************/

        //void cclimber_state::swimmer_set_background_pen()


        void cclimber_colorram_w(offs_t offset, uint8_t data)
        {
            /* A5 is not connected, there is only 0x200 bytes of RAM */
            m_colorram[(int)offset & ~0x20].op = data;
            m_colorram[offset |  0x20].op = data;
        }


        void flip_screen_x_w(int state)
        {
            m_flip_x = state != 0;
        }


        void flip_screen_y_w(int state)
        {
            m_flip_y = state != 0;
        }


        //void cclimber_state::sidebg_enable_w(int state)

        //void cclimber_state::palette_bank_w(int state)


        //TILE_GET_INFO_MEMBER(cclimber_state::cclimber_get_pf_tile_info)
        void cclimber_get_pf_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int code;
            int color;

            int flags = TILE_FLIPYX(m_colorram.op[tile_index] >> 6);

            /* vertical flipping flips two adjacent characters */
            if ((flags & 0x02) != 0)
                tile_index = tile_index ^ 0x20;

            code = ((m_colorram.op[tile_index] & 0x10) << 5) |
                    ((m_colorram.op[tile_index] & 0x20) << 3) |
                        m_videoram.op[tile_index];

            color = m_colorram.op[tile_index] & 0x0f;

            tileinfo.set(0, (uint32_t)code, (uint32_t)color, (uint8_t)flags);
        }


        //TILE_GET_INFO_MEMBER(cclimber_state::swimmer_get_pf_tile_info)

        //TILE_GET_INFO_MEMBER(cclimber_state::toprollr_get_pf_tile_info)


        //TILE_GET_INFO_MEMBER(cclimber_state::cclimber_get_bs_tile_info)
        void cclimber_get_bs_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int code;
            int color;

            /* only the lower right is visible */
            tileinfo.group = ((tile_index & 0x210) == 0x210) ? (uint8_t)0 : (uint8_t)1;

            /* the address doesn't use A4 of the coordinates, giving a 16x16 map */
            tile_index = ((tile_index & 0x1e0) >> 1) | (tile_index & 0x0f);

            code = ((m_bigsprite_control.op[1] & 0x08) << 5) | m_bigsprite_videoram.op[tile_index];
            color = m_bigsprite_control.op[1] & 0x07;

            tileinfo.set(2, (uint32_t)code, (uint32_t)color, 0);
        }


        //TILE_GET_INFO_MEMBER(cclimber_state::toprollr_get_bs_tile_info)

        //TILE_GET_INFO_MEMBER(cclimber_state::toproller_get_bg_tile_info)


        //VIDEO_START_MEMBER(cclimber_state,cclimber)
        void video_start_cclimber()
        {
            m_pf_tilemap = machine().tilemap().create(m_gfxdecode.op0, cclimber_get_pf_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8, 8, 32, 32);
            m_pf_tilemap.set_transparent_pen(0);
            m_pf_tilemap.set_scroll_cols(32);

            m_bs_tilemap = machine().tilemap().create(m_gfxdecode.op0, cclimber_get_bs_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8, 8, 32, 32);
            m_bs_tilemap.set_scroll_cols(1);
            m_bs_tilemap.set_scroll_rows(1);
            m_bs_tilemap.set_transmask(0, 0x01, 0);    // pen 0 is transparent
            m_bs_tilemap.set_transmask(1, 0x0f, 0);  // all 4 pens are transparent

            save_item(NAME(new { m_flip_x }));
            save_item(NAME(new { m_flip_y }));
        }


        //VIDEO_START_MEMBER(cclimber_state,swimmer)

        //VIDEO_START_MEMBER(cclimber_state,toprollr)


        void draw_playfield(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            m_pf_tilemap.mark_all_dirty();
            m_pf_tilemap.set_flip((m_flip_x ? TILEMAP_FLIPX : 0) | (m_flip_y ? TILEMAP_FLIPY : 0));
            for (int i = 0; i < 32; i++)
                m_pf_tilemap.set_scrolly(i, m_column_scroll.op[i]);

            m_pf_tilemap.draw(screen, bitmap, cliprect, 0, 0);
        }


        void cclimber_draw_bigsprite(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            uint8_t x = (uint8_t)(m_bigsprite_control.op[3] - 8);
            uint8_t y = m_bigsprite_control.op[2];
            int bigsprite_flip_x = (m_bigsprite_control.op[1] & 0x10) >> 4;
            int bigsprite_flip_y = (m_bigsprite_control.op[1] & 0x20) >> 5;

            if (bigsprite_flip_x != 0)
                x = (uint8_t)(0x80 - x);

            if (bigsprite_flip_y != 0)
                y = (uint8_t)(0x80 - y);

            m_bs_tilemap.mark_all_dirty();

            m_bs_tilemap.set_flip((bigsprite_flip_x != 0 ? TILEMAP_FLIPX : 0) | (((m_flip_y ? 1 : 0) ^ bigsprite_flip_y) != 0 ? TILEMAP_FLIPY : 0));

            m_bs_tilemap.set_scrollx(0, x);
            m_bs_tilemap.set_scrolly(0, y);

            m_bs_tilemap.draw(screen, bitmap, cliprect, 0, 0);
        }


        //void cclimber_state::toprollr_draw_bigsprite(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)


        void cclimber_draw_sprites(bitmap_ind16 bitmap, rectangle cliprect, gfx_element gfx)
        {
            int offs;

            /* draw the sprites -- note that it is important to draw them exactly in this
               order, to have the correct priorities. */
            for (offs = 0x1c; offs >= 0; offs -= 4)
            {
                int x = m_spriteram.op[offs + 3] + 1;
                /* x + 1 is evident in cclimber and ckong. It looks worse,
                but it has been confirmed on several PCBs. */

                int y = 240 - m_spriteram.op[offs + 2];

                int code = ((m_spriteram.op[offs + 1] & 0x10) << 3) |
                            ((m_spriteram.op[offs + 1] & 0x20) << 1) |
                            ( m_spriteram.op[offs + 0] & 0x3f);

                int color = m_spriteram.op[offs + 1] & 0x0f;

                int flipx = m_spriteram.op[offs + 0] & 0x40;
                int flipy = m_spriteram.op[offs + 0] & 0x80;

                if (m_flip_x)
                {
                    x = 242 - x;
                    flipx = (flipx == 0) ? 1 : 0;
                }

                if (m_flip_y)
                {
                    y = 240 - y;
                    flipy = (flipy == 0) ? 1 : 0;
                }

                gfx.transpen(bitmap, cliprect, (uint32_t)code, (uint32_t)color, flipx, flipy, x, y, 0);
            }
        }


        //void cclimber_state::toprollr_draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect, gfx_element *gfx)

        //void cclimber_state::swimmer_draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect, gfx_element *gfx)


        uint32_t screen_update_cclimber(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            bitmap.fill(CCLIMBER_BG_PEN, cliprect);
            draw_playfield(screen, bitmap, cliprect);

            /* draw the "big sprite" under the regular sprites */
            if ((m_bigsprite_control.op[0] & 0x01) != 0)
            {
                cclimber_draw_bigsprite(screen, bitmap, cliprect);
                cclimber_draw_sprites(bitmap, cliprect, m_gfxdecode.op0.gfx(1));
            }

            /* draw the "big sprite" over the regular sprites */
            else
            {
                cclimber_draw_sprites(bitmap, cliprect, m_gfxdecode.op0.gfx(1));
                cclimber_draw_bigsprite(screen, bitmap, cliprect);
            }

            return 0;
        }


        //uint32_t cclimber_state::screen_update_yamato(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t cclimber_state::screen_update_swimmer(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t cclimber_state::screen_update_toprollr(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)
    }
}
