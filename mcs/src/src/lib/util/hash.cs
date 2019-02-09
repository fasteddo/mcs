// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using ListBytesPointer = mame.ListPointer<System.Byte>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame.util
{
    public static class hash_global
    {
        // use these to define compile-time internal-format hash strings
        public static string CRC(string x) { return "R" + x; }  //#define CRC(x)              "R" #x
        public static string SHA1(string x) { return "S" + x; }  //#define SHA1(x)             "S" #x
        public const string NO_DUMP = "!";
        //#define BAD_DUMP            "^"
    }


    // ======================> hash_collection
    // a collection of the various supported hashes and flags
    public class hash_collection : global_object
    {
        // hash types are identified by non-hex alpha values (G-Z)
        public const char HASH_CRC = 'R';
        public const char HASH_SHA1 = 'S';

        // common combinations for requests
        public const string HASH_TYPES_CRC = "R";
        public const string HASH_TYPES_CRC_SHA1 = "RS";
        public const string HASH_TYPES_ALL = "RS";

        // flags are identified by punctuation marks
        public const char FLAG_NO_DUMP = '!';
        public const char FLAG_BAD_DUMP = '^';


        // internal state
        string m_flags = "";
        bool m_has_crc32;
        crc32_t m_crc32 = new crc32_t();
        bool m_has_sha1;
        sha1_t m_sha1 = new sha1_t();

        // creators
        class hash_creator
        {
            public bool m_doing_crc32;
            public crc32_creator m_crc32_creator = new crc32_creator();
            public bool m_doing_sha1;
            public sha1_creator m_sha1_creator = new sha1_creator();
        }
        hash_creator m_creator;


        // construction/destruction
        //-------------------------------------------------
        //  hash_collection - constructor
        //-------------------------------------------------
        public hash_collection()
        {
        }

        public hash_collection(string str)
        {
            from_internal_string(str);
        }

        public hash_collection(hash_collection src)
        {
            copyfrom(src);
        }


        // operators
        //hash_collection &operator=(const hash_collection &src);
        //bool operator==(const hash_collection &rhs) const;
        //bool operator!=(const hash_collection &rhs) const { return !(*this == rhs); }


        // getters
        public bool flag(char flag) { return m_flags.IndexOf(flag, 0) != -1; }

        //-------------------------------------------------
        //  hash_types - return a list of hash types as
        //  a string
        //-------------------------------------------------
        public string hash_types()
        {
            string buffer = "";
            if (m_has_crc32)
                buffer += HASH_CRC;
            if (m_has_sha1)
                buffer += HASH_SHA1;

            return buffer;
        }


        // hash manipulators

        //-------------------------------------------------
        //  reset - reset the hash collection to an empty
        //  set of hashes and flags
        //-------------------------------------------------
        public void reset()
        {
            m_flags = "";
            m_has_crc32 = false;
            m_has_sha1 = false;
            m_creator = null;
        }

        //bool add_from_string(char type, const char *buffer, int length = -1);
        //bool remove(char type);

        // CRC-specific helpers
        public bool crc(out uint32_t result) { result = m_crc32.op; return m_has_crc32; }
        public void add_crc(uint32_t crc) { m_crc32.op = crc; m_has_crc32 = true; }


        // SHA1-specific helpers
        //bool sha1(sha1_t &result) const { result = m_sha1; return m_has_sha1; }
        public void add_sha1(sha1_t sha1) { m_has_sha1 = true; m_sha1 = sha1; }


        // string conversion
        //string internal_string() const;

        //-------------------------------------------------
        //  macro_string - convert set of hashes and
        //  flags to a string in the macroized format
        //-------------------------------------------------
        public string macro_string()
        {
            string buffer = "";

            // handle CRCs
            if (m_has_crc32)
                buffer += string.Format("CRC({0}) ", m_crc32.as_string());

            // handle SHA1s
            if (m_has_sha1)
                buffer += string.Format("SHA1({0}) ", m_sha1.as_string());

            // append flags
            if (flag(FLAG_NO_DUMP))
                buffer += "NO_DUMP ";
            if (flag(FLAG_BAD_DUMP))
                buffer += "BAD_DUMP ";

            return buffer.Trim();
        }

        //string attribute_string() const;

        //-------------------------------------------------
        //  from_internal_string - convert an internal
        //  compact string to set of hashes and flags
        //-------------------------------------------------
        public bool from_internal_string(string str)
        {
            assert(str != null);

            // start fresh
            reset();

            // determine the end of the string
            int stringendIndex = str.Length;
            int ptrIndex = 0;

            // loop until we hit it
            bool errors = false;
            int skip_digits = 0;
            while (ptrIndex < stringendIndex)
            {
                char c = str[ptrIndex++];
                char uc = char.ToUpper(c);

                // non-hex alpha values specify a hash type
                if (uc >= 'G' && uc <= 'Z')
                {
                    skip_digits = 0;

                    if (uc == HASH_CRC)
                    {
                        m_has_crc32 = true;
                        errors = !m_crc32.from_string(str.Remove(0, ptrIndex), stringendIndex - ptrIndex);  // ptr, stringend - ptr);
                        skip_digits = 2 * 4/*sizeof(crc32_t)*/;
                    }
                    else if (uc == HASH_SHA1)
                    {
                        m_has_sha1 = true;
                        errors = !m_sha1.from_string(str.Remove(0, ptrIndex), stringendIndex - ptrIndex);
                        skip_digits = 2 * 4/*sizeof(sha1_t)*/;
                    }
                    else
                    {
                        errors = true;
                    }
                }

                // hex values are ignored, though unexpected
                else if ((uc >= '0' && uc <= '9') || (uc >= 'A' && uc <= 'F'))
                {
                    if (skip_digits != 0)
                        skip_digits--;
                    else
                        errors = true;
                }

                // anything else is a flag
                else if (skip_digits != 0)
                    errors = true;
                else
                    m_flags += c;
            }

            return !errors;
        }


        // creation

        //-------------------------------------------------
        //  begin - begin hashing
        //-------------------------------------------------
        void begin(string types = null)
        {
            // nuke previous creator and make a new one
            //delete m_creator;
            m_creator = new hash_creator();

            // by default use all types
            if (string.IsNullOrEmpty(types))
            {
                m_creator.m_doing_crc32 = true;
                m_creator.m_doing_sha1 = true;
            }
            // otherwise, just allocate the ones that are specified
            else
            {
                m_creator.m_doing_crc32 = types.IndexOf(HASH_CRC) != -1;
                m_creator.m_doing_sha1 = types.IndexOf(HASH_SHA1) != -1;
            }
        }

        //-------------------------------------------------
        //  buffer - add the given buffer to the hash
        //-------------------------------------------------
        void buffer(ListBytesPointer data, uint32_t length)  //const uint8_t *data
        {
            assert(m_creator != null);

            // append to each active hash
            if (m_creator.m_doing_crc32)
                m_creator.m_crc32_creator.append(data, length);
            if (m_creator.m_doing_sha1)
                m_creator.m_sha1_creator.append(data, length);
        }

        //-------------------------------------------------
        //  end - stop hashing
        //-------------------------------------------------
        void end()
        {
            //assert(m_creator != NULL);

            // finish up the CRC32
            if (m_creator.m_doing_crc32)
            {
                m_has_crc32 = true;
                m_crc32 = m_creator.m_crc32_creator.finish();
            }

            // finish up the SHA1
            if (m_creator.m_doing_sha1)
            {
                m_has_sha1 = true;
                m_sha1 = m_creator.m_sha1_creator.finish();
            }

            // nuke the creator
            //delete m_creator;
            m_creator = null;
        }

        public void compute(ListBytesPointer data, uint32_t length, string types = null) { begin(types); buffer(data, length); end(); }  //const uint8_t *data


        // internal helpers
        //-------------------------------------------------
        //  copyfrom - copy everything from another
        //  collection
        //-------------------------------------------------
        void copyfrom(hash_collection src)
        {
            // copy flags directly
            m_flags = src.m_flags;

            // copy hashes
            m_has_crc32 = src.m_has_crc32;
            m_crc32 = src.m_crc32;
            m_has_sha1 = src.m_has_sha1;
            m_sha1 = src.m_sha1;

            // don't copy creators
            m_creator = null;
        }
    }
}
