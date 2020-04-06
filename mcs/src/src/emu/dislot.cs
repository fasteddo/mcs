// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using u32 = System.UInt32;


namespace mame
{
    /// \brief Base class for configurable slot devices
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
        std.unordered_map<string, slot_option> m_options = new std.unordered_map<string, slot_option>();  //std::unordered_map<std::string,std::unique_ptr<slot_option>> m_options;
        u32 m_default_clock;
        string m_default_option;
        bool m_fixed;
        device_t m_card_device;


        // construction/destruction
        protected device_slot_interface(machine_config mconfig, device_t device)
            : base(device, "slot")
        {
            m_default_clock = DERIVED_CLOCK(1, 1);
            m_default_option = null;
            m_fixed = false;
            m_card_device = null;
        }

        /// \brief Set whether slot is fixed
        ///
        /// Allows a slot to be configured as fixed.  Fixed slots can only
        /// be configured programmatically.  Options for fixed slots are not
        /// user-selectable.  By default, slots are not fixed.
        /// \param [in] fixed True to mark the slot as fixed, or false to
        ///   mark it user-configurable.
        /// \sa fixed
        //void set_fixed(bool fixed) { m_fixed = fixed; }

        /// \brief Set the default option
        ///
        /// Set the default option the slot.  The corresponding card device
        /// will be loaded if no other option is selected by the user or
        /// programmatically.
        /// \param [in] option The name of the default option.  This must be
        ///   correspond to an option added using #option_add or
        ///   #option_add_internal, or be nullptr to not load any card
        ///   device by default.  The string is not copied and must remain
        ///   valid for the lifetime of the device (or until another default
        ///   option is configured).
        /// \sa default_option
        //void set_default_option(const char *option) { m_default_option = option; }

        /// \brief Clear options
        ///
        /// This removes all previously added options.
        //void option_reset() { m_options.clear(); }

        /// \brief Add a user-selectable option
        ///
        /// Adds an option that may be selected by the user via the command
        /// line or other configureation methods.
        /// \param [in] option The name used to select the option.  This
        ///   will also be used as the tag when instantiating the card.  It
        ///   must be a valid device tag, and should be terse but
        ///   descriptive.  The string is copied when the option is added.
        /// \param [in] devtype Device type of the option.  The device type
        ///   description is used in the user interface.
        /// \return A reference to the added option for additional
        ///   configuration.
        //slot_option &option_add(const char *option, const device_type &devtype);

        /// \brief Add an internal option
        ///
        /// Adds an option that may only be selected programmatically.  This
        /// is most often used for options used for loading software.
        /// \param [in] option The name used to select the option.  This
        ///   will also be used as the tag when instantiating the card.  It
        ///   must be a valid device tag.  The string is copied when the
        ///   option is added.
        /// \param [in] devtype Device type of the option.  The device type
        ///   description is used in the user interface.
        /// \return A reference to the added option for additional
        ///   configuration.
        //slot_option &option_add_internal(const char *option, const device_type &devtype);

        //void set_option_default_bios(const char *option, const char *default_bios) { config_option(option)->default_bios(default_bios); }
        //template <typename T> void set_option_machine_config(const char *option, T &&machine_config) { config_option(option)->machine_config(std::forward<T>(machine_config)); }
        //void set_option_device_input_defaults(const char *option, const input_device_default *default_input) { config_option(option)->input_device_defaults(default_input); }

        /// \brief Returns whether the slot is fixed
        ///
        /// Fixed slots can only be configured programmatically.  Slots are
        /// not fixed by default.
        /// \return True if the slot is fixed, or false if it is
        ///   user-configurable.
        /// \sa set_fixed
        public bool fixed_() { return m_fixed; }

        /// \brief Returns true if the slot has user-selectable options
        ///
        /// Returns true if the slot is not marked as fixed and has at least
        /// one user-selectable option (added using #option_add rather than
        /// #option_add_internal).  Returns false if the slot is marked as
        /// fixed, all options are internal, or there are no options.
        /// \return True if the slot has user-selectable options, or false
        ///   otherwise.
        //bool has_selectable_options() const;

        public string default_option() { return m_default_option; }
        public std.unordered_map<string, slot_option> option_list() { return m_options; }
        public slot_option option(string name) { if (!string.IsNullOrEmpty(name)) return m_options.find(name); return null; }
        public virtual string get_default_card_software() { return ""; }
        public device_t get_card_device() { return m_card_device; }
        public void set_card_device(device_t dev) { m_card_device = dev; }
        public string slot_name() { return device().tag().Substring(1); }  // + 1; }
        //slot_option &option_set(const char *tag, const device_type &devtype) { m_default_option = tag; m_fixed = true; return option_add_internal(tag, devtype); }


        protected override void interface_validity_check(validity_checker valid)
        {
            throw new emu_unimplemented();
        }


        void set_default_clock(u32 clock) { m_default_clock = clock; }


        //slot_option *config_option(const char *option);
    }


    //typedef device_interface_iterator<device_slot_interface> slot_interface_iterator;
    public class slot_interface_iterator : device_interface_iterator<device_slot_interface>
    {
        public slot_interface_iterator(device_t root, int maxdepth = 255) : base(root, maxdepth) { }
    }
}
