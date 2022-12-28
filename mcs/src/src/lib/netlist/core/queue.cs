// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using netlist_time_ext = mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>;  //using netlist_time_ext = plib::ptime<std::conditional<config::prefer_int128::value && plib::compile_info::has_int128::value, INT128, std::int64_t>::type, config::INTERNAL_RES::value>;
using queue_t = mame.netlist.detail.queue_base<mame.netlist.detail.net_t>;  //using queue_t = queue_base<device_arena, net_t>;  //using queue_t = queue_base<device_arena, net_t>;
using queue_t_entry_t = mame.plib.queue_entry_t<mame.plib.ptime<System.Int64, mame.plib.ptime_operators_int64, mame.plib.ptime_RES_config_INTERNAL_RES>, mame.netlist.detail.net_t>;  //using entry_t = plib::queue_entry_t<netlist_time_ext, O *>;
using size_t = System.UInt64;


namespace mame.netlist.detail
{
    // -----------------------------------------------------------------------------
    // queue_t
    // -----------------------------------------------------------------------------

    // We don't need a thread-safe queue currently. Parallel processing of
    // solvers will update inputs after parallel processing.

    //template <typename A, typename O>
    class queue_base<O> : plib.timed_queue_linear<plib.queue_entry_t<netlist_time_ext, O>, netlist_time_ext, O>,  //class queue_base : public config::timed_queue<A, plib::queue_entry_t<netlist_time_ext, O *>>, public plib::state_manager_t::callback_t  //using timed_queue = plib::timed_queue_linear<A, T>;
                          plib.state_manager_t.callback_t
    {
        //using entry_t = plib::queue_entry_t<netlist_time_ext, O *>;
        //using base_queue = config::timed_queue<A, entry_t>;
        public delegate size_t id_delegate(O obj);  //using id_delegate = plib::pmfp<std::size_t(const O *)>;
        public delegate O obj_delegate(size_t size);  //using obj_delegate = plib::pmfp<O *(std::size_t)>;


        size_t m_size;
        std.vector<Int64> m_times;  //std::vector<netlist_time_ext::internal_type> m_times;
        std.vector<size_t> m_net_ids;
        id_delegate m_get_id;  //id_delegate m_get_id;
        obj_delegate m_obj_by_id;  //obj_delegate m_obj_by_id;


        public queue_base(device_arena arena, size_t size, id_delegate get_id, obj_delegate get_obj)  //explicit queue_base(A &arena, std::size_t size, id_delegate get_id, obj_delegate get_obj)
            : base(size)  //: base_queue(arena, size)
        {
            m_size = 0;
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
            manager.save_item(this, m_size, module + "." + "size");
            manager.save_item(this, &m_times[0], module + "." + "times", m_times.size());
            manager.save_item(this, &m_net_ids[0], module + "." + "names", m_net_ids.size());
#endif
        }


        public void on_pre_save(plib.state_manager_t manager)
        {
            throw new emu_unimplemented();
#if false
            m_qsize = this->size();
            for (std::size_t i = 0; i < m_qsize; i++ )
            {
                m_times[i] =  this->list_pointer()[i].exec_time().as_raw();
                m_net_ids[i] = m_get_id(this->list_pointer()[i].object());
            }
#endif
        }


        public void on_post_load(plib.state_manager_t manager)
        {
            throw new emu_unimplemented();
#if false
            this->clear();
            for (std::size_t i = 0; i < m_size; i++ )
            {
                O *n = m_obj_by_id(m_net_ids[i]);
                this->template push<false>(entry_t(netlist_time_ext::from_raw(m_times[i]),n));
            }
#endif
        }
    }


    //using queue_t = queue_base<device_arena, net_t>;
}
