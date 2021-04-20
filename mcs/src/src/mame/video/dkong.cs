// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using int8_t = System.SByte;
using offs_t = System.UInt32;
using pen_t = System.UInt32;
using s16 = System.Int16;
using tilemap_memory_index = System.UInt32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    partial class dkong_state : driver_device
    {
        const pen_t RADARSCP_BCK_COL_OFFSET         = 256;
        const pen_t RADARSCP_GRID_COL_OFFSET        = RADARSCP_BCK_COL_OFFSET + 256;
        const pen_t RADARSCP_STAR_COL               = RADARSCP_GRID_COL_OFFSET + 8;

        const double cd4049_vl = 1.5/5.0;
        const double cd4049_vh = 3.5/5.0;
        const double cd4049_al = 0.01;


        static readonly res_net_decode_info dkong_decode_info = new res_net_decode_info
        (
            2,      /*  there may be two proms needed to construct color */
            0,      /*  start at 0 */
            255,    /*  end at 255 */
            /*  R,   G,   B,   R,   G,   B */
            new u16 [] { 256, 256,   0,   0,   0,   0},        /*  offsets */
            new s16 [] {   1,  -2,   0,   0,   2,   0},        /*  shifts */
            new u16 [] {0x07,0x04,0x03,0x00,0x03,0x00}         /*  masks */
        );


        static readonly res_net_info dkong_net_info = new res_net_info
        (
            RES_NET_VCC_5V | RES_NET_VBIAS_5V | RES_NET_VIN_MB7052 |  RES_NET_MONITOR_SANYO_EZV20,
            new res_net_channel_info [] {
                new res_net_channel_info( RES_NET_AMP_DARLINGTON, 470, 0, 3, new double [] { 1000, 470, 220 } ),
                new res_net_channel_info( RES_NET_AMP_DARLINGTON, 470, 0, 3, new double [] { 1000, 470, 220 } ),
                new res_net_channel_info( RES_NET_AMP_EMITTER,    680, 0, 2, new double [] {  470, 220,   0 } )  /*  dkong */
            }
        );


        static readonly res_net_info dkong_net_bck_info = new res_net_info
        (
            RES_NET_VCC_5V | RES_NET_VBIAS_5V | RES_NET_VIN_MB7052 |  RES_NET_MONITOR_SANYO_EZV20,
            new res_net_channel_info [] {
                new res_net_channel_info( RES_NET_AMP_DARLINGTON, 470, 0, 0, new double [] { 0 } ),
                new res_net_channel_info( RES_NET_AMP_DARLINGTON, 470, 0, 0, new double [] { 0 } ),
                new res_net_channel_info( RES_NET_AMP_EMITTER,    680, 0, 0, new double [] { 0 } )
            }
        );


        const double TRS_J1  = 1;         // (1) = Closed (0) = Open


        static readonly res_net_info radarscp_net_info = new res_net_info
        (
            RES_NET_VCC_5V | RES_NET_VBIAS_TTL | RES_NET_VIN_MB7052 |  RES_NET_MONITOR_SANYO_EZV20,
            new res_net_channel_info [] {
                new res_net_channel_info( RES_NET_AMP_DARLINGTON, 470 * TRS_J1, 470*(1-TRS_J1), 3, new double [] { 1000, 470, 220 } ),
                new res_net_channel_info( RES_NET_AMP_DARLINGTON, 470 * TRS_J1, 470*(1-TRS_J1), 3, new double [] { 1000, 470, 220 } ),
                new res_net_channel_info( RES_NET_AMP_EMITTER,    680 * TRS_J1, 680*(1-TRS_J1), 2, new double [] {  470, 220,   0 } )    /*  radarscp */
            }
        );


        static readonly res_net_info radarscp_net_bck_info = new res_net_info
        (
            RES_NET_VCC_5V | RES_NET_VBIAS_TTL | RES_NET_VIN_MB7052 |  RES_NET_MONITOR_SANYO_EZV20,
            new res_net_channel_info [] {
                new res_net_channel_info( RES_NET_AMP_DARLINGTON, 470, 4700, 0, new double [] { 0 } ),
                new res_net_channel_info( RES_NET_AMP_DARLINGTON, 470, 4700, 0, new double [] { 0 } ),
                new res_net_channel_info( RES_NET_AMP_EMITTER,    470, 4700, 0, new double [] { 0 } )    /*  radarscp */
            }
        );


        /* Radarscp star color */
        static readonly res_net_info radarscp_stars_net_info = new res_net_info
        (
            RES_NET_VCC_5V | RES_NET_VBIAS_5V | RES_NET_VIN_TTL_OUT | RES_NET_MONITOR_SANYO_EZV20,
            new res_net_channel_info [] {
                new res_net_channel_info( RES_NET_AMP_DARLINGTON, 4700, 470, 0, new double [] { 0 } ),
                new res_net_channel_info( RES_NET_AMP_DARLINGTON,    1,   0, 0, new double [] { 0 } ),    /*  dummy */
                new res_net_channel_info( RES_NET_AMP_EMITTER,       1,   0, 0, new double [] { 0 } ),    /*  dummy */
            }
        );


        /* Dummy struct to generate background palette entries */
        static readonly res_net_info radarscp_blue_net_info = new res_net_info
        (
            RES_NET_VCC_5V | RES_NET_VBIAS_5V | RES_NET_VIN_VCC | RES_NET_MONITOR_SANYO_EZV20,
            new res_net_channel_info [] {
                new res_net_channel_info( RES_NET_AMP_DARLINGTON,  470, 4700, 0, new double [] { 0 } ),   /*  bias/gnd exist in schematics, readable in TKG3 schematics */
                new res_net_channel_info( RES_NET_AMP_DARLINGTON,  470, 4700, 0, new double [] { 0 } ),   /*  bias/gnd exist in schematics, readable in TKG3 schematics */
                new res_net_channel_info( RES_NET_AMP_EMITTER,       0,    0, 8, new double [] { 128,64,32,16,8,4,2,1 } ),    /*  dummy */
            }
        );


        /* Dummy struct to generate grid palette entries */
        static readonly res_net_info radarscp_grid_net_info = new res_net_info
        (
            RES_NET_VCC_5V | RES_NET_VBIAS_5V | RES_NET_VIN_TTL_OUT | RES_NET_MONITOR_SANYO_EZV20,
            new res_net_channel_info [] {
                new res_net_channel_info( RES_NET_AMP_DARLINGTON,    0,   0, 1, new double [] { 1 } ),    /*  dummy */
                new res_net_channel_info( RES_NET_AMP_DARLINGTON,    0,   0, 1, new double [] { 1 } ),    /*  dummy */
                new res_net_channel_info( RES_NET_AMP_EMITTER,       0,   0, 1, new double [] { 1 } ),    /*  dummy */
            }
        );


        void dkong2b_palette(palette_device palette)
        {
            Pointer<uint8_t> color_prom = new Pointer<uint8_t>(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();

            std.vector<rgb_t> rgb;
            compute_res_net_all(out rgb, color_prom, dkong_decode_info, dkong_net_info);
            palette.dipalette.set_pen_colors(0, rgb);

            // Now treat tri-state black background generation

            for (int i = 0; i < 256; i++)
            {
                if ((i & 0x03) == 0x00)  // NOR => CS=1 => Tristate => real black
                {
                    int r = compute_res_net(1, 0, dkong_net_bck_info);
                    int g = compute_res_net(1, 1, dkong_net_bck_info);
                    int b = compute_res_net(1, 2, dkong_net_bck_info);
                    palette.dipalette.set_pen_color((pen_t)i, (u8)r, (u8)g, (u8)b);
                }
            }

            palette.dipalette.palette().normalize_range(0, 255);

            color_prom += 512;
            // color_prom now points to the beginning of the character color codes
            m_color_codes = color_prom; // we'll need it later
        }


        void radarscp_palette(palette_device palette)
        {
            Pointer<uint8_t> color_prom = new Pointer<uint8_t>(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();

            for (int i = 0; i < 256; i++)
            {
                // red component
                int r = compute_res_net((color_prom[256] >> 1) & 0x07, 0, radarscp_net_info);
                // green component
                int g = compute_res_net(((color_prom[256] << 2) & 0x04) | ((color_prom[0] >> 2) & 0x03), 1, radarscp_net_info);
                // blue component
                int b = compute_res_net((color_prom[0] >> 0) & 0x03, 2, radarscp_net_info);

                palette.dipalette.set_pen_color((pen_t)i, (u8)r, (u8)g, (u8)b);
                color_prom++;
            }

            // Now treat tri-state black background generation

            for (int i = 0; i < 256; i++)
            {
                if ((m_vidhw != DKONG_RADARSCP_CONVERSION) && ((i & 0x03) == 0x00))  //  NOR => CS=1 => Tristate => real black
                {
                    int r = compute_res_net( 1, 0, radarscp_net_bck_info );
                    int g = compute_res_net( 1, 1, radarscp_net_bck_info );
                    int b = compute_res_net( 1, 2, radarscp_net_bck_info );
                    palette.dipalette.set_pen_color((pen_t)i, (u8)r, (u8)g, (u8)b);
                }
            }

            // Star color
            palette.dipalette.set_pen_color(RADARSCP_STAR_COL,
                    (u8)compute_res_net(1, 0, radarscp_stars_net_info),
                    (u8)compute_res_net(0, 1, radarscp_stars_net_info),
                    (u8)compute_res_net(0, 2, radarscp_stars_net_info));

            // Oscillating background
            for (int i = 0; i < 256; i++)
            {
                int r = compute_res_net( 0, 0, radarscp_blue_net_info );
                int g = compute_res_net( 0, 1, radarscp_blue_net_info );
                int b = compute_res_net( i, 2, radarscp_blue_net_info );

                palette.dipalette.set_pen_color(RADARSCP_BCK_COL_OFFSET + (pen_t)i, (u8)r, (u8)g, (u8)b);
            }

            // Grid
            for (int i = 0; i < 8; i++)
            {
                int r = compute_res_net( BIT(i, 0), 0, radarscp_grid_net_info );
                int g = compute_res_net( BIT(i, 1), 1, radarscp_grid_net_info );
                int b = compute_res_net( BIT(i, 2), 2, radarscp_grid_net_info );

                palette.dipalette.set_pen_color(RADARSCP_GRID_COL_OFFSET + (pen_t)i, (u8)r, (u8)g, (u8)b);
            }

            palette.dipalette.palette().normalize_range(0, RADARSCP_GRID_COL_OFFSET + 7);

            color_prom += 256;
            // color_prom now points to the beginning of the character color codes
            m_color_codes = color_prom; // we'll need it later
        }


        //TILE_GET_INFO_MEMBER(dkong_state::dkong_bg_tile_info)
        void dkong_bg_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int code = m_video_ram.op[tile_index] + 256 * m_gfx_bank;
            int color = (m_color_codes[tile_index % 32 + 32 * (tile_index / 32 / 4)] & 0x0f) + 0x10 * m_palette_bank;

            tileinfo.set(0, (u32)code, (u32)color, 0);
        }


        //TILE_GET_INFO_MEMBER(dkong_state::radarscp1_bg_tile_info)
        void radarscp1_bg_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            int code = m_video_ram.op[tile_index] + 256 * m_gfx_bank;
            int color = (m_color_codes[tile_index % 32] & 0x0f);
            color = color | (m_palette_bank<<4);

            tileinfo.set(0, (u32)code, (u32)color, 0);
        }


        /***************************************************************************

          I/O Handling

        ***************************************************************************/

        void dkong_videoram_w(offs_t offset, uint8_t data)
        {
            if (m_video_ram.op[offset] != data)
            {
                m_video_ram[offset].op = data;
                m_bg_tilemap.mark_tile_dirty(offset);
            }
        }


        //void dkong_state::dkongjr_gfxbank_w(uint8_t data)
        //void dkong_state::dkong3_gfxbank_w(uint8_t data)


        void dkong_palettebank_w(offs_t offset, uint8_t data)
        {
            int newbank;

            newbank = m_palette_bank;

            if ((data & 1) != 0)
                newbank |= 1 << (int)offset;
            else
                newbank &= ~(1 << (int)offset);

            if (m_palette_bank != newbank)
            {
                m_palette_bank = (uint8_t)newbank;
                m_bg_tilemap.mark_all_dirty();
            }
        }


        void radarscp_grid_enable_w(uint8_t data)
        {
            throw new emu_unimplemented();
        }


        void radarscp_grid_color_w(uint8_t data)
        {
            throw new emu_unimplemented();
        }


        void dkong_flipscreen_w(uint8_t data)
        {
            m_flip = (uint8_t)(data & 0x01);
        }


        void dkong_spritebank_w(uint8_t data)
        {
            m_sprite_bank = (uint8_t)(data & 0x01);
        }


        /***************************************************************************
          Draw the game screen in the given bitmap_ind16.
        ***************************************************************************/
        void draw_sprites(bitmap_ind16 bitmap, rectangle cliprect, uint32_t mask_bank, uint32_t shift_bits)
        {
            int offs;
            int scanline_vf;    /* buffering scanline including flip */
            int scanline_vfc;   /* line buffering scanline including flip - this is the cached scanline_vf */
            int scanline;       /* current scanline */
            int add_y;
            int add_x;
            int num_sprt;

            /* Draw the sprites. There are two pecularities which have been mentioned by
             * a Donkey Kong II author at CAX 2008:
             * 1) On real hardware, sprites wrap around from the right to the left instead
             *    of clipping.
             * 2) On real hardware, there is a limit of 16 sprites per scanline.
             *    Sprites after the 16th (starting from the left) simply don't show.
             *
             * 2) is in line with the real hardware which buffers the sprite data
             * for one scanline. The ram is 64x9 and a sprite takes 4 bytes.
             * ==> 16 sprites per scanline.
             *
             * TODO: 9th bit is not understood right now.
             *
             * 1) is due to limitation of signals to 8 bit.
             *
             * This is quite different from galaxian. The dkong hardware updates sprites
             * only once every frame by dma. The number of sprites can not be processed
             * directly, Thus the preselection. The buffering takes place during the
             * active phase of the video signal. The scanline is than rendered into the linebuffer
             * during HBLANK.
             *
             * A sprite will be drawn:
             * a) FlipQ = 1 : (sprite_y + 0xF9 + scanline) & 0xF0 == 0xF0
             * b) FlipQ = 0 : (sprite_y + 0xF7 + (scanline ^ 0xFF)) & 0xF0 == 0xF0
             *
             * FlipQ = 1 ("Normal Play"):
             *
             * sprite_y = 0x20
             *
             * scanline
             * 0x10, 0xEF, 0x208, 0x00
             * 0x18, 0xE7, 0x200, 0x00
             * 0x19, 0xE6, 0x1FF, 0xF0
             * 0x20, 0xDF, 0x1F8, 0xF0
             *
             */

            scanline_vf = (cliprect.max_y - 1) & 0xFF;
            scanline_vfc = (cliprect.max_y - 1) & 0xFF;
            scanline = cliprect.max_y & 0xFF;

            if (m_flip != 0)
            {
                scanline_vf ^= 0xFF;
                scanline_vfc ^= 0xFF;
                add_y = 0xF7;
                add_x = 0xF7;
            }
            else
            {
                add_y = 0xF9;
                add_x = 0xF7;
            }

            for (offs = m_sprite_bank<<9, num_sprt=0; (num_sprt < 16) && (offs < (m_sprite_bank<<9) + 0x200) /* sprite_ram_size */; offs += 4)
            {
                int y = m_sprite_ram.op[offs];
                int do_draw = (((y + add_y + 1 + scanline_vf) & 0xF0) == 0xF0) ? 1 : 0;

                if (do_draw != 0)
                {
                    /* sprite_ram[offs + 2] & 0x40 is used by Donkey Kong 3 only */
                    /* sprite_ram[offs + 2] & 0x30 don't seem to be used (they are */
                    /* probably not part of the color code, since Mario Bros, which */
                    /* has similar hardware, uses a memory mapped port to change */
                    /* palette bank, so it's limited to 16 color codes) */

                    int code = (int)((m_sprite_ram.op[offs + 1] & 0x7f) + ((m_sprite_ram.op[offs + 2] & mask_bank) << (int)shift_bits));
                    int color = (m_sprite_ram.op[offs + 2] & 0x0f) + 16 * m_palette_bank;
                    int flipx = m_sprite_ram.op[offs + 2] & 0x80;
                    int flipy = m_sprite_ram.op[offs + 1] & 0x80;

                    /* On the real board, the x and y are read inverted after the first
                     * buffer stage. This due to the fact that the 82S09 delivers complements
                     * of stored data on read!
                     */

                    int x = (m_sprite_ram.op[offs + 3] + add_x + 1) & 0xFF;
                    if (m_flip != 0)
                    {
                        x = (x ^ 0xFF) - 15;
                        flipx = (flipx == 0) ? 1 : 0;
                    }
                    y = scanline - ((y + add_y + 1 + scanline_vfc) & 0x0F);

                    m_gfxdecode.op[0].digfx.gfx(1).transpen(bitmap, cliprect, (u32)code, (u32)color, flipx, flipy, x, y, 0);

                    // wraparound
                    m_gfxdecode.op[0].digfx.gfx(1).transpen(bitmap, cliprect, (u32)code, (u32)color, flipx, flipy, m_flip != 0 ? x + 256 : x - 256, y, 0);
                    m_gfxdecode.op[0].digfx.gfx(1).transpen(bitmap, cliprect, (u32)code, (u32)color, flipx, flipy, x, y - 256, 0);

                    num_sprt++;
                }
            }
        }


        void radarscp_draw_background(bitmap_ind16 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
        }


        void radarscp_scanline(int scanline)
        {
            throw new emu_unimplemented();
        }


        //TIMER_CALLBACK_MEMBER(dkong_state::scanline_callback)
        void scanline_callback(object ptr, int param)
        {
            int scanline = param;

            if ((m_hardware_type == HARDWARE_TRS02) || (m_hardware_type == HARDWARE_TRS01))
                radarscp_scanline(scanline);

            /* update any video up to the current scanline */
            //m_screen->update_now();
            m_screen.op[0].update_partial(m_screen.op[0].vpos());

            scanline = (scanline + 1) % VTOTAL;
            /* come back at the next appropriate scanline */
            m_scanline_timer.adjust(m_screen.op[0].time_until_pos(scanline), scanline);
        }


        void check_palette()
        {
            ioport_port port;
            int newset;

            port = ioport("VIDHW");
            if (port != null)
            {
                newset = (int)port.read();
                if (newset != m_vidhw)
                {
                    m_vidhw = (int8_t)newset;
                    switch (newset)
                    {
                        case DKONG_RADARSCP_CONVERSION:
                            radarscp_palette(m_palette.op[0]);
                            break;
                        case DKONG_BOARD:
                            dkong2b_palette(m_palette.op[0]);
                            break;
                    }
                }
            }
        }


        //VIDEO_START_MEMBER(dkong_state,dkong_base)
        void video_start_dkong_base()
        {
            m_cd4049_b = (Math.Log(0.0 - Math.Log(cd4049_al)) - Math.Log(0.0 - Math.Log((1.0-cd4049_al))) ) / Math.Log(cd4049_vh/cd4049_vl);
            m_cd4049_a = Math.Log(0.0 - Math.Log(cd4049_al)) - m_cd4049_b * Math.Log(cd4049_vh);

            m_gfx_bank = 0;
            m_palette_bank = 0;
            m_sprite_bank = 0;
            m_vidhw = -1;

            save_item(NAME(new { m_vidhw }));
            save_item(NAME(new { m_gfx_bank }));
            save_item(NAME(new { m_palette_bank }));
            save_item(NAME(new { m_sprite_bank }));
            save_item(NAME(new { m_grid_on }));

            save_item(NAME(new { m_grid_col }));
            save_item(NAME(new { m_flip }));

            // TRS01 TRS02
            save_item(NAME(new { m_sig30Hz }));
            save_item(NAME(new { m_blue_level }));
            save_item(NAME(new { m_cv1 }));
            save_item(NAME(new { m_cv2 }));
            save_item(NAME(new { m_vg1 }));
            save_item(NAME(new { m_vg2 }));
            save_item(NAME(new { m_vg3 }));
            save_item(NAME(new { m_cv3 }));
            save_item(NAME(new { m_cv4 }));

            save_item(NAME(new { m_lfsr_5I }));
            save_item(NAME(new { m_grid_sig }));
            save_item(NAME(new { m_rflip_sig }));
            save_item(NAME(new { m_star_ff }));
            save_item(NAME(new { m_counter }));
            save_item(NAME(new { m_pixelcnt }));
            save_item(NAME(new { m_bg_bits }));
        }


        //VIDEO_START_MEMBER(dkong_state,dkong)
        void video_start_dkong()
        {
            //VIDEO_START_CALL_MEMBER(dkong_base);
            video_start_dkong_base();

            m_scanline_timer = machine().scheduler().timer_alloc(scanline_callback);
            m_scanline_timer.adjust(m_screen.op[0].time_until_pos(0));

            switch (m_hardware_type)
            {
                case HARDWARE_TRS02:
                    m_screen.op[0].register_screen_bitmap(m_bg_bits);
                    m_gfx3 = memregion("gfx3").base_();
                    m_gfx3_len = (int)memregion("gfx3").bytes();
                    goto case HARDWARE_TKG04;  //[[fallthrough]];
                case HARDWARE_TKG04:
                case HARDWARE_TKG02:
                    m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op[0].digfx, dkong_bg_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS,  8, 8, 32, 32);
                    break;
                case HARDWARE_TRS01:
                    m_bg_tilemap = machine().tilemap().create(m_gfxdecode.op[0].digfx, radarscp1_bg_tile_info, tilemap_standard_mapper.TILEMAP_SCAN_ROWS,  8, 8, 32, 32);

                    m_screen.op[0].register_screen_bitmap(m_bg_bits);
                    m_gfx4 = memregion("gfx4").base_();
                    m_gfx3 = memregion("gfx3").base_();
                    m_gfx3_len = (int)memregion("gfx3").bytes();

                    break;
                default:
                    fatalerror("Invalid hardware type in dkong_video_start\n");
                    break;
            }
        }


        u32 screen_update_dkong(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            machine().tilemap().set_flip_all(m_flip != 0 ? TILEMAP_FLIPX | TILEMAP_FLIPY : 0);

            switch (m_hardware_type)
            {
                case HARDWARE_TKG02:
                case HARDWARE_TKG04:
                    check_palette();
                    m_bg_tilemap.draw(screen, bitmap, cliprect, 0, 0);
                    draw_sprites(bitmap, cliprect, 0x40, 1);
                    break;
                case HARDWARE_TRS01:
                case HARDWARE_TRS02:
                    m_bg_tilemap.draw(screen, bitmap, cliprect, 0, 0);
                    draw_sprites(bitmap, cliprect, 0x40, 1);
                    radarscp_draw_background(bitmap, cliprect);
                    break;
                default:
                    fatalerror("Invalid hardware type in dkong_video_update\n");
                    break;
            }
            return 0;
        }
    }
}
