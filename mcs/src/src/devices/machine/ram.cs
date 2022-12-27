// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using ram_device_enumerator = mame.device_type_enumerator<mame.ram_device>;  //typedef device_type_enumerator<ram_device> ram_device_enumerator;
using u32 = System.UInt32;

using static mame.device_global;
using static mame.ram_global;


namespace mame
{
    static class ram_global
    {
        /***************************************************************************
            CONSTANTS
        ***************************************************************************/
        const string RAM_TAG = "ram";
    }


    /***************************************************************************
        TYPE DEFINITIONS
    ***************************************************************************/
    class ram_device : device_t
    {
        //DEFINE_DEVICE_TYPE(RAM, ram_device, "ram", "RAM")
        public static readonly emu.detail.device_type_impl RAM = DEFINE_DEVICE_TYPE("ram", "RAM", (type, mconfig, tag, owner, clock) => { return new ram_device(mconfig, tag, owner, clock); });


        //using extra_option = std::pair<std::string, u32>;
        //using extra_option_vector = std::vector<extra_option>;


        // device state
        //u32                         m_size;
        //std::unique_ptr<u8 []>      m_pointer;

        // device config
        //char const *                m_default_size;
        //u8                          m_default_value;
        //mutable extra_option_vector m_extra_options;
        //char const *                m_extra_options_string;


        // construction/destruction
        ram_device(machine_config mconfig, string tag, device_t owner, u32 clock = 0)
            : base(mconfig, RAM, tag, owner, clock)
        {
            throw new emu_unimplemented();
#if false
            , m_size(0)
            , m_default_size(0)
            , m_default_value(0xff)
            , m_extra_options_string(nullptr)
#endif
        }


        // accessors
        //u32 size() const { return m_size; }
        //u32 mask() const { return m_size - 1; }
        //u8 *pointer() { return &m_pointer[0]; }
        //char const *default_size_string() const { return m_default_size; }
        //u32 default_size() const;
        //extra_option_vector const &extra_options() const;

        // read/write
        //u8 read(offs_t offset)              { return m_pointer[offset % m_size]; }
        //void write(offs_t offset, u8 data)  { m_pointer[offset % m_size] = data; }

        // inline configuration helpers
        //ram_device &set_default_size(char const *default_size) { m_default_size = default_size; return *this; }
        //ram_device &set_extra_options(char const *extra_options)
        //{
        //    m_extra_options_string = extra_options && extra_options[0] ? extra_options : nullptr;
        //    m_extra_options.clear();
        //    return *this;
        //}
        //ram_device &set_default_value(u8 default_value) { m_default_value = default_value; return *this; }


        protected override void device_start() { throw new emu_unimplemented(); }
        protected override void device_validity_check(validity_checker valid) { throw new emu_unimplemented(); }


        //bool is_valid_size(u32 size) const;
    }


    // device iterator
    //typedef device_type_enumerator<ram_device> ram_device_enumerator;
}
