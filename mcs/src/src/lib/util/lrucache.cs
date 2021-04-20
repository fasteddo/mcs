// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.util
{
    // LRU cache that behaves like std::map with differences:
    // * drops least-recently used items if necessary on insert to prevent size from exceeding max_size
    // * operator[], at, insert, emplace and find freshen existing entries
    // * iterates from least- to most-recently used rather than in order by key
    // * iterators to dropped items are invalidated
    // * not all map interfaces implemented
    // * copyable and swappable but not movable
    // * swap may invalidate past-the-end iterator, other iterators refer to new container
    //template <typename Key, typename T, typename Compare = std::less<Key>, class Allocator = std::allocator<std::pair<Key const, T> > >
    class lru_cache_map<Key, T> : std.map<Key, T>
    {
        public lru_cache_map(UInt32 max_size) : base()
        {
        }
    }
}
