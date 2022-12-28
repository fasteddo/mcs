// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_data_t = mame.netlist.core_device_data_t;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using device_param_t = mame.netlist.core_device_data_t;  //using device_param_t = const device_data_t &;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using device_t_constructor_data_t = mame.netlist.core_device_data_t;  //using constructor_data_t = device_data_t;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = device_param_t;  //using device_param_t = const device_data_t &;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;

using static mame.netlist.nl_errstr_global;


namespace mame.netlist
{
    // -------------------------------------------------------------------------
    // device_t construction parameters
    // -------------------------------------------------------------------------
    //using device_data_t = base_device_data_t;
    // The type use to pass data on
    //using device_param_t = const device_data_t &;


    // -----------------------------------------------------------------------------
    // device_t
    // -----------------------------------------------------------------------------
    public class device_t : base_device_t,
                            logic_family_t
    {
        //using constructor_data_t = device_data_t;
        //using constructor_param_t = device_param_t;


        param_model_t m_model;


        protected device_t(device_param_t data)
            : base(data)
        {
            m_model = new param_model_t(this, "MODEL", config.DEFAULT_LOGIC_FAMILY());


            set_logic_family(state().setup().family_from_model(m_model.op()));
            if (logic_family() == null)
                throw new nl_exception(MF_NULLPTR_FAMILY(this.name(), m_model.op()));
        }


        protected device_t(device_param_t data, string model)
            : base(data)
        {
            m_model = new param_model_t(this, "MODEL", model);


            set_logic_family(state().setup().family_from_model(m_model.op()));
            if (logic_family() == null)
                throw new nl_exception(MF_NULLPTR_FAMILY(this.name(), m_model.op()));
        }


        // only needed by proxies
        protected device_t(device_param_t data, logic_family_desc_t desc)
            : base(data)
        {
            m_model = new param_model_t(this, "MODEL", "");


            set_logic_family(desc);
            if (logic_family() == null)
                throw new nl_exception(MF_NULLPTR_FAMILY(this.name(), "<pointer provided by constructor>"));
        }


        //device_t(const device_t &) = delete;
        //device_t &operator=(const device_t &) = delete;
        //device_t(device_t &&) noexcept = delete;
        //device_t &operator=(device_t &&) noexcept = delete;

        //~device_t() noexcept override = default;


        // logic_family_t
        logic_family_desc_t m_logic_family;
        public logic_family_desc_t logic_family() { return m_logic_family; }
        public void set_logic_family(logic_family_desc_t fam) { m_logic_family = fam; }


        //template <typename T1, typename T2>
        //void push_two(T1 &term1, netlist_sig_t newQ1,
        //    const netlist_time &delay1, T2 &term2, netlist_sig_t newQ2,
        //    const netlist_time &delay2) noexcept
        //{
        //    if (delay2 < delay1)
        //    {
        //        term1.push(newQ1, delay1);
        //        term2.push(newQ2, delay2);
        //    }
        //    else
        //    {
        //        term2.push(newQ2, delay2);
        //        term1.push(newQ1, delay1);
        //    }
        //}


        //NETLIB_UPDATE_TERMINALSI() { }
    }


    // -------------------------------------------------------------------------
    // FIXME: Rename
    // -------------------------------------------------------------------------
    //template <typename CX>
    class sub_device_wrapper<CX> where CX : core_device_t
    {
        //using constructor_data_t = typename CX::constructor_data_t;
        //using constructor_param_t = typename CX::constructor_param_t;


        CX m_dev;  //device_arena::unique_ptr<CX> m_dev;


        //template <typename... Args>
        public sub_device_wrapper(base_device_t owner, CX args)  //sub_device_wrapper(base_device_t &owner, const pstring &name, Args &&...args)
        {
            // m_dev = owner.state().make_pool_object<CX>(owner, name,
            // std::forward<Args>(args)...);
            m_dev = args;  //m_dev = owner.state().make_pool_object<CX>(constructor_data_t{owner.state(), owner.name() + "." + name}, std::forward<Args>(args)...);
            owner.state().register_device(m_dev.name(), m_dev);  //owner.state().register_device(m_dev->name(), device_arena::owned_ptr<core_device_t>(m_dev.get(), false));
        }

        //template <typename... Args>
        public sub_device_wrapper(device_t owner, string name, CX args)  //sub_device_wrapper(device_t &owner, const pstring &name, Args &&...args)
        {
            // m_dev = owner.state().make_pool_object<CX>(owner, name,
            // std::forward<Args>(args)...);
            m_dev = args;  //m_dev = owner.state().make_pool_object<CX>(constructor_data_t{owner.state(), owner.name() + "." + name}, std::forward<Args>(args)...);
            owner.state().register_device(m_dev.name(), m_dev);  //owner.state().register_device(m_dev->name(), device_arena::owned_ptr<core_device_t>(m_dev.get(), false));
        }


        //CX &      operator()() { return *m_dev; }
        //const CX &operator()() const { return *m_dev; }
        public CX op() { return m_dev; }
    }
}
