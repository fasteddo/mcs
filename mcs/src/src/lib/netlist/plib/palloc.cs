// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.plib
{
    //============================================================
    // Standard arena_deleter
    //============================================================
    //template <typename P, typename T>
    //struct arena_deleter

    //============================================================
    // owned_ptr: smart pointer with ownership information
    //============================================================
    //template <typename SC, typename D>
    //class owned_ptr

    //============================================================
    // Arena allocator for use with containers
    //============================================================
    //template <class ARENA, class T, std::size_t ALIGN = alignof(T)>
    //class arena_allocator

    //============================================================
    //  Memory allocation
    //============================================================
    //struct aligned_arena


    //============================================================
    // Aligned vector
    //============================================================

    // FIXME: needs a separate file
    //template <class T, std::size_t ALIGN = PALIGN_VECTOROPT>
    class aligned_vector<T> : std.vector<T>  //class aligned_vector : public std::vector<T, aligned_allocator<T, ALIGN>>
    {
        //using base = std::vector<T, aligned_allocator<T, ALIGN>>;

        //using reference = typename base::reference;
        //using const_reference = typename base::const_reference;
        //using pointer = typename base::pointer;
        //using const_pointer = typename base::const_pointer;
        //using size_type = typename base::size_type;

        //using base::base;

        //base & as_base() noexcept { return *this; }
        //const base & as_base() const noexcept { return *this; }

        //C14CONSTEXPR reference operator[](size_type i) noexcept
        //{
        //    return assume_aligned_ptr<T, ALIGN>(&(base::operator[](0)))[i];
        //}
        //constexpr const_reference operator[](size_type i) const noexcept
        //{
        //    return assume_aligned_ptr<T, ALIGN>(&(base::operator[](0)))[i];
        //}

        //pointer data() noexcept { return assume_aligned_ptr<T, ALIGN>(base::data()); }
        //const_pointer data() const noexcept { return assume_aligned_ptr<T, ALIGN>(base::data()); }
    }
}
