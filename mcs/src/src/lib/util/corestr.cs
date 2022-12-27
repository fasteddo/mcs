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
            //for (;;)
            //{
            //    int c1 = tolower((UINT8)*s1++);
            //    int c2 = tolower((UINT8)*s2++);
            //    if (c1 == 0 || c1 != c2)
            //        return c1 - c2;
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


        /* additional string compare helper (up to 16 characters at the moment) */
        /*-------------------------------------------------
            core_strwildcmp - case-insensitive wildcard
            string compare (up to 16 characters at the
            moment)
        -------------------------------------------------*/
        public static int core_strwildcmp(string sp1, string sp2)
        {
            char [] s1 = new char[17];
            char [] s2 = new char[17];
            int i;
            int l1;
            int l2;
            int pIdx;  // char *p;

            //assert(strlen(sp1) < 16);
            //assert(strlen(sp2) < 16);

            if (string.IsNullOrEmpty(sp1)) Array.Copy("*".ToCharArray(), s1, "*".Length);  // strcpy(s1, "*");
            else { Array.Copy(sp1.ToCharArray(), s1, sp1.Length); }  // strncpy(s1, sp1, 16); s1[16] = 0; }

            if (string.IsNullOrEmpty(sp2)) Array.Copy("*".ToCharArray(), s2, "*".Length);  //strcpy(s2, "*");
            else { Array.Copy(sp2.ToCharArray(), s2, sp2.Length); }  // strncpy(s2, sp2, 16); s2[16] = 0; }

            pIdx = new string(s1).IndexOf('*');  //p = strchr(s1, '*');
            if (pIdx != -1)
            {
                for (i = pIdx; i < 16; i++)  // for (i = p - s1; i < 16; i++) s1[i] = '?';
                    s1[i] = '?';

                s1[16] = '\0';
            }

            pIdx = new string(s2).IndexOf('*');  //p = strchr(s2, '*');
            if (pIdx != -1)
            {
                for (i = pIdx; i < 16; i++)  // for (i = p - s2; i < 16; i++) s2[i] = '?';
                    s2[i] = '?';

                s2[16] = '\0';
            }

            l1 = s1.Length;
            if (l1 < 16)
            {
                for (i = l1 + 1; i < 16; i++)  // for (i = l1 + 1; i < 16; i++) s1[i] = ' ';
                    s1[i] = ' ';

                s1[16] = '\0';
            }

            l2 = s2.Length;
            if (l2 < 16)
            {
                for (i = l2 + 1; i < 16; i++)  // for (i = l2 + 1; i < 16; i++) s2[i] = ' ';
                    s2[i] = ' ';

                s2[16] = '\0';
            }

            for (i = 0; i < 16; i++)
            {
                if (s1[i] == '?' && s2[i] != '?')
                    s1[i] = s2[i];

                if (s2[i] == '?' && s1[i] != '?')
                    s2[i] = s1[i];
            }

            return core_stricmp(new string(s1), new string(s2));
        }


        public static bool core_iswildstr(string sp)
        {
            //for ( ; sp && *sp; sp++)
            //{
            //    if (('?' == *sp) || ('*' == *sp))
            //        return true;
            //}
            //return false;
            return sp.IndexOfAny("?*".ToCharArray()) != -1;
        }


        //void strdelchr(std::string& str, char chr);
        //void strreplacechr(std::string& str, char ch, char newch);


        public static string strtrimspace(string str)
        {
            return str.Trim();  //return internal_strtrimspace(str, false);
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
