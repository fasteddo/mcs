// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;
using System.Linq;

using emu_render_detail_bounds_vector = mame.std.vector<mame.emu.render.detail.bounds_step>;  //using bounds_vector = std::vector<bounds_step>;
using emu_render_detail_color_vector = mame.std.vector<mame.emu.render.detail.color_step>;  //using color_vector = std::vector<color_step>;
using ioport_value = System.UInt32;  //typedef u32 ioport_value;
using layout_element_environment = mame.emu.render.detail.layout_environment;  //using environment = emu::render::detail::layout_environment;
using layout_element_make_component_map = mame.std.map<string, mame.layout_element.make_component_func>;  //typedef std::map<std::string, make_component_func> make_component_map;
using layout_environment_entry_vector = mame.std.vector<mame.emu.render.detail.layout_environment.entry>;  //using entry_vector = std::vector<entry>;
using layout_file_element_map = mame.std.unordered_map<string, mame.layout_element>;  //using element_map = std::unordered_map<std::string, layout_element>;
using layout_file_environment = mame.emu.render.detail.layout_environment;  //using environment = emu::render::detail::layout_environment;
using layout_file_group_map = mame.std.unordered_map<string, mame.layout_group>;  //using group_map = std::unordered_map<std::string, layout_group>;
using layout_file_view_list = mame.std.list<mame.layout_view>;  //using view_list = std::list<layout_view>;
using layout_group_environment = mame.emu.render.detail.layout_environment;  //using environment = emu::render::detail::layout_environment;
using layout_group_group_map = mame.std.unordered_map<string, mame.layout_group>;  //using group_map = std::unordered_map<std::string, layout_group>;
using layout_group_transform = mame.std.array<mame.std.array<float, mame.u64_const_3>, mame.u64_const_3>;  //using transform = std::array<std::array<float, 3>, 3>;
using layout_view_edge_vector = mame.std.vector<mame.layout_view.edge>;  //using edge_vector = std::vector<edge>;
using layout_view_element_map = mame.std.unordered_map<string, mame.layout_element>;  //using element_map = std::unordered_map<std::string, layout_element>;
using layout_view_group_map = mame.std.unordered_map<string, mame.layout_group>;  //using group_map = std::unordered_map<std::string, layout_group>;
using layout_view_item_bounds_vector = mame.std.vector<mame.emu.render.detail.bounds_step>;  //using bounds_vector = emu::render::detail::bounds_vector;
using layout_view_item_color_vector = mame.std.vector<mame.emu.render.detail.color_step>;  //using color_vector = emu::render::detail::color_vector;
using layout_view_item_id_map = mame.std.unordered_map<string, mame.layout_view.item>;  //using item_id_map = std::unordered_map<std::reference_wrapper<std::string const>, item &, std::hash<std::string>, std::equal_to<std::string> >;
using layout_view_item_list = mame.std.list<mame.layout_view.item>;  //using item_list = std::list<item>;
using layout_view_item_ref_vector = mame.std.vector<mame.layout_view.item>;  //using item_ref_vector = std::vector<std::reference_wrapper<item> >;
using layout_view_screen_ref_vector = mame.std.vector<mame.screen_device>;  //using screen_ref_vector = std::vector<std::reference_wrapper<screen_device> >;
using layout_view_view_environment = mame.emu.render.detail.view_environment;  //using view_environment = emu::render::detail::view_environment;
using layout_view_visibility_toggle_vector = mame.std.vector<mame.layout_view.visibility_toggle>;  //using visibility_toggle_vector = std::vector<visibility_toggle>;
using s32 = System.Int32;
using s64 = System.Int64;
using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using size_t = System.UInt64;
using u8 = System.Byte;
using u32 = System.UInt32;
using unsigned = System.UInt32;


namespace mame
{
    public static class rendlay_global
    {
        public const int LOG_GROUP_BOUNDS_RESOLUTION = 1 << 1;
        public const int LOG_INTERACTIVE_ITEMS       = 1 << 2;
        //#define LOG_DISK_DRAW               (1U << 3)
        //#define LOG_IMAGE_LOAD              (1U << 4)

        ////#define VERBOSE (LOG_GROUP_BOUNDS_RESOLUTION | LOG_INTERACTIVE_ITEMS | LOG_DISK_DRAW | LOG_IMAGE_LOAD)
        public const int VERBOSE = 0;
        //#define LOG_OUTPUT_FUNC osd_printf_verbose
        public static void LOGMASKED(int mask, string format, params object [] args) { if ((VERBOSE & mask) != 0) g.osd_printf_verbose(format, args); }


        public const int LAYOUT_VERSION = 2;


        //enum
        //{
        public const int LINE_CAP_NONE  = 0;
        public const int LINE_CAP_START = 1;
        public const int LINE_CAP_END   = 2;
        //}


        public static readonly layout_group_transform identity_transform = new layout_group_transform(new std.array<float, u64_const_3>(1.0F, 0.0F, 0.0F), new std.array<float, u64_const_3>(0.0F, 1.0F, 0.0F), new std.array<float, u64_const_3>(0.0F, 0.0F, 1.0F));  //layout_group.transform identity_transform {{ {{ 1.0F, 0.0F, 0.0F }}, {{ 0.0F, 1.0F, 0.0F }}, {{ 0.0F, 0.0F, 1.0F }} }};


        public static void render_bounds_transform(ref render_bounds bounds, layout_group_transform trans)  //inline void render_bounds_transform(render_bounds &bounds, layout_group::transform const &trans)
        {
            bounds = new render_bounds()
            {
                x0 = (bounds.x0 * trans[0][0]) + (bounds.y0 * trans[0][1]) + trans[0][2],
                y0 = (bounds.x0 * trans[1][0]) + (bounds.y0 * trans[1][1]) + trans[1][2],
                x1 = (bounds.x1 * trans[0][0]) + (bounds.y1 * trans[0][1]) + trans[0][2],
                y1 = (bounds.x1 * trans[1][0]) + (bounds.y1 * trans[1][1]) + trans[1][2]
            };
        }


        static void alpha_blend(ref u32 dest, u32 a, u32 r, u32 g, u32 b, u32 inva)  //inline void alpha_blend(u32 &dest, u32 a, u32 r, u32 g, u32 b, u32 inva)
        {
            rgb_t dpix = new rgb_t(dest);
            u32 da = dpix.a();
            u32 finala = (a * 255) + (da * inva);
            u32 finalr = r + ((u32)dpix.r() * da * inva);
            u32 finalg = g + ((u32)dpix.g() * da * inva);
            u32 finalb = b + ((u32)dpix.b() * da * inva);
            dest = new rgb_t((u8)(finala / 255), (u8)(finalr / finala), (u8)(finalg / finala), (u8)(finalb / finala));
        }

        static void alpha_blend(ref u32 dest, render_color c, float fill)  //inline void alpha_blend(u32 &dest, render_color const &c, float fill)
        {
            u32 a = (u32)(c.a * fill * 255.0F);
            if (a != 0)
            {
                u32 r = (u32)(c.r * (255.0F * 255.0F)) * a;
                u32 g = (u32)(c.g * (255.0F * 255.0F)) * a;
                u32 b = (u32)(c.b * (255.0F * 255.0F)) * a;
                alpha_blend(ref dest, a, r, g, b, 255 - a);
            }
        }


        public static bool add_bounds_step(emu.render.detail.layout_environment env, emu_render_detail_bounds_vector steps, util.xml.data_node node)
        {
            int state = env.get_attribute_int(node, "state", 0);
            var posIdx = std.lower_bound(
                        steps,
                        state,
                        (emu.render.detail.bounds_step lhs, int rhs) => { return lhs.state < rhs; });

            if ((-1 != posIdx) && (state == steps[posIdx].state))  //if ((steps.end() != pos) && (state == pos->state))
                return false;

            //auto &ins(*steps.emplace(pos, emu::render::detail::bounds_step{ state, { 0.0F, 0.0F, 0.0F, 0.0F }, { 0.0F, 0.0F, 0.0F, 0.0F } }));
            var ins = new emu.render.detail.bounds_step() { state = state, bounds = new render_bounds() { x0 = 0.0F, y0 = 0.0F, x1 = 0.0F, y1 = 0.0F }, delta = new render_bounds() { x0 = 0.0F, y0 = 0.0F, x1 = 0.0F, y1 = 0.0F } };
            if (posIdx == -1) steps.push_back(ins); else steps.emplace(posIdx, ins);  //steps.emplace(posIdx, ins);

            env.parse_bounds(node, out ins.bounds);
            return true;
        }


        public static void set_bounds_deltas(emu_render_detail_bounds_vector steps)
        {
            if (steps.empty())
            {
                steps.emplace_back(new emu.render.detail.bounds_step() { state = 0, bounds = new render_bounds() { x0 = 0.0F, y0 = 0.0F, x1 = 1.0F, y1 = 1.0F }, delta = new render_bounds() { x0 = 0.0F, y0 = 0.0F, x1 = 0.0F, y1 = 0.0F } });
            }
            else
            {
                var iIdx = 0;  //auto i(steps.begin());
                var jIdx = iIdx;  //auto j(i);
                while (steps.Count != ++jIdx)  //while (steps.end() != ++j)
                {
                    var i = steps[iIdx];
                    var j = steps[jIdx];

                    //throw new emu_unimplemented();
#if false
                    assert(j->state > i->state);
#endif

                    i.delta.x0 = (j.bounds.x0 - i.bounds.x0) / (j.state - i.state);
                    i.delta.x1 = (j.bounds.x1 - i.bounds.x1) / (j.state - i.state);
                    i.delta.y0 = (j.bounds.y0 - i.bounds.y0) / (j.state - i.state);
                    i.delta.y1 = (j.bounds.y1 - i.bounds.y1) / (j.state - i.state);

                    iIdx = jIdx;  //i = j;
                }
            }
        }


        public static void normalize_bounds(emu_render_detail_bounds_vector steps, float x0, float y0, float xoffs, float yoffs, float xscale, float yscale)
        {
            var iIdx = 0;  //auto i(steps.begin());
            var i = steps[iIdx];
            i.bounds.x0 = x0 + (i.bounds.x0 - xoffs) * xscale;
            i.bounds.x1 = x0 + (i.bounds.x1 - xoffs) * xscale;
            i.bounds.y0 = y0 + (i.bounds.y0 - yoffs) * yscale;
            i.bounds.y1 = y0 + (i.bounds.y1 - yoffs) * yscale;

            var jIdx = iIdx;  //auto j(i);
            while (steps.Count != ++jIdx)  //while (steps.end() != ++j)
            {
                var j = steps[jIdx];
                j.bounds.x0 = x0 + (j.bounds.x0 - xoffs) * xscale;
                j.bounds.x1 = x0 + (j.bounds.x1 - xoffs) * xscale;
                j.bounds.y0 = y0 + (j.bounds.y0 - yoffs) * yscale;
                j.bounds.y1 = y0 + (j.bounds.y1 - yoffs) * yscale;

                i.delta.x0 = (j.bounds.x0 - i.bounds.x0) / (j.state - i.state);
                i.delta.x1 = (j.bounds.x1 - i.bounds.x1) / (j.state - i.state);
                i.delta.y0 = (j.bounds.y0 - i.bounds.y0) / (j.state - i.state);
                i.delta.y1 = (j.bounds.y1 - i.bounds.y1) / (j.state - i.state);

                iIdx = jIdx;  //i = j;
                i = steps[iIdx];
            }
        }


        public static render_bounds accumulate_bounds(emu_render_detail_bounds_vector steps)
        {
            var iIdx = 0;  //auto i(steps.begin());
            var i = steps[iIdx];
            render_bounds result = new render_bounds(i.bounds);
            while (steps.Count != ++iIdx)  //while (steps.end() != ++i)
                result |= i.bounds;

            return result;
        }


        public static render_bounds interpolate_bounds(emu_render_detail_bounds_vector steps, int state)
        {
            var posIdx = std.lower_bound(
                        steps,
                        state,
                        (emu.render.detail.bounds_step lhs, int rhs) => { return lhs.state < rhs; });

            if (0 == posIdx)  //if (steps.begin() == pos)
            {
                var pos = steps[posIdx];
                return new render_bounds(pos.bounds);
            }
            else
            {
                //--pos;
                --posIdx;
                var pos = steps[posIdx];
                render_bounds result = new render_bounds(pos.bounds);
                result.x0 += pos.delta.x0 * (state - pos.state);
                result.x1 += pos.delta.x1 * (state - pos.state);
                result.y0 += pos.delta.y0 * (state - pos.state);
                result.y1 += pos.delta.y1 * (state - pos.state);

                return result;
            }
        }


        public static bool add_color_step(emu.render.detail.layout_environment env, emu_render_detail_color_vector steps, util.xml.data_node node)
        {
            int state = env.get_attribute_int(node, "state", 0);
            var posIdx = std.lower_bound(
                        steps,
                        state,
                        (emu.render.detail.color_step lhs, int rhs) => { return lhs.state < rhs; });

            if ((-1 != posIdx) && (state == steps[posIdx].state))  //if ((steps.end() != pos) && (state == pos->state))
                return false;

            steps.emplace(posIdx, new emu.render.detail.color_step() { state = state, color = env.parse_color(node), delta = new render_color() { a = 0.0F, r = 0.0F, g = 0.0F, b = 0.0F } });
            return true;
        }


        public static void set_color_deltas(emu_render_detail_color_vector steps)
        {
            if (steps.empty())
            {
                steps.emplace_back(new emu.render.detail.color_step() { state = 0, color = new render_color() { a = 1.0F, r = 1.0F, g = 1.0F, b = 1.0F }, delta = new render_color() { a = 0.0F, r = 0.0F, g = 0.0F, b = 0.0F } });
            }
            else
            {
                var iIdx = 0;  //auto i(steps.begin());
                var jIdx = iIdx;  //auto j(i);
                while (steps.Count != ++jIdx)  //while (steps.end() != ++j)
                {
                    var i = steps[iIdx];
                    var j = steps[jIdx];

                    //throw new emu_unimplemented();
#if false
                    assert(j->state > i->state);
#endif

                    i.delta.a = (j.color.a - i.color.a) / (j.state - i.state);
                    i.delta.r = (j.color.r - i.color.r) / (j.state - i.state);
                    i.delta.g = (j.color.g - i.color.g) / (j.state - i.state);
                    i.delta.b = (j.color.b - i.color.b) / (j.state - i.state);

                    iIdx = jIdx;  //i = j;
                }
            }
        }


        public static render_color interpolate_color(emu_render_detail_color_vector steps, int state)
        {
            var posIdx = std.lower_bound(
                        steps,
                        state,
                        (emu.render.detail.color_step lhs, int rhs) => { return lhs.state < rhs; });

            if (0 == posIdx)  //if (steps.begin() == pos)
            {
                var pos = steps[posIdx];
                return pos.color;
            }
            else
            {
                //--pos;
                --posIdx;
                var pos = steps[posIdx];

                render_color result = new render_color(pos.color);
                result.a += pos.delta.a * (state - pos.state);
                result.r += pos.delta.r * (state - pos.state);
                result.g += pos.delta.g * (state - pos.state);
                result.b += pos.delta.b * (state - pos.state);
                return result;
            }
        }
    }


    class layout_syntax_error : ArgumentException { public layout_syntax_error(string format, params object [] args) : base(string.Format(format, args)) { } }
    class layout_reference_error : ArgumentOutOfRangeException { public layout_reference_error(string format, params object [] args) : base(string.Format(format, args)) { } }


    namespace emu.render.detail
    {
        public class bounds_step
        {
            public int state;
            public render_bounds bounds;
            public render_bounds delta;


            public void get(out render_bounds result) { result = bounds; }
        }

        //using bounds_vector = std::vector<bounds_step>;


        public class color_step
        {
            public int state;
            public render_color color;
            public render_color delta;


            public void get(out render_color result) { result = color; }
        }

        //using color_vector = std::vector<color_step>;
    }


    /// \brief A description of a piece of visible artwork
    ///
    /// Most view_items (except for those in the screen layer) have exactly
    /// one layout_element which describes the contents of the item.
    /// Elements are separate from items because they can be re-used
    /// multiple times within a layout.  Even though an element can contain
    /// a number of components, they are treated as if they were a single
    /// bitmap.
    public class layout_element
    {
        //using environment = emu::render::detail::layout_environment;
        public delegate component make_component_func(layout_element_environment env, util.xml.data_node compnode);  //typedef component::ptr (*make_component_func)(environment &env, util::xml::data_node const &compnode);
        //typedef std::map<std::string, make_component_func> make_component_map;


        /// \brief An image, rectangle, or disk in an element
        ///
        /// Each layout_element contains one or more components. Each
        /// component can describe either an image or a rectangle/disk
        /// primitive. Each component also has a "state" associated with it,
        /// which controls whether or not the component is visible (if the
        /// owning item has the same state, it is visible).
        public abstract class component
        {
            //typedef std::unique_ptr<component> ptr;

            //using bounds_vector = emu::render::detail::bounds_vector;
            //using color_vector = emu::render::detail::color_vector;


            // internal state
            int m_statemask;                // bits of state used to control visibility
            int m_stateval;                 // masked state value to make component visible
            emu_render_detail_bounds_vector m_bounds;                   // bounds of the element
            emu_render_detail_color_vector m_color;                    // color of the element


            // construction/destruction
            //-------------------------------------------------
            //  component - constructor
            //-------------------------------------------------
            protected component(layout_element_environment env, util.xml.data_node compnode)
            {
                throw new emu_unimplemented();
            }

            //virtual ~component() = default;


            // setup

            //-------------------------------------------------
            //  normalize_bounds - normalize component bounds
            //-------------------------------------------------
            public void normalize_bounds(float xoffs, float yoffs, float xscale, float yscale)
            {
                rendlay_global.normalize_bounds(m_bounds, 0.0F, 0.0F, xoffs, yoffs, xscale, yscale);
            }


            // getters
            public int statemask() { return m_statemask; }
            public int stateval() { return m_stateval; }


            //-------------------------------------------------
            //  statewrap - get state wraparound requirements
            //-------------------------------------------------
            public std.pair<int, bool> statewrap()
            {
                int result = 0;
                bool fold = false;
                Action<int, int> adjustmask =
                        (int val, int mask) =>
                        {
                            throw new emu_unimplemented();
#if false
                            assert(!(val & ~mask));
#endif

                            Func<int, int> splatright =
                                    (int x) =>
                                    {
                                        for (unsigned shift = 1; (4 /*sizeof(x)*/ * 4) >= shift; shift <<= 1)
                                            x |= (x >> (int)shift);
                                        return x;
                                    };

                            int unfolded = splatright(mask);
                            int folded = splatright(~mask | splatright(val));
                            if ((unsigned)folded < (unsigned)unfolded)
                            {
                                result |= folded;
                                fold = true;
                            }
                            else
                            {
                                result |= unfolded;
                            }
                        };

                adjustmask(stateval(), statemask());
                int max = maxstate();
                if (m_bounds.size() > 1U)
                    max = std.max(max, m_bounds.back().state);
                if (m_color.size() > 1U)
                    max = std.max(max, m_color.back().state);
                if (0 <= max)
                    adjustmask(max, ~0);

                return std.make_pair(result, fold);
            }


            //-------------------------------------------------
            //  overall_bounds - maximum bounds for all states
            //-------------------------------------------------
            public render_bounds overall_bounds()
            {
                return rendlay_global.accumulate_bounds(m_bounds);
            }


            //-------------------------------------------------
            //  bounds - bounds for a given state
            //-------------------------------------------------
            render_bounds bounds(int state)
            {
                return rendlay_global.interpolate_bounds(m_bounds, state);
            }


            //-------------------------------------------------
            //  color - color for a given state
            //-------------------------------------------------
            //render_color layout_element::component::color(int state) const


            // operations
            public virtual void preload(running_machine machine)
            {
            }


            //-------------------------------------------------
            //  draw - draw element to texture for a given
            //  state
            //-------------------------------------------------
            public virtual void draw(running_machine machine, bitmap_argb32 dest, int state)
            {
                // get the local scaled bounds
                render_bounds curbounds = bounds(state);
                rectangle pixelbounds = new rectangle(
                        (s32)(curbounds.x0 * (float)(dest.width()) + 0.5F),
                        (s32)(std.floorf(curbounds.x1 * (float)(dest.width()) - 0.5F)),
                        (s32)(curbounds.y0 * (float)(dest.height()) + 0.5F),
                        (s32)(std.floorf(curbounds.y1 * (float)(dest.height()) - 0.5F)));

                // based on the component type, add to the texture
                if (!pixelbounds.empty())
                    draw_aligned(machine, dest, pixelbounds, state);
            }


            // helpers

            //-------------------------------------------------
            //  maxstate - maximum state drawn differently
            //-------------------------------------------------
            public virtual int maxstate()
            {
                return -1;
            }


            protected virtual void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                // derived classes must override one form or other
                throw new Exception();
            }


            // drawing helpers

            //-------------------------------------------------
            //  draw_text - draw text in the specified color
            //-------------------------------------------------
            void draw_text(
                    render_font font,
                    bitmap_argb32 dest,
                    rectangle bounds,
                    string str,
                    int align,
                    render_color color)
            {
                // compute premultiplied colors
                u32 r = (u32)(color.r * 255.0f);
                u32 g = (u32)(color.g * 255.0f);
                u32 b = (u32)(color.b * 255.0f);
                u32 a = (u32)(color.a * 255.0f);

                // get the width of the string
                float aspect = 1.0f;
                int width;


                while (true)
                {
                    width = (int)font.string_width(bounds.height(), aspect, str);
                    if (width < bounds.width())
                        break;

                    aspect *= 0.9f;
                }


                // get alignment
                int curx;
                switch (align)
                {
                    // left
                    case 1:
                        curx = bounds.min_x;
                        break;

                    // right
                    case 2:
                        curx = bounds.max_x - width;
                        break;

                    // default to center
                    default:
                        curx = bounds.min_x + (bounds.width() - width) / 2;
                        break;
                }

                // allocate a temporary bitmap
                bitmap_argb32 tempbitmap = new bitmap_argb32(dest.width(), dest.height());

                // loop over characters
                while (!str.empty())
                {
                    char schar;  //char32_t schar;
                    int scharcount = unicode_global.uchar_from_utf8(out schar, str);  //int scharcount = uchar_from_utf8(&schar, str);

                    if (scharcount == -1)
                        break;

                    // get the font bitmap
                    rectangle chbounds;
                    font.get_scaled_bitmap_and_bounds(tempbitmap, bounds.height(), aspect, schar, out chbounds);

                    // copy the data into the target
                    for (int y = 0; y < chbounds.height(); y++)
                    {
                        int effy = bounds.top() + y;
                        if (effy >= bounds.top() && effy <= bounds.bottom())
                        {
                            throw new emu_unimplemented();
#if false
                            u32 const *const src = &tempbitmap.pix(y);
                            u32 *const d = &dest.pix(effy);
                            for (int x = 0; x < chbounds.width(); x++)
                            {
                                int effx = curx + x + chbounds.get_min_x();
                                if (effx >= bounds.get_min_x() && effx <= bounds.get_max_x())
                                {
                                    UInt32 spix = (rgb_t)(src[x]).a();
                                    if (spix != 0)
                                    {
                                        rgb_t dpix = d[effx];
                                        UInt32 ta = (a * (spix + 1)) >> 8;
                                        UInt32 tr = (r * ta + dpix.r() * (0x100 - ta)) >> 8;
                                        UInt32 tg = (g * ta + dpix.g() * (0x100 - ta)) >> 8;
                                        UInt32 tb = (b * ta + dpix.b() * (0x100 - ta)) >> 8;
                                        d[effx] = (rgb_t)(tr, tg, tb);
                                    }
                                }
                            }
#endif
                        }
                    }

                    // advance in the X direction
                    curx += (int)font.char_width(bounds.height(), aspect, schar);
                    str = str.Substring(scharcount);  //str.remove_prefix(scharcount);
                }
            }


            //-------------------------------------------------
            //  draw_segment_horizontal_caps - draw a
            //  horizontal LED segment with definable end
            //  and start points
            //-------------------------------------------------
            void draw_segment_horizontal_caps(bitmap_argb32 dest, int minx, int maxx, int midy, int width, int caps, rgb_t color)
            {
                // loop over the width of the segment
                for (int y = 0; y < width / 2; y++)
                {
                    throw new emu_unimplemented();
#if false
                    u32 *const d0 = &dest.pix(midy - y);
                    u32 *const d1 = &dest.pix(midy + y);
                    int ty = (y < width / 8) ? width / 8 : y;

                    // loop over the length of the segment
                    for (int x = minx + ((caps & LINE_CAP_START) ? ty : 0); x < maxx - ((caps & LINE_CAP_END) ? ty : 0); x++)
                        d0[x] = d1[x] = color;
#endif
                }
            }

            //-------------------------------------------------
            //  draw_segment_horizontal - draw a horizontal
            //  LED segment
            //-------------------------------------------------
            void draw_segment_horizontal(bitmap_argb32 dest, int minx, int maxx, int midy, int width, rgb_t color)
            {
                draw_segment_horizontal_caps(dest, minx, maxx, midy, width, rendlay_global.LINE_CAP_START | rendlay_global.LINE_CAP_END, color);
            }

            //-------------------------------------------------
            //  draw_segment_vertical_caps - draw a
            //  vertical LED segment with definable end
            //  and start points
            //-------------------------------------------------
            void draw_segment_vertical_caps(bitmap_argb32 dest, int miny, int maxy, int midx, int width, int caps, rgb_t color)
            {
                // loop over the width of the segment
                for (int x = 0; x < width / 2; x++)
                {
                    throw new emu_unimplemented();
#if false
                    u32 *const d0 = &dest.pix(0, midx - x);
                    u32 *const d1 = &dest.pix(0, midx + x);
                    int tx = (x < width / 8) ? width / 8 : x;

                    // loop over the length of the segment
                    for (int y = miny + ((caps & LINE_CAP.LINE_CAP_START) ? tx : 0); y < maxy - ((caps & LINE_CAP.LINE_CAP_END) ? tx : 0); y++)
                        d0[y * dest.rowpixels()] = d1[y * dest.rowpixels()] = color;
#endif
                }
            }

            //-------------------------------------------------
            //  draw_segment_vertical - draw a vertical
            //  LED segment
            //-------------------------------------------------
            void draw_segment_vertical(bitmap_argb32 dest, int miny, int maxy, int midx, int width, rgb_t color)
            {
                draw_segment_vertical_caps(dest, miny, maxy, midx, width, rendlay_global.LINE_CAP_START | rendlay_global.LINE_CAP_END, color);
            }

            //-------------------------------------------------
            //  draw_segment_diagonal_1 - draw a diagonal
            //  LED segment that looks like a backslash
            //-------------------------------------------------
            void draw_segment_diagonal_1(bitmap_argb32 dest, int minx, int maxx, int miny, int maxy, int width, rgb_t color)
            {
                // compute parameters
                width = (int)(width * 1.5);
                float ratio = (maxy - miny - width) / (float)(maxx - minx);

                // draw line
                for (int x = minx; x < maxx; x++)
                {
                    if (x >= 0 && x < dest.width())
                    {
                        throw new emu_unimplemented();
#if false
                        u32 *const d = &dest.pix(0, x);
                        int step = (int)((x - minx) * ratio);

                        for (int y = maxy - width - step; y < maxy - step; y++)
                        {
                            if (y >= 0 && y < dest.height())
                                d[y * dest.rowpixels()] = color;
                        }
#endif
                    }
                }
            }

            //-------------------------------------------------
            //  draw_segment_diagonal_2 - draw a diagonal
            //  LED segment that looks like a forward slash
            //-------------------------------------------------
            void draw_segment_diagonal_2(bitmap_argb32 dest, int minx, int maxx, int miny, int maxy, int width, rgb_t color)
            {
                // compute parameters
                width = (int)(width * 1.5);
                float ratio = (maxy - miny - width) / (float)(maxx - minx);

                // draw line
                for (int x = minx; x < maxx; x++)
                {
                    if (x >= 0 && x < dest.width())
                    {
                        throw new emu_unimplemented();
#if false
                        u32 *const d = &dest.pix(0, x);
                        int step = (int)((x - minx) * ratio);

                        for (int y = miny + step; y < miny + step + width; y++)
                        {
                            if (y >= 0 && y < dest.height())
                                d[y * dest.rowpixels()] = color;
                        }
#endif
                    }
                }
            }

            //-------------------------------------------------
            //  draw_segment_decimal - draw a decimal point
            //-------------------------------------------------
            void draw_segment_decimal(bitmap_argb32 dest, int midx, int midy, int width, rgb_t color)
            {
                // compute parameters
                width /= 2;
                float ooradius2 = 1.0f / (float)(width * width);

                // iterate over y
                for (UInt32 y = 0; y <= width; y++)
                {
                    throw new emu_unimplemented();
#if false
                    u32 *const d0 = &dest.pix(midy - y);
                    u32 *const d1 = &dest.pix(midy + y);
                    float xval = width * Math.Sqrt(1.0f - (float)(y * y) * ooradius2);
                    int left;
                    int right;

                    // compute left/right coordinates
                    left = midx - (int)(xval + 0.5f);
                    right = midx + (int)(xval + 0.5f);

                    // draw this scanline
                    for (UInt32 x = left; x < right; x++)
                        d0[x] = d1[x] = color;
#endif
                }
            }

            //void draw_segment_comma(bitmap_argb32 &dest, int minx, int maxx, int miny, int maxy, int width, rgb_t color);

            //-------------------------------------------------
            //  apply_skew - apply skew to a bitmap
            //-------------------------------------------------
            void apply_skew(bitmap_argb32 dest, int skewwidth)
            {
                for (int y = 0; y < dest.height(); y++)
                {
                    throw new emu_unimplemented();
#if false
                    u32 *const d = &dest.pix(0, x);
                    int offs = skewwidth * (dest.height() - y) / dest.height();
                    for (int x = dest.width() - skewwidth - 1; x >= 0; x--)
                        destrow[x + offs] = destrow[x];
                    for (int x = 0; x < offs; x++)
                        destrow[x] = 0;
#endif
                }
            }
        }


        // a texture encapsulates a texture for a given element in a given state
        public class texture : IDisposable
        {
            public layout_element m_element;      // pointer back to the element
            public render_texture m_texture;      // texture for this state
            public int m_state;        // associated state number


            //-------------------------------------------------
            //  texture - constructor
            //-------------------------------------------------
            public texture()
            {
                m_element = null;
                m_texture = null;
                m_state = 0;
            }


            //texture(texture const &that) = delete;
            //texture(texture that);


            ~texture()
            {
                g.assert(m_isDisposed);  // can remove
            }

            bool m_isDisposed = false;
            public void Dispose()
            {
                if (m_element != null)
                    m_element.machine().render().texture_free(m_texture);

                m_isDisposed = true;
            }


            //texture &operator=(texture const &that) = delete;
            //texture &operator=(texture &&that);
        }


        // image
        class image_component : component
        {
            // internal state
            //util::nsvg_image_ptr            m_svg;              // parsed SVG image
            //std::shared_ptr<NSVGrasterizer> m_rasterizer;       // SVG rasteriser
            bitmap_argb32 m_bitmap;           // source bitmap for images
            //bool                            m_hasalpha = false; // is there any alpha component present?

            // cold state
            string m_searchpath;       // asset search path (for lazy loading)
            string m_dirname;          // directory name of image file (for lazy loading)
            string m_imagefile;        // name of the image file (for lazy loading)
            string m_alphafile;        // name of the alpha file (for lazy loading)
            string m_data;             // embedded image data


            // construction/destruction
            public image_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
                throw new emu_unimplemented();
#if false
                , m_rasterizer(env.svg_rasterizer())
#endif

                m_searchpath = env.search_path() != null ? env.search_path() : "";
                m_dirname = env.directory_name() != null ? env.directory_name() : "";
                m_imagefile = env.get_attribute_string(compnode, "file");
                m_alphafile = env.get_attribute_string(compnode, "alphafile");
                m_data = get_data(compnode);
            }


            // overrides

            public override void preload(running_machine machine)
            {
                throw new emu_unimplemented();
#if false
                if (!m_bitmap.valid() && !m_svg)
                    load_image(machine);
#endif
            }


            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }


            // internal helpers
            //void draw_bitmap(bitmap_argb32 &dest, rectangle const &bounds, int state)
            //void draw_svg(bitmap_argb32 &dest, rectangle const &bounds, int state)
            //void alpha_blend(bitmap_argb32 const &srcbitmap, bitmap_argb32 &dstbitmap, rectangle const &bounds)


            void load_image(running_machine machine)
            {
                throw new emu_unimplemented();
            }


            //void load_image_data()
            //bool load_bitmap(util::core_file &file)
            //void load_svg(util::core_file &file)
            //void parse_svg(char *svgdata)


            static string get_data(util.xml.data_node compnode)
            {
                util.xml.data_node datanode = compnode.get_child("data");
                if (datanode != null && datanode.get_value() != null)
                    return datanode.get_value();
                else
                    return "";
            }


            //void load_bitmap(running_machine &machine)
        }


        // rectangle
        class rect_component : component
        {
            // construction/destruction
            rect_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
            }


            // overrides
            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // ellipse
        class disk_component : component
        {
            // construction/destruction
            disk_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
            }


            // overrides
            public override void draw(running_machine machine, bitmap_argb32 dest, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // text string
        class text_component : component
        {
            // internal state
            string m_string;                   // string for text components
            int m_textalign;                // text alignment to box


            // construction/destruction
            text_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
                m_string = env.get_attribute_string(compnode, "string");
                m_textalign = env.get_attribute_int(compnode, "align", 0);
            }


            // overrides
            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 7-segment LCD
        class led7seg_component : component
        {
            // construction/destruction
            led7seg_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
            }


            // overrides
            public override int maxstate() { return 255; }


            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 8-segment fluorescent (Gottlieb System 1)
        class led8seg_gts1_component : component
        {
            // construction/destruction
            led8seg_gts1_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
            }


            // overrides
            public override int maxstate() { return 255; }


            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 14-segment LCD
        class led14seg_component : component
        {
            // construction/destruction
            led14seg_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
            }


            // overrides
            public override int maxstate() { return 16383; }


            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 16-segment LCD
        class led16seg_component : component
        {
            // construction/destruction
            led16seg_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
            }


            // overrides
            public override int maxstate() { return 65535; }


            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 14-segment LCD with semicolon (2 extra segments)
        class led14segsc_component : component
        {
            // construction/destruction
            led14segsc_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
            }


            // overrides
            public override int maxstate() { return 65535; }


            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // 16-segment LCD with semicolon (2 extra segments)
        class led16segsc_component : component
        {
            // construction/destruction
            led16segsc_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
            }


            // overrides
            public override int maxstate() { return 262143; }


            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // row of dots for a dotmatrix
        class dotmatrix_component : component
        {
            // internal state
            int m_dots;


            // construction/destruction
            public dotmatrix_component(int dots, layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
                m_dots = dots;
            }


            // overrides
            public override int maxstate() { return (1 << m_dots) - 1; }


            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // simple counter
        class simplecounter_component : component
        {
            // internal state
            int m_digits;                   // number of digits for simple counters
            int m_textalign;                // text alignment to box
            int m_maxstate;


            // construction/destruction
            simplecounter_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
                m_digits = env.get_attribute_int(compnode, "digits", 2);
                m_textalign = env.get_attribute_int(compnode, "align", 0);
                m_maxstate = env.get_attribute_int(compnode, "maxstate", 999);
            }


            // overrides
            public override int maxstate() { return m_maxstate; }


            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }
        }


        // fruit machine reel
        class reel_component : component
        {
            const int MAX_BITMAPS = 32;

            // internal state
            //bitmap_argb32       m_bitmap[MAX_BITMAPS];      // source bitmap for images
            string m_searchpath;               // asset search path (for lazy loading)
            string m_dirname;                  // directory name of image file (for lazy loading)
            string [] m_imagefile = new string [MAX_BITMAPS];   // name of the image file (for lazy loading)

            // basically made up of multiple text strings / gfx
            int m_numstops;
            string [] m_stopnames = new string [MAX_BITMAPS];
            int m_stateoffset;
            int m_reelreversed;
            int m_numsymbolsvisible;
            int m_beltreel;


            // construction/destruction
            reel_component(layout_element_environment env, util.xml.data_node compnode)
                : base(env, compnode)
            {
                m_searchpath = env.search_path() != null ? env.search_path() : "";
                m_dirname = env.directory_name() != null ? env.directory_name() : "";


                string symbollist = env.get_attribute_string(compnode, "symbollist", "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14,15");

                // split out position names from string and figure out our number of symbols
                m_numstops = 0;
                for (var location = symbollist.find(','); g.npos != location; location = symbollist.find(','))  //for (std::string::size_type location = symbollist.find(','); std::string::npos != location; location = symbollist.find(','))
                {
                    m_stopnames[m_numstops] = symbollist.substr(0, location);
                    symbollist = symbollist.Substring((int)location + 1);  //symbollist.remove_prefix(location + 1);
                    m_numstops++;
                }

                m_stopnames[m_numstops++] = symbollist;

                for (int i = 0; i < m_numstops; i++)
                {
                    var location = m_stopnames[i].find(':');  //std::string::size_type const location = m_stopnames[i].find(':');
                    if (location != g.npos)
                    {
                        m_imagefile[i] = m_stopnames[i].substr(location + 1);
                        m_stopnames[i] = m_stopnames[i].Remove((int)location, 1);  //m_stopnames[i].erase(location);
                    }
                }

                m_stateoffset = env.get_attribute_int(compnode, "stateoffset", 0);
                m_numsymbolsvisible = env.get_attribute_int(compnode, "numsymbolsvisible", 3);
                m_reelreversed = env.get_attribute_int(compnode, "reelreversed", 0);
                m_beltreel = env.get_attribute_int(compnode, "beltreel", 0);
            }


            // overrides

            public override void preload(running_machine machine)
            {
                throw new emu_unimplemented();
            }


            public override int maxstate() { return 65535; }


            protected override void draw_aligned(running_machine machine, bitmap_argb32 dest, rectangle bounds, int state)
            {
                throw new emu_unimplemented();
            }


            // internal helpers
            //void draw_beltreel(running_machine &machine, bitmap_argb32 &dest, const rectangle &bounds, int state);
            //void load_reel_bitmap(int number);
        }


        static readonly layout_element_make_component_map s_make_component = new layout_element_make_component_map()
        {
            { "image",         make_component<image_component>         },
            { "text",          make_component<text_component>          },
            { "dotmatrix",     make_dotmatrix_component<int_const_8>   },
            { "dotmatrix5dot", make_dotmatrix_component<int_const_5>   },
            { "dotmatrixdot",  make_dotmatrix_component<int_const_1>   },
            { "simplecounter", make_component<simplecounter_component> },
            { "reel",          make_component<reel_component>          },
            { "led7seg",       make_component<led7seg_component>       },
            { "led8seg_gts1",  make_component<led8seg_gts1_component>  },
            { "led14seg",      make_component<led14seg_component>      },
            { "led14segsc",    make_component<led14segsc_component>    },
            { "led16seg",      make_component<led16seg_component>      },
            { "led16segsc",    make_component<led16segsc_component>    },
            { "rect",          make_component<rect_component>          },
            { "disk",          make_component<disk_component>          }
        };


        // internal state
        running_machine m_machine;      // reference to the owning machine
        std.vector<component> m_complist = new std.vector<component>();     // list of components  //std::vector<component::ptr> m_complist;     // list of components
        int m_defstate;     // default state of this element
        int m_statemask;    // mask to apply to state values
        bool m_foldhigh;     // whether we need to fold state values above the mask range
        std.vector<texture> m_elemtex = new std.vector<texture>();      // array of element textures used for managing the scaled bitmaps


        // construction/destruction
        //-------------------------------------------------
        //  layout_element - constructor
        //-------------------------------------------------
        public layout_element(layout_element_environment env, util.xml.data_node elemnode)
        {
            m_machine = env.machine();
            m_defstate = env.get_attribute_int(elemnode, "defstate", -1);
            m_statemask = 0;
            m_foldhigh = false;


            // parse components in order
            bool first = true;
            render_bounds bounds = new render_bounds() { x0 = 0.0f, y0 = 0.0f, x1 = 0.0f, y1 = 0.0f };
            for (util.xml.data_node compnode = elemnode.get_first_child(); compnode != null; compnode = compnode.get_next_sibling())
            {
                var make_func = s_make_component.find(compnode.get_name());
                if (make_func == null)
                    throw new layout_syntax_error(util.string_format("unknown element component {0}", compnode.get_name()));

                // insert the new component into the list
                //component const &newcomp(*m_complist.emplace_back(make_func->second(env, *compnode)));
                component newcomp = make_func(env, compnode);
                m_complist.push_back(newcomp);

                // accumulate bounds
                if (first)
                    bounds = newcomp.overall_bounds();
                else
                    bounds |= newcomp.overall_bounds();

                first = false;

                // determine the maximum state
                std.pair<int, bool> wrap = newcomp.statewrap();
                m_statemask |= wrap.first;
                m_foldhigh = m_foldhigh || wrap.second;
            }

            if (!m_complist.empty())
            {
                // determine the scale/offset for normalization
                float xoffs = bounds.x0;
                float yoffs = bounds.y0;
                float xscale = 1.0f / (bounds.x1 - bounds.x0);
                float yscale = 1.0f / (bounds.y1 - bounds.y0);

                // normalize all the component bounds
                foreach (var curcomp in m_complist)
                    curcomp.normalize_bounds(xoffs, yoffs, xscale, yscale);
            }

            // allocate an array of element textures for the states
            m_elemtex.resize(((u32)m_statemask + 1) << (m_foldhigh ? 1 : 0));
        }

        //virtual ~layout_element();


        // getters

        public running_machine machine() { return m_machine; }
        public int default_state() { return m_defstate; }


        //-------------------------------------------------
        //  state_texture - return a pointer to a
        //  render_texture for the given state, allocating
        //  one if needed
        //-------------------------------------------------
        public render_texture state_texture(int state)
        {
            if (m_foldhigh && (state & ~m_statemask) != 0)
                state = (state & m_statemask) | (((m_statemask << 1) | 1) & ~m_statemask);
            else
                state &= m_statemask;

            g.assert((int)m_elemtex.size() > state);

            if (m_elemtex[state].m_texture == null)
            {
                m_elemtex[state].m_element = this;
                m_elemtex[state].m_state = state;
                m_elemtex[state].m_texture = machine().render().texture_alloc(element_scale, m_elemtex[state]);
            }

            return m_elemtex[state].m_texture;
        }


        // operations

        //-------------------------------------------------
        //  preload - perform expensive loading upfront
        //  for all components
        //-------------------------------------------------
        public void preload()
        {
            foreach (component curcomp in m_complist)
                curcomp.preload(machine());
        }


        // internal helpers

        //-------------------------------------------------
        //  element_scale - scale an element by rendering
        //  all the components at the appropriate
        //  resolution
        //-------------------------------------------------
        static void element_scale(bitmap_argb32 dest, bitmap_argb32 source, rectangle sbounds, layout_element.texture param)  //static void element_scale(bitmap_argb32 &dest, bitmap_argb32 &source, const rectangle &sbounds, void *param);
        {
            texture elemtex = (texture)param;

            // draw components that are visible in the current state
            foreach (var curcomp in elemtex.m_element.m_complist)
            {
                if ((elemtex.m_state & curcomp.statemask()) == curcomp.stateval())
                    curcomp.draw(elemtex.m_element.machine(), dest, elemtex.m_state);
            }
        }


        //-------------------------------------------------
        //  make_component - create component of given type
        //-------------------------------------------------
        //template <typename T>
        static component make_component<T>(layout_element_environment env, util.xml.data_node compnode) where T : component  //template <typename T> static component::ptr make_component(environment &env, util::xml::data_node const &compnode);
        {
            // return std::make_unique<T>(env, compnode);
            if (typeof(T) == typeof(image_component))
                return new image_component(env, compnode);
            else
                throw new emu_unimplemented();
        }


        //template <int D>
        static component make_dotmatrix_component<int_D>(layout_element_environment env, util.xml.data_node compnode)  //static component::ptr make_dotmatrix_component(environment &env, util::xml::data_node const &compnode);
            where int_D : int_const, new()
        {
            int D = new int_D().value;
            return new dotmatrix_component(D, env, compnode);  //return std::make_unique<dotmatrix_component>(D, env, compnode);
        }
    }


    /// \brief A reusable group of elements
    ///
    /// Views expand/flatten groups into their component elements applying
    /// an optional coordinate transform.  This is mainly useful duplicating
    /// the same sublayout in multiple views.  It would be more useful
    /// within a view if it could be parameterised.  Groups only exist while
    /// parsing a layout file - no information about element grouping is
    /// preserved.
    public class layout_group
    {
        //using environment = emu::render::detail::layout_environment;
        //using group_map = std::unordered_map<std::string, layout_group>;
        //using transform = std::array<std::array<float, 3>, 3>;


        util.xml.data_node m_groupnode;
        render_bounds m_bounds;
        bool m_bounds_resolved;


        //-------------------------------------------------
        //  layout_group - constructor
        //-------------------------------------------------
        public layout_group(util.xml.data_node groupnode)
        {
            m_groupnode = groupnode;
            m_bounds = new render_bounds() { x0 = 0.0f, y0 = 0.0f, x1 = 0.0f, y1 = 0.0f };
            m_bounds_resolved = false;
        }

        //~layout_group();


        public util.xml.data_node get_groupnode() { return m_groupnode; }


        //transform make_transform(int orientation, render_bounds const &dest) const;
        //transform make_transform(int orientation, transform const &trans) const;
        //transform make_transform(int orientation, render_bounds const &dest, transform const &trans) const;
        //-------------------------------------------------
        //  make_transform - create abbreviated transform
        //  matrix for given destination bounds
        //-------------------------------------------------
        public layout_group_transform make_transform(int orientation, render_bounds dest)
        {
            g.assert(m_bounds_resolved);

            // make orientation matrix
            layout_group_transform result = new layout_group_transform(new std.array<float, u64_const_3>(1.0F, 0.0F, 0.0F), new std.array<float, u64_const_3>(0.0F, 1.0F, 0.0F), new std.array<float, u64_const_3>(0.0F, 0.0F, 1.0F));  //transform result{{ {{ 1.0F, 0.0F, 0.0F }}, {{ 0.0F, 1.0F, 0.0F }}, {{ 0.0F, 0.0F, 1.0F }} }};
            if ((orientation & g.ORIENTATION_SWAP_XY) != 0)
            {
                var temp1 = result[0][0]; result[0][0] = result[0][1];  result[0][1] = temp1;  //std::swap(result[0][0], result[0][1]);
                var temp2 = result[1][0]; result[1][0] = result[1][1];  result[1][1] = temp2;  //std::swap(result[1][0], result[1][1]);
            }

            if ((orientation & g.ORIENTATION_FLIP_X) != 0)
            {
                result[0][0] = -result[0][0];
                result[0][1] = -result[0][1];
            }

            if ((orientation & g.ORIENTATION_FLIP_Y) != 0)
            {
                result[1][0] = -result[1][0];
                result[1][1] = -result[1][1];
            }

            // apply to bounds and force into destination rectangle
            render_bounds bounds = new render_bounds(m_bounds);
            rendlay_global.render_bounds_transform(ref bounds, result);
            result[0][0] *= (dest.x1 - dest.x0) / std.fabs(bounds.x1 - bounds.x0);
            result[0][1] *= (dest.x1 - dest.x0) / std.fabs(bounds.x1 - bounds.x0);
            result[0][2] = dest.x0 - (std.min(bounds.x0, bounds.x1) * (dest.x1 - dest.x0) / std.fabs(bounds.x1 - bounds.x0));
            result[1][0] *= (dest.y1 - dest.y0) / std.fabs(bounds.y1 - bounds.y0);
            result[1][1] *= (dest.y1 - dest.y0) / std.fabs(bounds.y1 - bounds.y0);
            result[1][2] = dest.y0 - (std.min(bounds.y0, bounds.y1) * (dest.y1 - dest.y0) / std.fabs(bounds.y1 - bounds.y0));
            return result;
        }


        public layout_group_transform make_transform(int orientation, layout_group_transform trans)  //layout_group::transform layout_group::make_transform(int orientation, transform const &trans) const
        {
            g.assert(m_bounds_resolved);

            render_bounds dest = new render_bounds()
            {
                x0 = m_bounds.x0,
                y0 = m_bounds.y0,
                x1 = (orientation & g.ORIENTATION_SWAP_XY) != 0 ? (m_bounds.x0 + m_bounds.y1 - m_bounds.y0) : m_bounds.x1,
                y1 = (orientation & g.ORIENTATION_SWAP_XY) != 0 ? (m_bounds.y0 + m_bounds.x1 - m_bounds.x0) : m_bounds.y1
            };
            return make_transform(orientation, dest, trans);
        }


        public layout_group_transform make_transform(int orientation, render_bounds dest, layout_group_transform trans)  //layout_group::transform layout_group::make_transform(int orientation, render_bounds const &dest, transform const &trans) const
        {
            layout_group_transform next = make_transform(orientation, dest);
            layout_group_transform result = new layout_group_transform(new std.array<float, u64_const_3>(0.0F, 0.0F, 0.0F), new std.array<float, u64_const_3>(0.0F, 0.0F, 0.0F), new std.array<float, u64_const_3>(0.0F, 0.0F, 0.0F));  //transform result{{ {{ 0.0F, 0.0F, 0.0F }}, {{ 0.0F, 0.0F, 0.0F }}, {{ 0.0F, 0.0F, 0.0F }} }};
            for (unsigned y = 0; 3U > y; ++y)
            {
                for (unsigned x = 0; 3U > x; ++x)
                {
                    for (unsigned i = 0; 3U > i; ++i)
                        result[y][x] += trans[y][i] * next[i][x];
                }
            }

            return result;
        }


        public void set_bounds_unresolved()
        {
            m_bounds_resolved = false;
        }


        public void resolve_bounds(layout_group_environment env, layout_group_group_map groupmap)
        {
            if (!m_bounds_resolved)
            {
                std.vector<layout_group> seen = new std.vector<layout_group>();
                resolve_bounds(env, groupmap, seen);
            }
        }


        void resolve_bounds(layout_group_environment env, layout_group_group_map groupmap, std.vector<layout_group> seen)
        {
            if (seen.Contains(this))  //if (seen.end() != std::find(seen.begin(), seen.end(), this))
            {
                // a wild loop appears!
                string path = "";  //std::ostringstream path;
                foreach (layout_group group in seen)
                    path += ' ' + group.m_groupnode.get_attribute_string("name", "");  //path << ' ' << group->m_groupnode.get_attribute_string("name", "");
                path += ' ' + m_groupnode.get_attribute_string("name", "");  //path << ' ' << m_groupnode.get_attribute_string("name", "");
                throw new layout_syntax_error(util.string_format("recursively nested groups {0}", path));
            }

            seen.push_back(this);
            if (!m_bounds_resolved)
            {
                m_bounds.set_xy(0.0F, 0.0F, 1.0F, 1.0F);
                layout_group_environment local = new layout_group_environment(env);
                bool empty = true;
                resolve_bounds(local, m_groupnode, groupmap, seen, ref empty, false, false, true);
            }
            seen.pop_back();
        }


        void resolve_bounds(
                layout_group_environment env,
                util.xml.data_node parentnode,
                layout_group_group_map groupmap,
                std.vector<layout_group> seen,
                ref bool empty,
                bool vistoggle,
                bool repeat,
                bool init)
        {
            rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Group '{0}' resolve bounds empty={1} vistoggle={2} repeat={3} init={4}\n",
                parentnode.get_attribute_string("name", ""), empty, vistoggle, repeat, init);

            bool envaltered = false;
            bool unresolved = true;
            for (util.xml.data_node itemnode = parentnode.get_first_child(); !m_bounds_resolved && itemnode != null; itemnode = itemnode.get_next_sibling())
            {
                if (std.strcmp(itemnode.get_name(), "bounds") == 0)
                {
                    // use explicit bounds
                    env.parse_bounds(itemnode, out m_bounds);
                    m_bounds_resolved = true;
                }
                else if (std.strcmp(itemnode.get_name(), "param") == 0)
                {
                    envaltered = true;
                    if (!unresolved)
                    {
                        rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Environment altered{0}, unresolving groups\n", envaltered ? " again" : "");
                        unresolved = true;
                        foreach (var group in groupmap)
                            group.second().set_bounds_unresolved();
                    }

                    if (!repeat)
                        env.set_parameter(itemnode);
                    else
                        env.set_repeat_parameter(itemnode, init);
                }
                else if (std.strcmp(itemnode.get_name(), "element") == 0 ||
                    std.strcmp(itemnode.get_name(), "backdrop") == 0 ||
                    std.strcmp(itemnode.get_name(), "screen") == 0 ||
                    std.strcmp(itemnode.get_name(), "overlay") == 0 ||
                    std.strcmp(itemnode.get_name(), "bezel") == 0 ||
                    std.strcmp(itemnode.get_name(), "cpanel") == 0 ||
                    std.strcmp(itemnode.get_name(), "marquee") == 0)
                {
                    render_bounds itembounds;
                    util.xml.data_node boundsnode = itemnode.get_child("bounds");
                    env.parse_bounds(boundsnode, out itembounds);
                    while (boundsnode != null)
                    {
                        boundsnode = boundsnode.get_next_sibling("bounds");
                        if (boundsnode != null)
                        {
                            render_bounds b;
                            env.parse_bounds(boundsnode, out b);
                            itembounds |= b;
                        }
                    }

                    if (empty)
                        m_bounds = itembounds;
                    else
                        m_bounds |= itembounds;

                    empty = false;

                    rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Accumulate item bounds ({0} {1} {2} {3}) -> ({4} {5} {6} {7})\n",
                        itembounds.x0, itembounds.y0, itembounds.x1, itembounds.y1,
                        m_bounds.x0, m_bounds.y0, m_bounds.x1, m_bounds.y1);
                }
                else if (std.strcmp(itemnode.get_name(), "group") == 0)
                {
                    util.xml.data_node itemboundsnode = itemnode.get_child("bounds");
                    if (itemboundsnode != null)
                    {
                        render_bounds itembounds;
                        env.parse_bounds(itemboundsnode, out itembounds);
                        if (empty)
                            m_bounds = itembounds;
                        else
                            m_bounds |= itembounds;

                        empty = false;

                        rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Accumulate group '{0}' reference explicit bounds ({1} {2} {3} {4}) -> ({5} {6} {7} {8})\n",
                            itemnode.get_attribute_string("ref", ""),
                            itembounds.x0, itembounds.y0, itembounds.x1, itembounds.y1,
                            m_bounds.x0, m_bounds.y0, m_bounds.x1, m_bounds.y1);
                    }
                    else
                    {
                        string ref_ = env.get_attribute_string(itemnode, "ref");
                        if (ref_.empty())
                            throw new layout_syntax_error("nested group must have non-empty ref attribute");

                        var found = groupmap.find(ref_);
                        if (found == null)
                            throw new layout_syntax_error(util.string_format("unable to find group {0}", ref_));

                        int orientation = env.parse_orientation(itemnode.get_child("orientation"));
                        layout_group_environment local = new layout_group_environment(env);
                        found.resolve_bounds(local, groupmap, seen);
                        render_bounds itembounds = new render_bounds()
                        {
                            x0 = found.m_bounds.x0,
                            y0 = found.m_bounds.y0,
                            x1 = (orientation & g.ORIENTATION_SWAP_XY) != 0 ? (found.m_bounds.x0 + found.m_bounds.y1 - found.m_bounds.y0) : found.m_bounds.x1,
                            y1 = (orientation & g.ORIENTATION_SWAP_XY) != 0 ? (found.m_bounds.y0 + found.m_bounds.x1 - found.m_bounds.x0) : found.m_bounds.y1
                        };

                        if (empty)
                            m_bounds = itembounds;
                        else
                            m_bounds |= itembounds;

                        empty = false;

                        unresolved = false;
                        rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Accumulate group '{0}' reference computed bounds ({1} {2} {3} {4}) -> ({5} {6} {7} {8})\n",
                            itemnode.get_attribute_string("ref", ""),
                            itembounds.x0, itembounds.y0, itembounds.x1, itembounds.y1,
                            m_bounds.x0, m_bounds.y0, m_bounds.x1, m_bounds.y1);
                    }
                }
                else if (std.strcmp(itemnode.get_name(), "repeat") == 0)
                {
                    int count = env.get_attribute_int(itemnode, "count", -1);
                    if (0 >= count)
                        throw new layout_syntax_error("repeat must have positive integer count attribute");

                    layout_group_environment local = new layout_group_environment(env);
                    for (int i = 0; !m_bounds_resolved && (count > i); ++i)
                    {
                        resolve_bounds(local, itemnode, groupmap, seen, ref empty, false, true, i == 0);
                        local.increment_parameters();
                    }
                }
                else if (std.strcmp(itemnode.get_name(), "collection") == 0)
                {
                    if (!itemnode.has_attribute("name"))
                        throw new layout_syntax_error("collection must have name attribute");
                    layout_group_environment local = env;
                    resolve_bounds(local, itemnode, groupmap, seen, ref empty, true, false, true);
                }
                else
                {
                    throw new layout_syntax_error(util.string_format("unknown group element {0}", itemnode.get_name()));
                }
            }

            if (envaltered && !unresolved)
            {
                rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Environment was altered, marking groups unresolved\n");
                bool resolved = m_bounds_resolved;
                foreach (var group in groupmap)
                    group.second().set_bounds_unresolved();
                m_bounds_resolved = resolved;
            }

            if (!vistoggle && !repeat)
            {
                rendlay_global.LOGMASKED(rendlay_global.LOG_GROUP_BOUNDS_RESOLUTION, "Marking group '{0}' bounds resolved\n", parentnode.get_attribute_string("name", ""));
                m_bounds_resolved = true;
            }
        }
    }


    /// \brief A single view within a layout_file
    ///
    /// The view is described using arbitrary coordinates that are scaled to
    /// fit within the render target.  Pixels within a view are assumed to
    /// be square.
    public class layout_view
    {
        //using layout_environment = emu::render::detail::layout_environment;
        //using view_environment = emu::render::detail::view_environment;
        //using element_map = std::unordered_map<std::string, layout_element>;
        //using group_map = std::unordered_map<std::string, layout_group>;
        //using screen_ref_vector = std::vector<std::reference_wrapper<screen_device const>>;
        delegate void prepare_items_delegate();  //using prepare_items_delegate = delegate<void ()>;
        delegate void preload_delegate();  //using preload_delegate = delegate<void ()>;
        delegate void recomputed_delegate();  //using recomputed_delegate = delegate<void ()>;
        //using item_id_map = std::unordered_map<
        //        std::reference_wrapper<std::string const>,
        //        item &,
        //        std::hash<std::string>,
        //        std::equal_to<std::string> >;
        //using item_list = std::list<item>;
        //using item_ref_vector = std::vector<std::reference_wrapper<item> >;


        /// \brief A single item in a view
        ///
        /// Each view has a list of item structures describing the visual
        /// elements to draw, where they are located, additional blending
        /// modes, and bindings for inputs and outputs.
        public class item
        {
            //friend class layout_view;

            delegate int state_delegate();  //using state_delegate = delegate<int ()>;
            delegate void bounds_delegate(out render_bounds bounds);  //using bounds_delegate = delegate<void (render_bounds &)>;
            delegate void color_delegate(out render_color color);  //using color_delegate = delegate<void (render_color &)>;
            //using bounds_vector = emu::render::detail::bounds_vector;
            //using color_vector = emu::render::detail::color_vector;


            // internal state
            layout_element m_element;          // pointer to the associated element (non-screens only)
            state_delegate m_get_elem_state;   // resolved element state function
            state_delegate m_get_anim_state;   // resolved animation state function
            bounds_delegate m_get_bounds;       // resolved bounds function
            color_delegate m_get_color;        // resolved color function
            output_finder<u32_const_1> m_output;           // associated output  //output_finder<>         m_output;           // associated output
            output_finder<u32_const_1> m_animoutput;       // associated output for animation if different  //output_finder<>         m_animoutput;       // associated output for animation if different
            ioport_port m_animinput_port;   // input port used for animation
            int m_elem_state;       // element state used in absence of bindings
            ioport_value m_animmask;         // mask for animation state
            u8 m_animshift;        // shift for animation state
            ioport_port m_input_port;       // input port of this item
            ioport_field m_input_field;      // input port field of this item
            ioport_value m_input_mask;       // input mask of this item
            u8 m_input_shift;      // input mask rightshift for raw (trailing 0s)
            bool m_clickthrough;     // should click pass through to lower elements
            screen_device m_screen;           // pointer to screen
            int m_orientation;      // orientation of this item
            public emu_render_detail_bounds_vector m_bounds;           // bounds of the item
            emu_render_detail_color_vector m_color;            // color of the item
            public int m_blend_mode;       // blending mode to use when drawing
            public u32 m_visibility_mask;  // combined mask of parent visibility groups

            // cold items
            string m_id;               // optional unique item identifier
            string m_input_tag;        // input tag of this item
            string m_animinput_tag;    // tag of input port for animation state
            public emu_render_detail_bounds_vector m_rawbounds;        // raw (original) bounds of the item
            bool m_have_output;      // whether we actually have an output
            bool m_input_raw;        // get raw data from input port
            bool m_have_animoutput;  // whether we actually have an output for animation
            bool m_has_clickthrough; // whether clickthrough was explicitly configured


            // construction/destruction
            //-------------------------------------------------
            //  item - constructor
            //-------------------------------------------------
            public item(
                    layout_view_view_environment env,
                    util.xml.data_node itemnode,
                    layout_view_element_map elemmap,
                    int orientation,
                    layout_group_transform trans,
                    render_color color)
            {
                m_element = find_element(env, itemnode, elemmap);

                //throw new emu_unimplemented();
#if false
                m_output(env.device(), std::string(env.get_attribute_string(itemnode, "name")))
                m_animoutput(env.device(), make_animoutput_tag(env, itemnode));
#endif

                m_animinput_port = null;
                m_elem_state = m_element != null ? m_element.default_state() : 0;
                m_animmask = make_animmask(env, itemnode);
                m_animshift = (u8)get_state_shift(m_animmask);
                m_input_port = null;
                m_input_field = null;
                m_input_mask = (ioport_value)env.get_attribute_int(itemnode, "inputmask", 0);
                m_input_shift = (u8)get_state_shift(m_input_mask);
                m_clickthrough = env.get_attribute_bool(itemnode, "clickthrough", true);  //m_clickthrough(env.get_attribute_bool(itemnode, "clickthrough", "yes"))
                m_screen = null;
                m_color = make_color(env, itemnode, color);
                m_blend_mode = get_blend_mode(env, itemnode);
                m_visibility_mask = env.visibility_mask();
                m_id = env.get_attribute_string(itemnode, "id");
                m_input_tag = make_input_tag(env, itemnode);
                m_animinput_tag = make_animinput_tag(env, itemnode);
                m_rawbounds = make_bounds(env, itemnode, trans);
                m_have_output = !env.get_attribute_string(itemnode, "name").empty();  //m_have_output(env.get_attribute_string(itemnode, "name", "")[0])
                m_input_raw = env.get_attribute_bool(itemnode, "inputraw", false);
                m_have_animoutput = !make_animoutput_tag(env, itemnode).empty();
                m_has_clickthrough = !env.get_attribute_string(itemnode, "clickthrough").empty();  //m_has_clickthrough(env.get_attribute_string(itemnode, "clickthrough", "")[0])


                // fetch common data
                int index = env.get_attribute_int(itemnode, "index", -1);
                if (index != -1)
                    m_screen = new screen_device_enumerator(env.machine().root_device()).byindex(index);

                // sanity checks
                if (std.strcmp(itemnode.get_name(), "screen") == 0)
                {
                    if (itemnode.has_attribute("tag"))
                    {
                        string tag = env.get_attribute_string(itemnode, "tag");
                        m_screen = (screen_device)env.device().subdevice(tag);
                        if (m_screen == null)
                            throw new layout_reference_error(util.string_format("invalid screen tag '{0}'", tag));
                    }
                    else if (m_screen == null)
                    {
                        throw new layout_reference_error(util.string_format("invalid screen index {0}", index));
                    }
                }
                else if (m_element == null)
                {
                    throw new layout_syntax_error(util.string_format("item of type {0} requires an element tag", itemnode.get_name()));
                }

                // this can be called before resolving tags, make it return something valid
                m_bounds = m_rawbounds;
                m_get_bounds = (out render_bounds bounds) => { m_bounds.front().get(out bounds); };  //m_get_bounds = bounds_delegate(&emu::render::detail::bounds_step::get, &m_bounds.front());
            }

            //~item();


            // getters
            public string id() { return m_id; }
            public layout_element element() { return m_element; }
            public screen_device screen() { return m_screen; }
            //bool bounds_animated() const { return m_bounds.size() > 1U; }
            //bool color_animated() const { return m_color.size() > 1U; }
            public render_bounds bounds() { render_bounds result; m_get_bounds(out result); return result; }
            public render_color color() { render_color result; m_get_color(out result); return result; }
            public int blend_mode() { return m_blend_mode; }
            public u32 visibility_mask() { return m_visibility_mask; }
            public int orientation() { return m_orientation; }
            //render_container *screen_container() const { return m_screen ? &m_screen->container() : nullptr; }


            // interactivity
            public bool has_input() { return m_input_port != null; }  //bool has_input() const { return bool(m_input_port); }
            public std.pair<ioport_port, ioport_value> input_tag_and_mask() { return std.make_pair(m_input_port, m_input_mask); }
            public bool clickthrough() { return m_clickthrough; }


            // fetch state based on configured source
            public int element_state() { return m_get_elem_state(); }
            //int animation_state() const { return m_get_anim_state(); }


            // set state
            //void set_state(int state) { m_elem_state = state; }


            // set handlers
            //void set_element_state_callback(state_delegate &&handler);
            //void set_animation_state_callback(state_delegate &&handler);
            //void set_bounds_callback(bounds_delegate &&handler);
            //void set_color_callback(color_delegate &&handler);


            // resolve tags, if any
            //---------------------------------------------
            //  resolve_tags - resolve tags, if any are set
            //---------------------------------------------
            public void resolve_tags()
            {
                // resolve element state output and set default value
                if (m_have_output)
                {
                    throw new emu_unimplemented();
#if false
                    m_output.resolve();
                    if (m_element)
                        m_output = m_element->default_state();
#endif
                }

                // resolve animation state output
                if (m_have_animoutput)
                {
                    throw new emu_unimplemented();
#if false
                    m_animoutput.resolve();
#endif
                }

                // resolve animation state input
                if (!m_animinput_tag.empty())
                    m_animinput_port = m_element.machine().root_device().ioport(m_animinput_tag);

                // resolve element state input
                if (!m_input_tag.empty())
                {
                    m_input_port = m_element.machine().root_device().ioport(m_input_tag);
                    if (m_input_port != null)
                    {
                        // if there's a matching unconditional field, cache it
                        foreach (ioport_field field in m_input_port.fields())
                        {
                            if ((field.mask() & m_input_mask) != 0)
                            {
                                if (field.condition().condition() == ioport_condition.condition_t.ALWAYS)
                                    m_input_field = field;

                                break;
                            }
                        }

                        // if clickthrough isn't explicitly configured, having an I/O port implies false
                        if (!m_has_clickthrough)
                            m_clickthrough = false;
                    }
                }

                // choose optimal handlers
                m_get_elem_state = default_get_elem_state();
                m_get_anim_state = default_get_anim_state();
                m_get_bounds = default_get_bounds();
                m_get_color = default_get_color();
            }


            //-------------------------------------------------
            //  default_get_elem_state - get default element
            //  state handler
            //-------------------------------------------------
            state_delegate default_get_elem_state()
            {
                if (m_have_output)
                    return get_output;  //return state_delegate(&item::get_output, this);
                else if (m_input_port == null)
                    return get_state;  //return state_delegate(&item::get_state, this);
                else if (m_input_raw)
                    return get_input_raw;  //return state_delegate(&item::get_input_raw, this);
                else if (m_input_field != null)
                    return get_input_field_cached;  //return state_delegate(&item::get_input_field_cached, this);
                else
                    return get_input_field_conditional;  //return state_delegate(&item::get_input_field_conditional, this);
            }


            //-------------------------------------------------
            //  default_get_anim_state - get default animation
            //  state handler
            //-------------------------------------------------
            state_delegate default_get_anim_state()
            {
                if (m_have_animoutput)
                    return get_anim_output;  //return state_delegate(&item::get_anim_output, this);
                else if (m_animinput_port != null)
                    return get_anim_input;  //return state_delegate(&item::get_anim_input, this);
                else
                    return default_get_elem_state();
            }


            //-------------------------------------------------
            //  default_get_bounds - get default bounds handler
            //-------------------------------------------------
            bounds_delegate default_get_bounds()
            {
                return (m_bounds.size() == 1U)
                        ? (out render_bounds bounds) => { m_bounds.front().get(out bounds); }  //? bounds_delegate(&emu::render::detail::bounds_step::get, &m_bounds.front())
                        : (bounds_delegate)get_interpolated_bounds;  //: bounds_delegate(&item::get_interpolated_bounds, this);
            }


            //-------------------------------------------------
            //  default_get_color - get default color handler
            //-------------------------------------------------
            color_delegate default_get_color()
            {
                return (m_color.size() == 1U)
                        ? (out render_color result) => { m_color.front().get(out result); }  //? color_delegate(&emu::render::detail::color_step::get, &const_cast<emu::render::detail::color_step &>(m_color.front()))
                        : (color_delegate)get_interpolated_color;  //: color_delegate(&item::get_interpolated_color, this);
            }


            //-------------------------------------------------
            //  get_state - get state when no bindings
            //-------------------------------------------------
            int get_state()
            {
                return m_elem_state;
            }


            //-------------------------------------------------
            //  get_output - get element state output
            //-------------------------------------------------
            int get_output()
            {
                g.assert(m_have_output);
                return (int)(s32)m_output.op.op;
            }


            //-------------------------------------------------
            //  get_input_raw - get element state input
            //-------------------------------------------------
            int get_input_raw()
            {
                g.assert(m_input_port != null);
                return (int)(s32)((m_input_port.read() & m_input_mask) >> m_input_shift);  //return int(std::make_signed_t<ioport_value>((m_input_port->read() & m_input_mask) >> m_input_shift));
            }


            //-------------------------------------------------
            //  get_input_field_cached - element state
            //-------------------------------------------------
            int get_input_field_cached()
            {
                g.assert(m_input_port != null);
                g.assert(m_input_field != null);
                return ((m_input_port.read() ^ m_input_field.defvalue()) & m_input_mask) != 0 ? 1 : 0;
            }


            //-------------------------------------------------
            //  get_input_field_conditional - element state
            //-------------------------------------------------
            int get_input_field_conditional()
            {
                g.assert(m_input_port != null);
                g.assert(m_input_field == null);
                ioport_field field = m_input_port.field(m_input_mask);
                return (field != null && ((m_input_port.read() ^ field.defvalue()) & m_input_mask) != 0) ? 1 : 0;
            }


            //-------------------------------------------------
            //  get_anim_output - get animation output
            //-------------------------------------------------
            int get_anim_output()
            {
                g.assert(m_have_animoutput);
                return (int)(unsigned)((u32)((s32)m_animoutput.op.op & m_animmask) >> m_animshift);
            }


            //-------------------------------------------------
            //  get_anim_input - get animation input
            //-------------------------------------------------
            int get_anim_input()
            {
                g.assert(m_animinput_port != null);
                return (int)(s32)((m_animinput_port.read() & m_animmask) >> m_animshift);  //return int(std::make_signed_t<ioport_value>((m_animinput_port->read() & m_animmask) >> m_animshift));
            }


            //-------------------------------------------------
            //  get_interpolated_bounds - animated bounds
            //-------------------------------------------------
            void get_interpolated_bounds(out render_bounds result)
            {
                g.assert(m_bounds.size() > 1U);
                result = rendlay_global.interpolate_bounds(m_bounds, m_get_anim_state());
            }


            //-------------------------------------------------
            //  get_interpolated_color - animated color
            //-------------------------------------------------
            void get_interpolated_color(out render_color result)
            {
                g.assert(m_color.size() > 1U);
                result = rendlay_global.interpolate_color(m_color, m_get_anim_state());
            }


            //---------------------------------------------
            //  find_element - find element definition
            //---------------------------------------------
            static layout_element find_element(layout_view_view_environment env, util.xml.data_node itemnode, layout_view_element_map elemmap)
            {
                string name = env.get_attribute_string(itemnode, std.strcmp(itemnode.get_name(), "element") == 0 ? "ref" : "element");
                if (name.empty())
                    return null;

                // search the list of elements for a match, error if not found
                var found = elemmap.find(name);
                if (null != found)
                    return found;
                else
                    throw new layout_syntax_error("unable to find element {0}", name);
            }


            //---------------------------------------------
            //  make_bounds - get transformed bounds
            //---------------------------------------------
            static layout_view_item_bounds_vector make_bounds(
                    layout_view_view_environment env,
                    util.xml.data_node itemnode,
                    layout_group_transform trans)
            {
                layout_view_item_bounds_vector result = new emu_render_detail_bounds_vector();
                for (util.xml.data_node bounds = itemnode.get_child("bounds"); bounds != null; bounds = bounds.get_next_sibling("bounds"))
                {
                    if (!rendlay_global.add_bounds_step(env, result, bounds))
                    {
                        throw new layout_syntax_error(
                                util.string_format(
                                    "{0} item has duplicate bounds for state",
                                    itemnode.get_name()));
                    }
                }

                foreach (emu.render.detail.bounds_step step in result)
                {
                    rendlay_global.render_bounds_transform(ref step.bounds, trans);
                    if (step.bounds.x0 > step.bounds.x1)
                        std.swap(ref step.bounds.x0, ref step.bounds.x1);
                    if (step.bounds.y0 > step.bounds.y1)
                        std.swap(ref step.bounds.y0, ref step.bounds.y1);
                }

                rendlay_global.set_bounds_deltas(result);
                return result;
            }


            //---------------------------------------------
            //  make_color - get color inflection points
            //---------------------------------------------
            static layout_view_item_color_vector make_color(
                    layout_view_view_environment env,
                    util.xml.data_node itemnode,
                    render_color mult)
            {
                layout_view_item_color_vector result = new emu_render_detail_color_vector();
                for (util.xml.data_node color = itemnode.get_child("color"); color != null; color = color.get_next_sibling("color"))
                {
                    if (!rendlay_global.add_color_step(env, result, color))
                    {
                        throw new layout_syntax_error(
                                util.string_format(
                                    "{0} item has duplicate color for state",
                                    itemnode.get_name()));
                    }
                }

                if (result.empty())
                {
                    result.emplace_back(new emu.render.detail.color_step() { state = 0, color = mult, delta = new render_color() { a = 0.0F, r = 0.0F, g = 0.0F, b = 0.0F } });
                }
                else
                {
                    foreach (emu.render.detail.color_step step in result)
                        step.color *= mult;

                    rendlay_global.set_color_deltas(result);
                }

                return result;
            }


            //---------------------------------------------
            //  make_animoutput_tag - get animation output
            //  tag
            //---------------------------------------------
            string make_animoutput_tag(layout_view_view_environment env, util.xml.data_node itemnode)
            {
                util.xml.data_node animate = itemnode.get_child("animate");
                if (animate != null)
                    return env.get_attribute_string(animate, "name");
                else
                    return null;
            }


            //---------------------------------------------
            //  make_animinput_tag - get absolute tag for
            //  animation input
            //---------------------------------------------
            static string make_animinput_tag(layout_view_view_environment env, util.xml.data_node itemnode)
            {
                util.xml.data_node animate = itemnode.get_child("animate");
                return animate != null ? env.get_attribute_subtag(animate, "inputtag") : "";
            }


            //---------------------------------------------
            //  make_animmask - get animation state mask
            //---------------------------------------------
            static ioport_value make_animmask(layout_view_view_environment env, util.xml.data_node itemnode)
            {
                util.xml.data_node animate = itemnode.get_child("animate");

                //return animate ? env.get_attribute_int(*animate, "mask", ~ioport_value(0)) : ~ioport_value(0);
                if (animate != null)
                {
                    var ret = env.get_attribute_int(animate, "mask", s32.MaxValue);
                    return ret == s32.MaxValue ? ~(ioport_value)0 : (ioport_value)ret;
                }
                else
                {
                    return ~(ioport_value)0;
                }
            }


            //---------------------------------------------
            //  make_input_tag - get absolute input tag
            //---------------------------------------------
            static string make_input_tag(layout_view_view_environment env, util.xml.data_node itemnode)
            {
                return env.get_attribute_subtag(itemnode, "inputtag");
            }


            //---------------------------------------------
            //  get_blend_mode - explicit or implicit blend
            //---------------------------------------------
            static int get_blend_mode(layout_view_view_environment env, util.xml.data_node itemnode)
            {
                // see if there's a blend mode attribute
                string mode = itemnode.get_attribute_string_ptr("blend");
                if (mode != null)
                {
                    if (mode == "none")
                        return g.BLENDMODE_NONE;
                    else if (mode == "alpha")
                        return g.BLENDMODE_ALPHA;
                    else if (mode == "multiply")
                        return g.BLENDMODE_RGB_MULTIPLY;
                    else if (mode == "add")
                        return g.BLENDMODE_ADD;
                    else
                        throw new layout_syntax_error(util.string_format("unknown blend mode {0}", mode));
                }

                // fall back to implicit blend mode based on element type
                if (std.strcmp(itemnode.get_name(), "screen") == 0)
                    return -1; // magic number recognised by render.cpp to allow per-element blend mode
                else if (std.strcmp(itemnode.get_name(), "overlay") == 0)
                    return g.BLENDMODE_RGB_MULTIPLY;
                else
                    return g.BLENDMODE_ALPHA;
            }


            //---------------------------------------------
            //  get_state_shift - shift to right-align LSB
            //---------------------------------------------
            static unsigned get_state_shift(ioport_value mask)
            {
                unsigned result = 0;
                while (mask != 0 && g.BIT(mask, 0) == 0)
                {
                    ++result;
                    mask >>= 1;
                }

                return result;
            }
        }


        //**************************************************************************
        //  LAYOUT VIEW
        //**************************************************************************
        class layer_lists
        {
            public layout_view_item_list backdrops = new layout_view_item_list();
            public layout_view_item_list screens = new layout_view_item_list();
            public layout_view_item_list overlays = new layout_view_item_list();
            public layout_view_item_list bezels = new layout_view_item_list();
            public layout_view_item_list cpanels = new layout_view_item_list();
            public layout_view_item_list marquees = new layout_view_item_list();
        }


        /// \brief A subset of items in a view that can be hidden or shown
        ///
        /// Visibility toggles allow the user to show or hide selected parts
        /// of a view.
        public class visibility_toggle
        {
            string m_name;             // display name for the toggle
            u32 m_mask;             // toggle combination to show


            // construction/destruction/assignment
            public visibility_toggle(string name, u32 mask)
            {
                m_name = name;
                m_mask = mask;


                g.assert(mask != 0);
            }

            //visibility_toggle(visibility_toggle const &) = default;
            //visibility_toggle(visibility_toggle &&) = default;


            //visibility_toggle &operator=(visibility_toggle const &) = default;
            //visibility_toggle &operator=(visibility_toggle &&) = default;


            // getters
            public string name() { return m_name; }
            //u32 mask() const { return m_mask; }
        }


        //using visibility_toggle_vector = std::vector<visibility_toggle>;


        /// \brief An edge of an item in a view
        public class edge
        {
            unsigned m_index;            // index of item in some collection
            float m_position;         // position of edge on given axis
            bool m_trailing;         // false for edge at lower position on axis


            // construction/destruction
            public edge(unsigned index, float position, bool trailing)
            {
                m_index = index;
                m_position = position;
                m_trailing = trailing;
            }


            // getters
            public unsigned index() { return m_index; }
            public float position() { return m_position; }
            public bool trailing() { return m_trailing; }


            // comparison
            //constexpr bool operator<(edge const &that) const
            //{
            //    return std::make_tuple(m_position, m_trailing, m_index) < std::make_tuple(that.m_position, that.m_trailing, that.m_index);
            //}
        }

        //using edge_vector = std::vector<edge>;


        // internal state
        float m_effaspect;        // X/Y of the layout in current configuration
        render_bounds m_bounds;           // computed bounds of the view in current configuration
        layout_view_item_list m_items = new layout_view_item_list();            // list of layout items
        layout_view_item_ref_vector m_visible_items = new layout_view_item_ref_vector();    // all visible items
        layout_view_item_ref_vector m_screen_items = new layout_view_item_ref_vector();     // visible items that represent screens to draw
        layout_view_item_ref_vector m_interactive_items = new layout_view_item_ref_vector();// visible items that can accept pointer input
        layout_view_edge_vector m_interactive_edges_x = new layout_view_edge_vector();
        layout_view_edge_vector m_interactive_edges_y = new layout_view_edge_vector();
        layout_view_screen_ref_vector m_screens = new layout_view_screen_ref_vector();          // list screens visible in current configuration

        // handlers
        prepare_items_delegate m_prepare_items;    // prepare items for adding to render container
        preload_delegate m_preload;          // additional actions when visible items change
        recomputed_delegate m_recomputed;       // additional actions on resizing/visibility change

        // cold items
        string m_name;             // display name for the view
        string m_unqualified_name; // the name exactly as specified in the layout file
        layout_view_item_id_map m_items_by_id;      // items with non-empty ID indexed by ID
        layout_view_visibility_toggle_vector m_vistoggles = new layout_view_visibility_toggle_vector();       // collections of items that can be shown/hidden
        render_bounds m_expbounds = new render_bounds();        // explicit bounds of the view
        u32 m_defvismask;       // default visibility mask
        bool m_has_art;          // true if the layout contains non-screen elements


        // construction/destruction
        //-------------------------------------------------
        //  layout_view - constructor
        //-------------------------------------------------
        public layout_view(
                emu.render.detail.layout_environment env,
                util.xml.data_node viewnode,
                layout_view_element_map elemmap,
                layout_view_group_map groupmap)
        {
            m_effaspect = 1.0f;
            m_name = make_name(env, viewnode);
            m_unqualified_name = env.get_attribute_string(viewnode, "name");
            m_defvismask = 0;
            m_has_art = false;


            // parse the layout
            m_expbounds.x0 = m_expbounds.y0 = m_expbounds.x1 = m_expbounds.y1 = 0;
            layout_view_view_environment local = new layout_view_view_environment(env, m_name);
            layer_lists layers = new layer_lists();
            local.set_parameter("viewname", m_name);
            add_items(layers, local, viewnode, elemmap, groupmap, g.ROT0, rendlay_global.identity_transform, new render_color() { a = 1.0F, r = 1.0F, g = 1.0F, b = 1.0F }, true, false, true);

            // can't support legacy layers and modern visibility toggles at the same time
            if (!m_vistoggles.empty() && (!layers.backdrops.empty() || !layers.overlays.empty() || !layers.bezels.empty() || !layers.cpanels.empty() || !layers.marquees.empty()))
                throw new layout_syntax_error("view contains visibility toggles as well as legacy backdrop, overlay, bezel, cpanel and/or marquee elements");

            // create visibility toggles for legacy layers
            u32 mask = 1;
            if (!layers.backdrops.empty())
            {
                m_vistoggles.emplace_back(new visibility_toggle("Backdrops", mask));
                foreach (item backdrop in layers.backdrops)
                    backdrop.m_visibility_mask = mask;
                m_defvismask |= mask;
                mask <<= 1;
            }

            if (!layers.overlays.empty())
            {
                m_vistoggles.emplace_back(new visibility_toggle("Overlays", mask));
                foreach (item overlay in layers.overlays)
                    overlay.m_visibility_mask = mask;
                m_defvismask |= mask;
                mask <<= 1;
            }

            if (!layers.bezels.empty())
            {
                m_vistoggles.emplace_back(new visibility_toggle("Bezels", mask));
                foreach (item bezel in layers.bezels)
                    bezel.m_visibility_mask = mask;
                m_defvismask |= mask;
                mask <<= 1;
            }

            if (!layers.cpanels.empty())
            {
                m_vistoggles.emplace_back(new visibility_toggle("Control Panels", mask));
                foreach (item cpanel in layers.cpanels)
                    cpanel.m_visibility_mask = mask;
                m_defvismask |= mask;
                mask <<= 1;
            }

            if (!layers.marquees.empty())
            {
                m_vistoggles.emplace_back(new visibility_toggle("Backdrops", mask));
                foreach (item marquee in layers.marquees)
                    marquee.m_visibility_mask = mask;
                m_defvismask |= mask;
                mask <<= 1;
            }

            // deal with legacy element groupings
            if (!layers.overlays.empty() || (layers.backdrops.size() <= 1))
            {
                // screens (-1) + overlays (RGB multiply) + backdrop (add) + bezels (alpha) + cpanels (alpha) + marquees (alpha)
                foreach (item backdrop in layers.backdrops)
                    backdrop.m_blend_mode = g.BLENDMODE_ADD;

                foreach (var item in layers.screens) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.screens);
                foreach (var item in layers.overlays) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.overlays);
                foreach (var item in layers.backdrops) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.backdrops);
                foreach (var item in layers.bezels) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.bezels);
                foreach (var item in layers.cpanels) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.cpanels);
                foreach (var item in layers.marquees) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.marquees);
            }
            else
            {
                // multiple backdrop pieces and no overlays (Golly! Ghost! mode):
                // backdrop (alpha) + screens (add) + bezels (alpha) + cpanels (alpha) + marquees (alpha)
                foreach (item screen in layers.screens)
                {
                    if (screen.blend_mode() == -1)
                        screen.m_blend_mode = g.BLENDMODE_ADD;
                }

                foreach (var item in layers.backdrops) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.backdrops);
                foreach (var item in layers.screens) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.screens);
                foreach (var item in layers.bezels) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.bezels);
                foreach (var item in layers.cpanels) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.cpanels);
                foreach (var item in layers.marquees) m_items.push_back(item);  //m_items.splice(m_items.end(), layers.marquees);
            }

            // index items with keys supplied
            foreach (item curitem in m_items)
            {
                if (!curitem.id().empty())
                {
                    if (!m_items_by_id.emplace(curitem.id(), curitem))
                        throw new layout_syntax_error("view contains item with duplicate id attribute");
                }
            }

            // calculate metrics
            recompute(default_visibility_mask(), false);
            foreach (var group in groupmap)  //for (group_map::value_type &group : groupmap)
                group.second().set_bounds_unresolved();
        }

        //~layout_view();


        // getters
        //item *get_item(std::string const &id);
        public layout_view_item_list items() { return m_items; }


        //-------------------------------------------------
        //  has_screen - return true if this view contains
        //  the specified screen
        //-------------------------------------------------
        public bool has_screen(screen_device screen)
        {
            return std.find_if(m_items, (itm) => { return itm.screen() == screen; }) != default;  //return std::find_if(m_items.begin(), m_items.end(), [&screen] (auto &itm) { return itm.screen() == &screen; }) != m_items.end();
        }


        public string name() { return m_name; }
        //const std::string &unqualified_name() const { return m_unqualified_name; }
        //size_t visible_screen_count() const { return m_screens.size(); }
        public float effective_aspect() { return m_effaspect; }
        //const render_bounds &bounds() const { return m_bounds; }


        //-------------------------------------------------
        //  has_visible_screen - return true if this view
        //  has the given screen visble
        //-------------------------------------------------

        public bool has_visible_screen(screen_device screen)
        {
            return std.find_if(m_screens, (scr) => { return scr == screen; }) != default;  //return std::find_if(m_screens.begin(), m_screens.end(), [&screen] (auto const &scr) { return &scr.get() == &screen; }) != m_screens.end();
        }


        public layout_view_item_ref_vector visible_items() { return m_visible_items; }
        public layout_view_item_ref_vector visible_screen_items() { return m_screen_items; }
        public layout_view_item_ref_vector interactive_items() { return m_interactive_items; }
        public layout_view_edge_vector interactive_edges_x() { return m_interactive_edges_x; }
        public layout_view_edge_vector interactive_edges_y() { return m_interactive_edges_y; }
        //const screen_ref_vector &visible_screens() const { return m_screens; }
        //const visibility_toggle_vector &visibility_toggles() const { return m_vistoggles; }
        public u32 default_visibility_mask() { return m_defvismask; }
        public bool has_art() { return m_has_art; }


        // set handlers
        //void set_prepare_items_callback(prepare_items_delegate &&handler);
        //void set_preload_callback(preload_delegate &&handler);
        //void set_recomputed_callback(recomputed_delegate &&handler);


        // operations
        public void prepare_items() { if (m_prepare_items != null) m_prepare_items(); }


        //-------------------------------------------------
        //  recompute - recompute the bounds and aspect
        //  ratio of a view and all of its contained items
        //-------------------------------------------------
        public void recompute(u32 visibility_mask, bool zoom_to_screen)
        {
            // reset the bounds and collected active items
            render_bounds scrbounds = new render_bounds() { x0 = 0.0f, y0 = 0.0f, x1 = 0.0f, y1 = 0.0f };
            m_bounds = scrbounds;
            m_visible_items.clear();
            m_screen_items.clear();
            m_interactive_items.clear();
            m_interactive_edges_x.clear();
            m_interactive_edges_y.clear();
            m_screens.clear();

            // loop over items and filter by visibility mask
            bool first = true;
            bool scrfirst = true;
            foreach (item curitem in m_items)
            {
                if ((visibility_mask & curitem.visibility_mask()) == curitem.visibility_mask())
                {
                    render_bounds rawbounds = rendlay_global.accumulate_bounds(curitem.m_rawbounds);

                    // accumulate bounds
                    m_visible_items.emplace_back(curitem);
                    if (first)
                        m_bounds = rawbounds;
                    else
                        m_bounds |= rawbounds;

                    first = false;

                    // accumulate visible screens and their bounds bounds
                    if (curitem.screen() != null)
                    {
                        if (scrfirst)
                            scrbounds = rawbounds;
                        else
                            scrbounds |= rawbounds;

                        scrfirst = false;

                        // accumulate active screens
                        m_screen_items.emplace_back(curitem);
                        m_screens.emplace_back(curitem.screen());
                    }

                    // accumulate interactive elements
                    if (!curitem.clickthrough() || curitem.has_input())
                        m_interactive_items.emplace_back(curitem);
                }
            }

            // if we have an explicit bounds, override it
            if (m_expbounds.x1 > m_expbounds.x0)
                m_bounds = m_expbounds;

            render_bounds target_bounds = new render_bounds();
            if (!zoom_to_screen || scrfirst)
            {
                // if we're handling things normally, the target bounds are (0,0)-(1,1)
                m_effaspect = ((m_bounds.x1 > m_bounds.x0) && (m_bounds.y1 > m_bounds.y0)) ? m_bounds.aspect() : 1.0f;
                target_bounds.x0 = target_bounds.y0 = 0.0f;
                target_bounds.x1 = target_bounds.y1 = 1.0f;
            }
            else
            {
                // if we're cropping, we want the screen area to fill (0,0)-(1,1)
                m_effaspect = ((scrbounds.x1 > scrbounds.x0) && (scrbounds.y1 > scrbounds.y0)) ? scrbounds.aspect() : 1.0f;
                target_bounds.x0 = (m_bounds.x0 - scrbounds.x0) / scrbounds.width();
                target_bounds.y0 = (m_bounds.y0 - scrbounds.y0) / scrbounds.height();
                target_bounds.x1 = target_bounds.x0 + (m_bounds.width() / scrbounds.width());
                target_bounds.y1 = target_bounds.y0 + (m_bounds.height() / scrbounds.height());
            }

            // determine the scale/offset for normalization
            float xoffs = m_bounds.x0;
            float yoffs = m_bounds.y0;
            float xscale = target_bounds.width() / m_bounds.width();
            float yscale = target_bounds.height() / m_bounds.height();

            // normalize all the item bounds
            foreach (item curitem in items())
            {
                g.assert(curitem.m_rawbounds.size() == curitem.m_bounds.size());

                //std::copy(curitem.m_rawbounds.begin(), curitem.m_rawbounds.end(), curitem.m_bounds.begin());
                curitem.m_bounds = new emu_render_detail_bounds_vector();
                foreach (var it in curitem.m_rawbounds)
                    curitem.m_bounds.Add(it);

                rendlay_global.normalize_bounds(curitem.m_bounds, target_bounds.x0, target_bounds.y0, xoffs, yoffs, xscale, yscale);
            }

            // sort edges of interactive items
            rendlay_global.LOGMASKED(rendlay_global.LOG_INTERACTIVE_ITEMS, "Recalculated view '{0}' with {1} interactive items\n", name(), m_interactive_items.size());
            m_interactive_edges_x.reserve(m_interactive_items.size() * 2);
            m_interactive_edges_y.reserve(m_interactive_items.size() * 2);
            for (unsigned i = 0; m_interactive_items.size() > i; ++i)
            {
                item curitem = m_interactive_items[i];
                render_bounds curbounds = rendlay_global.accumulate_bounds(curitem.m_bounds);
                rendlay_global.LOGMASKED(rendlay_global.LOG_INTERACTIVE_ITEMS, "{0}: ({1} {2} {3} {4}) hasinput={5} clickthrough={6}\n",
                        i, curbounds.x0, curbounds.y0, curbounds.x1, curbounds.y1, curitem.has_input(), curitem.clickthrough());
                m_interactive_edges_x.emplace_back(new edge(i, curbounds.x0, false));
                m_interactive_edges_x.emplace_back(new edge(i, curbounds.x1, true));
                m_interactive_edges_y.emplace_back(new edge(i, curbounds.y0, false));
                m_interactive_edges_y.emplace_back(new edge(i, curbounds.y1, true));
            }

            m_interactive_edges_x.Sort();  //std::sort(m_interactive_edges_x.begin(), m_interactive_edges_x.end());
            m_interactive_edges_y.Sort();  //std::sort(m_interactive_edges_y.begin(), m_interactive_edges_y.end());

            if ((rendlay_global.VERBOSE & rendlay_global.LOG_INTERACTIVE_ITEMS) != 0)
            {
                foreach (edge e in m_interactive_edges_x)
                    rendlay_global.LOGMASKED(rendlay_global.LOG_INTERACTIVE_ITEMS, "x={0} {1}{2}\n", e.position(), e.trailing() ? ']' : '[', e.index());
                foreach (edge e in m_interactive_edges_y)
                    rendlay_global.LOGMASKED(rendlay_global.LOG_INTERACTIVE_ITEMS, "y={0} {1}{2}\n", e.position(), e.trailing() ? ']' : '[', e.index());
            }

            // additional actions typically supplied by script
            if (m_recomputed != null)
                m_recomputed();
        }


        //-------------------------------------------------
        //  preload - perform expensive loading upfront
        //  for visible elements
        //-------------------------------------------------
        public void preload()
        {
            foreach (item curitem in m_visible_items)
            {
                if (curitem.element() != null)
                    curitem.element().preload();
            }

            if (m_preload != null)
                m_preload();
        }


        // resolve tags, if any
        //-----------------------------
        //  resolve_tags - resolve tags
        //-----------------------------
        public void resolve_tags()
        {
            foreach (item curitem in items())
                curitem.resolve_tags();
        }


        // add items, recursing for groups
        //-------------------------------------------------
        //  add_items - add items, recursing for groups
        //-------------------------------------------------
        void add_items(
                layer_lists layers,
                layout_view_view_environment env,
                util.xml.data_node parentnode,
                layout_view_element_map elemmap,
                layout_view_group_map groupmap,
                int orientation,
                layout_group_transform trans,
                render_color color,
                bool root,
                bool repeat,
                bool init)
        {
            bool envaltered = false;
            bool unresolved = true;
            for (util.xml.data_node itemnode = parentnode.get_first_child(); itemnode != null; itemnode = itemnode.get_next_sibling())
            {
                if (std.strcmp(itemnode.get_name(), "bounds") == 0)
                {
                    // set explicit bounds
                    if (root)
                        env.parse_bounds(itemnode, out m_expbounds);
                }
                else if (std.strcmp(itemnode.get_name(), "param") == 0)
                {
                    envaltered = true;
                    if (!unresolved)
                    {
                        unresolved = true;
                        foreach (var group in groupmap)
                            group.second().set_bounds_unresolved();
                    }

                    if (!repeat)
                        env.set_parameter(itemnode);
                    else
                        env.set_repeat_parameter(itemnode, init);
                }
                else if (std.strcmp(itemnode.get_name(), "screen") == 0)
                {
                    layers.screens.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                }
                else if (std.strcmp(itemnode.get_name(), "element") == 0)
                {
                    layers.screens.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (std.strcmp(itemnode.get_name(), "backdrop") == 0)
                {
                    if (layers.backdrops.empty())
                        g.osd_printf_warning("Warning: layout view '{0}' contains deprecated backdrop element\n", name());
                    layers.backdrops.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (std.strcmp(itemnode.get_name(), "overlay") == 0)
                {
                    if (layers.overlays.empty())
                        g.osd_printf_warning("Warning: layout view '{0}' contains deprecated overlay element\n", name());
                    layers.overlays.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (std.strcmp(itemnode.get_name(), "bezel") == 0)
                {
                    if (layers.bezels.empty())
                        g.osd_printf_warning("Warning: layout view '{0}' contains deprecated bezel element\n", name());

                    layers.bezels.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (std.strcmp(itemnode.get_name(), "cpanel") == 0)
                {
                    if (layers.cpanels.empty())
                        g.osd_printf_warning("Warning: layout view '{0}' contains deprecated cpanel element\n", name());

                    layers.cpanels.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (std.strcmp(itemnode.get_name(), "marquee") == 0)
                {
                    if (layers.marquees.empty())
                        g.osd_printf_warning("Warning: layout view '{0}' contains deprecated marquee element\n", name());

                    layers.marquees.emplace_back(new item(env, itemnode, elemmap, orientation, trans, color));
                    m_has_art = true;
                }
                else if (std.strcmp(itemnode.get_name(), "group") == 0)
                {
                    string ref_ = env.get_attribute_string(itemnode, "ref");
                    if (ref_.empty())
                        throw new layout_syntax_error("group instantiation must have non-empty ref attribute");

                    var found = groupmap.find(ref_);
                    if (found == null)
                        throw new layout_syntax_error(util.string_format("unable to find group {0}", ref_));

                    unresolved = false;
                    found.resolve_bounds(env, groupmap);

                    layout_group_transform grouptrans = new layout_group_transform
                    (
                        new std.array<float, u64_const_3>(trans[0][0], trans[0][1], trans[0][2]),
                        new std.array<float, u64_const_3>(trans[1][0], trans[1][1], trans[1][2]),
                        new std.array<float, u64_const_3>(trans[2][0], trans[2][1], trans[2][2])
                    );
                    util.xml.data_node itemboundsnode = itemnode.get_child("bounds");
                    util.xml.data_node itemorientnode = itemnode.get_child("orientation");
                    int grouporient = env.parse_orientation(itemorientnode);
                    if (itemboundsnode != null)
                    {
                        render_bounds itembounds;
                        env.parse_bounds(itemboundsnode, out itembounds);
                        grouptrans = found.make_transform(grouporient, itembounds, trans);
                    }
                    else if (itemorientnode != null)
                    {
                        grouptrans = found.make_transform(grouporient, trans);
                    }

                    layout_view_view_environment local = new layout_view_view_environment(env, false);
                    add_items(
                            layers,
                            local,
                            found.get_groupnode(),
                            elemmap,
                            groupmap,
                            g.orientation_add(grouporient, orientation),
                            grouptrans,
                            env.parse_color(itemnode.get_child("color")) * color,
                            false,
                            false,
                            true);
                }
                else if (std.strcmp(itemnode.get_name(), "repeat") == 0)
                {
                    int count = env.get_attribute_int(itemnode, "count", -1);
                    if (0 >= count)
                        throw new layout_syntax_error("repeat must have positive integer count attribute");

                    layout_view_view_environment local = new layout_view_view_environment(env, false);
                    for (int i = 0; count > i; ++i)
                    {
                        add_items(layers, local, itemnode, elemmap, groupmap, orientation, trans, color, false, true, i == 0);
                        local.increment_parameters();
                    }
                }
                else if (std.strcmp(itemnode.get_name(), "collection") == 0)
                {
                    string name = env.get_attribute_string(itemnode, "name");
                    if (name.empty())
                        throw new layout_syntax_error("collection must have non-empty name attribute");

                    var found = std.find_if(m_vistoggles, (x) => { return x.name() == name; });  //var found = std::find_if(m_vistoggles.begin(), m_vistoggles.end(), [name] (auto const &x) { return x.name() == name; });
                    if (default != found)
                        throw new layout_syntax_error(util.string_format("duplicate collection name '{0}'", name));

                    m_defvismask |= (env.get_attribute_bool(itemnode, "visible", true) ? 1U : 0U) << (int)m_vistoggles.size(); // TODO: make this less hacky
                    layout_view_view_environment local = new layout_view_view_environment(env, true);
                    m_vistoggles.emplace_back(new visibility_toggle(name, local.visibility_mask()));
                    add_items(layers, local, itemnode, elemmap, groupmap, orientation, trans, color, false, false, true);
                }
                else
                {
                    throw new layout_syntax_error(util.string_format("unknown view item {0}", itemnode.get_name()));
                }
            }

            if (envaltered && !unresolved)
            {
                foreach (var group in groupmap)
                    group.second().set_bounds_unresolved();
            }
        }


        static string make_name(emu.render.detail.layout_environment env, util.xml.data_node viewnode)
        {
            string name = env.get_attribute_string(viewnode, "name");
            if (name.empty())
                throw new layout_syntax_error("view must have non-empty name attribute");

            if (env.is_root_device())
            {
                return name;
            }
            else
            {
                string tag = env.device().tag();
                if (':' == tag[0])
                    tag = tag.Substring(1);  //++tag;

                return util.string_format("{0} {1}", tag, name);
            }
        }
    }


    /// \brief Layout description file
    ///
    /// Comprises a list of elements and a list of views.  The elements are
    /// reusable items that the views reference.
    public class layout_file
    {
        //using element_map = std::unordered_map<std::string, layout_element>;
        //using group_map = std::unordered_map<std::string, layout_group>;
        //using view_list = std::list<layout_view>;
        delegate void resolve_tags_delegate();  //using resolve_tags_delegate = delegate<void ()>;
        //using environment = emu::render::detail::layout_environment;


        // internal state
        device_t m_device;       // device that caused file to be loaded
        layout_file_element_map m_elemmap;      // list of shared layout elements
        layout_file_view_list m_viewlist;     // list of views
        resolve_tags_delegate m_resolve_tags; // additional actions after resolving tags


        // construction/destruction
        //-------------------------------------------------
        //  layout_file - constructor
        //-------------------------------------------------
        public layout_file(
            device_t device,
            util.xml.data_node rootnode,
            string searchpath,
            string dirname)
        {
            m_device = device;
            m_elemmap = new layout_file_element_map();
            m_viewlist = new layout_file_view_list();


            try
            {
                layout_file_environment env = new layout_file_environment(device, searchpath, dirname);

                // find the layout node
                util.xml.data_node mamelayoutnode = rootnode.get_child("mamelayout");
                if (mamelayoutnode == null)
                    throw new layout_syntax_error("missing mamelayout node");

                // validate the config data version
                int version = (int)mamelayoutnode.get_attribute_int("version", 0);
                if (version != rendlay_global.LAYOUT_VERSION)
                    throw new layout_syntax_error(util.string_format("unsupported version {0}", version));

                // parse all the parameters, elements and groups
                layout_file_group_map groupmap = new layout_file_group_map();
                add_elements(env, mamelayoutnode, groupmap, false, true);

                // parse all the views
                for (util.xml.data_node viewnode = mamelayoutnode.get_child("view"); viewnode != null; viewnode = viewnode.get_next_sibling("view"))
                {
                    // the trouble with allowing errors to propagate here is that it wreaks havoc with screenless systems that use a terminal by default
                    // e.g. intlc44 and intlc440 have a terminal on the TTY port by default and have a view with the front panel with the terminal screen
                    // however, they have a second view with just the front panel which is very useful if you're using e.g. -tty null_modem with a socket
                    // if the error is allowed to propagate, the entire layout is dropped so you can't select the useful view
                    try
                    {
                        m_viewlist.emplace_back(new layout_view(env, viewnode, m_elemmap, groupmap));
                    }
                    catch (layout_reference_error err)
                    {
                        g.osd_printf_warning("Error instantiating layout view {0}: {1}\n", env.get_attribute_string(viewnode, "name"), err);
                    }
                }

                // load the content of the first script node
                if (!m_viewlist.empty())
                {
                    util.xml.data_node scriptnode = mamelayoutnode.get_child("script");
                    if (scriptnode != null)
                        emulator_info.layout_script_cb(this, scriptnode.get_value());
                }
            }
            catch (layout_syntax_error err)
            {
                // syntax errors are always fatal
                throw new emu_fatalerror("Error parsing XML layout: {0}", err);
            }
        }

        //~layout_file();


        // getters
        //device_t &device() const { return m_device; }
        //element_map const &elements() const { return m_elemmap; }
        public layout_file_view_list views() { return m_viewlist; }


        // resolve tags, if any
        //-------------------------------------------------
        //  resolve_tags - resolve tags
        //-------------------------------------------------
        public void resolve_tags()
        {
            foreach (layout_view view in views())
                view.resolve_tags();

            if (m_resolve_tags != null)
                m_resolve_tags();
        }


        // set handlers
        //void set_resolve_tags_callback(resolve_tags_delegate &&handler);


        // add elements and parameters
        void add_elements(
                layout_file_environment env,
                util.xml.data_node parentnode,
                layout_file_group_map groupmap,
                bool repeat,
                bool init)
        {
            for (util.xml.data_node childnode = parentnode.get_first_child(); childnode != null; childnode = childnode.get_next_sibling())
            {
                if (std.strcmp(childnode.get_name(), "param") == 0)
                {
                    if (!repeat)
                        env.set_parameter(childnode);
                    else
                        env.set_repeat_parameter(childnode, init);
                }
                else if (std.strcmp(childnode.get_name(), "element") == 0)
                {
                    string name = env.get_attribute_string(childnode, "name");
                    if (name.empty())
                        throw new layout_syntax_error("element must have non-empty name attribute");
                    if (!m_elemmap.emplace(name, new layout_element(env, childnode)))  //if (!m_elemmap.emplace(std::piecewise_construct, std::forward_as_tuple(name), std::forward_as_tuple(env, *childnode)).second)
                        throw new layout_syntax_error(util.string_format("duplicate element name {0}", name));
                    m_elemmap.emplace(name, new layout_element(env, childnode));
                }
                else if (std.strcmp(childnode.get_name(), "group") == 0)
                {
                    string name = env.get_attribute_string(childnode, "name");
                    if (name.empty())
                        throw new layout_syntax_error("group must have non-empty name attribute");
                    if (!groupmap.emplace(name, new layout_group(childnode)))  //if (!groupmap.emplace(std::piecewise_construct, std::forward_as_tuple(name), std::forward_as_tuple(childnode)).second)
                        throw new layout_syntax_error(util.string_format("duplicate group name {0}", name));
                    groupmap.emplace(name, new layout_group(childnode));
                }
                else if (std.strcmp(childnode.get_name(), "repeat") == 0)
                {
                    int count = env.get_attribute_int(childnode, "count", -1);
                    if (0 >= count)
                        throw new layout_syntax_error("repeat must have positive integer count attribute");
                    layout_file_environment local = new layout_file_environment(env);
                    for (int i = 0; count > i; ++i)
                    {
                        add_elements(local, childnode, groupmap, true, i == 0);
                        local.increment_parameters();
                    }
                }
                else if (repeat || (std.strcmp(childnode.get_name(), "view") != 0 && std.strcmp(childnode.get_name(), "script") != 0))
                {
                    throw new layout_syntax_error(util.string_format("unknown layout item {0}", childnode.get_name()));
                }
            }
        }
    }


    namespace emu.render.detail
    {
        public class layout_environment
        {
            public class entry
            {
                string m_name;
                string m_text;
                s64 m_int = 0;
                s64 m_int_increment = 0;
                double m_float = 0.0;
                double m_float_increment = 0.0;
                int m_shift = 0;
                bool m_text_valid = false;
                bool m_int_valid = false;
                bool m_float_valid = false;
                bool m_generator = false;


                public entry(string name, string t)
                {
                    m_name = name;
                    m_text = t;
                    m_text_valid = true;
                }

                public entry(string name, s64 i)
                {
                    m_name = name;
                    m_int = i;
                    m_int_valid = true;
                }

                public entry(string name, double f)
                {
                    m_name = name;
                    m_float = f;
                    m_float_valid = true;
                }

                public entry(string name, string t, s64 i, int s)
                {
                    m_name = name;
                    m_text = t;
                    m_int_increment = i;
                    m_shift = s;
                    m_text_valid = true;
                    m_generator = true;
                }

                public entry(string name, string t, double i, int s)
                {
                    m_name = name;
                    m_text = t;
                    m_float_increment = i;
                    m_shift = s;
                    m_text_valid = true;
                    m_generator = true;
                }

                //entry(entry &&) = default;
                //entry &operator=(entry &&) = default;


                public void set(string t)
                {
                    m_text = t;
                    m_text_valid = true;
                    m_int_valid = false;
                    m_float_valid = false;
                }

                public void set(s64 i)
                {
                    m_int = i;
                    m_text_valid = false;
                    m_int_valid = true;
                    m_float_valid = false;
                }

                public void set(double f)
                {
                    m_float = f;
                    m_text_valid = false;
                    m_int_valid = false;
                    m_float_valid = true;
                }


                public string name() { return m_name; }
                public bool is_generator() { return m_generator; }


                public string get_text()
                {
                    if (!m_text_valid)
                    {
                        if (m_float_valid)
                        {
                            m_text = m_float.ToString();  //m_text = std::to_string(m_float);
                            m_text_valid = true;
                        }
                        else if (m_int_valid)
                        {
                            m_text = m_int.ToString();  //m_text = std::to_string(m_int);
                            m_text_valid = true;
                        }
                    }

                    return m_text;
                }


                //void increment()
                //static bool name_less(entry const &lhs, entry const &rhs) { return lhs.name() < rhs.name(); }
            }

            //using entry_vector = std::vector<entry>;

#if false
            template <typename T, typename U>
            void try_insert(T &&name, U &&value)
            {
                entry_vector::iterator const pos(
                        std::lower_bound(
                            m_entries.begin(),
                            m_entries.end(),
                            name,
                            [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                if ((m_entries.end() == pos) || (pos->name() != name))
                    m_entries.emplace(pos, std::forward<T>(name), std::forward<U>(value));
            }
#endif


            //template <typename T, typename U>
            void set(string name, object value)  //void set(T &&name, U &&value)
            {
                //entry_vector::iterator const pos(
                //        std::lower_bound(
                //            m_entries.begin(),
                //            m_entries.end(),
                //            name,
                //            [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                //if ((m_entries.end() == pos) || (pos->name() != name))
                //    m_entries.emplace(pos, std::forward<T>(name), std::forward<U>(value));
                //else
                //    pos->set(std::forward<U>(value));
                int pos = 0;
                for (; pos < m_entries.Count; pos++)
                {
                    if (m_entries[pos].name().CompareTo(name) >= 0)
                        break;
                }

                if ((m_entries.Count == pos) || (m_entries[pos].name() != name))
                {
                    if (value is string)      m_entries.emplace(pos, new entry(name, (string)value));
                    else if (value is s64)    m_entries.emplace(pos, new entry(name, (s64)value));
                    else if (value is double) m_entries.emplace(pos, new entry(name, (double)value));
                    else throw new emu_unimplemented();
                }
                else
                {
                    if (value is string)      m_entries[pos].set((string)value);
                    else if (value is s64)    m_entries[pos].set((s64)value);
                    else if (value is double) m_entries[pos].set((double)value);
                    else throw new emu_unimplemented();
                }
            }


            void cache_device_entries()
            {
                throw new emu_unimplemented();
            }


            entry find_entry(string name)  //entry *find_entry(char const *begin, char const *end)
            {
                cache_device_entries();
                //entry_vector::iterator const pos(
                //        std::lower_bound(
                //            m_entries.begin(),
                //            m_entries.end(),
                //            str,
                //            [] (entry const &lhs, std::string_view const &rhs) { return lhs.name() < rhs; }));
                //if ((m_entries.end() != pos) && pos->name() == str)
                //    return &*pos;
                //else
                //    return m_next ? m_next->find_entry(str) : nullptr;
                int pos = 0;
                for (; pos < m_entries.Count; pos++)
                {
                    if (m_entries[pos].name().CompareTo(name) < 0)
                        break;
                }
                
                if ((m_entries.Count != pos) && (m_entries[pos].name() == name))
                    return m_entries[pos];
                else
                    return m_next != null ? m_next.find_entry(name) : null;
            }


            //template <typename... T>
            std.pair<string, bool> get_variable_text(string str)
            {
                entry found = find_entry(str);
                if (found != null)
                {
                    return std.make_pair(found.get_text(), true);
                }
                else
                {
                    return std.make_pair("", false);
                }
            }


            string expand(string str)  //std::pair<char const *, char const *> expand(char const *str)
            {
                char variable_start_char = '~';
                char variable_end_char = '~';

                // search for candidate variable references
                size_t start = 0;
                for (size_t pos = str.find_first_of(variable_start_char); pos != g.npos; )
                {
                    string new_str = str.Substring((int)pos + 1);
                    int termIdx = new_str.IndexOf(c => !is_variable_char(c));  //auto term = std::find_if_not(str.begin() + pos + 1, str.end(), is_variable_char);
                    if ((termIdx == -1) || (new_str[termIdx] != variable_end_char))  //if ((term == str.end()) || (*term != variable_end_char))
                    {
                        // not a valid variable name - keep searching
                        pos = str.find_first_of(variable_start_char, (size_t)termIdx + pos + 1);  //pos = str.find_first_of(variable_start_char, term - str.begin());
                    }
                    else
                    {
                        // looks like a variable reference - try to look it up
                        std.pair<string, bool> text = get_variable_text(str.substr(pos + 1, (size_t)termIdx - (pos + 1)));  //std::pair<std::string_view, bool> text = get_variable_text(str.substr(pos + 1, term - (str.begin() + pos + 1)));
                        if (text.second)
                        {
                            // variable found
                            if (start == 0)
                                m_buffer = "";  //m_buffer.seekp(0);
                            m_buffer += str.Substring((int)start, (int)pos - (int)start);  //m_buffer.write(&str[start], pos - start);
                            m_buffer += text.first;  //m_buffer.write(text.first.data(), text.first.length());
                            start = (size_t)termIdx + 1;  //start = term - str.begin() + 1;
                            pos = str.find_first_of(variable_start_char, start);
                        }
                        else
                        {
                            // variable not found - move on
                            pos = str.find_first_of(variable_start_char, pos + 1);
                        }
                    }
                }

                // short-circuit the case where no substitutions were made
                if (start == 0)
                {
                    return str;
                }
                else
                {
                    m_buffer += str.Substring((int)start, (int)str.length() - (int)start);  //m_buffer.write(&str[start], str.length() - start);
                    return m_buffer;
                }
            }


            static unsigned hex_prefix(string s)
            {
                return ((0 != s.length()) && (s[0] == '$')) ? 1U : ((2 <= s.length()) && (s[0] == '0') && ((s[1] == 'x') || (s[1] == 'X'))) ? 2U : 0U;
            }

            static unsigned dec_prefix(string s)
            {
                return ((0 != s.length()) && (s[0] == '#')) ? 1U : 0U;
            }


            int parse_int(string str, int defvalue)  //int parse_int(string begin, string end, int defvalue);
            {
                //std::istringstream stream;
                //stream.imbue(f_portable_locale);
                int result;
                if (str.Length >= 1 && str[0] == '$')  //if (begin[0] == '$')
                {
                    //stream.str(std::string(begin + 1, end));
                    //unsigned uvalue;
                    //stream >> std::hex >> uvalue;
                    //result = int(uvalue);
                    result = Convert.ToInt32(str);
                }
                else if (str.Length >= 2 && ((str[0] == '0') && ((str[1] == 'x') || (str[1] == 'X'))))  //else if ((begin[0] == '0') && ((begin[1] == 'x') || (begin[1] == 'X')))
                {
                    //stream.str(std::string(begin + 2, end));
                    //unsigned uvalue;
                    //stream >> std::hex >> uvalue;
                    //result = int(uvalue);
                    result = Convert.ToInt32(str);
                }
                else if (str.Length >= 1 && str[0] == '#')  //else if (begin[0] == '#')
                {
                    //stream.str(std::string(begin + 1, end));
                    //stream >> result;
                    result = Convert.ToInt32(str);
                }
                else
                {
                    //stream.str(std::string(begin, end));
                    //stream >> result;
                    result = Convert.ToInt32(str);
                }

                return !string.IsNullOrEmpty(str) ? result : defvalue;  //return stream ? result : defvalue;
            }


            string parameter_name(util.xml.data_node node)
            {
                string attrib = node.get_attribute_string_ptr("name");
                if (attrib == null)
                    throw new layout_syntax_error("parameter lacks name attribute");
                return expand(attrib);
            }


            static bool is_variable_char(char ch) { return (('0' <= ch) && ('9' >= ch)) || (('A' <= ch) && ('Z' >= ch)) || (('a' <= ch) && ('z' >= ch)) || ('_' == ch); }


            layout_environment_entry_vector m_entries = new layout_environment_entry_vector();
            string m_buffer;  //util::ovectorstream m_buffer;
            //std::shared_ptr<NSVGrasterizer> const m_svg_rasterizer;
            device_t m_device;
            string m_search_path;
            string m_directory_name;
            layout_environment m_next = null;
            //bool m_cached = false;


            public layout_environment(device_t device, string searchpath, string dirname)
            {
                //throw new emu_unimplemented();
#if false
                : m_svg_rasterizer(nsvgCreateRasterizer(), util::nsvg_deleter())
#endif

                m_device = device;
                m_search_path = searchpath;
                m_directory_name = dirname;
            }


            public layout_environment(layout_environment next)
            {
                //throw new emu_unimplemented();
#if false
                : m_svg_rasterizer(next.m_svg_rasterizer)
#endif

                m_device = next.m_device;
                m_search_path = next.m_search_path;
                m_directory_name = next.m_directory_name;
                m_next = next;
            }


            public device_t device() { return m_device; }
            public running_machine machine() { return device().machine(); }

            public bool is_root_device() { return device() == machine().root_device(); }
            public string search_path() { return m_search_path; }
            public string directory_name() { return m_directory_name; }
            //std::shared_ptr<NSVGrasterizer> const &svg_rasterizer() const { return m_svg_rasterizer; }

            public void set_parameter(string name, string value) { set(name, value); }
            public void set_parameter(string name, s64 value) { set(name, value); }
            public void set_parameter(string name, double value) { set(name, value); }

            public void set_parameter(util.xml.data_node node)
            {
                // do basic validation
                string name = parameter_name(node);
                if (node.has_attribute("start") || node.has_attribute("increment") || node.has_attribute("lshift") || node.has_attribute("rshift"))
                    throw new layout_syntax_error("start/increment/lshift/rshift attributes are only allowed for repeat parameters");
                string value = node.get_attribute_string_ptr("value");
                if (value == null)
                    throw new layout_syntax_error("parameter lacks value attribute");

                // expand value and stash
                set(name, expand(value));
            }


            public void set_repeat_parameter(util.xml.data_node node, bool init)
            {
                // two types are allowed here - static value, and start/increment/lshift/rshift
                string name = parameter_name(node);
                string start = node.get_attribute_string_ptr("start");
                if (start != null)
                {
                    // simple validity checks
                    if (node.has_attribute("value"))
                        throw new layout_syntax_error("start attribute may not be used in combination with value attribute");

                    int lshift = node.has_attribute("lshift") ? get_attribute_int(node, "lshift", -1) : 0;
                    int rshift = node.has_attribute("rshift") ? get_attribute_int(node, "rshift", -1) : 0;
                    if ((0 > lshift) || (0 > rshift))
                        throw new layout_syntax_error("lshift/rshift attributes must be non-negative integers");

                    // increment is more complex - it may be an integer or a floating-point number
                    s64 intincrement = 0;
                    double floatincrement = 0;
                    string increment = node.get_attribute_string_ptr("increment");
                    if (increment != null)
                    {
                        string expanded = expand(increment);
                        unsigned hexprefix = hex_prefix(expanded);
                        unsigned decprefix = dec_prefix(expanded);
                        bool floatchars = expanded.find_first_of(".eE") != g.npos;
                        string stream = expanded.Substring((int)(hexprefix + decprefix));  //std::istringstream stream(std::string(expanded.substr(hexprefix + decprefix)));
                        //stream.imbue(std::locale::classic());
                        bool success = true;
                        if (hexprefix == 0 && decprefix == 0 && floatchars)
                        {
                            //stream >> floatincrement;
                            success = double.TryParse(stream, out floatincrement);
                        }
                        else if (hexprefix != 0)
                        {
                            //u64 uvalue;
                            //stream >> std::hex >> uvalue;
                            //intincrement = s64(uvalue);
                            try { intincrement = Convert.ToInt64(stream, 16); }
                            catch (Exception) { success = false; }
                        }
                        else
                        {
                            //stream >> intincrement;
                            success = s64.TryParse(stream, out intincrement);
                        }

                        // reject obviously bad stuff
                        if (!success)
                            throw new layout_syntax_error("increment attribute must be a number");
                    }

                    // don't allow generator parameters to be redefined
                    if (init)
                    {
                        //entry_vector::iterator const pos(
                        //        std::lower_bound(
                        //            m_entries.begin(),
                        //            m_entries.end(),
                        //            name,
                        //            [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                        //if ((m_entries.end() != pos) && (pos->name() == name))
                        //    throw new rendlay_global.layout_syntax_error("generator parameters must be defined exactly once per scope");
                        int pos = 0;
                        for (; pos < m_entries.Count; pos++)
                        {
                            if (m_entries[pos].name().CompareTo(name) >= 0)
                                break;
                        }

                        if (pos != m_entries.Count && m_entries[pos].name() == name)
                            throw new layout_syntax_error("generator parameters must be defined exactly once per scope");

                        if (floatincrement != 0)
                            m_entries.emplace(pos, new entry(name, expand(start), floatincrement, lshift - rshift));
                        else
                            m_entries.emplace(pos, new entry(name, expand(start), intincrement, lshift - rshift));
                    }
                }
                else if (node.has_attribute("increment") || node.has_attribute("lshift") || node.has_attribute("rshift"))
                {
                    throw new layout_syntax_error("increment/lshift/rshift attributes require start attribute");
                }
                else
                {
                    string value = node.get_attribute_string_ptr("value");
                    if (value == null)
                        throw new layout_syntax_error("parameter lacks value attribute");
                    //entry_vector::iterator const pos(
                    //        std::lower_bound(
                    //            m_entries.begin(),
                    //            m_entries.end(),
                    //            name,
                    //            [] (entry const &lhs, auto const &rhs) { return lhs.name() < rhs; }));
                    //if ((m_entries.end() == pos) || (pos->name() != name))
                    //    m_entries.emplace(pos, std::move(name), std::string(expanded.first, expanded.second));
                    //else if (pos->is_generator())
                    //    throw new rendlay_global.layout_syntax_error("generator parameters must be defined exactly once per scope");
                    //else
                    //    pos->set(std::string(expanded.first, expanded.second));
                    int pos = 0;
                    for (; pos < m_entries.Count; pos++)
                    {
                        if (m_entries[pos].name().CompareTo(name) >= 0)
                            break;
                    }

                    if (pos == m_entries.Count && m_entries[pos].name() != name)
                        m_entries.emplace(pos, new entry(name, expand(value)));
                    else if (m_entries[pos].is_generator())
                        throw new layout_syntax_error("generator parameters must be defined exactly once per scope");
                    else
                        m_entries[pos].set(expand(value));
                }
            }


            public void increment_parameters()
            {
                throw new emu_unimplemented();
            }


            public string get_attribute_string(util.xml.data_node node, string name, string defvalue = "")
            {
                string attrib = node.get_attribute_string_ptr(name);
                return attrib != null ? expand(attrib) : defvalue;
            }


            public string get_attribute_subtag(util.xml.data_node node, string name)
            {
                string attrib = node.get_attribute_string_ptr(name);
                return attrib != null ? device().subtag(expand(attrib)) : "";
            }


            public int get_attribute_int(util.xml.data_node node, string name, int defvalue)
            {
                string attrib = node.get_attribute_string_ptr(name);
                if (attrib == null)
                    return defvalue;

                // similar to what XML nodes do
                return parse_int(expand(attrib), defvalue);
            }


            float get_attribute_float(util.xml.data_node node, string name, float defvalue)
            {
                string attrib = node.get_attribute_string_ptr(name);
                if (attrib == null)
                    return defvalue;

                // similar to what XML nodes do
                //std::istringstream stream(std::string(expand(*attrib)));
                //stream.imbue(f_portable_locale);
                //float result;
                //return (stream >> result) ? result : defvalue;
                string stream = expand(attrib);
                float result;
                return float.TryParse(stream, out result) ? result : defvalue;
            }


            public bool get_attribute_bool(util.xml.data_node node, string name, bool defvalue)
            {
                string attrib = node.get_attribute_string_ptr(name);
                if (string.IsNullOrEmpty(attrib))
                    return defvalue;

                // first try yes/no strings
                string expanded = expand(attrib);
                if ("yes" == expanded || "true" == expanded)
                    return true;
                if ("no" == expanded || "false" == expanded)
                    return false;

                // fall back to integer parsing
                return parse_int(expanded, defvalue ? 1 : 0) != 0;
            }


            public void parse_bounds(util.xml.data_node node, out render_bounds result)
            {
                result = new render_bounds();

                if (node == null)
                {
                    // default to unit rectangle
                    result.x0 = result.y0 = 0.0F;
                    result.x1 = result.y1 = 1.0F;
                }
                else
                {
                    // horizontal position/size
                    if (node.has_attribute("left"))
                    {
                        result.x0 = get_attribute_float(node, "left", 0.0F);
                        result.x1 = get_attribute_float(node, "right", 1.0F);
                    }
                    else
                    {
                        float width = get_attribute_float(node, "width", 1.0F);
                        if (node.has_attribute("xc"))
                            result.x0 = get_attribute_float(node, "xc", 0.0F) - (width / 2.0F);
                        else
                            result.x0 = get_attribute_float(node, "x", 0.0F);
                        result.x1 = result.x0 + width;
                    }

                    // vertical position/size
                    if (node.has_attribute("top"))
                    {
                        result.y0 = get_attribute_float(node, "top", 0.0F);
                        result.y1 = get_attribute_float(node, "bottom", 1.0F);
                    }
                    else
                    {
                        float height = get_attribute_float(node, "height", 1.0F);
                        if (node.has_attribute("yc"))
                            result.y0 = get_attribute_float(node, "yc", 0.0F) - (height / 2.0F);
                        else
                            result.y0 = get_attribute_float(node, "y", 0.0F);

                        result.y1 = result.y0 + height;
                    }

                    // check for errors
                    if ((result.x0 > result.x1) || (result.y0 > result.y1))
                        throw new layout_syntax_error(util.string_format("illegal bounds ({0}-{1})-({2}-{3})", result.x0, result.x1, result.y0, result.y1));
                }
            }


            public render_color parse_color(util.xml.data_node node)
            {
                // default to opaque white
                if (node == null)
                    return new render_color() { a = 1.0F, r = 1.0F, g = 1.0F, b = 1.0F };

                // parse attributes
                render_color result = new render_color()
                {
                    a = get_attribute_float(node, "alpha", 1.0F),
                    r = get_attribute_float(node, "red", 1.0F),
                    g = get_attribute_float(node, "green", 1.0F),
                    b = get_attribute_float(node, "blue", 1.0F)
                };

                // check for errors
                if ((0.0F > new [] { result.r, result.g, result.b, result.a }.Min()) || (1.0F < new [] { result.r, result.g, result.b, result.a }.Max()))
                    throw new layout_syntax_error(util.string_format("illegal RGBA color {0},{1},{2},{3}", result.r, result.g, result.b, result.a));

                return result;
            }


            public int parse_orientation(util.xml.data_node node)
            {
                // default to no transform
                if (node == null)
                    return g.ROT0;

                // parse attributes
                int result;
                int rotate = get_attribute_int(node, "rotate", 0);
                switch (rotate)
                {
                    case 0:     result = g.ROT0;      break;
                    case 90:    result = g.ROT90;     break;
                    case 180:   result = g.ROT180;    break;
                    case 270:   result = g.ROT270;    break;
                    default:    throw new layout_syntax_error(util.string_format("invalid rotate attribute {0}", rotate));
                }

                if (get_attribute_bool(node, "swapxy", false))
                    result ^= g.ORIENTATION_SWAP_XY;
                if (get_attribute_bool(node, "flipx", false))
                    result ^= g.ORIENTATION_FLIP_X;
                if (get_attribute_bool(node, "flipy", false))
                    result ^= g.ORIENTATION_FLIP_Y;

                return result;
            }
        }


        public class view_environment : layout_environment
        {
            view_environment m_next_view = null;
            string m_name;
            u32 m_visibility_mask = 0U;
            unsigned m_next_visibility_bit = 0U;


            public view_environment(layout_environment next, string name)
                : base(next)
            {
                m_name = name;
            }

            public view_environment(view_environment next, bool visibility)
                : base(next)
            {
                m_next_view = next;
                m_name = next.m_name;
                m_visibility_mask = next.m_visibility_mask | ((u32)(visibility ? 1 : 0) << (int)next.m_next_visibility_bit);
                m_next_visibility_bit = next.m_next_visibility_bit + (visibility ? 1U : 0);


                if (32U < m_next_visibility_bit)
                    throw new layout_syntax_error(util.string_format("view '{0}' contains too many visibility toggles", m_name));
            }

            //~view_environment()
            //{
            //    if (m_next_view)
            //        m_next_view->m_next_visibility_bit = m_next_visibility_bit;
            //}


            public u32 visibility_mask() { return m_visibility_mask; }
        }
    } // namespace emu::render::detail
}
