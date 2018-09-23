// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using pen_t = System.UInt32;
using tilemap_memory_index = System.UInt32;
using u8 = System.Byte;


namespace mame
{
    public partial class xevious_state : galaga_state
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

        int TOTAL_COLORS(int gfxn) { return (int)(m_gfxdecode.target.digfx.gfx(gfxn).colors() * m_gfxdecode.target.digfx.gfx(gfxn).granularity()); }

        //PALETTE_INIT_MEMBER(xevious_state,xevious)
        public void palette_init_xevious(palette_device palette)
        {
            ListBytesPointer color_prom = new ListBytesPointer(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();
            int i;

            for (i = 0; i < 128; i++)
            {
                int bit0,bit1,bit2,bit3,r,g,b;

                /* red component */
                bit0 = (color_prom[0] >> 0) & 0x01;
                bit1 = (color_prom[0] >> 1) & 0x01;
                bit2 = (color_prom[0] >> 2) & 0x01;
                bit3 = (color_prom[0] >> 3) & 0x01;
                r = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;
                /* green component */
                bit0 = (color_prom[256] >> 0) & 0x01;
                bit1 = (color_prom[256] >> 1) & 0x01;
                bit2 = (color_prom[256] >> 2) & 0x01;
                bit3 = (color_prom[256] >> 3) & 0x01;
                g = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;
                /* blue component */
                bit0 = (color_prom[2*256] >> 0) & 0x01;
                bit1 = (color_prom[2*256] >> 1) & 0x01;
                bit2 = (color_prom[2*256] >> 2) & 0x01;
                bit3 = (color_prom[2*256] >> 3) & 0x01;
                b = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

                palette.palette_interface().set_indirect_color(i, new rgb_t((byte)r,(byte)g,(byte)b));
                color_prom++;
            }

            /* color 0x80 is used by sprites to mark transparency */
            palette.palette_interface().set_indirect_color(0x80, new rgb_t(0,0,0));

            color_prom += 128;  /* the bottom part of the PROM is unused */
            color_prom += 2*256;
            /* color_prom now points to the beginning of the lookup table */

            /* background tiles */
            for (i = 0; i < TOTAL_COLORS(1); i++)
            {
                palette.palette_interface().set_pen_indirect((pen_t)(m_gfxdecode.target.digfx.gfx(1).colorbase() + i),
                        (UInt16)((color_prom[0] & 0x0f) | ((color_prom[TOTAL_COLORS(1)] & 0x0f) << 4)));

                color_prom++;
            }

            color_prom += TOTAL_COLORS(1);

            /* sprites */
            for (i = 0; i < TOTAL_COLORS(2); i++)
            {
                int c = (color_prom[0] & 0x0f) | ((color_prom[TOTAL_COLORS(2)] & 0x0f) << 4);

                palette.palette_interface().set_pen_indirect((pen_t)(m_gfxdecode.target.digfx.gfx(2).colorbase() + i),
                        (c & 0x80) != 0 ? (UInt16)(c & 0x7f) : (UInt16)0x80);

                color_prom++;
            }

            color_prom += TOTAL_COLORS(2);

            /* foreground characters */
            for (i = 0; i < TOTAL_COLORS(0); i++)
            {
                palette.palette_interface().set_pen_indirect((pen_t)(m_gfxdecode.target.digfx.gfx(0).colorbase() + i),
                        (i % 2 != 0) ? (UInt16)(i / 2) : (UInt16)0x80);
            }
        }


        /***************************************************************************
          Callbacks for the TileMap code
        ***************************************************************************/

        //TILE_GET_INFO_MEMBER(xevious_state::get_fg_tile_info)
        void get_fg_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            byte attr = m_xevious_fg_colorram.target[tile_index];

            /* the hardware has two character sets, one normal and one x-flipped. When
               screen is flipped, character y flip is done by the hardware inverting the
               timing signals, while x flip is done by selecting the 2nd character set.
               We reproduce this here, but since the tilemap system automatically flips
               characters when screen is flipped, we have to flip them back. */
            byte color = (byte)(((attr & 0x03) << 4) | ((attr & 0x3c) >> 2));
            tilemap_global.SET_TILE_INFO_MEMBER(ref tileinfo, 0,
                    (UInt32)(m_xevious_fg_videoram.target[tile_index] | (flip_screen() != 0 ? 0x100 : 0)),
                    color,
                    (byte)(tilemap_global.TILE_FLIPYX((attr & 0xc0) >> 6) ^ (flip_screen() != 0 ? tilemap_global.TILE_FLIPX : 0)));
        }

        //TILE_GET_INFO_MEMBER(xevious_state::get_bg_tile_info)
        void get_bg_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            byte code = m_xevious_bg_videoram.target[tile_index];
            byte attr = m_xevious_bg_colorram.target[tile_index];
            byte color = (byte)(((attr & 0x3c) >> 2) | ((code & 0x80) >> 3) | ((attr & 0x03) << 5));
            tilemap_global.SET_TILE_INFO_MEMBER(ref tileinfo, 1,
                    (UInt32)(code + ((attr & 0x01) << 8)),
                    color,
                    (byte)(tilemap_global.TILE_FLIPYX((attr & 0xc0) >> 6)));
        }


        /***************************************************************************

          Start the video hardware emulation.

        ***************************************************************************/
        //VIDEO_START_MEMBER(xevious_state,xevious)
        public void video_start_xevious()
        {
            m_bg_tilemap = machine().tilemap().create(m_gfxdecode.target.digfx, get_bg_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8,8,64,32);  //tilemap_get_info_delegate(FUNC(xevious_state::get_bg_tile_info),this),TILEMAP_SCAN_ROWS,8,8,64,32);
            m_fg_tilemap = machine().tilemap().create(m_gfxdecode.target.digfx, get_fg_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8,8,64,32);  //tilemap_get_info_delegate(FUNC(xevious_state::get_fg_tile_info),this),TILEMAP_SCAN_ROWS,8,8,64,32);

            m_bg_tilemap.set_scrolldx(-20,288+27);
            m_bg_tilemap.set_scrolldy(-16,-16);
            m_fg_tilemap.set_scrolldx(-32,288+32);
            m_fg_tilemap.set_scrolldy(-18,-10);
            m_fg_tilemap.set_transparent_pen(0);
            m_xevious_bs[0] = 0;
            m_xevious_bs[1] = 0;

            save_item(m_xevious_bs, "m_xevious_bs");
        }


        protected override void draw_sprites(bitmap_ind16 bitmap, rectangle cliprect)
        {
            ListBytesPointer spriteram = new ListBytesPointer(m_xevious_sr3.target, 0x780);  //uint8_t *spriteram = m_xevious_sr3 + 0x780;
            ListBytesPointer spriteram_2 = new ListBytesPointer(m_xevious_sr1.target, 0x780);  //uint8_t *spriteram_2 = m_xevious_sr1 + 0x780;
            ListBytesPointer spriteram_3 = new ListBytesPointer(m_xevious_sr2.target, 0x780);  //uint8_t *spriteram_3 = m_xevious_sr2 + 0x780;
            int offs;
            int sx;
            int sy;

            for (offs = 0;offs < 0x80;offs += 2)
            {
                if ((spriteram[offs + 1] & 0x40) == 0)  /* I'm not sure about this one */
                {
                    int bank,code,color,flipx,flipy;
                    UInt32 transmask;

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

                    transmask = m_palette.target.palette_interface().transpen_mask(m_gfxdecode.target.digfx.gfx(bank), (UInt32)color, 0x80);

                    if ((spriteram_3[offs] & 2) != 0)  /* double height (?) */
                    {
                        if ((spriteram_3[offs] & 1) != 0)  /* double width, double height */
                        {
                            code &= ~3;
                            m_gfxdecode.target.digfx.gfx(bank).transmask(bitmap,cliprect,
                                    (UInt32)(code+3),(UInt32)color,flipx,flipy,
                                    (flipx != 0) ? sx : sx+16, (flipy != 0) ? sy-16 : sy,transmask);
                            m_gfxdecode.target.digfx.gfx(bank).transmask(bitmap,cliprect,
                                    (UInt32)(code+1),(UInt32)color,flipx,flipy,
                                    (flipx != 0) ? sx : sx+16, (flipy != 0) ? sy : sy-16,transmask);
                        }
                        code &= ~2;
                        m_gfxdecode.target.digfx.gfx(bank).transmask(bitmap,cliprect,
                                (UInt32)(code+2),(UInt32)color,flipx,flipy,
                                (flipx != 0) ? sx+16 : sx, (flipy != 0) ? sy-16 : sy,transmask);
                        m_gfxdecode.target.digfx.gfx(bank).transmask(bitmap,cliprect,
                                (UInt32)code,(UInt32)color,flipx,flipy,
                                (flipx != 0) ? sx+16 : sx, (flipy != 0) ? sy : sy-16,transmask);
                    }
                    else if ((spriteram_3[offs] & 1) != 0) /* double width */
                    {
                        code &= ~1;
                        m_gfxdecode.target.digfx.gfx(bank).transmask(bitmap,cliprect,
                                (UInt32)code,(UInt32)color,flipx,flipy,
                                (flipx != 0) ? sx+16 : sx, (flipy != 0) ? sy-16 : sy,transmask);
                        m_gfxdecode.target.digfx.gfx(bank).transmask(bitmap,cliprect,
                                (UInt32)(code+1),(UInt32)color,flipx,flipy,
                                (flipx != 0) ? sx : sx+16, (flipy != 0) ? sy-16 : sy,transmask);
                    }
                    else    /* normal */
                    {
                        m_gfxdecode.target.digfx.gfx(bank).transmask(bitmap,cliprect,
                                (UInt32)code,(UInt32)color,flipx,flipy,sx,sy,transmask);
                    }
                }
            }
        }


        public UInt32 screen_update_xevious(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            m_bg_tilemap.draw(screen, bitmap, cliprect, 0,0);
            draw_sprites(bitmap,cliprect);
            m_fg_tilemap.draw(screen, bitmap, cliprect, 0,0);

            return 0;
        }


        /***************************************************************************
          Memory handlers
        ***************************************************************************/

        //WRITE8_MEMBER( xevious_state::xevious_fg_videoram_w )
        public void xevious_fg_videoram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_xevious_fg_videoram.target[offset] = data;
            m_fg_tilemap.mark_tile_dirty(offset);
        }

        //WRITE8_MEMBER( xevious_state::xevious_fg_colorram_w )
        public void xevious_fg_colorram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_xevious_fg_colorram.target[offset] = data;
            m_fg_tilemap.mark_tile_dirty(offset);
        }

        //WRITE8_MEMBER( xevious_state::xevious_bg_videoram_w )
        public void xevious_bg_videoram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_xevious_bg_videoram.target[offset] = data;
            m_bg_tilemap.mark_tile_dirty(offset);
        }

        //WRITE8_MEMBER( xevious_state::xevious_bg_colorram_w )
        public void xevious_bg_colorram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_xevious_bg_colorram.target[offset] = data;
            m_bg_tilemap.mark_tile_dirty(offset);
        }

        //WRITE8_MEMBER( xevious_state::xevious_vh_latch_w )
        public void xevious_vh_latch_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            int reg;
            int scroll = (int)(data + ((offset&0x01)<<8));   /* A0 -> D8 */

            reg = (int)((offset&0xf0)>>4);

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
                    flip_screen_set((UInt32)(scroll & 1));
                    break;
                default:
                    logerror("CRTC WRITE REG: {0}  Data: {1}\n", reg, scroll);
                    break;
            }
        }


        /* emulation for schematic 9B */
        //WRITE8_MEMBER( xevious_state::xevious_bs_w )
        public void xevious_bs_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_xevious_bs[offset & 1] = data;
        }

        //READ8_MEMBER( xevious_state::xevious_bb_r )
        public u8 xevious_bb_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            ListBytesPointer rom2a = new ListBytesPointer(memregion("gfx4").base_());  //uint8_t *rom2a = memregion("gfx4")->base();
            ListBytesPointer rom2b = new ListBytesPointer(rom2a, 0x1000);  //uint8_t *rom2b = rom2a+0x1000;
            ListBytesPointer rom2c = new ListBytesPointer(rom2a, 0x3000);  //uint8_t *rom2c = rom2a+0x3000;
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
                dat2 = bitswap(dat2, 6,7,5,4,3,2,1,0);
                /* flip x & y */
                if ((dat1 & 0x400) != 0) dat2 ^= 0x40;
                if ((dat1 & 0x200) != 0) dat2 ^= 0x80;
            }

            return (byte)dat2;
        }
    }
}
