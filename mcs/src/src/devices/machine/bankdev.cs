// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using endianness_t = mame.util.endianness;  //using endianness_t = util::endianness;
using offs_t = System.UInt32;  //using offs_t = u32;
using s8 = System.SByte;
using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;


namespace mame
{
    public class address_map_bank_device : device_t
                                           //device_memory_interface
    {
        //DEFINE_DEVICE_TYPE(ADDRESS_MAP_BANK, address_map_bank_device, "address_map_bank", "Address Map Bank")
        static device_t device_creator_address_map_bank_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new address_map_bank_device(mconfig, tag, owner, clock); }
        public static readonly device_type ADDRESS_MAP_BANK = g.DEFINE_DEVICE_TYPE(device_creator_address_map_bank_device, "address_map_bank", "Address Map Bank");


        public class device_memory_interface_address_map_bank : device_memory_interface
        {
            public device_memory_interface_address_map_bank(machine_config mconfig, device_t device) : base(mconfig, device) { }

            protected override space_config_vector memory_space_config() { return ((address_map_bank_device)device()).device_memory_interface_memory_space_config(); }
        }


        // internal state
        endianness_t m_endianness;
        u8 m_data_width;
        u8 m_addr_width;
        u32 m_stride;
        address_space_config m_program_config = new address_space_config();
        address_space m_program;
        offs_t m_offset;
        int m_shift;


        device_memory_interface_address_map_bank m_dimemory;


        // construction/destruction
        address_map_bank_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, ADDRESS_MAP_BANK, tag, owner, clock)
        {
            m_class_interfaces.Add(new device_memory_interface_address_map_bank(mconfig, this));  //device_memory_interface(mconfig, *this),

            m_dimemory = GetClassInterface<device_memory_interface_address_map_bank>();


            m_endianness = g.ENDIANNESS_NATIVE;
            m_data_width = 0;
            m_addr_width = 32;
            m_stride = 1;
            m_program = null;
            m_offset = 0;
            m_shift = 0;
        }


        // configuration helpers
        public address_map_bank_device set_map(address_map_constructor args) { m_dimemory.set_addrmap(0, args); return this; }  //template <typename... T> address_map_bank_device& set_map(T &&... args) { set_addrmap(0, std::forward<T>(args)...); return *this; }
        address_map_bank_device set_endianness(endianness_t endianness) { m_endianness = endianness; return this; }
        address_map_bank_device set_data_width(u8 data_width) { m_data_width = data_width; return this; }
        address_map_bank_device set_addr_width(u8 addr_width) { m_addr_width = addr_width; return this; }
        address_map_bank_device set_stride(u32 stride) { m_stride = stride; return this; }
        //address_map_bank_device& set_shift(u32 shift) { m_shift = shift; return *this; }


        public address_map_bank_device set_options(endianness_t endianness, u8 data_width, u8 addr_width, u32 stride = 1)
        {
            set_endianness(endianness);
            set_data_width(data_width);
            set_addr_width(addr_width);
            set_stride(stride);
            return this;
        }


        //template <typename... T> address_map_bank_device& map(T &&... args) { set_addrmap(0, std::forward<T>(args)...); return *this; }
        //address_map_bank_device& endianness(endianness_t endianness) { m_endianness = endianness; return *this; }
        //address_map_bank_device& data_width(u8 data_width) { m_data_width = data_width; return *this; }
        //address_map_bank_device& addr_width(u8 addr_width) { m_addr_width = addr_width; return *this; }
        //address_map_bank_device& stride(u32 stride) { m_stride = stride; return *this; }
        //address_map_bank_device& shift(u32 shift) { m_shift = shift; return *this; }

        //void amap8(address_map &map);


        public void amap16(address_map map) { map.op(0x00000000, 0xffffffff).rw((offset, mem_mask) => { return read16(offset, mem_mask); }, (offset, data, mem_mask) => { write16(offset, data, mem_mask); }); }
        //void amap32(address_map &map);
        //void amap64(address_map &map);

        //void write8(offs_t offset, u8 data);
        void write16(offs_t offset, u16 data, u16 mem_mask = 0xffff)
        {
            m_program.write_word(m_offset + (offset << (m_shift + 1)), data, mem_mask);
        }
        //void write32(offs_t offset, u32 data, u32 mem_mask = 0xffffffff);
        //void write64(offs_t offset, u64 data, u64 mem_mask = ~u64(0));

        //u8 read8(offs_t offset);
        u16 read16(offs_t offset, u16 mem_mask = 0xffff)
        {
            return m_program.read_word(m_offset + (offset << (m_shift + 1)), mem_mask);
        }
        //u32 read32(offs_t offset, u32 mem_mask = 0xffffffff);
        //u64 read64(offs_t offset, u64 mem_mask = ~u64(0));


        public void set_bank(offs_t bank)
        {
            m_offset = bank * m_stride;
        }


        protected override void device_start()
        {
            m_program = m_dimemory.space(g.AS_PROGRAM);

            save_item(g.NAME(new { m_offset }));
        }


        protected override void device_config_complete()
        {
            // don't replace existing config, so we don't lose the reference (see device_memory_interface_memory_space_config)
            var config = new address_space_config("program", m_endianness, m_data_width, m_addr_width, (s8)m_shift);  //m_program_config = address_space_config( "program", m_endianness, m_data_width, m_addr_width, m_shift );
            config.CopyTo(m_program_config);
        }


        // device_memory_interface overrides
        space_config_vector device_memory_interface_memory_space_config()
        {
            return new space_config_vector {
                std.make_pair(g.AS_PROGRAM, m_program_config)
            };
        }
    }
}
