// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;


namespace mame.netlist
{
    class netlist_t //: public plib::plog_dispatch_intf
    {
        //P_PREVENT_COPYING(netlist_t)


        //pnamedlist_t<device_t *> m_devices;
        //net_t::list_t m_nets;
#if NL_KEEP_STATISTICS
        //pnamedlist_t<core_device_t *> m_started_devices;
#endif

#if NL_KEEP_STATISTICS
        // performance
        //int m_perf_out_processed;
        //int m_perf_inp_processed;
        //int m_perf_inp_active;
#endif


        //netlist_time                m_stop;     // target time for current queue processing

        //netlist_time                m_time;
        //bool                        m_use_deactivate;
        //queue_t                     m_queue;


        //devices::NETLIB_NAME(mainclock) *    m_mainclock;
        //devices::NETLIB_NAME(solver) *       m_solver;
        //devices::NETLIB_NAME(gnd) *          m_gnd;


        //devices::NETLIB_NAME(netlistparams) *m_params;
        //setup_t *m_setup;
        //plog_base<NL_DEBUG> m_log;


        // ----------------------------------------------------------------------------------------
        // netlist_base_t
        // ----------------------------------------------------------------------------------------
        netlist_t(string aname)
        {
            throw new emu_unimplemented();
        }


        ~netlist_t()
        {
            throw new emu_unimplemented();
        }


        //pstring name() const { return m_name; }


        //ATTR_COLD void start();
        //ATTR_COLD void stop();


        //ATTR_HOT  const queue_t &queue() const { return m_queue; }
        //ATTR_HOT  queue_t &queue() { return m_queue; }
        //ATTR_HOT  const netlist_time &time() const { return m_time; }
        //ATTR_HOT  devices::NETLIB_NAME(solver) *solver() const { return m_solver; }
        //ATTR_HOT  devices::NETLIB_NAME(gnd) *gnd() const { return m_gnd; }
        //ATTR_HOT nl_double gmin() const;


        //ATTR_HOT void push_to_queue(net_t &out, const netlist_time &attime);
        //ATTR_HOT void remove_from_queue(net_t &out);


        //ATTR_HOT void process_queue(const netlist_time &delta);
        //ATTR_HOT  void abort_current_queue_slice() { m_stop = netlist_time::zero; }


        //ATTR_HOT  const bool &use_deactivate() const { return m_use_deactivate; }


        //ATTR_COLD void rebuild_lists(); /* must be called after post_load ! */


        //ATTR_COLD void set_setup(setup_t *asetup) { m_setup = asetup;  }
        //ATTR_COLD setup_t &setup() { return *m_setup; }


        //ATTR_COLD net_t *find_net(const pstring &name);


#if false
        template<class _C>
        ATTR_COLD plist_t<_C *> get_device_list()
        {
            plist_t<_C *> tmp;
            for (std::size_t i = 0; i < m_devices.size(); i++)
            {
                _C *dev = dynamic_cast<_C *>(m_devices[i]);
                if (dev != NULL)
                    tmp.add(dev);
            }
            return tmp;
        }

        template<class _C>
        ATTR_COLD _C *get_first_device()
        {
            for (std::size_t i = 0; i < m_devices.size(); i++)
            {
                _C *dev = dynamic_cast<_C *>(m_devices[i]);
                if (dev != NULL)
                    return dev;
            }
            return NULL;
        }

        template<class _C>
        ATTR_COLD _C *get_single_device(const char *classname)
        {
            _C *ret = NULL;
            for (std::size_t i = 0; i < m_devices.size(); i++)
            {
                _C *dev = dynamic_cast<_C *>(m_devices[i]);
                if (dev != NULL)
                {
                    if (ret != NULL)
                        this->log().fatal("more than one {1} device found", classname);
                    else
                        ret = dev;
                }
            }
            return ret;
        }
#endif


        //ATTR_COLD plog_base<NL_DEBUG> &log() { return m_log; }
        //ATTR_COLD const plog_base<NL_DEBUG> &log() const { return m_log; }


        // any derived netlist must override vlog inherited from plog_base
        //  virtual void vlog(const plog_level &l, const pstring &ls) = 0;

        /* from netlist_object */
        //virtual void reset();
        //virtual void save_register();
    }
}
