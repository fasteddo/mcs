// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using size_t = System.UInt64;


namespace mame.plib
{
    public class pstrutil_global
    {
        public static bool startsWith(string str, string arg) { return str.StartsWith(arg); }
        public static bool endsWith(string str, string value) { return str.EndsWith(value); }
        public static string ucase(string str) { return str.ToUpper(); }
        public static string trim(string str) { return str.Trim(); }
        public static string left(string str, size_t len) { return str.Substring(0, (int)len); }
        public static string right(string str, size_t nlen) { return (int)nlen >= str.Length ? str : str.Substring(str.Length - (int)nlen); }
        public static string replace_all(string str, string search, string replace) { return str.Replace(search, replace); }

        //template <typename T>
        public static std.vector<string> psplit(string str, string onstr, bool ignore_empty = false)  //std::vector<T> psplit(const T &str, const T &onstr, bool ignore_empty = false)
        {
            //std::vector<T> ret;
            //
            //auto p = str.begin();
            //auto pn = std::search(p, str.end(), onstr.begin(), onstr.end());
            //const auto ol = static_cast<typename T::difference_type>(onstr.length());
            //
            //while (pn != str.end())
            //{
            //    if (!ignore_empty || p != pn)
            //        ret.emplace_back(p, pn);
            //    p = std::next(pn, ol);
            //    pn = std::search(p, str.end(), onstr.begin(), onstr.end());
            //}
            //if (p != str.end())
            //{
            //    ret.emplace_back(p, str.end());
            //}
            //return ret;
            return new std.vector<string>(str.Split(new string [] { onstr }, ignore_empty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None));
        }


        //template <typename T>
        public static std.vector<string> psplit(string str, char onstr, bool ignore_empty = false)  //std::vector<T> psplit(const T &str, const typename T::value_type &onstr, bool ignore_empty = false)
        {
            //std::vector<T> ret;
            //
            //auto p = str.begin();
            //auto pn = std::find(p, str.end(), onstr);
            //
            //while (pn != str.end())
            //{
            //    if (!ignore_empty || p != pn)
            //        ret.emplace_back(p, pn);
            //    p = std::next(pn, 1);
            //    pn = std::find(p, str.end(), onstr);
            //}
            //
            //if (p != str.end())
            //{
            //    ret.emplace_back(p, str.end());
            //}
            //
            //return ret;
            return new std.vector<string>(str.Split(new char [] { onstr }, ignore_empty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None));
        }


        //template <typename T>
        public static std.vector<string> psplit(string str, std.vector<string> onstrl)  //std::vector<T> psplit(const T &str, const std::vector<T> &onstrl)
        {
            string col = "";  //T col = "";
            std.vector<string> ret = new std.vector<string>();  //std::vector<T> ret;

            size_t i = 0;  //auto i = str.begin();
            while (i != (size_t)str.Length)  //while (i != str.end())
            {
                size_t p = g.npos;  //auto p = T::npos;
                for (size_t j = 0; j < onstrl.size(); j++)  //for (std::size_t j=0; j < onstrl.size(); j++)
                {
                    if (onstrl[j] == str.Substring((int)i, Math.Min(str.Length - (int)i, onstrl[j].Length)))  //if (std::equal(onstrl[j].begin(), onstrl[j].end(), i))
                    {
                        p = j;
                        break;
                    }
                }

                if (p != g.npos) //if (p != T::npos)
                {
                    if (col != "")  //if (!col.empty())
                        ret.push_back(col);

                    col = "";  //col.clear();
                    ret.push_back(onstrl[p]);
                    i += onstrl[p].length();  //i = std::next(i, narrow_cast<typename T::difference_type>(onstrl[p].length()));
                }
                else
                {
                    char c = str[(int)i];  //typename T::value_type c = *i;
                    col += c;
                    i++;
                }
            }

            if (col != "")  //if (!col.empty())
                ret.push_back(col);

            return ret;
        }
    }
}
