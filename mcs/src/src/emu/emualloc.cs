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
        //#define pool_alloc(_pool, _type)                    (_pool).add_object(global_alloc(_type))
        //#define pool_alloc_clear(_pool, _type)              (_pool).add_object(global_alloc_clear _type)
        //#define pool_alloc_array(_pool, _type, _num)        (_pool).add_array(global_alloc_array(_type,_num), (_num))
        public static ListBase<T> pool_alloc_array<T>(UInt32 num) where T : new() { return global_object.global_alloc_array<T>(num); }  //(_pool).add_array(global_alloc_array(_type,_num), (_num))
        //#define pool_alloc_array_clear(_pool, _type, _num)  (_pool).add_array(global_alloc_array_clear<_type>(_num), (_num))
        public static ListBase<T> pool_alloc_array_clear<T>(UInt32 num) where T : new() { return global_object.global_alloc_array_clear<T>(num); }  // (_pool).add_array(global_alloc_array_clear<_type>(_num), (_num))
        //#define pool_free(_pool, v)                         (_pool).remove(v)
    }
}
