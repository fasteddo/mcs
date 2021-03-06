// license:BSD-3-Clause
// copyright-holders:Edward Fast

using System;
using System.Collections.Generic;

using u8 = System.Byte;
using u32 = System.UInt32;


namespace mame
{
    public delegate void driver_callback_delegate();


    public static class driver_global
    {
        //**************************************************************************
        //  CONFIGURATION MACROS
        //**************************************************************************

        // core machine callbacks
        public static void MCFG_MACHINE_START_OVERRIDE(machine_config config, driver_callback_delegate func) { driver_device.static_set_callback(config.root_device(), driver_device.callback_type.CB_MACHINE_START, func); }  //driver_callback_delegate(&_class::MACHINE_START_NAME(_func), #_class "::machine_start_" #_func, downcast<_class *>(owner)));
        public static void MCFG_MACHINE_RESET_OVERRIDE(machine_config config, driver_callback_delegate func) { driver_device.static_set_callback(config.root_device(), driver_device.callback_type.CB_MACHINE_RESET, func); }  //driver_callback_delegate(&_class::MACHINE_RESET_NAME(_func), #_class "::machine_reset_" #_func, downcast<_class *>(owner)));
        //define MCFG_MACHINE_RESET_REMOVE()             driver_device::static_set_callback(config.root_device(), driver_device::CB_MACHINE_RESET, driver_callback_delegate());

        // core video callbacks
        public static void MCFG_VIDEO_START_OVERRIDE(machine_config config, driver_callback_delegate func) { driver_device.static_set_callback(config.root_device(), driver_device.callback_type.CB_VIDEO_START, func); }  //driver_callback_delegate(&_class::VIDEO_START_NAME(_func), #_class "::video_start_" #_func, downcast<_class *>(owner)));
        //define MCFG_VIDEO_RESET_OVERRIDE(_class, _func)             driver_device::static_set_callback(config.root_device(), driver_device::CB_VIDEO_RESET, driver_callback_delegate(&_class::VIDEO_RESET_NAME(_func), #_class "::video_reset_" #_func, downcast<_class *>(owner)));
    }


    // ======================> driver_device
    // base class for machine driver-specific devices
    public class driver_device : device_t
    {
        // indexes into our generic callbacks
        public enum callback_type
        {
            CB_MACHINE_START,
            CB_MACHINE_RESET,
            CB_VIDEO_START,
            CB_VIDEO_RESET,
            CB_COUNT
        }


        // internal state
        game_driver m_system;                   // reference to the system description
        std.vector<string> m_searchpath = new std.vector<string>();           // media search path following parent/clone links
        driver_callback_delegate [] m_callbacks = new driver_callback_delegate[(int)callback_type.CB_COUNT];     // start/reset callbacks

        // generic video
        u8 m_flip_screen_x;
        u8 m_flip_screen_y;


        // construction/destruction

        //-------------------------------------------------
        //  driver_device - constructor
        //-------------------------------------------------

        public driver_device(machine_config mconfig, device_type type, string tag)
            : base(mconfig, type, tag, null, 0)
        {
            m_system = mconfig.gamedrv();
            m_flip_screen_x = 0;
            m_flip_screen_y = 0;


            // set the search path to include all parents and cache it because devices search system paths
            m_searchpath.emplace_back(m_system.name);
            std.set<game_driver> seen = new std.set<game_driver>();
            for (int ancestor = driver_list.clone(m_system); 0 <= ancestor; ancestor = driver_list.clone(ancestor))
            {
                if (!seen.insert(driver_list.driver(ancestor)))
                    throw new emu_fatalerror("driver_device({0}): parent/clone relationships form a loop", m_system.name);
                m_searchpath.emplace_back(driver_list.driver(ancestor).name);
            }
        }


        // getters
        public game_driver system() { return m_system; }


        // inline configuration helpers

        //-------------------------------------------------
        //  static_set_callback - set the a callback in
        //  the device configuration
        //-------------------------------------------------
        public static void static_set_callback(device_t device, callback_type type, driver_callback_delegate callback)
        {
            ((driver_device)device).m_callbacks[(int)type] = callback;
        }


        // dummy driver_init callback
        //-------------------------------------------------
        //  empty_init - default implementation which
        //  calls driver init
        //-------------------------------------------------
        public static void empty_init(device_t owner)
        {
            ((driver_device)owner).driver_init();
        }


        // output heler
        public output_manager output() { return machine().output(); }


        //-------------------------------------------------
        //  NMI callbacks
        //-------------------------------------------------
        //INTERRUPT_GEN_MEMBER( driver_device::nmi_line_pulse )   { device.execute().pulse_input_line(INPUT_LINE_NMI, attotime::zero); }
        public void nmi_line_pulse(device_t device) { device.execute().pulse_input_line(g.INPUT_LINE_NMI, attotime.zero); }

        //INTERRUPT_GEN_MEMBER( driver_device::nmi_line_assert )  { device.execute().set_input_line(INPUT_LINE_NMI, ASSERT_LINE); }
        void nmi_line_assert(device_t device) { device.execute().set_input_line(g.INPUT_LINE_NMI, g.ASSERT_LINE); }


        //INTERRUPT_GEN_MEMBER( driver_device::irq0_line_hold )   { device.execute().set_input_line(0, HOLD_LINE); }
        public void irq0_line_hold(device_t device) { device.execute().set_input_line(0, g.HOLD_LINE); }

        //void irq0_line_assert(device_t &device);

        //void irq1_line_hold(device_t &device);
        //void irq1_line_assert(device_t &device);

        //void irq2_line_hold(device_t &device);
        //void irq2_line_assert(device_t &device);

        //void irq3_line_hold(device_t &device);
        //void irq3_line_assert(device_t &device);

        //void irq4_line_hold(device_t &device);
        //void irq4_line_assert(device_t &device);

        //void irq5_line_hold(device_t &device);
        //void irq5_line_assert(device_t &device);

        //void irq6_line_hold(device_t &device);
        //void irq6_line_assert(device_t &device);

        //void irq7_line_hold(device_t &device);
        //void irq7_line_assert(device_t &device);


        //-------------------------------------------------
        //  searchpath - return cached search path
        //-------------------------------------------------
        public override std.vector<string> searchpath()
        {
            return m_searchpath;
        }


        public virtual void driver_init() { }


        // helpers called at startup
        protected virtual void driver_start() { }
        protected virtual void machine_start() { }
        protected virtual void sound_start() { }
        protected virtual void video_start() { }


        // helpers called at reset
        protected virtual void driver_reset() { }
        protected virtual void machine_reset() { }
        protected virtual void sound_reset() { }
        protected virtual void video_reset() { }


        // device-level overrides

        //-------------------------------------------------
        //  device_rom_region - return a pointer to the
        //  game's ROMs
        //-------------------------------------------------
        protected override Pointer<tiny_rom_entry> device_rom_region()
        {
            return new Pointer<tiny_rom_entry>(m_system.rom);
        }


        //-------------------------------------------------
        //  device_add_mconfig - add machine configuration
        //-------------------------------------------------
        protected override void device_add_mconfig(machine_config config)
        {
            m_system.machine_creator(config, this);
        }


        //-------------------------------------------------
        //  device_input_ports - return a pointer to the
        //  game's input ports
        //-------------------------------------------------
        protected override ioport_constructor device_input_ports()
        {
            return m_system.ipt;
        }

        //-------------------------------------------------
        //  device_start - device override which calls
        //  the various helpers
        //-------------------------------------------------
        protected override void device_start()
        {
            // reschedule ourselves to be last
            foreach (device_t test in new device_enumerator(this))
            {
                if (test != this && !test.started())
                    throw new device_missing_dependencies();
            }

            // call the game-specific init
            if (m_system.driver_init != null)
                m_system.driver_init(this);

            // finish image devices init process
            machine().image().postdevice_init();

            // start the various pieces
            driver_start();

            if (m_callbacks[(int)callback_type.CB_MACHINE_START] != null)
                m_callbacks[(int)callback_type.CB_MACHINE_START]();
            else
                machine_start();

            sound_start();

            if (m_callbacks[(int)callback_type.CB_VIDEO_START] != null)
                m_callbacks[(int)callback_type.CB_VIDEO_START]();
            else
                video_start();

            // save generic states
            save_item(NAME(new { m_flip_screen_x }));
            save_item(NAME(new { m_flip_screen_y }));
        }

        //-------------------------------------------------
        //  device_reset_after_children - device override
        //  which calls the various helpers; must happen
        //  after all child devices are reset
        //-------------------------------------------------
        protected override void device_reset_after_children()
        {
            // reset each piece
            driver_reset();

            if (m_callbacks[(int)callback_type.CB_MACHINE_RESET] != null)
                m_callbacks[(int)callback_type.CB_MACHINE_RESET]();
            else
                machine_reset();

            sound_reset();

            if (m_callbacks[(int)callback_type.CB_VIDEO_RESET] != null)
                m_callbacks[(int)callback_type.CB_VIDEO_RESET]();
            else
                video_reset();
        }


        // generic video

        //-------------------------------------------------
        //  flip_screen_set - set global flip
        //-------------------------------------------------
        public void flip_screen_set(u32 on)
        {
            // normalize to all 1
            if (on != 0)
                on = u32.MaxValue;  // ~0;

            // if something's changed, handle it
            if (m_flip_screen_x != on || m_flip_screen_y != on)
            {
                m_flip_screen_x = m_flip_screen_y = (byte)on;
                updateflip();
            }
        }

        //void flip_screen_x_set(u32 on);
        //void flip_screen_y_set(u32 on);
        public u32 flip_screen() { return m_flip_screen_x; }
        //u32 flip_screen_x() const { return m_flip_screen_x; }
        //u32 flip_screen_y() const { return m_flip_screen_y; }


        // helpers

        //-------------------------------------------------
        //  updateflip - handle global flipping
        //-------------------------------------------------
        void updateflip()
        {
            // push the flip state to all tilemaps
            machine().tilemap().set_flip_all((TILEMAP_FLIPX & m_flip_screen_x) | (TILEMAP_FLIPY & m_flip_screen_y));
        }
    }
}
