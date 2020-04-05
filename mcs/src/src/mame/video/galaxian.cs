// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using pen_t = System.UInt32;
using tilemap_memory_index = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;


namespace mame
{
    partial class galaxian_state : driver_device
    {
        /*************************************
         *
         *  Constants
         *
         *************************************/

        const int STAR_RNG_PERIOD     = (1 << 17) - 1;
        const int RGB_MAXIMUM         = 224;


        /*************************************
         *
         *  Palette setup
         *
         *************************************/

        void galaxian_palette(palette_device palette)
        {
            ListBytesPointer color_prom = new ListBytesPointer(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();
            int [] rgb_resistances3 = new int [3] { 1000, 470, 220 };
            int [] rgb_resistances2 = new int [2] { 470, 220 };

            /*
                Sprite/tilemap colors are mapped through a color PROM as follows:

                  bit 7 -- 220 ohm resistor  -- BLUE
                        -- 470 ohm resistor  -- BLUE
                        -- 220 ohm resistor  -- GREEN
                        -- 470 ohm resistor  -- GREEN
                        -- 1  kohm resistor  -- GREEN
                        -- 220 ohm resistor  -- RED
                        -- 470 ohm resistor  -- RED
                  bit 0 -- 1  kohm resistor  -- RED

                Note that not all boards have this configuration. Namco PCBs may
                have 330 ohm resistors instead of 220, but the default setup has
                also been used by Namco.

                In parallel with these resistors are a pair of 150 ohm and 100 ohm
                resistors on each R,G,B component that are connected to the star
                generator.

                And in parallel with the whole mess are a set of 100 ohm resistors
                on each R,G,B component that are enabled when a shell/missile is
                enabled.

                When computing weights, we use RGB_MAXIMUM as the maximum to give
                headroom for stars and shells/missiles. This is not fully accurate,
                but if we included all possible sources in parallel, the brightness
                of the main game would be very low to allow for all the oversaturation
                of the stars and shells/missiles.
            */
            double [] rweights = new double[3];
            double [] gweights = new double[3];
            double [] bweights = new double[2];
            compute_resistor_weights(0, RGB_MAXIMUM, -1.0,
                    3, rgb_resistances3, out rweights, 470, 0,
                    3, rgb_resistances3, out gweights, 470, 0,
                    2, rgb_resistances2, out bweights, 470, 0);

            // decode the palette first
            int len = (int)memregion("proms").bytes();
            for (int i = 0; i < len; i++)
            {
                byte bit0;
                byte bit1;
                byte bit2;

                /* red component */
                bit0 = (byte)BIT(color_prom[i], 0);
                bit1 = (byte)BIT(color_prom[i], 1);
                bit2 = (byte)BIT(color_prom[i], 2);
                int r = (byte)combine_weights(rweights, bit0, bit1, bit2);

                /* green component */
                bit0 = (byte)BIT(color_prom[i], 3);
                bit1 = (byte)BIT(color_prom[i], 4);
                bit2 = (byte)BIT(color_prom[i], 5);
                int g = (byte)combine_weights(gweights, bit0, bit1, bit2);

                /* blue component */
                bit0 = (byte)BIT(color_prom[i], 6);
                bit1 = (byte)BIT(color_prom[i], 7);
                int b = (byte)combine_weights(bweights, bit0, bit1);

                palette.palette_interface.set_pen_color((pen_t)i, new rgb_t((u8)r, (u8)g, (u8)b));
            }

            /*
                The maximum sprite/tilemap resistance is ~130 Ohms with all RGB
                outputs enabled (1/(1/1000 + 1/470 + 1/220)). Since we normalized
                to RGB_MAXIMUM, this maps RGB_MAXIMUM -> 130 Ohms.

                The stars are at 150 Ohms for the LSB, and 100 Ohms for the MSB.
                This means the 3 potential values are:

                    150 Ohms -> RGB_MAXIMUM * 130 / 150
                    100 Ohms -> RGB_MAXIMUM * 130 / 100
                     60 Ohms -> RGB_MAXIMUM * 130 / 60

                Since we can't saturate that high, we instead approximate this
                by compressing the values proportionally into the 194->255 range.
            */
            int minval = RGB_MAXIMUM * 130 / 150;
            int midval = RGB_MAXIMUM * 130 / 100;
            int maxval = RGB_MAXIMUM * 130 / 60;

            // compute the values for each of 4 possible star values
            uint8_t [] starmap = new uint8_t [4]
            {
                    0,
                    (uint8_t)minval,
                    (uint8_t)(minval + (255 - minval) * (midval - minval) / (maxval - minval)),
                    255
            };

            // generate the colors for the stars
            for (int i = 0; i < 64; i++)
            {
                byte bit0;
                byte bit1;

                // bit 5 = red @ 150 Ohm, bit 4 = red @ 100 Ohm
                bit0 = (byte)BIT(i,5);
                bit1 = (byte)BIT(i,4);
                int r = starmap[(bit1 << 1) | bit0];

                // bit 3 = green @ 150 Ohm, bit 2 = green @ 100 Ohm
                bit0 = (byte)BIT(i,3);
                bit1 = (byte)BIT(i,2);
                int g = starmap[(bit1 << 1) | bit0];

                // bit 1 = blue @ 150 Ohm, bit 0 = blue @ 100 Ohm
                bit0 = (byte)BIT(i,1);
                bit1 = (byte)BIT(i,0);
                int b = starmap[(bit1 << 1) | bit0];

                // set the RGB color
                m_star_color[i] = new rgb_t((u8)r, (u8)g, (u8)b);
            }

            // default bullet colors are white for the first 7, and yellow for the last one
            for (int i = 0; i < 7; i++)
                m_bullet_color[i] = new rgb_t(0xff, 0xff, 0xff);
            m_bullet_color[7] = new rgb_t(0xff, 0xff, 0x00);
        }


        /*************************************
         *
         *  Common video init
         *
         *************************************/

        protected override void video_start()
        {
            /* create a tilemap for the background */
            if (m_sfx_tilemap == 0)
            {
                /* normal galaxian hardware is row-based and individually scrolling columns */
                m_bg_tilemap = machine().tilemap().create(m_gfxdecode.target.digfx, bg_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS, GALAXIAN_XSCALE*8,8, 32,32);
                m_bg_tilemap.set_scroll_cols(32);
            }
            else
            {
                /* sfx hardware is column-based and individually scrolling rows */
                m_bg_tilemap = machine().tilemap().create(m_gfxdecode.target.digfx, bg_get_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_COLS, GALAXIAN_XSCALE*8,8, 32,32);
                m_bg_tilemap.set_scroll_rows(32);
            }
            m_bg_tilemap.set_transparent_pen(0);

            /* initialize globals */
            m_flipscreen_x = 0;
            m_flipscreen_y = 0;
            m_background_enable = 0;
            m_background_blue = 0;
            m_background_red = 0;
            m_background_green = 0;

            /* initialize stars */
            stars_init();

            /* register for save states */
            state_save_register();
        }


        void state_save_register()
        {
            save_item(m_flipscreen_x, "m_flipscreen_x");
            save_item(m_flipscreen_y, "m_flipscreen_y");
            save_item(m_background_enable, "m_background_enable");
            save_item(m_background_red, "m_background_red");
            save_item(m_background_green, "m_background_green");
            save_item(m_background_blue, "m_background_blue");

            save_item(m_sprites_base, "m_sprites_base");
            save_item(m_bullets_base, "m_bullets_base");
            save_item(m_gfxbank, "m_gfxbank");

            save_item(m_stars_enabled, "m_stars_enabled");
            save_item(m_star_rng_origin, "m_star_rng_origin");
            save_item(m_star_rng_origin_frame, "m_star_rng_origin_frame");
            save_item(m_stars_blink_state, "m_stars_blink_state");
        }


        //WRITE8_MEMBER(galaxian_state::galaxian_videoram_w)
        void galaxian_videoram_w(address_space space, offs_t offset, byte data, byte mem_mask = 0xff)
        {
            var videoram = m_videoram.target;  //uint8_t *videoram = m_videoram;

            /* update any video up to the current scanline */
            //m_screen->update_now();
            m_screen.target.update_partial(m_screen.target.vpos());

            /* store the data and mark the corresponding tile dirty */
            videoram[offset] = data;
            m_bg_tilemap.mark_tile_dirty(offset);
        }


        //WRITE8_MEMBER(galaxian_state::galaxian_objram_w)
        void galaxian_objram_w(address_space space, offs_t offset, byte data, byte mem_mask = 0xff)
        {
            /* update any video up to the current scanline */
            //  m_screen->update_now();
            m_screen.target.update_partial(m_screen.target.vpos());

            /* store the data */
            m_spriteram[offset] = data;

            /* the first $40 bytes affect the tilemap */
            if (offset < 0x40)
            {
                /* even entries control the scroll position */
                if ((offset & 0x01) == 0)
                {
                    /* Frogger: top and bottom 4 bits swapped entering the adder */
                    if (m_frogger_adjust != 0)
                        data = (byte)((data >> 4) | (data << 4));
                    if (m_sfx_tilemap == 0)
                        m_bg_tilemap.set_scrolly((int)(offset >> 1), data);
                    else
                        m_bg_tilemap.set_scrollx((int)(offset >> 1), GALAXIAN_XSCALE*data);
                }

                /* odd entries control the color base for the row */
                else
                {
                    for (offset >>= 1; offset < 0x0400; offset += 32)
                        m_bg_tilemap.mark_tile_dirty(offset);
                }
            }
        }


        /*************************************
         *
         *  Common video update
         *
         *************************************/

        u32 screen_update_galaxian(screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect)
        {
            /* draw the background layer (including stars) */
            m_draw_background_ptr(bitmap, cliprect);

            /* draw the tilemap characters over top */
            m_bg_tilemap.draw(screen, bitmap, cliprect, 0, 0);

            /* render the sprites next. Some custom pcbs (eg. zigzag, fantastc) have more than one sprite generator (ideally, this should be rendered in parallel) */
            for (int i = 0; i < m_numspritegens; i++)
                sprites_draw(bitmap, cliprect, new ListBytesPointer(m_spriteram.target, m_sprites_base + i * 0x20));

            /* if we have bullets to draw, render them following */
            if (m_draw_bullet_ptr != null)
                bullets_draw(bitmap, cliprect, new ListBytesPointer(m_spriteram.target, m_bullets_base));

            return 0;
        }


        /*************************************
         *
         *  Background tilemap
         *
         *************************************/

        //TILE_GET_INFO_MEMBER(galaxian_state::bg_get_tile_info)
        void bg_get_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            var videoram = m_videoram.target;  //uint8_t *videoram = m_videoram;
            byte x = (byte)(tile_index & 0x1f);

            UInt16 code = videoram[tile_index];
            byte attrib = m_spriteram[x*2+1];
            byte color = (byte)(attrib & 7);

            if (m_extend_tile_info_ptr != null)
                m_extend_tile_info_ptr(ref code, ref color, attrib, x);

            SET_TILE_INFO_MEMBER(ref tileinfo, 0, code, color, 0);
        }


        /*************************************
         *
         *  Sprite rendering
         *
         *************************************/

        void sprites_draw(bitmap_rgb32 bitmap, rectangle cliprect, ListBytesPointer spritebase)  //, uint8_t *spritebase)
        {
            rectangle clip = new rectangle(cliprect);
            int sprnum;

            /* the existence of +1 (sprite vs tile layer) is supported by a LOT of games */
            const int hoffset = 1;

            /* 16 of the 256 pixels of the sprites are hard-clipped at the line buffer */
            /* according to the schematics, it should be the first 16 pixels */
            clip.min_x = Math.Max(clip.min_x, ((m_flipscreen_x == 0) ? 1 : 0) * (16 + hoffset) * GALAXIAN_XSCALE);
            clip.max_x = Math.Min(clip.max_x, (256 - m_flipscreen_x * (16 + hoffset)) * GALAXIAN_XSCALE - 1);

            /* The line buffer is only written if it contains a '0' currently; */
            /* it is cleared during the visible area, and populated during HBLANK */
            /* To simulate this, we render backwards so that lower numbered sprites */
            /* have priority over higher numbered sprites. */
            for (sprnum = 7; sprnum >= 0; sprnum--)
            {
                ListBytesPointer base_ = new ListBytesPointer(spritebase, sprnum * 4);  //const uint8_t *base = &spritebase[sprnum * 4];
                /* Frogger: top and bottom 4 bits swapped entering the adder */
                byte base0 = m_frogger_adjust != 0 ? (byte)((base_[0] >> 4) | (base_[0] << 4)) : base_[0];
                /* the first three sprites match against y-1 */
                byte sy = (byte)(240 - (base0 - ((sprnum < 3) ? 1 : 0)));
                UInt16 code = (UInt16)(base_[1] & 0x3f);
                byte flipx = (byte)(base_[1] & 0x40);
                byte flipy = (byte)(base_[1] & 0x80);
                byte color = (byte)(base_[2] & 7);
                byte sx = (byte)(base_[3] + hoffset);

                /* extend the sprite information */
                if (m_extend_sprite_info_ptr != null)
                    m_extend_sprite_info_ptr(base_, ref sx, ref sy, ref flipx, ref flipy, ref code, ref color);

                /* apply flipscreen in X direction */
                if (m_flipscreen_x != 0)
                {
                    sx = (byte)(240 - sx);
                    flipx = flipx == 0 ? (byte)1 : (byte)0;
                }

                /* apply flipscreen in Y direction */
                if (m_flipscreen_y != 0)
                {
                    sy = (byte)(240 - sy);
                    flipy = flipy == 0 ? (byte)1 : (byte)0;
                }

                /* draw */

                        m_gfxdecode.target.digfx.gfx(1).transpen(bitmap,clip,
                        code, color,
                        flipx, flipy,
                        GALAXIAN_H0START + GALAXIAN_XSCALE * sx, sy, 0);
            }
        }


        /*************************************
         *
         *  Bullets rendering
         *
         *************************************/

        void bullets_draw(bitmap_rgb32 bitmap, rectangle cliprect, ListBytesPointer base_)  //, const uint8_t *base)
        {
            int y;

            /* iterate over scanlines */
            for (y = cliprect.min_y; y <= cliprect.max_y; y++)
            {
                byte shell = 0xff;
                byte missile = 0xff;
                byte effy;
                int which;

                /* the first 3 entries match Y-1 */
                effy = m_flipscreen_y != 0 ? (byte)((y - 1) ^ 255) : (byte)(y - 1);
                for (which = 0; which < 3; which++)
                    if ((byte)(base_[which*4+1] + effy) == 0xff)
                        shell = (byte)which;

                /* remaining entries match Y */
                effy = m_flipscreen_y != 0 ? (byte)(y ^ 255) : (byte)y;
                for (which = 3; which < 8; which++)
                {
                    if ((byte)(base_[which*4+1] + effy) == 0xff)
                    {
                        if (which != 7)
                            shell = (byte)which;
                        else
                            missile = (byte)which;
                    }
                }

                /* draw the shell */
                if (shell != 0xff)
                    m_draw_bullet_ptr(bitmap, cliprect, shell, 255 - base_[shell*4+3], y);
                if (missile != 0xff)
                    m_draw_bullet_ptr(bitmap, cliprect, missile, 255 - base_[missile*4+3], y);
            }
        }


        /*************************************
         *
         *  Screen orientation
         *
         *************************************/

        //WRITE8_MEMBER(galaxian_state::galaxian_flip_screen_x_w)
        void galaxian_flip_screen_x_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            if (m_flipscreen_x != (data & 0x01))
            {
                //      m_screen->update_now();
                m_screen.target.update_partial(m_screen.target.vpos());

                /* when the direction changes, we count a different number of clocks */
                /* per frame, so we need to reset the origin of the stars to the current */
                /* frame before we flip */
                stars_update_origin();

                m_flipscreen_x = (byte)(data & 0x01);
                m_bg_tilemap.set_flip((m_flipscreen_x != 0 ? TILEMAP_FLIPX : 0) | (m_flipscreen_y != 0 ? TILEMAP_FLIPY : 0));
            }
        }

        //WRITE8_MEMBER(galaxian_state::galaxian_flip_screen_y_w)
        void galaxian_flip_screen_y_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            if (m_flipscreen_y != (data & 0x01))
            {
                //      m_screen->update_now();
                m_screen.target.update_partial(m_screen.target.vpos());

                m_flipscreen_y = (byte)(data & 0x01);
                m_bg_tilemap.set_flip((m_flipscreen_x != 0 ? TILEMAP_FLIPX : 0) | (m_flipscreen_y != 0 ? TILEMAP_FLIPY : 0));
            }
        }

        //WRITE8_MEMBER(galaxian_state::galaxian_flip_screen_xy_w)
        void galaxian_flip_screen_xy_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            galaxian_flip_screen_x_w(space, offset, data);
            galaxian_flip_screen_y_w(space, offset, data);
        }


        /*************************************
         *
         *  Background controls
         *
         *************************************/

        //WRITE8_MEMBER(galaxian_state::galaxian_stars_enable_w)
        void galaxian_stars_enable_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            if (((m_stars_enabled ^ data) & 0x01) != 0)
            {
                //      m_screen->update_now();
                m_screen.target.update_partial(m_screen.target.vpos());
            }

            if (m_stars_enabled == 0 && (data & 0x01) != 0)
            {
                /* on the rising edge of this, the CLR on the shift registers is released */
                /* this resets the "origin" of this frame to 0 minus the number of clocks */
                /* we have counted so far */
                m_star_rng_origin = (UInt32)(STAR_RNG_PERIOD - (m_screen.target.vpos() * 512 + m_screen.target.hpos()));
                m_star_rng_origin_frame = (UInt32)m_screen.target.frame_number();
            }

            m_stars_enabled = (byte)(data & 0x01);
        }


        /*************************************
         *
         *  Star initialization
         *
         *************************************/

        void stars_init()
        {
            UInt32 shiftreg;
            int i;

            /* reset the blink and enabled states */
            m_stars_enabled = 0;  //false;
            m_stars_blink_state = 0;

            /* precalculate the RNG */
            m_stars = new byte [STAR_RNG_PERIOD];  //std::make_unique<uint8_t[]>(STAR_RNG_PERIOD);
            shiftreg = 0;
            for (i = 0; i < STAR_RNG_PERIOD; i++)
            {
                /* stars are enabled if the upper 8 bits are 1 and the low bit is 0 */
                int enabled = ((shiftreg & 0x1fe01) == 0x1fe00) ? 1 : 0;

                /* color comes from the 6 bits below the top 8 bits */
                int color = (int)((~shiftreg & 0x1f8) >> 3);

                /* store the color value in the low 6 bits and the enable in the upper bit */
                m_stars[i] = (byte)(color | (enabled << 7));

                /* the LFSR is fed based on the XOR of bit 12 and the inverse of bit 0 */
                shiftreg = (shiftreg >> 1) | ((((shiftreg >> 12) ^ ~shiftreg) & 1) << 16);
            }
        }


        /*************************************
         *
         *  Adjust the origin of stars
         *
         *************************************/

        void stars_update_origin()
        {
            int curframe = (int)m_screen.target.frame_number();

            /* only update on a different frame */
            if (curframe != m_star_rng_origin_frame)
            {
                /* The RNG period is 2^17-1; each frame, the shift register is clocked */
                /* 512*256 = 2^17 times. This means that we clock one extra time each */
                /* frame. However, if we are NOT flipped, there is a pair of D flip-flops */
                /* at 6B which delay the count so that we count 512*256-2 = 2^17-2 times. */
                /* In this case, we only one time less than the period each frame. Both */
                /* of these off-by-one countings produce the horizontal star scrolling. */
                int per_frame_delta = m_flipscreen_x != 0 ? 1 : -1;
                int total_delta = per_frame_delta * (curframe - (int)m_star_rng_origin_frame);

                /* we can't just use % here because mod of a negative number is undefined */
                while (total_delta < 0)
                    total_delta += STAR_RNG_PERIOD;

                /* now that everything is positive, do the mod */
                m_star_rng_origin = (m_star_rng_origin + (UInt32)total_delta) % STAR_RNG_PERIOD;
                m_star_rng_origin_frame = (UInt32)curframe;
            }
        }


        /*************************************
         *
         *  Draw a row of stars
         *
         *************************************/

        void stars_draw_row(bitmap_rgb32 bitmap, int maxx, int y, UInt32 star_offs, byte starmask)
        {
            int x;

            /* ensure our star offset is valid */
            star_offs %= STAR_RNG_PERIOD;

            /* iterate over the specified number of 6MHz pixels */
            for (x = 0; x < maxx; x++)
            {
                /* stars are suppressed unless V1 ^ H8 == 1 */
                int enable_star = (y ^ (x >> 3)) & 1;
                byte star;

                /*
                    The RNG clock is the master clock (18MHz) ANDed with the pixel clock (6MHz).
                    The divide-by-3 circuit that produces the pixel clock generates a square wave
                    with a 2/3 duty cycle, so the result of the AND generates a clock like this:
                                _   _   _   _   _   _   _   _
                      MASTER: _| |_| |_| |_| |_| |_| |_| |_| |
                                _______     _______     ______
                      PIXEL:  _|       |___|       |___|
                                _   _       _   _       _   _
                      RNG:    _| |_| |_____| |_| |_____| |_| |

                    Thus for each pixel, there are 3 master clocks and 2 RNG clocks, and the RNG
                    is clocked asymmetrically. To simulate this, we expand the horizontal screen
                    size by 3 and handle the first RNG clock with one pixel and the second RNG
                    clock with two pixels.
                */

                /* first RNG clock: one pixel */
                star = m_stars[star_offs++];
                if (star_offs >= STAR_RNG_PERIOD)
                    star_offs = 0;

                if (enable_star != 0 && (star & 0x80) != 0 && (star & starmask) != 0)
                {
                    bitmap.pix32(y, m_x_scale * x + 0)[0] = m_star_color[star & 0x3f];
                }

                /* second RNG clock: two pixels */
                star = m_stars[star_offs++];
                if (star_offs >= STAR_RNG_PERIOD)
                    star_offs = 0;

                if (enable_star != 0 && (star & 0x80) != 0 && (star & starmask) != 0)
                {
                    bitmap.pix32(y, m_x_scale * x + 1)[0] = m_star_color[star & 0x3f];
                    bitmap.pix32(y, m_x_scale * x + 2)[0] = m_star_color[star & 0x3f];
                }
            }
        }


        /*************************************
         *
         *  Background rendering
         *
         *************************************/

        void galaxian_draw_background(bitmap_rgb32 bitmap, rectangle cliprect)
        {
            /* erase the background to black first */
            bitmap.fill(rgb_t.black(), cliprect);

            /* update the star origin to the current frame */
            stars_update_origin();

            /* render stars if enabled */
            if (m_stars_enabled != 0)
            {
                int y;

                /* iterate over scanlines */
                for (y = cliprect.min_y; y <= cliprect.max_y; y++)
                {
                    UInt32 star_offs = m_star_rng_origin + (UInt32)y * 512;
                    stars_draw_row(bitmap, 256, y, star_offs, 0xff);
                }
            }
        }


        void background_draw_colorsplit(bitmap_rgb32 bitmap, rectangle cliprect, rgb_t color, int split, int split_flipped)
        {
            /* horizontal bgcolor split */
            if (m_flipscreen_x != 0)
            {
                rectangle draw = cliprect;
                draw.max_x = Math.Min(draw.max_x, split_flipped * GALAXIAN_XSCALE - 1);
                if (draw.min_x <= draw.max_x)
                    bitmap.fill(rgb_t.black(), draw);

                draw = cliprect;
                draw.min_x = Math.Max(draw.min_x, split_flipped * GALAXIAN_XSCALE);
                if (draw.min_x <= draw.max_x)
                    bitmap.fill(color, draw);
            }
            else
            {
                rectangle draw = cliprect;
                draw.max_x = Math.Min(draw.max_x, split * GALAXIAN_XSCALE - 1);
                if (draw.min_x <= draw.max_x)
                    bitmap.fill(color, draw);

                draw = cliprect;
                draw.min_x = Math.Max(draw.min_x, split * GALAXIAN_XSCALE);
                if (draw.min_x <= draw.max_x)
                    bitmap.fill(rgb_t.black(), draw);
            }
        }


        void frogger_draw_background(bitmap_rgb32 bitmap, rectangle cliprect)
        {
            /* according to schematics it is at 128+8; but it has been verified different on real machine.
            Video proof: http://www.youtube.com/watch?v=ssr69mQf224 */
            background_draw_colorsplit(bitmap, cliprect, new rgb_t(0,0,0x47), 128, 128);
        }


        /*************************************
         *
         *  Bullet rendering
         *
         *************************************/

        void galaxian_draw_pixel(bitmap_rgb32 bitmap, rectangle cliprect, int y, int x, rgb_t color)
        {
            if (y >= cliprect.min_y && y <= cliprect.max_y)
            {
                x *= GALAXIAN_XSCALE;
                x += GALAXIAN_H0START;
                if (x >= cliprect.min_x && x <= cliprect.max_x)
                {
                    bitmap.pix32(y, x)[0] = color;
                }

                x++;
                if (x >= cliprect.min_x && x <= cliprect.max_x)
                {
                    bitmap.pix32(y, x)[0] = color;
                }

                x++;
                if (x >= cliprect.min_x && x <= cliprect.max_x)
                {
                    bitmap.pix32(y, x)[0] = color;
                }
            }
        }


        void galaxian_draw_bullet(bitmap_rgb32 bitmap, rectangle cliprect, int offs, int x, int y)
        {
            /*
                Both "shells" and "missiles" begin displaying when the horizontal counter
                reaches $FC, and they stop displaying when it reaches $00, resulting in
                4-pixel-long shots. The first 7 entries are called "shells" and render as
                white; the final entry is called a "missile" and renders as yellow.
            */
            x -= 4;
            galaxian_draw_pixel(bitmap, cliprect, y, x++, m_bullet_color[offs]);
            galaxian_draw_pixel(bitmap, cliprect, y, x++, m_bullet_color[offs]);
            galaxian_draw_pixel(bitmap, cliprect, y, x++, m_bullet_color[offs]);
            galaxian_draw_pixel(bitmap, cliprect, y, x++, m_bullet_color[offs]);
        }


        /*************************************
         *
         *  Video extensions
         *
         *************************************/

#if false
        /*** generic ***/
        void galaxian_state::upper_extend_tile_info(uint16_t *code, uint8_t *color, uint8_t attrib, uint8_t x)
        {
            /* tiles are in the upper half of a larger ROM */
            *code += 0x100;
        }

        void galaxian_state::upper_extend_sprite_info(const uint8_t *base, uint8_t *sx, uint8_t *sy, uint8_t *flipx, uint8_t *flipy, uint16_t *code, uint8_t *color)
        {
            /* sprites are in the upper half of a larger ROM */
            *code += 0x40;
        }
#endif


        /*** Frogger ***/
        void frogger_extend_tile_info(ref uint16_t code, ref uint8_t color, uint8_t attrib, uint8_t x)
        {
            color = (byte)(((color >> 1) & 0x03) | ((color << 2) & 0x04));
        }


        void frogger_extend_sprite_info(ListBytesPointer basePtr, ref uint8_t sx, ref uint8_t sy, ref uint8_t flipx, ref uint8_t flipy, ref uint16_t code, ref uint8_t color)  //const uint8_t *base
        {
            color = (byte)(((color >> 1) & 0x03) | ((color << 2) & 0x04));
        }
    }
}
