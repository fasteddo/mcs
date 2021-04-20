// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    //============================================================
    //  Memory pool
    //============================================================
    //template <typename BASEARENA>
    class mempool_arena// : arena_base<mempool_arena<BASEARENA>, false, false>
    {
        //using size_type = typename BASEARENA::size_type;
        //using base_type = arena_base<mempool_arena<BASEARENA>, false, false>;
        //template <class T>
        //using base_allocator_type = typename BASEARENA::template allocator_type<T>;

        //mempool_arena(size_t min_alloc = (1<<21), size_t min_align = PALIGN_CACHELINE)

        //PCOPYASSIGNMOVE(mempool_arena, delete)

        //~mempool_arena()

        //void *allocate(size_t align, size_t size)

        //void deallocate(void *ptr, size_t size) noexcept

        //bool operator ==(const mempool_arena &rhs) const noexcept { return this == &rhs; }

        //BASEARENA &base_arena() noexcept { return m_arena; }

        //struct block

        //struct info

        //block * new_block(size_type min_bytes)

        //size_t m_min_alloc;
        //size_t m_min_align;
        //size_t m_block_align;
        //BASEARENA m_arena;

        //using base_allocator_typex = typename BASEARENA::template allocator_type<std::pair<void * const, info>>;
        //std::unordered_map<void *, info, std::hash<void *>, std::equal_to<void *>,
        //    base_allocator_typex> m_info;
        //      std::unordered_map<void *, info> m_info;
        //std::vector<block *, typename BASEARENA::template allocator_type<block *>> m_blocks;
    }
}
