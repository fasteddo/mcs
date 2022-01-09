// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.rendertypes_global;


namespace mame
{
    public static class rendertypes_global
    {
        //**************************************************************************
        //  CONSTANTS
        //**************************************************************************

        // blending modes
        //enum
        //{
        public const int BLENDMODE_NONE = 0;                                 // no blending
        public const int BLENDMODE_ALPHA = 1;                                    // standard alpha blend
        public const int BLENDMODE_RGB_MULTIPLY = 2;                             // apply source alpha to source pix, then multiply RGB values
        public const int BLENDMODE_ADD = 3;                                      // apply source alpha to source pix, then add to destination

        public const int BLENDMODE_COUNT = 4;
        //}
    }


    //**************************************************************************
    //  TYPE DEFINITIONS
    //**************************************************************************

    // render_bounds - floating point bounding rectangle
    public struct render_bounds
    {
        public float x0;                 // leftmost X coordinate
        public float y0;                 // topmost Y coordinate
        public float x1;                 // rightmost X coordinate
        public float y1;                 // bottommost Y coordinate


        public render_bounds(render_bounds rhs) { x0 = rhs.x0; y0 = rhs.y0; x1 = rhs.x1; y1 = rhs.y1; }


        public float width() { return x1 - x0; }
        public float height() { return y1 - y0; }
        public float aspect() { return width() / height(); }
        public bool includes(float x, float y) { return (x >= x0) && (x <= x1) && (y >= y0) && (y <= y1); }


        // intersection
        public static render_bounds operator&(render_bounds left, render_bounds right)  //constexpr render_bounds operator&(render_bounds const &b) const
        {
            return new render_bounds() { x0 = std.max(left.x0, right.x0), y0 = std.max(left.y0, right.y0), x1 = std.min(left.x1, right.x1), y1 = std.min(left.y1, right.y1) };  //return render_bounds{ (std::max)(x0, b.x0), (std::max)(y0, b.y0), (std::min)(x1, b.x1), (std::min)(y1, b.y1) };
        }


        //render_bounds &operator&=(render_bounds const &b)
        //{
        //    x0 = (std::max)(x0, b.x0);
        //    y0 = (std::max)(y0, b.y0);
        //    x1 = (std::min)(x1, b.x1);
        //    y1 = (std::min)(y1, b.y1);
        //    return *this;
        //}


        public static render_bounds operator|(render_bounds left, render_bounds right)  //constexpr render_bounds operator|(render_bounds const &b) const
        {
            return new render_bounds() { x0 = std.min(left.x0, right.x0), y0 = std.min(left.y0, right.y0), x1 = std.max(left.x1, right.x1), y1 = std.max(left.y1, right.y1) };  //return render_bounds{ (std::min)(x0, b.x0), (std::min)(y0, b.y0), (std::max)(x1, b.x1), (std::max)(y1, b.y1) };
        }


        //render_bounds &operator|=(render_bounds const &b)
        //{
        //    x0 = (std::min)(x0, b.x0);
        //    y0 = (std::min)(y0, b.y0);
        //    x1 = (std::max)(x1, b.x1);
        //    y1 = (std::max)(y1, b.y1);
        //    return *this;
        //}


        public render_bounds set_xy(float left, float top, float right, float bottom)
        {
            x0 = left;
            y0 = top;
            x1 = right;
            y1 = bottom;
            return this;
        }

        public render_bounds set_wh(float left, float top, float width, float height)
        {
            x0 = left;
            y0 = top;
            x1 = left + width;
            y1 = top + height;
            return this;
        }
    }


    // render_color - floating point set of ARGB values
    public struct render_color
    {
        public float a;                  // alpha component (0.0 = transparent, 1.0 = opaque)
        public float r;                  // red component (0.0 = none, 1.0 = max)
        public float g;                  // green component (0.0 = none, 1.0 = max)
        public float b;                  // blue component (0.0 = none, 1.0 = max)


        public render_color(render_color other) { this.a = other.a; this.r = other.r; this.g = other.g; this.b = other.b; }


        public static render_color operator*(render_color left, render_color right)  //constexpr render_color operator*(render_color const &c) const
        {
            return new render_color() { a = left.a * right.a, r = left.r * right.r, g = left.g * right.g, b = left.b * right.b };  //return render_color{ a * c.a, r * c.r, g * c.g, b * c.b };
        }

        //render_color &operator*=(render_color const &c)
        //{
        //    a *= c.a;
        //    r *= c.r;
        //    g *= c.g;
        //    b *= c.b;
        //    return *this;
        //}

        public render_color set(float alpha, float red, float green, float blue)
        {
            a = alpha;
            r = red;
            g = green;
            b = blue;
            return this;
        }
    }


    // render_texuv - floating point set of UV texture coordinates
    public struct render_texuv
    {
        public float u;                  // U coordinate (0.0-1.0)
        public float v;                  // V coordinate (0.0-1.0)


        public render_texuv(render_texuv tex) { u = tex.u; v = tex.v; }
    }


    // render_quad_texuv - floating point set of UV texture coordinates
    public struct render_quad_texuv
    {
        public render_texuv tl;                 // top-left UV coordinate
        public render_texuv tr;                 // top-right UV coordinate
        public render_texuv bl;                 // bottom-left UV coordinate
        public render_texuv br;                 // bottom-right UV coordinate


        public render_quad_texuv(render_quad_texuv other)
        {
            tl = other.tl;
            tr = other.tr;
            bl = other.bl;
            br = other.br;
        }
    }
}
