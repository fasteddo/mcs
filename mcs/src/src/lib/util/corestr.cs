// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using size_t = System.UInt64;

using static mame.corestr_global;


namespace mame
{
    public static class corestr_global
    {
        /* since stricmp is not part of the standard, we use this instead */
        /*-------------------------------------------------
            core_stricmp - case-insensitive string compare
        -------------------------------------------------*/
        public static int core_stricmp(string s1, string s2)
        {
            //auto s1_iter = s1.begin();
            //auto s2_iter = s2.begin();
            //while (true)
            //{
            //    if (s1.end() == s1_iter)
            //        return (s2.end() == s2_iter) ? 0 : -1;
            //    else if (s2.end() == s2_iter)
            //        return 1;
            //
            //    const int c1 = tolower(uint8_t(*s1_iter++));
            //    const int c2 = tolower(uint8_t(*s2_iter++));
            //    const int diff = c1 - c2;
            //    if (diff)
            //        return diff;
            //}

            return string.Compare(s1, s2, StringComparison.CurrentCultureIgnoreCase);
        }


        /* since strnicmp is not part of the standard, we use this instead */
        public static int core_strnicmp(string s1, string s2, size_t n)
        {
            //size_t i;
            //for (i = 0; i < n; i++)
            //{
            //    int c1 = tolower((uint8_t)*s1++);
            //    int c2 = tolower((uint8_t)*s2++);
            //    if (c1 == 0 || c1 != c2)
            //        return c1 - c2;
            //}
            //
            //return 0;

            return string.Compare(s1, 0, s2, 0, (int)n, StringComparison.CurrentCultureIgnoreCase);
        }


        /*-------------------------------------------------
            core_strwildcmp - case-insensitive wildcard
            string compare
        -------------------------------------------------*/
        public static int core_strwildcmp(string s1, string s2)
        {
            // slight tweak of core_stricmp() logic
            int s1_iterIdx = 0;  //auto s1_iter = s1.begin();
            int s2_iterIdx = 0;  //auto s2_iter = s2.begin();
            while (true)
            {
                if ((s1.Length != s1_iterIdx && s1[s1_iterIdx] == '*')  //if ((s1.end() != s1_iter && *s1_iter == '*')
                    || (s2.Length != s2_iterIdx && s2[s2_iterIdx] == '*'))  //|| (s2.end() != s2_iter && *s2_iter == '*'))
                    return 0;

                if (s1.Length == s1_iterIdx)  //if (s1.end() == s1_iter)
                    return (s2.Length == s2_iterIdx) ? 0 : -1;  //return (s2.end() == s2_iter) ? 0 : -1;
                else if (s2.Length == s2_iterIdx)  //else if (s2.end() == s2_iter)
                    return 1;

                char c1 = char.ToLower(s1[s1_iterIdx++]);  //const int c1 = tolower(uint8_t(*s1_iter++));
                char c2 = char.ToLower(s2[s2_iterIdx++]);  //const int c2 = tolower(uint8_t(*s2_iter++));
                int diff = (c1 != '?' && c2 != '?')
                    ? c1 - c2
                    : 0;
                if (diff != 0)
                    return diff;
            }
        }


        public static bool core_iswildstr(string sp)
        {
            //auto iter = std::find_if(s.begin(), s.end(), [](char c)
            //{
            //    return c == '?' || c == '*';
            //});
            //return iter != s.end();
            return sp.IndexOfAny("?*".ToCharArray()) != -1;
        }


        /* trim functions */
        //template<typename TPred>
        //std::string_view strtrimleft(std::string_view str, TPred pred)

        //template<typename TPred>
        //std::string_view strtrimright(std::string_view str, TPred pred)


        //void strdelchr(std::string& str, char chr);
        //void strreplacechr(std::string& str, char ch, char newch);


        public static string strtrimspace(string str)
        {
            //std::string_view str2 = strtrimleft(str, [] (char c) { return !isspace(uint8_t(c)); });
            //return strtrimright(str2, [] (char c) { return !isspace(uint8_t(c)); });
            return str.Trim();
        }


        //std::string &strtrimrightspace(std::string& str);
        //std::string &strmakeupper(std::string& str);
        //std::string &strmakelower(std::string& str);


        /**
         * @fn  int strreplace(std::string &str, const std::string& search, const std::string& replace)
         *
         * @brief   Strreplaces.
         *
         * @param [in,out]  str The string.
         * @param   search      The search.
         * @param   replace     The replace.
         *
         * @return  An int.
         */

        public static int strreplace(ref string str, string search, string replace)
        {
            int searchlen = (int)search.length();
            int replacelen = (int)replace.length();
            int matches = 0;

            for (int curindex = (int)str.find(search, 0); curindex != -1; curindex = (int)str.find(search, (size_t)(curindex + replacelen)))
            {
                matches++;
                str = str.Remove(curindex, searchlen).insert_((size_t)curindex, replace);  //str.erase(curindex, searchlen).insert(curindex, replace);
            }

            return matches;
        }
    }


    public static partial class util
    {
        //bool strequpper(std::string_view str, std::string_view ucstr);


        /**
         * @fn  bool streqlower(std::string_view str, std::string_view lcstr)
         *
         * @brief   Tests whether a mixed-case string matches a lowercase string.
         *
         * @param [in]  str   First string to compare (may be mixed-case).
         * @param [in]  lcstr Second string to compare (must be all lowercase).
         *
         * @return  True if the strings match regardless of case.
         */
        public static bool streqlower(string str, string lcstr)
        {
            //return std::equal(str.begin(), str.end(), lcstr.begin(), lcstr.end(),
            //                    [] (unsigned char c1, unsigned char c2) { return std::tolower(c1) == c2; });
            return string.Compare(str, lcstr, StringComparison.CurrentCultureIgnoreCase) == 0;
        }


        /**
         * @fn  double edit_distance(std::u32string const &lhs, std::u32string const &rhs)
         *
         * @brief   Compares strings and returns prefix-weighted similarity score (smaller is more similar).
         *
         * @param   lhs         First input.
         * @param   rhs         Second input.
         *
         * @return  Similarity score ranging from 0.0 (totally dissimilar) to 1.0 (identical).
         */
        // based on Jaro-Winkler distance - returns value from 0.0 (totally dissimilar) to 1.0 (identical)
        public static double edit_distance(string lhs, string rhs)
        {
            // based on Jaro-Winkler distance
            // TODO: this breaks if the lengths don't fit in a long int, but that's not a big limitation
            long MAX_PREFIX = 4;
            double PREFIX_WEIGHT = 0.1;
            double PREFIX_THRESHOLD = 0.7;

            string longer = (lhs.length() >= rhs.length()) ? lhs : rhs;
            string shorter = (lhs.length() < rhs.length()) ? lhs : rhs;

            // find matches
            long range = std.max((long)(longer.length() / 2) - 1, 0L);
            long [] match_idx = new long [shorter.length()];  //std::unique_ptr<long []> match_idx(std::make_unique<long []>(shorter.length()));
            bool [] match_flg = new bool [longer.length()];
            std.fill_n(match_idx, shorter.length(), -1);
            std.fill_n(match_flg, longer.length(), false);
            long match_cnt = 0;
            for (long i = 0; (long)shorter.length() > i; ++i)
            {
                char ch = shorter[(int)i];
                long n = std.min(i + range + 1L, (long)longer.length());
                for (long j = std.max(i - range, 0); n > j; ++j)
                {
                    if (!match_flg[j] && (ch == longer[(int)j]))
                    {
                        match_idx[i] = j;
                        match_flg[j] = true;
                        ++match_cnt;
                        break;
                    }
                }
            }

            // early exit if strings are very dissimilar
            if (match_cnt == 0)
                return 1.0;

            // now find transpositions
            char [] ms = new char [2 * match_cnt];  //std::unique_ptr<char32_t []> ms(std::make_unique<char32_t []>(2 * match_cnt));
            std.fill_n(ms, 2 * (size_t)match_cnt, (char)0);
            int ms1Idx = 0;  //char32_t *const ms1(&ms[0]);
            int ms2Idx = (int)match_cnt;  //char32_t *const ms2(&ms[match_cnt]);
            for (long i = 0, j = 0; (long)shorter.length() > i; ++i)
            {
                if (0 <= match_idx[i])
                    ms[ms1Idx + j++] = shorter[(int)i];  //ms1[j++] = shorter[i];
            }
            match_idx = null;  //match_idx.reset();
            for (long i = 0, j = 0; (long)longer.length() > i; ++i)
            {
                if (match_flg[i])
                    ms[ms2Idx + j++] = longer[(int)i];  //ms2[j++] = longer[i];
            }
            match_flg = null;  //match_flg.reset();
            long halftrans_cnt = 0;
            for (long i = 0; match_cnt > i; ++i)
            {
                if (ms[ms1Idx + i] != ms[ms2Idx + i])  //if (ms1[i] != ms2[i])
                    ++halftrans_cnt;
            }
            ms = null;  //ms.reset();

            // simple prefix detection
            long prefix_len = 0;
            for (long i = 0; (std.min((long)shorter.length(), MAX_PREFIX) > i) && (lhs[(int)i] == rhs[(int)i]); ++i)
                ++prefix_len;

            // do the weighting
            double m = match_cnt;
            double t = (double)halftrans_cnt / 2;
            double jaro = ((m / lhs.length()) + (m / rhs.length()) + ((m - t) / m)) / 3;
            double jaro_winkler = (PREFIX_THRESHOLD > jaro) ? jaro : (jaro + (PREFIX_WEIGHT * prefix_len * (1.0 - jaro)));
            return 1.0 - jaro_winkler;
        }
    }
}
