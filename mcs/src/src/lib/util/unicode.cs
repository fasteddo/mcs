// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using char32_t = System.UInt32;


namespace mame
{
    //typedef UINT16 utf16_char;
    //typedef UINT32 unicode_char;


    public static class unicode_global
    {
        /* these defines specify the maximum size of different types of Unicode
         * character encodings */
        public const int UTF8_CHAR_MAX = 6;
        public const int UTF16_CHAR_MAX = 2;

        /* these are UTF-8 encoded strings for common characters */
        public const string UTF8_NBSP               = "\xc2\xa0";          /* non-breaking space */

        public const string UTF8_MULTIPLY           = "\xc3\x97";          /* multiplication sign */
        //#define UTF8_DIVIDE             "\xc3\xb7"          /* division sign */
        //#define UTF8_SQUAREROOT         "\xe2\x88\x9a"      /* square root symbol */
        //#define UTF8_PLUSMINUS          "\xc2\xb1"          /* plusminus symbol */

        //#define UTF8_POW_2              "\xc2\xb2"          /* superscript 2 */
        //#define UTF8_POW_X              "\xcb\xa3"          /* superscript x */
        //#define UTF8_POW_Y              "\xca\xb8"          /* superscript y */
        //#define UTF8_PRIME              "\xca\xb9"          /* prime symbol */
        //#define UTF8_DEGREES            "\xc2\xb0"          /* degrees symbol */

        //#define UTF8_SMALL_PI           "\xcf\x80"          /* Greek small letter pi */
        //#define UTF8_CAPITAL_SIGMA      "\xce\xa3"          /* Greek capital letter sigma */
        //#define UTF8_CAPITAL_DELTA      "\xce\x94"          /* Greek capital letter delta */

        //#define UTF8_MACRON             "\xc2\xaf"          /* macron symbol */
        //#define UTF8_NONSPACE_MACRON    "\xcc\x84"          /* nonspace macron, use after another char */

        //#define a_RING                  "\xc3\xa5"          /* small a with a ring */
        //#define a_UMLAUT                "\xc3\xa4"          /* small a with an umlaut */
        //#define o_UMLAUT                "\xc3\xb6"          /* small o with an umlaut */
        //#define u_UMLAUT                "\xc3\xbc"          /* small u with an umlaut */
        //#define e_ACUTE                 "\xc3\xa9"          /* small e with an acute */
        //#define n_TILDE                 "\xc3\xb1"          /* small n with a tilde */

        //#define A_RING                  "\xc3\x85"          /* capital A with a ring */
        //#define A_UMLAUT                "\xc3\x84"          /* capital A with an umlaut */
        //#define O_UMLAUT                "\xc3\x96"          /* capital O with an umlaut */
        //#define U_UMLAUT                "\xc3\x9c"          /* capital U with an umlaut */
        //#define E_ACUTE                 "\xc3\x89"          /* capital E with an acute */
        //#define N_TILDE                 "\xc3\x91"          /* capital N with a tilde */

        //#define UTF8_LEFT               "\xe2\x86\x90"      /* cursor left */
        //#define UTF8_RIGHT              "\xe2\x86\x92"      /* cursor right */
        //#define UTF8_UP                 "\xe2\x86\x91"      /* cursor up */
        //#define UTF8_DOWN               "\xe2\x86\x93"      /* cursor down */

        //enum class unicode_normalization_form { C, D, KC, KD };


        /* tests to see if a unicode char is a valid code point */
        /*-------------------------------------------------
            uchar_isvalid - return true if a given
            character is a legitimate unicode character
        -------------------------------------------------*/

        static bool uchar_isvalid(char32_t uchar)
        {
            return (uchar < 0x110000) && !((uchar >= 0xd800) && (uchar <= 0xdfff));
        }


        // tests to see if a unicode char is printable
        //-------------------------------------------------
        //  uchar_is_printable - tests to see if a unicode
        //  char is printable
        //-------------------------------------------------
        public static bool uchar_is_printable(char32_t uchar)
        {
            return
                !(0x0001f >= uchar) &&                            // C0 control
                !((0x0007f <= uchar) && (0x0009f >= uchar)) &&    // DEL and C1 control
                !((0x0fdd0 <= uchar) && (0x0fddf >= uchar)) &&    // noncharacters
                !(0x0fffe == (uchar & 0x0ffff)) &&                // byte-order detection noncharacter
                !(0x0ffff == (uchar & 0x0ffff));                  // the other noncharacter
        }


        // tests to see if a unicode char is a digit
        //bool uchar_is_digit(char32_t uchar);


        /* converting strings to 32-bit Unicode chars */

        /*-------------------------------------------------
            uchar_from_utf8 - convert a UTF-8 sequence
            into a unicode character
        -------------------------------------------------*/
        public static int uchar_from_utf8(out char32_t uchar, string utf8char, int count)
        {
            uchar = 0;

            char32_t c;
            char32_t minchar;
            int auxlen;
            int i;
            char auxchar;

            /* validate parameters */
            if (utf8char == null || count == 0)
                return 0;

            int utf8charIdx = 0;

            /* start with the first byte */
            c = utf8char[utf8charIdx];
            count--;
            utf8charIdx++;

            /* based on that, determine how many additional bytes we need */
            if (c < 0x80)
            {
                /* unicode char 0x00000000 - 0x0000007F */
                c &= 0x7f;
                auxlen = 0;
                minchar = 0x00000000;
            }
            else if (c >= 0xc0 && c < 0xe0)
            {
                /* unicode char 0x00000080 - 0x000007FF */
                c &= 0x1f;
                auxlen = 1;
                minchar = 0x00000080;
            }
            else if (c >= 0xe0 && c < 0xf0)
            {
                /* unicode char 0x00000800 - 0x0000FFFF */
                c &= 0x0f;
                auxlen = 2;
                minchar = 0x00000800;
            }
            else if (c >= 0xf0 && c < 0xf8)
            {
                /* unicode char 0x00010000 - 0x001FFFFF */
                c &= 0x07;
                auxlen = 3;
                minchar = 0x00010000;
            }
            else if (c >= 0xf8 && c < 0xfc)
            {
                /* unicode char 0x00200000 - 0x03FFFFFF */
                c &= 0x03;
                auxlen = 4;
                minchar = 0x00200000;
            }
            else if (c >= 0xfc && c < 0xfe)
            {
                /* unicode char 0x04000000 - 0x7FFFFFFF */
                c &= 0x01;
                auxlen = 5;
                minchar = 0x04000000;
            }
            else
            {
                /* invalid */
                return -1;
            }

            /* exceeds the count? */
            if (auxlen > (int)count)
                return -1;

            /* we now know how long the char is, now compute it */
            for (i = 0; i < auxlen; i++)
            {
                auxchar = utf8char[i];

                /* all auxillary chars must be between 0x80-0xbf */
                if ((auxchar & 0xc0) != 0x80)
                    return -1;

                c = c << 6;
                c |= (UInt32)(auxchar & 0x3f);
            }

            /* make sure that this char is above the minimum */
            if (c < minchar)
                return -1;

            uchar = c;
            return auxlen + 1;
        }


        //int uchar_from_utf16(unicode_char *uchar, const utf16_char *utf16char, size_t count);
        //int uchar_from_utf16f(unicode_char *uchar, const utf16_char *utf16char, size_t count);

        // converting UTF-8 strings to/from "wide" strings
        //std::wstring wstring_from_utf8(const std::string &utf8string);
        //std::string utf8_from_wstring(const std::wstring &string);

        // unicode normalization
        //std::string normalize_unicode(const std::string &s, unicode_normalization_form normalization_form);
        //std::string normalize_unicode(const char *s, unicode_normalization_form normalization_form);
        //std::string normalize_unicode(const char *s, size_t length, unicode_normalization_form normalization_form);


        /* converting 32-bit Unicode chars to strings */

        /*-------------------------------------------------
            utf8_from_uchar - convert a unicode character
            into a UTF-8 sequence
        -------------------------------------------------*/
        public static int utf8_from_uchar(out string utf8string, /*UInt32 count,*/ char32_t uchar)
        {
            utf8string = "";

            int rc = 0;

            /* error on invalid characters */
            if (!uchar_isvalid(uchar))
                return -1;

            /* based on the value, output the appropriate number of bytes */
            if (uchar < 0x80)
            {
                /* unicode char 0x00000000 - 0x0000007F */
                //if (count < 1)
                //    return -1;

                //utf8string[rc++] = (char) uchar;
                utf8string += (char)uchar;
            }
            else if (uchar < 0x800)
            {
                /* unicode char 0x00000080 - 0x000007FF */
                //if (count < 2)
                //    return -1;

                utf8string += (char)(((char)(uchar >> 6)) | 0xC0);
                utf8string += (char)(((char) ((uchar >> 0) & 0x3F)) | 0x80);
            }
            else if (uchar < 0x10000)
            {
                /* unicode char 0x00000800 - 0x0000FFFF */
                //if (count < 3)
                //    return -1;

                utf8string += (char)(((char)(uchar >> 12)) | 0xE0);
                utf8string += (char)(((char)((uchar >> 6) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 0) & 0x3F)) | 0x80);
            }
            else if (uchar < 0x00200000)
            {
                /* unicode char 0x00010000 - 0x001FFFFF */
                //if (count < 4)
                //    return -1;

                utf8string += (char)(((char)(uchar >> 18)) | 0xF0);
                utf8string += (char)(((char)((uchar >> 12) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 6) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 0) & 0x3F)) | 0x80);
            }
            else if (uchar < 0x04000000)
            {
                /* unicode char 0x00200000 - 0x03FFFFFF */
                //if (count < 5)
                //    return -1;

                utf8string += (char)(((char)(uchar >> 24)) | 0xF8);
                utf8string += (char)(((char)((uchar >> 18) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 12) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 6) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 0) & 0x3F)) | 0x80);
            }
            else if (uchar < 0x80000000)
            {
                /* unicode char 0x04000000 - 0x7FFFFFFF */
                //if (count < 6)
                //    return -1;

                utf8string += (char)(((char)(uchar >> 30)) | 0xFC);
                utf8string += (char)(((char)((uchar >> 24) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 18) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 12) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 6) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 0) & 0x3F)) | 0x80);
            }
            else
            {
                rc = -1;
            }

            //return rc;
            return utf8string.Length;
        }


        //-------------------------------------------------
        //  utf8_from_uchar - convert a unicode character
        //  into a UTF-8 sequence
        //-------------------------------------------------
        public static string utf8_from_uchar(char32_t uchar)
        {
            string buffer;  //char buffer[UTF8_CHAR_MAX];
            var len = utf8_from_uchar(out buffer, uchar);
            return buffer;
        }


        //int utf16_from_uchar(utf16_char *utf16string, size_t count, unicode_char uchar);
        //int utf16f_from_uchar(utf16_char *utf16string, size_t count, unicode_char uchar);


        /* misc UTF-8 helpers */

        /*-------------------------------------------------
            utf8_previous_char - return a pointer to the
            previous character in a string
        -------------------------------------------------*/

        /**
         * @fn  const char *utf8_previous_char(const char *utf8string)
         *
         * @brief   UTF 8 previous character.
         *
         * @param   utf8string  The UTF 8string.
         *
         * @return  null if it fails, else a char*.
         */
        public static int utf8_previous_char(string utf8string, int start)  // const char *utf8_previous_char(const char *utf8string)
        {
            while ((utf8string[--start] & 0xc0) == 0x80)
            {
            }

            return start;
        }


        /*-------------------------------------------------
            utf8_is_valid_string - return true if the
            given string is a properly formed sequence of
            UTF-8 characters
        -------------------------------------------------*/

        /**
         * @fn  int utf8_is_valid_string(const char *utf8string)
         *
         * @brief   UTF 8 is valid string.
         *
         * @param   utf8string  The UTF 8string.
         *
         * @return  An int.
         */
        public static bool utf8_is_valid_string(string utf8string)
        {
            int remaining_length = utf8string.Length;

            int utf8stringIdx = 0;
            while (utf8string[utf8stringIdx] != 0)
            {
                char32_t uchar = 0;
                int charlen;

                /* extract the current character and verify it */
                charlen = uchar_from_utf8(out uchar, utf8string, remaining_length);
                if (charlen <= 0 || uchar == 0 || !uchar_isvalid(uchar))
                    return false;

                /* advance */
                utf8string += charlen;
                remaining_length -= charlen;
            }

            return true;
        }


        public static bool isprint(char32_t c)
        {
            // http://www.cplusplus.com/reference/cctype/isprint/
            // https://stackoverflow.com/questions/3253247/how-do-i-detect-non-printable-characters-in-net

            //For the standard ASCII character set (used by the "C" locale), printing characters are all with an ASCII code greater than 0x1f (US), except 0x7f (DEL).
            if (c > 0x1f && c <= 0xff && c != 0x7f)
                return true;

            return false;
        }
    }
}
