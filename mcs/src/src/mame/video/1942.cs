// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using indirect_pen_t = System.UInt16;
using offs_t = System.UInt32;
using pen_t = System.UInt32;
using tilemap_memory_index = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    partial class _1942_state : driver_device
    {
        void create_palette(palette_device palette)
        {
            Pointer<uint8_t> color_prom = new Pointer<uint8_t>(memregion("palproms").base_());  //const uint8_t *color_prom = memregion("palproms")->base();

            for (int i = 0; i < 256; i++)
            {
                /* red component */
                int bit0 = BIT(color_prom[i + 0 * 256], 0);
                int bit1 = BIT(color_prom[i + 0 * 256], 1);
                int bit2 = BIT(color_prom[i + 0 * 256], 2);
                int bit3 = BIT(color_prom[i + 0 * 256], 3);
                int r = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;
                /* green component */
                bit0 = BIT(color_prom[i + 1 * 256], 0);
                bit1 = BIT(color_prom[i + 1 * 256], 1);
                bit2 = BIT(color_prom[i + 1 * 256], 2);
                bit3 = BIT(color_prom[i + 1 * 256], 3);
                int g = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;
                /* blue component */
                bit0 = BIT(color_prom[i + 2 * 256], 0);
                bit1 = BIT(color_prom[i + 2 * 256], 1);
                bit2 = BIT(color_prom[i + 2 * 256], 2);
                bit3 = BIT(color_prom[i + 2 * 256], 3);
                int b = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

                palette.dipalette.set_indirect_color(i, new rgb_t((u8)r, (u8)g, (u8)b));
            }
        }


        void _1942_palette(palette_device palette)
        {
            create_palette(palette);

            /* characters use palette entries 128-143 */
            int colorbase = 0;
            Pointer<uint8_t> charlut_prom = new Pointer<uint8_t>(memregion("charprom").base_());  //const uint8_t *charlut_prom = memregion("charprom")->base();
            for (int i = 0; i < 64 * 4; i++)
            {
                palette.dipalette.set_pen_indirect((pen_t)(colorbase + i), (indirect_pen_t)(0x80 | charlut_prom[i]));
            }

            // background tiles use palette entries 0-63 in four banks
            colorbase += 64 * 4;
            Pointer<uint8_t> tilelut_prom = new Pointer<uint8_t>(memregion("tileprom").base_());  //const uint8_t *tilelut_prom = memregion("tileprom")->base();
            for (int i = 0; i < 32 * 8; i++)
            {
                palette.dipalette.set_pen_indirect((pen_t)(colorbase + 0 * 32 * 8 + i), (indirect_pen_t)(0x00 | tilelut_prom[i]));
                palette.dipalette.set_pen_indirect((pen_t)(colorbase + 1 * 32 * 8 + i), (indirect_pen_t)(0x10 | tilelut_prom[i]));
                palette.dipalette.set_pen_indirect((pen_t)(colorbase + 2 * 32 * 8 + i), (indirect_pen_t)(0x20 | tilelut_prom[i]));
                palette.dipalette.set_pen_indirect((pen_t)(colorbase + 3 * 32 * 8 + i), (indirect_pen_t)(0x30 | tilelut_prom[i]));
            }

            // sprites use palette entries 64-79
            colorbase += 4 * 32 * 8;
            Pointer<uint8_t> sprlut_prom = new Pointer<uint8_t>(memregion("sprprom").base_());  //const uint8_t *sprlut_prom = memregion("sprprom")->base();
            for (int i = 0; i < 16 * 16; i++)
            {
                palette.dipalette.set_pen_indirect((pen_t)(colorbase + i), (indirect_pen_t)(0x40 | sprlut_prom[i]));
            }
        }


        /***************************************************************************

          Callbacks for the TileMap code

        ***************************************************************************/

        //TILE_GET_INFO_MEMBER(_1942_state::get_fg_tile_info)
        void get_fg_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int code = m_fg_videoram[tile_index];
            int color = m_fg_videoram[tile_index + 0x400];
            tileinfo.set(0,
                    (u32)(code + ((color & 0x80) << 1)),
                    (u32)(color & 0x3f),
                    0);
        }

        //TILE_GET_INFO_MEMBER(_1942_state::get_bg_tile_info)
        void get_bg_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            tile_index = (tile_index & 0x0f) | ((tile_index & 0x01f0) << 1);

            int code = m_bg_videoram[tile_index];
            int color = m_bg_videoram[tile_index + 0x10];
            tileinfo.set(1,
                    (u32)(code + ((color & 0x80) << 1)),
                    (u32)((color & 0x1f) + (0x20 * m_palette_bank)),
                    TILE_FLIPYX((color & 0x60) >> 5));
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

        void _1942_fgvideoram_w(offs_t offset, uint8_t data)
        {
            m_fg_videoram[offset] = data;
            m_fg_tilemap.mark_tile_dirty(offset & 0x3ff);
        }


        void _1942_bgvideoram_w(offs_t offset, uint8_t data)
        {
            m_bg_videoram[offset] = data;
            m_bg_tilemap.mark_tile_dirty((offset & 0x0f) | ((offset >> 1) & 0x01f0));
        }


        void _1942_palette_bank_w(uint8_t data)
        {
            if (m_palette_bank != data)
            {
                m_palette_bank = data & 3;
                m_bg_tilemap.mark_all_dirty();
            }
        }


        void _1942_scroll_w(offs_t offset, uint8_t data)
        {
            m_scroll[offset] = data;
            m_bg_tilemap.set_scrollx(0, m_scroll[0] | (m_scroll[1] << 8));
        }


        void _1942_c804_w(uint8_t data)
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
            // Sprites 0 to 15 are drawn on all scanlines.
            // Sprites 16 to 23 are drawn on scanlines 16 to 127.
            // Sprites 24 to 31 are drawn on scanlines 128 to 239.
            //
            // The reason for this is ostensibly so that the back half of the sprite list can
            // be used to selectively mask sprites along the midpoint of the screen.
            //
            // Moreover, the H counter runs from 128 to 511 for a total of 384 horizontal
            // clocks per scanline. With an effective 6MHz pixel clock, this produces a
            // horizontal scan rate of exactly 15.625kHz, a standard scan rate for games
            // of this era.
            //
            // Sprites are drawn by MAME in reverse order, as the actual hardware only
            // permits a transparent pixel to be overwritten by an opaque pixel, and does
            // not support opaque-opaque overwriting - i.e., the first sprite to draw wins
            // control over its horizontal range. If MAME drew in forward order, it would
            // instead produce a last-sprite-wins behavior.

            for (int y = cliprect.min_y; y <= cliprect.max_y; y++)
            {
                rectangle cliprecty = new rectangle(cliprect.min_x, cliprect.max_x, y, y);
                uint8_t [] objdata = new uint8_t [4];
                uint8_t v = flip_screen() != 0 ? (uint8_t)(~(y - 1)) : (uint8_t)(y - 1);
                for (int h = 496; h >= 128; h -= 16)
                {
                    bool objcnt4 = BIT(h, 8) != BIT(~h, 7);
                    bool objcnt3 = ((BIT(v, 7) != 0) && objcnt4) != (BIT(~h, 7) != 0);
                    uint8_t obj_idx = (uint8_t)((h >> 4) & 7);
                    obj_idx |= objcnt3 ? (uint8_t)0x08 : (uint8_t)0x00;
                    obj_idx |= objcnt4 ? (uint8_t)0x10 : (uint8_t)0x00;
                    obj_idx <<= 2;
                    for (int i = 0; i < 4; i++)
                        objdata[i] = m_spriteram[obj_idx | i];

                    int code = (objdata[0] & 0x7f) + ((objdata[1] & 0x20) << 2) + ((objdata[0] & 0x80) << 1);
                    int col = objdata[1] & 0x0f;
                    int sx = objdata[3] - 0x10 * (objdata[1] & 0x10);
                    int sy = objdata[2];
                    int dir = 1;

                    uint8_t valpha = (uint8_t)sy;
                    uint8_t v2c = (uint8_t)((uint8_t)(~v) + (flip_screen() != 0 ? 0x01 : 0xff));
                    uint8_t lvbeta = (uint8_t)(v2c + valpha);
                    uint8_t vbeta = (uint8_t)(~lvbeta);
                    bool vleq = vbeta <= ((~valpha) & 0xff);
                    bool vinlen = true;
                    uint8_t vlen = (uint8_t)(objdata[1] >> 6);
                    switch (vlen & 3)
                    {
                    case 0:
                        vinlen = (BIT(lvbeta, 7) != 0) && (BIT(lvbeta, 6) != 0) && (BIT(lvbeta, 5) != 0) && (BIT(lvbeta, 4) != 0);
                        break;
                    case 1:
                        vinlen = (BIT(lvbeta, 7) != 0) && (BIT(lvbeta, 6) != 0) && (BIT(lvbeta, 5) != 0);
                        break;
                    case 2:
                        vinlen = (BIT(lvbeta, 7) != 0) && (BIT(lvbeta, 6) != 0);
                        break;
                    case 3:
                        vinlen = true;
                        break;
                    }
                    bool vinzone = !(vleq && vinlen);

                    if (flip_screen() != 0)
                    {
                        sx = 240 - sx;
                        sy = 240 - sy;
                        dir = -1;
                    }

                    /* handle double / quadruple height */
                    {
                        int i = (objdata[1] & 0xc0) >> 6;
                        if (i == 2)
                            i = 3;

                        if (!vinzone)
                        {
                            do
                            {
                                m_gfxdecode.target.digfx.gfx(2).transpen(bitmap, cliprecty, (u32)(code + i), (u32)col, (int)flip_screen(), (int)flip_screen(), sx, sy + 16 * i * dir, 15);
                            } while (i-- > 0);
                        }
                    }
                }
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
