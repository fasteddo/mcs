// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;


namespace mame
{
    //#define TOTAL_COLORS(gfxn) (m_gfxdecode->gfx(gfxn)->colors() * m_gfxdecode->gfx(gfxn)->granularity())
    //#define COLOR(gfxn,offs) (m_gfxdecode->gfx(gfxn)->colorbase() + offs)


    partial class snk6502_state : driver_device
    {
        /***************************************************************************
          Convert the color PROMs into a more useable format.

          Zarzon has a different PROM layout from the others.
        ***************************************************************************/
        protected void snk6502_palette(palette_device palette)
        {
            throw new emu_unimplemented();
#if false
            uint8_t const *const color_prom = memregion("proms")->base();
            for (int i = 0; i < palette.entries(); i++)
            {
                int bit0, bit1, bit2;

                // red component
                bit0 = BIT(color_prom[i], 0);
                bit1 = BIT(color_prom[i], 1);
                bit2 = BIT(color_prom[i], 2);

                int const r = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;

                // green component
                bit0 = BIT(color_prom[i], 3);
                bit1 = BIT(color_prom[i], 4);
                bit2 = BIT(color_prom[i], 5);

                int const g = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;

                // blue component
                bit0 = 0;
                bit1 = BIT(color_prom[i], 6);
                bit2 = BIT(color_prom[i], 7);

                int const b = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;

                m_palette_val[i] = rgb_t(r, g, b);
            }

            m_backcolor = 0;    // background color can be changed by the game

            for (int i = 0; i < TOTAL_COLORS(0); i++)
                palette.set_pen_color(COLOR(0, i), m_palette_val[i]);

            for (int i = 0; i < TOTAL_COLORS(1); i++)
            {
                if (i % 4 == 0)
                    palette.set_pen_color(COLOR(1, i), m_palette_val[4 * m_backcolor + 0x20]);
                else
                    palette.set_pen_color(COLOR(1, i), m_palette_val[i + 0x20]);
            }
#endif
        }


#if false
        void snk6502_state::videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram[offset] = data;
            m_bg_tilemap->mark_tile_dirty(offset);
        }

        void snk6502_state::videoram2_w(offs_t offset, uint8_t data)
        {
            m_videoram2[offset] = data;
            m_fg_tilemap->mark_tile_dirty(offset);
        }

        void snk6502_state::colorram_w(offs_t offset, uint8_t data)
        {
            m_colorram[offset] = data;
            m_bg_tilemap->mark_tile_dirty(offset);
            m_fg_tilemap->mark_tile_dirty(offset);
        }

        void snk6502_state::charram_w(offs_t offset, uint8_t data)
        {
            if (m_charram[offset] != data)
            {
                m_charram[offset] = data;
                m_gfxdecode->gfx(0)->mark_dirty((offset/8) % 256);
            }
        }


        void snk6502_state::flipscreen_w(uint8_t data)
        {
            /* bits 0-2 select background color */

            if (m_backcolor != (data & 7))
            {
                m_backcolor = data & 7;

                for (int i = 0;i < 32;i += 4)
                    m_palette->set_pen_color(COLOR(1, i), m_palette_val[4 * m_backcolor + 0x20]);
            }

            /* bit 3 selects char bank */

            int bank = (~data & 0x08) >> 3;

            if (m_charbank != bank)
            {
                m_charbank = bank;
                machine().tilemap().mark_all_dirty();
            }

            /* bit 7 flips screen */

            if (flip_screen() != (data & 0x80))
            {
                flip_screen_set(data & 0x80);
                machine().tilemap().mark_all_dirty();
            }
        }

        //void fantasy_state::fantasy_flipscreen_w(offs_t offset, uint8_t data)

        void snk6502_state::scrollx_w(uint8_t data)
        {
            m_bg_tilemap->set_scrollx(0, data);
        }

        void snk6502_state::scrolly_w(uint8_t data)
        {
            m_bg_tilemap->set_scrolly(0, data);
        }


        TILE_GET_INFO_MEMBER(snk6502_state::get_bg_tile_info)
        {
            int code = m_videoram[tile_index] + 256 * m_charbank;
            int color = (m_colorram[tile_index] & 0x38) >> 3;

            tileinfo.set(1, code, color, 0);
        }

        TILE_GET_INFO_MEMBER(snk6502_state::get_fg_tile_info)
        {
            int code = m_videoram2[tile_index];
            int color = m_colorram[tile_index] & 0x07;

            tileinfo.set(0, code, color, 0);
        }
#endif


        //VIDEO_START_MEMBER(snk6502_state,snk6502)
        protected void video_start_snk6502()
        {
            throw new emu_unimplemented();
#if false
            m_bg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(snk6502_state::get_bg_tile_info)), TILEMAP_SCAN_ROWS, 8, 8, 32, 32);
            m_fg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(snk6502_state::get_fg_tile_info)), TILEMAP_SCAN_ROWS, 8, 8, 32, 32);

            m_fg_tilemap->set_transparent_pen(0);

            m_gfxdecode->gfx(0)->set_source(m_charram);
            machine().save().register_postload(save_prepost_delegate(FUNC(snk6502_state::postload), this));
#endif
        }


#if false
        void snk6502_state::postload()
        {
            m_gfxdecode->gfx(0)->mark_all_dirty();
        }
#endif


        //VIDEO_START_MEMBER(snk6502_state,pballoon)


        protected uint32_t screen_update(screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
#if false
            m_bg_tilemap->draw(screen, bitmap, cliprect, 0, 0);
            m_fg_tilemap->draw(screen, bitmap, cliprect, 0, 0);
            return 0;
#endif
        }


        //void snk6502_state::satansat_palette(palette_device &palette)

        //void snk6502_state::satansat_b002_w(uint8_t data)

        //void snk6502_state::satansat_backcolor_w(uint8_t data)

        //TILE_GET_INFO_MEMBER(snk6502_state::satansat_get_bg_tile_info)

        //TILE_GET_INFO_MEMBER(snk6502_state::satansat_get_fg_tile_info)

        //VIDEO_START_MEMBER(snk6502_state,satansat)
    }
}
