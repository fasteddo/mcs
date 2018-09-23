// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


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
        //int core_strnicmp(const char *s1, const char *s2, size_t n);


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


        public static bool core_iswildstr(string sp) { throw new emu_unimplemented(); }


        //int strcatvprintf(std::string &str, const char *format, va_list args);

        //void strdelchr(std::string& str, char chr);
        //void strreplacechr(std::string& str, char ch, char newch);
        //std::string &strtrimspace(std::string& str);
        //std::string &strtrimrightspace(std::string& str);
        //std::string &strmakeupper(std::string& str);
        //std::string &strmakelower(std::string& str);
        //int strreplace(std::string &str, const std::string& search, const std::string& replace);
    }
}
