// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


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

        public static string append(this String str, string s) { return str + s; }
        public static char back(this String str) { return str.Length > 0 ? str[str.Length - 1] : '\0'; }
        public static string c_str(this String str) { return str; }
        public static int compare(this String str, string s) { return str.CompareTo(s); }
        public static bool empty(this String str) { return string.IsNullOrEmpty(str); }
        public static int find(this String str, char c) { return str.IndexOf(c); }
        public static int find(this String str, char c, int start) { return str.IndexOf(c, start); }
        public static int find(this String str, string s) { return str.IndexOf(s); }
        public static int find(this String str, string s, int start) { return str.IndexOf(s, start); }
        public static int find_first_of(this String str, string s, int pos = 0) { return str.IndexOf(s, pos); }
        public static int find_first_of(this String str, char c, int pos = 0) { return str.IndexOf(c, pos); }
        public static int find_last_of(this String str, string s) { return str.LastIndexOf(s, (str.Length - 1) > 0 ? str.Length - 1 : 0); }
        public static int find_last_of(this String str, string s, int pos) { return str.LastIndexOf(s, pos); }
        public static int find_last_of(this String str, char c) { return str.LastIndexOf(c, (str.Length - 1) > 0 ? str.Length - 1 : 0); }
        public static int find_last_of(this String str, char c, int pos) { return str.LastIndexOf(c, pos); }
        public static string insert(this String str, int pos, string s) { return str.Insert(pos, s); }
        public static int length(this String str) { return str.Length; }
        public static void reserve(this String str, int n) { }
        public static int rfind(this String str, char c) { return str.LastIndexOf(c); }
        public static int rfind(this String str, string s) { return str.LastIndexOf(s); }
        public static int size(this String str) { return str.Length; }
        public static string str(this String str) { return str; }
        public static string substr(this String str, int pos) { return str.Substring(pos); }
        public static string substr(this String str, int pos, int count) { return str.Substring(pos, count); }
    }
}
