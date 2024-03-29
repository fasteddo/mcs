// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using tilemap_memory_index = System.UInt32;  //typedef u32 tilemap_memory_index;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.emucore_global;
using static mame.tilemap_global;


namespace mame
{
    partial class centiped_state : driver_device
    {
        /*************************************
         *
         *  Tilemap callback
         *
         *************************************/

        //TILE_GET_INFO_MEMBER(centiped_state::centiped_get_tile_info)
        void centiped_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int data = m_videoram[tile_index].op;
            tileinfo.set(0, ((u32)data & 0x3f) + 0x40, 0, TILE_FLIPYX(data >> 6));
        }


        //TILE_GET_INFO_MEMBER(centiped_state::milliped_get_tile_info)
        void milliped_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int data = m_videoram[tile_index].op;
            int bank = ((data >> 6) & 1) | (m_gfx_bank << 1);
            int color = (data >> 6) & 3;
            /* Flip both x and y if flipscreen is non-zero */
            int flip_tiles = (m_flipscreen != 0) ? 0x03 : 0;

            tileinfo.set(0, ((u32)data & 0x3f) + 0x40 + ((u32)bank * 0x80), (u32)color, TILE_FLIPYX(flip_tiles));
        }


        /*************************************
         *
         *  Video system start
         *
         *************************************/

        void init_penmask()
        {
            for (int i = 0; i < 64; i++)
            {
                uint8_t mask = 1;
                if (((i >> 0) & 3) == 0) mask |= 2;
                if (((i >> 2) & 3) == 0) mask |= 4;
                if (((i >> 4) & 3) == 0) mask |= 8;
                m_penmask[i] = mask;
            }
        }


        void init_common()
        {
            save_item(NAME(new { m_flipscreen }));
            save_item(NAME(new { m_gfx_bank }));
            save_item(NAME(new { m_bullsdrt_sprites_bank }));

            m_flipscreen = 0;
            m_gfx_bank = 0;
            m_bullsdrt_sprites_bank = 0;
        }


        //VIDEO_START_MEMBER(centiped_state,centiped)
        void video_start_centiped()
        {
            init_common();
            init_penmask();

            m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op0, centiped_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8, 8, 32, 32);  //m_bg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(centiped_state::centiped_get_tile_info)), TILEMAP_SCAN_ROWS, 8, 8, 32, 32);
        }


        //VIDEO_START_MEMBER(centiped_state,milliped)
        void video_start_milliped()
        {
            init_common();
            init_penmask();

            m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op0, milliped_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, 8, 8, 32, 32);  //m_bg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(centiped_state::milliped_get_tile_info)), TILEMAP_SCAN_ROWS, 8, 8, 32, 32);
        }


        /*************************************
         *
         *  Video RAM writes
         *
         *************************************/

        void centiped_videoram_w(offs_t offset, uint8_t data)
        {
            m_videoram[offset].op = data;
            m_bg_tilemap.mark_tile_dirty(offset);
        }


        /*************************************
         *
         *  Screen flip
         *
         *************************************/

        //WRITE_LINE_MEMBER(centiped_state::flip_screen_w)
        void flip_screen_w(int state)
        {
            m_flipscreen = (byte)state;
        }


        /*************************************
         *
         *  Graphics bank
         *
         *************************************/

        //void centiped_state::multiped_gfxbank_w(uint8_t data)
        //void centiped_state::bullsdrt_tilesbank_w(offs_t offset, uint8_t data)
        //void centiped_state::bullsdrt_sprites_bank_w(uint8_t data)


        /***************************************************************************

            Centipede doesn't have a color PROM. Eight RAM locations control
            the color of characters and sprites. The meanings of the four bits are
            (all bits are inverted):

            bit 3 alternate
                  blue
                  green
            bit 0 red

            The alternate bit affects blue and green, not red. The way I weighted its
            effect might not be perfectly accurate, but is reasonably close.

            Centipede is unusual because the sprite color code specifies the
            colors to use one by one, instead of a combination code.

            FIXME: handle this using standard indirect colors instead of
                   custom implementation

            bit 5-4 = color to use for pen 11
            bit 3-2 = color to use for pen 10
            bit 1-0 = color to use for pen 01
            pen 00 is transparent

        ***************************************************************************/

        void centiped_paletteram_w(offs_t offset, uint8_t data)
        {
            m_paletteram[offset].op = data;

            /* bit 2 of the output palette RAM is always pulled high, so we ignore */
            /* any palette changes unless the write is to a palette RAM address */
            /* that is actually used */
            if ((offset & 4) != 0)
            {
                rgb_t color;

                int r = 0xff * ((~data >> 0) & 1);
                int g = 0xff * ((~data >> 1) & 1);
                int b = 0xff * ((~data >> 2) & 1);

                if ((~data & 0x08) != 0) /* alternate = 1 */
                {
                    /* when blue component is not 0, decrease it. When blue component is 0, */
                    /* decrease green component. */
                    if (b != 0) b = 0xc0;
                    else if (g != 0) g = 0xc0;
                }

                color = new rgb_t((u8)r, (u8)g, (u8)b);

                /* character colors, set directly */
                if ((offset & 0x08) == 0)
                    m_palette.op0.set_pen_color(offset & 0x03, color);

                /* sprite colors - set all the applicable ones */
                else
                {
                    int i;

                    offset = offset & 0x03;

                    for (i = 0; i < 0x100; i += 4)
                    {
                        if (offset == ((i >> 2) & 0x03))
                            m_palette.op0.set_pen_color((pen_t)i + 4 + 1, color);

                        if (offset == ((i >> 4) & 0x03))
                            m_palette.op0.set_pen_color((pen_t)i + 4 + 2, color);

                        if (offset == ((i >> 6) & 0x03))
                            m_palette.op0.set_pen_color((pen_t)i + 4 + 3, color);
                    }
                }
            }
        }


        /***************************************************************************

            Millipede doesn't have a color PROM, it uses RAM.
            The RAM seems to be connected to the video output this way:

            bit 7 red
                  red
                  red
                  green
                  green
                  blue
                  blue
            bit 0 blue

            Millipede is unusual because the sprite color code specifies the
            colors to use one by one, instead of a combination code.

            FIXME: handle this using standard indirect colors instead of
                   custom implementation

            bit 7-6 = palette bank (there are 4 groups of 4 colors)
            bit 5-4 = color to use for pen 11
            bit 3-2 = color to use for pen 10
            bit 1-0 = color to use for pen 01
            pen 00 is transparent

        ***************************************************************************/
        void milliped_set_color(offs_t offset, uint8_t data)
        {
            rgb_t color;
            int bit0;
            int bit1;
            int bit2;
            int r;
            int g;
            int b;

            /* red component */
            bit0 = (~data >> 5) & 0x01;
            bit1 = (~data >> 6) & 0x01;
            bit2 = (~data >> 7) & 0x01;
            r = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;

            /* green component */
            bit0 = 0;
            bit1 = (~data >> 3) & 0x01;
            bit2 = (~data >> 4) & 0x01;
            g = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;

            /* blue component */
            bit0 = (~data >> 0) & 0x01;
            bit1 = (~data >> 1) & 0x01;
            bit2 = (~data >> 2) & 0x01;
            b = 0x21 * bit0 + 0x47 * bit1 + 0x97 * bit2;

            color = new rgb_t((u8)r, (u8)g, (u8)b);

            /* character colors, set directly */
            if (offset < 0x10)
                m_palette.op0.set_pen_color(offset, color);

            /* sprite colors - set all the applicable ones */
            else
            {
                int base_ = (int)(offset & 0x0c);

                offset = offset & 0x03;

                for (int i = (base_ << 6); i < (base_ << 6) + 0x100; i += 4)
                {
                    if (offset == ((i >> 2) & 0x03))
                        m_palette.op0.set_pen_color((u32)i + 0x10 + 1, color);

                    if (offset == ((i >> 4) & 0x03))
                        m_palette.op0.set_pen_color((u32)i + 0x10 + 2, color);

                    if (offset == ((i >> 6) & 0x03))
                        m_palette.op0.set_pen_color((u32)i + 0x10 + 3, color);
                }
            }
        }


        void milliped_paletteram_w(offs_t offset, uint8_t data)
        {
            m_paletteram[offset].op = data;

            milliped_set_color(offset, data);
        }


        /*************************************
         *
         *  Video update
         *
         *************************************/

        uint32_t screen_update_centiped(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            rectangle spriteclip = cliprect;

            /* draw the background */
            m_bg_tilemap.draw(screen, bitmap, cliprect, 0, 0);

            /* apply the sprite clip */
            if (m_flipscreen != 0)
                spriteclip.min_x += 8;
            else
                spriteclip.max_x -= 8;

            /* draw the sprites */
            for (int offs = 0; offs < 0x10; offs++)
            {
                int code = ((m_spriteram[offs].op & 0x3e) >> 1) | ((m_spriteram[offs].op & 0x01) << 6);
                int color = m_spriteram[offs + 0x30].op;
                int flipx = (m_spriteram[offs].op >> 6) & 1;
                int flipy = (m_spriteram[offs].op >> 7) & 1;
                int x = m_spriteram[offs + 0x20].op;
                int y = 240 - m_spriteram[offs + 0x10].op;

                m_gfxdecode.op0.gfx(1).transmask(bitmap,spriteclip, (u32)code, (u32)color, flipx, flipy, x, y, m_penmask[color & 0x3f]);
            }

            return 0;
        }


        /*
         * This varies from Centipede, in that flipx is not in
         * the data, but is determined by VIDROT value at 0x2506.
         */
        uint32_t screen_update_milliped(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            rectangle spriteclip = cliprect;

            /* draw the background */
            m_bg_tilemap.draw(screen, bitmap, cliprect, 0, 0);

            /* apply the sprite clip */
            if (m_flipscreen != 0)
                spriteclip.min_x += 8;
            else
                spriteclip.max_x -= 8;

            /* draw the sprites */
            for (int offs = 0; offs < 0x10; offs++)
            {
                int code = ((m_spriteram[offs].op & 0x3e) >> 1) | ((m_spriteram[offs].op & 0x01) << 6) | (m_gfx_bank << 7);
                int color = m_spriteram[offs + 0x30].op;
                int flipx = m_flipscreen;
                int flipy = (m_spriteram[offs].op & 0x80);
                int x = m_spriteram[offs + 0x20].op;
                int y = 240 - m_spriteram[offs + 0x10].op;
                if (flipx != 0)
                {
                    flipy = flipy == 0 ? 1 : 0;
                }

                m_gfxdecode.op0.gfx(1).transmask(bitmap, spriteclip, (u32)code, (u32)color, flipx, flipy, x, y, m_penmask[color & 0x3f]);
            }

            return 0;
        }
    }
}
