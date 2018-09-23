// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using u32 = System.UInt32;


namespace mame
{
    // ======================> device_slot_interface
    public class device_slot_interface : device_interface
    {
        public class slot_option
        {
            // internal state
            string m_name;
            device_type m_devtype;
            bool m_selectable;
            string m_default_bios;
            machine_creator_wrapper m_machine_config;
            input_device_default [] m_input_device_defaults;
            u32 m_clock;


            public slot_option(string name, device_type devtype, bool selectable)
            {
                m_name = name;
                m_devtype = devtype;
                m_selectable = selectable;
                m_default_bios = null;
                m_machine_config = null;
                m_input_device_defaults = null;
                m_clock = 0;
            }


            public string name() { return m_name; }
            public device_type devtype() { return m_devtype; }
            public bool selectable() { return m_selectable; }
            public string default_bios() { return m_default_bios; }
            public machine_creator_wrapper machine_config() { return m_machine_config; }
            public input_device_default [] input_device_defaults() { return m_input_device_defaults; }
            public u32 clock() { return m_clock; }
        }


        // internal state
        std_unordered_map<string, slot_option> m_options = new std_unordered_map<string, slot_option>();  //std::unordered_map<std::string,std::unique_ptr<slot_option>> m_options;
        u32 m_default_clock;
        string m_default_option;
        bool m_fixed;
        device_t m_card_device;


        // construction/destruction
        public device_slot_interface(machine_config mconfig, device_t device)
            : base(device, "slot")
        {
            m_default_clock = device_global.DERIVED_CLOCK(1, 1);
            m_default_option = null;
            m_fixed = false;
            m_card_device = null;
        }

        //void set_fixed(bool fixed) { m_fixed = fixed; }
        //void set_default_option(const char *option) { m_default_option = option; }
        //void option_reset() { m_options.clear(); }
        //slot_option &option_add(const char *option, const device_type &devtype);
        //slot_option &option_add_internal(const char *option, const device_type &devtype);
        //void set_option_default_bios(const char *option, const char *default_bios) { config_option(option)->default_bios(default_bios); }
        //template <typename T> void set_option_machine_config(const char *option, T &&machine_config) { config_option(option)->machine_config(std::forward<T>(machine_config)); }
        //void set_option_device_input_defaults(const char *option, const input_device_default *default_input) { config_option(option)->input_device_defaults(default_input); }
        public bool fixed_() { return m_fixed; }
        public string default_option() { return m_default_option; }
        public Dictionary<string, slot_option> option_list() { return m_options; }
        public slot_option option(string name) { if (!string.IsNullOrEmpty(name)) return m_options.find(name); return null; }
        public virtual string get_default_card_software() { return ""; }
        public device_t get_card_device() { return m_card_device; }
        public void set_card_device(device_t dev) { m_card_device = dev; }
        public string slot_name() { return device().tag().Substring(1); }  // + 1; }

        void set_default_clock(u32 clock) { m_default_clock = clock; }

        //slot_option *config_option(const char *option);
    }


    //typedef device_interface_iterator<device_slot_interface> slot_interface_iterator;
    public class slot_interface_iterator : device_interface_iterator<device_slot_interface>
    {
        public slot_interface_iterator(device_t root, int maxdepth = 255) : base(root, maxdepth) { }
    }
}
