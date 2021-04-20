// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using offs_t = System.UInt32;
using PointerU8 = mame.Pointer<System.Byte>;
using tilemap_memory_index = System.UInt32;
using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    partial class atarisy2_state : driver_device
    {
        /***************************************************************************
            Atari System 2 hardware
        ****************************************************************************/

        /*************************************
         *
         *  Tilemap callbacks
         *
         *************************************/

        //TILE_GET_INFO_MEMBER(atarisy2_state::get_alpha_tile_info)
        void get_alpha_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            uint16_t data = (uint16_t)m_alpha_tilemap.target.basemem_read(tile_index);
            int code = data & 0x3ff;
            int color = (data >> 13) & 0x07;
            tileinfo.set(2, (uint32_t)code, (uint32_t)color, 0);
        }


        //TILE_GET_INFO_MEMBER(atarisy2_state::get_playfield_tile_info)
        void get_playfield_tile_info(tilemap_t tilemap, ref tile_data tileinfo, tilemap_memory_index tile_index)
        {
            uint16_t data = (uint16_t)m_playfield_tilemap.target.basemem_read(tile_index);
            int code = (int)(m_playfield_tile_bank[(data >> 10) & 1] << 10) | (data & 0x3ff);
            int color = (data >> 11) & 7;
            tileinfo.set(0, (uint32_t)code, (uint32_t)color, 0);
            tileinfo.category = (uint8_t)(((uint32_t)~data >> 14) & 3);
        }


        /*************************************
         *
         *  Video system start
         *
         *************************************/
        static readonly atari_motion_objects_config s_mob_config = new atari_motion_objects_config
        (
            1,                  /* index to which gfx system */
            1,                  /* number of motion object banks */
            true,               /* are the entries linked? */
            false,              /* are the entries split? */
            false,              /* render in reverse order? */
            false,              /* render in swapped X/Y order? */
            false,              /* does the neighbor bit affect the next object? */
            0,                  /* pixels per SLIP entry (0 for no-slip) */
            0,                  /* pixel offset for SLIPs */
            0,                  /* maximum number of links to visit/scanline (0=all) */

            0x00,               /* base palette entry */
            0x40,               /* maximum number of colors */
            15,                 /* transparent pen index */

            new atari_motion_objects_config.entry( 0,0,0,0x07f8 ), /* mask for the link */
            new atari_motion_objects_config.dual_entry( 0,0x07ff,0,0,  0x0007,0,0,0), /* mask for the code index */
            new atari_motion_objects_config.dual_entry( 0,0,0,0x3000 ), /* mask for the color */
            new atari_motion_objects_config.entry( 0,0,0xffc0,0 ), /* mask for the X position */
            new atari_motion_objects_config.entry( 0x7fc0,0,0,0 ), /* mask for the Y position */
            new atari_motion_objects_config.entry( 0 ),            /* mask for the width, in tiles*/
            new atari_motion_objects_config.entry( 0,0x3800,0,0 ), /* mask for the height, in tiles */
            new atari_motion_objects_config.entry( 0,0x4000,0,0 ), /* mask for the horizontal flip */
            new atari_motion_objects_config.entry( 0 ),            /* mask for the vertical flip */
            new atari_motion_objects_config.entry( 0,0,0,0xc000 ), /* mask for the priority */
            new atari_motion_objects_config.entry( 0,0x8000,0,0 ), /* mask for the neighbor */
            new atari_motion_objects_config.entry( 0 ),            /* mask for absolute coordinates */

            new atari_motion_objects_config.entry( 0 ),            /* mask for the special value */
            0                  /* resulting value to indicate "special" */
        );


        protected override void video_start()
        {
            // reset the statics
            m_yscroll_reset_timer = machine().scheduler().timer_alloc(reset_yscroll_callback, this);
            m_vrambank.target.set_bank(0);

            // save states
            save_item(NAME(new { m_playfield_tile_bank }));
        }


        /*************************************
         *
         *  Scroll/playfield bank write
         *
         *************************************/
        void xscroll_w(offs_t offset, uint16_t data, uint16_t mem_mask)
        {
            uint16_t oldscroll = m_xscroll[0];  //uint16_t oldscroll = *m_xscroll;
            uint16_t newscroll = oldscroll;
            COMBINE_DATA(ref newscroll, data, mem_mask);

            /* if anything has changed, force a partial update */
            if (newscroll != oldscroll)
                m_screen.target.update_partial(m_screen.target.vpos());

            /* update the playfield scrolling - hscroll is clocked on the following scanline */
            m_playfield_tilemap.target.tilemap.set_scrollx(0, newscroll >> 6);

            /* update the playfield banking */
            if (m_playfield_tile_bank[0] != (newscroll & 0x0f))
            {
                m_playfield_tile_bank[0] = (uint32_t)(newscroll & 0x0f);
                m_playfield_tilemap.target.tilemap.mark_all_dirty();
            }

            /* update the data */
            m_xscroll[0] = newscroll;  //*m_xscroll = newscroll;
        }


        //TIMER_CALLBACK_MEMBER(atarisy2_state::reset_yscroll_callback)
        void reset_yscroll_callback(object ptr, int param)
        {
            m_playfield_tilemap.target.tilemap.set_scrolly(0, param);
        }


        void yscroll_w(offs_t offset, uint16_t data, uint16_t mem_mask)
        {
            uint16_t oldscroll = m_yscroll[0];  //uint16_t oldscroll = *m_yscroll;
            uint16_t newscroll = oldscroll;
            COMBINE_DATA(ref newscroll, data, mem_mask);

            /* if anything has changed, force a partial update */
            if (newscroll != oldscroll)
                m_screen.target.update_partial(m_screen.target.vpos());

            /* if bit 4 is zero, the scroll value is clocked in right away */
            if ((newscroll & 0x10) == 0)
                m_playfield_tilemap.target.tilemap.set_scrolly(0, (newscroll >> 6) - m_screen.target.vpos());
            else
                m_yscroll_reset_timer.adjust(m_screen.target.time_until_pos(0), newscroll >> 6);

            /* update the playfield banking */
            if (m_playfield_tile_bank[1] != (newscroll & 0x0f))
            {
                m_playfield_tile_bank[1] = (uint32_t)(newscroll & 0x0f);
                m_playfield_tilemap.target.tilemap.mark_all_dirty();
            }

            /* update the data */
            m_yscroll[0] = newscroll;  //*m_yscroll = newscroll;
        }


        /*************************************
         *
         *  Palette RAM to RGB converter
         *
         *************************************/

        const int ZB = 115;
        const int Z3 = 78;
        const int Z2 = 37;
        const int Z1 = 17;
        const int Z0 = 9;

        static readonly int [] intensity_table = new int [16]
        {
            0, ZB+Z0, ZB+Z1, ZB+Z1+Z0, ZB+Z2, ZB+Z2+Z0, ZB+Z2+Z1, ZB+Z2+Z1+Z0,
            ZB+Z3, ZB+Z3+Z0, ZB+Z3+Z1, ZB+Z3+Z1+Z0, ZB+Z3+Z2, ZB+Z3+Z2+Z0, ZB+Z3+Z2+Z1, ZB+Z3+Z2+Z1+Z0
        };

        static readonly int [] color_table = new int [16]
        {
            0x0, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xa, 0xb, 0xc, 0xd, 0xe, 0xe, 0xf, 0xf
        };

        rgb_t RRRRGGGGBBBBIIII(uint32_t raw)
        {
            int i = intensity_table[raw & 15];
            uint8_t r = (uint8_t)((color_table[(raw >> 12) & 15] * i) >> 4);
            uint8_t g = (uint8_t)((color_table[(raw >>  8) & 15] * i) >> 4);
            uint8_t b = (uint8_t)((color_table[(raw >>  4) & 15] * i) >> 4);

            return new rgb_t(r, g, b);
        }


        /*************************************
         *
         *  Video RAM bank read/write handlers
         *
         *************************************/

        uint16_t slapstic_r(address_space space, offs_t offset)
        {
            uint16_t result = m_slapstic_base[offset];  //int result = m_slapstic_base[offset];
            m_slapstic.target.slapstic_tweak(space, offset);

            /* an extra tweak for the next opcode fetch */
            m_vrambank.target.set_bank((offs_t)m_slapstic.target.slapstic_tweak(space, 0x1234));
            return result;
        }


        void slapstic_w(address_space space, offs_t offset, uint16_t data)
        {
            m_slapstic.target.slapstic_tweak(space, offset);

            /* an extra tweak for the next opcode fetch */
            m_vrambank.target.set_bank((offs_t)m_slapstic.target.slapstic_tweak(space, 0x1234));
        }


        /*************************************
         *
         *  Video RAM read/write handlers
         *
         *************************************/

        void spriteram_w(offs_t offset, uint16_t data, uint16_t mem_mask)
        {
            /* force an update if the link of object 0 is about to change */
            if (offset == 0x0003)
                m_screen.target.update_partial(m_screen.target.vpos());

            //COMBINE_DATA(&m_mob->spriteram()[offset]);
            var temp = m_mob.target.spriteram().GetUInt16((int)offset);
            COMBINE_DATA(ref temp, data, mem_mask);
            m_mob.target.spriteram().SetUInt16((int)offset, temp);
        }


        /*************************************
         *
         *  Main refresh
         *
         *************************************/
        uint32_t screen_update_atarisy2(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            // start drawing
            m_mob.target.draw_async(cliprect);

            // reset priorities
            bitmap_ind8 priority_bitmap = screen.priority();
            priority_bitmap.fill(0, cliprect);

            // draw the playfield
            m_playfield_tilemap.target.tilemap.draw(screen, bitmap, cliprect, 0, 0);
            m_playfield_tilemap.target.tilemap.draw(screen, bitmap, cliprect, 1, 1);
            m_playfield_tilemap.target.tilemap.draw(screen, bitmap, cliprect, 2, 2);
            m_playfield_tilemap.target.tilemap.draw(screen, bitmap, cliprect, 3, 3);

            // draw and merge the MO
            bitmap_ind16 mobitmap = m_mob.target.bitmap();
            for (sparse_dirty_rect rect = m_mob.target.first_dirty_rect(cliprect); rect != null; rect = rect.next())
            {
                for (int y = rect.m_rect.top(); y <= rect.m_rect.bottom(); y++)
                {
                    PointerU16 mo = mobitmap.pix(y);  //uint16_t const *const mo = &mobitmap.pix(y);
                    PointerU16 pf = bitmap.pix(y);  //uint16_t *const pf = &bitmap.pix(y);
                    PointerU8 pri = priority_bitmap.pix(y);  //uint8_t const *const pri = &priority_bitmap.pix(y);
                    for (int x = rect.m_rect.left(); x <= rect.m_rect.right(); x++)
                    {
                        if (mo[x] != 0xffff)
                        {
                            int mopriority = mo[x] >> atari_motion_objects_device.PRIORITY_SHIFT;

                            // high priority PF?
                            if (((mopriority + pri[x]) & 2) != 0)
                            {
                                // only gets priority if PF pen is less than 8
                                if ((pf[x] & 0x08) == 0)
                                    pf[x] = (uint16_t)(mo[x] & atari_motion_objects_device.DATA_MASK);
                            }

                            // low priority
                            else
                            {
                                pf[x] = (uint16_t)(mo[x] & atari_motion_objects_device.DATA_MASK);
                            }
                        }
                    }
                }
            }

            // add the alpha on top
            m_alpha_tilemap.target.tilemap.draw(screen, bitmap, cliprect, 0, 0);
            return 0;
        }
    }
}
