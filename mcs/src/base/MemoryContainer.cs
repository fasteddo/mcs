// license:BSD-3-Clause
// copyright-holders:Edward Fast

//#define ASSERT_SLOW

using System;
using System.Buffers.Binary;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;


namespace mame
{
    // this wraps a Memory<T> struct so that bulk operations can be done efficiently
    // std.vector and MemoryU8 derive from this.
    public class MemoryContainer<T> : IList<T>, IReadOnlyList<T>
    {
        class MemoryContainerEnumerator : IEnumerator<T>
        {
            MemoryContainer<T> m_list;
            int m_index;
            int m_endIndex;
 
            public MemoryContainerEnumerator(MemoryContainer<T> list)
            {
                m_list = list;
                m_index = -1;
                m_endIndex = list.Count;
            }
 
            public void Dispose() { }
            public bool MoveNext() { if (m_index < m_endIndex) { m_index++; return m_index < m_endIndex; } return false; }
            object IEnumerator.Current { get { return null; } }
            public T Current
            {
                get
                {
                    if (m_index < 0) throw new InvalidOperationException(string.Format("ListBaseEnumerator() - {0}", m_index));
                    if (m_index >= m_endIndex) throw new InvalidOperationException(string.Format("ListBaseEnumerator() - {0}", m_index));
                    return m_list[m_index];
                }
            }
            public void Reset() { m_index = -1; }
        }


        T [] m_data;
        Memory<T> m_memory;
        int m_actualLength = 0;


        public MemoryContainer() : this(0) { }
        public MemoryContainer(int capacity, bool allocate = false) { m_data = new T [capacity]; m_memory = new Memory<T>(m_data); if (allocate) Resize(capacity); }
        public MemoryContainer(IEnumerable<T> collection) : this() { foreach (var item in collection) Add(item); }
        public MemoryContainer(T [] collection) : this() { SetInternal(collection); }


        // IList

        public virtual T this[int index] { get { return m_data[index]; } set { m_data[index] = value; } }

        public virtual int IndexOf(T item) { return Array.IndexOf(m_data, item, 0, Count); }
        public virtual void Insert(int index, T item)
        {
            var newData = m_data;
            var newMemory = m_memory;
            if (Count + 1 > Capacity)
            {
                int newSize = GetNextCapacitySize(Count);
                newData = new T [newSize];
                newMemory = new Memory<T>(newData);
                if (Capacity > 0)
                    m_memory[..Math.Min(index + 1, Count)].CopyTo(newMemory[..Math.Min(index + 1, Count)]);  // copy everything before index
            }
            if (Capacity > 0 && index < Count)
                m_memory[index..Count].CopyTo(newMemory[(index + 1)..]);  // copy everything after index
            newData[index] = item;
            m_data = newData;
            m_memory = newMemory;
            m_actualLength++;
        }
        public virtual void RemoveAt(int index) { RemoveRange(index, 1); }
        public virtual void Add(T item)
        {
            if (Count + 1 > Capacity)
                Capacity = GetNextCapacitySize(Count);

            m_data[m_actualLength] = item;
            m_actualLength++;
        }
        public virtual void Clear()
        {
            m_data = Array.Empty<T>();
            m_memory = new Memory<T>(m_data);
            m_actualLength = 0;
        }
        public virtual bool Contains(T item) { return IndexOf(item) != -1; }
        public virtual void CopyTo(T[] array, int arrayIndex) { CopyTo(0, array, arrayIndex, Count); }
        public virtual bool Remove(T item)
        {
            var index = IndexOf(item);
            if (index == -1)
                return false;

            RemoveAt(index);
            return true;
        }
        public virtual int Count { get { return m_actualLength; } }
        bool ICollection<T>.IsReadOnly { get { return ((ICollection<T>)m_data).IsReadOnly; } }
        public virtual IEnumerator<T> GetEnumerator() { return new MemoryContainerEnumerator(this); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }


        // List

        public virtual int Capacity
        {
            get { return m_data.Length; }
            set
            {
                if (value < Count)
                    throw new ArgumentOutOfRangeException();

                int newSize = value;
                var newData = new T [newSize];
                var newMemory = new Memory<T>(newData);
                m_memory.CopyTo(newMemory);
                m_data = newData;
                m_memory = newMemory;
            }
        }
        public virtual void CopyTo(int index, T[] array, int arrayIndex, int count) { CopyTo(index, new Span<T>(array, arrayIndex, count), count); }
        public virtual int FindIndex(int startIndex, int count, Predicate<T> match) { return Array.FindIndex(m_data, startIndex, count, match); }
        public virtual int FindIndex(int startIndex, Predicate<T> match) { return Array.FindIndex(m_data, startIndex, Count - startIndex, match); }
        public virtual int FindIndex(Predicate<T> match) { return Array.FindIndex(m_data, 0, Count, match); }
        public virtual int RemoveAll(Predicate<T> match)
        {
            int count = 0;
            int index = Array.FindIndex(m_data, 0, Count, match);
            while (index != -1)
            {
                RemoveAt(index);
                count++;
                index = Array.FindIndex(m_data, 0, Count, match);
            }
            return count;
        }
        public virtual void RemoveRange(int index, int count)
        {
            if (index + count < Count)
                m_memory[(index + count)..].CopyTo(m_memory[index..]);

            m_actualLength -= count;
        }
        public virtual void Sort(Comparison<T> comparison) { Array.Sort(m_data, 0, Count, Comparer<T>.Create(comparison)); }
        public virtual void Sort() { Array.Sort(m_data, 0, Count); }
        public virtual void Sort(IComparer<T> comparer) { Array.Sort(m_data, 0, Count, comparer); }
        public virtual T[] ToArray() { T [] newData = new T [Count]; Array.Copy(m_data, newData, Count); return newData; }


        public T [] data_raw { get { return m_data; } }
        public Memory<T> memory { get { return m_memory; } }


        // UInt64 helper
        public virtual T this[UInt64 index] { get { return m_data[index]; } set { m_data[index] = value; } }


        public bool MemoryEquals(MemoryContainer<T> other) { return m_data == other.m_data; }


        public virtual void AddRange(IEnumerable<T> collection)
        {
            foreach (var item in collection)
                Add(item);
        }


        public virtual bool CompareTo(MemoryContainer<T> right, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!this[i].Equals(right[i]))
                    return false;
            }
            return true;
        }


        public virtual void CopyTo(int srcStart, MemoryContainer<T> dest, int destStart, int count)
        {
            CopyTo(srcStart, dest.m_memory.Slice(destStart, count).Span, count);
        }


        public virtual void CopyTo(int srcStart, Span<T> dest, int count)
        {
            m_memory.Slice(srcStart, count).Span.CopyTo(dest);
        }


        public virtual void Resize(int count) { ResizeInternal(count, (T)default); }
        public virtual void Resize(int count, T data)
        {
            Debug.Assert(typeof(T).GetTypeInfo().IsValueType ? true : (data == null || data.Equals(default)) ? true : false);  // this function doesn't do what you'd expect for ref classes since it doesn't new() for each item.  Manually Add() in this case.
            ResizeInternal(count, data);
        }
        public virtual void Resize(int count, Func<T> creator)
        {
            ResizeInternal(count, creator);
        }


        protected virtual void ResizeInternal(int count, T data)
        {
            int current = Count;
            if (count < current)
            {
                RemoveRange(count, current - count);
            }
            else if (count > current)
            {
                if (count > Capacity)
                    Capacity = count;

                // Short cut for not calling Add() for each element.
                // If Fill() adds checks for Count, then we will need to call a FillInternal() instead.
                Fill(data, Count, count - current);
                m_actualLength = count;
            }
        }

        protected virtual void ResizeInternal(int count, Func<T> creator)
        {
            int current = Count;
            if (count < current)
            {
                RemoveRange(count, current - count);
            }
            else if (count > current)
            {
                if (count > Capacity)
                    Capacity = count;

                for (int i = 0; i < count - current; i++)
                    Add(creator());
            }
        }


        protected virtual void SetInternal(T [] collection)
        {
            m_data = collection;
            m_memory = new Memory<T>(m_data);
            m_actualLength = m_data.Length;
        }


        public virtual void Fill(T value) { Fill(value, 0, Count); }
        public virtual void Fill(T value, int count) { Fill(value, 0, count); }
        public virtual void Fill(T value, int start, int count)
        {
            var valueType = typeof(T).GetTypeInfo().IsValueType ? true : ((value == null || value.Equals(default)) ? true : false);
            if (valueType)
            {
                m_memory.Slice(start, count).Span.Fill(value);
            }
            else
            {
                for (int i = start; i < start + count; i++)
                    this[i] = value;
            }
        }
        public virtual void Fill(Func<T> creator)
        {
            for (int i = 0; i < Count; i++)
                this[i] = creator();
        }


        static int GetNextCapacitySize(int current) { return Math.Min(current + 1024, (current + 1) * 2); }  // cap the growth
    }


    // the usage of this should be when you have a flat piece of memory and intend to work on it with different types
    // eg, a byte array that you cast to UInt32 and do operations.
    // See PointerU8, PointerU16, etc
    public static class MemoryU8Extension
    {
        [Conditional("ASSERT_SLOW")] static void assert_slow(bool condition) { Debug.Assert(condition); }

        public static bool CompareTo(this MemoryContainer<byte> container, int startOffset, string compareTo) { return container.CompareTo(startOffset, compareTo.ToCharArray()); }
        public static bool CompareTo(this MemoryContainer<byte> container, int startOffset, char [] compareTo)
        {
            for (int i = 0; i < compareTo.Length; i++)
            {
                if (container[i] != compareTo[i])
                    return false;
            }
            return true;
        }

        public static string ToString(this MemoryContainer<byte> container, int startOffset, int length)
        {
            string s = "";
            for (int i = startOffset; i < startOffset + length; i++)
            {
                s += container[i];
            }

            return s;
        }


        public static int IndexOf(this MemoryContainer<byte> container, int startOffset, int endOffset, byte compare)
        {
            for (int i = startOffset; i < startOffset + endOffset; i++)
            {
                if (container[i] == compare)
                    return i;
            }

            return endOffset;
        }


        public static void SetUInt16(this MemoryContainer<byte> container, int offset16, UInt16 value) { container.SetUInt16Offs8(offset16 << 1, value); }
        public static void SetUInt32(this MemoryContainer<byte> container, int offset32, UInt32 value) { container.SetUInt32Offs8(offset32 << 2, value); }
        public static void SetUInt64(this MemoryContainer<byte> container, int offset64, UInt64 value) { container.SetUInt64Offs8(offset64 << 3, value); }

        public static void SetUInt16Offs8(this MemoryContainer<byte> container, int offset8, UInt16 value)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 2).Span;
            BinaryPrimitives.WriteUInt16LittleEndian(span, value);
#else
            var bytes = BitConverter.GetBytes(value);
#if MEMORY_BYTE_USE_ARRAY_COPY
            Array.Copy(bytes, 0, container.data, offset8, 2);
#else
            container.data_raw[offset8 + 1] = bytes[1];
            container.data_raw[offset8]     = bytes[0];
#endif
#endif
        }

        public static void SetUInt32Offs8(this MemoryContainer<byte> container, int offset8, UInt32 value)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 4).Span;
            BinaryPrimitives.WriteUInt32LittleEndian(span, value);
#else
            var bytes = BitConverter.GetBytes(value);
#if MEMORY_BYTE_USE_ARRAY_COPY
            Array.Copy(bytes, 0, container.data, offset8, 4);
#else
            container.data_raw[offset8 + 3] = bytes[3];
            container.data_raw[offset8 + 2] = bytes[2];
            container.data_raw[offset8 + 1] = bytes[1];
            container.data_raw[offset8] = bytes[0];
#endif
#endif
        }

        public static void SetUInt64Offs8(this MemoryContainer<byte> container, int offset8, UInt64 value)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 8).Span;
            BinaryPrimitives.WriteUInt64LittleEndian(span, value);
#else
            var bytes = BitConverter.GetBytes(value);
#if MEMORY_BYTE_USE_ARRAY_COPY
            Array.Copy(bytes, 0, container.data, offset8, 8);
#else
            container.data_raw[offset8 + 7] = bytes[7];
            container.data_raw[offset8 + 6] = bytes[6];
            container.data_raw[offset8 + 5] = bytes[5];
            container.data_raw[offset8 + 4] = bytes[4];
            container.data_raw[offset8 + 3] = bytes[3];
            container.data_raw[offset8 + 2] = bytes[2];
            container.data_raw[offset8 + 1] = bytes[1];
            container.data_raw[offset8]     = bytes[0];
#endif
#endif
        }

        public static UInt16 GetUInt16(this MemoryContainer<byte> container, int offset16 = 0) { return container.GetUInt16Offs8(offset16 << 1); }
        public static UInt32 GetUInt32(this MemoryContainer<byte> container, int offset32 = 0) { return container.GetUInt32Offs8(offset32 << 2); }
        public static UInt64 GetUInt64(this MemoryContainer<byte> container, int offset64 = 0) { return container.GetUInt64Offs8(offset64 << 3); }

        public static UInt16 GetUInt16Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 2).Span;
            return BinaryPrimitives.ReadUInt16LittleEndian(span);
#else
            return BitConverter.ToUInt16(container.data_raw, offset8);
#endif
        }

        public static UInt32 GetUInt32Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 4).Span;
            return BinaryPrimitives.ReadUInt32LittleEndian(span);
#else
            return BitConverter.ToUInt32(container.data_raw, offset8);
#endif
        }

        public static UInt64 GetUInt64Offs8(this MemoryContainer<byte> container, int offset8 = 0)
        {
            assert_slow(offset8 < container.Count);
#if MEMORY_BYTE_USE_BINARY_PRIMITIVES
            var span = container.memory.Slice(offset8, 8).Span;
            return BinaryPrimitives.ReadUInt64LittleEndian(span);
#else
            return BitConverter.ToUInt64(container.data_raw, offset8);
#endif
        }
    }
}
