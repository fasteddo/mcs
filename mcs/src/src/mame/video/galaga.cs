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
using uint16_t = System.UInt16;

using static mame.emucore_global;
using static mame.tilemap_global;
using static mame.util;


namespace mame
{
    partial class galaga_state : driver_device
    {
        /***************************************************************************

          Convert the color PROMs.

          Galaga has one 32x8 palette PROM and two 256x4 color lookup table PROMs
          (one for characters, one for sprites). Only the first 128 bytes of the
          lookup tables seem to be used.
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

        void galaga_palette(palette_device palette)
        {
            Pointer<uint8_t> color_prom = new Pointer<uint8_t>(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();

            // core palette
            for (int i = 0; i < 32; i++)
            {
                int bit0;
                int bit1;
                int bit2;

                bit0 = BIT(color_prom.op, 0);
                bit1 = BIT(color_prom.op, 1);
                bit2 = BIT(color_prom.op, 2);
                int r = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;
                bit0 = BIT(color_prom.op, 3);
                bit1 = BIT(color_prom.op, 4);
                bit2 = BIT(color_prom.op, 5);
                int g = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;
                bit0 = 0;
                bit1 = BIT(color_prom.op, 6);
                bit2 = BIT(color_prom.op, 7);
                int b = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;

                palette.set_indirect_color(i, new rgb_t((u8)r, (u8)g, (u8)b));
                color_prom++;
            }

            // palette for the stars
            for (int i = 0; i < 64; i++)
            {
                int [] map = new int[4] { 0x00, 0x47, 0x97, 0xde };

                int r = map[(i >> 0) & 0x03];
                int g = map[(i >> 2) & 0x03];
                int b = map[(i >> 4) & 0x03];

                palette.set_indirect_color(32 + i, new rgb_t((u8)r, (u8)g, (u8)b));
            }

            // characters
            for (int i = 0; i < 64*4; i++)
            {
                palette.set_pen_indirect((pen_t)i, (indirect_pen_t)((color_prom.op & 0x0f) | 0x10));
                color_prom++;
            }

            // sprites
            for (int i = 0; i < 64*4; i++)
            {
                palette.set_pen_indirect((pen_t)(64*4 + i), (indirect_pen_t)((color_prom.op & 0x0f)));
                color_prom++;
            }

            // now the stars
            for (int i = 0; i < 64; i++)
                palette.set_pen_indirect((pen_t)(64*4 + 64*4 + i), (indirect_pen_t)(32 + i));
        }


        /***************************************************************************
          Callbacks for the TileMap code
        ***************************************************************************/

        /* convert from 32x32 to 36x28 */
        //TILEMAP_MAPPER_MEMBER(galaga_state::tilemap_scan)
        protected tilemap_memory_index tilemap_scan(u32 col, u32 row, u32 num_cols, u32 num_rows)
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


        //TILE_GET_INFO_MEMBER(galaga_state::get_tile_info)
        void get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            /* the hardware has two character sets, one normal and one x-flipped. When
               screen is flipped, character y flip is done by the hardware inverting the
               timing signals, while x flip is done by selecting the 2nd character set.
               We reproduce this here, but since the tilemap system automatically flips
               characters when screen is flipped, we have to flip them back. */
            int color = m_videoram.op[tile_index + 0x400] & 0x3f;

            tileinfo.set(0,
                    (u32)(m_videoram.op[tile_index] & 0x7f) | (flip_screen() != 0 ? 0x80 : 0U) | (m_galaga_gfxbank << 8),
                    (u32)color,
                    flip_screen() != 0 ? TILE_FLIPX : (u8)0);

            tileinfo.group = (u8)color;
        }


        /***************************************************************************

          Start the video hardware emulation.

        ***************************************************************************/
        protected override void video_start()
        {
            m_fg_tilemap = machine().tilemap().create(m_gfxdecode.op0, get_tile_info, tilemap_scan, 8, 8, 36, 28);  //tilemap_get_info_delegate(FUNC(galaga_state::get_tile_info),this),tilemap_mapper_delegate(FUNC(galaga_state::tilemap_scan),this),8,8,36,28);
            m_fg_tilemap.configure_groups(m_gfxdecode.op0.gfx(0), 0x1f);

            m_galaga_gfxbank = 0;

            save_item(NAME(new { m_galaga_gfxbank }));
        }


        /***************************************************************************
          Memory handlers
        ***************************************************************************/
        void galaga_videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram[offset].op = data;
            m_fg_tilemap.mark_tile_dirty(offset & 0x3ff);
        }


        /***************************************************************************
          Display refresh
        ***************************************************************************/

        static readonly int [,] gfx_offs = new int[2,2]
        {
            { 0, 1 },
            { 2, 3 }
        };

        protected virtual void draw_sprites(bitmap_ind16 bitmap, rectangle cliprect)
        {
            Pointer<uint8_t> spriteram = new Pointer<uint8_t>(m_galaga_ram1.op, 0x380);  //uint8_t *spriteram = &m_galaga_ram1[0x380];
            Pointer<uint8_t> spriteram_2 = new Pointer<uint8_t>(m_galaga_ram2.op, 0x380);  //uint8_t *spriteram_2 = &m_galaga_ram2[0x380];
            Pointer<uint8_t> spriteram_3 = new Pointer<uint8_t>(m_galaga_ram3.op, 0x380);  //uint8_t *spriteram_3 = &m_galaga_ram3[0x380];

            for (int offs = 0; offs < 0x80; offs += 2)
            {
                int sprite = spriteram[offs] & 0x7f;
                int color = spriteram[offs + 1] & 0x3f;
                int sx = spriteram_2[offs + 1] - 40 + 0x100 * (spriteram_3[offs + 1] & 3);
                int sy = 256 - spriteram_2[offs] + 1;   // sprites are buffered and delayed by one scanline
                int flipx = (spriteram_3[offs] & 0x01);
                int flipy = (spriteram_3[offs] & 0x02) >> 1;
                int sizex = (spriteram_3[offs] & 0x04) >> 2;
                int sizey = (spriteram_3[offs] & 0x08) >> 3;

                sy -= 16 * sizey;
                sy = (sy & 0xff) - 32;  // fix wraparound

                if (flip_screen() != 0)
                {
                    flipx ^= 1;
                    flipy ^= 1;
                }

                for (int y = 0; y <= sizey; y++)
                {
                    for (int x = 0; x <= sizex; x++)
                    {
                        m_gfxdecode.op0.gfx(1).transmask(bitmap, cliprect,
                            (u32)(sprite + gfx_offs[y ^ (sizey * flipy), x ^ (sizex * flipx)]),
                            (u32)color,
                            flipx,flipy,
                            sx + 16 * x, sy + 16 * y,
                            m_palette.op0.transpen_mask(m_gfxdecode.op0.gfx(1), (u32)color, 0x0f));
                    }
                }
            }
        }


        u32 screen_update_galaga(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            bitmap.fill(m_palette.op0.black_pen(), cliprect);
            m_starfield.op0.draw_starfield(bitmap, cliprect, 0);
            draw_sprites(bitmap,cliprect);
            m_fg_tilemap.draw(screen, bitmap, cliprect);
            return 0;
        }


        //void WRITE_LINE_MEMBER(galaga_state.screen_vblank_galaga);
        void screen_vblank_galaga(int state)
        {
            // Galaga only scrolls in X direction - the SCROLL_Y pins
            // of the 05XX chip are tied to ground.
            uint8_t speed_index_X = (uint8_t)((m_videolatch.op0.q2_r() << 2) | (m_videolatch.op0.q1_r() << 1) | (m_videolatch.op0.q0_r() << 0));
            uint8_t speed_index_Y = 0;
            m_starfield.op0.set_scroll_speed(speed_index_X,speed_index_Y);

            m_starfield.op0.set_active_starfield_sets((uint8_t)m_videolatch.op0.q3_r(), (uint8_t)(m_videolatch.op0.q4_r() | 2));

            // _STARCLR signal enables/disables starfield
            m_starfield.op0.enable_starfield((uint8_t)m_videolatch.op0.q5_r());
        }
    }
}
