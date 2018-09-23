// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using device_type = mame.emu.detail.device_type_impl_base;
using offs_t = System.UInt32;
using u8 = System.Byte;
using u32 = System.UInt32;
using uint32_t = System.UInt32;


namespace mame
{
    public static class watchdog_global
    {
        //#define MCFG_WATCHDOG_ADD(_tag)         MCFG_DEVICE_ADD(_tag, WATCHDOG_TIMER, 0)
        public static void MCFG_WATCHDOG_ADD(out device_t device, machine_config config, device_t owner, string tag) { mconfig_global.MCFG_DEVICE_ADD(out device, config, owner, tag, watchdog_timer_device.WATCHDOG_TIMER, 0); }
        //#define MCFG_WATCHDOG_MODIFY(_tag)         MCFG_DEVICE_MODIFY(_tag)
        public static void MCFG_WATCHDOG_VBLANK_INIT(device_t device, string screen, int count) { ((watchdog_timer_device)device).set_vblank_count(screen, count); }
        //#define MCFG_WATCHDOG_TIME_INIT(_time)         watchdog_timer_device::static_set_time(*device, _time);
    }


    // ======================> watchdog_timer_device
    public class watchdog_timer_device : device_t
    {
        //DEFINE_DEVICE_TYPE(WATCHDOG_TIMER, watchdog_timer_device, "watchdog", "Watchdog Timer")
        static device_t device_creator_watchdog_timer_device(device_type type, machine_config mconfig, string tag, device_t owner, u32 clock) { return new watchdog_timer_device(mconfig, tag, owner, clock); }
        public static readonly device_type WATCHDOG_TIMER = DEFINE_DEVICE_TYPE(device_creator_watchdog_timer_device, "watchdog", "Watchdog Timer");


        // configuration data
        int m_vblank_count; // number of VBLANKs until resetting the machine
        attotime m_time;         // length of time until resetting the machine
        string m_screen_tag;   // the tag of the screen this timer tracks

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
            m_screen_tag = null;
        }


        // inline configuration helpers
        //-------------------------------------------------
        //  static_set_vblank_count - configuration helper
        //  to set the number of VBLANKs
        //-------------------------------------------------
        public void set_vblank_count(string screen_tag, int count) { m_screen_tag = screen_tag; m_vblank_count = count; }
        //void set_time(attotime time) { m_time = time; }


        // watchdog control

        //-------------------------------------------------
        //  watchdog_reset - reset the watchdog timer
        //-------------------------------------------------
        void watchdog_reset()
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

        //WRITE8_MEMBER( watchdog_timer_device::reset_w )
        public void reset_w(address_space space, offs_t offset, u8 data, u8 mem_mask = 0xff) { watchdog_reset(); }


        //READ8_MEMBER( watchdog_timer_device::reset_r )
        public byte reset_r(address_space space, offs_t offset, u8 mem_mask = 0xff) { watchdog_reset(); return (byte)space.unmap(); }

        //DECLARE_WRITE16_MEMBER( reset16_w );
        //DECLARE_READ16_MEMBER( reset16_r );
        //DECLARE_WRITE32_MEMBER( reset32_w );
        //DECLARE_READ32_MEMBER( reset32_r );


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
                screen_device screen = siblingdevice<screen_device>(m_screen_tag);
                if (screen != null)
                    screen.register_vblank_callback(watchdog_vblank);
            }

            save_item(m_enabled, "m_enabled");
            save_item(m_counter, "m_counter");
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
