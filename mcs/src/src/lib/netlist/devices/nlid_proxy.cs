// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    namespace devices
    {
        // -----------------------------------------------------------------------------
        // nld_base_proxy
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(base_proxy)
        public abstract class nld_base_proxy : device_t
        {
            // FIXME: these should be core_terminal_t and only used for connecting
            //        inputs. Fix, once the ugly hacks have been removed
            analog_t m_tp;
            analog_t m_tn;


            protected nld_base_proxy(netlist_state_t anetlist, string name, logic_t inout_proxied)
                : base(anetlist, name)
            {
                m_tp = null;
                m_tn = null;


                m_logic_family = inout_proxied.logic_family();

                std.vector<std.pair<string, string>> power_syms = new std.vector<std.pair<string, string>>() { new std.pair<string, string>("VCC", "VEE"), new std.pair<string, string>("VCC", "GND"), new std.pair<string, string>("VDD", "VSS") };

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
                        if (tp_ct != null && !tp_ct.is_analog())
                            throw new nl_exception(new plib.pfmt("Not an analog terminal: {0}").op(tp_ct.name()));
                        if (tp_cn != null && !tp_cn.is_analog())
                            throw new nl_exception(new plib.pfmt("Not an analog terminal: {0}").op(tp_cn.name()));

                        var tp_t = (analog_t)tp_ct;
                        var tn_t = (analog_t)tp_cn;
                        if (f && (tp_t != null && tn_t != null))
                            log().warning.op(nl_errstr_global.MI_MULTIPLE_POWER_TERMINALS_ON_DEVICE(inout_proxied.device().name(),
                                m_tp.name(), m_tn.name(),
                                tp_t != null ? tp_t.name() : "",
                                tn_t != null ? tn_t.name() : ""));
                        else if (tp_t != null && tn_t != null)
                        {
                            m_tp = tp_t;
                            m_tn = tn_t;
                            f = true;
                        }
                    }
                }
                if (!f)
                    log().error.op(nl_errstr_global.MI_NO_POWER_TERMINALS_ON_DEVICE_2(name, anetlist.setup().de_alias(inout_proxied.device().name())));
                else
                    log().verbose.op("D/A Proxy: Found power terminals on device {0}", inout_proxied.device().name());
            }


            // only used during setup
            public abstract detail.core_terminal_t proxy_term();
        }


        // -----------------------------------------------------------------------------
        // nld_a_to_d_proxy
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(base_a_to_d_proxy, base_proxy)
        public abstract class nld_base_a_to_d_proxy : nld_base_proxy
        {
            protected nld_base_a_to_d_proxy(netlist_state_t anetlist, string name, logic_input_t in_proxied)
                : base(anetlist, name, in_proxied)
            {
            }


            public abstract logic_output_t out_();
        }


        //NETLIB_OBJECT_DERIVED(a_to_d_proxy, base_a_to_d_proxy)
        class nld_a_to_d_proxy : nld_base_a_to_d_proxy
        {
            logic_output_t m_Q;
            analog_input_t m_I;


            public nld_a_to_d_proxy(netlist_state_t anetlist, string name, logic_input_t in_proxied)
                : base(anetlist, name, in_proxied)
            {
                m_Q = new logic_output_t(this, "Q");
                m_I = new analog_input_t(this, "I");
            }


            public override logic_output_t out_() { return m_Q; }


            public override detail.core_terminal_t proxy_term()
            {
                return m_I;
            }


            //NETLIB_RESETI();
            public override void reset()
            {
                throw new emu_unimplemented();
            }


            //NETLIB_UPDATEI();
            protected override void update()
            {
                throw new emu_unimplemented();
            }
        }


        // -----------------------------------------------------------------------------
        // nld_base_d_to_a_proxy
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(base_d_to_a_proxy, base_proxy)
        public abstract class nld_base_d_to_a_proxy : nld_base_proxy
        {
            protected nld_base_d_to_a_proxy(netlist_state_t anetlist, string name, logic_output_t out_proxied)
                : base(anetlist, name, out_proxied)
            {
            }


            // only used in setup
            public abstract logic_input_t in_();
        }


        //NETLIB_OBJECT_DERIVED(d_to_a_proxy, base_d_to_a_proxy)
        class nld_d_to_a_proxy : nld_base_d_to_a_proxy
        {
            //static constexpr const nl_fptype G_OFF = nlconst::magic(1e-9);

            logic_input_t m_I;
            //analog::NETLIB_NAME(twoterm) m_RP;
            analog.nld_twoterm m_RN;  //analog::NETLIB_NAME(twoterm) m_RN;
            //state_var<int> m_last_state;
            //bool m_is_timestep;


            public nld_d_to_a_proxy(netlist_state_t anetlist, string name, logic_output_t out_proxied)
                : base(anetlist, name, out_proxied)
            {
                throw new emu_unimplemented();
            }


            public override logic_input_t in_() { return m_I; }


            public override detail.core_terminal_t proxy_term()
            {
                return m_RN.P;
            }


            //NETLIB_RESETI();
            public override void reset()
            {
                throw new emu_unimplemented();
            }

            //NETLIB_UPDATEI();
            protected override void update()
            {
                throw new emu_unimplemented();
            }
        }
    } //namespace devices
} // namespace netlist
