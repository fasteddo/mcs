// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using PointerU8 = mame.Pointer<System.Byte>;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    partial class btime_state : driver_device
    {
        void btime_palette(palette_device palette)
        {
            // Burger Time doesn't have a color PROM, but Hamburge has.
            // This function is also used by Eggs.
            if (!m_prom_region.bool_)
                return;

            PointerU8 color_prom = new PointerU8(m_prom_region.op0.base_());  //uint8_t const *const color_prom = m_prom_region->base();

            for (int i = 0; i < palette.dipalette.entries(); i++)
            {
                // red component
                int bit0 = (color_prom[i] >> 0) & 0x01;
                int bit1 = (color_prom[i] >> 1) & 0x01;
                int bit2 = (color_prom[i] >> 2) & 0x01;
                int r = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;

                // green component
                bit0 = (color_prom[i] >> 3) & 0x01;
                bit1 = (color_prom[i] >> 4) & 0x01;
                bit2 = (color_prom[i] >> 5) & 0x01;
                int g = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;

                // blue component
                bit0 = 0;
                bit1 = (color_prom[i] >> 6) & 0x01;
                bit2 = (color_prom[i] >> 7) & 0x01;
                int b = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;

                palette.set_pen_color((pen_t)i, new rgb_t((uint8_t)r, (uint8_t)g, (uint8_t)b));
            }
        }


        //void btime_state::lnc_palette(palette_device &palette) const


        /***************************************************************************

        Start the video hardware emulation.

        ***************************************************************************/

        //VIDEO_START_MEMBER(btime_state,disco)

        //VIDEO_START_MEMBER(btime_state,bnj)

        //void btime_state::lnc_videoram_w(offs_t offset, uint8_t data)


        uint8_t btime_mirrorvideoram_r(offs_t offset)
        {
            int x;
            int y;

            /* swap x and y coordinates */
            x = (int)(offset / 32);
            y = (int)(offset % 32);
            offset = (offs_t)(32 * y + x);

            return m_videoram.op[offset];
        }


        uint8_t btime_mirrorcolorram_r(offs_t offset)
        {
            int x;
            int y;

            /* swap x and y coordinates */
            x = (int)(offset / 32);
            y = (int)(offset % 32);
            offset = (offs_t)(32 * y + x);

            return m_colorram.op[offset];
        }


        void btime_mirrorvideoram_w(offs_t offset, uint8_t data)
        {
            int x;
            int y;

            /* swap x and y coordinates */
            x = (int)(offset / 32);
            y = (int)(offset % 32);
            offset = (offs_t)(32 * y + x);

            m_videoram.op[offset] = data;
        }


        //void btime_state::lnc_mirrorvideoram_w(offs_t offset, uint8_t data)


        void btime_mirrorcolorram_w(offs_t offset, uint8_t data)
        {
            int x;
            int y;

            /* swap x and y coordinates */
            x = (int)(offset / 32);
            y = (int)(offset % 32);
            offset = (offs_t)(32 * y + x);

            m_colorram.op[offset] = data;
        }


        //void btime_state::deco_charram_w(offs_t offset, uint8_t data)

        //void btime_state::bnj_background_w(offs_t offset, uint8_t data)


        void bnj_scroll1_w(uint8_t data)
        {
            m_bnj_scroll1 = data;
        }


        //void btime_state::bnj_scroll2_w(uint8_t data)


        void btime_video_control_w(uint8_t data)
        {
            // Btime video control
            //
            // Bit 0   = Flip screen
            // Bit 1-7 = Unknown

            flip_screen_set(data & 0x01);
        }


        //void btime_state::bnj_video_control_w(uint8_t data)

        //void btime_state::zoar_video_control_w(uint8_t data)

        //void btime_state::disco_video_control_w(uint8_t data)


        void draw_chars(bitmap_ind16 bitmap, rectangle cliprect, uint8_t transparency, uint8_t color, int priority)
        {
            offs_t offs;

            for (offs = 0; offs < m_videoram.bytes(); offs++)
            {
                uint8_t x = (uint8_t)(31 - (offs / 32));
                uint8_t y = (uint8_t)(offs % 32);

                uint16_t code = (uint16_t)(m_videoram.op[offs] + 256 * (m_colorram.op[offs] & 3));

                /* check priority */
                if ((priority != -1) && (priority != ((code >> 7) & 0x01)))
                    continue;

                if (flip_screen() != 0)
                {
                    x = (uint8_t)(31 - x);
                    y = (uint8_t)(31 - y);
                }

                m_gfxdecode.op0.gfx(0).transpen(bitmap, cliprect,
                        code,
                        color,
                        (int)flip_screen(), (int)flip_screen(),
                        8 * x, 8 * y,
                        transparency != 0 ? 0 : unchecked((uint32_t)(-1)));
            }
        }


        void draw_sprites(bitmap_ind16 bitmap, rectangle cliprect, uint8_t color,
                                    uint8_t sprite_y_adjust, uint8_t sprite_y_adjust_flip_screen,
                                    Pointer<uint8_t> sprite_ram, offs_t interleave)
        {
            int i;
            offs_t offs;

            /* draw the sprites */
            for (i = 0, offs = 0; i < 8; i++, offs += 4 * interleave)
            {
                int x;
                int y;
                uint8_t flipx;
                uint8_t flipy;

                if ((sprite_ram[offs + 0] & 0x01) == 0) continue;

                x = 240 - sprite_ram[offs + 3 * interleave];
                y = 240 - sprite_ram[offs + 2 * interleave];

                flipx = (uint8_t)(sprite_ram[offs + 0] & 0x04);
                flipy = (uint8_t)(sprite_ram[offs + 0] & 0x02);

                if (flip_screen() != 0)
                {
                    x = 240 - x;
                    y = 240 - y + sprite_y_adjust_flip_screen;

                    flipx = flipx == 0 ? (uint8_t)1 : (uint8_t)0;
                    flipy = flipy == 0 ? (uint8_t)1 : (uint8_t)0;
                }

                y = y - sprite_y_adjust;

                m_gfxdecode.op0.gfx(1).transpen(bitmap, cliprect,
                        sprite_ram[offs + interleave],
                        color,
                        flipx, flipy,
                        x, y, 0);

                y = y + (flip_screen() != 0 ? -256 : 256);

                // Wrap around
                m_gfxdecode.op0.gfx(1).transpen(bitmap, cliprect,
                        sprite_ram[offs + interleave],
                        color,
                        flipx, flipy,
                        x, y, 0);
            }
        }


        void draw_background(bitmap_ind16 bitmap, rectangle cliprect, uint8_t [] tmap, uint8_t color)
        {
            int i;
            PointerU8 gfx = new PointerU8(memregion("bg_map").base_());  //const uint8_t *gfx = memregion("bg_map")->base();
            int scroll = -(m_bnj_scroll2 | ((m_bnj_scroll1 & 0x03) << 8));

            // One extra iteration for wrap around
            for (i = 0; i < 5; i++, scroll += 256)
            {
                offs_t offs;
                offs_t tileoffset = (offs_t)tmap[i & 3] * 0x100;

                // Skip if this tile is completely off the screen
                if (scroll > 256)
                    break;
                if (scroll < -256)
                    continue;

                for (offs = 0; offs < 0x100; offs++)
                {
                    int x = 240 - (16 * ((int)offs / 16) + scroll) - 1;
                    int y = 16 * ((int)offs % 16);

                    if (flip_screen() != 0)
                    {
                        x = 240 - x;
                        y = 240 - y;
                    }

                    m_gfxdecode.op0.gfx(2).opaque(bitmap, cliprect,
                            gfx[tileoffset + offs],
                            color,
                            (int)flip_screen(), (int)flip_screen(),
                            x, y);
                }
            }
        }


        uint32_t screen_update_btime(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            if ((m_bnj_scroll1 & 0x10) != 0)
            {
                int i;
                int start;

                // Generate tile map
                if (flip_screen() != 0)
                    start = 0;
                else
                    start = 1;

                for (i = 0; i < 4; i++)
                {
                    m_btime_tilemap[i] = (uint8_t)(start | (m_bnj_scroll1 & 0x04));
                    start = (start + 1) & 0x03;
                }

                draw_background(bitmap, cliprect, m_btime_tilemap, 0);
                draw_chars(bitmap, cliprect, 1, 0, -1);
            }
            else
            {
                draw_chars(bitmap, cliprect, 0, 0, -1);
            }

            draw_sprites(bitmap, cliprect, 0, 1, 0, m_videoram.op, 0x20);

            return 0;
        }


        //uint32_t btime_state::screen_update_eggs(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t btime_state::screen_update_lnc(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t btime_state::screen_update_zoar(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t btime_state::screen_update_bnj(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t btime_state::screen_update_cookrace(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)

        //uint32_t btime_state::screen_update_disco(screen_device &screen, bitmap_ind16 &bitmap, const rectangle &cliprect)
    }
}
