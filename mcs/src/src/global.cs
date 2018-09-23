using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

using ListBytes = mame.ListBase<System.Byte>;
using ListBytesPointer = mame.ListPointer<System.Byte>;
using u32 = System.UInt32;


namespace mame
{
    public static class global
    {
        // corealloc_global
        public static ListBase<T> global_alloc_array<T>(UInt32 num) where T : new() { return corealloc_global.global_alloc_array<T>(num); }
        public static ListBase<T> global_alloc_array_clear<T>(UInt32 num) where T : new() { return corealloc_global.global_alloc_array_clear<T>(num); }


        // corefile_global
        public static string core_filename_extract_base(string name, bool strip_extension = false) { return util.corefile_global.core_filename_extract_base(name, strip_extension); }
        public static bool core_filename_ends_with(string filename, string extension) { return util.corefile_global.core_filename_ends_with(filename, extension); }


        // corestr_global
        public static int core_stricmp(string s1, string s2) { return corestr_global.core_stricmp(s1, s2); }
        public static int core_strwildcmp(string sp1, string sp2) { return corestr_global.core_strwildcmp(sp1, sp2); }
        public static bool core_iswildstr(string sp) { return corestr_global.core_iswildstr(sp); }


        // coretmpl_global
        public static u32 make_bitmask32(u32 N) { return coretmpl_global.make_bitmask32(N); }
        public static int BIT(int x, int n) { return coretmpl_global.BIT(x, n); }
        public static UInt32 BIT(UInt32 x, int n)  { return coretmpl_global.BIT(x, n); }
        public static sbyte iabs(sbyte v) { return coretmpl_global.iabs(v); }
        public static short iabs(short v) { return coretmpl_global.iabs(v); }
        public static int iabs(int v) { return coretmpl_global.iabs(v); }
        public static Int64 iabs(Int64 v) { return coretmpl_global.iabs(v); }
        public static void reduce_fraction(ref UInt32 num, ref UInt32 den) { coretmpl_global.reduce_fraction(ref num, ref den); }


        // emualloc_global
        public static ListBase<T> pool_alloc_array<T>(UInt32 num) where T : new() { return emualloc_global.pool_alloc_array<T>(num); }
        public static ListBase<T> pool_alloc_array_clear<T>(UInt32 num) where T : new() { return emualloc_global.pool_alloc_array_clear<T>(num); }


        // emucore_global
        public static void fatalerror(string format, params object [] args) { throw new emu_fatalerror(format, args); }
        public static void assert(bool condition) { emucore_global.assert(condition); }
        public static void assert_always(bool condition, string message) { emucore_global.assert_always(condition, message); }


        // logmacro_global
        public static void LOG(device_t device, string format, params object [] args) { logmacro_global.LOG(device, format, args); }


        // machine_global
        public static ListBase<T> auto_alloc_array<T>(running_machine m, UInt32 c) where T : new() { return machine_global.auto_alloc_array<T>(m, c); }
        public static ListBase<T> auto_alloc_array_clear<T>(running_machine m, UInt32 c) where T : new() { return machine_global.auto_alloc_array_clear<T>(m, c); }


        // osdcore_interface
        public static void osd_printf_error(string format, params object [] args) { osdcore_interface.osd_printf_error(format, args); }
        public static void osd_printf_warning(string format, params object [] args) { osdcore_interface.osd_printf_warning(format, args); }
        public static void osd_printf_info(string format, params object [] args) { osdcore_interface.osd_printf_info(format, args); }
        public static void osd_printf_verbose(string format, params object [] args) { osdcore_interface.osd_printf_verbose(format, args); }
        public static void osd_printf_debug(string format, params object [] args) { osdcore_interface.osd_printf_debug(format, args); }


        // render_global
        public static u32 PRIMFLAG_BLENDMODE(u32 x) { return render_global.PRIMFLAG_BLENDMODE(x); }
        public static u32 PRIMFLAG_TEXWRAP(u32 x) { return render_global.PRIMFLAG_TEXWRAP(x); }
        public const u32 BLENDMODE_ALPHA = (u32)BLENDMODE.BLENDMODE_ALPHA;
        public const u32 PRIMFLAG_PACKABLE = render_global.PRIMFLAG_PACKABLE;


        // c++
        public static int sizeof_(object value)
        {
            // alternative to (u32)System.Runtime.InteropServices.Marshal.SizeOf(typeof(NativeType)); ?

            if (value is sbyte)       return 1;
            else if (value is byte)   return 1;
            else if (value is Int16)  return 2;
            else if (value is UInt16) return 2;
            else if (value is Int32)  return 4;
            else if (value is UInt32) return 4;
            else if (value is Int64)  return 8;
            else if (value is UInt64) return 8;
            else if (value is Type)
            {
                if ((Type)value == typeof(sbyte))       return 1;
                else if ((Type)value == typeof(byte))   return 1;
                else if ((Type)value == typeof(Int16))  return 2;
                else if ((Type)value == typeof(UInt16)) return 2;
                else if ((Type)value == typeof(Int32))  return 4;
                else if ((Type)value == typeof(UInt32)) return 4;
                else if ((Type)value == typeof(Int64))  return 8;
                else if ((Type)value == typeof(UInt64)) return 8;
                else throw new emu_unimplemented();
            }
            else throw new emu_unimplemented();
        }


        // c++ algorithm
        public static void fill<T>(ListBase<T> destination, T value) { memset(destination, value, (UInt32)destination.Count); }


        // c++ float.h  - https://www.johndcook.com/blog/2012/01/05/double-epsilon-dbl_epsilon/
        public const double DBL_EPSILON  =   2.2204460492503131e-016; /* smallest such that 1.0+DBL_EPSILON != 1.0 */


        // c++ math.h
        public static int lround(double x) { return (int)Math.Round(x, MidpointRounding.AwayFromZero); }


        // c++ stdio.h
        public static int memcmp<T>(ListPointer<T> ptr1, ListPointer<T> ptr2, UInt32 num) { return ptr1.compare(ptr2, (int)num) ? 0 : 1; }  //  const void * ptr1, const void * ptr2, size_t num
        public static void memcpy<T>(ListBase<T> destination, ListBase<T> source, UInt32 num) { destination.copy(0, 0, source, (int)num); }  //  void * destination, const void * source, size_t num );
        public static void memcpy<T>(ListPointer<T> destination, ListPointer<T> source, UInt32 num) { destination.copy(0, 0, source, (int)num); }  //  void * destination, const void * source, size_t num );
        public static void memset<T>(ListBase<T> destination, T value) { memset(destination, value, (UInt32)destination.Count); }
        public static void memset<T>(ListBase<T> destination, T value, UInt32 num) { for (int i = 0; i < num; i++) destination[i] = value; }
        public static void memset<T>(ListPointer<T> destination, T value, UInt32 num) { for (int i = 0; i < num; i++) destination[i] = value; }
        public static void memset<T>(T [] destination, T value) { memset(destination, value, (UInt32)destination.Length); }
        public static void memset<T>(T [] destination, T value, UInt32 num) { for (int i = 0; i < num; i++) destination[i] = value; }
        public static void memset<T>(T [,] destination, T value) { for (int i = 0; i < destination.GetLength(0); i++) for (int j = 0; j < destination.GetLength(1); j++) destination[i, j] = value; }


        // c++ strformat.h
        public static string string_format(string format, params object [] args) { return string.Format(format, args); }


        // c++ string.h
        public static int strlen(string str) { return str.Length; }
        public static int strcmp(string str1, string str2) { return string.Compare(str1, str2); }
        public static int strncmp(string str1, string str2, int num) { return string.Compare(str1, 0, str2, 0, num); }


        // c++ utility
        public static void swap<T>(ref T val1, ref T val2)
        {
            global.assert(typeof(T).IsValueType);

            T temp = val1;
            val1 = val2;
            val2 = temp;
        }

        public static KeyValuePair<T, V> make_pair<T, V>(T t, V v) { return new KeyValuePair<T, V>(t, v); }
    }


    public class std_list<T> : LinkedList<T>
    {
        public std_list() : base() { }
        public std_list(IEnumerable<T> collection) : base(collection) { }
        //protected LinkedList(SerializationInfo info, StreamingContext context);


        // std::list functions
        public void clear() { Clear(); }
        public LinkedListNode<T> emplace_back(T item) { return AddLast(item); }
        public bool empty() { return Count == 0; }
        public void push_back(T item) { AddLast(item); }
        public void push_front(T item) { AddFirst(item); }
        public int size() { return Count; }
    }


    public class std_map<K, V> : Dictionary<K, V>
    {
        public std_map() : base() { }
        //public Dictionary(int capacity);
        //public Dictionary(IEqualityComparer<TKey> comparer);
        //public Dictionary(IDictionary<TKey, TValue> dictionary);
        //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
        //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
        //protected Dictionary(SerializationInfo info, StreamingContext context);


        // std::map functions
        public bool emplace(K key, V value) { if (ContainsKey(key)) { return false; } else { Add(key, value); return true; } }
        public void erase(K key) { Remove(key); }
        public V find(K key) { V value; if (TryGetValue(key, out value)) return value; else return default(V); }
    }


    public class std_multimap<K, V> : Dictionary<K, List<V>> // std::multimap<std::string, ui_software_info, ci_less> m_list;
    {
        public std_multimap() : base() { }
        //public Dictionary(int capacity);
        //public Dictionary(IEqualityComparer<TKey> comparer);
        //public Dictionary(IDictionary<TKey, TValue> dictionary);
        //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
        //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
        //protected Dictionary(SerializationInfo info, StreamingContext context);
    }


    public class std_set<T> : HashSet<T>
    {
        public std_set() : base() { }
        //public HashSet(IEqualityComparer<T> comparer);
        //public HashSet(IEnumerable<T> collection);
        //public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer);
        //protected HashSet(SerializationInfo info, StreamingContext context);


        // std::set functions
        public bool emplace(T item) { return Add(item); }
        public bool erase(T item) { return Remove(item); }
        public bool find(T item) { return Contains(item); }
        public bool insert(T item) { return Add(item); }
    }


    public class std_unordered_map<K, V> : Dictionary<K, V>
    {
        public std_unordered_map() : base() { }
        //public Dictionary(int capacity);
        //public Dictionary(IEqualityComparer<TKey> comparer);
        //public Dictionary(IDictionary<TKey, TValue> dictionary);
        //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
        //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
        //protected Dictionary(SerializationInfo info, StreamingContext context);


        // std::unordered_map functions
        public void clear() { Clear(); }
        public bool emplace(K key, V value) { if (ContainsKey(key)) { return false; } else { Add(key, value); return true; } }
        public bool empty() { return Count == 0; }
        public V find(K key) { V value; if (TryGetValue(key, out value)) return value; else return default(V); }
        public bool insert(K key, V value) { if (ContainsKey(key)) { return false; } else { Add(key, value); return true; } }
        public int size() { return Count; }
    }


    public class std_unordered_set<T> : HashSet<T>
    {
        public std_unordered_set() : base() { }
        //public HashSet(IEqualityComparer<T> comparer);
        //public HashSet(IEnumerable<T> collection);
        //public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer);
        //protected HashSet(SerializationInfo info, StreamingContext context);


        // std::unordered_set functions
        public void clear() { Clear(); }
        public bool emplace(T item) { return Add(item); }
        public bool erase(T item) { return Remove(item); }
        public bool find(T item) { return Contains(item); }
        public bool insert(T item) { return Add(item); }
    }


    // we make a re-implementation of List so that it can be interchanged with RawBuffer
    public class ListBase<T> : IEnumerable<T>//, ICollection<T>
    {
        List<T> m_list;


        public ListBase() { m_list = new List<T>(); }
        public ListBase(int capacity) { m_list = new List<T>(capacity); }
        public ListBase(IEnumerable<T> collection) { m_list = new List<T>(collection); }


        public virtual IEnumerator<T> GetEnumerator() { return m_list.GetEnumerator(); }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }


        public virtual T this[int index] { get { return m_list[index]; } set { m_list[index] = value; } }

        public virtual int Count { get { return m_list.Count; } }
        public virtual int Capacity { get { return m_list.Capacity; } set { m_list.Capacity = value; } }
        //public virtual bool IsReadOnly { get; }


        public virtual void Add(T item) { m_list.Add(item); }
        public virtual void Clear() { m_list.Clear(); }
        //public virtual bool Contains(T item) { return m_list.Contains(item); }
        //public virtual void CopyTo(T[] array, int arrayIndex) { m_list.CopyTo(array, arrayIndex); }
        public virtual void CopyTo(int index, T[] array, int arrayIndex, int count) { m_list.CopyTo(index, array, arrayIndex, count); }
        public virtual int IndexOf(T item, int index, int count) { return m_list.IndexOf(item, index, count); }
        public virtual int IndexOf(T item, int index) { return m_list.IndexOf(item, index); }
        public virtual int IndexOf(T item) { return m_list.IndexOf(item); }
        public virtual void Insert(int index, T item) { m_list.Insert(index, item); }
        //public virtual bool Remove(T item) { return m_list.Remove(item); }
        public virtual void RemoveAt(int index) { m_list.RemoveAt(index); }
        public virtual void RemoveRange(int index, int count) { m_list.RemoveRange(index, count); }
        public virtual void Sort(Comparison<T> comparison) { m_list.Sort(comparison); }
        public virtual void Sort() { m_list.Sort(); }
        public virtual void Sort(IComparer<T> comparer) { m_list.Sort(comparer); }
        public virtual T[] ToArray() { return m_list.ToArray(); }


        // UInt32 helper
        public virtual T this[u32 index] { get { return this[(int)index]; } set { this[(int)index] = value; } }


        public virtual void copy(int destStart, int srcStart, ListBase<T> src, int count)
        {
            if (this != src)
            {
                for (int i = destStart; i < destStart + count; i++)
                    this[i] = src[srcStart + i];
            }
            else
            {
                // handle overlap very inefficiently
                T [] m_temp = new T[count];
                src.CopyTo(srcStart, m_temp, 0, count);
                for (int i = 0; i < count; i++)
                    this[i + destStart] = m_temp[i];
            }
        }


        public virtual void resize(int count, T data = default(T))
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
                    Add(data);
            }
        }


        public virtual void set(T value) { set(value, 0, Count); }
        public virtual void set(T value, int count) { set(value, 0, count); }
        public virtual void set(T value, int start, int count)
        {
            for (int i = start; i < start + count; i++)
                this[i] = value;
        }
    }


    public class std_vector<T> : ListBase<T>
    {
        public std_vector() : base() { }
        // this is different behavior as List<T> so that it matches how std::vector works
        public std_vector(int capacity, T data = default(T)) : base(capacity) { resize(capacity, data); }
        public std_vector(u32 capacity, T data = default(T)) : this((int)capacity, data) { }
        public std_vector(IEnumerable<T> collection) : base(collection) { }


        // std::vector functions
        public T back() { return empty() ? default(T) : this[Count - 1]; }
        public void clear() { Clear(); }
        public void emplace(int index, T item) { Insert(index, item); }
        public void emplace_back(T item) { Add(item); }
        public bool empty() { return Count == 0; }
        public void erase(int index) { RemoveAt(index); }
        public void insert(int index, T item) { Insert(index, item); }
        public void push_back(T item) { Add(item); }
        public void push_front(T item) { Insert(0, item); }
        public void reserve(int value) { Capacity = value; }
        public int size() { return Count; }
    }


    public class ListPointer<T>
    {
        protected ListBase<T> m_list;
        protected int m_offset;


        public ListPointer() { }
        public ListPointer(ListBase<T> list, int offset = 0) { m_list = list; m_offset = offset; }
        public ListPointer(ListPointer<T> listPtr, int offset = 0) : this(listPtr.m_list, listPtr.m_offset + offset) { }


        public virtual ListBase<T> Buffer { get { return m_list; } }
        public virtual int Offset { get { return m_offset; } }
        public virtual int Count { get { return m_list.Count; } }


        public virtual T this[int i] { get { return m_list[m_offset + i]; } set { m_list[m_offset + i] = value; } }
        public virtual T this[UInt32 i] { get { return m_list[m_offset + (int)i]; } set { m_list[m_offset + (int)i] = value; } }


        public static ListPointer<T> operator +(ListPointer<T> left, int right) { left.m_offset += right; return left; }
        public static ListPointer<T> operator +(ListPointer<T> left, UInt32 right) { left.m_offset += (int)right; return left; }
        public static ListPointer<T> operator ++(ListPointer<T> left) { left.m_offset++; return left; }
        public static ListPointer<T> operator -(ListPointer<T> left, int right) { left.m_offset -= right; return left; }
        public static ListPointer<T> operator -(ListPointer<T> left, UInt32 right) { left.m_offset -= (int)right; return left; }
        public static ListPointer<T> operator --(ListPointer<T> left) { left.m_offset--; return left; }


        public virtual bool compare(ListPointer<T> right, int count)
        {
            for (int i = 0; i < count; i++)
            {
                if (!this[i].Equals(right[i]))
                    return false;
            }
            return true;
        }

        public virtual void copy(int destStart, int srcStart, ListPointer<T> src, int count) { m_list.copy(m_offset + destStart, src.m_offset + srcStart, src.m_list, count); }

        public virtual void set(T value, int count) { set(value, 0, count); }
        public virtual void set(T value, int start, int count) { m_list.set(value, m_offset + start, count); }
    }


    public class ListBytesPointerRef
    {
        public ListBytesPointer m_listPtr;

        public ListBytesPointerRef() { }
        public ListBytesPointerRef(ListBytesPointer listPtr) { m_listPtr = listPtr; }
    }


    public class RawBufferPointer : ListBytesPointer
    {
        public RawBufferPointer() : base() { }
        public RawBufferPointer(RawBuffer list, int offset = 0) : base(list, offset) { }
        public RawBufferPointer(RawBufferPointer listPtr, int offset = 0) : base(listPtr, offset) { }

        public new byte this[int i] { get { return get_uint8(i); } set { set_uint8(i, value); } }
        public new byte this[UInt32 i] { get { return get_uint8((int)i); } set { set_uint8((int)i, value); } }

        public new RawBuffer Buffer { get { return (RawBuffer)m_list; } }

        public static RawBufferPointer operator +(RawBufferPointer left, int right) { left.m_offset += right; return left; }
        public static RawBufferPointer operator +(RawBufferPointer left, UInt32 right) { left.m_offset += (int)right; return left; }
        public static RawBufferPointer operator ++(RawBufferPointer left) { left.m_offset++; return left; }
        public static RawBufferPointer operator -(RawBufferPointer left, int right) { left.m_offset -= right; return left; }
        public static RawBufferPointer operator -(RawBufferPointer left, UInt32 right) { left.m_offset -= (int)right; return left; }
        public static RawBufferPointer operator --(RawBufferPointer left) { left.m_offset--; return left; }

        public void set_uint8(byte value) { set_uint8(0, value); }
        public void set_uint16(UInt16 value) { set_uint16(0, value); }
        public void set_uint32(UInt32 value) { set_uint32(0, value); }
        public void set_uint64(UInt64 value) { set_uint64(0, value); }
        public void set_uint8(int offset8, byte value) { ((RawBuffer)m_list).set_uint8(m_offset + offset8, value); }
        public void set_uint16(int offset16, UInt16 value) { ((RawBuffer)m_list).set_uint16(m_offset + offset16, value); }
        public void set_uint32(int offset32, UInt32 value) { ((RawBuffer)m_list).set_uint32(m_offset + offset32, value); }
        public void set_uint64(int offset64, UInt64 value) { ((RawBuffer)m_list).set_uint64(m_offset + offset64, value); }

        public byte get_uint8(int offset8 = 0) { return ((RawBuffer)m_list).get_uint8(m_offset + offset8); }
        public UInt16 get_uint16(int offset16 = 0) { return ((RawBuffer)m_list).get_uint16(m_offset + offset16); }
        public UInt32 get_uint32(int offset32 = 0) { return ((RawBuffer)m_list).get_uint32(m_offset + offset32); }
        public UInt64 get_uint64(int offset64 = 0) { return ((RawBuffer)m_list).get_uint64(m_offset + offset64); }

        public bool equals(string compareTo) { return equals(0, compareTo); }
        public bool equals(int startOffset, string compareTo) { return equals(startOffset, compareTo.ToCharArray()); }
        public bool equals(int startOffset, char [] compareTo) { return ((RawBuffer)m_list).equals(startOffset, compareTo); }

        public string get_string(int length) { return get_string(length); }
        public string get_string(int startOffset, int length) { return ((RawBuffer)m_list).get_string(startOffset, length); }
    }


    public class RawBuffer : ListBytes
    {
        [StructLayout(LayoutKind.Explicit)]
        struct RawBufferData
        {
            [FieldOffset(0)]
            public byte[] m_byte;

            [FieldOffset(0)]
            public UInt16[] m_uint16;

            [FieldOffset(0)]
            public UInt32[] m_uint32;

            [FieldOffset(0)]
            public UInt64[] m_uint64;
        }

        RawBufferData m_bufferData = new RawBufferData();
        int m_actualLength = 0;


        public RawBuffer() { m_bufferData.m_byte = new byte [0]; }
        public RawBuffer(int size) : this() { resize(size); }
        public RawBuffer(UInt32 size) : this((int)size) { }


        public override IEnumerator<byte> GetEnumerator() { throw new emu_unimplemented(); }


        public override byte this[int index] { get { return get_uint8(index); } set { set_uint8(index, value); } }

        public override int Count { get { return m_actualLength; } }
        public override int Capacity { get { return m_bufferData.m_byte.Length; } set { } }


        public override void Add(byte item)
        {
            if (Capacity <= Count + 1)
            {
                int newSize = Math.Min(Count + 1024, (Count + 1) * 2);  // cap the growth
                Array.Resize(ref m_bufferData.m_byte, newSize);
            }

            m_bufferData.m_byte[m_actualLength] = item;
            m_actualLength++;
        }

        public override void Clear() { m_bufferData.m_byte = new byte [0];  m_actualLength = 0; }

        //public override bool Contains(T item) { return m_list.Contains(item); }

        //public override void CopyTo(T[] array, int arrayIndex) { m_list.CopyTo(array, arrayIndex); }

        public override void CopyTo(int index, byte[] array, int arrayIndex, int count)
        {
            Array.Copy(m_bufferData.m_byte, index, array, arrayIndex, count);
        }

        public override int IndexOf(byte item, int index, int count) { throw new emu_unimplemented(); }
        public override int IndexOf(byte item, int index) { throw new emu_unimplemented(); }
        public override int IndexOf(byte item) { throw new emu_unimplemented(); }

        public override void Insert(int index, byte item) { throw new emu_unimplemented(); }

        //public override bool Remove(T item) { return m_list.Remove(item); }

        public override void RemoveAt(int index)
        {
            if (index > 0)
                Array.Copy(m_bufferData.m_byte, 0, m_bufferData.m_byte, 0, index);

            if (index < Count - 1)
                Array.Copy(m_bufferData.m_byte, index + 1, m_bufferData.m_byte, index, Count - index - 1);

            m_actualLength--;
        }

        public override void RemoveRange(int index, int count)
        {
            // horribly inefficient in this case
            while (count-- > 0)
                RemoveAt(index);
        }

        public override void Sort(Comparison<byte> comparison) { throw new emu_unimplemented(); }
        public override void Sort() { throw new emu_unimplemented(); }
        public override void Sort(IComparer<byte> comparer) { throw new emu_unimplemented(); }

        public override byte[] ToArray() { throw new emu_unimplemented(); }


        // UInt32 helper
        public override byte this[u32 index] { get { return get_uint8((int)index); } set { set_uint8((int)index, value); } }


        public bool equals(int startOffset, string compareTo) { return equals(startOffset, compareTo.ToCharArray()); }
        public bool equals(int startOffset, char [] compareTo)
        {
            for (int i = 0; i < compareTo.Length; i++)
            {
                if (this[i] != compareTo[i])
                    return false;
            }
            return true;
        }

        public string get_string(int startOffset, int length)
        {
            string s = "";
            for (int i = startOffset; i < startOffset + length; i++)
            {
                s += this[i];
            }

            return s;
        }


        public int find(int startOffset, int endOffset, byte compare)
        {
            for (int i = startOffset; i < startOffset + endOffset; i++)
            {
                if (this[i] == compare)
                    return i;
            }

            return endOffset;
        }

        public void set_uint8(int offset8, byte value)
        {
            global.assert(offset8 < Count);

            m_bufferData.m_byte[offset8] = value;
        }

        public void set_uint16(int offset16, UInt16 value)
        {
            global.assert(offset16 * 2 < Count);

            //this[offset16 * 2]     = (byte)(value >> 8);
            //this[offset16 * 2 + 1] = (byte)value;
            m_bufferData.m_uint16[offset16] = value;
        }

        public void set_uint32(int offset32, UInt32 value)
        {
            global.assert(offset32 * 4 < Count);

            //this[offset32 * 4]     = (byte)(value >> 24);
            //this[offset32 * 4 + 1] = (byte)(value >> 16);
            //this[offset32 * 4 + 2] = (byte)(value >>  8);
            //this[offset32 * 4 + 3] = (byte)value;
            m_bufferData.m_uint32[offset32] = value;
        }

        public void set_uint64(int offset64, UInt64 value)
        {
            global.assert(offset64 * 8 < Count);

            //this[offset64 * 8]     = (byte)(value >> 56);
            //this[offset64 * 8 + 1] = (byte)(value >> 48);
            //this[offset64 * 8 + 2] = (byte)(value >> 40);
            //this[offset64 * 8 + 3] = (byte)(value >> 32);
            //this[offset64 * 8 + 4] = (byte)(value >> 24);
            //this[offset64 * 8 + 5] = (byte)(value >> 16);
            //this[offset64 * 8 + 6] = (byte)(value >>  8);
            //this[offset64 * 8 + 7] = (byte)value;
            m_bufferData.m_uint64[offset64] = value;
        }

        public byte get_uint8(int offset8 = 0)
        {
            global.assert(offset8 < Count);

            return m_bufferData.m_byte[offset8];
        }

        public UInt16 get_uint16(int offset16 = 0)
        {
            global.assert(offset16 * 2 < Count);

            //return (UInt16)(this[offset16 * 2] << 8 | 
            //       (UInt16) this[offset16 * 2 + 1]);
            return m_bufferData.m_uint16[offset16];
        }

        public UInt32 get_uint32(int offset32 = 0)
        {
            global.assert(offset32 * 4 < Count);

            //return (UInt32)this[offset32 * 4]     << 24 | 
            //       (UInt32)this[offset32 * 4 + 1] << 16 | 
            //       (UInt32)this[offset32 * 4 + 2] <<  8 | 
            //       (UInt32)this[offset32 * 4 + 3]; 
            return m_bufferData.m_uint32[offset32];
        }

        public UInt64 get_uint64(int offset64 = 0)
        {
            global.assert(offset64 * 8 < Count);

            //return (UInt64)this[offset64 * 8]     << 56 | 
            //       (UInt64)this[offset64 * 8 + 1] << 48 | 
            //       (UInt64)this[offset64 * 8 + 2] << 40 | 
            //       (UInt64)this[offset64 * 8 + 3] << 32 | 
            //       (UInt64)this[offset64 * 8 + 4] << 24 | 
            //       (UInt64)this[offset64 * 8 + 5] << 16 | 
            //       (UInt64)this[offset64 * 8 + 6] <<  8 | 
            //       (UInt64)this[offset64 * 8 + 7]; 
            return m_bufferData.m_uint64[offset64];
        }
    }


    // manual boxing class of an int
    public class intref
    {
        int m_value;

        public intref() { }
        public intref(int i) { m_value = i; }

        public int i { get { return m_value; } set { m_value = value; } }

        // these might cause trouble, by replacing the ref, which is what we want to avoid
        //public static implicit operator int(intref x) { return x.get(); }
        //public static implicit operator intref(int x) { return new intref(x); }
    }


    // manual boxing class of a double
    public class doubleref
    {
        double m_value;

        public doubleref() { }
        public doubleref(double value) { m_value = value; }

        public double d { get { return m_value; } set { m_value = value; } }

        // these might cause trouble, by replacing the ref, which is what we want to avoid
        //public static implicit operator int(intref x) { return x.get(); }
        //public static implicit operator intref(int x) { return new intref(x); }
    }
}
