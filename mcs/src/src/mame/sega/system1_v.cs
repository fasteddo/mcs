// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using tilemap_memory_index = System.UInt32;  //typedef u32 tilemap_memory_index;
using u8 = System.Byte;
using u32 = System.UInt32;

using static mame.emucore_global;
using static mame.resnet_global;
using static mame.util;


namespace mame
{
    partial class system1_state : driver_device
    {
        void system1_palette(palette_device palette)
        {
            /*
              There are two kind of color handling: in the System 1 games, values in the
              palette RAM are directly mapped to colors with the usual BBGGGRRR format;
              in the System 2 ones (Choplifter, WBML, etc.), the value in the palette RAM
              is a lookup offset for three palette PROMs in RRRRGGGGBBBB format.

              It's hard to tell for sure because they use resistor packs, but here's
              what I think the values are from measurment with a volt meter:

              Blue: .250K ohms
              Blue: .495K ohms
              Green:.250K ohms
              Green:.495K ohms
              Green:.995K ohms
              Red:  .495K ohms
              Red:  .250K ohms
              Red:  .995K ohms

              accurate to +/- .003K ohms.
            */

            if (m_color_prom != null)
            {
                for (int pal = 0; pal < 256; pal++)
                {
                    u8 val;
                    val = m_color_prom[pal + 0 * 256].op;
                    u8 r = (u8)(0x0e * BIT(val, 0) + 0x1f * BIT(val, 1) + 0x43 * BIT(val, 2) + 0x8f * BIT(val, 3));

                    val = m_color_prom[pal + 1 * 256].op;
                    u8 g = (u8)(0x0e * BIT(val, 0) + 0x1f * BIT(val, 1) + 0x43 * BIT(val, 2) + 0x8f * BIT(val, 3));

                    val = m_color_prom[pal + 2 * 256].op;
                    u8 b = (u8)(0x0e * BIT(val, 0) + 0x1f * BIT(val, 1) + 0x43 * BIT(val, 2) + 0x8f * BIT(val, 3));

                    palette.set_indirect_color(pal, new rgb_t(r, g, b));
                }
            }
            else
            {
                int [] resistances_rg = new int [3] { 995, 495, 250 };
                int [] resistances_b = new int [2] { 495, 250 };

                double [] weights_r = new double [3];
                double [] weights_g = new double [3];
                double [] weights_b = new double [2];
                compute_resistor_weights(0, 255,    -1.0,
                        3,  resistances_rg, out weights_r,  0,    0,
                        3,  resistances_rg, out weights_g,  0,    0,
                        2,  resistances_b,  out weights_b,  0,    0);

                for (int i = 0; i < 256; i++)
                {
                    int bit0, bit1, bit2;

                    // red component
                    bit0 = BIT(i, 0);
                    bit1 = BIT(i, 1);
                    bit2 = BIT(i, 2);
                    int r = combine_weights(weights_r, bit0, bit1, bit2);

                    // green component
                    bit0 = BIT(i, 3);
                    bit1 = BIT(i, 4);
                    bit2 = BIT(i, 5);
                    int g = combine_weights(weights_g, bit0, bit1, bit2);

                    // blue component
                    bit0 = BIT(i, 6);
                    bit1 = BIT(i, 7);
                    int b = combine_weights(weights_b, bit0, bit1);

                    palette.set_indirect_color(i, new rgb_t((u8)r, (u8)g, (u8)b));
                }
            }
        }


        /*************************************
         *  Tile callback
         *************************************/
        //TILE_GET_INFO_MEMBER(system1_state::tile_get_info)
        void tile_get_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            PointerU8 rambase = (PointerU8)tilemap.user_data();  //const u8 *rambase = (const u8 *)tilemap.user_data();
            u32 tiledata = (u32)rambase[tile_index * 2 + 0] | ((u32)rambase[tile_index * 2 + 1] << 8);
            u32 code = ((tiledata >> 4) & 0x800) | (tiledata & 0x7ff);
            u32 color = (tiledata >> 5) & 0xff;

            tileinfo.set(0, code, color, 0);
        }


        /*************************************
         *  Video startup
         *************************************/
        void video_start_common(int pagecount)
        {
            int pagenum;

            /* allocate memory for the collision arrays */
            m_mix_collide = new u8 [64];  //m_mix_collide = make_unique_clear<u8[]>(64);
            m_sprite_collide = new u8 [1024];  //m_sprite_collide = make_unique_clear<u8[]>(1024);

            /* allocate memory for videoram */
            m_tilemap_pages = (u8)pagecount;
            m_videoram = new MemoryU8(0x800 * pagecount, true);  //m_videoram = make_unique_clear<u8[]>(0x800 * pagecount);

            /* create the tilemap pages */
            for (pagenum = 0; pagenum < pagecount; pagenum++)
            {
                m_tilemap_page[pagenum] = machine().tilemap().create(m_gfxdecode.op0, tile_get_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8,8, 32,32);
                m_tilemap_page[pagenum].set_transparent_pen(0);
                m_tilemap_page[pagenum].set_user_data(new PointerU8(m_videoram) + 0x800 * pagenum);
            }

            /* allocate a temporary bitmap for sprite rendering */
            m_screen.op0.register_screen_bitmap(m_sprite_bitmap);

            /* register for save stats */
            save_item(NAME(new { m_video_mode }));
            save_item(NAME(new { m_mix_collide_summary }));
            save_item(NAME(new { m_sprite_collide_summary }));
            save_item(NAME(new { m_videoram_bank }));

            //throw new emu_unimplemented();
#if false
            save_pointer(NAME(m_videoram), 0x800 * pagecount);
            save_pointer(NAME(m_mix_collide), 64);
            save_pointer(NAME(m_sprite_collide), 1024);
#endif
        }


        protected override void video_start()
        {
            throw new emu_unimplemented();
#if false
            video_start_common(2);
#endif
        }


        //VIDEO_START_MEMBER(system1_state,system2)
        void video_start_system2()
        {
            video_start_common(8);
        }


        /*************************************
         *  Video control
         *************************************/
        void common_videomode_w(u8 data)
        {
            if ((data & 0x6e) != 0) logerror("videomode = {0}\n",data);

            /* bit 4 is screen blank */
            m_video_mode = data;

            /* bit 7 is flip screen */
            flip_screen_set(data & 0x80);
        }


        /*************************************
         *  Mixer collision I/O
         *************************************/
        u8 mixer_collision_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
//            m_screen->update_now();
            m_screen->update_partial(m_screen->vpos());
            return m_mix_collide[offset & 0x3f] | 0x7e | (m_mix_collide_summary << 7);
#endif
        }


        void mixer_collision_w(offs_t offset, u8 data)
        {
            throw new emu_unimplemented();
#if false
//            m_screen->update_now();
            m_screen->update_partial(m_screen->vpos());
            m_mix_collide[offset & 0x3f] = 0;
#endif
        }


        void mixer_collision_reset_w(u8 data)
        {
            throw new emu_unimplemented();
#if false
//            m_screen->update_now();
            m_screen->update_partial(m_screen->vpos());
            m_mix_collide_summary = 0;
#endif
        }


        /*************************************
         *  Sprite collision I/O
         *************************************/
        u8 sprite_collision_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
//            m_screen->update_now();
            m_screen->update_partial(m_screen->vpos());
            return m_sprite_collide[offset & 0x3ff] | 0x7e | (m_sprite_collide_summary << 7);
#endif
        }


        void sprite_collision_w(offs_t offset, u8 data)
        {
            throw new emu_unimplemented();
#if false
//            m_screen->update_now();
            m_screen->update_partial(m_screen->vpos());
            m_sprite_collide[offset & 0x3ff] = 0;
#endif
        }


        void sprite_collision_reset_w(u8 data)
        {
            throw new emu_unimplemented();
#if false
//            m_screen->update_now();
            m_screen->update_partial(m_screen->vpos());
            m_sprite_collide_summary = 0;
#endif
        }


        /*************************************
         *  Video RAM access
         *************************************/
#if false
        inline void system1_state::videoram_wait_states(cpu_device *cpu)
        {
            /* The main Z80's CPU clock is halted whenever an access to VRAM happens,
               and is only restarted by the FIXST signal, which occurs once every
               'n' pixel clocks. 'n' is determined by the horizontal control PAL. */

            /* this assumes 4 5MHz pixel clocks per FIXST, or 8*4 20MHz CPU clocks,
               and is based on a dump of 315-5137 */
            const u32 cpu_cycles_per_fixst = 4 * 4;
            const u32 fixst_offset = 2 * 4;
            u32 cycles_until_next_fixst = cpu_cycles_per_fixst - ((cpu->total_cycles() - fixst_offset) % cpu_cycles_per_fixst);

            cpu->adjust_icount(-cycles_until_next_fixst);
        }
#endif


        u8 videoram_r(offs_t offset)
        {
            throw new emu_unimplemented();
#if false
            videoram_wait_states(m_maincpu);
            offset |= 0x1000 * ((m_videoram_bank >> 1) % (m_tilemap_pages / 2));
            return m_videoram[offset];
#endif
        }


        void videoram_w(offs_t offset, u8 data)
        {
            throw new emu_unimplemented();
#if false
            videoram_wait_states(m_maincpu);
            offset |= 0x1000 * ((m_videoram_bank >> 1) % (m_tilemap_pages / 2));
            m_videoram[offset] = data;

            m_tilemap_page[offset / 0x800]->mark_tile_dirty((offset % 0x800) / 2);

            /* force a partial update if the page is changing */
            if (m_tilemap_pages > 2 && offset >= 0x740 && offset < 0x748 && offset % 2 == 0)
            {
                //m_screen->update_now();
                m_screen->update_partial(m_screen->vpos());
            }
#endif
        }


        void videoram_bank_w(u8 data)
        {
            m_videoram_bank = data;
        }


        /*************************************
         *  Palette RAM access
         *************************************/
        void paletteram_w(offs_t offset, u8 data)
        {
            throw new emu_unimplemented();
#if false
            m_paletteram[offset] = data;
            m_palette.op0.set_pen_indirect(offset, m_paletteram[offset]);
#endif
        }


        /*************************************
         *  Sprite rendering
         *************************************/
#if false
        void system1_state::draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect, int xoffset)
        {
            const u32 gfxbanks = m_spriterom.bytes() / 0x8000;
            const int flipscreen = flip_screen();

            /* up to 32 sprites total */
            for (int spritenum = 0; spritenum < 32; spritenum++)
            {
                const u8 *spritedata = &m_spriteram[spritenum * 0x10];
                u16 srcaddr = spritedata[6] + (spritedata[7] << 8);
                const u16 stride = spritedata[4] + (spritedata[5] << 8);
                u8 bank = ((spritedata[3] & 0x80) >> 7) | ((spritedata[3] & 0x40) >> 5) | ((spritedata[3] & 0x20) >> 3);
                const int xstart = ((spritedata[2] | (spritedata[3] << 8)) & 0x1ff) + xoffset;
                int bottom = spritedata[1] + 1;
                int top = spritedata[0] + 1;
                const u16 palettebase = spritenum * 0x10;

                /* writing an 0xff into the first byte of sprite RAM seems to disable all sprites;
                   not sure if this applies to each sprite or only to the first one; see pitfall2
                   and wmatch for examples where this is done */
                if (spritedata[0] == 0xff)
                    return;

                /* clamp the bank to the size of the sprite ROMs */
                bank %= gfxbanks;
                const u8 *gfxbankbase = &m_spriterom[bank * 0x8000];

                /* flip sprites vertically */
                if (flipscreen)
                {
                    int temp = top;
                    top = 256 - bottom;
                    bottom = 256 - temp;
                }

                /* iterate over all rows of the sprite */
                for (int y = top; y < bottom; y++)
                {
                    u16 *const destbase = &bitmap.pix(y);

                    /* advance by the row counter */
                    srcaddr += stride;

                    /* skip if outside of our clipping area */
                    if (y < cliprect.min_y || y > cliprect.max_y)
                        continue;

                    /* iterate over X */
                    int addrdelta = (srcaddr & 0x8000) ? -1 : 1;
                    for (int x = xstart, curaddr = srcaddr; ; x += 4, curaddr += addrdelta)
                    {
                        u8 color1, color2;

                        const u8 data = gfxbankbase[curaddr & 0x7fff];

                        /* non-flipped case */
                        if (!(curaddr & 0x8000))
                        {
                            color1 = data >> 4;
                            color2 = data & 0x0f;
                        }
                        else
                        {
                            color1 = data & 0x0f;
                            color2 = data >> 4;
                        }

                        /* stop when we see color 0x0f */
                        if (color1 == 0x0f)
                            break;

                        /* draw if non-transparent */
                        if (color1 != 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                const int effx = flipscreen ? 0x1fe - (x + i) : (x + i);
                                if (effx >= cliprect.min_x && effx <= cliprect.max_x)
                                {
                                    const int prevpix = destbase[effx];

                                    if ((prevpix & 0x0f) != 0)
                                        m_sprite_collide[((prevpix >> 4) & 0x1f) + 32 * spritenum] = m_sprite_collide_summary = 1;
                                    destbase[effx] = color1 | palettebase;
                                }
                            }
                        }

                        /* stop when we see color 0x0f */
                        if (color2 == 0x0f)
                            break;

                        /* draw if non-transparent */
                        if (color2 != 0)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                const int effx = flipscreen ? 0x1fe - (x + 2 + i) : (x + 2 + i);
                                if (effx >= cliprect.min_x && effx <= cliprect.max_x)
                                {
                                    const int prevpix = destbase[effx];

                                    if ((prevpix & 0x0f) != 0)
                                        m_sprite_collide[((prevpix >> 4) & 0x1f) + 32 * spritenum] = m_sprite_collide_summary = 1;
                                    destbase[effx] = color2 | palettebase;
                                }
                            }
                        }
                    }
                }
            }
        }
#endif


        /*************************************
         *  Generic update code
         *************************************/
#if false
        void system1_state::video_update_common(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect, bitmap_ind16 &fgpixmap, bitmap_ind16 **bgpixmaps, const int *bgrowscroll, int bgyscroll, int spritexoffs)
        {
            if (m_video_mode & 0x10)
            {
                bitmap.fill(0, cliprect);
                return;
            }
            /* first clear the sprite bitmap and draw sprites within this area */
            m_sprite_bitmap.fill(0, cliprect);
            draw_sprites(m_sprite_bitmap, cliprect, spritexoffs);

            /* iterate over rows */
            for (int y = cliprect.min_y; y <= cliprect.max_y; y++)
            {
                const u16 *const fgbase = &fgpixmap.pix(y & 0xff);
                const u16 *const sprbase = &m_sprite_bitmap.pix(y & 0xff);
                u16 *const dstbase = &bitmap.pix(y);
                const int bgy = (y + bgyscroll) & 0x1ff;
                const int bgxscroll = bgrowscroll[y >> 3 & 0x1f];

                /* get the base of the left and right pixmaps for the effective background Y */
                const u16 *const bgbase[2] = { &bgpixmaps[(bgy >> 8) * 2 + 0]->pix(bgy & 0xff), &bgpixmaps[(bgy >> 8) * 2 + 1]->pix(bgy & 0xff) };

                /* iterate over pixels */
                for (int x = cliprect.min_x; x <= cliprect.max_x; x++)
                {
                    const int bgx = ((x - bgxscroll) / 2) & 0x1ff;
                    const u16 fgpix = fgbase[(x / 2) & 0xff];
                    const u16 bgpix = bgbase[bgx >> 8][bgx & 0xff];
                    const u16 sprpix = sprbase[x];

                    /* using the sprite, background, and foreground pixels, look up the color behavior */
                    const u8 lookup_index =  (((sprpix & 0xf) == 0) << 0) |
                                    (((fgpix & 7) == 0) << 1) |
                                    (((fgpix >> 9) & 3) << 2) |
                                    (((bgpix & 7) == 0) << 4) |
                                    (((bgpix >> 9) & 3) << 5);
                    u8 lookup_value = m_lookup_prom[lookup_index];

                    /* compute collisions based on two of the PROM bits */
                    if (!(lookup_value & 4))
                        m_mix_collide[((lookup_value & 8) << 2) | ((sprpix >> 4) & 0x1f)] = m_mix_collide_summary = 1;

                    /* the lower 2 PROM bits select the palette and which pixels */
                    lookup_value &= 3;
                    if (lookup_value == 0)
                        dstbase[x] = 0x000 | (sprpix & 0x1ff);
                    else if (lookup_value == 1)
                        dstbase[x] = 0x200 | (fgpix & 0x1ff);
                    else
                        dstbase[x] = 0x400 | (bgpix & 0x1ff);
                }
            }
        }
#endif


        /*************************************
         *  Board-specific update front-ends
         *************************************/
        u32 screen_update_system1(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
#if false
            bitmap_ind16 *bgpixmaps[4];
            int bgrowscroll[32];

            /* all 4 background pages are the same, fixed to page 0 */
            bgpixmaps[0] = bgpixmaps[1] = bgpixmaps[2] = bgpixmaps[3] = &m_tilemap_page[0]->pixmap();

            /* foreground is fixed to page 1 */
            bitmap_ind16 &fgpixmap = m_tilemap_page[1]->pixmap();

            /* get fixed scroll offsets */
            int xscroll = (s16)((m_videoram[0xffc] | (m_videoram[0xffd] << 8)) + 28);
            int yscroll = m_videoram[0xfbd];

            /* adjust for flipping */
            if (flip_screen())
            {
                xscroll = 640 - (xscroll & 0x1ff);
                yscroll = 764 - (yscroll & 0x1ff);
            }

            /* fill in the row scroll table */
            for (int y = 0; y < 32; y++)
                bgrowscroll[y] = xscroll;

            /* common update */
            video_update_common(screen, bitmap, cliprect, fgpixmap, bgpixmaps, bgrowscroll, yscroll, 0);
            return 0;
#endif
        }


        u32 screen_update_system2(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
#if false
            bitmap_ind16 *bgpixmaps[4];
            int rowscroll[32];
            int xscroll, yscroll;
            int sprxoffset;

            /* 4 independent background pages */
            bgpixmaps[0] = &m_tilemap_page[m_videoram[0x740] & 7]->pixmap();
            bgpixmaps[1] = &m_tilemap_page[m_videoram[0x742] & 7]->pixmap();
            bgpixmaps[2] = &m_tilemap_page[m_videoram[0x744] & 7]->pixmap();
            bgpixmaps[3] = &m_tilemap_page[m_videoram[0x746] & 7]->pixmap();

            /* foreground is fixed to page 0 */
            bitmap_ind16 &fgpixmap = m_tilemap_page[0]->pixmap();

            /* get scroll offsets */
            if (!flip_screen())
            {
                xscroll = ((m_videoram[0x7c0] | (m_videoram[0x7c1] << 8)) & 0x1ff) - 512 + 10;
                yscroll = m_videoram[0x7ba];
                sprxoffset = 14;
            }
            else
            {
                xscroll = 512 + 512 + 10 - (((m_videoram[0x7f6] | (m_videoram[0x7f7] << 8)) & 0x1ff) - 512 + 10);
                yscroll = 512 + 512 - m_videoram[0x784];
                sprxoffset = -14;
            }

            /* fill in the row scroll table */
            for (int y = 0; y < 32; y++)
                rowscroll[y] = xscroll;

            /* common update */
            video_update_common(screen, bitmap, cliprect, fgpixmap, bgpixmaps, rowscroll, yscroll, sprxoffset);
            return 0;
#endif
        }


        u32 screen_update_system2_rowscroll(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
#if false
            bitmap_ind16 *bgpixmaps[4];
            int rowscroll[32];
            int yscroll;
            int sprxoffset;

            /* 4 independent background pages */
            bgpixmaps[0] = &m_tilemap_page[m_videoram[0x740] & 7]->pixmap();
            bgpixmaps[1] = &m_tilemap_page[m_videoram[0x742] & 7]->pixmap();
            bgpixmaps[2] = &m_tilemap_page[m_videoram[0x744] & 7]->pixmap();
            bgpixmaps[3] = &m_tilemap_page[m_videoram[0x746] & 7]->pixmap();

            /* foreground is fixed to page 0 */
            bitmap_ind16 &fgpixmap = m_tilemap_page[0]->pixmap();

            /* get scroll offsets */
            if (!flip_screen())
            {
                for (int y = 0; y < 32; y++)
                    rowscroll[y] = ((m_videoram[0x7c0 + y * 2] | (m_videoram[0x7c1 + y * 2] << 8)) & 0x1ff) - 512 + 10;

                yscroll = m_videoram[0x7ba];
                sprxoffset = 14;
            }
            else
            {
                for (int y = 0; y < 32; y++)
                    rowscroll[y] = 512 + 512 + 10 - (((m_videoram[0x7fe - y * 2] | (m_videoram[0x7ff - y * 2] << 8)) & 0x1ff) - 512 + 10);

                yscroll = 512 + 512 - m_videoram[0x784];
                sprxoffset = -14;
            }

            /* common update */
            video_update_common(screen, bitmap, cliprect, fgpixmap, bgpixmaps, rowscroll, yscroll, sprxoffset);
            return 0;
#endif
        }
    }
}
