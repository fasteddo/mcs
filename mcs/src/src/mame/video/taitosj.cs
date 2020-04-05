// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int8_t = System.SByte;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using pen_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    partial class taitosj_state : driver_device
    {
        bool GLOBAL_FLIP_X { get { return (m_video_mode[0] & 0x01) != 0; } }
        bool GLOBAL_FLIP_Y { get { return (m_video_mode[0] & 0x02) != 0; } }
        int SPRITE_RAM_PAGE_OFFSET { get { return (m_video_mode[0] & 0x04) != 0 ? 0x80 : 0x00; } }
        bool SPRITES_ON { get { return (m_video_mode[0] & 0x80) != 0; } }
        const uint32_t TRANSPARENT_PEN         = 0x40;


        static readonly int [] layer_enable_mask = new int[3] { 0x10, 0x20, 0x40 };


        /***************************************************************************
          Convert the color PROMs into a more useable format.

          The Taito games don't have a color PROM. They use RAM to dynamically
          create the palette. The resolution is 9 bit (3 bits per gun).

          The RAM is connected to the RGB output this way:

          bit 0 -- inverter -- 270 ohm resistor  -- RED
          bit 7 -- inverter -- 470 ohm resistor  -- RED
                -- inverter -- 1  kohm resistor  -- RED
                -- inverter -- 270 ohm resistor  -- GREEN
                -- inverter -- 470 ohm resistor  -- GREEN
                -- inverter -- 1  kohm resistor  -- GREEN
                -- inverter -- 270 ohm resistor  -- BLUE
                -- inverter -- 470 ohm resistor  -- BLUE
          bit 0 -- inverter -- 1  kohm resistor  -- BLUE
        ***************************************************************************/
        void set_pens()
        {
            int [] resistances = new int[3] { 1000, 470, 270 };
            double [] rweights;
            double [] gweights;
            double [] bweights;
            int i;

            /* compute the color output resistor weights */
            compute_resistor_weights(0, 255, -1.0,
                    3, resistances, out rweights, 0, 0,
                    3, resistances, out gweights, 0, 0,
                    3, resistances, out bweights, 0, 0);

            for (i = 0; i < 0x40; i++)
            {
                int bit0;
                int bit1;
                int bit2;
                int r;
                int g;
                int b;
                int val;

                /* red component */
                val = m_paletteram[(i << 1) | 0x01];
                bit0 = (~val >> 6) & 0x01;
                bit1 = (~val >> 7) & 0x01;
                val = m_paletteram[(i << 1) | 0x00];
                bit2 = (~val >> 0) & 0x01;
                r = combine_weights(rweights, bit0, bit1, bit2);

                /* green component */
                val = m_paletteram[(i << 1) | 0x01];
                bit0 = (~val >> 3) & 0x01;
                bit1 = (~val >> 4) & 0x01;
                bit2 = (~val >> 5) & 0x01;
                g = combine_weights(gweights, bit0, bit1, bit2);

                /* blue component */
                val = m_paletteram[(i << 1) | 0x01];
                bit0 = (~val >> 0) & 0x01;
                bit1 = (~val >> 1) & 0x01;
                bit2 = (~val >> 2) & 0x01;
                b = combine_weights(bweights, bit0, bit1, bit2);

                m_palette.target.palette_interface.set_pen_color((pen_t)i, new rgb_t((u8)r, (u8)g, (u8)b));
            }
        }


        /***************************************************************************
          Start the video hardware emulation.
        ***************************************************************************/
        void compute_draw_order()
        {
            int i;
            var color_prom = memregion("proms").base_();  //uint8_t *color_prom = memregion("proms")->base();

            /* do a simple conversion of the PROM into layer priority order. Note that */
            /* this is a simplification, which assumes the PROM encodes a sensible priority */
            /* scheme. */
            for (i = 0; i < 32; i++)
            {
                int j;
                int mask = 0;   /* start with all four layers active, so we'll get the highest */
                                /* priority one in the first loop */
                for (j = 3; j >= 0; j--)
                {
                    int data = color_prom[0x10 * (i & 0x0f) + mask] & 0x0f;

                    if ((i & 0x10) != 0)
                        data = data >> 2;
                    else
                        data = data & 0x03;

                    mask |= (1 << data);    /* in next loop, we'll see which of the remaining */
                                            /* layers has top priority when this one is transparent */
                    m_draw_order[i, j] = data;
                }
            }
        }


        protected override void video_start()
        {
            int i;

            m_sprite_layer_collbitmap1.allocate(16,16);

            for (i = 0; i < 3; i++)
            {
                m_layer_bitmap[i] = new bitmap_ind16();
                m_screen.target.register_screen_bitmap(m_layer_bitmap[i]);
                m_sprite_layer_collbitmap2[i] = new bitmap_ind16();
                m_screen.target.register_screen_bitmap(m_sprite_layer_collbitmap2[i]);
            }

            m_sprite_sprite_collbitmap1.allocate(32,32);
            m_sprite_sprite_collbitmap2.allocate(32,32);

            m_gfxdecode.target.digfx.gfx(0).set_source(new ListBytesPointer(m_characterram.target));
            m_gfxdecode.target.digfx.gfx(1).set_source(new ListBytesPointer(m_characterram.target));
            m_gfxdecode.target.digfx.gfx(2).set_source(new ListBytesPointer(m_characterram.target, 0x1800));
            m_gfxdecode.target.digfx.gfx(3).set_source(new ListBytesPointer(m_characterram.target, 0x1800));

            compute_draw_order();
        }


        //READ8_MEMBER(taitosj_state::taitosj_gfxrom_r)
        u8 taitosj_gfxrom_r(address_space space, offs_t offset, u8 mem_mask = 0xff)
        {
            uint8_t ret;

            offs_t offs = (offs_t)(m_gfxpointer[0] | (m_gfxpointer[1] << 8));

            if (offs < 0x8000)
                ret = memregion("gfx1").base_()[offs];
            else
                ret = 0;

            offs = offs + 1;

            m_gfxpointer[0] = (uint8_t)(offs & 0xff);
            m_gfxpointer[1] = (uint8_t)(offs >> 8);

            return ret;
        }


        //WRITE8_MEMBER(taitosj_state::taitosj_characterram_w)
        void taitosj_characterram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            if (m_characterram[offset] != data)
            {
                if (offset < 0x1800)
                {
                    m_gfxdecode.target.digfx.gfx(0).mark_dirty((offset / 8) & 0xff);
                    m_gfxdecode.target.digfx.gfx(1).mark_dirty((offset / 32) & 0x3f);
                }
                else
                {
                    m_gfxdecode.target.digfx.gfx(2).mark_dirty((offset / 8) & 0xff);
                    m_gfxdecode.target.digfx.gfx(3).mark_dirty((offset / 32) & 0x3f);
                }

                m_characterram[offset] = data;
            }
        }


        //WRITE8_MEMBER(taitosj_state::taitosj_collision_reg_clear_w)
        void taitosj_collision_reg_clear_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            m_collision_reg[0] = 0;
            m_collision_reg[1] = 0;
            m_collision_reg[2] = 0;
            m_collision_reg[3] = 0;
        }


        bool get_sprite_xy(uint8_t which, out uint8_t sx, out uint8_t sy)
        {
            offs_t offs = (offs_t)(which * 4);

            sx = (uint8_t)(      m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs + 0] - 1);
            sy = (uint8_t)(240 - m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs + 1]);

            return sy < 240;
        }


        gfx_element get_sprite_gfx_element(uint8_t which)
        {
            offs_t offs = (offs_t)(which * 4);

            return m_gfxdecode.target.digfx.gfx((m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs + 3] & 0x40) != 0 ? 3 : 1);
        }


        int check_sprite_sprite_bitpattern(int sx1, int sy1, int which1,int sx2, int sy2, int which2)
        {
            int x;
            int y;
            int minx;
            int miny;
            int maxx = 16;
            int maxy = 16;

            offs_t offs1 = (offs_t)(which1 * 4);
            offs_t offs2 = (offs_t)(which2 * 4);

            /* normalize coordinates to (0,0) and compute overlap */
            if (sx1 < sx2)
            {
                sx2 -= sx1;
                sx1 = 0;
                minx = sx2;
            }
            else
            {
                sx1 -= sx2;
                sx2 = 0;
                minx = sx1;
            }

            if (sy1 < sy2)
            {
                sy2 -= sy1;
                sy1 = 0;
                miny = sy2;
            }
            else
            {
                sy1 -= sy2;
                sy2 = 0;
                miny = sy1;
            }

            /* draw the sprites into separate bitmaps and check overlapping region */
            m_sprite_layer_collbitmap1.fill(TRANSPARENT_PEN);
                get_sprite_gfx_element((uint8_t)which1).transpen(m_sprite_sprite_collbitmap1,m_sprite_sprite_collbitmap1.cliprect(),
                    (u32)(m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs1 + 3] & 0x3f),
                    0,
                    m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs1 + 2] & 0x01,
                    m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs1 + 2] & 0x02,
                    sx1, sy1, 0);

            m_sprite_sprite_collbitmap2.fill(TRANSPARENT_PEN);
                get_sprite_gfx_element((uint8_t)which2).transpen(m_sprite_sprite_collbitmap2,m_sprite_sprite_collbitmap2.cliprect(),
                    (u32)(m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs2 + 3] & 0x3f),
                    0,
                    m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs2 + 2] & 0x01,
                    m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs2 + 2] & 0x02,
                    sx2, sy2, 0);

            for (y = miny; y < maxy; y++)
            {
                for (x = minx; x < maxx; x++)
                {
                    if ((m_sprite_sprite_collbitmap1.pix16(y, x)[0] != TRANSPARENT_PEN) &&
                        (m_sprite_sprite_collbitmap2.pix16(y, x)[0] != TRANSPARENT_PEN))
                        return 1;  /* collided */
                }
            }

            return 0;
        }


        void check_sprite_sprite_collision()
        {
            if (SPRITES_ON)
            {
                int which1;

                /* chech each pair of sprites */
                for (which1 = 0; which1 < 0x20; which1++)
                {
                    int which2;
                    uint8_t sx1;
                    uint8_t sy1;

                    if ((which1 >= 0x10) && (which1 <= 0x17)) continue; /* no sprites here */

                    if (!get_sprite_xy((uint8_t)which1, out sx1, out sy1)) continue;

                    for (which2 = which1 + 1; which2 < 0x20; which2++)
                    {
                        uint8_t sx2;
                        uint8_t sy2;

                        if ((which2 >= 0x10) && (which2 <= 0x17)) continue;   /* no sprites here */

                        if (!get_sprite_xy((uint8_t)which2, out sx2, out sy2)) continue;

                        /* quickly rule out any pairs that cannot be touching */
                        if ((std.abs((int8_t)sx1 - (int8_t)sx2) < 16) &&
                            (std.abs((int8_t)sy1 - (int8_t)sy2) < 16))
                        {
                            int reg;

                            if (check_sprite_sprite_bitpattern(sx1, sy1, which1, sx2, sy2, which2) == 0)  continue;

                            /* mark sprite as collided */
                            /* note that only the sprite with the higher number is marked */
                            /* as collided. This is how the hardware works and required */
                            /* by Pirate Pete to be able to finish the last round. */

                            /* the last sprite has to be moved at the start of the list */
                            if (which2 == 0x1f)
                            {
                                reg = which1 >> 3;
                                if (reg == 3)  reg = 2;

                                m_collision_reg[reg] |= (uint8_t)(1 << (which1 & 0x07));
                            }
                            else
                            {
                                reg = which2 >> 3;
                                if (reg == 3)  reg = 2;

                                m_collision_reg[reg] |= (uint8_t)(1 << (which2 & 0x07));
                            }
                        }
                    }
                }
            }
        }


        void calculate_sprite_areas(int [] sprites_on, rectangle [] sprite_areas)
        {
            int which;
            int width = m_screen.target.width();
            int height = m_screen.target.height();

            for (which = 0; which < 0x20; which++)
            {
                uint8_t sx;
                uint8_t sy;

                if ((which >= 0x10) && (which <= 0x17)) continue;   /* no sprites here */

                if (get_sprite_xy((uint8_t)which, out sx, out sy))
                {
                    int minx, miny, maxx, maxy;

                    if (GLOBAL_FLIP_X)
                        sx = (uint8_t)(238 - sx);

                    if (GLOBAL_FLIP_Y)
                        sy = (uint8_t)(242 - sy);

                    minx = sx;
                    miny = sy;

                    maxx = minx + 15;
                    maxy = miny + 15;

                    /* check for bitmap bounds to avoid illegal memory access */
                    if (minx < 0) minx = 0;
                    if (miny < 0) miny = 0;
                    if (maxx >= width - 1)
                        maxx = width - 1;
                    if (maxy >= height - 1)
                        maxy = height - 1;

                    sprite_areas[which] = new rectangle();
                    sprite_areas[which].min_x = minx;
                    sprite_areas[which].max_x = maxx;
                    sprite_areas[which].min_y = miny;
                    sprite_areas[which].max_y = maxy;

                    sprites_on[which] = 1;
                }
                /* sprite is off */
                else
                {
                    sprites_on[which] = 0;
                }
            }
        }


        int check_sprite_layer_bitpattern(int which, rectangle [] sprite_areas)
        {
            int y;
            int x;
            offs_t offs = (offs_t)(which * 4);
            int result = 0;  /* no collisions */

            int check_layer_1 = m_video_mode[0] & layer_enable_mask[0];
            int check_layer_2 = m_video_mode[0] & layer_enable_mask[1];
            int check_layer_3 = m_video_mode[0] & layer_enable_mask[2];

            int minx = sprite_areas[which].min_x;
            int miny = sprite_areas[which].min_y;
            int maxx = sprite_areas[which].max_x + 1;
            int maxy = sprite_areas[which].max_y + 1;

            int flip_x = (m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs + 2] & 0x01) ^ (GLOBAL_FLIP_X ? 1 : 0);
            int flip_y = (m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs + 2] & 0x02) ^ (GLOBAL_FLIP_Y ? 1 : 0);

            /* draw sprite into a bitmap and check if layers collide */
            m_sprite_layer_collbitmap1.fill(TRANSPARENT_PEN);
            get_sprite_gfx_element((uint8_t)which).transpen(m_sprite_layer_collbitmap1,m_sprite_layer_collbitmap1.cliprect(),
                    (u32)(m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs + 3] & 0x3f),
                    0,
                    flip_x, flip_y,
                    0,0,0);

            for (y = miny; y < maxy; y++)
            {
                for (x = minx; x < maxx; x++)
                {
                    if (m_sprite_layer_collbitmap1.pix16(y - miny, x - minx)[0] != TRANSPARENT_PEN) /* is there anything to check for ? */
                    {
                        if (check_layer_1 != 0 && (m_sprite_layer_collbitmap2[0].pix16(y, x)[0] != TRANSPARENT_PEN))
                            result |= 0x01;  /* collided with layer 1 */

                        if (check_layer_2 != 0 && (m_sprite_layer_collbitmap2[1].pix16(y, x)[0] != TRANSPARENT_PEN))
                            result |= 0x02;  /* collided with layer 2 */

                        if (check_layer_3 != 0 && (m_sprite_layer_collbitmap2[2].pix16(y, x)[0] != TRANSPARENT_PEN))
                            result |= 0x04;  /* collided with layer 3 */
                    }
                }
            }

            return result;
        }


        void check_sprite_layer_collision(int [] sprites_on, rectangle [] sprite_areas)
        {
            if (SPRITES_ON)
            {
                int which;

                /* check each sprite */
                for (which = 0; which < 0x20; which++)
                {
                    if ((which >= 0x10) && (which <= 0x17)) continue;   /* no sprites here */

                    if (sprites_on[which] != 0)
                        m_collision_reg[3] |= (uint8_t)check_sprite_layer_bitpattern(which, sprite_areas);
                }
            }
        }


        void draw_layers()
        {
            offs_t offs;

            m_layer_bitmap[0].fill(TRANSPARENT_PEN);
            m_layer_bitmap[1].fill(TRANSPARENT_PEN);
            m_layer_bitmap[2].fill(TRANSPARENT_PEN);

            for (offs = 0; offs < 0x0400; offs++)
            {
                int sx = (int)(offs % 32);
                int sy = (int)(offs / 32);

                if (GLOBAL_FLIP_X) sx = 31 - sx;
                if (GLOBAL_FLIP_Y) sy = 31 - sy;

                m_gfxdecode.target.digfx.gfx((m_colorbank[0] & 0x08) != 0 ? 2 : 0).transpen(m_layer_bitmap[0],m_layer_bitmap[0].cliprect(),
                        m_videoram_1[offs],
                        (u32)(m_colorbank[0] & 0x07),
                        GLOBAL_FLIP_X ? 1 : 0,GLOBAL_FLIP_Y ? 1 : 0,
                        8*sx,8*sy,0);

                m_gfxdecode.target.digfx.gfx((m_colorbank[0] & 0x80) != 0 ? 2 : 0).transpen(m_layer_bitmap[1],m_layer_bitmap[1].cliprect(),
                        m_videoram_2[offs],
                        (u32)((m_colorbank[0] >> 4) & 0x07),
                        GLOBAL_FLIP_X ? 1 : 0,GLOBAL_FLIP_Y ? 1 : 0,
                        8*sx,8*sy,0);

                m_gfxdecode.target.digfx.gfx((m_colorbank[1] & 0x08) != 0 ? 2 : 0).transpen(m_layer_bitmap[2],m_layer_bitmap[2].cliprect(),
                        m_videoram_3[offs],
                        (u32)(m_colorbank[1] & 0x07),
                        GLOBAL_FLIP_X ? 1 : 0,GLOBAL_FLIP_Y ? 1 : 0,
                        8*sx,8*sy,0);
            }
        }


        void draw_sprites(bitmap_ind16 bitmap)
        {
            /*
               sprite visibility area is missing 4 pixels from the sides, surely to reduce
               wraparound side effects. This was verified on a real Elevator Action.
               Note that the clipping is asymmetrical. This matches the real thing.
               I'm not sure of what should happen when the screen is flipped, though.
             */
            rectangle spritevisiblearea = new rectangle(0*8+3, 32*8-1-1, 2*8, 30*8-1);
            rectangle spritevisibleareaflip = new rectangle(0*8+1, 32*8-3-1, 2*8, 30*8-1);

            if (SPRITES_ON)
            {
                int sprite;

                /* drawing order is a bit strange. The last sprite has to be moved at the start of the list. */
                for (sprite = 0x1f; sprite >= 0; sprite--)
                {
                    uint8_t sx;
                    uint8_t sy;

                    int which = (sprite - 1) & 0x1f;    /* move last sprite at the head of the list */
                    offs_t offs = (offs_t)(which * 4);

                    if ((which >= 0x10) && (which <= 0x17)) continue;   /* no sprites here */

                    if (get_sprite_xy((uint8_t)which, out sx, out sy))
                    {
                        int code = m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs + 3] & 0x3f;
                        int color = 2 * ((m_colorbank[1] >> 4) & 0x03) + ((m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs + 2] >> 2) & 0x01);
                        int flip_x = m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs + 2] & 0x01;
                        int flip_y = m_spriteram[SPRITE_RAM_PAGE_OFFSET + (int)offs + 2] & 0x02;

                        if (GLOBAL_FLIP_X)
                        {
                            sx = (uint8_t)(238 - sx);
                            flip_x = flip_x == 0 ? 1 : 0;
                        }

                        if (GLOBAL_FLIP_Y)
                        {
                            sy = (uint8_t)(242 - sy);
                            flip_y = flip_y == 0 ? 1 : 0;
                        }

                        get_sprite_gfx_element((uint8_t)which).transpen(bitmap,GLOBAL_FLIP_X ? spritevisibleareaflip : spritevisiblearea, (u32)code, (u32)color,
                                flip_x, flip_y, sx, sy,0);

                        /* draw with wrap around. The horizontal games (eg. sfposeid) need this */
                        get_sprite_gfx_element((uint8_t)which).transpen(bitmap,GLOBAL_FLIP_X ? spritevisibleareaflip : spritevisiblearea, (u32)code, (u32)color,
                                flip_x, flip_y, sx - 0x100, sy,0);
                    }
                }
            }
        }


        void taitosj_copy_layer(bitmap_ind16 bitmap, rectangle cliprect, int which, int [] sprites_on, rectangle [] sprite_areas)
        {
            int [] fudge1 = new int [3] { 3,  1, -1 };
            int [] fudge2 = new int [3] { 8, 10, 12 };

            if ((m_video_mode[0] & layer_enable_mask[which]) != 0)
            {
                int i;
                int scrollx;
                int [] scrolly = new int [32];

                scrollx = m_scroll[2 * which];

                if (GLOBAL_FLIP_X)
                    scrollx =  (scrollx & 0xf8) + ((scrollx + fudge1[which]) & 7) + fudge2[which];
                else
                    scrollx = -(scrollx & 0xf8) + ((scrollx + fudge1[which]) & 7) + fudge2[which];

                if (GLOBAL_FLIP_Y)
                {
                    for (i = 0;i < 32;i++)
                        scrolly[31 - i] =  m_colscrolly[32 * which + i] + m_scroll[2 * which + 1];
                }
                else
                {
                    for (i = 0;i < 32;i++)
                        scrolly[i]      = -m_colscrolly[32 * which + i] - m_scroll[2 * which + 1];
                }

                copyscrollbitmap_trans(bitmap, m_layer_bitmap[which], 1, new int [] { scrollx }, 32, scrolly, cliprect, TRANSPARENT_PEN);

                /* store parts covered with sprites for sprites/layers collision detection */
                for (i = 0; i < 0x20; i++)
                {
                    if ((i >= 0x10) && (i <= 0x17)) continue; /* no sprites here */

                    if (sprites_on[i] != 0)
                        copyscrollbitmap(m_sprite_layer_collbitmap2[which], m_layer_bitmap[which], 1, new int [] { scrollx }, 32, scrolly, sprite_areas[i]);
                }
            }
        }


        void copy_layer(bitmap_ind16 bitmap, rectangle cliprect, copy_layer_func_t copy_layer_func, int which, int [] sprites_on, rectangle [] sprite_areas)
        {
            if (which == 0)
                draw_sprites(bitmap);
            else
                copy_layer_func(bitmap, cliprect, which - 1, sprites_on, sprite_areas);
        }


        void copy_layers(bitmap_ind16 bitmap, rectangle cliprect, copy_layer_func_t copy_layer_func, int [] sprites_on, rectangle [] sprite_areas)
        {
            int i = 0;

            /* fill the screen with the background color */
            bitmap.fill((uint32_t)(8 * (m_colorbank[1] & 0x07)), cliprect);

            for (i = 0; i < 4; i++)
            {
                int which = m_draw_order[m_video_priority[0] & 0x1f, i];

                copy_layer(bitmap, cliprect, copy_layer_func, which, sprites_on, sprite_areas);
            }
        }


        void check_collision(int [] sprites_on, rectangle [] sprite_areas)
        {
            check_sprite_sprite_collision();

            check_sprite_layer_collision(sprites_on, sprite_areas);

            /*check_layer_layer_collision();*/  /* not implemented !!! */
        }


        int video_update_common(bitmap_ind16 bitmap, rectangle cliprect, copy_layer_func_t copy_layer_func)
        {
            int [] sprites_on = new int[0x20];           /* 1 if sprite is active */
            rectangle [] sprite_areas = new rectangle[0x20];   /* areas on bitmap (sprite locations) */

            set_pens();

            draw_layers();

            calculate_sprite_areas(sprites_on, sprite_areas);

            copy_layers(bitmap, cliprect, copy_layer_func, sprites_on, sprite_areas);

            /*check_sprite_layer_collision() uses drawn bitmaps, so it must me called _AFTER_ draw_layers() */
            check_collision(sprites_on, sprite_areas);

            return 0;
        }


        u32 screen_update_taitosj(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            return (u32)video_update_common(bitmap, cliprect, taitosj_copy_layer);
        }
    }
}
