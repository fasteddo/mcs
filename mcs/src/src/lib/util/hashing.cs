// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using MemoryU8 = mame.MemoryContainer<System.Byte>;
using PointerU8 = mame.Pointer<System.Byte>;
using size_t = System.UInt64;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.crc32_global;
using static mame.util.hashing_global;


namespace mame
{
    public static partial class util
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


            static uint32_t sha1_rol(uint32_t x, unsigned n)
            {
                return (x << (int)n) | (x >> (32 - (int)n));
            }

            static uint32_t sha1_b(PointerU32 data, unsigned i)
            {
                uint32_t r = data[(i + 13) & 15U];
                r ^= data[(i + 8) & 15U];
                r ^= data[(i + 2) & 15U];
                r ^= data[i & 15U];
                r = sha1_rol(r, 1);
                data[i & 15U] = r;
                return r;
            }

            static void sha1_r0(PointerU32 data, std.array<uint32_t, u64_const_5> d, unsigned i)  //inline void sha1_r0(const uint32_t *data, std::array<uint32_t, 5> &d, unsigned i)
            {
                d[i % 5] = d[i % 5] + ((d[(i + 3) % 5] & (d[(i + 2) % 5] ^ d[(i + 1) % 5])) ^ d[(i + 1) % 5]) + data[i] + 0x5a827999U + sha1_rol(d[(i + 4) % 5], 5);
                d[(i + 3) % 5] = sha1_rol(d[(i + 3) % 5], 30);
            }

            static void sha1_r1(PointerU32 data, std.array<uint32_t, u64_const_5> d, unsigned i)  //inline void sha1_r1(uint32_t *data, std::array<uint32_t, 5> &d, unsigned i)
            {
                d[i % 5] = d[i % 5] + ((d[(i + 3) % 5] & (d[(i + 2) % 5] ^ d[(i + 1) % 5])) ^ d[(i + 1) % 5])+ sha1_b(data, i) + 0x5a827999U + sha1_rol(d[(i + 4) % 5], 5);
                d[(i + 3) % 5] = sha1_rol(d[(i + 3) % 5], 30);
            }

            static void sha1_r2(PointerU32 data, std.array<uint32_t, u64_const_5> d, unsigned i)  //inline void sha1_r2(uint32_t *data, std::array<uint32_t, 5> &d, unsigned i)
            {
                d[i % 5] = d[i % 5] + (d[(i + 3) % 5] ^ d[(i + 2) % 5] ^ d[(i + 1) % 5]) + sha1_b(data, i) + 0x6ed9eba1U + sha1_rol(d[(i + 4) % 5], 5);
                d[(i + 3) % 5] = sha1_rol(d[(i + 3) % 5], 30);
            }

            static void sha1_r3(PointerU32 data, std.array<uint32_t, u64_const_5> d, unsigned i)  //inline void sha1_r3(uint32_t *data, std::array<uint32_t, 5> &d, unsigned i)
            {
                d[i % 5] = d[i % 5] + (((d[(i + 3) % 5] | d[(i + 2) % 5]) & d[(i + 1) % 5]) | (d[(i + 3) % 5] & d[(i + 2) % 5])) + sha1_b(data, i) + 0x8f1bbcdcU + sha1_rol(d[(i + 4) % 5], 5);
                d[(i + 3) % 5] = sha1_rol(d[(i + 3) % 5], 30);
            }

            static void sha1_r4(PointerU32 data, std.array<uint32_t, u64_const_5> d, unsigned i)  //inline void sha1_r4(uint32_t *data, std::array<uint32_t, 5> &d, unsigned i)
            {
                d[i % 5] = d[i % 5] + (d[(i + 3) % 5] ^ d[(i + 2) % 5] ^ d[(i + 1) % 5]) + sha1_b(data, i) + 0xca62c1d6U + sha1_rol(d[(i + 4) % 5], 5);
                d[(i + 3) % 5] = sha1_rol(d[(i + 3) % 5], 30);
            }

            public static void sha1_process(std.array<uint32_t, u64_const_5> st, PointerU32 data)  //inline void sha1_process(std::array<uint32_t, 5> &st, uint32_t *data)
            {
                std.array<uint32_t, u64_const_5> d = st;
                unsigned i = 0U;
                while (i < 16U)
                    sha1_r0(data, d, i++);
                while (i < 20U)
                    sha1_r1(data, d, i++);
                while (i < 40U)
                    sha1_r2(data, d, i++);
                while (i < 60U)
                    sha1_r3(data, d, i++);
                while (i < 80U)
                    sha1_r4(data, d, i++);
                for (i = 0U; i < 5U; i++)
                    st[i] += d[i];
            }
        }


        // ======================> SHA-1
        // final digest
        public class sha1_t
        {
            //static const sha1_t null;


            public MemoryU8 m_raw = new MemoryU8(20, true);  //uint8_t m_raw[20];


            //bool operator==(const sha1_t &rhs) const { return memcmp(m_raw, rhs.m_raw, sizeof(m_raw)) == 0; }
            //bool operator!=(const sha1_t &rhs) const { return memcmp(m_raw, rhs.m_raw, sizeof(m_raw)) != 0; }
            public static bool operator==(sha1_t left, sha1_t right) { return std.memcmp(left.m_raw, right.m_raw, (size_t)left.m_raw.Count) == 0; }
            public static bool operator!=(sha1_t left, sha1_t right) { return std.memcmp(left.m_raw, right.m_raw, (size_t)left.m_raw.Count) != 0; }

            public override bool Equals(Object obj)
            {
                if (obj == null || base.GetType() != obj.GetType()) return false;
                return this == (sha1_t)obj;
            }
            public override int GetHashCode()
            {
                int code = 0;
                foreach (var i in m_raw)
                    code ^= i.GetHashCode();
                return code;
            }


            public MemoryU8 op() { return m_raw; }  //operator uint8_t *() { return m_raw; }


            //-------------------------------------------------
            //  from_string - convert from a string
            //-------------------------------------------------
            public bool from_string(string string_)
            {
                // must be at least long enough to hold everything
                std.memset(m_raw, (uint8_t)0);

                if ((int)string_.length() < 2 * m_raw.Count)  //if (string.length() < 2 * sizeof(m_raw))
                    return false;

                // iterate through our raw buffer
                int stringIdx = 0;
                for (int i = 0; i < m_raw.Count; i++)  //for (auto & elem : m_raw)
                {
                    int upper = char_to_hex(string_[0]);
                    int lower = char_to_hex(string_[1]);
                    if (upper == -1 || lower == -1)
                        return false;

                    m_raw[i] = (uint8_t)((upper << 4) | lower);  //elem = (upper << 4) | lower;
                    string_ = string_[2..];  //string.remove_prefix(2);
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
            uint64_t m_cnt;
            std.array<uint32_t, u64_const_5> m_st = new std.array<uint32_t, u64_const_5>();  //std::array<uint32_t, 5> m_st;
            MemoryU8 m_buf = new MemoryU8(16 * 4, true);  //uint32_t m_buf[16];


            // construction/destruction
            public sha1_creator() { reset(); }


            // reset
            //-------------------------------------------------
            //  reset - prepare to digest a block of data
            //-------------------------------------------------
            void reset()
            {
                m_cnt = 0U;
                m_st[0] = 0xc3d2e1f0U;
                m_st[1] = 0x10325476U;
                m_st[2] = 0x98badcfeU;
                m_st[3] = 0xefcdab89U;
                m_st[4] = 0x67452301U;
            }


            // append data
            //-------------------------------------------------
            //  append - digest a block of data
            //-------------------------------------------------
            public void append(PointerU8 data, uint32_t length)  //void sha1_creator::append(const void *data, uint32_t length)
            {
    //#if LSB_FIRST
                unsigned swizzle = 3U;
    //#else
    //            constexpr unsigned swizzle = 0U;
    //#endif
                uint32_t residual = ((uint32_t)m_cnt >> 3) & 63U;
                m_cnt += (uint64_t)length << 3;
                uint32_t offset = 0U;
                if (length >= (64U - residual))
                {
                    if (residual != 0)
                    {
                        for (offset = 0U; (offset + residual) < 64U; offset++)
                            m_buf[(offset + residual) ^ swizzle] = data[offset];  //reinterpret_cast<uint8_t *>(m_buf)[(offset + residual) ^ swizzle] = reinterpret_cast<const uint8_t *>(data)[offset];
                        sha1_process(m_st, new PointerU32(m_buf));
                    }
                    while ((length - offset) >= 64U)
                    {
                        for (residual = 0U; residual < 64U; residual++, offset++)
                            m_buf[residual ^ swizzle] = data[offset];  //reinterpret_cast<uint8_t *>(m_buf)[residual ^ swizzle] = reinterpret_cast<const uint8_t *>(data)[offset];
                        sha1_process(m_st, new PointerU32(m_buf));
                    }
                    residual = 0U;
                }
                for ( ; offset < length; residual++, offset++)
                    m_buf[residual ^ swizzle] = data[offset];  //reinterpret_cast<uint8_t *>(m_buf)[residual ^ swizzle] = reinterpret_cast<const uint8_t *>(data)[offset];
            }


            // finalize and compute the final digest
            //-------------------------------------------------
            //  finish - compute final hash
            //-------------------------------------------------
            public sha1_t finish()
            {
                unsigned padlen = 64U - (63U & (((unsigned)m_cnt >> 3) + 8U));
                MemoryU8 padbuf = new MemoryU8(64, true);  //uint8_t padbuf[64];
                padbuf[0] = 0x80;
                for (unsigned i = 1U; i < padlen; i++)
                    padbuf[i] = 0x00;
                MemoryU8 lenbuf = new MemoryU8(8, true);  //uint8_t lenbuf[8];
                for (unsigned i = 0U; i < 8U; i++)
                    lenbuf[i] = (uint8_t)(m_cnt >> (int)((7U - i) << 3));
                append(new PointerU8(padbuf), padlen);
                append(new PointerU8(lenbuf), (uint32_t)lenbuf.Count);
                sha1_t result = new sha1_t();
                for (unsigned i = 0U; i < 20U; i++)
                    result.m_raw[i] = (uint8_t)(m_st[4U - (i >> 2)] >> (int)((3U - (i & 3)) << 3));
                return result;
            }


            // static wrapper to just get the digest from a block
            static sha1_t simple(PointerU8 data, uint32_t length)
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

            public override bool Equals(Object obj)
            {
                if (obj == null || base.GetType() != obj.GetType()) return false;
                return this == (crc32_t)obj;
            }
            public override int GetHashCode() { return m_raw.GetHashCode(); }


            public uint32_t op { get { return m_raw; } set { m_raw = value; } }  //constexpr operator uint32_t() const { return m_raw; }


            //-------------------------------------------------
            //  from_string - convert from a string
            //-------------------------------------------------
            public bool from_string(string string_)
            {
                // must be at least long enough to hold everything
                m_raw = 0;

                if (string_.length() < 2 * 4/*sizeof(m_raw)*/)
                    return false;

                // iterate through our raw buffer
                m_raw = 0;
                for (int bytenum = 0; bytenum < 4/*sizeof(m_raw)*/ * 2; bytenum++)
                {
                    int nibble = char_to_hex(string_[0]);
                    if (nibble == -1)
                        return false;

                    m_raw = (m_raw << 4) | (uint32_t)nibble;
                    string_ = string_[1..];  //string.remove_prefix(1);
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
            public void append(PointerU8 data, uint32_t length)  //void append(const void *data, uint32_t length);
            {
                m_accum.op = crc32(m_accum.op, data, length);
            }

            // finalize and compute the final digest
            public crc32_t finish() { return m_accum; }

            // static wrapper to just get the digest from a block
            static crc32_t simple(PointerU8 data, uint32_t length)  //static crc32_t simple(const void *data, uint32_t length)
            {
                crc32_creator creator = new crc32_creator();
                creator.append(data, length);
                return creator.finish();
            }
        }
    }
}
