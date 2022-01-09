// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using endianness_t = mame.util.endianness;  //using endianness_t = util::endianness;
using offs_t = System.UInt32;  //using offs_t = u32;
using PointerU8 = mame.Pointer<System.Byte>;
using u16 = System.UInt16;
using u32 = System.UInt32;

using static mame.cpp_global;
using static mame.emucore_global;
using static mame.emumem_global;


namespace mame
{
    // ======================> memory_array
    // memory information
    public class memory_array
    {
        // internal state
        PointerU8 m_base;  //void *              m_base;
        u32 m_bytes;
        int m_membits;
        endianness_t m_endianness;
        int m_bytes_per_entry;
        Func<int, u32> m_read_entry;  //u32 (memory_array::*m_read_entry)(int) const;
        Action<int, u32> m_write_entry;  //void (memory_array::*m_write_entry)(int, u32);


        // construction/destruction
        //-------------------------------------------------
        //  memory_array - constructor
        //-------------------------------------------------
        public memory_array()
        {
            m_base = null;
            m_bytes = 0;
            m_membits = 0;
            m_endianness = ENDIANNESS_LITTLE;
            m_bytes_per_entry = 0;
            m_read_entry = null;
            m_write_entry = null;
        }


        //memory_array(void *base, u32 bytes, int membits, endianness_t endianness, int bpe) { set(base, bytes, membits, endianness, bpe); }
        //template <typename _Type> memory_array(std::vector<_Type> &array, endianness_t endianness, int bpe) { set(array, endianness, bpe); }
        //memory_array(const address_space &space, void *base, u32 bytes, int bpe) { set(space, base, bytes, bpe); }
        //memory_array(const memory_share &share, int bpe) { set(share, bpe); }
        //memory_array(const memory_array &array) { set(array); }


        // configuration
        //-------------------------------------------------
        //  set - configure the parameters
        //-------------------------------------------------
        public void set(PointerU8 base_, u32 bytes, int membits, endianness_t endianness, int bpe)  //void set(void *base, u32 bytes, int membits, endianness_t endianness, int bpe);
        {
            // validate inputs
            assert(base_ != null);
            assert(bytes > 0);
            assert(membits == 8 || membits == 16 || membits == 32 || membits == 64);
            assert(bpe == 1 || bpe == 2 || bpe == 4);

            // populate direct data
            m_base = base_;
            m_bytes = bytes;
            m_membits = membits;
            m_endianness = endianness;
            m_bytes_per_entry = bpe;

            // derive data
            switch (bpe * 1000 + membits * 10 + (endianness == ENDIANNESS_LITTLE ? 0 : 1))
            {
                case 1 * 1000 +  8 * 10 + 0:    m_read_entry = read8_from_8;       m_write_entry = write8_to_8;        break;
                case 1 * 1000 +  8 * 10 + 1:    m_read_entry = read8_from_8;       m_write_entry = write8_to_8;        break;
                case 1 * 1000 + 16 * 10 + 0:    m_read_entry = read8_from_16le;    m_write_entry = write8_to_16le;     break;
                case 1 * 1000 + 16 * 10 + 1:    m_read_entry = read8_from_16be;    m_write_entry = write8_to_16be;     break;
                case 1 * 1000 + 32 * 10 + 0:    m_read_entry = read8_from_32le;    m_write_entry = write8_to_32le;     break;
                case 1 * 1000 + 32 * 10 + 1:    m_read_entry = read8_from_32be;    m_write_entry = write8_to_32be;     break;
                case 1 * 1000 + 64 * 10 + 0:    m_read_entry = read8_from_64le;    m_write_entry = write8_to_64le;     break;
                case 1 * 1000 + 64 * 10 + 1:    m_read_entry = read8_from_64be;    m_write_entry = write8_to_64be;     break;

                case 2 * 1000 +  8 * 10 + 0:    m_read_entry = read16_from_8le;    m_write_entry = write16_to_8le;     break;
                case 2 * 1000 +  8 * 10 + 1:    m_read_entry = read16_from_8be;    m_write_entry = write16_to_8be;     break;
                case 2 * 1000 + 16 * 10 + 0:    m_read_entry = read16_from_16;     m_write_entry = write16_to_16;      break;
                case 2 * 1000 + 16 * 10 + 1:    m_read_entry = read16_from_16;     m_write_entry = write16_to_16;      break;
                case 2 * 1000 + 32 * 10 + 0:    m_read_entry = read16_from_32le;   m_write_entry = write16_to_32le;    break;
                case 2 * 1000 + 32 * 10 + 1:    m_read_entry = read16_from_32be;   m_write_entry = write16_to_32be;    break;
                case 2 * 1000 + 64 * 10 + 0:    m_read_entry = read16_from_64le;   m_write_entry = write16_to_64le;    break;
                case 2 * 1000 + 64 * 10 + 1:    m_read_entry = read16_from_64be;   m_write_entry = write16_to_64be;    break;

                case 4 * 1000 +  8 * 10 + 0:    m_read_entry = read32_from_8le;    m_write_entry = write32_to_8le;     break;
                case 4 * 1000 +  8 * 10 + 1:    m_read_entry = read32_from_8be;    m_write_entry = write32_to_8be;     break;
                case 4 * 1000 + 16 * 10 + 0:    m_read_entry = read32_from_16le;   m_write_entry = write32_to_16le;    break;
                case 4 * 1000 + 16 * 10 + 1:    m_read_entry = read32_from_16be;   m_write_entry = write32_to_16be;    break;
                case 4 * 1000 + 32 * 10 + 0:    m_read_entry = read32_from_32;     m_write_entry = write32_to_32;      break;
                case 4 * 1000 + 32 * 10 + 1:    m_read_entry = read32_from_32;     m_write_entry = write32_to_32;      break;
                case 4 * 1000 + 64 * 10 + 0:    m_read_entry = read32_from_64le;   m_write_entry = write32_to_64le;    break;
                case 4 * 1000 + 64 * 10 + 1:    m_read_entry = read32_from_64be;   m_write_entry = write32_to_64be;    break;

                default:    throw new emu_fatalerror("Illegal memory bits/bus width combo in memory_array");
            }
        }


        //-------------------------------------------------
        //  set - additional setter variants
        //-------------------------------------------------
        //template <typename _Type> void set(std::vector<_Type> &array, endianness_t endianness, int bpe) { set(&array[0], array.size(), 8*sizeof(_Type), endianness, bpe); }
        public void set(address_space space, PointerU8 base_, u32 bytes, int bpe) { set(base_, bytes, space.data_width(), space.endianness(), bpe); }  //void set(const address_space &space, void *base, u32 bytes, int bpe);
        public void set(memory_share share, int bpe) { set(share.ptr(), (u32)share.bytes(), share.bitwidth(), share.endianness(), bpe); }
        public void set(memory_array array) { set(array.base_(), array.bytes(), array.membits(), array.endianness(), array.bytes_per_entry()); }


        // piecewise configuration
        //-------------------------------------------------
        //  piecewise configuration
        //-------------------------------------------------
        public void set_membits(int membits) { set(m_base, m_bytes, membits, m_endianness, m_bytes_per_entry); }
        public void set_endianness(endianness_t endianness) { set(m_base, m_bytes, m_membits, endianness, m_bytes_per_entry); }


        // getters
        public PointerU8 base_() { return m_base; }  //void *base() const { return m_base; }
        u32 bytes() { return m_bytes; }
        public int membits() { return m_membits; }
        endianness_t endianness() { return m_endianness; }
        public int bytes_per_entry() { return m_bytes_per_entry; }


        // entry-level readers and writers
        public u32 read(int index) { return m_read_entry(index); }
        //void write(int index, u32 data) { (this->*m_write_entry)(index, data); }


        // byte/word/dword-level readers and writers
        //u8 read8(offs_t offset) const { return reinterpret_cast<u8 *>(m_base)[offset]; }
        //u16 read16(offs_t offset) const { return reinterpret_cast<u16 *>(m_base)[offset]; }
        //u32 read32(offs_t offset) const { return reinterpret_cast<u32 *>(m_base)[offset]; }
        //u64 read64(offs_t offset) const { return reinterpret_cast<u64 *>(m_base)[offset]; }
        //void write8(offs_t offset, u8 data) { reinterpret_cast<u8 *>(m_base)[offset] = data; }
        public void write16(offs_t offset, u16 data, u16 mem_mask = 0xffff) { var temp = m_base.GetUInt16((int)offset); COMBINE_DATA(ref temp, data, mem_mask); m_base.SetUInt16((int)offset, temp); }  //{ COMBINE_DATA(&reinterpret_cast<u16 *>(m_base)[offset]); }
        //void write32(offs_t offset, u32 data, u32 mem_mask = 0xffffffff) { COMBINE_DATA(&reinterpret_cast<u32 *>(m_base)[offset]); }
        //void write64(offs_t offset, u64 data, u64 mem_mask = 0xffffffffffffffffU) { COMBINE_DATA(&reinterpret_cast<u64 *>(m_base)[offset]); }


        // internal read/write helpers for 1 byte entries
        u32 read8_from_8(int index) { throw new emu_unimplemented(); }     void write8_to_8(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read8_from_16le(int index) { throw new emu_unimplemented(); }  void write8_to_16le(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read8_from_16be(int index) { throw new emu_unimplemented(); }  void write8_to_16be(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read8_from_32le(int index) { throw new emu_unimplemented(); }  void write8_to_32le(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read8_from_32be(int index) { throw new emu_unimplemented(); }  void write8_to_32be(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read8_from_64le(int index) { throw new emu_unimplemented(); }  void write8_to_64le(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read8_from_64be(int index) { throw new emu_unimplemented(); }  void write8_to_64be(int index, u32 data) { throw new emu_unimplemented(); }


        // internal read/write helpers for 2 byte entries
        //-------------------------------------------------
        //  read16_from_*/write16_to_* - entry read/write
        //  heleprs for 2 bytes-per-entry
        //-------------------------------------------------
        u32 read16_from_8le(int index) { throw new emu_unimplemented(); }  void write16_to_8le(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read16_from_8be(int index) { throw new emu_unimplemented(); }  void write16_to_8be(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read16_from_16(int index) { return new PointerU16(m_base)[index]; }  //u32 memory_array::read16_from_16(int index) const { return reinterpret_cast<u16 *>(m_base)[index]; }
        void write16_to_16(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read16_from_32le(int index) { throw new emu_unimplemented(); } void write16_to_32le(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read16_from_32be(int index) { throw new emu_unimplemented(); } void write16_to_32be(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read16_from_64le(int index) { throw new emu_unimplemented(); } void write16_to_64le(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read16_from_64be(int index) { throw new emu_unimplemented(); } void write16_to_64be(int index, u32 data) { throw new emu_unimplemented(); }


        // internal read/write helpers for 4 byte entries
        u32 read32_from_8le(int index) { throw new emu_unimplemented(); }  void write32_to_8le(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read32_from_8be(int index) { throw new emu_unimplemented(); }  void write32_to_8be(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read32_from_16le(int index) { throw new emu_unimplemented(); } void write32_to_16le(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read32_from_16be(int index) { throw new emu_unimplemented(); } void write32_to_16be(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read32_from_32(int index) { throw new emu_unimplemented(); }   void write32_to_32(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read32_from_64le(int index) { throw new emu_unimplemented(); } void write32_to_64le(int index, u32 data) { throw new emu_unimplemented(); }
        u32 read32_from_64be(int index) { throw new emu_unimplemented(); } void write32_to_64be(int index, u32 data) { throw new emu_unimplemented(); }
    }
}
