// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint32_t = System.UInt32;


namespace mame
{
#if false
    const res_net_decode_info tnx1_state::mb7051_decode_info =
    {
        1,      /*  one prom 5 lines */
        0,      /*  start at 0 */
        15,     /*  end at 15 (banked) */
        /*  R,   G,   B,  */
        {   0,   0,   0 },      /*  offsets */
        {   0,   3,   6 },      /*  shifts */
        {0x07,0x07,0x03 }       /*  masks */
    };

    const res_net_decode_info tnx1_state::mb7052_decode_info =
    {
        1,      /*  two 4 bit proms */
        0,      /*  start at 0 */
        31,     /*  end at 31 (banked) */
        /*  R,   G,   B */
        {   0,   0,   0},       /*  offsets */
        {   0,   3,   6},       /*  shifts */
        {0x07,0x07,0x03}        /*  masks */
    };

    const res_net_info tnx1_state::txt_mb7051_net_info =
    {
        RES_NET_VCC_5V | RES_NET_VBIAS_5V | RES_NET_VIN_MB7051 | RES_NET_MONITOR_SANYO_EZV20,
        {
            { RES_NET_AMP_DARLINGTON, 470, 0, 3, { 1000, 470, 220 } },
            { RES_NET_AMP_DARLINGTON, 470, 0, 3, { 1000, 470, 220 } },
            { RES_NET_AMP_DARLINGTON, 680, 0, 2, {  470, 220,   0 } }
        }
    };

    const res_net_info tnx1_state::tnx1_bak_mb7051_net_info =
    {
        RES_NET_VCC_5V | RES_NET_VBIAS_5V | RES_NET_VIN_MB7051 | RES_NET_MONITOR_SANYO_EZV20,
        {
            { RES_NET_AMP_DARLINGTON, 470, 0, 3, { 1200, 680, 470 } },
            { RES_NET_AMP_DARLINGTON, 470, 0, 3, { 1200, 680, 470 } },
            { RES_NET_AMP_DARLINGTON, 680, 0, 2, {  680, 470,   0 } }
        }
    };

    const res_net_info tpp1_state::tpp1_bak_mb7051_net_info =
    {
        RES_NET_VCC_5V | RES_NET_VBIAS_5V | RES_NET_VIN_MB7051 | RES_NET_MONITOR_SANYO_EZV20,
        {
            { RES_NET_AMP_DARLINGTON, 470, 0, 3, { 1200, 680, 470 } },
            { RES_NET_AMP_DARLINGTON, 470, 0, 3, { 1200, 680, 470 } },
            { RES_NET_AMP_DARLINGTON, 680, 0, 2, {  680, 470,   0 } }
        }
    };

    const res_net_info tnx1_state::obj_mb7052_net_info =
    {
        RES_NET_VCC_5V | RES_NET_VBIAS_5V | RES_NET_VIN_MB7052 |  RES_NET_MONITOR_SANYO_EZV20,
        {
            { RES_NET_AMP_DARLINGTON, 470, 0, 3, { 1000, 470, 220 } },
            { RES_NET_AMP_DARLINGTON, 470, 0, 3, { 1000, 470, 220 } },
            { RES_NET_AMP_DARLINGTON, 680, 0, 2, {  470, 220,   0 } }
        }
    };


    void tpp1_state::tnx1_palette(palette_device &palette)
    {
        // Two of the PROM address pins are tied together
        for (int i = 0; i < 0x20; i++)
        {
            int const color = (i & 0xf) | ((i & 0x8) << 1);
            m_color_prom[i + 0x20] = m_color_prom[color + 0x20];
        }

        m_palette_bank_cache = -1;

        update_palette();
    }

    void tnx1_state::tnx1_palette(palette_device &palette)
    {
        // Two of the PROM address pins are tied together and one is not connected...
        for (int i = 0;i < 0x100;i++)
        {
            int const color = (i & 0x3f) | ((i & 0x20) << 1);
            m_color_prom_spr[i] = m_color_prom_spr[color];
        }

        m_palette_bank_cache = -1;

        update_palette();
    }

    void tnx1_state::update_palette()
    {
        if ((m_palette_bank ^ m_palette_bank_cache) & 0x08)
        {
            uint8_t *color_prom = m_color_prom + 16 * ((m_palette_bank & 0x08) >> 3);

            std::vector<rgb_t> rgb;

            compute_res_net_all(rgb, color_prom, mb7051_decode_info, bak_mb7051_net_info());
            m_palette->set_pen_colors(0, rgb);
        }

        if ((m_palette_bank ^ m_palette_bank_cache) & 0x08)
        {
            uint8_t *color_prom = m_color_prom + 32 + 16 * ((m_palette_bank & 0x08) >> 3);

            /* characters */
            for (int i = 0; i < 16; i++)
            {
                int r = compute_res_net((color_prom[i] >> 0) & 0x07, 0, txt_mb7051_net_info);
                int g = compute_res_net((color_prom[i] >> 3) & 0x07, 1, txt_mb7051_net_info);
                int b = compute_res_net((color_prom[i] >> 6) & 0x03, 2, txt_mb7051_net_info);
                m_palette->set_pen_color(16 + (2 * i) + 0, rgb_t(0, 0, 0));
                m_palette->set_pen_color(16 + (2 * i) + 1, rgb_t(r, g, b));
            }
        }

        if ((m_palette_bank ^ m_palette_bank_cache) & 0x07)
        {
            uint8_t *color_prom = m_color_prom_spr + 32 * (m_palette_bank & 0x07);

            /* sprites */
            std::vector<rgb_t> rgb;
            compute_res_net_all(rgb, color_prom, mb7052_decode_info, obj_mb7052_net_info);
            m_palette->set_pen_colors(48, rgb);
        }

        m_palette_bank_cache = m_palette_bank;
    }

    void tnx1_state::background_w(offs_t offset, uint8_t data)
    {
        // TODO: confirm the memory layout
        bool lsn = (data & 0x80) == 0;
        if (lsn)
        {
            m_background_ram[offset] = (m_background_ram[offset] & 0xf0) | (data & 0xf);
        }
        else
        {
            m_background_ram[offset] = (m_background_ram[offset] & 0x0f) | (data << 4);
        }
    }

    void tpp2_state::background_w(offs_t offset, uint8_t data)
    {
        // TODO: confirm the memory layout
        bool lsn = (offset & 0x40) == 0;
        offset = (offset & 0x3f) | ((offset & ~0x7f) >> 1);
        if (lsn)
        {
            m_background_ram[offset] = (m_background_ram[offset] & 0xf0) | (data & 0xf);
        }
        else
        {
            m_background_ram[offset] = (m_background_ram[offset] & 0x0f) | (data << 4);
        }
    }

    void tnx1_state::popeye_videoram_w(offs_t offset, uint8_t data)
    {
        m_videoram[offset] = data;
        m_fg_tilemap->mark_tile_dirty(offset);
    }

    void tnx1_state::popeye_colorram_w(offs_t offset, uint8_t data)
    {
        m_colorram[offset] = data;
        m_fg_tilemap->mark_tile_dirty(offset);
    }

    TILE_GET_INFO_MEMBER(tnx1_state::get_fg_tile_info)
    {
        int code = m_videoram[tile_index];
        int color = m_colorram[tile_index] & 0x0f;

        tileinfo.set(0, code, color, 0);
    }

    void tnx1_state::video_start()
    {
        m_background_ram.resize(0x1000);
        m_sprite_ram.resize(0x400);

        m_sprite_bitmap = std::make_unique<bitmap_ind16>(512, 512);

        m_fg_tilemap = &machine().tilemap().create(*m_gfxdecode, tilemap_get_info_delegate(*this, FUNC(tnx1_state::get_fg_tile_info)), TILEMAP_SCAN_ROWS, 16, 16, 32, 32);
        m_fg_tilemap->set_transparent_pen(0);

        m_bitmap[0].resize(512, 512);
        m_bitmap[1].resize(512, 512);

        m_field = 0;

        save_item(NAME(m_field));
        save_item(NAME(m_palette_bank));
        save_item(NAME(m_palette_bank_cache));
        save_item(NAME(m_background_ram));
        save_item(NAME(m_background_scroll));
        save_item(NAME(m_sprite_ram));
    }

    void tnx1_state::draw_sprites(bitmap_ind16 &bitmap, const rectangle &cliprect)
    {
        m_sprite_bitmap->fill(0, cliprect);

        for (int y = cliprect.min_y; y <= cliprect.max_y; y++)
        {
            struct attribute_memory
            {
                int row = 0;
                int sx = 0;
                uint8_t color = 0;
                uint16_t code = 0;
                int flipx = 0;
                int flipy = 0;
            } attributes[64];

            for (int offs = 4; offs < m_dmasource.bytes(); offs += 4)
            {
                int sy = 0x200 - (m_sprite_ram[offs + 1] * 2);
                int row = y - sy;
                if (flip_screen())
                {
                    sy ^= 0x1ff;
                    row = sy - y;
                }

                if (row >= 0 && row < 16)
                {
                    /*
                    * offs+3:
                    * bit 7 ? TODO: figure out why olive oil and wimpy have some of these bits set
                    * bit 6 ?
                    * bit 5 ?
                    * bit 4 MSB of sprite code
                    * bit 3 vertical flip
                    * bit 2 sprite bank
                    * bit 1 \ color (with bit 2 as well)
                    * bit 0 /
                    */

                    struct attribute_memory *a = &attributes[m_sprite_ram[offs] >> 2];
                    a->sx = m_sprite_ram[offs] * 2;
                    a->row = row;
                    a->code = ((m_sprite_ram[offs + 2] & 0x7f)
                        + ((m_sprite_ram[offs + 3] & 0x10) << 3)
                        + ((m_sprite_ram[offs + 3] & 0x04) << 6)) ^ 0x1ff;
                    a->color = (m_sprite_ram[offs + 3] & 0x07);
                    a->flipx = (m_sprite_ram[offs + 2] & 0x80) ? 0xf : 0;
                    a->flipy = (m_sprite_ram[offs + 3] & 0x08) ? 0xf : 0;
                }
            }

            int flipx = 0;
            for (int i = 0; i < 64; i++)
            {
                struct attribute_memory *a = &attributes[i];
                if (a->color != 0)
                {
                    gfx_element *gfx = m_gfxdecode->gfx(1);
                    const pen_t *pal = &m_palette->pen(gfx->colorbase() + gfx->granularity() * (a->color % gfx->colors()));
                    const uint8_t *source_base = gfx->get_data(a->code % gfx->elements());
                    const uint8_t *source = source_base + (a->row ^ a->flipy) * gfx->rowbytes();

                    if (bootleg_sprites() && flipx != a->flipx)
                    {
                        int px = a->sx - 7;
                        if (px >= 0 && px < 512)
                        {
                            if (flip_screen())
                                px ^= 0x1ff;

                            m_sprite_bitmap->pix(y, px) = 0;
                        }

                        flipx = a->flipx;
                    }

                    for (int x = 0; x < 16; x++)
                    {
                        int px = a->sx + x - 6;
                        if (px >= 0 && px < 512)
                        {
                            if (flip_screen())
                                px ^= 0x1ff;

                            uint16_t p = source[x ^ a->flipx];
                            if (p) p = pal[p];

                            m_sprite_bitmap->pix(y, px) = p;
                        }
                    }
                }
            }
        }

        copybitmap_trans(bitmap, *m_sprite_bitmap, 0, 0, 0, 0, cliprect, 0);
    }

    void tnx1_state::draw_field(bitmap_ind16 &bitmap, const rectangle &cliprect)
    {
        int x;
        int y;

        for (y=(cliprect.min_y & ~1) + m_field; y<=cliprect.max_y; y += 2)
            for (x=cliprect.min_x; x<=cliprect.max_x; x++)
                bitmap.pix(y, x) = 0;
    }

    void tnx1_state::draw_background(bitmap_ind16 &bitmap, const rectangle &cliprect)
    {
        for (int y = cliprect.min_y; y <= cliprect.max_y; y++)
        {
            int sy = y;
            if (flip_screen())
                sy ^= 0x1ff;

            sy -= 0x200 - (2 * m_background_scroll[1]);

            for (int x = cliprect.min_x; x <= cliprect.max_x; x++)
            {
                if (sy < 0)
                    bitmap.pix(y, x) = m_background_ram[0] & 0xf; // TODO: find out exactly where the data is fetched from
                else
                {
                    // TODO: confirm the memory layout
                    int sx = x + (2 * (m_background_scroll[0] | ((m_background_scroll[2] & 1) << 8))) + 0x70;
                    int shift = (sx & 0x200) / 0x80;

                    bitmap.pix(y, x) = (m_background_ram[((sx / 8) & 0x3f) + ((sy / 8) * 0x40)] >> shift) & 0xf;
                }
            }
        }
    }

    void tpp1_state::draw_background(bitmap_ind16 &bitmap, const rectangle &cliprect)
    {
        for (int y = cliprect.min_y; y <= cliprect.max_y; y++)
        {
            int sy = y;
            if (flip_screen())
                sy ^= 0x1ff;

            sy -= 0x200 - (2 * m_background_scroll[1]);

            for (int x = cliprect.min_x; x <= cliprect.max_x; x++)
            {
                if (sy < 0)
                    bitmap.pix(y, x) = m_background_ram[0] & 0xf; // TODO: find out exactly where the data is fetched from
                else
                {
                    // TODO: confirm the memory layout
                    int sx = x + (2 * m_background_scroll[0]) + 0x70;
                    int shift = (sy & 4);

                    bitmap.pix(y, x) = (m_background_ram[((sx / 8) & 0x3f) + ((sy / 8) * 0x40)] >> shift) & 0xf;
                }
            }
        }
    }

    void tpp2_state::draw_background(bitmap_ind16 &bitmap, const rectangle &cliprect)
    {
        for (int y = cliprect.min_y; y <= cliprect.max_y; y++)
        {
            int sy = y;
            if (flip_screen())
                sy ^= 0x1ff;

            sy -= 0x200 - (2 * m_background_scroll[1]);

            for (int x = cliprect.min_x; x <= cliprect.max_x; x++)
            {
                if (sy < 0)
                    bitmap.pix(y, x) = m_background_ram[((sy & 0x100) / 8) * 0x40] & 0xf;
                else
                {
                    // TODO: confirm the memory layout
                    int sx = x + (2 * m_background_scroll[0]) + 0x70;
                    int shift = (sy & 4);

                    bitmap.pix(y, x) = (m_background_ram[((sx / 8) & 0x3f) + ((sy / 8) * 0x40)] >> shift) & 0xf;
                }
            }
        }
    }
#endif


    partial class tnx1_state : driver_device
    {
        uint32_t screen_update(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
#if false
            const auto ilmode(m_io_mconf->read());
            bitmap_ind16 &bm((ilmode == 0) ? bitmap : m_bitmap[m_field]);

            update_palette();
            draw_background(bm, cliprect);
            draw_sprites(bm, cliprect);
            m_fg_tilemap->draw(screen, bm, cliprect, 0, 0);
            if (ilmode == 1)
            {
                for (int y=(cliprect.min_y); y<=cliprect.max_y; y ++)
                {
                    if ((y & 1) == m_field)
                        for (int x=cliprect.min_x; x<=cliprect.max_x; x++)
                            bitmap.pix(y, x) = 0;
                    else
                        for (int x=cliprect.min_x; x<=cliprect.max_x; x++)
                            bitmap.pix(y, x) = bm.pix(y, x);
                }
            }
            else if (ilmode == 2)
            {
                for (int y=(cliprect.min_y); y<=cliprect.max_y; y ++)
                {
                    auto &bm_last(m_bitmap[m_field ^ 1]);
                    if ((y & 1) == m_field)
                        for (int x=cliprect.min_x; x<=cliprect.max_x; x++)
                            bitmap.pix(y, x) = bm_last.pix(y, x);
                    else
                        for (int x=cliprect.min_x; x<=cliprect.max_x; x++)
                            bitmap.pix(y, x) = bm.pix(y, x);
                }
            }
            return 0;
#endif
        }
    }
}
