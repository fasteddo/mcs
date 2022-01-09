// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using static mame.cpp_global;


namespace mame
{
    class osd_watchdog
    {
        //osd_ticks_t                     m_timeout;
        //osd_event                       m_event;
        //std::atomic<std::int32_t>       m_do_exit;
        //std::unique_ptr<std::thread>    m_thread;


        public osd_watchdog()
        {
            //throw new emu_unimplemented();
#if false
            : m_timeout(60 * osd_ticks_per_second())
            , m_event(1, 0)
            , m_do_exit()
            , m_thread()

            m_thread.reset(new std::thread(&osd_watchdog::watchdog_thread, this));
#endif
        }

        ~osd_watchdog()
        {
            assert(m_isDisposed);  // can remove
        }

        bool m_isDisposed = false;
        public void Dispose()
        {
            //throw new emu_unimplemented();
#if false
            m_do_exit = 1;
            m_event.set();
            m_thread->join();
#endif

            m_isDisposed = true;
        }


        //osd_ticks_t     getTimeout(void) const { return m_timeout; }

        public void setTimeout(int timeout)
        {
            throw new emu_unimplemented();
        }


        public void reset()
        {
            throw new emu_unimplemented();
        }


        //static void *watchdog_thread(void *param);


        //void clear_event() { m_event.reset(); }
        //bool wait() { return m_event.wait(getTimeout()); }
        //INT32           do_exit(void) const { return m_do_exit; }
    }
}
