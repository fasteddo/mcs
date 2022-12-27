// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.netlist.nl_errstr_global;


namespace mame.netlist
{
    // -----------------------------------------------------------------------------
    // device_t
    // -----------------------------------------------------------------------------
    public class device_t : base_device_t,
                            logic_family_t
    {
        param_model_t m_model;


        //device_t(netlist_state_t &owner, const pstring &name);
        //device_t(netlist_state_t &owner, const pstring &name, const pstring &model);
        // only needed by proxies
        //device_t(netlist_state_t &owner, const pstring &name, const logic_family_desc_t *desc);

        //device_t(device_t &owner, const pstring &name);
        // pass in a default model - this may be overwritten by PARAM(DEVICE.MODEL, "XYZ(...)")
        //device_t(device_t &owner, const pstring &name, const pstring &model);

        protected device_t(object owner, string name)
            : base(owner, name)
        {
            if (owner is netlist_state_t) device_t_after_ctor((netlist_state_t)owner, name);
            else if (owner is device_t) device_t_after_ctor((device_t)owner, name);
            else throw new emu_unimplemented();
        }

        protected device_t(object owner, string name, string model)
            : base(owner, name)
        {
            if (owner is netlist_state_t) device_t_after_ctor((netlist_state_t)owner, name, model);
            else if (owner is device_t) device_t_after_ctor((device_t)owner, name, model);
            else throw new emu_unimplemented();
        }

        protected device_t(object owner, string name, logic_family_desc_t desc)
            : base(owner, name)
        {
            if (owner is netlist_state_t) device_t_after_ctor((netlist_state_t)owner, name, desc);
            else if (owner is device_t) throw new emu_unimplemented();
            else throw new emu_unimplemented();
        }


        public void device_t_after_ctor(netlist_state_t owner, string name)
            //: base(owner, name)
        {
            m_model = new param_model_t(this, "MODEL", config.DEFAULT_LOGIC_FAMILY());


            set_logic_family(state().setup().family_from_model(m_model.op()));
            if (logic_family() == null)
                throw new nl_exception(MF_NULLPTR_FAMILY(this.name(), m_model.op()));
        }


        public void device_t_after_ctor(netlist_state_t owner, string name, string model)
            //: base(owner, name)
        {
            m_model = new param_model_t(this, "MODEL", model);


            set_logic_family(state().setup().family_from_model(m_model.op()));
            if (logic_family() == null)
                throw new nl_exception(MF_NULLPTR_FAMILY(this.name(), m_model.op()));
        }


        // only needed by proxies
        public void device_t_after_ctor(netlist_state_t owner, string name, logic_family_desc_t desc)
            //: base(owner, name)
        {
            m_model = new param_model_t(this, "MODEL", "");


            set_logic_family(desc);
            if (logic_family() == null)
                throw new nl_exception(MF_NULLPTR_FAMILY(this.name(), "<pointer provided by constructor>"));
        }


        public void device_t_after_ctor(device_t owner, string name)
            //: base(owner, name)
        {
            m_model = new param_model_t(this, "MODEL", "");


            set_logic_family(owner.logic_family());
            if (logic_family() == null)
                throw new nl_exception(MF_NULLPTR_FAMILY(this.name(), "<owner logic family>"));
        }
    
    
        // pass in a default model - this may be overwritten by PARAM(DEVICE.MODEL, "XYZ(...)")
        public void device_t_after_ctor(device_t owner, string name, string model)
            //: base(owner, name)
        {
            m_model = new param_model_t(this, "MODEL", model);


            set_logic_family(state().setup().family_from_model(m_model.op()));
            if (logic_family() == null)
                throw new nl_exception(MF_NULLPTR_FAMILY(this.name(), m_model.op()));
        }

        //PCOPYASSIGNMOVE(device_t, delete)

        //~device_t() noexcept override = default;


        // logic_family_t
        logic_family_desc_t m_logic_family;
        public logic_family_desc_t logic_family() { return m_logic_family; }
        public void set_logic_family(logic_family_desc_t fam) { m_logic_family = fam; }


        //NETLIB_UPDATE_TERMINALSI() { }
    }
} // namespace netlist
