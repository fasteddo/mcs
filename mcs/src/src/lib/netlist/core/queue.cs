// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<NL_PREFER_INT128 && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using size_t = System.UInt64;


namespace mame.netlist
{
    namespace detail
    {
        // Use timed_queue_heap to use stdc++ heap functions instead of linear processing.
        // This slows down processing by about 35% on a Kaby Lake.
        // template <class T, bool TS>
        // using timed_queue = plib::timed_queue_heap<T, TS>;

        //template <class T, bool TS>
        public class timed_queue<T, bool_TS, U, V> : plib.timed_queue_linear<T, bool_TS, U, V>  //using timed_queue = plib::timed_queue_linear<T, TS>;
            where T : plib.pqentry_t<U, V>
            where bool_TS : bool_const, new()
            where U : netlist_time_ext
        {
            protected timed_queue(size_t list_size) : base(list_size) { }
        }


        // -----------------------------------------------------------------------------
        // queue_t
        // -----------------------------------------------------------------------------

        // We don't need a thread-safe queue currently. Parallel processing of
        // solvers will update inputs after parallel processing.

        //template <typename O, bool TS>
        class queue_base<O, bool_TS> : timed_queue<plib.pqentry_t<netlist_time_ext, O>, bool_TS, netlist_time_ext, O>,
                                       plib.state_manager_t.callback_t
            where bool_TS : bool_const, new()
        {
            //using entry_t = plib::pqentry_t<netlist_time_ext, O *>;
            //using base_queue = timed_queue<entry_t, false>;
            //using id_delegate = plib::pmfp<std::size_t, const O *>;
            //using obj_delegate = plib::pmfp<O *, std::size_t>;


            size_t m_qsize;
            std.vector<Int64> m_times;  //std::vector<netlist_time_ext::internal_type> m_times;
            std.vector<size_t> m_net_ids;
            Func<O, size_t> m_get_id;  //id_delegate m_get_id;
            Func<size_t, O> m_obj_by_id;  //obj_delegate m_obj_by_id;


            public queue_base(size_t size, Func<O, size_t> get_id, Func<size_t, O> get_obj)
                : base(size)  //: timed_queue<plib::pqentry_t<netlist_time_ext, O *>, false>(size)
            {
                m_qsize = 0;
                m_times = new std.vector<Int64>(size);
                m_net_ids = new std.vector<size_t>(size);
                m_get_id = get_id;
                m_obj_by_id = get_obj;
            }


            //~queue_base() noexcept override = default;

            //queue_base(const queue_base &) = delete;
            //queue_base(queue_base &&) = delete;
            //queue_base &operator=(const queue_base &) = delete;
            //queue_base &operator=(queue_base &&) = delete;


            public void register_state(plib.state_manager_t manager, string module)
            {
                throw new emu_unimplemented();
#if false
                manager.save_item(this, m_qsize, module + "." + "qsize");
                manager.save_item(this, &m_times[0], module + "." + "times", m_times.size());
                manager.save_item(this, &m_net_ids[0], module + "." + "names", m_net_ids.size());
#endif
            }


            public void on_pre_save(plib.state_manager_t manager)
            {
                throw new emu_unimplemented();
#if false
                plib::unused_var(manager);
                m_qsize = this->size();
                for (std::size_t i = 0; i < m_qsize; i++ )
                {
                    m_times[i] =  this->listptr()[i].exec_time().as_raw();
                    m_net_ids[i] = m_get_id(this->listptr()[i].object());
                }
#endif
            }


            public void on_post_load(plib.state_manager_t manager)
            {
                throw new emu_unimplemented();
#if false
                plib::unused_var(manager);
                this->clear();
                for (std::size_t i = 0; i < m_qsize; i++ )
                {
                    O *n = m_obj_by_id(m_net_ids[i]);
                    this->template push<false>(entry_t(netlist_time_ext::from_raw(m_times[i]),n));
                }
#endif
            }
        }

        //using queue_t = queue_base<net_t, false>;

    } // namespace detail
} // namespace netlist
