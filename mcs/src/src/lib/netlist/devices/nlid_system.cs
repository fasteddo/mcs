// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_time = mame.netlist.ptime_u64;  //using netlist_time = ptime<std::uint64_t, NETLIST_INTERNAL_RES>;


namespace mame.netlist
{
    namespace devices
    {
        // -----------------------------------------------------------------------------
        // netlistparams
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(netlistparams)
        class nld_netlistparams : device_t
        {
            //NETLIB_DEVICE_IMPL(netlistparams)
            static factory.element_t nld_netlistparams_c(string name, string classname, string def_param)
            { return new factory.device_element_t<nld_netlistparams>(name, classname, def_param, "__FILE__"); }
            public static factory.constructor_ptr_t decl_netlistparams = nld_netlistparams_c;


            param_logic_t m_use_deactivate;


            //NETLIB_CONSTRUCTOR(netlistparams)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_netlistparams(object owner, string name)
                : base(owner, name)
            {
                m_use_deactivate = new param_logic_t(this, "USE_DEACTIVATE", false /*0*/);
            }


            public param_logic_t use_deactivate { get { return m_use_deactivate; } }


            //NETLIB_UPDATEI() { }
            protected override void update()
            {
                throw new emu_unimplemented();
            }


            //NETLIB_RESETI() { }
            //NETLIB_UPDATE_PARAMI() { }
        }


        // -----------------------------------------------------------------------------
        // mainclock
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(mainclock)
        class nld_mainclock : device_t
        {
            logic_output_t m_Q;

            param_double_t m_freq;
            netlist_time m_inc;


            //NETLIB_CONSTRUCTOR(mainclock)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            nld_mainclock(object owner, string name)
                : base(owner, name)
            {
                m_Q = new logic_output_t(this, "Q");
                m_freq = new param_double_t(this, "FREQ", 7159000.0 * 5);


                m_inc = netlist_time.from_double(1.0 / (m_freq.op()*2.0));
            }


            public logic_output_t Q { get { return m_Q; } }


            //NETLIB_RESETI()
            protected override void reset()
            {
                throw new emu_unimplemented();
#if false
                m_Q.net().set_time(netlist_time.zero());
#endif
            }

            //NETLIB_UPDATE_PARAMI()
            public override void update_param()
            {
                throw new emu_unimplemented();
#if false
                m_inc = netlist_time::from_double(1.0 / (m_freq()*2.0));
#endif
            }

            //NETLIB_UPDATEI()
            protected override void update()
            {
                throw new emu_unimplemented();
#if false
                logic_net_t &net = m_Q.net();
                // this is only called during setup ...
                net.toggle_new_Q();
                net.set_time(netlist().time() + m_inc);
#endif
            }


            //inline static void mc_update(logic_net_t &net);
        }


        // -----------------------------------------------------------------------------
        // clock
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(clock)
        //{
        //    NETLIB_CONSTRUCTOR(clock)
        //    , m_feedback(*this, "FB")
        //    , m_Q(*this, "Q")
        //    , m_freq(*this, "FREQ", 7159000.0 * 5.0)
        //    {
        //        m_inc = netlist_time::from_double(1.0 / (m_freq()*2.0));
        //
        //        connect(m_feedback, m_Q);
        //    }
        //    NETLIB_UPDATEI();
        //    //NETLIB_RESETI();
        //    NETLIB_UPDATE_PARAMI();
        //
        //protected:
        //    logic_input_t m_feedback;
        //    logic_output_t m_Q;
        //
        //    param_double_t m_freq;
        //    netlist_time m_inc;
        //};


        // -----------------------------------------------------------------------------
        // extclock
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(extclock)
        //{
        //    NETLIB_CONSTRUCTOR(extclock)
        //    , m_freq(*this, "FREQ", 7159000.0 * 5.0)
        //    , m_pattern(*this, "PATTERN", "1,1")
        //    , m_offset(*this, "OFFSET", 0.0)
        //    , m_feedback(*this, "FB")
        //    , m_Q(*this, "Q")
        //    , m_cnt(*this, "m_cnt", 0)
        //    , m_off(*this, "m_off", netlist_time::zero())
        //    {
        //        m_inc[0] = netlist_time::from_double(1.0 / (m_freq() * 2.0));
        //
        //        connect(m_feedback, m_Q);
        //        {
        //            netlist_time base = netlist_time::from_double(1.0 / (m_freq()*2.0));
        //            std::vector<pstring> pat(plib::psplit(m_pattern(),","));
        //            m_off = netlist_time::from_double(m_offset());
        //
        //            unsigned long pati[32];
        //            for (int pI = 0; pI < 32; pI++)
        //            {
        //                pati[pI] = 0;
        //            }
        //            m_size = static_cast<std::uint8_t>(pat.size());
        //            unsigned long total = 0;
        //            for (unsigned i=0; i<m_size; i++)
        //            {
        //                pati[i] = static_cast<unsigned long>(pat[i].as_long());
        //                total += pati[i];
        //            }
        //            netlist_time ttotal = netlist_time::zero();
        //            for (unsigned i=0; i<m_size - 1; i++)
        //            {
        //                m_inc[i] = base * pati[i];
        //                ttotal += m_inc[i];
        //            }
        //            m_inc[m_size - 1] = base * total - ttotal;
        //        }
        //    }
        //    NETLIB_UPDATEI();
        //    NETLIB_RESETI();
        //    //NETLIB_UPDATE_PARAMI();
        //protected:
        //
        //    param_double_t m_freq;
        //    param_str_t m_pattern;
        //    param_double_t m_offset;
        //
        //    logic_input_t m_feedback;
        //    logic_output_t m_Q;
        //    state_var_u8 m_cnt;
        //    std::uint8_t m_size;
        //    state_var<netlist_time> m_off;
        //    netlist_time m_inc[32];
        //};


        // -----------------------------------------------------------------------------
        // Special support devices ...
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(logic_input)
        partial class nld_logic_input : device_t
        {
            //NETLIB_DEVICE_IMPL(logic_input)
            static factory.element_t nld_logic_input_c(string name, string classname, string def_param)
            { return new factory.device_element_t<nld_logic_input>(name, classname, def_param, "__FILE__"); }
            public static factory.constructor_ptr_t decl_logic_input = nld_logic_input_c;


            logic_output_t m_Q;

            param_logic_t m_IN;
            param_model_t m_FAMILY;


            //NETLIB_CONSTRUCTOR(logic_input)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_logic_input(object owner, string name)
                : base(owner, name)
            {
                m_Q = new logic_output_t(this, "Q");
                m_IN = new param_logic_t(this, "IN", false /*0*/);
                /* make sure we get the family first */
                m_FAMILY = new param_model_t(this, "FAMILY", "FAMILY(TYPE=TTL)");


                set_logic_family(setup().family_from_model(m_FAMILY.op()));
            }


            //NETLIB_UPDATE_AFTER_PARAM_CHANGE()

            //NETLIB_UPDATEI();
            //NETLIB_RESETI();
            //NETLIB_UPDATE_PARAMI();
        }


        //NETLIB_OBJECT(analog_input)
        partial class nld_analog_input : device_t
        {
            //NETLIB_DEVICE_IMPL(analog_input)
            static factory.element_t nld_analog_input_c(string name, string classname, string def_param)
            { return new factory.device_element_t<nld_analog_input>(name, classname, def_param, "__FILE__"); }
            public static factory.constructor_ptr_t decl_analog_input = nld_analog_input_c;


            analog_output_t m_Q;
            param_double_t m_IN;


            //NETLIB_CONSTRUCTOR(analog_input)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_analog_input(object owner, string name)
                : base(owner, name)
            {
                m_Q = new analog_output_t(this, "Q");
                m_IN = new param_double_t(this, "IN", 0.0);
            }


            //NETLIB_UPDATE_AFTER_PARAM_CHANGE()

            //NETLIB_UPDATEI();
            //NETLIB_RESETI();
            //NETLIB_UPDATE_PARAMI();
        }


        // -----------------------------------------------------------------------------
        // nld_gnd
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(gnd)
        class nld_gnd : device_t
        {
            //NETLIB_DEVICE_IMPL(gnd)
            static factory.element_t nld_gnd_c(string name, string classname, string def_param)
            { return new factory.device_element_t<nld_gnd>(name, classname, def_param, "__FILE__"); }
            public static factory.constructor_ptr_t decl_gnd = nld_gnd_c;


            analog_output_t m_Q;


            //NETLIB_CONSTRUCTOR(gnd)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            public nld_gnd(object owner, string name)
                : base(owner, name)
            {
                m_Q = new analog_output_t(this, "Q");
            }

            //NETLIB_UPDATEI()
            protected override void update()
            {
                throw new emu_unimplemented();
#if false
                m_Q.push(0.0);
#endif
            }

            //NETLIB_RESETI() { }
            protected override void reset() { }
        }


        // -----------------------------------------------------------------------------
        // nld_dummy_input
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(dummy_input, base_dummy)
        class nld_dummy_input : nld_base_dummy
        {
            static factory.element_t nld_dummy_input_c(string name, string classname, string def_param)
            { return new factory.device_element_t<nld_dummy_input>(name, classname, def_param, "__FILE__"); }
            public static factory.constructor_ptr_t decl_dummy_input = nld_dummy_input_c;


            //analog_input_t m_I;


            //NETLIB_CONSTRUCTOR_DERIVED(dummy_input, base_dummy)
            //detail.family_setter_t m_famsetter;
            //template <class CLASS>
            nld_dummy_input(netlist_t owner, string name)
                : base(owner, name)
            {
                throw new emu_unimplemented();
#if false
                m_I(*this, "I")
#endif
            }


            //NETLIB_RESETI() { }
            protected override void reset() { }
            //NETLIB_UPDATEI() { }
            protected override void update() { }
        }


        // -----------------------------------------------------------------------------
        // nld_frontier
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(frontier, base_dummy)
        //{
        //public:
        //    NETLIB_CONSTRUCTOR_DERIVED(frontier, base_dummy)
        //    , m_RIN(*this, "m_RIN", true)
        //    , m_ROUT(*this, "m_ROUT", true)
        //    , m_I(*this, "_I")
        //    , m_Q(*this, "_Q")
        //    , m_p_RIN(*this, "RIN", 1.0e6)
        //    , m_p_ROUT(*this, "ROUT", 50.0)
        //
        //    {
        //        register_subalias("I", m_RIN.m_P);
        //        register_subalias("G", m_RIN.m_N);
        //        connect(m_I, m_RIN.m_P);
        //
        //        register_subalias("_OP", m_ROUT.m_P);
        //        register_subalias("Q", m_ROUT.m_N);
        //        connect(m_Q, m_ROUT.m_P);
        //    }
        //
        //    NETLIB_RESETI()
        //    {
        //        m_RIN.set(1.0 / m_p_RIN(),0,0);
        //        m_ROUT.set(1.0 / m_p_ROUT(),0,0);
        //    }
        //
        //    NETLIB_UPDATEI()
        //    {
        //        m_Q.push(m_I());
        //    }
        //
        //private:
        //    analog::NETLIB_NAME(twoterm) m_RIN;
        //    /* Fixme: only works if the device is time-stepped - need to rework */
        //    analog::NETLIB_NAME(twoterm) m_ROUT;
        //    analog_input_t m_I;
        //    analog_output_t m_Q;
        //
        //    param_double_t m_p_RIN;
        //    param_double_t m_p_ROUT;
        //};


        /* -----------------------------------------------------------------------------
         * nld_function
         *
         * FIXME: Currently a proof of concept to get congo bongo working
         * ----------------------------------------------------------------------------- */
        //NETLIB_OBJECT(function)
        //{
        //    NETLIB_CONSTRUCTOR(function)
        //    , m_N(*this, "N", 1)
        //    , m_func(*this, "FUNC", "A0")
        //    , m_Q(*this, "Q")
        //    , m_compiled(this->name() + ".FUNCC", this, this->netlist().state())
        //    {
        //        std::vector<pstring> inps;
        //        for (int i=0; i < m_N(); i++)
        //        {
        //            pstring n = plib::pfmt("A{1}")(i);
        //            m_I.push_back(plib::make_unique<analog_input_t>(*this, n));
        //            inps.push_back(n);
        //            m_vals.push_back(0.0);
        //        }
        //        m_compiled.compile(inps, m_func());
        //    }
        //
        //protected:
        //
        //    NETLIB_RESETI();
        //    NETLIB_UPDATEI();
        //
        //private:
        //
        //    param_int_t m_N;
        //    param_str_t m_func;
        //    analog_output_t m_Q;
        //    std::vector<std::unique_ptr<analog_input_t>> m_I;
        //
        //    std::vector<double> m_vals;
        //    plib::pfunction m_compiled;
        //};


        // -----------------------------------------------------------------------------
        // nld_res_sw
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(res_sw)
        //{
        //public:
        //    NETLIB_CONSTRUCTOR(res_sw)
        //    , m_R(*this, "_R")
        //    , m_I(*this, "I")
        //    , m_RON(*this, "RON", 1.0)
        //    , m_ROFF(*this, "ROFF", 1.0E20)
        //    , m_last_state(*this, "m_last_state", 0)
        //    {
        //        register_subalias("1", m_R.m_P);
        //        register_subalias("2", m_R.m_N);
        //    }
        //
        //    analog::NETLIB_SUB(R_base) m_R;
        //    logic_input_t m_I;
        //    param_double_t m_RON;
        //    param_double_t m_ROFF;
        //
        //    NETLIB_RESETI();
        //    //NETLIB_UPDATE_PARAMI();
        //    NETLIB_UPDATEI();
        //
        //private:
        //    state_var<netlist_sig_t> m_last_state;
        //};
    } //namespace devices
} // namespace netlist
