// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;


namespace mame
{
    partial class mappy_state : driver_device
    {
        void superpac_palette(palette_device palette)
        {
            throw new emu_unimplemented();
#if false
            const uint8_t *color_prom = memregion("proms")->base();
            static constexpr int resistances[3] = { 1000, 470, 220 };

            // compute the color output resistor weights
            double rweights[3], gweights[3], bweights[2];
            compute_resistor_weights(0, 255, -1.0,
                    3, &resistances[0], rweights, 0, 0,
                    3, &resistances[0], gweights, 0, 0,
                    2, &resistances[1], bweights, 0, 0);

            // create a lookup table for the palette
            for (int i = 0; i < 32; i++)
            {
                int bit0, bit1, bit2;

                // red component
                bit0 = BIT(color_prom[i], 0);
                bit1 = BIT(color_prom[i], 1);
                bit2 = BIT(color_prom[i], 2);
                int const r = combine_weights(rweights, bit0, bit1, bit2);

                // green component
                bit0 = BIT(color_prom[i], 3);
                bit1 = BIT(color_prom[i], 4);
                bit2 = BIT(color_prom[i], 5);
                int const g = combine_weights(gweights, bit0, bit1, bit2);

                // blue component
                bit0 = BIT(color_prom[i], 6);
                bit1 = BIT(color_prom[i], 7);
                int const b = combine_weights(bweights, bit0, bit1);

                palette.set_indirect_color(i, rgb_t(r, g, b));
            }

            // color_prom now points to the beginning of the lookup table
            color_prom += 32;

            // characters map to the upper 16 palette entries
            for (int i = 0; i < 64*4; i++)
            {
                uint8_t const ctabentry = color_prom[i] & 0x0f;
                palette.set_pen_indirect(i, (ctabentry ^ 15) + 0x10);
            }

            // sprites map to the lower 16 palette entries
            for (int i = 64*4; i < 128*4; i++)
            {
                uint8_t const ctabentry = color_prom[i] & 0x0f;
                palette.set_pen_indirect(i, ctabentry);
            }
#endif
        }


        //void mappy_state::mappy_palette(palette_device &palette) const

        //void mappy_state::phozon_palette(palette_device &palette) const


        /***************************************************************************
          Callbacks for the TileMap code
        ***************************************************************************/
#if false
        /* convert from 32x32 to 36x28 */
        TILEMAP_MAPPER_MEMBER(mappy_state::superpac_tilemap_scan)
        {
            int offs;

            row += 2;
            col -= 2;
            if (col & 0x20)
                offs = row + ((col & 0x1f) << 5);
            else
                offs = col + (row << 5);

            return offs;
        }
#endif


        //TILEMAP_MAPPER_MEMBER(mappy_state::mappy_tilemap_scan)


#if false
        TILE_GET_INFO_MEMBER(mappy_state::superpac_get_tile_info)
        {
            uint8_t attr = m_videoram[tile_index + 0x400];

            tileinfo.category = (attr & 0x40) >> 6;
            tileinfo.group = attr & 0x3f;
            tileinfo.set(0,
                    m_videoram[tile_index],
                    attr & 0x3f,
                    0);
        }
#endif


        //TILE_GET_INFO_MEMBER(mappy_state::phozon_get_tile_info)

        //TILE_GET_INFO_MEMBER(mappy_state::mappy_get_tile_info)


        /***************************************************************************
          Start the video hardware emulation.
        ***************************************************************************/
        //VIDEO_START_MEMBER(mappy_state,superpac)
        void video_start_superpac()
        {
            throw new emu_unimplemented();
#if false
            m_bg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(mappy_state::superpac_get_tile_info)), tilemap_mapper_delegate(*this, FUNC(mappy_state::superpac_tilemap_scan)), 8, 8, 36, 28);
            m_screen->register_screen_bitmap(m_sprite_bitmap);

            m_bg_tilemap->configure_groups(*m_gfxdecode->gfx(0), 31);
#endif
        }


        //VIDEO_START_MEMBER(mappy_state,phozon)

        //VIDEO_START_MEMBER(mappy_state,mappy)


        /***************************************************************************
          Memory handlers
        ***************************************************************************/
#if false
        void mappy_state::superpac_videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram[offset] = data;
            m_bg_tilemap->mark_tile_dirty(offset & 0x3ff);
        }

        void mappy_state::mappy_videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram[offset] = data;
            m_bg_tilemap->mark_tile_dirty(offset & 0x7ff);
        }

        void mappy_state::superpac_flipscreen_w(uint8_t data)
        {
            flip_screen_set(data & 1);
        }

        uint8_t mappy_state::superpac_flipscreen_r()
        {
            flip_screen_set(1);
            return 0xff;
        }

        void mappy_state::mappy_scroll_w(offs_t offset, uint8_t data)
        {
            m_scroll = offset >> 3;
        }
#endif


        /***************************************************************************
          Display refresh
        ***************************************************************************/
        //void mappy_state::mappy_draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect, uint8_t *spriteram_base)

        //void mappy_state::phozon_draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect, uint8_t *spriteram_base)


        uint32_t screen_update_superpac(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
#if false
            bitmap_ind16 &sprite_bitmap = m_sprite_bitmap;

            m_bg_tilemap->draw(screen, bitmap, cliprect, TILEMAP_DRAW_OPAQUE | TILEMAP_DRAW_ALL_CATEGORIES,0);

            sprite_bitmap.fill(15, cliprect);
            mappy_draw_sprites(sprite_bitmap,cliprect,m_spriteram);
            copybitmap_trans(bitmap,sprite_bitmap,0,0,0,0,cliprect,15);

            /* Redraw the high priority characters */
            m_bg_tilemap->draw(screen, bitmap, cliprect, 1,0);

            /* sprite color 0/1 still has priority over that (ghost eyes in Pac 'n Pal) */
            for (int y = 0;y < sprite_bitmap.height();y++)
            {
                for (int x = 0;x < sprite_bitmap.width();x++)
                {
                    int spr_entry = sprite_bitmap.pix(y, x);
                    int spr_pen = m_palette->pen_indirect(spr_entry);
                    if (spr_pen == 0 || spr_pen == 1)
                        bitmap.pix(y, x) = spr_entry;
                }
            }
            return 0;
#endif
        }


        //uint32_t mappy_state::screen_update_phozon(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)


        //uint32_t mappy_state::screen_update_mappy(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)
    }
}
