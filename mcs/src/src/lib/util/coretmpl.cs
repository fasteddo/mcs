// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;

using s32 = System.Int32;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using u64 = System.UInt64;
using uX = mame.FlexPrim;

using static mame.util;


namespace mame
{
    public interface simple_list_item<ElementType>
    {
        ElementType next();
        void m_next_set(ElementType obj);
        ElementType m_next_get();
    }


    // ======================> simple_list
    // a simple_list is a singly-linked list whose 'next' pointer is owned
    // by the object
    //template<class ElementType>
    public class simple_list<ElementType> : IEnumerable<ElementType> where ElementType : simple_list_item<ElementType>
    {
        public class auto_iterator
        {
            // private state
            ElementType m_current;

            // construction/destruction
            public auto_iterator(ElementType ptr) { m_current = ptr; }

            public ElementType current() { return m_current; }
            public void advance() { m_current = m_current.next(); }

            // required operator overrides
            //bool operator!=(const auto_iterator &iter) const noexcept { return m_current != iter.m_current; }
            //_ElementType &operator*() const noexcept { return *m_current; }
            // note that _ElementType::next() must not return a const ptr
            //const auto_iterator &operator++() noexcept { m_current = m_current->next(); return *this; }
        }


        // internal state
        ElementType m_head = default;         // head of the singly-linked list
        ElementType m_tail = default;         // tail of the singly-linked list
        int m_count = 0;        // number of objects in the list


        // construction/destruction
        public simple_list() { }
        //~simple_list() { reset(); }


        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
        public IEnumerator<ElementType> GetEnumerator()
        {
            auto_iterator iter = new auto_iterator(m_head);

            while (iter.current() != null)
            {
                yield return iter.current();
                iter.advance();
            }
        }


        // simple getters
        public ElementType first() { return m_head; }
        public ElementType last() { return m_tail; }
        public int count() { return m_count; }
        public bool empty() { return count() == 0; }

        // range iterators
        public auto_iterator begin() { return new auto_iterator(m_head); }
        public auto_iterator end() { return new auto_iterator(default); }

        // remove (free) all objects in the list, leaving an empty list
        public void reset()
        {
            while (m_head != null)
                remove(m_head);
        }

        // add the given object to the head of the list
        public ElementType prepend(ElementType obj)
        {
            obj.m_next_set(m_head);
            m_head = obj;
            if (m_tail == null)
                m_tail = m_head;
            m_count++;
            return obj;
        }

        // add the given list to the head of the list
        public void prepend_list(simple_list<ElementType> list)
        {
            int count = list.count();
            if (count == 0)
                return;
            ElementType tail = list.last();
            ElementType head = list.detach_all();
            tail.m_next_set(m_head);
            m_head = head;
            if (m_tail == null)
                m_tail = tail;
            m_count += count;
        }

        // add the given object to the tail of the list
        public ElementType append(ElementType obj)
        {
            obj.m_next_set(default);
            if (m_tail != null)
            {
                m_tail.m_next_set(obj);
                m_tail = obj;
            }
            else
            {
                m_tail = m_head = obj;
            }
            m_count++;
            return obj;
        }

#if false
        // add the given list to the tail of the list
        void append_list(simple_list<_ElementType> &list)
        {
            int count = list.count();
            if (count == 0)
                return;
            _ElementType *tail = list.last();
            _ElementType *head = list.detach_all();
            if (m_tail != NULL)
                m_tail->m_next = head;
            else
                m_head = head;
            m_tail = tail;
            m_count += count;
        }
#endif

        // insert the given object after a particular object (NULL means prepend)
        public ElementType insert_after(ElementType obj, ElementType insert_after)
        {
            if (insert_after == null)
                return prepend(obj);

            obj.m_next_set(insert_after.m_next_get());
            insert_after.m_next_set(obj);
            if (m_tail.Equals(insert_after))
                m_tail = obj;
            m_count++;
            return obj;
        }

        // insert the given object before a particular object (NULL means append)
        public ElementType insert_before(ElementType obj, ElementType insert_before)
        {
            if (insert_before == null)
                return append(obj);

            for (ElementType curptr = m_head; curptr != null; curptr = curptr.m_next_get())
            {
                if (curptr.Equals(insert_before))
                {
                    obj.m_next_set(insert_before);
                    curptr = obj;

                    if (m_head.Equals(insert_before))
                        m_head = obj;

                    m_count++;

                    return obj;
                }
            }

            return obj;
        }


        // replace an item in the list at the same location, and remove it
        public ElementType replace_and_remove(ElementType object_, ElementType toreplace)
        {
            ElementType prev = default;
            for (ElementType cur = m_head; cur != null; prev = cur, cur = cur.m_next_get())
            {
                if (cur.Equals(toreplace))
                {
                    if (prev != null)
                        prev.m_next_set(object_);
                    else
                        m_head = object_;

                    if (m_tail.Equals(toreplace))
                        m_tail = object_;

                    object_.m_next_set(toreplace.m_next_get());
                    //delete &toreplace;
                    return object_;
                }
            }

            return append(object_);
        }


        // detach the head item from the list, but don't free its memory
        public ElementType detach_head()
        {
            ElementType result = m_head;
            if (result != null)
            {
                m_head = result.m_next_get();
                m_count--;
                if (m_head == null)
                    m_tail = default;
            }
            return result;
        }

        // detach the given item from the list, but don't free its memory
        public ElementType detach(ElementType obj)
        {
            ElementType prev = default;
            for (ElementType cur = m_head; cur != null; prev = cur, cur = cur.m_next_get())
            {
                if (cur.Equals(obj))
                {
                    if (prev != null)
                        prev.m_next_set(obj.m_next_get());
                    else
                        m_head = obj.m_next_get();

                    if (m_tail.Equals(obj))
                        m_tail = prev;

                    m_count--;
                    return obj;
                }
            }
            return obj;
        }

        // deatch the entire list, returning the head, but don't free memory
        public ElementType detach_all()
        {
            ElementType result = m_head;
            m_head = m_tail = default;
            m_count = 0;
            return result;
        }

        // remove the given object and free its memory
        public void remove(ElementType obj)
        {
            //global_free(&detach(object));
            detach(obj);
        }

        // find an object by index in the list
        public ElementType find(int index)
        {
            for (ElementType cur = m_head; cur != null; cur = cur.m_next_get())
            {
                if (index-- == 0)
                    return cur;
            }
            return default;
        }

        // return the index of the given object in the list
        public int indexof(ElementType obj)
        {
            int index = 0;
            for (ElementType cur = m_head; cur != null; cur = cur.m_next_get())
            {
                if (cur.Equals(obj))
                    return index;

                index++;
            }

            return -1;
        }
    }


    // ======================> fixed_allocator
    // a fixed_allocator is a simple class that maintains a free pool of objects
    //template<class ItemType>
    public class fixed_allocator<ItemType> where ItemType : simple_list_item<ItemType>, new()
    {
        // allocate a new item, either by recycling an old one, or by allocating a new one
        public ItemType alloc()
        {
            return new ItemType();
        }

        // reclaim an item by adding it to the free list
        public void reclaim(ItemType item) { }

        // reclaim all items from a list
        public void reclaim_all(simple_list<ItemType> _list) { _list.detach_all(); }
    }


    //template<typename T>
    //class contiguous_sequence_wrapper

    //template <typename T, std::size_t N, bool WriteWrap = false, bool ReadWrap = WriteWrap>
    //class fifo : protected std::array<T, N>


    public static partial class util
    {
        //template <typename CharT, typename Traits, typename Allocator>
        //std::basic_string_view<CharT, Traits> buf_to_string_view(basic_ovectorstream<CharT, Traits, Allocator> &stream)

        // For declaring an array of the same dimensions as another array (including multi-dimensional arrays)
        //template <typename T, typename U> struct equivalent_array_or_type { typedef T type; };
        //template <typename T, typename U, std::size_t N> struct equivalent_array_or_type<T, U[N]> { typedef typename equivalent_array_or_type<T, U>::type type[N]; };
        //template <typename T, typename U> using equivalent_array_or_type_t = typename equivalent_array_or_type<T, U>::type;
        //template <typename T, typename U> struct equivalent_array { };
        //template <typename T, typename U, std::size_t N> struct equivalent_array<T, U[N]> { typedef equivalent_array_or_type_t<T, U> type[N]; };
        //template <typename T, typename U> using equivalent_array_t = typename equivalent_array<T, U>::type;
        //#define EQUIVALENT_ARRAY(a, T) util::equivalent_array_t<T, std::remove_reference_t<decltype(a)> >

        //template <typename E>
        //using enable_enum_t = typename std::enable_if_t<std::is_enum<E>::value, typename std::underlying_type_t<E> >;

        // template function which takes a strongly typed enumerator and returns its value as a compile-time constant
        //template <typename E>
        //constexpr enable_enum_t<E> underlying_value(E e) noexcept

        // template function which takes an integral value and returns its representation as enumerator (even strongly typed)
        //template <typename E , typename T>
        //constexpr typename std::enable_if_t<std::is_enum<E>::value && std::is_integral<T>::value, E> enum_value(T value) noexcept


        /// \defgroup bitutils Useful functions for bit shuffling
        /// \{

        /// \brief Generate a right-aligned bit mask
        ///
        /// Generates a right aligned mask of the specified width.  Works with
        /// signed and unsigned integer types.
        /// \tparam T Desired output type.
        /// \tparam U Type of the input (generally resolved by the compiler).
        /// \param [in] n Width of the mask to generate in bits.
        /// \return Right-aligned mask of the specified width.
        //template <typename T, typename U> constexpr T make_bitmask(U n)
        //{
        //    return T((n < (8 * sizeof(T)) ? (std::make_unsigned_t<T>(1) << n) : std::make_unsigned_t<T>(0)) - 1);
        //}
        //template <typename T, typename U>
        public static u8 make_bitmask8(u32 n) { return make_bitmask8((s32)n); }
        public static u8 make_bitmask8(s32 n) { return (u8)((n < (8 * 1) ? (1U << n) : 0U) - 1); }
        public static u16 make_bitmask16(u32 n) { return make_bitmask16((s32)n); }
        public static u16 make_bitmask16(s32 n) { return (u16)((n < (8 * 2) ? (1U << n) : 0U) - 1); }
        public static u32 make_bitmask32(u32 n) { return make_bitmask32((s32)n); }
        public static u32 make_bitmask32(s32 n) { return (u32)((n < (8 * 4) ? (1U << n) : 0U) - 1); }
        public static u64 make_bitmask64(u32 n) { return make_bitmask64((s32)n); }
        public static u64 make_bitmask64(s32 n) { return (u64)((n < (8 * 8) ? (1U << n) : 0U) - 1); }
        public static uX make_bitmask_uX(int width, u32 n) { return make_bitmask_uX(width, (s32)n); }
        public static uX make_bitmask_uX(int width, s32 n) { return new uX(width, (u64)((n < (8 * 8) ? (1U << n) : 0U) - 1)); }


        /// \brief Extract a single bit from an integer
        ///
        /// Extracts a single bit from an integer into the least significant bit
        /// position.
        ///
        /// \param [in] x The integer to extract the bit from.
        /// \param [in] n The bit to extract, where zero is the least
        ///   significant bit of the input.
        /// \return Zero if the specified bit is unset, or one if it is set.
        /// \sa bitswap
        //template <typename T, typename U> constexpr T BIT(T x, U n) { return (x >> n) & T(1); }
        public static int BIT(int x, int n) { return (x >> n) & 1; }
        public static UInt32 BIT(UInt32 x, int n) { return (x >> n) & 1; }

        /// \brief Extract a bit field from an integer
        ///
        /// Extracts and right-aligns a bit field from an integer.
        ///
        /// \param [in] x The integer to extract the bit field from.
        /// \param [in] n The least significant bit position of the field to
        ///   extract, where zero is the least significant bit of the input.
        /// \param [in] w The width of the field to extract in bits.
        /// \return The field [n..(n+w-1)] from the input.
        /// \sa bitswap
        //template <typename T, typename U, typename V>
        public static UInt32 BIT(UInt32 x, UInt32 n, UInt32 w)  //template <typename T, typename U, typename V> constexpr T BIT(T x, U n, V w)
        {
            return (x >> (int)n) & make_bitmask32(w);
        }

        public static UInt32 BIT(UInt32 x, int n, UInt32 w)  //template <typename T, typename U, typename V> constexpr T BIT(T x, U n, V w)
        {
            return (x >> n) & make_bitmask32(w);
        }

        public static UInt32 BIT(UInt32 x, int n, int w)  //template <typename T, typename U, typename V> constexpr T BIT(T x, U n, V w)
        {
            return (x >> n) & make_bitmask32(w);
        }


        /// \brief Extract bits in arbitrary order
        ///
        /// Extracts bits from an integer.  Specify the bits in the order they
        /// should be arranged in the output, from most significant to least
        /// significant.  The extracted bits will be packed into a right-aligned
        /// field in the output.
        ///
        /// \param [in] val The integer to extract bits from.
        /// \param [in] b The first bit to extract from the input
        ///   extract, where zero is the least significant bit of the input.
        ///   This bit will appear in the most significant position of the
        ///   right-aligned output field.
        /// \param [in] c The remaining bits to extract, where zero is the
        ///   least significant bit of the input.
        /// \return The extracted bits packed into a right-aligned field.
        //template <typename T, typename U, typename... V> constexpr T bitswap(T val, U b, V... c) noexcept
        //{
        //    if constexpr (sizeof...(c) > 0U)
        //        return (BIT(val, b) << sizeof...(c)) | bitswap(val, c...);
        //    else
        //        return BIT(val, b);
        //}

        /// \brief Extract bits in arbitrary order with explicit count
        ///
        /// Extracts bits from an integer.  Specify the bits in the order they
        /// should be arranged in the output, from most significant to least
        /// significant.  The extracted bits will be packed into a right-aligned
        /// field in the output.  The number of bits to extract must be supplied
        /// as a template argument.
        ///
        /// A compile error will be generated if the number of bit positions
        /// supplied does not match the specified number of bits to extract, or
        /// if the output type is too small to hold the extracted bits.  This
        /// guards against some simple errors.
        ///
        /// \tparam B The number of bits to extract.  Must match the number of
        ///   bit positions supplied.
        /// \param [in] val The integer to extract bits from.
        /// \param [in] b Bits to extract, where zero is the least significant
        ///   bit of the input.  Specify bits in the order they should appear in
        ///   the output field, from most significant to least significant.
        /// \return The extracted bits packed into a right-aligned field.
        //template <unsigned B, typename T, typename... U> T bitswap(T val, U... b) noexcept
        //{
        //    static_assert(sizeof...(b) == B, "wrong number of bits");
        //    static_assert((sizeof(std::remove_reference_t<T>) * 8) >= B, "return type too small for result");
        //    return bitswap(val, b...);
        //}

        public static int bitswap(int val, int B1, int B0)
        {
            return ((BIT(val,B1) << 1) | (BIT(val,B0) << 0));
        }

        public static int bitswap(int val, int B3, int B2, int B1, int B0)
        {
            return ((BIT(val,B3) << 3) | (BIT(val,B2) << 2) | (BIT(val,B1) << 1) | (BIT(val,B0) << 0));
        }

        public static int bitswap(int val, int B5, int B4, int B3, int B2, int B1, int B0)
        {
            return ((BIT(val,B5) << 5) | (BIT(val,B4) << 4) | (BIT(val,B3) << 3) | (BIT(val,B2) << 2) |
                    (BIT(val,B1) << 1) | (BIT(val,B0) << 0));
        }

        public static int bitswap(int val, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0)
        {
            return ((BIT(val,B7) << 7) | (BIT(val,B6) << 6) | (BIT(val,B5) << 5) | (BIT(val,B4) << 4) |
                    (BIT(val,B3) << 3) | (BIT(val,B2) << 2) | (BIT(val,B1) << 1) | (BIT(val,B0) << 0));
        }

        public static int bitswap(int val, int B10, int B9, int B8, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0)
        {
            return ((BIT(val,B10) << 10) | (BIT(val, B9) <<  9) | (BIT(val, B8) <<  8) | (BIT(val, B7) <<  7) |
                    (BIT(val, B6) <<  6) | (BIT(val, B5) <<  5) | (BIT(val, B4) <<  4) | (BIT(val, B3) <<  3) |
                    (BIT(val, B2) <<  2) | (BIT(val, B1) <<  1) | (BIT(val, B0) <<  0));
        }

        public static int bitswap(int val, int B11, int B10, int B9, int B8, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0)
        {
            return ((BIT(val,B11) << 11) | (BIT(val,B10) << 10) | (BIT(val, B9) <<  9) | (BIT(val, B8) <<  8) |
                    (BIT(val, B7) <<  7) | (BIT(val, B6) <<  6) | (BIT(val, B5) <<  5) | (BIT(val, B4) <<  4) |
                    (BIT(val, B3) <<  3) | (BIT(val, B2) <<  2) | (BIT(val, B1) <<  1) | (BIT(val, B0) <<  0));
        }

        public static int bitswap(int val, int B15, int B14, int B13, int B12, int B11, int B10, int B9, int B8, int B7, int B6, int B5, int B4, int B3, int B2, int B1, int B0)
        {
            return ((BIT(val,B15) << 15) | (BIT(val,B14) << 14) | (BIT(val,B13) << 13) | (BIT(val,B12) << 12) |
                    (BIT(val,B11) << 11) | (BIT(val,B10) << 10) | (BIT(val, B9) <<  9) | (BIT(val, B8) <<  8) |
                    (BIT(val, B7) <<  7) | (BIT(val, B6) <<  6) | (BIT(val, B5) <<  5) | (BIT(val, B4) <<  4) |
                    (BIT(val, B3) <<  3) | (BIT(val, B2) <<  2) | (BIT(val, B1) <<  1) | (BIT(val, B0) <<  0));
        }

        //define BITSWAP24(val,B23,B22,B21,B20,B19,B18,B17,B16,B15,B14,B13,B12,B11,B10,B9,B8,B7,B6,B5,B4,B3,B2,B1,B0) \
        //    ((BIT(val,B23) << 23) | (BIT(val,B22) << 22) | (BIT(val,B21) << 21) | (BIT(val,B20) << 20) | \
        //        (BIT(val,B19) << 19) | (BIT(val,B18) << 18) | (BIT(val,B17) << 17) | (BIT(val,B16) << 16) | \
        //        (BIT(val,B15) << 15) | (BIT(val,B14) << 14) | (BIT(val,B13) << 13) | (BIT(val,B12) << 12) | \
        //        (BIT(val,B11) << 11) | (BIT(val,B10) << 10) | (BIT(val, B9) <<  9) | (BIT(val, B8) <<  8) | \
        //        (BIT(val, B7) <<  7) | (BIT(val, B6) <<  6) | (BIT(val, B5) <<  5) | (BIT(val, B4) <<  4) | \
        //        (BIT(val, B3) <<  3) | (BIT(val, B2) <<  2) | (BIT(val, B1) <<  1) | (BIT(val, B0) <<  0))

        //define BITSWAP32(val,B31,B30,B29,B28,B27,B26,B25,B24,B23,B22,B21,B20,B19,B18,B17,B16,B15,B14,B13,B12,B11,B10,B9,B8,B7,B6,B5,B4,B3,B2,B1,B0) \
        //    ((BIT(val,B31) << 31) | (BIT(val,B30) << 30) | (BIT(val,B29) << 29) | (BIT(val,B28) << 28) | \
        //        (BIT(val,B27) << 27) | (BIT(val,B26) << 26) | (BIT(val,B25) << 25) | (BIT(val,B24) << 24) | \
        //        (BIT(val,B23) << 23) | (BIT(val,B22) << 22) | (BIT(val,B21) << 21) | (BIT(val,B20) << 20) | \
        //        (BIT(val,B19) << 19) | (BIT(val,B18) << 18) | (BIT(val,B17) << 17) | (BIT(val,B16) << 16) | \
        //        (BIT(val,B15) << 15) | (BIT(val,B14) << 14) | (BIT(val,B13) << 13) | (BIT(val,B12) << 12) | \
        //        (BIT(val,B11) << 11) | (BIT(val,B10) << 10) | (BIT(val, B9) <<  9) | (BIT(val, B8) <<  8) | \
        //        (BIT(val, B7) <<  7) | (BIT(val, B6) <<  6) | (BIT(val, B5) <<  5) | (BIT(val, B4) <<  4) | \
        //        (BIT(val, B3) <<  3) | (BIT(val, B2) <<  2) | (BIT(val, B1) <<  1) | (BIT(val, B0) <<  0))


        // constexpr absolute value of an integer
        //template <typename T>
        //constexpr std::enable_if_t<std::is_signed<T>::value, T> iabs(T v)
        public static sbyte iabs(sbyte v) { return Math.Abs(v); }
        public static short iabs(short v) { return Math.Abs(v); }
        public static int iabs(int v) { return Math.Abs(v); }
        public static Int64 iabs(Int64 v) { return Math.Abs(v); }


        // reduce a fraction
        //template <typename M, typename N>
        public static void reduce_fraction(ref UInt32 num, ref UInt32 den)
        {
            var div = std.gcd(num, den);
            if (div != 0)
            {
                num /= div;
                den /= div;
            }
        }
    }
}
