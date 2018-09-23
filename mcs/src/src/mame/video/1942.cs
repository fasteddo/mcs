// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using uint32_t = System.UInt32;


namespace mame
{
    public partial class _1942_state : driver_device
    {
        //PALETTE_INIT_MEMBER(_1942_state,1942)
        public void palette_init_1942(palette_device palette)
        {
            throw new emu_unimplemented();
#if false
            create_palette();

            const UINT8 *color_prom = memregion("proms")->base();
            int i, colorbase;
            color_prom += 3 * 256;
            /* color_prom now points to the beginning of the lookup table */


            /* characters use palette entries 128-143 */
            colorbase = 0;
            for (i = 0; i < 64 * 4; i++)
            {
                m_palette->set_pen_indirect(colorbase + i, 0x80 | *color_prom++);
            }
            colorbase += 64 * 4;

            /* background tiles use palette entries 0-63 in four banks */
            for (i = 0; i < 32 * 8; i++)
            {
                m_palette->set_pen_indirect(colorbase + 0 * 32 * 8 + i, 0x00 | *color_prom);
                m_palette->set_pen_indirect(colorbase + 1 * 32 * 8 + i, 0x10 | *color_prom);
                m_palette->set_pen_indirect(colorbase + 2 * 32 * 8 + i, 0x20 | *color_prom);
                m_palette->set_pen_indirect(colorbase + 3 * 32 * 8 + i, 0x30 | *color_prom);
                color_prom++;
            }
            colorbase += 4 * 32 * 8;

            /* sprites use palette entries 64-79 */
            for (i = 0; i < 16 * 16; i++)
                m_palette->set_pen_indirect(colorbase + i, 0x40 | *color_prom++);
#endif
        }


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
