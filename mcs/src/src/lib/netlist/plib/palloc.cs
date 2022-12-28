// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using u32 = System.UInt32;


namespace mame.plib
{
    ///
    /// \brief alignment aware deleter class
    ///
    /// The deleter class expects the object to have been allocated with
    /// `alignof(T)` alignment if the ALIGN parameter is omitted. If ALIGN is
    /// given, this is used.
    ///
    /// \tparam A Arena type
    /// \tparam T Object type
    /// \tparam ALIGN alignment
    ///
    //template <typename A, typename T, std::size_t ALIGN = 0>
    //struct arena_deleter : public arena_deleter_base<A, T, ALIGN, A::has_static_deallocator>

    //============================================================
    // owned_ptr: smart pointer with ownership information
    //============================================================
    //template <typename SC, typename D>
    //class owned_ptr

    //============================================================
    // Arena allocator for use with containers
    //============================================================
    //template <class ARENA, class T, std::size_t ALIGN, bool HSA>
    //class arena_allocator

    //============================================================
    //  Memory allocation
    //============================================================
    //template <typename P, std::size_t MINALIGN, bool HSD, bool HSA>
    //struct arena_core

    //template <typename P, std::size_t MINALIGN, bool HSD, bool HSA>
    //struct arena_base : public arena_core<P, MINALIGN, HSD, HSA>

    //template <std::size_t MINALIGN>
    //struct aligned_arena : public arena_base<aligned_arena<MINALIGN>, MINALIGN, true, true>

    //struct std_arena : public arena_base<std_arena, 0, true, true>

    //template <typename A, typename T, template <typename, typename> class S, std::size_t ALIGN = PALIGN_VECTOROPT>
    //class arena_sequence : public S<T, typename A::template allocator_type<T, ALIGN>>

    //template <typename A, typename T, std::size_t ALIGN = PALIGN_VECTOROPT>
    //class arena_vector : public arena_sequence<A, T, std::vector, ALIGN>
}
