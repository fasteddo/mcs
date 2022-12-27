// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using tilemap_memory_index = System.UInt32;  //typedef u32 tilemap_memory_index;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.palette_global;
using static mame.tilemap_global;


namespace mame
{
    partial class mcr_state : driver_device
    {
        /*************************************
         *
         *  Tilemap callbacks
         *
         *************************************/

        /*
            The 90009 board uses 1 byte per tile:

            Byte 0:
                pppppppp = picture index
         */
        //TILE_GET_INFO_MEMBER(mcr_state::mcr_90009_get_tile_info)
        void mcr_90009_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            tileinfo.set(0, m_videoram[tile_index].op, 0, 0);

            /* sprite color base is constant 0x10 */
            tileinfo.category = 1;
        }


        /*
            The 90010 board uses 2 adjacent bytes per tile:

            Byte 0:
                pppppppp = picture index (low 8 bits)

            Byte 1:
                ss------ = sprite palette bank
                ---cc--- = tile palette bank
                -----y-- = Y flip
                ------x- = X flip
                -------p = picture index (high 1 bit)
         */
        //TILE_GET_INFO_MEMBER(mcr_state::mcr_90010_get_tile_info)
        void mcr_90010_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int data = m_videoram[tile_index * 2].op | (m_videoram[tile_index * 2 + 1].op << 8);
            int code = data & 0x1ff;
            int color = (data >> 11) & 3;
            tileinfo.set(0, (uint32_t)code, (uint32_t)color, TILE_FLIPYX(data >> 9));

            /* sprite color base comes from the top 2 bits */
            tileinfo.category = (uint8_t)((data >> 14) & 3);
        }


        /*
            The 91490 board uses 2 adjacent bytes per tile:

            Byte 0:
                pppppppp = picture index (low 8 bits)

            Byte 1:
                ss------ = sprite palette bank (can be disabled via jumpers)
                --cc---- = tile palette bank
                ----y--- = Y flip
                -----x-- = X flip
                ------pp = picture index (high 2 bits)
         */
        //TILE_GET_INFO_MEMBER(mcr_state::mcr_91490_get_tile_info)
        void mcr_91490_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int data = m_videoram[tile_index * 2].op | (m_videoram[tile_index * 2 + 1].op << 8);
            int code = data & 0x3ff;
            int color = (data >> 12) & 3;
            tileinfo.set(0, (uint32_t)code, (uint32_t)color, TILE_FLIPYX(data >> 10));

            /* sprite color base might come from the top 2 bits */
            tileinfo.category = (uint8_t)((data >> 14) & 3);
        }


        /*************************************
         *
         *  Common video startup/shutdown
         *
         *************************************/

        protected override void video_start()
        {
            /* the tilemap callback is based on the CPU board */
            switch (m_mcr_cpu_board)
            {
            case 90009:
                m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op0, mcr_90009_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 16,16, 32,30);
                break;

            case 90010:
                m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op0, mcr_90010_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 16,16, 32,30);
                break;

            case 91475:
                m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op0, mcr_90010_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 16,16, 32,30);
                break;

            case 91490:
                m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op0, mcr_91490_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 16,16, 32,30);
                break;

            default:
                throw new emu_fatalerror("mcr_state::video_start: Unknown mcr board");
                break;
            }
        }


        /*************************************
         *
         *  Palette RAM writes
         *
         *************************************/
        void mcr_set_color(int index, int data)
        {
            m_palette.op0.set_pen_color((pen_t)index, pal3bit((uint8_t)(data >> 6)), pal3bit((uint8_t)(data >> 0)), pal3bit((uint8_t)(data >> 3)));
        }


        void journey_set_color(int index, int data)
        {
            throw new emu_unimplemented();
        }


        void mcr_paletteram9_w(offs_t offset, uint8_t data)
        {
            // palette RAM is actually 9 bit (a 93419 SRAM)
            // however, there is no way for the CPU to read back
            // the high bit, because D8 of the SRAM is connected
            // to A0 of the bus rather than to a data line
            m_paletteram[offset].op = data;
            mcr_set_color((int)(offset / 2), (int)(data | ((offset & 1) << 8)));
        }


        /*************************************
         *
         *  Video RAM writes
         *
         *************************************/
#if false
        void mcr_state::mcr_90009_videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram[offset] = data;
            m_bg_tilemap->mark_tile_dirty(offset);
        }
#endif


        void mcr_90010_videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram[offset].op = data;
            m_bg_tilemap.mark_tile_dirty(offset / 2);

            /* palette RAM is mapped into the upper 0x80 bytes here */
            if ((offset & 0x780) == 0x780)
            {
                if (m_mcr_cpu_board != 91475)
                    mcr_set_color((int)((offset / 2) & 0x3f), (int)(data | ((offset & 1) << 8)));
                else
                    journey_set_color((int)((offset / 2) & 0x3f), (int)(data | ((offset & 1) << 8)));
            }
        }


        uint8_t twotiger_videoram_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
            /* Two Tigers swizzles the address bits on videoram */
            int effoffs = ((offset << 1) & 0x7fe) | ((offset >> 10) & 1);
            return m_videoram[effoffs];
#endif
        }


        void twotiger_videoram_w(offs_t offset, uint8_t data)
        {
            /* Two Tigers swizzles the address bits on videoram */
            int effoffs = (int)(((offset << 1) & 0x7fe) | ((offset >> 10) & 1));

            m_videoram[effoffs].op = data;
            m_bg_tilemap.mark_tile_dirty((tilemap_memory_index)(effoffs / 2));

            /* palette RAM is mapped into the upper 0x80 bytes here */
            if ((effoffs & 0x780) == 0x780)
                mcr_set_color((int)(((offset & 0x400) >> 5) | ((offset >> 1) & 0x1f)), (int)(data | ((offset & 1) << 8)));
        }


        void mcr_91490_videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram[offset].op = data;
            m_bg_tilemap.mark_tile_dirty(offset / 2);
        }


        /*************************************
         *
         *  91399 Video Gen sprite renderer
         *
         *  Paired with:
         *      90009 CPU -> fixed palette @ 1
         *      90010 CPU -> palette specified by tiles
         *
         *************************************/
        void render_sprites_91399(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            gfx_element gfx = m_gfxdecode.op0.gfx(1);

            /* render the sprites into the bitmap, ORing together */
            for (int offs = 0; offs < (int)m_spriteram.bytes(); offs += 4)
            {
                /* extract the bits of information */
                int code = m_spriteram[offs + 1].op & 0x3f;
                int hflip = (m_spriteram[offs + 1].op & 0x40) != 0 ? 31 : 0;
                int vflip = (m_spriteram[offs + 1].op & 0x80) != 0 ? 31 : 0;
                int sx = (m_spriteram[offs + 2].op - 4) * 2;
                int sy = (240 - m_spriteram[offs].op) * 2;

                /* apply cocktail mode */
                if (m_mcr_cocktail_flip != 0)
                {
                    hflip ^= 31;
                    vflip ^= 31;
                    sx = 466 - sx + m_mcr12_sprite_xoffs_flip;
                    sy = 450 - sy;
                }
                else
                {
                    sx += m_mcr12_sprite_xoffs;
                }

                /* clamp within 512 */
                sx &= 0x1ff;
                sy &= 0x1ff;

                /* loop over lines in the sprite */
                for (int y = 0; y < 32; y++, sy = (sy + 1) & 0x1ff)
                {
                    if (sy >= cliprect.min_y && sy <= cliprect.max_y)
                    {
                        Pointer<uint8_t> src = gfx.get_data((uint32_t)code) + (int)gfx.rowbytes() * (y ^ vflip);  //uint8_t const *const src = gfx->get_data(code) + gfx->rowbytes() * (y ^ vflip);
                        PointerU16 dst = bitmap.pix(sy);  //uint16_t *const dst = &bitmap.pix(sy);
                        Pointer<uint8_t> pri = screen.priority().pix(sy);  //uint8_t *const pri = &screen.priority().pix(sy);

                        /* loop over columns */
                        for (int x = 0; x < 32; x++)
                        {
                            int tx = (sx + x) & 0x1ff;
                            int pix = pri[tx] | src[x ^ hflip];

                            /* update the effective sprite pixel */
                            pri[tx] = (uint8_t)pix;

                            /* only draw if the low 3 bits are set */
                            if ((pix & 0x07) != 0)
                                dst[tx] = (uint16_t)pix;
                        }
                    }
                }
            }
        }


        /*************************************
         *
         *  91464 Super Video Gen sprite renderer
         *
         *  Paired with:
         *      91442 CPU -> fixed palette @ 1 (upper half) or 3 (lower half)
         *      91475 CPU -> palette specified by sprite board; sprites have extra implicit colors
         *      91490 CPU -> palette specified by sprite board or by tiles (select via jumpers)
         *      91721 CPU -> palette specified by sprite board
         *
         *************************************/
        void render_sprites_91464(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect, int primask, int sprmask, int colormask)
        {
            gfx_element gfx = m_gfxdecode.op0.gfx(1);

            /* render the sprites into the bitmap, working from topmost to bottommost */
            for (int offs = (int)m_spriteram.bytes() - 4; offs >= 0; offs -= 4)
            {
                /* extract the bits of information */
                int code = (m_spriteram[offs + 2].op + 256 * ((m_spriteram[offs + 1].op >> 3) & 0x01)) % (int)gfx.elements();
                int color = (((~m_spriteram[offs + 1].op & 3) << 4) & sprmask) | colormask;
                int hflip = (m_spriteram[offs + 1].op & 0x10) != 0 ? 31 : 0;
                int vflip = (m_spriteram[offs + 1].op & 0x20) != 0 ? 31 : 0;
                int sx = (m_spriteram[offs + 3].op - 3) * 2;
                int sy = (241 - m_spriteram[offs].op) * 2;

                /* apply cocktail mode */
                if (m_mcr_cocktail_flip != 0)
                {
                    hflip ^= 31;
                    vflip ^= 31;
                    sx = 480 - sx;
                    sy = 452 - sy;
                }

                /* clamp within 512 */
                sx &= 0x1ff;
                sy &= 0x1ff;

                /* loop over lines in the sprite */
                for (int y = 0; y < 32; y++, sy = (sy + 1) & 0x1ff)
                {
                    if (sy >= 2 && sy >= cliprect.min_y && sy <= cliprect.max_y)
                    {
                        Pointer<uint8_t> src = gfx.get_data((uint32_t)code) + (int)gfx.rowbytes() * (y ^ vflip);  //uint8_t const *const src = gfx->get_data(code) + gfx->rowbytes() * (y ^ vflip);
                        PointerU16 dst = bitmap.pix(sy);  //uint16_t *const dst = &bitmap.pix(sy);
                        Pointer<uint8_t> pri = screen.priority().pix(sy);  //uint8_t *const pri = &screen.priority().pix(sy);

                        /* loop over columns */
                        for (int x = 0; x < 32; x++)
                        {
                            int tx = (sx + x) & 0x1ff;
                            int pix = pri[tx];
                            if (pix != 0xff)
                            {
                                /* compute the final value */
                                pix = (pix & primask) | color | src[x ^ hflip];

                                /* if non-zero, draw */
                                if ((pix & 0x0f) != 0)
                                {
                                    /* mark this pixel so we don't draw there again */
                                    pri[tx] = 0xff;

                                    /* only draw if the low 3 bits are set */
                                    if ((pix & 0x07) != 0)
                                        dst[tx] = (uint16_t)pix;
                                }
                            }
                        }
                    }
                }
            }
        }


        /*************************************
         *
         *  Main refresh routines
         *
         *************************************/
        uint32_t screen_update_mcr(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            /* update the flip state */
            m_bg_tilemap.set_flip(m_mcr_cocktail_flip != 0 ? (TILEMAP_FLIPX | TILEMAP_FLIPY) : 0);

            /* draw the background */
            screen.priority().fill(0, cliprect);
            m_bg_tilemap.draw(screen, bitmap, cliprect, 0, 0x00);
            m_bg_tilemap.draw(screen, bitmap, cliprect, 1, 0x10);
            m_bg_tilemap.draw(screen, bitmap, cliprect, 2, 0x20);
            m_bg_tilemap.draw(screen, bitmap, cliprect, 3, 0x30);

            /* update the sprites and render them */
            switch (m_mcr_sprite_board)
            {
            case 91399:
                render_sprites_91399(screen, bitmap, cliprect);
                break;

            case 91464:
                if (m_mcr_cpu_board == 91442)
                    render_sprites_91464(screen, bitmap, cliprect, 0x00, 0x30, 0x00);
                else if (m_mcr_cpu_board == 91475)
                    render_sprites_91464(screen, bitmap, cliprect, 0x00, 0x30, 0x40);
                else if (m_mcr_cpu_board == 91490)
                    render_sprites_91464(screen, bitmap, cliprect, 0x00, 0x30, 0x00);
                else if (m_mcr_cpu_board == 91721)
                    render_sprites_91464(screen, bitmap, cliprect, 0x00, 0x30, 0x00);
                break;
            }

            return 0;
        }
    }
}
