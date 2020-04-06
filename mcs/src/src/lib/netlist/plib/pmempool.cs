// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    //============================================================
    //  Memory pool
    //============================================================
    public class mempool
    {
        //using size_type = std::size_t;

        //static constexpr const bool is_stateless = false;


        //template <class T, size_type ALIGN = alignof(T)>
        //using allocator_type = arena_allocator<mempool, T, ALIGN>;


        //size_t m_min_alloc;
        //size_t m_min_align;

        //std::vector<block *> m_blocks;

        //size_t m_stat_cur_alloc;
        //size_t m_stat_max_alloc;


        //mempool(size_t min_alloc = (1<<21), size_t min_align = PALIGN_CACHELINE)

        //COPYASSIGNMOVE(mempool, delete)

        //~mempool()

        //void *allocate(size_t align, size_t size)

        //static void deallocate(void *ptr, size_t size)

        //template <typename T>
        //using owned_pool_ptr = plib::owned_ptr<T, arena_deleter<mempool, T>>;

        //template <typename T>
        //using unique_pool_ptr = std::unique_ptr<T, arena_deleter<mempool, T>>;

        //template<typename T, typename... Args>
        //owned_pool_ptr<T> make_owned(Args&&... args)

        //template<typename T, typename... Args>
        //unique_pool_ptr<T> make_unique(Args&&... args)

        //size_type cur_alloc() const noexcept { return m_stat_cur_alloc; }
        //size_type max_alloc() const noexcept { return m_stat_max_alloc; }

        //bool operator ==(const mempool &rhs) const noexcept { return this == &rhs; }

        //block * new_block(size_type min_bytes)

        //static std::unordered_map<void *, info> &sinfo()
    }
}
