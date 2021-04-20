// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;


namespace mame.util
{
    public static class hashing_global
    {
        //-------------------------------------------------
        //  char_to_hex - return the hex value of a
        //  character
        //-------------------------------------------------
        public static int char_to_hex(char c)
        {
            if (c >= '0' && c <= '9')
                return c - '0';
            if (c >= 'a' && c <= 'f')
                return 10 + c - 'a';
            if (c >= 'A' && c <= 'F')
                return 10 + c - 'A';
            return -1;
        }
    }


    // ======================> SHA-1
    // final digest
    public class sha1_t : global_object
    {
        //static const sha1_t null;


        MemoryU8 m_raw = new MemoryU8();  //uint8_t m_raw[20];


        public sha1_t() { m_raw.Resize(20); }


        //bool operator==(const sha1_t &rhs) const { return memcmp(m_raw, rhs.m_raw, sizeof(m_raw)) == 0; }
        //bool operator!=(const sha1_t &rhs) const { return memcmp(m_raw, rhs.m_raw, sizeof(m_raw)) != 0; }
        public static bool operator==(sha1_t left, sha1_t right) { return memcmp(left.m_raw, right.m_raw, (UInt32)left.m_raw.Count) == 0; }
        public static bool operator!=(sha1_t left, sha1_t right) { return memcmp(left.m_raw, right.m_raw, (UInt32)left.m_raw.Count) != 0; }

        public MemoryU8 op() { return m_raw; }  //operator UINT8 *() { return m_raw; }


        //-------------------------------------------------
        //  from_string - convert from a string
        //-------------------------------------------------
        public bool from_string(string string_, int length = -1)
        {
            // must be at least long enough to hold everything
            memset(m_raw, (uint8_t)0);

            if (length == -1)
                length = strlen(string_);

            if (length < 2 * m_raw.Count)  //if (length < 2 * sizeof(m_raw))
                return false;

            // iterate through our raw buffer
            int stringIdx = 0;
            for (int i = 0; i < m_raw.Count; i++)  //for (auto & elem : m_raw)
            {
                int upper = hashing_global.char_to_hex(string_[stringIdx++]);
                int lower = hashing_global.char_to_hex(string_[stringIdx++]);
                if (upper == -1 || lower == -1)
                    return false;

                m_raw[i] = (byte)((upper << 4) | lower);
            }

            return true;
        }

        //-------------------------------------------------
        //  as_string - convert to a string
        //-------------------------------------------------
        public string as_string()
        {
            string ret = "";
            for (int i = 0; i < m_raw.Count; i++)
                ret += string.Format("{0:x2}", m_raw[i]); // "%02x", m_raw[i]);

            return ret;
        }
    }


    // creation helper
    class sha1_creator
    {
        // internal state
        sha1_ctx m_context;      // internal context


        // construction/destruction
        public sha1_creator() { reset(); }

        // reset
        void reset() { sha1_global.sha1_init(out m_context); }

        // append data
        public void append(Pointer<uint8_t> data, uint32_t length) { sha1_global.sha1_update(m_context, length, data); }
    
        // finalize and compute the final digest
        public sha1_t finish()
        {
            sha1_t result = new sha1_t();
            sha1_global.sha1_final(m_context);
            sha1_global.sha1_digest(m_context, (UInt32)result.op().Count, new Pointer<uint8_t>(result.op()));
            return result;
        }

        // static wrapper to just get the digest from a block
        static sha1_t simple(Pointer<uint8_t> data, uint32_t length)
        {
            sha1_creator creator = new sha1_creator();
            creator.append(data, length);
            return creator.finish();
        }
    }


    // ======================> CRC-32

    // final digest
    struct crc32_t
    {
        //static const crc32_t null;


        uint32_t m_raw;


        //crc32_t() { }
        //constexpr crc32_t(const crc32_t &rhs) = default;
        //constexpr crc32_t(const uint32_t crc) : m_raw(crc) { }

        //constexpr bool operator==(const crc32_t &rhs) const { return m_raw == rhs.m_raw; }
        //constexpr bool operator!=(const crc32_t &rhs) const { return m_raw != rhs.m_raw; }
        public static bool operator==(crc32_t left, crc32_t right) { return left.m_raw == right.m_raw; }
        public static bool operator!=(crc32_t left, crc32_t right) { return left.m_raw != right.m_raw; }

        //crc32_t &operator=(const crc32_t &rhs) = default;
        //crc32_t &operator=(const uint32_t crc) { m_raw = crc; return *this; }

        public uint32_t op { get { return m_raw; } set { m_raw = value; } }  //constexpr operator uint32_t() const { return m_raw; }


        //-------------------------------------------------
        //  from_string - convert from a string
        //-------------------------------------------------
        public bool from_string(string str, int length = -1)
        {
            int strIndex = 0;

            // must be at least long enough to hold everything
            m_raw = 0;
            if (length == -1)
                length = str.Length;

            if (length < 2 * 4/*sizeof(m_raw)*/)
                return false;

            // iterate through our raw buffer
            m_raw = 0;
            for (int bytenum = 0; bytenum < 4/*sizeof(m_raw)*/ * 2; bytenum++)
            {
                int nibble = hashing_global.char_to_hex(str[strIndex++]);
                if (nibble == -1)
                    return false;

                m_raw = (m_raw << 4) | (UInt32)nibble;
            }

            return true;
        }

        //-------------------------------------------------
        //  as_string - convert to a string
        //-------------------------------------------------
        public string as_string()
        {
            return string.Format("{0:x8}", m_raw);  // %08x", m_raw);
        }
    }


    // creation helper
    class crc32_creator
    {
        // internal state
        crc32_t m_accum;        // internal accumulator


        // construction/destruction
        public crc32_creator() { reset(); }

        // reset
        void reset() { m_accum.op = 0; }

        // append data
        //-------------------------------------------------
        //  append - hash a block of data, appending to
        //  the currently-accumulated value
        //-------------------------------------------------
        public void append(Pointer<uint8_t> data, uint32_t length)  //void append(const void *data, uint32_t length);
        {
            m_accum.op = crc32_global.crc32(m_accum.op, data, length);
        }

        // finalize and compute the final digest
        public crc32_t finish() { return m_accum; }

        // static wrapper to just get the digest from a block
        static crc32_t simple(Pointer<uint8_t> data, uint32_t length)  //static crc32_t simple(const void *data, uint32_t length)
        {
            crc32_creator creator = new crc32_creator();
            creator.append(data, length);
            return creator.finish();
        }
    }
}
