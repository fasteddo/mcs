// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<config::prefer_int128::value && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using object_t_props = mame.netlist.detail.property_store_t<mame.netlist.detail.object_t, string>;  //using props = property_store_t<object_t, pstring>;
using u32 = System.UInt32;

using static mame.netlist.nl_errstr_global;


namespace mame.netlist
{
    namespace detail
    {
        // -----------------------------------------------------------------------------
        // net_t
        // -----------------------------------------------------------------------------
        public class net_t : netlist_object_t
        {
            enum queue_status
            {
                DELAYED_DUE_TO_INACTIVE = 0,
                QUEUED,
                DELIVERED
            }


            state_var<netlist_sig_t> m_new_Q;
            state_var<netlist_sig_t> m_cur_Q;
            state_var<queue_status> m_in_queue;
            // FIXME: this needs to be saved as well
            plib.linked_list_t<core_terminal_t> m_list_active = new plib.linked_list_t<core_terminal_t>();  //plib::linked_list_t<core_terminal_t, 0> m_list_active;
            state_var<netlist_time_ext> m_next_scheduled_time;

            core_terminal_t m_rail_terminal;

#if NL_USE_INPLACE_CORE_TERMS
            plib::linked_list_t<core_terminal_t, 1> m_core_terms;
#endif


            protected net_t(netlist_state_t nl, string aname, core_terminal_t rail_terminal = null)
                : base(nl.exec(), aname)
            {
                m_new_Q = new state_var<netlist_sig_t>(this, "m_new_Q", (netlist_sig_t)0);
                m_cur_Q = new state_var<netlist_sig_t>(this, "m_cur_Q", (netlist_sig_t)0);
                m_in_queue = new state_var<queue_status>(this, "m_in_queue", queue_status.DELIVERED);
                m_next_scheduled_time = new state_var<netlist_time_ext>(this, "m_time", netlist_time_ext.zero());
                m_rail_terminal = rail_terminal;


                object_t_props.add(this, "");  //props::add(this, props::value_type());
            }


            //net_t(const net_t &) = delete;
            //net_t &operator=(const net_t &) = delete;
            //net_t(net_t &&) noexcept = delete;
            //net_t &operator=(net_t &&) noexcept = delete;

            //virtual ~net_t() noexcept = default;


            public virtual void reset()
            {
                m_next_scheduled_time.op = exec().time();
                m_in_queue.op = queue_status.DELIVERED;

                m_new_Q.op = 0;
                m_cur_Q.op = 0;

                var p = (analog_net_t)this;
                if (p != default)
                    p.set_Q_Analog(nlconst.zero());

                // rebuild m_list and reset terminals to active or analog out state

                m_list_active.clear();
                foreach (core_terminal_t ct in core_terms_copy())
                {
                    ct.reset();
                    if (ct.terminal_state() != logic_t.state_e.STATE_INP_PASSIVE)
                        m_list_active.push_back(ct);
                    ct.set_copied_input(m_cur_Q.op);
                }
            }


            // -----------------------------------------------------------------------------
            // Hot section
            //
            // Any changes below will impact performance.
            // -----------------------------------------------------------------------------

            public void toggle_new_Q() { m_new_Q.op = (m_cur_Q.op ^ 1); }


            public void toggle_and_push_to_queue(netlist_time delay)
            {
                toggle_new_Q();
                push_to_queue(delay);
            }


            void push_to_queue(netlist_time delay)
            {
                if (is_queued())
                    exec().queue_remove(this);

                m_next_scheduled_time.op = exec().time() + delay;
                if (config.avoid_noop_queue_pushes)
                    m_in_queue.op = (m_list_active.empty()
                                      ? queue_status.DELAYED_DUE_TO_INACTIVE
                                      : (m_new_Q != m_cur_Q
                                              ? queue_status.QUEUED
                                              : queue_status.DELIVERED));
                else
                    m_in_queue.op = m_list_active.empty()
                                     ? queue_status.DELAYED_DUE_TO_INACTIVE
                                     : queue_status.QUEUED;

                if (m_in_queue.op == queue_status.QUEUED)
                    exec().queue_push(new plib.queue_entry_t<netlist_time, net_t>(m_next_scheduled_time.op, this));
                else
                    update_inputs();
            }


            public bool is_queued() { return m_in_queue.op == queue_status.QUEUED; }


            // -----------------------------------------------------------------------------
            // Very hot
            // -----------------------------------------------------------------------------

            //template <bool KEEP_STATS>
            public void update_devs(bool KEEP_STATS)
            {
                //throw new emu_unimplemented();
#if false
                gsl_Expects(this->is_rail_net());
#endif

                m_in_queue.op = queue_status.DELIVERED; // mark as taken ...

                netlist_sig_t new_Q = m_new_Q.op;
                netlist_sig_t cur_Q = m_cur_Q.op;
                if (config.avoid_noop_queue_pushes || ((new_Q ^ cur_Q) != 0))
                {
                    m_cur_Q.op = new_Q;
                    var mask = (new_Q << (int)core_terminal_t.INP_LH_SHIFT)
                                      | (cur_Q << (int)core_terminal_t.INP_HL_SHIFT);

                    if (!KEEP_STATS)
                    {
                        foreach (var p in m_list_active)
                        {
                            p.set_copied_input(new_Q);
                            if (((u32)p.terminal_state() & mask) != 0)
                                p.run_delegate();
                        }
                    }
                    else
                    {
                        foreach (var p in m_list_active)
                        {
                            p.set_copied_input(new_Q);
                            var stats = p.device().stats();

                            throw new emu_unimplemented();
#if false
                            stats->m_stat_call_count.inc();
                            if ((p->terminal_state() & mask))
                            {
                                auto g(stats->m_stat_total_time.guard());
                                p->run_delegate();
                            }
#endif
                        }
                    }
                }
            }


            public netlist_time_ext next_scheduled_time() { return m_next_scheduled_time.op; }
            public void set_next_scheduled_time(netlist_time_ext ntime) { m_next_scheduled_time.op = ntime; }


            public bool is_rail_net() { return !(m_rail_terminal == null); }


            public core_terminal_t rail_terminal() { return m_rail_terminal; }


            public void add_to_active_list(core_terminal_t term)
            {
                if (!m_list_active.empty())
                {
                    term.set_copied_input(m_cur_Q.op);
                    m_list_active.push_front(term);
                }
                else
                {
                    m_list_active.push_front(term);
                    rail_terminal().device().do_inc_active();
                    if (m_in_queue.op == queue_status.DELAYED_DUE_TO_INACTIVE)
                    {
                        // if we avoid queue pushes we must test if m_cur_Q and
                        // m_new_Q are equal
                        if ((!config.avoid_noop_queue_pushes || (m_cur_Q != m_new_Q))
                            && (m_next_scheduled_time.op > exec().time()))
                        {
                            m_in_queue.op = queue_status.QUEUED; // pending
                            exec().queue_push(new plib.queue_entry_t<netlist_time, net_t>(m_next_scheduled_time.op, this));
                        }
                        else
                        {
                            m_in_queue.op = queue_status.DELIVERED;
                            m_cur_Q = m_new_Q;
                        }

                        update_inputs();
                    }
                    else
                    {
                        term.set_copied_input(m_cur_Q.op);
                    }
                }
            }


            public void remove_from_active_list(core_terminal_t term)
            {
                //throw new emu_unimplemented();
#if false
                gsl_Expects(!m_list_active.empty());
#endif
                m_list_active.remove(term);
                if (m_list_active.empty())
                {
                    if (true || config.avoid_noop_queue_pushes)
                    {
                        // All our connected outputs have signalled they no longer
                        // will act on input. We thus remove any potentially queued
                        // events and mark them.
                        // FIXME: May cause regression test to fail - revisit in
                        // this case
                        //
                        // This code is definitively needed for the
                        // AVOID_NOOP_QUEUE_PUSHES code path - therefore I left
                        // the if statement in and enabled it for all code paths
                        if (is_queued())
                        {
                            exec().queue_remove(this);
                            m_in_queue.op = queue_status.DELAYED_DUE_TO_INACTIVE;
                        }
                    }

                    rail_terminal().device().do_dec_active();
                }
            }


            // -----------------------------------------------------------------------------
            // setup stuff - cold
            // -----------------------------------------------------------------------------

            public bool is_logic() { return this is logic_net_t; }  //return dynamic_cast<const logic_net_t *>(this) != nullptr;
            public bool is_analog() { return this is analog_net_t; }  //return dynamic_cast<const analog_net_t *>(this) != nullptr;


            public void rebuild_list()     // rebuild m_list after a load
            {
                // rebuild m_list

                m_list_active.clear();
                foreach (core_terminal_t term in core_terms_ref())
                {
                    if (term.terminal_state() != logic_t.state_e.STATE_INP_PASSIVE)
                    {
                        m_list_active.push_back(term);
                        term.set_copied_input(m_cur_Q.op);
                    }
                }
            }


            ////public std.vector<core_terminal_t> core_terms() { return m_core_terms; }


            public void update_inputs()
            {
#if NL_USE_COPY_INSTEAD_OF_REFERENCE
                for (auto & term : m_core_terms)
                    term->m_Q = m_cur_Q;
#endif
                // nothing needs to be done if define not set
            }


            // -----------------------------------------------------------------
            // net management
            // -----------------------------------------------------------------

            public std.vector<detail.core_terminal_t> core_terms_copy()  //std::vector<detail::core_terminal_t *> core_terms_copy() noexcept(false)
            {
                std.vector<detail.core_terminal_t> ret = new std.vector<core_terminal_t>(core_terms_ref().size());
                ret.AddRange(core_terms_ref());  //std::copy(core_terms_ref().begin(), core_terms_ref().end(), ret.begin());
                return ret;
            }

            public void remove_terminal(detail.core_terminal_t term) { throw new emu_unimplemented(); }


            public void remove_all_terminals()
            {
                state().core_terms(this).clear();
            }


            public void add_terminal(detail.core_terminal_t terminal)
            {
                foreach (detail.core_terminal_t t in core_terms_ref())
                {
                    if (t == terminal)
                    {
                        state().log().fatal.op(MF_NET_1_DUPLICATE_TERMINAL_2(this.name(), t.name()));
                        throw new nl_exception(MF_NET_1_DUPLICATE_TERMINAL_2(this.name(), t.name()));
                    }
                }

                terminal.set_net(this);

                state().core_terms(this).push_back(terminal);
            }


            public bool core_terms_empty()
            {
                return core_terms_ref().empty();
            }


            // only used for logic nets
            public netlist_sig_t Q() { return m_cur_Q.op; }


            // only used for logic nets
            public void initial(netlist_sig_t val)
            {
                m_cur_Q.op = m_new_Q.op = val;
                update_inputs();
            }


            // only used for logic nets
            public void set_Q_and_push(netlist_sig_t newQ, netlist_time delay)
            {
                //throw new emu_unimplemented();
#if false
                gsl_Expects(delay >= netlist_time::zero());
#endif

                if (newQ != m_new_Q.op)
                {
                    m_new_Q.op = newQ;
                    push_to_queue(delay);
                }
            }


            // only used for logic nets
            public void set_Q_time(netlist_sig_t newQ, netlist_time_ext at)
            {
                //throw new emu_unimplemented();
#if false
                gsl_Expects(at >= netlist_time_ext::zero());
#endif

                if (newQ != m_new_Q.op)
                {
                    m_in_queue.op = queue_status.DELAYED_DUE_TO_INACTIVE;
                    m_next_scheduled_time.op = at;
                    m_cur_Q.op = m_new_Q.op = newQ;
                    update_inputs();
                }
                else
                {
                    m_cur_Q.op = newQ;
                    update_inputs();
                }
            }


            std.vector<detail.core_terminal_t> core_terms_ref()
            {
                return state().core_terms(this);
            }
        }
    } // namespace detail


    //class analog_net_t : public detail::net_t


    public class logic_net_t : detail.net_t
    {
        //using detail::net_t::initial;
        //using detail::net_t::Q;
        //using detail::net_t::set_Q_and_push;
        //using detail::net_t::set_Q_time;


        public logic_net_t(netlist_state_t nl, string aname, detail.core_terminal_t rail_terminal = null)
            : base(nl, aname, rail_terminal)
        {
        }
    }
}
