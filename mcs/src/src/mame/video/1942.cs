// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using indirect_pen_t = System.UInt16;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using offs_t = System.UInt32;
using pen_t = System.UInt32;
using u8 = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    public partial class _1942_state : driver_device
    {
        void create_palette()
        {
            ListBytesPointer color_prom = new ListBytesPointer(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();
            int i;

            for (i = 0; i < 256; i++)
            {
                int bit0;
                int bit1;
                int bit2;
                int bit3;
                int r;
                int g;
                int b;

                /* red component */
                bit0 = (color_prom[i + 0 * 256] >> 0) & 0x01;
                bit1 = (color_prom[i + 0 * 256] >> 1) & 0x01;
                bit2 = (color_prom[i + 0 * 256] >> 2) & 0x01;
                bit3 = (color_prom[i + 0 * 256] >> 3) & 0x01;
                r = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;
                /* green component */
                bit0 = (color_prom[i + 1 * 256] >> 0) & 0x01;
                bit1 = (color_prom[i + 1 * 256] >> 1) & 0x01;
                bit2 = (color_prom[i + 1 * 256] >> 2) & 0x01;
                bit3 = (color_prom[i + 1 * 256] >> 3) & 0x01;
                g = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;
                /* blue component */
                bit0 = (color_prom[i + 2 * 256] >> 0) & 0x01;
                bit1 = (color_prom[i + 2 * 256] >> 1) & 0x01;
                bit2 = (color_prom[i + 2 * 256] >> 2) & 0x01;
                bit3 = (color_prom[i + 2 * 256] >> 3) & 0x01;
                b = 0x0e * bit0 + 0x1f * bit1 + 0x43 * bit2 + 0x8f * bit3;

                m_palette.target.palette_interface().set_indirect_color(i, new rgb_t((u8)r, (u8)g, (u8)b));
            }
        }


        //PALETTE_INIT_MEMBER(_1942_state,1942)
        public void palette_init_1942(palette_device palette)
        {
            create_palette();

            ListBytesPointer color_prom = new ListBytesPointer(memregion("proms").base_());  //const uint8_t *color_prom = memregion("proms")->base();
            int i;
            int colorbase;
            color_prom += 3 * 256;
            /* color_prom now points to the beginning of the lookup table */


            /* characters use palette entries 128-143 */
            colorbase = 0;
            for (i = 0; i < 64 * 4; i++)
            {
                m_palette.target.palette_interface().set_pen_indirect((pen_t)(colorbase + i), (indirect_pen_t)(0x80 | color_prom[0]));  //*color_prom++);
                color_prom++;
            }
            colorbase += 64 * 4;

            /* background tiles use palette entries 0-63 in four banks */
            for (i = 0; i < 32 * 8; i++)
            {
                m_palette.target.palette_interface().set_pen_indirect((pen_t)(colorbase + 0 * 32 * 8 + i), (indirect_pen_t)(0x00 | color_prom[0]));  //*color_prom);
                m_palette.target.palette_interface().set_pen_indirect((pen_t)(colorbase + 1 * 32 * 8 + i), (indirect_pen_t)(0x10 | color_prom[0]));
                m_palette.target.palette_interface().set_pen_indirect((pen_t)(colorbase + 2 * 32 * 8 + i), (indirect_pen_t)(0x20 | color_prom[0]));
                m_palette.target.palette_interface().set_pen_indirect((pen_t)(colorbase + 3 * 32 * 8 + i), (indirect_pen_t)(0x30 | color_prom[0]));
                color_prom++;
            }
            colorbase += 4 * 32 * 8;

            /* sprites use palette entries 64-79 */
            for (i = 0; i < 16 * 16; i++)
            {
                m_palette.target.palette_interface().set_pen_indirect((pen_t)(colorbase + i), (indirect_pen_t)(0x40 | color_prom[0]));  //*color_prom++);
                color_prom++;
            }
        }


        /***************************************************************************

          Memory handlers

        ***************************************************************************/

        //WRITE8_MEMBER(_1942_state::_1942_fgvideoram_w)
        public void _1942_fgvideoram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
#if false
            m_fg_videoram[offset] = data;
            m_fg_tilemap->mark_tile_dirty(offset & 0x3ff);
#endif
        }


        //WRITE8_MEMBER(_1942_state::_1942_bgvideoram_w)
        public void _1942_bgvideoram_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
#if false
            m_bg_videoram[offset] = data;
            m_bg_tilemap->mark_tile_dirty((offset & 0x0f) | ((offset >> 1) & 0x01f0));
#endif
        }


        //WRITE8_MEMBER(_1942_state::_1942_palette_bank_w)
        public void _1942_palette_bank_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
#if false
            if (m_palette_bank != data)
            {
                m_palette_bank = data & 3;
                m_bg_tilemap->mark_all_dirty();
            }
#endif
        }


        //WRITE8_MEMBER(_1942_state::_1942_scroll_w)
        public void _1942_scroll_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
#if false
            m_scroll[offset] = data;
            m_bg_tilemap->set_scrollx(0, m_scroll[0] | (m_scroll[1] << 8));
#endif
        }


        //WRITE8_MEMBER(_1942_state::_1942_c804_w)
        public void _1942_c804_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff)
        {
            throw new emu_unimplemented();
#if false
            /* bit 7: flip screen
               bit 4: cpu B reset
               bit 0: coin counter */

            machine().bookkeeping().coin_counter_w(0,data & 0x01);

            m_audiocpu->set_input_line(INPUT_LINE_RESET, (data & 0x10) ? ASSERT_LINE : CLEAR_LINE);

            flip_screen_set(data & 0x80);
#endif
        }


        /***************************************************************************

          Display refresh

        ***************************************************************************/


        public uint32_t screen_update(screen_device screen, bitmap_ind16 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
#if false
            m_bg_tilemap->draw(screen, bitmap, cliprect, 0, 0);
            draw_sprites(bitmap, cliprect);
            m_fg_tilemap->draw(screen, bitmap, cliprect, 0, 0);
#endif

            return 0;
        }
    }
}
