// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using s32 = System.Int32;
using size_t = System.UInt64;
using std_time_t = System.Int64;
using u8 = System.Byte;
using unsigned = System.UInt32;


namespace mame
{
    public static class std
    {
        public static size_t size(string x) { return (size_t)x.Length; }
        public static size_t size<T>(T [] x) { return (size_t)x.Length; }
        public static size_t size<T>(MemoryContainer<T> x) { return (size_t)x.Count; }


        // c++ chrono
        public static class chrono
        {
            public static class system_clock
            {
                public static DateTimeOffset now() { return DateTimeOffset.Now; }
                public static DateTimeOffset from_time_t(std_time_t t) { return DateTimeOffset.FromUnixTimeSeconds(t); }
                public static std_time_t to_time_t(DateTimeOffset t) { return t.ToUnixTimeSeconds(); }
            }

            public static TimeSpan hours(int hours) { return new TimeSpan(hours, 0, 0); }
        }


        // c++ algorithm
        public static int clamp(int v, int lo, int hi) { return Math.Clamp(v, lo, hi); }
        public static float clamp(float v, float lo, float hi) { return Math.Clamp(v, lo, hi); }
        public static double clamp(double v, double lo, double hi) { return Math.Clamp(v, lo, hi); }
        public static void fill<T>(MemoryContainer<T> destination, T value) { std.memset(destination, value); }
        public static void fill<T>(IList<T> destination, T value) { std.memset(destination, value); }
        public static void fill<T>(IList<T> destination, Func<T> value) { std.memset(destination, value); }
        public static void fill<T>(T [,] destination, T value) { std.memset(destination, value); }
        public static void fill_n<T>(MemoryContainer<T> destination, size_t count, T value) { std.memset(destination, value, count); }
        public static void fill_n<T>(Pointer<T> destination, size_t count, T value) { std.memset(destination, value, count); }
        public static void fill_n(PointerU16 destination, size_t count, UInt16 value) { std.memset(destination, value, count); }
        public static void fill_n(PointerU32 destination, size_t count, UInt32 value) { std.memset(destination, value, count); }
        public static void fill_n(PointerU64 destination, size_t count, UInt64 value) { std.memset(destination, value, count); }
        public static void fill_n<T>(IList<T> destination, size_t count, T value) { std.memset(destination, value, count); }
        public static T find_if<T>(IEnumerable<T> list, Func<T, bool> pred) { foreach (var item in list) { if (pred(item)) return item; } return default;  }
        public static int lower_bound<C, V>(C collection, V value) where C : IList<V> where V : IComparable<V> { return lower_bound<C, V, V>(collection, value, (i, v) => { return i.CompareTo(v) < 0; }); }
        public static int lower_bound<C, I, V>(C collection, V value, Func<I, V, bool> func)
            where C : IList<I>
        {
            int first = 0;
            int count = collection.Count;

            if (count == 0)
                return -1;

            while (0 < count)
            {
                int count2 = count / 2;
                int mid = first + count2;
                if (func(collection[mid], value))
                {
                    first = mid + 1;
                    count -= count2 + 1;
                }
                else
                {
                    count = count2;
                }
            }

            return first;
        }
        // std::min/max behaves differently than Math.Min/Max with respect to NaN
        // https://docs.microsoft.com/en-us/dotnet/api/system.math.min?view=net-5.0#System_Math_Min_System_Single_System_Single_
        // https://stackoverflow.com/a/39919244
        public static u8 max(u8 a, u8 b) { return Math.Max(a, b); }
        public static int max(int a, int b) { return Math.Max(a, b); }
        public static UInt32 max(UInt32 a, UInt32 b) { return Math.Max(a, b); }
        public static Int64 max(Int64 a, Int64 b) { return Math.Max(a, b); }
        public static UInt64 max(UInt64 a, UInt64 b) { return Math.Max(a, b); }
        public static float max(float a, float b) { if (float.IsNaN(a) || float.IsNaN(b)) return a; else return Math.Max(a, b); }
        public static double max(double a, double b) { if (double.IsNaN(a) || double.IsNaN(b)) return a; else return Math.Max(a, b); }
        public static u8 min(u8 a, u8 b) { return Math.Min(a, b); }
        public static int min(int a, int b) { return Math.Min(a, b); }
        public static UInt32 min(UInt32 a, UInt32 b) { return Math.Min(a, b); }
        public static Int64 min(Int64 a, Int64 b) { return Math.Min(a, b); }
        public static UInt64 min(UInt64 a, UInt64 b) { return Math.Min(a, b); }
        public static float min(float a, float b) { if (float.IsNaN(a) || float.IsNaN(b)) return a; else return Math.Min(a, b); }
        public static double min(double a, double b) { if (double.IsNaN(a) || double.IsNaN(b)) return a; else return Math.Min(a, b); }
        public static void sort<T>(MemoryContainer<T> list, Comparison<T> pred) { list.Sort(pred); }


        // c++ cmath
        public static int abs(int arg) { return Math.Abs(arg); }
        public static float abs(float arg) { return Math.Abs(arg); }
        public static double abs(double arg) { return Math.Abs(arg); }
        public static float atan(float arg) { return (float)Math.Atan(arg); }
        public static double atan(double arg) { return Math.Atan(arg); }
        public static float ceil(float x) { return (float)Math.Ceiling(x); }
        public static double ceil(double x) { return Math.Ceiling(x); }
        public static float cos(float arg) { return (float)Math.Cos(arg); }
        public static double cos(double arg) { return Math.Cos(arg); }
        public static float exp(float x) { return (float)Math.Exp(x); }
        public static double exp(double x) { return Math.Exp(x); }
        public static float fabs(float arg) { return Math.Abs(arg); }
        public static double fabs(double arg) { return Math.Abs(arg); }
        public static float fabsf(float arg) { return Math.Abs(arg); }
        public static float floor(float arg) { return (float)Math.Floor(arg); }
        public static double floor(double arg) { return Math.Floor(arg); }
        public static float floorf(float arg) { return (float)Math.Floor(arg); }
        public static bool isnan(float arg) { return Single.IsNaN(arg); }
        public static bool isnan(double arg) { return Double.IsNaN(arg); }
        public static double fmod(double numer, double denom) { return numer % denom; }
        public static float log(float arg) { return (float)Math.Log(arg); }
        public static double log(double arg) { return Math.Log(arg); }
        public static double log10(double arg) { return Math.Log10(arg); }
        public static double log1p(double arg) { return Math.Abs(arg) > 1e-4 ? Math.Log(1.0 + arg) : (-0.5 * arg + 1.0) * arg; }  //https://stackoverflow.com/a/50012422
        public static float pow(float base_, float exponent) { return (float)Math.Pow(base_, exponent); }
        public static double pow(double base_, double exponent) { return Math.Pow(base_, exponent); }
        public static int lround(double x) { return (int)Math.Round(x, MidpointRounding.AwayFromZero); }
        public static float sin(float arg) { return (float)Math.Sin(arg); }
        public static double sin(double arg) { return Math.Sin(arg); }
        public static float sqrt(float arg) { return (float)Math.Sqrt(arg); }
        public static double sqrt(double arg) { return Math.Sqrt(arg); }
        public static float tan(float arg) { return (float)Math.Tan(arg); }
        public static double tan(double arg) { return Math.Tan(arg); }
        public static float trunc(float arg) { return (float)Math.Truncate(arg); }
        public static double trunc(double arg) { return Math.Truncate(arg); }


        // c++ cstdlib
        public static void abort() { terminate(); }
        public static int atoi(string str) { return Convert.ToInt32(str); }
        public static string getenv(string env_var) { return System.Environment.GetEnvironmentVariable(env_var); }
        public static UInt64 strtoul(string str, string endptr, int base_) { return Convert.ToUInt64(str, 16); }


        // c++ cstring
        public static int memcmp<T>(MemoryContainer<T> ptr1, MemoryContainer<T> ptr2, size_t num) { return ptr1.CompareTo(ptr2, (int)num) ? 0 : 1; }  //  const void * ptr1, const void * ptr2, size_t num
        public static int memcmp<T>(Pointer<T> ptr1, Pointer<T> ptr2, size_t num) { return ptr1.CompareTo(ptr2, (int)num) ? 0 : 1; }  //  const void * ptr1, const void * ptr2, size_t num
        public static void memcpy<T>(MemoryContainer<T> destination, MemoryContainer<T> source, size_t num) { source.CopyTo(0, destination, 0, (int)num); }  //void * destination, const void * source, size_t num );
        public static void memcpy<T>(Pointer<T> destination, Pointer<T> source, size_t num) { source.CopyTo(0, destination, 0, (int)num); }  //void * destination, const void * source, size_t num );
        public static void memset<T>(MemoryContainer<T> destination, T value) { destination.Fill(value); }
        public static void memset<T>(MemoryContainer<T> destination, T value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset<T>(Pointer<T> destination, T value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU16 destination, UInt16 value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU32 destination, UInt32 value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset(PointerU64 destination, UInt64 value, size_t num) { destination.Fill(value, (int)num); }
        public static void memset<T>(IList<T> destination, T value) { memset(destination, value, (size_t)destination.Count); }
        public static void memset<T>(IList<T> destination, T value, size_t num) { for (int i = 0; i < (int)num; i++) destination[i] = value; }
        public static void memset<T>(IList<T> destination, Func<T> value) { memset(destination, value, (size_t)destination.Count); }
        public static void memset<T>(IList<T> destination, Func<T> value, size_t num) { for (int i = 0; i < (int)num; i++) destination[i] = value(); }
        public static void memset<T>(T [,] destination, T value) { for (int i = 0; i < destination.GetLength(0); i++) for (int j = 0; j < destination.GetLength(1); j++) destination[i, j] = value; }
        public static int strchr(string str, char character) { return str.IndexOf(character); }
        public static int strcmp(string str1, string str2) { return string.Compare(str1, str2); }
        public static size_t strlen(string str) { return (size_t)str.Length; }
        public static int strncmp(string str1, string str2, size_t num) { return string.Compare(str1, 0, str2, 0, (int)num); }
        public static int strrchr(string str, char character) { return str.LastIndexOf(character); }
        public static size_t strspn(string str1, string str2)
        {
            size_t count = 0;
            foreach (var c in str1)
            {
                if (!str2.Contains(c))
                    break;
                count++;
            }
            return count;
        }
        public static int strstr(string str1, string str2) { return str1.IndexOf(str2); }
        public static string to_string(int val) { return val.ToString(); }
        public static string to_string(UInt32 val) { return val.ToString(); }
        public static string to_string(Int64 val) { return val.ToString(); }
        public static string to_string(UInt64 val) { return val.ToString(); }
        public static string to_string(float val) { return val.ToString(); }
        public static string to_string(double val) { return val.ToString(); }


        // c++ exception
        public static void terminate() { throw new mcs_fatal("std.terminate() called"); }


        // c++ iostream
        public static void cerr(string s) { Debug.WriteLine(s); }


        // c++ numeric
        public static UInt32 gcd(UInt32 a, UInt32 b)
        {
            return b != 0 ? gcd(b, a % b) : a;
        }


        // c++ utility
        public static void swap<T>(ref T val1, ref T val2)
        {
            Debug.Assert(typeof(T).GetTypeInfo().IsValueType);

            (val1, val2) = (val2, val1);
        }

        public static std.pair<T, V> make_pair<T, V>(T t, V v) { return new std.pair<T, V>(t, v); }


        // c++ array
        public class array<T, size_t_N> : IList<T>
            where size_t_N : u64_const, new()
        {
            protected static readonly size_t N = new size_t_N().value;


            MemoryContainer<T> m_data = new MemoryContainer<T>((int)N, true);


            public array() { }
            public array(params T [] args)
            {
                if (args.Length != (int)N)
                    throw new mcs_fatal("array() parameter count doen't match size. Provided: {0}, Expected: {1}", args.Length, N);

                for (int i = 0; i < args.Length; i++)
                    m_data[i] = args[i];
            }


            // IList
            public int IndexOf(T value) { return m_data.IndexOf(value); }
            void IList<T>.Insert(int index, T value) { throw new mcs_notimplemented(); }
            void IList<T>.RemoveAt(int index) { throw new mcs_notimplemented(); }
            void ICollection<T>.Add(T value) { throw new mcs_notimplemented(); }
            bool ICollection<T>.Contains(T value) { throw new mcs_notimplemented(); }
            void ICollection<T>.Clear() { throw new mcs_notimplemented(); }
            void ICollection<T>.CopyTo(T [] array, int index) { throw new mcs_notimplemented(); }
            bool ICollection<T>.Remove(T value) { throw new mcs_notimplemented(); }
            public int Count { get { return m_data.Count; } }
            bool ICollection<T>.IsReadOnly { get { throw new mcs_notimplemented(); } }
            IEnumerator IEnumerable.GetEnumerator() { return m_data.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return ((IEnumerable<T>)m_data).GetEnumerator(); }


            public static bool operator ==(array<T, size_t_N> lhs, array<T, size_t_N> rhs)
            {
                // TODO available in .NET 3.5 and higher
                //return Enumerable.SequenceEquals(lhs, rhs);

                if (ReferenceEquals(lhs, rhs))
                    return true;

                if (ReferenceEquals(lhs, default) || ReferenceEquals(rhs, default))
                    return false;

                if (lhs.size() != rhs.size())
                    return false;

                EqualityComparer<T> comparer = EqualityComparer<T>.Default;
                for (size_t i = 0; i < lhs.size(); i++)
                {
                    if (!comparer.Equals(lhs[i], rhs[i])) return false;
                }

                return true;
            }

            public static bool operator !=(array<T, size_t_N> lhs, array<T, size_t_N> rhs)
            {
                return !(lhs == rhs);
            }


            public override bool Equals(object obj)
            {
                return this == (array<T, size_t_N>)obj;
            }


            public override int GetHashCode()
            {
                return m_data.GetHashCode();
            }

            public T this[int index] { get { return m_data[index]; } set { m_data[index] = value; } }
            public T this[UInt64 index] { get { return m_data[index]; } set { m_data[index] = value; } }


            public size_t size() { return (size_t)Count; }
            public void fill(T value) { std.fill(this, value); }
            public Pointer<T> data() { return new Pointer<T>(m_data); }
        }


        // c++ errc
        public enum errc
        {
            address_family_not_supported,
            address_in_use,
            address_not_available,
            already_connected,
            argument_list_too_long,
            argument_out_of_domain,
            bad_address,
            bad_file_descriptor,
            bad_message,
            broken_pipe,
            connection_aborted,
            connection_already_in_progress,
            connection_refused,
            connection_reset,
            cross_device_link,
            destination_address_required,
            device_or_resource_busy,
            directory_not_empty,
            executable_format_error,
            file_exists,
            file_too_large,
            filename_too_long,
            function_not_supported,
            host_unreachable,
            identifier_removed,
            illegal_byte_sequence,
            inappropriate_io_control_operation,
            interrupted,
            invalid_argument,
            invalid_seek,
            io_error,
            is_a_directory,
            message_size,
            network_down,
            network_reset,
            network_unreachable,
            no_buffer_space,
            no_child_process,
            no_link,
            no_lock_available,
            no_message_available,
            no_message,
            no_protocol_option,
            no_space_on_device,
            no_stream_resources,
            no_such_device_or_address,
            no_such_device,
            no_such_file_or_directory,
            no_such_process,
            not_a_directory,
            not_a_socket,
            not_a_stream,
            not_connected,
            not_enough_memory,
            not_supported,
            operation_canceled,
            operation_in_progress,
            operation_not_permitted,
            operation_not_supported,
            operation_would_block,
            owner_dead,
            permission_denied,
            protocol_error,
            protocol_not_supported,
            read_only_file_system,
            resource_deadlock_would_occur,
            resource_unavailable_try_again,
            result_out_of_range,
            state_not_recoverable,
            stream_timeout,
            text_file_busy,
            timed_out,
            too_many_files_open_in_system,
            too_many_files_open,
            too_many_links,
            too_many_symbolic_link_levels,
            value_too_large,
            wrong_protocol_type,
        }


        // c++ error_category
        public class error_category
        {
            public virtual string name() { return "error_category.name() - TODO needs implementation"; }  //{ throw new emu_unimplemented(); }
            public virtual string message(int condition) { return string.Format("error_category.message({0}) - TODO needs implementation", condition); }  //{ throw new emu_unimplemented(); }
        }

        class generic_category : error_category
        {
            public override string name() { return "generic"; }
        }


        // c++ error_condition
        public class error_condition
        {
            int m_value;
            error_category m_category;

            public error_condition() { m_value = 0; m_category = new std.generic_category(); }
            public error_condition(errc e) { m_value = (int)e; m_category = new std.generic_category(); }

            public int value() { return m_value; }
            public error_category category() { return m_category; }
            public string message() { return category().message(value()); }

            public static bool operator ==(error_condition obj1, errc obj2) { return obj1 == new error_condition(obj2); }
            public static bool operator !=(error_condition obj1, errc obj2) { return obj1 != new error_condition(obj2); }

            public static implicit operator bool(error_condition e) { return !object.ReferenceEquals(e, null) && e.value() != 0; }
            public static implicit operator error_condition(errc d) { return new error_condition(d); }

            public override bool Equals(object obj)
            {
                return obj is error_condition condition &&
                       m_value == condition.m_value &&
                       EqualityComparer<error_category>.Default.Equals(m_category, condition.m_category);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(m_value.GetHashCode(), EqualityComparer<error_category>.Default.GetHashCode(m_category));
            }
        }


        // c++ forward_list
        public class forward_list<T> : IEnumerable<T>
        {
            LinkedList<T> m_list = new LinkedList<T>();


            public forward_list() { }
            public forward_list(IEnumerable<T> collection) { m_list = new LinkedList<T>(collection); }


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_list.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return m_list.GetEnumerator(); }


            public void emplace_front(T item) { m_list.AddFirst(item); }
        }


        // c++ istream
        public class istream
        {
        }


        // c++ list
        public class list<T> : IEnumerable<T>
        {
            LinkedList<T> m_list = new LinkedList<T>();


            public list() { }
            public list(IEnumerable<T> collection) { m_list = new LinkedList<T>(collection); }


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_list.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return m_list.GetEnumerator(); }


            // std::list functions
            public void clear() { m_list.Clear(); }
            public LinkedListNode<T> emplace_back(T item) { return m_list.AddLast(item); }
            public bool empty() { return m_list.Count == 0; }
            public void push_back(T item) { m_list.AddLast(item); }
            public void push_front(T item) { m_list.AddFirst(item); }
            public size_t size() { return (size_t)m_list.Count; }
        }


        // c++ map
        public class map<K, V> : IEnumerable<KeyValuePair<K, V>>
        {
            Dictionary<K, V> m_dictionary = new Dictionary<K, V>();


            public map() { }
            //public Dictionary(int capacity);
            //public Dictionary(IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary);
            //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
            //protected Dictionary(SerializationInfo info, StreamingContext context);


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_dictionary.GetEnumerator(); }
            IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() { return m_dictionary.GetEnumerator(); }


            public V this[K key] { get { return m_dictionary[key]; } set { m_dictionary[key] = value; } }


            public void Add(K key, V value) { m_dictionary.Add(key, value); }


            // std::map functions
            public void clear() { m_dictionary.Clear(); }
            public bool emplace(K key, V value) { if (m_dictionary.ContainsKey(key)) { return false; } else { m_dictionary.Add(key, value); return true; } }
            public void erase(K key) { m_dictionary.Remove(key); }
            public V find(K key) { V value; if (m_dictionary.TryGetValue(key, out value)) return value; else return default; }
            public size_t size() { return (size_t)m_dictionary.Count; }
            public bool try_emplace(K key, V value) { return emplace(key, value); }  // HACK - probably not the correct implementation
        }


        // c++ multimap
        public class multimap<K, V>
        {
            Dictionary<K, List<V>> m_dictionary = new Dictionary<K, List<V>>();


            public multimap() { }
            //public Dictionary(int capacity);
            //public Dictionary(IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary);
            //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
            //protected Dictionary(SerializationInfo info, StreamingContext context);
        }


        // c++ pair
        public class pair<T, V>
        {
            Tuple<T, V> m_pair;


            public pair(T key, V value) { m_pair = Tuple.Create(key, value); }

            public override string ToString() { return m_pair.ToString(); }


            public T first { get { return m_pair.Item1; } }
            public V second { get { return m_pair.Item2; } }
        }


        // c++ queue
        public class queue<T>
        {
            Queue<T> m_queue = new Queue<T>();

            public queue() { }


            // std::queue functions
            public void emplace(T value) { m_queue.Enqueue(value); }
            public bool empty() { return m_queue.Count == 0; }
            public T front() { return m_queue.Peek(); }
            public void pop() { m_queue.Dequeue(); }
            public size_t size() { return (size_t)m_queue.Count; }
        }


        // c++ set
        public class set<T> : IEnumerable<T>
        {
            HashSet<T> m_set = new HashSet<T>();


            public set() { }
            //public HashSet(IEqualityComparer<T> comparer);
            //public HashSet(IEnumerable<T> collection);
            //public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer);
            //protected HashSet(SerializationInfo info, StreamingContext context);


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_set.GetEnumerator(); }
            IEnumerator<T> IEnumerable<T>.GetEnumerator() { return m_set.GetEnumerator(); }


            public bool ContainsIf(Func<T, bool> predicate)
            {
                foreach (var element in m_set)
                {
                    if (predicate(element))
                        return true;
                }
                return false;
            }


            // std::set functions
            public void clear() { m_set.Clear(); }
            public bool emplace(T item) { return m_set.Add(item); }
            public bool empty() { return m_set.Count == 0; }
            public bool erase(T item) { return m_set.Remove(item); }
            public bool find(T item) { return m_set.Contains(item); }
            public T find(Func<T, bool> predicate) { return m_set.FirstOrDefault(predicate); }
            public bool insert(T item) { return m_set.Add(item); }
            public size_t size() { return (size_t)m_set.Count; }
        }


        // c++ stack
        public class stack<T>
        {
            Stack<T> m_stack = new Stack<T>();


            // std::stack functions
            public bool empty() { return m_stack.Count == 0; }
            public T top() { return m_stack.Peek(); }
            public void push(T value) { m_stack.Push(value); }
            public void pop() { m_stack.Pop(); }
        }


        // c++ stdexcept
        public class out_of_range : ArgumentOutOfRangeException
        {
            public out_of_range() : base() { }
            public out_of_range(string paramName) : base(paramName) { }
        }


        // c++ thread
        public class thread
        {
            public static unsigned hardware_concurrency() { return 1; }
        }


        // c++ unordered_map
        public class unordered_map<K, V> : IEnumerable<KeyValuePair<K, V>>
        {
            Dictionary<K, V> m_dictionary = new Dictionary<K, V>();


            public unordered_map() { }
            //public Dictionary(int capacity);
            //public Dictionary(IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary);
            //public Dictionary(int capacity, IEqualityComparer<TKey> comparer);
            //public Dictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer);
            //protected Dictionary(SerializationInfo info, StreamingContext context);


            // IEnumerable
            IEnumerator IEnumerable.GetEnumerator() { return m_dictionary.GetEnumerator(); }
            IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator() { return m_dictionary.GetEnumerator(); }


            // this behavior matches std::unordered_map [] operator, except it doesn't new() the object, but uses default instead.
            // this can cause some unexpected issues.  see models_t.value_str() for example
            public V this[K key]
            {
                get
                {
                    V value;
                    if (m_dictionary.TryGetValue(key, out value))
                        return value;

                    value = default;
                    m_dictionary.Add(key, value);
                    return value;
                }
                set
                {
                    m_dictionary[key] = value;
                }
            }


            // std::unordered_map functions
            public V at(K key) { V value; if (m_dictionary.TryGetValue(key, out value)) return value; else return default; }
            public void clear() { m_dictionary.Clear(); }
            public bool emplace(K key, V value) { if (m_dictionary.ContainsKey(key)) { return false; } else { m_dictionary.Add(key, value); return true; } }
            public bool empty() { return m_dictionary.Count == 0; }
            public bool erase(K key) { return m_dictionary.Remove(key); }
            public V find(K key) { return at(key); }
            public bool insert(K key, V value) { if (m_dictionary.ContainsKey(key)) { return false; } else { m_dictionary.Add(key, value); return true; } }
            public bool insert(std.pair<K, V> keyvalue) { return insert(keyvalue.first, keyvalue.second); }
            public size_t size() { return (size_t)m_dictionary.Count; }
        }


        // c++ unordered_set
        public class unordered_set<T>
        {
            HashSet<T> m_set = new HashSet<T>();


            public unordered_set() { }
            //public HashSet(IEqualityComparer<T> comparer);
            //public HashSet(IEnumerable<T> collection);
            //public HashSet(IEnumerable<T> collection, IEqualityComparer<T> comparer);
            //protected HashSet(SerializationInfo info, StreamingContext context);


            // std::unordered_set functions
            public void clear() { m_set.Clear(); }
            public bool emplace(T item) { return m_set.Add(item); }
            public bool erase(T item) { return m_set.Remove(item); }
            public bool find(T item) { return m_set.Contains(item); }
            public bool insert(T item) { return m_set.Add(item); }
        }


        // c++ vector
        public class vector<T> : MemoryContainer<T>
        {
            public vector() : base() { }
            // this is different behavior as List<T> so that it matches how std::vector works
            public vector(s32 count, T data = default) : base(count) { resize((size_t)count, data); }
            public vector(size_t count, T data = default) : base((int)count) { resize(count, data); }
            public vector(IEnumerable<T> collection) : base(collection) { }


            // std::vector functions
            public T front() { return this[0]; }
            public T back() { return empty() ? default : this[Count - 1]; }
            public void clear() { Clear(); }
            public Pointer<T> data() { return new Pointer<T>(this); }
            public void emplace(int index, T item) { Insert(index, item); }
            public void emplace_back(T item) { Add(item); }
            public bool empty() { return Count == 0; }
            public void erase(int index) { RemoveAt(index); }
            public void insert(int index, T item) { Insert(index, item); }
            public void pop_back() { if (Count > 0) { RemoveAt(Count - 1); } }
            public void push_back(T item) { Add(item); }
            public void push_front(T item) { Insert(0, item); }
            public void resize(size_t count, T data = default) { Resize((int)count, data); }
            public void resize(size_t count, Func<T> creator) { Resize((int)count, creator); }
            public void reserve(size_t value) { Capacity = (int)value; }
            public size_t size() { return (size_t)Count; }
        }
    }
}
