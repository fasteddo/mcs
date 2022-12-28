// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using indirect_pen_t = System.UInt16;  //typedef u16 indirect_pen_t;
using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using tilemap_memory_index = System.UInt32;  //typedef u32 tilemap_memory_index;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.drawgfx_global;
using static mame.resnet_global;
using static mame.tilemap_global;
using static mame.util;


namespace mame
{
    partial class rallyx_state : driver_device
    {
        //#define STARS_COLOR_BASE    (0x104)


        /***************************************************************************

          Convert the color PROMs.

          Rally X has one 32x8 palette PROM and one 256x4 color lookup table PROM.
          The palette PROM is connected to the RGB output this way:

          bit 7 -- 220 ohm resistor  -- BLUE
                -- 470 ohm resistor  -- BLUE
                -- 220 ohm resistor  -- GREEN
                -- 470 ohm resistor  -- GREEN
                -- 1  kohm resistor  -- GREEN
                -- 220 ohm resistor  -- RED
                -- 470 ohm resistor  -- RED
          bit 0 -- 1  kohm resistor  -- RED

          In Rally-X there is a 1 kohm pull-down on B only, in Locomotion the
          1 kohm pull-down is an all three RGB outputs.

        ***************************************************************************/
        void rallyx_palette(palette_device palette)
        {
            Pointer<uint8_t> color_prom = new Pointer<uint8_t>(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();
            int [] resistances_rg = new int [3] { 1000, 470, 220 };
            int [] resistances_b = new int [2] { 470, 220 };

            // compute the color output resistor weights
            double [] rweights = new double [3];
            double [] gweights = new double [3];
            double [] bweights = new double [2];
            compute_resistor_weights(0, 255, -1.0,
                    3, resistances_rg, out rweights,    0, 0,
                    3, resistances_rg, out gweights,    0, 0,
                    2, resistances_b,  out bweights, 1000, 0);

            // create a lookup table for the palette
            for (int i = 0; i < 0x20; i++)
            {
                int bit0;
                int bit1;
                int bit2;

                // red component
                bit0 = BIT(color_prom[i], 0);
                bit1 = BIT(color_prom[i], 1);
                bit2 = BIT(color_prom[i], 2);
                int r = combine_weights(rweights, bit0, bit1, bit2);

                // green component
                bit0 = BIT(color_prom[i], 3);
                bit1 = BIT(color_prom[i], 4);
                bit2 = BIT(color_prom[i], 5);
                int g = combine_weights(gweights, bit0, bit1, bit2);

                // blue component
                bit0 = BIT(color_prom[i], 6);
                bit1 = BIT(color_prom[i], 7);
                int b = combine_weights(bweights, bit0, bit1);

                palette.set_indirect_color(i, new rgb_t((uint8_t)r, (uint8_t)g, (uint8_t)b));
            }

            // color_prom now points to the beginning of the lookup table
            color_prom += 0x20;

            // character/sprites lookup table
            for (int i = 0x000; i < 0x100; i++)
            {
                uint8_t ctabentry = (uint8_t)(color_prom[i] & 0x0f);
                palette.set_pen_indirect((pen_t)i, ctabentry);
            }

            // bullets use colors 0x10-0x13
            for (int i = 0x100; i < 0x104; i++)
                palette.set_pen_indirect((pen_t)i, (indirect_pen_t)((i - 0x100) | 0x10));
        }


        //void rallyx_state::jungler_palette(palette_device &palette) const


        /***************************************************************************

          Callbacks for the TileMap code

        ***************************************************************************/
        // the video RAM has space for 32x32 tiles and is only partially used for the radar
        //TILEMAP_MAPPER_MEMBER(rallyx_state::fg_tilemap_scan)
        tilemap_memory_index fg_tilemap_scan(u32 col, u32 row, u32 num_cols, u32 num_rows)
        {
            return col + (row << 5);
        }


        void rallyx_get_tile_info(ref tile_data tileinfo, int tile_index, int ram_offs)
        {
            uint8_t attr = m_videoram[ram_offs + tile_index + 0x800].op;
            tileinfo.category = (u8)((attr & 0x20) >> 5);
            tileinfo.set(0,
                    m_videoram[ram_offs + tile_index].op,
                    (u32)attr & 0x3f,
                    (u8)(TILE_FLIPYX(attr >> 6) ^ TILE_FLIPX));
        }


        //TILE_GET_INFO_MEMBER(rallyx_state::rallyx_bg_get_tile_info)
        void rallyx_bg_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            rallyx_get_tile_info(ref tileinfo, (int)tile_index, 0x400);
        }


        //TILE_GET_INFO_MEMBER(rallyx_state::rallyx_fg_get_tile_info)
        void rallyx_fg_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            rallyx_get_tile_info(ref tileinfo, (int)tile_index, 0x000);
        }


        //inline void rallyx_state::locomotn_get_tile_info(tile_data &tileinfo,int tile_index,int ram_offs)

        //TILE_GET_INFO_MEMBER(rallyx_state::locomotn_bg_get_tile_info)

        //TILE_GET_INFO_MEMBER(rallyx_state::locomotn_fg_get_tile_info)


        /***************************************************************************

          Start the video hardware emulation.

        ***************************************************************************/
        //void rallyx_state::calculate_star_field(  )


        void video_start_common()
        {
            m_spriteram = m_videoram.op + 0x00;
            m_spriteram2 = m_spriteram + 0x800;
            m_radarx = m_videoram.op + 0x20;
            m_radary = m_radarx + 0x800;

            for (int i = 0; i < 16; i++)
                m_palette.op0.dipalette.shadow_table()[i] = (pen_t)i + 16;

            for (int i = 16; i < 32; i++)
                m_palette.op0.dipalette.shadow_table()[i] = (pen_t)i;

            for (int i = 0; i < 3; i++)
                m_drawmode_table[i] = DRAWMODE_SHADOW;

            m_drawmode_table[3] = DRAWMODE_NONE;
        }


        //VIDEO_START_MEMBER(rallyx_state,rallyx)
        void video_start_rallyx()
        {
            m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op0, rallyx_bg_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8, 8, 32, 32);  //m_bg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(rallyx_state::rallyx_bg_get_tile_info)), TILEMAP_SCAN_ROWS, 8, 8, 32, 32);
            m_fg_tilemap = machine().tilemap().create(m_gfxdecode.op0, rallyx_fg_get_tile_info, fg_tilemap_scan, 8, 8, 8, 32);  //m_fg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(rallyx_state::rallyx_fg_get_tile_info)), tilemap_mapper_delegate(*this, FUNC(rallyx_state::fg_tilemap_scan)), 8, 8, 8, 32);

            // the scrolling tilemap is slightly misplaced in Rally X
            m_bg_tilemap.set_scrolldx(3, 3);

            m_spriteram_base = 0x14;

            video_start_common();
        }


        //VIDEO_START_MEMBER(rallyx_state,jungler)

        //VIDEO_START_MEMBER(rallyx_state,locomotn)

        //VIDEO_START_MEMBER(rallyx_state,commsega)


        /***************************************************************************

          Memory handlers

        ***************************************************************************/
        void videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram[offset].op = data;
            if ((offset & 0x400) != 0)
                m_bg_tilemap.mark_tile_dirty(offset & 0x3ff);
            else
                m_fg_tilemap.mark_tile_dirty(offset & 0x3ff);
        }


        void scrollx_w(uint8_t data)
        {
            m_bg_tilemap.set_scrollx(0, data);
        }

        void scrolly_w(uint8_t data)
        {
            m_bg_tilemap.set_scrolly(0, data);
        }


        //void rallyx_state::stars_enable_w(int state)

        //void rallyx_state::plot_star( bitmap_ind16 &bitmap, const rectangle &cliprect, int x, int y, int color )

        //void rallyx_state::draw_stars( bitmap_ind16 &bitmap, const rectangle &cliprect )


        void rallyx_draw_sprites(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            for (int offs = 0x20 - 2; offs >= m_spriteram_base; offs -= 2)
            {
                int sx = m_spriteram[offs + 1] + ((m_spriteram2[offs + 1] & 0x80) << 1);
                int sy = 241 - m_spriteram2[offs];
                int color = m_spriteram2[offs + 1] & 0x3f;
                int flipx = m_spriteram[offs] & 1;
                int flipy = m_spriteram[offs] & 2;

                m_gfxdecode.op0.gfx(1).prio_transmask(bitmap, cliprect,
                        (u32)(m_spriteram[offs] & 0xfc) >> 2,
                        (u32)color,
                        flipx, flipy,
                        sx, sy,
                        screen.priority(), 0x02,
                        m_palette.op0.transpen_mask(m_gfxdecode.op0.gfx(1), (u32)color, 0));
            }
        }


        //void rallyx_state::locomotn_draw_sprites( screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect )


        void rallyx_draw_bullets(bitmap_ind16 bitmap, rectangle cliprect, bool transpen)
        {
            for (int offs = m_spriteram_base; offs < 0x20; offs++)
            {
                int x = m_radarx[offs] + ((~m_radarattr[offs & 0x0f].op & 0x01) << 8);
                int y = 253 - m_radary[offs];
                if (flip_screen() != 0)
                    x -= 3;

                if (transpen)
                    m_gfxdecode.op0.gfx(2).transpen(bitmap, cliprect,
                            ((u32)(m_radarattr[offs & 0x0f].op & 0x0e) >> 1) ^ 0x07,
                            0,
                            0, 0,
                            x, y,
                            3);
                else
                    m_gfxdecode.op0.gfx(2).transtable(bitmap, cliprect,
                            ((u32)(m_radarattr[offs & 0x0f].op & 0x0e) >> 1) ^ 0x07,
                            0,
                            0, 0,
                            x, y,
                            m_drawmode_table);
            }
        }


        //void rallyx_state::jungler_draw_bullets( bitmap_ind16 &bitmap, const rectangle &cliprect, bool transpen )

        //void rallyx_state::locomotn_draw_bullets( bitmap_ind16 &bitmap, const rectangle &cliprect, bool transpen )


        uint32_t screen_update_rallyx(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            /* the radar tilemap is just 8x32. We rely on the tilemap code to repeat it across
               the screen, and clip it to only the position where it is supposed to be shown */
            rectangle fg_clip = cliprect;
            rectangle bg_clip = cliprect;

            if (flip_screen() != 0)
            {
                bg_clip.min_x = 8 * 8;
                fg_clip.max_x = 8 * 8 - 1;
            }
            else
            {
                bg_clip.max_x = 28 * 8 - 1;
                fg_clip.min_x = 28 * 8;
            }

            screen.priority().fill(0, cliprect);

            m_bg_tilemap.draw(screen, bitmap, bg_clip, 0, 0);
            m_fg_tilemap.draw(screen, bitmap, fg_clip, 0, 0);
            m_bg_tilemap.draw(screen, bitmap, bg_clip, 1, 1);
            m_fg_tilemap.draw(screen, bitmap, fg_clip, 1, 1);

            rallyx_draw_bullets(bitmap, cliprect, true);
            rallyx_draw_sprites(screen, bitmap, cliprect);
            rallyx_draw_bullets(bitmap, cliprect, false);

            return 0;
        }


        //uint32_t rallyx_state::screen_update_jungler(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t rallyx_state::screen_update_locomotn(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)
    }
}
