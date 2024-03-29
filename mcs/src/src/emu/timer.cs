// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;

using device_timer_id = System.UInt32;  //typedef u32 device_timer_id;
using s32 = System.Int32;
using u32 = System.UInt32;

using static mame.device_global;
using static mame.emucore_global;
using static mame.osdcore_global;
using static mame.timer_global;


namespace mame
{
    // ======================> timer_device
    public class timer_device : device_t
    {
        //DEFINE_DEVICE_TYPE(TIMER, timer_device, "timer", "Timer")
        public static readonly emu.detail.device_type_impl TIMER = DEFINE_DEVICE_TYPE("timer", "Timer", (type, mconfig, tag, owner, clock) => { return new timer_device(mconfig, tag, owner, clock); });


        // a timer callbacks look like this
        public delegate void expired_delegate(timer_device device, object ptr, int param);  //typedef device_delegate<void (timer_device &, void *, s32)> expired_delegate;


        // timer types
        enum timer_type
        {
            TIMER_TYPE_PERIODIC,
            TIMER_TYPE_SCANLINE,
            TIMER_TYPE_GENERIC
        }


        // configuration data
        timer_type m_type;             // type of timer
        expired_delegate m_callback;         // the timer's callback function
        object m_ptr;  //void *                  m_ptr;              // the pointer parameter passed to the timer callback

        // periodic timers only
        attotime m_start_delay;      // delay before the timer fires for the first time
        attotime m_period;           // period of repeated timer firings
        s32 m_param;            // the integer parameter passed to the timer callback

        // scanline timers only
        optional_device<screen_device> m_screen;    // pointer to the screen device
        u32 m_first_vpos;       // the first vertical scanline position the timer fires on
        u32 m_increment;        // the number of scanlines between firings

        // internal state
        emu_timer m_timer;            // the backing timer
        bool m_first_time;       // indicates that the system is starting (scanline timers only)


        // construction/destruction
        //-------------------------------------------------
        //  timer_device - constructor
        //-------------------------------------------------
        timer_device(machine_config mconfig, string tag, device_t owner, u32 clock)
            : base(mconfig, TIMER, tag, owner, clock)
        {
            m_type = timer_type.TIMER_TYPE_GENERIC;
            m_callback = null;
            m_ptr = null;
            m_start_delay = attotime.zero;
            m_period = attotime.zero;
            m_param = 0;
            m_screen = new optional_device<screen_device>(this, finder_base.DUMMY_TAG);
            m_first_vpos = 0;
            m_increment = 0;
            m_timer = null;
            m_first_time = true;
        }


        // inline configuration helpers

        //template <typename Object> void configure_generic(Object &&cb)
        //{
        //    m_type = TIMER_TYPE_GENERIC;
        //    m_callback = std::forward<Object>(cb);
        //}
        //template <class FunctionClass> void configure_generic(void (FunctionClass::*callback)(timer_device &, void *, s32), const char *name)
        //{
        //    configure_generic(expired_delegate(callback, name, nullptr, static_cast<FunctionClass *>(nullptr)));
        //}


        public void configure_periodic(expired_delegate cb, attotime period)  //template <typename Object> void configure_periodic(Object &&cb, const attotime &period)
        {
            m_type = timer_type.TIMER_TYPE_PERIODIC;
            m_callback = cb;  //m_callback = std::forward<Object>(cb);
            m_period = period;
        }

        //template <class FunctionClass> void configure_periodic(void (FunctionClass::*callback)(timer_device &, void *, s32), const char *name,
        //    const attotime &period)
        //{
        //    configure_periodic(expired_delegate(callback, name, nullptr, static_cast<FunctionClass *>(nullptr)), period);
        //}


        public void configure_scanline<bool_Required>(expired_delegate callback, device_finder<screen_device, bool_Required> screen, int first_vpos, int increment)  //template <typename F, typename U> void configure_scanline(F &&callback, const char *name, U &&screen, int first_vpos, int increment)
            where bool_Required : bool_const, new()
        {
            m_type = timer_type.TIMER_TYPE_SCANLINE;
            m_callback = callback;  //m_callback.set(std::forward<F>(callback), name);
            m_screen.set_tag(screen);  //m_screen.set_tag(std::forward<U>(screen));
            m_first_vpos = (u32)first_vpos;
            m_increment = (u32)increment;
        }

        public void configure_scanline(expired_delegate callback, string screen, int first_vpos, int increment)  //template <typename F, typename U> void configure_scanline(F &&callback, const char *name, U &&screen, int first_vpos, int increment)
        {
            m_type = timer_type.TIMER_TYPE_SCANLINE;
            m_callback = callback;  //m_callback.set(std::forward<F>(callback), name);
            m_screen.set_tag(screen);  //m_screen.set_tag(std::forward<U>(screen));
            m_first_vpos = (u32)first_vpos;
            m_increment = (u32)increment;
        }

        //template <typename T, typename F, typename U> void configure_scanline(T &&target, F &&callback, const char *name, U &&screen, int first_vpos, int increment)
        //{
        //    m_type = TIMER_TYPE_SCANLINE;
        //    m_callback.set(std::forward<T>(target), std::forward<F>(callback), name);
        //    m_screen.set_tag(std::forward<U>(screen));
        //    m_first_vpos = first_vpos;
        //    m_increment = increment;
        //}

        //template <typename Object> void set_callback(Object &&cb) { m_callback = std::forward<Object>(cb); }
        //template <class FunctionClass> void set_callback(void (FunctionClass::*callback)(timer_device &, void *, s32), const char *name)
        //{
        //    set_callback(expired_delegate(callback, name, nullptr, static_cast<FunctionClass *>(nullptr)));
        //}

        //void set_start_delay(const attotime &delay) { m_start_delay = delay; }
        //void config_param(int param) { m_param = param; }


        // property getters
        //int param() const { return m_timer->param(); }
        //void *ptr() const { return m_ptr; }
        //bool enabled() const { return m_timer->enabled(); }


        // property setters
        //void set_param(int param) { assert(m_type == TIMER_TYPE_GENERIC); m_timer->set_param(param); }
        //void set_ptr(void *ptr) { m_ptr = ptr; }
        //void enable(bool enable = true) { m_timer->enable(enable); }


        // adjustments
        //void reset() { adjust(attotime::never, 0, attotime::never); }
        //void adjust(const attotime &duration, INT32 param = 0, const attotime &period = attotime::never) { assert(m_type == TIMER_TYPE_GENERIC); m_timer->adjust(duration, param, period); }


        // timing information
        //attotime time_elapsed() const { return m_timer->elapsed(); }
        //attotime time_left() const { return m_timer->remaining(); }
        //attotime start_time() const { return m_timer->start(); }
        //attotime fire_time() const { return m_timer->expire(); }


        // device-level overrides

        //-------------------------------------------------
        //  device_validity_check - validate the device
        //  configuration
        //-------------------------------------------------
        protected override void device_validity_check(validity_checker valid)
        {
            // type based configuration
            switch (m_type)
            {
                case timer_type.TIMER_TYPE_GENERIC:
                    if (m_screen.finder_tag() != finder_base.DUMMY_TAG || m_first_vpos != 0 || m_start_delay != attotime.zero)
                        osd_printf_warning("Generic timer specified parameters for a scanline timer\n");
                    if (m_period != attotime.zero || m_start_delay != attotime.zero)
                        osd_printf_warning("Generic timer specified parameters for a periodic timer\n");
                    break;

                case timer_type.TIMER_TYPE_PERIODIC:
                    if (m_screen.finder_tag() != finder_base.DUMMY_TAG || m_first_vpos != 0)
                        osd_printf_warning("Periodic timer specified parameters for a scanline timer\n");
                    if (m_period <= attotime.zero)
                        osd_printf_error("Periodic timer specified invalid period\n");
                    break;

                case timer_type.TIMER_TYPE_SCANLINE:
                    if (m_period != attotime.zero || m_start_delay != attotime.zero)
                        osd_printf_warning("Scanline timer specified parameters for a periodic timer\n");
                    if (m_param != 0)
                        osd_printf_warning("Scanline timer specified parameter which is ignored\n");
//          if (m_first_vpos < 0)
//              osd_printf_error("Scanline timer specified invalid initial position\n");
//          if (m_increment < 0)
//              osd_printf_error("Scanline timer specified invalid increment\n");
                    break;

                default:
                    osd_printf_error("Invalid type specified\n");
                    break;
            }
        }


        //-------------------------------------------------
        //  device_start - perform device-specific
        //  startup
        //-------------------------------------------------
        protected override void device_start()
        {
            if (m_type == timer_type.TIMER_TYPE_SCANLINE)
                m_timer = timer_alloc(scanline_tick);
            else
                m_timer = timer_alloc(generic_tick);

            //throw new emu_unimplemented();
#if false
            m_callback.resolve();
#endif

            // register for save states
            save_item(NAME(new { m_first_time }));
        }


        //-------------------------------------------------
        //  device_reset - reset the device
        //-------------------------------------------------
        protected override void device_reset()
        {
            // type based configuration
            switch (m_type)
            {
                case timer_type.TIMER_TYPE_GENERIC:
                case timer_type.TIMER_TYPE_PERIODIC:
                {
                    // convert the period into attotime
                    attotime period;
                    if (m_period > attotime.zero)
                    {
                        period = m_period;

                        // convert the start_delay into attotime
                        attotime start_delay = attotime.zero;
                        if (m_start_delay > attotime.zero)
                            start_delay = m_start_delay;

                        // allocate and start the backing timer
                        m_timer.adjust(start_delay, m_param, period);
                    }
                    break;
                }

                case timer_type.TIMER_TYPE_SCANLINE:
                    if (m_screen == null)
                        fatalerror("timer '{0}': unable to find screen '{1}'\n", tag(), m_screen.finder_tag());

                    // set the timer to fire immediately
                    m_first_time = true;
                    m_timer.adjust(attotime.zero, m_param);
                    break;
            }
        }


        //TIMER_CALLBACK_MEMBER(generic_tick);
        void generic_tick(s32 param)
        {
            throw new emu_unimplemented();
        }


        //TIMER_CALLBACK_MEMBER(scanline_tick);
        void scanline_tick(s32 param)
        {
            throw new emu_unimplemented();
        }
    }


    public static class timer_global
    {
        public static timer_device TIMER(machine_config mconfig, string tag, u32 clock = 0) { return emu.detail.device_type_impl.op<timer_device>(mconfig, tag, timer_device.TIMER, clock); }
    }
}
