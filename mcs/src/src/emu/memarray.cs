// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u32 = System.UInt32;


namespace mame
{
    // ======================> memory_array
    // memory information
    public class memory_array
    {
        delegate u32 read_entry(int value);


        // internal state
        object m_base;  //void *              m_base;
        u32 m_bytes;
        int m_membits;
        endianness_t m_endianness;
        int m_bytes_per_entry;
        read_entry m_read_entry;  //UINT32 (memory_array::*m_read_entry)(int);
        //void (memory_array::*m_write_entry)(int, UINT32);


        // construction/destruction
        //-------------------------------------------------
        //  memory_array - constructor
        //-------------------------------------------------
        public memory_array()
        {
            throw new emu_unimplemented();
        }

        //memory_array(void *base, UINT32 bytes, int membits, endianness_t endianness, int bpe) { set(base, bytes, membits, endianness, bpe); }
        //memory_array(const address_space &space, void *base, UINT32 bytes, int bpe) { set(space, base, bytes, bpe); }
        //memory_array(const memory_share &share, int bpe) { set(share, bpe); }
        //memory_array(const memory_array &array) { set(array); }


        // configuration
        //-------------------------------------------------
        //  set - configure the parameters
        //-------------------------------------------------
        public void set(object baseptr, UInt32 bytes, int membits, endianness_t endianness, int bpe)  //void memory_array::set(void *base, UINT32 bytes, int membits, endianness_t endianness, int bpe)
        {
            throw new emu_unimplemented();
        }


        //-------------------------------------------------
        //  set - additional setter variants
        //-------------------------------------------------
        public void set(address_space space, object baseptr, UInt32 bytes, int bpe) { set(baseptr, bytes, space.data_width(), space.endianness(), bpe); }
        public void set(memory_share share, int bpe) { set(share.ptr(), (UInt32)share.bytes(), share.bitwidth(), share.endianness(), bpe); }
        public void set(memory_array array) { set(array.baseptr(), array.bytes(), array.membits(), array.endianness(), array.bytes_per_entry()); }


        // piecewise configuration
        //-------------------------------------------------
        //  piecewise configuration
        //-------------------------------------------------
        public void set_membits(int membits) { set(m_base, m_bytes, membits, m_endianness, m_bytes_per_entry); }
        public void set_endianness(endianness_t endianness) { set(m_base, m_bytes, m_membits, endianness, m_bytes_per_entry); }


        // getters
        public object baseptr() { return m_base; }  //void *base() const { return m_base; }
        u32 bytes() { return m_bytes; }
        public int membits() { return m_membits; }
        endianness_t endianness() { return m_endianness; }
        public int bytes_per_entry() { return m_bytes_per_entry; }


        // entry-level readers and writers
        public u32 read(int index) { return m_read_entry(index); }
        //void write(int index, UINT32 data) { (this->*m_write_entry)(index, data); }


        // byte/word/dword-level readers and writers
        //UINT8 read8(offs_t offset) { return reinterpret_cast<UINT8 *>(m_base)[offset]; }
        //UINT16 read16(offs_t offset) { return reinterpret_cast<UINT16 *>(m_base)[offset]; }
        //UINT32 read32(offs_t offset) { return reinterpret_cast<UINT32 *>(m_base)[offset]; }
        //UINT64 read64(offs_t offset) { return reinterpret_cast<UINT64 *>(m_base)[offset]; }
        //void write8(offs_t offset, UINT8 data) { reinterpret_cast<UINT8 *>(m_base)[offset] = data; }
        //void write16(offs_t offset, UINT16 data, UINT16 mem_mask = 0xffff) { COMBINE_DATA(&reinterpret_cast<UINT16 *>(m_base)[offset]); }
        //void write32(offs_t offset, UINT32 data, UINT32 mem_mask = 0xffffffff) { COMBINE_DATA(&reinterpret_cast<UINT32 *>(m_base)[offset]); }
        //void write64(offs_t offset, UINT64 data, UINT64 mem_mask = U64(0xffffffffffffffff)) { COMBINE_DATA(&reinterpret_cast<UINT64 *>(m_base)[offset]); }


        // internal read/write helpers for 1 byte entries
        //UINT32 read8_from_8(int index);     void write8_to_8(int index, UINT32 data);
        //UINT32 read8_from_16le(int index);  void write8_to_16le(int index, UINT32 data);
        //UINT32 read8_from_16be(int index);  void write8_to_16be(int index, UINT32 data);
        //UINT32 read8_from_32le(int index);  void write8_to_32le(int index, UINT32 data);
        //UINT32 read8_from_32be(int index);  void write8_to_32be(int index, UINT32 data);
        //UINT32 read8_from_64le(int index);  void write8_to_64le(int index, UINT32 data);
        //UINT32 read8_from_64be(int index);  void write8_to_64be(int index, UINT32 data);


        // internal read/write helpers for 2 byte entries
        //UINT32 read16_from_8le(int index);  void write16_to_8le(int index, UINT32 data);
        //UINT32 read16_from_8be(int index);  void write16_to_8be(int index, UINT32 data);
        //UINT32 read16_from_16(int index);   void write16_to_16(int index, UINT32 data);
        //UINT32 read16_from_32le(int index); void write16_to_32le(int index, UINT32 data);
        //UINT32 read16_from_32be(int index); void write16_to_32be(int index, UINT32 data);
        //UINT32 read16_from_64le(int index); void write16_to_64le(int index, UINT32 data);
        //UINT32 read16_from_64be(int index); void write16_to_64be(int index, UINT32 data);


        // internal read/write helpers for 4 byte entries
        //UINT32 read32_from_8le(int index);  void write32_to_8le(int index, UINT32 data);
        //UINT32 read32_from_8be(int index);  void write32_to_8be(int index, UINT32 data);
        //UINT32 read32_from_16le(int index); void write32_to_16le(int index, UINT32 data);
        //UINT32 read32_from_16be(int index); void write32_to_16be(int index, UINT32 data);
        //UINT32 read32_from_32(int index);   void write32_to_32(int index, UINT32 data);
        //UINT32 read32_from_64le(int index); void write32_to_64le(int index, UINT32 data);
        //UINT32 read32_from_64be(int index); void write32_to_64be(int index, UINT32 data);
    }
}
