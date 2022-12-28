// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using log_type = mame.plib.plog_base<mame.netlist.nl_config_global.bool_const_NL_DEBUG>;  //using log_type =  plib::plog_base<NL_DEBUG>;
using netlist_time = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time = plib::ptime<std::int64_t, config::INTERNAL_RES::value>;
using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<config::prefer_int128::value && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using nl_fptype = System.Double;  //using nl_fptype = config::fptype;
using queue_t = mame.netlist.detail.queue_base<mame.netlist.detail.net_t>;  //using queue_t = queue_base<device_arena, net_t>;  //using queue_t = queue_base<device_arena, net_t>;
using queue_t_entry_t = mame.plib.queue_entry_t<mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>, mame.netlist.detail.net_t>;  //using entry_t = plib::queue_entry_t<netlist_time_ext, O *>;


namespace mame.netlist
{
    // -----------------------------------------------------------------------------
    // netlist_t
    // -----------------------------------------------------------------------------
    public class netlist_t // NOLINT(clang-analyzer-optin.performance.Padding)
    {
        netlist_state_t m_state;
        devices.nld_solver m_solver;

        // mostly rw
        netlist_time_ext m_time;
        devices.nld_mainclock m_main_clock;

        bool m_use_stats;
        queue_t m_queue;

        // performance
        //plib::pperftime_t<true>             m_stat_mainloop;
        //plib::pperfcount_t<true>            m_perf_out_processed;


        public netlist_t(netlist_state_t state, string aname)
        {
            m_state = state;
            m_solver = null;
            m_time = netlist_time_ext.zero();
            m_main_clock = null;
            m_use_stats = false;
            m_queue = new queue_t(state.pool(), config.max_queue_size,
                (net) => { return state.find_net_id(net); },  //detail::queue_t::id_delegate(&netlist_state_t :: find_net_id, &state),
                (id) => { return state.net_by_id(id); });  //detail::queue_t::obj_delegate(&netlist_state_t :: net_by_id, &state))


            state.save(this, (plib.state_manager_t.callback_t)m_queue, aname, "m_queue");
            state.save(this, m_time, aname, "m_time");
        }


        //netlist_t(const netlist_t &) = delete;
        //netlist_t &operator=(const netlist_t &) = delete;
        //netlist_t(netlist_t &&) noexcept = delete;
        //netlist_t &operator=(netlist_t &&) noexcept = delete;

        //virtual ~netlist_t() noexcept = default;


        // run functions

        public netlist_time_ext time() { return m_time; }


        public void process_queue(netlist_time_ext delta)
        {
            if (!m_use_stats)
            {
                process_queue_stats(false, delta);
            }
            else
            {
                throw new emu_unimplemented();
#if false
                var sm_guard = m_stat_mainloop.guard();
#endif

                process_queue_stats(true, delta);
            }
        }


        public void abort_current_queue_slice()
        {
            queue_remove(null);
            queue_push(new queue_t_entry_t(m_time, null));
        }


        //const detail::queue_t &queue() const noexcept { return m_queue; }


        //template<typename... Args>
        public void queue_push(queue_t_entry_t e)  //void queue_push(Args&&...args) noexcept
        {
            if (config.use_queue_stats && m_use_stats)
                m_queue.emplace<bool_const_true>(e);  //m_queue.emplace<true>(std::forward<Args>(args)...); // NOLINT(performance-move-const-arg)
            else
                m_queue.emplace<bool_const_false>(e);  //m_queue.emplace<false>(std::forward<Args>(args)...); // NOLINT(performance-move-const-arg)
        }


        //template <class R>
        public void queue_remove(detail.net_t elem)  //void queue_remove(R &&elem) noexcept
        {
            if (config.use_queue_stats && m_use_stats)
                m_queue.remove<bool_const_true>(elem);
            else
                m_queue.remove<bool_const_false>(elem);
        }

        // Control functions

        public void stop()
        {
            log().debug.op("Printing statistics ...\n");
            print_stats();
            log().debug.op("Stopping solver device ...\n");
            if (m_solver != null)
                m_solver.stop();
        }


        public void reset()
        {
            log().debug.op("Searching for main clock\n");
            m_main_clock = m_state.get_single_device<devices.nld_mainclock>("mainclock");

            log().debug.op("Searching for solver\n");
            m_solver = m_state.get_single_device<devices.nld_solver>("solver");

            // Don't reset time
            //m_time = netlist_time_ext::zero();
            m_queue.clear();
            if (m_main_clock != null)
                m_main_clock.m_Q.net().set_next_scheduled_time(m_time);
            //if (m_solver != nullptr)
            //  m_solver->reset();

            m_state.reset();
        }


        // only used by nltool to create static c-code
        //devices::nld_solver *solver() const noexcept { return m_solver; }

        // FIXME: force late type resolution
        //template <typename X = devices::nld_solver>
        public nl_fptype gmin(object solver = null)  //nl_fptype gmin([[maybe_unused]] X *solver = nullptr) const noexcept
        {
            return m_solver.gmin();  //return static_cast<X *>(m_solver)->gmin();
        }


        public netlist_state_t nl_state() { return m_state; }


        log_type log() { return m_state.log(); }


        void print_stats()
        {
            //throw new emu_unimplemented();
#if false
#endif
        }


        public bool use_stats() { return m_use_stats; }


        public bool stats_enabled() { return m_use_stats; }
        //void enable_stats(bool val) noexcept { m_use_stats = val; }


        //template <bool KEEP_STATS>
        void process_queue_stats(bool KEEP_STATS, netlist_time_ext delta)  //void process_queue_stats(netlist_time_ext delta) noexcept;
        {
            netlist_time_ext stop = m_time + delta;

            queue_push(new queue_t_entry_t(stop, null));

            if (m_main_clock == null)
            {
                m_time = m_queue.top().exec_time();
                detail.net_t obj = m_queue.top().object_();
                m_queue.pop();

                while (obj != null)
                {
                    obj.update_devs(KEEP_STATS);

                    //throw new emu_unimplemented();
#if false
                    if (KEEP_STATS)
                        m_perf_out_processed.inc();
#endif

                    queue_t_entry_t top = m_queue.top();
                    m_time = top.exec_time();
                    obj = top.object_();
                    m_queue.pop();
                }
            }
            else
            {
                logic_net_t mc_net = m_main_clock.m_Q.net();
                netlist_time inc = m_main_clock.m_inc;
                netlist_time_ext mc_time = mc_net.next_scheduled_time();

                do
                {
                    queue_t_entry_t top = m_queue.top();
                    while (top.exec_time() > mc_time)
                    {
                        m_time = mc_time;
                        mc_net.toggle_new_Q();
                        mc_net.update_devs(KEEP_STATS);
                        top = m_queue.top();
                        mc_time += inc;
                    }

                    m_time = top.exec_time();
                    detail.net_t obj = top.object_();
                    m_queue.pop();
                    if (!!(obj == null))
                        break;

                    obj.update_devs(KEEP_STATS);

                    //throw new emu_unimplemented();
#if false
                    if (KEEP_STATS)
                        m_perf_out_processed.inc();
#endif
                } while (true);

                mc_net.set_next_scheduled_time(mc_time);
            }
        }
    }
} // namespace netlist
