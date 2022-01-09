// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using char32_t = System.UInt32;
using size_t = System.UInt64;

using static mame.unicode_global;


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


        public enum unicode_normalization_form { C, D, KC, KD };


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

        //-------------------------------------------------
        //  uchar_from_utf8 - convert a UTF-8 sequence
        //  into a unicode character
        //-----------------------------------------------
        public static int uchar_from_utf8(out char uchar, string utf8str)
        {
            return uchar_from_utf8(out uchar, utf8str, (size_t)utf8str.length());
        }

        public static int uchar_from_utf8(out char uchar, string utf8char, size_t count)
        {
            //throw new emu_unimplemented();
#if false
#endif

            uchar = utf8char[0];
            return 1;
        }

        //int uchar_from_utf8(char32_t *uchar, std::string_view utf8str);
        //int uchar_from_utf16(unicode_char *uchar, const utf16_char *utf16char, size_t count);
        //int uchar_from_utf16f(unicode_char *uchar, const utf16_char *utf16char, size_t count);

        //-------------------------------------------------
        //  ustr_from_utf8 - convert a UTF-8 sequence into
        //  into a Unicode string
        //-------------------------------------------------
        public static string ustr_from_utf8(string utf8str) { return utf8str; }

        // converting UTF-8 strings to/from "wide" strings
        public static string wstring_from_utf8(string utf8string) { return utf8string; }  //std::wstring wstring_from_utf8(const std::string &utf8string);

        //std::string utf8_from_wstring(const std::wstring &string);

        // unicode normalization
        //-------------------------------------------------
        //  normalize_unicode - uses utf8proc to normalize
        //  unicode
        //-------------------------------------------------
        //std::string normalize_unicode(const char *s, unicode_normalization_form normalization_form, bool fold_case = false);
        public static string normalize_unicode(string s, unicode_normalization_form normalization_form, bool fold_case = false) { return s; }  //std::string normalize_unicode(std::string_view s, unicode_normalization_form normalization_form, bool fold_case = false);


        /* converting 32-bit Unicode chars to strings */

        /*-------------------------------------------------
            utf8_from_uchar - convert a unicode character
            into a UTF-8 sequence
        -------------------------------------------------*/
        public static int utf8_from_uchar(out string utf8string, /*size_t count,*/ char32_t uchar)
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
                rc++;
            }
            else if (uchar < 0x800)
            {
                /* unicode char 0x00000080 - 0x000007FF */
                //if (count < 2)
                //    return -1;

                utf8string += (char)(((char)(uchar >> 6)) | 0xC0);
                utf8string += (char)(((char) ((uchar >> 0) & 0x3F)) | 0x80);
                rc++;
                rc++;
            }
            else if (uchar < 0x10000)
            {
                /* unicode char 0x00000800 - 0x0000FFFF */
                //if (count < 3)
                //    return -1;

                utf8string += (char)(((char)(uchar >> 12)) | 0xE0);
                utf8string += (char)(((char)((uchar >> 6) & 0x3F)) | 0x80);
                utf8string += (char)(((char)((uchar >> 0) & 0x3F)) | 0x80);
                rc++;
                rc++;
                rc++;
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
                rc++;
                rc++;
                rc++;
                rc++;
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
                rc++;
                rc++;
                rc++;
                rc++;
                rc++;
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
                rc++;
                rc++;
                rc++;
                rc++;
                rc++;
                rc++;
            }
            else
            {
                rc = -1;
            }

            return rc;
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
                char uchar = (char)0;  //char32_t uchar = 0;
                int charlen;

                /* extract the current character and verify it */
                charlen = uchar_from_utf8(out uchar, utf8string, (size_t)remaining_length);
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
