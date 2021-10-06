// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using size_t = System.UInt64;


namespace mame
{
    public static class ExtensionArray
    {
        public static void Fill<T>(this T [] array, T value)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = value;
        }

        public static void Fill<T>(this T [] array, Func<T> creator)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = creator();
        }
    }


    public static class ExtensionIComparable
    {
        // EDF - this can be removed in newer .NET versions
        public static T Clamp<T>(this T val, T min, T max) where T : IComparable<T>
        {
            if (val.CompareTo(min) < 0) return min;
            else if(val.CompareTo(max) > 0) return max;
            else return val;
        }
    }


    public static class ExtensionKeyValuePair
    {
        // extensions to match std::pair
        // cannot derive from KeyValuePair since it's sealed.

        public static K first<K, V>(this KeyValuePair<K, V> kvp) { return kvp.Key; }
        public static V second<K, V>(this KeyValuePair<K, V> kvp) { return kvp.Value; }
    }


    public static class ExtensionString
    {
        // custom
        public static int FindLastNotOf(this String source, String chars)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (chars == null) throw new ArgumentNullException("chars");
            if (chars.Length == 0) return source.Length - 1;

            for (int i = source.Length - 1; i >= 0; i--)
            {
                if (chars.IndexOf(source[i]) == -1) return i;
            }

            return -1;
        }


        public static int IndexOf(this String source, Func<char, bool> predicate)
        {
            int i = 0;
            foreach (var element in source)
            {
                if (predicate(element))
                    return i;
                i++;
            }
            return -1;
        }


        // extensions to match std::string

        public static string append_(this String str, string s) { return str + s; }
        public static char back(this String str) { return str.Length > 0 ? str[str.Length - 1] : '\0'; }
        public static string clear_(this String str) { return string.Empty; }
        public static int compare(this String str, string s) { return str.CompareTo(s); }
        public static int compare(this String str, size_t pos, size_t len, string s) { return (pos + len) <= (size_t)str.Length ? str.Substring((int)pos, (int)len).CompareTo(s) : str.CompareTo(s); }
        public static bool empty(this String str) { return string.IsNullOrEmpty(str); }
        public static size_t find(this String str, char c) { return (size_t)str.IndexOf(c); }
        public static size_t find(this String str, char c, size_t start) { return (size_t)str.IndexOf(c, (int)start); }
        public static size_t find(this String str, string s) { return (size_t)str.IndexOf(s); }
        public static size_t find(this String str, string s, size_t start) { return (size_t)str.IndexOf(s, (int)start); }
        public static size_t find_first_of(this String str, string s, size_t pos = 0) { return (size_t)str.IndexOf(s, (int)pos); }
        public static size_t find_first_of(this String str, char c, size_t pos = 0) { return (size_t)str.IndexOf(c, (int)pos); }
        public static size_t find_last_not_of(this String str, char c) { return (size_t)str.FindLastNotOf(c.ToString()); }
        public static size_t find_last_of(this String str, string s) { return (size_t)str.LastIndexOf(s, (str.Length - 1) > 0 ? str.Length - 1 : 0); }
        public static size_t find_last_of(this String str, string s, int pos) { return (size_t)str.LastIndexOf(s, pos); }
        public static size_t find_last_of(this String str, char c) { return (size_t)str.LastIndexOf(c, (str.Length - 1) > 0 ? str.Length - 1 : 0); }
        public static size_t find_last_of(this String str, char c, size_t pos) { return (size_t)str.LastIndexOf(c, (int)pos); }
        public static string insert_(this String str, size_t pos, string s) { return str.Insert((int)pos, s); }
        public static size_t length(this String str) { return (size_t)str.Length; }
        public static void reserve(this String str, size_t n) { }
        public static size_t rfind(this String str, char c) { return (size_t)str.LastIndexOf(c); }
        public static size_t rfind(this String str, string s) { return (size_t)str.LastIndexOf(s); }
        public static size_t size(this String str) { return (size_t)str.Length; }
        public static string substr(this String str, size_t pos) { return str.Substring((int)pos); }
        public static string substr(this String str, size_t pos, size_t count) { return str.Substring((int)pos, (int)count); }
    }
}
