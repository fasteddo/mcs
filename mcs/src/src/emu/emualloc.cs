// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame
{
    public static class emualloc_global
    {
        //**************************************************************************
        //  MACROS
        //**************************************************************************

        // pool allocation helpers
        //#define pool_alloc(_pool, _type)                    (_pool).add_object(new _type)
        //#define pool_alloc_clear(_pool, _type)              (_pool).add_object(make_unique_clear _type .release())
        public static MemoryContainer<T> pool_alloc_array<T>(UInt32 num) where T : new() { return new MemoryContainer<T>((int)num); }  //#define pool_alloc_array(_pool, _type, _num)        (_pool).add_array(new _type [_num], (_num))
        public static MemoryContainer<T> pool_alloc_array_clear<T>(UInt32 num) where T : new() { return new MemoryContainer<T>((int)num); }  //#define pool_alloc_array_clear(_pool, _type, _num)  (_pool).add_array(make_unique_clear<_type []>(_num).release(), (_num))
        //#define pool_free(_pool, v)                         (_pool).remove(v)
    }
}
