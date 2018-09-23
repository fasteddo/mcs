using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using u32 = System.UInt32;


namespace mame
{
    public static class ExtensionKeyValuePair
    {
        // extensions to match std::pair
        // cannot derive from KeyValuePair since it's sealed.

        public static K first<K, V>(this KeyValuePair<K, V> kvp) { return kvp.Key; }
        public static V second<K, V>(this KeyValuePair<K, V> kvp) { return kvp.Value; }
    }


    public static class ExtensionString
    {
        // extensions to match std::string

        public static void append(this String str, string s) { str += s; }
        public static string c_str(this String str) { return str; }
        public static int compare(this String str, string s) { return str.CompareTo(s); }
        public static bool empty(this String str) { return string.IsNullOrEmpty(str); }
        public static int find(this String str, string s) { return str.IndexOf(s); }
        public static int find_first_of(this String str, string s, int pos = 0) { return str.IndexOf(s, pos); }
        public static int find_first_of(this String str, char c, int pos = 0) { return str.IndexOf(c, pos); }
        public static int find_last_of(this String str, string s, int pos = 0) { return str.LastIndexOf(s, pos); }
        public static int find_last_of(this String str, char c, int pos = 0) { return str.LastIndexOf(c, pos); }
        public static int length(this String str) { return str.Length; }
        public static void reserve(this String str, int n) { }
        public static string str(this String str) { return str; }
        public static string substr(this String str, int pos) { return str.Substring(pos); }
        public static string substr(this String str, int pos, int count) { return str.Substring(pos, count); }
    }
}
