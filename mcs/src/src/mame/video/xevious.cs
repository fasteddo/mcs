// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using indirect_pen_t = System.UInt16;  //typedef u16 indirect_pen_t;
using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using tilemap_memory_index = System.UInt32;  //typedef u32 tilemap_memory_index;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    partial class xevious_state : galaga_state
    {
        /***************************************************************************

          Convert the color PROMs into a more useable format.

          Xevious has three 256x4 palette PROMs (one per gun) and four 512x4 lookup
          table PROMs (two for sprites, two for background tiles; foreground
          characters map directly to a palette color without using a PROM).
          The palette PROMs are connected to the RGB output this way:

          bit 3 -- 220 ohm resistor  -- RED/GREEN/BLUE
                -- 470 ohm resistor  -- RED/GREEN/BLUE
                -- 1  kohm resistor  -- RED/GREEN/BLUE
          bit 0 -- 2.2kohm resistor  -- RED/GREEN/BLUE

        ***************************************************************************/

        void xevious_palette(palette_device palette)
        {
            Pointer<uint8_t> color_prom = new Pointer<uint8_t>(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();
            Func<int, int> TOTAL_COLORS = (int gfxn) => { return (int)(m_gfxdecode.op[0].digfx.gfx(gfxn).colors() * m_gfxdecode.op[0].digfx.gfx(gfxn).granularity()); };

            for (int i = 0; i < 128; i++)
            {
                int bit0;
                int bit1;
                int bit2;
                int bit3;

                // red component
                bit0 = g.BIT(color_prom[0], 0);
                bit1 = g.BIT(color_prom[0], 1);
                bit2 = g.BIT(color_prom[0], 2);
                bit3 = g.BIT(color_prom[0], 3);
                int r = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;
                // green component
                bit0 = g.BIT(color_prom[256], 0);
                bit1 = g.BIT(color_prom[256], 1);
                bit2 = g.BIT(color_prom[256], 2);
                bit3 = g.BIT(color_prom[256], 3);
                int gr = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;
                // blue component
                bit0 = g.BIT(color_prom[2*256], 0);
                bit1 = g.BIT(color_prom[2*256], 1);
                bit2 = g.BIT(color_prom[2*256], 2);
                bit3 = g.BIT(color_prom[2*256], 3);
                int b = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

                palette.dipalette.set_indirect_color(i, new rgb_t((u8)r, (u8)gr, (u8)b));
                color_prom++;
            }

            // color 0x80 is used by sprites to mark transparency
            palette.dipalette.set_indirect_color(0x80, new rgb_t(0, 0, 0));

            color_prom += 128;  // the bottom part of the PROM is unused
            color_prom += 2 * 256;
            // color_prom now points to the beginning of the lookup table

            // background tiles
            for (int i = 0; i < TOTAL_COLORS(1); i++)
            {
                palette.dipalette.set_pen_indirect(
                        (pen_t)(m_gfxdecode.op[0].digfx.gfx(1).colorbase() + i),
                        (indirect_pen_t)((color_prom[0] & 0x0f) | ((color_prom[TOTAL_COLORS(1)] & 0x0f) << 4)));

                color_prom++;
            }

            color_prom += TOTAL_COLORS(1);

            // sprites
            for (int i = 0; i < TOTAL_COLORS(2); i++)
            {
                int c = (color_prom[0] & 0x0f) | ((color_prom[TOTAL_COLORS(2)] & 0x0f) << 4);

                palette.dipalette.set_pen_indirect(
                        (pen_t)(m_gfxdecode.op[0].digfx.gfx(2).colorbase() + i),
                        (c & 0x80) != 0 ? (indirect_pen_t)(c & 0x7f) : (indirect_pen_t)0x80);

                color_prom++;
            }

            color_prom += TOTAL_COLORS(2);

            // foreground characters
            for (int i = 0; i < TOTAL_COLORS(0); i++)
            {
                palette.dipalette.set_pen_indirect(
                        (pen_t)(m_gfxdecode.op[0].digfx.gfx(0).colorbase() + i),
                        g.BIT(i, 0) != 0 ? (indirect_pen_t)(i >> 1) : (indirect_pen_t)0x80);
            }
        }


        /***************************************************************************
          Callbacks for the TileMap code
        ***************************************************************************/

        //TILE_GET_INFO_MEMBER(xevious_state::get_fg_tile_info)
        void get_fg_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            uint8_t attr = m_xevious_fg_colorram.op[tile_index];

            /* the hardware has two character sets, one normal and one x-flipped. When
               screen is flipped, character y flip is done by the hardware inverting the
               timing signals, while x flip is done by selecting the 2nd character set.
               We reproduce this here, but since the tilemap system automatically flips
               characters when screen is flipped, we have to flip them back. */
            uint8_t color = (uint8_t)(((attr & 0x03) << 4) | ((attr & 0x3c) >> 2));
            tileinfo.set(0,
                    (u32)(m_xevious_fg_videoram.op[tile_index] | (flip_screen() != 0 ? 0x100 : 0)),
                    color,
                    (u8)(g.TILE_FLIPYX((attr & 0xc0) >> 6) ^ (flip_screen() != 0 ? g.TILE_FLIPX : 0)));
        }

        //TILE_GET_INFO_MEMBER(xevious_state::get_bg_tile_info)
        void get_bg_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            uint8_t code = m_xevious_bg_videoram.op[tile_index];
            uint8_t attr = m_xevious_bg_colorram.op[tile_index];
            uint8_t color = (uint8_t)(((attr & 0x3c) >> 2) | ((code & 0x80) >> 3) | ((attr & 0x03) << 5));
            tileinfo.set(1,
                    (u32)(code + ((attr & 0x01) << 8)),
                    color,
                    g.TILE_FLIPYX((attr & 0xc0) >> 6));
        }


        /***************************************************************************

          Start the video hardware emulation.

        ***************************************************************************/
        //VIDEO_START_MEMBER(xevious_state,xevious)
        void video_start_xevious()
        {
            m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op[0].digfx, get_bg_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8,8,64,32);  //tilemap_get_info_delegate(FUNC(xevious_state::get_bg_tile_info),this),TILEMAP_SCAN_ROWS,8,8,64,32);
            m_fg_tilemap = machine().tilemap().create(m_gfxdecode.op[0].digfx, get_fg_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8,8,64,32);  //tilemap_get_info_delegate(FUNC(xevious_state::get_fg_tile_info),this),TILEMAP_SCAN_ROWS,8,8,64,32);

            m_bg_tilemap.set_scrolldx(-20,288+27);
            m_bg_tilemap.set_scrolldy(-16,-16);
            m_fg_tilemap.set_scrolldx(-32,288+32);
            m_fg_tilemap.set_scrolldy(-18,-10);
            m_fg_tilemap.set_transparent_pen(0);
            m_xevious_bs[0] = 0;
            m_xevious_bs[1] = 0;

            save_item(g.NAME(new { m_xevious_bs }));
        }


        protected override void draw_sprites(bitmap_ind16 bitmap, rectangle cliprect)
        {
            Pointer<uint8_t> spriteram = new Pointer<uint8_t>(m_xevious_sr3.op, 0x780);  //uint8_t *spriteram = m_xevious_sr3 + 0x780;
            Pointer<uint8_t> spriteram_2 = new Pointer<uint8_t>(m_xevious_sr1.op, 0x780);  //uint8_t *spriteram_2 = m_xevious_sr1 + 0x780;
            Pointer<uint8_t> spriteram_3 = new Pointer<uint8_t>(m_xevious_sr2.op, 0x780);  //uint8_t *spriteram_3 = m_xevious_sr2 + 0x780;
            int offs;
            int sx;
            int sy;

            for (offs = 0;offs < 0x80;offs += 2)
            {
                if ((spriteram[offs + 1] & 0x40) == 0)  /* I'm not sure about this one */
                {
                    int bank;
                    int code;
                    int color;
                    int flipx;
                    int flipy;
                    uint32_t transmask;

                    if ((spriteram_3[offs] & 0x80) != 0)
                    {
                        bank = 2;
                        code = (spriteram[offs] & 0x3f) + 0x100;
                    }
                    else
                    {
                        bank = 2;
                        code = spriteram[offs];
                    }

                    color = spriteram[offs + 1] & 0x7f;
                    flipx = spriteram_3[offs] & 4;
                    flipy = spriteram_3[offs] & 8;

                    sx = spriteram_2[offs + 1] - 40 + 0x100*(spriteram_3[offs + 1] & 1);
                    sy = 28*8-spriteram_2[offs]-1;

                    if (flip_screen() != 0)
                    {
                        flipx = flipx == 0 ? 1 : 0;
                        flipy = flipy == 0 ? 1 : 0;
                    }

                    transmask = m_palette.op[0].dipalette.transpen_mask(m_gfxdecode.op[0].digfx.gfx(bank), (u32)color, 0x80);

                    if ((spriteram_3[offs] & 2) != 0)  /* double height (?) */
                    {
                        if ((spriteram_3[offs] & 1) != 0)  /* double width, double height */
                        {
                            code &= ~3;
                            m_gfxdecode.op[0].digfx.gfx(bank).transmask(bitmap,cliprect,
                                    (u32)(code+3),(u32)color,flipx,flipy,
                                    (flipx != 0) ? sx : sx+16, (flipy != 0) ? sy-16 : sy,transmask);
                            m_gfxdecode.op[0].digfx.gfx(bank).transmask(bitmap,cliprect,
                                    (u32)(code+1),(u32)color,flipx,flipy,
                                    (flipx != 0) ? sx : sx+16, (flipy != 0) ? sy : sy-16,transmask);
                        }
                        code &= ~2;
                        m_gfxdecode.op[0].digfx.gfx(bank).transmask(bitmap,cliprect,
                                (u32)(code+2),(u32)color,flipx,flipy,
                                (flipx != 0) ? sx+16 : sx, (flipy != 0) ? sy-16 : sy,transmask);
                        m_gfxdecode.op[0].digfx.gfx(bank).transmask(bitmap,cliprect,
                                (u32)code,(u32)color,flipx,flipy,
                                (flipx != 0) ? sx+16 : sx, (flipy != 0) ? sy : sy-16,transmask);
                    }
                    else if ((spriteram_3[offs] & 1) != 0) /* double width */
                    {
                        code &= ~1;
                        m_gfxdecode.op[0].digfx.gfx(bank).transmask(bitmap,cliprect,
                                (u32)code,(u32)color,flipx,flipy,
                                (flipx != 0) ? sx+16 : sx, (flipy != 0) ? sy-16 : sy,transmask);
                        m_gfxdecode.op[0].digfx.gfx(bank).transmask(bitmap,cliprect,
                                (u32)(code+1),(u32)color,flipx,flipy,
                                (flipx != 0) ? sx : sx+16, (flipy != 0) ? sy-16 : sy,transmask);
                    }
                    else    /* normal */
                    {
                        m_gfxdecode.op[0].digfx.gfx(bank).transmask(bitmap,cliprect,
                                (u32)code,(u32)color,flipx,flipy,sx,sy,transmask);
                    }
                }
            }
        }


        u32 screen_update_xevious(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            m_bg_tilemap.draw(screen, bitmap, cliprect, 0,0);
            draw_sprites(bitmap,cliprect);
            m_fg_tilemap.draw(screen, bitmap, cliprect, 0,0);

            return 0;
        }


        /***************************************************************************
          Memory handlers
        ***************************************************************************/

        void xevious_fg_videoram_w(offs_t offset, uint8_t data)
        {
            m_xevious_fg_videoram.op[offset] = data;
            m_fg_tilemap.mark_tile_dirty(offset);
        }

        void xevious_fg_colorram_w(offs_t offset, uint8_t data)
        {
            m_xevious_fg_colorram.op[offset] = data;
            m_fg_tilemap.mark_tile_dirty(offset);
        }

        void xevious_bg_videoram_w(offs_t offset, uint8_t data)
        {
            m_xevious_bg_videoram.op[offset] = data;
            m_bg_tilemap.mark_tile_dirty(offset);
        }

        void xevious_bg_colorram_w(offs_t offset, uint8_t data)
        {
            m_xevious_bg_colorram.op[offset] = data;
            m_bg_tilemap.mark_tile_dirty(offset);
        }

        void xevious_vh_latch_w(offs_t offset, uint8_t data)
        {
            int reg;
            int scroll = (int)(data + ((offset&0x01)<<8));   /* A0 -> D8 */

            reg = (int)((offset & 0xf0) >> 4);

            switch (reg)
            {
                case 0:
                    m_bg_tilemap.set_scrollx(0,scroll);
                    break;
                case 1:
                    m_bg_tilemap.set_scrollx(0,scroll);
                    break;
                case 2:
                    m_bg_tilemap.set_scrolly(0,scroll);
                    break;
                case 3:
                    m_bg_tilemap.set_scrolly(0,scroll);
                    break;
                case 7:
                    flip_screen_set((u32)(scroll & 1));
                    break;
                default:
                    logerror("CRTC WRITE REG: {0}  Data: {1}\n", reg, scroll);
                    break;
            }
        }


        /* emulation for schematic 9B */
        void xevious_bs_w(offs_t offset, uint8_t data)
        {
            m_xevious_bs[offset & 1] = data;
        }


        uint8_t xevious_bb_r(offs_t offset)
        {
            Pointer<uint8_t> rom2a = new Pointer<uint8_t>(memregion("gfx4").base_());  //uint8_t *rom2a = memregion("gfx4")->base();
            Pointer<uint8_t> rom2b = new Pointer<uint8_t>(rom2a, 0x1000);  //uint8_t *rom2b = rom2a+0x1000;
            Pointer<uint8_t> rom2c = new Pointer<uint8_t>(rom2a, 0x3000);  //uint8_t *rom2c = rom2a+0x3000;
            int adr_2b;
            int adr_2c;
            int dat1;
            int dat2;

            /* get BS to 12 bit data from 2A,2B */
            adr_2b = ((m_xevious_bs[1] & 0x7e) << 6) | ((m_xevious_bs[0] & 0xfe) >> 1);

            if ((adr_2b & 1) != 0)
            {
                /* high bits select */
                dat1 = ((rom2a[adr_2b >> 1] & 0xf0) << 4) | rom2b[adr_2b];
            }
            else
            {
                /* low bits select */
                dat1 = ((rom2a[adr_2b >> 1] & 0x0f) << 8) | rom2b[adr_2b];
            }

            adr_2c = ((dat1 & 0x1ff) << 2) | ((m_xevious_bs[1] & 1) << 1) | (m_xevious_bs[0] & 1);
            if ((dat1 & 0x400) != 0) adr_2c ^= 1;
            if ((dat1 & 0x200) != 0) adr_2c ^= 2;

            if ((offset & 1) != 0)
            {
                /* return BB1 */
                dat2 = rom2c[adr_2c | 0x800];
            }
            else
            {
                /* return BB0 */
                dat2 = rom2c[adr_2c];
                /* swap bit 6 & 7 */
                dat2 = g.bitswap(dat2, 6,7,5,4,3,2,1,0);
                /* flip x & y */
                if ((dat1 & 0x400) != 0) dat2 ^= 0x40;
                if ((dat1 & 0x200) != 0) dat2 ^= 0x80;
            }

            return (uint8_t)dat2;
        }
    }
}
