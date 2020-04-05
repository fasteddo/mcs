// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_base_t = mame.netlist.netlist_state_t;


namespace mame.netlist
{
    namespace devices
    {
        // -----------------------------------------------------------------------------
        // nld_base_proxy
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT(base_proxy)
        public class nld_base_proxy : device_t
        {
            logic_t m_term_proxied;
            detail.core_terminal_t m_proxy_term;


            protected nld_base_proxy(netlist_state_t anetlist, string name, logic_t inout_proxied, detail.core_terminal_t proxy_inout)
                : base(anetlist, name)
            {
                m_logic_family = inout_proxied.logic_family();
                m_term_proxied = inout_proxied;
                m_proxy_term = proxy_inout;
            }


            protected detail.core_terminal_t proxy_term_prop { get { return m_proxy_term; } set { m_proxy_term = value; } }


            //logic_t &term_proxied() const { return *m_term_proxied; }
            public detail.core_terminal_t proxy_term() { return m_proxy_term; }
        }


        // -----------------------------------------------------------------------------
        // nld_a_to_d_proxy
        // -----------------------------------------------------------------------------
        //NETLIB_OBJECT_DERIVED(base_a_to_d_proxy, base_proxy)
        public class nld_base_a_to_d_proxy : nld_base_proxy
        {
            logic_output_t m_Q;


            protected nld_base_a_to_d_proxy(netlist_state_t anetlist, string name, logic_input_t in_proxied, detail.core_terminal_t in_proxy)
                : base(anetlist, name, in_proxied, in_proxy)
            {
                m_Q = new logic_output_t(this, "Q");
            }


            public virtual logic_output_t out_() { return m_Q; }
        }


        //NETLIB_OBJECT_DERIVED(a_to_d_proxy, base_a_to_d_proxy)
        class nld_a_to_d_proxy : nld_base_a_to_d_proxy
        {
            analog_input_t m_I;


            public nld_a_to_d_proxy(netlist_state_t anetlist, string name, logic_input_t in_proxied)
                : base(anetlist, name, in_proxied, null)// m_I)
            {
                m_I = new analog_input_t(this, "I");

                // set proxy_term after variable is initialized
                proxy_term_prop = m_I;
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
        public class nld_base_d_to_a_proxy : nld_base_proxy
        {
            logic_input_t m_I;


            protected nld_base_d_to_a_proxy(netlist_state_t anetlist, string name, logic_output_t out_proxied, detail.core_terminal_t proxy_out)
                : base(anetlist, name, out_proxied, proxy_out)
            {
                m_I = new logic_input_t(this, "I");
            }


            public virtual logic_input_t in_() { return m_I; }
        }


        //NETLIB_OBJECT_DERIVED(d_to_a_proxy, base_d_to_a_proxy)
        class nld_d_to_a_proxy : nld_base_d_to_a_proxy
        {
            //analog_output_t m_GNDHack;  // FIXME: Long term, we need to connect proxy gnd to device gnd
            //analog::NETLIB_SUB(twoterm) m_RV;
            //state_var<int> m_last_state;
            //bool m_is_timestep;


            public nld_d_to_a_proxy(netlist_base_t anetlist, string name, logic_output_t out_proxied)
                : base(anetlist, name, out_proxied, null)  //m_RV.m_P)
            {
                throw new emu_unimplemented();


                // TODO - add this code after initialization
                // set proxy_term after variable is initialized
                //proxy_term_prop = m_RV.m_p;
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
