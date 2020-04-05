// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using indirect_pen_t = System.UInt16;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using pen_t = System.UInt32;
using tilemap_memory_index = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint32_t = System.UInt32;


namespace mame
{
    partial class _1942_state : driver_device
    {
        void create_palette(palette_device palette)
        {
            ListBytesPointer color_prom = new ListBytesPointer(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();
            int i;

            for (i = 0; i < 256; i++)
            {
                int bit0;
                int bit1;
                int bit2;
                int bit3;

                /* red component */
                bit0 = (color_prom[i + 0 * 256] >> 0) & 0x01;
                bit1 = (color_prom[i + 0 * 256] >> 1) & 0x01;
                bit2 = (color_prom[i + 0 * 256] >> 2) & 0x01;
                bit3 = (color_prom[i + 0 * 256] >> 3) & 0x01;
                int r = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;
                /* green component */
                bit0 = (color_prom[i + 1 * 256] >> 0) & 0x01;
                bit1 = (color_prom[i + 1 * 256] >> 1) & 0x01;
                bit2 = (color_prom[i + 1 * 256] >> 2) & 0x01;
                bit3 = (color_prom[i + 1 * 256] >> 3) & 0x01;
                int g = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;
                /* blue component */
                bit0 = (color_prom[i + 2 * 256] >> 0) & 0x01;
                bit1 = (color_prom[i + 2 * 256] >> 1) & 0x01;
                bit2 = (color_prom[i + 2 * 256] >> 2) & 0x01;
                bit3 = (color_prom[i + 2 * 256] >> 3) & 0x01;
                int b = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

                palette.palette_interface.set_indirect_color(i, new rgb_t((u8)r, (u8)g, (u8)b));
            }
        }


        void _1942_palette(palette_device palette)
        {
            create_palette(palette);

            ListBytesPointer color_prom = new ListBytesPointer(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();
            color_prom += 3 * 256;
            // color_prom now points to the beginning of the lookup table


            /* characters use palette entries 128-143 */
            int colorbase = 0;
            for (int i = 0; i < 64 * 4; i++)
            {
                palette.palette_interface.set_pen_indirect((pen_t)(colorbase + i), (indirect_pen_t)(0x80 | color_prom[0]));  //*color_prom++);
                color_prom++;
            }

            // background tiles use palette entries 0-63 in four banks
            colorbase += 64 * 4;
            for (int i = 0; i < 32 * 8; i++)
            {
                palette.palette_interface.set_pen_indirect((pen_t)(colorbase + 0 * 32 * 8 + i), (indirect_pen_t)(0x00 | color_prom[0]));  //*color_prom);
                palette.palette_interface.set_pen_indirect((pen_t)(colorbase + 1 * 32 * 8 + i), (indirect_pen_t)(0x10 | color_prom[0]));
                palette.palette_interface.set_pen_indirect((pen_t)(colorbase + 2 * 32 * 8 + i), (indirect_pen_t)(0x20 | color_prom[0]));
                palette.palette_interface.set_pen_indirect((pen_t)(colorbase + 3 * 32 * 8 + i), (indirect_pen_t)(0x30 | color_prom[0]));
                color_prom++;
            }

            // sprites use palette entries 64-79
            colorbase += 4 * 32 * 8;
            for (int i = 0; i < 16 * 16; i++)
            {
                palette.palette_interface.set_pen_indirect((pen_t)(colorbase + i), (indirect_pen_t)(0x40 | color_prom[0]));  //*color_prom++);
                color_prom++;
            }
        }


        /***************************************************************************

          Callbacks for the TileMap code

        ***************************************************************************/

        //TILE_GET_INFO_MEMBER(_1942_state::get_fg_tile_info)
        void get_fg_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int code;
            int color;

            code = m_fg_videoram[tile_index];
            color = m_fg_videoram[tile_index + 0x400];
            SET_TILE_INFO_MEMBER(ref tileinfo, 0,
                    (UInt32)(code + ((color & 0x80) << 1)),
                    (UInt32)(color & 0x3f),
                    0);
        }

        //TILE_GET_INFO_MEMBER(_1942_state::get_bg_tile_info)
        void get_bg_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int code;
            int color;

            tile_index = (tile_index & 0x0f) | ((tile_index & 0x01f0) << 1);

            code = m_bg_videoram[tile_index];
            color = m_bg_videoram[tile_index + 0x10];
            SET_TILE_INFO_MEMBER(ref tileinfo, 1,
                    (UInt32)(code + ((color & 0x80) << 1)),
                    (UInt32)((color & 0x1f) + (0x20 * m_palette_bank)),
                    (byte)TILE_FLIPYX((color & 0x60) >> 5));
        }


        /***************************************************************************

          Start the video hardware emulation.

        ***************************************************************************/
        protected override void video_start()
        {
            m_fg_tilemap = machine().tilemap().create(m_gfxdecode.target.digfx, get_fg_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8, 8, 32, 32);
            m_bg_tilemap = machine().tilemap().create(m_gfxdecode.target.digfx, get_bg_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_COLS, 16, 16, 32, 16);

            m_fg_tilemap.set_transparent_pen(0);
        }


        /***************************************************************************

          Memory handlers

        ***************************************************************************/

        //WRITE8_MEMBER(_1942_state::_1942_fgvideoram_w)
        void _1942_fgvideoram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_fg_videoram[offset] = data;
            m_fg_tilemap.mark_tile_dirty(offset & 0x3ff);
        }


        //WRITE8_MEMBER(_1942_state::_1942_bgvideoram_w)
        void _1942_bgvideoram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_bg_videoram[offset] = data;
            m_bg_tilemap.mark_tile_dirty((offset & 0x0f) | ((offset >> 1) & 0x01f0));
        }


        //WRITE8_MEMBER(_1942_state::_1942_palette_bank_w)
        void _1942_palette_bank_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            if (m_palette_bank != data)
            {
                m_palette_bank = data & 3;
                m_bg_tilemap.mark_all_dirty();
            }
        }


        //WRITE8_MEMBER(_1942_state::_1942_scroll_w)
        void _1942_scroll_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_scroll[offset] = data;
            m_bg_tilemap.set_scrollx(0, m_scroll[0] | (m_scroll[1] << 8));
        }


        //WRITE8_MEMBER(_1942_state::_1942_c804_w)
        void _1942_c804_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            /* bit 7: flip screen
               bit 4: cpu B reset
               bit 0: coin counter */

            machine().bookkeeping().coin_counter_w(0, data & 0x01);

            m_audiocpu.target.set_input_line(device_execute_interface.INPUT_LINE_RESET, (data & 0x10) != 0 ? ASSERT_LINE : CLEAR_LINE);

            flip_screen_set((u32)(data & 0x80));
        }


        /***************************************************************************

          Display refresh

        ***************************************************************************/

        protected virtual void draw_sprites(bitmap_ind16 bitmap, rectangle cliprect)
        {
            int offs;

            for (offs = (int)m_spriteram.bytes() - 4; offs >= 0; offs -= 4)
            {
                int i;
                int code;
                int col;
                int sx;
                int sy;
                int dir;

                code = (m_spriteram[offs] & 0x7f) + 4 * (m_spriteram[offs + 1] & 0x20) + 2 * (m_spriteram[offs] & 0x80);
                col = m_spriteram[offs + 1] & 0x0f;
                sx = m_spriteram[offs + 3] - 0x10 * (m_spriteram[offs + 1] & 0x10);
                sy = m_spriteram[offs + 2];
                dir = 1;

                if (flip_screen() != 0)
                {
                    sx = 240 - sx;
                    sy = 240 - sy;
                    dir = -1;
                }

                /* handle double / quadruple height */
                i = (m_spriteram[offs + 1] & 0xc0) >> 6;
                if (i == 2)
                    i = 3;

                do
                {
                    m_gfxdecode.target.digfx.gfx(2).transpen(bitmap, cliprect,
                            (u32)(code + i), (u32)col,
                            (int)flip_screen(), (int)flip_screen(),
                            sx, sy + 16 * i * dir, 15);

                    i--;
                } while (i >= 0);
            }
        }


        u32 screen_update(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            m_bg_tilemap.draw(screen, bitmap, cliprect, 0, 0);
            draw_sprites(bitmap, cliprect);
            m_fg_tilemap.draw(screen, bitmap, cliprect, 0, 0);
            return 0;
        }
    }
}
