// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using uint8_t = System.Byte;
using uint32_t = System.UInt32;

using static mame.cpp_global;
using static mame.device_global;
using static mame.palette_global;
using static mame.render_global;
using static mame.rendertypes_global;


namespace mame
{
    class vector_options
    {
        //friend class vector_device;

        public static float s_flicker;
        public static float s_beam_width_min;
        public static float s_beam_width_max;
        public static float s_beam_dot_size;
        public static float s_beam_intensity_weight;

        public static void init(emu_options options)
        {
            s_beam_width_min = options.beam_width_min();
            s_beam_width_max = options.beam_width_max();
            s_beam_dot_size = options.beam_dot_size();
            s_beam_intensity_weight = options.beam_intensity_weight();
            s_flicker = options.flicker();
        }
    }


    public class vector_device : device_t
                                 //device_video_interface
    {
        //DEFINE_DEVICE_TYPE(VECTOR, vector_device, "vector_device", "VECTOR")
        static device_t device_creator_vector_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, uint32_t clock) { return new vector_device(mconfig, tag, owner, clock); }
        public static readonly device_type VECTOR = DEFINE_DEVICE_TYPE(device_creator_vector_device, "vector_device", "VECTOR");


        /* The vertices are buffered here */
        struct point
        {
            public int x;
            public int y;
            public rgb_t col;
            public int intensity;

            //point() : x(0), y(0), col(0), intensity(0) { }
        }


        const int VECTOR_WIDTH_DENOM = 512;

        // 20000 is needed for mhavoc (see MT 06668) 10000 is enough for other games
        const int MAX_POINTS = 20000;


        point [] m_vector_list;  //std::unique_ptr<point[]> m_vector_list;
        int m_vector_index;
        int m_min_intensity;
        int m_max_intensity;


        public device_video_interface m_divideo;


        public static rgb_t color111(uint8_t c) { return new rgb_t(pal1bit((uint8_t)(c >> 2)), pal1bit((uint8_t)(c >> 1)), pal1bit((uint8_t)(c >> 0))); }  //template <typename T> static constexpr rgb_t color111(T c) { return rgb_t(pal1bit(c >> 2), pal1bit(c >> 1), pal1bit(c >> 0)); }
        //template <typename T> static constexpr rgb_t color222(T c) { return rgb_t(pal2bit(c >> 4), pal2bit(c >> 2), pal2bit(c >> 0)); }
        //template <typename T> static constexpr rgb_t color444(T c) { return rgb_t(pal4bit(c >> 8), pal4bit(c >> 4), pal4bit(c >> 0)); }


        // construction/destruction
        vector_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, VECTOR, tag, owner, clock)
        {
            m_divideo = new device_video_interface(mconfig, this);  //, device_video_interface(mconfig, *this)


            m_vector_list = null;
            m_min_intensity = 255;
            m_max_intensity = 0;
        }


        public uint32_t screen_update(screen_device screen, bitmap_rgb32 bitmap, rectangle cliprect)
        {
            uint32_t flags = PRIMFLAG_ANTIALIAS(1) | PRIMFLAG_BLENDMODE(BLENDMODE_ADD) | PRIMFLAG_VECTOR(1);
            rectangle visarea = screen.visible_area();
            float xscale = 1.0f / (65536 * visarea.width());
            float yscale = 1.0f / (65536 * visarea.height());
            float xoffs = (float)visarea.min_x;
            float yoffs = (float)visarea.min_y;

            int curpointIdx = 0;  //point *curpoint;
            int lastx = 0;
            int lasty = 0;

            curpointIdx = 0;  //curpoint = m_vector_list.get();
            var curpoint = m_vector_list;

            screen.container().empty();
            screen.container().add_rect(0.0f, 0.0f, 1.0f, 1.0f, new rgb_t(0xff,0x00,0x00,0x00), PRIMFLAG_BLENDMODE(BLENDMODE_ALPHA) | PRIMFLAG_VECTORBUF(1));

            for (int i = 0; i < m_vector_index; i++)
            {
                render_bounds coords = new render_bounds();

                float intensity = (float)curpoint[curpointIdx].intensity / 255.0f;  //float intensity = (float)curpoint->intensity / 255.0f;
                float intensity_weight = normalized_sigmoid(intensity, vector_options.s_beam_intensity_weight);

                // check for static intensity
                float beam_width = m_min_intensity == m_max_intensity
                    ? vector_options.s_beam_width_min
                    : vector_options.s_beam_width_min + intensity_weight * (vector_options.s_beam_width_max - vector_options.s_beam_width_min);

                // normalize width
                beam_width *= 1.0f / (float)VECTOR_WIDTH_DENOM;

                // apply point scale for points
                if (lastx == curpoint[curpointIdx].x && lasty == curpoint[curpointIdx].y)  //if (lastx == curpoint->x && lasty == curpoint->y)
                    beam_width *= vector_options.s_beam_dot_size;

                coords.x0 = ((float)lastx - xoffs) * xscale;
                coords.y0 = ((float)lasty - yoffs) * yscale;
                coords.x1 = ((float)curpoint[curpointIdx].x - xoffs) * xscale;  //coords.x1 = ((float)curpoint->x - xoffs) * xscale;
                coords.y1 = ((float)curpoint[curpointIdx].y - yoffs) * yscale;  //coords.y1 = ((float)curpoint->y - yoffs) * yscale;

                if (curpoint[curpointIdx].intensity != 0)  //if (curpoint->intensity != 0)
                {
                    screen.container().add_line(
                        coords.x0, coords.y0, coords.x1, coords.y1,
                        beam_width,
                        new rgb_t(((uint32_t)curpoint[curpointIdx].intensity << 24) | (curpoint[curpointIdx].col & 0xffffff)),  //(curpoint->intensity << 24) | (curpoint->col & 0xffffff),
                        flags);
                }

                lastx = curpoint[curpointIdx].x;  //lastx = curpoint->x;
                lasty = curpoint[curpointIdx].y;  //lasty = curpoint->y;

                curpointIdx++;  //curpoint++;
            }

            return 0;
        }


        public void clear_list()
        {
            m_vector_index = 0;
        }


        public void add_point(int x, int y, rgb_t color, int intensity)
        {
            //point *newpoint;

            intensity = std.clamp(intensity, 0, 255);

            m_min_intensity = intensity > 0 ? std.min(m_min_intensity, intensity) : m_min_intensity;
            m_max_intensity = intensity > 0 ? std.max(m_max_intensity, intensity) : m_max_intensity;

            if (vector_options.s_flicker != 0 && (intensity > 0))
            {
                float random = (float)(machine().rand() & 255) / 255.0f; // random value between 0.0 and 1.0

                intensity -= (int)(intensity * random * vector_options.s_flicker);

                intensity = std.clamp(intensity, 0, 255);
            }

            ref point newpoint = ref m_vector_list[m_vector_index];
            newpoint.x = x;
            newpoint.y = y;
            newpoint.col = color;
            newpoint.intensity = intensity;

            m_vector_index++;
            if (m_vector_index >= MAX_POINTS)
            {
                m_vector_index--;
                logerror("*** Warning! Vector list overflow!\n");
            }
        }


        // device-level overrides
        protected override void device_start()
        {
            vector_options.init(machine().options());

            m_vector_index = 0;

            /* allocate memory for tables */
            m_vector_list = new point[MAX_POINTS];
        }


        float normalized_sigmoid(float n, float k)
        {
            // valid for n and k in range of -1.0 and 1.0
            return (n - n * k) / (k - fabs(n) * 2.0f * k + 1.0f);
        }
    }
}
