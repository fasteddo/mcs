// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u8 = System.Byte;
using u16 = System.UInt16;
using u32 = System.UInt32;
using uint32_t = System.UInt32;


namespace mame
{
    // ======================> watchdog_timer_device
    public class watchdog_timer_device : device_t
    {
        //DEFINE_DEVICE_TYPE(WATCHDOG_TIMER, watchdog_timer_device, "watchdog", "Watchdog Timer")
        static device_t device_creator_watchdog_timer_device(emu.detail.device_type_impl_base type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new watchdog_timer_device(mconfig, tag, owner, clock); }
        public static readonly device_type WATCHDOG_TIMER = DEFINE_DEVICE_TYPE(device_creator_watchdog_timer_device, "watchdog", "Watchdog Timer");


        // configuration data
        int m_vblank_count; // number of VBLANKs until resetting the machine
        attotime m_time;         // length of time until resetting the machine
        optional_device<screen_device> m_screen; // the tag of the screen this timer tracks

        // internal state
        bool m_enabled;      // is the watchdog enabled?
        int m_counter;      // counter for VBLANK tracking
        emu_timer m_timer;        // timer for triggering reset


        // construction/destruction
        //-------------------------------------------------
        //  watchdog_timer_device - constructor
        //-------------------------------------------------
        watchdog_timer_device(machine_config mconfig, string tag, device_t owner, uint32_t clock = 0)
            : base(mconfig, WATCHDOG_TIMER, tag, owner, clock)
        {
            m_vblank_count = 0;
            m_time = attotime.zero;
            m_screen = new optional_device<screen_device>(this, finder_base.DUMMY_TAG);
        }


        // inline configuration helpers
        //-------------------------------------------------
        //  static_set_vblank_count - configuration helper
        //  to set the number of VBLANKs
        //-------------------------------------------------
        //template <typename T>
        public void set_vblank_count(string screen_tag, int count) { m_screen.set_tag(screen_tag); m_vblank_count = count; }  //void set_vblank_count(T &&screen_tag, int32_t count) { m_screen.set_tag(std::forward<T>(screen_tag)); m_vblank_count = count; }
        public void set_vblank_count(finder_base screen, int count) { m_screen.set_tag(screen); m_vblank_count = count; }  //void set_vblank_count(T &&screen_tag, int32_t count) { m_screen.set_tag(std::forward<T>(screen_tag)); m_vblank_count = count; }


        public void set_time(attotime time) { m_time = time; }


        // watchdog control

        //-------------------------------------------------
        //  watchdog_reset - reset the watchdog timer
        //-------------------------------------------------
        public void watchdog_reset()
        {
            // if we're not enabled, skip it
            if (!m_enabled)
                m_timer.adjust(attotime.never);

            // VBLANK-based watchdog?
            else if (m_vblank_count != 0)
                m_counter = m_vblank_count;

            // timer-based watchdog?
            else if (m_time != attotime.zero)
                m_timer.adjust(m_time);

            // default to an obscene amount of time (3 seconds)
            else
                m_timer.adjust(attotime.from_seconds(3));
        }


        //void watchdog_enable(bool enable = true);
        //int32_t get_vblank_counter() const { return m_counter; }


        // read/write handlers

        public void reset_w(u8 data = 0) { watchdog_reset(); }
        public u8 reset_r(address_space space) { watchdog_reset(); return (u8)space.unmap(); }

        public void reset16_w(u16 data = 0) { watchdog_reset(); }
        //u16 reset16_r(address_space &space);
        //void reset32_w(u32 data = 0);
        //u32 reset32_r(address_space &space);


        // device-level overrides

        protected override void device_validity_check(validity_checker valid) { throw new emu_unimplemented(); }


        //-------------------------------------------------
        //  device_start - perform device-specific
        //  startup
        //-------------------------------------------------
        protected override void device_start()
        {
            // initialize the watchdog
            m_counter = 0;
            m_timer = timer_alloc();

            if (m_vblank_count != 0)
            {
                // fetch the screen
                if (m_screen != null)
                    m_screen.op[0].register_vblank_callback(watchdog_vblank);
            }

            save_item(NAME(new { m_enabled }));
            save_item(NAME(new { m_counter }));
        }


        //-------------------------------------------------
        //  device_reset - reset the device
        //-------------------------------------------------
        protected override void device_reset()
        {
            // set up the watchdog timer; only start off enabled if explicitly configured
            m_enabled = (m_vblank_count != 0 || m_time != attotime.zero);
            watchdog_reset();
            m_enabled = true;
        }


        //public override void device_timer(emu_timer timer, device_timer_id id, int param, object ptr) { throw new emu_unimplemented(); }


        // internal helpers

        //-------------------------------------------------
        //  watchdog_fired - trigger machine reset
        //-------------------------------------------------
        void watchdog_fired()
        {
            logerror("Reset caused by the watchdog!!!\n");

            bool verbose = machine().options().verbose();
#if MAME_DEBUG
            verbose = true;
#endif
            if (verbose)
                popmessage("Reset caused by the watchdog!!!\n");

            machine().schedule_soft_reset();
        }


        //-------------------------------------------------
        //  watchdog_vblank - VBLANK state callback for
        //  watchdog timers
        //-------------------------------------------------
        void watchdog_vblank(screen_device screen, bool vblank_state)
        {
            // VBLANK starting
            if (vblank_state && m_enabled)
            {
                // check the watchdog
                if (m_vblank_count != 0)
                    if (--m_counter == 0)
                        watchdog_fired();
            }
        }
    }
}
