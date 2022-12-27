// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using indirect_pen_t = System.UInt16;  //typedef u16 indirect_pen_t;
using MemoryU8 = mame.MemoryContainer<System.Byte>;
using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using PointerU8 = mame.Pointer<System.Byte>;
using tilemap_memory_index = System.UInt32;  //typedef u32 tilemap_memory_index;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.drawgfx_global;
using static mame.emucore_global;
using static mame.util;


namespace mame
{
    partial class polepos_state : driver_device
    {
        void polepos_palette(palette_device palette)
        {
            PointerU8 color_prom = new PointerU8(memregion("proms").base_());  //uint8_t const *const color_prom = memregion("proms")->base();

            /*******************************************************
             * Color PROMs
             * Sheet 15B: middle, 136014-137,138,139
             * Inputs: MUX0 ... MUX3, ALPHA/BACK, SPRITE/BACK, 128V, COMPBLANK
             *
             * Note that we only decode the lower 128 colors because
             * the upper 128 are all black and used during the
             * horizontal and vertical blanking periods.
             * The purpose of the 128V input is to use a different palette for the
             * background and for the road; it is irrelevant for alpha and
             * sprites because their palette is the same in both halves.
             * Anyway, we emulate that to a certain extent, using different
             * colortables for the two halves of the screen. We don't support the
             * palette change in the middle of a sprite, however.
             * Also, note that priority encoding is done is such a way that alpha
             * will use palette bank 2 or 3 depending on whether there is a sprite
             * below the pixel or not. That would be tricky to emulate, and it's
             * not needed because of course the two banks are the same.
             *******************************************************/
            for (int i = 0; i < 128; i++)
            {
                int bit0, bit1, bit2, bit3;

                // Sheet 15B: 136014-0137 red component
                bit0 = BIT(color_prom[0x000 + i], 0);
                bit1 = BIT(color_prom[0x000 + i], 1);
                bit2 = BIT(color_prom[0x000 + i], 2);
                bit3 = BIT(color_prom[0x000 + i], 3);
                int r = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

                // Sheet 15B: 136014-0138 green component
                bit0 = BIT(color_prom[0x100 + i], 0);
                bit1 = BIT(color_prom[0x100 + i], 1);
                bit2 = BIT(color_prom[0x100 + i], 2);
                bit3 = BIT(color_prom[0x100 + i], 3);
                int g = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

                // Sheet 15B: 136014-0139 blue component
                bit0 = BIT(color_prom[0x200 + i], 0);
                bit1 = BIT(color_prom[0x200 + i], 1);
                bit2 = BIT(color_prom[0x200 + i], 2);
                bit3 = BIT(color_prom[0x200 + i], 3);
                int b = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

                palette.set_indirect_color(i, new rgb_t((uint8_t)r, (uint8_t)g, (uint8_t)b));
            }

            /*******************************************************
             * Alpha colors (colors 0x000-0x1ff)
             * Sheet 15B: top left, 136014-140
             * Inputs: SHFT0, SHFT1 and CHA8* ... CHA13*
             *******************************************************/
            for (int i = 0; i < 64*4; i++)
            {
                int color = color_prom[0x300 + i];
                palette.set_pen_indirect((pen_t)(0x0000 + i), (color != 15) ? (indirect_pen_t)(0x020 + color) : (indirect_pen_t)0x2f);
                palette.set_pen_indirect((pen_t)(0x0100 + i), (color != 15) ? (indirect_pen_t)(0x060 + color) : (indirect_pen_t)0x2f);
            }

            /*******************************************************
             * Background colors (colors 0x200-0x2ff)
             * Sheet 13A: left, 136014-141
             * Inputs: SHFT2, SHFT3 and CHA8 ... CHA13
             * The background is only in the top half of the screen
             *******************************************************/
            for (int i = 0; i < 64*4; i++)
            {
                int color = color_prom[0x400 + i];
                palette.set_pen_indirect((pen_t)(0x0200 + i), (indirect_pen_t)(0x000 + color));
            }

            /*******************************************************
             * Sprite colors (colors 0x300-0xaff)
             * Sheet 14B: right, 136014-146
             * Inputs: CUSTOM0 ... CUSTOM3 and DATA0 ... DATA5
             *******************************************************/
            for (int i = 0; i < 64*16; i++)
            {
                int color = color_prom[0xc00 + i];
                palette.set_pen_indirect((pen_t)(0x0300 + i), (color != 15) ? (indirect_pen_t)(0x010 + color) : (indirect_pen_t)0x1f);
                palette.set_pen_indirect((pen_t)(0x0700 + i), (color != 15) ? (indirect_pen_t)(0x050 + color) : (indirect_pen_t)0x1f);
            }

            /*******************************************************
             * Road colors (colors 0xb00-0x0eff)
             * Sheet 13A: bottom left, 136014-145
             * Inputs: R1 ... R6 and CHA0 ... CHA3
             * The road is only in the bottom half of the screen
             *******************************************************/
            for (int i = 0; i < 64*16; i++)
            {
                int color = color_prom[0x800 + i];
                palette.set_pen_indirect((pen_t)(0x0b00 + i), (indirect_pen_t)(0x040 + color));
            }

            /* 136014-142, 136014-143, 136014-144 Vertical position modifiers */
            for (int i = 0; i < 256; i++)
            {
                int j = color_prom[0x500 + i] + (color_prom[0x600 + i] << 4) + (color_prom[0x700 + i] << 8);
                m_vertical_position_modifier[i] = (uint16_t)j;
            }
        }


        /***************************************************************************
          Callbacks for the TileMap code
        ***************************************************************************/
        //TILE_GET_INFO_MEMBER(polepos_state::bg_get_tile_info)
        void bg_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            uint16_t word = m_view16_memory[tile_index].op;
            int code = (word & 0xff) | ((word & 0x4000) >> 6);
            int color = (word & 0x3f00) >> 8;
            tileinfo.set(1,
                    (uint32_t)code,
                    (uint32_t)color,
                    0);
        }


        //TILE_GET_INFO_MEMBER(polepos_state::tx_get_tile_info)
        void tx_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            uint16_t word = m_alpha16_memory[tile_index].op;
            int code = (word & 0xff) | ((word & 0x4000) >> 6);
            int color = (word & 0x3f00) >> 8;

            /* I assume the purpose of CHACL is to allow the Z80 to control
               the display (therefore using only the bottom 8 bits of tilemap RAM)
               in case the Z8002 is not working. */
            if (m_chacl == 0)
            {
                code &= 0xff;
                color = 0;
            }

            /* 128V input to the palette PROM */
            if (tile_index >= 32*16) color |= 0x40;

            tileinfo.set(0,
                    (uint32_t)code,
                    (uint32_t)color,
                    0);
            tileinfo.group = (uint8_t)color;
        }


        /***************************************************************************
          Start the video hardware emulation.
        ***************************************************************************/
        protected override void video_start()
        {
            m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op0, bg_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_COLS, 8, 8, 64, 16);
            m_tx_tilemap = machine().tilemap().create(m_gfxdecode.op0, tx_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8, 8, 32, 32);

            m_tx_tilemap.configure_groups(m_gfxdecode.op0.gfx(0), 0x2f);

            save_item(NAME(new { m_road16_vscroll }));
            save_item(NAME(new { m_chacl }));
            save_item(NAME(new { m_scroll }));
        }


        /***************************************************************************
          Sprite memory
        ***************************************************************************/
        uint8_t sprite_r(offs_t offset)
        {
            return (uint8_t)(m_sprite16_memory[offset].op & 0xff);
        }


        void sprite_w(offs_t offset, uint8_t data)
        {
            m_sprite16_memory[offset].op = (uint16_t)((m_sprite16_memory[offset].op & 0xff00) | data);
        }


        /***************************************************************************
          Road memory
        ***************************************************************************/
        uint8_t road_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
            return m_road16_memory[offset] & 0xff;
#endif
        }


        void road_w(offs_t offset, uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            m_road16_memory[offset] = (m_road16_memory[offset] & 0xff00) | data;
#endif
        }


        void road16_vscroll_w(offs_t offset, uint16_t data, uint16_t mem_mask)
        {
            throw new emu_unimplemented();
#if false
            COMBINE_DATA(&m_road16_vscroll);
#endif
        }


        /***************************************************************************
          View memory
        ***************************************************************************/
        void view16_w(offs_t offset, uint16_t data, uint16_t mem_mask)
        {
            throw new emu_unimplemented();
#if false
            COMBINE_DATA(&m_view16_memory[offset]);
            if (offset < 0x400)
                m_bg_tilemap->mark_tile_dirty(offset);
#endif
        }


        uint8_t view_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
            return m_view16_memory[offset] & 0xff;
#endif
        }


        void view_w(offs_t offset, uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            m_view16_memory[offset] = (m_view16_memory[offset] & 0xff00) | data;
            if (offset < 0x400)
                m_bg_tilemap->mark_tile_dirty(offset);
#endif
        }


        void view16_hscroll_w(offs_t offset, uint16_t data, uint16_t mem_mask)
        {
            throw new emu_unimplemented();
#if false
            COMBINE_DATA(&m_scroll);
            m_bg_tilemap->set_scrollx(0,m_scroll);
#endif
        }


        //WRITE_LINE_MEMBER(polepos_state::chacl_w)
        public void chacl_w(int state)
        {
            m_chacl = state;
            m_tx_tilemap.mark_all_dirty();
        }


        /***************************************************************************
          Alpha memory
        ***************************************************************************/
        void alpha16_w(offs_t offset, uint16_t data, uint16_t mem_mask)
        {
            throw new emu_unimplemented();
#if false
            COMBINE_DATA(&m_alpha16_memory[offset]);
            m_tx_tilemap->mark_tile_dirty(offset);
#endif
        }


        uint8_t alpha_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
            return m_alpha16_memory[offset] & 0xff;
#endif
        }


        void alpha_w(offs_t offset, uint8_t data)
        {
            throw new emu_unimplemented();
#if false
            m_alpha16_memory[offset] = (m_alpha16_memory[offset] & 0xff00) | data;
            m_tx_tilemap->mark_tile_dirty(offset);
#endif
        }


        /***************************************************************************
          Display refresh
        ***************************************************************************/
        void draw_road(bitmap_ind16 bitmap)
        {
            PointerU8 road_control = new PointerU8(memregion("gfx5").base_());  //const uint8_t *road_control = memregion("gfx5")->base();
            PointerU8 road_bits1 = road_control + 0x2000;  //const uint8_t *road_bits1 = road_control + 0x2000;
            PointerU8 road_bits2 = road_control + 0x4000;  //const uint8_t *road_bits2 = road_control + 0x4000;

            /* loop over the lower half of the screen */
            for (int y = 128; y < 256; y++)
            {
                int xoffs;
                int yoffs;
                int xscroll;
                int roadpal;
                MemoryU8 scanline = new MemoryU8((256 + 8) * 2, true);  //uint16_t scanline[256 + 8];
                PointerU16 dest = new PointerU16(scanline);
                pen_t pen_base;

                /* first add the vertical position modifier and the vertical scroll */
                yoffs = ((m_vertical_position_modifier[y] + m_road16_vscroll) >> 3) & 0x1ff;

                /* then use that as a lookup into the road memory */
                roadpal = m_road16_memory[yoffs].op & 15;

                /* this becomes the palette base for the scanline */
                pen_base = (pen_t)(0x0b00 + (roadpal << 6));

                /* now fetch the horizontal scroll offset for this scanline */
                xoffs = m_road16_memory[0x380 + (y & 0x7f)].op & 0x3ff;

                /* the road is drawn in 8-pixel chunks, so round downward and adjust the base */
                /* note that we assume there is at least 8 pixels of slop on the left/right */
                xscroll = xoffs & 7;
                xoffs &= ~7;

                /* loop over 8-pixel chunks */
                for (int x = 0; x < 256 / 8 + 1; x++, xoffs += 8)
                {
                    /* if the 0x200 bit of the xoffset is set, a special pin on the custom */
                    /* chip is set and the /CE and /OE for the road chips is disabled */
                    if ((xoffs & 0x200) != 0)
                    {
                        /* in this case, it looks like we just fill with 0 */
                        for (int i = 0; i < 8; i++)
                            { dest.op = (uint16_t)(pen_base | 0);  dest++; }  //*dest++ = pen_base | 0;
                    }

                    /* otherwise, we clock in the bits and compute the road value */
                    else
                    {
                        /* the road ROM offset comes from the current scanline and the X offset */
                        int romoffs = ((y & 0x07f) << 6) + ((xoffs & 0x1f8) >> 3);

                        /* fetch the current data from the road ROMs */
                        int control = road_control[romoffs];
                        int bits1 = road_bits1[romoffs];
                        int bits2 = road_bits2[(romoffs & 0xfff) | ((romoffs & 0x1000) >> 1)];

                        /* extract the road value and the carry-in bit */
                        int roadval = control & 0x3f;
                        int carin = control >> 7;

                        /* draw this 8-pixel chunk */
                        for (int i = 8; i > 0; i--)
                        {
                            int bits = BIT(bits1,i) + (BIT(bits2,i) << 1);
                            if (carin == 0 && bits != 0) bits++;
                            dest.op = (uint16_t)(pen_base | ((uint32_t)roadval & 0x3f));  dest++;  //*dest++ = pen_base | (roadval & 0x3f);
                            roadval += bits;
                        }
                    }
                }

                /* draw the scanline */
                draw_scanline16(bitmap, 0, y, 256, new PointerU16(scanline, xscroll), null);  //draw_scanline16(bitmap, 0, y, 256, &scanline[xscroll], nullptr);
            }
        }


        void zoom_sprite(bitmap_ind16 bitmap, int big,
                uint32_t code, uint32_t color, int flipx, int sx, int sy,
                int sizex, int sizey)
        {
            gfx_element gfx = m_gfxdecode.op0.gfx(big != 0 ? 3 : 2);  //gfx_element *gfx = m_gfxdecode->gfx(big ? 3 : 2);
            Pointer<uint8_t> gfxdata = gfx.get_data(code % gfx.elements());  //const uint8_t *gfxdata = gfx->get_data(code % gfx->elements());
            Pointer<uint8_t> scaling_rom = new Pointer<uint8_t>(memregion("gfx6").base_());  //uint8_t *scaling_rom = memregion("gfx6")->base();
            uint32_t transmask = m_palette.op0.transpen_mask(gfx, color, 0x1f);  //uint32_t transmask = m_palette->transpen_mask(*gfx, color, 0x1f);
            int coloroffs = (int)(gfx.colorbase() + color * gfx.granularity());  //int coloroffs = gfx->colorbase() + color * gfx->granularity();

            if (flipx != 0) flipx = big != 0 ? 0x1f : 0x0f;

            for (int y = 0; y <= sizey; y++)
            {
                int yy = (sy + y) & 0x1ff;

                /* the following should be a reasonable reproduction of how the real hardware works */
                if (yy >= 0x10 && yy < 0xf0)
                {
                    int dy = scaling_rom[(y << 6) + sizey] & 0x1f;
                    int xx = sx & 0x3ff;
                    int siz = 0;
                    int offs = 0;
                    Pointer<uint8_t> src; //const uint8_t *src;

                    if (big == 0) dy >>= 1;
                    src = gfxdata + dy * (int)gfx.rowbytes();

                    for (int x = (big != 0 ? 0x40 : 0x20); x > 0; x--)
                    {
                        if (xx < 0x100)
                        {
                            int pen = src[offs/2 ^ flipx];

                            if (((transmask >> pen) & 1) == 0)
                                bitmap.pix(yy, xx).op = (uint16_t)(pen + coloroffs);
                        }

                        offs++;

                        siz = siz + 1 + sizex;
                        if ((siz & 0x40) != 0)
                        {
                            siz &= 0x3f;
                            xx = (xx + 1) & 0x3ff;
                        }
                    }
                }
            }
        }


        void draw_sprites(bitmap_ind16 bitmap, rectangle cliprect)
        {
            PointerU16 posmem = m_sprite16_memory[0x380];  //uint16_t *posmem = &m_sprite16_memory[0x380];
            PointerU16 sizmem = m_sprite16_memory[0x780];  //uint16_t *sizmem = &m_sprite16_memory[0x780];

            for (int i = 0; i < 64; i++, posmem += 2, sizmem += 2)
            {
                int sx = (posmem[1] & 0x3ff) - 0x40 + 4;
                int sy = 512 - (posmem[0] & 0x1ff) + 1; // sprites are buffered and delayed by one scanline
                int sizex = (sizmem[1] & 0x3f00) >> 8;
                int sizey = (sizmem[0] & 0x3f00) >> 8;
                int code = sizmem[0] & 0x7f;
                int flipx = sizmem[0] & 0x80;
                int color = sizmem[1] & 0x3f;

                /* 128V input to the palette PROM */
                if (sy >= 128) color |= 0x40;

                zoom_sprite(bitmap, (sizmem[0] & 0x8000) != 0 ? 1 : 0,
                            (uint32_t)code,
                            (uint32_t)color,
                            flipx,
                            sx, sy,
                            sizex,sizey);
            }
        }


        uint32_t screen_update(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            rectangle clip = cliprect;
            clip.max_y = 127;
            m_bg_tilemap.draw(screen, bitmap, clip, 0, 0);
            draw_road(bitmap);
            draw_sprites(bitmap,cliprect);
            m_tx_tilemap.draw(screen, bitmap, cliprect, 0, 0);
            return 0;
        }
    }
}
