// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using offs_t = System.UInt32;  //using offs_t = u32;
using pen_t = System.UInt32;  //typedef u32 pen_t;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame
{
    partial class mw8080bw_state : driver_device
    {
        uint32_t screen_update_mw8080bw(screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect)
        {
            throw new emu_unimplemented();
        }


        /*************************************
         *
         *  Space Encounters
         *
         *************************************/

        //#define SPCENCTR_TOP_TRENCH_DARK_RGB32_PEN       rgb_t(0x4d, 0x4d, 0x4d)
        //#define SPCENCTR_TOP_TRENCH_LIGHT_RGB32_PEN      rgb_t(0xe6, 0xe6, 0xe6)
        //#define SPCENCTR_SIDE_TRENCH_DARK_RGB32_PEN      rgb_t(0x1a, 0x1a, 0x1a)
        //#define SPCENCTR_SIDE_TRENCH_LIGHT_RGB32_PEN     rgb_t(0x80, 0x80, 0x80)
        //#define SPCENCTR_BOTTOM_TRENCH_DARK_RGB32_PEN    rgb_t(0x0d, 0x0d, 0x0d)
        //#define SPCENCTR_BOTTOM_TRENCH_LIGHT_RGB32_PEN   rgb_t(0x66, 0x66, 0x66)
        //#define SPCENCTR_BRIGHTNESS_DECAY                10


        //uint32_t spcenctr_state::screen_update(screen_device &screen, bitmap_rgb32 &bitmap, const rectangle &cliprect)


        /*************************************
         *
         *  Phantom II
         *
         *************************************/

        //#define PHANTOM2_CLOUD_COUNTER_START      (0x0e0b)
        //#define PHANTOM2_CLOUD_COUNTER_END        (0x1000)
        //#define PHANTOM2_CLOUD_COUNTER_PERIOD     (PHANTOM2_CLOUD_COUNTER_END - PHANTOM2_CLOUD_COUNTER_START)

        //#define PHANTOM2_RGB32_CLOUD_PEN          rgb_t(0xc0, 0xc0, 0xc0)

        //uint32_t mw8080bw_state::screen_update_phantom2(screen_device &screen, bitmap_rgb32 &bitmap, const rectangle &cliprect)


        //WRITE_LINE_MEMBER(mw8080bw_state::screen_vblank_phantom2)


        /*************************************
         *
         *  Space Invaders
         *
         *************************************/

        // the flip screen circuit is just a couple of relays on the monitor PCB

        uint32_t screen_update_invaders(screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect)
        {
            uint8_t x = 0;
            uint8_t y = MW8080BW_VCOUNTER_START_NO_VBLANK;
            uint8_t video_data = 0;

            while (true)
            {
                // plot the current pixel
                pen_t pen = (video_data & 0x01) != 0 ? rgb_t.white() : rgb_t.black();

                if (m_flip_screen != 0)
                    bitmap.pix(MW8080BW_VBSTART - 1 - (y - MW8080BW_VCOUNTER_START_NO_VBLANK), MW8080BW_HPIXCOUNT - 1 - x).SetUInt32(0, pen);
                else
                    bitmap.pix(y - MW8080BW_VCOUNTER_START_NO_VBLANK, x).SetUInt32(0, pen);

                // next pixel
                video_data = (uint8_t)(video_data >> 1);
                x = (uint8_t)(x + 1);

                // end of line?
                if (x == 0)
                {
                    // yes, flush out the shift register
                    for (int i = 0; i < 4; i++)
                    {
                        pen = (video_data & 0x01) != 0 ? rgb_t.white() : rgb_t.black();

                        if (m_flip_screen != 0)
                            bitmap.pix(MW8080BW_VBSTART - 1 - (y - MW8080BW_VCOUNTER_START_NO_VBLANK), MW8080BW_HPIXCOUNT - 1 - (256 + i)).SetUInt32(0, pen);
                        else
                            bitmap.pix(y - MW8080BW_VCOUNTER_START_NO_VBLANK, 256 + i).SetUInt32(0, pen);

                        video_data = (uint8_t)(video_data >> 1);
                    }

                    // next row, video_data is now 0, so the next line will start with 4 blank pixels
                    y = (uint8_t)(y + 1);

                    // end of screen?
                    if (y == 0)
                        break;
                }
                else if ((x & 0x07) == 0x04) // the video RAM is read at every 8 pixels starting with pixel 4
                {
                    offs_t offs = (offs_t)((y << 5) | (x >> 3));
                    video_data = m_main_ram[offs].op;
                }
            }

            return 0;
        }
    }
}
