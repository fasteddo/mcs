// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using nl_fptype_ops = mame.plib.constants_operators_double;


namespace mame.netlist
{
    namespace devices
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


            protected nld_base_proxy(netlist_state_t anetlist, string name, logic_t inout_proxied)
                : base(anetlist, name, inout_proxied.logic_family())
            {
                m_tp = null;
                m_tn = null;


                if (logic_family() == null)
                {
                    throw new nl_exception(nl_errstr_global.MF_NULLPTR_FAMILY_NP("nld_base_proxy"));
                }


                bool f = false;
                foreach (var pwr_sym in power_syms)
                {
                    string devname = inout_proxied.device().name();

                    var tp_ct = anetlist.setup().find_terminal(devname + "." + pwr_sym.first,
                            /*detail::terminal_type::INPUT,*/ false);
                    var tp_cn = anetlist.setup().find_terminal(devname + "." + pwr_sym.second,
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
                            log().warning.op(nl_errstr_global.MI_MULTIPLE_POWER_TERMINALS_ON_DEVICE(inout_proxied.device().name(),
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
                    throw new nl_exception(nl_errstr_global.MF_NO_POWER_TERMINALS_ON_DEVICE_2(name, anetlist.setup().de_alias(inout_proxied.device().name())));

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
            protected nld_base_a_to_d_proxy(netlist_state_t anetlist, string name, logic_input_t in_proxied)
                : base(anetlist, name, in_proxied)
            {
            }


            public abstract logic_output_t out_();
        }


        class nld_a_to_d_proxy : nld_base_a_to_d_proxy
        {
            logic_output_t m_Q;
            analog_input_t m_I;


            public nld_a_to_d_proxy(netlist_state_t anetlist, string name, logic_input_t in_proxied)
                : base(anetlist, name, in_proxied)
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
                throw new emu_unimplemented();
            }
        }


        // -----------------------------------------------------------------------------
        // nld_base_d_to_a_proxy
        // -----------------------------------------------------------------------------
        public abstract class nld_base_d_to_a_proxy : nld_base_proxy
        {
            protected nld_base_d_to_a_proxy(netlist_state_t anetlist, string name, logic_output_t out_proxied)
                : base(anetlist, name, out_proxied)
            {
            }


            // only used in setup
            public abstract logic_input_t in_();
        }


        class nld_d_to_a_proxy : nld_base_d_to_a_proxy
        {
            static readonly nl_fptype G_OFF = nlconst.cgmin();

            logic_input_t m_I;
            analog.nld_twoterm m_RP;  //analog::NETLIB_NAME(twoterm) m_RP;
            analog.nld_twoterm m_RN;  //analog::NETLIB_NAME(twoterm) m_RN;
            state_var<netlist_sig_t> m_last_state;


            public nld_d_to_a_proxy(netlist_state_t anetlist, string name, logic_output_t out_proxied)
                : base(anetlist, name, out_proxied)
            {
                m_I = new logic_input_t(this, "I", input);
                m_RP = new analog.nld_twoterm(this, "RP");
                m_RN = new analog.nld_twoterm(this, "RN");
                m_last_state = new state_var<netlist_sig_t>(this, "m_last_var", terminal_t.OUT_TRISTATE());


                register_subalias("Q", "RN.1");

                log().verbose.op("D/A Proxy: Found power terminals on device {0}", out_proxied.device().name());
                if (anetlist.is_extended_validation())
                {
                    // During validation, don't connect to terminals found
                    // This will cause terminals not connected to a rail net to
                    // fail connection stage.
                    connect(m_RN.N(), m_RP.P());
                }
                else
                {
                    connect(m_RN.N(), m_tn);
                    connect(m_RP.P(), m_tp);
                }
                connect(m_RN.P(), m_RP.N());
                //printf("vcc: %f\n", logic_family()->fixed_V());
            }


            public override logic_input_t in_() { return m_I; }


            public override detail.core_terminal_t proxy_term()
            {
                return m_RN.setup_P();
            }


            //NETLIB_RESETI();
            public override void reset()
            {
                ////m_Q.initial(0.0);
                m_last_state.op = terminal_t.OUT_TRISTATE();
                m_RN.reset();
                m_RP.reset();
                m_RN.set_G_V_I(plib.pglobal.reciprocal<nl_fptype, nl_fptype_ops>(logic_family().R_low()),
                        logic_family().low_offset_V(), nlconst.zero());
                m_RP.set_G_V_I(G_OFF,
                    nlconst.zero(),
                    nlconst.zero());
            }


            //NETLIB_HANDLERI(input);
            //NETLIB_HANDLER(d_to_a_proxy ,input)
            void input()
            {
                var state = m_I.op();
                if (state != m_last_state.op)
                {
                    // RN, RP are connected ...
                    m_RN.change_state(() =>  //m_RN.change_state([this, &state]()
                    {
                        //switch (state)
                        {
                            if (state == 0)  //case 0:
                            {
                                m_RN.set_G_V_I(plib.pglobal.reciprocal<nl_fptype, nl_fptype_ops>(logic_family().R_low()),
                                        logic_family().low_offset_V(), nlconst.zero());
                                m_RP.set_G_V_I(G_OFF,
                                    nlconst.zero(),
                                    nlconst.zero());
                            }//    break;
                            else if (state == 0)  //case 1:
                            {
                                m_RN.set_G_V_I(G_OFF,
                                    nlconst.zero(),
                                    nlconst.zero());
                                m_RP.set_G_V_I(plib.pglobal.reciprocal<nl_fptype, nl_fptype_ops>(logic_family().R_high()),
                                        logic_family().high_offset_V(), nlconst.zero());
                            }//    break;
                            else if (state == terminal_t.OUT_TRISTATE())  //case terminal_t.OUT_TRISTATE():
                            {
                                m_RN.set_G_V_I(G_OFF,
                                    nlconst.zero(),
                                    nlconst.zero());
                                m_RP.set_G_V_I(G_OFF,
                                    nlconst.zero(),
                                    nlconst.zero());
                            }//    break;
                            else  //default:
                            {
                                plib.pglobal.terminate("unknown state for proxy: this should never happen!");
                            }//    break;
                        }
                    });

                    m_last_state.op = state;
                }
            }
        }
    } //namespace devices
} // namespace netlist
