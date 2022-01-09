// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using size_t = System.UInt64;
using static_vector_size_type = System.UInt64;  //using size_type = std::size_t;
using unsigned = System.UInt32;


namespace mame.plib
{
    /// \brief Array holding uninitialized elements
    ///
    /// Use with care. This template is provided to improve locality of storage
    /// in high frequency applications. It should not be used for anything else.
    ///
    //template <class C, std::size_t N>
    class uninitialised_array<C, size_t_N>
        where size_t_N : u64_const, new()
    {
        //using value_type = C;
        //using pointer = value_type *;
        //using const_pointer = const value_type *;
        //using reference = value_type &;
        //using const_reference = const value_type &;
        //using iterator = value_type *;
        //using const_iterator = const value_type *;
        //using size_type = std::size_t;
        //using difference_type = std::ptrdiff_t;
        //using reverse_iterator = std::reverse_iterator<iterator>;
        //using const_reverse_iterator = std::reverse_iterator<const_iterator>;


        // ensure proper alignment
        //PALIGNAS_VECTOROPT()
        std.array<C, size_t_N> m_buf;  //std::array<typename std::aligned_storage<sizeof(C), alignof(C)>::type, N> m_buf;
        unsigned m_initialized;


        //uninitialised_array_t() noexcept = default;
        public uninitialised_array() { }


        //PCOPYASSIGNMOVE(uninitialised_array_t, delete)
        //~uninitialised_array_t() noexcept
        //{
        //    if (m_initialized>=N)
        //        for (size_type i=0; i<N; ++i)
        //            (*this)[i].~C();
        //}


        //constexpr size_t size() const noexcept { return N; }

        //constexpr bool empty() const noexcept { return size() == 0; }

        //reference operator[](size_type index) noexcept

        //constexpr const_reference operator[](size_type index) const noexcept
        public C this[int index] { get { return m_buf[index]; } set { m_buf[index] = value; } }
        public C this[UInt64 index] { get { return m_buf[index]; } set { m_buf[index] = value; } }


        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator begin() const noexcept { return reinterpret_cast<iterator>(&m_buf[0]); }
        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator end() const noexcept { return reinterpret_cast<iterator>(&m_buf[0] + N); }

        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator begin() noexcept { return reinterpret_cast<iterator>(&m_buf[0]); }
        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator end() noexcept { return reinterpret_cast<iterator>(&m_buf[0] + N); }

        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //const_iterator cbegin() const noexcept { return reinterpret_cast<const_iterator>(&m_buf[0]); }
        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //const_iterator cend() const noexcept { return reinterpret_cast<const_iterator>(&m_buf[0] + N); }

        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator begin() const noexcept { return reinterpret_cast<iterator>(&m_buf[0]); }
        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator end() const noexcept { return reinterpret_cast<iterator>(&m_buf[N]); }

        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator begin() noexcept { return reinterpret_cast<iterator>(&m_buf[0]); }
        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator end() noexcept { return reinterpret_cast<iterator>(&m_buf[N]); }

        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //const_iterator cbegin() const noexcept { return reinterpret_cast<const_iterator>(&m_buf[0]); }
        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //const_iterator cend() const noexcept { return reinterpret_cast<const_iterator>(&m_buf[N]); }
    }


    /// \brief fixed allocation vector
    ///
    /// Currently only emplace_back and clear are supported.
    ///
    /// Use with care. This template is provided to improve locality of storage
    /// in high frequency applications. It should not be used for anything else.
    ///
    //template <class C, std::size_t N>
    class static_vector<C, size_t_N>
        where size_t_N : u64_const, new()
    {
        protected static readonly size_t N = new size_t_N().value;


        //using value_type = C;
        //using pointer = value_type *;
        //using const_pointer = const value_type *;
        //using reference = value_type &;
        //using const_reference = const value_type &;
        //using iterator = value_type *;
        //using const_iterator = const value_type *;
        //using size_type = std::size_t;
        //using difference_type = std::ptrdiff_t;
        //using reverse_iterator = std::reverse_iterator<iterator>;
        //using const_reverse_iterator = std::reverse_iterator<const_iterator>;


        std.array<C, size_t_N> m_buf = new std.array<C, size_t_N>();
        static_vector_size_type m_pos;


        public static_vector()
        {
            m_pos = 0;
        }


        //PCOPYASSIGNMOVE(static_vector, delete)
        //~static_vector() noexcept

        public size_t size() { return m_pos; }

        //constexpr bool empty() const noexcept { return size() == 0; }

        //void clear()

        //template<typename... Args>
        public void emplace_back(C args)  //void emplace_back(Args&&... args)
        {
            // placement new on buffer
            m_buf[m_pos] = args;  //new (&m_buf[m_pos]) C(std::forward<Args>(args)...);
            m_pos++;
        }


        //reference operator[](size_type index) noexcept
        //constexpr const_reference operator[](size_type index) const noexcept
        public C op(size_t index) { return m_buf[index]; }


        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator begin() const noexcept { return reinterpret_cast<iterator>(&m_buf[0]); }
        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator end() const noexcept { return reinterpret_cast<iterator>(&m_buf[0] + m_pos); }

        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator begin() noexcept { return reinterpret_cast<iterator>(&m_buf[0]); }
        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //iterator end() noexcept { return reinterpret_cast<iterator>(&m_buf[0] + m_pos); }

        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //const_iterator cbegin() const noexcept { return reinterpret_cast<const_iterator>(&m_buf[0]); }
        // NOLINTNEXTLINE(cppcoreguidelines-pro-type-reinterpret-cast)
        //const_iterator cend() const noexcept { return reinterpret_cast<const_iterator>(&m_buf[0] + m_pos); }
    }


    /// \brief a simple linked list.
    ///
    /// The list allows insertions and deletions whilst being processed.
    ///
    //template <class LC>
    class linkedlist_t<LC>
    {
        //struct element_t

        //struct iter_t final : public std::iterator<std::forward_iterator_tag, LC>


        //LC *m_head;
        LinkedList<LC> m_list = new LinkedList<LC>();


        public linkedlist_t() { }  //constexpr element_t() : m_next(nullptr), m_prev(nullptr) {}


        //constexpr iter_t begin() const noexcept { return iter_t(m_head); }
        //constexpr iter_t end() const noexcept { return iter_t(nullptr); }


        public void push_front(LC elem)
        {
            m_list.AddFirst(elem);
        }


        public void push_back(LC elem)
        {
            m_list.AddLast(elem);
        }


        public void remove(LC elem)
        {
            m_list.Remove(elem);
        }

        //constexpr LC *front() const noexcept { return m_head; }

        public bool empty() { return m_list.Count == 0; }

        public void clear() { m_list.Clear(); }


        public IEnumerator<LC> GetEnumerator() { return m_list.GetEnumerator(); }
    }
}
