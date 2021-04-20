// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using nl_fptype = System.Double;  //using nl_fptype = config::fptype;


namespace mame.netlist
{
    /// \brief Logic families descriptors are used to create proxy devices.
    ///  The logic family describes the analog capabilities of logic devices,
    ///  inputs and outputs.
    public abstract class logic_family_desc_t
    {
        public nl_fptype m_low_thresh_PCNT;   //!< low input threshhold offset. If the input voltage is below this value times supply voltage, a "0" input is signalled
        public nl_fptype m_high_thresh_PCNT;  //!< high input threshhold offset. If the input voltage is above the value times supply voltage, a "0" input is signalled
        public nl_fptype m_low_VO;            //!< low output voltage offset. This voltage is output if the ouput is "0"
        public nl_fptype m_high_VO;           //!< high output voltage offset. The supply voltage minus this offset is output if the ouput is "1"
        public nl_fptype m_R_low;             //!< low output resistance. Value of series resistor used for low output
        public nl_fptype m_R_high;            //!< high output resistance. Value of series resistor used for high output
        public string m_vcc;             //!< default power pin name for positive supply
        public string m_gnd;             //!< default power pin name for negative supply


        protected logic_family_desc_t()
        { }

        //PCOPYASSIGNMOVE(logic_family_desc_t, delete)

        //virtual ~logic_family_desc_t() noexcept = default;


        public abstract devices.nld_base_d_to_a_proxy create_d_a_proxy(netlist_state_t anetlist, string name, logic_output_t proxied);  //virtual device_arena::unique_ptr<devices::nld_base_d_to_a_proxy> create_d_a_proxy(netlist_state_t &anetlist, const pstring &name, const logic_output_t *proxied) const = 0;
        public abstract devices.nld_base_a_to_d_proxy create_a_d_proxy(netlist_state_t anetlist, string name, logic_input_t proxied);  //virtual device_arena::unique_ptr<devices::nld_base_a_to_d_proxy> create_a_d_proxy(netlist_state_t &anetlist, const pstring &name, const logic_input_t *proxied) const = 0;


        //nl_fptype low_thresh_V(nl_fptype VN, nl_fptype VP) const noexcept{ return VN + (VP - VN) * m_low_thresh_PCNT; }
        //nl_fptype high_thresh_V(nl_fptype VN, nl_fptype VP) const noexcept{ return VN + (VP - VN) * m_high_thresh_PCNT; }
        public nl_fptype low_offset_V() { return m_low_VO; }
        public nl_fptype high_offset_V() { return m_high_VO; }
        public nl_fptype R_low() { return m_R_low; }
        public nl_fptype R_high() { return m_R_high; }

        //bool is_above_high_thresh_V(nl_fptype V, nl_fptype VN, nl_fptype VP) const noexcept
        //{ return V > high_thresh_V(VN, VP); }

        //bool is_below_low_thresh_V(nl_fptype V, nl_fptype VN, nl_fptype VP) const noexcept
        //{ return V < low_thresh_V(VN, VP); }

        public string vcc_pin() { return m_vcc; }
        public string gnd_pin() { return m_gnd; }
    }


    /// \brief Base class for devices, terminals, outputs and inputs which support
    ///  logic families.
    ///  This class is a storage container to store the logic family for a
    ///  netlist object. You will not directly use it. Please refer to
    ///  \ref NETLIB_FAMILY to learn how to define a logic family for a device.
    ///
    /// All terminals inherit the family description from the device
    /// The default is the ttl family, but any device can override the family.
    /// For individual terminals, the family can be overwritten as well.
    ///
    interface logic_family_t
    {
        //logic_family_desc_t m_logic_family;
        logic_family_desc_t logic_family();
        void set_logic_family(logic_family_desc_t fam);


        //logic_family_t() : m_logic_family(nullptr) {}
        //logic_family_t(const logic_family_desc_t *d) : m_logic_family(d) {}
        //PCOPYASSIGNMOVE(logic_family_t, delete)


        //PCOPYASSIGNMOVE(logic_family_t, delete)


        //const logic_family_desc_t *logic_family() const noexcept { return m_logic_family; }
        //void set_logic_family(const logic_family_desc_t *fam) noexcept { m_logic_family = fam; }


        //~logic_family_t() noexcept = default; // prohibit polymorphic destruction
    }
} // namespace netlist
