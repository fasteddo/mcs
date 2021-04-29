// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using uint8_t = System.Byte;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;


namespace mame
{
    // used by galaga, bosconian, and their various clones
    public class starfield_05xx_device : device_t
    {
        //DEFINE_DEVICE_TYPE(STARFIELD_05XX, starfield_05xx_device, "namco_05xx_starfield", "Namco 05xx Starfield")
        static device_t device_creator_starfield_05xx_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new starfield_05xx_device(mconfig, tag, owner, clock); }
        public static readonly device_type STARFIELD_05XX = DEFINE_DEVICE_TYPE(device_creator_starfield_05xx_device, "namco_05xx_starfield", "Namco 05xx Starfield");


        const uint16_t STARS_COLOR_BASE = 64*4+64*4;

        const int VISIBLE_LINES = 224;
        const int STARFIELD_PIXEL_WIDTH = 256;

        const int LFSR_CYCLES_PER_LINE = 256;
        const int LFSR_HIT_MASK = 0xFA14;
        const int LFSR_HIT_VALUE = 0x7800;
        const uint16_t LFSR_SEED = 0x7FFF;


        static readonly int [] speed_X_cycle_count_offset =
        {
            0,  1,  2,  3, -4, -3, -2, -1
        };

        static readonly int [] pre_vis_cycle_count_values =
        {
            22 * LFSR_CYCLES_PER_LINE,
            23 * LFSR_CYCLES_PER_LINE,
            22 * LFSR_CYCLES_PER_LINE,
            23 * LFSR_CYCLES_PER_LINE,
            19 * LFSR_CYCLES_PER_LINE,
            20 * LFSR_CYCLES_PER_LINE,
            20 * LFSR_CYCLES_PER_LINE,
            22 * LFSR_CYCLES_PER_LINE
        };

        static readonly int [] post_vis_cycle_count_values =
        {
            10 * LFSR_CYCLES_PER_LINE,
            10 * LFSR_CYCLES_PER_LINE,
            12 * LFSR_CYCLES_PER_LINE,
            12 * LFSR_CYCLES_PER_LINE,
            9  * LFSR_CYCLES_PER_LINE,
            9  * LFSR_CYCLES_PER_LINE,
            10 * LFSR_CYCLES_PER_LINE,
            9  * LFSR_CYCLES_PER_LINE
        };


        uint8_t  m_enable;
        uint16_t m_lfsr;
        uint16_t m_pre_vis_cycle_count;
        uint16_t m_post_vis_cycle_count;
        uint8_t  m_set_a;
        uint8_t  m_set_b;

        uint16_t m_offset_x;
        uint16_t m_offset_y;
        uint16_t m_limit_x;


        starfield_05xx_device(machine_config mconfig, string tag, device_t owner, uint32_t clock)
            : base(mconfig, STARFIELD_05XX, tag, owner, clock)
        {
            m_enable = 0;
            m_lfsr = LFSR_SEED;
            m_pre_vis_cycle_count = 0;
            m_post_vis_cycle_count = 0;
            m_set_a = 0;
            m_set_b = 0;
            m_offset_x = 0;
            m_offset_y = 0;
            m_limit_x = 0;
        }


        public void enable_starfield(uint8_t on)
        {
            if (on == 0) m_lfsr = LFSR_SEED;

            m_enable = (on != 0) ? (uint8_t)1 : (uint8_t)0;
        }


        public void set_scroll_speed(uint8_t index_x, uint8_t index_y)
        {
            // Set initial pre- and post- visible cycle counts based on vertical
            // scroll registers
            m_pre_vis_cycle_count = (uint16_t)pre_vis_cycle_count_values[index_y];
            m_post_vis_cycle_count = (uint16_t)post_vis_cycle_count_values[index_y];

            // X scrolling occurs during pre-visible portion, so adjust
            // pre-visible cycle count to based on horizontal scroll registers
            m_pre_vis_cycle_count += (uint16_t)speed_X_cycle_count_offset[index_x];
        }


        public void set_active_starfield_sets(uint8_t set_a, uint8_t set_b)
        {
            // Set active starfield sets based on starfield select registers
            m_set_a = set_a;
            m_set_b = set_b;
        }


        public void set_starfield_config(uint16_t off_x, uint16_t off_y, uint16_t lim_x)
        {
            // Set X and Y starfield position offsets
            m_offset_x = off_x;
            m_offset_y = off_y;

            // Set X range limit
            m_limit_x = lim_x;
        }


        public void draw_starfield(bitmap_ind16 bitmap, rectangle cliprect, int flip)
        {
            if (m_enable == 0)
                return;

            uint16_t pre_vis_cycle_count = m_pre_vis_cycle_count;
            uint16_t post_vis_cycle_count = m_post_vis_cycle_count;

            // Advance the LFSR during the pre-visible portion of the frame
            do { m_lfsr = get_next_lfsr_state(m_lfsr); } while ((--pre_vis_cycle_count) != 0);

            // Now we are in visible portion of the frame - Output all LFSR hits here
            for (int y = m_offset_y; y < VISIBLE_LINES + m_offset_y; y++)
            {
                for (int x = m_offset_x; x < STARFIELD_PIXEL_WIDTH + m_offset_x; x++)
                {
                    // Check lfsr for hit
                    if ((m_lfsr&LFSR_HIT_MASK) == LFSR_HIT_VALUE)
                    {
                        uint8_t star_set = (uint8_t)g.bitswap(m_lfsr, 10, 8);

                        if ((m_set_a == star_set) || (m_set_b == star_set))
                        {
                            // don't draw the stars that are beyond the X limit
                            if (x < m_limit_x)
                            {
                                int dx = x;

                                if (flip != 0) dx += 64;

                                if (cliprect.contains(dx, y))
                                {
                                    uint8_t color;

                                    color  = (uint8_t)((m_lfsr >> 5) & 0x7);
                                    color |= (uint8_t)((m_lfsr << 3) & 0x18);
                                    color |= (uint8_t)((m_lfsr << 2) & 0x20);
                                    color = (uint8_t)((~color) & 0x3F);

                                    bitmap.pix(y, dx)[0] = (uint16_t)(STARS_COLOR_BASE + color);
                                }
                            }
                        }
                    }

                    // Advance LFSR
                    m_lfsr = get_next_lfsr_state(m_lfsr);
                }
            }

            // Advance the LFSR during the post-visible portion of the frame
            do { m_lfsr = get_next_lfsr_state(m_lfsr); } while ((--post_vis_cycle_count) != 0);
        }


        protected override void device_start()
        {
            save_item(NAME(new { m_enable }));
            save_item(NAME(new { m_lfsr }));
            save_item(NAME(new { m_pre_vis_cycle_count }));
            save_item(NAME(new { m_post_vis_cycle_count }));
            save_item(NAME(new { m_set_a }));
            save_item(NAME(new { m_set_b }));

            save_item(NAME(new { m_offset_x }));
            save_item(NAME(new { m_offset_y }));
            save_item(NAME(new { m_limit_x }));
        }


        protected override void device_reset()
        {
            m_enable = 0;
            m_lfsr = LFSR_SEED;
            m_pre_vis_cycle_count = 0;
            m_post_vis_cycle_count = 0;
            m_set_a = 0;
            m_set_b = 0;
        }


        uint16_t get_next_lfsr_state(uint16_t lfsr)
        {
            uint16_t bit;

            // 16-bit FIBONACCI-style LFSR with taps at 16,13,11, and 6
            // These taps produce a maximal sequence of 65,535 steps.

            bit = (uint16_t)((lfsr >> 0) ^ (lfsr >> 3) ^ (lfsr >> 5) ^ (lfsr >> 10));
            lfsr = (uint16_t)((lfsr >> 1) | (bit << 15));

            return lfsr;
        }
    }
}
