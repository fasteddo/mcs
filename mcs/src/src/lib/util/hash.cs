// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using size_t = System.UInt64;
using uint8_t = System.Byte;
using uint32_t = System.UInt32;
using uint64_t = System.UInt64;
using unsigned = System.UInt32;

using static mame.cpp_global;
using static mame.hash_global;


namespace mame
{
    public static class hash_global
    {
        // use these to define compile-time internal-format hash strings
        public static string CRC(string x) { return "R" + x; }  //#define CRC(x)              "R" #x
        public static string SHA1(string x) { return "S" + x; }  //#define SHA1(x)             "S" #x
        public const string NO_DUMP = "!";
        //#define BAD_DUMP            "^"
    }


    public static partial class util
    {
        // ======================> hash_collection
        // a collection of the various supported hashes and flags
        public class hash_collection
        {
            class hash_creator
            {
                crc32_creator m_crc32_creator;  //std::optional<crc32_creator> m_crc32_creator;
                sha1_creator m_sha1_creator;  //std::optional<sha1_creator> m_sha1_creator;


                // constructor
                public hash_creator(bool doing_crc32, bool doing_sha1)
                {
                    if (doing_crc32)
                        m_crc32_creator = new crc32_creator();  //m_crc32_creator.emplace();
                    if (doing_sha1)
                        m_sha1_creator = new sha1_creator();  //m_sha1_creator.emplace();
                }


                // add the given buffer to the hash
                public void append(Pointer<uint8_t> buffer, size_t length)  //void append(void const *buffer, std::size_t length)
                {
                    // append to each active hash
                    if (m_crc32_creator != null)
                        m_crc32_creator.append(buffer, (uint32_t)length);
                    if (m_sha1_creator != null)
                        m_sha1_creator.append(buffer, (uint32_t)length);
                }


                // stop hashing
                public void finish(hash_collection hashes)
                {
                    // finish up the CRC32
                    if (m_crc32_creator != null)
                    {
                        hashes.add_crc(m_crc32_creator.finish().op);
                        m_crc32_creator = null;  //m_crc32_creator.reset();
                    }

                    // finish up the SHA1
                    if (m_sha1_creator != null)
                    {
                        hashes.add_sha1(m_sha1_creator.finish());
                        m_sha1_creator = null;  //m_sha1_creator.reset();
                    }
                }
            }


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


            // construction/destruction
            //-------------------------------------------------
            //  hash_collection - constructor
            //-------------------------------------------------
            public hash_collection()
            {
                m_has_crc32 = false;
                m_has_sha1 = false;
            }

            public hash_collection(string str) : this() { from_internal_string(str); }
            public hash_collection(hash_collection src) : this() { copyfrom(src); }


            // operators
            //hash_collection &operator=(const hash_collection &src);
            //bool operator==(const hash_collection &rhs) const;
            //bool operator!=(const hash_collection &rhs) const { return !(*this == rhs); }
            public static bool operator==(hash_collection left, hash_collection right)
            {
                // match CRCs
                int matches = 0;
                if (left.m_has_crc32 && right.m_has_crc32)
                {
                    if (left.m_crc32 != right.m_crc32)
                        return false;
                    matches++;
                }

                // match SHA1s
                if (left.m_has_sha1 && right.m_has_sha1)
                {
                    if (left.m_sha1 != right.m_sha1)
                        return false;
                    matches++;
                }

                // if all shared hashes match, return true
                return matches > 0;
            }
            public static bool operator!=(hash_collection left, hash_collection right) { return !(left == right); }

            public override bool Equals(Object obj)
            {
                if (obj == null || base.GetType() != obj.GetType()) return false;
                return this == (hash_collection)obj;
            }
            public override int GetHashCode() { return m_crc32.GetHashCode() ^ m_sha1.GetHashCode(); }


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
            }

            //bool add_from_string(char type, std::string_view string);
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

                // remove trailing space
                //if (!buffer.empty())
                //{
                //    assert(buffer.back() == ' ');
                //    buffer = buffer.substr(0, buffer.length() - 1);
                //}
                return buffer.Trim();
            }


            //-------------------------------------------------
            //  attribute_string - convert set of hashes and
            //  flags to a string in XML attribute format
            //-------------------------------------------------
            public string attribute_string()
            {
                string buffer = "";

                // handle CRCs
                if (m_has_crc32)
                    buffer = buffer.append_("crc=\"").append_(m_crc32.as_string()).append_("\" ");

                // handle SHA1s
                if (m_has_sha1)
                    buffer = buffer.append_("sha1=\"").append_(m_sha1.as_string()).append_("\" ");

                // append flags
                if (flag(FLAG_NO_DUMP))
                    buffer = buffer.append_("status=\"nodump\"");
                if (flag(FLAG_BAD_DUMP))
                    buffer = buffer.append_("status=\"baddump\"");

                // remove trailing space
                if (!buffer.empty() && buffer.back() == ' ')
                    buffer = buffer.substr(0, buffer.length() - 1);

                return buffer;
            }


            //-------------------------------------------------
            //  from_internal_string - convert an internal
            //  compact string to set of hashes and flags
            //-------------------------------------------------
            public bool from_internal_string(string string_)
            {
                // start fresh
                reset();

                // loop until we hit the end of the string
                bool errors = false;
                int skip_digits = 0;
                while (!string_.empty())
                {
                    char c = string_[0];
                    char uc = char.ToUpper(c);
                    string_ = string_[1..];  //string.remove_prefix(1);

                    // non-hex alpha values specify a hash type
                    if (uc >= 'G' && uc <= 'Z')
                    {
                        skip_digits = 0;

                        if (uc == HASH_CRC)
                        {
                            m_has_crc32 = true;
                            errors = !m_crc32.from_string(string_);
                            skip_digits = 2 * 4;  //skip_digits = 2 * sizeof(crc32_t);
                        }
                        else if (uc == HASH_SHA1)
                        {
                            m_has_sha1 = true;
                            errors = !m_sha1.from_string(string_);
                            skip_digits = 2 * 4;  //skip_digits = 2 * sizeof(sha1_t);
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
            //  compute - hash a block of data
            //-------------------------------------------------
            public void compute(Pointer<uint8_t> data, uint32_t length, string types = null)  //void compute(const uint8_t *data, uint32_t length, const char *types = nullptr);
            {
                // begin
                hash_creator creator = create(types);

                // run the hashes
                creator.append(data, length);

                // end
                creator.finish(this);
            }


            //-------------------------------------------------
            //  compute - hash data from a stream
            //-------------------------------------------------
            public std.error_condition compute(random_read stream, uint64_t offset, size_t length, out size_t actual, string types = null)  //std::error_condition compute(random_read &stream, uint64_t offset, size_t length, size_t &actual, const char *types = nullptr);
            {
                // begin
                hash_creator creator = create(types);

                // local buffer of arbitrary size
                Pointer<uint8_t> buffer = new Pointer<uint8_t>(new MemoryContainer<uint8_t>(2048, true));  //uint8_t buffer[2048];

                // run the hashes
                actual = 0U;
                while (length != 0)
                {
                    // determine the size of the next chunk
                    unsigned chunk_length = (unsigned)std.min((int)length, buffer.Count);

                    // read one chunk
                    size_t bytes_read;
                    std.error_condition err = stream.read_at(offset, buffer, chunk_length, out bytes_read);
                    if (err)
                        return err;

                    if (bytes_read == 0) // EOF?
                        break;

                    offset += bytes_read;
                    length -= chunk_length;

                    // append the chunk
                    creator.append(buffer, bytes_read);
                    actual += bytes_read;
                }

                // end
                creator.finish(this);
                return new std.error_condition();
            }


            // internal helpers
            //-------------------------------------------------
            //  create - begin hashing
            //-------------------------------------------------
            hash_creator create(string types = null)
            {
                // by default use all types
                if (types == null)
                    return new hash_creator(true, true);

                // otherwise, just allocate the ones that are specified
                else
                    return new hash_creator(std.strchr(types, HASH_CRC) != -1, std.strchr(types, HASH_SHA1) != -1);
            }


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
            }
        }
    }
}
