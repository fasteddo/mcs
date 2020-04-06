// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using pen_t = System.UInt32;
using tilemap_memory_index = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    partial class digdug_state : galaga_state
    {
        /***************************************************************************

          Convert the color PROMs.

          digdug has one 32x8 palette PROM and two 256x4 color lookup table PROMs
          (one for characters, one for sprites).
          The palette PROM is connected to the RGB output this way:

          bit 7 -- 220 ohm resistor  -- BLUE
                -- 470 ohm resistor  -- BLUE
                -- 220 ohm resistor  -- GREEN
                -- 470 ohm resistor  -- GREEN
                -- 1  kohm resistor  -- GREEN
                -- 220 ohm resistor  -- RED
                -- 470 ohm resistor  -- RED
          bit 0 -- 1  kohm resistor  -- RED

        ***************************************************************************/

        void digdug_palette(palette_device palette)
        {
            ListBytesPointer color_prom = new ListBytesPointer(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();

            for (int i = 0;i < 32;i++)
            {
                int bit0;
                int bit1;
                int bit2;

                bit0 = BIT(color_prom[0], 0);
                bit1 = BIT(color_prom[0], 1);
                bit2 = BIT(color_prom[0], 2);
                int r = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;
                bit0 = BIT(color_prom[0], 3);
                bit1 = BIT(color_prom[0], 4);
                bit2 = BIT(color_prom[0], 5);
                int g = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;
                bit0 = 0;
                bit1 = BIT(color_prom[0], 6);
                bit2 = BIT(color_prom[0], 7);
                int b = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;
                palette.palette_interface.set_indirect_color(i, new rgb_t((byte)r,(byte)g,(byte)b));
                color_prom++;
            }

            // characters - direct mapping
            for (int i = 0; i < 16; i++)
            {
                palette.palette_interface.set_pen_indirect((pen_t)((i << 1) | 0), 0);
                palette.palette_interface.set_pen_indirect((pen_t)((i << 1) | 1), (UInt16)i);
            }

            // sprites
            for (int i = 0; i < 0x100; i++)
            {
                palette.palette_interface.set_pen_indirect((pen_t)(16*2 + i), (UInt16)((color_prom[0] & 0x0f) | 0x10));
                color_prom++;
            }

            // bg_select
            for (int i = 0; i < 0x100; i++)
            {
                palette.palette_interface.set_pen_indirect((pen_t)(16*2 + 256 + i), (UInt16)(color_prom[0] & 0x0f));
                color_prom++;
            }
        }


        /***************************************************************************
          Callbacks for the TileMap code
        ***************************************************************************/
        /* convert from 32x32 to 36x28 */
        //TILEMAP_MAPPER_MEMBER(digdug_state::tilemap_scan)
        protected new tilemap_memory_index tilemap_scan(UInt32 col, UInt32 row, UInt32 num_cols, UInt32 num_rows)
        {
            int offs;

            row += 2;
            col -= 2;
            if ((col & 0x20) != 0)
                offs = (int)(row + ((col & 0x1f) << 5));
            else
                offs = (int)(col + (row << 5));

            return (tilemap_memory_index)offs;
        }


        //TILE_GET_INFO_MEMBER(digdug_state::bg_get_tile_info)
        void bg_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            ListBytesPointer rom = new ListBytesPointer(memregion("gfx4").base_());  //uint8_t *rom = memregion("gfx4")->base();

            int code = rom[(int)(tile_index | (m_bg_select << 10))];

            /* when the background is "disabled", it is actually still drawn, but using
               a color code that makes all pixels black. There are pullups setting the
               code to 0xf, but also solder pads that optionally connect the lines with
               tilemap RAM, therefore allowing to pick some bits of the color code from
               the top 4 bits of alpha code. This feature is not used by Dig Dug. */

            int color = m_bg_disable != 0 ? 0xf : (code >> 4);
            SET_TILE_INFO_MEMBER(ref tileinfo, 2,
                    (UInt32)code,
                    (UInt32)(color | m_bg_color_bank),
                    0);
        }

        //TILE_GET_INFO_MEMBER(digdug_state::tx_get_tile_info)
        void tx_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            byte code = m_videoram[tile_index];
            int color;

            /* the hardware has two ways to pick the color, either straight from the
               bottom 4 bits of the character code, or from the top 4 bits through a
               formula. The former method isnot used by Dig Dug and seems kind of
               useless (I don't know what use they were thinking of when they added
               it), anyway here it is reproduced faithfully. */

            if (m_tx_color_mode != 0)
                color = code & 0x0f;
            else
                color = ((code >> 4) & 0x0e) | ((code >> 3) & 2);

            /* the hardware has two character sets, one normal and one x-flipped. When
               screen is flipped, character y flip is done by the hardware inverting the
               timing signals, while x flip is done by selecting the 2nd character set.
               We reproduce this here, but since the tilemap system automatically flips
               characters when screen is flipped, we have to flip them back. */
            SET_TILE_INFO_MEMBER(ref tileinfo, 0,
                    (UInt32)((code & 0x7f) | (flip_screen() != 0 ? 0x80 : 0)),
                    (UInt32)color,
                    flip_screen() != 0 ? TILE_FLIPX : (byte)0);
        }


        /***************************************************************************
          Start the video hardware emulation.
        ***************************************************************************/
        //VIDEO_START_MEMBER(digdug_state,digdug)
        void video_start_digdug()
        {
            m_bg_select = 0;
            m_tx_color_mode = 0;
            m_bg_disable = 0;
            m_bg_color_bank = 0;

            m_bg_tilemap = machine().tilemap().create(gfxdecode.target.digfx, bg_get_tile_info, tilemap_scan, 8,8,36,28);  //m_bg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(digdug_state::bg_get_tile_info)), tilemap_mapper_delegate(*this, FUNC(digdug_state::tilemap_scan)), 8, 8, 36, 28);
            m_fg_tilemap = machine().tilemap().create(gfxdecode.target.digfx, tx_get_tile_info, tilemap_scan, 8,8,36,28);  //m_fg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(digdug_state::tx_get_tile_info)), tilemap_mapper_delegate(*this, FUNC(digdug_state::tilemap_scan)), 8, 8, 36, 28);

            m_fg_tilemap.set_transparent_pen(0);

            save_item(m_bg_select, "m_bg_select");
            save_item(m_tx_color_mode, "m_tx_color_mode");
            save_item(m_bg_disable, "m_bg_disable");
            save_item(m_bg_color_bank, "m_bg_color_bank");
        }


        /***************************************************************************
          Display refresh
        ***************************************************************************/

        static readonly int [,] gfx_offs = new int [2,2]
        {
            { 0, 1 },
            { 2, 3 }
        };

        protected override void draw_sprites(bitmap_ind16 bitmap, rectangle cliprect)
        {
            ListBytesPointer spriteram = new ListBytesPointer(m_digdug_objram.target, 0x380);  //uint8_t *spriteram = m_digdug_objram + 0x380;
            ListBytesPointer spriteram_2 = new ListBytesPointer(m_digdug_posram.target, 0x380);  //uint8_t *spriteram_2 = m_digdug_posram + 0x380;
            ListBytesPointer spriteram_3 = new ListBytesPointer(m_digdug_flpram.target, 0x380);  //uint8_t *spriteram_3 = m_digdug_flpram + 0x380;
            int offs;

            // mask upper and lower columns
            rectangle visarea = cliprect;
            visarea.min_x = 2*8;
            visarea.max_x = 34*8-1;

            for (offs = 0;offs < 0x80;offs += 2)
            {
                int sprite = spriteram[offs];
                int color = spriteram[offs+1] & 0x3f;
                int sx = spriteram_2[offs+1] - 40+1;
                int sy = 256 - spriteram_2[offs] + 1;   // sprites are buffered and delayed by one scanline
                int flipx = (spriteram_3[offs] & 0x01);
                int flipy = (spriteram_3[offs] & 0x02) >> 1;
                int size  = (sprite & 0x80) >> 7;
                int x,y;

                if (size != 0)
                    sprite = (sprite & 0xc0) | ((sprite & ~0xc0) << 2);

                sy -= 16 * size;
                sy = (sy & 0xff) - 32;  // fix wraparound

                if (flip_screen() != 0)
                {
                    flipx ^= 1;
                    flipy ^= 1;
                }

                for (y = 0;y <= size;y++)
                {
                    for (x = 0;x <= size;x++)
                    {
                        UInt32 transmask =  palette.target.palette_interface.transpen_mask(gfxdecode.target.digfx.gfx(1), (UInt32)color, 0x1f);
                        gfxdecode.target.digfx.gfx(1).transmask(bitmap,visarea,
                            (UInt32)(sprite + gfx_offs[y ^ (size * flipy), x ^ (size * flipx)]),
                            (UInt32)color,
                            flipx,flipy,
                            ((sx + 16*x) & 0xff), sy + 16*y,transmask);
                        /* wraparound */
                        gfxdecode.target.digfx.gfx(1).transmask(bitmap,visarea,
                            (UInt32)(sprite + gfx_offs[y ^ (size * flipy), x ^ (size * flipx)]),
                            (UInt32)color,
                            flipx,flipy,
                            ((sx + 16*x) & 0xff) + 0x100, sy + 16*y,transmask);
                    }
                }
            }
        }


        u32 screen_update_digdug(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            m_bg_tilemap.draw(screen, bitmap, cliprect, 0,0);
            m_fg_tilemap.draw(screen, bitmap, cliprect, 0,0);
            draw_sprites(bitmap,cliprect);

            return 0;
        }


        /***************************************************************************
          Memory handlers
        ***************************************************************************/

        //WRITE8_MEMBER( digdug_state::digdug_videoram_w )
        void digdug_videoram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_videoram[offset] = data;
            m_fg_tilemap.mark_tile_dirty(offset & 0x3ff);
        }


        //WRITE8_MEMBER(digdug_state::bg_select_w)
        void bg_select_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            // select background picture
            if (m_bg_select != (data & 0x03))
            {
                m_bg_select = (byte)(data & 0x03);
                m_bg_tilemap.mark_all_dirty();
            }

            // background color bank
            if (m_bg_color_bank != (data & 0x30))
            {
                m_bg_color_bank = (byte)(data & 0x30);
                m_bg_tilemap.mark_all_dirty();
            }
        }

        //WRITE_LINE_MEMBER(digdug_state::tx_color_mode_w)
        void tx_color_mode_w(int state)
        {
            // select alpha layer color mode (see tx_get_tile_info)
            m_tx_color_mode = (byte)state;
            m_fg_tilemap.mark_all_dirty();
        }

        //WRITE_LINE_MEMBER(digdug_state::bg_disable_w)
        void bg_disable_w(int state)
        {
            // "disable" background (see bg_get_tile_info)
            m_bg_disable = (byte)state;
            m_bg_tilemap.mark_all_dirty();
        }
    }
}
