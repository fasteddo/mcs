// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using bitmap_ptr = mame.bitmap_argb32;
using texture_ptr = mame.render_texture;


namespace mame.ui
{
    public class widgets_manager
    {
        //using bitmap_ptr = std::unique_ptr<bitmap_argb32>;
        //using texture_ptr = std::unique_ptr<render_texture, texture_destroyer>;


        //class texture_destroyer
        //{
        //public:
        //    texture_destroyer(render_manager &manager) : m_manager(manager) { }
        //    void operator()(render_texture *texture) const { m_manager.get().texture_free(texture); }
        //private:
        //    std::reference_wrapper<render_manager> m_manager;
        //};


        bitmap_ptr m_hilight_bitmap;
        texture_ptr m_hilight_texture;
        bitmap_ptr  m_hilight_main_bitmap;
        texture_ptr m_hilight_main_texture;
        texture_ptr m_arrow_texture;


        public widgets_manager(running_machine machine)
        {
            m_hilight_bitmap = new bitmap_ptr(256, 1);
            m_hilight_texture = null;  //, m_hilight_texture(nullptr, machine.render())
            m_hilight_main_bitmap = new bitmap_ptr(1, 128);
            m_hilight_main_texture = null;  //, m_hilight_main_texture(nullptr, machine.render())
            m_arrow_texture = null;  //, m_arrow_texture(nullptr, machine.render())


            render_manager render = machine.render();

            // create a texture for hilighting items
            for (UInt32 x = 0; x < 256; ++x)
            {
                UInt32 alpha = ((x < 25) ? (0xff * x / 25) : (x >(256 - 25)) ? (0xff * (255 - x) / 25) : 0xff);
                //m_hilight_bitmap->pix32(0, x) = rgb_t(alpha, 0xff, 0xff, 0xff);
                RawBuffer m_hilight_bitmapBuf;
                UInt32 m_hilight_bitmapOffset = m_hilight_bitmap.pix32(out m_hilight_bitmapBuf, 0, (int)x);
                m_hilight_bitmapBuf.set_uint32((int)m_hilight_bitmapOffset, new rgb_t((byte)alpha,0xff,0xff,0xff));
            }

            m_hilight_texture = render.texture_alloc();  //m_hilight_texture.reset(render.texture_alloc());
            m_hilight_texture.set_bitmap(m_hilight_bitmap, m_hilight_bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);

            // create a texture for hilighting items in main menu
            for (UInt32 y = 0; y < 128; ++y)
            {
                UInt32 r1 = 0;
                UInt32 g1 = 169;
                UInt32 b1 = 255; // any start color
                UInt32 r2 = 0;
                UInt32 g2 = 39;
                UInt32 b2 = 130; // any stop color
                UInt32 r = r1 + (y * (r2 - r1) / 128);
                UInt32 g = g1 + (y * (g2 - g1) / 128);
                UInt32 b = b1 + (y * (b2 - b1) / 128);
                //m_hilight_main_bitmap->pix32(y, 0) = rgb_t(r, g, b);
                RawBuffer m_hilight_main_bitmapBuf;
                UInt32 m_hilight_main_bitmapOffset = m_hilight_main_bitmap.pix32(out m_hilight_main_bitmapBuf, (int)y, 0);
                m_hilight_main_bitmapBuf.set_uint32((int)m_hilight_main_bitmapOffset, new rgb_t((byte)r, (byte)g, (byte)b));
            }

            m_hilight_main_texture = render.texture_alloc();  //m_hilight_main_texture.reset(render.texture_alloc());
            m_hilight_main_texture.set_bitmap(m_hilight_main_bitmap, m_hilight_main_bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);

            // create a texture for arrow icons
            m_arrow_texture = render.texture_alloc(render_triangle);  //m_arrow_texture.reset(render.texture_alloc(render_triangle));
        }


        public render_texture hilight_texture() { return m_hilight_texture; }
        public render_texture hilight_main_texture() { return m_hilight_main_texture; }
        public render_texture arrow_texture() { return m_arrow_texture; }


        //-------------------------------------------------
        //  render_triangle - render a triangle that
        //  is used for up/down arrows and left/right
        //  indicators
        //-------------------------------------------------
        static void render_triangle(bitmap_argb32 dest, bitmap_argb32 source, rectangle sbounds, object param)  //void *param)
        {
            int halfwidth = dest.width() / 2;
            int height = dest.height();
            int x, y;

            // start with all-transparent
            dest.fill(new rgb_t(0x00, 0x00, 0x00, 0x00));

            // render from the tip to the bottom
            for (y = 0; y < height; y++)
            {
                int linewidth = (y * (halfwidth - 1) + (height / 2)) * 255 * 2 / height;
                //uint32_t *target = &dest.pix32(y, halfwidth);
                RawBuffer targetBuf;
                UInt32 targetOffset = dest.pix32(out targetBuf, y, halfwidth);

                // don't antialias if height < 12
                if (dest.height() < 12)
                {
                    int pixels = (linewidth + 254) / 255;
                    if (pixels % 2 == 0) pixels++;
                    linewidth = pixels * 255;
                }

                // loop while we still have data to generate
                for (x = 0; linewidth > 0; x++)
                {
                    int dalpha;

                    // first column we only consume one pixel
                    if (x == 0)
                    {
                        dalpha = Math.Min(0xff, linewidth);
                        targetBuf.set_uint32((int)targetOffset + x, new rgb_t((byte)dalpha, 0xff, 0xff, 0xff));  //target[x] = new rgb_t(dalpha, 0xff, 0xff, 0xff);
                    }

                    // remaining columns consume two pixels, one on each side
                    else
                    {
                        dalpha = Math.Min(0x1fe, linewidth);
                        targetBuf.set_uint32((int)targetOffset + x, new rgb_t((byte)(dalpha / 2), 0xff, 0xff, 0xff));  //target[x] = target[-x] = new rgb_t((byte)(dalpha / 2), 0xff, 0xff, 0xff);
                        targetBuf.set_uint32((int)targetOffset + (-x), new rgb_t((byte)(dalpha / 2), 0xff, 0xff, 0xff));
                    }

                    // account for the weight we consumed */
                    linewidth -= dalpha;
                }
            }
        }
    }
}
