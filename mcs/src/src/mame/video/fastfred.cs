// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using indirect_pen_t = System.UInt16;  //typedef u16 indirect_pen_t;
using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using tilemap_memory_index = System.UInt32;  //typedef u32 tilemap_memory_index;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.resnet_global;
using static mame.tilemap_global;
using static mame.util;


namespace mame
{
    partial class fastfred_state : galaxold_state
    {
        /***************************************************************************

          Convert the color PROMs into a more useable format.

          bit 0 -- 1  kohm resistor  -- RED/GREEN/BLUE
                -- 470 ohm resistor  -- RED/GREEN/BLUE
                -- 220 ohm resistor  -- RED/GREEN/BLUE
          bit 3 -- 100 ohm resistor  -- RED/GREEN/BLUE

        ***************************************************************************/
        void fastfred_palette(palette_device palette)
        {
            Pointer<uint8_t> color_prom = new Pointer<uint8_t>(memregion("proms").base_());  //uint8_t const *const color_prom = memregion("proms")->base();
            int [] resistances = new int [4] { 1000, 470, 220, 100 };  //static constexpr int resistances[4] = { 1000, 470, 220, 100 };

            // compute the color output resistor weights
            double [] rweights = new double [4];
            double [] gweights = new double [4];
            double [] bweights = new double [4];
            compute_resistor_weights(0, 255, -1.0,
                    4, resistances, out rweights, 470, 0,
                    4, resistances, out gweights, 470, 0,
                    4, resistances, out bweights, 470, 0);

            // create a lookup table for the palette
            for (int i = 0; i < 0x100; i++)
            {
                int bit0;
                int bit1;
                int bit2;
                int bit3;

                // red component
                bit0 = BIT(color_prom[i | 0x000], 0);
                bit1 = BIT(color_prom[i | 0x000], 1);
                bit2 = BIT(color_prom[i | 0x000], 2);
                bit3 = BIT(color_prom[i | 0x000], 3);
                int r = combine_weights(rweights, bit0, bit1, bit2, bit3);

                // green component
                bit0 = BIT(color_prom[i | 0x100], 0);
                bit1 = BIT(color_prom[i | 0x100], 1);
                bit2 = BIT(color_prom[i | 0x100], 2);
                bit3 = BIT(color_prom[i | 0x100], 3);
                int g = combine_weights(gweights, bit0, bit1, bit2, bit3);

                // blue component
                bit0 = BIT(color_prom[i | 0x200], 0);
                bit1 = BIT(color_prom[i | 0x200], 1);
                bit2 = BIT(color_prom[i | 0x200], 2);
                bit3 = BIT(color_prom[i | 0x200], 3);
                int b = combine_weights(bweights, bit0, bit1, bit2, bit3);

                palette.set_indirect_color(i, new rgb_t((uint8_t)r, (uint8_t)g, (uint8_t)b));
            }

            // characters and sprites use the same palette
            for (int i = 0; i < 0x100; i++)
                palette.set_pen_indirect((pen_t)i, (indirect_pen_t)i);
        }


        /***************************************************************************

          Callbacks for the TileMap code

        ***************************************************************************/
        //TILE_GET_INFO_MEMBER(fastfred_state::get_tile_info)
        void get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            uint8_t x = (uint8_t)(tile_index & 0x1f);

            uint16_t code = (uint16_t)(m_charbank | m_videoram.op[tile_index]);
            uint8_t color = (uint8_t)(m_colorbank | (m_attributesram.op[2 * x + 1] & 0x07));

            tileinfo.set(0, code, color, 0);
        }


        /*************************************
         *
         *  Video system start
         *
         *************************************/
        //VIDEO_START_MEMBER(fastfred_state,fastfred)
        void video_start_fastfred()
        {
            m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op0, get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8, 8, 32, 32);

            m_bg_tilemap.set_transparent_pen(0);
            m_bg_tilemap.set_scroll_cols(32);
        }


        /*************************************
         *
         *  Memory handlers
         *
         *************************************/

        void fastfred_videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram.op[offset] = data;
            m_bg_tilemap.mark_tile_dirty(offset);
        }


        void fastfred_attributes_w(offs_t offset, uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            if (m_attributesram[offset] != data)
            {
                if (offset & 0x01)
                {
                    /* color change */
                    int i;

                    for (i = offset / 2; i < 0x0400; i += 32)
                        m_bg_tilemap->mark_tile_dirty(i);
                }
                else
                {
                    /* coloumn scroll */
                    m_bg_tilemap->set_scrolly(offset / 2, data);
                }

                m_attributesram[offset] = data;
            }
#endif
        }


        //WRITE_LINE_MEMBER(fastfred_state::charbank1_w)
        void charbank1_w(int state)
        {
            uint16_t new_data = (uint16_t)((m_charbank & 0x0200) | (state << 8));

            if (new_data != m_charbank)
            {
                m_bg_tilemap.mark_all_dirty();

                m_charbank = new_data;
            }
        }


        //WRITE_LINE_MEMBER(fastfred_state::charbank2_w)
        void charbank2_w(int state)
        {
            uint16_t new_data = (uint16_t)((m_charbank & 0x0100) | (state << 9));

            if (new_data != m_charbank)
            {
                m_bg_tilemap.mark_all_dirty();

                m_charbank = new_data;
            }
        }


        //WRITE_LINE_MEMBER(fastfred_state::colorbank1_w)
        void colorbank1_w(int state)
        {
            uint8_t new_data = (uint8_t)((m_colorbank & 0x10) | (state << 3));

            if (new_data != m_colorbank)
            {
                m_bg_tilemap.mark_all_dirty();

                m_colorbank = new_data;
            }
        }


        //WRITE_LINE_MEMBER(fastfred_state::colorbank2_w)
        void colorbank2_w(int state)
        {
            uint8_t new_data = (uint8_t)((m_colorbank & 0x08) | (state << 4));

            if (new_data != m_colorbank)
            {
                m_bg_tilemap.mark_all_dirty();

                m_colorbank = new_data;
            }
        }


        //WRITE_LINE_MEMBER(fastfred_state::flip_screen_x_w)
        void flip_screen_x_w(int state)
        {
            flip_screen_x_set((uint32_t)state);

            m_bg_tilemap.set_flip((flip_screen_x() != 0 ? TILEMAP_FLIPX : 0) | (flip_screen_y() != 0 ? TILEMAP_FLIPY : 0));
        }


        //WRITE_LINE_MEMBER(fastfred_state::flip_screen_y_w)
        void flip_screen_y_w(int state)
        {
            flip_screen_y_set((uint32_t)state);

            m_bg_tilemap.set_flip((flip_screen_x() != 0 ? TILEMAP_FLIPX : 0) | (flip_screen_y() != 0 ? TILEMAP_FLIPY : 0));
        }


        /*************************************
         *
         *  Video update
         *
         *************************************/
        void draw_sprites(bitmap_ind16 bitmap, rectangle cliprect)
        {
            rectangle spritevisiblearea = new rectangle(2 * 8, 32 * 8 - 1, 2 * 8, 30 * 8 - 1);  //const rectangle spritevisiblearea(2*8, 32*8-1, 2*8, 30*8-1);
            rectangle spritevisibleareaflipx = new rectangle(0 * 8, 30 * 8 - 1, 2 * 8, 30 * 8 - 1);  //const rectangle spritevisibleareaflipx(0*8, 30*8-1, 2*8, 30*8-1);
            int offs;

            for (offs = (int)m_spriteram.bytes() - 4; offs >= 0; offs -= 4)
            {
                uint8_t code;
                uint8_t sx;
                uint8_t sy;
                int flipx;
                int flipy;

                sx = m_spriteram.op[offs + 3];
                sy = (uint8_t)(240 - m_spriteram.op[offs]);

                if (m_hardware_type == 3)
                {
                    // Imago
                    code  = (uint8_t)((m_spriteram.op[offs + 1]) & 0x3f);
                    flipx = 0;
                    flipy = 0;
                }
                else if (m_hardware_type == 2)
                {
                    // Boggy 84
                    code  =  (uint8_t)(m_spriteram.op[offs + 1] & 0x7f);
                    flipx =  0;
                    flipy =  m_spriteram.op[offs + 1] & 0x80;
                }
                else if (m_hardware_type == 1)
                {
                    // Fly-Boy/Fast Freddie/Red Robin
                    code  =  (uint8_t)(m_spriteram.op[offs + 1] & 0x7f);
                    flipx =  0;
                    flipy = ~m_spriteram.op[offs + 1] & 0x80;
                }
                else
                {
                    // Jump Coaster
                    code  = (uint8_t)((m_spriteram.op[offs + 1] & 0x3f) | 0x40);
                    flipx = ~m_spriteram.op[offs + 1] & 0x40;
                    flipy =  m_spriteram.op[offs + 1] & 0x80;
                }


                if (flip_screen_x() != 0)
                {
                    sx = (uint8_t)(240 - sx);
                    flipx = flipx == 0 ? 1 : 0;
                }
                if (flip_screen_y() != 0)
                {
                    sy = (uint8_t)(240 - sy);
                    flipy = flipy == 0 ? 1 : 0;
                }

                m_gfxdecode.op0.gfx(1).transpen(bitmap, flip_screen_x() != 0 ? spritevisibleareaflipx : spritevisiblearea,
                        code,
                        (uint32_t)(m_colorbank | (m_spriteram.op[offs + 2] & 0x07)),
                        flipx, flipy,
                        sx, sy, 0);
            }
        }


        uint32_t screen_update_fastfred(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            bitmap.fill(m_background_color.op0, cliprect);
            m_bg_tilemap.draw(screen, bitmap, cliprect, 0, 0);
            draw_sprites(bitmap, cliprect);

            return 0;
        }


        //TILE_GET_INFO_MEMBER(fastfred_state::imago_get_tile_info_bg)

        //TILE_GET_INFO_MEMBER(fastfred_state::imago_get_tile_info_fg)

        //TILE_GET_INFO_MEMBER(fastfred_state::imago_get_tile_info_web)

        //void fastfred_state::imago_fg_videoram_w(offs_t offset, uint8_t data)

        //WRITE_LINE_MEMBER(fastfred_state::imago_charbank_w)

        //VIDEO_START_MEMBER(fastfred_state,imago)

        //uint32_t fastfred_state::screen_update_imago(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)
    }
}
