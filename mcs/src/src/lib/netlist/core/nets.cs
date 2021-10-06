// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using netlist_sig_t = System.UInt32;  //using netlist_sig_t = std::uint32_t;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<NL_PREFER_INT128 && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using object_t_props = mame.netlist.detail.property_store_t<mame.netlist.detail.object_t, string>;  //using props = property_store_t<object_t, pstring>;
using u32 = System.UInt32;


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
            plib.linkedlist_t<core_terminal_t> m_list_active = new plib.linkedlist_t<core_terminal_t>();
            state_var<netlist_time_ext> m_next_scheduled_time;

            core_terminal_t m_railterminal;


            protected net_t(netlist_state_t nl, string aname, core_terminal_t railterminal = null)
                : base(nl.exec(), aname)
            {
                m_new_Q = new state_var<netlist_sig_t>(this, "m_new_Q", (netlist_sig_t)0);
                m_cur_Q = new state_var<netlist_sig_t>(this, "m_cur_Q", (netlist_sig_t)0);
                m_in_queue = new state_var<queue_status>(this, "m_in_queue", queue_status.DELIVERED);
                m_next_scheduled_time = new state_var<netlist_time_ext>(this, "m_time", netlist_time_ext.zero());
                m_railterminal = railterminal;


                object_t_props.add(this, "");  //props::add(this, props::value_type());
            }


            //PCOPYASSIGNMOVE(net_t, delete)

            //virtual ~net_t() noexcept = default;


            public virtual void reset()
            {
                m_next_scheduled_time.op = exec().time();
                m_in_queue.op = queue_status.DELIVERED;

                m_new_Q.op = 0;
                m_cur_Q.op = 0;

                var p = this is analog_net_t ? (analog_net_t)this : null;  //auto *p = dynamic_cast<analog_net_t *>(this);

                if (p != null)
                    p.set_Q_Analog(nlconst.zero());

                // rebuild m_list and reset terminals to active or analog out state

                m_list_active.clear();
                foreach (core_terminal_t ct in exec().nlstate().core_terms(this))
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
                if (!!is_queued())
                    exec().qremove(this);

                m_next_scheduled_time.op = exec().time() + delay;
#if (AVOID_NOOP_QUEUE_PUSHES)
                m_in_queue = (m_list_active.empty() ? queue_status::DELAYED_DUE_TO_INACTIVE
                    : (m_new_Q != m_cur_Q ? queue_status::QUEUED : queue_status::DELIVERED));
                if (m_in_queue == queue_status::QUEUED)
                    exec().qpush(m_next_scheduled_time, this);
                else
                    update_inputs();
#else
                m_in_queue.op = m_list_active.empty() ? queue_status.DELAYED_DUE_TO_INACTIVE : queue_status.QUEUED;
                if (m_in_queue.op == queue_status.QUEUED)
                    exec().qpush(new plib.pqentry_t<netlist_time, net_t>(m_next_scheduled_time.op, this));
                else
                    update_inputs();
#endif
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
#if !AVOID_NOOP_QUEUE_PUSHES
                if ((new_Q ^ cur_Q) != 0)
#endif
                {
                    var mask = (new_Q << (int)core_terminal_t.INP_LH_SHIFT) | (cur_Q << (int)core_terminal_t.INP_HL_SHIFT);
                    m_cur_Q.op = new_Q;

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
                            if (((u32)p.terminal_state() & mask) != 0)
                            {
                                var g = stats->m_stat_total_time.guard();
                                p.run_delegate();
                            }
#endif
                        }
                    }
                }
            }


            public netlist_time_ext next_scheduled_time() { return m_next_scheduled_time.op; }
            public void set_next_scheduled_time(netlist_time_ext ntime) { m_next_scheduled_time.op = ntime; }


            public bool is_rail_net() { return !(m_railterminal == null); }


            public core_terminal_t railterminal() { return m_railterminal; }


            //void add_to_active_list(core_terminal_t &term) noexcept;
            //void remove_from_active_list(core_terminal_t &term) noexcept;


            // -----------------------------------------------------------------------------
            // setup stuff - cold
            // -----------------------------------------------------------------------------

            //bool is_logic() const noexcept;
            public bool is_analog() { return this is analog_net_t; }  //return dynamic_cast<const analog_net_t *>(this) != nullptr;


            public void rebuild_list()     // rebuild m_list after a load
            {
                // rebuild m_list

                m_list_active.clear();
                foreach (var term in exec().nlstate().core_terms(this))
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
                if (newQ != m_new_Q.op)
                {
                    m_new_Q.op = newQ;
                    push_to_queue(delay);
                }
            }


            // only used for logic nets
            //inline void set_Q_time(const netlist_sig_t &newQ, const netlist_time_ext &at) noexcept
        }
    } // namespace detail


    //class analog_net_t : public detail::net_t


    public class logic_net_t : detail.net_t
    {
        //using detail::net_t::Q;
        //using detail::net_t::initial;
        //using detail::net_t::set_Q_and_push;
        //using detail::net_t::set_Q_time;


        public logic_net_t(netlist_state_t nl, string aname, detail.core_terminal_t railterminal = null)
            : base(nl, aname, railterminal)
        {
        }
    }
} // namespace netlist
