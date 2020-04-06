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
using uint8_t = System.Byte;
using uint16_t = System.UInt16;


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
            ListBytesPointer color_prom = new ListBytesPointer(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();

            // core palette
            for (int i = 0; i < 32; i++)
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

                palette.palette_interface.set_indirect_color(i, new rgb_t((byte)r, (byte)g, (byte)b));
                color_prom++;
            }

            // palette for the stars
            for (int i = 0; i < 64; i++)
            {
                int [] map = new int[4] { 0x00, 0x47, 0x97, 0xde };

                int r = map[(i >> 0) & 0x03];
                int g = map[(i >> 2) & 0x03];
                int b = map[(i >> 4) & 0x03];

                palette.palette_interface.set_indirect_color(32 + i, new rgb_t((byte)r, (byte)g, (byte)b));
            }

            // characters
            for (int i = 0; i < 64*4; i++)
            {
                palette.palette_interface.set_pen_indirect((pen_t)i, (indirect_pen_t)((color_prom[0] & 0x0f) | 0x10));
                color_prom++;
            }

            // sprites
            for (int i = 0; i < 64*4; i++)
            {
                palette.palette_interface.set_pen_indirect((pen_t)(64*4 + i), (indirect_pen_t)((color_prom[0] & 0x0f)));
                color_prom++;
            }

            // now the stars
            for (int i = 0; i < 64; i++)
                palette.palette_interface.set_pen_indirect((pen_t)(64*4 + 64*4 + i), (indirect_pen_t)(32 + i));
        }


        /***************************************************************************
          Callbacks for the TileMap code
        ***************************************************************************/

        /* convert from 32x32 to 36x28 */
        //TILEMAP_MAPPER_MEMBER(galaga_state::tilemap_scan)
        protected tilemap_memory_index tilemap_scan(UInt32 col, UInt32 row, UInt32 num_cols, UInt32 num_rows)
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
            int color = m_videoram[tile_index + 0x400] & 0x3f;

            SET_TILE_INFO_MEMBER(ref tileinfo, 0,
                    (UInt32)((m_videoram[tile_index] & 0x7f) | (flip_screen() != 0 ? 0x80 : 0) | (m_galaga_gfxbank << 8)),
                    (UInt32)color,
                    flip_screen() != 0 ? TILE_FLIPX : (byte)0);

            tileinfo.group = (byte)color;
        }


        /***************************************************************************

          Start the video hardware emulation.

        ***************************************************************************/
        //VIDEO_START_MEMBER(galaga_state,galaga)
        void video_start_galaga()
        {
            m_fg_tilemap = machine().tilemap().create(m_gfxdecode.target.digfx, get_tile_info, tilemap_scan, 8, 8, 36, 28);  //tilemap_get_info_delegate(FUNC(galaga_state::get_tile_info),this),tilemap_mapper_delegate(FUNC(galaga_state::tilemap_scan),this),8,8,36,28);
            m_fg_tilemap.configure_groups(m_gfxdecode.target.digfx.gfx(0), 0x1f);

            m_galaga_gfxbank = 0;

            save_item(m_galaga_gfxbank, "m_galaga_gfxbank");
        }


        /***************************************************************************
          Memory handlers
        ***************************************************************************/
        //WRITE8_MEMBER(galaga_state::galaga_videoram_w)
        void galaga_videoram_w(address_space space, offs_t offset, byte data, byte mem_mask = 0xff)
        {
            m_videoram[offset] = data;
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

        protected virtual void draw_sprites(bitmap_ind16 bitmap, rectangle cliprect )
        {
            ListBytesPointer spriteram = new ListBytesPointer(m_galaga_ram1.target, 0x380);  //uint8_t *spriteram = m_galaga_ram1 + 0x380;
            ListBytesPointer spriteram_2 = new ListBytesPointer(m_galaga_ram2.target, 0x380);  //uint8_t *spriteram_2 = m_galaga_ram2 + 0x380;
            ListBytesPointer spriteram_3 = new ListBytesPointer(m_galaga_ram3.target, 0x380);  //uint8_t *spriteram_3 = m_galaga_ram3 + 0x380;
            int offs;

            for (offs = 0; offs < 0x80; offs += 2)
            {
                int sprite = spriteram[offs] & 0x7f;
                int color = spriteram[offs+1] & 0x3f;
                int sx = spriteram_2[offs+1] - 40 + 0x100*(spriteram_3[offs+1] & 3);
                int sy = 256 - spriteram_2[offs] + 1;   // sprites are buffered and delayed by one scanline
                int flipx = (spriteram_3[offs] & 0x01);
                int flipy = (spriteram_3[offs] & 0x02) >> 1;
                int sizex = (spriteram_3[offs] & 0x04) >> 2;
                int sizey = (spriteram_3[offs] & 0x08) >> 3;
                int x,y;

                sy -= 16 * sizey;
                sy = (sy & 0xff) - 32;  // fix wraparound

                if (flip_screen() != 0)
                {
                    flipx ^= 1;
                    flipy ^= 1;
                }

                for (y = 0; y <= sizey; y++)
                {
                    for (x = 0; x <= sizex; x++)
                    {
                        m_gfxdecode.target.digfx.gfx(1).transmask(bitmap,cliprect,
                            (UInt32)(sprite + gfx_offs[y ^ (sizey * flipy), x ^ (sizex * flipx)]),
                            (UInt32)color,
                            flipx,flipy,
                            sx + 16*x, sy + 16*y,
                            m_palette.target.palette_interface.transpen_mask(m_gfxdecode.target.digfx.gfx(1), (UInt32)color, 0x0f));
                    }
                }
            }
        }


        u32 screen_update_galaga(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            bitmap.fill(m_palette.target.palette_interface.black_pen(), cliprect);
            m_starfield.target.draw_starfield(bitmap,cliprect,0);
            draw_sprites(bitmap,cliprect);
            m_fg_tilemap.draw(screen, bitmap, cliprect, 0,0);
            return 0;
        }


        //void WRITE_LINE_MEMBER(galaga_state.screen_vblank_galaga);
        void screen_vblank_galaga(int state)
        {
            // Galaga only scrolls in X direction - the SCROLL_Y pins
            // of the 05XX chip are tied to ground.
            uint8_t speed_index_X = (uint8_t)((m_videolatch.target.q2_r() << 2) | (m_videolatch.target.q1_r() << 1) | (m_videolatch.target.q0_r() << 0));
            uint8_t speed_index_Y = 0;
            m_starfield.target.set_scroll_speed(speed_index_X,speed_index_Y);

            m_starfield.target.set_active_starfield_sets((uint8_t)m_videolatch.target.q3_r(), (uint8_t)(m_videolatch.target.q4_r() | 2));

            // _STARCLR signal enables/disables starfield
            m_starfield.target.enable_starfield((uint8_t)m_videolatch.target.q5_r());
        }
    }
}
