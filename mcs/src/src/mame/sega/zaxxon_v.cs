// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;


namespace mame
{
    partial class zaxxon_state : driver_device
    {
        /*************************************
         *  Palette conversion
         *************************************/
        void zaxxon_palette(palette_device palette)
        {
            throw new emu_unimplemented();
#if false
            uint8_t const *const color_prom = memregion("proms")->base();
            static constexpr int resistances[3] = { 1000, 470, 220 };

            // compute the color output resistor weights
            double rweights[3], gweights[3], bweights[2];
            compute_resistor_weights(0, 255, -1.0,
                    3,  &resistances[0], rweights, 470, 0,
                    3,  &resistances[0], gweights, 470, 0,
                    2,  &resistances[1], bweights, 470, 0);

            // initialize the palette with these colors
            for (int i = 0; i < palette.entries(); i++)
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

                palette.set_pen_color(i, rgb_t(r, g, b));
            }

            // color_prom now points to the beginning of the character color codes
            m_color_codes = &color_prom[256];
#endif
        }


#if false
        /*************************************
         *  Tilemap callbacks
         *************************************/
        TILE_GET_INFO_MEMBER(zaxxon_state::get_bg_tile_info)
        {
            const uint8_t *source = memregion("tilemap_dat")->base();
            int size = memregion("tilemap_dat")->bytes() / 2;
            int eff_index = tile_index & (size - 1);
            int code = source[eff_index] + 256 * (source[eff_index + size] & 3);
            int color = source[eff_index + size] >> 4;

            tileinfo.set(1, code, color, 0);
        }


        TILE_GET_INFO_MEMBER(zaxxon_state::zaxxon_get_fg_tile_info)
        {
            int sx = tile_index % 32;
            int sy = tile_index / 32;
            int code = m_videoram[tile_index];
            int color = m_color_codes[sx + 32 * (sy / 4)] & 0x0f;

            tileinfo.set(0, code, color * 2, 0);
        }


        TILE_GET_INFO_MEMBER(zaxxon_state::razmataz_get_fg_tile_info)
        {
            int code = m_videoram[tile_index];
            int color = m_color_codes[code] & 0x0f;

            tileinfo.set(0, code, color * 2, 0);
        }


        TILE_GET_INFO_MEMBER(zaxxon_state::congo_get_fg_tile_info)
        {
            int code = m_videoram[tile_index] + (m_congo_fg_bank << 8);
            int color = m_colorram[tile_index] & 0x1f;

            tileinfo.set(0, code, color * 2, 0);
        }


        /*************************************
         *  Video startup
         *************************************/
        void zaxxon_state::video_start_common(tilemap_get_info_delegate &&fg_tile_info)
        {
            /* reset globals */
            m_bg_enable = 0;
            m_bg_color = 0;
            m_bg_position = 0;
            m_fg_color = 0;
            m_flip_screen = false;
            m_congo_fg_bank = 0;
            m_congo_color_bank = 0;
            memset(m_congo_custom, 0, sizeof(m_congo_custom));

            // create a background and foreground tilemap
            m_bg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(zaxxon_state::get_bg_tile_info)), TILEMAP_SCAN_ROWS, 8,8, 32,512);
            m_fg_tilemap = &machine().tilemap().create(*m_gfxdecode, std::move(fg_tile_info), TILEMAP_SCAN_ROWS, 8,8, 32,32);

            // configure the foreground tilemap
            m_fg_tilemap->set_transparent_pen(0);

            // register for save states
            save_item(NAME(m_bg_enable));
            save_item(NAME(m_bg_color));
            save_item(NAME(m_bg_position));
            save_item(NAME(m_fg_color));
            save_item(NAME(m_flip_screen));
        }


        void zaxxon_state::video_start()
        {
            video_start_common(tilemap_get_info_delegate(*this, FUNC(zaxxon_state::zaxxon_get_fg_tile_info)));
        }


        //VIDEO_START_MEMBER(zaxxon_state,razmataz)

        //VIDEO_START_MEMBER(zaxxon_state,congo)
#endif


        /*************************************
         *  Video latches and controls
         *************************************/
        //WRITE_LINE_MEMBER(zaxxon_state::flipscreen_w)
        void flipscreen_w(int state)
        {
            throw new emu_unimplemented();
#if false
            /* low bit controls flip; background and sprite flip are handled at render time */
            m_flip_screen = !state;
            m_fg_tilemap->set_flip(m_flip_screen ? (TILEMAP_FLIPX | TILEMAP_FLIPY) : 0);
#endif
        }


        //WRITE_LINE_MEMBER(zaxxon_state::fg_color_w)
        void fg_color_w(int state)
        {
            throw new emu_unimplemented();
#if false
            /* low bit selects high color palette index */
            m_fg_color = state * 0x80;
            m_fg_tilemap->set_palette_offset(m_fg_color + (m_congo_color_bank << 8));
#endif
        }


#if false
        void zaxxon_state::bg_position_w(offs_t offset, uint8_t data)
        {
            /* 11 bits of scroll position are stored */
            if (offset == 0)
                m_bg_position = (m_bg_position & 0x700) | ((data << 0) & 0x0ff);
            else
                m_bg_position = (m_bg_position & 0x0ff) | ((data << 8) & 0x700);
        }
#endif


        //WRITE_LINE_MEMBER(zaxxon_state::bg_color_w)
        void bg_color_w(int state)
        {
            throw new emu_unimplemented();
#if false
            /* low bit selects high color palette index */
            m_bg_color = state * 0x80;
#endif
        }


        //WRITE_LINE_MEMBER(zaxxon_state::bg_enable_w)
        void bg_enable_w(int state)
        {
            throw new emu_unimplemented();
#if false
            /* low bit enables/disables the background layer */
            m_bg_enable = state;
#endif
        }


        //WRITE_LINE_MEMBER(zaxxon_state::congo_fg_bank_w)

        //WRITE_LINE_MEMBER(zaxxon_state::congo_color_bank_w)


        /*************************************
         *  Foreground tilemap access
         *************************************/
#if false
        void zaxxon_state::zaxxon_videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram[offset] = data;
            m_fg_tilemap->mark_tile_dirty(offset);
        }
#endif


        //void zaxxon_state::congo_colorram_w(offs_t offset, uint8_t data)

        //void zaxxon_state::congo_sprite_custom_w(address_space &space, offs_t offset, uint8_t data)


        /*************************************
         *  Background pixmap drawing
         *************************************/
#if false
        void zaxxon_state::draw_background(bitmap_ind16 &bitmap, const rectangle &cliprect, int skew)
        {
            /* only draw if enabled */
            if (m_bg_enable)
            {
                bitmap_ind16 &pixmap = m_bg_tilemap->pixmap();
                int colorbase = m_bg_color + (m_congo_color_bank << 8);
                int xmask = pixmap.width() - 1;
                int ymask = pixmap.height() - 1;
                int flipmask = m_flip_screen ? 0xff : 0x00;
                int flipoffs = m_flip_screen ? 0x38 : 0x40;

                /* the starting X value is offset by 1 pixel (normal) or 7 pixels */
                /* (flipped) due to a delay in the loading */
                if (!m_flip_screen)
                    flipoffs -= 1;
                else
                    flipoffs += 7;

                /* loop over visible rows */
                for (int y = cliprect.min_y; y <= cliprect.max_y; y++)
                {
                    uint16_t *const dst = &bitmap.pix(y);

                    /* VF = flipped V signals */
                    int vf = y ^ flipmask;

                    /* base of the source row comes from VF plus the scroll value */
                    /* this is done by the 3 4-bit adders at U56, U74, U75 */
                    int srcy = vf + ((m_bg_position << 1) ^ 0xfff) + 1;
                    uint16_t const *src = &pixmap.pix(srcy & ymask);

                    /* loop over visible columns */
                    for (int x = cliprect.min_x; x <= cliprect.max_x; x++)
                    {
                        /* start with HF = flipped H signals */
                        int srcx = x ^ flipmask;
                        if (skew)
                        {
                            /* position within source row is a two-stage addition */
                            /* first stage is HF plus half the VF, done by the 2 4-bit */
                            /* adders at U53, U54 */
                            srcx += ((vf >> 1) ^ 0xff) + 1;

                            /* second stage is first stage plus a constant based on the flip */
                            /* value is 0x40 for non-flipped, or 0x38 for flipped */
                            srcx += flipoffs;
                        }

                        /* store the pixel, offset by the color offset */
                        dst[x] = src[srcx & xmask] + colorbase;
                    }
                }
            }

            /* if not enabled, fill the background with black */
            else
                bitmap.fill(m_palette->black_pen(), cliprect);
        }


        /*************************************
         *  Sprite drawing
         *************************************/
        inline int zaxxon_state::find_minimum_y(uint8_t value, int flip)
        {
            int flipmask = flip ? 0xff : 0x00;
            int flipconst = flip ? 0xef : 0xf1;
            int y;

            /* the sum of the Y position plus a constant based on the flip state */
            /* is added to the current flipped VF; if the top 3 bits are 1, we hit */

            /* first find a 16-pixel bucket where we hit */
            for (y = 0; y < 256; y += 16)
            {
                int sum = (value + flipconst + 1) + (y ^ flipmask);
                if ((sum & 0xe0) == 0xe0)
                    break;
            }

            /* then scan backwards until we no longer match */
            while (1)
            {
                int sum = (value + flipconst + 1) + ((y - 1) ^ flipmask);
                if ((sum & 0xe0) != 0xe0)
                    break;
                y--;
            }

            /* add one line since we draw sprites on the previous line */
            return (y + 1) & 0xff;
        }


        inline int zaxxon_state::find_minimum_x(uint8_t value, int flip)
        {
            int flipmask = flip ? 0xff : 0x00;
            int x;

            /* the sum of the X position plus a constant specifies the address within */
            /* the line bufer; if we're flipped, we will write backwards */
            x = (value + 0xef + 1) ^ flipmask;
            if (flipmask)
                x -= 31;
            return x & 0xff;
        }


        void zaxxon_state::draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect, uint16_t flipxmask, uint16_t flipymask)
        {
            uint8_t *spriteram = m_spriteram;
            gfx_element *gfx = m_gfxdecode->gfx(2);
            int flip = m_flip_screen;
            int flipmask = flip ? 0xff : 0x00;
            int offs;

            /* only the lower half of sprite RAM is read during rendering */
            for (offs = 0x7c; offs >= 0; offs -= 4)
            {
                int sy = find_minimum_y(spriteram[offs], flip);
                int flipy = (spriteram[offs + (flipymask >> 8)] ^ flipmask) & flipymask;
                int flipx = (spriteram[offs + (flipxmask >> 8)] ^ flipmask) & flipxmask;
                int code = spriteram[offs + 1];
                int color = (spriteram[offs + 2] & 0x1f) + (m_congo_color_bank << 5);
                int sx = find_minimum_x(spriteram[offs + 3], flip);

                /* draw with 256 pixel offsets to ensure we wrap properly */
                    gfx->transpen(bitmap,cliprect, code, color, flipx, flipy, sx, sy, 0);
                    gfx->transpen(bitmap,cliprect, code, color, flipx, flipy, sx, sy - 0x100, 0);
                    gfx->transpen(bitmap,cliprect, code, color, flipx, flipy, sx - 0x100, sy, 0);
                    gfx->transpen(bitmap,cliprect, code, color, flipx, flipy, sx - 0x100, sy - 0x100, 0);
            }
        }
#endif


        /*************************************
         *  Core video updates
         *************************************/
        uint32_t screen_update_zaxxon(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
#if false
            draw_background(bitmap, cliprect, true);
            draw_sprites(bitmap, cliprect, 0x140, 0x180);
            m_fg_tilemap->draw(screen, bitmap, cliprect, 0, 0);
            return 0;
#endif
        }


        //uint32_t zaxxon_state::screen_update_futspy(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t zaxxon_state::screen_update_razmataz(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t zaxxon_state::screen_update_ixion(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t zaxxon_state::screen_update_congo(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)
    }
}
