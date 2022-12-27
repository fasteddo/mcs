// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_type = mame.emu.detail.device_type_impl_base;  //typedef emu::detail::device_type_impl_base const &device_type;
using int32_t = System.Int32;
using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;
using uint16_t = System.UInt16;
using uint32_t = System.UInt32;

using static mame.cpp_global;
using static mame.emucore_global;


namespace mame
{
    // ======================> sparse_dirty_rect

    // class representing a single dirty region
    public class sparse_dirty_rect : simple_list_item<sparse_dirty_rect>  //: public rectangle
    {
        //friend class simple_list<sparse_dirty_rect>;

        // internal state
        public rectangle m_rect;
        sparse_dirty_rect m_next;

        public sparse_dirty_rect() { m_next = null; }

        // getters
        public sparse_dirty_rect next() { return m_next; }
        public sparse_dirty_rect m_next_get() { return m_next; }
        public void m_next_set(sparse_dirty_rect value) { m_next = value; }
    }


    // ======================> sparse_dirty_bitmap
    class sparse_dirty_bitmap
    {
        // internal state
        int m_width;
        int m_height;
        int m_granularity;
        bitmap_ind8 m_bitmap = new bitmap_ind8();
        rectangle m_rect_list_bounds;
        fixed_allocator<sparse_dirty_rect> m_rect_allocator = new fixed_allocator<sparse_dirty_rect>();
        simple_list<sparse_dirty_rect> m_rect_list = new simple_list<sparse_dirty_rect>();


        // construction/destruction
        public sparse_dirty_bitmap(int granularity = 3)
        {
            m_width = 0;
            m_height = 0;
            m_granularity = granularity;
            m_rect_list_bounds = new rectangle(0, -1, 0, -1);
        }


        public sparse_dirty_bitmap(int width, int height, int granularity = 3)
        {
            m_width = 0;
            m_height = 0;
            m_granularity = granularity;
            m_rect_list_bounds = new rectangle(0, -1, 0, -1);


            // resize to the specified width/height
            resize(width, height);
        }


        // dirtying operations - partially intersecting tiles are dirtied
        void dirty(rectangle rect) { dirty(rect.left(), rect.right(), rect.top(), rect.bottom()); }

        //-------------------------------------------------
        //  dirty -- dirty a region
        //-------------------------------------------------
        public void dirty(int32_t left, int32_t right, int32_t top, int32_t bottom)
        {
            // compute a rectangle in dirty space, and fill it with 1
            rectangle rect = new rectangle(left >> m_granularity, right >> m_granularity, top >> m_granularity, bottom >> m_granularity);
            m_bitmap.fill(1, rect);

            // invalidate existing rect list
            invalidate_rect_list();
        }


        void dirty_all() { dirty(0, m_width - 1, 0, m_height - 1); }


        // cleaning operations - partially intersecting tiles are NOT cleaned

        public void clean(rectangle rect) { clean(rect.left(), rect.right(), rect.top(), rect.bottom()); }


        //-------------------------------------------------
        //  clean a region -- dirty a region
        //-------------------------------------------------
        void clean(int32_t left, int32_t right, int32_t top, int32_t bottom)
        {
            // if right or bottom intersect the edge of the bitmap, round up
            int round = (1 << m_granularity) - 1;
            if (right >= m_width - 1)
                right = m_width + round;
            if (bottom >= m_height - 1)
                bottom = m_height + round;

            // compute a rectangle in dirty space, and fill it with 0
            rectangle rect = new rectangle((left + round) >> m_granularity, (right - round) >> m_granularity, (top + round) >> m_granularity, (bottom - round) >> m_granularity);
            m_bitmap.fill(0, rect);

            // invalidate existing rect list
            invalidate_rect_list();
        }


        public void clean_all() { clean(0, m_width - 1, 0, m_height - 1); }


        // convert to rect list

        sparse_dirty_rect first_dirty_rect() { rectangle fullrect = new rectangle(0, m_width - 1, 0, m_height - 1); return first_dirty_rect(fullrect); }

        //-------------------------------------------------
        //  first_dirty_rect -- return the first dirty
        //  rectangle in the list
        //-------------------------------------------------
        public sparse_dirty_rect first_dirty_rect(rectangle cliprect)
        {
            // if what we have is valid, just return it again
            if (m_rect_list_bounds == cliprect)
                return m_rect_list.empty() ? null : m_rect_list.first();

            // reclaim the dirty list and start over
            m_rect_allocator.reclaim_all(m_rect_list);

            // compute dirty space rectangle coordinates
            int sx = cliprect.min_x >> m_granularity;
            int ex = cliprect.max_x >> m_granularity;
            int sy = cliprect.min_y >> m_granularity;
            int ey = cliprect.max_y >> m_granularity;
            int tilesize = 1 << m_granularity;

            // loop over all grid rows that intersect our cliprect
            for (int y = sy; y <= ey; y++)
            {
                PointerU8 dirtybase = m_bitmap.pix(y);  //uint8_t *dirtybase = &m_bitmap.pix(y);
                sparse_dirty_rect currect = null;

                // loop over all grid columns that intersect our cliprect
                for (int x = sx; x <= ex; x++)
                {
                    // if this tile is not dirty, end our current run and continue
                    if (dirtybase[x] == 0)
                    {
                        if (currect != null)
                            currect.m_rect &= cliprect;  //*currect &= cliprect;
                        currect = null;
                        continue;
                    }

                    // if we can't add to an existing rect, create a new one
                    if (currect == null)
                    {
                        // allocate a new rect and add it to the list
                        currect = m_rect_list.append(m_rect_allocator.alloc());

                        // make a rect describing this grid square
                        currect.m_rect.min_x = x << m_granularity;
                        currect.m_rect.max_x = currect.m_rect.min_x + tilesize - 1;
                        currect.m_rect.min_y = y << m_granularity;
                        currect.m_rect.max_y = currect.m_rect.min_y + tilesize - 1;
                    }

                    // if we can add to the previous rect, just expand its width
                    else
                    {
                        currect.m_rect.max_x += tilesize;
                    }
                }

                // clip the last rect to the cliprect
                if (currect != null)
                    currect.m_rect &= cliprect;
            }

            // mark the list as valid
            m_rect_list_bounds = cliprect;
            return m_rect_list.empty() ? null : m_rect_list.first();
        }


        // dynamic resizing
        //-------------------------------------------------
        //  resize -- resize the bitmap
        //-------------------------------------------------
        public void resize(int width, int height)
        {
            // set new size
            m_width = width;
            m_height = height;

            // resize the bitmap
            int round = (1 << m_granularity) - 1;
            m_bitmap.resize((width + round) >> m_granularity, (height + round) >> m_granularity);

            // reset everything
            dirty_all();
        }


        // invalidate cached rect list
        void invalidate_rect_list() { m_rect_list_bounds.set(0, -1, 0, -1); }
    }


    // ======================> sprite_device
    //template<typename _SpriteRAMType, class _BitmapType>
    public abstract class sprite_device<_SpriteRAMType, _BitmapType> : device_t where _BitmapType : bitmap_ind16, new()
    {
        // constants
        const int BITMAP_SLOP = 16;


        // configuration
        int32_t m_xorigin;              // X origin for drawing
        int32_t m_yorigin;              // Y origin for drawing

        // memory pointers and buffers
        PointerU8 m_spriteram;  //_SpriteRAMType *                m_spriteram;            // pointer to spriteram pointer
        int32_t m_spriteram_bytes;      // size of sprite RAM in bytes
        std.vector<byte> m_buffer = new std.vector<byte>();  //std::vector<_SpriteRAMType>          m_buffer;               // buffered spriteram for those that use it

        // bitmaps
        _BitmapType m_bitmap = new _BitmapType();               // live bitmap
        sparse_dirty_bitmap m_dirty;                // dirty bitmap


        Func<PointerU16, int, int, int, _BitmapType> m_bitmapCreator;


        // construction/destruction - only for subclasses
        public sprite_device(machine_config mconfig, device_type type, string tag, device_t owner, int dirty_granularity = 3)
            : base(mconfig, type, tag, owner, 0)
        {
            m_xorigin = 0;
            m_yorigin = 0;
            m_spriteram = null;
            m_spriteram_bytes = 0;
            m_dirty = new sparse_dirty_bitmap(dirty_granularity);


            force_clear();
        }


        public void sprite_device_after_ctor(Func<PointerU16, int, int, int, _BitmapType> bitmapCreator)
        {
            m_bitmapCreator = bitmapCreator;
        }


        // getters
        //int32_t xorigin() const { return m_xorigin; }
        //int32_t yorigin() const { return m_yorigin; }

        public _BitmapType bitmap() { return m_bitmap; }

        //sparse_dirty_rect *first_dirty_rect() { return m_dirty.first_dirty_rect(); }
        public sparse_dirty_rect first_dirty_rect(rectangle cliprect) { return m_dirty.first_dirty_rect(cliprect); }
        public PointerU8 spriteram() { return m_spriteram; }  //_SpriteRAMType *spriteram() const { return m_spriteram; }
        //uint32_t spriteram_bytes() const { return m_spriteram_bytes; }
        //uint32_t spriteram_elements() const { return m_spriteram_bytes / sizeof(_SpriteRAMType); }
        //_SpriteRAMType *buffer() { return &m_buffer[0]; }

        // configuration

        void set_spriteram(PointerU8 base_, uint32_t bytes) { assert(base_ != null && bytes != 0); m_spriteram = base_; m_spriteram_bytes = (int32_t)bytes; m_buffer.resize((size_t)m_spriteram_bytes); }  //void set_spriteram(_SpriteRAMType *base, uint32_t bytes) { assert(base != nullptr && bytes != 0); m_spriteram = base; m_spriteram_bytes = bytes; m_buffer.resize(m_spriteram_bytes / sizeof(_SpriteRAMType)); }

        //void set_origin(int32_t xorigin = 0, int32_t yorigin = 0) { m_xorigin = xorigin; m_yorigin = yorigin; }
        //void set_xorigin(int32_t xorigin) { m_xorigin = xorigin; }
        //void set_yorigin(int32_t yorigin) { m_yorigin = yorigin; }

        // buffering
        //void copy_to_buffer() { assert(m_spriteram != nullptr); memcpy(m_buffer, m_spriteram, m_spriteram_bytes); }

        // clearing

        //void clear() { clear(m_bitmap.cliprect()); }

        void clear(rectangle cliprect)
        {
            for (sparse_dirty_rect rect = m_dirty.first_dirty_rect(cliprect); rect != null; rect = rect.next())
                m_bitmap.fill(~0U, rect.m_rect);
            m_dirty.clean(cliprect);
        }


        // force clear (don't use dirty rects)
        void force_clear()
        {
            m_bitmap.fill(uint32_t.MaxValue);  //~0);
            m_dirty.clean_all();
        }


        // drawing
        public void draw_async(rectangle cliprect, bool clearit = true)
        {
            // if the cliprect exceeds our current bitmap dimensions, expand
            if (cliprect.right() >= m_bitmap.width() || cliprect.bottom() >= m_bitmap.height())
            {
                int new_width = std.max(cliprect.right() + 1, m_bitmap.width());
                int new_height = std.max(cliprect.bottom() + 1, m_bitmap.height());
                m_bitmap.resize(new_width, new_height, BITMAP_SLOP, BITMAP_SLOP);
                m_dirty.resize(new_width, new_height);
            }

            // clear out the region
            if (clearit)
                clear(cliprect);

            // wrap the bitmap, adjusting for x/y origins
            _BitmapType wrapped = m_bitmapCreator(m_bitmap.pix(0) - m_xorigin - m_yorigin * m_bitmap.rowpixels(), m_xorigin + cliprect.right() + 1, m_yorigin + cliprect.bottom() + 1, m_bitmap.rowpixels());  //_BitmapType wrapped(&m_bitmap.pix(0) - m_xorigin - m_yorigin * m_bitmap.rowpixels(), m_xorigin + cliprect.right() + 1, m_yorigin + cliprect.bottom() + 1, m_bitmap.rowpixels());

            // compute adjusted cliprect in source space
            rectangle adjusted = cliprect;
            adjusted.offset(m_xorigin, m_yorigin);

            // render
            draw(wrapped, adjusted);
        }


        // device-level overrides

        protected override void device_start()
        {
            // find spriteram
            memory_share spriteram = owner().memshare(tag());
            if (spriteram != null)
            {
                set_spriteram(spriteram.ptr(), (uint32_t)spriteram.bytes());  //set_spriteram(reinterpret_cast<_SpriteRAMType *>(spriteram->ptr()), spriteram->bytes());

                // save states
                save_item(NAME(new { m_buffer }));
            }
        }


        // subclass overrides
        protected abstract void draw(_BitmapType bitmap, rectangle cliprect);


        // subclass helpers
        void mark_dirty(rectangle rect) { mark_dirty(rect.left(), rect.right(), rect.top(), rect.bottom()); }
        protected void mark_dirty(int32_t left, int32_t right, int32_t top, int32_t bottom) { m_dirty.dirty(left - m_xorigin, right - m_xorigin, top - m_yorigin, bottom - m_yorigin); }
    }


    //typedef sprite_device<uint8_t, bitmap_ind16> sprite8_device_ind16;

    //typedef sprite_device<uint16_t, bitmap_ind16> sprite16_device_ind16;
    public abstract class sprite16_device_ind16 : sprite_device<uint16_t, bitmap_ind16>
    {
        public sprite16_device_ind16(machine_config mconfig, device_type type, string tag, device_t owner, int dirty_granularity = 3) : base(mconfig, type, tag, owner, dirty_granularity)
        {
            sprite_device_after_ctor((base_, width, height, rowpixels) => { return new bitmap_ind16(base_, width, height, rowpixels); });
        }
    }

    //typedef sprite_device<uint32_t, bitmap_ind16> sprite32_device_ind16;

    //typedef sprite_device<uint8_t, bitmap_ind32> sprite8_device_ind32;
    //typedef sprite_device<uint16_t, bitmap_ind32> sprite16_device_ind32;
    //typedef sprite_device<uint32_t, bitmap_ind32> sprite32_device_ind32;
}
