// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using param_fp_t = mame.netlist.param_num_t<System.Double, mame.netlist.param_num_t_operators_double>;  //using param_fp_t = param_num_t<nl_fptype>;
using param_logic_t = mame.netlist.param_num_t<bool, mame.netlist.param_num_t_operators_bool>;  //using param_logic_t = param_num_t<bool>;
using unsigned = System.UInt32;

using static mame.nl_factory_global;


namespace mame.netlist.devices
{
    // -----------------------------------------------------------------------------
    // clock
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(clock)
    class nld_clock : device_t
    {
        //NETLIB_DEVICE_IMPL(clock,               "CLOCK",                  "FREQ")
        public static readonly factory.constructor_ptr_t decl_clock = NETLIB_DEVICE_IMPL<nld_clock>("CLOCK", "FREQ");


        logic_input_t m_feedback;
        logic_output_t m_Q;

        param_fp_t m_freq;
        netlist_time m_inc;

        nld_power_pins m_supply;


        //NETLIB_CONSTRUCTOR(clock)
        //detail.family_setter_t m_famsetter;
        //template <class CLASS>
        public nld_clock(object owner, string name)
            : base(owner, name)
        {
            m_feedback = new logic_input_t(this, "FB", fb);
            m_Q = new logic_output_t(this, "Q");
            m_freq = new param_fp_t(this, "FREQ", nlconst.magic(7159000.0 * 5.0));
            m_supply = new nld_power_pins(this);


            m_inc = netlist_time.from_fp(plib.pg.reciprocal(m_freq.op() * nlconst.two()));

            connect("FB", "Q");
        }


        //NETLIB_UPDATE_PARAMI()
        public override void update_param()
        {
            throw new emu_unimplemented();
            //m_inc = netlist_time::from_fp(plib::reciprocal(m_freq()*nlconst::two()));
        }


        //NETLIB_HANDLERI(fb)
        void fb()
        {
            throw new emu_unimplemented();
            //m_Q.push(m_feedback() ^ 1, m_inc);
        }
    }


    // -----------------------------------------------------------------------------
    // varclock
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(varclock)


    // -----------------------------------------------------------------------------
    // extclock
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(extclock)


    // -----------------------------------------------------------------------------
    // Special support devices ...
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(logic_input)
    class nld_logic_input : device_t
    {
        //NETLIB_DEVICE_IMPL(logic_input, "LOGIC_INPUT", "IN,FAMILY")
        public static readonly factory.constructor_ptr_t decl_logic_input = NETLIB_DEVICE_IMPL<nld_logic_input>("LOGIC_INPUT", "IN,FAMILY");

        //NETLIB_DEVICE_IMPL_ALIAS(logic_input_ttl, logic_input, "TTL_INPUT", "IN")
        public static readonly factory.constructor_ptr_t decl_logic_input_ttl = NETLIB_DEVICE_IMPL_ALIAS<nld_logic_input>("logic_input_ttl", "TTL_INPUT", "IN");


        logic_output_t m_Q;

        param_logic_t m_IN;
        nld_power_pins m_supply;  //NETLIB_NAME(power_pins) m_supply;


        //NETLIB_CONSTRUCTOR(logic_input)
        //detail.family_setter_t m_famsetter;
        //template <class CLASS>
        public nld_logic_input(object owner, string name)
            : base(owner, name)
        {
            m_Q = new logic_output_t(this, "Q");
            m_IN = new param_logic_t(this, "IN", false);
            m_supply = new nld_power_pins(this);
        }


        //NETLIB_RESETI();
        public override void reset() { m_Q.initial(0); }

        //NETLIB_UPDATE_PARAMI();
        public override void update_param()
        {
            //printf("%s %d\n", name().c_str(), m_IN());
            m_Q.push((netlist_sig_t)((m_IN.op() ? 1 : 0) & 1), netlist_time.from_nsec(1));
        }
    }


    //template<std::size_t N>
    //NETLIB_OBJECT(logic_inputN)


    //NETLIB_OBJECT(analog_input)
    class nld_analog_input : device_t
    {
        //NETLIB_DEVICE_IMPL(analog_input,        "ANALOG_INPUT",           "IN")
        public static readonly factory.constructor_ptr_t decl_analog_input = NETLIB_DEVICE_IMPL<nld_analog_input>("ANALOG_INPUT", "IN");


        analog_output_t m_Q;
        param_fp_t m_IN;


        //NETLIB_CONSTRUCTOR(analog_input)
        //detail.family_setter_t m_famsetter;
        //template <class CLASS>
        public nld_analog_input(object owner, string name)
            : base(owner, name)
        {
            m_Q = new analog_output_t(this, "Q");
            m_IN = new param_fp_t(this, "IN", nlconst.zero());
        }


        //NETLIB_RESETI();
        public override void reset() { m_Q.initial(nlconst.zero()); }

        //NETLIB_UPDATE_PARAMI();
        public override void update_param() { m_Q.push(m_IN.op()); }
    }


    // -----------------------------------------------------------------------------
    // nld_frontier
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(frontier)
    class nld_frontier : device_t
    {
        //NETLIB_DEVICE_IMPL(frontier,            "FRONTIER_DEV",           "+I,+G,+Q")
        public static readonly factory.constructor_ptr_t decl_frontier = NETLIB_DEVICE_IMPL<nld_frontier>("FRONTIER_DEV", "+I,+G,+Q");


        analog.nld_twoterm m_RIN;
        analog.nld_twoterm m_ROUT;
        analog_input_t m_I;
        analog_output_t m_Q;

        param_fp_t m_p_RIN;
        param_fp_t m_p_ROUT;


        //NETLIB_CONSTRUCTOR(frontier)
        //detail.family_setter_t m_famsetter;
        //template <class CLASS>
        public nld_frontier(object owner, string name)
            : base(owner, name)
        {
            m_RIN = new analog.nld_twoterm(this, "m_RIN", input); // FIXME: does not look right
            m_ROUT = new analog.nld_twoterm(this, "m_ROUT", input); // FIXME: does not look right
            m_I = new analog_input_t(this, "_I", input);
            m_Q = new analog_output_t(this, "_Q");
            m_p_RIN = new param_fp_t(this, "RIN", nlconst.magic(1.0e6));
            m_p_ROUT = new param_fp_t(this, "ROUT", nlconst.magic(50.0));


            register_subalias("I", "m_RIN.1");
            register_subalias("G", "m_RIN.2");
            connect("_I", "m_RIN.1");

            register_subalias("_OP", "m_ROUT.1");
            register_subalias("Q", "m_ROUT.2");
            connect("_Q", "m_ROUT.1");
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            throw new emu_unimplemented();
            ////printf("%s: in %f out %f\n", name().c_str(), m_p_RIN(), m_p_ROUT());
            //m_RIN.set_G_V_I(plib::reciprocal(m_p_RIN()),0,0);
            //m_ROUT.set_G_V_I(plib::reciprocal(m_p_ROUT()),0,0);
        }


        //NETLIB_HANDLERI(input)
        void input()
        {
            throw new emu_unimplemented();
            //m_Q.push(m_I());
        }
    }


    /* -----------------------------------------------------------------------------
        * nld_function
        *
        * FIXME: Currently a proof of concept to get congo bongo working
        * ----------------------------------------------------------------------------- */
    //NETLIB_OBJECT(function)


    // -----------------------------------------------------------------------------
    // nld_sys_dsw1
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(sys_dsw1)
    class nld_sys_dsw1 : device_t
    {
        //NETLIB_DEVICE_IMPL(sys_dsw1,            "SYS_DSW",                "+I,+1,+2")
        public static readonly factory.constructor_ptr_t decl_sys_dsw1 = NETLIB_DEVICE_IMPL<nld_sys_dsw1>("SYS_DSW", "+I,+1,+2");


        param_fp_t m_RON;
        param_fp_t m_ROFF;


        analog.nld_R_base m_R;
        logic_input_t m_I;

        state_var<netlist_sig_t> m_last_state;


        //NETLIB_CONSTRUCTOR(sys_dsw1)
        //detail.family_setter_t m_famsetter;
        //template <class CLASS>
        public nld_sys_dsw1(object owner, string name)
            : base(owner, name)
        {
            m_RON = new param_fp_t(this, "RON", nlconst.one());
            m_ROFF = new param_fp_t(this, "ROFF", nlconst.magic(1.0E20));
            m_R = new analog.nld_R_base(this, "_R");
            m_I = new logic_input_t(this, "I", input);
            m_last_state = new state_var<netlist_sig_t>(this, "m_last_state", 0);


            register_subalias("1", "_R.1");
            register_subalias("2", "_R.2");
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            m_last_state.op = 0;
            m_R.set_R(m_ROFF.op());
        }


        //NETLIB_UPDATE_PARAMI();


        //FIXME: used by 74123

        //const terminal_t &P() const noexcept { return m_R.P(); }
        //const terminal_t &N() const noexcept { return m_R.N(); }
        //const logic_input_t &I() const noexcept { return m_I; }


        //NETLIB_HANDLERI(input)
        void input()
        {
            netlist_sig_t state = m_I.op();
            if (state != m_last_state.op)
            {
                m_last_state.op = state;
                nl_fptype R = (state != 0) ? m_RON.op() : m_ROFF.op();

                m_R.change_state(() => //[this, &R]()
                {
                    m_R.set_R(R);
                });
            }
        }
    }


    // -----------------------------------------------------------------------------
    // nld_sys_dsw2
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(sys_dsw2)


    // -----------------------------------------------------------------------------
    // nld_sys_comp
    // -----------------------------------------------------------------------------
    //NETLIB_OBJECT(sys_compd)


    // -----------------------------------------------------------------------------
    // nld_sys_noise - noise source
    //
    // An externally clocked noise source.
    // -----------------------------------------------------------------------------
    //template <typename E, template<class> class D>
    //NETLIB_OBJECT(sys_noise)
    class nld_sys_noise<E, D, D_ops> : device_t
        where D_ops : plib.distribution_ops<D>, new()
    {
        static readonly D_ops ops = new D_ops();


        //using engine = E;
        //using distribution = D<nl_fptype>;


        analog.nld_twoterm m_T;
        logic_input_t m_I;
        param_fp_t m_RI;
        param_fp_t m_sigma;
        state_var<E> m_mt;  //state_var<engine> m_mt;
        state_var<D> m_dis;  //state_var<distribution> m_dis;


        //NETLIB_CONSTRUCTOR(sys_noise)
        //detail.family_setter_t m_famsetter;
        //template <class CLASS>
        public nld_sys_noise(object owner, string name)
            : base(owner, name)
        {
            m_T = new analog.nld_twoterm(this, "m_T");
            m_I = new logic_input_t(this, "I", input);
            m_RI = new param_fp_t(this, "RI", nlconst.magic(0.1));
            m_sigma = new param_fp_t(this, "SIGMA", nlconst.zero());
            m_mt = new state_var<E>(this, "m_mt");
            m_dis = new state_var<D>(this, "m_dis", ops.new_(m_sigma.op()));


            register_subalias("1", "m_T.1");
            register_subalias("2", "m_T.2");
        }


        //NETLIB_HANDLERI(input)
        void input()
        {
            throw new emu_unimplemented();
            //nl_fptype val = m_dis()(m_mt());
            //m_T.change_state([this, val]()
            //{
            //    m_T.set_G_V_I(plib::reciprocal(m_RI()), val, nlconst::zero());
            //});
        }


        //NETLIB_RESETI()
        public override void reset()
        {
            throw new emu_unimplemented();
            //m_T.set_G_V_I(plib::reciprocal(m_RI()), nlconst::zero(), nlconst::zero());
        }
    }
}
