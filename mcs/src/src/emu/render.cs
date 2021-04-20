// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ioport_value = System.UInt32;
using PointerU8 = mame.Pointer<System.Byte>;
using render_target_view_mask_vector = mame.std.vector<mame.std.pair<mame.layout_view, System.UInt32>>;  //using view_mask_vector = std::vector<view_mask_pair>;
using s32 = System.Int32;
using screen_device_enumerator = mame.device_type_enumerator<mame.screen_device>;  //typedef device_type_enumerator<screen_device> screen_device_enumerator;
using size_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using u64 = System.UInt64;
using unsigned = System.UInt32;


namespace mame
{
    // texture scaling callback
    public delegate void texture_scaler_func(bitmap_argb32 dest, bitmap_argb32 source, rectangle sbounds, layout_element.texture param);  //typedef void (*texture_scaler_func)(bitmap_argb32 &dest, bitmap_argb32 &source, const rectangle &sbounds, void *param);


    public static class render_global
    {
        // render scaling modes
        //enum
        //{
        public const int SCALE_FRACTIONAL      = 0;                              // compute fractional scaling factors for both axes
        public const int SCALE_FRACTIONAL_X    = 1;                              // compute fractional scaling factor for x-axis, and integer factor for y-axis
        public const int SCALE_FRACTIONAL_Y    = 2;                              // compute fractional scaling factor for y-axis, and integer factor for x-axis
        public const int SCALE_FRACTIONAL_AUTO = 3;                              // automatically compute fractional scaling for x/y-axes based on source native orientation
        public const int SCALE_INTEGER         = 4;                              // compute integer scaling factors for both axes, based on target dimensions
        //}


        //enum
        //{
        const int COMPONENT_TYPE_IMAGE = 0;
        const int COMPONENT_TYPE_RECT  = 1;
        const int COMPONENT_TYPE_DISK  = 2;
        const int COMPONENT_TYPE_MAX   = 3;
        //}


        //enum
        //{
        public const u8 CONTAINER_ITEM_LINE = 0;
        public const u8 CONTAINER_ITEM_QUAD = 1;
        const u8 CONTAINER_ITEM_MAX  = 2;
        //}


        public const UInt32 INTERNAL_FLAG_CHAR      = 0x00000001;

        // render creation flags
        public const byte RENDER_CREATE_NO_ART        = 0x01;         // ignore any views that have art in them
        public const byte RENDER_CREATE_SINGLE_FILE   = 0x02;         // only load views from the file specified
        public const byte RENDER_CREATE_HIDDEN        = 0x04;         // don't make this target visible


        // flags for primitives
        const int PRIMFLAG_TEXORIENT_SHIFT = 0;
        public const u32 PRIMFLAG_TEXORIENT_MASK = 15 << PRIMFLAG_TEXORIENT_SHIFT;

        const int PRIMFLAG_TEXFORMAT_SHIFT = 4;
        public const u32 PRIMFLAG_TEXFORMAT_MASK = 15 << PRIMFLAG_TEXFORMAT_SHIFT;

        const int PRIMFLAG_BLENDMODE_SHIFT = 8;
        public const u32 PRIMFLAG_BLENDMODE_MASK = 15 << PRIMFLAG_BLENDMODE_SHIFT;

        const int PRIMFLAG_ANTIALIAS_SHIFT = 12;
        const u32 PRIMFLAG_ANTIALIAS_MASK = 1 << PRIMFLAG_ANTIALIAS_SHIFT;

        const int PRIMFLAG_SCREENTEX_SHIFT = 13;
        //const u32 PRIMFLAG_SCREENTEX_MASK = 1 << PRIMFLAG_SCREENTEX_SHIFT;

        const int PRIMFLAG_TEXWRAP_SHIFT = 14;
        //const u32 PRIMFLAG_TEXWRAP_MASK = 1 << PRIMFLAG_TEXWRAP_SHIFT;

        const int PRIMFLAG_TEXSHADE_SHIFT = 15;
        //const u32 PRIMFLAG_TEXSHADE_MASK = 3 << PRIMFLAG_TEXSHADE_SHIFT;

        const int PRIMFLAG_VECTOR_SHIFT = 17;
        const u32 PRIMFLAG_VECTOR_MASK = 1 << PRIMFLAG_VECTOR_SHIFT;

        const int PRIMFLAG_VECTORBUF_SHIFT = 18;
        const u32 PRIMFLAG_VECTORBUF_MASK = 1 << PRIMFLAG_VECTORBUF_SHIFT;

        const int PRIMFLAG_TYPE_SHIFT = 19;
        //const u32 PRIMFLAG_TYPE_MASK = 3 << PRIMFLAG_TYPE_SHIFT;
        public const u32 PRIMFLAG_TYPE_LINE = 0 << PRIMFLAG_TYPE_SHIFT;
        public const u32 PRIMFLAG_TYPE_QUAD = 1 << PRIMFLAG_TYPE_SHIFT;

        const int PRIMFLAG_PACKABLE_SHIFT = 21;
        public const u32 PRIMFLAG_PACKABLE = 1 << PRIMFLAG_PACKABLE_SHIFT;


        public static u32 PRIMFLAG_TEXORIENT(u32 x) { return x << PRIMFLAG_TEXORIENT_SHIFT; }
        public static u32 PRIMFLAG_GET_TEXORIENT(u32 x) { return (x & PRIMFLAG_TEXORIENT_MASK) >> PRIMFLAG_TEXORIENT_SHIFT; }

        public static u32 PRIMFLAG_TEXFORMAT(u32 x) { return x << PRIMFLAG_TEXFORMAT_SHIFT; }
        public static u32 PRIMFLAG_GET_TEXFORMAT(u32 x) { return (x & PRIMFLAG_TEXFORMAT_MASK) >> PRIMFLAG_TEXFORMAT_SHIFT; }

        public static u32 PRIMFLAG_BLENDMODE(u32 x) { return x << PRIMFLAG_BLENDMODE_SHIFT; }
        public static u32 PRIMFLAG_GET_BLENDMODE(u32 x) { return (x & PRIMFLAG_BLENDMODE_MASK) >> PRIMFLAG_BLENDMODE_SHIFT; }

        public static u32 PRIMFLAG_ANTIALIAS(u32 x) { return x << PRIMFLAG_ANTIALIAS_SHIFT; }
        public static bool PRIMFLAG_GET_ANTIALIAS(u32 x) { return ((x & PRIMFLAG_ANTIALIAS_MASK) >> PRIMFLAG_ANTIALIAS_SHIFT) != 0; }

        public static u32 PRIMFLAG_SCREENTEX(u32 x) { return x << PRIMFLAG_SCREENTEX_SHIFT; }
        //#define PRIMFLAG_GET_SCREENTEX(x)   (((x) & PRIMFLAG_SCREENTEX_MASK) >> PRIMFLAG_SCREENTEX_SHIFT)

        public static u32 PRIMFLAG_TEXWRAP(u32 x) { return x << PRIMFLAG_TEXWRAP_SHIFT; }
        //#define PRIMFLAG_GET_TEXWRAP(x)     (((x) & PRIMFLAG_TEXWRAP_MASK) >> PRIMFLAG_TEXWRAP_SHIFT)

        public static u32 PRIMFLAG_TEXSHADE(u32 x) { return x << PRIMFLAG_TEXSHADE_SHIFT; }
        //#define PRIMFLAG_GET_TEXSHADE(x)    (((x) & PRIMFLAG_TEXSHADE_MASK) >> PRIMFLAG_TEXSHADE_SHIFT)

        //#define PRIMFLAG_VECTOR(x)          ((x) << PRIMFLAG_VECTOR_SHIFT)
        public static bool PRIMFLAG_GET_VECTOR(u32 x) { return ((x & PRIMFLAG_VECTOR_MASK) >> PRIMFLAG_VECTOR_SHIFT) != 0; }

        //#define PRIMFLAG_VECTORBUF(x)       ((x) << PRIMFLAG_VECTORBUF_SHIFT)
        public static bool PRIMFLAG_GET_VECTORBUF(u32 x) { return ((x & PRIMFLAG_VECTORBUF_MASK) >> PRIMFLAG_VECTORBUF_SHIFT) != 0; }


        // precomputed UV coordinates for various orientations
        public static readonly render_quad_texuv [] oriented_texcoords = new render_quad_texuv[8]
        {
            new render_quad_texuv() { tl = new render_texuv() { u = 0, v = 0 }, tr = new render_texuv() { u = 1, v = 0 }, bl = new render_texuv() { u = 0, v = 1 }, br = new render_texuv() { u = 1, v = 1 } },     // 0
            new render_quad_texuv() { tl = new render_texuv() { u = 1, v = 0 }, tr = new render_texuv() { u = 0, v = 0 }, bl = new render_texuv() { u = 1, v = 1 }, br = new render_texuv() { u = 0, v = 1 } },     // ORIENTATION_FLIP_X
            new render_quad_texuv() { tl = new render_texuv() { u = 0, v = 1 }, tr = new render_texuv() { u = 1, v = 1 }, bl = new render_texuv() { u = 0, v = 0 }, br = new render_texuv() { u = 1, v = 0 } },     // ORIENTATION_FLIP_Y
            new render_quad_texuv() { tl = new render_texuv() { u = 1, v = 1 }, tr = new render_texuv() { u = 0, v = 1 }, bl = new render_texuv() { u = 1, v = 0 }, br = new render_texuv() { u = 0, v = 0 } },     // ORIENTATION_FLIP_X | ORIENTATION_FLIP_Y
            new render_quad_texuv() { tl = new render_texuv() { u = 0, v = 0 }, tr = new render_texuv() { u = 0, v = 1 }, bl = new render_texuv() { u = 1, v = 0 }, br = new render_texuv() { u = 1, v = 1 } },     // ORIENTATION_SWAP_XY
            new render_quad_texuv() { tl = new render_texuv() { u = 0, v = 1 }, tr = new render_texuv() { u = 0, v = 0 }, bl = new render_texuv() { u = 1, v = 1 }, br = new render_texuv() { u = 1, v = 0 } },     // ORIENTATION_SWAP_XY | ORIENTATION_FLIP_X
            new render_quad_texuv() { tl = new render_texuv() { u = 1, v = 0 }, tr = new render_texuv() { u = 1, v = 1 }, bl = new render_texuv() { u = 0, v = 0 }, br = new render_texuv() { u = 0, v = 1 } },     // ORIENTATION_SWAP_XY | ORIENTATION_FLIP_Y
            new render_quad_texuv() { tl = new render_texuv() { u = 1, v = 1 }, tr = new render_texuv() { u = 1, v = 0 }, bl = new render_texuv() { u = 0, v = 1 }, br = new render_texuv() { u = 0, v = 0 } }      // ORIENTATION_SWAP_XY | ORIENTATION_FLIP_X | ORIENTATION_FLIP_Y
        };


        //-------------------------------------------------
        //  apply_orientation - apply orientation to a
        //  set of bounds
        //-------------------------------------------------
        public static void apply_orientation(render_bounds bounds, int orientation)
        {
            // swap first
            if ((orientation & global_object.ORIENTATION_SWAP_XY) != 0)
            {
                std.swap(ref bounds.x0, ref bounds.y0);
                std.swap(ref bounds.x1, ref bounds.y1);
            }

            // apply X flip
            if ((orientation & global_object.ORIENTATION_FLIP_X) != 0)
            {
                bounds.x0 = 1.0f - bounds.x0;
                bounds.x1 = 1.0f - bounds.x1;
            }

            // apply Y flip
            if ((orientation & global_object.ORIENTATION_FLIP_Y) != 0)
            {
                bounds.y0 = 1.0f - bounds.y0;
                bounds.y1 = 1.0f - bounds.y1;
            }
        }


        //-------------------------------------------------
        //  normalize_bounds - normalize bounds so that
        //  x0/y0 are less than x1/y1
        //-------------------------------------------------
        public static void normalize_bounds(render_bounds bounds)
        {
            if (bounds.x0 > bounds.x1)
                std.swap(ref bounds.x0, ref bounds.x1);
            if (bounds.y0 > bounds.y1)
                std.swap(ref bounds.y0, ref bounds.y1);
        }
    }


    // an object_transform is used to track transformations when building an object list
    class object_transform
    {
        public float yoffs;       // offset transforms
        public float xscale;
        public float yscale;     // scale transforms
        public float xoffs;
        public render_color color = new render_color();              // color transform
        public int orientation;        // orientation transform
        public bool no_center;          // center the container?
    }


    // render_texinfo - texture information
    public class render_texinfo
    {
        public PointerU8 base_;  //void *              base;               // base of the data
        public u32 rowpixels;          // pixels per row
        public u32 width;              // width of the image
        public u32 height;             // height of the image
        public u32 seqid;              // sequence ID
        public u64 unique_id;          // unique identifier to pass to osd
        public u64 old_id;             // previously allocated id, if applicable
        public MemoryContainer<rgb_t> palette;  // const rgb_t *       palette;            // palette for PALETTE16 textures, bcg lookup table for RGB32/YUY16
        public u32 palette_length;
    }


    // ======================> render_layer_config
    // render_layer_config - describes the state of layers
    public class render_layer_config
    {
        const u8 ZOOM_TO_SCREEN           = 0x01; // zoom to screen area by default
        const u8 ENABLE_SCREEN_OVERLAY    = 0x02; // enable screen overlays
        const u8 DEFAULT = ENABLE_SCREEN_OVERLAY;


        u8 m_state = DEFAULT;


        public render_layer_config() { }


        render_layer_config set_flag(u8 flag, bool enable)
        {
            if (enable) m_state |= flag;
            else m_state &= (u8)~flag;
            return this;
        }


        //bool operator==(const render_layer_config &rhs) const { return m_state == rhs.m_state; }
        //bool operator!=(const render_layer_config &rhs) const { return m_state != rhs.m_state; }


        public bool screen_overlay_enabled() { return (m_state & ENABLE_SCREEN_OVERLAY) != 0; }
        public bool zoom_to_screen() { return (m_state & ZOOM_TO_SCREEN) != 0; }


        public render_layer_config set_screen_overlay_enabled(bool enable)    { return set_flag(ENABLE_SCREEN_OVERLAY, enable); }
        public render_layer_config set_zoom_to_screen(bool zoom)              { return set_flag(ZOOM_TO_SCREEN, zoom); }
    }


    // ======================> render_primitive
    // render_primitive - a single low-level primitive for the rendering engine
    public class render_primitive : simple_list_item<render_primitive>
    {
        // render primitive types
        public enum primitive_type
        {
            INVALID = 0,                        // invalid type
            LINE,                               // a single line
            QUAD                                // a rectilinear quad
        }


        // public state
        public primitive_type type = primitive_type.INVALID;               // type of primitive
        public render_bounds bounds;             // bounds or positions
        public render_bounds full_bounds;        // bounds or positions (unclipped)
        public render_color color;              // RGBA values
        public u32 flags = 0;              // flags
        public float width = 0;              // width (for line primitives)
        public render_texinfo texture;            // texture info (for quad primitives)
        public render_quad_texuv texcoords;          // texture coordinates (for quad primitives)
        public render_container container = null;          // the render container we belong to

        // internal state
        render_primitive m_next = null;             // pointer to next element


        public render_primitive() { }


        // getters
        public render_primitive next() { return m_next; }
        public render_primitive m_next_get() { return m_next; }
        public void m_next_set(render_primitive value) { m_next = value; }

        //bool packable(const INT32 pack_size) const { return (flags & PRIMFLAG_PACKABLE) && texture.base != nullptr && texture.width <= pack_size && texture.height <= pack_size; }
        //float get_quad_width() const { return bounds.x1 - bounds.x0; }
        //float get_quad_height() const { return bounds.y1 - bounds.y0; }
        //float get_full_quad_width() const { return fabsf(full_bounds.x1 - full_bounds.x0); }
        //float get_full_quad_height() const { return fabsf(full_bounds.y1 - full_bounds.y0); }


        // reset to prepare for re-use
        //-------------------------------------------------
        //  reset - reset the state of a primitive after
        //  it is re-allocated
        //-------------------------------------------------
        public void reset()
        {
            // do not clear m_next!
            //memset(&type, 0, uintptr_t(&texcoords + 1) - uintptr_t(&type));
            type = primitive_type.INVALID;
            bounds = new render_bounds();
            color = new render_color();
            flags = 0;
            width = 0.0f;
            texture = new render_texinfo();
            texcoords = new render_quad_texuv();
        }
    }


    // ======================> render_primitive_list
    // render_primitive_list - an object containing a list head plus a lock
    public class render_primitive_list : IEnumerable<render_primitive>, IDisposable
    {
        // a reference is an abstract reference to an internal object of some sort
        class reference : simple_list_item<reference>
        {
            reference m_next;             // link to the next reference
            public object m_refptr;  //void *              m_refptr;           // reference pointer

            public reference next() { return m_next; }
            public reference m_next_get() { return m_next; }
            public void m_next_set(reference value) { m_next = value; }
        }


        // internal state
        simple_list<render_primitive> m_primlist = new simple_list<render_primitive>();               // list of primitives
        simple_list<reference> m_reflist = new simple_list<reference>();                       // list of references

        fixed_allocator<render_primitive> m_primitive_allocator = new fixed_allocator<render_primitive>();// allocator for primitives
        fixed_allocator<reference> m_reference_allocator = new fixed_allocator<reference>();       // allocator for references

        //std::recursive_mutex     m_lock;                             // lock to protect list accesses


        // construction/destruction
        //-------------------------------------------------
        //  render_primitive_list - constructor
        //-------------------------------------------------
        public render_primitive_list()
        {
        }

        ~render_primitive_list()
        {
            //throw new emu_unimplemented();
#if false
            global.assert(m_isDisposed);  // can remove
#endif
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            release_all();
            m_isDisposed = true;
        }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<render_primitive> GetEnumerator()
        {
            var iter = m_primlist.begin();

            while (iter.current() != null)
            {
                yield return iter.current();
                iter.advance();
            }
        }


        // getters
        public render_primitive first() { return m_primlist.first(); }
        public simple_list<render_primitive> primlist() { return m_primlist; }


        // range iterators
        //using auto_iterator = simple_list<render_primitive>::auto_iterator;
        simple_list<render_primitive>.auto_iterator begin() { return m_primlist.begin(); }
        simple_list<render_primitive>.auto_iterator end() { return m_primlist.end(); }


        // lock management
        public void acquire_lock()
        {
            //throw new emu_unimplemented();
#if false
            m_lock.lock();
#endif
        }

        public void release_lock()
        {
            //throw new emu_unimplemented();
#if false
            m_lock.unlock();
#endif
        }


        // reference management

        //-------------------------------------------------
        //  add_reference - add a new reference
        //-------------------------------------------------
        public void add_reference(object refptr) /* possibly only bitmap_t refs */  //void *refptr)
        {
            // skip if we already have one
            if (has_reference(refptr))
                return;

            // set the refptr and link us into the list
            reference ref_ = m_reference_allocator.alloc();
            ref_.m_refptr = refptr;
            m_reflist.append(ref_);
        }

        //-------------------------------------------------
        //  has_reference - find a refptr in a reference
        //  list
        //-------------------------------------------------
        public bool has_reference(object refptr) /* possibly only bitmap_t refs */  //void *refptr)
        {
            // skip if we already have one
            foreach (reference ref_ in m_reflist)
            {
                if (ref_.m_refptr == refptr)
                    return true;
            }

            return false;
        }


        // helpers for our friends to manipulate the list
        //-------------------------------------------------
        //  alloc - allocate a new empty primitive
        //-------------------------------------------------
        public render_primitive alloc(render_primitive.primitive_type type)
        {
            render_primitive result = m_primitive_allocator.alloc();
            result.reset();
            result.type = type;
            return result;
        }

        //-------------------------------------------------
        //  release_all - release the contents of
        //  a render list
        //-------------------------------------------------
        public void release_all()
        {
            // release all the live items while under the lock
            m_primitive_allocator.reclaim_all(m_primlist);
            m_reference_allocator.reclaim_all(m_reflist);
        }

        public void append(render_primitive prim) { append_or_return(prim, false); }

        //-------------------------------------------------
        //  append_or_return - append a primitive to the
        //  end of the list, or return it to the free
        //  list, based on a flag
        //-------------------------------------------------
        public void append_or_return(render_primitive prim, bool clipped)
        {
            if (!clipped)
                m_primlist.append(prim);
            else
                m_primitive_allocator.reclaim(prim);
        }
    }


    // ======================> render_texture
    // a render_texture is used to track transformations when building an object list
    public class render_texture : global_object, simple_list_item<render_texture>
    {
        const int MAX_TEXTURE_SCALES = 16;

        // a scaled_texture contains a single scaled entry for a texture
        struct scaled_texture
        {
            public bitmap_argb32 bitmap;  // final bitmap  //std::unique_ptr<bitmap_argb32>  bitmap;     // final bitmap
            public u32 seqid;                  // sequence number
        }

        // internal state
        render_manager m_manager;                  // reference to our manager
        render_texture m_next;                     // next texture (for free list)
        bitmap_t m_bitmap;                   // pointer to the original bitmap
        rectangle m_sbounds;                  // source bounds within the bitmap
        texture_format m_format;                   // format of the texture data
        u64 m_osddata;                  // aux data to pass to osd
        u64 m_id;                       // unique id to pass to osd
        u64 m_old_id;                   // previous id, if applicable

        // scaling state (ARGB32 only)
        texture_scaler_func m_scaler;                   // scaling callback
        layout_element.texture m_param;  //void *              m_param;                    // scaling callback parameter
        u32 m_curseq;                   // current sequence number
        scaled_texture [] m_scaled = new scaled_texture[MAX_TEXTURE_SCALES];// array of scaled variants of this texture


        // construction/destruction
        //-------------------------------------------------
        //  render_texture - constructor
        //-------------------------------------------------
        public render_texture()
        {
            m_manager = null;
            m_next = null;
            m_bitmap = null;
            m_format = texture_format.TEXFORMAT_ARGB32;
            m_id = ~0UL; // ~0ULL;
            m_old_id = ~0UL; // ~0ULL;
            m_scaler = null;
            m_param = null;
            m_curseq = 0;


            m_sbounds.set(0, -1, 0, -1);
            //memset(m_scaled, 0, sizeof(m_scaled));
        }

        //~render_texture()
        //{
        //    release();
        //}


        // reset before re-use
        //-------------------------------------------------
        //  reset - reset the state of a texture after
        //  it has been re-allocated
        //-------------------------------------------------
        public void reset(render_manager manager, texture_scaler_func scaler = null, layout_element.texture param = null) //, void *param = null)
        {
            m_manager = manager;
            if (scaler != null)
            {
                //assert(m_format == TEXFORMAT_ARGB32);
                m_scaler = scaler;
                m_param = param;
            }

            m_old_id = m_id;
            m_id = ~0UL;  //~0L;
        }


        // release resources when freed
        //-------------------------------------------------
        //  release - release resources when we are freed
        //-------------------------------------------------
        public void release()
        {
            // free all scaled versions
            for (int i = 0; i < m_scaled.Length; i++)  //for (auto &elem : m_scaled)
            {
                var elem = m_scaled[i];

                m_manager.invalidate_all(elem.bitmap);
                elem.bitmap = null;  //elem.bitmap.reset();
                elem.seqid = 0;
            }

            // invalidate references to the original bitmap as well
            m_manager.invalidate_all(m_bitmap);
            m_bitmap = null;
            m_sbounds.set(0, -1, 0, -1);
            m_format = texture_format.TEXFORMAT_ARGB32;
            m_curseq = 0;
        }


        // getters
        public render_texture next() { return m_next; }
        public render_texture m_next_get() { return m_next; }
        public void m_next_set(render_texture value) { m_next = value; }

        public texture_format format() { return m_format; }
        //render_manager *manager() const { return m_manager; }


        // configure the texture bitmap
        //-------------------------------------------------
        //  set_bitmap - set a new source bitmap
        //-------------------------------------------------
        public void set_bitmap(bitmap_t bitmap, rectangle sbounds, texture_format format)
        {
            //assert(bitmap.cliprect().contains(sbounds));

            // ensure we have a valid palette for palettized modes
            if (format == texture_format.TEXFORMAT_PALETTE16)
            {
                //throw new emu_unimplemented();
#if false
                assert(bitmap.palette() != NULL);
#endif
            }

            // invalidate references to the old bitmap
            if (bitmap != m_bitmap && m_bitmap != null)
                m_manager.invalidate_all(m_bitmap);

            // set the new bitmap/palette
            m_bitmap = bitmap;
            m_sbounds = sbounds;
            m_format = format;

            // invalidate all scaled versions
            for (int i = 0; i < m_scaled.Length; i++)  //for (auto & elem : m_scaled)
            {
                var elem = m_scaled[i];

                if (elem.bitmap != null)
                    m_manager.invalidate_all(elem.bitmap);
                elem.bitmap = null;  //elem.bitmap.reset();
                elem.seqid = 0;
            }
        }


        // set a unique identifier
        public void set_id(u64 id) { m_old_id = m_id; m_id = id; }


        // generic high-quality bitmap scaler
        //-------------------------------------------------
        //  hq_scale - generic high quality resampling
        //  scaler
        //-------------------------------------------------
        public static void hq_scale(bitmap_argb32 dest, bitmap_argb32 source, rectangle sbounds, layout_element.texture param) //, void *param)
        {
            render_color color = new render_color() { a = 1.0f, r = 1.0f, g = 1.0f, b = 1.0f };
            bitmap_argb32 sourcesub = new bitmap_argb32(source, sbounds);
            rendutil_global.render_resample_argb_bitmap_hq(dest, sourcesub, color);
        }


        // internal helpers

        //-------------------------------------------------
        //  get_scaled - get a scaled bitmap (if we can)
        //-------------------------------------------------
        public void get_scaled(UInt32 dwidth, UInt32 dheight, render_texinfo texinfo, render_primitive_list primlist, UInt32 flags = 0)
        {
            // source width/height come from the source bounds
            int swidth = m_sbounds.width();
            int sheight = m_sbounds.height();

            // ensure height/width are non-zero
            if (dwidth == 0) dwidth = 1;
            if (dheight == 0) dheight = 1;

            texinfo.unique_id = m_id;
            texinfo.old_id = m_old_id;
            if (m_old_id != ~0UL)  //~0ULL)
                m_old_id = ~0UL;  //~0ULL;

            // are we scaler-free? if so, just return the source bitmap
            if (m_scaler == null || (m_bitmap != null && swidth == dwidth && sheight == dheight))
            {
                if (m_bitmap == null)
                    return;

                // add a reference and set up the source bitmap
                primlist.add_reference(m_bitmap);
                texinfo.base_ = m_bitmap.raw_pixptr(m_sbounds.top(), m_sbounds.left());
                texinfo.rowpixels = (UInt32)m_bitmap.rowpixels();
                texinfo.width = (UInt32)swidth;
                texinfo.height = (UInt32)sheight;
                // palette will be set later
                texinfo.seqid = ++m_curseq;
            }
            else
            {
                // make sure we can recover the original argb32 bitmap
                bitmap_argb32 dummy = new bitmap_argb32();
                bitmap_argb32 srcbitmap = (m_bitmap != null) ? (bitmap_argb32)m_bitmap : dummy;

                // is it a size we already have?
                scaled_texture scaled; // = null;
                scaled.bitmap = new bitmap_argb32();
                scaled.seqid = 0;
                int scalenum;
                for (scalenum = 0; scalenum < m_scaled.Length; scalenum++)
                {
                    scaled = m_scaled[scalenum];

                    // we need a non-NULL bitmap with matching dest size
                    if (scaled.bitmap != null && dwidth == scaled.bitmap.width() && dheight == scaled.bitmap.height())
                        break;
                }

                // did we get one?
                if (scalenum == m_scaled.Length)
                {
                    int lowest = -1;

                    // didn't find one -- take the entry with the lowest seqnum
                    for (scalenum = 0; scalenum < m_scaled.Length; scalenum++)
                        if ((lowest == -1 || m_scaled[scalenum].seqid < m_scaled[lowest].seqid) && !primlist.has_reference(m_scaled[scalenum].bitmap))
                            lowest = scalenum;

                    if (-1 == lowest)
                        throw new emu_fatalerror("render_texture::get_scaled: Too many live texture instances!");

                    // throw out any existing entries
                    scaled = m_scaled[lowest];
                    if (scaled.bitmap != null)
                    {
                        m_manager.invalidate_all(scaled.bitmap);
                        //global_free(scaled.bitmap);
                        scaled.bitmap = null;
                    }

                    // allocate a new bitmap
                    scaled.bitmap = new bitmap_argb32((int)dwidth, (int)dheight);
                    scaled.seqid = ++m_curseq;

                    // let the scaler do the work
                    m_scaler(scaled.bitmap, srcbitmap, m_sbounds, m_param);  //, m_param);
                }

                // finally fill out the new info
                primlist.add_reference(scaled.bitmap);
                texinfo.base_ = scaled.bitmap.pix(0);
                texinfo.rowpixels = (u32)scaled.bitmap.rowpixels();
                texinfo.width = dwidth;
                texinfo.height = dheight;
                // palette will be set later
                texinfo.seqid = scaled.seqid;
            }
        }

        //-------------------------------------------------
        //  get_adjusted_palette - return the adjusted
        //  palette for a texture
        //-------------------------------------------------
        public MemoryContainer<rgb_t> get_adjusted_palette(render_container container, ref u32 out_length)  //const rgb_t *get_adjusted_palette(render_container &container, u32 &out_length);
        {
            // override the palette with our adjusted palette
            switch (m_format)
            {
                case texture_format.TEXFORMAT_PALETTE16:

                    assert(m_bitmap.palette() != null);

                    // return our adjusted palette
                    return container.bcg_lookup_table(m_format, out out_length, m_bitmap.palette());

                case texture_format.TEXFORMAT_RGB32:
                case texture_format.TEXFORMAT_ARGB32:
                case texture_format.TEXFORMAT_YUY16:

                    // if no adjustment necessary, return NULL
                    if (!container.has_brightness_contrast_gamma_changes())
                        return null;

                    return container.bcg_lookup_table(m_format, out out_length);

                default:
                    assert(false);
                    break;
            }

            return null;
        }
    }


    // ======================> render_container
    // a render_container holds a list of items and an orientation for the entire collection
    public class render_container : global_object,
                                    simple_list_item<render_container>,
                                    IDisposable
    {
        // user settings describes the collected user-controllable settings
        public class user_settings
        {
            // public state
            public int                 m_orientation;      // orientation
            public float               m_brightness;       // brightness
            public float               m_contrast;         // contrast
            public float               m_gamma;            // gamma
            public float               m_xscale;           // horizontal scale factor
            public float               m_yscale;           // vertical scale factor
            public float               m_xoffset;          // horizontal offset
            public float               m_yoffset;          // vertical offset

            // construction/destruction
            //-------------------------------------------------
            //  user_settings - constructor
            //-------------------------------------------------
            public user_settings()
            {
                m_orientation = 0;
                m_brightness = 1.0f;
                m_contrast = 1.0f;
                m_gamma = 1.0f;
                m_xscale = 1.0f;
                m_yscale = 1.0f;
                m_xoffset = 0.0f;
                m_yoffset = 0.0f;
            }
        }


        // an item describes a high level primitive that is added to a container
        public class item : simple_list_item<item>
        {
            // internal state
            item m_next;             // pointer to the next element in the list
            public u8 m_type;             // type of element
            public render_bounds m_bounds = new render_bounds();           // bounds of the element
            public render_color m_color = new render_color();            // RGBA factors
            public u32 m_flags;            // option flags
            public u32 m_internal;         // internal flags
            public float m_width;            // width of the line (lines only)
            public render_texture m_texture;          // pointer to the source texture (quads only)


            public item()
            {
                m_next = null;
                m_type = 0;
                m_flags = 0;
                m_internal = 0;
                m_width = 0;
                m_texture = null;
            }


            // getters
            public item next() { return m_next; }
            public item m_next_get() { return m_next; }
            public void m_next_set(item value) { m_next = value; }

            public u8 type() { return m_type; }
            public render_bounds bounds() { return m_bounds; }
            public render_color color() { return m_color; }
            public u32 flags() { return m_flags; }
            public u32 internal_flags() { return m_internal; }
            public float width() { return m_width; }
            public render_texture texture() { return m_texture; }
        }


        // internal state
        render_container m_next;                 // the next container in the list
        render_manager m_manager;              // reference back to the owning manager
        simple_list<item> m_itemlist = new simple_list<item>();             // head of the item list
        fixed_allocator<item> m_item_allocator = new fixed_allocator<item>();       // free container items
        screen_device m_screen;               // the screen device
        user_settings m_user = new user_settings();                 // user settings
        bitmap_argb32 m_overlaybitmap;        // overlay bitmap
        render_texture m_overlaytexture;       // overlay texture
        palette_client m_palclient;       // client to the screen palette
        std.vector<rgb_t> m_bcglookup = new std.vector<rgb_t>();   // full palette lookup with bcg adjustments
        std.vector<rgb_t> m_bcglookup256 = new std.vector<rgb_t>(0x400);         // lookup table for brightness/contrast/gamma


        // construction/destruction

        //-------------------------------------------------
        //  render_container - constructor
        //-------------------------------------------------
        public render_container(render_manager manager, screen_device screen = null)
        {
            m_next = null;
            m_manager = manager;
            m_screen = screen;
            m_overlaybitmap = null;
            m_overlaytexture = null;


            // make sure it is empty
            empty();

            // if we have a screen, read and apply the options
            if (m_screen != null)
            {
                // set the initial orientation and brightness/contrast/gamma
                m_user.m_orientation = m_screen.orientation();
                m_user.m_brightness = manager.machine().options().brightness();
                m_user.m_contrast = manager.machine().options().contrast();
                m_user.m_gamma = manager.machine().options().gamma();
                // palette client will be allocated later
            }

            recompute_lookups();
        }

        ~render_container()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            // free all the container items
            empty();

            // free the overlay texture
            m_manager.texture_free(m_overlaytexture);

            if (m_palclient != null)
                m_palclient.Dispose();

            m_isDisposed = true;
        }


        // getters
        public render_container next() { return m_next; }
        public render_container m_next_get() { return m_next; }
        public void m_next_set(render_container value) { m_next = value; }

        public screen_device screen() { return m_screen; }
        public render_manager manager() { return m_manager; }
        public render_texture overlay() { return m_overlaytexture; }
        public int orientation() { return m_user.m_orientation; }
        public float xscale() { return m_user.m_xscale; }
        public float yscale() { return m_user.m_yscale; }
        public float xoffset() { return m_user.m_xoffset; }
        public float yoffset() { return m_user.m_yoffset; }
        bool is_empty() { return m_itemlist.empty(); }
        public user_settings get_user_settings() { return m_user; }


        // setters

        //-------------------------------------------------
        //  set_overlay - set the overlay bitmap for the
        //  container
        //-------------------------------------------------
        public void set_overlay(bitmap_argb32 bitmap)
        {
            // free any existing texture
            m_manager.texture_free(m_overlaytexture);

            // set the new data and allocate the texture
            m_overlaybitmap = bitmap;
            if (m_overlaybitmap != null)
            {
                m_overlaytexture = m_manager.texture_alloc(overlay_scale);
                m_overlaytexture.set_bitmap(bitmap, bitmap.cliprect(), texture_format.TEXFORMAT_ARGB32);
            }
        }

        //-------------------------------------------------
        //  set_user_settings - set the current user
        //  settings for a container
        //-------------------------------------------------
        public void set_user_settings(user_settings settings)
        {
            m_user = settings;
            recompute_lookups();
        }


        // empty the item list
        public void empty() { m_item_allocator.reclaim_all(m_itemlist); }


        // add items to the list

        //-------------------------------------------------
        //  add_line - add a line item to this container
        //-------------------------------------------------
        public void add_line(float x0, float y0, float x1, float y1, float width, rgb_t argb, UInt32 flags)
        {
            item newitem = add_generic(render_global.CONTAINER_ITEM_LINE, x0, y0, x1, y1, argb);
            newitem.m_width = width;
            newitem.m_flags = flags;
        }

        //-------------------------------------------------
        //  add_quad - add a quad item to this container
        //-------------------------------------------------
        public void add_quad(float x0, float y0, float x1, float y1, rgb_t argb, render_texture texture, UInt32 flags)
        {
            item newitem = add_generic(render_global.CONTAINER_ITEM_QUAD, x0, y0, x1, y1, argb);
            newitem.m_texture = texture;
            newitem.m_flags = flags;
        }

        //-------------------------------------------------
        //  add_char - add a char item to this container
        //-------------------------------------------------
        public void add_char(float x0, float y0, float height, float aspect, rgb_t argb, render_font font, UInt16 ch)
        {
            // compute the bounds of the character cell and get the texture
            render_bounds bounds = new render_bounds();
            bounds.x0 = x0;
            bounds.y0 = y0;
            render_texture texture = font.get_char_texture_and_bounds(height, aspect, ch, ref bounds);

            // add it like a quad
            item newitem = add_generic(render_global.CONTAINER_ITEM_QUAD, bounds.x0, bounds.y0, bounds.x1, bounds.y1, argb);
            newitem.m_texture = texture;
            newitem.m_flags = render_global.PRIMFLAG_TEXORIENT(emucore_global.ROT0) | PRIMFLAG_BLENDMODE(rendertypes_global.BLENDMODE_ALPHA) | render_global.PRIMFLAG_PACKABLE;
            newitem.m_internal = render_global.INTERNAL_FLAG_CHAR;
        }

        public void add_point(float x0, float y0, float diameter, rgb_t argb, UInt32 flags) { add_line(x0, y0, x0, y0, diameter, argb, flags); }

        public void add_rect(float x0, float y0, float x1, float y1, rgb_t argb, UInt32 flags) { add_quad(x0, y0, x1, y1, argb, null, flags); }


        // brightness/contrast/gamma helpers

        public bool has_brightness_contrast_gamma_changes() { return (m_user.m_brightness != 1.0f || m_user.m_contrast != 1.0f || m_user.m_gamma != 1.0f); }

        //-------------------------------------------------
        //  apply_brightness_contrast_gamma - apply the
        //  container's brightess, contrast, and gamma to
        //  an 8-bit value
        //-------------------------------------------------
        byte apply_brightness_contrast_gamma(byte value) { return rendutil_global.apply_brightness_contrast_gamma(value, m_user.m_brightness, m_user.m_contrast, m_user.m_gamma); }

        //-------------------------------------------------
        //  apply_brightness_contrast_gamma_fp - apply the
        //  container's brightess, contrast, and gamma to
        //  a floating-point value
        //-------------------------------------------------
        public float apply_brightness_contrast_gamma_fp(float value) { return rendutil_global.apply_brightness_contrast_gamma_fp(value, m_user.m_brightness, m_user.m_contrast, m_user.m_gamma); }

        //-------------------------------------------------
        //  bcg_lookup_table - return the appropriate
        //  brightness/contrast/gamma lookup table for a
        //  given texture mode
        //-------------------------------------------------
        public MemoryContainer<rgb_t> bcg_lookup_table(texture_format texformat, out u32 out_length, palette_t palette = null)  //const rgb_t *bcg_lookup_table(int texformat, u32 &out_length, palette_t *palette = nullptr);
        {
            switch (texformat)
            {
                case texture_format.TEXFORMAT_PALETTE16:
                    if (m_palclient == null) // if adjusted palette hasn't been created yet, create it
                    {
                        m_palclient = new palette_client(palette);
                        m_bcglookup.resize(palette.max_index());
                        recompute_lookups();
                    }
                    //assert (palette == &m_palclient->palette());
                    out_length = (u32)palette.max_index();
                    return m_bcglookup;

                case texture_format.TEXFORMAT_RGB32:
                case texture_format.TEXFORMAT_ARGB32:
                case texture_format.TEXFORMAT_YUY16:
                    out_length = (UInt32)m_bcglookup256.Count;  //ARRAY_LENGTH(m_bcglookup256);
                    return m_bcglookup256;

                default:
                    out_length = 0;
                    return null;
            }
        }


        // generic screen overlay scaler
        //-------------------------------------------------
        //  overlay_scale - scaler for an overlay
        //-------------------------------------------------
        void overlay_scale(bitmap_argb32 dest, bitmap_argb32 source, rectangle sbounds, layout_element.texture param)  //, void *param)
        {
            // simply replicate the source bitmap over the target
            for (int y = 0; y < dest.height(); y++)
            {
                throw new emu_unimplemented();
#if false
                u32 const *const src = &source.pix(y % source.height());
                u32 *dst = &dest.pix(y);
                int sx = 0;

                // loop over columns
                for (int x = 0; x < dest.width(); x++)
                {
                    *dst++ = src[sx++];
                    if (sx >= source.width())
                        sx = 0;
                }
#endif
            }
        }


        // internal helpers

        public simple_list<item> items() { return m_itemlist; }


        //-------------------------------------------------
        //  add_generic - add a generic item to a
        //  container
        //-------------------------------------------------
        item add_generic(u8 type, float x0, float y0, float x1, float y1, rgb_t argb)
        {
            item newitem = m_item_allocator.alloc();

            //throw new emu_unimplemented();
#if false
            assert(x0 == x0);
            assert(x1 == x1);
            assert(y0 == y0);
            assert(y1 == y1);
#endif

            // copy the data into the new item
            newitem.m_type = type;
            newitem.m_bounds.x0 = x0;
            newitem.m_bounds.y0 = y0;
            newitem.m_bounds.x1 = x1;
            newitem.m_bounds.y1 = y1;
            newitem.m_color.r = (float)argb.r() * (1.0f / 255.0f);
            newitem.m_color.g = (float)argb.g() * (1.0f / 255.0f);
            newitem.m_color.b = (float)argb.b() * (1.0f / 255.0f);
            newitem.m_color.a = (float)argb.a() * (1.0f / 255.0f);
            newitem.m_flags = 0;
            newitem.m_internal = 0;
            newitem.m_width = 0;
            newitem.m_texture = null;

            // add the item to the container
            return m_itemlist.append(newitem);
        }


        //-------------------------------------------------
        //  recompute_lookups - recompute the lookup table
        //  for the render container
        //-------------------------------------------------
        void recompute_lookups()
        {
            // recompute the 256 entry lookup table
            for (int i = 0; i < 0x100; i++)
            {
                byte adjustedval = apply_brightness_contrast_gamma((byte)i);
                m_bcglookup256[i + 0x000] = new rgb_t((UInt32)adjustedval << 0);
                m_bcglookup256[i + 0x100] = new rgb_t((UInt32)adjustedval << 8);
                m_bcglookup256[i + 0x200] = new rgb_t((UInt32)adjustedval << 16);
                m_bcglookup256[i + 0x300] = new rgb_t((UInt32)adjustedval << 24);
            }

            // recompute the palette entries
            if (m_palclient != null)
            {
                palette_t palette = m_palclient.palette();
                MemoryContainer<rgb_t> adjusted_palette = palette.entry_list_adjusted();  //rgb_t *adjusted_palette = palette.entry_list_adjusted();
                int colors = palette.max_index();

                if (has_brightness_contrast_gamma_changes())
                {
                    for (int i = 0; i < colors; i++)
                    {
                        rgb_t newval = adjusted_palette[i];
                        m_bcglookup[i] = new rgb_t((newval & 0xff000000) |
                                         m_bcglookup256[0x200 + newval.r()] |
                                         m_bcglookup256[0x100 + newval.g()] |
                                         m_bcglookup256[0x000 + newval.b()]);
                    }
                }
                else
                {
                    memcpy(m_bcglookup, adjusted_palette, (UInt32)colors);  //memcpy(&m_bcglookup[0], adjusted_palette, colors * sizeof(rgb_t));
                }
            }
        }


        //-------------------------------------------------
        //  update_palette - update any dirty palette
        //  entries
        //-------------------------------------------------
        public void update_palette()
        {
            // skip if no client
            if (m_palclient == null)
                return;

            // get the dirty list
            u32 mindirty;
            u32 maxdirty;
            MemoryContainer<u32> dirty = m_palclient.dirty_list(out mindirty, out maxdirty);  //const u32 *dirty = m_palclient->dirty_list(mindirty, maxdirty);

            // iterate over dirty items and update them
            if (dirty != null)
            {
                palette_t palette = m_palclient.palette();
                MemoryContainer<rgb_t> adjusted_palette = palette.entry_list_adjusted();

                if (has_brightness_contrast_gamma_changes())
                {
                    // loop over chunks of 32 entries, since we can quickly examine 32 at a time
                    for (u32 entry32 = mindirty / 32; entry32 <= maxdirty / 32; entry32++)
                    {
                        u32 dirtybits = dirty[(int)entry32];
                        if (dirtybits != 0)
                        {
                            // this chunk of 32 has dirty entries; fix them up
                            for (u32 entry = 0; entry < 32; entry++)
                            {
                                if ((dirtybits & (1 << (int)entry)) != 0)
                                {
                                    u32 finalentry = entry32 * 32 + entry;
                                    rgb_t newval = adjusted_palette[(int)finalentry];
                                    m_bcglookup[finalentry] = new rgb_t((newval & 0xff000000) |
                                                                    m_bcglookup256[0x200 + newval.r()] |
                                                                    m_bcglookup256[0x100 + newval.g()] |
                                                                    m_bcglookup256[0x000 + newval.b()]);
                                }
                            }
                        }
                    }
                }
                else
                {
                    memcpy(new Pointer<rgb_t>(m_bcglookup, (int)mindirty), new Pointer<rgb_t>(adjusted_palette, (int)mindirty), (maxdirty - mindirty + 1));  //memcpy(&m_bcglookup[mindirty], &adjusted_palette[mindirty], (maxdirty - mindirty + 1) * sizeof(rgb_t));
                }
            }
        }
    }


    // ======================> render_target
    // a render_target describes a surface that is being rendered to
    public class render_target : global_object, simple_list_item<render_target>
    {
        // constants
        const int NUM_PRIMLISTS = 3;
        const int MAX_CLEAR_EXTENTS = 1000;


        // internal state
        render_target m_next;                     // link to next target
        render_manager m_manager;                  // reference to our owning manager
        std.list<layout_file> m_filelist;                // list of layout files
        render_target_view_mask_vector m_views = new render_target_view_mask_vector();                    // views we consider
        unsigned m_curview;                  // current view index
        u32 m_flags;                    // creation flags
        render_primitive_list [] m_primlist = new render_primitive_list[NUM_PRIMLISTS];  // list of primitives
        int m_listindex;                // index of next primlist to use
        s32 m_width;                    // width in pixels
        s32 m_height;                   // height in pixels
        render_bounds m_bounds = new render_bounds();                   // bounds of the target
        bool m_keepaspect;               // constrain aspect ratio
        bool m_int_overscan;             // allow overscan on integer scaled targets
        float m_pixel_aspect;             // aspect ratio of individual pixels
        int m_scale_mode;               // type of scale to apply
        int m_int_scale_x;              // horizontal integer scale factor
        int m_int_scale_y;              // vertical integer scale factor
        float m_max_refresh;              // maximum refresh rate, 0 or if none
        int m_orientation;              // orientation
        render_layer_config m_layerconfig;              // layer configuration
        std.vector<bool> m_hit_test = new std.vector<bool>();                 // used when mapping points to inputs
        layout_view m_base_view;                // the view at the time of first frame
        int m_base_orientation;         // the orientation at the time of first frame
        render_layer_config m_base_layerconfig = new render_layer_config();         // the layer configuration at the time of first frame
        int m_maxtexwidth;              // maximum width of a texture
        int m_maxtexheight;             // maximum height of a texture
        simple_list<render_container> m_debug_containers = new simple_list<render_container>();   // list of debug containers
        s32 m_clear_extent_count;       // number of clear extents
        s32 [] m_clear_extents = new s32[MAX_CLEAR_EXTENTS]; // array of clear extents
        bool m_transform_container;      // determines whether the screen container is transformed by the core renderer,
                                         // otherwise the respective render API will handle the transformation (scale, offset)
        bool m_external_artwork;         // external artwork was loaded (driver file or override)


        // construction/destruction
        //-------------------------------------------------
        //  render_target - constructor
        //-------------------------------------------------
        public render_target(render_manager manager, internal_layout layoutfile = null, u32 flags = 0) : this(manager, layoutfile, flags, constructor_impl_t.CONSTRUCTOR_IMPL) { }
        public render_target(render_manager manager, util.xml.data_node layout, u32 flags = 0) : this(manager, layout, flags, constructor_impl_t.CONSTRUCTOR_IMPL) { }

        enum constructor_impl_t { CONSTRUCTOR_IMPL };
        //template <typename T> render_target(render_manager &manager, T&& layout, u32 flags, constructor_impl_t);
        render_target(render_manager manager, object layout, u32 flags, constructor_impl_t impl)
        {
            for (int i = 0; i < m_primlist.Length; i++)
                m_primlist[i] = new render_primitive_list();

            m_next = null;
            m_manager = manager;
            m_filelist = new std.list<layout_file>();
            m_curview = 0;
            m_flags = flags;
            m_listindex = 0;
            m_width = 640;
            m_height = 480;
            m_pixel_aspect = 0.0f;
            m_max_refresh = 0;
            m_orientation = 0;
            m_base_view = null;
            m_base_orientation = (int)emucore_global.ROT0;
            m_maxtexwidth = 65536;
            m_maxtexheight = 65536;
            m_transform_container = true;
            m_external_artwork = false;


            // determine the base layer configuration based on options
            m_base_layerconfig.set_zoom_to_screen(manager.machine().options().artwork_crop());

            // aspect and scale options
            m_keepaspect = manager.machine().options().keep_aspect() && (flags & render_global.RENDER_CREATE_HIDDEN) == 0;
            m_int_overscan = manager.machine().options().int_overscan();
            m_int_scale_x = manager.machine().options().int_scale_x();
            m_int_scale_y = manager.machine().options().int_scale_y();
            if (m_manager.machine().options().auto_stretch_xy())
                m_scale_mode = render_global.SCALE_FRACTIONAL_AUTO;
            else if (manager.machine().options().uneven_stretch_x())
                m_scale_mode = render_global.SCALE_FRACTIONAL_X;
            else if (manager.machine().options().uneven_stretch_y())
                m_scale_mode = render_global.SCALE_FRACTIONAL_Y;
            else if (manager.machine().options().uneven_stretch())
                m_scale_mode = render_global.SCALE_FRACTIONAL;
            else
                m_scale_mode = render_global.SCALE_INTEGER;

            // determine the base orientation based on options
            if (!manager.machine().options().rotate())
                m_base_orientation = rendutil_global.orientation_reverse((int)(manager.machine().system().flags & machine_flags.type.MASK_ORIENTATION));

            // rotate left/right
            if (manager.machine().options().ror() || (manager.machine().options().auto_ror() && ((UInt32)manager.machine().system().flags & emucore_global.ORIENTATION_SWAP_XY) != 0))
                m_base_orientation = rendutil_global.orientation_add((int)emucore_global.ROT90, m_base_orientation);
            if (manager.machine().options().rol() || (manager.machine().options().auto_rol() && ((UInt32)manager.machine().system().flags & emucore_global.ORIENTATION_SWAP_XY) != 0))
                m_base_orientation = rendutil_global.orientation_add((int)emucore_global.ROT270, m_base_orientation);

            // flip X/Y
            if (manager.machine().options().flipx())
                m_base_orientation ^= (int)emucore_global.ORIENTATION_FLIP_X;
            if (manager.machine().options().flipy())
                m_base_orientation ^= (int)emucore_global.ORIENTATION_FLIP_Y;

            // set the orientation and layerconfig equal to the base
            m_orientation = m_base_orientation;
            m_layerconfig = m_base_layerconfig;

            // load the layout files
            //load_layout_files(std::forward<T>(layout), flags & RENDER_CREATE_SINGLE_FILE);
            if (layout == null || layout is internal_layout)
                load_layout_files((internal_layout)layout, (flags & render_global.RENDER_CREATE_SINGLE_FILE) != 0);
            else if (layout is util.xml.data_node)
                load_layout_files((util.xml.data_node)layout, (flags & render_global.RENDER_CREATE_SINGLE_FILE) != 0);
            else
                throw new emu_unimplemented();

            foreach (layout_file file in m_filelist)
            {
                foreach (layout_view view in file.views())
                {
                    if ((m_flags & render_global.RENDER_CREATE_NO_ART) == 0 || !view.has_art())
                        m_views.emplace_back(new std.pair<layout_view, ioport_value>(view, view.default_visibility_mask()));
                }
            }


            // set the current view to the first one
            set_view(0);

            // make us the UI target if there is none
            if (!hidden() && manager.ui_target() == null)
                manager.set_ui_target(this);
        }


        // getters
        public render_target next() { return m_next; }
        public render_target m_next_get() { return m_next; }
        public void m_next_set(render_target value) { m_next = value; }

        render_manager manager() { return m_manager; }
        public int width() { return m_width; }
        public int height() { return m_height; }
        public float pixel_aspect() { return m_pixel_aspect; }
        //bool keepaspect() const { return m_keepaspect; }
        int scale_mode() { return m_scale_mode; }
        public float max_update_rate() { return m_max_refresh; }
        public int orientation() { return m_orientation; }
        //render_layer_config layer_config() const { return m_layerconfig; }
        public layout_view current_view() { return m_views[m_curview].first; }
        public unsigned view() { return m_curview; }
        public bool external_artwork() { return m_external_artwork; }
        public bool hidden() { return ((m_flags & render_global.RENDER_CREATE_HIDDEN) != 0); }

        //-------------------------------------------------
        //  is_ui_target - return true if this is the
        //  UI target
        //-------------------------------------------------
        bool is_ui_target() { return (this == m_manager.ui_target()); }

        //-------------------------------------------------
        //  index - return the index of this target
        //-------------------------------------------------
        int index() { return m_manager.targetlist().indexof(this); }


        // setters

        //-------------------------------------------------
        //  set_bounds - set the bounds and pixel aspect
        //  of a target
        //-------------------------------------------------
        public void set_bounds(int width, int height, float pixel_aspect = 0)
        {
            m_width = width;
            m_height = height;
            m_bounds.x0 = 0;
            m_bounds.y0 = 0;
            m_bounds.x1 = (float)width;
            m_bounds.y1 = (float)height;
            m_pixel_aspect = pixel_aspect != 0 ? pixel_aspect : 1;
        }

        //void set_max_update_rate(float updates_per_second) { m_max_refresh = updates_per_second; }
        //void set_orientation(int orientation) { m_orientation = orientation; }

        //-------------------------------------------------
        //  set_view - dynamically change the view for
        //  a target
        //-------------------------------------------------
        public void set_view(unsigned viewindex)
        {
            if (m_views.size() > viewindex)
            {
                m_curview = viewindex;
                current_view().recompute(visibility_mask(), m_layerconfig.zoom_to_screen());
                current_view().preload();
            }
        }

        //void set_max_texture_size(int maxwidth, int maxheight);
        //void set_transform_container(bool transform_container) { m_transform_container = transform_container; }
        //void set_keepaspect(bool keepaspect) { m_keepaspect = keepaspect; }
        //void set_scale_mode(int scale_mode) { m_scale_mode = scale_mode; }


        // layer config getters
        //bool screen_overlay_enabled() const { return m_layerconfig.screen_overlay_enabled(); }
        //bool zoom_to_screen() const { return m_layerconfig.zoom_to_screen(); }
        u32 visibility_mask() { return m_views[m_curview].second; }


        // layer config setters
        //void set_visibility_toggle(unsigned index, bool enable);
        public void set_screen_overlay_enabled(bool enable) { m_layerconfig.set_screen_overlay_enabled(enable); update_layer_config(); }
        public void set_zoom_to_screen(bool zoom) { m_layerconfig.set_zoom_to_screen(zoom); update_layer_config(); }


        // view configuration helper
        //-------------------------------------------------
        //  configured_view - select a view for this
        //  target based on the configuration parameters
        //-------------------------------------------------
        public unsigned configured_view(string viewname, int targetindex, int numtargets)
        {
            layout_view view = null;

            // if it isn't "auto" or an empty string, try to match it as a view name prefix
            if (viewname != null && !string.IsNullOrEmpty(viewname) && strcmp(viewname, "auto") != 0)
            {
                // scan for a matching view name
                size_t viewlen = (size_t)strlen(viewname);
                for (unsigned i = 0; (view == null) && (m_views.size() > i); ++i)
                {
                    if (core_strnicmp(m_views[i].first.name().c_str(), viewname, viewlen) == 0)
                        view = m_views[i].first;
                }
            }

            // if we don't have a match, default to the nth view
            std.vector<screen_device> screens = new std.vector<screen_device>();  //std::vector<std::reference_wrapper<screen_device> > screens;
            foreach (screen_device screen in new screen_device_enumerator(m_manager.machine().root_device()))
                screens.push_back(screen);

            if ((view == null) && !screens.empty())
            {
                // if we have enough targets to be one per screen, assign in order
                if (numtargets >= screens.size())
                {
                    // find the first view with this screen and this screen only
                    screen_device screen = screens[index() % screens.size()];
                    for (unsigned i = 0; (view == null) && (m_views.size() > i); ++i)
                    {
                        foreach (layout_view.item viewitem in m_views[i].first.items())
                        {
                            screen_device viewscreen = viewitem.screen();
                            if (viewscreen == screen)
                            {
                                view = m_views[i].first;
                            }
                            else if (viewscreen != null)
                            {
                                view = null;
                                break;
                            }
                        }
                    }
                }

                // otherwise, find the first view that has all the screens
                if (view == null)
                {
                    for (unsigned i = 0; (view == null) && (m_views.size() > i); ++i)
                    {
                        layout_view curview = m_views[i].first;
                        if (std.find_if(screens, (screen_device screen) => { return !curview.has_screen(screen); }) == default)
                            view = curview;
                    }
                }
            }

            // make sure it's a valid view
            return view != null ? (unsigned)view_index(view) : 0;
        }


        // view information

        //char const *view_name(unsigned index);


        // bounds computations

        //-------------------------------------------------
        //  compute_visible_area - compute the visible
        //  area for the given target with the current
        //  layout and proposed new parameters
        //-------------------------------------------------
        void compute_visible_area(int target_width, int target_height, float target_pixel_aspect, int target_orientation, out int visible_width, out int visible_height)
        {
            switch (m_scale_mode)
            {
                case render_global.SCALE_FRACTIONAL:
                {
                    float width;
                    float height;
                    float scale;

                    // constrained case
                    if (m_keepaspect)
                    {
                        // start with the aspect ratio of the square pixel layout
                        width = current_view().effective_aspect();
                        height = 1.0f;

                        // first apply target orientation
                        if ((target_orientation & emucore_global.ORIENTATION_SWAP_XY) != 0)
                            std.swap(ref width, ref height);

                        // apply the target pixel aspect ratio
                        height *= target_pixel_aspect;

                        // based on the height/width ratio of the source and target, compute the scale factor
                        if (width / height > (float)target_width / (float)target_height)
                            scale = (float)target_width / width;
                        else
                            scale = (float)target_height / height;
                    }

                    // stretch-to-fit case
                    else
                    {
                        width = (float)target_width;
                        height = (float)target_height;
                        scale = 1.0f;
                    }

                    // set the final width/height
                    visible_width = (int)rendutil_global.render_round_nearest(width * scale);
                    visible_height = (int)rendutil_global.render_round_nearest(height * scale);
                    break;
                }

                default:
                {
                    // get source size and aspect
                    int src_width;
                    int src_height;
                    compute_minimum_size(out src_width, out src_height);
                    float src_aspect = current_view().effective_aspect();

                    // apply orientation if required
                    if ((target_orientation & emucore_global.ORIENTATION_SWAP_XY) != 0)
                        src_aspect = 1.0f / src_aspect;

                    // get target aspect
                    float target_aspect = (float)target_width / (float)target_height * target_pixel_aspect;
                    bool target_is_portrait = (target_aspect < 1.0f);

                    // apply automatic axial stretching if required
                    int scale_mode = m_scale_mode;
                    if (m_scale_mode == render_global.SCALE_FRACTIONAL_AUTO)
                    {
                        bool is_rotated = (((UInt32)m_manager.machine().system().flags & emucore_global.ORIENTATION_SWAP_XY) ^ (target_orientation & emucore_global.ORIENTATION_SWAP_XY)) != 0;
                        scale_mode = is_rotated ^ target_is_portrait ? render_global.SCALE_FRACTIONAL_Y : render_global.SCALE_FRACTIONAL_X;
                    }

                    // determine the scale mode for each axis
                    bool x_is_integer = !((!target_is_portrait && scale_mode == render_global.SCALE_FRACTIONAL_X) || (target_is_portrait && scale_mode == render_global.SCALE_FRACTIONAL_Y));
                    bool y_is_integer = !((target_is_portrait && scale_mode == render_global.SCALE_FRACTIONAL_X) || (!target_is_portrait && scale_mode == render_global.SCALE_FRACTIONAL_Y));

                    // first compute scale factors to fit the screen
                    float xscale = (float)target_width / src_width;
                    float yscale = (float)target_height / src_height;
                    float maxxscale = Math.Max(1.0f, (float)(m_int_overscan ? rendutil_global.render_round_nearest(xscale) : Math.Floor(xscale)));
                    float maxyscale = Math.Max(1.0f, (float)(m_int_overscan ? rendutil_global.render_round_nearest(yscale) : Math.Floor(yscale)));

                    // now apply desired scale mode and aspect correction
                    if (m_keepaspect && target_aspect > src_aspect) xscale *= src_aspect / target_aspect * (maxyscale / yscale);
                    if (m_keepaspect && target_aspect < src_aspect) yscale *= target_aspect / src_aspect * (maxxscale / xscale);
                    if (x_is_integer) xscale = Math.Min(maxxscale, Math.Max(1, rendutil_global.render_round_nearest(xscale)));
                    if (y_is_integer) yscale = Math.Min(maxyscale, Math.Max(1, rendutil_global.render_round_nearest(yscale)));

                    // check if we have user defined scale factors, if so use them instead
                    int user_scale_x = target_is_portrait? m_int_scale_y : m_int_scale_x;
                    int user_scale_y = target_is_portrait? m_int_scale_x : m_int_scale_y;
                    xscale = user_scale_x > 0 ? user_scale_x : xscale;
                    yscale = user_scale_y > 0 ? user_scale_y : yscale;

                    // set the final width/height
                    visible_width = (int)rendutil_global.render_round_nearest(src_width * xscale);
                    visible_height = (int)rendutil_global.render_round_nearest(src_height * yscale);
                    break;
                }
            }
        }


        //-------------------------------------------------
        //  compute_minimum_size - compute the "minimum"
        //  size of a target, which is the smallest bounds
        //  that will ensure at least 1 target pixel per
        //  source pixel for all included screens
        //-------------------------------------------------
        public void compute_minimum_size(out int minwidth, out int minheight)
        {
            float maxxscale = 1.0f;
            float maxyscale = 1.0f;
            int screens_considered = 0;

            // early exit in case we are called between device teardown and render teardown
            if (m_manager.machine().phase() == machine_phase.EXIT)
            {
                minwidth = 640;
                minheight = 480;
                return;
            }

            if (m_views.empty())
                throw new emu_fatalerror("Mandatory artwork is missing");

            // scan the current view for all screens
            foreach (layout_view.item curitem in current_view().items())
            {
                // iterate over items in the layer
                screen_device screen = curitem.screen();
                if (screen != null)
                {
                    // use a hard-coded default visible area for vector screens
                    rectangle vectorvis = new rectangle(0, 639, 0, 479);
                    rectangle visarea = (screen.screen_type() == screen_type_enum.SCREEN_TYPE_VECTOR) ? vectorvis : screen.visible_area();

                    // apply target orientation to the bounds
                    render_bounds bounds = curitem.bounds();
                    render_global.apply_orientation(bounds, m_orientation);
                    render_global.normalize_bounds(bounds);

                    // based on the orientation of the screen container, check the bitmap
                    float xscale, yscale;
                    if ((rendutil_global.orientation_add(m_orientation, screen.container().orientation()) & ORIENTATION_SWAP_XY) == 0)
                    {
                        xscale = (float)visarea.width() / bounds.width();
                        yscale = (float)visarea.height() / bounds.height();
                    }
                    else
                    {
                        xscale = (float)visarea.height() / bounds.width();
                        yscale = (float)visarea.width() / bounds.height();
                    }

                    // pick the greater
                    maxxscale = std.max(xscale, maxxscale);
                    maxyscale = std.max(yscale, maxyscale);
                    screens_considered++;
                }
            }

            // if there were no screens considered, pick a nominal default
            if (screens_considered == 0)
            {
                maxxscale = 640.0f;
                maxyscale = 480.0f;
            }

            // round up
            minwidth = (int)rendutil_global.render_round_nearest(maxxscale);
            minheight = (int)rendutil_global.render_round_nearest(maxyscale);
        }


        // get a primitive list
        //-------------------------------------------------
        //  get_primitives - return a list of primitives
        //  for a given render target
        //-------------------------------------------------
        public render_primitive_list get_primitives()
        {
            // remember the base values if this is the first frame
            if (m_base_view == null)
                m_base_view = current_view();

            // switch to the next primitive list
            render_primitive_list list = m_primlist[m_listindex];
            m_listindex = (m_listindex + 1) % m_primlist.Length;
            list.acquire_lock();

            // free any previous primitives
            list.release_all();

            // compute the visible width/height
            int viswidth;
            int visheight;
            compute_visible_area(m_width, m_height, m_pixel_aspect, m_orientation, out viswidth, out visheight);

            // create a root transform for the target
            object_transform root_xform = new object_transform();
            root_xform.xoffs = (float)(m_width - viswidth) / 2;
            root_xform.yoffs = (float)(m_height - visheight) / 2;
            root_xform.xscale = (float)viswidth;
            root_xform.yscale = (float)visheight;
            root_xform.color = new render_color();
            root_xform.color.r = root_xform.color.g = root_xform.color.b = root_xform.color.a = 1.0f;
            root_xform.orientation = m_orientation;
            root_xform.no_center = false;

            if (m_manager.machine().phase() >= machine_phase.RESET)
            {
                // we're running - iterate over items in the view
                current_view().prepare_items();
                foreach (layout_view.item curitem in current_view().visible_items())
                {
                    // first apply orientation to the bounds
                    render_bounds bounds = curitem.bounds();
                    render_global.apply_orientation(bounds, root_xform.orientation);
                    render_global.normalize_bounds(bounds);

                    // apply the transform to the item
                    object_transform item_xform = new object_transform();
                    item_xform.xoffs = root_xform.xoffs + bounds.x0 * root_xform.xscale;
                    item_xform.yoffs = root_xform.yoffs + bounds.y0 * root_xform.yscale;
                    item_xform.xscale = (bounds.x1 - bounds.x0) * root_xform.xscale;
                    item_xform.yscale = (bounds.y1 - bounds.y0) * root_xform.yscale;
                    item_xform.color = curitem.color() * root_xform.color;
                    item_xform.orientation = rendutil_global.orientation_add(curitem.orientation(), root_xform.orientation);
                    item_xform.no_center = false;

                    // if there is no associated element, it must be a screen element
                    if (curitem.screen() != null)
                        add_container_primitives(list, root_xform, item_xform, curitem.screen().container(), curitem.blend_mode());
                    else
                        add_element_primitives(list, item_xform, curitem.element(), curitem.element_state(), curitem.blend_mode());
                }
            }
            else
            {
                // if we are not in the running stage, draw an outer box
                render_primitive prim = list.alloc(render_primitive.primitive_type.QUAD);
                prim.bounds.set_xy(0.0f, 0.0f, (float)m_width, (float)m_height);
                prim.full_bounds = prim.bounds;
                prim.color.set(1.0f, 0.1f, 0.1f, 0.1f);
                prim.texture.base_ = null;
                prim.flags = PRIMFLAG_BLENDMODE(rendertypes_global.BLENDMODE_ALPHA);
                list.append(prim);

                if (m_width > 1 && m_height > 1)
                {
                    prim = list.alloc(render_primitive.primitive_type.QUAD);
                    prim.bounds.set_xy(1.0f, 1.0f, (float)(m_width - 1), (float)(m_height - 1));
                    prim.full_bounds = prim.bounds;
                    prim.color.set(1.0f, 0.0f, 0.0f, 0.0f);
                    prim.texture.base_ = null;
                    prim.flags = PRIMFLAG_BLENDMODE(rendertypes_global.BLENDMODE_ALPHA);
                    list.append(prim);
                }
            }

            // process the debug containers
            foreach (render_container debug in m_debug_containers)
            {
                object_transform ui_xform = new object_transform();
                ui_xform.xoffs = 0;
                ui_xform.yoffs = 0;
                ui_xform.xscale = (float)m_width;
                ui_xform.yscale = (float)m_height;
                ui_xform.color = new render_color();
                ui_xform.color.r = ui_xform.color.g = ui_xform.color.b = 1.0f;
                ui_xform.color.a = 0.9f;
                ui_xform.orientation = m_orientation;
                ui_xform.no_center = true;

                // add UI elements
                add_container_primitives(list, root_xform, ui_xform, debug, rendertypes_global.BLENDMODE_ALPHA);
            }

            // process the UI if we are the UI target
            if (is_ui_target())
            {
                // compute the transform for the UI
                object_transform ui_xform = new object_transform();
                ui_xform.xoffs = 0;
                ui_xform.yoffs = 0;
                ui_xform.xscale = (float) m_width;
                ui_xform.yscale = (float) m_height;
                ui_xform.color = new render_color();
                ui_xform.color.r = ui_xform.color.g = ui_xform.color.b = ui_xform.color.a = 1.0f;
                ui_xform.orientation = m_orientation;
                ui_xform.no_center = false;

                // add UI elements
                add_container_primitives(list, root_xform, ui_xform, m_manager.ui_container(), rendertypes_global.BLENDMODE_ALPHA);
            }

            // optimize the list before handing it off
            add_clear_and_optimize_primitive_list(list);
            list.release_lock();
            return list;
        }


        // hit testing
        //-------------------------------------------------
        //  map_point_container - attempts to map a point
        //  on the specified render_target to the
        //  specified container, if possible
        //-------------------------------------------------
        public bool map_point_container(int target_x, int target_y, render_container container, out float container_x, out float container_y)
        {
            std.pair<float, float> target_f = map_point_internal(target_x, target_y);

            // explicitly check for the UI container
            if (container == m_manager.ui_container())
            {
                // this hit test went against the UI container
                if ((target_f.first >= 0.0f) && (target_f.first < 1.0f) && (target_f.second >= 0.0f) && (target_f.second < 1.0f))
                {
                    // this point was successfully mapped
                    container_x = (float)target_x / m_width;
                    container_y = (float)target_y / m_height;
                    return true;
                }
            }
            else
            {
                if ((m_orientation & ORIENTATION_FLIP_X) != 0)
                    target_f = new std.pair<float, float>(1.0f - target_f.first, target_f.second);  //target_f.first = 1.0f - target_f.first;
                if ((m_orientation & ORIENTATION_FLIP_Y) != 0)
                    target_f = new std.pair<float, float>(target_f.first, 1.0f - target_f.second);  //target_f.second = 1.0f - target_f.second;
                if ((m_orientation & ORIENTATION_SWAP_XY) != 0)
                    target_f = new std.pair<float, float>(target_f.second, target_f.first);  //std::swap(target_f.first, target_f.second);

                // try to find the right container
                var items = current_view().visible_screen_items();
                var found = std.find_if(
                           items,
                           (layout_view.item item) => { return item.screen().container() == container; });
                if (default != found)
                {
                    layout_view.item item = found;
                    render_bounds bounds = item.bounds();
                    if (bounds.includes(target_f.first, target_f.second))
                    {
                        // point successfully mapped
                        container_x = (target_f.first - bounds.x0) / bounds.width();
                        container_y = (target_f.second - bounds.y0) / bounds.height();
                        return true;
                    }
                }
            }

            // default to point not mapped
            container_x = container_y = -1.0f;
            return false;
        }


        //-------------------------------------------------
        //  map_point_input - attempts to map a point on
        //  the specified render_target to an input port
        //  field, if possible
        //-------------------------------------------------
        public bool map_point_input(int target_x, int target_y, out ioport_port input_port, out ioport_value input_mask, out float input_x, out float input_y)
        {
            std.pair<float, float> target_f = map_point_internal(target_x, target_y);
            if ((m_orientation & ORIENTATION_FLIP_X) != 0)
                target_f = new std.pair<float, float>(1.0f - target_f.first, target_f.second);  //target_f.first = 1.0f - target_f.first;
            if ((m_orientation & ORIENTATION_FLIP_Y) != 0)
                target_f = new std.pair<float, float>(target_f.first, 1.0f - target_f.second);  //target_f.second = 1.0f - target_f.second;
            if ((m_orientation & ORIENTATION_SWAP_XY) != 0)
                target_f = new std.pair<float, float>(target_f.second, target_f.first);  //std::swap(target_f.first, target_f.second);

            var items = current_view().interactive_items();
            m_hit_test.resize(items.size() * 2);
            std.fill(m_hit_test, false);

            foreach (var edge in current_view().interactive_edges_x())
            {
                if ((edge.position() > target_f.first) || ((edge.position() == target_f.first) && edge.trailing()))
                    break;
                else
                    m_hit_test[edge.index()] = !edge.trailing();
            }

            foreach (var edge in current_view().interactive_edges_y())
            {
                if ((edge.position() > target_f.second) || ((edge.position() == target_f.second) && edge.trailing()))
                    break;
                else
                    m_hit_test[items.size() + (int)edge.index()] = !edge.trailing();
            }

            for (unsigned i = 0; items.size() > i; ++i)
            {
                if (m_hit_test[i] && m_hit_test[items.size() + (int)i])
                {
                    layout_view.item item = items[i];
                    render_bounds bounds = item.bounds();
                    if (bounds.includes(target_f.first, target_f.second))
                    {
                        if (item.has_input())
                        {
                            // point successfully mapped
                            //std::tie(input_port, input_mask) = item.input_tag_and_mask();
                            var ret = item.input_tag_and_mask();
                            input_port = ret.first;
                            input_mask = ret.second; 

                            input_x = (target_f.first - bounds.x0) / bounds.width();
                            input_y = (target_f.second - bounds.y0) / bounds.height();
                            return true;
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            // default to point not mapped
            input_port = null;
            input_mask = 0;
            input_x = input_y = -1.0f;
            return false;
        }


        // reference tracking
        //-------------------------------------------------
        //  invalidate_all - if any of our primitive lists
        //  contain a reference to the given pointer,
        //  clear them
        //-------------------------------------------------
        public void invalidate_all(object refptr)  /* possibly only bitmap_t refs */  // void *refptr)
        {
            // iterate through all our primitive lists
            for (int listnum = 0; listnum < m_primlist.Length; listnum++)
            {
                render_primitive_list list = m_primlist[listnum];

                // if we have a reference to this object, release our list
                list.acquire_lock();
                if (list.has_reference(refptr))
                    list.release_all();
                list.release_lock();
            }
        }


        // resolve tag lookups
        //-------------------------------------------------
        //  resolve_tags - resolve tag lookups
        //-------------------------------------------------
        public void resolve_tags()
        {
            foreach (layout_file file in m_filelist)
                file.resolve_tags();

            current_view().recompute(visibility_mask(), m_layerconfig.zoom_to_screen());
            current_view().preload();
        }


        //using view_mask_pair = std::pair<std::reference_wrapper<layout_view>, u32>;
        //using view_mask_vector = std::vector<view_mask_pair>;


        // debug containers
        //render_container *debug_alloc();
        //void debug_free(render_container &container);
        //void debug_append(render_container &container);


        // private classes declared in render.cpp
        //struct object_transform;


        // internal helpers

        //-------------------------------------------------
        //  update_layer_config - recompute after a layer
        //  config change
        //-------------------------------------------------
        void update_layer_config()
        {
            current_view().recompute(visibility_mask(), m_layerconfig.zoom_to_screen());
        }


        //-------------------------------------------------
        //  load_layout_files - load layout files for a
        //  given render target
        //-------------------------------------------------
        void load_layout_files(internal_layout layoutfile, bool singlefile)
        {
            bool have_artwork  = false;

            // if there's an explicit file, load that first
            string basename = m_manager.machine().basename();
            if (layoutfile != null)
                have_artwork |= load_layout_file(basename.c_str(), layoutfile);

            // if we're only loading this file, we know our final result
            if (!singlefile)
                load_additional_layout_files(basename.c_str(), have_artwork);
        }

        void load_layout_files(util.xml.data_node rootnode, bool singlefile)
        {
            bool have_artwork  = false;

            // if there's an explicit file, load that first
            string basename = m_manager.machine().basename();
            have_artwork |= load_layout_file(m_manager.machine().root_device(), rootnode, m_manager.machine().options().art_path(), basename.c_str());

            // if we're only loading this file, we know our final result
            if (!singlefile)
                load_additional_layout_files(basename.c_str(), have_artwork);
        }


        // local screen info to avoid repeated code
        class load_additional_layout_files_screen_info
        {
            screen_device m_device;  //std::reference_wrapper<screen_device const> m_device;
            bool m_rotated;
            std.pair<unsigned, unsigned> m_physical;
            std.pair<unsigned, unsigned> m_native;


            public load_additional_layout_files_screen_info(screen_device screen)
            {
                m_device = screen;
                m_rotated = (screen.orientation() & emucore_global.ORIENTATION_SWAP_XY) != 0;
                m_physical = screen.physical_aspect();
                m_native = new std.pair<unsigned, unsigned>((unsigned)screen.visible_area().width(), (unsigned)screen.visible_area().height());


                UInt32 tempFirst = m_native.first;
                UInt32 tempSecond = m_native.second;
                reduce_fraction(ref tempFirst, ref tempSecond);
                m_native = new std.pair<unsigned, unsigned>(tempFirst, tempSecond);

                if (m_rotated)
                {
                    m_physical = new std.pair<unsigned, unsigned>(m_physical.second, m_physical.first);  // std.swap(m_physical.first, m_physical.second);
                    m_native = new std.pair<unsigned, unsigned>(m_native.second, m_native.first);  // std.swap(m_native.first, m_native.second);
                }
            }


            //screen_device const &device() const { return m_device.get(); }
            //bool rotated() const { return m_rotated; }
            public bool square() { return m_physical.first == m_native.first && m_physical.second == m_native.second; }
            public unsigned physical_x() { return m_physical.first; }
            public unsigned physical_y() { return m_physical.second; }
            public unsigned native_x() { return m_native.first; }
            public unsigned native_y() { return m_native.second; }

            //std::pair<float, float> tiled_size() const
            //{
            //    if (physical_x() == physical_y())
            //        return std::make_pair(1.0F, 1.0F);
            //    else if (physical_x() > physical_y())
            //        return std::make_pair(1.0F, float(physical_y()) / physical_x());
            //    else
            //        return std::make_pair(float(physical_x()) / physical_y(), 1.0F);
            //}
        }


        void load_additional_layout_files(string basename, bool have_artwork)
        {
            m_external_artwork = false;

            // if override_artwork defined, load that and skip artwork other than default
            string override_art = m_manager.machine().options().override_artwork();
            if (override_art != null)
            {
                if (load_layout_file(override_art, override_art))
                    m_external_artwork = true;
                else if (load_layout_file(override_art, "default"))
                    m_external_artwork = true;
            }

            game_driver system = m_manager.machine().system();

            // Skip if override_artwork has found artwork
            if (!m_external_artwork)
            {
                // try to load a file based on the driver name
                if (!load_layout_file(basename, system.name))
                    m_external_artwork |= load_layout_file(basename, "default");
                else
                    m_external_artwork = true;

                // if a default view has been specified, use that as a fallback
                bool have_default = false;
                if (system.default_layout != null)
                    have_default |= load_layout_file(null, system.default_layout);

                m_manager.machine().config().apply_default_layouts(
                        /*[this, have_default]*/ (device_t dev, internal_layout layout) =>
                        { have_default |= load_layout_file(null, layout, dev); });

                // try to load another file based on the parent driver name
                int cloneof = driver_list.clone(system);
                while (0 <= cloneof)
                {
                    if (!m_external_artwork || ((UInt64)driver_list.driver(cloneof).flags & MACHINE_IS_BIOS_ROOT) == 0)
                    {
                        if (!load_layout_file(driver_list.driver(cloneof).name, driver_list.driver(cloneof).name))
                            m_external_artwork |= load_layout_file(driver_list.driver(cloneof).name, "default");
                        else
                            m_external_artwork = true;
                    }

                    // Check the parent of the parent to cover bios based artwork
                    game_driver parent = driver_list.driver(cloneof);
                    cloneof = driver_list.clone(parent);
                }

                have_artwork |= m_external_artwork;

                // Use fallback artwork if defined and no artwork has been found yet
                if (!have_artwork)
                {
                    string fallback_art = m_manager.machine().options().fallback_artwork();
                    if (!string.IsNullOrEmpty(fallback_art))  //if (fallback_art && *fallback_art)
                    {
                        if (!load_layout_file(fallback_art, fallback_art))
                            have_artwork |= load_layout_file(fallback_art, "default");
                        else
                            have_artwork = true;
                    }
                }
            }

            screen_device_enumerator iter = new screen_device_enumerator(m_manager.machine().root_device());
            std.vector<load_additional_layout_files_screen_info> screens = new std.vector<load_additional_layout_files_screen_info>();  //std::vector<screen_info> const screens(std::begin(iter), std::end(iter));
            foreach (var screen in iter)
                screens.push_back(new load_additional_layout_files_screen_info(screen));

            // need this because views aren't fully set up yet
            //auto const nth_view =
            //    [this] (unsigned n) -> layout_view *
            Func<unsigned, layout_view> nth_view = (n) =>
                {
                    foreach (layout_file file in m_filelist)
                        foreach (layout_view view in file.views())
                            if ((m_flags & render_global.RENDER_CREATE_NO_ART) == 0 || !view.has_art())
                                if (n-- == 0)
                                    return view;
                    return null;
                };

            if (screens.empty()) // ensure the fallback view for systems with no screens is loaded if necessary
            {
                if (nth_view(0) == null)
                {
                    load_layout_file(null, noscreens_global.layout_noscreens);
                    if (m_filelist.empty())
                        throw new emu_fatalerror("Couldn't parse default layout??");
                }
            }
            else // generate default layouts for larger numbers of screens
            {
                util.xml.file root = util.xml.file.create();
                if (root == null)
                    throw new emu_fatalerror("Couldn't create XML document??");

                util.xml.data_node layoutnode = root.add_child("mamelayout", null);
                if (layoutnode == null)
                    throw new emu_fatalerror("Couldn't create XML node??");

                layoutnode.set_attribute_int("version", 2);

                // generate individual physical aspect views
                for (unsigned i = 0; screens.size() > i; ++i)
                {
                    util.xml.data_node viewnode = layoutnode.add_child("view", null);
                    if (viewnode == null)
                        throw new emu_fatalerror("Couldn't create XML node??");

                    viewnode.set_attribute(
                            "name",
                            string_format(
                                "Screen {0} Standard ({1}:{2})",
                                i, screens[i].physical_x(), screens[i].physical_y()).c_str());
                    util.xml.data_node screennode = viewnode.add_child("screen", null);
                    if (screennode == null)
                        throw new emu_fatalerror("Couldn't create XML node??");

                    screennode.set_attribute_int("index", (int)i);

                    util.xml.data_node boundsnode = screennode.add_child("bounds", null);
                    if (boundsnode == null)
                        throw new emu_fatalerror("Couldn't create XML node??");

                    boundsnode.set_attribute_int("x", 0);
                    boundsnode.set_attribute_int("y", 0);
                    boundsnode.set_attribute_int("width", (int)screens[(int)i].physical_x());
                    boundsnode.set_attribute_int("height", (int)screens[(int)i].physical_y());
                }

                // generate individual pixel aspect views
                for (unsigned i = 0; screens.size() > i; ++i)
                {
                    if (!screens[(int)i].square())
                    {
                        util.xml.data_node viewnode = layoutnode.add_child("view", null);
                        if (viewnode == null)
                            throw new emu_fatalerror("Couldn't create XML node??");

                        viewnode.set_attribute(
                                "name",
                                string_format(
                                    "Screen {0} Pixel Aspect ({1}:{2})",
                                    i, screens[i].native_x(), screens[i].native_y()).c_str());

                        util.xml.data_node screennode = viewnode.add_child("screen", null);
                        if (screennode == null)
                            throw new emu_fatalerror("Couldn't create XML node??");

                        screennode.set_attribute_int("index", (int)i);
                        util.xml.data_node boundsnode = screennode.add_child("bounds", null);
                        if (boundsnode == null)
                            throw new emu_fatalerror("Couldn't create XML node??");

                        boundsnode.set_attribute_int("x", 0);
                        boundsnode.set_attribute_int("y", 0);
                        boundsnode.set_attribute_int("width", (int)screens[(int)i].native_x());
                        boundsnode.set_attribute_int("height", (int)screens[(int)i].native_y());
                    }
                }

                // generate the fake cocktail view for single-screen systems
                if (screens.size() == 1U)
                {
                    util.xml.data_node viewnode = layoutnode.add_child("view", null);
                    if (viewnode == null)
                        throw new emu_fatalerror("Couldn't create XML node??");

                    viewnode.set_attribute("name", "Cocktail");

                    util.xml.data_node mirrornode = viewnode.add_child("screen", null);
                    if (mirrornode == null)
                        throw new emu_fatalerror("Couldn't create XML node??");

                    mirrornode.set_attribute_int("index", 0);

                    util.xml.data_node mirrorbounds = mirrornode.add_child("bounds", null);
                    if (mirrorbounds == null)
                        throw new emu_fatalerror("Couldn't create XML node??");

                    mirrorbounds.set_attribute_int("x", 0);
                    mirrorbounds.set_attribute_float("y", (-0.01f * Math.Min(screens[0].physical_x(), screens[0].physical_y())) - screens[0].physical_y());
                    mirrorbounds.set_attribute_int("width", (int)screens[0].physical_x());
                    mirrorbounds.set_attribute_int("height", (int)screens[0].physical_y());

                    util.xml.data_node flipper = mirrornode.add_child("orientation", null);
                    if (flipper == null)
                        throw new emu_fatalerror("Couldn't create XML node??");

                    flipper.set_attribute_int("rotate", 180);

                    util.xml.data_node screennode = viewnode.add_child("screen", null);
                    if (screennode == null)
                        throw new emu_fatalerror("Couldn't create XML node??");

                    screennode.set_attribute_int("index", 0);

                    util.xml.data_node screenbounds = screennode.add_child("bounds", null);
                    if (screenbounds == null)
                        throw new emu_fatalerror("Couldn't create XML node??");

                    screenbounds.set_attribute_int("x", 0);
                    screenbounds.set_attribute_int("y", 0);
                    screenbounds.set_attribute_int("width", (int)screens[0].physical_x());
                    screenbounds.set_attribute_int("height", (int)screens[0].physical_y());
                }

                // generate tiled views if the supplied artwork doesn't provide a view of all screens
                bool need_tiles = screens.size() >= 3;
                if (!need_tiles && (screens.size() >= 2))
                {
                    need_tiles = true;
                    int viewindex = 0;
                    for (layout_view view = nth_view((unsigned)viewindex); need_tiles && view != null; view = nth_view((unsigned)(++viewindex)))
                    {
                        bool screen_missing = false;
                        foreach (screen_device screen in iter)
                        {
                            if (!view.has_screen(screen))
                            {
                                screen_missing = true;
                                break;
                            }
                        }

                        if (!screen_missing)
                            need_tiles = false;
                    }
                }

                if (need_tiles)
                {
                    throw new emu_unimplemented();
                }

                // try to parse it
                if (!load_layout_file(m_manager.machine().root_device(), root, m_manager.machine().options().art_path(), null))
                    throw new emu_fatalerror("Couldn't parse generated layout??");
            }
        }


        //-------------------------------------------------
        //  load_layout_file - load a single layout file
        //  and append it to our list
        //-------------------------------------------------
        bool load_layout_file(string dirname, internal_layout layout_data, device_t device = null)
        {
            throw new emu_unimplemented();
        }


        bool load_layout_file(string dirname, string filename)
        {
            // build the path and optionally prepend the directory
            string fname = "";
            if (dirname != null)
                fname = fname.append_(dirname).append_(PATH_SEPARATOR);
            fname = fname.append_(filename).append_(".lay");

            // attempt to open matching files
            util.xml.parse_options parseopt = new util.xml.parse_options();
            util.xml.parse_error parseerr = new util.xml.parse_error();
            parseopt.error = parseerr;
            emu_file layoutfile = new emu_file(m_manager.machine().options().art_path(), OPEN_FLAG_READ);
            layoutfile.set_restrict_to_mediapath(1);
            bool result = false;
            for (osd_file.error filerr = layoutfile.open(fname); osd_file.error.NONE == filerr; filerr = layoutfile.open_next())
            {
                // read the file and parse as XML
                util.xml.file rootnode = util.xml.file.read(layoutfile.core_file_get(), parseopt);
                if (rootnode != null)
                {
                    // extract directory name from location of layout file
                    string artdir = layoutfile.fullpath();
                    var dirsep = artdir.LastIndexOf(PATH_SEPARATOR[0]);  //auto const dirsep(std::find_if(artdir.rbegin(), artdir.rend(), &util::is_directory_separator));
                    artdir = artdir.Substring(0, dirsep);  //artdir.erase(dirsep.base(), artdir.end());

                    // record a warning if we didn't get a properly-formatted XML file
                    if (!load_layout_file(m_manager.machine().root_device(), rootnode, null, artdir.c_str()))
                        osd_printf_warning("Improperly formatted XML layout file '{0}', ignoring\n", filename);
                    else
                        result = true;
                }
                else if (parseerr.error_message != null)
                {
                    osd_printf_warning(
                            "Error parsing XML layout file '{0}' at line {1} column {2}: {3}, ignoring\n",
                            filename,
                            parseerr.error_line,
                            parseerr.error_column,
                            parseerr.error_message);
                }
                else
                {
                    osd_printf_warning("Error parsing XML layout file '{0}', ignorning\n", filename);
                }
            }

            return result;
        }


        bool load_layout_file(device_t device, util.xml.data_node rootnode, string searchpath, string dirname)
        {
            // parse and catch any errors
            try
            {
                m_filelist.emplace_back(new layout_file(device, rootnode, searchpath, dirname));
            }
            catch (emu_fatalerror err)
            {
                osd_printf_warning("{0}\n", err.what());
                return false;
            }

            return true;
        }


        //-------------------------------------------------
        //  add_container_primitives - add primitives
        //  based on the container
        //-------------------------------------------------
        void add_container_primitives(render_primitive_list list, object_transform root_xform, object_transform xform, render_container container, int blendmode)
        {
            // first update the palette for the container, if it is dirty
            container.update_palette();

            // compute the clip rect
            render_bounds cliprect = new render_bounds();
            cliprect.x0 = xform.xoffs;
            cliprect.y0 = xform.yoffs;
            cliprect.x1 = xform.xoffs + xform.xscale;
            cliprect.y1 = xform.yoffs + xform.yscale;
            cliprect &= m_bounds;

            float root_xoffs = root_xform.xoffs + Math.Abs(root_xform.xscale - xform.xscale) * 0.5f;
            float root_yoffs = root_xform.yoffs + Math.Abs(root_xform.yscale - xform.yscale) * 0.5f;

            render_bounds root_cliprect = new render_bounds();
            root_cliprect.x0 = root_xoffs;
            root_cliprect.y0 = root_yoffs;
            root_cliprect.x1 = root_xoffs + root_xform.xscale;
            root_cliprect.y1 = root_yoffs + root_xform.yscale;
            root_cliprect &= m_bounds;

            // compute the container transform
            object_transform container_xform = new object_transform();
            container_xform.orientation = rendutil_global.orientation_add(container.orientation(), xform.orientation);
            {
                float xscale = (container_xform.orientation & emucore_global.ORIENTATION_SWAP_XY) != 0 ? container.yscale() : container.xscale();
                float yscale = (container_xform.orientation & emucore_global.ORIENTATION_SWAP_XY) != 0 ? container.xscale() : container.yscale();
                float xoffs = (container_xform.orientation & emucore_global.ORIENTATION_SWAP_XY) != 0 ? container.yoffset() : container.xoffset();
                float yoffs = (container_xform.orientation & emucore_global.ORIENTATION_SWAP_XY) != 0 ? container.xoffset() : container.yoffset();
                if ((container_xform.orientation & emucore_global.ORIENTATION_FLIP_X) != 0) xoffs = -xoffs;
                if ((container_xform.orientation & emucore_global.ORIENTATION_FLIP_Y) != 0) yoffs = -yoffs;
                if (!m_transform_container)
                {
                    xscale = 1.0f;
                    yscale = 1.0f;
                    xoffs = 0.0f;
                    yoffs = 0.0f;
                }
                container_xform.xscale = xform.xscale * xscale;
                container_xform.yscale = xform.yscale * yscale;
                if (xform.no_center)
                {
                    container_xform.xoffs = xform.xscale * (xoffs) + xform.xoffs;
                    container_xform.yoffs = xform.yscale * (yoffs) + xform.yoffs;
                }
                else
                {
                    container_xform.xoffs = xform.xscale * (0.5f - 0.5f * xscale + xoffs) + xform.xoffs;
                    container_xform.yoffs = xform.yscale * (0.5f - 0.5f * yscale + yoffs) + xform.yoffs;
                }
                container_xform.color = xform.color;
            }

            // iterate over elements
            foreach (render_container.item curitem in container.items())
            {
                // compute the oriented bounds
                render_bounds bounds = curitem.bounds();
                render_global.apply_orientation(bounds, container_xform.orientation);

                float xscale = container_xform.xscale;
                float yscale = container_xform.yscale;
                float xoffs = container_xform.xoffs;
                float yoffs = container_xform.yoffs;
                if (!m_transform_container && render_global.PRIMFLAG_GET_VECTOR(curitem.flags()))
                {
                    xoffs = root_xoffs;
                    yoffs = root_yoffs;
                }

                // allocate the primitive and set the transformed bounds/color data
                render_primitive prim = list.alloc(render_primitive.primitive_type.INVALID);

                prim.container = container; /* pass the container along for access to user_settings */

                prim.bounds.x0 = rendutil_global.render_round_nearest(xoffs + bounds.x0 * xscale);
                prim.bounds.y0 = rendutil_global.render_round_nearest(yoffs + bounds.y0 * yscale);
                if ((curitem.internal_flags() & render_global.INTERNAL_FLAG_CHAR) != 0)
                {
                    prim.bounds.x1 = prim.bounds.x0 + rendutil_global.render_round_nearest((bounds.x1 - bounds.x0) * xscale);
                    prim.bounds.y1 = prim.bounds.y0 + rendutil_global.render_round_nearest((bounds.y1 - bounds.y0) * yscale);
                }
                else
                {
                    prim.bounds.x1 = rendutil_global.render_round_nearest(xoffs + bounds.x1 * xscale);
                    prim.bounds.y1 = rendutil_global.render_round_nearest(yoffs + bounds.y1 * yscale);
                }

                // compute the color of the primitive
                prim.color.r = container_xform.color.r * curitem.color().r;
                prim.color.g = container_xform.color.g * curitem.color().g;
                prim.color.b = container_xform.color.b * curitem.color().b;
                prim.color.a = container_xform.color.a * curitem.color().a;

                // copy unclipped bounds
                prim.full_bounds = prim.bounds;

                // now switch off the type
                bool clipped = true;
                switch (curitem.type())
                {
                    case render_global.CONTAINER_ITEM_LINE:
                        // adjust the color for brightness/contrast/gamma
                        prim.color.a = container.apply_brightness_contrast_gamma_fp(prim.color.a);
                        prim.color.r = container.apply_brightness_contrast_gamma_fp(prim.color.r);
                        prim.color.g = container.apply_brightness_contrast_gamma_fp(prim.color.g);
                        prim.color.b = container.apply_brightness_contrast_gamma_fp(prim.color.b);

                        // set the line type
                        prim.type = render_primitive.primitive_type.LINE;
                        prim.flags |= render_global.PRIMFLAG_TYPE_LINE;

                        // scale the width by the minimum of X/Y scale factors
                        prim.width = curitem.width() * Math.Min(container_xform.xscale, container_xform.yscale);
                        prim.flags |= curitem.flags();

                        // clip the primitive
                        if (!m_transform_container && render_global.PRIMFLAG_GET_VECTOR(curitem.flags()))
                        {
                            clipped = rendutil_global.render_clip_line(prim.bounds, root_cliprect);
                        }
                        else
                        {
                            clipped = rendutil_global.render_clip_line(prim.bounds, cliprect);
                        }
                        break;

                    case render_global.CONTAINER_ITEM_QUAD:
                        // set the quad type
                        prim.type = render_primitive.primitive_type.QUAD;
                        prim.flags |= render_global.PRIMFLAG_TYPE_QUAD;

                        // normalize the bounds
                        render_global.normalize_bounds(prim.bounds);

                        // get the scaled bitmap and set the resulting palette
                        if (curitem.texture() != null)
                        {
                            // determine the final orientation
                            int finalorient = rendutil_global.orientation_add((int)render_global.PRIMFLAG_GET_TEXORIENT(curitem.flags()), container_xform.orientation);

                            // based on the swap values, get the scaled final texture
                            int width = (finalorient & emucore_global.ORIENTATION_SWAP_XY) != 0 ? (int)(prim.bounds.y1 - prim.bounds.y0) : (int)(prim.bounds.x1 - prim.bounds.x0);
                            int height = (finalorient & emucore_global.ORIENTATION_SWAP_XY) != 0 ? (int)(prim.bounds.x1 - prim.bounds.x0) : (int)(prim.bounds.y1 - prim.bounds.y0);
                            width = Math.Min(width, m_maxtexwidth);
                            height = Math.Min(height, m_maxtexheight);

                            curitem.texture().get_scaled((UInt32)width, (UInt32)height, prim.texture, list, curitem.flags());

                            // set the palette
                            prim.texture.palette = curitem.texture().get_adjusted_palette(container, ref prim.texture.palette_length);

                            // determine UV coordinates and apply clipping
                            prim.texcoords = new render_quad_texuv(render_global.oriented_texcoords[finalorient]);

                            // apply clipping
                            clipped = rendutil_global.render_clip_quad(prim.bounds, cliprect, prim.texcoords);

                            // apply the final orientation from the quad flags and then build up the final flags
                            prim.flags |= (curitem.flags() & ~(render_global.PRIMFLAG_TEXORIENT_MASK | render_global.PRIMFLAG_BLENDMODE_MASK | render_global.PRIMFLAG_TEXFORMAT_MASK))
                                | render_global.PRIMFLAG_TEXORIENT((UInt32)finalorient)
                                | render_global.PRIMFLAG_TEXFORMAT((UInt32)curitem.texture().format());
                            prim.flags |= blendmode != -1
                                ? PRIMFLAG_BLENDMODE((UInt32)blendmode)
                                : PRIMFLAG_BLENDMODE(render_global.PRIMFLAG_GET_BLENDMODE(curitem.flags()));
                        }
                        else
                        {
                            // adjust the color for brightness/contrast/gamma
                            prim.color.r = container.apply_brightness_contrast_gamma_fp(prim.color.r);
                            prim.color.g = container.apply_brightness_contrast_gamma_fp(prim.color.g);
                            prim.color.b = container.apply_brightness_contrast_gamma_fp(prim.color.b);

                            // no texture
                            prim.texture.base_ = null;

                            if (render_global.PRIMFLAG_GET_VECTORBUF(curitem.flags()))
                            {
                                // flags X(1) flip-x, Y(2) flip-y, S(4) swap-xy
                                //
                                // X  Y  S   e.g.       flips
                                // 0  0  0   asteroid   !X !Y
                                // 0  0  1   -           X  Y
                                // 0  1  0   speedfrk   !X  Y
                                // 0  1  1   tempest    !X  Y
                                // 1  0  0   -           X !Y
                                // 1  0  1   -           x !Y
                                // 1  1  0   solarq      X  Y
                                // 1  1  1   barrier    !X !Y

                                bool flip_x = ((UInt32)m_manager.machine().system().flags & emucore_global.ORIENTATION_FLIP_X) == emucore_global.ORIENTATION_FLIP_X;
                                bool flip_y = ((UInt32)m_manager.machine().system().flags & emucore_global.ORIENTATION_FLIP_Y) == emucore_global.ORIENTATION_FLIP_Y;
                                bool swap_xy = ((UInt32)m_manager.machine().system().flags & emucore_global.ORIENTATION_SWAP_XY) == emucore_global.ORIENTATION_SWAP_XY;

                                int vectororient = 0;
                                if (flip_x)
                                {
                                    vectororient |= (int)emucore_global.ORIENTATION_FLIP_X;
                                }
                                if (flip_y)
                                {
                                    vectororient |= (int)emucore_global.ORIENTATION_FLIP_Y;
                                }
                                if ((flip_x && flip_y && swap_xy) || (!flip_x && !flip_y && swap_xy))
                                {
                                    vectororient ^= (int)emucore_global.ORIENTATION_FLIP_X;
                                    vectororient ^= (int)emucore_global.ORIENTATION_FLIP_Y;
                                }

                                // determine the final orientation (textures are up-side down, so flip axis for vectors to immitate that behavior)
                                int finalorient = rendutil_global.orientation_add(vectororient, container_xform.orientation);

                                // determine UV coordinates
                                prim.texcoords = new render_quad_texuv(render_global.oriented_texcoords[finalorient]);

                                // apply clipping
                                clipped = rendutil_global.render_clip_quad(prim.bounds, cliprect, prim.texcoords);

                                // apply the final orientation from the quad flags and then build up the final flags
                                prim.flags |= (curitem.flags() & ~(render_global.PRIMFLAG_TEXORIENT_MASK | render_global.PRIMFLAG_BLENDMODE_MASK | render_global.PRIMFLAG_TEXFORMAT_MASK))
                                    | render_global.PRIMFLAG_TEXORIENT((UInt32)finalorient);
                                prim.flags |= blendmode != -1
                                    ? PRIMFLAG_BLENDMODE((UInt32)blendmode)
                                    : PRIMFLAG_BLENDMODE(render_global.PRIMFLAG_GET_BLENDMODE(curitem.flags()));
                            }
                            else
                            {
                                // set the basic flags
                                prim.flags |= (curitem.flags() & ~render_global.PRIMFLAG_BLENDMODE_MASK)
                                    | PRIMFLAG_BLENDMODE(rendertypes_global.BLENDMODE_ALPHA);

                                // apply clipping
                                clipped = rendutil_global.render_clip_quad(prim.bounds, cliprect, null);
                            }
                        }
                        break;
                }

                // add to the list or free if we're clipped out
                list.append_or_return(prim, clipped);
            }

            // add the overlay if it exists
            if (container.overlay() != null && m_layerconfig.screen_overlay_enabled())
            {
                int width;
                int height;

                // allocate a primitive
                render_primitive prim = list.alloc(render_primitive.primitive_type.QUAD);
                prim.bounds.set_wh(xform.xoffs, xform.yoffs, xform.xscale, xform.yscale);
                prim.full_bounds = prim.bounds;
                prim.color = container_xform.color;
                width = (int)(rendutil_global.render_round_nearest(prim.bounds.x1) - rendutil_global.render_round_nearest(prim.bounds.x0));
                height = (int)(rendutil_global.render_round_nearest(prim.bounds.y1) - rendutil_global.render_round_nearest(prim.bounds.y0));

                container.overlay().get_scaled(
                    (container_xform.orientation & emucore_global.ORIENTATION_SWAP_XY) != 0 ? (UInt32)height : (UInt32)width,
                    (container_xform.orientation & emucore_global.ORIENTATION_SWAP_XY) != 0 ? (UInt32)width : (UInt32)height, prim.texture, list);

                // determine UV coordinates
                prim.texcoords = new render_quad_texuv(render_global.oriented_texcoords[container_xform.orientation]);

                // set the flags and add it to the list
                prim.flags = render_global.PRIMFLAG_TEXORIENT((UInt32)container_xform.orientation)
                    | render_global.PRIMFLAG_BLENDMODE(rendertypes_global.BLENDMODE_RGB_MULTIPLY)
                    | render_global.PRIMFLAG_TEXFORMAT((UInt32)container.overlay().format())
                    | render_global.PRIMFLAG_TEXSHADE(1);

                list.append_or_return(prim, false);
            }
        }

        //-------------------------------------------------
        //  add_element_primitives - add the primitive
        //  for an element in the current state
        //-------------------------------------------------
        void add_element_primitives(render_primitive_list list, object_transform xform, layout_element element, int state, int blendmode)
        {
            // limit state range to non-negative values
            if (state < 0)
                state = 0;

            // get a pointer to the relevant texture
            render_texture texture = element.state_texture(state);
            if (texture != null)
            {
                render_primitive prim = list.alloc(render_primitive.primitive_type.QUAD);

                // configure the basics
                prim.color = xform.color;
                prim.flags = render_global.PRIMFLAG_TEXORIENT((UInt32)xform.orientation) | PRIMFLAG_BLENDMODE((UInt32)blendmode) | render_global.PRIMFLAG_TEXFORMAT((UInt32)texture.format());

                // compute the bounds
                int width = (int)rendutil_global.render_round_nearest(xform.xscale);
                int height = (int)rendutil_global.render_round_nearest(xform.yscale);
                prim.bounds.set_wh(rendutil_global.render_round_nearest(xform.xoffs), rendutil_global.render_round_nearest(xform.yoffs), (float)width, (float)height);
                prim.full_bounds = prim.bounds;
                if ((xform.orientation & emucore_global.ORIENTATION_SWAP_XY) != 0)
                    std.swap(ref width, ref height);
                width = std.min(width, m_maxtexwidth);
                height = std.min(height, m_maxtexheight);

                // get the scaled texture and append it
                texture.get_scaled((UInt32)width, (UInt32)height, prim.texture, list, prim.flags);

                // compute the clip rect
                render_bounds cliprect = prim.bounds & m_bounds;

                // determine UV coordinates and apply clipping
                prim.texcoords = new render_quad_texuv(render_global.oriented_texcoords[xform.orientation]);
                bool clipped = rendutil_global.render_clip_quad(prim.bounds, cliprect, prim.texcoords);

                // add to the list or free if we're clipped out
                list.append_or_return(prim, clipped);
            }
        }


        //-------------------------------------------------
        //  map_point_internal - internal logic for
        //  mapping points
        //-------------------------------------------------
        std.pair<float, float> map_point_internal(s32 target_x, s32 target_y)
        {
            // compute the visible width/height
            s32 viswidth;
            s32 visheight;
            compute_visible_area(m_width, m_height, m_pixel_aspect, m_orientation, out viswidth, out visheight);

            // create a root transform for the target
            object_transform root_xform = new object_transform();
            root_xform.xoffs = (float)(m_width - viswidth) / 2;
            root_xform.yoffs = (float)(m_height - visheight) / 2;

            // convert target coordinates to float
            if (!m_manager.machine().ui().is_menu_active())
                return std.make_pair((float)(target_x - root_xform.xoffs) / viswidth, (float)(target_y - root_xform.yoffs) / visheight);
            else
                return std.make_pair((float)(target_x) / m_width, (float)(target_y) / m_height);
        }


        // config callbacks
        //void config_load(xml_data_node &targetnode);
        //bool config_save(xml_data_node &targetnode);


        // view lookups

        //-------------------------------------------------
        //  view_name - return the name of the indexed
        //  view, or NULL if it doesn't exist
        //-------------------------------------------------
        layout_view view_by_index(unsigned index)
        {
            return (m_views.size() > index) ? m_views[index].first : null;
        }

        //-------------------------------------------------
        //  view_index - return the index of the given
        //  view
        //-------------------------------------------------
        int view_index(layout_view targetview)
        {
            // return index of view, or zero if not found
            for (int index = 0; m_views.size() > index; ++index)
            {
                if (m_views[index].first == targetview)
                    return index;
            }

            return 0;
        }


        // optimized clearing

        //-------------------------------------------------
        //  init_clear_extents - reset the extents list
        //-------------------------------------------------
        void init_clear_extents()
        {
            m_clear_extents[0] = -m_height;
            m_clear_extents[1] = 1;
            m_clear_extents[2] = m_width;
            m_clear_extent_count = 3;
        }

        //-------------------------------------------------
        //  remove_clear_extent - remove a quad from the
        //  list of stuff to clear, unless it overlaps
        //  a previous quad
        //-------------------------------------------------
        bool remove_clear_extent(render_bounds bounds)
        {
            int maxIdx = MAX_CLEAR_EXTENTS;  // INT32 *max = &m_clear_extents[MAX_CLEAR_EXTENTS];
            int lastIdx = m_clear_extent_count;  //INT32 *last = &m_clear_extents[m_clear_extent_count];
            int extIdx = 0;  //INT32 *ext = &m_clear_extents[0];
            int boundsx0 = (int)Math.Ceiling(bounds.x0);
            int boundsx1 = (int)Math.Floor(bounds.x1);
            int boundsy0 = (int)Math.Ceiling(bounds.y0);
            int boundsy1 = (int)Math.Floor(bounds.y1);
            int y0;
            int y1 = 0;

            // loop over Y extents
            while (extIdx < lastIdx)
            {
                int linelastIdx = 0;  // INT32 *linelast;

                // first entry of each line should always be negative
                //assert(ext[0] < 0.0f);
                y0 = y1;
                y1 = y0 - m_clear_extents[extIdx] ; // ext[0];

                // do we intersect this extent?
                if (boundsy0 < y1 && boundsy1 > y0)
                {
                    int xextIdx = 0;  // INT32 *xext;
                    int x0;
                    int x1 = 0;

                    // split the top
                    if (y0 < boundsy0)
                    {
                        int diff = boundsy0 - y0;

                        // make a copy of this extent
                        //memmove(&ext[ext[1] + 2], &ext[0], (last - ext) * sizeof(*ext));
                        for (int i = 0; i < (lastIdx - extIdx); i++)
                            m_clear_extents[extIdx + m_clear_extents[extIdx + 1] + 2 + i] = m_clear_extents[extIdx + i];

                        lastIdx += m_clear_extents[extIdx + 1] + 2;
                        if (lastIdx >= maxIdx)  //if (last >= max)
                            throw new emu_fatalerror("render_target::remove_clear_extent: Ran out of clear extents!");

                        // split the extent between pieces
                        m_clear_extents[extIdx + m_clear_extents[extIdx + 1] + 2] = -(-m_clear_extents[extIdx] - diff);  // ext[ext[1] + 2] = -(-ext[0] - diff);
                        m_clear_extents[extIdx] = -diff;  // ext[0] = -diff;

                        // advance to the new extent
                        y0 -= m_clear_extents[extIdx]; // y0 -= ext[0];
                        extIdx += m_clear_extents[extIdx + 1] + 2;  // ext += ext[1] + 2;
                        y1 = y0 - m_clear_extents[extIdx];  // y1 = y0 - ext[0];
                    }

                    // split the bottom
                    if (y1 > boundsy1)
                    {
                        int diff = y1 - boundsy1;

                        // make a copy of this extent
                        //memmove(&ext[ext[1] + 2], &ext[0], (last - ext) * sizeof(*ext));
                        for (int i = 0; i < (lastIdx - extIdx); i++)
                            m_clear_extents[extIdx + m_clear_extents[extIdx + 1] + 2 + i] = m_clear_extents[extIdx + i];

                        lastIdx += m_clear_extents[extIdx + 1] + 2;  // last += ext[1] + 2;
                        if (lastIdx >= maxIdx)  //if (last >= max)
                            throw new emu_fatalerror("render_target::remove_clear_extent: Ran out of clear extents!");

                        // split the extent between pieces
                        m_clear_extents[extIdx + m_clear_extents[extIdx + 1] + 2] = -diff;  // ext[ext[1] + 2] = -diff;
                        m_clear_extents[extIdx] = -(-m_clear_extents[extIdx] - diff);  //ext[0] = -(-ext[0] - diff);

                        // recompute y1
                        y1 = y0 - m_clear_extents[extIdx];  //y1 = y0 - ext[0];
                    }

                    // now remove the X extent
                    linelastIdx = extIdx + m_clear_extents[extIdx + 1] + 2;  // linelast = &ext[ext[1] + 2];
                    xextIdx = extIdx + 2;  // xext = &ext[2];
                    while (xextIdx < linelastIdx)
                    {
                        x0 = x1;
                        x1 = x0 + m_clear_extents[xextIdx];  // x1 = x0 + xext[0];

                        // do we fully intersect this extent?
                        if (boundsx0 >= x0 && boundsx1 <= x1)
                        {
                            // yes; split it
                            //memmove(&xext[2], &xext[0], (last - xext) * sizeof(*xext));
                            for (int i = 0; i < (lastIdx - xextIdx); i++)
                                m_clear_extents[xextIdx + 2 + i] = m_clear_extents[xextIdx + i];

                            lastIdx += 2;
                            linelastIdx += 2;
                            if (lastIdx >= maxIdx)  //if (last >= max)
                                throw new emu_fatalerror("render_target::remove_clear_extent: Ran out of clear extents!");

                            // split this extent into three parts
                            m_clear_extents[xextIdx] = boundsx0 - x0;  // xext[0] = boundsx0 - x0;
                            m_clear_extents[xextIdx + 1] = boundsx1 - boundsx0;
                            m_clear_extents[xextIdx + 2] = x1 - boundsx1;

                            // recompute x1
                            x1 = boundsx1;
                            xextIdx += 2;
                        }

                        // do we partially intersect this extent?
                        else if (boundsx0 < x1 && boundsx1 > x0)
                            goto abort;

                        // advance
                        xextIdx++;

                        // do we partially intersect the next extent (which is a non-clear extent)?
                        if (xextIdx < linelastIdx)
                        {
                            x0 = x1;
                            x1 = x0 + m_clear_extents[xextIdx];  // x1 = x0 + xext[0];
                            if (boundsx0 < x1 && boundsx1 > x0)
                                goto abort;
                            xextIdx++;
                        }
                    }

                    // update the count
                    m_clear_extents[extIdx + 1] = linelastIdx - (extIdx + 2); // ext[1] = linelast - &ext[2];
                }

                // advance to the next row
                extIdx += 2 + m_clear_extents[extIdx + 1];  // extIdx += 2 + ext[1];
            }

            // update the total count
            m_clear_extent_count = lastIdx - 0;  //m_clear_extent_count = last - &m_clear_extents[0];
            return true;

        abort:
            // update the total count even on a failure as we may have split extents
            m_clear_extent_count = lastIdx - 0;  // m_clear_extent_count = last - &m_clear_extents[0];
            return false;
        }

        //-------------------------------------------------
        //  add_clear_extents - add the accumulated
        //  extents as a series of quads to clear
        //-------------------------------------------------
        void add_clear_extents(render_primitive_list list)
        {
            simple_list<render_primitive> clearlist = new simple_list<render_primitive>();
            int lastIdx = m_clear_extent_count;  // int *last = &m_clear_extents[m_clear_extent_count];
            int extIdx = 0;  // int *ext = &m_clear_extents[0];
            int y0;
            int y1 = 0;

            // loop over all extents
            while (extIdx < lastIdx)
            {
                int linelastIdx = extIdx + m_clear_extents[extIdx + 1] + 2;  // int *linelast = &ext[ext[1] + 2];
                int xextIdx = extIdx + 2;  // int *xext = &ext[2];
                int x0;
                int x1 = 0;

                // first entry should always be negative
                //assert(ext[0] < 0);
                y0 = y1;
                y1 = y0 - m_clear_extents[extIdx];  //ext[0];

                // now remove the X extent
                while (xextIdx < linelastIdx)
                {
                    x0 = x1;
                    x1 = x0 + m_clear_extents[xextIdx++];  //*xext++;

                    // only add entries for non-zero widths
                    if (x1 - x0 > 0)
                    {
                        render_primitive prim = list.alloc(render_primitive.primitive_type.QUAD);
                        prim.bounds.set_xy((float)x0, (float)y0, (float)x1, (float)y1);
                        prim.full_bounds = prim.bounds;
                        prim.color.set(1.0f, 0.0f, 0.0f, 0.0f);
                        prim.texture.base_ = null;
                        prim.flags = PRIMFLAG_BLENDMODE(rendertypes_global.BLENDMODE_ALPHA);
                        clearlist.append(prim);
                    }

                    // skip the non-clearing extent
                    x0 = x1;
                    x1 = x0 + m_clear_extents[xextIdx++];  //*xext++;
                }

                // advance to the next part
                extIdx += 2 + m_clear_extents[extIdx + 1]; //ext += 2 + ext[1];
            }

            // we know that the first primitive in the list will be the global clip
            // so we insert the clears immediately after
            list.primlist().prepend_list(clearlist);
        }


        //-------------------------------------------------
        //  add_clear_and_optimize_primitive_list -
        //  optimize the primitive list
        //-------------------------------------------------
        void add_clear_and_optimize_primitive_list(render_primitive_list list)
        {
            // start with the assumption that we need to clear the whole screen
            init_clear_extents();

            // scan the list until we hit an intersection quad or a line
            foreach (render_primitive prim in list)
            {
                // switch off the type
                switch (prim.type)
                {
                    case render_primitive.primitive_type.LINE:
                        goto done;

                    case render_primitive.primitive_type.QUAD:
                    {
                        // stop when we hit an alpha texture
                        if (render_global.PRIMFLAG_GET_TEXFORMAT(prim.flags) == (UInt32)texture_format.TEXFORMAT_ARGB32)
                            goto done;

                        // if this quad can't be cleanly removed from the extents list, we're done
                        if (!remove_clear_extent(prim.bounds))
                            goto done;

                        // change the blendmode on the first primitive to be NONE
                        if (render_global.PRIMFLAG_GET_BLENDMODE(prim.flags) == rendertypes_global.BLENDMODE_RGB_MULTIPLY)
                        {
                            // RGB multiply will multiply against 0, leaving nothing
                            prim.color.set(1.0f, 0.0f, 0.0f, 0.0f);
                            prim.texture.base_ = null;
                            prim.flags = (prim.flags & ~render_global.PRIMFLAG_BLENDMODE_MASK) | PRIMFLAG_BLENDMODE(rendertypes_global.BLENDMODE_NONE);
                        }
                        else
                        {
                            // for alpha or add modes, we will blend against 0 or add to 0; treat it like none
                            prim.flags = (prim.flags & ~render_global.PRIMFLAG_BLENDMODE_MASK) | PRIMFLAG_BLENDMODE(rendertypes_global.BLENDMODE_NONE);
                        }

                        // since alpha is disabled, premultiply the RGB values and reset the alpha to 1.0
                        prim.color.r *= prim.color.a;
                        prim.color.g *= prim.color.a;
                        prim.color.b *= prim.color.a;
                        prim.color.a = 1.0f;
                        break;
                    }

                    default:
                        throw new emu_fatalerror("Unexpected primitive type");
                }
            }

        done:
            // now add the extents to the clear list
            add_clear_extents(list);
        }
    }


    // ======================> render_manager
    // contains machine-global information and operations
    public class render_manager : global_object, IDisposable
    {
        // internal state
        running_machine m_machine;          // reference back to the machine

        // array of live targets
        simple_list<render_target> m_targetlist = new simple_list<render_target>();       // list of targets
        render_target m_ui_target;        // current UI target

        // texture lists
        u32 m_live_textures;    // number of live textures
        u64 m_texture_id;       // rolling texture ID counter
        fixed_allocator<render_texture> m_texture_allocator = new fixed_allocator<render_texture>(); // texture allocator

        // containers for the UI and for screens
        render_container m_ui_container;     // UI container
        simple_list<render_container> m_screen_container_list = new simple_list<render_container>(); // list of containers for the screen


        // construction/destruction

        //-------------------------------------------------
        //  render_manager - constructor
        //-------------------------------------------------
        public render_manager(running_machine machine)
        {
            m_machine = machine;
            m_ui_target = null;
            m_live_textures = 0;
            m_texture_id = 0;
            m_ui_container = new render_container(this);


            // register callbacks
            machine.configuration().config_register("video", config_load, config_save);

            // create one container per screen
            foreach (screen_device screen in new screen_device_enumerator(machine.root_device()))
                screen.set_container(container_alloc(screen));
        }

        ~render_manager()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            // free all the containers since they may own textures
            m_ui_container.Dispose();
            container_free(m_ui_container);
            foreach (var container in m_screen_container_list)
                container.Dispose();
            m_screen_container_list.reset();

            //throw new emu_unimplemented();
#if false
            // better not be any outstanding textures when we die
            global.assert(m_live_textures == 0);
#endif

            m_isDisposed = true;
        }


        // getters
        public running_machine machine() { return m_machine; }


        // global queries

        //-------------------------------------------------
        //  is_live - return if the screen is 'live'
        //-------------------------------------------------
        public bool is_live(screen_device screen)
        {
            // iterate over all live targets and or together their screen masks
            foreach (render_target target in m_targetlist)
            {
                if (!target.hidden())
                {
                    layout_view view = target.current_view();
                    if (view.has_visible_screen(screen))
                        return true;
                }
            }

            return false;
        }

        //-------------------------------------------------
        //  max_update_rate - return the smallest maximum
        //  update rate across all targets
        //-------------------------------------------------
        public float max_update_rate()
        {
            // iterate over all live targets and or together their screen masks
            float minimum = 0;
            foreach (render_target target in m_targetlist)
            {
                if (target.max_update_rate() != 0)
                {
                    if (minimum == 0)
                        minimum = target.max_update_rate();
                    else
                        minimum = Math.Min(target.max_update_rate(), minimum);
                }
            }

            return minimum;
        }


        // targets

        //-------------------------------------------------
        //  target_alloc - allocate a new target
        //-------------------------------------------------
        public render_target target_alloc(internal_layout layoutfile = null, u32 flags = 0) { return m_targetlist.append(new render_target(this, layoutfile, flags)); }
        render_target target_alloc(util.xml.data_node layout, u32 flags = 0) { return m_targetlist.append(new render_target(this, layout, flags)); }


        //-------------------------------------------------
        //  target_free - free a target
        //-------------------------------------------------
        public void target_free(render_target target)
        {
            if (target != null)
                m_targetlist.remove(target);
        }


        //const simple_list<render_target> &targets() const { return m_targetlist; }


        public render_target first_target() { return m_targetlist.first(); }
        public simple_list<render_target> targetlist() { return m_targetlist; }

        //-------------------------------------------------
        //  target_by_index - get a render_target by index
        //-------------------------------------------------
        public render_target target_by_index(int index)
        {
            // count up the targets until we hit the requested index
            foreach (render_target target in m_targetlist)
            {
                if (!target.hidden())
                {
                    if (index-- == 0)
                        return target;
                }
            }

            return null;
        }


        // UI targets
        public render_target ui_target() { /*assert(m_ui_target != NULL);*/ return m_ui_target; }
        public void set_ui_target(render_target target) { m_ui_target = target; }

        //-------------------------------------------------
        //  ui_aspect - return the aspect ratio for UI
        //  fonts
        //-------------------------------------------------
        public float ui_aspect(render_container rc = null)
        {
            int orient;
            float aspect;

            if (rc == m_ui_container || rc == null)
            {
                // ui container, aggregated multi-screen target

                orient = rendutil_global.orientation_add(m_ui_target.orientation(), m_ui_container.orientation());
                // based on the orientation of the target, compute height/width or width/height
                if ((orient & emucore_global.ORIENTATION_SWAP_XY) == 0)
                    aspect = (float)m_ui_target.height() / (float)m_ui_target.width();
                else
                    aspect = (float)m_ui_target.width() / (float)m_ui_target.height();

                // if we have a valid pixel aspect, apply that and return
                if (m_ui_target.pixel_aspect() != 0.0f)
                {
                    float pixel_aspect = m_ui_target.pixel_aspect();

                    if ((orient & emucore_global.ORIENTATION_SWAP_XY) != 0)
                        pixel_aspect = 1.0f / pixel_aspect;

                    return aspect /= pixel_aspect;
                }
            }
            else
            {
                // single screen container

                orient = rc.orientation();
                // based on the orientation of the target, compute height/width or width/height
                if ((orient & emucore_global.ORIENTATION_SWAP_XY) == 0)
                    aspect = (float)rc.screen().visible_area().height() / (float)rc.screen().visible_area().width();
                else
                    aspect = (float)rc.screen().visible_area().width() / (float)rc.screen().visible_area().height();
            }

            // if not, clamp for extreme proportions
            if (aspect < 0.66f)
                aspect = 0.66f;
            if (aspect > 1.5f)
                aspect = 1.5f;

            return aspect;
        }


        // UI containers
        public render_container ui_container() { assert(m_ui_container != null);  return m_ui_container; }


        // textures
        //-------------------------------------------------
        //  texture_alloc - allocate a new texture
        //-------------------------------------------------
        public render_texture texture_alloc(texture_scaler_func scaler = null, layout_element.texture param = null) //, void *param = NULL);
        {
            // allocate a new texture and reset it
            render_texture tex = m_texture_allocator.alloc();
            tex.reset(this, scaler, param);
            tex.set_id(m_texture_id);
            m_texture_id++;
            m_live_textures++;
            return tex;
        }

        //-------------------------------------------------
        //  texture_free - release a texture
        //-------------------------------------------------
        public void texture_free(render_texture texture)
        {
            if (texture != null)
            {
                m_live_textures--;
                texture.release();
            }
            m_texture_allocator.reclaim(texture);
        }


        // fonts
        //-------------------------------------------------
        //  font_alloc - allocate a new font instance
        //-------------------------------------------------
        public render_font font_alloc(string filename = null) { return new render_font(this, filename); }


        // reference tracking
        //-------------------------------------------------
        //  invalidate_all - remove all refs to a
        //  particular reference pointer
        //-------------------------------------------------
        public void invalidate_all(object refptr)  /* possibly only bitmap_t refs */  // void *refptr)
        {
            // permit NULL
            if (refptr == null)
                return;

            // loop over targets
            foreach (render_target target in m_targetlist)
                target.invalidate_all(refptr);
        }


        // resolve tag lookups
        //-------------------------------------------------
        //  resolve_tags - resolve tag lookups
        //-------------------------------------------------
        public void resolve_tags()
        {
            foreach (render_target target in m_targetlist)
            {
                target.resolve_tags();
            }
        }


        // containers

        //-------------------------------------------------
        //  container_alloc - allocate a new container
        //-------------------------------------------------
        render_container container_alloc(screen_device screen = null)
        {
            render_container container = new render_container(this, screen);
            if (screen != null)
                m_screen_container_list.append(container);

            return container;
        }

        //-------------------------------------------------
        //  container_free - release a container
        //-------------------------------------------------
        void container_free(render_container container)
        {
            m_screen_container_list.remove(container);
        }


        // config callbacks

        //-------------------------------------------------
        //  config_load - read and apply data from the
        //  configuration file
        //-------------------------------------------------
        void config_load(config_type cfg_type, util.xml.data_node parentnode)
        {
            //throw new emu_unimplemented();
        }

        //-------------------------------------------------
        //  config_save - save data to the configuration
        //  file
        //-------------------------------------------------
        void config_save(config_type cfg_type, util.xml.data_node parentnode)
        {
            //throw new emu_unimplemented();
        }
    }
}
