// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using base_device_t_constructor_param_t = mame.netlist.core_device_data_t;  //using constructor_param_t = base_device_param_t;  //using base_device_param_t = const base_device_data_t &;  //using base_device_data_t = core_device_data_t;
using device_param_t = mame.netlist.core_device_data_t;  //using device_param_t = const device_data_t &;  //using device_data_t = base_device_data_t;  //using base_device_data_t = core_device_data_t;
using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;

using static mame.netlist.nl_errstr_global;


namespace mame.netlist.devices
{
    // -----------------------------------------------------------------------------
    // nld_base_proxy
    // -----------------------------------------------------------------------------
    public abstract class nld_base_proxy : device_t
    {
        static readonly std.vector<std.pair<string, string>> power_syms = new std.vector<std.pair<string, string>>() { new std.pair<string, string>("VCC", "VEE"), new std.pair<string, string>("VCC", "GND"), new std.pair<string, string>("VDD", "VSS") };


        // FIXME: these should be core_terminal_t and only used for connecting
        //        inputs. Fix, once the ugly hacks have been removed
        protected analog_t m_tp;
        protected analog_t m_tn;


        protected nld_base_proxy(device_param_t data, logic_t inout_proxied)
            : base(data, inout_proxied.logic_family())
        {
            m_tp = null;
            m_tn = null;


            if (logic_family() == null)
            {
                throw new nl_exception(MF_NULLPTR_FAMILY_NP("nld_base_proxy"));
            }


            bool f = false;
            foreach (var pwr_sym in power_syms)
            {
                string devname = inout_proxied.device().name();

                var tp_ct = state().setup().find_terminal(devname + "." + pwr_sym.first,
                        /*detail::terminal_type::INPUT,*/ false);
                var tp_cn = state().setup().find_terminal(devname + "." + pwr_sym.second,
                    /*detail::terminal_type::INPUT,*/ false);
                if (tp_ct != null && tp_cn != null)
                {
                    if (!tp_ct.is_analog())
                        throw new nl_exception(new plib.pfmt("Not an analog terminal: {0}").op(tp_ct.name()));
                    if (!tp_cn.is_analog())
                        throw new nl_exception(new plib.pfmt("Not an analog terminal: {0}").op(tp_cn.name()));

                    var tp_t = (analog_t)tp_ct;
                    var tn_t = (analog_t)tp_cn;
                    if (f && (tp_t != null && tn_t != null))
                    {
                        log().warning.op(MI_MULTIPLE_POWER_TERMINALS_ON_DEVICE(inout_proxied.device().name(),
                            m_tp.name(), m_tn.name(),
                            tp_t != null ? tp_t.name() : "",
                            tn_t != null ? tn_t.name() : ""));
                    }
                    else if (tp_t != null && tn_t != null)
                    {
                        m_tp = tp_t;
                        m_tn = tn_t;
                        f = true;
                    }
                }
            }

            if (!f)
                throw new nl_exception(MF_NO_POWER_TERMINALS_ON_DEVICE_2(name(), state().setup().de_alias(inout_proxied.device().name())));

            log().verbose.op("D/A Proxy: Found power terminals on device {0}", inout_proxied.device().name());
        }


        // only used during setup
        public abstract detail.core_terminal_t proxy_term();
    }


    // -----------------------------------------------------------------------------
    // nld_a_to_d_proxy
    // -----------------------------------------------------------------------------
    public abstract class nld_base_a_to_d_proxy : nld_base_proxy
    {
        protected nld_base_a_to_d_proxy(device_param_t data, logic_input_t in_proxied)
            : base(data, in_proxied)
        {
        }


        public abstract logic_output_t out_();
    }


    class nld_a_to_d_proxy : nld_base_a_to_d_proxy
    {
        logic_output_t m_Q;
        analog_input_t m_I;


        public nld_a_to_d_proxy(device_param_t data, logic_input_t in_proxied)
            : base(data, in_proxied)
        {
            m_Q = new logic_output_t(this, "Q");
            m_I = new analog_input_t(this, "I", input);
        }


        public override logic_output_t out_() { return m_Q; }


        public override detail.core_terminal_t proxy_term()
        {
            return m_I;
        }


        //NETLIB_RESETI();


        //NETLIB_HANDLERI(input);
        //NETLIB_HANDLER(a_to_d_proxy, input)
        void input()
        {
            var v = m_I.Q_Analog();
            var vn = m_tn.net().Q_Analog();
            var vp = m_tp.net().Q_Analog();

            if (logic_family().is_above_high_threshold_V(v, vn, vp))
            {
                out_().push(1, netlist_time.quantum());
            }
            else if (logic_family().is_below_low_threshold_V(v, vn, vp))
            {
                out_().push(0, netlist_time.quantum());
            }
            else
            {
                // do nothing
            }
        }
    }


    // -----------------------------------------------------------------------------
    // nld_base_d_to_a_proxy
    // -----------------------------------------------------------------------------
    public abstract class nld_base_d_to_a_proxy : nld_base_proxy
    {
        protected nld_base_d_to_a_proxy(device_param_t data, logic_output_t out_proxied)
            : base(data, out_proxied)
        {
        }


        // only used in setup
        public abstract logic_input_t in_();
    }


    class nld_d_to_a_proxy : nld_base_d_to_a_proxy
    {
        static readonly nl_fptype G_OFF = nlconst.cgmin();

        logic_input_t m_I;
        sub_device_wrapper<analog.nld_two_terminal> m_RP;  //NETLIB_SUB_NS(analog, two_terminal) m_RP;
        sub_device_wrapper<analog.nld_two_terminal> m_RN;  //NETLIB_SUB_NS(analog, two_terminal) m_RN;
        state_var<netlist_sig_t> m_last_state;


        public nld_d_to_a_proxy(device_param_t data, logic_output_t out_proxied)
            : base(data, out_proxied)
        {
            m_I = new logic_input_t(this, "I", input);
            m_RP = new sub_device_wrapper<analog.nld_two_terminal>(this, new analog.nld_two_terminal(new base_device_t_constructor_param_t(this, "RP")));
            m_RN = new sub_device_wrapper<analog.nld_two_terminal>(this, new analog.nld_two_terminal(new base_device_t_constructor_param_t(this, "RN")));
            m_last_state = new state_var<netlist_sig_t>(this, "m_last_var", terminal_t.OUT_TRISTATE());


            register_sub_alias("Q", "RN.1");

            connect(m_RN.op().N(), m_tn);
            connect(m_RP.op().P(), m_tp);

            connect(m_RN.op().P(), m_RP.op().N());
        }


        public override logic_input_t in_() { return m_I; }


        public override detail.core_terminal_t proxy_term()
        {
            return m_RN.op().setup_P();
        }


        //NETLIB_RESETI();
        public override void reset()
        {
            ////m_Q.initial(0.0);
            m_last_state.op = terminal_t.OUT_TRISTATE();
            m_RN.op().reset();
            m_RP.op().reset();
            m_RN.op().set_G_V_I(plib.pg.reciprocal(logic_family().R_low()),logic_family().low_offset_V(), nlconst.zero());
            m_RP.op().set_G_V_I(G_OFF, nlconst.zero(), nlconst.zero());
        }


        //NETLIB_HANDLERI(input);
        //NETLIB_HANDLER(d_to_a_proxy ,input)
        void input()
        {
            var state = m_I.op();
            if (state != m_last_state.op)
            {
                // RN, RP are connected ...
                m_RN.op().change_state(() =>  //m_RN.change_state([this, &state]()
                {
                    //switch (state)
                    {
                        if (state == 0)  //case 0:
                        {
                            m_RN.op().set_G_V_I(plib.pg.reciprocal(logic_family().R_low()),
                                    logic_family().low_offset_V(), nlconst.zero());
                            m_RP.op().set_G_V_I(G_OFF,
                                nlconst.zero(),
                                nlconst.zero());
                        }//    break;
                        else if (state == 0)  //case 1:
                        {
                            m_RN.op().set_G_V_I(G_OFF,
                                nlconst.zero(),
                                nlconst.zero());
                            m_RP.op().set_G_V_I(plib.pg.reciprocal(logic_family().R_high()),
                                    logic_family().high_offset_V(), nlconst.zero());
                        }//    break;
                        else if (state == terminal_t.OUT_TRISTATE())  //case terminal_t.OUT_TRISTATE():
                        {
                            m_RN.op().set_G_V_I(G_OFF,
                                nlconst.zero(),
                                nlconst.zero());
                            m_RP.op().set_G_V_I(G_OFF,
                                nlconst.zero(),
                                nlconst.zero());
                        }//    break;
                        else  //default:
                        {
                            plib.pg.terminate("unknown state for proxy: this should never happen!");
                        }//    break;
                    }
                });

                m_last_state.op = state;
            }
        }
    }
}
