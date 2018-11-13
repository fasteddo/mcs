// license:BSD-3-Clause
// copyright-holders:Edward Fast

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


        // extensions to match pstring (netlist)

        public static double as_double(this String str) { return Convert.ToDouble(str); }
        public static Int32 as_long(this String str) { return Convert.ToInt32(str); }
        public static bool endsWith(this String str, string arg) { return str.EndsWith(arg); }
        public static bool equals(this String str, string string_) { return str == string_; }
        public static int find_first_not_of(this String str, string no)
        {
            int pos = 0;
            foreach (var it in str)  //for (auto it = begin(); it != end(); ++it, ++pos)
            {
                bool f = true;
                foreach (var jt in no)  //for (code_t const jt : no)
                {
                    if (it == jt)
                    {
                        f = false;
                        break;
                    }
                }

                if (f)
                    return pos;
            }

            return -1;
        }

        public static int find_last_not_of(this String str, string no)
        {
            /* FIXME: reverse iterator */
            int last_found = -1;
            int pos = 0;
            for (int i = 0; i < str.Length; i++, ++pos)  //for (auto it = begin(); it != end(); ++it, ++pos)
            {
                var it = str[i];

                bool f = true;
                foreach (var jt in no)  //for (code_t const jt : no)
                {
                    if (it == jt)
                    {
                        f = false;
                        break;
                    }
                }

                if (f)
                    last_found = pos;
            }

            return last_found;
        }

        public static string left(this String str, int len) { return str.substr(0, len); }
        public static string ltrim(this String str, string ws = " \t\n\r") { return str.substr(str.find_first_not_of(ws)); }
        public static string rtrim(this String str, string ws = " \t\n\r") { var f = str.find_last_not_of(ws); return f == -1 ? "" : str.substr(0, f + 1); }
        public static bool startsWith(this String str, string arg) { return str.StartsWith(arg); }
        public static string trim(this String str, string ws = " \t\n\r") { return str.ltrim(ws).rtrim(ws); }
        public static string ucase(this String str) { return str.ToUpper(); }
    }
}
