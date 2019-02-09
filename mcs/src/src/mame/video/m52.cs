// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using pen_t = System.UInt32;
using tilemap_memory_index = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    partial class m52_state : driver_device
    {
        const int BGHEIGHT = 128;


        /*************************************
         *
         *  Palette configuration
         *
         *************************************/
        void init_palette()
        {
            int [] resistances_3 = new int [3] { 1000, 470, 220 };
            int [] resistances_2 = new int [2] { 470, 220 };
            double [] weights_r = new double[3];
            double [] weights_g = new double[3];
            double [] weights_b = new double[3];
            double scale;

            /* compute palette information for characters/backgrounds */
            scale = compute_resistor_weights(0, 255, -1.0,
                3, resistances_3, out weights_r, 0, 0,
                3, resistances_3, out weights_g, 0, 0,
                2, resistances_2, out weights_b, 0, 0);

            /* character palette */
            var char_pal = memregion("tx_pal").base_();
            for (int i = 0; i < 512; i++)
            {
                uint8_t promval = char_pal[i];
                int r = combine_3_weights(weights_r, BIT(promval, 0), BIT(promval, 1), BIT(promval, 2));
                int g = combine_3_weights(weights_g, BIT(promval, 3), BIT(promval, 4), BIT(promval, 5));
                int b = combine_2_weights(weights_b, BIT(promval, 6), BIT(promval, 7));

                m_tx_palette.target.palette_interface.set_pen_color((pen_t)i, new rgb_t((uint8_t)r, (uint8_t)g, (uint8_t)b));
            }

            /* background palette */
            var back_pal = memregion("bg_pal").base_();
            for (int i = 0; i < 32; i++)
            {
                uint8_t promval = back_pal[i];
                int r = combine_3_weights(weights_r, BIT(promval, 0), BIT(promval, 1), BIT(promval, 2));
                int g = combine_3_weights(weights_g, BIT(promval, 3), BIT(promval, 4), BIT(promval, 5));
                int b = combine_2_weights(weights_b, BIT(promval, 6), BIT(promval, 7));

                m_bg_palette.target.palette_interface.set_indirect_color(i, new rgb_t((uint8_t)r, (uint8_t)g, (uint8_t)b));
            }

            /* background
             the palette is a 32x8 PROM with many colors repeated. The address of
             the colors to pick is as follows:
             xbb00: mountains
             0xxbb: hills
             1xxbb: city

             this seems hacky, surely all bytes in the PROM should be used, not just picking the ones that give the colours we want?

             */
            m_bg_palette.target.palette_interface.set_pen_indirect(0 * 4 + 0, 0);
            m_bg_palette.target.palette_interface.set_pen_indirect(0 * 4 + 1, 4);
            m_bg_palette.target.palette_interface.set_pen_indirect(0 * 4 + 2, 8);
            m_bg_palette.target.palette_interface.set_pen_indirect(0 * 4 + 3, 12);
            m_bg_palette.target.palette_interface.set_pen_indirect(1 * 4 + 0, 0);
            m_bg_palette.target.palette_interface.set_pen_indirect(1 * 4 + 1, 1);
            m_bg_palette.target.palette_interface.set_pen_indirect(1 * 4 + 2, 2);
            m_bg_palette.target.palette_interface.set_pen_indirect(1 * 4 + 3, 3);
            m_bg_palette.target.palette_interface.set_pen_indirect(2 * 4 + 0, 0);
            m_bg_palette.target.palette_interface.set_pen_indirect(2 * 4 + 1, 16 + 1);
            m_bg_palette.target.palette_interface.set_pen_indirect(2 * 4 + 2, 16 + 2);
            m_bg_palette.target.palette_interface.set_pen_indirect(2 * 4 + 3, 16 + 3);

            init_sprite_palette(resistances_3, resistances_2, weights_r, weights_g, weights_b, scale);
        }


        void init_sprite_palette(int [] resistances_3, int [] resistances_2, double [] weights_r, double [] weights_g, double [] weights_b, double scale)
        {
            var sprite_pal = memregion("spr_pal").base_();
            var sprite_table = memregion("spr_clut").base_();

            /* compute palette information for sprites */
            compute_resistor_weights(0, 255, scale,
                2, resistances_2, out weights_r, 470, 0,
                3, resistances_3, out weights_g, 470, 0,
                3, resistances_3, out weights_b, 470, 0);

            /* sprite palette */
            for (int i = 0; i < 32; i++)
            {
                uint8_t promval = sprite_pal[i];
                int r = combine_2_weights(weights_r, BIT(promval, 6), BIT(promval, 7));
                int g = combine_3_weights(weights_g, BIT(promval, 3), BIT(promval, 4), BIT(promval, 5));
                int b = combine_3_weights(weights_b, BIT(promval, 0), BIT(promval, 1), BIT(promval, 2));

                m_sp_palette.target.palette_interface.set_indirect_color(i, new rgb_t((uint8_t)r, (uint8_t)g, (uint8_t)b));
            }

            /* sprite lookup table */
            for (int i = 0; i < 256; i++)
            {
                uint8_t promval = sprite_table[i];
                m_sp_palette.target.palette_interface.set_pen_indirect((pen_t)i, promval);
            }
        }


        /*************************************
         *
         *  Tilemap info callback
         *
         *************************************/
        //TILE_GET_INFO_MEMBER(m52_state::get_tile_info)
        void get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            uint8_t video = m_videoram.target[tile_index];
            uint8_t color = m_colorram.target[tile_index];

            int flag = 0;
            int code = 0;

            code = video;

            if ((color & 0x80) != 0)
            {
                code |= 0x100;
            }

            if (tile_index / 32 <= 6)
            {
                flag |= tilemap_global.TILE_FORCE_LAYER0; /* lines 0 to 6 are opaqe? */
            }

            tilemap_global.SET_TILE_INFO_MEMBER(ref tileinfo, 0, (UInt32)code, (UInt32)(color & 0x7f), (byte)flag);
        }


        /*************************************
         *
         *  Video startup
         *
         *************************************/
        protected override void video_start()
        {
            m_tx_tilemap = machine().tilemap().create(m_tx_gfxdecode.target.digfx, get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS,  8, 8, 32, 32);

            m_tx_tilemap.set_transparent_pen(0);
            m_tx_tilemap.set_scrolldx(127, 127);
            m_tx_tilemap.set_scrolldy(16, 16);
            m_tx_tilemap.set_scroll_rows(4); /* only lines 192-256 scroll */

            init_palette();

            save_item(m_bg1xpos, "m_bg1xpos");
            save_item(m_bg1ypos, "m_bg1ypos");
            save_item(m_bg2xpos, "m_bg2xpos");
            save_item(m_bg2ypos, "m_bg2ypos");
            save_item(m_bgcontrol, "m_bgcontrol");

            m_spritelimit = 0x100-4;
            m_do_bg_fills = true;
        }


        /*************************************
         *
         *  Scroll registers
         *
         *************************************/

        //WRITE8_MEMBER(m52_state::m52_scroll_w)
        public void m52_scroll_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
        /*
            According to the schematics there is only one video register that holds the X scroll value
            with a NAND gate on the V64 and V128 lines to control when it's read, and when
            255 (via 8 pull up resistors) is used.

            So we set the first 3 quarters to 255 and the last to the scroll value
        */
            m_tx_tilemap.set_scrollx(0, 255);
            m_tx_tilemap.set_scrollx(1, 255);
            m_tx_tilemap.set_scrollx(2, 255);
            m_tx_tilemap.set_scrollx(3, -(data + 1));
        }


        /*************************************
         *
         *  Video RAM write handlers
         *
         *************************************/

        //WRITE8_MEMBER(m52_state::m52_videoram_w)
        public void m52_videoram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_videoram.target[offset] = data;
            m_tx_tilemap.mark_tile_dirty(offset);
        }


        //WRITE8_MEMBER(m52_state::m52_colorram_w)
        public void m52_colorram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_colorram.target[offset] = data;
            m_tx_tilemap.mark_tile_dirty(offset);
        }


        /*************************************
         *
         *  Custom protection
         *
         *************************************/

        /* This looks like some kind of protection implemented by a custom chip on the
           scroll board. It mangles the value written to the port m52_bg1xpos_w, as
           follows: result = popcount(value & 0x7f) ^ (value >> 7) */
        //READ8_MEMBER(m52_state::m52_protection_r)
        public u8 m52_protection_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            int popcount = 0;
            int temp;

            for (temp = m_bg1xpos & 0x7f; temp != 0; temp >>= 1)
                popcount += temp & 1;

            return (u8)(popcount ^ (m_bg1xpos >> 7));
        }


        /*************************************
         *
         *  Background control write handlers
         *
         *************************************/

        //WRITE8_MEMBER(m52_state::m52_bg1ypos_w)
        public void m52_bg1ypos_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_bg1ypos = data;
        }

        //WRITE8_MEMBER(m52_state::m52_bg1xpos_w)
        public void m52_bg1xpos_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_bg1xpos = data;
        }

        //WRITE8_MEMBER(m52_state::m52_bg2xpos_w)
        public void m52_bg2xpos_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_bg2xpos = data;
        }

        //WRITE8_MEMBER(m52_state::m52_bg2ypos_w)
        public void m52_bg2ypos_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_bg2ypos = data;
        }

        //WRITE8_MEMBER(m52_state::m52_bgcontrol_w)
        public void m52_bgcontrol_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_bgcontrol = data;
        }


        /*************************************
         *
         *  Outputs
         *
         *************************************/

        //WRITE8_MEMBER(m52_state::m52_flipscreen_w)
        public void m52_flipscreen_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* screen flip is handled both by software and hardware */
            flip_screen_set((u32)((data & 0x01) ^ (~ioport("DSW2").read() & 0x01)));

            machine().bookkeeping().coin_counter_w(0, data & 0x02);
            machine().bookkeeping().coin_counter_w(1, data & 0x20);
        }


        /*************************************
         *
         *  Background rendering
         *
         *************************************/

        void draw_background(bitmap_rgb32 bitmap, rectangle cliprect, int xpos, int ypos, int image)
        {
            rectangle rect = new rectangle();
            rectangle visarea = m_screen.target.visible_area();
            var paldata = m_bg_palette.target.device_palette_interface.pens();


            if (flip_screen() != 0)
            {
                xpos = 264 - xpos;
                ypos = 264 - ypos - BGHEIGHT;
            }

            xpos += 124;

            /* this may not be correct */
            ypos += 16;


            m_bg_gfxdecode.target.digfx.gfx(image).transpen(bitmap, cliprect,
                0, 0,
                (int)flip_screen(),
                (int)flip_screen(),
                xpos,
                ypos, 0);


            m_bg_gfxdecode.target.digfx.gfx(image).transpen(bitmap, cliprect,
                0, 0,
                (int)flip_screen(),
                (int)flip_screen(),
                xpos - 256,
                ypos, 0);

            // create a solid fill below the 64 pixel high bg images
            if (m_do_bg_fills)
            {
                rect.min_x = visarea.min_x;
                rect.max_x = visarea.max_x;

                if (flip_screen() != 0)
                {
                    rect.min_y = ypos - BGHEIGHT;
                    rect.max_y = ypos - 1;
                }
                else
                {
                    rect.min_y = ypos + BGHEIGHT;
                    rect.max_y = ypos + 2 * BGHEIGHT - 1;
                }

                bitmap.fill(paldata[m_bg_gfxdecode.target.digfx.gfx(image).colorbase() + 3], rect);
            }
        }


        /*************************************
         *
         *  Sprites rendering
         *
         *************************************/

        void draw_sprites(bitmap_rgb32 bitmap, rectangle cliprect, int initoffs)
        {
            int offs;

            /* draw the sprites */
            for (offs = initoffs; offs >= (initoffs & 0xc0); offs -= 4)
            {
                int sy = 257 - m_spriteram.target[offs];
                int color = m_spriteram.target[offs + 1] & 0x3f;
                int flipx = m_spriteram.target[offs + 1] & 0x40;
                int flipy = m_spriteram.target[offs + 1] & 0x80;
                int code = m_spriteram.target[offs + 2];
                int sx = m_spriteram.target[offs + 3];

                /* sprites from offsets $00-$7F are processed in the upper half of the frame */
                /* sprites from offsets $80-$FF are processed in the lower half of the frame */
                rectangle clip = cliprect;
                if (!((offs & 0x80) != 0))
                {
                    clip.min_y = 0;
                    clip.max_y = 127;
                }
                else
                {
                    clip.min_y = 128;
                    clip.max_y = 255;
                }

                /* adjust for flipping */
                if (flip_screen() != 0)
                {
                    int temp = clip.min_y;
                    clip.min_y = 255 - clip.max_y;
                    clip.max_y = 255 - temp;
                    flipx = (!(flipx != 0)) ? 1 : 0;
                    flipy = (!(flipy != 0)) ? 1 : 0;
                    sx = 238 - sx;
                    sy = 282 - sy;
                }

                sx += 129;

                /* in theory anyways; in practice, some of the molecule-looking guys get clipped */
#if SPLIT_SPRITES
                sect_rect(&clip, cliprect);
#else
                clip = cliprect;
#endif

                m_sp_gfxdecode.target.digfx.gfx(0).transmask(bitmap, clip,
                    (u32)code, (u32)color, flipx, flipy, sx, sy,
                    m_sp_palette.target.palette_interface.transpen_mask(m_sp_gfxdecode.target.digfx.gfx(0), (u32)color,  0));
            }
        }


        /*************************************
         *
         *  Video render
         *
         *************************************/

        public uint32_t screen_update_m52(screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect)
        {
            int offs;
            var paldata = m_sp_palette.target.device_palette_interface.pens();

            bitmap.fill(paldata[0], cliprect);

            if (!((m_bgcontrol & 0x20) != 0))
            {
                if (!((m_bgcontrol & 0x10) != 0))
                    draw_background(bitmap, cliprect, m_bg2xpos, m_bg2ypos, 0); /* distant mountains */

                // only one of these be drawn at once (they share the same scroll register) (alpha1v leaves everything enabled)
                if (!((m_bgcontrol & 0x02) != 0))
                    draw_background(bitmap, cliprect, m_bg1xpos, m_bg1ypos, 1); /* hills */
                else if (!((m_bgcontrol & 0x04) != 0))
                    draw_background(bitmap, cliprect, m_bg1xpos, m_bg1ypos, 2); /* cityscape */
            }

            m_tx_tilemap.set_flip((flip_screen() != 0) ? TILEMAP_FLIPX | TILEMAP_FLIPY : 0);

            m_tx_tilemap.draw(screen, bitmap, cliprect, 0, 0);

            /* draw the sprites */
            for (offs = 0x3c; offs <= m_spritelimit; offs += 0x40)
                draw_sprites(bitmap, cliprect, offs);

            return 0;
        }
    }
}
